using System;using System.IO;using System.Collections.Generic;
namespace ScoreModern {
 public sealed partial class ScoreText {
  byte[] verificationDatBackup;
  internal void SetDatMetricBitsForVerification(int code,uint bits){if(code<0||code>104)throw new ArgumentOutOfRangeException("code");float value=BitConverter.ToSingle(BitConverter.GetBytes(bits),0);if(Single.IsNaN(value)||Single.IsInfinity(value))throw new InvalidDataException("Diagnostic DAT metric must be finite.");if(textDat==null)textDat=File.ReadAllBytes(NativeLibraryLocation.FileFor(RootDirectory,"SCOR3_10.DAT"));if(verificationDatBackup==null){verificationDatBackup=textDat;textDat=(byte[])textDat.Clone();}Buffer.BlockCopy(BitConverter.GetBytes(bits),0,textDat,2896+code*4,4);}
  internal void ResetDatMetricVerification(){if(verificationDatBackup!=null){textDat=verificationDatBackup;verificationDatBackup=null;}}
  int DatMetricFace(int face){if(face>=90)return face;RequireMetricFace(face);return face;}
  bool TryDatScreenMetric(int face,int code,out float metric){
   metric=0;Face data;if(faces.TryGetValue(face,out data)){if(code<0||code>=data.Widths.Length)return false;metric=ScoreModern.RecoveredFragments.TextMetricArithmetic.Normalize(data.Widths[code],638);return true;}
   // Isolated original screen-bank measurements are required for every
   // unsupplied face admitted here; this never supplies a Type1 font.
   return face==-1&&((code>=0&&code<=76)||code==94);
  }
 }
}
