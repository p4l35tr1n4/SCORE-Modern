using System;
namespace ScoreModern.RecoveredFragments {
 // Logical quarter-note duration of an explicit Code 18 signature. Graphic
 // single-number signatures do not specify a beat unit and remain ambiguous.
 public static class InputMeterDuration {
  public static bool TryQuarterBeats(float numerator,float denominator,float secondNumerator,float secondDenominator,out float beats){
   beats=0;if(!Finite(numerator)||!Finite(denominator)||!Finite(secondNumerator)||!Finite(secondDenominator))return false;
   if(numerator==98||numerator==99){beats=4;return secondNumerator==0;}
   if(numerator<=0||denominator<=0||secondNumerator<0)return false;
   beats=4*numerator/denominator;
   if(secondNumerator>0){float unit=secondDenominator>0?secondDenominator:denominator;beats+=4*secondNumerator/unit;}
   return Finite(beats)&&beats>0;
  }
  static bool Finite(float value){return !Single.IsNaN(value)&&!Single.IsInfinity(value);}
 }
}
