using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScoreModern {
 public static class NativeOutputLocation {
  const string Settings="output";

  public static string RequireWritableDestination(string root,string path){
   return Path.GetFullPath(path);
  }

  public static string Folder(string root){
   if(NativeSettings.Exists(Settings)){
    string chosen=NativeSettings.ReadAllText(Settings).Trim();
    if(Directory.Exists(chosen))return Path.GetFullPath(chosen);
   }
   return RequireWritableDestination(root,Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"SCORE Modern"));
  }

  public static string EnsureFolder(string root){
   string folder=Folder(root);
   Directory.CreateDirectory(folder);
   return folder;
  }

  public static string Resolve(string root,string path){
   return RequireWritableDestination(root,Path.IsPathRooted(path)?path:Path.Combine(EnsureFolder(root),path));
  }

  internal static void SetFolder(string root,string folder){
   folder=RequireWritableDestination(root,folder);
   if(!Directory.Exists(folder))throw new DirectoryNotFoundException(folder);
   NativeSettings.WriteAllText(Settings,folder);
  }
 }

 public sealed partial class Editor {
  void AddOutputFolderMenu(ContextMenuStrip menu){
   var item=new ToolStripMenuItem("Save and export folder...");
   item.Click+=(sender,args)=>Safe(()=>{
    using(var dialog=new FolderBrowserDialog{
     Description="Choose where SCORE Modern saves new scores, input files and exports",
     SelectedPath=NativeOutputLocation.Folder(source)
    }){
     if(dialog.ShowDialog(this)!=DialogResult.OK)return;
     NativeOutputLocation.SetFolder(source,dialog.SelectedPath);
     Message("Save and export folder: "+NativeOutputLocation.Folder(source));
    }
   });
   menu.Items.Add(item);
  }
 }
}
