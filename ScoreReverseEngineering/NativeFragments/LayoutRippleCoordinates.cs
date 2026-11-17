using System;
namespace ScoreModern.RecoveredFragments {
 // Original RIP's 2C0C:0971 and 09D5, with signed one-based coordinate words.
 public static class LayoutRippleCoordinates {
  public static void Shift(float[] words,short[] references,float delta){foreach(short reference in references){int at=Math.Abs((int)reference)-1;if(at<0||at>=words.Length)throw new NotSupportedException("Original RIP coordinate leaves its word bank.");words[at]=(float)((double)words[at]+delta);}}
  public static void Scale(float[] words,short[] references,float origin,float factor){float offset=(float)((double)origin-(double)origin*factor);foreach(short reference in references){int at=Math.Abs((int)reference)-1;if(at<0||at>=words.Length)throw new NotSupportedException("Original RIP coordinate leaves its word bank.");words[at]=(float)((double)words[at]*factor+offset);}}
  public static void StaffShift(float[] words,short[] references,float delta){foreach(short reference in references)if(reference>0){int at=reference-2;if(at<0||at>=words.Length)throw new NotSupportedException("Original RIP staff reference leaves its word bank.");words[at]=(float)((double)words[at]+delta);}}
 }
}
