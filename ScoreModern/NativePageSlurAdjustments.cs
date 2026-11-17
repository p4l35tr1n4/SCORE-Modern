using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal static class NativePageSlurAdjustments {
  static int Bits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0);}
  // PAGE 2F23:07C0..0B96, after LI/J and before final staff assignment.
  internal static List<Item> Apply(IList<Item> input,int bottom,int top){
   var rows=input.Select(item=>item.Clone()).ToList();int cursor=0;
   for(int staff=bottom;staff<=top;staff++){
    var pending=new List<int>();int n=cursor,last=-1;bool nextStaff=false;
    for(;n<rows.Count;n++){
     var row=rows[n];last=n;if(row.Staff<staff)continue;if(row.Staff>staff){nextStaff=true;break;}
     if(row.Code==1)break;if(row.Code==5)pending.Add(n);
    }
    if(!nextStaff&&pending.Count>0&&last>=0){float x=(float)((double)rows[last].P(3)-9);foreach(int index in pending)rows[index].Set(3,x);}
    cursor=n;
   }
   float size=1;
   foreach(var item in rows){
    if(item.Code==8){size=item.Count>=5&&Bits(item.P(5))!=0?item.P(5):1;continue;}
    if(item.Code!=5)continue;if(item.Count<6)throw new NotSupportedException("PAGE short slur adjustment requires its original adjacent-word path.");
    if(Bits(item.P(4))!=Bits(item.P(5)))continue;
    float length=(float)((double)item.P(6)-item.P(3));
    if(item.Count>=8&&item.P(8)<=-2){
     float threshold=item.P(8)==-3?11:9.5f;if((double)threshold*size<length)continue;
     item.Set(3,(float)((double)size*2+item.P(3)));item.Set(6,(float)((double)size*0.800000011920929+item.P(6)));item.Set(8,0);
     item.Set(4,(float)((double)item.P(4)+(Bits(item.P(7))<0?-1:1)));item.Set(5,item.P(4));
    }else if((double)size*2>length)item.Set(1,13);
   }
   return rows;
  }
 }
}
