using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutSymbolSpacingResult {
  public float Left,Right,Minimum,Scratch,ScaleScratch;public bool ResetParameter10Scratch;
 }
 // Original 2F68:08BD, 0A4F and 0CAF. Spacing values are the original
 // compiler literals, not measured renderer bounds. Caller record selection,
 // scale preparation and output-table assembly are separate operations.
 public static class LayoutSymbolSpacing {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static void Finite(params float[] values){foreach(float f in values)if(Single.IsNaN(f)||Single.IsInfinity(f))throw new NotSupportedException("Non-finite symbol-spacing input is outside this helper domain.");}
  // JUST 3.01, measured resident 0E64:089C..0963.
  internal static LayoutSymbolSpacingResult JustBar(short previous,short count,float p5){
   Finite(p5);
   long number;
   if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(p5),0),out number))
    throw new NotSupportedException("Bar style leaves the finite conversion domain.");
   short kind=unchecked((short)number);
   float left=previous==3||previous==17?0:1,right=2.5f;
   if(count>=5&&kind>0&&kind!=7){
    if(kind!=4){
     left+=1;
     if(kind==2)left+=1;
     if(kind>2)left+=4;
    }
    if(kind>3)right=6.5f;
   }
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Scratch=left};
  }
  public static LayoutSymbolSpacingResult Bar(short previous,short count,float p5,float p7,float effectiveP8,float p13,float p14) {
   Finite(p5,p7,effectiveP8,p13,p14);float right=2.5f,left=1;bool reset=false;
   long number;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(p5),0),out number))throw new NotSupportedException("Bar-style conversion is outside the recovered finite converter.");
   short kind=unchecked((short)number);
   if(count>=5&&kind>=1&&kind<=6) {
    reset=true;
    switch(kind) {
     case 1:left=(float)((double)effectiveP8*2.2f+p13);break;
     case 2:left=(float)((double)effectiveP8*3.3f+p13);break;
     case 3:left=(float)((double)effectiveP8*5.4f+p14+p13);break;
     case 4:left=(float)((double)effectiveP8*2);right=(float)((double)effectiveP8*6+p13+p14);break;
     case 5:left=(float)((double)effectiveP8*5.4f+p13+p14);right=(float)((double)effectiveP8*6+p13+p14);break;
     case 6:left=(float)((double)effectiveP8*4.3f+p14);right=(float)((double)effectiveP8*7+p13+p14);break;
    }
   }
   if(previous==3||previous==17)left=(float)((double)left-1);
   float scratch=left;
   if(p7!=0){scratch=p7;left=(float)((double)left-p7);right=(float)((double)p7+right);}
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Scratch=scratch,ResetParameter10Scratch=reset};
  }
  public static LayoutSymbolSpacingResult Key(short previous,float p7,float p5,bool standaloneJust=false) {
   Finite(p7,p5);float left=previous==1?2:0;
   float scratch=(float)Math.Abs((double)p5%100);float scale=Bits(p7)==0?1:p7;
   float right=previous==3&&!standaloneJust?5.5f:4.5f;
   right=(float)(((double)scratch-1)*scale*2.8f+right);
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Scratch=scratch,ScaleScratch=scale};
  }
  public static LayoutSymbolSpacingResult Meter(short previous,float minimum,float[] parameters,short count,bool standaloneJust=false) {
   if(parameters==null||count<1||count>parameters.Length)throw new NotSupportedException("Original meter-spacing extent leaves its record.");
   Finite(minimum);
   // This helper reads the raw backing buffer, unlike the preceding padded
   // record getter. The caller must supply actual adjacent words for any read
   // beyond the declared count; inventing zero padding would change SCORE.
   Func<int,float> p=delegate(int n){if(n<1||n>parameters.Length)throw new NotSupportedException("Original meter-spacing backing words are unavailable.");float value=parameters[n-1];Finite(value);return value;};
   float left=0,right=standaloneJust?6.5f:7.5f;
   if(previous==14){minimum=(float)((double)minimum-1.5f);left=.5f;}
   if(previous==1)left=2;
   if(previous==17&&!standaloneJust)left=-1.5f;
   float scratch=p(5),scaleScratch=0;
   if((p(6)>=10||scratch>=10)&&scratch<=90)right=(float)((double)right+2.5f);
   int last=count;while(last>0&&p(last)==0)last--;
   if(last==0)throw new NotSupportedException("Original meter-spacing scan leaves an all-zero record.");
   if(last>=8) {
    scratch=0;
    if(last<12||p(12)==0){scratch=4.5f;if(last>=10&&p(10)!=0)scratch=(float)((double)p(10)+scratch);}
    if(last>=11&&p(11)!=0)scratch=(float)((double)p(11)+scratch);
    scaleScratch=p(8)>10||p(9)>10?6:3.5f;
    right=(float)((double)right+scratch+scaleScratch);
   }
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Minimum=minimum,Scratch=scratch,ScaleScratch=scaleScratch};
  }
 }
}
