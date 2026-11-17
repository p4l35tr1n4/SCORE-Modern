using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class DatTextLookupProof {
 class Memory:IDatTextLookupMemory {
  public byte[] Bytes=new byte[65536];public ushort Segment;public List<ushort> Reads=new List<ushort>();
  public ushort ReadWord(ushort s,ushort o){if(s!=Segment)throw new Exception("Segment");Reads.Add(o);return (ushort)(Bytes[o]|Bytes[(ushort)(o+1)]<<8);}
  public void Seed(ushort o,ushort v){Bytes[o]=(byte)v;Bytes[(ushort)(o+1)]=(byte)(v>>8);}
 }
 static DatTextLookupWords Interpret(byte[] code,Memory m,ushort ds){int pc=0;ushort bx=0,ax=0,dx=0;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length){switch(f()){
  case 0x8b:byte mod=f();if(mod==0x1e)bx=m.ReadWord(ds,word());else if(mod==0x87)ax=m.ReadWord(ds,(ushort)(bx+word()));else if(mod==0x97)dx=m.ReadWord(ds,(ushort)(bx+word()));else throw new Exception("MOV");break;
  case 0xd1:if(f()!=0xe3)throw new Exception("SHL");bx=(ushort)(bx<<1);break;
  case 0xeb:sbyte d=(sbyte)f();if(0x4c3+pc+d!=0x505)throw new Exception("Branch target");return new DatTextLookupWords{BX=bx,AX=ax,DX=dx};
  default:throw new Exception("Opcode");
 }}throw new Exception("Missing jump");}
 static bool Same(DatTextLookupWords a,DatTextLookupWords b){return a.AX==b.AX&&a.DX==b.DX&&a.BX==b.BX;}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[18];Array.Copy(raw,0x20f0+0x4c3,code,0,18);if(BitConverter.ToString(code)!="8B-1E-78-86-D1-E3-D1-E3-8B-87-3C-8A-8B-97-3E-8A-EB-30")throw new Exception("Original bytes");
  if(args.Length>1){string p=args[1];byte[] live=File.ReadAllBytes(Path.Combine(p,"lookup-code.bin"));if(live.Length!=18)throw new Exception("Live length");for(int i=0;i<18;i++)if(live[i]!=code[i])throw new Exception("Live code");Memory m=new Memory{Segment=H(args[2]),Bytes=File.ReadAllBytes(Path.Combine(p,"lookup-before-ds.bin"))};DatTextLookupWords r=DatTextLookup.Read(m,m.Segment);if(!Same(r,new DatTextLookupWords{BX=H(args[3]),AX=H(args[4]),DX=H(args[5])}))throw new Exception("Runtime words");byte[] after=File.ReadAllBytes(Path.Combine(p,"lookup-after-ds.bin"));if(m.Bytes.Length!=65536||after.Length!=65536)throw new Exception("Capture length");for(int i=0;i<65536;i++)if(m.Bytes[i]!=after[i])throw new Exception("Runtime memory");Console.WriteLine("PASS original natural font-98 DAT lookup: 18 live code bytes, AX/DX/BX and all 65536 DS bytes unchanged, zero exclusions.");return 0;}
  int count=0;for(int pattern=0;pattern<4;pattern++){Memory m=new Memory{Segment=(ushort)(0x2000+pattern)};for(int i=0;i<65536;i++)m.Bytes[i]=(byte)((i*37+pattern*51)^(i>>8));for(int index=0;index<65536;index++){m.Seed(0x8678,(ushort)index);m.Reads.Clear();DatTextLookupWords a=DatTextLookup.Read(m,m.Segment);ushort[] reads=m.Reads.ToArray();m.Reads.Clear();DatTextLookupWords b=Interpret(code,m,m.Segment);if(!Same(a,b)||reads.Length!=m.Reads.Count)throw new Exception("Model words/extent");for(int i=0;i<reads.Length;i++)if(reads[i]!=m.Reads[i])throw new Exception("Read order");count++;}}
  Console.WriteLine("PASS "+count+" original-instruction DAT lookup comparisons: every index word, four memory patterns, 16-bit shifts/address wrapping, index/table aliasing and ordered reads.");Console.WriteLine("Scope: word/read projection only; out-of-range cases are model tests, not original font workflows. No float arithmetic, character semantics, complete flags/ABI or rendering.");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
