using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScoreModern {
 internal enum ScorlasInputSequence {PageNumberAndPartLetter,ConsecutiveNumbers,ConsecutiveCharacters}
 internal static class NativeScorlasBatch {
  static readonly Regex PagePart=new Regex(@"^(.*?)([0-9]+)([^0-9]*?)([A-Za-z])$");
  static readonly Regex NumberSuffix=new Regex(@"^(.*?)([0-9]+)$");
  internal static ScorlasInputSequence Detect(string file){string stem=Path.GetFileNameWithoutExtension(file);return PagePart.IsMatch(stem)?ScorlasInputSequence.PageNumberAndPartLetter:NumberSuffix.IsMatch(stem)?ScorlasInputSequence.ConsecutiveNumbers:ScorlasInputSequence.ConsecutiveCharacters;}
  static string AddNumber(string value,int amount){char[] digits=value.ToCharArray();int carry=amount;for(int n=digits.Length-1;n>=0;n--){int sum=digits[n]-'0'+carry;digits[n]=(char)('0'+sum%10);carry=sum/10;}return (carry==0?"":carry.ToString(System.Globalization.CultureInfo.InvariantCulture))+new string(digits);}
  static char AddCharacter(char start,int amount){int end=start>='A'&&start<='Z'?'Z':start>='a'&&start<='z'?'z':start>='0'&&start<='9'?'9':-1;if(end<0||amount>end-start)throw new NotSupportedException("This character sequence would pass its final letter or digit. Choose numeric filenames or split the batch; character rollover has not been assumed.");return (char)(start+amount);}
  internal static List<List<string>> Define(string start,int perPage,int count,bool countFiles,ScorlasInputSequence sequence){
   if(String.IsNullOrWhiteSpace(start))throw new ArgumentException("Choose a starting page/file.");
   if(perPage<=0||count<=0)throw new ArgumentException("Files per page and count must be positive.");
   int total=checked(countFiles?count:perPage*count);if(total>100000)throw new NotSupportedException("Preview at most 100,000 files in one batch; split larger definitions into smaller batches.");
   string full=Path.GetFullPath(start),folder=Path.GetDirectoryName(full),stem=Path.GetFileNameWithoutExtension(full),extension=Path.GetExtension(full);var part=PagePart.Match(stem);var number=NumberSuffix.Match(stem);
   if(sequence==ScorlasInputSequence.PageNumberAndPartLetter&&!part.Success)throw new ArgumentException("Page/part naming needs a page number followed by a part letter, such as MAT001A.MUS.");
   if(sequence==ScorlasInputSequence.ConsecutiveNumbers&&!number.Success)throw new ArgumentException("Numeric naming needs a number at the end of the filename, such as wandh01.mus.");
   if(stem.Length==0)throw new ArgumentException("The starting filename has no stem.");
   var pages=new List<List<string>>();for(int n=0;n<total;n++){
    int page=n/perPage,file=n%perPage;string next;
    if(sequence==ScorlasInputSequence.PageNumberAndPartLetter)next=part.Groups[1].Value+AddNumber(part.Groups[2].Value,page)+part.Groups[3].Value+AddCharacter(part.Groups[4].Value[0],file);
    else if(sequence==ScorlasInputSequence.ConsecutiveNumbers)next=number.Groups[1].Value+AddNumber(number.Groups[2].Value,n);
    else next=stem.Substring(0,stem.Length-1)+AddCharacter(stem[stem.Length-1],n);
    if(file==0)pages.Add(new List<string>());pages[page].Add(Path.Combine(folder,next+extension));
   }return pages;
  }
  internal static string[] OutputNames(string first,int count,bool numeric){
   string full=Path.GetFullPath(first),folder=Path.GetDirectoryName(full),stem=Path.GetFileNameWithoutExtension(full),extension=Path.GetExtension(full);if(stem.Length==0||count<=0)throw new ArgumentException("Choose a valid first output filename.");
   var number=NumberSuffix.Match(stem);if(numeric&&!number.Success)throw new ArgumentException("Numeric output names need a suffix such as TILED01.PS.");
   var names=new string[count];for(int n=0;n<count;n++){string next=n==0?stem:numeric?number.Groups[1].Value+AddNumber(number.Groups[2].Value,n):stem.Substring(0,stem.Length-1)+AddCharacter(stem[stem.Length-1],n);names[n]=Path.Combine(folder,next+extension);}return names;
  }
  internal static string Export(string root,IList<List<string>> pages,string firstOutput,NativePrintJob settings,bool numericNames,bool resolveInitialOffsets){
   return ExportControlled(root,pages,firstOutput,settings,numericNames,resolveInitialOffsets,null,null);
  }
  internal static string ExportControlled(string root,IList<List<string>> pages,string firstOutput,NativePrintJob settings,bool numericNames,bool resolveInitialOffsets,Func<bool> cancelled,Action<int,int> progress){
   if(pages==null||pages.Count==0||pages.Any(page=>page==null||page.Count==0))throw new ArgumentException("Every batch page needs at least one file.");
   string[] outputs=OutputNames(firstOutput,pages.Count,numericNames);var inputPaths=new HashSet<string>(pages.SelectMany(page=>page).Select(Path.GetFullPath),StringComparer.OrdinalIgnoreCase);
   foreach(string file in inputPaths)if(!File.Exists(file))throw new FileNotFoundException("Batch input is missing.",file);
   foreach(string file in outputs)if(inputPaths.Contains(file))throw new InvalidDataException("An output filename would overwrite a batch input: "+file);
   var job=(settings??NativePrintJob.Defaults(root)).CopyConfiguration();
   if(resolveInitialOffsets)using(var last=new Canvas{Score=Score.Load(pages[0][pages[0].Count-1]),Library=new DrawingLibrary(root),TextLayout=new ScoreText(root)})job.Resolve(last);
   string token=Guid.NewGuid().ToString("N"),folder=Path.GetDirectoryName(outputs[0]);var staged=new List<string>();var committed=new List<string>();var backups=new Dictionary<string,string>();bool success=false;var messages=new List<string>();
   try{
    for(int n=0;n<pages.Count;n++){if(progress!=null)progress(n,pages.Count);if(cancelled!=null&&cancelled())throw new OperationCanceledException("SCORLAS batch cancelled before publication; existing outputs were preserved.");string temporary=Path.Combine(folder,".scorlasbatch-"+token+"-"+n+".tmp");staged.Add(temporary);NativeScorlasWorkflow.ExportPrepared(root,pages[n],temporary,job,false);}
    if(progress!=null)progress(pages.Count,pages.Count);if(cancelled!=null&&cancelled())throw new OperationCanceledException("SCORLAS batch cancelled before publication; existing outputs were preserved.");
    try{
     for(int n=0;n<outputs.Length;n++){
      if(File.Exists(outputs[n])){string backup=Path.Combine(folder,".scorlasbatch-"+token+"-"+n+".backup");File.Replace(staged[n],outputs[n],backup);backups.Add(outputs[n],backup);}else File.Move(staged[n],outputs[n]);committed.Add(outputs[n]);messages.Add("Saved page "+(n+1)+": "+outputs[n]);
     }
    }catch(Exception publicationError){
     var failures=new List<string>();for(int n=committed.Count-1;n>=0;n--)try{string backup;if(backups.TryGetValue(committed[n],out backup))File.Replace(backup,committed[n],null);else File.Delete(committed[n]);}catch(IOException error){failures.Add(committed[n]+": "+error.Message);}catch(UnauthorizedAccessException error){failures.Add(committed[n]+": "+error.Message);}
     if(failures.Count>0)throw new IOException("Batch publication failed; some backups remain for recovery."+Environment.NewLine+String.Join(Environment.NewLine,failures.ToArray()),publicationError);throw;
    }
    success=true;return String.Join(Environment.NewLine,messages.ToArray());
   }finally{foreach(string temporary in staged)if(File.Exists(temporary))File.Delete(temporary);if(success)foreach(string backup in backups.Values)if(File.Exists(backup))File.Delete(backup);}
  }
 }
}
