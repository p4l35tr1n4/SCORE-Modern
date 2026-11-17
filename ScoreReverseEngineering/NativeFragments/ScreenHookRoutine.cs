using System;
namespace ScoreModern.RecoveredFragments {
 public interface IScreenHookMemory:ICapacityStringMemory {void WriteByte(ushort segment,ushort offset,byte value);}
 public enum ScreenHookOutcome {Forwarded,ExternalControlTransfer}
 public struct ScreenHookResult {public SignedDivide16State State;public ScreenHookOutcome Outcome;}
 public static class ScreenHookRoutine {
  static void Push(IScreenHookMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  static ushort Pop(IScreenHookMemory m,ref SignedDivide16State s){ushort v=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);return v;}
  static void AH(ref SignedDivide16State s,byte v){s.AX=(ushort)((s.AX&255)|(v<<8));}
  static bool Bios(IScreenHookMemory m,ICapacityInterruptEffects fx,ref SignedDivide16State s,ushort at){ushort self=s.CS;s.IP=at;s=fx.Interrupt(0x10,m,s);return s.CS==self&&s.IP==(ushort)(at+2);}
  static void DecRow(ref SignedDivide16State s){byte a=(byte)(s.DX>>8);s.EFlags=(RasterStatusFlags.Sub8(s.EFlags,a,1)&~1u)|(s.EFlags&1u);s.DX=(ushort)((s.DX&255)|((byte)(a-1)<<8));}
  static ScreenHookResult External(SignedDivide16State s){return new ScreenHookResult{State=s,Outcome=ScreenHookOutcome.ExternalControlTransfer};}
  // Relative 16FA:0235..02AD. Entry already has the CPU-created interrupt frame.
  // STI/CLD flag changes are modeled; interrupt scheduling/shadow is not.
  public static ScreenHookResult Execute(IScreenHookMemory m,ICapacityInterruptEffects fx,SignedDivide16State s,ushort dataSegment){
   if(s.IP!=0x0235)throw new ArgumentException("Screen hook entry must be 0235.");ushort self=s.CS;s.EFlags=(s.EFlags|0x200u)&~0x400u;Push(m,ref s,s.DS);Push(m,ref s,s.AX);s.AX=dataSegment;s.DS=s.AX;s.AX=Pop(m,ref s);
   byte ah=(byte)(s.AX>>8),al=(byte)s.AX;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,ah,0);
   if(ah==0){m.WriteByte(s.DS,0x4ba2,al);goto RestoreDs;}
   byte mode=m.ReadByte(s.DS,0x4ba2);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,mode,2);if((sbyte)mode<=2)goto RestoreDs;
   s.EFlags=RasterStatusFlags.Sub8(s.EFlags,ah,0x0e);if(ah!=0x0e)goto RestoreDs;
   s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,8);if(al==8)goto RestoreDs;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,7);if(al==7)goto RestoreDs;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,13);if(al==13)goto RestoreDs;
   s.BX=7;Push(m,ref s,s.BX);Push(m,ref s,s.CX);Push(m,ref s,s.DX);Push(m,ref s,s.AX);AH(ref s,3);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x026d))return External(s);s.AX=Pop(m,ref s);
   al=(byte)s.AX;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,10);
   if(al!=10){byte col=m.ReadByte(s.DS,0x4b9a);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)s.DX,col);if((sbyte)(byte)s.DX<(sbyte)col)goto RestoreSaved;}
   byte row=m.ReadByte(s.DS,0x4b9b);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)(s.DX>>8),row);if((sbyte)(byte)(s.DX>>8)<(sbyte)row)goto RestoreSaved;
   Push(m,ref s,s.AX);Push(m,ref s,s.DX);s.CX=m.ReadWord(s.DS,0x4b98);s.DX=m.ReadWord(s.DS,0x4b9a);s.AX=0x0601;s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x028f))return External(s);
   s.DX=Pop(m,ref s);DecRow(ref s);AH(ref s,2);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x0298))return External(s);s.DX=m.ReadWord(s.DS,0x4b9e);DecRow(ref s);m.WriteWord(s.DS,0x4b9e,s.DX);s.AX=Pop(m,ref s);
   RestoreSaved:s.DX=Pop(m,ref s);s.CX=Pop(m,ref s);s.BX=Pop(m,ref s);
   RestoreDs:s.DS=Pop(m,ref s);ushort ip=m.ReadWord(self,0x01cc),cs=m.ReadWord(self,0x01ce);s.IP=ip;s.CS=cs;return new ScreenHookResult{State=s,Outcome=ScreenHookOutcome.Forwarded};
  }
 }
}
