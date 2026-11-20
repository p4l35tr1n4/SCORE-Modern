using System;
using System.Globalization;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal sealed class NativePrintExtents {
  internal short Left=32760,Bottom=32750,Right=-32600,Top=short.MinValue;
  internal bool HasPoints;
  internal void Include(short x,short y){
   HasPoints=true;
   Left=Math.Min(Left,x);Bottom=Math.Min(Bottom,y);
   Right=Math.Max(Right,x);Top=Math.Max(Top,y);
  }
  internal string DscBox(NativePrintJob job){
   if(!HasPoints)return "0 0 0 0";
   return DscBox(job,Left,Bottom,Right,Top);
  }
  internal static string DscBox(NativePrintJob job,long leftRaw,long bottomRaw,long rightRaw,long topRaw){
   double scale=job.Scale;
   int offsetX=job.LeftMargin,offsetY=job.BottomMargin;
   // Original 3046:0238–02B3 rotates the scan extrema, independently
   // of the PostScript transform. Mirror affects engraving only.
   if(job.Rotate){
    offsetX=(int)Math.Truncate(34000.0/job.Size-offsetY);
    offsetY=unchecked((short)job.LeftMargin);
    long oldLeft=leftRaw,oldRight=rightRaw;leftRaw=-topRaw;
    rightRaw=-bottomRaw;
    bottomRaw=oldLeft-266;
    topRaw=oldRight-266;
   }
   double left=(offsetX+leftRaw)*scale,right=(offsetX+rightRaw)*scale;
   double bottom=(offsetY+bottomRaw)*scale,top=(offsetY+topRaw)*scale;
   return String.Join(" ",Array.ConvertAll(new[]{Round(left)-1,Round(bottom)-1,Round(right)+1,Round(top)+1},n=>n.ToString(CultureInfo.InvariantCulture)));
  }
  static int Round(double value){return checked((int)Math.Round(value,MidpointRounding.ToEven));}
 }
 public static class NativePrintMinimum {
  public static short Scan(Canvas canvas,Action<int,short> checkpoint=null){return Extents(canvas,checkpoint).Bottom;}
  internal static NativePrintExtents Extents(Canvas canvas,Action<int,short> checkpoint=null){
   if(canvas==null||canvas.Score==null||canvas.Library==null)
    throw new InvalidOperationException("A score and external library are required for automatic print offsets.");
   canvas.RebuildGuideMeasurement=true;canvas.LayoutScore();
   var dat=new NativeDatGlyphs(canvas.Library.Root);
   var cache=new NativeDatHeadCache();
   var extent=new NativePrintExtents();
   short font=0;
   int index=0;
   foreach(var item in canvas.Score.Items){
    index++;
    try{
     short next;
     var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,cache,font,out next);
     foreach(var p in points){
      if(p.CommandOnly)continue;
      short x=NativeNotationContext.Word(p.X,true),y=NativeNotationContext.Word(p.Y,false);
      if(x>=-32600)extent.Include(x,y);
     }
     font=next;
    }catch(Exception error){
     if(!(error is NotSupportedException||error is System.IO.InvalidDataException||error is System.IO.IOException))throw;
     throw new NotSupportedException("Automatic print-offset scan item "+index+": "+error.Message,error);
    }
    if(checkpoint!=null)checkpoint(index,extent.Bottom);
   }
   return extent;
  }
 }
}

