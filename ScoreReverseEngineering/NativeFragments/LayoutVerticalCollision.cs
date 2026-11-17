using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2C5C:0893..098F. Contours contain signed vector coordinates,
 // with separate empty sentinels. No renderer bounding boxes are consulted.
 public static class LayoutVerticalCollision {
  public static int Minimum(short[] upper,short[] lower,short radius) {
   if(upper==null||lower==null||upper.Length<101||lower.Length<101||radius<0)throw new NotSupportedException("Original VJ contour table/radius leaves the verified domain.");
   int minimum=99999;
   for(int i=0;i<101;i++)if(upper[i]!=32760){int end=Math.Min(100,i+radius),start=Math.Max(0,i-radius);for(int j=start;j<=end;j++)if(lower[j]!=-32760){int gap=(int)upper[i]-lower[j];if(gap<minimum)minimum=gap;}}
   return minimum;
  }
 }
}
