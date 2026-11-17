using System;
namespace ScoreModern.RecoveredFragments {
 // Original resident 11DA:0578..0667. The collector supplies signed
 // one-based float-word references. References remain reusable and ordered.
 public static class LayoutCoordinateAffine {
  public static void Apply(float[] words,short[] references,float first,float second,float third,short mode){
   if(mode==3)mode=-3;float offset=0;if(mode<0)offset=(float)((double)second-(double)first*third);
   foreach(short reference in references){int index=Math.Abs((int)reference)-1;if(index<0||index>=words.Length)throw new NotSupportedException("Original affine coordinate reference leaves its buffer.");words[index]=mode<0?(float)((double)words[index]*first+offset):(float)((double)words[index]+first);}
  }
 }
}
