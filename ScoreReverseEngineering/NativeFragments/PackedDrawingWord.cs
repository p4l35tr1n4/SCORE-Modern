namespace ScoreModern.RecoveredFragments {
 public struct PackedDrawingParts { public short High,Middle,Low,FinalAX; }
 public static class PackedDrawingWord {
  // Relative 14A6:0E9C..0EC6 integer/data projection only. Signed IDIV
  // truncates toward zero. Names denote numeric positions, not symbol meanings.
  // No flags, x87 transforms, table bounds or drawing policy are supplied.
  public static PackedDrawingParts Decode(short word) {
   int high=word/10000;
   int remainder=word-high*10000;
   return new PackedDrawingParts {High=(short)high,Middle=(short)(remainder/100),
    Low=(short)(word-(word/100)*100),FinalAX=(short)(word/100)};
  }
 }
}
