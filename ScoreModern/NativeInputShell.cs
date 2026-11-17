using System;using System.IO;using System.Text;using System.Linq;using System.Collections.Generic;using System.Globalization;using System.Drawing;using System.Windows.Forms;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  sealed class InputSession {
   public readonly List<string> PendingPhysical=new List<string>();public readonly List<List<string>> PhysicalGroups=new List<List<string>>();public InputMacroContext ContinuationBaseline;public int PendingRhythms;public string MotiveReplacementLine;public char MotiveReplacementName;public InputMacroContext MotiveBaseline;public string LetterLaunchNotice;public bool LetterEditInput,LetterInput,PositionsQuestion,LetterCheckpoint;public float LetterLeft,LetterRight;public InputStaffTransaction Transaction;public List<Item> Prefix;public readonly List<string> Lines=new List<string>();public bool SaveQuestion,AbandonQuestion,FilenameQuestion,SpacingPause,ReplayedSpacingWarning;public int Correction=-1;public string RejectedLine,InputError;public List<string> CorrectionLines;
   public string CorrectionHeader,AcceptedHeader,CorrectionPositions,AcceptedPositions,LastInputPhysical;
   public int CorrectedStaff;
   public bool TrackDisplay;
   public readonly List<KeyValuePair<int,float>> ReferenceDisplayPositions=new List<KeyValuePair<int,float>>();
   public bool ProximityAfterError,PitchProximity;
   public string BeamMenuEcho,MidiNotice;public MidiInputLine MidiLine;public int? MouseClef;
  }
  sealed class ShortRhythmList : ArgumentException {
   public ShortRhythmList(int entered,int needed):base(String.Format(CultureInfo.InvariantCulture,
    "*** Mismatch - you typed {0,2} rhythms. {1,2} rhythms needed.",entered,needed)){}
  }
  InputSession musicInput;InputSetupState inputSetup=new InputSetupState();Score inputPresetBackup;
  static readonly string[] setupLabels={"TREBLE","BASS","TENOR","ALTO","PIANO","VOCAL","QUART","","",""};
  static readonly string[] scor4TripletRhythms={"1.5","3","6","12","24","48","96"};
  static readonly string[] scor4DupleRhythms={"1","2","4","8","16","32","64"};
  static string InputNumber(float n){int whole=(int)n,part=(int)Math.Round(((double)n-whole)*100,MidpointRounding.AwayFromZero);return whole.ToString(CultureInfo.InvariantCulture)+(part==0?"":"."+part.ToString(CultureInfo.InvariantCulture));}
  static float[] InputParameters(Item item){var values=new float[item.Count];for(int i=0;i<values.Length;i++)values[i]=item.P(i+1);return values;}
  string InputHeader(){return musicInput.AcceptedHeader??("IN"+inputSetup.Staff+" "+inputSetup.LeftEnd+" "+inputSetup.Vertical+" "+InputNumber(inputSetup.Size)+" "+inputSetup.RightEnd);}
  string InputPositions(){return musicInput.AcceptedPositions??(inputSetup.Spacing>0
   ?"SP"+inputSetup.Spacing.ToString(CultureInfo.InvariantCulture)
   :inputSetup.LeftPosition+" "+inputSetup.RightPosition);}
  void StartMusicInput(){ExitEdit(false);((NativeCommandBox)command).CaptureVerticalNavigation=true;musicInput=new InputSession();inputPresetBackup=null;pendingScor4Preset=null;scor4SetupSecond=false;mode=ScreenMode.Input;canvas.Selected=null;canvas.BlockItemEditing=true;command.Clear();FindInputBounds();UpdateMusicInput();LayoutClassic();command.Focus();}
  void FindInputBounds(){var bounds=InputSetupBounds.Find(score.Items.Select(InputParameters).ToList(),inputSetup);inputSetup.LeftPosition=bounds.Left;inputSetup.RightPosition=bounds.Right;inputSetup.Key=bounds.Key;inputSetup.MeterTop=bounds.MeterTop;inputSetup.MeterBottom=bounds.MeterBottom;}
  void LeaveMusicInput(){CloseMidiInput();CloseInputFieldEditor();((NativeCommandBox)command).CaptureVerticalNavigation=false;musicInput=null;((NativeCommandBox)command).ScoreEditing=false;UpdateInputNumbers();UpdateCorrectionField();inputPresetBackup=null;pendingScor4Preset=null;scor4SetupSecond=false;canvas.BlockItemEditing=false;mode=ScreenMode.Main;command.Clear();Message("");UpdateClassic();LayoutClassic();command.Focus();}
  void CommitInputField(){
   if(command.Text.Length==0)return;
   ApplyInputField(command.Text);
   command.Clear();
   UpdateMusicInput();
  }
  void ApplyInputField(string text){
   text=text.Trim().ToUpperInvariant();
   if(inputSetup.Selected==11)inputSetup.KeyText(text);
   else if(inputSetup.Selected==6&&(text=="P"||text=="R"))inputSetup.SpacingLetter(text[0]);
   else{
    float value;
    if(!Single.TryParse(text,NumberStyles.AllowLeadingSign|NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out value))
     throw new InvalidDataException("Enter a SCORE setup value.");
    if(inputSetup.Numeric(value))FindInputBounds();
   }
  }
  void LoadInputPreset(int scan){command.Clear();string name=InputSetupState.PresetName(scan),path=NativeLibraryLocation.FileFor(source,name);CommitInputPreset(PlaceInputPreset(Score.Load(path)));}
  List<Item> PlaceInputPreset(Score preset){var append=new List<Item>();foreach(var record in preset.Items){var item=record.Clone();item.Set(2,(float)((double)item.P(2)+inputSetup.Staff-1));if(item.Count<3)item.Set(3,0);
    // Original overlay-one section 5, 0637..0687 and 0833..0A42.
    // Setup placement is the GM transform, including its two-sided P3 anchor.
    if(item.Code==8){
     if(inputSetup.LeftEnd!=0)item.Set(3,inputSetup.LeftEnd);
     if(inputSetup.Vertical!=0)item.Set(4,(float)((double)item.P(4)+inputSetup.Vertical));
     if(inputSetup.Size!=1)item.Set(5,inputSetup.Size);
     if(inputSetup.RightEnd!=200)item.Set(6,inputSetup.RightEnd);
    }else if(item.P(3)>100){if(inputSetup.RightEnd!=200)item.Set(3,(float)((double)item.P(3)+inputSetup.RightEnd-200));}
    else if(inputSetup.LeftEnd!=0)item.Set(3,(float)((double)item.P(3)+inputSetup.LeftEnd));
    if(item.Staff<1||item.Staff>32)throw new NotSupportedException("Preset exceeds the verified SCORE staff range.");append.Add(item);}return append;}
  void CommitInputPreset(List<Item> items){Checkpoint();inputPresetBackup=score.Clone();score.Items.AddRange(items);if(scor4Interface)RememberScor4PresetClefs(items);dirty=true;FindInputBounds();RefreshAll(null);UpdateMusicInput();}
  void UndoInputPreset(){if(inputPresetBackup==null)return;score.Items=inputPresetBackup.Items.Select(i=>i.Clone()).ToList();inputPresetBackup=null;FindInputBounds();RefreshAll(null);UpdateMusicInput();}
  void BeginInputStaff(){scor4PitchBank=Scor4PitchBank.Main;scor4RhythmSecond=false;scor4MarksBank=0;musicInput.MouseClef=null;((NativeCommandBox)command).ScoreInsert=false;CommitInputField();musicInput.MotiveBaseline=inputMotives.Clone();if(inputSetup.Size<0)throw new NotSupportedException("Negative input staff size is prevented.");
   var prefs=NativePreferences.Read(source);
   float beam=(float)((double)prefs.Number("BEAM",.5f)/100),dynamics=prefs.Number("DYNAMICS",0);
   short ties;
   try{ties=prefs.Word("TIES",1);}
   catch(NotSupportedException error){
    throw new NotSupportedException("F2 input cannot use TIES "+prefs.Text("TIES")+" in "+prefs.Path+". Set TIES to an integer in the preferences editor. The supplied SCORE 3.11 LIB uses 1.",error);
   }
   // Accepting the next F2 staff redraws the earlier normalized reference
   // positions even when no intervening Z was issued (original case 1715).
   bool trackedSetup=CanTrackDisplayRedraw()&&
    (!physicalDisplayReferenceRedrawPending||!musicInput.LetterInput&&inputSetup.Spacing>0);
   InputReferenceSpacing reference=null;
   if(inputSetup.Spacing>0){
    if(!musicInput.LetterInput){
     if(!musicInput.LetterCheckpoint){Checkpoint();musicInput.LetterCheckpoint=true;}
     var order=InputTranspositionOrder.OrderForMain(score.Items.Select(InputParameters).ToList(),false);
     score.Items=order.Select(index=>score.Items[index]).ToList();dirty=true;canvas.LayoutScore();
    }
    if(inputSetup.Spacing==inputSetup.Staff)throw new InvalidDataException("Choose a different spacing staff.");
    var staffRecord=score.Items.FirstOrDefault(i=>i.Code==8&&i.Staff==inputSetup.Spacing);
    if(staffRecord==null)throw new InvalidDataException("The spacing staff must exist before input.");
    float referenceScale=canvas.PageDocument.Tables.Scales[inputSetup.Spacing]/25f;
    var existingStaff=score.Items.FirstOrDefault(i=>i.Code==8&&i.Staff==inputSetup.Staff);
    float inputScale=existingStaff==null?(inputSetup.Size==0?1:inputSetup.Size):canvas.PageDocument.Tables.Scales[inputSetup.Staff]/25f;
    if(!(referenceScale>0&&inputScale>0)||Single.IsInfinity(referenceScale)||Single.IsInfinity(inputScale))
     throw new NotSupportedException("Reference spacing requires positive finite staff sizes.");
    var referenceItems=score.Items.Where(i=>i.Staff==inputSetup.Spacing).ToList();
    var referenceRows=referenceItems.Select(InputParameters).ToList();
    var normalized=new List<KeyValuePair<int,float>>();
    reference=new InputReferenceSpacing(referenceRows,(i,x)=>normalized.Add(new KeyValuePair<int,float>(i,x)));
    foreach(var change in normalized){
     var original=referenceItems[change.Key];
     musicInput.ReferenceDisplayPositions.Add(new KeyValuePair<int,float>(score.Items.IndexOf(original),original.P(3)));
     original.Set(3,change.Value);
    }
   }
   if(musicInput.LetterInput)
    musicInput.TrackDisplay=musicInput.TrackDisplay&&!musicInput.LetterEditInput&&
     musicInput.LetterLeft==0&&musicInput.LetterRight==200&&
     (inputSetup.Spacing==-1&&trackedSetup||reference!=null&&DisplayResourcesMatch());
   else
    musicInput.TrackDisplay=!score.Items.Any(i=>i.Code==8&&i.Staff==inputSetup.Staff)&&
     (inputSetup.Spacing==-1||reference!=null)&&inputSetup.Size>0&&inputSetup.LeftEnd>=0&&inputSetup.Vertical==0&&
     inputSetup.RightEnd==200&&inputSetup.LeftPosition==inputSetup.LeftEnd&&inputSetup.RightPosition==200&&trackedSetup;
   if(!musicInput.TrackDisplay)InvalidateDisplayStorage();
   if(!musicInput.LetterCheckpoint)Checkpoint();if(!score.Items.Any(i=>i.Code==8&&i.Staff==inputSetup.Staff))score.Items.Add(Item.Make(inputSetup.StaffRecord()));
   float inputBeginning=Math.Max(0,musicInput.LetterInput?musicInput.LetterLeft:inputSetup.LeftPosition);
   if(!musicInput.LetterInput){
    // The setup caller snapshots geometry before 10D0:0CF7 formats its
    // signed words by reference. The header retains the minus sign while
    // the pending setup word is replaced with its magnitude (0D1C..0D23).
    musicInput.AcceptedHeader=InputHeader();
    musicInput.AcceptedPositions=InputPositions();
    if(inputSetup.LeftEnd<0)inputSetup.LeftEnd=-inputSetup.LeftEnd;
    if(inputSetup.Vertical<0)inputSetup.Vertical=-inputSetup.Vertical;
    if(inputSetup.Spacing<=0&&inputSetup.LeftPosition<0)inputSetup.LeftPosition=-inputSetup.LeftPosition;
   }
   musicInput.Prefix=score.Items.Select(i=>i.Clone()).ToList();musicInput.Lines.Clear();canvas.LayoutScore();var bounds=InputSetupBounds.Find(musicInput.Prefix.Select(InputParameters).ToList(),inputSetup);float scale=canvas.PageDocument.Tables.Scales[inputSetup.Staff]/25f,following=inputSetup.Staff<32?canvas.PageDocument.Tables.Scales[inputSetup.Staff+1]:25f;musicInput.Transaction=new InputStaffTransaction(inputSetup.Staff,bounds.Clef,inputBeginning,musicInput.LetterInput?musicInput.LetterRight:inputSetup.RightPosition,scale,following,beam,dynamics,ties,inputSetup.Spacing==0,0.5849624872207642f);musicInput.Transaction.UseStaffCoordinates(new InputEndpointStaffContext(canvas.PageDocument.Tables.Bases,canvas.PageDocument.Tables.Scales,inputSetup.Staff,scor4Interface));if(reference!=null)musicInput.Transaction.UseReferenceSpacing(reference);inputPresetBackup=null;command.Clear();dirty=true;RefreshAll(null);UpdateMusicInput();ObserveInputDisplay(true);OpenMidiInput(prefs);}
  InputPitchState InputPitchDefaults(){string[] keys={"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#"};return new InputPitchState{KeySignature=Array.IndexOf(keys,inputSetup.Key)-7,MeterTop=inputSetup.MeterTop,MeterBottom=inputSetup.MeterBottom};}
  void ApplyInputStage(InputStaffTransaction tx,string text,InputMacroContext motives=null){var motiveTarget=motives??inputMotives;var trialMotives=motiveTarget.Clone();switch(tx.Stage){case InputStaffStage.Pitches:
   var pitchState=InputPitchDefaults();
   pitchState.Proximity=musicInput.PitchProximity;
   tx.Pitches(InputTextLists.Pitches(text,pitchState,trialMotives),true,NativeEditCompatibility.AllowLargeInput,true);
   tx.ObservePitchPreviewLoader();
   if(ReferenceEquals(tx,musicInput.Transaction))musicInput.PitchProximity=pitchState.Proximity;
   break;
   case InputStaffStage.Rhythms:
   if(text.EndsWith(";",StringComparison.Ordinal)&&String.IsNullOrWhiteSpace(text.Substring(0,text.Length-1))){
    tx.SkipRhythms();
    break;
   }
   var rhythms=InputTextLists.Rhythms(text,trialMotives,NativeEditCompatibility.ReproduceInvalidStates);
   if(rhythms.Length<tx.RhythmCount)throw new ShortRhythmList(rhythms.Length,tx.RhythmCount);
   tx.Rhythms(rhythms,NativeEditCompatibility.ReproduceInvalidStates);break;
   case InputStaffStage.Marks:var marks=InputMarkText.Parse(text,trialMotives,NativeEditCompatibility.RejectMultiDigitF);tx.Marks(marks.Values,marks.Opposite,NativeEditCompatibility.ReproduceInvalidStates);break;case InputStaffStage.Beams:
    int meterTop=inputSetup.MeterTop,meterBottom=inputSetup.MeterBottom;
    tx.UpdateInputMeter(ref meterTop,ref meterBottom);
    tx.Beams(InputEndpointText.Parse(text,true,meterTop,meterBottom,trialMotives));
    break;case InputStaffStage.Slurs:tx.Slurs(InputEndpointText.Parse(text,false,inputSetup.MeterTop,inputSetup.MeterBottom,trialMotives).Pairs);break;default:throw new InvalidOperationException("Input staff already complete.");}trialMotives.ClearReplacementAllowances();motiveTarget.ReplaceFrom(trialMotives);}
  void PaintInputPreview(){var values=musicInput.Transaction.Preview(musicInput.Prefix.Select(InputParameters).ToList());score.Items=values.Select(p=>Item.Make(p)).ToList();dirty=true;RefreshAll(null);}
  void NextInputStaff(){if(musicInput!=null&&musicInput.LetterInput){LeaveMusicInput();ResumeOriginalCommandFileCorrection();return;}int staff=musicInput.CorrectedStaff==0?inputSetup.Staff:musicInput.CorrectedStaff;musicInput=new InputSession();inputSetup.Staff=Math.Min(32,staff+1);inputSetup.Selected=1;inputPresetBackup=null;FindInputBounds();command.Clear();UpdateMusicInput();}
  void SubmitMusicInput(string text){((NativeCommandBox)command).ScoreInsert=false;if(musicInput.MotiveReplacementLine!=null){AcknowledgeInputMotiveReplacement();return;}if(SubmitOriginalInputPositions(text))return;if(musicInput.SpacingPause){AcknowledgeSpacingWarning();return;}string rawPhysical=text;text=text.Trim();if(musicInput.Transaction==null){command.Text=text;CommitInputField();return;}if(musicInput.AbandonQuestion)return;if(musicInput.FilenameQuestion){SaveInputTranscript(text);return;}if(musicInput.Correction>=0){SubmitInputCorrection(text);return;}if(musicInput.SaveQuestion){if(text.Equals("N",StringComparison.OrdinalIgnoreCase)){NextInputStaff();return;}if(text.Equals("Y",StringComparison.OrdinalIgnoreCase)){if(NativeDiskDialogChoice.Files&&originalCommandFileQueue==null){string chosen=ChooseCommandFile("Save staff input",true,TextFileFilter,"",false);if(chosen!=null)SaveInputTranscript(chosen);}else{musicInput.FilenameQuestion=true;command.Clear();UpdateMusicInput();}}return;}
   musicInput.LastInputPhysical=rawPhysical;
   bool rhythms=musicInput.Transaction.Stage==InputStaffStage.Rhythms;
   string physical=text;
   bool continued=IsInputContinuation(text,musicInput.Transaction.Stage==InputStaffStage.Pitches);
   if(musicInput.ContinuationBaseline==null)musicInput.ContinuationBaseline=inputMotives.Clone();
   text=InputPhysicalPrefix()+text;
   try{
    if(continued){ParseInputContinuation(text);musicInput.PendingPhysical.Add(physical);musicInput.RejectedLine=musicInput.InputError=null;command.Clear();UpdateMusicInput();return;}
    ApplyInputTranscriptLine(musicInput.Transaction,text,musicInput.ContinuationBaseline);
    inputMotives.ReplaceFrom(musicInput.ContinuationBaseline);
   }
   catch(InputEndpointRepromptException){musicInput.InputError=musicInput.RejectedLine=null;command.Clear();UpdateMusicInput();return;}
   catch(InputAutomaticBeamMeterException error){RejectInputLine(physical,error.Message);return;}
   catch(InputMotiveReplacementException error){musicInput.MotiveReplacementLine=physical;musicInput.MotiveReplacementName=error.Name;command.Clear();UpdateMusicInput();return;}
   catch(ShortRhythmList error){RejectInputLine(physical,error.Message);return;}
   catch(InputProximityDelimiterException error){
    bool simple=text.IndexOf('[')<0;
    if(simple)musicInput.ProximityAfterError=musicInput.PitchProximity=true;
    RejectInputLine(physical,null,simple?(int?)error.Caret:null);return;
   }
   catch(FormatException){RejectInputLine(physical,null);return;}
   catch(ArgumentException){RejectInputLine(physical,null);return;}
   catch(InvalidDataException){RejectInputLine(physical,null);return;}
   catch(NotSupportedException error){RejectInputLine(physical,error.Message);return;}
   musicInput.RejectedLine=musicInput.InputError=null;
   musicInput.Lines.Add(text);
   var physicalGroup=new List<string>(musicInput.PendingPhysical);physicalGroup.Add(physical);musicInput.PhysicalGroups.Add(physicalGroup);
   musicInput.PendingPhysical.Clear();musicInput.PendingRhythms=0;musicInput.ContinuationBaseline=null;
   if(rhythms&&musicInput.Transaction.ReferenceSpacingWarning!=null){
    musicInput.SpacingPause=true;
    command.Clear();
    UpdateMusicInput();
    return;
   }
   PaintInputPreview();
   musicInput.SaveQuestion=musicInput.Transaction.Stage==InputStaffStage.Complete;
   command.Clear();
   UpdateMusicInput();
   ObserveInputDisplay(false);
  }
  void AcknowledgeInputMotiveReplacement(){string line=musicInput.MotiveReplacementLine;inputMotives.AllowReplacement(musicInput.MotiveReplacementName);if(musicInput.ContinuationBaseline!=null)musicInput.ContinuationBaseline.AllowReplacement(musicInput.MotiveReplacementName);musicInput.MotiveReplacementLine=null;SubmitMusicInput(line);}
  void AcknowledgeSpacingWarning(){
   musicInput.SpacingPause=false;
   PaintInputPreview();
   UpdateMusicInput();
   ObserveInputDisplay(false);
   command.Focus();
  }
  void ApplyInputTranscriptLine(InputStaffTransaction tx,string text,InputMacroContext motives=null){
   if(text.Equals("G",StringComparison.OrdinalIgnoreCase)&&tx.Stage>=InputStaffStage.Marks){
    while(tx.Stage!=InputStaffStage.Complete)ApplyInputStage(tx,";",motives);
   }else ApplyInputStage(tx,text,motives);
  }
  void RejectInputLine(string text,string unsupported,int? caret=null){
   // A single staccato token missing its separator is rejected before DOS
   // touches its display buffers. Other rejection histories are unverified.
   bool rejectedCompactStaccato=musicInput.TrackDisplay&&musicInput.Transaction!=null&&
    unsupported==null&&musicInput.Transaction.Stage==InputStaffStage.Marks&&
    musicInput.PendingPhysical.Count==0&&text.Equals("S1;",StringComparison.OrdinalIgnoreCase);
   if(musicInput.TrackDisplay&&!rejectedCompactStaccato){musicInput.TrackDisplay=false;InvalidateDisplayStorage();}
   ((NativeCommandBox)command).ScoreInsert=false;
   musicInput.RejectedLine=text;
   musicInput.InputError=unsupported??"*** Syntax Error. Correct line then press <Enter>. Press <ESC> to Exit.";
   command.Text=text;
   command.SelectionStart=caret??command.TextLength;
   UpdateMusicInput();
   command.Focus();
  }
  byte[] EncodeInputTranscript(){var lines=new List<string>{InputHeader(),InputPositions()};lines.AddRange(InputPhysicalRows().Select(line=>line.Trim().ToUpperInvariant()));return Encoding.GetEncoding(437).GetBytes(String.Join("\r\n",lines.ToArray())+"\r\n");}
  void SaveInputTranscript(string name){if(String.IsNullOrWhiteSpace(name))throw new InvalidDataException("Enter an input-file name.");string path=NativeOutputLocation.Resolve(source,name);if(File.Exists(path)&&MessageBox.Show(this,"Write over "+path+"?","SCORE input file",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;ScoreFileStorage.Write(path,EncodeInputTranscript());NextInputStaff();}
  void StartInputCorrection(){
   musicInput.ReplayedSpacingWarning=false;
   musicInput.AbandonQuestion=false;
   musicInput.Correction=0;
   musicInput.CorrectionLines=new List<string>(InputPhysicalRows());
   musicInput.CorrectionHeader=InputHeader();
   musicInput.CorrectionPositions=InputPositions();
   SetInputCorrectionLine();
  }
  void SubmitInputCorrection(string text){
   int index=musicInput.Correction;
   var lines=musicInput.CorrectionLines;
   if(text.Equals("B",StringComparison.OrdinalIgnoreCase)){
    if(index>0){musicInput.Correction=0;SetInputCorrectionLine();}
    else NextInputStaff(); // 2FBA:0514 writes Y; caller accepts the current staff.
    return;
   }
   string current=CorrectionValue(index);
   if(text.Equals("G",StringComparison.OrdinalIgnoreCase)){
    ReplayInputCorrection();
    return;
   }
   // 2FBA:0590..060D. Near the tail SCORE truncates the last
   // physical record; earlier deletion shifts records and restarts at Pos.1.
   if(text.Equals("D",StringComparison.OrdinalIgnoreCase)){
    if(index==0)return;
    if(index>=lines.Count){
     if(lines.Count==0){RejectInputLine(text,"The input list has no music record to truncate.");return;}
     lines.RemoveAt(lines.Count-1);ReplayInputCorrection();return;
    }
    lines.RemoveAt(Math.Max(0,index-2));musicInput.CorrectionPositions="";
    musicInput.Correction=1;SetInputCorrectionLine();return;
   }
   // 2FBA:051D..058E duplicates the current physical record into
   // the following slot, retaining the cursor and the entered I on screen.
   if(text.Equals("I",StringComparison.OrdinalIgnoreCase)){
    if(lines.Count+2<25){
     var rows=new List<string>{musicInput.CorrectionHeader,musicInput.CorrectionPositions};rows.AddRange(lines);
     rows.Insert(index,rows[index]);musicInput.CorrectionHeader=rows[0];musicInput.CorrectionPositions=rows[1];
     lines.Clear();lines.AddRange(rows.Skip(2));
    }
    command.Text=text;command.SelectionStart=command.TextLength;UpdateMusicInput();return;
   }
   if(text.Length==0)text=current;
   if(index==0&&text!=current){
    if(!TryCorrectionHeader(text,current)){
     RejectInputLine(text,"This change to the IN setup row is outside the verified correction cases.");
     return;
    }
    musicInput.CorrectionHeader=text.ToUpperInvariant();
   }
   if(index==1&&text!=current&&musicInput.LetterEditInput){float letterLeft,letterRight;bool proportional;if(!TryLetterCorrectionPositions(text,out letterLeft,out letterRight,out proportional)){RejectInputLine(text,"Enter two increasing positions, optionally followed by 1 for proportional spacing.");return;}musicInput.CorrectionPositions=text;
   }else if(index==1&&text!=current){
    int left,right;
    if(inputSetup.Spacing!=-1||!TryCorrectionPositions(text,out left,out right)){
     RejectInputLine(text,"Enter two increasing whole positions within the staff bounds for rhythmic spacing.");
     return;
    }
    musicInput.CorrectionPositions=left+" "+right;
   }
   if(index>=2)lines[index-2]=text;
   if(index+1<2+lines.Count){musicInput.Correction++;SetInputCorrectionLine();return;}
   ReplayInputCorrection();
  }

  bool TryCorrectionHeader(string text,string current){
   int letterStaff;if(musicInput.LetterEditInput&&TryLetterCorrectionStaff(text,out letterStaff))return true;
   InputSetupState destination;
   if(TryCorrectionDestination(text,out destination))return true;
   var fields=text.Split(' ');
   var previous=current.Split(' ');
   if(fields.Length!=5||previous.Length!=5
    ||!fields[0].Equals(previous[0],StringComparison.OrdinalIgnoreCase)
    ||fields[1]!=previous[1]||fields[2]!=previous[2])return false;
   if(inputSetup.Spacing==-1&&fields[3]==previous[3]){
    int right;
    return Int32.TryParse(fields[4],NumberStyles.None,CultureInfo.InvariantCulture,out right)
     &&right>inputSetup.LeftEnd&&right>=1&&right<=inputSetup.RightEnd;
   }
   if(inputSetup.Spacing>0&&fields[4]==previous[4]){
    float size;
    return Single.TryParse(fields[3],NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out size)
     &&size>0&&size<=inputSetup.Size;
   }
   return false;
  }
  bool TryCorrectionDestination(string text,out InputSetupState destination){
   destination=null;
   if(inputSetup.Spacing!=-1)return false;
   var fields=text.Split(' ');
   int staff,left,vertical,right;float size;
   if(fields.Length!=5||!fields[0].StartsWith("IN",StringComparison.OrdinalIgnoreCase)
    ||!Int32.TryParse(fields[0].Substring(2),NumberStyles.None,CultureInfo.InvariantCulture,out staff)
    ||staff<1||staff>32||staff==inputSetup.Staff
    ||!Int32.TryParse(fields[1],NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out left)
    ||!Int32.TryParse(fields[2],NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out vertical)
    ||!Single.TryParse(fields[3],NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out size)
    ||!Int32.TryParse(fields[4],NumberStyles.None,CultureInfo.InvariantCulture,out right)
    ||left< -99||left>199||vertical< -99||vertical>99||!(size>0&&size<10)
    ||right<1||right>216||right<=left)return false;
   destination=new InputSetupState{Staff=staff,LeftEnd=left,Vertical=vertical,Size=size,RightEnd=right};
   return true;
  }

  bool TryCorrectionPositions(string text,out int left,out int right){
   left=right=0;
   var fields=text.Split(new[]{' ','\t'},StringSplitOptions.RemoveEmptyEntries);
   return fields.Length==2&&Int32.TryParse(fields[0],NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out left)
    &&Int32.TryParse(fields[1],NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out right)
    &&left>=inputSetup.LeftEnd&&right<=inputSetup.RightEnd&&left<right;
  }

  void ReplayInputCorrection(int? retainedCount=null){
   var physicalGroups=GroupInputRows(musicInput.CorrectionLines,true);
   var pendingPhysical=new List<string>();
   if(physicalGroups.Count>0&&IsInputContinuation(physicalGroups.Last().Last(),physicalGroups.Count==1)){
    pendingPhysical=physicalGroups.Last();physicalGroups.RemoveAt(physicalGroups.Count-1);
   }
   var lines=physicalGroups.Select(group=>String.Concat(group.ToArray())).ToList();
   if(retainedCount.HasValue)lines=lines.Take(retainedCount.Value).ToList();
   // Keep the visible, accepted staff until the complete correction replays.
   int left,right;float letterLeft,letterRight;bool letterProportional;
   var candidate=musicInput.Transaction.Restart();
   if(musicInput.LetterEditInput&&TryLetterCorrectionPositions(musicInput.CorrectionPositions,out letterLeft,out letterRight,out letterProportional))candidate=musicInput.Transaction.Restart(letterLeft,letterRight,letterProportional);
   else if(musicInput.CorrectionPositions!=InputPositions()&&TryCorrectionPositions(musicInput.CorrectionPositions,out left,out right))candidate=musicInput.Transaction.Restart(left,right);
   var prefixItems=musicInput.Prefix;
   InputSetupState destination=null;int letterStaff;
   if(musicInput.LetterEditInput&&TryLetterCorrectionStaff(musicInput.CorrectionHeader,out letterStaff)&&letterStaff!=musicInput.Transaction.OriginalStaff){
    if(!TryLetterCorrectionPositions(musicInput.CorrectionPositions,out letterLeft,out letterRight,out letterProportional))throw new NotSupportedException("EDI destination correction requires a valid position row.");
    var existing=prefixItems.FirstOrDefault(item=>item.Staff==letterStaff&&item.Code==8);destination=new InputSetupState{Staff=letterStaff,LeftEnd=existing==null?0:(int)existing.P(3),Vertical=existing==null?0:(int)existing.P(4),Size=existing==null?1:canvas.PageDocument.Tables.Scales[letterStaff]/25f,RightEnd=existing==null||existing.P(6)==0?200:(int)existing.P(6)};
    if(existing==null)prefixItems=new List<Item>(prefixItems){Item.Make(8,letterStaff,0,0,0,0)};
    int clef=existing==null?0:InputSetupBounds.Find(prefixItems.Select(InputParameters).ToList(),destination).Clef;float following=letterStaff<32?canvas.PageDocument.Tables.Scales[letterStaff+1]:25f;
    candidate=musicInput.Transaction.RestartOnStaffRange(letterStaff,clef,destination.Size,following,letterLeft,letterRight,letterProportional);
   }else if(TryCorrectionDestination(musicInput.CorrectionHeader,out destination)){
    if(!TryCorrectionPositions(musicInput.CorrectionPositions,out left,out right)){
     musicInput.Correction=1;
     RejectInputLine(musicInput.CorrectionPositions,"Enter two increasing whole positions within the staff bounds for rhythmic spacing.");
     return;
    }
    var existing=prefixItems.FirstOrDefault(item=>item.Staff==destination.Staff&&item.Code==8);
    if(existing==null)prefixItems=new List<Item>(prefixItems){
     Item.Make(new float[]{8,destination.Staff,destination.LeftEnd,destination.Vertical,destination.Size,destination.RightEnd})
    };
    float scale=existing==null?(float)((double)destination.Size*25)/25f:canvas.PageDocument.Tables.Scales[destination.Staff]/25f;
    int clef=existing==null?0:InputSetupBounds.Find(prefixItems.Select(InputParameters).ToList(),destination).Clef;
    float next=destination.Staff==32?25f:canvas.PageDocument.Tables.Scales[destination.Staff+1];
    candidate=musicInput.Transaction.RestartOnStaffRange(destination.Staff,clef,scale,next,left,right);
   }
   var prefix=prefixItems.Select(InputParameters).ToList();
   var candidateMotives=(musicInput.MotiveBaseline??inputMotives).Clone();
   List<float[]> beforeRhythms=null;
   var displayReplay=musicInput.TrackDisplay?new List<List<float[]>>():null;
   for(int n=0;n<lines.Count;n++){
    try{
     if(n==1&&lines.Count==2&&pendingPhysical.Count==0)beforeRhythms=candidate.Preview(prefix);
     ApplyInputTranscriptLine(candidate,lines[n],candidateMotives);
     if(displayReplay!=null&&n<=1)displayReplay.Add(candidate.Preview(prefix));
     // A replay ending with rhythms waits before painting them. If more
     // recorded input follows (including G), SCORE prints the mismatch and
     // replays that input without another physical-key acknowledgment.
     if(candidate.ReferenceSpacingWarning!=null&&beforeRhythms!=null)break;
    }
    catch(InputAutomaticBeamMeterException error){RejectCorrection(n,error.Message);return;}
    catch(FormatException){RejectCorrection(n,null);return;}
    catch(ShortRhythmList error){RejectCorrection(n,error.Message);return;}
    catch(ArgumentException){RejectCorrection(n,null);return;}
    catch(InvalidDataException){RejectCorrection(n,null);return;}
    catch(NotSupportedException error){RejectCorrection(n,error.Message);return;}
   }
   musicInput.SpacingPause=beforeRhythms!=null&&candidate.ReferenceSpacingWarning!=null;
   musicInput.ReplayedSpacingWarning=!musicInput.SpacingPause&&candidate.ReferenceSpacingWarning!=null;
   var preview=(musicInput.SpacingPause?beforeRhythms:candidate.Preview(prefix)).Select(Item.Make).ToList();
   // Entering correction leaves both physical banks intact. A completed
   // replay redraws the normalized references and ordinary edited pitches.
   if(musicInput.TrackDisplay){
    bool emptySlurReopened=candidate.Stage==InputStaffStage.Slurs&&
     preview.Zip(score.Items,(a,b)=>a.Bytes.SequenceEqual(b.Bytes)).All(equal=>equal);
    bool trackedReplay=!musicInput.LetterInput&&musicInput.SaveQuestion&&
     (candidate.Stage==InputStaffStage.Complete||emptySlurReopened)&&
     preview.Count==score.Items.Count&&preview.Zip(score.Items,TrackInputReplayItem).All(tracked=>tracked)&&
     preview.Count(ReplayStaccato)==score.Items.Count(ReplayStaccato);
    if(trackedReplay){
     musicInput.ReferenceDisplayPositions.Clear();
     ReplayInputDisplayPreviews(displayReplay);
    }
    else{musicInput.TrackDisplay=false;InvalidateDisplayStorage();}
   }
   inputMotives.ReplaceFrom(candidateMotives);
   musicInput.Transaction=candidate;
   musicInput.BeamMenuEcho=null;
   musicInput.Prefix=prefixItems;
   if(destination!=null)musicInput.CorrectedStaff=destination.Staff;
   musicInput.AcceptedHeader=musicInput.CorrectionHeader;
   musicInput.AcceptedPositions=musicInput.CorrectionPositions;
   musicInput.Lines.Clear();
   musicInput.Lines.AddRange(lines);
   musicInput.PhysicalGroups.Clear();musicInput.PhysicalGroups.AddRange(physicalGroups.Take(lines.Count));
   musicInput.Correction=-1;
   musicInput.CorrectionLines=null;
   musicInput.PendingPhysical.Clear();musicInput.PendingPhysical.AddRange(pendingPhysical);
   musicInput.ContinuationBaseline=pendingPhysical.Count==0?null:candidateMotives.Clone();
   musicInput.PendingRhythms=0;
   if(pendingPhysical.Count>0)ParseInputContinuation(String.Concat(pendingPhysical.ToArray()));
   musicInput.RejectedLine=musicInput.InputError=null;
   musicInput.SaveQuestion=candidate.Stage==InputStaffStage.Complete;
   score.Items=preview;
   dirty=true;
   command.Clear();
   RefreshAll(null);
   UpdateMusicInput();
   ObserveInputDisplay(false);
  }
  bool TrackInputReplayItem(Item current,Item previous){
   if(current.Bytes.SequenceEqual(previous.Bytes))return true;
   if(IsUnmarkedQuarterOrHalf(current)&&IsUnmarkedQuarterOrHalf(previous)&&current.P(7)!=previous.P(7))
    return current.P(4)==previous.P(4);
   if(IsUnmarkedQuarterOrHalf(previous)&&previous.P(7)==1&&current.Count==7&&current.P(5)==12)
    return current.Bytes.Take(20).SequenceEqual(previous.Bytes.Take(20))&&
     current.Bytes.Skip(24).SequenceEqual(previous.Bytes.Skip(24));
   if(!OrdinaryInputReplayNote(current)||!OrdinaryInputReplayNote(previous))return false;
   if(ReplayStaccato(current)!=ReplayStaccato(previous))
    return Enumerable.Range(1,10).All(p=>current.P(p)==previous.P(p));
   // An unmarked quarter note inside the staff retains its vector count
   // when only the pitch changes (original correction case 1749).
   return current.Count==7&&previous.Count==7&&current.P(4)>=2&&current.P(4)<=10&&previous.P(4)>=2&&previous.P(4)<=10&&
    current.Bytes.Take(16).SequenceEqual(previous.Bytes.Take(16))&&current.Bytes.Skip(20).SequenceEqual(previous.Bytes.Skip(20));
  }
  bool IsUnmarkedQuarterOrHalf(Item item){
   return item.Code==1&&item.Staff==inputSetup.Staff&&item.Count==7&&item.P(5)==10&&
    item.P(4)>=2&&item.P(4)<=10&&(item.P(6)==0&&item.P(7)==1||item.P(6)==1&&item.P(7)==2);
  }
  bool OrdinaryInputReplayNote(Item item){
   return item.Code==1&&item.Staff==inputSetup.Staff&&(item.Count==7||item.Count==11&&item.P(11)==7)&&
    item.P(5)==10&&item.P(6)==0&&item.P(7)==1&&item.P(8)==0&&item.P(9)==0&&item.P(10)==0;
  }
  bool ReplayStaccato(Item item){return OrdinaryInputReplayNote(item)&&item.P(11)==7;}
  string CorrectionValue(int index){
   return index==0?musicInput.CorrectionHeader:index==1?musicInput.CorrectionPositions:musicInput.CorrectionLines[index-2];
  }
  string CorrectionLabel(int index){
   index=InputCorrectionStage(index);
   string[] labels={"Input= ","Pos.1, pos.2 >","Notes,etc. >","Rhythm >","Marks  >","Beams  >","Slurs >"};
   return labels[Math.Min(index,labels.Length-1)];
  }
  void RejectCorrection(int line,string unsupported){
   musicInput.Correction=line+2;
   RejectInputLine(CorrectionValue(line+2),unsupported);
  }
  void SetInputCorrectionLine(){
   ((NativeCommandBox)command).ScoreInsert=false;
   musicInput.RejectedLine=musicInput.InputError=null;
   command.Text=CorrectionValue(musicInput.Correction);
   command.SelectionStart=command.TextLength;
   UpdateMusicInput();
  }
  void EscapeMusicInput(){if(musicInput.LetterEditInput&&musicInput.Correction>=0)return;if(musicInput.Transaction==null){LeaveMusicInput();return;}if(musicInput.Transaction.Stage==InputStaffStage.Pitches&&musicInput.Lines.Count==0){AbandonMusicInput();return;}musicInput.AbandonQuestion=true;musicInput.RejectedLine=musicInput.InputError=null;command.Clear();UpdateMusicInput();command.Focus();}
  void AbandonMusicInput(){score.Items=musicInput.Prefix.Select(i=>i.Clone()).ToList();dirty=true;NextInputStaff();RefreshAll(null);UpdateMusicInput();}
  bool MusicInputKey(KeyEventArgs e){if(musicInput==null)return false;
   if(e.Modifiers==Keys.Control&&e.KeyCode==Keys.Q&&musicInput.Transaction!=null&&musicInput.Correction<0&&
      !musicInput.SaveQuestion&&!musicInput.AbandonQuestion&&!musicInput.SpacingPause&&musicInput.MotiveReplacementLine==null){
    if(command.TextLength==0&&!String.IsNullOrEmpty(musicInput.LastInputPhysical)){
     command.Text=musicInput.LastInputPhysical;
     command.SelectionStart=command.TextLength;
    }
    e.SuppressKeyPress=true;
    return true;
   }
   if(musicInput.MotiveReplacementLine!=null){if(e.KeyCode==Keys.ShiftKey||e.KeyCode==Keys.ControlKey||e.KeyCode==Keys.Menu)return false;e.SuppressKeyPress=true;AcknowledgeInputMotiveReplacement();return true;}
   if(musicInput.SpacingPause){
    if(e.KeyCode==Keys.ShiftKey||e.KeyCode==Keys.ControlKey||e.KeyCode==Keys.Menu)return false;
    e.SuppressKeyPress=true;
    AcknowledgeSpacingWarning();
    return true;
   }if(e.KeyCode==Keys.Escape){e.SuppressKeyPress=true;EscapeMusicInput();return true;}if(musicInput.AbandonQuestion){e.SuppressKeyPress=true;if(e.KeyCode==Keys.N)StartInputCorrection();else if(e.KeyCode==Keys.Y)AbandonMusicInput();return true;}if(musicInput.Correction>=0&&(e.KeyCode==Keys.Up||e.KeyCode==Keys.Down)){
   e.SuppressKeyPress=true;
   if(e.KeyCode==Keys.Up){
    if(musicInput.Correction>=2&&!String.IsNullOrWhiteSpace(command.Text))
     musicInput.CorrectionLines[musicInput.Correction-2]=command.Text.Trim();
    if(musicInput.Correction>0){musicInput.Correction--;SetInputCorrectionLine();}
   }else SubmitInputCorrection(command.Text.Trim());
   return true;
  }if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;string text=command.Text;command.Clear();SubmitMusicInput(text);return true;}if(e.KeyCode>=Keys.F1&&e.KeyCode<=Keys.F10){e.SuppressKeyPress=true;MusicInputFunction(e.KeyCode-Keys.F1+1,e.Modifiers);return true;}if(musicInput.PositionsQuestion)return false;if(musicInput.Transaction==null){if(e.Control&&e.KeyCode==Keys.End){e.SuppressKeyPress=true;UndoInputPreset();return true;}if(e.KeyCode==Keys.Left||e.KeyCode==Keys.Right){e.SuppressKeyPress=true;CommitInputField();inputSetup.MoveField(e.KeyCode==Keys.Left?-1:1);UpdateMusicInput();return true;}if(e.KeyCode==Keys.Up||e.KeyCode==Keys.Down){e.SuppressKeyPress=true;if(NativeScoreBehaviour.Enabled){if((e.Modifiers&(Keys.Control|Keys.Alt))==0)PanInputSetup(e.KeyCode,5);}else{CommitInputField();MoveInputFieldRow(e.KeyCode==Keys.Down?1:-1);}return true;}if((e.KeyCode==Keys.PageUp||e.KeyCode==Keys.PageDown)&&(e.Modifiers&(Keys.Control|Keys.Alt))==0){e.SuppressKeyPress=true;PanInputSetup(e.KeyCode,50);return true;}if(e.KeyCode==Keys.Space){e.SuppressKeyPress=true;BeginInputStaff();return true;}}if(e.Control&&(e.KeyCode==Keys.S||e.KeyCode==Keys.O||e.KeyCode==Keys.Z||e.KeyCode==Keys.Y)){e.SuppressKeyPress=true;return true;}return false;}
  void PanInputSetup(Keys key,float amount){ApplyMouseCamera(()=>NativeDosViewport.Pan(key==Keys.Up||key==Keys.PageUp?"ZU":"ZD",amount));}
  bool MusicInputCharacter(char c){if(musicInput!=null&&musicInput.PositionsQuestion)return false;if(musicInput==null||musicInput.Transaction!=null)return false;if(c=='/'){CommitInputField();inputSetup.MoveField(1);UpdateMusicInput();return true;}char upper=Char.ToUpperInvariant(c);if(inputSetup.Selected==6&&command.TextLength==0&&(upper=='P'||upper=='R')){inputSetup.SpacingLetter(upper);UpdateMusicInput();return true;}if(Char.IsControl(c))return false;if(inputSetup.Selected==11)return !(upper>='A'&&upper<='G'||upper=='S'||c=='#');return !(Char.IsDigit(c)||c=='.'||c=='-'&&command.SelectionStart==0&&(inputSetup.Selected==2||inputSetup.Selected==3));}
  void MusicInputFunction(int key,Keys modifiers){if(musicInput.PositionsQuestion)return;CloseInputFieldEditor();if(musicInput.SpacingPause){AcknowledgeSpacingWarning();return;}if(musicInput.Transaction==null){InputSetupFunction(key,modifiers);return;}if(key==1){Reference();return;}string token=null;switch(musicInput.Transaction.Stage){case InputStaffStage.Pitches:string[] pitch={"","r","m","su","sd","so","p","o"};if(key<=8)token=pitch[key-1];break;case InputStaffStage.Rhythms:token=InputRhythmKeyToken(key,modifiers);break;case InputStaffStage.Marks:string[] marks={"","p","mp","mf","f","w","ac","ri","o","ob"};token=marks[key-1]+" ";break;case InputStaffStage.Beams:if(key>=2){token=key==10?"ab":key+"b";if(CanSubmitAutomaticBeam(key)&&musicInput.Correction<0&&!musicInput.AbandonQuestion&&musicInput.InputError==null){SubmitAutomaticBeam(token);return;}}break;}if(token!=null)AppendMusicInputFunctionToken(token);}
  void AppendMusicInputFunctionToken(string token){
   char prior=command.SelectionStart==0?'\0':command.Text[command.SelectionStart-1];
   bool separate=(musicInput.Transaction.Stage==InputStaffStage.Pitches||musicInput.Transaction.Stage==InputStaffStage.Rhythms||musicInput.Transaction.Stage==InputStaffStage.Marks)&&command.SelectionStart>0&&prior!='/'&&prior!='('&&!(musicInput.Transaction.Stage==InputStaffStage.Marks&&(prior=='p'||prior=='f'));
   // 0F4F:1AD1..1B5B writes the separator and function token separately.
   if(command.SelectionLength==0){if(separate)StoreInputMouseToken("/");StoreInputMouseToken(token);}else command.SelectedText=(separate?"/":"")+token;
   command.Focus();
  }
  void InputSetupFunction(int key,Keys modifiers){
   if(scor4Interface){
    if(modifiers==Keys.None||modifiers==Keys.Alt)
     RunScor4Key(key-1+(modifiers==Keys.Alt?10:0));
    return;
   }
   int scan=key+0x3a;
   if((modifiers&Keys.Alt)!=0)scan=key+0x67;
   else if((modifiers&Keys.Control)!=0)scan=key+0x5d;
   else if((modifiers&Keys.Shift)!=0)scan=key+0x53;
   LoadInputPreset(scan);
  }
  string InputRhythmKeyToken(int key,Keys modifiers){
   if(key<2||key>8)return null;
   if(scor4Interface){
    if(modifiers==Keys.Control)return scor4TripletRhythms[key-2];
    if(modifiers==Keys.None){
     scor4RhythmWord=(short)(1<<(key-2));
     return scor4DupleRhythms[key-2];
    }
   }
   string[] rhythm={"w","h","q","e","s","32","64"};
   string token=rhythm[key-2];
   if((modifiers&Keys.Shift)!=0)token+=".";
   if((modifiers&Keys.Alt)!=0)token=key==7?"tt":key==8?"12:1":"t"+token;
   return token;
  }
  bool CanSubmitAutomaticBeam(int key){
   // Shared reader 0F4F:0BB7..0C29 replaces the draft with nB; for every
   // F2..F10 scan in the beam stage, independently of its current caret.
   return key>=2&&key<=10&&command.SelectionLength==0;
  }
  void SubmitAutomaticBeam(string token){
   string line=token+";";
   string echo=command.Text+line;
   SubmitMusicInput(line);
   if(echo==line||musicInput.Transaction.Stage!=InputStaffStage.Slurs)return;
   musicInput.BeamMenuEcho=echo;
   UpdateMusicInput();
  }
  void AutoCompleteMusicInputLine(){
   if(musicInput==null||musicInput.Transaction==null||command.TextLength!=157)return;
   // Shared reader 0F4F:02CD returns after the 157th physical character.
   string text=command.Text;
   command.Clear();
   SubmitMusicInput(text);
  }
  bool MusicInputMenuButton(int key,MouseButtons button){
   if(mode!=ScreenMode.Input||musicInput==null||musicInput.Transaction==null
    ||musicInput.Correction>=0||musicInput.AbandonQuestion||musicInput.SpacingPause||key<2||key>10)return false;
   var stage=musicInput.Transaction.Stage;
   if(stage==InputStaffStage.Beams){
    if(!CanSubmitAutomaticBeam(key)||musicInput.InputError!=null)return false;
   }else if(stage==InputStaffStage.Pitches||stage==InputStaffStage.Rhythms){
    if(key>8)return false;
   }else if(stage!=InputStaffStage.Marks)return false;
   Keys modifier=Keys.None;
   if(stage==InputStaffStage.Rhythms)
    modifier=button==MouseButtons.Right?(scor4Interface?Keys.Control:Keys.Shift):Keys.Alt;
   MusicInputFunction(key,modifier);
   UpdateMusicInput();
   return true;
  }
  // Original10D0:0EF7..0FB5 scans adjacent characters up to the caret,
  // including the next byte when the first byte is immediately before it.
  static int InputMouseClef(string text,int caret,int clef){
   for(int i=0;i<Math.Min(caret,text.Length);i++){
    int first=text[i],next=i+1<text.Length?text[i+1]:' ';
    if(first>90)first-=32;if(next>90)next-=32;
    if(first=='A'&&next=='L')clef=2;
    if(first=='B'&&next=='A')clef=1;
    if((first=='P'||first=='T')&&next=='R')clef=0;
    if(first=='T'&&next=='E')clef=3;
   }
   return clef;
  }
  void StoreInputMouseToken(string token){
   int caret=command.SelectionStart,oldLength=command.TextLength;
   // 10D0:0188 increments both cursor and logical end for each byte,
   // even in overtype mode. Preserve the resulting trailing blanks.
   if(caret+token.Length>156||oldLength+token.Length>156)return;
   if(((NativeCommandBox)command).ScoreInsert)command.Text=command.Text.Insert(caret,token);
   else{
    string text=command.Text.PadRight(Math.Max(oldLength,caret+token.Length));
    command.Text=(text.Substring(0,caret)+token+text.Substring(caret+token.Length)).PadRight(oldLength+token.Length);
   }
   command.SelectionStart=caret+token.Length;command.SelectionLength=0;
  }
  void MusicInputMouse(int buttons,Keys modifiers,PointF position,PointF start,bool released){
   if(!released)return;
   command.Focus();
   // 13EE:005E maps Control to middle before retaining the largest held mask.
   if((modifiers&Keys.Control)!=0)buttons=Math.Max(4,buttons);
   if((modifiers&Keys.Alt)==0&&MusicInputEndpointMouse(buttons,position))return;
   int staff=inputSetup.Staff;
   var tx=musicInput.Transaction;
   string text=command.Text;
   if(tx!=null&&tx.Stage==InputStaffStage.Rhythms
    &&musicInput.Correction<0&&!musicInput.AbandonQuestion&&musicInput.InputError==null){
    // 13EE:03DC..0420 dispatches combined buttons before the rhythm
    // stage's notation-area no-action path at 052A..05B2.
    if(buttons==5||buttons==6)EscapeMusicInput();
    else if(buttons==7)MusicInputFunction(8,Keys.None);
    return;
   }
   if(tx!=null&&tx.Stage==InputStaffStage.Pitches
    &&musicInput.Correction<0&&!musicInput.AbandonQuestion&&musicInput.InputError==null
    &&(modifiers&Keys.Alt)==0&&buttons>=1&&buttons<=7
    &&NativeDosViewport.Divisor>0
    &&command.SelectionLength==0){
    var pointer=NativeMouseSelector.Pointer(canvas,position);
    float rawBase=canvas.PageDocument.Tables.Bases[staff];
    float scale=canvas.PageDocument.Tables.Scales[staff];
    if(pointer.X>=0&&pointer.X<=639&&pointer.Y>=canvas.DosClipTop&&pointer.Y<=461&&scale>0){
     if(buttons==3){StoreInputMouseToken(":");return;}
     int clef=InputMouseClef(text,command.SelectionStart,musicInput.MouseClef??tx.InitialClef);
     musicInput.MouseClef=clef;
     int clefOffset=clef==1?12:clef==2?6:clef==3?8:0;
     {
      // 10D0:0EAF stores the raw ordinate delta before dividing by seven staff units.
      short ordinate=unchecked((short)(NativeDosViewport.OriginY-(int)pointer.Y-1));
      float distance=(float)((double)ordinate*NativeDosViewport.Divisor-rawBase);
      double pitchValue=27-clefOffset+Math.Round(distance/(7.0*scale),MidpointRounding.AwayFromZero);
      if(pitchValue<0||pitchValue>63)return;
      int pitch=(int)pitchValue;
      char prior=command.SelectionStart==0?' ' :text[command.SelectionStart-1];
      string separator=command.SelectionStart==0||prior=='/'||prior==':'||prior=='('?"":"/";
      string sign=buttons>=4?new[]{"N","S","F",""}[buttons-4]:"";
      StoreInputMouseToken(separator+"CDEFGAB"[pitch%7]+sign+(pitch/7).ToString(CultureInfo.InvariantCulture));
      return;
     }
    }
   }
   // The original dispatcher returns without editing for inactive regions.
   return;
  }
  bool MusicInputEndpointMouse(int buttons,PointF position){
   var tx=musicInput.Transaction;
   if(tx==null||tx.Stage<InputStaffStage.Marks||tx.Stage>InputStaffStage.Slurs
    ||musicInput.Correction>=0||musicInput.AbandonQuestion||musicInput.SpacingPause
    ||musicInput.InputError!=null||NativeDosViewport.Divisor<=0
    ||buttons<1||buttons>7
    ||command.SelectionLength!=0)return false;
   var point=NativeMouseSelector.Pointer(canvas,position);
   if(point.X<0||point.X>639||point.Y<canvas.DosClipTop||point.Y>canvas.DosClipBottom)return false;
   // 13EE:03DC handles combined buttons before the endpoint dispatcher.
   // Masks five/six request Escape; all three return scan 42h (F8).
   if(buttons==3)return true;
   if(buttons==5||buttons==6){EscapeMusicInput();return true;}
   if(buttons==7){MusicInputFunction(8,Keys.None);return true;}
   // 10D0:0821 skips number formatting for a middle click at column one.
   if(buttons==4&&command.SelectionStart==0){
    StoreInputMouseToken(";");
    SubmitMusicInput(command.Text);
    return true;
   }
   var notes=tx.MouseNotePositions();
   if(notes.Length<1||notes.Length>(NativeEditCompatibility.AllowLargeInput?200:150))return false;
   double number;
   {
    var anchors=new float[notes.Length+2];
    var staffRecord=musicInput.Prefix.FirstOrDefault(item=>item.Code==8&&item.Staff==tx.OriginalStaff);
    anchors[0]=staffRecord==null?0:staffRecord.P(3);
    anchors[anchors.Length-1]=200;
    for(int i=0;i<notes.Length;i++){
     if(Single.IsNaN(notes[i])||Single.IsInfinity(notes[i]))return false;
     anchors[i+1]=(float)((int)((double)notes[i]*100)/100.0);
    }
    Array.Sort(anchors,1,notes.Length);
    // 10D0:0878 stores the low signed product word before division.
    short product=unchecked((short)(((int)point.X-NativeDosViewport.OriginX)*NativeDosViewport.Divisor));
    double pointerX=product/150.0;
    float x=(float)pointerX,lastAnchor=anchors[notes.Length];
    // The original scans the note table through its terminal 200 word.
    // A last note beyond 200 makes that terminal word a descending boundary.
    if(lastAnchor>=200&&x>=lastAnchor)number=notes.Length+(lastAnchor==200&&x>200?1:0);
    else if(lastAnchor<200&&x>200)number=notes.Length+1;
    else {
     int interval=0;
     while(interval<notes.Length&&x>=anchors[interval+1])interval++;
     float lower=anchors[interval],upper=anchors[interval+1];
     float distance=(float)((double)x-lower),width=(float)((double)upper-lower);
     if(distance>0&&!(width>0))return false;
     int fraction=distance<=0?0:(int)Math.Round((double)distance/width*100,MidpointRounding.AwayFromZero);
     // 10D0:09B5 selects integer endpoints for beams and slurs.
     if(tx.Stage!=InputStaffStage.Marks)number=fraction>=50?interval+1:interval;
     else number=fraction>=99?interval+1:fraction<=1?interval:interval+fraction/100.0;
    }
   }
   string token=number.ToString(number==Math.Truncate(number)?"0":"0.00",CultureInfo.InvariantCulture);
   if(number>=100){
    if(NativeEditCompatibility.ReproduceInvalidStates){
     // The original writes the tens quotient as one character in every
     // endpoint stage. A value above 99 can therefore start with punctuation.
     token=(char)('0'+(int)number/10)+token.Substring(2);
    }else if(tx.Stage!=InputStaffStage.Marks){
     // In a beam or slur, a typed hundreds digit is a packed control.
     // A click on an unrepresentable note must not target a different note.
     return true;
    }
   }
   int caret=command.SelectionStart;
   // 10D0:0842..0873 preserves these separators before writing the number.
   if(caret>0&&" /+-[".IndexOf(command.Text[caret-1])<0)token=" "+token;
   StoreInputMouseToken(token+(buttons==1?" ":buttons==2?"/":";"));
   if(buttons==4)SubmitMusicInput(command.Text);
   return true;
  }
  protected override bool ProcessCmdKey(ref System.Windows.Forms.Message message,Keys keys){
   if(PrintingKey(keys))return true;
   if(OverviewKey(keys))return true;
   if(keys==(Keys.Alt|Keys.S)){Safe(()=>BeginOverview());return true;}
   if(mode==ScreenMode.EditMenu&&pendingGraphicalMenu==0&&(keys==(Keys.Shift|Keys.F6)||keys==(Keys.Alt|Keys.F6))){
    Safe(()=>ToggleEditGraphicMenu(keys==(Keys.Shift|Keys.F6)?2:3));
    return true;
   }

   if(HandleInputDisplayKey(keys))return true;
   if(OriginalGroupFieldKey(keys))return true;
   if(HandleScor4MenuKey(new KeyEventArgs(keys)))return true;
   if(SharedNavigationKey(keys))return true;
   if(SharedReaderKey(keys))return true;
   Keys code=keys&Keys.KeyCode;
   if(mode==ScreenMode.Edit&&editParameter==0&&parameters.Focused&&parameters.HandleHighlightedKey(new KeyEventArgs(keys)))return true;
   if(mode==ScreenMode.Edit&&!originalItemZero&&!parameters.EditingValue&&setupEditField==0&&!inlineTextActive&&!pendingEditAcknowledgement&&pendingFilteredEdit==null&&externalDrawingNameItem==null&&letterHelpPages==null&&musicInput==null&&
      (((keys&Keys.Modifiers)==Keys.Alt&&code>=Keys.D0&&code<=Keys.D9)||
       editParameter==0&&((keys&Keys.Modifiers)==Keys.None&&(code==Keys.Left||code==Keys.Right||code==Keys.Up||code==Keys.Down||code==Keys.PageUp||code==Keys.PageDown||code==Keys.Insert||code==Keys.End||code==Keys.Tab)||keys==(Keys.Shift|Keys.Tab))||
       editParameter!=0&&(keys&Keys.Modifiers)==Keys.None&&(code==Keys.Up||code==Keys.Down||code==Keys.Left||code==Keys.Right||code==Keys.Home||code==Keys.PageUp||code==Keys.PageDown))){
    bool consumed=false;Safe(()=>consumed=HandleOriginalEditMovementKey(new KeyEventArgs(keys)));if(consumed)return true;
   }
   if(musicInput!=null&&keys==(Keys.Control|Keys.Q)&&MusicInputKey(new KeyEventArgs(keys)))return true;
   if(musicInput!=null&&code>=Keys.F1&&code<=Keys.F10){Safe(()=>MusicInputKey(new KeyEventArgs(keys)));return true;}
   if(pendingEditAcknowledgement&&code==Keys.Tab)return AcknowledgeEditPause();
   if(mode==ScreenMode.Edit&&canvas.Focused&&!originalItemZero&&(keys==Keys.Tab||keys==(Keys.Shift|Keys.Tab))){
    Safe(()=>MoveSelected(keys==Keys.Tab?10:-10,0));
    return true;
   }
   return base.ProcessCmdKey(ref message,keys);
  }
  void InputMenu(string[] labels,int count){for(int i=0;i<functionKeys.Length;i++){functionKeys[i].Visible=i<count;functionStrip.ColumnStyles[i].SizeType=SizeType.Percent;functionStrip.ColumnStyles[i].Width=i<count?100f/count:0;if(i<count)functionKeys[i].Text="F"+(i+1)+(String.IsNullOrEmpty(labels[i])?"":"-"+labels[i]);}UpdateScor4Menu();}
  void UpdateMusicInput(){
   ((NativeCommandBox)command).ScoreEditing=musicInput!=null&&musicInput.Transaction!=null&&(!musicInput.SaveQuestion||musicInput.Correction>=0)&&!musicInput.AbandonQuestion&&!musicInput.FilenameQuestion;if(musicInput==null)return;if(musicInput.Transaction!=null){
    var retained=musicInput.Transaction;originalInputStageHeader=Math.Min(5,(int)retained.Stage+1);
    // Original F2 and Main FL/ENH share DS:8780 and the bounds pointer
    // DS:7954 == DS:7D1A. Pending setup fields live separately at DS:83E0.
    originalNotationStaff=retained.OriginalStaff;
    originalNotationRange=new[]{originalNotationStaff,retained.OriginalSelectionLeft,retained.OriginalSelectionRight,0,originalNotationStaff};
   }UpdateInputNumbers();UpdatePinnedHeader();UpdateInputRows();UpdateCorrectionField();
   if(musicInput.PositionsQuestion){InputMenu(new[]{"HELP","INPUT","EDIT","TEXT","PLAY","PRINT","DISK","EXIT"},8);Message((musicInput.LetterLaunchNotice==null?"":musicInput.LetterLaunchNotice+"\r\n")+"Type Pos1, Pos2. ->");return;}
   if(musicInput.Transaction==null){InputMenu(setupLabels,10);Message(pendingScor4Preset!=null?Scor4PresetWarning:(inputPresetBackup==null?"":"<Control End> to Undo last Setup\r\n")+"Alter Parameters, Get Staff Setups, Press SPACE to Continue or Press ESC to Exit");return;}
   if(musicInput.MotiveReplacementLine!=null){Message("Replacing Motive "+musicInput.MotiveReplacementName+"\r\nPress any key to continue.");return;}
   if(musicInput.SpacingPause){
    InputMenu(new[]{"HELP","WHOLE","HALF","QUARTER","EIGHT","1/16","1/32","1/64"},8);
    Message(musicInput.Transaction.ReferenceSpacingWarning+"\r\nPress any key to continue.");
    return;
   }
   var stage=musicInput.Transaction.Stage;string[] labels=stage==InputStaffStage.Pitches?new[]{"HELP","REST","BAR","STM UP","STM DN","STM ORD","PROX ON","PROX OFF"}:stage==InputStaffStage.Rhythms?new[]{"HELP","WHOLE","HALF","QUARTER","EIGHT","1/16","1/32","1/64"}:stage==InputStaffStage.Marks?new[]{"HELP","p","mp","mf","f","WEDGE","accel","rit","8va","8vb"}:stage==InputStaffStage.Beams?new[]{"HELP","2B","3B","4B","5B","6B","7B","8B","9B","AUTO"}:new[]{"HELP","****","****","****","****","****","****","****"};InputMenu(labels,labels.Length);
   string proximityText=musicInput.ProximityAfterError?"Proximity mode is ON":"Proximity mode is OFF";
   string question=musicInput.FilenameQuestion?"Input file name ->":musicInput.AbandonQuestion?"Abandon input? ->":musicInput.Correction>=0?"D=delete line, B=backup 1 line, I=insert line, G=go, <Enter>=line O.K.":musicInput.SaveQuestion?"Save this staff of input to a file? (Y/N) ->":stage==InputStaffStage.Pitches?"Enter bar lines, clefs, key signatures, time signatures, pitches and rests.\r\n"+proximityText+"\r\nType notes,etc>":stage==InputStaffStage.Rhythms?"Enter "+(musicInput.Transaction.RhythmCount-musicInput.PendingRhythms).ToString().PadLeft(2)+" rhythms ->":stage==InputStaffStage.Marks?"Type marks   ->":stage==InputStaffStage.Beams?"Type beams   ->":"Type slurs   ->";var history=new List<string>();string[] prefixes={"Type notes,etc>","Enter "+musicInput.Transaction.RhythmCount.ToString().PadLeft(2)+" rhythms ->","Type marks   ->","Type beams   ->","Type slurs   ->"};for(int i=0;i<musicInput.Lines.Count;i++){if(i<musicInput.PhysicalGroups.Count&&musicInput.PhysicalGroups[i].Count>1){foreach(string row in musicInput.PhysicalGroups[i])history.Add(prefixes[Math.Min(i,4)]+row);}else history.Add(prefixes[Math.Min(i,4)]+(i==3&&musicInput.BeamMenuEcho!=null?musicInput.BeamMenuEcho:musicInput.Lines[i]));}foreach(string row in musicInput.PendingPhysical)history.Add(prefixes[Math.Min(musicInput.Lines.Count,4)]+row);if(musicInput.InputError!=null){
    history.Add((musicInput.Correction>=0?CorrectionLabel(musicInput.Correction):question)+musicInput.RejectedLine);
    history.Add(musicInput.InputError);
   }
   if(stage==InputStaffStage.Pitches&&musicInput.MidiNotice!=null)question=musicInput.MidiNotice;
   if(stage!=InputStaffStage.Pitches&&musicInput.ProximityAfterError)history.Insert(0,proximityText);
   if(musicInput.ReplayedSpacingWarning){history.Clear();history.Add(musicInput.Transaction.ReferenceSpacingWarning);}
   else if(stage==InputStaffStage.Beams&&musicInput.Transaction.ReferenceSpacingWarning!=null)
    history.Add(musicInput.Transaction.ReferenceSpacingWarning);
   Message((musicInput.LetterLaunchNotice==null?"":musicInput.LetterLaunchNotice+"\r\n")+String.Join("\r\n",history.ToArray())+(history.Count==0?"":"\r\n")+question);
   if(inputCorrectionLine.Visible)LayoutTextContents(true);
  }
 }
}


