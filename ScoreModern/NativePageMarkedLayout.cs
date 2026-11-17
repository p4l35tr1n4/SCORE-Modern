using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScoreModern {
 internal static class NativePageMarkedLayout {
  // PAGE P/M counts the negative width entries, then uses their ordinal
  // positions in the full score's bar table (not the marked items' P3s).
  // Single-line ranges and the measured multi-line and multi-page proposals.
  internal static NativePagePreparedPart Prepare(string path,int from,int to,float size,float height,int firstPage,bool centered,bool oldPlanningPrecision){
   return PrepareRange(path,from,to,size,height,firstPage,centered,oldPlanningPrecision,false);
  }

  internal static NativePagePreparedPart PrepareNumbered(string path,int from,int to,float size,float height,int firstPage,bool centered,bool oldPlanningPrecision){
   return PrepareRange(path,from,to,size,height,firstPage,centered,oldPlanningPrecision,true);
  }

  static NativePagePreparedPart PrepareRange(string path,int from,int to,float size,float height,int firstPage,bool centered,bool oldPlanningPrecision,bool byNumber){
   var source=Path.GetFullPath(path);var score=Score.Load(source);
   var headers=score.Items.Where(i=>i.Code==8).OrderBy(i=>i.Staff).ToArray();
   int staves=headers.Length;
   if(staves==0||staves>32||headers.Where((h,n)=>h.Staff!=n+1||h.P(5)<=0||h.P(5)!=headers[0].P(5)).Any())
    throw new NotSupportedException("Marked PAGE layout requires one system with consecutive, equally sized staves.");
   var widths=NativePageBarPlan.PrepareLayoutFile(score);
   var marks=Enumerable.Range(0,widths.Widths.Length).Where(n=>widths.Widths[n]<0).ToArray();
   if(byNumber){
    if(from<1||to<from||to>widths.Positions.Length)
     throw new ArgumentException("Choose a range of bars within the score.");
   }else if(from<0||to<=from||to>marks.Length){
    throw new NotSupportedException("Choose a range within the marked bars.");
   }
   int end=byNumber?to-1:marks[to-1];
   int start=byNumber?from-1:from==0?0:marks[from-1]+1;
   int count=end-start+1;
   bool alternating=end==8&&to==5&&from<=1&&
    marks.Take(5).SequenceEqual(new[]{0,2,4,6,8});
   bool simplePlanning=staves==1&&size==1&&height==10&&!oldPlanningPrecision;
   if(!byNumber&&count!=to-from&&!alternating&&!simplePlanning)
    throw new NotSupportedException("This marked PAGE selection pattern needs original verification.");
   var input=NativePageSort.Apply(score.Items,false);
   var initial=NativePageTemplates.InitializeMode(input,(float)((double)size/headers[0].P(5)),null,true);
   var trimmed=start==0?input:input.Where(i=>i.P(3)>widths.Positions[start-1]).ToList();
   NativePageCurveJoin.Apply(trimmed);
   float widthScale=(float)((double)size*.01f);
   var weights=widths.Widths.Skip(start).Take(count)
    .Select(weight=>checked((short)Math.Truncate((double)Math.Abs(weight)*widthScale+.5))).ToArray();
   int lines=Math.Max(1,(int)(weights.Sum(weight=>(int)weight)/200.0+.6f));
   if(count>10&&(staves!=1||size!=1||oldPlanningPrecision))
    throw new NotSupportedException("This long PAGE range needs original verification.");
   bool verifiedSingleLineNumberedRange=byNumber&&staves==1&&size==1&&start<=7&&!oldPlanningPrecision&&lines==1;
   if(count>=7||(count==3&&!verifiedSingleLineNumberedRange))
    return PrepareMultipleLines(source,score,staves,size,height,firstPage,centered,widths,start,count,weights,lines,trimmed,initial,oldPlanningPrecision,byNumber);
   if(count>=4){
    bool measuredNumberedRange=byNumber&&staves==1&&size==1&&
     (count==4&&(start>=1&&start<=3||start==5||start==6&&end==widths.Positions.Length-1)||
      count==5&&(start==1||start==5&&end==widths.Positions.Length-1)||count==6&&start>=1&&start<=4);
    bool measuredInterior=(start==1&&end==4)||(start==2&&end>=5&&end<=7)||(start==3&&(end==7||end==8))||(start==4&&(end==7||end==8))||measuredNumberedRange;
    if((start!=0&&end!=6&&!measuredInterior)||lines!=1||oldPlanningPrecision)throw new NotSupportedException("This fresh single-line marked PAGE range needs original verification.");
   }
   // The first cut retains the planned position. The cut at the second
   // boundary was measured at hundredth precision in both tested ranges.
   float cut=end==0?NativePageBarPlan.CutPositions(widths.Positions)[end]:(float)(Math.Truncate((double)widths.Positions[end]*100)/100+.01);
   var section=byNumber&&end==widths.Positions.Length-1
    ?NativePagePartition.TakeAll(trimmed)
    :NativePagePartition.Split(trimmed,cut);
   var fit=NativePageCoordinates.Fit(section.Line,0,section.End,20,200);
   var prepared=NativePageSort.Apply(initial.Templates.Insert(NativePageInitialCleanup.Apply(fit,initial.Spans)),true);
   var warnings=new List<string>();
   var laid=new NativePageLineLayout().ApplyWithWarnings(prepared,1,(short)staves,initial.Units,new float[4],warnings.Add);
   laid=NativePagePostLine.ApplyNegativeEndMode(laid,-2,0);
   laid=NativePageSlurAdjustments.Apply(laid,1,staves);
   laid=NativePageRestFollowup.Apply(laid,initial.Units,1,1);
   var plan=NativePageDefaultPartPlan.NumberedPages(new[]{new[]{count}},height,firstPage,centered);
   var page=plan[0];var assembly=new NativePageAssemblyBank(1,staves,height);
   assembly.Append(laid);
   var records=assembly.Finish(page.Number,page.NumberEnabled,page.NumberX,page.NumberY,page.NumberFont);
   var file=new NativePageFileBank(new byte[14200*4]).Encode(records,NativeMeasurements.Get(score));
   return new NativePagePreparedPart(plan,new[]{file},new[]{source},warnings.AsReadOnly());
  }
  static NativePagePreparedPart PrepareMultipleLines(string source,Score score,int staves,float size,float height,int firstPage,bool centered,NativePageSpacingResult widths,int start,int count,short[] weights,int lines,List<Item> trimmed,NativePageInitialization initial,bool oldPlanningPrecision,bool byNumber){
   bool longRange=count>10;
   bool measuredNumberedRange=byNumber&&staves==1&&size==1&&height==10&&!oldPlanningPrecision&&
    (start==1&&(count==7||count==9)||start==2&&count==8||start==3&&count==7&&start+count==widths.Positions.Length);
   if(start!=0&&!longRange&&!measuredNumberedRange&&(start+count!=9||count<7||count>8||oldPlanningPrecision))throw new NotSupportedException("This interior multiline PAGE range needs original verification.");
   int capacity=Math.Max(1,(int)(height/(float)((double)staves*size)));
   var proposal=new NativePageAutomaticLayout().Apply(weights,widths.Leading.Skip(start).Take(count).Select(value=>checked((short)value)).ToArray(),lines,capacity,height,1);
   bool verifiedInteriorPageBreak=byNumber&&staves==1&&size==1&&height==1&&start==1&&count==8;
   if(count==3&&(!proposal.Bars.SequenceEqual(new short[]{1,2})||capacity!=1)||count>=7&&!longRange&&
    (capacity==1&&start!=0&&!verifiedInteriorPageBreak||oldPlanningPrecision||!proposal.Bars.SequenceEqual(new short[]{(short)(count/2),(short)((count+1)/2)})))
    throw new NotSupportedException("This multiline PAGE proposal needs original verification.");
   var linesPerPage=NativePageDefaultPartPlan.GroupLines(proposal,count);
   var pages=NativePageDefaultPartPlan.NumberedPages(linesPerPage,height,firstPage,centered);
   var output=new List<byte[]>();
   var measurement=NativeMeasurements.Get(score);
   var symbolSpacing=new float[4];
   int last=start-1;
   var bank=new NativePageFileBank(new byte[14200*4]);
   var warnings=new List<string>();
   var core=new NativePageLineLayout();
   float offset=0;
   foreach(var page in pages){
    var assembly=new NativePageAssemblyBank(page.Bars.Length,staves,height);
    foreach(int bars in page.Bars){
     last+=bars;
     // 0B45:00C7 adds .01 to the stored planner position. Reloading
     // Old Page Data uses positions serialized to hundredths.
     // 2F23:01B6 then adds the retained REAL4 rebase offset.
     float position=widths.Positions[last];
     if(oldPlanningPrecision)position=(float)(Math.Truncate((double)position*100)/100);
     float splitAt=(float)((double)(float)((double)position+.01f)+offset);
     var partition=last==widths.Positions.Length-1?NativePagePartition.TakeAll(trimmed):NativePagePartition.Split(trimmed,splitAt);
     var fit=NativePageCoordinates.Fit(partition.Line,0,partition.End,20,200);
     var ready=NativePageSort.Apply(initial.Templates.Insert(NativePageInitialCleanup.Apply(fit,initial.Spans)),true);
     var laid=core.ApplyWithWarnings(ready,1,(short)staves,initial.Units,symbolSpacing,warnings.Add);
     laid=NativePagePostLine.ApplyNegativeEndMode(laid,-2,0);
     laid=NativePageSlurAdjustments.Apply(laid,1,staves);
     laid=NativePageRestFollowup.Apply(laid,initial.Units,1,1);
     assembly.Append(laid);
     if(last<start+count-1){
      trimmed=NativePageCoordinates.Rebase(partition.Remainder,splitAt);
      offset=(float)((double)offset-(float)((double)splitAt-10));
     }
    }
    output.Add(bank.Encode(assembly.Finish(page.Number,page.NumberEnabled,page.NumberX,page.NumberY,page.NumberFont),measurement));
   }
   return new NativePagePreparedPart(pages,output,new[]{source},warnings.AsReadOnly());
  }

 }
}
