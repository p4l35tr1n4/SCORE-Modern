using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public enum InputStaffStage { Pitches, Rhythms, Marks, Beams, Slurs, Complete }
 // Transaction boundary for the recovered numerical stages. Text expansion,
 // setup/preset merging and screen input remain the caller's responsibility.
 // A failed stage never changes the accepted working records or stage.
 public sealed class InputStaffTransaction {
  readonly float staff,beginning,end,scale,followingScale,beam,dynamics;
  readonly int clef;readonly short ties;readonly bool proportional;readonly float exponent;
  List<float[]> rows=new List<float[]>();int[] map,stems;
  float left;float? rememberedSpacingLeft;InputPitchResult pitches;
  InputReferenceSpacing referenceSpacing;InputEndpointStaffContext staffContext;
  public string ReferenceSpacingWarning {get;private set;}
  public InputStaffStage Stage {get;private set;}
  public float OriginalStaff {get{return staff;}}
  public float OriginalSelectionLeft {get{return Stage==InputStaffStage.Pitches?beginning:left;}}
  public float OriginalSelectionRight {get{return end;}}
  public int InitialClef {get{return clef;}}
  public int RhythmCount {get{return pitches==null?0:pitches.Events;}}
  public InputStaffTransaction(float staff,int clef,float beginning,float end,float scale,float followingScale,float beam,float dynamics,short ties):this(staff,clef,beginning,end,scale,followingScale,beam,dynamics,ties,false,1){}
  public InputStaffTransaction(float staff,int clef,float beginning,float end,float scale,float followingScale,float beam,float dynamics,short ties,bool proportional,float exponent){this.staff=staff;this.clef=clef;this.beginning=beginning;this.end=end;this.scale=scale;this.followingScale=followingScale;this.beam=beam;this.dynamics=dynamics;this.ties=ties;this.proportional=proportional;this.exponent=exponent;Stage=InputStaffStage.Pitches;}
  public InputStaffTransaction Restart(float? correctedBeginning=null,float? correctedEnd=null,bool? correctedProportional=null){
   return new InputStaffTransaction(staff,clef,correctedBeginning??beginning,correctedEnd??end,scale,followingScale,beam,dynamics,ties,correctedProportional??proportional,exponent){referenceSpacing=referenceSpacing,staffContext=staffContext};
  }
  public InputStaffTransaction RestartOnStaff(int number,int initialClef,float factor,float nextFactor){
   return new InputStaffTransaction(number,initialClef,0,200,factor,nextFactor,beam,dynamics,ties,proportional,exponent){staffContext=staffContext==null?null:staffContext.ForStaff(number)};
  }
  public InputStaffTransaction RestartOnStaffRange(int number,int initialClef,float factor,float nextFactor,float correctedBeginning,float correctedEnd,bool? correctedProportional=null){return new InputStaffTransaction(number,initialClef,correctedBeginning,correctedEnd,factor,nextFactor,beam,dynamics,ties,correctedProportional??proportional,exponent){staffContext=staffContext==null?null:staffContext.ForStaff(number)};}
  public void UseStaffCoordinates(InputEndpointStaffContext context){Require(InputStaffStage.Pitches);staffContext=context;}
  public void ObservePitchPreviewLoader(){
   Require(InputStaffStage.Rhythms);
   if(rows.Count==0)return;
   byte[] item=InputRecordCompaction.Compact(rows[rows.Count-1]);
   int count=(int)BitConverter.ToSingle(item,0);
   rememberedSpacingLeft=count>=7?BitConverter.ToSingle(item,28):0;
  }
  public void UseReferenceSpacing(InputReferenceSpacing reference){
   Require(InputStaffStage.Pitches);
   referenceSpacing=reference;
  }
  static List<float[]> Copy(IList<float[]> source){var result=new List<float[]>();foreach(var p in source)result.Add((float[])p.Clone());return result;}
  void Require(InputStaffStage expected){if(Stage!=expected)throw new InvalidOperationException("Expected "+expected+", current stage "+Stage);}
  public List<float[]> WorkingRecords {get{return Copy(rows);}}
  public void UpdateInputMeter(ref int top,ref int bottom){
   for(int index=rows.Count-1;index>=0;index--){
    var row=rows[index];
    if(row[0]!=18)continue;
    top=(int)row[4];
    bottom=(int)row[5];
    return;
   }
  }

  public float[] MouseNotePositions(){
   var positions=new float[map.Length-1];
   for(int i=0;i<positions.Length;i++)positions[i]=rows[map[i+1]-1][2];
   return positions;
  }
  public void Pitches(float[] tokens,bool insetLeft){Pitches(tokens,insetLeft,false);}
  public void Pitches(float[] tokens,bool insetLeft,bool allowLargeInput){Pitches(tokens,insetLeft,allowLargeInput,false);}
  public void Pitches(float[] tokens,bool insetLeft,bool allowLargeInput,bool useReferenceClef){
   Require(InputStaffStage.Pitches);
   // F2 uses the reference staff's opening clef as its pitch-preview left
   // bound. The rhythm pass has a separate first timed anchor.
   float? previewLeft=useReferenceClef&&referenceSpacing!=null?referenceSpacing.OpeningClefX:null;
   var candidate=InputPitchRecords.Compile(tokens,staff,clef,previewLeft??beginning,end,insetLeft&&!previewLeft.HasValue);
   if(!allowLargeInput&&candidate.NoteIndices.Count>150)
    throw new NotSupportedException("SCORE can damage working records above 150 pitched notes. Split the entry or enable larger F2 input buffers.");
   var candidateMap=new int[candidate.NoteIndices.Count+1];var candidateStems=new int[candidateMap.Length];for(int i=1;i<candidateMap.Length;i++){candidateMap[i]=candidate.NoteIndices[i-1];candidateStems[i]=candidate.ForcedStems[i-1];}pitches=candidate;map=candidateMap;stems=candidateStems;rows=Copy(candidate.Records);left=candidate.Left;Stage=InputStaffStage.Rhythms;}
  public void Rhythms(float[] tokens){Rhythms(tokens,false);}
  public void Rhythms(float[] tokens,bool reproduceNonfiniteChordDuration){
   Require(InputStaffStage.Rhythms);
   float? referenceStart=referenceSpacing==null?(float?)null:referenceSpacing.Start;
   float rhythmEnd=referenceSpacing==null?end:referenceSpacing.QueryEnd;
   bool chord=false;
   foreach(var row in rows)if(row[0]==1&&row[14]<0)chord=true;
   var forcedStems=pitches.ForcedStems.ToArray();
   var candidate=chord
    ?InputChordRhythmRecords.Apply(rows,tokens,left,rhythmEnd,scale,forcedStems,proportional,exponent,referenceStart,null,rememberedSpacingLeft)
    :InputRhythmRecords.Apply(rows,tokens,left,rhythmEnd,scale,proportional,exponent,forcedStems,referenceStart,rememberedSpacingLeft);
   string warning=referenceSpacing==null?null:referenceSpacing.Apply(candidate,scale);
   if(chord)for(int i=0;i+1<candidate.Count;i++){
    var row=candidate[i];
    var next=candidate[i+1];
    if(row[0]!=1||row[14]<0||row[6]!=0||next[0]!=1||next[14]!=-1)continue;
    if(!reproduceNonfiniteChordDuration)
     throw new NotSupportedException("A zero-duration chord writes an invalid duration in SCORE. Choose a positive duration or enable original invalid states.");
    // SCORE's x87 indefinite word is retained only in the chord leader.
    row[6]=float.NaN;
   }
   ReferenceSpacingWarning=warning;
   rows=candidate;
   Stage=InputStaffStage.Marks;
  }
  public void Marks(float[] tokens,int[] opposite){Marks(tokens,opposite,false);}
  // Original marks use literal positions 0 and 200. Slur fractions use
  // the input limits, while outgoing ties use the literal end position 200.
  public void Marks(float[] tokens,int[] opposite,bool reproduceRetainedDescriptor){Require(InputStaffStage.Marks);var candidate=InputMarksCompiler.Apply(rows,map,tokens,opposite,staff,0,200,followingScale,dynamics,reproduceRetainedDescriptor);rows=candidate.Records;Stage=InputStaffStage.Beams;}
  public void SkipRhythms(){
   Require(InputStaffStage.Rhythms);
   Stage=InputStaffStage.Marks;
  }
  public void Beams(float[] endpointPairs){Require(InputStaffStage.Beams);if(endpointPairs==null||endpointPairs.Length%2!=0)throw new ArgumentException("Paired beam endpoints required");var candidate=Copy(rows);var candidateStems=(int[])stems.Clone();for(int i=0;i<endpointPairs.Length;i+=2){var result=InputUniformBeam.Build(candidate,map,candidateStems,endpointPairs[i],endpointPairs[i+1],left,end,beam,staffContext);candidate=result.Records;candidateStems=result.ForcedStems;}rows=candidate;stems=candidateStems;Stage=InputStaffStage.Slurs;}
  public void Slurs(float[] endpointPairs){Require(InputStaffStage.Slurs);if(endpointPairs==null||endpointPairs.Length%2!=0)throw new ArgumentException("Paired slur endpoints required");var candidate=Copy(rows);for(int i=0;i<endpointPairs.Length;i+=2){var endpoint=InputEndpointProjection.Decode(endpointPairs[i],endpointPairs[i+1],5,candidate,map,stems,left,end,staffContext);candidate=InputSlurConstruction.Build(candidate,endpoint,staff,map.Length,ties,200,staffContext);}rows=candidate;Stage=InputStaffStage.Complete;}
  public void Beams(InputEndpointTokens tokens){if(tokens==null)throw new ArgumentNullException("tokens");Require(InputStaffStage.Beams);Beams(tokens.Automatic?InputAutomaticBeams.Groups(rows,InputAutomaticBeams.Durations(rows),tokens.GroupControl,tokens.Skip):tokens.Pairs);}
  public List<float[]> Preview(IList<float[]> prefix){var result=Copy(prefix);int first=result.Count+1;foreach(var row in rows){byte[] bytes=InputRecordCompaction.Compact(row);int count=(int)BitConverter.ToSingle(bytes,0);var item=new float[count];Buffer.BlockCopy(bytes,4,item,0,count*4);result.Add(item);}// Original preview item loader 1249:0121..0128 clears absent
   // parameter words. Its final loaded record supplies the retained P7.
   if(Stage==InputStaffStage.Rhythms&&result.Count>0){var last=result[result.Count-1];rememberedSpacingLeft=last.Length>=7?last[6]:0;}
   return InputBeamStemCompletion.Apply(result,first,staffContext);}
  public List<float[]> Commit(IList<float[]> prefix){Require(InputStaffStage.Complete);return Preview(prefix);}
 }
}
