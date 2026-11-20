using System;using System.Collections.Generic;using System.Drawing;
namespace ScoreModern {
 public static class NativeKeySignatures {
  static float F(double x){return (float)x;}
  // Key signature path, relative 0B0D:0008..033A.
  public static List<PrimitiveGlyphRequest> Prepare(Item i,NativeNotationContext page){
   if(page==null||i.Code!=17||Single.IsNaN(i.P(5))||Single.IsInfinity(i.P(5))||Math.Abs(i.P(5))>32767||Single.IsNaN(i.P(6))||Single.IsInfinity(i.P(6))||Math.Abs(i.P(6))>32767)return null;
   for(int j=17;j<=20;j++)if(i.P(j)!=0)return null;if(i.P(14)!=0&&i.P(14)!=1)return null;if(Single.IsNaN(i.P(15))||Single.IsInfinity(i.P(15))||Math.Abs(i.P(15))>32767)return null;
   int signed=(int)i.P(5),count=Math.Abs(signed),clef=(int)i.P(6),glyph=signed<0?6:7;
   if(count>=100){glyph=8;count%=100;}count=Math.Max(1,count);
   float origin=F((double)page.Prepared.X+i.P(15)*150),scale=page.Scale,x=F(scale*15.0+origin),gap=F(scale*16.799999237060547),size=1;
   if(i.P(7)!=0)gap=F(i.P(7)*(double)gap);
   if(clef>=100){size=.800000011920929f;gap=F(size*(double)gap);clef-=100;}
   if(i.P(8)!=0){
    var explicitKey=new List<PrimitiveGlyphRequest>();
    foreach(int field in new[]{8,9,10,11,12,13,16}){
     float value=i.P(field);
     if(value==0)break;
     if(value!=(int)value||value<100||value>399)return null;
     int word=(int)value;
     explicitKey.Add(new PrimitiveGlyphRequest{Glyph=word/100+5,X=x,Y=F((word%100*7)*(double)scale+page.Prepared.Y),Horizontal=size,Vertical=size});
     x=F((double)x+gap);
    }
    return explicitKey;
   }
   if(i.P(5)<-107||i.P(5)>107||i.P(5)!=(int)i.P(5)||count>7||clef<0||clef>3)return null;
   for(int field=9;field<=13;field++)if(i.P(field)!=0)return null;
   if(i.P(16)!=0)return null;
   float[] flats={49,35,42,56},sharps={77,63,70,35};
   float y=F((signed<0?flats[clef]:sharps[clef])*(double)scale+page.Prepared.Y),threshold=F((clef==0?84.5:clef==1?70.5:77.5)*scale+page.Prepared.Y),span=F(scale*49.0),signedScale=signed<0?-scale:scale,up=F(signedScale*28.0),down=F(-signedScale*21.0);
   int direction=signed>=0&&clef==3?1:-1;var result=new List<PrimitiveGlyphRequest>();
   for(int j=0;j<count;j++){result.Add(new PrimitiveGlyphRequest{Glyph=glyph,X=x,Y=y,Horizontal=size,Vertical=size,WidthSelector=0});direction=-direction;x=F((double)x+gap);y=F((double)y+(direction>=0?down:up));if(y>threshold){y=F((double)y-span);direction=-direction;}}
   return result;
  }
  public static bool Draw(Graphics g,Item i,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache){var plan=Prepare(i,page);if(plan==null)return false;foreach(var r in plan){Glyph glyph;if(library==null||!library.Symbols.TryGetValue(r.Glyph,out glyph))return false;}// Cached calls print immediately. The original flushes the expanded paths after the key.
   using(var buffer=NativePrintedPointBuffer.Begin(g)){if(cache!=null)foreach(var r in plan)if(cache.Contains(r.Glyph)&&!NativePrimitiveGlyphs.Draw(g,page,library,cache,r))return false;
   foreach(var r in plan)if((cache==null||!cache.Contains(r.Glyph))&&!NativePrimitiveGlyphs.Draw(g,page,library,null,r))return false;buffer.Complete();return true;}}
 }
}
