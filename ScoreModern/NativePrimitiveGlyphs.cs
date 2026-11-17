using System;using System.Collections.Generic;using System.Drawing;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public struct PrimitiveGlyphRequest {public int Glyph,WidthSelector;public float X,Y,Horizontal,Vertical;}
 public static class NativePrimitiveGlyphs {
  static float F(double v){return (float)v;}
  // K306: cached requests divide the signed width numerator by scale before
  // clamping. Negative selector and scale can therefore yield a positive pen.
  public static bool Draw(Graphics g,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,PrimitiveGlyphRequest r,IEnumerable<BarlinePoint> trailingStroke=null){if(NativeDatDrawing.Primitive(g,page,library,r))return true;if(library==null||page==null||r.Glyph<0||r.Glyph>44)return false;Glyph glyph;if(!library.Symbols.TryGetValue(r.Glyph,out glyph))return false;RenderResourceAudit.Record("printed note primitive",r.Glyph+":"+glyph.Name,glyph.Source,"original shared primitive request; SCORE-word quantization, fill plus stroke");
   if(r.Horizontal==r.Vertical&&cache!=null&&cache.Contains(r.Glyph)){float scale=F((double)r.Horizontal*page.Scale/25);if(scale==0)return true;float width=F(NativePrintContext.UnclampedWidth((short)r.WidthSelector)/(double)scale);if(width<0)width=0;bool inheritedDefault=width==NativePrintContext.RawWidth(0);width=Single.Parse(PrintedGlyphDefinition.Fixed3(width),CultureInfo.InvariantCulture);scale=Single.Parse(PrintedGlyphDefinition.Fixed3(scale),CultureInfo.InvariantCulture);return NativeLibrarySymbols.DrawCached(g,page,glyph,new LibrarySymbolPlan{Glyph=r.Glyph},scale,width,6.25f,NativeNotationContext.RawWord(r.X),NativeNotationContext.RawWord(r.Y),inheritedDefault);}
   float horizontal=F(F(r.Horizontal/4.0)*(double)page.Scale),vertical=F(F(r.Vertical/4.0)*(double)page.Scale);var points=new List<BarlinePoint>();for(int n=0;n<glyph.Points.Length;n++)points.Add(new BarlinePoint(F(glyph.Points[n].X*(double)horizontal+r.X),F(glyph.Points[n].Y*(double)vertical+r.Y),glyph.Operations[n]));if(trailingStroke!=null)points.AddRange(trailingStroke);NativeNoteDetails.Path(g,page,points,NativePrintContext.Width((short)r.WidthSelector),inheritedWidth:r.WidthSelector==0);if(r.WidthSelector!=0)NativePrintContext.RestoreDefaultLineWidth();return true;
  }
 }
}
