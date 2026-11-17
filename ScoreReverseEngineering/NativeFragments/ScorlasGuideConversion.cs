using System;
namespace ScoreModern.RecoveredFragments {
 // Original SCORLAS 0C7A:057E..05B6 loads the single-precision factor,
 // multiplies the single-precision P8 in the x87 register, then calls
 // 1F6B:083A. That helper selects truncation, FISTPs a signed qword,
 // restores the control word and returns its low two words.
 public static class ScorlasGuideConversion {
  public static int Convert(float value,bool metric){
   double product=(double)value*(metric?(double)1574.8031005859375f:4000.0);
   // Keep unmeasured overflow/exception states outside this valid domain.
   if(Double.IsNaN(product)||Double.IsInfinity(product)||product<Int32.MinValue||product>Int32.MaxValue)throw new NotSupportedException("SCORLAS guide conversion outside the verified finite signed32 domain.");
   return (int)Math.Truncate(product);
  }
 }
}
