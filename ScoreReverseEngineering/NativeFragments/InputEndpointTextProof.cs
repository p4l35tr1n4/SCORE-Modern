using System;using System.IO;using ScoreModern.RecoveredFragments;
class InputEndpointTextProof {
 static int Main(string[] a){try{var parsed=InputEndpointText.Parse(a[1],a[0]=="beams",4,4);float[] values=parsed.Automatic?new[]{9999f,parsed.GroupControl,(float)parsed.Skip}:parsed.Pairs;byte[] expected=File.ReadAllBytes(a[2]);if(values.Length!=BitConverter.ToInt16(expected,0))throw new Exception("Endpoint token count");for(int i=0;i<values.Length;i++)if(BitConverter.ToInt32(BitConverter.GetBytes(values[i]),0)!=BitConverter.ToInt32(expected,2+i*4))throw new Exception("Token "+i+": "+values[i]+" != "+BitConverter.ToSingle(expected,2+i*4));Console.WriteLine("PASS "+values.Length+" original endpoint text token words");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
