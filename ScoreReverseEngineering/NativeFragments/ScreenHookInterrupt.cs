using System;
namespace ScoreModern.RecoveredFragments {
 public interface IScreenBiosTransferEffects {SignedDivide16State TransferToBios(IScreenHookMemory memory,SignedDivide16State state);}
 public static class ScreenHookInterrupt {
  // Measured 16-bit real-mode CD 10 entry. The destination BIOS/IRET is external.
  public static SignedDivide16State Enter(IScreenHookMemory m,SignedDivide16State s){
   ushort next=(ushort)(s.IP+2);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,(ushort)s.EFlags);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,next);s.EFlags&=~0x300u;s.IP=m.ReadWord(0,0x40);s.CS=m.ReadWord(0,0x42);return s;
  }
 }
 public sealed class ScreenHookInterruptEffects:ICapacityInterruptEffects {
  readonly IScreenBiosTransferEffects bios;readonly ICapacityInterruptEffects other;readonly ushort hookCS,hookIP,dataSegment;
  public ScreenHookInterruptEffects(IScreenBiosTransferEffects biosEffects,ICapacityInterruptEffects otherInterruptEffects,ushort hookedSegment,ushort hookedOffset,ushort relocatedDataSegment){if(biosEffects==null)throw new ArgumentNullException("biosEffects");bios=biosEffects;other=otherInterruptEffects;hookCS=hookedSegment;hookIP=hookedOffset;dataSegment=relocatedDataSegment;}
  public SignedDivide16State Interrupt(byte number,IHorizontalRasterMemory memory,SignedDivide16State state){
   if(number!=0x10){if(other==null)throw new InvalidOperationException("Other interrupt effect required.");return other.Interrupt(number,memory,state);}
   var m=memory as IScreenHookMemory;if(m==null)throw new ArgumentException("Screen-hook byte memory required.");state=ScreenHookInterrupt.Enter(m,state);
   if(state.CS==hookCS&&state.IP==hookIP){var r=ScreenHookRoutine.Execute(m,this,state,dataSegment);state=r.State;if(r.Outcome!=ScreenHookOutcome.Forwarded)return state;}
   return bios.TransferToBios(m,state);
  }
 }
}
