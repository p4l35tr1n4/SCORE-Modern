using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativePageInitialization {
  internal NativePageTemplates Templates;internal List<Item> Input;
  internal float[] Units,Spans;
 }
 internal sealed partial class NativePageTemplates {
  // PAGE 0B45:0353..095D: initialize templates from the first extracted
  // system. A retained bank is explicit; null starts a new native workspace.
  internal static NativePageInitialization Initialize(IList<Item> input,float sizeRatio,byte[] retained){
   return InitializeMode(input,sizeRatio,retained,false);
  }
  internal static NativePageInitialization InitializeMode(IList<Item> input,float sizeRatio,byte[] retained,bool preserveInstrumentIds){
   if(input==null||input.Count==0||sizeRatio<=0||Single.IsNaN(sizeRatio)||Single.IsInfinity(sizeRatio))throw new ArgumentException("PAGE requires a nonempty first system and a positive finite size ratio.");
   var result=new NativePageInitialization{Input=input.Select(item=>item.Clone()).ToList(),Units=new float[34],Spans=new float[34]};
   var cache=new NativePageTemplates(retained??new byte[10756]);result.Templates=cache;
   Array.Clear(cache.bank,1540,9216);Array.Copy(BitConverter.GetBytes((short)999),0,cache.bank,0,2);Array.Copy(BitConverter.GetBytes((short)0),0,cache.bank,2,2);
   bool rhythmic=false;
   foreach(var item in result.Input){
    int staff=item.Staff;if(item.Code<=2)rhythmic=true;
    if(rhythmic){if(item.Code==14){if(staff<1||staff>32)throw new NotSupportedException("PAGE bar span leaves the staff table.");float span=item.Count>=4?item.P(4):1;if(BitConverter.ToInt32(BitConverter.GetBytes(span),0)==0)span=1;result.Spans[staff]=span;}continue;}
    short bottom=BitConverter.ToInt16(cache.bank,0),top=BitConverter.ToInt16(cache.bank,2);Array.Copy(BitConverter.GetBytes((short)Math.Min(bottom,staff)),0,cache.bank,0,2);Array.Copy(BitConverter.GetBytes((short)Math.Max(top,staff)),0,cache.bank,2,2);
    int type=item.Code==8?1:item.Code==3?5:item.Code==17?6:item.Code==18?7:0;
    if(item.Code==14){if(item.Count<=4)type=2;else{int style=(int)Math.Truncate(item.P(5));if(style==8||style==9)type=style-5;}}
    if(type==0)continue;
    if(type==1&&item.Count<5)item.Set(5,0);
    int words=Math.Min(item.Count+1,12);Array.Copy(item.Bytes,0,cache.bank,cache.Offset(type,staff),words*4);if(words==12)cache.Set(type,staff,0,11);if(type==5)cache.Set(type,staff,0,5);
    if(type==1){
     float size=cache.Get(1,staff,5);if(BitConverter.ToInt32(BitConverter.GetBytes(size),0)==0)size=1;double scaled=(double)sizeRatio*size;cache.Set(1,staff,5,(float)scaled);result.Units[staff]=(float)(scaled*25);
     cache.Set(1,staff,4,0);if(cache.Get(1,staff,8)<0)cache.Set(1,staff,8,0);if(!preserveInstrumentIds)cache.Set(1,staff,9,0);
    }
   }
   int first=BitConverter.ToInt16(cache.bank,0),last=BitConverter.ToInt16(cache.bank,2);if(first<1||last>32||first>last)throw new NotSupportedException("PAGE did not find valid initial staff bounds.");
   float origin=cache.Get(1,first,3);if(BitConverter.ToInt32(BitConverter.GetBytes(origin),0)!=0)for(int staff=first;staff<=last;staff++)for(int type=1;type<=7;type++){float x=cache.Get(type,staff,3);if(BitConverter.ToInt32(BitConverter.GetBytes(x),0)!=0)cache.Set(type,staff,3,(float)((double)x-origin));}
   return result;
  }
 }
}
