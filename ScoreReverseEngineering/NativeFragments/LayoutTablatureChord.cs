using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutTablatureChordNote {public float[] Row;public LayoutTablaturePitchPlan Plan;public int SortKey;}
 public static class LayoutTablatureChord {
  static int Bits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0);}
  static void Swap(float[] a,float[] b,int parameter){float value=a[parameter];a[parameter]=b[parameter];b[parameter]=value;}
  public static void Apply(List<LayoutTablatureChordNote> notes,int staffStrings){
   for(int first=0;first<notes.Count;){int last=first;while(last+1<notes.Count&&notes[last].Row[7]>=0&&Bits(notes[last+1].Row[2])==Bits(notes[first].Row[2]))last++;
    if(last>first){
     //2C0C:106C..1074 tests the stored marker words. Positive zero is
     //the empty marker, so this group never reaches the adjustment loop.
     if(Bits(notes[first].Row[2])==0){first=last+1;continue;}
     //Negative zero has a nonzero marker word and follows ordinary adjustment.
     //2C0C:10A8..11DF sorts the absolute-pitch word table and swaps only
     //P4/P5/P8 of the existing output slots, retaining other parameters.
     bool again;do{again=false;for(int n=first+1;n<=last;n++)if(notes[n-1].SortKey>notes[n].SortKey){Swap(notes[n-1].Row,notes[n].Row,3);Swap(notes[n-1].Row,notes[n].Row,4);Swap(notes[n-1].Row,notes[n].Row,7);int sort=notes[n-1].SortKey;notes[n-1].SortKey=notes[n].SortKey;notes[n].SortKey=sort;var plan=notes[n-1].Plan;notes[n-1].Plan=notes[n].Plan;notes[n].Plan=plan;again=true;}}while(again);
     //11DF..1313: make adjacent string positions distinct and contiguous.
     for(int n=last;n>first;n--){int lower=(int)notes[n-1].Row[7],upper=(int)notes[n].Row[7];if(upper>=lower){lower++;if(lower>staffStrings||lower>notes[n-1].Plan.Strings)throw new NotSupportedException("TAB chord string-capacity handler remains guarded.");notes[n-1].Row[7]=lower;}
      while(lower-(int)notes[n].Row[7]>1){for(int j=last;j>=n;j--){int next=(int)notes[j].Row[7]+1;if(next>notes[j].Plan.Strings)throw new NotSupportedException("TAB chord reaches an unverified tuning index.");notes[j].Row[7]=next;}}
     }
     //1314..1491: query the actual TAB helper for the chosen string/fret,
     //then move the recovered ranges while the positive-fret span exceeds5.
     var seen=new HashSet<string>();
     while(true){string state="";for(int n=first;n<=last;n++)state+=((int)notes[n].Row[7]).ToString()+",";if(!seen.Add(state))throw new NotSupportedException("Repeated original TAB chord-adjustment state is prevented.");
      int maximum=0,minimum=99,maximumString=0,maximumIndex=first;
      for(int n=first;n<=last;n++){int requested=(int)notes[n].Row[7],chosen=requested<0?requested:notes[n].Plan.SelectedString(requested);int fret=requested<0?0:unchecked((short)(notes[n].Plan.Absolute-notes[n].Plan.Tuning[chosen]));maximum=Math.Max(maximum,fret);if(fret>0)minimum=Math.Min(minimum,fret);if(chosen>=maximumString){maximumString=chosen;maximumIndex=n;}}
      if(maximum-minimum<=5)break;
      if(notes[last].Row[7]==1){for(int n=first;n<last;n++){if(notes[n].Row[7]==staffStrings)throw new NotSupportedException("Original TAB chord-capacity failure remains guarded.");notes[n].Row[7]++;}}
      else{for(int n=maximumIndex;n<=last;n++){if(notes[n].Row[7]<=0)throw new NotSupportedException("Original TAB chord adjustment reaches its zero-string failure handler.");notes[n].Row[7]--;}}
     }
    }
    first=last+1;
   }
  }
 }
}
