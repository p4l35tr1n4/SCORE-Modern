using System;
namespace ScoreModern.RecoveredFragments {
 public static class ClippingCpu {
  // 16-bit real-mode entry at clipping offset 078C. Normal-core fault flag
  // details are explicitly tied to the verified DOSBox-X reference environment.
  public static SignedDivide16Result Execute(IHorizontalRasterMemory memory,SignedDivide16State s){
   if(s.IP!=0x078c)throw new ArgumentException("Clipping entry must be 078C.");
   try {
    LineClippingFlagResult r=LineClipping.ExecuteWithFlags(memory,s.DS,s.AX,s.BX,s.CX,s.DX,s.SI,s.DI,s.EFlags);
    s.AX=r.Words.AX;s.BX=r.Words.BX;s.CX=r.Words.CX;s.DX=r.Words.DX;s.SI=r.Words.SI;s.DI=r.Words.DI;s.IP=r.Words.Exit;s.EFlags=r.EFlags;
    return new SignedDivide16Result{State=s,Fault=SignedDivide16Fault.None,DefinedEFlagsMask=0xffffffffu};
   }catch(LineClippingDivideFault fault){
    if(!fault.HasEFlags)throw;
    s.AX=fault.Words.AX;s.BX=fault.Words.BX;s.CX=fault.Words.CX;s.DX=fault.Words.DX;s.SI=fault.Words.SI;s.DI=fault.Words.DI;s.IP=fault.Words.Exit;s.EFlags=fault.EFlags;
    return SignedDivide16.Execute(memory,s);
   }
  }
 }
}
