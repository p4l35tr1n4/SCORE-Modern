using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScoreModern {
 static class NativeSettings {
  sealed class Snapshot{
   internal readonly DateTime Modified;
   internal readonly long Length;
   internal readonly Dictionary<string,string> Values;
   internal Snapshot(FileInfo file,Dictionary<string,string> values){
    Modified=file.LastWriteTimeUtc;Length=file.Length;
    Values=new Dictionary<string,string>(values,StringComparer.OrdinalIgnoreCase);
   }
  }
  static volatile Snapshot snapshot;
  static readonly string[] LegacyKeys={
   "beams","dat","display","edit-compatibility","endings","font-choices","fontinit",
   "ghostscript","input-highlight","lib-status","library","mouse","notation","open-folder",
   "output","postscript","preference-paths","slurs","splash","startup-folder","text-cache",
   "type1","window","workspace"
  };
  internal static string FilePath {
   get {return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ScoreModern.ini");}
  }

  static Dictionary<string,string> ParseFile(string path,out bool encoded){
   var settings=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
    string[] lines=File.ReadAllLines(path);
    encoded=lines.FirstOrDefault(line=>!String.IsNullOrWhiteSpace(line))=="[SCORE Modern]";
    string key=null;
    foreach(string line in lines){
     if(!encoded&&key!=null&&line.StartsWith("  | ",StringComparison.Ordinal)){
      settings[key]+="\n"+line.Substring(4);
      continue;
     }
     key=null;
     string prefix=line.TrimStart();
     if(prefix.StartsWith(";")||prefix.StartsWith("#")||prefix.StartsWith("["))continue;
     int separator=line.IndexOf('=');
     if(separator<1)continue;
     key=line.Substring(0,separator).Trim();
     if(key.Length==0){key=null;continue;}
     string value=line.Substring(separator+1);
     settings[key]=encoded?Encoding.UTF8.GetString(Convert.FromBase64String(value)):value;
    }
   return settings;
  }

  static Dictionary<string,string> Open(){
   if(File.Exists(FilePath)){
    bool encoded;
    var saved=ParseFile(FilePath,out encoded);
    if(encoded)Commit(saved);
    return saved;
   }
   var settings=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
    foreach(string key in LegacyKeys){
     string old=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ScoreModern."+key+".ini");
     if(File.Exists(old))settings[key]=File.ReadAllText(old);
    }
    Commit(settings);
   return settings;
  }

  static void Commit(Dictionary<string,string> settings){
   Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
   ScoreFileStorage.Write(FilePath,Serialize(settings,"[SCORE Modern settings]"));
   snapshot=new Snapshot(new FileInfo(FilePath),settings);
  }

  static byte[] Serialize(Dictionary<string,string> settings,string header){
   var output=new StringBuilder(header+"\r\n; Lines beginning with '  | ' continue the preceding value.\r\n");
   foreach(var entry in settings.OrderBy(item=>item.Key,StringComparer.OrdinalIgnoreCase)){
    string value=entry.Key.Equals("font-choices",StringComparison.OrdinalIgnoreCase)?FontVersionChoices.ReadableRows(entry.Value):entry.Value;
    value=value.Replace("\r\n","\n").Replace('\r','\n').Replace("\n","\r\n  | ");
    output.Append(entry.Key).Append('=').Append(value).Append("\r\n");
   }
   return Encoding.UTF8.GetBytes(output.ToString());
  }

  static T Access<T>(Func<Dictionary<string,string>,T> action){
   using(var mutex=new Mutex(false,@"Local\ScoreModern.Settings")){
    try{mutex.WaitOne();}catch(AbandonedMutexException){}
    try{return action(Open());}finally{mutex.ReleaseMutex();}
   }
  }
  [ThreadStatic]static Dictionary<string,string> readScope;
  sealed class ReadSnapshot:IDisposable {readonly Dictionary<string,string> previous;internal ReadSnapshot(){previous=readScope;if(previous==null){Read("__snapshot_refresh__");readScope=snapshot==null?new Dictionary<string,string>():snapshot.Values;}}public void Dispose(){readScope=previous;}}
  internal static IDisposable SnapshotRead(){return new ReadSnapshot();}
  internal static string Read(string key){
   if(readScope!=null){string scoped;return readScope.TryGetValue(key,out scoped)?scoped:null;}
   var file=new FileInfo(FilePath);
   var cached=snapshot;
   string value;
   if(cached!=null&&file.Exists&&file.Length==cached.Length&&file.LastWriteTimeUtc==cached.Modified)
    return cached.Values.TryGetValue(key,out value)?value:null;
   return Access(settings=>{
    var current=new FileInfo(FilePath);
    if(current.Exists)snapshot=new Snapshot(current,settings);
    string entry;
    return settings.TryGetValue(key,out entry)?entry:null;
   });
  }
  internal static string Import(string key,Func<string> legacy){
   return Access(settings=>{
    string value;
    if(settings.TryGetValue(key,out value))return value;
    value=legacy()??"";
    settings[key]=value;
    Commit(settings);
    return value;
   });
  }
  internal static string InstallationKey(string prefix,string root){
   string full=Path.GetFullPath(root).TrimEnd('\\','/').ToUpperInvariant();
   using(var hash=System.Security.Cryptography.SHA256.Create()){
    return prefix+"."+BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(full))).Replace("-","");
   }
  }
  internal static void WriteBundle(Dictionary<string,string> values){Access(settings=>{foreach(var item in values)settings[item.Key]=item.Value;Commit(settings);return true;});}
  internal static bool NeedsFirstSetup(){
   if(!File.Exists(FilePath))return true;
   string root=Read("startup-folder");
   if(String.IsNullOrWhiteSpace(root)||!Directory.Exists(root))return true;
   NativeLibraryLocation.Load(root);
   return !Directory.Exists(NativeLibraryLocation.DirectoryFor(root,false));
  }

  internal static void CompleteFirstSetup(string root,Dictionary<string,string> values){
   string scoreKey=NativePreferences.SettingKey(root);
   Access(settings=>{
    string existing;
    settings.TryGetValue(scoreKey,out existing);
    var preferences=ScoreDirectives(existing).Where(entry=>IsPortableScoreDirective(entry.Name))
     .ToDictionary(entry=>entry.Name,StringComparer.OrdinalIgnoreCase);
    string portableScore;
    if(settings.TryGetValue(PortableScoreKey,out portableScore)){
     foreach(var entry in ScoreDirectives(portableScore).Where(entry=>IsPortableScoreDirective(entry.Name)))
      preferences[entry.Name]=entry;
     settings.Remove(PortableScoreKey);
    }
    values[scoreKey]+="\r\n"+String.Join("\r\n",preferences.Values.Select(entry=>NativePreferences.Directive(entry.Name,entry.Value)));
    foreach(var item in values)settings[item.Key]=item.Value;
    Commit(settings);
    return true;
   });
  }
  const string PortableHeader="[SCORE Modern portable settings]";
  const string PortableScoreKey="score-preferences";
  // Only settings with portable values belong here. Mixed settings that
  // embed paths, device identities or local history stay with the machine.
  static readonly HashSet<string> PortableKeys=new HashSet<string>((
   "approximate-rendering beams colour-background colour-text colour-edit colour-notation colour-duplicate-font "+
   "command-list-dialogs compact-text delete-key-as-f7 disk-file-dialogs disk-folder-dialogs dpy-native-arrays "+
   "draw-colour-background draw-colour-border draw-colour-origin-axes draw-colour-staff-grid draw-colour-outline draw-colour-overlay draw-colour-construction-points draw-colour-fill draw-colour-square-grid draw-colour-editable-points draw-colour-selected-point "+
   "drawing-names edi-repeated-marks edit-compatibility endings f2-large-input f2-reject-multiple-f-digits "+
   "grace-chord input-highlight keyboard-edit-overview keyboard-filter-escape keyboard-group-full keyboard-home "+
   "keyboard-item-page keyboard-item-tab keyboard-main-pan keyboard-recall-r keyboard-text-setup-full lib-status "+
   "menu-three-button-exit midi-held-chords mouse notation notation-gap notation-gap-points page-frame page-gap page-gap-points "+
   "postscript print-go-suppression print-incomplete-eps print-menu quarter-tone-spacing recent-file-count "+
   "ruler-background ruler-crosshair ruler-height ruler-staff ruler-units ruler-width rulers score-behaviour scorlas-empty-bounds "+
   "splash splash-seconds startup-reopen-last tab-accidental-table tab-dos-load-segment tab-quarter-tones "+
   "text-area-font-size text-cache ui-font workspace zoom-command-defaults zoom-framing").Split(' '),StringComparer.OrdinalIgnoreCase);

  static bool IsPortableScoreDirective(string name){
   return name!="LIBRARY"&&name!="CODE9"&&name!="GUITAR";
  }

  static IEnumerable<ScorePreference> ScoreDirectives(string value){
   return NativePreferences.Parse((value??"").Split('\n'),PortableScoreKey).Entries.Values;
  }

  internal static void ExportPortable(string path,string root){
   if(String.Equals(Path.GetFullPath(path),FilePath,StringComparison.OrdinalIgnoreCase))
    throw new IOException("Choose a separate file for portable settings.");
   string scoreKey=NativePreferences.SettingKey(root);
   var portable=Access(settings=>{
    var values=settings.Where(entry=>PortableKeys.Contains(entry.Key))
     .ToDictionary(entry=>entry.Key,entry=>entry.Value,StringComparer.OrdinalIgnoreCase);
    string score;
    settings.TryGetValue(scoreKey,out score);
    values[PortableScoreKey]=String.Join("\r\n",ScoreDirectives(score)
     .Where(entry=>IsPortableScoreDirective(entry.Name))
     .Select(entry=>NativePreferences.Directive(entry.Name,entry.Value)));
    return values;
   });
   ScoreFileStorage.Write(path,Serialize(portable,PortableHeader));
  }

  internal static void ImportPortable(string path,string root){
   if(File.ReadLines(path).FirstOrDefault(line=>!String.IsNullOrWhiteSpace(line))!=PortableHeader)
    throw new InvalidDataException("Select a portable INI exported by SCORE Modern.");
   bool encoded;
   var portable=ParseFile(path,out encoded);
   string score;
   portable.TryGetValue(PortableScoreKey,out score);
   var directives=ScoreDirectives(score).Where(entry=>IsPortableScoreDirective(entry.Name)).ToArray();
   string scoreKey=NativePreferences.SettingKey(root);
   Access(settings=>{
    foreach(string key in PortableKeys)settings.Remove(key);
    foreach(var entry in portable.Where(entry=>PortableKeys.Contains(entry.Key)))settings[entry.Key]=entry.Value;
    string existing;
    settings.TryGetValue(scoreKey,out existing);
    settings[scoreKey]=String.Join("\r\n",ScoreDirectives(existing)
     .Where(entry=>!IsPortableScoreDirective(entry.Name)).Concat(directives)
     .Select(entry=>NativePreferences.Directive(entry.Name,entry.Value)));
    Commit(settings);
    return true;
   });
  }

  internal static void WriteAllText(string key,string value){
   Access(settings=>{if(value==null)settings.Remove(key);else settings[key]=value;Commit(settings);return true;});
  }
  internal static bool Exists(string key){return Read(key)!=null;}
  internal static string ReadAllText(string key){return Read(key)??throwMissing(key);}
  static string throwMissing(string key){throw new FileNotFoundException("Setting not found: "+key);}
  internal static string[] ReadAllLines(string key){
   var lines=new List<string>();
   using(var reader=new StringReader(ReadAllText(key))){
    string line;
    while((line=reader.ReadLine())!=null)lines.Add(line);
   }
   return lines.ToArray();
  }
  internal static void WriteAllLines(string key,string[] lines){WriteAllText(key,String.Join("\r\n",lines)+"\r\n");}
 }
}
