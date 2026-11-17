using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputPitchResult {public readonly List<float[]> Records=new List<float[]>();public readonly List<int> NoteIndices=new List<int>();public readonly List<int> ForcedStems=new List<int>();public float Left;public int Events,FinalClef;}
 public static class InputPitchRecords {
  static bool IsSecondary(float value){int kind=(int)(Math.Abs(value)/1000);return value<0&&(kind==1||kind==7);}
  static int Pitch(int token,int clef){int pitch=Math.Abs(token)%1000,octave=(pitch%100-1)/7,degree=(pitch%100-1)%7+1,offset=clef>1?clef*2+2:clef*12;return (octave-4)*7+offset+degree;}
  static void SortChord(float[] tokens,int start,int clef,int stem){int end=start+1;while(end<tokens.Length&&IsSecondary(tokens[end]))end++;int n=end-start,low=99,high=-99;int[] pitch=new int[n];var group=new float[n];for(int i=0;i<n;i++){group[i]=Math.Abs(tokens[start+i]);pitch[i]=Pitch((int)group[i],clef);low=Math.Min(low,pitch[i]);high=Math.Max(high,pitch[i]);}int balance;if(stem!=0)balance=11-stem;else if(n==2||high-7!=7-low)balance=14-high-low;else{balance=0;for(int i=1;i<n-1;i++){if(pitch[i]<8)balance++;if(pitch[i]>7)balance--;}}int sentinel=balance>0?99:-99;for(int i=0;i<n;i++){int best=sentinel,chosen=-1;for(int j=0;j<n;j++)if(sentinel>0?pitch[j]<best:pitch[j]>best){best=pitch[j];chosen=j;}if(chosen<0)throw new NotSupportedException("Chord sort sentinel reached");tokens[start+i]=i==0?group[chosen]:-group[chosen];pitch[chosen]=sentinel;}}
  // Section55 pitch record projection. CompileSingleNotes retains its original
  // restricted contract; Compile also performs recovered chord sorting/layout.
  public static InputPitchResult CompileSingleNotes(float[] tokens,float staff,int clef,float left,float right,bool insetLeft){return Core(tokens,staff,clef,left,right,insetLeft,false);}
  public static InputPitchResult Compile(float[] tokens,float staff,int clef,float left,float right,bool insetLeft){return Core(tokens,staff,clef,left,right,insetLeft,true);}
  static InputPitchResult Core(float[] input,float staff,int clef,float left,float right,bool insetLeft,bool chords){
   float[] tokens=input==null?null:(float[])input.Clone();
   if(tokens==null||tokens.Length==0)throw new ArgumentException("Pitch tokens required.");
   var output=new InputPitchResult();int initialClef=clef,stem=0,savedStem=0,cue=0;float head=0;
   if(insetLeft&&(int)(tokens[0]/1000)!=4&&BitConverter.ToInt32(BitConverter.GetBytes(left),0)==0)left=1.5f;
   output.Left=left;int divisions=tokens.Length+1;if(chords)foreach(float v in tokens)if(v<0&&((int)(-v/1000)==1||(int)(-v/1000)==7))divisions--;float step=(float)(((double)right-left)/divisions);int position=0,leader=-1;
   for(int tokenIndex=0;tokenIndex<tokens.Length;tokenIndex++){float value=tokens[tokenIndex];if(float.IsNaN(value)||float.IsInfinity(value)||value<short.MinValue||value>=32768)throw new ArgumentOutOfRangeException("tokens");int token=(int)Math.Truncate(value),absolute=Math.Abs(token),kind=absolute/1000;if(!chords&&(kind==1||kind==7)&&token<0)throw new NotSupportedException("Signed chord tokens require the chord compiler.");if(kind==7)kind=1;
    if(chords&&kind==1&&token>0&&tokenIndex+1<tokens.Length&&IsSecondary(tokens[tokenIndex+1])){SortChord(tokens,tokenIndex,clef,stem);value=tokens[tokenIndex];token=(int)value;absolute=Math.Abs(token);}
    bool secondary=kind==1&&token<0;
    float[] p=new float[15];p[1]=staff;
    if(kind==5){stem=savedStem=token-5000;continue;}
    if(kind==6){head=token-6000;continue;}
    if(kind==8){cue=token-8000;if(cue==0)stem=savedStem;else if(stem==0)stem=10;continue;}
    if(kind==3){clef=absolute-3000;if(token<0)continue;p[4]=clef;if(clef==4)clef=0;if(output.Records.Count!=0&&(initialClef<0||output.Records[output.Records.Count-1][0]!=14))p[3]=100;initialClef=-1;}
    else if(kind==4){p[3]=token%100;p[4]=(token-4000)/100;kind=14;if(p[4]==9){p[3]=-p[3];p[4]=0;}}
    else if(kind==17){p[4]=absolute-17000;if(p[4]==0)continue;if(token<0)p[4]=-p[4];p[5]=clef;}
    else if(kind==1){int pitch=absolute%1000;p[3]=Pitch(absolute,clef);p[11]=head;
     if(secondary){if(leader<0)throw new ArgumentException("Chord member without leader");p[14]=-1;p[4]=pitch/100+(absolute>7000?100:0);var first=output.Records[leader];int count=output.Records.Count-leader,rootPitch=(int)first[3]%100,memberPitch=(int)p[3];if(memberPitch<rootPitch)count=-count;if(memberPitch==rootPitch)count=first[4]%100>=20?-1:1;first[6]=count;first[7]=Math.Max(first[7],(float)Math.Abs(rootPitch-memberPitch));}
     else{float direction=stem==0?(p[3]>=7?20:10):stem;p[4]=pitch/100+direction+(absolute>7000?100:0);if(direction>=20){if(p[3]>14)p[7]=p[3]-14;}else if(p[3]<0)p[7]=-p[3];output.Events++;leader=output.Records.Count;}}
    else if(kind==2){int rest=absolute-2000;output.Events++;if(token<0){p[7]=rest;p[4]=-2;p[14]=-1;}else switch(rest){case 1:p[5]=-1;break;case 2:p[4]=-2;p[14]=-1;break;case 3:p[4]=-4;p[14]=-1;break;case 4:p[3]=-6;break;case 5:p[3]=6;break;case 6:p[7]=-1;break;}}
    else{kind=18;p[4]=(token-20000)/100;p[5]=token%100;}
    if(kind<3&&cue!=0)p[3]=(float)((double)p[3]+(p[3]<0?-100:100));
    if(!secondary)position++;p[2]=(float)((double)position*step+left);p[0]=kind;output.Records.Add(p);if(kind==1&&!secondary){output.NoteIndices.Add(output.Records.Count);output.ForcedStems.Add(stem);}
   }
   if(chords)foreach(int index in output.NoteIndices){var first=output.Records[index-1];float span=first[6];if(span==0)continue;int direction=(int)first[4]%100;if(span<0&&direction<20)first[4]=(float)((double)first[4]+10);else if(span>0&&direction>=20)first[4]=(float)((double)first[4]-10);first[6]=0;InputChordLayout.Apply(output.Records,index-1,(int)Math.Truncate(index-1+Math.Abs(span)));}
   output.FinalClef=clef;return output;
  }
 }
}
