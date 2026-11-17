using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICapacityInterruptEffects {SignedDivide16State Interrupt(byte number,IHorizontalRasterMemory memory,SignedDivide16State state);}
 public enum CapacityDeviceOutcome {ReachedReturn,ExternalControlTransfer}
 public struct CapacityDeviceResult {public SignedDivide16State State;public CapacityDeviceOutcome Outcome;public bool RetfExecuted;}
 public static class CapacityDeviceCallees {
  static CapacityDeviceResult Done(SignedDivide16State s,CapacityDeviceOutcome o){return new CapacityDeviceResult{State=s,Outcome=o};}
  static CapacityDeviceResult Return(IHorizontalRasterMemory m,CapacityDeviceResult r){if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){var s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
  // Relative 16FA:090D..091C. INT effects include its actual continuation.
  public static CapacityDeviceResult KeyStatus(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x090d)throw new ArgumentException("Key-status entry must be 090D.");ushort self=s.CS;s.AX=(ushort)((s.AX&255)|0x100);s.IP=0x090f;s=fx.Interrupt(0x16,m,s);
   if(s.CS!=self||s.IP!=0x0911)return Done(s,CapacityDeviceOutcome.ExternalControlTransfer);
   s.AX=(ushort)((s.EFlags&0x40)!=0?0:1);s.IP=0x091c;return Done(s,CapacityDeviceOutcome.ReachedReturn);
  }
  public static CapacityDeviceResult KeyStatusThroughReturn(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){return Return(m,KeyStatus(m,fx,s));}
  // Relative 16FA:0A32..0A4E. The gate's meaning and device values remain external.
  public static CapacityDeviceResult Poll(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x0a32)throw new ArgumentException("Poll entry must be 0A32.");ushort self=s.CS;s.AX=m.ReadWord(s.DS,0xac58);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.AX,0);
   if(s.AX!=0){s.AX=3;s.IP=0x0a3d;s=fx.Interrupt(0x33,m,s);if(s.CS!=self||s.IP!=0x0a3f)return Done(s,CapacityDeviceOutcome.ExternalControlTransfer);m.WriteWord(s.DS,0xac50,s.AX);m.WriteWord(s.DS,0xac52,s.BX);m.WriteWord(s.DS,0xac54,s.CX);m.WriteWord(s.DS,0xac56,s.DX);}
   s.IP=0x0a4e;return Done(s,CapacityDeviceOutcome.ReachedReturn);
  }
  public static CapacityDeviceResult PollThroughReturn(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){return Return(m,Poll(m,fx,s));}
 }
}
