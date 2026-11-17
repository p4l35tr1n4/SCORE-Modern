using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal static class NativePageInitialCleanup {
  // PAGE 084F:0014..007D removes staff records, then 09DC..0C82
  // handles initial bars and repeated meter/bar records before templates.
  internal static List<Item> Apply(IList<Item> input,float[] spans){
   var rows=input.Where(item=>item.Code!=8).Select(item=>item.Clone()).ToList();bool rhythmic=false;
   for(int n=0;n<rows.Count;n++){
    Item item=rows[n];if(item.Code<=2)rhythmic=true;
    if(item.Code!=14&&item.Code!=18)continue;
    bool remove=false;
    if(item.Code==14){int style=item.Count>=5?(int)Math.Truncate(item.P(5)):0;
     if(style>7&&item.P(4)>=0)remove=true;
     else if(!rhythmic){if(style==0)remove=true;else if(style==4)continue;else remove=true;}
    }
    if(remove){rows.RemoveAt(n--);continue;}
    if(item.Code==14&&item.Count>=4){if(item.Staff<1||spans==null||item.Staff>=spans.Length)throw new NotSupportedException("PAGE bar staff leaves its explicit span table.");item.Set(4,spans[item.Staff]);}
    for(int j=n+1;j<rows.Count;j++){
     var later=rows[j];if(later.P(2)!=item.P(2))continue;
     if(later.Code<=2||item.Code!=18&&later.Code==17)break;
     if(later.Code!=item.Code)continue;
     bool deleteCurrent=item.Code!=14||item.Count<=4&&(later.Count<5||later.P(5)<8||later.P(4)<0);
     if(deleteCurrent){rows.RemoveAt(n--);break;}
     rows.RemoveAt(j);n--;break; // source restarts at the current record after deleting a later one
    }
   }return rows;
  }
 }
}
