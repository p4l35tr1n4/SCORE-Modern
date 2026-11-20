using System;
using System.Collections.Generic;
using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintedOrnaments {
  static float F(double v){return (float)v;}
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,ScoreText text,HashSet<int> cache){
   if(page==null||library==null||text==null||NativeNotationView.UsesDat)return false;
   int type=(int)item.P(7);
   if(item.P(7)!=type)return false;
   if(type==20||type==-88){
    for(int n=13;n<=20;n++)if(n!=13&&n!=15&&item.P(n)!=0)return false;
    return DrawPedal(g,item,page,library,cache);
   }
   if(type==1){
    var harp=NativeDatOrnaments.Harp(item,page,true,NativeLibraryLocation.IsScor4(library.Root));
    NativeNoteDetails.Path(g,page,harp[0].ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),NativePrintContext.Width(2));
    NativeNoteDetails.Path(g,page,harp[1].ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),NativePrintContext.Width(2));
    var points=harp[2].ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation));
    for(int n=0;n<points.Count;n+=5){var pedal=points.GetRange(n,5);pedal.Add(pedal[1]);NativeNoteDetails.Path(g,page,pedal,NativePrintContext.DefaultLineWidth);}
    return true;
   }
   if(type==21||type==22){
    foreach(int n in new[]{10,13,14,15,16,17,18,19,20})if(item.P(n)!=0)return false;
    var points=NativeDatOrnaments.Prepare(item,page,null,library,text);
    var path=new List<BarlinePoint>();
    foreach(var point in points)path.Add(new BarlinePoint(point.X,point.Y,point.Operation));
    NativeNoteDetails.Path(g,page,path,NativePrintContext.DefaultLineWidth);
    return true;
   }
   string label=type==8?"8va":type==-8?"8ba":type==15?"15ma":type==88?"8":null;
   if(label==null)return false;
   foreach(int n in new[]{14,16,17,18,19,20})if(item.P(n)!=0)return false;
   float itemSize=item.P(5)==0?1:item.P(5);
   if(Single.IsNaN(itemSize)||Single.IsInfinity(itemSize)||Math.Abs(itemSize)>32767)return false;
   float size=F(F((double)itemSize*.75)*1.100000023841858);
   var body=Item.Make(16,item.P(2),item.P(3),item.P(4),1,size,0,1002,0,type==88?0:.6000000238418579f,0,1,0);
   if(type!=88)body.Set(11,item.P(11));
   text.Set(body,label,page.Scale);
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Prepared=page.Prepared,Projection=page.Projection};
   context.Prepared.X=F((double)page.Prepared.X+item.P(15)*150);
   text.Draw(g,body,body.Text,0,0,page.Scale/5,page.Scale,context);
   if((short)item.P(6)==0)return true;
   float height=F((double)item.P(4)+(type==88?F((double)F((double)itemSize*1.100000023841858)*1.5+.4000000059604645):.4000000059604645));
   context.Prepared.X=F((double)page.Prepared.X+item.P(15)*150+itemSize*page.Scale*(type==15?62:type==88?18:43));
   context.Prepared.Y=page.Vertical(height);
   float dash=item.P(8),gap=item.P(9);
   if(dash==0){dash=F((double)itemSize*page.Scale/25);gap=F((double)dash*1.5);}
   var line=Item.Make(4,item.P(2),item.P(3),height,height,item.P(6),1,dash,gap,item.P(10),item.P(11),item.P(12),item.P(13));
   return NativePrintedLines.Draw(g,line,context);
  }
  static bool DrawPedal(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache){
   Glyph pressed,pedal;
   if(!library.Symbols.TryGetValue(49,out pressed)||!library.Symbols.TryGetValue(48,out pedal))return false;
   float factor=item.P(9)==0?.4f:F((double)item.P(9)*.4f);
   float scale=F((double)factor*page.Scale),x=F((double)page.Prepared.X+item.P(15)*150),y=page.Prepared.Y;
   float step=F((double)scale*53);
   int pedals=(short)item.P(5);
   var points=new List<BarlinePoint>();
   Action<int,Glyph,float,short> symbol=(id,glyph,xx,selector)=>{
    // The original pressed-pedal calls at 1C0C:093D/09B2/0A0E retain
    // the item width word. The expanded glyph path decrements it at
    // 0E38:0423, while the cached call uses it directly.
    if(cache==null||!cache.Contains(id)){
     AppendGlyph(points,glyph,xx,y,scale);
     NativeNoteDetails.Path(g,page,points,NativePrintContext.Width(unchecked((short)(selector-1))));
     points.Clear();return;
    }
    float projection=F((double)scale/6.25),width=F(NativePrintContext.RawWidth(selector)/(double)projection);
    var culture=System.Globalization.CultureInfo.InvariantCulture;
    width=Single.Parse(PrintedGlyphDefinition.Fixed3(width),culture);
    projection=Single.Parse(PrintedGlyphDefinition.Fixed3(projection),culture);
    NativeLibrarySymbols.DrawCached(g,page,glyph,new LibrarySymbolPlan{Glyph=id},projection,width,6.25f,
     NativeNotationContext.Word(xx,true),NativeNotationContext.Word(y,false));
   };
   for(int n=0;n<3;n++){
    int digit=n==0?pedals/100:n==1?pedals%100/10:pedals%10;
    if(digit!=0)symbol(49,pressed,F((double)x+n*step),(short)item.P(10));
   }
   NativeNoteDetails.Path(g,page,points,NativePrintContext.DefaultLineWidth);
   points.Clear();
   symbol(48,pedal,x,0);
   NativeNoteDetails.Path(g,page,points,NativePrintContext.Width(-1));
   if((short)item.P(6)!=0){
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Prepared=page.Prepared,Projection=page.Projection};
    context.Prepared.X=x;
    float left=item.P(8)==0?item.P(3):item.P(8);
    if(item.P(8)!=0)context.Prepared.X=F((double)left*150);
    var line=Item.Make(4,item.P(2),left,item.P(4),item.P(4),item.P(6),0,0,0,item.P(10),item.P(11),item.P(12),item.P(13));
    if(!NativePrintedLines.Draw(g,line,context))return false;
   }
   RenderResourceAudit.Record("printed pedal", "48/49",pedal.Source,"original pedal bank and continuation");
   return true;
  }
  static void AppendGlyph(List<BarlinePoint> points,Glyph glyph,float x,float y,float scale){
   for(int n=0;n<glyph.Points.Length;n++)
    points.Add(new BarlinePoint(F((double)x+glyph.Points[n].X*(double)scale),
     F((double)y+glyph.Points[n].Y*(double)scale),glyph.Operations[n]));
  }
 }
}

