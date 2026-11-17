using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Drawing;using System.Windows.Forms;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  static void CompareInputItems(Score score,string path){byte[] expected=File.ReadAllBytes(path);string metadata=Path.ChangeExtension(path.Replace("-items.bin",".json"),".json");var count=System.Text.RegularExpressions.Regex.Match(File.ReadAllText(metadata),"\"count\"\\s*:\\s*([0-9]+)");if(!count.Success||score.Items.Count!=Int32.Parse(count.Groups[1].Value))throw new Exception("Input preview item count differs in "+metadata);int at=0;foreach(var item in score.Items){if(at+item.Bytes.Length>expected.Length)throw new Exception("Input preview length");for(int i=0;i<item.Bytes.Length;i++)if(item.Bytes[i]!=expected[at+i])throw new Exception("Input preview differs at byte "+(at+i)+" in "+path);at+=item.Bytes.Length;}}
  PointF InputMouseTestPoint(int x,int y){
   if(NativeNotationView.IsDosViewport)return new PointF(x*canvas.RenderScale,(y-97)*canvas.RenderScale-canvas.CropTop);
   var p=canvas.PageDocument.Projection.Point((x-NativeDosViewport.OriginX)*NativeDosViewport.Divisor,(NativeDosViewport.OriginY-y)*NativeDosViewport.Divisor);
   return new PointF((p.X+canvas.CameraOffset.X+canvas.ScreenOrigin.X)*canvas.RenderScale,(p.Y+canvas.CameraOffset.Y+canvas.ScreenOrigin.Y)*canvas.RenderScale-canvas.CropTop);
  }
  void ClickInputTest(int button,int x,int y,int? pressY=null){
   ScoreMouse(button,Keys.None,InputMouseTestPoint(x,y),InputMouseTestPoint(x,pressY??y),true);
  }
  void VerifyMidiInput(){
   var pitch=new MidiInputLine(InputStaffStage.Pitches);
   string line="";
   foreach(int key in new[]{60,61,62,63,64,65,66,67,68,69,70,71,72,48,84,60,44,45,46,43})
    {line=pitch.Note(key,36,line,0)??line;pitch.Release(key);}
   if(line!="PC4/CS/D/EF/EN/F/FS/G/GS/A/BF/BN/C/C3/C6/CN4/SD/SO/SU;")
    throw new Exception("F2 MIDI chromatic pitch capture differs: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   for(int key=60;key<=71;key++){
    pitch.Note(37,36,line,0);
    line=pitch.Note(key,36,line,0);
    pitch.Release(key);
   }
   if(pitch.Note(43,36,line,0)!="PC4/DF/D/EF/FF/F/GF/G/AF/A/BF/CF;")
    throw new Exception("F2 MIDI flat spelling capture differs: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   for(int key=60;key<=71;key++){
    pitch.Note(38,36,line,0);
    line=pitch.Note(key,36,line,0);
    pitch.Release(key);
   }
   if(pitch.Note(43,36,line,0)!="PCN4/DN/DN/DN/EN/FN/GN/GN/AN/AN/AN/BN;")
    throw new Exception("F2 MIDI natural spelling capture differs: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   for(int key=60;key<=71;key++){
    pitch.Note(39,36,line,0);
    line=pitch.Note(key,36,line,0);
    pitch.Release(key);
   }
   if(pitch.Note(43,36,line,0)!="PBS3/CS/D/DS/E/ES/FS/G/GS/A/AS/B;")
    throw new Exception("F2 MIDI sharp spelling capture differs: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   foreach(int key in new[]{60,37,36,39,36,38,38,36,38,37,36,37,37,63,39,39,62,38,38,66,47,47,44,47,47,67,47,47,47,61,43})
    {line=pitch.Note(key,36,line,0)??line;pitch.Release(key);}
   if(line!="PC4/MD /MDH /M2 /MD1 /E/CSS/F/TE/K1S/K7S;")
    throw new Exception("F2 MIDI repeated control capture differs: "+line);
   if(InputPitchToken.Parse("MDH",new InputPitchState())!=4201)
    throw new Exception("F2 MIDI heavy double bar differs from DOS type 2.");
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   foreach(int key in new[]{44,45,46,41,40,60,62,64,65,67,69,71,61,63,66,68,70}){
    pitch.Note(47,36,line,0);
    pitch.Note(47,36,line,0);
    line=pitch.Note(key,36,line,0)??line;
   }
   if(line!="/TE/TR/K2F/AL/TE/K2S/K4S/K1F/K1S/K3S/K5S/K5F/K3F/K6S/K4F/K2F")
    throw new Exception("F2 MIDI clef and key selector capture differs: "+line);
   pitch.Note(47,36,line,0);
   pitch.Note(47,36,line,0);
   if(pitch.Note(43,36,line,0)!=line+"/BA")
    throw new Exception("F2 MIDI bass-clef selector differs from DOS.");
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   for(int key=60;key<=71;key++){
    pitch.Note(37,36,line,0);
    pitch.Note(37,36,line,0);
    line=pitch.Note(key,36,line,0);
    pitch.Release(key);
   }
   if(line!="PDFF4/C/EFF/E/E/GFF/F/AFF/G/BFF/B/B")
    throw new Exception("F2 MIDI double-flat sweep differs: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   for(int key=60;key<=71;key++){
    pitch.Note(39,36,line,0);
    pitch.Note(39,36,line,0);
    line=pitch.Note(key,36,line,0);
    pitch.Release(key);
   }
   if(line!="PC4/C/CSS/E/DSS/F/F/FSS/G/GSS/B/ASS")
    throw new Exception("F2 MIDI double-sharp sweep differs: "+line);
   foreach(var capture in new[]{
    new {Keys=new[]{37,66,66,36,66,78,39,66,66,36,66,43},Text="PGF4/G/M /GF/GF5/FS4/F/M /FS;"},
    new {Keys=new[]{37,66,66,37,66,65,66,38,66,66,39,66,66,43},Text="PGF4/G/GF/F/G/FN/F/FS/FN;"},
    new {Keys=new[]{37,61,38,61,61,37,68,38,68,68,37,71,38,71,71,43},Text="PDF4/CN/C/AF4/GN/G/CF/BN/B;"},
    new {Keys=new[]{37,37,60,38,60,60,39,39,62,38,62,62,38,38,61,61,38,38,63,63,43},Text="PDFF4/CN/C/CSS/DN/D/C/CS/E/EF;"},
    new {Keys=new[]{47,47,63,37,61,39,61,61,38,61,61,37,37,60,38,60,39,39,62,38,38,66,66,43},Text="/K3F/PDF4/CS/C/DN/D/DFF/CN/CSS/G4/GF;"},
    new {Keys=new[]{37,37,37,62,39,39,39,61,38,38,38,63,37,39,66,39,37,68,38,37,60,37,38,62,43},Text="PCSS4/C/E/GN/AN/DFF4/DFF;"}
   }){
    pitch=new MidiInputLine(InputStaffStage.Pitches);
    line="";
    foreach(int key in capture.Keys){
     line=pitch.Note(key,36,line,0)??line;
     pitch.Release(key);
    }
    if(line!=capture.Text)throw new Exception("F2 MIDI stateful accidental capture differs: "+line);
   }
   foreach(int selector in new[]{65,67}){
    pitch=new MidiInputLine(InputStaffStage.Pitches);
    pitch.Note(47,36,"",0);
    pitch.Note(47,36,"",0);
    line=pitch.Note(selector,36,"",0);
    for(int key=60;key<=71;key++){
     line=pitch.Note(key,36,line,0);
     pitch.Release(key);
    }
    string expected=selector==65?"/K1F/PC4/CS/D/EF/EN/F/FS/G/AF/AN/B/BN"
     :"/K1S/PC4/CS/D/DS/E/FN/FS/G/GS/A/BF/BN";
    if(line!=expected)throw new Exception("F2 MIDI chromatic sweep after an in-line signature differs: "+line);
   }
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   foreach(int selector in new[]{62,63}){
    pitch.Note(47,36,line,0);
    pitch.Note(47,36,line,0);
    line=pitch.Note(selector,36,line,0);
    for(int key=60;key<=71;key++){
     line=pitch.Note(key,36,line,0);
     pitch.Release(key);
    }
   }
   if(line!="/K2S/PCN4/CS/D/DS/E/FN/FS/G/GS/A/AS/B/K3F/C4/DF/DN/E/EN/F/GF/GN/A/AN/B/BN")
    throw new Exception("F2 MIDI D-to-E-flat sweep differs from DOS: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   foreach(int key in new[]{60,61,66,47,47,47,59}
    .Concat(Enumerable.Range(60,12)).Concat(new[]{47,47,47,61}).Concat(Enumerable.Range(60,12)).Concat(new[]{43})){
    line=pitch.Note(key,36,line,0)??line;
    pitch.Release(key);
   }
   if(line!="PC4/CS/FS/K7F/CN4/D/EFF/EF/F/FN/G/GN/A/BFF/BF/C/K7S/B3/C/CSS/D/EN/ES/F/FSS/G/GSS/A/BN;")
    throw new Exception("F2 MIDI seven-flat and seven-sharp sweep differs from DOS: "+line);
   pitch=new MidiInputLine(InputStaffStage.Pitches);
   line="";
   foreach(int key in new[]{60,47,47,65,60,39,68,68,47,47,67,60,47,47,65,68,47,47,60,60,43}){
    line=pitch.Note(key,36,line,0)??line;
    pitch.Release(key);
   }
   if(line!="PC4/K1F/C4/GS4/G/K1S/C4/K1F/AF4/C4;")
    throw new Exception("F2 MIDI returning to an earlier key signature differs from DOS: "+line);
   var map=new MidiInputLine(InputStaffStage.Rhythms);
   line="";
   for(int key=64;key<=86;key++)line=map.Note(key,36,line,23);
   if(line!=" W/H/H./Q/Q./E/E./S/32/S./64/G/TW/TH/TH./TQ/TQ./TE/TE./TS/48/TS./96;")
    throw new Exception("F2 MIDI rhythm map differs: "+line);
   var rhythm=new MidiInputLine(InputStaffStage.Rhythms);
   line="";
   foreach(int key in new[]{64,66,54,48,55})line=rhythm.Note(key,36,line,8)??line;
   if(line!=" W/H ;")throw new Exception("F2 MIDI rhythm editing capture differs: "+line);

   var empty=NativeMeasurements.New(source);
   empty.Items.Clear();
   SetDocument(empty,"");
   inputSetup=new InputSetupState();
   StartMusicInput();
   BeginInputStaff();
   musicInput.MidiLine=new MidiInputLine(InputStaffStage.Pitches,true);
   foreach(int key in new[]{60,64})ReceiveMidiInput((uint)(0x90|key<<8|64<<16),36);
   ReceiveMidiInput(0x80|60<<8,36);
   ReceiveMidiInput(0x90|67<<8|64<<16,36);
   foreach(int key in new[]{64,67})ReceiveMidiInput((uint)(0x80|key<<8),36);
   foreach(int key in new[]{71,43}){
    ReceiveMidiInput((uint)(0x90|key<<8|64<<16),36);
    ReceiveMidiInput((uint)(0x90|key<<8),36);
   }
   if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms||musicInput.Lines[0]!="PC4:E:G/B;")
    throw new Exception("MIDI note packets did not advance F2 to rhythms.");
   foreach(int key in new[]{67,69})ReceiveMidiInput((uint)(0x90|key<<8|64<<16),36);
   if(musicInput.Transaction.Stage!=InputStaffStage.Marks||musicInput.Lines[1]!="Q/E;")
    throw new Exception("MIDI rhythm packets did not automatically advance F2 to marks.");
   AbandonMusicInput();
   LeaveMusicInput();

   // Fresh original MIDI-reader captures for every setup key keep AEB8=0
   // until a MIDI key-signature control changes it; setup Key is not a
   // MIDI spelling-table selector.
   foreach(string setupKey in new[]{"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#"}){
   StartMusicInput();
   inputSetup.Key=setupKey;
   BeginInputStaff();
   for(int key=60;key<=71;key++){
    ReceiveMidiInput((uint)(0x90|key<<8|64<<16),36);
    ReceiveMidiInput((uint)(0x80|key<<8),36);
   }
   ReceiveMidiInput((uint)(0x90|43<<8|64<<16),36);
   if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms||
      musicInput.Lines[0]!="PC4/CS/D/EF/EN/F/FS/G/GS/A/BF/BN;")
    throw new Exception("F2 MIDI pitch capture with "+setupKey+" selected in staff setup differs.");
   AbandonMusicInput();
   LeaveMusicInput();
   }

   foreach(bool useMidi in new[]{false,true}){
    StartMusicInput();
    BeginInputStaff();
    Execute(useMidi?"PC4:E:G/C;":"TR/C4/D4/R/E4;");
    if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms)
     throw new Exception("Blank rhythm fixture did not reach rhythms.");
    byte[] pitchPreview=score.Encode();
    if(useMidi){
     foreach(int key in new[]{48,55})ReceiveMidiInput((uint)(0x90|key<<8|64<<16),36);
    }else Execute(";");
    if(musicInput.Transaction.Stage!=InputStaffStage.Marks||!score.Encode().SequenceEqual(pitchPreview))
     throw new Exception("Blank rhythms did not preserve the pitch preview.");
    Execute("G");
    if(!musicInput.SaveQuestion||!score.Encode().SequenceEqual(pitchPreview))
     throw new Exception("Blank rhythm staff changed before the save prompt.");
    AbandonMusicInput();
    LeaveMusicInput();
   }
  }
  public void VerifyMusicInput(string evidence){
   unsavedDecision=()=>DialogResult.No;
   VerifyMidiInput();
   foreach(bool chord in new[]{false,true}){
    if(musicInput!=null)LeaveMusicInput();var document=NativeMeasurements.New(source);document.Items.Clear();SetDocument(document,"");inputSetup=new InputSetupState();dirty=false;StartMusicInput();if(functionKeys.Count(b=>b.Visible)!=10)throw new Exception("Staff setup function menu");BeginInputStaff();string[] lines=chord?new[]{"TR/C4:E4:G4/C4:E4:G4;","Q/Q;","P 1;",";","1 2;"}:new[]{"TR/C4/D4/E4/F4;","E/E/E/E;",";","1 4;","1 4;"};string folder=Path.Combine(evidence,chord?"chord-phrase-original":"phrase-original");string[] labels={"pitches","rhythms","marks","beams","slurs"};
    for(int i=0;i<lines.Length;i++){Execute(lines[i]);Application.DoEvents();CompareInputItems(score,Path.Combine(folder,labels[i]+"-items.bin"));}
    byte[] native=OriginalMusSave.Prepare(score,source).Encode(),original=File.ReadAllBytes(Path.Combine(folder,"original.MUS"));if(!native.SequenceEqual(original))throw new Exception("Windows input transaction MUS differs");if(!musicInput.SaveQuestion)throw new Exception("Missing input-save question");Execute("N");if(inputSetup.Staff!=2||musicInput.Transaction!=null)throw new Exception("Next staff setup transition");float retainedStaff=BitConverter.ToSingle(File.ReadAllBytes(Path.Combine(folder,"after-save-ds.bin")),0x8780);if(originalNotationStaff!=retainedStaff)throw new Exception("F2 changed the retained Main notation staff before the next staff was accepted.");LeaveMusicInput();dirty=false;
   }
   var empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=2};StartMusicInput();BeginInputStaff();foreach(string line in new[]{"TR/C4;","Q;","P  1;","G"})Execute(line);byte[] transcript=EncodeInputTranscript();string measured=Path.Combine(evidence,"function-controls-original","original-transcript.txt");if(File.Exists(measured)&&!transcript.SequenceEqual(File.ReadAllBytes(measured)))throw new Exception("Input transcript bytes differ");Execute("Y");if(!musicInput.FilenameQuestion)throw new Exception("Filename prompt missing");string target=Path.Combine(evidence,"native-transcript-"+Guid.NewGuid().ToString("N"));Execute(target);if(!File.ReadAllBytes(target).SequenceEqual(transcript)||inputSetup.Staff!=3)throw new Exception("Input-file save transition");LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();Execute("TR/C4;");EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.Y));if(inputSetup.Staff!=2||score.Items.Count!=1||score.Items[0].Code!=8)throw new Exception("Abandon did not preserve only staff setup");LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();Execute("TR/C4;");MusicInputFunction(7,Keys.None);MusicInputFunction(8,Keys.None);MusicInputFunction(7,Keys.Shift);MusicInputFunction(7,Keys.Alt);MusicInputFunction(8,Keys.Alt);if(command.Text!="32/64/32./tt/12:1")throw new Exception("Measured function-key text differs: "+command.Text);AbandonMusicInput();LeaveMusicInput();
   foreach(bool proportional in new[]{false,true}){empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=proportional?7:6,Size=1.05f,Spacing=proportional?0:-1};StartMusicInput();BeginInputStaff();foreach(string line in proportional?new[]{"TR/C4/D4;","Q/H;","G"}:new[]{"TR/C4;","Q;","G"})Execute(line);if(!EncodeInputTranscript().SequenceEqual(File.ReadAllBytes(Path.Combine(evidence,proportional?"proportional-transcript-original":"fractional-transcript-original","original-transcript.txt"))))throw new Exception("Original IN transcript differs");AbandonMusicInput();LeaveMusicInput();}
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();EscapeMusicInput();if(inputSetup.Staff!=2||musicInput.Transaction!=null||score.Items.Count!=1)throw new Exception("Empty pitch Escape differs");LeaveMusicInput();
   foreach(string control in new[]{"D at pitches","D at rhythms","D at marks","I"}){
    empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
    bool deletePitches=control=="D at pitches";
    Execute("TR/C4/D4;");Execute("Q/Q;");
    if(!deletePitches)Execute("S 1;");
    EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
    int row=deletePitches?2:control=="D at rhythms"?3:4;
    for(int i=0;i<row;i++)SubmitInputCorrection("");
    if(control.StartsWith("D",StringComparison.Ordinal)){
     SubmitInputCorrection("D");
     if(musicInput.Correction!=-1||musicInput.Transaction.Stage!=(deletePitches?InputStaffStage.Rhythms:InputStaffStage.Marks)
      ||score.Items.Count!=4||score.Items.Any(item=>item.P(11)!=0))throw new Exception("F2 correction delete differs from original");
     if(deletePitches&&(score.Items[1].P(3)!=51.125f||score.Items[2].P(3)!=100.75f||score.Items[3].P(3)!=150.375f
      ||score.Items.Skip(2).Any(item=>item.P(7)!=0)))throw new Exception("F2 pitch-row delete preview differs from original");
    }else{
     SubmitInputCorrection(control);
     if(musicInput.Correction!=4||command.Text!="I")throw new Exception("Original I retains the current correction row");
     SubmitInputCorrection("P 2;");
     if(musicInput.Correction!=5||command.Text!="S 1;"||score.Items.Count!=4||score.Items[2].P(11)!=7)throw new Exception("F2 correction insertion did not shift the original row");
     SubmitInputCorrection(";");
     if(musicInput.Correction!=-1||musicInput.Transaction.Stage!=InputStaffStage.Slurs||score.Items.Count!=5)throw new Exception("F2 inserted mark did not advance to slurs");
     var mark=score.Items[4];
     if(mark.Code!=9||mark.P(3)!=105.25f||mark.P(4)!=-3||mark.P(5)!=53||mark.P(6)!=1)throw new Exception("F2 inserted mark differs from original");
    }
    AbandonMusicInput();LeaveMusicInput();
   }
   foreach(int row in new[]{4,5}){
    empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
    foreach(string line in new[]{"TR/C4/D4;","E/E;","S 1;","1 2;"})Execute(line);
    byte[] before=score.Encode();
    foreach(int goRow in new[]{0,1}){
     EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
     if(goRow==1)SubmitInputCorrection("");
     SubmitInputCorrection("G");
     if(musicInput.Transaction.Stage!=InputStaffStage.Slurs||!before.SequenceEqual(score.Encode()))
      throw new Exception("F2 Go from a setup row changed the accepted staff");
    }
    EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
    for(int i=0;i<row;i++)SubmitInputCorrection("");
    SubmitInputCorrection("D");
    if(musicInput.Transaction.Stage!=InputStaffStage.Beams||score.Items.Count!=4||score.Items[2].P(11)!=7)
     throw new Exception("F2 correction did not retain the mark and reopen beams");
    Execute("G");
    if(System.Text.Encoding.ASCII.GetString(EncodeInputTranscript())!="IN1 0 0 1 200\r\n0 200\r\nTR/C4/D4;\r\nE/E;\r\nS 1;\r\nG\r\n")
     throw new Exception("F2 deletion transcript differs from original");
    AbandonMusicInput();LeaveMusicInput();
   }
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
   foreach(string line in new[]{"TR/C4/D4;","Q/Q;","P 2;",";"})Execute(line);
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("");SubmitInputCorrection("12 160");
   if(score.Items[1].P(3)!=1.5f)throw new Exception("F2 position draft moved the accepted preview");
   for(int i=0;i<4;i++)SubmitInputCorrection("");
   if(musicInput.Transaction.Stage!=InputStaffStage.Slurs||score.Items.Count!=5||score.Items[0].P(6)!=0
    ||score.Items[1].P(3)!=12||score.Items[2].P(3)!=21||score.Items[3].P(3)!=90.5f||score.Items[4].P(3)!=90.5f)
    throw new Exception("F2 position correction differs from original");
   Execute(";");
   if(System.Text.Encoding.ASCII.GetString(EncodeInputTranscript())!="IN1 0 0 1 200\r\n12 160\r\nTR/C4/D4;\r\nQ/Q;\r\nP 2;\r\n;\r\n;\r\n")
    throw new Exception("F2 corrected transcript bytes differ from original");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
   Execute("TR/E4/F4;");Execute("Q/Q;");
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("IN1 0 0 1 180");SubmitInputCorrection("12 160");
   SubmitInputCorrection("");SubmitInputCorrection("");Execute("G");
   if(score.Items.Count!=4||score.Items[0].P(6)!=0||score.Items[1].P(3)!=12
    ||score.Items[2].P(3)!=21||score.Items[3].P(3)!=90.5f
    ||System.Text.Encoding.ASCII.GetString(EncodeInputTranscript())!="IN1 0 0 1 180\r\n12 160\r\nTR/E4/F4;\r\nQ/Q;\r\nG\r\n")
    throw new Exception("F2 corrected IN header differs from original");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
   foreach(string line in new[]{"TR/C4/D4/E4/F4;","Q/E/E/H;","G","N"})Execute(line);
   inputSetup.Spacing=1;BeginInputStaff();Execute("TR/E4/F4/G4/A4;");Execute("Q/Q/Q/Q;");
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("");
   if(command.Text!="SP1")throw new Exception("F2 linked correction lacks the SP row");
   SubmitInputCorrection("");SubmitInputCorrection("");SubmitInputCorrection("E/E/E/E;");
   if(!musicInput.SpacingPause||score.Items.Count!=12||score.Items[7].P(3)!=34.583332f||score.Items[8].P(7)!=0)
    throw new Exception("F2 linked correction warning preview differs from original");
   MusicInputKey(new KeyEventArgs(Keys.Enter));
   if(musicInput.SpacingPause||musicInput.Transaction.Stage!=InputStaffStage.Marks||score.Items[7].P(3)!=1.5f
    ||score.Items[8].P(3)!=10.5f||score.Items[9].P(3)!=57.876f||score.Items[10].P(3)!=105.251f
    ||score.Items[11].P(3)!=152.626f||score.Items.Skip(8).Any(item=>item.P(7)!=.5f))
    throw new Exception("F2 linked correction after acknowledgment differs from original");
   Execute("G");
   if(System.Text.Encoding.ASCII.GetString(EncodeInputTranscript())!="IN2 0 0 1 200\r\nSP1\r\nTR/E4/F4/G4/A4;\r\nE/E/E/E;\r\nG\r\n")
    throw new Exception("F2 linked correction transcript differs from original");
   Execute("N");BeginInputStaff();
   foreach(string line in new[]{"TR/C4/D4/E4/F4;","Q/Q/Q/Q;","S 1;"})Execute(line);
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   for(int i=0;i<3;i++)SubmitInputCorrection("");
   SubmitInputCorrection("E/E/E/E;");SubmitInputCorrection("");
   if(musicInput.SpacingPause||musicInput.Transaction.Stage!=InputStaffStage.Beams||score.Items.Count!=18
    ||score.Items[14].P(11)!=7||score.Items.Skip(14).Any(item=>item.P(7)!=.5f))
    throw new Exception("F2 linked correction did not replay the accepted mark");
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("IN3 0 0 0.8 200");
   for(int i=0;i<4;i++)SubmitInputCorrection("");
   Execute("G");
   if(score.Items[12].P(5)!=0||System.Text.Encoding.ASCII.GetString(EncodeInputTranscript())!="IN3 0 0 0.8 200\r\nSP1\r\nTR/C4/D4/E4/F4;\r\nE/E/E/E;\r\nS 1;\r\nG\r\n")
    throw new Exception("F2 corrected linked size differs from original");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();Execute("TR/C4/D4;");
   if(!MusicInputMenuButton(3,MouseButtons.Right)||command.Text!="h.")throw new Exception("F2 right-click rhythm differs from original");
   command.Clear();
   if(!MusicInputMenuButton(3,MouseButtons.Middle)||command.Text!="th")throw new Exception("F2 middle-click rhythm differs from original");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
   ClickInputTest(1,200,300);
   if(command.TextLength!=0)throw new Exception("F2 out-of-range mouse pitch changed the line");
   ClickInputTest(1,200,440,425);ClickInputTest(2,200,435);
   if(command.Text!="C4/D4")throw new Exception("F2 mouse release pitches differ from original");
   command.Clear();
   foreach(int button in new[]{4,7,5,6,2,3,1})ClickInputTest(button,200,430);
   if(command.Text!="FN4/F4/FS4/FF4/F4:F4")throw new Exception("F2 default-staff mouse button sequence differs from original");
   Execute("TR/C4/D4;");Execute("E/E;");
   ClickInputTest(1,187,300);
   if(command.Text!="1.48 ")throw new Exception("F2 fractional mark click differs from original");
   command.Text="S ";command.SelectionStart=command.TextLength;
   ClickInputTest(4,52,439);
   if(musicInput.Transaction.Stage!=InputStaffStage.Beams||score.Items[2].P(11)!=7)throw new Exception("F2 mouse mark was not submitted");
   ClickInputTest(1,187,300);ClickInputTest(4,336,437);
   if(musicInput.Transaction.Stage!=InputStaffStage.Slurs||score.Items.Last().Code!=6)throw new Exception("F2 mouse beam was not submitted");
   ClickInputTest(1,187,300);ClickInputTest(4,336,437);
   if(!musicInput.SaveQuestion||score.Items.Last().Code!=5)throw new Exception("F2 mouse slur was not submitted");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
   Execute("TR/C4:E4/R/G4/A4;");Execute("Q/Q/Q/Q;");
   ClickInputTest(1,52,300);ClickInputTest(1,194,300);ClickInputTest(1,336,300);ClickInputTest(1,478,300);
   if(command.Text!="1 1.50 2 3 ")throw new Exception("F2 mouse endpoints counted a rest or chord secondary");
   command.Text="S ";command.SelectionStart=command.TextLength;
   ClickInputTest(4,336,300);
   if(musicInput.Transaction.Stage!=InputStaffStage.Beams||score.Items[5].P(11)!=7||score.Items[4].Code!=2)
    throw new Exception("F2 mouse mark was not applied to the note after a rest");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=4};StartMusicInput();BeginInputStaff();
   Execute("TR/C4/D4;");Execute("Q/Q;");EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("IN9 12 3 0.8 180");
   for(int i=0;i<3;i++)SubmitInputCorrection("");
   if(inputSetup.Staff!=4||musicInput.Transaction.Stage!=InputStaffStage.Marks||score.Items.Count!=5
    ||score.Items[0].Code!=8||score.Items[0].Staff!=4||score.Items[1].Code!=8||score.Items[1].Staff!=9
    ||score.Items[1].P(3)!=12||score.Items[1].P(4)!=3||score.Items[1].P(5)!=.8f||score.Items[1].P(6)!=180
    ||score.Items[2].P(3)!=1.5f||score.Items[3].P(3)!=8.70000076293945f||score.Items[4].P(3)!=104.350006103516f
    ||score.Items.Skip(2).Any(item=>item.Staff!=9))throw new Exception("F2 IN destination correction differs from original");
   Execute("G");
   if(System.Text.Encoding.ASCII.GetString(EncodeInputTranscript())!="IN9 12 3 0.8 180\r\n0 200\r\nTR/C4/D4;\r\nQ/Q;\r\nG\r\n")
    throw new Exception("F2 IN destination transcript differs from original");
   Execute("N");if(inputSetup.Staff!=10)throw new Exception("F2 IN destination next staff differs from original");
   BeginInputStaff();Execute("TR/C4/D4;");Execute("Q/Q;");EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("IN9 5 7 0.7 170");
   for(int i=0;i<3;i++)SubmitInputCorrection("");
   if(score.Items.Count!=9||score.Items.Count(item=>item.Staff==9&&item.Code==8)!=1
    ||score.Items[1].P(3)!=12||score.Items[1].P(4)!=3||score.Items[1].P(5)!=.8f||score.Items[1].P(6)!=180
    ||!score.Items.Skip(6).SelectMany(item=>item.Bytes).SequenceEqual(score.Items.Skip(2).Take(3).SelectMany(item=>item.Bytes)))
    throw new Exception("F2 IN existing destination geometry or replay differs from original");
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   SubmitInputCorrection("IN11 0 0 1 200");
   for(int i=0;i<3;i++)SubmitInputCorrection("");
   if(score.Items.Count!=10||score.Items.Count(item=>item.Staff==9)!=4||score.Items[5].Staff!=10
    ||score.Items[6].Code!=8||score.Items[6].Staff!=11||score.Items[6].P(5)!=1||score.Items[6].P(6)!=200
    ||score.Items.Skip(7).Any(item=>item.Staff!=11)||score.Items[8].P(3)!=10.5f||score.Items[9].P(3)!=105.25f)
    throw new Exception("F2 repeated IN destination replay differs from original");
   Execute("G");Execute("N");
   if(inputSetup.Staff!=12)throw new Exception("F2 repeated IN destination next staff differs from original");
   LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=5};StartMusicInput();BeginInputStaff();
   foreach(string expected in new[]{"BA/D2/F2/G2/A2","AL/C3/E3/F3/G3","TE/A2/C3/D3/E3","TR/B3/D4/E4/F4"}){
    command.Text=expected.Substring(0,3);command.SelectionStart=command.TextLength;
    foreach(int y in new[]{190,185,180,175})ClickInputTest(1,200,y);
    if(command.Text!=expected)throw new Exception("F2 clef mouse pitches differ from original: "+expected);
   }
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=6,Size=.8f};StartMusicInput();BeginInputStaff();
   command.Text="TR/";command.SelectionStart=command.TextLength;
   foreach(int y in new[]{125,120,115,110})ClickInputTest(1,200,y);
   ClickInputTest(3,200,110);ClickInputTest(1,200,115);
   if(command.Text!="TR/B3/D4/F4/A4:F4")throw new Exception("F2 scaled staff mouse pitches or chord differ from original");
   foreach(int button in new[]{5,4,6,7})ClickInputTest(button,200,115);
   if(command.Text!="TR/B3/D4/F4/A4:F4/FS4/FN4/FF4/F4")throw new Exception("F2 mouse accidental buttons differ from original");
   NativeDosViewport.ZoomTo(2,0,65);
   command.Text="TR/";command.SelectionStart=command.TextLength;
   foreach(int y in new[]{223,218,213})ClickInputTest(1,200,y);
   if(command.Text!="TR/C4/D4/D4")throw new Exception("F2 zoomed mouse pitches differ from original");
   foreach(string clef in new[]{"BA","AL","TE","TR"}){
    command.AppendText("/"+clef+"/");
    ClickInputTest(1,200,218);
   }
   if(command.Text!="TR/C4/D4/D4/BA/F2/AL/E3/TE/C3/TR/D4")
    throw new Exception("F2 mid-line clef mouse pitches differ from original");
   command.Text="TR";command.SelectionStart=command.TextLength;
   foreach(string control in new[]{"P","SU","O"}){
    command.AppendText("/"+control+"/");
    ClickInputTest(1,200,218);
   }
   if(command.Text!="TR/P/D4/SU/D4/O/D4")
    throw new Exception("F2 mouse pitches mixed with proximity controls differ from original");
   Execute("TR/C4/P/D/SU/D/O/D4;");
   if(musicInput.Transaction.Stage!=InputStaffStage.Pitches||musicInput.InputError==null||score.Items.Count!=1)
    throw new Exception("F2 standalone P was not rejected");
   Execute("TR/C4/PD/SU/D/OD4;");
   if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms||musicInput.Transaction.RhythmCount!=4
    ||score.Items.Count!=6||!score.Items.Skip(2).Select(item=>item.P(4)).SequenceEqual(new[]{1f,2f,2f,2f}))
    throw new Exception("F2 attached proximity controls differ from original");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=6,Size=.8f};StartMusicInput();BeginInputStaff();
   Execute("TR/C4/P/D/OD4;");
   if(command.SelectionStart!=7||command.SelectionLength!=0||!musicInput.ProximityAfterError
    ||musicInput.Transaction.Stage!=InputStaffStage.Pitches)
    throw new Exception("F2 rejected proximity line did not retain the original delimiter cursor");
   Execute(command.Text.Remove(command.SelectionStart,1));
   if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms||musicInput.Transaction.RhythmCount!=3
    ||!score.Items.Skip(2).Select(item=>item.P(4)).SequenceEqual(new[]{1f,2f,2f}))
    throw new Exception("F2 repaired proximity line differs from original");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=6,Size=.8f};StartMusicInput();BeginInputStaff();
   Execute("TR/C4/P/B;");
   if(command.SelectionStart!=7||!musicInput.ProximityAfterError)throw new Exception("F2 proximity retry did not retain the original state");
   Execute(command.Text.Remove(command.SelectionStart-1,1));
   if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms||musicInput.Transaction.RhythmCount!=3
    ||musicInput.Lines[0]!="TR/C4//B;"||!score.Items.Skip(2).Select(item=>item.P(4)).SequenceEqual(new[]{1f,1f,0f}))
    throw new Exception("F2 proximity retry without P differs from original");
   byte[] proximityPreview=score.Encode();
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));SubmitInputCorrection("");SubmitInputCorrection("G");
   if(musicInput.Transaction.Stage!=InputStaffStage.Rhythms||!proximityPreview.SequenceEqual(score.Encode()))
    throw new Exception("F2 correction replay lost the retained proximity state");
   AbandonMusicInput();LeaveMusicInput();
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState{Staff=6,Size=.8f};StartMusicInput();BeginInputStaff();
   foreach(string line in new[]{"BA/C3;","Q;","G","N"})Execute(line);
   ApplyInputField("6");BeginInputStaff();
   foreach(int y in new[]{125,120,115,110})ClickInputTest(1,200,y);
   if(command.Text!="D2/F2/A2/C3")throw new Exception("F2 mouse pitch did not use the inherited bass clef");
   Execute(command.Text+"/TR/D4;");
   foreach(string line in new[]{"Q/Q/Q/Q/Q;","G","N"})Execute(line);
   ApplyInputField("6");BeginInputStaff();
   ClickInputTest(1,200,120);ClickInputTest(1,200,115);
   if(command.Text!="D4/F4")throw new Exception("F2 mouse pitch did not use the last inherited clef");
   command.Clear();
   MusicInputFunction(2,Keys.None);
   if(!MusicInputMenuButton(2,MouseButtons.Right)||!MusicInputMenuButton(2,MouseButtons.Middle)||command.Text!="r/r/r")
    throw new Exception("F2 pitch menu mouse buttons differ from original");
   AbandonMusicInput();LeaveMusicInput();
   for(int key=2;key<=10;key++){
    empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
    Execute("TR/C4/D4;");Execute("E/E;");
    MusicInputFunction(2,Keys.None);
    if(!MusicInputMenuButton(2,MouseButtons.Right)||!MusicInputMenuButton(2,MouseButtons.Middle)||command.Text!="p /p /p ")
     throw new Exception("F2 mark menu separators differ from original");
    Execute("S 1;");
    MouseButtons button=key==3?MouseButtons.Right:key==10?MouseButtons.Middle:MouseButtons.Left;
    string token=key==10?"ab;":key+"b;";
    if(button==MouseButtons.Left)MusicInputFunction(key,Keys.None);
    else if(!MusicInputMenuButton(key,button))throw new Exception("F2 automatic beam menu button was ignored");
    if(musicInput.Transaction.Stage!=InputStaffStage.Slurs||musicInput.Lines.Last()!=token||score.Items.Last().Code!=6)
     throw new Exception("F2 automatic beam menu did not submit the original token");
    AbandonMusicInput();LeaveMusicInput();
   }
   empty=NativeMeasurements.New(source);empty.Items.Clear();SetDocument(empty,"");inputSetup=new InputSetupState();StartMusicInput();BeginInputStaff();
   foreach(string line in new[]{"TR/C4/D4/E4/F4/G4/A4;","EX6;",";"})Execute(line);
   command.Text="1 2";command.SelectionStart=command.TextLength;
   MusicInputFunction(2,Keys.None);
   var automaticBeams=score.Items.Where(item=>item.Code==6).ToArray();
   if(musicInput.Transaction.Stage!=InputStaffStage.Slurs||musicInput.BeamMenuEcho!="1 22b;"
    ||musicInput.Lines.Last()!="2b;"||automaticBeams.Length!=3
    ||!automaticBeams.Select(item=>item.P(3)).SequenceEqual(new[]{10.5f,73.666664f,136.833328f})
    ||!automaticBeams.Select(item=>item.P(6)).SequenceEqual(new[]{42.083332f,105.25f,168.416656f}))
    throw new Exception("F2 automatic beam menu with existing text differs from original");
   byte[] automaticPreview=score.Encode();
   EscapeMusicInput();MusicInputKey(new KeyEventArgs(Keys.N));
   for(int i=0;i<5;i++)SubmitInputCorrection("");
   if(!command.Text.Equals("2b;",StringComparison.OrdinalIgnoreCase))throw new Exception("F2 beam correction retained the discarded draft");
   SubmitInputCorrection("");
   if(!automaticPreview.SequenceEqual(score.Encode())||musicInput.BeamMenuEcho!=null)
    throw new Exception("F2 beam correction differs from original automatic preview");
   AbandonMusicInput();LeaveMusicInput();
   using(var label=new NativeTextLabel{Wrapped=true,Font=command.Font,Text=String.Join(" ",Enumerable.Repeat("A native warning uses the available text pane width.",10))}){if(label.ContentHeight(1100)>=label.ContentHeight(550))throw new Exception("Native messages still wrap at fixed columns");}int boundary=top.Height;Message(String.Join("\r\n",Enumerable.Repeat("A separate warning.",20)));LayoutTextContents(true);if(top.Height!=boundary)throw new Exception("Messages moved graphics boundary");
   SetDocument(Score.Load(Path.Combine(source,"MUSDAT","MOZART.MUS")),"");var word=score.Items.First(x=>x.Code==16);BeginEdit(word);LayoutClassic();if(itemTextLine.Top!=parameters.Bottom)throw new Exception("Word text is not beneath parameter rows");if(!itemTextLine.Visible||itemTextLine.Text!=word.Text.TrimEnd('\0',' '))throw new Exception("Raw word text not shown below parameters");ExitEdit(false);
   Console.WriteLine("PASS Windows five-stage input: original compact previews and MUS files, correction replay and transcripts, function keys and mouse entry, and staff transitions.");
  }
 }
 public static class NativeInputChecks {
  public static int Run(string[] args){try{using(var editor=new Editor(args[1],"")){editor.Show();Application.DoEvents();editor.VerifyMusicInput(args[2]);editor.Close();}return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 }
}
