using System;using System.IO;using System.Text;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 internal sealed class PostScriptFontRun {
  public string Name;public short Word,RequestedWord;public float X,Y,EmX,EmY,Letter,Space,Rotation,PivotX,PivotY;public bool PageCoordinates=true,ReencodedUgrave;
  public readonly Dictionary<byte,string> ExtraEncoding=new Dictionary<byte,string>();
  public readonly List<byte> Bytes=new List<byte>();public Rectangle Crop;public Bitmap Image;public float[] World;
  internal static string CopyFont{get{return NativeFontInitialization.Prolog(NativePrintContext.Root,DefaultCopyFont);}}
  internal const string DefaultCopyFont="/SMCopyFont {dup maxlength 3 -1 roll add dict begin {1 index /FID eq 2 index /UniqueID eq or {pop pop} {def} ifelse} forall /PaintType 2 def currentdict end} bind def";
  internal static string N(double n){if(Double.IsNaN(n)||Double.IsInfinity(n))throw new NotSupportedException("Non-finite PostScript text coordinate");return n.ToString("0.############",CultureInfo.InvariantCulture);}
  void ApplyExternalEncoding(){if(!NativeFontInitialization.External)return;var table=NativeFontInitialization.Read(NativePrintContext.Root).Encoding;foreach(byte code in Bytes){string name;if(table.TryGetValue(code,out name))ExtraEncoding[code]=name;}}
  internal string SelectionKey(){ApplyExternalEncoding();var key=new StringBuilder(Name+"|"+Word+"|"+N(EmX)+"|"+N(EmY)+"|"+ReencodedUgrave);var codes=new List<byte>(ExtraEncoding.Keys);codes.Sort();foreach(byte code in codes)key.Append("|").Append(code).Append(":").Append(ExtraEncoding[code]);return key.ToString();}
  public void Write(TextWriter w,bool reuseFont=false){ApplyExternalEncoding();
   if(!System.Text.RegularExpressions.Regex.IsMatch(Name??"",@"^[A-Za-z0-9_-]+$"))throw new InvalidDataException("Unsupported PostScript font name");
   if(Rotation!=0)w.WriteLine(N(PivotX)+" "+N(PivotY)+" translate "+((double)-Rotation).ToString("F2",CultureInfo.InvariantCulture)+" rotate "+N(-PivotX)+" "+N(-PivotY)+" translate");
   if(!reuseFont){string face=Name;if(NativeFontInitialization.External){w.WriteLine("/"+Name+" /SMinitial acc ReE");face="SMinitial";}if(ReencodedUgrave){w.WriteLine("/SMaccent /"+face+" findfont dup length dict begin {1 index /FID ne {def}{pop pop} ifelse} forall /Encoding Encoding 256 array copy def Encoding 22 /ugrave put currentdict end definefont pop");face="SMaccent";}if(ExtraEncoding.Count>0){w.WriteLine("/SMspecial /"+face+" findfont dup length dict begin {1 index /FID ne {def}{pop pop} ifelse} forall /Encoding Encoding 256 array copy def");foreach(var pair in ExtraEncoding)w.WriteLine("Encoding "+pair.Key+" /"+pair.Value+" put");w.WriteLine("currentdict end definefont pop");face="SMspecial";}if(Word>=100){w.WriteLine("/SMfont 2 /"+face+" findfont SMCopyFont definefont pop");face="SMfont";}
   w.WriteLine("/"+face+" findfont ["+((double)EmX).ToString("F3",CultureInfo.InvariantCulture)+" 0 0 "+((double)EmY).ToString("F3",CultureInfo.InvariantCulture)+" 0 0] makefont setfont");}w.Write(N(X)+" "+N(Y)+" moveto ");
   if(Letter!=0||Space!=0)w.Write(((double)Space).ToString("F2",CultureInfo.InvariantCulture)+" 0 32 "+((double)Letter).ToString("F2",CultureInfo.InvariantCulture)+" 0 ");
   w.WriteLine("<"+BitConverter.ToString(Bytes.ToArray()).Replace("-","")+"> "+((Letter!=0||Space!=0)?"awidthshow":"show"));
  }
  public float[] PixelMatrix(){
   double a=World[0],b=World[1],c=World[2],d=World[3],tx=World[4],ty=World[5];
   if(PageCoordinates){tx+=a*70+c*(16750.0/30);ty+=b*70+d*(16750.0/30);}
   return new[]{(float)(a/30),(float)(b/30),(float)(-c/30),(float)(-d/30),(float)tx,(float)ty};
  }
  public Rectangle Bounds(Rectangle clip){ApplyExternalEncoding();
   var face=NativeTupletFonts.Find(Name);if(face==null)throw new InvalidDataException(NativeTupletFonts.Status);RectangleF box=RectangleF.Empty;bool ink=false;double advance=0;
   // The bounded Code9 piu request uses u for conservative crop estimation;
   // its existing full-em padding is checked against the original accent page.
   foreach(byte ch in Bytes){char cropCharacter=ReencodedUgrave&&ch==22?'u':(char)ch;float width;string glyph;using(var path=ExtraEncoding.TryGetValue(ch,out glyph)?face.NamedCharacter(glyph,Word>=100,out width):Word>=100?face.OutlineCharacter(cropCharacter,out width):face.Character(cropCharacter,out width)){
    if(path.PointCount>0){using(var m=new Matrix(EmX/1000,0,0,EmY/1000,(float)(X+advance),Y))path.Transform(m);if(Rotation!=0)using(var m=new Matrix()){m.RotateAt(-Rotation,new PointF(PivotX,PivotY));path.Transform(m);}var p=PixelMatrix();using(var m=new Matrix(p[0],p[1],p[2],p[3],p[4],p[5]))path.Transform(m);var bounds=path.GetBounds();box=ink?RectangleF.Union(box,bounds):bounds;ink=true;}
   }advance+=(double)width*EmX/1000+Letter+(ch==32?Space:0);}
   if(!ink)return Rectangle.Empty;double scale=Math.Max(Math.Sqrt(World[0]*World[0]+World[1]*World[1]),Math.Sqrt(World[2]*World[2]+World[3]*World[3]));
   // Conservative padding around actual external outlines. Clipping is checked
   // against full-page reference renders in the backend verification suite.
   double pad=Math.Max(Math.Abs(EmX),Math.Abs(EmY))*scale/30+16;
   var result=Rectangle.FromLTRB((int)Math.Floor(box.Left-pad),(int)Math.Floor(box.Top-pad),(int)Math.Ceiling(box.Right+pad),(int)Math.Ceiling(box.Bottom+pad));result.Intersect(clip);
   if((long)result.Width*result.Height>100000000)throw new NotSupportedException("Text raster region exceeds the configured limit");return result;
  }
 }
 internal sealed class PostScriptTextPart {public PostScriptFontRun Font;public NativeTextToken Vector;public float X;}
 internal sealed class PostScriptTextPlan {
  public NativeTextLayout Layout;public NativeNotationContext Page;public float X,Y,Staff;public float? VectorStrokeWidth;public readonly List<PostScriptTextPart> Parts=new List<PostScriptTextPart>();
 }
 public sealed partial class ScoreText {
  internal PostScriptTextPlan PreparePostScript(Item item,NativeNotationContext page,float[] world){
   var layout=PrepareLayout(item,item.Text,page.Scale,0);if(!layout.VectorSupported)return null;var ts=PrintedRenderingTokens(layout,(short)item.P(8),0);bool forced=TextConverterFontState.Begin((short)item.P(8),0).Forced;var values=PrintedValues(layout,page.Scale);
   var origin=TextParameterPreparation.Prepare(TextParameters(item),page.Prepared.X,100,.2800000011920929f,150);var plan=new PostScriptTextPlan{Layout=layout,Page=page,X=origin.Origin,Y=page.Prepared.Y,Staff=page.Scale,VectorStrokeWidth=VectorTextStrokeWidth((short)item.P(8),layout.FinalFont)};float x=plan.X;PostScriptFontRun current=null;
   foreach(var t in ts){if(t.TabX.HasValue){x=t.TabX.Value;current=null;continue;}if(t.AdvanceOnly){x=TextMetricArithmetic.AddPosition(1,x,t.AdjustedAdvance,150);continue;}if(PrintedModifier(t.Code))continue;if(t.Code>=1000){if(!forced)current=null;continue;}
    if(PrintedMusic(t.Code)||t.FontWord%100>=90){current=null;plan.Parts.Add(new PostScriptTextPart{Vector=t,X=x});}
    else {if(t.Character>255)return null;if(current==null||current.RequestedWord!=t.FontWord){var selectedFont=PrintedFontCarry.Select(MissingPrintedFace(t.FontWord%100)?null:ExternalFontName(t.FontWord%100),values.EmX,values.EmY,t.FontWord>=100);current=new PostScriptFontRun{Name=selectedFont.Name,RequestedWord=t.FontWord,Word=(short)(t.FontWord%100+(selectedFont.Outline?100:0)),X=NativeNotationContext.RawWord(x),Y=NativeNotationContext.RawWord(plan.Y),EmX=selectedFont.EmX,EmY=selectedFont.EmY,Letter=values.Letter,Space=values.Space,Rotation=values.Rotation,PivotX=NativeNotationContext.RawWord(plan.X),PivotY=NativeNotationContext.RawWord(plan.Y),World=world};plan.Parts.Add(new PostScriptTextPart{Font=current});}current.Bytes.Add((byte)t.Character);if(t.GlyphName!=null)current.ExtraEncoding[(byte)t.Character]=t.GlyphName;}
    x=TextMetricArithmetic.AddPosition(1,x,t.AdjustedAdvance,150);
   }return plan;
  }
  internal void PaintPostScript(Graphics g,PostScriptTextPlan plan,float? vectorStrokeWidth=null){
   var state=g.Save();try{float pivotX=NativeNotationContext.RawWord(plan.X),pivotY=NativeNotationContext.RawWord(plan.Y);var p=plan.Page.Point(pivotX,pivotY);g.TranslateTransform(p.X,p.Y);g.RotateTransform(PrintedValues(plan.Layout,plan.Staff).Rotation);
    foreach(var part in plan.Parts)if(part.Font!=null){
     RenderResourceAudit.Record("PostScript font run",part.Font.Name,NativeTupletFonts.DirectoryPath,"explicit external PFB/PFA lookup for screen; EPS font reference");
     GhostscriptTextRenderer.Paint(g,part.Font);
    }
    using(var vectors=new VectorTextFrame(g,true,vectorStrokeWidth??plan.VectorStrokeWidth,new PointF(pivotX,pivotY),-PrintedValues(plan.Layout,plan.Staff).Rotation)){
     foreach(var part in plan.Parts)if(part.Vector!=null)DrawVectorToken(vectors,part.Vector,plan.Layout,plan.Staff,part.X,plan.Y,pivotX,pivotY);
    }
    RenderStatus="Ghostscript Type-1 text; external SCORE vector resources";
   }finally{g.Restore(state);}
  }
 }
}

