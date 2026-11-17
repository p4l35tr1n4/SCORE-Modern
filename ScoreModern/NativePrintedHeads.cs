using System;using System.Collections.Generic;using System.Globalization;using System.Drawing;using System.Drawing.Drawing2D;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintedHeads {
  internal static bool Parenthesized(Item item){
   return item.P(6)>=10&&item.P(6)<20&&item.P(6)==(int)item.P(6);
  }
  internal static bool Custom(Item item){
   return item.P(6)>=20&&item.P(6)<3000&&item.P(6)==(int)item.P(6);
  }
  internal static Item CustomOctaveText(Item item,NativeNotationContext page,ScoreText text){
   if(text==null||!Custom(item)||(item.P(6)<45||item.P(6)>47))return null;
   var body=Item.Make(16,item.P(2),item.P(3),item.P(4),1,item.P(6),item.P(7),item.P(11)==0?1002:item.P(11),item.P(9),.6f,0,1,0);
   text.Set(body,item.P(6)==45?"8va":item.P(6)==46?"8ba":"15ma",page.Scale);
   return body;
  }
  public static bool Request(Item item,NativeNotationContext page,out PrintedHeadRequest request){
   request=new PrintedHeadRequest();if(page==null||item.Code!=1||(item.P(4)<=-200||item.P(4)>=200)||(Single.IsNaN(item.P(15))||Single.IsInfinity(item.P(15))||Math.Abs(item.P(15))>32767)||(item.P(12)!=0&&item.P(12)!=1&&item.P(12)!=2)||item.P(20)!=0||item.P(6)<0||item.P(6)>=4)return false;
   float noteScale,headX,headY;try{NativeScanMusicVectors.HeadProjection(item,page,out noteScale,out headX,out headY);}catch(NotSupportedException){return false;}float scale=(float)((double)noteScale/25);if(scale==0)return false;
   request=new PrintedHeadRequest{Glyph=11+(int)item.P(6),X=NativeNotationContext.RawWord(headX),Y=NativeNotationContext.RawWord(headY),Scale=PrintedGlyphDefinition.Fixed3(scale),Width=PrintedGlyphDefinition.Fixed3(Math.Max(0,(float)((double)NativePrintContext.RawWidth(0)/scale)))};return true;
  }
  internal static bool DrawJoinedStem(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache){
   if(NativeNotationView.UsesDat||item.P(6)!=5||item.P(4)<=-100||item.P(4)>=100||
    item.P(10)!=0||item.P(12)!=0||item.P(15)!=0||(cache!=null&&cache.Contains(16)))return false;
   for(int n=16;n<=20;n++)if(item.P(n)!=0)return false;
   var plan=NativeNoteDetails.Prepare(item,page);
   if(plan==null||!plan.Stem||plan.Slash)return false;
   var head=new PrimitiveGlyphRequest{Glyph=16,X=page.Prepared.X,Y=page.Prepared.Y,Horizontal=1,Vertical=1};
   var stem=new[]{new BarlinePoint(plan.StemX,plan.StemStart,1),new BarlinePoint(plan.StemX,plan.StemEnd,0)};
   return NativePrimitiveGlyphs.Draw(g,page,library,cache,head,stem);
  }
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library){return Draw(g,item,page,library,null);}
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache){
   return Draw(g,item,page,library,cache,null);
  }
  internal static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,ScoreText text){

   if(NativeDatDrawing.Head(g,item,page,library))return true;
   if(item.P(6)==-1)return true;
   if(Parenthesized(item)){
    var inner=item.Clone();
    inner.Set(6,item.P(6)-10);
    float projectedScale,hx,hy;NativeScanMusicVectors.HeadProjection(item,page,out projectedScale,out hx,out hy);
    float innerScale,innerX,innerY;NativeScanMusicVectors.HeadProjection(inner,page,out innerScale,out innerX,out innerY);
    var innerContext=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};
    innerContext.Prepared.X=(float)((double)page.Prepared.X+hx-innerX);
    if(!Draw(g,inner,innerContext,library,cache))return false;
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};
    context.Prepared.Scale=projectedScale;context.Prepared.Y=hy;
    float left=-13.800000190734863f;int accidental=(int)item.P(5)%10;
    if(accidental!=0)left=(float)((double)left-14.399999618530273);
    if(accidental==4)left=(float)((double)left-12.600000381469727);
    context.Prepared.X=(float)((double)hx+left*projectedScale);
    if(!NativeLibrarySymbols.Draw(g,Item.Make(9,item.P(2),item.P(3),item.P(4),208,1.4f,1.4f),context,library,null,null))return false;
    double right=item.P(9)<10?31.200000762939453:(int)item.P(9)%100<20?39.20000076293945:51.20000076293945;
    context.Prepared.X=(float)((double)hx+right*projectedScale);
    return NativeLibrarySymbols.Draw(g,Item.Make(9,item.P(2),item.P(3),item.P(4),208,-1.4f,1.4f),context,library,null,null);
   }

   if(Custom(item)){
    if(item.P(6)>=45&&item.P(6)<=47){
     float aliasScale,aliasX,aliasY;NativeScanMusicVectors.HeadProjection(item,page,out aliasScale,out aliasX,out aliasY);
     var aliasContext=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};aliasContext.Prepared.Scale=aliasScale;aliasContext.Prepared.X=aliasX;aliasContext.Prepared.Y=aliasY;
     var body=CustomOctaveText(item,aliasContext,text);
     if(body==null)return false;
     text.Draw(g,body,body.Text,0,0,aliasContext.Scale/5,aliasContext.Scale,aliasContext);
     return true;
    }
    float customScale,cx,cy;NativeScanMusicVectors.HeadProjection(item,page,out customScale,out cx,out cy);
    var customContext=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};
    customContext.Prepared.Scale=customScale;customContext.Prepared.X=cx;customContext.Prepared.Y=cy;
    var symbol=Item.Make(9,item.P(2),item.P(3),item.P(4),item.P(6),.8330000042915344f);if(NativeLibrarySymbols.IsTextAlias((int)item.P(6))){symbol.Set(9,item.P(9));symbol.Set(11,item.P(11));}
    return NativeLibrarySymbols.Draw(g,symbol,customContext,library,text,cache);
   }
   if(item.P(6)>=4&&item.P(6)<=9&&item.P(6)==(int)item.P(6)&&page!=null){
    int head=(int)item.P(6);if(head==7)return true;
    if(item.P(20)!=0)return false;
    float projectedScale,x,y;try{NativeScanMusicVectors.HeadProjection(item,page,out projectedScale,out x,out y);}catch(NotSupportedException){return false;}
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};
    context.Prepared.Scale=projectedScale;context.Prepared.X=x;context.Prepared.Y=y;
    if(head<8)return NativePrimitiveGlyphs.Draw(g,context,library,cache,new PrimitiveGlyphRequest{Glyph=11+head,X=x,Y=y,Horizontal=1,Vertical=1});
    int glyphId=head==8?95:139;
    var symbol=Item.Make(9,item.P(2),item.P(3),item.P(4),glyphId,.8330000042915344f);
    return NativeLibrarySymbols.Draw(g,symbol,context,library,null,cache);
   }
   PrintedHeadRequest r;if(!Request(item,page,out r)||library==null)return false;Glyph glyph;if(!library.Symbols.TryGetValue(r.Glyph,out glyph))return false;
   float noteScale,headX,headY;
   NativeScanMusicVectors.HeadProjection(item,page,out noteScale,out headX,out headY);
   if(cache!=null&&!cache.Contains(r.Glyph)){
    float size=item.P(15)==0?(Math.Abs(item.P(4))>=100?.6000000238418579f:1):item.P(15);
    return NativePrimitiveGlyphs.Draw(g,page,library,cache,new PrimitiveGlyphRequest{Glyph=r.Glyph,X=headX,Y=headY,Horizontal=size,Vertical=size});
   }
   RenderResourceAudit.Record("printed glyph",r.Glyph+":"+glyph.Name,glyph.Source,"verified eligible notehead EPS projection");
   float scale=Single.Parse(r.Scale,CultureInfo.InvariantCulture),width=Single.Parse(r.Width,CultureInfo.InvariantCulture);
   bool inheritedDefault=(float)((double)noteScale/25)==1;
   if(inheritedDefault)width=NativePrintContext.DefaultLineWidth;var state=g.Save();try{using(var matrix=page.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);g.TranslateTransform(r.X,r.Y);g.ScaleTransform(scale,scale);
    using(var path=new GraphicsPath(FillMode.Alternate))using(var pen=new Pen(Color.Black,width)){pen.LineJoin=LineJoin.Round;bool have=false;PointF previous=new PointF();Action flush=()=>{if(path.PointCount>0){g.TrackedFillPath(Brushes.Black,path);g.TrackedDrawPath(pen,path);var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScoreGlyph(path,true,width,scale,r.X,r.Y,inheritedDefault);}path.Reset();have=false;};
     for(int j=0;j<glyph.Points.Length;j++){var p=PrintedGlyphDefinition.Vertex((short)glyph.Points[j].X,(short)glyph.Points[j].Y,glyph.Operations[j]);if(p.Operation>0){if(p.Operation!=3)flush();path.StartFigure();have=false;}var next=new PointF(p.X,p.Y);if(have)path.AddLine(previous,next);previous=next;have=true;}flush();
    }
   }finally{g.Restore(state);}return true;
  }
 }
}


