using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ScoreModern {
 public sealed partial class Editor {
  int studyStage,studyBottom,studyCount,studyNoteStaff;
  float studyLeft,studyRight;

  void SubmitOriginalStemSpacing(string text){
   if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){
    ClearMainGroup();Message("");return;
   }
   if(studyStage==3){
    if(!String.IsNullOrWhiteSpace(text))throw new InvalidDataException(GroupDeletePrompt);
    ApplyOriginalStemSpacing();
    return;
   }
   var fields=text.Split(new[]{' ',',','	'},StringSplitOptions.RemoveEmptyEntries);
   var values=new float[fields.Length];
   for(int i=0;i<fields.Length;i++)
    if(!Single.TryParse(fields[i],NumberStyles.Float,CultureInfo.InvariantCulture,out values[i])||
       Single.IsNaN(values[i])||Single.IsInfinity(values[i]))
     throw new InvalidDataException("Enter finite SCORE parameters.");
   if(studyStage==0){
    if(values.Length!=2)throw new InvalidDataException(SystemSourcePrompt);
    studyBottom=GroupInteger(values[0],1,32);
    studyCount=GroupInteger(values[1],1,33-studyBottom);
    studyStage=1;Message("Enter Left position, Right position. ->");
   }else if(studyStage==1){
    if(values.Length!=2)throw new InvalidDataException("Enter Left position, Right position. ->");
    studyLeft=values[0];studyRight=values[1]<=studyLeft?220:values[1];
    studyStage=2;Message("Type number of staff containing notes to be adjusted.->");
   }else{
    if(values.Length!=1)throw new InvalidDataException("Type number of staff containing notes to be adjusted.->");
    studyNoteStaff=GroupInteger(values[0],1,32);
    studyStage=3;Message(GroupDeletePrompt);
   }
  }

  void ApplyOriginalStemSpacing(){
   var original=score.Items.OrderBy(item=>item.Staff).ThenBy(item=>item.P(3)).ToArray();
   var order=original.Select(item=>item.Clone()).ToArray();
   var notes=order.Where(item=>item.Code==1&&item.Staff==studyNoteStaff&&item.Count>=5&&
    Math.Abs(item.P(4))<100&&(int)item.P(5)>=10&&item.P(3)>=studyLeft&&item.P(3)<=studyRight).ToArray();
   if(notes.Length>80)throw new NotSupportedException("The original STUD note list is limited to 80 notes.");
   if(notes.Length>=2){
    var page=new NativePageDocument(score);
    float width=(float)((double)page.Tables.Scales[studyBottom]*2.8333001136779785f/25);
    float first=notes[0].P(3),left=first,span=notes[notes.Length-1].P(3)-first;
    float offset=1000;
    bool aborted=false;int intervals=0;
    int previous=(int)notes[0].P(5)%100/10;
    foreach(var note in notes.Skip(1)){
     float right=note.P(3);
     int stem=(int)note.P(5)%100/10;
     float delta=stem>previous?width:stem<previous?-width:0;
     float distance=right-left;
     // Original 2CF5:1276-127F returns immediately on a zero interval,
     // without the final rescaling. Earlier interval writes remain committed.
     if(distance==0){
      if(intervals!=0&&!NativeEditCompatibility.ReproduceInvalidStates)
       throw new NotSupportedException("STUD's coincident-note return would retain intermediate shifted coordinates. Original invalid-state compatibility permits that partial result.");
      aborted=true;break;
     }
     float destination=left+offset;
     float factor=(float)(((double)right+delta-left)/distance);
     TransformStemSpacingInterval(order,left,right,destination,factor);
     intervals++;
     offset+=delta;left=right;previous=stem;
    }
    float finalLeft=first+1000,finalRight=left+offset;
    float finalSpan=finalRight-finalLeft;
    if(!aborted){
     if(finalSpan==0)throw new InvalidDataException("STUD produced a zero horizontal span.");
     TransformStemSpacingInterval(order,finalLeft,finalRight,first,(float)((double)span/finalSpan));
    }
   }
   for(int i=0;i<order.Length;i++)
    if(order[i].Bytes.SequenceEqual(original[i].Bytes))order[i]=original[i];
   ClearMainGroup();
   ApplyMainOrder(order,"");
  }

  void CheckOriginalStemSpacing(){
   var previous=score;
   string previousFile=current;
   try{
    foreach(bool reverse in new[]{false,true}){
     var fixture=NativeMeasurements.New(source);
     fixture.Items.Clear();
     fixture.Items.Add(Item.Make(8,1,0,0,1,200));
     for(int i=0;i<4;i++)fixture.Items.Add(Item.Make(1,1,20+i*20,7,(i%2==0)^reverse?10:20,0,1,0));
     fixture.Items.Add(Item.Make(2,1,40,5,-2,0,1));
     SetDocument(fixture,"");
     byte[] before=score.Encode();
     Execute("STUD");Execute("1 1");Execute("0 220");Execute("1");Execute("");
     float[] wanted=reverse?new[]{19.999998092651367f,38.017547607421875f,61.982444763183594f,79.99987030029297f}:
      new[]{20.00000762939453f,41.803672790527344f,58.19634246826172f,80.0001220703125f};
     var actual=score.Items.Where(item=>item.Code==1).Select(item=>item.P(3)).ToArray();
     if(!actual.SequenceEqual(wanted)||score.Items.Single(item=>item.Code==2).P(3)!=wanted[1])
      throw new InvalidOperationException("STUD differs from the original alternating-stem and rest positions.");
     Undo(false);
     if(!score.Encode().SequenceEqual(before))
      throw new InvalidOperationException("STUD undo did not restore the original records.");
    }
    Console.WriteLine("PASS: DOS STUD alternating-stem spacing and undo.");
   }finally{ClearMainGroup();SetDocument(previous,previousFile);}
  }

  void TransformStemSpacingInterval(Item[] order,float left,float right,float destination,float factor){
   float origin=(float)(destination-(double)left*factor);
   float low=left-.001f,high=right+.001f;
   foreach(var item in order){
    if(item.Staff<studyBottom||item.Staff>=studyBottom+studyCount)continue;
    if(item.Code<1||item.Code>18)throw new NotSupportedException("STUD requires a supported SCORE item code.");
    NativeItemMotion.TransformHorizontal(item,x=>x>=low&&x<=high?(float)((double)x*factor+origin):x);
   }
  }
 }
}
