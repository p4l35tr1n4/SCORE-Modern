using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICapacityStringMemory:IHorizontalRasterMemory {byte ReadByte(ushort segment,ushort offset);}
 public interface ICapacityCharacterEffects {SignedDivide16State CallCharacter(ICapacityStringMemory memory,SignedDivide16State state);}
 public static class CapacityStringOutput {
  static void Push(ICapacityStringMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  // Relative 16FA:06F6..0712: character callee effects remain explicit.
  public static CapacityDeviceResult Execute(ICapacityStringMemory m,ICapacityCharacterEffects fx,SignedDivide16State s,ushort characterSegment){
   if(s.IP!=0x06f6)throw new ArgumentException("String entry must be 06F6.");ushort self=s.CS;Push(m,ref s,s.BP);s.BP=s.SP;Push(m,ref s,s.DI);s.DI=m.ReadWord(s.SS,(ushort)(s.BP+6));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+8));
   while(true){byte b=m.ReadByte(s.ES,s.DI);s.AX=(ushort)((s.AX&0xff00)|b);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,b,0);if(b==0)break;
    Push(m,ref s,s.ES);Push(m,ref s,s.DI);Push(m,ref s,self);Push(m,ref s,0x070b);s.CS=characterSegment;s.IP=0x073b;s=fx.CallCharacter(m,s);
    if(s.CS!=self||s.IP!=0x070b)return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ExternalControlTransfer};s.EFlags=RasterStatusFlags.Inc16(s.EFlags,s.DI);s.DI=(ushort)(s.DI+1);
   }
   s.DI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0710;return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ReachedReturn};
  }
  public static CapacityDeviceResult ExecuteThroughReturn(ICapacityStringMemory m,ICapacityCharacterEffects fx,SignedDivide16State s,ushort characterSegment){var r=Execute(m,fx,s,characterSegment);if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+8);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
}
