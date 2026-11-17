using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputPitchState {public int Octave=4,Previous=29,PreviousDegree=1,KeySignature,MeterTop=4,MeterBottom=4,BarSpan=1;public bool Proximity;}
 public static class InputPitchToken {
  // Candidate transcription of section68's pitch-token leaf. List expansion,
  // chord delimiters, macro calls and saved-input replay are separate routines.
  public static float? Parse(string text,InputPitchState state){
   if(state==null)throw new ArgumentNullException("state");if(text==null)throw new ArgumentNullException("text");string t=text.Trim().ToUpperInvariant();if(t.Length==0)throw new InvalidDataException("Missing pitch token.");
   if(t=="P")throw new InvalidDataException("Attach P to the following pitch, for example PD.");
   while(t.Length>0&&(t[0]=='P'||t[0]=='O')){state.Proximity=t[0]=='P';t=t.Substring(1);}
   if(t.Length==0)return null;
   switch(t){case "TR":return 3000;case "BA":return 3001;case "AL":return 3002;case "TE":return 3003;case "-TR":return -3000;case "-BA":return -3001;case "-AL":return -3002;case "-TE":return -3003;case "SU":return 5010;case "SD":return 5020;case "SO":return 5000;case "S+":return 6001;case "S-":return 6002;case "S0":return 6000;case "Q+":return 8001;case "Q-":case "Q0":return 8000;case "R":return 2000;case "RI":return 2001;case "RW":return 2002;case "RP":return 2003;case "RD":return 2004;case "RU":return 2005;case "RF":return 2006;case "COM":state.MeterTop=99;state.MeterBottom=1;return 29901;case "CUT":state.MeterTop=98;state.MeterBottom=1;return 29801;case "T":return 20000+state.MeterTop*100+state.MeterBottom;case "K":return Key(state.KeySignature);}
   Match m=Regex.Match(t,@"^K([0-9]+)([SF])(N?)$");if(m.Success){int n=Int(m.Groups[1].Value);if(m.Groups[3].Length!=0)n+=100;return Key(m.Groups[2].Value=="F"?-n:n);}
   m=Regex.Match(t,@"^([0-9]+)\s+([0-9]+)$");if(m.Success){int top=Int(m.Groups[1].Value),bottom=Int(m.Groups[2].Value);if(bottom==0){bottom=top;top=0;}state.MeterTop=top;state.MeterBottom=bottom;return 20000+top*100+bottom;}
   if(Regex.IsMatch(t,@"^[0-9]+$")){
    int bottom=Int(t);
    state.MeterTop=0;state.MeterBottom=bottom;
    return 20000+bottom;
   }
   m=Regex.Match(t,@"^R([0-9]+)$");if(m.Success)return -2000-Int(m.Groups[1].Value);
   m=Regex.Match(t,@"^M(DH|D|H|R|L|RL|RR|RD|S|DS|I)?\s*([0-9]*)$");if(m.Success){int type=0;switch(m.Groups[1].Value){case "D":type=100;break;case "DH":case "H":type=200;break;case "L":case "RL":type=300;break;case "R":case "RR":type=400;break;case "RD":type=500;break;case "S":case "DS":type=700;break;case "I":type=900;break;}if(m.Groups[2].Length!=0)state.BarSpan=Int(m.Groups[2].Value);return 4000+type+state.BarSpan;}
   m=Regex.Match(t,@"^([A-G])(\(?)((?:FF|SS|F|S|N)?)([0-9]?)([UJ]?)$");if(!m.Success)throw new InvalidDataException("Unsupported pitch token: "+text);
   int degree="CDEFGAB".IndexOf(m.Groups[1].Value[0])+1,accidental=0;switch(m.Groups[3].Value){case "F":accidental=100;break;case "S":accidental=200;break;case "N":accidental=300;break;case "FF":accidental=400;break;case "SS":accidental=500;break;}
   bool octave=m.Groups[4].Length!=0;if(octave)state.Octave=m.Groups[4].Value[0]-'0';
   bool jump=m.Groups[5].Length!=0;if(jump){if(m.Groups[5].Value=="U"&&degree<=state.PreviousDegree)state.Octave++;if(m.Groups[5].Value=="J"&&state.PreviousDegree<=degree)state.Octave--;}
   int absolute=state.Octave*7+degree;if(state.Proximity&&!octave&&!jump){int delta=state.Previous-absolute;if(delta>3){state.Octave++;absolute+=7;}if(delta< -3){state.Octave--;absolute-=7;}}
   state.Previous=absolute;state.PreviousDegree=degree;return 1000+absolute+accidental+(m.Groups[2].Length==0?0:6000);
  }
  static int Int(string t){int value;if(!int.TryParse(t,NumberStyles.None,CultureInfo.InvariantCulture,out value)||value>999)throw new InvalidDataException("Input control exceeds this token projection's measured domain.");return value;}
  static int Key(int value){return value==0?17000:(value<0?-17000+value:17000+value);}
 }
}

