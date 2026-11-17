using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionClefState {
  public short RetainedStaff,RetainedTarget;
  public TranspositionClefState(short staff,short target){RetainedStaff=staff;RetainedTarget=target;}
 }
 // 2DC2:1665..1AC4. Caller supplies retained state explicitly: the
 // original final clef scan uses it even if no entry changes clef.
 public static class InputTranspositionClefs {
  static readonly short[,] shifts={{0,12,6,8},{-12,0,-6,-4},{-6,6,0,2},{-8,4,-2,0}};
  static short Word(float value){long v;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out v))throw new NotSupportedException("Non-finite clef conversion parameter.");return unchecked((short)v);}
  static bool Negative(float value){return (BitConverter.ToUInt32(BitConverter.GetBytes(value),0)&0x80000000)!=0;}
  public static void Apply(IList<float[]> rows,IList<TranspositionRecord> entries,short[] pitches,float staff,short target,TranspositionClefState state){
   if(state==null||entries.Count!=pitches.Length||target<0||target>3)throw new NotSupportedException("Clef conversion caller state outside recovered domain.");
   int cursor=0;
   for(int i=0;i<entries.Count;){
    short old=entries[i].Clef;if(old==target){i++;continue;}
    if(old<0||old>3)throw new NotSupportedException("Original clef shift table index outside measured four-clef domain.");
    short delta=shifts[old,target];int last=i;
    while(last<entries.Count&&entries[last].Clef==old){
     var entry=entries[last];var row=rows[entry.Item-1];
     if(entry.Kind>=0&&entry.Kind!=2){
      pitches[last]=unchecked((short)(pitches[last]+delta));
      if(row.Length>=4){short pitch=pitches[last],stored=Word(row[3]);if(Math.Abs((int)stored)>=100)pitch=unchecked((short)(pitch+(stored<0?-100:100)));row[3]=pitch;}
     }
     entry.Clef=target;last++;
    }
    int final=last-1;state.RetainedStaff=Word(staff);state.RetainedTarget=target;
    if(cursor>=rows.Count)throw new NotSupportedException("Clef conversion cursor leaves item list.");
    float lower=(float)((double)rows[cursor][2]-4f),upper=(float)((double)rows[entries[final].Item-1][2]+4f);
    for(int n=0;n<rows.Count;n++){
     var row=rows[n];if(row.Length<3||Word(row[1])!=state.RetainedStaff)continue;short code=Word(row[0]);
     if((code==5||code==6)&&row.Length>=5&&row[2]>=lower&&row[2]<=upper){
      float first=row[3];
      if(code==6&&Math.Abs(first)>=100f){float remainder=(float)((double)first%100f+delta);first=Negative(remainder)?-100f:100f;first=(float)((double)first+remainder);}
      else first=(float)((double)first+delta);
      row[3]=first;row[4]=(float)((double)row[4]+delta);
     }
     if(code==17){if(row.Length<6){Array.Resize(ref row,6);rows[n]=row;}row[5]=target;}
    }
    cursor=entries[final].Item;i=last;
   }
   bool seen=false;
   for(int n=0;n<rows.Count;n++){
    var row=rows[n];if(row.Length<2||Word(row[1])!=state.RetainedStaff||Word(row[0])!=3)continue;
    if(seen){row[0]=13;continue;}seen=true;
    if(row.Length<5){if(state.RetainedTarget==0)continue;Array.Resize(ref row,5);rows[n]=row;}
    row[4]=state.RetainedTarget;
   }
  }
 }
}
