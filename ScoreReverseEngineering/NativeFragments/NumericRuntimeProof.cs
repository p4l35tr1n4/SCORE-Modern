using System;
using System.IO;
using ScoreModern.RecoveredFragments;
class NumericRuntimeProof {
 class Memory:INumericConverterContext {
  public byte[] Bytes;ushort ds;
  public Memory(byte[] b,ushort segment){Bytes=(byte[])b.Clone();ds=segment;}
  public byte ReadByte(ushort s,ushort p){if(s!=ds)throw new Exception("Uncaptured segment");return Bytes[p];}
  public void WriteWord(ushort s,ushort p,ushort v){if(s!=ds)throw new Exception("Uncaptured segment");Bytes[p]=(byte)v;Bytes[(ushort)(p+1)]=(byte)(v>>8);}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static void Main(string[] a){
  byte[] before=File.ReadAllBytes(a[0]),after=File.ReadAllBytes(a[1]);ushort sp=H(a[2]),ds=H(a[3]);int stackBytes=int.Parse(a[4]);
  Memory m=new Memory(before,ds);NumericConverterResult r=NumericConverter.Execute(m,H(a[5]),H(a[6]),H(a[7]),H(a[8]));
  if(r.AX!=H(a[9])||r.DX!=H(a[10])||r.Status!=H(a[11]))throw new Exception("Return/status mismatch");
  int n=0;for(int i=0;i<65536;i++){if(i>=sp-stackBytes&&i<sp)continue;if(m.Bytes[i]!=after[i])throw new Exception("DS mismatch at "+i.ToString("X4"));n++;}
  Console.WriteLine("PASS AX="+r.AX.ToString("X4")+" DX="+r.DX.ToString("X4")+" status="+r.Status.ToString("X4")+"; "+n+" DS bytes; "+stackBytes+" exact stack-write bytes excluded.");
 }
}
