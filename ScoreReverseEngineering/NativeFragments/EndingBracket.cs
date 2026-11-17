using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class EndingBracketResult {
  public readonly List<TupletBracketPoint> Points=new List<TupletBracketPoint>();
  public TupletPlacement? Number;public float DotX,DotY;public bool HasDot;
 }
 public static class EndingBracket {
  static float F(double v){return (float)v;}
  static short I(float v){return unchecked((short)(int)Math.Truncate(v));}
  // Geometry/request projection of 1149:12D0..1396,1656..1758,1627..1653.
  // The optional tenth ending uses the measured 4..9 ending path with number 10.
  public static TupletBracketPoint DotPoint(float x,float y,float scale,EndingBracketResult r){float unit=F((double)scale*.25f);return new TupletBracketPoint(0,F((double)x*unit+r.DotX),F((double)y*unit+r.DotY));}
  public static EndingBracketResult Build(float[] p,TupletBracketContext c,bool printed,bool tomsTenth){
   if(p==null||p.Length<21||c==null)throw new ArgumentException("Missing ending context.");
   short subtype=I(p[8]),side=I(p[9]);bool tenth=subtype==10&&tomsTenth;
   if(subtype<1||subtype==3||subtype>9&&!tenth)throw new NotSupportedException("Not an ending bracket.");
   if(tenth)subtype=9;
   var r=new EndingBracketResult();float scale=c.Scale;
   float height=F((double)p[7]*scale*7),top=F((double)height+c.LeftY),right=F((double)p[6]*150);
   short start=1;
   if(side!=-1||subtype==2){r.Points.Add(new TupletBracketPoint(1,c.LeftX,c.LeftY));start=0;}
   r.Points.Add(new TupletBracketPoint(start,c.LeftX,top));
   r.Points.Add(new TupletBracketPoint(0,right,top));
   if(subtype==1)r.Points.Add(new TupletBracketPoint(0,right,c.LeftY));
   if(side<0)return r;
   short number=side>0?side:(tenth?(short)10:subtype);
   float numberY=F((double)top-(double)scale*28),along=F((double)scale*(printed?13.5f:14.5f));
   r.DotX=F((double)scale*(subtype==1?2:5)+c.LeftX+along);r.DotY=F((double)numberY+scale);r.HasDot=true;
   r.Number=new TupletPlacement{Number=number,X=TupletGeometry.Center(number,scale,along,c.LeftX),Y=numberY,FontScale=1};
   return r;
  }
 }
}
