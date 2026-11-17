using System;
namespace ScoreModern.RecoveredFragments {
 public enum CapacityFollowupCallKind {KeyStatus,PromptOutput,KeyRead,Poll}
 public interface ICapacityFollowupEffects {SignedDivide16State Call(CapacityFollowupCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state);}
 public enum CapacityFollowupOutcome {ReachedReturn,ExternalControlTransfer,UnverifiedBranch}
 public struct CapacityFollowupResult {public SignedDivide16State State;public CapacityFollowupOutcome Outcome;public bool RetfExecuted;}
 public static class CapacityFollowup {
  static void Push(IHorizontalRasterMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  static bool Call(IHorizontalRasterMemory m,ICapacityFollowupEffects fx,ref SignedDivide16State s,CapacityFollowupCallKind kind,ushort segment,ushort offset,ushort ret){ushort self=s.CS;Push(m,ref s,self);Push(m,ref s,ret);s.CS=segment;s.IP=offset;s=fx.Call(kind,m,s);return s.CS==self&&s.IP==ret;}
  static CapacityFollowupResult Result(SignedDivide16State s,CapacityFollowupOutcome o){return new CapacityFollowupResult{State=s,Outcome=o};}
  // Relative 1597:0B47..0BAC; callee and device effects remain external.
  public static CapacityFollowupResult Execute(IHorizontalRasterMemory m,ICapacityFollowupEffects fx,SignedDivide16State s,ushort calleeSegment){
   if(s.IP!=0x0b47)throw new ArgumentException("Follow-up entry must be 0B47.");
   Push(m,ref s,s.BP);s.BP=s.SP;s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.SP,4);s.SP=(ushort)(s.SP-4);
   s.AX=m.ReadWord(s.DS,0x8184);ushort v=m.ReadWord(s.DS,0x8180);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,v,s.AX);
   if(v!=s.AX)goto Cleanup;
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.KeyStatus,calleeSegment,0x090d,0x0b5b))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   m.WriteWord(s.DS,0xaea8,s.AX);m.WriteWord(s.SS,(ushort)(s.BP-2),0x001e);s.AX=0x1aac;Push(m,ref s,s.DS);Push(m,ref s,s.AX);s.AX=(ushort)(s.BP-4);m.WriteWord(s.DS,0x62c6,s.AX);m.WriteWord(s.DS,0x62c8,s.SS);
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.PromptOutput,calleeSegment,0x06f6,0x0b77))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   StatusRepeat:
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.KeyStatus,calleeSegment,0x090d,0x0b7c))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   // OR AX,AX: DOSBox-X normal-core logical AF=0 policy; non-status bits retained.
   uint bits=s.AX==0?64u:0u;if((s.AX&0x8000)!=0)bits|=128;int parity=s.AX&255;parity^=parity>>4;parity^=parity>>2;parity^=parity>>1;if((parity&1)==0)bits|=4;s.EFlags=(s.EFlags&~0x8d5u)|bits;
   if(s.AX==0){
    if(!Call(m,fx,ref s,CapacityFollowupCallKind.Poll,calleeSegment,0x0a32,0x0b85))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
    v=m.ReadWord(s.DS,0xac52);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,v,0);
    if(v==0)goto StatusRepeat;
    do {
     if(!Call(m,fx,ref s,CapacityFollowupCallKind.Poll,calleeSegment,0x0a32,0x0b91))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
     v=m.ReadWord(s.DS,0xac52);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,v,0);
    }while(v!=0);
    goto Cleanup;
   }
   s.AX=0xaea8;Push(m,ref s,s.DS);Push(m,ref s,s.AX);s.AX=0xaeaa;Push(m,ref s,s.DS);Push(m,ref s,s.AX);
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.KeyRead,calleeSegment,0x0841,0x0ba9))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   Cleanup:
   s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0bac;return Result(s,CapacityFollowupOutcome.ReachedReturn);
  }
  public static CapacityFollowupResult ExecuteThroughReturn(IHorizontalRasterMemory m,ICapacityFollowupEffects fx,SignedDivide16State s,ushort calleeSegment){var r=Execute(m,fx,s,calleeSegment);if(r.Outcome==CapacityFollowupOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
}
