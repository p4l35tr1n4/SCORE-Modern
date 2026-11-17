using System;using System.Text;using System.IO;using System.Collections.Generic;using System.Linq;using System.Windows.Forms;using System.Drawing;
namespace ScoreModern {
 public sealed partial class ScoreText {
  public string FontReport(Score score){var ids=new SortedSet<int>();ids.Add(0);ids.Add(2);if(score!=null)foreach(var item in score.Items)if(item.Code==16){for(int p=0;p<item.Text.Length;p++){int face,length;if(FontControl(item.Text,p,out face,out length)){if(face%100<90)ids.Add(face%100);p+=length-1;}}var forced=ScoreModern.RecoveredFragments.TextConverterFontState.Begin(unchecked((short)(int)item.P(8)),0);if(forced.Forced&&forced.MetricFace<90)ids.Add(forced.MetricFace);}
   var report=new StringBuilder("SCORE metrics remain external and authoritative. No Windows TTF/OTF substitution is approved or enabled.\r\nType 1 folder: "+(NativeTupletFonts.DirectoryPath??"not selected")+"\r\nPSC destination folder: "+NativeLibraryLocation.DirectoryFor(RootDirectory,false)+"\r\nDrawing LIB folder: "+NativeLibraryLocation.DirectoryFor(RootDirectory,true)+"\r\n\r\nText references and standard notation label fonts:\r\n");foreach(int id in ids){string name=ExternalFontName(id);report.Append("Font "+id.ToString("00")+": ");if(name==null){report.AppendLine("MISSING PSC metrics (FM"+id.ToString("00")+".PSC). No replacement selected.");continue;}report.AppendLine(name);report.AppendLine("  Metrics: "+NativeLibraryLocation.FileFor(RootDirectory,"FM"+id.ToString("00")+".PSC"));try{report.AppendLine("  Type 1: "+NativeTupletFonts.ResolvePath(name));}catch(Exception error){report.AppendLine("  UNRESOLVED: "+error.Message);}}
   report.AppendLine("\r\nAvailable metric assignments in active LIB:");foreach(var face in faces.OrderBy(x=>x.Key))report.AppendLine(face.Key.ToString("00")+" = "+face.Value.Name);return report.ToString();}
 }
 public sealed partial class Editor {
  internal void ShowFontResources(){ShowCopyableList("Font resources","font-resources",canvas.TextLayout.FontReport(score)+"\r\nLast text-cache refresh issues:\r\n"+textCacheIssues);}
 }
}
