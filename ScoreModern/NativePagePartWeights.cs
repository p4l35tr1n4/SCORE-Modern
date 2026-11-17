using System;
using System.Collections.Generic;
using System.Linq;
namespace ScoreModern {
 internal sealed class NativePagePartWeights {internal short[] Widths,Leading;internal NativePagePartPlan BarPlan;}
 internal static class NativePagePartWeightCollection {
  static void F(byte[] d,int at,float value){Buffer.BlockCopy(BitConverter.GetBytes(value),0,d,at,4);}
  static void W(byte[] d,int at,int value){Buffer.BlockCopy(BitConverter.GetBytes(unchecked((short)value)),0,d,at,2);}
  // Explicit fresh native workspace. Raw source-history replays remain a
  // separate entry; no captured original memory is used by this producer.
  internal static NativePagePartWeights Prepare(IList<Score> parts){
   var plan=NativePageBarPlan.Prepare(parts);var widths=new List<short>();
   var leading=new short[parts.Sum(part=>part.Items.Count(item=>item.Code==14))+1];
   var trailing=new List<short>();var workspace=new byte[65536];int at=0;float scale=0;
   var counts=new int[parts.Count];var positions=new List<float>();var consumed=new List<float>();
   for(int file=0;file<parts.Count;file++){var part=parts[file];var rhythm=NativePageBarPlan.Collect(NativePageSort.Apply(part.Items,true),1,1);float[] durations=plan.Durations.Skip(at).Take(plan.FileBars[file]).ToArray();
    F(workspace,0x779c,1);W(workspace,0x7772,rhythm.Cursor);W(workspace,0x7794,rhythm.Rhythms.Length);F(workspace,0x7752,rhythm.Total);
    for(int n=0;n<durations.Length;n++)F(workspace,0x77a6+n*4,durations[n]);
    // A rest can share the preceding meter's rhythmic slot. The spacing
    // loop still consumes a duration for each drawn bar from its work bank.
    int capacity=part.Items.Count(item=>item.Code==14)+1;
    if(0x77a6+capacity*4>workspace.Length)throw new NotSupportedException("PAGE part duration bank exceeded.");
    var spacingDurations=new float[capacity];
    Buffer.BlockCopy(workspace,0x77a6,spacingDurations,0,capacity*4);
    var result=NativePageSpacingPlan.Collect(rhythm.Items,spacingDurations,scale,null,workspace);scale=result.FinalMultiplier;
    counts[file]=result.Widths.Length;
    positions.AddRange(counts[file]==durations.Length?plan.Positions.Skip(at).Take(counts[file]):result.Positions);
    consumed.AddRange(spacingDurations.Take(counts[file]));
    int barAt=widths.Count;
    for(int n=0;n<result.Leading.Length;n++)if(result.LeadingWritten[n])leading[barAt+n]=unchecked((short)result.Leading[n]);
    widths.AddRange(result.Widths.Select(x=>unchecked((short)x)));trailing.AddRange(result.Trailing.Select(x=>unchecked((short)x)));at+=durations.Length;
   }
   var combined=new short[widths.Count];for(int n=0;n<combined.Length;n++)combined[n]=unchecked((short)Math.Min((int)unchecked((short)(leading[n+1]+trailing[n])),9000));
   return new NativePagePartWeights{Widths=widths.ToArray(),Leading=combined,
    BarPlan=new NativePagePartPlan{FileBars=counts,Positions=positions.ToArray(),Durations=consumed.ToArray()}};
  }
 }
}
