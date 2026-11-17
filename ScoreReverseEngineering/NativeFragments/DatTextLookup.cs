using System;
namespace ScoreModern.RecoveredFragments {
 public interface IDatTextLookupMemory {ushort ReadWord(ushort segment,ushort offset);}
 public struct DatTextLookupWords {public ushort BX,AX,DX;}
 public static class DatTextLookup {
  // Original 020F:04C3..04D4 word/read projection. The trailing jump has no
  // effect on these words. No bounds check, character mapping or float policy added.
  public static DatTextLookupWords Read(IDatTextLookupMemory m,ushort ds){
   if(m==null)throw new ArgumentNullException("m");
   ushort bx=m.ReadWord(ds,0x8678);bx=unchecked((ushort)(bx<<1));bx=unchecked((ushort)(bx<<1));
   ushort ax=m.ReadWord(ds,unchecked((ushort)(bx+0x8a3c)));
   ushort dx=m.ReadWord(ds,unchecked((ushort)(bx+0x8a3e)));
   return new DatTextLookupWords{BX=bx,AX=ax,DX=dx};
  }
 }
}
