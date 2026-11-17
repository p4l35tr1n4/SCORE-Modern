using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2F68:033A..03DA. The two tests use the same originally loaded P3,
 // even if the first test has already stored zero. All indexed items are visited,
 // including those outside the command's selected system/range.
 public static class LayoutJustificationSnap {
  public static void Apply(float[] words,int[] itemStarts,float tolerance,float rightEdge) {
   if(words==null||itemStarts==null)throw new ArgumentNullException();
   foreach(int word in itemStarts) {
    int header=word-1;
    if(header<0||header>=words.Length)throw new NotSupportedException("Original coordinate snap leaves its record bank.");
    long count;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(words[header]),0),out count))throw new NotSupportedException("Original coordinate snap count leaves its finite domain.");
    if(unchecked((short)count)<3)continue;
    int p3=header+3;if(p3>=words.Length)throw new NotSupportedException("Original coordinate snap leaves its record bank.");
    float x=words[p3];
    if(Math.Abs((double)x)<tolerance)words[p3]=0;
    if(Math.Abs((double)rightEdge-x)<tolerance)words[p3]=rightEdge;
   }
  }
 }
}
