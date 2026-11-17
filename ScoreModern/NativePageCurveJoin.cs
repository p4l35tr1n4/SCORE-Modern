using System;
using System.Collections.Generic;

namespace ScoreModern {
 internal static class NativePageCurveJoin {
  // PAGE 0C6A:0004..03D1 runs on the collected, unsorted bank before
  // partitioning. A staff header after music marks an input-file join.
  // Keep the continuation record as Code 13; its endpoint is not fitted
  // by the Code 4–8 coordinate collector.
  internal static void Apply(List<Item> rows){
   bool rhythm=false;
   for(int i=0;i<rows.Count;i++){
    var item=rows[i];if(item.Code<=2){rhythm=true;continue;}
    if(item.Code!=8||!rhythm)continue;int bar=i-1;
    while(bar>=0&&rows[bar].Code!=14)bar--;
    if(bar<0)throw new NotSupportedException("PAGE found an input-file join without a preceding bar.");
    float threshold=(float)((double)rows[bar].P(3)-2);
    for(int n=0;n<bar;n++){
     var left=rows[n];if(left.Code!=5)continue;
     if(left.Count<7)throw new NotSupportedException("PAGE short curve at an input-file join requires its adjacent-word path.");
     if(left.P(6)<threshold)continue;
     float control=left.Count>=8?left.P(8):0;bool negative=left.P(7)<0;Item match=null;
     for(int k=i+1;k<rows.Count;k++){
      var right=rows[k];if(right.Code==1)break;
      if(right.Code!=5||right.P(2)!=left.P(2))continue;
      if(right.Count<7)throw new NotSupportedException("PAGE short curve continuation requires its adjacent-word path.");
      if(right.P(7)>=0&&negative)continue;
      match=right;
      if((right.Count>=8?right.P(8):0)==control)break;
     }
     if(match==null)left.Set(6,threshold);
     else{left.Set(6,match.P(6));left.Set(5,match.P(5));match.Set(1,13);}
    }
    rhythm=false;
   }
  }
 }
}
