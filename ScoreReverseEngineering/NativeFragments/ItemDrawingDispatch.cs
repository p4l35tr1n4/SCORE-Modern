using System;
namespace ScoreModern.RecoveredFragments {
 public struct ItemDrawingRoute {public bool UsesTable;public ushort TableByteIndex,Target;}
 public static class ItemDrawingDispatch {
  // Relative 12DD:0542..0554 route/data projection. The table stays in
  // supplied memory; no original targets or resource contents are embedded.
  // Bypass means execution continues at 0575, not an unsupported-item policy.
  // AX/BX exchange, flags, ABI and the secondary table are outside this API.
  public static ItemDrawingRoute Select(IDatTextLookupMemory memory,ushort ds,ushort cs){
   if(memory==null)throw new ArgumentNullException("memory");
   ushort index=unchecked((ushort)(memory.ReadWord(ds,0x7e50)-1));
   if(index>15)return new ItemDrawingRoute{UsesTable=false};
   ushort byteIndex=(ushort)(index*2);
   return new ItemDrawingRoute{UsesTable=true,TableByteIndex=byteIndex,
    Target=memory.ReadWord(cs,unchecked((ushort)(0x0555+byteIndex)))};
  }
 }
}
