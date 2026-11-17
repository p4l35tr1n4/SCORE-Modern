using System;
namespace ScoreModern.RecoveredFragments {
 // Original 3111:0004..0300 coordinate-allocation projection. Numerical
 // power uses the same candidate projection as F2, checked at stored float
 // boundaries; this does not claim x87 ABI/exception/flag equivalence.
 public static class LayoutLineupCoordinates {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup coordinate conversion leaves the finite signed-word domain.");return (short)Math.Truncate(f);}
  public static void Apply(float[] merged,float[] positions,float[] weights,short count,float left,float right,float staffUnit,float exponent) {
   Apply(merged,positions,weights,count,left,right,staffUnit,exponent,false);
  }
  internal static void Apply(float[] merged,float[] positions,float[] weights,short count,float left,float right,float staffUnit,float exponent,bool roundEachPosition) {
   if(count<1||count>=positions.Length||count>weights.Length||count>merged.Length)throw new NotSupportedException("Original lineup coordinate table extent is invalid.");
   float positiveTotal=0,symbolTotal=0;
   for(int i=0;i<count;i++) {
    int kind=Word(merged[i]);float width;
    if(kind<0) {
     width=0;
     if(i!=count-1){width=3.5f;int next=Word(merged[i+1]);
      if(kind==-14||kind==-140){if(next==-15||next==-17)width=1.5f;if(next==-18)width=1;}
      else if(kind==-15){width=12;if(next< -16){width=8;if(next==-18)width=6;}}
      else if(kind==-18&&next==-14)width=4.6f;
      symbolTotal=(float)((double)symbolTotal+width);
     }
    } else {double power=Math.Pow(merged[i],exponent);width=(float)power;positiveTotal=(float)(power+positiveTotal);}
    weights[i]=width;
   }
   float symbolScale=(float)((double)staffUnit/25);
   if(positiveTotal==0)throw new NotSupportedException("Original lineup coordinate allocation divides by zero.");
   float noteScale=(float)(((double)right-left-(double)symbolTotal*symbolScale)/positiveTotal);
   double current=left;positions[0]=(float)current;
   for(int i=0;i<count;i++) {
    float scale=Bits(merged[i])>0?noteScale:symbolScale;
    weights[i]=(float)((double)weights[i]*scale);
    current+=weights[i];
    positions[i+1]=(float)current;
    if(roundEachPosition)current=positions[i+1];
   }
  }
 }
}
