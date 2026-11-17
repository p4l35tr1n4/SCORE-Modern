using System;using System.IO;using System.Linq;using System.Text;using System.Text.RegularExpressions;using System.Collections.Generic;using System.Security.Cryptography;
namespace ScoreModern {
 internal sealed class FontAllocation {
  internal int Number;internal string Name="",Path="",Error="",FontPath="",Afm="";internal bool AfmMatchesInstalled;internal byte[] Bytes;
  internal FontAllocation Copy(){return (FontAllocation)MemberwiseClone();}
 }
 internal sealed class FontCatalogEntry {
  internal string Name,Path,Family,Version,Hash,Description;internal string[] Afms=new string[0];
 }
 internal sealed class FontAllocationSnapshot {
  internal string Key,Stamp,Folder;internal FontAllocation[] Allocations;internal FontCatalogEntry[] Fonts;internal string[] Errors;
 }
 internal static class FontAllocationCatalog {
  internal static FontAllocationSnapshot Read(string key,string folder,string[] metrics){
   var errors=new List<string>();var fonts=new List<FontCatalogEntry>();var afms=new Dictionary<string,List<string>>(StringComparer.Ordinal);var stamp=new StringBuilder();var allocations=new FontAllocation[90];
   for(int n=0;n<90;n++){var row=new FontAllocation{Number=n,Path=metrics[n]};allocations[n]=row;try{if(!File.Exists(row.Path))continue;row.Bytes=File.ReadAllBytes(row.Path);var records=FortranRecords.Read(row.Bytes);if(records.Count<1||records[0].Length<354)throw new InvalidDataException("Incomplete PSC metric record");row.Name=Encoding.ASCII.GetString(records[0],0,28).Trim();if(row.Name.Length==0)throw new InvalidDataException("Empty PSC font name");stamp.Append(n).Append(':').Append(Hash(row.Bytes)).Append('|');}catch(Exception e){if(!(e is IOException||e is UnauthorizedAccessException||e is InvalidDataException))throw;row.Error=e.Message;errors.Add("FM"+n.ToString("00")+": "+e.Message);}}
   if(!String.IsNullOrEmpty(folder)&&Directory.Exists(folder)){
    string[] files;try{files=Directory.GetFiles(folder,"*",SearchOption.AllDirectories);}catch(Exception e){if(!(e is IOException||e is UnauthorizedAccessException))throw;files=new string[0];errors.Add(e.Message);}
    foreach(string file in files.Where(f=>String.Equals(System.IO.Path.GetExtension(f),".afm",StringComparison.OrdinalIgnoreCase)).OrderBy(f=>f,StringComparer.OrdinalIgnoreCase))try{string name=FontconvPscModel.FontName(File.ReadAllText(file));List<string> paths;if(!afms.TryGetValue(name,out paths)){paths=new List<string>();afms.Add(name,paths);}paths.Add(file);var info=new FileInfo(file);stamp.Append(file).Append(info.Length).Append(info.LastWriteTimeUtc.Ticks);}catch(Exception e){if(!(e is IOException||e is UnauthorizedAccessException||e is InvalidDataException))throw;errors.Add(System.IO.Path.GetFileName(file)+": "+e.Message);}
    var seen=new HashSet<string>(StringComparer.Ordinal);
    foreach(string file in files.Where(f=>String.Equals(System.IO.Path.GetExtension(f),".pfb",StringComparison.OrdinalIgnoreCase)||String.Equals(System.IO.Path.GetExtension(f),".pfa",StringComparison.OrdinalIgnoreCase)).OrderBy(f=>f,StringComparer.OrdinalIgnoreCase))try{byte[] bytes=File.ReadAllBytes(file);string hash=Hash(bytes);stamp.Append(file).Append(hash);if(!seen.Add(hash))continue;string header=Encoding.GetEncoding(28591).GetString(bytes,0,Math.Min(bytes.Length,65536));var m=Regex.Match(header,@"/FontName\s+/([^\s]+)");if(!m.Success){errors.Add(System.IO.Path.GetFileName(file)+": no Type 1 FontName");continue;}string name=m.Groups[1].Value;List<string> matches;afms.TryGetValue(name,out matches);fonts.Add(new FontCatalogEntry{Name=name,Path=file,Hash=hash,Family=Literal(header,"FamilyName"),Version=Literal(header,"version"),Description=FontVersionChoices.Description(file),Afms=matches==null?new string[0]:matches.ToArray()});}catch(Exception e){if(!(e is IOException||e is UnauthorizedAccessException||e is InvalidDataException))throw;errors.Add(System.IO.Path.GetFileName(file)+": "+e.Message);}
   }
   var converted=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
   foreach(var row in allocations.Where(row=>row.Name.Length>0&&row.Error.Length==0)){
    List<string> candidates;
    if(!afms.TryGetValue(row.Name,out candidates))continue;
    var matches=new List<string>();
    var installed=FortranRecords.Read(row.Bytes);
    foreach(string path in candidates){
     byte[] data;
     if(!converted.TryGetValue(path,out data)){
      try{data=new FontconvPscModel().Convert(File.ReadAllText(path));}
      catch(Exception e){if(!(e is IOException||e is UnauthorizedAccessException||e is InvalidDataException||e is FormatException||e is OverflowException))throw;data=null;}
      converted[path]=data;
     }
     if(data==null||installed.Count<2)continue;
     var generated=FortranRecords.Read(data);
     if(Same(installed[0],generated[0])&&Same(installed[1],generated[1]))matches.Add(path);
    }
    if(matches.Count==1){row.Afm=matches[0];row.AfmMatchesInstalled=true;}
    else if(candidates.Count==1)row.Afm=candidates[0];
   }
   return new FontAllocationSnapshot{Key=key,Folder=folder,Stamp=Hash(Encoding.UTF8.GetBytes(stamp.ToString())),Allocations=allocations,Fonts=fonts.OrderBy(f=>f.Name,StringComparer.Ordinal).ThenBy(f=>f.Path,StringComparer.OrdinalIgnoreCase).ToArray(),Errors=errors.ToArray()};
  }
  static string Literal(string text,string name){var m=Regex.Match(text,@"/"+name+@"\s*\(([^)]*)\)");return m.Success?m.Groups[1].Value:"";}
  internal static string Hash(byte[] data){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(data)).Replace("-","");}
  internal static bool Same(byte[] a,byte[] b){return a==null?b==null:b!=null&&a.SequenceEqual(b);}
  internal static void Assign(FontAllocation[] rows,int target,FontCatalogEntry font,int? from){
   if(target<0||target>=rows.Length)throw new ArgumentOutOfRangeException("target");
   int existing=from??Array.FindIndex(rows,r=>r.Name==font.Name);if(existing==target&&from.HasValue)return;
   var old=rows[target].Copy();if(existing>=0&&existing!=target){var value=rows[existing].Copy();Set(rows[target],value);Set(rows[existing],old);}if(!from.HasValue){rows[target].Name=font.Name;rows[target].Bytes=null;rows[target].Error="";rows[target].FontPath=font.Path;rows[target].Afm=font.Afms.Length==1?font.Afms[0]:"";rows[target].AfmMatchesInstalled=false;}
  }
  static void Set(FontAllocation target,FontAllocation value){target.Name=value.Name;target.Bytes=value.Bytes;target.Error=value.Error;target.FontPath=value.FontPath;target.Afm=value.Afm;target.AfmMatchesInstalled=value.AfmMatchesInstalled;}
 }
 // All file changes are staged and rolled back together. Expected bytes protect against external edits.
 internal sealed class FontMetricWrite {internal string Path;internal byte[] Expected,Data;}
 internal static class FontMetricTransaction {
  internal static Action<int> BeforeCommitForTests=null;
  internal static void Commit(IList<FontMetricWrite> changes){
   var paths=new HashSet<string>(StringComparer.OrdinalIgnoreCase);foreach(var c in changes){c.Path=System.IO.Path.GetFullPath(c.Path);if(!paths.Add(c.Path))throw new IOException("Conflicting metric destinations: "+c.Path);Check(c);}
   var staged=new string[changes.Count];var backups=new string[changes.Count];var committed=new bool[changes.Count];bool success=false;
   try{for(int n=0;n<changes.Count;n++){var c=changes[n];if(c.Data==null)continue;staged[n]=System.IO.Path.Combine(System.IO.Path.GetDirectoryName(c.Path),".ScoreModern-"+Guid.NewGuid().ToString("N")+".tmp");using(var f=new FileStream(staged[n],FileMode.CreateNew,FileAccess.Write,FileShare.None)){f.Write(c.Data,0,c.Data.Length);f.Flush(true);}}
    for(int n=0;n<changes.Count;n++){var c=changes[n];if(BeforeCommitForTests!=null)BeforeCommitForTests(n);Check(c);if(c.Expected!=null){backups[n]=c.Path+".ScoreModern-"+Guid.NewGuid().ToString("N")+".backup";if(c.Data==null)File.Move(c.Path,backups[n]);else File.Replace(staged[n],c.Path,backups[n]);}else if(c.Data!=null)File.Move(staged[n],c.Path);committed[n]=true;}success=true;
   }catch(Exception failure){var recovery=new List<string>();for(int n=changes.Count-1;n>=0;n--)if(committed[n])try{var c=changes[n];if(!FontAllocationCatalog.Same(File.Exists(c.Path)?File.ReadAllBytes(c.Path):null,c.Data))throw new IOException("Destination changed after commit");if(c.Expected!=null){if(File.Exists(c.Path))File.Replace(backups[n],c.Path,null);else File.Move(backups[n],c.Path);}else if(File.Exists(c.Path))File.Delete(c.Path);}catch(Exception){recovery.Add(backups[n]??changes[n].Path);}if(recovery.Count>0)throw new IOException("PSC save failed; recovery copies retained: "+String.Join(", ",recovery),failure);throw;
   }finally{foreach(string path in staged)if(path!=null&&File.Exists(path))try{File.Delete(path);}catch(IOException){}if(success)foreach(string path in backups)if(path!=null&&File.Exists(path))try{File.Delete(path);}catch(IOException){}}
  }
  static void Check(FontMetricWrite c){if(!FontAllocationCatalog.Same(File.Exists(c.Path)?File.ReadAllBytes(c.Path):null,c.Expected))throw new IOException("PSC changed outside this window: "+c.Path+". Reload before saving.");}
 }
}
