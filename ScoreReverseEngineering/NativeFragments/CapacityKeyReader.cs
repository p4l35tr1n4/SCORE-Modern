using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICapacityKeyMemory:IHorizontalRasterMemory {void WriteByte(ushort segment,ushort offset,byte value);}
 public static class CapacityKeyReader {
  // Relative 16FA:0841..0857: full word-register/flags/stack projection.
  public static CapacityDeviceResult Execute(ICapacityKeyMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x0841)throw new ArgumentException("Key-reader entry must be 0841.");ushort self=s.CS;s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;s.AX=(ushort)(s.AX&255);s.IP=0x0846;s=fx.Interrupt(0x16,m,s);
   if(s.CS!=self||s.IP!=0x0848)return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ExternalControlTransfer};
   s.BX=m.ReadWord(s.SS,(ushort)(s.BP+6));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+8));m.WriteByte(s.ES,s.BX,(byte)s.AX);
   s.BX=m.ReadWord(s.SS,(ushort)(s.BP+10));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+12));m.WriteByte(s.ES,s.BX,(byte)(s.AX>>8));
   s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0855;return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ReachedReturn};
  }
  public static CapacityDeviceResult ExecuteThroughReturn(ICapacityKeyMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){var r=Execute(m,fx,s);if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+12);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
}
