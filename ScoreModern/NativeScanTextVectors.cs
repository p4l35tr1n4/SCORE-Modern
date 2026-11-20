using System;using System.IO;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class ScoreText {
  public bool ShowActualDatFonts;
  // DAT screen text. The original screen dispatcher uses packed strokes for
  // basic characters even for drawing-font faces 90..94. Printed text keeps
  // its separate Type-1/DRW path. Source text is separate from scan working words.
  public List<PrimitiveTextPoint> ScanGraveLibraryAlias(Item working,NativeNotationContext page,short previousFont=0){float ignored;return ScanGraveLibraryAlias(working,page,previousFont,out ignored);}
  internal List<PrimitiveTextPoint> ScanGraveLibraryAlias(Item working,NativeNotationContext page,short previousFont,out float finalX){
   // Original Code9 81: plain piu strokes, then BODN8 slot4 at the u origin,
   // retaining the u-specific horizontal metric correction (1CCA:0A9D).
   var layout=PrepareLayout(working,"piu",page.Scale,previousFont);var points=ScanStrokeText(working,"piu",page,previousFont);
   var tokens=RenderingTokens(layout,unchecked((short)(int)working.P(8)),previousFont);
   var origin=TextParameterPreparation.Prepare(TextParameters(working),page.Prepared.X,100,.2800000011920929f,150);float x=origin.Origin;
   for(int n=0;n<tokens.Count;n++){
    var token=tokens[n];if(n==tokens.Count-1){Face face;if(token.Character!='u'||!faces.TryGetValue(token.FontWord%100,out face))throw new NotSupportedException("DAT grave alias metric face unavailable.");
     float metric=TextMetricArithmetic.Normalize(face.Widths[token.Code],638),primitive=BitConverter.ToSingle(textDat,2892+(token.Code+1)*4);
     var glyphLayout=new NativeTextLayout{Parameters=layout.Parameters};glyphLayout.Parameters.GlyphHorizontal=TextMetricArithmetic.StrokeHorizontal(metric,layout.Parameters.GlyphHorizontal,primitive);
     points.AddRange(DrawingPoints(90,84,glyphLayout,page.Scale,x,page.Prepared.Y));
    }x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);
   }finalX=x;return points;
  }
  public List<PrimitiveTextPoint> ScanStrokeText(Item working,string sourceText,NativeNotationContext page,short previousFont){short finalFont;return ScanStrokeText(working,sourceText,page,previousFont,out finalFont);}
  public List<PrimitiveTextPoint> ScanStrokeText(Item working,string sourceText,NativeNotationContext page,short previousFont,out short finalFont){
   float finalX;
   return ScanStrokeText(working,sourceText,page,previousFont,out finalFont,out finalX);
  }
  internal List<PrimitiveTextPoint> ScanStrokeText(Item working,string sourceText,NativeNotationContext page,short previousFont,out short finalFont,out float finalX){

   if(working==null||page==null||working.Code!=16)throw new NotSupportedException("Requires an unrotated Code16 scan context.");
   var layout=PrepareLayout(working,sourceText,page.Scale,previousFont,true);finalFont=layout.FinalFont;
   foreach(var token in layout.Tokens)if(token.Code<0)throw new NotSupportedException("Text outside the verified scan token domain.");
   var tokens=RenderingTokens(layout,unchecked((short)(int)working.P(8)),previousFont);
   var origin=TextParameterPreparation.Prepare(TextParameters(working),page.Prepared.X,100,.2800000011920929f,150);
   float x=origin.Origin;var points=new List<PrimitiveTextPoint>();int accent=0;
   if(textDat==null)textDat=File.ReadAllBytes(NativeLibraryLocation.FileFor(RootDirectory,"SCOR3_10.DAT"));
   foreach(var token in tokens){
    if(token.TabX.HasValue){x=token.TabX.Value;continue;}
    if(token.Code>=1000)continue;
    if(token.Code>=83&&token.Code<=87){accent=token.Code;continue;}
    if(token.FontWord%100==99&&PrintedMusic(token.Code)){points.AddRange(DrawingPoints(90,token.Code>=162?token.Code-68:token.Code,layout,page.Scale,x,page.Prepared.Y));x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);continue;}
    if(token.Code>=89&&token.Code!=94&&token.Code<=167){
     int[] codes={95,96,97,98,99,100,103,109,110,111,113,114,115,129,130,131,136,143,144,151,152,153,149,132,133,134,135,145,147,138,139,140,146};int[] glyphs={40,10,53,23,54,24,78,76,77,10,28,35,40,58,34,65,75,21,24,48,51,54,0,1,2,3,4,5,6,7,8,9,97};int at=Array.IndexOf(codes,(int)token.Code);var gl=new NativeTextLayout{Parameters=layout.Parameters};
     if(at>=0){int target=glyphs[at]+(at>=22&&at!=32?95:0),face=token.FontWord%100;float metric;if(face>=90){int mi=token.Code+1;if(mi==150)mi=94;if(mi==146||mi==148)mi=101+(mi-146)/2;if(mi>=133&&mi<=141)mi-=36;metric=BitConverter.ToSingle(textDat,2892+mi*4);}else if(TryDatScreenMetric(token.DatMetricFace,token.Code,out metric)){}else throw new NotSupportedException("DAT auxiliary text metric font needs verification.");float primitive=BitConverter.ToSingle(textDat,2892+(target+1)*4);gl.Parameters.GlyphHorizontal=TextMetricArithmetic.StrokeHorizontal(metric,gl.Parameters.GlyphHorizontal,primitive);if(at>=22&&at!=32)points.AddRange(DatAuxiliaryPoints(glyphs[at],gl,page.Scale,x,page.Prepared.Y));else if(target<70)points.AddRange(StrokePoints(target,gl,page.Scale,x,page.Prepared.Y));else points.AddRange(DrawingPoints(face>=90&&face<=94?face:90,target,gl,page.Scale,x,page.Prepared.Y));}
     else if(token.Code>=162)points.AddRange(DrawingPoints(90,token.Code-68,gl,page.Scale,x,page.Prepared.Y));else if(token.Code==89)points.AddRange(DrawingPoints(90,89,gl,page.Scale,x,page.Prepared.Y));
     else {int face=token.FontWord%100,mi=token.Code+1;float m;if(face>=90){if(mi==150)mi=94;if(mi==146||mi==148)mi=101+(mi-146)/2;if(mi>=133&&mi<=141)mi-=36;m=BitConverter.ToSingle(textDat,2892+mi*4);}else if(TryDatScreenMetric(token.DatMetricFace,token.Code,out m)){}else throw new NotSupportedException("DAT default control metric face needs verification.");float pm=BitConverter.ToSingle(textDat,2892+34*4);gl.Parameters.GlyphHorizontal=TextMetricArithmetic.StrokeHorizontal(m,gl.Parameters.GlyphHorizontal,pm);points.AddRange(StrokePoints(33,gl,page.Scale,x,page.Prepared.Y));}
     x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);continue;
    }
    if(token.Code!=94){
     int face=token.FontWord%100;if(face==99)face=98;
     // Main-screen whole-buffer tests cover all eight special glyphs in
     // faces90..94 and98: they share BODN resources, independently of text face.
     bool commonDrawing=(token.Code>=70&&token.Code<=76||token.Code==88)&&(face>=90&&face<=94||face==98);
     if(commonDrawing||token.Code==88||(token.Code>=70&&token.Code<88&&face<95)){
      points.AddRange(DrawingPoints(commonDrawing?90:face>=90&&face<=94?face:90,token.Code,layout,page.Scale,x,page.Prepared.Y));x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);continue;
     }
     if(ShowActualDatFonts&&face>=90&&face<=94){points.AddRange(DrawingPoints(face,token.Code,layout,page.Scale,x,page.Prepared.Y));if(accent!=0){points.AddRange(DrawingPoints(90,accent,layout,page.Scale,x,page.Prepared.Y));accent=0;}x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);continue;}if(face==98||(face>=90&&face<=94)){points.AddRange(StrokePoints(token.Code,layout,page.Scale,x,page.Prepared.Y));if(accent!=0){points.AddRange(DrawingPoints(90,accent,layout,page.Scale,x,page.Prepared.Y));accent=0;}x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);continue;}
     float metric;if(token.Code<0||token.Code>=70||face<0||face>=90||!TryDatScreenMetric(token.DatMetricFace,token.Code,out metric))throw new NotSupportedException("Scan stroke glyph requires a verified SCORE font and character route.");
     float primitive=BitConverter.ToSingle(textDat,2892+(token.Code+1)*4);
     var glyphLayout=new NativeTextLayout{Parameters=layout.Parameters};
     glyphLayout.Parameters.GlyphHorizontal=TextMetricArithmetic.StrokeHorizontal(metric,layout.Parameters.GlyphHorizontal,primitive);
     points.AddRange(StrokePoints(token.Code,glyphLayout,page.Scale,x,page.Prepared.Y));
     if(accent!=0){points.AddRange(DrawingPoints(90,accent,glyphLayout,page.Scale,x,page.Prepared.Y));accent=0;}
    }
    x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);
   }
   finalX=x;
   if(working.P(9)!=0){for(int n=0;n<points.Count;n++){var q=points[n];q.X=NativeNotationContext.Word(q.X,true);q.Y=NativeNotationContext.Word(q.Y,false);points[n]=q;}NativeDatLines.Rotate(points,working.P(9),origin.Origin,page.Prepared.Y);}
   return points;
  }
 }
}


