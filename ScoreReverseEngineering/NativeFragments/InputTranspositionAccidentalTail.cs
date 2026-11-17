using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionAccidentalTailResult {
  public short[] Flags;public short ActiveRelationships,RelationshipIndex;
  public TranspositionRelationshipWindow Window;
 }
 // Main argument-zero continuation 3195:0822 -> 0C54..10D5. The
 // caller provides every pitch-table word, including retained key/rest slots.
 public static class InputTranspositionAccidentalTail {
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long v;if(!FiniteBinary32Integer64.TryConvert(Bits(f),out v))throw new NotSupportedException("Non-finite accidental parameter.");return unchecked((short)v);}
  static void Clear(IList<float[]> rows,int item){if(item<1||item>rows.Count||rows[item-1].Length<5)throw new NotSupportedException("Accidental record is too short.");rows[item-1][4]=unchecked((short)(Word(rows[item-1][4])/10*10));}
  public static TranspositionAccidentalTailResult Apply(IList<float[]> rows,IList<TranspositionRecord> entries,short[] pitches,IList<int> relationships,short activeCount,float stopPosition,short kindZeroGuard,TranspositionRelationshipWindow initialWindow,float margin,float width){
   int count=entries.Count;if(count==0)throw new NotSupportedException("Empty transposition engine table violates the recovered caller precondition.");
   if(count>200||pitches.Length!=count)throw new NotSupportedException("Accidental tail table dimensions differ.");
   var flags=new short[count];short cursor=0;var window=initialWindow;
   InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
   for(int i=0;i<count;i++){
    var current=entries[i];short kind=current.Kind;
    // 1041 indexes the guard by kind (0/1), not by entry number.
    if(kind<0||kind>1||(kind==0?kindZeroGuard:flags[0])<0)continue;
    float x=current.Position;int prior;
    if(activeCount==0||current.Accidental!=0)goto Forward;
    if(x>stopPosition)goto Right;
   Left:
    if(window.StartLow>x)goto Forward;
    if(!(window.StartHigh<x))goto Forward;
    if(window.EndLow<=x)goto ClearCurrent;
    InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
    if(activeCount==0)goto Forward;
    goto Left;
   Right:
    if(current.Kind!=1)goto Forward;
    if(x<=window.StartHigh)goto Forward;
    if(x<window.EndLow||x>window.EndHigh)goto AdvanceRight;
    prior=i;
   Previous:
    if(--prior<0)goto AdvanceRight;
    if(entries[prior].Kind<0||entries[prior].Kind>1||entries[prior].Clef!=current.Clef)goto AdvanceRight;
    if(Bits(entries[prior].Position)==Bits(x))goto Previous;
    if(pitches[prior]==pitches[i])goto ClearCurrent;
    float previousPosition=entries[prior].Position;
   PreviousChord:
    if(--prior<0||Bits(entries[prior].Position)!=Bits(previousPosition))goto AdvanceRight;
    if(pitches[prior]==pitches[i])goto ClearCurrent;
    goto PreviousChord;
   AdvanceRight:
    InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
    if(activeCount==0)goto Forward;
    goto Right;
   ClearCurrent:
    Clear(rows,current.Item);
    InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
    continue;
   Forward:
    for(int j=i+1;j<count;j++){
     var next=entries[j];if(next.Clef!=current.Clef)break;
     if(next.Kind<0)continue;
     if(next.Kind==1&&Bits(next.Position)!=Bits(x))break;
     if(pitches[j]!=pitches[i])continue;
     if(next.Accidental!=0)break;
     Clear(rows,next.Item);flags[j]=-1;
    }
   }
   return new TranspositionAccidentalTailResult{Flags=flags,ActiveRelationships=activeCount,RelationshipIndex=cursor,Window=window};
  }
 }
}
