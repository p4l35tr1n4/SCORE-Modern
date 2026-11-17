using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputBeamLevels {
  // Section59 0B87..0E00: pack secondary beam runs and isolated hooks into
  // the two three-word control groups of each Code6 record.
  public static List<float[]> Pack(float[] prototype,int[] original,float[] x){
   int n=original.Length;if(n<2||x.Length!=n)throw new ArgumentException("Beam level inputs");int minimum=int.MaxValue;foreach(int f in original){if(f<1)throw new NotSupportedException("Zero/grace beam-level branch");minimum=Math.Min(minimum,f);}int[] levels=(int[])original.Clone(),hook=new int[n];
   for(int i=0;i<n;i++)if(levels[i]!=minimum){int previous=i==0?0:levels[i-1];if(i==n-1){if(previous<levels[i])hook[i]=-1;}else if(levels[i]!=levels[i+1]&&levels[i+1]<levels[i]&&previous<levels[i])hook[i]=previous<levels[i+1]?1:-1;}
   bool different=false;foreach(int f in levels)if(f!=minimum)different=true;if(!different)return new List<float[]>{(float[])prototype.Clone()};
   var result=new List<float[]>();float[] current=(float[])prototype.Clone();int cursor=0,prior=minimum,slot=9,triplets=0;
   while(cursor<n){while(cursor<n&&levels[cursor]==minimum){prior=minimum;cursor++;}if(cursor>=n)break;int stop=cursor,level=levels[cursor];if(cursor<n-1){int last=cursor;for(int j=cursor+1;j<n;j++){if(levels[j]<=minimum){stop=last;break;}last=j;level=Math.Min(level,levels[j]);stop=j;}}
    float start,end;if(stop==cursor){if(hook[cursor]>=0){start=x[cursor];end=0;}else{start=0;end=x[cursor];}}else{start=x[cursor];end=x[stop];}
    current[slot]=prior*9+level;current[slot+1]=start;current[slot+2]=end;triplets++;
    for(int j=cursor;j<=stop;j++)if(levels[j]==level)levels[j]=minimum;int next=stop+1;for(int j=cursor;j<=stop;j++)if(levels[j]>level){next=j;break;}cursor=next;prior=level;
    if(slot==9)slot=12;else{result.Add(current);current=(float[])prototype.Clone();slot=9;triplets=0;}
   }
   if(triplets!=0)result.Add(current);return result;
  }
 }
}
