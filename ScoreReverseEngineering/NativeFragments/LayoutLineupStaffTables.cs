using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupStaffTablesResult {public short FirstMerged,Count;}
 // Original 3111:0303..0674: align one saved staff stream to the merged
 // symbol/duration stream before the source-coordinate remapping pass.
 public static class LayoutLineupStaffTables {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  public static LayoutLineupStaffTablesResult Apply(float[] merged,short mergedCount,float[] storedRhythms,float[] storedPositions,short firstStored,float[] outputRhythms,float[] outputPositions){return new Engine(merged,mergedCount,storedRhythms,storedPositions,firstStored,outputRhythms,outputPositions).Run();}
  sealed class Engine {
   float[] merged,saved,positions,output,oldX;int limit,si,mi,oi,first,steps;float sv,mv,mergedSum,savedSum;
   public Engine(float[] a,int n,float[] b,float[] x,int start,float[] c,float[] y){merged=a;limit=n;saved=b;positions=x;si=start-1;output=c;oldX=y;}
   void Step(){if(++steps>100000)throw new NotSupportedException("Original staff-table traversal exceeds its verified domain.");}
   float Saved(int index){Step();if(index<1||index>saved.Length)throw new NotSupportedException("Original saved staff rhythm leaves its bank.");return saved[index-1];}
   float Merged(int index){Step();if(index<1||index>merged.Length)throw new NotSupportedException("Original merged rhythm leaves its bank.");return merged[index-1];}
   void Append(float rhythm,float x){Step();if(oi>=output.Length||oi>=oldX.Length)throw new NotSupportedException("Original aligned staff table exceeds its buffer.");output[oi]=rhythm;oldX[oi++]=x;}
   LayoutLineupStaffTablesResult Result(){return new LayoutLineupStaffTablesResult{FirstMerged=(short)first,Count=(short)oi};}
   public LayoutLineupStaffTablesResult Run(){sv=Saved(si+1);for(mi=1;mi<=limit;mi++){mv=Merged(mi);if(Bits(mv)>0||Bits(mv)==Bits(sv)){first=mi;mi--;goto NewGroup;}}
    throw new NotSupportedException("Original staff-table alignment requests its error handler for an unmatched stream.");
NewGroup:
    mergedSum=savedSum=0;si++;sv=Saved(si);
    while(true){
     if(mi==limit)return Result();mi++;mv=Merged(mi);
     if(Bits(mv)>=0){mergedSum=(float)((double)mv+mergedSum);
      while(true){
       if(Bits(sv)>0){savedSum=(float)((double)sv+savedSum);Append(sv,positions[si-1]);
        while(true){float delta=(float)((double)mergedSum-savedSum);if(Math.Abs((double)delta)<=0.002f)goto NewGroup;if(Bits(delta)>0)break;
         if(mi==limit)return Result();
         do{mi++;mv=Merged(mi);if(Bits(mv)>=0)break;if(mi==limit)return Result();}while(true);
         mergedSum=(float)((double)mv+mergedSum);
        }
       }
       si++;sv=Saved(si);
      }
     }
     while(true){
      if(Bits(mv)==Bits(sv)){Append(sv,positions[si-1]);goto NewGroup;}
      if(Bits(sv)>0||sv!=-15)break;si++;sv=Saved(si);
     }
     if(si<2)throw new NotSupportedException("Original symbol midpoint reads before its saved-position table.");
     Append(mv,(float)(((double)positions[si-1]-positions[si-2])/2+positions[si-2]));
    }
   }
  }
 }
}
