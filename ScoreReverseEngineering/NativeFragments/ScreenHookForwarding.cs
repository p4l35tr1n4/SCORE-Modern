using System;
namespace ScoreModern.RecoveredFragments {
 public interface IScreenForwardMemory {ushort ReadWord(ushort segment,ushort offset);}
 public struct ScreenForwardResult {
  public readonly ushort DX,CX,BX,DS,SP,IP,CS;
  public ScreenForwardResult(ushort dx,ushort cx,ushort bx,ushort ds,ushort sp,ushort ip,ushort cs){DX=dx;CX=cx;BX=bx;DS=ds;SP=sp;IP=ip;CS=cs;}
 }
 public static class ScreenHookForwarding {
  // Original 16FA:02A5..02AD: four POPs then a CS-relative far-pointer read.
  // Read/result projection only. Does not execute BIOS code or IRET.
  public static ScreenForwardResult RestoreAndResolve(IScreenForwardMemory m,ushort ss,ushort sp,ushort codeSegment){
   if(m==null)throw new ArgumentNullException("m");
   ushort dx=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort cx=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort bx=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort ds=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort ip=m.ReadWord(codeSegment,0x01cc);
   ushort cs=m.ReadWord(codeSegment,0x01ce);
   return new ScreenForwardResult(dx,cx,bx,ds,sp,ip,cs);
  }
 }
}
