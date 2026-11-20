using System;using ScoreModern.RecoveredFragments;using System.Collections.Generic;
namespace ScoreModern {
 public static class NativeDatClefs {
  static float F(double x){return (float)x;}static short W(float x){return unchecked((short)(int)Math.Truncate(x));}static bool Present(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0)!=0;}
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext page,NativeDatGlyphs dat,DrawingLibrary library,ScoreText text,out short? font){
   font=null;for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT clef outside finite word domain.");
   for(int n=12;n<=20;n++)if(n!=15&&i.P(n)!=0)throw new NotSupportedException("DAT clef extended fields need verification.");
   int glyph=W(i.P(5));bool external=glyph>=45&&glyph<3000&&glyph!=55&&glyph!=66;if(glyph<0||glyph>44&&glyph!=55&&glyph!=66&&!external)throw new NotSupportedException("DAT clef selector needs special resource verification.");float h=Present(i.P(6))?i.P(6):1,v=Present(i.P(7))?i.P(7):h,x=F((double)page.Prepared.X+i.P(15)*150),y=page.Prepared.Y;
   if(glyph==19&&i.P(4)==0&&i.P(6)==0&&i.P(7)==0&&i.P(10)==0&&i.P(15)==0)return new List<PrimitiveTextPoint>();
   if(external){
    if(library==null||i.P(5)!=glyph)throw new NotSupportedException("DAT clef library controls outside the measured route.");
    var drawing=Item.Make(9,i.P(2),i.P(3),i.P(4),glyph,h,v,0,0,i.P(10));
    drawing.Set(15,i.P(15));
    if(NativeLibrarySymbols.IsTextAlias(glyph)){
     short face;
     var textPoints=NativeLibrarySymbols.ScanTextAlias(drawing,page,text,out face);
     font=face;
     return textPoints;
    }
    return NativeLibrarySymbols.ScanGlyph(drawing,page,library);
   }
   if(glyph<=5){float offset=21;if(Math.Abs((int)W(i.P(4)))>=100){h=F((double)h*.800000011920929f);v=h;float[] shifts={.3499999940395355f,8.75f,5.599999904632568f,5.599999904632568f,5.599999904632568f,.3499999940395355f};offset=F((double)offset+shifts[glyph]);}if(glyph==3){offset=F((double)offset+14);glyph=2;}y=F((double)page.Scale*offset+y);}
   if(glyph==44||glyph==55||glyph==66){if(text==null)throw new NotSupportedException("DAT TAB clef needs external drawing resources.");h=F((double).10000000149011612f*h);v=F((double).10000000149011612f*v);if(glyph!=66){float factor=glyph==44?.6000000238418579f:.8299999833106995f;h=F((double)factor*h);v=F((double)factor*v);y=page.Vertical(F((double)i.P(4)+(glyph==44?1.2000000476837158f:.3499999940395355f)));}var body=Item.Make(9,i.P(2),i.P(3),i.P(4),3,h,v,0,0,i.P(10));var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=x;context.Prepared.Y=y;return NativeLibrarySymbols.ScanGlyph(body,context,new DrawingLibrary(text.RootDirectory));}
   var points=new List<PrimitiveTextPoint>();int code=Math.Abs((int)Math.Round(i.P(5)*100.0,MidpointRounding.AwayFromZero));
   if(code!=0&&(code<100||code==180)){
    float height=-5.25f,shift=.07999999821186066f;if(code==88||code==15){height=10.949999809265137f;shift=code==88?.11999999731779099f:.08500000089406967f;}if(code==180){code=80;height=0;shift=.05999999865889549f;glyph=1;}
    float pitch=F((double)height*h+(double)(i.P(4)%100)+3),position=F((double)page.Scale*h*shift+i.P(3)),nx=F(position*150.0),ny=page.Vertical(pitch),size=F((double)h*.699999988079071f);int number=code/10;if(number==1)number=15;if(text==null)throw new NotSupportedException("DAT clef numeral needs external text resources.");
    var body=Item.Make(16,i.P(2),position,pitch,1,size,0,1003,0,0,0,1,0);string value=number.ToString(System.Globalization.CultureInfo.InvariantCulture);text.Set(body,value,page.Scale);var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=nx;context.Prepared.Y=ny;points.AddRange(text.ScanStrokeText(body,value,context,0));font=3;
   }
   points.AddRange(dat.Expand(glyph,h,v,page.Scale,x,y));return points;
  }
 }
}
