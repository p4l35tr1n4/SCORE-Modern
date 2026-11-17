using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionRecord {
  public int Item;public short Kind,Accidental,Clef;public short? Pitch;public float Position;
 }
 public sealed class TranspositionCollection {
  public readonly List<TranspositionRecord> Records=new List<TranspositionRecord>();
  public readonly List<int> RelationshipItems=new List<int>();
  public bool HasExtendedBeam;public short FirstKeyState=-1;public float? FirstNotePosition;
 }
 // First pass only: original 32AC:000C..03D3. Subsequent bar/voice
 // partitioning is deliberately separate and not implemented by this leaf.
 public static class InputTranspositionCollection {
  static readonly short[] accidental={0,-1,1,3,-2,2};
  static short W(float value){long word;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word))throw new NotSupportedException("Non-finite transposition parameter.");return unchecked((short)word);}
  static float P(float[] row,int number){return row.Length>=number?row[number-1]:0;}
  public static TranspositionCollection FirstPass(IList<float[]> rows,short staff,float expandedLeft,float expandedRight,short mode){return FirstPass(rows,staff,expandedLeft,expandedRight,mode,null);}
  public static TranspositionCollection FirstPass(IList<float[]> rows,short staff,float expandedLeft,float expandedRight,short mode,short[] recoveredAccidentalTable){
   var map=recoveredAccidentalTable??accidental;
   var result=new TranspositionCollection();short clef=0;bool selectedNoteSeen=false;
   for(int index=0;index<rows.Count;index++){
    var row=rows[index];if(W(P(row,2))!=staff)continue;
    float x=P(row,3);if(x>expandedRight)break;
    short code=W(P(row,1));
    if(code<=2){
     if(x<expandedLeft)continue;
     if(row.Length>=7&&(BitConverter.ToUInt32(BitConverter.GetBytes(P(row,7)),0)&0x80000000)!=0)continue;
     if(row.Length<7||P(row,7)==0)throw new NotSupportedException("Original transposition refuses a selected note/rest with no duration.");
     if(code!=1&&code!=2)throw new NotSupportedException("Unverified transposition record code.");
     if(result.Records.Count>=200)throw new NotSupportedException("Transposition collection exceeds its recovered table capacity.");
     short value=0;short? pitch=null;
     if(code!=2){int digit=W(P(row,5))%10;if(digit<0||digit>=map.Length)throw new NotSupportedException("Accidental table index outside recovered range.");value=map[digit];pitch=unchecked((short)(W(P(row,4))%100));}
     if(!selectedNoteSeen)result.FirstNotePosition=x;
     result.Records.Add(new TranspositionRecord{Item=index+1,Kind=code==2?(short)-1:(short)0,Accidental=value,Pitch=pitch,Clef=clef,Position=x});
     selectedNoteSeen=true;
    }else if(code==5&&row.Length>=7){
     // 32AC:015D..01E7: compare the two stored float words, not
     // numerical equality; negative zero and positive zero differ here.
     bool enabled=row.Length<8||P(row,8)<=0;
     if(enabled&&BitConverter.ToUInt32(BitConverter.GetBytes(P(row,4)),0)==BitConverter.ToUInt32(BitConverter.GetBytes(P(row,5)),0)){
      if(result.RelationshipItems.Count>=50)throw new NotSupportedException("Relationship collection exceeds its safe table region.");
      result.RelationshipItems.Add(index+1);
     }
    }else if(code==3&&row.Length>=5){clef=W(P(row,5));if(mode==15&&P(row,5)==0.8f)clef=4;}
    else if(code==17){
     if(result.FirstKeyState<0)result.FirstKeyState=selectedNoteSeen?(short)1:(short)0;
     if(result.Records.Count>=200)throw new NotSupportedException("Transposition collection exceeds its recovered table capacity.");
     result.Records.Add(new TranspositionRecord{Item=index+1,Kind=2,Accidental=W(P(row,5)),Clef=clef,Position=x});
    }else if(code==6&&row.Length>=7)result.HasExtendedBeam=true;
   }
   return result;
  }
  // Original 32AC:03D9..04FC, before boundary sorting/partitioning.
  // Caller supplies the measured first selected note position; key-only
  // selections require separate retained-state verification.
  public static List<float> Boundaries(IList<float[]> rows,short staff,float firstNotePosition,float expandedRight){
   var result=new List<float>();
   foreach(var row in rows){float x=P(row,3);if(x<firstNotePosition)continue;if(x>expandedRight)break;
    if(W(P(row,1))!=14||W(P(row,2))>staff)continue;
    short span=row.Length>=4?W(P(row,4)):(short)1;if(span==0)span=1;
    if(unchecked((short)(staff-W(P(row,2))))>=span)continue;
    uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(x),0);bool found=false;
    foreach(float boundary in result)if(BitConverter.ToUInt32(BitConverter.GetBytes(boundary),0)==bits){found=true;break;}
    if(found)continue;if(result.Count>=99)throw new NotSupportedException("Original boundary table exceeds its safe sentinel capacity.");result.Add(x);
   }
   return result;
  }
 }
}
