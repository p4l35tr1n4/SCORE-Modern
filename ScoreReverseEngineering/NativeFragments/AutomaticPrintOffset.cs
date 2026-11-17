namespace ScoreModern.RecoveredFragments {
 // Original relative 27A1:06E3..06F2 and 2608:10DF..112A.
 // This fragment consumes an already measured vector minimum. It does not
 // establish which drawing requests belong in the original minimum scan.
 public static class AutomaticPrintOffset {
  public static int FromMinimumWord(short minimumY) {
   return unchecked((short)(-24000-minimumY));
  }
  public static int SelectBottom(int preliminary) {
   return preliminary>2200 ? unchecked(preliminary+800) : 3000;
  }
 }
}
