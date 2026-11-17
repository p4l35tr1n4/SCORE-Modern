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
