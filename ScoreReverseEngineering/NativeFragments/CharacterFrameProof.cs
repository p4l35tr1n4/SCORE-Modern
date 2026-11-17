using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class CharacterFrameProof {
 class Memory:ICharacterFrameMemory {
  public Dictionary<ushort,byte> Bytes=new Dictionary<ushort,byte>();public List<string> Events=new List<string>();
  public void Seed(ushort o,ushort v){Bytes[o]=(byte)v;Bytes[(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Bytes.TryGetValue(o,out a);Bytes.TryGetValue((ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add("R:"+s+":"+o+":"+v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add("W:"+s+":"+o+":"+v);Seed(o,v);}
  public Memory Copy(){Memory m=new Memory();foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string E(CharacterEntryWords r){return r.SP+":"+r.BP+":"+r.DI+":"+r.ES;}
 static string L(CharacterReturnWords r){return r.SP+":"+r.BP+":"+r.DI+":"+r.IP+":"+r.CS;}
 static CharacterEntryWords RefEnter(byte[] code,Memory m,ushort ss,ushort sp,ushort bp,ushort di){int pc=0;ushort es=0;while(pc<code.Length){switch(code[pc++]){case 0x55:sp=(ushort)(sp-2);m.WriteWord(ss,sp,bp);break;case 0x8b:if(code[pc++]!=0xec)throw new Exception("MOV BP");bp=sp;break;case 0x57:sp=(ushort)(sp-2);m.WriteWord(ss,sp,di);break;case 0xc4:if(code[pc++]!=0x7e)throw new Exception("LES");ushort at=(ushort)(bp+(sbyte)code[pc++]);di=m.ReadWord(ss,at);es=m.ReadWord(ss,(ushort)(at+2));break;default:throw new Exception("Entry opcode");}}return new CharacterEntryWords{SP=sp,BP=bp,DI=di,ES=es};}
 static CharacterReturnWords RefLeave(byte[] code,Memory m,ushort ss,ushort sp){int pc=0;ushort bp=0,di=0;while(pc<code.Length){switch(code[pc++]){case 0x5f:di=m.ReadWord(ss,sp);sp=(ushort)(sp+2);break;case 0x5d:bp=m.ReadWord(ss,sp);sp=(ushort)(sp+2);break;case 0xca:ushort purge=(ushort)(code[pc++]|code[pc++]<<8),ip=m.ReadWord(ss,sp);sp=(ushort)(sp+2);ushort cs=m.ReadWord(ss,sp);sp=(ushort)(sp+2+purge);return new CharacterReturnWords{SP=sp,BP=bp,DI=di,IP=ip,CS=cs};default:throw new Exception("Return opcode");}}throw new Exception("No RETF");}
 static void Same(Memory a,Memory b){if(a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Extent");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Order");foreach(var p in a.Bytes)if(b.Bytes[p.Key]!=p.Value)throw new Exception("Memory");}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static void Run(string[] args){byte[] original=File.ReadAllBytes(args[0]),entry=new byte[7],exit=new byte[5];Array.Copy(original,0x16fa0+0x73b,entry,0,7);Array.Copy(original,0x16fa0+0x7ba,exit,0,5);if(BitConverter.ToString(entry)!="55-8B-EC-57-C4-7E-06"||BitConverter.ToString(exit)!="5F-5D-CA-04-00")throw new Exception("Original code");
  if(args.Length>1){byte[] before=File.ReadAllBytes(args[2]),after=File.ReadAllBytes(args[3]);if(before.Length!=65536||after.Length!=65536)throw new Exception("Capture length");Memory m=new Memory();for(int i=0;i<65536;i++)m.Bytes[(ushort)i]=before[i];ushort ss=H(args[4]),sp=H(args[5]);if(args[1]=="entry"){CharacterEntryWords r=CharacterOutputFrame.Enter(m,ss,sp,H(args[6]),H(args[7]));if(E(r)!=E(new CharacterEntryWords{SP=H(args[8]),BP=H(args[9]),DI=H(args[10]),ES=H(args[11])}))throw new Exception("Entry words");}else{CharacterReturnWords r=CharacterOutputFrame.Leave(m,ss,sp);if(L(r)!=L(new CharacterReturnWords{SP=H(args[6]),BP=H(args[7]),DI=H(args[8]),IP=H(args[9]),CS=H(args[10])}))throw new Exception("Return words");}for(int i=0;i<65536;i++)if(m.Bytes[(ushort)i]!=after[i])throw new Exception("Runtime memory "+i.ToString("X4"));Console.WriteLine("PASS original "+args[1]+" frame: projected words and all 65536 stack bytes, zero exclusions.");return;}
  int count=0;for(int n=0;n<65536;n++)for(int layout=0;layout<4;layout++){ushort ss=0x871e,sp=new ushort[]{(ushort)n,0,0xfffa,0xe230}[layout],bp=(ushort)(n^0xa55a),di=(ushort)(n*13+7);Memory seed=new Memory();for(int k=-4;k<12;k+=2)seed.Seed((ushort)(sp+k),(ushort)(n+k*777));Memory a=seed.Copy(),b=seed.Copy();if(E(CharacterOutputFrame.Enter(a,ss,sp,bp,di))!=E(RefEnter(entry,b,ss,sp,bp,di)))throw new Exception("Entry");Same(a,b);count++;a=seed.Copy();b=seed.Copy();if(L(CharacterOutputFrame.Leave(a,ss,sp))!=L(RefLeave(exit,b,ss,sp)))throw new Exception("Return");Same(a,b);count++;}
  Console.WriteLine("PASS "+count+" original-instruction entry/return frame comparisons: word patterns, all SP offsets, offset wrap, saved words, far-word order and RETF purge.");Console.WriteLine("Scope: explicit word/memory projection; no hardware address-limit exceptions or complete CPU/flags/interrupt ABI.");
 }
 static int Main(string[] args){try{Run(args);return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
