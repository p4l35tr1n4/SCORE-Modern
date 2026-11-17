using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ScoreModern {
 public sealed partial class Editor {
  bool TryEditTypedEnharmonic(string verb,string argument){
   if(verb!="ENH")return false;
   float requested=0;
   if(argument.Length!=0&&(!Single.TryParse(argument,NumberStyles.Float,CultureInfo.InvariantCulture,out requested)||
      Single.IsNaN(requested)||Single.IsInfinity(requested)||Math.Abs(requested)>100000))
    throw new InvalidDataException("Enter a finite ENH direction.");
   var note=canvas.Selected;
   if(note==null||note.Code!=1)throw new InvalidDataException("Select a note before changing its spelling.");
   int control=OriginalEnharmonicClef(note);
   int step=Math.Sign(requested);
   if(step==0){
    // SCORE 2FAC:0DC2–0DFA takes the accidental remainder modulo 10.
    int accidental=(int)(note.P(5)%10);
    step=accidental==2||accidental==5?1:-1;
   }
   ApplyOriginalEnharmonic(note,control,step);
   return true;
  }

  int OriginalEnharmonicClef(Item note){
   // SCORE 2FAC:0E21–0E9D scans in record order and replaces only for a closer clef.
   Item clef=null;
   float nearest=999;
   foreach(var item in score.Items){
    if(item.Code!=3||item.Staff!=note.Staff)continue;
    float distance=note.P(3)-item.P(3);
    if(distance>=0&&distance<nearest){nearest=distance;clef=item;}
   }
   int control=20;
   if(clef!=null){
    float clefType=clef.P(5);
    // The DOS dispatcher adds 20 to the stored clef type, then the numeric editor truncates it.
    float clefControl=20f+clefType;
    if(!(clefType>=0&&clefControl<24))
     throw new NotSupportedException("Typed ENH requires a treble, bass, alto or tenor clef control.");
    control=(int)clefControl;
   }
   return control;
  }

  bool TryEditExplicitEnharmonic(float[] values){
   int first=0;
   while(first<values.Length&&!IsExplicitEnharmonicControl(values[first]))first+=2;
   if(first>=values.Length)return false;
   var note=canvas.Selected;
   if(note==null||note.Code!=1)throw new InvalidDataException("Select a note before changing its spelling.");
   var edits=new List<float>();
   var working=note.Clone();
   for(int i=0;i<values.Length;i+=2){
    float control=values[i],value=i+1<values.Length?values[i+1]:0;
    if(IsExplicitEnharmonicControl(control)){
     int direction=Math.Sign(value);
     int delta;
     if(!TryOriginalEnharmonicDelta((int)control,working.P(4),working.P(5),direction,out delta))continue;
     // A successful rewrite replaces the rest of the numeric command buffer.
     edits.AddRange(new[]{400f,direction<0?-1f:1f,500f,delta});
     break;
    }
    bool additive=control>=300&&control<=1900&&control%100==0;
    bool copy=control>=3000&&control<=19000&&control%1000==0;
    float parameter=copy?control/1000:additive?control/100:control;
    if(parameter<3||parameter>19||parameter!=(int)parameter)
     throw new NotSupportedException("Mixed enharmonic edits support P3–P19 controls.");
    // SCORE 316D:07BE–07CB skips equivalences whose source is below P3.
    if(copy&&(value==1||value==2))continue;
    if(copy&&(value<3||value>19||value!=(int)value))
     throw new NotSupportedException("Mixed enharmonic edits can copy from P3–P19.");
    working.Set((int)parameter,copy?working.P((int)value):additive?working.P((int)parameter)+value:value);
    edits.Add(control);
    edits.Add(value);
   }
   if(edits.Count!=0)EditOriginalParameters(edits.ToArray());
   return true;
  }

  static bool IsExplicitEnharmonicControl(float control){
   return control>=20&&control<=23&&control==(int)control;
  }

  void ApplyOriginalEnharmonic(Item note,int control,int step){
   int accidentalDelta;
   if(TryOriginalEnharmonicDelta(control,note.P(4),note.P(5),step,out accidentalDelta))
    EditOriginalParameters(new[]{400f,step,500f,accidentalDelta});
  }

  // SCORE runtime segment 316D, offsets 036D–0592, emits additive controls 400 and 500.
  static bool TryOriginalEnharmonicDelta(int control,float pitch,float packedAccidental,int direction,out int delta){
   if(!(pitch>=Int16.MinValue&&pitch<=Int16.MaxValue&&packedAccidental>=Int16.MinValue&&packedAccidental<=Int16.MaxValue))
    throw new NotSupportedException("Enharmonic pitch or accidental is outside the verified range.");
   int clefOffset=control==21?2:control==22?1:control==23?-1:0;
   int pitchClass=((int)pitch%100+28+clefOffset)%7;
   int accidental=(int)packedAccidental%10;
   // Other remainders miss the named accidental branches and take the default delta.
   if(accidental<0||accidental>5)accidental=0;
   // A zero control raises P4 but selects the downward accidental branches, except for a double sharp.
   bool up=direction>0||direction==0&&accidental==5;
   bool narrow=up?pitchClass==0||pitchClass==3:pitchClass==1||pitchClass==4;
   delta=0;
   switch(accidental){
    case 0:case 3:
     delta=(up?(narrow?1:4):(narrow?2:5))-accidental;
     return true;
    case 1:
     if(up&&!narrow)return false;
     delta=up?3:narrow?2:1;
     return true;
    case 2:
     if(!up&&!narrow)return false;
     delta=!up?3:narrow?1:-1;
     return true;
    case 4:
     if(up)return false;
     delta=narrow?-3:-1;
     return true;
    case 5:
     if(!up)return false;
     delta=narrow?-3:-2;
     return true;
    default:
     throw new NotSupportedException("This packed accidental is outside the recovered enharmonic cases.");
   }
  }
 }
}
