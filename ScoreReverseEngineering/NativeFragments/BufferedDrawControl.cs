using System;
namespace ScoreModern.RecoveredFragments {
 public interface IBufferedDrawMemory {ushort ReadWord(ushort segment,ushort offset);void WriteWord(ushort segment,ushort offset,ushort value);}
 // AX is projected by every method. CX is projected only by SelectRasterPath
 // and UpdatePosition; its default in DecodeMarker is not an original CX value.
 // Branch is meaningful only for DecodeMarker and SelectRasterPath.
 public struct BufferedDrawWords {public ushort AX,CX;public bool Branch;}
 public static class BufferedDrawControl {
  // 1BF3:0041..004D. Branch=true is the encoded-marker path.
  public static BufferedDrawWords DecodeMarker(IBufferedDrawMemory m,ushort ds,ushort ax){
   if(m==null)throw new ArgumentNullException("m");bool marker=unchecked((short)ax)<=unchecked((short)0x80a8);
   if(marker){ax=unchecked((ushort)(ax+0x7fbc));m.WriteWord(ds,0xac48,ax);}
   return new BufferedDrawWords{AX=ax,Branch=marker};
  }
  // 1807:0738..0743. Branch=true selects the following skip-raster far jump.
  public static BufferedDrawWords SelectRasterPath(IBufferedDrawMemory m,ushort ds){
   if(m==null)throw new ArgumentNullException("m");ushort ax=m.ReadWord(ds,0xac42),cx=m.ReadWord(ds,0xac48);
   return new BufferedDrawWords{AX=ax,CX=cx,Branch=unchecked((sbyte)(byte)cx)>0};
  }
  // 1807:0B1C..0B26, before RETF. Explicit saved-position data effects only.
  public static BufferedDrawWords UpdatePosition(IBufferedDrawMemory m,ushort ds,ushort ax){
   if(m==null)throw new ArgumentNullException("m");ushort cx=m.ReadWord(ds,0xac40);m.WriteWord(ds,0xac44,cx);m.WriteWord(ds,0xac46,ax);
   return new BufferedDrawWords{AX=ax,CX=cx};
  }
 }
}
