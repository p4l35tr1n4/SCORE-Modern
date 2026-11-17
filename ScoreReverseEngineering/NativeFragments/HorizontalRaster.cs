using System;
namespace ScoreModern.RecoveredFragments {
 public interface IHorizontalRasterMemory {
  ushort ReadWord(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
  // Original byte OR instruction effect. A VGA bus/plane model is external.
  void OrByte(ushort segment,ushort offset,byte mask);
 }
 public struct HorizontalRasterWords {public ushort AX,BX,CX,DX,SI,DI;public uint EFlags,DefinedEFlagsMask;}
 public static class HorizontalRaster {
  static ushort DivideByEight(ushort ax){
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV overflow path is outside this projection.");
   return (ushort)((ax/8)|((ax%8)<<8));
  }
  // 1807:0AC6..0B15, after clipping/preparation and before register POPs.
  // Successful original byte-DIV paths only; no full flags/exception/VGA ABI.
  public static HorizontalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx){
   return Core(m,ds,es,ax,cx,dx,null);
  }
  public static HorizontalRasterWords ExecuteWithFlags(IByteOrResultMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,uint eflags){return Core(m,ds,es,ax,cx,dx,new RasterFlagState{Value=eflags});}
  static HorizontalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,RasterFlagState flags){
   if(m==null)throw new ArgumentNullException("m");
   ushort bx=unchecked((ushort)(ax<<1));ushort di=m.ReadWord(ds,unchecked((ushort)(bx+0x456a)));ushort si=di;
   ax=m.ReadWord(ds,0x4b68);
   if(unchecked((short)ax)>=unchecked((short)m.ReadWord(ds,0x4b6c))){ushort old=ax;ax=m.ReadWord(ds,0x4b6c);m.WriteWord(ds,0x4b6c,old);}
   cx=(ushort)((cx&0xff00)|8);ax=DivideByEight(ax);cx=(ushort)((cx&255)|(ax&0xff00));ax=(ushort)(ax&255);di=unchecked((ushort)(di+ax));
   ax=m.ReadWord(ds,0x4b6c);ax=DivideByEight(ax);cx=(ushort)((cx&0xff00)|7);cx=(ushort)((cx&0xff00)|unchecked((byte)((cx&255)-(ax>>8))));ax=(ushort)(ax&255);si=unchecked((ushort)(si+ax));
   dx=(ushort)((dx&0xff00)|unchecked((byte)(255<<(cx&255))));cx=(ushort)((cx&0xff00)|(cx>>8));bx=(ushort)((bx&0xff00)|unchecked((byte)(255>>(cx&255))));
   if(si>di){m.OrByte(es,di,(byte)bx);ax=(ushort)((ax&0xff00)|255);while(true){di=unchecked((ushort)(di+1));if(di>=si)break;m.OrByte(es,di,(byte)ax);}}
   else dx=(ushort)((dx&0xff00)|((dx&255)&(bx&255)));
   if(flags==null)m.OrByte(es,di,(byte)dx);else flags.Value=RasterStatusFlags.Logical8(flags.Value,((IByteOrResultMemory)m).OrByteAndGetResult(es,di,(byte)dx));
   return new HorizontalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
