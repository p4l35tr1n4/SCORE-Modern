using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Original 3060:000C remapping orchestration. This calls the independently
 // recovered staff-table alignment, coordinate collector and affine routine.
 public static class LayoutLineupRemap {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup remapping leaves its finite signed-word domain.");return (short)Math.Truncate(f);}
  public static void Apply(float[] words,int[] starts,short bottom,short top,float[] merged,float[] newPositions,short count,float[] savedRhythms,float[] savedPositions,short[] staffStarts,float[] staffUnits,short staffThreshold,int capacity){
   int steps=0;Action step=delegate{if(++steps>100000)throw new NotSupportedException("Original lineup remapping exceeds its verified traversal domain.");};
   Func<int,int,int> pIndex=delegate(int i,int parameter){int at=starts[i]-1;if(at<0||at+parameter>=words.Length||Word(words[at])<parameter)throw new NotSupportedException("Original short lineup remapping record is not verified.");return at+parameter;};
   Func<int,int,float> p=delegate(int i,int n){return words[pIndex(i,n)];};
   if(count<1||count>merged.Length||count>newPositions.Length)throw new NotSupportedException("Original lineup destination table extent is invalid.");
   float upper=(float)((double)newPositions[count-1]+1001),clefThreshold=0;
   for(int i=0;i<starts.Length;i++){int staff=Word(p(i,2));if(Word(p(i,1))==8&&bottom<=staff&&staff<=top)words[pIndex(i,2)]=-p(i,2);}
   for(int i=0;i<starts.Length;i++){int staff=Word(p(i,2)),code=Word(p(i,1));if(staff<bottom||staff>top)continue;if(code<3){clefThreshold=(float)((double)p(i,3)-2);break;}if(code==3){clefThreshold=(float)((double)p(i,3)+2);break;}}
   for(int staff=bottom;staff<=top;staff++){
    if(staff<0||staff+1>=staffStarts.Length)throw new NotSupportedException("Original lineup staff table leaves its buffer.");if(staffStarts[staff]==staffStarts[staff+1])continue;
    float[] oldRhythms=new float[150],oldPositions=new float[150];var aligned=LayoutLineupStaffTables.Apply(merged,count,savedRhythms,savedPositions,staffStarts[staff],oldRhythms,oldPositions);
    int ni=aligned.FirstMerged,oi=1,oldCount=aligned.Count;if(ni<1||ni>count||oldCount<1)throw new NotSupportedException("Original lineup staff alignment has no remapping interval.");
    float startNew=newPositions[ni-1],newRhythm=merged[ni-1],threshold=(float)((double)startNew+800),oldRhythm=oldRhythms[0],startOld=oldPositions[0],nextNew=0,nextNewRhythm=0;
    bool pending=false;
    Action advance=delegate{step();ni++;if(ni<1||ni>newPositions.Length||ni>merged.Length)throw new NotSupportedException("Original lineup advances beyond its destination buffer.");nextNew=newPositions[ni-1];nextNewRhythm=merged[ni-1];};
    while(true){step();
     if(!pending){if(count<ni)break;advance();}
     if(oldCount<=oi)break;oi++;float nextOldRhythm=oldRhythms[oi-1],nextOld=oldPositions[oi-1],stopNew,remaining;
     if(Bits(newRhythm)==Bits(oldRhythm))goto EqualRhythms;
     remaining=(float)((double)newRhythm-oldRhythm);
     if(Math.Abs((double)remaining)<=0.002f)goto EqualRhythms;
     while(Bits(remaining)<0){
      while(Bits(nextNewRhythm)<=0)advance();newRhythm=(float)((double)nextNewRhythm+newRhythm);remaining=(float)((double)newRhythm-oldRhythm);advance();
      if(Math.Abs((double)remaining)<0.002f)goto EqualRhythms;
      if(Bits(nextNewRhythm)<=0)throw new NotSupportedException("Original lineup reports a rhythm-alignment error.");
     }
     pending=true;stopNew=(float)((((double)nextNew-startNew)*oldRhythm)/newRhythm+startNew);goto MapInterval;
EqualRhythms:
     pending=false;stopNew=nextNew;remaining=nextNewRhythm;
     if(nextNewRhythm!=nextOldRhythm){
      if(nextOldRhythm==-14||nextOldRhythm==-140){stopNew=(float)(((double)nextNew-startNew)/2+startNew);remaining=nextOldRhythm;ni--;}
      else if(nextNewRhythm==-14||nextNewRhythm==-140){nextOld=(float)(((double)nextOld-startOld)/2+startOld);nextOldRhythm=nextNewRhythm;oi--;}
     }
MapInterval:
     if(startOld<nextOld){
      var selected=LayoutCoordinateCollection.Collect(words,starts,(short)staff,(short)staff,startOld,nextOld,0.001f,1,(short)-starts.Length,staffThreshold,0,200,100,capacity);
      float ratio=(float)(((double)stopNew-startNew)/((double)nextOld-startOld)),offset=(float)((double)startNew+1000);
      LayoutCoordinateAffine.Apply(words,selected.References,ratio,offset,startOld,-1);
      startNew=stopNew;newRhythm=remaining;oldRhythm=nextOldRhythm;startOld=nextOld;
     }
    }
    var tail=LayoutCoordinateCollection.Collect(words,starts,(short)staff,(short)staff,threshold,upper,0.001f,1,(short)starts.Length,staffThreshold,0,200,100,capacity);
    foreach(short reference in tail.References){int index=Math.Abs((int)reference)-1;if(threshold<=words[index])words[index]=(float)((double)words[index]-1000);}
   }
   var clefs=new List<int>();var bars=new List<float>();float lastBar=0;
   for(int i=0;i<starts.Length;i++){int staff=Word(p(i,2)),code=Word(p(i,1));float x=p(i,3);
    if(bottom<=staff&&staff<=top){if(code==3&&x>=clefThreshold)clefs.Add(i);else if(code==14&&x>=clefThreshold){bars.Add(x);if(x>lastBar)lastBar=x;}}
    if(staff<0)words[pIndex(i,2)]=-p(i,2);
   }
   foreach(int item in clefs){int staff=Word(p(item,2));float x=p(item,3),gap=(float)((double)staffUnits[staff]*0.3f+1),next=lastBar;bool hasNote=false;
    for(int j=item+1;j<starts.Length;j++){if(Word(p(j,2))!=staff)break;float code=p(j,1);if(code==14){next=p(j,3);break;}if(code<=2){next=p(j,3);hasNote=true;break;}}
    if(hasNote){foreach(float bar in bars)if(bar>=x){if(next>=bar)next=bar;break;}}
    int previous=item;float previousX;
    do{step();previous--;if(previous<0)throw new NotSupportedException("Original clef adjustment reads before the item table.");previousX=p(previous,3);if((double)next-previousX>gap){words[pIndex(item,3)]=(float)(((double)next-gap)+1);goto NextClef;}}while(Word(p(previous,1))>2&&Word(p(previous,1))<17);
    words[pIndex(item,3)]=(float)(((double)next-previousX)/2+previousX);
NextClef:;
   }
  }
 }
}
