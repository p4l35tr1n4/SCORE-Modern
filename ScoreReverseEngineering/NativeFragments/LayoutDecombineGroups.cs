using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Original 2D75:000D..015F, after DEC's staff sort. Values are group
 // sizes; omitted zero cells repeat the last nonzero size.
 public static class LayoutDecombineGroups {
  static short Word(float value){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out n))throw new NotSupportedException("Original DEC group size leaves finite conversion domain.");return unchecked((short)n);}
  public static short[] Boundaries(short maximum,float[] sizes,short lastSize){
   if(maximum<2||maximum>32||sizes==null||sizes.Length==0||sizes[0]==0)return new short[0];
   if(sizes[0]>=maximum)throw new NotSupportedException("Original DEC grouping at/above the staff maximum remains unverified.");
   var result=new List<short>();short total=0,last=lastSize;
   for(int i=0;i<32;i++){if(i<24){short value=Word(i<sizes.Length?sizes[i]:0);if(value!=0)last=value;}if(last<=0)throw new NotSupportedException("Original DEC nonpositive grouping traversal remains unverified.");result.Add(last);total=unchecked((short)(total+last));if(total>=maximum)break;}
   if(total<maximum)throw new NotSupportedException("Original DEC grouping exceeds its 32-cell table.");for(int i=1;i<result.Count;i++)result[i]=(short)Math.Min(32,result[i-1]+result[i]);return result.ToArray();
  }
 }
}
