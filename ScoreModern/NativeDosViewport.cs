using System;using System.Globalization;using System.IO;using System.Drawing;using System.Drawing.Drawing2D;using System.Drawing.Imaging;using System.Collections.Generic;using System.Runtime.InteropServices;using System.Windows.Forms;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDosViewport {
  public static short Divisor=50,OriginX=20,OriginY=-47;
  static long frameRevision;static Item lastActive;static short retainedAnchor;
  static Point? pendingEmptyZoom;
  public static void Remember(Item item){if(item!=null)lastActive=item;}
  public static float ZoomFactor=1;static float horizontalPanPercent=10,verticalPanPercent=10;
  // SCORE's mode-17 descriptor supplies 50 for both coordinate divisors.
  // Keep its single-precision constants and stored default percentages before
  // truncating the extended expression, as at original 1449:0489/04AD/05B4.
  public static void ZoomTo(float factor,float xPercent,float yPercent){ZoomTo(factor,xPercent,yPercent,false);}
  internal static void ZoomToScor4Fit(float factor){ZoomTo(factor,0,0,true);}
  static void ZoomTo(float factor,float xPercent,float yPercent,bool scor4){
   if(float.IsNaN(factor)||float.IsInfinity(factor)||factor<0||factor>48)throw new InvalidDataException("DOS viewport zoom must be between 0 and 48. Packed zoom controls are not yet verified.");
   if(float.IsNaN(xPercent)||float.IsInfinity(xPercent)||float.IsNaN(yPercent)||float.IsInfinity(yPercent))throw new InvalidDataException("Enter finite viewport percentages.");
   if(factor==0)factor=1;
   Point origin=ZoomOrigin(factor,xPercent,yPercent,scor4);
   short divisor=(short)(int)(50.0/factor);if(divisor<=0)throw new InvalidDataException("DOS viewport divisor is outside the recovered positive range.");
   OriginX=(short)origin.X;OriginY=(short)origin.Y;Divisor=divisor;ZoomFactor=factor;Refresh();
  }
  static Point ZoomOrigin(float factor,float xPercent,float yPercent,bool scor4=false){
   if(factor>1){if(xPercent==0)xPercent=(float)(50.0/((double)factor+.75));if(yPercent==0)yPercent=(float)(50.0/((double)factor+1.5));}
   // SCORE 4.01 stores binary32 4.65 at CA0E with the two menu rows active.
   double width=6.4f,height=scor4?4.65f:4.8f;
   short x=20,y=unchecked((short)(int)(height*90-480.0*factor));
   if(yPercent!=0)y=unchecked((short)(int)(((double)yPercent*height-480.0)*factor+height*45));
   if(xPercent!=0)x=unchecked((short)(int)(width*50-(double)factor*xPercent*width));
   return new Point(x,y);
  }
  public static void Pan(string direction,float percent){Pan(direction,percent,false);}
  internal static void Pan(string direction,float percent,bool scor4){
   if(float.IsNaN(percent)||float.IsInfinity(percent))throw new InvalidDataException("Enter a finite pan percentage.");
   bool horizontal=direction=="ZL"||direction=="ZR";
   if(percent==0)percent=horizontal?horizontalPanPercent:verticalPanPercent;else if(horizontal)horizontalPanPercent=percent;else verticalPanPercent=percent;
   double signed=(direction=="ZL"||direction=="ZU")?-(double)percent:percent;
   short delta=unchecked((short)(int)(signed*(horizontal?(double)6.4f:scor4?(double)4.65f:(double)4.8f)));
   if(horizontal)OriginX=unchecked((short)(OriginX+delta));else OriginY=unchecked((short)(OriginY+delta));Refresh();
  }
  public static void CenterMouse(Canvas canvas,float factor,short x,short y){
   if(float.IsNaN(factor)||float.IsInfinity(factor)||factor<=0)throw new InvalidDataException("Mouse zoom size must be finite and positive.");factor=Math.Min(32,factor);short divisor=(short)(int)(50.0/factor);if(divisor<=0)throw new InvalidDataException("Mouse zoom divisor is outside the recovered range.");OriginX=factor>1?unchecked((short)(213-x/divisor)):(short)20;retainedAnchor=(short)MouseCenterY(canvas);OriginY=unchecked((short)(retainedAnchor+y/divisor));Divisor=divisor;ZoomFactor=factor;
   if(canvas.Selected!=null){var context=canvas.PageDocument.Contexts[canvas.Selected];var point=NativeEditCursorPosition.Get(canvas.Selected,context,divisor,divisor,OriginX,OriginY);KeepEditedCursorVisible(canvas,(short)point.X,(short)point.Y);}Refresh();
  }
  // 1449:02BC..02D9 derives the centre from the current graphics bounds.
  static int MouseCenterY(Canvas canvas){return (int)((double)(canvas.DosClipBottom-canvas.DosClipTop)/1.5+canvas.DosClipTop);}
  public static void Center(Canvas canvas,float factor){
   if(float.IsNaN(factor)||float.IsInfinity(factor)||factor<0)throw new InvalidDataException("Centering requires a nonnegative finite zoom.");
   if(factor==0)factor=2;
   factor=Math.Min(32,factor);short divisor=(short)(int)(50.0/factor);if(divisor<=0)throw new InvalidDataException("Centering divisor is outside the recovered positive range.");
   if(canvas.Score==null)return;
   if(canvas.Score.Items.Count==0){
    if(canvas.RetainedRestartPoint.HasValue){
     Point point=canvas.RetainedRestartPoint.Value;
     OriginX=factor>1?unchecked((short)(213-point.X/divisor)):(short)20;
     retainedAnchor=(short)MouseCenterY(canvas);
     OriginY=unchecked((short)(retainedAnchor+point.Y/divisor));
     Divisor=divisor;ZoomFactor=factor;Refresh();
    }
    return;
   }
   NativeEditPositionGuide.Set(canvas,null);NativeDosFrameCache.HideCursor(canvas);canvas.LayoutScore();Item active=canvas.Selected??(lastActive!=null&&canvas.Score.Items.Contains(lastActive)?lastActive:canvas.Score.Items[canvas.Score.Items.Count-1]);
   var retained=canvas.Selected==null&&(canvas.DisappearingStaffs.Count!=0||canvas.TextLayout.ShowActualDatFonts)
    ?canvas.RetainedDisplayCenter:null;
   if(retained.HasValue){
    Point point=retained.Value;
    OriginX=factor>1?unchecked((short)(213-point.X/divisor)):(short)20;
    retainedAnchor=(short)MouseCenterY(canvas);
    OriginY=unchecked((short)(retainedAnchor+point.Y/divisor));
    Divisor=divisor;ZoomFactor=factor;Refresh();
    return;
   }
   var dat=NativeDatDrawing.Get(canvas.Library);short font=0;PrimitiveTextPoint? first=null;bool awaitingNext=false;
   foreach(var item in canvas.Score.Items){short next;var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,NativeDatDrawing.HeadCache,font,out next);font=next;
    if(item==active||awaitingNext){if(item==active&&item.Code==8&&item.Staff==1&&item.P(8)!=0)points=NativeScanStructuralVectors.ScreenGuide(item,canvas.PageDocument);if(points.Count>0){first=points[0];awaitingNext=false;}else awaitingNext=true;}
   }
   if(!first.HasValue)throw new NotSupportedException("Centering past the final vector requires retained-buffer verification.");
   short x=NativeNotationContext.Word(first.Value.X,true),y=NativeNotationContext.Word(first.Value.Y,false);
   OriginX=factor>1?unchecked((short)(213-x/divisor)):(short)20;retainedAnchor=(short)MouseCenterY(canvas);OriginY=unchecked((short)(retainedAnchor+y/divisor));Divisor=divisor;ZoomFactor=factor;
   if(canvas.Selected!=null){var context=canvas.PageDocument.Contexts[active];var point=NativeEditCursorPosition.Get(active,context,divisor,divisor,OriginX,OriginY);short cursorX=(short)point.X,cursorY=(short)point.Y;
    KeepEditedCursorVisible(canvas,cursorX,cursorY);
   }Refresh();
  }
  // Original1449:012C..019D chooses the largest ordinary buffered Y;
  // 02BC..032F centers that point, then subtracts one quarter of bottom.
  public static void Top(Canvas canvas){
   if(canvas.Scor4Interface){
    short? origin=Scor4TopOrigin(canvas);
    if(!origin.HasValue)return;
    NativeEditPositionGuide.Set(canvas,null);NativeDosFrameCache.HideCursor(canvas);
    retainedAnchor=(short)Scor4TopAnchor;OriginX=20;OriginY=origin.Value;Divisor=50;ZoomFactor=1;
    canvas.Scor4LiteralZoom=true;Refresh();return;
   }
   short? highest=HighestBufferedY(canvas);
   if(!highest.HasValue)return;NativeEditPositionGuide.Set(canvas,null);NativeDosFrameCache.HideCursor(canvas);OriginX=20;retainedAnchor=(short)MouseCenterY(canvas);OriginY=unchecked((short)(retainedAnchor+highest.Value/50-canvas.DosClipBottom/4));Divisor=50;ZoomFactor=1;Refresh();
  }
  // 25C9:0340–03B1 in the original SCORE 4 two-row graphics frame.
  static int Scor4TopAnchor{get{return (int)((446-97)/1.5+97);}}
  internal static short? Scor4TopOrigin(Canvas canvas){
   short? highest=HighestBufferedY(canvas);
   return highest.HasValue?(short?)(Scor4TopAnchor+highest.Value/50-446/3):null;
  }
  internal static short? HighestBufferedY(Canvas canvas){
   if(canvas==null||canvas.Score==null||canvas.Score.Items.Count==0)return null;
   canvas.LayoutScore();var dat=NativeDatDrawing.Get(canvas.Library);var cache=new NativeDatHeadCache();short font=0,highest=-32700;bool found=false;
   foreach(var item in canvas.Score.Items){if(canvas.DisplayHidden(item))continue;short next;var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,cache,font,out next);font=next;
    if(item.Code==8&&item.Staff==1)points.InsertRange(0,NativeScanStructuralVectors.ScreenGuide(item,canvas.PageDocument));
    foreach(var point in points){if(point.CommandOnly)continue;short x=NativeNotationContext.Word(point.X,true),y=NativeNotationContext.Word(point.Y,false);if(x>=-32600&&y>highest){highest=y;found=true;}}
   }
   return found?(short?)highest:null;
  }
  // Original 1449:05DA..061A reuses C584 for either axis; only centering/top
  // recomputes it. Explicit Z retains it, including its initial zero value.
  public static void KeepEditedCursorVisible(Canvas canvas,short x,short y){
   int anchor=retainedAnchor;
   if(x<=1||x>=638)OriginX=unchecked((short)(OriginX-x+anchor));
   if(y<=canvas.DosClipTop||y>=canvas.DosClipBottom)OriginY=unchecked((short)(OriginY-y+anchor));
  }
  public static bool NeedsEditEntryReveal(short x,short y){return x<=0||x>=638||y<=97||y>=461;}
  public static void RevealActive(Canvas canvas){
   if(canvas.Selected==null)return;var context=canvas.PageDocument.Contexts[canvas.Selected];
   var point=NativeEditCursorPosition.Get(canvas.Selected,context,Divisor,Divisor,OriginX,OriginY);short x=(short)point.X,y=(short)point.Y;
   if(NeedsEditEntryReveal(x,y)){NativeDosFrameCache.HideCursor(canvas);Center(canvas,ZoomFactor);}
  }
  public static bool CenteringCommand(Canvas canvas,string verb,string rest){
   float size;
   if(verb=="Z"){
    string[] values=rest.Split(new[]{' ','\t',','},StringSplitOptions.RemoveEmptyEntries);
    bool zero=values.Length>0&&values.Length<=3&&float.TryParse(values[0],NumberStyles.Float,CultureInfo.InvariantCulture,out size)&&size==0;
    if(rest.Length==0||zero){
     float horizontal=values.Length>1?float.Parse(values[1],CultureInfo.InvariantCulture):0;
     float vertical=values.Length>2?float.Parse(values[2],CultureInfo.InvariantCulture):0;
     if(float.IsNaN(horizontal)||float.IsInfinity(horizontal)||float.IsNaN(vertical)||float.IsInfinity(vertical))throw new InvalidDataException("Enter finite viewport percentages.");
     if(NativeZoomFraming.HelpDefaults&&values.Length<=1)Command(canvas,"Z","1");
     else if(canvas.Selected==null&&canvas.Score!=null){
      if(canvas.Score.Items.Count!=0)Center(canvas,ZoomFactor);
      else if(values.Length>1)pendingEmptyZoom=ZoomOrigin(1,horizontal,vertical);
     }
     return true;
    }
   }
   if(verb=="ZT"&&rest.Length==0&&canvas.Score!=null){
    if(canvas.Score.Items.Count==0&&pendingEmptyZoom.HasValue){
     Point pending=pendingEmptyZoom.Value;
     OriginX=(short)pending.X;OriginY=(short)pending.Y;Divisor=50;ZoomFactor=1;Refresh();
    }
    return true;
   }
   if(verb!="ZZ")return false;Center(canvas,rest.Length==0?(NativeZoomFraming.HelpDefaults?3:2):float.Parse(rest,CultureInfo.InvariantCulture));return true;
  }
  public static bool Command(string verb,string rest){return Command(null,verb,rest);}
  public static bool Command(Canvas canvas,string verb,string rest){
   if(verb!="Z"&&verb!="ZL"&&verb!="ZR"&&verb!="ZU"&&verb!="ZD")return false;
   string[] values=rest.Split(new[]{' ','\t',','},StringSplitOptions.RemoveEmptyEntries);
   if(verb=="Z"){if(values.Length==0)throw new NotSupportedException("A bare Z rebuild requires the active editor context.");if(values.Length>3)throw new InvalidDataException("Use Z size [horizontal-percent [vertical-percent]].");float[] p=new float[3];for(int n=0;n<values.Length;n++)p[n]=float.Parse(values[n],CultureInfo.InvariantCulture);if(p[0]==0)throw new NotSupportedException("Zero zoom with additional arguments needs rebuild-path verification.");short savedX=OriginX,savedY=OriginY,savedDivisor=Divisor;if(canvas!=null)NativeEditPositionGuide.Set(canvas,null);bool scor4=canvas!=null&&canvas.Scor4Interface;ZoomTo(p[0],p[1],p[2],scor4);if(scor4){canvas.HasZoomCommand=true;canvas.Scor4LiteralZoom=canvas.Selected==null;}
    if(canvas!=null&&canvas.Selected!=null){var context=canvas.PageDocument.Contexts[canvas.Selected];var cursor=NativeEditCursorPosition.Get(canvas.Selected,context,Divisor,Divisor,OriginX,OriginY);short x=(short)cursor.X,y=(short)cursor.Y;
     //1449:05DA..061A uses AC98..AC9E without refreshing them on this explicit-Z path.
     //06A3..06B8 saved the preceding viewport descriptor in those words.
     if(x<=savedDivisor||x>=savedX)OriginX=unchecked((short)(OriginX-x+retainedAnchor));if(y<=savedDivisor||y>=savedY)OriginY=unchecked((short)(OriginY-y+retainedAnchor));RevealActive(canvas);Refresh();
    }}
   else{
    if(values.Length>1)throw new InvalidDataException("Use "+verb+" [percent].");
    bool scor4=canvas!=null&&canvas.Scor4Interface;
    Pan(verb,values.Length==0?0:float.Parse(values[0],CultureInfo.InvariantCulture),scor4);
    if(scor4)canvas.HasZoomCommand=true;
   }return true;
  }
  public static PointF Point(float x,float y){return new PointF(NativeNotationContext.Word(x,true)/Divisor+OriginX,-NativeNotationContext.Word(y,false)/Divisor+OriginY-97);}
  public static void Reset(){ZoomTo(1,0,0);}
  internal static void DisplayChanged(){frameRevision++;}
  internal static void DeleteReplay(Canvas canvas,NativeFreshDisplayArray before,int ordinal,NativeFreshDisplayArray after){NativeDosFrameCache.DeleteReplay(canvas,before,ordinal,after,frameRevision);}
  internal static void AppendReplay(Canvas canvas,NativeFreshDisplayArray after,bool restoreCursorAfterDraw){NativeDosFrameCache.AppendReplay(canvas,after,restoreCursorAfterDraw,frameRevision);}
  static void Refresh(){pendingEmptyZoom=null;frameRevision++;foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void AddMenu(ContextMenuStrip menu){var reset=new ToolStripMenuItem("DOS viewport: reset view");reset.Click+=(s,e)=>Reset();menu.Items.Add(reset);var zoom=new ToolStripMenuItem("DOS viewport zoom");foreach(short value in new short[]{50,25,10,5}){short divisor=value;var item=new ToolStripMenuItem((50/divisor)+"x");item.Click+=(s,e)=>{ZoomTo(50f/divisor,0,0);};zoom.DropDownItems.Add(item);menu.Opening+=(s,e)=>{item.Checked=Divisor==divisor;};}menu.Items.Add(zoom);}
  public static void Advance(Canvas canvas){if(NativeNotationView.IsDosViewport&&canvas.Score!=null&&canvas.PageDocument!=null)NativeDosFrameCache.Get(canvas,frameRevision);}
  public static byte[] Snapshot(Canvas canvas){Advance(canvas);return (byte[])canvas.DosInputNumbers(NativeDosFrameCache.Get(canvas,frameRevision).Plane).Clone();}
  public static bool Draw(Canvas canvas,Graphics target,bool paper){if(!NativeNotationView.IsDosViewport)return false;canvas.DrawingIssues.Clear();if(canvas.Score==null||canvas.PageDocument==null)return true;var raster=NativeDosFrameCache.Get(canvas,frameRevision);var plane=canvas.DosInputNumbers(NativeDosFrameCache.PresentationPlane(canvas,raster,canvas.DosCursorVisible));
   int height=canvas.DosClipBottom+1;
   using(var bitmap=new Bitmap(640,height,PixelFormat.Format1bppIndexed)){
    var palette=bitmap.Palette;
    palette.Entries[0]=Color.White;
    palette.Entries[1]=Color.Black;
    bitmap.Palette=palette;
    var bits=bitmap.LockBits(new Rectangle(0,0,640,height),ImageLockMode.WriteOnly,PixelFormat.Format1bppIndexed);
    try{
     // SCORE and GDI+ both store the leftmost pixel in the most significant bit.
     const int bytesPerRow=640/8;
     if(bits.Stride==bytesPerRow)Marshal.Copy(plane,0,bits.Scan0,bytesPerRow*height);
     else for(int y=0;y<height;y++)Marshal.Copy(plane,y*bytesPerRow,IntPtr.Add(bits.Scan0,y*bits.Stride),bytesPerRow);
    }finally{bitmap.UnlockBits(bits);}
    var saved=target.Save();
    try{
     target.InterpolationMode=InterpolationMode.NearestNeighbor;
     target.PixelOffsetMode=PixelOffsetMode.Half;
     target.DrawImage(bitmap,new RectangleF(0,-97,640,height),new RectangleF(0,0,640,height),GraphicsUnit.Pixel);
    }finally{target.Restore(saved);}
   }
   return true;
  }
 }
}
