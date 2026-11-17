using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Parent TR Code 12 pass, 2F87:03DF..0517. Tables are the measured
 // signed words at DS:41CA and DS:41B2, including their index-zero slots.
 public static class InputTranspositionEmbeddedControl {
  public const string FingeringWarning="******** Fingerings for guitar chords will NOT transpose when P5 = -1 or -2.";
  static readonly short[] pitches={70,1,3,5,6,8,10,12};
  static readonly short[] encodings={6,10,21,20,31,30,40,42,50,61,60,71,70};
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long v;if(!FiniteBinary32Integer64.TryConvert(Bits(f),out v))throw new NotSupportedException("Non-finite embedded control.");return unchecked((short)v);}
  public static void Apply(IList<float[]> rows,short staff,short semitoneClass,out bool warningRequired){
   warningRequired=false;if(semitoneClass%12==0)return;
   if(semitoneClass< -11||semitoneClass>11)throw new NotSupportedException("Parent semitone class is not reduced modulo twelve.");
   foreach(var row in rows){
    if(row.Length<7||Word(row[0])!=12||Word(row[1])!=staff||(Bits(row[4])&0x80000000)==0||Bits(row[6])==0)continue;
    short packed=Word(row[6]);int group=packed/10;if(group<0||group>=pitches.Length)throw new NotSupportedException("Embedded pitch leaves its recovered table.");
    short accidental=unchecked((short)(packed%10));if(accidental==2)accidental=-1;
    short next=unchecked((short)(pitches[group]-accidental+semitoneClass));next=unchecked((short)(next%12));if(next<=0)next=unchecked((short)(next+12));
    row[6]=encodings[next];if(row[4]>=-2f)warningRequired=true;
   }
  }
 }
}
