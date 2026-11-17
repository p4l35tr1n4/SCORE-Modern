using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputListExpansion {
  // Overlay 8 section 64, 3763:0158..3783:00d7. A repeated span
  // counts positive/ordinary entries; negative chord members belong to
  // their preceding entry. The repeat count includes the existing span.
  // Measured single/double-dot expansion: select the earlier note/chord
  // before the immediately preceding item; pass over intervening non-pitches.
  // Native hypotheses beyond the recorded contexts remain in the F2 ledger.
  public static void RepeatBeforePrevious(List<float> tokens,bool keepAccidentals){
   if(tokens==null)throw new ArgumentNullException("tokens");
   int prior=tokens.Count-1;
   while(prior>=0&&ChordMember(tokens[prior]))prior--;
   int first=prior-1;
   while(first>=0){int kind=(int)Math.Abs(tokens[first])/1000;if(tokens[first]>0&&(kind==1||kind==7))break;first--;}
   if(first<0)throw new NotSupportedException("Dot repetition has no verified earlier note or chord.");
   int stop=first+1;while(stop<tokens.Count&&ChordMember(tokens[stop]))stop++;
   var span=tokens.GetRange(first,stop-first);
   if((long)tokens.Count+span.Count>short.MaxValue)throw new NotSupportedException("Expanded input exceeds the signed 16-bit list representation");
   foreach(float original in span){float value=original;if(!keepAccidentals){float remainder=(float)((double)value%100);value=(float)((value<0?-1000:1000)+(double)remainder);}tokens.Add(value);}
  }
  static bool ChordMember(float token){int kind=Math.Abs((int)token)/1000;return token<0&&(kind==1||kind==7);}
  public static void Repeat(List<float> tokens,int entries,int total,bool keepAccidentals){
   if(tokens==null)throw new ArgumentNullException("tokens");
   if(entries<1||total<1)throw new ArgumentOutOfRangeException("entries");
   int first=tokens.Count,seen=0;
   while(first>0){first--;float value=tokens[first];int kind=(int)(value/1000);if(!(value<=0&&(kind==-1||kind==-7)))seen++;if(seen==entries)break;}
   if(seen!=entries)throw new ArgumentException("Repeat extends before the input list");
   float[] span=tokens.GetRange(first,tokens.Count-first).ToArray();
   long count=(long)tokens.Count+(long)span.Length*(total-1);if(count>short.MaxValue)throw new NotSupportedException("Expanded input exceeds the signed 16-bit list representation");
   for(int repetition=1;repetition<total;repetition++)foreach(float original in span){float value=original;int kind=Math.Abs((int)value)/1000;if(!keepAccidentals&&(kind==1||kind==7)){float remainder=(float)((double)value%100);value=(float)((value<0?-kind:kind)*1000+(double)remainder);}tokens.Add(value);}
  }
 }
}
