using System;using System.Globalization;using System.Drawing;using System.IO;using System.Windows.Forms;
namespace ScoreModern {
 public sealed partial class Editor {
  const string TextAreaFontSizeSetting="text-area-font-size";
  float textAreaFontPoints;
  void LoadTextAreaFontSize(){
   textAreaFontPoints=0;
   try{float points;if(Single.TryParse(NativeSettings.Read(TextAreaFontSizeSetting),NumberStyles.Float,CultureInfo.InvariantCulture,out points)&&points>=6&&points<=24)textAreaFontPoints=points;}
   catch(IOException){}catch(UnauthorizedAccessException){}
  }
  void SetTextAreaFontSize(float points,bool save){
   if(Single.IsNaN(points)||Single.IsInfinity(points)||points!=0&&(points<6||points>24))throw new ArgumentOutOfRangeException("points","Choose Automatic or 6–24 points.");
   if(save)NativeSettings.WriteAllText(TextAreaFontSizeSetting,points==0?"auto":points.ToString("0.##",CultureInfo.InvariantCulture));
   textAreaFontPoints=points;LayoutClassic();RefreshOriginalPrintMenu();UpdateLetterHelpOverlay();top.Invalidate(true);
  }
  void AddTextAreaFontSizeMenu(ContextMenuStrip menu){menu.Items.Add("Text area font size...",null,(s,e)=>Safe(ChooseTextAreaFontSize));}
  void ChooseTextAreaFontSize(){
   using(var dialog=new ScoreDialog("text-area-font-size")){
    dialog.Text="Text area font size";dialog.StartPosition=FormStartPosition.CenterParent;dialog.ClientSize=new Size(450,210);
    var body=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(12)};
    var automatic=new CheckBox{Text="Automatic (window width and Windows text size)",AutoSize=true,Checked=textAreaFontPoints==0};
    var line=new FlowLayoutPanel{AutoSize=true,WrapContents=false};line.Controls.Add(new Label{Text="Font size (points):",AutoSize=true,Margin=new Padding(0,6,8,0)});
    var size=new NumericUpDown{Minimum=6,Maximum=24,DecimalPlaces=1,Increment=.5m,Width=90,Value=(decimal)(textAreaFontPoints==0?12:textAreaFontPoints),Enabled=!automatic.Checked};line.Controls.Add(size);
    automatic.CheckedChanged+=(s,e)=>size.Enabled=!automatic.Checked;
    body.Controls.Add(automatic);body.Controls.Add(line);body.Controls.Add(new Label{AutoSize=true,MaximumSize=new Size(420,0),Text="Fixed sizes scale with Windows DPI. Smaller text leaves more room for the graphics. Lines remain whole. Automatic retains the existing window-based sizing."});
    var footer=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(8)};
    var cancel=new Button{Text="Cancel",AutoSize=true,DialogResult=DialogResult.Cancel};var apply=new Button{Text="Apply",AutoSize=true,DialogResult=DialogResult.OK};footer.Controls.Add(cancel);footer.Controls.Add(apply);
    dialog.Controls.Add(body);dialog.Controls.Add(footer);dialog.AcceptButton=apply;dialog.CancelButton=cancel;
    if(dialog.ShowDialog(this)==DialogResult.OK)SetTextAreaFontSize(automatic.Checked?0:(float)size.Value,true);
   }
  }
 }
}
