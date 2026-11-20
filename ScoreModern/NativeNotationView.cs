using System;using System.IO;using System.Drawing;using System.Drawing.Drawing2D;using System.Collections.Generic;using System.Windows.Forms;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public enum NotationView {Vector,Dat,DatPixels,DatViewport}
 public static class NativeNotationView {
  public static NotationView Mode{get;private set;}
  [ThreadStatic] public static bool PixelPass;[ThreadStatic] static bool printing;
  public static bool IsDosViewport{get{return UsesDat&&Mode==NotationView.DatViewport;}}
  internal static bool IsPrinting{get{return printing;}}
  public static bool UsesDat{get{return Mode!=NotationView.Vector&&!printing&&NativeEpsExport.Current==null;}}
  static string Settings{get{return "notation";}}
  public static void Load(){Mode=NotationView.Vector;try{if(NativeSettings.Exists(Settings)){string s=NativeSettings.ReadAllText(Settings).Trim();if(s=="dat")Mode=NotationView.Dat;else if(s=="dat-pixels")Mode=NotationView.DatPixels;else if(s=="dos-viewport")Mode=NotationView.DatViewport;}}catch(IOException){}}
  // Synchronous retained-array scans must not relayout every open form.
  internal static void SetForScan(NotationView mode){Mode=mode;}
  public static void Set(NotationView mode,bool save){Mode=mode;if(save)NativeSettings.WriteAllText(Settings,mode==NotationView.Dat?"dat":mode==NotationView.DatPixels?"dat-pixels":mode==NotationView.DatViewport?"dos-viewport":"vector");foreach(Form form in Application.OpenForms){ResizeCanvases(form);form.Invalidate(true);}}
  static void ResizeCanvases(Control c){var canvas=c as Canvas;if(canvas!=null)canvas.LayoutScore();foreach(Control child in c.Controls)ResizeCanvases(child);}
  public static void AddMenu(ContextMenuStrip menu){menu.Items.Add(new ToolStripSeparator());var high=new ToolStripMenuItem("Notation: high-resolution WYSIWYG");var dat=new ToolStripMenuItem("Notation: DAT shapes at native resolution");var pixels=new ToolStripMenuItem("Notation: DAT shapes on DOS-style pixel grid");high.Click+=(s,e)=>Set(NotationView.Vector,true);dat.Click+=(s,e)=>Set(NotationView.Dat,true);pixels.Click+=(s,e)=>Set(NotationView.DatPixels,true);menu.Items.Add(high);menu.Items.Add(dat);menu.Items.Add(pixels);var original=new ToolStripMenuItem("Notation: original DOS viewport");original.Click+=(s,e)=>Set(NotationView.DatViewport,true);menu.Items.Add(original);NativeDosViewport.AddMenu(menu);menu.Opening+=(s,e)=>{high.Checked=Mode==NotationView.Vector;dat.Checked=Mode==NotationView.Dat;pixels.Checked=Mode==NotationView.DatPixels;original.Checked=Mode==NotationView.DatViewport;};}
  public static bool DrawPixelPage(Canvas canvas,Graphics target,bool paper){
   if(!UsesDat||Mode!=NotationView.DatPixels||PixelPass)return false;bool framed=canvas.UsesOriginalZoomFrame;float pixelScale=framed?30f/NativeDosViewport.Divisor:640/canvas.PageWidth;var origin=framed?canvas.CameraOffset:canvas.ScreenOrigin;float viewWidth=canvas.ScreenFraming||framed?canvas.ViewWidth:canvas.PageWidth,viewHeight=canvas.ScreenFraming||framed?canvas.ViewHeight:canvas.PageHeight;int width=Math.Max(1,(int)Math.Ceiling(viewWidth*pixelScale)),height=Math.Max(1,(int)Math.Ceiling(viewHeight*pixelScale));
   using(var bitmap=new Bitmap(width,height))using(var g=Graphics.FromImage(bitmap)){Color background=Color.White;g.Clear(canvas.ScreenFraming?Color.Transparent:background);g.ScaleTransform(canvas.ScreenFraming||framed?pixelScale:width/canvas.PageWidth,canvas.ScreenFraming||framed?pixelScale:height/canvas.PageHeight);g.TranslateTransform(origin.X,origin.Y);PixelPass=true;try{canvas.Draw(g,paper);}finally{PixelPass=false;}var bits=bitmap.LockBits(new Rectangle(0,0,width,height),System.Drawing.Imaging.ImageLockMode.ReadWrite,System.Drawing.Imaging.PixelFormat.Format32bppArgb);try{byte[] bytes=new byte[bits.Stride*height];System.Runtime.InteropServices.Marshal.Copy(bits.Scan0,bytes,0,bytes.Length);for(int y=0;y<height;y++)for(int x=0;x<width;x++){int at=y*bits.Stride+x*4;bool ink=bytes[at+3]!=0&&bytes[at]+bytes[at+1]+bytes[at+2]<384;bytes[at]=ink?(byte)0:background.B;bytes[at+1]=ink?(byte)0:background.G;bytes[at+2]=ink?(byte)0:background.R;bytes[at+3]=canvas.ScreenFraming&&!ink?(byte)0:(byte)255;}System.Runtime.InteropServices.Marshal.Copy(bytes,0,bits.Scan0,bytes.Length);}finally{bitmap.UnlockBits(bits);}var state=target.Save();try{target.InterpolationMode=InterpolationMode.NearestNeighbor;target.PixelOffsetMode=PixelOffsetMode.Half;target.DrawImage(bitmap,new RectangleF(-origin.X,-origin.Y,viewWidth,viewHeight),new RectangleF(0,0,width,height),GraphicsUnit.Pixel);}finally{target.Restore(state);}}return true;
  }
  public static void Print(Canvas canvas,Graphics g){bool old=printing;printing=true;try{canvas.Draw(g,false);}finally{printing=old;}}
 }
 public static class NativeDatDrawing {
  static string loadedRoot;static NativeDatGlyphs glyphs;static NativeDatHeadCache heads;
  [ThreadStatic] public static Action<string> Report;[ThreadStatic] static short textFont;
  public static void BeginFrame(){textFont=0;if(heads!=null)heads.GridState=null;}
  internal static NativeDatHeadCache HeadCache{get{return heads;}}
  internal static NativeDatGlyphs Get(DrawingLibrary library){if(library==null)throw new InvalidDataException("SCORE LIB directory is required for DAT display.");if(glyphs==null||!String.Equals(loadedRoot,NativeLibraryLocation.CacheKey(library.Root),StringComparison.OrdinalIgnoreCase)){glyphs=new NativeDatGlyphs(library.Root);heads=new NativeDatHeadCache();loadedRoot=NativeLibraryLocation.CacheKey(library.Root);}return glyphs;}
  static bool Attempt(string route,Action draw){try{draw();return true;}catch(NotSupportedException e){Issue(route,e.Message);return false;}catch(InvalidDataException e){Issue(route,e.Message);return false;}catch(IOException e){Issue(route,e.Message);return false;}}
  static void Issue(string route,string detail){
   RenderResourceAudit.Unavailable("DAT "+route,detail);
   string message="DAT "+route+": "+detail;
   if(!NativeApproximation.Enabled)throw new NotSupportedException(message);
   if(Report!=null)Report(message+" Vector rendering used because approximation is enabled.");
  }
  public static void Points(Graphics g,NativeNotationContext page,IEnumerable<PrimitiveTextPoint> points){var lines=new List<BarlinePoint>();bool pending=false;foreach(var p in points){if(p.CommandOnly){if(p.Operation!=1&&p.Operation!=2&&p.Operation!=99)throw new NotSupportedException("DAT standalone command needs verification.");pending=true;continue;}lines.Add(new BarlinePoint(p.X,p.Y,(short)(p.Operation>0||pending?1:0)));pending=false;}NativeNoteDetails.Path(g,page,lines,NativePrintContext.DefaultLineWidth);}
  public static bool Item(Graphics g,Item item,NativeNotationContext page,NativePageDocument doc,DrawingLibrary library,ScoreText text){if(!NativeNotationView.UsesDat||page==null)return false;
   return Attempt("item "+item.Code,()=>{short next;var points=Prepare(item,page,doc,library,text,Get(library),heads,textFont,out next);if(item.Code==8&&item.Staff==1)Points(g,page,NativeScanStructuralVectors.ScreenGuide(item,doc));Points(g,page,points);textFont=next;});
  }
  public static List<PrimitiveTextPoint> Prepare(Item item,NativeNotationContext page,NativePageDocument doc,DrawingLibrary library,ScoreText text,NativeDatGlyphs dat,NativeDatHeadCache cache,short previousFont,out short finalFont){
   return Prepare(item,page,doc,library,text,dat,cache,previousFont,out finalFont,false);
  }
  internal static List<PrimitiveTextPoint> Prepare(Item item,NativeNotationContext page,NativePageDocument doc,DrawingLibrary library,ScoreText text,NativeDatGlyphs dat,NativeDatHeadCache cache,short previousFont,out short finalFont,bool actualScreenFonts){
   finalFont=previousFont;
   if(item.Code==19)return new List<PrimitiveTextPoint>();
   if(item.Code==3){short? font;var points=NativeDatClefs.Prepare(item,page,dat,library,text,out font);if(font.HasValue)finalFont=font.Value;return points;}
   if(item.Code==7){var points=NativeDatOrnaments.Prepare(item,page,dat,library,text);short type=unchecked((short)(int)item.P(7));if(type==8||type==-8||type==15)finalFont=2;else if(type==88)finalFont=2;return points;}
   if(item.Code==12&&item.P(5)<0){if(NativeEditCompatibility.ReproduceInvalidStates&&cache.GridState==null)cache.GridState=NativePrintedChordGrid.InitialState(doc,page.SourceScore,text);return NativePrintedChordGrid.Scan(item,page,text,dat,previousFont,out finalFont,NativeEditCompatibility.ReproduceInvalidStates?cache.GridState:null);}
   if(item.Code==12||item.Code==13||item.Code==15)return NativeDatSpecialItems.Prepare(item,page,dat);
   if(item.Code==10)return NativeDatNumbers.Prepare(item,page,text,dat,out finalFont);
   if(item.Code==11)return NativeDatUserDrawings.Prepare(item,page,library);
   if(item.Code==18&&item.P(5)<90&&(short)(int)item.P(8)!=0)return NativeDatMeters.Prepare(item,page,text,dat);
   if(item.Code==2)return NativeDatRestAttachments.PrepareItem(item,page,dat,library,text);
   if(NativeTablatureDrawing.IsTab(item))return NativeTablatureDrawing.Scan(item,page,text,previousFont,out finalFont);
   if(item.Code==1)return NativeScanMusicVectors.Note(item,page,dat,cache,text,previousFont,out finalFont);
   if(item.Code==16)return text.ScanStrokeText(item,item.Text,page,previousFont,out finalFont);
   if(item.Code==9&&NativeCode9Terms.IsTerm((int)item.P(5)))return NativeCode9Terms.Scan(item,page,text,previousFont,out finalFont);
   if(item.Code==9&&NativeLibrarySymbols.IsTextAlias((int)item.P(5))&&(short)(int)item.P(11)>=0)return NativeLibrarySymbols.ScanTextAlias(item,page,text,out finalFont,previousFont);
   if(item.Code==4||item.Code==5||item.Code==6){short? labelFont;var points=NativeScanDynamicVectors.Prepare(item,page,library.Root,text,out labelFont,actualScreenFonts);if(labelFont.HasValue)finalFont=labelFont.Value;return points;}
   if(item.Code==8){var staff=NativeScanStructuralVectors.Prepare(item,doc,library);
    // Default F6 caller compares working P2 with its external
    // staff selector (measured 1.0) and clears that staff's working P8 at05B5.
    // Other staff records retain P8, including the tested negative/hidden cases.
    if(item.Staff!=1)staff.InsertRange(0,NativeScanStructuralVectors.ScreenGuide(item,doc));return staff;}
   if(item.Code==14){return NativeDatBarlines.Prepare(item,doc,library,actualScreenFonts);}
   return NativeScanMusicVectors.Prepare(item,page,dat,library,text);
  }

  public static bool Head(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library){if(!NativeNotationView.UsesDat||page==null)return false;return Attempt("notehead",()=>{var dat=Get(library);Points(g,page,NativeScanMusicVectors.Head(item,page,dat,heads));});}
  public static bool Flags(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library){if(!NativeNotationView.UsesDat||page==null)return false;return Attempt("flags",()=>Points(g,page,NativeScanMusicVectors.Flags(item,page,Get(library))));}
  public static bool Primitive(Graphics g,NativeNotationContext page,DrawingLibrary library,PrimitiveGlyphRequest request){if(!NativeNotationView.UsesDat||page==null)return false;return Attempt("glyph "+request.Glyph,()=>Points(g,page,Get(library).Expand(request.Glyph,request.Horizontal,request.Vertical,page.Scale,request.X,request.Y)));}
  public static bool Dots(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library){if(!NativeNotationView.UsesDat||page==null)return false;return Attempt("dots",()=>{var p=NativeNoteDetails.PrepareDat(item,page);if(p==null)throw new NotSupportedException("Dot request outside the recovered controls.");float x=p.DotX;for(int n=0;n<p.DotCount;n++){Points(g,page,Get(library).Expand(24,p.DotScale,p.DotScale,page.Scale,x,p.DotY));x=(float)((double)x+p.Scale*13);}});}
 }
}
