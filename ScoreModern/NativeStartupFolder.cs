using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScoreModern {
 static class NativeStartupFolder {
  internal static bool ReopenLast {get{return NativeSettings.Read("startup-reopen-last")=="true";}}
  internal static string InitialFile(string explicitFile){
   if(!String.IsNullOrEmpty(explicitFile))return explicitFile;
   string last=ReopenLast?NativeSettings.Read("startup-last-file"):null;
   return !String.IsNullOrEmpty(last)&&File.Exists(last)?last:null;
  }
  internal static void RememberFile(string path){
   if(Program.CheckMode||String.IsNullOrEmpty(path)||!File.Exists(path))return;
   try{NativeSettings.WriteAllText("startup-last-file",Path.GetFullPath(path));NativeRecentFiles.Remember(path);}
   catch(IOException){}catch(UnauthorizedAccessException){}
  }
  static string Settings {
   get { return "startup-folder"; }
  }

  internal static string Preferred {
   get {
    try{
     if(NativeSettings.Exists(Settings)){
      string saved=NativeSettings.ReadAllText(Settings).Trim();
      if(Path.IsPathRooted(saved))return Path.GetFullPath(saved);
     }
    }catch(IOException){}catch(UnauthorizedAccessException){}catch(ArgumentException){}catch(NotSupportedException){}
    return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
   }
  }

  internal static bool HasLibrary(string folder){
   return Directory.Exists(Path.Combine(folder,"LIB"))||
    Directory.Exists(NativeLibraryLocation.DirectoryFor(folder,false));
  }

  internal static string Choose(IWin32Window owner,string initial){
   using(var dialog=new FolderBrowserDialog{
    Description="Choose the startup SCORE folder containing LIB",
    SelectedPath=initial,
    ShowNewFolderButton=false
   }){
    return dialog.ShowDialog(owner)==DialogResult.OK?Path.GetFullPath(dialog.SelectedPath):null;
   }
  }

  internal static void Save(string folder){
   if(!HasLibrary(folder))throw new DirectoryNotFoundException("The startup SCORE folder must contain a LIB folder: "+folder);
   NativeScoreIdentity.Require(folder,true);
   NativeSettings.WriteAllText(Settings,Path.GetFullPath(folder));
  }
 }

 public sealed partial class Editor {
  void AddStartupFolderMenu(ContextMenuStrip menu){
   var item=new ToolStripMenuItem("Startup SCORE folder...");
   item.Click+=(sender,args)=>Safe(()=>{
    string folder=NativeStartupFolder.Choose(this,NativeStartupFolder.Preferred);
    if(folder==null)return;
    NativeStartupFolder.Save(folder);
    Message("Startup SCORE folder: "+folder+"\r\nUsed the next time you open SCORE Modern without a folder argument.");
   });
   menu.Items.Add(item);
   var reopen=new ToolStripMenuItem("Reopen last file on startup");
   reopen.Click+=(sender,args)=>Safe(()=>{NativeSettings.WriteAllText("startup-reopen-last",ReopenStartupValue());});
   menu.Items.Add(reopen);
   menu.Opening+=(sender,args)=>reopen.Checked=NativeStartupFolder.ReopenLast;
   menu.Opening+=(sender,args)=>item.ToolTipText=NativeStartupFolder.Preferred;
  }
  static string ReopenStartupValue(){return NativeStartupFolder.ReopenLast?"false":"true";}
 }
}
