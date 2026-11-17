using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionPartition {
  public short[] Kinds;public float[] Boundaries;
 }
 // Original 32AC:0506..0750. Inputs are the collected boundary positions,
 // record positions and kind words; boundary collection is a separate pass.
 public static class InputTranspositionPartition {
  public static TranspositionPartition Apply(IList<float> bars,IList<float> positions,IList<short> kinds){
   if(positions.Count!=kinds.Count||(kinds.Count==0&&bars.Count!=0)||kinds.Count>200||bars.Count>99)throw new NotSupportedException("Partition input exceeds recovered table bounds.");
   var result=new TranspositionPartition{Kinds=new short[kinds.Count]};for(int i=0;i<kinds.Count;i++){result.Kinds[i]=kinds[i];if(Single.IsNaN(positions[i])||Single.IsInfinity(positions[i])||positions[i]>=9999)throw new NotSupportedException("Partition position outside verified finite sentinel domain.");}
   if(bars.Count==0){result.Boundaries=new float[0];return result;}
   float[] slots=new float[101];for(int i=0;i<bars.Count;i++){if(Single.IsNaN(bars[i])||Single.IsInfinity(bars[i]))throw new NotSupportedException("Non-finite partition boundary.");slots[i]=bars[i];}
   // Literal original loop: it compares a second slot even for one bar.
   // Equal boundaries can repeat indefinitely; protect that invalid state.
   int current=1,steps=0;
   do{
    current++;
    while(slots[current-2]>=slots[current-1]){
     if(++steps>20000)throw new NotSupportedException("Original boundary sort repeats an equal-boundary state.");
     float swap=slots[current-2];slots[current-2]=slots[current-1];slots[current-1]=swap;
     if(current==2)break;current--;
    }
   }while(current<bars.Count);
   int count=bars.Count;
   do{if(++count>100)throw new NotSupportedException("Original boundary sentinel exceeds its table.");}while(BitConverter.ToUInt32(BitConverter.GetBytes(slots[count-1]),0)!=0);
   slots[count-1]=9999;result.Boundaries=new float[count];Array.Copy(slots,result.Boundaries,count);
   if(result.Kinds[0]==0)result.Kinds[0]=1;
   int boundary=0;
   for(int i=0;i<kinds.Count;i++){
    if(positions[i]<slots[boundary]||result.Kinds[i]<0)continue;
    if(result.Kinds[i]==0){result.Kinds[i]=1;for(int j=i+1;j<kinds.Count&&result.Kinds[j]==0&&positions[j]==positions[i];j++)result.Kinds[j]=1;}
    boundary++;while(positions[i]>slots[boundary]&&boundary<count-1)boundary++;
   }
   return result;
  }
 }
}
