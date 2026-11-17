using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class JustificationGapResult {
  public float MinimumTotal, Width, DeficitPerGap, PositiveSlack, NegativeSlack;
  public bool CompressedMinimums;
 }
 // Original 2F68:000D..01D3. This prepares gaps; it neither selects records
 // nor applies coordinates. Array cells correspond to the original far-pointer
 // arguments at BP+06 (slack), +0A (minimum), and +0E (positions).
 public static class LayoutJustificationGaps {
  static bool SignBit(float value) { return BitConverter.ToInt32(BitConverter.GetBytes(value),0)<0; }
  public static JustificationGapResult Prepare(float[] slack,float[] minimum,float[] positions,short pointCount,float left,float right) {
   if(slack==null||minimum==null||positions==null||pointCount<0||pointCount>positions.Length||pointCount-1>minimum.Length||pointCount-1>slack.Length)
    throw new NotSupportedException("Original justification gap preparation would leave its buffers.");
   // The original signed loop bounds also accept zero points: no gap
   // iterations or coordinate writes occur (measured by a staff-only J).
   int gaps=pointCount-1;float sum=0;
   for(int i=0;i<gaps;i++)sum=(float)((double)sum+minimum[i]);
   double extendedWidth=(double)right-left;float width=(float)extendedWidth;
   bool compressed=extendedWidth<sum;float deficit=0;
   if(compressed) {
    if(gaps==0)throw new NotSupportedException("Original justification gap preparation divides by zero.");
    deficit=(float)(((double)sum-width)/gaps);
    for(int i=0;i<gaps;i++)minimum[i]=(float)((double)minimum[i]-deficit);
   }
   for(int i=0;i<gaps;i++)slack[i]=(float)(((double)positions[i+1]-positions[i])-minimum[i]);
   float positive=0,negative=0;
   for(int i=0;i<gaps;i++) {
    if(SignBit(slack[i]))negative=(float)((double)negative-slack[i]);
    else positive=(float)((double)positive+slack[i]);
   }
   return new JustificationGapResult{MinimumTotal=sum,Width=width,DeficitPerGap=deficit,CompressedMinimums=compressed,PositiveSlack=positive,NegativeSlack=negative};
  }
 }
}
