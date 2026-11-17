using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal static class NativePagePartAssembly {
  // One parts page, with the original planner's counts and pre-TMP bar
  // positions supplied explicitly. Planning and file serialization are
  // separate stages; this method never infers them from visible notation.
  internal static List<Item> SingleStaffPage(IList<Score> parts,int[] fileBars,float[] planPositions,int[] lineBars,float[] lineLeft,float staffSize,float height,int pageNumber,bool numberPage,float numberX,float numberY,float numberFont){
   if(parts==null||parts.Count==0||fileBars==null||fileBars.Length!=parts.Count||fileBars.Any(n=>n<1)||lineBars==null||lineBars.Length==0||lineBars.Any(n=>n<1)||lineLeft==null||lineLeft.Length!=lineBars.Length||lineBars.Sum()!=fileBars.Sum()||planPositions==null||planPositions.Length!=fileBars.Sum())throw new ArgumentException("PAGE requires complete explicit file and line bar counts and planning coordinates.");
   var header=parts[0].Items.FirstOrDefault(item=>item.Code==8);if(header==null||header.P(5)<=0||parts.Any(part=>part.Items.Any(item=>item.Staff!=1)))throw new NotSupportedException("This PAGE assembly entry requires extracted single-staff parts with a positive initial staff size.");
   var initial=NativePageTemplates.Initialize(NativePageSort.Apply(parts[0].Items,false),(float)((double)staffSize/header.P(5)),null);var collection=new NativePageCollection();var core=new NativePageLineLayout();var assembly=new NativePageAssemblyBank(lineBars.Length,1,height);var cuts=NativePageBarPlan.CutPositions(planPositions);int nextFile=0,available=0,used=0;
   for(int line=0;line<lineBars.Length;line++){
    int requested=lineBars[line];while(available<requested){if(nextFile>=parts.Count)throw new InvalidOperationException("PAGE exhausted the explicitly planned files.");collection.Append(parts[nextFile].Items);available+=fileBars[nextFile++];}
    used+=requested;var bank=collection.Records();var partition=available==requested?NativePagePartition.TakeAll(bank):NativePagePartition.Split(bank,(float)((double)cuts[used-1]+collection.Offset));
    var prepared=NativePageCoordinates.Fit(partition.Line,0,partition.End,lineLeft[line],200);var cleaned=NativePageInitialCleanup.Apply(prepared,initial.Spans);var inserted=initial.Templates.Insert(cleaned);var laid=core.Apply(NativePageSort.Apply(inserted,true),1,1,initial.Units,new float[4]);laid=NativePagePostLine.ApplyNegativeEndMode(laid,-2,0);laid=NativePageSlurAdjustments.Apply(laid,1,1);laid=NativePageRestFollowup.Apply(laid,initial.Units,1,1);assembly.Append(laid);
    available-=requested;float retainedOffset=0;var remainder=partition.Remainder;if(remainder.Count!=0){remainder=NativePageCoordinates.Rebase(remainder,partition.End);retainedOffset=(float)((double)collection.Offset-(float)((double)partition.End-10));}collection.Retain(remainder,retainedOffset);
   }
   return assembly.Finish(pageNumber,numberPage,numberX,numberY,numberFont);
  }
 }
}
