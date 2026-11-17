using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativeDrawCurve {
  // DRAW leaves the last row's upper coefficient and the solution beyond
  // the active rows in its work area between curve commands.
  readonly float[] savedUpper=new float[50];
  readonly PointF[] savedSlope=new PointF[51];

  internal void Replace(Glyph glyph,int start,int end,int divisions){
   int count=end-start+1;
   if(start<0||end>=glyph.Points.Length||count<2||divisions<1)
    throw new InvalidDataException("Select consecutive points on one contour.");
   if(glyph.Operations.Skip(start+1).Take(count-1).Any(op=>op>0))
    throw new InvalidDataException("Select points within a single contour.");
   if((long)(count-1)*divisions+1+glyph.Points.Length-count>NativeDrawDocument.Capacity)
    throw new InvalidDataException("The curve exceeds DRAW's 4,000-vector capacity.");
   var anchors=glyph.Points.Skip(start).Take(count).ToList();
   bool closed=anchors[0]==anchors[count-1];
   if(count+(closed?2:0)>50)
    throw new InvalidDataException("Select at most 50 open or 48 closed curve points.");
   int lastY=Math.Abs((int)anchors[count-1].Y);
   int factor=lastY>1600?16000/lastY:10;
   if(factor==0)throw new InvalidDataException("The final Y coordinate exceeds the curve scaling range.");
   if(closed){
    anchors.Add(anchors[1]);
    anchors.Insert(0,anchors[count-2]);
   }
   var knots=anchors.Select(p=>new PointF(unchecked((short)((int)p.X*factor)),
    unchecked((short)((int)p.Y*factor)))).ToArray();
   var distance=new float[knots.Length-1];
   for(int i=0;i<distance.Length;i++){
    short dx=Difference(knots[i+1].X,knots[i].X),dy=Difference(knots[i+1].Y,knots[i].Y);
    distance[i]=(float)Math.Sqrt((double)(float)(dx*dx)+dy*dy);
   }
   PointF[] slopes=Solve(knots,distance);
   var points=new List<PointF>(glyph.Points.Take(start));
   var operations=new List<short>(glyph.Operations.Take(start));
   int first=closed?1:0,last=distance.Length-(closed?1:0);
   for(int i=first;i<last;i++){
    float chord=NonZero(distance[i]);
    double squared=(double)chord*chord;
    float step=(float)((double)chord/divisions);
    float sample=0;
    for(int j=0;j<divisions;j++){
     points.Add(new PointF(Evaluate(knots[i].X,knots[i+1].X,slopes[i].X,slopes[i+1].X,chord,squared,sample,factor),
      Evaluate(knots[i].Y,knots[i+1].Y,slopes[i].Y,slopes[i+1].Y,chord,squared,sample,factor)));
     operations.Add(j!=0?(short)0:i!=first?(short)-1:glyph.Operations[start]>0?glyph.Operations[start]:(short)-2);
     sample=(float)((double)sample+step);
    }
   }
   points.Add(glyph.Points[end]);operations.Add(-3);
   points.AddRange(glyph.Points.Skip(end+1));operations.AddRange(glyph.Operations.Skip(end+1));
   glyph.Points=points.ToArray();glyph.Operations=operations.ToArray();
  }

  static short Difference(float a,float b){return unchecked((short)((int)a-(int)b));}
  static float NonZero(float value){return value==0?.1f:value;}

  PointF[] Solve(PointF[] knots,float[] h){
   int last=knots.Length-1;
   var diagonal=new float[knots.Length];
   var rhs=new PointF[knots.Length];
   diagonal[0]=1;savedUpper[0]=.5f;
   rhs[0]=Delta(knots[0],knots[1],(float)(1.5/NonZero(h[0])));
   for(int i=1;i<=last;i++){
    float lower;
    if(i==last){
     lower=2;diagonal[i]=4;
     rhs[i]=Delta(knots[i-1],knots[i],(float)(6.0/NonZero(h[i-1])));
    }else{
     float left=h[i-1],right=h[i];
     lower=right;savedUpper[i]=left;
     diagonal[i]=(float)(2*((double)left+right));
     float leftSquared=(float)((double)left*left),rightSquared=(float)((double)right*right);
     float scale=(float)(3.0/NonZero((float)((double)left*right)));
     rhs[i]=new PointF(
      (float)(scale*((double)Difference(knots[i+1].X,knots[i].X)*leftSquared+(double)Difference(knots[i].X,knots[i-1].X)*rightSquared)),
      (float)(scale*((double)Difference(knots[i+1].Y,knots[i].Y)*leftSquared+(double)Difference(knots[i].Y,knots[i-1].Y)*rightSquared)));
    }
    if(lower==0)continue;
    float ratio=(float)((double)diagonal[i-1]/lower);
    float divisor=NonZero((float)((double)diagonal[i]*ratio-savedUpper[i-1]));
    float upper=(float)((double)savedUpper[i]*ratio);
    savedUpper[i]=(float)((double)upper/divisor);
    rhs[i]=new PointF(
     (float)((double)(float)((double)rhs[i].X*ratio-rhs[i-1].X)/divisor),
     (float)((double)(float)((double)rhs[i].Y*ratio-rhs[i-1].Y)/divisor));
    diagonal[i]=1;
   }
   for(int i=last;i>=0;i--){
    float divisor=NonZero(diagonal[i]);
    savedSlope[i]=new PointF(
     (float)(((double)rhs[i].X-(double)savedUpper[i]*savedSlope[i+1].X)/divisor),
     (float)(((double)rhs[i].Y-(double)savedUpper[i]*savedSlope[i+1].Y)/divisor));
   }
   return savedSlope;
  }

  static PointF Delta(PointF a,PointF b,float scale){
   return new PointF((float)((double)Difference(b.X,a.X)*scale),(float)((double)Difference(b.Y,a.Y)*scale));
  }

  static float Evaluate(float left,float right,float fromSlope,float toSlope,float chord,double squared,
    float sample,int factor){
   short delta=Difference(right,left);
   float quadratic=(float)((double)delta*(float)(3/squared)-
    (toSlope+2.0*fromSlope)*(float)(1.0/chord));
   float cubic=(float)((toSlope+(double)fromSlope)*(float)(1/squared)+
    (double)delta*(float)(-2/(squared*chord)));
   double power=(double)sample*sample;
   float value=(float)((double)fromSlope*sample+quadratic*(double)(float)power+cubic*(double)(float)(power*sample)+left);
   return unchecked((short)Math.Round((double)value/factor,MidpointRounding.AwayFromZero));
  }
 }
}
