using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupWorkspace {
  public readonly float[] Rhythms=new float[150],Positions=new float[151],Merged=new float[150],Scratch=new float[500],Union=new float[150],Subdivisions=new float[150];
  public int MaximumStaff=32;
  public bool StandaloneJust;
  public bool Scor4DurationScale;
  public bool Page4RoundEachPosition;
 }
 public static class LayoutLineupSystem {
  // Original LI arithmetic pipeline, excluding the parent selection/sort/CW
  // pass. Grace/coincidence cleanup follows the original invalidated-sort flag.
  public static LayoutLineupPreparationResult Apply(float[] words,int[] starts,short bottom,short top,float[] staffUnits,float exponent,short staffThreshold,int capacity,LayoutLineupWorkspace work){
   return ApplyWithBeatPolicy(words,starts,bottom,top,staffUnits,exponent,staffThreshold,capacity,work,false);
  }
  public static LayoutLineupPreparationResult ApplyWithBeatPolicy(float[] words,int[] starts,short bottom,short top,float[] staffUnits,float exponent,short staffThreshold,int capacity,LayoutLineupWorkspace work,bool returnUnequalBeats,Action<int> acknowledgeNegativePosition=null){
   // PAGE4 1D4D:00DE–0154 limits P6 of selected staff records before collecting rhythms.
   if(work.Page4RoundEachPosition){
    foreach(int start in starts){
     int index=start-1;
     if((short)words[index]<6||(short)words[index+1]!=8)continue;
     short staff=(short)words[index+2];
     if(staff>=bottom&&staff<=top&&words[index+6]>200f)words[index+6]=200f;
    }
   }
   // JUST4 at file offset 925D and PAGE4 at 15974 load 4.0 before
   // multiplying a positive duration. The classic path uses 2.0.
   float rhythmScale=work.Scor4DurationScale?4f:2f;
   var result=LayoutLineupPreparation.ApplyWithRhythmScale(words,starts,bottom,top,work.Rhythms,work.Positions,work.Merged,work.Scratch,work.Union,work.Subdivisions,returnUnequalBeats,acknowledgeNegativePosition,work.StandaloneJust,rhythmScale);
   if(result.Bottom<0||result.UnequalBeats)return result;
   LayoutLineupCoordinates.Apply(work.Merged,work.Rhythms,work.Positions,result.Count,result.Left,result.Right,staffUnits[result.Bottom],exponent,work.Page4RoundEachPosition);
   int staffLimit=work.MaximumStaff;
   var historyRhythms=new float[2000];var historyPositions=new float[2000];var staffStarts=new short[staffLimit+2];int total=0;
   for(int staff=result.Bottom;staff<=result.Top;staff++){
    if(staff<1||staff>staffLimit)throw new NotSupportedException("Original lineup staff-history scope leaves its table.");staffStarts[staff]=(short)(total+1);
    foreach(var stream in result.StaffStreams)if(stream.Staff==staff){if(total+stream.Rhythms.Length>historyRhythms.Length)throw new NotSupportedException("Original lineup saved-stream bank capacity exceeded.");Array.Copy(stream.Rhythms,0,historyRhythms,total,stream.Rhythms.Length);Array.Copy(stream.Positions,0,historyPositions,total,stream.Positions.Length);total+=stream.Rhythms.Length;break;}
   }
   staffStarts[result.Top+1]=(short)(total+1);
   LayoutLineupRemap.Apply(words,starts,result.Bottom,result.Top,work.Merged,work.Rhythms,result.Count,historyRhythms,historyPositions,staffStarts,staffUnits,staffThreshold,capacity);
   if(result.NeedsCleanup)LayoutLineupCleanup.Apply(words,starts,result.Bottom,result.Top,staffUnits);
   return result;
  }
 }
}
