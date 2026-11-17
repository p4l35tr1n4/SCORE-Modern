using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Globalization;using System.Windows.Forms;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  enum TransposePrompt { Staff, Interval, Menu, Clefs, ClefChoice, Key, Warning, Normalize }
  const string TransposeStaffPrompt="Enter staff number to transpose or \"B\" or <ESC> to backout. ->";
  const string TransposeIntervalPrompt="Enter number of half steps (+/-) or \"M\" for a Menu of transpositions. ->";
  const string TransposeClefsPrompt="Change clefs? (y or n, <Enter>=no) ->";
  const string TransposeKeyPrompt="Add key signature? (y or n, <Enter>=no) ->";
  TransposePrompt transposePrompt;float transposeStaff;
  bool transposeCheckpoint;
  TranspositionOperationSession transposeOperation;
  Score transposeDisplayStart;
  RetainedParameterStorage transposeParameterStart;
  // Fresh original DS captures establish these resident words; the workspace
  // persists between transpositions rather than resetting on each request.
  readonly TranspositionWorkspace transposeWorkspace=new TranspositionWorkspace(new TranspositionCollectorState(0),new TranspositionClefState(0x2020,0x2020),new short[200],new short[200],0,new TranspositionRelationshipWindow(),false);
  bool IsTransposeGroup {get{return groupCommand=="TR"||groupCommand=="TRK";}}
  bool ExecuteMainTranspose(string verb,string arguments){
   if(verb!="TR"&&verb!="TRK")return false;
   var numericArguments=arguments.Split(new[]{' ',',','\t'},StringSplitOptions.RemoveEmptyEntries);
   foreach(string value in numericArguments){float number;if(!Single.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out number)||Single.IsNaN(number)||Single.IsInfinity(number))throw new InvalidDataException("TR requires finite numeric arguments.");}
   transposeDisplayStart=CanTrackDisplayRedraw()?score.Clone():null;
   transposeParameterStart=null;
   if(NativeEditCompatibility.ReproduceInvalidStates&&score.Trailer.Length==20&&score.RawStorageLayoutVerified&&
      score.OriginalFilePayload!=null&&!parameterStorageDirty){
    var active=ParameterBytes(score);
    if(active.Length+20==score.OriginalFilePayload.Length&&
       active.SequenceEqual(score.OriginalFilePayload.Take(active.Length))&&parameterStorage.Matches(active))
     transposeParameterStart=parameterStorage.Clone();
   }
   groupCommand=verb;groupRange=null;transposeOperation=null;transposeCheckpoint=false;
   var rows=score.Items.Select(TransposeRow).ToList();
   int[] order=InputTranspositionOrder.OrderForMain(rows,transposeWorkspace.AlreadySorted).ToArray();
   bool modified=InputTranspositionOrder.MarksModified(rows.Count,transposeWorkspace.AlreadySorted);
   if(modified||order.Where((value,index)=>value!=index).Any()){
    Checkpoint();transposeCheckpoint=true;score.Items=order.Select(index=>score.Items[index]).ToList();originalLayoutMarksModified|=modified;dirty|=modified;RefreshAll(null);
    if(transposeDisplayStart!=null)transposeDisplayStart.Items=order.Select(index=>transposeDisplayStart.Items[index]).ToList();
   }
   transposePrompt=TransposePrompt.Staff;
   string first=numericArguments.FirstOrDefault();
   // Main's parent reads the first numeric argument; later numeric words do
   // not supply a range. A literal positive zero takes the staff-question path.
   float numeric;
   if(first==null||Single.TryParse(first,NumberStyles.Float,CultureInfo.InvariantCulture,out numeric)&&BitConverter.ToUInt32(BitConverter.GetBytes(numeric),0)==0)Message(TransposeStaffPrompt);
   else SubmitMainTranspose(first);
   command.Focus();return true;
  }
  static float[] TransposeRow(Item item){var result=new float[item.Count];for(int p=0;p<result.Length;p++)result[p]=item.P(p+1);return result;}
  static string SeparateTransposeStaff(string text){
   int length=text.StartsWith("TRK",StringComparison.OrdinalIgnoreCase)?3:text.StartsWith("TR",StringComparison.OrdinalIgnoreCase)?2:0;
   if(length!=0&&text.Length>length){char value=text[length];if(Char.IsDigit(value)||value=='+'||value=='-'||value=='.')return text.Insert(length," ");}
   return text;
  }
  static short TransposeWord(string text){
   float value;if(String.IsNullOrWhiteSpace(text))value=0;
   else if(!Single.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value)||Single.IsInfinity(value)||Single.IsNaN(value))throw new InvalidDataException("Enter a finite SCORE number.");
   long integer;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out integer))throw new InvalidDataException("Number outside the original integer converter.");
   return unchecked((short)integer);
  }
  void TransposeShow(TransposePrompt next,string text){transposePrompt=next;Message(text);command.Focus();}
  void PublishTransposeRows(){
   if(transposeOperation==null)return;
   var next=transposeOperation.Result.Rows.Select(row=>Item.Make(row)).ToList();
   // The engine edits parameter words in existing records. Equal-length
   // records retain their raw count word, including converter-written bits.
   // A new/extended record receives the newly constructed count instead.
   for(int i=0;i<Math.Min(next.Count,score.Items.Count);i++)if(next[i].Count==score.Items[i].Count)Buffer.BlockCopy(score.Items[i].Bytes,0,next[i].Bytes,0,4);
   bool changed=next.Count!=score.Items.Count||next.Where((item,index)=>index>=score.Items.Count||!item.Bytes.SequenceEqual(score.Items[index].Bytes)).Any();
   if(changed){if(!transposeCheckpoint){Checkpoint();transposeCheckpoint=true;}score.Items=next;dirty=true;RefreshAll(null);}
  }
  void TransposeAskKey(){
   if(transposeOperation.Result.KeyQuestion)TransposeShow(TransposePrompt.Key,TransposeKeyPrompt);
   else FinishMainTranspose(false);
  }
  void FinishMainTranspose(bool key){
   var result=InputTranspositionOperation.Finish(transposeOperation,key);PublishTransposeRows();
   RefreshTransposeTextTerminators();
   if(result.WarningRequired)TransposeShow(TransposePrompt.Warning,InputTranspositionEmbeddedControl.FingeringWarning);
   else TransposeShow(TransposePrompt.Normalize,EnharmonicNormalizePrompt);
  }
  void SubmitMainTranspose(string input){
   string text=input.Trim(),answer=text.ToUpperInvariant();
   if(answer=="B"){
    switch(transposePrompt){
     case TransposePrompt.Clefs:case TransposePrompt.ClefChoice:transposeOperation=null;TransposeShow(TransposePrompt.Interval,TransposeIntervalPrompt);return;
     case TransposePrompt.Key:TransposeShow(TransposePrompt.Clefs,TransposeClefsPrompt);return;
     default:ClearMainGroup();transposeOperation=null;Message("");return;
    }
   }
   switch(transposePrompt){
    case TransposePrompt.Staff:
     transposeStaff=TransposeWord(text);
     if(transposeStaff<1&&!NativeEditCompatibility.ReproduceInvalidStates)throw new InvalidDataException("Enter a positive staff number.");
     TransposeShow(TransposePrompt.Interval,"Transpose staff number "+transposeStaff.ToString(CultureInfo.InvariantCulture)+Environment.NewLine+TransposeIntervalPrompt);return;
    case TransposePrompt.Interval:
     if(answer=="M"){
      var menu=new List<string>();for(short n=1;n<=16;n++)menu.Add(n.ToString(CultureInfo.InvariantCulture)+": "+InputTranspositionInterval.Menu(n).ToString("+0;-0;0",CultureInfo.InvariantCulture));
      TransposeShow(TransposePrompt.Menu,String.Join("   ",menu)+Environment.NewLine+"Type number ->");return;
     }
     BeginMainTranspose(TransposeWord(text));return;
    case TransposePrompt.Menu:
     short choice=TransposeWord(text);if(choice<1||choice>16){ClearMainGroup();Message("");return;}
     BeginMainTranspose(InputTranspositionInterval.Menu(choice));return;
    case TransposePrompt.Clefs:
     if(answer=="Y")TransposeShow(TransposePrompt.ClefChoice,"Change all clefs to: 0=treble, 1=bass, 2=alto, 3=tenor.  ->");
     else TransposeAskKey();return;
    case TransposePrompt.ClefChoice:
     short clef=TransposeWord(text);if(clef<0&&!NativeEditCompatibility.ReproduceInvalidStates)throw new InvalidDataException("Negative clef table indices are prevented.");
     if(clef<=3){InputTranspositionOperation.ChangeClef(transposeOperation,clef);PublishTransposeRows();}
     TransposeAskKey();return;
    case TransposePrompt.Key:FinishMainTranspose(answer=="Y");return;
    case TransposePrompt.Warning:TransposeShow(TransposePrompt.Normalize,EnharmonicNormalizePrompt);return;
    case TransposePrompt.Normalize:
     if(answer=="N"){transposeParameterStart=null;ObserveDisplayTranspose(transposeDisplayStart);ClearMainGroup();transposeOperation=null;Message("");return;}
     var displayBefore=transposeDisplayStart;
     var displayBeforeNormalize=displayBefore==null&&transposeParameterStart==null?null:score.Clone();
     transposeDisplayStart=null;
     groupRange=new[]{transposeStaff,-10f,200f,0f,transposeStaff};groupCommand=EnharmonicNormalize;
     transposeOperation=null;int depth=undo.Count;CommitOriginalFlip("FLN");
     // Native undo treats the whole interactive command as one operation.
     // Keep its initial checkpoint rather than an extra pre-normalization one.
     if(transposeCheckpoint)while(undo.Count>depth)undo.Pop();
     bool retainedLastWord=ApplyOriginalTransposeTail(displayBeforeNormalize);
     RefreshTransposeTextTerminators();
     ObserveDisplayTranspose(displayBefore,displayBeforeNormalize,(int)transposeStaff,retainedLastWord);return;
   }
  }
  bool ApplyOriginalTransposeTail(Score before){
   var storage=transposeParameterStart;
   transposeParameterStart=null;
   if(storage==null||before==null||before.Items.Count==0||before.Items.Count!=score.Items.Count)return false;
   var last=score.Items.Last();
   if(!last.Bytes.SequenceEqual(before.Items.Last().Bytes)||
      before.Items.Zip(score.Items,(old,current)=>current.Count<old.Count).Any(shrunk=>shrunk))return false;
   int length=score.Items.Sum(item=>item.Bytes.Length);
   int previousLength=before.Items.Sum(item=>item.Bytes.Length);
   if(before.OriginalFilePayload==null||previousLength+20!=before.OriginalFilePayload.Length||length<=previousLength)return false;
   // The original backwards copy begins at the low half of the packed
   // bank's last word (13A9:01E8–0225 and 202B:0714–0735).
   var high=storage.ReadKnown(length-2,2);
   if(high==null)return false;
   var word=BitConverter.GetBytes(last.P(last.Count));
   if(high[0]==word[2]&&high[1]==word[3])return false;
   Array.Copy(high,0,word,2,2);
   var updated=last.Clone();
   updated.Set(updated.Count,BitConverter.ToSingle(word,0));
   bool tracked=parameterStorage.Matches(ParameterBytes(score));
   score.Items[score.Items.Count-1]=updated;
   var active=ParameterBytes(score);
   if(tracked)parameterStorage.Pack(active);
   else parameterStorage.Opaque(active);
   score.RetainedUndefinedWord=parameterStorage.UndefinedWord();
   RefreshAll(null);
   return true;
  }
  void RefreshTransposeTextTerminators(){
   // DOS redraw runs its text converter again after note reconstruction.
   // That reconstruction may replace a record whose count byte was touched
   // by the converter before normalization. Reapply the measured converter,
   // with the existing protection/compatibility policy, rather than patching
   // a saved-file offset or a particular item number.
   foreach(var word in score.Items.Where(item=>item.Code==16).ToArray())NativeTextTerminator.Refresh(score,word);
  }
  void BeginMainTranspose(short semitones){
   var request=new TranspositionRequest{Staff=transposeStaff,Left=-10,Right=200,Semitones=semitones,ForceKeyQuestion=groupCommand=="TRK"};
   transposeOperation=InputTranspositionOperation.Begin(score.Items.Select(TransposeRow).ToList(),request,transposeWorkspace);
   if(transposeOperation.Result.Collection.FirstPass.Records.Count==0){ClearMainGroup();transposeOperation=null;Message("");return;}
   TransposeShow(TransposePrompt.Clefs,TransposeClefsPrompt);
  }
 }
}
