using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal static class NativeJustLayout {
  const short StaffLimit=98;

  internal static byte[] EncodeOutput(Score source,bool framed){
   var output=source.Clone();
   if(source.Trailer.Length==24){
    output.Trailer=new byte[20];
    Buffer.BlockCopy(source.Trailer,0,output.Trailer,0,4);
    Buffer.BlockCopy(source.Trailer,8,output.Trailer,4,16);
   }
   BitConverter.GetBytes(4f).CopyTo(output.Trailer,12);
   if(framed&&output.Items.Count>0)
    BitConverter.GetBytes((float)output.Items[0].Count).CopyTo(output.Trailer,0);
   var bytes=output.Encode();
   return framed?FortranRecords.Write(bytes):bytes;
  }

  internal static Score AdjustHeight(Score source,float height){
   return AdjustHeightBatch(new[]{source},height)[0];
  }

  internal static Score[] AdjustHeightBatch(IList<Score> scores,float height){
   if(scores.Count==0)throw new ArgumentException("Choose scores for height adjustment.");
   if(!(height>0)||Single.IsInfinity(height))throw new ArgumentException("Enter a finite positive height.");
   short units=NativeMeasurements.Get(scores[0]);
   float total=0;
   for(int index=scores.Count-1;index>=0;index--){
    if(NativeMeasurements.Get(scores[index])!=units)
     throw new NotSupportedException("The files in a JUST height job must use the same measurement units.");
    total=(float)((double)total+HeightGuide(scores[index]).P(8));
   }
   float ratio=(float)((double)height/total);
   if(!(ratio>0)||Single.IsInfinity(ratio))throw new NotSupportedException("The height ratio is outside the finite range.");
   return scores.Select(score=>ScaleHeight(score,ratio)).ToArray();
  }

  static Item HeightGuide(Score source){
   var staves=source.Items.Where(item=>item.Code==8).ToArray();
   if(staves.GroupBy(item=>item.Staff).Any(group=>group.Count()>1))
    throw new NotSupportedException("JUST needs one staff definition per staff in this file.");
   var first=staves.FirstOrDefault(item=>item.Staff==1);
   if(first==null||!(first.P(8)>0)||Single.IsInfinity(first.P(8)))
    throw new NotSupportedException("JUST needs the current height in staff 1 parameter 8.");
   return first;
  }

  static Score ScaleHeight(Score source,float ratio){
   var result=source.Clone();
   var staves=result.Items.Where(item=>item.Code==8).OrderBy(item=>item.Staff).ToArray();
   if(staves.Any(staff=>staff.Staff<1||staff.Staff>StaffLimit||Single.IsNaN(staff.P(4))||
    Single.IsInfinity(staff.P(4))||!(staff.P(5)>=0)||Single.IsInfinity(staff.P(5))))
    throw new NotSupportedException("This staff position or size leaves the verified JUST height domain.");
   int index=0;
   // H uses the local 32-entry staff table, even when J or L uses higher
   // staves. A gap in that table points forward to the next staff record.
   for(int position=1;position<=32&&index<staves.Length;position++){
    while(index<staves.Length&&staves[index].Staff<position)index++;
    if(index==staves.Length||staves[index].Staff>32)break;
    var staff=staves[index];
    double size=staff.P(5)==0?1:staff.P(5);
    double nominal=18.0*(position-1)/size;
    staff.Set(4,(float)(((double)staff.P(4)+nominal)*ratio-nominal));
    if(position==1)staff.Set(8,(float)((double)staff.P(8)*ratio));
   }
   var rows=result.Items.Select(item=>Enumerable.Range(1,item.Count).Select(item.P).ToArray()).ToList();
   result.Items=InputTranspositionOrder.OrderForMain(rows,false).Select(itemIndex=>result.Items[itemIndex]).ToList();
   return result;
  }

  internal static Score LineUp(Score source){
   return ApplyHorizontal(source,true);
  }

  internal static Score Justify(Score source){
   return Justify(source,null,null);
  }

  internal static Score Justify(Score source,float? left,float? right){
   return ApplyHorizontal(source,false,left,right);
  }

  internal static Score LineUpAndJustify(Score source){
   return ApplyHorizontal(source,true,justifyAfterLineup:true);
  }

  internal static Score[] ApplyHorizontalBatch(IList<Score> scores,bool lineup,bool justify,float? left,float? right){
   return ApplyHorizontalBatchWithMessages(scores,lineup,justify,left,right,null);
  }

  internal static Score[] ApplyHorizontalBatchWithMessages(IList<Score> scores,bool lineup,bool justify,
   float? left,float? right,Action<string> report,bool scor4=false){
   if(scores.Count==0)throw new ArgumentException("Choose scores for horizontal layout.");
   var combined=scores[0].Clone();
   combined.Items.Clear();
   var offsets=new int[scores.Count+1];
   for(int index=0;index<scores.Count;index++){
    var score=scores[index];
    if(score.Items.Count==0)throw new InvalidDataException("A JUST input contains no items.");
    if(NativeMeasurements.Get(score)!=NativeMeasurements.Get(combined))
     throw new NotSupportedException("The files in a JUST layout job must use the same measurement units.");
    int top=score.Items.Max(item=>item.Staff);
    if(top<1||top>StaffLimit-offsets[index]||score.Items.Any(item=>item.Staff<1))
     throw new NotSupportedException("This JUST job exceeds the supported combined staff range of 1–98.");
    offsets[index+1]=offsets[index]+top;
    foreach(var original in score.Items){
     var item=original.Clone();
     if(offsets[index]!=0)item.Set(2,item.P(2)+offsets[index]);
     combined.Items.Add(item);
    }
   }
   combined=ApplyHorizontal(combined,lineup,left,right,lineup&&justify,report,scor4);
   var result=new Score[scores.Count];
   for(int index=0;index<scores.Count;index++){
    result[index]=scores[index].Clone();
    result[index].Items=combined.Items.Where(item=>item.Staff>offsets[index]&&item.Staff<=offsets[index+1]).ToList();
    if(offsets[index]!=0)
     foreach(var item in result[index].Items)item.Set(2,item.P(2)-offsets[index]);
   }
   return result;
  }

  static Score ApplyHorizontal(Score source,bool lineup,float? left=null,float? right=null,bool justifyAfterLineup=false,
   Action<string> report=null,bool scor4=false){
   if(source.Items.Count==0)throw new InvalidDataException("The input score contains no items.");
   if(source.Items.Count>9999)throw new InvalidDataException("The job exceeds JUST's 9,999-item capacity.");
   short bottom=checked((short)source.Items.Min(item=>item.Staff));
   short top=checked((short)source.Items.Max(item=>item.Staff));
   if(bottom<1||top>StaffLimit)throw new NotSupportedException("This JUST job exceeds the supported staff range of 1–98.");
   if(source.Items.Where(item=>item.Code==8).GroupBy(item=>item.Staff).Any(group=>group.Count()>1))
    throw new NotSupportedException("JUST needs one staff definition per staff in this file.");
   var rows=source.Items.Select(item=>Enumerable.Range(1,item.Count).Select(item.P).ToArray()).ToList();
   var order=InputTranspositionOrder.OrderForMain(rows,false);
   var words=new float[32000];
   var starts=new int[rows.Count];
   int position=0;
   for(int index=0;index<order.Length;index++){
    var item=source.Items[order[index]];
    if(position+item.Count+1>words.Length)throw new InvalidDataException("The score exceeds the JUST parameter capacity.");
    starts[index]=position+1;
    Buffer.BlockCopy(item.Bytes,0,words,position*4,item.Bytes.Length);
    position+=item.Count+1;
   }
   if(position+source.Trailer.Length/4<=words.Length)
    Buffer.BlockCopy(source.Trailer,0,words,position*4,source.Trailer.Length);
   var scales=Enumerable.Repeat(25f,StaffLimit+1).ToArray();
   foreach(var item in source.Items.Where(item=>item.Code==8)){
    float size=item.P(5);
    scales[item.Staff]=(float)((size==0?1.0:size)*25);
   }
   if(lineup){
    var lined=LayoutLineupSystem.ApplyWithBeatPolicy(words,starts,bottom,top,scales,
     BitConverter.ToSingle(BitConverter.GetBytes(0x3f15c01au),0),StaffLimit,2800,
     new LayoutLineupWorkspace{MaximumStaff=StaffLimit,StandaloneJust=true,Scor4DurationScale=scor4},true);
    if(lined.Bottom<0)throw new NotSupportedException("The score has no rhythmic streams to line up.");
    if(lined.UnequalBeats&&report!=null)report("JUST stopped lining up at staff "+lined.BeatTotals.Last().Staff+
     " because its beat total differs.");
    bottom=lined.Bottom;top=lined.Top;
    if(!lined.UnequalBeats){left=lined.Left;right=lined.Right;}
   }
   if(!lineup||justifyAfterLineup){
    var bars=source.Items.Where(item=>item.Code==14&&item.Staff==bottom).ToArray();
    float low=left??(bars.Length==0?-999:bars.Min(item=>item.P(3)));
    float high=right??(bars.Length==0?-999:bars.Max(item=>item.P(3)));
    if(low!=high||left.HasValue||right.HasValue){
     if(!(low<high))throw new ArgumentException("The right limit must be greater than the left limit.");
     var justified=LayoutJustificationSystem.Apply(words,starts,bottom,top,low,high,scales,new float[4],StaffLimit,2800,
      new LayoutJustificationWorkspace{StandaloneJust=true,PositionJoinTolerance=.1f,
       OriginalQuarterToneWidths=NativeEditCompatibility.OriginalQuarterToneWidths});
     if(justified.Gaps.CompressedMinimums&&report!=null)report("JUST reduced minimum spacing from "+
      justified.Gaps.MinimumTotal.ToString("0.000")+" to fit "+justified.Gaps.Width.ToString("0.000")+".");
    }
   }
   var result=source.Clone();
   result.Items.Clear();
   for(int index=0;index<starts.Length;index++){
    int length=source.Items[order[index]].Bytes.Length;
    var bytes=new byte[length];
    Buffer.BlockCopy(words,(starts[index]-1)*4,bytes,0,length);
    result.Items.Add(new Item(bytes));
   }
   return result;
  }
 }
}
