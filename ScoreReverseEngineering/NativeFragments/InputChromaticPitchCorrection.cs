using System;using System.Linq;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Modern opt-in correction. The recovered transposition path remains unchanged.
 // Preserve sounding pitch when the original's accidental-limit respelling crosses
 // a diatonic neighbour in the wrong direction. Rows retain index correspondence.
 public static class InputChromaticPitchCorrection {
  static readonly int[] natural={0,2,4,5,7,9,11},clefOffsets={0,-12,-6,-8},sharps={3,0,4,1,5,2,6},flats={6,2,5,1,4,0,3};
  static float P(float[] row,int n){return n<=row.Length?row[n-1]:0;}
  static int[] Order(IList<float[]> rows,short staff){return Enumerable.Range(0,rows.Count).Where(n=>P(rows[n],2)==staff).OrderBy(n=>P(rows[n],3)).ThenBy(n=>P(rows[n],1)==3?0:P(rows[n],1)==17?1:P(rows[n],1)==14?2:3).ToArray();}
  static int Letter(int position,out int octave){octave=(int)Math.Floor((position-1)/7.0);return position-1-octave*7;}
  static int Base(int position){int octave;int letter=Letter(position,out octave);return 60+12*octave+natural[letter];}
  static int KeyAlter(int position,int key){int octave;int letter=Letter(position,out octave);return key>0&&sharps.Take(key).Contains(letter)?1:key<0&&flats.Take(-key).Contains(letter)?-1:0;}
  static int Alter(int digit){switch(digit){case 1:return -1;case 2:return 1;case 3:return 0;case 4:return -2;case 5:return 2;default:throw new NotSupportedException("Chromatic correction requires conventional accidentals.");}}
  static float Cue(float pitch){return pitch+(pitch<0?-100:100);}
  static int Digit(int alter){return alter==-2?4:alter==-1?1:alter==0?3:alter==1?2:5;}
  static void Context(float[] row,ref int clef,ref int key,Dictionary<int,int> accidentals,Dictionary<int,int> custom){int code=(int)P(row,1);if(code==3){clef=(int)P(row,5);if(clef<0||clef>3)throw new NotSupportedException("Chromatic correction requires a standard clef.");}else if(code==14)accidentals.Clear();else if(code==17){
   if(P(row,8)!=0){key=0;custom.Clear();foreach(int field in new[]{8,9,10,11,12,13,16}){int glyph=(int)P(row,field);if(glyph==0)continue;int type=glyph/100,octave;int letter=Letter(glyph%100+(int)P(row,4)+clefOffsets[clef],out octave);if(type<1||type>3)throw new NotSupportedException("Custom key accidental must be flat, sharp or natural.");custom[letter]=type==1?-1:type==2?1:0;}accidentals.Clear();}
   else if(Math.Abs(P(row,5))<=7){key=(int)P(row,5);custom.Clear();accidentals.Clear();}
  }}
  static int Sound(float[] row,int clef,int key,Dictionary<int,int> accidentals,Dictionary<int,int> custom){int position=(int)P(row,4)%100+clefOffsets[clef],digit=Math.Abs((int)P(row,5))%10,alter=KeyAlter(position,key),octave;int letter=Letter(position,out octave);if(custom.ContainsKey(letter))alter=custom[letter];if(digit!=0){alter=Alter(digit);accidentals[position]=alter;}else if(accidentals.ContainsKey(position))alter=accidentals[position];return Base(position)+alter;}
  public static Dictionary<int,int> Apply(IList<float[]> before,IList<float[]> after,short staff,short semitones){
   if(before==null||after==null||after.Count<before.Count)throw new ArgumentException("Transposition rows lost correspondence.");
   var expected=new Dictionary<int,int>();var accidentals=new Dictionary<int,int>();int clef=0,key=0;var custom=new Dictionary<int,int>();
   foreach(int n in Order(before,staff)){var row=before[n];Context(row,ref clef,ref key,accidentals,custom);if(P(row,1)==1)expected.Add(n,Sound(row,clef,key,accidentals,custom)+semitones);}
   // Zero-key placeholders are semantic changes even though the original erases
   // their drawing code. Retain them for modern carry/cancellation processing.
   for(int n=0;n<before.Count;n++)if(P(before[n],1)==17&&P(after[n],1)==13&&P(after[n],5)==0)after[n][0]=17;
   for(int n=0;n<before.Count;n++)if(P(before[n],1)==17&&P(before[n],8)!=0)after[n]=(float[])before[n].Clone();
   var corrections=new Dictionary<int,int>();accidentals.Clear();custom.Clear();clef=key=0;
   foreach(int n in Order(after,staff)){var row=after[n];Context(row,ref clef,ref key,accidentals,custom);if(P(row,1)!=1||!expected.ContainsKey(n))continue;bool cue=Math.Abs(P(before[n],4))>=100;if(cue&&Math.Abs(row[3])<100)row[3]=Cue(row[3]);int position=(int)P(row,4)%100+clefOffsets[clef];var prior=new Dictionary<int,int>(accidentals);int actual=Sound(row,clef,key,accidentals,custom),desired=expected[n];if(actual==desired)continue;
    int candidate=position,alter=desired-Base(candidate);bool found=Math.Abs(alter)<=2;
    for(int distance=1;!found&&distance<=32;distance++)foreach(int direction in new[]{-1,1}){int possible=position+direction*distance,difference=desired-Base(possible);if(Math.Abs(difference)<=2){candidate=possible;alter=difference;found=true;break;}}
    if(!found)throw new InvalidOperationException("No conventional spelling for transposed pitch.");
    accidentals.Clear();foreach(var pair in prior)accidentals.Add(pair.Key,pair.Value);
    int delta=candidate-position;row[3]=cue?Cue(row[3]%100+delta):row[3]+delta;int oldDigit=Math.Abs((int)row[4])%10;row[4]=row[4]-oldDigit+Digit(alter);accidentals[candidate]=alter;if(delta!=0)corrections[n]=delta;
    if(Sound(row,clef,key,accidentals,custom)!=desired)throw new InvalidOperationException("Chromatic correction did not conserve pitch.");
   }return corrections;
  }
 }
}
