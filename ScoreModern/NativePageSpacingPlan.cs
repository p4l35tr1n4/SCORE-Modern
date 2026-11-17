using System;
using System.Collections.Generic;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal sealed class NativePageSpacingResult {
  internal int[] Widths,Leading,Trailing;internal bool[] LeadingWritten;internal float[] Positions;internal float FinalMultiplier;
 }
 internal static class NativePageSpacingPlan {
  static bool Nonzero(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  static short Word(double value){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes((float)value),0),out n))throw new NotSupportedException("PAGE spacing integer conversion leaves its finite domain.");return unchecked((short)n);}
  // PAGE 0D29:0133..0892. Incoming items are the collector-mutated working
  // copy; assembly later rereads the untouched files. Initial multiplier is
  // explicit because the source retains it through key/meter branches.
  internal static NativePageSpacingResult Collect(IList<Item> input,float[] durations,float initialMultiplier,Action<int,int,float,float,float,float,float> observe=null,byte[] initialWorkspace=null){
   var rows=NativePageSort.Apply(input,false);var widths=new List<int>();var leading=new List<int>();var leadingWritten=new List<bool>();var trailing=new List<int>();var positions=new List<float>();
   float left=0,right=0,minLeft=0,minRight=0,sum=0,scale=initialMultiplier,lastGroup=0;int pending=0,restWeight=0,state=0,previous=0,seen=0;bool started=false;
   byte[] workspace=initialWorkspace==null?new byte[65536]:(byte[])initialWorkspace.Clone();if(workspace.Length!=65536)throw new ArgumentException("PAGE spacing requires a complete explicit workspace.");
   Action<int,float> setFloat=(address,value)=>Buffer.BlockCopy(BitConverter.GetBytes(value),0,workspace,address,4);
   Action<int,int> setWord=(address,value)=>Buffer.BlockCopy(BitConverter.GetBytes(unchecked((short)value)),0,workspace,address,2);
   Action<int,int> put=delegate(int index,int value){while(leading.Count<=index){leading.Add(0);leadingWritten.Add(false);}leading[index]=value;leadingWritten[index]=true;};
   for(int i=0;i<rows.Count;i++){
    Item item=rows[i];int code=Word(item.P(1));
    if(code>=3&&code!=18&&code!=14&&(code!=17||!started))continue;
    if(code<3||code==18||code==17)started=true;
    for(int group=0;group<=rows.Count;group++){
     item=rows[i];var p=new float[14];for(int n=1;n<=Math.Min(item.Count,14);n++)p[n-1]=item.P(n);
     if(p[5]<=-4&&code==1){p[4]=0;p[8]=0;}
     bool scaled=false,skip=false;
     if(code==1){var note=NativePageNoteSpacing.Apply(p,item.Count,item.Count>=10?item.P(10):0);scale=note.Multiplier;left=note.Left;right=note.Right;state=1;}
     else if(code==2){scale=1;left=0;var rest=LayoutRestSpacing.Apply(scale,p[5],p[4],p[3],left,(short)item.Count,p[8],p[9],item.Count>=15?item.P(15):0);left=rest.Left;right=rest.Right;scale=rest.Multiplier;
      if(item.Count>=17&&item.P(17)!=0)p[6]=item.P(17);
      restWeight=Word((double)p[6]*100+.5);if(state==0)put(widths.Count,restWeight);state=2;scaled=true;
     }
     else if(code==14){
      if(pending<=0){pending=-1;skip=true;}
      else if(p[4]>7){if(!Nonzero(right)&&BitConverter.ToInt32(BitConverter.GetBytes(p[3]),0)>=0)skip=true;}
      else if(BitConverter.ToInt32(BitConverter.GetBytes(p[3]),0)>=0){
       // Measured 02A2: helper left goes to 778A, helper right to P5.
       // PAGE then zeros 778A; 778E is untouched. First helper write
       // replaces P5 with 2.5 before the style conversion.
       var bar=LayoutSymbolSpacing.Bar((short)previous,(short)item.Count,2.5f,p[6],p[7],p[12],p[13]);p[4]=bar.Right;right=0;
      }
     }
     else if(code==17){var key=LayoutSymbolSpacing.Key((short)previous,p[6],p[4]);left=key.Left;right=key.Right;}
     else if(code==18){
      if(initialWorkspace==null)throw new NotSupportedException("PAGE meter call needs its explicit adjacent scratch workspace.");
      setFloat(0x777e,lastGroup);setFloat(0x778a,right);setFloat(0x778e,left);setWord(0x7792,item.Count);setFloat(0x7796,minRight);setWord(0x779a,previous);
      NativePageSpacingHelperCalls.Meter(workspace);right=BitConverter.ToSingle(workspace,0x778a);left=BitConverter.ToSingle(workspace,0x778e);
     }
     if(skip)break;
     if(scaled){left=(float)((double)scale*left);right=(float)((double)scale*right);}
     if(observe!=null)observe(0x65b,code,left,right,minLeft,minRight,sum);
     pending=1;previous=code;if(minLeft<left)minLeft=left;if(right>minRight)minRight=right;
     int original=i;int look=i+1;
     while(look<rows.Count){int next=Word(rows[look].P(1));if(next<=2||next>=17||next==14)break;look++;}
     if(look<rows.Count&&Math.Abs((double)rows[look].P(3)-p[2])<=.009999999776482582&&!(Word(rows[look].P(1))==18&&code==14)){
      i=look;code=Word(rows[look].P(1));if(group==rows.Count)throw new NotSupportedException("PAGE coincident-spacing loop exceeded its input.");continue;
     }
     // The original look-ahead consumes ignored records only on a successful
     // coincidence; otherwise its physical iterator is restored.
     i=original;lastGroup=(float)((double)minRight+minLeft);sum=(float)((double)minRight+minLeft+sum);minLeft=minRight=0;
     if(code==14&&pending!=0){
      if(seen>=durations.Length)throw new NotSupportedException("PAGE spacing has more bars than its duration stream.");
      trailing.Add(state==2?restWeight:0);state=0;float duration=(float)((double)durations[seen]*800);sum=(float)((double)100*sum);if(duration<0)sum=-sum;
      widths.Add(Word((double)sum+duration));positions.Add(p[2]);seen++;sum=0;
     }
     pending=1;break;
    }
   }
   while(leading.Count<widths.Count+1){leading.Add(0);leadingWritten.Add(false);}
   return new NativePageSpacingResult{Widths=widths.ToArray(),Leading=leading.ToArray(),LeadingWritten=leadingWritten.ToArray(),Trailing=trailing.ToArray(),Positions=positions.ToArray(),FinalMultiplier=scale};
  }
 }
}
