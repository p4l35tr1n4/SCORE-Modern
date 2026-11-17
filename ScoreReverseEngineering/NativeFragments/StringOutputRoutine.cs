using System;
namespace ScoreModern.RecoveredFragments {
 public interface IStringOutputContext:ICharacterFrameMemory {
  byte ReadByte(ushort segment,ushort offset);
  CharacterRoutineResult CallCharacter(CharacterRoutineWords words,ushort targetSegment,ushort targetOffset);
 }
 public struct StringOutputResult {public CharacterRoutineResult Result;public bool ReachedStringReturn;}
 public static class StringOutputRoutine {
  // Original 16FA:06F6..0712 word/memory/control projection. Segment values are
  // explicit relocation/context inputs. No host text encoding or callee ABI is inferred.
  public static StringOutputResult Run(IStringOutputContext m,CharacterRoutineWords w,ushort currentSegment,ushort characterSegment){
   if(m==null)throw new ArgumentNullException("m");
   CharacterEntryWords e=CharacterOutputFrame.Enter(m,w.SS,w.SP,w.BP,w.DI);w.SP=e.SP;w.BP=e.BP;w.DI=e.DI;w.ES=e.ES;
   while(true){
    byte b=m.ReadByte(w.ES,w.DI);w.AX=(ushort)((w.AX&0xff00)|b);
    if(b==0)break;
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,w.ES);
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,w.DI);
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,currentSegment);
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,0x070b);
    CharacterRoutineResult c=m.CallCharacter(w,characterSegment,0x073b);w=c.Words;
    if(c.CS!=currentSegment||c.IP!=0x070b)return new StringOutputResult{Result=c,ReachedStringReturn=false};
    w.DI=unchecked((ushort)(w.DI+1));
   }
   CharacterReturnWords r=CharacterOutputFrame.Leave(m,w.SS,w.SP);w.SP=r.SP;w.BP=r.BP;w.DI=r.DI;
   return new StringOutputResult{Result=new CharacterRoutineResult{Words=w,CS=r.CS,IP=r.IP},ReachedStringReturn=true};
  }
 }
}
