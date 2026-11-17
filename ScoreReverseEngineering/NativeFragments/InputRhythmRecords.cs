using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputRhythmRecords {
  static bool HasOrdinaryRestRhythm(float duration,float shape,float dots){
   if(!(shape>=-3&&shape<=7&&shape==Math.Truncate(shape))||
      !(dots>=0&&dots<=3&&dots==Math.Truncate(dots)))return false;
   double baseDuration=Math.Pow(2,-shape);
   return duration==baseDuration*(2-Math.Pow(2,-dots));
  }
  internal static bool HasStandardRestControls(float[] row){
   if(row.Length<7||row[0]!=2||!(row[6]>0&&row[6]<65))return false;
   float displacement=row[3],shape=row[4],dots=row[5];
   float attachment=row.Length>=8?row[7]:0,working=row.Length>=15?row[14]:0;
   // RW, RP and numbered whole rests keep their shape and entered duration.
   bool numbered=shape==-2&&attachment>=1&&attachment<=999&&attachment==Math.Truncate(attachment);
   if(displacement==0&&(attachment==0||numbered)&&dots==0&&(shape==-2||shape==-4))
    return working==0||working==-1;
   if(working!=0||(displacement!=0&&displacement!=-6&&displacement!=6)||
      (attachment!=0&&(attachment!=-1||displacement!=0)))return false;
   if(dots==-1&&displacement==0&&attachment==0){
    for(int n=0;n<=3;n++)if(HasOrdinaryRestRhythm(row[6],shape,n))return true;
    return false;
   }
   return HasOrdinaryRestRhythm(row[6],shape,dots);
  }
  // Candidate non-chord record projection of section57. Reference-staff
  // interpolation and the chord displacement callees are separate branches.
  public static List<float[]> Apply(IList<float[]> source,float[] encoded,float left,float right,float scale,bool proportional,float exponent,int[] forcedStems,float? referenceStart=null,float? rememberedLeft=null){
   var records=new List<float[]>();int notes=0,pitched=0;foreach(var p in source){if(p.Length!=15)throw new ArgumentException("Working records have fifteen words.");int code=(int)p[0];if(code!=1&&code!=2&&code!=3&&code!=14&&code!=17&&code!=18)throw new NotSupportedException("Unsupported working-record code.");if(code<3){if(code==1&&p[14]<0)throw new NotSupportedException("Chord branch required.");notes++;if(code==1)pitched++;}records.Add((float[])p.Clone());}
   if(notes!=encoded.Length||notes==0)throw new ArgumentException("Rhythm count differs from note count.");if(forcedStems!=null&&forcedStems.Length!=pitched)throw new ArgumentException("Stem-control count differs.");
   var values=new InputRhythmValue[notes];float total=0;var weights=new float[notes];
   for(int n=0;n<notes;n++){
    values[n]=InputRhythmDecoder.Decode(encoded[n]);
    float weight=values[n].Duration;
    if(values[n].Denominator==.0625f)weight=referenceStart.HasValue?0:.1f;
    // Section 56 supplies the x87 indefinite value as the spacing weight for zero duration.
    weights[n]=values[n].Duration==0?float.NaN:proportional?(float)Math.Pow(weight,exponent):weight;
    total=(float)((double)total+weights[n]);
   }
   bool allReferenceGrace=referenceStart.HasValue&&Array.TrueForAll(values,value=>value.Denominator==.0625f);
   if(total==0&&!allReferenceGrace)throw new NotSupportedException("Zero spacing weight outside the recovered grace-reference branch.");
   int active=records.Count;float end=right,margin=0;
   while(!referenceStart.HasValue&&active>0){var last=records[active-1];int code=(int)last[0];float width;if(code==18){width=last[4]>=10||last[5]>=10?7:4;margin=1.5f;}else if(code==17){width=(float)(Math.Abs((double)last[4]%100)*2.799999952316284+.5);margin=2.5f;}else break;end=(float)((double)end-width*scale);last[2]=end;active--;}
   end=(float)((double)end-(double)margin*scale);
   // Section57 retains DS:8118 when the requested interval is exhausted.
   // The caller must supply that word; it is not a geometric clamp.
   if(!referenceStart.HasValue&&left>=end){if(!rememberedLeft.HasValue)throw new NotSupportedException("Exhausted rhythm interval requires the original retained left word.");left=rememberedLeft.Value;}
   float available=(float)((double)end-left),cursor=left;int ix=0,noteIndex=0;
   for(int i=0;i<active;i++){var p=records[i];int code=(int)p[0];if(code<3)continue;float width=0;int next=i+1<active?(int)records[i+1][0]:0;
    if(code==17){width=(float)(Math.Abs((double)p[4]%100)*2.799999952316284+2);if(next==18)width=(float)((double)width-0.30000001192092896);if(i>0&&records[i-1][0]>2)records[i-1][14]=(float)((double)records[i-1][14]+scale);}
    else if(i+1<active){if(code==14){width=next==3||next>16?1.5f:3.5f;if(next==1&&records[i+1][4]%10!=0)width=4.5f;}else if(code==18)width=p[4]>=10||p[5]>=10?9:7;else{width=i<2?9:8;if(next>16)width=(float)((double)width+next-19);}}
    width=(float)((double)scale*width);p[14]=width;available=(float)((double)available-width);
   }
   for(int recordIndex=0;recordIndex<active;recordIndex++){var p=records[recordIndex];int code=(int)p[0];if(referenceStart.HasValue&&code>2&&(code!=3||recordIndex>(records[0][0]==14?1:0))){p[14]=0;continue;}if(code>2){float width=p[14];p[14]=0;if(recordIndex==active-1)cursor=end;if(recordIndex>0&&records[recordIndex-1][0]<=2&&code!=14){float third=(float)((double)width/3);available=(float)((double)available+third);cursor=(float)((double)cursor-third);}p[2]=cursor;cursor=(float)((double)cursor+width);continue;}if(ix==0&&referenceStart.HasValue){cursor=referenceStart.Value;available=(float)((double)end-cursor);}var value=values[ix];bool grace=value.Denominator==.0625f;int control=code!=1||forcedStems==null?0:forcedStems[noteIndex],pitch=(int)p[3];
    if(code==1&&grace&&Math.Abs(pitch)<100){pitch+=pitch<0?-100:100;p[3]=pitch;}
    p[6]=value.Duration;
    if(code==1&&Math.Abs(pitch)>99){int v=pitch%100;float extension=0;if(p[4]%100<20){if(v<1)extension=(float)((double)(2-v)*1.666659951210022-2.0);}else if(v>13&&!grace)extension=(float)((double)(v-12)*1.666659951210022-2.0);if(grace){if(extension==0)extension=1;extension=(float)((double)extension+100);if(control==0)p[4]=(float)((double)(p[4]%10)+10);}p[7]=extension;}
    if(code==2){
     if(grace)p[6]=65;
     if(p[5]>=0&&p[14]==0)p[5]=(float)((double)value.DotControl/10);
     float flags=p[14]==-1?0:value.FlagCount;
     p[12]=0;
     if(p[14]==0){
      int head=3-(int)value.Denominator;if(head==0)head=1;
      p[4]=value.Denominator<4?-head:flags;
     }
     // Section57 Code2 bypasses the common note/chord P9/P13 stores.
     // Rest shape/dots are in P5/P6; cue and numeric rhythms do not change that.
     p[8]=0;
    }else if(grace){p[6]=65;p[8]=1;p[12]=1;}else{p[8]=(float)((double)value.FlagCount+value.DotControl+(control==20&&value.DotControl!=0?100:0));p[12]=value.FlagCount;if(value.Denominator<4){int head=3-(int)value.Denominator;if(head==0)head=1;p[5]=head;}}
    if(code==1)noteIndex++;p[2]=cursor;double weight=weights[ix];cursor=(float)(weight/total*available+cursor);ix++;
   }
   // Section 57 repairs negative-sign positions from right to left, including
   // positions made indefinite by the zero-duration spacing calculation.
   if(float.IsNaN(total)&&!referenceStart.HasValue){
    for(int i=active-1;i>=0;i--){
     if(!float.IsNaN(records[i][2]))continue;
     if(i==active-1){records[i][2]=end-4;continue;}
     int previous=i-1;
     while(previous>0&&!(records[previous][2]>0))previous--;
     float next=records[i+1][2],gap=next-(previous>=0?records[previous][2]:0);
     records[i][2]=next-(gap<=4?gap/2:3);
    }
   }
   for(int i=1;i<active-1;i++)if(records[i][0]==3&&records[i-1][0]!=14){float width=(float)(7.0*scale);if((double)width+.625<=records[i+1][2]-records[i-1][2])records[i][2]=(float)((double)records[i+1][2]-width);}
   return records;
  }
 }
}
