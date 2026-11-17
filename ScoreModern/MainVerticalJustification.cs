using System;using System.Collections.Generic;using System.Linq;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  short verticalSteps=4,verticalRadius=1;float verticalStaffGap;readonly float[] verticalOffsets=new float[33];
  const string VerticalClearancePrompt="Type minimum vertical space steps, minimum horizontal space steps,"+"\n"+"and minimum steps between staves.  (defaults = 4,4,0)";
  const string VerticalStaffPrompt="Type bottom staff number and number of staves to process.  (defaults = 1,32)";
  void SubmitMainVerticalJustification(string text){
   if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){if(layoutPrompt==LayoutPrompt.Staff){layoutPrompt=LayoutPrompt.Vertical;Message(VerticalClearancePrompt);}else{ClearMainGroup();Message("");}return;}
   var values=LayoutNumbers(text);if(values.Length>20)throw new NotSupportedException("Original VJ argument capacity exceeded.");Func<int,short> word=i=>TransposeWord((values.Length>i?values[i]:0).ToString("R",CultureInfo.InvariantCulture));
   if(layoutPrompt==LayoutPrompt.Vertical){verticalSteps=word(0);if(verticalSteps<1)verticalSteps=4;short horizontal=word(1);if(horizontal==0)horizontal=4;verticalRadius=(short)Math.Max(0,horizontal/2-1);verticalStaffGap=values.Length>2?values[2]:0;layoutPrompt=LayoutPrompt.Staff;Message(VerticalStaffPrompt);return;}
   short first=word(0),count=word(1);if(first<1||first>32)first=1;if(count<1)count=32;int top=first+count-1;if(top>32)top=32;CommitMainVerticalJustification(first,(short)top);
  }
  void CommitMainVerticalJustification(short bottom,short top){
   var candidate=score.Clone();var offsets=(float[])verticalOffsets.Clone();bool redraw=false,marksModified=false;var transcript=new List<string>();
   for(int pass=0;pass<3;pass++){
    var doc=new NativePageDocument(candidate);if(doc.Issues.Count!=0)throw new NotSupportedException(String.Join(Environment.NewLine,doc.Issues));var minima=new short[33][];var maxima=new short[33][];var occupied=new bool[33];for(int staff=1;staff<=32;staff++){minima[staff]=new short[101];maxima[staff]=new short[101];for(int i=0;i<101;i++){minima[staff][i]=32760;maxima[staff][i]=-32760;}}
    var dat=new NativeDatGlyphs(source);var cache=new NativeDatHeadCache();short font=0;
    foreach(var item in candidate.Items){short next;var points=NativeDatDrawing.Prepare(item,doc.Contexts[item],doc,canvas.Library,canvas.TextLayout,dat,cache,font,out next);font=next;
     if(item.Staff<bottom||item.Staff>top||item.Code==14||item.Count>11&&item.Code==1&&BitConverter.ToInt32(item.Bytes,48)!=0||item.Count>11&&item.Code==16&&item.Text.StartsWith("_99",StringComparison.Ordinal))continue;
     occupied[item.Staff]=true;bool have=false;short px=0,py=0;int skip=item.Code==8&&item.Staff!=1&&item.P(8)!=0?2:0;
     for(int i=skip;i<points.Count;i++){var point=points[i];short x=NativeNotationContext.Word(point.X,true),y=NativeNotationContext.Word(point.Y,false);if(have&&point.Operation==0)LayoutVerticalContour.Apply(new short[]{0,0,px,x,0},new short[]{0,py,y,0,0},1,4,false,false,minima[item.Staff],maxima[item.Staff],300,150);px=x;py=y;have=true;}
    }
    var placement=LayoutVerticalPlacement.Apply(minima,maxima,occupied,doc.Tables.Scales,doc.Tables.Bases,bottom,top,verticalSteps,verticalRadius,verticalStaffGap,offsets);bottom=placement.Bottom;top=placement.Top;if(bottom!=top){marksModified=true;if(pass==0)transcript.Add("Moving up/down");}if(bottom>=top)break;redraw=true;
    for(int staff=bottom+1;staff<=top;staff++)if(occupied[staff])transcript.Add("Staff"+staff.ToString(CultureInfo.InvariantCulture).PadLeft(3)+VerticalFixed(placement.Offsets[staff]));
    foreach(var item in candidate.Items)if(item.Code==8&&item.Staff>bottom&&item.Staff<=top&&BitConverter.ToInt32(BitConverter.GetBytes(placement.Offsets[item.Staff]),0)!=0){item.Set(4,(float)((double)item.P(4)+placement.Offsets[item.Staff]));}
    if(!placement.Repeat)break;if(pass<2)transcript.Add("Some points above top limit.  Trying again - Pass"+(pass+2).ToString(CultureInfo.InvariantCulture).PadLeft(2));
   }
   bool changed=candidate.Items.Where((item,i)=>!item.Bytes.SequenceEqual(score.Items[i].Bytes)).Any();
   bool trackDisplay=redraw&&changed&&CanTrackDisplayRedraw();
   if(changed){Checkpoint();score.Items=candidate.Items;}
   originalLayoutMarksModified|=marksModified;dirty|=marksModified;
   Array.Copy(offsets,verticalOffsets,33);
   ClearMainGroup();
   if(redraw)RefreshTransposeTextTerminators();
   RefreshAll(null);
   if(trackDisplay)RebuildTrackedDisplay();
   Message(String.Join(Environment.NewLine,transcript));
  }
  static string VerticalFixed(float value){string text=SingleFixedDecimal.Format(value,3);if(text.StartsWith("0.",StringComparison.Ordinal))text=text.Substring(1);else if(text.StartsWith("-0.",StringComparison.Ordinal))text="-"+text.Substring(2);if(text.Length>9)throw new NotSupportedException("Original VJ fixed-output field overflow is outside the verified formatting domain.");return text.PadLeft(9);}
 }
}
