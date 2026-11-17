using System;
using System.IO;
using System.Text;
namespace ScoreModern {
 internal sealed class NativePageFilenameStep {internal string Stem;internal bool Exhausted;}
 internal static class NativePageFilenameSequence {
  // PAGE 2E8F:0771..0818. Signed-byte comparisons are intentional.
  // Digits wrap to0 and letters pastZ toA while carrying to the left.
  internal static NativePageFilenameStep StepRaw(string stem){
   if(String.IsNullOrEmpty(stem))throw new ArgumentException("PAGE filename stem is empty.");var encoding=Encoding.GetEncoding(437,EncoderFallback.ExceptionFallback,DecoderFallback.ExceptionFallback);byte[] bytes=encoding.GetBytes(stem);
   for(int n=bytes.Length-1;n>=0;n--){byte next=unchecked((byte)(bytes[n]+1));if((sbyte)next>=65){if((sbyte)next<=90){bytes[n]=next;return new NativePageFilenameStep{Stem=encoding.GetString(bytes)};}bytes[n]=65;}else{if((sbyte)next<=57){bytes[n]=next;return new NativePageFilenameStep{Stem=encoding.GetString(bytes)};}bytes[n]=48;}}
   return new NativePageFilenameStep{Stem=encoding.GetString(bytes),Exhausted=true};
  }
  internal static string[] OriginalNames(string first,int count){
   if(count<1)throw new ArgumentException("PAGE needs at least one output page.");string path=Path.GetFullPath(first),folder=Path.GetDirectoryName(path),stem=Path.GetFileNameWithoutExtension(path),extension=Path.GetExtension(path);if(stem.Length==0)throw new ArgumentException("Choose a PAGE output filename.");
   if(stem.AnyNonAscii())throw new NotSupportedException("Original PAGE filename sequencing needs an ASCII stem; choose numeric Windows sequencing for other names.");stem=stem.ToUpperInvariant();var result=new string[count];result[0]=path;
   for(int n=1;n<count;n++){var step=StepRaw(stem);if(step.Exhausted)throw new InvalidOperationException("The original PAGE filename sequence is exhausted. Choose another first filename.");stem=step.Stem;result[n]=Path.Combine(folder,stem+extension);}return result;
  }
  static bool AnyNonAscii(this string value){foreach(char c in value)if(c>127)return true;return false;}
 }
}
