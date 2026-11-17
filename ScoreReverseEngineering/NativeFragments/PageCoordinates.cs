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


