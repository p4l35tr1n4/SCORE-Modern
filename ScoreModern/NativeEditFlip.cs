using System;
namespace ScoreModern {public sealed partial class Editor {
 // Original 2FAC:08C7..09E0. This is the selected-item shortcut,
 // distinct from the Main chord-reconstruction flip routines.
 void FlipOriginalEditedItem(string argument){
  RequireNoLetterArguments("FLI",argument);var item=canvas.Selected;if(item==null)return;
  if(item.Code==1){int control=CenterWord(item.P(5))%100;if(control<10)return;EditOriginalParameters(new[]{500f,control<20?10f:-10f});return;}
  if(item.Code==5){EditOriginalParameters(new[]{7f,-item.P(7)});return;}
  if(item.Code==6){int control=Math.Abs(CenterWord(item.P(7))%100);if(control<10)return;float delta=control<20?10:-10;if(BitConverter.ToInt32(BitConverter.GetBytes(item.P(7)),0)<0)delta=-delta;EditOriginalParameters(new[]{700f,delta});return;}
  if(scor4Interface&&item.Code==12&&item.P(5)==2){EditOriginalParameters(new[]{7f,-item.P(7)});return;}
  if(item.Code==9||item.Code==11||(scor4Interface&&item.Code==16)){float value=item.P(7);if(BitConverter.ToInt32(BitConverter.GetBytes(value),0)==0){value=(float)((double)item.P(6)%100);if(value==0)value=1;}EditOriginalParameters(new[]{7f,-value});}
 }
}}
