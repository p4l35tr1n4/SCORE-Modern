using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurThicknessResult {public short[] X,Y;public float Thickness,EffectiveControl,RampStep,FinalRamp;public int Target,FinalIncrement;}
 public static class SlurThickness {
  static float F(double v){return (float)v;}
  static int T(double v){if(Double.IsNaN(v)||v<Int32.MinValue||v>Int32.MaxValue)throw new NotSupportedException("Conversion outside verified finite range.");return (int)Math.Truncate(v);}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  // Relative 1149:0993..0AB3: negative P12 reflects the second half.
  public static void Reflect(float[] p,SlurSampleResult sample,short[] y,bool printed){
   if(unchecked((short)T(p[12]))>=0)return;
   int n=sample.Count,peak=T((double)sample.Origin.LeftY+sample.Height);if(peak>32600)peak=32600;int partner=2*n-(n/2)-1;
   for(int k=n/2-1;k<n;k++){
    short twice=unchecked((short)((peak-y[k])*2));int v=y[k]+twice;if(v>32600)v=32600;y[k]=unchecked((short)v);
    if(printed&&k!=n-1){v=unchecked((short)(y[partner]+twice));if(v>32600)v=32600;y[partner]=unchecked((short)v);partner--;}
   }
  }
  public static SlurThicknessResult Build(float[] p,SlurOriginContext context,SlurSampleResult sample,float configuredControl){
   int n=sample.Count;var r=new SlurThicknessResult{X=new short[2*n-1],Y=new short[2*n-1]};Array.Copy(sample.X,r.X,n);Array.Copy(sample.Y,r.Y,n);
   float amount=F((double)context.Scale*3.5);float control=Present(p[13])?p[13]:configuredControl;if(control<=0)control=1;
   short length=unchecked((short)T(sample.Origin.Length));if(length<3000){double factor=length<=600?(double).6f:1.0-(3000-length)*(double).4f/2400.0;control=F(factor*control);}
   r.EffectiveControl=control;amount=F((double)control*amount);r.Thickness=amount;r.Target=T((double)amount+.5);float rampStep=F((double)amount/((double)sample.HalfSteps*.85f));r.RampStep=rampStep;
   int sign=unchecked((short)T(p[7]))<0?-1:1;float ramp=0;int current=0,source=n,destination=n,mirror=2*n-1,midpoint=2+(int)sample.HalfSteps;short value=0;
   for(int index=2;index<=n;index++){
    destination++;source--;r.X[destination-1]=r.X[source-1];
    if(index<=midpoint){if(current<r.Target){ramp=F((double)rampStep+ramp);current=T(ramp);}short increment=unchecked((short)(current*sign));value=unchecked((short)(r.Y[source-1]+increment));r.Y[destination-1]=value;mirror--;r.Y[mirror-1]=value;}
   }
   r.Y[destination-1]=r.Y[0];r.FinalRamp=ramp;r.FinalIncrement=current;return r;
  }
 }
}
