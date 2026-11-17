using System;using System.Collections.Generic;using System.Drawing;
namespace ScoreModern {
 public static class NativeNoteAttachments {
  static float F(double v){return (float)v;}static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  // Original Code1 mark-placement categories, DS:11A0; these select the
  // shared stem/staff-clearance algorithm, not per-score positional offsets.
  static readonly int[] Category={0,1,1,1,-1,0,0,0,-1,-1,-1,1,1,1,1,1,1,1,1,-1,1,1,1,1,1,-1};
  public static List<PrimitiveGlyphRequest> Accidental(Item item,NativeNotationContext page,bool datScan=false){
   bool dat=datScan||NativeNotationView.UsesDat;
   var note=dat?NativeNoteDetails.PrepareDat(item,page):NativeNoteDetails.Prepare(item,page);
   if(note==null)return null;
   float control=item.P(5);
   if(Single.IsNaN(control)||control<0||control>=200)return null;
   int value=(int)control,accidental=value%10;
   var result=new List<PrimitiveGlyphRequest>();
   if(accidental==0)return result;
   // Cached requests print immediately; expanded requests remain in the
   // per-item point buffer. Their order is verified by N125–N130 traces.
   float fraction=F(control%1);
   if(fraction>.8999999761581421)fraction=F(.8999999761581421-fraction);
   float offset=F((double)fraction*note.Scale*60);
   result.Add(new PrimitiveGlyphRequest{
    Glyph=control>=100||accidental>5?139+accidental:accidental+5,
    X=F((double)(item.P(10)!=10?note.NoteX:page.Prepared.X)-offset),
    Y=note.NoteY,Horizontal=1,Vertical=1,WidthSelector=0
   });
   return result;
  }
  // Marks retain recovered direction/length and use P13 in horizontal
  // SCORE units; nonzero P14 replaces the fractional P11 displacement.
  public static List<PrimitiveGlyphRequest> Marks(Item i,NativeNotationContext page,bool datScan=false){var n=datScan?NativeNoteDetails.PrepareDat(i,page):NativeNoteDetails.Prepare(i,page);if(n==null)return null;var result=new List<PrimitiveGlyphRequest>();float markControl=i.P(11);float markSize=i.P(18)==0?1:i.P(18);if(Single.IsNaN(markSize)||Single.IsInfinity(markSize)||Math.Abs(markSize)>32767)return null;
   float magnitude=Math.Abs(markControl);
   if(magnitude>=26&&magnitude<30){
    int multi=(int)magnitude;
    int direction=n.StemKind==1?1:-1;
    bool upSide=markControl<0?direction==1:direction==-1;
    float delta=markControl<0?F((double)n.SignedStemLength+direction*1.5*n.StaffStep):F(-direction*2.0*n.StaffStep);
    float multiFraction=i.P(14)!=0?i.P(14):F(10.0*F(markControl%1));
    if(i.P(14)==0&&n.StemKind==1)multiFraction=-multiFraction;
    delta=F((double)multiFraction*n.StaffStep+delta);
    float markX=F((double)n.NoteX+i.P(13)*150);
    if(multi==29){
     result.Add(NativeTrillGlyph.Prepare(markSize,markX,F((double)n.NoteY+delta-n.Scale*7),n.Scale));
     return result;
    }
    if(markControl>0&&Math.Abs(n.Pitch)%2==1&&(direction==1?n.Pitch>=5:n.Pitch<=9))
     delta=F((double)delta-direction*n.StaffStep);
    int count=multi-24;
    float dotX=F((double)markX+(2.0-(count-1)*4.300000190734863)*n.Scale);
    float dotY=F((double)n.NoteY+delta);
    for(int k=0;k<count;k++){
     result.Add(new PrimitiveGlyphRequest{Glyph=24,X=dotX,Y=dotY,Horizontal=F((double).72f*markSize),Vertical=F((double)(upSide?.72f:-.72f)*markSize)});
     dotX=F((double)dotX+8.600000381469727*n.Scale);
    }
    return result;
   }
   if(Single.IsNaN(i.P(11))||i.P(11)<=-26||i.P(11)>=26)return null;int mark=Math.Abs((int)i.P(11));if(mark==0)return result;int category=Category[mark],stem=n.StemKind;float step=n.StaffStep,h=F((mark==7?.7200000286102295:1)*markSize),v=h,x=F((double)n.NoteX+i.P(13)*150),d=F((double)step*2);
   if(category==1){d=F((double)step+d);float boundary=F((double)page.RawBase-n.NoteY);if(i.P(11)>=0){if(stem==1)d=F((double)n.SignedStemLength+d-step);boundary=F((double)step*13+boundary);stem=1;if(d<boundary)d=boundary;}else{d=F((double)d-step*6);if(stem==2)d=F((double)d+n.SignedStemLength+step);boundary=F((double)step+boundary);stem=2;if(mark==14)v=-v;if(d>boundary)d=boundary;}}
   else{if(stem==1)d=-d;if(i.P(11)<=0){float adjust=F((double)step*page.Scale*.5/n.Scale);if(stem==1)adjust=-adjust;d=F((double)d-adjust);d=F((double)n.SignedStemLength-d);}if((stem==1&&i.P(11)>0)||(stem==2&&i.P(11)<=0))v=-v;
    if((mark==6||mark==7)&&i.P(11)>=0&&Math.Abs(n.Pitch)%2==1){float snap=0;if(n.Pitch<=9&&stem==2)snap=step;if(n.Pitch>=5&&stem==1)snap=-step;d=F((double)d+snap);}}
   if(mark==7)x=F((double)x+2*n.Scale);
   float fraction=i.P(14)!=0?i.P(14):F(10.0*F(i.P(11)%1));if(!(i.P(14)!=0)&&stem==1&&category!=1)fraction=-fraction;d=F((double)fraction*step+d);float y=F((double)n.NoteY+d);
   if(mark>=20&&mark<=23){
    var trill=NativeTrillGlyph.Prepare(h,x,F((double)y-step),n.Scale);
    result.Add(trill);
    if(mark>20)result.Add(NativeTrillGlyph.Accidental(mark-20,h,x,trill.Y,n.Scale));
   }
   else if(mark==24||mark==25){result.Add(new PrimitiveGlyphRequest{Glyph=mark==24?38:119,X=x,Y=y,Horizontal=h,Vertical=v});}
   else{int glyph=mark<=3?mark+5:mark+17;if(mark<=3){x=F((double)n.Scale*18+x);h=F(.6499999761581421*h);v=h;}result.Add(new PrimitiveGlyphRequest{Glyph=glyph,X=x,Y=y,Horizontal=h,Vertical=v,WidthSelector=0});}return result;
  }
  internal static List<RecoveredFragments.PrimitiveTextPoint> HeavyLine(Item item,NativeNotationContext page){
   if(item.P(11)!=99)return null;
   var note=NativeNoteDetails.PrepareDat(item,page);if(note==null)return null;
   float start=item.P(13),end=item.P(14);
   if(Single.IsNaN(start)||Single.IsNaN(end)||Math.Abs(start)>32767||Math.Abs(end)>32767)return null;
   float x=note.NoteX,y=note.NoteY,scale=note.Scale;
   // The default start scales with the note. Entered offsets and the default
   // end are horizontal SCORE units, independent of staff size.
   float left=F((double)x+(start==0?13.8*scale:150.0*start));
   float right=F((double)x+150.0*(end==0?10:end));
   if(right<=left)return null;
   float bottom=F((double)y-2.8*scale),top=F((double)y+2.8*scale);
   return NativeDatSpecialItems.Box(left,bottom,right,top,true);
  }
  internal static bool HasFingeringControl(Item item){
   float mark=Math.Abs(item.P(11));
   return mark>=30&&mark<=1056&&mark!=99;
  }
  internal static Item Fingering(Item item,NativeNotationContext page,ScoreText text,bool dat,out NativeNotationContext context,float? outputScale=null){
   context=null;
   float mark=item.P(11);
   int stemControl=(int)item.P(5)/10%10;
   if(text==null||!HasFingeringControl(item)||
    item.P(5)<0||item.P(5)>=200||(stemControl!=1&&stemControl!=2)||
    item.P(6)<-1||item.P(6)>=3000||item.P(6)!=(int)item.P(6))return null;
   var note=dat?NativeNoteDetails.PrepareDat(item,page):NativeNoteDetails.Prepare(item,page);
   if(note==null)return null;
   int number=Math.Abs((int)mark)-30;
   int direction=note.StemKind==1?1:-1;
   float displacement=item.P(14)!=0?item.P(14):F(-direction*10.0*F(mark%1));
   float steps=mark<0?-1+1.5f*direction:-1-2.5f*direction;
   float stem=mark<0?note.SignedStemLength:0;
   float size=F(.699999988079071*(item.P(18)==0?1:item.P(18)));
   if(size==0||Single.IsInfinity(size)||Single.IsNaN(size))return null;
   // SCORE keeps the fingering font at staff size for cue and grace notes.
   context=GlyphContext(page,outputScale??page.Scale);
   context.Prepared.X=F((double)note.NoteX+note.Scale*4+item.P(13)*150);
   // 0D:141C..143B stores the relative ordinate before adding the note Y.
   float ordinate=F((double)stem+note.StaffStep*steps);
   ordinate=F((double)displacement*note.StaffStep+ordinate);
   context.Prepared.Y=F((double)note.NoteY+ordinate);
   float spacing=number>=10&&item.P(14)!=0?item.P(14):1;
   var body=Item.Make(16,item.P(2),item.P(3),item.P(4),spacing,size,0,1000,0,0,0,1,0);
   string value=NativeNumberDisplay.Value(number);
   text.Set(body,value,note.Scale);
   return body;
  }
  internal static bool DrawFingering(Graphics graphics,Item item,NativeNotationContext page,ScoreText text){
   NativeNotationContext context;
   var body=Fingering(item,page,text,false,out context);
   if(body==null)return false;
   text.Draw(graphics,body,body.Text,0,0,context.Scale/5,context.Scale,context);
   return true;
  }
  static NativeNotationContext GlyphContext(NativeNotationContext page,float scale){var p=page.Prepared;p.Scale=scale;return new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Projection=page.Projection,Prepared=p};}
  static bool Draw(Graphics g,Item i,NativeNotationContext page,DrawingLibrary lib,HashSet<int> cache,List<PrimitiveGlyphRequest> requests){if(requests==null)return false;var n=NativeNoteDetails.Prepare(i,page);var context=GlyphContext(page,n.Scale);foreach(var r in requests){Glyph glyph;if(lib==null||!lib.Symbols.TryGetValue(r.Glyph,out glyph))return false;}foreach(var r in requests){if(r.Glyph<=44){if(!NativePrimitiveGlyphs.Draw(g,context,lib,cache,r))return false;}else{var placed=context.Prepared;placed.X=r.X;placed.Y=r.Y;var glyphContext=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Projection=page.Projection,Prepared=placed};var body=Item.Make(9,i.P(2),i.P(3),i.P(4),r.Glyph,r.Horizontal,r.Vertical);if(!NativeLibrarySymbols.Draw(g,body,glyphContext,lib,null,cache))return false;}}return true;}
  public static bool DrawAccidental(Graphics g,Item i,NativeNotationContext page,DrawingLibrary lib,HashSet<int> cache){return Draw(g,i,page,lib,cache,Accidental(i,page));}
  public static bool DrawMarks(Graphics g,Item i,NativeNotationContext page,DrawingLibrary lib,HashSet<int> cache){
   var line=HeavyLine(i,page);
   if(line==null)return Draw(g,i,page,lib,cache,Marks(i,page));
   var points=line.ConvertAll(p=>new BarlinePoint(p.X,p.Y,p.Operation));
   points.Add(new BarlinePoint(line[1].X,line[1].Y,0));
   NativeNoteDetails.Path(g,page,points,NativePrintContext.DefaultLineWidth);
   return true;
  }
 }
}
