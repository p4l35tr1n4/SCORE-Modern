using System;
namespace ScoreModern {
 public static class NativeDosEditCursor {
  // Built-in cursor words measured at DS:451A/4556. Original 202B:04C9..057D
  // saves the covered background, then ANDs/XORs these rotated 16-bit masks.
  // These are UI pixels,
  // not a font or a replacement for external DAT notation resources.
  static readonly ushort[] Ink={0,0x10,0x38,0x7c,0xfe,0x38,0x38,0x38,0x38,0};
  static readonly ushort[] Mask={0xffef,0xff83,0xff01,0xff00,0xff00,0xff00,0xff01,0xff01,0xff01,0xff01};
  static ushort Rotate(ushort x,int n){return unchecked((ushort)((x>>n)|(x<<(16-n))));}
  public sealed class State {
   readonly byte[] background=new byte[20];int column,top;bool active;
   internal void RestorePixels(byte[] plane){if(!active)return;for(int row=0;row<10;row++)Buffer.BlockCopy(background,row*2,plane,(top+row)*80+column,2);}
   public void Hide(byte[] plane){RestorePixels(plane);active=false;}
   public void Show(byte[] plane,short x,short y){Hide(plane);int left=Math.Max(1,Math.Min(632,(int)unchecked((short)(x-3))));column=left/8;top=Math.Max(97,Math.Min(453,(int)y));for(int row=0;row<10;row++)Buffer.BlockCopy(plane,(top+row)*80+column,background,row*2,2);Draw(plane,x,y);active=true;}
  }
  // 0F4F:179D reads DOS time and shows the arrow in these two intervals.
  internal static bool VisibleAt(int hundredths){return hundredths>25&&(hundredths<50||hundredths>75);}
  public static void Draw(byte[] plane,short x,short y){
   int left=Math.Max(1,Math.Min(632,(int)unchecked((short)(x-3)))),top=Math.Max(97,Math.Min(453,(int)y)),shift=left%8,column=left/8;
   for(int row=0;row<10;row++){int offset=(top+row)*80+column;ushort previous=(ushort)(plane[offset]|plane[offset+1]<<8),value=(ushort)((previous&Rotate(Mask[row],shift))^Rotate(Ink[row],shift));plane[offset]=(byte)value;plane[offset+1]=(byte)(value>>8);}
  }
 }
}
