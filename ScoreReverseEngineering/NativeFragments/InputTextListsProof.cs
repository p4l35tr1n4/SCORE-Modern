using System;using System.IO;using ScoreModern.RecoveredFragments;
class InputTextListsProof {
 static int Main(string[] a){try{float[] actual=a[0]=="pitches"?InputTextLists.Pitches(a[1],new InputPitchState()):InputTextLists.Rhythms(a[1]);byte[] expected=File.ReadAllBytes(a[2]);int count=BitConverter.ToInt16(expected,0);if(actual.Length!=count)throw new Exception("Token count "+actual.Length+" != "+count);for(int i=0;i<count;i++)if(BitConverter.ToInt32(BitConverter.GetBytes(actual[i]),0)!=BitConverter.ToInt32(expected,2+i*4))throw new Exception("Token "+(i+1)+": "+actual[i]+" != "+BitConverter.ToSingle(expected,2+i*4));Console.WriteLine("PASS "+count+" original "+a[0]+" list token words");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
