using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordLayout {
  // Section56 0D2E and 1755: original stage1 chord displacement and
  // accidental packing. Integer scratch controls stay separate from P5.
  static readonly short[] spacing={240,250,215,440,250,240,290,240,440,300,240,255,220,430,280,250,280,240,500,330,
155,250,180,380,200,240,290,230,440,250,200,250,160,420,180,0,280,160,0,0,
155,200,120,360,180,180,220,110,400,0,130,200,120,320,140,0,0,0,0,0,
120,180,120,350,160,0,200,110,0,0,0,120,0,0,0,0,0,0,0,0,
0,110,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
// Original DS39A8 table spills into DS3A70 data at distance7.
// The first pair is the stored exponent0.5849624872; preserve these
// measured adjacent words rather than rejecting the reachable lookup.
-16358,16149,10512,0,14136,14381,19761,93,10512,59,14136,5164,129,4,5,6,7,8,9,10};
  static readonly int[] below={0,1,3,3,1,1},above={0,5,4,4,5,1};
  static int W(float p){return unchecked((short)(int)Math.Truncate(p));}
  static int Gap(int type,ref int other,int distance){int d=distance-1;if(d<1)d=1;if(other==4)other=1;else if(other==5)other=4;if(type<1||type>5||other<1||other>4||d>6)throw new NotSupportedException("Chord separation lookup outside its recovered table");return spacing[(d-1)*20+(other-1)*5+type-1];}
  // start/end are zero-based inclusive. The return is the original mutated
  // endpoint argument, not an application item index.
  public static int Apply(IList<float[]> rows,int start,int end){
   if(start<0||end>=rows.Count||end<=start)throw new ArgumentException("Chord range");int n=end-start+1,direction=rows[start][4]%100>=20?20:10;
   for(int i=start+1;i<=end;i++){if(Math.Abs(W(rows[i][3])%100-W(rows[i-1][3])%100)<2){rows[i][9]=direction;i++;}}
   int[] pitch=new int[n+1],record=new int[n+1],kind=new int[n+1],low=new int[n+1],high=new int[n+1],column=new int[n+1],basis=new int[n+1];
   for(int k=1;k<=n;k++){int i=direction==20?start+k-1:end-k+1;record[k]=i;pitch[k]=W(rows[i][3])%100;if(rows[i][9]==20)basis[k]=column[k]=283;kind[k]=W(rows[i][4]%10);if(kind[k]!=0){if(kind[k]<1||kind[k]>5)throw new NotSupportedException("Accidental type");low[k]=pitch[k]-below[kind[k]];high[k]=pitch[k]+above[kind[k]];}}
   for(int i=1;i<=n;i++)if(kind[i]!=0&&basis[i]==0)for(int j=1;j<=n;j++)if(basis[j]!=0&&j!=i&&pitch[j]<=high[i]&&low[i]<=pitch[j])column[i]=283;
   int at=1;
   while(at<n){if(kind[at]!=0){int best=999,target=0,extra=0;for(int j=at+1;j<=n;j++)if(kind[j]>0){int difference=pitch[at]-pitch[j],other=kind[j],gap=difference<7?Gap(kind[at],ref other,difference):0;if(gap<best){best=gap;extra=gap;target=j;}}
     if(target!=0){int position=column[at]+extra;if(column[target]<position)column[target]=position;for(int j=at+1;j<target;j++)if(kind[j]!=0){position+=240;if(kind[j]==4)position+=200;if(column[j]<position)column[j]=position;}at=target;continue;}}
    at++;
   }
   for(int i=1;i<n;i++)if(kind[i]!=0&&column[i]!=0){int lower=low[i],position=column[i];for(int j=i+1;j<=n;j++)if(kind[j]!=0){if(high[j]<=lower)break;int old=column[j],other=kind[j],difference=position-old;if(other==4)difference-=200;if(Math.Abs(difference)<240){int gap=Gap(kind[i],ref other,pitch[i]-pitch[j]);if(difference>0){int threshold=other==4?0:200;if(old==0||threshold<position-old||gap>200){int shift=240-(position-old);if(other==4)shift+=200;if(shift>0)for(int k=i;k<j;k++)if(kind[k]!=0&&column[k]!=0)column[k]+=shift;}else column[j]=gap+position;break;}column[j]=gap+position;}}}
   for(int k=1;k<=n;k++){int shift=Math.Max(0,Math.Min(899,column[k]-basis[k]));rows[record[k]][4]=(float)(shift/1000.0+rows[record[k]][4]);}
   return n+1;
  }
 }
}
