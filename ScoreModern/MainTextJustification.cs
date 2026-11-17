using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Globalization;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  LayoutTextJustificationWorkspace textJustificationWorkspace=new LayoutTextJustificationWorkspace();
  void PrepareMainTextJustification(){
   // The original sorts after the text-level response, including B.
   // The pre-answer records are unchanged in the measured prompt trace.
   var rows=score.Items.Select(TransposeRow).ToList();var order=InputTranspositionOrder.OrderForMain(rows,false);bool modified=InputTranspositionOrder.MarksModified(rows.Count,false);
   if(modified||order.Where((value,index)=>value!=index).Any()){Checkpoint();layoutTextCheckpoint=true;score.Items=order.Select(index=>score.Items[index]).ToList();originalLayoutMarksModified|=modified;dirty|=modified;RefreshAll(null);}
  }
  void CommitMainTextJustification(short textStaff,float level){
   var scales=new PageCoordinates();foreach(var item in score.Items.Where(i=>i.Code==8)){var p=new float[Math.Max(21,item.Count+1)];for(int j=1;j<=item.Count;j++)p[j]=item.P(j);scales.SetStaff(p,NativeMeasurements.Get(score)==1);}
   var rows=score.Items.Select(TransposeRow).ToList();int[] order=InputTranspositionOrder.OrderForMain(rows,false);var starts=new int[order.Length];var words=new float[16384];int at=0;
   for(int i=0;i<order.Length;i++){var original=score.Items[order[i]];var row=rows[order[i]];if(at+row.Length+1>words.Length)throw new NotSupportedException("Original text-justification source bank capacity exceeded.");starts[i]=at+1;words[at++]=BitConverter.ToSingle(original.Bytes,0);Array.Copy(row,0,words,at,row.Length);at+=row.Length;}
   if(at+score.Trailer.Length/4<=words.Length)Buffer.BlockCopy(score.Trailer,0,words,at*4,score.Trailer.Length);
   var calculation=LayoutTextJustification.Apply(words,starts,layoutBottom,layoutTop,layoutLeft,layoutRight,textStaff,level,scales.Scales,32,2800,textJustificationWorkspace);
   var next=new List<Item>();for(int i=0;i<order.Length;i++){var row=new float[rows[order[i]].Length];Array.Copy(words,starts[i],row,0,row.Length);var item=Item.Make(row);Buffer.BlockCopy(words,(starts[i]-1)*4,item.Bytes,0,4);next.Add(item);}
   var transcript=new List<string>();if(rows.Count>0)transcript.Add("*** Sorting by staff number.");if(calculation.Gaps!=null&&calculation.Gaps.CompressedMinimums){transcript.Add("*** Not enough space to justify ideally.");transcript.Add(((double)calculation.Gaps.MinimumTotal).ToString(".000",CultureInfo.InvariantCulture).PadLeft(8)+((double)calculation.Gaps.Width).ToString(".000",CultureInfo.InvariantCulture).PadLeft(9));}
   bool changed=InputTranspositionOrder.MarksModified(rows.Count,false)||next.Count!=score.Items.Count||next.Where((item,i)=>!item.Bytes.SequenceEqual(score.Items[i].Bytes)).Any();
   if(changed){if(!layoutTextCheckpoint)Checkpoint();score.Items=next;originalLayoutMarksModified|=InputTranspositionOrder.MarksModified(rows.Count,false);dirty=true;}
   ClearMainGroup();if(calculation.PointCount>0)RefreshTransposeTextTerminators();RefreshAll(null);Message(String.Join(Environment.NewLine,transcript));
  }
 }
}
