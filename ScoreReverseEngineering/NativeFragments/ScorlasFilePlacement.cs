namespace ScoreModern.RecoveredFragments {
 // Original SCORLAS measured runtime 0C7A:0680..081A. Inputs are the
 // original signed-word scan extrema and an already converted int32 guide.
 // This does not model the separate floating-point P8 conversion.
 public struct ScorlasFilePlacement {
  public int Delta,Cumulative,NextAdvance,OverallHeight,RawHeight,BottomShift,NormalizedHeight,DscBottom,DscTop;
 }
 public sealed class ScorlasFilePlacementState {
  bool started;int previousGuide,previousAdvance,firstBottomShift,cumulative;
  public ScorlasFilePlacement Add(short minimum,short maximum,int guide){
   int height=unchecked((int)maximum-minimum),shift=unchecked((short)(minimum+24000)),normalized=unchecked(height+shift),delta=0,total=height;
   if(!started){firstBottomShift=shift;cumulative=0;started=true;}
   else{int correction=previousGuide!=0?0:shift;delta=unchecked(previousAdvance-correction);cumulative=unchecked(cumulative+delta);total=unchecked(cumulative+normalized-firstBottomShift);}
   previousGuide=guide;previousAdvance=guide!=0?guide:unchecked(normalized+1000);
   // 06A2..06A9 replaces 3494 with this file's signed minimum. 07A8..07B3
   // stores the cumulative height; 07FF..0811 adds it to that same minimum.
   // These finalizer bounds are deliberately separate from translated ink.
   return new ScorlasFilePlacement{Delta=delta,Cumulative=cumulative,NextAdvance=previousAdvance,OverallHeight=total,RawHeight=height,BottomShift=shift,NormalizedHeight=normalized,DscBottom=minimum,DscTop=unchecked(minimum+total)};
  }
 }
}
