using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 //Positive-mode3195:0822..0C54. The returned frequency words are the caller-visible
 //9D4 table; duration-end copies at0BFF have no read in the Main TAB caller.
 public static class LayoutTablatureRelationships {
  static int Bits(float value){return BitConverter.ToInt32(BitConverter.GetBytes(value),0);}
  public static void Apply(IList<float[]> rows,TranspositionCollectorResult collected,Dictionary<int,short> frequencies){
   var entries=collected.FirstPass.Records;var relationships=collected.FirstPass.RelationshipItems;if(relationships.Count==0||entries.Count==0)return;
   int count=entries.Count;var ids=new int[count+1];var pitch=new short[count+1];var clef=new short[count+1];var accidental=new short[count+1];var kind=new short[count+1];var x=new float[count+1];
   for(int n=1;n<=count;n++){var entry=entries[n-1];ids[n]=entry.Item;pitch[n]=entry.Pitch??0;clef[n]=entry.Clef;accidental[n]=entry.Accidental;kind[n]=collected.Partition.Kinds[n-1];x[n]=entry.Position;}
   var pendingIds=new int[11];var pendingPitches=new short[11];var pendingClefs=new short[11];int gathered=0,pending=0,pendingRead=0,index=1,baseId=0,search=0,target=0,steps=0;short basePitch=0,baseClef=0,active=(short)relationships.Count,cursor=0;bool advanceThenNext=false,changedPosition=false;float position=0,candidate=0,first=collected.FirstPass.FirstNotePosition??-999;var window=new TranspositionRelationshipWindow{StartLow=-999,StartHigh=-999};
  Entry:
   if(++steps>20000)throw new NotSupportedException("Repeated original TAB relationship state is prevented.");if(index>count)goto Done;if(kind[index]<0||kind[index]>1)goto Next;position=x[index];if(cursor==0)goto AdvanceAgain;
  Position:
   if(window.StartLow>position)goto Next;if(position<=window.StartHigh)goto Start;
   if(position>first||pending==0)goto AdvanceAgain;
   pendingRead++;if(pendingRead>gathered)throw new NotSupportedException("TAB relationship pending index outside recovered table.");if(pendingRead==pending)pending=0;basePitch=pendingPitches[pendingRead];baseClef=pendingClefs[pendingRead];baseId=pendingIds[pendingRead];position=-999;search=index;goto Search;
  Start:
   if(index<count)goto Prepare;if(++gathered>10)throw new NotSupportedException("TAB relationship pending table capacity is prevented.");pendingIds[gathered]=ids[index];pendingPitches[gathered]=pitch[index];pendingClefs[gathered]=clef[index];goto AdvanceNext;
  Prepare:
   if(index==count)goto Done;basePitch=pitch[index];baseClef=clef[index];baseId=ids[index];search=index+1;
   if(accidental[search]!=0&&Bits(x[search])!=Bits(position)&&(pitch[search]!=basePitch||accidental[index]!=accidental[search]))goto AdvanceNext;
  Search:
   changedPosition=false;
  SearchEntry:
   if(search>count)goto AdvanceNext;if(kind[search]==2||kind[search]<0)goto AdvanceNext;
   if(accidental[search]!=0)goto SearchNext;if(clef[search]!=baseClef)goto AdvanceNext;candidate=x[search];
   if(changedPosition){if(Bits(candidate)!=Bits(position))goto AdvanceNext;}else{if(Bits(candidate)==Bits(position))goto SearchNext;position=candidate;changedPosition=true;}
   if(pitch[search]!=basePitch)goto SearchNext;if(candidate<window.EndLow||candidate>window.EndHigh)goto AdvanceNext;
   target=ids[search];if(!frequencies.ContainsKey(target))throw new NotSupportedException("TAB relationship references a non-note frequency slot.");frequencies[target]=0;ids[search]=baseId;advanceThenNext=true;if(Bits(candidate)!=Bits(first))goto Advance;goto AdvanceAgain;
  SearchNext:
   if(++steps>20000)throw new NotSupportedException("Repeated original TAB relationship search is prevented.");search++;goto SearchEntry;
  AdvanceNext:advanceThenNext=true;goto Advance;
  AdvanceAgain:advanceThenNext=false;
  Advance:
   if(cursor==active)goto Done;InputTranspositionRelationshipWindow.Advance(rows,relationships,ref active,ref cursor,ref window,3,6);if(advanceThenNext)goto Next;goto Position;
  Next:index++;goto Entry;
  Done:pending=gathered;
  }
 }
}
