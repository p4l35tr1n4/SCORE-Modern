using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputBeamFitResult {public float Start,End,UnquantizedStart,UnquantizedEnd;public int Direction;}
 public static class InputBeamFit {
  // Section54 numerical path for a single-staff, non-chord group. Inputs are
  // the representative pitches and x positions prepared by section59.
  public static InputBeamFitResult Fit(float[] pitch,float[] x,int direction,int flags,float slopeLimit){
   if(pitch==null||x==null||pitch.Length!=x.Length||pitch.Length<2)throw new ArgumentException("Beam group");
   if(direction!=0&&direction!=10&&direction!=20)throw new ArgumentException("Direction");
   int n=pitch.Length;float a=pitch[0],b=pitch[n-1],high=a,low=a,span=0;
   foreach(float p in pitch){if(float.IsNaN(p)||float.IsInfinity(p))throw new ArgumentException("Finite pitch required");if(p>high)high=p;if(p<low)low=p;}
   if(direction==0){float above=F((double)high-7),below=F(7.0-low);direction=20;if(above==below){int hi=0,lo=0;foreach(float p in pitch){if(p>7)hi++;if(p<7)lo++;}if(hi<lo)direction=10;}else if(above<below)direction=10;}
   float extreme=direction==10?high:low;bool flatten=false;
   if(a==b)flatten=Bits(extreme)!=Bits(a);
   else{int count=0;foreach(float p in pitch)if(p==extreme)count++;flatten=count>=2;}
   if(flatten)a=b=F((double)extreme+(flags==1?(direction==10?-1:1):0));
   else if(a!=b){
    span=F((double)x[n-1]-x[0]);if(span==0)throw new ArgumentException("Zero beam span");float slope=F(((double)b-a)/span),gap=direction==20?99:-99,targetPitch=0;int at=0;
    for(int i=1;i<n-1;i++){float line=F(((double)x[i]-x[0])*slope+a),distance=F((double)pitch[i]-line);if(direction==20?distance<0&&distance<gap:distance>0&&distance>gap){gap=distance;targetPitch=pitch[i];at=i;}}
    float allowance=flags>1?0:1;
    if(direction==20?gap!=99&&gap< -allowance:gap!= -99&&gap>allowance){
     float target=F((double)targetPitch+(direction==20?allowance:-allowance));
     if(Bits(extreme)!=Bits(a)&&Bits(extreme)!=Bits(b))a=b=F((double)extreme+(flags==1?(direction==10?-1:1):0));
     else{
      float other=direction==20?a:b,anchor=direction==20?b:a;
      // Section54:0751-07A6 compares direction-ordered endpoints; JA selects the far-end output.
      if(anchor>other)b=Extrapolate(a,target,x[0],x[at],x[n-1]);else a=Extrapolate(b,target,x[n-1],x[at],x[0]);
     }
    }
   }
   if(direction==20){if(a>14)a=14;if(b>14)b=14;}else{if(a<0)a=0;if(b<0)b=0;}
   bool shiftEnd=direction==20?a<b:b<a;float difference=Math.Abs(F((double)a-b));
   if(difference>1.5f&&slopeLimit<difference/span){
    if(difference>=3){difference=F((double)difference-1);if(flags<2){float change=direction==20?1:-1;if(shiftEnd)a=F((double)a+change);else b=F((double)b+change);}}
    float oldA=a,oldB=b;
    for(float slope=slopeLimit;slope<difference/span;slope=F((double)slope+0.009999999776482582)){float shift=F((double)difference-(double)slope*span);if(direction==20)shift=-shift;if(shiftEnd)b=F((double)oldB+shift);else a=F((double)oldA+shift);if(Math.Abs(F((double)b-a))>=1)break;}
   }
   int qa=(short)Math.Truncate((double)a*2),qb=(short)Math.Truncate((double)b*2);float adjust=0;
   if(qa==qb&&(qa/2)%2!=0&&flags%2==0){if(direction==20&&qa>13)adjust=.5f;else if(direction==10&&qa<15)adjust=-.5f;}
   return new InputBeamFitResult{Direction=direction,UnquantizedStart=a,UnquantizedEnd=b,Start=F(qa/2.0+adjust),End=F(qb/2.0+adjust)};
  }
  static float Extrapolate(float anchor,float target,float anchorX,float targetX,float resultX){float slope=F(((double)target-anchor)/((double)targetX-anchorX));return F(((double)resultX-anchorX)*slope+anchor);}
  static float F(double x){return (float)x;}static int Bits(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0);}
 }
}
