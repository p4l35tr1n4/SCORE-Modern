using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativePagePartitionResult {
  internal List<Item> Line,Remainder;internal float End;
 }
 internal static class NativePagePartition {
  // PAGE 2F23:0221..04EF, before its separate remainder rebase and affine
  // line preparation. Input is the original sorted working bank.
  internal static NativePagePartitionResult Split(IList<Item> input,float cut){
   if(input==null||input.Count==0||Single.IsNaN(cut)||Single.IsInfinity(cut))throw new ArgumentException("PAGE requires a finite split position and a nonempty bank.");
   var working=input.Select(item=>item.Clone()).ToList();var remainder=new List<Item>();float threshold=(float)((double)cut-2);int stop=-1;
   for(int n=0;n<working.Count;n++){
    var item=working[n];if(item.Code==5&&item.P(6)>=threshold){if(item.Count<6)throw new NotSupportedException("PAGE short crossing-curve records require their original adjacent-word path.");var copy=item.Clone();item.Set(6,cut);copy.Set(3,cut);remainder.Add(copy);}
    if(item.P(3)>cut){stop=n;break;}
   }
   if(stop<1)throw new NotSupportedException("PAGE split has no retained line or no remaining item; the full-bank branch is separate.");
   // 03A2..0495: rewrite the last retained repeat bar and prepend its
   // continuation, after any copied crossing curves and before the tail.
   for(int n=stop-1;n>=0;n--)if(working[n].Code==14){var bar=working[n];if(bar.Count>=5){long value;if(!ScoreModern.RecoveredFragments.FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(bar.P(5)),0),out value))throw new NotSupportedException("Non-finite PAGE repeat controls require the original error path.");short style=unchecked((short)value);if(style>=4&&style<=6){bar.Set(5,style==4?1:3);var continuation=bar.Clone();continuation.Set(5,4);continuation.Set(3,cut);remainder.Add(continuation);}}break;}
   remainder.AddRange(working.Skip(stop));
   return new NativePagePartitionResult{Line=working.Take(stop).ToList(),Remainder=remainder,End=cut};
  }
  // Exact-file boundary branch 2F23:0592..0607 scans raw signed high/low
  // words, with zero as the initial maximum; it does not use the bar +.01.
  internal static NativePagePartitionResult TakeAll(IList<Item> input){int maximum=0;foreach(var item in input){float x=item.P(3);if(Single.IsNaN(x)||Single.IsInfinity(x))throw new NotSupportedException("Non-finite PAGE end coordinates require the original error-state path.");int bits=BitConverter.ToInt32(BitConverter.GetBytes(x),0);if(bits>maximum)maximum=bits;}return new NativePagePartitionResult{Line=input.Select(item=>item.Clone()).ToList(),Remainder=new List<Item>(),End=BitConverter.ToSingle(BitConverter.GetBytes(maximum),0)};}
 }
}
