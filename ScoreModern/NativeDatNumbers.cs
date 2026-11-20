using ScoreModern.RecoveredFragments;
using System;
using System.Collections.Generic;
namespace ScoreModern {
 public static class NativeDatNumbers {
  static float F(double x){return (float)x;}
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext page,ScoreText text,NativeDatGlyphs dat,out short finalFont){
   NativeNumberFrame frame;
   if(text==null||!NativeNumberDisplay.Prepare(i,page,out frame))
    throw new NotSupportedException("DAT number controls outside the measured route.");
   string value=NativeNumberDisplay.Value(frame.Number);
   var body=NativeNumberDisplay.TextItem(i,frame);
   var position=NativeNumberDisplay.Position(page,frame);
   var points=text.ScanStrokeText(body,value,position,0,out finalFont);
   if(frame.Enclosure==0)return points;
   float width=text.PrepareLayout(body,value,page.Scale,0,true).Width;
   var area=NativeNumberDisplay.Enclosure(i,page,frame,width);
   int thickness=i.P(10)==0?1:(int)i.P(10);
   int copies=frame.Enclosure>2?0:(int)F((double)thickness*page.Scale/25);
   if(frame.Enclosure==2){
    for(int n=0;n<=(copies>0?1:0);n++){
     float delta=25*copies*n;
     points.AddRange(NativeDatSpecialItems.Box(F((double)area.X-area.Width-delta),F((double)area.Y-area.Height-delta),
      F((double)area.X+area.Width+delta),F((double)area.Y+area.Height+delta),false));
    }
    return points;
   }
   float factor=F((double)page.Scale*3.5);
   float h=F((double)area.Width/factor),v=F((double)area.Height/factor);
   points.AddRange(dat.Circle(page.Scale,area.X,area.Y,h,v,false,12));
   {
    if(copies>0){
     float delta=F((double)7.142899990081787f*copies/page.Scale);
     points.AddRange(dat.Circle(page.Scale,area.X,area.Y,F((double)h+delta),F((double)v+delta),false,12));
    }
   }
   return points;
  }
 }
}
