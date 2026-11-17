using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativePageCollection {
  readonly List<Item> records=new List<Item>();
  internal float Offset {get;private set;}
  internal void Retain(IList<Item> remainder,float retainedOffset){records.Clear();records.AddRange(remainder.Select(item=>item.Clone()));Offset=retainedOffset;}
  // PAGE 2F23:00B7..0199. A nonempty destination advances the retained
  // offset by the source literal 200, then translates only the collector's
  // references in [-10,220]. Raw record count words are copied unchanged.
  internal void Append(IList<Item> input){
   input=NativePageSort.Apply(input,false);
   if(records.Count==0)records.AddRange(input.Select(item=>item.Clone()));
   else{var bank=NativePageCoordinates.Collect(input,-10,220);Offset=(float)((double)Offset+200);foreach(short reference in bank.References){int index=Math.Abs((int)reference)-1;bank.Words[index]=(float)((double)bank.Words[index]+Offset);}records.AddRange(bank.Items());}
   if(records.Count>2800||records.Sum(item=>item.Count+1)>14200)throw new NotSupportedException("PAGE collection exceeds its original item or parameter bank.");
  }
  // 0CC8:02E2 rebuilds record indices, without sorting. In particular a
  // preceding bar can remain just to the right of the next file's staff.
  internal List<Item> Records(){return records.Select(item=>item.Clone()).ToList();}
 }
}
