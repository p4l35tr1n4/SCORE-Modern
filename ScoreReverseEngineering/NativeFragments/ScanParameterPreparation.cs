using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class ScanPreparedParameters {
  public uint[] Words;public short EffectiveCopyLength;
  public bool StaffOneP8Branch;public uint StaffOneP8Bits;
 }
 public static class ScanParameterPreparation {
  // The caller's twentieth working word lies outside the 19-word copy.
  public static ScanPreparedParameters PrepareFrame(short declaredLength,uint[] followingWords,uint carriedTwentiethWord) {
   var result=Prepare(declaredLength,followingWords);var frame=new uint[20];Array.Copy(result.Words,frame,19);frame[19]=carriedTwentiethWord;result.Words=frame;return result;
  }
  // Relative 27A1:049D..05C3, parameter-word projection only. The caller
  // supplies the original converted record length and 19 following raw words,
  // including neighbouring record words when the declared payload is shorter.
  // P8 conversion and external flag/stack writes are not reconstructed here.
  public static ScanPreparedParameters Prepare(short declaredLength,uint[] followingWords) {
   if(declaredLength<0||followingWords==null||followingWords.Length<19)throw new NotSupportedException("Requires nonnegative length and 19 accessible source words.");
   short length=declaredLength;
   short high=unchecked((short)(followingWords[11]>>16));ushort low=unchecked((ushort)followingWords[11]);
   if(followingWords[0]==0x41800000u&&(high>0x3f80||(high==0x3f80&&low>0)))length=12;
   var p=new uint[19];for(int j=0;j<19;j++)if(j<length)p[j]=followingWords[j];
   var result=new ScanPreparedParameters{Words=p,EffectiveCopyLength=length};
   if(p[0]==0x41000000u){
    p[11]=0x3f800000u;
    if(p[1]==0x3f800000u&&p[7]!=0){result.StaffOneP8Branch=true;result.StaffOneP8Bits=p[7];p[7]=0;}
   }
   return result;
  }
 }
}
