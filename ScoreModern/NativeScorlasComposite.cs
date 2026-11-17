using System;using System.IO;using System.Collections.Generic;using System.Drawing;using System.Globalization;
using ScoreModern.RecoveredFragments;
using IOPath=System.IO.Path;
namespace ScoreModern {
 public sealed partial class NativeEpsExport {
  void CompositeFrame(Canvas canvas,int delta,bool first,NativePrintJob job){
   if(first)writer.WriteLine("SMphysical setmatrix\n"+job.RawProgram()+"/SMcomposite gstate def");
   else {
    string move="0 "+delta.ToString(CultureInfo.InvariantCulture)+" translate";
    writer.WriteLine("SMcomposite setgstate "+move+" SMcomposite currentgstate pop");
    // The original emits the next translation into the current PostScript
    // state, including effects of a preceding font99 program.
    if(literalState)writer.WriteLine("gsave SMliteral setgstate "+move+" SMliteral currentgstate pop grestore\ngsave SMcomposite setgstate /SMliteralbase matrix currentmatrix def grestore");
    if(textState)writer.WriteLine("gsave SMtextstate setgstate "+move+" SMtextstate currentgstate pop grestore");
   }
   // A PostScript matrix array stores real operands at lower precision than
   // the interpreter's retained CTM. Preserve gstate between raw translations.
   baseRawSetup="SMcomposite setgstate";
   writer.WriteLine("SMcomposite setgstate -2100 -27250 translate 55.55555555555556 dup scale /SMpage matrix currentmatrix def");
   writer.WriteLine(N(612.0/canvas.PageWidth)+" "+N(-792.0/canvas.PageHeight)+" scale 0 "+N(-canvas.PageHeight)+" translate /SMcanvas matrix currentmatrix def");
  }
  // Internal integration surface. Input is in source top-file order; SCORE
  // prints the last file first. Publish only after the complete stream succeeds.
  internal static void WriteScorlas(IList<Canvas> input,string root,string file,NativePrintJob job){WriteScorlasNamed(input,root,file,job,null);}
  internal static void WriteScorlasNamed(IList<Canvas> input,string root,string file,NativePrintJob job,IList<string> sourceNames){
   if(input==null||input.Count==0)throw new ArgumentException("SCORLAS needs at least one file.");
   if(job==null)throw new ArgumentNullException("job");
   if(sourceNames!=null&&sourceNames.Count!=input.Count)throw new ArgumentException("SCORLAS source-name count differs from files.");
   if(job.Header!="None"&&sourceNames==null)throw new NotSupportedException("SCORLAS captions need the source names supplied to the original file reader.");
   string destination=IOPath.GetFullPath(file),temporary=IOPath.Combine(IOPath.GetDirectoryName(destination),".scorlas-"+Guid.NewGuid().ToString("N")+".tmp");
   var state=new ScorlasFilePlacementState();var frames=new List<ScorlasFilePlacement>();var canvases=new List<Canvas>();var extents=new List<NativePrintExtents>();
   foreach(var canvas in input){if(canvas==null||canvas.Score==null)throw new ArgumentException("Missing composite score.");canvas.LayoutScore();}
   for(int n=input.Count-1;n>=0;n--){var canvas=input[n];var extent=NativePrintMinimum.Extents(canvas);if(!extent.HasPoints){
     if(!NativePrintBehaviour.OriginalScorlasEmpty)throw new NotSupportedException("SCORLAS empty files can produce reversed original EPS bounds. Enable original empty-file bounds in printing preferences to reproduce verified cases.");
     if(canvas.Score.Items.Count!=0)throw new NotSupportedException("SCORLAS nonempty undrawable-file state needs verification.");
     // 0C7A:0421..0439, measured after an empty top file: no scan
     // points replace these sentinels before the normal tiling recurrence.
     extent=new NativePrintExtents{Left=32750,Bottom=32750,Right=-32700,Top=-32750};
    }float p8=0;foreach(var item in canvas.Score.Items)if(item.Code==8&&item.Staff==1)p8=item.P(8);int guide=ScorlasGuideConversion.Convert(p8,BitConverter.ToSingle(canvas.Score.Trailer,canvas.Score.Trailer.Length-12)==1);frames.Add(state.Add(extent.Bottom,extent.Top,guide));canvases.Add(canvas);extents.Add(extent);}
   job.ResolveScorlas(canvases[0]);
   long left=Int64.MaxValue,right=Int64.MinValue;
   for(int n=0;n<extents.Count;n++){left=Math.Min(left,extents[n].Left);right=Math.Max(right,extents[n].Right);}
   // Original 0C7A:06A2..0811 leaves the last scan minimum and accumulated
   // height for the later DSC rewrite, rather than unioning translated ink.
   var finalFrame=frames[frames.Count-1];
   string compositeBox=NativePrintExtents.DscBox(job,left,finalFrame.DscBottom,right,finalFrame.DscTop);
   try{using(var bitmap=new Bitmap(1,1))using(var graphics=Graphics.FromImage(bitmap))using(job.Begin())using(NativePrintContext.Begin(root))using(var eps=new NativeEpsExport(graphics,temporary,canvases[0].PageWidth,canvases[0].PageHeight,compositeBox,null,canvases[0].Score,sourceNames==null?null:sourceNames[input.Count-1],true)){
     // SCORLAS emits the glyph-definition catalogue from the first printed
     // file. Later files reuse it, including an empty initial catalogue.
     var sharedCache=NativeLibrarySymbols.Cache(canvases[0].Score,canvases[0].Library);
     try{for(int n=0;n<canvases.Count;n++){var canvas=canvases[n];var before=canvas.CompositeLibraryCache;try{canvas.CompositeLibraryCache=sharedCache;eps.CompositeFrame(canvas,frames[n].Delta,n==0,job);canvas.PageDocument.DatBarlineUnit=NativePrintContext.ExportGeometryUnit;canvas.Draw(graphics,false);}finally{canvas.CompositeLibraryCache=before;}}}
     catch{eps.Abort();throw;}
    }
    if(File.Exists(destination))File.Replace(temporary,destination,null);else File.Move(temporary,destination);
   }finally{File.Delete(temporary);}
  }
 }
}
