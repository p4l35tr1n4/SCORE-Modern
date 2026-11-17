using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputBeamConstructionResult {public List<float[]> Records;public int[] ForcedStems;}
 public static class InputUniformBeam {
  // Section59's beam construction path with explicit staff coordinates, including chord reorientation,
  // accidental packing and post-reorientation decimal dot controls.
  public static InputBeamConstructionResult Build(IList<float[]> source,int[] map,int[] stems,float first,float last,float begin,float end,float slopeLimit,InputEndpointStaffContext context=null){
   stems=(int[])stems.Clone();var rows=new List<float[]>();foreach(var p in source)rows.Add((float[])p.Clone());
   var e=InputEndpointProjection.Decode(first,last,4,rows,map,stems,begin,end,context);
   if(e.StartFlag!=e.EndFlag||e.StartNote<1||e.EndNote>=map.Length)throw new NotSupportedException("Beam endpoint boundary");
   int count=e.EndNote-e.StartNote+1;if(count<2)throw new ArgumentException("Beam note range");
   var pitches=new float[count];var x=new float[count];var starts=new int[count];var ends=new int[count];int min=99,max=-99;
   for(int i=0;i<count;i++){int firstRecord=map[e.StartNote+i]-1,lastRecord=firstRecord;while(lastRecord+1<rows.Count&&rows[lastRecord+1][0]==1&&rows[lastRecord+1][14]<0)lastRecord++;starts[i]=firstRecord;ends[i]=lastRecord;for(int j=firstRecord;j<=lastRecord;j++){var p=rows[j];if(p[0]!=1||p[11]!=0&&context==null)throw new NotSupportedException("Cross-staff beam requires its original staff context");if(Math.Abs(p[3])<100){int pitch=(short)Math.Truncate(p[3]);if(pitch<min)min=pitch;if(pitch>max)max=pitch;}}}
   bool cueBeam=Math.Abs(rows[starts[0]][3])>=100;int direction=e.Stem,group=(7-min<=max-7)?20:10;if(direction!=0)group=direction;bool setGroup=stems[e.StartNote]==0;
   if(setGroup)for(int i=0;i<count;i++)if(ends[i]>starts[i]){int start=starts[i],stop=ends[i],old=(short)Math.Truncate(rows[start][4]),cue=0;if(old>99){old-=100;cue=100;}if(old/10!=group/10){rows[start][4]=old%10+cue;rows[start][7]=0;var pitch=new float[stop-start+1];var accidental=new float[pitch.Length];for(int j=start;j<=stop;j++){rows[j][9]=0;rows[j][8]=F((double)rows[j][8]%100);pitch[j-start]=rows[j][3];accidental[j-start]=(short)Math.Truncate(rows[j][4]);}for(int j=start;j<=stop;j++){rows[j][3]=pitch[stop-j];rows[j][4]=accidental[stop-j];}rows[start][4]=F((double)rows[start][4]+group);int returned=InputChordLayout.Apply(rows,start,stop);InputChordDots.ApplyRange(rows,start,returned);}}
   int flags=0,minimum=99;var levels=new int[count];
   for(int i=0;i<count;i++){
    int note=e.StartNote+i;var p=rows[starts[i]];
    if(setGroup&&stems[note]==0){stems[note]=group;int prior=(short)Math.Truncate(p[4])%100;if(prior>9){if(group==20&&prior<20)p[4]=F((double)p[4]+10);else if(group==10&&prior>19)p[4]=F((double)p[4]-10);int mark=(short)Math.Truncate(p[10]);if(mark>=0&&Array.IndexOf(new[]{4,5,8,9,10,19},mark)>=0){float value=InputAttachedMarks.DefaultValue(mark,p[3],p[4]);p[10]=value;}}}
    int f=(short)Math.Truncate(p[12]);if(f==0)throw new ArgumentException("Unflagged note in beam");flags=Math.Max(flags,f);minimum=Math.Min(minimum,f);levels[i]=f;pitches[i]=rows[ends[i]][3]%100;if(context!=null)context.Adjust(rows[ends[i]],ref pitches[i]);x[i]=rows[ends[i]][2];
   }
   var fit=InputBeamFit.Fit(pitches,x,setGroup?group:direction,flags,slopeLimit);
   direction=fit.Direction;float shift=0;int a=(short)Math.Truncate(fit.Start),b=(short)Math.Truncate(fit.End),offset=0;
   if(flags>2){offset=2;if(direction>19&&a>13||direction<20&&a<1)offset=4;offset+=(flags-3)*3;}
   if(direction<20){if(a==b&&a>9)offset-=2;}else{offset=-offset;if(a==b&&a<5)offset+=2;}
   if(e.StartFlag!=0)minimum=2; // Section59 forces two beam levels for grace endpoints.
   shift=cueBeam?(direction<20?0.800000011920929f:-0.800000011920929f):F(offset/2.0);float cueMarker=cueBeam?((short)Math.Truncate(fit.Start)<0?-100:100):0;var beam=new float[15];beam[0]=6;beam[1]=rows[e.StartItem-1][1];beam[2]=e.StartX;beam[3]=F((double)fit.Start+shift+cueMarker);beam[4]=F((double)fit.End+shift);beam[5]=e.EndX;beam[6]=direction+minimum;beam[7]=e.Number;rows.AddRange(InputBeamLevels.Pack(beam,levels,x));
   int firstOrnament=-1;for(int i=0;i<source.Count;i++)if(source[i][0]==6&&source[i][6]<0){firstOrnament=i;break;}
   for(int i=0;i<count;i++){int note=e.StartNote+i;var p=rows[map[note]-1];int stem=stems[note];if(stem>=0){if(stem==0)stem=p[11]==0?direction:30-(int)p[11]*10;int prior=(short)Math.Truncate(p[4])%100;if(prior<20&&stem>19)p[4]=F((double)p[4]+10);else if(prior>=20&&stem<20)p[4]=F((double)p[4]-10);}
    // Section59 a16b: an attached tremolo becomes relative to the final
    // beam's four coordinate words, while P11 retains its note's X.
    if(firstOrnament>=0)for(int j=firstOrnament;j<source.Count;j++){var ornament=rows[j];if(ornament[0]!=6||ornament[2]!=p[2])continue;ornament[6]=-direction;ornament[9]=F((double)F((double)p[12]*10+ornament[9])-10);ornament[10]=p[2];var finalBeam=rows[rows.Count-1];for(int k=2;k<=5;k++)ornament[k]=finalBeam[k];break;}
   }
   return new InputBeamConstructionResult{Records=rows,ForcedStems=stems};
  }
  static float F(double n){return (float)n;}
 }
}
