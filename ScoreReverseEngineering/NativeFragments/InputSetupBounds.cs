using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputSetupBoundsResult {public int Left,Right,Clef;public string Key;public int MeterTop,MeterBottom;}
 public static class InputSetupBounds {
  static float P(float[] row,int index){return row.Length>=index?row[index-1]:0;}
  static int W(float n){if(float.IsNaN(n)||float.IsInfinity(n)||n<short.MinValue||n>short.MaxValue)throw new NotSupportedException("Setup bound outside finite signed-word projection");return (short)Math.Truncate(n);}
  // 28b9:184a is the runtime ANINT-to-integer wrapper. Ordinary positive
  // coordinates have direct debugger fixtures; extreme/fault inputs remain
  // separate from this setup projection.
  static int Nearest(double n){double value=Math.Round(n,MidpointRounding.AwayFromZero);if(double.IsNaN(value)||value<short.MinValue||value>short.MaxValue)throw new NotSupportedException("Setup bound conversion outside signed word");return (short)value;}
  public static InputSetupBoundsResult Find(IList<float[]> records,InputSetupState state){
   var result=new InputSetupBoundsResult{Left=state.LeftPosition,Right=state.RightPosition,Key=state.Key,MeterTop=state.MeterTop,MeterBottom=state.MeterBottom};int maximum=-10,right=218,key=0,top=0,bottom=0;
   if(records.Count==0)return result;
   foreach(var row in records){int code=W(P(row,1)),staff=W(P(row,2));if(staff!=state.Staff)continue;if(code==3)result.Clef=W(P(row,5));float position=P(row,3);int x=Nearest(position);
    if(code==8){maximum=Math.Max(maximum,x);int end=row.Length<6?200:W(P(row,6));if(end==0)end=200;right=Math.Min(right,end);continue;}
    if(code!=3&&code<17&&code!=14)continue;
    if(code==14&&x>=50&&x<=159)continue;
    if(x>=100){if(x>=150)right=Math.Min(right,x);continue;}
    float width;
    if(code==3)width=10.239999771118164f;
    else if(code==14)width=1.399999976158142f;
    else if(code==17&&row.Length>4){float value=P(row,5);width=(float)((Math.Abs((double)value%100)-1)*2.799999952316284+2+1);key=W(value);}
    else{width=6;if(row.Length>4)top=W(P(row,5));if(row.Length>5)bottom=W(P(row,6));}
    maximum=Math.Max(maximum,Nearest((double)state.Size*width+position));
   }
   if(key!=0&&key>=-7&&key<=7){string[] names={"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#"};result.Key=names[key+7];}
   if(top!=0){result.MeterTop=top;result.MeterBottom=bottom;}
   result.Left=maximum==-10?state.LeftEnd:maximum;result.Right=right==218?state.RightEnd:right;return result;
  }
 }
}
