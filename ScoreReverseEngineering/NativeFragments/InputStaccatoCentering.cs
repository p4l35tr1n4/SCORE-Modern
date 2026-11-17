using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputStaccatoCentering {
  // Runtime316D:00E4..0220 at load0824 (relative2949). The numeric
  // projection follows the captured DOSBox-X x87 path; it is not an x87
  // status/exception emulator. Its multiplication intermediates are binary64.
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static double D(double v){return BitConverter.ToDouble(BitConverter.GetBytes(v),0);}
  static short W(float v){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(v),0),out n))throw new NotSupportedException("Centering control exceeds the verified finite conversion domain.");return unchecked((short)n);}
  static void Finite(float v){if(Single.IsNaN(v)||Single.IsInfinity(v))throw new NotSupportedException("Nonfinite temporary mark centering requires original fault verification.");}
  public static bool Apply(float[] p,float[] staffScales){
   if(p==null||p.Length<19)throw new ArgumentException("Missing note parameters.");
   if(p[0]!=1||p[12]!=99)return false;
   float result=0;
   if(BitConverter.ToInt32(BitConverter.GetBytes(p[10]),0)<0&&p[4]>=10&&p[5]!=2&&W(p[10])==-7){
    Finite(p[3]);Finite(p[4]);Finite(p[5]);Finite(p[14]);
    short staff=W(p[1]);if(staff<1||staff>32||staffScales==null||staffScales.Length<=staff)throw new NotSupportedException("Temporary mark centering needs a verified staff1..32 scale.");
    float size=Present(p[14])?p[14]:1;
    if(Math.Abs(p[3])>=100)size=.6000000238418579f;
    Finite(staffScales[staff]);
    double product=D((double)staffScales[staff]*size);
    product=D(product*1.350000023841858f);
    result=(float)D(product/25.0);
    Finite(result);
    if(p[4]>=20)result=BitConverter.ToSingle(BitConverter.GetBytes(BitConverter.ToInt32(BitConverter.GetBytes(result),0)^Int32.MinValue),0);
   }
   p[12]=result;return true;
  }
 }
}
