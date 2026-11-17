using System;
namespace ScoreModern.RecoveredFragments {
 // The x87 environment is explicit. This interface does not invent exception,
 // status-word, tag-word or underlying-stack behavior.
 public interface ICoordinateFpuEffects {
  void Wait();
  void StoreControlWord(IHorizontalRasterMemory memory,ushort segment,ushort offset);
  void LoadControlWord(IHorizontalRasterMemory memory,ushort segment,ushort offset);
  void StoreInteger64AndPop(IHorizontalRasterMemory memory,ushort segment,ushort offset);
 }
 public static class FiniteBinary32Integer64 {
  // Numeric projection for finite binary32 inputs, truncation toward zero.
  // False means outside this projection, not an original exception policy.
  public static bool TryConvert(uint bits,out long value){
   value=0;bool negative=(bits&0x80000000u)!=0;int exponent=(int)((bits>>23)&255);uint fraction=bits&0x7fffff;
   if(exponent==255)return false;
   int power=exponent-127;if(exponent==0||power<0)return true;
   ulong significand=0x800000u|fraction,magnitude;int shift=power-23;
   if(shift<0)magnitude=significand>>(-shift);
   else {ulong limit=negative?0x8000000000000000UL:0x7fffffffffffffffUL;if(shift>=64||significand>(limit>>shift))return false;magnitude=significand<<shift;}
   value=unchecked((long)(negative?0UL-magnitude:magnitude));return true;
  }
 }
 public static class CoordinateConversion {
  // Relative 18B9:0C70..0CA2; finite-path general-register/stack projection.
  // Wait locations follow captured runtime bytes, including runtime NOPs.
  public static SignedDivide16State Execute(IHorizontalRasterMemory m,ICoordinateFpuEffects fpu,SignedDivide16State s){
   if(s.IP!=0x0c70)throw new ArgumentException("Converter entry must be 0C70.");
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;s.SP=(ushort)(s.SP-12);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BX);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CX);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.SI);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.DI);
   fpu.Wait();fpu.StoreControlWord(m,s.SS,(ushort)(s.BP-2));fpu.Wait();
   s.AX=m.ReadWord(s.SS,(ushort)(s.BP-2));byte high=(byte)((s.AX>>8)|12);s.AX=(ushort)((s.AX&255)|(high<<8));s.EFlags=RasterStatusFlags.Logical8(s.EFlags,high);
   m.WriteWord(s.SS,(ushort)(s.BP-4),s.AX);fpu.LoadControlWord(m,s.SS,(ushort)(s.BP-4));
   fpu.StoreInteger64AndPop(m,s.SS,(ushort)(s.BP-12));fpu.LoadControlWord(m,s.SS,(ushort)(s.BP-2));
   s.AX=m.ReadWord(s.SS,(ushort)(s.BP-12));s.DX=m.ReadWord(s.SS,(ushort)(s.BP-10));
   s.DI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.SI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.CX=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.BX=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0ca2;return s;
  }
  public static SignedDivide16State ExecuteThroughReturn(IHorizontalRasterMemory m,ICoordinateFpuEffects fpu,SignedDivide16State s){
   s=Execute(m,fpu,s);ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;return s;
  }
 }
}
