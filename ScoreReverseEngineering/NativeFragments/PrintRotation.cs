using System;
namespace ScoreModern.RecoveredFragments {
 public static class PrintRotation {
  // Verified FLD/FMUL/FSTP uses DS:281C, with DS:7290=1.3333f and
  // DS:728C=0.75f in the measured print overlay. Size-word effect; Offset consumes the separate integer candidate.
  // Original print-menu size word: rounded binary32 literal1.3333 on,
  // exactly0.75 off. The unchanged flag path does not multiply again.
  // Data effect only; no x87 flags, stack or calling-convention adapter.
  public static float Apply(float size,bool before,bool after){if(before==after)return size;return (float)((double)size*(after?(double)1.3333f:.75));}
  public static int Offset(int candidate,bool before,bool after){if(before==after)return candidate;return checked((int)Math.Truncate(candidate*(after?(double)1.333f:.75)));}
 }
}
