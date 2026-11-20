using System;using System.Drawing;using System.Collections.Generic;
namespace ScoreModern {
 public static class NativeTrillGlyph {
  // Shared by standalone Code7 and Code1's delegated trill. Relative
  // 13E8:0024..012B, without the optional continuation/accidental branches.
  public static PrimitiveGlyphRequest Prepare(float size,float x,float y,float scale){if(size==0)size=1;float h=(float)((double)size*1.100000023841858);return new PrimitiveGlyphRequest{Glyph=37,X=x,Y=(float)((double)y+scale*7),Horizontal=h,Vertical=h,WidthSelector=-1};}
  static float F(double value){return (float)value;}
  internal static PrimitiveGlyphRequest Accidental(int accidental,float size,float x,float y,float scale){
   float horizontal=F((double)size*scale*23);
   float vertical=F((double)size*(scale*7)*(accidental==1?3:3.5999999046325684));
   float glyphSize=F((double)size*.6499999761581421);
   return new PrimitiveGlyphRequest{Glyph=accidental+5,X=F((double)x+horizontal),Y=F((double)y+vertical),
    Horizontal=glyphSize,Vertical=glyphSize,WidthSelector=-1};
  }
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache){
   if(item.Code!=7||page==null||item.P(7)!=0||Single.IsNaN(item.P(5))||Single.IsInfinity(item.P(5))||Math.Abs(item.P(5))>32767)return false;
   for(int n=10;n<=20;n++)if(n!=11&&n!=12&&n!=13&&n!=15&&item.P(n)!=0)return false;
   if(NativeNotationView.UsesDat&&item.P(8)!=0)return false;
   int accidental=(int)item.P(8);
   if(item.P(8)!=accidental||accidental<0||accidental>3||
    (accidental!=0&&(library==null||!library.Symbols.ContainsKey(accidental+5))))return false;
   float end=item.P(6),size=item.P(5)==0?1:item.P(5);
   float x=F((double)page.Prepared.X+item.P(15)*150);
   var glyph=Prepare(size,x,page.Prepared.Y,page.Scale);
   if(end<=item.P(3)){
    if(!NativePrimitiveGlyphs.Draw(g,page,library,cache,glyph))return false;
   }else{
    if(NativeNotationView.UsesDat||Single.IsNaN(end)||Single.IsInfinity(end)||
     (Single.IsNaN(item.P(9))||Single.IsInfinity(item.P(9))||Math.Abs(item.P(9))>32767)||Single.IsNaN(item.P(12))||Math.Abs(item.P(12))>32767)return false;
    float wiggle=item.P(9)==0?1:item.P(9),waveSize=F((double)wiggle*size);if(!(waveSize>0))waveSize=1;
    float start=F((double)x+size*page.Scale*22.850000381469727),right=F((double)end*150);
    if(start>right)return false;
    float pitch=F((double)item.P(11)-wiggle+item.P(4)+2.200000047683716f);
    float y=page.Vertical(F((double)pitch+.4000000059604645f));
    bool rotated=item.P(13)!=0;
    if(library==null||!library.Symbols.ContainsKey(glyph.Glyph))return false;
    // Cached primitives print immediately. Expanded body points stay in
    // SCORE's vector buffer while a horizontal continuation prints directly.
    // Buffered rotated continuations retain the ordinary append order.
    bool bodyBeforeWave=rotated||cache!=null&&cache.Contains(glyph.Glyph);
    if(bodyBeforeWave&&!NativePrimitiveGlyphs.Draw(g,page,library,cache,glyph))return false;
    // SCORE emits cached accidentals immediately. Expanded points drain
    // after the continuation, which writes directly to the printer stream.
    if(accidental!=0&&(rotated||cache!=null&&cache.Contains(accidental+5))){
     if(!NativePrimitiveGlyphs.Draw(g,page,library,cache,Accidental(accidental,size,glyph.X,glyph.Y,page.Scale)))return false;
     accidental=0;
    }
    if(rotated){
     var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Prepared=page.Prepared,Projection=page.Projection};
     context.Prepared.X=start;context.Prepared.Y=y;
     var wave=Item.Make(4,item.P(2),item.P(3),pitch,pitch,end,-1,waveSize,0,0,0,item.P(12),item.P(13));
     if(!NativePrintedFilledWave.Draw(g,wave,context))return false;
    }else if(!NativePrintedFilledWave.DrawHorizontal(g,page,start,y,right,waveSize,(short)(int)item.P(12)!=0))return false;
    if(!bodyBeforeWave&&!NativePrimitiveGlyphs.Draw(g,page,library,cache,glyph))return false;
   }
   return accidental==0||NativePrimitiveGlyphs.Draw(g,page,library,cache,
    Accidental(accidental,size,glyph.X,glyph.Y,page.Scale));
  }
 }
}
