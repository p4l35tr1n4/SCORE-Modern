namespace ScoreModern.RecoveredFragments {
 public struct RasterDispatchWords {public ushort BX,CX,DX,Exit;public uint EFlags;}
 public static class RasterDispatch {
  static uint CompareFlags(ushort a,ushort b,uint flags){ushort r=(ushort)(a-b);uint f=0;if(a<b)f|=1;if(((a^b^r)&16)!=0)f|=16;if(r==0)f|=64;if((r&0x8000)!=0)f|=128;if(((a^b)&(a^r)&0x8000)!=0)f|=2048;int p=r&255;p^=p>>4;p^=p>>2;p^=p>>1;if((p&1)==0)f|=4;return (flags&~0x8d5u)|f;}
  // Relative 1807:0981..09B7. No raster writes or stack effects.
  public static RasterDispatchWords Execute(IHorizontalRasterMemory m,ushort ds,ushort bx,ushort cx,ushort dx,uint flags){
   flags=CompareFlags(dx,bx,flags);
   if(dx==bx)return new RasterDispatchWords{BX=bx,CX=cx,DX=dx,Exit=0x0a3d,EFlags=flags};
   m.WriteWord(ds,0x4b70,dx);dx=(ushort)(dx>>1);m.WriteWord(ds,0x4b74,dx);
   bx=m.ReadWord(ds,0x4b6a);cx=m.ReadWord(ds,0x4b68);ushort right=m.ReadWord(ds,0x4b6c);flags=CompareFlags(cx,right,flags);
   if((short)cx>=(short)right){ushort old=cx;cx=m.ReadWord(ds,0x4b6c);m.WriteWord(ds,0x4b6c,old);m.WriteWord(ds,0x4b68,cx);
    old=bx;bx=m.ReadWord(ds,0x4b6e);m.WriteWord(ds,0x4b6e,old);m.WriteWord(ds,0x4b6a,bx);m.WriteWord(ds,0x4b78,0xfffe);}
   return new RasterDispatchWords{BX=bx,CX=cx,DX=dx,Exit=0x09b8,EFlags=flags};
  }
 }
}
