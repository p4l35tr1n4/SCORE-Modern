using System;
namespace ScoreModern.RecoveredFragments {
 public enum CapacityCallKind {StringOutput,Followup}
 public interface ICapacityCallEffects {
  // Execute the separately provided callee, returning its actual continuation.
  // No message presentation, keyboard policy or callee ABI is invented here.
  SignedDivide16State Call(CapacityCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state);
 }
 public enum CapacityMessageOutcome {ReachedReturn,ExternalControlTransfer}
 public struct CapacityMessageResult {public SignedDivide16State State;public CapacityMessageOutcome Outcome;public bool RetfExecuted;}
 public static class CapacityMessageHandler {
  static SignedDivide16State Call(IHorizontalRasterMemory m,ICapacityCallEffects effects,CapacityCallKind kind,SignedDivide16State s,ushort segment,ushort offset,ushort returnOffset){
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,returnOffset);s.CS=segment;s.IP=offset;return effects.Call(kind,m,s);
  }
  // Original relative 0C25:084F..0881. The two callees remain explicit effects.
  public static CapacityMessageResult Execute(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   if(s.IP!=0x084f)throw new ArgumentException("Capacity helper entry must be 084F.");ushort self=s.CS;
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.SP,4);s.SP=(ushort)(s.SP-4);
   ushort guard=m.ReadWord(s.DS,0x80e0);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,guard,0);
   if(guard==0){
    ushort old=m.ReadWord(s.DS,0x80e0);s.EFlags=RasterStatusFlags.Inc16(s.EFlags,old);m.WriteWord(s.DS,0x80e0,(ushort)(old+1));m.WriteWord(s.SS,(ushort)(s.BP-2),0x002a);
    s.AX=0x1148;s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.DS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.AX);
    s.AX=(ushort)(s.BP-4);m.WriteWord(s.DS,0x62c6,s.AX);m.WriteWord(s.DS,0x62c8,s.SS);
    s=Call(m,effects,CapacityCallKind.StringOutput,s,stringSegment,0x06f6,0x0879);
    if(s.CS!=self||s.IP!=0x0879)return new CapacityMessageResult{State=s,Outcome=CapacityMessageOutcome.ExternalControlTransfer};
    s=Call(m,effects,CapacityCallKind.Followup,s,followupSegment,0x0b47,0x087e);
    if(s.CS!=self||s.IP!=0x087e)return new CapacityMessageResult{State=s,Outcome=CapacityMessageOutcome.ExternalControlTransfer};
   }
   s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0881;return new CapacityMessageResult{State=s,Outcome=CapacityMessageOutcome.ReachedReturn};
  }
  public static CapacityMessageResult ExecuteThroughReturn(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   CapacityMessageResult r=Execute(m,effects,s,stringSegment,followupSegment);
   if(r.Outcome==CapacityMessageOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;
  }
 }
}
