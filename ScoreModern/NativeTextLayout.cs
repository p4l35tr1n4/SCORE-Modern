using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class NativeTextToken {
  public short Code,FontWord,DatMetricFace;public char Character;public string GlyphName;public float Advance,AdjustedAdvance,Position;internal float Metric;internal bool AdvanceOnly;internal float? TabX;
 }
 public sealed class NativeTextLayout {
  public TextPreparedParameters Parameters;public TextMetricScale Scale;
  internal int RawCharacterCount;public float Width,LetterExtra;public short FinalFont;public bool ExternalType1=true,VectorSupported=true,MetricsSupported=true;
  public readonly List<NativeTextToken> Tokens=new List<NativeTextToken>();
 }
 public struct NativePrintedTextValues {public float EmX,EmY,Letter,Space,Rotation;}
 public sealed partial class ScoreText {
  byte[] textDat;
  public string RenderStatus{get;private set;}
  static float[] TextParameters(Item item){var p=new float[21];for(int n=1;n<=20;n++)p[n]=item.P(n);return p;}
  public static bool FontControl(string text,int p,out int id,out int length){
   id=0;length=0;if(p>=text.Length||text[p]!='_')return false;int first=p+1;bool outline=first<text.Length&&text[first]=='_';if(outline)first++;
   if(first+1>=text.Length||text[first]<'0'||text[first]>'9'||text[first+1]<'0'||text[first+1]>'9')return false;
   id=(text[first]-'0')*10+text[first+1]-'0'+(outline?100:0);length=first+2-p;return true;
  }
  internal bool HasOriginalTabControl(string text){for(int n=0;n<text.Length;n++){float stop;int end;if(ReadTab(text,n,out stop,out end))return true;}return false;}
  static bool ReadTab(string text,int start,out float x,out int end){
   x=0;
   end=start;
   if(text[start]!='~'||start+1==text.Length)return false;
   char first=text[start+1];
   if((first<'0'||first>'9')&&first!='+'&&first!='-'&&first!='.')return false;
   end=text.IndexOf('~',start+1);
   float stop;
   if(end<0||!Single.TryParse(text.Substring(start+1,end-start-1),NumberStyles.AllowLeadingSign|NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out stop))return false;
   if(Single.IsInfinity(stop)||Math.Abs(stop)>200)throw new NotSupportedException("SCORE text tab is outside the verified horizontal range.");
   x=(float)((double)stop*150);
   return true;
  }
  internal float LastPrintedGlyphOrigin(Item item,string text,NativeNotationContext page){
   var layout=PrepareLayout(item,text,page.Scale,0);var tokens=RenderingTokens(layout,(short)(int)item.P(8),0);
   var origin=TextParameterPreparation.Prepare(TextParameters(item),page.Prepared.X,100,.2800000011920929f,150);float x=origin.Origin;
   int last=tokens.Count-1;while(last>=0&&(tokens[last].Code>=1000||tokens[last].Code>=83&&tokens[last].Code<=87))last--;
   for(int n=0;n<last;n++){var token=tokens[n];if(token.TabX.HasValue)x=token.TabX.Value;else x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);}return x;
  }
  public NativeTextLayout PrepareLayout(Item item,string text,float staffScale,short previousFont){return PrepareLayout(item,text,staffScale,previousFont,false);}
  internal NativeTextLayout PrepareMetricLayout(Item item,string text,float staffScale,short previousFont){return PrepareLayout(item,text,staffScale,previousFont,false,true);}
  public NativeTextLayout PrepareLayout(Item item,string text,float staffScale,short previousFont,bool datControls){return PrepareLayout(item,text,staffScale,previousFont,datControls,false);}
  NativeTextLayout PrepareLayout(Item item,string text,float staffScale,short previousFont,bool datControls,bool originalMetrics){
   if(item.P(8)==99)text=NativeRawPostScript.Normalize(text);
   if(datControls){int controls=0;bool missing=false;for(int q=0;q<text.Length;q++){int face,length;if(!FontControl(text,q,out face,out length))continue;controls++;face%=100;if((face==35||face==41)&&!faces.ContainsKey(face))missing=true;q+=length-1;}if(missing&&controls>1)throw new NotSupportedException("DAT missing-metric switches within one string need verification.");}
   var result=new NativeTextLayout();result.RawCharacterCount=text.Length;result.Parameters=TextParameterPreparation.Prepare(TextParameters(item),0,100,.2800000011920929f,150);
   result.Scale=TextMetricArithmetic.Prepare(result.Parameters.Horizontal,staffScale,25,3.2832999229431152f,item.P(10));
   var font=TextConverterFontState.Begin(unchecked((short)(int)item.P(8)),previousFont);float total=0;
   if(textDat==null)textDat=File.ReadAllBytes(NativeLibraryLocation.FileFor(RootDirectory,"SCOR3_10.DAT"));
   int printedAccent=0;for(int p=0;p<text.Length;p++){
    int id,length,tabEnd;float tabX;var t=new NativeTextToken();
    if(ReadTab(text,p,out tabX,out tabEnd)){
     // SCORE stores the tab in Code16 text. It resets the drawing origin
     // without contributing to the width cached in P13.
     t.TabX=tabX;
     t.FontWord=font.Word;
     result.Tokens.Add(t);
     p=tabEnd;
     continue;
    }
    if(FontControl(text,p,out id,out length)){t.Code=(short)(font.Forced?1100:1000+id);font.Accept(t.Code);if(datControls)t.DatMetricFace=(short)DatMetricFace(font.MetricFace);else t.DatMetricFace=(short)PrintedMetricFace(font.MetricFace);p+=length-1;}
    else {
     char c=text[p];int code;if(datControls)code=DatCharacter(text,ref p);else{code=PrintedCharacter(text,ref p,font.Word,ref printedAccent,out c,out t.GlyphName);t.GlyphName=NativeFontInitialization.Glyph(RootDirectory,c,t.GlyphName);}t.Character=c;t.Code=(short)code;
     t.DatMetricFace=(short)(datControls?DatMetricFace(font.MetricFace):PrintedMetricFace(font.MetricFace));
     if(code<0){result.ExternalType1=false;result.VectorSupported=false;result.MetricsSupported=false;t.Advance=Advance(c,font.MetricFace)*staffScale*result.Parameters.Horizontal*144/28;}
     else {
      if(datControls&&code>167)throw new NotSupportedException("DAT raised-text control needs verification: "+code);
      int index=code+1;float metric;Face face;
      if(code>=162){metric=code==162?.5f:code==166?.949999988079071f:.800000011920929f;}else
      if((datControls&&code>=83&&code<=87)||(!datControls&&PrintedModifier(code))){metric=0;}else
      if(font.MetricFace>=90||(index>=81&&index<84)||(index>=89&&index<=93)){int metricIndex=index;if((datControls||originalMetrics||font.Word%100==99||(font.MetricFace>=90&&font.MetricFace<=94)||font.MetricFace==98)&&font.MetricFace>=90){if(metricIndex==150)metricIndex=94;if(metricIndex==146||metricIndex==148)metricIndex=101+(metricIndex-146)/2;if(metricIndex>=133&&metricIndex<=141)metricIndex-=36;}metric=BitConverter.ToSingle(textDat,2892+metricIndex*4);}
      else if(datControls&&TryDatScreenMetric(t.DatMetricFace,code,out metric)){}
      else if(!datControls&&t.DatMetricFace==-1)metric=0;
      else if(faces.TryGetValue(datControls?font.MetricFace:t.DatMetricFace,out face))metric=TextMetricArithmetic.Normalize(face.Widths[code],638);
      else throw new InvalidDataException("External SCORE metric face missing: "+font.MetricFace);
      t.Metric=metric;t.Advance=TextMetricArithmetic.Advance(metric,result.Scale,code==94);
     }
     if(font.MetricFace>=90||font.Word>=100||PrintedMusic(code))result.ExternalType1=false;
     if(font.MetricFace>=90 && !((font.MetricFace<=94||font.MetricFace==98)&&(code<70||code==94)||(font.MetricFace<=94||font.MetricFace==98)&&((code>=70&&code<=76)||code==88||code==132||code==140)))result.VectorSupported=false;
     //The external DAT supplies metrics independently of the vector glyph route.
     if(font.MetricFace>=95&&font.MetricFace!=98)result.MetricsSupported=false;
    }
    RecordOriginalMetricFace(t.DatMetricFace);t.FontWord=font.Word;result.Tokens.Add(t);total=TextMetricArithmetic.Accumulate(total,t.Advance);
   }
   result.Width=TextMetricArithmetic.Finish(total,result.Parameters.Spacing);result.FinalFont=font.Word;
   var advances=new float[result.Tokens.Count];for(int n=0;n<advances.Length;n++)advances[n]=result.Tokens[n].Advance;
   result.LetterExtra=TextParameterPreparation.Interletter(result.Width,result.Parameters.Spacing,advances);
   var adjusted=TextParameterPreparation.Adjust(advances,result.LetterExtra);float position=0;
   for(int n=0;n<adjusted.Length;n++){result.Tokens[n].Position=position;result.Tokens[n].AdjustedAdvance=adjusted[n];position=TextMetricArithmetic.Accumulate(position,adjusted[n]);}
   return result;
  }
  static float Printed3(float v){return Single.Parse(((double)v).ToString("F3",CultureInfo.InvariantCulture),CultureInfo.InvariantCulture);}
  static float Printed2(float v){return Single.Parse(((double)v).ToString("F2",CultureInfo.InvariantCulture),CultureInfo.InvariantCulture);}
  public static List<NativeTextToken> RenderingTokens(NativeTextLayout layout,short overrideWord,short previousFont){
   var result=new List<NativeTextToken>();var font=TextConverterFontState.Begin(overrideWord,previousFont);int normal=layout.Tokens.Count==0?0:layout.Tokens[0].Code;
   for(int n=0;n<layout.Tokens.Count;n++){
    var token=layout.Tokens[n];if(n>0&&token.Code>=1000&&token.Code<=1099){if(token.Code==normal)continue;normal=token.Code;}
    font.Accept(token.Code);result.Add(new NativeTextToken{Code=token.Code,FontWord=font.Word,DatMetricFace=token.DatMetricFace,Character=token.Character,GlyphName=token.GlyphName,Advance=token.Advance,AdjustedAdvance=token.AdjustedAdvance,Position=token.Position,TabX=token.TabX});
   }return result;
  }
  public static NativePrintedTextValues PrintedValues(NativeTextLayout layout,float staff){return new NativePrintedTextValues{
   EmX=Printed3(TupletGeometry.PrintedEm(layout.Parameters.Horizontal,staff)),EmY=Printed3(TupletGeometry.PrintedEm(layout.Parameters.Vertical,staff)),
   Letter=Printed2(TextParameterPreparation.OutputExtra(layout.LetterExtra,true,150)),Space=Printed2(TextParameterPreparation.OutputExtra(layout.Scale.ExtraSpace,true,150)),Rotation=Printed2(layout.Parameters.Rotation)};}
  public void Draw(Graphics g,Item item,string text,float x,float y,float step,float? staffScale=null,NativeNotationContext page=null){
   if(item.P(8)==99&&text!=null&&text.IndexOfAny(new[]{'\r','\n'})<0&&text.TrimStart(' ').StartsWith("%",StringComparison.Ordinal)){
    RenderResourceAudit.Record("raw PostScript comment","Code16 P8=99",RootDirectory,"measured single-line comment: packed DAT screen text, no printed marks");var output=NativeEpsExport.For(g);if(output!=null)output.Comment(text.TrimStart(' ').Substring(1));return;
   }
   if(GhostscriptTextRenderer.TryDraw(g,this,item,page))return;
   float staff=staffScale.HasValue?staffScale.Value:(float)((double)step*5);var layout=PrepareLayout(item,text,staff,0);
   short overrideWord=unchecked((short)(int)item.P(8));var rendering=PrintedRenderingTokens(layout,overrideWord,0);bool forced=TextConverterFontState.Begin(overrideWord,0).Forced;
   if(!layout.VectorSupported){
    RenderStatus=NativeFontInitialization.External?"This text control is outside the verified external FONTINIT.PSC route.":"This text control is outside the verified SCORE rendering routes.";
    throw new NotSupportedException(RenderStatus);
   }
   var fonts=new Dictionary<string,Type1Outline>();
   var printed=PrintedValues(layout,staff);float emX=printed.EmX,emY=printed.EmY,letter=printed.Letter,space=printed.Space;
   var state=g.Save();try{
    float rawX=page==null?0:page.Prepared.X,rawY=page==null?0:page.Prepared.Y;
    var origin=TextParameterPreparation.Prepare(TextParameters(item),rawX,100,.2800000011920929f,150);rawX=origin.Origin;
    float pivotX=NativeNotationContext.RawWord(rawX),pivotY=NativeNotationContext.RawWord(rawY);
    if(page!=null){var point=page.Point(pivotX,pivotY);g.TranslateTransform(point.X,point.Y);}else g.TranslateTransform(x+layout.Parameters.Origin/30,y);
    g.RotateTransform(printed.Rotation);
    float run=0;bool newRun=true;
    float currentX=rawX;
    var vectorTokens=new List<PostScriptTextPart>();
    foreach(var token in rendering){
     if(token.TabX.HasValue){currentX=token.TabX.Value;newRun=true;continue;}
     if(token.AdvanceOnly){currentX=TextMetricArithmetic.AddPosition(1,currentX,token.AdjustedAdvance,150);continue;}
     if(PrintedModifier(token.Code))continue;if(token.Code>=1000){if(!forced)newRun=true;continue;}
     if(PrintedMusic(token.Code)||token.FontWord%100>=90){
      vectorTokens.Add(new PostScriptTextPart{Vector=token,X=currentX});newRun=true;
     }else{
     if(newRun){run=(NativeNotationContext.RawWord(currentX)-pivotX)/30f;newRun=false;}
     var selectedFont=PrintedFontCarry.Select(MissingPrintedFace(token.FontWord%100)?null:ExternalFontName(token.FontWord%100),printed.EmX,printed.EmY,token.FontWord>=100);emX=selectedFont.EmX;emY=selectedFont.EmY;Type1Outline face;if(!fonts.TryGetValue(selectedFont.Name,out face)){face=NativeTupletFonts.Find(selectedFont.Name);if(face==null){RenderStatus=NativeTupletFonts.Status;throw new FileNotFoundException(RenderStatus);}fonts.Add(selectedFont.Name,face);}
     float width;using(var path=token.GlyphName!=null?face.NamedCharacter(token.GlyphName,selectedFont.Outline,out width):selectedFont.Outline?face.OutlineCharacter(token.Character,out width):face.Character(token.Character,out width)){
      using(var matrix=new Matrix(emX/30000,0,0,-emY/30000,run,(pivotY-NativeNotationContext.RawWord(rawY))/30f))path.Transform(matrix);
      if(path.PointCount>0){if(selectedFont.Outline)using(var pen=new Pen(Color.Black,0)){pen.LineJoin=LineJoin.Round;g.VectorDrawPath(pen,path);}else g.VectorFillPath(Brushes.Black,path);}
     }
     run+=(width*emX/1000+letter+(token.Character==' '?space:0))/30;
     }
     currentX=TextMetricArithmetic.AddPosition(1,currentX,token.AdjustedAdvance,150);
    }
    using(var vectors=new VectorTextFrame(g,page!=null,VectorTextStrokeWidth(overrideWord,layout.FinalFont),new PointF(pivotX,pivotY),-printed.Rotation)){
     foreach(var part in vectorTokens)DrawVectorToken(vectors,part.Vector,layout,staff,part.X,rawY,pivotX,pivotY);
    }
    RenderStatus="External vector fonts; verified SCORE text arithmetic";
   }finally{g.Restore(state);}
  }
 }
}
