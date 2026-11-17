using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 // Ordinary measures with barlines spanning the entire system.
 internal sealed class NativePageMeasureNumbers {
  int current;
  readonly bool every;
  internal NativePageMeasureNumbers(int first,bool every){current=first==-1?0:first;this.every=every;}
  internal void Append(List<Item> line,int bars,float indent){
   var staffs=line.Where(i=>i.Code==8).OrderBy(i=>i.Staff).ToArray();
   var bottom=staffs.First();
   var top=staffs.Last();
   var ends=line.Where(i=>i.Code==14&&i.Staff==bottom.Staff&&i.P(4)==staffs.Length&&i.P(3)>top.P(3)).OrderBy(i=>i.P(3)).ToArray();
   if(ends.Length!=bars||ends.Any(i=>i.P(5)!=0)||ends[ends.Length-1].P(3)!=200)
    throw new NotSupportedException("PAGE measure numbering needs a verified ordinary bar sequence.");
   if(current>32767-bars)throw new NotSupportedException("PAGE measure numbers exceed the original signed-word range.");
   // PAGE 084F:0C93-0CD2 emits the opening label when the current measure exceeds one.
   if(current>1)line.Add(Item.Make(10,top.Staff,(float)((double)indent+.1f),15,current,201,1));
   if(every)for(int n=0;n<ends.Length-1;n++)line.Add(Item.Make(10,top.Staff,ends[n].P(3),15,current+n+1,201,1));
   current+=bars;
  }
 }
}
