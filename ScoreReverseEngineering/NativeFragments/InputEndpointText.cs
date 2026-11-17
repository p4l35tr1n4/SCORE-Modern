using System;using System.Collections.Generic;using System.Globalization;using System.IO;using System.Text.RegularExpressions;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputAutomaticBeamMeterException:FormatException {
  public InputAutomaticBeamMeterException(int top,int bottom):base("Cannot auto-beam time signature "+top.ToString(CultureInfo.InvariantCulture).PadLeft(2)+"/"+bottom.ToString(CultureInfo.InvariantCulture).PadLeft(2)+" Correct line then press <Enter>. Press <ESC> to Exit."){}
 }
 public sealed class InputEndpointRepromptException:FormatException {}
 public sealed class InputEndpointTokens {public float[] Pairs;public bool Automatic;public float GroupControl;public int Skip;}
 public static class InputEndpointText {
  // Overlay8 section69: the leading '+' control is encoded on the first
  // endpoint, independently of the numeric sign of the first endpoint.
  public static InputEndpointTokens Parse(string text,bool beams,int meterTop,int meterBottom){
   if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(!text.EndsWith(";"))throw new InvalidDataException("Terminate endpoints with a semicolon");text=text.Substring(0,text.Length-1).Trim();var result=new InputEndpointTokens{Pairs=new float[0]};if(text.Length==0)return result;
   if(beams){if(text=="A"||text=="B"||text=="AB"){int group;if(meterBottom==0||meterBottom==4||meterTop==99)group=2;else if(meterBottom==2||meterTop==98)group=4;else if(meterBottom==1)group=8;else if(meterBottom==8){if(meterTop==2||meterTop==4||meterTop==8)group=2;else if(meterTop==3||meterTop==6||meterTop==9||meterTop==12)group=3;else throw new InputAutomaticBeamMeterException(meterTop,meterBottom);}else throw new InputAutomaticBeamMeterException(meterTop,meterBottom);result.Automatic=true;result.GroupControl=group;return result;}
    Match auto=Regex.Match(text,@"^[+-]?((?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+))B\s*([+-]?(?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+))?$");if(auto.Success){result.Automatic=true;result.GroupControl=Number(auto.Groups[1].Value);result.Skip=auto.Groups[2].Success?checked((short)Number(auto.Groups[2].Value)):0;return result;}}
   var pairs=new List<float>();foreach(string field in text.Split('/')){
    string part=field.Trim();if(part.Length==0)continue;int at=0;
    bool forceUp=part[at]=='+';if(forceUp)at++;
    float first=InputRealReader.Read(part,ref at);
    if(at<part.Length)at++;while(at<part.Length&&part[at]==' ')at++;
    if(at<part.Length&&part[at]=='+'){forceUp=true;at++;}
    float last=InputRealReader.Read(part,ref at);
    if(forceUp)first=(float)((double)first+10000);pairs.Add(first);pairs.Add(last);
   }result.Pairs=pairs.ToArray();return result;
  }
  public static InputEndpointTokens Parse(string text,bool beams,int meterTop,int meterBottom,InputMacroContext context){
   if(context==null)throw new ArgumentNullException("context");var pairs=new List<float>();InputEndpointTokens automatic=null;
   foreach(string field in InputTextLists.ExpansionFields(text.ToUpperInvariant())){string part=field.Trim();if(part.Length==0)continue;Match definition=Regex.Match(part,@"^([A-Z])\[(.*)\]$");if(definition.Success){char name=definition.Groups[1].Value[0];var body=Parse(definition.Groups[2].Value+";",beams,meterTop,meterBottom,context);if(body.Automatic){
     string source=definition.Groups[2].Value.Trim();
     if(source=="A"||source=="B"||source=="AB")throw new InvalidDataException("Store a numeric automatic-beam interval in a motive.");
     if(automatic!=null||pairs.Count!=0)throw new InputEndpointRepromptException();
     context.Define(name,new[]{9999f,body.GroupControl,body.Skip});
     automatic=body;
    }else{
     if(automatic!=null)throw new InputEndpointRepromptException();
     context.Define(name,body.Pairs);
     pairs.AddRange(body.Pairs);
    }
    continue;}Match recall=Regex.Match(part,@"^@(-?)([A-Z])([+\-0-9. ]*)$");
    if(recall.Success){
     string suffix=recall.Groups[3].Value.Trim();
     if(suffix.Length>0)Number(suffix);
     char name=recall.Groups[2].Value[0];
     float[] body;
     if(!context.Definitions.TryGetValue(name,out body))throw new InvalidDataException("Undefined input motive "+name);
     if(beams&&body.Length==3&&body[0]==9999){
      if(automatic!=null||pairs.Count!=0)throw new InputEndpointRepromptException();
      automatic=new InputEndpointTokens{Automatic=true,GroupControl=body[1],Skip=checked((short)body[2]),Pairs=new float[0]};
      continue;
     }
     if(automatic!=null)throw new InputEndpointRepromptException();
     if(body.Length%2!=0)throw new InvalidDataException("Endpoint motive requires paired words.");
     if(recall.Groups[1].Length==0)pairs.AddRange(body);
     else for(int i=body.Length-2;i>=0;i-=2){
      pairs.Add(body[i]);
      pairs.Add(body[i+1]);
     }
     continue;
    }var parsed=Parse(part+";",beams,meterTop,meterBottom);if(parsed.Automatic){if(automatic!=null||pairs.Count!=0)throw new InputEndpointRepromptException();automatic=parsed;}else{if(automatic!=null)throw new InputEndpointRepromptException();pairs.AddRange(parsed.Pairs);}}
   return automatic??new InputEndpointTokens{Pairs=pairs.ToArray()};
  }
  static float Number(string text){int at=0;float value=InputRealReader.Read(text,ref at);if(at!=text.Length)throw new NotSupportedException("Automatic-beam number reaches an unverified real-reader boundary.");return value;}
 }
}
