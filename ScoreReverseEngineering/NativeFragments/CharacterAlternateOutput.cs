using System;
namespace ScoreModern.RecoveredFragments {
 public struct CharacterBiosWords {public ushort AX,BX,CX,DX,SS,SP;}
 public interface ICharacterBiosContext {
  void WriteWord(ushort segment,ushort offset,ushort value);
  ushort ReadWord(ushort segment,ushort offset);
  void Interrupt10(ref CharacterBiosWords words);
 }
 public static class CharacterAlternateOutput {
  // Original 16FA:0780..07B1; ends at 07BA, before the epilogue.
  // Explicit word/memory projection. BIOS calls are external effects.
  public static CharacterBiosWords Run(ICharacterBiosContext context,CharacterBiosWords w){
   if(context==null)throw new ArgumentNullException("context");
   w.SP=unchecked((ushort)(w.SP-2));context.WriteWord(w.SS,w.SP,w.AX);
   w.AX=0x0a08;w.CX=1;w.BX=7;context.Interrupt10(ref w);
   w.AX=0x0907;w.CX=1;w.BX=0x0087;context.Interrupt10(ref w);
   w.AX=context.ReadWord(w.SS,w.SP);w.SP=unchecked((ushort)(w.SP+2));
   w.AX=(ushort)((w.AX&255)|0x0900);w.CX=1;w.BX=0x0087;context.Interrupt10(ref w);
   w.AX=(ushort)((w.AX&255)|0x0300);w.BX=(ushort)(w.BX&255);context.Interrupt10(ref w);
   w.DX=(ushort)((w.DX&0xff00)|unchecked((byte)((w.DX&255)+1)));
   w.AX=(ushort)((w.AX&255)|0x0200);w.BX=(ushort)(w.BX&255);context.Interrupt10(ref w);
   return w;
  }
 }
}
