using System;using System.IO;using System.Text;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeDrawingNames {
  static bool documented;
  public static bool Documented{get{return !NativeScoreBehaviour.Enabled&&documented;}}
  public static void Load(){documented=NativeSettings.Exists("drawing-names")&&NativeSettings.ReadAllText("drawing-names").Trim()=="documented";}
  public static void Set(bool value,bool save){documented=value;NativeDosViewport.DisplayChanged();if(save)NativeSettings.WriteAllText("drawing-names",value?"documented":"original");}
  internal static string Name(Item item){
   string raw=item.ExternalDrawingNameOverride==null?Encoding.GetEncoding(437).GetString(item.Bytes,56,Documented?24:6):item.ExternalDrawingNameOverride.Substring(0,Documented?24:6);int zero=raw.IndexOf('\0');
   if(!Documented)return zero>=0?raw.Substring(0,zero):raw.TrimEnd(' ')+".DRW";
   string name=(zero>=0?raw.Substring(0,zero):raw).TrimEnd(' ');
   if(name.IndexOf(':')>=0||name.IndexOf('/')>=0||name.IndexOf('\\')>=0)throw new NotSupportedException("Code11 uses a local/library name, not a drive or path.");
   string stem=Path.GetFileNameWithoutExtension(name),extension=Path.GetExtension(name);
   if(stem.Length==0||stem.Length>6||extension.Length!=0&&!extension.Equals(".DRW",StringComparison.OrdinalIgnoreCase))throw new NotSupportedException("Code11 needs a prefix of one to six characters, optionally followed by .DRW.");
   return stem+".DRW";
  }
  public static void AddMenu(ContextMenuStrip menu,Action changed){
   var choice=new ToolStripMenuItem("Drawing filenames: accept full names as in Tom's Help");
   choice.Click+=(s,e)=>{Set(!Documented,true);changed();};menu.Items.Add(choice);
   menu.Opening+=(s,e)=>{choice.Checked=Documented;choice.Enabled=!NativeScoreBehaviour.Enabled;choice.ToolTipText="Default: original six-byte SCORE3.11 filename interpretation. Optional: documented prefix or .DRW filename and current-folder lookup. Strict SCORE keeps the original interpretation.";};
  }
 }
}
