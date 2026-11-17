using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  // Physical rows are retained separately from the five compiler lists.
  // Original measurements: stage-7/continuation-original and
  // motive-across-lines-original. A slash continues without compacting items.
  string InputPhysicalPrefix(){return String.Concat(musicInput.PendingPhysical.ToArray());}
  static bool IsInputContinuation(string text,bool pitches){
   return text.TrimEnd().EndsWith("/",StringComparison.Ordinal)&&(!pitches||text.IndexOf(';')<0);
  }
  static string CompleteInputPrefix(string text){
   int depth=0,last=0;
   for(int i=0;i<text.Length;i++){
    if(text[i]=='[')depth++;
    else if(text[i]==']')depth--;
    else if(text[i]=='/'&&depth==0)last=i+1;
    if(depth<0)throw new InvalidDataException("Unmatched macro bracket");
   }
   // The final slash is a delimiter, not an empty repeat token.
   return last==0?";":text.Substring(0,last-1)+";";
  }
  void ParseInputContinuation(string text){
   var motives=(musicInput.ContinuationBaseline??inputMotives).Clone();
   string complete=CompleteInputPrefix(text);
   int rhythms=0;
   switch(musicInput.Transaction.Stage){
    case InputStaffStage.Pitches:
     var state=InputPitchDefaults();state.Proximity=musicInput.PitchProximity;
     if(complete!=";")InputTextLists.Pitches(complete,state,motives);
     musicInput.ProximityAfterError=state.Proximity;break;
    case InputStaffStage.Rhythms:if(complete!=";")rhythms=InputTextLists.Rhythms(complete,motives,NativeEditCompatibility.ReproduceInvalidStates).Length;break;
    case InputStaffStage.Marks:InputMarkText.Parse(complete,motives,NativeEditCompatibility.RejectMultiDigitF);break;
    case InputStaffStage.Beams:InputEndpointText.Parse(complete,true,inputSetup.MeterTop,inputSetup.MeterBottom,motives);break;
    case InputStaffStage.Slurs:InputEndpointText.Parse(complete,false,inputSetup.MeterTop,inputSetup.MeterBottom,motives);break;
   }
   musicInput.PendingRhythms=rhythms;
   motives.ClearReplacementAllowances();inputMotives.ReplaceFrom(motives);
  }
  IEnumerable<string> InputPhysicalRows(){
   for(int i=0;i<musicInput.Lines.Count;i++){
    if(i<musicInput.PhysicalGroups.Count)foreach(string row in musicInput.PhysicalGroups[i])yield return row;
    else yield return musicInput.Lines[i];
   }
  }
  int InputCorrectionStage(int index){
   if(index<2)return index;
   int stage=2;
   for(int i=0;i<index-2;i++)if(!IsInputContinuation(musicInput.CorrectionLines[i],stage==2))stage++;
   return stage;
  }
  static List<List<string>> GroupInputRows(IEnumerable<string> rows,bool allowUnfinished=false){
   var groups=new List<List<string>>();var current=new List<string>();
   foreach(string row in rows){current.Add(row);if(!IsInputContinuation(row,groups.Count==0)){groups.Add(current);current=new List<string>();}}
   if(current.Count>0){if(!allowUnfinished)throw new InvalidDataException("The final physical input row is unfinished.");groups.Add(current);}
   return groups;
  }
 }
}
