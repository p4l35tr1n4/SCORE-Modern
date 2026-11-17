using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputChordPitchRecords {
  // Section55 chord-ordering and record path, restricted to treble natural
  // notes separated by at least a third. The accidental-column and adjacent
  // notehead displacement branches are intentionally outside this entry point.
  public static InputPitchResult CompileSpacedNaturals(float[] source,float staff,float left,float right){
   if(source==null||source.Length==0||source[0]!=3000)throw new NotSupportedException("This chord projection requires an initial TR token");
   var tokens=(float[])source.Clone();int positive=0;
   for(int i=0;i<tokens.Length;i++){int word=(int)tokens[i];if(word>=0)positive++;if(i==0)continue;if(Math.Abs(word)/1000!=1||Math.Abs(word)%1000>=100)throw new NotSupportedException("Natural treble notes required");if(word<0)continue;
    int end=i+1;while(end<tokens.Length&&tokens[end]<0)end++;if(end-i<2)continue;
    var group=new List<int>();for(int j=i;j<end;j++)group.Add(Math.Abs((int)tokens[j]));group.Sort();for(int j=1;j<group.Count;j++)if(group[j]%100-group[j-1]%100<2)throw new NotSupportedException("Adjacent/unison chord branch");
    int low=group[0]%100-28,high=group[group.Count-1]%100-28,balance;
    if(group.Count==2||high-7!=7-low)balance=14-high-low;
    else{balance=0;for(int j=1;j<group.Count-1;j++){int pitch=group[j]%100-28;if(pitch<8)balance++;if(pitch>7)balance--;}}
    if(balance<=0)group.Reverse();for(int j=0;j<group.Count;j++)tokens[i+j]=j==0?group[j]:-group[j];
   }
   var absolute=new float[tokens.Length];for(int i=0;i<tokens.Length;i++)absolute[i]=Math.Abs(tokens[i]);var raw=InputPitchRecords.CompileSingleNotes(absolute,staff,0,left,right,true);var result=new InputPitchResult{Left=raw.Left,FinalClef=raw.FinalClef};float step=(float)(((double)right-result.Left)/(positive+1));int position=0,leader=-1;
   for(int i=0;i<tokens.Length;i++){var p=raw.Records[i];if(tokens[i]>=0){position++;if(i>0){leader=i;result.Events++;result.NoteIndices.Add(i+1);result.ForcedStems.Add(0);}}
    else{var root=raw.Records[leader];root[7]=Math.Max(root[7],(float)Math.Abs((double)root[3]-p[3]));p[4]=0;p[7]=0;p[14]=-1;}
    p[2]=(float)((double)position*step+result.Left);result.Records.Add(p);
   }
   return result;
  }
 }
}
