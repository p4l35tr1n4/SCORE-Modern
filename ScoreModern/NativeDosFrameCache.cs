using System.Drawing;
using System;using System.Collections.Generic;using System.Runtime.CompilerServices;
namespace ScoreModern {
 public static class NativeDosFrameCache {
  sealed class Frame {
   public Score Score;public DrawingLibrary Library;public ScoreText Text;public byte[] Music,EditBase;public long Revision;public short Divisor,X,Y,ClipTop;public bool BoundsMeasured;public readonly Dictionary<Item,RectangleF> Bounds=new Dictionary<Item,RectangleF>();public Item Selected,EditOriginal;public bool EditWasModified;public short? GuideMeasurement;public float DatBarlineUnit;public NativeDosRaster Raster;public NativeDosEditCursor.State Cursor=new NativeDosEditCursor.State();public readonly List<string> Issues=new List<string>();
   public readonly NativeDatHeadCache Heads=new NativeDatHeadCache();
  }
  static readonly ConditionalWeakTable<Canvas,Frame> Frames=new ConditionalWeakTable<Canvas,Frame>();
  static bool Same(byte[] a,byte[] b){if(a==null||a.Length!=b.Length)return false;for(int n=0;n<a.Length;n++)if(a[n]!=b[n])return false;return true;}
  public static void HideCursor(Canvas canvas){Frame f;if(Frames.TryGetValue(canvas,out f)&&f.Raster!=null)f.Cursor.Hide(f.Raster.Plane);}
  internal static byte[] PresentationPlane(Canvas canvas,NativeDosRaster raster,bool cursorVisible){
   if(cursorVisible)return raster.Plane;
   var plane=(byte[])raster.Plane.Clone();
   Frames.GetOrCreateValue(canvas).Cursor.RestorePixels(plane);
   return plane;
  }
  internal static void DeleteReplay(Canvas canvas,NativeFreshDisplayArray before,int ordinal,NativeFreshDisplayArray after,long revision){
   Frame frame;
   if(before==null||after==null||!NativeNotationView.IsDosViewport||!Frames.TryGetValue(canvas,out frame)||frame.Raster==null||
    frame.Revision!=revision||frame.Divisor!=NativeDosViewport.Divisor||frame.X!=NativeDosViewport.OriginX||frame.Y!=NativeDosViewport.OriginY||frame.ClipTop!=canvas.DosClipTop)return;
   frame.Cursor.Hide(frame.Raster.Plane);
   NativeDosDeleteRegion.Replay(frame.Raster,before,ordinal,after,canvas.DosClipBottom,canvas.DosFullGraphics);
   frame.Music=canvas.Score.Encode();frame.EditWasModified=false;frame.EditOriginal=null;frame.Selected=null;frame.BoundsMeasured=false;
  }
  internal static void AppendReplay(Canvas canvas,NativeFreshDisplayArray after,bool restoreCursorAfterDraw,long revision){
   Frame frame;
   if(after==null||!NativeNotationView.IsDosViewport||!Frames.TryGetValue(canvas,out frame)||frame.Raster==null||
    frame.Revision!=revision||frame.Divisor!=NativeDosViewport.Divisor||frame.X!=NativeDosViewport.OriginX||frame.Y!=NativeDosViewport.OriginY||frame.ClipTop!=canvas.DosClipTop)return;
   if(!restoreCursorAfterDraw)frame.Cursor.Hide(frame.Raster.Plane);
   NativeDosDeleteRegion.Append(frame.Raster,after,after.Pointers[after.Pointers.Length-2],after.X.Length);
   // After an interior deletion the next item's edit cursor can still
   // cover the restored vectors. SCORE restores its saved pixels last.
   if(restoreCursorAfterDraw)frame.Cursor.Hide(frame.Raster.Plane);
   frame.Music=canvas.Score.Encode();frame.EditWasModified=false;frame.EditOriginal=null;frame.Selected=null;frame.BoundsMeasured=false;
  }
  // Original-grid parameter preview/abort sequences verify these object routes.
  // Other controls still require their own original comparisons.
  static bool VerifiedPreviewCode(int code){return code>=1&&code<=18;}
  static NativeDosRaster Empty(Canvas canvas){return new NativeDosRaster(NativeDosViewport.Divisor,NativeDosViewport.Divisor,NativeDosViewport.OriginX,NativeDosViewport.OriginY,canvas.DosClipTop,canvas.DosClipBottom,canvas.DosFullGraphics);}
  static void DrawItem(Canvas canvas,Frame f,NativeDosRaster raster,Item item,ref short font){if(canvas.DisplayHidden(item))return;try{short next;var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,NativeDatDrawing.Get(canvas.Library),f.Heads,font,out next);if(NativeMouseSelector.ShowPoints){var isolated=Empty(canvas);isolated.Draw(points);f.Bounds[item]=NativeItemBounds.DosPlane(isolated.Plane);}raster.Draw(points);font=next;}catch(NotSupportedException e){RecordDrawIssue(f,item,e);}
   catch(System.IO.InvalidDataException e){RecordDrawIssue(f,item,e);}
   catch(System.IO.IOException e){RecordDrawIssue(f,item,e);}
  }
  static void RecordDrawIssue(Frame frame,Item item,Exception error){
   string message="DOS viewport: "+error.Message;
   frame.Issues.Add(message);
   RenderResourceAudit.Unavailable("DOS viewport item "+item.Code,message);
  }
  public static NativeDosRaster Get(Canvas canvas,long revision){
   var f=Frames.GetOrCreateValue(canvas);byte[] music=canvas.Score.Encode();bool identity=f.Score!=canvas.Score||f.Library!=canvas.Library||f.Text!=canvas.TextLayout,musicChanged=!Same(f.Music,music);
   bool view=f.DatBarlineUnit!=canvas.PageDocument.DatBarlineUnit||f.GuideMeasurement!=canvas.PageDocument.GuideMeasurement||f.Raster==null||f.Revision!=revision||f.Divisor!=NativeDosViewport.Divisor||f.X!=NativeDosViewport.OriginX||f.Y!=NativeDosViewport.OriginY||f.ClipTop!=canvas.DosClipTop||NativeMouseSelector.ShowPoints&&!f.BoundsMeasured;
   bool entered=canvas.EditOriginal!=null&&f.EditOriginal!=canvas.EditOriginal;
   bool commit=f.EditOriginal!=null&&f.EditOriginal!=canvas.EditOriginal&&f.EditWasModified;
   bool preview=!identity&&!view&&!entered&&musicChanged&&canvas.EditOriginal!=null&&f.EditBase!=null&&canvas.Selected!=null&&VerifiedPreviewCode(canvas.Selected.Code);
   bool rebuild=identity||musicChanged||view||commit;
   if(identity){f.EditBase=null;f.Cursor=new NativeDosEditCursor.State();}
   if(rebuild){f.Issues.Clear();var raster=Empty(canvas);if(!preview)f.Bounds.Clear();short font=0;
    if(preview){Buffer.BlockCopy(f.EditBase,0,raster.Plane,0,f.EditBase.Length);if(!Same(canvas.Selected.Bytes,canvas.EditOriginal.Bytes))DrawItem(canvas,f,raster,canvas.Selected,ref font);f.Cursor=new NativeDosEditCursor.State();}
    // A new viewport draw builds its glyph offsets from its own first note.
    // An in-place preview retains the cache of the frame it is updating.
    else{f.Heads.Reset();foreach(var item in canvas.Score.Items){
     var audit=RenderResourceAudit.Current;
     if(audit!=null)audit.Item(item);
     DrawItem(canvas,f,raster,item,ref font);
    }NativeDosOverlays.Draw(raster,canvas.Score,canvas.PageDocument,canvas.TextLayout,canvas.DisappearingStaffs);raster.DrawDevice(NativeSpacingScale.Device(canvas));raster.DrawDevice(NativeEditPositionGuide.Device(canvas));// A fresh scene contains no cursor: old saved pixels belong to the previous raster.
    f.Cursor=new NativeDosEditCursor.State();if(canvas.EditModified&&canvas.Selected!=null)f.Issues.Add("DOS viewport: pending edit preview for this Code "+canvas.Selected.Code+" state is not yet verified; full scene drawing used.");}
    f.Raster=raster;f.BoundsMeasured=NativeMouseSelector.ShowPoints;
   }
   if(rebuild||f.Selected!=canvas.Selected||entered){
    // Original restore follows scene redraw; selection-only changes keep VRAM.
    f.Cursor.Hide(f.Raster.Plane);
    if(canvas.EditOriginal!=null&&(entered||view||identity))f.EditBase=(byte[])f.Raster.Plane.Clone();
    if(canvas.Selected!=null){var page=canvas.PageDocument.Contexts[canvas.Selected];var point=NativeEditCursorPosition.Get(canvas.Selected,page,NativeDosViewport.Divisor,NativeDosViewport.Divisor,NativeDosViewport.OriginX,NativeDosViewport.OriginY);f.Cursor.Show(f.Raster.Plane,(short)point.X,(short)point.Y);}
   }
   f.DatBarlineUnit=canvas.PageDocument.DatBarlineUnit;f.GuideMeasurement=canvas.PageDocument.GuideMeasurement;f.Score=canvas.Score;f.Library=canvas.Library;f.Text=canvas.TextLayout;f.Music=music;f.Revision=revision;f.Divisor=NativeDosViewport.Divisor;f.X=NativeDosViewport.OriginX;f.Y=NativeDosViewport.OriginY;f.ClipTop=canvas.DosClipTop;f.Selected=canvas.Selected;f.EditOriginal=canvas.EditOriginal;f.EditWasModified=canvas.EditModified;if(canvas.EditOriginal==null)f.EditBase=null;
   if(NativeMouseSelector.ShowPoints){canvas.ItemBounds.Clear();foreach(var pair in f.Bounds)canvas.ItemBounds[pair.Key]=pair.Value;}canvas.DrawingIssues.Clear();canvas.DrawingIssues.AddRange(f.Issues);return f.Raster;
  }
 }
}

