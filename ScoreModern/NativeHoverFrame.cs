using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Canvas {
  Bitmap hoverFrame;
  bool hoverFrameValid,hoverInvalidation;

  protected override void OnInvalidated(InvalidateEventArgs e){
   if(!hoverInvalidation)hoverFrameValid=false;
   base.OnInvalidated(e);
  }

  void DiscardHoverFrame(){
   hoverFrameValid=false;
   if(hoverFrame!=null){hoverFrame.Dispose();hoverFrame=null;}
  }

  void PaintScoreScreen(PaintEventArgs e){
   base.OnPaint(e);
   int width=ClientSize.Width,height=ClientSize.Height;
   bool cacheable=NativeMouseSelector.ShowPoints&&Score!=null&&Parent!=null&&!UsesOriginalZoomFrame&&
    !NativeNotationView.IsDosViewport&&!GhostscriptTextRenderer.Enabled&&
    RenderResourceAudit.Current==null&&width>0&&height>0&&(long)width*height<=12000000&&
    !Score.Items.Exists(item=>item.Code==11||item.Code==15);
   if(!cacheable){
    DiscardHoverFrame();
    PaintScore(e,true);
    return;
   }
   if(hoverFrame==null||hoverFrame.Size!=ClientSize){
    DiscardHoverFrame();
    hoverFrame=new Bitmap(width,height,PixelFormat.Format32bppArgb);
    hoverFrame.SetResolution(e.Graphics.DpiX,e.Graphics.DpiY);
   }
   if(!hoverFrameValid){
    using(var graphics=Graphics.FromImage(hoverFrame)){
     graphics.Clear(BackColor);
     PaintScore(new PaintEventArgs(graphics,ClientRectangle),false);
    }
    hoverFrameValid=true;
   }
   e.Graphics.DrawImageUnscaled(hoverFrame,0,0);
   var state=e.Graphics.Save();
   try{
    e.Graphics.ScaleTransform(RenderScale,RenderScale);
    e.Graphics.TranslateTransform(0,-CropTop/RenderScale);
    e.Graphics.TranslateTransform(CameraOffset.X+ScreenOrigin.X,CameraOffset.Y+ScreenOrigin.Y);
    NativeMouseSelector.DrawPoints(this,e.Graphics);
   }finally{e.Graphics.Restore(state);}
   NotifyGuides();
  }

  protected override void Dispose(bool disposing){
   if(disposing)DiscardHoverFrame();
   base.Dispose(disposing);
  }
 }
}
