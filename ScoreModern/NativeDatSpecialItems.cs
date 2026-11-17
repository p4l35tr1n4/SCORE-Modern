using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern{
 public static class NativeDatSpecialItems{
  static float F(double x){return (float)x;}
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext p,NativeDatGlyphs dat){
   if(i.Code==13)return new List<PrimitiveTextPoint>();
   if(i.Code==7){if(i.P(7)!=0||i.P(6)>i.P(3)||i.P(5)<0||i.P(5)>2)throw new NotSupportedException("DAT trill continuation controls need verification.");for(int n=8;n<=20;n++)if(i.P(n)!=0)throw new NotSupportedException("DAT trill auxiliary controls need verification.");var q=NativeTrillGlyph.Prepare(i.P(5),p.Prepared.X,p.Prepared.Y,p.Scale);return dat.Expand(q.Glyph,q.Horizontal,q.Vertical,p.Scale,q.X,q.Y);}
   if(i.Code==12&&i.P(5)==10)return NativeAccordionButton.Scan(i,p,dat);
   if(i.Code==12&&i.P(5)==2)return NativeGuitarBend.Scan(i,p,dat);
   if(i.Code==12){if(i.P(5)!=0&&i.P(5)!=1)throw new NotSupportedException("DAT special shape subtype needs verification.");for(int n=8;n<=20;n++)if(i.P(n)!=0&&n!=8&&n!=13&&n!=9&&n!=10&&n!=11&&n!=12)throw new NotSupportedException("DAT shape auxiliary controls need verification.");float h=i.P(7)==0?i.P(6):i.P(7);List<PrimitiveTextPoint> points;
    int extra=(short)(int)F((double)i.P(8)*p.Scale/25);
    if(i.P(5)==1){
     int step=(int)i.P(12);if(step==0)step=1;
     int start=(int)i.P(10),stop=(int)i.P(11);
     if(start<0)start+=360;
     if(stop==0)stop=360;
     if(stop<start)stop+=360;
     if(step<1||start<0||start>360||stop>720)throw new NotSupportedException("DAT circle angular domain needs verification.");
     points=dat.ShapeCircle(p.Scale,p.Prepared.X,p.Prepared.Y,i.P(6),h,i.P(13)!=0,step,start,stop);
     float expansion=F((double)7.142899990081787f*extra/p.Scale);
     if(extra>0){
      // A wrapped arc keeps only the initial expanded vertex in SCORE's
      // DAT outline pass. The primary arc still crosses zero normally.
      int outlineEnd=stop>360?start:stop;
      points.AddRange(dat.ShapeCircle(p.Scale,p.Prepared.X,p.Prepared.Y,F((double)i.P(6)+expansion),F((double)h+expansion),false,step,start,outlineEnd));
     }
    }else{
     float factor=F((double)p.Scale*3.5),dx=F((double)factor*i.P(6)),dy=F((double)factor*h),shear=(short)(int)i.P(10)!=0?F((double)p.Scale*i.P(10)*7):0;
     float left=F((double)p.Prepared.X-dx),right=F((double)p.Prepared.X+dx),bottom=F((double)p.Prepared.Y-dy),top=F((double)p.Prepared.Y+dy);
     points=new List<PrimitiveTextPoint>();
     for(int n=0;n<=(extra>0?1:0);n++){
      float delta=25*extra*n;
      var outline=Box(F((double)left-delta),F((double)bottom-delta),F((double)right+delta),F((double)top+delta),i.P(13)!=0);
      for(int k=2;k<4;k++){var q=outline[k];q.X=F((double)q.X+shear);outline[k]=q;}
      points.AddRange(outline);
     }
    }
    if((short)(int)i.P(9)!=0){for(int n=0;n<points.Count;n++){var q=points[n];q.X=NativeNotationContext.Word(q.X,true);q.Y=NativeNotationContext.Word(q.Y,false);points[n]=q;}NativeDatLines.Rotate(points,i.P(9),p.Prepared.X,p.Prepared.Y);}return points;}

   if(i.Code==15){i=IncludedPostScriptDisplay.WorkingItem(i);if(i.P(13)!=0&&!i.ExternalDrawingResetResolved)throw new NotSupportedException("DAT included-PS rotation/reset controls need verification.");
    if(i.P(11)==-9999){float h=i.P(5)==0?1:i.P(5),v=i.P(6)==0?h:i.P(6),factor=F((double)p.Scale/.44999998807907104),right=F(((double)i.P(9)-i.P(7))*h*factor+p.Prepared.X),top=F(((double)i.P(10)-i.P(8))*factor*v+p.Prepared.Y),left=p.Prepared.X,bottom=p.Prepared.Y;var points=new List<PrimitiveTextPoint>{new PrimitiveTextPoint{X=left,Y=bottom,Operation=1},new PrimitiveTextPoint{X=left,Y=top},new PrimitiveTextPoint{X=right,Y=top},new PrimitiveTextPoint{X=right,Y=bottom},new PrimitiveTextPoint{X=left,Y=bottom},new PrimitiveTextPoint{X=right,Y=top},new PrimitiveTextPoint{X=left,Y=top,Operation=1},new PrimitiveTextPoint{X=right,Y=bottom}};if((short)(int)i.P(12)!=0){for(int n=0;n<points.Count;n++){var q=points[n];q.X=NativeNotationContext.Word(q.X,true);q.Y=NativeNotationContext.Word(q.Y,false);points[n]=q;}NativeDatLines.Rotate(points,i.P(12),p.Prepared.X,p.Prepared.Y);}return points;}
    float f=F((double)p.Scale*7),w=i.P(5)==0?3:i.P(5),height=i.P(6)==0?w:i.P(6);return Box(p.Prepared.X,p.Prepared.Y,F((double)w*f+p.Prepared.X),F((double)height*f+p.Prepared.Y),false);}

   throw new NotSupportedException("DAT special item route needs verification.");
  }
  internal static List<PrimitiveTextPoint> Box(float left,float bottom,float right,float top,bool fill){return new List<PrimitiveTextPoint>{new PrimitiveTextPoint{X=left,Y=bottom,Operation=(short)(fill?2:1)},new PrimitiveTextPoint{X=right,Y=bottom},new PrimitiveTextPoint{X=right,Y=top},new PrimitiveTextPoint{X=left,Y=top},new PrimitiveTextPoint{X=left,Y=bottom}};}
 }
}

