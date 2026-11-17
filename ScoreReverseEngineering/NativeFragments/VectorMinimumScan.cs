using System;
namespace ScoreModern.RecoveredFragments {
 public static class VectorMinimumScan {
  // Minimum-only projection of relative 27A1:05C8..0667. Buffer header is
  // the next X index; X[1] pairs with Y[0]. Other bounding-box fields, flags,
  // record preparation and the drawing call are outside this fragment.
  public static short Accumulate(short minimum,short[] xBuffer,short[] yBuffer) {
   if(xBuffer==null||yBuffer==null||xBuffer.Length==0)throw new ArgumentException("Missing vector buffers.");
   int next=unchecked((ushort)xBuffer[0]);
   if(next<1||next>32767||xBuffer.Length<next||yBuffer.Length<next-1)throw new NotSupportedException("Outside the bounded positive buffer-count domain.");
   for(int index=1;index<next;index++) {
    if(xBuffer[index]<-32600)continue;
    short y=yBuffer[index-1];if(y<minimum)minimum=y;
   }
   return minimum;
  }
 }
}
