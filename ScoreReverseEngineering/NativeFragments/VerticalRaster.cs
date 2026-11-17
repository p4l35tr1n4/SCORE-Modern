using System;
namespace ScoreModern.RecoveredFragments {
 public struct VerticalRasterWords { public ushort AX,BX,CX,DX,SI,DI,BP;public uint EFlags,DefinedEFlagsMask; }
 public static class VerticalRaster {
  // Relative 1807:0A96..0AC5. Video byte-OR effects remain external.
  public static VerticalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx) {
   return Core(m,ds,es,ax,bx,cx,dx,null);
  }
  public static VerticalRasterWords ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,uint eflags){return Core(m,ds,es,ax,bx,cx,dx,new RasterFlagState{Value=eflags});}
  static VerticalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,RasterFlagState flags){
   ushort si=ax;
   cx=(ushort)((cx&0xff00)|8); ax=m.ReadWord(ds,0x4b68);
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV fault is outside this projection.");
   ax=(ushort)((ax/8)|((ax%8)<<8));
   cx=(ushort)((cx&0xff00)|(ax>>8)); dx=(ushort)((dx&0xff00)|0x80);
   int shift=(cx&255)&7; byte mask=(byte)dx;
   mask=(byte)((mask>>shift)|(mask<<((8-shift)&7)));dx=(ushort)((dx&0xff00)|mask);
   cx=bx;cx=(ushort)(cx+1);cx=(ushort)(cx-m.ReadWord(ds,0x4b6a));
   ax=(ushort)(ax&255);ushort bp=ax;bx=(ushort)(si<<1);ushort di;
   do { di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);
    ushort stride=m.ReadWord(ds,0x4b78);if(flags!=null)flags.Value=RasterStatusFlags.Add16(flags.Value,bx,stride);bx=(ushort)(bx+stride);cx=(ushort)(cx-1);
   } while(cx!=0);
   return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
