using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICharacterDecisionMemory { byte ReadByte(ushort segment,ushort offset); ushort ReadWord(ushort segment,ushort offset); }
 public struct CharacterDecisionResult {
  public readonly byte AL; public readonly ushort BX; public readonly bool Branch;
  public CharacterDecisionResult(byte al,ushort bx,bool branch){AL=al;BX=bx;Branch=branch;}
 }
 public static class CharacterOutputDecisions {
  // 16FA:0748..0754. Branch=true reaches 0755; false reaches 0770.
  public static CharacterDecisionResult ProbeBackspace(ICharacterDecisionMemory m,ushort ds,ushort es,ushort di,byte dl,ushort bx){
   if(m==null)throw new ArgumentNullException("m");
   byte al=m.ReadByte(es,di);
   bool branch=al==8 && unchecked((sbyte)dl)<=unchecked((sbyte)m.ReadByte(ds,0x4b98));
   return new CharacterDecisionResult(al,bx,branch);
  }
  // 16FA:0770..077F. Branch=true reaches ordinary output 07B3; false reaches 0780.
  public static CharacterDecisionResult ProbeAlternateGate(ICharacterDecisionMemory m,ushort ds,ushort es,ushort di,ushort bx){
   if(m==null)throw new ArgumentNullException("m");
   byte al=m.ReadByte(es,di);
   if(unchecked((sbyte)al)<=27)return new CharacterDecisionResult(al,bx,true);
   bx=m.ReadWord(ds,0x8164);
   return new CharacterDecisionResult(al,bx,bx==0);
  }
 }
}
