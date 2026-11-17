using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2F12:00C2..0300 and 0368..040F. The metric/spacing caller supplies
 // new tables; this routine merges them in original order with float32 stores.
 public static class LayoutJustificationMerge {
  static void Bound(int index,float[] a){if(index<0||index>=a.Length)throw new NotSupportedException("Original justification merge would leave its tables.");}
  public static short Append(float[] oldPositions,float[] oldMinimum,float[] newPositions,float[] newMinimum,float[] mergedPositions,float[] mergedMinimum,short oldCount,short newCount,float right,float rightThreshold,float epsilon) {
   if(newCount==0)return oldCount;
   if(oldCount<1||newCount<1)throw new NotSupportedException("Invalid original justification table count.");
   Bound(newCount-1,newPositions);
   if((double)right-newPositions[newCount-1]>epsilon){Bound(newCount,newPositions);newPositions[newCount++]=right;}
   if(oldCount==1){Array.Copy(newPositions,oldPositions,newCount);Array.Copy(newMinimum,oldMinimum,newCount);return newCount;}
   int nextNew=1,nextOld=1;short output=1;
   for(int steps=0;steps<400;steps++) {
    Bound(nextNew,newPositions);Bound(nextOld,oldPositions);Bound(nextNew-1,newMinimum);Bound(nextOld-1,oldMinimum);
    float n=newPositions[nextNew],o=oldPositions[nextOld];
    if(Math.Abs((double)n-o)<=epsilon) {
     float position=o<n?o:n;float gap=oldMinimum[nextOld-1];if(gap<newMinimum[nextNew-1])gap=newMinimum[nextNew-1];
     Bound(output-1,mergedMinimum);mergedMinimum[output-1]=gap;output++;Bound(output-1,mergedPositions);mergedPositions[output-1]=position;
     if(rightThreshold<=position){Array.Copy(mergedPositions,oldPositions,output);Array.Copy(mergedMinimum,oldMinimum,output);return output;}
     nextOld++;nextNew++;
    } else if(n<o) {
     var r=LayoutJustificationAllocation.Apply(mergedPositions,mergedMinimum,output,n,o,oldMinimum[nextOld-1],newMinimum[nextNew-1]);
     output=r.NextIndex;oldMinimum[nextOld-1]=r.Remaining;nextNew++;
    } else {
     var r=LayoutJustificationAllocation.Apply(mergedPositions,mergedMinimum,output,o,n,newMinimum[nextNew-1],oldMinimum[nextOld-1]);
     output=r.NextIndex;newMinimum[nextNew-1]=r.Remaining;nextOld++;
    }
   }
   throw new NotSupportedException("Original justification merge exceeded its finite table traversal.");
  }
 }
}
