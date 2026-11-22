using System;using System.IO;using System.Linq;using System.Text;using System.Collections.Generic;
namespace ScoreModern {internal static class NativePartResourceIndex {
  internal static string Companion(string path){return path+".smxresources";}
  internal static void Bind(Score score,string path){
   string companion=Companion(path);if(!File.Exists(companion))return;var lines=File.ReadAllLines(companion,Encoding.UTF8);if(lines.Length<1||lines.Length>1421||lines[0]!="SMX part resources 1")throw new InvalidDataException("Invalid part-resource index: "+companion);var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);foreach(string name in lines.Skip(1)){string ext=Path.GetExtension(name);if(name!=Path.GetFileName(name)||name.Length==0||ext!=".DRW"&&ext!=".EPS")throw new InvalidDataException("Part-resource filenames must name local DRW/EPS files.");names.Add(name);}foreach(var item in score.Items.Where(i=>i.Code==11||i.Code==15)){string name=item.Code==11?NativeDrawingNames.Name(item):IncludedPostScriptDisplay.FileName(item);if(!names.Contains(name))continue;string resource=Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path)),name);if(!File.Exists(resource))throw new FileNotFoundException("Part resource is missing: "+resource,resource);item.PartResourcePath=resource;}
  }
}}
