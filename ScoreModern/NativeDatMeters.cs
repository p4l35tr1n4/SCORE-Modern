using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatMeters {
  // Relative0B0D:0340..0785; keep the mutable top-height and signed denominator
  // state used by the original two-pass meter routine.
  static float F(double x){return (float)x;}
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext p,ScoreText text,NativeDatGlyphs dat){
   for(int n=3;n<=13;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n)))throw new NotSupportedException("DAT compound meter nonfinite controls need fault verification.");
   int a=(short)(int)i.P(5),b=(short)(int)i.P(6),c=(short)(int)i.P(8),d=(short)(int)i.P(9);if(a<0||a>=90||b<0||b>=90||c<1||c>=90||d< -1||d>=90||!NativeMeterDisplay.ValidSize(i.P(7)))throw new NotSupportedException("DAT compound meter numeric domain needs verification.");
   if(!NativeMeterDisplay.VerifiedReservedControl(i.P(16)))throw new NotSupportedException("DAT compound meter P16 needs verification.");
   if(!NativeMeterDisplay.VerifiedCompoundP17(i.P(17)))throw new NotSupportedException("DAT compound meter P17 needs verification.");
   for(int n=18;n<=20;n++)if(i.P(n)!=0)throw new NotSupportedException("DAT compound meter auxiliary controls need verification.");
   if(!NativeMeterDisplay.VerifiedJustificationControl(i.P(14)))throw new NotSupportedException("DAT compound meter layout control outside the measured domain.");
   if(Single.IsNaN(i.P(15))||Single.IsInfinity(i.P(15)))throw new NotSupportedException("DAT compound meter displacement must be finite.");
   float rawSize=i.P(7)==0?1:i.P(7),size=F((double)rawSize*1.25),origin=F((double)p.Prepared.X+i.P(15)*150),unit=F((double)p.Scale*rawSize*6),extra=F(((double)i.P(11)+4.199999809265137)*unit),plus=origin;bool suppress=NativeMeterDisplay.SuppressCompoundPlus(i.P(12));
   if(d==0)d=-1;if(!suppress){float gap=a>9||b>9&&d>0?6.599999904632568f:3.5f;plus=F(((double)gap+i.P(10))*unit+plus);}var result=new List<PrimitiveTextPoint>();
   Action<int,float,float> number=(n,x,y)=>{var context=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=p.Prepared,Projection=p.Projection};context.Prepared.X=x;context.Prepared.Y=y;short font;var body=Item.Make(16,i.P(2),0,0,1,size,0,i.P(13)==0?94:i.P(13),0,0,0,1,0);result.AddRange(text.ScanStrokeText(body,n.ToString(System.Globalization.CultureInfo.InvariantCulture),context,0,out font));};
   float firstTop=a==0?4.079999923706055f:5.71999979019165f;float centered=F((double)p.Scale*size*7.599999904632568+origin),topX=a>0&&a<10&&b>=10&&d>=0?centered:origin,bottomX=b<10&&a>=10&&d>=0?centered:origin;
   number(a==0?b:a,topX,p.Vertical(F((double)size*firstTop+i.P(4))));
   if(a!=0&&b>0){if(d<0){float width=b>9?2.4000000953674316f:3.799999952316284f;if(a>9)width=F((double)width+3.4000000953674316);bottomX=F(((double)width+i.P(10))*p.Scale*size*4.800000190734863+origin);}number(b,bottomX,p.Vertical(F((double)size*2.4800000190734863+i.P(4))));}
   if(!suppress){float height=d>0?4.71999979019165f:6.599999904632568f;result.AddRange(dat.Expand(38,size,size,p.Scale,plus,p.Vertical(F((double)height*size+i.P(4)))));}
   float second=F((double)plus+extra),secondCentered=F((double)p.Scale*size*7.599999904632568+second);number(c,c<10&&d>=10?secondCentered:second,p.Vertical(F((double)size*firstTop+i.P(4))));if(d>0)number(d,d<10&&c>=10?secondCentered:second,p.Vertical(F((double)size*2.4800000190734863+i.P(4))));return result;
  }
 }
}
