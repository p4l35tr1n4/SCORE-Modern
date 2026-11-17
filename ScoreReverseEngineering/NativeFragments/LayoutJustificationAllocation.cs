using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class JustificationAllocationResult {
  public short NextIndex;public float Allocated,Remaining;
 }
 // Decoded original 2F12:0493: one allocation step, seven far-pointer arguments.
 // Runtime verified at its natural caller/return and 14 isolated finite cases;
 // not yet integrated into Main J.
 // Separate binary32 stores are retained. Caller collection, ordering and
 // complete justification are separate routines; this helper does not guess
 // either the minimum gap or the relationship between score records.
 public static class LayoutJustificationAllocation {
  public static JustificationAllocationResult Apply(float[] positions,float[] allocations,short index,float target,float limit,float remaining,float minimum){
   if(positions==null||allocations==null||index<1||index>=positions.Length||index>allocations.Length)throw new NotSupportedException("Original justification allocation would leave its buffers.");
   float anchor=positions[index-1];
   float allocation=(float)(((double)target-anchor)*remaining/((double)limit-anchor));
   if(Single.IsNaN(allocation)||Single.IsInfinity(allocation))throw new NotSupportedException("Original justification allocation produced a non-finite value.");
   if(allocation<minimum)allocation=minimum;
   allocations[index-1]=allocation;
   remaining=(float)((double)remaining-allocation);
   short next=unchecked((short)(index+1));positions[next-1]=target;
   return new JustificationAllocationResult{NextIndex=next,Allocated=allocation,Remaining=remaining};
  }
 }
}
