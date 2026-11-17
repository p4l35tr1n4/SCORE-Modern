using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionInterval {public short Semitones,SemitoneClass,Key,Diatonic;}
 // Parent 2F87:019E..0251. Literal DS:4162/4182/419C tables, with
 // signed-word arithmetic and the original single octave increment.
 public static class InputTranspositionInterval {
  static readonly short[] menu={-8,-5,-4,-3,-2,2,3,5,7,8,9,10,14,21,12,-12};
  static readonly short[] keys={-6,1,-4,3,-2,5,0,-5,2,-3,4,-1,6};
  static readonly short[] degrees={0,1,1,2,2,3,3,4,5,5,6,6};
  public static short Menu(short choice){if(choice<1||choice>16)throw new NotSupportedException("Original transposition menu choice must be 1..16.");return menu[choice-1];}
  public static TranspositionInterval Convert(short semitones){
   short keyIndex=unchecked((short)(semitones+7));
   if(keyIndex>13)keyIndex=unchecked((short)(keyIndex-12));
   if(keyIndex>13)keyIndex=unchecked((short)(keyIndex-12));
   if(keyIndex<=0)keyIndex=unchecked((short)(keyIndex+12));
   short absolute=unchecked((short)(semitones<0?-semitones:semitones));
   if(keyIndex<1||keyIndex>keys.Length||absolute<0)throw new NotSupportedException("Original transposition interval leaves its literal lookup tables.");
   short remainder=(short)(absolute%12),diatonic=degrees[remainder];
   if(absolute>=12)diatonic=unchecked((short)(diatonic+7));
   if(semitones<0){remainder=unchecked((short)-remainder);diatonic=unchecked((short)-diatonic);}
   return new TranspositionInterval{Semitones=semitones,SemitoneClass=remainder,Key=keys[keyIndex-1],Diatonic=diatonic};
  }
 }
}
