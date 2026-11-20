using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurSampleResult {
  public short[] X,Y;public int Count;public float Step,FinalX,FinalIncrement,Exponent,HalfSteps,Height;public SlurOriginState Origin;
 }
 public static class SlurSampling {
  static float F(double v){return (float)v;}
  static short W(float v){return unchecked((short)(int)Math.Truncate(v));}
  static int T(double v){if(Double.IsNaN(v)||v<Int32.MinValue||v>Int32.MaxValue)throw new NotSupportedException("Conversion outside verified finite range.");return (int)Math.Truncate(v);}
  static short Y(double v){int n=T(v);if(n>32600)n=32600;return unchecked((short)n);}
  // Relative 1149:0227..07A8, normal point-generation path.
  // Math.Pow is a candidate numerical projection checked against original integer words,
  // not a claim of x87 power-helper or exception/flag equivalence.
  public static SlurSampleResult Build(float[] p,SlurOriginContext context,bool printed){
   if(W(p[16])!=0||W(p[9])==99)throw new NotSupportedException("Numbered/straight Code5 paths are separate.");
   var o=SlurOrigins.Distance(p,context);bool trimmed=BitConverter.ToInt32(BitConverter.GetBytes(p[14]),0)!=0||BitConverter.ToInt32(BitConverter.GetBytes(p[15]),0)!=0;return BuildState(p,context,o,printed,trimmed);
  }
  public static SlurSampleResult BuildState(float[] p,SlurOriginContext context,SlurOriginState o,bool printed,bool trimmed){
   // Original 0252..0259 clears the positive working half control when
   // explicit trims are present; sample counts use that cache, not source P12.
   int control=W(p[11]),half=W(p[12]);if(trimmed&&half>0)half=0;int divisor=printed?200:500;
   if(trimmed)divisor=200;
   if(control!=0)divisor=500;
   int count=(unchecked((short)T(o.Length))/divisor)*2;count=Math.Max(count,8);
   if(control!=0||printed)count=unchecked((short)(count+2*Math.Abs((int)W(p[7]))));
   if(control!=0){
    if(control< -3)control/=2;control=Math.Abs(control);
    if(control>=2){count=control*2;if(half>0)count*=2;}
    else {count-=2;if(p[11]<=0&&control==1&&count>8){count=count/2-2;if(count%2!=0)count--;if(count<5)count=6;}}
   }
   if(count<4||count>20000)throw new NotSupportedException("Sample count outside verified safe projection range.");
   float step=F((double)o.Length/(count-1));double twiceCenter=(double)o.CurveCenter+o.CurveCenter;float center2=F(twiceCenter);float increment=F(twiceCenter*step),x=o.LeftX;
   var r=new SlurSampleResult{Origin=o,Count=count,Step=step,X=new short[count],Y=new short[count],Height=o.Height};
   int switchIndex=count/2-2;
   for(int i=0;i<count;i++){int n=T(x);if(n>32760)n=32760;r.X[i]=unchecked((short)n);x=F((double)increment+x);if(i==switchIndex)increment=F((2.0-center2)*step);}
   r.FinalX=x;r.FinalIncrement=increment;
   double rawExponent=(double)p[9]+2.5;float exponent=F(rawExponent);if(rawExponent<(double).1f)exponent=.1f;r.Exponent=exponent;
   int halfSteps=count/2-1;float steps=halfSteps;r.HalfSteps=steps;int peak=T((double)o.Height+o.LeftY);
   for(int k=halfSteps;k>=0;k--){double t=(double)(float)k/steps;double power=Math.Pow(t,exponent);short value=Y(peak-power*o.Height);int j=halfSteps-k;r.Y[j]=value;r.Y[count-1-j]=value;}
   return r;
  }
 }
}
