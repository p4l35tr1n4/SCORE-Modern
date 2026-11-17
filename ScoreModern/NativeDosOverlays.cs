using System;using System.Collections.Generic;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDosOverlays {
  static float F(double x){return (float)x;}
  public static void Draw(NativeDosRaster raster,Score score,NativePageDocument doc,ScoreText text,ISet<int> hidden=null){
   foreach(var item in score.Items)if(item.Code==8&&item.Staff==1&&(hidden==null||!hidden.Contains(1)))raster.Draw(NativeScanStructuralVectors.ScreenGuide(item,doc));
   foreach(var number in text.ShowActualDatFonts?new NumberPlan[0]:Numbers(score,doc,text,hidden))raster.Draw(number.Points);
   for(int y=raster.GraphicsTop;y<=461;y++){Set(raster.Plane,0,y);Set(raster.Plane,639,y);}
  }
  public sealed class NumberPlan {public int Staff;public NativeNotationContext Context;public List<PrimitiveTextPoint> Points;}
  public static IEnumerable<NumberPlan> Numbers(Score score,NativePageDocument doc,ScoreText text,ISet<int> hidden=null){
   var defined=new SortedDictionary<int,Item>();foreach(var item in score.Items)if(item.Code==8&&item.Staff>=1&&item.Staff<=32)defined[item.Staff]=item;
   foreach(var entry in defined){if(hidden!=null&&hidden.Contains(entry.Key)||entry.Value.P(12)==1)continue;int staff=entry.Key;float scale=doc.Tables.Scales[staff],y=F((double)doc.Tables.Bases[staff]+scale*42);var context=new NativeNotationContext{Staff=staff,RawBase=doc.Tables.Bases[staff],Prepared=new PageItemContext{Scale=scale,X=-900,Y=y},Projection=doc.Projection};var body=Item.Make(16,staff,0,0,1,F(25.0/scale),0,98,0,0,0,1,0);yield return new NumberPlan{Staff=staff,Context=context,Points=text.ScanStrokeText(body,staff.ToString(CultureInfo.InvariantCulture),context,0)};}
  }
  public static void DrawVector(Canvas canvas,System.Drawing.Graphics graphics){NativeSpacingScale.DrawVector(canvas,graphics);NativeEditPositionGuide.DrawVector(canvas,graphics);if(NativeNotationView.IsDosViewport||NativeNotationView.IsPrinting||NativeEpsExport.For(graphics)!=null||canvas.Score==null||canvas.PageDocument==null||canvas.TextLayout==null||canvas.TextLayout.ShowActualDatFonts)return;foreach(var number in Numbers(canvas.Score,canvas.PageDocument,canvas.TextLayout,canvas.DisappearingStaffs))NativeDatDrawing.Points(graphics,number.Context,number.Points);}
  static void Set(byte[] plane,int x,int y){plane[y*80+x/8]|=(byte)(128>>(x%8));}
 }
}

