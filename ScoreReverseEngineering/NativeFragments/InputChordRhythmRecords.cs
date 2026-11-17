using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordRhythmRecords {
  // Section57's ordinary chord-member loop: one rhythm event advances the
  // whole chord. Secondary members keep the duration/head, without flags.
  // Reference interpolation follows this projection.
  public static List<float[]> Apply(IList<float[]> source,float[] encoded,float left,float right,float scale,int[] stems){
   return Apply(source,encoded,left,right,scale,stems,false,1);
  }
  public static List<float[]> Apply(IList<float[]> source,float[] encoded,float left,float right,float scale,int[] stems,bool proportional,float exponent,float? referenceStart=null,short? retainedGraceWord=null,float? rememberedLeft=null){
   var reduced=new List<float[]>();var leaders=new List<int>();int current=-1;
   foreach(var p in source){
    if(p[0]==1&&Math.Abs(p[3])>=200)throw new NotSupportedException("Chord pitch controls.");
    if(p[0]==1&&p[14]<0){
     if(p[14]!=-1||current<0)throw new NotSupportedException("Chord member controls.");
    }else{
     current=reduced.Count;
     reduced.Add((float[])p.Clone());
    }
    leaders.Add(current);
   }
   var laid=InputRhythmRecords.Apply(reduced,encoded,left,right,scale,proportional,exponent,stems,referenceStart,rememberedLeft);
   var result=new List<float[]>();
   for(int i=0;i<source.Count;i++){
    var old=source[i];var leader=laid[leaders[i]];
    bool grouped=old[14]<0||(i+1<source.Count&&leaders[i+1]==leaders[i]);
    if(old[0]==1&&old[11]!=0&&old[11]!=1&&old[11]!=2)
     throw new NotSupportedException("Chord staff displacement leaves the F2 S+/S- controls.");
    if(old[0]!=1||old[14]>=0){
     if(old[0]==1&&Math.Abs(old[3])>=100&&leader[6]!=65&&grouped)
      leader[7]=Math.Max(leader[7],2*old[7]);
     result.Add(leader);
     continue;
    }
    var member=(float[])old.Clone();
    member[2]=leader[2];member[5]=leader[5];member[6]=leader[6];
    member[8]=(float)((double)leader[8]-leader[12]);member[12]=0;
    result.Add(member);
   }
   for(int first=0;first<source.Count;first++){
    if(result[first][0]!=1||result[first][6]!=65||source[first][14]<0)continue;
    int last=first;
    while(last+1<source.Count&&source[last+1][0]==1&&source[last+1][14]<0)last++;
    if(last>first)ProjectGraceChord(source,result,first,last,retainedGraceWord);
    first=last;
   }
   InputChordDots.Apply(result);return result;
  }
  static void ProjectGraceChord(IList<float[]> source,IList<float[]> result,int first,int last,short? retainedGraceWord){
   int count=last-first+1,direction=result[first][4]%100>=20?20:10;
   // The automatic-stem branch has already removed the leader's marker.
   bool preserveLeaderParentheses=result[first][4]>=100;
   var pitches=new int[count];
   var accidentals=new float[count];
   for(int n=0;n<count;n++){
    var pitch=source[first+n];
    pitches[n]=(int)pitch[3]%100;
    accidentals[n]=(int)pitch[4]%10+(pitch[4]>=100?100:0);
   }
   Array.Sort(pitches,accidentals);
   for(int n=1;n<count;n++)if(pitches[n]==pitches[n-1])
    throw new NotSupportedException("Repeated pitches in a grace chord can stall the original SCORE.");
   for(int n=0;n<count;n++){
    int index=direction==10?n:count-n-1,pitch=pitches[index];
    var row=result[first+n];
    row[3]=pitch+(pitch<0?-100:100);
    row[4]=n==0?direction+(preserveLeaderParentheses?accidentals[index]:accidentals[index]%100):accidentals[index];
    row[9]=0;
    if(n>0){row[6]=64;row[8]=0;}
   }
   int span=Math.Max(0,pitches[count-1])-pitches[0];
   // 2CBE:0DA0 skips the span assignment at 0F50 when the working header
   // is not 2. Section57:0CA6 then consumes the retained DS:B970 word.
   int spanWord=retainedGraceWord.HasValue?retainedGraceWord.Value:span+61;
   result[first][7]=(float)(spanWord*1.666599988937378-.33329999446868896);
   InputChordLayout.Apply(result,first,last);
  }
 }
}
