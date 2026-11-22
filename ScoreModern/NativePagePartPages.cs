using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativePagePartPage {
  internal readonly int[] Bars;internal readonly float[] Left;internal readonly float Height;
  internal float[] Indents;
  internal readonly int Number;internal readonly bool NumberEnabled;internal readonly float NumberX,NumberY,NumberFont;
  internal NativePagePartPage(int[] bars,float[] left,float height,int number,bool numberEnabled,float numberX,float numberY,float numberFont){Bars=bars==null?null:(int[])bars.Clone();Left=left==null?null:(float[])left.Clone();Height=height;Number=number;NumberEnabled=numberEnabled;NumberX=numberX;NumberY=numberY;NumberFont=numberFont;}
 }
 internal static class NativePagePartPages {
  // Shared original workspaces persist across page boundaries. Layout and
  // numbering controls are explicit, independent of the automatic proposal.
  internal static List<List<Item>> Assemble(IList<Score> parts,int staves,float size,IList<NativePagePartPage> pages,float[] afterSymbol,float control43E0,short control43E4,Action<int,string,IList<Item>> observe=null){
   return AssembleWithWarnings(parts,staves,size,pages,afterSymbol,control43E0,control43E4,observe,null);
  }
  internal static List<List<Item>> AssembleWithWarnings(IList<Score> parts,int staves,float size,IList<NativePagePartPage> pages,float[] afterSymbol,float control43E0,short control43E4,Action<int,string,IList<Item>> observe,Action<string> warnings){
   return AssembleWithNumbering(parts,staves,size,pages,afterSymbol,control43E0,control43E4,observe,warnings,null);
  }
  internal static List<List<Item>> AssembleWithNumbering(IList<Score> parts,int staves,float size,IList<NativePagePartPage> pages,float[] afterSymbol,float control43E0,short control43E4,Action<int,string,IList<Item>> observe,Action<string> warnings,NativePageMeasureNumbers numbering){
   return AssembleRange(parts,staves,size,pages,afterSymbol,control43E0,control43E4,observe,warnings,numbering,0,0);
  }
  internal static List<List<Item>> AssembleRange(IList<Score> parts,int staves,float size,IList<NativePagePartPage> pages,float[] afterSymbol,float control43E0,short control43E4,Action<int,string,IList<Item>> observe,Action<string> warnings,NativePageMeasureNumbers numbering,int skip,int take){
   return AssembleTransposed(parts,staves,size,pages,afterSymbol,control43E0,control43E4,observe,warnings,numbering,skip,take,null);
  }
  internal static List<List<Item>> AssembleTransposed(IList<Score> parts,int staves,float size,IList<NativePagePartPage> pages,float[] afterSymbol,float control43E0,short control43E4,Action<int,string,IList<Item>> observe,Action<string> warnings,NativePageMeasureNumbers numbering,int skip,int take,NativePageTranspose transpose,bool preserveInstrumentIds=false,float[] savedPositions=null,bool scor4=false){
   if(staves<1||staves>32||parts==null||parts.Count==0||parts.Any(part=>part.Items.Any(item=>item.Staff<1||item.Staff>staves)))throw new NotSupportedException("PAGE assembly leaves its original staff bank.");
   if(pages==null||pages.Count==0||pages.Any(page=>page.Bars==null||page.Bars.Length==0||page.Bars.Any(n=>n<1)||page.Left==null||page.Left.Length!=page.Bars.Length||page.Indents!=null&&page.Indents.Length!=page.Bars.Length)||afterSymbol==null||afterSymbol.Length!=4)throw new ArgumentException("PAGE requires explicit page, line and after-symbol controls.");
   if(transpose!=null)transpose.ReportWarning=warnings;
   var plan=NativePagePartWeightCollection.Prepare(parts).BarPlan;int total=plan.FileBars.Sum();
   if(take==0)take=total;
   if(skip<0||take<1||skip>total-take||pages.Sum(page=>page.Bars.Sum())!=take)throw new ArgumentException("PAGE layout must account for every planned bar.");
   var first=NativePageSort.Apply(parts[0].Items,false);var header=first.FirstOrDefault(item=>item.Code==8);if(header==null||header.P(5)<=0)throw new NotSupportedException("PAGE requires a positive initial staff size.");
   var initial=NativePageTemplates.InitializeMode(first,(float)((double)size/header.P(5)),null,preserveInstrumentIds);var collection=new NativePageCollection();var core=new NativePageLineLayout(scor4);var cuts=NativePageBarPlan.CutPositions(savedPositions??plan.Positions);var output=new List<List<Item>>();int nextFile=0,available=0,used=skip,lineNumber=0;
   int remainingSkip=skip;
   while(remainingSkip>=plan.FileBars[nextFile])remainingSkip-=plan.FileBars[nextFile++];
   foreach(var page in pages){var assembly=new NativePageAssemblyBank(page.Bars.Length,staves,page.Height);
    for(int line=0;line<page.Bars.Length;line++){
     int requested=page.Bars[line];while(available<requested){
      if(nextFile>=parts.Count)throw new InvalidOperationException("PAGE exhausted the planned input files.");
      var records=parts[nextFile].Items;
      if(remainingSkip>0){
       float start=plan.Positions[skip-1];
       if(records.Count==0||records.Any(i=>i.Code==14&&i.P(3)<=start&&i.P(5)!=0&&
        !(staves>1&&i.Staff==1&&i.P(4)==staves&&i.P(5)==8&&
          records.Any(opening=>opening.Code==8&&opening.Staff==1&&opening.P(3)==i.P(3)))))
        throw new NotSupportedException("PAGE range start requires verified ordinary bars.");
       records=NativePageSort.Apply(records,false).Where(i=>i.P(3)>start).ToList();
      }
      collection.Append(records);available+=plan.FileBars[nextFile++]-remainingSkip;remainingSkip=0;
     }
     used+=requested;var bank=collection.Records();NativePageCurveJoin.Apply(bank);var partition=available==requested?NativePagePartition.TakeAll(bank):NativePagePartition.Split(bank,(float)((double)cuts[used-1]+collection.Offset));
     var fitted=NativePageCoordinates.Fit(partition.Line,0,partition.End,page.Left[line],200);var cleaned=NativePageInitialCleanup.Apply(fitted,initial.Spans);var inserted=initial.Templates.Insert(cleaned);
     PositionOpeningRepeat(inserted);
     float indent=page.Indents==null?0:page.Indents[line];
     if(indent!=0)inserted=NativePageCoordinates.Fit(inserted,0,200,indent,200); // 084F:085E..08CD
     if(transpose!=null)transpose.RetainSortScratch(inserted);
     var ready=NativePageSort.Apply(inserted,true);
     if(transpose!=null){
      int count=ready.Count;ready=transpose.Apply(ready);
      if(ready.Count>count)ready=NativePageSort.Apply(ready,true);
     }
     lineNumber++;if(observe!=null)observe(lineNumber,"before",ready);var laid=core.ApplyWithWarnings(ready,1,(short)staves,initial.Units,afterSymbol,warnings==null?null:(Action<string>)(message=>warnings("Line "+lineNumber+Environment.NewLine+message)));if(observe!=null)observe(lineNumber,"after",laid);laid=NativePagePostLine.ApplyNegativeEndMode(laid,-2,0);laid=NativePageSlurAdjustments.Apply(laid,1,staves);laid=NativePageRestFollowup.Apply(laid,initial.Units,control43E0,control43E4);if(numbering!=null)numbering.Append(laid,requested,indent);assembly.Append(laid);
     available-=requested;float retainedOffset=0;var remainder=partition.Remainder;if(remainder.Count!=0){remainder=NativePageCoordinates.Rebase(remainder,partition.End);retainedOffset=(float)((double)collection.Offset-(float)((double)partition.End-10));}collection.Retain(remainder,retainedOffset);
    }
    output.Add(assembly.Finish(page.Number,page.NumberEnabled,page.NumberX,page.NumberY,page.NumberFont));
   }return output;
  }
  static void PositionOpeningRepeat(List<Item> items){
   if(!items.Any(i=>i.Code==14&&i.Count>=5&&i.P(5)==4))return;
   // PAGE 084F:0386..0464 scans staff-sorted records. It moves the first
   // opening repeat to one half unit before the first note or rest.
   var ordered=NativePageSort.Apply(items,true);
   int firstRhythm=ordered.FindIndex(i=>i.Code<=2);
   if(firstRhythm<0)return;
   var repeat=ordered.Take(firstRhythm).FirstOrDefault(i=>i.Code==14&&i.Count>=5&&i.P(5)==4);
   if(repeat==null)return;
   repeat.Set(3,(float)((double)ordered[firstRhythm].P(3)-.5));
   items.Clear();items.AddRange(NativePageSort.Apply(ordered,true));
  }
 }
}

