using System;using System.IO;using System.Drawing;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativePageFraming {
  public static bool FollowPrint{get;private set;}
  static NativePageFraming(){FollowPrint=true;}
  public static void Load(){FollowPrint=NativeSettings.Read("page-frame")!="letter-overflow";}
  public static void Set(bool follow,bool save){if(save)NativeSettings.WriteAllText("page-frame",follow?"print":"letter-overflow");FollowPrint=follow;foreach(Form form in Application.OpenForms){var editor=form as Editor;if(editor!=null)editor.RefreshPageFrame();}}
 }
 public sealed partial class Editor {
  SizeF ActivePaperSize(){var job=printJob;return job==null?new SizeF(8.5f,11):new SizeF(job.PaperWidth,job.PaperHeight);}
  internal void RefreshPageFrame(){RefreshRulerLayout();canvas.LayoutScore();if(fitting)Fit();UpdateClassic();canvas.Invalidate();}
  void AddPageFrameMenu(ContextMenuStrip menu){var active=new ToolStripMenuItem("Page frame: follow active print paper size");var fixedPage=new ToolStripMenuItem("Page frame: fixed Letter with off-page content");active.Click+=(s,e)=>NativePageFraming.Set(true,true);fixedPage.Click+=(s,e)=>NativePageFraming.Set(false,true);menu.Items.Add(active);menu.Items.Add(fixedPage);menu.Opening+=(s,e)=>{active.Checked=NativePageFraming.FollowPrint;fixedPage.Checked=!NativePageFraming.FollowPrint;var size=ActivePaperSize();active.ToolTipText="Active paper: "+size.Width+" x "+size.Height+" inches. Both views show overflow without changing engraving or EPS. "+canvas.ScreenFrameStatus;};}
 }
 public sealed partial class Canvas {
  public Func<SizeF> ScreenPaperSize;
  RectangleF screenWorld,screenPaper;
  uint screenStamp;Score screenScore;string screenLibrary;RectangleF screenInk;
  public string ScreenFrameStatus="";
  internal bool ScreenFraming{get{return ScreenPaperSize!=null&&!NativeNotationView.IsDosViewport&&!UsesOriginalZoomFrame;}}
  internal PointF ScreenOrigin{get{return ScreenFraming?new PointF(-screenWorld.Left,-screenWorld.Top):PointF.Empty;}}
  internal float ScreenWidth{get{return UsesOriginalZoomFrame?640*NativeDosViewport.Divisor/30f:ScreenFraming&&!screenWorld.IsEmpty?screenWorld.Width:PageWidth;}}
  internal float ScreenHeight{get{return UsesOriginalZoomFrame?365*NativeDosViewport.Divisor/30f:ScreenFraming&&!screenWorld.IsEmpty?screenWorld.Height:PageHeight;}}
  internal RectangleF ScreenPaper{get{return screenPaper;}}
  void UpdateScreenFrame(){
   if(!ScreenFraming||Score==null||Library==null)return;
   uint stamp=2166136261;unchecked{foreach(var item in Score.Items)foreach(byte value in item.Bytes)stamp=(stamp^value)*16777619;foreach(byte value in Score.Trailer)stamp=(stamp^value)*16777619;}
   string library=NativeLibraryLocation.CacheKey(Library.Root)+"|"+NativeCode9Terms.ResourceKey(Library.Root)+"|"+NativeTupletFonts.DirectoryPath+"|"+NativeNotationView.Mode+"|"+NativeBeamDisplay.Appearance+"|"+NativeTupletFonts.ResourceRevision+"|"+NativePreferences.Read(Library.Root).GetHashCode()+"|"+NativeApproximation.Enabled+"|"+(PageDocument==null?"":PageDocument.GuideMeasurement.ToString()+"|"+PageDocument.DatBarlineUnit.ToString("R",System.Globalization.CultureInfo.InvariantCulture));
   if(screenScore!=Score||screenStamp!=stamp||screenLibrary!=library){
    screenScore=Score;screenStamp=stamp;screenLibrary=library;screenInk=RectangleF.Empty;ScreenFrameStatus="";
    var issues=DrawingIssues.ToArray();var approximations=ApproximatedItems.ToArray();bool ghostscript=GhostscriptTextRenderer.Enabled,pixelPass=NativeNotationView.PixelPass;
    try{GhostscriptTextRenderer.Enabled=false;NativeNotationView.PixelPass=true;using(var bitmap=new Bitmap(1,1))using(var graphics=Graphics.FromImage(bitmap))using(NativeItemBounds.Begin(this,true)){Draw(graphics,false);foreach(var box in ItemBounds.Values)screenInk=screenInk.IsEmpty?box:RectangleF.Union(screenInk,box);}if(DrawingIssues.Count>0)ScreenFrameStatus=String.Join("\r\n",DrawingIssues.ToArray());}
    catch(NotSupportedException error){ScreenFrameStatus="Off-page extent unavailable: "+error.Message;}
    catch(OutOfMemoryException error){ScreenFrameStatus="Off-page extent unavailable: "+error.Message;}
    finally{GhostscriptTextRenderer.Enabled=ghostscript;NativeNotationView.PixelPass=pixelPass;DrawingIssues.Clear();DrawingIssues.AddRange(issues);ApproximatedItems.Clear();ApproximatedItems.AddRange(approximations);}
   }
   var size=NativePageFraming.FollowPrint?ScreenPaperSize():new SizeF(8.5f,11);
   float width=size.Width*4000/30,height=size.Height*4000/30;
   screenPaper=new RectangleF(0,PageHeight-height,width,height);screenWorld=screenPaper;if(NativeWorkspaceGuides.PageGap){float padding=NativeWorkspaceGuides.PageGapPoints*4000/(72*30);screenWorld.Inflate(padding,padding);}
   if(!screenInk.IsEmpty){var ink=screenInk;float gap=NativeWorkspaceGuides.Gap?NativeWorkspaceGuides.GapPoints*4000/(72*30):2;ink.Inflate(gap,gap);screenWorld=RectangleF.Union(screenWorld,ink);}
  }
  void PaintScreenPaper(Graphics graphics){if(!ScreenFraming)return;graphics.Clear(Parent==null?BackColor:Parent.BackColor);using(var brush=new SolidBrush(BackColor))graphics.FillRectangle(brush,screenPaper);using(var pen=new Pen(Color.FromArgb(100,100,100),1/RenderScale))graphics.DrawRectangle(pen,screenPaper.X,screenPaper.Y,screenPaper.Width,screenPaper.Height);}
 }
}
