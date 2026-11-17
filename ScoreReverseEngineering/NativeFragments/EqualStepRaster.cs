using System;
namespace ScoreModern.RecoveredFragments {
 public static class EqualStepRaster {
  // Relative 1807:0A3D..0A95. SI is preserved; video effects external.
  public static VerticalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si){
   return Core(m,ds,es,ax,cx,dx,si,null);
  }
  public static VerticalRasterWords ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si,uint eflags){return Core(m,ds,es,ax,cx,dx,si,new RasterFlagState{Value=eflags});}
  static VerticalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si,RasterFlagState flags){
   ushort bx=(ushort)(ax<<1);cx=(ushort)((cx&0xff00)|8);ax=m.ReadWord(ds,0x4b68);
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV fault is outside this projection.");
   ax=(ushort)((ax/8)|((ax%8)<<8));cx=(ushort)((cx&0xff00)|(ax>>8));dx=(ushort)((dx&0xff00)|0x80);
   for(int n=0;n<(cx&255);n++){byte b=(byte)dx;dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}
   cx=m.ReadWord(ds,0x4b72);cx=(ushort)(cx+1);ax=(ushort)(ax&255);ushort bp=ax;
   ax=m.ReadWord(ds,0x4b6c);bool right=(short)ax>(short)m.ReadWord(ds,0x4b68);ushort di;
   do{di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);bx=(ushort)(bx+m.ReadWord(ds,0x4b78));
    byte b=(byte)dx;
    if(right){dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,0x80);if((byte)dx==0x80){if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,bp);bp=(ushort)(bp+1);}}
    else{dx=(ushort)((dx&0xff00)|((b<<1)&255)|(b>>7));if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,1);if((byte)dx==1){if(flags!=null)flags.Value=RasterStatusFlags.Dec16(flags.Value,bp);bp=(ushort)(bp-1);}}
    cx=(ushort)(cx-1);
   }while(cx!=0);
   return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
