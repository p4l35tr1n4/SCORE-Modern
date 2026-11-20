using System;using System.Collections.Generic;using System.IO;using System.Linq;
namespace ScoreModern {
 internal static class NativeScor4FileFeatures {
  internal static string[] Describe(Score score){
   var result=new List<string>();
   foreach(var item in score.Items){
    if(item.Code==4&&item.P(16)==1&&(item.P(5)>=999||item.P(7)==0))result.Add(item.P(5)>=999?"hairpin endpoint offsets (Code 4, P16–P18)":"straight-line endpoint offsets (Code 4, P16–P18)");
    if(item.Code==5&&item.P(16)==1)result.Add("version-dependent slur endpoint mode (Code 5, P16–P18)");
    if(item.Code==7&&item.P(7)==1&&item.P(15)!=0)result.Add("version-dependent harp-pedal control (Code 7, P15: SCORE 3.11 line angle / SCOR4 horizontal offset)");
    if(item.Code==11&&item.P(11)!=0)result.Add("user-drawing horizontal offset (Code 11, P11; commercial SCOR4)");
    if(item.Code==10&&item.P(16)==1)result.Add("number justification control (Code 10, P16)");
    if(item.Code==18&&item.P(16)!=0)result.Add("time-signature printer-outline thickness adjustment (Code 18, P16)");
   }
   return result.Distinct().ToArray();
  }
 }

 public sealed partial class Editor {
  string pendingScor4FileNotice;bool scor4FileNoticeQueued;
  void QueueCoreScor4FileNotice(Score document,string path){
   pendingScor4FileNotice=null;if(scor4Interface||String.IsNullOrEmpty(path))return;
   var features=NativeScor4FileFeatures.Describe(document);if(features.Length==0)return;
   pendingScor4FileNotice=Path.GetFileName(path)+" contains SCOR4 extensions or version-dependent controls:\r\n\r\n"+String.Join("\r\n",features)+"\r\n\r\nThe active profile is SCORE 3.11. The score remains open and all parameters are preserved. Select SCOR4 in Preferences for SCOR4 interpretation.";
   FlushCoreScor4FileNotice();
  }
  void FlushCoreScor4FileNotice(){if(Program.CheckMode||!IsHandleCreated||!Visible||scor4FileNoticeQueued||pendingScor4FileNotice==null)return;
   scor4FileNoticeQueued=true;BeginInvoke(new Action(()=>{scor4FileNoticeQueued=false;string text=pendingScor4FileNotice;pendingScor4FileNotice=null;if(!IsDisposed&&!scor4Interface&&text!=null)NativeInformationNotice.Show(this,"scor4-file-profile","SCOR4 settings in SCORE 3.11 mode",text);}));
  }
 }
}
