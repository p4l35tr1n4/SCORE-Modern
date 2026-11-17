using System;
using System.Collections.Generic;
using System.Globalization;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 sealed class MidiInputLine {
  static readonly string[] RhythmTokens={
   "W","H","H.","Q","Q.","E","E.","S","32","S.","64","G",
   "TW","TH","TH.","TQ","TQ.","TE","TE.","TS","48","TS.","96"
  };
  // Signed key index DS:AEB8 selects the original rows at DS:BAF4 and DS:BBA8.
  static readonly string[][] KeyPitchSpellings={
   new[]{"CN","DF","EFF","EF","FF","FN","GF","GN","AF","BFF","BF","CF"},
   new[]{"CN","DF","DN","EF","FF","F","GF","GN","AF","BFF","BF","CF"},
   new[]{"C","DF","DN","EF","FF","F","GF","GN","AF","AN","BF","CF"},
   new[]{"C","DF","DN","EF","EN","F","GF","G","AF","AN","BF","CF"},
   new[]{"C","DF","D","EF","EN","F","GF","G","AF","AN","BF","BN"},
   new[]{"C","DF","D","EF","EN","F","FS","G","AF","A","BF","BN"},
   new[]{"C","CS","D","EF","E","F","FS","G","AF","A","BF","BN"},
   new[]{"C","CS","D","EF","E","F","FS","G","GS","A","BF","B"},
   new[]{"C","CS","D","DS","E","FN","FS","G","GS","A","BF","B"},
   new[]{"CN","CS","D","DS","E","FN","FS","G","GS","A","AS","B"},
   new[]{"CN","CS","D","DS","E","ES","FS","GN","GS","A","AS","B"},
   new[]{"BS","CS","DN","DS","E","ES","FS","GN","GS","A","AS","B"},
   new[]{"BS","CS","DN","DS","E","ES","FS","FSS","GS","AN","AS","B"},
   new[]{"BS","CS","CSS","DS","EN","ES","FS","FSS","GS","AN","AS","B"},
   new[]{"BS","CS","CSS","DS","EN","ES","FS","FSS","GS","GSS","AS","BN"},
  };
  enum AccidentalRoute {Suppress, FromSpelling, Up, Down}
  const AccidentalRoute None=AccidentalRoute.Suppress, Relative=AccidentalRoute.FromSpelling,
   Up=AccidentalRoute.Up, Down=AccidentalRoute.Down;
  // Original pitch-class jumps at 2CF0:0C85, 0CB1, 0D08 and 0D34.
  static readonly AccidentalRoute[] FlatRoute={None,Relative,None,Relative,Up,None,Relative,None,Relative,None,Relative,Up};
  static readonly AccidentalRoute[] SharpRoute={Down,Relative,None,Relative,None,Down,Relative,None,Relative,None,Relative,None};
  static readonly AccidentalRoute[] DoubleFlatRoute={Up,None,Up,None,None,Up,None,Up,None,Up,None,None};
  static readonly AccidentalRoute[] DoubleSharpRoute={None,None,Down,None,Down,None,None,Down,None,Down,None,Down};
  static readonly string[] MidiKeySignatures={null,"K5F","K2S","K3F","K4S","K1F","K6S","K1S","K4F","K3S","K2F","K5S"};
  static readonly string[] MidiControlSelectors={null,"K5F","K2S","K3F","TE","AL","TE","BA","TE","TR","K2F"};
  Dictionary<int,string> accidentals=new Dictionary<int,string>();
  // SCORE keeps the spelling of each MIDI pitch class across bars and octaves.
  string[] spellingOverrides=new string[12];
  readonly bool groupHeldPitches;
  readonly HashSet<int> heldPitches=new HashSet<int>();
  readonly Stack<PitchSnapshot> pitchUndo=new Stack<PitchSnapshot>();
  int activeKey;
  bool anchorAfterSignature,proximityActive;
  int previousDegree=-1;
  int accidentalKey=-1;
  int octaveShift;
  int octaveControls,accidentalCode,barSpan;
  bool keyboardRhythm;
  string last="";
  internal readonly InputStaffStage Stage;
  internal string Notice {get;private set;}

  struct PitchSnapshot {
   internal string Line,Accidental;
   internal int Previous,Pitch,Key;
   internal bool AnchorAfterSignature,ProximityActive;
   internal string[] PreviousSpellings;
   internal Dictionary<int,string> BarAccidentals;
  }

  internal MidiInputLine(InputStaffStage stage):this(stage,false){}
  internal MidiInputLine(InputStaffStage stage,bool groupHeldPitches){Stage=stage;this.groupHeldPitches=groupHeldPitches;}

  internal void Release(int key){heldPitches.Remove(key);}

  internal string Note(int key,int baseKey,string current,int rhythms){
   Notice=null;
   if(Stage==InputStaffStage.Rhythms)return Rhythm(key,current,rhythms);
   if(current!=last)SynchronizeKeyboardPrefix(current);

   int control=key-baseKey;
   if(control==11){octaveControls++;return null;}
   if(octaveControls>0){
    int taps=octaveControls;
    octaveControls=0;
    if(taps==1){octaveShift=5-key/12;Notice=key==60?"Middle C at normal position.":"Middle C = "+(-octaveShift).ToString(CultureInfo.InvariantCulture)+" octave(s) above normal.";return null;}
    if(taps>=3){
     switch(key%12){
      case 11:return Control(current,"K7F");
      case 1:return Control(current,"K7S");
      case 6:return Control(current,"K6F");
     }
    }
    if(taps>=2){
     string selector=control>=0&&control<MidiControlSelectors.Length
      ?MidiControlSelectors[control]:MidiKeySignatures[key%12];
     return selector==null?null:Control(current,selector);
    }
   }
   switch(control){
    case 0:
     string bar=accidentalKey==1?"MD":accidentalKey==3?"MDH":"M";
     return Control(current,bar+(barSpan==0?"":barSpan.ToString(CultureInfo.InvariantCulture)),true);
    case 1:case 2:case 3:
     accidentalKey=control;
     accidentalCode=unchecked((short)(accidentalCode+(control==2?3:control==3?2:1)));
     if(control==1&&accidentalCode==2)accidentalCode=4;
     if(control==3&&accidentalCode==4)accidentalCode=5;
     if(control==2)barSpan++;
     return null;
    case 4:return Control(current,accidentalKey==3?"RI":"R");
    case 5:return Control(current,"RW");
    case 6:return Backspace(current);
    case 7:return current+";";
    case 8:return Control(current,"SD");
    case 9:return Control(current,"SO");
    case 10:return Control(current,"SU");
   }

   int octave=key/12-1+octaveShift;
   if(octave<0||octave>9)return null;
   int semitone=key%12;
   string remembered=spellingOverrides[semitone];
   string name=remembered??KeyPitchSpellings[activeKey+7][semitone];
   bool modified=accidentalKey>=0;
   bool suppressAccidental=false;
   if(modified){
    int code=accidentalCode;
    ResetAccidental();
    string previousSpelling=name;
    name=ModifiedPitch(code,semitone,previousSpelling);
    suppressAccidental=name==null;
    if(suppressAccidental)name=previousSpelling;
    if(name.Length>1&&name[0]!=previousSpelling[0])
     spellingOverrides[semitone]=name[1]=='N'?name.Substring(0,1):name;
   }

   if(semitone==0&&name[0]=='B')octave--;
   if(semitone==11&&name[0]=='C')octave++;
   if(octave<0||octave>9)return null;
   int degree=octave*7+"CDEFGAB".IndexOf(name[0]);
   string accidental=name.Substring(1);
   string old;
   accidentals.TryGetValue(degree,out old);
   string effective=old;
   if(effective==null){
    if(activeKey<0&&"BEADGCF".IndexOf(name[0])< -activeKey)effective="F";
    if(activeKey>0&&"FCGDAEB".IndexOf(name[0])<activeKey)effective="S";
   }
   string applied=accidental=="N"?"":accidental;
   string mark=suppressAccidental?"":modified?accidental:applied==effective?"":applied.Length==0&&!String.IsNullOrEmpty(effective)?"N":applied;
   string oct=previousDegree<0||anchorAfterSignature||Math.Abs(previousDegree-degree)>3?octave.ToString(CultureInfo.InvariantCulture):"";
   bool chord=groupHeldPitches&&!NativeScoreBehaviour.Enabled&&heldPitches.Count>0&&pitchUndo.Count>0&&pitchUndo.Peek().Pitch>=0;
   heldPitches.Add(key);
   pitchUndo.Push(new PitchSnapshot{Line=current,Accidental=old,Previous=previousDegree,Pitch=degree,
    Key=activeKey,AnchorAfterSignature=anchorAfterSignature,ProximityActive=proximityActive});
   if(!suppressAccidental)accidentals[degree]=applied;
   string token=(proximityActive?"":"P")+name[0]+mark+oct;
   proximityActive=true;
   previousDegree=degree;
   anchorAfterSignature=false;
   return last=chord?current+":"+token:Append(current,token);
  }

  // Overlay8/index62, keyboard-prefix scanner3000:5DD3. This
  // reconstructs the MIDI reader's context, not the score compiler's state.
  void SynchronizeKeyboardPrefix(string current){
   previousDegree=-1;anchorAfterSignature=false;
   Array.Clear(spellingOverrides,0,spellingOverrides.Length);ResetAccidental();
   octaveControls=octaveShift=0;heldPitches.Clear();accidentals.Clear();pitchUndo.Clear();
   int octave=0,previousLetter=0;var block=new System.Text.StringBuilder();
   foreach(char c in (current+"/").ToUpperInvariant()){
    if(c=='P'){proximityActive=true;continue;}
    if(c=='O'){proximityActive=false;continue;}
    if(c=='M'){accidentals.Clear();block.Clear();continue;}
    if(c!='/'&&c!=':'&&c!='\''){if(c!=' '&&block.Length<7)block.Append(c);continue;}
    string token=block.ToString();block.Clear();if(token.Length<2)continue;
    if(token=="TR"||token=="BA"||token=="AL"||token=="TE")continue;
    var signature=System.Text.RegularExpressions.Regex.Match(token,@"^K([0-7])([FS])(N?)$");
    if(signature.Success){activeKey=(signature.Groups[1].Value[0]-'0')*(signature.Groups[2].Value=="F"?-1:1);if(signature.Groups[3].Success&&signature.Groups[3].Length>0)activeKey=0;accidentals.Clear();Array.Clear(spellingOverrides,0,spellingOverrides.Length);previousDegree=0;previousLetter=0;anchorAfterSignature=true;continue;}
    var note=System.Text.RegularExpressions.Regex.Match(token,@"^([A-G])(FF|SS|F|S|N)?([0-9]?)");
    if(!note.Success||token[1]=='U'||token[1]=='J')continue;
    int letter="CDEFGAB".IndexOf(note.Groups[1].Value[0]);
    if(note.Groups[3].Length>0)octave=note.Groups[3].Value[0]-'0';
    else if(proximityActive&&Math.Abs(letter-previousLetter)>3)octave+=letter>previousLetter?-1:1;
    previousDegree=octave*7+letter;previousLetter=letter;anchorAfterSignature=false;
    string suffix=note.Groups[2].Value;
    if(suffix.Length>0){
     // The original keyboard scanner's single-S table word is2, as is
     // its double-S word; this differs from musical semitone arithmetic.
     int alteration=suffix=="N"?0:suffix[0]=='F'?-suffix.Length:2;
     int center=((activeKey+7)*4%7)*2;if(center>4)center--;if(activeKey>5)center++;if(activeKey< -1)center--;
     int index=new[]{0,2,4,5,7,9,11}[letter]+alteration;
     int relative=index-center;if(relative<0)relative+=12;
     // CS:1354 dispatches natural relative classes to14BE (no write),
     // black classes to136C. 0F92 writes the selected spelling and the
     // keyboard pitch's accidental word; the active row has12 entries.
     if(relative<0||relative>=12||relative==1||relative==3||relative==6||relative==8||relative==10){
      // 0F92 receives the unreduced index in BC0A, while the dispatch
      // uses D804. Out-of-row writes do not change the active row, but
      // the helper still stores the keyboard note's accidental.
      if(index>=0&&index<12)spellingOverrides[index]=token[0]+suffix;
      accidentals[previousDegree]=suffix=="N"?"":suffix;
     }
    }
   }
   last=current;
  }

  void ResetAccidental(){accidentalKey=-1;accidentalCode=barSpan=0;}

  string Control(string current,string token,bool bar=false){
   ResetAccidental();
   bool keyChange=token.StartsWith("K",StringComparison.Ordinal);
   pitchUndo.Push(new PitchSnapshot{Line=current,Previous=previousDegree,Pitch=-1,Key=activeKey,
    AnchorAfterSignature=anchorAfterSignature,ProximityActive=proximityActive,BarAccidentals=bar||keyChange?accidentals:null,
    PreviousSpellings=keyChange?spellingOverrides:null});
   if(bar||keyChange)accidentals=new Dictionary<int,string>();
   if(keyChange){
    activeKey=(token[1]-'0')*(token[2]=='F'?-1:1);
    spellingOverrides=new string[12];
    anchorAfterSignature=true;
   }
   return last=current+(current.EndsWith("/",StringComparison.Ordinal)?"":"/")+token+(bar?" ":"");
  }

  string Backspace(string current){
   ResetAccidental();
   heldPitches.Clear();
   if(pitchUndo.Count==0)return null;
   var previous=pitchUndo.Pop();
   activeKey=previous.Key;
   anchorAfterSignature=previous.AnchorAfterSignature;proximityActive=previous.ProximityActive;
   if(previous.PreviousSpellings!=null)spellingOverrides=previous.PreviousSpellings;
   previousDegree=previous.Previous;
   if(previous.BarAccidentals!=null)accidentals=previous.BarAccidentals;
   if(previous.Pitch>=0){
    if(previous.Accidental==null)accidentals.Remove(previous.Pitch);
    else accidentals[previous.Pitch]=previous.Accidental;
   }
   return last=previous.Line;
  }

  static string ModifiedPitch(int code,int semitone,string currentSpelling){
   string suffix;
   AccidentalRoute route;
   switch(code){
    case 1:suffix="F";route=FlatRoute[semitone];break;
    case 2:suffix="S";route=SharpRoute[semitone];break;
    case 3:suffix="N";route=Relative;break;
    case 4:suffix="FF";route=DoubleFlatRoute[semitone];break;
    case 5:suffix="SS";route=DoubleSharpRoute[semitone];break;
    default:return null;
   }
   string currentSuffix=currentSpelling.Substring(1);
   if(currentSuffix==suffix)return currentSpelling;
   switch(route){
    case None:return null;
    case Up:return ShiftPitchLetter(currentSpelling[0],1)+suffix;
    case Down:return ShiftPitchLetter(currentSpelling[0],-1)+suffix;
    default:
     int shift=currentSuffix.StartsWith("F",StringComparison.Ordinal)?-1:
      currentSuffix.StartsWith("S",StringComparison.Ordinal)?1:0;
     return ShiftPitchLetter(currentSpelling[0],shift)+suffix;
   }
  }

  static char ShiftPitchLetter(char letter,int shift){
   const string letters="ABCDEFG";
   return letters[(letters.IndexOf(letter)+shift+7)%7];
  }

  string Rhythm(int key,string current,int needed){
   keyboardRhythm|=current!=last;
   int index=key-64;
   if(index<0||index>=RhythmTokens.Length){
    switch(key%12){
     case 6:keyboardRhythm=true;return last=current.Length==0?current:current.Substring(0,current.Length-1);
     case 7:return current+";";
     default:keyboardRhythm=true;return last=current+" ";
    }
   }
   last=current.Length==0?" "+RhythmTokens[index]:Append(current,RhythmTokens[index]);
   if(!keyboardRhythm&&InputTextLists.Rhythms(last+";").Length==needed)return last+";";
   return last;
  }

  static string Append(string line,string token){
   return line.Length==0?token:line+(line.EndsWith("/",StringComparison.Ordinal)||line.EndsWith(":",StringComparison.Ordinal)||line.EndsWith("'",StringComparison.Ordinal)?"":"/")+token;
  }
 }
}


