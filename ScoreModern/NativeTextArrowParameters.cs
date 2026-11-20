using System;using System.IO;
namespace ScoreModern {public sealed partial class Editor {
 Item textArrowItem;readonly float[] textArrowTail=new float[6];
 void ResetOriginalTextArrowState(Item item){textArrowItem=item;Array.Clear(textArrowTail,0,textArrowTail.Length);}
 float OriginalArrowValue(int parameter){return OriginalArrowValueFor(canvas.Selected,parameter);}
 float OriginalArrowValueFor(Item item,int parameter){if(item!=null&&item.Code==16&&parameter>=14&&parameter<=19)return Object.ReferenceEquals(textArrowItem,item)?textArrowTail[parameter-14]:0;return item==null?0:item.P(parameter);}
 void EditOriginalArrowParameter(int parameter,float value){
  var item=canvas.Selected;if(item==null||item.Code!=16||parameter<12){EditOriginalParameters(new[]{(float)parameter,value});return;}
  if(!Object.ReferenceEquals(textArrowItem,item))ResetOriginalTextArrowState(item);
  if(parameter==12&&(Single.IsNaN(value)||Single.IsInfinity(value)||value!=Math.Truncate(value)||value<0||value>Math.Min(160,item.Bytes.Length-56)))throw new NotSupportedException("This text-length arrow would read outside the captured text buffer. Use TX to supply the text; retained-buffer extension still requires verification.");
  Checkpoint();if(parameter==12)item.Set(12,value);else if(parameter>=14&&parameter<=19)textArrowTail[parameter-14]=value;
  // Converter recalculates P13 and appends its measured space terminator.
  // P14–P19 are working numeric words; packed text replaces them on storage.
  RefreshAll(item);
 }
}}
