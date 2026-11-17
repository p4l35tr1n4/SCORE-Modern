using System;
namespace ScoreModern.RecoveredFragments {
 public static class BufferedBeamLinePipeline {
  sealed class Words:IPointBufferMemory {
   readonly IHorizontalRasterMemory memory;
   public Words(IHorizontalRasterMemory m){memory=m;}
   public ushort ReadWord(ushort s,ushort o){return memory.ReadWord(s,o);}
   public void WriteWord(ushort s,ushort o,ushort v){memory.WriteWord(s,o,v);}
  }
  // Compose the verified normal decoder coordinate block, its actual far-call
  // frame, and the complete flag-aware native driver through RETF. Entry 004F,
  // normal exit in the original decoder at 0074. No host graphics/ROM embedded.
  public static LineDriverBodyResult Execute(IByteOrResultMemory memory,SignedDivide16State state,ushort driverSegment){
   if(memory==null)throw new ArgumentNullException("memory");
   state=BufferedCoordinateDecode.Execute(new Words(memory),state);
   state.SP=unchecked((ushort)(state.SP-2));memory.WriteWord(state.SS,state.SP,state.CS);
   state.SP=unchecked((ushort)(state.SP-2));memory.WriteWord(state.SS,state.SP,0x74);
   state.CS=driverSegment;state.IP=0x738;
   return LineDriverBody.ExecuteThroughReturnWithFlags(memory,state);
  }
 }
}
