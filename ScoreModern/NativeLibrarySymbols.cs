using System;using System.IO;using System.Collections.Generic;using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class LibrarySymbolPlan {public int Glyph;public float X,Y,H,V,Rotation;public bool Cached;}
 public static class NativeLibrarySymbols {
  static readonly int[] AliasIds={66,67,78,45,46,47,63,64,65,68,72,73,79,80,81,85,86,111,113,114};
  static readonly string[] AliasText={"pizz.","arco","a tempo","8va","8ba","15ma","cresc.","dim.","rit.","accel.","gliss.","molto","espr.","meno","pi>>u","subito","poco","decresc.","rall.","mosso"};
  static float F(double v){return (float)v;}static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  public static bool IsTextAlias(int glyph){return Array.IndexOf(AliasIds,glyph)>=0;}
  internal static Item AliasTextItem(Item i,NativeNotationContext page,ScoreText text){
   int number=(int)i.P(5),alias=Array.IndexOf(AliasIds,number);if(alias<0)return null;
   float h=F(i.P(6)%100);if(!Present(h))h=1;float v=Present(i.P(7))?i.P(7):h,size=number<50?1:1.0499999523162842f;
   short font=(short)(int)i.P(11);var body=Item.Make(16,i.P(2),i.P(3),i.P(4),number<50?1:size,F((double)h*size),number<50?i.P(7):F((double)v*size),font!=0?font:alias<3?1000:1002,0,number<50?.6000000238418579f:0,font==0?i.P(11):0,1,0);
   text.Set(body,number==81?"piu":AliasText[alias],page.Scale);return body;
  }
  public static List<PrimitiveTextPoint> ScanTextAlias(Item i,NativeNotationContext page,ScoreText text){short ignored;return ScanTextAlias(i,page,text,out ignored);}
  public static List<PrimitiveTextPoint> ScanTextAlias(Item i,NativeNotationContext page,ScoreText text,out short finalFont,short previousFont=0){float ignoredX;short ignoredCode;return ScanTextAlias(i,page,text,out finalFont,out ignoredX,out ignoredCode,previousFont);}
  internal static List<PrimitiveTextPoint> ScanTextAlias(Item i,NativeNotationContext page,ScoreText text,out short finalFont,out float finalX,out short lastCode,short previousFont=0){
   finalFont=0;
   int number=(int)i.P(5),alias=Array.IndexOf(AliasIds,number);
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT alias input outside the finite signed-word domain.");
   if(alias<0||text==null)throw new NotSupportedException("DAT library text alias controls outside the measured route.");
   foreach(int n in new[]{12,14,17,18,19,20})if(i.P(n)!=0)throw new NotSupportedException("DAT library text alias auxiliary controls need verification.");
   float h=F(i.P(6)%100);if(!Present(h))h=1;float v=Present(i.P(7))?i.P(7):h;float size=number<50?1:1.0499999523162842f;
   var body=Item.Make(16,i.P(2),i.P(3),i.P(4),number<50?1:size,F((double)h*size),number<50?i.P(7):F((double)v*size),(short)(int)i.P(11)!=0?(short)(int)i.P(11):alias<3?1000:1002,0,number<50?.6000000238418579f:0,(short)(int)i.P(11)==0?i.P(11):0,1,0);
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=F((double)page.Prepared.X+i.P(15)*150);
   text.Set(body,number==81?"piu":AliasText[alias],page.Scale);var layout=text.PrepareLayout(body,number==81?"piu":body.Text,page.Scale,previousFont);finalFont=layout.FinalFont;lastCode=0;foreach(var token in ScoreText.RenderingTokens(layout,(short)(int)body.P(8),previousFont))if(token.Code<1000)lastCode=token.Code;var points=number==81?text.ScanGraveLibraryAlias(body,context,previousFont,out finalX):text.ScanStrokeText(body,body.Text,context,previousFont,out finalFont,out finalX);if(Present(i.P(9))){for(int n=0;n<points.Count;n++){var q=points[n];q.X=NativeNotationContext.Word(q.X,true);q.Y=NativeNotationContext.Word(q.Y,false);points[n]=q;}NativeDatLines.Rotate(points,i.P(9),context.Prepared.X,context.Prepared.Y);}return points;
  }

  public static List<PrimitiveTextPoint> ScanGlyph(Item i,NativeNotationContext page,DrawingLibrary library){
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT library input outside the finite signed-word domain.");
   if(i.P(5)<0||i.P(5)>=3000||IsTextAlias((int)i.P(5))&&(short)(int)i.P(11)>=0)throw new NotSupportedException("DAT library selector outside the recovered glyph route.");
   for(int n=16;n<=20;n++)if(i.P(n)!=0&&n!=16&&n!=20)throw new NotSupportedException("DAT library extended controls need verification.");
   int number=(int)i.P(5);Glyph glyph;if(!library.Symbols.TryGetValue(number,out glyph))throw new InvalidDataException("External library glyph unavailable.");
   float h=F(i.P(6)%100);if(!Present(h))h=1;float v=Present(i.P(7))?i.P(7):h;
   // Expanded Code9 DRW coordinates use full scale below50; primitive/cache factors are separate.
   h=number>=50?F(.30000001192092896*h):h;v=number>=50?F(.30000001192092896*v):v;h=F((double)h*page.Scale);v=F((double)v*page.Scale);
   float x=F((double)page.Prepared.X+i.P(15)*150),y=page.Prepared.Y;var points=new List<PrimitiveTextPoint>();
   for(int n=0;n<glyph.Points.Length;n++)points.Add(new PrimitiveTextPoint{X=NativeNotationContext.Word(F((double)glyph.Points[n].X*h+x),true),Y=NativeNotationContext.Word(F((double)glyph.Points[n].Y*v+y),false),Operation=glyph.Operations[n]});
   if((short)(int)i.P(9)!=0)NativeDatLines.Rotate(points,i.P(9),x,y);
   short width=unchecked((short)(int)i.P(8)),outline=unchecked((short)((short)(int)i.P(10)-(number>=45?1:0)));if(width==0&&outline>0)width=unchecked((short)(outline*101));NativeDatLines.Thickness(points,width);return points;
  }
  static bool PrintedThicknessSupported(Item i){
   float copies=i.P(8),width=i.P(10);
   return copies>=-1&&copies<=32767&&copies==(int)copies&&width>=-32768&&width<=32767&&width==(int)width;
  }
  public static LibrarySymbolPlan Prepare(Item i,NativeNotationContext page,bool cached){if(page==null||i.Code!=9||i.P(5)<0||i.P(5)>=3000||!PrintedThicknessSupported(i)||(i.P(11)!=0&&!(IsTextAlias((int)i.P(5))&&(short)(int)i.P(11)<0))||i.P(12)!=0||i.P(13)!=0||i.P(14)!=0)return null;for(int n=16;n<=20;n++)if(i.P(n)!=0)return null;int glyph=(int)i.P(5);if(Array.IndexOf(AliasIds,glyph)>=0&&(short)(int)i.P(11)>=0)return null;float h=F(i.P(6)%100);if(!Present(h))h=1;float v=Present(i.P(7))?i.P(7):h;return new LibrarySymbolPlan{Glyph=glyph,X=F((double)page.Prepared.X+i.P(15)*150),Y=page.Prepared.Y,H=h,V=v,Rotation=i.P(9),Cached=cached&&h==v&&i.P(8)==0&&i.P(10)==0&&(int)i.P(9)==0};}
  public static HashSet<int> Cache(Score score,DrawingLibrary lib){
   // Original overlay3 23E8:0000..0542 prepass: second eligible use,
   // 100 definitions, strict cumulative 2000 buffered-word budget.
   var cached=new HashSet<int>();int limit=NativePrintContext.ProcedureLimit;
   if(limit<0)return cached;
   var seen=new HashSet<int>();int used=0;
   Func<int,bool> register=number=>{if(cached.Contains(number)||cached.Count>=100)return false;if(seen.Add(number))return false;Glyph glyph;if(!lib.Symbols.TryGetValue(number,out glyph))return false;int cost=1+glyph.Points.Length;foreach(int op in glyph.Operations)if(op>0)cost++;if(used+cost>=limit)return true;used+=cost;cached.Add(number);return true;};
   int[] textIds=AliasIds;
   foreach(var i in score.Items){int number=(int)i.P(5);if(i.Code==9){if(number<0||number>=3000||Array.IndexOf(textIds,number)>=0)continue;float h=i.P(6),v=i.P(7)==0?h:i.P(7);if(BitConverter.ToInt32(BitConverter.GetBytes(h),0)!=BitConverter.ToInt32(BitConverter.GetBytes(v),0)||(int)i.P(8)!=0||(int)i.P(9)!=0)continue;register(number);}
    // Original overlay3:0455..0461 clears cached P10/P11 when a
    // repeated glyph reaches definition construction. Later mark dispatch
    // in this same record therefore sees zero, even after a budget refusal.
    else if(i.Code==1){bool clearedMark=false;int accidental=number%10;if(accidental!=0){accidental+=5;if(accidental>10)accidental+=134;clearedMark=register(accidental);}int head=(int)i.P(6);if(head>=10&&head<=19)head-=10;if(head>=0&&head!=7){if(head<=6)clearedMark=register(head+11)||clearedMark;else if(head==8)clearedMark=register(95)||clearedMark;else if(head==9)clearedMark=register(139)||clearedMark;else if(head>=20&&head<3000&&!IsTextAlias(head))clearedMark=register(head)||clearedMark;}int mark=clearedMark?0:Math.Abs((int)i.P(11));if(mark>0&&mark<29){if(mark<=3)register(mark+5);else if(mark<=19)register(mark+17);else if(mark>=21&&mark<=23)register(mark-15);else if(mark==25)register(119);else if(mark>=24)register(38);}}
    else if(i.Code==2){if(number>=0)register(Math.Min(number+43,44));else if(number==-4)register(39);}
    else if(i.Code==3&&(i.P(7)==0||i.P(7)==i.P(6)))register(number==3?2:number);
    else if(i.Code==17)register(Math.Abs(number)>100?8:number>0?7:6);
    else if(i.Code==18&&number>=98)register(117-number);
    else if(i.Code==7){if((int)i.P(7)==0){register(37);if(i.P(8)>=1&&i.P(8)<=3)register((int)i.P(8)+5);}else if((int)i.P(7)==20){if(number!=0)register(49);register(48);}}
   }return cached;
  }
  public static float RotatedWord(double value){return (float)Math.Max(-32750,Math.Min(32750,Math.Truncate(value)));}
  public static bool Draw(Graphics g,Item i,NativeNotationContext page,DrawingLibrary lib,ScoreText text,HashSet<int> cache,bool clefRequest=false){if(page==null||lib==null)return false;
   // LEFT36 original EPS verifies these nonpainting fields in the recorded domains.
   // Normalize only the tested field on a private clone; retain every other control.
   if(i.Code==9&&i.P(5)==79&&i.P(13)==9.266332f){i=i.Clone();i.Set(13,0);}
   if(i.Code==9&&i.P(5)==53&&i.P(20)==-1){i=i.Clone();i.Set(20,0);}
   if(i.Code==9&&i.P(5)==57&&i.P(16)==-5.3f){i=i.Clone();i.Set(16,0);}
   if(NativeCode9Terms.IsTerm((int)i.P(5)))return NativeCode9Terms.Draw(g,i,page,text);
   int number=(int)i.P(5),alias=Array.IndexOf(AliasIds,number);if(alias>=0&&(short)(int)i.P(11)>=0){if(i.P(6)==0){i=i.Clone();i.Set(6,1);}if(text==null)return false;
    for(int n=10;n<=20;n++)if(n!=10&&n!=11&&n!=15&&n!=16&&i.P(n)!=0)return false;
    if(Single.IsNaN(i.P(11))||Single.IsInfinity(i.P(11)))return false;
    short fontWord=unchecked((short)(int)i.P(11));
    if(fontWord!=0&&!(fontWord>=90&&fontWord<=94||fontWord==98||fontWord>=1000&&fontWord<=1089))return false;
    float horizontal=F(i.P(6)%100);if(!Present(horizontal))horizontal=1;
    float vertical=Present(i.P(7))?i.P(7):horizontal,size=number<50?1:1.0499999523162842f;
    float font=fontWord!=0?fontWord:alias<3?1000:1002;
    var body=Item.Make(16,i.P(2),i.P(3),i.P(4),number<50?1:size,F((double)horizontal*size),number<50?i.P(7):F((double)vertical*size),font,i.P(9),number<50?.6000000238418579f:0,fontWord==0?i.P(11):0,1,0);
    if(i.P(15)!=0){
     page=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};
     page.Prepared.X=F((double)page.Prepared.X+i.P(15)*150);
    }text.Set(body,number==81?"piu":AliasText[alias],page.Scale);if(number==81){if(NativeEpsExport.For(g)==null&&!GhostscriptTextRenderer.Enabled)return false;try{var plan=text.PreparePostScript(body,page,GhostscriptTextRenderer.DeviceMatrix(g));if(plan==null||plan.Parts.Count!=1||plan.Parts[0].Font==null)return false;var run=plan.Parts[0].Font;run.Bytes[2]=22;run.ReencodedUgrave=true;if(NativeEpsExport.For(g)==null)GhostscriptTextRenderer.Prepare(run,GhostscriptTextRenderer.Clip(g));text.PaintPostScript(g,plan);return true;}catch(Exception error){if(NativeEpsExport.For(g)!=null)throw;RenderResourceAudit.Unavailable("accented symbol alias",error.Message);return false;}}text.Draw(g,body,body.Text,0,0,page.Scale/5,page.Scale,page);return true;}
   var p=Prepare(i,page,cache!=null&&cache.Contains(number));if(p==null)return false;Glyph glyph;
   if(clefRequest&&cache!=null&&cache.Contains(number)&&p.H==p.V&&i.P(8)==0&&(int)i.P(9)==0)p.Cached=true;
   if(!lib.Symbols.TryGetValue(p.Glyph,out glyph))throw new InvalidDataException("External library glyph "+p.Glyph+" is unavailable.");
   if(glyph.Points.Length==0){
    RenderResourceAudit.Record("blank library slot",p.Glyph.ToString(),glyph.Source,"external DRW directory");
    return true;
   }
   var points=new List<BarlinePoint>();float width=NativePrintContext.Width((short)((int)i.P(10)-(number>=45?1:0)));
   if(p.Cached){float factor=p.Glyph<50?6.25f:7.5f,scale=F((double)p.H*page.Scale/(p.Glyph<50?6.25:25));bool inheritedDefault=p.Glyph<50&&scale==1;scale=Single.Parse(PrintedGlyphDefinition.Fixed3(scale),System.Globalization.CultureInfo.InvariantCulture);if(scale==0)return true;float numerator=clefRequest?NativePrintContext.UnclampedWidth((short)((int)i.P(10)-(number>=45?1:0))):NativePrintContext.RawWidth((short)(p.Glyph<50?0:-1));float pen=F(numerator/F((double)p.H*page.Scale/(p.Glyph<50?6.25:25)));if(pen<0)pen=0;pen=Single.Parse(PrintedGlyphDefinition.Fixed3(pen),System.Globalization.CultureInfo.InvariantCulture);short x=NativeNotationContext.RawWord(p.X),y=NativeNotationContext.RawWord(p.Y);
    // Cached procedures preserve fractional transformed vertices; use the same
    // external normalized path through its own PostScript transform below.
    return DrawCached(g,page,glyph,p,scale,pen,factor,x,y,inheritedDefault);
   }
   float h=p.H,v=p.V;if(p.Glyph>=50){h=F(.30000001192092896*h);v=F(.30000001192092896*v);}h=F((double)h*page.Scale);v=F((double)v*page.Scale);short originX=NativeNotationContext.Word(p.X,true),originY=NativeNotationContext.Word(p.Y,false);float angle=F(p.Rotation/57.2957763671875),sin=F(Math.Sin(angle)),cos=F(Math.Cos(angle));
   for(int n=0;n<glyph.Points.Length;n++){float x=NativeNotationContext.Word(F(glyph.Points[n].X*(double)h+p.X),true),y=NativeNotationContext.Word(F(glyph.Points[n].Y*(double)v+p.Y),false);if((int)p.Rotation!=0){float dx=F(x-originX),dy=F(y-originY);x=RotatedWord((double)dx*cos+(double)dy*sin+originX);y=RotatedWord((double)dy*cos-(double)dx*sin+originY);}int operation=glyph.Operations[n];if(i.P(8)==-1&&operation>0)operation=1;points.Add(new BarlinePoint(x,y,operation));}
   int copies=(int)i.P(8);
   if(copies>0&&(long)points.Count*(1+2*(copies/100+copies%100))>30000)
    throw new NotSupportedException("Printed library thickness exceeds the verified buffer domain.");
   RenderResourceAudit.Record("printed library symbol",p.Glyph+":"+glyph.Name,glyph.Source,"recovered expanded DRW scaling and integer-buffer rotation");
   NativeNoteDetails.Path(g,page,points,width,true);
   if(copies>0){
    var original=points.ToArray();
    points.Clear();
    for(int n=1;n<=copies/100;n++)AppendThickness(points,original,false,n);
    for(int n=1;n<=copies%100;n++)AppendThickness(points,original,true,n);
    NativeNoteDetails.Path(g,page,points,NativePrintContext.DefaultLineWidth,true);
   }
   return true;
  }
  internal static void AppendThickness(List<BarlinePoint> points,BarlinePoint[] original,bool horizontal,int count=1){
   foreach(int offset in new[]{14*count,-14*count})
    foreach(var point in original)
     points.Add(new BarlinePoint(horizontal?unchecked((short)(point.X+offset)):point.X,
      horizontal?point.Y:unchecked((short)(point.Y+offset)),point.Operation));
  }
  internal static bool DrawCached(Graphics g,NativeNotationContext page,Glyph glyph,LibrarySymbolPlan p,float scale,float width,float factor,short x,short y,bool inheritedDefault=false){if(inheritedDefault)width=NativePrintContext.DefaultLineWidth;RenderResourceAudit.Record("printed library symbol",p.Glyph+":"+glyph.Name,glyph.Source,"recovered repeated-glyph procedure scaling");var state=g.Save();try{using(var m=page.Projection.Matrix())g.MultiplyTransform(m,System.Drawing.Drawing2D.MatrixOrder.Prepend);g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);using(var path=new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Alternate))using(var pen=new Pen(Color.Black,width)){pen.LineJoin=System.Drawing.Drawing2D.LineJoin.Round;int kind=1;bool have=false;PointF prev=new PointF();Action flush=()=>{if(path.PointCount>0){if(kind!=1)g.TrackedFillPath(Brushes.Black,path);g.TrackedDrawPath(pen,path);var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScoreGlyph(path,kind!=1,width,scale,x,y,inheritedDefault);}path.Reset();have=false;};for(int n=0;n<glyph.Points.Length;n++){int op=glyph.Operations[n];if(op>0){if(op!=3&&!(op==1&&kind==1))flush();path.StartFigure();have=false;if(op!=3)kind=op==1?1:2;}var next=new PointF((int)F(glyph.Points[n].X*(double)factor),(int)F(glyph.Points[n].Y*(double)factor));if(have)path.AddLine(prev,next);prev=next;have=true;}flush();}}finally{g.Restore(state);}if(!inheritedDefault)NativePrintContext.RestoreDefaultLineWidth();return true;}
 }
}


