using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupStaffStream {public short Staff;public float[] Rhythms,Positions;}
 public sealed class LayoutLineupBeatTotal {public short Staff;public float Total;}
 public sealed class LayoutLineupPreparationResult {public LayoutLineupBeatTotal[] BeatTotals;public short Bottom,Top,Count;public float Left,Right;public bool NeedsCleanup,UnequalBeats;public LayoutLineupStaffStream[] StaffStreams;}
 // Original 31FE:000B through the 3111:0004 call. The following source
 // remapping and cleanup are deliberately separate; this is not a Main route.
 public static class LayoutLineupPreparation {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup preparation conversion leaves its finite signed-word domain.");return (short)Math.Truncate(f);}
  public static LayoutLineupPreparationResult Apply(float[] words,int[] starts,short bottom,short top,float[] rhythms,float[] positions,float[] merged,float[] scratch,float[] union,float[] subdivisions) {
   return ApplyWithBeatPolicy(words,starts,bottom,top,rhythms,positions,merged,scratch,union,subdivisions,false);
  }
  public static LayoutLineupPreparationResult ApplyWithBeatPolicy(float[] words,int[] starts,short bottom,short top,float[] rhythms,float[] positions,float[] merged,float[] scratch,float[] union,float[] subdivisions,bool returnUnequalBeats,Action<int> acknowledgeNegativePosition=null,bool standaloneJust=false) {
   return ApplyWithRhythmScale(words,starts,bottom,top,rhythms,positions,merged,scratch,union,subdivisions,returnUnequalBeats,acknowledgeNegativePosition,standaloneJust,2f);
  }
  internal static LayoutLineupPreparationResult ApplyWithRhythmScale(float[] words,int[] starts,short bottom,short top,float[] rhythms,float[] positions,float[] merged,float[] scratch,float[] union,float[] subdivisions,bool returnUnequalBeats,Action<int> acknowledgeNegativePosition,bool standaloneJust,float rhythmScale) {
   int cursor=1,mergedCount=0;float left=9999,right=200,previousTotal=0;bool needsCleanup=false;var history=new List<LayoutLineupStaffStream>();var totals=new List<LayoutLineupBeatTotal>();
   for(int staff=bottom;staff<=top;staff++){
    var collected=LayoutRhythmicCollection.Collect(words,starts,staff,-1,(short)cursor,rhythms,positions,scratch,acknowledgeNegativePosition,standaloneJust);cursor=collected.NextItem;int count=collected.Count;needsCleanup|=collected.InvalidatedSort;totals.Add(new LayoutLineupBeatTotal{Staff=(short)staff,Total=collected.Total});
    if(collected.Total==0){if(staff==bottom)bottom++;if(top<bottom){bottom=-1;break;}continue;}
    while(count>0&&positions[count-1]>200)count--;if(count<1)throw new NotSupportedException("Original lineup trims beyond its rhythm table.");
    int lastTag=Word(rhythms[count-1]);
    if(lastTag==-140)rhythms[count-1]=-14;
    else if(lastTag!=-14){
     if(lastTag<0){for(int i=0;i<starts.Length;i++){int at=starts[i]-1,n=Word(words[at]);if(Word(words[at+2])==bottom&&Word(words[at+1])==8){if(n>5)right=words[at+6];break;}}if(Bits(right)==0)right=200;}
     if(count>=rhythms.Length||count>=positions.Length)throw new NotSupportedException("Original lineup end marker exceeds its table.");rhythms[count]=-14;positions[count++]=right;
    }
    int first=1;while(Bits(positions[first-1])<0){first++;if(count<=first)throw new NotSupportedException("Original lineup excludes all selected positions.");count--;}
    int kept=0;bool ordinarySeen=false;
    for(int i=0;i<count;i++){float value=rhythms[i];bool keep=true;
     if(Bits(value)>0){value=(float)((double)rhythmScale*value);ordinarySeen=true;}
     else if(value!=-14){if(value!=-3)ordinarySeen=true;else if(!ordinarySeen){value=-15;ordinarySeen=true;}else keep=false;}
     if(keep){rhythms[kept]=value;positions[kept++]=positions[i];}
    }
    count=kept;for(int i=count;i<150&&i<rhythms.Length;i++)rhythms[i]=0;
    var staffRhythms=new float[count];var staffPositions=new float[count];Array.Copy(rhythms,staffRhythms,count);Array.Copy(positions,staffPositions,count);history.Add(new LayoutLineupStaffStream{Staff=(short)staff,Rhythms=staffRhythms,Positions=staffPositions});
    if(staff==bottom){previousTotal=collected.Total;mergedCount=LayoutLineupTableCompact.Apply(rhythms,merged,(short)first,(short)(first+count-1),-14);for(int i=mergedCount;i<150&&i<merged.Length;i++)merged[i]=0;left=positions[first-1];right=positions[count-1];continue;}
    if(positions[first-1]<left)left=positions[first-1];if(rhythms[count-1]==-14&&right<positions[count-1])right=positions[count-1];
    if(Math.Abs((double)collected.Total-previousTotal)>0.005f){
     if(returnUnequalBeats)return new LayoutLineupPreparationResult{BeatTotals=totals.ToArray(),Bottom=bottom,Top=top,Count=(short)mergedCount,Left=left,Right=right,NeedsCleanup=needsCleanup,StaffStreams=history.ToArray(),UnequalBeats=true};
     throw new NotSupportedException("Original lineup reports unequal beat totals between staves.");
    }
    var combined=LayoutLineupStreamUnion.Apply(rhythms,(short)first,(short)count,merged,(short)mergedCount,union);
    mergedCount=LayoutLineupTableCompact.Apply(union,merged,1,combined.Count,-14);
    int positiveEnd=first-1;for(int i=first-1;i<count;i++)if(Bits(rhythms[i])>0)rhythms[positiveEnd++]=rhythms[i];
    if(positiveEnd>=rhythms.Length||mergedCount>=merged.Length)throw new NotSupportedException("Original lineup sentinel exceeds its table.");rhythms[positiveEnd]=0;merged[mergedCount]=0;
    var groups=LayoutLineupDurationGroups.Apply(rhythms,merged,subdivisions,(short)(first-1),(short)mergedCount,0);mergedCount=groups.OutputCount;Array.Copy(subdivisions,merged,mergedCount);
   }
   return new LayoutLineupPreparationResult{BeatTotals=totals.ToArray(),Bottom=bottom,Top=top,Count=(short)mergedCount,Left=left,Right=right,NeedsCleanup=needsCleanup,StaffStreams=history.ToArray()};
  }
 }
}
