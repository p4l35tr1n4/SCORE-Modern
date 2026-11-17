using System;
namespace ScoreModern.RecoveredFragments {
 public interface ISpanBookkeepingMemory {
  ushort ReadWord(ushort segment,ushort offset);
  byte ReadByte(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
  void WriteByte(ushort segment,ushort offset,byte value);
 }
 public static class SpanBookkeeping {
  // Original 1F08:011F..0136. Data effects plus updated SI only.
  // The two descriptor-pointer reads remain separate, preserving alias order.
  public static ushort Execute(ISpanBookkeepingMemory m,ushort ss,ushort sourceArgumentOffset,ushort ds,ushort remainingSi,ushort transferredDi){
   if(m==null)throw new ArgumentNullException("m");
   ushort value=m.ReadWord(ss,sourceArgumentOffset);
   m.WriteWord(ss,sourceArgumentOffset,unchecked((ushort)(value+transferredDi)));
   ushort descriptor=m.ReadWord(ds,0x6259);
   ushort usedOffset=unchecked((ushort)(descriptor+0x0a));
   value=m.ReadWord(ds,usedOffset);
   m.WriteWord(ds,usedOffset,unchecked((ushort)(value+transferredDi)));
   remainingSi=unchecked((ushort)(remainingSi-transferredDi));
   if(unchecked((short)transferredDi)>0){
    descriptor=m.ReadWord(ds,0x6259);
    ushort flagOffset=unchecked((ushort)(descriptor+4));
    byte flags=m.ReadByte(ds,flagOffset);m.WriteByte(ds,flagOffset,(byte)(flags|1));
   }
   return remainingSi;
  }
 }
}
