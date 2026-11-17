using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutRippleResult {public bool Applied;public int[] Starts;public short Source,Destination,Count;}
 // Original overlay 6, 2C0C:0000 through the first LJ call. Caller supplies
 // the original sorted record index and retains headers and external resources.
 public static class LayoutRipple {
  static int W(float v){if(Single.IsNaN(v)||Single.IsInfinity(v)||v< -32768||v>=32768)throw new NotSupportedException("Original RIP working word leaves its finite domain.");return (short)Math.Truncate(v);}
  public static LayoutRippleResult Prepare(float[] words,int[] starts,short count,short source,short destination,short bars){
   var result=new LayoutRippleResult{Starts=starts,Source=source,Destination=destination,Count=count};if(count<1||source<1||destination<1||bars<1)return result;if(source+count-1>32||destination+count-1>32)throw new NotSupportedException("Original RIP staff traversal leaves the supported table.");
   int sourceFirst=0,destinationFirst=0,lastPosition=-999;var barWords=new List<int>();int bottom=Math.Min(source,destination),top=Math.Max(source,destination);var live=new List<int>(starts);
   foreach(int w in starts){int at=w-1,code=W(words[at+1]),staff=W(words[at+2]);if(staff<bottom)continue;if(staff>top)break;if(staff==destination){if(destinationFirst==0&&(code<3||code==18))destinationFirst=w;}else if(code<3||code==18){if(sourceFirst==0)sourceFirst=w;}else if(code==14&&sourceFirst!=0){int rounded=(int)Math.Round(words[at+3],MidpointRounding.AwayFromZero);if(rounded==lastPosition)throw new NotSupportedException("Original RIP reports repeated bar positions.");barWords.Add(w);lastPosition=rounded;}}
   if(bars>barWords.Count)return result;if(sourceFirst==0)throw new NotSupportedException("Original RIP source boundary is absent.");
   // A zero destination reference is used by the original for an empty
   // destination; its same one-based expression reads word 2 as the anchor.
   // Keep that measured arithmetic rather than inventing a replacement point.
   for(int staff=top;staff<top+count;staff++){
    int remove=live.FindIndex(candidate=>W(words[candidate+1])==staff&&words[candidate]==18&&words[candidate+2]>=195);if(remove<0)continue;
    int w=live[remove],length=W(words[w-1])+1;Array.Copy(words,w-1+length,words,w-1,words.Length-(w-1+length));live.RemoveAt(remove);
    for(int i=remove;i<live.Count;i++)live[i]-=length;
    // The original deletion helper compacts the word bank and index table.
    // Keep references to the already collected source bar list in that bank.
    for(int i=0;i<barWords.Count;i++)if(barWords[i]>w)barWords[i]-=length;
    if(sourceFirst>w)sourceFirst-=length;if(destinationFirst>w)destinationFirst-=length;
   }
   int cutIndex=destination<source?barWords.Count-bars:bars,cutWord=cutIndex==0?0:barWords[cutIndex-1]+3;float cutoff=cutIndex==0?(float)((double)words[sourceFirst+2]-1):(float)((double)words[cutWord-1]+0.10000000149011612f);float shift,anchor,total,ratio;short mode=0;float left,right;int low=destination,high=destination+count-1;
   Func<int,int,float,float,short,short[]> collect=(b,t,l,r,m)=>LayoutCoordinateCollection.Collect(words,live.ToArray(),(short)b,(short)t,l,r,.001f,1,(short)live.Count,32,0,200,100,2800,m).References;
   Action<float,float,float,short> move=(l,r,delta,m)=>{for(int staff=source;staff<source+count;staff++){var references=collect(staff,staff,l,r,m);LayoutRippleCoordinates.Shift(words,references,delta);LayoutRippleCoordinates.StaffShift(words,references,destination-source);}};
   if(destination<source){float tail=(float)(200d-cutoff);anchor=(float)((double)words[destinationFirst+2]-1);shift=(float)((double)tail+10);LayoutRippleCoordinates.Shift(words,collect(low,high,anchor,210,0),shift);move(cutoff,210,(float)((double)anchor-cutoff),2);float available=(float)(200d-anchor);total=(float)((double)available+tail+9.899999618530273f);ratio=(float)(((double)available-1)/total);mode=-1;left=anchor;right=(float)((double)anchor+total);
   }else{anchor=words[destinationFirst+2];float sourceLeft=(float)((double)words[sourceFirst+2]-1);float width=(float)((double)cutoff-sourceLeft);shift=(float)(210d-sourceLeft);move(sourceLeft,cutoff,shift,0);right=(float)((double)width+210);ratio=(float)((200d-anchor)/((double)right-anchor));mode=2;left=anchor;total=right;}
   LayoutRippleCoordinates.Scale(words,collect(low,high,left,right,mode),anchor,ratio);if(mode>=0){low=source;high=source+count-1;}LayoutRippleCoordinates.Shift(words,collect(low,high,210,1000,0),-shift);if(cutWord!=0)words[cutWord-1]=200;result.Applied=true;result.Starts=live.ToArray();return result;
  }
 }
}
