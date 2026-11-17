using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputRecordCompaction {
  // Section65 099E..0B9D: working-record parameter selection. Codes below6
  // expose at most12 words; other codes expose15. Numeric zero trimming also
  // trims negative zero. File trailers and document append state are separate.
  public static byte[] Compact(float[] parameters){if(parameters==null||parameters.Length!=15)throw new ArgumentException("Fifteen working words required.");int count=parameters[0]>=6?15:12;while(count>0&&parameters[count-1]==0)count--;byte[] result=new byte[(count+1)*4];Buffer.BlockCopy(BitConverter.GetBytes((float)count),0,result,0,4);Buffer.BlockCopy(parameters,0,result,4,count*4);return result;}
 }
}
