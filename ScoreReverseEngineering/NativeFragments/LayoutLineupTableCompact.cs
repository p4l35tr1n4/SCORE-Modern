using System;
namespace ScoreModern.RecoveredFragments {
 // Original 31FE:0AF4..0B94. Equal adjacent -14 bar tags collapse;
 // other repeated values are retained without numeric conversion.
 public static class LayoutLineupTableCompact {
  public static short Apply(float[] source,float[] destination,short first,short last,float retainedTag) {
   if(first<1||last>source.Length)throw new NotSupportedException("Original lineup compaction leaves its source table.");
   int count=0;float previous=0;
   for(int i=first-1;i<last;i++){
    float value=source[i];int bits=BitConverter.ToInt32(BitConverter.GetBytes(value),0);
    if(bits==BitConverter.ToInt32(BitConverter.GetBytes(previous),0)&&bits==BitConverter.ToInt32(BitConverter.GetBytes(retainedTag),0))continue;
    if(count>=destination.Length)throw new NotSupportedException("Original lineup compaction exceeds its destination table.");destination[count++]=value;previous=value;
   }
   return (short)count;
  }
 }
}
