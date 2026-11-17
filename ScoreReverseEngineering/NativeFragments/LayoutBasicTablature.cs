using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class LayoutBasicTablature {
  //2C0C:0377..051D staff/leading bar;0539..05C8 head, clef and bar-helper controls.
  //0C5F..0CB9 copies records;0B59..0BE9 clears bar tails and sets rest P6=-1.
  //182E..1895 creates rhythm rests;14AF..153C/1898..1A98 create ordinary rhythm notes.
  static short Word(float value){long word;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word))throw new NotSupportedException("Non-finite TAB rhythm attachment.");return unchecked((short)word);}
  //2C0C:0AB1..0B13 is shared by hairpins and retained ordinary slurs.
  static float[] MainSpan(float[] note,int destination,int strings){var span=new float[13];Array.Copy(note,span,Math.Min(note.Length,8));span[1]=destination;span[3]=span[4]=14.5f-2*(6-strings);float distance=(float)((double)note[5]-note[2]-10);span[6]=(float)((double)(distance>0?(float)((double)distance/100):0)+1);span[7]=-1;span[12]=.5f;return span;}
  sealed class TabResultList:List<float[]>{readonly Action<float[],int> write;int words;internal TabResultList(Action<float[],int> callback){write=callback;}public new void Add(float[] row){if(write!=null)write(row,words);base.Add(row);words+=row.Length+1;}}
  //2C0C:0C5F..0C93 copies twenty16-bit words: header and P1..P9.
  //Longer bodies retain destination RN bytes; the caller must establish them.
  static float[] CopyRetained(float[] note,List<float[]> result,Func<float[],int,float[]> copy){if(copy==null)throw new NotSupportedException("Original TAB trailing destination storage is unavailable.");int words=0;foreach(var row in result)words+=row.Length+1;return copy(note,words);}
  public static List<float[]> Build(float[] staff,IList<float[]> notes,int destination,bool clef,bool rhythmLine,short[] packedTuning,int strings,bool bass,IList<short> frequencies,bool hasCollection){return Build(staff,notes,destination,clef,rhythmLine,packedTuning,strings,bass,frequencies,hasCollection,null,-2);}
  public static List<float[]> Build(float[] staff,IList<float[]> notes,int destination,bool clef,bool rhythmLine,short[] packedTuning,int strings,bool bass,IList<short> frequencies,bool hasCollection,short[] recoveredAccidentals,int firstDelta,Func<float[],int,float[]> retainedCopy=null,Action<float[],int> writeRecord=null){
   if(!hasCollection)return new List<float[]>();
   float size=staff[4]==0?1:staff[4];const float factor=1.399999976158142f;
   int head=strings==6?(bass?16:6):strings==7?7:strings==5&&bass?15:14;
   float tabClef=strings==6||strings==7?66:strings==5&&bass?55:44,barControl=strings==6?13:strings==7?15:strings==5&&bass?0:9;
   var result=new TabResultList(writeRecord){new[]{8f,(float)destination,staff[2],(float)((double)staff[3]/factor-strings+1),(float)((double)size*factor),staff.Length>=6?staff[5]:200f,(float)strings},new[]{14f,(float)destination,staff[2],2f}};
   var convertedNotes=new List<LayoutTablatureChordNote>();var rhythmSlurs=new HashSet<float[]>();
   if(clef)result.Add(new[]{3f,(float)destination,(float)((double)staff[2]+2),0f,tabClef});
   float previousMainSlurX=0,previousMainSlurEnd=0;int sourceIndex=0;int previousRhythmX=0;foreach(var note in notes){int frequency=frequencies[sourceIndex++];
    if(note[0]==6)continue;float[] copied=null;if(retainedCopy!=null){int words=0;foreach(var row in result)words+=row.Length+1;copied=retainedCopy(note,words);}
    if(note[0]==5){
     //2C0C:0910..0B13: P9>=98 retains the copied body and its rhythm index.
     if(note.Length>=9&&note[8]>=98){var retained=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):(float[])note.Clone();retained[1]=destination;result.Add(retained);rhythmSlurs.Add(note);continue;}
     float dx=(float)((double)note[2]-previousMainSlurX),de=(float)((double)note[5]-previousMainSlurEnd);previousMainSlurX=note[2];previousMainSlurEnd=note[5];short control=note.Length==7?(copied!=null&&copied.Length>7?Word(copied[7]):(short)1):Word(note[7]);
     if(control>0||Math.Abs(dx)<=3&&Math.Abs(de)<=3)continue;rhythmSlurs.Add(note);if(control/2+1==0)continue;
     result.Add(MainSpan(note,destination,strings));continue;}
    if(note[0]==4&&note[4]==999){result.Add(MainSpan(note,destination,strings));continue;}
    if(note[0]==4||note[0]==18){var body=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):(float[])note.Clone();body[1]=destination;result.Add(body);continue;}
    if(note[0]==2){var rest=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):(float[])note.Clone();rest[1]=destination;rest[5]=-1;result.Add(rest);continue;}
    if(note[0]==14){var bar=new float[11];var source=note.Length>9?(copied??CopyRetained(note,result,retainedCopy)):note;Array.Copy(source,bar,Math.Min(source.Length,10));bar[1]=destination;bar[10]=barControl;result.Add(bar);continue;}
    float accidental=(float)((double)note[4]%10);if(accidental==0&&frequency<=0){for(int prior=convertedNotes.Count-1;prior>=0;prior--)if((int)convertedNotes[prior].Row[3]%100==(int)note[3]%100){accidental=convertedNotes[prior].Row[4];break;}}if(accidental==0){int natural=2*(((int)note[3]%100-(bass?5:0)+27)%7);if(natural>5)natural--;int chromatic=frequency%12;if(natural==0&&chromatic==11)natural=12;if(natural==11&&chromatic==0)chromatic=12;int delta=chromatic-natural;if(recoveredAccidentals==null){if(delta< -2||delta>2)throw new NotSupportedException("TAB accidental table read rejected by the selected safe policy.");int[] encodings={4,1,3,2,5};accidental=encodings[delta+2];}else{int at=delta-firstDelta;if(at<0||at>=recoveredAccidentals.Length)throw new NotSupportedException("TAB accidental lookup leaves the recovered signed-remainder domain.");accidental=recoveredAccidentals[at];}}
    var plan=LayoutTablaturePitch.Prepare(head,(int)note[3]%100,Word(accidental)%10,packedTuning);
    //The original scans the stored tuning order even when adjacent pitches cross.
    // Original single-note scan retains the last string when all tunings exceed the pitch.
    int chosen=plan.Strings;for(int n=1;n<=plan.Strings;n++)if(plan.Tuning[n]<=plan.Absolute){chosen=n;break;}
    //2C0C:089C..08EC compares source P6 with literal6 and writes P9=-1.
    int count=note[5]==6?9:8;for(int pair=0;pair<3;pair++)if(packedTuning[pair]!=0)count=16+pair;
    var converted=new float[count];Array.Copy(new[]{1f,(float)destination,note[2],note[3],(float)accidental,(float)-head,note[6],frequency<=0?-1f:(float)chosen},converted,8);
    for(int pair=0;pair<3;pair++)if(15+pair<count)converted[15+pair]=packedTuning[pair];if(note[5]==6)converted[8]=-1;result.Add(converted);convertedNotes.Add(new LayoutTablatureChordNote{Row=converted,Plan=plan,SortKey=frequency});
    //0895/0898 writes both coordinate words to shared B960. The rhythm
    //phase189F reads this same word pair; it is not reset between phases.
    previousRhythmX=BitConverter.ToInt32(BitConverter.GetBytes(note[2]),0);
   }
   LayoutTablatureChord.Apply(convertedNotes,strings);
   //189F..18AB compares both words of the carried main-note coordinate.
   if(rhythmLine)foreach(var note in notes){
    if(note[0]==14||note[0]==4||note[0]==18)continue;
    //2C0C:0943..0B13 suppresses nearby repeated slur spans;15BC..16BE creates the rhythm copy.
    if(note[0]==5){if(!rhythmSlurs.Contains(note))continue;var slur=new float[13];Array.Copy(note,slur,7);slur[1]=destination;slur[2]=(float)((double)note[2]-factor);slur[3]=slur[4]=1;slur[6]=-Math.Abs(note[6]);slur[7]=-2;slur[12]=.699999988079071f;result.Add(slur);continue;}
    if(note[0]==6){var beam=new float[18];Array.Copy(note,beam,Math.Min(note.Length,18));beam[1]=destination;beam[3]=beam[4]=2.0999999046325684f;if(beam[6]<20)beam[6]=(float)((double)beam[6]+10);beam[17]=.800000011920929f;result.Add(beam);continue;}
    //2C0C:182E..1895 retains P1..P7 and clears P8..P14 before rhythm size.
    if(note[0]==2){var rest=new float[15];Array.Copy(note,rest,Math.Min(note.Length,7));rest[1]=destination;rest[3]=-7;rest[14]=0.699999988079071f;result.Add(rest);continue;}
    int rhythmX=BitConverter.ToInt32(BitConverter.GetBytes(note[2]),0);if(rhythmX==previousRhythmX)continue;previousRhythmX=rhythmX;
    var rhythm=new[]{1f,(float)destination,note[2],0.8999999761581421f,20f,7f,-2f,-1.5f,0f,0f,0f,0f,0f,0f,0.800000011920929f,0f,-0.699999988079071f};if(note.Length>10)rhythm[10]=unchecked((short)-Word(Math.Abs(note[10])));if(note.Length>8)rhythm[8]=note[8];short sourceHead=Word(note[5]);if(sourceHead==1||sourceHead==2){rhythm[5]=9;Array.Resize(ref rhythm,15);}if(Math.Abs(note[3])>=100){rhythm[3]=-2.200000047683716f;rhythm[4]=10;rhythm[7]=100;rhythm[9]=-2.5f;rhythm[14]=.550000011920929f;Array.Resize(ref rhythm,15);}result.Add(rhythm);
   }
   return result;
  }
 }
}
