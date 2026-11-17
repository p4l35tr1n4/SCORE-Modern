using System;
namespace ScoreModern.RecoveredFragments {
 public interface IConsumerContextMemory {
  ushort ReadWord(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public static class ConsumerContextSaver {
  // Original 18B9:233E data/result effects. Explicit entry register words.
  // No exception/nonlocal-return meaning or full ABI adapter is supplied.
  public static ushort Execute(IConsumerContextMemory m,ushort stackSegment,ushort entrySp,ushort entryBp,ushort entrySi,ushort entryDi,ushort entryDs){
   if(m==null)throw new ArgumentNullException("m");
   ushort destination=m.ReadWord(stackSegment,unchecked((ushort)(entrySp+4)));
   ushort destinationSegment=m.ReadWord(stackSegment,unchecked((ushort)(entrySp+6)));
   m.WriteWord(destinationSegment,destination,entryBp);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+2)),entryDi);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+4)),entrySi);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+6)),entrySp);
   ushort value=m.ReadWord(stackSegment,entrySp);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+8)),value);
   value=m.ReadWord(stackSegment,unchecked((ushort)(entrySp+2)));
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+10)),value);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+12)),entryDs);
   return 0;
  }
 }
}
