namespace ScoreModern.RecoveredFragments {
 // Original 2DC2:1AC7..1C1B and 1C1E..1C56. This is the key/diatonic
 // conversion leaf, not a complete TR command or a note transposer.
 public static class InputTranspositionKey {
  static short W(int value){return unchecked((short)value);}
  public static short KeyPosition(short key){
   short value=key;if(value<0)value=W(value+7);
   if(value%2!=0)value=W(value+7);
   return W(value/2);
  }
  public static void Convert(short semitones,short oldKey,out short diatonic,out short newKey){
   if(semitones==12||semitones==-12){diatonic=semitones==12?(short)7:(short)-7;newKey=oldKey;return;}
   newKey=W(W(oldKey-W(5*semitones))%12);
   if(newKey< -6)newKey=W(newKey+12);if(newKey>6)newKey=W(newKey-12);
   if(newKey==6||newKey==-6)newKey=oldKey<0?(short)-6:(short)6;
   short difference=W(KeyPosition(oldKey)-KeyPosition(newKey));diatonic=difference;
   if(semitones>=0){
    if(difference>0)diatonic=W(7-difference);
    if(difference<0)diatonic=W(-difference);
    if(semitones>10&&diatonic<6){diatonic=W(-difference+7);if(diatonic<6)diatonic=W(diatonic+7);}
   }else{
    if(difference<0)diatonic=W(-7-difference);
    if(difference>0)diatonic=W(-difference);
    if(semitones< -10&&diatonic> -6){diatonic=W(-7-difference);if(diatonic> -6)diatonic=W(diatonic-7);}
   }
  }
 }
}
