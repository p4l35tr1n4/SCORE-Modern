using System;using System.Globalization;
namespace ScoreModern.RecoveredFragments {
 public static class SlurLineWidth {
  public static float DefaultUnit{get{return BitConverter.ToSingle(BitConverter.GetBytes(0x41557e4b),0);}}
  // Overlay three, relative 2587:0778..07A3; explicit printer-context inputs.
  public static float RawField(short selector,short basis,float unit){return (float)(((double)basis+selector)*unit);}
  public static float Field(short selector,short basis,float unit){float value=RawField(selector,basis,unit);if(BitConverter.ToInt32(BitConverter.GetBytes(value),0)<0)value=0;return value;}
  // REAL4 fixed output in the shipped .NET Framework target; original width
  // requests, including its seven-significant-digit boundary, have replay checks.
  public static string Format(float value){string text=value.ToString("F4",CultureInfo.InvariantCulture);return text.StartsWith("0.")?text.Substring(1):text;}
  public static float Printed(short selector){return Single.Parse(Format(Field(selector,2,DefaultUnit)),CultureInfo.InvariantCulture);}
 }
}
