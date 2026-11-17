using System;using System.IO;using System.Text;using System.Globalization;
namespace ScoreModern {public sealed partial class Editor {
 readonly float[] originalPostScriptBoxState=new float[4];
 float[] OriginalPostScriptBox(string program){
  int eof=program.IndexOf((char)26);if(eof>=0)program=program.Substring(0,eof);
  foreach(string line in program.Replace("\r\n","\n").Replace('\r','\n').Split('\n')){
   if(!line.StartsWith("%%BoundingBox:",StringComparison.Ordinal))continue;
   string input=line.TrimEnd(' ');var numbers=new System.Collections.Generic.List<float>();int cursor=14;
   while(cursor<input.Length-1&&numbers.Count<9){char c=input[cursor];if(c>='0'&&c<='9'||c=='+'||c=='-'||c=='.'){
    int consumed=0;float value=ScoreModern.RecoveredFragments.InputRealReader.Read(input.Substring(cursor),ref consumed);
    if(consumed==0||Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("EPS bounding-box numeric control leaves the verified SCORE reader domain.");
    numbers.Add(value);cursor+=consumed;
   }else cursor++;}
   if(numbers.Count==0)continue;
   if(numbers.Count<4&&!NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Original SCORE parser read fewer than four bounding-box values. Original invalid-state compatibility can retain preceding header values.");
   var box=(float[])originalPostScriptBoxState.Clone();for(int n=0;n<Math.Min(4,numbers.Count);n++)box[n]=numbers[n];Array.Copy(box,originalPostScriptBoxState,4);return box;
  }
  return null;
 }
 bool SubmitOriginalPostScriptName(string input){var item=externalDrawingNameItem;if(item==null||item.Code!=15)return false;if(input=="-"&&!NativeEditCompatibility.ReproduceInvalidStates){if(externalDrawingLoad==null)item.Set(13,0);FinishOriginalPostScriptName(item,"");return true;}if(input.Length==0||input.Length>24)throw new InvalidDataException("Enter a PostScript filename of 1 to 24 bytes.");char[] upper=input.ToCharArray();for(int n=0;n<upper.Length;n++)if(upper[n]>='a'&&upper[n]<='z')upper[n]=(char)(upper[n]-'a'+'A');var encoding=Encoding.GetEncoding(437,EncoderFallback.ExceptionFallback,DecoderFallback.ExceptionFallback);string name=new String(upper);byte[] bytes=encoding.GetBytes(name.PadRight(24,' '));if(bytes.Length!=24)throw new InvalidDataException("PostScript filenames use 24 SCORE bytes.");var preview=item.Clone();if(preview.Count<19)preview.Set(19,0);preview.Set(13,0);Array.Copy(bytes,0,preview.Bytes,56,24);preview.ExternalDrawingNameOverride=null;
  string path;
  try{path=IncludedPostScriptDisplay.Resolve(canvas.Library.Root,current,preview,scor4Interface);}
  catch(FileNotFoundException){
   if(!NativeEditCompatibility.ReproduceInvalidStates)throw;
   string message="*** PostScript program file not found.  "+name;
   if(externalDrawingLoad==null){
    // DOS retains the filename and P13 after the failed lookup.
    FinishOriginalPostScriptName(item,message+Environment.NewLine+"Press any key to continue.");
    pendingEditAcknowledgement=true;
   }else FinishOriginalPostScriptName(item,message);
   return true;
  }
  float[] box=OriginalPostScriptBox(Encoding.GetEncoding(28591).GetString(File.ReadAllBytes(path)));if(box!=null){for(int n=0;n<4;n++)preview.Set(7+n,box[n]);preview.Set(11,-9999);}else if(item.P(11)==-9999){for(int n=7;n<=10;n++)preview.Set(n,0);}if(externalDrawingLoad!=null){item.ExternalDrawingNameOverride=name.PadRight(24,' ');item.ExternalPostScriptBoxOverride=new[]{preview.P(7),preview.P(8),preview.P(9),preview.P(10),preview.P(11)};}else{item.Bytes=preview.Bytes;item.ExternalDrawingNameOverride=null;item.ExternalPostScriptBoxOverride=null;}FinishOriginalPostScriptName(item,"");return true;
 }
 void FinishOriginalPostScriptName(Item item,string message){CompleteExternalDrawingName(item,message);}
}}
