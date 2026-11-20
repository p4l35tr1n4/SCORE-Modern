using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  Color inputHighlightColor=SystemColors.Highlight;
  static string InputHighlightSettingsPath {
   get {return "input-highlight";}
  }

  void AddInputHighlightMenu(ContextMenuStrip menu){
   try{
    int argb;
    if(NativeSettings.Exists(InputHighlightSettingsPath)&&Int32.TryParse(NativeSettings.ReadAllText(InputHighlightSettingsPath),out argb))
     inputHighlightColor=Color.FromArgb(argb);
   }catch(IOException){}catch(UnauthorizedAccessException){}

   var item=new ToolStripMenuItem("F2 field highlight colour...");
   item.Click+=(sender,args)=>Safe(()=>{
    using(var picker=new ColorDialog{Color=inputHighlightColor,FullOpen=true}){
     if(picker.ShowDialog(this)!=DialogResult.OK)return;
     NativeSettings.WriteAllText(InputHighlightSettingsPath,picker.Color.ToArgb().ToString(CultureInfo.InvariantCulture));
     inputHighlightColor=picker.Color;
     if(musicInput!=null)UpdateMusicInput();
    }
   });
   menu.Items.Add(item);
  }

  sealed class SetupValueBox:TextBox {
   protected override bool IsInputKey(Keys key){
    Keys code=key&Keys.KeyCode;
    return code==Keys.Enter||code==Keys.Escape||code==Keys.Up||code==Keys.Down||base.IsInputKey(key);
   }
  }
  sealed class InputFieldSpan {
   internal int Start,ValueStart,End;
   internal string Value;
  }
  readonly SetupValueBox setupValueBox=new SetupValueBox{Visible=false,AutoSize=false,BorderStyle=BorderStyle.FixedSingle};
  readonly System.Collections.Generic.Dictionary<NativeTextLabel,InputFieldSpan[]> setupSpans=
   new System.Collections.Generic.Dictionary<NativeTextLabel,InputFieldSpan[]>();
  Font setupValueFont;
  int setupEditField;
  string setupOriginalValue;

  void InitializeInputFieldEditing(){
   inputStaffLine.MouseClick+=(s,e)=>Safe(()=>ClickInputField((NativeTextLabel)s,1,e));
   contextLine.MouseClick+=(s,e)=>Safe(()=>ClickInputField((NativeTextLabel)s,6,e));
   setupValueBox.KeyDown+=(s,e)=>{
    Keys key=e.KeyCode;
    if((key==Keys.PageUp||key==Keys.PageDown)&&(e.Modifiers&(Keys.Control|Keys.Alt))==0){
     e.SuppressKeyPress=true;
     PanInputSetup(key,50);
     return;
    }
    if(key==Keys.End&&e.Modifiers==Keys.Control){
     e.SuppressKeyPress=true;
     if(!ApplySetupEditor())return;
     UndoInputPreset();
     FocusInputSetupField();
     return;
    }
    if(key!=Keys.Enter&&key!=Keys.Escape&&key!=Keys.Up&&key!=Keys.Down&&key!=Keys.Space&&
       (e.Modifiers!=Keys.None||key!=Keys.Left&&key!=Keys.Right))return;
    e.SuppressKeyPress=true;
    if(key==Keys.Escape){
     if(setupValueBox.Text!=setupOriginalValue){setupValueBox.Text=setupOriginalValue;setupValueBox.SelectAll();}
     else EscapeMusicInput();
     return;
    }
    if((key==Keys.Up||key==Keys.Down)&&NativeScoreBehaviour.Enabled){if((e.Modifiers&(Keys.Control|Keys.Alt))==0)PanInputSetup(key,5);return;}
    if(!ApplySetupEditor())return;
    if(key==Keys.Space){Safe(BeginInputStaff);return;}
    if(key==Keys.Left||key==Keys.Right){inputSetup.MoveField(key==Keys.Right?1:-1);UpdateMusicInput();}
    if(key==Keys.Up||key==Keys.Down)MoveInputFieldRow(key==Keys.Down?1:-1);
    FocusInputSetupField();
   };
   setupValueBox.KeyPress+=(s,e)=>{
    if(e.KeyChar!='/')return;
    e.Handled=true;
    if(!ApplySetupEditor())return;
    inputSetup.MoveField(1);
    UpdateMusicInput();
    FocusInputSetupField();
   };
   setupValueBox.LostFocus+=(s,e)=>CloseInputFieldEditor();
   Disposed+=(s,e)=>{setupValueBox.Dispose();if(setupValueFont!=null)setupValueFont.Dispose();};
  }
  bool ApplySetupEditor(){
   if(musicInput==null||musicInput.LetterInput||musicInput.Transaction!=null){CloseInputFieldEditor();return false;}
   try{
    inputSetup.Selected=setupEditField;
    string value=setupValueBox.Text.Trim();
    if(value.Length==0)throw new InvalidDataException("Enter a SCORE setup value.");
    if(!String.Equals(value,setupOriginalValue,StringComparison.Ordinal))ApplyInputField(value);
   }catch(ArgumentException error){Message(error.Message);return false;}
    catch(InvalidDataException error){Message(error.Message);return false;}
    catch(NotSupportedException error){Message(error.Message);return false;}
   CloseInputFieldEditor();
   UpdateMusicInput();
   return true;
  }
  static RectangleF InputFieldBounds(NativeTextLabel row,InputFieldSpan field,bool value=false){using(var graphics=row.CreateGraphics()){float cell;using(var fitted=row.FitInputFont(graphics,out cell))return NativeInterfaceText.TextSpan(graphics,row.Text,fitted??row.Font,value?field.ValueStart:field.Start,field.End-(value?field.ValueStart:field.Start),row.ClientRectangle);}}
  static float FieldCenter(NativeTextLabel row,InputFieldSpan field){var bounds=InputFieldBounds(row,field);return (bounds.Left+bounds.Right)/2;}
  void MoveInputFieldRow(int direction){
   bool topRow=inputSetup.Selected<=5;
   if(topRow==(direction<0))return;
   var from=(NativeTextLabel)(topRow?inputStaffLine:contextLine);
   var to=(NativeTextLabel)(topRow?contextLine:inputStaffLine);
   var old=setupSpans[from][inputSetup.Selected-(topRow?1:6)];
   float x=FieldCenter(from,old);
   var candidates=setupSpans[to];
   int nearest=0;
   for(int i=1;i<candidates.Length;i++)
    if(Math.Abs(FieldCenter(to,candidates[i])-x)<Math.Abs(FieldCenter(to,candidates[nearest])-x))nearest=i;
   inputSetup.Selected=(topRow?6:1)+nearest;
   UpdateMusicInput();
  }

  void CloseInputFieldEditor(){
   if(setupEditField==0)return;
   setupEditField=0;
   setupOriginalValue=null;
   setupValueBox.Hide();
  }
  void ClickInputField(NativeTextLabel row,int firstField,MouseEventArgs args){
   InputFieldSpan[] spans;
   if(args.Button!=MouseButtons.Left||musicInput==null||musicInput.LetterInput||musicInput.Transaction!=null||
      !setupSpans.TryGetValue(row,out spans))return;
   int index=-1;
   using(var graphics=row.CreateGraphics()){
    float cell;
    using(var fitted=row.FitInputFont(graphics,out cell)){
     if(cell<=0)return;
     for(int i=0;i<spans.Length;i++)
      if(NativeInterfaceText.TextSpan(graphics,row.Text,fitted??row.Font,spans[i].Start,spans[i].End-spans[i].Start,row.ClientRectangle).Contains(args.X,args.Y)){index=i;break;}
    }
   }
   if(index<0)return;
   CloseInputFieldEditor();
   CommitInputField();
   inputSetup.Selected=firstField+index;
   UpdateMusicInput();
   OpenInputFieldEditor(row,index);
  }
  void FocusInputSetupField(){
   if(musicInput==null||musicInput.LetterInput||musicInput.PositionsQuestion||musicInput.Transaction!=null)
    return;
   int first=inputSetup.Selected<=5?1:6;
   var row=(NativeTextLabel)(first==1?inputStaffLine:contextLine);
   if(row.Visible&&setupSpans.ContainsKey(row))OpenInputFieldEditor(row,inputSetup.Selected-first);
  }
  void OpenInputFieldEditor(NativeTextLabel row,int index){
   var field=setupSpans[row][index];
   using(var graphics=row.CreateGraphics()){
    float cell;
    var fitted=row.FitInputFont(graphics,out cell);
    var oldFont=setupValueFont;
    setupValueFont=fitted;
    setupValueBox.Font=fitted??row.Font;
    if(oldFont!=null)oldFont.Dispose();
    var bounds=NativeInterfaceText.TextSpan(graphics,row.Text,fitted??row.Font,field.ValueStart,field.End-field.ValueStart,row.ClientRectangle);int left=(int)Math.Floor(bounds.Left);
    int width=Math.Max(1,Math.Min(row.ClientSize.Width-left,(int)Math.Ceiling(bounds.Width+NativeInterfaceText.TextWidth(graphics," ",fitted??row.Font))));
    row.Controls.Add(setupValueBox);
    setupValueBox.Bounds=new Rectangle(left,0,width,row.ClientSize.Height);
   }
   setupValueBox.BackColor=inputHighlightColor;
   setupValueBox.ForeColor=inputHighlightColor.GetBrightness()>.5f?Color.Black:Color.White;
   setupOriginalValue=field.Value;
   setupValueBox.Text=field.Value;
   setupEditField=inputSetup.Selected;
   setupValueBox.Show();
   setupValueBox.BringToFront();
   setupValueBox.Focus();
   setupValueBox.SelectAll();
  }

  void SetInputRow(NativeTextLabel row,int firstField,string[] fields,string[] separators){
   row.FitInputRow=true;
   row.Cursor=musicInput.Transaction==null?Cursors.IBeam:Cursors.Default;
   row.HighlightColor=inputHighlightColor;
   row.HighlightLength=0;
   var text=new StringBuilder();
   var spans=new InputFieldSpan[fields.Length];
   for(int index=0;index<fields.Length;index++){
    if(index>0)text.Append(separators[index-1]);
    if(musicInput.Transaction==null&&inputSetup.Selected==firstField+index){
     row.HighlightStart=text.Length;
     row.HighlightLength=fields[index].Length;
    }
    string field=fields[index];
    int marker=field.IndexOf(" = ",StringComparison.Ordinal);
    int value=marker<0?0:marker+3;
    spans[index]=new InputFieldSpan{Start=text.Length,ValueStart=text.Length+value,
     End=text.Length+field.Length,Value=field.Substring(value)};
    text.Append(field);
   }
   setupSpans[row]=spans;
   row.Text=text.ToString();
   row.Invalidate();
  }

  void UpdateInputRows(){if(musicInput!=null&&musicInput.LetterInput){inputStaffLine.Visible=false;((NativeTextLabel)contextLine).FitInputRow=false;contextLine.Cursor=Cursors.Default;contextLine.Text=">"+musicInput.AcceptedHeader;return;}
   var culture=CultureInfo.InvariantCulture;
   string spacing=inputSetup.Spacing<0?"R":inputSetup.Spacing==0?"P":inputSetup.Spacing.ToString(culture);
   SetInputRow((NativeTextLabel)inputStaffLine,1,new[]{
    "Staff # = "+inputSetup.Staff.ToString(culture),
    "Left End = "+inputSetup.LeftEnd.ToString(culture),
    "Vert. Pos = "+inputSetup.Vertical.ToString(culture),
    "Size = "+inputSetup.Size.ToString("0.00",culture),
    "Right End = "+inputSetup.RightEnd.ToString(culture)
   },new[]{"   ","   ","   ","   "});
   SetInputRow((NativeTextLabel)contextLine,6,new[]{
    "Spacing = "+spacing,
    "Left Pos = "+inputSetup.LeftPosition.ToString(culture),
    "Right Pos = "+inputSetup.RightPosition.ToString(culture),
    "Meter = "+inputSetup.MeterTop.ToString(culture),
    inputSetup.MeterBottom.ToString(culture),
    "Key Sig. = "+inputSetup.Key
   },new[]{"   ","   ","   ","/","   "});
  }
 }
}
