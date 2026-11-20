using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ScoreModern {
 internal static class NativeScorlasNames {
  static readonly Encoding Cp437=Encoding.GetEncoding(437,EncoderFallback.ExceptionFallback,DecoderFallback.ExceptionFallback);
  // Original M/T writes one starting filename and tile count per line.
  // Explicit path resolution keeps DOS-drive interpretation out of this parser.
  internal static List<List<string>> Read(string file,Func<string,string> resolve){
   var pages=new List<List<string>>();int line=0;
   foreach(string raw in File.ReadAllLines(file,Cp437)){
    line++;if(String.IsNullOrWhiteSpace(raw))continue;
    var match=Regex.Match(raw,@"^\s*(.+?)\s+(\d+)\s*$");int count;
    if(!match.Success||!Int32.TryParse(match.Groups[2].Value,NumberStyles.None,CultureInfo.InvariantCulture,out count)||count<1)throw new InvalidDataException("Invalid SCORLAS name-list entry at line "+line+". Expected starting filename and files per page.");
    string name=match.Groups[1].Value.Trim();if(name.Length>1&&name[0]=='\"'&&name[name.Length-1]=='\"')name=name.Substring(1,name.Length-2);
    string path=resolve==null?Path.GetFullPath(name):resolve(name);
    pages.AddRange(NativeScorlasBatch.Define(path,count,1,false,ScorlasInputSequence.ConsecutiveCharacters));
   }
   if(pages.Count==0)throw new InvalidDataException("The SCORLAS name list is empty.");return pages;
  }
  internal static void Write(string file,IList<List<string>> pages){
   var text=new StringBuilder();if(pages==null||pages.Count==0)throw new ArgumentException("No pages to save.");
   foreach(var page in pages){if(page==null||page.Count==0)throw new ArgumentException("Every page needs at least one file.");var sequence=NativeScorlasBatch.Define(page[0],page.Count,1,false,ScorlasInputSequence.ConsecutiveCharacters)[0];for(int n=0;n<page.Count;n++)if(!String.Equals(Path.GetFullPath(page[n]),sequence[n],StringComparison.OrdinalIgnoreCase))throw new NotSupportedException("An original SCORLAS name list requires each page's files to advance by the final filename character. This page has a different order; its editable batch remains unchanged.");text.Append(page[0]).Append(' ').Append(page.Count.ToString(CultureInfo.InvariantCulture)).Append("\r\n");}
   foreach(var page in pages)foreach(string input in page)if(String.Equals(Path.GetFullPath(input),Path.GetFullPath(file),StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("The name list would overwrite an input score.");
   byte[] bytes=Cp437.GetBytes(text.ToString());string destination=Path.GetFullPath(file),temporary=Path.Combine(Path.GetDirectoryName(destination),".scorlasnames-"+Guid.NewGuid().ToString("N")+".tmp");
   try{File.WriteAllBytes(temporary,bytes);if(File.Exists(destination))File.Replace(temporary,destination,null);else File.Move(temporary,destination);}finally{File.Delete(temporary);}
  }
 }
}
