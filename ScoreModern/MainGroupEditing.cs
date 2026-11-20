using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  const string GroupSourcePrompt="Enter Staff #, Left position, Right position, [Code #, Upper Staff #]";
  const string GroupDestinationPrompt="Enter new Staff #, Left pos., Right pos., [steps up-down]";
  const string GroupAlterPrompt="Type P#, chng,  P#, chng,  P#,  chng, ...";
  const string GroupDeletePrompt="Press \"B\" or <ESC> to backout, <Enter> to continue. ->";
  const string SystemSourcePrompt="Enter lowest staff # of system and how many staves in system. ->";
  const string SystemDestinationPrompt="Type new bottom staff number ->";
  string groupCommand;
  float[] groupRange;
  float originalNotationStaff=1;
  float[] originalNotationRange={1,0,220,0,1};

  void StartMainGroup(string verb,string arguments){
   RequireNoLetterArguments(verb,arguments);
   groupCommand=verb;
   groupRange=null;
   Message(IsSystemGroup?SystemSourcePrompt:GroupSourcePrompt);
   command.Focus();
  }

  void ClearMainGroup(){if(IsTransposeGroup)transposeDisplayStart=null;if(IsLayoutGroup){layoutPreparation=null;layoutScales=null;}groupCommand=null;groupRange=null;pendingScor4Paste=null;}
  bool IsSystemGroup {get{return groupCommand=="CS"||groupCommand=="MS";}}

  bool HandleMainGroupKey(KeyEventArgs key){
   if(groupCommand==null)return false;
   if(mode==ScreenMode.EditGroupMenu&&key.KeyCode==Keys.Escape&&(!IsLayoutGroup||layoutPrompt==LayoutPrompt.Confirm)&&(!IsTransposeGroup||transposePrompt==TransposePrompt.Interval)){originalGroupFlipQuestion=false;transposeOperation=null;ClearMainGroup();command.Clear();UpdateClassic();key.SuppressKeyPress=true;return true;}
   if(groupCommand=="COM"&&layoutPrompt==LayoutPrompt.Discard&&!key.Control&&!key.Alt&&(key.KeyCode==Keys.Y||key.KeyCode==Keys.N)){SubmitMainCombine(key.KeyCode==Keys.Y?"Y":"N");command.Clear();key.SuppressKeyPress=true;return true;}
   if(groupCommand=="DEC"&&decombineOverwrite&&!key.Control&&!key.Alt&&(key.KeyCode==Keys.Y||key.KeyCode==Keys.N)){SubmitMainDecombine(key.KeyCode==Keys.Y?"Y":"N");command.Clear();key.SuppressKeyPress=true;return true;}
   if(groupCommand=="DEC"&&layoutPrompt==LayoutPrompt.Continue&&key.KeyCode!=Keys.ShiftKey&&key.KeyCode!=Keys.ControlKey&&key.KeyCode!=Keys.Menu){SubmitMainDecombine("");command.Clear();key.SuppressKeyPress=true;return true;}
   if(IsLayoutGroup&&layoutPrompt==LayoutPrompt.Duplicate&&!key.Control&&!key.Alt){
    if(key.KeyCode==Keys.Y||key.KeyCode==Keys.N){SubmitMainLayout(key.KeyCode==Keys.Y?"Y":"N");command.Clear();key.SuppressKeyPress=true;return true;}
    if(key.KeyCode==Keys.Escape||key.KeyCode==Keys.B||key.KeyCode==Keys.Enter){LayoutDuplicateQuestion(true);command.Clear();key.SuppressKeyPress=true;return true;}
   }
   if(key.KeyCode==Keys.Escape){
    if(IsLayoutGroup){if(groupCommand=="SEP")BackoutMainSeparate();else if((groupCommand=="J"||groupCommand=="JT")&&layoutPrompt==LayoutPrompt.Range)SubmitMainLayout("B");else if(groupCommand=="JT"&&layoutPrompt==LayoutPrompt.Text)SubmitMainLayout("B");else if(groupCommand=="VJ"&&layoutPrompt==LayoutPrompt.Staff)SubmitMainVerticalJustification("B");else{ClearMainGroup();Message("");}command.Clear();key.SuppressKeyPress=true;return true;}
    if(IsTransposeGroup){SubmitMainTranspose(transposePrompt==TransposePrompt.Warning?"":"B");command.Clear();key.SuppressKeyPress=true;return true;}
    if(groupRange==null||groupCommand=="DE"||groupCommand=="CT"||groupCommand=="CRV"||IsFlipGroup||groupCommand=="ENH"||groupCommand==EnharmonicNormalize||groupCommand=="STUD"){ClearMainGroup();Message("");}
    else{groupRange=null;Message(IsSystemGroup?SystemSourcePrompt:GroupSourcePrompt);}
    command.Clear();
   }else if(!key.Control&&!key.Alt&&(key.KeyCode<Keys.F1||key.KeyCode>Keys.F24))return false;
   key.SuppressKeyPress=true;
   return true;
  }

  bool SubmitMainGroup(string text){
   bool packed=groupCommand=="M"||groupCommand=="C"||groupCommand=="A"||groupCommand=="DE"||groupCommand=="J"||groupCommand=="LJ"||groupCommand=="LI"||groupCommand=="TR"||groupCommand=="TRK"||IsFlipGroup;
   if(!packed)return SubmitMainGroupCore(text);
   using(var storage=BeginParameterMutation()){bool result=SubmitMainGroupCore(text);storage.Complete();return result;}
  }
  bool SubmitMainGroupCore(string text){
   if(groupCommand==null)return false;
   if(groupCommand=="PST"){SubmitScor4Paste(text);return true;}
   if(groupCommand=="EDI"){SubmitOriginalEditInput(text);return true;}
   if(IsLayoutGroup){SubmitMainLayout(text);return true;}
   if(IsTransposeGroup){SubmitMainTranspose(text);return true;}
   if(groupCommand=="STUD"){SubmitOriginalStemSpacing(text);return true;}
   if(groupCommand==EnharmonicNormalize){
    string answer=text.Trim().ToUpperInvariant();
    if(answer=="N"){ClearMainGroup();Message("");}
    else if(answer==""||answer=="Y")CommitOriginalFlip("FLN");
    else throw new InvalidDataException(EnharmonicNormalizePrompt);
    return true;
   }
   if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){
    HandleMainGroupKey(new KeyEventArgs(Keys.Escape));
    return true;
   }
   if(groupCommand=="CT"&&groupRange!=null){SubmitScor4CutCode(text);return true;}
   if(groupCommand=="ENH"&&groupRange!=null){
    if(!String.IsNullOrWhiteSpace(text))throw new InvalidDataException(GroupDeletePrompt);
    CommitOriginalGroupEnharmonic();
    return true;
   }
   if(IsFlipGroup&&groupRange!=null){
    if(!String.IsNullOrWhiteSpace(text))throw new InvalidDataException(GroupDeletePrompt);
    CommitOriginalFlip();
    return true;
   }
   if(groupCommand=="CRV"&&groupRange!=null){
    if(!String.IsNullOrWhiteSpace(text))throw new InvalidDataException(GroupDeletePrompt);
    var order=score.Items.Select(item=>InMainGroup(item)?WithOriginalCurvature(item):item).ToArray();
    ClearMainGroup();
    ApplyMainOrder(order,"");
    return true;
   }
   if(groupCommand=="DE"&&groupRange!=null){
    if(!String.IsNullOrWhiteSpace(text))throw new InvalidDataException(GroupDeletePrompt);
    var kept=score.Items.Where(item=>!InMainGroup(item)).ToList();
    ClearMainGroup();
    ApplyMainOrder(kept,"");
    return true;
   }
   var fields=text.Split(new[]{' ',',','\t'},StringSplitOptions.RemoveEmptyEntries);
   var values=new float[fields.Length];
   for(int i=0;i<fields.Length;i++)
    if(!Single.TryParse(fields[i],NumberStyles.Float,CultureInfo.InvariantCulture,out values[i])||
       Single.IsNaN(values[i])||Single.IsInfinity(values[i]))
     throw new InvalidDataException("Enter finite SCORE group parameters.");
   if(IsSystemGroup){
    SubmitSystemGroup(values);
    return true;
   }
   if(groupRange==null){
    if((IsFlipGroup||groupCommand=="ENH")&&values.Length>3)
     throw new InvalidDataException("Enter staff number and up to two horizontal positions.");
    if(values.Length<(groupCommand=="A"?4:1)||values.Length>5)
     throw new InvalidDataException("Enter up to five source parameters, starting with the staff number.");
    Array.Resize(ref values,5);
    int staff;
    if(IsFlipGroup||groupCommand=="ENH"){
     if(values[0]<1||values[0]>=32768)throw new NotSupportedException("Notation staff conversion is outside the original positive signed-word domain.");
     staff=(short)Math.Truncate(values[0]);
     // The original retains the prior staff/range and executes immediately
     // when the entered staff word exceeds 32. It does not clamp to the score.
     if(staff>32){
      groupRange=(float[])originalNotationRange.Clone();
      groupRange[0]=groupRange[4]=(short)Math.Truncate(originalNotationStaff);
      if(IsFlipGroup)CommitOriginalFlip();else CommitOriginalGroupEnharmonic();
      return true;
     }
     originalNotationStaff=values[0];
     values[0]=staff;
    }else staff=GroupInteger(values[0],1,Int16.MaxValue);
    if(values[2]==0)values[2]=220;
    if(values[4]==0)values[4]=staff;
    GroupInteger(values[4],staff,Int16.MaxValue);
    GroupInteger(values[3],groupCommand=="A"?1:0,99);
    if(values[2]<values[1])throw new InvalidDataException("The right position must be at or after the left position.");
    groupRange=values;
    if(groupCommand=="CT"){
     Message(String.Format(CultureInfo.InvariantCulture,
      "Lower Staff {0,2} {1,8:0.000}     Upper Staff {2,2} {3,8:0.000}\r\nType Code number. 0 = all.",
      staff,values[1],values[4],values[2]));
     return true;
    }
    if(IsFlipGroup||groupCommand=="ENH")originalNotationRange=(float[])values.Clone();
    Message(groupCommand=="A"?GroupAlterPrompt:groupCommand=="DE"||groupCommand=="CRV"||IsFlipGroup||groupCommand=="ENH"?GroupDeletePrompt:GroupDestinationPrompt);
    return true;
   }
   if(groupCommand=="A"){
    AlterMainGroup(values);
    return true;
   }
   if(values.Length<1||values.Length>4)throw new InvalidDataException("Enter a destination staff and up to three movement parameters.");
   Array.Resize(ref values,4);
   bool allStaves=groupRange[0]>32;
   int destination=GroupInteger(values[0],1,allStaves?Int16.MaxValue:32);
   if(allStaves&&destination!=groupRange[0])
    throw new NotSupportedException("For all-staff movement, repeat the source staff selector at the destination prompt.");
   bool scale=values[2]!=0;
   if(scale&&(groupRange[1]==groupRange[2]||values[2]<=values[1]))
    throw new InvalidDataException("Scaling requires increasing source and destination ranges.");
   float factor=scale?(float)(((double)values[2]-values[1])/((double)groupRange[2]-groupRange[1])):1;
   float origin=scale?(float)(values[1]-(double)groupRange[1]*factor):0;
   int deltaStaff=destination-(int)groupRange[0];
   var changes=new List<KeyValuePair<int,Item>>();
   for(int index=0;index<score.Items.Count;index++){
    var original=score.Items[index];
    if(!InMainGroup(original))continue;
    if(original.Code<1||original.Code>18)
     throw new NotSupportedException("Group movement is not verified for code "+original.Code+" in this mode.");
    if(original.Staff+deltaStaff<1||original.Staff+deltaStaff>32)
     throw new InvalidDataException("The move would put an item outside staff 1–32.");
    var changed=original.Clone();
    if(deltaStaff!=0)changed.Set(2,original.P(2)+deltaStaff);
    if(changed.Code==8&&changed.Count>=6&&changed.P(6)==0&&(scale||values[1]!=0))
     changed.Set(6,200);
    if(scale)
     NativeItemMotion.TransformHorizontal(changed,x=>(float)(origin+(double)x*factor));
    else NativeItemMotion.Horizontal(changed,values[1]);
    if(changed.Count>=4&&changed.Code!=3&&changed.Code!=8&&changed.Code!=17&&changed.Code!=18)
     NativeItemMotion.Vertical(changed,values[3]);
    if(Single.IsInfinity(changed.P(3))||Single.IsInfinity(changed.P(4)))
     throw new InvalidDataException("The movement would create an infinite position.");
    changes.Add(new KeyValuePair<int,Item>(index,changed));
   }
   bool copy=groupCommand=="C";
   var next=score.Items.ToList();
   foreach(var change in changes){
    if(copy)next.Add(change.Value);
    else if(!next[change.Key].Bytes.SequenceEqual(change.Value.Bytes))next[change.Key]=change.Value;
   }
   bool trackDisplay=changes.Count>0&&changes.All(change=>
    NativeRetainedDisplayArray.SupportsNote(score.Items[change.Key])&&
    NativeRetainedDisplayArray.SupportsNote(change.Value))&&CanTrackDisplayRedraw();
   ClearMainGroup();
   ApplyMainOrder(next,"",trackDisplay);
   return true;
  }

  void SubmitSystemGroup(float[] values){
   if(groupRange==null){
    if(values.Length!=2)throw new InvalidDataException("Enter the lowest staff and number of staves in the system.");
    int lowest=GroupInteger(values[0],1,32);
    GroupInteger(values[1],1,33-lowest);
    groupRange=values;
    Message(SystemDestinationPrompt);
    return;
   }
   if(values.Length!=1)throw new InvalidDataException("Enter the new lowest staff number.");
   int sourceStaff=(int)groupRange[0],span=(int)groupRange[1];
   int destination=GroupInteger(values[0],1,33-span);
   int shift=destination-sourceStaff;
   bool copy=groupCommand=="CS";
   var order=score.Items.ToList();
   for(int i=0;i<score.Items.Count;i++){
    var item=score.Items[i];
    if(item.Staff<sourceStaff||item.Staff>=sourceStaff+span||!copy&&shift==0)continue;
    var changed=item.Clone();
    changed.Set(2,item.P(2)+shift);
    if(copy)order.Add(changed);else order[i]=changed;
   }
   ClearMainGroup();
   ApplyMainOrder(order,"");
  }

  bool InMainGroup(Item item){
   float left=groupRange[1]-.001f,right=groupRange[2]+.001f;
   return (groupRange[0]>32||item.Staff>=groupRange[0]&&item.Staff<=groupRange[4])&&
    item.P(3)>=left&&item.P(3)<=right&&
    (groupRange[3]==0||item.Code==(int)groupRange[3]);
  }

  void AlterMainGroup(float[] values){
   if(values.Length<2||values.Length>24||values.Length%2!=0)
    throw new InvalidDataException("Enter up to twelve complete parameter and value pairs.");
   var changes=new List<OriginalParameterOperation>();
   for(int index=0;index<values.Length;index+=2){
    int control=GroupInteger(values[index],3,19000);
    bool copy=control>=3000&&control%1000==0;
    bool add=control>=300&&control<=1900&&control%100==0;
    int parameter=copy?control/1000:add?control/100:control;
    if(parameter<3||parameter>19)throw new NotSupportedException("Group alter supports P3–P19.");
    int sourceParameter=copy?GroupInteger(values[index+1],1,19):0;
    if(copy&&sourceParameter<3)continue;
    changes.Add(new OriginalParameterOperation{Parameter=parameter,Add=add,Copy=copy,
     Source=sourceParameter,Value=values[index+1]});
   }
   if(changes.Count==0){ClearMainGroup();Message("");return;}
   var order=score.Items.ToList();
   var warnings=new List<string>();
   var displayOrdinals=new List<int>();
   bool trackedNotes=true;
   for(int i=0;i<order.Count;i++){
    if(!InMainGroup(order[i]))continue;
    var original=order[i];
    int code=original.Code;
    if(code<1||code>18)
     throw new NotSupportedException("Group alteration requires a SCORE item code from 1 to 18.");
    bool packed=code==11||code==15||code==16;
    var changed=original.Clone();
    foreach(var change in changes){
     // The DOS text editor leaves length and width in the numeric record,
     // but exposes zero for the detached text words.
     if(code==16&&change.Parameter>=12)continue;
     float value=change.Copy?(code==16&&change.Source>=14?0:changed.P(change.Source)):
      change.Add?changed.P(change.Parameter)+change.Value:change.Value;
     if(Single.IsInfinity(value))throw new InvalidDataException("The alteration would create an infinite parameter.");
     if(code==2&&change.Parameter==9&&value==-1){
      order[i]=changed;
      var workingScore=new Score{Items=order,Trailer=score.Trailer,SourcePath=score.SourcePath};
      NativeRestCentering.Apply(NativeRestCentering.PlanItem(workingScore,changed));
     }else changed.Set(change.Parameter,value);
    }
    if((code==11||code==15)&&original.Count>=13){
     // DOS applies the entire command to a working record before repacking the
     // original filename. A later equivalence can read a temporary write.
     const int payloadOffset=56;
     Array.Copy(original.Bytes,payloadOffset,changed.Bytes,payloadOffset,original.Bytes.Length-payloadOffset);
     if(changed.Count!=original.Count){
      Array.Resize(ref changed.Bytes,original.Bytes.Length);
      Array.Copy(original.Bytes,changed.Bytes,4);
     }
     if(original.HasVerifiedShortDrawingName)changed.RestoreShortDrawingPadding();
    }else if(!packed)changed.TrimEditedNumericParameters();
    if((code==15||code==16)&&Math.Abs(changed.P(10))>=32767f){
     changed.Set(10,0);
     warnings.Add(String.Format(CultureInfo.InvariantCulture,
      "*** Code#{0},  P10 too big.  Staff {1,3},  P3={2,7:F2}",code,changed.Staff,changed.P(3)));
    }
    if(code==16&&Enumerable.Range(5,7).Any(parameter=>changed.P(parameter)!=original.P(parameter))){
     order[i]=changed;
     var workingScore=new Score{Items=order,Trailer=score.Trailer,SourcePath=score.SourcePath};
     var cache=NativeTextCache.Refresh(workingScore,canvas.TextLayout,changed);
     if(cache.Unresolved.Count!=0)throw new NotSupportedException(String.Join(Environment.NewLine,cache.Unresolved));
    }
    order[i]=changed.Bytes.SequenceEqual(original.Bytes)?original:changed;
    trackedNotes&=NativeRetainedDisplayArray.SupportsNote(original)&&NativeRetainedDisplayArray.SupportsNote(changed);
    displayOrdinals.Add(i+1);
   }
   bool trackDisplay=trackedNotes&&displayOrdinals.Count>0&&CanTrackDisplayRedraw();
   ClearMainGroup();
   ApplyMainOrder(order,String.Join(Environment.NewLine,warnings));
   // 14DF:0C10 draws each selected working item, then 316D:0A2C and
   // 2EFD:0309 publish and compact that replacement in ordinal order.
   if(trackDisplay)ReplayTrackedDisplayReplacements(displayOrdinals,1,false);
  }

  static int GroupInteger(float value,int minimum,int maximum){
   if(value<minimum||value>maximum||value!=Math.Truncate(value))
    throw new InvalidDataException("Enter an integer within the supported staff or code range.");
   return (int)value;
  }

  void CheckMainGroupEditing(){
   var previous=score;
   string previousFile=current;
   float previousArrow=editArrowIncrement,previousHorizontal=lastHorizontalMove,previousVertical=lastVerticalMove;
   try{
    var fixture=NativeMeasurements.New(source);
    fixture.Items=new List<Item>{Item.Make(8,1,0,0,1,200),Item.Make(8,2,0,40,1,200),
     Item.Make(1,1,60,7,10,0,1),Item.Make(1,2,80,5,10,0,1)};
    SetDocument(fixture,"");
    Execute("M");Execute("1 55 65 1");Execute("1 10 0 2");
    if(!score.Items[2].Bytes.SequenceEqual(Item.Make(1,1,70,9,10,0,1).Bytes))
     throw new Exception("Main M translation differs from DOS.");
    Execute("C");Execute("1 65 75 1");Execute("2 5");
    if(score.Items.Count!=5||!score.Items[4].Bytes.SequenceEqual(Item.Make(1,2,75,9,10,0,1).Bytes))
     throw new Exception("Main C copy differs from DOS.");
    Execute("A");Execute("2 70 80 1");Execute("4 12 300 5");
    if(!score.Items[3].Bytes.SequenceEqual(Item.Make(1,2,85,12,10,0,1).Bytes)||
       !score.Items[4].Bytes.SequenceEqual(Item.Make(1,2,80,12,10,0,1).Bytes))
     throw new Exception("Main A parameter changes differ from DOS.");
    byte[] beforeDelete=score.Encode();
    Execute("DE");Execute("2 0 0 1");Execute("B");
    if(!score.Encode().SequenceEqual(beforeDelete))throw new Exception("Cancelled group DE changed the score.");
    Execute("DE");Execute("2 0 0 1");Execute("");
    if(score.Items.Count!=3||score.Items.Any(item=>item.Code==1&&item.Staff==2))
     throw new Exception("Main DE selection differs.");
    Undo(false);
    if(!score.Encode().SequenceEqual(beforeDelete))throw new Exception("Group DE undo did not restore the score.");
    var beforeSystem=score.Items.ToArray();
    Execute("CS");Execute("1 2");Execute("4");
    if(score.Items.Count!=2*beforeSystem.Length)throw new Exception("CS did not append a complete system.");
    Execute("MS");Execute("4 2");Execute("7");
    for(int i=0;i<beforeSystem.Length;i++){
     var expected=beforeSystem[i].Clone();expected.Set(2,expected.P(2)+6);
     if(!score.Items[i].Bytes.SequenceEqual(beforeSystem[i].Bytes)||
        !score.Items[beforeSystem.Length+i].Bytes.SequenceEqual(expected.Bytes))
      throw new Exception("CS and MS did not preserve system records.");
    }
    var note=score.Items.First(item=>item.Code==1);
    BeginEdit(note);
    foreach(var test in new[]{new {Command="FLT",Value=11f},new {Command="SHP",Value=12f},
     new {Command="NAT",Value=13f},new {Command="NAC",Value=10f}}){
     Execute(test.Command);
     if(note.P(5)!=test.Value)throw new Exception(test.Command+" differs from DOS.");
    }
    Execute("5 12.23");Execute("FLT");
    if(note.P(5)!=11.23f)throw new Exception("FLT lost the fractional displacement.");
    Execute("5 -12.23");Execute("NAT");
    if(note.P(5)!=-7.229999542236328f)throw new Exception("NAT signed P5 differs from DOS.");
    Execute("4 12 5 10");Execute("20 1");
    if(note.P(4)!=13||note.P(5)!=14||note.Count>=20)
     throw new Exception("Explicit treble enharmonic edit differs from DOS.");
    Execute("4 12 5 10");Execute("21 1");
    if(note.P(4)!=13||note.P(5)!=11)
     throw new Exception("Explicit bass enharmonic edit differs from DOS.");
    Execute("4 12 5 11");Execute("20 1");
    if(note.P(4)!=12||note.P(5)!=11)
     throw new Exception("Unavailable enharmonic spelling changed the note.");
    Execute("4 12 5 10 6 0");Execute("20 1 600 1");
    if(note.P(4)!=13||note.P(5)!=14||note.P(6)!=0)
     throw new Exception("Successful enharmonic edit did not discard the following controls.");
    Execute("4 12 5 10");Execute("600 2 20 1");
    if(note.P(4)!=13||note.P(5)!=14||note.P(6)!=2)
     throw new Exception("Mixed enharmonic edit did not preserve the preceding change.");
    Execute("4 14 5 14");Execute("5 10 20 1");
    if(note.P(4)!=15||note.P(5)!=11||note.P(6)!=2)
     throw new Exception("Mixed enharmonic edit did not use the preceding accidental change.");
    Execute("4 12 5 11");Execute("20 1 600 3");
    if(note.P(4)!=12||note.P(5)!=11||note.P(6)!=5)
     throw new Exception("Unavailable enharmonic spelling prevented a later edit.");
    Execute("4 -12 5 10");Execute("20 1");
    if(note.P(4)!=-11||note.P(5)!=14)
     throw new Exception("Signed enharmonic pitch differs from DOS.");
    Execute("4 12 5 -12.25");Execute("20 -1");
    if(note.P(4)!=11||note.P(5)!=-7.25f)
     throw new Exception("Signed enharmonic accidental differs from DOS.");
    Execute("3 25 4 12 5 10");Execute("4000 3 20 1");
    if(note.P(4)!=26||note.P(5)!=14)
     throw new Exception("Mixed enharmonic edit did not use the preceding equivalence.");
    Execute("4 12 5 16");Execute("20 2");
    if(note.P(4)!=13||note.P(5)!=20)
     throw new Exception("Numeric ENH did not use the DOS sign and accidental default branch.");
    Execute("4 12 5 10");Execute("20 -2");
    if(note.P(4)!=11||note.P(5)!=15)
     throw new Exception("Negative numeric ENH direction differs from DOS.");
    Execute("4 12 5 10");Execute("20 0");
    if(note.P(4)!=13||note.P(5)!=15)
     throw new Exception("Zero numeric ENH direction differs from DOS.");
    Execute("4000 1 20 1");Execute("4000 2 20 1");
    if(note.P(4)!=15||note.P(5)!=11)
     throw new Exception("Mixed ENH did not skip the DOS equivalences from P1 and P2.");
    Execute("S");
    ExitEdit(false);
    var other=score.Items[3];
    float pitch=other.P(4);
    BeginEdit(other);
    Execute("A");
    if(other.P(3)!=note.P(3)||other.P(4)!=pitch)
     throw new Exception("Typed note alignment differs from DOS.");
    ExitEdit(false);
    var rangeFixture=NativeMeasurements.New(source);
    rangeFixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),Item.Make(8,2,0,40,1,200),
     Item.Make(1,2,80,5,10,0,1),Item.Make(1,1,60,7,10,0,1),
     Item.Make(2,2,20),Item.Make(1,1,20,5,10,0,1),Item.Make(1,2,20,7,10,0,1),Item.Make(2,1,20)
    };
    SetDocument(rangeFixture,"");
    Execute("A");Execute("2 0 0 2");Execute("300 5 6000 3");
    if(!score.Items[4].Bytes.SequenceEqual(Item.Make(2,2,25,0,0,25).Bytes))
     throw new Exception("Group A did not copy the newly altered source parameter.");
    Execute("C");Execute("1 10 90 0 2");Execute("4 20 180 2");
    var scaled=new[]{Item.Make(1,5,160,7,10,0,1),Item.Make(1,4,120,9,10,0,1),
     Item.Make(2,5,50,2,0,25),Item.Make(1,4,40,7,10,0,1),
     Item.Make(1,5,40,9,10,0,1),Item.Make(2,4,40)};
    if(score.Items.Count!=14||!score.Items.Skip(8).Zip(scaled,(a,b)=>a.Bytes.SequenceEqual(b.Bytes)).All(same=>same))
     throw new Exception("Scaled group C differs from DOS.");
    Execute("M");Execute("99 0 0 2");Execute("99 5");
    if(score.Items[4].P(3)!=30||score.Items[7].P(3)!=25||score.Items[10].P(3)!=55||score.Items[13].P(3)!=45)
     throw new Exception("All-staff group M differs from DOS.");
    var sameStaff=score.Items.Take(8).ToArray();
    Execute("CS");Execute("1 2");Execute("1");
    if(score.Items.Count!=22||!score.Items.Skip(14).Zip(sameStaff,(a,b)=>a.Bytes.SequenceEqual(b.Bytes)).All(same=>same))
     throw new Exception("Group CS did not append the same-staff copies in DOS order.");
    Execute("MS");Execute("4 2");Execute("1");
    if(score.Items.Count!=22||!score.Items.Skip(8).Take(6).Select(item=>item.Staff).SequenceEqual(new[]{2,1,2,1,2,1}))
     throw new Exception("Group MS did not preserve existing destination records.");
    var overlap=score.Items.ToArray();
    Execute("CS");Execute("1 2");Execute("2");
    if(score.Items.Count!=44||!score.Items.Skip(22).Select(item=>item.Staff).SequenceEqual(overlap.Select(item=>item.Staff+1)))
     throw new Exception("Group CS did not copy the original range once to the overlapping destination.");
    NativeMeasurements.Set(score,0);
    var movedNote=score.Items[2];
    BeginEdit(movedNote);
    Execute("INC 2.5");AcknowledgeEditPause();Execute("INC");AcknowledgeEditPause();
    if(editArrowIncrement!=2.5f)throw new Exception("INC did not retain its configured increment.");
    Execute("RI 1");Execute("UI 1");Execute("RI");
    if(movedNote.P(3)!=133.33333f||movedNote.P(4)!=27.857141f)
     throw new Exception("Inch movement differs from the recorded DOS distances.");
    ExitEdit(false);
    var beamFixture=NativeMeasurements.New(source);
    var firstMatch=Item.Make(1,1,15.513394f,5,10,0,.25f);
    var beam=Item.Make(6,4,13.964936f,5,6.5f,21.45653f,11,0,0,11,17.71074f,21.45653f);
    var slur=Item.Make(5,4,14.327945f,14.5f,14.5f,201.25444f,8,-1,3.5f);
    beamFixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),firstMatch,Item.Make(8,4,0,40,1,200),beam,slur,
     Item.Make(3,4,6.8f,0,1),Item.Make(4,4,15.360858f,-1.5f,999,37.64846f)
    };
    SetDocument(beamFixture,"");
    Execute("M");Execute("99 5 205 6");Execute("99 12.3 79.7");
    var expectedBeam=Item.Make(6,4,15.321184f,5,6.5f,17.84585f,11,0,0,11,16.583519f,17.84585f);
    beam=score.Items[3];
    if(!beam.Bytes.SequenceEqual(expectedBeam.Bytes))
     throw new Exception("Group M scale rounding differs from the DOS beam.");
    BeginEdit(beam);Execute("S");
    if(canvas.Selected!=firstMatch)
     throw new Exception("S did not select the first matching item across staves.");
    Execute("X");Execute("I 5");Execute("A");Execute("X");
    var expectedSlur=Item.Make(5,4,15.321184f,14.5f,14.5f,201.25444f,8,-1,3.5f);
    if(!score.Items[4].Bytes.SequenceEqual(expectedSlur.Bytes))
     throw new Exception("A did not align the slur while preserving its endpoint.");
    BeginEdit(beam);Function(5);
    if(canvas.Selected!=firstMatch)
     throw new Exception("F5 did not select the first matching item across staves.");
    Execute("X");Execute("I 7");Function(6);Execute("X");
    var expectedLine=Item.Make(4,4,15.321184f,-1.5f,999,37.64846f);
    if(!score.Items[6].Bytes.SequenceEqual(expectedLine.Bytes))
     throw new Exception("F6 did not align the line while preserving its endpoint.");
    Execute("A");Execute("99 0 220 3");Execute("300 2");
    if(!score.Items[5].Bytes.SequenceEqual(Item.Make(3,4,8.8f,0,1).Bytes))
     throw new Exception("Group A did not preserve the other clef parameters.");
    var clefFixture=NativeMeasurements.New(source);
    var trebleNote=Item.Make(1,1,25,12,10,0,1);
    var bassNote=Item.Make(1,1,75,12,10,0,1);
    var noClefNote=Item.Make(1,2,25,12,10,0,1);
    clefFixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),Item.Make(3,1,50,0,1),Item.Make(3,1,5,0,0),trebleNote,bassNote,
     Item.Make(8,2,0,40,1,200),Item.Make(3,2,50,0,1),noClefNote
    };
    SetDocument(clefFixture,"");
    BeginEdit(trebleNote);Execute("ENH 1");Execute("X");
    BeginEdit(bassNote);Execute("ENH 1");Execute("X");
    if(trebleNote.P(4)!=13||trebleNote.P(5)!=14||bassNote.P(4)!=13||bassNote.P(5)!=11)
     throw new Exception("Typed ENH did not use the preceding clef in horizontal order.");
    BeginEdit(noClefNote);Execute("ENH 1");Execute("X");
    if(noClefNote.P(4)!=13||noClefNote.P(5)!=14)
     throw new Exception("Typed ENH did not use treble before the first clef.");
    BeginEdit(bassNote);
    Execute("4 12 5 10");Execute("ENH");
    if(bassNote.P(4)!=11||bassNote.P(5)!=15)
     throw new Exception("Bare ENH chose the wrong direction for a bass natural.");
    Execute("4 12 5 12");Execute("ENH");
    if(bassNote.P(4)!=13||bassNote.P(5)!=13)
     throw new Exception("Bare ENH chose the wrong direction for a bass sharp.");
    Execute("X");
    var altoNote=Item.Make(1,1,25,13,10,0,1);
    var tenorNote=Item.Make(1,2,25,11,10,0,1);
    var cClefFixture=NativeMeasurements.New(source);
    cClefFixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),Item.Make(8,2,0,40,1,200),
     Item.Make(3,1,10,0,2),Item.Make(3,2,10,0,3),altoNote,tenorNote
    };
    SetDocument(cClefFixture,"");
    BeginEdit(altoNote);Execute("ENH 1");Execute("X");
    BeginEdit(tenorNote);Execute("ENH 1");Execute("X");
    if(altoNote.P(4)!=14||altoNote.P(5)!=11||tenorNote.P(4)!=12||tenorNote.P(5)!=11)
     throw new Exception("Typed ENH did not use the alto and tenor clef controls.");
    var shiftedNote=Item.Make(1,1,25,13,10,0,1);
    var cueNote=Item.Make(1,2,25,12,10,0,1);
    var octaveNote=Item.Make(1,3,25,12,10,0,1);
    var extendedClefs=NativeMeasurements.New(source);
    extendedClefs.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),Item.Make(8,2,0,40,1,200),Item.Make(8,3,0,80,1,200),
     Item.Make(3,1,10,2,2),Item.Make(3,2,10,100,1),Item.Make(3,3,10,0,1.8f),
     shiftedNote,cueNote,octaveNote
    };
    SetDocument(extendedClefs,"");
    foreach(var item in new[]{shiftedNote,cueNote,octaveNote}){
     BeginEdit(item);Execute("ENH 1");Execute("X");
    }
    if(shiftedNote.P(4)!=14||cueNote.P(4)!=13||octaveNote.P(4)!=13||
       new[]{shiftedNote,cueNote,octaveNote}.Any(item=>item.P(5)!=11))
     throw new Exception("Typed ENH did not use the original shifted and fractional clef controls.");
    var inchFixture=NativeMeasurements.New(source);
    NativeMeasurements.Set(inchFixture,0);
    var movedSlur=Item.Make(5,1,25,12,14,65,2);
    inchFixture.Items=new List<Item>{Item.Make(8,1,0,0,1,200),movedSlur,Item.Make(1,1,25,12,10,0,1)};
    SetDocument(inchFixture,"");
    BeginEdit(movedSlur);
    Execute("RI 1");Execute("UI 1");Execute("DI .5");Execute("LI .5");Execute("X");
    var expectedMovedSlur=Item.Make(5,1,38.333332f,23.42857f,25.42857f,78.333328f,2);
    if(!movedSlur.Bytes.SequenceEqual(expectedMovedSlur.Bytes))
     throw new Exception("Inch movement did not reproduce the DOS slur endpoints.");
    Execute("METRIC");
    BeginEdit(movedSlur);Execute("RI 1");Execute("UI 1");Execute("X");
    var expectedMetricSlur=Item.Make(5,1,65,46.285713f,48.285713f,104.99999f,2);
    if(NativeMeasurements.Get(score)!=1||!movedSlur.Bytes.SequenceEqual(expectedMetricSlur.Bytes))
     throw new Exception("Explicit inch movement differed in a metric score.");
    BeginEdit(movedSlur);
    Execute("RC 1");Execute("UC 1");Execute("LP 1");Execute("DP 1");Execute("X");
    var expectedUnitSlur=Item.Make(5,1,75.128319f,54.967129f,56.967129f,115.128311f,2);
    if(!movedSlur.Bytes.SequenceEqual(expectedUnitSlur.Bytes))
     throw new Exception("Centimeter and point movement did not reproduce the DOS slur endpoints.");
    var reducedFixture=NativeMeasurements.New(source);
    var reducedSlur=Item.Make(5,1,25,12,14,65,2);
    reducedFixture.Items=new List<Item>{Item.Make(8,1,0,0,.6f,200),reducedSlur,Item.Make(1,1,25,12,10,0,1)};
    SetDocument(reducedFixture,"");
    BeginEdit(reducedSlur);Execute("UC 1");Execute("DI .5");Execute("UP 1");Execute("X");
    var expectedReducedSlur=Item.Make(5,1,25,8.479607f,10.479607f,65,2);
    if(!reducedSlur.Bytes.SequenceEqual(expectedReducedSlur.Bytes))
     throw new Exception("Vertical unit movement did not reproduce the DOS reduced-staff endpoints.");
    BeginEdit(reducedSlur);
    HandleOriginalEditMovementKey(new KeyEventArgs(Keys.Shift|Keys.F10));
    Execute("R 1");Execute("U 1");Execute("X");
    var expectedMidpoint=Item.Make(5,1,25,8.479607f,10.479607f,65,3,0,0,.51f);
    if(!reducedSlur.Bytes.SequenceEqual(expectedMidpoint.Bytes))
     throw new Exception("Slur midpoint movement differs from the DOS record.");
    var unitLine=Item.Make(4,1,15,4,6,40,0);
    var unitBeam=Item.Make(6,1,60,12,14,78,11,0,0,11,70,78);
    var unitFixture=NativeMeasurements.New(source);
    unitFixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),unitLine,unitBeam,
     Item.Make(1,1,60,5,10,0,.25f),Item.Make(1,1,70,6,10,0,.25f),Item.Make(1,1,78,7,10,0,.25f)
    };
    SetDocument(unitFixture,"");
    foreach(var item in new[]{unitLine,unitBeam}){
     BeginEdit(item);Execute("RC 1");Execute("UP 1");Execute("X");
    }
    var expectedUnitLine=Item.Make(4,1,25.498688f,4.3174601f,6.3174601f,50.498688f);
    var expectedUnitBeam=Item.Make(6,1,70.498688f,12.31746f,14.31746f,88.498688f,11,0,0,11,80.498688f,88.498688f);
    if(!unitLine.Bytes.SequenceEqual(expectedUnitLine.Bytes)||!unitBeam.Bytes.SequenceEqual(expectedUnitBeam.Bytes))
     throw new Exception("Unit movement did not reproduce the DOS line and beam records.");
    BeginEdit(unitBeam);
    Execute("INC 2.5");AcknowledgeEditPause();
    HandleOriginalEditMovementKey(new KeyEventArgs(Keys.F9));
    MoveSelected(0,editArrowIncrement);
    HandleOriginalEditMovementKey(new KeyEventArgs(Keys.F10));
    MoveSelected(0,-editArrowIncrement);
    HandleOriginalEditMovementKey(new KeyEventArgs(Keys.Home));
    expectedUnitBeam.Set(4,expectedUnitBeam.P(4)+2.5f);
    expectedUnitBeam.Set(5,expectedUnitBeam.P(5)-2.5f);
    if(editEndpoint!=0||!unitBeam.Bytes.SequenceEqual(expectedUnitBeam.Bytes))
     throw new Exception("Beam endpoint lock differs from the DOS movement.");
    HandleOriginalEditMovementKey(new KeyEventArgs(Keys.Control|Keys.I));
    if(editArrowIncrement!=1||!pendingEditAcknowledgement||!AcknowledgeEditPause())
     throw new Exception("Ctrl+I did not reset and acknowledge the arrow increment.");
    Execute("INC 2.5");AcknowledgeEditPause();
    HandleOriginalEditMovementKey(new KeyEventArgs(Keys.Alt|Keys.I));
    if(editArrowIncrement!=2.5f||!pendingEditAcknowledgement||!AcknowledgeEditPause())
     throw new Exception("Alt+I did not preserve and acknowledge the arrow increment.");
    Execute("X");
    var drawingFixture=NativeMeasurements.New(source);
    var drawing=Item.Make(11,1,20,10,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0);
    byte[] drawingName=System.Text.Encoding.ASCII.GetBytes("BDIB0 ");
    Array.Copy(drawingName,0,drawing.Bytes,56,drawingName.Length);
    var packedText=Item.Make(16,1,30,5,1,1,0,0,0,0,0,10,15.5828085f,0,0,0);
    var packedTextBytes=System.Text.Encoding.ASCII.GetBytes("_00B. Tbn.  ");
    Array.Copy(packedTextBytes,0,packedText.Bytes,56,packedTextBytes.Length);
    var packedPostScript=Item.Make(15,1,40,26,80,90,1,32,0,0,0,0,0,0,0,0,0,0,0);
    var packedPostScriptBytes=System.Text.Encoding.ASCII.GetBytes("RECT.PS                 ");
    Array.Copy(packedPostScriptBytes,0,packedPostScript.Bytes,56,packedPostScriptBytes.Length);
    drawingFixture.Items=new List<Item>{Item.Make(8,1,0,0,1,200),drawing,packedText,packedPostScript};
    SetDocument(drawingFixture,"");
    Execute("A");Execute("99 0 220 11");Execute("300 2 400 1");
    var alteredDrawing=score.Items[1];
    if(alteredDrawing.P(3)!=22||alteredDrawing.P(4)!=11||
       !alteredDrawing.Bytes.Skip(56).Take(6).SequenceEqual(drawingName)||
       BitConverter.ToUInt32(alteredDrawing.Bytes,76)!=0x20202020u)
     throw new Exception("Code 11 group alteration did not preserve the name and restore DOS padding.");
    var expectedPackedDrawing=alteredDrawing.Clone();
    expectedPackedDrawing.Set(10,2);
    expectedPackedDrawing.Set(3,23);
    Execute("A");Execute("99 0 220 11");Execute("14 2 10000 14 300 1");
    if(!score.Items[1].Bytes.SequenceEqual(expectedPackedDrawing.Bytes))
     throw new Exception("Group A did not use the temporary payload word before repacking the DOS drawing.");
    Execute("A");Execute("99 0 220 16");Execute("10 4 14 2 10000 14 12 6 13 1");
    if(!score.Items[2].Bytes.SequenceEqual(packedText.Bytes))
     throw new Exception("Group A did not detach the text words during DOS numeric editing.");
    Execute("A");Execute("99 0 220 15");Execute("10 32766.998");
    if(score.Items[3].P(10)!=32766.998046875f)
     throw new Exception("Group A discarded a valid P10 just below the DOS limit.");
    Execute("A");Execute("99 0 220 15");Execute("10 -32767");
    if(score.Items[3].P(10)!=0)
     throw new Exception("Group A did not clear P10 at the DOS limit.");
    var centerFixture=NativeMeasurements.New(source);
    centerFixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),Item.Make(14,1,10,0),Item.Make(14,1,80,0),Item.Make(2,1,45,4,-2,0,1,0,0),
     Item.Make(3,1,5,1,1),Item.Make(7,1,22,13,1,36)
    };
    SetDocument(centerFixture,"");
    Execute("A");Execute("99 0 220 2");Execute("9 -1");
    if(!score.Items[3].Bytes.SequenceEqual(Item.Make(2,1,45,4,-2,0,1,0,58.85f).Bytes))
     throw new Exception("Group A automatic rest centering differs from the DOS record.");
    Execute("A");Execute("99 0 220 7");Execute("6 0 4000 1 4000 2 400 2");
    Execute("A");Execute("99 0 220 3");Execute("6 1.2");
    if(!score.Items[4].Bytes.SequenceEqual(Item.Make(3,1,5,1,1,1.2f).Bytes)||
       !score.Items[5].Bytes.SequenceEqual(Item.Make(7,1,22,15,1).Bytes))
     throw new Exception("Group A record length or low-source equivalence differs from DOS.");
    Execute("A");Execute("99 0 220 3");Execute("5 2 600 1 7000 6 19 1");
    var expanded=Item.Make(3,1,5,1,2,2.2f,2.2f,0,0,0,0,0,0,0,0,0,0,0,1);
    if(!score.Items[4].Bytes.SequenceEqual(expanded.Bytes))
     throw new Exception("Group A did not extend the numeric record with the DOS parameter sequence.");
    Execute("A");Execute("99 0 220 3");Execute("19 0");
    if(!score.Items[4].Bytes.SequenceEqual(Item.Make(3,1,5,1,2,2.2f,2.2f).Bytes))
     throw new Exception("Group A did not remove the cleared trailing numeric words.");
    Console.WriteLine("PASS: bounded Main M, C, A, DE, CS and MS workflows.");
   }finally{
    editArrowIncrement=previousArrow;lastHorizontalMove=previousHorizontal;lastVerticalMove=previousVertical;
    itemAlignmentPosition=null;ClearMainGroup();SetDocument(previous,previousFile);
   }
  }
 }
}
