using System;using System.Drawing;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeZoomFraming {
  static bool fullPage=NativeSettings.Read("zoom-framing")=="full-page";
  static bool helpDefaults=NativeSettings.Read("zoom-command-defaults")=="toms";
  public static bool HelpDefaults{get{return !NativeScoreBehaviour.Enabled&&helpDefaults;}}
  internal static void SetDefinitions(bool help,bool save){helpDefaults=help;if(save)NativeSettings.WriteAllText("zoom-command-defaults",help?"toms":"executable");}
  public static bool Dos{get{return NativeScoreBehaviour.Enabled||!fullPage;}}
  internal static void Set(bool dos,bool save){fullPage=!dos;if(save)NativeSettings.WriteAllText("zoom-framing",dos?"dos":"full-page");foreach(Form form in Application.OpenForms){var editor=form as Editor;if(editor!=null)editor.RefreshZoomFraming();}}
  internal static void AddMenu(ContextMenuStrip menu){var dos=new ToolStripMenuItem("Z/ZZ framing: original DOS viewport (hi-res supported)");var page=new ToolStripMenuItem("Z/ZZ framing: full page with SCORE zoom/pan amounts");dos.Click+=(s,e)=>Set(true,true);page.Click+=(s,e)=>Set(false,true);menu.Items.Add(dos);menu.Items.Add(page);var measured=new ToolStripMenuItem("Z/ZZ defaults: executable (Z retains zoom, ZZ uses 2)");var help=new ToolStripMenuItem("Z/ZZ defaults: Tom's Help (Z resets to 1, ZZ uses 3)");measured.Click+=(s,e)=>SetDefinitions(false,true);help.Click+=(s,e)=>SetDefinitions(true,true);menu.Items.Add(measured);menu.Items.Add(help);menu.Opening+=(s,e)=>{dos.Checked=Dos;page.Checked=!Dos;page.Enabled=!NativeScoreBehaviour.Enabled;measured.Checked=!HelpDefaults;help.Checked=HelpDefaults;help.Enabled=!NativeScoreBehaviour.Enabled;};}
 }
 public sealed partial class Canvas {
  internal bool OriginalZoomFrame,HasZoomCommand,Scor4Interface,Scor4LiteralZoom;
  internal bool UsesOriginalZoomFrame{get{return OriginalZoomFrame&&!NativeNotationView.IsDosViewport;}}
  internal void LayoutOriginalZoomFrame(){if(!UsesOriginalZoomFrame||PageDocument==null||Parent==null)return;float fit=Math.Min(Parent.ClientSize.Width/640f,Parent.ClientSize.Height/365f);if(fit<=0)return;Zoom=fit*30/(NativeDosViewport.Divisor*DisplayDpi);CropTop=0;CameraOffset=new PointF(NativeDosViewport.OriginX*NativeDosViewport.Divisor/30f-PageDocument.Projection.OffsetX,(NativeDosViewport.OriginY-97)*NativeDosViewport.Divisor/30f-PageDocument.Projection.OffsetY);}
 }
 public sealed partial class Editor {
  internal void RefreshZoomFraming(){if(canvas.HasZoomCommand){canvas.OriginalZoomFrame=NativeZoomFraming.Dos;if(canvas.OriginalZoomFrame){viewport.AutoScrollMinSize=Size.Empty;viewport.AutoScrollPosition=Point.Empty;canvas.LayoutScore();}else if(!NativeNotationView.IsDosViewport)ApplyScrollableCamera(canvas.CameraOffset);else canvas.LayoutScore();}UpdateClassic();}
  bool ExecuteCrossViewZoom(string verb,string rest){
   bool pan=verb=="ZL"||verb=="ZR"||verb=="ZU"||verb=="ZD";
   if(NativeNotationView.IsDosViewport||!(verb=="Z"||verb=="ZZ"||pan))return false;
   if(mode==ScreenMode.Edit&&canvas.EditModified){Message("Viewport command ignored while item parameters are changed; commit or abort first.");return true;}
   bool retainScor4Frame=pan&&canvas.Scor4LiteralZoom;
   ApplyMouseCamera(()=>{
    if(!NativeDosViewport.CenteringCommand(canvas,verb,rest))NativeDosViewport.Command(canvas,verb,rest);
    if(pan){
     canvas.Scor4LiteralZoom=retainScor4Frame;
     NativeDosViewport.RevealActive(canvas);
    }
   });
   return true;
  }
 }
}
