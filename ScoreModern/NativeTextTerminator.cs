using System;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // Original converter 0A33:0090..0096 appends one space after the
 // declared text. Preserve the measured in-record padding write. A write
 // into the next record is gated by the existing original-invalid-states policy.
 public static class NativeTextTerminator {
  public static bool Refresh(Score score,Item item){
   if(item==null||item.Code!=16||item.Count<13)return false;
   // A stored empty line is measured using the caller's temporary blank,
   // not a writable zero-byte RN payload (natural all-blank RIP capture).
   float length=item.P(12);if(Single.IsNaN(length)||Single.IsInfinity(length)||length<=0||length>item.Bytes.Length-56)return false;
   int at=56+(int)Math.Truncate(length);
   if(at<item.Bytes.Length){if(item.Bytes[at]==InputTextTerminatorWrite.Space)return false;item.Bytes[at]=InputTextTerminatorWrite.Space;return true;}
   if(at!=item.Bytes.Length||!NativeEditCompatibility.ReproduceInvalidStates)return false;
   int index=score.Items.IndexOf(item);if(index<0||index+1>=score.Items.Count)return false;
   var next=score.Items[index+1];if(next.Bytes.Length==0||next.Bytes[0]==InputTextTerminatorWrite.Space)return false;
   next.Bytes[0]=InputTextTerminatorWrite.Space;return true;
  }
 }
}
