using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutSpacingSelectionResult {
  public float[] Positions;public short[] RecordWords;public int StoppedItem;
 }
 public static class LayoutSpacingSelection {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Layout spacing conversion is outside the verified signed-word domain.");return (short)Math.Truncate(f);}
  // Original 2F68:0E6E: adjacent paired exchanges, backing up after a swap.
  // Equal positions retain their original reference order.
  public static void Sort(float[] positions,short[] records,short count) {
   if(positions==null||records==null||count<2||count>positions.Length||count>records.Length)throw new NotSupportedException("Original spacing sort requires its measured two-or-more-entry domain.");
   for(int i=0;i<count;i++)if(Single.IsNaN(positions[i])||Single.IsInfinity(positions[i]))throw new NotSupportedException("Non-finite original spacing-sort input is prevented.");
   int current=1;
   while(current<count) {
    int previous=current-1;
    if(positions[current]<positions[previous]) {
     short r=records[current];records[current]=records[previous];records[previous]=r;
     float x=positions[current];positions[current]=positions[previous];positions[previous]=x;
     if(previous>0)current=previous;
    } else current++;
   }
  }
  // Original 2DCE:000A..026E. Its horizontal comparisons use signed high
  // words and unsigned low words, not x87 numeric comparisons. Preserve that
  // ordering even for negative coordinates and signed zero.
  public static LayoutSpacingSelectionResult Collect(float[] words,int[] starts,short staff,short firstStaff,short cursor,float left,float right,float tolerance,float one,float clefLimit,float spanningLimit,float barLimit,float drawingLimit,int capacity) {
   float low=(float)((double)left-tolerance),high=(float)((double)right+tolerance);
   var positions=new List<float>();var records=new List<short>();int stopped=starts.Length+1;
   for(int i=0;i<starts.Length;i++) {
    int w=starts[i],start=w-1;if(start<0||start+2>=words.Length)throw new NotSupportedException("Original spacing record leaves its buffer.");
    int count=Word(words[start]);if(count<2||start+count>=words.Length)throw new NotSupportedException("Original spacing record extent is invalid.");
    Func<int,float> p=delegate(int n){return n<=count?words[start+n]:0;};
    int code=Word(p(1));if(code>4&&code!=14) {
     if(code<17)continue;
     // Original 2CB5:075D inspects the stored parameter count and P14/P15
     // bit patterns. It does not classify by drawing appearance or name.
     if(count>=14&&Bits(p(14))!=0||count>=15&&Bits(p(15))!=0)continue;
    }
    int itemStaff=Word(p(2));if(staff<itemStaff){stopped=i+1;break;}
    float x=p(3);if(Bits(x)<Bits(low)||Bits(x)>Bits(high))continue;
    bool selected=false;
    if(code==14) {
     float span=p(4);if(itemStaff<firstStaff||Bits(span)<0||p(5)>spanningLimit)continue;
     if(Bits(span)==0)span=one;
     selected=staff<(double)itemStaff+span;
    } else {
     if(cursor>i+1||itemStaff!=staff)continue;
     if(code==1)selected=Bits(p(7))>=0;
     else if(code==2)selected=Bits(p(6))>=0&&Bits(p(7))>=0;
     else if(code==3)selected=p(5)<=clefLimit;
     else if(code==4)selected=count>=7&&p(7)<=barLimit&&Bits(p(6))==0;
     else if(code==18){float size=p(7);if(Bits(size)==0)size=one;selected=Math.Abs((double)p(4)*size)<=drawingLimit;}
     else selected=true;
    }
    if(!selected)continue;
    if(positions.Count>=capacity||w>Int16.MaxValue)throw new NotSupportedException("Original spacing selection exceeds its buffer.");
    positions.Add(x);records.Add((short)w);
   }
   return new LayoutSpacingSelectionResult{Positions=positions.ToArray(),RecordWords=records.ToArray(),StoppedItem=stopped};
  }
 }
}
