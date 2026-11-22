# Recovered machine, geometry and output routines

The listings reproduce the local source verbatim. Source comments state the scope of each model.

## AutomaticPrintOffset

[Workspace source](../../ScoreReverseEngineering/NativeFragments/AutomaticPrintOffset.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 // Original relative 27A1:06E3..06F2 and 2608:10DF..112A.
 // This fragment consumes an already measured vector minimum. It does not
 // establish which drawing requests belong in the original minimum scan.
 public static class AutomaticPrintOffset {
  public static int FromMinimumWord(short minimumY) {
   return unchecked((short)(-24000-minimumY));
  }
  public static int SelectBottom(int preliminary) {
   return preliminary>2200 ? unchecked(preliminary+800) : 3000;
  }
 }
}
````

## BeamBufferEdit

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BeamBufferEdit.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class BeamBufferEdit {
  // Original relative 00FD:0632..06C3. Includes both signed guards and
  // ordered segment-pointer reloads. Does not interpret the resulting shape.
  public static SignedDivide16State Execute(IPointBufferMemory m,SignedDivide16State s){
   if(m==null)throw new ArgumentNullException("m");
   if(s.IP!=0x632)throw new ArgumentException("Beam edit entry must be 0632.");
   s.ES=m.ReadWord(s.DS,0x6650);s.AX=m.ReadWord(s.ES,0);m.WriteWord(s.DS,0x8202,s.AX);
   s.SI=unchecked((ushort)(s.AX<<1));s.ES=m.ReadWord(s.DS,0x6652);s.BX=unchecked((ushort)(s.SI-2));
   s.DI=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.DI,0x7f58);
   if((short)s.DI>0x7f58){s.IP=0x6c4;return s;}
   s.BX=unchecked((ushort)(s.SI-4));ushort other=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));
   s.EFlags=RasterStatusFlags.Sub16(s.EFlags,other,0x7f58);
   if((short)other>0x7f58){s.IP=0x6c4;return s;}
   s.ES=m.ReadWord(s.DS,0x6650);s.BX=s.SI;s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));
   s.BX=unchecked((ushort)(s.SI+2));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.ES=m.ReadWord(s.DS,0x6652);s.BX=s.SI;s.AX=m.ReadWord(s.DS,0xbf84);
   s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,s.DI);s.AX=unchecked((ushort)(s.AX+s.DI));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.ES=m.ReadWord(s.DS,0x6650);s.BX=unchecked((ushort)(s.SI-2));s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));
   s.BX=unchecked((ushort)(s.SI+4));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.SI=unchecked((ushort)(m.ReadWord(s.DS,0x8202)<<1));s.ES=m.ReadWord(s.DS,0x6652);s.BX=unchecked((ushort)(s.SI-4));
   s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));ushort delta=m.ReadWord(s.DS,0xbf84);
   s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,delta);s.AX=unchecked((ushort)(s.AX+delta));
   s.BX=unchecked((ushort)(s.SI+2));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.ES=m.ReadWord(s.DS,0x6650);s.AX=m.ReadWord(s.DS,0x8202);s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,2);
   s.AX=unchecked((ushort)(s.AX+2));m.WriteWord(s.ES,0,s.AX);s.IP=0x6c4;return s;
  }
 }
}
````

## BeamGeometry

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BeamGeometry.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public enum BeamAppearance {Printed,DosScreen}
 public struct BeamVertex {public float X,Y;public BeamVertex(float x,float y){X=x;Y=y;}}
 public sealed class BeamShape {public BeamVertex[] Vertices;public int Group;}
 public struct BeamReference {public float LeftX,LeftY,Dx,Dy,StaffScale;public bool Cue;}
 public sealed class BeamGeometryContext {
  public float Scale,LeftVertical;
  // Original caller's staff-coordinate transformation, supplied explicitly.
  public Func<float,float> Vertical;
 }
 public static class BeamGeometry {
  static float F(double v){return (float)v;}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static bool Same(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}
  static float P(float[] p,int n){return n<p.Length?p[n]:0;}
  static int I(float v){return unchecked((short)(int)Math.Truncate(v));}
  static float Offset(float v,float scale){if(v==10||v==20){float a=F((double)scale*2.8333332538604736f/25);return v==20?-a:a;}return F((double)scale*v*0.03999999910593033f);}
  static void Adjust(ref float left,ref float right,int modifier,float shift){if(modifier==0)return;if(modifier!=2)left=F((double)left+shift);if(modifier==2||modifier==3)right=F((double)right+shift);}
  // Geometry/data lift of the beam and tremolo paths at relative 00FD:000A.
  // Context preparation, CPU flags,
  // x87 status/exception delivery and tuplet glyph output are separate layers.
  public static List<BeamShape> Build(float[] p,BeamGeometryContext c,BeamAppearance appearance){BeamReference reference;return Build(p,c,appearance,out reference);}
  public static List<BeamShape> Build(float[] p,BeamGeometryContext c,BeamAppearance appearance,out BeamReference reference){
   reference=new BeamReference();
   if(p==null||c==null||c.Vertical==null)throw new ArgumentNullException();
   int type=I(P(p,7)),direction=Math.Abs(type)/10,count=Math.Abs(type)%10;
   if(direction!=1&&direction!=2)throw new NotSupportedException("Unverified beam direction.");
   bool tremolo=type<0,floating=type>0&&count==0;
   float scale=c.Scale;if(Present(P(p,18)))scale=F((double)scale*P(p,18));
   // Preserve the original P4 cache/cue result at 0037..0076, including
   // its measured non-cue result when an explicit P18 is present.
   float cue=!Present(P(p,18))&&Math.Abs(P(p,4))>=100?.6000000238418579f:1;
   float staffScale=scale;scale=F((double)scale*cue);float spacing=F((double)scale*10.5),rise=F((double)scale*49),head=F((double)scale*17);
   float left=P(p,3),right=P(p,6);if(I(P(p,13))==0){if(P(p,14)!=0)left=F((double)left+Offset(P(p,14),scale));if(P(p,15)!=0)right=F((double)right+Offset(P(p,15),scale));}
   float baseLeft=left,baseRight=right;left=F((double)left*150);right=F((double)right*150);
   float baseShift=direction==1?head:0,reverseShift=direction==1?-head:head;
   if(direction==1){left=F((double)left+head);right=F((double)right+head);}else{rise=-rise;spacing=-spacing;}
   int modifier=I(P(p,9));Adjust(ref left,ref right,modifier/100,reverseShift);
   float ly=F((double)c.LeftVertical+rise),ry=F((double)c.Vertical(P(p,5))+rise);
   float width=F((double)scale*7);float factor=P(p,17);if(!Present(factor)&&cue!=1)factor=.800000011920929f;if(Present(factor))width=F((double)width*factor);
   if(direction==1)width=-width;
   float tremLeft=0,tremRight=0;bool fixedAngle=false;
   if(tremolo){
    float half=F((double)P(p,12)*.5);if(!Present(half))half=2;
    float halfScore=F((double)half*scale/25),center=P(p,11);
    if(I(center)==0||Same(left,right)){
     fixedAngle=I(center)==0;if(fixedAngle)center=baseLeft;
     if(fixedAngle){double height=(double)scale*2.916599988937378f*half;
      ry=F(height+ly);ly=F((double)ly-F(height));}
     right=F((double)cue*1000+left);
    }
    tremLeft=F((double)center-halfScore);tremRight=F((double)center+halfScore);
   }
   float refLeft=left,refRight=right,refLY=ly,refRY=ry;
   float dx=F((double)right-left);if(!Present(dx))dx=100;
   float dy=F((double)ry-ly);reference=new BeamReference{LeftX=left,LeftY=ly,Dx=dx,Dy=dy,StaffScale=staffScale,Cue=cue!=1};
   float slope=F((double)dy/dx),intercept=F((double)ly-(double)slope*left);
   var result=new List<BeamShape>();
   Action<int,int,float,float,float,float> emit=(group,n,x1,x2,y1,y2)=>{
    for(int k=0;k<n;k++){
     float delta=appearance==BeamAppearance.Printed?width:(direction==1?-60:60);
     // Screen-buffer additions operate on truncated integer words, not float32.
     float a=y1,b=y2,aa,bb;
     if(appearance==BeamAppearance.DosScreen){a=(short)(int)a;b=(short)(int)b;aa=unchecked((short)((short)a+(short)delta));bb=unchecked((short)((short)b+(short)delta));x1=(short)(int)x1;x2=(short)(int)x2;}
     else{aa=F((double)a+delta);bb=F((double)b+delta);}
     result.Add(new BeamShape{Group=group,Vertices=new[]{new BeamVertex(x2,bb),new BeamVertex(x2,b),new BeamVertex(x1,a),new BeamVertex(x1,aa)}});
     y1=F((double)y1-spacing);y2=F((double)y2-spacing);
    }
   };
   bool emptyFloating=floating&&I(P(p,10))==0;
   if(!tremolo&&(!floating||emptyFloating))emit(0,Math.Max(1,count),left,right,ly,ry);
   if(tremolo||(floating&&!emptyFloating)){
    int packed=I(P(p,10)),n=Math.Max(1,Math.Abs(packed%10));
    float a=tremLeft,b=tremRight;
    if(floating){
     float headScore=F((double)baseShift/150);
     a=F((double)scale*.11332999914884567f+P(p,11)-headScore+baseLeft);
     b=F((double)P(p,12)-headScore+baseRight);
     float inset=F((double)scale*.14000000059604645f);
     if(!Present(P(p,11)))a=F((double)baseLeft+inset);
     if(!Present(P(p,12)))b=F((double)baseRight-inset);
    }
    if(tremolo&&I(P(p,11))==-1)a=baseLeft;
    float x1=F((double)a*150+baseShift),x2=F((double)b*150+baseShift);
    int firstModifier=tremolo?(modifier/100!=0?1:0):(modifier%100)/10;
    Adjust(ref x1,ref x2,firstModifier,reverseShift);
    float offset=F((double)spacing*(packed/10)),y1,y2;
    if(fixedAngle&&!Same(refLY,refRY)){y1=refLY;y2=refRY;}
    else if(!Same(refLY,refRY)){y1=F((double)slope*x1+intercept-offset);y2=F((double)slope*x2+intercept-offset);}
    else{y1=F((double)refLY-offset);y2=F((double)refRY-offset);}
    emit(1,n,x1,x2,y1,y2);
   }
   for(int group=(tremolo||floating)?2:1;group<=2;group++){
    int at=group==1?10:13,packed=I(P(p,at)),n=Math.Max(1,Math.Abs(packed%10));if(packed==0)continue;
    float a=P(p,at+1),b=P(p,at+2);int ia=I(a),ib=I(b);
    if(ia==-1)a=baseLeft;if(ib==-1)b=baseRight;
    float x1=refLeft,x2=refRight;
    if(group==1&&!Present(P(p,11))&&!Present(P(p,12))){a=F((double)baseLeft+F((double)scale*.14000000059604645f));b=F((double)baseRight-F((double)scale*.14000000059604645f));ia=ib=1;}
    if(ia!=0)x1=F((double)a*150+baseShift);if(ib!=0)x2=F((double)b*150+baseShift);
    int mod=group==1?(modifier%100)/10:modifier%10;Adjust(ref x1,ref x2,mod,reverseShift);
    float stub=F((double)scale*16.200000762939453f);if(ib==0)x2=F((double)x1+stub);if(ia==0)x1=F((double)x2-stub);
    float offset=F((double)spacing*(packed/10));
    float y1,y2;if(!Same(refLY,refRY)){y1=F((double)slope*x1+intercept-offset);y2=F((double)slope*x2+intercept-offset);}else{y1=F((double)refLY-offset);y2=F((double)refRY-offset);}
    emit(group,n,x1,x2,y1,y2);
   }
   return result;
  }
 }
}
````

## BeamStemAdjustment

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BeamStemAdjustment.cs)

````csharp
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
````

## BiosKeyReader

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BiosKeyReader.cs)

````csharp
using System;

namespace ScoreModern.RecoveredFragments
{
    public interface IBiosKeyContext
    {
        // Explicit dependency for original INT 16h/AH=00h. No Windows-to-BIOS
        // mapping or original hardware/timing behavior is supplied here.
        ushort ReadBiosKey();
        void WriteByte(ushort segment, ushort offset, byte value);
    }

    public static class BiosKeyReader
    {
        // Original root 16FA:0841, 23 bytes. Ports the BIOS result and ordered
        // output-byte effects, not the complete register/flag/stack ABI.
        public static void Execute(IBiosKeyContext context, ushort asciiSegment,
            ushort asciiOffset, ushort scanSegment, ushort scanOffset)
        {
            if (context == null) throw new ArgumentNullException("context");
            ushort ax = context.ReadBiosKey();
            context.WriteByte(asciiSegment, asciiOffset, (byte)(ax & 255));
            context.WriteByte(scanSegment, scanOffset, (byte)(ax >> 8));
        }
    }
}
````

## BufferedBeamLinePipeline

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BufferedBeamLinePipeline.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class BufferedBeamLinePipeline {
  sealed class Words:IPointBufferMemory {
   readonly IHorizontalRasterMemory memory;
   public Words(IHorizontalRasterMemory m){memory=m;}
   public ushort ReadWord(ushort s,ushort o){return memory.ReadWord(s,o);}
   public void WriteWord(ushort s,ushort o,ushort v){memory.WriteWord(s,o,v);}
  }
  // Compose the verified normal decoder coordinate block, its actual far-call
  // frame, and the complete flag-aware native driver through RETF. Entry 004F,
  // normal exit in the original decoder at 0074. No host graphics/ROM embedded.
  public static LineDriverBodyResult Execute(IByteOrResultMemory memory,SignedDivide16State state,ushort driverSegment){
   if(memory==null)throw new ArgumentNullException("memory");
   state=BufferedCoordinateDecode.Execute(new Words(memory),state);
   state.SP=unchecked((ushort)(state.SP-2));memory.WriteWord(state.SS,state.SP,state.CS);
   state.SP=unchecked((ushort)(state.SP-2));memory.WriteWord(state.SS,state.SP,0x74);
   state.CS=driverSegment;state.IP=0x738;
   return LineDriverBody.ExecuteThroughReturnWithFlags(memory,state);
  }
 }
}
````

## BufferedCoordinateDecode

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BufferedCoordinateDecode.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class BufferedCoordinateDecode {
  static void Divide(ref SignedDivide16State s,short divisor){
   int dividend=(int)(((uint)s.DX<<16)|s.AX);
   if(divisor==0)throw new DivideByZeroException("Original decoder divide fault requires external delivery.");
   long q=(long)dividend/divisor;if(q<short.MinValue||q>short.MaxValue)throw new OverflowException("Original decoder quotient fault requires external delivery.");
   s.AX=unchecked((ushort)q);s.DX=unchecked((ushort)(dividend%divisor));
  }
  // Relative 1BF3:004F..006E, normal coordinate path through the call site.
  // Full successful-exit flags are defined by the final ADD. Fault delivery
  // and the enclosing loop/driver call are outside this entry point.
  public static SignedDivide16State Execute(IPointBufferMemory m,SignedDivide16State s){
   if(m==null)throw new ArgumentNullException("m");if(s.IP!=0x4f)throw new ArgumentException("Decoder coordinate entry must be 004F.");
   s.DX=(short)s.AX<0?(ushort)65535:(ushort)0;Divide(ref s,(short)m.ReadWord(s.DS,0x4b7e));
   ushort add=m.ReadWord(s.DS,0x4b7a);s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,add);s.AX=unchecked((ushort)(s.AX+add));m.WriteWord(s.DS,0xac40,s.AX);
   s.EFlags=RasterStatusFlags.Dec16(s.EFlags,s.SI);s.SI=unchecked((ushort)(s.SI-1));s.EFlags=RasterStatusFlags.Dec16(s.EFlags,s.SI);s.SI=unchecked((ushort)(s.SI-1));
   s.BX=m.ReadWord(s.SS,unchecked((ushort)(s.BP+6)));s.ES=m.ReadWord(s.SS,unchecked((ushort)(s.BP+8)));
   s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX+s.SI)));s.DX=(short)s.AX<0?(ushort)65535:(ushort)0;Divide(ref s,(short)s.DI);
   s.EFlags=RasterStatusFlags.Sub16(s.EFlags,0,s.AX);s.AX=unchecked((ushort)(0-s.AX));add=m.ReadWord(s.DS,0x4b7c);
   s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,add);s.AX=unchecked((ushort)(s.AX+add));m.WriteWord(s.DS,0xac42,s.AX);s.IP=0x6f;return s;
  }
 }
}
````

## BufferedDrawControl

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BufferedDrawControl.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IBufferedDrawMemory {ushort ReadWord(ushort segment,ushort offset);void WriteWord(ushort segment,ushort offset,ushort value);}
 // AX is projected by every method. CX is projected only by SelectRasterPath
 // and UpdatePosition; its default in DecodeMarker is not an original CX value.
 // Branch is meaningful only for DecodeMarker and SelectRasterPath.
 public struct BufferedDrawWords {public ushort AX,CX;public bool Branch;}
 public static class BufferedDrawControl {
  // 1BF3:0041..004D. Branch=true is the encoded-marker path.
  public static BufferedDrawWords DecodeMarker(IBufferedDrawMemory m,ushort ds,ushort ax){
   if(m==null)throw new ArgumentNullException("m");bool marker=unchecked((short)ax)<=unchecked((short)0x80a8);
   if(marker){ax=unchecked((ushort)(ax+0x7fbc));m.WriteWord(ds,0xac48,ax);}
   return new BufferedDrawWords{AX=ax,Branch=marker};
  }
  // 1807:0738..0743. Branch=true selects the following skip-raster far jump.
  public static BufferedDrawWords SelectRasterPath(IBufferedDrawMemory m,ushort ds){
   if(m==null)throw new ArgumentNullException("m");ushort ax=m.ReadWord(ds,0xac42),cx=m.ReadWord(ds,0xac48);
   return new BufferedDrawWords{AX=ax,CX=cx,Branch=unchecked((sbyte)(byte)cx)>0};
  }
  // 1807:0B1C..0B26, before RETF. Explicit saved-position data effects only.
  public static BufferedDrawWords UpdatePosition(IBufferedDrawMemory m,ushort ds,ushort ax){
   if(m==null)throw new ArgumentNullException("m");ushort cx=m.ReadWord(ds,0xac40);m.WriteWord(ds,0xac44,cx);m.WriteWord(ds,0xac46,ax);
   return new BufferedDrawWords{AX=ax,CX=cx};
  }
 }
}
````

## BufferedPointCaller

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BufferedPointCaller.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IBufferedPointFpuEffects:ICoordinateFpuEffects {
  void LoadBinary32(IHorizontalRasterMemory memory,ushort segment,ushort offset);
 }
 public enum BufferedPointOutcome {Completed,CapacityHandlerRequired,NormalPathReady,ExternalControlTransfer}
 public struct BufferedPointCallerResult {
  public SignedDivide16State State;public BufferedPointOutcome Outcome;public ushort RequestedCalleeOffset;public bool RetfExecuted;
 }
 public static class BufferedPointCaller {
  sealed class AppendMemory:IPointBufferMemory {
   readonly IHorizontalRasterMemory memory;public AppendMemory(IHorizontalRasterMemory m){memory=m;}
   public ushort ReadWord(ushort s,ushort o){return memory.ReadWord(s,o);}
   public void WriteWord(ushort s,ushort o,ushort v){memory.WriteWord(s,o,v);}
  }
  static void LoadPointer(IHorizontalRasterMemory m,ref SignedDivide16State s,ushort offset){ushort at=(ushort)(s.BP+offset);s.BX=m.ReadWord(s.SS,at);s.ES=m.ReadWord(s.SS,(ushort)(at+2));}
  static SignedDivide16State Convert(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,SignedDivide16State s,ushort converterSegment,ushort returnOffset){
   fpu.LoadBinary32(m,s.ES,s.BX);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,returnOffset);
   s.CS=converterSegment;s.IP=0x0c70;return CoordinateConversion.ExecuteThroughReturn(m,fpu,s);
  }
  // Relative 0C25:0752..084E. The unported capacity callee is an explicit
  // request boundary, not an invented error or buffer-drain policy.
  public static BufferedPointCallerResult Enter(IHorizontalRasterMemory m,SignedDivide16State s){
   if(s.IP!=0x0752)throw new ArgumentException("Point caller entry must be 0752.");
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.SI);
   s.ES=m.ReadWord(s.DS,0x69da);s.AX=m.ReadWord(s.DS,0x80de);ushort count=m.ReadWord(s.ES,0);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,count,s.AX);
   if((short)count>=(short)s.AX){s.IP=0x0764;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.CapacityHandlerRequired,RequestedCalleeOffset=0x084f};}
   s.IP=0x076c;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.NormalPathReady};
  }
  public static BufferedPointCallerResult Execute(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,SignedDivide16State s,ushort converterSegment){
   BufferedPointCallerResult gate=Enter(m,s);if(gate.Outcome==BufferedPointOutcome.CapacityHandlerRequired)return gate;s=gate.State;
   LoadPointer(m,ref s,14);s=Convert(m,fpu,s,converterSegment,0x0778);m.WriteWord(s.DS,0xc590,s.AX);m.WriteWord(s.DS,0xc592,s.DX);
   LoadPointer(m,ref s,10);s=Convert(m,fpu,s,converterSegment,0x078b);m.WriteWord(s.DS,0xc594,s.AX);m.WriteWord(s.DS,0xc596,s.DX);
   s.EFlags=CoordinateWordClamp.Execute(m,s.DS,s.EFlags);
   PointBufferWords appended=PointBufferAppend.ExecuteWithFlags(new AppendMemory(m),s.DS,s.SS,s.BP,s.EFlags);
   s.AX=appended.AX;s.BX=appended.BX;s.SI=appended.SI;s.ES=appended.ES;s.EFlags=appended.EFlags;
   s.SI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x084c;
   return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.Completed};
  }
  static BufferedPointCallerResult RunCapacity(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   ushort self=s.CS;s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,self);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,0x0769);s.IP=0x084f;
   CapacityMessageResult handler=CapacityMessageHandler.ExecuteThroughReturn(m,effects,s,stringSegment,followupSegment);s=handler.State;
   if(handler.Outcome!=CapacityMessageOutcome.ReachedReturn||s.CS!=self||s.IP!=0x0769)return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.ExternalControlTransfer};
   s.SI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x084c;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.Completed};
  }
  public static BufferedPointCallerResult ExecuteWithCapacityEffects(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,ICapacityCallEffects effects,SignedDivide16State s,ushort converterSegment,ushort stringSegment,ushort followupSegment){
   BufferedPointCallerResult r=Execute(m,fpu,s,converterSegment);return r.Outcome==BufferedPointOutcome.CapacityHandlerRequired?RunCapacity(m,effects,r.State,stringSegment,followupSegment):r;
  }
  // Also exposes the measured comparison boundary for the approved AX-only fixtures.
  public static BufferedPointCallerResult ExecuteCapacityAtComparison(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   if(s.IP!=0x075d)throw new ArgumentException("Capacity comparison entry must be 075D.");ushort count=m.ReadWord(s.ES,0);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,count,s.AX);
   if((short)count<(short)s.AX){s.IP=0x076c;return new BufferedPointCallerResult{State=s,Outcome=BufferedPointOutcome.NormalPathReady};}
   s.IP=0x0764;return RunCapacity(m,effects,s,stringSegment,followupSegment);
  }
  public static BufferedPointCallerResult ExecuteCapacityAtComparisonThroughReturn(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){return Return(m,ExecuteCapacityAtComparison(m,effects,s,stringSegment,followupSegment));}
  public static BufferedPointCallerResult ExecuteThroughReturn(IHorizontalRasterMemory m,IBufferedPointFpuEffects fpu,SignedDivide16State s,ushort converterSegment){
   BufferedPointCallerResult result=Execute(m,fpu,s,converterSegment);
   return Return(m,result);
  }
  static BufferedPointCallerResult Return(IHorizontalRasterMemory m,BufferedPointCallerResult result){
   if(result.Outcome==BufferedPointOutcome.Completed){SignedDivide16State s=result.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+16);s.IP=ip;s.CS=cs;result.State=s;result.RetfExecuted=true;}
   return result;
  }
 }
}
````

## CapacityCharacterOutput

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityCharacterOutput.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class CapacityCharacterOutput {
  static void Push(ICapacityStringMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  static bool Bios(ICapacityStringMemory m,ICapacityInterruptEffects fx,ref SignedDivide16State s,ushort at){ushort self=s.CS;s.IP=at;s=fx.Interrupt(0x10,m,s);return s.CS==self&&s.IP==(ushort)(at+2);}
  static void AH(ref SignedDivide16State s,byte v){s.AX=(ushort)((s.AX&255)|(v<<8));}
  static uint Inc8(uint f,byte a){byte b=(byte)(a+1);uint bits=(uint)(f&1);if(b==0)bits|=64;if((b&128)!=0)bits|=128;if(a==127)bits|=2048;if(((a^1^b)&16)!=0)bits|=16;int p=b;p^=p>>4;p^=p>>2;p^=p>>1;if((p&1)==0)bits|=4;return (f&~0x8d5u)|bits;}
  static CapacityDeviceResult Result(SignedDivide16State s,CapacityDeviceOutcome o){return new CapacityDeviceResult{State=s,Outcome=o};}
  // Relative 16FA:073B..07BE. BIOS INT10 effects and continuation are explicit.
  public static CapacityDeviceResult Execute(ICapacityStringMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x073b)throw new ArgumentException("Character entry must be 073B.");Push(m,ref s,s.BP);s.BP=s.SP;Push(m,ref s,s.DI);s.DI=m.ReadWord(s.SS,(ushort)(s.BP+6));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+8));AH(ref s,3);s.BX=(ushort)(s.BX&255);
   if(!Bios(m,fx,ref s,0x0746))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
   byte al=m.ReadByte(s.ES,s.DI);s.AX=(ushort)((s.AX&0xff00)|al);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,8);
   if(al==8){byte left=m.ReadByte(s.DS,0x4b98);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)s.DX,left);if((sbyte)(byte)s.DX<=(sbyte)left){
    byte dh=(byte)(s.DX>>8);s.EFlags=(RasterStatusFlags.Sub8(s.EFlags,dh,1)&~1u)|(s.EFlags&1u);s.DX=(ushort)(((byte)(dh-1)<<8)|m.ReadByte(s.DS,0x4b9a));AH(ref s,2);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x075f))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.AX=0x0a20;s.BX=0;s.CX=1;if(!Bios(m,fx,ref s,0x076b))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);goto Cleanup;
   }}
   al=m.ReadByte(s.ES,s.DI);s.AX=(ushort)((s.AX&0xff00)|al);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,27);
   if((sbyte)al>27){s.BX=m.ReadWord(s.DS,0x8164);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.BX,0);if(s.BX!=0){
    Push(m,ref s,s.AX);s.AX=0x0a08;s.CX=1;s.BX=7;if(!Bios(m,fx,ref s,0x078a))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.AX=0x0907;s.CX=1;s.BX=0x87;if(!Bios(m,fx,ref s,0x0795))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.AX=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);AH(ref s,9);s.CX=1;s.BX=0x87;if(!Bios(m,fx,ref s,0x07a0))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    AH(ref s,3);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x07a6))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
    s.EFlags=Inc8(s.EFlags,(byte)s.DX);s.DX=(ushort)((s.DX&0xff00)|(byte)(s.DX+1));AH(ref s,2);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x07ae))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);goto Cleanup;
   }}
   AH(ref s,0x0e);s.BX=7;if(!Bios(m,fx,ref s,0x07b8))return Result(s,CapacityDeviceOutcome.ExternalControlTransfer);
   Cleanup:s.DI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x07bc;return Result(s,CapacityDeviceOutcome.ReachedReturn);
  }
  public static CapacityDeviceResult ExecuteThroughReturn(ICapacityStringMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){var r=Execute(m,fx,s);if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+8);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
 public sealed class CapacityCharacterNativeEffects:ICapacityCharacterEffects {
  readonly ICapacityInterruptEffects interrupts;public CapacityCharacterNativeEffects(ICapacityInterruptEffects effects){if(effects==null)throw new ArgumentNullException("effects");interrupts=effects;}
  public SignedDivide16State CallCharacter(ICapacityStringMemory memory,SignedDivide16State state){return CapacityCharacterOutput.ExecuteThroughReturn(memory,interrupts,state).State;}
 }
}
````

## CapacityDeviceCallees

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityDeviceCallees.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICapacityInterruptEffects {SignedDivide16State Interrupt(byte number,IHorizontalRasterMemory memory,SignedDivide16State state);}
 public enum CapacityDeviceOutcome {ReachedReturn,ExternalControlTransfer}
 public struct CapacityDeviceResult {public SignedDivide16State State;public CapacityDeviceOutcome Outcome;public bool RetfExecuted;}
 public static class CapacityDeviceCallees {
  static CapacityDeviceResult Done(SignedDivide16State s,CapacityDeviceOutcome o){return new CapacityDeviceResult{State=s,Outcome=o};}
  static CapacityDeviceResult Return(IHorizontalRasterMemory m,CapacityDeviceResult r){if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){var s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
  // Relative 16FA:090D..091C. INT effects include its actual continuation.
  public static CapacityDeviceResult KeyStatus(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x090d)throw new ArgumentException("Key-status entry must be 090D.");ushort self=s.CS;s.AX=(ushort)((s.AX&255)|0x100);s.IP=0x090f;s=fx.Interrupt(0x16,m,s);
   if(s.CS!=self||s.IP!=0x0911)return Done(s,CapacityDeviceOutcome.ExternalControlTransfer);
   s.AX=(ushort)((s.EFlags&0x40)!=0?0:1);s.IP=0x091c;return Done(s,CapacityDeviceOutcome.ReachedReturn);
  }
  public static CapacityDeviceResult KeyStatusThroughReturn(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){return Return(m,KeyStatus(m,fx,s));}
  // Relative 16FA:0A32..0A4E. The gate's meaning and device values remain external.
  public static CapacityDeviceResult Poll(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x0a32)throw new ArgumentException("Poll entry must be 0A32.");ushort self=s.CS;s.AX=m.ReadWord(s.DS,0xac58);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.AX,0);
   if(s.AX!=0){s.AX=3;s.IP=0x0a3d;s=fx.Interrupt(0x33,m,s);if(s.CS!=self||s.IP!=0x0a3f)return Done(s,CapacityDeviceOutcome.ExternalControlTransfer);m.WriteWord(s.DS,0xac50,s.AX);m.WriteWord(s.DS,0xac52,s.BX);m.WriteWord(s.DS,0xac54,s.CX);m.WriteWord(s.DS,0xac56,s.DX);}
   s.IP=0x0a4e;return Done(s,CapacityDeviceOutcome.ReachedReturn);
  }
  public static CapacityDeviceResult PollThroughReturn(IHorizontalRasterMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){return Return(m,Poll(m,fx,s));}
 }
}
````

## CapacityFollowup

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityFollowup.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public enum CapacityFollowupCallKind {KeyStatus,PromptOutput,KeyRead,Poll}
 public interface ICapacityFollowupEffects {SignedDivide16State Call(CapacityFollowupCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state);}
 public enum CapacityFollowupOutcome {ReachedReturn,ExternalControlTransfer,UnverifiedBranch}
 public struct CapacityFollowupResult {public SignedDivide16State State;public CapacityFollowupOutcome Outcome;public bool RetfExecuted;}
 public static class CapacityFollowup {
  static void Push(IHorizontalRasterMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  static bool Call(IHorizontalRasterMemory m,ICapacityFollowupEffects fx,ref SignedDivide16State s,CapacityFollowupCallKind kind,ushort segment,ushort offset,ushort ret){ushort self=s.CS;Push(m,ref s,self);Push(m,ref s,ret);s.CS=segment;s.IP=offset;s=fx.Call(kind,m,s);return s.CS==self&&s.IP==ret;}
  static CapacityFollowupResult Result(SignedDivide16State s,CapacityFollowupOutcome o){return new CapacityFollowupResult{State=s,Outcome=o};}
  // Relative 1597:0B47..0BAC; callee and device effects remain external.
  public static CapacityFollowupResult Execute(IHorizontalRasterMemory m,ICapacityFollowupEffects fx,SignedDivide16State s,ushort calleeSegment){
   if(s.IP!=0x0b47)throw new ArgumentException("Follow-up entry must be 0B47.");
   Push(m,ref s,s.BP);s.BP=s.SP;s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.SP,4);s.SP=(ushort)(s.SP-4);
   s.AX=m.ReadWord(s.DS,0x8184);ushort v=m.ReadWord(s.DS,0x8180);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,v,s.AX);
   if(v!=s.AX)goto Cleanup;
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.KeyStatus,calleeSegment,0x090d,0x0b5b))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   m.WriteWord(s.DS,0xaea8,s.AX);m.WriteWord(s.SS,(ushort)(s.BP-2),0x001e);s.AX=0x1aac;Push(m,ref s,s.DS);Push(m,ref s,s.AX);s.AX=(ushort)(s.BP-4);m.WriteWord(s.DS,0x62c6,s.AX);m.WriteWord(s.DS,0x62c8,s.SS);
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.PromptOutput,calleeSegment,0x06f6,0x0b77))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   StatusRepeat:
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.KeyStatus,calleeSegment,0x090d,0x0b7c))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   // OR AX,AX: DOSBox-X normal-core logical AF=0 policy; non-status bits retained.
   uint bits=s.AX==0?64u:0u;if((s.AX&0x8000)!=0)bits|=128;int parity=s.AX&255;parity^=parity>>4;parity^=parity>>2;parity^=parity>>1;if((parity&1)==0)bits|=4;s.EFlags=(s.EFlags&~0x8d5u)|bits;
   if(s.AX==0){
    if(!Call(m,fx,ref s,CapacityFollowupCallKind.Poll,calleeSegment,0x0a32,0x0b85))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
    v=m.ReadWord(s.DS,0xac52);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,v,0);
    if(v==0)goto StatusRepeat;
    do {
     if(!Call(m,fx,ref s,CapacityFollowupCallKind.Poll,calleeSegment,0x0a32,0x0b91))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
     v=m.ReadWord(s.DS,0xac52);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,v,0);
    }while(v!=0);
    goto Cleanup;
   }
   s.AX=0xaea8;Push(m,ref s,s.DS);Push(m,ref s,s.AX);s.AX=0xaeaa;Push(m,ref s,s.DS);Push(m,ref s,s.AX);
   if(!Call(m,fx,ref s,CapacityFollowupCallKind.KeyRead,calleeSegment,0x0841,0x0ba9))return Result(s,CapacityFollowupOutcome.ExternalControlTransfer);
   Cleanup:
   s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0bac;return Result(s,CapacityFollowupOutcome.ReachedReturn);
  }
  public static CapacityFollowupResult ExecuteThroughReturn(IHorizontalRasterMemory m,ICapacityFollowupEffects fx,SignedDivide16State s,ushort calleeSegment){var r=Execute(m,fx,s,calleeSegment);if(r.Outcome==CapacityFollowupOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
}
````

## CapacityFollowupNativeEffects

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityFollowupNativeEffects.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Connects the verified instruction bodies. BIOS/device services and character
 // rendering remain injected dependencies; no Windows mapping is supplied.
 public sealed class CapacityFollowupNativeEffects:ICapacityFollowupEffects {
  readonly ICapacityInterruptEffects interrupts;readonly ICapacityCharacterEffects characters;readonly ushort characterSegment;
  public CapacityFollowupNativeEffects(ICapacityInterruptEffects interruptEffects,ICapacityCharacterEffects characterEffects,ushort characterTargetSegment){if(interruptEffects==null||characterEffects==null)throw new ArgumentNullException();interrupts=interruptEffects;characters=characterEffects;characterSegment=characterTargetSegment;}
  public SignedDivide16State Call(CapacityFollowupCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state){
   switch(kind){
    case CapacityFollowupCallKind.KeyStatus:return CapacityDeviceCallees.KeyStatusThroughReturn(memory,interrupts,state).State;
    case CapacityFollowupCallKind.Poll:return CapacityDeviceCallees.PollThroughReturn(memory,interrupts,state).State;
    case CapacityFollowupCallKind.PromptOutput:
     var text=memory as ICapacityStringMemory;if(text==null)throw new ArgumentException("String byte reads required.");return CapacityStringOutput.ExecuteThroughReturn(text,characters,state,characterSegment).State;
    case CapacityFollowupCallKind.KeyRead:
     var key=memory as ICapacityKeyMemory;if(key==null)throw new ArgumentException("Key output byte writes required.");return CapacityKeyReader.ExecuteThroughReturn(key,interrupts,state).State;
    default:throw new ArgumentOutOfRangeException("kind");
   }
  }
 }
}
````

## CapacityKeyReader

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityKeyReader.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICapacityKeyMemory:IHorizontalRasterMemory {void WriteByte(ushort segment,ushort offset,byte value);}
 public static class CapacityKeyReader {
  // Relative 16FA:0841..0857: full word-register/flags/stack projection.
  public static CapacityDeviceResult Execute(ICapacityKeyMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){
   if(s.IP!=0x0841)throw new ArgumentException("Key-reader entry must be 0841.");ushort self=s.CS;s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;s.AX=(ushort)(s.AX&255);s.IP=0x0846;s=fx.Interrupt(0x16,m,s);
   if(s.CS!=self||s.IP!=0x0848)return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ExternalControlTransfer};
   s.BX=m.ReadWord(s.SS,(ushort)(s.BP+6));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+8));m.WriteByte(s.ES,s.BX,(byte)s.AX);
   s.BX=m.ReadWord(s.SS,(ushort)(s.BP+10));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+12));m.WriteByte(s.ES,s.BX,(byte)(s.AX>>8));
   s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0855;return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ReachedReturn};
  }
  public static CapacityDeviceResult ExecuteThroughReturn(ICapacityKeyMemory m,ICapacityInterruptEffects fx,SignedDivide16State s){var r=Execute(m,fx,s);if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+12);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
}
````

## CapacityMessageHandler

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityMessageHandler.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public enum CapacityCallKind {StringOutput,Followup}
 public interface ICapacityCallEffects {
  // Execute the separately provided callee, returning its actual continuation.
  // No message presentation, keyboard policy or callee ABI is invented here.
  SignedDivide16State Call(CapacityCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state);
 }
 public enum CapacityMessageOutcome {ReachedReturn,ExternalControlTransfer}
 public struct CapacityMessageResult {public SignedDivide16State State;public CapacityMessageOutcome Outcome;public bool RetfExecuted;}
 public static class CapacityMessageHandler {
  static SignedDivide16State Call(IHorizontalRasterMemory m,ICapacityCallEffects effects,CapacityCallKind kind,SignedDivide16State s,ushort segment,ushort offset,ushort returnOffset){
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,returnOffset);s.CS=segment;s.IP=offset;return effects.Call(kind,m,s);
  }
  // Original relative 0C25:084F..0881. The two callees remain explicit effects.
  public static CapacityMessageResult Execute(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   if(s.IP!=0x084f)throw new ArgumentException("Capacity helper entry must be 084F.");ushort self=s.CS;
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.SP,4);s.SP=(ushort)(s.SP-4);
   ushort guard=m.ReadWord(s.DS,0x80e0);s.EFlags=RasterStatusFlags.Sub16(s.EFlags,guard,0);
   if(guard==0){
    ushort old=m.ReadWord(s.DS,0x80e0);s.EFlags=RasterStatusFlags.Inc16(s.EFlags,old);m.WriteWord(s.DS,0x80e0,(ushort)(old+1));m.WriteWord(s.SS,(ushort)(s.BP-2),0x002a);
    s.AX=0x1148;s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.DS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.AX);
    s.AX=(ushort)(s.BP-4);m.WriteWord(s.DS,0x62c6,s.AX);m.WriteWord(s.DS,0x62c8,s.SS);
    s=Call(m,effects,CapacityCallKind.StringOutput,s,stringSegment,0x06f6,0x0879);
    if(s.CS!=self||s.IP!=0x0879)return new CapacityMessageResult{State=s,Outcome=CapacityMessageOutcome.ExternalControlTransfer};
    s=Call(m,effects,CapacityCallKind.Followup,s,followupSegment,0x0b47,0x087e);
    if(s.CS!=self||s.IP!=0x087e)return new CapacityMessageResult{State=s,Outcome=CapacityMessageOutcome.ExternalControlTransfer};
   }
   s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0881;return new CapacityMessageResult{State=s,Outcome=CapacityMessageOutcome.ReachedReturn};
  }
  public static CapacityMessageResult ExecuteThroughReturn(IHorizontalRasterMemory m,ICapacityCallEffects effects,SignedDivide16State s,ushort stringSegment,ushort followupSegment){
   CapacityMessageResult r=Execute(m,effects,s,stringSegment,followupSegment);
   if(r.Outcome==CapacityMessageOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;
  }
 }
}
````

## CapacityMessageNativeEffects

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityMessageNativeEffects.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Connects the enclosing message handler to verified native instruction bodies.
 public sealed class CapacityMessageNativeEffects:ICapacityCallEffects {
  readonly CapacityFollowupNativeEffects followup;readonly ICapacityCharacterEffects characters;readonly ushort calleeSegment,characterSegment;
  public CapacityMessageNativeEffects(ICapacityInterruptEffects interruptEffects,ICapacityCharacterEffects characterEffects,ushort sharedCalleeSegment,ushort characterTargetSegment){if(interruptEffects==null||characterEffects==null)throw new ArgumentNullException();characters=characterEffects;calleeSegment=sharedCalleeSegment;characterSegment=characterTargetSegment;followup=new CapacityFollowupNativeEffects(interruptEffects,characterEffects,characterTargetSegment);}
  public static CapacityMessageNativeEffects CreateWithNativeCharacters(ICapacityInterruptEffects interruptEffects,ushort sharedCalleeSegment,ushort characterTargetSegment){return new CapacityMessageNativeEffects(interruptEffects,new CapacityCharacterNativeEffects(interruptEffects),sharedCalleeSegment,characterTargetSegment);}
  public SignedDivide16State Call(CapacityCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state){
   switch(kind){
    case CapacityCallKind.StringOutput:var text=memory as ICapacityStringMemory;if(text==null)throw new ArgumentException("String byte reads required.");return CapacityStringOutput.ExecuteThroughReturn(text,characters,state,characterSegment).State;
    case CapacityCallKind.Followup:return CapacityFollowup.ExecuteThroughReturn(memory,followup,state,calleeSegment).State;
    default:throw new ArgumentOutOfRangeException("kind");
   }
  }
 }
}
````

## CapacityStringOutput

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CapacityStringOutput.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICapacityStringMemory:IHorizontalRasterMemory {byte ReadByte(ushort segment,ushort offset);}
 public interface ICapacityCharacterEffects {SignedDivide16State CallCharacter(ICapacityStringMemory memory,SignedDivide16State state);}
 public static class CapacityStringOutput {
  static void Push(ICapacityStringMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  // Relative 16FA:06F6..0712: character callee effects remain explicit.
  public static CapacityDeviceResult Execute(ICapacityStringMemory m,ICapacityCharacterEffects fx,SignedDivide16State s,ushort characterSegment){
   if(s.IP!=0x06f6)throw new ArgumentException("String entry must be 06F6.");ushort self=s.CS;Push(m,ref s,s.BP);s.BP=s.SP;Push(m,ref s,s.DI);s.DI=m.ReadWord(s.SS,(ushort)(s.BP+6));s.ES=m.ReadWord(s.SS,(ushort)(s.BP+8));
   while(true){byte b=m.ReadByte(s.ES,s.DI);s.AX=(ushort)((s.AX&0xff00)|b);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,b,0);if(b==0)break;
    Push(m,ref s,s.ES);Push(m,ref s,s.DI);Push(m,ref s,self);Push(m,ref s,0x070b);s.CS=characterSegment;s.IP=0x073b;s=fx.CallCharacter(m,s);
    if(s.CS!=self||s.IP!=0x070b)return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ExternalControlTransfer};s.EFlags=RasterStatusFlags.Inc16(s.EFlags,s.DI);s.DI=(ushort)(s.DI+1);
   }
   s.DI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0710;return new CapacityDeviceResult{State=s,Outcome=CapacityDeviceOutcome.ReachedReturn};
  }
  public static CapacityDeviceResult ExecuteThroughReturn(ICapacityStringMemory m,ICapacityCharacterEffects fx,SignedDivide16State s,ushort characterSegment){var r=Execute(m,fx,s,characterSegment);if(r.Outcome==CapacityDeviceOutcome.ReachedReturn){s=r.State;ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+8);s.IP=ip;s.CS=cs;r.State=s;r.RetfExecuted=true;}return r;}
 }
}
````

## CharacterAlternateOutput

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CharacterAlternateOutput.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct CharacterBiosWords {public ushort AX,BX,CX,DX,SS,SP;}
 public interface ICharacterBiosContext {
  void WriteWord(ushort segment,ushort offset,ushort value);
  ushort ReadWord(ushort segment,ushort offset);
  void Interrupt10(ref CharacterBiosWords words);
 }
 public static class CharacterAlternateOutput {
  // Original 16FA:0780..07B1; ends at 07BA, before the epilogue.
  // Explicit word/memory projection. BIOS calls are external effects.
  public static CharacterBiosWords Run(ICharacterBiosContext context,CharacterBiosWords w){
   if(context==null)throw new ArgumentNullException("context");
   w.SP=unchecked((ushort)(w.SP-2));context.WriteWord(w.SS,w.SP,w.AX);
   w.AX=0x0a08;w.CX=1;w.BX=7;context.Interrupt10(ref w);
   w.AX=0x0907;w.CX=1;w.BX=0x0087;context.Interrupt10(ref w);
   w.AX=context.ReadWord(w.SS,w.SP);w.SP=unchecked((ushort)(w.SP+2));
   w.AX=(ushort)((w.AX&255)|0x0900);w.CX=1;w.BX=0x0087;context.Interrupt10(ref w);
   w.AX=(ushort)((w.AX&255)|0x0300);w.BX=(ushort)(w.BX&255);context.Interrupt10(ref w);
   w.DX=(ushort)((w.DX&0xff00)|unchecked((byte)((w.DX&255)+1)));
   w.AX=(ushort)((w.AX&255)|0x0200);w.BX=(ushort)(w.BX&255);context.Interrupt10(ref w);
   return w;
  }
 }
}
````

## CharacterOutputDecisions

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CharacterOutputDecisions.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICharacterDecisionMemory { byte ReadByte(ushort segment,ushort offset); ushort ReadWord(ushort segment,ushort offset); }
 public struct CharacterDecisionResult {
  public readonly byte AL; public readonly ushort BX; public readonly bool Branch;
  public CharacterDecisionResult(byte al,ushort bx,bool branch){AL=al;BX=bx;Branch=branch;}
 }
 public static class CharacterOutputDecisions {
  // 16FA:0748..0754. Branch=true reaches 0755; false reaches 0770.
  public static CharacterDecisionResult ProbeBackspace(ICharacterDecisionMemory m,ushort ds,ushort es,ushort di,byte dl,ushort bx){
   if(m==null)throw new ArgumentNullException("m");
   byte al=m.ReadByte(es,di);
   bool branch=al==8 && unchecked((sbyte)dl)<=unchecked((sbyte)m.ReadByte(ds,0x4b98));
   return new CharacterDecisionResult(al,bx,branch);
  }
  // 16FA:0770..077F. Branch=true reaches ordinary output 07B3; false reaches 0780.
  public static CharacterDecisionResult ProbeAlternateGate(ICharacterDecisionMemory m,ushort ds,ushort es,ushort di,ushort bx){
   if(m==null)throw new ArgumentNullException("m");
   byte al=m.ReadByte(es,di);
   if(unchecked((sbyte)al)<=27)return new CharacterDecisionResult(al,bx,true);
   bx=m.ReadWord(ds,0x8164);
   return new CharacterDecisionResult(al,bx,bx==0);
  }
 }
}
````

## CharacterOutputFrame

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CharacterOutputFrame.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface ICharacterFrameMemory {ushort ReadWord(ushort segment,ushort offset);void WriteWord(ushort segment,ushort offset,ushort value);}
 public struct CharacterEntryWords {public ushort SP,BP,DI,ES;}
 public struct CharacterReturnWords {public ushort SP,BP,DI,IP,CS;}
 public static class CharacterOutputFrame {
  // 16FA:073B..0741. Explicit data/word projection, before the BIOS query.
  public static CharacterEntryWords Enter(ICharacterFrameMemory m,ushort ss,ushort sp,ushort bp,ushort di){
   if(m==null)throw new ArgumentNullException("m");
   sp=unchecked((ushort)(sp-2));m.WriteWord(ss,sp,bp);bp=sp;
   sp=unchecked((ushort)(sp-2));m.WriteWord(ss,sp,di);
   ushort offset=m.ReadWord(ss,unchecked((ushort)(bp+6)));
   ushort segment=m.ReadWord(ss,unchecked((ushort)(bp+8)));
   return new CharacterEntryWords{SP=sp,BP=bp,DI=offset,ES=segment};
  }
  // 16FA:07BA..07BE. Four ordered word reads, then RETF's argument purge.
  public static CharacterReturnWords Leave(ICharacterFrameMemory m,ushort ss,ushort sp){
   if(m==null)throw new ArgumentNullException("m");
   ushort di=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort bp=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort ip=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort cs=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+6));
   return new CharacterReturnWords{SP=sp,BP=bp,DI=di,IP=ip,CS=cs};
  }
 }
}
````

## CharacterOutputRoutine

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CharacterOutputRoutine.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct CharacterRoutineWords {public ushort AX,BX,CX,DX,SI,DI,BP,DS,ES,SS,SP;}
 public struct CharacterRoutineResult {public CharacterRoutineWords Words;public ushort IP,CS;}
 public interface ICharacterRoutineContext:ICharacterFrameMemory,ICharacterDecisionMemory {
  new ushort ReadWord(ushort segment,ushort offset);
  // Return offsets identify original call boundaries; BIOS effects remain external.
  void Interrupt10(ref CharacterRoutineWords words,ushort returnOffset);
 }
 public static class CharacterOutputRoutine {
  sealed class AlternateBridge:ICharacterBiosContext {
   public ICharacterRoutineContext Context;public CharacterRoutineWords Words;int index;
   public void WriteWord(ushort s,ushort o,ushort v){Context.WriteWord(s,o,v);}
   public ushort ReadWord(ushort s,ushort o){return Context.ReadWord(s,o);}
   public void Interrupt10(ref CharacterBiosWords w){Words.AX=w.AX;Words.BX=w.BX;Words.CX=w.CX;Words.DX=w.DX;Words.SS=w.SS;Words.SP=w.SP;Context.Interrupt10(ref Words,new ushort[]{0x78c,0x797,0x7a2,0x7a8,0x7b0}[index++]);w.AX=Words.AX;w.BX=Words.BX;w.CX=Words.CX;w.DX=Words.DX;w.SS=Words.SS;w.SP=Words.SP;}
  }
  // Word/memory/control projection of 16FA:073B..07BE. Flags, interrupt frames,
  // hardware exceptions, and BIOS implementation remain external to this interface.
  public static CharacterRoutineResult Run(ICharacterRoutineContext m,CharacterRoutineWords w){
   if(m==null)throw new ArgumentNullException("m");
   CharacterEntryWords e=CharacterOutputFrame.Enter(m,w.SS,w.SP,w.BP,w.DI);w.SP=e.SP;w.BP=e.BP;w.DI=e.DI;w.ES=e.ES;
   w.AX=(ushort)((w.AX&255)|0x0300);w.BX=(ushort)(w.BX&255);m.Interrupt10(ref w,0x748);
   CharacterDecisionResult d=CharacterOutputDecisions.ProbeBackspace(m,w.DS,w.ES,w.DI,(byte)w.DX,w.BX);w.AX=(ushort)((w.AX&0xff00)|d.AL);w.BX=d.BX;
   if(d.Branch){
    w.DX=(ushort)((w.DX&255)|((byte)((w.DX>>8)-1)<<8));w.DX=(ushort)((w.DX&0xff00)|m.ReadByte(w.DS,0x4b9a));
    w.AX=(ushort)((w.AX&255)|0x0200);w.BX=(ushort)(w.BX&255);m.Interrupt10(ref w,0x761);
    w.AX=0x0a20;w.BX=0;w.CX=1;m.Interrupt10(ref w,0x76d);
   }else{
    d=CharacterOutputDecisions.ProbeAlternateGate(m,w.DS,w.ES,w.DI,w.BX);w.AX=(ushort)((w.AX&0xff00)|d.AL);w.BX=d.BX;
    if(d.Branch){w.AX=(ushort)((w.AX&255)|0x0e00);w.BX=7;m.Interrupt10(ref w,0x7ba);}
    else{
     AlternateBridge bridge=new AlternateBridge{Context=m,Words=w};CharacterBiosWords b=CharacterAlternateOutput.Run(bridge,new CharacterBiosWords{AX=w.AX,BX=w.BX,CX=w.CX,DX=w.DX,SS=w.SS,SP=w.SP});w=bridge.Words;w.AX=b.AX;w.BX=b.BX;w.CX=b.CX;w.DX=b.DX;w.SS=b.SS;w.SP=b.SP;
    }
   }
   CharacterReturnWords r=CharacterOutputFrame.Leave(m,w.SS,w.SP);w.SP=r.SP;w.BP=r.BP;w.DI=r.DI;return new CharacterRoutineResult{Words=w,IP=r.IP,CS=r.CS};
  }
 }
}
````

## ClippingCpu

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ClippingCpu.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class ClippingCpu {
  // 16-bit real-mode entry at clipping offset 078C. Normal-core fault flag
  // details are explicitly tied to the verified DOSBox-X reference environment.
  public static SignedDivide16Result Execute(IHorizontalRasterMemory memory,SignedDivide16State s){
   if(s.IP!=0x078c)throw new ArgumentException("Clipping entry must be 078C.");
   try {
    LineClippingFlagResult r=LineClipping.ExecuteWithFlags(memory,s.DS,s.AX,s.BX,s.CX,s.DX,s.SI,s.DI,s.EFlags);
    s.AX=r.Words.AX;s.BX=r.Words.BX;s.CX=r.Words.CX;s.DX=r.Words.DX;s.SI=r.Words.SI;s.DI=r.Words.DI;s.IP=r.Words.Exit;s.EFlags=r.EFlags;
    return new SignedDivide16Result{State=s,Fault=SignedDivide16Fault.None,DefinedEFlagsMask=0xffffffffu};
   }catch(LineClippingDivideFault fault){
    if(!fault.HasEFlags)throw;
    s.AX=fault.Words.AX;s.BX=fault.Words.BX;s.CX=fault.Words.CX;s.DX=fault.Words.DX;s.SI=fault.Words.SI;s.DI=fault.Words.DI;s.IP=fault.Words.Exit;s.EFlags=fault.EFlags;
    return SignedDivide16.Execute(memory,s);
   }
  }
 }
}
````

## ConsumerContextSaver

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ConsumerContextSaver.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IConsumerContextMemory {
  ushort ReadWord(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public static class ConsumerContextSaver {
  // Original 18B9:233E data/result effects. Explicit entry register words.
  // No exception/nonlocal-return meaning or full ABI adapter is supplied.
  public static ushort Execute(IConsumerContextMemory m,ushort stackSegment,ushort entrySp,ushort entryBp,ushort entrySi,ushort entryDi,ushort entryDs){
   if(m==null)throw new ArgumentNullException("m");
   ushort destination=m.ReadWord(stackSegment,unchecked((ushort)(entrySp+4)));
   ushort destinationSegment=m.ReadWord(stackSegment,unchecked((ushort)(entrySp+6)));
   m.WriteWord(destinationSegment,destination,entryBp);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+2)),entryDi);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+4)),entrySi);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+6)),entrySp);
   ushort value=m.ReadWord(stackSegment,entrySp);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+8)),value);
   value=m.ReadWord(stackSegment,unchecked((ushort)(entrySp+2)));
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+10)),value);
   m.WriteWord(destinationSegment,unchecked((ushort)(destination+12)),entryDs);
   return 0;
  }
 }
}
````

## CoordinateConversion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CoordinateConversion.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // The x87 environment is explicit. This interface does not invent exception,
 // status-word, tag-word or underlying-stack behavior.
 public interface ICoordinateFpuEffects {
  void Wait();
  void StoreControlWord(IHorizontalRasterMemory memory,ushort segment,ushort offset);
  void LoadControlWord(IHorizontalRasterMemory memory,ushort segment,ushort offset);
  void StoreInteger64AndPop(IHorizontalRasterMemory memory,ushort segment,ushort offset);
 }
 public static class FiniteBinary32Integer64 {
  // Numeric projection for finite binary32 inputs, truncation toward zero.
  // False means outside this projection, not an original exception policy.
  public static bool TryConvert(uint bits,out long value){
   value=0;bool negative=(bits&0x80000000u)!=0;int exponent=(int)((bits>>23)&255);uint fraction=bits&0x7fffff;
   if(exponent==255)return false;
   int power=exponent-127;if(exponent==0||power<0)return true;
   ulong significand=0x800000u|fraction,magnitude;int shift=power-23;
   if(shift<0)magnitude=significand>>(-shift);
   else {ulong limit=negative?0x8000000000000000UL:0x7fffffffffffffffUL;if(shift>=64||significand>(limit>>shift))return false;magnitude=significand<<shift;}
   value=unchecked((long)(negative?0UL-magnitude:magnitude));return true;
  }
 }
 public static class CoordinateConversion {
  // Relative 18B9:0C70..0CA2; finite-path general-register/stack projection.
  // Wait locations follow captured runtime bytes, including runtime NOPs.
  public static SignedDivide16State Execute(IHorizontalRasterMemory m,ICoordinateFpuEffects fpu,SignedDivide16State s){
   if(s.IP!=0x0c70)throw new ArgumentException("Converter entry must be 0C70.");
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BP);s.BP=s.SP;s.SP=(ushort)(s.SP-12);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.BX);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CX);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.SI);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.DI);
   fpu.Wait();fpu.StoreControlWord(m,s.SS,(ushort)(s.BP-2));fpu.Wait();
   s.AX=m.ReadWord(s.SS,(ushort)(s.BP-2));byte high=(byte)((s.AX>>8)|12);s.AX=(ushort)((s.AX&255)|(high<<8));s.EFlags=RasterStatusFlags.Logical8(s.EFlags,high);
   m.WriteWord(s.SS,(ushort)(s.BP-4),s.AX);fpu.LoadControlWord(m,s.SS,(ushort)(s.BP-4));
   fpu.StoreInteger64AndPop(m,s.SS,(ushort)(s.BP-12));fpu.LoadControlWord(m,s.SS,(ushort)(s.BP-2));
   s.AX=m.ReadWord(s.SS,(ushort)(s.BP-12));s.DX=m.ReadWord(s.SS,(ushort)(s.BP-10));
   s.DI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.SI=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.CX=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.BX=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
   s.SP=s.BP;s.BP=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=0x0ca2;return s;
  }
  public static SignedDivide16State ExecuteThroughReturn(IHorizontalRasterMemory m,ICoordinateFpuEffects fpu,SignedDivide16State s){
   s=Execute(m,fpu,s);ushort ip=m.ReadWord(s.SS,s.SP),cs=m.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;return s;
  }
 }
}
````

## CoordinateWordClamp

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CoordinateWordClamp.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public static class CoordinateWordClamp {
  // Relative 0C25:0792..07EA. Registers are untouched; EFLAGS and the
  // literal working words are projected without assigning geometric roles.
  public static uint Execute(IHorizontalRasterMemory m,ushort ds,uint flags){
   ushort high=m.ReadWord(ds,0xc592);flags=RasterStatusFlags.Sub16(flags,high,0);
   if((short)high<0)goto FirstLower;
   if((short)high>0)goto FirstUpper;
   ushort low=m.ReadWord(ds,0xc590);flags=RasterStatusFlags.Sub16(flags,low,0x7ff8);
   if(low<0x7ff8)goto FirstLower;
  FirstUpper:
   m.WriteWord(ds,0xc590,0x7ff8);m.WriteWord(ds,0xc592,0);goto Second;
  FirstLower:
   high=m.ReadWord(ds,0xc592);flags=RasterStatusFlags.Sub16(flags,high,0xffff);
   if((short)high> -1)goto Second;
   if((short)high< -1)goto FirstLowerWrite;
   low=m.ReadWord(ds,0xc590);flags=RasterStatusFlags.Sub16(flags,low,0x80a8);
   if(low>=0x80a8)goto Second;
  FirstLowerWrite:
   m.WriteWord(ds,0xc590,0x80a8);m.WriteWord(ds,0xc592,0xffff);
  Second:
   high=m.ReadWord(ds,0xc596);flags=RasterStatusFlags.Sub16(flags,high,0);
   if((short)high<0)return flags;
   if((short)high>0)goto SecondUpper;
   low=m.ReadWord(ds,0xc594);flags=RasterStatusFlags.Sub16(flags,low,0x7fee);
   if(low<=0x7fee)return flags;
  SecondUpper:
   m.WriteWord(ds,0xc594,0x7fee);m.WriteWord(ds,0xc596,0);return flags;
  }
 }
}
````

## CoreDllVersion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CoreDllVersion.cs)

````csharp
using System.Reflection;

[assembly: AssemblyTitle("ScoreModern.dll")]
[assembly: AssemblyProduct("SCORE Modern Core")]
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.3.61.2")]
[assembly: AssemblyInformationalVersion("0.3.61.2")]
````

## DatTextLookup

[Workspace source](../../ScoreReverseEngineering/NativeFragments/DatTextLookup.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IDatTextLookupMemory {ushort ReadWord(ushort segment,ushort offset);}
 public struct DatTextLookupWords {public ushort BX,AX,DX;}
 public static class DatTextLookup {
  // Original 020F:04C3..04D4 word/read projection. The trailing jump has no
  // effect on these words. No bounds check, character mapping or float policy added.
  public static DatTextLookupWords Read(IDatTextLookupMemory m,ushort ds){
   if(m==null)throw new ArgumentNullException("m");
   ushort bx=m.ReadWord(ds,0x8678);bx=unchecked((ushort)(bx<<1));bx=unchecked((ushort)(bx<<1));
   ushort ax=m.ReadWord(ds,unchecked((ushort)(bx+0x8a3c)));
   ushort dx=m.ReadWord(ds,unchecked((ushort)(bx+0x8a3e)));
   return new DatTextLookupWords{BX=bx,AX=ax,DX=dx};
  }
 }
}
````

## DirectionalStringBlocks

[Workspace source](../../ScoreReverseEngineering/NativeFragments/DirectionalStringBlocks.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IStringBlockMemory {
  byte ReadByte(ushort segment,ushort offset);
  ushort ReadWord(ushort segment,ushort offset);
  void WriteByte(ushort segment,ushort offset,byte value);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public struct StringCopyResult {
  public readonly ushort SI,DI,CX;
  public StringCopyResult(ushort si,ushort di){SI=si;DI=di;CX=0;}
 }
 public struct StringFillResult {
  public readonly ushort DI,CX;
  public StringFillResult(ushort di){DI=di;CX=0;}
 }
 public static class DirectionalStringBlocks {
  // Original 1F08:0115..011B. Preserve word transfers and their order;
  // overlapping buffers must not be replaced by host memmove semantics.
  public static StringCopyResult Copy(IStringBlockMemory m,ushort ds,ushort si,ushort es,ushort di,ushort count,bool directionFlag){
   if(m==null)throw new ArgumentNullException("m");
   int step=directionFlag?-2:2;
   for(int words=count>>1;words!=0;words--){
    ushort value=m.ReadWord(ds,si);m.WriteWord(es,di,value);
    si=unchecked((ushort)(si+step));di=unchecked((ushort)(di+step));
   }
   if((count&1)!=0){
    byte value=m.ReadByte(ds,si);m.WriteByte(es,di,value);
    int delta=directionFlag?-1:1;si=unchecked((ushort)(si+delta));di=unchecked((ushort)(di+delta));
   }
   return new StringCopyResult(si,di);
  }
  // Original 1F08:00EF..00F5. AX supplies each word and AL the odd byte.
  public static StringFillResult Fill(IStringBlockMemory m,ushort es,ushort di,ushort ax,ushort count,bool directionFlag){
   if(m==null)throw new ArgumentNullException("m");
   int step=directionFlag?-2:2;
   for(int words=count>>1;words!=0;words--){m.WriteWord(es,di,ax);di=unchecked((ushort)(di+step));}
   if((count&1)!=0){m.WriteByte(es,di,(byte)ax);di=unchecked((ushort)(di+(directionFlag?-1:1)));}
   return new StringFillResult(di);
  }
 }
}
````

## EndingBracket

[Workspace source](../../ScoreReverseEngineering/NativeFragments/EndingBracket.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class EndingBracketResult {
  public readonly List<TupletBracketPoint> Points=new List<TupletBracketPoint>();
  public TupletPlacement? Number;public float DotX,DotY;public bool HasDot;
 }
 public static class EndingBracket {
  static float F(double v){return (float)v;}
  static short I(float v){return unchecked((short)(int)Math.Truncate(v));}
  // Geometry/request projection of 1149:12D0..1396,1656..1758,1627..1653.
  // The optional tenth ending uses the measured 4..9 ending path with number 10.
  public static TupletBracketPoint DotPoint(float x,float y,float scale,EndingBracketResult r){float unit=F((double)scale*.25f);return new TupletBracketPoint(0,F((double)x*unit+r.DotX),F((double)y*unit+r.DotY));}
  public static EndingBracketResult Build(float[] p,TupletBracketContext c,bool printed,bool tomsTenth){
   if(p==null||p.Length<21||c==null)throw new ArgumentException("Missing ending context.");
   short subtype=I(p[8]),side=I(p[9]);bool tenth=subtype==10&&tomsTenth;
   if(subtype<1||subtype==3||subtype>9&&!tenth)throw new NotSupportedException("Not an ending bracket.");
   if(tenth)subtype=9;
   var r=new EndingBracketResult();float scale=c.Scale;
   float height=F((double)p[7]*scale*7),top=F((double)height+c.LeftY),right=F((double)p[6]*150);
   short start=1;
   if(side!=-1||subtype==2){r.Points.Add(new TupletBracketPoint(1,c.LeftX,c.LeftY));start=0;}
   r.Points.Add(new TupletBracketPoint(start,c.LeftX,top));
   r.Points.Add(new TupletBracketPoint(0,right,top));
   if(subtype==1)r.Points.Add(new TupletBracketPoint(0,right,c.LeftY));
   if(side<0)return r;
   short number=side>0?side:(tenth?(short)10:subtype);
   float numberY=F((double)top-(double)scale*28),along=F((double)scale*(printed?13.5f:14.5f));
   r.DotX=F((double)scale*(subtype==1?2:5)+c.LeftX+along);r.DotY=F((double)numberY+scale);r.HasDot=true;
   r.Number=new TupletPlacement{Number=number,X=TupletGeometry.Center(number,scale,along,c.LeftX),Y=numberY,FontScale=1};
   return r;
  }
 }
}
````

## EqualStepRaster

[Workspace source](../../ScoreReverseEngineering/NativeFragments/EqualStepRaster.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class EqualStepRaster {
  // Relative 1807:0A3D..0A95. SI is preserved; video effects external.
  public static VerticalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si){
   return Core(m,ds,es,ax,cx,dx,si,null);
  }
  public static VerticalRasterWords ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si,uint eflags){return Core(m,ds,es,ax,cx,dx,si,new RasterFlagState{Value=eflags});}
  static VerticalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si,RasterFlagState flags){
   ushort bx=(ushort)(ax<<1);cx=(ushort)((cx&0xff00)|8);ax=m.ReadWord(ds,0x4b68);
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV fault is outside this projection.");
   ax=(ushort)((ax/8)|((ax%8)<<8));cx=(ushort)((cx&0xff00)|(ax>>8));dx=(ushort)((dx&0xff00)|0x80);
   for(int n=0;n<(cx&255);n++){byte b=(byte)dx;dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}
   cx=m.ReadWord(ds,0x4b72);cx=(ushort)(cx+1);ax=(ushort)(ax&255);ushort bp=ax;
   ax=m.ReadWord(ds,0x4b6c);bool right=(short)ax>(short)m.ReadWord(ds,0x4b68);ushort di;
   do{di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);bx=(ushort)(bx+m.ReadWord(ds,0x4b78));
    byte b=(byte)dx;
    if(right){dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,0x80);if((byte)dx==0x80){if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,bp);bp=(ushort)(bp+1);}}
    else{dx=(ushort)((dx&0xff00)|((b<<1)&255)|(b>>7));if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,1);if((byte)dx==1){if(flags!=null)flags.Value=RasterStatusFlags.Dec16(flags.Value,bp);bp=(ushort)(bp-1);}}
    cx=(ushort)(cx-1);
   }while(cx!=0);
   return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
````

## EqualStepRasterFlagsModel

[Workspace source](../../ScoreReverseEngineering/NativeFragments/EqualStepRasterFlagsModel.cs)

````csharp
using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class EqualStepRasterFlagsModel {
 class Memory:IByteOrResultMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public byte OrByteAndGetResult(ushort s,ushort o,byte mask){OrByte(s,o,mask);return Bytes[(s<<4)+o];}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(VerticalRasterWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP+":"+w.EFlags;}
 static VerticalRasterWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si,uint inputFlags){uint fullFlags=inputFlags;int pc=0,steps=0;ushort bx=0,di=0,bp=0;bool less=false,equal=false;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<2000000){switch(f()){
 case 0x8b:byte op=f();if(op==0xd8)bx=ax;else if(op==0xe8)bp=ax;else if(op==0xbf)di=m.ReadWord(ds,(ushort)(bx+word()));else if(op==0x0e)cx=m.ReadWord(ds,word());else throw new Exception("MOV");break;
 case 0xd1:if(f()!=0xe3)throw new Exception("SHL");bx=(ushort)(bx<<1);break;
 case 0xa1:ax=m.ReadWord(ds,word());break;case 0xb1:cx=(ushort)((cx&0xff00)|f());break;
 case 0xf6:if(f()!=0xf1)throw new Exception("DIV opcode");int v=cx&255;if(v==0||ax/v>255)throw new Exception("Excluded fault");ax=(ushort)((ax/v)|((ax%v)<<8));break;
 case 0x8a:if(f()!=0xcc)throw new Exception("MOV CL");cx=(ushort)((cx&0xff00)|(ax>>8));break;
 case 0xb2:dx=(ushort)((dx&0xff00)|f());break;
 case 0xd2:if(f()!=0xca)throw new Exception("ROR");for(int n=0;n<(cx&255);n++){byte b=(byte)dx;dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}break;
 case 0xd0:byte rot=f(),r=(byte)dx;if(rot==0xca)dx=(ushort)((dx&0xff00)|(r>>1)|((r&1)<<7));else if(rot==0xc2)dx=(ushort)((dx&0xff00)|((r<<1)&255)|(r>>7));else throw new Exception("Rotate1");break;
 case 0x2a:if(f()!=0xe4)throw new Exception("SUB AH");ax=(ushort)(ax&255);break;
 case 0x3b:if(f()!=6)throw new Exception("CMP word");ushort rhs=m.ReadWord(ds,word());less=(short)ax<(short)rhs;equal=ax==rhs;break;
 case 0x7f:sbyte jg=(sbyte)f();if(!less&&!equal)pc+=jg;break;case 0x75:sbyte jne=(sbyte)f();if(!equal)pc+=jne;break;
 case 0x41:cx=(ushort)(cx+1);break;case 0x45:fullFlags=(IndependentFlagArithmetic.Apply(fullFlags,bp,1,16,true)&~1u)|(fullFlags&1u);bp=(ushort)(bp+1);break;case 0x4d:fullFlags=(IndependentFlagArithmetic.Apply(fullFlags,bp,1,16,false)&~1u)|(fullFlags&1u);bp=(ushort)(bp-1);break;
 case 0x03:byte add=f();if(add==0xfd)di=(ushort)(di+bp);else if(add==0x1e){ushort step=m.ReadWord(ds,word());fullFlags=IndependentFlagArithmetic.Apply(fullFlags,bx,step,16,true);bx=(ushort)(bx+step);}else throw new Exception("ADD");break;
 case 0x26:if(f()!=8||f()!=0x15)throw new Exception("OR");m.OrByte(es,di,(byte)dx);fullFlags=IndependentFlagArithmetic.Result(fullFlags,0,((Memory)m).Bytes[(es<<4)+di],8);break;
 case 0x80:if(f()!=0xfa)throw new Exception("CMP byte");byte byteRhs=f();fullFlags=IndependentFlagArithmetic.Apply(fullFlags,(byte)dx,byteRhs,8,false);equal=(byte)dx==byteRhs;break;
 case 0xe2:sbyte d=(sbyte)f();cx=(ushort)(cx-1);if(cx!=0)pc+=d;break;
 case 0xea:if(word()!=0xb16||word()!=0x1807)throw new Exception("Far exit");pc=code.Length;break;
 case 0xe9:short jump=(short)word();if(0xa3d+pc+jump!=0xb16)throw new Exception("Near exit");pc=code.Length;break;
 default:throw new Exception("Opcode at "+(pc-1));
 }}if(pc!=code.Length)throw new Exception("Exit");return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=fullFlags};}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 class Captured:IHorizontalRasterMemory {public byte[] Data,Plane;public ushort DS,ES;public int Writes;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){if(s!=ES)throw new Exception("Video segment");Plane[o]|=v;Writes++;}}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[89];Array.Copy(raw,0x18070+0xa3d,code,0,89);
 if(args.Length>1){string p=args[1];byte[] live=File.ReadAllBytes(Path.Combine(p,"raster-code.bin")),relocated=(byte[])code.Clone();foreach(int at in new[]{0xa77}){int i=at-0xa3d+3;relocated[i]=0x2b;relocated[i+1]=0x20;}if(live.Length!=89)throw new Exception("Live length");for(int i=0;i<89;i++)if(live[i]!=relocated[i])throw new Exception("Live code");Captured m=new Captured{DS=H(args[2]),ES=H(args[3]),Data=File.ReadAllBytes(Path.Combine(p,"raster-before-ds.bin")),Plane=File.ReadAllBytes(Path.Combine(p,"raster-before-plane2.bin"))};var r=EqualStepRaster.Execute(m,m.DS,m.ES,H(args[4]),H(args[5]),H(args[6]),H(args[7]));var e=new VerticalRasterWords{AX=H(args[8]),BX=H(args[9]),CX=H(args[10]),DX=H(args[11]),SI=H(args[12]),DI=H(args[13]),BP=H(args[14])};if(Show(r)!=Show(e))throw new Exception("Runtime words "+Show(r)+" != "+Show(e));byte[] after=File.ReadAllBytes(Path.Combine(p,"raster-after-ds.bin")),plane=File.ReadAllBytes(Path.Combine(p,"raster-after-plane2.bin"));if(m.Data.Length!=65536||m.Plane.Length!=65536||after.Length!=65536||plane.Length!=65536)throw new Exception("Capture lengths");for(int i=0;i<65536;i++)if(m.Data[i]!=after[i]||m.Plane[i]!=plane[i])throw new Exception("Runtime memory "+i);Console.WriteLine("PASS original equal-step block: 89 relocated code bytes, seven result words, all 65536 DS and 65536 plane2 bytes; "+m.Writes+" explicit plane2 OR effects.");return 0;}
 int count=0;for(int x=0;x<640;x++)for(int k=0;k<32;k++){ushort ds=0x2000,es=(ushort)((k&1)==0?0x4000:ds),row=(ushort)(k*1021),cx=(ushort)(x^0xa55a),dx=(ushort)(k*701),si=(ushort)(x*13);Memory seed=new Memory{Pattern=k};seed.Seed(ds,0x4b68,(ushort)x);seed.Seed(ds,0x4b6c,(ushort)(x+((k&4)==0?3:-3)));seed.Seed(ds,0x4b72,(ushort)(k%16));seed.Seed(ds,0x4b78,(ushort)((k&2)==0?2:0xfffe));Memory a=seed.Copy(),b=seed.Copy();var n=EqualStepRaster.ExecuteWithFlags(a,ds,es,row,cx,dx,si,0xA55A1234u ^ row);var q=Interpret(code,b,ds,es,row,cx,dx,si,0xA55A1234u ^ row);if(Show(n)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model state");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effect");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Model memory");count++;}
 Console.WriteLine("PASS full EFLAGS "+count+" instruction-model cases: 640 columns, both direction branches, masks, word wrapping, row strides and data/video aliases. Ordinary RAM OR model; successful DIV paths only.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
````

## FormatterWordWriter

[Workspace source](../../ScoreReverseEngineering/NativeFragments/FormatterWordWriter.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IFormatterWordContext {
  ushort ReadWord(ushort offset);
  void WriteByte(ushort offset, byte value);
  void WriteWord(ushort offset, ushort value);
 }
 public static class FormatterWordWriter {
  // Original 1F08:2A4B data effects only. The context supplies the actual DS
  // memory operations. This is not an ABI, stack, register or flag adapter.
  public static void Execute(IFormatterWordContext context, ushort low, ushort high) {
   if(context==null)throw new ArgumentNullException("context");
   ushort cursor=context.ReadWord(0xd988);
   int value=unchecked((int)(((uint)high<<16)|low));
   if(value>=-120 && value<=120) {
    context.WriteByte(cursor,unchecked((byte)low));
    cursor=unchecked((ushort)(cursor+1));
   } else {
    context.WriteByte(cursor,0x7f);
    cursor=unchecked((ushort)(cursor+1));
    context.WriteWord(cursor,low);
    context.WriteWord(unchecked((ushort)(cursor+2)),high);
    cursor=unchecked((ushort)(cursor+4));
   }
   context.WriteWord(0xd988,cursor);
  }
 }
}
````

## HorizontalRaster

[Workspace source](../../ScoreReverseEngineering/NativeFragments/HorizontalRaster.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IHorizontalRasterMemory {
  ushort ReadWord(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
  // Original byte OR instruction effect. A VGA bus/plane model is external.
  void OrByte(ushort segment,ushort offset,byte mask);
 }
 public struct HorizontalRasterWords {public ushort AX,BX,CX,DX,SI,DI;public uint EFlags,DefinedEFlagsMask;}
 public static class HorizontalRaster {
  static ushort DivideByEight(ushort ax){
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV overflow path is outside this projection.");
   return (ushort)((ax/8)|((ax%8)<<8));
  }
  // 1807:0AC6..0B15, after clipping/preparation and before register POPs.
  // Successful original byte-DIV paths only; no full flags/exception/VGA ABI.
  public static HorizontalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx){
   return Core(m,ds,es,ax,cx,dx,null);
  }
  public static HorizontalRasterWords ExecuteWithFlags(IByteOrResultMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,uint eflags){return Core(m,ds,es,ax,cx,dx,new RasterFlagState{Value=eflags});}
  static HorizontalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,RasterFlagState flags){
   if(m==null)throw new ArgumentNullException("m");
   ushort bx=unchecked((ushort)(ax<<1));ushort di=m.ReadWord(ds,unchecked((ushort)(bx+0x456a)));ushort si=di;
   ax=m.ReadWord(ds,0x4b68);
   if(unchecked((short)ax)>=unchecked((short)m.ReadWord(ds,0x4b6c))){ushort old=ax;ax=m.ReadWord(ds,0x4b6c);m.WriteWord(ds,0x4b6c,old);}
   cx=(ushort)((cx&0xff00)|8);ax=DivideByEight(ax);cx=(ushort)((cx&255)|(ax&0xff00));ax=(ushort)(ax&255);di=unchecked((ushort)(di+ax));
   ax=m.ReadWord(ds,0x4b6c);ax=DivideByEight(ax);cx=(ushort)((cx&0xff00)|7);cx=(ushort)((cx&0xff00)|unchecked((byte)((cx&255)-(ax>>8))));ax=(ushort)(ax&255);si=unchecked((ushort)(si+ax));
   dx=(ushort)((dx&0xff00)|unchecked((byte)(255<<(cx&255))));cx=(ushort)((cx&0xff00)|(cx>>8));bx=(ushort)((bx&0xff00)|unchecked((byte)(255>>(cx&255))));
   if(si>di){m.OrByte(es,di,(byte)bx);ax=(ushort)((ax&0xff00)|255);while(true){di=unchecked((ushort)(di+1));if(di>=si)break;m.OrByte(es,di,(byte)ax);}}
   else dx=(ushort)((dx&0xff00)|((dx&255)&(bx&255)));
   if(flags==null)m.OrByte(es,di,(byte)dx);else flags.Value=RasterStatusFlags.Logical8(flags.Value,((IByteOrResultMemory)m).OrByteAndGetResult(es,di,(byte)dx));
   return new HorizontalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
````

## HorizontalRasterFlagsModel

[Workspace source](../../ScoreReverseEngineering/NativeFragments/HorizontalRasterFlagsModel.cs)

````csharp
using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class HorizontalRasterFlagsModel {
 class Memory:IByteOrResultMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public byte OrByteAndGetResult(ushort s,ushort o,byte mask){OrByte(s,o,mask);return Bytes[(s<<4)+o];}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(HorizontalRasterWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.EFlags;}
 static HorizontalRasterWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,uint inputFlags){uint fullFlags=inputFlags;int pc=0,steps=0;ushort bx=0,si=0,di=0;bool sl=false,uc=false,z=false;Action<ushort,ushort> cmp=(a,b)=>{fullFlags=IndependentFlagArithmetic.Apply(fullFlags,a,b,16,false);sl=(short)a<(short)b;uc=a<b;z=a==b;};Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<10000){switch(f()){
  case 0x8b:byte mov=f();if(mov==0xd8)bx=ax;else if(mov==0xbf)di=m.ReadWord(ds,(ushort)(bx+word()));else if(mov==0xf7)si=di;else throw new Exception("MOV");break;
  case 0xd1:if(f()!=0xe3)throw new Exception("SHL BX");bx=(ushort)(bx<<1);break;
  case 0xa1:ax=m.ReadWord(ds,word());break;case 0x3b:byte compare=f();if(compare==6)cmp(ax,m.ReadWord(ds,word()));else if(compare==0xf7)cmp(si,di);else if(compare==0xfe)cmp(di,si);else throw new Exception("CMP");break;
  case 0x7c:sbyte jl=(sbyte)f();if(sl)pc+=jl;break;case 0x76:sbyte jbe=(sbyte)f();if(uc||z)pc+=jbe;break;case 0x73:sbyte jae=(sbyte)f();if(!uc)pc+=jae;break;case 0xeb:sbyte j=(sbyte)f();pc+=j;break;
  case 0x87:if(f()!=6)throw new Exception("XCHG");ushort at=word(),old=m.ReadWord(ds,at);m.WriteWord(ds,at,ax);ax=old;break;
  case 0xb1:cx=(ushort)((cx&0xff00)|f());break;case 0xb2:dx=(ushort)((dx&0xff00)|f());break;case 0xb3:bx=(ushort)((bx&0xff00)|f());break;case 0xb0:ax=(ushort)((ax&0xff00)|f());break;
  case 0xf6:if(f()!=0xf1)throw new Exception("DIV");int divisor=cx&255;if(divisor==0||ax/divisor>255)throw new Exception("Original divide-fault case outside model");ax=(ushort)((ax/divisor)|((ax%divisor)<<8));break;
  case 0x8a:byte part=f();if(part==0xec)cx=(ushort)((cx&255)|(ax&0xff00));else if(part==0xcd)cx=(ushort)((cx&0xff00)|(cx>>8));else throw new Exception("MOV byte");break;
  case 0x2a:byte sub=f();if(sub==0xe4)ax=(ushort)(ax&255);else if(sub==0xcc)cx=(ushort)((cx&0xff00)|(byte)((cx&255)-(ax>>8)));else throw new Exception("SUB byte");break;
  case 0x03:byte add=f();if(add==0xf8)di=(ushort)(di+ax);else if(add==0xf0)si=(ushort)(si+ax);else throw new Exception("ADD");break;
  case 0xd2:byte shift=f();if(shift==0xe2)dx=(ushort)((dx&0xff00)|(byte)((dx&255)<<(cx&255)));else if(shift==0xeb)bx=(ushort)((bx&0xff00)|(byte)((bx&255)>>(cx&255)));else throw new Exception("Shift byte");break;
  case 0x26:if(f()!=8)throw new Exception("OR byte");byte operand=f();if(operand==0x1d)m.OrByte(es,di,(byte)bx);else if(operand==5)m.OrByte(es,di,(byte)ax);else if(operand==0x15)m.OrByte(es,di,(byte)dx);else throw new Exception("OR source");fullFlags=IndependentFlagArithmetic.Result(fullFlags,0,((Memory)m).Bytes[(es<<4)+di],8);break;
  case 0x47:di=(ushort)(di+1);break;case 0x22:if(f()!=0xd3)throw new Exception("AND");dx=(ushort)((dx&0xff00)|((dx&255)&(bx&255)));break;
  default:throw new Exception("Opcode");
 }}if(pc!=code.Length)throw new Exception("Model exit");return new HorizontalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,EFlags=fullFlags};}
 static void Check(byte[] code,ushort x0,ushort x1,ushort row,ushort rowBase,bool alias){ushort ds=0x2000,es=(ushort)(alias?ds:0x4000),cx=(ushort)(row^0xa55a),dx=(ushort)(x0^0x1357);Memory seed=new Memory{Pattern=row&3};seed.Seed(ds,(ushort)((ushort)(row<<1)+0x456a),rowBase);seed.Seed(ds,0x4b68,x0);seed.Seed(ds,0x4b6c,x1);Memory a=seed.Copy(),b=seed.Copy();HorizontalRasterWords p=HorizontalRaster.ExecuteWithFlags(a,ds,es,row,cx,dx,0xA55A1234u ^ row),q=Interpret(code,b,ds,es,row,cx,dx,0xA55A1234u ^ row);if(Show(p)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Result/extent");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effects");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Memory");}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 class Captured:IHorizontalRasterMemory {public byte[] Data,Plane;public ushort DS,ES;public int Writes;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){if(s!=ES)throw new Exception("Video segment");Plane[o]|=v;Writes++;}}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[80];Array.Copy(raw,0x18070+0xac6,code,0,80);if(code[0]!=0x8b||code[77]!=0x26||code[79]!=0x15)throw new Exception("Original boundary");
  if(args.Length>1){string p=args[1];byte[] live=File.ReadAllBytes(Path.Combine(p,"raster-code.bin"));if(live.Length!=80)throw new Exception("Live length");for(int i=0;i<80;i++)if(live[i]!=code[i])throw new Exception("Live code");Captured m=new Captured{DS=H(args[2]),ES=H(args[3]),Data=File.ReadAllBytes(Path.Combine(p,"raster-before-ds.bin")),Plane=File.ReadAllBytes(Path.Combine(p,"raster-before-plane2.bin"))};HorizontalRasterWords r=HorizontalRaster.Execute(m,m.DS,m.ES,H(args[4]),H(args[5]),H(args[6]));HorizontalRasterWords e=new HorizontalRasterWords{AX=H(args[7]),BX=H(args[8]),CX=H(args[9]),DX=H(args[10]),SI=H(args[11]),DI=H(args[12])};if(Show(r)!=Show(e))throw new Exception("Runtime words "+Show(r)+" != "+Show(e));byte[] after=File.ReadAllBytes(Path.Combine(p,"raster-after-ds.bin")),plane=File.ReadAllBytes(Path.Combine(p,"raster-after-plane2.bin"));if(m.Data.Length!=65536||after.Length!=65536||m.Plane.Length!=65536||plane.Length!=65536)throw new Exception("Capture length");for(int i=0;i<65536;i++)if(m.Data[i]!=after[i]||m.Plane[i]!=plane[i])throw new Exception("Runtime data/plane "+i.ToString("X4"));Console.WriteLine("PASS original horizontal raster: 80 live code bytes, six result words, all 65536 DS and 65536 plane2 bytes; "+m.Writes+" OR effects. Plane2 byte-OR is an explicit fixture effect model, not a general VGA bus implementation.");return 0;}
  int count=0;for(int x0=0;x0<640;x0++)for(int x1=0;x1<640;x1++){Check(code,(ushort)x0,(ushort)x1,433,34640,false);count++;}for(int row=0;row<65536;row++)for(int k=0;k<4;k++){ushort[] left={0,1,7,20},right={0,7,8,620},bases={65530,0,0x4b68,34640};Check(code,left[k],right[k],(ushort)row,bases[k],true);count++;}Console.WriteLine("PASS full EFLAGS "+count+" original-instruction horizontal-raster comparisons: every 640-wide endpoint pair, all row words, mask boundaries, swaps, address wrap, data/video aliases and ordered byte-OR effects.");Console.WriteLine("Scope: successful original byte-DIV paths only; clipping, divide faults, complete CPU/VGA semantics and other line cases are outside this projection.");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
````

## IncludedEpsTransform

[Workspace source](../../ScoreReverseEngineering/NativeFragments/IncludedEpsTransform.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class IncludedEpsTransform {
  public float X,Y,ScaleX,ScaleY,BoxX,BoxY,Rotation;public bool Rotate;
  static float F(double v){return (float)v;}static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  public static IncludedEpsTransform Prepare(PageItemContext page,float staffBase,float vertical,float horizontalSize,float verticalSize,float boxLeft,float boxBottom,float rotation,float printSize,int leftOffset,int bottomOffset){
   float global=F((double)printSize/0.017999999225139618),staff=F((double)page.Scale/25),width=Present(horizontalSize)?horizontalSize:1,height=Present(verticalSize)?verticalSize:width;long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(rotation),0),out n))throw new NotSupportedException("EPS rotation outside the verified finite word conversion domain.");
   return new IncludedEpsTransform{X=F(((double)leftOffset+page.X)*printSize),Y=F(((double)staffBase+(double)vertical*staff*175+bottomOffset)*printSize),ScaleX=F((double)F((double)staff*width)*global),ScaleY=F((double)F((double)staff*height)*global),BoxX=F(-(double)boxLeft),BoxY=F(-(double)boxBottom),Rotate=unchecked((short)n)!=0,Rotation=F(-(double)rotation)};
  }
 }
}
````

## IncludedPostScriptVariables

[Workspace source](../../ScoreReverseEngineering/NativeFragments/IncludedPostScriptVariables.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original relative 2486:004A..01AB, observed at runtime 2CAA.
 // This models arithmetic for the P11 != -9999 branch only. It does not
 // format its decimal output, search for a file, or execute PostScript.
 public sealed class IncludedPostScriptVariables {
  public float GlobalSize,LineWidth,LeftOffset,BottomOffset,StaffLevel,PointsLevel,StaffSize,PP3,PP4,PP5,PP6;
  static float F(double value){return (float)value;}
  public static IncludedPostScriptVariables Prepare(PageItemContext item,float staffLevel,float p5,float p6,float printSize,float lineWidth,int leftOffset,int bottomOffset){
   var r=new IncludedPostScriptVariables();r.GlobalSize=F((double)printSize/0.017999999225139618);r.StaffSize=F((double)item.Scale/25);r.LineWidth=F((double)lineWidth*printSize);r.LeftOffset=F((double)leftOffset*printSize);r.BottomOffset=F((double)bottomOffset*printSize);r.StaffLevel=staffLevel;r.PointsLevel=F(((double)bottomOffset+staffLevel)*0.017999999225139618);r.PP3=item.X;r.PP4=item.Y;
   float step=F((double)r.StaffSize*175);r.PP5=F((double)step*p5);r.PP6=F((double)p6*step);return r;
  }
 }
}
````

## IndependentFlagArithmetic

[Workspace source](../../ScoreReverseEngineering/NativeFragments/IndependentFlagArithmetic.cs)

````csharp
using System;
static class IndependentFlagArithmetic {
 public static uint Apply(uint prior,ushort a,ushort b,int width,bool add){int mask=width==8?255:65535,sign=width==8?128:32768;a=(ushort)(a&mask);b=(ushort)(b&mask);int n=add?a+b:a-b;int result=n&mask;int sa=a>=sign?a-(mask+1):a,sb=b>=sign?b-(mask+1):b,signed=add?sa+sb:sa-sb;uint bits=0;if(add?n>mask:n<0)bits|=1;if(add?(a&15)+(b&15)>15:(a&15)<(b&15))bits|=16;if(signed < -sign || signed>=sign)bits|=2048;return Result(prior,bits,(ushort)result,width);}
 public static uint Result(uint prior,uint bits,ushort result,int width){int mask=width==8?255:65535,sign=width==8?128:32768;int r=result&mask;if(r==0)bits|=64;if((r&sign)!=0)bits|=128;bool odd=false;for(int n=0;n<8;n++)if((r&(1<<n))!=0)odd=!odd;if(!odd)bits|=4;return (prior&~0x8d5u)|bits;}
}
````

## ItemDrawingDispatch

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ItemDrawingDispatch.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct ItemDrawingRoute {public bool UsesTable;public ushort TableByteIndex,Target;}
 public static class ItemDrawingDispatch {
  // Relative 12DD:0542..0554 route/data projection. The table stays in
  // supplied memory; no original targets or resource contents are embedded.
  // Bypass means execution continues at 0575, not an unsupported-item policy.
  // AX/BX exchange, flags, ABI and the secondary table are outside this API.
  public static ItemDrawingRoute Select(IDatTextLookupMemory memory,ushort ds,ushort cs){
   if(memory==null)throw new ArgumentNullException("memory");
   ushort index=unchecked((ushort)(memory.ReadWord(ds,0x7e50)-1));
   if(index>15)return new ItemDrawingRoute{UsesTable=false};
   ushort byteIndex=(ushort)(index*2);
   return new ItemDrawingRoute{UsesTable=true,TableByteIndex=byteIndex,
    Target=memory.ReadWord(cs,unchecked((ushort)(0x0555+byteIndex)))};
  }
 }
}
````

## LineClipping

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LineClipping.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct LineClippingWords {public ushort AX,BX,CX,DX,SI,DI,Exit;}
 public struct LineClippingFlagResult {public LineClippingWords Words;public uint EFlags;}
 public sealed class LineClippingDivideFault:System.Exception {
  public LineClippingWords Words;public uint EFlags;public bool HasEFlags;
  public LineClippingDivideFault():base("Original clipping IDIV requests vector-zero delivery."){}
 }
 public static class LineClipping {
  static void Multiply(ref ushort ax,ref ushort dx,ushort b,FlagState flags){int n=(short)ax*(short)b;ax=(ushort)n;dx=(ushort)(n>>16);
   // Explicit DOSBox-X 2026.10.01 normal-core policy for architecturally undefined IMUL flags.
   if(flags!=null){uint bits=0;if(ax==0)bits|=64;if((ax&0x8000)!=0)bits|=128;if(n<short.MinValue||n>short.MaxValue)bits|=0x801;flags.Value=(flags.Value&~0x8c1u)|bits;}}
  static void Divide(ref ushort ax,ref ushort dx,ushort b,ushort site,ushort bx,ushort si,ushort di,FlagState flags){int n=unchecked((int)(((uint)dx<<16)|ax));int d=(short)b;
   bool bad=d==0||(n==int.MinValue&&d==-1);int q=bad?0:n/d;
   if(bad||q<short.MinValue||q>short.MaxValue)throw new LineClippingDivideFault{Words=new LineClippingWords{AX=ax,BX=bx,CX=b,DX=dx,SI=si,DI=di,Exit=site},EFlags=flags==null?0:flags.Value,HasEFlags=flags!=null};
   dx=(ushort)(n%d);ax=(ushort)q;}
  sealed class FlagState {
   public uint Value;
   public void Subtract(ushort a,ushort b){ushort r=(ushort)(a-b);uint bits=0;if(a<b)bits|=1;if(((a^b^r)&16)!=0)bits|=16;if(r==0)bits|=64;if((r&0x8000)!=0)bits|=128;if(((a^b)&(a^r)&0x8000)!=0)bits|=0x800;
    int parity=r&255;parity^=parity>>4;parity^=parity>>2;parity^=parity>>1;if((parity&1)==0)bits|=4;Value=(Value&~0x8d5u)|bits;}
  }
  // Original relative 1807:078C..0980. Normal exits have defined arithmetic flags.
  public static LineClippingFlagResult ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di,uint eflags){
   FlagState f=new FlagState{Value=eflags};LineClippingWords words=ExecuteCore(m,ds,ax,bx,cx,dx,si,di,f);return new LineClippingFlagResult{Words=words,EFlags=f.Value};
  }
  public static LineClippingWords Execute(IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di){
   return ExecuteCore(m,ds,ax,bx,cx,dx,si,di,null);
  }
  static LineClippingWords ExecuteCore(IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di,FlagState flags){
   System.Func<ushort,ushort,int> compare=(a,b)=>{if(flags!=null)flags.Subtract(a,b);return ((short)a).CompareTo((short)b);};
   int iterations=0;ushort temp;bool positive;
   if(compare(ax,m.ReadWord(ds,0x4a7c))>=0)goto Reject;
   cx=m.ReadWord(ds,0x4b6e);if(compare(cx,m.ReadWord(ds,0x4a76))<0)goto Reject;
   cx=m.ReadWord(ds,0x4b68);if(compare(cx,dx)<=0)goto Ordered;
   temp=dx;dx=cx;cx=temp;
  Ordered:
   if(compare(cx,m.ReadWord(ds,0x4a80))>=0)goto Reject;
   if(compare(dx,m.ReadWord(ds,0x4a72))<0)goto Reject;
   if(compare(cx,dx)==0)goto Vertical;
   if(compare(bx,0)!=0)goto General;
   if(compare(cx,m.ReadWord(ds,0x4a72))<0)cx=m.ReadWord(ds,0x4a72);
   if(compare(dx,m.ReadWord(ds,0x4a80))>=0)dx=m.ReadWord(ds,0x4a7e);
   m.WriteWord(ds,0x4b68,cx);m.WriteWord(ds,0x4b6c,dx);goto Horizontal;
  Vertical:
   if(compare(ax,m.ReadWord(ds,0x4a76))<0){ax=m.ReadWord(ds,0x4a76);m.WriteWord(ds,0x4b6a,ax);}
   bx=m.ReadWord(ds,0x4b6e);
   if(compare(bx,m.ReadWord(ds,0x4a7c))>=0){bx=m.ReadWord(ds,0x4a7a);m.WriteWord(ds,0x4b6e,bx);}
   goto VerticalExit;
  General:
   if(compare(bx,0)==0)goto Horizontal;
   dx=m.ReadWord(ds,0x4b6c);temp=m.ReadWord(ds,0x4b68);positive=(short)temp<(short)dx;if(flags!=null)flags.Subtract(dx,temp);dx=(ushort)(dx-temp);
   if(dx==0){bx=m.ReadWord(ds,0x4b6e);goto VerticalExit;}
   m.WriteWord(ds,0x4b72,bx);si=bx;
   if(!positive)dx=(ushort)(-dx);
   m.WriteWord(ds,0x4b70,dx);di=dx;
  Reclip:
   if(++iterations>65536)throw new NotSupportedException("Non-terminating clipping projection boundary.");
   dx=m.ReadWord(ds,0x4b6e);ax=m.ReadWord(ds,0x4b6a);bx=m.ReadWord(ds,0x4b6c);cx=m.ReadWord(ds,0x4b68);
   di=(ushort)(cx-bx);si=(ushort)(ax-dx);
   if(compare(cx,m.ReadWord(ds,0x4a80))>=0)goto FirstRight;
   if(compare(cx,m.ReadWord(ds,0x4a72))<0)goto FirstLeft;
   if(compare(bx,m.ReadWord(ds,0x4a80))>=0)goto SecondRight;
   if(compare(bx,m.ReadWord(ds,0x4a72))<0)goto SecondLeft;
   if(compare(ax,m.ReadWord(ds,0x4a7c))>=0)goto FirstBottom;
   if(compare(ax,m.ReadWord(ds,0x4a76))<0)goto FirstTop;
   if(compare(dx,m.ReadWord(ds,0x4a7c))>=0)goto SecondBottom;
   if(compare(dx,m.ReadWord(ds,0x4a76))<0)goto SecondTop;
   dx=si;dx=(ushort)(-dx);m.WriteWord(ds,0x4b72,dx);bx=di;
   if(compare(bx,0)<=0){if(flags!=null)flags.Subtract(0,bx);bx=(ushort)(-bx);}
   m.WriteWord(ds,0x4b70,bx);di=bx;dx=di;bx=si;goto Sloped;
  FirstRight:
   cx=m.ReadWord(ds,0x4a7e);if(compare(m.ReadWord(ds,0x4b6c),cx)>0)goto Reject;
  AdjustFirstX:
   m.WriteWord(ds,0x4b68,cx);ax=cx;if(flags!=null)flags.Subtract(ax,bx);ax=(ushort)(ax-bx);cx=si;Multiply(ref ax,ref dx,cx,flags);cx=di;Divide(ref ax,ref dx,cx,0x08d3,bx,si,di,flags);
   ax=(ushort)(ax+m.ReadWord(ds,0x4b6e));m.WriteWord(ds,0x4b6a,ax);goto Reclip;
  FirstLeft:
   cx=m.ReadWord(ds,0x4a72);if(compare(m.ReadWord(ds,0x4b6c),cx)<0)goto Reject;goto AdjustFirstX;
  SecondRight:
   bx=m.ReadWord(ds,0x4a7e);if(compare(m.ReadWord(ds,0x4b68),bx)>0)goto Reject;
  AdjustSecondX:
   m.WriteWord(ds,0x4b6c,bx);ax=cx;if(flags!=null)flags.Subtract(ax,bx);ax=(ushort)(ax-bx);cx=si;Multiply(ref ax,ref dx,cx,flags);cx=di;Divide(ref ax,ref dx,cx,0x0908,bx,si,di,flags);
   ax=(ushort)(ax-m.ReadWord(ds,0x4b6a));ax=(ushort)(-ax);m.WriteWord(ds,0x4b6e,ax);goto Reclip;
  SecondLeft:
   bx=m.ReadWord(ds,0x4a72);if(compare(m.ReadWord(ds,0x4b68),bx)<0)goto Reject;goto AdjustSecondX;
  FirstBottom:
   ax=m.ReadWord(ds,0x4a7a);if(compare(m.ReadWord(ds,0x4b6e),ax)>0)goto Reject;
  AdjustFirstY:
   m.WriteWord(ds,0x4b6a,ax);if(flags!=null)flags.Subtract(ax,dx);ax=(ushort)(ax-dx);cx=di;Multiply(ref ax,ref dx,cx,flags);cx=si;Divide(ref ax,ref dx,cx,0x0936,bx,si,di,flags);
   ax=(ushort)(ax+bx);m.WriteWord(ds,0x4b68,ax);goto Reclip;
  FirstTop:
   ax=m.ReadWord(ds,0x4a76);if(compare(m.ReadWord(ds,0x4b6e),ax)<0)goto Reject;goto AdjustFirstY;
  SecondBottom:
   dx=m.ReadWord(ds,0x4a7a);if(compare(m.ReadWord(ds,0x4b6a),dx)>0)goto Reject;
  AdjustSecondY:
   m.WriteWord(ds,0x4b6e,dx);if(flags!=null)flags.Subtract(ax,dx);ax=(ushort)(ax-dx);cx=di;Multiply(ref ax,ref dx,cx,flags);cx=si;Divide(ref ax,ref dx,cx,0x0964,bx,si,di,flags);
   ax=(ushort)(ax-m.ReadWord(ds,0x4b68));ax=(ushort)(-ax);m.WriteWord(ds,0x4b6c,ax);goto Reclip;
  SecondTop:
   dx=m.ReadWord(ds,0x4a76);if(compare(m.ReadWord(ds,0x4b6a),dx)<0)goto Reject;goto AdjustSecondY;
  Horizontal:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0ac6};
  VerticalExit:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0a96};
  Sloped:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0981};
  Reject:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0b16};
  }
 }
}
````

## LineDriverBody

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LineDriverBody.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct LineDriverBodyResult {
  public SignedDivide16State State;public uint DefinedEFlagsMask;
  public ushort ClippingExit,RasterEntry;public SignedDivide16Fault Fault;public bool RetfExecuted;
 }
 public static class LineDriverBody {
  sealed class ControlMemory:IBufferedDrawMemory {
   readonly IHorizontalRasterMemory memory;
   public ControlMemory(IHorizontalRasterMemory m){memory=m;}
   public ushort ReadWord(ushort s,ushort o){return memory.ReadWord(s,o);}
   public void WriteWord(ushort s,ushort o,ushort v){memory.WriteWord(s,o,v);}
  }
  static void Apply(ref SignedDivide16State s,VerticalRasterWords r){s.AX=r.AX;s.BX=r.BX;s.CX=r.CX;s.DX=r.DX;s.SI=r.SI;s.DI=r.DI;s.BP=r.BP;}
  // Original body 1807:0738 through 0B27, stopped before RETF.
  // Video instruction effects remain supplied by the caller's memory adapter.
  public static LineDriverBodyResult Execute(IHorizontalRasterMemory memory,SignedDivide16State s){
   return ExecuteCore(memory,s,false);
  }
  public static LineDriverBodyResult ExecuteWithFlags(IByteOrResultMemory memory,SignedDivide16State s){return ExecuteCore(memory,s,true);}
  static LineDriverBodyResult ExecuteCore(IHorizontalRasterMemory memory,SignedDivide16State s,bool fullFlags){
   if(s.IP!=0x0738)throw new ArgumentException("Line driver body entry must be 0738.");
   ControlMemory control=new ControlMemory(memory);BufferedDrawWords selection=BufferedDrawControl.SelectRasterPath(control,s.DS);
   s.AX=selection.AX;s.CX=selection.CX;if(fullFlags)s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)s.CX,0);ushort clipExit=0,rasterEntry=0;
   if(!selection.Branch){
    s.SP=(ushort)(s.SP-2);memory.WriteWord(s.SS,s.SP,s.BP);
    s.SP=(ushort)(s.SP-2);memory.WriteWord(s.SS,s.SP,s.DI);
    s.SP=(ushort)(s.SP-2);memory.WriteWord(s.SS,s.SP,s.SI);
    LinePreparationWords p=LinePreparation.Execute(memory,s.DS,s.AX);
    s.AX=p.AX;s.BX=p.BX;s.CX=p.CX;s.DX=p.DX;s.DI=p.DI;s.ES=p.ES;s.IP=0x078c;
    SignedDivide16Result clipped=ClippingCpu.Execute(memory,s);s=clipped.State;clipExit=s.IP;
    if(clipped.Fault!=SignedDivide16Fault.None)return new LineDriverBodyResult{State=s,DefinedEFlagsMask=clipped.DefinedEFlagsMask,ClippingExit=clipExit,Fault=clipped.Fault};
    if(s.IP==0x0981){RasterDispatchWords d=RasterDispatch.Execute(memory,s.DS,s.BX,s.CX,s.DX,s.EFlags);s.BX=d.BX;s.CX=d.CX;s.DX=d.DX;s.EFlags=d.EFlags;s.IP=d.Exit;}
    rasterEntry=s.IP;
    switch(s.IP){
     case 0x09b8:VerticalRasterWords sl=fullFlags?SlopedRaster.ExecuteWithFlags(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX,s.EFlags):SlopedRaster.Execute(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX);Apply(ref s,sl);if(fullFlags)s.EFlags=sl.EFlags;break;
     case 0x0a3d:VerticalRasterWords eq=fullFlags?EqualStepRaster.ExecuteWithFlags(memory,s.DS,s.ES,s.AX,s.CX,s.DX,s.SI,s.EFlags):EqualStepRaster.Execute(memory,s.DS,s.ES,s.AX,s.CX,s.DX,s.SI);Apply(ref s,eq);if(fullFlags)s.EFlags=eq.EFlags;break;
     case 0x0a96:VerticalRasterWords v=fullFlags?VerticalRaster.ExecuteWithFlags(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX,s.EFlags):VerticalRaster.Execute(memory,s.DS,s.ES,s.AX,s.BX,s.CX,s.DX);Apply(ref s,v);if(fullFlags)s.EFlags=v.EFlags;break;
     case 0x0ac6:HorizontalRasterWords h=fullFlags?HorizontalRaster.ExecuteWithFlags((IByteOrResultMemory)memory,s.DS,s.ES,s.AX,s.CX,s.DX,s.EFlags):HorizontalRaster.Execute(memory,s.DS,s.ES,s.AX,s.CX,s.DX);if(fullFlags)s.EFlags=h.EFlags;s.AX=h.AX;s.BX=h.BX;s.CX=h.CX;s.DX=h.DX;s.SI=h.SI;s.DI=h.DI;break;
     case 0x0b16:break;
     default:throw new NotSupportedException("Unverified clipping/raster exit.");
    }
    s.SI=memory.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
    s.DI=memory.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
    s.BP=memory.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);
    s.AX=memory.ReadWord(s.DS,0xac42);
   }
   BufferedDrawWords position=BufferedDrawControl.UpdatePosition(control,s.DS,s.AX);s.AX=position.AX;s.CX=position.CX;s.IP=0x0b27;
   // Full flags require the explicit CPU-visible horizontal OR result contract.
   return new LineDriverBodyResult{State=s,DefinedEFlagsMask=fullFlags?0xffffffffu:~0x8d5u,ClippingExit=clipExit,RasterEntry=rasterEntry,Fault=SignedDivide16Fault.None};
  }
  public static LineDriverBodyResult ExecuteThroughReturnWithFlags(IByteOrResultMemory memory,SignedDivide16State s){
   return Return(memory,ExecuteWithFlags(memory,s));
  }
  public static LineDriverBodyResult ExecuteThroughReturn(IHorizontalRasterMemory memory,SignedDivide16State s){
   LineDriverBodyResult result=Execute(memory,s);
   return Return(memory,result);
  }
  static LineDriverBodyResult Return(IHorizontalRasterMemory memory,LineDriverBodyResult result){
   if(result.Fault==SignedDivide16Fault.None){SignedDivide16State s=result.State;ushort ip=memory.ReadWord(s.SS,s.SP),cs=memory.ReadWord(s.SS,(ushort)(s.SP+2));s.SP=(ushort)(s.SP+4);s.IP=ip;s.CS=cs;result.State=s;result.RetfExecuted=true;}
   return result;
  }
 }
}
````

## LinePreparation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LinePreparation.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public struct LinePreparationWords {public ushort AX,BX,CX,DX,DI,ES;}
 public static class LinePreparation {
  // Relative 1807:074C..078B; entry follows the three stack pushes.
  public static LinePreparationWords Execute(IHorizontalRasterMemory m,ushort ds,ushort ax){
   ushort cx=m.ReadWord(ds,0xac40);m.WriteWord(ds,0x4b68,cx);m.WriteWord(ds,0x4b6a,ax);
   ushort dx=m.ReadWord(ds,0xac44);m.WriteWord(ds,0x4b6c,dx);
   ushort di=m.ReadWord(ds,0xac46);m.WriteWord(ds,0x4b6e,di);ushort bx=di;
   m.WriteWord(ds,0x4b78,2);di=m.ReadWord(ds,0x4a26);ushort es=di;
   ushort original=bx;bx=(ushort)(bx-ax);
   // JGE after SUB compares signed operands, including its overflow flag.
   if((short)original<(short)ax){
    bx=(ushort)(-bx);ushort old=m.ReadWord(ds,0x4b6e);m.WriteWord(ds,0x4b6e,ax);ax=old;
    m.WriteWord(ds,0x4b6a,ax);ushort x=dx;dx=cx;cx=x;
    m.WriteWord(ds,0x4b68,cx);m.WriteWord(ds,0x4b6c,dx);
   }
   return new LinePreparationWords{AX=ax,BX=bx,CX=cx,DX=dx,DI=di,ES=es};
  }
 }
}
````

## Mode12Video

[Workspace source](../../ScoreReverseEngineering/NativeFragments/Mode12Video.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class RealModeIret16 {
  // Recorded DOSBox-X 386-class real-mode 16-bit IRET flag mask.
  public static SignedDivide16State Execute(IHorizontalRasterMemory m,SignedDivide16State s){ushort ip=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);ushort cs=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);ushort flags=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=ip;s.CS=cs;s.EFlags=(s.EFlags&~0x7fd5u)|((uint)flags&0x7fd5u)|2u;return s;}
 }
 public interface IMode12Surface {void WriteGlyph(int row,int column,byte character,byte color);void Scroll(int top,int left,int bottom,int right,int lines,byte background);}
 public interface IMode12GlyphSource {byte Row(byte character,int line);}
 public sealed class ExternalVgaFont:IMode12GlyphSource {
  readonly byte[] bytes;public ExternalVgaFont(byte[] data){if(data==null||data.Length!=4096)throw new ArgumentException("An external 256-glyph 8x16 font (4096 bytes) is required.");bytes=(byte[])data.Clone();}
  public byte Row(byte character,int line){if(line<0||line>=16)throw new ArgumentOutOfRangeException("line");return bytes[character*16+line];}
 }
 public sealed class Mode12Plane2Surface:IMode12Surface {
  public readonly byte[] Bytes;readonly IMode12GlyphSource font;
  public Mode12Plane2Surface(byte[] initial,IMode12GlyphSource glyphs){if(initial==null||initial.Length!=65536||glyphs==null)throw new ArgumentException("Plane-2 reference bytes and external glyph source required.");Bytes=(byte[])initial.Clone();font=glyphs;}
  public void WriteGlyph(int row,int column,byte character,byte color){if(row<0||row>=30||column<0||column>=80)throw new NotSupportedException("Glyph outside verified mode-12 viewport.");for(int y=0;y<16;y++){int at=(row*16+y)*80+column;byte bits=(color&4)!=0?font.Row(character,y):(byte)0;Bytes[at]=(byte)((color&0x80)!=0?(Bytes[at]^bits):bits);}}
  public void Scroll(int top,int left,int bottom,int right,int lines,byte background){int height=bottom-top+1;int shift=lines==0?height:Math.Min(lines,height);byte fill=(background&4)!=0?(byte)255:(byte)0;for(int y=top*16;y<=(bottom+1)*16-1;y++)for(int x=left;x<=right;x++)Bytes[y*80+x]=y+shift*16<(bottom+1)*16?Bytes[(y+shift*16)*80+x]:fill;}
 }
 public struct Mode12Glyph {public byte Character,Color;public Mode12Glyph(byte c,byte a){Character=c;Color=a;}}
 public sealed class Mode12CellSurface:IMode12Surface {
  readonly List<Mode12Glyph>[] cells=new List<Mode12Glyph>[80*30];readonly byte[] backgrounds=new byte[80*30];public long Revision{get;private set;}
  public Mode12CellSurface(){for(int i=0;i<cells.Length;i++)cells[i]=new List<Mode12Glyph>();}
  public byte Background(int row,int column){return backgrounds[row*80+column];}
  public Mode12Glyph[] Cell(int row,int column){return cells[row*80+column].ToArray();}
  public void WriteGlyph(int row,int column,byte character,byte color){if(row<0||row>=30||column<0||column>=80)throw new NotSupportedException("Glyph outside verified viewport.");var cell=cells[row*80+column];if((color&0x80)==0){cell.Clear();backgrounds[row*80+column]=0;}cell.Add(new Mode12Glyph(character,color));Revision++;}
  public void Scroll(int top,int left,int bottom,int right,int lines,byte background){int shift=lines==0?bottom-top+1:Math.Min(lines,bottom-top+1);for(int r=top;r<=bottom;r++)for(int c=left;c<=right;c++){var cell=cells[r*80+c];cell.Clear();if(r+shift<=bottom){cell.AddRange(cells[(r+shift)*80+c]);backgrounds[r*80+c]=backgrounds[(r+shift)*80+c];}else backgrounds[r*80+c]=background;}Revision++;}
  public void Clear(){foreach(var c in cells)c.Clear();Array.Clear(backgrounds,0,backgrounds.Length);Revision++;}
 }
 public sealed class Mode12VideoServices {
  readonly IMode12Surface surface;public Mode12VideoServices(IMode12Surface output){if(output==null)throw new ArgumentNullException("output");surface=output;}
  void Cursor(IScreenHookMemory m,int row,int col){m.WriteWord(0x40,0x50,(ushort)((row<<8)|col));}
  public SignedDivide16State ExecuteService(IScreenHookMemory m,SignedDivide16State s){
   if(m.ReadByte(0x40,0x49)!=0x12||m.ReadWord(0x40,0x4a)!=80||m.ReadByte(0x40,0x84)!=29||m.ReadWord(0x40,0x85)!=16||m.ReadByte(0x40,0x62)!=0)throw new NotSupportedException("Only measured 640x480 mode 12h / page zero / 8x16 cells is implemented.");
   byte ah=(byte)(s.AX>>8),al=(byte)s.AX;ushort cursor=m.ReadWord(0x40,0x50);int row=cursor>>8,col=cursor&255;
   if(ah==3){if((s.BX>>8)!=0)throw new NotSupportedException("Cursor page outside verified scope.");s.DX=cursor;s.CX=m.ReadWord(0x40,0x60);return s;}
   if(ah==2){if((s.BX>>8)!=0||s.DX>>8>=30||(s.DX&255)>=80)throw new NotSupportedException("Cursor outside verified viewport.");Cursor(m,s.DX>>8,s.DX&255);return s;}
   if(ah==6){if(al>127)throw new NotSupportedException("Signed scroll count outside verified scope.");int top=s.CX>>8,left=s.CX&255,bottom=Math.Min(29,s.DX>>8),right=Math.Min(79,s.DX&255);if(top<=bottom&&left<=right)surface.Scroll(top,left,bottom,right,al,(byte)(s.BX>>8));return s;}
   if(row>=30||col>=80)throw new NotSupportedException("Current cursor outside verified viewport.");
   if(ah==9||ah==10){if((s.BX>>8)!=0)throw new NotSupportedException("Glyph page outside verified scope.");for(int n=0;n<s.CX;n++){surface.WriteGlyph(row,col,al,(byte)s.BX);if(++col==80){col=0;row++;}}return s;}
   if(ah==14){switch(al){case 8:if(col>0)col--;break;case 13:col=0;break;case 10:row++;break;default:if(al<32)throw new NotSupportedException("Control character not yet verified.");surface.WriteGlyph(row,col,al,(byte)s.BX);col++;break;}if(col==80){col=0;row++;}if(row==30){surface.Scroll(0,0,29,79,1,0);row--;}Cursor(m,row,col);return s;}
   throw new NotSupportedException("Video service AH="+ah.ToString("X2")+" not verified.");
  }
 }
 public sealed class Mode12BiosTransfer:IScreenBiosTransferEffects {
  readonly Mode12VideoServices video;readonly ushort callbackSegment,iretOffset;
  public Mode12BiosTransfer(Mode12VideoServices services,ushort measuredCallbackSegment,ushort measuredIretOffset){if(services==null)throw new ArgumentNullException("services");video=services;callbackSegment=measuredCallbackSegment;iretOffset=measuredIretOffset;}
  public SignedDivide16State TransferToBios(IScreenHookMemory m,SignedDivide16State state){state=video.ExecuteService(m,state);state.CS=callbackSegment;state.IP=iretOffset;return RealModeIret16.Execute(m,state);}
 }
}
````

## NumericConverter

[Workspace source](../../ScoreReverseEngineering/NativeFragments/NumericConverter.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface INumericConverterContext {
  byte ReadByte(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public struct NumericConverterResult {
  public readonly ushort AX,DX,Status;
  public NumericConverterResult(ushort ax,ushort dx,ushort status){AX=ax;DX=dx;Status=status;}
 }
 public static class NumericConverter {
  // Literal data/result translation of original 1F08:1BD7 and 1C95.
  // Explicit segmented memory; no host number parser or ABI adapter.
  static byte Load(INumericConverterContext m,ushort segment,ref ushort si){byte b=m.ReadByte(segment,si);si=unchecked((ushort)(si+1));return b;}
  static ushort Classify(ref byte al,byte radix,out bool carry){
   if(al>=0x61)al=(byte)(al&0xdf);
   byte old=al;al=unchecked((byte)(al-0x30));carry=old<0x30;
   if(carry)return unchecked((ushort)(short)(sbyte)al);
   if(al>=0x0a){
    if(al<0x11){carry=true;return unchecked((ushort)(short)(sbyte)al);}
    al=unchecked((byte)(al-7));
   }
   carry=!(al<radix);return unchecked((ushort)(short)(sbyte)al);
  }
  public static NumericConverterResult Execute(INumericConverterContext m,ushort statusSegment,ushort statusOffset,ushort inputSegment,ushort inputOffset){
   if(m==null)throw new ArgumentNullException("m");
   ushort si=inputOffset,bx=0,di=0,cx=10,status=0,ax=0,oldHigh=0,carryWord=0,addend=0,prior=0;
   uint product=0,sum=0;bool negative=false,carry=false;byte al=Load(m,inputSegment,ref si),old=0;
   if(al==0x2b||al==0x2d){negative=al==0x2d;al=Load(m,inputSegment,ref si);}
  DecimalFast:
   old=al;al=unchecked((byte)(al-0x30));
   if(old<0x30)goto Terminal;
   if(al>=0x0a)goto StatusOne;
   ax=unchecked((ushort)(short)(sbyte)al);
   product=(uint)bx*cx;sum=(product&65535)+ax;bx=(ushort)sum;
   di=unchecked((ushort)(di+(product>>16)+(sum>>16)));
   al=Load(m,inputSegment,ref si);
   if(di==0)goto DecimalFast;
  Slow:
   ax=Classify(ref al,(byte)cx,out carry);if(carry)goto Terminal;
   product=(uint)bx*cx;sum=(product&65535)+ax;bx=(ushort)sum;
   carryWord=unchecked((ushort)((product>>16)+(sum>>16)));
   oldHigh=di;di=carryWord;product=(uint)oldHigh*cx;
   if((product>>16)!=0)goto StatusOne;
   addend=(ushort)product;prior=di;di=unchecked((ushort)(di+addend));
   if(((~(prior^addend)&(prior^di))&0x8000)!=0)goto StatusOne;
   al=Load(m,inputSegment,ref si);goto Slow;
  Terminal:
   if(al==0xf3)goto Radix;
   status=0;si=unchecked((ushort)(si-1));if(m.ReadByte(inputSegment,si)!=0)status=4;goto Finish;
  Radix:
   if(cx!=10){status=2;goto Finish;}
   if(bx==0)cx=16;
   else{cx=bx;if((byte)cx>0x24||(byte)cx<2){status=3;goto Finish;}}
   al=Load(m,inputSegment,ref si);di=0;bx=0;
  RadixFast:
   ax=Classify(ref al,(byte)cx,out carry);if(carry)goto Terminal;
   product=(uint)bx*cx;sum=(product&65535)+ax;bx=(ushort)sum;
   di=unchecked((ushort)(di+(product>>16)+(sum>>16)));
   al=Load(m,inputSegment,ref si);if(di==0)goto RadixFast;goto Slow;
  StatusOne:
   status=1;
  Finish:
   m.WriteWord(statusSegment,statusOffset,status);
   ax=bx;oldHigh=di;
   if(negative){ax=unchecked((ushort)-ax);oldHigh=unchecked((ushort)-(oldHigh+(bx!=0?1:0)));}
   return new NumericConverterResult(ax,oldHigh,status);
  }
 }
}
````

## OutputCountAdvance

[Workspace source](../../ScoreReverseEngineering/NativeFragments/OutputCountAdvance.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct OutputCountResult {
  public readonly ushort AX,DX;
  public OutputCountResult(ushort ax,ushort dx){AX=ax;DX=dx;}
 }
 public static class OutputCountAdvance {
  // Original 1F08:4863..486C. Count is read from the original argument word;
  // signed extension and carry follow CWD / ADD / ADC. Full flags/ABI excluded.
  public static OutputCountResult Execute(ISpanBookkeepingMemory m,ushort ss,ushort countOffset,ushort ds,ushort descriptorSi){
   if(m==null)throw new ArgumentNullException("m");
   ushort count=m.ReadWord(ss,countOffset);
   ushort highAdd=unchecked((short)count)<0?(ushort)0xffff:(ushort)0;
   ushort lowOffset=unchecked((ushort)(descriptorSi+0x12));
   uint sum=(uint)m.ReadWord(ds,lowOffset)+count;
   m.WriteWord(ds,lowOffset,(ushort)sum);
   ushort highOffset=unchecked((ushort)(descriptorSi+0x14));
   ushort high=m.ReadWord(ds,highOffset);
   m.WriteWord(ds,highOffset,unchecked((ushort)(high+highAdd+(sum>>16))));
   return new OutputCountResult(count,highAdd);
  }
 }
}
````

## PackedDrawingWord

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PackedDrawingWord.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public struct PackedDrawingParts { public short High,Middle,Low,FinalAX; }
 public static class PackedDrawingWord {
  // Relative 14A6:0E9C..0EC6 integer/data projection only. Signed IDIV
  // truncates toward zero. Names denote numeric positions, not symbol meanings.
  // No flags, x87 transforms, table bounds or drawing policy are supplied.
  public static PackedDrawingParts Decode(short word) {
   int high=word/10000;
   int remainder=word-high*10000;
   return new PackedDrawingParts {High=(short)high,Middle=(short)(remainder/100),
    Low=(short)(word-(word/100)*100),FinalAX=(short)(word/100)};
  }
 }
}
````

## PageCoordinates

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PageCoordinates.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct PageItemContext {public float Scale,X,Y,VerticalInput;public short HorizontalWord;public bool ScreenHorizontalLimit;}
 public struct PageStaffLine {public float Left,Right,Y;}
 public sealed class PageStaffResult {public float Scale,Base,Bottom,Spacing;public int LineCount;public short WidthSelector;public readonly List<PageStaffLine> Lines=new List<PageStaffLine>();}
 public sealed class PageCoordinates {
  public readonly float[] Scales=new float[33],Bases=new float[33];
  static float F(double v){return (float)v;}static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static short I(float v){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(v),0),out n))throw new NotSupportedException("Parameter outside the verified finite word conversion domain.");return unchecked((short)n);}
  public PageCoordinates(){for(int i=1;i<=32;i++){Scales[i]=25;Bases[i]=i*3150-27150-525;}}
  static void Staff(int s){if(s<1||s>32)throw new NotSupportedException("Staff index outside the measured 1..32 table.");}
  // Original relative 12DD:0002..01C2; explicit units-state input, no UI/ABI claim.
  public PageStaffResult SetStaff(float[] p,bool alternateUnits){return SetStaff(p,I(p[2]),I(p[10]),alternateUnits);}
  public PageStaffResult SetStaff(float[] p,int staff,short referenceWord,bool alternateUnits){
   if(p==null||p.Length<21)throw new ArgumentException("Missing staff parameters.");Staff(staff);
   float size=Present(p[5])?p[5]:1,scale=F((double)size*25);Scales[staff]=scale;
   float displacement=F((double)p[4]%1000);if(displacement>=300)throw new NotSupportedException("Original staff displacement error branch (remainder >=300).");
   float bottom;
   if(Present(p[10])){
    float distance=F(((double)p[10]%100)*4000);if(alternateUnits)distance=F((double)distance/2.5399999618530273f);
    int reference=referenceWord/100;if(reference==0)reference=1;Staff(reference);
    bottom=F((double)Scales[reference]*21+Bases[reference]+distance);
   }else bottom=F((double)displacement*scale*7+(staff*3150-27150));
   float baseline=F((double)bottom-(double)scale*21);Bases[staff]=baseline;
   var r=new PageStaffResult{Scale=scale,Base=baseline,Bottom=bottom,Spacing=F((double)scale*14)};
   short thickness=I(p[11]);r.WidthSelector=thickness==99?(short)0:thickness==0&&size<.6499999761581421f?(short)-1:thickness;
   if(I(p[7])<0)return r;
   int control=Math.Abs(I(p[4])/1000);if(I(p[7])>0)control=I(p[7]);
   if(control==0)control=5;
   else if(control>=100){int origin=control/100;if(origin>1)bottom=F((double)(origin-1)*r.Spacing+bottom);control%=100;}
   r.LineCount=control;float left=F((double)p[3]*150),right=Present(p[6])?F((double)p[6]*150):30000;
   for(int j=0;j<control;j++){r.Lines.Add(new PageStaffLine{Left=left,Right=right,Y=bottom});bottom=F((double)r.Spacing+bottom);}
   return r;
  }
  // Original relative 12DD:07F9..08C2, after the parameter cache selected a staff.
  public PageItemContext Prepare(float[] p,int staff,int code){
   Staff(staff);double horizontal=(double)p[3]*150;float vertical=p[4];if(code<=3||code==6)vertical=F((double)vertical%100);
   return new PageItemContext{Scale=Scales[staff],X=F(horizontal),Y=F((double)Scales[staff]*vertical*7+Bases[staff]),VerticalInput=vertical,HorizontalWord=I(F(horizontal)),ScreenHorizontalLimit=horizontal>32700};
  }
  // Original relative 12DD:08C3..08F5; the caller supplies its own field value.
  public float Vertical(int staff,float value){Staff(staff);return F((double)value*Scales[staff]*7+Bases[staff]);}
 }
}
````

## ParameterWordReader

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ParameterWordReader.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IParameterWordMemory {
  byte ReadByte(ushort segment,ushort offset);
  ushort ReadWord(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public static class ParameterWordReader {
  // Data effects of original 1F08:36E0..370A. The cursor is the far-pointer
  // local at SS:BP-8; destination is DS:SI. Preserve all memory-access order.
  // Returned word is the block's updated SI; full ABI/flags are not supplied.
  public static ushort Execute(IParameterWordMemory m,ushort cursorSegment,ushort cursorOffset,ushort destinationSegment,ushort destinationOffset){
   if(m==null)throw new ArgumentNullException("m");
   ushort source=m.ReadWord(cursorSegment,cursorOffset);
   ushort sourceSegment=m.ReadWord(cursorSegment,unchecked((ushort)(cursorOffset+2)));
   ushort cursor=m.ReadWord(cursorSegment,cursorOffset);
   m.WriteWord(cursorSegment,cursorOffset,unchecked((ushort)(cursor+1)));
   short value=unchecked((sbyte)m.ReadByte(sourceSegment,source));
   m.WriteWord(destinationSegment,destinationOffset,unchecked((ushort)value));
   m.WriteWord(destinationSegment,unchecked((ushort)(destinationOffset+2)),value<0?(ushort)0xffff:(ushort)0);
   if(m.ReadByte(destinationSegment,destinationOffset)==0x7f){
    source=m.ReadWord(cursorSegment,cursorOffset);
    sourceSegment=m.ReadWord(cursorSegment,unchecked((ushort)(cursorOffset+2)));
    ushort low=m.ReadWord(sourceSegment,source);
    ushort high=m.ReadWord(sourceSegment,unchecked((ushort)(source+2)));
    m.WriteWord(destinationSegment,destinationOffset,low);
    m.WriteWord(destinationSegment,unchecked((ushort)(destinationOffset+2)),high);
    cursor=m.ReadWord(cursorSegment,cursorOffset);
    m.WriteWord(cursorSegment,cursorOffset,unchecked((ushort)(cursor+4)));
   }
   return unchecked((ushort)(destinationOffset+4));
  }
 }
}
````

## PointBufferAppend

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PointBufferAppend.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IPointBufferMemory {ushort ReadWord(ushort segment,ushort offset);void WriteWord(ushort segment,ushort offset,ushort value);}
 public struct PointBufferWords {public ushort AX,BX,SI,ES;public uint EFlags,DefinedEFlagsMask;}
 public static class PointBufferAppend {
  // Original 0C25:07EB..0847 word/memory projection, after conversion/clamps
  // and before the epilogue. Marker values remain numeric, not named operations.
  public static PointBufferWords Execute(IPointBufferMemory m,ushort ds,ushort ss,ushort bp){
   return Core(m,ds,ss,bp,null);
  }
  public static PointBufferWords ExecuteWithFlags(IPointBufferMemory m,ushort ds,ushort ss,ushort bp,uint flags){return Core(m,ds,ss,bp,new RasterFlagState{Value=flags});}
  static PointBufferWords Core(IPointBufferMemory m,ushort ds,ushort ss,ushort bp,RasterFlagState flags){
   if(m==null)throw new ArgumentNullException("m");
   ushort es=m.ReadWord(ds,0x69da);ushort ax=m.ReadWord(es,0);m.WriteWord(ds,0xc58e,ax);
   ushort bx=m.ReadWord(ss,unchecked((ushort)(bp+6)));es=m.ReadWord(ss,unchecked((ushort)(bp+8)));ushort si=m.ReadWord(es,bx);
   if(unchecked((short)si)>0){
    bx=unchecked((ushort)(ax<<1));es=m.ReadWord(ds,0x69da);bx=unchecked((ushort)(bx+2));ax=unchecked((ushort)(si+0x8044));m.WriteWord(es,unchecked((ushort)(bx-2)),ax);
    m.WriteWord(ds,0xc58e,unchecked((ushort)(m.ReadWord(ds,0xc58e)+1)));
   }
   si=m.ReadWord(ds,0xc58e);if(flags!=null)flags.Value=(flags.Value&~1u)|((si&0x8000)!=0?1u:0u);si=unchecked((ushort)(si<<1));es=m.ReadWord(ds,0x69da);bx=unchecked((ushort)(si+2));ax=m.ReadWord(ds,0xc590);m.WriteWord(es,unchecked((ushort)(bx-2)),ax);
   es=m.ReadWord(ds,0x69dc);bx=si;ax=m.ReadWord(ds,0xc594);m.WriteWord(es,unchecked((ushort)(bx-2)),ax);
   es=m.ReadWord(ds,0x69da);ax=m.ReadWord(ds,0xc58e);if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,ax);ax=unchecked((ushort)(ax+1));m.WriteWord(es,0,ax);
   return new PointBufferWords{AX=ax,BX=bx,SI=si,ES=es,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
````

## PrefixWritePreparation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PrefixWritePreparation.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IPrefixPreparationMemory {
  byte ReadByte(ushort segment,ushort offset);
  ushort ReadWord(ushort segment,ushort offset);
  void WriteByte(ushort segment,ushort offset,byte value);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public struct PrefixPreparationResult {
  public readonly ushort AX,BP,SP,SI,DI,RelativeTargetSegment,TargetOffset;
  public PrefixPreparationResult(ushort ax,ushort bp,ushort sp,ushort si,ushort di){AX=ax;BP=bp;SP=sp;SI=si;DI=di;RelativeTargetSegment=0x18b9;TargetOffset=0x312e;}
 }
 public static class PrefixWritePreparation {
  static void Push(IPrefixPreparationMemory m,ushort ss,ref ushort sp,ushort value){sp=unchecked((ushort)(sp-2));m.WriteWord(ss,sp,value);}
  // Original 1F08:33FA..3458, through the far-call frame preparation.
  // Explicit entry words and memory; stops before the external callee.
  // Source sequences remain at original data addresses; no strings embedded.
  public static PrefixPreparationResult Prepare(IPrefixPreparationMemory m,ushort ss,ushort ds,ushort cs,ushort sp,ushort bp,ushort si,ushort di){
   if(m==null)throw new ArgumentNullException("m");
   Push(m,ss,ref sp,bp);bp=sp;sp=unchecked((ushort)(sp-6));Push(m,ss,ref sp,di);Push(m,ss,ref sp,si);
   si=2;di=0x6391;
   ushort descriptor=m.ReadWord(ds,0x6259);byte handle;
   if(m.ReadByte(ds,unchecked((ushort)(descriptor+2)))==0)handle=1;
   else{descriptor=m.ReadWord(ds,0x6259);handle=m.ReadByte(ds,unchecked((ushort)(descriptor+2)));}
   m.WriteByte(ss,unchecked((ushort)(bp-6)),handle);
   byte prefix=m.ReadByte(ss,unchecked((ushort)(bp+4)));
   if(prefix==0x2b)si=1;
   else if(prefix==0x30)si=4;
   else if(prefix==0x31){di=0x638b;si=6;}
   Push(m,ss,ref sp,si);Push(m,ss,ref sp,ds);Push(m,ss,ref sp,di);
   ushort ax=unchecked((ushort)(short)(sbyte)m.ReadByte(ss,unchecked((ushort)(bp-6))));
   Push(m,ss,ref sp,ax);
   Push(m,ss,ref sp,cs);Push(m,ss,ref sp,0x3459);
   return new PrefixPreparationResult(ax,bp,sp,si,di);
  }
 }
}
````

## PrimitiveTextGeometry

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PrimitiveTextGeometry.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public struct PrimitiveTextScale {public float X,Y,OriginX,OriginY;}
 public struct PrimitiveTextPoint {public float X,Y;public short Operation;public bool CommandOnly;public short? RetainedMarkerY;public int MarkerWordBack;}
 public static class PrimitiveTextGeometry {
  // Relative 14A6:0E2D..0EEE. Packed words and origins remain external inputs.
  public static PrimitiveTextScale Prepare(float horizontal,float vertical,float staff,float x,float y,float left,float baseline){
   float sx=(float)((double)horizontal*staff),sy=(float)((double)vertical*staff);
   return new PrimitiveTextScale{X=sx,Y=sy,OriginX=(float)((double)x-(double)sx*left),OriginY=(float)((double)y-(double)sy*baseline)};
  }
  public static PrimitiveTextPoint Point(short packed,PrimitiveTextScale scale){
   short operation=(short)(packed/10000),x=(short)((packed%10000)/100),y=(short)(packed%100);
   return new PrimitiveTextPoint{X=(float)((double)x*scale.X+scale.OriginX),Y=(float)((double)y*scale.Y+scale.OriginY),Operation=operation};
  }
 }
}
````

## PrintedGlyphDefinition

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PrintedGlyphDefinition.cs)

````csharp
using System;using System.Globalization;
namespace ScoreModern.RecoveredFragments {
 public struct PrintedGlyphVertex {public int X,Y;public short Operation;}
 public struct PrintedHeadRequest {public int Glyph;public short X,Y;public string Scale,Width;}
 public static class PrintedGlyphDefinition {
  // Observed normal-scale PostScript definition projection. Coordinate and
  // operation arrays remain in the user's external drawing files.
  public static PrintedGlyphVertex Vertex(short x,short y,short operation){return new PrintedGlyphVertex{X=(int)Math.Truncate(x*6.25),Y=(int)Math.Truncate(y*6.25),Operation=operation};}
  public static string Fixed3(float value){string s=value.ToString("F3",CultureInfo.InvariantCulture);return s.StartsWith("0.")?s.Substring(1):s;}
 }
}
````

## PrintRotation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/PrintRotation.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class PrintRotation {
  // Verified FLD/FMUL/FSTP uses DS:281C, with DS:7290=1.3333f and
  // DS:728C=0.75f in the measured print overlay. Size-word effect; Offset consumes the separate integer candidate.
  // Original print-menu size word: rounded binary32 literal1.3333 on,
  // exactly0.75 off. The unchanged flag path does not multiply again.
  // Data effect only; no x87 flags, stack or calling-convention adapter.
  public static float Apply(float size,bool before,bool after){if(before==after)return size;return (float)((double)size*(after?(double)1.3333f:.75));}
  public static int Offset(int candidate,bool before,bool after){if(before==after)return candidate;return checked((int)Math.Truncate(candidate*(after?(double)1.333f:.75)));}
 }
}
````

## RasterDispatch

[Workspace source](../../ScoreReverseEngineering/NativeFragments/RasterDispatch.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public struct RasterDispatchWords {public ushort BX,CX,DX,Exit;public uint EFlags;}
 public static class RasterDispatch {
  static uint CompareFlags(ushort a,ushort b,uint flags){ushort r=(ushort)(a-b);uint f=0;if(a<b)f|=1;if(((a^b^r)&16)!=0)f|=16;if(r==0)f|=64;if((r&0x8000)!=0)f|=128;if(((a^b)&(a^r)&0x8000)!=0)f|=2048;int p=r&255;p^=p>>4;p^=p>>2;p^=p>>1;if((p&1)==0)f|=4;return (flags&~0x8d5u)|f;}
  // Relative 1807:0981..09B7. No raster writes or stack effects.
  public static RasterDispatchWords Execute(IHorizontalRasterMemory m,ushort ds,ushort bx,ushort cx,ushort dx,uint flags){
   flags=CompareFlags(dx,bx,flags);
   if(dx==bx)return new RasterDispatchWords{BX=bx,CX=cx,DX=dx,Exit=0x0a3d,EFlags=flags};
   m.WriteWord(ds,0x4b70,dx);dx=(ushort)(dx>>1);m.WriteWord(ds,0x4b74,dx);
   bx=m.ReadWord(ds,0x4b6a);cx=m.ReadWord(ds,0x4b68);ushort right=m.ReadWord(ds,0x4b6c);flags=CompareFlags(cx,right,flags);
   if((short)cx>=(short)right){ushort old=cx;cx=m.ReadWord(ds,0x4b6c);m.WriteWord(ds,0x4b6c,old);m.WriteWord(ds,0x4b68,cx);
    old=bx;bx=m.ReadWord(ds,0x4b6e);m.WriteWord(ds,0x4b6e,old);m.WriteWord(ds,0x4b6a,bx);m.WriteWord(ds,0x4b78,0xfffe);}
   return new RasterDispatchWords{BX=bx,CX=cx,DX=dx,Exit=0x09b8,EFlags=flags};
  }
 }
}
````

## RasterStatusFlags

[Workspace source](../../ScoreReverseEngineering/NativeFragments/RasterStatusFlags.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 // Returns the original CPU-visible OR result while performing its video effect.
 // This is not necessarily the resulting byte in display plane 2.
 public interface IByteOrResultMemory:IHorizontalRasterMemory {
  byte OrByteAndGetResult(ushort segment,ushort offset,byte mask);
 }
 public static class RasterStatusFlags {
  const uint Status=0x8d5;
  static uint Result(uint flags,uint bits,ushort result,int width){
   uint sign=width==8?0x80u:0x8000u,mask=width==8?255u:65535u;
   result=(ushort)(result&mask);if(result==0)bits|=64;if((result&sign)!=0)bits|=128;
   int p=result&255;p^=p>>4;p^=p>>2;p^=p>>1;if((p&1)==0)bits|=4;
   return (flags&~Status)|bits;
  }
  static uint Sub(uint flags,ushort a,ushort b,int width){
   uint mask=width==8?255u:65535u,sign=width==8?0x80u:0x8000u;a=(ushort)(a&mask);b=(ushort)(b&mask);ushort r=(ushort)((a-b)&mask);uint bits=0;
   if(a<b)bits|=1;if(((a^b^r)&16)!=0)bits|=16;if(((a^b)&(a^r)&sign)!=0)bits|=2048;return Result(flags,bits,r,width);
  }
  public static uint Sub8(uint flags,byte a,byte b){return Sub(flags,a,b,8);}
  public static uint Sub16(uint flags,ushort a,ushort b){return Sub(flags,a,b,16);}
  public static uint Add16(uint flags,ushort a,ushort b){ushort r=(ushort)(a+b);uint bits=0;if((uint)a+b>65535)bits|=1;if(((a^b^r)&16)!=0)bits|=16;if(((~(a^b))&(a^r)&0x8000)!=0)bits|=2048;return Result(flags,bits,r,16);}
  public static uint Inc16(uint flags,ushort a){return (Add16(flags,a,1)&~1u)|(flags&1u);}
  public static uint Dec16(uint flags,ushort a){return (Sub16(flags,a,1)&~1u)|(flags&1u);}
  // DOSBox-X normal-core logical-byte policy clears AF (architecturally undefined).
  public static uint Logical8(uint flags,byte result){return Result(flags,0,result,8);}
 }
 internal sealed class RasterFlagState {public uint Value;}
}
````

## RetainedParameterStorage

[Workspace source](../../ScoreReverseEngineering/NativeFragments/RetainedParameterStorage.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // The original RN workspace has14200 real words. A fresh measured VM
 // starts with zero words; loading overlays the file, packing overwrites
 // its active prefix, and saving retains the word immediately after it.
 public sealed class RetainedParameterStorage {
  public const int Capacity=14200*4;
  readonly byte[] data=new byte[Capacity+24];
  readonly bool[] known=new bool[Capacity+24];
  int length;
  public RetainedParameterStorage(){for(int n=0;n<Capacity;n++)known[n]=true;}
  public RetainedParameterStorage Clone(){var result=new RetainedParameterStorage();Array.Copy(data,result.data,data.Length);Array.Copy(known,result.known,known.Length);result.length=length;return result;}
  public void Forget(){Array.Clear(known,0,known.Length);}
  public void Opaque(byte[] active){Forget();length=Math.Min(active.Length,Capacity);Array.Copy(active,data,length);}
  public bool Matches(byte[] active){if(active.Length!=length)return false;for(int n=0;n<length;n++)if(active[n]!=data[n])return false;return true;}
  public void Load(byte[] payload,int trailerBytes,byte[] producer,byte[] active){
   if(payload.Length>Capacity||active.Length>Capacity){Opaque(active);return;}
   int original=payload.Length-trailerBytes;if(original<0){Opaque(active);return;}
   Array.Copy(payload,data,payload.Length);for(int n=0;n<payload.Length;n++)known[n]=true;
   if(trailerBytes==24&&producer!=null){Array.Copy(producer,0,data,original+4,4);}else for(int n=original+4;n<original+trailerBytes;n++)known[n]=false;
   Array.Copy(active,data,active.Length);for(int n=0;n<active.Length;n++)known[n]=true;length=active.Length;
   // Short-record expansion has its own loader continuation; do not infer
   // how it relocates the trailing storage from the logical records alone.
   if(original!=length)for(int n=length;n<Math.Min(known.Length,length+24);n++)known[n]=false;
  }
  public void Pack(byte[] active){
   if(active.Length>Capacity){Opaque(active);return;}
   Array.Copy(active,data,active.Length);for(int n=0;n<active.Length;n++)known[n]=true;length=active.Length;
   // Recorded routines reuse these following words as scratch. The first
   // trailing word itself is retained; the other five are written on SA.
   for(int n=length+4;n<Math.Min(known.Length,length+24);n++)known[n]=false;
  }
  //3054:0591..05C0 writes three literal spaces after the text bytes,
  //including bytes beyond the last packed record. Unknown untouched bytes stay unknown.
  public void TextInputPadding(int offset){if(offset<0||offset+3>Capacity)return;for(int n=offset;n<offset+3;n++){data[n]=32;known[n]=true;}}
  public void WriteKnown(int offset,byte[] bytes){if(offset<0||offset>data.Length-bytes.Length)throw new NotSupportedException("RN write exceeds its retained storage.");Array.Copy(bytes,0,data,offset,bytes.Length);for(int n=offset;n<offset+bytes.Length;n++)known[n]=true;}
  public void CopyBytes(int source,int destination,int count){if(source<0||destination<0||count<0||source>data.Length-count||destination>data.Length-count)throw new NotSupportedException("RN copy exceeds its retained storage.");Array.Copy(data,source,data,destination,count);Array.Copy(known,source,known,destination,count);}
  public void CommitReplay(byte[] active){if(active.Length>Capacity)throw new NotSupportedException("RN replay exceeds capacity.");WriteKnown(0,active);length=active.Length;}
  public byte[] ReadKnown(int offset,int count){if(offset<0||count<0||offset>data.Length-count)return null;for(int n=offset;n<offset+count;n++)if(!known[n])return null;var result=new byte[count];Array.Copy(data,offset,result,0,count);return result;}
  public byte[] UndefinedWord(){if(length+4>data.Length)return null;for(int n=length;n<length+4;n++)if(!known[n])return null;var word=new byte[4];Array.Copy(data,length,word,0,4);return word;}
  public void Saved(byte[] trailer){if(length+trailer.Length>data.Length)return;Array.Copy(trailer,0,data,length,trailer.Length);for(int n=length;n<length+trailer.Length;n++)known[n]=true;}
 }
}
````

## ScanP8Conversion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScanP8Conversion.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class ScanP8Conversion {
  // Relative 27A1:057D..05B8, finite numerical projection. The original
  // multiplies two binary32 operands without an intervening binary32 store,
  // calls its truncating integer helper, and stores the returned low DWORD.
  // The caller supplies the measured factor; x87 flags/stack effects and
  // exceptional conversion delivery are outside this API.
  public static int Convert(uint sourceBits,float factor) {
   float source=BitConverter.ToSingle(BitConverter.GetBytes(sourceBits),0);
   if(Single.IsNaN(source)||Single.IsInfinity(source)||Single.IsNaN(factor)||Single.IsInfinity(factor))throw new NotSupportedException("Requires finite source and factor.");
   // A binary32 product has at most 48 significant bits, representable
   // exactly in binary64 across the binary32 exponent range.
   double product=(double)source*factor;
   if(product < -9223372036854775808.0 || product >= 9223372036854775808.0)throw new NotSupportedException("Outside the verified signed-64-bit conversion projection.");
   return unchecked((int)(long)Math.Truncate(product));
  }
 }
}
````

## ScanParameterPreparation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScanParameterPreparation.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class ScanPreparedParameters {
  public uint[] Words;public short EffectiveCopyLength;
  public bool StaffOneP8Branch;public uint StaffOneP8Bits;
 }
 public static class ScanParameterPreparation {
  // The caller's twentieth working word lies outside the 19-word copy.
  public static ScanPreparedParameters PrepareFrame(short declaredLength,uint[] followingWords,uint carriedTwentiethWord) {
   var result=Prepare(declaredLength,followingWords);var frame=new uint[20];Array.Copy(result.Words,frame,19);frame[19]=carriedTwentiethWord;result.Words=frame;return result;
  }
  // Relative 27A1:049D..05C3, parameter-word projection only. The caller
  // supplies the original converted record length and 19 following raw words,
  // including neighbouring record words when the declared payload is shorter.
  // P8 conversion and external flag/stack writes are not reconstructed here.
  public static ScanPreparedParameters Prepare(short declaredLength,uint[] followingWords) {
   if(declaredLength<0||followingWords==null||followingWords.Length<19)throw new NotSupportedException("Requires nonnegative length and 19 accessible source words.");
   short length=declaredLength;
   short high=unchecked((short)(followingWords[11]>>16));ushort low=unchecked((ushort)followingWords[11]);
   if(followingWords[0]==0x41800000u&&(high>0x3f80||(high==0x3f80&&low>0)))length=12;
   var p=new uint[19];for(int j=0;j<19;j++)if(j<length)p[j]=followingWords[j];
   var result=new ScanPreparedParameters{Words=p,EffectiveCopyLength=length};
   if(p[0]==0x41000000u){
    p[11]=0x3f800000u;
    if(p[1]==0x3f800000u&&p[7]!=0){result.StaffOneP8Branch=true;result.StaffOneP8Bits=p[7];p[7]=0;}
   }
   return result;
  }
 }
}
````

## ScoreVersionControls

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScoreVersionControls.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original commercial SCOR4 controls. Callers explicitly select the version;
 // the default remains SCORE 3.11. No process-wide version state is retained.
 public static class ScoreVersionControls {
  public static LayoutSymbolSpacingResult NumberSpacing(float size,float offset,float staffScale){
   if(Single.IsNaN(size)||Single.IsInfinity(size)||Single.IsNaN(offset)||Single.IsInfinity(offset)||!(staffScale>0))throw new NotSupportedException("Nonfinite number justification controls.");
   // Original SCOR4 J fixtures SHORTO/SHORTM/NRIGHT/NSIZE/NTWO:
   // fixed left/right allowances .2 and 4, independent of size/digit count.
   // Keep division extended until each final binary32 assignment.

   double shift=(double)offset/staffScale;
   return new LayoutSymbolSpacingResult{Left=(float)((double).2f-shift),Right=(float)((double)4f+shift)};
  }
  public static bool SlurEndpointMode(bool scor4,float p16){return scor4&&p16==1;}
  public static short SlurSubtype(bool scor4,float p8,float p16){return SlurEndpointMode(scor4,p16)?(short)0:unchecked((short)(int)Math.Truncate(p8));}
  public static bool SlurEndpointOverride(bool scor4,float[] p){return p!=null&&p.Length>18&&SlurEndpointMode(scor4,p[16]);}
  public static float[] SlurParameters(bool scor4,float[] parameters){if(!SlurEndpointOverride(scor4,parameters))return parameters;var p=(float[])parameters.Clone();p[8]=0;return p;}
  public static float DrawingOffset(bool scor4,float value){return scor4?Horizontal(value):0;}
  public static float HarpOffset(bool scor4,float value){return scor4?Horizontal(value):0;}
  public static float Horizontal(float value){if(Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("Nonfinite SCORE horizontal offset.");return (float)((double)value*150);}
  public static bool MeterThicknessSupported(bool scor4,float value){return scor4?!Single.IsNaN(value)&&!Single.IsInfinity(value)&&value>=-32768&&value<=32767:value==0||value==-2||value==2||value==1.25f||value==-1.25f||value==.5f||value==-1;}
  public static short MeterThicknessSelector(bool scor4,float value){if(!MeterThicknessSupported(scor4,value))throw new NotSupportedException("Meter outline thickness leaves the verified word domain.");return scor4?unchecked((short)(int)Math.Truncate(value)):(short)0;}
 }
}
````

## ScorlasFilePlacement

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScorlasFilePlacement.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 // Original SCORLAS measured runtime 0C7A:0680..081A. Inputs are the
 // original signed-word scan extrema and an already converted int32 guide.
 // This does not model the separate floating-point P8 conversion.
 public struct ScorlasFilePlacement {
  public int Delta,Cumulative,NextAdvance,OverallHeight,RawHeight,BottomShift,NormalizedHeight,DscBottom,DscTop;
 }
 public sealed class ScorlasFilePlacementState {
  bool started;int previousGuide,previousAdvance,firstBottomShift,cumulative;
  public ScorlasFilePlacement Add(short minimum,short maximum,int guide){
   int height=unchecked((int)maximum-minimum),shift=unchecked((short)(minimum+24000)),normalized=unchecked(height+shift),delta=0,total=height;
   if(!started){firstBottomShift=shift;cumulative=0;started=true;}
   else{int correction=previousGuide!=0?0:shift;delta=unchecked(previousAdvance-correction);cumulative=unchecked(cumulative+delta);total=unchecked(cumulative+normalized-firstBottomShift);}
   previousGuide=guide;previousAdvance=guide!=0?guide:unchecked(normalized+1000);
   // 06A2..06A9 replaces 3494 with this file's signed minimum. 07A8..07B3
   // stores the cumulative height; 07FF..0811 adds it to that same minimum.
   // These finalizer bounds are deliberately separate from translated ink.
   return new ScorlasFilePlacement{Delta=delta,Cumulative=cumulative,NextAdvance=previousAdvance,OverallHeight=total,RawHeight=height,BottomShift=shift,NormalizedHeight=normalized,DscBottom=minimum,DscTop=unchecked(minimum+total)};
  }
 }
}
````

## ScorlasGuideConversion

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScorlasGuideConversion.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Original SCORLAS 0C7A:057E..05B6 loads the single-precision factor,
 // multiplies the single-precision P8 in the x87 register, then calls
 // 1F6B:083A. That helper selects truncation, FISTPs a signed qword,
 // restores the control word and returns its low two words.
 public static class ScorlasGuideConversion {
  public static int Convert(float value,bool metric){
   double product=(double)value*(metric?(double)1574.8031005859375f:4000.0);
   // Keep unmeasured overflow/exception states outside this valid domain.
   if(Double.IsNaN(product)||Double.IsInfinity(product)||product<Int32.MinValue||product>Int32.MaxValue)throw new NotSupportedException("SCORLAS guide conversion outside the verified finite signed32 domain.");
   return (int)Math.Truncate(product);
  }
 }
}
````

## ScorlasStatisticsArithmetic

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScorlasStatisticsArithmetic.cs)

````csharp
using System;
using System.Numerics;
namespace ScoreModern.RecoveredFragments {
 // Original SCORLAS 0824:06F4..0733: signed32 ADD/ADC 111,
 // retained FILD*REAL4 size/4000, FSTP REAL4; metric then multiplies
 // that stored REAL4 by the stored REAL4 2.54 and stores again.
 public static class ScorlasStatisticsArithmetic {
  public static float Height(int cumulativeHeight,float printSize,bool metric){
   if(Single.IsNaN(printSize)||Single.IsInfinity(printSize)||printSize<=0)throw new NotSupportedException("SCORLAS statistics requires a finite positive print size.");
   int padded=unchecked(cumulativeHeight+111);
   uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(printSize),0),fraction=bits&0x7fffff;int exponent=(int)((bits>>23)&255);
   if(exponent!=0)fraction|=0x800000;
   BigInteger numerator=(BigInteger)padded*fraction,denominator=4000;int power=exponent==0?-149:exponent-150;
   if(power>=0)numerator<<=power;else denominator<<=-power;
   float inches=ToSingle(numerator,denominator);
   return metric?(float)((double)inches*2.53999996185302734375f):inches;
  }
  static int Bits(BigInteger value){byte[] bytes=value.ToByteArray();int end=bytes.Length-1;while(end>0&&bytes[end]==0)end--;int high=bytes[end],bits=end*8;while(high!=0){bits++;high>>=1;}return bits;}
  static int ComparePower(BigInteger n,BigInteger d,int power){return power>=0?n.CompareTo(d<<power):(n<<-power).CompareTo(d);}
  static BigInteger Rounded(BigInteger n,BigInteger d,int power){if(power>=0)n<<=power;else d<<=-power;BigInteger remainder;BigInteger q=BigInteger.DivRem(n,d,out remainder);int cmp=(remainder<<1).CompareTo(d);return cmp>0||cmp==0&&!q.IsEven?q+1:q;}
  static float ToSingle(BigInteger n,BigInteger d){
   if(n.IsZero)return 0;uint sign=n.Sign<0?0x80000000u:0;n=BigInteger.Abs(n);int exponent=Bits(n)-Bits(d);while(ComparePower(n,d,exponent)<0)exponent--;while(ComparePower(n,d,exponent+1)>=0)exponent++;
   uint word;
   if(exponent< -126){BigInteger q=Rounded(n,d,149);word=(uint)q;}
   else if(exponent>127)word=0x7f800000;
   else{BigInteger q=Rounded(n,d,23-exponent);if(q==(BigInteger.One<<24)){q>>=1;exponent++;}word=exponent>127?0x7f800000u:((uint)(exponent+127)<<23)|((uint)q&0x7fffff);}
   return BitConverter.ToSingle(BitConverter.GetBytes(sign|word),0);
  }
 }
}
````

## ScreenHookDecisions

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScreenHookDecisions.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class ScreenHookDecisions {
  // Original 16FA:023F..0261. Data/branch projection only; does not perform
  // STI, CLD, frame saves, BIOS calls or the final far transfer.
  public static bool ShouldProbeCursor(IStringBlockMemory m,ushort ds,ushort ax){
   if(m==null)throw new ArgumentNullException("m");
   byte ah=(byte)(ax>>8),al=(byte)ax;
   if(ah==0){m.WriteByte(ds,0x4ba2,al);return false;}
   if(unchecked((sbyte)m.ReadByte(ds,0x4ba2))<=2)return false;
   if(ah!=0x0e)return false;
   return al!=8&&al!=7&&al!=0x0d;
  }
  // Original 16FA:0270..027F. Preserve the LF bypass and conditional reads.
  public static bool ShouldScroll(IStringBlockMemory m,ushort ds,byte al,ushort dx){
   if(m==null)throw new ArgumentNullException("m");
   if(al!=0x0a){
    byte bound=m.ReadByte(ds,0x4b9a);
    if(unchecked((sbyte)(byte)dx)<unchecked((sbyte)bound))return false;
   }
   byte row=m.ReadByte(ds,0x4b9b);
   return unchecked((sbyte)(byte)(dx>>8))>=unchecked((sbyte)row);
  }
  // Original 16FA:029A..02A3. Returns the block's updated DX word.
  public static ushort DecrementSavedRow(IStringBlockMemory m,ushort ds){
   if(m==null)throw new ArgumentNullException("m");
   ushort value=m.ReadWord(ds,0x4b9e);
   byte row=unchecked((byte)((value>>8)-1));
   ushort result=(ushort)((value&255)|(row<<8));
   m.WriteWord(ds,0x4b9e,result);return result;
  }
 }
}
````

## ScreenHookForwarding

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScreenHookForwarding.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IScreenForwardMemory {ushort ReadWord(ushort segment,ushort offset);}
 public struct ScreenForwardResult {
  public readonly ushort DX,CX,BX,DS,SP,IP,CS;
  public ScreenForwardResult(ushort dx,ushort cx,ushort bx,ushort ds,ushort sp,ushort ip,ushort cs){DX=dx;CX=cx;BX=bx;DS=ds;SP=sp;IP=ip;CS=cs;}
 }
 public static class ScreenHookForwarding {
  // Original 16FA:02A5..02AD: four POPs then a CS-relative far-pointer read.
  // Read/result projection only. Does not execute BIOS code or IRET.
  public static ScreenForwardResult RestoreAndResolve(IScreenForwardMemory m,ushort ss,ushort sp,ushort codeSegment){
   if(m==null)throw new ArgumentNullException("m");
   ushort dx=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort cx=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort bx=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort ds=m.ReadWord(ss,sp);sp=unchecked((ushort)(sp+2));
   ushort ip=m.ReadWord(codeSegment,0x01cc);
   ushort cs=m.ReadWord(codeSegment,0x01ce);
   return new ScreenForwardResult(dx,cx,bx,ds,sp,ip,cs);
  }
 }
}
````

## ScreenHookInterrupt

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScreenHookInterrupt.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IScreenBiosTransferEffects {SignedDivide16State TransferToBios(IScreenHookMemory memory,SignedDivide16State state);}
 public static class ScreenHookInterrupt {
  // Measured 16-bit real-mode CD 10 entry. The destination BIOS/IRET is external.
  public static SignedDivide16State Enter(IScreenHookMemory m,SignedDivide16State s){
   ushort next=(ushort)(s.IP+2);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,(ushort)s.EFlags);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,next);s.EFlags&=~0x300u;s.IP=m.ReadWord(0,0x40);s.CS=m.ReadWord(0,0x42);return s;
  }
 }
 public sealed class ScreenHookInterruptEffects:ICapacityInterruptEffects {
  readonly IScreenBiosTransferEffects bios;readonly ICapacityInterruptEffects other;readonly ushort hookCS,hookIP,dataSegment;
  public ScreenHookInterruptEffects(IScreenBiosTransferEffects biosEffects,ICapacityInterruptEffects otherInterruptEffects,ushort hookedSegment,ushort hookedOffset,ushort relocatedDataSegment){if(biosEffects==null)throw new ArgumentNullException("biosEffects");bios=biosEffects;other=otherInterruptEffects;hookCS=hookedSegment;hookIP=hookedOffset;dataSegment=relocatedDataSegment;}
  public SignedDivide16State Interrupt(byte number,IHorizontalRasterMemory memory,SignedDivide16State state){
   if(number!=0x10){if(other==null)throw new InvalidOperationException("Other interrupt effect required.");return other.Interrupt(number,memory,state);}
   var m=memory as IScreenHookMemory;if(m==null)throw new ArgumentException("Screen-hook byte memory required.");state=ScreenHookInterrupt.Enter(m,state);
   if(state.CS==hookCS&&state.IP==hookIP){var r=ScreenHookRoutine.Execute(m,this,state,dataSegment);state=r.State;if(r.Outcome!=ScreenHookOutcome.Forwarded)return state;}
   return bios.TransferToBios(m,state);
  }
 }
}
````

## ScreenHookRoutine

[Workspace source](../../ScoreReverseEngineering/NativeFragments/ScreenHookRoutine.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IScreenHookMemory:ICapacityStringMemory {void WriteByte(ushort segment,ushort offset,byte value);}
 public enum ScreenHookOutcome {Forwarded,ExternalControlTransfer}
 public struct ScreenHookResult {public SignedDivide16State State;public ScreenHookOutcome Outcome;}
 public static class ScreenHookRoutine {
  static void Push(IScreenHookMemory m,ref SignedDivide16State s,ushort v){s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,v);}
  static ushort Pop(IScreenHookMemory m,ref SignedDivide16State s){ushort v=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);return v;}
  static void AH(ref SignedDivide16State s,byte v){s.AX=(ushort)((s.AX&255)|(v<<8));}
  static bool Bios(IScreenHookMemory m,ICapacityInterruptEffects fx,ref SignedDivide16State s,ushort at){ushort self=s.CS;s.IP=at;s=fx.Interrupt(0x10,m,s);return s.CS==self&&s.IP==(ushort)(at+2);}
  static void DecRow(ref SignedDivide16State s){byte a=(byte)(s.DX>>8);s.EFlags=(RasterStatusFlags.Sub8(s.EFlags,a,1)&~1u)|(s.EFlags&1u);s.DX=(ushort)((s.DX&255)|((byte)(a-1)<<8));}
  static ScreenHookResult External(SignedDivide16State s){return new ScreenHookResult{State=s,Outcome=ScreenHookOutcome.ExternalControlTransfer};}
  // Relative 16FA:0235..02AD. Entry already has the CPU-created interrupt frame.
  // STI/CLD flag changes are modeled; interrupt scheduling/shadow is not.
  public static ScreenHookResult Execute(IScreenHookMemory m,ICapacityInterruptEffects fx,SignedDivide16State s,ushort dataSegment){
   if(s.IP!=0x0235)throw new ArgumentException("Screen hook entry must be 0235.");ushort self=s.CS;s.EFlags=(s.EFlags|0x200u)&~0x400u;Push(m,ref s,s.DS);Push(m,ref s,s.AX);s.AX=dataSegment;s.DS=s.AX;s.AX=Pop(m,ref s);
   byte ah=(byte)(s.AX>>8),al=(byte)s.AX;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,ah,0);
   if(ah==0){m.WriteByte(s.DS,0x4ba2,al);goto RestoreDs;}
   byte mode=m.ReadByte(s.DS,0x4ba2);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,mode,2);if((sbyte)mode<=2)goto RestoreDs;
   s.EFlags=RasterStatusFlags.Sub8(s.EFlags,ah,0x0e);if(ah!=0x0e)goto RestoreDs;
   s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,8);if(al==8)goto RestoreDs;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,7);if(al==7)goto RestoreDs;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,13);if(al==13)goto RestoreDs;
   s.BX=7;Push(m,ref s,s.BX);Push(m,ref s,s.CX);Push(m,ref s,s.DX);Push(m,ref s,s.AX);AH(ref s,3);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x026d))return External(s);s.AX=Pop(m,ref s);
   al=(byte)s.AX;s.EFlags=RasterStatusFlags.Sub8(s.EFlags,al,10);
   if(al!=10){byte col=m.ReadByte(s.DS,0x4b9a);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)s.DX,col);if((sbyte)(byte)s.DX<(sbyte)col)goto RestoreSaved;}
   byte row=m.ReadByte(s.DS,0x4b9b);s.EFlags=RasterStatusFlags.Sub8(s.EFlags,(byte)(s.DX>>8),row);if((sbyte)(byte)(s.DX>>8)<(sbyte)row)goto RestoreSaved;
   Push(m,ref s,s.AX);Push(m,ref s,s.DX);s.CX=m.ReadWord(s.DS,0x4b98);s.DX=m.ReadWord(s.DS,0x4b9a);s.AX=0x0601;s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x028f))return External(s);
   s.DX=Pop(m,ref s);DecRow(ref s);AH(ref s,2);s.BX=(ushort)(s.BX&255);if(!Bios(m,fx,ref s,0x0298))return External(s);s.DX=m.ReadWord(s.DS,0x4b9e);DecRow(ref s);m.WriteWord(s.DS,0x4b9e,s.DX);s.AX=Pop(m,ref s);
   RestoreSaved:s.DX=Pop(m,ref s);s.CX=Pop(m,ref s);s.BX=Pop(m,ref s);
   RestoreDs:s.DS=Pop(m,ref s);ushort ip=m.ReadWord(self,0x01cc),cs=m.ReadWord(self,0x01ce);s.IP=ip;s.CS=cs;return new ScreenHookResult{State=s,Outcome=ScreenHookOutcome.Forwarded};
  }
 }
}
````

## SelectorCopy

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SelectorCopy.cs)

````csharp
using System;

namespace ScoreModern.RecoveredFragments
{
    // Literal memory-copy fragment only, NOT a complete routine or application feature.
    // Source: overlay 8, 2796:02D6 + 0x22 through +0x43.
    // Contract: caller supplies the original 64 KiB DS contents, signed SI state,
    // and direction flag. Stack/register/call effects outside the copy are excluded.
    public static class SelectorCopy
    {
        public static bool Apply(byte[] dataSegment, short state, bool directionFlag)
        {
            if (dataSegment == null || dataSegment.Length != 65536)
                throw new ArgumentException("A complete 64 KiB data segment is required.");
            if (state <= 1 || state >= 6) return false;

            ushort source = (ushort)(0x3c46 + 6 * state);
            ushort destination = 0x3c6e;
            int increment = directionFlag ? -2 : 2;
            for (int n = 0; n < 3; n++)
            {
                // MOVSW reads the complete word before writing it.
                byte low = dataSegment[source];
                byte high = dataSegment[unchecked((ushort)(source + 1))];
                dataSegment[destination] = low;
                dataSegment[unchecked((ushort)(destination + 1))] = high;
                source = unchecked((ushort)(source + increment));
                destination = unchecked((ushort)(destination + increment));
            }
            return true;
        }
    }
}
````

## SelectorRoutine

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SelectorRoutine.cs)

````csharp
using System;

namespace ScoreModern.RecoveredFragments
{
    // Host interfaces expose the unresolved machine context and callees.
    // They do not supply inferred formatting behavior or segment defaults.
    public interface ISelectorContext
    {
        ushort DataSegment { get; }
        bool DirectionFlag { get; }
        ushort ReadWord(ushort segment, ushort offset);
        void WriteWord(ushort segment, ushort offset, ushort value);
        ushort CallFar(ushort relativeSegment, ushort offset, ushort[] stackWords);
    }

    public static class SelectorRoutine
    {
        // Original overlay 8, relative 2796:02D6. This ports observable data
        // accesses/control flow/call arguments; it is NOT a 16-bit ABI adapter.
        // Physical stack effects, registers, interrupts and callee internals
        // remain outside this interface and require separate integration proof.
        public static void Execute(ISelectorContext context, ushort parameterSegment, ushort parameterOffset)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.ReadWord(parameterSegment, parameterOffset) == 1)
            {
                context.CallFar(0x1f08, 0x0003, new ushort[] { 0x3c7a });
                return;
            }

            // The original reloads the parameter pointer and reads the word
            // again. Do not collapse the two reads into one cached state.
            short state = unchecked((short)context.ReadWord(parameterSegment, parameterOffset));
            if (state <= 1 || state >= 6) return;

            ushort source = (ushort)(0x3c46 + 6 * state);
            ushort destination = 0x3c6e;
            ushort destinationSegment = context.DataSegment; // PUSH DS; POP ES
            for (int n = 0; n < 3; n++)
            {
                ushort value = context.ReadWord(context.DataSegment, source);
                context.WriteWord(destinationSegment, destination, value);
                int delta = context.DirectionFlag ? -2 : 2;
                source = unchecked((ushort)(source + delta));
                destination = unchecked((ushort)(destination + delta));
            }
            ushort returnedAx = context.CallFar(0x1f08, 0x2ac6,
                new ushort[] { 0x0010, 0x3c6a, context.DataSegment });
            context.CallFar(0x1f08, 0x0003, new ushort[] { 0x3c82, returnedAx });
        }
    }
}
````

## SignedDivide16

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SignedDivide16.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public enum SignedDivide16Fault {None,ZeroDivisor,QuotientOverflow}
 public struct SignedDivide16State {
  public ushort AX,BX,CX,DX,SI,DI,BP,CS,IP,DS,ES,SS,SP;
  public uint EFlags;
 }
 public struct SignedDivide16Result {
  public SignedDivide16State State;
  public SignedDivide16Fault Fault;
  // On successful IDIV the arithmetic flag values are undefined, not verified outputs.
  public uint DefinedEFlagsMask;
 }
 public static class SignedDivide16 {
  // IDIV CX and vector-zero delivery in the measured 16-bit real-mode VM.
  // A fault preserves the faulting IP. Success advances past F7 F9.
  public static SignedDivide16Result Execute(IHorizontalRasterMemory m,SignedDivide16State s){
   int n=unchecked((int)(((uint)s.DX<<16)|s.AX)),d=(short)s.CX;
   SignedDivide16Fault cause=SignedDivide16Fault.None;int q=0;
   if(d==0)cause=SignedDivide16Fault.ZeroDivisor;
   else if(n==int.MinValue&&d==-1)cause=SignedDivide16Fault.QuotientOverflow;
   else {q=n/d;if(q<short.MinValue||q>short.MaxValue)cause=SignedDivide16Fault.QuotientOverflow;}
   if(cause==SignedDivide16Fault.None){s.AX=(ushort)q;s.DX=(ushort)(n%d);s.IP=(ushort)(s.IP+2);
    return new SignedDivide16Result{State=s,Fault=cause,DefinedEFlagsMask=~0x8d5u};}
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,(ushort)s.EFlags);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.CS);
   s.SP=(ushort)(s.SP-2);m.WriteWord(s.SS,s.SP,s.IP);
   s.EFlags&=~0x300u;
   s.IP=m.ReadWord(0,0);s.CS=m.ReadWord(0,2);
   return new SignedDivide16Result{State=s,Fault=cause,DefinedEFlagsMask=0xffffffffu};
  }
 }
}
````

## SingleFixedDecimal

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SingleFixedDecimal.cs)

````csharp
using System;using System.Globalization;using System.Numerics;
namespace ScoreModern.RecoveredFragments {
 // Numeric value only: original single-real fixed output caps significant
 // digits at DS:5C36=7 (runtime 272C:0EF6..0F1B), then rounds discarded
 // digits >= '5' with carry (20DD:0CA4). Field padding is separate.
 public static class SingleFixedDecimal {
  public static string Format(float value,int decimalPlaces){
   if(float.IsNaN(value)||float.IsInfinity(value))throw new NotSupportedException("Non-finite original decimal formatting is not verified.");
   if(decimalPlaces<0||decimalPlaces>9)throw new ArgumentOutOfRangeException("decimalPlaces");
   uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(value),0);bool negative=(bits&0x80000000)!=0;uint mantissa=bits&0x7fffff;int exponent=(int)((bits>>23)&255);if(exponent!=0)mantissa|=0x800000;int shift=exponent==0?-149:exponent-150;BigInteger numerator=mantissa,denominator=BigInteger.One;if(shift>=0)numerator<<=shift;else denominator<<=-shift;
   BigInteger rounded=BigInteger.Zero;
   if(mantissa!=0){int e=(int)Math.Floor(Math.Log10(Math.Abs((double)value)));Func<int,int> compare=power=>power>=0?numerator.CompareTo(denominator*BigInteger.Pow(10,power)):(numerator*BigInteger.Pow(10,-power)).CompareTo(denominator);while(compare(e)<0)e--;while(compare(e+1)>=0)e++;int places=Math.Min(decimalPlaces,6-e);BigInteger n=numerator,d=denominator;if(places>=0)n*=BigInteger.Pow(10,places);else d*=BigInteger.Pow(10,-places);BigInteger remainder;rounded=BigInteger.DivRem(n,d,out remainder);if(remainder*2>=d)rounded++;rounded*=BigInteger.Pow(10,decimalPlaces-places);}
   string digits=rounded.ToString(CultureInfo.InvariantCulture);if(decimalPlaces>0){digits=digits.PadLeft(decimalPlaces+1,'0');digits=digits.Insert(digits.Length-decimalPlaces,".");}return (negative?"-":"")+digits;
  }
 }
}
````

## SlopedRaster

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlopedRaster.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public static class SlopedRaster {
  static ushort Rotate(ushort dx){byte b=(byte)dx;return (ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}
  // Relative 1807:09B8..0A3C. Both far exits target 0B16.
  public static VerticalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx){
   return Core(m,ds,es,ax,bx,cx,dx,null);
  }
  public static VerticalRasterWords ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,uint eflags){return Core(m,ds,es,ax,bx,cx,dx,new RasterFlagState{Value=eflags});}
  static VerticalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,RasterFlagState flags){
   bx=(ushort)(bx<<1);ax=m.ReadWord(ds,0x4b68);cx=(ushort)((cx&0xff00)|8);
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV fault is outside this projection.");
   ax=(ushort)((ax/8)|((ax%8)<<8));cx=(ushort)((cx&0xff00)|(ax>>8));dx=(ushort)((dx&0xff00)|0x80);
   for(int n=0;n<(cx&255);n++)dx=Rotate(dx);
   ax=(ushort)(ax&255);ushort bp=ax;ax=0;cx=m.ReadWord(ds,0x4b70);ushort si=m.ReadWord(ds,0x4b72),di;
   if((short)cx>=(short)si){
    cx=(ushort)(cx+1);
    do{di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);
     ax=(ushort)(ax+si);
     if((short)ax>(short)m.ReadWord(ds,0x4b74)){ax=(ushort)(ax-m.ReadWord(ds,0x4b70));bx=(ushort)(bx+m.ReadWord(ds,0x4b78));}
     dx=Rotate(dx);if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,0);if((sbyte)(byte)dx<=0){if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,bp);bp=(ushort)(bp+1);}cx=(ushort)(cx-1);
    }while(cx!=0);
   }else{
    cx=m.ReadWord(ds,0x4b72);cx=(ushort)(cx>>1);m.WriteWord(ds,0x4b74,cx);
    cx=m.ReadWord(ds,0x4b72);cx=(ushort)(cx+1);si=m.ReadWord(ds,0x4b70);
    do{di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);
     ax=(ushort)(ax+si);
     if((short)ax>(short)m.ReadWord(ds,0x4b74)){ax=(ushort)(ax-m.ReadWord(ds,0x4b72));dx=Rotate(dx);if(flags!=null)flags.Value=RasterStatusFlags.Sub8(flags.Value,(byte)dx,0);if((sbyte)(byte)dx<=0){if(flags!=null)flags.Value=RasterStatusFlags.Inc16(flags.Value,bp);bp=(ushort)(bp+1);}}
     ushort stride=m.ReadWord(ds,0x4b78);if(flags!=null)flags.Value=RasterStatusFlags.Add16(flags.Value,bx,stride);bx=(ushort)(bx+stride);cx=(ushort)(cx-1);
    }while(cx!=0);
   }
   return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
````

## SlopedRasterFlagsModel

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlopedRasterFlagsModel.cs)

````csharp
using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class SlopedRasterFlagsModel {
 class Memory:IByteOrResultMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public byte OrByteAndGetResult(ushort s,ushort o,byte mask){OrByte(s,o,mask);return Bytes[(s<<4)+o];}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(VerticalRasterWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP+":"+w.EFlags;}
 static VerticalRasterWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,uint inputFlags){uint fullFlags=inputFlags;int pc=0,steps=0;ushort si=0,di=0,bp=0;bool less=false,equal=false;Action<ushort,ushort> cmp=(a,b)=>{fullFlags=IndependentFlagArithmetic.Apply(fullFlags,a,b,16,false);less=(short)a<(short)b;equal=a==b;};Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<2000000){switch(f()){
 case 0x8b:byte op=f();if(op==0xe8)bp=ax;else if(op==0xbf)di=m.ReadWord(ds,(ushort)(bx+word()));else if(op==0x0e)cx=m.ReadWord(ds,word());else if(op==0x36)si=m.ReadWord(ds,word());else throw new Exception("MOV");break;
 case 0xd1:byte shift=f();if(shift==0xe3)bx=(ushort)(bx<<1);else if(shift==0xe9)cx=(ushort)(cx>>1);else throw new Exception("Shift");break;
 case 0xa1:ax=m.ReadWord(ds,word());break;case 0xb1:cx=(ushort)((cx&0xff00)|f());break;
 case 0xf6:if(f()!=0xf1)throw new Exception("DIV opcode");int v=cx&255;if(v==0||ax/v>255)throw new Exception("Excluded fault");ax=(ushort)((ax/v)|((ax%v)<<8));break;
 case 0x8a:if(f()!=0xcc)throw new Exception("MOV CL");cx=(ushort)((cx&0xff00)|(ax>>8));break;
 case 0xb2:dx=(ushort)((dx&0xff00)|f());break;
 case 0xd2:if(f()!=0xca)throw new Exception("ROR");for(int n=0;n<(cx&255);n++){byte b=(byte)dx;dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}break;
 case 0xd0:if(f()!=0xca)throw new Exception("ROR1");byte r=(byte)dx;dx=(ushort)((dx&0xff00)|(r>>1)|((r&1)<<7));break;
 case 0x2a:if(f()!=0xe4)throw new Exception("SUB AH");ax=(ushort)(ax&255);break;
 case 0x2b:byte sub=f();if(sub==0xc0)ax=0;else if(sub==6)ax=(ushort)(ax-m.ReadWord(ds,word()));else throw new Exception("SUB");break;
 case 0x3b:byte comp=f();if(comp==0xce)cmp(cx,si);else if(comp==6)cmp(ax,m.ReadWord(ds,word()));else throw new Exception("CMP");break;
 case 0x7c:sbyte jl=(sbyte)f();if(less)pc+=jl;break;case 0x7e:sbyte jle=(sbyte)f();if(less||equal)pc+=jle;break;case 0x7f:sbyte jg=(sbyte)f();if(!less&&!equal)pc+=jg;break;
 case 0x41:cx=(ushort)(cx+1);break;case 0x45:fullFlags=(IndependentFlagArithmetic.Apply(fullFlags,bp,1,16,true)&~1u)|(fullFlags&1u);bp=(ushort)(bp+1);break;
 case 0x03:byte add=f();if(add==0xfd)di=(ushort)(di+bp);else if(add==0xc6)ax=(ushort)(ax+si);else if(add==0x1e){ushort step=m.ReadWord(ds,word());fullFlags=IndependentFlagArithmetic.Apply(fullFlags,bx,step,16,true);bx=(ushort)(bx+step);}else throw new Exception("ADD");break;
 case 0x26:if(f()!=8||f()!=0x15)throw new Exception("OR");m.OrByte(es,di,(byte)dx);fullFlags=IndependentFlagArithmetic.Result(fullFlags,0,((Memory)m).Bytes[(es<<4)+di],8);break;
 case 0x80:if(f()!=0xfa)throw new Exception("CMP byte");sbyte rhs=(sbyte)f(),lhs=(sbyte)(byte)dx;fullFlags=IndependentFlagArithmetic.Apply(fullFlags,(byte)lhs,(byte)rhs,8,false);less=lhs<rhs;equal=lhs==rhs;break;
 case 0xe2:sbyte d=(sbyte)f();cx=(ushort)(cx-1);if(cx!=0)pc+=d;break;
 case 0xea:if(word()!=0xb16||word()!=0x1807)throw new Exception("Far exit");pc=code.Length;break;
 case 0x89:if(f()!=0x0e)throw new Exception("MOV memory");m.WriteWord(ds,word(),cx);break;
 default:throw new Exception("Opcode at "+(pc-1));
 }}if(pc!=code.Length)throw new Exception("Exit");return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=fullFlags};}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 class Captured:IHorizontalRasterMemory {public byte[] Data,Plane;public ushort DS,ES;public int Writes;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){if(s!=ES)throw new Exception("Video segment");Plane[o]|=v;Writes++;}}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[133];Array.Copy(raw,0x18070+0x9b8,code,0,133);
 if(args.Length>1){string p=args[1];byte[] live=File.ReadAllBytes(Path.Combine(p,"raster-code.bin")),relocated=(byte[])code.Clone();foreach(int at in new[]{0x9fd,0xa38}){int i=at-0x9b8+3;relocated[i]=0x2b;relocated[i+1]=0x20;}if(live.Length!=133)throw new Exception("Live length");for(int i=0;i<133;i++)if(live[i]!=relocated[i])throw new Exception("Live code");Captured m=new Captured{DS=H(args[2]),ES=H(args[3]),Data=File.ReadAllBytes(Path.Combine(p,"raster-before-ds.bin")),Plane=File.ReadAllBytes(Path.Combine(p,"raster-before-plane2.bin"))};var r=SlopedRaster.Execute(m,m.DS,m.ES,H(args[4]),H(args[5]),H(args[6]),H(args[7]));var e=new VerticalRasterWords{AX=H(args[8]),BX=H(args[9]),CX=H(args[10]),DX=H(args[11]),SI=H(args[12]),DI=H(args[13]),BP=H(args[14])};if(Show(r)!=Show(e))throw new Exception("Runtime words "+Show(r)+" != "+Show(e));byte[] after=File.ReadAllBytes(Path.Combine(p,"raster-after-ds.bin")),plane=File.ReadAllBytes(Path.Combine(p,"raster-after-plane2.bin"));for(int i=0;i<65536;i++)if(m.Data[i]!=after[i]||m.Plane[i]!=plane[i])throw new Exception("Runtime memory "+i);Console.WriteLine("PASS original sloped block: 133 relocated code bytes, seven result words, all 65536 DS and 65536 plane2 bytes; "+m.Writes+" explicit plane2 OR effects.");return 0;}
 int count=0;for(int x=0;x<640;x++)for(int k=0;k<32;k++){ushort ds=0x2000,es=(ushort)((k&1)==0?0x4000:ds),row=(ushort)(k*1021),cx=(ushort)(x^0xa55a),dx=(ushort)(k*701);Memory seed=new Memory{Pattern=k};seed.Seed(ds,0x4b68,(ushort)x);seed.Seed(ds,0x4b70,(ushort)(k%16));seed.Seed(ds,0x4b72,(ushort)((k*7)%17));seed.Seed(ds,0x4b74,(ushort)((k%16)/2));seed.Seed(ds,0x4b78,(ushort)((k&2)==0?2:0xfffe));Memory a=seed.Copy(),b=seed.Copy();var n=SlopedRaster.ExecuteWithFlags(a,ds,es,row,row,cx,dx,0xA55A1234u ^ row);var q=Interpret(code,b,ds,es,row,row,cx,dx,0xA55A1234u ^ row);if(Show(n)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model state");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effect");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Model memory");count++;}
 Console.WriteLine("PASS full EFLAGS "+count+" instruction-model cases: 640 columns, both comparison branches, masks, word wrapping, row strides and data/video aliases. Ordinary RAM OR model; successful DIV paths only.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
````

## SlurGeometry

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurGeometry.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurGeometryResult {public SlurVectorBuffer Buffer;public TupletPlacement? Number;public int Root;}
 public static class SlurGeometry {
  static float F(double v){return (float)v;}static int W(float v){return unchecked((short)(int)Math.Truncate(v));}static bool P(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static void Distance(SlurOriginState o,float[] p,bool trimmed){
   float span=F((double)o.OriginalRight-o.OriginalLeft);o.HorizontalSpan=span;
   if(!trimmed&&W(p[12])>0){if(W(p[12])==1){o.RightX=F((double)span+o.RightX);o.StopFraction=.5f;}else{o.LeftX=F((double)o.LeftX-span);o.EffectiveStartFraction=.5f;o.LeftY=F((double)o.LeftY-o.DeltaY);}span=F(2.0*span);o.DeltaY=F(2.0*o.DeltaY);}
   o.Length=span;if(BitConverter.ToInt32(BitConverter.GetBytes(p[4]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(p[5]),0))o.Length=F(Math.Sqrt((double)o.DeltaY*o.DeltaY+(double)span*span));
  }
  static void Put(SlurVectorBuffer b,SlurSampleResult sample,SlurThicknessResult thick,bool printed){short[] x=printed?thick.X:sample.X,y=printed?thick.Y:sample.Y;b.X[b.Base&32767]=unchecked((short)(printed?0x8046:0x8045));for(int j=0;j<x.Length;j++){b.X[(b.Base+j+1)&32767]=x[j];b.Y[(b.Base+j)&32767]=y[j];}b.End=unchecked((short)(b.Base+x.Length+1));b.CurveEnd=unchecked((short)(b.Base+sample.Count));b.SampleCount=sample.Count;}
  public static SlurGeometryResult Build(float[] parameters,SlurOriginContext context,bool printed,float configuredControl,SlurVectorBuffer caller,bool rejectInvalidTrims){
   if(parameters==null||parameters.Length<21)throw new ArgumentException("Missing slur parameters.");
   float[] p=(float[])ScoreVersionControls.SlurParameters(context.Scor4,parameters).Clone();bool straight=W(p[9])==99;
   if(rejectInvalidTrims&&(p[14]<0||p[14]>1||p[15]<0||p[15]>1||(p[15]!=0&&p[14]>p[15])))throw new ArgumentException("Slur trim must be an ordered interval within 0..1.");
   bool widthPrefix=printed&&(straight?W(p[13])!=0:W(p[11])==0);var b=caller??new SlurVectorBuffer{End=printed?(straight?1:(widthPrefix?2:1)):20};int root=b.End;b.Base=root;b.X[0]=unchecked((short)root);if(widthPrefix){int prefix=straight?root:root-1;b.X[prefix]=unchecked((short)0x80a6);b.Y[prefix-1]=unchecked((short)(straight?W(p[13])*2:-1));if(straight){b.Base=root+1;b.X[0]=unchecked((short)b.Base);}}var o=SlurOrigins.Prepare(p,context);if(ScoreVersionControls.SlurEndpointOverride(context.Scor4,p)){p[16]=p[17]=p[18]=0;}int number=W(p[16]),split=number;float gap=0;
   if(!straight&&number>0&&W(p[11])!=0){number=-number;split=number;}
   if(straight){
    Distance(o,p,P(o.EffectiveStartFraction)||P(o.StopFraction));int at=b.Base;b.X[at]=unchecked((short)0x8045);
    b.X[at+1]=unchecked((short)(int)Math.Truncate(o.LeftX));b.X[at+3]=unchecked((short)(int)Math.Truncate((double)o.LeftX+o.Length));b.X[at+2]=unchecked((short)(int)Math.Truncate((double)o.Length*o.CurveCenter+o.LeftX));
    int lineY=(int)Math.Truncate(o.LeftY);b.Y[at]=unchecked((short)lineY);b.Y[at+2]=unchecked((short)lineY);b.Y[at+1]=unchecked((short)(int)Math.Truncate((double)lineY+o.Height));b.End=unchecked((short)(at+4));b.CurveEnd=unchecked((short)(at+3));b.SampleCount=3;
   }else for(int pass=0;pass<2;pass++){
    bool trimmed=P(o.EffectiveStartFraction)||P(o.StopFraction);Distance(o,p,trimmed);
    if(split>0){o.CurveCenter=.5f;double value=(double)context.Scale*(Math.Abs(number)<10?11.4f:16.2f)/o.Length;gap=F(value);o.StopFraction=F(.5-value);}
    var sample=SlurSampling.BuildState(p,context,o,printed,trimmed);SlurThicknessResult thick=null;
    if(printed){thick=SlurThickness.Build(p,context,sample,configuredControl);p[13]=thick.EffectiveControl;SlurThickness.Reflect(p,sample,thick.Y,true);}else SlurThickness.Reflect(p,sample,sample.Y,false);
    Put(b,sample,thick,printed);SlurPostprocess.Pattern(b,p,context.Scale,printed);SlurPostprocess.Trim(b,p,o.EffectiveStartFraction,o.StopFraction,printed);
    if(split>0){split=0;o.StopFraction=0;o.EffectiveStartFraction=F((double)gap+.5);b.Base=b.End;continue;}break;
   }
   b.Base=root;b.X[0]=unchecked((short)b.End);SlurPostprocess.Rotate(b,o);var result=new SlurGeometryResult{Buffer=b,Root=root};
   if(number==0)return result;short textNumber=unchecked((short)Math.Abs(number));bool negativeHeight=BitConverter.ToInt32(BitConverter.GetBytes(p[7]),0)<0;float offset;
   if(number>0)offset=P(o.DeltaY)?9:0;else offset=P(o.DeltaY)?(negativeHeight?-5:23):(negativeHeight?-14:14);
   float h=F((double)o.Height-(double)context.Scale*offset),left=o.LeftX,y,along;
   if(P(o.DeltaY)){
    float sin=F(Math.Sin(b.Angle)),cos=F(Math.Cos(b.Angle));double half=(double)o.HorizontalSpan*.5;float savedHalf=F(half);
    left=F(half*cos-(double)h*sin+left);y=F((double)h*cos+(double)savedHalf*sin+o.LeftY);along=0;
   }else{
    y=F((double)h+o.LeftY);along=F((double)o.Length*o.CurveCenter);int control=W(p[11]);if(control< -3)control/=2;control=Math.Abs(control);
    if(control!=0){if(control==3)return result;float vertical=F(-(double)context.Scale*14);int leg=W(p[7]);if(control==2)leg=-leg;if(leg<0)vertical=-vertical;y=F((double)y+vertical);}
    y=F((double)y-(double)context.Scale*9);
   }
   y=F((double)p[18]*context.Scale*7+y);left=F((double)p[17]*context.Scale*6+left);
   result.Number=new TupletPlacement{Number=textNumber,FontScale=.875f,X=TupletGeometry.Center(textNumber,context.Scale,along,left),Y=y};return result;
  }
 }
}
````

## SlurLineWidth

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurLineWidth.cs)

````csharp
using System;using System.Globalization;
namespace ScoreModern.RecoveredFragments {
 public static class SlurLineWidth {
  public static float DefaultUnit{get{return BitConverter.ToSingle(BitConverter.GetBytes(0x41557e4b),0);}}
  // Overlay three, relative 2587:0778..07A3; explicit printer-context inputs.
  public static float RawField(short selector,short basis,float unit){return (float)(((double)basis+selector)*unit);}
  public static float Field(short selector,short basis,float unit){float value=RawField(selector,basis,unit);if(BitConverter.ToInt32(BitConverter.GetBytes(value),0)<0)value=0;return value;}
  // REAL4 fixed output in the shipped .NET Framework target; original width
  // requests, including its seven-significant-digit boundary, have replay checks.
  public static string Format(float value){string text=value.ToString("F4",CultureInfo.InvariantCulture);return text.StartsWith("0.")?text.Substring(1):text;}
  public static float Printed(short selector){return Single.Parse(Format(Field(selector,2,DefaultUnit)),CultureInfo.InvariantCulture);}
 }
}
````

## SlurOrigins

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurOrigins.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurOriginContext {public bool Scor4;public float Scale,LeftX,LeftY,RightY;}
 public sealed class SlurOriginState {
  public float LeftX,RightX,LeftY,DeltaY,Height,CurveCenter,WorkingRight,OriginalLeft,OriginalRight,StartFraction,EffectiveStartFraction,StopFraction,Length,HorizontalSpan;
  public short OffsetWord,NumberWord;
 }
 public static class SlurOrigins {
  static float F(double v){return (float)v;}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static short Word(float v){return unchecked((short)(int)Math.Truncate(v));}
  // Original geometry tables at DS:145E..1482 and DS:148E..14B2.
  // Only verified documented indices 1..10 are exposed; index zero is not a table entry.
  static readonly float[] left={0,0,9,27,36,18,9,18,9,0,0};
  static readonly float[] right={0,0,9,-6,-6,14,14,9,0,9,14};
  // Projection through relative 1149:0208. No buffer, CPU flag or raster side effects.
  public static SlurOriginState Prepare(float[] p,SlurOriginContext c){
   if(p==null||p.Length<21||c==null)throw new ArgumentException("Missing slur parameters/context.");
   bool explicitEnds=ScoreVersionControls.SlurEndpointOverride(c.Scor4,p);
   short offset=explicitEnds?(short)0:Word(p[8]);if(offset>0&&offset<10)throw new NotSupportedException("Positive Code5 bracket modes use another path.");
   if(offset < -9 || offset>19)throw new NotSupportedException("Offset outside measured slur-origin domain.");
   var r=new SlurOriginState{LeftX=c.LeftX,LeftY=c.LeftY,DeltaY=F((double)c.RightY-c.LeftY),Height=F((double)p[7]*c.Scale*7),CurveCenter=Present(p[10])?p[10]:.5f,OffsetWord=offset,NumberWord=Word(p[16]),StartFraction=p[14],EffectiveStartFraction=p[14],StopFraction=p[15]};
   if(explicitEnds){r.LeftX=F((double)c.LeftX+ScoreVersionControls.Horizontal(p[17]));r.WorkingRight=p[6];r.RightX=F((double)p[6]*150+ScoreVersionControls.Horizontal(p[18]));r.OriginalLeft=r.LeftX;r.OriginalRight=r.RightX;r.NumberWord=0;return r;}
   float a,b;
   if(offset==0&&Present(p[8])){double raw=(double)p[8]*36;a=F(raw);b=F(18-raw);}
   else {int index=offset<=0?10-offset:offset;index-=9;if(index>10)index=1;a=left[index];b=right[index];}
   r.LeftX=F((double)a*c.Scale+r.LeftX);
   r.WorkingRight=p[6];if(r.WorkingRight<p[3])r.WorkingRight=F((double)p[3]+3);
   b=F((double)c.Scale*b);r.RightX=F((double)r.WorkingRight*150+b);r.OriginalLeft=r.LeftX;r.OriginalRight=r.RightX;
   return r;
  }
  // Projection from 0208 through 030E for the normal curve path, before point generation.
  public static SlurOriginState Distance(float[] p,SlurOriginContext c){
   var r=Prepare(p,c);r.HorizontalSpan=F((double)r.RightX-r.LeftX);float span=r.HorizontalSpan;
   short half=Word(p[12]);
   if(!Present(r.StartFraction)&&!Present(r.StopFraction)&&half>0){
    if(half==1){r.RightX=F((double)span+r.RightX);r.StopFraction=.5f;}
    else {r.LeftX=F((double)r.LeftX-span);r.EffectiveStartFraction=.5f;r.LeftY=F((double)r.LeftY-r.DeltaY);}
    span=F(2.0*span);r.DeltaY=F(2.0*r.DeltaY);
   }
   r.Length=span;
   // Original compares the original parameter bit patterns, not transformed endpoint floats.
   if(BitConverter.ToInt32(BitConverter.GetBytes(p[4]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(p[5]),0))r.Length=F(Math.Sqrt((double)r.DeltaY*r.DeltaY+(double)span*span));
   return r;
  }
 }
}
````

## SlurPostprocess

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurPostprocess.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurWordSegment {
  readonly byte[] memory;readonly int origin;public int Length{get{return 32768;}}
  public SlurWordSegment(byte[] bytes,int address){memory=bytes;origin=address;}
  public short this[int index]{get{int a=(origin+((index&32767)*2))&0xfffff;return unchecked((short)(memory[a]|memory[(a+1)&0xfffff]<<8));}set{int a=(origin+((index&32767)*2))&0xfffff;memory[a]=(byte)value;memory[(a+1)&0xfffff]=(byte)(value>>8);}}
 }
 public sealed class SlurVectorBuffer {
  public readonly SlurWordSegment X,Y;public int Capacity=31000;
  public SlurVectorBuffer():this(0,4063){}
  public SlurVectorBuffer(ushort xSegment,ushort ySegment){var memory=new byte[1048576];X=new SlurWordSegment(memory,xSegment*16);Y=new SlurWordSegment(memory,ySegment*16);}
public int Base=16,End,CurveEnd,SampleCount;public float Angle,Sin,Cos;
  public void Copy(int destination,int source){X[destination&32767]=X[source&32767];Y[(destination-1)&32767]=Y[(source-1)&32767];}
  public void Move(int destination,int source,int count){int step=count>0?1:-1;int n=Math.Abs(count);if(n>32768)throw new NotSupportedException("Slur move count exceeds segment.");for(int i=0;i<n;i++)X[(destination+step*i)&32767]=X[(source+step*i)&32767];for(int i=0;i<n;i++)Y[(destination-1+step*i)&32767]=Y[(source-1+step*i)&32767];}
  public short[] ExtractX(){var a=new short[Math.Max(0,End-Base)];for(int i=0;i<a.Length;i++)a[i]=X[Base+i];return a;}
  public short[] ExtractY(){var a=new short[Math.Max(0,End-Base-1)];for(int i=0;i<a.Length;i++)a[i]=Y[Base+i];return a;}
 }
 public static class SlurPostprocess {
  static float F(double v){return (float)v;}static int T(double v){return checked((int)Math.Truncate(v));}static int W(float v){return unchecked((short)T(v));}static bool P(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  public static SlurVectorBuffer From(SlurSampleResult sample,SlurThicknessResult thickness,bool printed){var b=new SlurVectorBuffer();short[] x=printed?thickness.X:sample.X,y=printed?thickness.Y:sample.Y;b.End=b.Base+x.Length+1;b.CurveEnd=b.Base+sample.Count;b.SampleCount=sample.Count;b.X[b.Base]=unchecked((short)(printed?0x8046:0x8045));for(int i=0;i<x.Length;i++)b.X[b.Base+i+1]=x[i];for(int i=0;i<y.Length;i++)b.Y[b.Base+i]=y[i];return b;}
  // Relative 1149:0AB3..0F15. Coordinates and control words share the X buffer.
  public static void Pattern(SlurVectorBuffer b,float[] p,float scale,bool printed){
   int control=W(p[11]);if(control==0)return;int start=b.Base,n=b.SampleCount;
   if(p[11]<0){
    int scratch=b.Capacity-399;if(b.X[0]>b.Capacity-400)return;int count=b.CurveEnd-start;for(int k=0;k<count;k++)b.Copy(scratch+k,start+1+k);
    float size=scale;if(p[13]>0)size=F((double)p[13]*size);
    float[] table={-1.6f,-1.6f,-.6f,.6f,1.6f,1.6f,.6f,-.6f,-1.6f,-1.6f,-.6f};short[] offsets=new short[11];for(int k=0;k<11;k++)offsets[k]=unchecked((short)Math.Round((double)table[k]*size,MidpointRounding.AwayFromZero));
    b.End=start-1;for(int k=0;k<count;k++){
     b.End++;b.X[b.End]=unchecked((short)0x8046);int x=b.X[scratch+k],y=b.Y[scratch+k-1];
     for(int j=0;j<9;j++){b.End++;b.X[b.End]=unchecked((short)(x+offsets[j]));b.Y[b.End-1]=unchecked((short)(y+offsets[j+2]));}
    }
    b.CurveEnd=b.End;b.End++;return;
   }
   if(!printed){int extra=(n-1)/2;b.End+=extra;b.CurveEnd+=extra;for(int i=b.CurveEnd;i>=start+4;i-=3){int source=i-extra;b.Copy(i,source);b.Copy(i-1,source-1);b.X[i-2]=unchecked((short)0x8045);extra--;}return;}
   int originalEnd=b.End,last=originalEnd-2,output=originalEnd;b.Copy(output,last);output++;b.Copy(output,start+1);int lo=start+1,hi=originalEnd-2;output++;b.X[output]=unchecked((short)0x8046);hi-=2;
   for(int guard=0;hi>b.CurveEnd&&guard<10000;guard++){
    lo+=2;int source=lo;for(int j=1;j<=5;j++){output++;b.Copy(output,source);source++;if(j==2)source=hi;if(j==4)source=lo;}
    output++;b.X[output]=unchecked((short)0x8046);hi-=2;
   }
   int tail=b.CurveEnd-1;for(int j=1;j<=4;j++){output++;b.Copy(output,tail);tail++;if(j==3)tail=b.CurveEnd-1;}
   int countOut=output-originalEnd+1;b.Move(start+3,originalEnd,countOut);b.End=start+3+countOut;
  }
  // Relative 1149:0F15..1132; effective start and stop are working values.
  public static void Trim(SlurVectorBuffer b,float[] p,float start,float stop,bool printed){
   if((!P(start)&&!P(stop))||p[11]<0)return;int total=b.CurveEnd-b.Base;
   int first=T((double)(float)total*start+.51f),last=T((double)(float)total*stop+.5);if(last==0)last=total;int count=last-first;
   if(printed){
    if(W(p[11])!=0){first/=2;last/=2;if(first>0)first=b.Base+first*6;last=b.Base+last*6-1;if(last>b.End)last=b.End;if(first>0){count=last-first;b.Move(b.Base+1,first,count);b.End=b.Base+1+count;}else b.End=last;return;}
    if(first>0){int right=b.End-first,source=b.Base+first+1;b.Copy(right,source);if(last!=total){count+=2;int other=b.End-last,target=b.Base+last+1;b.Move(target,other,count);count=count*2-3;}else count=right-source+1;b.Move(b.Base+1,source,count);b.End=b.Base+1+count;return;}
    int from=b.End-count,to=b.Base+count+1;b.Move(to,from,count);count*=2;b.End=b.Base+1+count;return;
   }
   b.Move(b.Base+1,b.Base+1+first,count);b.End=b.Base+1+count;
  }
  // Relative 0614:0989..0B5A, called with a negative buffer index (radians).
  public static void Rotate(SlurVectorBuffer b,SlurOriginState o){
   if(!P(o.DeltaY))return;b.Angle=F(Math.Atan2(o.DeltaY,o.HorizontalSpan));double angle=-(double)b.Angle;b.Sin=F(Math.Sin(angle));b.Cos=F(Math.Cos(F(angle)));int ox=T(o.LeftX),oy=T(o.LeftY);
   for(int i=b.Base;i<b.End;i++){
    if(b.X[i]<-32600)continue;float x=F(b.X[i]-ox),y=F(b.Y[i-1]-oy);int nx=T((double)x*b.Cos+(double)y*b.Sin+ox),ny=T((double)y*b.Cos-(double)x*b.Sin+oy);b.X[i]=(short)Math.Max(-32750,Math.Min(32750,nx));b.Y[i-1]=(short)Math.Max(-32750,Math.Min(32750,ny));
   }
  }
 }
}
````

## SlurSampling

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurSampling.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurSampleResult {
  public short[] X,Y;public int Count;public float Step,FinalX,FinalIncrement,Exponent,HalfSteps,Height;public SlurOriginState Origin;
 }
 public static class SlurSampling {
  static float F(double v){return (float)v;}
  static short W(float v){return unchecked((short)(int)Math.Truncate(v));}
  static int T(double v){if(Double.IsNaN(v)||v<Int32.MinValue||v>Int32.MaxValue)throw new NotSupportedException("Conversion outside verified finite range.");return (int)Math.Truncate(v);}
  static short Y(double v){int n=T(v);if(n>32600)n=32600;return unchecked((short)n);}
  // Relative 1149:0227..07A8, normal point-generation path.
  // Math.Pow is a candidate numerical projection checked against original integer words,
  // not a claim of x87 power-helper or exception/flag equivalence.
  public static SlurSampleResult Build(float[] p,SlurOriginContext context,bool printed){
   if(W(p[16])!=0||W(p[9])==99)throw new NotSupportedException("Numbered/straight Code5 paths are separate.");
   var o=SlurOrigins.Distance(p,context);bool trimmed=BitConverter.ToInt32(BitConverter.GetBytes(p[14]),0)!=0||BitConverter.ToInt32(BitConverter.GetBytes(p[15]),0)!=0;return BuildState(p,context,o,printed,trimmed);
  }
  public static SlurSampleResult BuildState(float[] p,SlurOriginContext context,SlurOriginState o,bool printed,bool trimmed){
   // Original 0252..0259 clears the positive working half control when
   // explicit trims are present; sample counts use that cache, not source P12.
   int control=W(p[11]),half=W(p[12]);if(trimmed&&half>0)half=0;int divisor=printed?200:500;
   if(trimmed)divisor=200;
   if(control!=0)divisor=500;
   int count=(unchecked((short)T(o.Length))/divisor)*2;count=Math.Max(count,8);
   if(control!=0||printed)count=unchecked((short)(count+2*Math.Abs((int)W(p[7]))));
   if(control!=0){
    if(control< -3)control/=2;control=Math.Abs(control);
    if(control>=2){count=control*2;if(half>0)count*=2;}
    else {count-=2;if(p[11]<=0&&control==1&&count>8){count=count/2-2;if(count%2!=0)count--;if(count<5)count=6;}}
   }
   if(count<4||count>20000)throw new NotSupportedException("Sample count outside verified safe projection range.");
   float step=F((double)o.Length/(count-1));double twiceCenter=(double)o.CurveCenter+o.CurveCenter;float center2=F(twiceCenter);float increment=F(twiceCenter*step),x=o.LeftX;
   var r=new SlurSampleResult{Origin=o,Count=count,Step=step,X=new short[count],Y=new short[count],Height=o.Height};
   int switchIndex=count/2-2;
   for(int i=0;i<count;i++){int n=T(x);if(n>32760)n=32760;r.X[i]=unchecked((short)n);x=F((double)increment+x);if(i==switchIndex)increment=F((2.0-center2)*step);}
   r.FinalX=x;r.FinalIncrement=increment;
   double rawExponent=(double)p[9]+2.5;float exponent=F(rawExponent);if(rawExponent<(double).1f)exponent=.1f;r.Exponent=exponent;
   int halfSteps=count/2-1;float steps=halfSteps;r.HalfSteps=steps;int peak=T((double)o.Height+o.LeftY);
   for(int k=halfSteps;k>=0;k--){double t=(double)(float)k/steps;double power=Math.Pow(t,exponent);short value=Y(peak-power*o.Height);int j=halfSteps-k;r.Y[j]=value;r.Y[count-1-j]=value;}
   return r;
  }
 }
}
````

## SlurThickness

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurThickness.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurThicknessResult {public short[] X,Y;public float Thickness,EffectiveControl,RampStep,FinalRamp;public int Target,FinalIncrement;}
 public static class SlurThickness {
  static float F(double v){return (float)v;}
  static int T(double v){if(Double.IsNaN(v)||v<Int32.MinValue||v>Int32.MaxValue)throw new NotSupportedException("Conversion outside verified finite range.");return (int)Math.Truncate(v);}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  // Relative 1149:0993..0AB3: negative P12 reflects the second half.
  public static void Reflect(float[] p,SlurSampleResult sample,short[] y,bool printed){
   if(unchecked((short)T(p[12]))>=0)return;
   int n=sample.Count,peak=T((double)sample.Origin.LeftY+sample.Height);if(peak>32600)peak=32600;int partner=2*n-(n/2)-1;
   for(int k=n/2-1;k<n;k++){
    short twice=unchecked((short)((peak-y[k])*2));int v=y[k]+twice;if(v>32600)v=32600;y[k]=unchecked((short)v);
    if(printed&&k!=n-1){v=unchecked((short)(y[partner]+twice));if(v>32600)v=32600;y[partner]=unchecked((short)v);partner--;}
   }
  }
  public static SlurThicknessResult Build(float[] p,SlurOriginContext context,SlurSampleResult sample,float configuredControl){
   int n=sample.Count;var r=new SlurThicknessResult{X=new short[2*n-1],Y=new short[2*n-1]};Array.Copy(sample.X,r.X,n);Array.Copy(sample.Y,r.Y,n);
   float amount=F((double)context.Scale*3.5);float control=Present(p[13])?p[13]:configuredControl;if(control<=0)control=1;
   short length=unchecked((short)T(sample.Origin.Length));if(length<3000){double factor=length<=600?(double).6f:1.0-(3000-length)*(double).4f/2400.0;control=F(factor*control);}
   r.EffectiveControl=control;amount=F((double)control*amount);r.Thickness=amount;r.Target=T((double)amount+.5);float rampStep=F((double)amount/((double)sample.HalfSteps*.85f));r.RampStep=rampStep;
   int sign=unchecked((short)T(p[7]))<0?-1:1;float ramp=0;int current=0,source=n,destination=n,mirror=2*n-1,midpoint=2+(int)sample.HalfSteps;short value=0;
   for(int index=2;index<=n;index++){
    destination++;source--;r.X[destination-1]=r.X[source-1];
    if(index<=midpoint){if(current<r.Target){ramp=F((double)rampStep+ramp);current=T(ramp);}short increment=unchecked((short)(current*sign));value=unchecked((short)(r.Y[source-1]+increment));r.Y[destination-1]=value;mirror--;r.Y[mirror-1]=value;}
   }
   r.Y[destination-1]=r.Y[0];r.FinalRamp=ramp;r.FinalIncrement=current;return r;
  }
 }
}
````

## SlurVectorCommands

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurVectorCommands.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct SlurPlotRequest {public short Kind,X,Y;public SlurPlotRequest(short k,short x,short y){Kind=k;X=x;Y=y;}}
 public static class SlurVectorCommands {
  // The original PostScript buffer reader treats a marker followed by a word
  // as a packet. The following X word is a coordinate even when it resembles
  // another marker. This matters at the lower coordinate clamp.
  public static List<SlurPlotRequest> Decode(SlurVectorBuffer b,int root,bool appendRestore,bool printed){
   var result=new List<SlurPlotRequest>();int originalEnd=b.End,end=originalEnd+(appendRestore?1:0);bool pending=true;
   for(int i=root;i<end;i++){
    short x=appendRestore&&i==originalEnd?unchecked((short)0x80a7):b.X[i];short kind=0;
    if(printed?x< -32600:x<= -32600){
     short command=unchecked((short)(x+0x7fbc));
     if(command==98||command==99){result.Add(new SlurPlotRequest(command,0,b.Y[i-1]));pending=true;continue;}
     if(command<90){i++;x=appendRestore&&i==originalEnd?unchecked((short)0x80a7):b.X[i];if(command>0){if(command>3)throw new NotSupportedException("Slur marker "+command);kind=command;}}
     else throw new NotSupportedException("Slur marker "+command);
    }
    if(kind==0&&pending)kind=1;result.Add(new SlurPlotRequest(kind,x,b.Y[i-1]));pending=false;
   }
   return result;
  }
 }
}
````

## SpanBookkeeping

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SpanBookkeeping.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface ISpanBookkeepingMemory {
  ushort ReadWord(ushort segment,ushort offset);
  byte ReadByte(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
  void WriteByte(ushort segment,ushort offset,byte value);
 }
 public static class SpanBookkeeping {
  // Original 1F08:011F..0136. Data effects plus updated SI only.
  // The two descriptor-pointer reads remain separate, preserving alias order.
  public static ushort Execute(ISpanBookkeepingMemory m,ushort ss,ushort sourceArgumentOffset,ushort ds,ushort remainingSi,ushort transferredDi){
   if(m==null)throw new ArgumentNullException("m");
   ushort value=m.ReadWord(ss,sourceArgumentOffset);
   m.WriteWord(ss,sourceArgumentOffset,unchecked((ushort)(value+transferredDi)));
   ushort descriptor=m.ReadWord(ds,0x6259);
   ushort usedOffset=unchecked((ushort)(descriptor+0x0a));
   value=m.ReadWord(ds,usedOffset);
   m.WriteWord(ds,usedOffset,unchecked((ushort)(value+transferredDi)));
   remainingSi=unchecked((ushort)(remainingSi-transferredDi));
   if(unchecked((short)transferredDi)>0){
    descriptor=m.ReadWord(ds,0x6259);
    ushort flagOffset=unchecked((ushort)(descriptor+4));
    byte flags=m.ReadByte(ds,flagOffset);m.WriteByte(ds,flagOffset,(byte)(flags|1));
   }
   return remainingSi;
  }
 }
}
````

## StaffHeightLayout

[Workspace source](../../ScoreReverseEngineering/NativeFragments/StaffHeightLayout.cs)

````csharp
using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class StaffHeightLayout {
  //2CF5:0347..0595, ordinary P4/P5 with zero P8/P10. The instruction
  //055A..0567 stores P10 + cumulative physical gap +10000 before the later parameter update subtracts the marker.
  //stream retains the unrounded physical difference until the P4 step store.
  public static float[] Targets(float[] positions,float[] sizes,int maximum,int[] groups,float oldHeight,float newHeight,bool metric,float[] physicalPositions){
   if(maximum<2||maximum>32||groups.Length==0||groups[0]<=0)throw new NotSupportedException("Height grouping leaves its verified positive staff domain.");var bounds=new List<int>();int total=0,last=groups[0];for(int i=0;i<32;i++){if(i<groups.Length&&groups[i]!=0)last=groups[i];if(last<1)throw new NotSupportedException("Nonpositive height groups are prevented.");total+=last;bounds.Add(Math.Min(maximum,total));if(total>=maximum)break;}if(total<maximum||bounds.Count<2)throw new NotSupportedException("Height grouping has no adjustable interval.");
   float unit=metric?0.11112520098686218f:0.04374999925494194f;double physical=((double)newHeight-oldHeight)/(bounds.Count-1);float step=(float)(physical/unit),current=step,physicalStep=(float)physical,currentPhysical=physicalStep;if(Single.IsNaN(step)||Single.IsInfinity(step))throw new NotSupportedException("Height step leaves the finite domain.");var targets=(float[])positions.Clone();for(int group=0;group<bounds.Count-1;group++){for(int staff=bounds[group]+1;staff<=bounds[group+1];staff++){if(physicalPositions[staff]!=0){float packed=(float)((double)physicalPositions[staff]+currentPhysical+10000);if(!(packed>10000&&packed<10300))throw new NotSupportedException("Height P10 target reaches an unverified marker boundary.");physicalPositions[staff]=(float)((double)packed-10000);continue;}float size=sizes[staff];if(size==0)size=1;if(!(size>0)||Single.IsInfinity(size))throw new NotSupportedException("Height staff size leaves its verified positive domain.");targets[staff]=(float)((double)current/size+positions[staff]);if(Single.IsNaN(targets[staff])||Single.IsInfinity(targets[staff])||Math.Abs(targets[staff])>=300)throw new NotSupportedException("Height target reaches unverified packed staff controls.");}current=(float)((double)step+current);currentPhysical=(float)((double)physicalStep+currentPhysical);}return targets;
  }
 }
}
````

## StringOutputRoutine

[Workspace source](../../ScoreReverseEngineering/NativeFragments/StringOutputRoutine.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public interface IStringOutputContext:ICharacterFrameMemory {
  byte ReadByte(ushort segment,ushort offset);
  CharacterRoutineResult CallCharacter(CharacterRoutineWords words,ushort targetSegment,ushort targetOffset);
 }
 public struct StringOutputResult {public CharacterRoutineResult Result;public bool ReachedStringReturn;}
 public static class StringOutputRoutine {
  // Original 16FA:06F6..0712 word/memory/control projection. Segment values are
  // explicit relocation/context inputs. No host text encoding or callee ABI is inferred.
  public static StringOutputResult Run(IStringOutputContext m,CharacterRoutineWords w,ushort currentSegment,ushort characterSegment){
   if(m==null)throw new ArgumentNullException("m");
   CharacterEntryWords e=CharacterOutputFrame.Enter(m,w.SS,w.SP,w.BP,w.DI);w.SP=e.SP;w.BP=e.BP;w.DI=e.DI;w.ES=e.ES;
   while(true){
    byte b=m.ReadByte(w.ES,w.DI);w.AX=(ushort)((w.AX&0xff00)|b);
    if(b==0)break;
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,w.ES);
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,w.DI);
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,currentSegment);
    w.SP=unchecked((ushort)(w.SP-2));m.WriteWord(w.SS,w.SP,0x070b);
    CharacterRoutineResult c=m.CallCharacter(w,characterSegment,0x073b);w=c.Words;
    if(c.CS!=currentSegment||c.IP!=0x070b)return new StringOutputResult{Result=c,ReachedStringReturn=false};
    w.DI=unchecked((ushort)(w.DI+1));
   }
   CharacterReturnWords r=CharacterOutputFrame.Leave(m,w.SS,w.SP);w.SP=r.SP;w.BP=r.BP;w.DI=r.DI;
   return new StringOutputResult{Result=new CharacterRoutineResult{Words=w,CS=r.CS,IP=r.IP},ReachedStringReturn=true};
  }
 }
}
````

## TextConverterFontState

[Workspace source](../../ScoreReverseEngineering/NativeFragments/TextConverterFontState.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public struct TextConverterFontState {
  public bool Forced;public short Word;
  // Relative 020F:001D..004C: retain the full hundreds component of the
  // converter's font word. File loading and its failure effects stay external.
  public static TextConverterFontState Begin(short overrideWord,short previousWord){
   return new TextConverterFontState{Forced=overrideWord>=90,
    Word=overrideWord>=90?(short)(overrideWord%1000):previousWord};
  }
  // Single/double-underbar font tokens. 1100 selects outline Times when
  // unforced, but is ignored when a P8 override forces the font.
  // This consumes decoded tokens, not the complete string grammar.
  public void Accept(short token){if(!Forced&&token>=1000&&token<=1199)Word=(short)(token-1000);}
  public int MetricFace{get{return Word%100;}}
 }
}
````

## TextMetricArithmetic

[Workspace source](../../ScoreReverseEngineering/NativeFragments/TextMetricArithmetic.cs)

````csharp
namespace ScoreModern.RecoveredFragments {
 public struct TextMetricScale {public float Character,ExtraSpace;}
 public static class TextMetricArithmetic {
  // Relative 14A6:0872..0882: standard-font metric adaptation for the
  // primitive stroke path. No binary32 store occurs between multiply/divide.
  public static float StrokeHorizontal(float metric,float horizontal,float primitiveMetric){
   if(float.IsNaN(metric)||float.IsInfinity(metric)||float.IsNaN(horizontal)||float.IsInfinity(horizontal)||float.IsNaN(primitiveMetric)||float.IsInfinity(primitiveMetric)||primitiveMetric==0)throw new System.NotSupportedException("Requires finite metrics and nonzero primitive metric.");
   return (float)((double)metric*horizontal/primitiveMetric);
  }
  // Numerical projection of relative 020F:0052..007F and 050C..0587,
  // plus metric normalization at 14A6:0D6B..0D9C. External metrics and
  // measured constants are arguments; no font data or parser is embedded.
  // Binary32 stores are explicit. This is not an x87 flags/exception model.
  public static TextMetricScale Prepare(float size,float staff,float divisor,float factor,float extraSpace){
   double initial=(double)size*staff/divisor;
   float stored=(float)initial;
   return new TextMetricScale{Character=(float)((double)factor*stored),ExtraSpace=(float)(initial*extraSpace)};
  }
  public static float Normalize(short width,float divisor){return (float)((double)width/divisor);}
  public static float Advance(float metric,TextMetricScale scale,bool space){
   float result=(float)((double)scale.Character*metric);
   return space?(float)((double)result+scale.ExtraSpace):result;
  }
  public static float Accumulate(float total,float advance){return (float)((double)total+advance);}
  public static float Finish(float total,float multiplier){return (float)((double)multiplier*total);}
  // Relative 14A6:0B16..0B36. The index is the state at this leaf's entry;
  // the output callee may replace it before this leaf is reached.
  public static float AddPosition(short index,float x,float advance,float units){return index>160?x:(float)((double)advance*units+x);}
 }
}
````

## TextParameterPreparation

[Workspace source](../../ScoreReverseEngineering/NativeFragments/TextParameterPreparation.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct TextPreparedParameters {
  public float Spacing,Horizontal,Vertical,GlyphHorizontal,GlyphVertical,Origin,Rotation,RestoredHorizontal,RestoredVertical;
 }
 public static class TextParameterPreparation {
  static float F(double v){return (float)v;}
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  // Relative 14A6:0017..0080, 00A8..00D1, 01BB..0223.
  // Finite scalar projection; caller supplies measured constants and initial X.
  // Character decoding, font override services and x87 flags are outside this API.
  public static TextPreparedParameters Prepare(float[] p,float x,float modulus,float glyphScale,float units){
   float h=F((double)p[6]%modulus);if(!Present(h))h=1;
   float v=Present(p[7])?p[7]:h;
   return new TextPreparedParameters{Spacing=Present(p[5])?p[5]:1,Horizontal=h,Vertical=v,
    GlyphHorizontal=F((double)h*glyphScale),GlyphVertical=F((double)v*glyphScale),
    Origin=F((double)p[11]*units+x),Rotation=p[9],RestoredHorizontal=p[6],RestoredVertical=p[7]};
  }
  // Relative 14A6:00F3..019B. The counter begins at -1, so this distributes
  // the width difference over gaps, rather than over the number of glyphs.
  public static float Interletter(float total,float factor,float[] advances){
   if(factor==1)return 0;int gaps=-1;foreach(float value in advances)if(value!=0)gaps++;
   return gaps<1?0:F(((double)total-(double)total/factor)/gaps);
  }
  public static float[] Adjust(float[] advances,float delta){
   var result=(float[])advances.Clone();for(int j=0;j<result.Length-1;j++)if(Present(result[j]))result[j]=F((double)result[j]+delta);return result;
  }
  // Relative 14A6:0231..026E; screen and printed units remain explicit.
  public static float OutputExtra(float value,bool printed,float units){return printed?F((double)value*units):value;}
 }
}
````

## TupletBracket

[Workspace source](../../ScoreReverseEngineering/NativeFragments/TupletBracket.cs)

````csharp
using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct TupletBracketPoint {public short Tag;public float X,Y;public TupletBracketPoint(short tag,float x,float y){Tag=tag;X=x;Y=y;}}
 public sealed class TupletBracketContext {public float Scale,LeftX,LeftY,RightY;}
 public sealed class TupletBracketResult {public readonly List<TupletBracketPoint> Points=new List<TupletBracketPoint>();public TupletPlacement? Number;}
 public static class TupletBracket {
  static float F(double value){return (float)value;}
  static bool Present(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0)!=0;}
  static short I(float value){return unchecked((short)(int)Math.Truncate(value));}
  // Geometry projection of relative 1149:000C..00B9,12D0..1653 and
  // 18A7..18E4 for P8 word 3. Caller coordinates are supplied explicitly.
  public static TupletBracketResult Build(float[] p,TupletBracketContext c){
   if(p==null||p.Length<21||c==null)throw new ArgumentException("Missing bracket context or parameters.");
   if(I(p[8])!=3)throw new NotSupportedException("This path is the original Code5 tuplet bracket.");
   var result=new TupletBracketResult();float scale=c.Scale,left=c.LeftX,leftY=c.LeftY;
   float percent=Present(p[10])?p[10]:.5f;
   float height=F((double)p[7]*scale*7),dy=F((double)c.RightY-leftY),top=F((double)height+leftY);
   float right=F((double)p[6]*150);short side=I(p[9]),mode=I(p[11]);short start=1;
   if(side!=-1){result.Points.Add(new TupletBracketPoint(1,left,leftY));start=0;}
   result.Points.Add(new TupletBracketPoint(start,left,top));
   short number=side>0?side:(short)3;float gap=F((double)scale*13);if(number>9)gap=F((double)gap*1.5f);
   double extendedRight=(double)scale*17.5f+right;right=F(extendedRight);float dx=F(extendedRight-left);
   if(side< -1)gap=F(-(double)p[9]*75);
   float along=F((double)dx*percent),gapLeft=F((double)left-gap+along),numberY;
   if(Present(dy)){
    leftY=F((double)dy+leftY);
    numberY=F((double)height-(1.0-percent)*dy+leftY);
    float slope=F((double)dy/dx);
    if(mode==0)result.Points.Add(new TupletBracketPoint(0,gapLeft,F(((double)gapLeft-c.LeftX)*slope+top)));
    gapLeft=F((double)gap+gap+gapLeft);
    if(mode==0)result.Points.Add(new TupletBracketPoint(1,gapLeft,F(((double)gapLeft-c.LeftX)*slope+top)));
    top=F((double)dy+top);
   }else{
    numberY=top;
    if(mode==0){result.Points.Add(new TupletBracketPoint(0,gapLeft,top));float gapRight=F((double)gap+gap+gapLeft);result.Points.Add(new TupletBracketPoint(1,gapRight,top));}
   }
   result.Points.Add(new TupletBracketPoint(0,right,top));result.Points.Add(new TupletBracketPoint(0,right,leftY));
   if(side< -1||mode==3)return result;
   if(mode!=0){float offset=F(-(double)scale*14);short leg=I(p[7]);if(mode==2)leg=unchecked((short)-leg);if(leg<0)offset=-offset;numberY=F((double)numberY+offset);}
   float textY=F((double)numberY-(double)scale*9);textY=F((double)p[18]*scale*7+textY);
   float textLeft=F((double)p[17]*scale*6+left);
   result.Number=new TupletPlacement{Number=number,X=TupletGeometry.Center(number,scale,along,textLeft),Y=textY,FontScale=.875f};return result;
  }
 }
}
````

## TupletGeometry

[Workspace source](../../ScoreReverseEngineering/NativeFragments/TupletGeometry.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 // Data projection of 00FD:0974..0A53 and 1149:1835..1891.
 // Explicit caller reference endpoints; no font substitution or CPU-state claim.
 public struct TupletPlacement {public float X,Y,FontScale;public short Number;}
 public static class TupletGeometry {
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  public static string NumberText(short number){
   if(number>1000){int n=number;if(n>2000)n-=968;return ((char)unchecked((byte)(n+88))).ToString();}
   if(number<10)return ((char)unchecked((byte)(number+48))).ToString();
   if(number<100)return new string(new[]{(char)(number/10+48),(char)(number%10+48)});
   return new string(new[]{(char)(number/100+48),(char)((number%100)/10+48),(char)(number%10+48)});
  }
  public static float Center(short number,float scale,float along,float left){float factor=number<10?7:number>=20?14.5f:number==11?12:13.5f;return (float)((double)left-(double)scale*factor+along);}

  // Measured 1CCA:0205 and 2CAA:0E4D..0ECE, including float32 stores.
  public static float PrintedEm(float fontScale,float staffScale){float a=(float)((double)fontScale*.2800000011920929f);float b=(float)((double)a*staffScale);return (float)((double)b*110.20600128173828f);}

  public static TupletPlacement Build(short number,int direction,bool cue,float percent,float verticalShift,float scale,float leftX,float leftY,float dx,float dy){
   float offset=direction==1?6.5f:-24.5f;
   offset=(float)((double)verticalShift*7+offset);
   float font=.875f;if(cue){offset=(float)((double)offset*.699999988079071f);font=.6000000238418579f;}
   if(!Present(percent))percent=.5f;
   float y=(float)((double)offset*scale+(double)dy*percent+leftY);
   float along=(float)((double)percent*dx);
   float x=Center(number,scale,along,leftX);
   return new TupletPlacement{Number=number,X=x,Y=y,FontScale=font};
  }
 }
}
````

## VerticalRaster

[Workspace source](../../ScoreReverseEngineering/NativeFragments/VerticalRaster.cs)

````csharp
using System;
namespace ScoreModern.RecoveredFragments {
 public struct VerticalRasterWords { public ushort AX,BX,CX,DX,SI,DI,BP;public uint EFlags,DefinedEFlagsMask; }
 public static class VerticalRaster {
  // Relative 1807:0A96..0AC5. Video byte-OR effects remain external.
  public static VerticalRasterWords Execute(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx) {
   return Core(m,ds,es,ax,bx,cx,dx,null);
  }
  public static VerticalRasterWords ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,uint eflags){return Core(m,ds,es,ax,bx,cx,dx,new RasterFlagState{Value=eflags});}
  static VerticalRasterWords Core(IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,RasterFlagState flags){
   ushort si=ax;
   cx=(ushort)((cx&0xff00)|8); ax=m.ReadWord(ds,0x4b68);
   if(ax>=2048)throw new NotSupportedException("Original byte-DIV fault is outside this projection.");
   ax=(ushort)((ax/8)|((ax%8)<<8));
   cx=(ushort)((cx&0xff00)|(ax>>8)); dx=(ushort)((dx&0xff00)|0x80);
   int shift=(cx&255)&7; byte mask=(byte)dx;
   mask=(byte)((mask>>shift)|(mask<<((8-shift)&7)));dx=(ushort)((dx&0xff00)|mask);
   cx=bx;cx=(ushort)(cx+1);cx=(ushort)(cx-m.ReadWord(ds,0x4b6a));
   ax=(ushort)(ax&255);ushort bp=ax;bx=(ushort)(si<<1);ushort di;
   do { di=m.ReadWord(ds,(ushort)(bx+0x456a));di=(ushort)(di+bp);m.OrByte(es,di,(byte)dx);
    ushort stride=m.ReadWord(ds,0x4b78);if(flags!=null)flags.Value=RasterStatusFlags.Add16(flags.Value,bx,stride);bx=(ushort)(bx+stride);cx=(ushort)(cx-1);
   } while(cx!=0);
   return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=flags==null?0:flags.Value,DefinedEFlagsMask=flags==null?0:0xffffffffu};
  }
 }
}
````

## VerticalRasterFlagsModel

[Workspace source](../../ScoreReverseEngineering/NativeFragments/VerticalRasterFlagsModel.cs)

````csharp
using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class VerticalRasterFlagsModel {
 class Memory:IByteOrResultMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public byte OrByteAndGetResult(ushort s,ushort o,byte mask){OrByte(s,o,mask);return Bytes[(s<<4)+o];}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(VerticalRasterWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP+":"+w.EFlags;}
 static VerticalRasterWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort bx,ushort cx,ushort dx,uint inputFlags){uint fullFlags=inputFlags;int pc=0,steps=0;ushort si=0,di=0,bp=0;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<1000000){switch(f()){
 case 0x8b:byte op=f();if(op==0xf0)si=ax;else if(op==0xcb)cx=bx;else if(op==0xe8)bp=ax;else if(op==0xde)bx=si;else if(op==0xbf)di=m.ReadWord(ds,(ushort)(bx+word()));else throw new Exception("MOV");break;
 case 0xb1:cx=(ushort)((cx&0xff00)|f());break;case 0xa1:ax=m.ReadWord(ds,word());break;
 case 0xf6:if(f()!=0xf1)throw new Exception("DIV opcode");int v=cx&255;if(v==0||ax/v>255)throw new Exception("Excluded fault");ax=(ushort)((ax/v)|((ax%v)<<8));break;
 case 0x8a:if(f()!=0xcc)throw new Exception("MOV CL");cx=(ushort)((cx&0xff00)|(ax>>8));break;
 case 0xb2:dx=(ushort)((dx&0xff00)|f());break;
 case 0xd2:if(f()!=0xca)throw new Exception("ROR");for(int n=0;n<(cx&255);n++){byte b=(byte)dx;dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}break;
 case 0x41:cx=(ushort)(cx+1);break;case 0x2b:if(f()!=0x0e)throw new Exception("SUB");cx=(ushort)(cx-m.ReadWord(ds,word()));break;
 case 0x2a:if(f()!=0xe4)throw new Exception("SUB AH");ax=(ushort)(ax&255);break;
 case 0xd1:if(f()!=0xe3)throw new Exception("SHL");bx=(ushort)(bx<<1);break;
 case 0x03:byte add=f();if(add==0xfd)di=(ushort)(di+bp);else if(add==0x1e){ushort step=m.ReadWord(ds,word());fullFlags=IndependentFlagArithmetic.Apply(fullFlags,bx,step,16,true);bx=(ushort)(bx+step);}else throw new Exception("ADD");break;
 case 0x26:if(f()!=8||f()!=0x15)throw new Exception("OR");m.OrByte(es,di,(byte)dx);fullFlags=IndependentFlagArithmetic.Result(fullFlags,0,((Memory)m).Bytes[(es<<4)+di],8);break;
 case 0xe2:sbyte d=(sbyte)f();cx=(ushort)(cx-1);if(cx!=0)pc+=d;break;
 case 0xeb:if(f()!=0x51||pc!=47||code[47]!=0x90)throw new Exception("External exit");pc=48;break;
 default:throw new Exception("Opcode");
 }}if(pc!=48)throw new Exception("Exit");return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp,EFlags=fullFlags};}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 class Captured:IHorizontalRasterMemory {public byte[] Data,Plane;public ushort DS,ES;public int Writes;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){if(s!=ES)throw new Exception("Video segment");Plane[o]|=v;Writes++;}}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[48];Array.Copy(raw,0x18070+0xa96,code,0,48);
 if(args.Length>1){string p=args[1];byte[] live=File.ReadAllBytes(Path.Combine(p,"raster-code.bin"));if(live.Length!=48)throw new Exception("Live length");for(int i=0;i<48;i++)if(live[i]!=code[i])throw new Exception("Live code");Captured m=new Captured{DS=H(args[2]),ES=H(args[3]),Data=File.ReadAllBytes(Path.Combine(p,"raster-before-ds.bin")),Plane=File.ReadAllBytes(Path.Combine(p,"raster-before-plane2.bin"))};var r=VerticalRaster.Execute(m,m.DS,m.ES,H(args[4]),H(args[5]),H(args[6]),H(args[7]));var e=new VerticalRasterWords{AX=H(args[8]),BX=H(args[9]),CX=H(args[10]),DX=H(args[11]),SI=H(args[12]),DI=H(args[13]),BP=H(args[14])};if(Show(r)!=Show(e))throw new Exception("Runtime words "+Show(r)+" != "+Show(e));byte[] after=File.ReadAllBytes(Path.Combine(p,"raster-after-ds.bin")),plane=File.ReadAllBytes(Path.Combine(p,"raster-after-plane2.bin"));for(int i=0;i<65536;i++)if(m.Data[i]!=after[i]||m.Plane[i]!=plane[i])throw new Exception("Runtime memory "+i);Console.WriteLine("PASS original vertical block: 48 code bytes, seven result words, all 65536 DS and 65536 plane2 bytes; "+m.Writes+" explicit plane2 OR effects.");return 0;}
 int count=0;for(int x=0;x<640;x++)for(int k=0;k<32;k++){ushort ds=0x2000,es=(ushort)((k&1)==0?0x4000:ds),row=(ushort)(k*1021),last=(ushort)(row+(k%15)),cx=(ushort)(x^0xa55a),dx=(ushort)(k*701);Memory seed=new Memory{Pattern=k};seed.Seed(ds,0x4b68,(ushort)x);seed.Seed(ds,0x4b6a,row);seed.Seed(ds,0x4b78,(ushort)((k&2)==0?2:0xfffe));Memory a=seed.Copy(),b=seed.Copy();var n=VerticalRaster.ExecuteWithFlags(a,ds,es,row,last,cx,dx,0xA55A1234u ^ row);var q=Interpret(code,b,ds,es,row,last,cx,dx,0xA55A1234u ^ row);if(Show(n)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model state");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effect");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Model memory");count++;}
 Console.WriteLine("PASS full EFLAGS "+count+" instruction-model cases: 640 columns, mask positions, word wrapping, positive/negative row strides and data/video aliases. Ordinary RAM OR model; successful DIV paths only.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
````
