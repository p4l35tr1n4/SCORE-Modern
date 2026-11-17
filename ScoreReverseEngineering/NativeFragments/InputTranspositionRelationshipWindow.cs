using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct TranspositionRelationshipWindow {public float StartLow,StartHigh,EndLow,EndHigh;}
 // Original 3195:10D8..1179. FST leaves the extended intermediate on
 // the x87 stack: the high endpoint is not calculated from the rounded low.
 public static class InputTranspositionRelationshipWindow {
  public static void Advance(IList<float[]> rows,IList<int> itemNumbers,ref short activeCount,ref short index,ref TranspositionRelationshipWindow window,float margin,float width){
   if(index>=activeCount){activeCount=0;return;}
   if(index<0||activeCount>itemNumbers.Count)throw new NotSupportedException("Relationship cursor outside recorded table.");
   index=unchecked((short)(index+1));int item=itemNumbers[index-1]-1;
   if(item<0||item>=rows.Count||rows[item].Length<6)throw new NotSupportedException("Relationship points outside its parameter record.");
   var row=rows[item];double end=(double)row[5]-margin,start=(double)row[2]-margin;
   window.EndLow=(float)end;window.EndHigh=(float)(end+width);
   window.StartLow=(float)start;window.StartHigh=(float)(start+width);
  }
 }
}
