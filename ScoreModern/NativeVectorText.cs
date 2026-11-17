using System;using System.IO;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class ScoreText {
  readonly Dictionary<string,Glyph[]> textDrawings=new Dictionary<string,Glyph[]>();
  static readonly string[] VectorFamilies={"BODN","BODB","BODI","BDIB","MUSI"};
  public Glyph VectorGlyph(int face,int code){
   string family=face>=90&&face<=94?VectorFamilies[face-90]:"BODN";string name=family+(code/10);Glyph[] glyphs;
   if(!textDrawings.TryGetValue(name,out glyphs)){glyphs=DrawingLibrary.Load(NativeLibraryLocation.FileFor(RootDirectory,name+".DRW"));textDrawings.Add(name,glyphs);}RenderResourceAudit.Record("text DRW glyph",face+":"+code,NativeLibraryLocation.FileFor(RootDirectory,name+".DRW"),"external slot "+(code%10));return glyphs[code%10];
  }
  public PrimitiveTextPoint[] StrokePoints(int code,NativeTextLayout layout,float staff,float x,float y){
   if(code<0||code>=70)throw new NotSupportedException("Stroke-text code outside the verified packed directory.");
   if(textDat==null)textDat=File.ReadAllBytes(NativeLibraryLocation.FileFor(RootDirectory,"SCOR3_10.DAT"));
   RenderResourceAudit.Record("DAT stroke glyph",code.ToString(),NativeLibraryLocation.FileFor(RootDirectory,"SCOR3_10.DAT"),"verified packed directory domain 0..69");int start=BitConverter.ToUInt16(textDat,3316+code*2),end=BitConverter.ToUInt16(textDat,3318+code*2);
   var scale=PrimitiveTextGeometry.Prepare(layout.Parameters.GlyphHorizontal,layout.Parameters.GlyphVertical,staff,x,y,40,26);var result=new PrimitiveTextPoint[end-start];
   for(int n=0;n<result.Length;n++)result[n]=PrimitiveTextGeometry.Point(BitConverter.ToInt16(textDat,3456+(start+n)*2),scale);return result;
  }
  public PrimitiveTextPoint[] DrawingPoints(int face,int code,NativeTextLayout layout,float staff,float x,float y){
   Glyph glyph=VectorGlyph(face,code);float sx=(float)((double)layout.Parameters.GlyphHorizontal*staff),sy=(float)((double)layout.Parameters.GlyphVertical*staff);var points=new PrimitiveTextPoint[glyph.Points.Length];
   for(int n=0;n<points.Length;n++)points[n]=new PrimitiveTextPoint{X=(float)((double)glyph.Points[n].X*sx+x),Y=(float)((double)glyph.Points[n].Y*sy+y),Operation=glyph.Operations[n]};return points;
  }
  PrimitiveTextPoint[] DatAuxiliaryPoints(int slot,NativeTextLayout layout,float staff,float x,float y){
   const string name="BODO0";Glyph[] glyphs;if(!textDrawings.TryGetValue(name,out glyphs)){glyphs=DrawingLibrary.Load(NativeLibraryLocation.FileFor(RootDirectory,name+".DRW"));textDrawings.Add(name,glyphs);}var glyph=glyphs[slot];float h=(float)((double)layout.Parameters.GlyphHorizontal*staff),v=(float)((double)layout.Parameters.GlyphVertical*staff);var points=new PrimitiveTextPoint[glyph.Points.Length];for(int n=0;n<points.Length;n++)points[n]=new PrimitiveTextPoint{X=(float)((double)glyph.Points[n].X*h+x),Y=(float)((double)glyph.Points[n].Y*v+y),Operation=glyph.Operations[n]};RenderResourceAudit.Record("DAT auxiliary text glyph",slot.ToString(),NativeLibraryLocation.FileFor(RootDirectory,name+".DRW"),"original shared character substitution");return points;
  }
  // F6 buffers the primitive contours while emitting Type 1 text. The
  // active font at the end selects the width for all buffered contours.
  static float? VectorTextStrokeWidth(short selector,short finalFont){
   return selector<0&&finalFont%100>=90?(float?)NativePrintContext.FixedWidth(selector):null;
  }
  sealed class VectorTextFrame:IDisposable {
   readonly Graphics graphics;readonly GraphicsPath path=new GraphicsPath(FillMode.Alternate);readonly Pen pen;readonly float width;
   readonly GraphicsPath rawPath=new GraphicsPath(FillMode.Alternate);readonly PointF rawPivot;readonly float rawRotation;bool rawEps;
   bool have,outline;int kind=1;PointF previous,rawPrevious;
   public VectorTextFrame(Graphics g,bool scoreCoordinates=false,float? strokeWidth=null,PointF? pivot=null,float rotation=0){graphics=g;rawPivot=pivot??PointF.Empty;rawRotation=rotation;rawEps=scoreCoordinates;width=strokeWidth??NativePrintContext.DefaultLineWidth;pen=new Pen(Color.Black,width/30);pen.LineJoin=LineJoin.Round;}
   public void Flush(){if(path.PointCount>0){var eps=NativeEpsExport.For(graphics);if(rawEps&&eps!=null){if(kind!=1&&!outline)graphics.FillPath(Brushes.Black,path);graphics.DrawPath(pen,path);using(var turn=eps.PushRawRotation(rawPivot,rawRotation))eps.ScorePath(rawPath,kind!=1&&!outline,width);}else{if(kind!=1&&!outline)graphics.VectorFillPath(Brushes.Black,path);graphics.VectorDrawPath(pen,path);}}path.Reset();rawPath.Reset();have=false;}
   public void Add(PrimitiveTextPoint[] points,bool unfilled,float originX,float originY,bool oneOutlineStroke=false){
    if(outline!=unfilled)Flush();outline=unfilled;
    // Original outlined text accumulates its contours in one stroked path.
    // Separate strokes darken antialiased intersections (notably the hash).
    foreach(var p in points){int operation=(unfilled||oneOutlineStroke)&&p.Operation>0?1:p.Operation;
     if(operation>0){if(operation!=3&&!(operation==1&&kind==1))Flush();path.StartFigure();rawPath.StartFigure();have=false;kind=operation==1?1:2;}
     var rawNext=new PointF(NativeNotationContext.Word(p.X,true),NativeNotationContext.Word(p.Y,false));var next=new PointF((rawNext.X-originX)/30,-(rawNext.Y-originY)/30);if(have){path.AddLine(previous,next);rawPath.AddLine(rawPrevious,rawNext);}previous=next;rawPrevious=rawNext;have=true;
    }
    if(kind!=1||oneOutlineStroke)Flush();
   }
   public void Dispose(){Flush();path.Dispose();rawPath.Dispose();pen.Dispose();}
  }
  void DrawVectorToken(VectorTextFrame frame,NativeTextToken token,NativeTextLayout layout,float staff,float x,float y,float originX,float originY){
   if(token.Code==94)return;int face=PrintedMusic(token.Code)?90:token.FontWord%100;bool stroke=face==98;
   //The primitive directory contains only0..69. Its common punctuation
   //uses the shared BODN drawings; the measured hash/at substitutions use BODO.
   PrimitiveTextPoint[] points;if((face>=90&&face<=94||face==98)&&token.Code==132)points=DatAuxiliaryPoints(1,layout,staff,x,y);else if((face>=90&&face<=94||face==98)&&token.Code==140)points=DatAuxiliaryPoints(9,layout,staff,x,y);else if((face>=90&&face<=94||face==98)&&((token.Code>=70&&token.Code<=76)||token.Code==88))points=DrawingPoints(90,token.Code,layout,staff,x,y);else points=stroke?StrokePoints(token.Code,layout,staff,x,y):DrawingPoints(face,token.Code>=162?token.Code-68:token.Code,layout,staff,x,y);
   frame.Add(points,token.FontWord>=100,originX,originY,token.FontWord>=100&&PrintedMusic(token.Code));
  }
 }
}

