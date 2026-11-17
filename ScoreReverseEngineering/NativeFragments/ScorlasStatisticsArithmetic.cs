using System;
using System.Numerics;
namespace ScoreModern.RecoveredFragments {
 // Original SCORLAS 0824:06F4..0733: signed32 ADD/ADC 111,
 // retained FILD*REAL4 size/4000, FSTP REAL4; metric then multiplies
 // that stored REAL4 by the stored REAL4 2.54 and stores again.
 public static class ScorlasStatisticsArithmetic {
  public static float Height(int cumulativeHeight,float printSize,bool metric){
   if(Single.IsNaN(printSize)||Single.IsInfinity(printSize)||printSize<=0)throw new NotSupportedException("SCORLAS statistics requires a finite positive print size.");
   int padded=unchecked(cumulativeHeight+111);
   uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(printSize),0),fraction=bits&0x7fffff;int exponent=(int)((bits>>23)&255);
   if(exponent!=0)fraction|=0x800000;
   BigInteger numerator=(BigInteger)padded*fraction,denominator=4000;int power=exponent==0?-149:exponent-150;
   if(power>=0)numerator<<=power;else denominator<<=-power;
   float inches=ToSingle(numerator,denominator);
   return metric?(float)((double)inches*2.53999996185302734375f):inches;
  }
  static int Bits(BigInteger value){byte[] bytes=value.ToByteArray();int end=bytes.Length-1;while(end>0&&bytes[end]==0)end--;int high=bytes[end],bits=end*8;while(high!=0){bits++;high>>=1;}return bits;}
  static int ComparePower(BigInteger n,BigInteger d,int power){return power>=0?n.CompareTo(d<<power):(n<<-power).CompareTo(d);}
  static BigInteger Rounded(BigInteger n,BigInteger d,int power){if(power>=0)n<<=power;else d<<=-power;BigInteger remainder;BigInteger q=BigInteger.DivRem(n,d,out remainder);int cmp=(remainder<<1).CompareTo(d);return cmp>0||cmp==0&&!q.IsEven?q+1:q;}
  static float ToSingle(BigInteger n,BigInteger d){
   if(n.IsZero)return 0;uint sign=n.Sign<0?0x80000000u:0;n=BigInteger.Abs(n);int exponent=Bits(n)-Bits(d);while(ComparePower(n,d,exponent)<0)exponent--;while(ComparePower(n,d,exponent+1)>=0)exponent++;
   uint word;
   if(exponent< -126){BigInteger q=Rounded(n,d,149);word=(uint)q;}
   else if(exponent>127)word=0x7f800000;
   else{BigInteger q=Rounded(n,d,23-exponent);if(q==(BigInteger.One<<24)){q>>=1;exponent++;}word=exponent>127?0x7f800000u:((uint)(exponent+127)<<23)|((uint)q&0x7fffff);}
   return BitConverter.ToSingle(BitConverter.GetBytes(sign|word),0);
  }
 }
}
