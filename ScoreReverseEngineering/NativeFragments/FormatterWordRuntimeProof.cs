using System;
using System.IO;
using ScoreModern.RecoveredFragments;
class FormatterWordRuntimeProof {
 class Memory:IFormatterWordContext {
  public byte[] Bytes;
  public Memory(byte[] b){Bytes=(byte[])b.Clone();}
  public ushort ReadWord(ushort p){return (ushort)(Bytes[p]|(Bytes[(ushort)(p+1)]<<8));}
  public void WriteByte(ushort p,byte v){Bytes[p]=v;}
  public void WriteWord(ushort p,ushort v){Bytes[p]=(byte)v;Bytes[(ushort)(p+1)]=(byte)(v>>8);}
 }
 static void Main(string[] args){
  byte[] before=File.ReadAllBytes(args[0]),after=File.ReadAllBytes(args[1]);int value=int.Parse(args[2]);ushort sp=Convert.ToUInt16(args[3],16);
  if(before.Length!=65536||after.Length!=65536)throw new Exception("Snapshot length");
  Memory m=new Memory(before);FormatterWordWriter.Execute(m,(ushort)value,(ushort)(value>>16));int count=0;
  for(int i=0;i<65536;i++){
   if(i==(ushort)(sp-2)||i==(ushort)(sp-1)||i==(ushort)(sp-6)||i==(ushort)(sp-5))continue;
   if(m.Bytes[i]!=after[i])throw new Exception("Runtime mismatch at "+i.ToString("X4"));count++;
  }
  Console.WriteLine("PASS original runtime value "+value+"; "+count+" DS bytes; four exact PUSH bytes excluded; cursor "+m.ReadWord(0xd988).ToString("X4"));
 }
}
