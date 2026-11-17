using System;
namespace ScoreModern.RecoveredFragments {
 public interface IFormatterWordContext {
  ushort ReadWord(ushort offset);
  void WriteByte(ushort offset, byte value);
  void WriteWord(ushort offset, ushort value);
 }
 public static class FormatterWordWriter {
  // Original 1F08:2A4B data effects only. The context supplies the actual DS
  // memory operations. This is not an ABI, stack, register or flag adapter.
  public static void Execute(IFormatterWordContext context, ushort low, ushort high) {
   if(context==null)throw new ArgumentNullException("context");
   ushort cursor=context.ReadWord(0xd988);
   int value=unchecked((int)(((uint)high<<16)|low));
   if(value>=-120 && value<=120) {
    context.WriteByte(cursor,unchecked((byte)low));
    cursor=unchecked((ushort)(cursor+1));
   } else {
    context.WriteByte(cursor,0x7f);
    cursor=unchecked((ushort)(cursor+1));
    context.WriteWord(cursor,low);
    context.WriteWord(unchecked((ushort)(cursor+2)),high);
    cursor=unchecked((ushort)(cursor+4));
   }
   context.WriteWord(0xd988,cursor);
  }
 }
}
