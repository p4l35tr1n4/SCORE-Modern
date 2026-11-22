# Recovered input, spacing and layout routines

The listings reproduce the local source verbatim. Source comments state the scope of each model.

## InputAttachedMarks

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputAttachedMarks.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputAttachedMarks {
  // Default-side attached-mark numeric branch, section60 0D9F/0DE2.
  // Opposite-side marks, Code9 additions and ranges are caller branches.
  public static float DefaultValue(int mark,float pitch,float stem){
   if(mark<1||mark>36)throw new ArgumentOutOfRangeException("mark");
   if(mark!=4&&mark!=5&&mark!=8&&mark!=9&&mark!=10&&mark!=19)return mark;
   int y=((int)Math.Truncate(pitch))%100;float offset=0;
   if(stem%100<20){if(y<4){if(mark==5)offset=0.05000000074505806f;}else{offset=(float)((double)y/10-(mark==5?0.3499999940395355f:0.4000000059604645f));if(offset>0.8999999761581421f)offset=0.8999999761581421f;}}
   else{if(y>10){if(mark==5)offset=0.05000000074505806f;}else{offset=(float)((mark==5?1.0499999523162842f:1f)-(double)y/10);if(offset>0.8999999761581421f)offset=0.8999999761581421f;}}
   return (float)((double)mark+offset);
  }
 }
}
````

## InputAutomaticBeams

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputAutomaticBeams.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputAutomaticBeams {
  static float F(double n){return (float)n;}
  static bool Present(float n){return BitConverter.ToInt32(BitConverter.GetBytes(n),0)!=0;}
  static void Emit(List<float> pairs,int first,int last){if(first<last){pairs.Add(first);pairs.Add(last);}}
  // Overlay8 section63. Duration words are the separate stage57 projection,
  // not the rhythm input tokens: secondary/invisible items have zero duration.
  public static float[] Groups(IList<float[]> rows,float[] durations,float groupControl,int skip){
   if(rows.Count!=durations.Length)throw new ArgumentException("Duration projection length");float length=F(groupControl/2.0),threshold=F((double)length-0.0010000000474974513);if(length<=0||float.IsNaN(length)||float.IsInfinity(length))throw new ArgumentOutOfRangeException("groupControl","Nonpositive automatic beam intervals are prevented");
   var pairs=new List<float>();int position=-1,lastFlagged=0,notes=0,first=0,flags=0;
   if(skip!=0)do{do{position++;if(position>=rows.Count)return pairs.ToArray();}while(rows[position][0]>2||!Present(durations[position])||durations[position]>=64);if(rows[position][0]==1)lastFlagged++;skip--;}while(skip>0);
   for(int i=0;i<rows.Count;i++)if(rows[i][0]==1&&Present(durations[i])){int beginning=++notes;if(durations[i]>=64){while(i<rows.Count-1){i++;if(Present(durations[i])){if(durations[i]<64)break;if(rows[i][0]==1)notes++;}}Emit(pairs,beginning,notes);if(rows[i][0]==1)notes++;}}
   float elapsed=0;notes=lastFlagged;
   while(true){
    if(position<rows.Count-1){position++;var row=rows[position];float duration=durations[position];if(row[0]<=2&&Present(duration)){
      if(row[0]==2){if(duration>=64)continue;}
      else{notes++;if(duration>=64)continue;flags=(short)Math.Truncate(row[12]);if(flags!=0){lastFlagged=notes;if(first==0)first=notes;}}
      elapsed=F((double)duration+elapsed);if(flags!=0&&elapsed<threshold)continue;
     }else continue;
    }
    if(first!=0){Emit(pairs,first,lastFlagged);first=0;}
    if(position>=rows.Count-1)return pairs.ToArray();
    if(elapsed>=threshold){do{elapsed=F((double)elapsed-length);}while(threshold<elapsed);if(Math.Abs(elapsed)<0.0010000000474974513f)elapsed=0;}
   }
  }
  public static float[] Durations(IList<float[]> rows){var values=new float[rows.Count];for(int i=0;i<rows.Count;i++)values[i]=BitConverter.ToInt32(BitConverter.GetBytes(rows[i][14]),0)<0?0:rows[i][6];return values;}
 }
}
````

## InputBeamFit

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputBeamFit.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputBeamFitResult {public float Start,End,UnquantizedStart,UnquantizedEnd;public int Direction;}
 public static class InputBeamFit {
  // Section54 numerical path for a single-staff, non-chord group. Inputs are
  // the representative pitches and x positions prepared by section59.
  public static InputBeamFitResult Fit(float[] pitch,float[] x,int direction,int flags,float slopeLimit){
   if(pitch==null||x==null||pitch.Length!=x.Length||pitch.Length<2)throw new ArgumentException("Beam group");
   if(direction!=0&&direction!=10&&direction!=20)throw new ArgumentException("Direction");
   int n=pitch.Length;float a=pitch[0],b=pitch[n-1],high=a,low=a,span=0;
   foreach(float p in pitch){if(float.IsNaN(p)||float.IsInfinity(p))throw new ArgumentException("Finite pitch required");if(p>high)high=p;if(p<low)low=p;}
   if(direction==0){float above=F((double)high-7),below=F(7.0-low);direction=20;if(above==below){int hi=0,lo=0;foreach(float p in pitch){if(p>7)hi++;if(p<7)lo++;}if(hi<lo)direction=10;}else if(above<below)direction=10;}
   float extreme=direction==10?high:low;bool flatten=false;
   if(a==b)flatten=Bits(extreme)!=Bits(a);
   else{int count=0;foreach(float p in pitch)if(p==extreme)count++;flatten=count>=2;}
   if(flatten)a=b=F((double)extreme+(flags==1?(direction==10?-1:1):0));
   else if(a!=b){
    span=F((double)x[n-1]-x[0]);if(span==0)throw new ArgumentException("Zero beam span");float slope=F(((double)b-a)/span),gap=direction==20?99:-99,targetPitch=0;int at=0;
    for(int i=1;i<n-1;i++){float line=F(((double)x[i]-x[0])*slope+a),distance=F((double)pitch[i]-line);if(direction==20?distance<0&&distance<gap:distance>0&&distance>gap){gap=distance;targetPitch=pitch[i];at=i;}}
    float allowance=flags>1?0:1;
    if(direction==20?gap!=99&&gap< -allowance:gap!= -99&&gap>allowance){
     float target=F((double)targetPitch+(direction==20?allowance:-allowance));
     if(Bits(extreme)!=Bits(a)&&Bits(extreme)!=Bits(b))a=b=F((double)extreme+(flags==1?(direction==10?-1:1):0));
     else{
      float other=direction==20?a:b,anchor=direction==20?b:a;
      // Section54:0751-07A6 compares direction-ordered endpoints; JA selects the far-end output.
      if(anchor>other)b=Extrapolate(a,target,x[0],x[at],x[n-1]);else a=Extrapolate(b,target,x[n-1],x[at],x[0]);
     }
    }
   }
   if(direction==20){if(a>14)a=14;if(b>14)b=14;}else{if(a<0)a=0;if(b<0)b=0;}
   bool shiftEnd=direction==20?a<b:b<a;float difference=Math.Abs(F((double)a-b));
   if(difference>1.5f&&slopeLimit<difference/span){
    if(difference>=3){difference=F((double)difference-1);if(flags<2){float change=direction==20?1:-1;if(shiftEnd)a=F((double)a+change);else b=F((double)b+change);}}
    float oldA=a,oldB=b;
    for(float slope=slopeLimit;slope<difference/span;slope=F((double)slope+0.009999999776482582)){float shift=F((double)difference-(double)slope*span);if(direction==20)shift=-shift;if(shiftEnd)b=F((double)oldB+shift);else a=F((double)oldA+shift);if(Math.Abs(F((double)b-a))>=1)break;}
   }
   int qa=(short)Math.Truncate((double)a*2),qb=(short)Math.Truncate((double)b*2);float adjust=0;
   if(qa==qb&&(qa/2)%2!=0&&flags%2==0){if(direction==20&&qa>13)adjust=.5f;else if(direction==10&&qa<15)adjust=-.5f;}
   return new InputBeamFitResult{Direction=direction,UnquantizedStart=a,UnquantizedEnd=b,Start=F(qa/2.0+adjust),End=F(qb/2.0+adjust)};
  }
  static float Extrapolate(float anchor,float target,float anchorX,float targetX,float resultX){float slope=F(((double)target-anchor)/((double)targetX-anchorX));return F(((double)resultX-anchorX)*slope+anchor);}
  static float F(double x){return (float)x;}static int Bits(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0);}
 }
}
````

## InputBeamLevels

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputBeamLevels.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputBeamLevels {
  // Section59 0B87..0E00: pack secondary beam runs and isolated hooks into
  // the two three-word control groups of each Code6 record.
  public static List<float[]> Pack(float[] prototype,int[] original,float[] x){
   int n=original.Length;if(n<2||x.Length!=n)throw new ArgumentException("Beam level inputs");int minimum=int.MaxValue;foreach(int f in original){if(f<1)throw new NotSupportedException("Zero/grace beam-level branch");minimum=Math.Min(minimum,f);}int[] levels=(int[])original.Clone(),hook=new int[n];
   for(int i=0;i<n;i++)if(levels[i]!=minimum){int previous=i==0?0:levels[i-1];if(i==n-1){if(previous<levels[i])hook[i]=-1;}else if(levels[i]!=levels[i+1]&&levels[i+1]<levels[i]&&previous<levels[i])hook[i]=previous<levels[i+1]?1:-1;}
   bool different=false;foreach(int f in levels)if(f!=minimum)different=true;if(!different)return new List<float[]>{(float[])prototype.Clone()};
   var result=new List<float[]>();float[] current=(float[])prototype.Clone();int cursor=0,prior=minimum,slot=9,triplets=0;
   while(cursor<n){while(cursor<n&&levels[cursor]==minimum){prior=minimum;cursor++;}if(cursor>=n)break;int stop=cursor,level=levels[cursor];if(cursor<n-1){int last=cursor;for(int j=cursor+1;j<n;j++){if(levels[j]<=minimum){stop=last;break;}last=j;level=Math.Min(level,levels[j]);stop=j;}}
    float start,end;if(stop==cursor){if(hook[cursor]>=0){start=x[cursor];end=0;}else{start=0;end=x[cursor];}}else{start=x[cursor];end=x[stop];}
    current[slot]=prior*9+level;current[slot+1]=start;current[slot+2]=end;triplets++;
    for(int j=cursor;j<=stop;j++)if(levels[j]==level)levels[j]=minimum;int next=stop+1;for(int j=cursor;j<=stop;j++)if(levels[j]>level){next=j;break;}cursor=next;prior=level;
    if(slot==9)slot=12;else{result.Add(current);current=(float[])prototype.Clone();slot=9;triplets=0;}
   }
   if(triplets!=0)result.Add(current);return result;
  }
 }
}
````

## InputBeamStemCompletion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputBeamStemCompletion.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputBeamStemCompletion {
  // Overlay2/index17, same-direction beam path, including cue/grace and cross-staff notes. This runs on compact
  // items after input compaction, independently of screen/EPS rendering.
  public static List<float[]> Apply(IList<float[]> items,int firstItem,InputEndpointStaffContext context=null){
   var rows=new List<float[]>();foreach(var p in items)rows.Add((float[])p.Clone());
   int priorStart=0,priorPitch=0;bool hadBeam=false;
   for(int i=firstItem-1;i<rows.Count;i++){
    var beam=rows[i];if(P(beam,1)!=6||beam.Length<7)continue;int descriptor=(short)Math.Truncate(P(beam,7));if(descriptor<0)continue;
    int direction=descriptor/10;if(direction!=1&&direction!=2)throw new NotSupportedException("Beam direction branch");
    if(P(beam,9)!=0||P(beam,11)<0||P(beam,14)<0||P(beam,18)!=0)throw new NotSupportedException("Reversed or interrupted beam branch");
    bool cue=Math.Abs(P(beam,4))>=100;float factor=cue?0.6000000238418579f:1;
    float start=F((double)P(beam,3)-0.009999999776482582),end=F((double)P(beam,6)+0.009999999776482582),pitch=P(beam,4)%100;
    // The 4.01 input saves use the stored endpoints for interpolation. The
    // two-note and asymmetric fixtures distinguish this from the 3.11
    // padded endpoints and binary32 slope/intercept stores.
    bool scor4=context!=null&&context.Scor4;
    double anchor=scor4?P(beam,3):start;
    double span=scor4?(double)P(beam,6)-anchor:F((double)end-start);
    if(span==0)return rows;
    double slope=((double)P(beam,5)-pitch)/span;
    if(!scor4)slope=F(slope);
    double intercept=pitch-anchor*slope;
    if(!scor4)intercept=F(intercept);
    bool repeated=hadBeam&&Bits(start)==priorStart&&Bits(pitch)==priorPitch;
    for(int j=firstItem-1;j<rows.Count;j++){
     var note=rows[j];if(P(note,1)!=1||P(note,2)!=P(beam,2)||note.Length<5||P(note,3)<start||P(note,3)>end)continue;
     int noteDirection=((short)Math.Truncate(P(note,5))%100)/10;if(noteDirection==0)continue;
     if((Math.Abs(P(note,4))>=100)!=cue)continue;
     if(P(note,12)!=0&&context==null||P(note,15)!=0)throw new NotSupportedException("Opposite stem, cross-staff or scaled-note branch");
     bool opposed=noteDirection!=direction;
     float noteX=P(note,3),evaluationX=noteX;
     if(opposed)evaluationX=F((double)noteX+F((double)factor*(direction==1?-2.833329916000366:2.833329916000366)));
     float extension=F((double)evaluationX*slope+intercept-(cue?P(note,4)%100:P(note,4)));
     if(opposed){
      float height=F((double)factor*14);if(direction==1)height=-height;
      extension=F((double)height-extension);
      float depth=F((double)(descriptor%10-1)*1.5);
      depth=F((double)depth+OpposedRangeDepth(beam,11,noteX));
      depth=F((double)depth+OpposedRangeDepth(beam,14,noteX));
      if(direction==2)extension=-extension;
      extension=F((double)F((double)depth*factor)+extension);
     }else if(direction==2)extension=-extension;
     if(context!=null)context.AdjustBeamStem(note,noteDirection,ref extension);
     if(cue&&opposed)extension=F((double)extension+5.599999904632568);
     extension=F((double)extension/factor);
     if(note.Length<8){Array.Resize(ref note,8);rows[j]=note;}
     if(!repeated||P(note,8)<extension)note[7]=extension;
     if(note.Length>8){int flag=(short)Math.Truncate(note[8])%10;if(flag!=0)note[8]=F((double)note[8]-flag);}
    }
    priorStart=Bits(start);priorPitch=Bits(pitch);hadBeam=true;
   }
   return rows;
  }
  // 2EB6:0A16/0B0D: the range's preceding word encodes its depth.
  // A zero endpoint disables it; a negative endpoint uses the beam endpoint.
  static float OpposedRangeDepth(float[] beam,int field,float x){
   if(beam.Length<field+1)return 0;
   float start=P(beam,field),end=P(beam,field+1);
   if(start<0)start=P(beam,3);else if(start==0)return 0;
   if(end<0)end=P(beam,6);else if(end==0)return 0;
   int descriptor=(short)Math.Truncate(P(beam,field-1));
   if(descriptor<0||x<start||x>end)return 0;
   return F((double)(descriptor%10)*1.5);
  }
  static float P(float[] p,int n){return p.Length>=n?p[n-1]:0;}static float F(double x){return (float)x;}static int Bits(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0);}
 }
}
````

## InputChordDots

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputChordDots.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordDots {
  static bool HasBits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  // Section56 1B62: dotted chord controls stored in decimal orders of P9.
  public static void Apply(IList<float[]> rows){if(rows.Count>0)ApplyRange(rows,0,rows.Count);}
  public static void ApplyRange(IList<float[]> rows,int first,int countLimit){
   int at=first;while(at>=0&&at<rows.Count){
    if(rows[at][0]!=1||rows[at][8]<10){if(at>=countLimit-2)return;at++;continue;}int end=at;do{int next=end+1;if(next>=rows.Count||rows[next][0]!=1||rows[next][4]>=10)break;end=next;if(next>=countLimit-1)break;}while(true);
    if(end==at){if(at>=countLimit-2)return;at++;continue;}int found=-1;for(int i=at;i<=end;i++)if(HasBits(rows[i][9])){found=i;break;}
    if(found>=0){bool up=rows[at][4]<20;int mode=0;
     if(!up&&found>0)rows[found-1][8]=(float)((double)rows[found-1][8]%100);
     int start=up?end:at,finish=up?at:end,step=up?-1:1;
     for(int i=start;;i+=step){float shift=0,extra=0;
      if(up){if(!HasBits(rows[i][9])){extra=1000;if(i!=start){if(rows[i+1][3]-rows[i][3]>2)mode=0;else if(mode!=0){shift=100;mode=1;}}if(rows[i][8]>=100)shift=0;rows[i][8]=(float)((double)rows[i][8]+shift+extra);}else mode=1;}
      else if(i!=start){bool change=true;if(!HasBits(rows[i][9])){if(rows[i-1][3]-rows[i][3]>2)mode=0;else if(mode!=0){shift=100;mode=1;}if(mode==0)change=false;}else{extra=1000;shift=100;mode=1;}if(change){if(rows[i][8]>=100)shift=0;rows[i][8]=(float)((double)rows[i][8]+shift+extra);}}
      if(i==finish)break;
     }
    }
    at=end;if(at>=countLimit-2)return;at++;
   }
  }
 }
}
````

## InputChordLayout

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputChordLayout.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordLayout {
  // Section56 0D2E and 1755: original stage1 chord displacement and
  // accidental packing. Integer scratch controls stay separate from P5.
  static readonly short[] spacing={240,250,215,440,250,240,290,240,440,300,240,255,220,430,280,250,280,240,500,330,
155,250,180,380,200,240,290,230,440,250,200,250,160,420,180,0,280,160,0,0,
155,200,120,360,180,180,220,110,400,0,130,200,120,320,140,0,0,0,0,0,
120,180,120,350,160,0,200,110,0,0,0,120,0,0,0,0,0,0,0,0,
0,110,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
// Original DS39A8 table spills into DS3A70 data at distance7.
// The first pair is the stored exponent0.5849624872; preserve these
// measured adjacent words rather than rejecting the reachable lookup.
-16358,16149,10512,0,14136,14381,19761,93,10512,59,14136,5164,129,4,5,6,7,8,9,10};
  static readonly int[] below={0,1,3,3,1,1},above={0,5,4,4,5,1};
  static int W(float p){return unchecked((short)(int)Math.Truncate(p));}
  static int Gap(int type,ref int other,int distance){int d=distance-1;if(d<1)d=1;if(other==4)other=1;else if(other==5)other=4;if(type<1||type>5||other<1||other>4||d>6)throw new NotSupportedException("Chord separation lookup outside its recovered table");return spacing[(d-1)*20+(other-1)*5+type-1];}
  // start/end are zero-based inclusive. The return is the original mutated
  // endpoint argument, not an application item index.
  public static int Apply(IList<float[]> rows,int start,int end){
   if(start<0||end>=rows.Count||end<=start)throw new ArgumentException("Chord range");int n=end-start+1,direction=rows[start][4]%100>=20?20:10;
   for(int i=start+1;i<=end;i++){if(Math.Abs(W(rows[i][3])%100-W(rows[i-1][3])%100)<2){rows[i][9]=direction;i++;}}
   int[] pitch=new int[n+1],record=new int[n+1],kind=new int[n+1],low=new int[n+1],high=new int[n+1],column=new int[n+1],basis=new int[n+1];
   for(int k=1;k<=n;k++){int i=direction==20?start+k-1:end-k+1;record[k]=i;pitch[k]=W(rows[i][3])%100;if(rows[i][9]==20)basis[k]=column[k]=283;kind[k]=W(rows[i][4]%10);if(kind[k]!=0){if(kind[k]<1||kind[k]>5)throw new NotSupportedException("Accidental type");low[k]=pitch[k]-below[kind[k]];high[k]=pitch[k]+above[kind[k]];}}
   for(int i=1;i<=n;i++)if(kind[i]!=0&&basis[i]==0)for(int j=1;j<=n;j++)if(basis[j]!=0&&j!=i&&pitch[j]<=high[i]&&low[i]<=pitch[j])column[i]=283;
   int at=1;
   while(at<n){if(kind[at]!=0){int best=999,target=0,extra=0;for(int j=at+1;j<=n;j++)if(kind[j]>0){int difference=pitch[at]-pitch[j],other=kind[j],gap=difference<7?Gap(kind[at],ref other,difference):0;if(gap<best){best=gap;extra=gap;target=j;}}
     if(target!=0){int position=column[at]+extra;if(column[target]<position)column[target]=position;for(int j=at+1;j<target;j++)if(kind[j]!=0){position+=240;if(kind[j]==4)position+=200;if(column[j]<position)column[j]=position;}at=target;continue;}}
    at++;
   }
   for(int i=1;i<n;i++)if(kind[i]!=0&&column[i]!=0){int lower=low[i],position=column[i];for(int j=i+1;j<=n;j++)if(kind[j]!=0){if(high[j]<=lower)break;int old=column[j],other=kind[j],difference=position-old;if(other==4)difference-=200;if(Math.Abs(difference)<240){int gap=Gap(kind[i],ref other,pitch[i]-pitch[j]);if(difference>0){int threshold=other==4?0:200;if(old==0||threshold<position-old||gap>200){int shift=240-(position-old);if(other==4)shift+=200;if(shift>0)for(int k=i;k<j;k++)if(kind[k]!=0&&column[k]!=0)column[k]+=shift;}else column[j]=gap+position;break;}column[j]=gap+position;}}}
   for(int k=1;k<=n;k++){int shift=Math.Max(0,Math.Min(899,column[k]-basis[k]));rows[record[k]][4]=(float)(shift/1000.0+rows[record[k]][4]);}
   return n+1;
  }
 }
}
````

## InputChordPitchRecords

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputChordPitchRecords.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordPitchRecords {
  // Section55 chord-ordering and record path, restricted to treble natural
  // notes separated by at least a third. The accidental-column and adjacent
  // notehead displacement branches are intentionally outside this entry point.
  public static InputPitchResult CompileSpacedNaturals(float[] source,float staff,float left,float right){
   if(source==null||source.Length==0||source[0]!=3000)throw new NotSupportedException("This chord projection requires an initial TR token");
   var tokens=(float[])source.Clone();int positive=0;
   for(int i=0;i<tokens.Length;i++){int word=(int)tokens[i];if(word>=0)positive++;if(i==0)continue;if(Math.Abs(word)/1000!=1||Math.Abs(word)%1000>=100)throw new NotSupportedException("Natural treble notes required");if(word<0)continue;
    int end=i+1;while(end<tokens.Length&&tokens[end]<0)end++;if(end-i<2)continue;
    var group=new List<int>();for(int j=i;j<end;j++)group.Add(Math.Abs((int)tokens[j]));group.Sort();for(int j=1;j<group.Count;j++)if(group[j]%100-group[j-1]%100<2)throw new NotSupportedException("Adjacent/unison chord branch");
    int low=group[0]%100-28,high=group[group.Count-1]%100-28,balance;
    if(group.Count==2||high-7!=7-low)balance=14-high-low;
    else{balance=0;for(int j=1;j<group.Count-1;j++){int pitch=group[j]%100-28;if(pitch<8)balance++;if(pitch>7)balance--;}}
    if(balance<=0)group.Reverse();for(int j=0;j<group.Count;j++)tokens[i+j]=j==0?group[j]:-group[j];
   }
   var absolute=new float[tokens.Length];for(int i=0;i<tokens.Length;i++)absolute[i]=Math.Abs(tokens[i]);var raw=InputPitchRecords.CompileSingleNotes(absolute,staff,0,left,right,true);var result=new InputPitchResult{Left=raw.Left,FinalClef=raw.FinalClef};float step=(float)(((double)right-result.Left)/(positive+1));int position=0,leader=-1;
   for(int i=0;i<tokens.Length;i++){var p=raw.Records[i];if(tokens[i]>=0){position++;if(i>0){leader=i;result.Events++;result.NoteIndices.Add(i+1);result.ForcedStems.Add(0);}}
    else{var root=raw.Records[leader];root[7]=Math.Max(root[7],(float)Math.Abs((double)root[3]-p[3]));p[4]=0;p[7]=0;p[14]=-1;}
    p[2]=(float)((double)position*step+result.Left);result.Records.Add(p);
   }
   return result;
  }
 }
}
````

## InputChordRhythmRecords

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputChordRhythmRecords.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordRhythmRecords {
  // Section57's ordinary chord-member loop: one rhythm event advances the
  // whole chord. Secondary members keep the duration/head, without flags.
  // Reference interpolation follows this projection.
  public static List<float[]> Apply(IList<float[]> source,float[] encoded,float left,float right,float scale,int[] stems){
   return Apply(source,encoded,left,right,scale,stems,false,1);
  }
  public static List<float[]> Apply(IList<float[]> source,float[] encoded,float left,float right,float scale,int[] stems,bool proportional,float exponent,float? referenceStart=null,short? retainedGraceWord=null,float? rememberedLeft=null){
   var reduced=new List<float[]>();var leaders=new List<int>();int current=-1;
   foreach(var p in source){
    if(p[0]==1&&Math.Abs(p[3])>=200)throw new NotSupportedException("Chord pitch controls.");
    if(p[0]==1&&p[14]<0){
     if(p[14]!=-1||current<0)throw new NotSupportedException("Chord member controls.");
    }else{
     current=reduced.Count;
     reduced.Add((float[])p.Clone());
    }
    leaders.Add(current);
   }
   var laid=InputRhythmRecords.Apply(reduced,encoded,left,right,scale,proportional,exponent,stems,referenceStart,rememberedLeft);
   var result=new List<float[]>();
   for(int i=0;i<source.Count;i++){
    var old=source[i];var leader=laid[leaders[i]];
    bool grouped=old[14]<0||(i+1<source.Count&&leaders[i+1]==leaders[i]);
    if(old[0]==1&&old[11]!=0&&old[11]!=1&&old[11]!=2)
     throw new NotSupportedException("Chord staff displacement leaves the F2 S+/S- controls.");
    if(old[0]!=1||old[14]>=0){
     if(old[0]==1&&Math.Abs(old[3])>=100&&leader[6]!=65&&grouped)
      leader[7]=Math.Max(leader[7],2*old[7]);
     result.Add(leader);
     continue;
    }
    var member=(float[])old.Clone();
    member[2]=leader[2];member[5]=leader[5];member[6]=leader[6];
    member[8]=(float)((double)leader[8]-leader[12]);member[12]=0;
    result.Add(member);
   }
   for(int first=0;first<source.Count;first++){
    if(result[first][0]!=1||result[first][6]!=65||source[first][14]<0)continue;
    int last=first;
    while(last+1<source.Count&&source[last+1][0]==1&&source[last+1][14]<0)last++;
    if(last>first)ProjectGraceChord(source,result,first,last,retainedGraceWord);
    first=last;
   }
   InputChordDots.Apply(result);return result;
  }
  static void ProjectGraceChord(IList<float[]> source,IList<float[]> result,int first,int last,short? retainedGraceWord){
   int count=last-first+1,direction=result[first][4]%100>=20?20:10;
   // The automatic-stem branch has already removed the leader's marker.
   bool preserveLeaderParentheses=result[first][4]>=100;
   var pitches=new int[count];
   var accidentals=new float[count];
   for(int n=0;n<count;n++){
    var pitch=source[first+n];
    pitches[n]=(int)pitch[3]%100;
    accidentals[n]=(int)pitch[4]%10+(pitch[4]>=100?100:0);
   }
   Array.Sort(pitches,accidentals);
   for(int n=1;n<count;n++)if(pitches[n]==pitches[n-1])
    throw new NotSupportedException("Repeated pitches in a grace chord can stall the original SCORE.");
   for(int n=0;n<count;n++){
    int index=direction==10?n:count-n-1,pitch=pitches[index];
    var row=result[first+n];
    row[3]=pitch+(pitch<0?-100:100);
    row[4]=n==0?direction+(preserveLeaderParentheses?accidentals[index]:accidentals[index]%100):accidentals[index];
    row[9]=0;
    if(n>0){row[6]=64;row[8]=0;}
   }
   int span=Math.Max(0,pitches[count-1])-pitches[0];
   // 2CBE:0DA0 skips the span assignment at 0F50 when the working header
   // is not 2. Section57:0CA6 then consumes the retained DS:B970 word.
   int spanWord=retainedGraceWord.HasValue?retainedGraceWord.Value:span+61;
   result[first][7]=(float)(spanWord*1.666599988937378-.33329999446868896);
   InputChordLayout.Apply(result,first,last);
  }
 }
}
````

## InputChromaticPitchCorrection

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputChromaticPitchCorrection.cs)

````csharp
using System;using System.Linq;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Modern opt-in correction. The recovered transposition path remains unchanged.
 // Preserve sounding pitch when the original's accidental-limit respelling crosses
 // a diatonic neighbour in the wrong direction. Rows retain index correspondence.
 public static class InputChromaticPitchCorrection {
  static readonly int[] natural={0,2,4,5,7,9,11},clefOffsets={0,-12,-6,-8},sharps={3,0,4,1,5,2,6},flats={6,2,5,1,4,0,3};
  static float P(float[] row,int n){return n<=row.Length?row[n-1]:0;}
  static int[] Order(IList<float[]> rows,short staff){return Enumerable.Range(0,rows.Count).Where(n=>P(rows[n],2)==staff).OrderBy(n=>P(rows[n],3)).ThenBy(n=>P(rows[n],1)==3?0:P(rows[n],1)==17?1:P(rows[n],1)==14?2:3).ToArray();}
  static int Letter(int position,out int octave){octave=(int)Math.Floor((position-1)/7.0);return position-1-octave*7;}
  static int Base(int position){int octave;int letter=Letter(position,out octave);return 60+12*octave+natural[letter];}
  static int KeyAlter(int position,int key){int octave;int letter=Letter(position,out octave);return key>0&&sharps.Take(key).Contains(letter)?1:key<0&&flats.Take(-key).Contains(letter)?-1:0;}
  static int Alter(int digit){switch(digit){case 1:return -1;case 2:return 1;case 3:return 0;case 4:return -2;case 5:return 2;default:throw new NotSupportedException("Chromatic correction requires conventional accidentals.");}}
  static float Cue(float pitch){return pitch+(pitch<0?-100:100);}
  static int Digit(int alter){return alter==-2?4:alter==-1?1:alter==0?3:alter==1?2:5;}
  static void Context(float[] row,ref int clef,ref int key,Dictionary<int,int> accidentals,Dictionary<int,int> custom){int code=(int)P(row,1);if(code==3){clef=(int)P(row,5);if(clef<0||clef>3)throw new NotSupportedException("Chromatic correction requires a standard clef.");}else if(code==14)accidentals.Clear();else if(code==17){
   if(P(row,8)!=0){key=0;custom.Clear();foreach(int field in new[]{8,9,10,11,12,13,16}){int glyph=(int)P(row,field);if(glyph==0)continue;int type=glyph/100,octave;int letter=Letter(glyph%100+(int)P(row,4)+clefOffsets[clef],out octave);if(type<1||type>3)throw new NotSupportedException("Custom key accidental must be flat, sharp or natural.");custom[letter]=type==1?-1:type==2?1:0;}accidentals.Clear();}
   else if(Math.Abs(P(row,5))<=7){key=(int)P(row,5);custom.Clear();accidentals.Clear();}
  }}
  static int Sound(float[] row,int clef,int key,Dictionary<int,int> accidentals,Dictionary<int,int> custom){int position=(int)P(row,4)%100+clefOffsets[clef],digit=Math.Abs((int)P(row,5))%10,alter=KeyAlter(position,key),octave;int letter=Letter(position,out octave);if(custom.ContainsKey(letter))alter=custom[letter];if(digit!=0){alter=Alter(digit);accidentals[position]=alter;}else if(accidentals.ContainsKey(position))alter=accidentals[position];return Base(position)+alter;}
  public static Dictionary<int,int> Apply(IList<float[]> before,IList<float[]> after,short staff,short semitones){
   if(before==null||after==null||after.Count<before.Count)throw new ArgumentException("Transposition rows lost correspondence.");
   var expected=new Dictionary<int,int>();var accidentals=new Dictionary<int,int>();int clef=0,key=0;var custom=new Dictionary<int,int>();
   foreach(int n in Order(before,staff)){var row=before[n];Context(row,ref clef,ref key,accidentals,custom);if(P(row,1)==1)expected.Add(n,Sound(row,clef,key,accidentals,custom)+semitones);}
   // Zero-key placeholders are semantic changes even though the original erases
   // their drawing code. Retain them for modern carry/cancellation processing.
   for(int n=0;n<before.Count;n++)if(P(before[n],1)==17&&P(after[n],1)==13&&P(after[n],5)==0)after[n][0]=17;
   for(int n=0;n<before.Count;n++)if(P(before[n],1)==17&&P(before[n],8)!=0)after[n]=(float[])before[n].Clone();
   var corrections=new Dictionary<int,int>();accidentals.Clear();custom.Clear();clef=key=0;
   foreach(int n in Order(after,staff)){var row=after[n];Context(row,ref clef,ref key,accidentals,custom);if(P(row,1)!=1||!expected.ContainsKey(n))continue;bool cue=Math.Abs(P(before[n],4))>=100;if(cue&&Math.Abs(row[3])<100)row[3]=Cue(row[3]);int position=(int)P(row,4)%100+clefOffsets[clef];var prior=new Dictionary<int,int>(accidentals);int actual=Sound(row,clef,key,accidentals,custom),desired=expected[n];if(actual==desired)continue;
    int candidate=position,alter=desired-Base(candidate);bool found=Math.Abs(alter)<=2;
    for(int distance=1;!found&&distance<=32;distance++)foreach(int direction in new[]{-1,1}){int possible=position+direction*distance,difference=desired-Base(possible);if(Math.Abs(difference)<=2){candidate=possible;alter=difference;found=true;break;}}
    if(!found)throw new InvalidOperationException("No conventional spelling for transposed pitch.");
    accidentals.Clear();foreach(var pair in prior)accidentals.Add(pair.Key,pair.Value);
    int delta=candidate-position;row[3]=cue?Cue(row[3]%100+delta):row[3]+delta;int oldDigit=Math.Abs((int)row[4])%10;row[4]=row[4]-oldDigit+Digit(alter);accidentals[candidate]=alter;if(delta!=0)corrections[n]=delta;
    if(Sound(row,clef,key,accidentals,custom)!=desired)throw new InvalidOperationException("Chromatic correction did not conserve pitch.");
   }return corrections;
  }
 }
}
````

## InputDynamics

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputDynamics.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputDynamics {
  // Section60 default-side Code9 dynamics path and 0DE2 position correction.
  // The mark-list parser and opposite-side descriptor control are separate.
  public static float[] Create(int symbol,float[] note,float staff,float preference){
   if(symbol<50||symbol>58)throw new NotSupportedException("Outside the ordinary dynamic-symbol branch");if(note==null||note.Length!=15||note[0]!=1)throw new ArgumentException("Working note required");
   int pitch=((short)Math.Truncate(note[3]))%100;float correction=0;
   if(note[4]%100<20&&pitch<=2)correction=pitch-3;
   var p=new float[15];p[0]=9;p[1]=staff;p[2]=note[2];p[3]=(float)((double)correction-2);p[4]=symbol;p[5]=1;p[14]=preference;return p;
  }
 }
}
````

## InputEditInverse

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputEditInverse.cs)

````csharp
using System;using System.Collections.Generic;using System.Globalization;using System.Linq;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputEditInverseResult {public string Header,Positions;public string[] Lines;public List<float[]> Prefix,Selected;public float Left,Right;public int Staff,InitialClef;}
 public static class InputEditInverse {
  static float P(float[] r,int n){return n<=r.Length?r[n-1]:0;}
  static int Word(float n){if(Single.IsNaN(n)||Single.IsInfinity(n)||n<short.MinValue||n>short.MaxValue)throw new NotSupportedException("EDI word projection leaves the recovered finite domain.");return (short)Math.Truncate(n);}
  static bool Zero(float n){return BitConverter.ToInt32(BitConverter.GetBytes(n),0)==0;}
  static bool Same(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}
  //2C53:21F0..23D1; sign-word test, repeated single-precision subtraction
  // of10000 and '+' markers, then six digit slots with no leading zero.
  public static string Number(float n){
   if(Single.IsNaN(n)||Single.IsInfinity(n))throw new NotSupportedException("EDI nonfinite numeric text is prevented.");
   string prefix="";if((BitConverter.ToUInt32(BitConverter.GetBytes(n),0)&0x80000000u)!=0){prefix="-";n=-n;}
   while(n>=10000f){float next=(float)((double)n-10000f);if(next==n||prefix.Length>=150)throw new NotSupportedException("EDI packed number would exceed the recovered line buffer.");prefix+="+";n=next;}
   int scaled=(int)((double)n*100+.019999999552965164);string result=scaled==0?"0":scaled<100?"":(scaled/100).ToString(CultureInfo.InvariantCulture);
   if(scaled%100!=0)result+="."+(scaled%100).ToString("00",CultureInfo.InvariantCulture).TrimEnd('0');return prefix+result;
  }
  static string Rhythm(float[] row,bool reproduceInvalid){float duration=P(row,7);if(Single.IsNaN(duration)||Single.IsInfinity(duration)||duration<=0)throw new NotSupportedException("EDI invalid rhythm value is prevented.");if(duration>=32)return "G";int dots=Word(P(row,1))==2?Word(P(row,6)):Word(P(row,9))%100/10;dots=Math.Max(0,dots);if(dots>255)throw new NotSupportedException("EDI extended dot branch needs further verification.");int code=(int)((double)duration*1000);if(dots==1)code=unchecked((short)(code<<1))/3;else if(dots==2)code=unchecked((short)(code<<2))/7;int[] values={250,500,1000,2000,4000,8000,333};string[] tokens={"S","E","Q","H","W","D","T"};int index=Array.IndexOf(values,code);if(index<0){float reciprocal=(float)(4.0/duration);int whole=(int)((double)reciprocal+.009999999776482582);if(whole<0||whole>32767)throw new NotSupportedException("EDI numeric rhythm leaves the recovered signed-word writer.");if(whole>99&&!reproduceInvalid)throw new NotSupportedException("Original EDI emits nonnumeric rhythm digits above 99; this invalid input is prevented unless original invalid states are enabled.");double fraction=(double)reciprocal-whole;string numeric=whole<10?whole.ToString(CultureInfo.InvariantCulture):System.Text.Encoding.GetEncoding(437).GetString(new[]{unchecked((byte)(whole/10+48)),(byte)(whole%10+48)});if(fraction>=.009999999776482582){int decimalWord=(int)(((double)reciprocal%1)*10000);numeric+="."+decimalWord.ToString("0000",CultureInfo.InvariantCulture);}return numeric+new string('.',dots);}return tokens[index]+new string('.',dots);}
  //2C53:1E35..2084 removes repeated token bodies, retains slash repeats,
  //then replaces runs of four or more events with X and their count.
  //This bounded projection only supplies ordinary rhythm tokens here.
  static string CompressRhythms(IList<string> tokens,bool reproduceInvalid){var result=new List<string>();for(int i=0;i<tokens.Count;){int end=i+1;while(end<tokens.Count&&tokens[end]==tokens[i])end++;int count=end-i;if(count>99&&!reproduceInvalid)throw new NotSupportedException("Original EDI emits nonnumeric repetition counts above 99; this invalid input is prevented unless original invalid states are enabled.");string digits=count<10?count.ToString(CultureInfo.InvariantCulture):System.Text.Encoding.GetEncoding(437).GetString(new[]{unchecked((byte)(count/10+48)),(byte)(count%10+48)});result.Add(tokens[i]+(count>=4?"X"+digits:new string('/',count-1)));i=end;}return String.Join("/",result);}
  //2C53:23D4..2501: original note-anchor interpolation, including its .1 tolerance.
  static float EventPosition(IList<float> anchors,float x,float left,float right){if(anchors.Count==0||Single.IsNaN(x)||Single.IsInfinity(x))throw new NotSupportedException("EDI event projection needs finite note anchors.");int index=0;for(;index<anchors.Count;index++){double difference=(double)x-anchors[index];if(Math.Abs(difference)<.10000000149011612)return index+1;if((float)difference<0)break;}float lower=index==0?left:anchors[index-1],upper=index<anchors.Count?anchors[index]:right,denominator=(float)((double)upper-lower);if(denominator==0)return 0;return(float)((((double)x-lower)/denominator+index+1)-1);}
  // Recovered collector0004..049A and ordinary note/clef paths of
  //049B..173D. Unported selected codes stop before any caller mutation.
  public static InputEditInverseResult Project(IList<float[]> source,int staff,float left,float right,bool sorted=false){return Project(source,staff,left,right,sorted,false);}
  public static InputEditInverseResult Project(IList<float[]> source,int staff,float left,float right,bool sorted,bool documentedRepeatedMarks){return Project(source,staff,left,right,sorted,documentedRepeatedMarks,false);}
  public static InputEditInverseResult Project(IList<float[]> source,int staff,float left,float right,bool sorted,bool documentedRepeatedMarks,bool reproduceInvalid){return Project(source,staff,left,right,sorted,documentedRepeatedMarks,reproduceInvalid,null);}
  public static InputEditInverseResult Project(IList<float[]> source,int staff,float left,float right,bool sorted,bool documentedRepeatedMarks,bool reproduceInvalid,float? verifiedTrailingWord){
   if(staff<1||staff>32)throw new NotSupportedException("EDI staff leaves the original table.");if(Zero(right))right=200;var order=InputTranspositionOrder.OrderForMain(source,sorted);var rows=order.Select(i=>(float[])source[i].Clone()).ToList();var selected=new List<float[]>();var prefix=new List<float[]>();int preceding=-1;float first=999,end=0;bool finished=false;
   for(int rowIndex=0;rowIndex<rows.Count;rowIndex++){var row=rows[rowIndex];int code=Word(P(row,1));float x=P(row,3);bool eligible=Word(P(row,2))==staff;
    if(eligible&&code==3&&x<left){preceding=row.Length<5?0:Word(P(row,5));eligible=false;}
    if(x<left||code==8||code>9&&code<17&&code!=14)eligible=false;
    if(eligible&&code==1&&row.Length>=10&&!Zero(P(row,10))&&P(row,10)!=10&&P(row,10)!=20)eligible=false;
    if(eligible&&code==4&&P(row,5)!=999)eligible=false;
    if(eligible&&code==14&&P(row,5)>=8)eligible=false;
    if(eligible&&code==6&&P(row,6)>right)eligible=false;
    if(eligible&&code==7){float collectorP7=P(row,7);if(row.Length<7){if(row.Length!=6)throw new NotSupportedException("EDI short Code7 requires its recorded adjacent state.");if(rowIndex+1<rows.Count)collectorP7=rows[rowIndex+1].Length;else{if(!verifiedTrailingWord.HasValue||!order.SequenceEqual(Enumerable.Range(0,source.Count)))throw new NotSupportedException("EDI short Code7 at the sorted-buffer tail requires its recorded adjacent state.");collectorP7=verifiedTrailingWord.Value;if(Single.IsNaN(collectorP7)||Single.IsInfinity(collectorP7))throw new NotSupportedException("EDI nonfinite trailing storage needs its original floating-comparison verification.");}}if(!Zero(collectorP7)&&Math.Abs(collectorP7)!=8)eligible=false;}
    if(eligible&&!finished){if(Math.Abs((double)right-x)<=.05000000074505806){x=right;row[2]=x;}if(x<=right){selected.Add(row);if(code!=5)first=Math.Min(first,x);continue;}if(code!=5&&code!=6){end=(float)((double)x-.10000000149011612);finished=true;}}
    prefix.Add(row);
   }
   if(selected.Count==0)throw new NotSupportedException("EDI empty-scope path has not been verified.");if(end==0)end=right;
   var pitches=new List<string>();var rhythms=new List<string>();var marks=new List<string>();var endpoints=new List<float[]>();var anchors=new List<float>();int noteCount=0,lastMark=0,staffControl=0;int clef=preceding>0?preceding:0;if(clef>4)clef=0;string[] clefs={"TR","BA","AL","TE","PR"};int[] offsets={0,12,6,8,0};if(preceding>0)pitches.Add(clefs[clef]);bool firstNote=true;int octave=-1;float previousX=-1;int stem=0;bool priorNote=false,cueEnabled=false;
   foreach(var row in selected){int code=Word(P(row,1));if(code==4||code==5||code==6||code==7||code==9){endpoints.Add(row);continue;}if(code==3){clef=row.Length>=5?Word(P(row,5)):0;if(clef<0)throw new NotSupportedException("EDI negative clef index is prevented.");if(clef>4)clef=0;pitches.Add(clefs[clef]);priorNote=false;continue;}if(code==17){int key=Word(P(row,5)),absolute=Math.Abs(key);if(row.Length>13||absolute%100>7||absolute>107)throw new NotSupportedException("EDI extended key-signature controls need inverse verification.");pitches.Add("K"+(absolute%100).ToString(CultureInfo.InvariantCulture)+(key<0?"F":"S")+(absolute>100?"N":""));priorNote=false;continue;}if(code==18){int top=Word(P(row,5)),bottom=Word(P(row,6));if(row.Length>15||top<0||top<98&&(bottom<0||bottom>99))throw new NotSupportedException("EDI extended time-signature controls need inverse verification.");pitches.Add(top>=98?(top==98?"CUT":"COM"):top.ToString(CultureInfo.InvariantCulture)+" "+bottom.ToString(CultureInfo.InvariantCulture));priorNote=false;continue;}if(code==14){int span=Word(P(row,4)),style=Word(P(row,5));if(row.Length>17||span>99||span>=0&&(style< -1||style>7))throw new NotSupportedException("EDI extended bar-line controls need inverse verification.");if(span>=0&&style==-1&&!reproduceInvalid)throw new NotSupportedException("Original EDI emits MN for this thick bar, then rejects its own input. This error path is prevented unless original invalid states are enabled.");string bar="M"+(span<0?"I":(style==-1?"N":new[]{"","D","H","RL","RR","RD","I ","DS"}[style])+(span>0?span.ToString(CultureInfo.InvariantCulture):""));pitches.Add((pitches.Count>0?" ":"")+bar);priorNote=false;continue;}if(code==2){int shape=Word(P(row,5)),restPitch=Word(P(row,4));if(shape!=-2&&shape!=-4&&P(row,8)>=0&&P(row,6)>=0){if(Math.Abs(restPitch)>=100){restPitch%=100;if(!cueEnabled){pitches.Add("Q+");cueEnabled=true;}}else if(cueEnabled){pitches.Add("Q0");cueEnabled=false;}}string rest=shape==-2?"RW":shape==-4?"RP":P(row,8)<0?"RF":P(row,6)<0?"RI":restPitch<0?"RD":restPitch>0?"RU":"R";pitches.Add(rest);rhythms.Add(Rhythm(row,reproduceInvalid));priorNote=false;continue;}if(code!=1)throw new NotSupportedException("This EDI selected item needs its recovered inverse branch.");if(!Zero(P(row,10))&&P(row,10)!=10&&P(row,10)!=20)throw new NotSupportedException("EDI extended note attachments still need inverse verification.");int staffDirection=Word(P(row,12));if(staffDirection<0||staffDirection>2)throw new NotSupportedException("EDI cross-staff control leaves its recovered three-entry table.");if(staffDirection!=staffControl){pitches.Add(new[]{"S0","S+","S-"}[staffDirection]);staffControl=staffDirection;}int rawPitch=Word(P(row,4));int pitch=rawPitch%100;int flags=Word(P(row,5));if(flags<0)throw new NotSupportedException("EDI accidental branch leaves the verified ordinary domain.");int force=flags%100/10;if(force>2)throw new NotSupportedException("EDI stem control leaves the ordinary domain.");if(force!=0&&force!=stem){bool ordinary=pitch<7?force==1:force==2;if(!ordinary||stem!=0){if(ordinary)force=0;pitches.Add(force==0?"SO":force==1?"SU":"SD");stem=force;}}
    if(Math.Abs(rawPitch)>=100&&P(row,7)<64&&!cueEnabled){pitches.Add("Q+");cueEnabled=true;}else if(Math.Abs(rawPitch)<100&&cueEnabled){pitches.Add("Q0");cueEnabled=false;}bool chord=Same(previousX,P(row,3));if(chord&&!priorNote)throw new NotSupportedException("EDI nonconsecutive same-position events need inverse verification.");previousX=P(row,3);if(!chord){noteCount++;anchors.Add(P(row,3));}int mark=Word(P(row,11));if(mark!=0){int kind=Math.Abs(mark);string[] names={"FIF","FIS","FIN","W","A","T","S","WS","AS","TS","D","U","H","FE","AR","TH","MO","IM","HW"};string descriptor=kind>=1&&kind<=19?names[kind-1]:kind>=20&&kind<=25?new[]{"TR","TRF","TRS","TRN","PL","ACT"}[kind-20]:kind>=26&&kind<=29?(documentedRepeatedMarks?new[]{"S2","S3","S4","TR"}[kind-26]:"TL"):kind>=30&&kind<=39?"F"+(kind-30).ToString(CultureInfo.InvariantCulture):null;if(descriptor==null||noteCount>99)throw new NotSupportedException("EDI attached-mark descriptor/target leaves the recovered bounded writer.");string target=noteCount.ToString(CultureInfo.InvariantCulture);if(mark==lastMark)marks[marks.Count-1]+=" "+target;else marks.Add((mark<0?"-":"")+descriptor+" "+target);lastMark=mark;}int adjusted=pitch-offsets[clef],degree=adjusted%7;if(degree<1)degree+=7;if(degree>5)degree-=7;int nextOctave=(adjusted+27)/7;if(nextOctave<0||nextOctave>9)throw new NotSupportedException("EDI pitch octave leaves the input token domain.");string note=(firstNote?"O":"")+((char)('B'+degree)).ToString();firstNote=false;if(flags>99)note+="(";int accidental=flags%10;// Original CE path emits no accidental letters for units6..9.
    if(accidental<6)note+=new[]{"","F","S","N","FF","SS"}[accidental];if(octave!=nextOctave){note+=nextOctave.ToString(CultureInfo.InvariantCulture);octave=nextOctave;}if(chord)pitches[pitches.Count-1]+="'"+note;else{pitches.Add(note);rhythms.Add(Rhythm(row,reproduceInvalid));}priorNote=true;
   }
   var beams=new List<string>();var slurs=new List<string>();int lastEndpointCode=0;float lastStart=0,lastEnd=0;foreach(var row in endpoints){int code=Word(P(row,1));if(code==4||code==7){string descriptor;if(code==4){if(P(row,5)!=999)throw new NotSupportedException("EDI non-hairpin Code4 is outside the inverse collector.");descriptor=(P(row,4)>3?"+":"")+"C"+(BitConverter.ToInt32(BitConverter.GetBytes(P(row,7)),0)<0?"-":"");}else{int control=Word(P(row,7));if(Math.Abs(control)==8)descriptor=control<0?"OB":"O";else if(control==0){int accidental=Word(P(row,8));if(accidental<0||accidental>3)throw new NotSupportedException("EDI trill accidental leaves its source letter table.");descriptor="TR"+new[]{"","F","S","N"}[accidental]+"W";}else throw new NotSupportedException("EDI Code7 inverse control needs verification.");}float finishX=code==7?(float)((double)P(row,6)-4.166999816894531):P(row,6);marks.Add(descriptor+" "+Number(EventPosition(anchors,P(row,3),first,end))+" "+Number(EventPosition(anchors,finishX,first,end)));continue;}if(code==6&&Word(P(row,7))<1){int strokes=Word(P(row,10))%10;if(strokes<0||strokes>9)throw new NotSupportedException("EDI tremolo control needs its original parser branch.");marks.Add("TM"+strokes.ToString(CultureInfo.InvariantCulture)+" "+Number(EventPosition(anchors,P(row,3),first,end)));continue;}if(code==9){int glyph=Word(P(row,5));bool named=glyph>50&&glyph<69;if(glyph<1||glyph==59)throw new NotSupportedException("EDI glyph index needs its original inverse branch.");float x=P(row,3);string markPrefix="",descriptor;if(named){x=(float)((double)x+.5);bool opposite=glyph==66||glyph==67?Word(P(row,4))<11:Word(P(row,4))>3;markPrefix=opposite?(glyph==66||glyph==67?"-":"+"):"";string[] labels={"PPP","PP","P","MP","MF","F","FF","FFF",null,"FP","SF","SFZ","CR","DI","RI","PZ","AO","AC"};descriptor=labels[glyph-51];}else{markPrefix=Word(P(row,4))>3?"+":"";descriptor="N"+glyph.ToString(CultureInfo.InvariantCulture);}marks.Add(markPrefix+descriptor+" "+Number(EventPosition(anchors,x,first,end)));continue;}float begin=EventPosition(anchors,P(row,3),first,end),finish=EventPosition(anchors,P(row,6),first,end);int endpointControl=Word(P(row,8));if(code==5&&endpointControl< -1)finish=(float)((double)finish+.00009999999747378752);if(code==lastEndpointCode&&begin==lastStart&&finish==lastEnd)continue;lastEndpointCode=code;lastStart=begin;lastEnd=finish;if(code==5&&(endpointControl==1||endpointControl==2))continue;if(code==6){begin=Math.Max(1,begin);finish=Math.Min(anchors.Count,finish);}else{if((double)finish-.0010000000474974513>anchors.Count&&(anchors.Count>begin||Word(P(row,8))==-1))finish=99;if(begin<1)begin=Word(P(row,8))==-1?99:0;}if(code==6&&endpointControl!=0)finish=(float)((double)finish+endpointControl*100);else if(code==5&&endpointControl==3){int ending=Word(P(row,9));finish=(float)((double)finish+(ending==0?3:ending)*100);}bool negativeEndpoint=code==6?P(row,7)>=20:BitConverter.ToInt32(BitConverter.GetBytes(P(row,7)),0)<0;float encodedFinish=negativeEndpoint?-finish:(float)((double)finish+10000);string endpointText=encodedFinish>=10000?"+"+Number((float)((double)encodedFinish-10000)):Number(encodedFinish);string pair=Number(begin)+" "+endpointText;(code==6?beams:slurs).Add(pair);}if(rhythms.Count==0)throw new NotSupportedException("EDI needs at least one verified timed event.");return new InputEditInverseResult{Header="IN "+staff.ToString(CultureInfo.InvariantCulture),Positions=Number(first)+" "+Number(end)+" 1",Lines=new[]{String.Join("/",pitches)+";",CompressRhythms(rhythms,reproduceInvalid)+";",String.Join("/",marks)+";",String.Join("/",beams)+";",String.Join("/",slurs)+";"},Prefix=prefix,Selected=selected,Staff=staff,InitialClef=preceding>0?preceding:0,Left=first,Right=end};
  }
 }
}
````

## InputEndpointProjection

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputEndpointProjection.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputEndpointResult {public short StartFlag,EndFlag,StartItem,EndItem,Number,StartNote,EndNote,Stem;public float StartPitch,EndPitch,StartX,EndX;}
 public static class InputEndpointProjection {
  // Single-staff numerical projection of resident 01AE:0039 and
  // its 000E/033C endpoint leaves. Parameter words and encoded decimal controls
  // are kept separate. Cross-staff changes are handled by the 054E callee.
  public static InputEndpointResult Decode(float first,float last,int stage,IList<float[]> records,int[] noteToItem,int[] stems,float beginning,float end,InputEndpointStaffContext context=null){
   int a=Number(first),b=Number(last),rawB=b,bound=noteToItem.Length;
   var result=new InputEndpointResult{StartNote=(short)a,EndNote=(short)b};
   if(b==0)b=1;if(a==0||a==99)a=1;else if(b<=a){b=a+1;result.EndNote=(short)b;}if(b>=bound)b=bound-1;
   short firstNumber,firstStem,lastNumber,lastStem;Detail(first,a,stage,records,noteToItem,stems,out result.StartItem,out result.StartPitch,out result.StartFlag,out firstNumber,out firstStem,context);Detail(last,b,stage,records,noteToItem,stems,out result.EndItem,out result.EndPitch,out result.EndFlag,out lastNumber,out lastStem,context);result.Number=firstNumber!=0?firstNumber:lastNumber;result.Stem=firstStem!=0?firstStem:lastStem;
   result.StartX=records[result.StartItem-1][2];result.EndX=records[result.EndItem-1][2];
   if(stage==5){float fa=(float)((double)first%1),fb=(float)((double)last%1);int next=rawB+1;if(HasBits(fa)){a=result.StartNote;if(a>=bound)return result;float target;if(a==0){target=result.StartX;result.StartX=beginning;}else target=records[Mapped(noteToItem,a+1,records.Count)-1][2];result.StartX=(float)(((double)target-result.StartX)*fa+result.StartX);result.StartNote=result.EndNote=1;}
    if(HasBits(fb)){if(next<=a)next=a+1;float target;if(next<bound){if(rawB<=a)result.EndX=records[result.StartItem-1][2];target=records[Mapped(noteToItem,next,records.Count)-1][2];}else target=end;result.EndX=(float)(((double)target-result.EndX)*fb+result.EndX);result.StartNote=result.EndNote=1;}
   }
   return result;
  }
  // The original tests both stored words, so negative zero is present.
  static bool HasBits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  static int Word(float value){if(float.IsNaN(value)||float.IsInfinity(value)||value<=-32768||value>=32768)throw new ArgumentOutOfRangeException("value","Outside this finite endpoint projection.");return (short)Math.Truncate(value);}
  static int Number(float value){return Math.Abs(Word(value)%100);}
  static int Mapped(int[] map,int note,int count){if(note<1||note>=map.Length||map[note]<1||map[note]>count)throw new ArgumentOutOfRangeException("note","No verified endpoint mapping for this item.");return map[note];}
  static void Detail(float encoded,int note,int stage,IList<float[]> records,int[] map,int[] stems,out short item,out float pitch,out short flag,out short number,out short stem,InputEndpointStaffContext context){
   int word=Word(encoded);stem=0;if(stage!=5&&stems!=null){if(note>=stems.Length)throw new ArgumentException("Stem map length");stem=(short)Math.Abs(stems[note]);}if(stem==0){if(word<0)stem=20;if(word>9999)stem=10;}number=(short)((Math.Abs(word)%10000)/100);item=(short)Mapped(map,note,records.Count);int selected=item-1;if(stage!=5)while(selected+1<records.Count&&records[selected+1][0]==1&&records[selected+1][14]<0)selected++;var p=records[selected];pitch=p[3];flag=0;if(Math.Abs(pitch)>=100){if(p[6]>=64)flag=1;pitch=(float)((double)pitch-(pitch<0?-100:100));}if(p[11]!=0){if(context==null)throw new NotSupportedException("Cross-staff endpoint requires its measured staff-table context.");context.Adjust(p,ref pitch);}
  }
 }
}
````

## InputEndpointStaffContext

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputEndpointStaffContext.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputEndpointStaffContext {
  readonly float[] bases,scales;readonly int staff;
  internal readonly bool Scor4;
  public InputEndpointStaffContext(float[] bases,float[] scales,int staff):this(bases,scales,staff,false){}
  public InputEndpointStaffContext(float[] bases,float[] scales,int staff,bool scor4){if(bases==null||scales==null||bases.Length<33||scales.Length<33||staff<1||staff>32)throw new ArgumentException("SCORE staff table context required.");this.bases=(float[])bases.Clone();this.scales=(float[])scales.Clone();this.staff=staff;Scor4=scor4;}
  public InputEndpointStaffContext ForStaff(int value){return new InputEndpointStaffContext(bases,scales,value,Scor4);}
  // Resident 09D2:054E..060D: P12 is truncated, boundary controls are
  // cleared in the working record, and the endpoint pitch is shifted using
  // the raw staff baselines and the active (DS:8040) staff scale.
  // Overlay2/index17 stem completion uses a separate, stored-float
  // magnitude and the note direction, after the beam-direction negation.
  public void AdjustBeamStem(float[] row,int direction,ref float extension){
   if(row.Length<12)return;int control=(short)Math.Truncate(row[11]);if(control<1||control>2)return;
   if(staff==1&&control==2||staff==32&&control==1){row[11]=0;return;}
   int target=staff-(control*2-3);float scale=scales[staff];
   if(!(scale>0)||Single.IsInfinity(scale))throw new NotSupportedException("Cross-staff stem scale outside the positive finite domain.");
   float distance=(float)(((double)bases[staff]-bases[target])/((double)scale*7));
   if((long)distance<0)distance=-distance;if(direction==control)distance=-distance;
   extension=(float)((double)extension+distance);
  }
  public void Adjust(float[] row,ref float pitch){int control=(short)Math.Truncate(row[11]);if(control==0)return;if(control!=1&&control!=2)throw new NotSupportedException("Cross-staff control outside the F2 S+/S- domain.");if(staff==1&&control==2||staff==32&&control==1){row[11]=0;return;}int target=staff-(control*2-3);float scale=scales[staff];if(!(scale>0)||Single.IsInfinity(scale))throw new NotSupportedException("Cross-staff endpoint scale outside the positive finite domain.");pitch=(float)(((double)bases[target]-bases[staff])/((double)scale*7)+pitch);}
 }
}
````

## InputEndpointText

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputEndpointText.cs)

````csharp
using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputAutomaticBeamMeterException:FormatException {
  public InputAutomaticBeamMeterException(int top,int bottom):base("Cannot auto-beam time signature "+top.ToString(CultureInfo.InvariantCulture).PadLeft(2)+"/"+bottom.ToString(CultureInfo.InvariantCulture).PadLeft(2)+" Correct line then press <Enter>. Press <ESC> to Exit."){}
 }
 public sealed class InputEndpointRepromptException:FormatException {}
 public sealed class InputEndpointTokens {public float[] Pairs;public bool Automatic;public float GroupControl;public int Skip;}
 public static class InputEndpointText {
  // Overlay8 section69: the leading '+' control is encoded on the first
  // endpoint, independently of the numeric sign of the first endpoint.
  public static InputEndpointTokens Parse(string text,bool beams,int meterTop,int meterBottom){
   if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(!text.EndsWith(";"))throw new InvalidDataException("Terminate endpoints with a semicolon");text=text.Substring(0,text.Length-1).Trim();var result=new InputEndpointTokens{Pairs=new float[0]};if(text.Length==0)return result;
   if(beams){if(text=="A"||text=="B"||text=="AB"){int group;if(meterBottom==0||meterBottom==4||meterTop==99)group=2;else if(meterBottom==2||meterTop==98)group=4;else if(meterBottom==1)group=8;else if(meterBottom==8){if(meterTop==2||meterTop==4||meterTop==8)group=2;else if(meterTop==3||meterTop==6||meterTop==9||meterTop==12)group=3;else throw new InputAutomaticBeamMeterException(meterTop,meterBottom);}else throw new InputAutomaticBeamMeterException(meterTop,meterBottom);result.Automatic=true;result.GroupControl=group;return result;}
    Match auto=Regex.Match(text,@"^[+-]?((?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+))B\s*([+-]?(?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+))?$");if(auto.Success){result.Automatic=true;result.GroupControl=Number(auto.Groups[1].Value);result.Skip=auto.Groups[2].Success?checked((short)Number(auto.Groups[2].Value)):0;return result;}}
   var pairs=new List<float>();foreach(string field in text.Split('/')){
    string part=field.Trim();if(part.Length==0)continue;int at=0;
    bool forceUp=part[at]=='+';if(forceUp)at++;
    float first=InputRealReader.Read(part,ref at);
    if(at<part.Length)at++;while(at<part.Length&&part[at]==' ')at++;
    if(at<part.Length&&part[at]=='+'){forceUp=true;at++;}
    float last=InputRealReader.Read(part,ref at);
    if(forceUp)first=(float)((double)first+10000);pairs.Add(first);pairs.Add(last);
   }result.Pairs=pairs.ToArray();return result;
  }
  public static InputEndpointTokens Parse(string text,bool beams,int meterTop,int meterBottom,InputMacroContext context){
   if(context==null)throw new ArgumentNullException("context");var pairs=new List<float>();InputEndpointTokens automatic=null;
   foreach(string field in InputTextLists.ExpansionFields(text.ToUpperInvariant())){string part=field.Trim();if(part.Length==0)continue;Match definition=Regex.Match(part,@"^([A-Z])\[(.*)\]$");if(definition.Success){char name=definition.Groups[1].Value[0];var body=Parse(definition.Groups[2].Value+";",beams,meterTop,meterBottom,context);if(body.Automatic){
     string source=definition.Groups[2].Value.Trim();
     if(source=="A"||source=="B"||source=="AB")throw new InvalidDataException("Store a numeric automatic-beam interval in a motive.");
     if(automatic!=null||pairs.Count!=0)throw new InputEndpointRepromptException();
     context.Define(name,new[]{9999f,body.GroupControl,body.Skip});
     automatic=body;
    }else{
     if(automatic!=null)throw new InputEndpointRepromptException();
     context.Define(name,body.Pairs);
     pairs.AddRange(body.Pairs);
    }
    continue;}Match recall=Regex.Match(part,@"^@(-?)([A-Z])([+\-0-9. ]*)$");
    if(recall.Success){
     string suffix=recall.Groups[3].Value.Trim();
     if(suffix.Length>0)Number(suffix);
     char name=recall.Groups[2].Value[0];
     float[] body;
     if(!context.Definitions.TryGetValue(name,out body))throw new InvalidDataException("Undefined input motive "+name);
     if(beams&&body.Length==3&&body[0]==9999){
      if(automatic!=null||pairs.Count!=0)throw new InputEndpointRepromptException();
      automatic=new InputEndpointTokens{Automatic=true,GroupControl=body[1],Skip=checked((short)body[2]),Pairs=new float[0]};
      continue;
     }
     if(automatic!=null)throw new InputEndpointRepromptException();
     if(body.Length%2!=0)throw new InvalidDataException("Endpoint motive requires paired words.");
     if(recall.Groups[1].Length==0)pairs.AddRange(body);
     else for(int i=body.Length-2;i>=0;i-=2){
      pairs.Add(body[i]);
      pairs.Add(body[i+1]);
     }
     continue;
    }var parsed=Parse(part+";",beams,meterTop,meterBottom);if(parsed.Automatic){if(automatic!=null||pairs.Count!=0)throw new InputEndpointRepromptException();automatic=parsed;}else{if(automatic!=null)throw new InputEndpointRepromptException();pairs.AddRange(parsed.Pairs);}}
   return automatic??new InputEndpointTokens{Pairs=pairs.ToArray()};
  }
  static float Number(string text){int at=0;float value=InputRealReader.Read(text,ref at);if(at!=text.Length)throw new NotSupportedException("Automatic-beam number reaches an unverified real-reader boundary.");return value;}
 }
}
````

## InputListExpansion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputListExpansion.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputListExpansion {
  // Overlay 8 section 64, 3763:0158..3783:00d7. A repeated span
  // counts positive/ordinary entries; negative chord members belong to
  // their preceding entry. The repeat count includes the existing span.
  // Measured single/double-dot expansion: select the earlier note/chord
  // before the immediately preceding item; pass over intervening non-pitches.
  // Native hypotheses beyond the recorded contexts remain in the F2 ledger.
  public static void RepeatBeforePrevious(List<float> tokens,bool keepAccidentals){
   if(tokens==null)throw new ArgumentNullException("tokens");
   int prior=tokens.Count-1;
   while(prior>=0&&ChordMember(tokens[prior]))prior--;
   int first=prior-1;
   while(first>=0){int kind=(int)Math.Abs(tokens[first])/1000;if(tokens[first]>0&&(kind==1||kind==7))break;first--;}
   if(first<0)throw new NotSupportedException("Dot repetition has no verified earlier note or chord.");
   int stop=first+1;while(stop<tokens.Count&&ChordMember(tokens[stop]))stop++;
   var span=tokens.GetRange(first,stop-first);
   if((long)tokens.Count+span.Count>short.MaxValue)throw new NotSupportedException("Expanded input exceeds the signed 16-bit list representation");
   foreach(float original in span){float value=original;if(!keepAccidentals){float remainder=(float)((double)value%100);value=(float)((value<0?-1000:1000)+(double)remainder);}tokens.Add(value);}
  }
  static bool ChordMember(float token){int kind=Math.Abs((int)token)/1000;return token<0&&(kind==1||kind==7);}
  public static void Repeat(List<float> tokens,int entries,int total,bool keepAccidentals){
   if(tokens==null)throw new ArgumentNullException("tokens");
   if(entries<1||total<1)throw new ArgumentOutOfRangeException("entries");
   int first=tokens.Count,seen=0;
   while(first>0){first--;float value=tokens[first];int kind=(int)(value/1000);if(!(value<=0&&(kind==-1||kind==-7)))seen++;if(seen==entries)break;}
   if(seen!=entries)throw new ArgumentException("Repeat extends before the input list");
   float[] span=tokens.GetRange(first,tokens.Count-first).ToArray();
   long count=(long)tokens.Count+(long)span.Length*(total-1);if(count>short.MaxValue)throw new NotSupportedException("Expanded input exceeds the signed 16-bit list representation");
   for(int repetition=1;repetition<total;repetition++)foreach(float original in span){float value=original;int kind=Math.Abs((int)value)/1000;if(!keepAccidentals&&(kind==1||kind==7)){float remainder=(float)((double)value%100);value=(float)((value<0?-kind:kind)*1000+(double)remainder);}tokens.Add(value);}
  }
 }
}
````

## InputMainNoteReconstruction

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMainNoteReconstruction.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputMainNoteReconstruction {
  static float P(float[] p,int n){return p.Length>=n?p[n-1]:0;}
  static int W(float value){if(float.IsNaN(value)||float.IsInfinity(value)||value<short.MinValue||value>=32768)throw new NotSupportedException("Main flip note conversion outside the original signed-word domain.");return (short)Math.Truncate(value);}
  // Main overlay's note transcription (36:075F-0AB4), then recovered sections55/57.
  // A group is the contiguous Code1 records at one original horizontal position.
  public static List<float[]> Build(IList<float[]> source,int stem,short? retainedGraceWord=null,bool originalInvalidStates=false){
   if(source==null||source.Count==0)throw new ArgumentException("Note group");
   var tokens=new List<float>();if(stem!=0)tokens.Add(5000+stem);
   bool cue=false;for(int i=0;i<source.Count;i++){
    var note=source[i];int pitch=W(P(note,4)),control=W(P(note,5));
    if(P(note,7)<64){bool next=Math.Abs(pitch)>=100;if(next!=cue){tokens.Add(next?8001:8000);cue=next;}}
    // Main36:0782-078D tests strictly greater than 100 (JLE), unlike
    // the generic pitch compiler's >=100 packed-accidental convention.
    int token=1028+pitch%100+(control%10)*100+(control>100?6000:0);
    tokens.Add(i==0?token:-token);
   }
   var pitched=InputPitchRecords.Compile(tokens.ToArray(),P(source[0],2),0,0,200,false);
   if(pitched.Records.Count!=source.Count)throw new NotSupportedException("Main transcription changed the note-group count.");
   if(pitched.Records.Exists(row=>row[0]!=1)){
    if(originalInvalidStates&&pitched.Events==0)return pitched.Records;
    throw new NotSupportedException("Main transcription would turn this outlying pitch into a non-note working record. Original invalid-state compatibility permits the measured compact-record effect.");
   }
   float duration=P(source[0],7);if(duration==65)duration=64;
   if(duration<=0)throw new NotSupportedException("The original refuses notes without rhythmic time.");
   float encoded=(float)((W(P(source[0],9))%100/10)*1000.0+duration);
   var rows=InputChordRhythmRecords.Apply(pitched.Records,new[]{encoded},0,200,1,pitched.ForcedStems.ToArray(),false,1,null,retainedGraceWord);
   return rows;
  }
 }
}
````

## InputMarksCompiler

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMarksCompiler.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputMarksResult {public List<float[]> Records;public int FirstOrnament=-1;}
 public static class InputMarksCompiler {
  static float F(double n){return (float)n;}static int W(float n){if(float.IsNaN(n)||float.IsInfinity(n)||n<int.MinValue||n>int.MaxValue)throw new ArgumentOutOfRangeException("n");return (int)Math.Truncate(n);}
  static float P(float[] p,int n){return p[n-1];}
  static float Offset(int kind,int sign,float[] note){int y=W(note[3])%100;float result=0;if(note[4]%100<20){if(kind<50){if(y<4)return kind==5?0.05000000074505806f:0;result=F(y/10.0-(kind==5?0.3499999940395355:0.4000000059604645));}else if(y<=2&&sign>=0)return y-3;}else{if(kind<50){if(y>10)return kind==5?0.05000000074505806f:0;result=F((kind==5?1.0499999523162842:1)-y/10.0);}else if(y>=12&&sign<0)return y-11;}return result>0.8999999761581421f?0.8999999761581421f:result;}
  static int Mapped(int[] map,int index,int count){index=Math.Max(1,index);if(index>=map.Length||map[index]<1||map[index]>count)throw new ArgumentOutOfRangeException("index","Mark lies outside the input note map");return map[index]-1;}
  static float X(IList<float[]> rows,int[] map,int index,float fraction,float beginning,float end){int at=Mapped(map,index,rows.Count);float x=rows[at][2];if(BitConverter.ToInt32(BitConverter.GetBytes(fraction),0)!=0){float target;if(index==0){target=x;x=beginning;}else target=index<map.Length-1?rows[Mapped(map,index+1,rows.Count)][2]:end;x=F(((double)target-x)*fraction+x);}return x;}
  public static InputMarksResult Apply(IList<float[]> original,int[] map,float[] tokens,int[] opposite,float staff,float beginning,float end,float followingStaffScale,float dynamics){
   return Apply(original,map,tokens,opposite,staff,beginning,end,followingStaffScale,dynamics,false);
  }
  public static InputMarksResult Apply(IList<float[]> original,int[] map,float[] tokens,int[] opposite,float staff,float beginning,float end,float followingStaffScale,float dynamics,bool reproduceRetainedDescriptor){
   var rows=new List<float[]>();foreach(var p in original)rows.Add((float[])p.Clone());var output=new InputMarksResult{Records=rows};int kind=0,sign=0;bool literal=false;
   float[] retained=null;
   for(int cursor=0;cursor<tokens.Length;cursor++){
    float value=tokens[cursor];int integer=W(value),absolute=Math.Abs(integer);if(absolute>9998){kind=absolute/10000;if(kind==0)throw new ArgumentException("Zero mark descriptor");literal=integer<0;sign=opposite!=null&&opposite[cursor]!=0?-kind:kind;retained=null;continue;}
    if(kind==0){
     if(!reproduceRetainedDescriptor)throw new NotSupportedException("A reversed mark motive starts with a target. Enter a mark before it or enable original invalid states.");
     // 3262:0017..0030 leaves the descriptor and record template untouched.
     // In an ordinary single-note phrase these are the final rhythm note.
     if(original.Count!=map.Length||original.Count<2||original[0][0]!=3)
      throw new NotSupportedException("The original retained mark descriptor is unverified for this phrase.");
     for(int i=1;i<original.Count;i++){
      var note=original[i];
      if(note[0]!=1||!(note[2]>original[i-1][2])||!(note[3]>=1&&note[3]<=13)||
         !(note[6]>0&&note[6]<64)||note[11]!=0||note[14]!=0)
       throw new NotSupportedException("The original retained mark descriptor is unverified for this phrase.");
     }
     retained=original[original.Count-1];
     kind=W(retained[3]);literal=true;
    }
    bool range=absolute>=1000;int first=range?absolute-1000:absolute,last=first;if(range&&kind>=95&&kind<=99){while(Math.Abs(W(value))>=1000)value=F((double)value-1000);first=Math.Abs(W(value));range=false;}else if(range){if(++cursor>=tokens.Length)throw new ArgumentException("Incomplete mark range");last=W(tokens[cursor]);}float fraction=range?0:F((double)value-first);
    if(kind>=95&&!literal){if(++cursor>=tokens.Length)throw new ArgumentException("Incomplete span mark");int finish=Math.Abs(W(tokens[cursor]));int beginIndex=Mapped(map,first,rows.Count);float startX=X(rows,map,first,fraction,beginning,end),finishX=X(rows,map,finish,F((double)tokens[cursor]-finish),beginning,end);var p=new float[15];p[1]=staff;p[2]=startX;bool below=sign>0;
     bool ottava=kind<97||kind>119;float y=ottava?(kind==95?-1.5f:12.5f):(below?0:14),extreme=below?99:-99,inset=ottava?1.5f:0;
     if(ottava){if(float.IsNaN(followingStaffScale)||float.IsInfinity(followingStaffScale))throw new NotSupportedException("Following staff scale for this octave span is not verified");below=kind==95;p[0]=7;p[4]=1;p[5]=F((double)finishX+followingStaffScale/6.0);if(kind<100){p[6]=kind==95?-8:8;p[11]=kind==95?1:-1;}else p[7]=kind%10;}else{p[0]=4;p[4]=999;p[5]=finishX;if(kind==98)p[6]=-1;}
     extreme=below?99:-99;for(int n=Math.Max(1,first);n<=finish;n++){var note=rows[Mapped(map,n,rows.Count)];float pitch=F((double)note[3]%100),stem=F((double)note[4]%100);if(below){if(stem>=20)pitch=F((double)pitch-7);if(pitch<extreme)extreme=pitch;}else{if(stem<20)pitch=F((double)pitch+7);if(pitch>extreme)extreme=pitch;}}
     if(below&&extreme<4)y=F((double)F((double)extreme-inset)-3.5);else if(!below&&extreme>10)y=F((double)F((double)extreme-inset)+3.5);p[3]=y;rows.Add(p);continue;
    }
    for(int n=Math.Max(1,first);;n++){
     int at=Mapped(map,n,rows.Count);var note=rows[at];if(kind<=36&&!literal){float correction=sign>=0&&Array.IndexOf(new[]{4,5,8,9,10,19},kind)>=0?Offset(kind,sign,note):0;note[10]=F((double)sign+correction);}
     else if(kind<71||literal){int effective=kind==66||kind==67?-sign:sign;float correction=Offset(kind,effective,note);var p=new float[15];p[0]=9;p[1]=staff;p[2]=X(rows,map,n==Math.Max(1,first)?first:n,n==Math.Max(1,first)?fraction:0,beginning,end);p[3]=F((double)correction+(effective<0?14:-2));p[4]=kind;p[5]=1;if(!literal)p[14]=dynamics;
      if(retained!=null)Array.Copy(retained,6,p,6,3);
      rows.Add(p);}
     else{if(output.FirstOrnament<0)output.FirstOrnament=rows.Count;var p=new float[15];p[0]=6;p[1]=staff;p[2]=X(rows,map,n==Math.Max(1,first)?first:n,n==Math.Max(1,first)?fraction:0,beginning,end);p[6]=F(-((double)note[4]%100));p[9]=kind-80;if(note[5]>=2&&note[5]<=3)p[13]=p[6]>=-10?-1.2300000190734863f:1.600000023841858f;float shift=F((kind>91?1.0:2.0)-note[7]);if(W(note[8])%10!=0){if(p[6]>-20)shift=F(1.0+shift);note[7]=F((double)note[7]+.5);}if(p[6]>-20)shift=-shift;p[3]=F((double)(note[3]%100)+shift);rows.Add(p);}
     if(n>=last)break;
    }
   }
   return output;
  }
 }
}
````

## InputMarkText

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMarkText.cs)

````csharp
using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputMarkTokens {public float[] Values;public int[] Opposite;}
 public static class InputMarkText {
  // Canonical mark spellings from overlay8 section69. The numeric consumer
  // receives descriptors, target words and range controls independently.
  static readonly Dictionary<string,int> names=new Dictionary<string,int>{
   {"A",5},{"AR",15},{"AS",9},{"AO",67},{"AC",68},{"ACT",25},{"C",97},{"CR",63},{"C-",98},
   {"D",11},{"DI",64},{"F",56},{"FP",60},{"FE",14},{"FF",57},{"FFF",58},
   {"FIF",1},{"FIS",2},{"FIN",3},{"MO",17},{"IM",18},{"HW",19},
   {"H",13},{"M",17},{"MF",55},{"MP",54},{"O",96},{"OB",95},
   {"P",53},{"PP",52},{"PPP",51},{"PL",24},{"PZ",66},{"RI",65},
   {"S",7},{"S2",26},{"S3",27},{"S4",28},{"SF",61},{"SFZ",62},
   {"T",6},{"TS",10},{"TH",16},{"TR",20},{"TRF",21},{"TRS",22},{"TRN",23},
   {"TM0",93},{"TM4",94},{"TM5",93},{"TM6",93},{"TM7",93},{"TM8",93},{"TM9",93},{"TL",93},{"TME",91},{"TMS",92},{"TMT",93},{"TM1",91},{"TM2",92},{"TM3",93},{"TRW",120},{"TRFW",121},{"TRSW",122},{"TRNW",123},{"U",12},{"W",4},{"WS",8}};
  public static InputMarkTokens Parse(string text,InputMacroContext context){return Parse(text,context,false);}
  public static InputMarkTokens Parse(string text,InputMacroContext context,bool rejectMultiDigitF){if(context==null)throw new ArgumentNullException("context");var values=new List<float>();var flags=new List<int>();foreach(string field in InputTextLists.ExpansionFields(text)){string part=field.Trim();if(part.Length==0)continue;Match definition=Regex.Match(part,@"^([A-Z])\[(.*)\]$");if(definition.Success){char name=definition.Groups[1].Value[0];var body=Parse(definition.Groups[2].Value+";",context,rejectMultiDigitF);// Original motive memory retains the float words, not the initial opposed-side flags.
context.Define(name,body.Values);values.AddRange(body.Values);flags.AddRange(body.Opposite);continue;}Match recall=Regex.Match(part,@"^@(-?)([A-Z])([+\-0-9. ]*)$");if(recall.Success){
 string suffix=recall.Groups[3].Value.Trim();
 if(suffix.Length>0){
  int at=0;
  InputRealReader.Read(suffix,ref at);
  if(at!=suffix.Length)throw new InvalidDataException("Invalid mark motive suffix: "+suffix);
 }
 char name=recall.Groups[2].Value[0];
 float[] body;
 if(!context.Definitions.TryGetValue(name,out body))throw new InvalidDataException("Undefined input motive "+name);
 if(recall.Groups[1].Length==0)values.AddRange(body);
 else {
  // Overlay 8 tests the signed word obtained by dividing the current word
  // by 1000. Its grouped route includes earlier negative words up to the
  // preceding nonnegative word. Other words are emitted singly.
  for(int index=body.Length-1;index>=0;index--){
   int kind=unchecked((short)(int)Math.Truncate((double)body[index]/1000));
   if(kind==0||kind==-1||kind==-7){
    int first=index-1;
    while(first>=0&&body[first]<0)first--;
    if(first<0)throw new InvalidDataException("Reverse mark motive starts before its definition.");
    for(int i=first;i<=index;i++)values.Add(body[i]);
    index=first;
   }else values.Add(body[index]);
  }
 }
 flags.AddRange(new int[body.Length]);
 continue;}var parsed=Parse(part+";",rejectMultiDigitF);values.AddRange(parsed.Values);flags.AddRange(parsed.Opposite);}return new InputMarkTokens{Values=values.ToArray(),Opposite=flags.ToArray()};}
  public static InputMarkTokens Parse(string text){return Parse(text,false);}
  public static InputMarkTokens Parse(string text,bool rejectMultiDigitF){
   if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(!text.EndsWith(";"))throw new InvalidDataException("Terminate marks with a semicolon");text=text.Substring(0,text.Length-1);var values=new List<float>();var flags=new List<int>();if(text.Trim().Length==0)return new InputMarkTokens{Values=values.ToArray(),Opposite=flags.ToArray()};
   foreach(string field in text.Split('/')){
    Match m=Regex.Match(field.Trim(),@"^([+-]?)([A-Z][A-Z0-9-]*)(?:\s+(.+))?$");if(!m.Success)throw new InvalidDataException("Separate the mark and its target with a space");int kind;string name=m.Groups[2].Value;
    Match literal=Regex.Match(name,@"^N([0-9]+)$"),number=Regex.Match(name,@"^F([0-9]+)$");
    string targets=m.Groups[3].Value;
    if(name=="S1"&&targets.Length==0)
     throw new InvalidDataException("Separate the mark and its target with a space");
    if(name=="N"){Match n=Regex.Match(targets,@"^([0-9]+)\s+(.+)$");if(!n.Success)throw new InvalidDataException("N requires a library glyph number and target");kind=-Integer(n.Groups[1].Value);targets=n.Groups[2].Value;}
    else if(literal.Success)kind=-Integer(literal.Groups[1].Value);
    else if(number.Success){
     string digits=number.Groups[1].Value;
     if(rejectMultiDigitF&&digits.Length>1)throw new InvalidDataException("Separate a single F digit and its target with a space.");
     // The original F branch consumes one digit. Remaining digits are
     // read by its target reader before the explicitly separated targets.
     kind=digits[0]-'0'+30;
     if(digits.Length>1)targets=digits.Substring(1)+" "+targets;
    }
    else if(!names.TryGetValue(name,out kind))throw new NotSupportedException("Mark spelling requires its original parser branch: "+name);
    if(targets.Trim().Length==0)throw new InvalidDataException("Separate the mark and its target with a space");int opposite=m.Groups[1].Length==0?0:(kind<0?-1:1);values.Add((float)((long)kind*10000));flags.Add(opposite);
    foreach(Match target in Regex.Matches(targets,@"\S+")){
     string[] range=target.Value.Split(new[]{':','\''});if(range.Length>2)throw new InvalidDataException("Invalid mark range");float value=Number(range[0]);if(range.Length==2)value=(float)((double)value+1000);values.Add(value);flags.Add(0);if(range.Length==2){values.Add(Number(range[1]));flags.Add(0);}
    }
   }
   return new InputMarkTokens{Values=values.ToArray(),Opposite=flags.ToArray()};
  }
  static int Integer(string text){int n;if(!int.TryParse(text,NumberStyles.None,CultureInfo.InvariantCulture,out n)||n>short.MaxValue)throw new InvalidDataException("Mark number exceeds the signed word representation");return n;}
  static float Number(string text){float n;if(!float.TryParse(text,NumberStyles.AllowLeadingSign|NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out n)||float.IsNaN(n)||float.IsInfinity(n))throw new InvalidDataException("Invalid mark target: "+text);return n;}
 }
}
````

## InputMeterDuration

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMeterDuration.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Logical quarter-note duration of an explicit Code 18 signature. Graphic
 // single-number signatures do not specify a beat unit and remain ambiguous.
 public static class InputMeterDuration {
  public static bool TryQuarterBeats(float numerator,float denominator,float secondNumerator,float secondDenominator,out float beats){
   beats=0;if(!Finite(numerator)||!Finite(denominator)||!Finite(secondNumerator)||!Finite(secondDenominator))return false;
   if(numerator==98||numerator==99){beats=4;return secondNumerator==0;}
   if(numerator<=0||denominator<=0||secondNumerator<0)return false;
   beats=4*numerator/denominator;
   if(secondNumerator>0){float unit=secondDenominator>0?secondDenominator:denominator;beats+=4*secondNumerator/unit;}
   return Finite(beats)&&beats>0;
  }
  static bool Finite(float value){return !Single.IsNaN(value)&&!Single.IsInfinity(value);}
 }
}
````

## InputPitchRecords

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputPitchRecords.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputPitchResult {public readonly List<float[]> Records=new List<float[]>();public readonly List<int> NoteIndices=new List<int>();public readonly List<int> ForcedStems=new List<int>();public float Left;public int Events,FinalClef;}
 public static class InputPitchRecords {
  static bool IsSecondary(float value){int kind=(int)(Math.Abs(value)/1000);return value<0&&(kind==1||kind==7);}
  static int Pitch(int token,int clef){int pitch=Math.Abs(token)%1000,octave=(pitch%100-1)/7,degree=(pitch%100-1)%7+1,offset=clef>1?clef*2+2:clef*12;return (octave-4)*7+offset+degree;}
  static void SortChord(float[] tokens,int start,int clef,int stem){int end=start+1;while(end<tokens.Length&&IsSecondary(tokens[end]))end++;int n=end-start,low=99,high=-99;int[] pitch=new int[n];var group=new float[n];for(int i=0;i<n;i++){group[i]=Math.Abs(tokens[start+i]);pitch[i]=Pitch((int)group[i],clef);low=Math.Min(low,pitch[i]);high=Math.Max(high,pitch[i]);}int balance;if(stem!=0)balance=11-stem;else if(n==2||high-7!=7-low)balance=14-high-low;else{balance=0;for(int i=1;i<n-1;i++){if(pitch[i]<8)balance++;if(pitch[i]>7)balance--;}}int sentinel=balance>0?99:-99;for(int i=0;i<n;i++){int best=sentinel,chosen=-1;for(int j=0;j<n;j++)if(sentinel>0?pitch[j]<best:pitch[j]>best){best=pitch[j];chosen=j;}if(chosen<0)throw new NotSupportedException("Chord sort sentinel reached");tokens[start+i]=i==0?group[chosen]:-group[chosen];pitch[chosen]=sentinel;}}
  // Section55 pitch record projection. CompileSingleNotes retains its original
  // restricted contract; Compile also performs recovered chord sorting/layout.
  public static InputPitchResult CompileSingleNotes(float[] tokens,float staff,int clef,float left,float right,bool insetLeft){return Core(tokens,staff,clef,left,right,insetLeft,false);}
  public static InputPitchResult Compile(float[] tokens,float staff,int clef,float left,float right,bool insetLeft){return Core(tokens,staff,clef,left,right,insetLeft,true);}
  static InputPitchResult Core(float[] input,float staff,int clef,float left,float right,bool insetLeft,bool chords){
   float[] tokens=input==null?null:(float[])input.Clone();
   if(tokens==null||tokens.Length==0)throw new ArgumentException("Pitch tokens required.");
   var output=new InputPitchResult();int initialClef=clef,stem=0,savedStem=0,cue=0;float head=0;
   if(insetLeft&&(int)(tokens[0]/1000)!=4&&BitConverter.ToInt32(BitConverter.GetBytes(left),0)==0)left=1.5f;
   output.Left=left;int divisions=tokens.Length+1;if(chords)foreach(float v in tokens)if(v<0&&((int)(-v/1000)==1||(int)(-v/1000)==7))divisions--;float step=(float)(((double)right-left)/divisions);int position=0,leader=-1;
   for(int tokenIndex=0;tokenIndex<tokens.Length;tokenIndex++){float value=tokens[tokenIndex];if(float.IsNaN(value)||float.IsInfinity(value)||value<short.MinValue||value>=32768)throw new ArgumentOutOfRangeException("tokens");int token=(int)Math.Truncate(value),absolute=Math.Abs(token),kind=absolute/1000;if(!chords&&(kind==1||kind==7)&&token<0)throw new NotSupportedException("Signed chord tokens require the chord compiler.");if(kind==7)kind=1;
    if(chords&&kind==1&&token>0&&tokenIndex+1<tokens.Length&&IsSecondary(tokens[tokenIndex+1])){SortChord(tokens,tokenIndex,clef,stem);value=tokens[tokenIndex];token=(int)value;absolute=Math.Abs(token);}
    bool secondary=kind==1&&token<0;
    float[] p=new float[15];p[1]=staff;
    if(kind==5){stem=savedStem=token-5000;continue;}
    if(kind==6){head=token-6000;continue;}
    if(kind==8){cue=token-8000;if(cue==0)stem=savedStem;else if(stem==0)stem=10;continue;}
    if(kind==3){clef=absolute-3000;if(token<0)continue;p[4]=clef;if(clef==4)clef=0;if(output.Records.Count!=0&&(initialClef<0||output.Records[output.Records.Count-1][0]!=14))p[3]=100;initialClef=-1;}
    else if(kind==4){p[3]=token%100;p[4]=(token-4000)/100;kind=14;if(p[4]==9){p[3]=-p[3];p[4]=0;}}
    else if(kind==17){p[4]=absolute-17000;if(p[4]==0)continue;if(token<0)p[4]=-p[4];p[5]=clef;}
    else if(kind==1){int pitch=absolute%1000;p[3]=Pitch(absolute,clef);p[11]=head;
     if(secondary){if(leader<0)throw new ArgumentException("Chord member without leader");p[14]=-1;p[4]=pitch/100+(absolute>7000?100:0);var first=output.Records[leader];int count=output.Records.Count-leader,rootPitch=(int)first[3]%100,memberPitch=(int)p[3];if(memberPitch<rootPitch)count=-count;if(memberPitch==rootPitch)count=first[4]%100>=20?-1:1;first[6]=count;first[7]=Math.Max(first[7],(float)Math.Abs(rootPitch-memberPitch));}
     else{float direction=stem==0?(p[3]>=7?20:10):stem;p[4]=pitch/100+direction+(absolute>7000?100:0);if(direction>=20){if(p[3]>14)p[7]=p[3]-14;}else if(p[3]<0)p[7]=-p[3];output.Events++;leader=output.Records.Count;}}
    else if(kind==2){int rest=absolute-2000;output.Events++;if(token<0){p[7]=rest;p[4]=-2;p[14]=-1;}else switch(rest){case 1:p[5]=-1;break;case 2:p[4]=-2;p[14]=-1;break;case 3:p[4]=-4;p[14]=-1;break;case 4:p[3]=-6;break;case 5:p[3]=6;break;case 6:p[7]=-1;break;}}
    else{kind=18;p[4]=(token-20000)/100;p[5]=token%100;}
    if(kind<3&&cue!=0)p[3]=(float)((double)p[3]+(p[3]<0?-100:100));
    if(!secondary)position++;p[2]=(float)((double)position*step+left);p[0]=kind;output.Records.Add(p);if(kind==1&&!secondary){output.NoteIndices.Add(output.Records.Count);output.ForcedStems.Add(stem);}
   }
   if(chords)foreach(int index in output.NoteIndices){var first=output.Records[index-1];float span=first[6];if(span==0)continue;int direction=(int)first[4]%100;if(span<0&&direction<20)first[4]=(float)((double)first[4]+10);else if(span>0&&direction>=20)first[4]=(float)((double)first[4]-10);first[6]=0;InputChordLayout.Apply(output.Records,index-1,(int)Math.Truncate(index-1+Math.Abs(span)));}
   output.FinalClef=clef;return output;
  }
 }
}
````

## InputPitchToken

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputPitchToken.cs)

````csharp
using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputPitchState {public int Octave=4,Previous=29,PreviousDegree=1,KeySignature,MeterTop=4,MeterBottom=4,BarSpan=1;public bool Proximity;}
 public static class InputPitchToken {
  // Candidate transcription of section68's pitch-token leaf. List expansion,
  // chord delimiters, macro calls and saved-input replay are separate routines.
  public static float? Parse(string text,InputPitchState state){
   if(state==null)throw new ArgumentNullException("state");if(text==null)throw new ArgumentNullException("text");string t=text.Trim().ToUpperInvariant();if(t.Length==0)throw new InvalidDataException("Missing pitch token.");
   if(t=="P")throw new InvalidDataException("Attach P to the following pitch, for example PD.");
   while(t.Length>0&&(t[0]=='P'||t[0]=='O')){state.Proximity=t[0]=='P';t=t.Substring(1);}
   if(t.Length==0)return null;
   switch(t){case "TR":return 3000;case "BA":return 3001;case "AL":return 3002;case "TE":return 3003;case "-TR":return -3000;case "-BA":return -3001;case "-AL":return -3002;case "-TE":return -3003;case "SU":return 5010;case "SD":return 5020;case "SO":return 5000;case "S+":return 6001;case "S-":return 6002;case "S0":return 6000;case "Q+":return 8001;case "Q-":case "Q0":return 8000;case "R":return 2000;case "RI":return 2001;case "RW":return 2002;case "RP":return 2003;case "RD":return 2004;case "RU":return 2005;case "RF":return 2006;case "COM":state.MeterTop=99;state.MeterBottom=1;return 29901;case "CUT":state.MeterTop=98;state.MeterBottom=1;return 29801;case "T":return 20000+state.MeterTop*100+state.MeterBottom;case "K":return Key(state.KeySignature);}
   Match m=Regex.Match(t,@"^K([0-9]+)([SF])(N?)$");if(m.Success){int n=Int(m.Groups[1].Value);if(m.Groups[3].Length!=0)n+=100;return Key(m.Groups[2].Value=="F"?-n:n);}
   m=Regex.Match(t,@"^([0-9]+)\s+([0-9]+)$");if(m.Success){int top=Int(m.Groups[1].Value),bottom=Int(m.Groups[2].Value);if(bottom==0){bottom=top;top=0;}state.MeterTop=top;state.MeterBottom=bottom;return 20000+top*100+bottom;}
   if(Regex.IsMatch(t,@"^[0-9]+$")){
    int bottom=Int(t);
    state.MeterTop=0;state.MeterBottom=bottom;
    return 20000+bottom;
   }
   m=Regex.Match(t,@"^R([0-9]+)$");if(m.Success)return -2000-Int(m.Groups[1].Value);
   m=Regex.Match(t,@"^M(DH|D|H|R|L|RL|RR|RD|S|DS|I)?\s*([0-9]*)$");if(m.Success){int type=0;switch(m.Groups[1].Value){case "D":type=100;break;case "DH":case "H":type=200;break;case "L":case "RL":type=300;break;case "R":case "RR":type=400;break;case "RD":type=500;break;case "S":case "DS":type=700;break;case "I":type=900;break;}if(m.Groups[2].Length!=0)state.BarSpan=Int(m.Groups[2].Value);return 4000+type+state.BarSpan;}
   m=Regex.Match(t,@"^([A-G])(\(?)((?:FF|SS|F|S|N)?)([0-9]?)([UJ]?)$");if(!m.Success)throw new InvalidDataException("Unsupported pitch token: "+text);
   int degree="CDEFGAB".IndexOf(m.Groups[1].Value[0])+1,accidental=0;switch(m.Groups[3].Value){case "F":accidental=100;break;case "S":accidental=200;break;case "N":accidental=300;break;case "FF":accidental=400;break;case "SS":accidental=500;break;}
   bool octave=m.Groups[4].Length!=0;if(octave)state.Octave=m.Groups[4].Value[0]-'0';
   bool jump=m.Groups[5].Length!=0;if(jump){if(m.Groups[5].Value=="U"&&degree<=state.PreviousDegree)state.Octave++;if(m.Groups[5].Value=="J"&&state.PreviousDegree<=degree)state.Octave--;}
   int absolute=state.Octave*7+degree;if(state.Proximity&&!octave&&!jump){int delta=state.Previous-absolute;if(delta>3){state.Octave++;absolute+=7;}if(delta< -3){state.Octave--;absolute-=7;}}
   state.Previous=absolute;state.PreviousDegree=degree;return 1000+absolute+accidental+(m.Groups[2].Length==0?0:6000);
  }
  static int Int(string t){int value;if(!int.TryParse(t,NumberStyles.None,CultureInfo.InvariantCulture,out value)||value>999)throw new InvalidDataException("Input control exceeds this token projection's measured domain.");return value;}
  static int Key(int value){return value==0?17000:(value<0?-17000+value:17000+value);}
 }
}
````

## InputRealReader

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputRealReader.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputRealReader {
  // Resident16D3:05EF..0799. The caller supplies a cursor; the first
  // nondigit terminates the value (there is no exponent grammar). Sign,
  // signed32 mantissa and decimal power have separate stored words.
  public static float Read(string text,ref int at){
   int beginning=at,p=at,decimalAt=-1,digits=0;int mantissa=0;float sign=1;
   if(p<text.Length&&text[p]=='+')p++;
   while(p<text.Length){
    char c=text[p];
    if(c=='-'){sign=-sign;p++;while(p<text.Length&&text[p]==' ')p++;}
    else if(c=='.'){decimalAt=p;p++;}
    else if(c>='0'&&c<='9'){mantissa=unchecked(mantissa*10+c-'0');digits++;p++;}
    else break;
    if(p-beginning>=12)return 0;
   }
   if(digits==0||mantissa<0)return 0;
   float storedMantissa=mantissa,storedPower=decimalAt<0?1:(float)Math.Pow(10,p-decimalAt-1);
   float value=(float)((double)sign*storedMantissa/storedPower);
   if(value==0&&sign<0)value=-1;
   at=p;return value;
  }
 }
}
````

## InputRecordCompaction

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputRecordCompaction.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputRecordCompaction {
  // Section65 099E..0B9D: working-record parameter selection. Codes below6
  // expose at most12 words; other codes expose15. Numeric zero trimming also
  // trims negative zero. File trailers and document append state are separate.
  public static byte[] Compact(float[] parameters){if(parameters==null||parameters.Length!=15)throw new ArgumentException("Fifteen working words required.");int count=parameters[0]>=6?15:12;while(count>0&&parameters[count-1]==0)count--;byte[] result=new byte[(count+1)*4];Buffer.BlockCopy(BitConverter.GetBytes((float)count),0,result,0,4);Buffer.BlockCopy(parameters,0,result,4,count*4);return result;}
 }
}
````

## InputReferenceSpacing

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputReferenceSpacing.cs)

````csharp
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ScoreModern.RecoveredFragments {
 // Reference spacing from overlay 7 index 57. The DOS reference table
 // stores the horizontal anchors and subtracts .001 from its upper search
 // boundaries. Grace and barline entries can supply a right drawing
 // anchor without consuming beats.
 public sealed class InputReferenceSpacing {
  readonly float clefX,end,total;
  readonly float[] anchors,queryGrid,upperAnchors;
  struct Marker {
   public readonly int Code,Slot;
   public readonly float X;
   public Marker(int code,int slot,float x){Code=code;Slot=slot;X=x;}
  }
  readonly List<Marker> markers=new List<Marker>();
  public bool HasClefAtZero {get{return OpeningClefX==0;}}
  internal float? OpeningClefX {get;private set;}
  internal float Start {get{return anchors[0];}}
  internal float QueryEnd {get{return end;}}

  public InputReferenceSpacing(IList<float[]> staff):this(staff,null){}
  public InputReferenceSpacing(IList<float[]> staff,Action<int,float> normalizedPosition){
   var notation=CollectReferenceRows(staff,normalizedPosition).ToArray();
   // Source SP collector skips opening bar drawings before its clef/timed table.
   // Interior brackets still supply right drawing anchors (measured offset probe).
   int start=0;while(start<notation.Length&&Bar(notation[start]))start++;
   bool hasClef=notation.Length>start&&OpeningClef(notation[start]);
   int clef=hasClef?start:start-1;
   if(notation.Length<=clef+1)throw new NotSupportedException("Reference spacing needs timed events.");
   // An absent source clef supplies zero; source appearance does not change timing.
   var events=SpacingEvents(notation,clef+1,false);
   var notes=events.Where(row=>!Grace(row)&&!Structural(row)).ToList();
   if(notes.Count==0)
    throw new NotSupportedException("Reference spacing needs at least one timed event.");
   clefX=hasClef?notation[clef][2]:0;
   if(hasClef)OpeningClefX=clefX;
   // Without an explicit closing bar, overlay 7 appends its literal 200 marker.
   bool hasClosingBar=Bar(events[events.Count-1]);
   end=hasClosingBar?events[events.Count-1][2]:200;
   anchors=new float[notes.Count+1];
   for(int i=0;i<notes.Count;i++){
    total=(float)((double)total+notes[i][6]);
    anchors[i]=notes[i][2];
   }
   anchors[notes.Count]=end;
   upperAnchors=anchors.Skip(1).ToArray();
   int spanIndex=-1;
   bool hasIntermediateAnchor=false;
   for(int i=0;i<events.Count;i++){
    var row=events[i];
    if(Structural(row))markers.Add(new Marker((int)row[0],i+2+clef,row[2]));
    if(!Grace(row)&&!Structural(row)){spanIndex++;hasIntermediateAnchor=false;}
    else if(row[0]!=3&&spanIndex>=0&&!hasIntermediateAnchor){upperAnchors[spanIndex]=row[2];hasIntermediateAnchor=true;}
   }
   if(!hasClosingBar){
    markers.Add(new Marker(14,events.Count+2+clef,end));
   }
   // Overlay 7 index 56 maps reference durations onto a separate query grid.
   // Index 57 interpolates from that grid to the stored horizontal positions.
   queryGrid=new float[anchors.Length];
   queryGrid[0]=anchors[0];
   queryGrid[notes.Count]=end;
   float span=(float)((double)end-anchors[0]);
   float unitsPerBeat=(float)((double)span/total);
   for(int i=1;i<notes.Count;i++)
    queryGrid[i]=(float)((double)notes[i-1][6]*unitsPerBeat+queryGrid[i-1]);
   for(int i=1;i<anchors.Length;i++){
    bool flatTerminal=i==anchors.Length-1&&anchors[i]==anchors[i-1]&&upperAnchors[i-1]==anchors[i];
    if((!flatTerminal&&(!(anchors[i]>anchors[i-1])||
        !(upperAnchors[i-1]>anchors[i-1]&&upperAnchors[i-1]<=anchors[i])))||
       !(queryGrid[i]-queryGrid[i-1]>.001f))
     throw new NotSupportedException("Reference spacing needs increasing horizontal anchors and distinct query positions.");
   }
  }

  // Reference setup consumes the common rhythmic collector's positions and
  // reduced durations. Coincident notes can leave duration in the scratch
  // group for later onsets, even when both notes have independent stems.
  static List<float[]> CollectReferenceRows(IList<float[]> staff,Action<int,float> normalizedPosition){
   var result=new List<float[]>();if(staff.Count==0)return result;
   var words=new List<float>();var starts=new int[staff.Count];
   for(int i=0;i<staff.Count;i++){
    starts[i]=words.Count+1;words.Add(staff[i].Length);words.AddRange(staff[i]);
   }
   var rhythm=new float[staff.Count+4];var positions=new float[rhythm.Length];var scratch=new float[rhythm.Length];
   float number=staff[0][1];var bank=words.ToArray();var collected=LayoutRhythmicCollection.Collect(bank,starts,number,0,1,rhythm,positions,scratch);
   // These are private timing-table rows, never emitted as notation items.
   for(int i=0;i<collected.Count;i++){
    float duration=rhythm[i],x=positions[i];
    if(duration>0)result.Add(new[]{1f,number,x,0,10,0,duration});
    else if(duration==-64)result.Add(new[]{1f,number,x,100,10,0,65,100});
    else if(duration==-14||duration==-140)result.Add(new[]{14f,number,x,1});
    else if(duration==-3||duration==-17||duration==-18)result.Add(new[]{-duration,number,x});
    else throw new NotSupportedException("Reference timing marker is outside the verified projection.");
   }
   if(normalizedPosition!=null)for(int i=0;i<staff.Count;i++)
    if(staff[i].Length>=3&&bank[starts[i]+2]!=staff[i][2])normalizedPosition(i,bank[starts[i]+2]);
   return result;
  }

  static float P(float[] row,int parameter){return row.Length>=parameter?row[parameter-1]:0;}
  static bool OpeningClef(float[] row){
   float kind=P(row,5);
   return row[0]==3&&kind>=0&&kind<=4&&kind==Math.Truncate(kind);
  }
  static bool Bar(float[] row){
   if(row[0]!=14)return false;
   float style=P(row,5);
   if(P(row,4)<0)return style>=-1&&style<=7&&style==Math.Truncate(style);
   float span=P(row,4);
   return span>=1&&span<=32&&span==Math.Truncate(span)&&style>=-1&&style<=11&&style==Math.Truncate(style);
  }
  static bool Structural(float[] row){
   return Bar(row)||row[0]==3||row[0]==17||row[0]==18;
  }
  static bool Grace(float[] row){return row[0]==1&&P(row,7)==65;}
  static bool GraceMember(float[] row){return row[0]==1&&P(row,7)==64&&P(row,5)%100<10;}
  static bool SupportedEvent(float[] row,bool working){
   if(Structural(row))return true;
   float duration=P(row,7);
   if(Grace(row))return Math.Abs(P(row,4))>=100&&Math.Abs(P(row,4))<200&&P(row,8)>=100&&P(row,15)==0;
   if(GraceMember(row))return false;
   if(row.Length<7||!(duration<65&&(duration>0||working&&duration==0)))return false;
   if(row[0]==1)return true;
   // Reference timing consumes rest onsets/durations, independently of drawing controls.
   return row[0]==2;
  }
  static List<float[]> SpacingEvents(IList<float[]> records,int first,bool working){
   var events=new List<float[]>();
   float[] leader=null;
   for(int i=first;i<records.Count;i++){
    var row=records[i];
    // Working notes carry a member marker. Compact SCORE items retain the
    // stemless P5 and share the preceding chord leader's onset and duration.
    if(leader!=null&&leader[0]==1&&row[0]==1&&row.Length>=7&&
       row[2].Equals(leader[2])&&(row[6]==leader[6]||Grace(leader)&&GraceMember(row))&&Math.Abs(row[3])<200&&
       leader[4]%100>=10&&leader[4]%100<30&&row[4]>=0&&row[4]%100<10&&P(row,15)==(working?-1:0))
     continue;
    if(!SupportedEvent(row,working))throw new NotSupportedException("This item falls outside the verified reference spacing route.");
    leader=row;
    events.Add(row);
   }
   return events;
  }
  internal string Apply(List<float[]> records,float scale){
   int leadingBar=records.Count>0&&Bar(records[0])?1:0;
   bool hasClef=records.Count>leadingBar&&OpeningClef(records[leadingBar]);
   int first=leadingBar+(hasClef?1:0);
   if(records.Count<=first)throw new NotSupportedException("Reference spacing needs a linked timed event.");
   float actual=0;
   foreach(var row in SpacingEvents(records,first,true))
    if(!Grace(row)&&!Structural(row))actual=row[6]==0?float.NaN:(float)((double)actual+row[6]);
   if(leadingBar==1)records[0][2]=0;
   if(hasClef)records[leadingBar][2]=clefX;
   int interval=0,markerIndex=0;
   float left=queryGrid[0],upper=(float)((double)queryGrid[1]-.0010000000474974513);
   for(int i=first;i<records.Count;i++){
    if(Grace(records[i])||GraceMember(records[i])){records[i][2]=-1;continue;}
    if(Structural(records[i])){
     var row=records[i];
     int previous=i-1;
     while(previous>0&&(records[previous][2]<0||float.IsNaN(records[previous][2])))previous--;
     float prior=records[previous][2],position=prior;
     int match=markerIndex;
     while(match<markers.Count&&markers[match].Code!=row[0])match++;
     if(match<markers.Count){
      markerIndex=match+1;
      if(markers[match].X>=prior)position=markers[match].X;
      else if(Bar(row))for(int candidate=markerIndex;candidate<markers.Count&&markers[candidate].Slot<=100;candidate++){
       if(markers[candidate].Code!=14||markers[candidate].X<prior)continue;
       position=markers[candidate].X;
       break;
      }
     }
     records[i][2]=position;
     continue;
    }
    float query=records[i][2];
    while(upper<=query){
     if(interval+2>=anchors.Length)throw new NotSupportedException("Reference onset falls beyond the verified anchor table.");
     interval++;
     left=upper;
     upper=(float)((double)queryGrid[interval+1]-.0010000000474974513);
    }
    records[i][2]=(float)(((double)upperAnchors[interval]-anchors[interval])*((double)query-left)/
     ((double)upper-left)+anchors[interval]);
   }
   for(int i=records.Count-1;i>=first;i--){
    bool grace=Grace(records[i]),indefinite=float.IsNaN(records[i][2]);
    if(!grace&&(!indefinite||records[i][0]==1&&P(records[i],15)==-1))continue;
    int following=i+1;
    while(following<records.Count&&(grace&&GraceMember(records[following])||
      indefinite&&records[following][0]==1&&P(records[following],15)==-1))following++;
    float position=(float)((double)end-4);
    if(following<records.Count){
     int previous=i-1;
     while(previous>0&&!(records[previous][2]>0))previous--;
     float next=records[following][2],gap=(float)((double)next-records[previous][2]);
     float offset=gap<=4?(float)((double)gap/2):3;
     position=(float)((double)next-offset);
    }
    for(int member=i;member<following;member++)records[member][2]=position;
   }
   for(int i=first;i<records.Count;i++){
    var row=records[i];
    if(!Structural(row))continue;
    bool trailing=i==records.Count-1;
    float next=trailing?end:records[i+1][2];
    float position=row[2],prior=records[i-1][2];
    if(next>=position&&position!=prior)continue;
    if(Bar(row))row[2]=trailing?200:(float)((double)next-(double)scale*2.5);
    else if(row[0]==18){
     int previousCode=(int)records[i-1][0];
     float origin=trailing&&previousCode!=14?end:prior;
     row[2]=(float)((double)origin+(double)scale*(previousCode==3?5.5:1.5));
    }
    else if(row[0]==17&&trailing)row[2]=(float)((double)prior+(double)scale*2);
    else{
     float width=row[0]==3?6.5f:(float)(((double)Math.Abs(P(row,5))-1)*2.5+4.5);
     double gap=(double)next-position;
     if(gap<width)width=(float)(gap/2);
     row[2]=(float)((double)next-(double)scale*width);
    }
   }
   if(float.IsNaN(actual)||Math.Abs((float)((double)total-actual))<=.007000000216066837f)return null;
   return String.Format(CultureInfo.InvariantCulture,
    "*** Mismatch - Spacing Staff has {0,7:F4} Quarter Notes.\r\n{1,8} Quarter notes on this staff.",
    total,actual==0?".0000":actual.ToString("F4",CultureInfo.InvariantCulture));
  }
 }
}
````

## InputRhythmDecoder

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputRhythmDecoder.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct InputRhythmValue {
  public float Duration,DotControl,Denominator,FlagCount;
 }
 public static class InputRhythmDecoder {
  // Numerical projection of original section 56, 0263D:000D..012F.
  // Requires finite positive, bounded input. Stores to binary32 are explicit.
  // This does not model x87 exception/condition flags or invalid conversion states.
  public static InputRhythmValue Decode(float encoded) {
   if(float.IsNaN(encoded)||float.IsInfinity(encoded)||encoded<0||encoded>=3276800f)throw new ArgumentOutOfRangeException("encoded");
   float duration=(float)((double)encoded%1000.0);
   var result=new InputRhythmValue{Duration=duration};
   if(duration<=0)return result;
   short dots=ConvertWord((double)encoded/100.0);
   result.DotControl=dots;
   float baseDuration=duration;
   if(dots!=0){double power=Math.Pow(2.0,dots/10.0);float storedPower=(float)power;baseDuration=(float)((double)duration*storedPower/(storedPower*2.0-1.0));}
   double quotient=4.0/baseDuration;result.Denominator=(float)quotient;
   short denominator=ConvertWord(quotient+0.0010000000474974513);
   short threshold=8,count=1;
   while(denominator>=threshold&&count<11){threshold=unchecked((short)(threshold<<1));count++;}
   result.FlagCount=count-1;
   return result;
  }
  static short ConvertWord(double value){if(value<short.MinValue||value>=32768||double.IsNaN(value))throw new ArgumentOutOfRangeException("encoded","Original conversion is outside this finite numerical projection.");return (short)Math.Truncate(value);}
 }
}
````

## InputRhythmRecords

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputRhythmRecords.cs)

````csharp
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
````

## InputRhythmToken

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputRhythmToken.cs)

````csharp
using System;
using System.Globalization;
using System.IO;
namespace ScoreModern.RecoveredFragments {
 public static class InputRhythmToken {
  // Candidate transcription of the section 67 rhythm-token leaf. Slash repetition and list controls
  // belong to the caller; this method accepts one complete duration expression.
  public static float Parse(string text){return Parse(text,false);}
  public static float Parse(string text,bool originalMantissaWrap){
   if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(text.Length==0)throw new InvalidDataException("Missing rhythm.");
   int at=0;float sum=0,packedDots=0;
   while(at<text.Length){
    float duration;bool triplet=false;int start=at;
    if(text[at]=='T'){triplet=true;at++;if(at==text.Length||text[at]=='.'){duration=(float)(4.0/12.0);goto dotted;}}
    string letters="GSEQHWD";float[] denominators={.0625f,16,8,4,2,1,.5f};int letter=letters.IndexOf(text[at]);if(triplet&&text[at]=='T'){at++;duration=(float)(4.0/48.0);goto dotted;}
    if(letter>=0){at++;float denominator=denominators[letter];if(triplet)denominator=(float)((double)denominator*1.5);duration=(float)(4.0/denominator);}
    else{if(triplet)throw new InvalidDataException("A triplet prefix requires a rhythm letter.");float first=Number(text,ref at,originalMantissaWrap);if(at<text.Length&&(text[at]==':'||text[at]=='\'')){at++;float second=Number(text,ref at,originalMantissaWrap);if(first==0)throw new InvalidDataException("Zero rhythm ratio divisor.");duration=(float)((double)second/first);}else{duration=first==0?0:(float)(4.0/first);}}
    dotted: int dots=0;while(at<text.Length&&text[at]=='.'){dots++;at++;}
    float extra=duration;for(int n=0;n<dots;n++){extra=(float)(.5*(double)extra);duration=(float)((double)duration+extra);}
    if(sum==0)packedDots=dots*1000;else packedDots=0;sum=(float)((double)sum+duration);
    if(at==start||float.IsNaN(sum)||float.IsInfinity(sum))throw new InvalidDataException("Invalid rhythm.");
   }
   return (float)((double)sum+packedDots);
  }
  // Positive finite branch of the original resident real reader16D3:05EF..0799:
  // accumulate signed32 digits, store the mantissa and power-of-ten as
  // binary32, then divide. No host float-parser substitution is used.
  static float Number(string text,ref int at,bool originalMantissaWrap){
   int start=at,digits=0,decimals=0,mantissa=0;
   // The rhythm caller consumes a standalone 12-digit token as zero
   // when the original real reader rejects its length.
   if(at==0&&text.Length==12&&Array.TrueForAll(text.ToCharArray(),c=>c>='0'&&c<='9')){
    at=text.Length;
    return 0;
   }
   bool point=false;
   while(at<text.Length){
    char c=text[at];
    if(c>='0'&&c<='9'){
     long next=(long)mantissa*10+c-'0';
     if(!originalMantissaWrap&&next>Int32.MaxValue)
      throw new NotSupportedException("Rhythm decimal mantissa leaves the recovered positive signed32 branch.");
     mantissa=unchecked((int)next);
     digits++;
     if(point)decimals++;
     at++;
    }else if(c=='.'&&!point&&at+1<text.Length&&text[at+1]>='0'&&text[at+1]<='9'){
     point=true;
     at++;
    }else break;
    if(at-start>=12)throw new NotSupportedException("Rhythm number reaches the original real-reader length boundary.");
   }
   if(digits==0||mantissa<0)throw new InvalidDataException("Invalid rhythm token.");
   float storedMantissa=mantissa,storedPower=(float)Math.Pow(10,decimals);
   return(float)((double)storedMantissa/storedPower);
  }

 }
}
````

## InputSetupBounds

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputSetupBounds.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputSetupBoundsResult {public int Left,Right,Clef;public string Key;public int MeterTop,MeterBottom;}
 public static class InputSetupBounds {
  static float P(float[] row,int index){return row.Length>=index?row[index-1]:0;}
  static int W(float n){if(float.IsNaN(n)||float.IsInfinity(n)||n<short.MinValue||n>short.MaxValue)throw new NotSupportedException("Setup bound outside finite signed-word projection");return (short)Math.Truncate(n);}
  // 28b9:184a is the runtime ANINT-to-integer wrapper. Ordinary positive
  // coordinates have direct debugger fixtures; extreme/fault inputs remain
  // separate from this setup projection.
  static int Nearest(double n){double value=Math.Round(n,MidpointRounding.AwayFromZero);if(double.IsNaN(value)||value<short.MinValue||value>short.MaxValue)throw new NotSupportedException("Setup bound conversion outside signed word");return (short)value;}
  public static InputSetupBoundsResult Find(IList<float[]> records,InputSetupState state){
   var result=new InputSetupBoundsResult{Left=state.LeftPosition,Right=state.RightPosition,Key=state.Key,MeterTop=state.MeterTop,MeterBottom=state.MeterBottom};int maximum=-10,right=218,key=0,top=0,bottom=0;
   if(records.Count==0)return result;
   foreach(var row in records){int code=W(P(row,1)),staff=W(P(row,2));if(staff!=state.Staff)continue;if(code==3)result.Clef=W(P(row,5));float position=P(row,3);int x=Nearest(position);
    if(code==8){maximum=Math.Max(maximum,x);int end=row.Length<6?200:W(P(row,6));if(end==0)end=200;right=Math.Min(right,end);continue;}
    if(code!=3&&code<17&&code!=14)continue;
    if(code==14&&x>=50&&x<=159)continue;
    if(x>=100){if(x>=150)right=Math.Min(right,x);continue;}
    float width;
    if(code==3)width=10.239999771118164f;
    else if(code==14)width=1.399999976158142f;
    else if(code==17&&row.Length>4){float value=P(row,5);width=(float)((Math.Abs((double)value%100)-1)*2.799999952316284+2+1);key=W(value);}
    else{width=6;if(row.Length>4)top=W(P(row,5));if(row.Length>5)bottom=W(P(row,6));}
    maximum=Math.Max(maximum,Nearest((double)state.Size*width+position));
   }
   if(key!=0&&key>=-7&&key<=7){string[] names={"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#"};result.Key=names[key+7];}
   if(top!=0){result.MeterTop=top;result.MeterBottom=bottom;}
   result.Left=maximum==-10?state.LeftEnd:maximum;result.Right=right==218?state.RightEnd:right;return result;
  }
 }
}
````

## InputSetupState

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputSetupState.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputSetupState {
  public int Staff=1,LeftEnd,Vertical,RightEnd=200,Spacing=-1,LeftPosition,RightPosition=200,MeterTop=4,MeterBottom=4,Selected=1;
  public float Size=1;public string Key="C";
  // Resident 1a75:000f: the range test uses the truncated integer, even
  // for the size field, whose accepted value is stored as a float.
  static readonly int[] minimum={0,1,-99,-99,0,1,1,-99,1,1,1};
  static readonly int[] maximum={0,32,199,99,9,216,32,199,216,99,99};
  public void MoveField(int delta){Selected+=delta;if(Selected>11)Selected=1;if(Selected<1)Selected=11;}
  public bool Numeric(float value){if(Selected<1||Selected>10)throw new InvalidOperationException("Numeric setup field required");if(float.IsNaN(value)||float.IsInfinity(value)||value<int.MinValue||value>int.MaxValue)throw new ArgumentOutOfRangeException("value");int n=(int)Math.Truncate(value);if(n<minimum[Selected]||n>maximum[Selected])throw new ArgumentOutOfRangeException("value","Outside the original setup field range");
   switch(Selected){case 1:Staff=n;return true;case 2:if(RightEnd<=n)RightEnd=n+5;if(LeftPosition<n)LeftPosition=n;LeftEnd=n;break;case 3:Vertical=n;break;case 4:Size=value;break;case 5:if(n<=LeftEnd)LeftEnd=n-5;if(n<RightPosition)RightPosition=n;RightEnd=n;break;case 6:Spacing=n;break;case 7:if(RightPosition<=n)RightPosition=n+5;LeftPosition=n;break;case 8:if(n<=LeftPosition)LeftPosition=n-5;RightPosition=n;break;case 9:MeterTop=n;break;case 10:MeterBottom=n;break;}return false;
  }
  public void SpacingLetter(char letter){if(Selected!=6)throw new InvalidOperationException("Select spacing first");if(letter=='R')Spacing=-1;else if(letter=='P')Spacing=0;else throw new ArgumentException("R or P required");}
  public void KeyText(string key){if(Selected!=11)throw new InvalidOperationException("Select key signature first");if(key==null||key.Length<1||key.Length>2||key[0]<'A'||key[0]>'G')throw new ArgumentException("Key signature text");if(key.Length==2){char suffix=key[1];if(suffix=='F'||suffix=='B')suffix='b';if(suffix=='S')suffix='#';if(suffix!='b'&&suffix!='#')throw new ArgumentException("Key signature suffix");if(suffix=='b'&&key[0]=='F'||suffix=='#'&&key[0]!='C'&&key[0]!='F')throw new ArgumentException("Outside the original setup key signatures");key=key[0].ToString()+suffix;}Key=key;}
  // Overlay8 section61's filename selection. Files remain external.
  public static string PresetName(int scan){if(scan<0x3b||scan>0x71||(scan>0x44&&scan<0x54))throw new ArgumentOutOfRangeException("scan");if(scan<0x44)return "F"+(scan-0x3a)+".SET";if(scan<0x5d)return scan==0x44?"F10.SET":"F1"+(char)(scan-0x23)+".SET";if(scan<0x68)scan+=10;return scan==0x71?"F30.SET":"F2"+(char)(scan-0x37)+".SET";}
  public float[] StaffRecord(){return new float[]{8,Staff,LeftEnd,Vertical,Size==1?0:Size,RightEnd==200?0:RightEnd};}
 }
}
````

## InputSlurConstruction

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputSlurConstruction.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputSlurConstruction {
  static float F(double v){return (float)v;}static int W(float v){return unchecked((short)(int)Math.Truncate(v));}static bool BitsEqual(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}
  public static float InitialHeight(float left,float right,float y1,float y2,float offset,float height){if(offset<1){float span=F((double)right-left);if(offset< -1&&offset> -4)span=F((double)offset*2+span);float result=F((double)span/20+Math.Abs((double)y2-y1)*0.10000000149011612+0.8999999761581421);if(BitConverter.ToInt32(BitConverter.GetBytes(height),0)<0)result=-result;return result;}return height;}
  static float AvoidLine(float y,int direction){if(y%2==1){int i=W(y);if(direction==10){if(i<=11)y=F(1.0+y);}else if(i>=3)y=F((double)y-1);}return y;}
  static int Stem(float[] p){int stem=(W(p[4])%100)/10*10;return stem!=0?stem:W(p[3])>6?20:10;}
  static float Attach(float[] p,float pitch,int stem,int direction){float space=F((double)(p[7]%100)+8);if(W(p[4])%100<10)space=2;return stem<20?(direction==20?F((double)pitch-2):F((double)pitch+space)):(direction==20?F((double)pitch-space):F((double)pitch+2));}
  static readonly int[] marks={4,5,6,7,8,9,10,19,30,31,32,33,34,35};
  static float CorrectMark(float[] note,float y,int direction,int stem,int number){float mark=note[10];int word=W(mark);if(word==0||Math.Abs((double)mark-word)>0.10000000149011612||Array.IndexOf(marks,Math.Abs(word))<0)return y;float shift=1.5f;if(word<1){if(stem!=direction)return y;}else{if(stem==direction)return y;if(number==0){if(Math.Abs(word)!=6&&Math.Abs(word)!=7){if(mark%1<=0.20000000298023224)note[10]=F((double)mark+0.18000000715255737);return y;}int pitch=W(note[3]%100);if(pitch%2==1&&((pitch<9&&stem==20)||(pitch>5&&stem==10)))shift=2.5f;}}if(direction==20)shift=-shift;return F((double)shift+y);}
  // Section58 slur/parallel-tie construction. Cross-staff endpoint mutation
  // is a separate routine. Existing input records are cloned.
  public static List<float[]> Build(IList<float[]> original,InputEndpointResult e,float staff,int boundary,short tiesMode,float end,InputEndpointStaffContext context=null){
   var rows=new List<float[]>();foreach(var p in original){if(p.Length!=15)throw new ArgumentException("Fifteen words required.");if(p[0]==1&&p[11]!=0&&context==null)throw new NotSupportedException("Cross-staff slur branch required.");rows.Add((float[])p.Clone());}
   int first=e.StartItem-1,last=e.EndItem-1;if(first<0||last<0||first>=rows.Count||last>=rows.Count)throw new ArgumentOutOfRangeException("e");int ties=tiesMode;if(first+1<rows.Count&&rows[first+1][2]==rows[first][2])ties=0;
   float pitch1=e.StartPitch,pitch2=e.EndPitch;int stem1=Stem(rows[e.StartNote==99?last:first]),stem2=Stem(rows[e.EndNote==99?first:last]);if(e.StartNote==99)pitch1=pitch2;if(e.EndNote==99)pitch2=pitch1;
   int direction=e.Stem;if(direction==0){if(e.StartFlag!=0)direction=30-stem1;else{direction=20;for(int i=first;i<=last;i++)if(rows[i][0]==1){int s=W(rows[i][4])%100;if(s<10&&rows[i][3]%100>6)break;if(s>19){direction=10;break;}}}}
   float x1=e.StartX,x2=e.EndX,y1=0,y2=0,height=0,offset=0;int mode=0,number=e.Number;if(e.EndNote>=boundary)x2=end;
   bool tiePath=e.StartNote==0||(e.StartNote!=99&&e.EndNote!=99&&(e.EndNote>=boundary||(number==0&&BitsEqual(pitch1,pitch2)&&e.EndNote-e.StartNote==1)));
   if(e.StartNote==0||e.StartNote==99)x1=F((double)rows[first][2]-5);
   bool attach=true;
   if(tiePath){mode=1;if(ties>0)offset=0.4000000059604645f;else if(!((double)x2-x1<10&&ties>=0)&&W(rows[last][4])%10==0){float shift=W(pitch1)%2!=0?1.2000000476837158f:.5f;if(direction==20)shift=-shift;y1=y2=F((double)shift+pitch1);offset=rows[first][8]>=10?-3:-2;attach=false;}else offset=-1;}
   else offset=-1;
   if(attach){if(e.EndNote>=boundary)x2=F((double)x2-1.5);if(stem1==stem2){y1=Attach(rows[first],pitch1,stem1,direction);y2=Attach(rows[last],pitch2,stem2,direction);if(stem1==direction)offset=direction==10?-4:0;}
    else{float difference=F((double)pitch1-pitch2);if(direction==stem1){y1=y2=Attach(rows[last],pitch2,stem2,direction);if(difference>0&&direction==10)y1=F((double)pitch1+2);if(difference<0&&direction==20)y1=F((double)pitch1-2);}else{y1=y2=Attach(rows[first],pitch1,stem1,direction);if(difference>0&&direction==20)y2=F((double)pitch2-2);if(difference<0&&direction==10)y2=F((double)pitch2+2);}}
    if(mode!=1){y1=AvoidLine(y1,direction);y2=AvoidLine(y2,direction);}if(number!=0){offset=3;height=2;}else height=InitialHeight(x1,x2,y1,y2,offset,0);
   }else height=InitialHeight(x1,x2,y1,y2,offset,0);
   if(direction==20)height=-height;
   if(mode==0){y1=CorrectMark(rows[first],y1,direction,stem1,number);y2=CorrectMark(rows[last],y2,direction,stem2,number);if(e.EndNote-e.StartNote>1)AvoidInterior(rows,first,last,x1,x2,direction,number,ref y1,ref y2,ref height);}
   var result=new float[15];result[0]=5;result[1]=staff;result[2]=x1;result[3]=y1;result[4]=y2;result[5]=x2;result[6]=height;result[7]=offset;result[8]=number;int originalCount=rows.Count;rows.Add(result);
   if(mode>=1){int members=1;while(first+members<originalCount&&rows[first+members][2]==rows[first][2])members++;if(members>1){int remaining=members,threshold=members/2+1,pointer=first;while(remaining>1){if(remaining==threshold){height=-height;direction=30-direction;}remaining--;pointer++;float pitch=rows[pointer][3]%100;float shift=W(pitch)%2!=0?1.2000000476837158f:.5f;if(direction==20)shift=-shift;float y=F((double)pitch+shift);var parallel=(float[])result.Clone();parallel[3]=parallel[4]=y;parallel[6]=height;rows.Add(parallel);}}}
   return rows;
  }
  static void AvoidInterior(List<float[]> rows,int first,int last,float x1,float x2,int direction,int number,ref float y1,ref float y2,ref float height){if(x1==x2)throw new NotSupportedException("Zero horizontal slur span requires the fault branch.");int start=first;
   while(true){float slope=F(((double)y2-y1)/((double)x2-x1)),baseline=F((double)y1-(direction==20?-2.5:2.5));bool restart=false;for(int i=start+1;i<last;i++){var p=rows[i];if(p[0]!=1)continue;int stem=(W(p[4])%100)/10*10;if(stem==direction)continue;float line=F(((double)p[2]-x1)*slope+baseline+height),mark=0;if(p[10]>0){float part=F((double)(p[10]%1)*10);mark=stem==10?F(-(double)part-2):F(2.0+part);}float pitch=p[3];if(pitch<100){if(pitch<=-100)pitch=F(100.0+pitch);}else pitch=F((double)pitch-100);float obstacle=F((double)pitch+mark);if(direction==20?obstacle<line:line<obstacle){float difference=F((double)obstacle-line);if(number==0)height=F((double)difference+height);else{float shift=F(.5+difference);if(direction==20)shift=F((double)shift-1);y1=F((double)shift+y1);y2=F((double)shift+y2);start=i;restart=true;break;}}}if(!restart)return;
   }
  }
 }
}
````

## InputStaccatoCentering

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputStaccatoCentering.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputStaccatoCentering {
  // Runtime316D:00E4..0220 at load0824 (relative2949). The numeric
  // projection follows the captured DOSBox-X x87 path; it is not an x87
  // status/exception emulator. Its multiplication intermediates are binary64.
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static double D(double v){return BitConverter.ToDouble(BitConverter.GetBytes(v),0);}
  static short W(float v){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(v),0),out n))throw new NotSupportedException("Centering control exceeds the verified finite conversion domain.");return unchecked((short)n);}
  static void Finite(float v){if(Single.IsNaN(v)||Single.IsInfinity(v))throw new NotSupportedException("Nonfinite temporary mark centering requires original fault verification.");}
  public static bool Apply(float[] p,float[] staffScales){
   if(p==null||p.Length<19)throw new ArgumentException("Missing note parameters.");
   if(p[0]!=1||p[12]!=99)return false;
   float result=0;
   if(BitConverter.ToInt32(BitConverter.GetBytes(p[10]),0)<0&&p[4]>=10&&p[5]!=2&&W(p[10])==-7){
    Finite(p[3]);Finite(p[4]);Finite(p[5]);Finite(p[14]);
    short staff=W(p[1]);if(staff<1||staff>32||staffScales==null||staffScales.Length<=staff)throw new NotSupportedException("Temporary mark centering needs a verified staff1..32 scale.");
    float size=Present(p[14])?p[14]:1;
    if(Math.Abs(p[3])>=100)size=.6000000238418579f;
    Finite(staffScales[staff]);
    double product=D((double)staffScales[staff]*size);
    product=D(product*1.350000023841858f);
    result=(float)D(product/25.0);
    Finite(result);
    if(p[4]>=20)result=BitConverter.ToSingle(BitConverter.GetBytes(BitConverter.ToInt32(BitConverter.GetBytes(result),0)^Int32.MinValue),0);
   }
   p[12]=result;return true;
  }
 }
}
````

## InputStaffTransaction

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputStaffTransaction.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public enum InputStaffStage { Pitches, Rhythms, Marks, Beams, Slurs, Complete }
 // Transaction boundary for the recovered numerical stages. Text expansion,
 // setup/preset merging and screen input remain the caller's responsibility.
 // A failed stage never changes the accepted working records or stage.
 public sealed class InputStaffTransaction {
  readonly float staff,beginning,end,scale,followingScale,beam,dynamics;
  readonly int clef;readonly short ties;readonly bool proportional;readonly float exponent;
  List<float[]> rows=new List<float[]>();int[] map,stems;
  float left;float? rememberedSpacingLeft;InputPitchResult pitches;
  InputReferenceSpacing referenceSpacing;InputEndpointStaffContext staffContext;
  public string ReferenceSpacingWarning {get;private set;}
  public InputStaffStage Stage {get;private set;}
  public float OriginalStaff {get{return staff;}}
  public float OriginalSelectionLeft {get{return Stage==InputStaffStage.Pitches?beginning:left;}}
  public float OriginalSelectionRight {get{return end;}}
  public int InitialClef {get{return clef;}}
  public int RhythmCount {get{return pitches==null?0:pitches.Events;}}
  public InputStaffTransaction(float staff,int clef,float beginning,float end,float scale,float followingScale,float beam,float dynamics,short ties):this(staff,clef,beginning,end,scale,followingScale,beam,dynamics,ties,false,1){}
  public InputStaffTransaction(float staff,int clef,float beginning,float end,float scale,float followingScale,float beam,float dynamics,short ties,bool proportional,float exponent){this.staff=staff;this.clef=clef;this.beginning=beginning;this.end=end;this.scale=scale;this.followingScale=followingScale;this.beam=beam;this.dynamics=dynamics;this.ties=ties;this.proportional=proportional;this.exponent=exponent;Stage=InputStaffStage.Pitches;}
  public InputStaffTransaction Restart(float? correctedBeginning=null,float? correctedEnd=null,bool? correctedProportional=null){
   return new InputStaffTransaction(staff,clef,correctedBeginning??beginning,correctedEnd??end,scale,followingScale,beam,dynamics,ties,correctedProportional??proportional,exponent){referenceSpacing=referenceSpacing,staffContext=staffContext};
  }
  public InputStaffTransaction RestartOnStaff(int number,int initialClef,float factor,float nextFactor){
   return new InputStaffTransaction(number,initialClef,0,200,factor,nextFactor,beam,dynamics,ties,proportional,exponent){staffContext=staffContext==null?null:staffContext.ForStaff(number)};
  }
  public InputStaffTransaction RestartOnStaffRange(int number,int initialClef,float factor,float nextFactor,float correctedBeginning,float correctedEnd,bool? correctedProportional=null){return new InputStaffTransaction(number,initialClef,correctedBeginning,correctedEnd,factor,nextFactor,beam,dynamics,ties,correctedProportional??proportional,exponent){staffContext=staffContext==null?null:staffContext.ForStaff(number)};}
  public void UseStaffCoordinates(InputEndpointStaffContext context){Require(InputStaffStage.Pitches);staffContext=context;}
  public void ObservePitchPreviewLoader(){
   Require(InputStaffStage.Rhythms);
   if(rows.Count==0)return;
   byte[] item=InputRecordCompaction.Compact(rows[rows.Count-1]);
   int count=(int)BitConverter.ToSingle(item,0);
   rememberedSpacingLeft=count>=7?BitConverter.ToSingle(item,28):0;
  }
  public void UseReferenceSpacing(InputReferenceSpacing reference){
   Require(InputStaffStage.Pitches);
   referenceSpacing=reference;
  }
  static List<float[]> Copy(IList<float[]> source){var result=new List<float[]>();foreach(var p in source)result.Add((float[])p.Clone());return result;}
  void Require(InputStaffStage expected){if(Stage!=expected)throw new InvalidOperationException("Expected "+expected+", current stage "+Stage);}
  public List<float[]> WorkingRecords {get{return Copy(rows);}}
  public void UpdateInputMeter(ref int top,ref int bottom){
   for(int index=rows.Count-1;index>=0;index--){
    var row=rows[index];
    if(row[0]!=18)continue;
    top=(int)row[4];
    bottom=(int)row[5];
    return;
   }
  }

  public float[] MouseNotePositions(){
   var positions=new float[map.Length-1];
   for(int i=0;i<positions.Length;i++)positions[i]=rows[map[i+1]-1][2];
   return positions;
  }
  public void Pitches(float[] tokens,bool insetLeft){Pitches(tokens,insetLeft,false);}
  public void Pitches(float[] tokens,bool insetLeft,bool allowLargeInput){Pitches(tokens,insetLeft,allowLargeInput,false);}
  public void Pitches(float[] tokens,bool insetLeft,bool allowLargeInput,bool useReferenceClef){
   Require(InputStaffStage.Pitches);
   // F2 uses the reference staff's opening clef as its pitch-preview left
   // bound. The rhythm pass has a separate first timed anchor.
   float? previewLeft=useReferenceClef&&referenceSpacing!=null?referenceSpacing.OpeningClefX:null;
   var candidate=InputPitchRecords.Compile(tokens,staff,clef,previewLeft??beginning,end,insetLeft&&!previewLeft.HasValue);
   if(!allowLargeInput&&candidate.NoteIndices.Count>150)
    throw new NotSupportedException("SCORE can damage working records above 150 pitched notes. Split the entry or enable larger F2 input buffers.");
   var candidateMap=new int[candidate.NoteIndices.Count+1];var candidateStems=new int[candidateMap.Length];for(int i=1;i<candidateMap.Length;i++){candidateMap[i]=candidate.NoteIndices[i-1];candidateStems[i]=candidate.ForcedStems[i-1];}pitches=candidate;map=candidateMap;stems=candidateStems;rows=Copy(candidate.Records);left=candidate.Left;Stage=InputStaffStage.Rhythms;}
  public void Rhythms(float[] tokens){Rhythms(tokens,false);}
  public void Rhythms(float[] tokens,bool reproduceNonfiniteChordDuration){
   Require(InputStaffStage.Rhythms);
   float? referenceStart=referenceSpacing==null?(float?)null:referenceSpacing.Start;
   float rhythmEnd=referenceSpacing==null?end:referenceSpacing.QueryEnd;
   bool chord=false;
   foreach(var row in rows)if(row[0]==1&&row[14]<0)chord=true;
   var forcedStems=pitches.ForcedStems.ToArray();
   var candidate=chord
    ?InputChordRhythmRecords.Apply(rows,tokens,left,rhythmEnd,scale,forcedStems,proportional,exponent,referenceStart,null,rememberedSpacingLeft)
    :InputRhythmRecords.Apply(rows,tokens,left,rhythmEnd,scale,proportional,exponent,forcedStems,referenceStart,rememberedSpacingLeft);
   string warning=referenceSpacing==null?null:referenceSpacing.Apply(candidate,scale);
   if(chord)for(int i=0;i+1<candidate.Count;i++){
    var row=candidate[i];
    var next=candidate[i+1];
    if(row[0]!=1||row[14]<0||row[6]!=0||next[0]!=1||next[14]!=-1)continue;
    if(!reproduceNonfiniteChordDuration)
     throw new NotSupportedException("A zero-duration chord writes an invalid duration in SCORE. Choose a positive duration or enable original invalid states.");
    // SCORE's x87 indefinite word is retained only in the chord leader.
    row[6]=float.NaN;
   }
   ReferenceSpacingWarning=warning;
   rows=candidate;
   Stage=InputStaffStage.Marks;
  }
  public void Marks(float[] tokens,int[] opposite){Marks(tokens,opposite,false);}
  // Original marks use literal positions 0 and 200. Slur fractions use
  // the input limits, while outgoing ties use the literal end position 200.
  public void Marks(float[] tokens,int[] opposite,bool reproduceRetainedDescriptor){Require(InputStaffStage.Marks);var candidate=InputMarksCompiler.Apply(rows,map,tokens,opposite,staff,0,200,followingScale,dynamics,reproduceRetainedDescriptor);rows=candidate.Records;Stage=InputStaffStage.Beams;}
  public void SkipRhythms(){
   Require(InputStaffStage.Rhythms);
   Stage=InputStaffStage.Marks;
  }
  public void Beams(float[] endpointPairs){Require(InputStaffStage.Beams);if(endpointPairs==null||endpointPairs.Length%2!=0)throw new ArgumentException("Paired beam endpoints required");var candidate=Copy(rows);var candidateStems=(int[])stems.Clone();for(int i=0;i<endpointPairs.Length;i+=2){var result=InputUniformBeam.Build(candidate,map,candidateStems,endpointPairs[i],endpointPairs[i+1],left,end,beam,staffContext);candidate=result.Records;candidateStems=result.ForcedStems;}rows=candidate;stems=candidateStems;Stage=InputStaffStage.Slurs;}
  public void Slurs(float[] endpointPairs){Require(InputStaffStage.Slurs);if(endpointPairs==null||endpointPairs.Length%2!=0)throw new ArgumentException("Paired slur endpoints required");var candidate=Copy(rows);for(int i=0;i<endpointPairs.Length;i+=2){var endpoint=InputEndpointProjection.Decode(endpointPairs[i],endpointPairs[i+1],5,candidate,map,stems,left,end,staffContext);candidate=InputSlurConstruction.Build(candidate,endpoint,staff,map.Length,ties,200,staffContext);}rows=candidate;Stage=InputStaffStage.Complete;}
  public void Beams(InputEndpointTokens tokens){if(tokens==null)throw new ArgumentNullException("tokens");Require(InputStaffStage.Beams);Beams(tokens.Automatic?InputAutomaticBeams.Groups(rows,InputAutomaticBeams.Durations(rows),tokens.GroupControl,tokens.Skip):tokens.Pairs);}
  public List<float[]> Preview(IList<float[]> prefix){var result=Copy(prefix);int first=result.Count+1;foreach(var row in rows){byte[] bytes=InputRecordCompaction.Compact(row);int count=(int)BitConverter.ToSingle(bytes,0);var item=new float[count];Buffer.BlockCopy(bytes,4,item,0,count*4);result.Add(item);}// Original preview item loader 1249:0121..0128 clears absent
   // parameter words. Its final loaded record supplies the retained P7.
   if(Stage==InputStaffStage.Rhythms&&result.Count>0){var last=result[result.Count-1];rememberedSpacingLeft=last.Length>=7?last[6]:0;}
   return InputBeamStemCompletion.Apply(result,first,staffContext);}
  public List<float[]> Commit(IList<float[]> prefix){Require(InputStaffStage.Complete);return Preview(prefix);}
 }
}
````

## InputTextLists

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTextLists.cs)

````csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputMotiveReplacementException:Exception{public readonly char Name;public InputMotiveReplacementException(char name):base("Replacing Motive "+name){Name=name;}}
 public sealed class InputMacroContext {
 internal readonly Dictionary<char,float[]> Definitions=new Dictionary<char,float[]>();internal readonly List<char> Order=new List<char>();readonly Dictionary<char,int> replacementAllowances=new Dictionary<char,int>();public int StoredDefinitionCount{get;private set;}
 public IEnumerable<KeyValuePair<char,float[]>> Entries{get{foreach(char name in Order)yield return new KeyValuePair<char,float[]>(name,(float[])Definitions[name].Clone());}}
 public void AllowReplacement(char name){int count;replacementAllowances.TryGetValue(name,out count);replacementAllowances[name]=count+1;}public void ClearReplacementAllowances(){replacementAllowances.Clear();}
 internal void Define(char name,float[] body){if(Definitions.ContainsKey(name)){int count;if(!replacementAllowances.TryGetValue(name,out count)||count==0)throw new InputMotiveReplacementException(name);replacementAllowances[name]=count-1;}else Order.Add(name);Definitions[name]=(float[])body.Clone();StoredDefinitionCount++;}
 public void Clear(){Definitions.Clear();Order.Clear();replacementAllowances.Clear();StoredDefinitionCount=0;}
 public InputMacroContext Clone(){var result=new InputMacroContext();foreach(char name in Order){result.Order.Add(name);result.Definitions.Add(name,(float[])Definitions[name].Clone());}foreach(var pair in replacementAllowances)result.replacementAllowances.Add(pair.Key,pair.Value);result.StoredDefinitionCount=StoredDefinitionCount;return result;}
 public void ReplaceFrom(InputMacroContext source){var copy=source.Clone();Clear();foreach(char name in copy.Order){Order.Add(name);Definitions.Add(name,copy.Definitions[name]);}foreach(var pair in copy.replacementAllowances)replacementAllowances.Add(pair.Key,pair.Value);StoredDefinitionCount=copy.StoredDefinitionCount;}
 }
 public sealed class InputProximityDelimiterException : FormatException {
  public readonly int Caret;
  public InputProximityDelimiterException(int caret):base("Attach P to the following pitch."){Caret=caret;}
 }
 public static class InputTextLists {
  static string Body(string text){if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(!text.EndsWith(";"))throw new InvalidDataException("Terminate the input line with a semicolon");text=text.Substring(0,text.Length-1);if(text.IndexOf(';')>=0)throw new InvalidDataException("Unexpected input after a semicolon");return text;}
  static int Count(string text){int n;if(!int.TryParse(text,out n)||n<1||n>short.MaxValue)throw new InvalidDataException("Repeat count is outside the signed word representation");return n;}
  static int DecodeRepeatCount(string text,ref bool keepAccidentals){
   int total=Count(text);
   // SCORE stores the preserve-accidentals flag in the hundreds range.
   // Explicit X counts reaching that range take the same decoding path.
   if(!keepAccidentals&&total>=100){
    keepAccidentals=true;
    total=Math.Max(1,total-100);
   }
   return total;
  }
  static string[] Fields(string text){string body=Body(text);var fields=new List<string>();int start=0,depth=0;for(int i=0;i<body.Length;i++){if(body[i]=='['){if(depth!=0)throw new NotSupportedException("Nested macro definitions require the original error path");depth++;}else if(body[i]==']'){if(depth!=1)throw new InvalidDataException("Unmatched macro bracket");depth--;}else if(body[i]=='/'&&depth==0){fields.Add(body.Substring(start,i-start));start=i+1;}}if(depth!=0)throw new InvalidDataException("Unclosed macro definition");fields.Add(body.Substring(start));return fields.ToArray();}
  internal static string[] ExpansionFields(string text){return Fields(text);}
  static bool Macro(string text,InputMacroContext context,Func<string,float[]> parse,List<float> result,bool pitches=false){
   Match definition=Regex.Match(text,@"^([A-Z])\[(.*)\]$");if(definition.Success){char name=definition.Groups[1].Value[0];float[] defined=parse(definition.Groups[2].Value+";");context.Define(name,defined);result.AddRange(defined);return true;}
   Match call=Regex.Match(text,@"^@([$\- ]*)([A-Z])([+\-0-9. ]*)$");
   if(!call.Success)return false;
   float[] body;if(!context.Definitions.TryGetValue(call.Groups[2].Value[0],out body))
    throw new InvalidDataException("Undefined input macro "+call.Groups[2].Value);
   float shift=0;string suffix=call.Groups[3].Value.Trim();
   if(suffix.Length>0){int cursor=0;shift=InputRealReader.Read(suffix,ref cursor);
    if(cursor!=suffix.Length)throw new NotSupportedException("This motive suffix reaches an unverified real-reader boundary.");
    if(!pitches&&shift<0)throw new InvalidDataException("Negative rhythm-motive factors are not accepted.");
   }
   var expansion=new List<float>();
   if(call.Groups[1].Value.IndexOf('-')>=0){
    for(int end=body.Length-1;end>=0;){
     int first=end,kind=(int)Math.Abs(body[end])/1000;
     if(body[end]<0&&(kind==1||kind==7)){
      while(first>=0&&body[first]<0)first--;
      if(first<0)throw new InvalidDataException("Reverse motive has no chord leader.");
     }
     for(int i=first;i<=end;i++)expansion.Add(body[i]);
     end=first-1;
    }
   }else expansion.AddRange(body);
   for(int i=0;i<expansion.Count;i++){
    float token=expansion[i],absolute=Math.Abs(token);int kind=(int)absolute/1000;
    if(pitches&&shift!=0&&(kind==1||kind==7)){
     float changed=(float)((double)absolute+shift);expansion[i]=token<0?-changed:changed;
    }else if(shift!=0&&!pitches){float remainder=token%1000;
     expansion[i]=token<1000?(float)((double)token*shift):(float)(((double)remainder*shift+token)-remainder);
    }
   }
   // Overlay8 motive caller's optional $ pass operates on packed pitch
   // words after reversal and displacement, retaining accidental hundreds.
   if(call.Groups[1].Value.IndexOf('$')>=0){
    int anchor=0;
    for(int i=0;i<expansion.Count;i++){
     int value=(int)expansion[i],absolute=Math.Abs(value),kind=absolute/1000;
     if(kind!=1&&kind!=7)continue;
     int degree=absolute%100;
     if(anchor==0){anchor=degree*2;continue;}
     int changed=kind*1000+anchor-degree;if(value<0)changed=-changed;
     expansion[i]=(value%1000/100)*100+changed;
    }
   }
   result.AddRange(expansion);return true;
  }
  // Slash lists, chord members and the literal repeat controls. Macro expansion
  // and original cursor/correction handling are separate caller operations.
  public static float[] Pitches(string text,InputPitchState state){return Pitches(text,state,new InputMacroContext());}
  public static float[] Pitches(string text,InputPitchState state,InputMacroContext context){
   if(text==null)throw new ArgumentNullException("text");
   text=text.TrimEnd();
   int semicolon=text.IndexOf(';');
   if(semicolon>=0&&semicolon<text.Length-1){
    // A semicolon completes the physical row. Inside the pitch list it
    // separates fields, including the tail left by a MIDI Enter overtype.
    char last=text[text.Length-1];
    string body=last==';'||last=='/'?text.Substring(0,text.Length-1):text;
    text=body.Replace(';','/')+";";
   }
   var result=new List<float>();string[] fields=Fields(text);
   int nextField=0,chordState=0;
   for(int fieldIndex=0;fieldIndex<fields.Length;fieldIndex++){
    string field=fields[fieldIndex];
    int delimiter=nextField+field.Length;
    nextField=delimiter+1;
    if(field=="P"&&fieldIndex<fields.Length-1)throw new InputProximityDelimiterException(delimiter);
    string t=field.Trim();if(Macro(t,context,body=>Pitches(body,state,context),result,true))continue;if(t.Length==0){InputListExpansion.Repeat(result,1,2,false);continue;}
    if(t[0]=='.'){InputListExpansion.RepeatBeforePrevious(result,t.Length>1&&t[1]=='.');continue;}
    Match repeat=Regex.Match(t,@"^([XZ])([0-9]+)\s+([0-9]+)$");if(repeat.Success){bool keepSpan=repeat.Groups[1].Value=="Z";int repetitions=DecodeRepeatCount(repeat.Groups[3].Value,ref keepSpan);InputListExpansion.Repeat(result,Count(repeat.Groups[2].Value),repetitions,keepSpan);continue;}
    int total=1;bool keep=false;repeat=Regex.Match(t,@"^(.*?)([XZ])([0-9]+)$");if(repeat.Success){t=repeat.Groups[1].Value;keep=repeat.Groups[2].Value=="Z";total=DecodeRepeatCount(repeat.Groups[3].Value,ref keep);}
    string[] chord=t.Split(new[]{':','\''});int before=result.Count;
    // Original section65's delimiter word survives controls. A colon arms
    // the chord before the current token is emitted; a slash changes an
    // active chord to its final-member state. Only a pitch consumes it.
    for(int i=0;i<chord.Length;i++){
     bool colon=i+1<chord.Length;
     if(colon){if(chordState==0)chordState=-1;}else if(chordState>0)chordState=2;
     float? token=InputPitchToken.Parse(chord[i],state);
     if(token.HasValue){int kind=(int)Math.Abs(token.Value)/1000;
      if(kind==1||kind==7){result.Add(chordState>0?-Math.Abs(token.Value):token.Value);if(chordState<0)chordState=1;else if(chordState==2)chordState=0;}
      else result.Add(token.Value);
     }
    }
    if(total!=1){if(result.Count==before)throw new InvalidDataException("Nothing to repeat");InputListExpansion.Repeat(result,1,total,keep);}
   }
   return result.ToArray();
  }
  public static float[] Rhythms(string text){return Rhythms(text,new InputMacroContext());}
  public static float[] Rhythms(string text,InputMacroContext context){return Rhythms(text,context,false);}
  public static float[] Rhythms(string text,InputMacroContext context,bool originalMantissaWrap){
   var result=new List<float>();foreach(string field in Fields(text)){string t=field.Trim();if(Macro(t,context,body=>Rhythms(body,context,originalMantissaWrap),result))continue;if(t.Length==0){InputListExpansion.Repeat(result,1,2,true);continue;}Match m=Regex.Match(t,@"^X([0-9]+)\s+([0-9]+)$");if(m.Success){bool keepSpan=false;int repetitions=DecodeRepeatCount(m.Groups[2].Value,ref keepSpan);InputListExpansion.Repeat(result,Count(m.Groups[1].Value),repetitions,true);continue;}int total=1;m=Regex.Match(t,@"^(.*?)X([0-9]+)$");if(m.Success){t=m.Groups[1].Value;bool keep=false;total=DecodeRepeatCount(m.Groups[2].Value,ref keep);}result.Add(InputRhythmToken.Parse(t,originalMantissaWrap));InputListExpansion.Repeat(result,1,total,true);}return result.ToArray();
  }
 }
}
````

## InputTextTerminatorWrite

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTextTerminatorWrite.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Measured 0A33:0090..0096: LEA DI,[BX+SI]; MOV SI,0136; MOVSB.
 // DS:0136 is the measured ASCII space. This models that one byte write,
 // including 16-bit address addition, not the surrounding text parser.
 public static class InputTextTerminatorWrite {
  public const byte Space=0x20;
  public static ushort Offset(ushort destination,ushort length){return unchecked((ushort)(destination+length));}
  public static ushort Apply(byte[] segment,ushort destination,ushort length){
   if(segment==null||segment.Length!=65536)throw new ArgumentException("A complete measured 16-bit destination segment is required.");
   ushort offset=Offset(destination,length);segment[offset]=Space;return offset;
  }
 }
}
````

## InputTranspositionAccidentalTail

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionAccidentalTail.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionAccidentalTailResult {
  public short[] Flags;public short ActiveRelationships,RelationshipIndex;
  public TranspositionRelationshipWindow Window;
 }
 // Main argument-zero continuation 3195:0822 -> 0C54..10D5. The
 // caller provides every pitch-table word, including retained key/rest slots.
 public static class InputTranspositionAccidentalTail {
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long v;if(!FiniteBinary32Integer64.TryConvert(Bits(f),out v))throw new NotSupportedException("Non-finite accidental parameter.");return unchecked((short)v);}
  static void Clear(IList<float[]> rows,int item){if(item<1||item>rows.Count||rows[item-1].Length<5)throw new NotSupportedException("Accidental record is too short.");rows[item-1][4]=unchecked((short)(Word(rows[item-1][4])/10*10));}
  public static TranspositionAccidentalTailResult Apply(IList<float[]> rows,IList<TranspositionRecord> entries,short[] pitches,IList<int> relationships,short activeCount,float stopPosition,short kindZeroGuard,TranspositionRelationshipWindow initialWindow,float margin,float width){
   int count=entries.Count;if(count==0)throw new NotSupportedException("Empty transposition engine table violates the recovered caller precondition.");
   if(count>200||pitches.Length!=count)throw new NotSupportedException("Accidental tail table dimensions differ.");
   var flags=new short[count];short cursor=0;var window=initialWindow;
   InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
   for(int i=0;i<count;i++){
    var current=entries[i];short kind=current.Kind;
    // 1041 indexes the guard by kind (0/1), not by entry number.
    if(kind<0||kind>1||(kind==0?kindZeroGuard:flags[0])<0)continue;
    float x=current.Position;int prior;
    if(activeCount==0||current.Accidental!=0)goto Forward;
    if(x>stopPosition)goto Right;
   Left:
    if(window.StartLow>x)goto Forward;
    if(!(window.StartHigh<x))goto Forward;
    if(window.EndLow<=x)goto ClearCurrent;
    InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
    if(activeCount==0)goto Forward;
    goto Left;
   Right:
    if(current.Kind!=1)goto Forward;
    if(x<=window.StartHigh)goto Forward;
    if(x<window.EndLow||x>window.EndHigh)goto AdvanceRight;
    prior=i;
   Previous:
    if(--prior<0)goto AdvanceRight;
    if(entries[prior].Kind<0||entries[prior].Kind>1||entries[prior].Clef!=current.Clef)goto AdvanceRight;
    if(Bits(entries[prior].Position)==Bits(x))goto Previous;
    if(pitches[prior]==pitches[i])goto ClearCurrent;
    float previousPosition=entries[prior].Position;
   PreviousChord:
    if(--prior<0||Bits(entries[prior].Position)!=Bits(previousPosition))goto AdvanceRight;
    if(pitches[prior]==pitches[i])goto ClearCurrent;
    goto PreviousChord;
   AdvanceRight:
    InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
    if(activeCount==0)goto Forward;
    goto Right;
   ClearCurrent:
    Clear(rows,current.Item);
    InputTranspositionRelationshipWindow.Advance(rows,relationships,ref activeCount,ref cursor,ref window,margin,width);
    continue;
   Forward:
    for(int j=i+1;j<count;j++){
     var next=entries[j];if(next.Clef!=current.Clef)break;
     if(next.Kind<0)continue;
     if(next.Kind==1&&Bits(next.Position)!=Bits(x))break;
     if(pitches[j]!=pitches[i])continue;
     if(next.Accidental!=0)break;
     Clear(rows,next.Item);flags[j]=-1;
    }
   }
   return new TranspositionAccidentalTailResult{Flags=flags,ActiveRelationships=activeCount,RelationshipIndex=cursor,Window=window};
  }
 }
}
````

## InputTranspositionAttachments

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionAttachments.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Parent TR 2F87:0554..0790, before the optional normalization call.
 // Uses overlap bounds and packed/odd-value rules from the original pass.
 public static class InputTranspositionAttachments {
  // Parent DS:8198 retains the collector relationship count across the
  // measured Main engine; bank:30F4 supplies the extended-beam flag.
  public static bool ShouldApply(short relationshipCount,bool extendedBeam){return relationshipCount!=0||extendedBeam;}
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short Word(double v){if(Double.IsNaN(v)||Double.IsInfinity(v)||v<=Int64.MinValue||v>=Int64.MaxValue)throw new NotSupportedException("Attachment value outside finite integer conversion.");return unchecked((short)(long)Math.Truncate(v));}
  public static void Apply(IList<float[]> rows,short staff,float left,float right,float diatonic,bool enabled){
   if(!enabled)return;short lower=Word((double)left-4f),upper=Word((double)right+4f);
   foreach(var row in rows){
    if(row.Length<6||Word(row[1])!=staff)continue;short code=Word(row[0]);if(code!=5&&code!=6)continue;
    if(Word(row[2])>upper||Word(row[5])<lower)continue;
    bool slur=code==5;float direction=row.Length>=7&&(Bits(row[6])&0x80000000)!=0?-1f:1f;
    float value=(float)((double)row[4]+diatonic);
    if(slur&&value%2f==1f)value=(float)((double)value+direction);
    row[4]=value;
    value=row[3];
    // 0752..0788 stores remainder+shift at 8190, then tests its
    // high word at 8192. The new remainder chooses the packed sign,
    // including when transposition crosses zero; not the old P4 sign.
    if(Math.Abs(value)>=100f){float remainder=(float)((double)value%100f+diatonic);value=(Bits(remainder)&0x80000000)!=0?-100f:100f;value=(float)((double)value+remainder);}
    else value=(float)((double)value+diatonic);
    if(slur&&value%2f==1f)value=(float)((double)value+direction);
    row[3]=value;
   }
  }
 }
}
````

## InputTranspositionClefs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionClefs.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionClefState {
  public short RetainedStaff,RetainedTarget;
  public TranspositionClefState(short staff,short target){RetainedStaff=staff;RetainedTarget=target;}
 }
 // 2DC2:1665..1AC4. Caller supplies retained state explicitly: the
 // original final clef scan uses it even if no entry changes clef.
 public static class InputTranspositionClefs {
  static readonly short[,] shifts={{0,12,6,8},{-12,0,-6,-4},{-6,6,0,2},{-8,4,-2,0}};
  static short Word(float value){long v;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out v))throw new NotSupportedException("Non-finite clef conversion parameter.");return unchecked((short)v);}
  static bool Negative(float value){return (BitConverter.ToUInt32(BitConverter.GetBytes(value),0)&0x80000000)!=0;}
  public static void Apply(IList<float[]> rows,IList<TranspositionRecord> entries,short[] pitches,float staff,short target,TranspositionClefState state){
   if(state==null||entries.Count!=pitches.Length||target<0||target>3)throw new NotSupportedException("Clef conversion caller state outside recovered domain.");
   int cursor=0;
   for(int i=0;i<entries.Count;){
    short old=entries[i].Clef;if(old==target){i++;continue;}
    if(old<0||old>3)throw new NotSupportedException("Original clef shift table index outside measured four-clef domain.");
    short delta=shifts[old,target];int last=i;
    while(last<entries.Count&&entries[last].Clef==old){
     var entry=entries[last];var row=rows[entry.Item-1];
     if(entry.Kind>=0&&entry.Kind!=2){
      pitches[last]=unchecked((short)(pitches[last]+delta));
      if(row.Length>=4){short pitch=pitches[last],stored=Word(row[3]);if(Math.Abs((int)stored)>=100)pitch=unchecked((short)(pitch+(stored<0?-100:100)));row[3]=pitch;}
     }
     entry.Clef=target;last++;
    }
    int final=last-1;state.RetainedStaff=Word(staff);state.RetainedTarget=target;
    if(cursor>=rows.Count)throw new NotSupportedException("Clef conversion cursor leaves item list.");
    float lower=(float)((double)rows[cursor][2]-4f),upper=(float)((double)rows[entries[final].Item-1][2]+4f);
    for(int n=0;n<rows.Count;n++){
     var row=rows[n];if(row.Length<3||Word(row[1])!=state.RetainedStaff)continue;short code=Word(row[0]);
     if((code==5||code==6)&&row.Length>=5&&row[2]>=lower&&row[2]<=upper){
      float first=row[3];
      if(code==6&&Math.Abs(first)>=100f){float remainder=(float)((double)first%100f+delta);first=Negative(remainder)?-100f:100f;first=(float)((double)first+remainder);}
      else first=(float)((double)first+delta);
      row[3]=first;row[4]=(float)((double)row[4]+delta);
     }
     if(code==17){if(row.Length<6){Array.Resize(ref row,6);rows[n]=row;}row[5]=target;}
    }
    cursor=entries[final].Item;i=last;
   }
   bool seen=false;
   for(int n=0;n<rows.Count;n++){
    var row=rows[n];if(row.Length<2||Word(row[1])!=state.RetainedStaff||Word(row[0])!=3)continue;
    if(seen){row[0]=13;continue;}seen=true;
    if(row.Length<5){if(state.RetainedTarget==0)continue;Array.Resize(ref row,5);rows[n]=row;}
    row[4]=state.RetainedTarget;
   }
  }
 }
}
````

## InputTranspositionCollection

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionCollection.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionRecord {
  public int Item;public short Kind,Accidental,Clef;public short? Pitch;public float Position;
 }
 public sealed class TranspositionCollection {
  public readonly List<TranspositionRecord> Records=new List<TranspositionRecord>();
  public readonly List<int> RelationshipItems=new List<int>();
  public bool HasExtendedBeam;public short FirstKeyState=-1;public float? FirstNotePosition;
 }
 // First pass only: original 32AC:000C..03D3. Subsequent bar/voice
 // partitioning is deliberately separate and not implemented by this leaf.
 public static class InputTranspositionCollection {
  static readonly short[] accidental={0,-1,1,3,-2,2};
  static short W(float value){long word;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word))throw new NotSupportedException("Non-finite transposition parameter.");return unchecked((short)word);}
  static float P(float[] row,int number){return row.Length>=number?row[number-1]:0;}
  public static TranspositionCollection FirstPass(IList<float[]> rows,short staff,float expandedLeft,float expandedRight,short mode){return FirstPass(rows,staff,expandedLeft,expandedRight,mode,null);}
  public static TranspositionCollection FirstPass(IList<float[]> rows,short staff,float expandedLeft,float expandedRight,short mode,short[] recoveredAccidentalTable){
   var map=recoveredAccidentalTable??accidental;
   var result=new TranspositionCollection();short clef=0;bool selectedNoteSeen=false;
   for(int index=0;index<rows.Count;index++){
    var row=rows[index];if(W(P(row,2))!=staff)continue;
    float x=P(row,3);if(x>expandedRight)break;
    short code=W(P(row,1));
    if(code<=2){
     if(x<expandedLeft)continue;
     if(row.Length>=7&&(BitConverter.ToUInt32(BitConverter.GetBytes(P(row,7)),0)&0x80000000)!=0)continue;
     if(row.Length<7||P(row,7)==0)throw new NotSupportedException("Original transposition refuses a selected note/rest with no duration.");
     if(code!=1&&code!=2)throw new NotSupportedException("Unverified transposition record code.");
     if(result.Records.Count>=200)throw new NotSupportedException("Transposition collection exceeds its recovered table capacity.");
     short value=0;short? pitch=null;
     if(code!=2){int digit=W(P(row,5))%10;if(digit<0||digit>=map.Length)throw new NotSupportedException("Accidental table index outside recovered range.");value=map[digit];pitch=unchecked((short)(W(P(row,4))%100));}
     if(!selectedNoteSeen)result.FirstNotePosition=x;
     result.Records.Add(new TranspositionRecord{Item=index+1,Kind=code==2?(short)-1:(short)0,Accidental=value,Pitch=pitch,Clef=clef,Position=x});
     selectedNoteSeen=true;
    }else if(code==5&&row.Length>=7){
     // 32AC:015D..01E7: compare the two stored float words, not
     // numerical equality; negative zero and positive zero differ here.
     bool enabled=row.Length<8||P(row,8)<=0;
     if(enabled&&BitConverter.ToUInt32(BitConverter.GetBytes(P(row,4)),0)==BitConverter.ToUInt32(BitConverter.GetBytes(P(row,5)),0)){
      if(result.RelationshipItems.Count>=50)throw new NotSupportedException("Relationship collection exceeds its safe table region.");
      result.RelationshipItems.Add(index+1);
     }
    }else if(code==3&&row.Length>=5){clef=W(P(row,5));if(mode==15&&P(row,5)==0.8f)clef=4;}
    else if(code==17){
     if(result.FirstKeyState<0)result.FirstKeyState=selectedNoteSeen?(short)1:(short)0;
     if(result.Records.Count>=200)throw new NotSupportedException("Transposition collection exceeds its recovered table capacity.");
     result.Records.Add(new TranspositionRecord{Item=index+1,Kind=2,Accidental=W(P(row,5)),Clef=clef,Position=x});
    }else if(code==6&&row.Length>=7)result.HasExtendedBeam=true;
   }
   return result;
  }
  // Original 32AC:03D9..04FC, before boundary sorting/partitioning.
  // Caller supplies the measured first selected note position; key-only
  // selections require separate retained-state verification.
  public static List<float> Boundaries(IList<float[]> rows,short staff,float firstNotePosition,float expandedRight){
   var result=new List<float>();
   foreach(var row in rows){float x=P(row,3);if(x<firstNotePosition)continue;if(x>expandedRight)break;
    if(W(P(row,1))!=14||W(P(row,2))>staff)continue;
    short span=row.Length>=4?W(P(row,4)):(short)1;if(span==0)span=1;
    if(unchecked((short)(staff-W(P(row,2))))>=span)continue;
    uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(x),0);bool found=false;
    foreach(float boundary in result)if(BitConverter.ToUInt32(BitConverter.GetBytes(boundary),0)==bits){found=true;break;}
    if(found)continue;if(result.Count>=99)throw new NotSupportedException("Original boundary table exceeds its safe sentinel capacity.");result.Add(x);
   }
   return result;
  }
 }
}
````

## InputTranspositionCollector

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionCollector.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionCollectorState {
  public float FirstNotePosition;
  public TranspositionCollectorState(float firstNotePosition){FirstNotePosition=firstNotePosition;}
 }
 public sealed class TranspositionCollectorResult {
  public TranspositionCollection FirstPass;public List<float> CollectedBoundaries;
  public TranspositionPartition Partition;public float ExpandedLeft,ExpandedRight;
 }
 // Original 32AC:000C..0750. State is supplied by the caller, not silently
 // reset for key-only/empty selections. Main workflow ownership is separate.
 public static class InputTranspositionCollector {
  public static TranspositionCollectorResult Build(IList<float[]> rows,short staff,float left,float right,short mode,TranspositionCollectorState state){return Build(rows,staff,left,right,mode,state,null);}
  public static TranspositionCollectorResult Build(IList<float[]> rows,short staff,float left,float right,short mode,TranspositionCollectorState state,short[] recoveredAccidentalTable){
   if(state==null)throw new ArgumentNullException("state");
   var result=new TranspositionCollectorResult{ExpandedLeft=(float)((double)left-.001f),ExpandedRight=(float)((double)right+.001f)};
   result.FirstPass=InputTranspositionCollection.FirstPass(rows,staff,result.ExpandedLeft,result.ExpandedRight,mode,recoveredAccidentalTable);
   float first=result.FirstPass.FirstNotePosition??state.FirstNotePosition;
   result.CollectedBoundaries=result.FirstPass.Records.Count==0?new List<float>():InputTranspositionCollection.Boundaries(rows,staff,first,result.ExpandedRight);
   var positions=new List<float>();var kinds=new List<short>();foreach(var record in result.FirstPass.Records){positions.Add(record.Position);kinds.Add(record.Kind);}
   result.Partition=InputTranspositionPartition.Apply(result.CollectedBoundaries,positions,kinds);
   state.FirstNotePosition=first;return result;
  }
 }
}
````

## InputTranspositionEmbeddedControl

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionEmbeddedControl.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Parent TR Code 12 pass, 2F87:03DF..0517. Tables are the measured
 // signed words at DS:41CA and DS:41B2, including their index-zero slots.
 public static class InputTranspositionEmbeddedControl {
  public const string FingeringWarning="******** Fingerings for guitar chords will NOT transpose when P5 = -1 or -2.";
  static readonly short[] pitches={70,1,3,5,6,8,10,12};
  static readonly short[] encodings={6,10,21,20,31,30,40,42,50,61,60,71,70};
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long v;if(!FiniteBinary32Integer64.TryConvert(Bits(f),out v))throw new NotSupportedException("Non-finite embedded control.");return unchecked((short)v);}
  public static void Apply(IList<float[]> rows,short staff,short semitoneClass,out bool warningRequired){
   warningRequired=false;if(semitoneClass%12==0)return;
   if(semitoneClass< -11||semitoneClass>11)throw new NotSupportedException("Parent semitone class is not reduced modulo twelve.");
   foreach(var row in rows){
    if(row.Length<7||Word(row[0])!=12||Word(row[1])!=staff||(Bits(row[4])&0x80000000)==0||Bits(row[6])==0)continue;
    short packed=Word(row[6]);int group=packed/10;if(group<0||group>=pitches.Length)throw new NotSupportedException("Embedded pitch leaves its recovered table.");
    short accidental=unchecked((short)(packed%10));if(accidental==2)accidental=-1;
    short next=unchecked((short)(pitches[group]-accidental+semitoneClass));next=unchecked((short)(next%12));if(next<=0)next=unchecked((short)(next+12));
    row[6]=encodings[next];if(row[4]>=-2f)warningRequired=true;
   }
  }
 }
}
````

## InputTranspositionInterval

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionInterval.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionInterval {public short Semitones,SemitoneClass,Key,Diatonic;}
 // Parent 2F87:019E..0251. Literal DS:4162/4182/419C tables, with
 // signed-word arithmetic and the original single octave increment.
 public static class InputTranspositionInterval {
  static readonly short[] menu={-8,-5,-4,-3,-2,2,3,5,7,8,9,10,14,21,12,-12};
  static readonly short[] keys={-6,1,-4,3,-2,5,0,-5,2,-3,4,-1,6};
  static readonly short[] degrees={0,1,1,2,2,3,3,4,5,5,6,6};
  public static short Menu(short choice){if(choice<1||choice>16)throw new NotSupportedException("Original transposition menu choice must be 1..16.");return menu[choice-1];}
  public static TranspositionInterval Convert(short semitones){
   short keyIndex=unchecked((short)(semitones+7));
   if(keyIndex>13)keyIndex=unchecked((short)(keyIndex-12));
   if(keyIndex>13)keyIndex=unchecked((short)(keyIndex-12));
   if(keyIndex<=0)keyIndex=unchecked((short)(keyIndex+12));
   short absolute=unchecked((short)(semitones<0?-semitones:semitones));
   if(keyIndex<1||keyIndex>keys.Length||absolute<0)throw new NotSupportedException("Original transposition interval leaves its literal lookup tables.");
   short remainder=(short)(absolute%12),diatonic=degrees[remainder];
   if(absolute>=12)diatonic=unchecked((short)(diatonic+7));
   if(semitones<0){remainder=unchecked((short)-remainder);diatonic=unchecked((short)-diatonic);}
   return new TranspositionInterval{Semitones=semitones,SemitoneClass=remainder,Key=keys[keyIndex-1],Diatonic=diatonic};
  }
 }
}
````

## InputTranspositionKey

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionKey.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 // Original 2DC2:1AC7..1C1B and 1C1E..1C56. This is the key/diatonic
 // conversion leaf, not a complete TR command or a note transposer.
 public static class InputTranspositionKey {
  static short W(int value){return unchecked((short)value);}
  public static short KeyPosition(short key){
   short value=key;if(value<0)value=W(value+7);
   if(value%2!=0)value=W(value+7);
   return W(value/2);
  }
  public static void Convert(short semitones,short oldKey,out short diatonic,out short newKey){
   if(semitones==12||semitones==-12){diatonic=semitones==12?(short)7:(short)-7;newKey=oldKey;return;}
   newKey=W(W(oldKey-W(5*semitones))%12);
   if(newKey< -6)newKey=W(newKey+12);if(newKey>6)newKey=W(newKey-12);
   if(newKey==6||newKey==-6)newKey=oldKey<0?(short)-6:(short)6;
   short difference=W(KeyPosition(oldKey)-KeyPosition(newKey));diatonic=difference;
   if(semitones>=0){
    if(difference>0)diatonic=W(7-difference);
    if(difference<0)diatonic=W(-difference);
    if(semitones>10&&diatonic<6){diatonic=W(-difference+7);if(diatonic<6)diatonic=W(diatonic+7);}
   }else{
    if(difference<0)diatonic=W(-7-difference);
    if(difference>0)diatonic=W(-difference);
    if(semitones< -10&&diatonic> -6){diatonic=W(-7-difference);if(diatonic> -6)diatonic=W(diatonic-7);}
   }
  }
 }
}
````

## InputTranspositionKeyInsertion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionKeyInsertion.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // 32AC:0756..0905. Appends the key; does not rebuild the collector table.
 // Constants measured in DS:7E1C,7E22..7E32. Caller owns capacity checks,
 // prompt choices, subsequent sorting and engine key-override state.
 public static class InputTranspositionKeyInsertion {
  // Parent 2F87:030D..0368: K forces the question, not an insertion.
  public static bool ShouldPrompt(bool forceQuestion,short firstKeyState,short semitoneClass){return forceQuestion||(firstKeyState!=0&&semitoneClass!=0);}
  public static bool ShouldInsert(bool answerYes,short firstKeyState,short semitoneClass){return answerYes&&firstKeyState!=0&&semitoneClass!=0;}
  static short Word(float value){long v;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out v))throw new NotSupportedException("Non-finite key-insertion parameter.");return unchecked((short)v);}
  public static float[] Append(IList<float[]> rows,float staff,short key,short clef){
   short selected=Word(staff);float position=999f;bool placed=false;
   // Original scans only through count-1 so a clef has a following record.
   for(int i=0;i<rows.Count-1;i++){
    var row=rows[i];if(row.Length<3)throw new NotSupportedException("Key-insertion scan lacks the original reader fields.");
    if(Word(row[1])!=selected)continue;
    if(row[2]<position)position=row[2];
    if(Word(row[0])!=3)continue;
    position=row[2];var next=rows[i+1];
    if(next.Length>=3)position=(float)(((double)next[2]-position)/2f+position);
    placed=true;break;
   }
   if(!placed)position=(float)((double)position+1f);
   var inserted=new float[]{17f,staff,position,0f,key,clef};rows.Add(inserted);return inserted;
  }
 }
}
````

## InputTranspositionKeyRecord

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionKeyRecord.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionKeyRecordResult {
  public float[] Parameters;
  public short OldKeyBase,NewKeyBase,Diatonic,PitchBase,KeyOverride;
 }
 // Main argument-zero key-record span, 3195:00A2..01EB, then the
 // kind-2 exit through 02A2..02D3 to 0757. Full command ownership is separate.
 public static class InputTranspositionKeyRecord {
  static short W(int value){return unchecked((short)value);}
  public static TranspositionKeyRecordResult Apply(float[] parameters,short collectedKey,short semitones,short previousOverride,short clef,short collectorIndex,float erasedCode){
   if(parameters.Length<3)throw new NotSupportedException("Key record is shorter than the verified loaded-record domain.");
   short old=collectedKey;if(Math.Abs((int)old)>=100)old=0;
   short diatonic,key;InputTranspositionKey.Convert(semitones,old,out diatonic,out key);
   var result=new TranspositionKeyRecordResult{Parameters=(float[])parameters.Clone(),OldKeyBase=W(7*old+49),NewKeyBase=W(7*key+49),Diatonic=diatonic,PitchBase=W(diatonic+6),KeyOverride=key};
   if(key==0&&(collectorIndex==1||previousOverride==0)){
    result.Parameters[0]=erasedCode;
   }else{
    short written=key==0?W((previousOverride<0?-100:100)+previousOverride):key;
    // 0165 skips P5 when it is absent. Later extension for P6 does
    // not retroactively write the new key into the inserted P5 slot.
    if(result.Parameters.Length>=5)result.Parameters[4]=written;
    if(result.Parameters.Length>=6)result.Parameters[5]=clef;
    else if(clef!=0){Array.Resize(ref result.Parameters,6);result.Parameters[5]=clef;}
   }
   return result;
  }
 }
}
````

## InputTranspositionMainPass

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionMainPass.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionMainState {
  public short Semitones,TargetKey,Diatonic,KeyOverride,KindZeroGuard;
  public float Sentinel,ErasedCode,RelationshipMargin,RelationshipWidth;
  public short[] AccidentalMarkers;
  public TranspositionRelationshipWindow InitialWindow;
 }
 public sealed class TranspositionMainPassResult {
  public TranspositionAccidentalTailResult Tail;public float StopPosition;
 }
 // Main argument word zero only, 3195:000D..10D5. Explicit caller state
 // avoids assigning defaults to the retained bank words. Parent attachment,
 // prompt and normalization passes are outside this component.
 public static class InputTranspositionMainPass {
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short W(int value){return unchecked((short)value);}
  static void ResetMarkers(short[] words){Array.Clear(words,1,199);}
  public static TranspositionMainPassResult Apply(IList<float[]> rows,IList<TranspositionRecord> entries,short[] pitches,IList<int> relationships,TranspositionMainState state){
   if(entries.Count==0||entries.Count>200||pitches.Length!=entries.Count||state.AccidentalMarkers==null||state.AccidentalMarkers.Length!=200)throw new NotSupportedException("Main transposition engine table dimensions violate its caller contract.");
   short oldKey=0;float stop=state.Sentinel;ResetMarkers(state.AccidentalMarkers);
   for(int i=0;i<entries.Count;i++){
    var entry=entries[i];if(entry.Item<1||entry.Item>rows.Count)throw new NotSupportedException("Transposition record index is outside the score.");
    var row=rows[entry.Item-1];
    if(entry.Kind==2){
     var key=InputTranspositionKeyRecord.Apply(row,entry.Accidental,state.Semitones,state.KeyOverride,entry.Clef,W(i+1),state.ErasedCode);
     rows[entry.Item-1]=key.Parameters;oldKey=W((key.OldKeyBase-49)/7);state.TargetKey=W((key.NewKeyBase-49)/7);state.Diatonic=key.Diatonic;state.KeyOverride=key.KeyOverride;ResetMarkers(state.AccidentalMarkers);continue;
    }
    if(entry.Kind<0)continue;
    if(entry.Kind!=0&&Bits(entry.Position)!=Bits(state.Sentinel))ResetMarkers(state.AccidentalMarkers);
    if((Bits(stop)&0x80000000)!=0)stop=entry.Position;
    if(row.Length<5)throw new NotSupportedException("Transposed note lacks its verified parameters.");
    var note=InputTranspositionPitch.Apply(row[3],row[4],pitches[i],entry.Accidental,entry.Clef,oldKey,state.TargetKey,state.Diatonic,state.KeyOverride);
    row[3]=note.Pitch;row[4]=note.Stem;
    if(note.AccidentalMarker.HasValue){int index=note.AccidentalMarker.Value;if(index<0||index>=200)throw new NotSupportedException("Accidental marker leaves its recovered table.");state.AccidentalMarkers[index]=1;}
   }
   var tail=InputTranspositionAccidentalTail.Apply(rows,entries,pitches,relationships,W(relationships.Count),stop,state.KindZeroGuard,state.InitialWindow,state.RelationshipMargin,state.RelationshipWidth);
   return new TranspositionMainPassResult{Tail=tail,StopPosition=stop};
  }
 }
}
````

## InputTranspositionOperation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionOperation.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionRequest {
  public float Staff,Left,Right;public short Semitones;public short? Clef;
  public short[] ClefChanges;
  public bool AddKey,ForceKeyQuestion;
 }
 public sealed class TranspositionWorkspace {
  public readonly TranspositionCollectorState Collector;
  public readonly TranspositionClefState Clefs;
  public readonly short[] PitchWords,AccidentalMarkers;
  public short KindZeroGuard;public bool AlreadySorted;
  public TranspositionRelationshipWindow Window;
  public TranspositionWorkspace(TranspositionCollectorState collector,TranspositionClefState clefs,short[] pitchWords,short[] markers,short guard,TranspositionRelationshipWindow window,bool sorted){
   if(collector==null||clefs==null||pitchWords==null||pitchWords.Length!=200||markers==null||markers.Length!=200)throw new ArgumentException("Explicit original transposition workspace required.");
   Collector=collector;Clefs=clefs;PitchWords=pitchWords;AccidentalMarkers=markers;KindZeroGuard=guard;Window=window;AlreadySorted=sorted;
  }
 }
 public sealed class TranspositionOperationResult {
  public List<float[]> Rows;public TranspositionCollectorResult Collection;
  public TranspositionInterval Interval;public TranspositionMainPassResult Engine;
  public bool SortMarksModified,KeyQuestion,InsertedKey,WarningRequired;
 }
 public sealed class TranspositionOperationSession {
  internal TranspositionRequest Request;internal TranspositionWorkspace Workspace;
  internal short[] Pitches;internal short Staff;internal bool Finished;
  public TranspositionOperationResult Result;
 }
 // Recovered Main argument-zero composition. Interactive prompts and final
 // optional normalization belong to the caller. Retained words are supplied,
 // not synthesized from a presumed application history.
 public static class InputTranspositionOperation {
  static short Word(float value){long v;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out v))throw new NotSupportedException("Non-finite transposition staff.");return unchecked((short)v);}
  public static TranspositionOperationResult Apply(IList<float[]> source,TranspositionRequest request,TranspositionWorkspace workspace){
   var session=Begin(source,request,workspace);
   if(request.ClefChanges!=null){
    if(request.Clef.HasValue)throw new ArgumentException("Use one clef choice or an explicit ordered choice sequence.");
    foreach(short choice in request.ClefChanges)ChangeClef(session,choice);
   }else if(request.Clef.HasValue)ChangeClef(session,request.Clef.Value);
   return Finish(session,request.AddKey);
  }
  public static TranspositionOperationSession Begin(IList<float[]> source,TranspositionRequest request,TranspositionWorkspace workspace){
   if(request==null||workspace==null)throw new ArgumentNullException();
   var result=new TranspositionOperationResult();result.SortMarksModified=InputTranspositionOrder.MarksModified(source.Count,workspace.AlreadySorted);
   var order=InputTranspositionOrder.OrderForMain(source,workspace.AlreadySorted);var rows=new List<float[]>();foreach(int i in order)rows.Add((float[])source[i].Clone());result.Rows=rows;
   short staff=Word(request.Staff);var interval=InputTranspositionInterval.Convert(request.Semitones);result.Interval=interval;
   var collection=InputTranspositionCollector.Build(rows,staff,request.Left,request.Right,9,workspace.Collector);result.Collection=collection;
   var entries=collection.FirstPass.Records;int count=entries.Count;
   var pitches=new short[count];Array.Copy(workspace.PitchWords,pitches,count);
   for(int i=0;i<count;i++){entries[i].Kind=collection.Partition.Kinds[i];if(entries[i].Pitch.HasValue)pitches[i]=entries[i].Pitch.Value;}
   result.KeyQuestion=count!=0&&InputTranspositionKeyInsertion.ShouldPrompt(request.ForceKeyQuestion,collection.FirstPass.FirstKeyState,interval.SemitoneClass);
   return new TranspositionOperationSession{Request=request,Workspace=workspace,Pitches=pitches,Staff=staff,Result=result};
  }
  public static void ChangeClef(TranspositionOperationSession session,short choice){
   if(session==null||session.Finished)throw new InvalidOperationException("No active transposition operation.");
   if(session.Pitches.Length==0)return;
   // Preserve each intermediate binary32 store when B revisits the question.
   InputTranspositionClefs.Apply(session.Result.Rows,session.Result.Collection.FirstPass.Records,session.Pitches,session.Request.Staff,choice,session.Workspace.Clefs);
   Array.Copy(session.Pitches,session.Workspace.PitchWords,session.Pitches.Length);
  }
  public static TranspositionOperationResult Finish(TranspositionOperationSession session,bool addKey){
   return FinishCore(session,addKey,true);
  }
  public static TranspositionOperationResult FinishCore(TranspositionOperationSession session,bool addKey,bool mainAttachments){
   if(session==null||session.Finished)throw new InvalidOperationException("No active transposition operation.");
   session.Finished=true;
   var result=session.Result;var request=session.Request;var workspace=session.Workspace;
   var rows=result.Rows;var collection=result.Collection;var interval=result.Interval;var entries=collection.FirstPass.Records;var pitches=session.Pitches;int count=entries.Count;short staff=session.Staff;
   if(count==0)return result;
   short target=interval.Key,diatonic=interval.Diatonic,forcedKey=0;
   result.KeyQuestion=InputTranspositionKeyInsertion.ShouldPrompt(request.ForceKeyQuestion,collection.FirstPass.FirstKeyState,interval.SemitoneClass);
   result.InsertedKey=InputTranspositionKeyInsertion.ShouldInsert(addKey,collection.FirstPass.FirstKeyState,interval.SemitoneClass);
   if(result.InsertedKey){InputTranspositionKey.Convert(interval.Semitones,0,out diatonic,out target);forcedKey=target;InputTranspositionKeyInsertion.Append(rows,request.Staff,target,entries[0].Clef);}
   // Parent D8CE captures this shift before keys inside the engine change it.
   short attachmentShift=diatonic;
   var state=new TranspositionMainState{Semitones=interval.Semitones,TargetKey=target,Diatonic=diatonic,KeyOverride=forcedKey,KindZeroGuard=workspace.KindZeroGuard,Sentinel=-999,ErasedCode=13,RelationshipMargin=3,RelationshipWidth=6,InitialWindow=workspace.Window,AccidentalMarkers=workspace.AccidentalMarkers};
   result.Engine=InputTranspositionMainPass.Apply(rows,entries,pitches,collection.FirstPass.RelationshipItems,state);workspace.Window=result.Engine.Tail.Window;Array.Copy(pitches,workspace.PitchWords,count);
   bool warning;InputTranspositionEmbeddedControl.Apply(rows,staff,interval.SemitoneClass,out warning);result.WarningRequired=warning;
   if(mainAttachments)InputTranspositionAttachments.Apply(rows,staff,request.Left,request.Right,attachmentShift,InputTranspositionAttachments.ShouldApply((short)collection.FirstPass.RelationshipItems.Count,collection.FirstPass.HasExtendedBeam));
   // Main X/nonzero sort intentionally leaves AlreadySorted unchanged.
   return result;
  }
 }
}
````

## InputTranspositionOrder

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionOrder.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Section36:0007, measured at 32B0:0007 for the TR caller. Mode X,
 // nonzero secondary command word: stable signed-32 composite-key order;
 // no trailing-parameter normalization in this caller branch.
 public static class InputTranspositionOrder {
  // 006C..007C and 0390..039C: this caller marks the document
  // modified when sorting runs, even if its final order is unchanged.
  // Its nonzero secondary word returns before setting the sorted flag.
  public static bool MarksModified(int recordCount,bool alreadySorted){return recordCount>=2&&!alreadySorted;}
  static int Integer(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value<Int32.MinValue||value>Int32.MaxValue)throw new NotSupportedException("Sort key outside verified finite signed-32 domain.");return (int)value;}
  public static int[] OrderForMain(IList<float[]> rows,bool alreadySorted){
   int count=rows.Count;var order=new int[count];for(int i=0;i<count;i++)order[i]=i;
   if(count<2||alreadySorted)return order;
   var keys=new int[count];var used=new bool[count];
   for(int i=0;i<count;i++){
    var row=rows[i];if(row.Length<2)throw new NotSupportedException("Sort requires a staff parameter.");
    int position=row.Length>=3?Integer((double)row[2]*100f):0;
    keys[i]=unchecked(position+Integer(row[1])*1000000);
    if(keys[i]>=99000000)throw new NotSupportedException("Original sort key reaches its sentinel domain.");
   }
   for(int output=0;output<count;output++){
    int minimum=99000000,index=-1;
    for(int i=0;i<count;i++)if(!used[i]&&keys[i]<minimum){minimum=keys[i];index=i;}
    if(index<0)throw new NotSupportedException("Original sort has no candidate.");order[output]=index;used[index]=true;
   }
   return order;
  }
 }
}
````

## InputTranspositionPartition

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionPartition.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionPartition {
  public short[] Kinds;public float[] Boundaries;
 }
 // Original 32AC:0506..0750. Inputs are the collected boundary positions,
 // record positions and kind words; boundary collection is a separate pass.
 public static class InputTranspositionPartition {
  public static TranspositionPartition Apply(IList<float> bars,IList<float> positions,IList<short> kinds){
   if(positions.Count!=kinds.Count||(kinds.Count==0&&bars.Count!=0)||kinds.Count>200||bars.Count>99)throw new NotSupportedException("Partition input exceeds recovered table bounds.");
   var result=new TranspositionPartition{Kinds=new short[kinds.Count]};for(int i=0;i<kinds.Count;i++){result.Kinds[i]=kinds[i];if(Single.IsNaN(positions[i])||Single.IsInfinity(positions[i])||positions[i]>=9999)throw new NotSupportedException("Partition position outside verified finite sentinel domain.");}
   if(bars.Count==0){result.Boundaries=new float[0];return result;}
   float[] slots=new float[101];for(int i=0;i<bars.Count;i++){if(Single.IsNaN(bars[i])||Single.IsInfinity(bars[i]))throw new NotSupportedException("Non-finite partition boundary.");slots[i]=bars[i];}
   // Literal original loop: it compares a second slot even for one bar.
   // Equal boundaries can repeat indefinitely; protect that invalid state.
   int current=1,steps=0;
   do{
    current++;
    while(slots[current-2]>=slots[current-1]){
     if(++steps>20000)throw new NotSupportedException("Original boundary sort repeats an equal-boundary state.");
     float swap=slots[current-2];slots[current-2]=slots[current-1];slots[current-1]=swap;
     if(current==2)break;current--;
    }
   }while(current<bars.Count);
   int count=bars.Count;
   do{if(++count>100)throw new NotSupportedException("Original boundary sentinel exceeds its table.");}while(BitConverter.ToUInt32(BitConverter.GetBytes(slots[count-1]),0)!=0);
   slots[count-1]=9999;result.Boundaries=new float[count];Array.Copy(slots,result.Boundaries,count);
   if(result.Kinds[0]==0)result.Kinds[0]=1;
   int boundary=0;
   for(int i=0;i<kinds.Count;i++){
    if(positions[i]<slots[boundary]||result.Kinds[i]<0)continue;
    if(result.Kinds[i]==0){result.Kinds[i]=1;for(int j=i+1;j<kinds.Count&&result.Kinds[j]==0&&positions[j]==positions[i];j++)result.Kinds[j]=1;}
    boundary++;while(positions[i]>slots[boundary]&&boundary<count-1)boundary++;
   }
   return result;
  }
 }
}
````

## InputTranspositionPitch

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionPitch.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionPitchResult {public float Pitch,Stem;public short? AccidentalMarker;}
 // Main TR mode (argument word zero), original 3195:03E0..045C,
 // 05BC..0757. Analysis/negative modes use distinct original branches.
 public static class InputTranspositionPitch {
  static readonly short[] clefs={0,-12,-6,-8,-7};
  static readonly short[] encodings={4,1,0,2,5};
  static readonly short[,] keys={
   {-1,-1,-1,-1,-1,-1,-1},{-1,-1,-1,0,-1,-1,-1},{0,-1,-1,0,-1,-1,-1},
   {0,-1,-1,0,0,-1,-1},{0,0,-1,0,0,-1,-1},{0,0,-1,0,0,0,-1},{0,0,0,0,0,0,-1},
   {0,0,0,0,0,0,0},{0,0,0,1,0,0,0},{1,0,0,1,0,0,0},{1,0,0,1,1,0,0},
   {1,1,0,1,1,0,0},{1,1,0,1,1,1,0},{1,1,1,1,1,1,0},{1,1,1,1,1,1,1}};
  static short W(int value){return unchecked((short)value);}
  static short Word(float value){long result;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out result))throw new NotSupportedException("Non-finite transposition word.");return unchecked((short)result);}
  static short Degree(short value){short degree=W(W(value+6)%7+1);if(degree<=0)degree=W(degree+7);return degree;}
  public static TranspositionPitchResult Apply(float originalPitch,float originalStem,short collectedPitch,short accidental,short clef,short oldKey,short newKey,short diatonic,short keyOverride){
   if(clef<0||clef>=clefs.Length||oldKey< -7||oldKey>7||newKey< -7||newKey>7||accidental< -2||accidental>3)throw new NotSupportedException("Pitch transform outside recovered table domain.");
   short absolute=W(collectedPitch+clefs[clef]),degree=Degree(absolute),next=Degree(W(diatonic+degree));
   short target=keys[newKey+7,next-1],pitch=collectedPitch;var result=new TranspositionPitchResult{Stem=originalStem};
   if(accidental!=0||keyOverride==0){
    short encoded;
    if(accidental!=0){
     result.AccidentalMarker=W(absolute+100);short explicitValue=accidental==3?(short)0:accidental;
     target=W(target+W(explicitValue-keys[oldKey+7,degree-1]));
     if(target==0)encoded=3;
     else{if(target< -2){pitch=W(pitch+1);target=-2;}if(target>2){pitch=W(pitch-1);target=2;}encoded=encodings[target+2];}
    }else encoded=encodings[target+2];
    result.Stem=W(W(Word(originalStem)/10*10)+encoded);
   }
   short finalPitch=W(pitch+diatonic);
   if(Math.Abs(originalPitch)>=100)finalPitch=W(finalPitch+(finalPitch<0?-100:100));
   result.Pitch=finalPitch;return result;
  }
 }
}
````

## InputTranspositionRelationshipWindow

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionRelationshipWindow.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct TranspositionRelationshipWindow {public float StartLow,StartHigh,EndLow,EndHigh;}
 // Original 3195:10D8..1179. FST leaves the extended intermediate on
 // the x87 stack: the high endpoint is not calculated from the rounded low.
 public static class InputTranspositionRelationshipWindow {
  public static void Advance(IList<float[]> rows,IList<int> itemNumbers,ref short activeCount,ref short index,ref TranspositionRelationshipWindow window,float margin,float width){
   if(index>=activeCount){activeCount=0;return;}
   if(index<0||activeCount>itemNumbers.Count)throw new NotSupportedException("Relationship cursor outside recorded table.");
   index=unchecked((short)(index+1));int item=itemNumbers[index-1]-1;
   if(item<0||item>=rows.Count||rows[item].Length<6)throw new NotSupportedException("Relationship points outside its parameter record.");
   var row=rows[item];double end=(double)row[5]-margin,start=(double)row[2]-margin;
   window.EndLow=(float)end;window.EndHigh=(float)(end+width);
   window.StartLow=(float)start;window.StartHigh=(float)(start+width);
  }
 }
}
````

## InputUniformBeam

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputUniformBeam.cs)

````csharp
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
````

## LayoutBasicTablature

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutBasicTablature.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class LayoutBasicTablature {
  //2C0C:0377..051D staff/leading bar;0539..05C8 head, clef and bar-helper controls.
  //0C5F..0CB9 copies records;0B59..0BE9 clears bar tails and sets rest P6=-1.
  //182E..1895 creates rhythm rests;14AF..153C/1898..1A98 create ordinary rhythm notes.
  static short Word(float value){long word;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word))throw new NotSupportedException("Non-finite TAB rhythm attachment.");return unchecked((short)word);}
  //2C0C:0AB1..0B13 is shared by hairpins and retained ordinary slurs.
  static float[] MainSpan(float[] note,int destination,int strings){var span=new float[13];Array.Copy(note,span,Math.Min(note.Length,8));span[1]=destination;span[3]=span[4]=14.5f-2*(6-strings);float distance=(float)((double)note[5]-note[2]-10);span[6]=(float)((double)(distance>0?(float)((double)distance/100):0)+1);span[7]=-1;span[12]=.5f;return span;}
  sealed class TabResultList:List<float[]>{readonly Action<float[],int> write;int words;internal TabResultList(Action<float[],int> callback){write=callback;}public new void Add(float[] row){if(write!=null)write(row,words);base.Add(row);words+=row.Length+1;}}
  //2C0C:0C5F..0C93 copies twenty16-bit words: header and P1..P9.
  //Longer bodies retain destination RN bytes; the caller must establish them.
  static float[] CopyRetained(float[] note,List<float[]> result,Func<float[],int,float[]> copy){if(copy==null)throw new NotSupportedException("Original TAB trailing destination storage is unavailable.");int words=0;foreach(var row in result)words+=row.Length+1;return copy(note,words);}
  public static List<float[]> Build(float[] staff,IList<float[]> notes,int destination,bool clef,bool rhythmLine,short[] packedTuning,int strings,bool bass,IList<short> frequencies,bool hasCollection){return Build(staff,notes,destination,clef,rhythmLine,packedTuning,strings,bass,frequencies,hasCollection,null,-2);}
  public static List<float[]> Build(float[] staff,IList<float[]> notes,int destination,bool clef,bool rhythmLine,short[] packedTuning,int strings,bool bass,IList<short> frequencies,bool hasCollection,short[] recoveredAccidentals,int firstDelta,Func<float[],int,float[]> retainedCopy=null,Action<float[],int> writeRecord=null){
   if(!hasCollection)return new List<float[]>();
   float size=staff[4]==0?1:staff[4];const float factor=1.399999976158142f;
   int head=strings==6?(bass?16:6):strings==7?7:strings==5&&bass?15:14;
   float tabClef=strings==6||strings==7?66:strings==5&&bass?55:44,barControl=strings==6?13:strings==7?15:strings==5&&bass?0:9;
   var result=new TabResultList(writeRecord){new[]{8f,(float)destination,staff[2],(float)((double)staff[3]/factor-strings+1),(float)((double)size*factor),staff.Length>=6?staff[5]:200f,(float)strings},new[]{14f,(float)destination,staff[2],2f}};
   var convertedNotes=new List<LayoutTablatureChordNote>();var rhythmSlurs=new HashSet<float[]>();
   if(clef)result.Add(new[]{3f,(float)destination,(float)((double)staff[2]+2),0f,tabClef});
   float previousMainSlurX=0,previousMainSlurEnd=0;int sourceIndex=0;int previousRhythmX=0;foreach(var note in notes){int frequency=frequencies[sourceIndex++];
    if(note[0]==6)continue;float[] copied=null;if(retainedCopy!=null){int words=0;foreach(var row in result)words+=row.Length+1;copied=retainedCopy(note,words);}
    if(note[0]==5){
     //2C0C:0910..0B13: P9>=98 retains the copied body and its rhythm index.
     if(note.Length>=9&&note[8]>=98){var retained=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):(float[])note.Clone();retained[1]=destination;result.Add(retained);rhythmSlurs.Add(note);continue;}
     float dx=(float)((double)note[2]-previousMainSlurX),de=(float)((double)note[5]-previousMainSlurEnd);previousMainSlurX=note[2];previousMainSlurEnd=note[5];short control=note.Length==7?(copied!=null&&copied.Length>7?Word(copied[7]):(short)1):Word(note[7]);
     if(control>0||Math.Abs(dx)<=3&&Math.Abs(de)<=3)continue;rhythmSlurs.Add(note);if(control/2+1==0)continue;
     result.Add(MainSpan(note,destination,strings));continue;}
    if(note[0]==4&&note[4]==999){result.Add(MainSpan(note,destination,strings));continue;}
    if(note[0]==4||note[0]==18){var body=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):(float[])note.Clone();body[1]=destination;result.Add(body);continue;}
    if(note[0]==2){var rest=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):(float[])note.Clone();rest[1]=destination;rest[5]=-1;result.Add(rest);continue;}
    if(note[0]==14){var bar=new float[11];var source=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):note;Array.Copy(source,bar,Math.Min(source.Length,10));bar[1]=destination;bar[10]=barControl;result.Add(bar);continue;}
    float accidental=(float)((double)note[4]%10);if(accidental==0&&frequency<=0){for(int prior=convertedNotes.Count-1;prior>=0;prior--)if((int)convertedNotes[prior].Row[3]%100==(int)note[3]%100){accidental=convertedNotes[prior].Row[4];break;}}if(accidental==0){int natural=2*(((int)note[3]%100-(bass?5:0)+27)%7);if(natural>5)natural--;int chromatic=frequency%12;if(natural==0&&chromatic==11)natural=12;if(natural==11&&chromatic==0)chromatic=12;int delta=chromatic-natural;if(recoveredAccidentals==null){if(delta< -2||delta>2)throw new NotSupportedException("TAB accidental table read rejected by the selected safe policy.");int[] encodings={4,1,3,2,5};accidental=encodings[delta+2];}else{int at=delta-firstDelta;if(at<0||at>=recoveredAccidentals.Length)throw new NotSupportedException("TAB accidental lookup leaves the recovered signed-remainder domain.");accidental=recoveredAccidentals[at];}}
    var plan=LayoutTablaturePitch.Prepare(head,(int)note[3]%100,Word(accidental)%10,packedTuning);
    //The original scans the stored tuning order even when adjacent pitches cross.
    // Original single-note scan retains the last string when all tunings exceed the pitch.
    int chosen=plan.Strings;for(int n=1;n<=plan.Strings;n++)if(plan.Tuning[n]<=plan.Absolute){chosen=n;break;}
    //2C0C:089C..08EC compares source P6 with literal6 and writes P9=-1.
    int count=note[5]==6?9:8;for(int pair=0;pair<3;pair++)if(packedTuning[pair]!=0)count=16+pair;
    var converted=new float[count];Array.Copy(new[]{1f,(float)destination,note[2],note[3],(float)accidental,(float)-head,note[6],frequency<=0?-1f:(float)chosen},converted,8);
    for(int pair=0;pair<3;pair++)if(15+pair<count)converted[15+pair]=packedTuning[pair];if(note[5]==6)converted[8]=-1;result.Add(converted);convertedNotes.Add(new LayoutTablatureChordNote{Row=converted,Plan=plan,SortKey=frequency});
    //0895/0898 writes both coordinate words to shared B960. The rhythm
    //phase189F reads this same word pair; it is not reset between phases.
    previousRhythmX=BitConverter.ToInt32(BitConverter.GetBytes(note[2]),0);
   }
   LayoutTablatureChord.Apply(convertedNotes,strings);
   //189F..18AB compares both words of the carried main-note coordinate.
   if(rhythmLine)foreach(var note in notes){
    if(note[0]==14||note[0]==4||note[0]==18)continue;
    //2C0C:0943..0B13 suppresses nearby repeated slur spans;15BC..16BE creates the rhythm copy.
    if(note[0]==5){if(!rhythmSlurs.Contains(note))continue;var slur=new float[13];Array.Copy(note,slur,7);slur[1]=destination;slur[2]=(float)((double)note[2]-factor);slur[3]=slur[4]=1;slur[6]=-Math.Abs(note[6]);slur[7]=-2;slur[12]=.699999988079071f;result.Add(slur);continue;}
    if(note[0]==6){var beam=new float[18];Array.Copy(note,beam,Math.Min(note.Length,18));beam[1]=destination;beam[3]=beam[4]=2.0999999046325684f;if(beam[6]<20)beam[6]=(float)((double)beam[6]+10);beam[17]=.800000011920929f;result.Add(beam);continue;}
    //2C0C:182E..1895 retains P1..P7 and clears P8..P14 before rhythm size.
    if(note[0]==2){var rest=new float[15];Array.Copy(note,rest,Math.Min(note.Length,7));rest[1]=destination;rest[3]=-7;rest[14]=0.699999988079071f;result.Add(rest);continue;}
    int rhythmX=BitConverter.ToInt32(BitConverter.GetBytes(note[2]),0);if(rhythmX==previousRhythmX)continue;previousRhythmX=rhythmX;
    var rhythm=new[]{1f,(float)destination,note[2],0.8999999761581421f,20f,7f,-2f,-1.5f,0f,0f,0f,0f,0f,0f,0.800000011920929f,0f,-0.699999988079071f};if(note.Length>10)rhythm[10]=unchecked((short)-Word(Math.Abs(note[10])));if(note.Length>8)rhythm[8]=note[8];short sourceHead=Word(note[5]);if(sourceHead==1||sourceHead==2){rhythm[5]=9;Array.Resize(ref rhythm,15);}if(Math.Abs(note[3])>=100){rhythm[3]=-2.200000047683716f;rhythm[4]=10;rhythm[7]=100;rhythm[9]=-2.5f;rhythm[14]=.550000011920929f;Array.Resize(ref rhythm,15);}result.Add(rhythm);
   }
   return result;
  }
 }
}
````

## LayoutCoordinateAffine

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutCoordinateAffine.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original resident 11DA:0578..0667. The collector supplies signed
 // one-based float-word references. References remain reusable and ordered.
 public static class LayoutCoordinateAffine {
  public static void Apply(float[] words,short[] references,float first,float second,float third,short mode){
   if(mode==3)mode=-3;float offset=0;if(mode<0)offset=(float)((double)second-(double)first*third);
   foreach(short reference in references){int index=Math.Abs((int)reference)-1;if(index<0||index>=words.Length)throw new NotSupportedException("Original affine coordinate reference leaves its buffer.");words[index]=mode<0?(float)((double)words[index]*first+offset):(float)((double)words[index]+first);}
  }
 }
}
````

## LayoutCoordinateCollection

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutCoordinateCollection.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutCoordinateCollectionResult {
  public short[] References;public short FirstItem,LastItem;
 }
 // Main J's mode-zero path of original 11DA:000F/04C4. This is a word-reference
 // collector, not a geometric hit test. Nonzero collection modes are separate.
 public static class LayoutCoordinateCollection {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Original layout collector conversion leaves the verified signed-word domain.");return (short)Math.Truncate(f);}
  public static LayoutCoordinateCollectionResult Collect(float[] words,int[] itemStarts,short bottom,short top,float left,float right,float tolerance,short first,short last,short staffTableThreshold,float codeFilter,float emptyStaffControl,float compoundBarControl,int capacity,short mode=0) {
   if(mode!=0&&mode!=1&&mode!=2&&mode!=-1)throw new NotSupportedException("Original layout collection mode remains unverified.");
   if(words==null||itemStarts==null||first<1||Math.Abs((int)last)>itemStarts.Length||capacity<0)throw new NotSupportedException("Original layout collection would leave its buffers.");
   if(top<bottom)top=bottom;
   if(bottom==1&&staffTableThreshold<top)bottom=9999;
   short filter=Word(codeFilter);if(left>right){float t=left;left=right;right=t;}
   left=(float)((double)left-tolerance);right=(float)((double)right+tolerance);
   var references=new List<short>();short firstSelected=0;int limit=Math.Abs((int)last);
   Action<int> append=delegate(int value){if(references.Count>=capacity||value< -32768||value>32767)throw new NotSupportedException("Original layout coordinate-list capacity exceeded.");references.Add((short)value);};
   for(int i=first-1;i<limit;i++) {
    int w=itemStarts[i],start=w-1;if(start<0||start+3>=words.Length)throw new NotSupportedException("Original layout item leaves its buffer.");
    int count=Word(words[start]),code=Word(words[start+1]),staff=Word(words[start+2]);
    if(count<3||start+count>=words.Length)throw new NotSupportedException("Original short layout record requires its separate extension path.");
    if(bottom!=9999&&(staff<bottom||staff>top)) {if(staff>top&&last<0)last=(short)i;continue;}
    if(filter!=0&&filter!=code)continue;
    bool selected=left<=words[start+3]&&words[start+3]<=right;
    if(selected){append(w+3);if(firstSelected==0)firstSelected=(short)(i+1);}
    if(mode==1||mode>0&&!selected)continue;
    if(count<6||code<4||code>8)continue;
    // Original PAGE 227E:0219..024E addresses (one-based count index*4)+24-4: P6.
    // Staff size P5 is separate and must never receive the empty-end control.
    if(code==8&&Bits(words[start+6])==0)words[start+6]=emptyStaffControl;
    if(code==4&&count>=7&&Bits(words[start+7])<0&&Bits(words[start+6])==0)continue;
    Action<int> extra=delegate(int parameter){float x=words[start+parameter];if(left<=x&&x<=right){append(-(w+parameter));if(firstSelected==0)firstSelected=(short)(i+1);}};
    extra(6);if(count<8)continue;
    if(code==4) {
     if(words[start+7]!=compoundBarControl)continue;
     extra(8);for(int p=10;p<=count&&p<16;p+=2)extra(p);
    } else if(code==7) {
     if(Word(words[start+7])>=20&&Bits(words[start+8])>0)extra(8);
    } else if(code==6) {
     if(count<11)continue;
     if(Bits(words[start+11])>0)extra(11);
     if(Bits(words[start+7])<0)continue;
     if(count>=12&&Bits(words[start+12])>0)extra(12);
     if(count>=14&&Bits(words[start+13])!=0) {
      if(Bits(words[start+14])>0)extra(14);
      if(count>=15&&Bits(words[start+15])>0)extra(15);
     }
    }
   }
   return new LayoutCoordinateCollectionResult{References=references.ToArray(),FirstItem=firstSelected,LastItem=last};
  }
 }
}
````

## LayoutDecombineGroups

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutDecombineGroups.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Original 2D75:000D..015F, after DEC's staff sort. Values are group
 // sizes; omitted zero cells repeat the last nonzero size.
 public static class LayoutDecombineGroups {
  static short Word(float value){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out n))throw new NotSupportedException("Original DEC group size leaves finite conversion domain.");return unchecked((short)n);}
  public static short[] Boundaries(short maximum,float[] sizes,short lastSize){
   if(maximum<2||maximum>32||sizes==null||sizes.Length==0||sizes[0]==0)return new short[0];
   if(sizes[0]>=maximum)throw new NotSupportedException("Original DEC grouping at/above the staff maximum remains unverified.");
   var result=new List<short>();short total=0,last=lastSize;
   for(int i=0;i<32;i++){if(i<24){short value=Word(i<sizes.Length?sizes[i]:0);if(value!=0)last=value;}if(last<=0)throw new NotSupportedException("Original DEC nonpositive grouping traversal remains unverified.");result.Add(last);total=unchecked((short)(total+last));if(total>=maximum)break;}
   if(total<maximum)throw new NotSupportedException("Original DEC grouping exceeds its 32-cell table.");for(int i=1;i<result.Count;i++)result[i]=(short)Math.Min(32,result[i-1]+result[i]);return result.ToArray();
  }
 }
}
````

## LayoutDisappearingStaffs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutDisappearingStaffs.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 //2DF6:0ABD..0C05. The table has32 signed words, initially1.
 public sealed class LayoutDisappearingStaffs {
  readonly short[] words=new short[32];
  public LayoutDisappearingStaffs(){for(int i=0;i<words.Length;i++)words[i]=1;}
  public bool Hidden(int staff){return staff>=1&&staff<=32&&words[staff-1]<0;}
  public int[] HiddenStaffs(){var result=new List<int>();for(int i=1;i<=32;i++)if(Hidden(i))result.Add(i);return result.ToArray();}
  public void Apply(float[] values){
   if(values.Length==0)return;foreach(float value in values)if(Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("Non-finite DP argument.");if(values.Length>23)throw new NotSupportedException("DP arguments exceed the recovered23-word loop.");
   if(values[0]>32||BitConverter.ToInt32(BitConverter.GetBytes(values[0]),0)<0){short word=BitConverter.ToInt32(BitConverter.GetBytes(values[0]),0)<0?(short)-1:(short)1;for(int i=0;i<32;i++)words[i]=word;return;}
   if(values.Length>=3&&values[1]==0&&values[2]!=0){int first=Word(values[0]),last=Word(values[2]);if(first<1||last>32)throw new NotSupportedException("DP range outside the original staff table is prevented.");for(int i=first;i<=last;i++)words[i-1]=unchecked((short)-words[i-1]);return;}
   foreach(float value in values){int staff=Word(value);if(staff==0)break;if(staff<0)throw new NotSupportedException("DP list index outside the original staff table is prevented.");if(staff<=32)words[staff-1]=unchecked((short)-words[staff-1]);}
  }
  static short Word(float value){long word;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word))throw new NotSupportedException("Non-finite DP staff.");return unchecked((short)word);}
 }
}
````

## LayoutItemSpacing

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutItemSpacing.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2DCE:000A..1441. Explicit caller-owned buffers retain the original
 // history. Main command selection/undo and preference loading are not done here.
 public sealed class LayoutItemSpacing {
  readonly byte[] data;readonly float[] words,selected,aux,gaps,points;readonly short[] refs;readonly int[] starts;
  public LayoutItemSpacing(byte[] workspace,float[] scoreWords,int[] itemStarts,float[] selectedPositions,short[] selectedReferences,float[] auxiliary,float[] minimumGaps,float[] outputPositions){data=workspace;words=scoreWords;starts=itemStarts;selected=selectedPositions;refs=selectedReferences;aux=auxiliary;gaps=minimumGaps;points=outputPositions;}
  float F(int a){return BitConverter.ToSingle(data,a);}void F(int a,double f){Buffer.BlockCopy(BitConverter.GetBytes((float)f),0,data,a,4);}
  short W(int a){return BitConverter.ToInt16(data,a);}void W(int a,int v){Buffer.BlockCopy(BitConverter.GetBytes(unchecked((short)v)),0,data,a,2);}
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(f),0),out n))throw new NotSupportedException("Spacing conversion leaves its finite integer domain.");return unchecked((short)n);}
  float Raw(int word,int p){int i=word-1+p;if(i<0||i>=words.Length)throw new NotSupportedException("Spacing record word leaves its buffer.");return words[i];}
  static void Bound(int i,float[] a){if(i<0||i>=a.Length)throw new NotSupportedException("Original spacing table leaves its buffer.");}
  void Symbol(LayoutSymbolSpacingResult r){F(0xd4d4,r.Left);F(0xd4a6,r.Right);F(0xafac,r.Scratch);if(r.ResetParameter10Scratch)F(0xb9de,0);}
  public short Apply(short staff,float staffScale,float firstStaff,ref short cursor,float left,float right,float[] afterSymbol,bool originalQuarterToneWidths=true,float? positionJoinTolerance=null,bool standaloneJust=false) {return Apply(staff,staffScale,firstStaff,ref cursor,left,right,afterSymbol,originalQuarterToneWidths,positionJoinTolerance,standaloneJust,false);}
  public short Apply(short staff,float staffScale,float firstStaff,ref short cursor,float left,float right,float[] afterSymbol,bool originalQuarterToneWidths,float? positionJoinTolerance,bool standaloneJust,bool scor4Numbers) {
   if(afterSymbol==null||afterSymbol.Length!=4)throw new ArgumentException("Explicit AFTC/AFTB/AFTK/AFTM values are required.");
   if(standaloneJust&&originalQuarterToneWidths){
    // The old JUST.EXE adjacent descriptors differ from SCORE.EXE's.
    Buffer.BlockCopy(BitConverter.GetBytes(0x00001a94u),0,data,0x3886,4);
    Buffer.BlockCopy(BitConverter.GetBytes(0x4c2d1a94u),0,data,0x388e,4);
   }
   W(0xd4de,0);W(0xd484,0);F(0xd4e4,(double)left-F(0x7748));F(0xd4e0,(double)right+F(0x7748));W(0xd4f6,Word(firstStaff));W(0xd4f8,staff);
   var choice=LayoutSpacingSelection.Collect(words,starts,staff,W(0xd4f6),cursor,left,right,F(0x7748),F(0x774c),F(0x7754),F(0x7758),F(0x775e),F(0x7750),selected.Length,scor4Numbers);
   W(0xd484,choice.Positions.Length);Array.Copy(choice.Positions,selected,choice.Positions.Length);Array.Copy(choice.RecordWords,refs,choice.RecordWords.Length);
   if(choice.Positions.Length<2)return 0;
   LayoutSpacingSelection.Sort(selected,refs,W(0xd484));F(0xd4d8,(double)staffScale*F(0x7762));points[0]=F(0xd4e4);gaps[0]=0;short count=1;F(0xd492,0);W(0xd4dc,0);W(0xd48a,0);
   var note=new LayoutNoteSpacingSpan(data,words,refs,selected,originalQuarterToneWidths,standaloneJust?F(0xd4d8):1);
   for(int iteration=0;iteration<1000;iteration++) {
    W(0xd48a,W(0xd48a)+1);int i=W(0xd48a);if(i<1||i>W(0xd484))throw new NotSupportedException("Original spacing traversal leaves its selection.");
    int w=refs[i-1];W(0xd48c,w);W(0xd4ce,Word(Raw(w,1)));note.LoadRecord(w,0xd4f4);F(0xd4a6,0);F(0xd4d4,0);F(0xd48e,F(0x774c));F(0xd4b0,(double)F(0xb9c6)%F(0x7766));
    int kind=W(0xd4ce),previous=W(0xd4de);bool append=true;
    if(kind<2)append=note.Apply();
    else {
     W(0xd4dc,0);
     if(kind==14){F(0xb9d6,F(0x774c));if(W(0xd4f4)>=17)F(0xb9d6,(double)Raw(w,17)/F(0xd4d8));Symbol(standaloneJust?LayoutSymbolSpacing.JustBar((short)previous,W(0xd4f4),F(0xb9ca)):LayoutSymbolSpacing.Bar((short)previous,W(0xd4f4),F(0xb9ca),F(0xb9d2),F(0xb9d6),F(0xb9ea),F(0xb9ee)));}
     else if(kind==2){int n=W(0xd4f4);var r=LayoutRestSpacing.Apply(F(0xd48e),F(0xb9ce),F(0xb9ca),F(0xb9c6),F(0xd4d4),(short)n,n>=9?Raw(w,9):0,n>=10?Raw(w,10):0,n>=15?Raw(w,15):0);F(0xd48e,r.Multiplier);F(0xd4a6,r.Right);F(0xd4d4,r.Left);if(r.Parameter10Scratch.HasValue)F(0xafac,r.Parameter10Scratch.Value);}
     else if(kind==3)Clef(i,w,previous,count);
     else if(kind==4){F(0xd4d4,F(0x7796));if(previous==14)F(0xd4d4,F(0x77aa));if(previous==3||previous>16)F(0xd4d4,0);F(0xd4a6,F(0x779a));if(Raw(w,0)>=F(0x77ae)){F(0xd4c0,(double)Raw(w,15)/F(0xd4d8));F(0xd4d4,(double)F(0xd4d4)-F(0xd4c0));F(0xd4a6,(double)F(0xd4c0)+F(0x779a));}}
     else if(kind==10&&scor4Numbers){var r=ScoreVersionControls.NumberSpacing(Raw(w,6),Raw(w,15),F(0xd4d8));Symbol(r);}
     else if(kind==17){var r=LayoutSymbolSpacing.Key((short)previous,F(0xb9d2),F(0xb9ca),standaloneJust);Symbol(r);F(0xafb0,r.ScaleScratch);}
     else {int n=W(0xd4f4);var window=new float[words.Length-w];Array.Copy(words,w,window,0,window.Length);var r=LayoutSymbolSpacing.Meter((short)previous,gaps[count-1],window,(short)n,standaloneJust);Symbol(r);gaps[count-1]=r.Minimum;if(n>=8)F(0xafb0,r.ScaleScratch);if(W(0xd48a)==W(0xd484))F(0xd4a6,(double)F(0xd4a6)-F(0x77b2));}
    }
    if(append) {
     if(Math.Abs((double)F(0xb9c2)-points[count-1])>=(positionJoinTolerance??F(0x77b6))){count++;Bound(count-1,points);Bound(count-1,gaps);Bound(count-1,aux);points[count-1]=F(0xb9c2);gaps[count-1]=0;aux[count-1]=0;}
     if(aux[count-1]<F(0xd4d4)||kind==18)aux[count-1]=F(0xd4d4);if(gaps[count-1]<F(0xd4a6))gaps[count-1]=F(0xd4a6);
     if(kind<=2||kind==4) {
      F(0xd4b6,0);
      if(previous==14)F(0xd4b6,(double)afterSymbol[1]/F(0xd4d8)-F(0x77b2));
      if(previous==17)F(0xd4b6,(double)afterSymbol[2]/F(0xd4d8)-F(0x77bc));
      if(previous==18)F(0xd4b6,(double)afterSymbol[3]/F(0xd4d8)-F(0x77c0));
      if(previous==3&&W(0xd482)==0)F(0xd4b6,(double)afterSymbol[0]/F(0xd4d8)-F(0x779a));
      if(Bits(F(0xd4b6))>0)aux[count-1]=(float)((double)F(0xd4b6)+aux[count-1]);
     }
     W(0xd4de,kind);
    }
    if(W(0xd48a)>=W(0xd484))break;
    if(iteration==999)throw new NotSupportedException("Original spacing traversal exceeded its table.");
   }
   cursor=W(0xd48a);
   if(count!=1){Bound(count,aux);aux[count]=0;for(int i=0;i<count;i++)gaps[i]=(float)(((double)gaps[i]+aux[i+1])*F(0xd4d8));W(0xd48a,count+1);}
   return count;
  }
  void Clef(int i,int w,int previous,short count) {
   F(0xd4a6,F(0x779e));W(0xd482,0);if(F(0xb9c6)>=F(0x7766)){F(0xd4a6,F(0x77a2));W(0xd482,1);}
   bool preceding=previous==14;
   if(!preceding){if(previous==1)F(0xd4d4,F(0x774c));if(i==W(0xd484))return;preceding=count==1;}
   if(preceding){F(0xd4c0,gaps[count-1]);if(F(0xd4c0)>=F(0x77a6))F(0xd4c0,(double)F(0xd4c0)-F(0x774c));if(F(0xd4c0)<F(0x77a6))F(0xd4c0,F(0x7750));gaps[count-1]=F(0xd4c0);return;}
   Bound(i,selected);Bound(i-2,selected);double next=selected[i];
   if((double)F(0xd4a6)*F(0xd4d8)>next-F(0xb9c2))return;
   F(0xd4c0,gaps[count-1]);if((double)F(0xb9c2)-selected[i-2]>=F(0xd4c0))return;
   F(0xd4c0,(double)selected[i-2]+F(0xd4c0));if(next-F(0x77aa)<=F(0xd4c0))return;
   F(0xb9c2,F(0xd4c0));words[w+2]=F(0xd4c0);
  }
 }
}
````

## LayoutJustificationAllocation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationAllocation.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class JustificationAllocationResult {
  public short NextIndex;public float Allocated,Remaining;
 }
 // Decoded original 2F12:0493: one allocation step, seven far-pointer arguments.
 // Runtime verified at its natural caller/return and 14 isolated finite cases;
 // not yet integrated into Main J.
 // Separate binary32 stores are retained. Caller collection, ordering and
 // complete justification are separate routines; this helper does not guess
 // either the minimum gap or the relationship between score records.
 public static class LayoutJustificationAllocation {
  public static JustificationAllocationResult Apply(float[] positions,float[] allocations,short index,float target,float limit,float remaining,float minimum){
   if(positions==null||allocations==null||index<1||index>=positions.Length||index>allocations.Length)throw new NotSupportedException("Original justification allocation would leave its buffers.");
   float anchor=positions[index-1];
   float allocation=(float)(((double)target-anchor)*remaining/((double)limit-anchor));
   if(Single.IsNaN(allocation)||Single.IsInfinity(allocation))throw new NotSupportedException("Original justification allocation produced a non-finite value.");
   if(allocation<minimum)allocation=minimum;
   allocations[index-1]=allocation;
   remaining=(float)((double)remaining-allocation);
   short next=unchecked((short)(index+1));positions[next-1]=target;
   return new JustificationAllocationResult{NextIndex=next,Allocated=allocation,Remaining=remaining};
  }
 }
}
````

## LayoutJustificationCoordinates

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationCoordinates.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2F68:0447 and 0607. The signed list contains one-based float-word
 // references, NOT item numbers. Updates intentionally occur in list order.
 // Callers own collection and range selection; no renderer bounds are used.
 public sealed class JustificationCoordinates {
  readonly float[] words;readonly short[] references;
  readonly float ratioThreshold,ratioReplacement;
  readonly bool roundScaleNumerator;
  public float LowerBound;
  public JustificationCoordinates(float[] words,short[] references,float lowerBound,float ratioThreshold,float ratioReplacement)
   :this(words,references,lowerBound,ratioThreshold,ratioReplacement,false){}
  internal JustificationCoordinates(float[] words,short[] references,float lowerBound,float ratioThreshold,float ratioReplacement,bool roundScaleNumerator) {
   this.words=words;this.references=references;LowerBound=lowerBound;
   this.ratioThreshold=ratioThreshold;this.ratioReplacement=ratioReplacement;
   this.roundScaleNumerator=roundScaleNumerator;
   foreach(short reference in references)if(reference!=0&&(reference==Int16.MinValue||Math.Abs((int)reference)>words.Length))throw new NotSupportedException("Original justification coordinate reference leaves its buffer.");
  }
  // 0607: remaining references at/above the threshold, without consuming them.
  public void Shift(float threshold,float delta) {
   foreach(short reference in references)if(reference!=0) {
    int i=Math.Abs((int)reference)-1;float x=words[i];
    if(threshold<=x)words[i]=(float)((double)x+delta);
   }
  }
  // 0447: consume references inside the inclusive interval after scaling.
  public void Scale(float left,float right,float delta) {
   float span=(float)((double)right-left);
   if(BitConverter.ToInt32(BitConverter.GetBytes(span),0)==0)return; // original diagnostic then return
   // FST AFB0 retains the extended sum for the following division.
   double numerator=(double)span+delta;
   if(roundScaleNumerator)numerator=(float)numerator;
   double extendedRatio=numerator/span;
   float ratio=(float)extendedRatio;
   if((roundScaleNumerator?ratio:extendedRatio)<=ratioThreshold)ratio=ratioReplacement;
   for(int n=0;n<references.Length;n++)if(references[n]!=0) {
    int i=Math.Abs((int)references[n])-1;float x=words[i];
    if(x<left||x>right)continue;
    double value=((double)x-left)*ratio+left;
    if(LowerBound>value)value=LowerBound;
    words[i]=(float)value;references[n]=0;
   }
   LowerBound=(float)((double)left+ratioThreshold);
  }
 }
 public static class LayoutJustificationRedistribution {
  static bool NonZeroBits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0)!=0;}
  // Original 2F68:01D4..0339. Gap preparation is a preceding routine span;
  // final item-coordinate snapping is deliberately left to the record caller.
  public static bool Apply(float[] slack,float[] positions,short pointCount,float positiveSlack,float negativeSlack,JustificationCoordinates coordinates) {
   if(pointCount<0||pointCount>positions.Length||pointCount-1>slack.Length)throw new NotSupportedException("Original justification redistribution would leave its buffers.");
   if(!NonZeroBits(negativeSlack))return true;
   if(!NonZeroBits(positiveSlack))return false; // original emits diagnostic and skips redistribution
   float ratio=(float)((double)negativeSlack/positiveSlack);
   for(int i=1;i<pointCount;i++) {
    float delta=-slack[i-1];
    if(BitConverter.ToInt32(BitConverter.GetBytes(delta),0)<0)delta=(float)((double)delta*ratio);
    if(!NonZeroBits(delta))continue;
    if(delta<=0) {
     coordinates.Scale(positions[i-1],positions[i],delta);
     coordinates.Shift(positions[i],delta);
     for(int j=i;j<pointCount;j++)positions[j]=(float)((double)positions[j]+delta);
    } else {
     float saved=positions[i];coordinates.Shift(saved,delta);
     for(int j=i;j<pointCount;j++)positions[j]=(float)((double)positions[j]+delta);
     coordinates.Scale(positions[i-1],saved,delta);
    }
   }
   return true;
  }
 }
}
````

## LayoutJustificationGaps

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationGaps.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class JustificationGapResult {
  public float MinimumTotal, Width, DeficitPerGap, PositiveSlack, NegativeSlack;
  public bool CompressedMinimums;
 }
 // Original 2F68:000D..01D3. This prepares gaps; it neither selects records
 // nor applies coordinates. Array cells correspond to the original far-pointer
 // arguments at BP+06 (slack), +0A (minimum), and +0E (positions).
 public static class LayoutJustificationGaps {
  static bool SignBit(float value) { return BitConverter.ToInt32(BitConverter.GetBytes(value),0)<0; }
  public static JustificationGapResult Prepare(float[] slack,float[] minimum,float[] positions,short pointCount,float left,float right) {
   if(slack==null||minimum==null||positions==null||pointCount<0||pointCount>positions.Length||pointCount-1>minimum.Length||pointCount-1>slack.Length)
    throw new NotSupportedException("Original justification gap preparation would leave its buffers.");
   // The original signed loop bounds also accept zero points: no gap
   // iterations or coordinate writes occur (measured by a staff-only J).
   int gaps=pointCount-1;float sum=0;
   for(int i=0;i<gaps;i++)sum=(float)((double)sum+minimum[i]);
   double extendedWidth=(double)right-left;float width=(float)extendedWidth;
   bool compressed=extendedWidth<sum;float deficit=0;
   if(compressed) {
    if(gaps==0)throw new NotSupportedException("Original justification gap preparation divides by zero.");
    deficit=(float)(((double)sum-width)/gaps);
    for(int i=0;i<gaps;i++)minimum[i]=(float)((double)minimum[i]-deficit);
   }
   for(int i=0;i<gaps;i++)slack[i]=(float)(((double)positions[i+1]-positions[i])-minimum[i]);
   float positive=0,negative=0;
   for(int i=0;i<gaps;i++) {
    if(SignBit(slack[i]))negative=(float)((double)negative-slack[i]);
    else positive=(float)((double)positive+slack[i]);
   }
   return new JustificationGapResult{MinimumTotal=sum,Width=width,DeficitPerGap=deficit,CompressedMinimums=compressed,PositiveSlack=positive,NegativeSlack=negative};
  }
 }
}
````

## LayoutJustificationMerge

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationMerge.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2F12:00C2..0300 and 0368..040F. The metric/spacing caller supplies
 // new tables; this routine merges them in original order with float32 stores.
 public static class LayoutJustificationMerge {
  static void Bound(int index,float[] a){if(index<0||index>=a.Length)throw new NotSupportedException("Original justification merge would leave its tables.");}
  public static short Append(float[] oldPositions,float[] oldMinimum,float[] newPositions,float[] newMinimum,float[] mergedPositions,float[] mergedMinimum,short oldCount,short newCount,float right,float rightThreshold,float epsilon) {
   if(newCount==0)return oldCount;
   if(oldCount<1||newCount<1)throw new NotSupportedException("Invalid original justification table count.");
   Bound(newCount-1,newPositions);
   if((double)right-newPositions[newCount-1]>epsilon){Bound(newCount,newPositions);newPositions[newCount++]=right;}
   if(oldCount==1){Array.Copy(newPositions,oldPositions,newCount);Array.Copy(newMinimum,oldMinimum,newCount);return newCount;}
   int nextNew=1,nextOld=1;short output=1;
   for(int steps=0;steps<400;steps++) {
    Bound(nextNew,newPositions);Bound(nextOld,oldPositions);Bound(nextNew-1,newMinimum);Bound(nextOld-1,oldMinimum);
    float n=newPositions[nextNew],o=oldPositions[nextOld];
    if(Math.Abs((double)n-o)<=epsilon) {
     float position=o<n?o:n;float gap=oldMinimum[nextOld-1];if(gap<newMinimum[nextNew-1])gap=newMinimum[nextNew-1];
     Bound(output-1,mergedMinimum);mergedMinimum[output-1]=gap;output++;Bound(output-1,mergedPositions);mergedPositions[output-1]=position;
     if(rightThreshold<=position){Array.Copy(mergedPositions,oldPositions,output);Array.Copy(mergedMinimum,oldMinimum,output);return output;}
     nextOld++;nextNew++;
    } else if(n<o) {
     var r=LayoutJustificationAllocation.Apply(mergedPositions,mergedMinimum,output,n,o,oldMinimum[nextOld-1],newMinimum[nextNew-1]);
     output=r.NextIndex;oldMinimum[nextOld-1]=r.Remaining;nextNew++;
    } else {
     var r=LayoutJustificationAllocation.Apply(mergedPositions,mergedMinimum,output,o,n,newMinimum[nextNew-1],oldMinimum[nextOld-1]);
     output=r.NextIndex;newMinimum[nextNew-1]=r.Remaining;nextOld++;
    }
   }
   throw new NotSupportedException("Original justification merge exceeded its finite table traversal.");
  }
 }
}
````

## LayoutJustificationSnap

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationSnap.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2F68:033A..03DA. The two tests use the same originally loaded P3,
 // even if the first test has already stored zero. All indexed items are visited,
 // including those outside the command's selected system/range.
 public static class LayoutJustificationSnap {
  public static void Apply(float[] words,int[] itemStarts,float tolerance,float rightEdge) {
   if(words==null||itemStarts==null)throw new ArgumentNullException();
   foreach(int word in itemStarts) {
    int header=word-1;
    if(header<0||header>=words.Length)throw new NotSupportedException("Original coordinate snap leaves its record bank.");
    long count;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(words[header]),0),out count))throw new NotSupportedException("Original coordinate snap count leaves its finite domain.");
    if(unchecked((short)count)<3)continue;
    int p3=header+3;if(p3>=words.Length)throw new NotSupportedException("Original coordinate snap leaves its record bank.");
    float x=words[p3];
    if(Math.Abs((double)x)<tolerance)words[p3]=0;
    if(Math.Abs((double)rightEdge-x)<tolerance)words[p3]=rightEdge;
   }
  }
 }
}
````

## LayoutJustificationSystem

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationSystem.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutJustificationWorkspace {
  public readonly byte[] Spacing=LayoutSpacingWorkspace.Create();
  public bool OriginalQuarterToneWidths=true;
  public float? PositionJoinTolerance;
  public bool StandaloneJust;
  public bool Scor4NumberJustification;
  public bool Page4RoundScaleNumerator;
  public readonly float[] Selected=new float[200],Auxiliary=new float[200],Minimum=new float[200],Positions=new float[200],OldMinimum=new float[200],OldPositions=new float[200],MergedPositions=new float[200],MergedMinimum=new float[200];
  public readonly short[] SelectedReferences=new short[200];
  public void ResetForIsolatedTest(){Array.Clear(Spacing,0,Spacing.Length);Buffer.BlockCopy(LayoutSpacingWorkspace.Create(),0,Spacing,0,Spacing.Length);foreach(var table in new[]{Selected,Auxiliary,Minimum,Positions,OldMinimum,OldPositions,MergedPositions,MergedMinimum})Array.Clear(table,0,table.Length);Array.Clear(SelectedReferences,0,SelectedReferences.Length);}
 }
 public sealed class LayoutJustificationSystemResult {
  public short[] CoordinateReferences;public short PointCount,FirstPoint,SelectionCursor;
  public JustificationGapResult Gaps;public bool RedistributionApplied;
 }
 // Original 2F12:0003..0490 orchestration. Caller sorting, record preparation,
 // prompts, redraw and undo remain separate. Staff scales and collector limits
 // are explicit recovered caller inputs, never inferred from renderer bounds.
 public static class LayoutJustificationSystem {
  public static LayoutJustificationSystemResult Apply(float[] words,int[] starts,short bottom,short top,float left,float right,float[] scales,float[] afterSymbol,short staffTableThreshold,short coordinateCapacity,LayoutJustificationWorkspace work) {
   if(work==null||scales==null||bottom<0||top<bottom||top>=scales.Length)throw new NotSupportedException("Original justification staff traversal leaves its tables.");
   var collection=LayoutCoordinateCollection.Collect(words,starts,bottom,top,left,right,0.001f,1,checked((short)starts.Length),staffTableThreshold,0,200,100,coordinateCapacity);
   work.MergedPositions[0]=left;work.MergedMinimum[0]=0;short oldCount=1,cursor=1;
   var spacing=new LayoutItemSpacing(work.Spacing,words,starts,work.Selected,work.SelectedReferences,work.Auxiliary,work.Minimum,work.Positions);
   float threshold=(float)((double)right-0.2f);
   for(int staff=bottom;staff<=top;staff++) {
    short count=spacing.Apply((short)staff,scales[staff],bottom,ref cursor,left,right,afterSymbol,work.OriginalQuarterToneWidths,work.PositionJoinTolerance,work.StandaloneJust,work.Scor4NumberJustification);
    if(count!=0)oldCount=LayoutJustificationMerge.Append(work.OldPositions,work.OldMinimum,work.Positions,work.Minimum,work.MergedPositions,work.MergedMinimum,oldCount,count,right,threshold,0.1f);
   }
   int first=0;if((double)work.OldPositions[1]-work.OldPositions[0]<=0.1f){first=1;oldCount--;}
   if(oldCount<0||first+oldCount>work.OldPositions.Length)throw new NotSupportedException("Original justification has no finite coordinate intervals.");
   var positions=new float[oldCount];var minimum=new float[oldCount];var slack=new float[oldCount];Array.Copy(work.OldPositions,first,positions,0,oldCount);Array.Copy(work.OldMinimum,first,minimum,0,oldCount);
   var gaps=LayoutJustificationGaps.Prepare(slack,minimum,positions,oldCount,left,right);
   var coordinates=new JustificationCoordinates(words,collection.References,-9999,0.2f,1,work.Page4RoundScaleNumerator);
   bool applied=LayoutJustificationRedistribution.Apply(slack,positions,oldCount,gaps.PositiveSlack,gaps.NegativeSlack,coordinates);
   if(!work.StandaloneJust&&BitConverter.ToInt32(BitConverter.GetBytes(gaps.NegativeSlack),0)!=0&&BitConverter.ToInt32(BitConverter.GetBytes(gaps.PositiveSlack),0)!=0)LayoutJustificationSnap.Apply(words,starts,0.01f,200);
   Array.Copy(positions,0,work.OldPositions,first,oldCount);Array.Copy(minimum,0,work.OldMinimum,first,oldCount);Array.Copy(slack,work.Selected,slack.Length);
   return new LayoutJustificationSystemResult{CoordinateReferences=collection.References,PointCount=oldCount,FirstPoint=(short)(first+1),SelectionCursor=cursor,Gaps=gaps,RedistributionApplied=applied};
  }
 }
}
````

## LayoutLineupCleanup

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupCleanup.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Original 3111:0675 grace/coincidence cleanup, after source remapping.
 // Preserves record order and stored binary32 boundaries; caller owns undo.
 public static class LayoutLineupCleanup {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup cleanup leaves its finite word domain.");return (short)Math.Truncate(f);}
  public static void Apply(float[] words,int[] starts,short bottom,short top,float[] staffUnits){
   int steps=0;Action step=delegate{if(++steps>100000)throw new NotSupportedException("Original lineup cleanup exceeds its verified traversal domain.");};
   Func<int,int,int> at=delegate(int item,int parameter){if(item<0||item>=starts.Length)throw new NotSupportedException("Lineup cleanup item leaves its table.");int index=starts[item]-1;if(Word(words[index])<parameter||index+parameter>=words.Length)throw new NotSupportedException("Original short cleanup record is not verified.");return index+parameter;};
   Func<int,int,float> p=delegate(int item,int parameter){return words[at(item,parameter)];};
   var bars=new List<float>();
   for(int i=0;i<starts.Length;i++)if(bottom<=Word(p(i,2))&&Word(p(i,2))<=top&&p(i,1)==14){float x=p(i,3);bool duplicate=false;for(int j=bars.Count-1;j>=0;j--)if(Bits(x)==Bits(bars[j])||Math.Abs((double)x-bars[j])<=0.01f){words[at(i,3)]=bars[j];duplicate=true;break;}if(!duplicate)bars.Add(x);}
   int currentStaff=0,lastAnchor=-1,staffStart=0;
   for(int i=0;i<starts.Length;i++){step();int staff=Word(p(i,2)),code=Word(p(i,1));if(staff<bottom)continue;if(staff>top)return;
    if(staff!=currentStaff){lastAnchor=-1;staffStart=i;}currentStaff=staff;
    if(code==1&&p(i,7)>=64){
     if(i>0&&p(i-1,1)==14&&p(i-1,3)==p(i,3))continue;
     var items=new List<int>{i};var repeated=new List<bool>{false};var oldPositions=new List<float>{p(i,3)};float previousX=p(i,3),right=200;int nextItem=i,nextCode=0;
     while(true){step();nextItem=i+1;if(nextItem>=starts.Length)return;int nextStaff=Word(p(nextItem,2));
      if(nextStaff!=staff){right=200;break;}
      i=nextItem;nextCode=Word(p(i,1));
      if(nextCode==1){if(p(i,7)<64){right=p(i,3);break;}float x=p(i,3);bool same=Bits(x)==Bits(previousX)||Math.Abs((double)previousX-x)<=0.01f;if(same)words[at(i,3)]=previousX;else{oldPositions.Add(x);previousX=x;}items.Add(i);repeated.Add(same);continue;}
      if(nextCode<4||nextCode==14){right=p(i,3);break;}
      if(nextCode>=17){int count=Word(words[starts[i]-1]);bool excluded=count>=14&&Bits(p(i,14))!=0||count>=15&&Bits(p(i,15))!=0;if(!excluded){right=p(i,3);break;}}
     }
     float scale=(float)((double)staffUnits[staff]/25);
     if(nextCode==1&&p(nextItem,5)%10!=0)right=(float)((double)right-(double)scale*2.5f);
     float left=lastAnchor>=0?p(lastAnchor,3):0;foreach(float bar in bars)if(left<bar&&bar<right){left=bar;break;}
     float available=(float)((double)right-left),minimum=(float)((oldPositions.Count*4.0+2)*scale),ratio=1;if(available<minimum)ratio=(float)((double)available/minimum);
     float spacing=(float)((double)ratio*scale*4);if(spacing<=0.05f)throw new NotSupportedException("Original grace cleanup reports insufficient spacing; diagnostic continuation is not yet verified.");
     right=(float)((double)right-(oldPositions.Count+1.0)*spacing);var newPositions=new List<float>();
     for(int k=0;k<items.Count;k++){if(!repeated[k]){right=(float)((double)right+spacing);newPositions.Add(right);}words[at(items[k],3)]=right;}
     currentStaff=0;
     for(int k=staffStart;k<=i;k++){int attachment=Word(p(k,1));if(attachment>4&&attachment<7){float x=p(k,3),end=p(k,6);for(int j=0;j<oldPositions.Count;j++){if(Bits(oldPositions[j])==Bits(end))words[at(k,6)]=newPositions[j];else if(Bits(oldPositions[j])==Bits(x))words[at(k,3)]=newPositions[j];}}}
     if(staffStart<i)i--;
    }else if(code<4||code==14)lastAnchor=i;
    else if(code>16){int count=Word(words[starts[i]-1]);if(!(count>=14&&Bits(p(i,14))!=0||count>=15&&Bits(p(i,15))!=0))lastAnchor=i;}
   }
  }
 }
}
````

## LayoutLineupCoordinates

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupCoordinates.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 3111:0004..0300 coordinate-allocation projection. Numerical
 // power uses the same candidate projection as F2, checked at stored float
 // boundaries; this does not claim x87 ABI/exception/flag equivalence.
 public static class LayoutLineupCoordinates {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup coordinate conversion leaves the finite signed-word domain.");return (short)Math.Truncate(f);}
  public static void Apply(float[] merged,float[] positions,float[] weights,short count,float left,float right,float staffUnit,float exponent) {
   Apply(merged,positions,weights,count,left,right,staffUnit,exponent,false);
  }
  internal static void Apply(float[] merged,float[] positions,float[] weights,short count,float left,float right,float staffUnit,float exponent,bool roundEachPosition) {
   if(count<1||count>=positions.Length||count>weights.Length||count>merged.Length)throw new NotSupportedException("Original lineup coordinate table extent is invalid.");
   float positiveTotal=0,symbolTotal=0;
   for(int i=0;i<count;i++) {
    int kind=Word(merged[i]);float width;
    if(kind<0) {
     width=0;
     if(i!=count-1){width=3.5f;int next=Word(merged[i+1]);
      if(kind==-14||kind==-140){if(next==-15||next==-17)width=1.5f;if(next==-18)width=1;}
      else if(kind==-15){width=12;if(next< -16){width=8;if(next==-18)width=6;}}
      else if(kind==-18&&next==-14)width=4.6f;
      symbolTotal=(float)((double)symbolTotal+width);
     }
    } else {double power=Math.Pow(merged[i],exponent);width=(float)power;positiveTotal=(float)(power+positiveTotal);}
    weights[i]=width;
   }
   float symbolScale=(float)((double)staffUnit/25);
   if(positiveTotal==0)throw new NotSupportedException("Original lineup coordinate allocation divides by zero.");
   float noteScale=(float)(((double)right-left-(double)symbolTotal*symbolScale)/positiveTotal);
   double current=left;positions[0]=(float)current;
   for(int i=0;i<count;i++) {
    float scale=Bits(merged[i])>0?noteScale:symbolScale;
    weights[i]=(float)((double)weights[i]*scale);
    current+=weights[i];
    positions[i+1]=(float)current;
    if(roundEachPosition)current=positions[i+1];
   }
  }
 }
}
````

## LayoutLineupDurationGroups

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupDurationGroups.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupDurationGroupsResult {public short OutputCount,PriorIndex,CurrentIndex;public float PriorValue,CurrentValue,PriorSum,CurrentSum;}
 // Original 31FE:0CCD..0F34, with 0C02/0C2F/0C94. This aligns the
 // two duration streams after the caller's symbol and beat-union pass.
 public static class LayoutLineupDurationGroups {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  public static LayoutLineupDurationGroupsResult Apply(float[] current,float[] prior,float[] output,short firstCurrent,short priorCount,short initialOutputCount) {
   return new Engine(current,prior,output,firstCurrent,priorCount,initialOutputCount).Run();
  }
  sealed class Engine {
   float[] a,b,c;int ai,bi,bCount,ci,steps;float av,bv,asum,bsum;
   public Engine(float[] current,float[] prior,float[] output,int firstCurrent,int priorCount,int initialOutputCount){a=current;b=prior;c=output;ai=firstCurrent;bi=0;bCount=priorCount;ci=initialOutputCount;}
   void Step(){if(++steps>100000)throw new NotSupportedException("Original duration-group traversal exceeded its verified buffer domain.");}
   void Append(float value){Step();if(ci<0||ci>=c.Length)throw new NotSupportedException("Duration-group output leaves its buffer.");c[ci++]=value;}
   void Prepare(){Step();ai++;if(ai<1||ai>a.Length)throw new NotSupportedException("Current duration stream leaves its buffer.");av=a[ai-1];asum=(float)((double)av+asum);}
   void Adjust(bool one){while(true){Step();bi++;if(bi<1||bi>b.Length)throw new NotSupportedException("Prior duration stream leaves its buffer.");bv=b[bi-1];if(Bits(bv)>0)break;Append(bv);if(one)return;}bsum=(float)((double)bv+bsum);}
   public LayoutLineupDurationGroupsResult Run(){
    while(true){asum=bsum=0;Adjust(true);
     if(Bits(bv)>=0){Prepare();
      while(Math.Abs((double)bsum-asum)>0.01f){Step();
       if(Bits(bv)<Bits(av)){
        while(true){Append(bv);if(Math.Abs((double)bsum-asum)<0.01f)goto EndCycle;
         if(bsum<=asum){Adjust(false);if(av<=(double)bv+0.005f)goto CurrentRemainder;}
         if((double)bsum-0.005f<=asum)continue;
         Prepare();if((double)bv+0.005f<av)continue;break;
        }
        ai--;asum=(float)((double)asum-av);
CurrentRemainder:
        av=(float)(((double)asum-bsum)+bv);
       }else{
        while(true){
         while(true){Append(av);if(Math.Abs((double)bsum-asum)<0.01f)goto EndCycle;
          if(asum<=bsum){Prepare();if(bv<=(double)av+0.005f)goto PriorRemainder;}
          if((double)asum-0.005f<=bsum)continue;break;
         }
         Adjust(false);if((double)av+0.005f<bv)continue;break;
        }
        bi--;bsum=(float)((double)bsum-bv);
PriorRemainder:
        bv=(float)(((double)bsum-asum)+av);
       }
       asum=av;bsum=bv;
      }
      Append(bv);
     }
EndCycle:
     if(bCount<=bi)return new LayoutLineupDurationGroupsResult{OutputCount=(short)ci,PriorIndex=(short)bi,CurrentIndex=(short)ai,PriorValue=bv,CurrentValue=av,PriorSum=bsum,CurrentSum=asum};
    }
   }
  }
 }
}
````

## LayoutLineupPreparation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupPreparation.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupStaffStream {public short Staff;public float[] Rhythms,Positions;}
 public sealed class LayoutLineupBeatTotal {public short Staff;public float Total;}
 public sealed class LayoutLineupPreparationResult {public LayoutLineupBeatTotal[] BeatTotals;public short Bottom,Top,Count;public float Left,Right;public bool NeedsCleanup,UnequalBeats;public LayoutLineupStaffStream[] StaffStreams;}
 // Original 31FE:000B through the 3111:0004 call. The following source
 // remapping and cleanup are deliberately separate; this is not a Main route.
 public static class LayoutLineupPreparation {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup preparation conversion leaves its finite signed-word domain.");return (short)Math.Truncate(f);}
  public static LayoutLineupPreparationResult Apply(float[] words,int[] starts,short bottom,short top,float[] rhythms,float[] positions,float[] merged,float[] scratch,float[] union,float[] subdivisions) {
   return ApplyWithBeatPolicy(words,starts,bottom,top,rhythms,positions,merged,scratch,union,subdivisions,false);
  }
  public static LayoutLineupPreparationResult ApplyWithBeatPolicy(float[] words,int[] starts,short bottom,short top,float[] rhythms,float[] positions,float[] merged,float[] scratch,float[] union,float[] subdivisions,bool returnUnequalBeats,Action<int> acknowledgeNegativePosition=null,bool standaloneJust=false) {
   return ApplyWithRhythmScale(words,starts,bottom,top,rhythms,positions,merged,scratch,union,subdivisions,returnUnequalBeats,acknowledgeNegativePosition,standaloneJust,2f);
  }
  internal static LayoutLineupPreparationResult ApplyWithRhythmScale(float[] words,int[] starts,short bottom,short top,float[] rhythms,float[] positions,float[] merged,float[] scratch,float[] union,float[] subdivisions,bool returnUnequalBeats,Action<int> acknowledgeNegativePosition,bool standaloneJust,float rhythmScale) {
   int cursor=1,mergedCount=0;float left=9999,right=200,previousTotal=0;bool needsCleanup=false;var history=new List<LayoutLineupStaffStream>();var totals=new List<LayoutLineupBeatTotal>();
   for(int staff=bottom;staff<=top;staff++){
    var collected=LayoutRhythmicCollection.Collect(words,starts,staff,-1,(short)cursor,rhythms,positions,scratch,acknowledgeNegativePosition,standaloneJust);cursor=collected.NextItem;int count=collected.Count;needsCleanup|=collected.InvalidatedSort;totals.Add(new LayoutLineupBeatTotal{Staff=(short)staff,Total=collected.Total});
    if(collected.Total==0){if(staff==bottom)bottom++;if(top<bottom){bottom=-1;break;}continue;}
    while(count>0&&positions[count-1]>200)count--;if(count<1)throw new NotSupportedException("Original lineup trims beyond its rhythm table.");
    int lastTag=Word(rhythms[count-1]);
    if(lastTag==-140)rhythms[count-1]=-14;
    else if(lastTag!=-14){
     if(lastTag<0){for(int i=0;i<starts.Length;i++){int at=starts[i]-1,n=Word(words[at]);if(Word(words[at+2])==bottom&&Word(words[at+1])==8){if(n>5)right=words[at+6];break;}}if(Bits(right)==0)right=200;}
     if(count>=rhythms.Length||count>=positions.Length)throw new NotSupportedException("Original lineup end marker exceeds its table.");rhythms[count]=-14;positions[count++]=right;
    }
    int first=1;while(Bits(positions[first-1])<0){first++;if(count<=first)throw new NotSupportedException("Original lineup excludes all selected positions.");count--;}
    int kept=0;bool ordinarySeen=false;
    for(int i=0;i<count;i++){float value=rhythms[i];bool keep=true;
     if(Bits(value)>0){value=(float)((double)rhythmScale*value);ordinarySeen=true;}
     else if(value!=-14){if(value!=-3)ordinarySeen=true;else if(!ordinarySeen){value=-15;ordinarySeen=true;}else keep=false;}
     if(keep){rhythms[kept]=value;positions[kept++]=positions[i];}
    }
    count=kept;for(int i=count;i<150&&i<rhythms.Length;i++)rhythms[i]=0;
    var staffRhythms=new float[count];var staffPositions=new float[count];Array.Copy(rhythms,staffRhythms,count);Array.Copy(positions,staffPositions,count);history.Add(new LayoutLineupStaffStream{Staff=(short)staff,Rhythms=staffRhythms,Positions=staffPositions});
    if(staff==bottom){previousTotal=collected.Total;mergedCount=LayoutLineupTableCompact.Apply(rhythms,merged,(short)first,(short)(first+count-1),-14);for(int i=mergedCount;i<150&&i<merged.Length;i++)merged[i]=0;left=positions[first-1];right=positions[count-1];continue;}
    if(positions[first-1]<left)left=positions[first-1];if(rhythms[count-1]==-14&&right<positions[count-1])right=positions[count-1];
    if(Math.Abs((double)collected.Total-previousTotal)>0.005f){
     if(returnUnequalBeats)return new LayoutLineupPreparationResult{BeatTotals=totals.ToArray(),Bottom=bottom,Top=top,Count=(short)mergedCount,Left=left,Right=right,NeedsCleanup=needsCleanup,StaffStreams=history.ToArray(),UnequalBeats=true};
     throw new NotSupportedException("Original lineup reports unequal beat totals between staves.");
    }
    var combined=LayoutLineupStreamUnion.Apply(rhythms,(short)first,(short)count,merged,(short)mergedCount,union);
    mergedCount=LayoutLineupTableCompact.Apply(union,merged,1,combined.Count,-14);
    int positiveEnd=first-1;for(int i=first-1;i<count;i++)if(Bits(rhythms[i])>0)rhythms[positiveEnd++]=rhythms[i];
    if(positiveEnd>=rhythms.Length||mergedCount>=merged.Length)throw new NotSupportedException("Original lineup sentinel exceeds its table.");rhythms[positiveEnd]=0;merged[mergedCount]=0;
    var groups=LayoutLineupDurationGroups.Apply(rhythms,merged,subdivisions,(short)(first-1),(short)mergedCount,0);mergedCount=groups.OutputCount;Array.Copy(subdivisions,merged,mergedCount);
   }
   return new LayoutLineupPreparationResult{BeatTotals=totals.ToArray(),Bottom=bottom,Top=top,Count=(short)mergedCount,Left=left,Right=right,NeedsCleanup=needsCleanup,StaffStreams=history.ToArray()};
  }
 }
}
````

## LayoutLineupRemap

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupRemap.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Original 3060:000C remapping orchestration. This calls the independently
 // recovered staff-table alignment, coordinate collector and affine routine.
 public static class LayoutLineupRemap {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Lineup remapping leaves its finite signed-word domain.");return (short)Math.Truncate(f);}
  public static void Apply(float[] words,int[] starts,short bottom,short top,float[] merged,float[] newPositions,short count,float[] savedRhythms,float[] savedPositions,short[] staffStarts,float[] staffUnits,short staffThreshold,int capacity){
   int steps=0;Action step=delegate{if(++steps>100000)throw new NotSupportedException("Original lineup remapping exceeds its verified traversal domain.");};
   Func<int,int,int> pIndex=delegate(int i,int parameter){int at=starts[i]-1;if(at<0||at+parameter>=words.Length||Word(words[at])<parameter)throw new NotSupportedException("Original short lineup remapping record is not verified.");return at+parameter;};
   Func<int,int,float> p=delegate(int i,int n){return words[pIndex(i,n)];};
   if(count<1||count>merged.Length||count>newPositions.Length)throw new NotSupportedException("Original lineup destination table extent is invalid.");
   float upper=(float)((double)newPositions[count-1]+1001),clefThreshold=0;
   for(int i=0;i<starts.Length;i++){int staff=Word(p(i,2));if(Word(p(i,1))==8&&bottom<=staff&&staff<=top)words[pIndex(i,2)]=-p(i,2);}
   for(int i=0;i<starts.Length;i++){int staff=Word(p(i,2)),code=Word(p(i,1));if(staff<bottom||staff>top)continue;if(code<3){clefThreshold=(float)((double)p(i,3)-2);break;}if(code==3){clefThreshold=(float)((double)p(i,3)+2);break;}}
   for(int staff=bottom;staff<=top;staff++){
    if(staff<0||staff+1>=staffStarts.Length)throw new NotSupportedException("Original lineup staff table leaves its buffer.");if(staffStarts[staff]==staffStarts[staff+1])continue;
    float[] oldRhythms=new float[150],oldPositions=new float[150];var aligned=LayoutLineupStaffTables.Apply(merged,count,savedRhythms,savedPositions,staffStarts[staff],oldRhythms,oldPositions);
    int ni=aligned.FirstMerged,oi=1,oldCount=aligned.Count;if(ni<1||ni>count||oldCount<1)throw new NotSupportedException("Original lineup staff alignment has no remapping interval.");
    float startNew=newPositions[ni-1],newRhythm=merged[ni-1],threshold=(float)((double)startNew+800),oldRhythm=oldRhythms[0],startOld=oldPositions[0],nextNew=0,nextNewRhythm=0;
    bool pending=false;
    Action advance=delegate{step();ni++;if(ni<1||ni>newPositions.Length||ni>merged.Length)throw new NotSupportedException("Original lineup advances beyond its destination buffer.");nextNew=newPositions[ni-1];nextNewRhythm=merged[ni-1];};
    while(true){step();
     if(!pending){if(count<ni)break;advance();}
     if(oldCount<=oi)break;oi++;float nextOldRhythm=oldRhythms[oi-1],nextOld=oldPositions[oi-1],stopNew,remaining;
     if(Bits(newRhythm)==Bits(oldRhythm))goto EqualRhythms;
     remaining=(float)((double)newRhythm-oldRhythm);
     if(Math.Abs((double)remaining)<=0.002f)goto EqualRhythms;
     while(Bits(remaining)<0){
      while(Bits(nextNewRhythm)<=0)advance();newRhythm=(float)((double)nextNewRhythm+newRhythm);remaining=(float)((double)newRhythm-oldRhythm);advance();
      if(Math.Abs((double)remaining)<0.002f)goto EqualRhythms;
      if(Bits(nextNewRhythm)<=0)throw new NotSupportedException("Original lineup reports a rhythm-alignment error.");
     }
     pending=true;stopNew=(float)((((double)nextNew-startNew)*oldRhythm)/newRhythm+startNew);goto MapInterval;
EqualRhythms:
     pending=false;stopNew=nextNew;remaining=nextNewRhythm;
     if(nextNewRhythm!=nextOldRhythm){
      if(nextOldRhythm==-14||nextOldRhythm==-140){stopNew=(float)(((double)nextNew-startNew)/2+startNew);remaining=nextOldRhythm;ni--;}
      else if(nextNewRhythm==-14||nextNewRhythm==-140){nextOld=(float)(((double)nextOld-startOld)/2+startOld);nextOldRhythm=nextNewRhythm;oi--;}
     }
MapInterval:
     if(startOld<nextOld){
      var selected=LayoutCoordinateCollection.Collect(words,starts,(short)staff,(short)staff,startOld,nextOld,0.001f,1,(short)-starts.Length,staffThreshold,0,200,100,capacity);
      float ratio=(float)(((double)stopNew-startNew)/((double)nextOld-startOld)),offset=(float)((double)startNew+1000);
      LayoutCoordinateAffine.Apply(words,selected.References,ratio,offset,startOld,-1);
      startNew=stopNew;newRhythm=remaining;oldRhythm=nextOldRhythm;startOld=nextOld;
     }
    }
    var tail=LayoutCoordinateCollection.Collect(words,starts,(short)staff,(short)staff,threshold,upper,0.001f,1,(short)starts.Length,staffThreshold,0,200,100,capacity);
    foreach(short reference in tail.References){int index=Math.Abs((int)reference)-1;if(threshold<=words[index])words[index]=(float)((double)words[index]-1000);}
   }
   var clefs=new List<int>();var bars=new List<float>();float lastBar=0;
   for(int i=0;i<starts.Length;i++){int staff=Word(p(i,2)),code=Word(p(i,1));float x=p(i,3);
    if(bottom<=staff&&staff<=top){if(code==3&&x>=clefThreshold)clefs.Add(i);else if(code==14&&x>=clefThreshold){bars.Add(x);if(x>lastBar)lastBar=x;}}
    if(staff<0)words[pIndex(i,2)]=-p(i,2);
   }
   foreach(int item in clefs){int staff=Word(p(item,2));float x=p(item,3),gap=(float)((double)staffUnits[staff]*0.3f+1),next=lastBar;bool hasNote=false;
    for(int j=item+1;j<starts.Length;j++){if(Word(p(j,2))!=staff)break;float code=p(j,1);if(code==14){next=p(j,3);break;}if(code<=2){next=p(j,3);hasNote=true;break;}}
    if(hasNote){foreach(float bar in bars)if(bar>=x){if(next>=bar)next=bar;break;}}
    int previous=item;float previousX;
    do{step();previous--;if(previous<0)throw new NotSupportedException("Original clef adjustment reads before the item table.");previousX=p(previous,3);if((double)next-previousX>gap){words[pIndex(item,3)]=(float)(((double)next-gap)+1);goto NextClef;}}while(Word(p(previous,1))>2&&Word(p(previous,1))<17);
    words[pIndex(item,3)]=(float)(((double)next-previousX)/2+previousX);
NextClef:;
   }
  }
 }
}
````

## LayoutLineupStaffTables

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupStaffTables.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupStaffTablesResult {public short FirstMerged,Count;}
 // Original 3111:0303..0674: align one saved staff stream to the merged
 // symbol/duration stream before the source-coordinate remapping pass.
 public static class LayoutLineupStaffTables {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  public static LayoutLineupStaffTablesResult Apply(float[] merged,short mergedCount,float[] storedRhythms,float[] storedPositions,short firstStored,float[] outputRhythms,float[] outputPositions){return new Engine(merged,mergedCount,storedRhythms,storedPositions,firstStored,outputRhythms,outputPositions).Run();}
  sealed class Engine {
   float[] merged,saved,positions,output,oldX;int limit,si,mi,oi,first,steps;float sv,mv,mergedSum,savedSum;
   public Engine(float[] a,int n,float[] b,float[] x,int start,float[] c,float[] y){merged=a;limit=n;saved=b;positions=x;si=start-1;output=c;oldX=y;}
   void Step(){if(++steps>100000)throw new NotSupportedException("Original staff-table traversal exceeds its verified domain.");}
   float Saved(int index){Step();if(index<1||index>saved.Length)throw new NotSupportedException("Original saved staff rhythm leaves its bank.");return saved[index-1];}
   float Merged(int index){Step();if(index<1||index>merged.Length)throw new NotSupportedException("Original merged rhythm leaves its bank.");return merged[index-1];}
   void Append(float rhythm,float x){Step();if(oi>=output.Length||oi>=oldX.Length)throw new NotSupportedException("Original aligned staff table exceeds its buffer.");output[oi]=rhythm;oldX[oi++]=x;}
   LayoutLineupStaffTablesResult Result(){return new LayoutLineupStaffTablesResult{FirstMerged=(short)first,Count=(short)oi};}
   public LayoutLineupStaffTablesResult Run(){sv=Saved(si+1);for(mi=1;mi<=limit;mi++){mv=Merged(mi);if(Bits(mv)>0||Bits(mv)==Bits(sv)){first=mi;mi--;goto NewGroup;}}
    throw new NotSupportedException("Original staff-table alignment requests its error handler for an unmatched stream.");
NewGroup:
    mergedSum=savedSum=0;si++;sv=Saved(si);
    while(true){
     if(mi==limit)return Result();mi++;mv=Merged(mi);
     if(Bits(mv)>=0){mergedSum=(float)((double)mv+mergedSum);
      while(true){
       if(Bits(sv)>0){savedSum=(float)((double)sv+savedSum);Append(sv,positions[si-1]);
        while(true){float delta=(float)((double)mergedSum-savedSum);if(Math.Abs((double)delta)<=0.002f)goto NewGroup;if(Bits(delta)>0)break;
         if(mi==limit)return Result();
         do{mi++;mv=Merged(mi);if(Bits(mv)>=0)break;if(mi==limit)return Result();}while(true);
         mergedSum=(float)((double)mv+mergedSum);
        }
       }
       si++;sv=Saved(si);
      }
     }
     while(true){
      if(Bits(mv)==Bits(sv)){Append(sv,positions[si-1]);goto NewGroup;}
      if(Bits(sv)>0||sv!=-15)break;si++;sv=Saved(si);
     }
     if(si<2)throw new NotSupportedException("Original symbol midpoint reads before its saved-position table.");
     Append(mv,(float)(((double)positions[si-1]-positions[si-2])/2+positions[si-2]));
    }
   }
  }
 }
}
````

## LayoutLineupStreamUnion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupStreamUnion.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupStreamUnionResult {public short Count,CurrentIndex,PriorIndex;public float LastValue,CurrentSum,PriorSum;}
 // Original 31FE:0583..0918 union loop. Inputs are the caller's already
 // normalized rhythm/symbol streams; duration subdivision follows separately.
 public static class LayoutLineupStreamUnion {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  public static LayoutLineupStreamUnionResult Apply(float[] current,short firstCurrent,short currentCount,float[] prior,short priorCount,float[] output){return new Engine(current,firstCurrent,currentCount,prior,priorCount,output).Run();}
  sealed class Engine {
   float[] a,b,c;int ai,bi=1,an,bn,ci,steps;float av,bv,last,asum,bsum,difference;
   public Engine(float[] current,int first,int currentCount,float[] prior,int priorCount,float[] output){a=current;b=prior;c=output;ai=first;an=currentCount;bn=priorCount;}
   void Step(){if(++steps>100000)throw new NotSupportedException("Original lineup stream union exceeds its verified traversal domain.");}
   float A(int index){Step();if(index<1||index>a.Length)throw new NotSupportedException("Current lineup stream leaves its buffer.");return a[index-1];}
   float B(int index){Step();if(index<1||index>b.Length)throw new NotSupportedException("Prior lineup stream leaves its buffer.");return b[index-1];}
   void Append(){Step();if(Bits(bv)!=0&&(Bits(bv)>0||Bits(last)<=0||bv==-14)){
    if(ci>=c.Length)throw new NotSupportedException("Lineup stream union exceeds its output buffer.");c[ci++]=bv;last=bv;
   }}
   public LayoutLineupStreamUnionResult Run(){int nextA=0,nextB=0;
    while(true){Step();bv=B(bi);av=A(ai);
     if(av==-14&&Bits(bv)<0&&bv!=-14&&bi!=bn){
      for(int look=bi+1;look<=bn;look++){float tag=B(look);if(tag>=0)break;if(tag==-14){last=0;goto AppendValue;}}
     }
     if(Bits(bv)==0)bv=av;
     if(bv==-17&&bi<bn&&B(bi+1)==-14)last=bv;
     if(Bits(av)==Bits(bv)||Bits(av)==0)goto AppendValue;
     if(Bits(bv)<=0){
      if(Bits(av)<=0){
       if(bv<av){if(av==-14&&Bits(last)<0)goto DecrementCurrent;}
       else if(bv!=-14||Bits(last)>=0)goto DecrementCurrent;
       goto DecrementPrior;
      }
DecrementCurrent:
      ai--;goto AppendValue;
     }
     if(Bits(av)<0)goto DecrementPrior;
     asum=(float)((double)av+asum);
     while(true){
      bsum=(float)((double)bv+bsum);Append();
      while(true){difference=(float)((double)bsum-asum);if(difference<=0.001f)break;
       while(true){nextA=ai+1;if(nextA>an)goto Finish;av=A(nextA);if(Bits(av)>0)break;if(difference<=0.02f)goto ResetSums;bv=av;ai=nextA;Append();}
       asum=(float)((double)av+asum);ai=nextA;
      }
      if(difference>=-0.001f)break;
      while(true){nextB=bi+1;if(nextB>bn)goto Finish;bv=B(nextB);if(Bits(bv)>0)break;if(difference>=-0.02f)goto ResetSums;bi=nextB;Append();}
      bi=nextB;
     }
ResetSums:
     asum=bsum=0;goto Advance;
DecrementPrior:
     bi--;bv=av;
AppendValue:
     Append();
Advance:
     if(ai>=an&&bi>=bn)goto Finish;
     if(bi<bn)bi++;if(ai<an)ai++;
    }
Finish:
    return new LayoutLineupStreamUnionResult{Count=(short)ci,CurrentIndex=(short)ai,PriorIndex=(short)bi,LastValue=last,CurrentSum=asum,PriorSum=bsum};
   }
  }
 }
}
````

## LayoutLineupSystem

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupSystem.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutLineupWorkspace {
  public readonly float[] Rhythms=new float[150],Positions=new float[151],Merged=new float[150],Scratch=new float[500],Union=new float[150],Subdivisions=new float[150];
  public int MaximumStaff=32;
  public bool StandaloneJust;
  public bool Scor4DurationScale;
  public bool Page4RoundEachPosition;
 }
 public static class LayoutLineupSystem {
  // Original LI arithmetic pipeline, excluding the parent selection/sort/CW
  // pass. Grace/coincidence cleanup follows the original invalidated-sort flag.
  public static LayoutLineupPreparationResult Apply(float[] words,int[] starts,short bottom,short top,float[] staffUnits,float exponent,short staffThreshold,int capacity,LayoutLineupWorkspace work){
   return ApplyWithBeatPolicy(words,starts,bottom,top,staffUnits,exponent,staffThreshold,capacity,work,false);
  }
  public static LayoutLineupPreparationResult ApplyWithBeatPolicy(float[] words,int[] starts,short bottom,short top,float[] staffUnits,float exponent,short staffThreshold,int capacity,LayoutLineupWorkspace work,bool returnUnequalBeats,Action<int> acknowledgeNegativePosition=null){
   // PAGE4 1D4D:00DE–0154 limits P6 of selected staff records before collecting rhythms.
   if(work.Page4RoundEachPosition){
    foreach(int start in starts){
     int index=start-1;
     if((short)words[index]<6||(short)words[index+1]!=8)continue;
     short staff=(short)words[index+2];
     if(staff>=bottom&&staff<=top&&words[index+6]>200f)words[index+6]=200f;
    }
   }
   // JUST4 at file offset 925D and PAGE4 at 15974 load 4.0 before
   // multiplying a positive duration. The classic path uses 2.0.
   float rhythmScale=work.Scor4DurationScale?4f:2f;
   var result=LayoutLineupPreparation.ApplyWithRhythmScale(words,starts,bottom,top,work.Rhythms,work.Positions,work.Merged,work.Scratch,work.Union,work.Subdivisions,returnUnequalBeats,acknowledgeNegativePosition,work.StandaloneJust,rhythmScale);
   if(result.Bottom<0||result.UnequalBeats)return result;
   LayoutLineupCoordinates.Apply(work.Merged,work.Rhythms,work.Positions,result.Count,result.Left,result.Right,staffUnits[result.Bottom],exponent,work.Page4RoundEachPosition);
   int staffLimit=work.MaximumStaff;
   var historyRhythms=new float[2000];var historyPositions=new float[2000];var staffStarts=new short[staffLimit+2];int total=0;
   for(int staff=result.Bottom;staff<=result.Top;staff++){
    if(staff<1||staff>staffLimit)throw new NotSupportedException("Original lineup staff-history scope leaves its table.");staffStarts[staff]=(short)(total+1);
    foreach(var stream in result.StaffStreams)if(stream.Staff==staff){if(total+stream.Rhythms.Length>historyRhythms.Length)throw new NotSupportedException("Original lineup saved-stream bank capacity exceeded.");Array.Copy(stream.Rhythms,0,historyRhythms,total,stream.Rhythms.Length);Array.Copy(stream.Positions,0,historyPositions,total,stream.Positions.Length);total+=stream.Rhythms.Length;break;}
   }
   staffStarts[result.Top+1]=(short)(total+1);
   LayoutLineupRemap.Apply(words,starts,result.Bottom,result.Top,work.Merged,work.Rhythms,result.Count,historyRhythms,historyPositions,staffStarts,staffUnits,staffThreshold,capacity);
   if(result.NeedsCleanup)LayoutLineupCleanup.Apply(words,starts,result.Bottom,result.Top,staffUnits);
   return result;
  }
 }
}
````

## LayoutLineupTableCompact

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutLineupTableCompact.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 31FE:0AF4..0B94. Equal adjacent -14 bar tags collapse;
 // other repeated values are retained without numeric conversion.
 public static class LayoutLineupTableCompact {
  public static short Apply(float[] source,float[] destination,short first,short last,float retainedTag) {
   if(first<1||last>source.Length)throw new NotSupportedException("Original lineup compaction leaves its source table.");
   int count=0;float previous=0;
   for(int i=first-1;i<last;i++){
    float value=source[i];int bits=BitConverter.ToInt32(BitConverter.GetBytes(value),0);
    if(bits==BitConverter.ToInt32(BitConverter.GetBytes(previous),0)&&bits==BitConverter.ToInt32(BitConverter.GetBytes(retainedTag),0))continue;
    if(count>=destination.Length)throw new NotSupportedException("Original lineup compaction exceeds its destination table.");destination[count++]=value;previous=value;
   }
   return (short)count;
  }
 }
}
````

## LayoutNoteSpacingSpan

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutNoteSpacingSpan.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Projection of original 2DCE:0352..0E2D, stopping before common table append.
 // The caller supplies explicit workspace values/preferences and raw score
 // words. No captured workspace, resource data or renderer bounds are embedded.
 // Apply returns false for the original early-finalization branch at 13AE.
 public sealed class LayoutNoteSpacingSpan {
  readonly byte[] data;readonly float[] score,positions;readonly short[] references;readonly bool originalQuarterToneWidths;
  readonly float followingBarScale;
  public LayoutNoteSpacingSpan(byte[] workspace,float[] scoreWords,short[] recordReferences,float[] selectedPositions,bool originalQuarterToneWidths=true,float followingBarScale=1){data=workspace;score=scoreWords;references=recordReferences;positions=selectedPositions;this.originalQuarterToneWidths=originalQuarterToneWidths;this.followingBarScale=followingBarScale;if(data==null||data.Length<65536)throw new ArgumentException("A complete explicit workspace is required.");}
  short W(int a){return BitConverter.ToInt16(data,a);}void W(int a,int v){var b=BitConverter.GetBytes(unchecked((short)v));Buffer.BlockCopy(b,0,data,a,2);}
  float F(int a){return BitConverter.ToSingle(data,a);}void F(int a,double v){var b=BitConverter.GetBytes((float)v);Buffer.BlockCopy(b,0,data,a,4);}
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(double f){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes((float)f),0),out n))throw new NotSupportedException("Note-spacing integer conversion is outside the finite converter domain.");return unchecked((short)n);}
  float Raw(int w,int p){int index=w-1+p;if(index<0||index>=score.Length)throw new NotSupportedException("Original note-spacing word reference leaves its buffer.");return score[index];}
  int Ref(int i){if(i<1||i>references.Length)throw new NotSupportedException("Original note-spacing selection reference leaves its list.");return references[i-1];}
  float Pos(int i){if(i<1||i>positions.Length)throw new NotSupportedException("Original note-spacing position leaves its table.");return positions[i-1];}
  void Pos(int i,float x){if(i<1||i>positions.Length)throw new NotSupportedException("Original note-spacing position leaves its table.");positions[i-1]=x;}
  double Mod(double value,double divisor){if(Double.IsNaN(value)||Double.IsInfinity(value)||divisor==0)throw new NotSupportedException("Non-finite original note-spacing remainder is prevented.");return value%divisor;}
  void Load(int w,int countAddress){
   int count=Word(Raw(w,0));if(count<1)throw new NotSupportedException("Original note-spacing record extent is invalid.");W(countAddress,count);W(0xb9c0,Word(Raw(w,2)));Array.Clear(data,0xb9c2,48);
   int last=Math.Min(count,14);W(0xd552,last);W(0xafa2,1);
   for(int p=3;p<=last;p++)F(0xb9c2+(p-3)*4,Raw(w,p));W(0xafa0,last>=3?last+1:3);
   if(F(0xb9ce)<=F(0x77f6)&&Raw(w,1)==F(0x77e0)){F(0xb9ca,0);F(0xb9da,0);}
  }
  public void LoadRecord(int wordIndex,int countAddress){Load(wordIndex,countAddress);}
  public bool Apply(){
   W(0xae9c,Word(F(0xb9ce)));F(0xd4f0,F(0x383e));int style=W(0xae9c);
   if(style<=19&&(style%10==2||style%10==3))F(0xd4f0,F(0x3842));
   W(0xd4ba,W(0xd48a));W(0xae9a,Word(F(0xb9ca)));if(W(0xae9a)>100)F(0xb9ca,Mod(F(0xb9ca),F(0x7766)));
   W(0xd4a0,Word((double)F(0xb9ca)/F(0x776a)));
   if(W(0xd4a0)!=1){
    for(int bound=0;bound<1000;bound++){
     if(W(0xd4a0)!=0||W(0xd4ba)>=W(0xd484))goto Ready;
     W(0xd4ba,W(0xd4ba)+1);W(0xd4a2,Ref(W(0xd4ba)));
     if(Raw(W(0xd4a2),1)!=F(0x774c)||Raw(W(0xd4a2),3)!=F(0xb9c2))goto Ready;
     W(0xd4a0,(Word(Raw(W(0xd4a2),5))%100)/10);if(W(0xd4a0)==1)break;
     if(bound==999)throw new NotSupportedException("Original note-spacing voice search exceeded its list.");
    }
   }
   if(W(0xd4ba)>=W(0xd484))return false;
   W(0xd4a2,Ref(W(0xd4ba)+1));W(0xd4aa,Word(Raw(W(0xd4a2),1)));
   if(W(0xd4aa)==14)goto FollowingBar;
   F(0xd4c8,Raw(W(0xd4a2),3));if(W(0xd4aa)!=1)goto Ready;
   for(int bound=0;bound<1000;bound++){
    W(0xd49e,Word(Raw(W(0xd4a2),5))%100);if(W(0xd49e)>=20)goto FollowingBar;
    if(Bits(Raw(W(0xd4a2),3))!=Bits(F(0xd4c8))||W(0xd49e)>=10||W(0xd4ba)>=W(0xd484))goto Ready;
    W(0xd4ba,W(0xd4ba)+1);W(0xd4a2,Ref(W(0xd4ba)+1));W(0xd4aa,Word(Raw(W(0xd4a2),1)));if(W(0xd4aa)!=1)goto Ready;
    if(bound==999)throw new NotSupportedException("Original note-spacing following-voice search exceeded its list.");
   }
  FollowingBar:
   for(int bound=0;bound<1000;bound++){
    W(0xd4ba,W(0xd4ba)+1);if(W(0xd4ba)>=W(0xd484))goto Extra;
    W(0xd4a2,Ref(W(0xd4ba)+1));W(0xd4aa,Word(Raw(W(0xd4a2),1)));
    if(W(0xd4aa)!=1||Bits(Raw(W(0xd4a2),3))!=Bits(F(0xd4c8)))goto Extra;
    W(0xd49e,Word(Raw(W(0xd4a2),5))%100);if(W(0xd49e)<10)continue;if(W(0xd49e)<20)goto Ready;goto Extra;
   }
   throw new NotSupportedException("Original note-spacing bar search exceeded its list.");
  Extra:
   F(0xd4c8,F(0x776e));if(Bits(F(0xb9da))!=0)F(0xd4c8,F(0x7772));F(0xd4f0,(double)F(0xd4c8)*followingBarScale+F(0xd4f0));
  Ready:
   if(W(0xd4cc)!=W(0xd4dc))W(0xd4dc,0);W(0xd4cc,0);W(0xd4a0,W(0xd48a));F(0xd47e,F(0xb9ca));
   W(0xd49e,W(0xd4a0)+1);
   for(int bound=0;bound<1000;bound++){
    if(W(0xd49e)>W(0xd484)+1)break;W(0xd48a,W(0xd49e));if(W(0xd49e)>W(0xd484)||Raw(Ref(W(0xd49e)),1)!=F(0x774c))break;
    double delta=(double)Pos(W(0xd49e))-Pos(W(0xd49e)-1);F(0xd4c0,delta);if(delta>=F(0x7776))break;Pos(W(0xd49e),Pos(W(0xd49e)-1));W(0xd49e,W(0xd49e)+1);
    if(bound==999)throw new NotSupportedException("Original note-spacing coincidence search exceeded its list.");
   }
   F(0xd4bc,0);W(0xd48a,W(0xd48a)-1);if(Math.Abs((double)F(0xb9c6))>=F(0x7766))F(0xd48e,F(0x777a));
   if(W(0xd4f4)>=15&&Bits(Raw(W(0xd48c),15))!=0)F(0xd48e,(double)Raw(W(0xd48c),15)*F(0xd48e));
   F(0xd4ac,0);F(0xd4c8,0);W(0xd49e,W(0xd4a0));
   for(int bound=0;bound<1000;bound++){
    W(0xd4aa,Ref(W(0xd49e)));
    if(Raw(W(0xd4aa),1)==F(0x774c))ProcessMember();
    W(0xd49e,W(0xd49e)+1);if(W(0xd49e)>W(0xd48a))break;
    if(bound==999)throw new NotSupportedException("Original note-spacing member loop exceeded its list.");
   }
   F(0xd4d4,(double)F(0xd4ac)*F(0xd48e));F(0xd4a6,((double)F(0xd4c8)+F(0xd4f0))*F(0xd48e));
   W(0xd4b4,W(0xd4a0));W(0xd4a2,W(0xd48a));F(0xd4ea,F(0xd4b0));F(0xd49a,F(0xd47e));F(0xd4fa,0);
   if(W(0xd4f4)>=9)F(0xd4fa,Mod(Raw(W(0xd48c),9),F(0x776a)));
   F(0xd492,F(0xb9e6));return true;
  }
  void ProcessMember(){
   W(0xd4ee,0);foreach(int a in new[]{0xd4d0,0xd4c4,0xd496,0xd4bc,0xd486})F(a,0);Load(W(0xd4aa),0xd4ba);
   if(W(0xd4ba)>=9&&Bits(F(0xb9da))!=0){
    F(0xd486,F(0xb9da));W(0xd4e8,Word(F(0xd486)));W(0xd4c0,W(0xd4e8)%10);W(0xd4e8,(W(0xd4e8)%100)/10);
    if(W(0xd4c0)!=0&&Mod(F(0xb9ca),F(0x7766))<F(0x777e))F(0xd4bc,(double)F(0xd48e)*F(0x7782));
    F(0xd486,Mod(F(0xd486),F(0x774c)));if(F(0xd486)>F(0x7786))F(0xd486,(double)F(0x7786)-F(0xd486));
    F(0xd486,(double)W(0xd4e8)*F(0x778a)+(double)F(0xd486)*F(0x776a));if(F(0xd486)>F(0xd4bc))F(0xd4bc,F(0xd486));W(0xd4ee,0);
   }
   F(0xd47a,0);
   if(W(0xd4ba)>=10&&Bits(Raw(W(0xd4aa),10))!=0){
    F(0xd47a,Raw(W(0xd4aa),10));F(0xd4c0,F(0xd47a));
    if(F(0xd4c0)==F(0x776a)||F(0xd4c0)==F(0x777e)){
     F(0xd4c0,F(0x3846));if(F(0xd4f0)==F(0x3842))F(0xd4c0,F(0x383e));
     if(F(0xd47a)>=F(0x777e)){F(0xd4c4,-F(0xd4c4));W(0xd4dc,0);F(0xd4d0,F(0xd4c0));}else F(0xd4c4,F(0xd4c0));
    }else if(Bits(F(0xd4c0))<0)F(0xd4d0,-F(0xd4c0));else{W(0xd4ba,-1);F(0xd4c4,F(0xd4c0));}
   }
   F(0xd4c4,(double)F(0xd4bc)+F(0xd4c4));W(0xae9a,Word(F(0xb9ca)));W(0xd4a4,W(0xae9a)%10);
   if(W(0xd4a4)!=0){
    W(0xd4dc,0);if(W(0xae9a)>100)W(0xd4a4,W(0xd4a4)+9);
    // User-approved default: Tom's Help explicitly gives 106..109 the
    // same symbols as 6..9. Compatibility retains the original adjacent
    // descriptor read in the caller's explicit DOS workspace instead.
    int widthIndex=W(0xd4a4);if(widthIndex>=15&&widthIndex<=18&&!originalQuarterToneWidths)widthIndex-=9;
    if(widthIndex<0||widthIndex>18)throw new NotSupportedException("Original accidental-spacing control leaves the verified table/descriptor domain.");
    F(0xd4c0,F(0x3846+widthIndex*4));
    if(W(0xd4de)==1&&W(0xd4b4)==W(0xd4a2)){
     if(F(0xb9e6)!=F(0xd492)){
      if(F(0xd49a)>=F(0x777e)||F(0xd47e)>=F(0x777e))goto SkipAccidental;
      F(0xd4b6,F(0xb9e6));if(F(0xd4b6)==F(0x774c))F(0xd4b6,-F(0xd4b6));if(F(0xd492)==F(0x774c))F(0xd492,-F(0xd492));
      if(F(0xd4b6)>=F(0xd492))goto SkipAccidental;
     }else if(Math.Abs((double)F(0xd4ea)-F(0xd4b0))>=F(0x778e)){
      bool subtract=F(0xd49a)<F(0x777e)?F(0xd4ea)>F(0xd4b0):(F(0xd4ea)<F(0xd4b0)||Bits(F(0xd4fa))==0);
      if(subtract)F(0xd4c0,(double)F(0xd4c0)-F(0x776e));
     }
    }
    F(0xd4b6,Mod(F(0xb9ca),F(0x774c)));if(F(0xd4b6)>F(0x7786))F(0xd4b6,(double)F(0x7786)-F(0xd4b6));
    F(0xd4d0,(double)F(0xd4b6)*F(0x776a)+F(0xd4c0)+F(0xd4d0));if(W(0xd4ba)<0)F(0xd4d0,(double)F(0xd4d0)-F(0xd47a));
   }
  SkipAccidental:
   F(0xb9c6,Mod(F(0xb9c6),F(0x7766)));W(0xd4cc,0);if(F(0xb9c6)>F(0x7792))W(0xd4cc,1);if(F(0xb9c6)<F(0x7750))W(0xd4cc,-1);
   if(W(0xd4cc)==0||W(0xd4dc)!=W(0xd4cc))W(0xd4dc,W(0xd4cc));else{if(F(0xd4d0)<F(0x7796))F(0xd4d0,F(0x7796));if(W(0xd4ee)!=0)W(0xd4dc,0);}
   if(W(0xae9c)>=10&&W(0xae9c)<=19){F(0xd4d0,(double)F(0x778a)+F(0xd4d0));F(0xd4c4,(double)F(0x779a)+F(0xd4c4));}
   if(F(0xd4d0)>F(0xd4ac))F(0xd4ac,F(0xd4d0));if(F(0xd4c4)>F(0xd4c8))F(0xd4c8,F(0xd4c4));
  }
 }
}
````

## LayoutParentPreparation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutParentPreparation.cs)

````csharp
using System;using System.Linq;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Projection of 2D31:0155..05E1 before the LI/J core. Duplicate deletion is
 // deliberately interactive: callers must supply each measured Y/N decision.
 public sealed class LayoutParentPreparation {
  public readonly List<float[]> Rows;public short Bottom,Top;public float Left,Right;
  public readonly List<int> Origins;
  public bool Modified,Complete;public int DuplicateIndex=-1;int scan;readonly bool lineup;
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(f),0),out n))throw new NotSupportedException("Layout word conversion is outside its finite domain.");return unchecked((short)n);}
  static float P(float[] r,int p){return r.Length>=p?r[p-1]:0;}
  void Sort(){int[] order=InputTranspositionOrder.OrderForMain(Rows,false);var next=order.Select(i=>Rows[i]).ToArray();var origins=order.Select(i=>Origins[i]).ToArray();Rows.Clear();Rows.AddRange(next);Origins.Clear();Origins.AddRange(origins);}
  public LayoutParentPreparation(IList<float[]> rows,short bottom,short top,float left,float right,bool alreadySorted,bool lineupMode=false) {
   lineup=lineupMode;
   Rows=rows.Select(r=>(float[])r.Clone()).ToList();Origins=Enumerable.Range(0,rows.Count).ToList();Bottom=bottom;Top=top;Left=left;Right=right;
   if(!alreadySorted){Modified=InputTranspositionOrder.MarksModified(Rows.Count,false);Sort();}
   float previous=-9999;
   foreach(var row in Rows){int code=Word(P(row,1));if(code==1){previous=P(row,3);continue;}if(code==4&&row.Length>=7&&Bits(P(row,6))==0&&Bits(P(row,7))<0&&Math.Abs((double)previous-P(row,3))<0.1f){row[2]=(float)((double)previous-0.001f);Modified=true;}previous=-9999;}
   Sort();FindDuplicate();
  }
  void FindDuplicate(){
   DuplicateIndex=-1;
   while(scan<Rows.Count-1){var row=Rows[scan];var next=Rows[scan+1];short staff=Word(P(row,2)),code=Word(P(row,1));if(lineup&&staff>=Bottom&&staff<=Top&&code==8){Left=row.Length>=3?row[2]:0;Right=row.Length>=6&&Bits(row[5])!=0?row[5]:200;}if(staff>=Bottom&&staff<=Top&&(code==3||code==8||code==17||code==18)&&P(next,1)==code&&P(next,2)==staff&&Math.Abs((double)P(row,3)-P(next,3))<=0.2f){DuplicateIndex=scan;return;}scan++;}
   FinishAnchors();
  }
  public void AnswerDuplicate(bool delete){if(DuplicateIndex<0||Complete)throw new InvalidOperationException("No layout duplicate decision is pending.");if(delete){Rows.RemoveAt(scan);Origins.RemoveAt(scan);Modified=true;}else scan++;FindDuplicate();}
  void FinishAnchors(){
   if(Rows.Count==0){Complete=true;return;}
   short maximum=Rows[Rows.Count-1].Length<1?(short)1:Word(P(Rows[Rows.Count-1],2));if(maximum<Bottom){Complete=true;return;}if(maximum<Top)Top=maximum;
   int cursor=0;
   for(int staff=Bottom;staff<=Top;staff++) {
    float origin=-9999;int clef=-1;
    for(int i=cursor;i<Rows.Count;i++){
     var row=Rows[i];short code=Word(P(row,1));if(Word(P(row,2))!=staff||(code!=8&&code!=3))continue;
     if(row.Length<3){Array.Resize(ref row,3);Rows[i]=row;Modified=true;}
     if(code==3){clef=i;if(origin==-9999)continue;}else{origin=row[2];if(clef<0)continue;}
     float x=Rows[clef][2];if(Bits(x)==Bits(origin)&&Left<=x&&Right>x){Rows[clef][2]=(float)((double)origin+1.5f);Modified=true;}
     cursor=i+1;break;
    }
   }
   Sort();Complete=true;
  }
 }
}
````

## LayoutRestSpacing

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutRestSpacing.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutRestSpacingResult {public float Multiplier,Right,Left;public float? Parameter10Scratch;}
 public static class LayoutRestSpacing {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  // Original 2F68:06BE..08BC. All literals match both the original unpacked
  // initial data and measured runtime data (compiler-constants.json).
  // This is a spacing helper, not a glyph-bounds approximation.
  public static LayoutRestSpacingResult Apply(float multiplier,float p6,float p5,float p4,float left,short parameterCount,float p9,float p10,float p15) {
   foreach(float f in new[]{multiplier,p6,p5,p4,left,p9,p10,p15})if(Single.IsNaN(f)||Single.IsInfinity(f))throw new NotSupportedException("Non-finite rest-spacing controls are outside this verified helper domain.");
   float right=4.5f;
   if(p5<0) {
    right=6;
    if(p5==-4)right=9.5f;
    if(parameterCount>=9&&Bits(p9)!=0)right=1;
   } else if(p5>1)right=(float)(((double)p5-1)*.5+right);
   if(p6!=0)right=(float)((double)p6*1.75+right);
   if(Math.Abs((double)p4)>=100)multiplier=.6f;
   float? scratch=null;
   if(parameterCount>=10) {
    scratch=p10;
    if(Bits(p10)!=0){left=-p10;right=(float)((double)p10+right);}
    // Original 088F is FILD dword, then the later multiply reads the same
    // bytes as float. In the verified zero comparison that is a raw-bit
    // nonzero test: a negative-zero P15 must reach the multiply too.
    if(parameterCount>=15&&Bits(p15)!=0)multiplier=(float)((double)p15*multiplier);
   }
   return new LayoutRestSpacingResult{Multiplier=multiplier,Right=right,Left=left,Parameter10Scratch=scratch};
  }
 }
}
````

## LayoutRhythmicCollection

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutRhythmicCollection.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutRhythmicCollectionResult {
  public short Count,NextItem;public float Total;public bool InvalidatedSort;
 }
 // Original 2CB5:000B..0543, with its 0544 meter filter and 05E2
 // simultaneous-duration reducer. Arrays and scratch remain caller-owned.
 public static class LayoutRhythmicCollection {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Rhythmic collection leaves the verified finite word domain.");return (short)Math.Truncate(f);}
  static bool Excluded(float[] words,int at,int count){return count>=14&&Bits(words[at+14])!=0||count>=15&&Bits(words[at+15])!=0;}
  public static LayoutRhythmicCollectionResult Collect(float[] words,int[] starts,float staff,short inputMode,short cursor,float[] rhythms,float[] positions,float[] scratch,Action<int> acknowledgeNegativePosition=null,bool standaloneJust=false) {
   if(words==null||starts==null||rhythms==null||positions==null||scratch==null||cursor<1)throw new ArgumentException("Invalid rhythmic collector buffers.");
   int target=Word(staff),first=-1,last=-1,next=cursor,count=0,group=0;float total=0,lastX=-990f;bool invalidated=false;
   Func<int,int> extent=delegate(int i){if(i<0||i>=starts.Length)throw new NotSupportedException("Rhythmic record index leaves its bank.");int at=starts[i]-1;if(at<0||at+2>=words.Length)throw new NotSupportedException("Rhythmic record leaves its bank.");int n=Word(words[at]);if(n<2||at+n>=words.Length)throw new NotSupportedException("Rhythmic record extent is invalid.");if(n<3)throw new NotSupportedException("Original short rhythmic record path is not yet verified.");return n;};
   Func<int,int,bool> meterExcluded=delegate(int at,int n){if(n<4)return false;float size=1;if(n>=7&&Bits(words[at+7])!=0)size=words[at+7];return Math.Abs((double)words[at+4]*size)>2;};
   for(int i=cursor-1;i<starts.Length;i++) {
    int n=extent(i),at=starts[i]-1,code=Word(words[at+1]),itemStaff=Word(words[at+2]);
    if(first<0){
     if(itemStaff<target)continue;
     bool ignored=code>3&&code<17&&code!=14;
     if(standaloneJust&&ignored)continue;
     float x=words[at+3];if(Math.Abs((double)x)<=0.01f){if(n>2)words[at+3]=0;x=0;}
     if(ignored)continue;
     if(Bits(x)<0){
      if(acknowledgeNegativePosition==null)throw new NotSupportedException("Original rhythmic collector requests input for a negative first position.");
      // PAGE 21A8:00D2–00EE warns, waits, and resumes at the next item.
      acknowledgeNegativePosition(i+1);
      continue;
     }
     if(itemStaff!=target)break;
     if(code==14&&n>4){int subtype=Word(words[at+5]);if(subtype>7&&subtype<11)continue;}
     if(code>16&&(Excluded(words,at,n)||code==18&&meterExcluded(at,n)))continue;
     first=i;
    } else if(itemStaff>target){next=i+1;last=i-1;break;}
    if(i==starts.Length-1){last=i;/* Original leaves the caller cursor unchanged here. */}
   }
   if(first<0)return new LayoutRhythmicCollectionResult{NextItem=(short)next};
   while(last>first){int at=starts[last]-1;int code=Word(words[at+1]);if(code<3||code==14||code>16)break;last--;}
   if(last<first)return new LayoutRhythmicCollectionResult{NextItem=(short)next};
   {int at=starts[last]-1;if(Math.Abs(200.0-words[at+3])<=0.01f)words[at+3]=200;}
   Action flush=delegate {
    if(count<1)throw new NotSupportedException("Original empty rhythmic reduction would address the preceding table cell.");
    int old=group;group=0;float minimum=scratch[0];
    if(old>=2){int minimumIndex=0;for(int j=1;j<old;j++)if(scratch[j]<=minimum){minimum=scratch[j];minimumIndex=j;}
     if(minimumIndex!=0){scratch[minimumIndex]=scratch[0];scratch[0]=minimum;}
     for(int j=1;j<old;j++){double difference=(double)scratch[j]-minimum;if(difference>0.001f){if(group>=scratch.Length)throw new NotSupportedException("Rhythmic scratch capacity exceeded.");scratch[group++]=(float)difference;}}
    }
    rhythms[count-1]=minimum;
   };
   for(int i=first;i<=last;i++) {
    int n=extent(i),at=starts[i]-1,code=Word(words[at+1]);float x=words[at+3],duration=0;bool add=false;
    if(code<3){if(n<7)continue;duration=words[at+7];if(Bits(duration)<=0)continue;
     if(duration>=64){invalidated=true;if(inputMode!=0)continue;duration=-64;}
     if(Math.Abs((double)x-lastX)>0.1f)add=true;
    }else{
     if(!(code==14||code==3||code>16)||x==lastX)continue;
     if(code>16&&Excluded(words,at,n)||code==18&&meterExcluded(at,n))continue;
     if(code==14&&n>11&&Bits(words[at+12])>0)code=140;
     if(code==3&&n>=5&&words[at+5]>=44)continue;
     duration=-code;add=true;
    }
    if(add){if(count>0)flush();if(duration==99&&group!=0)throw new NotSupportedException("Original rhythmic collector requests input for its pending-duration diagnostic.");
     if(count>=rhythms.Length||count>=positions.Length)throw new NotSupportedException("Rhythmic table capacity exceeded.");positions[count++]=x;lastX=x;
    }
    if(group>=scratch.Length)throw new NotSupportedException("Rhythmic scratch capacity exceeded.");scratch[group++]=duration;
   }
   flush();for(int i=0;i<count;i++)if(Bits(rhythms[i])>0)total=(float)((double)total+rhythms[i]);if(total==0)count=0;
   return new LayoutRhythmicCollectionResult{Count=(short)count,NextItem=(short)next,Total=total,InvalidatedSort=invalidated};
  }
 }
}
````

## LayoutRipple

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutRipple.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutRippleResult {public bool Applied;public int[] Starts;public short Source,Destination,Count;}
 // Original overlay 6, 2C0C:0000 through the first LJ call. Caller supplies
 // the original sorted record index and retains headers and external resources.
 public static class LayoutRipple {
  static int W(float v){if(Single.IsNaN(v)||Single.IsInfinity(v)||v< -32768||v>=32768)throw new NotSupportedException("Original RIP working word leaves its finite domain.");return (short)Math.Truncate(v);}
  public static LayoutRippleResult Prepare(float[] words,int[] starts,short count,short source,short destination,short bars){
   var result=new LayoutRippleResult{Starts=starts,Source=source,Destination=destination,Count=count};if(count<1||source<1||destination<1||bars<1)return result;if(source+count-1>32||destination+count-1>32)throw new NotSupportedException("Original RIP staff traversal leaves the supported table.");
   int sourceFirst=0,destinationFirst=0,lastPosition=-999;var barWords=new List<int>();int bottom=Math.Min(source,destination),top=Math.Max(source,destination);var live=new List<int>(starts);
   foreach(int w in starts){int at=w-1,code=W(words[at+1]),staff=W(words[at+2]);if(staff<bottom)continue;if(staff>top)break;if(staff==destination){if(destinationFirst==0&&(code<3||code==18))destinationFirst=w;}else if(code<3||code==18){if(sourceFirst==0)sourceFirst=w;}else if(code==14&&sourceFirst!=0){int rounded=(int)Math.Round(words[at+3],MidpointRounding.AwayFromZero);if(rounded==lastPosition)throw new NotSupportedException("Original RIP reports repeated bar positions.");barWords.Add(w);lastPosition=rounded;}}
   if(bars>barWords.Count)return result;if(sourceFirst==0)throw new NotSupportedException("Original RIP source boundary is absent.");
   // A zero destination reference is used by the original for an empty
   // destination; its same one-based expression reads word 2 as the anchor.
   // Keep that measured arithmetic rather than inventing a replacement point.
   for(int staff=top;staff<top+count;staff++){
    int remove=live.FindIndex(candidate=>W(words[candidate+1])==staff&&words[candidate]==18&&words[candidate+2]>=195);if(remove<0)continue;
    int w=live[remove],length=W(words[w-1])+1;Array.Copy(words,w-1+length,words,w-1,words.Length-(w-1+length));live.RemoveAt(remove);
    for(int i=remove;i<live.Count;i++)live[i]-=length;
    // The original deletion helper compacts the word bank and index table.
    // Keep references to the already collected source bar list in that bank.
    for(int i=0;i<barWords.Count;i++)if(barWords[i]>w)barWords[i]-=length;
    if(sourceFirst>w)sourceFirst-=length;if(destinationFirst>w)destinationFirst-=length;
   }
   int cutIndex=destination<source?barWords.Count-bars:bars,cutWord=cutIndex==0?0:barWords[cutIndex-1]+3;float cutoff=cutIndex==0?(float)((double)words[sourceFirst+2]-1):(float)((double)words[cutWord-1]+0.10000000149011612f);float shift,anchor,total,ratio;short mode=0;float left,right;int low=destination,high=destination+count-1;
   Func<int,int,float,float,short,short[]> collect=(b,t,l,r,m)=>LayoutCoordinateCollection.Collect(words,live.ToArray(),(short)b,(short)t,l,r,.001f,1,(short)live.Count,32,0,200,100,2800,m).References;
   Action<float,float,float,short> move=(l,r,delta,m)=>{for(int staff=source;staff<source+count;staff++){var references=collect(staff,staff,l,r,m);LayoutRippleCoordinates.Shift(words,references,delta);LayoutRippleCoordinates.StaffShift(words,references,destination-source);}};
   if(destination<source){float tail=(float)(200d-cutoff);anchor=(float)((double)words[destinationFirst+2]-1);shift=(float)((double)tail+10);LayoutRippleCoordinates.Shift(words,collect(low,high,anchor,210,0),shift);move(cutoff,210,(float)((double)anchor-cutoff),2);float available=(float)(200d-anchor);total=(float)((double)available+tail+9.899999618530273f);ratio=(float)(((double)available-1)/total);mode=-1;left=anchor;right=(float)((double)anchor+total);
   }else{anchor=words[destinationFirst+2];float sourceLeft=(float)((double)words[sourceFirst+2]-1);float width=(float)((double)cutoff-sourceLeft);shift=(float)(210d-sourceLeft);move(sourceLeft,cutoff,shift,0);right=(float)((double)width+210);ratio=(float)((200d-anchor)/((double)right-anchor));mode=2;left=anchor;total=right;}
   LayoutRippleCoordinates.Scale(words,collect(low,high,left,right,mode),anchor,ratio);if(mode>=0){low=source;high=source+count-1;}LayoutRippleCoordinates.Shift(words,collect(low,high,210,1000,0),-shift);if(cutWord!=0)words[cutWord-1]=200;result.Applied=true;result.Starts=live.ToArray();return result;
  }
 }
}
````

## LayoutRippleCoordinates

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutRippleCoordinates.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original RIP's 2C0C:0971 and 09D5, with signed one-based coordinate words.
 public static class LayoutRippleCoordinates {
  public static void Shift(float[] words,short[] references,float delta){foreach(short reference in references){int at=Math.Abs((int)reference)-1;if(at<0||at>=words.Length)throw new NotSupportedException("Original RIP coordinate leaves its word bank.");words[at]=(float)((double)words[at]+delta);}}
  public static void Scale(float[] words,short[] references,float origin,float factor){float offset=(float)((double)origin-(double)origin*factor);foreach(short reference in references){int at=Math.Abs((int)reference)-1;if(at<0||at>=words.Length)throw new NotSupportedException("Original RIP coordinate leaves its word bank.");words[at]=(float)((double)words[at]*factor+offset);}}
  public static void StaffShift(float[] words,short[] references,float delta){foreach(short reference in references)if(reference>0){int at=reference-2;if(at<0||at>=words.Length)throw new NotSupportedException("Original RIP staff reference leaves its word bank.");words[at]=(float)((double)words[at]+delta);}}
 }
}
````

## LayoutSeparateParts

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutSeparateParts.cs)

````csharp
using System;using System.Collections.Generic;using System.Linq;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutSeparateResult {public List<float[]> Rows;public List<int> Origins;public readonly List<short> ChangedStaffs=new List<short>();public int Deleted;}
 public static class LayoutSeparateParts {
  static int Bits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0);}static float P(float[] row,int n){return row.Length>=n?row[n-1]:0;}static short W(float value){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out n))throw new NotSupportedException("Original SEP word conversion leaves its finite domain.");return unchecked((short)n);}
  static float Pitch(float value){return(float)((double)value-Math.Truncate((double)value/100)*100);}
  public static void Validate(IList<float[]> rows,short bottom,short top,float left,float right){for(int staff=bottom;staff<=top;staff++){float previousX=0,previousRhythm=0;foreach(var row in rows){short code=W(P(row,1));if(W(P(row,2))!=staff||code>=3||P(row,3)<left||P(row,3)>right)continue;float rhythm=P(row,7);if(row.Length<7||Bits(rhythm)==0)throw new NotSupportedException("Original SEP requires complete nonzero rhythms on the selected notes/rests.");if(rhythm>64)rhythm=64;if(Bits(P(row,3))==Bits(previousX)&&(code==2||Bits(rhythm)!=0&&Bits(rhythm)!=Bits(previousRhythm)))throw new NotSupportedException("Original SEP rejects incompatible coincident rhythms/rests.");previousX=P(row,3);previousRhythm=rhythm;}}}
  public static LayoutSeparateResult Apply(IList<float[]> source,short bottom,short top,float left,float right,bool deleteBottom){
   if(bottom<1||top>32||top<bottom)throw new NotSupportedException("Original SEP staff traversal leaves its table.");Validate(source,bottom,top,left,right);var result=new LayoutSeparateResult{Rows=source.Select(row=>(float[])row.Clone()).ToList(),Origins=Enumerable.Range(0,source.Count).ToList()};var rows=result.Rows;
   for(short staff=bottom;staff<=top;staff++){int changed=0;for(int cursor=0;cursor<rows.Count-1;cursor++){var row=rows[cursor];if(W(P(row,2))>staff)break;if(W(P(row,2))!=staff||W(P(row,1))!=1||P(row,3)<left)continue;if(P(row,3)>right)break;int selected=cursor,retained=-1,scan=1;float extreme=Pitch(P(row,4)),second=deleteBottom?100:-100;
     for(;cursor+scan<rows.Count;scan++){var next=rows[cursor+scan];if(P(next,3)>(double)P(row,3)+0.05000000074505806||W(P(next,2))!=staff)break;if(P(next,1)!=1)continue;float pitch=Pitch(P(next,4));if(deleteBottom?pitch<extreme:pitch>extreme){second=extreme;retained=selected;selected=cursor+scan;/* Original retains its initial comparison pitch. */}else if(deleteBottom?pitch<second:pitch>second){second=pitch;retained=cursor+scan;}}
     if(retained<0)continue;var keep=rows[retained];var deleted=rows[selected];if(keep.Length<5){cursor+=scan-2;continue;}short p5=W(P(keep,5));if(p5<10)p5=unchecked((short)(p5+W((float)((double)P(deleted,5)/10))*10));short packed=keep.Length>8?W(P(keep,9)):(short)0,low=unchecked((short)(packed%10)),high=unchecked((short)(packed/10));if(deleted.Length>8){short other=W(P(deleted,9));if(low==0)low=unchecked((short)(other%10));if(high==0)high=unchecked((short)((other/10)*10));}float p11=keep.Length>10?P(keep,11):0;if(deleted.Length>10&&Bits(p11)==0)p11=P(deleted,11);if(keep.Length<deleted.Length){Array.Resize(ref keep,deleted.Length);rows[retained]=keep;}keep[4]=p5;if(deleted.Length>8)keep[8]=unchecked((short)(high*10+low));if(deleted.Length>9)keep[9]=0;if(deleted.Length>10)keep[10]=p11;rows.RemoveAt(selected);result.Origins.RemoveAt(selected);result.Deleted++;changed++;cursor+=scan-2;
    }if(changed>0)result.ChangedStaffs.Add(staff);
   }return result;
  }
 }
}
````

## LayoutSpacingSelection

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutSpacingSelection.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutSpacingSelectionResult {
  public float[] Positions;public short[] RecordWords;public int StoppedItem;
 }
 public static class LayoutSpacingSelection {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Layout spacing conversion is outside the verified signed-word domain.");return (short)Math.Truncate(f);}
  // Original 2F68:0E6E: adjacent paired exchanges, backing up after a swap.
  // Equal positions retain their original reference order.
  public static void Sort(float[] positions,short[] records,short count) {
   if(positions==null||records==null||count<2||count>positions.Length||count>records.Length)throw new NotSupportedException("Original spacing sort requires its measured two-or-more-entry domain.");
   for(int i=0;i<count;i++)if(Single.IsNaN(positions[i])||Single.IsInfinity(positions[i]))throw new NotSupportedException("Non-finite original spacing-sort input is prevented.");
   int current=1;
   while(current<count) {
    int previous=current-1;
    if(positions[current]<positions[previous]) {
     short r=records[current];records[current]=records[previous];records[previous]=r;
     float x=positions[current];positions[current]=positions[previous];positions[previous]=x;
     if(previous>0)current=previous;
    } else current++;
   }
  }
  // Original 2DCE:000A..026E. Its horizontal comparisons use signed high
  // words and unsigned low words, not x87 numeric comparisons. Preserve that
  // ordering even for negative coordinates and signed zero.
  public static LayoutSpacingSelectionResult Collect(float[] words,int[] starts,short staff,short firstStaff,short cursor,float left,float right,float tolerance,float one,float clefLimit,float spanningLimit,float barLimit,float drawingLimit,int capacity) {return Collect(words,starts,staff,firstStaff,cursor,left,right,tolerance,one,clefLimit,spanningLimit,barLimit,drawingLimit,capacity,false);}
  public static LayoutSpacingSelectionResult Collect(float[] words,int[] starts,short staff,short firstStaff,short cursor,float left,float right,float tolerance,float one,float clefLimit,float spanningLimit,float barLimit,float drawingLimit,int capacity,bool scor4Numbers) {
   float low=(float)((double)left-tolerance),high=(float)((double)right+tolerance);
   var positions=new List<float>();var records=new List<short>();int stopped=starts.Length+1;
   for(int i=0;i<starts.Length;i++) {
    int w=starts[i],start=w-1;if(start<0||start+2>=words.Length)throw new NotSupportedException("Original spacing record leaves its buffer.");
    int count=Word(words[start]);if(count<2||start+count>=words.Length)throw new NotSupportedException("Original spacing record extent is invalid.");
    Func<int,float> p=delegate(int n){return n<=count?words[start+n]:0;};
    int code=Word(p(1));if(code>4&&code!=14&&!(scor4Numbers&&code==10&&count>=16&&p(16)==1)) {
     if(code<17)continue;
     // Original 2CB5:075D inspects the stored parameter count and P14/P15
     // bit patterns. It does not classify by drawing appearance or name.
     if(count>=14&&Bits(p(14))!=0||count>=15&&Bits(p(15))!=0)continue;
    }
    int itemStaff=Word(p(2));if(staff<itemStaff){stopped=i+1;break;}
    float x=p(3);if(Bits(x)<Bits(low)||Bits(x)>Bits(high))continue;
    bool selected=false;
    if(code==14) {
     float span=p(4);if(itemStaff<firstStaff||Bits(span)<0||p(5)>spanningLimit)continue;
     if(Bits(span)==0)span=one;
     selected=staff<(double)itemStaff+span;
    } else {
     if(cursor>i+1||itemStaff!=staff)continue;
     if(code==1)selected=Bits(p(7))>=0;
     else if(code==2)selected=Bits(p(6))>=0&&Bits(p(7))>=0;
     else if(code==3)selected=p(5)<=clefLimit;
     else if(code==4)selected=count>=7&&p(7)<=barLimit&&Bits(p(6))==0;
     else if(code==18){float size=p(7);if(Bits(size)==0)size=one;selected=Math.Abs((double)p(4)*size)<=drawingLimit;}
     else selected=true;
    }
    if(!selected)continue;
    if(positions.Count>=capacity||w>Int16.MaxValue)throw new NotSupportedException("Original spacing selection exceeds its buffer.");
    positions.Add(x);records.Add((short)w);
   }
   return new LayoutSpacingSelectionResult{Positions=positions.ToArray(),RecordWords=records.ToArray(),StoppedItem=stopped};
  }
 }
}
````

## LayoutSpacingWorkspace

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutSpacingWorkspace.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Compiler numeric constants recovered from original initialized data.
 // Four following descriptor words reproduce the measured DOS load for the
 // opt-in quarter-tone compatibility behavior, not portable font widths.
 // No resource, font or installation/serial data is embedded.
 public static class LayoutSpacingWorkspace {
  public static byte[] Create() {
   var data=new byte[65536];
   Buffer.BlockCopy(BitConverter.GetBytes(0x40900000u),0,data,0x383E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40A00000u),0,data,0x3842,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40354FDFu),0,data,0x3846,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x384A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x4019999Au),0,data,0x384E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40066666u),0,data,0x3852,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40800000u),0,data,0x3856,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40200000u),0,data,0x385A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x385E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x4059999Au),0,data,0x3862,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x3866,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40333333u),0,data,0x386A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40833333u),0,data,0x386E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40933333u),0,data,0x3872,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40833333u),0,data,0x3876,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40C66666u),0,data,0x387A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40900000u),0,data,0x387E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x00002910u),0,data,0x3882,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x00003729u),0,data,0x3886,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x002E2910u),0,data,0x388A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x362D3729u),0,data,0x388E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3A83126Fu),0,data,0x7748,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F800000u),0,data,0x774C,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x7750,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40A00000u),0,data,0x7754,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40E00000u),0,data,0x7758,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0xBF800000u),0,data,0x775E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3D23D70Au),0,data,0x7762,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x42C80000u),0,data,0x7766,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41200000u),0,data,0x776A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FC00000u),0,data,0x776E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F000000u),0,data,0x7772,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3DCCCCCDu),0,data,0x7776,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F333333u),0,data,0x777A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41A00000u),0,data,0x777E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40133333u),0,data,0x7782,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F666666u),0,data,0x7786,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FD9999Au),0,data,0x778A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40800000u),0,data,0x778E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41400000u),0,data,0x7792,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FA66666u),0,data,0x7796,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FE66666u),0,data,0x779A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40F00000u),0,data,0x779E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40D00000u),0,data,0x77A2,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40400000u),0,data,0x77A6,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3E4CCCCDu),0,data,0x77AA,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41700000u),0,data,0x77AE,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40200000u),0,data,0x77B2,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3CF5C28Fu),0,data,0x77B6,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40333333u),0,data,0x77BC,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x406CCCCDu),0,data,0x77C0,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F800000u),0,data,0x77E0,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0xC0800000u),0,data,0x77F6,4);
   return data;
  }
 }
}
````

## LayoutSymbolSpacing

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutSymbolSpacing.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutSymbolSpacingResult {
  public float Left,Right,Minimum,Scratch,ScaleScratch;public bool ResetParameter10Scratch;
 }
 // Original 2F68:08BD, 0A4F and 0CAF. Spacing values are the original
 // compiler literals, not measured renderer bounds. Caller record selection,
 // scale preparation and output-table assembly are separate operations.
 public static class LayoutSymbolSpacing {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static void Finite(params float[] values){foreach(float f in values)if(Single.IsNaN(f)||Single.IsInfinity(f))throw new NotSupportedException("Non-finite symbol-spacing input is outside this helper domain.");}
  // JUST 3.01, measured resident 0E64:089C..0963.
  internal static LayoutSymbolSpacingResult JustBar(short previous,short count,float p5){
   Finite(p5);
   long number;
   if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(p5),0),out number))
    throw new NotSupportedException("Bar style leaves the finite conversion domain.");
   short kind=unchecked((short)number);
   float left=previous==3||previous==17?0:1,right=2.5f;
   if(count>=5&&kind>0&&kind!=7){
    if(kind!=4){
     left+=1;
     if(kind==2)left+=1;
     if(kind>2)left+=4;
    }
    if(kind>3)right=6.5f;
   }
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Scratch=left};
  }
  public static LayoutSymbolSpacingResult Bar(short previous,short count,float p5,float p7,float effectiveP8,float p13,float p14) {
   Finite(p5,p7,effectiveP8,p13,p14);float right=2.5f,left=1;bool reset=false;
   long number;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(p5),0),out number))throw new NotSupportedException("Bar-style conversion is outside the recovered finite converter.");
   short kind=unchecked((short)number);
   if(count>=5&&kind>=1&&kind<=6) {
    reset=true;
    switch(kind) {
     case 1:left=(float)((double)effectiveP8*2.2f+p13);break;
     case 2:left=(float)((double)effectiveP8*3.3f+p13);break;
     case 3:left=(float)((double)effectiveP8*5.4f+p14+p13);break;
     case 4:left=(float)((double)effectiveP8*2);right=(float)((double)effectiveP8*6+p13+p14);break;
     case 5:left=(float)((double)effectiveP8*5.4f+p13+p14);right=(float)((double)effectiveP8*6+p13+p14);break;
     case 6:left=(float)((double)effectiveP8*4.3f+p14);right=(float)((double)effectiveP8*7+p13+p14);break;
    }
   }
   if(previous==3||previous==17)left=(float)((double)left-1);
   float scratch=left;
   if(p7!=0){scratch=p7;left=(float)((double)left-p7);right=(float)((double)p7+right);}
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Scratch=scratch,ResetParameter10Scratch=reset};
  }
  public static LayoutSymbolSpacingResult Key(short previous,float p7,float p5,bool standaloneJust=false) {
   Finite(p7,p5);float left=previous==1?2:0;
   float scratch=(float)Math.Abs((double)p5%100);float scale=Bits(p7)==0?1:p7;
   float right=previous==3&&!standaloneJust?5.5f:4.5f;
   right=(float)(((double)scratch-1)*scale*2.8f+right);
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Scratch=scratch,ScaleScratch=scale};
  }
  public static LayoutSymbolSpacingResult Meter(short previous,float minimum,float[] parameters,short count,bool standaloneJust=false) {
   if(parameters==null||count<1||count>parameters.Length)throw new NotSupportedException("Original meter-spacing extent leaves its record.");
   Finite(minimum);
   // This helper reads the raw backing buffer, unlike the preceding padded
   // record getter. The caller must supply actual adjacent words for any read
   // beyond the declared count; inventing zero padding would change SCORE.
   Func<int,float> p=delegate(int n){if(n<1||n>parameters.Length)throw new NotSupportedException("Original meter-spacing backing words are unavailable.");float value=parameters[n-1];Finite(value);return value;};
   float left=0,right=standaloneJust?6.5f:7.5f;
   if(previous==14){minimum=(float)((double)minimum-1.5f);left=.5f;}
   if(previous==1)left=2;
   if(previous==17&&!standaloneJust)left=-1.5f;
   float scratch=p(5),scaleScratch=0;
   if((p(6)>=10||scratch>=10)&&scratch<=90)right=(float)((double)right+2.5f);
   int last=count;while(last>0&&p(last)==0)last--;
   if(last==0)throw new NotSupportedException("Original meter-spacing scan leaves an all-zero record.");
   if(last>=8) {
    scratch=0;
    if(last<12||p(12)==0){scratch=4.5f;if(last>=10&&p(10)!=0)scratch=(float)((double)p(10)+scratch);}
    if(last>=11&&p(11)!=0)scratch=(float)((double)p(11)+scratch);
    scaleScratch=p(8)>10||p(9)>10?6:3.5f;
    right=(float)((double)right+scratch+scaleScratch);
   }
   return new LayoutSymbolSpacingResult{Left=left,Right=right,Minimum=minimum,Scratch=scratch,ScaleScratch=scaleScratch};
  }
 }
}
````

## LayoutTablatureAccidentalTable

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureAccidentalTable.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class LayoutTablatureAccidentalTable {
  //2C0C lookup DS:4062+2*delta; original finite signed remainder domain.
  //Original unpacked image with EXEPACK relocation, verified against live DS.
  public const int FirstDelta=-23;
  static readonly ushort[] raw={79,85,68,0,1,2,0,10,20,73,87,80,68,85,70,19787,20559,21330,22612,11610,20782,4,1,3,2,5,10512,0,12127,0,10512,70,12127,74,10512,122,12127,74,10512,176,12127,0,10512,224,12127,0,10512};
  public static short[] Recover(ushort loadSegment){var result=new short[raw.Length];for(int i=0;i<raw.Length;i++)result[i]=unchecked((short)raw[i]);result[28]=unchecked((short)(raw[28]+loadSegment));result[32]=unchecked((short)(raw[32]+loadSegment));result[36]=unchecked((short)(raw[36]+loadSegment));result[40]=unchecked((short)(raw[40]+loadSegment));result[44]=unchecked((short)(raw[44]+loadSegment));return result;}
 }
}
````

## LayoutTablatureChord

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureChord.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutTablatureChordNote {public float[] Row;public LayoutTablaturePitchPlan Plan;public int SortKey;}
 public static class LayoutTablatureChord {
  static int Bits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0);}
  static void Swap(float[] a,float[] b,int parameter){float value=a[parameter];a[parameter]=b[parameter];b[parameter]=value;}
  public static void Apply(List<LayoutTablatureChordNote> notes,int staffStrings){
   for(int first=0;first<notes.Count;){int last=first;while(last+1<notes.Count&&notes[last].Row[7]>=0&&Bits(notes[last+1].Row[2])==Bits(notes[first].Row[2]))last++;
    if(last>first){
     //2C0C:106C..1074 tests the stored marker words. Positive zero is
     //the empty marker, so this group never reaches the adjustment loop.
     if(Bits(notes[first].Row[2])==0){first=last+1;continue;}
     //Negative zero has a nonzero marker word and follows ordinary adjustment.
     //2C0C:10A8..11DF sorts the absolute-pitch word table and swaps only
     //P4/P5/P8 of the existing output slots, retaining other parameters.
     bool again;do{again=false;for(int n=first+1;n<=last;n++)if(notes[n-1].SortKey>notes[n].SortKey){Swap(notes[n-1].Row,notes[n].Row,3);Swap(notes[n-1].Row,notes[n].Row,4);Swap(notes[n-1].Row,notes[n].Row,7);int sort=notes[n-1].SortKey;notes[n-1].SortKey=notes[n].SortKey;notes[n].SortKey=sort;var plan=notes[n-1].Plan;notes[n-1].Plan=notes[n].Plan;notes[n].Plan=plan;again=true;}}while(again);
     //11DF..1313: make adjacent string positions distinct and contiguous.
     for(int n=last;n>first;n--){int lower=(int)notes[n-1].Row[7],upper=(int)notes[n].Row[7];if(upper>=lower){lower++;if(lower>staffStrings||lower>notes[n-1].Plan.Strings)throw new NotSupportedException("TAB chord string-capacity handler remains guarded.");notes[n-1].Row[7]=lower;}
      while(lower-(int)notes[n].Row[7]>1){for(int j=last;j>=n;j--){int next=(int)notes[j].Row[7]+1;if(next>notes[j].Plan.Strings)throw new NotSupportedException("TAB chord reaches an unverified tuning index.");notes[j].Row[7]=next;}}
     }
     //1314..1491: query the actual TAB helper for the chosen string/fret,
     //then move the recovered ranges while the positive-fret span exceeds5.
     var seen=new HashSet<string>();
     while(true){string state="";for(int n=first;n<=last;n++)state+=((int)notes[n].Row[7]).ToString()+",";if(!seen.Add(state))throw new NotSupportedException("Repeated original TAB chord-adjustment state is prevented.");
      int maximum=0,minimum=99,maximumString=0,maximumIndex=first;
      for(int n=first;n<=last;n++){int requested=(int)notes[n].Row[7],chosen=requested<0?requested:notes[n].Plan.SelectedString(requested);int fret=requested<0?0:unchecked((short)(notes[n].Plan.Absolute-notes[n].Plan.Tuning[chosen]));maximum=Math.Max(maximum,fret);if(fret>0)minimum=Math.Min(minimum,fret);if(chosen>=maximumString){maximumString=chosen;maximumIndex=n;}}
      if(maximum-minimum<=5)break;
      if(notes[last].Row[7]==1){for(int n=first;n<last;n++){if(notes[n].Row[7]==staffStrings)throw new NotSupportedException("Original TAB chord-capacity failure remains guarded.");notes[n].Row[7]++;}}
      else{for(int n=maximumIndex;n<=last;n++){if(notes[n].Row[7]<=0)throw new NotSupportedException("Original TAB chord adjustment reaches its zero-string failure handler.");notes[n].Row[7]--;}}
     }
    }
    first=last+1;
   }
  }
 }
}
````

## LayoutTablatureFrequency

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureFrequency.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 //3195:000D..0822, positive mode1, ordinary finite note/rest durations.
 //This leaf returns source chromatic words; it never changes MUS records.
 public static class LayoutTablatureFrequency {
  static readonly short[] clefs={0,-12,-6,-8,-7},degrees={0,0,2,4,5,7,9,11};
  static readonly short[,] keys={
   {-1,-1,-1,-1,-1,-1,-1},{-1,-1,-1,0,-1,-1,-1},{0,-1,-1,0,-1,-1,-1},
   {0,-1,-1,0,0,-1,-1},{0,0,-1,0,0,-1,-1},{0,0,-1,0,0,0,-1},{0,0,0,0,0,0,-1},
   {0,0,0,0,0,0,0},{0,0,0,1,0,0,0},{1,0,0,1,0,0,0},{1,0,0,1,1,0,0},
   {1,1,0,1,1,0,0},{1,1,0,1,1,1,0},{1,1,1,1,1,1,0},{1,1,1,1,1,1,1}};
  static void Reset(short[] carry,int key){
   if(key< -7||key>7)throw new NotSupportedException("TAB key outside the recovered table.");
   Array.Clear(carry,0,carry.Length);if(key==0)return;
   //Original counter2, then196 writes into indexed cells3..198.
   for(int i=3;i<=198;i++)carry[i]=keys[key+7,(i-3)%7];
  }
  public static Dictionary<int,short> Apply(TranspositionCollectorResult collection){
   var result=new Dictionary<int,short>();var carry=new short[200];int key=0;float previous=0;
   for(int i=0;i<collection.FirstPass.Records.Count;i++){
    var entry=collection.FirstPass.Records[i];int kind=collection.Partition.Kinds[i];
    if(kind==2){key=entry.Accidental;if(Math.Abs(key)>=100)key=0;Reset(carry,key);continue;}
    if(kind<0){previous=0;continue;}
    if(kind!=0&&BitConverter.ToInt32(BitConverter.GetBytes(previous),0)!=BitConverter.ToInt32(BitConverter.GetBytes(entry.Position),0))Reset(carry,key);
    if(!entry.Pitch.HasValue||entry.Clef<0||entry.Clef>=clefs.Length)throw new NotSupportedException("TAB pitch/clef outside the recovered table.");
    int absolute=unchecked((short)(entry.Pitch.Value+clefs[entry.Clef])),index=unchecked((short)(absolute+100));
    if(index<3||index>198)throw new NotSupportedException("TAB carry index outside verified initialized cells.");
    int degree=unchecked((short)(absolute+6))%7+1;if(degree<=0)degree+=7;
    if(entry.Accidental!=0)carry[index]=entry.Accidental==3?(short)0:entry.Accidental;
    result.Add(entry.Item,unchecked((short)(((absolute+34)/7)*12+degrees[degree]+carry[index])));previous=entry.Position;
   }
   return result;
  }
 }
}
````

## LayoutTablaturePitch

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablaturePitch.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutTablaturePitchPlan {
  public int Absolute,Strings;public int[] Tuning;
  public int SelectedString(int requested){if(requested<0||requested>Strings)throw new NotSupportedException("TAB requested string outside the recovered table.");int preferred=Strings;for(int n=1;n<=Strings;n++)if(Tuning[n]<=Absolute){preferred=n;break;}return Math.Max(preferred,requested);}
 }
 public static class LayoutTablaturePitch {
  //1BA1:0007..0318: original pitch, clef, accidental and tuning word tables.
  //Main TAB supplies the head selected at2C0C:0539..05C8. TP supplies its
  //edited head; its measured caller supplies zero tuning/capo arguments.
  // Drawing caller 1BA1:019B..02A9 has tuning replacement and capo rules
  // distinct from the previously measured TAB command input normalization.
  public static LayoutTablaturePitchPlan PrepareDrawing(int head,int pitch,int accidental,short[] packed,short capo){
   var plan=Prepare(head,pitch,accidental,null);bool bass=head/10==1;
   if(bass)for(int n=1;n<=plan.Strings;n++)plan.Tuning[n]=unchecked((short)(plan.Tuning[n]+12));
   for(int pair=0;packed!=null&&pair<packed.Length;pair++){
    int[] values={packed[pair]/100,packed[pair]%100};
    for(int part=0;part<2;part++){int index=pair*2+part+1,value=values[part];if(index>plan.Strings||value==0)continue;
     if(value>32)plan.Tuning[index]=value;else{if(value>=21)value=20-value;plan.Tuning[index]=unchecked((short)(plan.Tuning[index]+value));}
    }
   }
   for(int n=1;n<=plan.Strings;n++)plan.Tuning[n]=unchecked((short)(plan.Tuning[n]+capo-(bass?12:0)));
   return plan;
  }
  public static LayoutTablaturePitchPlan Prepare(int head,int pitch,int accidental,short[] packed){
   int variant=head/10,strings=head%10,octave=0;
   if(variant==8||variant==15||variant==9){octave=variant==8?7:variant==15?14:-7;variant=0;}
   if(variant<0||variant>=4||strings<1||strings>8||accidental<0||accidental>9)throw new NotSupportedException("TAB pitch plan outside the recovered automatic tables.");
   int[] clefs={0,-12,-6,-8},accidentals={0,-1,1,0,-2,2,0,2,4,5},degrees={2,0,2,4,5,7,9,11},table={40,76,71,67,62,57,52,47,47,0};
   int diatonic=pitch+octave+clefs[variant],degree=(diatonic+6)%7+1;if(degree<=0)degree+=7;
   int absolute=(diatonic+34)/7*12+degrees[degree]+accidentals[accidental],shift=6-strings;if(variant==1)shift=(shift+1)/2+1;shift=Math.Max(0,shift);var tuning=new int[9];
   for(int n=1;n<=strings;n++){if(n+shift>=table.Length)throw new NotSupportedException("TAB tuning index outside the recovered table.");tuning[n]=table[n+shift];}
   if(variant==1&&strings==6)tuning[1]=72;
   if(variant==0&&strings==4){tuning[1]=76;tuning[2]=69;tuning[3]=62;tuning[4]=55;}
   int capo=variant==1?-12:0;for(int n=1;n<=strings;n++)tuning[n]=unchecked((short)(tuning[n]+capo));
   if(packed!=null)for(int pair=0;pair<packed.Length;pair++){int left=packed[pair]/100,right=packed[pair]%100;if(left>=10)left=20-left;if(right>=10)right=20-right;int first=pair*2+1;if(first<=strings)tuning[first]=unchecked((short)(tuning[first]+left));if(first+1<=strings)tuning[first+1]=unchecked((short)(tuning[first+1]+right));}
   return new LayoutTablaturePitchPlan{Absolute=absolute,Strings=strings,Tuning=tuning};
  }
 }
}
````

## LayoutTablatureRelationships

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureRelationships.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 //Positive-mode3195:0822..0C54. The returned frequency words are the caller-visible
 //9D4 table; duration-end copies at0BFF have no read in the Main TAB caller.
 public static class LayoutTablatureRelationships {
  static int Bits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0);}
  public static void Apply(IList<float[]> rows,TranspositionCollectorResult collected,Dictionary<int,short> frequencies){
   var entries=collected.FirstPass.Records;var relationships=collected.FirstPass.RelationshipItems;if(relationships.Count==0||entries.Count==0)return;
   int count=entries.Count;var ids=new int[count+1];var pitch=new short[count+1];var clef=new short[count+1];var accidental=new short[count+1];var kind=new short[count+1];var x=new float[count+1];
   for(int n=1;n<=count;n++){var entry=entries[n-1];ids[n]=entry.Item;pitch[n]=entry.Pitch??0;clef[n]=entry.Clef;accidental[n]=entry.Accidental;kind[n]=collected.Partition.Kinds[n-1];x[n]=entry.Position;}
   var pendingIds=new int[11];var pendingPitches=new short[11];var pendingClefs=new short[11];int gathered=0,pending=0,pendingRead=0,index=1,baseId=0,search=0,target=0,steps=0;short basePitch=0,baseClef=0,active=(short)relationships.Count,cursor=0;bool advanceThenNext=false,changedPosition=false;float position=0,candidate=0,first=collected.FirstPass.FirstNotePosition??-999;var window=new TranspositionRelationshipWindow{StartLow=-999,StartHigh=-999};
  Entry:
   if(++steps>20000)throw new NotSupportedException("Repeated original TAB relationship state is prevented.");if(index>count)goto Done;if(kind[index]<0||kind[index]>1)goto Next;position=x[index];if(cursor==0)goto AdvanceAgain;
  Position:
   if(window.StartLow>position)goto Next;if(position<=window.StartHigh)goto Start;
   if(position>first||pending==0)goto AdvanceAgain;
   pendingRead++;if(pendingRead>gathered)throw new NotSupportedException("TAB relationship pending index outside recovered table.");if(pendingRead==pending)pending=0;basePitch=pendingPitches[pendingRead];baseClef=pendingClefs[pendingRead];baseId=pendingIds[pendingRead];position=-999;search=index;goto Search;
  Start:
   if(index<count)goto Prepare;if(++gathered>10)throw new NotSupportedException("TAB relationship pending table capacity is prevented.");pendingIds[gathered]=ids[index];pendingPitches[gathered]=pitch[index];pendingClefs[gathered]=clef[index];goto AdvanceNext;
  Prepare:
   if(index==count)goto Done;basePitch=pitch[index];baseClef=clef[index];baseId=ids[index];search=index+1;
   if(accidental[search]!=0&&Bits(x[search])!=Bits(position)&&(pitch[search]!=basePitch||accidental[index]!=accidental[search]))goto AdvanceNext;
  Search:
   changedPosition=false;
  SearchEntry:
   if(search>count)goto AdvanceNext;if(kind[search]==2||kind[search]<0)goto AdvanceNext;
   if(accidental[search]!=0)goto SearchNext;if(clef[search]!=baseClef)goto AdvanceNext;candidate=x[search];
   if(changedPosition){if(Bits(candidate)!=Bits(position))goto AdvanceNext;}else{if(Bits(candidate)==Bits(position))goto SearchNext;position=candidate;changedPosition=true;}
   if(pitch[search]!=basePitch)goto SearchNext;if(candidate<window.EndLow||candidate>window.EndHigh)goto AdvanceNext;
   target=ids[search];if(!frequencies.ContainsKey(target))throw new NotSupportedException("TAB relationship references a non-note frequency slot.");frequencies[target]=0;ids[search]=baseId;advanceThenNext=true;if(Bits(candidate)!=Bits(first))goto Advance;goto AdvanceAgain;
  SearchNext:
   if(++steps>20000)throw new NotSupportedException("Repeated original TAB relationship search is prevented.");search++;goto SearchEntry;
  AdvanceNext:advanceThenNext=true;goto Advance;
  AdvanceAgain:advanceThenNext=false;
  Advance:
   if(cursor==active)goto Done;InputTranspositionRelationshipWindow.Advance(rows,relationships,ref active,ref cursor,ref window,3,6);if(advanceThenNext)goto Next;goto Position;
  Next:index++;goto Entry;
  Done:pending=gathered;
  }
 }
}
````

## LayoutTextJustification

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTextJustification.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutTextJustificationWorkspace {public readonly float[] Positions=new float[200],Minimum=new float[200],Slack=new float[200];}
 public sealed class LayoutTextJustificationResult {public short PointCount;public JustificationGapResult Gaps;}
 // Original 2D31:0665 text-justification calculation after its text-level
 // prompt and sort. P11/P13 are the original stored text offset/width fields.
 public static class LayoutTextJustification {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Text justification leaves its finite word domain.");return (short)Math.Truncate(f);}
  public static LayoutTextJustificationResult Apply(float[] words,int[] starts,short bottom,short top,float left,float right,short textStaff,float level,float[] staffUnits,short staffThreshold,int capacity,LayoutTextJustificationWorkspace work){
   if(textStaff<1||textStaff>=staffUnits.Length)throw new NotSupportedException("Original text-justification staff index leaves its table.");
   float gap=(float)(((double)staffUnits[textStaff]/25)*1.5f);
   var references=LayoutCoordinateCollection.Collect(words,starts,bottom,top,left,right,0.001f,1,(short)starts.Length,staffThreshold,0,200,100,capacity).References;
   work.Positions[0]=left;work.Minimum[0]=0;float baseline=0;int count=1;
   for(int i=0;i<starts.Length;i++){
    int at=starts[i]-1,n=Word(words[at]);if(n<3||at+n>=words.Length)throw new NotSupportedException("Original short text-justification record is not verified.");int code=Word(words[at+1]),staff=Word(words[at+2]);float x=words[at+3];if(code!=16||x<left||x>right||staff<textStaff)continue;if(staff>textStaff)break;
    if(n<13)throw new NotSupportedException("Original text-justification metrics record is incomplete.");float p4=words[at+4];if(level!=99&&Math.Abs((double)p4-level)>1.9f)continue;
    if(Bits(baseline)==0)baseline=p4;else{if(Math.Abs((double)p4-baseline)>1)throw new NotSupportedException("Original JT reports text on more than one level.");words[at+4]=baseline;}
    if(count>=work.Positions.Length)throw new NotSupportedException("Original text-justification point capacity exceeded.");float offset=words[at+11];work.Minimum[count]=(float)((double)words[at+13]+gap+offset);if(Bits(offset)!=0)work.Minimum[count-1]=(float)((double)work.Minimum[count-1]-offset);work.Positions[count++]=x;
   }
   if(count==1)return new LayoutTextJustificationResult();
   if((double)right-work.Positions[count-1]>0.2f){if(count>=work.Positions.Length)throw new NotSupportedException("Original text-justification right boundary exceeds its table.");work.Positions[count++]=right;}
   int first=0;if((double)work.Positions[1]-left<=0.2f){first=1;count--;}
   var positions=new float[count];var minimum=new float[count];var slack=new float[count];Array.Copy(work.Positions,first,positions,0,count);Array.Copy(work.Minimum,first,minimum,0,count);
   var gaps=LayoutJustificationGaps.Prepare(slack,minimum,positions,(short)count,left,right);var coordinates=new JustificationCoordinates(words,references,-9999,0.2f,1);LayoutJustificationRedistribution.Apply(slack,positions,(short)count,gaps.PositiveSlack,gaps.NegativeSlack,coordinates);if(Bits(gaps.NegativeSlack)!=0&&Bits(gaps.PositiveSlack)!=0)LayoutJustificationSnap.Apply(words,starts,0.01f,200);
   Array.Copy(positions,0,work.Positions,first,count);Array.Copy(minimum,0,work.Minimum,first,count);Array.Copy(slack,work.Slack,count);return new LayoutTextJustificationResult{PointCount=(short)count,Gaps=gaps};
  }
 }
}
````

## LayoutVerticalCollision

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutVerticalCollision.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2C5C:0893..098F. Contours contain signed vector coordinates,
 // with separate empty sentinels. No renderer bounding boxes are consulted.
 public static class LayoutVerticalCollision {
  public static int Minimum(short[] upper,short[] lower,short radius) {
   if(upper==null||lower==null||upper.Length<101||lower.Length<101||radius<0)throw new NotSupportedException("Original VJ contour table/radius leaves the verified domain.");
   int minimum=99999;
   for(int i=0;i<101;i++)if(upper[i]!=32760){int end=Math.Min(100,i+radius),start=Math.Max(0,i-radius);for(int j=start;j<=end;j++)if(lower[j]!=-32760){int gap=(int)upper[i]-lower[j];if(gap<minimum)minimum=gap;}}
   return minimum;
  }
 }
}
````

## LayoutVerticalContour

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutVerticalContour.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class LayoutVerticalContour {
  static short W(int value){return unchecked((short)value);}
  static void Put(short[] minimum,short[] maximum,int bin,short low,short high){if(bin<0||bin>100)throw new NotSupportedException("Original VJ contour writes beyond the 101-cell table.");if(maximum[bin]<high)maximum[bin]=high;if(low<minimum[bin])minimum[bin]=low;}
  // Original 2C5C:057A..0892. Inputs are original one-based X/Y word
  // streams and item-vector offsets, not pixels or bounding rectangles.
  public static void Apply(short[] x,short[] y,int first,int next,bool skipGuide,bool initialize,short[] minimum,short[] maximum,short binWidth,short bias){
   if(x==null||y==null||minimum==null||maximum==null||minimum.Length<101||maximum.Length<101||binWidth<=0)throw new NotSupportedException("Original VJ contour input leaves its verified finite domain.");
   if(initialize)for(int i=0;i<101;i++){minimum[i]=32760;maximum[i]=-32760;}
   if(first==next)return;int start=first+(skipGuide?4:1),end=next-2;
   for(int i=start;i<=end;i++){
    if(i<1||i+1>=x.Length||i>=y.Length)throw new NotSupportedException("Original VJ vector read leaves its bank.");
    short rawA=x[i],rawB=x[i+1];if(rawA< -32600||rawB< -32600||(rawA<0&&rawB<0)||(rawA>30000&&rawB>30000))continue;
    int a=W(rawA+bias)/binWidth,b=W(rawB+bias)/binWidth;short high=y[i-1],low=y[i];
    if(a==b){if(high<low){short t=high;high=low;low=t;}}
    else if(high!=low){
     short distance=W((b-a)*binWidth);if(distance<1){a=b;short t=high;high=low;low=t;distance=W(-distance);}
     if(distance<=0)throw new NotSupportedException("Original VJ interpolation distance is not a positive finite word.");
     float slope=(float)((double)W(low-high)/distance);int count=W(distance+binWidth)/binWidth;short step=0;
     for(int n=0;n<count;n++,a++,step=W(step+binWidth))if(a>=0&&a<101){double value=(double)step*slope+high;if(value< -32768||value>=32768)throw new NotSupportedException("Original VJ interpolation conversion leaves signed16 domain.");short point=W((int)Math.Truncate(value));Put(minimum,maximum,a,point,point);}continue;
    }else if(a>b){int t=a;a=b;b=t;}
    for(int bin=a;;bin++){if(bin>=0){Put(minimum,maximum,bin,low,high);if(bin==b||bin==100)break;}if(bin>32767)throw new NotSupportedException("Original VJ contour iteration leaves its finite domain.");}
   }
  }
 }
}
````

## LayoutVerticalPlacement

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutVerticalPlacement.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutVerticalPlacementResult {public short Bottom,Top;public bool Repeat;public readonly float[] Offsets=new float[33];}
 public static class LayoutVerticalPlacement {
  static long Integer(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value< -9223372036854775808d||value>=9223372036854775808d)throw new NotSupportedException("Original VJ conversion leaves its finite integer domain.");return (long)Math.Truncate(value);}
  public static LayoutVerticalPlacementResult Apply(short[][] minimum,short[][] maximum,bool[] occupied,float[] scales,float[] bases,short bottom,short top,short verticalSteps,short radius,float staffGap,float[] retainedOffsets){
   if(bottom<1||top>32||top<bottom||scales.Length<33||bases.Length<33||occupied.Length<33||retainedOffsets.Length<33)throw new NotSupportedException("Original VJ staff traversal leaves its tables.");
   int first=999,last=0;for(int i=1;i<=32;i++)if(occupied[i]){first=Math.Min(first,i);last=Math.Max(last,i);}bottom=(short)Math.Max(bottom,first);top=(short)Math.Min(top,last);var result=new LayoutVerticalPlacementResult{Bottom=bottom,Top=top};Array.Copy(retainedOffsets,result.Offsets,33);if(bottom>=top)return result;
   int previous=bottom;float cumulative=0;
   for(int staff=bottom+1;staff<=top;staff++)if(occupied[staff]){
    float staffUnit=(float)((double)scales[staff]*7),smallerUnit=(float)((double)Math.Min(scales[staff],scales[previous])*7);int distance=LayoutVerticalCollision.Minimum(minimum[staff],maximum[previous],radius);
    if(distance!=99999){int required=unchecked((int)Integer((double)verticalSteps*smallerUnit));short floor=unchecked((short)Integer((double)staffGap*smallerUnit-((double)bases[staff]-bases[previous]-(double)smallerUnit*8)));int shift=unchecked(required-distance);if(shift<floor)shift=floor;cumulative=(float)((double)cumulative+shift);previous=staff;if(bases[staff]>=32100){cumulative=(float)(32000d-bases[staff]);result.Repeat=true;}}
    double delta=(double)cumulative/staffUnit;result.Offsets[staff]=Math.Abs(delta)<0.20000000298023224?0:(float)delta;
   }
   Array.Copy(result.Offsets,retainedOffsets,33);return result;
  }
 }
}
````
