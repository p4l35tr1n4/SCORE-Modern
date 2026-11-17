using System;
namespace ScoreModern.RecoveredFragments {
 public static class ScreenHookDecisions {
  // Original 16FA:023F..0261. Data/branch projection only; does not perform
  // STI, CLD, frame saves, BIOS calls or the final far transfer.
  public static bool ShouldProbeCursor(IStringBlockMemory m,ushort ds,ushort ax){
   if(m==null)throw new ArgumentNullException("m");
   byte ah=(byte)(ax>>8),al=(byte)ax;
   if(ah==0){m.WriteByte(ds,0x4ba2,al);return false;}
   if(unchecked((sbyte)m.ReadByte(ds,0x4ba2))<=2)return false;
   if(ah!=0x0e)return false;
   return al!=8&&al!=7&&al!=0x0d;
  }
  // Original 16FA:0270..027F. Preserve the LF bypass and conditional reads.
  public static bool ShouldScroll(IStringBlockMemory m,ushort ds,byte al,ushort dx){
   if(m==null)throw new ArgumentNullException("m");
   if(al!=0x0a){
    byte bound=m.ReadByte(ds,0x4b9a);
    if(unchecked((sbyte)(byte)dx)<unchecked((sbyte)bound))return false;
   }
   byte row=m.ReadByte(ds,0x4b9b);
   return unchecked((sbyte)(byte)(dx>>8))>=unchecked((sbyte)row);
  }
  // Original 16FA:029A..02A3. Returns the block's updated DX word.
  public static ushort DecrementSavedRow(IStringBlockMemory m,ushort ds){
   if(m==null)throw new ArgumentNullException("m");
   ushort value=m.ReadWord(ds,0x4b9e);
   byte row=unchecked((byte)((value>>8)-1));
   ushort result=(ushort)((value&255)|(row<<8));
   m.WriteWord(ds,0x4b9e,result);return result;
  }
 }
}
