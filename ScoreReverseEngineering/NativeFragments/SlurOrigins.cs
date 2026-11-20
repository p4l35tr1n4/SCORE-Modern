using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurOriginContext {public bool Scor4;public float Scale,LeftX,LeftY,RightY;}
 public sealed class SlurOriginState {
  public float LeftX,RightX,LeftY,DeltaY,Height,CurveCenter,WorkingRight,OriginalLeft,OriginalRight,StartFraction,EffectiveStartFraction,StopFraction,Length,HorizontalSpan;
  public short OffsetWord,NumberWord;
 }
 public static class SlurOrigins {
  static float F(double v){return (float)v;}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static short Word(float v){return unchecked((short)(int)Math.Truncate(v));}
  // Original geometry tables at DS:145E..1482 and DS:148E..14B2.
  // Only verified documented indices 1..10 are exposed; index zero is not a table entry.
  static readonly float[] left={0,0,9,27,36,18,9,18,9,0,0};
  static readonly float[] right={0,0,9,-6,-6,14,14,9,0,9,14};
  // Projection through relative 1149:0208. No buffer, CPU flag or raster side effects.
  public static SlurOriginState Prepare(float[] p,SlurOriginContext c){
   if(p==null||p.Length<21||c==null)throw new ArgumentException("Missing slur parameters/context.");
   bool explicitEnds=ScoreVersionControls.SlurEndpointOverride(c.Scor4,p);
   short offset=explicitEnds?(short)0:Word(p[8]);if(offset>0&&offset<10)throw new NotSupportedException("Positive Code5 bracket modes use another path.");
   if(offset < -9 || offset>19)throw new NotSupportedException("Offset outside measured slur-origin domain.");
   var r=new SlurOriginState{LeftX=c.LeftX,LeftY=c.LeftY,DeltaY=F((double)c.RightY-c.LeftY),Height=F((double)p[7]*c.Scale*7),CurveCenter=Present(p[10])?p[10]:.5f,OffsetWord=offset,NumberWord=Word(p[16]),StartFraction=p[14],EffectiveStartFraction=p[14],StopFraction=p[15]};
   if(explicitEnds){r.LeftX=F((double)c.LeftX+ScoreVersionControls.Horizontal(p[17]));r.WorkingRight=p[6];r.RightX=F((double)p[6]*150+ScoreVersionControls.Horizontal(p[18]));r.OriginalLeft=r.LeftX;r.OriginalRight=r.RightX;r.NumberWord=0;return r;}
   float a,b;
   if(offset==0&&Present(p[8])){double raw=(double)p[8]*36;a=F(raw);b=F(18-raw);}
   else {int index=offset<=0?10-offset:offset;index-=9;if(index>10)index=1;a=left[index];b=right[index];}
   r.LeftX=F((double)a*c.Scale+r.LeftX);
   r.WorkingRight=p[6];if(r.WorkingRight<p[3])r.WorkingRight=F((double)p[3]+3);
   b=F((double)c.Scale*b);r.RightX=F((double)r.WorkingRight*150+b);r.OriginalLeft=r.LeftX;r.OriginalRight=r.RightX;
   return r;
  }
  // Projection from 0208 through 030E for the normal curve path, before point generation.
  public static SlurOriginState Distance(float[] p,SlurOriginContext c){
   var r=Prepare(p,c);r.HorizontalSpan=F((double)r.RightX-r.LeftX);float span=r.HorizontalSpan;
   short half=Word(p[12]);
   if(!Present(r.StartFraction)&&!Present(r.StopFraction)&&half>0){
    if(half==1){r.RightX=F((double)span+r.RightX);r.StopFraction=.5f;}
    else {r.LeftX=F((double)r.LeftX-span);r.EffectiveStartFraction=.5f;r.LeftY=F((double)r.LeftY-r.DeltaY);}
    span=F(2.0*span);r.DeltaY=F(2.0*r.DeltaY);
   }
   r.Length=span;
   // Original compares the original parameter bit patterns, not transformed endpoint floats.
   if(BitConverter.ToInt32(BitConverter.GetBytes(p[4]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(p[5]),0))r.Length=F(Math.Sqrt((double)r.DeltaY*r.DeltaY+(double)span*span));
   return r;
  }
 }
}
