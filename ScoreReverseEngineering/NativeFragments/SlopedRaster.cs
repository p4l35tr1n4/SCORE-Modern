using System;
namespace ScoreModern.RecoveredFragments {
 public static class SlopedRaster {
  static ushort Rotate(ushort dx){byte b=(byte)dx;return (ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}
  // Relative 1807:09B8..0A3C. Both far exits target 0B16.
  public static VerticalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx){
   return Core(m,ds,es,ax,bx,cx,dx,null);
  }
  public static VerticalRasterWords ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,uint eflags){return Core(m,ds,es,ax,bx,cx,dx,new RasterFlagState{Value=eflags});}
  static VerticalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,RasterFlagState flags){
   bx=(ushort)(bx<<1);ax=m.ReadWord(ds,0x4b68);cx=(ushort)((cx&0xff00)|8);
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV fault is outside this projection.");
   ax=(ushort)((ax/8)|((ax%8)<<8));cx=(ushort)((cx&0xff00)|(ax>>8));dx=(ushort)((dx&0xff00)|0x80);
   for(int n=0;n<(cx&255);n++)dx=Rotate(dx);
   ax=(ushort)(ax&255);ushort bp=ax;ax=0;cx=m.ReadWord(ds,0x4b70);ushort si=m.ReadWord(ds,0x4b72),di;
   if((short)cx>=(short)si){
    cx=(ushort)(cx+1);
    do{di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);
     ax=(ushort)(ax+si);
     if((short)ax>(short)m.ReadWord(ds,0x4b74)){ax=(ushort)(ax-m.ReadWord(ds,0x4b70));bx=(ushort)(bx+m.ReadWord(ds,0x4b78));}
     dx=Rotate(dx);if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,0);if((sbyte)(byte)dx<=0){if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,bp);bp=(ushort)(bp+1);}cx=(ushort)(cx-1);
    }while(cx!=0);
   }else{
    cx=m.ReadWord(ds,0x4b72);cx=(ushort)(cx>>1);m.WriteWord(ds,0x4b74,cx);
    cx=m.ReadWord(ds,0x4b72);cx=(ushort)(cx+1);si=m.ReadWord(ds,0x4b70);
    do{di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);
     ax=(ushort)(ax+si);
     if((short)ax>(short)m.ReadWord(ds,0x4b74)){ax=(ushort)(ax-m.ReadWord(ds,0x4b72));dx=Rotate(dx);if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,0);if((sbyte)(byte)dx<=0){if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,bp);bp=(ushort)(bp+1);}}
     ushort stride=m.ReadWord(ds,0x4b78);if(flags!=null)flags.Value=RasterStatusFlags.Add16(flags.Value,bx,stride);bx=(ushort)(bx+stride);cx=(ushort)(cx-1);
    }while(cx!=0);
   }
   return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
