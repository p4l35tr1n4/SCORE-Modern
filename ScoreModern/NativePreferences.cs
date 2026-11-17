using System;using System.IO;using System.Text;using System.Linq;using System.Collections.Generic;using System.Globalization;using System.Windows.Forms;
namespace ScoreModern {
 public sealed class ScorePreference {
  internal bool IsOverride;
  public string Name,Value;public int Line;
 }
 public sealed class ScorePreferences {
  internal string SourceLibrary;
  public string Path;public readonly Dictionary<string,ScorePreference> Entries=new Dictionary<string,ScorePreference>(StringComparer.OrdinalIgnoreCase);public readonly List<string> Issues=new List<string>();
  public string Text(string name){ScorePreference item;return Entries.TryGetValue(name,out item)?item.Value:null;}
  public float Number(string name,float fallback){string value=Text(name);if(value==null)return fallback;float result;if(!Single.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out result)||Single.IsNaN(result)||Single.IsInfinity(result))throw new NotSupportedException("Invalid "+name+" preference in "+Path);if(result==0&&value.TrimStart().StartsWith("-"))return BitConverter.ToSingle(BitConverter.GetBytes(unchecked((int)0x80000000)),0);return result;}
  public short Word(string name,short fallback){float value=Number(name,fallback);if(value<short.MinValue||value>short.MaxValue||value!=Math.Truncate(value))throw new NotSupportedException(name+" preference requires a verified signed16 integer value.");return (short)value;}
  public void Issue(string message){if(!Issues.Contains(message))Issues.Add(message);}
 }
 public static class NativePreferences {
  static readonly string[] Names={"SCREEN","MIDI","IRQ","KEYDISK","LIBRARY","CODE9","GUITAR","REST","COLOR","BEAM","BMTH","TIES","SLUR","MIDK","METRIC","INCHES","DYNAMICS","DESC","DASH","XDASH","OUTPUT","RESOLUTION","PROCEDURES","SETSTROKE","AFTC","AFTB","AFTK","AFTM","MOUSE"};
  static readonly Dictionary<string,ScorePreferences> Cache=new Dictionary<string,ScorePreferences>(StringComparer.OrdinalIgnoreCase);static readonly Dictionary<string,string> Stamps=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
  public static bool FollowPaths{get;private set;}
  static string Settings{get{return "preference-paths";}}
  static NativePreferences(){FollowPaths=true;try{if(NativeSettings.Exists(Settings))FollowPaths=NativeSettings.ReadAllText(Settings).Trim()!="selectors";}catch(IOException){}catch(UnauthorizedAccessException){}}
  public static void Clear(){Cache.Clear();Stamps.Clear();}
  public static void Restore(string root,ScorePreferences data){Cache[System.IO.Path.GetFullPath(root)+"|"+NativeLibraryLocation.RevisionFor(root)]=data;}
  public static void Validate(string root){var data=Read(root);NativeScreenPalette.Read(root);short units=data.Word("MEASUREMENT",0);if(units!=0&&units!=1)throw new NotSupportedException("Measurement mode must be inches or metric.");using(NativePrintContext.Begin(root)){}data.Word("REST",6);short mouse=data.Word("MOUSE",0);if(mouse!=0&&mouse!=1)throw new NotSupportedException("PREF.SCR MOUSE requires a verified 0 or 1 value.");data.Number("SLUR",1);ResourceDirectory(root,"LIBRARY","LIB");ResourceFile(root,"CODE9","CODE9.DAT");ResourceFile(root,"GUITAR","GUITAR.DAT");}

  public static void SetFollowPaths(bool follow,bool save){FollowPaths=follow;Clear();if(save)NativeSettings.WriteAllText(Settings,follow?"original":"selectors");}
  public static string OriginalSourcePath(string root,bool useInstallation=false){var choice=NativeLibraryLocation.Get(root);if(!useInstallation&&choice!=null&&choice.All){if(NativeLibraryLocation.IsScor4(root)&&NativeLibraryLocation.GetSelection(root,true)==choice){string scor4=System.IO.Path.Combine(choice.Folder,"PREF-4.SCR");if(File.Exists(scor4))return scor4;}string selected=System.IO.Path.Combine(choice.Folder,"PREF.SCR");if(File.Exists(selected))return selected;}string local=System.IO.Path.Combine(root,"PREF.SCR");return File.Exists(local)?local:System.IO.Path.Combine(root,"LIB","PREF.SCR");}
  static string[] PreferenceLines(string path){return File.Exists(path)?File.ReadAllLines(path):new string[0];}
  internal static ScorePreferences Defaults(string root){
   string path=OriginalSourcePath(root);
   var data=Parse(PreferenceLines(path),path);
   data.SourceLibrary=data.Text("LIBRARY");
   return data;
  }
  internal static string SettingKey(string root){return NativeSettings.InstallationKey(
   NativeLibraryLocation.IsScor4(root)&&NativeLibraryLocation.GetSelection(root,true)!=null?"score-preferences-scor4":"score-preferences",root);}
  internal static string Directive(string name,string value){
   if(value.IndexOfAny(new[]{'\r','\n'})>=0)throw new InvalidDataException("A preference must occupy one line.");
   if(name!="MEASUREMENT")return name+" "+value;
   if(value.Trim()=="1")return "METRIC";
   if(value.Trim()=="0")return "INCHES";
   throw new InvalidDataException("Measurement mode must be 0 or 1.");
  }
  static string LegacyOverrides(ScorePreferences defaults){
   string original=Path.GetFullPath(defaults.Path).ToUpperInvariant();
   string key;
   using(var hash=System.Security.Cryptography.SHA256.Create()){
    key=BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(original))).Replace("-","");
   }
   string relative=Path.Combine("Preferences",key,"PREF.MSCR");
   string besideApp=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,relative);
   string local=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"SCORE Modern",relative);
   string legacy=new[]{Program.CheckMode?besideApp:local,besideApp,Path.ChangeExtension(defaults.Path,"MSCR")}.FirstOrDefault(File.Exists);
   if(legacy==null)return "";
   return String.Join("\r\n",Parse(File.ReadAllLines(legacy),legacy).Entries.Values
    .Where(entry=>entry.Value!=defaults.Text(entry.Name))
    .Select(entry=>Directive(entry.Name,entry.Value)));
  }
  public static string SourcePath(string root){return OriginalSourcePath(root);}
  public static ScorePreferences Read(string root){
   string key=Path.GetFullPath(root)+"|"+NativeLibraryLocation.RevisionFor(root);
   ScorePreferences result;
   if(Cache.TryGetValue(key,out result))return result;
   var defaults=Defaults(root);
   string saved=NativeSettings.Import(SettingKey(root),()=>LegacyOverrides(defaults));
   var overrides=Parse(saved.Split(new[]{"\r\n","\n"},StringSplitOptions.RemoveEmptyEntries),NativeSettings.FilePath);
   foreach(var entry in overrides.Entries){
    entry.Value.IsOverride=true;
    defaults.Entries[entry.Key]=entry.Value;
   }
   defaults.Issues.AddRange(overrides.Issues);
   if(!File.Exists(defaults.Path))defaults.Issue("PREF.SCR not found: "+defaults.Path+". Verified default preferences are used.");
   return Cache[key]=defaults;
  }
  public static ScorePreferences Parse(string[] lines,string source){var data=new ScorePreferences{Path=source};for(int n=0;n<lines.Length;n++){string line=lines[n].Trim();if(line.Length==0)continue;int end=line.IndexOfAny(new[]{' ','\t'});string token=end<0?line:line.Substring(0,end),key=token.Length>4?token.Substring(0,4):token;string canonical=Names.FirstOrDefault(x=>String.Equals(x.Length>4?x.Substring(0,4):x,key,StringComparison.OrdinalIgnoreCase));if(key.Equals("HELP",StringComparison.OrdinalIgnoreCase))continue;if(canonical==null){data.Issue("Unrecognised PREF.SCR directive at line "+(n+1)+": "+token);continue;}string value=end<0?"":line.Substring(end).Trim();if(canonical=="METRIC"||canonical=="INCHES"){canonical="MEASUREMENT";value=token.StartsWith("METR",StringComparison.OrdinalIgnoreCase)?"1":"0";}float zero;if(canonical=="BMTH"&&Single.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out zero)&&zero==0){data.Issue("BMTH 0 retains the previous thickness, as in SCORE.");continue;}data.Entries[canonical]=new ScorePreference{Name=canonical,Value=value,Line=n+1};}return data;}
  public static string MapPath(string root,string value){string full=System.IO.Path.GetFullPath(root).TrimEnd('\\','/');value=value.Trim().Trim('"').Replace('/','\\');if(value.StartsWith("\\\\"))throw new NotSupportedException("Use a folder selector for a network resource path.");if(System.IO.Path.IsPathRooted(value)){string host=System.IO.Path.GetFullPath(value);if(Directory.Exists(host)||File.Exists(host)||host.StartsWith(full+System.IO.Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||String.Equals(host,full,StringComparison.OrdinalIgnoreCase))return host;}if(value.Length>=2&&value[1]==':')value=value.Substring(2);value=value.TrimStart('\\');string result=System.IO.Path.GetFullPath(System.IO.Path.Combine(full,value));if(!result.StartsWith(full+System.IO.Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&!String.Equals(result,full,StringComparison.OrdinalIgnoreCase))throw new NotSupportedException("PREF.SCR path escapes the selected SCORE installation; use an explicit folder selector.");return result;}
  public static string ResourceDirectory(string root,string name,string fallback){string original=System.IO.Path.Combine(root,fallback);if(name=="LIBRARY"&&NativeLibraryLocation.IsScor4(root)){var selected=NativeLibraryLocation.GetSelection(root,true);if(selected!=null&&selected.All)return selected.Folder;}if(!FollowPaths)return original;var data=Read(root);string raw=data.Text(name);if(String.IsNullOrEmpty(raw))return original;try{string path=raw.Length==1&&Char.IsLetter(raw[0])?original:MapPath(root,raw);if(Directory.Exists(path))return path;data.Issue(name+" path not found: "+path+". Using "+original+".");}catch(ArgumentException e){data.Issue(name+": "+e.Message);}catch(NotSupportedException e){data.Issue(name+": "+e.Message);}return original;}
  public static string ResourceFile(string root,string name,string fallback){
   string library=ResourceDirectory(root,"LIBRARY","LIB");
   string original=Path.Combine(library,fallback);
   if(!FollowPaths)return original;
   var data=Read(root);
   string raw=data.Text(name);
   if(String.IsNullOrEmpty(raw))return original;
   try{
    // The preference may still name the library at its previous location.
    // Recognize only the standard filename under the declared or active LIB.
    if(!data.Entries[name].IsOverride&&File.Exists(original)&&
       IsLibraryResource(raw,data.SourceLibrary,fallback))return original;
    string path=MapPath(root,raw);
    if(File.Exists(path))return path;
    if(File.Exists(original)&&(IsLibraryResource(raw,data.Text("LIBRARY"),fallback)||
       IsLibraryResource(path,library,fallback)))return original;
    data.Issue(name+" file not found: "+path+". Using "+original+".");
   }catch(ArgumentException e){data.Issue(name+": "+e.Message);}
   catch(NotSupportedException e){data.Issue(name+": "+e.Message);}
   return original;
  }
  static bool IsLibraryResource(string value,string directory,string file){
   if(String.IsNullOrWhiteSpace(directory))return false;
   string prefix=directory.Trim().Trim('"').Replace('/','\\').TrimEnd('\\');
   string path=value.Trim().Trim('"').Replace('/','\\');
   return String.Equals(path,prefix+"\\"+file,StringComparison.OrdinalIgnoreCase)||
    String.Equals(path,prefix+file,StringComparison.OrdinalIgnoreCase);
  }
 }
}
