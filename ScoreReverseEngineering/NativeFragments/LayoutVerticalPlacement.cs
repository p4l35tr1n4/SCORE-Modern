using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutVerticalPlacementResult {public short Bottom,Top;public bool Repeat;public readonly float[] Offsets=new float[33];}
 public static class LayoutVerticalPlacement {
  static long Integer(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value< -9223372036854775808d||value>=9223372036854775808d)throw new NotSupportedException("Original VJ conversion leaves its finite integer domain.");return (long)Math.Truncate(value);}
  public static LayoutVerticalPlacementResult Apply(short[][] minimum,short[][] maximum,bool[] occupied,float[] scales,float[] bases,short bottom,short top,short verticalSteps,short radius,float staffGap,float[] retainedOffsets){
   if(bottom<1||top>32||top<bottom||scales.Length<33||bases.Length<33||occupied.Length<33||retainedOffsets.Length<33)throw new NotSupportedException("Original VJ staff traversal leaves its tables.");
   int first=999,last=0;for(int i=1;i<=32;i++)if(occupied[i]){first=Math.Min(first,i);last=Math.Max(last,i);}bottom=(short)Math.Max(bottom,first);top=(short)Math.Min(top,last);var result=new LayoutVerticalPlacementResult{Bottom=bottom,Top=top};Array.Copy(retainedOffsets,result.Offsets,33);if(bottom>=top)return result;
   int previous=bottom;float cumulative=0;
   for(int staff=bottom+1;staff<=top;staff++)if(occupied[staff]){
    float staffUnit=(float)((double)scales[staff]*7),smallerUnit=(float)((double)Math.Min(scales[staff],scales[previous])*7);int distance=LayoutVerticalCollision.Minimum(minimum[staff],maximum[previous],radius);
    if(distance!=99999){int required=unchecked((int)Integer((double)verticalSteps*smallerUnit));short floor=unchecked((short)Integer((double)staffGap*smallerUnit-((double)bases[staff]-bases[previous]-(double)smallerUnit*8)));int shift=unchecked(required-distance);if(shift<floor)shift=floor;cumulative=(float)((double)cumulative+shift);previous=staff;if(bases[staff]>=32100){cumulative=(float)(32000d-bases[staff]);result.Repeat=true;}}
    double delta=(double)cumulative/staffUnit;result.Offsets[staff]=Math.Abs(delta)<0.20000000298023224?0:(float)delta;
   }
   Array.Copy(result.Offsets,retainedOffsets,33);return result;
  }
 }
}
