using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupStreamUnionResult {public short Count,CurrentIndex,PriorIndex;public float LastValue,CurrentSum,PriorSum;}
 // Original 31FE:0583..0918 union loop. Inputs are the caller's already
 // normalized rhythm/symbol streams; duration subdivision follows separately.
 public static class LayoutLineupStreamUnion {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  public static LayoutLineupStreamUnionResult Apply(float[] current,short firstCurrent,short currentCount,float[] prior,short priorCount,float[] output){return new Engine(current,firstCurrent,currentCount,prior,priorCount,output).Run();}
  sealed class Engine {
   float[] a,b,c;int ai,bi=1,an,bn,ci,steps;float av,bv,last,asum,bsum,difference;
   public Engine(float[] current,int first,int currentCount,float[] prior,int priorCount,float[] output){a=current;b=prior;c=output;ai=first;an=currentCount;bn=priorCount;}
   void Step(){if(++steps>100000)throw new NotSupportedException("Original lineup stream union exceeds its verified traversal domain.");}
   float A(int index){Step();if(index<1||index>a.Length)throw new NotSupportedException("Current lineup stream leaves its buffer.");return a[index-1];}
   float B(int index){Step();if(index<1||index>b.Length)throw new NotSupportedException("Prior lineup stream leaves its buffer.");return b[index-1];}
   void Append(){Step();if(Bits(bv)!=0&&(Bits(bv)>0||Bits(last)<=0||bv==-14)){
    if(ci>=c.Length)throw new NotSupportedException("Lineup stream union exceeds its output buffer.");c[ci++]=bv;last=bv;
   }}
   public LayoutLineupStreamUnionResult Run(){int nextA=0,nextB=0;
    while(true){Step();bv=B(bi);av=A(ai);
     if(av==-14&&Bits(bv)<0&&bv!=-14&&bi!=bn){
      for(int look=bi+1;look<=bn;look++){float tag=B(look);if(tag>=0)break;if(tag==-14){last=0;goto AppendValue;}}
     }
     if(Bits(bv)==0)bv=av;
     if(bv==-17&&bi<bn&&B(bi+1)==-14)last=bv;
     if(Bits(av)==Bits(bv)||Bits(av)==0)goto AppendValue;
     if(Bits(bv)<=0){
      if(Bits(av)<=0){
       if(bv<av){if(av==-14&&Bits(last)<0)goto DecrementCurrent;}
       else if(bv!=-14||Bits(last)>=0)goto DecrementCurrent;
       goto DecrementPrior;
      }
DecrementCurrent:
      ai--;goto AppendValue;
     }
     if(Bits(av)<0)goto DecrementPrior;
     asum=(float)((double)av+asum);
     while(true){
      bsum=(float)((double)bv+bsum);Append();
      while(true){difference=(float)((double)bsum-asum);if(difference<=0.001f)break;
       while(true){nextA=ai+1;if(nextA>an)goto Finish;av=A(nextA);if(Bits(av)>0)break;if(difference<=0.02f)goto ResetSums;bv=av;ai=nextA;Append();}
       asum=(float)((double)av+asum);ai=nextA;
      }
      if(difference>=-0.001f)break;
      while(true){nextB=bi+1;if(nextB>bn)goto Finish;bv=B(nextB);if(Bits(bv)>0)break;if(difference>=-0.02f)goto ResetSums;bi=nextB;Append();}
      bi=nextB;
     }
ResetSums:
     asum=bsum=0;goto Advance;
DecrementPrior:
     bi--;bv=av;
AppendValue:
     Append();
Advance:
     if(ai>=an&&bi>=bn)goto Finish;
     if(bi<bn)bi++;if(ai<an)ai++;
    }
Finish:
    return new LayoutLineupStreamUnionResult{Count=(short)ci,CurrentIndex=(short)ai,PriorIndex=(short)bi,LastValue=last,CurrentSum=asum,PriorSum=bsum};
   }
  }
 }
}
