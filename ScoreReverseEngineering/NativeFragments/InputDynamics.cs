using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputDynamics {
  // Section60 default-side Code9 dynamics path and 0DE2 position correction.
  // The mark-list parser and opposite-side descriptor control are separate.
  public static float[] Create(int symbol,float[] note,float staff,float preference){
   if(symbol<50||symbol>58)throw new NotSupportedException("Outside the ordinary dynamic-symbol branch");if(note==null||note.Length!=15||note[0]!=1)throw new ArgumentException("Working note required");
   int pitch=((short)Math.Truncate(note[3]))%100;float correction=0;
   if(note[4]%100<20&&pitch<=2)correction=pitch-3;
   var p=new float[15];p[0]=9;p[1]=staff;p[2]=note[2];p[3]=(float)((double)correction-2);p[4]=symbol;p[5]=1;p[14]=preference;return p;
  }
 }
}
