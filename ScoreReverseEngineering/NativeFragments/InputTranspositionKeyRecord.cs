using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionKeyRecordResult {
  public float[] Parameters;
  public short OldKeyBase,NewKeyBase,Diatonic,PitchBase,KeyOverride;
 }
 // Main argument-zero key-record span, 3195:00A2..01EB, then the
 // kind-2 exit through 02A2..02D3 to 0757. Full command ownership is separate.
 public static class InputTranspositionKeyRecord {
  static short W(int value){return unchecked((short)value);}
  public static TranspositionKeyRecordResult Apply(float[] parameters,short collectedKey,short semitones,short previousOverride,short clef,short collectorIndex,float erasedCode){
   if(parameters.Length<3)throw new NotSupportedException("Key record is shorter than the verified loaded-record domain.");
   short old=collectedKey;if(Math.Abs((int)old)>=100)old=0;
   short diatonic,key;InputTranspositionKey.Convert(semitones,old,out diatonic,out key);
   var result=new TranspositionKeyRecordResult{Parameters=(float[])parameters.Clone(),OldKeyBase=W(7*old+49),NewKeyBase=W(7*key+49),Diatonic=diatonic,PitchBase=W(diatonic+6),KeyOverride=key};
   if(key==0&&(collectorIndex==1||previousOverride==0)){
    result.Parameters[0]=erasedCode;
   }else{
    short written=key==0?W((previousOverride<0?-100:100)+previousOverride):key;
    // 0165 skips P5 when it is absent. Later extension for P6 does
    // not retroactively write the new key into the inserted P5 slot.
    if(result.Parameters.Length>=5)result.Parameters[4]=written;
    if(result.Parameters.Length>=6)result.Parameters[5]=clef;
    else if(clef!=0){Array.Resize(ref result.Parameters,6);result.Parameters[5]=clef;}
   }
   return result;
  }
 }
}
