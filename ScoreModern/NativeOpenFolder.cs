using System;using System.IO;using System.Text;
namespace ScoreModern {
 public static class NativeOpenFolder {
  static string remembered;
  static string Settings{get{return "open-folder";}}
  static NativeOpenFolder(){Load();}
  public static void Load(){remembered=null;try{if(NativeSettings.Exists(Settings))remembered=NativeSettings.ReadAllText(Settings).Trim();}catch(IOException){}catch(UnauthorizedAccessException){}}
  public static string InitialDirectory(string root){if(!String.IsNullOrEmpty(remembered)&&Directory.Exists(remembered))return remembered;string music=Path.Combine(root,"MUSDAT");return Directory.Exists(music)?music:root;}
  public static void Remember(string file,bool save){string folder=Path.GetDirectoryName(Path.GetFullPath(file));if(!Directory.Exists(folder))return;remembered=folder;if(save)NativeSettings.WriteAllText(Settings,folder);}
 }
}
