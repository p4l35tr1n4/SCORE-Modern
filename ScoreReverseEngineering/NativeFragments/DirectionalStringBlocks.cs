using System;
namespace ScoreModern.RecoveredFragments {
 public interface IStringBlockMemory {
  byte ReadByte(ushort segment,ushort offset);
  ushort ReadWord(ushort segment,ushort offset);
  void WriteByte(ushort segment,ushort offset,byte value);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public struct StringCopyResult {
  public readonly ushort SI,DI,CX;
  public StringCopyResult(ushort si,ushort di){SI=si;DI=di;CX=0;}
 }
 public struct StringFillResult {
  public readonly ushort DI,CX;
  public StringFillResult(ushort di){DI=di;CX=0;}
 }
 public static class DirectionalStringBlocks {
  // Original 1F08:0115..011B. Preserve word transfers and their order;
  // overlapping buffers must not be replaced by host memmove semantics.
  public static StringCopyResult Copy(IStringBlockMemory m,ushort ds,ushort si,ushort es,ushort di,ushort count,bool directionFlag){
   if(m==null)throw new ArgumentNullException("m");
   int step=directionFlag?-2:2;
   for(int words=count>>1;words!=0;words--){
    ushort value=m.ReadWord(ds,si);m.WriteWord(es,di,value);
    si=unchecked((ushort)(si+step));di=unchecked((ushort)(di+step));
   }
   if((count&1)!=0){
    byte value=m.ReadByte(ds,si);m.WriteByte(es,di,value);
    int delta=directionFlag?-1:1;si=unchecked((ushort)(si+delta));di=unchecked((ushort)(di+delta));
   }
   return new StringCopyResult(si,di);
  }
  // Original 1F08:00EF..00F5. AX supplies each word and AL the odd byte.
  public static StringFillResult Fill(IStringBlockMemory m,ushort es,ushort di,ushort ax,ushort count,bool directionFlag){
   if(m==null)throw new ArgumentNullException("m");
   int step=directionFlag?-2:2;
   for(int words=count>>1;words!=0;words--){m.WriteWord(es,di,ax);di=unchecked((ushort)(di+step));}
   if((count&1)!=0){m.WriteByte(es,di,(byte)ax);di=unchecked((ushort)(di+(directionFlag?-1:1)));}
   return new StringFillResult(di);
  }
 }
}
