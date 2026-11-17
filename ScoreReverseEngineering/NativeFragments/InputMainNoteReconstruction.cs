using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputMainNoteReconstruction {
  static float P(float[] p,int n){return p.Length>=n?p[n-1]:0;}
  static int W(float value){if(float.IsNaN(value)||float.IsInfinity(value)||value<short.MinValue||value>=32768)throw new NotSupportedException("Main flip note conversion outside the original signed-word domain.");return (short)Math.Truncate(value);}
  // Main overlay's note transcription (36:075F-0AB4), then recovered sections55/57.
  // A group is the contiguous Code1 records at one original horizontal position.
  public static List<float[]> Build(IList<float[]> source,int stem,short? retainedGraceWord=null,bool originalInvalidStates=false){
   if(source==null||source.Count==0)throw new ArgumentException("Note group");
   var tokens=new List<float>();if(stem!=0)tokens.Add(5000+stem);
   bool cue=false;for(int i=0;i<source.Count;i++){
    var note=source[i];int pitch=W(P(note,4)),control=W(P(note,5));
    if(P(note,7)<64){bool next=Math.Abs(pitch)>=100;if(next!=cue){tokens.Add(next?8001:8000);cue=next;}}
    // Main36:0782-078D tests strictly greater than 100 (JLE), unlike
    // the generic pitch compiler's >=100 packed-accidental convention.
    int token=1028+pitch%100+(control%10)*100+(control>100?6000:0);
    tokens.Add(i==0?token:-token);
   }
   var pitched=InputPitchRecords.Compile(tokens.ToArray(),P(source[0],2),0,0,200,false);
   if(pitched.Records.Count!=source.Count)throw new NotSupportedException("Main transcription changed the note-group count.");
   if(pitched.Records.Exists(row=>row[0]!=1)){
    if(originalInvalidStates&&pitched.Events==0)return pitched.Records;
    throw new NotSupportedException("Main transcription would turn this outlying pitch into a non-note working record. Original invalid-state compatibility permits the measured compact-record effect.");
   }
   float duration=P(source[0],7);if(duration==65)duration=64;
   if(duration<=0)throw new NotSupportedException("The original refuses notes without rhythmic time.");
   float encoded=(float)((W(P(source[0],9))%100/10)*1000.0+duration);
   var rows=InputChordRhythmRecords.Apply(pitched.Records,new[]{encoded},0,200,1,pitched.ForcedStems.ToArray(),false,1,null,retainedGraceWord);
   return rows;
  }
 }
}
