using System;using ScoreModern.RecoveredFragments;using System.Collections.Generic;
namespace ScoreModern {
 public static class NativeDatKeys {
  static float F(double x){return (float)x;}static short W(float x){return unchecked((short)(int)Math.Truncate(x));}static bool Present(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0)!=0;}
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext page,NativeDatGlyphs dat){
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT key input outside finite word domain.");
   for(int n=17;n<=20;n++)if(i.P(n)!=0)throw new NotSupportedException("DAT key extended fields need verification.");
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=F((double)page.Prepared.X+i.P(15)*150);var points=new List<PrimitiveTextPoint>();
   if(W(i.P(8))!=0){float gap=F(page.Scale*16.799999237060547);if(Present(i.P(7)))gap=F(i.P(7)*(double)gap);float size=W(i.P(6))>=100?.800000011920929f:1;if(size!=1)gap=F(size*(double)gap);float x=F(page.Scale*15.0+context.Prepared.X);
    foreach(int n in new[]{8,9,10,11,12,13,16}){int value=W(i.P(n));if(value==0)break;int glyph=value/100+5;if(glyph<6||glyph>8)throw new NotSupportedException("DAT irregular key glyph controls need original verification.");short pitch=unchecked((short)(value%100*7));float y=F(pitch*(double)page.Scale+page.Prepared.Y);points.AddRange(dat.Expand(glyph,size,size,page.Scale,x,y));x=F((double)x+gap);}return points;}
   int decimalCode=Math.Abs((int)Math.Round(i.P(5)*100.0,MidpointRounding.AwayFromZero));if(Math.Abs(i.P(5))>109||decimalCode!=0&&(decimalCode<100||decimalCode==180))throw new NotSupportedException("DAT key numeric value reaches an unverified shared-glyph branch.");
int signed=W(i.P(5)),count=Math.Abs(signed),clef=W(i.P(6)),glyphId=signed<0?6:7;if(count>=100){glyphId=8;count%=100;}count=Math.Max(1,count);float staff=page.Scale,x0=F(staff*15.0+context.Prepared.X),advance=F(staff*16.799999237060547),size0=1;if(Present(i.P(7)))advance=F(i.P(7)*(double)advance);if(clef>=100){size0=.800000011920929f;advance=F(size0*(double)advance);clef-=100;}if(clef<0||clef>3)clef=0;
   float[] flats={49,35,42,56},sharps={77,63,70,35};float y0=F((signed<0?flats[clef]:sharps[clef])*(double)staff+page.Prepared.Y),threshold=F((clef==0?84.5:clef==1?70.5:77.5)*staff+page.Prepared.Y),span=F(staff*49.0),signedScale=signed<0?-staff:staff,up=F(signedScale*28.0),down=F(-signedScale*21.0);int direction=signed>=0&&clef==3?1:-1;
   for(int n=0;n<count;n++){points.AddRange(dat.Expand(glyphId,size0,size0,staff,x0,y0));direction=-direction;x0=F((double)x0+advance);y0=F((double)y0+(direction>=0?down:up));if(y0>threshold){y0=F((double)y0-span);direction=-direction;}}return points;

  }
 }
}
