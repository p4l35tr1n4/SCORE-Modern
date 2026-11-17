using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 //3195:000D..0822, positive mode1, ordinary finite note/rest durations.
 //This leaf returns source chromatic words; it never changes MUS records.
 public static class LayoutTablatureFrequency {
  static readonly short[] clefs={0,-12,-6,-8,-7},degrees={0,0,2,4,5,7,9,11};
  static readonly short[,] keys={
   {-1,-1,-1,-1,-1,-1,-1},{-1,-1,-1,0,-1,-1,-1},{0,-1,-1,0,-1,-1,-1},
   {0,-1,-1,0,0,-1,-1},{0,0,-1,0,0,-1,-1},{0,0,-1,0,0,0,-1},{0,0,0,0,0,0,-1},
   {0,0,0,0,0,0,0},{0,0,0,1,0,0,0},{1,0,0,1,0,0,0},{1,0,0,1,1,0,0},
   {1,1,0,1,1,0,0},{1,1,0,1,1,1,0},{1,1,1,1,1,1,0},{1,1,1,1,1,1,1}};
  static void Reset(short[] carry,int key){
   if(key< -7||key>7)throw new NotSupportedException("TAB key outside the recovered table.");
   Array.Clear(carry,0,carry.Length);if(key==0)return;
   //Original counter2, then196 writes into indexed cells3..198.
   for(int i=3;i<=198;i++)carry[i]=keys[key+7,(i-3)%7];
  }
  public static Dictionary<int,short> Apply(TranspositionCollectorResult collection){
   var result=new Dictionary<int,short>();var carry=new short[200];int key=0;float previous=0;
   for(int i=0;i<collection.FirstPass.Records.Count;i++){
    var entry=collection.FirstPass.Records[i];int kind=collection.Partition.Kinds[i];
    if(kind==2){key=entry.Accidental;if(Math.Abs(key)>=100)key=0;Reset(carry,key);continue;}
    if(kind<0){previous=0;continue;}
    if(kind!=0&&BitConverter.ToInt32(BitConverter.GetBytes(previous),0)!=BitConverter.ToInt32(BitConverter.GetBytes(entry.Position),0))Reset(carry,key);
    if(!entry.Pitch.HasValue||entry.Clef<0||entry.Clef>=clefs.Length)throw new NotSupportedException("TAB pitch/clef outside the recovered table.");
    int absolute=unchecked((short)(entry.Pitch.Value+clefs[entry.Clef])),index=unchecked((short)(absolute+100));
    if(index<3||index>198)throw new NotSupportedException("TAB carry index outside verified initialized cells.");
    int degree=unchecked((short)(absolute+6))%7+1;if(degree<=0)degree+=7;
    if(entry.Accidental!=0)carry[index]=entry.Accidental==3?(short)0:entry.Accidental;
    result.Add(entry.Item,unchecked((short)(((absolute+34)/7)*12+degrees[degree]+carry[index])));previous=entry.Position;
   }
   return result;
  }
 }
}
