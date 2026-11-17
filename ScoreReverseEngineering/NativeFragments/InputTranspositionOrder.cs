using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Section36:0007, measured at 32B0:0007 for the TR caller. Mode X,
 // nonzero secondary command word: stable signed-32 composite-key order;
 // no trailing-parameter normalization in this caller branch.
 public static class InputTranspositionOrder {
  // 006C..007C and 0390..039C: this caller marks the document
  // modified when sorting runs, even if its final order is unchanged.
  // Its nonzero secondary word returns before setting the sorted flag.
  public static bool MarksModified(int recordCount,bool alreadySorted){return recordCount>=2&&!alreadySorted;}
  static int Integer(double value){if(Double.IsNaN(value)||Double.IsInfinity(value)||value<Int32.MinValue||value>Int32.MaxValue)throw new NotSupportedException("Sort key outside verified finite signed-32 domain.");return (int)value;}
  public static int[] OrderForMain(IList<float[]> rows,bool alreadySorted){
   int count=rows.Count;var order=new int[count];for(int i=0;i<count;i++)order[i]=i;
   if(count<2||alreadySorted)return order;
   var keys=new int[count];var used=new bool[count];
   for(int i=0;i<count;i++){
    var row=rows[i];if(row.Length<2)throw new NotSupportedException("Sort requires a staff parameter.");
    int position=row.Length>=3?Integer((double)row[2]*100f):0;
    keys[i]=unchecked(position+Integer(row[1])*1000000);
    if(keys[i]>=99000000)throw new NotSupportedException("Original sort key reaches its sentinel domain.");
   }
   for(int output=0;output<count;output++){
    int minimum=99000000,index=-1;
    for(int i=0;i<count;i++)if(!used[i]&&keys[i]<minimum){minimum=keys[i];index=i;}
    if(index<0)throw new NotSupportedException("Original sort has no candidate.");order[output]=index;used[index]=true;
   }
   return order;
  }
 }
}

