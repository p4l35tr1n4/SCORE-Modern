using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Globalization;using System.Windows.Forms;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  enum LayoutPrompt { Staff, Range, Confirm, Duplicate, Text, Vertical, Groups, Output, Continue, Discard, FileName, FileCount, Part, Normalize }
  LayoutPrompt layoutPrompt;short layoutBottom,layoutTop;float layoutLeft,layoutRight;
  short layoutRequestedStaff,layoutRequestedTop;bool layoutTextCheckpoint;
  LayoutParentPreparation layoutPreparation;float[] layoutScales;
  List<string> layoutTranscript;
  readonly LayoutJustificationWorkspace layoutWorkspace=new LayoutJustificationWorkspace();
  LayoutLineupWorkspace lineupWorkspace=new LayoutLineupWorkspace();
  bool IsLayoutGroup {get{return groupCommand=="J"||groupCommand=="JT"||groupCommand=="VJ"||groupCommand=="DEC"||groupCommand=="COM"||groupCommand=="SEP"||groupCommand=="RIP"||groupCommand=="L"||groupCommand=="LI"||groupCommand=="LJ"||groupCommand=="RIP";}}
  bool IsLineupGroup {get{return groupCommand=="L"||groupCommand=="LI"||groupCommand=="LJ"||groupCommand=="RIP";}}
  bool ExecuteMainLayout(string verb,string arguments){
   if(verb!="J"&&verb!="JT"&&verb!="VJ"&&verb!="DEC"&&verb!="COM"&&verb!="SEP"&&verb!="RIP"&&verb!="L"&&verb!="LI"&&verb!="LJ")return false;
   if(LayoutNumbers(arguments).Length>20)throw new NotSupportedException("Original layout command argument buffer capacity exceeded.");
   if(verb=="RIP"){groupCommand=verb;rippleInput.Clear();layoutPrompt=LayoutPrompt.Staff;Message("How many staves in a system? (B=backup) ->");return true;}
   if(verb=="SEP"){separateWorking=null;groupCommand=verb;layoutPrompt=LayoutPrompt.Range;Message(SeparateRangePrompt);return true;}
   if(verb=="COM"){BeginMainCombine();return true;}
   if(verb=="DEC"){BeginMainDecombine();return true;}
   if(verb=="VJ"){groupCommand=verb;groupRange=null;layoutPrompt=LayoutPrompt.Vertical;Message(VerticalClearancePrompt);command.Focus();return true;}
   groupCommand=verb;groupRange=null;layoutTextCheckpoint=false;layoutPrompt=LayoutPrompt.Staff;Message(SystemSourcePrompt);command.Focus();return true;
  }
  // Original16D3:05EF..0799 accumulates integer digits, stores them as
  // binary32, then divides by a binary32 decimal power. Direct Float.Parse
  // can round differently. Minus signs toggle the stored sign; signed zero
  // is replaced by the measured -1 sentinel at0722..0732.
  static float OriginalCommandNumber(string token){
   if(token.Length==0)throw new InvalidDataException("Enter a SCORE number.");
   if(token.Length>11)throw new NotSupportedException("This numeric token exceeds the original eleven-character conversion domain.");
   int sign=1,dot=-1,start=token[0]=='+'?1:0;uint digits=0;bool any=false,afterMinus=false;
   for(int n=start;n<token.Length;n++){
    char c=token[n];if(c=='-'){sign=-sign;afterMinus=true;continue;}if(c==' '&&afterMinus)continue;afterMinus=false;if(c=='.'){dot=n;continue;}
    if(c<'0'||c>'9')throw new NotSupportedException("This numeric-token control still requires original verification.");
    any=true;digits=unchecked(digits*10+(uint)(c-'0'));
   }
   if(!any||digits>Int32.MaxValue)return 0;
   float power=dot<0?1:(float)Math.Pow(10,token.Length-dot-1);
   double number=(double)(float)digits*sign/power;
   if(number==0&&sign<0)return -1;
   return (float)number;
  }
  static float[] LayoutNumbers(string text,bool allSeparators=false){
   // Outer16D3:04AE..05E7 consumes one numeric terminator. A further
   // non-space separator can create an explicit zero parameter; splitting
   // and dropping empty fields loses that original positional control.
   var values=new System.Collections.Generic.List<float>();int state=-1;
   for(int at=0;at<text.Length;){char c=text[at];if(c==' '){state=1;at++;continue;}
    if((c>='0'&&c<='9')||c=='.'||c=='-'){
     int end=at;bool afterMinus=false;while(end<text.Length){char digit=text[end];if(digit=='-'){afterMinus=true;end++;continue;}if(digit==' '&&afterMinus){end++;continue;}if((digit>='0'&&digit<='9')||digit=='.'){afterMinus=false;end++;continue;}break;}
     if(!allSeparators&&end<text.Length&&text[end]!=' '&&text[end]!=','&&text[end]!='+'&&text[end]!='\t')throw new NotSupportedException("This numeric terminator still requires original verification.");values.Add(OriginalCommandNumber(text.Substring(at,end-at)));state=-1;at=Math.Min(text.Length,end+1);
    }else{if(!allSeparators&&c!=','&&c!='+'&&c!='\t')throw new NotSupportedException("This numeric argument separator still requires original verification.");if(state!=0)values.Add(0);if(allSeparators&&state>0)values.Add(0);state=0;at++;}
   }return values.ToArray();
  }
  void SubmitMainLayout(string text){
   if(groupCommand=="SEP"){SubmitMainSeparate(text);return;}
   if(groupCommand=="RIP"&&layoutPrompt!=LayoutPrompt.Duplicate){SubmitMainRipple(text);return;}
   if(groupCommand=="VJ"){SubmitMainVerticalJustification(text);return;}
   if(groupCommand=="DEC"){SubmitMainDecombine(text);return;}
   if(groupCommand=="COM"){SubmitMainCombine(text);return;}
   if(layoutPrompt==LayoutPrompt.Duplicate){
    string answer=text.Trim();if(!answer.Equals("Y",StringComparison.OrdinalIgnoreCase)&&!answer.Equals("N",StringComparison.OrdinalIgnoreCase)){LayoutDuplicateQuestion(true);return;}
    layoutTranscript.Add(LayoutDuplicateText()+"  "+answer.ToUpperInvariant());layoutPreparation.AnswerDuplicate(answer.Equals("Y",StringComparison.OrdinalIgnoreCase));ContinueMainJustification();return;
   }
   if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){if(layoutPrompt==LayoutPrompt.Text){PrepareMainTextJustification();ClearMainGroup();Message(score.Items.Count>0?"*** Sorting by staff number.":"");return;}if(layoutPrompt==LayoutPrompt.Range){layoutLeft=0;layoutRight=200;layoutPrompt=LayoutPrompt.Confirm;Message(GroupDeletePrompt);}else{ClearMainGroup();Message("");}return;}
   if(layoutPrompt==LayoutPrompt.Text){var values=LayoutNumbers(text);if(values.Length>20)throw new NotSupportedException("Original text-level argument capacity exceeded.");short staff=TransposeWord(values.Length>0?values[0].ToString("R",CultureInfo.InvariantCulture):"0");PrepareMainTextJustification();CommitMainTextJustification(staff,values.Length>1?values[1]:0);return;}
   if(layoutPrompt==LayoutPrompt.Staff){
    var values=LayoutNumbers(text);if(values.Length>20)throw new NotSupportedException("Original system argument buffer capacity exceeded.");
    short first=TransposeWord(values.Length>0?values[0].ToString("R",CultureInfo.InvariantCulture):"0"),count=TransposeWord(values.Length>1?values[1].ToString("R",CultureInfo.InvariantCulture):"0");
    layoutRequestedStaff=first;
    if(first>32){layoutBottom=1;layoutTop=32;layoutRequestedTop=0;}else{if(first==0)first=1;if(count==0)count=1;layoutBottom=first;layoutTop=unchecked((short)(first+count-1));layoutRequestedTop=layoutTop;}
    if(layoutBottom<1||layoutTop<layoutBottom)throw new NotSupportedException("Original layout staff traversal would leave its staff table.");
    if(IsLineupGroup){layoutLeft=0;layoutRight=200;layoutPrompt=LayoutPrompt.Confirm;Message(GroupDeletePrompt);}else{layoutPrompt=LayoutPrompt.Range;Message("Enter Left position, Right position. ->");}return;
   }
   if(layoutPrompt==LayoutPrompt.Range){
    var values=LayoutNumbers(text);if(values.Length>20)throw new NotSupportedException("Original range argument buffer capacity exceeded.");layoutLeft=values.Length>0?values[0]:0;layoutRight=values.Length>1?values[1]:0;
    if(BitConverter.ToInt32(BitConverter.GetBytes(layoutRight),0)==0)layoutRight=200;
    if(layoutRight<=layoutLeft){Message("Enter Left position, Right position. ->");return;}
    layoutPrompt=LayoutPrompt.Confirm;Message(GroupDeletePrompt);return;
   }
   if(!String.IsNullOrWhiteSpace(text))throw new NotSupportedException("Only the measured Enter/B layout confirmation is implemented.");
   if(groupCommand=="JT"){layoutPrompt=LayoutPrompt.Text;Message("Type text's staff # and P4 level of text (99=any level)");return;}
   CommitMainJustification();
  }
  void CommitMainJustification(){
   var scales=new PageCoordinates();
   foreach(var item in score.Items.Where(i=>i.Code==8)){
    var p=new float[Math.Max(21,item.Count+1)];for(int j=1;j<=item.Count;j++)p[j]=item.P(j);scales.SetStaff(p,NativeMeasurements.Get(score)==1);
   }
   layoutScales=scales.Scales;layoutTranscript=new List<string>();if(score.Items.Count>0)layoutTranscript.Add("*** Sorting by staff number.");layoutPreparation=new LayoutParentPreparation(score.Items.Select(TransposeRow).ToList(),layoutBottom,layoutTop,layoutLeft,layoutRight,false,IsLineupGroup);ContinueMainJustification();
  }
  string LayoutDuplicateText(){var row=layoutPreparation.Rows[layoutPreparation.DuplicateIndex];string position=((double)row[2]).ToString(".00",CultureInfo.InvariantCulture).PadLeft(6);return "*** Do you want to delete duplicate item?  Code# "+TransposeWord(row[0].ToString("R",CultureInfo.InvariantCulture))+", Staff# "+TransposeWord(row[1].ToString("R",CultureInfo.InvariantCulture))+", P3="+position;}
  void LayoutDuplicateQuestion(bool rejected){
   if(rejected)System.Media.SystemSounds.Beep.Play();
   Message(String.Join(Environment.NewLine,layoutTranscript.Concat(new[]{LayoutDuplicateText()})));command.Focus();
  }
  void ContinueMainJustification(){
   var prepared=layoutPreparation;
   if(!prepared.Complete){layoutPrompt=LayoutPrompt.Duplicate;LayoutDuplicateQuestion(false);return;}
   if(prepared.Rows.Count==0||prepared.Top<prepared.Bottom){ClearMainGroup();Message("");return;}
   var starts=new int[prepared.Rows.Count];var words=new float[16384];int at=0;
   for(int i=0;i<prepared.Rows.Count;i++){var row=prepared.Rows[i];if(at+row.Length+1>words.Length)throw new NotSupportedException("Original layout record bank capacity exceeded.");starts[i]=at+1;var original=(groupCommand=="RIP"?rippleWorking:score).Items[prepared.Origins[i]];words[at++]=original.Count==row.Length?BitConverter.ToSingle(original.Bytes,0):row.Length;Array.Copy(row,0,words,at,row.Length);at+=row.Length;}
   if(at+score.Trailer.Length/4<=words.Length)Buffer.BlockCopy(score.Trailer,0,words,at*4,score.Trailer.Length);
   var pref=NativePreferences.Read(source);var aft=new[]{pref.Number("AFTC",0),pref.Number("AFTB",0),pref.Number("AFTK",0),pref.Number("AFTM",0)};
   bool noRhythm=false;bool justifyAfter=!IsLineupGroup||groupCommand=="LJ"||groupCommand=="RIP";
   lineupWorkspace.Scor4DurationScale=scor4Interface;
   if(IsLineupGroup){var lineup=LayoutLineupSystem.Apply(words,starts,prepared.Bottom,prepared.Top,layoutScales,BitConverter.ToSingle(BitConverter.GetBytes(0x3f15c01au),0),32,2800,lineupWorkspace);foreach(var total in lineup.BeatTotals){string beats=SingleFixedDecimal.Format(total.Total,3);if(beats.StartsWith("0."))beats=beats.Substring(1);layoutTranscript.Add("Staff"+total.Staff.ToString(CultureInfo.InvariantCulture).PadLeft(3)+" has"+beats.PadLeft(7)+" beats");}noRhythm=lineup.Bottom<0;if(noRhythm)justifyAfter=false;prepared.Bottom=lineup.Bottom;prepared.Top=lineup.Top;prepared.Left=lineup.Left;prepared.Right=lineup.Right;}
   bool scor4LineupAndJustify=scor4Interface&&groupCommand=="LJ";
   if(scor4Interface&&IsLineupGroup&&(!justifyAfter||scor4LineupAndJustify)&&!noRhythm)
    RepositionScor4LineupText(prepared.Rows,words,starts,prepared.Bottom,prepared.Top);
   if(justifyAfter){layoutWorkspace.OriginalQuarterToneWidths=NativeEditCompatibility.OriginalQuarterToneWidths;var calculation=LayoutJustificationSystem.Apply(words,starts,prepared.Bottom,prepared.Top,prepared.Left,prepared.Right,layoutScales,aft,32,2800,layoutWorkspace);
    if(calculation.Gaps.CompressedMinimums){layoutTranscript.Add("*** Not enough space to justify ideally.");layoutTranscript.Add(((double)calculation.Gaps.MinimumTotal).ToString(".000",CultureInfo.InvariantCulture).PadLeft(8)+((double)calculation.Gaps.Width).ToString(".000",CultureInfo.InvariantCulture).PadLeft(9));}}
   var next=new List<Item>();for(int i=0;i<starts.Length;i++){var row=new float[prepared.Rows[i].Length];Array.Copy(words,starts[i],row,0,row.Length);var item=Item.Make(row);Buffer.BlockCopy(words,(starts[i]-1)*4,item.Bytes,0,4);next.Add(item);}
   if(groupCommand=="RIP"){ContinueRippleLineup(next);return;}
   // 2D31:0651/0657 schedules CW after the system core, before Main returns.
   var centered=score.Clone();centered.Items=next;var centerChanges=new List<NativeRestCenterChange>();
   // 8190 retains the raw staff word even when AFF4 normalizes zero to one.
   // 8192 retains the requested top rather than the later record-based clamp.
   if(justifyAfter){
    Func<int,List<NativeRestCenterChange>> center=staff=>scor4LineupAndJustify
     ?NativeRestCentering.PlanWholeRests(centered,staff):NativeRestCentering.Plan(centered,staff);
    if(layoutRequestedStaff>32)centerChanges.AddRange(center(33));
    else if(layoutRequestedStaff>0)for(int staff=layoutRequestedStaff;staff<=Math.Min(32,(int)layoutRequestedTop);staff++)centerChanges.AddRange(center(staff));
   }
   NativeRestCentering.Apply(centerChanges);
   // Lineup and justification redraw the retained display banks on the ordinary path.
   bool trackDisplay=(IsLineupGroup||groupCommand=="J")&&!noRhythm&&next.Count==score.Items.Count&&
    layoutRequestedStaff>=1&&layoutRequestedTop<=32&&CanTrackDisplayRedraw();
   string transcript=String.Join(Environment.NewLine,layoutTranscript);
   Checkpoint();score.Items=next;originalLayoutMarksModified|=!noRhythm||InputTranspositionOrder.MarksModified(next.Count,false);
   dirty=true;ClearMainGroup();RefreshTransposeTextTerminators();RefreshAll(null);
   if(trackDisplay)RebuildTrackedDisplay(centeredRests:centerChanges.Count==0?null:centerChanges.Select(change=>next.IndexOf(change.Item)));
   Message(transcript);
  }

  static void RepositionScor4LineupText(IList<float[]> rows,float[] words,int[] starts,short bottom,short top){
   foreach(var staff in Enumerable.Range(0,rows.Count).GroupBy(i=>(int)rows[i][1])){
    if(staff.Key<bottom||staff.Key>top)continue;
    var anchors=staff.Where(i=>IsScor4LineupTextAnchor((int)rows[i][0]))
     .GroupBy(i=>rows[i][2]).OrderBy(group=>group.Key).Select(group=>group.First()).ToArray();
    int right=1;
    foreach(int index in staff.Where(i=>rows[i][0]==16).OrderBy(i=>rows[i][2])){
     float source=rows[index][2];
     while(right<anchors.Length&&source>rows[anchors[right]][2])right++;
     if(right>=anchors.Length)break;
     int a=anchors[right-1],b=anchors[right];
     float oldLeft=rows[a][2],oldRight=rows[b][2];
     if(source<oldLeft)continue;
     double left=words[starts[a]+2],rightPosition=words[starts[b]+2];
     words[starts[index]+2]=(float)(left+((double)source-oldLeft)*(rightPosition-left)/((double)oldRight-oldLeft));
    }
   }
  }
  static bool IsScor4LineupTextAnchor(int code){return code>=1&&code<=3||code==14||code==17||code==18;}
 }
}
