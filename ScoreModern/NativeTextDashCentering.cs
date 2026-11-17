using System;using System.Collections.Generic;
using System.Linq;
namespace ScoreModern {public sealed partial class Editor {
 // Shared numerical projection of original lyric dash creation and
 // 2F6F:0200..05FA, with explicit binary32 stores at the original points.
 // 20DD:0C70 truncates the live x87 value with FISTP qword. A float
 // store here would turn a quotient just below1 into1 and add a dash.
 static float OriginalDashAutomaticCount(double quotient){if(Double.IsNaN(quotient)||Double.IsInfinity(quotient)||quotient< -32768||quotient>=32768)throw new NotSupportedException("Automatic dash count leaves the verified signed16 domain.");return (short)(int)quotient+1;}
 static float[] OriginalDashedTextGeometry(float left,float leftWidth,float right,float staffScale,float size,float baseY,float dashSize,float? countOverride){return OriginalDashedTextGeometryCore(left,leftWidth,right,staffScale,size,baseY,dashSize,countOverride,false);}
 static float[] OriginalLyricDashedTextGeometry(float left,float leftWidth,float right,float staffScale,float size,float baseY,float dashSize,float? countOverride){return OriginalDashedTextGeometryCore(left,leftWidth,right,staffScale,size,baseY,dashSize,countOverride,true);}
 //2C0C:038C uses the live x87 gap;2F6F:0531 reloads its binary32 store.
 static float[] OriginalDashedTextGeometryCore(float left,float leftWidth,float right,float staffScale,float size,float baseY,float dashSize,float? countOverride,bool liveGap){
  float dashFactor=BitConverter.ToInt32(BitConverter.GetBytes(dashSize),0)==0?1.2833375930786133f:(float)((double)dashSize/.8571400046348572);float scale=(float)((double)staffScale/25),factor=(float)((double)staffScale/25*size),endOfText=(float)((double)leftWidth+left);right=Math.Min(right,201);float end=(float)((double)right-3.1700000762939453*factor);double available=(double)right-3.1700000762939453*factor-endOfText;float count=countOverride.HasValue?countOverride.Value:OriginalDashAutomaticCount(available/((double)factor*25));if(count==-1||factor==0||scale==0)throw new NotSupportedException("Degenerate lyric dash geometry is prevented.");double gap=((float)available-(double)dashFactor*factor*count)/((double)count+1);float startBase=(float)((double)endOfText+gap);end=(float)((double)end+scale*.20000000298023224);float y=(float)((double)baseY+size*.800000011920929),length=(float)((double)dashFactor*size*.8571400046348572),spacing=(float)(.8571400046348572*(liveGap?gap:(float)gap)/scale);float start=(float)((double)scale*1.5+startBase),stop=(float)((double)scale*.10000000149011612+endOfText);if(start<stop)start=stop;if((double)end-startBase<.20000000298023224)end=(float)((double)startBase+.019999999552965164);return new[]{start,y,end,length,spacing};
 }
 // CD solid branch 2F6F:0315..0475 uses staff-relative note clearances.
 float[] OriginalSolidTextGeometry(float staff,float left,float width,float right,float staffScale,float size,float y){
  float scale=(float)((double)staffScale/25),factor=(float)((double)staffScale/25*size),endOfText=(float)((double)left+width),end=(float)((double)Math.Min(right,201)-3.1700000762939453*factor),limit=(float)((double)end-scale*4),note=0;int bits=BitConverter.ToInt32(BitConverter.GetBytes(staff),0);
  foreach(var item in score.Items)if(item.P(1)==1&&BitConverter.ToInt32(BitConverter.GetBytes(item.P(2)),0)==bits&&item.P(3)<=limit&&item.P(3)>note)note=item.P(3);float candidate=(float)((double)scale*4+note);if((double)scale*2+endOfText<candidate&&candidate<end)end=candidate;float start=(float)((double)scale*1.5+endOfText);if((double)end-endOfText<.20000000298023224)end=(float)((double)endOfText+.019999999552965164);return new[]{start,y,end,0f,0f};
 }
 void CenterOriginalTextDashes(float[] values){
  var line=canvas.Selected;if(line==null||line.P(1)!=4)return;
  bool changedCount=values.Length>0&&CenterWord(values[0])!=0;
  var edits=OriginalDashEdits(line,changedCount?values[0]:line.P(14),changedCount,NativePreferences.Read(source).Number("DASH",1.1f));
  if(edits.Length!=0)EditOriginalParameters(edits);
 }

 float[] OriginalDashEdits(Item line,float count,bool changedCount,float dashSize){
  bool solid=BitConverter.ToInt32(BitConverter.GetBytes(line.P(7)),0)==0;Item left=null,right=null;float leftX=-9999,rightX=9999;int staffBits=BitConverter.ToInt32(BitConverter.GetBytes(line.P(2)),0);
  foreach(var text in score.Items){if(text.P(1)!=16||BitConverter.ToInt32(BitConverter.GetBytes(text.P(2)),0)!=staffBits||Math.Abs((double)text.P(4)-line.P(4))>1.899999976158142)continue;double x=(double)text.P(3)+text.P(11);float stored=(float)x;if(x<=line.P(3)){if(stored>=leftX){left=text;leftX=stored;}}else if(stored<rightX){right=text;rightX=stored;}}
  if(left==null&&right==null)return changedCount?new[]{14f,count}:new float[0];if(BitConverter.ToInt32(BitConverter.GetBytes(leftX),0)<0)leftX=0;var sizeSource=left??right;float size=sizeSource.P(6);if(BitConverter.ToInt32(BitConverter.GetBytes(size),0)==0)size=1;canvas.LayoutScore();int staff=(int)sizeSource.P(2);if(staff<1||staff>32)throw new NotSupportedException("CD staff scale leaves the original table.");float staffScale=canvas.PageDocument.Tables.Scales[staff];var g=solid?OriginalSolidTextGeometry(line.P(2),leftX,left==null?0:left.P(13),rightX,staffScale,size,sizeSource.P(4)):OriginalDashedTextGeometry(leftX,left==null?0:left.P(13),rightX,staffScale,size,sizeSource.P(4),dashSize,BitConverter.ToInt32(BitConverter.GetBytes(count),0)==0?(float?)null:count);var edits=new List<float>{3,g[0],4,g[1],5,g[1],6,g[2],8,g[3],9,g[4]};if(changedCount){edits.Add(14);edits.Add(count);}return edits.ToArray();
 }

 void CenterScor4GroupDashes(short staff){
  var preferences=NativePreferences.Read(source);
  float dashSize=preferences.Number("DASH",1.2857099771499634f);
  short count=preferences.Word("XDASH",0);
  var records=score.Items.ToArray();
  bool changed=false;
  for(int n=0;n<records.Length;n++){
   Item item=records[n];
   if(item.Code!=4||(staff>0&&item.Staff!=staff)||item.Count<6||(item.Count>10&&item.Count!=14))continue;
   if(BitConverter.ToInt32(item.Bytes,16)!=BitConverter.ToInt32(item.Bytes,20))continue;
   bool dashed=BitConverter.ToInt32(BitConverter.GetBytes(item.P(7)),0)!=0;
   var edits=OriginalDashEdits(item,dashed?count:0,dashed&&item.Count==14,dashSize);
   if(edits.Length==0)continue;
   var replacement=item.Clone();
   for(int p=0;p<edits.Length;p+=2)replacement.Set((int)edits[p],edits[p+1]);
   if(replacement.Bytes.SequenceEqual(item.Bytes))continue;
   records[n]=replacement;changed=true;
  }
  if(changed){
   using(var storage=BeginParameterMutation()){
    Checkpoint();score.Items=records.ToList();dirty=true;RefreshAll(null);storage.Complete();
   }
  }
  Message("");
 }
}}
