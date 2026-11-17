using System;using System.Collections.Generic;using System.Linq;
namespace ScoreModern {
 // Original COM regrouping at 2F4A:0250 onward, before its final sort.
 // Input groups are the records appended by the original file reader.
 public static class NativeCombine {
  public static Score Project(IList<Score> input,bool metric){
   if(input==null||input.Count<1||input.Count>32)throw new NotSupportedException("Original COM input count leaves its group tables.");var groups=input.Select(score=>score.Items.Select(item=>item.Clone()).ToList()).ToList();float staffOffset=0,vertical=0,savedPosition=0,savedScale=1;float units=metric?0.11112499982118607f:0.04374999925494194f;
   for(int group=groups.Count-1;group>=0;group--){int high=0,low=999;float guide=0;int originalMaximum=groups[group].Where(item=>item.Code==8).Select(item=>item.Staff).DefaultIfEmpty(0).Max();
    for(int i=groups[group].Count-1;i>=0;i--){var item=groups[group][i];if(group!=groups.Count-1)item.Set(2,(float)((double)item.P(2)+staffOffset));if(item.Code!=8)continue;
     if(group!=groups.Count-1){if(item.Count>9&&BitConverter.ToInt32(item.Bytes,40)!=0)throw new NotSupportedException("Original COM reports a nonzero staff reference control.");float scale=item.Count>4?item.P(5):1;if(BitConverter.ToInt32(BitConverter.GetBytes(scale),0)==0)scale=1;item.Set(4,(float)((double)item.P(4)+(double)vertical/scale));}
     int staff=item.Staff;if(staff>high){high=staff;savedPosition=item.Count>3?item.P(4):0;if(item.Count>4)savedScale=item.P(5);if(BitConverter.ToInt32(BitConverter.GetBytes(savedScale),0)==0)savedScale=1;}else if(staff<low)low=staff;else continue;
     if(item.Count>7){guide=item.P(8);item.Set(8,0);}
    }
    if(high<low)low=high;staffOffset=(float)((double)staffOffset+originalMaximum);vertical=BitConverter.ToInt32(BitConverter.GetBytes(guide),0)==0?(float)((double)savedPosition*savedScale):(float)((double)guide/units-unchecked((short)((high-low+1)*18))+vertical);
   }
   return new Score{Items=groups.SelectMany(items=>items).ToList(),Trailer=(byte[])input[input.Count-1].Trailer.Clone()};
  }
 }
}
