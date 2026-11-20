using System;using System.IO;using System.Text;using System.Globalization;using System.Drawing;using System.Drawing.Drawing2D;
namespace ScoreModern {
 // EPS observes the same GDI draw calls as Canvas.Draw; no raster image is wrapped.
 public sealed partial class NativeEpsExport : IDisposable {
  [ThreadStatic] internal static NativeEpsExport Current;
  readonly bool fontMode;string baseRawSetup;string rawTransform="";bool literalState,textState,fontInitializationWritten;int literalScope;string lastFontSelection;string RawSetup{get{return (literalState?"SMliteral setgstate":baseRawSetup)+rawTransform;}}bool aborted;readonly Graphics target;TextWriter writer;public int Paths;
  static string N(double v){if(Double.IsNaN(v)||Double.IsInfinity(v))throw new NotSupportedException("Non-finite EPS coordinate.");return v.ToString("0.#########",CultureInfo.InvariantCulture);}
  // SCORE's ordinary black is initial state, not a reset after font99 code.
  internal string DefaultInkProgram{get{return literalState?"":"0 setgray ";}}
  NativeEpsExport(Graphics g,string file,float width,float height,string box,string boundsWarning,Score score,string captionName=null,bool scorlasCaption=false){if(Current!=null)throw new InvalidOperationException("Nested EPS export.");target=g;var job=NativePrintJob.Active;baseRawSetup=job!=null&&!job.Freeze&&job.BottomMargin!=27250
    ?"SMphysical setmatrix\n"+job.RawProgram().TrimEnd('\n')
    :"SMpage setmatrix\n.018 dup scale 2100 27250 translate";fontMode=GhostscriptTextRenderer.Enabled;string initializer=fontMode?PostScriptFontRun.CopyFont:null;writer=new StreamWriter(file,false,Encoding.GetEncoding(28591));writer.WriteLine("%!PS-Adobe-3.0 EPSF-3.0\n%%Creator: SCORE Modern shared vector drawing path\n%%BoundingBox: "+box+"\n%%Pages: 1\n%%LanguageLevel: 2\n%%EndComments\n%%Page: 1 1\n") ;writer.WriteLine("200 dict begin /SMsave save def");writer.Write(NativeRawPostScript.Aliases);if(job!=null){
    // SCORLAS writes its device prolog from the first printed file, before
    // later tiling translations. Its complete DSC scan box is separate.
    writer.Write(job.DeviceProgram(scorlasCaption&&job.ResolvedExtents!=null?job.ResolvedExtents.DscBox(job):box));
    writer.WriteLine("/SMphysical matrix currentmatrix def");
    if(!job.Header.Equals("None",StringComparison.OrdinalIgnoreCase))writer.Write(new NativePrintCaptions(job,score,captionName,scorlasCaption).PostScript());
    writer.WriteLine(job.DeltaProgram());
   }else writer.WriteLine("/SMphysical matrix currentmatrix def");writer.WriteLine("/SMpage matrix currentmatrix def");if(fontMode){writer.WriteLine(initializer);fontInitializationWritten=true;}writer.WriteLine(NativePrintJob.Active==null?"gsave\n0 0 612 792 rectclip":"gsave");writer.WriteLine(N(612.0/width)+" "+N(-792.0/height)+" scale\n0 "+N(-height)+" translate\n/SMcanvas matrix currentmatrix def");if(NativePrintContext.StrokeAdjustment)writer.WriteLine("true setstrokeadjust");Current=this;if(boundsWarning!=null)Comment(boundsWarning);if(job!=null&&job.OffsetWarning!=null)Comment(job.OffsetWarning);}
  internal static NativeEpsExport For(Graphics g){return Current!=null&&Object.ReferenceEquals(Current.target,g)?Current:null;}
  sealed class RawTransformScope:IDisposable{readonly NativeEpsExport owner;readonly string before;internal RawTransformScope(NativeEpsExport owner,string before){this.owner=owner;this.before=before;}public void Dispose(){owner.rawTransform=before;}}
  internal IDisposable PushRawRotation(PointF pivot,float rotation){string before=rawTransform;if(rotation!=0)rawTransform+="\n"+N(pivot.X)+" "+N(pivot.Y)+" translate "+N(rotation)+" rotate "+N(-pivot.X)+" "+N(-pivot.Y)+" translate";return new RawTransformScope(this,before);}
  internal void StageItem(Action render){
   var output=writer;int previousPaths=Paths;
   using(var item=new StringWriter(CultureInfo.InvariantCulture)){
    writer=item;
    try{render();}
    catch{Paths=previousPaths;throw;}
    finally{writer=output;}
    output.Write(item.ToString());
   }
  }
  internal void Abort(){aborted=true;}
  internal void Program(string program){writer.WriteLine(literalState?"gsave SMliteral setgstate SMliteralbase matrix invertmatrix concat SMphysical concat":"gsave SMphysical setmatrix");writer.Write(program);writer.WriteLine("grestore");}
  void RawProgram(string program){writer.WriteLine("gsave "+RawSetup);writer.Write(program);writer.WriteLine("grestore");}
  void EnsureLiteralState(){
   if(literalState)return;
   writer.WriteLine("gsave "+baseRawSetup+"\n/SMliteralbase matrix currentmatrix def\ngrestore");
   writer.WriteLine("gsave "+(textState?"SMtextstate setgstate":RawSetup+"\n0 setgray 1 setlinejoin 0 setlinecap "+NativePrintContext.DefaultLineWidth.ToString("F4",CultureInfo.InvariantCulture)+" setlinewidth")+"\n/SMliteral gstate def\ngrestore");literalState=true;
  }
  sealed class LiteralRotation:IDisposable {readonly NativeEpsExport owner;readonly string saved;internal LiteralRotation(NativeEpsExport owner,string saved){this.owner=owner;this.saved=saved;}public void Dispose(){owner.writer.WriteLine("/SMliteral "+saved+" def");owner.lastFontSelection=null;}}
  internal IDisposable LiteralTextRotation(float x,float y,float rotation){if(rotation==0)return null;EnsureLiteralState();string saved="SMsavedliteral"+(++literalScope);writer.WriteLine("/"+saved+" SMliteral gstate copy def\ngsave SMliteral setgstate\n"+N(x)+" "+N(y)+" translate "+N(-rotation)+" rotate "+N(-x)+" "+N(-y)+" translate\nSMliteral currentgstate pop grestore");return new LiteralRotation(this,saved);}
  internal void ScoreLiteralProgram(string program){EnsureLiteralState();writer.WriteLine("gsave SMliteral setgstate");writer.WriteLine(program);writer.WriteLine("SMliteral currentgstate pop grestore");}
  internal void ScoreFilledWaveProgram(string program,int repeats){Paths+=repeats;RawProgram(program);}
  internal void ScoreRepeatDot(string x,string y,string radius){Paths++;RawProgram(DefaultInkProgram+"newpath "+x+" "+y+" "+radius+" -270 90 arc eofill\n");}
  internal void ScoreArc(string x,string y,string radius,string ratio,string inverse,int start,int stop,float width,bool fill,bool stroke,PointF pivot,float rotation){
   Paths++;
   string paint=fill?(stroke?"gsave eofill grestore stroke":"eofill"):"stroke";
   string turn=rotation==0?"":N(pivot.X)+" "+N(pivot.Y)+" translate "+N(rotation)+" rotate "+N(-pivot.X)+" "+N(-pivot.Y)+" translate\n";
   RawProgram(DefaultInkProgram+"1 setlinejoin 0 setlinecap "+width.ToString("F4",CultureInfo.InvariantCulture)+" setlinewidth\n"+turn+
    "1 "+ratio+" scale newpath "+x+" "+y+" "+radius+" "+start+" "+stop+" arc\n1 "+inverse+" scale "+paint+"\n");
  }
  // Verified raw SCORE word coordinates, avoiding a float32 GDI projection
  // round trip in EPS. The fixed print profile is shared with NativePageLayout.
  internal void ScorePath(GraphicsPath path,bool fill,double lineWidth,PointF pivot=default(PointF),float rotation=0){if(path.PointCount==0)return;Paths++;writer.WriteLine("gsave "+RawSetup+"\n"+DefaultInkProgram+"1 setlinejoin 0 setlinecap\n"+lineWidth.ToString("0.0000",CultureInfo.InvariantCulture)+" setlinewidth");if(rotation!=0)writer.WriteLine(N(pivot.X)+" "+N(pivot.Y)+" translate "+N(rotation)+" rotate "+N(-pivot.X)+" "+N(-pivot.Y)+" translate");writer.WriteLine("newpath");var points=path.PathPoints;var types=path.PathTypes;for(int i=0;i<points.Length;i++){int type=types[i]&7;if(type!=0&&type!=1)throw new NotSupportedException("Raw SCORE path requires verified line commands.");writer.WriteLine(N(points[i].X)+" "+N(points[i].Y)+(type==0?" moveto":" lineto"));if((types[i]&128)!=0)writer.WriteLine("closepath");}writer.WriteLine(fill?"gsave eofill grestore stroke":"stroke");writer.WriteLine("grestore");}
  internal void ScoreMoveStroke(PointF point,double width,PointF pivot,float rotation){Paths++;string turn=rotation==0?"":N(pivot.X)+" "+N(pivot.Y)+" translate "+N(rotation)+" rotate "+N(-pivot.X)+" "+N(-pivot.Y)+" translate\n";RawProgram(DefaultInkProgram+"1 setlinejoin 0 setlinecap "+width.ToString("0.0000",CultureInfo.InvariantCulture)+" setlinewidth\n"+turn+"newpath "+N(point.X)+" "+N(point.Y)+" moveto stroke\n");}
  internal void ItemMarker(int item){writer.WriteLine("/SMbounditem "+item+" def");}
  internal void Comment(string text){writer.WriteLine("% "+text.Replace("\r"," ").Replace("\n"," "));}
  internal void ScoreGlyph(GraphicsPath path,bool fill,float width,float scale,short x,short y,bool inheritedDefault=false){if(path.PointCount==0)return;Paths++;writer.WriteLine("gsave "+RawSetup+"\n"+DefaultInkProgram+"1 setlinejoin 0 setlinecap\n"+x+" "+y+" translate\n"+scale.ToString("F3",CultureInfo.InvariantCulture)+" dup scale\n"+(literalState&&inheritedDefault?"":width.ToString(inheritedDefault?"F4":"F3",CultureInfo.InvariantCulture)+" setlinewidth\n")+"newpath");var points=path.PathPoints;var types=path.PathTypes;for(int n=0;n<points.Length;n++){int type=types[n]&7;if(type!=0&&type!=1)throw new NotSupportedException("External glyph requires verified line commands.");writer.WriteLine(N(points[n].X)+" "+N(points[n].Y)+(type==0?" moveto":" lineto"));if((types[n]&128)!=0)writer.WriteLine("closepath");}writer.WriteLine(fill?"gsave eofill grestore stroke":"stroke");writer.WriteLine("grestore");}
  static Color ColorOf(Brush b){var solid=b as SolidBrush;if(solid==null||solid.Color.A!=255)throw new NotSupportedException("EPS requires an opaque solid brush.");return solid.Color;}
  internal void Path(GraphicsPath path,Brush brush,Pen pen){if(path.PointCount==0)return;Paths++;writer.WriteLine(literalState?"gsave SMliteral setgstate SMliteralbase matrix invertmatrix concat SMcanvas concat":"gsave");using(var m=target.Transform){float[] a=m.Elements;writer.WriteLine("["+String.Join(" ",Array.ConvertAll(a,v=>N(v)))+"] concat");}
   Color c=ColorOf(pen==null?brush:pen.Brush);writer.WriteLine(N(c.R/255.0)+" "+N(c.G/255.0)+" "+N(c.B/255.0)+" setrgbcolor");
   if(pen!=null){if(pen.Alignment!=PenAlignment.Center||pen.StartCap!=pen.EndCap)throw new NotSupportedException("Unsupported EPS pen alignment/caps.");int cap=pen.StartCap==LineCap.Flat?0:pen.StartCap==LineCap.Round?1:pen.StartCap==LineCap.Square?2:-1;if(cap<0)throw new NotSupportedException("Unsupported EPS line cap.");writer.WriteLine(N(pen.Width)+" setlinewidth "+cap+" setlinecap "+(pen.LineJoin==LineJoin.Round?1:pen.LineJoin==LineJoin.Bevel?2:0)+" setlinejoin "+N(pen.MiterLimit)+" setmiterlimit");if(pen.DashStyle!=DashStyle.Solid){float[] pattern=pen.DashStyle==DashStyle.Dash?new float[]{3,1}:pen.DashStyle==DashStyle.Dot?new float[]{1,1}:pen.DashStyle==DashStyle.DashDot?new float[]{3,1,1,1}:pen.DashStyle==DashStyle.DashDotDot?new float[]{3,1,1,1,1,1}:pen.DashPattern;writer.WriteLine("["+String.Join(" ",Array.ConvertAll(pattern,v=>N(v*pen.Width)))+"] "+N(pen.DashOffset*pen.Width)+" setdash");}}
   writer.WriteLine("newpath");PointF[] pts=path.PathPoints;byte[] types=path.PathTypes;
   for(int i=0;i<pts.Length;i++){int type=types[i]&7;if(type==0)writer.WriteLine(N(pts[i].X)+" "+N(pts[i].Y)+" moveto");else if(type==1)writer.WriteLine(N(pts[i].X)+" "+N(pts[i].Y)+" lineto");else if(type==3){if(i+2>=pts.Length||(types[i+1]&7)!=3||(types[i+2]&7)!=3)throw new InvalidDataException("Incomplete cubic path.");writer.WriteLine(N(pts[i].X)+" "+N(pts[i].Y)+" "+N(pts[i+1].X)+" "+N(pts[i+1].Y)+" "+N(pts[i+2].X)+" "+N(pts[i+2].Y)+" curveto");i+=2;}else throw new NotSupportedException("Unknown GDI path type.");if((types[i]&128)!=0)writer.WriteLine("closepath");}
   writer.WriteLine(pen!=null?"stroke":path.FillMode==FillMode.Alternate?"eofill":"fill");writer.WriteLine("grestore");
  }
  internal void Font(PostScriptFontRun run){if(!fontInitializationWritten){writer.WriteLine(PostScriptFontRun.CopyFont);fontInitializationWritten=true;}writer.WriteLine("gsave");if(run.PageCoordinates)writer.WriteLine(RawSetup+" "+NativePrintContext.PrintedDefault.ToString("F4",CultureInfo.InvariantCulture)+" setlinewidth 1 setlinejoin "+DefaultInkProgram);else{using(var m=target.Transform)writer.WriteLine("["+String.Join(" ",Array.ConvertAll(m.Elements,v=>N(v)))+"] concat");writer.WriteLine(".0333333333333333 -.0333333333333333 scale 0 setgray");}string selected=run.SelectionKey();run.Write(writer,literalState&&selected==lastFontSelection);lastFontSelection=selected;if(run.PageCoordinates&&run.Rotation==0){writer.WriteLine("/SMtextstate gstate def");textState=true;if(literalState)writer.WriteLine("SMliteral currentgstate pop");}if(run.Rotation!=0)lastFontSelection=null;writer.WriteLine("grestore");}
  internal void Text(string text,Font font,Brush brush,float x,float y,StringFormat format){
   throw new NotSupportedException("Windows diagnostic text cannot be engraved in SCORE EPS.");
  }
  public void Dispose(){if(Current!=this)return;
   try{
    if(!aborted){
     writer.WriteLine("grestore");
     writer.WriteLine("showpage");
     writer.WriteLine("SMsave restore end");
     writer.WriteLine("%%Trailer\n%%EOF");
    }
   }finally{Current=null;writer.Dispose();}}
  // Publish only a finished page. A failed render must leave an existing EPS intact.
  static void WritePage(Canvas canvas,Graphics g,string file){
   string destination=System.IO.Path.GetFullPath(file);
   string directory=System.IO.Path.GetDirectoryName(destination);
   string temporary=System.IO.Path.Combine(directory,".score-"+Guid.NewGuid().ToString("N")+".tmp");
   try{
    var active=NativePrintJob.Active;
    var geometry=active??new NativePrintJob{Freeze=true};
    string box="0 0 612 792",boundsWarning=null;
    try{box=(geometry.ResolvedExtents??NativePrintMinimum.Extents(canvas)).DscBox(geometry);}
    catch(NotSupportedException error){boundsWarning="SCORE bounding scan unavailable. Page bounds used. "+error.Message;}
    canvas.PageDocument.DatBarlineUnit=NativePrintContext.ExportGeometryUnit;
    bool exited=false;
    using(var eps=new NativeEpsExport(g,temporary,canvas.PageWidth,canvas.PageHeight,box,boundsWarning,canvas.Score)){
     try{canvas.Draw(g,false);}
     catch(NativePrintFinishException){}
     catch(NativePrintExitException){eps.Abort();exited=true;}
     catch{eps.Abort();throw;}
    }
    if(exited&&!NativePrintBehaviour.RetainIncomplete)throw new NativePrintExitException();
    if(File.Exists(destination))File.Replace(temporary,destination,null);
    else File.Move(temporary,destination);
    if(exited)throw new NativePrintExitException();
   }finally{File.Delete(temporary);}
  }
  public static void Write(Canvas canvas,string root,string file){
   using(var bitmap=new Bitmap(1,1))using(var g=Graphics.FromImage(bitmap)){
    canvas.LayoutScore();
    if(canvas.PageDocument.Issues.Count!=0)throw new NotSupportedException(String.Join("\r\n",canvas.PageDocument.Issues.ToArray()));
    using(NativePrintContext.Begin(root))WritePage(canvas,g,file);
   }
  }
  public static int Run(string[] args){try{if(args.Length<4)throw new ArgumentException("--export-eps SCORE_ROOT MUS_FILE OUTPUT_EPS");NativePrintJob job=NativePrintJob.Defaults(args[1]);foreach(string arg in args)if(arg.StartsWith("--print-settings="))job=NativePrintJob.Read(arg.Substring(17),job);var score=Score.Load(args[2]);using(var canvas=new Canvas())using(var bitmap=new Bitmap(1,1))using(var g=Graphics.FromImage(bitmap)){canvas.Library=new DrawingLibrary(args[1]);canvas.TextLayout=new ScoreText(args[1]);canvas.Score=score;canvas.Scor4Interface=NativeLibraryLocation.IsScor4(args[1]);canvas.LayoutScore();if(canvas.PageDocument.Issues.Count!=0)throw new NotSupportedException(String.Join("; ",canvas.PageDocument.Issues.ToArray()));if(job!=null)job.Resolve(canvas);using(var jobScope=job==null?null:job.Begin())using(NativePrintContext.Begin(args[1])){WritePage(canvas,g,args[3]);Console.WriteLine("PASS vector EPS.");}}return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 }
 public static class NativeVectorDrawing {
  static void Record(Graphics g,GraphicsPath p,Brush b,Pen pen){NativeItemBounds.Path(g,p,pen);var e=NativeEpsExport.For(g);if(e!=null)e.Path(p,b,pen);}
  public static void VectorDrawPath(this Graphics g,Pen pen,GraphicsPath p){NativeItemPaint.Path(g,p,pen,(copy,value)=>g.DrawPath(value,copy));Record(g,p,null,pen);}
  public static void VectorFillPath(this Graphics g,Brush b,GraphicsPath p){NativeItemPaint.Path(g,p,b,(copy,value)=>g.FillPath(value,copy));Record(g,p,b,null);}
  public static void VectorDrawLine(this Graphics g,Pen p,float x,float y,float x2,float y2){NativeItemPaint.Paint(g,p,value=>g.DrawLine(value,x,y,x2,y2));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddLine(x,y,x2,y2);Record(g,a,null,p);}}
  public static void VectorDrawLines(this Graphics g,Pen p,PointF[] v){NativeItemPaint.Paint(g,p,value=>g.DrawLines(value,v));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddLines(v);Record(g,a,null,p);}}
  public static void VectorDrawPolygon(this Graphics g,Pen p,PointF[] v){NativeItemPaint.Paint(g,p,value=>g.DrawPolygon(value,v));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddPolygon(v);Record(g,a,null,p);}}
  public static void VectorFillPolygon(this Graphics g,Brush b,PointF[] v){g.VectorFillPolygon(b,v,FillMode.Alternate);}
  public static void VectorFillPolygon(this Graphics g,Brush b,PointF[] v,FillMode mode){NativeItemPaint.Paint(g,b,value=>g.FillPolygon(value,v,mode));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath(mode)){a.AddPolygon(v);Record(g,a,b,null);}}
  public static void VectorDrawEllipse(this Graphics g,Pen p,float x,float y,float w,float h){NativeItemPaint.Paint(g,p,value=>g.DrawEllipse(value,x,y,w,h));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddEllipse(x,y,w,h);Record(g,a,null,p);}}
  public static void VectorFillEllipse(this Graphics g,Brush b,float x,float y,float w,float h){NativeItemPaint.Paint(g,b,value=>g.FillEllipse(value,x,y,w,h));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddEllipse(x,y,w,h);Record(g,a,b,null);}}
  public static void VectorDrawRectangle(this Graphics g,Pen p,float x,float y,float w,float h){NativeItemPaint.Paint(g,p,value=>g.DrawRectangle(value,x,y,w,h));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddRectangle(new RectangleF(x,y,w,h));Record(g,a,null,p);}}
  public static void VectorFillRectangle(this Graphics g,Brush b,float x,float y,float w,float h){NativeItemPaint.Paint(g,b,value=>g.FillRectangle(value,x,y,w,h));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddRectangle(new RectangleF(x,y,w,h));Record(g,a,b,null);}}
  public static void VectorDrawBezier(this Graphics g,Pen p,float x,float y,float x1,float y1,float x2,float y2,float x3,float y3){NativeItemPaint.Paint(g,p,value=>g.DrawBezier(value,x,y,x1,y1,x2,y2,x3,y3));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddBezier(x,y,x1,y1,x2,y2,x3,y3);Record(g,a,null,p);}}
  public static void VectorFillPie(this Graphics g,Brush b,float x,float y,float w,float h,float start,float sweep){NativeItemPaint.Paint(g,b,value=>g.FillPie(value,x,y,w,h,start,sweep));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddPie(x,y,w,h,start,sweep);Record(g,a,b,null);}}
  public static void VectorDrawArc(this Graphics g,Pen p,RectangleF r,float start,float sweep){NativeItemPaint.Paint(g,p,value=>g.DrawArc(value,r,start,sweep));if(NativeEpsExport.For(g)!=null||NativeItemBounds.Active)using(var a=new GraphicsPath()){a.AddArc(r,start,sweep);Record(g,a,null,p);}}
  public static void VectorDrawString(this Graphics g,string s,Font f,Brush b,float x,float y){if(RenderResourceAudit.WindowsText(s))return;g.DrawString(s,f,b,x,y);var e=NativeEpsExport.For(g);if(e!=null)e.Text(s,f,b,x,y,StringFormat.GenericDefault);}
  public static void VectorDrawString(this Graphics g,string s,Font f,Brush b,float x,float y,StringFormat format){if(RenderResourceAudit.WindowsText(s))return;g.DrawString(s,f,b,x,y,format);var e=NativeEpsExport.For(g);if(e!=null)e.Text(s,f,b,x,y,format);}
 }
}




