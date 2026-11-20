using System;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintedSymbols {
  static bool Zero(Item i,int first,int last){for(int n=first;n<=last;n++)if(i.P(n)!=0)return false;return true;}
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache=null){
   if(page==null||library==null)return false;
   int glyph=-1;float x=page.Prepared.X,y=page.Prepared.Y,scale=page.Scale/25,horizontal=1;
   if(item.Code==18&&(item.P(5)==98||item.P(5)==99)){
    for(int n=17;n<=20;n++)if(item.P(n)!=0)return false;
    if(!ScoreVersionControls.MeterThicknessSupported(NativeLibraryLocation.IsScor4(library.Root),item.P(16)))return false;
    if(!NativeMeterDisplay.ValidSize(item.P(10))||!NativeMeterDisplay.ValidSize(item.P(11)))return false;
    if((item.P(14)!=0&&item.P(14)!=1)||!NativeMeterDisplay.ValidSize(item.P(7))||Single.IsNaN(item.P(15))||Single.IsInfinity(item.P(15)))return false;
    float size=item.P(7)==0?1:item.P(7);
    x=(float)((double)x+item.P(15)*150);
    glyph=117-(int)item.P(5);horizontal=3*size;
    scale=(float)((double)page.Scale*(float)((double)size*.75)/6.25);
   }
   else if(item.Code==3&&(item.P(5)==0||item.P(5)==1||item.P(5)==2)&&(item.P(6)==0||item.P(6)==1)&&Zero(item,7,20)&&(item.P(4)==0||(item.P(4)==100&&item.P(5)==1))){
    glyph=(int)item.P(5);bool cue=item.P(4)==100;horizontal=cue?.800000011920929f:1;float scaled=cue?(float)((double)page.Scale*.800000011920929):page.Scale;scale=scaled/25;y=(float)((double)page.Prepared.Y+(cue?29.75:21)*page.Scale);
   }
   else if(item.Code==2&&item.P(4)==0&&item.P(5)==0&&item.P(6)==0&&item.P(7)==1&&Zero(item,8,20)){glyph=43;y=(float)((double)page.Prepared.Y+56*page.Scale);}
   else if(item.Code==2&&item.P(4)==0&&item.P(5)==-2&&item.P(6)==0&&item.P(7)==4&&item.P(8)==0&&item.P(9)>0&&Zero(item,10,20)){
    Glyph rest;if(!library.Symbols.TryGetValue(41,out rest))return false;RenderResourceAudit.Record("printed rest", "41:"+rest.Name,rest.Source,"natural whole-rest branch; five point requests verified");
    float rawX=(float)((double)item.P(9)*150),rawY=(float)((double)page.Prepared.Y+63*page.Scale);var state=g.Save();try{using(var m=page.Projection.Matrix())g.MultiplyTransform(m,MatrixOrder.Prepend);using(var path=new GraphicsPath(FillMode.Alternate))using(var pen=new Pen(Color.Black,NativePrintContext.DefaultLineWidth)){pen.LineJoin=LineJoin.Round;PointF previous=new PointF();bool have=false;foreach(var p in rest.Points){var next=new PointF(NativeNotationContext.Word((float)((double)p.X*page.Scale/4+rawX),true),NativeNotationContext.Word((float)((double)p.Y*page.Scale/4+rawY),false));if(have)path.AddLine(previous,next);previous=next;have=true;}g.VectorFillPath(Brushes.Black,path);if(true)g.VectorDrawPath(pen,path);}}finally{g.Restore(state);}return true;
   }
   if(glyph<0)return false;return DrawGlyph(g,page,library,glyph,x,y,PrintedGlyphDefinition.Fixed3(scale),horizontal,cache,item.Code==18?(NativeLibraryLocation.IsScor4(library.Root)&&item.P(16)!=0?ScoreVersionControls.MeterThicknessSelector(true,item.P(16)):unchecked((short)(int)item.P(10))):(short)0);
  }
  internal static bool DrawGlyph(Graphics g,NativeNotationContext page,DrawingLibrary library,int number,float rawX,float rawY,string formattedScale,float horizontal,HashSet<int> cache,short widthSelector=0){
   Glyph glyph;if(!library.Symbols.TryGetValue(number,out glyph))return false;if(cache!=null&&!cache.Contains(number))return NativePrimitiveGlyphs.Draw(g,page,library,null,new PrimitiveGlyphRequest{Glyph=number,X=rawX,Y=rawY,Horizontal=horizontal,Vertical=horizontal,WidthSelector=widthSelector});short x=NativeNotationContext.RawWord(rawX),y=NativeNotationContext.RawWord(rawY);float scale=Single.Parse(formattedScale,CultureInfo.InvariantCulture);if(scale==0)return false;
   float width=(float)((double)NativePrintContext.UnclampedWidth(widthSelector)/scale);if(width<0)width=0;
   bool inheritedDefault=width==NativePrintContext.RawWidth(0);width=inheritedDefault?NativePrintContext.DefaultLineWidth:Single.Parse(PrintedGlyphDefinition.Fixed3(width),CultureInfo.InvariantCulture);   var eps=NativeEpsExport.For(g);if(eps!=null)eps.Comment("SCORE external glyph "+number+" x "+x+" y "+y+" scale "+formattedScale+" width "+width.ToString("F3",CultureInfo.InvariantCulture));
   RenderResourceAudit.Record("printed glyph",number+":"+glyph.Name,glyph.Source,"naturally measured clef/common-time request domain");var state=g.Save();try{using(var matrix=page.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);
    using(var path=new GraphicsPath(FillMode.Alternate))using(var pen=new Pen(Color.Black,width)){pen.LineJoin=LineJoin.Round;int kind=1;bool have=false;PointF previous=new PointF();Action flush=()=>{if(path.PointCount>0){if(kind!=1)g.TrackedFillPath(Brushes.Black,path);if(kind==1||true)g.TrackedDrawPath(pen,path);if(eps!=null)eps.ScoreGlyph(path,kind!=1,width,scale,x,y,inheritedDefault);}path.Reset();have=false;};
     for(int j=0;j<glyph.Points.Length;j++){var p=PrintedGlyphDefinition.Vertex((short)glyph.Points[j].X,(short)glyph.Points[j].Y,glyph.Operations[j]);if(p.Operation>0){if(p.Operation!=3&&!(p.Operation==1&&kind==1))flush();path.StartFigure();have=false;kind=p.Operation==1?1:2;}var next=new PointF(p.X,p.Y);if(have)path.AddLine(previous,next);previous=next;have=true;}flush();}
   }finally{g.Restore(state);}if(!inheritedDefault)NativePrintContext.RestoreDefaultLineWidth();return true;
  }
 }
}
