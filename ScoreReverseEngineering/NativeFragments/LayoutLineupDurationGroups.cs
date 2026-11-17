using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupDurationGroupsResult {public short OutputCount,PriorIndex,CurrentIndex;public float PriorValue,CurrentValue,PriorSum,CurrentSum;}
 // Original 31FE:0CCD..0F34, with 0C02/0C2F/0C94. This aligns the
 // two duration streams after the caller's symbol and beat-union pass.
 public static class LayoutLineupDurationGroups {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  public static LayoutLineupDurationGroupsResult Apply(float[] current,float[] prior,float[] output,short firstCurrent,short priorCount,short initialOutputCount) {
   return new Engine(current,prior,output,firstCurrent,priorCount,initialOutputCount).Run();
  }
  sealed class Engine {
   float[] a,b,c;int ai,bi,bCount,ci,steps;float av,bv,asum,bsum;
   public Engine(float[] current,float[] prior,float[] output,int firstCurrent,int priorCount,int initialOutputCount){a=current;b=prior;c=output;ai=firstCurrent;bi=0;bCount=priorCount;ci=initialOutputCount;}
   void Step(){if(++steps>100000)throw new NotSupportedException("Original duration-group traversal exceeded its verified buffer domain.");}
   void Append(float value){Step();if(ci<0||ci>=c.Length)throw new NotSupportedException("Duration-group output leaves its buffer.");c[ci++]=value;}
   void Prepare(){Step();ai++;if(ai<1||ai>a.Length)throw new NotSupportedException("Current duration stream leaves its buffer.");av=a[ai-1];asum=(float)((double)av+asum);}
   void Adjust(bool one){while(true){Step();bi++;if(bi<1||bi>b.Length)throw new NotSupportedException("Prior duration stream leaves its buffer.");bv=b[bi-1];if(Bits(bv)>0)break;Append(bv);if(one)return;}bsum=(float)((double)bv+bsum);}
   public LayoutLineupDurationGroupsResult Run(){
    while(true){asum=bsum=0;Adjust(true);
     if(Bits(bv)>=0){Prepare();
      while(Math.Abs((double)bsum-asum)>0.01f){Step();
       if(Bits(bv)<Bits(av)){
        while(true){Append(bv);if(Math.Abs((double)bsum-asum)<0.01f)goto EndCycle;
         if(bsum<=asum){Adjust(false);if(av<=(double)bv+0.005f)goto CurrentRemainder;}
         if((double)bsum-0.005f<=asum)continue;
         Prepare();if((double)bv+0.005f<av)continue;break;
        }
        ai--;asum=(float)((double)asum-av);
CurrentRemainder:
        av=(float)(((double)asum-bsum)+bv);
       }else{
        while(true){
         while(true){Append(av);if(Math.Abs((double)bsum-asum)<0.01f)goto EndCycle;
          if(asum<=bsum){Prepare();if(bv<=(double)av+0.005f)goto PriorRemainder;}
          if((double)asum-0.005f<=bsum)continue;break;
         }
         Adjust(false);if((double)av+0.005f<bv)continue;break;
        }
        bi--;bsum=(float)((double)bsum-bv);
PriorRemainder:
        bv=(float)(((double)bsum-asum)+av);
       }
       asum=av;bsum=bv;
      }
      Append(bv);
     }
EndCycle:
     if(bCount<=bi)return new LayoutLineupDurationGroupsResult{OutputCount=(short)ci,PriorIndex=(short)bi,CurrentIndex=(short)ai,PriorValue=bv,CurrentValue=av,PriorSum=bsum,CurrentSum=asum};
    }
   }
  }
 }
}
