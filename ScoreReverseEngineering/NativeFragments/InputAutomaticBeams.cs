using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputAutomaticBeams {
  static float F(double n){return (float)n;}
  static bool Present(float n){return BitConverter.ToInt32(BitConverter.GetBytes(n),0)!=0;}
  static void Emit(List<float> pairs,int first,int last){if(first<last){pairs.Add(first);pairs.Add(last);}}
  // Overlay8 section63. Duration words are the separate stage57 projection,
  // not the rhythm input tokens: secondary/invisible items have zero duration.
  public static float[] Groups(IList<float[]> rows,float[] durations,float groupControl,int skip){
   if(rows.Count!=durations.Length)throw new ArgumentException("Duration projection length");float length=F(groupControl/2.0),threshold=F((double)length-0.0010000000474974513);if(length<=0||float.IsNaN(length)||float.IsInfinity(length))throw new ArgumentOutOfRangeException("groupControl","Nonpositive automatic beam intervals are prevented");
   var pairs=new List<float>();int position=-1,lastFlagged=0,notes=0,first=0,flags=0;
   if(skip!=0)do{do{position++;if(position>=rows.Count)return pairs.ToArray();}while(rows[position][0]>2||!Present(durations[position])||durations[position]>=64);if(rows[position][0]==1)lastFlagged++;skip--;}while(skip>0);
   for(int i=0;i<rows.Count;i++)if(rows[i][0]==1&&Present(durations[i])){int beginning=++notes;if(durations[i]>=64){while(i<rows.Count-1){i++;if(Present(durations[i])){if(durations[i]<64)break;if(rows[i][0]==1)notes++;}}Emit(pairs,beginning,notes);if(rows[i][0]==1)notes++;}}
   float elapsed=0;notes=lastFlagged;
   while(true){
    if(position<rows.Count-1){position++;var row=rows[position];float duration=durations[position];if(row[0]<=2&&Present(duration)){
      if(row[0]==2){if(duration>=64)continue;}
      else{notes++;if(duration>=64)continue;flags=(short)Math.Truncate(row[12]);if(flags!=0){lastFlagged=notes;if(first==0)first=notes;}}
      elapsed=F((double)duration+elapsed);if(flags!=0&&elapsed<threshold)continue;
     }else continue;
    }
    if(first!=0){Emit(pairs,first,lastFlagged);first=0;}
    if(position>=rows.Count-1)return pairs.ToArray();
    if(elapsed>=threshold){do{elapsed=F((double)elapsed-length);}while(threshold<elapsed);if(Math.Abs(elapsed)<0.0010000000474974513f)elapsed=0;}
   }
  }
  public static float[] Durations(IList<float[]> rows){var values=new float[rows.Count];for(int i=0;i<rows.Count;i++)values[i]=BitConverter.ToInt32(BitConverter.GetBytes(rows[i][14]),0)<0?0:rows[i][6];return values;}
 }
}
