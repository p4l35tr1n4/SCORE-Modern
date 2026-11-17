using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativePageAssemblyBank {
  readonly int systems,staves;readonly float height;
  readonly List<Item> records=new List<Item>();int remaining,appended;bool finished;
  internal NativePageAssemblyBank(int systemCount,int stavesPerSystem,float controlHeight){if(systemCount<1||stavesPerSystem<1||systemCount*stavesPerSystem>32||Single.IsNaN(controlHeight)||Single.IsInfinity(controlHeight))throw new ArgumentException("PAGE assembly requires finite height and 1–32 destination staves.");systems=systemCount;staves=stavesPerSystem;height=controlHeight;remaining=systems*staves;}
  // Parts-mode path of PAGE 2E8F:0031..0215. It retains the complete record
  // payload; DEC 2CE6 converts its one-past extent to a word count, not a trim.
  internal List<Item> Append(IList<Item> input){
   if(finished||appended>=systems)throw new InvalidOperationException("This PAGE bank has already received its systems.");remaining-=staves;appended++;var line=input.Select(item=>item.Clone()).ToList();foreach(var item in line)item.Set(2,(float)((double)item.P(2)+remaining));records.AddRange(line.Select(item=>item.Clone()));return line;
  }
  // Final parts page: source 2E8F:0439..05EE, plus its separate number
  // constructor 0824..093F. Number placement/font remain explicit controls.
  internal List<Item> Finish(int pageNumber,bool numberPage,float numberX,float numberY,float numberFont,float retainedSingleStaffGap=0){
   if(finished||appended!=systems)throw new InvalidOperationException("Complete every system before finishing the PAGE bank.");finished=true;
   if(numberPage)records.Add(Item.Make(10,systems*staves,numberX,numberY,pageNumber,numberFont));
   var result=NativePageSort.Apply(records,true);
   float gap=(float)(((double)height/(systems*staves)-0.7873200178146362)/0.04374000057578087);
   float extra=0,within=staves>1?(float)(2.0/(staves-1)):retainedSingleStaffGap;int left=staves;
   foreach(var item in result){if(item.Code!=8)continue;left--;if(item.P(2)!=1){float size=item.Count>=5?item.P(5):1;if(BitConverter.ToInt32(BitConverter.GetBytes(size),0)==0)size=1;item.Set(4,(float)((((double)item.P(2)-1)*gap)/size-extra));if(left<=0){extra=0;left=staves;}}extra=(float)((double)within+extra);}
   return result;
  }
 }
}
