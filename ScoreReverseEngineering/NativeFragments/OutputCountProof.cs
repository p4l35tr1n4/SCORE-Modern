using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class OutputCountProof {
 class Memory:ISpanBookkeepingMemory {
  public Dictionary<int,ushort> Words=new Dictionary<int,ushort>();public List<string> Events=new List<string>();
  public ushort ReadWord(ushort s,ushort o){int a=(s<<4)+o;ushort v;Words.TryGetValue(a,out v);Events.Add("R"+a+":"+v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){int a=(s<<4)+o;Words[a]=v;Events.Add("W"+a+":"+v);}
  public byte ReadByte(ushort s,ushort o){throw new Exception("Unexpected byte read");}
  public void WriteByte(ushort s,ushort o,byte v){throw new Exception("Unexpected byte write");}
  public Memory Copy(){Memory m=new Memory();foreach(var p in Words)m.Words[p.Key]=p.Value;return m;}
 }
 static OutputCountResult Reference(byte[] c,Memory m,ushort ss,ushort bp,ushort ds,ushort si){
  int pc=0;ushort ax=0,dx=0;bool carry=false;Func<byte> f=()=>c[pc++];
  while(pc<c.Length){byte op=f();ushort at,old;uint sum;switch(op){
   case 0x8b:if(f()!=0x46)throw new Exception("MOV");ax=m.ReadWord(ss,(ushort)(bp+(sbyte)f()));break;
   case 0x99:dx=(short)ax<0?(ushort)0xffff:(ushort)0;break;
   case 0x01:if(f()!=0x44)throw new Exception("ADD");at=(ushort)(si+(sbyte)f());old=m.ReadWord(ds,at);sum=(uint)old+ax;carry=sum>65535;m.WriteWord(ds,at,(ushort)sum);break;
   case 0x11:if(f()!=0x54)throw new Exception("ADC");at=(ushort)(si+(sbyte)f());old=m.ReadWord(ds,at);sum=(uint)old+dx+(carry?1u:0u);m.WriteWord(ds,at,(ushort)sum);break;
   default:throw new Exception("Opcode");
  }}return new OutputCountResult(ax,dx);
 }
 class Captured:ISpanBookkeepingMemory {
  public byte[] Data;public Captured(byte[] b){Data=(byte[])b.Clone();}
  public byte ReadByte(ushort s,ushort p){if(s!=0x871e)throw new Exception("Segment");return Data[p];}
  public ushort ReadWord(ushort s,ushort p){return (ushort)(ReadByte(s,p)|(ReadByte(s,(ushort)(p+1))<<8));}
  public void WriteByte(ushort s,ushort p,byte v){if(s!=0x871e)throw new Exception("Segment");Data[p]=v;}
  public void WriteWord(ushort s,ushort p,ushort v){WriteByte(s,p,(byte)v);WriteByte(s,(ushort)(p+1),(byte)(v>>8));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static void Main(string[] a){
  byte[] source=File.ReadAllBytes(a[0]),code=new byte[10];Array.Copy(source,0x1f080+0x4863,code,0,10);if(BitConverter.ToString(code)!="8B-46-08-99-01-44-12-11-54-14")throw new Exception("Code identity");
  if(a.Length>1){byte[] actual=File.ReadAllBytes(a[1]);if(actual.Length!=10)throw new Exception("Runtime code length");for(int i=0;i<10;i++)if(code[i]!=actual[i])throw new Exception("Runtime code");Captured m=new Captured(File.ReadAllBytes(a[2]));OutputCountResult r=OutputCountAdvance.Execute(m,H(a[4]),H(a[5]),H(a[6]),H(a[7]));if(r.AX!=H(a[8])||r.DX!=H(a[9]))throw new Exception("Runtime result");byte[] after=File.ReadAllBytes(a[3]);for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("Runtime DS "+i.ToString("X4"));Console.WriteLine("PASS original output count block: exact 10 code bytes, all 65536 DS bytes and AX/DX; zero exclusions.");return;}
  int total=0;for(int n=0;n<65536;n++)for(int seed=0;seed<4;seed++)for(int layout=0;layout<4;layout++){
   ushort ds=0x2000,ss=layout==3?(ushort)0x1fff:ds,si=0x9000,arg=layout==1?(ushort)0x9012:layout==2?(ushort)0x9014:layout==3?(ushort)0x8010:(ushort)0x8000;
   Memory initial=new Memory();initial.WriteWord(ds,0x9012,new ushort[]{0,0xffff,0xffff,(ushort)(n*31)}[seed]);initial.WriteWord(ds,0x9014,new ushort[]{0,0xffff,0x7fff,(ushort)(n^0xa55a)}[seed]);initial.WriteWord(ss,arg,(ushort)n);Memory actual=initial.Copy(),reference=initial.Copy();OutputCountResult aResult=OutputCountAdvance.Execute(actual,ss,arg,ds,si),e=Reference(code,reference,ss,(ushort)(arg-8),ds,si);
   if(aResult.AX!=e.AX||aResult.DX!=e.DX||actual.Words.Count!=reference.Words.Count||actual.Events.Count!=reference.Events.Count)throw new Exception("Result/extent");for(int i=0;i<actual.Events.Count;i++)if(actual.Events[i]!=reference.Events[i])throw new Exception("Memory order");foreach(var p in actual.Words)if(!reference.Words.ContainsKey(p.Key)||reference.Words[p.Key]!=p.Value)throw new Exception("Memory");total++;
  }
  Console.WriteLine("PASS "+total+" original-instruction output-count cases; all count words, four counter patterns, signed extension/carry and four argument/counter/segment layouts.");
 }
}
