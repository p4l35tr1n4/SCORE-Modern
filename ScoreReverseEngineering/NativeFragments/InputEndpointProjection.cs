using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputEndpointResult {public short StartFlag,EndFlag,StartItem,EndItem,Number,StartNote,EndNote,Stem;public float StartPitch,EndPitch,StartX,EndX;}
 public static class InputEndpointProjection {
  // Single-staff numerical projection of resident 01AE:0039 and
  // its 000E/033C endpoint leaves. Parameter words and encoded decimal controls
  // are kept separate. Cross-staff changes are handled by the 054E callee.
  public static InputEndpointResult Decode(float first,float last,int stage,IList<float[]> records,int[] noteToItem,int[] stems,float beginning,float end,InputEndpointStaffContext context=null){
   int a=Number(first),b=Number(last),rawB=b,bound=noteToItem.Length;
   var result=new InputEndpointResult{StartNote=(short)a,EndNote=(short)b};
   if(b==0)b=1;if(a==0||a==99)a=1;else if(b<=a){b=a+1;result.EndNote=(short)b;}if(b>=bound)b=bound-1;
   short firstNumber,firstStem,lastNumber,lastStem;Detail(first,a,stage,records,noteToItem,stems,out result.StartItem,out result.StartPitch,out result.StartFlag,out firstNumber,out firstStem,context);Detail(last,b,stage,records,noteToItem,stems,out result.EndItem,out result.EndPitch,out result.EndFlag,out lastNumber,out lastStem,context);result.Number=firstNumber!=0?firstNumber:lastNumber;result.Stem=firstStem!=0?firstStem:lastStem;
   result.StartX=records[result.StartItem-1][2];result.EndX=records[result.EndItem-1][2];
   if(stage==5){float fa=(float)((double)first%1),fb=(float)((double)last%1);int next=rawB+1;if(HasBits(fa)){a=result.StartNote;if(a>=bound)return result;float target;if(a==0){target=result.StartX;result.StartX=beginning;}else target=records[Mapped(noteToItem,a+1,records.Count)-1][2];result.StartX=(float)(((double)target-result.StartX)*fa+result.StartX);result.StartNote=result.EndNote=1;}
    if(HasBits(fb)){if(next<=a)next=a+1;float target;if(next<bound){if(rawB<=a)result.EndX=records[result.StartItem-1][2];target=records[Mapped(noteToItem,next,records.Count)-1][2];}else target=end;result.EndX=(float)(((double)target-result.EndX)*fb+result.EndX);result.StartNote=result.EndNote=1;}
   }
   return result;
  }
  // The original tests both stored words, so negative zero is present.
  static bool HasBits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  static int Word(float value){if(float.IsNaN(value)||float.IsInfinity(value)||value<=-32768||value>=32768)throw new ArgumentOutOfRangeException("value","Outside this finite endpoint projection.");return (short)Math.Truncate(value);}
  static int Number(float value){return Math.Abs(Word(value)%100);}
  static int Mapped(int[] map,int note,int count){if(note<1||note>=map.Length||map[note]<1||map[note]>count)throw new ArgumentOutOfRangeException("note","No verified endpoint mapping for this item.");return map[note];}
  static void Detail(float encoded,int note,int stage,IList<float[]> records,int[] map,int[] stems,out short item,out float pitch,out short flag,out short number,out short stem,InputEndpointStaffContext context){
   int word=Word(encoded);stem=0;if(stage!=5&&stems!=null){if(note>=stems.Length)throw new ArgumentException("Stem map length");stem=(short)Math.Abs(stems[note]);}if(stem==0){if(word<0)stem=20;if(word>9999)stem=10;}number=(short)((Math.Abs(word)%10000)/100);item=(short)Mapped(map,note,records.Count);int selected=item-1;if(stage!=5)while(selected+1<records.Count&&records[selected+1][0]==1&&records[selected+1][14]<0)selected++;var p=records[selected];pitch=p[3];flag=0;if(Math.Abs(pitch)>=100){if(p[6]>=64)flag=1;pitch=(float)((double)pitch-(pitch<0?-100:100));}if(p[11]!=0){if(context==null)throw new NotSupportedException("Cross-staff endpoint requires its measured staff-table context.");context.Adjust(p,ref pitch);}
  }
 }
}
