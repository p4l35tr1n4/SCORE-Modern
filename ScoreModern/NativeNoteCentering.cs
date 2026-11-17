using System;using System.Collections.Generic;using System.Globalization;using System.IO;
namespace ScoreModern { public sealed partial class Editor {
 Item retainedCenterRight;Score retainedCenterScore;
 void CenterScor4Slur(bool left,string argument){
  RequireNoLetterArguments(left?"CNL":"CNR",argument);
  var selected=canvas.Selected;
  if(mode!=ScreenMode.Edit||selected==null||selected.Code!=5)return;
  int staff=selected.Staff;
  Item nearest=null;
  float first=float.PositiveInfinity,lastBar=float.NegativeInfinity,distance=float.PositiveInfinity;
  float target=selected.P(left?6:3);
  foreach(var item in score.Items){
   if(item.Staff!=staff)continue;
   if(item.Code==14){lastBar=Math.Max(lastBar,item.P(3));continue;}
   if(item.Code!=1)continue;
   float position=item.P(3);
   first=Math.Min(first,position);
   float delta=(float)Math.Abs((double)position-target);
   if(delta<distance){distance=delta;nearest=item;}
  }
  if(nearest==null||(!left&&float.IsInfinity(lastBar)))
   throw new NotSupportedException("SCOR4 slur centering without the required staff notes or barline has not been verified.");
  float start=left?(float)((double)first-5):nearest.P(3);
  float end=left?nearest.P(3):(float)((double)lastBar-2);
  EditOriginalParameters(new[]{3f,start,6f,end});
 }
 // Recovered overlay 2C0C:0010/0401/07B3. Distances are explicitly stored
 // as binary32 before comparison; equal distances retain the first item.
 void CenterOriginalNote(string argument,bool vertical=false) {
  if(vertical)RequireNoLetterArguments("CV",argument);
  if(mode!=ScreenMode.Edit||canvas.Selected==null)return;
  var selected=canvas.Selected;float requested=0;
  if(argument.Length!=0&&(!Single.TryParse(argument,NumberStyles.Float,CultureInfo.InvariantCulture,out requested)||Single.IsNaN(requested)||Single.IsInfinity(requested)||requested<0||requested>32767))throw new InvalidDataException("CN requires a staff number from 0 to 32767.");
  int staff=(int)requested;if(staff==0)staff=(int)selected.P(2);
  if(selected.Code==5&&((int)selected.P(8)==1||(int)selected.P(8)==2)){CenterOriginalEnding(selected,staff,(int)selected.P(8));return;}
  var operations=new List<float>();
  int initialOrdinal=0;
  Func<float,Item> nearest=x=>{float distance=99999f;Item best=null;int ordinal=0;initialOrdinal=0;foreach(var item in score.Items){if(Object.ReferenceEquals(item,selected)||(int)item.P(2)!=staff||(int)item.P(1)>2)continue;ordinal++;float d=(float)Math.Abs((double)item.P(3)-x);if(d<distance){if(initialOrdinal==0)initialOrdinal=ordinal;distance=d;best=item;}}return best;};
  bool negativeBeam=selected.Code==6&&BitConverter.ToInt32(BitConverter.GetBytes(selected.P(7)),0)<0;
  Func<Item,bool> tremoloHead=match=>negativeBeam&&match!=null&&match.Code==1&&match.Count>=6&&CenterWord(match.P(6))>=2&&CenterWord(match.P(6))<=3;
  Action<int,float,Item> append=(parameter,x,match)=>{operations.Add(parameter);operations.Add(x);if(tremoloHead(match)){float offset=selected.P(7)>=-10?-1.2300000190734863f:1.600000023841858f;if(match.Count>=10&&BitConverter.ToInt32(BitConverter.GetBytes(match.P(10)),0)!=0){float extra=match.P(10);if(extra==10)extra=3.259999990463257f;else if(extra==20)extra=-3.259999990463257f;offset=(float)((double)offset+extra);}operations.Add(14);operations.Add(offset);}};
  Item last=null;var first=nearest(selected.P(3));int firstOrdinal=initialOrdinal;if(first!=null)append(3,first.P(3),first);
  bool oneEnded=negativeBeam&&BitConverter.ToInt32(BitConverter.GetBytes(selected.P(11)),0)==0;
  if(!oneEnded&&(selected.Code==5||selected.Code==6)){
   if(first!=null&&firstOrdinal==1&&selected.P(6)<=first.P(3)){
    if(vertical){
     if(!NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Reversed CV uses an unrefreshed original item pointer. Original invalid-state behavior is disabled.");
     if(retainedCenterScore!=score||retainedCenterRight==null||!score.Items.Contains(retainedCenterRight))throw new NotSupportedException("Reversed CV has no verified retained item-pointer history in this document.");
     // The short branch pre-fills two pairs without advancing B6E8. The
     // vertical helper overwrites pair two, leaving P3 unchanged (0EE4..0F12).
     operations.Clear();operations.Add(6);operations.Add(first.P(3));
     bool above=CenterWord(selected.P(7))>=0;
     if(first.Code==1){operations.Add(4);operations.Add(OriginalSlurVertical(first,selected,above));}
     if(!Object.ReferenceEquals(first,retainedCenterRight)&&retainedCenterRight.Code==1){operations.Add(5);operations.Add(OriginalSlurVertical(retainedCenterRight,selected,above));}
     EditOriginalParameters(operations.ToArray());return;
    }
    operations.Clear();operations.Add(6);operations.Add(first.P(3));operations.Add(3);operations.Add((float)((double)first.P(3)-4-(selected.Code==5&&selected.P(8)<=-2?5:0)));
   }else{
    float query=selected.P(6),right=0;bool found=false;
    for(int attempt=0;attempt<1024;attempt++){
     last=nearest(query);if(last==null)break;retainedCenterRight=last;retainedCenterScore=score;right=last.P(3);
     if(tremoloHead(last)||operations.Count<2||BitConverter.ToInt32(BitConverter.GetBytes(right),0)!=BitConverter.ToInt32(BitConverter.GetBytes(operations[operations.Count-1]),0)){found=true;break;}
     query=(float)((double)query+2);if(query>=199.5f){right=200;found=true;break;}
    }
    if(!found)throw new NotSupportedException("CN endpoint search exceeds the verified finite domain.");
    append(6,right,last);
   }
  }
  if(selected.Code==6&&!oneEnded&&(double)selected.P(7)%10!=0){
   int end=negativeBeam?11:15;
   for(int parameter=11;parameter<=end;parameter++){
    if(parameter>selected.Count)break;if(parameter==13||!(selected.P(parameter)>0))continue;
    int previous=operations.Count>=2?(int)operations[operations.Count-2]:0;float previousX=operations.Count>=2?operations[operations.Count-1]:0;
    bool deduplicate=(parameter==12||parameter==15)&&previous!=6&&previous!=12;
    float query=selected.P(parameter),value=0;bool found=false;
    for(int attempt=0;attempt<100000;attempt++){
     var match=nearest(query);if(match==null)break;value=match.P(3);
     if(!deduplicate||BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=BitConverter.ToInt32(BitConverter.GetBytes(previousX),0)){found=true;break;}
     float next=(float)((double)query+2);if(next==query)break;query=next;if(query>=199.5f){value=200;found=true;break;}
    }
    if(!found)throw new NotSupportedException("CN beam anchor search exceeds the verified finite domain.");append(parameter,value,nearest(value));
   }
  }
  if(vertical&&selected.Code==5){bool above=CenterWord(selected.P(7))>=0;if(first!=null&&first.Code==1){operations.Add(4);operations.Add(OriginalSlurVertical(first,selected,above));}if(last!=null&&last.Code==1){operations.Add(5);operations.Add(OriginalSlurVertical(last,selected,above));}}
  if(operations.Count!=0)EditOriginalParameters(operations.ToArray());
 }
 static short CenterWord(float value){if(Single.IsNaN(value)||Single.IsInfinity(value)||value< -32768||value>=32768)throw new NotSupportedException("Slur centring word conversion outside the verified signed16 range.");return (short)(int)value;}
 static float OriginalSlurTip(Item note,bool above){
  // Overlay 2C0C:0F4E..1161; each marked store rounds to binary32.
  float pitch=note.Count>=4?(float)((double)note.P(4)%100):0;
  int kind=note.Count>=5?CenterWord(note.P(5))%100:0;
  float extra=note.Count>=8?(float)((double)note.P(8)%100):0;
  if(kind<10)extra=-6;
  if(note.Count>=9){int levels=CenterWord((float)((double)note.P(9)%10))-1;if(levels>0)extra=(float)((double)extra+unchecked((short)(2*levels)));}
  bool longStem=above?kind<20:kind>=20;
  float offset=longStem?(above?(float)((double)extra+8.399999618530273):(float)(-8.399999618530273-extra)):(above?2f:-2f);
  if(longStem&&Math.Abs((double)note.P(4))>=100)offset=(float)((double).6000000238418579f*offset);
  return (float)((double)pitch+offset);
 }
 float OriginalSlurVertical(Item nearest,Item selected,bool above){
  float value=OriginalSlurTip(nearest,above);int staff=CenterWord(selected.P(2));int position=BitConverter.ToInt32(BitConverter.GetBytes(nearest.P(3)),0);
  // Original 0C7D..0F4B includes simultaneous heads only at identical P3 bits.
  foreach(var note in score.Items){if(note.Code!=1||CenterWord(note.P(2))!=staff||BitConverter.ToInt32(BitConverter.GetBytes(note.P(3)),0)!=position)continue;float other=OriginalSlurTip(note,above);if(above?other>value:other<value)value=other;}
  // Resident 13A9:00D0..014D moves odd staff-line positions into a space.
  if((double)value%2==1&&(above?(int)value<=11:(int)value>=3))value=(float)((double)value+(above?1:-1));
  return value;
 }
 void SetOriginalEditCurvature(string argument){RequireNoLetterArguments("CR",argument);if(canvas.Selected==null||canvas.Selected.Code!=5)return;var item=canvas.Selected;float curve=OriginalCurvature(item.P(3),item.P(4),item.P(5),item.P(6));if(item.P(7)<0)curve=-curve;EditOriginalParameters(new[]{7f,curve});}
 void CenterOriginalEnding(Item selected,int staff,int kind) {
  // Overlay 2C0C:0AA0..0C53; bar-end search advances by the literal 2.
  Func<float,Item> nearest=x=>{float distance=99999;Item best=null;foreach(var bar in score.Items){if(bar.Code!=14||(int)bar.P(2)!=staff)continue;float d=(float)Math.Abs((double)bar.P(3)-x);if(d<distance){distance=d;best=bar;}}return best;};
  var left=nearest(selected.P(3));if(left==null)return;
  canvas.LayoutScore();Func<Item,float> scale=bar=>{int n=(int)bar.P(2);if(n<1||n>32)throw new NotSupportedException("CN ending staff outside recovered scale table.");return canvas.PageDocument.Tables.Scales[n];};
  float leftX=left.P(3);
  if(kind==1){if(left.Count>=5&&left.P(5)==3)leftX=selected.P(3);}
  else if(left.Count<5||left.P(5)!=3)leftX=selected.P(3);
  else leftX=(float)((double)leftX-scale(left)/25.0);
  float query=selected.P(6),rightX=query;bool found=false;
  for(int attempt=0;attempt<1024;attempt++) {
   var right=nearest(query);if(right==null)break;retainedCenterRight=right;retainedCenterScore=score;float x=right.P(3);
   bool retry=kind==1?BitConverter.ToInt32(BitConverter.GetBytes(x),0)==BitConverter.ToInt32(BitConverter.GetBytes(leftX),0):(double)x-leftX<=4;
   if(!retry){rightX=kind==1?(float)((double)x-(double)scale(right)*2.299999952316284/25):x;found=true;break;}
   query=(float)((double)query+2);
   if(query>200){rightX=Math.Min(query,200);found=true;break;}
  }
  if(!found)throw new NotSupportedException("CN ending search exceeds the verified finite domain.");
  EditOriginalParameters(new[]{3f,leftX,6f,rightX});
 }
}}
