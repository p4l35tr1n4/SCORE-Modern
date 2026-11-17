using System;using System.Globalization;
namespace ScoreModern.RecoveredFragments {
 public struct PrintedGlyphVertex {public int X,Y;public short Operation;}
 public struct PrintedHeadRequest {public int Glyph;public short X,Y;public string Scale,Width;}
 public static class PrintedGlyphDefinition {
  // Observed normal-scale PostScript definition projection. Coordinate and
  // operation arrays remain in the user's external drawing files.
  public static PrintedGlyphVertex Vertex(short x,short y,short operation){return new PrintedGlyphVertex{X=(int)Math.Truncate(x*6.25),Y=(int)Math.Truncate(y*6.25),Operation=operation};}
  public static string Fixed3(float value){string s=value.ToString("F3",CultureInfo.InvariantCulture);return s.StartsWith("0.")?s.Substring(1):s;}
 }
}
