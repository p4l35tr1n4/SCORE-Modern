using System;
namespace ScoreModern.RecoveredFragments {
 public interface IPrefixPreparationMemory {
  byte ReadByte(ushort segment,ushort offset);
  ushort ReadWord(ushort segment,ushort offset);
  void WriteByte(ushort segment,ushort offset,byte value);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public struct PrefixPreparationResult {
  public readonly ushort AX,BP,SP,SI,DI,RelativeTargetSegment,TargetOffset;
  public PrefixPreparationResult(ushort ax,ushort bp,ushort sp,ushort si,ushort di){AX=ax;BP=bp;SP=sp;SI=si;DI=di;RelativeTargetSegment=0x18b9;TargetOffset=0x312e;}
 }
 public static class PrefixWritePreparation {
  static void Push(IPrefixPreparationMemory m,ushort ss,ref ushort sp,ushort value){sp=unchecked((ushort)(sp-2));m.WriteWord(ss,sp,value);}
  // Original 1F08:33FA..3458, through the far-call frame preparation.
  // Explicit entry words and memory; stops before the external callee.
  // Source sequences remain at original data addresses; no strings embedded.
  public static PrefixPreparationResult Prepare(IPrefixPreparationMemory m,ushort ss,ushort ds,ushort cs,ushort sp,ushort bp,ushort si,ushort di){
   if(m==null)throw new ArgumentNullException("m");
   Push(m,ss,ref sp,bp);bp=sp;sp=unchecked((ushort)(sp-6));Push(m,ss,ref sp,di);Push(m,ss,ref sp,si);
   si=2;di=0x6391;
   ushort descriptor=m.ReadWord(ds,0x6259);byte handle;
   if(m.ReadByte(ds,unchecked((ushort)(descriptor+2)))==0)handle=1;
   else{descriptor=m.ReadWord(ds,0x6259);handle=m.ReadByte(ds,unchecked((ushort)(descriptor+2)));}
   m.WriteByte(ss,unchecked((ushort)(bp-6)),handle);
   byte prefix=m.ReadByte(ss,unchecked((ushort)(bp+4)));
   if(prefix==0x2b)si=1;
   else if(prefix==0x30)si=4;
   else if(prefix==0x31){di=0x638b;si=6;}
   Push(m,ss,ref sp,si);Push(m,ss,ref sp,ds);Push(m,ss,ref sp,di);
   ushort ax=unchecked((ushort)(short)(sbyte)m.ReadByte(ss,unchecked((ushort)(bp-6))));
   Push(m,ss,ref sp,ax);
   Push(m,ss,ref sp,cs);Push(m,ss,ref sp,0x3459);
   return new PrefixPreparationResult(ax,bp,sp,si,di);
  }
 }
}
