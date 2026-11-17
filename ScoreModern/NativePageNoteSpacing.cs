using System;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal sealed class NativePageNoteSpacingResult {
  internal float Left,Right,Multiplier;
 }
 internal static class NativePageNoteSpacing {
  // PAGE 0D29:03CF..0659. This is PAGE's inline spacing calculation,
  // independent of the score renderer and of the larger J note caller.
  internal static NativePageNoteSpacingResult Apply(float[] p,int count,float rawP10) {
   if(p==null||p.Length<9)throw new ArgumentException("PAGE working parameters P1..P9 are required.");
   foreach(float v in p)if(Single.IsNaN(v)||Single.IsInfinity(v))throw new NotSupportedException("Non-finite PAGE note-spacing input.");
   if(Single.IsNaN(rawP10)||Single.IsInfinity(rawP10))throw new NotSupportedException("Non-finite PAGE raw P10.");
   float right=p[5]==2||p[5]==3?5:4.5f,left=0,scale=Math.Abs((double)p[3])>=100?.6f:1;
   float accidental=p[4]%10;
   if(BitConverter.ToInt32(BitConverter.GetBytes(accidental),0)!=0){
    float baseWidth=2.5f;if(accidental>=4)baseWidth=4;
    left=(float)((double)(accidental%1)*10+baseWidth);
   }
   float p9=p[8];
   if(BitConverter.ToInt32(BitConverter.GetBytes(p9),0)!=0){
    float extra=0;
    if(p9>=10){long converted;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(p9),0),out converted))throw new NotSupportedException("PAGE note-spacing integer conversion leaves the verified finite domain.");short integer=unchecked((short)converted);int tens=(integer%100)/10;extra=(float)((double)(p9%1)*10+(double)tens*1.5f);}
    else if(p[4]<20)extra=1.5f;
    right=(float)((double)right+extra);
   }
   if(count>=10){float saved=right;
    if(rawP10==20)left=(float)((double)saved+left);
    if(rawP10==10)right=(float)((double)saved+right);
    if(rawP10<10){int bits=BitConverter.ToInt32(BitConverter.GetBytes(rawP10),0);
     if(bits>0)right=(float)((double)rawP10+right);
     if(bits<0)left=(float)((double)rawP10+left);
    }
   }
   return new NativePageNoteSpacingResult{Left=(float)((double)scale*left),Right=(float)((double)scale*right),Multiplier=scale};
  }
 }
}
