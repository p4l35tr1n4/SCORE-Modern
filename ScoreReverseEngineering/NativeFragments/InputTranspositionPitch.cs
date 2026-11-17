using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionPitchResult {public float Pitch,Stem;public short? AccidentalMarker;}
 // Main TR mode (argument word zero), original 3195:03E0..045C,
 // 05BC..0757. Analysis/negative modes use distinct original branches.
 public static class InputTranspositionPitch {
  static readonly short[] clefs={0,-12,-6,-8,-7};
  static readonly short[] encodings={4,1,0,2,5};
  static readonly short[,] keys={
   {-1,-1,-1,-1,-1,-1,-1},{-1,-1,-1,0,-1,-1,-1},{0,-1,-1,0,-1,-1,-1},
   {0,-1,-1,0,0,-1,-1},{0,0,-1,0,0,-1,-1},{0,0,-1,0,0,0,-1},{0,0,0,0,0,0,-1},
   {0,0,0,0,0,0,0},{0,0,0,1,0,0,0},{1,0,0,1,0,0,0},{1,0,0,1,1,0,0},
   {1,1,0,1,1,0,0},{1,1,0,1,1,1,0},{1,1,1,1,1,1,0},{1,1,1,1,1,1,1}};
  static short W(int value){return unchecked((short)value);}
  static short Word(float value){long result;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out result))throw new NotSupportedException("Non-finite transposition word.");return unchecked((short)result);}
  static short Degree(short value){short degree=W(W(value+6)%7+1);if(degree<=0)degree=W(degree+7);return degree;}
  public static TranspositionPitchResult Apply(float originalPitch,float originalStem,short collectedPitch,short accidental,short clef,short oldKey,short newKey,short diatonic,short keyOverride){
   if(clef<0||clef>=clefs.Length||oldKey< -7||oldKey>7||newKey< -7||newKey>7||accidental< -2||accidental>3)throw new NotSupportedException("Pitch transform outside recovered table domain.");
   short absolute=W(collectedPitch+clefs[clef]),degree=Degree(absolute),next=Degree(W(diatonic+degree));
   short target=keys[newKey+7,next-1],pitch=collectedPitch;var result=new TranspositionPitchResult{Stem=originalStem};
   if(accidental!=0||keyOverride==0){
    short encoded;
    if(accidental!=0){
     result.AccidentalMarker=W(absolute+100);short explicitValue=accidental==3?(short)0:accidental;
     target=W(target+W(explicitValue-keys[oldKey+7,degree-1]));
     if(target==0)encoded=3;
     else{if(target< -2){pitch=W(pitch+1);target=-2;}if(target>2){pitch=W(pitch-1);target=2;}encoded=encodings[target+2];}
    }else encoded=encodings[target+2];
    result.Stem=W(W(Word(originalStem)/10*10)+encoded);
   }
   short finalPitch=W(pitch+diatonic);
   if(Math.Abs(originalPitch)>=100)finalPitch=W(finalPitch+(finalPitch<0?-100:100));
   result.Pitch=finalPitch;return result;
  }
 }
}
