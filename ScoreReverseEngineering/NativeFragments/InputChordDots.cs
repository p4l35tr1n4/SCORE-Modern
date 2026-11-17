using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordDots {
  static bool HasBits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  // Section56 1B62: dotted chord controls stored in decimal orders of P9.
  public static void Apply(IList<float[]> rows){if(rows.Count>0)ApplyRange(rows,0,rows.Count);}
  public static void ApplyRange(IList<float[]> rows,int first,int countLimit){
   int at=first;while(at>=0&&at<rows.Count){
    if(rows[at][0]!=1||rows[at][8]<10){if(at>=countLimit-2)return;at++;continue;}int end=at;do{int next=end+1;if(next>=rows.Count||rows[next][0]!=1||rows[next][4]>=10)break;end=next;if(next>=countLimit-1)break;}while(true);
    if(end==at){if(at>=countLimit-2)return;at++;continue;}int found=-1;for(int i=at;i<=end;i++)if(HasBits(rows[i][9])){found=i;break;}
    if(found>=0){bool up=rows[at][4]<20;int mode=0;
     if(!up&&found>0)rows[found-1][8]=(float)((double)rows[found-1][8]%100);
     int start=up?end:at,finish=up?at:end,step=up?-1:1;
     for(int i=start;;i+=step){float shift=0,extra=0;
      if(up){if(!HasBits(rows[i][9])){extra=1000;if(i!=start){if(rows[i+1][3]-rows[i][3]>2)mode=0;else if(mode!=0){shift=100;mode=1;}}if(rows[i][8]>=100)shift=0;rows[i][8]=(float)((double)rows[i][8]+shift+extra);}else mode=1;}
      else if(i!=start){bool change=true;if(!HasBits(rows[i][9])){if(rows[i-1][3]-rows[i][3]>2)mode=0;else if(mode!=0){shift=100;mode=1;}if(mode==0)change=false;}else{extra=1000;shift=100;mode=1;}if(change){if(rows[i][8]>=100)shift=0;rows[i][8]=(float)((double)rows[i][8]+shift+extra);}}
      if(i==finish)break;
     }
    }
    at=end;if(at>=countLimit-2)return;at++;
   }
  }
 }
}
