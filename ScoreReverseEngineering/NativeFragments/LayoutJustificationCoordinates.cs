using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2F68:0447 and 0607. The signed list contains one-based float-word
 // references, NOT item numbers. Updates intentionally occur in list order.
 // Callers own collection and range selection; no renderer bounds are used.
 public sealed class JustificationCoordinates {
  readonly float[] words;readonly short[] references;
  readonly float ratioThreshold,ratioReplacement;
  readonly bool roundScaleNumerator;
  public float LowerBound;
  public JustificationCoordinates(float[] words,short[] references,float lowerBound,float ratioThreshold,float ratioReplacement)
   :this(words,references,lowerBound,ratioThreshold,ratioReplacement,false){}
  internal JustificationCoordinates(float[] words,short[] references,float lowerBound,float ratioThreshold,float ratioReplacement,bool roundScaleNumerator) {
   this.words=words;this.references=references;LowerBound=lowerBound;
   this.ratioThreshold=ratioThreshold;this.ratioReplacement=ratioReplacement;
   this.roundScaleNumerator=roundScaleNumerator;
   foreach(short reference in references)if(reference!=0&&(reference==Int16.MinValue||Math.Abs((int)reference)>words.Length))throw new NotSupportedException("Original justification coordinate reference leaves its buffer.");
  }
  // 0607: remaining references at/above the threshold, without consuming them.
  public void Shift(float threshold,float delta) {
   foreach(short reference in references)if(reference!=0) {
    int i=Math.Abs((int)reference)-1;float x=words[i];
    if(threshold<=x)words[i]=(float)((double)x+delta);
   }
  }
  // 0447: consume references inside the inclusive interval after scaling.
  public void Scale(float left,float right,float delta) {
   float span=(float)((double)right-left);
   if(BitConverter.ToInt32(BitConverter.GetBytes(span),0)==0)return; // original diagnostic then return
   // FST AFB0 retains the extended sum for the following division.
   double numerator=(double)span+delta;
   if(roundScaleNumerator)numerator=(float)numerator;
   double extendedRatio=numerator/span;
   float ratio=(float)extendedRatio;
   if((roundScaleNumerator?ratio:extendedRatio)<=ratioThreshold)ratio=ratioReplacement;
   for(int n=0;n<references.Length;n++)if(references[n]!=0) {
    int i=Math.Abs((int)references[n])-1;float x=words[i];
    if(x<left||x>right)continue;
    double value=((double)x-left)*ratio+left;
    if(LowerBound>value)value=LowerBound;
    words[i]=(float)value;references[n]=0;
   }
   LowerBound=(float)((double)left+ratioThreshold);
  }
 }
 public static class LayoutJustificationRedistribution {
  static bool NonZeroBits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0)!=0;}
  // Original 2F68:01D4..0339. Gap preparation is a preceding routine span;
  // final item-coordinate snapping is deliberately left to the record caller.
  public static bool Apply(float[] slack,float[] positions,short pointCount,float positiveSlack,float negativeSlack,JustificationCoordinates coordinates) {
   if(pointCount<0||pointCount>positions.Length||pointCount-1>slack.Length)throw new NotSupportedException("Original justification redistribution would leave its buffers.");
   if(!NonZeroBits(negativeSlack))return true;
   if(!NonZeroBits(positiveSlack))return false; // original emits diagnostic and skips redistribution
   float ratio=(float)((double)negativeSlack/positiveSlack);
   for(int i=1;i<pointCount;i++) {
    float delta=-slack[i-1];
    if(BitConverter.ToInt32(BitConverter.GetBytes(delta),0)<0)delta=(float)((double)delta*ratio);
    if(!NonZeroBits(delta))continue;
    if(delta<=0) {
     coordinates.Scale(positions[i-1],positions[i],delta);
     coordinates.Shift(positions[i],delta);
     for(int j=i;j<pointCount;j++)positions[j]=(float)((double)positions[j]+delta);
    } else {
     float saved=positions[i];coordinates.Shift(saved,delta);
     for(int j=i;j<pointCount;j++)positions[j]=(float)((double)positions[j]+delta);
     coordinates.Scale(positions[i-1],saved,delta);
    }
   }
   return true;
  }
 }
}
