using System;using System.IO;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class NativeDatGlyphs {
  readonly string root;readonly bool drawingVectors;internal bool DrawingVectors{get{return drawingVectors;}}readonly byte[] data;DrawingLibrary drawingLibrary;public readonly string Source;
  public NativeDatGlyphs(string root,bool drawingVectors=false){this.root=root;this.drawingVectors=drawingVectors;Source=NativeLibraryLocation.FileFor(root,"SCOR3_10.DAT");data=File.ReadAllBytes(Source);if(data.Length<10464)throw new InvalidDataException("DAT music-glyph arrays are incomplete.");for(int n=0;n<46;n++){int at=BitConverter.ToUInt16(data,9422+n*2);if(at<1||at>476||(n>0&&at<BitConverter.ToUInt16(data,9422+(n-1)*2)))throw new InvalidDataException("DAT music-glyph directory is outside the recovered domain.");}}
  public short[] SpacingScaleWords(){var result=new short[414];for(int n=0;n<result.Length;n++)result[n]=BitConverter.ToInt16(data,4794+n*2);RenderResourceAudit.Record("DAT spacing scale","207 packed coordinate pairs",Source,"external scale table at original file offset4794");return result;}
  public List<PrimitiveTextPoint> ExternalDrawing(int glyph,float horizontal,float vertical,float scale,float x,float y){
   return ExternalDrawing(glyph,horizontal,vertical,scale,x,y,true);
  }
  List<PrimitiveTextPoint> ExternalDrawing(int glyph,float horizontal,float vertical,float scale,float x,float y,bool reduced){
   if(drawingLibrary==null)drawingLibrary=new DrawingLibrary(root);Glyph drawing;if(!drawingLibrary.Symbols.TryGetValue(glyph,out drawing))throw new InvalidDataException("External drawing unavailable: "+glyph);float h=reduced?(float)((double)horizontal*.30000001192092896):horizontal,v=reduced?(float)((double)vertical*.30000001192092896):vertical;h=(float)((double)h*scale);v=(float)((double)v*scale);var points=new List<PrimitiveTextPoint>();foreach(var p in drawing.Points)points.Add(new PrimitiveTextPoint{X=(float)((double)p.X*h+x),Y=(float)((double)p.Y*v+y),Operation=drawing.Operations[points.Count]});return points;
  }
  public List<PrimitiveTextPoint> ExternalHead(int glyph,float scale,float x,float y){return ExternalDrawing(glyph,.8330000042915344f,.8330000042915344f,scale,x,y,glyph>=50);}
  // The original circle helper uses the external DAT sine/cosine pairs.
  // Repeat dots request a filled six-sided circle at 60-degree increments.
  public List<PrimitiveTextPoint> RepeatDot(float staff,float x,float y){
   float radius=(float)((double)staff*.03500000014901161),amplitude=(float)((double).5799999833106995f*radius);var points=new List<PrimitiveTextPoint>();
   for(int angle=0;angle<=360;angle+=60){int at=4+(angle*2+1)*4;float dx=BitConverter.ToSingle(data,at),dy=BitConverter.ToSingle(data,at+4);if(Single.IsNaN(dx)||Single.IsInfinity(dx)||Single.IsNaN(dy)||Single.IsInfinity(dy))throw new InvalidDataException("DAT circle table contains a non-finite coordinate.");points.Add(new PrimitiveTextPoint{X=(float)((double)dx*amplitude+x),Y=(float)((double)dy*amplitude+y),Operation=(short)(angle==0?2:0)});}
   RenderResourceAudit.Record("DAT repeat dot","six-sided circle",Source,"external trigonometric coordinate table");return points;
  }
  internal void CircleTablePoint(int angle,out float x,out float y){
   int at=8+(angle%360)*8;
   x=BitConverter.ToSingle(data,at);
   y=BitConverter.ToSingle(data,at+4);
  }
  public List<PrimitiveTextPoint> Circle(float scale,float x,float y,float horizontal,float vertical,bool fill,int step=1,int start=0,int stop=360){
   float radius=(float)((double)scale*.03500000014901161),h=(float)((double)horizontal*radius),v=(float)((double)vertical*radius);var result=new List<PrimitiveTextPoint>();
   for(int angle=start;angle<=stop;angle+=step){float dx,dy;CircleTablePoint(angle,out dx,out dy);result.Add(new PrimitiveTextPoint{X=(float)((double)dx*h+x),Y=(float)((double)dy*v+y),Operation=(short)(angle==start?(fill?2:1):0)});}return result;
  }
  internal List<PrimitiveTextPoint> ShapeCircle(float scale,float x,float y,float horizontal,float vertical,bool fill,int step,int start,int stop){
   var points=Circle(scale,x,y,horizontal,vertical,fill,step,start,Math.Min(stop,360));
   if(stop>360){
    foreach(var point in Circle(scale,x,y,horizontal,vertical,false,step,0,stop-361)){
     var next=point;next.Operation=0;points.Add(next);
    }
   }
   return points;
  }
  public List<PrimitiveTextPoint> Expand(int glyph,float horizontal,float vertical,float staff,float x,float y){
   foreach(float value in new[]{horizontal,vertical,staff,x,y})if(Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("DAT glyph projection requires finite inputs.");
   if(horizontal==0||vertical==0)throw new NotSupportedException("DAT glyph scale defaults must be resolved by the original caller.");
   if(glyph<0||glyph>44)throw new NotSupportedException("DAT music glyph requires index 0..44.");if(drawingVectors){if(drawingLibrary==null)drawingLibrary=new DrawingLibrary(root);Glyph drawing;if(!drawingLibrary.Symbols.TryGetValue(glyph,out drawing))throw new InvalidDataException("External drawing unavailable: "+glyph);float h=(float)((double)(float)(horizontal/4.0)*staff),v=(float)((double)(float)(vertical/4.0)*staff);var points=new List<PrimitiveTextPoint>();foreach(var p in drawing.Points)points.Add(new PrimitiveTextPoint{X=NativeNotationContext.Word((float)((double)p.X*h+x),true),Y=NativeNotationContext.Word((float)((double)p.Y*v+y),false),Operation=drawing.Operations[points.Count]});return points;}int start=BitConverter.ToUInt16(data,9422+glyph*2),end=BitConverter.ToUInt16(data,9424+glyph*2);
   if(start==end)throw new NotSupportedException("DAT glyph has no direct coordinate stream; its original special route is required.");
   float sx=(float)((double)horizontal*staff),sy=(float)((double)vertical*staff);var result=new List<PrimitiveTextPoint>();
   for(int n=start;n<end;n++){float px=BitConverter.ToSingle(data,5622+(n-1)*4),py=BitConverter.ToSingle(data,7522+(n-1)*4);short op=BitConverter.ToInt16(data,9514+(n-1)*2);if(Single.IsNaN(px)||Single.IsInfinity(px)||Single.IsNaN(py)||Single.IsInfinity(py)||op<0||op>1)throw new InvalidDataException("DAT music point outside the verified finite operation domain.");result.Add(new PrimitiveTextPoint{X=(float)((double)px*sx+x),Y=(float)((double)py*sy+y),Operation=op});}
   RenderResourceAudit.Record("DAT music glyph",glyph.ToString(),Source,"external single-precision coordinate arrays and operation directory");return result;
  }
 }
 public sealed class NativeDatHeadCache {
  internal NativePrintedChordGrid.State GridState;
  readonly bool freshMarkerWords;
  internal Func<int,short> ReadMarkerY;
  internal int MarkerPrefix;
  public NativeDatHeadCache(bool trackFreshMarkerWords=false){freshMarkerWords=trackFreshMarkerWords;}
  internal void Reset(){offsets=null;GridState=null;}
  uint scaleBits;List<PrimitiveTextPoint> offsets;
  internal bool MatchesScale(float scale){return offsets!=null&&scaleBits==BitConverter.ToUInt32(BitConverter.GetBytes(scale),0);}
  public List<PrimitiveTextPoint> Project(List<PrimitiveTextPoint> first,float scale,float x,float y){
   short ox=NativeNotationContext.Word(x,true),oy=NativeNotationContext.Word(y,false);uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(scale),0);
   if(offsets==null||bits!=scaleBits){scaleBits=bits;offsets=new List<PrimitiveTextPoint>();int at=MarkerPrefix;foreach(var p in first){short retained=ReadMarkerY==null?(short)0:ReadMarkerY(at);offsets.Add(new PrimitiveTextPoint{X=unchecked((short)(NativeNotationContext.Word(p.X,true)-ox)),Y=unchecked((short)(NativeNotationContext.Word(p.Y,false)-oy)),Operation=p.Operation,RetainedMarkerY=freshMarkerWords&&p.Operation>0?(short?)unchecked((short)(retained-oy)):null});if(p.Operation>0||p.CommandOnly)at++;if(!p.CommandOnly)at++;}return first;}
   var result=new List<PrimitiveTextPoint>();foreach(var p in offsets){short xx=unchecked((short)((short)p.X+ox));int yy=(short)p.Y+oy;yy=Math.Min(32700,yy);if(xx< -32600||xx>32760||yy< -32768)throw new NotSupportedException("Cached DAT head outside the verified unclipped domain.");result.Add(new PrimitiveTextPoint{X=xx,Y=yy,Operation=p.Operation,RetainedMarkerY=p.RetainedMarkerY.HasValue?(short?)unchecked((short)Math.Min(32700,p.RetainedMarkerY.Value+oy)):null});}return result;
  }
 }
 public static class NativeScanMusicVectors {
  static float F(double x){return (float)x;}
  public static void HeadProjection(Item i,NativeNotationContext page,out float scale,out float x,out float y){
   if(i==null||page==null||i.Code!=1||i.P(6)<-1||i.P(6)>=20&&!NativePrintedHeads.Custom(i)||Single.IsNaN(i.P(4))||Single.IsInfinity(i.P(4))||Math.Abs(i.P(4))>32767)throw new NotSupportedException("Note head outside the recovered request.");
   float baseScale=page.Scale;bool sizeSet=BitConverter.ToInt32(BitConverter.GetBytes(i.P(15)),0)!=0;if(sizeSet)baseScale=F((double)baseScale*i.P(15));scale=!sizeSet&&Math.Abs((int)i.P(4)/100)==1?F((double)baseScale*.6000000238418579):baseScale;x=page.Prepared.X;y=page.Prepared.Y;
   if(i.P(10)!=0){float shift=i.P(10)==10||i.P(10)==20?F(((int)i.P(6)==2||(int)i.P(6)==3?19.0:17.0)*scale):F((double)baseScale*i.P(10)*6);if(i.P(10)==20)shift=-shift;x=F((double)shift+x);}
   if(i.P(12)>0){int cross=(int)i.P(12),target=page.Staff-cross*2+3;if(cross>0&&target>0){if(page.StaffBases==null||target>=page.StaffBases.Length)throw new NotSupportedException("Cross-staff notehead needs the recovered staff-base table.");y=F((double)page.StaffBases[target]-page.StaffBases[page.Staff]+y);}}
  }
  public static List<PrimitiveTextPoint> Head(Item i,NativeNotationContext page,NativeDatGlyphs dat,NativeDatHeadCache cache=null){float scale,x,y;HeadProjection(i,page,out scale,out x,out y);if(cache!=null&&cache.ReadMarkerY!=null)cache.MarkerPrefix=NativeNoteDetails.PrepareDat(i,page).LedgerCount*3;
   int rawHead=(int)i.P(6),head=rawHead>=10&&rawHead<20?rawHead-10:rawHead;bool parenthesized=rawHead>=10&&rawHead<20;var points=head>=20?dat.ExternalHead(head,scale,x,y):head==7||head==-1?new List<PrimitiveTextPoint>():head==8||head==9?dat.ExternalHead(head==8?95:139,scale,x,y):dat.Expand(11+head,1,1,scale,x,y);if(head==0&&cache!=null)points=cache.Project(points,scale,x,y);
   if(parenthesized){float left=-13.800000190734863f;int accidental=(int)i.P(5)%10;if(accidental!=0)left=F((double)left-14.399999618530273);if(accidental==4)left=F((double)left-12.600000381469727);left=F((double)left*scale+x);points.AddRange(dat.ExternalDrawing(208,1.399999976158142f,1.399999976158142f,scale,left,y));float right=31.200000762939453f;if(i.P(9)>=10)right=39.200000762939453f;if(i.P(9)>=10&&(int)i.P(9)%100>=20)right=51.200000762939453f;right=F((double)right*scale+x);points.AddRange(dat.ExternalDrawing(208,-1.399999976158142f,1.399999976158142f,scale,right,y));}return points;
  }
  public static List<PrimitiveTextPoint> Flags(Item i,NativeNotationContext page,NativeDatGlyphs dat){
   var note=NativeNoteDetails.PrepareDat(i,page);int head=(int)i.P(6);bool paren=head>=10&&head<20;if(paren)head-=10;if(note==null||(head < -1 || head>9&&head<20 || head>=3000)||i.P(9)<0||i.P(9)>=2000)throw new NotSupportedException("Flag controls outside the recovered request.");int count=(int)i.P(9)%10;if(count<1)throw new NotSupportedException("Requires a positive flag unit.");bool up=note.StemKind==1;if(!up&&note.StemKind!=2)throw new NotSupportedException("Requires an up/down stem.");float scale=note.Scale,increment=F((up?7.0:-7.0)*scale),x=up?F((double)note.NoteX+note.HeadWidth):note.NoteX,y=F((double)(i.P(8)%100)*increment+note.NoteY+increment);if(count>1)y=F((double)y-increment/2.0);var result=new List<PrimitiveTextPoint>();for(int n=0;n<count;n++){result.AddRange(dat.Expand(20,1,up?-1:1,scale,x,y));y=F((double)y+increment+increment);}return result;
  }
  public static List<PrimitiveTextPoint> Note(Item i,NativeNotationContext page,NativeDatGlyphs dat,NativeDatHeadCache cache=null){
   short finalFont;
   return Note(i,page,dat,cache,null,0,out finalFont);
  }
  public static List<PrimitiveTextPoint> Note(Item i,NativeNotationContext page,NativeDatGlyphs dat,NativeDatHeadCache cache,ScoreText text,short previousFont,out short finalFont){
   finalFont=previousFont;
   var n=NativeNoteDetails.PrepareDat(i,page);if(n==null)throw new NotSupportedException("Note component controls outside the recovered DAT plan.");var points=new List<PrimitiveTextPoint>();float yy=n.LedgerY;for(int k=0;k<n.LedgerCount;k++){points.Add(new PrimitiveTextPoint{X=n.LedgerLeft,Y=yy,Operation=1});points.Add(new PrimitiveTextPoint{X=n.LedgerRight,Y=yy});yy=F((double)yy+n.LedgerIncrement);}if(NativePrintedHeads.Custom(i)&&NativeLibrarySymbols.IsTextAlias((int)i.P(6))){
    if((i.P(11)!=0&&!NativePrintedTextHeads.VerifiedFontMark(i)))throw new NotSupportedException("Custom text-head attachment state requires additional original verification.");
    float scale,hx,hy;HeadProjection(i,page,out scale,out hx,out hy);
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};context.Prepared.Scale=scale;context.Prepared.X=hx;context.Prepared.Y=hy;
    float endX;short lastCode;int selector=(int)i.P(6);
    var symbol=Item.Make(9,i.P(2),i.P(3),i.P(4),selector,selector<50?i.P(6):.8330000042915344f,selector<50?i.P(7):0);symbol.Set(9,i.P(9));symbol.Set(11,i.P(11));
    points.AddRange(NativeLibrarySymbols.ScanTextAlias(symbol,context,text,out finalFont,out endX,out lastCode,previousFont));
    // The text converter leaves the final glyph code and forced font in the
    // note routine's integer cache. Float source parameters have been restored.
    int stemKind=lastCode/10%10,flagCount=(int)i.P(9)%10;bool up=stemKind!=2;
    float extension=F((double)i.P(8)-100),extra=flagCount>1?F((flagCount-1)*2.0-.5):0;
     if(flagCount>0){float dy=F((double)page.Prepared.VerticalInput+(up?1:-1)*F((double)extension+extra)*.6000000238418579),dx=F((double)n.Scale*6);
      points.Add(new PrimitiveTextPoint{X=F((up?.7829999923706055:-1.8830000162124634)*dx+endX),Y=NativeNoteDetails.SlashVertical(i,page,F((up?2.799999952316284:-5.5)*.6000000238418579+dy)),Operation=1});
      points.Add(new PrimitiveTextPoint{X=F((up?6.5:3.799999952316284)*dx+endX),Y=NativeNoteDetails.SlashVertical(i,page,F((up?5.400000095367432:-2.799999952316284)*.6000000238418579+dy))});
     }
    if(!(selector>=100&&selector<=198&&selector!=139)&&selector<900){
     float stemX=up?F((double)endX+n.HeadWidth):endX,direction=up?1:-1;
     points.Add(new PrimitiveTextPoint{X=stemX,Y=i.P(17)%100!=0?F((double)n.NoteY+(i.P(17)%100)*(double)n.Scale*7*direction):F((double)n.NoteY+2.0999999046325684*n.Scale*direction),Operation=1});
     points.Add(new PrimitiveTextPoint{X=stemX,Y=F((double)n.NoteY+F(((double)extra+extension+7)*n.Scale*7)*direction)});
    }
    if((short)(int)(i.P(17)/100)!=0)points.Add(new PrimitiveTextPoint{Operation=99,CommandOnly=true});
    if(flagCount>0){float increment=F((up?7.0:-7.0)*n.Scale),fx=up?F((double)endX+n.HeadWidth):endX,fy=F((double)(i.P(8)%100)*increment+n.NoteY+increment);if(flagCount>1)fy=F((double)fy-increment/2.0);
     for(int flag=0;flag<flagCount;flag++){points.AddRange(dat.Expand(20,1,up?-1:1,n.Scale,fx,fy));fy=F((double)fy+increment+increment);}
    }
    int accidental=lastCode%10;if(accidental!=0){float fraction=F(i.P(5)%1);if(fraction>.8999999761581421)fraction=F(.8999999761581421-fraction);float offset=F((double)fraction*n.Scale*60);if(i.P(10)==10)offset=F((double)n.HeadWidth+offset);float ax=F((double)endX-offset);int glyph=accidental>5?139+accidental:5+accidental;points.AddRange(glyph<=44?dat.Expand(glyph,1,1,n.Scale,ax,n.NoteY):dat.ExternalDrawing(glyph,1,1,n.Scale,ax,n.NoteY));}
    float dotFraction=F(i.P(9)%1);if(dotFraction>.8999999761581421)dotFraction=F(.8999999761581421-dotFraction);if((int)i.P(9)>=1000)dotFraction=F(.2840000092983246+dotFraction);float aliasDotX=F((double)endX+F(((double)dotFraction*60+18)*n.Scale));
    for(int dot=0;dot<n.DotCount;dot++){points.AddRange(dat.Expand(24,1,1,n.Scale,aliasDotX,n.DotY));aliasDotX=F((double)aliasDotX+n.Scale*13);}
    if(i.P(11)!=0){var finger=i.Clone();// Original 1CCA:00A3 converter leaves P13 width; accented-glyph path
    // writes BODN0 over P14/P15. The note fingering reads P13/P14 afterwards.
    // N159 live entry/return snapshots and N155 whole buffers verify this state.
    if(selector==81){var alias=NativeLibrarySymbols.AliasTextItem(symbol,context,text);finger.Set(13,text.PrepareLayout(alias,alias.Text,context.Scale,previousFont).Width);finger.Set(14,BitConverter.ToSingle(System.Text.Encoding.ASCII.GetBytes("BODN"),0));}finger.Set(5,20);finger.Set(10,0);var attachmentPage=NativePrintedTextHeads.AttachmentPage(page,endX);NativeNotationContext fingerContext;var fingerBody=NativeNoteAttachments.Fingering(finger,attachmentPage,text,true,out fingerContext,i.P(15)!=0?n.Scale:page.Scale);if(fingerBody==null)throw new NotSupportedException("Alias fingering context outside measured state.");points.AddRange(text.ScanStrokeText(fingerBody,fingerBody.Text,fingerContext,finalFont,out finalFont));}
    return points;
   }else points.AddRange(Head(i,page,dat,cache));if(n.Stem||n.Slash){if(n.Slash){points.Add(new PrimitiveTextPoint{X=n.SlashX1,Y=n.SlashY1,Operation=1});points.Add(new PrimitiveTextPoint{X=n.SlashX2,Y=n.SlashY2});}if(n.Stem){points.Add(new PrimitiveTextPoint{X=n.StemX,Y=n.StemStart,Operation=1});points.Add(new PrimitiveTextPoint{X=n.StemX,Y=n.StemEnd});}if(n.Stem&&(short)(int)(i.P(17)/100)!=0)points.Add(new PrimitiveTextPoint{Operation=99,CommandOnly=true});}if((int)i.P(9)%10>0)points.AddRange(Flags(i,page,dat));var accidentals=NativeNoteAttachments.Accidental(i,page,true);if(accidentals==null)throw new NotSupportedException("Accidental controls outside the recovered DAT plan.");foreach(var r in accidentals)points.AddRange(r.Glyph<=44?dat.Expand(r.Glyph,r.Horizontal,r.Vertical,n.Scale,r.X,r.Y):dat.ExternalDrawing(r.Glyph,r.Horizontal,r.Vertical,n.Scale,r.X,r.Y));float dotX=n.DotX;for(int k=0;k<n.DotCount;k++){points.AddRange(dat.Expand(24,1,1,n.Scale,dotX,n.DotY));dotX=F((double)dotX+n.Scale*13);}NativeNotationContext fingeringContext;
   var fingering=NativeNoteAttachments.Fingering(i,page,text,true,out fingeringContext);
   if(fingering!=null){points.AddRange(text.ScanStrokeText(fingering,fingering.Text,fingeringContext,previousFont,out finalFont));return points;}
   var heavyLine=NativeNoteAttachments.HeavyLine(i,page);
   if(heavyLine!=null){points.AddRange(heavyLine);return points;}
   var marks=NativeNoteAttachments.Marks(i,page,true);if(marks==null)throw new NotSupportedException("Mark controls outside the recovered DAT plan.");foreach(var r in marks)points.AddRange(r.Glyph<=44?dat.Expand(r.Glyph,r.Horizontal,r.Vertical,n.Scale,r.X,r.Y):dat.ExternalDrawing(r.Glyph,r.Horizontal,r.Vertical,n.Scale,r.X,r.Y));return points;
  }
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext page,NativeDatGlyphs dat,DrawingLibrary library=null,ScoreText text=null){
   if(i==null||page==null||dat==null)throw new ArgumentNullException();
   if(i.Code==2&&i.P(8)>0)return NativeDatRestAttachments.Number(i,page,dat,library,text);
   if(i.Code==2&&(i.P(8)==-1||i.P(8)==-2))return NativeDatRestAttachments.Fermata(i,page,dat,library,text);
   if(i.Code==3){short? font;return NativeDatClefs.Prepare(i,page,dat,library,text,out font);}

   float longRestOrigin;
   if(i.Code==2&&NativeDatRestAttachments.TryUnnumberedLongOrigin(i,page,out longRestOrigin))
    return dat.Expand(38,1,1,NativeDatRestAttachments.EffectiveScale(i,page),longRestOrigin,page.Vertical(NativeDatRestAttachments.Pitch(i)));
   if(i.Code==2&&i.P(5)>=-19&&i.P(5)<=-11)return NativeDatRestAttachments.EuropeanBody(i,page,dat,library,text);
   if(i.Code==2&&(i.P(5)==-6||i.P(5)==-7)&&i.P(6)==0&&i.P(8)==0&&(i.P(9)>=0||i.P(9)==-1)){for(int n=10;n<=20;n++)if(n!=15&&i.P(n)!=0)throw new NotSupportedException("Narrow multi-rest controls outside the measured DAT path.");return dat.ExternalDrawing(i.P(5)==-6?116:117,.8299999833106995f,.8299999833106995f,NativeDatRestAttachments.EffectiveScale(i,page),i.P(9)>0?F((double)i.P(9)*150):page.Prepared.X,page.Vertical(F(8.0+NativeDatRestAttachments.Pitch(i))));}
   if(i.Code==2&&i.P(4)==0&&i.P(5)==-2&&i.P(6)==0&&(i.P(7)==3||i.P(7)==4)&&i.P(8)==0&&i.P(9)>0){for(int n=10;n<=20;n++)if(i.P(n)!=0&&!(n==13&&i.P(n)==1)&&n!=15)throw new NotSupportedException("Whole-rest controls outside the bounded request.");return dat.Expand(41,1,1,NativeDatRestAttachments.EffectiveScale(i,page),F((double)i.P(9)*150),F((double)page.Prepared.Y+63*page.Scale));}
   if(i.Code==2&&i.P(6)<=-1&&i.P(6)>=-32768&&i.P(5)>=-3&&i.P(5)<=10){for(int n=8;n<=20;n++)if(i.P(n)!=0&&n!=9&&n!=10)throw new NotSupportedException("Invisible-rest auxiliary controls are not yet verified.");return new List<PrimitiveTextPoint>();}
   if(i.Code==2&&i.P(6)>-1&&i.P(6)<=32767&&i.P(5)>=-4&&i.P(5)<=10&&i.P(5)==(int)i.P(5)&&(i.P(9)==0||i.P(9)==-1||i.P(9)>0)){
    for(int n=8;n<=20;n++)if(i.P(n)!=0&&n!=9&&n!=15)throw new NotSupportedException("Rest auxiliary control is not yet verified for DAT display.");int rest=(int)i.P(5),glyph=rest>=1?44:43+rest;float offset=rest==-4?0:rest==-2?63:rest==-1?49:56,restScale=NativeDatRestAttachments.EffectiveScale(i,page);var points=new List<PrimitiveTextPoint>();int count=Math.Max(1,rest);float origin=rest<=-2&&i.P(9)>0?F((double)i.P(9)*150):page.Prepared.X;float x=count>1?F((double)origin-3*restScale):origin,units=F(offset/7.0-Math.Min(2,count/2)*2);bool cue=i.P(15)==0&&Math.Abs((int)i.P(4))>=100;float verticalPitch=i.P(15)!=0?i.P(4):i.P(4)%100;if(count>1&&cue){units=F((double)units+.699999988079071);units=F((double)units+.699999988079071);}float increment=cue?1.399999976158142f:F(2.0*(i.P(15)==0?1:i.P(15))),y=page.Vertical(F((double)verticalPitch+units));
    float[] dotOffsets={Single.NaN,19,19,19,13,15.5f,18.399999618530273f,22.299999237060547f,26.200000762939453f};int dotIndex=Math.Min(8,rest+4);float dotX=F((double)origin+dotOffsets[dotIndex]*restScale),dotY=page.Vertical(F((double)verticalPitch+(rest>2?10:8)));
    // Original 0F73:0BFE..0C98: the DAT scan replaces >=5 dots with
    // a guide before the body. F6 printing independently emits all dots.
    if(i.P(6)>=5&&rest!=-4){float span=F((double)i.P(6)*150),width=F((double)restScale*16.799999237060547),guideY=F((double)(i.P(15)!=0?restScale:page.Scale)*42+page.Prepared.Y);points.Add(new PrimitiveTextPoint{X=F((double)origin-span),Y=guideY,Operation=1});points.Add(new PrimitiveTextPoint{X=F((double)origin+width+span),Y=guideY});}
    for(int n=0;n<(i.P(6)>=5?0:(int)i.P(6));n++){points.AddRange(dat.Expand(24,1,1,restScale,dotX,dotY));dotX=F((double)dotX+13*restScale);}
    for(int n=0;n<count;n++){points.AddRange(dat.Expand(glyph,1,1,restScale,x,y));units=F((double)units+increment);y=page.Vertical(F((double)verticalPitch+units));x=F((double)x+4.5*restScale);}
    int pitch=(int)i.P(4)%100;if((rest==-2&&(pitch>=4||pitch<=-7))||(rest==-1&&(pitch>=5||pitch<=-6))){float yy=page.Vertical(F((double)verticalPitch+offset/7.0));points.Add(new PrimitiveTextPoint{X=F((double)origin-8*restScale),Y=yy,Operation=1});points.Add(new PrimitiveTextPoint{X=F((double)origin+25*restScale),Y=yy});}return points;
   }
   if(i.Code==17)return NativeDatKeys.Prepare(i,page,dat);
   if(i.Code==18&&library!=null&&(i.P(5)==98||i.P(5)==99)){for(int n=16;n<=20;n++)if(n!=16&&i.P(n)!=0)throw new NotSupportedException("Common-time auxiliary control is not yet verified for DAT display.");if(!ScoreVersionControls.MeterThicknessSupported(NativeLibraryLocation.IsScor4(library.Root),i.P(16)))throw new NotSupportedException("Common-time thickness outside supported domain.");Glyph glyph;if(!library.Symbols.TryGetValue(117-(int)i.P(5),out glyph))throw new InvalidDataException("External common-time drawing unavailable.");if(!NativeMeterDisplay.ValidSize(i.P(7)))throw new NotSupportedException("Common-time size outside the measured domain.");float size=i.P(7)==0?1:i.P(7),factor=F(page.Scale*(double)F(size*.75)),origin=F((double)page.Prepared.X+i.P(15)*150);var points=new List<PrimitiveTextPoint>();for(int n=0;n<glyph.Points.Length;n++)points.Add(new PrimitiveTextPoint{X=F(glyph.Points[n].X*(double)factor+origin),Y=F(glyph.Points[n].Y*(double)factor+page.Prepared.Y),Operation=glyph.Operations[n]});short thickness=unchecked((short)(int)i.P(10));if(thickness>0)NativeDatLines.Thickness(points,unchecked((short)(thickness*101)));return points;}
   if(i.Code==18&&text!=null){bool zeroMeter=i.P(5)==0&&i.P(6)==0;if(zeroMeter){i=i.Clone();i.Set(6,1);}var numbers=NativeMeterDisplay.Prepare(i,page,false,NativeLibraryLocation.IsScor4(text.RootDirectory));if(numbers==null)throw new NotSupportedException("Meter controls outside the numeric DAT route.");if(zeroMeter){var z=numbers[0];z.Number=0;numbers[0]=z;}var points=new List<PrimitiveTextPoint>();foreach(var n in numbers){if(n.Number<0||n.Number>99)throw new NotSupportedException("Numeric DAT meter outside the recovered domain.");if(i.P(13)!=0){
      var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Prepared=page.Prepared,Projection=page.Projection};
      context.Prepared.X=n.X;context.Prepared.Y=n.Y;
      var number=Item.Make(16,i.P(2),0,0,1,n.Size,0,i.P(13),0,0,0,1,0);
      short font;points.AddRange(text.ScanStrokeText(number,n.Number.ToString(System.Globalization.CultureInfo.InvariantCulture),context,0,out font));continue;
     }if(n.Number>=10){var body=Item.Make(16,i.P(2),i.P(3),i.P(4),1,n.Size,0,94,0,0,0,1,0);var numericLayout=text.PrepareLayout(body,n.Number.ToString(System.Globalization.CultureInfo.InvariantCulture),page.Scale,0);float x=n.X;foreach(var token in numericLayout.Tokens){points.AddRange((dat.DrawingVectors?text.DrawingPoints(94,token.Code,numericLayout,page.Scale,x,n.Y):text.StrokePoints(token.Code,numericLayout,page.Scale,x,n.Y)));x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);}continue;}float size=F(.2800000011920929*n.Size);var layout=new NativeTextLayout{Parameters=new TextPreparedParameters{GlyphHorizontal=size,GlyphVertical=size}};points.AddRange((dat.DrawingVectors?text.DrawingPoints(94,n.Number,layout,page.Scale,n.X,n.Y):text.StrokePoints(n.Number,layout,page.Scale,n.X,n.Y)));}return points;}

   if(i.Code==9&&NativeLibrarySymbols.IsTextAlias((int)i.P(5))&&(short)(int)i.P(11)>=0)return NativeLibrarySymbols.ScanTextAlias(i,page,text);
   if(i.Code==9&&library!=null)return NativeLibrarySymbols.ScanGlyph(i,page,library);
   throw new NotSupportedException("DAT item request is not yet recovered.");
  }
 }
}
