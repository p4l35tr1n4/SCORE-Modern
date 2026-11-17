using System;using System.IO;
namespace ScoreModern {
 public static class NativeMeasurements {
  public static short Get(Score score){if(score==null)return 0;int offset=score.Trailer.Length-12;if(offset<0)throw new InvalidDataException("Missing MUS measurement trailer.");float value=BitConverter.ToSingle(score.Trailer,offset);if(value!=0&&value!=1)throw new NotSupportedException("MUS measurement code outside verified inches/metric values.");return (short)value;}
  public static void Set(Score score,short value){if(value!=0&&value!=1)throw new ArgumentOutOfRangeException("value");int offset=score.Trailer.Length-12;if(offset<0)throw new InvalidDataException("Missing MUS measurement trailer.");Array.Copy(BitConverter.GetBytes((float)value),0,score.Trailer,offset,4);}
  public static Score New(string root){var score=Score.New();Set(score,NativePreferences.Read(root).Word("MEASUREMENT",0));return score;}
  public static string Label(Score score){try{return Get(score)==1?"METRIC (centimeters)":"INCHES";}catch(NotSupportedException){return "UNVERIFIED CODE "+BitConverter.ToSingle(score.Trailer,score.Trailer.Length-12).ToString(System.Globalization.CultureInfo.InvariantCulture);}}
 }
}
