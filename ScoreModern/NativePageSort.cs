using System;
using System.Collections.Generic;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal static class NativePageSort {
  static int Integer(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value<Int32.MinValue||value>Int32.MaxValue)throw new NotSupportedException("PAGE sort key leaves the recovered signed-32 domain.");return (int)value;}
  // 0CC8:0385..060F: signed composite keys, stable minimum selection.
  // Reader mode zero adds the staff number to hundredths of P3; S/X
  // multiplies the staff word by 1,000,000 before adding the position key.
  internal static List<Item> Apply(IList<Item> input,bool staffFirst){
   var rows=input.Select((item,index)=>new{Item=item,Index=index,Key=unchecked((item.Count<3?0:Integer((double)item.P(3)*100))+Integer(item.P(2))*(staffFirst?1000000:1))}).ToArray();
   if(rows.Any(row=>row.Item.Count<2||row.Key>=99000000))throw new NotSupportedException("PAGE sort reaches its short-record or sentinel path.");
   return rows.OrderBy(row=>row.Key).ThenBy(row=>row.Index).Select(row=>row.Item.Clone()).ToList();
  }
 }
}
