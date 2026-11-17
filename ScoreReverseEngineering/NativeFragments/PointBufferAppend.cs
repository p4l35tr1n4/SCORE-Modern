using System;
namespace ScoreModern.RecoveredFragments {
 public interface IPointBufferMemory {ushort ReadWord(ushort segment,ushort offset);void WriteWord(ushort segment,ushort offset,ushort value);}
 public struct PointBufferWords {public ushort AX,BX,SI,ES;public uint EFlags,DefinedEFlagsMask;}
 public static class PointBufferAppend {
  // Original 0C25:07EB..0847 word/memory projection, after conversion/clamps
  // and before the epilogue. Marker values remain numeric, not named operations.
  public static PointBufferWords Execute(IPointBufferMemory m,ushort ds,ushort ss,ushort bp){
   return Core(m,ds,ss,bp,null);
  }
  public static PointBufferWords ExecuteWithFlags(IPointBufferMemory m,ushort ds,ushort ss,ushort bp,uint flags){return Core(m,ds,ss,bp,new RasterFlagState{Value=flags});}
  static PointBufferWords Core(IPointBufferMemory m,ushort ds,ushort ss,ushort bp,RasterFlagState flags){
   if(m==null)throw new ArgumentNullException("m");
   ushort es=m.ReadWord(ds,0x69da);ushort ax=m.ReadWord(es,0);m.WriteWord(ds,0xc58e,ax);
   ushort bx=m.ReadWord(ss,unchecked((ushort)(bp+6)));es=m.ReadWord(ss,unchecked((ushort)(bp+8)));ushort si=m.ReadWord(es,bx);
   if(unchecked((short)si)>0){
    bx=unchecked((ushort)(ax<<1));es=m.ReadWord(ds,0x69da);bx=unchecked((ushort)(bx+2));ax=unchecked((ushort)(si+0x8044));m.WriteWord(es,unchecked((ushort)(bx-2)),ax);
    m.WriteWord(ds,0xc58e,unchecked((ushort)(m.ReadWord(ds,0xc58e)+1)));
   }
   si=m.ReadWord(ds,0xc58e);if(flags!=null)flags.Value=(flags.Value&~1u)|((si&0x8000)!=0?1u:0u);si=unchecked((ushort)(si<<1));es=m.ReadWord(ds,0x69da);bx=unchecked((ushort)(si+2));ax=m.ReadWord(ds,0xc590);m.WriteWord(es,unchecked((ushort)(bx-2)),ax);
   es=m.ReadWord(ds,0x69dc);bx=si;ax=m.ReadWord(ds,0xc594);m.WriteWord(es,unchecked((ushort)(bx-2)),ax);
   es=m.ReadWord(ds,0x69da);ax=m.ReadWord(ds,0xc58e);if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,ax);ax=unchecked((ushort)(ax+1));m.WriteWord(es,0,ax);
   return new PointBufferWords{AX=ax,BX=bx,SI=si,ES=es,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
