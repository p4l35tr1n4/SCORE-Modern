using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ScoreModern {
 public sealed partial class Editor {
  string pendingAppendPath;
  Score pendingAppendScore;
  byte[] pendingAppendBaseline;

  bool ExecuteMainLetterCommand(string verb,string arguments){
   if(scor4Interface){
    switch(verb){
     case "DIRS": case "DIRSET": RequireNoLetterArguments(verb,arguments);RefreshScor4Files();return true;
     case "OP": RequireNoLetterArguments(verb,arguments);OpenScor4FromMenu();return true;
     case "DB": case "DX": RequireNoLetterArguments(verb,arguments);StepScor4File(verb=="DB"?-1:1);return true;
     case "SNB": RequireNoLetterArguments(verb,arguments);SavePreviousScor4File();return true;
     case "DATE": RequireNoLetterArguments(verb,arguments);ShowScoreVersion();return true;
     case "RS": if(arguments.Length==0){RestartScor4Score();return true;}break;
    }
    short staff;
    if((verb=="CD"&&Int16.TryParse(arguments,NumberStyles.Integer,CultureInfo.InvariantCulture,out staff))||
       (verb.Length>2&&verb.StartsWith("CD",StringComparison.Ordinal)&&arguments.Length==0&&
        Int16.TryParse(verb.Substring(2),NumberStyles.Integer,CultureInfo.InvariantCulture,out staff))){
     CenterScor4GroupDashes(staff);return true;
    }
    if(verb=="CD"&&arguments.Length==0){CenterScor4GroupDashes(0);return true;}
   }
   if(verb.Length>3&&verb.StartsWith("DPY",StringComparison.Ordinal)&&(Char.IsDigit(verb[3])||verb[3]=='-'||verb[3]=='+')){arguments=verb.Substring(3)+(arguments.Length==0?"":" "+arguments);verb="DPY";}
   if(scor4Interface&&verb=="GP"){
    RequireNoLetterArguments(verb,arguments);
    if(String.IsNullOrEmpty(scor4PreviousFile))Message("No previous file.");
    else OpenScor4File(scor4PreviousFile);
    return true;
   }
   if(scor4Interface&&verb=="RSM"){
    RequireNoLetterArguments(verb,arguments);
    RestartScor4SameFile();
    return true;
   }
   if(scor4Interface&&verb=="PA"){
    RequireNoLetterArguments(verb,arguments);
    ChangeScor4Path();
    return true;
   }
   if(ExecuteMainItemOrder(verb,arguments))return true;
   switch(verb){
    case "M":case "C":case "A":case "DE":case "CS":case "MS":
     StartMainGroup(verb,arguments);
     return true;
    case "VER":
     RequireNoLetterArguments(verb,arguments);
     ShowScoreVersion();
     return true;
    case "CD":
     if(arguments.Length!=0)Directory.SetCurrentDirectory(Path.GetFullPath(arguments));
     Message("");
     return true;
    case "DIR":
     ShowScoreDirectory(arguments);
     return true;
    case "DEL":
     StartDiskDelete(arguments);return true;
    case "DRV":
     RequireNoLetterArguments(verb,arguments);StartDriveChange();return true;
    case "TYPE":
     if(arguments.Length==0&&NativeDiskDialogChoice.Files){arguments=ChooseCommandFile("Open text listing",false,TextFileFilter);if(arguments==null)return true;}
     StartTypeListing(arguments);return true;
    case "DPY":
     StartDrawExport(arguments);return true;
    case "PMX":
     RequireNoLetterArguments(verb,arguments);pendingParameterExport=true;if(NativeDiskDialogChoice.Files){string chosen=ChooseCommandFile("Save score parameters",true,TextFileFilter);if(chosen!=null)return SubmitParameterExport(chosen);pendingParameterExport=false;return true;}Message("Enter filename ->");return true;
    case "PM":
     RequireNoLetterArguments(verb,arguments);
     ShowScoreParameters();
     return true;
    case "PR":
     RequireNoLetterArguments(verb,arguments);EnterOriginalPrintMenu();return true;
    case "SNX":
     RequireNoLetterArguments(verb,arguments);SaveNextScoreFile();return true;
    case "NX":case "NB":
     RequireNoLetterArguments(verb,arguments);
     StepScoreFile(verb=="NX");
     return true;
    case "GM":
     if(arguments.Length==0&&NativeDiskDialogChoice.Files){arguments=ChooseWindowsDiskFile("GM");if(arguments==null)return true;}
     string path=Resolve(arguments,Directory.GetCurrentDirectory());
     if(!File.Exists(path)){
      ShowLetterListing("File not found - "+Path.GetFileName(path)+"\r\nPress any key to continue.");
      return true;
     }
     pendingAppendPath=path;
     Message("Type a number for staff offset or press <Enter> ->");
     return true;
    default:return false;
   }
  }

  static void RequireNoLetterArguments(string verb,string arguments){
   if(arguments.Length!=0)throw new InvalidDataException(verb+" does not take arguments.");
  }

  void ShowScoreVersion(){
   uint serial=NativeScoreIdentity.Require(source,false).Serial;
   var culture=CultureInfo.InvariantCulture;
   string version=scor4Interface?"4.0100":"3.1100";
   string text=String.Format(culture,"Version =   {0}  --  Serial # = {1}",version,serial);
   if(score.Trailer.Length==24&&!String.IsNullOrEmpty(current)&&File.Exists(current)){
    uint creator=BitConverter.ToUInt32(score.Trailer,4);
    if(creator>=1000000&&creator<=9999999)
     text+="\r\n"+String.Format(culture,"This file created by Serial # = {0}{1}File created: {2:MM-dd-yy   HH:mm}",
      creator,scor4Interface?" ═══════ ":"        ",File.GetLastWriteTime(current));
   }
   Message(text+"\r\n"+BuildVersion.DisplayVersion);
  }

  void ShowScoreDirectory(string arguments){
   string pattern=arguments.Length==0?"*.*":arguments;
   string directory=Path.GetDirectoryName(pattern);
   string folder=String.IsNullOrEmpty(directory)?Directory.GetCurrentDirectory():Path.GetFullPath(directory);
   string filter=Path.GetFileName(pattern);
   if(String.IsNullOrEmpty(filter))throw new InvalidDataException("Enter a directory search pattern.");
   var names=Directory.Exists(folder)?Directory.GetFileSystemEntries(folder,filter)
    .Select(Path.GetFileName).Select(name=>name.ToUpperInvariant()).ToList():new System.Collections.Generic.List<string>();
   if(filter=="*.*"&&Directory.Exists(folder))names.InsertRange(0,new[]{".",".."});
   var lines=new System.Collections.Generic.List<string>{"Directory of "+pattern.ToUpperInvariant()};
   for(int i=0;i<names.Count;i+=6)
    lines.Add(String.Concat(names.Skip(i).Take(6).Select(name=>name.PadRight(Math.Max(13,name.Length+1)))).TrimEnd());
   if(names.Count==0)lines.Add("File not found.");
   if(ShowCommandList("Directory",new[]{lines.ToArray()}))return;
   lines.Add("Press any key to continue.");
   ShowLetterListing(String.Join("\r\n",lines));
  }

  void ShowLetterListing(string text){
   pendingEditAcknowledgement=true;
   Message(text);
   textWindow.AutoScrollPosition=System.Drawing.Point.Empty;
   command.Focus();
  }

  void ShowScoreParameters(){
   if(score.Items.Any(item=>item.Code!=16&&item.Count>20))throw new NotSupportedException("PM records above20 parameters require verification; the measured30-parameter case reaches an original console-write error.");
   var pages=new System.Collections.Generic.List<string[]>();var page=new System.Collections.Generic.List<string>();int row=0;
   for(int index=0;index<score.Items.Count;index++){
    string formatted=FormatScoreParameterRow(score.Items[index],index+1);string[] lines=formatted.Split(new[]{"\r\n","\n"},StringSplitOptions.None);page.AddRange(lines);foreach(string line in lines)row+=1+line.Length/80;
    // Original3054:11EF queries BIOS cursor DX and compares with1400h.
    // The native display uses full-width logical rows; only pagination
    // retains the original80-column cursor accounting.
    if(row>=20){page.Add("Press any key to continue.");pages.Add(page.ToArray());page.Clear();row=0;}
   }
   page.Add("Press any key to continue.");pages.Add(page.ToArray());
   if(ShowCommandList("Score parameters",pages))return;
   letterHelpPages=pages;letterHelpIsType=false;letterHelpIsParameters=true;letterHelpPage=0;letterHelpTopicPending=false;letterHelpPreviousMessage="";ShowLetterHelpPage();
  }

  static readonly string[] OriginalParameterLabels={"","NOTE ","REST ","CLEF ","LINE ","SLUR ","BEAM ","TRILL","STAFF","MISC ","NUMB ","LIBRY","CIRCL","DUMMY","BARS ","POST ","WORD ","KSIG ","METER","MPUMX"};
  static string FormatScoreParameterRow(Item item,int number){
   if(item.Code<1||item.Code>=OriginalParameterLabels.Length)throw new NotSupportedException("PM item label is outside the original table.");
   var row=new StringBuilder(number.ToString(CultureInfo.InvariantCulture).PadLeft(3)).Append(") ").Append(OriginalParameterLabels[item.Code]);
   if(item.Code==16||item.Code==15||item.Code==11){
    row.Append((ExportFixed(item.P(1),0)+".").PadLeft(4)).Append(ExportFixed(item.P(2),1).PadLeft(5));
    for(int p=3;p<=Math.Min(item.Count,12);p++)row.Append(ExportFixed(item.P(p),p==3?3:2).PadLeft(p==3?8:9));
    string text=item.Code==16?item.Text:Encoding.GetEncoding(437).GetString(item.Bytes,56,Math.Min(item.Code==11?5:24,Math.Max(0,item.Bytes.Length-56)));
    return row.Append("\r\n").Append(text).ToString();
   }
   row.Append((ExportFixed(item.P(1),0)+".").PadLeft(4)).Append(ExportFixed(item.P(2),1).PadLeft(4));
   int count=item.Code==16?Math.Min(item.Count,11):item.Count;
   for(int p=3;p<=count;p++){int places=p==3||p==6?3:p==7?4:2,width=p==3?8:p==6?10:p==7?11:p<=13?9:8;row.Append(ExportFixed(item.P(p),places).PadLeft(width));}
   if(item.Code==16)row.Append("\r\n").Append(item.Text);
   return row.ToString();
  }

  bool SubmitScoreAppend(string text){
   if(pendingAppendPath==null)return false;
   int offset=0;
   if(!String.IsNullOrWhiteSpace(text)&&!Int32.TryParse(text.Trim(),NumberStyles.Integer,CultureInfo.InvariantCulture,out offset))
    throw new InvalidDataException("Enter an integer staff offset.");
   string path=pendingAppendPath;
   pendingAppendPath=null;
   if(!File.Exists(path)){
    ShowLetterListing("File not found - "+Path.GetFileName(path)+"\r\nPress any key to continue.");
    return true;
   }
   var added=PrepareOriginalLoadedScore(Score.Load(path));
   foreach(var item in added.Items){
    float destination=item.P(2)+offset;
    if(destination<1||destination>=33||Single.IsNaN(destination))
     throw new InvalidDataException("The staff offset would put an item outside staff 1–32.");
    if(offset!=0)item.Set(2,destination);
   }
   var next=score.Clone();
   next.Items.AddRange(added.Items);
   next.Encode();
   using(canvas.TextLayout.PreserveAlignmentMetricHistory())using(var probe=new Canvas()){probe.Score=next;probe.Library=canvas.Library;probe.TextLayout=canvas.TextLayout;probe.LayoutScore();}
   var occupied=score.Items.Where(item=>item.Code==8).Select(item=>item.Staff);
   if(added.Items.Where(item=>item.Code==8).Select(item=>item.Staff).Intersect(occupied).Any()){
    pendingAppendScore=next;
    pendingAppendBaseline=score.Encode();
    Message("Warning: This action will cause existing staves to be overwritten.\r\nPress ESC to abort, any other key to continue");
   }else CommitScoreAppend(next);
   return true;
  }

  void ClearScoreAppend(){
   pendingAppendPath=null;
   pendingAppendScore=null;
   pendingAppendBaseline=null;
  }

  void CommitScoreAppend(Score next){
   Checkpoint();
   score=next;
   dirty=true;
   Message("");
   RefreshAll(null);
  }

  bool HandleScoreAppendKey(System.Windows.Forms.KeyEventArgs key){
   var code=key.KeyCode;
   if(pendingAppendScore!=null){
    if(code==System.Windows.Forms.Keys.ShiftKey||code==System.Windows.Forms.Keys.ControlKey||code==System.Windows.Forms.Keys.Menu)return false;
    var next=pendingAppendScore;
    bool unchanged=score.Encode().SequenceEqual(pendingAppendBaseline);
    ClearScoreAppend();
    if(code==System.Windows.Forms.Keys.Escape)Message("");
    else if(unchanged)CommitScoreAppend(next);
    else Message("The score changed while GM was waiting. Run GM again.");
    key.SuppressKeyPress=true;
    return true;
   }
   if(pendingAppendPath==null)return false;
   if(code==System.Windows.Forms.Keys.Escape){
    pendingAppendPath=null;
    command.Clear();
    Message("");
   }else if(!key.Control&&!key.Alt&&(code<System.Windows.Forms.Keys.F1||code>System.Windows.Forms.Keys.F24))return false;
   key.SuppressKeyPress=true;
   return true;
  }

  void SaveNextScoreFile(){if(String.IsNullOrEmpty(current)){StartLetterSave("SCOREAB.MUS");return;}string stem=Path.GetFileNameWithoutExtension(current),extension=Path.GetExtension(current);string name=OriginalFilenameSequence.Step(stem,extension,true,false,NativeScoreBehaviour.Enabled?(char?)null:'A');StartLetterSave(Path.Combine(Path.GetDirectoryName(current),name));}
  string originalSequenceFile,originalSequenceOwner;
  void StepScoreFile(bool forward){
   if(String.IsNullOrEmpty(current)){Message("Open a score in the file sequence first.");return;}
   string folder=Path.GetDirectoryName(current);
   string stem=Path.GetFileNameWithoutExtension(current);
   string extension=Path.GetExtension(current);
   if(stem.Length==0){Message("The current score has no filename stem.");return;}
   if(NativeScoreBehaviour.Enabled||stem[stem.Length-1]>='0'&&stem[stem.Length-1]<='9'){
    string previousSequence=originalSequenceFile;
    if(NativeScoreBehaviour.Enabled&&originalSequenceOwner==current&&!String.IsNullOrEmpty(originalSequenceFile)){
     folder=Path.GetDirectoryName(originalSequenceFile);stem=Path.GetFileNameWithoutExtension(originalSequenceFile);extension=Path.GetExtension(originalSequenceFile);
    }
    string target=OriginalFilenameSequence.Target(folder,stem,extension,forward);
    if(NativeScoreBehaviour.Enabled){originalSequenceFile=target;originalSequenceOwner=current;}
    if(!File.Exists(target)){Message("File not found - "+Path.GetFileName(target));return;}
    string prior=current;LoadFile(target);
    if(current==prior&&current!=target)originalSequenceFile=previousSequence;
    return;
   }
   char last=Char.ToUpperInvariant(stem[stem.Length-1]);
   if(last<'A'||last>'Z'||forward&&last=='Z'){
    Message("No verified "+(forward?"next":"previous")+" filename in this sequence.");
    return;
   }
   string next;
   if(!forward&&last=='A'){
    next=Directory.EnumerateFiles(folder).Where(path=>
      String.Equals(Path.GetExtension(path),extension,StringComparison.OrdinalIgnoreCase)&&
      Path.GetFileNameWithoutExtension(path).EndsWith("A",StringComparison.OrdinalIgnoreCase)&&
      StringComparer.OrdinalIgnoreCase.Compare(Path.GetFileNameWithoutExtension(path),stem)<0)
     .OrderByDescending(path=>Path.GetFileNameWithoutExtension(path),StringComparer.OrdinalIgnoreCase).FirstOrDefault();
   }else{
    char step=(char)(stem[stem.Length-1]+(forward?1:-1));
    next=Path.Combine(folder,stem.Substring(0,stem.Length-1)+step+extension);
   }
   if(next==null||!File.Exists(next)){
    Message(next==null?"No previous file in this sequence.":"File not found - "+Path.GetFileName(next));
    return;
   }
   LoadFile(next);
  }

  void CheckMainLetters(){
   foreach(var sample in new[]{new[]{"N4701","N4702.MUS","N4700.MUS"},new[]{"W47Z09","W47Z10.MUS","W47Z08.MUS"},new[]{"W47Z99","W48A00.MUS","W47Z98.MUS"},new[]{"W47Z00","W47Z01.MUS","W47Y99.MUS"},new[]{"J","I.MUS","I.MUS"},new[]{"T47A","T47B.MUS","T46Z.MUS"}}){
    if(OriginalFilenameSequence.Step(sample[0],".MUS",true,false)!=sample[1]||OriginalFilenameSequence.Step(sample[0],".MUS",false,false)!=sample[2])throw new Exception("Recovered filename step differs: "+sample[0]);
   }
   if(OriginalFilenameSequence.Step("W48A00",".MUS",true,true)!="W48A10.MUS"||OriginalFilenameSequence.Step("A46Z",".MUS",false,true)!="A46A.MUS"||OriginalFilenameSequence.Step("H",".MUS",false,true)!="A.MUS")throw new Exception("Original missing-file fallback differs.");
   string previousDirectory=Directory.GetCurrentDirectory();
   string folder=Path.Combine(Path.GetTempPath(),"score-letter-"+Guid.NewGuid().ToString("N"));
   var previousDecision=unsavedDecision;
   var previousScore=score;
   string previousFile=current;
   Directory.CreateDirectory(folder);
   try{
    string a=Path.Combine(folder,"SONAA.MUS"),b=Path.Combine(folder,"SONAB.MUS"),later=Path.Combine(folder,"SONBA.MUS");
    File.Copy(Path.Combine(source,"MUSDAT","PRACTICE.MUS"),a);
    File.Copy(Path.Combine(source,"MUSDAT","MOZART.MUS"),b);
    File.Copy(b,later);
    Execute("CD "+folder);
    if(Directory.GetCurrentDirectory()!=folder)throw new Exception("CD did not change the process working folder.");
    Execute("DIR *.MUS");
    if(!pendingEditAcknowledgement||!responseLine.Text.Contains("SONAA.MUS")||
       !responseLine.Text.Contains("SONBA.MUS"))throw new Exception("DIR listing differs.");
    Execute("");
    if(pendingEditAcknowledgement)throw new Exception("DIR acknowledgment did not return to Main.");
    SetDocument(NativeMeasurements.New(source),"");
    score.Items.Clear();
    score.Items.Add(Item.Make(8,1,0,0,1,200));
    score.Items.Add(Item.Make(1,1,50,7,10,0,1));
    Execute("PM");
    if(!responseLine.Text.Contains("  1) STAFF  8. 1.0    .000      .00     1.00   200.000")||
       !responseLine.Text.Contains("  2) NOTE   1. 1.0  50.000     7.00    10.00      .000     1.0000"))
     throw new Exception("PM rows differ from the DOS fixture.");
    Execute("");
    Execute("VER");
    if(!responseLine.Text.Contains("Version =   3.1100  --  Serial # = "+NativeScoreIdentity.Require(source,false).Serial))
     throw new Exception("VER identity differs.");
    SetDocument(Score.Load(a),a);
    Execute("NX");
    if(current!=b)throw new Exception("NX did not step the filename stem.");
    Execute("VER");
    uint producer=BitConverter.ToUInt32(score.Trailer,4);
    if(!responseLine.Text.Contains("This file created by Serial # = "+producer)||
       !responseLine.Text.Contains(File.GetLastWriteTime(b).ToString("MM-dd-yy   HH:mm",CultureInfo.InvariantCulture)))
     throw new Exception("VER file identity and DOS timestamp differ.");
    Execute("NB");
    if(current!=a)throw new Exception("NB did not step back.");
    SetDocument(Score.Load(later),later);
    Execute("NB");
    if(current!=a)throw new Exception("NB did not select the preceding A-ending stem.");
    score.Items.Add(Item.Make(2,1,20));
    unsavedDecision=()=>System.Windows.Forms.DialogResult.Cancel;
    Execute("NX");
    if(current!=a)throw new Exception("Cancelled NX replaced the open score.");
    string numericA=Path.Combine(folder,"wandh01.mus"),numericB=Path.Combine(folder,"wandh02.mus");
    File.Copy(a,numericA);File.Copy(b,numericB);
    SetDocument(Score.Load(numericA),numericA);Execute("NX");
    if(current!=numericB)throw new Exception("NX did not load the next numeric suffix in the same folder.");
    Execute("NX");
    if(current!=numericB)throw new Exception("A missing numeric successor replaced the open score.");
    Execute("NB");
    if(current!=numericA)throw new Exception("NB did not load the previous numeric suffix.");
    score.Items.Add(Item.Make(2,1,20));Execute("NX");
    if(current!=numericA)throw new Exception("Cancelled numeric NX replaced the open score.");
    SetDocument(Score.Load(a),a);
    var prefix=score.Encode();
    var incoming=Score.Load(b).Items;
    Execute("GM SONAB");
    Execute("");
    if(pendingAppendScore==null)throw new Exception("GM failed to warn about occupied staves.");
    HandleScoreAppendKey(new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Escape));
    if(!score.Encode().SequenceEqual(prefix))throw new Exception("Cancelled GM changed the score.");
    int count=score.Items.Count;
    Execute("GM SONAB");
    Execute("10");
    if(current!=a||!dirty||score.Items.Count!=count+incoming.Count)throw new Exception("GM offset did not append to the current score.");
    for(int n=0;n<incoming.Count;n++){
     var expected=incoming[n].Clone();
     expected.Set(2,expected.P(2)+10);
     if(!score.Items[count+n].Bytes.SequenceEqual(expected.Bytes))throw new Exception("GM offset changed another parameter.");
    }
    var beforeMissing=score.Encode();
    Execute("GM MISSING");
    if(!score.Encode().SequenceEqual(beforeMissing)||!pendingEditAcknowledgement||pendingAppendPath!=null||
       !responseLine.Text.StartsWith("File not found",StringComparison.Ordinal))
     throw new Exception("Missing GM changed the current score.");
    Execute("");
    Execute("GM SONAB");
    Execute("");
    HandleScoreAppendKey(new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Space));
    if(score.Items.Count!=count+incoming.Count*2||
       incoming.Where((item,index)=>!item.Bytes.SequenceEqual(score.Items[count+incoming.Count+index].Bytes)).Any())
     throw new Exception("Confirmed GM did not preserve the incoming records.");
    Console.WriteLine("PASS: VER, PM, DIR, CD, NX, NB and GM letter commands.");
   }finally{
    Directory.SetCurrentDirectory(previousDirectory);
    unsavedDecision=previousDecision;
    SetDocument(previousScore,previousFile);
    foreach(string file in Directory.GetFiles(folder))File.Delete(file);
    Directory.Delete(folder);
   }
  }
 }
}
