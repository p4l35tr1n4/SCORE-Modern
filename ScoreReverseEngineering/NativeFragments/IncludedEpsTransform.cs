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
