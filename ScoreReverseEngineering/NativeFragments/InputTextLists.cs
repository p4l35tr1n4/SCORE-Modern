using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputMotiveReplacementException:Exception{public readonly char Name;public InputMotiveReplacementException(char name):base("Replacing Motive "+name){Name=name;}}
 public sealed class InputMacroContext {
 internal readonly Dictionary<char,float[]> Definitions=new Dictionary<char,float[]>();internal readonly List<char> Order=new List<char>();readonly Dictionary<char,int> replacementAllowances=new Dictionary<char,int>();public int StoredDefinitionCount{get;private set;}
 public IEnumerable<KeyValuePair<char,float[]>> Entries{get{foreach(char name in Order)yield return new KeyValuePair<char,float[]>(name,(float[])Definitions[name].Clone());}}
 public void AllowReplacement(char name){int count;replacementAllowances.TryGetValue(name,out count);replacementAllowances[name]=count+1;}public void ClearReplacementAllowances(){replacementAllowances.Clear();}
 internal void Define(char name,float[] body){if(Definitions.ContainsKey(name)){int count;if(!replacementAllowances.TryGetValue(name,out count)||count==0)throw new InputMotiveReplacementException(name);replacementAllowances[name]=count-1;}else Order.Add(name);Definitions[name]=(float[])body.Clone();StoredDefinitionCount++;}
 public void Clear(){Definitions.Clear();Order.Clear();replacementAllowances.Clear();StoredDefinitionCount=0;}
 public InputMacroContext Clone(){var result=new InputMacroContext();foreach(char name in Order){result.Order.Add(name);result.Definitions.Add(name,(float[])Definitions[name].Clone());}foreach(var pair in replacementAllowances)result.replacementAllowances.Add(pair.Key,pair.Value);result.StoredDefinitionCount=StoredDefinitionCount;return result;}
 public void ReplaceFrom(InputMacroContext source){var copy=source.Clone();Clear();foreach(char name in copy.Order){Order.Add(name);Definitions.Add(name,copy.Definitions[name]);}foreach(var pair in copy.replacementAllowances)replacementAllowances.Add(pair.Key,pair.Value);StoredDefinitionCount=copy.StoredDefinitionCount;}
 }
 public sealed class InputProximityDelimiterException : FormatException {
  public readonly int Caret;
  public InputProximityDelimiterException(int caret):base("Attach P to the following pitch."){Caret=caret;}
 }
 public static class InputTextLists {
  static string Body(string text){if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(!text.EndsWith(";"))throw new InvalidDataException("Terminate the input line with a semicolon");text=text.Substring(0,text.Length-1);if(text.IndexOf(';')>=0)throw new InvalidDataException("Unexpected input after a semicolon");return text;}
  static int Count(string text){int n;if(!int.TryParse(text,out n)||n<1||n>short.MaxValue)throw new InvalidDataException("Repeat count is outside the signed word representation");return n;}
  static int DecodeRepeatCount(string text,ref bool keepAccidentals){
   int total=Count(text);
   // SCORE stores the preserve-accidentals flag in the hundreds range.
   // Explicit X counts reaching that range take the same decoding path.
   if(!keepAccidentals&&total>=100){
    keepAccidentals=true;
    total=Math.Max(1,total-100);
   }
   return total;
  }
  static string[] Fields(string text){string body=Body(text);var fields=new List<string>();int start=0,depth=0;for(int i=0;i<body.Length;i++){if(body[i]=='['){if(depth!=0)throw new NotSupportedException("Nested macro definitions require the original error path");depth++;}else if(body[i]==']'){if(depth!=1)throw new InvalidDataException("Unmatched macro bracket");depth--;}else if(body[i]=='/'&&depth==0){fields.Add(body.Substring(start,i-start));start=i+1;}}if(depth!=0)throw new InvalidDataException("Unclosed macro definition");fields.Add(body.Substring(start));return fields.ToArray();}
  internal static string[] ExpansionFields(string text){return Fields(text);}
  static bool Macro(string text,InputMacroContext context,Func<string,float[]> parse,List<float> result,bool pitches=false){
   Match definition=Regex.Match(text,@"^([A-Z])\[(.*)\]$");if(definition.Success){char name=definition.Groups[1].Value[0];float[] defined=parse(definition.Groups[2].Value+";");context.Define(name,defined);result.AddRange(defined);return true;}
   Match call=Regex.Match(text,@"^@([$\- ]*)([A-Z])([+\-0-9. ]*)$");
   if(!call.Success)return false;
   float[] body;if(!context.Definitions.TryGetValue(call.Groups[2].Value[0],out body))
    throw new InvalidDataException("Undefined input macro "+call.Groups[2].Value);
   float shift=0;string suffix=call.Groups[3].Value.Trim();
   if(suffix.Length>0){int cursor=0;shift=InputRealReader.Read(suffix,ref cursor);
    if(cursor!=suffix.Length)throw new NotSupportedException("This motive suffix reaches an unverified real-reader boundary.");
    if(!pitches&&shift<0)throw new InvalidDataException("Negative rhythm-motive factors are not accepted.");
   }
   var expansion=new List<float>();
   if(call.Groups[1].Value.IndexOf('-')>=0){
    for(int end=body.Length-1;end>=0;){
     int first=end,kind=(int)Math.Abs(body[end])/1000;
     if(body[end]<0&&(kind==1||kind==7)){
      while(first>=0&&body[first]<0)first--;
      if(first<0)throw new InvalidDataException("Reverse motive has no chord leader.");
     }
     for(int i=first;i<=end;i++)expansion.Add(body[i]);
     end=first-1;
    }
   }else expansion.AddRange(body);
   for(int i=0;i<expansion.Count;i++){
    float token=expansion[i],absolute=Math.Abs(token);int kind=(int)absolute/1000;
    if(pitches&&shift!=0&&(kind==1||kind==7)){
     float changed=(float)((double)absolute+shift);expansion[i]=token<0?-changed:changed;
    }else if(shift!=0&&!pitches){float remainder=token%1000;
     expansion[i]=token<1000?(float)((double)token*shift):(float)(((double)remainder*shift+token)-remainder);
    }
   }
   // Overlay8 motive caller's optional $ pass operates on packed pitch
   // words after reversal and displacement, retaining accidental hundreds.
   if(call.Groups[1].Value.IndexOf('$')>=0){
    int anchor=0;
    for(int i=0;i<expansion.Count;i++){
     int value=(int)expansion[i],absolute=Math.Abs(value),kind=absolute/1000;
     if(kind!=1&&kind!=7)continue;
     int degree=absolute%100;
     if(anchor==0){anchor=degree*2;continue;}
     int changed=kind*1000+anchor-degree;if(value<0)changed=-changed;
     expansion[i]=(value%1000/100)*100+changed;
    }
   }
   result.AddRange(expansion);return true;
  }
  // Slash lists, chord members and the literal repeat controls. Macro expansion
  // and original cursor/correction handling are separate caller operations.
  public static float[] Pitches(string text,InputPitchState state){return Pitches(text,state,new InputMacroContext());}
  public static float[] Pitches(string text,InputPitchState state,InputMacroContext context){
   if(text==null)throw new ArgumentNullException("text");
   text=text.TrimEnd();
   int semicolon=text.IndexOf(';');
   if(semicolon>=0&&semicolon<text.Length-1){
    // A semicolon completes the physical row. Inside the pitch list it
    // separates fields, including the tail left by a MIDI Enter overtype.
    char last=text[text.Length-1];
    string body=last==';'||last=='/'?text.Substring(0,text.Length-1):text;
    text=body.Replace(';','/')+";";
   }
   var result=new List<float>();string[] fields=Fields(text);
   int nextField=0,chordState=0;
   for(int fieldIndex=0;fieldIndex<fields.Length;fieldIndex++){
    string field=fields[fieldIndex];
    int delimiter=nextField+field.Length;
    nextField=delimiter+1;
    if(field=="P"&&fieldIndex<fields.Length-1)throw new InputProximityDelimiterException(delimiter);
    string t=field.Trim();if(Macro(t,context,body=>Pitches(body,state,context),result,true))continue;if(t.Length==0){InputListExpansion.Repeat(result,1,2,false);continue;}
    if(t[0]=='.'){InputListExpansion.RepeatBeforePrevious(result,t.Length>1&&t[1]=='.');continue;}
    Match repeat=Regex.Match(t,@"^([XZ])([0-9]+)\s+([0-9]+)$");if(repeat.Success){bool keepSpan=repeat.Groups[1].Value=="Z";int repetitions=DecodeRepeatCount(repeat.Groups[3].Value,ref keepSpan);InputListExpansion.Repeat(result,Count(repeat.Groups[2].Value),repetitions,keepSpan);continue;}
    int total=1;bool keep=false;repeat=Regex.Match(t,@"^(.*?)([XZ])([0-9]+)$");if(repeat.Success){t=repeat.Groups[1].Value;keep=repeat.Groups[2].Value=="Z";total=DecodeRepeatCount(repeat.Groups[3].Value,ref keep);}
    string[] chord=t.Split(new[]{':','\''});int before=result.Count;
    // Original section65's delimiter word survives controls. A colon arms
    // the chord before the current token is emitted; a slash changes an
    // active chord to its final-member state. Only a pitch consumes it.
    for(int i=0;i<chord.Length;i++){
     bool colon=i+1<chord.Length;
     if(colon){if(chordState==0)chordState=-1;}else if(chordState>0)chordState=2;
     float? token=InputPitchToken.Parse(chord[i],state);
     if(token.HasValue){int kind=(int)Math.Abs(token.Value)/1000;
      if(kind==1||kind==7){result.Add(chordState>0?-Math.Abs(token.Value):token.Value);if(chordState<0)chordState=1;else if(chordState==2)chordState=0;}
      else result.Add(token.Value);
     }
    }
    if(total!=1){if(result.Count==before)throw new InvalidDataException("Nothing to repeat");InputListExpansion.Repeat(result,1,total,keep);}
   }
   return result.ToArray();
  }
  public static float[] Rhythms(string text){return Rhythms(text,new InputMacroContext());}
  public static float[] Rhythms(string text,InputMacroContext context){return Rhythms(text,context,false);}
  public static float[] Rhythms(string text,InputMacroContext context,bool originalMantissaWrap){
   var result=new List<float>();foreach(string field in Fields(text)){string t=field.Trim();if(Macro(t,context,body=>Rhythms(body,context,originalMantissaWrap),result))continue;if(t.Length==0){InputListExpansion.Repeat(result,1,2,true);continue;}Match m=Regex.Match(t,@"^X([0-9]+)\s+([0-9]+)$");if(m.Success){bool keepSpan=false;int repetitions=DecodeRepeatCount(m.Groups[2].Value,ref keepSpan);InputListExpansion.Repeat(result,Count(m.Groups[1].Value),repetitions,true);continue;}int total=1;m=Regex.Match(t,@"^(.*?)X([0-9]+)$");if(m.Success){t=m.Groups[1].Value;bool keep=false;total=DecodeRepeatCount(m.Groups[2].Value,ref keep);}result.Add(InputRhythmToken.Parse(t,originalMantissaWrap));InputListExpansion.Repeat(result,1,total,true);}return result.ToArray();
  }
 }
}
