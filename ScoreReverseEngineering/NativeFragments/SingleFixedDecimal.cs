using System;using System.Globalization;using System.Numerics;
namespace ScoreModern.RecoveredFragments {
 // Numeric value only: original single-real fixed output caps significant
 // digits at DS:5C36=7 (runtime 272C:0EF6..0F1B), then rounds discarded
 // digits >= '5' with carry (20DD:0CA4). Field padding is separate.
 public static class SingleFixedDecimal {
  public static string Format(float value,int decimalPlaces){
   if(float.IsNaN(value)||float.IsInfinity(value))throw new NotSupportedException("Non-finite original decimal formatting is not verified.");
   if(decimalPlaces<0||decimalPlaces>9)throw new ArgumentOutOfRangeException("decimalPlaces");
   uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(value),0);bool negative=(bits&0x80000000)!=0;uint mantissa=bits&0x7fffff;int exponent=(int)((bits>>23)&255);if(exponent!=0)mantissa|=0x800000;int shift=exponent==0?-149:exponent-150;BigInteger numerator=mantissa,denominator=BigInteger.One;if(shift>=0)numerator<<=shift;else denominator<<=-shift;
   BigInteger rounded=BigInteger.Zero;
   if(mantissa!=0){int e=(int)Math.Floor(Math.Log10(Math.Abs((double)value)));Func<int,int> compare=power=>power>=0?numerator.CompareTo(denominator*BigInteger.Pow(10,power)):(numerator*BigInteger.Pow(10,-power)).CompareTo(denominator);while(compare(e)<0)e--;while(compare(e+1)>=0)e++;int places=Math.Min(decimalPlaces,6-e);BigInteger n=numerator,d=denominator;if(places>=0)n*=BigInteger.Pow(10,places);else d*=BigInteger.Pow(10,-places);BigInteger remainder;rounded=BigInteger.DivRem(n,d,out remainder);if(remainder*2>=d)rounded++;rounded*=BigInteger.Pow(10,decimalPlaces-places);}
   string digits=rounded.ToString(CultureInfo.InvariantCulture);if(decimalPlaces>0){digits=digits.PadLeft(decimalPlaces+1,'0');digits=digits.Insert(digits.Length-decimalPlaces,".");}return (negative?"-":"")+digits;
  }
 }
}
