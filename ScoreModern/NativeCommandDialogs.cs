using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  static bool ListDialogs { get { return !NativeScoreBehaviour.Enabled && NativeSettings.Read("command-list-dialogs")=="true"; } }

  void ShowCopyableList(string title,string key,string content) {
   using(var font=new Font("Consolas",10))
   using(var dialog=new ScoreDialog(key){Text=title,Width=950,Height=650,StartPosition=FormStartPosition.CenterParent}) {
    var text=new TextBox{Dock=DockStyle.Fill,Multiline=true,ReadOnly=true,ScrollBars=ScrollBars.Both,WordWrap=false,Font=font,Text=content};
    var menu=new ContextMenuStrip();menu.Items.Add("Copy",null,(s,e)=>text.Copy());menu.Items.Add("Select all",null,(s,e)=>text.SelectAll());menu.Items.Add(new ToolStripSeparator());menu.Items.Add("Save as...",null,(s,e)=>{using(var save=new SaveFileDialog{Title="Save text output",Filter=TextFileFilter+"|All files (*.*)|*.*",DefaultExt="txt",FileName="output.txt",RestoreDirectory=true})if(save.ShowDialog(dialog)==DialogResult.OK)Safe(()=>File.WriteAllText(save.FileName,text.Text,System.Text.Encoding.UTF8));});text.ContextMenuStrip=menu;dialog.Controls.Add(text);dialog.Disposed+=(s,e)=>menu.Dispose();
    dialog.ShowDialog(this);
   }
  }

  bool ShowCommandList(string title,IEnumerable<string[]> pages) {
   if(!ListDialogs)return false;
   var lines=pages.SelectMany(page=>page.Take(page.Length-(page.LastOrDefault()=="Press any key to continue."?1:0)));
   ShowCopyableList(title,"command-list",String.Join("\r\n",lines));
   return true;
  }

  string ChooseCommandFile(string title,bool save,string filter,string name="",bool confirmOverwrite=true) {
   using(FileDialog dialog=save?(FileDialog)new SaveFileDialog():new OpenFileDialog()) {
    if(save)((SaveFileDialog)dialog).OverwritePrompt=confirmOverwrite;
    dialog.Title=title;
    dialog.Filter=filter+"|All files (*.*)|*.*";
    dialog.InitialDirectory=Directory.GetCurrentDirectory();
    dialog.RestoreDirectory=true;
    dialog.FileName=name;
    return dialog.ShowDialog(this)==DialogResult.OK?dialog.FileName:null;
   }
  }

  const string TextFileFilter="Text files (*.txt)|*.txt";
  const string ScoreFileFilter="SCORE documents (*.mus;*.pag)|*.mus;*.pag";

  void AddCommandOutputPreferences(ContextMenuStrip menu) {
   var listing=new ToolStripMenuItem("Show command lists in copyable dialogs");
   listing.Click+=(s,e)=>NativeSettings.WriteAllText("command-list-dialogs",ListDialogs?"false":"true");
   var recent=new ToolStripMenuItem("Recent files shown...");
   recent.Click+=(s,e)=>{
    using(var dialog=new ScoreDialog("recent-file-count"){Text="Recent files shown",AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,StartPosition=FormStartPosition.CenterParent}) {
     var panel=new FlowLayoutPanel{AutoSize=true,Padding=new Padding(12)};
     var count=new NumericUpDown{Minimum=0,Maximum=NativeRecentFiles.Capacity,Value=NativeRecentFiles.Count};
     var ok=new Button{Text="OK",DialogResult=DialogResult.OK};
     var cancel=new Button{Text="Cancel",DialogResult=DialogResult.Cancel};
     panel.Controls.AddRange(new Control[]{count,ok,cancel});
     dialog.Controls.Add(panel);
     dialog.AcceptButton=ok;
     dialog.CancelButton=cancel;
     if(dialog.ShowDialog(this)==DialogResult.OK)
      NativeSettings.WriteAllText("recent-file-count",count.Value.ToString(CultureInfo.InvariantCulture));
    }
   };
   menu.Items.Add(listing);
   menu.Items.Add(recent);
   menu.Opening+=(s,e)=>{listing.Enabled=!NativeScoreBehaviour.Enabled;listing.Checked=ListDialogs;recent.ToolTipText=NativeRecentFiles.Count.ToString(CultureInfo.InvariantCulture);};
  }
 }

 static class NativeRecentFiles {
  internal const int Capacity=50;
  internal static int Count {
   get {
    int count;
    return Int32.TryParse(NativeSettings.Read("recent-file-count"),out count)?Math.Max(0,Math.Min(Capacity,count)):10;
   }
  }
  static IEnumerable<string> History() {
   return (NativeSettings.Read("recent-files")??"").Split(new[]{'\n'},StringSplitOptions.RemoveEmptyEntries);
  }
  internal static IEnumerable<string> Visible() { return History().Where(File.Exists).Take(Count); }
  internal static void Remember(string path) {
   path=Path.GetFullPath(path);
   var files=new[]{path}.Concat(History()).Distinct(StringComparer.OrdinalIgnoreCase).Take(Capacity);
   NativeSettings.WriteAllText("recent-files",String.Join("\n",files));
  }
 }
}
