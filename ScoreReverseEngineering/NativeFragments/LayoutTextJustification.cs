using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutTextJustificationWorkspace {public readonly float[] Positions=new float[200],Minimum=new float[200],Slack=new float[200];}
 public sealed class LayoutTextJustificationResult {public short PointCount;public JustificationGapResult Gaps;}
 // Original 2D31:0665 text-justification calculation after its text-level
 // prompt and sort. P11/P13 are the original stored text offset/width fields.
 public static class LayoutTextJustification {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Text justification leaves its finite word domain.");return (short)Math.Truncate(f);}
  public static LayoutTextJustificationResult Apply(float[] words,int[] starts,short bottom,short top,float left,float right,short textStaff,float level,float[] staffUnits,short staffThreshold,int capacity,LayoutTextJustificationWorkspace work){
   if(textStaff<1||textStaff>=staffUnits.Length)throw new NotSupportedException("Original text-justification staff index leaves its table.");
   float gap=(float)(((double)staffUnits[textStaff]/25)*1.5f);
   var references=LayoutCoordinateCollection.Collect(words,starts,bottom,top,left,right,0.001f,1,(short)starts.Length,staffThreshold,0,200,100,capacity).References;
   work.Positions[0]=left;work.Minimum[0]=0;float baseline=0;int count=1;
   for(int i=0;i<starts.Length;i++){
    int at=starts[i]-1,n=Word(words[at]);if(n<3||at+n>=words.Length)throw new NotSupportedException("Original short text-justification record is not verified.");int code=Word(words[at+1]),staff=Word(words[at+2]);float x=words[at+3];if(code!=16||x<left||x>right||staff<textStaff)continue;if(staff>textStaff)break;
    if(n<13)throw new NotSupportedException("Original text-justification metrics record is incomplete.");float p4=words[at+4];if(level!=99&&Math.Abs((double)p4-level)>1.9f)continue;
    if(Bits(baseline)==0)baseline=p4;else{if(Math.Abs((double)p4-baseline)>1)throw new NotSupportedException("Original JT reports text on more than one level.");words[at+4]=baseline;}
    if(count>=work.Positions.Length)throw new NotSupportedException("Original text-justification point capacity exceeded.");float offset=words[at+11];work.Minimum[count]=(float)((double)words[at+13]+gap+offset);if(Bits(offset)!=0)work.Minimum[count-1]=(float)((double)work.Minimum[count-1]-offset);work.Positions[count++]=x;
   }
   if(count==1)return new LayoutTextJustificationResult();
   if((double)right-work.Positions[count-1]>0.2f){if(count>=work.Positions.Length)throw new NotSupportedException("Original text-justification right boundary exceeds its table.");work.Positions[count++]=right;}
   int first=0;if((double)work.Positions[1]-left<=0.2f){first=1;count--;}
   var positions=new float[count];var minimum=new float[count];var slack=new float[count];Array.Copy(work.Positions,first,positions,0,count);Array.Copy(work.Minimum,first,minimum,0,count);
   var gaps=LayoutJustificationGaps.Prepare(slack,minimum,positions,(short)count,left,right);var coordinates=new JustificationCoordinates(words,references,-9999,0.2f,1);LayoutJustificationRedistribution.Apply(slack,positions,(short)count,gaps.PositiveSlack,gaps.NegativeSlack,coordinates);if(Bits(gaps.NegativeSlack)!=0&&Bits(gaps.PositiveSlack)!=0)LayoutJustificationSnap.Apply(words,starts,0.01f,200);
   Array.Copy(positions,0,work.Positions,first,count);Array.Copy(minimum,0,work.Minimum,first,count);Array.Copy(slack,work.Slack,count);return new LayoutTextJustificationResult{PointCount=(short)count,Gaps=gaps};
  }
 }
}
