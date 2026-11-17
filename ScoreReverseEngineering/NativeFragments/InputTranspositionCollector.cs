using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionCollectorState {
  public float FirstNotePosition;
  public TranspositionCollectorState(float firstNotePosition){FirstNotePosition=firstNotePosition;}
 }
 public sealed class TranspositionCollectorResult {
  public TranspositionCollection FirstPass;public List<float> CollectedBoundaries;
  public TranspositionPartition Partition;public float ExpandedLeft,ExpandedRight;
 }
 // Original 32AC:000C..0750. State is supplied by the caller, not silently
 // reset for key-only/empty selections. Main workflow ownership is separate.
 public static class InputTranspositionCollector {
  public static TranspositionCollectorResult Build(IList<float[]> rows,short staff,float left,float right,short mode,TranspositionCollectorState state){return Build(rows,staff,left,right,mode,state,null);}
  public static TranspositionCollectorResult Build(IList<float[]> rows,short staff,float left,float right,short mode,TranspositionCollectorState state,short[] recoveredAccidentalTable){
   if(state==null)throw new ArgumentNullException("state");
   var result=new TranspositionCollectorResult{ExpandedLeft=(float)((double)left-.001f),ExpandedRight=(float)((double)right+.001f)};
   result.FirstPass=InputTranspositionCollection.FirstPass(rows,staff,result.ExpandedLeft,result.ExpandedRight,mode,recoveredAccidentalTable);
   float first=result.FirstPass.FirstNotePosition??state.FirstNotePosition;
   result.CollectedBoundaries=result.FirstPass.Records.Count==0?new List<float>():InputTranspositionCollection.Boundaries(rows,staff,first,result.ExpandedRight);
   var positions=new List<float>();var kinds=new List<short>();foreach(var record in result.FirstPass.Records){positions.Add(record.Position);kinds.Add(record.Kind);}
   result.Partition=InputTranspositionPartition.Apply(result.CollectedBoundaries,positions,kinds);
   state.FirstNotePosition=first;return result;
  }
 }
}
