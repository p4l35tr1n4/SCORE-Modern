using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionMainState {
  public short Semitones,TargetKey,Diatonic,KeyOverride,KindZeroGuard;
  public float Sentinel,ErasedCode,RelationshipMargin,RelationshipWidth;
  public short[] AccidentalMarkers;
  public TranspositionRelationshipWindow InitialWindow;
 }
 public sealed class TranspositionMainPassResult {
  public TranspositionAccidentalTailResult Tail;public float StopPosition;
 }
 // Main argument word zero only, 3195:000D..10D5. Explicit caller state
 // avoids assigning defaults to the retained bank words. Parent attachment,
 // prompt and normalization passes are outside this component.
 public static class InputTranspositionMainPass {
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short W(int value){return unchecked((short)value);}
  static void ResetMarkers(short[] words){Array.Clear(words,1,199);}
  public static TranspositionMainPassResult Apply(IList<float[]> rows,IList<TranspositionRecord> entries,short[] pitches,IList<int> relationships,TranspositionMainState state){
   if(entries.Count==0||entries.Count>200||pitches.Length!=entries.Count||state.AccidentalMarkers==null||state.AccidentalMarkers.Length!=200)throw new NotSupportedException("Main transposition engine table dimensions violate its caller contract.");
   short oldKey=0;float stop=state.Sentinel;ResetMarkers(state.AccidentalMarkers);
   for(int i=0;i<entries.Count;i++){
    var entry=entries[i];if(entry.Item<1||entry.Item>rows.Count)throw new NotSupportedException("Transposition record index is outside the score.");
    var row=rows[entry.Item-1];
    if(entry.Kind==2){
     var key=InputTranspositionKeyRecord.Apply(row,entry.Accidental,state.Semitones,state.KeyOverride,entry.Clef,W(i+1),state.ErasedCode);
     rows[entry.Item-1]=key.Parameters;oldKey=W((key.OldKeyBase-49)/7);state.TargetKey=W((key.NewKeyBase-49)/7);state.Diatonic=key.Diatonic;state.KeyOverride=key.KeyOverride;ResetMarkers(state.AccidentalMarkers);continue;
    }
    if(entry.Kind<0)continue;
    if(entry.Kind!=0&&Bits(entry.Position)!=Bits(state.Sentinel))ResetMarkers(state.AccidentalMarkers);
    if((Bits(stop)&0x80000000)!=0)stop=entry.Position;
    if(row.Length<5)throw new NotSupportedException("Transposed note lacks its verified parameters.");
    var note=InputTranspositionPitch.Apply(row[3],row[4],pitches[i],entry.Accidental,entry.Clef,oldKey,state.TargetKey,state.Diatonic,state.KeyOverride);
    row[3]=note.Pitch;row[4]=note.Stem;
    if(note.AccidentalMarker.HasValue){int index=note.AccidentalMarker.Value;if(index<0||index>=200)throw new NotSupportedException("Accidental marker leaves its recovered table.");state.AccidentalMarkers[index]=1;}
   }
   var tail=InputTranspositionAccidentalTail.Apply(rows,entries,pitches,relationships,W(relationships.Count),stop,state.KindZeroGuard,state.InitialWindow,state.RelationshipMargin,state.RelationshipWidth);
   return new TranspositionMainPassResult{Tail=tail,StopPosition=stop};
  }
 }
}
