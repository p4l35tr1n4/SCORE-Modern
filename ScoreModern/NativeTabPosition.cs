using System;using System.Globalization;
namespace ScoreModern {public sealed partial class Editor {
 // Original TP caller 2FAC:0C83..0D95 and automatic chooser 1BA1:0007..
 // 0318. Constants below are the executable's word tables at152A/153C/1550.
 void OriginalTabPosition(string argument,bool fromScor4Menu=false){
  var item=canvas.Selected;if(item==null||item.Code!=1||!(item.P(6)<=-4))return;
  float amount=0;if(argument.Length!=0&&(!Single.TryParse(argument,NumberStyles.Float,CultureInfo.InvariantCulture,out amount)||Single.IsNaN(amount)||Single.IsInfinity(amount)))throw new InvalidOperationException("TP requires a finite minimum fret number.");int minimum=CenterWord(amount);
  if(BitConverter.ToInt32(BitConverter.GetBytes(item.P(8)),0)<0){EditOriginalParameters(new[]{8f,-1f});return;}
  int requested=fromScor4Menu?0:CenterWord(item.P(8));if(BitConverter.ToInt32(BitConverter.GetBytes(item.P(9)),0)<0){EditOriginalParameters(new[]{8f,(float)requested});return;}
  // Measured caller copies only P4/P5 into the parsed TP input. Its tuning
  // and capo words are zero even when the edited item has P16..19 set.
  // Evidence: batch-3/tab-tuning-call-original/entry.json and result.json.
  int head=-CenterWord(item.P(6)),variant=head/10,strings=head%10;
  if(variant==8||variant==15||variant==9){variant=0;}
  if(strings<1||strings>8||requested>strings)throw new NotSupportedException("TP string index outside the verified automatic table.");
  if(variant>=4){if(requested==0||minimum>0){EditOriginalParameters(new[]{8f,(float)requested});return;}throw new NotSupportedException("Manual TP with a nonpositive minimum and a positive string can produce an invalid original string value; that path remains guarded pending scratch-state verification.");}
  int pitch=CenterWord((float)((double)item.P(4)%100)),accidental=CenterWord((float)((double)item.P(5)%10));
  var plan=RecoveredFragments.LayoutTablaturePitch.Prepare(head,pitch,accidental,null);int absolute=plan.Absolute;var tuning=plan.Tuning;
  int preferred=strings;for(int n=1;n<=strings;n++)if(tuning[n]<=absolute){preferred=n;break;}
  int chosen=Math.Max(preferred,requested);while(chosen<strings&&unchecked((short)(absolute-tuning[chosen]))<minimum)chosen++;
  EditOriginalParameters(new[]{8f,(float)chosen});
 }
 static void AdjustOriginalTabTuning(int[] tuning,int first,float word){int value=CenterWord(word),left=value/100,right=value%100;if(left>=10)left=20-left;if(right>=10)right=20-right;tuning[first]=unchecked((short)(tuning[first]+left));tuning[first+1]=unchecked((short)(tuning[first+1]+right));}
}}
