using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputMarkTokens {public float[] Values;public int[] Opposite;}
 public static class InputMarkText {
  // Canonical mark spellings from overlay8 section69. The numeric consumer
  // receives descriptors, target words and range controls independently.
  static readonly Dictionary<string,int> names=new Dictionary<string,int>{
   {"A",5},{"AR",15},{"AS",9},{"AO",67},{"AC",68},{"ACT",25},{"C",97},{"CR",63},{"C-",98},
   {"D",11},{"DI",64},{"F",56},{"FP",60},{"FE",14},{"FF",57},{"FFF",58},
   {"FIF",1},{"FIS",2},{"FIN",3},{"MO",17},{"IM",18},{"HW",19},
   {"H",13},{"M",17},{"MF",55},{"MP",54},{"O",96},{"OB",95},
   {"P",53},{"PP",52},{"PPP",51},{"PL",24},{"PZ",66},{"RI",65},
   {"S",7},{"S2",26},{"S3",27},{"S4",28},{"SF",61},{"SFZ",62},
   {"T",6},{"TS",10},{"TH",16},{"TR",20},{"TRF",21},{"TRS",22},{"TRN",23},
   {"TM0",93},{"TM4",94},{"TM5",93},{"TM6",93},{"TM7",93},{"TM8",93},{"TM9",93},{"TL",93},{"TME",91},{"TMS",92},{"TMT",93},{"TM1",91},{"TM2",92},{"TM3",93},{"TRW",120},{"TRFW",121},{"TRSW",122},{"TRNW",123},{"U",12},{"W",4},{"WS",8}};
  public static InputMarkTokens Parse(string text,InputMacroContext context){return Parse(text,context,false);}
  public static InputMarkTokens Parse(string text,InputMacroContext context,bool rejectMultiDigitF){if(context==null)throw new ArgumentNullException("context");var values=new List<float>();var flags=new List<int>();foreach(string field in InputTextLists.ExpansionFields(text)){string part=field.Trim();if(part.Length==0)continue;Match definition=Regex.Match(part,@"^([A-Z])\[(.*)\]$");if(definition.Success){char name=definition.Groups[1].Value[0];var body=Parse(definition.Groups[2].Value+";",context,rejectMultiDigitF);// Original motive memory retains the float words, not the initial opposed-side flags.
context.Define(name,body.Values);values.AddRange(body.Values);flags.AddRange(body.Opposite);continue;}Match recall=Regex.Match(part,@"^@(-?)([A-Z])([+\-0-9. ]*)$");if(recall.Success){
 string suffix=recall.Groups[3].Value.Trim();
 if(suffix.Length>0){
  int at=0;
  InputRealReader.Read(suffix,ref at);
  if(at!=suffix.Length)throw new InvalidDataException("Invalid mark motive suffix: "+suffix);
 }
 char name=recall.Groups[2].Value[0];
 float[] body;
 if(!context.Definitions.TryGetValue(name,out body))throw new InvalidDataException("Undefined input motive "+name);
 if(recall.Groups[1].Length==0)values.AddRange(body);
 else {
  // Overlay 8 tests the signed word obtained by dividing the current word
  // by 1000. Its grouped route includes earlier negative words up to the
  // preceding nonnegative word. Other words are emitted singly.
  for(int index=body.Length-1;index>=0;index--){
   int kind=unchecked((short)(int)Math.Truncate((double)body[index]/1000));
   if(kind==0||kind==-1||kind==-7){
    int first=index-1;
    while(first>=0&&body[first]<0)first--;
    if(first<0)throw new InvalidDataException("Reverse mark motive starts before its definition.");
    for(int i=first;i<=index;i++)values.Add(body[i]);
    index=first;
   }else values.Add(body[index]);
  }
 }
 flags.AddRange(new int[body.Length]);
 continue;}var parsed=Parse(part+";",rejectMultiDigitF);values.AddRange(parsed.Values);flags.AddRange(parsed.Opposite);}return new InputMarkTokens{Values=values.ToArray(),Opposite=flags.ToArray()};}
  public static InputMarkTokens Parse(string text){return Parse(text,false);}
  public static InputMarkTokens Parse(string text,bool rejectMultiDigitF){
   if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(!text.EndsWith(";"))throw new InvalidDataException("Terminate marks with a semicolon");text=text.Substring(0,text.Length-1);var values=new List<float>();var flags=new List<int>();if(text.Trim().Length==0)return new InputMarkTokens{Values=values.ToArray(),Opposite=flags.ToArray()};
   foreach(string field in text.Split('/')){
    Match m=Regex.Match(field.Trim(),@"^([+-]?)([A-Z][A-Z0-9-]*)(?:\s+(.+))?$");if(!m.Success)throw new InvalidDataException("Separate the mark and its target with a space");int kind;string name=m.Groups[2].Value;
    Match literal=Regex.Match(name,@"^N([0-9]+)$"),number=Regex.Match(name,@"^F([0-9]+)$");
    string targets=m.Groups[3].Value;
    if(name=="S1"&&targets.Length==0)
     throw new InvalidDataException("Separate the mark and its target with a space");
    if(name=="N"){Match n=Regex.Match(targets,@"^([0-9]+)\s+(.+)$");if(!n.Success)throw new InvalidDataException("N requires a library glyph number and target");kind=-Integer(n.Groups[1].Value);targets=n.Groups[2].Value;}
    else if(literal.Success)kind=-Integer(literal.Groups[1].Value);
    else if(number.Success){
     string digits=number.Groups[1].Value;
     if(rejectMultiDigitF&&digits.Length>1)throw new InvalidDataException("Separate a single F digit and its target with a space.");
     // The original F branch consumes one digit. Remaining digits are
     // read by its target reader before the explicitly separated targets.
     kind=digits[0]-'0'+30;
     if(digits.Length>1)targets=digits.Substring(1)+" "+targets;
    }
    else if(!names.TryGetValue(name,out kind))throw new NotSupportedException("Mark spelling requires its original parser branch: "+name);
    if(targets.Trim().Length==0)throw new InvalidDataException("Separate the mark and its target with a space");int opposite=m.Groups[1].Length==0?0:(kind<0?-1:1);values.Add((float)((long)kind*10000));flags.Add(opposite);
    foreach(Match target in Regex.Matches(targets,@"\S+")){
     string[] range=target.Value.Split(new[]{':','\''});if(range.Length>2)throw new InvalidDataException("Invalid mark range");float value=Number(range[0]);if(range.Length==2)value=(float)((double)value+1000);values.Add(value);flags.Add(0);if(range.Length==2){values.Add(Number(range[1]));flags.Add(0);}
    }
   }
   return new InputMarkTokens{Values=values.ToArray(),Opposite=flags.ToArray()};
  }
  static int Integer(string text){int n;if(!int.TryParse(text,NumberStyles.None,CultureInfo.InvariantCulture,out n)||n>short.MaxValue)throw new InvalidDataException("Mark number exceeds the signed word representation");return n;}
  static float Number(string text){float n;if(!float.TryParse(text,NumberStyles.AllowLeadingSign|NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out n)||float.IsNaN(n)||float.IsInfinity(n))throw new InvalidDataException("Invalid mark target: "+text);return n;}
 }
}
