using System;using System.Drawing;using System.Collections.Generic;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintedClefs {
  static float F(double value){return (float)value;}
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,ScoreText text,HashSet<int> cache){
   if(item.Code!=3||page==null||library==null)return false;
   float selector=item.P(5),pitchControl=item.P(4);
   if(Single.IsNaN(selector)||Single.IsInfinity(selector)||Math.Abs(selector)>32767)return false;
   int decimalCode=Math.Abs((int)Math.Round(selector*100.0,MidpointRounding.AwayFromZero));
   bool octave=decimalCode!=0&&(decimalCode<100||decimalCode==180);
   bool tab=selector==44||selector==55||(selector==66||selector==66.1f);
   bool libraryDrawing=selector>=45&&selector<3000&&selector==(int)selector&&!tab;
   bool primitive=selector>=0&&selector<44||selector==1.81f||selector>-1&&selector<1;
   if(!octave&&!tab&&!libraryDrawing&&!primitive&&(selector<0||selector>5||selector!=(int)selector))return false;
   for(int n=4;n<=7;n++)if(Single.IsNaN(item.P(n))||Single.IsInfinity(item.P(n)))return false;
   for(int n=8;n<=20;n++)if(n!=15&&n!=10&&item.P(n)!=0)return false;
   if(Single.IsNaN(item.P(10))||item.P(10)<-32768||item.P(10)>32767||item.P(10)!=(int)item.P(10))return false;
   short widthSelector=(short)item.P(10);
   if(Single.IsNaN(item.P(15))||Single.IsInfinity(item.P(15)))return false;
   if(Math.Abs(pitchControl)>32767)return false;
   bool cue=Math.Abs((int)pitchControl)>=100;
   int glyph=tab?3:(int)selector==3?2:(int)selector;
   if(libraryDrawing){
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Projection=page.Projection,Prepared=page.Prepared};
    context.Prepared.X=F((double)page.Prepared.X+item.P(15)*150);
    var drawing=Item.Make(9,item.P(2),item.P(3),pitchControl,selector,item.P(6)==0?1:item.P(6),item.P(7),0,0,item.P(10));
    return NativeLibrarySymbols.Draw(g,drawing,context,library,text,cache,true);
   }
   if(!library.Symbols.ContainsKey(glyph))return false;
   float h=item.P(6)==0?1:item.P(6),v=item.P(7)==0?h:item.P(7),offset=(int)selector>5&&!octave?0:21;
   if(tab)return DrawTab(g,item,page,library,h,v);
   if(cue){
    h=F((double)h*.800000011920929f);
    v=h;
    offset=F((double)offset+(glyph==0||glyph==5?.3499999940395355f:glyph==1?8.75f:5.599999904632568f));
   }
   if((int)selector==3)offset=F((double)offset+14);
   if(octave){
    if(text==null)return false;
    float height=decimalCode==180?0:decimalCode==88||decimalCode==15?10.949999809265137f:-5.25f;
    float shift=decimalCode==180?.05999999865889549f:decimalCode==88?.11999999731779099f:decimalCode==15?.08500000089406967f:.07999999821186066f;
    int number=decimalCode==180?8:decimalCode/10;if(number==1)number=15;
    float pitch=F((double)height*h+(double)(pitchControl%100)+3);
    float position=F((double)page.Scale*h*shift+item.P(3));
    var context=new NativeNotationContext{SourceScore=page.SourceScore,Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};context.Prepared.X=F(position*150.0);context.Prepared.Y=page.Vertical(pitch);
    var body=Item.Make(16,item.P(2),position,pitch,1,F((double)h*.699999988079071f),0,1003,0,0,0,1,0);string numeral=number.ToString(CultureInfo.InvariantCulture);text.Set(body,numeral,page.Scale);
    RenderResourceAudit.Record("printed clef numeral",numeral,text.RootDirectory,"recovered choral tenor text request; external metric face3");text.Draw(g,body,body.Text,0,0,page.Scale/5,page.Scale,context);
    // Original 0CC1:0245 prepares the numeral integer cache; 0280..02CB restores only the clef fields. P10 remains zero.
    widthSelector=0;
   }
   float effective=F((double)page.Scale*h),scale=effective/25,y=F((double)page.Scale*offset+page.Prepared.Y);
   float x=F((double)page.Prepared.X+item.P(15)*150);
   if(h!=v||h<0)return NativePrimitiveGlyphs.Draw(g,page,library,cache,new PrimitiveGlyphRequest{Glyph=glyph,X=x,Y=y,Horizontal=h,Vertical=v,WidthSelector=widthSelector});
   return NativePrintedSymbols.DrawGlyph(g,page,library,glyph,x,y,PrintedGlyphDefinition.Fixed3(scale),h,cache,widthSelector);
  }
  static bool DrawTab(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,float h,float v){
   int selector=(int)item.P(5);float factor=selector==44?.6000000238418579f:selector==55?.8299999833106995f:1;
   h=F((double)F(.10000000149011612*h)*factor);
   v=F((double)F(.10000000149011612*v)*factor);
   var context=new NativeNotationContext{Staff=page.Staff,Projection=page.Projection,RawBase=page.RawBase,Prepared=page.Prepared};
   context.Prepared.X=F((double)page.Prepared.X+item.P(15)*150);
   if(selector!=66)context.Prepared.Y=page.Vertical(F((double)item.P(4)+(selector==44?1.2000000476837158f:.3499999940395355f)));
   var drawing=Item.Make(9,item.P(2),item.P(3),item.P(4),3,h,v,0,0,item.P(10));
   return NativeLibrarySymbols.Draw(g,drawing,context,library,null,null);
  }
 }
}
