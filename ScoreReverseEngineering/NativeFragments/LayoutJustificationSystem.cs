using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutJustificationWorkspace {
  public readonly byte[] Spacing=LayoutSpacingWorkspace.Create();
  public bool OriginalQuarterToneWidths=true;
  public float? PositionJoinTolerance;
  public bool StandaloneJust;
  public bool Scor4NumberJustification;
  public bool Page4RoundScaleNumerator;
  public readonly float[] Selected=new float[200],Auxiliary=new float[200],Minimum=new float[200],Positions=new float[200],OldMinimum=new float[200],OldPositions=new float[200],MergedPositions=new float[200],MergedMinimum=new float[200];
  public readonly short[] SelectedReferences=new short[200];
  public void ResetForIsolatedTest(){Array.Clear(Spacing,0,Spacing.Length);Buffer.BlockCopy(LayoutSpacingWorkspace.Create(),0,Spacing,0,Spacing.Length);foreach(var table in new[]{Selected,Auxiliary,Minimum,Positions,OldMinimum,OldPositions,MergedPositions,MergedMinimum})Array.Clear(table,0,table.Length);Array.Clear(SelectedReferences,0,SelectedReferences.Length);}
 }
 public sealed class LayoutJustificationSystemResult {
  public short[] CoordinateReferences;public short PointCount,FirstPoint,SelectionCursor;
  public JustificationGapResult Gaps;public bool RedistributionApplied;
 }
 // Original 2F12:0003..0490 orchestration. Caller sorting, record preparation,
 // prompts, redraw and undo remain separate. Staff scales and collector limits
 // are explicit recovered caller inputs, never inferred from renderer bounds.
 public static class LayoutJustificationSystem {
  public static LayoutJustificationSystemResult Apply(float[] words,int[] starts,short bottom,short top,float left,float right,float[] scales,float[] afterSymbol,short staffTableThreshold,short coordinateCapacity,LayoutJustificationWorkspace work) {
   if(work==null||scales==null||bottom<0||top<bottom||top>=scales.Length)throw new NotSupportedException("Original justification staff traversal leaves its tables.");
   var collection=LayoutCoordinateCollection.Collect(words,starts,bottom,top,left,right,0.001f,1,checked((short)starts.Length),staffTableThreshold,0,200,100,coordinateCapacity);
   work.MergedPositions[0]=left;work.MergedMinimum[0]=0;short oldCount=1,cursor=1;
   var spacing=new LayoutItemSpacing(work.Spacing,words,starts,work.Selected,work.SelectedReferences,work.Auxiliary,work.Minimum,work.Positions);
   float threshold=(float)((double)right-0.2f);
   for(int staff=bottom;staff<=top;staff++) {
    short count=spacing.Apply((short)staff,scales[staff],bottom,ref cursor,left,right,afterSymbol,work.OriginalQuarterToneWidths,work.PositionJoinTolerance,work.StandaloneJust,work.Scor4NumberJustification);
    if(count!=0)oldCount=LayoutJustificationMerge.Append(work.OldPositions,work.OldMinimum,work.Positions,work.Minimum,work.MergedPositions,work.MergedMinimum,oldCount,count,right,threshold,0.1f);
   }
   int first=0;if((double)work.OldPositions[1]-work.OldPositions[0]<=0.1f){first=1;oldCount--;}
   if(oldCount<0||first+oldCount>work.OldPositions.Length)throw new NotSupportedException("Original justification has no finite coordinate intervals.");
   var positions=new float[oldCount];var minimum=new float[oldCount];var slack=new float[oldCount];Array.Copy(work.OldPositions,first,positions,0,oldCount);Array.Copy(work.OldMinimum,first,minimum,0,oldCount);
   var gaps=LayoutJustificationGaps.Prepare(slack,minimum,positions,oldCount,left,right);
   var coordinates=new JustificationCoordinates(words,collection.References,-9999,0.2f,1,work.Page4RoundScaleNumerator);
   bool applied=LayoutJustificationRedistribution.Apply(slack,positions,oldCount,gaps.PositiveSlack,gaps.NegativeSlack,coordinates);
   if(!work.StandaloneJust&&BitConverter.ToInt32(BitConverter.GetBytes(gaps.NegativeSlack),0)!=0&&BitConverter.ToInt32(BitConverter.GetBytes(gaps.PositiveSlack),0)!=0)LayoutJustificationSnap.Apply(words,starts,0.01f,200);
   Array.Copy(positions,0,work.OldPositions,first,oldCount);Array.Copy(minimum,0,work.OldMinimum,first,oldCount);Array.Copy(slack,work.Selected,slack.Length);
   return new LayoutJustificationSystemResult{CoordinateReferences=collection.References,PointCount=oldCount,FirstPoint=(short)(first+1),SelectionCursor=cursor,Gaps=gaps,RedistributionApplied=applied};
  }
 }
}
