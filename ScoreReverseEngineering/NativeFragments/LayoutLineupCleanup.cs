using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Original 3111:0675 grace/coincidence cleanup, after source remapping.
 // Preserves record order and stored binary32 boundaries; caller owns undo.
 public static class LayoutLineupCleanup {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup cleanup leaves its finite word domain.");return (short)Math.Truncate(f);}
  public static void Apply(float[] words,int[] starts,short bottom,short top,float[] staffUnits){
   int steps=0;Action step=delegate{if(++steps>100000)throw new NotSupportedException("Original lineup cleanup exceeds its verified traversal domain.");};
   Func<int,int,int> at=delegate(int item,int parameter){if(item<0||item>=starts.Length)throw new NotSupportedException("Lineup cleanup item leaves its table.");int index=starts[item]-1;if(Word(words[index])<parameter||index+parameter>=words.Length)throw new NotSupportedException("Original short cleanup record is not verified.");return index+parameter;};
   Func<int,int,float> p=delegate(int item,int parameter){return words[at(item,parameter)];};
   var bars=new List<float>();
   for(int i=0;i<starts.Length;i++)if(bottom<=Word(p(i,2))&&Word(p(i,2))<=top&&p(i,1)==14){float x=p(i,3);bool duplicate=false;for(int j=bars.Count-1;j>=0;j--)if(Bits(x)==Bits(bars[j])||Math.Abs((double)x-bars[j])<=0.01f){words[at(i,3)]=bars[j];duplicate=true;break;}if(!duplicate)bars.Add(x);}
   int currentStaff=0,lastAnchor=-1,staffStart=0;
   for(int i=0;i<starts.Length;i++){step();int staff=Word(p(i,2)),code=Word(p(i,1));if(staff<bottom)continue;if(staff>top)return;
    if(staff!=currentStaff){lastAnchor=-1;staffStart=i;}currentStaff=staff;
    if(code==1&&p(i,7)>=64){
     if(i>0&&p(i-1,1)==14&&p(i-1,3)==p(i,3))continue;
     var items=new List<int>{i};var repeated=new List<bool>{false};var oldPositions=new List<float>{p(i,3)};float previousX=p(i,3),right=200;int nextItem=i,nextCode=0;
     while(true){step();nextItem=i+1;if(nextItem>=starts.Length)return;int nextStaff=Word(p(nextItem,2));
      if(nextStaff!=staff){right=200;break;}
      i=nextItem;nextCode=Word(p(i,1));
      if(nextCode==1){if(p(i,7)<64){right=p(i,3);break;}float x=p(i,3);bool same=Bits(x)==Bits(previousX)||Math.Abs((double)previousX-x)<=0.01f;if(same)words[at(i,3)]=previousX;else{oldPositions.Add(x);previousX=x;}items.Add(i);repeated.Add(same);continue;}
      if(nextCode<4||nextCode==14){right=p(i,3);break;}
      if(nextCode>=17){int count=Word(words[starts[i]-1]);bool excluded=count>=14&&Bits(p(i,14))!=0||count>=15&&Bits(p(i,15))!=0;if(!excluded){right=p(i,3);break;}}
     }
     float scale=(float)((double)staffUnits[staff]/25);
     if(nextCode==1&&p(nextItem,5)%10!=0)right=(float)((double)right-(double)scale*2.5f);
     float left=lastAnchor>=0?p(lastAnchor,3):0;foreach(float bar in bars)if(left<bar&&bar<right){left=bar;break;}
     float available=(float)((double)right-left),minimum=(float)((oldPositions.Count*4.0+2)*scale),ratio=1;if(available<minimum)ratio=(float)((double)available/minimum);
     float spacing=(float)((double)ratio*scale*4);if(spacing<=0.05f)throw new NotSupportedException("Original grace cleanup reports insufficient spacing; diagnostic continuation is not yet verified.");
     right=(float)((double)right-(oldPositions.Count+1.0)*spacing);var newPositions=new List<float>();
     for(int k=0;k<items.Count;k++){if(!repeated[k]){right=(float)((double)right+spacing);newPositions.Add(right);}words[at(items[k],3)]=right;}
     currentStaff=0;
     for(int k=staffStart;k<=i;k++){int attachment=Word(p(k,1));if(attachment>4&&attachment<7){float x=p(k,3),end=p(k,6);for(int j=0;j<oldPositions.Count;j++){if(Bits(oldPositions[j])==Bits(end))words[at(k,6)]=newPositions[j];else if(Bits(oldPositions[j])==Bits(x))words[at(k,3)]=newPositions[j];}}}
     if(staffStart<i)i--;
    }else if(code<4||code==14)lastAnchor=i;
    else if(code>16){int count=Word(words[starts[i]-1]);if(!(count>=14&&Bits(p(i,14))!=0||count>=15&&Bits(p(i,15))!=0))lastAnchor=i;}
   }
  }
 }
}
