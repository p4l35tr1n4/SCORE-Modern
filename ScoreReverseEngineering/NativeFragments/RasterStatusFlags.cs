namespace ScoreModern.RecoveredFragments {
 // Returns the original CPU-visible OR result while performing its video effect.
 // This is not necessarily the resulting byte in display plane 2.
 public interface IByteOrResultMemory:IHorizontalRasterMemory {
  byte OrByteAndGetResult(ushort segment,ushort offset,byte mask);
 }
 public static class RasterStatusFlags {
  const uint Status=0x8d5;
  static uint Result(uint flags,uint bits,ushort result,int width){
   uint sign=width==8?0x80u:0x8000u,mask=width==8?255u:65535u;
   result=(ushort)(result&mask);if(result==0)bits|=64;if((result&sign)!=0)bits|=128;
   int p=result&255;p^=p>>4;p^=p>>2;p^=p>>1;if((p&1)==0)bits|=4;
   return (flags&~Status)|bits;
  }
  static uint Sub(uint flags,ushort a,ushort b,int width){
   uint mask=width==8?255u:65535u,sign=width==8?0x80u:0x8000u;a=(ushort)(a&mask);b=(ushort)(b&mask);ushort r=(ushort)((a-b)&mask);uint bits=0;
   if(a<b)bits|=1;if(((a^b^r)&16)!=0)bits|=16;if(((a^b)&(a^r)&sign)!=0)bits|=2048;return Result(flags,bits,r,width);
  }
  public static uint Sub8(uint flags,byte a,byte b){return Sub(flags,a,b,8);}
  public static uint Sub16(uint flags,ushort a,ushort b){return Sub(flags,a,b,16);}
  public static uint Add16(uint flags,ushort a,ushort b){ushort r=(ushort)(a+b);uint bits=0;if((uint)a+b>65535)bits|=1;if(((a^b^r)&16)!=0)bits|=16;if(((~(a^b))&(a^r)&0x8000)!=0)bits|=2048;return Result(flags,bits,r,16);}
  public static uint Inc16(uint flags,ushort a){return (Add16(flags,a,1)&~1u)|(flags&1u);}
  public static uint Dec16(uint flags,ushort a){return (Sub16(flags,a,1)&~1u)|(flags&1u);}
  // DOSBox-X normal-core logical-byte policy clears AF (architecturally undefined).
  public static uint Logical8(uint flags,byte result){return Result(flags,0,result,8);}
 }
 internal sealed class RasterFlagState {public uint Value;}
}
