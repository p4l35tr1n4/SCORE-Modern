using System;using System.Linq;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDecombine {
  // Original function at physical entry 2D8B:0008. Group records are emitted from
  // highest staff group down, preserving indexed order within each group.
  public static List<Score> Project(Score source,short[] boundaries){
   if(source==null||boundaries==null)throw new ArgumentNullException();var result=new List<Score>();int high=boundaries.Length==0?0:boundaries[boundaries.Length-1];
   for(int group=boundaries.Length-1;group>=0;group--){int low=group==0?0:boundaries[group-1];var output=new Score{Trailer=(byte[])source.Trailer.Clone()};float origin=0;bool first=true;
    foreach(var sourceItem in source.Items){if(sourceItem.P(2)<=low||sourceItem.P(2)>high)continue;var item=sourceItem.Clone();item.Set(2,(float)((double)item.P(2)-low));if(group>0&&item.Code==8){float scale=item.Count>4?item.P(5):1;if(BitConverter.ToInt32(BitConverter.GetBytes(scale),0)==0)scale=1;if(first){if(item.Count>3){origin=(float)((double)item.P(4)*scale);item.Set(4,0);}first=false;}else item.Set(4,(float)((double)item.P(4)-(double)origin/scale));}output.Items.Add(item);}
    var following=source.Items.FirstOrDefault(item=>item.P(2)>high);if(following!=null)Array.Copy(following.Bytes,0,output.Trailer,0,4);result.Add(output);high=low;
   }return result;
  }
 }
}

