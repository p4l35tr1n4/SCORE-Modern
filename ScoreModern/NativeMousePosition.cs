using System;using System.IO;using System.Drawing;using System.Windows.Forms;using System.Globalization;using System.Linq;
namespace ScoreModern {
 public static class NativeMousePosition {
  static readonly short[] offsets={1,-1,0,0,-1,0,0,-1,2,-1,-1,-1,-1,-1,0,-1,-1,4,3};
  static readonly short[] rests={5,5,7,8,7,4,4,2,2,1,1,1,1};
  static short W(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value<Int32.MinValue||value>Int32.MaxValue)throw new NotSupportedException("Mouse positioning control outside the verified range.");return unchecked((short)(int)value);}
  public static int Preference(string root){
   // Absent MOUSE selects with the middle button in the supplied SCOR4 4.01.
   short fallback=(short)(NativeLibraryLocation.IsScor4(root)?1:0);
   short value=NativePreferences.Read(root).Word("MOUSE",fallback);
   if(value!=0&&value!=1)throw new NotSupportedException("PREF.SCR MOUSE value outside the verified0/1 branch.");return value;
  }
  public static PointF Delta(Item item,NativeNotationContext page,PointF pointer,short divisor,short originX,short originY){
   if(page==null||item.Code<1||item.Code>18||divisor<=0)throw new NotSupportedException("Mouse positioning item/viewport outside the recovered domain.");
   int code=item.Code;short horizontal=0,offset=offsets[code];
   if(code==2){int rest=W(item.P(5));if(rest< -5||rest>7)throw new NotSupportedException("Mouse rest index outside the verified range.");offset=rests[rest+5];}
   if(code==6){if(W(Math.Abs(item.P(7)))>=20)offset=-7;else{offset=6;horizontal=2;}}
   if(code==5){short extension=W(item.P(8));if(extension==-1)horizontal=1;else if(extension==-2)horizontal=3;}
   short xProduct=W((W(pointer.X)-originX)*divisor);float x=(float)((double)xProduct/150.0);x=(float)((double)x-horizontal);float dx=(float)((double)x-item.P(3));
   int dy=0;if(code!=3&&code<17){float old=code==8?0:item.P(4);short oldPitch=W(Math.Round(old,MidpointRounding.AwayFromZero));oldPitch=(short)(oldPitch%100);int verticalProduct=(originY-W(pointer.Y)-1)*divisor;float raw=(float)verticalProduct;short pitch=W(Math.Round(((double)raw-page.RawBase)/((double)page.Scale*7),MidpointRounding.AwayFromZero));dy=unchecked((short)(pitch-offset-oldPitch));}
   return new PointF(dx,dy);
  }
 }
 public sealed partial class Editor {
  int mouseAxis;
  bool preserveMouseView;
  void SelectByMouse(Item item){
   if(item==null||item==canvas.Selected)return;
   Point scroll=viewport.AutoScrollPosition;
   preserveMouseView=true;
   try{
    if(mode==ScreenMode.Edit&&canvas.Selected!=null)ExitToItem(score.Items.IndexOf(item));
    else BeginEdit(item);
   }finally{
    viewport.AutoScrollPosition=new Point(-scroll.X,-scroll.Y);
    preserveMouseView=false;
   }
  }
  void ScoreMouse(int buttons,Keys modifiers,PointF position,PointF start,bool released){Safe(()=>{
   if(externalDrawingNameItem!=null||letterHelpPages!=null||drawExportStage!=DrawExportStage.None||pendingParameterExport||pendingDriveChange||pendingDiskDelete!=null||pendingDiskFilename!=null||pendingLetterSave!=null||pendingLetterLoad!=null){if(released)command.Focus();return;}
   if(inlineTextActive){if(released)inlineText.Focus();return;}
   if(Scor4ScreenDrag(buttons,position,start,released))return;
   if(musicInput!=null&&(modifiers&Keys.Alt)==0){MusicInputMouse(buttons,modifiers,position,start,released);return;}
   if(score==null||originalItemZero)return;bool edit=mode==ScreenMode.Edit&&canvas.Selected!=null;int effective=(modifiers&Keys.Alt)!=0?buttons:(modifiers&Keys.Control)!=0?4:buttons;
   if((modifiers&Keys.Alt)!=0){canvas.MouseOperationLocked=!released;if(released){if(effective==7){if(musicInput==null||NativeMousePosition.Preference(source)==1)MouseNavigation(effective,position,start);}else MouseZoom(effective,position,start);if(musicInput!=null)command.Focus();}return;}
   if(edit&&effective==1&&modifiers==Keys.None&&!NativeMouseSelector.OriginalUnboundedSelection&&!canvas.MouseOperationLocked){
    Size drag=SystemInformation.DragSize;
    if(Math.Abs(position.X-start.X)<drag.Width/2f&&Math.Abs(position.Y-start.Y)<drag.Height/2f){
     if(released)SelectByMouse(canvas.HitTest(position));
     return;
    }
   }
   if(edit&&(effective==1||effective==2)){canvas.MouseOperationLocked=!released;
    var point=NativeMouseSelector.Pointer(canvas,position);var initial=NativeMouseSelector.Pointer(canvas,start);
    if((modifiers&Keys.Shift)!=0&&mouseAxis==0){float x=Math.Abs(point.X-initial.X),y=Math.Abs(point.Y-initial.Y);if(x<2&&y<2){if(released){mouseAxis=0;}return;}mouseAxis=y>x?2:1;}
    var item=canvas.Selected;var context=canvas.PageDocument.Contexts[item];short divisor=NativeDosViewport.Divisor;short ox=NativeDosViewport.OriginX,oy=NativeDosViewport.OriginY;
    var delta=NativeMousePosition.Delta(item,context,point,divisor,ox,oy);if(mouseAxis==1)delta.Y=0;if(mouseAxis==2)delta.X=0;
    if(delta.X!=0||delta.Y!=0){MoveSelected(delta.X,delta.Y);}
    if(released){if(effective==2){int target=canvas.EditModified?score.Items.Count:score.Items.Count-1;CopyEditedItem(CopyContinuation.Item,target);}else{
     // Mouse positioning writes the working record and clears its pending flag,
     // but retains the entry backup: original F8 still undoes the released move.
     if(canvas.EditModified)canvas.Selected.TrimEditedNumericParameters();if(editOriginal!=null)editOriginal.TrimEditedNumericParameters();canvas.EditModified=false;direction=1;RefreshAll(canvas.Selected);LayoutClassic();NativeDosViewport.Advance(canvas);UpdateClassic();
    }mouseAxis=0;}return;
   }
   if(!released)return;mouseAxis=0;
   if((mode==ScreenMode.EditMenu||mode==ScreenMode.Disk)&&effective>=5){
    // Original13EE:03DC..0420: 5/6 -> Escape, 7 -> F8.
    if(effective==7&&!(mode==ScreenMode.EditMenu&&NativeMenuMouseOptions.HelpExit)){Function(8);return;}
    if(pendingFilteredEdit!=null){FilteredEditKey(new KeyEventArgs(Keys.Escape));return;}
    pendingFilteredEdit=null;selectionPending=false;mode=ScreenMode.Main;command.Clear();Message("");UpdateClassic();command.Focus();return;
   }
   if(pendingFilteredEdit!=null){string arguments=NativeFilteredMouse.Arguments(canvas,NativeMouseSelector.Pointer(canvas,position),pendingFilteredEdit=="ST");SubmitFilteredEdit(arguments);return;}
   if(edit&&(effective==5||effective==6)){ExitEdit(false);mode=ScreenMode.Main;UpdateClassic();return;}if(edit&&effective==7){
    // Measured AB: a pending edit restores the original record byte count;
    // the unmodified route writes back a trimmed numeric record instead.
    bool pending=canvas.EditModified;AbortCurrent();if(!pending&&canvas.Selected!=null)canvas.Selected.TrimEditedNumericParameters();return;
   }
   if(effective==1||effective==2||effective==3||effective==4&&(edit||NativeMousePosition.Preference(source)==1)){
    SelectByMouse(canvas.HitTest(position));return;
   }
   MouseNavigation(effective,position,start);
  });}
  void ApplyScrollableCamera(PointF wanted){
   // Full-page Windows scrolling cannot expose negative child coordinates.
   // Keep the recovered SCORE camera transform, represented by a positive
   // canvas offset plus the equivalent Windows scroll position.
   double scale=canvas.RenderScale;var scroll=new Point((int)Math.Ceiling(Math.Max(0,-wanted.X*scale)),(int)Math.Ceiling(Math.Max(0,-wanted.Y*scale)));
   canvas.CameraOffset=new PointF(Math.Max(0,wanted.X),Math.Max(0,wanted.Y));viewport.AutoScrollPosition=Point.Empty;viewport.AutoScrollMinSize=Size.Empty;canvas.LayoutScore();
   viewport.AutoScrollMinSize=new Size(Math.Max(canvas.Width,scroll.X+viewport.ClientSize.Width),Math.Max(canvas.Height,scroll.Y+viewport.ClientSize.Height));viewport.AutoScrollPosition=scroll;
  }
  void ApplyMouseCamera(Action operation){short oldDivisor=NativeDosViewport.Divisor,oldX=NativeDosViewport.OriginX,oldY=NativeDosViewport.OriginY;float oldScale=canvas.RenderScale;Point oldScroll=viewport.AutoScrollPosition;PointF oldCamera=canvas.CameraOffset;bool previousLiteralZoom=canvas.Scor4LiteralZoom;canvas.Scor4LiteralZoom=false;try{operation();}catch{canvas.Scor4LiteralZoom=previousLiteralZoom;throw;}canvas.HasZoomCommand=true;if(!NativeNotationView.IsDosViewport&&NativeZoomFraming.Dos){fitting=false;canvas.OriginalZoomFrame=true;viewport.AutoScrollMinSize=Size.Empty;viewport.AutoScrollPosition=Point.Empty;canvas.LayoutScore();}else if(!NativeNotationView.IsDosViewport){canvas.OriginalZoomFrame=false;double ratio=(double)oldDivisor/NativeDosViewport.Divisor;fitting=false;canvas.Zoom=(float)Math.Max(.1,Math.Min(16,canvas.Zoom*ratio));double scale=canvas.RenderScale,q=scale/oldScale,k=scale*NativeDosViewport.Divisor/30;double x=q*(oldScale*oldCamera.X+oldScroll.X)+k*(NativeDosViewport.OriginX-ratio*oldX),y=q*(oldScale*oldCamera.Y+oldScroll.Y)+k*((NativeDosViewport.OriginY-97)-ratio*(oldY-97));ApplyScrollableCamera(new PointF((float)(x/scale),(float)(y/scale)));}else canvas.LayoutScore();NativeDosViewport.Advance(canvas);UpdateClassic();}

  void MouseZoom(int buttons,PointF position,PointF start){var first=NativeMouseSelector.Pointer(canvas,start);var last=NativeMouseSelector.Pointer(canvas,position);float width=Math.Abs(last.X-first.X),height=Math.Abs(last.Y-first.Y);short divisor=NativeDosViewport.Divisor,ox=NativeDosViewport.OriginX,oy=NativeDosViewport.OriginY;float factor=NativeDosViewport.ZoomFactor;
   short x1=unchecked((short)(((short)first.X-ox)*divisor));int y1=(oy-(short)first.Y-1)*divisor;short anchorX=x1,anchorY=unchecked((short)y1);
   if(width>=2&&height>=2){float horizontal=(float)(637.0/width),vertical=(float)(364.0/height);factor=(float)((double)Math.Min(horizontal,vertical)*factor);short x2=unchecked((short)(((short)last.X-ox)*divisor));int y2=(oy-(short)last.Y-1)*divisor;anchorX=unchecked((short)((x1+x2)/2));anchorY=unchecked((short)((y1+y2)/2));}
   else{float multiplier=(buttons==2||buttons==6)?(factor>1?.5f:.8f):(factor>=1?2f:1.25f);factor=(float)((double)multiplier*factor);}
   ApplyMouseCamera(()=>NativeDosViewport.CenterMouse(canvas,factor,anchorX,anchorY));
  }
  void MouseNavigation(int buttons,PointF position,PointF start){int preference=NativeMousePosition.Preference(source);bool pan=buttons==4&&preference==0||buttons==7&&preference==1;if(pan){var point=NativeMouseSelector.Pointer(canvas,position);int third=(canvas.DosClipBottom-canvas.DosClipTop)/3,lower=canvas.DosClipTop+third,upper=canvas.DosClipTop+2*third;string direction;float distance,denominator;if(point.X>405){direction="ZR";distance=point.X-405;denominator=212;}else if(point.X<233){direction="ZL";distance=233-point.X;denominator=212;}else if(point.Y>upper){direction="ZD";distance=point.Y-upper;denominator=240;}else if(point.Y<lower){direction="ZU";distance=lower-point.Y;denominator=240;}else return;float percent=(float)((double)distance*40/denominator);ApplyMouseCamera(()=>NativeDosViewport.Pan(direction,percent,scor4Interface));return;}
   if(buttons==5||buttons==6){MouseZoom(buttons,position,start);return;}if(buttons==7)ApplyMouseCamera(()=>NativeDosViewport.Center(canvas,1));
  }
 }
}
