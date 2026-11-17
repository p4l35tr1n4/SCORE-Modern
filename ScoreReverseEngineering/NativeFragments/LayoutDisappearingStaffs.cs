using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 //2DF6:0ABD..0C05. The table has32 signed words, initially1.
 public sealed class LayoutDisappearingStaffs {
  readonly short[] words=new short[32];
  public LayoutDisappearingStaffs(){for(int i=0;i<words.Length;i++)words[i]=1;}
  public bool Hidden(int staff){return staff>=1&&staff<=32&&words[staff-1]<0;}
  public int[] HiddenStaffs(){var result=new List<int>();for(int i=1;i<=32;i++)if(Hidden(i))result.Add(i);return result.ToArray();}
  public void Apply(float[] values){
   if(values.Length==0)return;foreach(float value in values)if(Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("Non-finite DP argument.");if(values.Length>23)throw new NotSupportedException("DP arguments exceed the recovered23-word loop.");
   if(values[0]>32||BitConverter.ToInt32(BitConverter.GetBytes(values[0]),0)<0){short word=BitConverter.ToInt32(BitConverter.GetBytes(values[0]),0)<0?(short)-1:(short)1;for(int i=0;i<32;i++)words[i]=word;return;}
   if(values.Length>=3&&values[1]==0&&values[2]!=0){int first=Word(values[0]),last=Word(values[2]);if(first<1||last>32)throw new NotSupportedException("DP range outside the original staff table is prevented.");for(int i=first;i<=last;i++)words[i-1]=unchecked((short)-words[i-1]);return;}
   foreach(float value in values){int staff=Word(value);if(staff==0)break;if(staff<0)throw new NotSupportedException("DP list index outside the original staff table is prevented.");if(staff<=32)words[staff-1]=unchecked((short)-words[staff-1]);}
  }
  static short Word(float value){long word;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word))throw new NotSupportedException("Non-finite DP staff.");return unchecked((short)word);}
 }
}
