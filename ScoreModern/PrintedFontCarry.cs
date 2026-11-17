using System;using System.IO;using System.Collections.Generic;
namespace ScoreModern {
 internal struct PrintedFontSelection {public string Name;public float EmX,EmY;public bool Outline;}
 internal static class PrintedFontCarry {
  [ThreadStatic]static PrintedFontSelection current;
  internal static PrintedFontSelection Checkpoint {get{return current;}set{current=value;}}
  internal static void Begin(){current=new PrintedFontSelection();}
  internal static PrintedFontSelection Select(string name,float x,float y,bool outline=false){if(name!=null)current=new PrintedFontSelection{Name=name,EmX=x,EmY=y,Outline=outline};if(current.Name==null)throw new NotSupportedException("Missing SCORE font before a verified printed font selection.");return current;}
 }
 public sealed partial class ScoreText {
  void RequireMetricFace(int face){if(face<0||face>=90||!faces.ContainsKey(face))throw new InvalidDataException("SCORE font "+face+" has no readable FM"+face.ToString("00")+".PSC in the active LIB. Supply its original metrics; no replacement font or metrics have been selected.");}
  bool MissingPrintedFace(int face){RequireMetricFace(face);return false;}
  int PrintedMetricFace(int face){if(face>=90)return face;RequireMetricFace(face);return face;}
 }
}
