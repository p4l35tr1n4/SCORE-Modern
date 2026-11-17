using System;
using System.Collections.Generic;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal sealed class NativePageRhythmPlan {
  internal List<Item> Items;internal float[] Rhythms,Positions;internal short Cursor;internal float Total;
 }
 internal sealed class NativePagePartPlan {
  internal int[] FileBars;internal float[] Positions,Durations;
 }
 internal static class NativePageBarPlan {
  // 0D29:007B..0133 groups the rhythmic stream at -14/-140. The planner
  // runs this on a working copy; later assembly rereads the untouched TMP.
  internal static NativePagePartPlan Prepare(IList<Score> parts){
   var positions=new List<float>();var durations=new List<float>();var counts=new int[parts.Count];
   for(int file=0;file<parts.Count;file++){var rhythm=Collect(NativePageSort.Apply(parts[file].Items,true),1,1);float duration=0;
    for(int n=0;n<rhythm.Rhythms.Length;n++){float value=rhythm.Rhythms[n];if(value>=0){duration=(float)((double)duration+value);continue;}
     if(value!=-14&&value!=-140||BitConverter.ToInt32(BitConverter.GetBytes(duration),0)==0)continue;
     positions.Add(rhythm.Positions[n]);durations.Add(value==-140?-duration:duration);counts[file]++;duration=0;
    }
    if(counts[file]==0)throw new NotSupportedException("PAGE part has no measured rhythmic bar plan.");
   }
   return new NativePagePartPlan{FileBars=counts,Positions=positions.ToArray(),Durations=durations.ToArray()};
  }
  // General PAGE Layout scans the composite system for spacing positions,
  // even though the grouped durations come from staff 1 (0D29:0033..088E).
  // Keep this separate from the extracted-Parts planner.
  internal static NativePageSpacingResult PrepareLayoutFile(Score score){
   var rhythm=Collect(NativePageSort.Apply(score.Items,true),1,1);
   var grouped=Prepare(new[]{score}).Durations;
   int capacity=score.Items.Count(i=>i.Code==14)+1;
   var durations=new float[Math.Max(grouped.Length,capacity)];
   if(0x77a6+durations.Length*4>65536)throw new NotSupportedException("PAGE layout duration bank exceeded.");
   Array.Copy(grouped,durations,grouped.Length);
   var work=new byte[65536];
   Buffer.BlockCopy(BitConverter.GetBytes(1f),0,work,0x779c,4);
   Buffer.BlockCopy(BitConverter.GetBytes(rhythm.Cursor),0,work,0x7772,2);
   Buffer.BlockCopy(BitConverter.GetBytes(checked((short)rhythm.Rhythms.Length)),0,work,0x7794,2);
   Buffer.BlockCopy(BitConverter.GetBytes(rhythm.Total),0,work,0x7752,4);
   Buffer.BlockCopy(durations,0,work,0x77a6,4*durations.Length);
   return NativePageSpacingPlan.Collect(rhythm.Items,durations,0,null,work);
  }
  // PAGE 21A8:000D is the utility's rhythmic collector. This entry keeps
  // its record mutations and all four outputs explicit for comparison.
  internal static NativePageRhythmPlan Collect(IList<Item> input,float staff,short cursor){
   var words=new float[14200];var starts=new int[input.Count];int at=0;
   for(int n=0;n<input.Count;n++){var item=input[n];if(at+item.Count+1>words.Length)throw new NotSupportedException("PAGE rhythmic input exceeds its original parameter bank.");starts[n]=at+1;Buffer.BlockCopy(item.Bytes,0,words,at*4,item.Bytes.Length);at+=item.Count+1;}
   var rhythms=new float[2800];var positions=new float[2800];var scratch=new float[2800];var result=LayoutRhythmicCollection.Collect(words,starts,staff,0,cursor,rhythms,positions,scratch);var rows=new List<Item>();
   for(int n=0;n<input.Count;n++){var raw=new byte[input[n].Bytes.Length];Buffer.BlockCopy(words,(starts[n]-1)*4,raw,0,raw.Length);rows.Add(new Item(raw));}
   return new NativePageRhythmPlan{Items=rows,Rhythms=rhythms.Take(result.Count).ToArray(),Positions=positions.Take(result.Count).ToArray(),Cursor=result.NextItem,Total=result.Total};
  }
  // PAGE 0B45:00A1..00DC, measured before/after each first write and after
  // the complete loop. These are pre-serialization plan positions, rather
  // than the slightly different bar positions read back from TMP files.
  internal static float[] CutPositions(IList<float> positions){
   var result=new float[positions.Count];for(int n=0;n<result.Length;n++){
    if(Single.IsNaN(positions[n])||Single.IsInfinity(positions[n]))throw new NotSupportedException("Non-finite PAGE plan positions require the original error path.");
    result[n]=(float)((double)positions[n]+0.009999999776482582);
   }return result;
  }
 }
}
