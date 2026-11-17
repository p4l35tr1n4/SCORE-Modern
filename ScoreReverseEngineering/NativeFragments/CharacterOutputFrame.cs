using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICharacterFrameMemory {ushort ReadWord(ushort segment,ushort offset);void WriteWord(ushort segment,ushort offset,ushort value);}
 public struct CharacterEntryWords {public ushort SP,BP,DI,ES;}
 public struct CharacterReturnWords {public ushort SP,BP,DI,IP,CS;}
 public static class CharacterOutputFrame {
  // 16FA:073B..0741. Explicit data/word projection, before the BIOS query.
  public static CharacterEntryWords Enter(ICharacterFrameMemory m,ushort ss,ushort sp,ushort bp,ushort di){
   if(m==null)throw new ArgumentNullException("m");
   sp=unchecked((ushort)(sp-2));m.WriteWord(ss,sp,bp);bp=sp;
   sp=unchecked((ushort)(sp-2));m.WriteWord(ss,sp,di);
   ushort offset=m.ReadWord(ss,unchecked((ushort)(bp+6)));
   ushort segment=m.ReadWord(ss,unchecked((ushort)(bp+8)));
   return new CharacterEntryWords{SP=sp,BP=bp,DI=offset,ES=segment};
  }
  // 16FA:07BA..07BE. Four ordered word reads, then RETF's argument purge.
  public static CharacterReturnWords Leave(ICharacterFrameMemory m,ushort ss,ushort sp){
   if(m==null)throw new ArgumentNullException("m");
   ushort di=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort bp=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort ip=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort cs=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+6));
   return new CharacterReturnWords{SP=sp,BP=bp,DI=di,IP=ip,CS=cs};
  }
 }
}
