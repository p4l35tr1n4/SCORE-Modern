namespace ScoreModern.RecoveredFragments {
 public enum SignedDivide16Fault {None,ZeroDivisor,QuotientOverflow}
 public struct SignedDivide16State {
  public ushort AX,BX,CX,DX,SI,DI,BP,CS,IP,DS,ES,SS,SP;
  public uint EFlags;
 }
 public struct SignedDivide16Result {
  public SignedDivide16State State;
  public SignedDivide16Fault Fault;
  // On successful IDIV the arithmetic flag values are undefined, not verified outputs.
  public uint DefinedEFlagsMask;
 }
 public static class SignedDivide16 {
  // IDIV CX and vector-zero delivery in the measured 16-bit real-mode VM.
  // A fault preserves the faulting IP. Success advances past F7 F9.
  public static SignedDivide16Result Execute(IHorizontalRasterMemory m,SignedDivide16State s){
   int n=unchecked((int)(((uint)s.DX<<16)|s.AX)),d=(short)s.CX;
   SignedDivide16Fault cause=SignedDivide16Fault.None;int q=0;
   if(d==0)cause=SignedDivide16Fault.ZeroDivisor;
   else if(n==int.MinValue&&d==-1)cause=SignedDivide16Fault.QuotientOverflow;
   else {q=n/d;if(q<short.MinValue||q>short.MaxValue)cause=SignedDivide16Fault.QuotientOverflow;}
   if(cause==SignedDivide16Fault.None){s.AX=(ushort)q;s.DX=(ushort)(n%d);s.IP=(ushort)(s.IP+2);
    return new SignedDivide16Result{State=s,Fault=cause,DefinedEFlagsMask=~0x8d5u};}
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,(ushort)s.EFlags);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.IP);
   s.EFlags&=~0x300u;
   s.IP=m.ReadWord(0,0);s.CS=m.ReadWord(0,2);
   return new SignedDivide16Result{State=s,Fault=cause,DefinedEFlagsMask=0xffffffffu};
  }
 }
}
