using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutRestSpacingResult {public float Multiplier,Right,Left;public float? Parameter10Scratch;}
 public static class LayoutRestSpacing {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  // Original 2F68:06BE..08BC. All literals match both the original unpacked
  // initial data and measured runtime data (compiler-constants.json).
  // This is a spacing helper, not a glyph-bounds approximation.
  public static LayoutRestSpacingResult Apply(float multiplier,float p6,float p5,float p4,float left,short parameterCount,float p9,float p10,float p15) {
   foreach(float f in new[]{multiplier,p6,p5,p4,left,p9,p10,p15})if(Single.IsNaN(f)||Single.IsInfinity(f))throw new NotSupportedException("Non-finite rest-spacing controls are outside this verified helper domain.");
   float right=4.5f;
   if(p5<0) {
    right=6;
    if(p5==-4)right=9.5f;
    if(parameterCount>=9&&Bits(p9)!=0)right=1;
   } else if(p5>1)right=(float)(((double)p5-1)*.5+right);
   if(p6!=0)right=(float)((double)p6*1.75+right);
   if(Math.Abs((double)p4)>=100)multiplier=.6f;
   float? scratch=null;
   if(parameterCount>=10) {
    scratch=p10;
    if(Bits(p10)!=0){left=-p10;right=(float)((double)p10+right);}
    // Original 088F is FILD dword, then the later multiply reads the same
    // bytes as float. In the verified zero comparison that is a raw-bit
    // nonzero test: a negative-zero P15 must reach the multiply too.
    if(parameterCount>=15&&Bits(p15)!=0)multiplier=(float)((double)p15*multiplier);
   }
   return new LayoutRestSpacingResult{Multiplier=multiplier,Right=right,Left=left,Parameter10Scratch=scratch};
  }
 }
}
