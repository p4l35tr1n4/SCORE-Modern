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
