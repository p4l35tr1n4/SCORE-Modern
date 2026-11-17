using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class ConsumerContextProof {
 class Memory:IConsumerContextMemory {
  public Dictionary<int,ushort> Words=new Dictionary<int,ushort>();public List<string> Events=new List<string>();
  public ushort ReadWord(ushort s,ushort o){int a=(s<<4)+o;ushort x;Words.TryGetValue(a,out x);Events.Add("R"+a+":"+x);return x;}
  public void WriteWord(ushort s,ushort o,ushort x){int a=(s<<4)+o;Words[a]=x;Events.Add("W"+a+":"+x);}
  public Memory Copy(){Memory m=new Memory();foreach(var x in Words)m.Words[x.Key]=x.Value;return m;}
 }
 // This interpreter decodes the exact 42-byte routine through the RETF
 // boundary. Registers/flags/return transfer beyond the native data interface
 // are not compared; all ordered memory operations are compared.
 static ushort Reference(byte[] code,Memory m,ushort ss,ushort sp,ushort bp,ushort si,ushort di,ushort ds){
  int pc=0;ushort ax=0,bx=0,cx=0,dx=0;Func<byte> f=()=>code[pc++];
  while(true){byte op=f(),q,d;switch(op){
   case 0x8b:q=f();switch(q){case 0xc5:ax=bp;break;case 0xec:bp=sp;break;case 0x4e:d=f();cx=m.ReadWord(ss,(ushort)(bp+(sbyte)d));break;case 0xe8:bp=ax;break;default:throw new Exception("MOV");}break;
   case 0x8c:if(f()!=0xda)throw new Exception("Segment read");dx=ds;break;
   case 0xc5:if(f()!=0x5e)throw new Exception("LDS");d=f();bx=m.ReadWord(ss,(ushort)(bp+(sbyte)d));ds=m.ReadWord(ss,(ushort)(bp+(sbyte)d+2));break;
   case 0x89:q=f();if(q==0x07)m.WriteWord(ds,bx,ax);else{d=f();ushort value;if(q==0x7f)value=di;else if(q==0x77)value=si;else if(q==0x67)value=sp;else if(q==0x4f)value=cx;else if(q==0x57)value=dx;else throw new Exception("Store");m.WriteWord(ds,(ushort)(bx+(sbyte)d),value);}break;
   case 0x8e:if(f()!=0xda)throw new Exception("Segment write");ds=dx;break;
   case 0x33:if(f()!=0xc0)throw new Exception("XOR");ax=0;break;
   case 0xcb:return ax;
   default:throw new Exception("Opcode");
  }}
 }
 static void Main(string[] a){
  byte[] b=File.ReadAllBytes(a[0]),code=new byte[42];Array.Copy(b,0x18b90+0x233e,code,0,42);byte[] runtime=File.ReadAllBytes(a[1]);if(runtime.Length!=42)throw new Exception("Code length");for(int n=0;n<42;n++)if(code[n]!=runtime[n])throw new Exception("Code identity");
  int count=0;for(int n=0;n<65536;n++)for(int layout=0;layout<8;layout++){
   ushort ss=0x2000,ds=(ushort)(n%2==0?0x2000:0x3000),sp=0x8000,ptr=(ushort)(layout==0?0x1000:sp-4+2*layout),seg=(ushort)(layout==7?0x1fff:ss);if(layout==7)ptr=(ushort)(ptr+16);
   Memory seed=new Memory();seed.Words[(ss<<4)+sp]=(ushort)(n^0x1234);seed.Words[(ss<<4)+sp+2]=(ushort)(n*17);seed.Words[(ss<<4)+sp+4]=ptr;seed.Words[(ss<<4)+sp+6]=seg;
   Memory actual=seed.Copy(),reference=seed.Copy();ushort bp=(ushort)n,si=(ushort)(n*31+7),di=(ushort)(n^0xa55a);
   ushort expected=Reference(code,reference,ss,sp,bp,si,di,ds),result=ConsumerContextSaver.Execute(actual,ss,sp,bp,si,di,ds);
   if(result!=expected||actual.Events.Count!=reference.Events.Count||actual.Words.Count!=reference.Words.Count)throw new Exception("Result/extent");
   for(int i=0;i<actual.Events.Count;i++)if(actual.Events[i]!=reference.Events[i])throw new Exception("Ordered memory event mismatch");
   foreach(var p in actual.Words)if(!reference.Words.ContainsKey(p.Key)||reference.Words[p.Key]!=p.Value)throw new Exception("Memory mismatch");count++;
  }
  byte[] before=File.ReadAllBytes(a[2]),after=File.ReadAllBytes(a[3]);Memory captured=new Memory();for(int n=0;n<65535;n+=2)captured.Words[(0x871e<<4)+n]=(ushort)(before[n]|before[n+1]<<8);
  // Snapshot destination 6293 is odd, so byte-wise runtime memory is compared
  // with a separate adapter that accurately models overlapping word writes.
  Bytes mem=new Bytes(before);ushort ret=ConsumerContextSaver.Execute(mem,0x871e,0xe1f0,0xe1fc,0x3ac7,0x7e82,0x871e);if(ret!=0)throw new Exception("Return");
  for(int n=0;n<65536;n++)if(mem.Data[n]!=after[n])throw new Exception("Original DS mismatch "+n.ToString("X4"));
  Console.WriteLine("PASS "+count+" original-instruction comparisons including ordered reads/writes and stack/segment alias layouts; exact 42-byte identity; all 65536 original runtime DS bytes match without exclusions.");
 }
 class Bytes:IConsumerContextMemory {
  public byte[] Data;public Bytes(byte[] b){Data=(byte[])b.Clone();}
  public ushort ReadWord(ushort s,ushort p){if(s!=0x871e)throw new Exception("Uncaptured segment");return (ushort)(Data[p]|Data[(ushort)(p+1)]<<8);}
  public void WriteWord(ushort s,ushort p,ushort v){if(s!=0x871e)throw new Exception("Uncaptured segment");Data[p]=(byte)v;Data[(ushort)(p+1)]=(byte)(v>>8);}
 }
}
