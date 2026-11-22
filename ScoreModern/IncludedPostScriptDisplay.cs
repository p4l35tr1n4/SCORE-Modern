using System;using System.IO;using System.Text;using System.Windows.Forms;
namespace ScoreModern {
 public static class IncludedPostScriptDisplay {
  static bool configuredMapDosPaths=true;public static bool MapDosPaths{get{return NativeScoreBehaviour.Enabled||configuredMapDosPaths;}}
  static bool configuredSearchBesideMusic;public static bool SearchBesideMusic{get{return NativeScoreBehaviour.Enabled?false:configuredSearchBesideMusic;}private set{configuredSearchBesideMusic=value;}}public static string Status="Included PostScript: selected SCORE folder";
  static string Settings{get{return "postscript";}}
  public static void Load(){SearchBesideMusic=false;configuredMapDosPaths=true;try{if(NativeSettings.Exists(Settings)){var lines=NativeSettings.ReadAllLines(Settings);SearchBesideMusic=lines.Length>0&&lines[0]=="score-and-mus";configuredMapDosPaths=lines.Length<2||lines[1]!="windows-paths";}}catch(IOException){}}
  static void Save(){NativeSettings.WriteAllLines(Settings,new[]{configuredSearchBesideMusic?"score-and-mus":"score-only",configuredMapDosPaths?"mapped-dos-paths":"windows-paths"});}
  public static void SetPaths(bool mapped,bool save){configuredMapDosPaths=mapped;if(save)Save();PostScriptProgramPreview.Clear();foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void Set(bool besideMusic,bool save){SearchBesideMusic=besideMusic;if(save)Save();PostScriptProgramPreview.Clear();foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void AddMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("PostScript files: also search beside MUS");item.Click+=(s,e)=>Set(!SearchBesideMusic,true);menu.Items.Add(item);var mapping=new ToolStripMenuItem("Map DOS PostScript paths to SCORE folder");mapping.Click+=(s,e)=>SetPaths(!MapDosPaths,true);menu.Items.Add(mapping);menu.Opening+=(s,e)=>{mapping.Checked=MapDosPaths;mapping.Enabled=!NativeScoreBehaviour.Enabled;mapping.ToolTipText="Use an existing absolute Windows file first; otherwise map its DOS path into the selected SCORE folder. Disable for literal Windows paths only.";};menu.Opening+=(s,e)=>{item.Checked=SearchBesideMusic;item.ToolTipText=Status;};}
  public static Item WorkingItem(Item item){if(item.Code!=15||item.ExternalPostScriptBoxOverride==null)return item;var copy=item.Clone();copy.ExternalPostScriptBoxOverride=null;for(int n=0;n<5;n++)copy.Set(7+n,item.ExternalPostScriptBoxOverride[n]);copy.Set(13,0);return copy;}
  public static string FileName(Item item){if(item.ExternalDrawingNameOverride!=null)return item.ExternalDrawingNameOverride.TrimEnd(' ','\0');if(item.Code!=15||item.Bytes.Length<80)throw new NotSupportedException("Included PostScript filename requires the original 24-byte field.");return Encoding.GetEncoding(437).GetString(item.Bytes,56,24).TrimEnd(' ','\0');}
  public static string Resolve(string scoreRoot,string musicPath,Item item){
   return Resolve(scoreRoot,musicPath,item,false);
  }
  internal static string Resolve(string scoreRoot,string musicPath,Item item,bool scor4){
   if(!String.IsNullOrEmpty(item.PartResourcePath)){if(!File.Exists(item.PartResourcePath))throw new FileNotFoundException("Part PostScript resource is missing: "+item.PartResourcePath,item.PartResourcePath);return Path.GetFullPath(item.PartResourcePath);}
   string name=FileName(item);
   if(String.IsNullOrEmpty(name))throw new FileNotFoundException("Included PostScript filename is empty.");
   if(Path.IsPathRooted(name)||name.IndexOf(':')>=0){
    string absolute=MapDosPaths?NativePreferences.MapPath(scoreRoot,name):Path.GetFullPath(name);
    if(File.Exists(absolute))return absolute;
    throw new FileNotFoundException("PostScript program file not found: "+absolute,absolute);
   }
   string folder=scor4?Directory.GetCurrentDirectory():scoreRoot;
   if(!String.IsNullOrEmpty(folder)){
    string path=Path.GetFullPath(Path.Combine(folder,name));
    if(File.Exists(path))return path;
   }
   if(SearchBesideMusic&&!String.IsNullOrEmpty(musicPath)){
    string path=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(musicPath)),name));
    if(File.Exists(path))return path;
   }
   string searched=scor4?"the current working folder":"the selected SCORE folder";
   throw new FileNotFoundException("PostScript program file not found: "+name+". Searched "+searched+
    (SearchBesideMusic?" and the MUS folder.":". Enable the optional MUS-folder search if required."));
  }
 }
}
