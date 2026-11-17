using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  float? itemAlignmentPosition;
  float editArrowIncrement=1;
  int editEndpoint,editParameter;

  bool HandleOriginalEditMovementKey(KeyEventArgs key){
   if(canvas.Selected!=null&&editParameter==0&&key.Modifiers==Keys.None&&(key.KeyCode==Keys.Left||key.KeyCode==Keys.Right||key.KeyCode==Keys.Up||key.KeyCode==Keys.Down)){key.SuppressKeyPress=true;MoveSelected(key.KeyCode==Keys.Left?-editArrowIncrement:key.KeyCode==Keys.Right?editArrowIncrement:0,key.KeyCode==Keys.Up?editArrowIncrement:key.KeyCode==Keys.Down?-editArrowIncrement:0);return true;}
   if(canvas.Selected!=null&&editParameter==0&&(key.Modifiers==Keys.None||key.Modifiers==Keys.Shift)){
    if(key.Modifiers==Keys.None&&(key.KeyCode==Keys.PageUp||key.KeyCode==Keys.PageDown)){key.SuppressKeyPress=true;MoveSelected(0,key.KeyCode==Keys.PageUp?NativeKeyboardOptions.ItemPageStep:-NativeKeyboardOptions.ItemPageStep);return true;}
    if(key.KeyCode==Keys.Tab){key.SuppressKeyPress=true;MoveSelected(key.Shift?-NativeKeyboardOptions.ItemTabStep:NativeKeyboardOptions.ItemTabStep,0);return true;}
    if(key.Modifiers==Keys.None&&key.KeyCode==Keys.Insert){key.SuppressKeyPress=true;Function(3);return true;}
    if(key.Modifiers==Keys.None&&key.KeyCode==Keys.End){key.SuppressKeyPress=true;RecordSharedLine("");command.Clear();ExitEdit(false);return true;}
   }
   if(key.Modifiers==Keys.Alt&&key.KeyCode>=Keys.D0&&key.KeyCode<=Keys.D9){
    int selected=key.KeyCode==Keys.D0?10:key.KeyCode-Keys.D0;
    if(editParameter==1){selected+=10;if(selected==20)selected=10;}
    editParameter=selected==editParameter?0:selected;editEndpoint=0;
    key.SuppressKeyPress=true;UpdateClassic();return true;
   }
   if(editParameter!=0&&key.Modifiers==Keys.None){
    if(key.KeyCode==Keys.PageUp||key.KeyCode==Keys.PageDown){key.SuppressKeyPress=true;float change=key.KeyCode==Keys.PageUp?NativeKeyboardOptions.ItemPageStep:-NativeKeyboardOptions.ItemPageStep;Safe(()=>EditOriginalArrowParameter(editParameter,OriginalArrowValue(editParameter)+change));return true;}
    if(key.KeyCode==Keys.Up||key.KeyCode==Keys.Down){key.SuppressKeyPress=true;float change=key.KeyCode==Keys.Up?editArrowIncrement:-editArrowIncrement;Safe(()=>EditOriginalArrowParameter(editParameter,OriginalArrowValue(editParameter)+change));return true;}
    if(key.KeyCode==Keys.Left||key.KeyCode==Keys.Right){key.SuppressKeyPress=true;editArrowIncrement=(float)((double)editArrowIncrement+.10000000149011612);if(key.KeyCode==Keys.Left)editArrowIncrement=(float)((double)editArrowIncrement-.20000000298023224);UpdateClassic();return true;}
   }

   if(key.Modifiers==Keys.Shift&&key.KeyCode==Keys.F10&&canvas.Selected!=null&&
      (canvas.Selected.Code==5||canvas.Selected.Code==6)){
    key.SuppressKeyPress=true;
    editEndpoint=scor4Interface&&canvas.Selected.Code==5&&editEndpoint==3?0:3;editParameter=0;
    UpdateClassic();
    return true;
   }
   if(key.Modifiers==Keys.Shift&&key.KeyCode==Keys.F9&&editEndpoint==3){
    key.SuppressKeyPress=true;editEndpoint=0;UpdateClassic();return true;
   }
   if(key.Modifiers==Keys.None&&(key.KeyCode==Keys.F9||key.KeyCode==Keys.F10)){
    key.SuppressKeyPress=true;
    var item=canvas.Selected;
    if(item==null||item.Code<4||item.Code>8)
     Message("Endpoint movement requires a line, slur, beam, ornament or staff.");
    else{editEndpoint=key.KeyCode==Keys.F9?1:2;editParameter=0;UpdateClassic();}
    return true;
   }
   if(key.Modifiers==Keys.None&&key.KeyCode==Keys.Home&&(editEndpoint!=0||editParameter!=0)){
    key.SuppressKeyPress=true;
    editEndpoint=0;editParameter=0;
    UpdateClassic();
    return true;
   }
   if(key.KeyCode==Keys.I&&(key.Modifiers==Keys.Control||key.Modifiers==Keys.Alt)){
    key.SuppressKeyPress=true;
    if(key.Control)editArrowIncrement=1;
    pendingEditAcknowledgement=true;
    Message(ArrowIncrementMessage()+"\r\nPress any key to continue.");
    return true;
   }
   if(key.Modifiers!=Keys.Control)return false;
   string move;
   switch(key.KeyCode){
    case Keys.L:move="L";break;
    case Keys.R:move="R";break;
    case Keys.U:move="U";break;
    case Keys.D:move="D";break;
    default:return false;
   }
   key.SuppressKeyPress=true;
   Safe(()=>Execute(move));
   return true;
  }

  string ArrowIncrementMessage(){
   return "*** Arrow edit increment = "+editArrowIncrement.ToString("0.0000",CultureInfo.InvariantCulture);
  }

  bool ExecuteEditMovement(string verb,string arguments){
   if(scor4Interface){
    switch(verb){
     case "LL":RequireNoLetterArguments(verb,arguments);MoveSelected(-.5f,0);return true;
     case "RR":RequireNoLetterArguments(verb,arguments);MoveSelected(.5f,0);return true;
     case "UU":RequireNoLetterArguments(verb,arguments);MoveSelected(0,.5f);return true;
     case "DD":RequireNoLetterArguments(verb,arguments);MoveSelected(0,-.5f);return true;
     case "MKL":case "MKR":case "MKU":case "MKD":
      RequireNoLetterArguments(verb,arguments);
      var note=canvas.Selected;
      if(note!=null&&note.Code==1&&note.P(11)!=0){
       int parameter=verb=="MKL"||verb=="MKR"?13:14;
       float offset=verb=="MKL"||verb=="MKD"?-.5f:.5f;
       EditOriginalParameters(new[]{(float)parameter,note.P(parameter)+offset});
      }
      return true;
    }
   }
   if(verb=="INC"){
    if(arguments.Length!=0){
     float increment;
     if(!Single.TryParse(arguments,NumberStyles.Float,CultureInfo.InvariantCulture,out increment)||
        Single.IsNaN(increment)||Single.IsInfinity(increment))
      throw new InvalidDataException("Enter a finite arrow increment after INC.");
     editArrowIncrement=increment;
    }
    pendingEditAcknowledgement=true;Message(ArrowIncrementMessage()+Environment.NewLine+"Press any key to continue.");
    return true;
   }
   if(verb.Length!=2||"LRUD".IndexOf(verb[0])<0||"ICP".IndexOf(verb[1])<0)return false;
   if(canvas.Selected==null||canvas.Selected.Code<1||canvas.Selected.Code>18||canvas.Selected.Count<3)
    throw new NotSupportedException("Movement with explicit units requires a SCORE item with a stored position.");
   bool horizontal=verb[0]=='L'||verb[0]=='R';
   float distance=horizontal?lastHorizontalMove:lastVerticalMove;
   if(arguments.Length!=0){
    float amount;
    if(!Single.TryParse(arguments,NumberStyles.Float,CultureInfo.InvariantCulture,out amount))
     throw new InvalidDataException("Enter a movement distance.");
    // SCORE overlay 05 uses binary32 divisors at DS:760A–7622.
    float divisor;
    if(horizontal)divisor=verb[1]=='I'?.0375f:verb[1]=='C'?.09525f:2.7f;
    else{
     int staff=canvas.Selected.Staff;
     if(staff<1||staff>32)throw new NotSupportedException("Vertical unit movement requires staff 1–32.");
     float staffScale=canvas.PageDocument.Tables.Scales[staff];
     float perScale=verb[1]=='I'?.00175f:verb[1]=='C'?.004445f:.126f;
     divisor=(float)((double)staffScale*perScale);
    }
    distance=(float)((double)amount/divisor);
    if(Single.IsNaN(distance)||Single.IsInfinity(distance))
     throw new InvalidDataException("Enter a finite movement distance.");
    if(horizontal)lastHorizontalMove=distance;else lastVerticalMove=distance;
   }
   MoveSelected(horizontal?(verb[0]=='L'?-distance:distance):0,
    horizontal?0:verb[0]=='D'?-distance:distance);
   return true;
  }

  bool ExecuteItemAlignment(string verb,string arguments){
   if(verb!="S"&&verb!="A")return false;
   RequireNoLetterArguments(verb,arguments);
   var item=canvas.Selected;
   if(item==null)throw new InvalidDataException("Select an item before using typed alignment.");
   if(item.Count<3)throw new NotSupportedException("Set and Align require a stored horizontal position.");
   if(verb=="S"){
    itemAlignmentPosition=item.P(3);NativeEditPositionGuide.Set(canvas,item.P(3));
    editFilter=new OriginalEditFilter{Position=true,Horizontal=item.P(3),Staff=99,Codes=new int[0]};
    direction=1;
    int first=NextEditingIndex(0,1);
    if(first>=0&&score.Items[first]!=item){
     if(canvas.EditModified)item.TrimEditedNumericParameters();
     editOriginal=null;
     BeginEdit(score.Items[first]);
    }else UpdateClassic();
   }
   else{
    if(!itemAlignmentPosition.HasValue)throw new InvalidDataException("Use S or F5 to set an alignment position first.");
    if(item.P(3)!=itemAlignmentPosition.Value)
     EditOriginalParameters(new[]{3f,itemAlignmentPosition.Value});
   }
   return true;
  }

  bool ExecuteNoteAccidental(string verb,string arguments){
   int accidental;
   switch(verb){
    case "FLT":accidental=1;break;
    case "SHP":accidental=2;break;
    case "NAT":accidental=3;break;
    case "NAC":accidental=0;break;
    default:return false;
   }
   RequireNoLetterArguments(verb,arguments);
   if(canvas.Selected==null||canvas.Selected.Code!=1)
    throw new InvalidDataException("Select a note before changing its accidental.");
   float value=canvas.Selected.P(5);
   if(!(value>=Int16.MinValue&&value<=Int16.MaxValue))
    throw new InvalidDataException("The note accidental control is outside the verified signed-word range.");
   // Signed remainder keeps the other packed P5 digits and fractional offset.
   EditOriginalParameters(new[]{5f,value+(accidental-(int)value%10)});
   return true;
  }
 }
}
