using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct TupletBracketPoint {public short Tag;public float X,Y;public TupletBracketPoint(short tag,float x,float y){Tag=tag;X=x;Y=y;}}
 public sealed class TupletBracketContext {public float Scale,LeftX,LeftY,RightY;}
 public sealed class TupletBracketResult {public readonly List<TupletBracketPoint> Points=new List<TupletBracketPoint>();public TupletPlacement? Number;}
 public static class TupletBracket {
  static float F(double value){return (float)value;}
  static bool Present(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  static short I(float value){return unchecked((short)(int)Math.Truncate(value));}
  // Geometry projection of relative 1149:000C..00B9,12D0..1653 and
  // 18A7..18E4 for P8 word 3. Caller coordinates are supplied explicitly.
  public static TupletBracketResult Build(float[] p,TupletBracketContext c){
   if(p==null||p.Length<21||c==null)throw new ArgumentException("Missing bracket context or parameters.");
   if(I(p[8])!=3)throw new NotSupportedException("This path is the original Code5 tuplet bracket.");
   var result=new TupletBracketResult();float scale=c.Scale,left=c.LeftX,leftY=c.LeftY;
   float percent=Present(p[10])?p[10]:.5f;
   float height=F((double)p[7]*scale*7),dy=F((double)c.RightY-leftY),top=F((double)height+leftY);
   float right=F((double)p[6]*150);short side=I(p[9]),mode=I(p[11]);short start=1;
   if(side!=-1){result.Points.Add(new TupletBracketPoint(1,left,leftY));start=0;}
   result.Points.Add(new TupletBracketPoint(start,left,top));
   short number=side>0?side:(short)3;float gap=F((double)scale*13);if(number>9)gap=F((double)gap*1.5f);
   double extendedRight=(double)scale*17.5f+right;right=F(extendedRight);float dx=F(extendedRight-left);
   if(side< -1)gap=F(-(double)p[9]*75);
   float along=F((double)dx*percent),gapLeft=F((double)left-gap+along),numberY;
   if(Present(dy)){
    leftY=F((double)dy+leftY);
    numberY=F((double)height-(1.0-percent)*dy+leftY);
    float slope=F((double)dy/dx);
    if(mode==0)result.Points.Add(new TupletBracketPoint(0,gapLeft,F(((double)gapLeft-c.LeftX)*slope+top)));
    gapLeft=F((double)gap+gap+gapLeft);
    if(mode==0)result.Points.Add(new TupletBracketPoint(1,gapLeft,F(((double)gapLeft-c.LeftX)*slope+top)));
    top=F((double)dy+top);
   }else{
    numberY=top;
    if(mode==0){result.Points.Add(new TupletBracketPoint(0,gapLeft,top));float gapRight=F((double)gap+gap+gapLeft);result.Points.Add(new TupletBracketPoint(1,gapRight,top));}
   }
   result.Points.Add(new TupletBracketPoint(0,right,top));result.Points.Add(new TupletBracketPoint(0,right,leftY));
   if(side< -1||mode==3)return result;
   if(mode!=0){float offset=F(-(double)scale*14);short leg=I(p[7]);if(mode==2)leg=unchecked((short)-leg);if(leg<0)offset=-offset;numberY=F((double)numberY+offset);}
   float textY=F((double)numberY-(double)scale*9);textY=F((double)p[18]*scale*7+textY);
   float textLeft=F((double)p[17]*scale*6+left);
   result.Number=new TupletPlacement{Number=number,X=TupletGeometry.Center(number,scale,along,textLeft),Y=textY,FontScale=.875f};return result;
  }
 }
}
