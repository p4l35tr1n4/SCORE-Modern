using System;
namespace ScoreModern.RecoveredFragments {
 public interface IBufferedPointFpuEffects:ICoordinateFpuEffects {
  void LoadBinary32(IHorizontalRasterMemory memory,ushort segment,ushort offset);
 }
 public enum BufferedPointOutcome {Completed,CapacityHandlerRequired,NormalPathReady,ExternalControlTransfer}
 public struct BufferedPointCallerResult {
  public SignedDivide16State State;public BufferedPointOutcome Outcome;public ushort RequestedCalleeOffset;public bool RetfExecuted;
 }
 public static class BufferedPointCaller {
  sealed class AppendMemory:IPointBufferMemory {
   readonly IHorizontalRasterMemory memory;public AppendMemory(IHorizontalRasterMemory m){memory=m;}
   public ushort ReadWord(ushort s,ushort o){return memory.ReadWord(s,o);}
   public void WriteWord(ushort s,ushort o,ushort v){memory.WriteWord(s,o,v);}
  }
  static void LoadPointer(IHorizontalRasterMemory m,ref SignedDivide16State s,ushort offset){ushort at=(ushort)(s.BP+offset);s.BX=m.ReadWord(s.SS,at);s.ES=m.ReadWord(s.SS,(ushort)(at+2));}
  static SignedDivide16State Convert(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,SignedDivide16State s,ushort converterSegment,ushort returnOffset){
   fpu.LoadBinary32(m,s.ES,s.BX);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,returnOffset);
   s.CS=converterSegment;s.IP=0x0c70;return CoordinateConversion.ExecuteThroughReturn(m,fpu,s);
  }
  // Relative 0C25:0752..084E. The unported capacity callee is an explicit
  // request boundary, not an invented error or buffer-drain policy.
  public static BufferedPointCallerResult Enter(IHorizontalRasterMemory m,SignedDivide16State s){
   if(s.IP!=0x0752)throw new ArgumentException("Point caller entry must be 0752.");
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.SI);
   s.ES=m.ReadWord(s.DS,0x69da);s.AX=m.ReadWord(s.DS,0x80de);ushort count=m.ReadWord(s.ES,0);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,count,s.AX);
   if((short)count>=(short)s.AX){s.IP=0x0764;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.CapacityHandlerRequired,RequestedCalleeOffset=0x084f};}
   s.IP=0x076c;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.NormalPathReady};
  }
  public static BufferedPointCallerResult Execute(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,SignedDivide16State s,ushort converterSegment){
   BufferedPointCallerResult gate=Enter(m,s);if(gate.Outcome==BufferedPointOutcome.CapacityHandlerRequired)return gate;s=gate.State;
   LoadPointer(m,ref s,14);s=Convert(m,fpu,s,converterSegment,0x0778);m.WriteWord(s.DS,0xc590,s.AX);m.WriteWord(s.DS,0xc592,s.DX);
   LoadPointer(m,ref s,10);s=Convert(m,fpu,s,converterSegment,0x078b);m.WriteWord(s.DS,0xc594,s.AX);m.WriteWord(s.DS,0xc596,s.DX);
   s.EFlags=CoordinateWordClamp.Execute(m,s.DS,s.EFlags);
   PointBufferWords appended=PointBufferAppend.ExecuteWithFlags(new AppendMemory(m),s.DS,s.SS,s.BP,s.EFlags);
   s.AX=appended.AX;s.BX=appended.BX;s.SI=appended.SI;s.ES=appended.ES;s.EFlags=appended.EFlags;
   s.SI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x084c;
   return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.Completed};
  }
  static BufferedPointCallerResult RunCapacity(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   ushort self=s.CS;s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,self);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,0x0769);s.IP=0x084f;
   CapacityMessageResult handler=CapacityMessageHandler.ExecuteThroughReturn(m,effects,s,stringSegment,followupSegment);s=handler.State;
   if(handler.Outcome!=CapacityMessageOutcome.ReachedReturn||s.CS!=self||s.IP!=0x0769)return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.ExternalControlTransfer};
   s.SI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x084c;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.Completed};
  }
  public static BufferedPointCallerResult ExecuteWithCapacityEffects(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,ICapacityCallEffects effects,SignedDivide16State s,ushort converterSegment,ushort stringSegment,ushort followupSegment){
   BufferedPointCallerResult r=Execute(m,fpu,s,converterSegment);return r.Outcome==BufferedPointOutcome.CapacityHandlerRequired?RunCapacity(m,effects,r.State,stringSegment,followupSegment):r;
  }
  // Also exposes the measured comparison boundary for the approved AX-only fixtures.
  public static BufferedPointCallerResult ExecuteCapacityAtComparison(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   if(s.IP!=0x075d)throw new ArgumentException("Capacity comparison entry must be 075D.");ushort count=m.ReadWord(s.ES,0);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,count,s.AX);
   if((short)count<(short)s.AX){s.IP=0x076c;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.NormalPathReady};}
   s.IP=0x0764;return RunCapacity(m,effects,s,stringSegment,followupSegment);
  }
  public static BufferedPointCallerResult ExecuteCapacityAtComparisonThroughReturn(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){return Return(m,ExecuteCapacityAtComparison(m,effects,s,stringSegment,followupSegment));}
  public static BufferedPointCallerResult ExecuteThroughReturn(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,SignedDivide16State s,ushort converterSegment){
   BufferedPointCallerResult result=Execute(m,fpu,s,converterSegment);
   return Return(m,result);
  }
  static BufferedPointCallerResult Return(IHorizontalRasterMemory m,BufferedPointCallerResult result){
   if(result.Outcome==BufferedPointOutcome.Completed){SignedDivide16State s=result.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+16);s.IP=ip;s.CS=cs;result.State=s;result.RetfExecuted=true;}
   return result;
  }
 }
}
