using System;using System.Collections.Generic;
namespace ScoreModern {public sealed partial class Editor {
 // Original 323E:0415..060A and 2D55:0643..06C5. This is an
 // ordered text-record operation, not a visual-neighbour search.
 void OriginalTextLineSpacing(string verb,float[] values){
  var selected=canvas.Selected;if(values.Length==0||BitConverter.ToInt32(BitConverter.GetBytes(values[0]),0)==0)return;
  if(canvas.EditModified){pendingEditAcknowledgement=true;Message("Cannot perform function until done editing current item."+Environment.NewLine+"Press any key to continue.");return;}
  float spacing=values[0];canvas.LayoutScore();int staff=(int)selected.P(2);if(staff<1||staff>32)throw new NotSupportedException("LS staff scale leaves the original table.");float scale=canvas.PageDocument.Tables.Scales[staff];
  if(verb!="LS"){float unit=verb=="LSI"?.0017500000540167093f:verb=="LSC"?.0044450000859797f:.12600000202655792f;if(scale==0)throw new NotSupportedException("LS physical-unit zero staff-size division is prevented.");spacing=(float)((double)spacing/((double)scale*unit));}
  int count=values.Length>1?CenterWord(values[1]):0;if(count==0)count=999;int direction=1;if(count<=0){spacing=-spacing;direction=-1;count+=2;}int start=score.Items.IndexOf(selected)+1,limit=Math.Max(1,start+count-1);bool alignDifferent=values.Length>2&&CenterWord(values[2])!=0;float y=(float)((double)selected.P(4)-spacing);var targets=new List<Item>();var positions=new List<float>();
  Func<float,float,bool> same=(a,b)=>BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);
  for(int number=start+direction;number>=1&&number<=score.Items.Count;number+=direction){var item=score.Items[number-1];if(item.P(1)!=16||!same(item.P(2),selected.P(2))||!alignDifferent&&!same(item.P(3),selected.P(3)))break;targets.Add(item);positions.Add(y);y=(float)((double)y-spacing);if(number==limit)break;}
  if(targets.Count==0)return;var prepared=new List<Item>();for(int n=0;n<targets.Count;n++){var next=targets[n].Clone();next.Set(3,selected.P(3));next.Set(4,positions[n]);next.Set(6,selected.P(6));next.Set(7,selected.P(7));var text=canvas.TextLayout.PrepareMetricLayout(next,next.Text,scale,0);if(!text.MetricsSupported)throw new NotSupportedException("LS text metrics require original verification.");next.Set(13,text.Width);prepared.Add(next);}Checkpoint();for(int n=0;n<targets.Count;n++)targets[n].Bytes=prepared[n].Bytes;dirty=true;RefreshAll(selected);canvas.EditModified=false;
 }
}}
