using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class ParameterWordReaderProof {
 class Memory:IParameterWordMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();
  byte Raw(ushort s,ushort o){byte b;return Bytes.TryGetValue((s<<4)+o,out b)?b:(byte)0;}
  public byte ReadByte(ushort s,ushort o){byte b=Raw(s,o);Events.Add("B"+((s<<4)+o)+":"+b);return b;}
  public ushort ReadWord(ushort s,ushort o){ushort x=(ushort)(Raw(s,o)|(Raw(s,(ushort)(o+1))<<8));Events.Add("R"+((s<<4)+o)+":"+x);return x;}
  public void WriteWord(ushort s,ushort o,ushort x){Seed(s,o,(byte)x);Seed(s,(ushort)(o+1),(byte)(x>>8));Events.Add("W"+((s<<4)+o)+":"+x);}
  public void Seed(ushort s,ushort o,byte x){Bytes[(s<<4)+o]=x;}
  public Memory Copy(){Memory m=new Memory();foreach(var x in Bytes)m.Bytes[x.Key]=x.Value;return m;}
 }
 static ushort Reference(byte[] c,Memory m,ushort ss,ushort bp,ushort ds,ushort si){
  int pc=0;ushort bx=0,es=0,ax=0,dx=0;bool equal=false;Func<byte> f=()=>c[pc++];
  while(pc<c.Length){byte op=f(),q;ushort at,x;switch(op){
   case 0xc4:if(f()!=0x5e)throw new Exception("LES");at=(ushort)(bp+(sbyte)f());bx=m.ReadWord(ss,at);es=m.ReadWord(ss,(ushort)(at+2));break;
   case 0xff:if(f()!=0x46)throw new Exception("INC");at=(ushort)(bp+(sbyte)f());x=m.ReadWord(ss,at);m.WriteWord(ss,at,(ushort)(x+1));break;
   case 0x26:op=f();q=f();if(op==0x8a&&q==7)ax=(ushort)((ax&0xff00)|m.ReadByte(es,bx));else if(op==0x8b&&q==7)ax=m.ReadWord(es,bx);else if(op==0x8b&&q==0x57)dx=m.ReadWord(es,(ushort)(bx+(sbyte)f()));else throw new Exception("ES read");break;
   case 0x98:ax=unchecked((ushort)(short)(sbyte)ax);break;
   case 0x99:dx=(short)ax<0?(ushort)0xffff:(ushort)0;break;
   case 0x89:q=f();if(q==4)m.WriteWord(ds,si,ax);else if(q==0x54)m.WriteWord(ds,(ushort)(si+(sbyte)f()),dx);else throw new Exception("Store");break;
   case 0x80:if(f()!=0x3c)throw new Exception("CMP");equal=m.ReadByte(ds,si)==f();break;
   case 0x75:sbyte displacement=(sbyte)f();if(!equal)pc+=displacement;break;
   case 0x83:q=f();if(q==0x46){at=(ushort)(bp+(sbyte)f());byte increment=f();x=m.ReadWord(ss,at);m.WriteWord(ss,at,(ushort)(x+increment));}else if(q==0xc6)si=(ushort)(si+f());else throw new Exception("ADD");break;
   default:throw new Exception("Opcode "+op.ToString("X2"));
  }}return si;
 }
 static byte[] code;static int count;
 static void Check(byte first,ushort low,ushort high,int layout){
  ushort ss=0x2000,bp=0x8008,ds=layout==7?(ushort)0x1fff:ss,si=(ushort)(layout<6?0x8000+2*layout:0x1000);if(layout==7)si=0x8010;
  Memory seed=new Memory();seed.Seed(0x2000,0x9000,first);seed.WriteWord(0x2000,0x9001,low);seed.WriteWord(0x2000,0x9003,high);seed.WriteWord(ss,0x8000,0x9000);seed.WriteWord(ss,0x8002,0x2000);seed.Events.Clear();
  Memory actual=seed.Copy(),reference=seed.Copy();ushort expected=Reference(code,reference,ss,bp,ds,si),result=ParameterWordReader.Execute(actual,ss,0x8000,ds,si);
  if(expected!=result||actual.Events.Count!=reference.Events.Count||actual.Bytes.Count!=reference.Bytes.Count)throw new Exception("Extent/result");
  for(int n=0;n<actual.Events.Count;n++)if(actual.Events[n]!=reference.Events[n])throw new Exception("Memory order");
  foreach(var p in actual.Bytes)if(!reference.Bytes.ContainsKey(p.Key)||reference.Bytes[p.Key]!=p.Value)throw new Exception("Memory mismatch");count++;
 }
 class Captured:IParameterWordMemory {
  public byte[] Data;public Captured(byte[] b){Data=(byte[])b.Clone();}
  void Segment(ushort s){if(s!=0x871e)throw new Exception("Uncaptured segment");}
  public byte ReadByte(ushort s,ushort p){Segment(s);return Data[p];}
  public ushort ReadWord(ushort s,ushort p){Segment(s);return (ushort)(Data[p]|Data[(ushort)(p+1)]<<8);}
  public void WriteWord(ushort s,ushort p,ushort v){Segment(s);Data[p]=(byte)v;Data[(ushort)(p+1)]=(byte)(v>>8);}
 }
 static void Main(string[] a){
  byte[] original=File.ReadAllBytes(a[0]);code=new byte[43];Array.Copy(original,0x1f080+0x36e0,code,0,43);
  string hex=BitConverter.ToString(code).Replace("-","").ToLowerInvariant();if(hex!="c45ef8ff46f8268a0798998904895402803c7f7513c45ef8268b07268b570289048954028346f80483c604")throw new Exception("Original identity");
  if(a.Length>1){
   byte[] runtimeCode=File.ReadAllBytes(a[6]);if(runtimeCode.Length!=43)throw new Exception("Runtime code length");for(int i=0;i<43;i++)if(runtimeCode[i]!=code[i])throw new Exception("Runtime byte identity");
   byte[] before=File.ReadAllBytes(a[1]),after=File.ReadAllBytes(a[2]);Captured m=new Captured(before);
   ushort cursor=Convert.ToUInt16(a[3],16),destination=Convert.ToUInt16(a[4],16),expectedSi=Convert.ToUInt16(a[5],16);
   ushort si=ParameterWordReader.Execute(m,0x871e,cursor,0x871e,destination);if(si!=expectedSi)throw new Exception("Runtime SI");
   for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("Original runtime DS mismatch "+i.ToString("X4"));
   Console.WriteLine("PASS exact 43 original runtime bytes; all 65536 original DS bytes, including cursor/local storage, and updated SI; no exclusions.");return;
  }
  for(int b=0;b<256;b++)for(int layout=0;layout<8;layout++)Check((byte)b,0x1234,0x89ab,layout);
  for(int low=0;low<65536;low++)for(int layout=0;layout<8;layout++)Check(0x7f,(ushort)low,(ushort)(low^0xa55a),layout);
  Console.WriteLine("PASS "+count+" original-instruction comparisons; exact 43-byte identity; ordered memory operations, cursor/destination overlap and physically aliased segment layouts. Original runtime comparison is separate.");
 }
}
