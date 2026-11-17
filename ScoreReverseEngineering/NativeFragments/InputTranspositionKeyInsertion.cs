using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // 32AC:0756..0905. Appends the key; does not rebuild the collector table.
 // Constants measured in DS:7E1C,7E22..7E32. Caller owns capacity checks,
 // prompt choices, subsequent sorting and engine key-override state.
 public static class InputTranspositionKeyInsertion {
  // Parent 2F87:030D..0368: K forces the question, not an insertion.
  public static bool ShouldPrompt(bool forceQuestion,short firstKeyState,short semitoneClass){return forceQuestion||(firstKeyState!=0&&semitoneClass!=0);}
  public static bool ShouldInsert(bool answerYes,short firstKeyState,short semitoneClass){return answerYes&&firstKeyState!=0&&semitoneClass!=0;}
  static short Word(float value){long v;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out v))throw new NotSupportedException("Non-finite key-insertion parameter.");return unchecked((short)v);}
  public static float[] Append(IList<float[]> rows,float staff,short key,short clef){
   short selected=Word(staff);float position=999f;bool placed=false;
   // Original scans only through count-1 so a clef has a following record.
   for(int i=0;i<rows.Count-1;i++){
    var row=rows[i];if(row.Length<3)throw new NotSupportedException("Key-insertion scan lacks the original reader fields.");
    if(Word(row[1])!=selected)continue;
    if(row[2]<position)position=row[2];
    if(Word(row[0])!=3)continue;
    position=row[2];var next=rows[i+1];
    if(next.Length>=3)position=(float)(((double)next[2]-position)/2f+position);
    placed=true;break;
   }
   if(!placed)position=(float)((double)position+1f);
   var inserted=new float[]{17f,staff,position,0f,key,clef};rows.Add(inserted);return inserted;
  }
 }
}
