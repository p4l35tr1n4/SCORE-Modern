using System;
namespace ScoreModern.RecoveredFragments {
 public struct TextPreparedParameters {
  public float Spacing,Horizontal,Vertical,GlyphHorizontal,GlyphVertical,Origin,Rotation,RestoredHorizontal,RestoredVertical;
 }
 public static class TextParameterPreparation {
  static float F(double v){return (float)v;}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  // Relative 14A6:0017..0080, 00A8..00D1, 01BB..0223.
  // Finite scalar projection; caller supplies measured constants and initial X.
  // Character decoding, font override services and x87 flags are outside this API.
  public static TextPreparedParameters Prepare(float[] p,float x,float modulus,float glyphScale,float units){
   float h=F((double)p[6]%modulus);if(!Present(h))h=1;
   float v=Present(p[7])?p[7]:h;
   return new TextPreparedParameters{Spacing=Present(p[5])?p[5]:1,Horizontal=h,Vertical=v,
    GlyphHorizontal=F((double)h*glyphScale),GlyphVertical=F((double)v*glyphScale),
    Origin=F((double)p[11]*units+x),Rotation=p[9],RestoredHorizontal=p[6],RestoredVertical=p[7]};
  }
  // Relative 14A6:00F3..019B. The counter begins at -1, so this distributes
  // the width difference over gaps, rather than over the number of glyphs.
  public static float Interletter(float total,float factor,float[] advances){
   if(factor==1)return 0;int gaps=-1;foreach(float value in advances)if(value!=0)gaps++;
   return gaps<1?0:F(((double)total-(double)total/factor)/gaps);
  }
  public static float[] Adjust(float[] advances,float delta){
   var result=(float[])advances.Clone();for(int j=0;j<result.Length-1;j++)if(Present(result[j]))result[j]=F((double)result[j]+delta);return result;
  }
  // Relative 14A6:0231..026E; screen and printed units remain explicit.
  public static float OutputExtra(float value,bool printed,float units){return printed?F((double)value*units):value;}
 }
}
