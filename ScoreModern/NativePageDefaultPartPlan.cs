using System;
using System.Linq;
using System.Collections.Generic;
namespace ScoreModern {
 internal static class NativePageDefaultPartPlan {
  // PAGE 0FF5:0C2A..0D58: retained REAL4 scale, rounded integer bar
  // weights, page line capacity and the original .6 line-count bias.
  // PAGE adds the first indent to the total and reduces the target line
  // width by the continuation indent before calculating the line count.
  internal static List<NativePagePartPage> Prepare(IList<Score> parts,int staves,float size,float height){
   return PrepareWithNumbering(parts,staves,size,height,1,false);
  }
  internal static List<NativePagePartPage> PrepareWithNumbering(IList<Score> parts,int staves,float size,float height,int firstPage,bool centered){
   return PrepareProposal(parts,staves,size,height,firstPage,centered,0,0);
  }
  internal static List<NativePagePartPage> PrepareProposal(IList<Score> parts,int staves,float size,float height,int firstPage,bool centered,int pageCount,int linesPerPage){
   return PrepareRangeProposal(parts,staves,size,height,firstPage,centered,0,0,pageCount,linesPerPage);
  }
  internal static List<NativePagePartPage> PrepareRangeProposal(IList<Score> parts,int staves,float size,float height,int firstPage,bool centered,int skip,int take,int pageCount,int linesPerPage,int firstIndent=0,int otherIndent=0){
   if(staves<1||staves>32||Single.IsNaN(size)||Single.IsInfinity(size)||size<=0||Single.IsNaN(height)||Single.IsInfinity(height)||height<=0)throw new NotSupportedException("PAGE default plan leaves its original positive-control/staff bank.");
   var raw=NativePagePartWeightCollection.Prepare(parts);
   if(take!=0){raw.Widths=raw.Widths.Skip(skip).Take(take).ToArray();raw.Leading=raw.Leading.Skip(skip).Take(take).ToArray();}
   float scale=(float)((double)size*.009999999776482582);var weights=new short[raw.Widths.Length];int total=0;
   for(int n=0;n<weights.Length;n++){double value=Math.Truncate((double)Math.Abs((int)raw.Widths[n])*scale+.5);if(value<0||value>32767)throw new NotSupportedException("PAGE scaled bar weight leaves its signed-word domain.");weights[n]=(short)value;total+=weights[n];}
   total+=firstIndent;
   int openingWeight=weights[0]+firstIndent-otherIndent;
   if(total<1||total>32767||openingWeight<0||openingWeight>32767)throw new NotSupportedException("PAGE indented weights leave the positive signed-word domain.");
   weights[0]=(short)openingWeight;
   int lines=(int)Math.Truncate(total/(200.0-otherIndent)+.6000000238418579);if(lines==0)lines=1;float systemSize=(float)((double)staves*size);int capacity=(int)Math.Truncate((double)height/systemSize);if(capacity==0)capacity=1;
   if(pageCount!=0||linesPerPage!=0){
    if(pageCount<1||pageCount>100||linesPerPage<1||linesPerPage>32/staves||pageCount*linesPerPage>weights.Length)
     throw new ArgumentException("Choose a positive page count and lines per page within the number of bars and the original staff capacity.");
    lines=pageCount*linesPerPage;capacity=linesPerPage;
   }
   var layout=new NativePageAutomaticLayout().Apply(weights,raw.Leading,lines,capacity,height,1,proposalTotal:total);
   return NumberedPages(GroupLines(layout,weights.Length),height,firstPage,centered);
  }
  internal static List<int[]> GroupLines(NativePageAutomaticResult layout,int barCount){
   var groups=new List<int[]>();var current=new List<int>();
   for(int n=1;n<layout.PageList.Length;n++){int bars=layout.PageList[n];if(bars==0){if(current.Count!=0){groups.Add(current.ToArray());current.Clear();}}else{if(bars<0)throw new NotSupportedException("PAGE default proposal produced a negative line count.");current.Add(bars);}}
   if(current.Count!=0)groups.Add(current.ToArray());if(groups.Sum(g=>g.Sum())!=barCount)throw new InvalidOperationException("PAGE page list does not account for the complete part.");
   return groups;
  }
  internal static List<NativePagePartPage> NumberedPages(IList<int[]> groups,float height,int firstPage,bool centered){
   if(firstPage<0||firstPage>32767||Math.Max(1,firstPage)>32768-groups.Count)throw new NotSupportedException("PAGE numbering leaves the original signed-word range.");
   return groups.Select((bars,n)=>{
    int number=Math.Max(1,firstPage)+n;
    float x=centered?(number>=100?96.6f:number>=10?98.3f:99.5f):number%2!=0?196f:4f;
    return new NativePagePartPage(bars,Enumerable.Repeat(20f,bars.Length).ToArray(),height,number,firstPage!=0,x,24,201.1f);
   }).ToList();
  }
 }
}
