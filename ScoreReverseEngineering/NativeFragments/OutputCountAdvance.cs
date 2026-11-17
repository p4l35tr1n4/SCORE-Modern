using System;
namespace ScoreModern.RecoveredFragments {
 public struct OutputCountResult {
  public readonly ushort AX,DX;
  public OutputCountResult(ushort ax,ushort dx){AX=ax;DX=dx;}
 }
 public static class OutputCountAdvance {
  // Original 1F08:4863..486C. Count is read from the original argument word;
  // signed extension and carry follow CWD / ADD / ADC. Full flags/ABI excluded.
  public static OutputCountResult Execute(ISpanBookkeepingMemory m,ushort ss,ushort countOffset,ushort ds,ushort descriptorSi){
   if(m==null)throw new ArgumentNullException("m");
   ushort count=m.ReadWord(ss,countOffset);
   ushort highAdd=unchecked((short)count)<0?(ushort)0xffff:(ushort)0;
   ushort lowOffset=unchecked((ushort)(descriptorSi+0x12));
   uint sum=(uint)m.ReadWord(ds,lowOffset)+count;
   m.WriteWord(ds,lowOffset,(ushort)sum);
   ushort highOffset=unchecked((ushort)(descriptorSi+0x14));
   ushort high=m.ReadWord(ds,highOffset);
   m.WriteWord(ds,highOffset,unchecked((ushort)(high+highAdd+(sum>>16))));
   return new OutputCountResult(count,highAdd);
  }
 }
}
