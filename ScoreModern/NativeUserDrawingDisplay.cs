using System;using System.Drawing;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeUserDrawingDisplay {
  public static bool Draw(Graphics graphics,Item item,NativeNotationContext page,DrawingLibrary library){
   if(page==null||library==null)return false;
   float copies=item.P(8),outline=item.P(10);
   if((Single.IsNaN(copies)||copies< -1||copies>32767||copies!=(int)copies)||
    Single.IsNaN(outline)||outline<-32768||outline>32767||outline!=(int)outline)
    throw new NotSupportedException("Printed Code11 thickness control outside the verified range.");
   int selector;
   var glyph=NativeDatUserDrawings.Load(item,library,out selector);
   var buffer=NativeDatUserDrawings.Prepare(item,page,library,false);
   if(copies>0&&(long)buffer.Count*(1+2*((int)copies/100+(int)copies%100))>30000)
    throw new NotSupportedException("Printed Code11 thickness exceeds the verified buffer domain.");
   var points=buffer.ConvertAll(p=>new BarlinePoint(p.X,p.Y,copies==-1&&p.Operation>0?(short)1:p.Operation));
   float width=NativePrintContext.Width((short)outline);
   if(copies>0&&width==NativePrintContext.DefaultLineWidth){
    // Original F6 strokes the base drawing and all same-width copies together.
    // Separate strokes darken overlapping edges in the EPS raster.
    var original=points.ToArray();
    for(int n=1;n<=(int)copies/100;n++)NativeLibrarySymbols.AppendThickness(points,original,false,n);
    for(int n=1;n<=(int)copies%100;n++)NativeLibrarySymbols.AppendThickness(points,original,true,n);
    NativeNoteDetails.Path(graphics,page,points,width,true);
   }else{
    NativeNoteDetails.Path(graphics,page,points,width,true);
    if(copies>0){
     var original=points.ToArray();
     for(int n=1;n<=(int)copies/100;n++)PaintCopies(graphics,page,points,original,false,n);
     for(int n=1;n<=(int)copies%100;n++)PaintCopies(graphics,page,points,original,true,n);
    }
   }
   RenderResourceAudit.Record("user DRW vector",selector+":"+glyph.Name,glyph.Source,"external named drawing; verified printed transform and thickness");
   return true;
  }
  static void PaintCopies(Graphics graphics,NativeNotationContext page,List<BarlinePoint> points,BarlinePoint[] original,bool horizontal,int count){
   points.Clear();
   NativeLibrarySymbols.AppendThickness(points,original,horizontal,count);
   NativeNoteDetails.Path(graphics,page,points.GetRange(0,original.Length),NativePrintContext.DefaultLineWidth,true);
   NativeNoteDetails.Path(graphics,page,points.GetRange(original.Length,original.Length),NativePrintContext.DefaultLineWidth,true);
  }
 }
}
