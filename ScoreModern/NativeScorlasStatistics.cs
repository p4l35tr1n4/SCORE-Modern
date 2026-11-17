using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal static class NativeScorlasStatistics {
  static string Integer(int value,int width){string text=value.ToString(CultureInfo.InvariantCulture);return text.Length>width?new string('*',width):text.PadLeft(width);}
  static string Real(float value){string text=SingleFixedDecimal.Format(value,2);if(text.StartsWith("0.",StringComparison.Ordinal))text=text.Substring(1);else if(text.StartsWith("-0.",StringComparison.Ordinal))text="-"+text.Substring(2);return text.Length>6?"******":text.PadLeft(6);}
  internal static string Create(string root,IList<List<string>> pages,NativePrintJob settings,IList<List<string>> displayNames=null){
   if(pages==null||pages.Count==0)throw new ArgumentException("Add at least one page.");
   if(displayNames!=null&&displayNames.Count!=pages.Count)throw new ArgumentException("Statistics names do not match the page count.");
   var report=new StringBuilder();int grand=0;float size=(settings??NativePrintJob.Defaults(root)).Size;
   for(int page=0;page<pages.Count;page++){
    var files=pages[page];if(files==null||files.Count==0)throw new ArgumentException("Every page needs at least one file.");if(displayNames!=null&&displayNames[page].Count!=files.Count)throw new ArgumentException("Statistics names do not match the file count.");
    for(int n=0;n<files.Count;n++){string name=displayNames==null?Path.GetFullPath(files[n]):displayNames[page][n];if(name.IndexOfAny(new[]{'\r','\n'})>=0)throw new ArgumentException("A statistics filename contains a line break.");report.Append(' ').Append(name.Length>24?name.Substring(0,24):name.PadRight(24)).Append("\r\n");}
    var placement=new ScorlasFilePlacementState();short items=0;
    for(int n=files.Count-1;n>=0;n--)using(var canvas=new Canvas{Score=Score.Load(files[n]),Library=new DrawingLibrary(root),TextLayout=new ScoreText(root)}){
     canvas.LayoutScore();var extents=NativePrintMinimum.Extents(canvas);
     if(!extents.HasPoints){if(canvas.Score.Items.Count!=0||!NativePrintBehaviour.OriginalScorlasEmpty)throw new NotSupportedException("Statistics for an empty tile require the original empty-file bounds option.");extents=new NativePrintExtents{Left=32750,Bottom=32750,Right=-32700,Top=-32750};}
     bool metric=BitConverter.ToSingle(canvas.Score.Trailer,canvas.Score.Trailer.Length-12)==1;float p8=0;foreach(var item in canvas.Score.Items)if(item.Code==8&&item.Staff==1)p8=item.P(8);
     var frame=placement.Add(extents.Bottom,extents.Top,ScorlasGuideConversion.Convert(p8,metric));items=unchecked((short)(items+(canvas.Score.Items.Count==0?1:canvas.Score.Items.Count)));
     float height=ScorlasStatisticsArithmetic.Height(frame.OverallHeight,size,metric);report.Append(metric?" Total centimeters =":" Total inches =").Append(Real(height)).Append("   Total items =").Append(Integer(items,4)).Append("\r\n\r\n");
    }
    grand=unchecked(grand+items);
   }
   return report.Append(" Grand total =").Append(Integer(grand,6)).Append(" items.\r\n\r\n").ToString();
  }
 }
}
