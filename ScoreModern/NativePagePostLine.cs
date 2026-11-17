using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal static class NativePagePostLine {
  // PAGE 084F:0C83..10B2, negative end-mode path used by parts assembly.
  // Positive/automatic end modes call a separate producer at 10B3.
  internal static List<Item> ApplyNegativeEndMode(IList<Item> input,short mode,short outputTag){
   if(mode>=-1)throw new NotSupportedException("PAGE automatic end-item production requires its separate source path.");
   var rows=input.Select(item=>item.Clone()).ToList();
   foreach(var item in rows){
    if(item.Code>=4&&item.Code<=8){if(item.Count<6)continue;if(item.Code==6&&item.Count>=11)for(int p=11;p<=15&&p<=item.Count;p++)if(item.P(p)>204)item.Set(p,204);if(item.P(6)>202)item.Set(6,202);}
    else if(item.Code==14&&item.P(3)>=199.89999389648438f&&item.P(3)<=200.10000610351562f)item.Set(3,200);
   }
   if(outputTag==69){if(rows.Count==0||rows[rows.Count-1].Count!=7)throw new NotSupportedException("PAGE end-marker removal would cut through a record.");rows.RemoveAt(rows.Count-1);}
   return rows;
  }
 }
}
