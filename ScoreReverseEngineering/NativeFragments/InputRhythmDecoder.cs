using System;
namespace ScoreModern.RecoveredFragments {
 public struct InputRhythmValue {
  public float Duration,DotControl,Denominator,FlagCount;
 }
 public static class InputRhythmDecoder {
  // Numerical projection of original section 56, 0263D:000D..012F.
  // Requires finite positive, bounded input. Stores to binary32 are explicit.
  // This does not model x87 exception/condition flags or invalid conversion states.
  public static InputRhythmValue Decode(float encoded) {
   if(float.IsNaN(encoded)||float.IsInfinity(encoded)||encoded<0||encoded>=3276800f)throw new ArgumentOutOfRangeException("encoded");
   float duration=(float)((double)encoded%1000.0);
   var result=new InputRhythmValue{Duration=duration};
   if(duration<=0)return result;
   short dots=ConvertWord((double)encoded/100.0);
   result.DotControl=dots;
   float baseDuration=duration;
   if(dots!=0){double power=Math.Pow(2.0,dots/10.0);float storedPower=(float)power;baseDuration=(float)((double)duration*storedPower/(storedPower*2.0-1.0));}
   double quotient=4.0/baseDuration;result.Denominator=(float)quotient;
   short denominator=ConvertWord(quotient+0.0010000000474974513);
   short threshold=8,count=1;
   while(denominator>=threshold&&count<11){threshold=unchecked((short)(threshold<<1));count++;}
   result.FlagCount=count-1;
   return result;
  }
  static short ConvertWord(double value){if(value<short.MinValue||value>=32768||double.IsNaN(value))throw new ArgumentOutOfRangeException("encoded","Original conversion is outside this finite numerical projection.");return (short)Math.Truncate(value);}
 }
}
