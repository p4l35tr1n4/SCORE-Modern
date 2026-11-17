using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class CharacterDecisionProof {
 class Memory:ICharacterDecisionMemory {
  public byte[] Bytes=new byte[0x110000];public List<long> Reads=new List<long>();
  public byte ReadByte(ushort s,ushort o){int p=(s<<4)+o;byte v=Bytes[p];Reads.Add(((long)p<<16)|v);return v;}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;ushort v=(ushort)(Bytes[p]|Bytes[(s<<4)+(ushort)(o+1)]<<8);Reads.Add(((long)p<<16)|v);return v;}
  public void Seed(ushort s,ushort o,byte v){Bytes[(s<<4)+o]=v;}
 }
 static CharacterDecisionResult Interpret(byte[] code,Memory m,ushort ds,ushort es,ushort di,byte dl,ushort bx,int entry){
  int pc=0,cmp=0;byte al=0;Func<byte> f=()=>code[pc++];Func<ushort> w=()=>{byte l=f();return (ushort)(l|f()<<8);};
  while(pc<code.Length){byte op=f();switch(op){
   case 0x26:if(f()!=0x8a||f()!=5)throw new Exception("LES byte read");al=m.ReadByte(es,di);break;
   case 0x3c:byte imm=f();cmp=entry==0x748?al-imm:(sbyte)al-(sbyte)imm;break;
   case 0x75:sbyte nz=(sbyte)f();if(cmp!=0)return new CharacterDecisionResult(al,bx,entry+pc+nz==0x755);break;
   case 0x3a:if(f()!=0x16)throw new Exception("CMP byte");cmp=(sbyte)dl-(sbyte)m.ReadByte(ds,w());break;
   case 0x7f:sbyte gt=(sbyte)f();if(cmp>0)return new CharacterDecisionResult(al,bx,entry+pc+gt==0x755);break;
   case 0x7e:sbyte le=(sbyte)f();if(cmp<=0)return new CharacterDecisionResult(al,bx,entry+pc+le==0x7b3);break;
   case 0x8b:if(f()!=0x1e)throw new Exception("MOV BX");bx=m.ReadWord(ds,w());break;
   case 0x83:if(f()!=0xfb)throw new Exception("CMP BX");cmp=bx-f();break;
   case 0x74:sbyte eq=(sbyte)f();if(cmp==0)return new CharacterDecisionResult(al,bx,entry+pc+eq==0x7b3);break;
   default:throw new Exception("Opcode "+op);
  }}return new CharacterDecisionResult(al,bx,entry==0x748);
 }
 static void Check(byte[] code,Memory m,ushort ds,ushort es,ushort di,byte dl,ushort bx,int entry){
  m.Reads.Clear();CharacterDecisionResult a=entry==0x748?CharacterOutputDecisions.ProbeBackspace(m,ds,es,di,dl,bx):CharacterOutputDecisions.ProbeAlternateGate(m,ds,es,di,bx);long[] reads=m.Reads.ToArray();m.Reads.Clear();CharacterDecisionResult b=Interpret(code,m,ds,es,di,dl,bx,entry);
  if(a.AL!=b.AL||a.BX!=b.BX||a.Branch!=b.Branch||reads.Length!=m.Reads.Count)throw new Exception("Result/read extent");for(int i=0;i<reads.Length;i++)if(reads[i]!=m.Reads[i])throw new Exception("Read order");
 }
 static int Main(string[] args){try{Run(args);return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Run(string[] args){
  byte[] original=File.ReadAllBytes(args[0]),first=new byte[13],second=new byte[16];Array.Copy(original,0x16fa0+0x748,first,0,13);Array.Copy(original,0x16fa0+0x770,second,0,16);
  if(BitConverter.ToString(first)!="26-8A-05-3C-08-75-21-3A-16-98-4B-7F-1B"||BitConverter.ToString(second)!="26-8A-05-3C-1B-7E-3C-8B-1E-64-81-83-FB-00-74-33")throw new Exception("Original bytes");
  if(args.Length>1){string p=args[1];ushort ds=Convert.ToUInt16(args[2],16),es=Convert.ToUInt16(args[3],16),di=Convert.ToUInt16(args[4],16),bx=Convert.ToUInt16(args[6],16);byte dl=Convert.ToByte(args[5],16);int entry=Convert.ToInt32(args[7],16);Memory m=new Memory();Array.Copy(File.ReadAllBytes(Path.Combine(p,args[8]+"-ds.bin")),0,m.Bytes,ds<<4,65536);Array.Copy(File.ReadAllBytes(Path.Combine(p,args[8]+"-es.bin")),0,m.Bytes,es<<4,65536);CharacterDecisionResult a=entry==0x748?CharacterOutputDecisions.ProbeBackspace(m,ds,es,di,dl,bx):CharacterOutputDecisions.ProbeAlternateGate(m,ds,es,di,bx);if(a.AL!=Convert.ToByte(args[9],16)||a.BX!=Convert.ToUInt16(args[10],16)||a.Branch!=bool.Parse(args[11]))throw new Exception("Runtime result");foreach(string segment in new[]{"ds","es"}){byte[] before=File.ReadAllBytes(Path.Combine(p,args[8]+"-"+segment+".bin")),after=File.ReadAllBytes(Path.Combine(p,args[12]+"-"+segment+".bin"));for(int i=0;i<65536;i++)if(before[i]!=after[i])throw new Exception("Runtime memory");}Console.WriteLine("PASS original block "+entry.ToString("X4")+": AL/BX/branch and 131072 captured DS/ES bytes unchanged, zero exclusions.");return;}
  Memory mem=new Memory();int count=0;for(int layout=0;layout<4;layout++){
   ushort ds=0x2000,es=(ushort)(layout==0?0x3000:0x2000),di=new ushort[]{0x1234,0x4b98,0x8164,0xffff}[layout];
   for(int n=0;n<65536;n++)for(int k=0;k<2;k++){mem.Seed(ds,0x4b98,(byte)(n>>8));mem.Seed(es,di,(byte)(k==0?8:0xff));Check(first,mem,ds,es,di,(byte)n,(ushort)(n^0xa55a),0x748);count++;}
   for(int n=0;n<65536;n++)foreach(byte ch in new byte[]{0,27,28,255}){mem.Seed(ds,0x8164,(byte)n);mem.Seed(ds,0x8165,(byte)(n>>8));mem.Seed(es,di,ch);Check(second,mem,ds,es,di,0,(ushort)(n^0xa55a),0x770);count++;}
   for(int n=0;n<256;n++)foreach(ushort word in new ushort[]{0,1,0x8000,0xffff}){mem.Seed(ds,0x8164,(byte)word);mem.Seed(ds,0x8165,(byte)(word>>8));mem.Seed(es,di,(byte)n);Check(second,mem,ds,es,di,0,0x1234,0x770);count++;}
  }
  Console.WriteLine("PASS "+count+" original-instruction decision comparisons: signed-byte bounds, all flag words, all character bytes, conditional reads and four alias layouts.");Console.WriteLine("Scope: AL/BX/branch/read projection only; no flags, BIOS, return ABI or renderer.");
 }
}
