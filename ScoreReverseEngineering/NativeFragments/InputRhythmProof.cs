using System;using System.IO;using System.Globalization;using ScoreModern.RecoveredFragments;
class InputRhythmProof {
 static uint Bits(float x){return BitConverter.ToUInt32(BitConverter.GetBytes(x),0);}
 static float Read(string x){return BitConverter.ToSingle(BitConverter.GetBytes(uint.Parse(x,NumberStyles.HexNumber)),0);}
 static int Main(string[] args){try{int n=0;foreach(string line in File.ReadAllLines(args[0])){if(line.Length==0)continue;string[] f=line.Split('\t');var v=InputRhythmDecoder.Decode(Read(f[0]));float[] values={v.FlagCount,v.DotControl,Read(f[0]),v.Denominator,v.Duration};for(int k=0;k<5;k++)if(Bits(values[k])!=uint.Parse(f[k+1],NumberStyles.HexNumber))throw new Exception("Case "+n+" output "+k+": "+Bits(values[k]).ToString("x8")+" expected "+f[k+1]);if(f.Length>6&&Bits(InputRhythmToken.Parse(f[6]))!=Bits(Read(f[0])))throw new Exception("Token "+f[6]);n++;}Console.WriteLine("PASS "+n+" original rhythm-helper cases; five binary32 argument words per case.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
