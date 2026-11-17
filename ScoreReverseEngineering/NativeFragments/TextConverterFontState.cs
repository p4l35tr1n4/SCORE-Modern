namespace ScoreModern.RecoveredFragments {
 public struct TextConverterFontState {
  public bool Forced;public short Word;
  // Relative 020F:001D..004C: retain the full hundreds component of the
  // converter's font word. File loading and its failure effects stay external.
  public static TextConverterFontState Begin(short overrideWord,short previousWord){
   return new TextConverterFontState{Forced=overrideWord>=90,
    Word=overrideWord>=90?(short)(overrideWord%1000):previousWord};
  }
  // Single/double-underbar font tokens. 1100 selects outline Times when
  // unforced, but is ignored when a P8 override forces the font.
  // This consumes decoded tokens, not the complete string grammar.
  public void Accept(short token){if(!Forced&&token>=1000&&token<=1199)Word=(short)(token-1000);}
  public int MetricFace{get{return Word%100;}}
 }
}
