using System;using System.Drawing;using System.Collections.Generic;using System.IO;using System.Linq;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeMouseSelector {
  // Original measured INT3F index38 entry0004, runtime2C97:0004..0257.
  // This is the selection anchor, not the independently recovered edit cursor.
  static readonly short[] offsets={1,-1,0,0,-1,0,0,-1,2,-1,-1,-1,-1,-1,0,-1,-1,4,3};
  static readonly short[] rests={5,5,7,8,7,4,4,2,2,1,1,1,1};
  static long Integer(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value< -9223372036854775808.0||value>=9223372036854775808.0)throw new NotSupportedException("Mouse coordinate outside the recovered finite conversion range.");return (long)value;}
  static short Word(double value){return unchecked((short)Integer(value));}
  static PointF Continuous(Item item,NativeNotationContext page,short dx,short dy,short originX,short originY,bool original){
   if(page==null||dx<=0||dy<=0||item.Code<1||item.Code>18)throw new NotSupportedException("Mouse selector item or viewport outside the recovered domain.");
   int code=item.Code;float pitch=code==8?0:(float)((double)item.P(4)%100.0);short offset=offsets[code],horizontal=0;
   if(code==14){pitch=3;offset=0;}
   else if(code==6){if(Word(Math.Abs(item.P(7)))>=20)offset=-8;else{offset=6;horizontal=2;}}
   else if(code==5){short extension=Word(item.P(8));if(extension==-1)horizontal=1;else if(extension==-2)horizontal=3;}
   else if(code==2){short kind=Word(item.P(5));if(kind< -5||kind>7)throw new NotSupportedException("Mouse rest-table index outside the verified domain.");offset=rests[kind+5];}
   pitch=(float)((double)pitch+offset);float position=(float)((double)item.P(3)+horizontal);
   // Original helper returns the low signed32 bits before signed32/16 divide.
   double verticalRaw=(double)page.Scale*pitch*7.0+page.RawBase;double x=(double)position*150.0/dx+originX;
   if(!original)return new PointF((float)x,(float)(originY-verticalRaw/dy));
   int vertical=unchecked((int)Integer(verticalRaw));return new PointF(Word(x),unchecked((short)(originY-vertical/dy)));
  }
  public static Point Anchor(Item item,NativeNotationContext page,short dx,short dy,short originX,short originY){var p=Continuous(item,page,dx,dy,originX,originY,true);return new Point((int)p.X,(int)p.Y);}
  public static int Choose(IList<Point> anchors,Point mouse,int activeIndex){
   short best=0x7ef4;int selected=anchors.Count==0?-1:0;
   for(int n=0;n<anchors.Count;n++){short x=unchecked((short)(anchors[n].X-mouse.X)),y=unchecked((short)(anchors[n].Y-mouse.Y));short ax=unchecked((short)(x<0?-x:x)),ay=unchecked((short)(y<0?-y:y));short distance=unchecked((short)(ax+ay));if(distance<best&&n!=activeIndex){best=distance;selected=n;}}
   return selected;
  }
  static bool configuredHighResolution;public static bool HighResolution{get{return NativeScoreBehaviour.Enabled?false:configuredHighResolution;}private set{configuredHighResolution=value;}}static bool configuredShowPoints;public static bool ShowPoints{get{return NativeScoreBehaviour.Enabled?false:configuredShowPoints;}private set{configuredShowPoints=value;}}
  static bool configuredOriginalUnboundedSelection;public static bool OriginalUnboundedSelection{get{return NativeScoreBehaviour.Enabled?true:configuredOriginalUnboundedSelection;}private set{configuredOriginalUnboundedSelection=value;}}
  static bool configuredOutlinePoints;public static bool OutlinePoints{get{return NativeScoreBehaviour.Enabled?false:configuredOutlinePoints;}private set{configuredOutlinePoints=value;}}
  static bool configuredLargePointNumbers;public static bool LargePointNumbers{get{return NativeScoreBehaviour.Enabled?false:configuredLargePointNumbers;}private set{configuredLargePointNumbers=value;}}
  static string Settings{get{return "mouse";}}
  public static void Load(){HighResolution=false;ShowPoints=false;OriginalUnboundedSelection=false;OutlinePoints=false;LargePointNumbers=false;try{if(NativeSettings.Exists(Settings)){var lines=NativeSettings.ReadAllLines(Settings);HighResolution=lines.Contains("high-resolution");ShowPoints=lines.Contains("show-points");OriginalUnboundedSelection=lines.Contains("original-unbounded");OutlinePoints=false;LargePointNumbers=lines.Contains("large-point-numbers");}}catch(IOException){}}
  public static void Set(bool highResolution,bool showPoints,bool save){HighResolution=highResolution;ShowPoints=showPoints;if(save)Save();foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  static void Save(){
   NativeSettings.WriteAllText(Settings,(HighResolution?"high-resolution":"original-grid")+"\n"+
    (ShowPoints?"show-points":"hide-points")+"\n"+(OriginalUnboundedSelection?"original-unbounded":"bounded")+"\n"+
    (OutlinePoints?"outline-points":"cross-points")+"\n"+(LargePointNumbers?"large-point-numbers":"normal-point-numbers"));
  }
  public static void AddMenu(ContextMenuStrip menu){
   var original=new ToolStripMenuItem("Mouse selection: original DOS grid");
   var high=new ToolStripMenuItem("Mouse selection: high-resolution distances");
   var points=new ToolStripMenuItem("Mouse click-points: show on item hover");
   
   var numbers=new ToolStripMenuItem("Mouse click-points: larger item numbers");
   numbers.Click+=(sender,args)=>{
    LargePointNumbers=!LargePointNumbers;
    Save();
    foreach(Form form in Application.OpenForms)form.Invalidate(true);
   };
   menu.Items.Add(numbers);
   menu.Opening+=(sender,args)=>numbers.Checked=LargePointNumbers;
   var unlimited=new ToolStripMenuItem("Mouse selection: original unrestricted clicks");
   original.Click+=(s,e)=>Set(false,ShowPoints,true);
   high.Click+=(s,e)=>Set(true,ShowPoints,true);
   points.Click+=(s,e)=>Set(HighResolution,!ShowPoints,true);
   unlimited.Click+=(s,e)=>{OriginalUnboundedSelection=!OriginalUnboundedSelection;Set(HighResolution,ShowPoints,true);};
   menu.Items.AddRange(new ToolStripItem[]{original,high,points,unlimited});
   menu.Opening+=(s,e)=>{
    original.Checked=!HighResolution;
    high.Checked=HighResolution;
    points.Checked=ShowPoints;
    unlimited.Checked=OriginalUnboundedSelection;
   };
  }
  static PointF Pixel(Canvas canvas,PointF point){point=new PointF(point.X/canvas.RenderScale,(point.Y+canvas.CropTop)/canvas.RenderScale);if(NativeNotationView.IsDosViewport)return new PointF(Math.Max(1,Math.Min(638,point.X)),Math.Max(canvas.DosClipTop,Math.Min(479,point.Y+97)));point=new PointF(point.X-canvas.CameraOffset.X-canvas.ScreenOrigin.X,point.Y-canvas.CameraOffset.Y-canvas.ScreenOrigin.Y);var projection=canvas.PageDocument.Projection;var pixel=new PointF((point.X-projection.OffsetX)*30/NativeDosViewport.Divisor+NativeDosViewport.OriginX,NativeDosViewport.OriginY-(projection.OffsetY-point.Y)*30/NativeDosViewport.Divisor);return canvas.UsesOriginalZoomFrame?new PointF(Math.Max(1,Math.Min(638,pixel.X)),Math.Max(97,Math.Min(479,pixel.Y))):pixel;}
  public static PointF Pointer(Canvas canvas,PointF screen){var point=Pixel(canvas,screen);return new PointF(Word(Math.Round(point.X,MidpointRounding.AwayFromZero)),Word(Math.Round(point.Y,MidpointRounding.AwayFromZero)));}
  static PointF ItemPixel(Canvas canvas,Item item){NativeNotationContext context;if(!canvas.PageDocument.Contexts.TryGetValue(item,out context))throw new NotSupportedException("Mouse item has no recovered staff context.");return Continuous(item,context,NativeDosViewport.Divisor,NativeDosViewport.Divisor,NativeDosViewport.OriginX,NativeDosViewport.OriginY,!HighResolution);}
  public static PointF DisplayPoint(Canvas canvas,Item item){var point=ItemPixel(canvas,item);if(NativeNotationView.IsDosViewport)return new PointF(point.X,point.Y-97);return canvas.PageDocument.Projection.Point((point.X-NativeDosViewport.OriginX)*NativeDosViewport.Divisor,(NativeDosViewport.OriginY-point.Y)*NativeDosViewport.Divisor);}
  static PointF DisplayCoordinates(Canvas canvas,PointF screen){
   var point=new PointF(screen.X/canvas.RenderScale,(screen.Y+canvas.CropTop)/canvas.RenderScale);
   if(!NativeNotationView.IsDosViewport)point=new PointF(point.X-canvas.CameraOffset.X-canvas.ScreenOrigin.X,point.Y-canvas.CameraOffset.Y-canvas.ScreenOrigin.Y);
   return point;
  }
  static float TargetRadius(Canvas canvas){return Math.Max(6,8*canvas.DisplayDpi/canvas.RenderScale);}
  static Item BoundedHit(Canvas canvas,PointF screen){
   var mouse=DisplayCoordinates(canvas,screen);
   float radius=TargetRadius(canvas),best=Single.PositiveInfinity;
   Item selected=null;
   foreach(var item in canvas.Score.Items){
    if(item.Code==13||item.Code==2&&item.P(6)==-1||item.Code==14&&item.P(4)<0&&!NativeNotationView.UsesDat)continue;
    PointF anchor;
    try{anchor=DisplayPoint(canvas,item);}catch(NotSupportedException){continue;}
    float dx=Math.Abs(anchor.X-mouse.X),dy=Math.Abs(anchor.Y-mouse.Y);
    if(dx>radius||dy>radius||dx+dy>=best)continue;
    best=dx+dy;
    selected=item;
   }
   return selected;
  }
  public static Item HitTest(Canvas canvas,PointF screen){if(canvas.Score==null||canvas.PageDocument==null||canvas.Score.Items.Count==0)return null;if(!OriginalUnboundedSelection)return BoundedHit(canvas,screen);var mouse=Pixel(canvas,screen);int active=canvas.Score.Items.IndexOf(canvas.Selected);int selected;
   if(!HighResolution){var anchors=canvas.Score.Items.Select(i=>{var p=ItemPixel(canvas,i);return new Point((int)p.X,(int)p.Y);}).ToList();selected=Choose(anchors,new Point(Word(Math.Round(mouse.X,MidpointRounding.AwayFromZero)),Word(Math.Round(mouse.Y,MidpointRounding.AwayFromZero))),active);}
   else{double best=0x7ef4;selected=0;for(int n=0;n<canvas.Score.Items.Count;n++){var p=ItemPixel(canvas,canvas.Score.Items[n]);double distance=Math.Abs(p.X-mouse.X)+Math.Abs(p.Y-mouse.Y);if(n!=active&&distance<best){best=distance;selected=n;}}}
   return selected<0?null:canvas.Score.Items[selected];
  }
  internal static bool HoverChanged(Canvas canvas,PointF? before,PointF? after){
   if(!ShowPoints||before==after||canvas.PageDocument==null)return false;
   PointF? old=before.HasValue&&canvas.ClientRectangle.Contains(Point.Round(before.Value))?(PointF?)DisplayCoordinates(canvas,before.Value):null;
   PointF? next=after.HasValue&&canvas.ClientRectangle.Contains(Point.Round(after.Value))?(PointF?)DisplayCoordinates(canvas,after.Value):null;
   foreach(var bounds in canvas.ItemBounds.Values)
    if((old.HasValue&&bounds.Contains(old.Value))!=(next.HasValue&&bounds.Contains(next.Value)))return true;
   return false;
  }
  public static void DrawPoints(Canvas canvas,Graphics g){if(!ShowPoints||canvas.Score==null||canvas.PageDocument==null||!canvas.HoverPoint.HasValue)return;var hover=canvas.HoverPoint.Value;if(!canvas.ClientRectangle.Contains(Point.Round(hover)))return;hover=DisplayCoordinates(canvas,hover);float unit=canvas.DisplayDpi/canvas.RenderScale;using(var pen=new Pen(Color.DarkRed,unit))using(var font=new Font("Consolas",(LargePointNumbers?16:10)*unit,GraphicsUnit.Pixel)){for(int n=0;n<canvas.Score.Items.Count;n++){RectangleF bounds;if(!canvas.ItemBounds.TryGetValue(canvas.Score.Items[n],out bounds)||!bounds.Contains(hover))continue;PointF p;try{p=DisplayPoint(canvas,canvas.Score.Items[n]);}catch(NotSupportedException error){string issue="Mouse click-point unavailable for item "+(n+1)+" (Code "+canvas.Score.Items[n].Code+"): "+error.Message;if(!canvas.DrawingIssues.Contains(issue))canvas.DrawingIssues.Add(issue);continue;}if(OutlinePoints&&!OriginalUnboundedSelection){float radius=TargetRadius(canvas);g.DrawRectangle(pen,p.X-radius,p.Y-radius,2*radius,2*radius);}else{g.DrawLine(pen,p.X-4*unit,p.Y,p.X+4*unit,p.Y);g.DrawLine(pen,p.X,p.Y-4*unit,p.X,p.Y+4*unit);}g.DrawString((n+1).ToString(),font,Brushes.DarkRed,p.X+(OutlinePoints&&!OriginalUnboundedSelection?TargetRadius(canvas)+2*unit:5*unit),p.Y+2*unit);}}}

 }
}
