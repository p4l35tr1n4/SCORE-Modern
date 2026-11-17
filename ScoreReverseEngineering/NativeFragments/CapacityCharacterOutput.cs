using System;
namespace ScoreModern.RecoveredFragments {
 public static class CapacityCharacterOutput {
  static void Push(ICapacityStringMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  static bool Bios(ICapacityStringMemory m,ICapacityInterruptEffects fx,ref SignedDivide16State s,ushort at){ushort self=s.CS;s.IP=at;s=fx.Interrupt(0x10,m,s);return s.CS==self&&s.IP==(ushort)(at+2);}
  static void AH(ref SignedDivide16State s,byte v){s.AX=(ushort)((s.AX&255)|(v<<8));}
  static uint Inc8(uint f,byte a){byte b=(byte)(a+1);uint bits=(uint)(f&1);if(b==0)bits|=64;if((b&128)!=0)bits|=128;if(a==127)bits|=2048;if(((a^1^b)&16)!=0)bits|=16;int p=b;p^=p>>4;p^=p>>2;p^=p>>1;if((p&1)==0)bits|=4;return (f&~0x8d5u)|bits;}
  static CapacityDeviceResult Result(SignedDivide16State s,CapacityDeviceOutcome o){return new CapacityDeviceResult{State=s,Outcome=o};}
  // Relative 16FA:073B..07BE. BIOS INT10 effects and continuation are explicit.
  public static CapacityDeviceResult Execute(ICapacityStringMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x073b)throw new ArgumentException("Character entry must be 073B.");Push(m,ref s,s.BP);s.BP=s.SP;Push(m,ref s,s.DI);s.DI=m.ReadWord(s.SS,(ushort)(s.BP+6));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+8));AH(ref s,3);s.BX=(ushort)(s.BX&255);
   if(!Bios(m,fx,ref s,0x0746))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
   byte al=m.ReadByte(s.ES,s.DI);s.AX=(ushort)((s.AX&0xff00)|al);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,8);
   if(al==8){byte left=m.ReadByte(s.DS,0x4b98);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)s.DX,left);if((sbyte)(byte)s.DX<=(sbyte)left){
    byte dh=(byte)(s.DX>>8);s.EFlags=(RasterStatusFlags.Sub8(s.EFlags,dh,1)&~1u)|(s.EFlags&1u);s.DX=(ushort)(((byte)(dh-1)<<8)|m.ReadByte(s.DS,0x4b9a));AH(ref s,2);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x075f))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.AX=0x0a20;s.BX=0;s.CX=1;if(!Bios(m,fx,ref s,0x076b))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);goto Cleanup;
   }}
   al=m.ReadByte(s.ES,s.DI);s.AX=(ushort)((s.AX&0xff00)|al);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,27);
   if((sbyte)al>27){s.BX=m.ReadWord(s.DS,0x8164);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.BX,0);if(s.BX!=0){
    Push(m,ref s,s.AX);s.AX=0x0a08;s.CX=1;s.BX=7;if(!Bios(m,fx,ref s,0x078a))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.AX=0x0907;s.CX=1;s.BX=0x87;if(!Bios(m,fx,ref s,0x0795))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.AX=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);AH(ref s,9);s.CX=1;s.BX=0x87;if(!Bios(m,fx,ref s,0x07a0))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    AH(ref s,3);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x07a6))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.EFlags=Inc8(s.EFlags,(byte)s.DX);s.DX=(ushort)((s.DX&0xff00)|(byte)(s.DX+1));AH(ref s,2);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x07ae))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);goto Cleanup;
   }}
   AH(ref s,0x0e);s.BX=7;if(!Bios(m,fx,ref s,0x07b8))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
   Cleanup:s.DI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x07bc;return Result(s,CapacityDeviceOutcome.ReachedReturn);
  }
  public static CapacityDeviceResult ExecuteThroughReturn(ICapacityStringMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){var r=Execute(m,fx,s);if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+8);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
 public sealed class CapacityCharacterNativeEffects:ICapacityCharacterEffects {
  readonly ICapacityInterruptEffects interrupts;public CapacityCharacterNativeEffects(ICapacityInterruptEffects effects){if(effects==null)throw new ArgumentNullException("effects");interrupts=effects;}
  public SignedDivide16State CallCharacter(ICapacityStringMemory memory,SignedDivide16State state){return CapacityCharacterOutput.ExecuteThroughReturn(memory,interrupts,state).State;}
 }
}
