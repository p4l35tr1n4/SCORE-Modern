using System;
namespace ScoreModern.RecoveredFragments {
 public static class ScanP8Conversion {
  // Relative 27A1:057D..05B8, finite numerical projection. The original
  // multiplies two binary32 operands without an intervening binary32 store,
  // calls its truncating integer helper, and stores the returned low DWORD.
  // The caller supplies the measured factor; x87 flags/stack effects and
  // exceptional conversion delivery are outside this API.
  public static int Convert(uint sourceBits,float factor) {
   float source=BitConverter.ToSingle(BitConverter.GetBytes(sourceBits),0);
   if(Single.IsNaN(source)||Single.IsInfinity(source)||Single.IsNaN(factor)||Single.IsInfinity(factor))throw new NotSupportedException("Requires finite source and factor.");
   // A binary32 product has at most 48 significant bits, representable
   // exactly in binary64 across the binary32 exponent range.
   double product=(double)source*factor;
   if(product < -9223372036854775808.0 || product >= 9223372036854775808.0)throw new NotSupportedException("Outside the verified signed-64-bit conversion projection.");
   return unchecked((int)(long)Math.Truncate(product));
  }
 }
}
