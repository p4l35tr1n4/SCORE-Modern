using System;using System.Collections.Generic;using System.Globalization;using System.IO;

using System.Linq;

namespace ScoreModern {

 public sealed partial class Editor {

  sealed class OriginalParameterOperation {public int Parameter,Source;public float Value;public bool Add,Copy;}

  void EditOriginalParameters(float[] values){

   // Original C103 and M801: a leading "9 -1" is the Code10/18 CM shortcut. It
   // returns before later pairs, and preserves the prior P9 value. A later
   // pair assigning P9=-1 remains an ordinary stored assignment.
   if(canvas.Selected!=null&&(canvas.Selected.Code==10||canvas.Selected.Code==18)&&values.Length>=2&&values[0]==9&&values[1]==-1){CenterOriginalBarOrMeter("");return;}

   if(scor4Interface&&canvas.Selected!=null&&canvas.Selected.Code==6&&values.Length==2&&values[1]==999&&(values[0]==11||values[0]==14)){
    FlipScor4InnerBeam((int)values[0]);return;
   }

   if(canvas.Selected!=null&&canvas.Selected.Code==6&&values.Length>=2&&(values[0]==8||values[0]==9)&&values[1]<0){
    var plan=NativeBeamTemporaryEdits.Prepare(score.Items,canvas.Selected,values[0],values[1]);if(plan.Pending!=null){StageBeamTemporary(plan,values);return;}if(values[0]==9&&values.Length>2){var expanded=new float[plan.Operations.Length+values.Length-2];Array.Copy(plan.Operations,expanded,plan.Operations.Length);Array.Copy(values,2,expanded,plan.Operations.Length,values.Length-2);values=expanded;}else values=plan.Operations;
   }
   bool ignoredTextControl=false;var operations=new List<OriginalParameterOperation>();for(int k=0;k<values.Length;k+=2){float word=values[k],value=k+1<values.Length?values[k+1]:0;if(word!=(int)word)throw new InvalidDataException("Enter an integer parameter control.");int control=(int)word;if((control>=20&&control<=23)||(control>=2000&&control<=2300&&control%100==0))throw new NotSupportedException("Enharmonic controls P20-P23 still require original verification.");var op=new OriginalParameterOperation{Value=value};if(control>=1&&control<=19)op.Parameter=control;else if(control%100==0&&control/100>=1&&control/100<=19){op.Parameter=control/100;op.Add=true;}else if(control%1000==0&&control/1000>=1&&control/1000<=19){op.Parameter=control/1000;op.Copy=true;if(value!=(int)value||value<1||value>19)throw new InvalidDataException("Enter a source parameter from 1 to 19.");op.Source=(int)value;}else throw new NotSupportedException("This parameter control still needs original verification.");if(op.Copy&&op.Source<3)continue;if(canvas.Selected.Code==16&&op.Parameter>=12){ignoredTextControl=true;continue;}operations.Add(op);}

   if(operations.Count==0){if(ignoredTextControl){Checkpoint();RefreshAll(canvas.Selected);}return;}

   using(var display=BeginDisplayNoteParameters(operations)){Checkpoint();canvas.Selected.ConsumedCenteringParameterCount=0;foreach(var op in operations){float value=op.Copy?canvas.Selected.P(op.Source):op.Add?canvas.Selected.P(op.Parameter)+op.Value:op.Value;if(op.Parameter==9&&value==-1&&canvas.Selected.Code==2){var plan=NativeRestCentering.PlanItem(score,canvas.Selected);NativeRestCentering.Apply(plan);}else canvas.Selected.Set(op.Parameter,value);}ConsumeOriginalMarkCentering();dirty=true;if((canvas.Selected.Code==11&&canvas.Selected.P(13)==1)||(canvas.Selected.Code==15&&canvas.Selected.P(13)!=0)){canvas.Selected.ExternalDrawingResetResolved=false;BeginExternalDrawingName(canvas.Selected);return;}RefreshAll(canvas.Selected);display.Complete();}

  }

  void ConsumeOriginalMarkCentering(){
   var item=canvas.Selected;if(item==null||item.P(1)!=1||item.P(13)!=99)return;
   var parameters=new float[19];for(int n=0;n<parameters.Length;n++)parameters[n]=item.P(n+1);
   if(ScoreModern.RecoveredFragments.InputStaccatoCentering.Apply(parameters,new NativePageDocument(score).Tables.Scales)){item.ConsumedCenteringParameterCount=13;item.Set(13,parameters[12]);}
  }

  void CenterOriginalBarOrMeter(string argument){

   if(mode!=ScreenMode.Edit||canvas.Selected==null)return;

   var item=canvas.Selected;

   bool meter=item.Code==18,alignBar=item.Code==14,key=scor4Interface&&item.Code==17;

   if(!meter&&!alignBar&&!key&&item.Code!=10)return;

   int targetStaff=0;

   if(argument.Length!=0){

    float staff;

    if(!Single.TryParse(argument,NumberStyles.Float,CultureInfo.InvariantCulture,out staff)||!(staff>=0&&staff<=32))

     throw new NotSupportedException("CM requires a staff number from 0 to 32.");

    targetStaff=(int)staff;

   }

   float size=meter?item.P(7):item.P(6)%100;

   if(BitConverter.ToInt32(BitConverter.GetBytes(size),0)==0)size=1;

   if(meter){

    if((double)item.P(4)*size<2){canvas.EditModified=true;return;}

    if(BitConverter.ToInt32(BitConverter.GetBytes(item.P(8)),0)!=0){
     pendingEditAcknowledgement=true;
     Message("*** Cannot center double meters"+Environment.NewLine+"Press any key to continue.");
     return;
    }

   }



   // Original overlay 2C0C:010D–0251 searches by stored P3 and the

   // uppermost staff of each bar. An equal distance replaces the old match.

   int staffLimit=(short)(int)((double)item.P(2)+1);

   float distance=15;

   Item nearest=null;

   foreach(var bar in score.Items){

    if(bar.Code!=14)continue;

    if(alignBar){

     if(bar.Staff!=targetStaff)continue;

    }else{

     if(bar.Staff>=staffLimit)continue;

     int span=BitConverter.ToInt32(BitConverter.GetBytes(bar.P(4)),0)==0?1:(short)(int)bar.P(4);

     if(bar.Staff+span!=staffLimit)continue;

    }

    float delta=(float)Math.Abs((double)item.P(3)-bar.P(3));

    if(delta<=distance){nearest=bar;distance=delta;}

   }

   if(nearest==null){canvas.EditModified=true;return;}

   if(alignBar){

    Checkpoint();

    item.Set(3,nearest.P(3));

    RefreshAll(item);

    NativeDosViewport.Advance(canvas);

    return;

   }



   NativeNotationContext page;

   if(!canvas.PageDocument.Contexts.TryGetValue(item,out page))throw new NotSupportedException("CM needs a verified staff context.");

   if(key){
    float control=Math.Abs(item.P(5));
    float count=control%100;
    float clef=item.P(6);
    if(control>=200||count>7||count!=Math.Truncate(count)||clef<0||clef>=200||clef%100>3||clef!=Math.Truncate(clef)||
       Enumerable.Range(8,6).Any(p=>item.P(p)!=0)||item.P(16)!=0)
     throw new NotSupportedException("This key-signature centering combination still needs SCORE 4 verification.");
    double staffSize=page.Scale/25.0;
    float spacing=item.P(7)==0?1f:item.P(7);
    EditOriginalParameters(new[]{3f,nearest.P(3),15f,(float)((.8f-staffSize*count*1.41f)*spacing)});
    return;
   }

   // Overlay 2C0C:0254–03D5 selects the width factor from DS:7046–7072.

   float width;

   if(meter){

    float top=item.P(5),bottom=item.P(6);

    width=.066f;

    if(top<98){

     if(top>9||bottom>9)width=.122f;

     if(top>19||bottom>19)width=.132f;

    }

   }else{

    int number=(short)(int)item.P(5);

    width=number<=1?.038f:number<=9?.046f:number==11?.084f:number<=99?.096f:number==101||number==111?.140f:.148f;

   }

   Checkpoint();

   item.Set(3,nearest.P(3));

   item.Set(15,(float)(-(double)size*page.Scale*width));

   RefreshAll(item);

   NativeDosViewport.Advance(canvas);

  }

  bool ShowOriginalParameter(string verb,string rest){if(mode!=ScreenMode.Edit||canvas.Selected==null)return false;bool query=verb=="P"||verb.Length>1&&verb[0]=='P'&&(Char.IsDigit(verb[1])||verb[1]=='-'||verb[1]=='+'||verb[1]=='.');if(!query)return false;string text=verb=="P"?rest:verb.Substring(1);int n;if(!Int32.TryParse(text,out n)||n<1||n>20)throw new NotSupportedException("Enter a parameter number from 1 to 20.");pendingEditAcknowledgement=true;Message("Parameter"+n.ToString(CultureInfo.InvariantCulture).PadLeft(3)+" was "+ExportFixed(editOriginal==null?canvas.Selected.P(n):editOriginal.P(n),4).PadLeft(11)+"  Press any key to continue.");return true;}

 }

}

