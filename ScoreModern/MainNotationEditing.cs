using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 public sealed partial class Editor {
  const string EnharmonicNormalize="ENH-NORMALIZE";
  const string EnharmonicNormalizePrompt="Normalize stem directions?  (y or n, <Enter>=yes) ->";
  int originalInputStageHeader;
  short? testGraceWord;

  bool ExecuteMainNotationCommand(string verb,string arguments){
   if(ExecuteMainLayout(verb,arguments))return true;
   if(ExecuteMainTranspose(verb,arguments))return true;
   if(verb=="STUD"){
    studyStage=0;groupCommand="STUD";groupRange=null;
    Message(SystemSourcePrompt);command.Focus();return true;
   }
   if(verb=="ENH"){
    StartMainGroup(verb,"");
    Message("Enter Staff number, Left pos., Right pos. ->");
    return true;
   }
   if(verb=="STM"){ShortenOriginalStems(arguments);return true;}
   if(verb=="AD"){AdjustOriginalBeamStems(arguments);return true;}
   if(verb=="FL"||verb=="FLU"||verb=="FLD"||verb=="FLO"||verb=="FLN"){
    StartMainGroup(verb,"");
    string description=verb=="FLU"?"Up.":verb=="FLD"?"Down.":"to Opposite Direction.";
    Message((verb=="FLN"?"Normalize Stem Directions.":"Flip Stems, etc. "+description)+Environment.NewLine+
     "Enter Staff number, Left pos., Right pos. ->");
    return true;
   }
   if(verb!="CRV")return false;
   StartMainGroup(verb,arguments);
   return true;
  }

  void CommitOriginalGroupEnharmonic(){
   var order=score.Items.OrderBy(item=>item.Staff).ThenBy(item=>item.P(3)).ToArray();
   bool flats=false,sharps=false;
   foreach(var note in order.Where(item=>item.Code==1&&InMainGroup(item))){
    int accidental=(int)note.P(5)%10;
    flats|=accidental==1||accidental==4;
    sharps|=accidental==2||accidental==5;
   }
   if(flats&&sharps){
    ClearMainGroup();
    pendingEditAcknowledgement=true;
    ApplyMainOrder(order,"*** Can't mix sharps and flats.");
    return;
   }
   if(!flats&&!sharps){
    ClearMainGroup();pendingEditAcknowledgement=true;
    ApplyMainOrder(order,"*** No enharmonic changes made.");return;
   }
   bool changedAny=false;
   for(int i=0;i<order.Length;i++){
    var note=order[i];
    if(note.Code!=1||!InMainGroup(note))continue;
    // Once a direction is established, SCORE respells every selected note,
    // including naturals. The typed enharmonic leaf supplies each delta.
    int step=sharps?1:-1,delta;
    if(!TryOriginalEnharmonicDelta(OriginalEnharmonicClef(note),note.P(4),note.P(5),step,out delta))continue;
    var changed=note.Clone();
    changed.Set(4,note.P(4)+step);
    changed.Set(5,note.P(5)+delta);
    order[i]=changed;
    changedAny=true;
   }
   if(!changedAny){
    ClearMainGroup();
    pendingEditAcknowledgement=true;
    ApplyMainOrder(order,"*** No enharmonic changes made.");
    return;
   }
   groupCommand=EnharmonicNormalize;
   ApplyMainOrder(order,EnharmonicNormalizePrompt);
  }

  bool IsFlipGroup {get{return groupCommand=="FL"||groupCommand=="FLO"||
   groupCommand=="FLU"||groupCommand=="FLD"||groupCommand=="FLN";}}

  void CommitOriginalFlip(string operation=null,bool separatePartsCaller=false){
   CommitOriginalFlipWithTies(operation,separatePartsCaller,1);
  }
  void CommitOriginalFlipWithTies(string operation,bool separatePartsCaller,short tiesMode){
   operation=operation??groupCommand;
   var order=BuildOriginalFlip(score,operation,separatePartsCaller,tiesMode,groupRange,source,ref originalInputStageHeader,testGraceWord);
   ClearMainGroup();
   ApplyMainOrder(order,"");
  }
  internal static List<Item> NormalizePageStems(IList<Item> input,string root,bool page4=false){
   var score=NativeMeasurements.New(root);score.Items=input.ToList();int stage=0;
   int staffCount=input.Where(item=>item.Code==8).Select(item=>item.Staff).DefaultIfEmpty(1).Max();
   // PAGE's fitter at 1A38:0897 reads +0.0 from its own slope storage.
   return BuildOriginalFlip(score,"FLN",true,0,new[]{1f,0f,200f,0f,staffCount},root,ref stage,null,0f,page4).ToList();
  }
  static Item[] BuildOriginalFlip(Score score,string operation,bool separatePartsCaller,short tiesMode,float[] range,string root,ref int stage,short? graceWord,float? beamSlope=null,bool page4=false){
   var order=score.Items.OrderBy(item=>item.Staff).ThenBy(item=>item.P(3)).ToArray();
   Func<Item,bool> selected=item=>(range[0]>32||item.Staff>=range[0]&&item.Staff<=range[4])&&
    item.P(3)>=range[1]-.001f&&item.P(3)<=range[2]+.001f&&(range[3]==0||item.Code==(int)range[3]);
   short? retained=NativeEditCompatibility.OriginalGraceState?(graceWord??(stage==2?null:(short?)0)):null;
   var completedChords=new HashSet<int>();
   var overCapacity=new HashSet<int>(order.Where(item=>item.Code==1&&selected(item)).GroupBy(item=>item.Staff).Where(g=>g.Count()>100).Select(g=>g.Key));
   if(overCapacity.Count!=0&&!NativeEditCompatibility.ReproduceInvalidStates)
    throw new NotSupportedException("The original refuses more than 100 selected notes on one staff.");
   for(int n=0;n<order.Length;n++){
    var item=order[n];
    if(overCapacity.Contains(item.Staff))continue;
    if(completedChords.Contains(n)||!selected(item))continue;
    if(item.Code!=1)continue;
    int[] chord=Enumerable.Range(n,order.Length-n).Where(index=>order[index].Code==1&&
     order[index].Staff==item.Staff&&order[index].P(3)==item.P(3)).ToArray();
    int stem=(int)item.P(5)%100/10;
    bool separateChord=separatePartsCaller&&chord.Length>1;
    if(stem!=1&&stem!=2&&!separateChord){
     if(NativeEditCompatibility.ReproduceInvalidStates)continue;
     throw new NotSupportedException("Flip requires explicit stems in the selected note range.");
    }
    int target=operation=="FLU"?1:operation=="FLD"?2:
     operation=="FLN"?(Math.Abs(item.P(4))>=100||item.P(4)%100<7?1:2):3-stem;
    if(chord.Length>1){
     if(separateChord){int chordRoot=Array.FindIndex(chord,index=>(int)order[index].P(5)%100/10!=0);if(chordRoot>0)chord=chord.Skip(chordRoot).Concat(chord.Take(chordRoot)).ToArray();}
     if(!separateChord&&chord.Skip(1).Any(index=>(int)order[index].P(5)%100/10!=0)){
      if(NativeEditCompatibility.ReproduceInvalidStates){foreach(int index in chord)completedChords.Add(index);continue;}
      throw new NotSupportedException("The original refuses two explicit stems in the same chord.");
     }
     ReconstructOriginalNoteGroup(order,chord,operation=="FLN"?0:target*10,retained);
     foreach(int index in chord)completedChords.Add(index);
     continue;
    }
    ReconstructOriginalNoteGroup(order,new[]{n},operation=="FLN"?0:target*10,retained);
   }
   var fittedBeamStems=new Dictionary<string,int>();
   var sameStartBeams=new Dictionary<string,Item>();
   var adjustedBeams=new HashSet<int>();
   for(int n=0;n<order.Length;n++){
    var beam=order[n];
    if(beam.Code!=6||!selected(beam)||overCapacity.Contains(beam.Staff))continue;
    string key=beam.Staff+":"+BitConverter.ToInt32(BitConverter.GetBytes(beam.P(3)),0);
    Item earlier;bool adjusted=true;
    // Main36:04BF-057B tags subsequent beams at the same exact X as
    // siblings; 0F29-10F2 restores their saved fields around the first fit.
    if(beam.P(7)>=0&&beam.P(7)<30&&sameStartBeams.TryGetValue(key,out earlier)){
     var sibling=beam.Clone();sibling.Set(4,earlier.P(4));sibling.Set(5,earlier.P(5));
     sibling.Set(7,(int)earlier.P(7)/10*10+(int)beam.P(7)%10);order[n]=sibling;
    }else{
     adjusted=RefitOriginalFlipBeam(order,n,operation,fittedBeamStems,range,root,retained,beamSlope);
     if(adjusted)sameStartBeams[key]=order[n];
    }
    if(adjusted)adjustedBeams.Add(n);
   }
   if(adjustedBeams.Count!=0){stage=4;ApplyRecoveredBeamAdjustment(score,order,33,false,-1,adjustedBeams,page4);}
   var previousSlurEndpoints=new Dictionary<int,Tuple<int,int>>();
   var generatedSlurs=new Dictionary<int,Queue<float[]>>();
   for(int n=0;n<order.Length;n++){
    var slur=order[n];
    if(slur.Code!=5||!selected(slur)||overCapacity.Contains(slur.Staff))continue;
    // 2DC2:02A4..0302 collects Code5 with at least seven parameters,
    // excluding integer P8=1/2 and a bitwise-zero P7. Ties (P8=-2)
    // participate in normalization too; selection is not limited to -1.
    if(slur.Count<7)continue;
    if(slur.Count>=8){long offsetWord;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(slur.P(8)),0),out offsetWord))throw new NotSupportedException("Non-finite slur offset in original normalization.");short slurOffset=unchecked((short)offsetWord);if(slurOffset==1||slurOffset==2)continue;}
    if(BitConverter.ToInt32(BitConverter.GetBytes(slur.P(7)),0)==0)continue;
    var notes=order.Where(item=>item.Code==1&&item.Staff==slur.Staff&&selected(item)).ToArray();
    if(notes.Length==0)continue;
    var eventGroups=notes.GroupBy(item=>item.P(3)).ToArray();int events=eventGroups.Length;
    int[] map=new int[events+1];int offset=0,first=0,last=events+1;
    int leftWord=unchecked((short)(int)Math.Truncate(slur.P(3))),rightWord=unchecked((short)(int)Math.Truncate(slur.P(6)));
    // Main 36:1130-1394: word-quantised endpoints, last match wins;
    // the start comparison takes precedence over the end comparison.
    for(int e=0;e<events;e++){
     map[e+1]=offset+1;offset+=eventGroups[e].Count();
     int x=unchecked((short)(int)Math.Truncate(eventGroups[e].Key));
     if(Math.Abs((int)unchecked((short)(leftWord-x)))<=1)first=e+1;
     else if(Math.Abs((int)unchecked((short)(rightWord-x)))<=1)last=e+1;
    }
    // 2DC2:11F6..1206 suppresses consecutive equal endpoint pairs in
    // the eligible curve list; 1279..1282 retains the last emitted pair.
    Tuple<int,int> previousPair;
    if(previousSlurEndpoints.TryGetValue(slur.Staff,out previousPair)&&previousPair.Item1==first&&previousPair.Item2==last)continue;
    previousSlurEndpoints[slur.Staff]=Tuple.Create(first,last);
    var rows=notes.Select(item=>{
     var row=new float[15];for(int k=0;k<15;k++)row[k]=item.P(k+1);
     // Main's transcription passes pitch/stem/rhythm/marks; P12 is retained
     // in the original compact item but not forwarded as a slur input control.
     row[11]=0;return row;
    }).ToList();
    var endpoints=InputEndpointProjection.Decode(first,last,5,rows,map,null,range[1],range[2]);
    var builtRows=InputSlurConstruction.Build(rows,endpoints,slur.Staff,events+1,tiesMode,range[2]);
    stage=5;
    var built=builtRows[rows.Count];
    for(int k=0;k<notes.Length;k++)if(notes[k].P(11)!=builtRows[k][10]){
     int at=Array.IndexOf(order,notes[k]);var marked=notes[k].Clone();marked.Set(11,builtRows[k][10]);order[at]=marked;
    }
    if(tiesMode==0){
     int bank=beamSlope.HasValue?slur.Staff:0;
     Queue<float[]> queue;
     if(!generatedSlurs.TryGetValue(bank,out queue))generatedSlurs.Add(bank,queue=new Queue<float[]>());
     foreach(var generated in builtRows.Skip(rows.Count))queue.Enqueue(generated);
     continue;
    }
    // Main 36:1404-1438 compares the two stored start-coordinate words
    // before copying the reconstructed curve back to the compact item.
    if(!SameFlipPosition(slur.P(3),built[2]))continue;
    var normalized=slur.Clone();normalized.Set(4,built[3]);normalized.Set(5,built[4]);
    float height=OriginalCurvature(slur.P(3),built[3],built[4],slur.P(6));
    normalized.Set(7,built[6]<0?-height:height);order[n]=normalized;
   }
   if(tiesMode==0){
    // PAGE 12C0:1391..1657 restarts the candidate cursor on each staff.
    // A nonmatching generated record can stall writes on its own staff.
    for(int n=0;n<order.Length;n++){
     var slur=order[n];
     if(slur.Code!=5||slur.Count<7||!selected(slur)||slur.P(7)==0||overCapacity.Contains(slur.Staff))continue;
     short offset=unchecked((short)(int)slur.P(8));if(offset==1||offset==2)continue;
     Queue<float[]> queue;
     if(!generatedSlurs.TryGetValue(beamSlope.HasValue?slur.Staff:0,out queue)||
      queue.Count==0||!SameFlipPosition(slur.P(3),queue.Peek()[2]))continue;
     var built=queue.Dequeue();
     var normalized=slur.Clone();normalized.Set(4,built[3]);normalized.Set(5,built[4]);
     normalized.Set(7,InputSlurConstruction.InitialHeight(slur.P(3),slur.P(6),built[3],built[4],built[7],built[6]));
     order[n]=normalized;
    }
   }
   return order;
  }
  static bool SameFlipPosition(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}

  static void ReconstructOriginalNoteGroup(Item[] order,int[] group,int stem,short? retained){
   var rows=new List<float[]>();foreach(int index in group){var item=order[index];var row=new float[item.Count];for(int p=0;p<row.Length;p++)row[p]=item.P(p+1);rows.Add(row);}
   var result=InputMainNoteReconstruction.Build(rows,stem,retained,NativeEditCompatibility.ReproduceInvalidStates);
   for(int i=0;i<group.Length;i++){
    var original=order[group[i]];var changed=original.Clone();
    foreach(int parameter in new[]{4,5,8,9,10})changed.Set(parameter,result[i][parameter-1]);
    changed.Set(11,i==0?OriginalFlipMark(original.P(11),changed.P(4),changed.P(5)):0);
    order[group[i]]=changed;
   }
  }

  static float OriginalFlipMark(float value,float pitch,float stem){
   int mark=(int)value;
   return value>=0&&Array.IndexOf(new[]{4,5,8,9,10,19},mark)>=0?
    InputAttachedMarks.DefaultValue(mark,pitch,stem):value;
  }

  static bool RefitOriginalFlipBeam(Item[] order,int index,string operation,Dictionary<string,int> fittedBeamStems,float[] range,string root,short? retained,float? beamSlope){
   var beam=order[index];
   int descriptor=(int)beam.P(7);
   if(descriptor<0||descriptor>=30||beam.P(6)<range[1]||beam.P(6)>range[2]){
    if(NativeEditCompatibility.ReproduceInvalidStates)return false;
    throw new NotSupportedException("Original beam flip refuses this tremolo or out-of-range endpoint; original invalid-state compatibility retains its partial note writes.");
   }
   var notes=order.Where(item=>item.Code==1&&item.Staff==beam.Staff&&
    item.P(3)>=beam.P(3)-.001f&&item.P(3)<=beam.P(6)+.001f).ToArray();
   // Section59 carries the first endpoint cue class across the beam.
   bool cue=notes.Length>0&&Math.Abs(notes[0].P(4))>=100&&Math.Abs(notes[0].P(4))<200;
   if(notes.Length<2||notes[0].P(3)!=beam.P(3)||notes[notes.Length-1].P(3)!=beam.P(6)){
    if(NativeEditCompatibility.ReproduceInvalidStates)return false;
    throw new NotSupportedException("Beam flip requires ordinary pitched endpoints.");
   }
   var groups=notes.GroupBy(item=>item.P(3)).Select(group=>group.ToArray()).ToArray();
   if(groups.Length<2){
    if(NativeEditCompatibility.ReproduceInvalidStates)return false;
    throw new NotSupportedException("The original refuses a beam without distinct endpoint events.");
   }
   int[] levels=groups.Select(group=>(int)InputRhythmDecoder.Decode(
    (float)(((int)group[0].P(9)/10)*1000.0+group[0].P(7))).FlagCount).ToArray();
   int flags=levels.Max();
   if(levels.Min()<1){
    if(NativeEditCompatibility.ReproduceInvalidStates)return false;
    throw new NotSupportedException("The original refuses a beam containing a note without a tail.");
   }
   int direction=operation=="FLU"?10:operation=="FLD"?20:operation=="FLN"?0:(int)notes[0].P(5)%100/10*10;
   // Section59's endpoint leaf takes the persistent first nonzero endpoint
   // stem. Its final loop retains the working stem map between beam inputs.
   if(direction==0){int prior;
    if(fittedBeamStems.TryGetValue(beam.Staff+":"+notes[0].P(3).ToString("R",CultureInfo.InvariantCulture),out prior)||
       fittedBeamStems.TryGetValue(beam.Staff+":"+notes[notes.Length-1].P(3).ToString("R",CultureInfo.InvariantCulture),out prior))direction=prior;
   }
   if(operation=="FLN"&&cue)direction=10;
   if(groups.Any(group=>group.Length>1)){
    // Section59 selects a shared chord direction before taking each chord's
    // final member as the fitter's representative pitch.
    if(direction==0)direction=7-notes.Min(item=>item.P(4)%100)<=notes.Max(item=>item.P(4)%100)-7?20:10;
    foreach(var group in groups){
     if(group.Length<2||(int)group[0].P(5)%100/10==direction/10)continue;
     ReconstructOriginalNoteGroup(order,group.Select(item=>Array.IndexOf(order,item)).ToArray(),direction,retained);
    }
    notes=order.Where(item=>item.Code==1&&item.Staff==beam.Staff&&item.P(3)>=beam.P(3)-.001f&&item.P(3)<=beam.P(6)+.001f).ToArray();
    groups=notes.GroupBy(item=>item.P(3)).Select(group=>group.ToArray()).ToArray();
   }
   var representatives=groups.Select(group=>group[group.Length-1]).ToArray();
   float slope=beamSlope??(float)((double)(root==null?.5f:NativePreferences.Read(root).Number("BEAM",.5f))/100);
   var fit=InputBeamFit.Fit(representatives.Select(item=>item.P(4)%100).ToArray(),
    representatives.Select(item=>item.P(3)).ToArray(),direction,flags,slope);
   int a=(int)fit.Start,b=(int)fit.End,offset=0;
   if(flags>2){
    offset=2;
    if(fit.Direction>19&&a>13||fit.Direction<20&&a<1)offset=4;
    offset+=(flags-3)*3;
   }
   if(fit.Direction<20){if(a==b&&a>9)offset-=2;}
   else{offset=-offset;if(a==b&&a<5)offset+=2;}
   var changed=beam.Clone();
   float shift=cue?(fit.Direction<20?.800000011920929f:-.800000011920929f):offset/2f;
   changed.Set(4,(float)((double)fit.Start+shift+(cue?(fit.Start<0?-100:100):0)));
   changed.Set(5,(float)((double)fit.End+shift));
   changed.Set(7,fit.Direction+levels.Min());
   order[index]=changed;
   foreach(var note in notes)fittedBeamStems[beam.Staff+":"+note.P(3).ToString("R",CultureInfo.InvariantCulture)]=fit.Direction;
   foreach(var note in notes){
    if((int)note.P(5)%100/10==0)continue;
    int at=Array.IndexOf(order,note);var replacement=note.Clone();
    replacement.Set(5,note.P(5)+(fit.Direction/10-(int)note.P(5)%100/10)*10);
    replacement.Set(11,OriginalFlipMark(note.P(11),note.P(4),replacement.P(5)));
    order[at]=replacement;
   }
   return true;
  }

  void ShortenOriginalStems(string arguments){
   int staff;
   if(!Int32.TryParse(arguments,NumberStyles.Integer,CultureInfo.InvariantCulture,out staff)||staff<1)
    throw new InvalidDataException("STM requires a staff number. Use a number greater than 32 for all staves.");
   var order=score.Items.Select(note=>{
    if(note.Code!=1||staff<=32&&note.Staff!=staff||Math.Abs(note.P(4))>=100||note.Count<5)return note;
    int stem=(int)note.P(5);
    if(stem<10)return note;
    float extension=note.P(8);
    float tip=(float)((stem<20?note.P(4):14.0-note.P(4))+extension);
    if(tip<8)return note;
    float adjustment=tip<10?(float)((10.0-tip)*.5-1.5):-1.5f;
    if((int)note.P(9)%10>=1)adjustment+=.5f;
    float adjusted=(float)((extension<0?0:extension)+(double)adjustment);
    if(note.P(8)==adjusted)return note;
    var changed=note.Clone();
    changed.Set(8,adjusted);
    return changed;
   }).ToArray();
   ApplyMainOrder(order,"");
  }

  void AdjustOriginalBeamStems(string arguments){
   var fields=arguments.Split(new[]{' ', ',', '\t'},StringSplitOptions.RemoveEmptyEntries);
   float staffValue,force=0;
   if(fields.Length<1||fields.Length>2||!Single.TryParse(fields[0],NumberStyles.Float,CultureInfo.InvariantCulture,out staffValue)||Single.IsNaN(staffValue)||Single.IsInfinity(staffValue)||staffValue<short.MinValue||staffValue>=32768)
    throw new InvalidDataException("AD requires a finite signed-word staff value.");
   int staff=(short)Math.Truncate(staffValue);
   if(staff<1&&!NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("A nonpositive AD staff is prevented; original invalid-state compatibility permits the measured no-op.");
   if(fields.Length>1&&(!Single.TryParse(fields[1],NumberStyles.Float,CultureInfo.InvariantCulture,out force)||Single.IsNaN(force)||Single.IsInfinity(force)))
    throw new InvalidDataException("AD's optional direction override must be finite.");
   if(force<short.MinValue||force>=32768)throw new InvalidDataException("AD direction override outside the original signed-word domain.");
   var order=score.Items.ToArray();ApplyRecoveredBeamAdjustment(order,staff,(short)Math.Truncate(force)!=0,-1);ApplyMainOrder(order,"");
  }

  void ApplyRecoveredBeamAdjustment(Item[] order,int staff,bool force,int onlyBeam,ISet<int> selectedBeams=null){
   ApplyRecoveredBeamAdjustment(score,order,staff,force,onlyBeam,selectedBeams);
  }
  static void ApplyRecoveredBeamAdjustment(Score score,Item[] order,int staff,bool force,int onlyBeam,ISet<int> selectedBeams,bool page4=false){
   var rows=order.Select(item=>Enumerable.Range(1,item.Count).Select(p=>item.P(p)).ToArray()).ToList();
   int maximum=order.Where(item=>item.Code==8).Select(item=>item.Staff).DefaultIfEmpty(0).Max();
   var result=BeamStemAdjustment.Apply(rows,staff,force,new NativePageDocument(score).Tables,maximum,onlyBeam,selectedBeams,page4);
   for(int i=0;i<order.Length;i++){
    bool changed=result[i].Length!=rows[i].Length;
    if(!changed)for(int p=0;p<rows[i].Length;p++)if(BitConverter.ToInt32(BitConverter.GetBytes(rows[i][p]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(result[i][p]),0)){changed=true;break;}
    if(!changed)continue;var item=order[i].Clone();for(int p=0;p<result[i].Length;p++)item.Set(p+1,result[i][p]);order[i]=item;
   }
  }

  void CheckMainNotationEditing(){
   var previous=score;
   string previousFile=current;
   try{
    var fixture=NativeMeasurements.New(source);
    fixture.Items.Clear();
    fixture.Items.Add(Item.Make(8,1,0,0,1,200));
    fixture.Items.Add(Item.Make(1,1,20,3,10,0,.5f,0,1));
    fixture.Items.Add(Item.Make(1,1,30,6,20,0,.5f,0,1));
    fixture.Items.Add(Item.Make(1,1,40,7,10,0,.5f,0,1));
    fixture.Items.Add(Item.Make(6,1,20,10,12,40,11));
    fixture.Items.Add(Item.Make(5,1,10,5,6,22,2));
    fixture.Items.Add(Item.Make(5,1,180,5,6,172,-2,-1));
    SetDocument(fixture,"");
    Execute("AD 1");
    if(score.Items[1].P(8)!=7.000999450683594f||score.Items[1].P(9)!=0||
       score.Items[2].P(8)!=0||score.Items[2].P(9)!=1||
       score.Items[3].P(8)!=4.999001502990723f||score.Items[3].P(9)!=0)
     throw new Exception("AD did not match the DOS beam stems: "+String.Join(", ",score.Items.Skip(1).Take(3).Select(item=>item.P(8).ToString("R",CultureInfo.InvariantCulture)+"/"+item.P(9).ToString("R",CultureInfo.InvariantCulture))));
    Execute("AD 1 1");
    if(score.Items[2].P(8)!=-18.716949462890625f||score.Items[2].P(9)!=0)
     throw new Exception("AD did not match the DOS forced stem.");
    Execute("CRV");Execute("1 0 220");Execute("B");
    if(score.Items[5].P(7)!=2)throw new Exception("Cancelled CRV changed the score.");
    Execute("CRV");Execute("1 0 220");Execute("");
    if(score.Items[5].P(7)!=1.600000023841858f||score.Items[6].P(7)!=-.5999999642372131f)
     throw new Exception("CRV did not match the DOS slur curvatures.");
    var downFixture=NativeMeasurements.New(source);
    downFixture.Items=fixture.Items.Take(5).Select(item=>item.Clone()).ToList();
    downFixture.Items[4].Set(7,21);
    SetDocument(downFixture,"");
    Execute("AD 1 1");
    if(score.Items[1].P(8)!=-6.7159504890441895f||score.Items[2].P(8)!=-5.000000476837158f||
       score.Items[3].P(8)!=-8.717948913574219f)
     throw new Exception("AD did not match the DOS forced down-beam rounding.");
    var stemFixture=NativeMeasurements.New(source);
    stemFixture.Items.Clear();
    stemFixture.Items.Add(Item.Make(8,1,0,0,1,200));
    stemFixture.Items.Add(Item.Make(1,1,20,8,10,0,.5f,.5f));
    stemFixture.Items.Add(Item.Make(1,1,30,7,20,0,.5f,.5f));
    stemFixture.Items.Add(Item.Make(1,1,40,4,10,0,.5f,4));
    SetDocument(stemFixture,"");
    Execute("STM 1");
    if(score.Items[1].P(8)!=-.25f||score.Items[2].P(8)!=.5f||score.Items[3].P(8)!=3.5f)
     throw new Exception("STM did not match the DOS shortening boundaries.");
    var flaggedFixture=NativeMeasurements.New(source);
    flaggedFixture.Items.Clear();
    flaggedFixture.Items.Add(Item.Make(8,1,0,0,1,200));
    flaggedFixture.Items.Add(Item.Make(1,1,20,8,10,0,.5f,2,3));
    flaggedFixture.Items.Add(Item.Make(1,1,40,8,20,0,.5f,2,3));
    flaggedFixture.Items.Add(Item.Make(1,1,60,13,10,0,.5f,-3));
    SetDocument(flaggedFixture,"");
    Execute("STM 1");
    if(score.Items[1].P(8)!=1||score.Items[2].P(8)!=2||score.Items[3].P(8)!=-1.5f)
     throw new Exception("STM did not match flagged and negative-extension DOS records.");
    foreach(string verb in new[]{"FL","FLU","FLD","FLO"}){
     var flipFixture=NativeMeasurements.New(source);
     flipFixture.Items.Clear();
     flipFixture.Items.Add(Item.Make(8,1,0,0,1,200));
     flipFixture.Items.Add(Item.Make(1,1,20,4,11,0,.5f,2,1));
     flipFixture.Items.Add(Item.Make(1,1,40,11,22,0,.5f,1,1));
     flipFixture.Items.Add(Item.Make(1,1,50,7,20,0,.5f,0,1));
     flipFixture.Items.Add(Item.Make(5,1,20,5,6,40,2,-1));
     flipFixture.Items.Add(Item.Make(5,1,40,9,8,50,-2,-1));
     SetDocument(flipFixture,"");
     Execute(verb);Execute("1 0 220");Execute("");
     bool up=verb=="FLU",down=verb=="FLD";
     var expected=new[]{
      Item.Make(8,1,0,0,1,200),
      Item.Make(1,1,20,4,up?11:21,0,.5f,0,1,0,0),
      Item.Make(5,1,20,up?2:6,up?8:13,40,up?-2.5f:2.5999999046325684f,-1),
      Item.Make(1,1,40,11,down?22:12,0,.5f,0,1,0,0),
      Item.Make(5,1,40,down?13:8,down?10:4,50,down?1.6999999284744263f:-1.7999999523162842f,-1),
      Item.Make(1,1,50,7,down?20:10,0,.5f,0,1,0,0)
     };
     if(!score.Items.Zip(expected,(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
      throw new Exception(verb+" did not match the DOS note and slur records: "+String.Join("; ",score.Items.Select((a,i)=>String.Join(",",Enumerable.Range(1,Math.Max(a.Count,expected[i].Count)).Where(k=>a.P(k)!=expected[i].P(k)).Select(k=>"item"+i+" P"+k+"="+a.P(k).ToString("R",CultureInfo.InvariantCulture)+" expected "+expected[i].P(k).ToString("R",CultureInfo.InvariantCulture))))));
    }
    var normalizeFixture=NativeMeasurements.New(source);
    normalizeFixture.Items.Clear();
    normalizeFixture.Items.Add(Item.Make(8,1,0,0,1,200));
    normalizeFixture.Items.Add(Item.Make(1,1,20,4,21,0,.5f,0,1,0,0));
    normalizeFixture.Items.Add(Item.Make(1,1,40,11,12,0,.5f,0,1,0,0));
    normalizeFixture.Items.Add(Item.Make(1,1,50,7,10,0,.5f,0,1,0,0));
    SetDocument(normalizeFixture,"");
    Execute("FLN");Execute("1 0 220");Execute("");
    if(score.Items[1].P(5)!=11||score.Items[2].P(5)!=22||score.Items[3].P(5)!=20)
     throw new Exception("FLN did not match the DOS normalized note stems.");
    var adControlFixture=NativeMeasurements.New(source);adControlFixture.Items.Clear();
    adControlFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    adControlFixture.Items.Add(Item.Make(1f,1f,20f,3f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,1f,30f,5f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,1f,40f,7f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,1f,50f,10f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(6f,1f,20f,8f,10f,50f,11f));
    adControlFixture.Items.Add(Item.Make(8f,2f,0f,25f,1f,200f));
    adControlFixture.Items.Add(Item.Make(1f,2f,20f,103f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,2f,30f,105f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,2f,40f,107f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,2f,50f,110f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(6f,2f,20f,8f,10f,50f,11f));
    adControlFixture.Items.Add(Item.Make(8f,3f,0f,50f,1f,200f));
    adControlFixture.Items.Add(Item.Make(1f,3f,20f,-103f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,3f,30f,-101f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,3f,40f,-99f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,3f,50f,-96f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(6f,3f,20f,8f,10f,50f,11f));
    adControlFixture.Items.Add(Item.Make(8f,4f,0f,75f,1f,200f));
    adControlFixture.Items.Add(Item.Make(1f,4f,20f,3f,10f,0f,0.5f,2f,1f,0f,0f,1f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,4f,30f,5f,10f,0f,0.5f,2f,1f,0f,0f,1f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,4f,40f,7f,10f,0f,0.5f,2f,1f,0f,0f,1f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(1f,4f,50f,10f,10f,0f,0.5f,2f,1f,0f,0f,1f,0f,0f,0f));
    adControlFixture.Items.Add(Item.Make(6f,4f,20f,8f,10f,50f,11f));
    adControlFixture.Items.Add(Item.Make(8f,5f,0f,100f,1f,200f));
    adControlFixture.Items.Add(Item.Make(1f,5f,20f,3f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0.699999988f));
    adControlFixture.Items.Add(Item.Make(1f,5f,30f,5f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0.699999988f));
    adControlFixture.Items.Add(Item.Make(1f,5f,40f,7f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0.699999988f));
    adControlFixture.Items.Add(Item.Make(1f,5f,50f,10f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0.699999988f));
    adControlFixture.Items.Add(Item.Make(6f,5f,20f,8f,10f,50f,11f));
    adControlFixture.Items.Add(Item.Make(8f,6f,0f,125f,1f,200f));
    adControlFixture.Items.Add(Item.Make(1f,6f,20f,3f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,1.5f));
    adControlFixture.Items.Add(Item.Make(1f,6f,30f,5f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,1.5f));
    adControlFixture.Items.Add(Item.Make(1f,6f,40f,7f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,1.5f));
    adControlFixture.Items.Add(Item.Make(1f,6f,50f,10f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,1.5f));
    adControlFixture.Items.Add(Item.Make(6f,6f,20f,8f,10f,50f,11f));
    SetDocument(adControlFixture, "");Execute("AD 99");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,3f,10f,0f,0.5f,5.00066614f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,30f,5f,10f,0f,0.5f,3.66688871f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,40f,7f,10f,0f,0.5f,2.33311129f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,50f,10f,10f,0f,0.5f,-0.00066614151f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,1f,20f,8f,10f,50f,11f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,103f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,2f,30f,105f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,2f,40f,107f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,2f,50f,110f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(6f,2f,20f,8f,10f,50f,11f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,-103f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,3f,30f,-101f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,3f,40f,-99f,10f,0f,0.5f,108.333115f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,3f,50f,-96f,10f,0f,0.5f,105.999336f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,3f,20f,8f,10f,50f,11f),Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,3f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,4f,30f,5f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,4f,40f,7f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,4f,50f,10f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(6f,4f,20f,8f,10f,50f,11f),Item.Make(8f,5f,0f,100f,1f,200f),Item.Make(1f,5f,20f,3f,10f,0f,0.5f,5.00066614f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,5f,30f,5f,10f,0f,0.5f,3.66688871f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,5f,40f,7f,10f,0f,0.5f,2.33311129f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,5f,50f,10f,10f,0f,0.5f,2f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(6f,5f,20f,8f,10f,50f,11f),Item.Make(8f,6f,0f,125f,1f,200f),Item.Make(1f,6f,20f,3f,10f,0f,0.5f,5.00066614f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(1f,6f,30f,5f,10f,0f,0.5f,3.66688871f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(1f,6f,40f,7f,10f,0f,0.5f,2.33311129f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(1f,6f,50f,10f,10f,0f,0.5f,2f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(6f,6f,20f,8f,10f,50f,11f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("AD cue, scale and cross-staff fixture differs from the original records.");
    var beamFlipFixture=NativeMeasurements.New(source);
    beamFlipFixture.Items.Clear();
    beamFlipFixture.Items.Add(Item.Make(8,1,0,0,1,200));
    foreach(var point in new[]{new[]{20f,3f},new[]{30f,5f},new[]{40f,7f},new[]{50f,10f}})
     beamFlipFixture.Items.Add(Item.Make(1,1,point[0],point[1],10,0,.25f,0,0));
    beamFlipFixture.Items.Add(Item.Make(6,1,20,8,10,50,11));
    SetDocument(beamFlipFixture,"");
    Execute("FLD");Execute("1 0 220");Execute("");
    var flippedBeam=score.Items.Single(item=>item.Code==6);
    float[] extensions={-.0003330707550048828f,1.666555643081665f,3.333444356918335f,6.000332832336426f};
    if(flippedBeam.P(4)!=3||flippedBeam.P(5)!=4||flippedBeam.P(7)!=22||
       !score.Items.Where(item=>item.Code==1).Select(item=>item.P(8)).SequenceEqual(extensions))
     throw new Exception("Beam flip differs: "+flippedBeam.P(4)+"/"+flippedBeam.P(5)+"/"+flippedBeam.P(7)+" stems "+String.Join(", ",score.Items.Where(item=>item.Code==1).Select(item=>item.P(8).ToString("R",CultureInfo.InvariantCulture))));
    foreach(var durations in new[]{new[]{.5f,.25f,.125f,.5f},new[]{.125f,.25f,.5f,.125f},new[]{.5f,.5f,.25f,.5f}}){
     var mixedFixture=NativeMeasurements.New(source);mixedFixture.Items.Clear();
     mixedFixture.Items.Add(Item.Make(8,1,0,0,1,200));
     float[] pitches={3,5,7,10};
     for(int i=0;i<4;i++)mixedFixture.Items.Add(Item.Make(1,1,20+i*10,pitches[i],10,0,durations[i],0,0));
     mixedFixture.Items.Add(Item.Make(6,1,20,8,10,50,12));
     SetDocument(mixedFixture,"");Execute("FLD");Execute("1 0 220");Execute("");
     var mixedBeam=score.Items.Single(item=>item.Code==6);
     bool triple=durations.Contains(.125f);
     float[] mixedExtensions=triple?new[]{.9996669292449951f,2.666555643081665f,4.333444595336914f,7.000332832336426f}:extensions;
     if(mixedBeam.P(4)!=(triple?2:3)||mixedBeam.P(5)!=(triple?3:4)||mixedBeam.P(7)!=21||
        !score.Items.Where(item=>item.Code==1).Select(item=>item.P(8)).SequenceEqual(mixedExtensions))
      throw new Exception("Mixed-duration beam flip differs from the original records.");
    }
    foreach(string verb in new[]{"FL","FLU","FLD","FLN"}){
     var chordFixture=NativeMeasurements.New(source);
     chordFixture.Items.Clear();
     chordFixture.Items.Add(Item.Make(8,1,0,0,1,200));
     chordFixture.Items.Add(Item.Make(1,1,40,1,10,0,1,4));
     chordFixture.Items.Add(Item.Make(1,1,40,3,0,0,1));
     chordFixture.Items.Add(Item.Make(1,1,40,5,0,0,1));
     SetDocument(chordFixture,"");
     Execute(verb);Execute("1 0 220");Execute("");
     bool down=verb=="FL"||verb=="FLD";
     var expectedChord=new[]{
      Item.Make(8,1,0,0,1,200),
      Item.Make(1,1,40,down?5:1,down?20:10,0,1,4,0,0,0),
      Item.Make(1,1,40,3,0,0,1,0,0,0,0),
      Item.Make(1,1,40,down?1:5,0,0,1,0,0,0,0)
     };
     if(!score.Items.Zip(expectedChord,(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
      throw new Exception(verb+" did not match the DOS chord orientation and record widths.");
    }
    var adjacentChordFixture=NativeMeasurements.New(source);adjacentChordFixture.Items.Clear();
    adjacentChordFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,20f,3f,11f,0f,1f,3f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,20f,4f,1f,0f,1f,0f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,20f,5f,2f,0f,1f,0f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,40f,1f,12f,0f,1f,3f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,40f,2f,1f,0f,1f,0f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,40f,4f,0f,0f,1f,0f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,60f,6f,10f,0f,1f,3f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f));
    adjacentChordFixture.Items.Add(Item.Make(1f,1f,60f,8f,0f,0f,1f,0f,0f));
    SetDocument(adjacentChordFixture, "");
    Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,5f,22.2830009f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,20f,4f,1.25f,0f,1f,0f,0f,20f,0f),Item.Make(1f,1f,20f,3f,1.773f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,4f,20f,0f,1f,3f,0f,0f,0f),Item.Make(1f,1f,40f,2f,1.28299999f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,1f,2.24000001f,0f,1f,0f,0f,20f,0f),Item.Make(1f,1f,60f,8f,20f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f,20f,0f),Item.Make(1f,1f,60f,6f,0f,0f,1f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Adjacent chord down differs from DOS accidental packing.");
    Execute("FLU");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,3f,11.4899998f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,20f,4f,1.25f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,20f,5f,2f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,1f,12.2399998f,0f,1f,3f,0f,0f,0f),Item.Make(1f,1f,40f,2f,1f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,40f,4f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,60f,6f,10f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,60f,8f,0f,0f,1f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Adjacent chord up differs from DOS accidental packing.");
    Execute("FLU");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,3f,11.4899998f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,20f,4f,1.25f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,20f,5f,2f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,1f,12.2399998f,0f,1f,3f,0f,0f,0f),Item.Make(1f,1f,40f,2f,1f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,40f,4f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,60f,6f,10f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,60f,8f,0f,0f,1f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Adjacent chord up-repeat differs from DOS accidental packing.");
    var flipControlFixture=NativeMeasurements.New(source);flipControlFixture.Items.Clear();
    flipControlFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,1f,20f,103f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,1f,30f,105f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,1f,40f,107f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,1f,50f,110f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(6f,1f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(8f,2f,0f,25f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,2f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    flipControlFixture.Items.Add(Item.Make(1f,2f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    flipControlFixture.Items.Add(Item.Make(1f,2f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    flipControlFixture.Items.Add(Item.Make(1f,2f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    flipControlFixture.Items.Add(Item.Make(6f,2f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(8f,3f,0f,50f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,3f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,3f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,3f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,3f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(6f,3f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(8f,4f,0f,75f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,4f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,4f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,4f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,4f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(6f,4f,20f,8f,10f,50f,2f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(8f,5f,0f,100f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,5f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,5f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,5f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,5f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(6f,5f,20f,8f,10f,50f,-12f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(8f,6f,0f,125f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,6f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,6f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,6f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,6f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(6f,6f,20f,8f,10f,50f,12f,0f,1f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(8f,7f,0f,150f,1f,200f));
    flipControlFixture.Items.Add(Item.Make(1f,7f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,7f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,7f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(1f,7f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    flipControlFixture.Items.Add(Item.Make(6f,7f,20f,8f,10f,50f,12f,0f,0f,0f,1f));
    SetDocument(flipControlFixture, "");Execute("FLD");Execute("2 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==2).Zip(new[]{Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,3f,20f,0f,0.25f,-0.000333070755f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(6f,2f,20f,3f,4f,50f,22f,0f,0f,0f,0f),Item.Make(1f,2f,30f,5f,20f,0f,0.25f,1.66655564f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,40f,7f,20f,0f,0.25f,3.33344436f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,50f,10f,20f,0f,0.25f,6.00033283f,0f,0f,0f,0f,0f,0f,0.699999988f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Beam flip control staff 2 differs from original records.");
    SetDocument(flipControlFixture, "");Execute("FLD");Execute("3 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==3).Zip(new[]{Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,3f,20f,0f,0.25f,42.9996681f,0f,0f,0f,1f,0f,0f,0f),Item.Make(6f,3f,20f,3f,4f,50f,22f,0f,0f,0f,0f),Item.Make(1f,3f,30f,5f,20f,0f,0.25f,44.6665573f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,40f,7f,20f,0f,0.25f,46.3334427f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,50f,10f,20f,0f,0.25f,49.0003319f,0f,0f,0f,1f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Beam flip control staff 3 differs from original records.");
    SetDocument(flipControlFixture, "");Execute("FLD");Execute("4 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==4).Zip(new[]{Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,3f,20f,0f,0.25f,-0.000333070755f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,4f,20f,3f,4f,50f,22f,0f,0f,0f,0f),Item.Make(1f,4f,30f,5f,20f,0f,0.25f,1.66655564f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,40f,7f,20f,0f,0.25f,3.33344436f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,50f,10f,20f,0f,0.25f,6.00033283f,0f,0f,0f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Beam flip control staff 4 differs from original records.");
    var slurVariantFixture=NativeMeasurements.New(source);slurVariantFixture.Items.Clear();
    slurVariantFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    slurVariantFixture.Items.Add(Item.Make(1f,1f,20f,4f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(5f,1f,20f,6f,13f,40f,2.5999999f,-2f));
    slurVariantFixture.Items.Add(Item.Make(1f,1f,40f,11f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(8f,2f,0f,25f,1f,200f));
    slurVariantFixture.Items.Add(Item.Make(1f,2f,20f,4f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(5f,2f,20f,6f,13f,40f,2.5999999f,0f));
    slurVariantFixture.Items.Add(Item.Make(1f,2f,40f,11f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(8f,3f,0f,50f,1f,200f));
    slurVariantFixture.Items.Add(Item.Make(1f,3f,20f,4f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,1f));
    slurVariantFixture.Items.Add(Item.Make(1f,3f,40f,11f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(8f,4f,0f,75f,1f,200f));
    slurVariantFixture.Items.Add(Item.Make(1f,4f,20f,4f,10f,0f,0.5f,0f,1f,0f,0f));
    slurVariantFixture.Items.Add(Item.Make(5f,4f,20f,6f,13f,40f,2.5999999f,-1f));
    slurVariantFixture.Items.Add(Item.Make(1f,4f,40f,11f,10f,0f,0.5f,0f,1f,0f,0f));
    SetDocument(slurVariantFixture, "");Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==1).Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,1f,20f,6f,13f,40f,2.5999999f,-2f),Item.Make(1f,1f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("slurVariantFixture staff 1 differs from DOS.");
    SetDocument(slurVariantFixture, "");Execute("FLD");Execute("2 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==2).Zip(new[]{Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,2f,20f,6f,13f,40f,2.5999999f,0f),Item.Make(1f,2f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("slurVariantFixture staff 2 differs from DOS.");
    SetDocument(slurVariantFixture, "");Execute("FLD");Execute("3 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==3).Zip(new[]{Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,1f),Item.Make(1f,3f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("slurVariantFixture staff 3 differs from DOS.");
    SetDocument(slurVariantFixture, "");Execute("FLD");Execute("4 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==4).Zip(new[]{Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,4f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,4f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("slurVariantFixture staff 4 differs from DOS.");
    var beamAuxFixture=NativeMeasurements.New(source);beamAuxFixture.Items.Clear();
    beamAuxFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,1f,20f,103f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,1f,30f,105f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,1f,40f,107f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,1f,50f,110f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(6f,1f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(8f,2f,0f,25f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,2f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    beamAuxFixture.Items.Add(Item.Make(1f,2f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    beamAuxFixture.Items.Add(Item.Make(1f,2f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    beamAuxFixture.Items.Add(Item.Make(1f,2f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    beamAuxFixture.Items.Add(Item.Make(6f,2f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(8f,3f,0f,50f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,3f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,3f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,3f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,3f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(6f,3f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(8f,4f,0f,75f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,4f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,4f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,4f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,4f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(6f,4f,20f,8f,10f,50f,2f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(8f,5f,0f,100f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,5f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,5f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,5f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,5f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(6f,5f,20f,8f,10f,50f,-12f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(8f,6f,0f,125f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,6f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,6f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,6f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,6f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(6f,6f,20f,8f,10f,50f,12f,0f,1f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(8f,7f,0f,150f,1f,200f));
    beamAuxFixture.Items.Add(Item.Make(1f,7f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,7f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,7f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(1f,7f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    beamAuxFixture.Items.Add(Item.Make(6f,7f,20f,8f,10f,50f,12f,0f,0f,0f,1f));
    SetDocument(beamAuxFixture, "");Execute("FLD");Execute("6 0 220");Execute("");
    if(!score.Items.Where(item=>item.Staff==6).Zip(new[]{Item.Make(8f,6f,0f,125f,1f,200f),Item.Make(1f,6f,20f,3f,20f,0f,0.25f,-0.000333070755f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,6f,20f,3f,4f,50f,22f,0f,1f,0f,0f),Item.Make(1f,6f,30f,5f,20f,0f,0.25f,1.66655564f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,40f,7f,20f,0f,0.25f,3.33344436f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,50f,10f,20f,0f,0.25f,6.00033283f,0f,0f,0f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("beamAuxFixture staff 6 differs from DOS.");
    var wideChordFixture=NativeMeasurements.New(source);wideChordFixture.Items.Clear();
    wideChordFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,20f,1f,10f,0f,1f,20f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,20f,8f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,20f,10f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,50f,4f,10f,0f,1f,20f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,50f,6f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,50f,9f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,80f,-2f,10f,0f,1f,20f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,80f,7f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,80f,16f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,110f,1f,10f,0f,1f,20f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,110f,7f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,110f,13f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,140f,1f,10f,0f,1f,20f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,140f,5f,0f,0f,1f,0f));
    wideChordFixture.Items.Add(Item.Make(1f,1f,140f,7f,0f,0f,1f,0f));
    SetDocument(wideChordFixture, "");Execute("FLN");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,10f,0f,1f,9f,0f,0f,0f),Item.Make(1f,1f,20f,8f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,20f,10f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,50f,4f,10f,0f,1f,5f,0f,0f,0f),Item.Make(1f,1f,50f,6f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,50f,9f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,80f,-2f,10f,0f,1f,18f,0f,0f,0f),Item.Make(1f,1f,80f,7f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,80f,16f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,110f,1f,10f,0f,1f,12f,0f,0f,0f),Item.Make(1f,1f,110f,7f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,110f,13f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,140f,1f,10f,0f,1f,6f,0f,0f,0f),Item.Make(1f,1f,140f,5f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,140f,7f,0f,0f,1f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Wide-chord normalization differs from DOS.");
    var chordBeamControls=NativeMeasurements.New(source);chordBeamControls.Items.Clear();
    chordBeamControls.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    chordBeamControls.Items.Add(Item.Make(1f,1f,20f,1f,11f,0f,0.25f,0f,0f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,1f,20f,3f,1f,0f,0.25f,0f,0f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,1f,40f,4f,11f,0f,0.25f,0f,0f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,1f,40f,5f,1f,0f,0.25f,0f,0f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(6f,1f,20f,9f,11f,40f,12f));
    chordBeamControls.Items.Add(Item.Make(8f,2f,0f,40f,1f,200f));
    chordBeamControls.Items.Add(Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f));
    chordBeamControls.Items.Add(Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f));
    SetDocument(chordBeamControls, "");
    Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,5f,21f,0f,0.25f,3.99950051f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,1f,1.15499997f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,1f,2f,40f,22f),Item.Make(1f,1f,40f,9f,21f,0f,0.25f,7.00049925f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.523f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,40f,4f,1f,0f,0.25f,0f,0f,20f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("chordBeamControls staff1-fld differs from DOS.");
    Execute("FLU");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("chordBeamControls staff1-flu differs from DOS.");
    Execute("FLN");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("chordBeamControls staff1-fln differs from DOS.");
    Execute("FLD");Execute("2 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,5f,21f,0f,0.25f,4f,112f,0f,0f),Item.Make(1f,2f,20f,3f,1.39499998f,0f,0.25f,0f,110f,0f,0f),Item.Make(1f,2f,20f,1f,1.15499997f,0f,0.25f,0f,110f,0f,0f),Item.Make(1f,2f,40f,9f,21f,0f,0.25f,5f,112f,0f,0f),Item.Make(1f,2f,40f,5f,1.523f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,1f,0f,0.25f,0f,1110f,20f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("chordBeamControls staff2-fld differs from DOS.");
    Execute("FLU");Execute("2 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11.1549997f,0f,0.25f,4f,12f,0f,0f),Item.Make(1f,2f,20f,3f,1.39499998f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11.1199999f,0f,0.25f,5f,1112f,0f,0f),Item.Make(1f,2f,40f,5f,1.36000001f,0f,0.25f,0f,10f,10f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,1010f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("chordBeamControls staff2-flu differs from DOS.");
    Execute("FLN");Execute("2 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11.1549997f,0f,0.25f,4f,12f,0f,0f),Item.Make(1f,2f,20f,3f,1.39499998f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11.1199999f,0f,0.25f,5f,1112f,0f,0f),Item.Make(1f,2f,40f,5f,1.36000001f,0f,0.25f,0f,10f,10f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,1010f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Dotted-chord normalization differs from DOS.");
    var packedStemControls=NativeMeasurements.New(source);packedStemControls.Items.Clear();
    packedStemControls.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    packedStemControls.Items.Add(Item.Make(1f,1f,10f,4f,10f,0f,2f,3f,0f,10f,1f));
    packedStemControls.Items.Add(Item.Make(1f,1f,30f,4f,20f,0f,2f,3f,0f,10f,1f));
    packedStemControls.Items.Add(Item.Make(1f,1f,50f,4f,110f,0f,2f,3f,0f,10f,1f));
    packedStemControls.Items.Add(Item.Make(1f,1f,70f,4f,120f,0f,2f,3f,0f,10f,1f));
    packedStemControls.Items.Add(Item.Make(1f,1f,90f,4f,11.5f,0f,2f,3f,0f,10f,1f));
    SetDocument(packedStemControls, "");
    Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,10f,4f,20f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,30f,4f,20f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,50f,4f,120f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,70f,4f,120f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,90f,4f,21f,0f,2f,0f,0f,0f,1f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("packedStemControls fld differs from DOS.");
    Execute("FLU");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,10f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,30f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,50f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,70f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,90f,4f,11f,0f,2f,0f,0f,0f,1f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("packedStemControls flu differs from DOS.");
    Execute("FLN");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,10f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,30f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,50f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,70f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,90f,4f,11f,0f,2f,0f,0f,0f,1f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("packedStemControls fln differs from DOS.");
    var markFixture=NativeMeasurements.New(source);markFixture.Items.Clear();
    markFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    markFixture.Items.Add(Item.Make(1f,1f,5f,4f,10f,0f,0.5f,0f,1f,0f,1f));
    markFixture.Items.Add(Item.Make(1f,1f,10f,11f,10f,0f,0.5f,0f,1f,0f,2f));
    markFixture.Items.Add(Item.Make(1f,1f,15f,4f,10f,0f,0.5f,0f,1f,0f,3f));
    markFixture.Items.Add(Item.Make(1f,1f,20f,11f,10f,0f,0.5f,0f,1f,0f,4f));
    markFixture.Items.Add(Item.Make(1f,1f,25f,4f,10f,0f,0.5f,0f,1f,0f,5f));
    markFixture.Items.Add(Item.Make(1f,1f,30f,11f,10f,0f,0.5f,0f,1f,0f,6f));
    markFixture.Items.Add(Item.Make(1f,1f,35f,4f,10f,0f,0.5f,0f,1f,0f,7f));
    markFixture.Items.Add(Item.Make(1f,1f,40f,11f,10f,0f,0.5f,0f,1f,0f,8f));
    markFixture.Items.Add(Item.Make(1f,1f,45f,4f,10f,0f,0.5f,0f,1f,0f,9f));
    markFixture.Items.Add(Item.Make(1f,1f,50f,11f,10f,0f,0.5f,0f,1f,0f,10f));
    markFixture.Items.Add(Item.Make(1f,1f,55f,4f,10f,0f,0.5f,0f,1f,0f,11f));
    markFixture.Items.Add(Item.Make(1f,1f,60f,11f,10f,0f,0.5f,0f,1f,0f,12f));
    markFixture.Items.Add(Item.Make(1f,1f,65f,4f,10f,0f,0.5f,0f,1f,0f,13f));
    markFixture.Items.Add(Item.Make(1f,1f,70f,11f,10f,0f,0.5f,0f,1f,0f,14f));
    markFixture.Items.Add(Item.Make(1f,1f,75f,4f,10f,0f,0.5f,0f,1f,0f,15f));
    markFixture.Items.Add(Item.Make(1f,1f,80f,11f,10f,0f,0.5f,0f,1f,0f,16f));
    markFixture.Items.Add(Item.Make(1f,1f,85f,4f,10f,0f,0.5f,0f,1f,0f,17f));
    markFixture.Items.Add(Item.Make(1f,1f,90f,11f,10f,0f,0.5f,0f,1f,0f,18f));
    markFixture.Items.Add(Item.Make(1f,1f,95f,4f,10f,0f,0.5f,0f,1f,0f,19f));
    markFixture.Items.Add(Item.Make(1f,1f,100f,11f,10f,0f,0.5f,0f,1f,0f,20f));
    markFixture.Items.Add(Item.Make(1f,1f,105f,4f,10f,0f,0.5f,0f,1f,0f,21f));
    markFixture.Items.Add(Item.Make(1f,1f,110f,11f,10f,0f,0.5f,0f,1f,0f,22f));
    markFixture.Items.Add(Item.Make(1f,1f,115f,4f,10f,0f,0.5f,0f,1f,0f,23f));
    markFixture.Items.Add(Item.Make(1f,1f,120f,11f,10f,0f,0.5f,0f,1f,0f,24f));
    markFixture.Items.Add(Item.Make(1f,1f,125f,4f,10f,0f,0.5f,0f,1f,0f,25f));
    markFixture.Items.Add(Item.Make(1f,1f,130f,11f,10f,0f,0.5f,0f,1f,0f,26f));
    markFixture.Items.Add(Item.Make(1f,1f,135f,4f,10f,0f,0.5f,0f,1f,0f,27f));
    markFixture.Items.Add(Item.Make(1f,1f,140f,11f,10f,0f,0.5f,0f,1f,0f,28f));
    markFixture.Items.Add(Item.Make(1f,1f,145f,4f,10f,0f,0.5f,0f,1f,0f,29f));
    markFixture.Items.Add(Item.Make(1f,1f,150f,11f,10f,0f,0.5f,0f,1f,0f,30f));
    markFixture.Items.Add(Item.Make(1f,1f,155f,4f,10f,0f,0.5f,0f,1f,0f,31f));
    markFixture.Items.Add(Item.Make(1f,1f,160f,11f,10f,0f,0.5f,0f,1f,0f,32f));
    markFixture.Items.Add(Item.Make(1f,1f,165f,4f,10f,0f,0.5f,0f,1f,0f,33f));
    markFixture.Items.Add(Item.Make(1f,1f,170f,11f,10f,0f,0.5f,0f,1f,0f,34f));
    markFixture.Items.Add(Item.Make(1f,1f,175f,4f,10f,0f,0.5f,0f,1f,0f,35f));
    markFixture.Items.Add(Item.Make(1f,1f,180f,11f,10f,0f,0.5f,0f,1f,0f,36f));
    markFixture.Items.Add(Item.Make(1f,1f,185f,4f,10f,0f,0.5f,0f,1f,0f,4.4000001f));
    markFixture.Items.Add(Item.Make(1f,1f,190f,11f,10f,0f,0.5f,0f,1f,0f,5.5f));
    markFixture.Items.Add(Item.Make(1f,1f,195f,4f,10f,0f,0.5f,0f,1f,0f,8.80000019f));
    SetDocument(markFixture, "");
    Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,5f,4f,20f,0f,0.5f,0f,1f,0f,1f),Item.Make(1f,1f,10f,11f,20f,0f,0.5f,0f,1f,0f,2f),Item.Make(1f,1f,15f,4f,20f,0f,0.5f,0f,1f,0f,3f),Item.Make(1f,1f,20f,11f,20f,0f,0.5f,0f,1f,0f,4f),Item.Make(1f,1f,25f,4f,20f,0f,0.5f,0f,1f,0f,5.6500001f),Item.Make(1f,1f,30f,11f,20f,0f,0.5f,0f,1f,0f,6f),Item.Make(1f,1f,35f,4f,20f,0f,0.5f,0f,1f,0f,7f),Item.Make(1f,1f,40f,11f,20f,0f,0.5f,0f,1f,0f,8f),Item.Make(1f,1f,45f,4f,20f,0f,0.5f,0f,1f,0f,9.60000038f),Item.Make(1f,1f,50f,11f,20f,0f,0.5f,0f,1f,0f,10f),Item.Make(1f,1f,55f,4f,20f,0f,0.5f,0f,1f,0f,11f),Item.Make(1f,1f,60f,11f,20f,0f,0.5f,0f,1f,0f,12f),Item.Make(1f,1f,65f,4f,20f,0f,0.5f,0f,1f,0f,13f),Item.Make(1f,1f,70f,11f,20f,0f,0.5f,0f,1f,0f,14f),Item.Make(1f,1f,75f,4f,20f,0f,0.5f,0f,1f,0f,15f),Item.Make(1f,1f,80f,11f,20f,0f,0.5f,0f,1f,0f,16f),Item.Make(1f,1f,85f,4f,20f,0f,0.5f,0f,1f,0f,17f),Item.Make(1f,1f,90f,11f,20f,0f,0.5f,0f,1f,0f,18f),Item.Make(1f,1f,95f,4f,20f,0f,0.5f,0f,1f,0f,19.6000004f),Item.Make(1f,1f,100f,11f,20f,0f,0.5f,0f,1f,0f,20f),Item.Make(1f,1f,105f,4f,20f,0f,0.5f,0f,1f,0f,21f),Item.Make(1f,1f,110f,11f,20f,0f,0.5f,0f,1f,0f,22f),Item.Make(1f,1f,115f,4f,20f,0f,0.5f,0f,1f,0f,23f),Item.Make(1f,1f,120f,11f,20f,0f,0.5f,0f,1f,0f,24f),Item.Make(1f,1f,125f,4f,20f,0f,0.5f,0f,1f,0f,25f),Item.Make(1f,1f,130f,11f,20f,0f,0.5f,0f,1f,0f,26f),Item.Make(1f,1f,135f,4f,20f,0f,0.5f,0f,1f,0f,27f),Item.Make(1f,1f,140f,11f,20f,0f,0.5f,0f,1f,0f,28f),Item.Make(1f,1f,145f,4f,20f,0f,0.5f,0f,1f,0f,29f),Item.Make(1f,1f,150f,11f,20f,0f,0.5f,0f,1f,0f,30f),Item.Make(1f,1f,155f,4f,20f,0f,0.5f,0f,1f,0f,31f),Item.Make(1f,1f,160f,11f,20f,0f,0.5f,0f,1f,0f,32f),Item.Make(1f,1f,165f,4f,20f,0f,0.5f,0f,1f,0f,33f),Item.Make(1f,1f,170f,11f,20f,0f,0.5f,0f,1f,0f,34f),Item.Make(1f,1f,175f,4f,20f,0f,0.5f,0f,1f,0f,35f),Item.Make(1f,1f,180f,11f,20f,0f,0.5f,0f,1f,0f,36f),Item.Make(1f,1f,185f,4f,20f,0f,0.5f,0f,1f,0f,4.5999999f),Item.Make(1f,1f,190f,11f,20f,0f,0.5f,0f,1f,0f,5.05000019f),Item.Make(1f,1f,195f,4f,20f,0f,0.5f,0f,1f,0f,8.60000038f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Attached mark FLD differs from DOS.");
    Execute("FLU");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,5f,4f,10f,0f,0.5f,0f,1f,0f,1f),Item.Make(1f,1f,10f,11f,10f,0f,0.5f,0f,1f,0f,2f),Item.Make(1f,1f,15f,4f,10f,0f,0.5f,0f,1f,0f,3f),Item.Make(1f,1f,20f,11f,10f,0f,0.5f,0f,1f,0f,4.69999981f),Item.Make(1f,1f,25f,4f,10f,0f,0.5f,0f,1f,0f,5.05000019f),Item.Make(1f,1f,30f,11f,10f,0f,0.5f,0f,1f,0f,6f),Item.Make(1f,1f,35f,4f,10f,0f,0.5f,0f,1f,0f,7f),Item.Make(1f,1f,40f,11f,10f,0f,0.5f,0f,1f,0f,8.69999981f),Item.Make(1f,1f,45f,4f,10f,0f,0.5f,0f,1f,0f,9f),Item.Make(1f,1f,50f,11f,10f,0f,0.5f,0f,1f,0f,10.6999998f),Item.Make(1f,1f,55f,4f,10f,0f,0.5f,0f,1f,0f,11f),Item.Make(1f,1f,60f,11f,10f,0f,0.5f,0f,1f,0f,12f),Item.Make(1f,1f,65f,4f,10f,0f,0.5f,0f,1f,0f,13f),Item.Make(1f,1f,70f,11f,10f,0f,0.5f,0f,1f,0f,14f),Item.Make(1f,1f,75f,4f,10f,0f,0.5f,0f,1f,0f,15f),Item.Make(1f,1f,80f,11f,10f,0f,0.5f,0f,1f,0f,16f),Item.Make(1f,1f,85f,4f,10f,0f,0.5f,0f,1f,0f,17f),Item.Make(1f,1f,90f,11f,10f,0f,0.5f,0f,1f,0f,18f),Item.Make(1f,1f,95f,4f,10f,0f,0.5f,0f,1f,0f,19f),Item.Make(1f,1f,100f,11f,10f,0f,0.5f,0f,1f,0f,20f),Item.Make(1f,1f,105f,4f,10f,0f,0.5f,0f,1f,0f,21f),Item.Make(1f,1f,110f,11f,10f,0f,0.5f,0f,1f,0f,22f),Item.Make(1f,1f,115f,4f,10f,0f,0.5f,0f,1f,0f,23f),Item.Make(1f,1f,120f,11f,10f,0f,0.5f,0f,1f,0f,24f),Item.Make(1f,1f,125f,4f,10f,0f,0.5f,0f,1f,0f,25f),Item.Make(1f,1f,130f,11f,10f,0f,0.5f,0f,1f,0f,26f),Item.Make(1f,1f,135f,4f,10f,0f,0.5f,0f,1f,0f,27f),Item.Make(1f,1f,140f,11f,10f,0f,0.5f,0f,1f,0f,28f),Item.Make(1f,1f,145f,4f,10f,0f,0.5f,0f,1f,0f,29f),Item.Make(1f,1f,150f,11f,10f,0f,0.5f,0f,1f,0f,30f),Item.Make(1f,1f,155f,4f,10f,0f,0.5f,0f,1f,0f,31f),Item.Make(1f,1f,160f,11f,10f,0f,0.5f,0f,1f,0f,32f),Item.Make(1f,1f,165f,4f,10f,0f,0.5f,0f,1f,0f,33f),Item.Make(1f,1f,170f,11f,10f,0f,0.5f,0f,1f,0f,34f),Item.Make(1f,1f,175f,4f,10f,0f,0.5f,0f,1f,0f,35f),Item.Make(1f,1f,180f,11f,10f,0f,0.5f,0f,1f,0f,36f),Item.Make(1f,1f,185f,4f,10f,0f,0.5f,0f,1f,0f,4f),Item.Make(1f,1f,190f,11f,10f,0f,0.5f,0f,1f,0f,5.75f),Item.Make(1f,1f,195f,4f,10f,0f,0.5f,0f,1f,0f,8f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Attached mark FLU differs from DOS.");
    var cueBeamFixture=NativeMeasurements.New(source);cueBeamFixture.Items.Clear();
    cueBeamFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,1f,20f,103f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,1f,30f,105f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,1f,40f,107f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,1f,50f,110f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(6f,1f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(8f,2f,0f,25f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,2f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    cueBeamFixture.Items.Add(Item.Make(1f,2f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    cueBeamFixture.Items.Add(Item.Make(1f,2f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    cueBeamFixture.Items.Add(Item.Make(1f,2f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f));
    cueBeamFixture.Items.Add(Item.Make(6f,2f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(8f,3f,0f,50f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,3f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,3f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,3f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,3f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(6f,3f,20f,8f,10f,50f,12f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(8f,4f,0f,75f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,4f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,4f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,4f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,4f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(6f,4f,20f,8f,10f,50f,2f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(8f,5f,0f,100f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,5f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,5f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,5f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,5f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(6f,5f,20f,8f,10f,50f,-12f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(8f,6f,0f,125f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,6f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,6f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,6f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,6f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(6f,6f,20f,8f,10f,50f,12f,0f,1f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(8f,7f,0f,150f,1f,200f));
    cueBeamFixture.Items.Add(Item.Make(1f,7f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,7f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,7f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(1f,7f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f));
    cueBeamFixture.Items.Add(Item.Make(6f,7f,20f,8f,10f,50f,12f,0f,0f,0f,1f));
    SetDocument(cueBeamFixture, "");Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,103f,20f,0f,0.25f,1.3327831f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,1f,20f,102.199997f,3.20000005f,50f,22f,0f,0f,0f,0f),Item.Make(1f,1f,30f,105f,20f,0f,0.25f,4.11092901f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,40f,107f,20f,0f,0.25f,6.88907528f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,50f,110f,20f,0f,0.25f,11.3338881f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(6f,2f,20f,8f,10f,50f,12f,0f,0f,0f,0f),Item.Make(1f,2f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(6f,3f,20f,8f,10f,50f,12f,0f,0f,0f,0f),Item.Make(1f,3f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,4f,20f,8f,10f,50f,2f,0f,0f,0f,0f),Item.Make(1f,4f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,5f,0f,100f,1f,200f),Item.Make(1f,5f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,5f,20f,8f,10f,50f,-12f,0f,0f,0f,0f),Item.Make(1f,5f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,5f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,5f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,6f,0f,125f,1f,200f),Item.Make(1f,6f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,6f,20f,8f,10f,50f,12f,0f,1f,0f,0f),Item.Make(1f,6f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,7f,0f,150f,1f,200f),Item.Make(1f,7f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,7f,20f,8f,10f,50f,12f,0f,0f,0f,1f),Item.Make(1f,7f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,7f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,7f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Cue-beam flip differs from the original records.");
    byte[] cueBefore=score.Encode();Execute("AD 1");
    if(!score.Encode().SequenceEqual(cueBefore))throw new Exception("AD changed the already fitted original cue beam.");
    var markedSlurFixture=NativeMeasurements.New(source);markedSlurFixture.Items.Clear();
    markedSlurFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    markedSlurFixture.Items.Add(Item.Make(1f,1f,20f,4f,21f,0f,0.5f,0f,1f,0f,4f));
    markedSlurFixture.Items.Add(Item.Make(5f,1f,20f,6f,13f,40f,2.5999999f,-1f));
    markedSlurFixture.Items.Add(Item.Make(1f,1f,40f,11f,12f,0f,0.5f,0f,1f,0f,4f));
    markedSlurFixture.Items.Add(Item.Make(8f,2f,0f,25f,1f,200f));
    markedSlurFixture.Items.Add(Item.Make(1f,2f,20f,4f,21f,0f,0.5f,0f,1f,0f,6f));
    markedSlurFixture.Items.Add(Item.Make(5f,2f,20f,6f,13f,40f,2.5999999f,-1f));
    markedSlurFixture.Items.Add(Item.Make(1f,2f,40f,11f,12f,0f,0.5f,0f,1f,0f,6f));
    markedSlurFixture.Items.Add(Item.Make(8f,3f,0f,50f,1f,200f));
    markedSlurFixture.Items.Add(Item.Make(1f,3f,20f,4f,21f,0f,0.5f,0f,1f,0f,19f));
    markedSlurFixture.Items.Add(Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,-1f));
    markedSlurFixture.Items.Add(Item.Make(1f,3f,40f,11f,12f,0f,0.5f,0f,1f,0f,19f));
    SetDocument(markedSlurFixture, "");Execute("FLN");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,11f,0f,0.5f,0f,1f,0f,4f),Item.Make(5f,1f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,1f,40f,11f,22f,0f,0.5f,0f,1f,0f,4.17999983f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,21f,0f,0.5f,0f,1f,0f,6f),Item.Make(5f,2f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,2f,40f,11f,12f,0f,0.5f,0f,1f,0f,6f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,21f,0f,0.5f,0f,1f,0f,19f),Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,3f,40f,11f,12f,0f,0.5f,0f,1f,0f,19f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Marked slur normalization differs from DOS.");
    Execute("FLN");Execute("2 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,11f,0f,0.5f,0f,1f,0f,4f),Item.Make(5f,1f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,1f,40f,11f,22f,0f,0.5f,0f,1f,0f,4.17999983f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,11f,0f,0.5f,0f,1f,0f,6f),Item.Make(5f,2f,20f,13f,14.5f,40f,2.04999995f,-1f),Item.Make(1f,2f,40f,11f,22f,0f,0.5f,0f,1f,0f,6f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,21f,0f,0.5f,0f,1f,0f,19f),Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,3f,40f,11f,12f,0f,0.5f,0f,1f,0f,19f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Marked-slur control 2 differs from DOS.");
    Execute("FLN");Execute("3 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,11f,0f,0.5f,0f,1f,0f,4f),Item.Make(5f,1f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,1f,40f,11f,22f,0f,0.5f,0f,1f,0f,4.17999983f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,11f,0f,0.5f,0f,1f,0f,6f),Item.Make(5f,2f,20f,13f,14.5f,40f,2.04999995f,-1f),Item.Make(1f,2f,40f,11f,22f,0f,0.5f,0f,1f,0f,6f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,11f,0f,0.5f,0f,1f,0f,19f),Item.Make(5f,3f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,3f,40f,11f,22f,0f,0.5f,0f,1f,0f,19.1800003f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Marked-slur control 3 differs from DOS.");
    var packedChordFixture=NativeMeasurements.New(source);packedChordFixture.Items.Clear();
    packedChordFixture.Items.Add(Item.Make(8f,1f,0f,0f,1f,200f));
    packedChordFixture.Items.Add(Item.Make(1f,1f,30f,3f,111f,0f,0.5f,2f,1f,0f,0f));
    packedChordFixture.Items.Add(Item.Make(1f,1f,30f,4f,101f,0f,0.5f,0f,0f,0f,0f));
    packedChordFixture.Items.Add(Item.Make(1f,1f,30f,5f,101f,0f,0.5f,0f,0f,0f,0f));
    packedChordFixture.Items.Add(Item.Make(8f,2f,0f,40f,1f,200f));
    packedChordFixture.Items.Add(Item.Make(1f,2f,30f,103f,11f,0f,0.5f,2f,1f,0f,0f));
    packedChordFixture.Items.Add(Item.Make(1f,2f,30f,104f,1f,0f,0.5f,0f,0f,0f,0f));
    packedChordFixture.Items.Add(Item.Make(1f,2f,30f,105f,1f,0f,0.5f,0f,0f,0f,0f));
    packedChordFixture.Items.Add(Item.Make(8f,3f,0f,80f,1f,200f));
    packedChordFixture.Items.Add(Item.Make(1f,3f,30f,3f,11f,0f,0.5f,2f,1f,0f,4f));
    packedChordFixture.Items.Add(Item.Make(1f,3f,30f,4f,1f,0f,0.5f,0f,0f,0f,4f));
    packedChordFixture.Items.Add(Item.Make(1f,3f,30f,5f,1f,0f,0.5f,0f,0f,0f,4f));
    SetDocument(packedChordFixture, "");Execute("FLD");Execute("1 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,103f,11f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1f,0f,0.5f,0f,0f,0f,0f),Item.Make(1f,2f,30f,105f,1f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,3f,11f,0f,0.5f,2f,1f,0f,4f),Item.Make(1f,3f,30f,4f,1f,0f,0.5f,0f,0f,0f,4f),Item.Make(1f,3f,30f,5f,1f,0f,0.5f,0f,0f,0f,4f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Parenthesized chord flip differs from DOS.");
    Execute("FLD");Execute("2 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,105f,21.2830009f,0f,0.5f,4f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,2f,30f,103f,1.76300001f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,3f,11f,0f,0.5f,2f,1f,0f,4f),Item.Make(1f,3f,30f,4f,1f,0f,0.5f,0f,0f,0f,4f),Item.Make(1f,3f,30f,5f,1f,0f,0.5f,0f,0f,0f,4f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Cue chord flip differs from DOS.");
    Execute("FLD");Execute("3 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,105f,21.2830009f,0f,0.5f,4f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,2f,30f,103f,1.76300001f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,5f,21.2830009f,0f,0.5f,2f,1f,0f,4.5f),Item.Make(1f,3f,30f,4f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,3f,30f,3f,1.76300001f,0f,0.5f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Marked chord flip differs from DOS.");
    Execute("FLN");Execute("2 0 220");Execute("");
    if(!score.Items.Zip(new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,103f,11.4799995f,0f,0.5f,4f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1.24000001f,0f,0.5f,0f,0f,10f,0f),Item.Make(1f,2f,30f,105f,1f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,5f,21.2830009f,0f,0.5f,2f,1f,0f,4.5f),Item.Make(1f,3f,30f,4f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,3f,30f,3f,1.76300001f,0f,0.5f,0f,0f,0f,0f)},(actual,wanted)=>actual.Bytes.SequenceEqual(wanted.Bytes)).All(equal=>equal))
     throw new Exception("Cue chord normalization differs from DOS.");
    var slurNormalFixture=NativeMeasurements.New(source);
    slurNormalFixture.Items.Clear();
    slurNormalFixture.Items.Add(Item.Make(8,1,0,0,1,200));
    slurNormalFixture.Items.Add(Item.Make(1,1,20,4,21,0,.5f,0,1,0,0));
    slurNormalFixture.Items.Add(Item.Make(5,1,20,6,13,40,2.6f,-1));
    slurNormalFixture.Items.Add(Item.Make(1,1,40,11,12,0,.5f,0,1,0,0));
    slurNormalFixture.Items.Add(Item.Make(5,1,40,8,4,50,-1.8f,-1));
    slurNormalFixture.Items.Add(Item.Make(1,1,50,7,10,0,.5f,0,1,0,0));
    SetDocument(slurNormalFixture,"");
    Execute("FLN");Execute("1 0 220");Execute("");
    if(score.Items[2].P(4)!=13||score.Items[2].P(5)!=13||score.Items[2].P(7)!=1.899999976158142f||
       score.Items[4].P(4)!=13||score.Items[4].P(5)!=10||score.Items[4].P(7)!=1.6999999284744263f)
     throw new Exception("FLN did not match the DOS normalized slurs.");
    var enharmonicFixture=NativeMeasurements.New(source);
    enharmonicFixture.Items.Clear();
    enharmonicFixture.Items.Add(Item.Make(8,1,0,0,1,200));
    enharmonicFixture.Items.Add(Item.Make(1,1,20,4,11,0,.5f,2,1));
    enharmonicFixture.Items.Add(Item.Make(1,1,40,11,22,0,.5f,1,1));
    SetDocument(enharmonicFixture,"");
    Execute("ENH");Execute("1 0 220");Execute("");
    if(!pendingEditAcknowledgement||score.Items[1].P(5)!=11||score.Items[2].P(5)!=22)
     throw new Exception("ENH did not reject mixed sharps and flats.");
    AcknowledgeEditPause();
    Execute("ENH");Execute("1 20 20");Execute("");Execute("N");
    Execute("ENH");Execute("1 40 40");Execute("");Execute("N");
    if(score.Items[1].P(4)!=3||score.Items[1].P(5)!=13||score.Items[1].P(8)!=2||
       score.Items[2].P(4)!=12||score.Items[2].P(5)!=21||score.Items[2].P(8)!=1)
     throw new Exception("ENH did not match the DOS flat and sharp records.");
    Execute("ENH");Execute("1 20 20");Execute("");
    if(!pendingEditAcknowledgement||score.Items[1].P(5)!=13)
     throw new Exception("ENH did not preserve the DOS natural-only range.");
    AcknowledgeEditPause();
    Console.WriteLine("PASS: DOS CRV, AD, STM, ENH and ordinary flip notation edits.");
   }finally{ClearMainGroup();SetDocument(previous,previousFile);}
  }

  // Original resident 13A9:0006 stores the span, divides it by DS:69BE (20),
  // then adds the unrounded slope product and DS:69C2. Multiplying by .05f differs.
  static float OriginalCurvature(float left,float y1,float y2,float right){
   float span=(float)((double)right-left);
   return (float)(Math.Abs((double)y2-y1)*.1f+(double)span/20+.9f);
  }
  static Item WithOriginalCurvature(Item item){
   if(item.Code!=5||(item.P(8)!=-1&&item.P(8)!=0))return item;
   float curvature=OriginalCurvature(item.P(3),item.P(4),item.P(5),item.P(6));
   if(item.P(7)<0)curvature=-curvature;
   if(item.P(7)==curvature)return item;
   var changed=item.Clone();
   changed.Set(7,curvature);
   return changed;
  }
 }
}
