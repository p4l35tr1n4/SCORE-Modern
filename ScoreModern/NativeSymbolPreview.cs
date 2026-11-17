using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScoreModern {
 internal static class NativeSymbolPreview {
  internal static void Draw(Graphics graphics,Glyph glyph,RectangleF area){
   if(glyph.Points.Length==0||area.Width<=10||area.Height<=10)return;
   var bounds=glyph.Bounds;
   // The raw vertex bounds exclude the stroke. Allow for the pen's minimum
   // screen width, its miter joins at larger scales, and antialiasing.
   const float devicePadding=5,vectorPadding=2;
   float scale=Math.Min((area.Width-2*devicePadding)/(bounds.Width+2*vectorPadding),
    (area.Height-2*devicePadding)/(bounds.Height+2*vectorPadding));
   var state=graphics.Save();
   try{
    graphics.SetClip(area,CombineMode.Intersect);
    graphics.SmoothingMode=SmoothingMode.AntiAlias;
    glyph.Draw(graphics,area.Left+area.Width/2-(bounds.Left+bounds.Width/2)*scale,
     area.Top+area.Height/2+(bounds.Top+bounds.Height/2)*scale,scale,scale,0,false,Color.Black);
   }finally{graphics.Restore(state);}
  }
 }
}
