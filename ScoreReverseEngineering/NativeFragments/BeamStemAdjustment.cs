using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class BeamStemAdjustment {
  // Live overlay routine 2EB6:0006-0A13; partial interval helper 0A16-0B0A.
  // Explicit binary32 stores follow the original, including its FST/non-pop stores.
  const float Head=2.833329916000366f,Cue=.6000000238418579f;
  static float F(double v){return (float)v;}static float P(float[] p,int n){return p.Length>=n?p[n-1]:0;}
  static int W(double v){if(double.IsNaN(v)||double.IsInfinity(v)||v<short.MinValue||v>=32768)throw new NotSupportedException("Original AD integer conversion outside the finite word domain.");return (short)Math.Truncate(v);}
  static void Set(List<float[]> rows,int index,int n,float value){var p=rows[index];if(p.Length<n){Array.Resize(ref p,n);rows[index]=p;}p[n-1]=value;}
  // SCOR4 selected-beam stem adjustment, 2591:0972..0A04.
  // The editor stores the stem attachment coordinates before interpolating
  // the two endpoint differences. Main AD uses a different slope calculation.
  public static List<float[]> ApplyScor4Selected(IList<float[]> source,int selected,bool french,bool includeOpposite,int onlyNote=-1){
   var rows=new List<float[]>(source);
   var beam=source[selected];
   int descriptor=W(P(beam,7)),direction=descriptor/10;
   if(descriptor<11||direction>2||descriptor%10==0||Math.Abs(P(beam,4))>=100||
      P(beam,8)!=0||P(beam,9)!=0||P(beam,10)!=0||P(beam,13)!=0||
      P(beam,14)!=0||P(beam,15)!=0||P(beam,18)!=0)
    throw new NotSupportedException("This SCOR4 selected-beam adjustment needs a measured extended beam case.");

   float anchor=P(beam,3),end=P(beam,6);
   float headOffset=direction==1?Head:0;
   float left=F((double)anchor+headOffset),right=F((double)end+headOffset);
   float span=F((double)right-left);
   if(span<=0)return rows;
   int staff=W(P(beam,2));
   float frenchThickness=F((descriptor%10-1)*1.5);
   for(int i=0;i<source.Count;i++){
    if(onlyNote>=0&&i!=onlyNote)continue;
    var note=source[i];
    if(W(P(note,1))!=1||W(P(note,2))!=staff||P(note,3)<anchor||P(note,3)>end)continue;
    int stem=W(P(note,5))%100/10;
    bool opposite=stem!=direction;
    if(stem<1||stem>2||opposite&&!includeOpposite)continue;
    if(Math.Abs(P(note,4))>=100||P(note,10)!=0||P(note,12)!=0||P(note,15)!=0)
     throw new NotSupportedException("This SCOR4 selected-beam adjustment needs a measured extended note case.");
    float position=F((double)P(note,3)+(stem==1?Head:0));
    float offset=F((double)position-left);
    float leftHeight=F((double)P(beam,4)-P(note,4));
    float rightHeight=F((double)P(beam,5)-P(note,4));
    if(stem==2){leftHeight=-leftHeight;rightHeight=-rightHeight;}
    float extension=F(((double)rightHeight-leftHeight)*((double)offset/span)+leftHeight);
    if(opposite)extension=F((double)extension+frenchThickness+1-14);
    if(french&&Math.Abs((double)position-left)>.10000000149011612&&Math.Abs((double)right-position)>.10000000149011612)
     extension=F((double)extension-frenchThickness);
    rows[i]=(float[])note.Clone();
    Set(rows,i,8,extension);
   }
   return rows;
  }

  public static List<float[]> ConfigureScor4Selected(IList<float[]> source,int selected,bool includeOpposite){
   var beam=source[selected];
   int direction=W(P(beam,7))/10,staff=W(P(beam,2));
   if(direction<1||direction>2||beam.Length>15||P(beam,8)!=0||P(beam,9)!=0)
    throw new NotSupportedException("This SCOR4 automatic beam needs a measured extended beam case.");

   var positions=new List<float>();
   var levels=new List<int>();
   var oppositePositions=new HashSet<float>();
   float[] durations={.5f,.25f,.125f,.0625f,.03125f,.015625f};
   foreach(var note in source){
    float x=P(note,3);
    if(W(P(note,1))!=1||W(P(note,2))!=staff||x<P(beam,3)||x>P(beam,6))continue;
    int stem=W(P(note,5))%100/10;
    if(stem!=direction&&!includeOpposite)continue;
    int level=Array.IndexOf(durations,P(note,7))+1;
    if(stem<1||stem>2||level==0||
       positions.Count!=0&&x<=positions[positions.Count-1])
     throw new NotSupportedException("This SCOR4 automatic beam needs a measured stem or rhythm case.");
    if(stem!=direction){
     if(x==P(beam,3)||x==P(beam,6))
      throw new NotSupportedException("This SCOR4 automatic beam needs a measured opposite-stem endpoint case.");
     oppositePositions.Add(x);
    }
    positions.Add(x);
    levels.Add(level);
   }
   if(positions.Count<2)return new List<float[]>(source);
   if(positions[0]!=P(beam,3)||positions[positions.Count-1]!=P(beam,6))
    throw new NotSupportedException("This SCOR4 automatic beam needs a measured offset endpoint case.");

   var prototype=new float[15];
   Array.Copy(beam,prototype,7);
   int minimum=levels[0];
   foreach(int level in levels)minimum=Math.Min(minimum,level);
   prototype[6]=direction*10+minimum;
   // SCOR4 packs complete inner runs from right to left. Isolated hooks
   // still point toward the adjacent note in display order.
   var reverseLevels=levels.ToArray();
   var reversePositions=positions.ToArray();
   Array.Reverse(reverseLevels);
   Array.Reverse(reversePositions);
   var packed=InputBeamLevels.Pack(prototype,reverseLevels,reversePositions);
   if(packed.Count!=1)throw new NotSupportedException("Multiple SCOR4 automatic beam records need original verification.");
   var updated=packed[0];
   int count=7;
   for(int control=9;control<=12;control+=3){
    if(updated[control]==0)continue;
    float left=updated[control+1],right=updated[control+2];
    if(left==0||right==0){float swap=left;left=right;right=swap;}
    int reversal=(oppositePositions.Contains(left)?1:0)+(oppositePositions.Contains(right)?2:0);
    updated[8]+=reversal*(control==9?10:1);
    updated[control+1]=left==positions[0]?-1:left;
    updated[control+2]=right==positions[positions.Count-1]?-1:right;
    count=control+3;
   }
   Array.Resize(ref updated,count);
   var rows=new List<float[]>(source);
   rows[selected]=prototype;
   var adjusted=ApplyScor4Selected(rows,selected,false,includeOpposite);
   adjusted[selected]=updated;
   return adjusted;
  }
  struct Partial {public float Left,Right,Inside,Outside;}
  static Partial Interval(float[] beam,int leftField){
   var result=new Partial();if(beam.Length<leftField+1)return result;
   float left=P(beam,leftField),right=P(beam,leftField+1);
   if(left==0||right==0)return result;
   result.Left=left<0?P(beam,3):left;result.Right=right<0?P(beam,6):right;
   int descriptor=W(P(beam,leftField-1));result.Inside=F((descriptor%10)*1.5);
   if(descriptor<0){result.Outside=-result.Inside;result.Inside=0;}return result;
  }
  public static List<float[]> Apply(IList<float[]> source,int staff,bool force,PageCoordinates tables,int maximumStaff,int onlyBeam=-1,ISet<int> selectedBeams=null){
   return Apply(source,staff,force,tables,maximumStaff,onlyBeam,selectedBeams,false);
  }
  public static List<float[]> Apply(IList<float[]> source,int staff,bool force,PageCoordinates tables,int maximumStaff,int onlyBeam,ISet<int> selectedBeams,bool page4){
   var rows=new List<float[]>();foreach(var p in source)rows.Add((float[])p.Clone());float previousLeft=0,previousPitch=0;
   for(int i=0;i<rows.Count;i++){
    var beam=rows[i];if(onlyBeam>=0&&i!=onlyBeam||selectedBeams!=null&&!selectedBeams.Contains(i)||W(P(beam,1))!=6||beam.Length<7||staff<=maximumStaff&&W(P(beam,2))!=staff)continue;
    int descriptor=W(P(beam,7));if(descriptor<0)continue;int direction=descriptor/10;
    if(direction!=1&&direction!=2)throw new NotSupportedException("AD beam direction outside the documented beam domain.");
    int beamStaff=W(P(beam,2)),cueState=-1;float beamScale=1;
    if(P(beam,18)!=0){beamScale=P(beam,18);cueState=1;}
    else if(Math.Abs(P(beam,4))>=100){beamScale=Cue;cueState=0;}
    float pitch=F((double)P(beam,4)%100),left=F((double)P(beam,3)-.009999999776482582),right=F((double)P(beam,6)+.009999999776482582);
    int reverse=W((double)P(beam,9)/100);float reverseLeft=0,reverseRight=0;
    if(reverse!=0){if(reverse!=200)reverseLeft=Head;if(reverse==200||reverse==300)reverseRight=Head;}
    if(direction==1){reverseLeft=-reverseLeft;reverseRight=-reverseRight;}
    float reverseSpan=F((double)reverseLeft+reverseRight),scaledReverse=F((double)beamScale*reverseSpan);
    double anchor=page4?P(beam,3):left;
    double span=page4?(double)P(beam,6)-anchor+scaledReverse:F((double)right-left+scaledReverse);
    if(span==0)return rows;
    double slope=((double)P(beam,5)-pitch)/span;
    if(!page4)slope=F(slope);
    double intercept=(double)pitch-anchor*slope;
    if(!page4)intercept=F(intercept);
    bool negativePartial=P(beam,10)<0||P(beam,13)<0;
    Partial secondary=Interval(beam,11),tertiary=Interval(beam,14);
    for(int n=0;n<rows.Count;n++){
     var note=rows[n];if(W(P(note,1))!=1||W(P(note,2))!=beamStaff||note.Length<5||P(note,3)<left||P(note,3)>right)continue;
     int stem=(W(P(note,5))%100)/10;if(stem==0)continue;bool same=stem==direction;if(!same&&!force)continue;
     float noteScale=P(note,15)!=0?P(note,15):1,rawPitch=P(note,4),notePitch=rawPitch;
     if(Math.Abs(rawPitch)>=100){if(cueState!=0)continue;noteScale=Cue;notePitch=F((double)rawPitch%100);}
     else if(cueState==0)continue;
     float shift=same?0:direction==1?-Head:Head;
     float extension=F(((double)P(note,3)+(double)noteScale*shift)*slope+intercept-notePitch);
     if(!same){float length=F((double)noteScale*14);if(direction==1)length=-length;extension=F((double)length-extension);}
     float extra=0;
     if(!same||negativePartial){
      if(!same)extra=F((descriptor%10-1)*1.5);
      float x=P(note,3);
      if(x>=secondary.Left&&x<=secondary.Right)extra=F((double)extra+(same?secondary.Outside:secondary.Inside));
      if(x>=tertiary.Left&&x<=tertiary.Right)extra=F((double)extra+(same?tertiary.Outside:tertiary.Inside));
      extra=F((double)extra*noteScale);
     }
     if(direction==2)extension=-extension;
     // Original same-direction/ordinary beams jump past the extra term.
     // Adding a synthetic +0 here would erase the FCHS result's -0.
     if(!same||negativePartial)extension=F((double)extra+extension);
     int cross=W(P(note,12));
     if(cross==1||cross==2){int neighbor=beamStaff+(cross==1?1:-1);if(neighbor<1||neighbor>maximumStaff)Set(rows,n,12,0);else{
      float displacement=F(((double)tables.Bases[beamStaff]-tables.Bases[neighbor])/((double)tables.Scales[beamStaff]*7));
      displacement=Math.Abs(displacement);if(cross==stem)displacement=-displacement;extension=F((double)extension+displacement);
     }}
     if(cueState==0&&!same)extension=F((double)extension+5.599999904632568);
     extension=F((double)extension/beamScale);
     // PAGE4's normalization buffer stores positive zero on a level
     // endpoint. The original Main AD path above retains the FCHS sign.
     if(page4&&extension==0f)extension=0f;
     if(left!=previousLeft||pitch!=previousPitch||P(note,8)<extension)Set(rows,n,8,extension);
     if(note.Length>=9){int flag=W(P(note,9))%10;if(flag!=0)Set(rows,n,9,F((double)P(note,9)-flag));}
    }
    previousLeft=left;previousPitch=pitch;
   }
   return rows;
  }
 }
}
