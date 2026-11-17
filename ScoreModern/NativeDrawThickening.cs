using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ScoreModern {
 internal static class NativeDrawThickening {
  const float VerticalSlope=9999;

  struct Edge {
   internal float Slope,Intercept;
   internal PointF Offset;
  }

  internal static void Apply(Glyph glyph,float width,float taper){
   if(glyph.Points.Length<2)throw new InvalidDataException("Draw at least two points before thickening.");
   if(Single.IsNaN(width)||Single.IsInfinity(width)||Single.IsNaN(taper)||Single.IsInfinity(taper))
    throw new InvalidDataException("Enter finite thickening controls.");
   int count=glyph.Points.Length;
   float taperPoints=(float)((double)count*taper);
   double rounded=Math.Round(taperPoints,MidpointRounding.AwayFromZero);
   if(Math.Abs(rounded)>=Int16.MaxValue)throw new InvalidDataException("The taper leaves DRAW's coordinate range.");
   int rampCount=Math.Abs((int)rounded)+1;
   bool ramp=rampCount>=2;
   float step=ramp?(float)(1.0/taperPoints):0;
   float strength=0;
   var reverse=new List<PointF>();
   if(!ramp)reverse.Add(glyph.Points[0]);
   Edge previous=new Edge();
   for(int index=0;index<count-1;index++){
    PointF a=glyph.Points[index],b=glyph.Points[index+1];
    Edge edge=Offset(a,b,width,a);
    PointF point=edge.Offset;
    bool parallel=false;
    if(index!=0)point=Join(previous,edge,a.X==b.X,a.Y==b.Y,out parallel);
    edge.Offset=point;
    previous=edge;
    if(ramp){
     point=new PointF((float)((double)strength*(float)((double)point.X-a.X)+a.X),
      (float)((double)strength*(float)((double)point.Y-a.Y)+a.Y));
     strength=index+2<count-rampCount?Math.Min(1,(float)((double)strength+step)):
      Math.Max(0,(float)((double)strength-step));
    }
    PointF word=Round(point);
    if(!parallel&&(reverse.Count==0||reverse[reverse.Count-1]!=word))reverse.Add(word);
   }
   if(!ramp)reverse.Add(Round(Offset(glyph.Points[count-2],glyph.Points[count-1],width,glyph.Points[count-1]).Offset));
   if(count+reverse.Count>NativeDrawDocument.Capacity)
    throw new InvalidDataException("The thickened contour exceeds DRAW's 4,000-vector capacity.");
   reverse.Reverse();
   var operations=new short[count+reverse.Count];
   Array.Copy(glyph.Operations,operations,count);
   operations[0]=2;
   glyph.Points=glyph.Points.Concat(reverse).ToArray();
   glyph.Operations=operations;
  }

  static Edge Offset(PointF from,PointF to,float width,PointF at){
   float dx=to.X-from.X,dy=to.Y-from.Y;
   if(dx==0){
    float x=(float)((double)at.X+(dy<0?width:-width));
    return new Edge{Offset=new PointF(x,at.Y),Slope=VerticalSlope,Intercept=x};
   }
   if(dy==0)
    return new Edge{Offset=new PointF(at.X,(float)((double)at.Y+(dx<0?-width:width)))};
   float slope=(float)((double)dy/dx);
   float normal=(float)(Math.Sin(Math.Atan(slope))*width);
   if(dx<0)normal=-normal;
   float xOffset=(float)((double)at.X-normal),yOffset=(float)((double)at.Y+(double)normal/slope);
   return new Edge{Offset=new PointF(xOffset,yOffset),Slope=slope,
    Intercept=(float)((double)xOffset-(double)yOffset/slope)};
  }

  static PointF Join(Edge old,Edge next,bool vertical,bool horizontal,out bool parallel){
   parallel=false;
   PointF a=old.Offset,b=next.Offset;
   if(vertical)return old.Slope==0?b:new PointF(b.X,(float)(((double)b.X-a.X)*old.Slope+a.Y));
   if(horizontal)return old.Slope==0?b:new PointF((float)(((double)b.Y-a.Y)/old.Slope+a.X),b.Y);
   if(old.Slope==0)return new PointF((float)((double)a.Y/next.Slope+next.Intercept),a.Y);
   if(old.Slope==VerticalSlope)return new PointF(a.X,(float)(((double)a.X-next.Intercept)*next.Slope));
   if(old.Slope==next.Slope){parallel=true;return b;}
   float denominator=(float)(1.0/next.Slope-1.0/old.Slope);
   float y=(float)(((double)old.Intercept-next.Intercept)/denominator);
   return new PointF((float)((double)y/next.Slope+next.Intercept),y);
  }

  static PointF Round(PointF p){
   double x=Math.Round(p.X,MidpointRounding.AwayFromZero),y=Math.Round(p.Y,MidpointRounding.AwayFromZero);
   if(!(x>=Int16.MinValue&&x<=Int16.MaxValue&&y>=Int16.MinValue&&y<=Int16.MaxValue))
    throw new InvalidDataException("The thickened contour leaves DRAW's coordinate range.");
   return new PointF((float)x,(float)y);
  }
 }
}
