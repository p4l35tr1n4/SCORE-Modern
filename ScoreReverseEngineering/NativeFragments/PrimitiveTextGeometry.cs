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
