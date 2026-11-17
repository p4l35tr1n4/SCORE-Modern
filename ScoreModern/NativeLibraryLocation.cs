using System;using System.Linq;using System.IO;using System.Text;using System.Collections.Generic;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeLibraryLocation {
  public sealed class Selection {public string Folder;public bool All;}
  static readonly Dictionary<string,int> revisions=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
  static readonly Dictionary<string,Selection> choices=new Dictionary<string,Selection>(StringComparer.OrdinalIgnoreCase);
  static readonly Dictionary<string,Selection> scor4Choices=new Dictionary<string,Selection>(StringComparer.OrdinalIgnoreCase);
  static readonly HashSet<string> scor4Roots=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
  static string Key(string root){string full=Path.GetFullPath(root);return full.Length==Path.GetPathRoot(full).Length?full:full.TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);}
  static string Settings{get{return "library";}}
  public static int RevisionFor(string root){int value;revisions.TryGetValue(Key(root),out value);return value;}
  internal static bool IsScor4(string root){return scor4Roots.Contains(Key(root));}
  static void AdvanceRevision(string root){string key=Key(root);revisions[key]=RevisionFor(root)+1;}
  internal static void SetProfile(string root,bool scor4){
   if(scor4?scor4Roots.Add(Key(root)):scor4Roots.Remove(Key(root)))AdvanceRevision(root);
  }
  internal static Selection GetSelection(string root,bool scor4){
   Selection found;
   return (scor4?scor4Choices:choices).TryGetValue(Key(root),out found)?found:null;
  }
  internal static bool HasScor4Resources(string root){
   var value=IsScor4(root)?GetSelection(root,true):null;
   return value!=null&&value.All;
  }
  public static Selection Get(string root){return (IsScor4(root)?GetSelection(root,true):null)??GetSelection(root,false);}
  internal static void SetSelection(string root,bool scor4,string folder,bool all){
   var selections=scor4?scor4Choices:choices;
   string key=Key(root);
   if(String.IsNullOrEmpty(folder))selections.Remove(key);
   else selections[key]=new Selection{Folder=Path.GetFullPath(folder),All=all};
   AdvanceRevision(root);
  }
  public static void Set(string root,string folder,bool all){SetSelection(root,IsScor4(root),folder,all);}
  public static string DirectoryFor(string root,bool drawing){var value=Get(root);return value!=null&&(drawing||value.All)?value.Folder:NativePreferences.ResourceDirectory(root,"LIBRARY","LIB");}
  public static string FileFor(string root,string name){if(name.Equals("PREF.SCR",StringComparison.OrdinalIgnoreCase))return NativePreferences.SourcePath(root);if(name.Equals("CODE9.DAT",StringComparison.OrdinalIgnoreCase)||name.Equals("GUITAR.DAT",StringComparison.OrdinalIgnoreCase)){string mapped=NativePreferences.ResourceFile(root,name.StartsWith("CODE9",StringComparison.OrdinalIgnoreCase)?"CODE9":"GUITAR",name);var selection=Get(root);if(selection!=null&&selection.All){string chosen=Path.Combine(selection.Folder,Path.GetFileName(mapped));if(File.Exists(chosen))return chosen;}return File.Exists(mapped)?mapped:Path.Combine(root,"LIB",name);}if(name.Equals("SCOR3_10.DAT",StringComparison.OrdinalIgnoreCase))return NativeScoreIdentity.Require(root,false).Path;string candidate=Path.Combine(DirectoryFor(root,Path.GetExtension(name).Equals(".DRW",StringComparison.OrdinalIgnoreCase)),name);return File.Exists(candidate)?candidate:Path.Combine(root,"LIB",name);}
  public static string CacheKey(string root){int revision;revisions.TryGetValue(Key(root),out revision);return DirectoryFor(root,true)+"|"+DirectoryFor(root,false)+"|"+revision;}
  static string SelectionKey(string root,bool scor4=false){return NativeSettings.InstallationKey(scor4?"library-scor4":Settings,root);}
  public static void Save(string root){
   SaveSelection(root,IsScor4(root));
  }
  internal static void SaveSelection(string root,bool scor4){
   var value=GetSelection(root,scor4);
   NativeSettings.WriteAllText(SelectionKey(root,scor4),value==null?"":(value.All?"all":"drawings")+"\n"+value.Folder);
  }
  internal static void LoadScor4(string root){
   string saved=NativeSettings.Read(SelectionKey(root,true));
   int separator=String.IsNullOrEmpty(saved)?-1:saved.IndexOf('\n');
   if(separator>0)SetSelection(root,true,saved.Substring(separator+1),saved.Substring(0,separator)=="all");
  }
  public static void Load(){Load(null);}
  internal static void Load(string startupRoot){
   try{
    string legacy=NativeSettings.Read(Settings);
    string[] lines=(legacy??"").TrimEnd('\r','\n').Split(new[]{"\r\n","\n"},StringSplitOptions.None);
    if(lines.Length>=3&&Directory.Exists(lines[0]))
     NativeSettings.Import(SelectionKey(lines[0]),()=>String.IsNullOrEmpty(lines[1])?"":lines[2]+"\n"+lines[1]);
    if(String.IsNullOrEmpty(startupRoot))startupRoot=lines.Length>=3&&Directory.Exists(lines[0])?lines[0]:null;
    if(String.IsNullOrEmpty(startupRoot))return;
    string saved=NativeSettings.Read(SelectionKey(startupRoot));
    if(saved==null&&lines.Length==2&&Directory.Exists(lines[0])&&(lines[1]=="all"||lines[1]=="drawings")){
     saved=lines[1]+"\n"+lines[0];
     NativeSettings.WriteAllText(SelectionKey(startupRoot),saved);
     NativeSettings.WriteAllText(Settings,null);
    }
    if(String.IsNullOrEmpty(saved))return;
    int separator=saved.IndexOf('\n');
    if(separator>0)SetSelection(startupRoot,false,saved.Substring(separator+1),saved.Substring(0,separator)=="all");
   }catch(IOException){}catch(ArgumentException){}catch(UnauthorizedAccessException){}
  }
 }
 public sealed partial class Editor {
  void InstallResolvedLibrary(bool refreshCaches){
   NativeScoreIdentity.Require(source,!Program.CheckMode);
   NativePreferences.Validate(source);
   string folder=NativeLibraryLocation.DirectoryFor(source,true);
   bool readable=false;
   foreach(string drawing in Directory.GetFiles(folder,"*.DRW")){
    try{if(DrawingLibrary.Load(drawing).Any(g=>g.Points.Length>0)){readable=true;break;}}
    catch(InvalidDataException){}
   }
   if(!readable)throw new InvalidDataException("The selected folder has no readable DRW drawings.");
   var library=new DrawingLibrary(source);
   var text=new ScoreText(source);
   var selection=NativeLibraryLocation.Get(source);
   if(selection!=null&&selection.All){new NativeDatGlyphs(source);OriginalMusSave.Prepare(score??Score.New(),source);}
   if(score!=null)using(var probe=new Canvas()){probe.Score=score;probe.Library=library;probe.TextLayout=text;probe.LayoutScore();}
   canvas.Library=library;canvas.TextLayout=text;
   if(score!=null){
    if(refreshCaches){RefreshTextCaches();RefreshAll(canvas.Selected);}
    else{canvas.LayoutScore();UpdateClassic();}
   }
   canvas.Invalidate();
  }
  void WithLibraryChange(Action change,Action rollback,bool reload,Action save,bool refreshCaches=false){
   var beforePreferences=NativePreferences.Read(source);
   var oldLibrary=canvas.Library;
   var oldText=canvas.TextLayout;
   try{
    change();
    if(reload)InstallResolvedLibrary(refreshCaches);
    if(save!=null)save();
   }catch{
    rollback();
    NativePreferences.Restore(source,beforePreferences);
    canvas.Library=oldLibrary;canvas.TextLayout=oldText;canvas.LayoutScore();
    throw;
   }
  }
  void ApplyLibraryFolder(string folder,bool all,bool save){
   if(!String.IsNullOrEmpty(folder)&&!Directory.Exists(folder))throw new DirectoryNotFoundException("The selected LIB folder does not exist.");
   bool scor4=NativeLibraryLocation.IsScor4(source);
   var before=NativeLibraryLocation.GetSelection(source,scor4);
   WithLibraryChange(
    ()=>{if(save)NativeLibraryLocation.Set(source,folder,all);},
    ()=>{if(save)NativeLibraryLocation.SetSelection(source,scor4,before==null?null:before.Folder,before!=null&&before.All);},
    true,save?(Action)(()=>NativeLibraryLocation.Save(source)):null,true);
   Message("LIB: "+NativeLibraryLocation.DirectoryFor(source,true)+(all?" (all resources, including save serial)":" (drawings only)"));
  }
  void AddLibraryMenu(ContextMenuStrip menu){var group=new ToolStripMenuItem("Library (LIB)");var browse=new ToolStripMenuItem("Browse for LIB folder...");var all=new ToolStripMenuItem("Use selected LIB for all resources");var reset=new ToolStripMenuItem("Use SCORE's default LIB");var dataFile=new ToolStripMenuItem("Choose serial DAT...");browse.Click+=(s,e)=>Safe(()=>{using(var dialog=new FolderBrowserDialog{Description="Choose an external LIB folder (drawings only by default)",SelectedPath=NativeLibraryLocation.DirectoryFor(source,true)})if(dialog.ShowDialog(this)==DialogResult.OK)ApplyLibraryFolder(dialog.SelectedPath,false,true);});all.Click+=(s,e)=>Safe(()=>{var selected=NativeLibraryLocation.Get(source);if(selected!=null)ApplyLibraryFolder(selected.Folder,!selected.All,true);});reset.Click+=(s,e)=>Safe(()=>ApplyLibraryFolder(null,false,true));group.DropDownItems.Add(browse);group.DropDownItems.Add(all);dataFile.Click+=(s,e)=>Safe(()=>{NativeScoreIdentity.Require(source,true,true);var current=NativeLibraryLocation.Get(source);ApplyLibraryFolder(current==null?null:current.Folder,current!=null&&current.All,false);});group.DropDownItems.Add(dataFile);group.DropDownItems.Add(reset);menu.Items.Add(group);menu.Opening+=(s,e)=>{var selected=NativeLibraryLocation.Get(source);all.Checked=selected!=null&&selected.All;all.Enabled=selected!=null;reset.Enabled=selected!=null;group.ToolTipText=NativeLibraryLocation.DirectoryFor(source,true)+(selected!=null&&selected.All?" — all LIB resources":" — drawings only");};}
 }
}
