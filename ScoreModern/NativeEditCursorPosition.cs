using System;using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeEditCursorPosition {
  // Recovered object cursor paths in original 1E75:054C..070E.
  // Cursor pitch is an integer control with its own type/rest offset table;
  // graphics coordinates retain their independent fractional controls.
  static readonly short[] TypeOffsets={1,-1,0,0,-1,0,0,-1,2,-1,-1,-1,-1,-1,0,-1,-1,4,3};
  static readonly short[] RestOffsets={5,5,7,8,7,4,4,2,2,1,1,1,1};
  static short Word(float value){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out n))throw new NotSupportedException("Cursor parameter outside the recovered finite conversion range.");return unchecked((short)n);}
  static short Pixel(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value<Int32.MinValue||value>Int32.MaxValue)throw new NotSupportedException("Cursor projection outside the recovered signed-integer range.");return unchecked((short)(int)value);}
  public static Point Get(Item item,NativeNotationContext page,short dx,short dy,short originX,short originY){
   if(item.Code>=1&&item.Code<=18){
    short pitch=Word(item.P(4)),offset=TypeOffsets[item.Code],xoffset=0;
    if(item.Code<=3||item.Code==6)pitch=(short)(pitch%100);
    short head=Word(item.P(6)),extension=Word(item.P(8));if(item.Code==1&&head<=-4&&extension>0)pitch=unchecked((short)((-(head%10)-extension)*2+3));
    if(item.Code==2){int rest=Word(item.P(5));if(rest< -5||rest>7)rest=-5;offset=RestOffsets[rest+5];}
    else if(item.Code==14){pitch=3;offset=0;}
    else if(item.Code==6){if(Word(Math.Abs(item.P(7)))>=20)offset=-7;else{offset=6;xoffset=2;}}
    else if(item.Code==5){if(extension==-1)xoffset=1;else if(extension==-2)xoffset=3;}
    else if(item.Code==8){pitch=2;offset=0;}
    short control=unchecked((short)(pitch+offset));float rawY=(float)((double)page.Scale*control*7+page.RawBase);
    return new Point(Pixel(((double)item.P(3)+xoffset)*150/dx+originX),Pixel((double)originY-(double)rawY/dy));
   }
   return new Point(unchecked((short)(NativeNotationContext.Word(page.Prepared.X,true)/dx+originX)),unchecked((short)(-NativeNotationContext.Word(page.Prepared.Y,false)/dy+originY)));
  }
 }
}
