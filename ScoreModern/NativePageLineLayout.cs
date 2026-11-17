using System;using System.Collections.Generic;using System.Globalization;using System.Linq;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // PAGE's already-assembled line bank enters LI/J directly. Main's selection,
 // duplicate prompts and short-record extension are a different caller.
 // Verified at PAGE 0FE6:0096..00F7, with before/after buffers from two lines.
 internal sealed class NativePageLineLayout {
  readonly LayoutLineupWorkspace lineup;
  readonly LayoutJustificationWorkspace justification;
  internal NativePageLineLayout(bool scor4=false){
   lineup=new LayoutLineupWorkspace{Scor4DurationScale=scor4,Page4RoundEachPosition=scor4};
   justification=new LayoutJustificationWorkspace{Page4RoundScaleNumerator=scor4};
  }
  internal List<Item> Apply(IList<Item> input,short bottom,short top,float[] staffUnits,float[] afterSymbol){
   return ApplyWithWarnings(input,bottom,top,staffUnits,afterSymbol,null);
  }
  internal List<Item> ApplyWithWarnings(IList<Item> input,short bottom,short top,float[] staffUnits,float[] afterSymbol,Action<string> warning){
   if(input==null||input.Count==0||input.Count>2800)throw new NotSupportedException("PAGE line item bank is outside the verified capacity.");
   if(bottom!=1||top<1||top>32)throw new NotSupportedException("PAGE line layout leaves the original staff bank.");
   if(staffUnits==null||staffUnits.Length<=top||afterSymbol==null||afterSymbol.Length!=4)throw new ArgumentException("PAGE requires explicit staff units and after-symbol spacing.");
   var words=new float[14200];var starts=new int[input.Count];int at=0;
   for(int n=0;n<input.Count;n++){
    var item=input[n];if(item==null||item.Count<3||at+item.Count+1>words.Length)throw new NotSupportedException("PAGE line parameters exceed the original bank.");
    starts[n]=at+1;Buffer.BlockCopy(item.Bytes,0,words,at*4,item.Bytes.Length);at+=item.Count+1;
   }
   var result=LayoutLineupSystem.ApplyWithBeatPolicy(words,starts,bottom,top,staffUnits,BitConverter.ToSingle(BitConverter.GetBytes(0x3f15c01a),0),32,2800,lineup,true,
    ordinal=>{if(warning!=null)warning("Item "+ordinal.ToString(CultureInfo.InvariantCulture)+" found at a negative position.");});
   // The original Parts caller acknowledges the unequal-beats diagnostic,
   // preserves the collector writes, and still calls J (source423).
   if(result.UnequalBeats&&warning!=null)warning(String.Join(Environment.NewLine,result.BeatTotals.Select(t=>"Staff "+t.Staff.ToString(CultureInfo.InvariantCulture)+" has "+t.Total.ToString("F3",CultureInfo.InvariantCulture)+" beats")));
   if(result.Bottom<0)throw new NotSupportedException("PAGE line without rhythmic content needs its separate original path.");
   LayoutJustificationSystem.Apply(words,starts,result.Bottom,result.Top,result.Left,result.Right,staffUnits,afterSymbol,32,2800,justification);
   var output=new List<Item>();for(int n=0;n<input.Count;n++){var bytes=new byte[input[n].Bytes.Length];Buffer.BlockCopy(words,(starts[n]-1)*4,bytes,0,bytes.Length);output.Add(new Item(bytes));}return output;
  }
 }
}
