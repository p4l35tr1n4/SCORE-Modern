using System;
namespace ScoreModern.RecoveredFragments {
 public struct LineDriverBodyResult {
  public SignedDivide16State State;public uint DefinedEFlagsMask;
  public ushort ClippingExit,RasterEntry;public SignedDivide16Fault Fault;public bool RetfExecuted;
 }
 public static class LineDriverBody {
  sealed class ControlMemory:IBufferedDrawMemory {
   readonly IHorizontalRasterMemory memory;
   public ControlMemory(IHorizontalRasterMemory m){memory=m;}
   public ushort ReadWord(ushort s,ushort o){return memory.ReadWord(s,o);}
   public void WriteWord(ushort s,ushort o,ushort v){memory.WriteWord(s,o,v);}
  }
  static void Apply(ref SignedDivide16State s,VerticalRasterWords r){s.AX=r.AX;s.BX=r.BX;s.CX=r.CX;s.DX=r.DX;s.SI=r.SI;s.DI=r.DI;s.BP=r.BP;}
  // Original body 1807:0738 through 0B27, stopped before RETF.
  // Video instruction effects remain supplied by the caller's memory adapter.
  public static LineDriverBodyResult Execute(IHorizontalRasterMemory memory,SignedDivide16State s){
   return ExecuteCore(memory,s,false);
  }
  public static LineDriverBodyResult ExecuteWithFlags(IByteOrResultMemory memory,SignedDivide16State s){return ExecuteCore(memory,s,true);}
  static LineDriverBodyResult ExecuteCore(IHorizontalRasterMemory memory,SignedDivide16State s,bool fullFlags){
   if(s.IP!=0x0738)throw new ArgumentException("Line driver body entry must be 0738.");
   ControlMemory control=new ControlMemory(memory);BufferedDrawWords selection=BufferedDrawControl.SelectRasterPath(control,s.DS);
   s.AX=selection.AX;s.CX=selection.CX;if(fullFlags)s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)s.CX,0);ushort clipExit=0,rasterEntry=0;
   if(!selection.Branch){
    s.SP=(ushort)(s.SP-2);memory.WriteWord(s.SS,s.SP,s.BP);
    s.SP=(ushort)(s.SP-2);memory.WriteWord(s.SS,s.SP,s.DI);
    s.SP=(ushort)(s.SP-2);memory.WriteWord(s.SS,s.SP,s.SI);
    LinePreparationWords p=LinePreparation.Execute(memory,s.DS,s.AX);
    s.AX=p.AX;s.BX=p.BX;s.CX=p.CX;s.DX=p.DX;s.DI=p.DI;s.ES=p.ES;s.IP=0x078c;
    SignedDivide16Result clipped=ClippingCpu.Execute(memory,s);s=clipped.State;clipExit=s.IP;
    if(clipped.Fault!=SignedDivide16Fault.None)return new LineDriverBodyResult{State=s,DefinedEFlagsMask=clipped.DefinedEFlagsMask,ClippingExit=clipExit,Fault=clipped.Fault};
    if(s.IP==0x0981){RasterDispatchWords d=RasterDispatch.Execute(memory,s.DS,s.BX,s.CX,s.DX,s.EFlags);s.BX=d.BX;s.CX=d.CX;s.DX=d.DX;s.EFlags=d.EFlags;s.IP=d.Exit;}
    rasterEntry=s.IP;
    switch(s.IP){
     case 0x09b8:VerticalRasterWords sl=fullFlags?SlopedRaster.ExecuteWithFlags(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX,s.EFlags):SlopedRaster.Execute(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX);Apply(ref s,sl);if(fullFlags)s.EFlags=sl.EFlags;break;
     case 0x0a3d:VerticalRasterWords eq=fullFlags?EqualStepRaster.ExecuteWithFlags(memory,s.DS,s.ES,s.AX,s.CX,s.DX,s.SI,s.EFlags):EqualStepRaster.Execute(memory,s.DS,s.ES,s.AX,s.CX,s.DX,s.SI);Apply(ref s,eq);if(fullFlags)s.EFlags=eq.EFlags;break;
     case 0x0a96:VerticalRasterWords v=fullFlags?VerticalRaster.ExecuteWithFlags(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX,s.EFlags):VerticalRaster.Execute(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX);Apply(ref s,v);if(fullFlags)s.EFlags=v.EFlags;break;
     case 0x0ac6:HorizontalRasterWords h=fullFlags?HorizontalRaster.ExecuteWithFlags((IByteOrResultMemory)memory,s.DS,s.ES,s.AX,s.CX,s.DX,s.EFlags):HorizontalRaster.Execute(memory,s.DS,s.ES,s.AX,s.CX,s.DX);if(fullFlags)s.EFlags=h.EFlags;s.AX=h.AX;s.BX=h.BX;s.CX=h.CX;s.DX=h.DX;s.SI=h.SI;s.DI=h.DI;break;
     case 0x0b16:break;
     default:throw new NotSupportedException("Unverified clipping/raster exit.");
    }
    s.SI=memory.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
    s.DI=memory.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
    s.BP=memory.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
    s.AX=memory.ReadWord(s.DS,0xac42);
   }
   BufferedDrawWords position=BufferedDrawControl.UpdatePosition(control,s.DS,s.AX);s.AX=position.AX;s.CX=position.CX;s.IP=0x0b27;
   // Full flags require the explicit CPU-visible horizontal OR result contract.
   return new LineDriverBodyResult{State=s,DefinedEFlagsMask=fullFlags?0xffffffffu:~0x8d5u,ClippingExit=clipExit,RasterEntry=rasterEntry,Fault=SignedDivide16Fault.None};
  }
  public static LineDriverBodyResult ExecuteThroughReturnWithFlags(IByteOrResultMemory memory,SignedDivide16State s){
   return Return(memory,ExecuteWithFlags(memory,s));
  }
  public static LineDriverBodyResult ExecuteThroughReturn(IHorizontalRasterMemory memory,SignedDivide16State s){
   LineDriverBodyResult result=Execute(memory,s);
   return Return(memory,result);
  }
  static LineDriverBodyResult Return(IHorizontalRasterMemory memory,LineDriverBodyResult result){
   if(result.Fault==SignedDivide16Fault.None){SignedDivide16State s=result.State;ushort ip=memory.ReadWord(s.SS,s.SP),cs=memory.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;result.State=s;result.RetfExecuted=true;}
   return result;
  }
 }
}
