using System;using System.Drawing;using System.Globalization;using System.IO;using System.Windows.Forms;
namespace ScoreModern {
 sealed class PreferencesActionsMenu : ContextMenuStrip {
  internal void RefreshActions(){OnOpening(new System.ComponentModel.CancelEventArgs());}
 }
 public class ScoreDialog : Form {
  bool baselineApplied,closeTextOutputOnEscape;
  Font scaledSystemFont;
  protected override void OnHandleCreated(EventArgs e){base.OnHandleCreated(e);if(ShowInTaskbar)NativeDialogTaskbar.Set(Handle,sizeSetting);}
  protected override void Dispose(bool disposing){base.Dispose(disposing);if(disposing&&scaledSystemFont!=null){scaledSystemFont.Dispose();scaledSystemFont=null;}}
  readonly string sizeSetting;
  Size openedSize;
  bool restoredSize;
  public ScoreDialog():this(null){}
  internal ScoreDialog(string dialogName){
   sizeSetting="dialog-size."+(dialogName??GetType().Name);
   AutoScaleMode=AutoScaleMode.None;AutoScroll=true;
   var systemFont=SystemFonts.MessageBoxFont;float scale=NativeUiFont.WindowsTextScale;
   if(scale==1f)Font=systemFont;
   else Font=scaledSystemFont=new Font(systemFont.FontFamily,systemFont.SizeInPoints*scale,systemFont.Style,GraphicsUnit.Point);
  }
  static void SizeTableHeaders(Control parent){foreach(Control child in parent.Controls){var grid=child as DataGridView;if(grid!=null)grid.ColumnHeadersHeightSizeMode=DataGridViewColumnHeadersHeightSizeMode.AutoSize;SizeTableHeaders(child);}}
  void ApplyBaseline(){if(baselineApplied)return;baselineApplied=true;SizeTableHeaders(this);SuspendLayout();AutoScaleMode=AutoScaleMode.Dpi;AutoScaleDimensions=new SizeF(96,96);ResumeLayout(true);PerformAutoScale();}
  static void PrepareHandles(Control c){var handle=c.Handle;foreach(Control child in c.Controls)PrepareHandles(child);}
  internal void PrepareForDisplay(){ApplyBaseline();RestoreSize();FitWorkingArea();PrepareHandles(this);PerformLayout();}
  internal void AddTextOutputCloseButton(){
   var close=new Button{Text="Close",AutoSize=true,Anchor=AnchorStyles.Right};
   close.Click+=(s,e)=>Close();
   var footer=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(4)};
   footer.Controls.Add(close);
   Controls.Add(footer);
   SetTextOutputCancelButton(close);
  }
  internal void SetTextOutputCancelButton(Button close){
   ActiveControl=close;
   CancelButton=close;
   close.DialogResult=DialogResult.None;
   closeTextOutputOnEscape=true;
  }
  protected override bool ProcessCmdKey(ref Message message,Keys keyData){
   if(keyData==Keys.Escape&&closeTextOutputOnEscape){
    Close();
    return true;
   }
   return base.ProcessCmdKey(ref message,keyData);
  }
  static void InitializeTextOutputs(Control parent,bool initialSelection){
   foreach(Control child in parent.Controls){
    var text=child as TextBox;
    if(text!=null&&text.Multiline&&text.ReadOnly){
     if(initialSelection)text.Select(0,0);
     else{
      text.BackColor=NativeWindowsColors.TextOutputBackground;
      text.ForeColor=NativeWindowsColors.TextOutputText;
     }
    }
    InitializeTextOutputs(child,initialSelection);
   }
  }
  static void AddTextOutputMenus(Control parent){foreach(Control child in parent.Controls){var text=child as TextBox;if(text!=null&&text.Multiline&&text.ReadOnly&&text.ContextMenuStrip==null){var menu=new ContextMenuStrip();menu.Items.Add("Copy",null,(s,e)=>text.Copy());menu.Items.Add("Select all",null,(s,e)=>text.SelectAll());menu.Items.Add(new ToolStripSeparator());menu.Items.Add("Save as...",null,(s,e)=>{using(var file=new SaveFileDialog{Title="Save text output",Filter="Text files (*.txt)|*.txt|All files (*.*)|*.*",DefaultExt="txt",FileName="output.txt",RestoreDirectory=true})if(file.ShowDialog(text.FindForm())==DialogResult.OK)try{File.WriteAllText(file.FileName,text.Text,System.Text.Encoding.UTF8);}catch(Exception error){if(!(error is IOException||error is UnauthorizedAccessException))throw;MessageBox.Show(text.FindForm(),error.Message,"Save text output",MessageBoxButtons.OK,MessageBoxIcon.Error);}});text.ContextMenuStrip=menu;text.Disposed+=(s,e)=>menu.Dispose();}AddTextOutputMenus(child);}}
  protected override void OnLoad(EventArgs e){ApplyBaseline();InitializeTextOutputs(this,false);AddTextOutputMenus(this);base.OnLoad(e);}
  protected override void OnShown(EventArgs e){
   RestoreSize();
   FitWorkingArea();
   openedSize=LogicalSize(Size);
   base.OnShown(e);
   InitializeTextOutputs(this,true);
  }
  Size LogicalSize(Size pixels){
   float scale=NativeDisplay.Scale(this);
   return new Size((int)Math.Round(pixels.Width/scale),(int)Math.Round(pixels.Height/scale));
  }
  void RestoreSize(){
   try{
    string saved=NativeSettings.Read(sizeSetting);
    if(String.IsNullOrEmpty(saved))return;
    string[] parts=saved.Split(',');
    int width,height;
    if(parts.Length!=2||!Int32.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out width)||
     !Int32.TryParse(parts[1],NumberStyles.None,CultureInfo.InvariantCulture,out height)||width<=0||height<=0)return;
    double scale=NativeDisplay.Scale(this);
    Size work=Screen.FromControl(this).WorkingArea.Size;
    Size=new Size((int)Math.Min(work.Width,Math.Round(width*scale)),(int)Math.Min(work.Height,Math.Round(height*scale)));
    restoredSize=true;
   }catch(IOException){}catch(UnauthorizedAccessException){}
  }
  protected override void OnFormClosed(FormClosedEventArgs e){
   if(!openedSize.IsEmpty){
    Size current=LogicalSize(WindowState==FormWindowState.Normal?Size:RestoreBounds.Size);
    if(!current.IsEmpty&&(!restoredSize||current!=openedSize)){
     try{NativeSettings.WriteAllText(sizeSetting,current.Width.ToString(CultureInfo.InvariantCulture)+","+current.Height.ToString(CultureInfo.InvariantCulture));}
     catch(IOException){}catch(UnauthorizedAccessException){}
    }
   }
   base.OnFormClosed(e);
  }
  protected override void WndProc(ref Message m){base.WndProc(ref m);if(m.Msg==0x02e0&&Visible)FitWorkingArea();}
  void FitWorkingArea(){Rectangle area=Screen.FromControl(this).WorkingArea;Size desired=Size;Size limited=new Size(Math.Min(desired.Width,area.Width),Math.Min(desired.Height,area.Height));if(limited!=desired){AutoScrollMinSize=new Size(Math.Max(0,ClientSize.Width),Math.Max(0,ClientSize.Height));Size=limited;}if(Left<area.Left||Right>area.Right||Top<area.Top||Bottom>area.Bottom)Location=new Point(Math.Max(area.Left,Math.Min(Left,area.Right-Width)),Math.Max(area.Top,Math.Min(Top,area.Bottom-Height)));}
 }
 public static class OriginalScoreMenu {
  static bool Belongs(string text){return text.StartsWith("Printing:")||text.StartsWith("F7 files: ")||text.StartsWith("F7 folders: ")||text.StartsWith("F6 menu: ")||text.StartsWith("EDI: ")||text.StartsWith("TAB: ")||text.StartsWith("Z/ZZ ")||text.StartsWith("DPY:")||text=="Strict SCORE behaviour"||text.StartsWith("Font initialisation:")||text.StartsWith("Preferences:")||text.StartsWith("Startup:")||text.StartsWith("Mouse selection:")||text.StartsWith("Mouse click-points:")||text.StartsWith("Notation:")||text.StartsWith("DOS viewport")||text.StartsWith("Beams/slurs:")||text.StartsWith("Slurs:")||text.StartsWith("Editing:")||text.StartsWith("Grace-chord flips:")||text.StartsWith("Endings:")||text.StartsWith("Cache-only text")||text.StartsWith("Interface text:")||text.StartsWith("Use external VGA")||text.StartsWith("PostScript files:");}
  public static void Consolidate(ContextMenuStrip menu){var group=new ToolStripMenuItem("Original SCORE behaviour");var move=new System.Collections.Generic.List<ToolStripItem>();foreach(ToolStripItem item in menu.Items)if(Belongs(item.Text))move.Add(item);foreach(var item in move){menu.Items.Remove(item);group.DropDownItems.Add(item);}for(int n=menu.Items.Count-1;n>=0;n--)if(menu.Items[n]is ToolStripSeparator)menu.Items.RemoveAt(n);menu.Items.Insert(0,group);}
 }
}
