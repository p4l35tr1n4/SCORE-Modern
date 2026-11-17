using System;using System.Linq;using System.Drawing;using System.IO;using System.Text;using System.Windows.Forms;
namespace ScoreModern {
 sealed class TextRowScrollPanel:Panel {
  internal int LinePitch=1;
  internal int AlignedOffset(int offset){return Math.Max(0,(int)Math.Round((double)offset/LinePitch)*LinePitch);}
  void SnapToRow(){
   int offset=-AutoScrollPosition.Y;
   int aligned=AlignedOffset(offset);
   if(offset!=aligned)AutoScrollPosition=new Point(0,aligned);
  }
  protected override void OnScroll(ScrollEventArgs e){base.OnScroll(e);SnapToRow();}
  protected override void OnMouseWheel(MouseEventArgs e){base.OnMouseWheel(e);SnapToRow();}
 }

 public sealed partial class Editor {
  const int DefaultTextRows=7;
  const string CompactTextSetting="compact-text";
  readonly TextRowScrollPanel textWindow=new TextRowScrollPanel();readonly Panel textBody=new Panel(),editHeader=new Panel();readonly Label itemTextLine=new NativeTextLabel(),inputStaffLine=new NativeTextLabel();readonly ToolTip resourcePathsTip=new ToolTip();Font resourceHeaderFont;
  readonly TextBox inlineText=new NativeCommandBox{Dock=DockStyle.Top,AutoSize=false,BorderStyle=BorderStyle.None,MaxLength=160,Visible=false};
  readonly NativeTextLabel inputCorrectionLine=new NativeTextLabel{Visible=false,Cursor=Cursors.IBeam};
  bool correctionInPlace;
  Item inlineTextTarget;
  bool inlineTextActive,inlineTextInsert;
  int textWindowRows=DefaultTextRows,textRowHeight=1;bool movingTextBoundary,showFallbackLibraryPath,compactText;
  bool inputFullGraphics,inputMenuHidden,inputDosTextBoundary;
  static string FallbackLibrarySetting {get{return "lib-status";}}
  void InitializeTextWindow(){
   LoadTextAreaFontSize();
   compactText=NativeSettings.Read(CompactTextSetting)=="on";
   try{showFallbackLibraryPath=NativeSettings.Exists(FallbackLibrarySetting)&&NativeSettings.ReadAllText(FallbackLibrarySetting).Trim()=="show";}
   catch(IOException){}catch(UnauthorizedAccessException){}
   prompt.Dock=DockStyle.Bottom;textWindow.Dock=DockStyle.Fill;textWindow.AutoScroll=true;textWindow.BackColor=ScreenColour;textBody.BackColor=ScreenColour;
   textBody.Controls.Add(responseLine);textBody.Controls.Add(contextLine);textBody.Controls.Add(inputStaffLine);textWindow.Controls.Add(textBody);
   editHeader.Dock=DockStyle.Top;editHeader.Height=0;itemTextLine.Dock=DockStyle.Top;editHeader.Controls.Add(itemTextLine);editHeader.Controls.Add(inlineText);editHeader.Controls.Add(parameters);
   textBody.Controls.Add(inputCorrectionLine);
   inputCorrectionLine.MouseClick+=(s,e)=>{
    if(e.Button!=MouseButtons.Left||!inputCorrectionLine.Visible||correctionInPlace)return;
    correctionInPlace=true;
    command.Dock=DockStyle.None;
    inputCorrectionLine.Controls.Add(command);
    UpdateCorrectionField();
    command.Focus();
    command.SelectionStart=command.TextLength;
   };
   command.TextChanged+=(s,e)=>{
    if(inputCorrectionLine.Visible&&!correctionInPlace&&musicInput!=null&&musicInput.Correction>=0)
     inputCorrectionLine.Text=CorrectionLabel(musicInput.Correction)+command.Text;
   };
   InitializeInputFieldEditing();inlineText.KeyPress+=(sender,args)=>{if(inlineTextActive)OriginalInlineTextCharacter(args);};
   itemTextLine.Cursor=Cursors.IBeam;
   itemTextLine.MouseClick+=(sender,args)=>{if(args.Button==MouseButtons.Left&&parameters.Item==canvas.Selected)EditText(args.Location);};
   top.Controls.Add(textWindow);top.Controls.Add(prompt);top.Controls.Add(editHeader);fileLine.Dock=DockStyle.Top;top.Controls.Add(fileLine);
   foreach(Control row in new Control[]{inputStaffLine,contextLine,responseLine})row.Dock=DockStyle.None;
   ((NativeTextLabel)responseLine).Wrapped=true;responseLine.TextChanged+=(s,e)=>{if(!layoutBusy)LayoutTextContents(true);};
  }
  void UpdateCorrectionField(){
   bool visible=musicInput!=null&&musicInput.Correction>=0&&!musicInput.AbandonQuestion;
   bool restoreFocus=false;
   if(!visible&&correctionInPlace){
    restoreFocus=command.Focused;
    correctionInPlace=false;
    prompt.Controls.Add(command);
    prompt.Controls.SetChildIndex(command,0);
    command.Dock=DockStyle.Fill;
   }
   inputCorrectionLine.Visible=visible;
   if(visible){
    inputCorrectionLine.Text=CorrectionLabel(musicInput.Correction)+(correctionInPlace?"":command.Text);
    PositionCorrectionEditor();
   }
   if(restoreFocus)command.Focus();
  }
  void PositionCorrectionEditor(){
   if(!correctionInPlace)return;
   int x;
   using(var graphics=inputCorrectionLine.CreateGraphics())
    x=(int)Math.Round(NativeInterfaceText.TextWidth(graphics,inputCorrectionLine.Text,inputCorrectionLine.Font));
   command.SetBounds(x,0,Math.Max(1,inputCorrectionLine.Width-x),inputCorrectionLine.Height);
  }
  void EditText(){EditText(null);}
  void EditText(Point? caret){
   if(letterHelpPages!=null||drawExportStage!=DrawExportStage.None||pendingParameterExport||pendingDriveChange||pendingDiskDelete!=null||pendingDiskFilename!=null||pendingLetterSave!=null||pendingLetterLoad!=null){command.Focus();return;}
   if(musicInput!=null||score==null)return;
   if(inlineTextActive){inlineText.Focus();return;}
   inlineTextTarget=canvas.Selected!=null&&canvas.Selected.Code==16?canvas.Selected:null;
   inlineText.Text=inlineTextTarget==null?"_00New text":inlineTextTarget.Text.TrimEnd('\0',' ');
   inlineTextActive=true;inlineTextInsert=false;
   UpdateClassic();
   inlineText.Focus();
   if(caret.HasValue)inlineText.SelectionStart=inlineText.GetCharIndexFromPosition(caret.Value);
   else inlineText.SelectionStart=inlineText.TextLength;
   Message("Edit SCORE text in place. Enter completes text editing.");
  }
  int InlineTextPrefix(){int font;string text=inlineText.Text;if(ScoreText.FontCode(text,0,out font))return 3;if(text.StartsWith("__",StringComparison.Ordinal)&&ScoreText.FontCode(text,1,out font))return 4;return 0;}
  bool OriginalInlineTextKey(KeyEventArgs e){if(!inlineTextActive)return false;if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;FinishInlineText(true);}else if(e.KeyCode==Keys.Escape){e.SuppressKeyPress=true;}else if(e.KeyCode==Keys.Insert){e.SuppressKeyPress=true;inlineTextInsert=!inlineTextInsert;}else if(e.KeyCode==Keys.Home){e.SuppressKeyPress=true;inlineText.Select(InlineTextPrefix(),0);}else if(e.KeyCode==Keys.End){e.SuppressKeyPress=true;inlineText.Select(inlineText.TextLength,0);}else if(e.Modifiers==Keys.None&&(e.KeyCode==Keys.Left||e.KeyCode==Keys.Right)){e.SuppressKeyPress=true;inlineText.Select(Math.Max(0,Math.Min(inlineText.TextLength,inlineText.SelectionStart+(e.KeyCode==Keys.Left?-1:1))),0);}else if(e.KeyCode==Keys.Delete||e.KeyCode==Keys.Back){e.SuppressKeyPress=true;int at=inlineText.SelectionStart,count=inlineText.SelectionLength;if(count==0&&e.KeyCode==Keys.Back&&at>0){at--;count=1;}else if(count==0&&e.KeyCode==Keys.Delete&&at<inlineText.TextLength)count=1;if(count>0){inlineText.Text=inlineText.Text.Remove(at,count);inlineText.Select(at,0);}}return true;}
  void OriginalInlineTextCharacter(KeyPressEventArgs e){if(Char.IsControl(e.KeyChar))return;e.Handled=true;int at=inlineText.SelectionStart,remove=inlineText.SelectionLength;if(!inlineTextInsert&&remove==0&&at<inlineText.TextLength)remove=1;string next=inlineText.Text.Remove(at,Math.Min(remove,inlineText.TextLength-at)).Insert(at,e.KeyChar.ToString());if(next.Length>inlineText.MaxLength)return;inlineText.Text=next;inlineText.Select(at+1,0);}
  void FinishInlineText(bool apply){
   if(!inlineTextActive)return;
   if(apply){
    try{
     string value=inlineText.Text;
     if(value.IndexOfAny(new[]{'\r','\n'})>=0)throw new InvalidDataException("Use one SCORE text item per line.");
     if(inlineTextTarget!=null&&canvas.Selected!=inlineTextTarget)
      throw new InvalidOperationException("The selected text item changed. Press Esc to cancel this edit.");
     if(inlineTextTarget==null||value!=inlineTextTarget.Text.TrimEnd('\0',' '))ApplyTextValue(value);
    }catch(InvalidDataException error){Message(error.Message);return;}
     catch(NotSupportedException error){Message(error.Message);return;}
     catch(InvalidOperationException error){Message(error.Message);return;}
     catch(IOException error){Message(error.Message);return;}
   }
   inlineTextActive=false;
   inlineTextTarget=null;
   UpdateClassic();
   Message("");
   command.Focus();
  }
  void AddCompactTextMenu(ContextMenuStrip menu){
   var item=new ToolStripMenuItem("Compact text"){Checked=compactText};
   item.Click+=(sender,args)=>Safe(()=>{
    bool compact=!compactText;
    NativeSettings.WriteAllText(CompactTextSetting,compact?"on":"off");
    compactText=compact;
    item.Checked=compact;
    LayoutClassic();
    top.Invalidate(true);
   });
   menu.Items.Add(item);
  }
  void AddFallbackLibraryPathMenu(ContextMenuStrip menu){
   var item=new ToolStripMenuItem("Show fallback LIB path in status line"){Checked=showFallbackLibraryPath};
   item.Click+=(sender,args)=>Safe(()=>{
    bool show=!showFallbackLibraryPath;
    NativeSettings.WriteAllText(FallbackLibrarySetting,show?"show":"hide");
    showFallbackLibraryPath=show;
    item.Checked=show;
    UpdatePinnedHeader();
   });
   menu.Items.Add(item);
  }
  void UpdatePinnedHeader(){
   if(String.IsNullOrEmpty(source))return;
   string file=String.IsNullOrEmpty(current)?"SCORE.MUS":System.IO.Path.GetFileName(current),drawings=NativeLibraryLocation.DirectoryFor(source,true),metrics=NativeLibraryLocation.DirectoryFor(source,false),normal=System.IO.Path.Combine(source,"LIB"),fonts=NativeTupletFonts.DirectoryPath;
   string fallback=showFallbackLibraryPath&&!String.Equals(drawings.TrimEnd('\\','/'),normal.TrimEnd('\\','/'),StringComparison.OrdinalIgnoreCase)
    ?" [fallback LIB: "+normal+"]":"";
   string text=(dirty?"* ":"")+file+"  Items="+(score==null?0:score.Items.Count)+"  Zoom="+canvas.Zoom.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture)+"  LIB: "+drawings+fallback+(String.Equals(drawings.TrimEnd('\\','/'),metrics.TrimEnd('\\','/'),StringComparison.OrdinalIgnoreCase)?"":"  Metrics: "+metrics)+"  Fonts: "+(String.IsNullOrEmpty(fonts)?"(not selected)":fonts);
   fileLine.Text=text;fileLine.Visible=true;resourcePathsTip.SetToolTip(fileLine,text);
   float size=classicFont==null?12:classicFont.Size;int width=Math.Max(1,top.ClientSize.Width);while(size>7){using(var font=NativeUiFont.Create(size)){if(TextRenderer.MeasureText(text,font,Size.Empty,TextFormatFlags.NoPadding|TextFormatFlags.SingleLine).Width<=width)break;}size-=.5f;}
   if(!NativeUiFont.Matches(resourceHeaderFont)||Math.Abs(resourceHeaderFont.Size-size)>.01f){var old=resourceHeaderFont;resourceHeaderFont=NativeUiFont.Create(size);fileLine.Font=resourceHeaderFont;if(old!=null)old.Dispose();}
  }
  void LayoutTextContents(bool reveal){
   UpdatePinnedHeader();if(textWindow.ClientSize.Width<1)return;int scrollY=Math.Max(0,-textWindow.AutoScrollPosition.Y);bool atEnd=scrollY+textWindow.ClientSize.Height>=textWindow.AutoScrollMinSize.Height-1;
   editHeader.Height=(parameters.Visible?parameters.Height:0)+(inlineText.Visible?inlineText.Height:itemTextLine.Visible?itemTextLine.Height:0);
   int available=Math.Max(1,textWindow.ClientSize.Width-SystemInformation.VerticalScrollBarWidth);responseLine.Height=String.IsNullOrEmpty(responseLine.Text)?0:Math.Max(textRowHeight,((NativeTextLabel)responseLine).ContentHeight(available));
   var rows=new Control[]{inputStaffLine,contextLine,responseLine,inputCorrectionLine};int height=rows.Where(c=>c.Visible).Sum(c=>c.Height),width=Math.Max(1,textWindow.ClientSize.Width-(height>textWindow.ClientSize.Height?SystemInformation.VerticalScrollBarWidth:0)),y=0;foreach(Control control in rows){if(!control.Visible)continue;control.SetBounds(0,y,control==responseLine?available:width,control.Height);y+=control.Height;}textBody.Size=new Size(width,height);
   PositionCorrectionEditor();
   textWindow.AutoScrollMinSize=new Size(0,height);textWindow.AutoScrollPosition=new Point(0,reveal||atEnd?Math.Max(0,height-textWindow.ClientSize.Height):textWindow.AlignedOffset(scrollY));
  }
  void LayoutTextWindow(int row){UpdateGroupWindowState();textRowHeight=row;textWindow.LinePitch=row;((NativeTextLabel)responseLine).LinePitch=row;inputCorrectionLine.Height=row;inputCorrectionLine.Font=classicFont;inputStaffLine.Height=row;inputStaffLine.Font=classicFont;inputStaffLine.Visible=musicInput!=null&&!musicInput.LetterInput;top.Height=prompt.Height+fileLine.Height+(textWindowRows-2)*row;top.Visible=!inputFullGraphics||mode==ScreenMode.EditGroupMenu&&!NativeKeyboardOptions.GroupFull||postGroupGraphicsTop>=0;
   functionStrip.Visible=!inputFullGraphics&&!inputMenuHidden;
   canvas.DosFullGraphics=inputFullGraphics;
   canvas.DosClipBottom=(short)(inputFullGraphics?479:inputMenuHidden?478:Scor4MenuActive?446:461);
   canvas.DosTextClipOverride=postGroupGraphicsTop>=0?postGroupGraphicsTop:inputFullGraphics?0:inputDosTextBoundary?97+16*(textWindowRows-DefaultTextRows):-1;
   canvas.CropTop=NativeNotationView.IsDosViewport&&(canvas.DosTextClipOverride>=0||canvas.DosGroupHeader)
    ?(int)Math.Round((canvas.DosClipTop-97)*canvas.RenderScale)
    :inputFullGraphics?0:(textWindowRows-DefaultTextRows)*row;
   LayoutTextContents(false);}
  bool HandleInputDisplayKey(Keys keys){
   if(mode==ScreenMode.Print&&UsesOriginalPrintMenu)return false;
   // Main and input share the measured original graphics-window controls.
    if((keys&Keys.Modifiers)!=Keys.Alt)return false;
   switch(keys&Keys.KeyCode){
    case Keys.C:inputDosTextBoundary=true;SetTextBoundary(20);return true;
    case Keys.O:inputDosTextBoundary=true;SetTextBoundary(6);return true;
    case Keys.U:inputDosTextBoundary=true;MoveTextBoundary(-1);return true;
     case Keys.D:inputDosTextBoundary=true;MoveTextBoundary(1);return true;
    case Keys.M:inputMenuHidden=!inputMenuHidden;break;
    case Keys.F:if(originalTextSetupActive&&!NativeKeyboardOptions.TextSetupFull||mode==ScreenMode.EditGroupMenu&&!NativeKeyboardOptions.GroupFull)return true;postGroupGraphicsTop=-1;inputFullGraphics=!inputFullGraphics;break;
    default:return false;
   }
   NativeDosViewport.DisplayChanged();
   movingTextBoundary=true;
   try{LayoutClassic();}finally{movingTextBoundary=false;}
   if(inputFullGraphics)canvas.Focus();else command.Focus();
   return true;
  }
  void MoveTextBoundary(int direction){SetTextBoundary(Math.Max(inputDosTextBoundary?6:DefaultTextRows,Math.Min(inputDosTextBoundary?20:19,textWindowRows+direction)));}
  void SetTextBoundary(int next){bool hadPostGroup=postGroupGraphicsTop>=0;postGroupGraphicsTop=-1;if(next==textWindowRows&&!hadPostGroup)return;Point scroll=viewport.AutoScrollPosition;int textScroll=Math.Max(0,-textWindow.AutoScrollPosition.Y);bool atEnd=textScroll+textWindow.ClientSize.Height>=textWindow.AutoScrollMinSize.Height-1;textWindowRows=next;movingTextBoundary=true;try{LayoutClassic();LayoutTextContents(atEnd);if(!atEnd)textWindow.AutoScrollPosition=new Point(0,textWindow.AlignedOffset(textScroll));viewport.AutoScrollPosition=new Point(-scroll.X,-scroll.Y);}finally{movingTextBoundary=false;}canvas.Invalidate();}
 }
}


