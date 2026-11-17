using System;
namespace ScoreModern.RecoveredFragments {
 public struct CharacterRoutineWords {public ushort AX,BX,CX,DX,SI,DI,BP,DS,ES,SS,SP;}
 public struct CharacterRoutineResult {public CharacterRoutineWords Words;public ushort IP,CS;}
 public interface ICharacterRoutineContext:ICharacterFrameMemory,ICharacterDecisionMemory {
  new ushort ReadWord(ushort segment,ushort offset);
  // Return offsets identify original call boundaries; BIOS effects remain external.
  void Interrupt10(ref CharacterRoutineWords words,ushort returnOffset);
 }
 public static class CharacterOutputRoutine {
  sealed class AlternateBridge:ICharacterBiosContext {
   public ICharacterRoutineContext Context;public CharacterRoutineWords Words;int index;
   public void WriteWord(ushort s,ushort o,ushort v){Context.WriteWord(s,o,v);}
   public ushort ReadWord(ushort s,ushort o){return Context.ReadWord(s,o);}
   public void Interrupt10(ref CharacterBiosWords w){Words.AX=w.AX;Words.BX=w.BX;Words.CX=w.CX;Words.DX=w.DX;Words.SS=w.SS;Words.SP=w.SP;Context.Interrupt10(ref Words,new ushort[]{0x78c,0x797,0x7a2,0x7a8,0x7b0}[index++]);w.AX=Words.AX;w.BX=Words.BX;w.CX=Words.CX;w.DX=Words.DX;w.SS=Words.SS;w.SP=Words.SP;}
  }
  // Word/memory/control projection of 16FA:073B..07BE. Flags, interrupt frames,
  // hardware exceptions, and BIOS implementation remain external to this interface.
  public static CharacterRoutineResult Run(ICharacterRoutineContext m,CharacterRoutineWords w){
   if(m==null)throw new ArgumentNullException("m");
   CharacterEntryWords e=CharacterOutputFrame.Enter(m,w.SS,w.SP,w.BP,w.DI);w.SP=e.SP;w.BP=e.BP;w.DI=e.DI;w.ES=e.ES;
   w.AX=(ushort)((w.AX&255)|0x0300);w.BX=(ushort)(w.BX&255);m.Interrupt10(ref w,0x748);
   CharacterDecisionResult d=CharacterOutputDecisions.ProbeBackspace(m,w.DS,w.ES,w.DI,(byte)w.DX,w.BX);w.AX=(ushort)((w.AX&0xff00)|d.AL);w.BX=d.BX;
   if(d.Branch){
    w.DX=(ushort)((w.DX&255)|((byte)((w.DX>>8)-1)<<8));w.DX=(ushort)((w.DX&0xff00)|m.ReadByte(w.DS,0x4b9a));
    w.AX=(ushort)((w.AX&255)|0x0200);w.BX=(ushort)(w.BX&255);m.Interrupt10(ref w,0x761);
    w.AX=0x0a20;w.BX=0;w.CX=1;m.Interrupt10(ref w,0x76d);
   }else{
    d=CharacterOutputDecisions.ProbeAlternateGate(m,w.DS,w.ES,w.DI,w.BX);w.AX=(ushort)((w.AX&0xff00)|d.AL);w.BX=d.BX;
    if(d.Branch){w.AX=(ushort)((w.AX&255)|0x0e00);w.BX=7;m.Interrupt10(ref w,0x7ba);}
    else{
     AlternateBridge bridge=new AlternateBridge{Context=m,Words=w};CharacterBiosWords b=CharacterAlternateOutput.Run(bridge,new CharacterBiosWords{AX=w.AX,BX=w.BX,CX=w.CX,DX=w.DX,SS=w.SS,SP=w.SP});w=bridge.Words;w.AX=b.AX;w.BX=b.BX;w.CX=b.CX;w.DX=b.DX;w.SS=b.SS;w.SP=b.SP;
    }
   }
   CharacterReturnWords r=CharacterOutputFrame.Leave(m,w.SS,w.SP);w.SP=r.SP;w.BP=r.BP;w.DI=r.DI;return new CharacterRoutineResult{Words=w,IP=r.IP,CS=r.CS};
  }
 }
}
