using System;
namespace ScoreModern.RecoveredFragments {
 public interface IParameterWordMemory {
  byte ReadByte(ushort segment,ushort offset);
  ushort ReadWord(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public static class ParameterWordReader {
  // Data effects of original 1F08:36E0..370A. The cursor is the far-pointer
  // local at SS:BP-8; destination is DS:SI. Preserve all memory-access order.
  // Returned word is the block's updated SI; full ABI/flags are not supplied.
  public static ushort Execute(IParameterWordMemory m,ushort cursorSegment,ushort cursorOffset,ushort destinationSegment,ushort destinationOffset){
   if(m==null)throw new ArgumentNullException("m");
   ushort source=m.ReadWord(cursorSegment,cursorOffset);
   ushort sourceSegment=m.ReadWord(cursorSegment,unchecked((ushort)(cursorOffset+2)));
   ushort cursor=m.ReadWord(cursorSegment,cursorOffset);
   m.WriteWord(cursorSegment,cursorOffset,unchecked((ushort)(cursor+1)));
   short value=unchecked((sbyte)m.ReadByte(sourceSegment,source));
   m.WriteWord(destinationSegment,destinationOffset,unchecked((ushort)value));
   m.WriteWord(destinationSegment,unchecked((ushort)(destinationOffset+2)),value<0?(ushort)0xffff:(ushort)0);
   if(m.ReadByte(destinationSegment,destinationOffset)==0x7f){
    source=m.ReadWord(cursorSegment,cursorOffset);
    sourceSegment=m.ReadWord(cursorSegment,unchecked((ushort)(cursorOffset+2)));
    ushort low=m.ReadWord(sourceSegment,source);
    ushort high=m.ReadWord(sourceSegment,unchecked((ushort)(source+2)));
    m.WriteWord(destinationSegment,destinationOffset,low);
    m.WriteWord(destinationSegment,unchecked((ushort)(destinationOffset+2)),high);
    cursor=m.ReadWord(cursorSegment,cursorOffset);
    m.WriteWord(cursorSegment,cursorOffset,unchecked((ushort)(cursor+4)));
   }
   return unchecked((ushort)(destinationOffset+4));
  }
 }
}
