using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class CharacterRoutineProof {
 class Context:ICharacterRoutineContext {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();public int Seed,Pattern,Calls;
  public void SeedByte(ushort s,ushort o,byte v){Bytes[(s<<4)+o]=v;}
  public void SeedWord(ushort s,ushort o,ushort v){SeedByte(s,o,(byte)v);SeedByte(s,(ushort)(o+1),(byte)(v>>8));}
  public byte ReadByte(ushort s,ushort o){byte v;Bytes.TryGetValue((s<<4)+o,out v);Events.Add("B:"+s+":"+o+":"+v);return v;}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Bytes.TryGetValue((s<<4)+o,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add("R:"+s+":"+o+":"+v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add("W:"+s+":"+o+":"+v);SeedWord(s,o,v);}
  public Context Copy(){Context c=new Context{Seed=Seed,Pattern=Pattern};foreach(var p in Bytes)c.Bytes[p.Key]=p.Value;return c;}
  public void Interrupt10(ref CharacterRoutineWords w,ushort ret){Events.Add("I:"+ret+":"+Show(w));int c=Calls++;if(Pattern==0){if((w.AX>>8)==3){w.DX=(ushort)Seed;w.CX=(ushort)(Seed^0xa55a);}return;}
   if(Pattern>=2)WriteWord(w.SS,w.SP,(ushort)(Seed+c*31));
   w.AX=(ushort)(Seed+c*777);w.BX=(ushort)(Seed^c*257);w.CX=(ushort)(Seed+c*131);w.DX=(ushort)(Seed+c*17);
   if(Pattern==3){w.SS=(ushort)(0x2000+c);w.SP=(ushort)(Seed-c*2);w.DS=(ushort)(0x2100+c);w.ES=(ushort)(0x2200+c);w.DI=(ushort)(Seed+c);w.BP=(ushort)(Seed^c);w.SI=(ushort)(Seed-c);}
  }
 }
 static string Show(CharacterRoutineWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP+":"+w.DS+":"+w.ES+":"+w.SS+":"+w.SP;}
 static CharacterRoutineResult Interpret(byte[] code,Context m,CharacterRoutineWords w){int pc=0,cmp=0,steps=0;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<200){byte op=f();switch(op){
  case 0x55:w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,w.BP);break;case 0x57:w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,w.DI);break;case 0x50:w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,w.AX);break;
  case 0x58:w.AX=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);break;case 0x5f:w.DI=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);break;case 0x5d:w.BP=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);break;
  case 0x8b:byte mod=f();if(mod==0xec)w.BP=w.SP;else if(mod==0x1e)w.BX=m.ReadWord(w.DS,word());else throw new Exception("MOV");break;
  case 0xc4:if(f()!=0x7e)throw new Exception("LES");ushort at=(ushort)(w.BP+(sbyte)f());ushort off=m.ReadWord(w.SS,at),seg=m.ReadWord(w.SS,(ushort)(at+2));w.DI=off;w.ES=seg;break;
  case 0x26:if(f()!=0x8a||f()!=5)throw new Exception("ES byte");w.AX=(ushort)((w.AX&0xff00)|m.ReadByte(w.ES,w.DI));break;
  case 0x8a:if(f()!=0x16)throw new Exception("DL byte");w.DX=(ushort)((w.DX&0xff00)|m.ReadByte(w.DS,word()));break;
  case 0x3c:cmp=(sbyte)(byte)w.AX-(sbyte)f();break;
  case 0x3a:if(f()!=0x16)throw new Exception("CMP DL");cmp=(sbyte)(byte)w.DX-(sbyte)m.ReadByte(w.DS,word());break;
  case 0x83:if(f()!=0xfb)throw new Exception("CMP BX");cmp=w.BX-f();break;
  case 0x75:sbyte nz=(sbyte)f();if(cmp!=0)pc+=nz;break;case 0x74:sbyte eq=(sbyte)f();if(cmp==0)pc+=eq;break;case 0x7f:sbyte gt=(sbyte)f();if(cmp>0)pc+=gt;break;case 0x7e:sbyte le=(sbyte)f();if(cmp<=0)pc+=le;break;case 0xeb:sbyte d=(sbyte)f();pc+=d;break;
  case 0xfe:byte inc=f();if(inc==0xce)w.DX=(ushort)((w.DX&255)|((byte)((w.DX>>8)-1)<<8));else if(inc==0xc2)w.DX=(ushort)((w.DX&0xff00)|(byte)((w.DX&255)+1));else throw new Exception("INC/DEC");break;
  case 0xb8:w.AX=word();break;case 0xb9:w.CX=word();break;case 0xbb:w.BX=word();break;case 0xb4:w.AX=(ushort)((w.AX&255)|f()<<8);break;case 0xb0:w.AX=(ushort)((w.AX&0xff00)|f());break;case 0xb7:w.BX=(ushort)((w.BX&255)|f()<<8);break;
  case 0xcd:if(f()!=0x10)throw new Exception("INT");m.Interrupt10(ref w,(ushort)(0x73b+pc));break;
  case 0xca:ushort purge=word(),ip=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);ushort cs=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2+purge);return new CharacterRoutineResult{Words=w,IP=ip,CS=cs};
  default:throw new Exception("Opcode "+op.ToString("X2"));
 }}throw new Exception("No return");}
 static void Check(byte[] code,int seed,int pattern,byte ch,ushort flag,byte bound,ushort sp,ushort es,ushort di){CharacterRoutineWords w=new CharacterRoutineWords{AX=(ushort)(seed^0x1357),BX=(ushort)seed,CX=0x7777,DX=0x5555,SI=0x1234,DI=0x7e9a,BP=0xe256,DS=0x2000,ES=0x3000,SS=0x2000,SP=sp};Context initial=new Context{Seed=seed,Pattern=pattern};initial.SeedWord(w.SS,sp,0x2468);initial.SeedWord(w.SS,(ushort)(sp+2),0x1357);initial.SeedWord(w.SS,(ushort)(sp+4),di);initial.SeedWord(w.SS,(ushort)(sp+6),es);initial.SeedWord(w.DS,0x8164,flag);initial.SeedByte(w.DS,0x4b98,bound);initial.SeedByte(w.DS,0x4b9a,0x4f);initial.SeedByte(es,di,ch);Context a=initial.Copy(),b=initial.Copy();CharacterRoutineResult x=CharacterOutputRoutine.Run(a,w),y=Interpret(code,b,w);if(Show(x.Words)!=Show(y.Words)||x.IP!=y.IP||x.CS!=y.CS||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Result/extent");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Effect "+i+" seed "+seed+" "+a.Events[i]+" != "+b.Events[i]);foreach(var p in a.Bytes)if(b.Bytes[p.Key]!=p.Value)throw new Exception("Memory");}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[132];Array.Copy(raw,0x16fa0+0x73b,code,0,132);int count=0;for(int ch=0;ch<256;ch++)foreach(ushort flag in new ushort[]{0,1,0x8000,0xffff})foreach(int pattern in new int[]{0,1,2,3})foreach(ushort di in new ushort[]{0xac5e,0x8164}){Check(code,ch*257,pattern,(byte)ch,flag,(byte)(ch^0x80),0xe230,0x2000,di);count++;}for(int n=0;n<65536;n++){Check(code,n,0,8,0,(byte)(n>>8),(ushort)n,0x2000,0xac5e);count++;Check(code,n,2,0x20,1,0,(ushort)n,0x2000,0xac5e);count++;}Console.WriteLine("PASS "+count+" composed-routine original-instruction comparisons: all character bytes, four flag words, signed cursor pairs, all SP offsets, memory aliasing and explicit BIOS effect doubles.");Console.WriteLine("Scope: word/memory/control projection with external BIOS; complete CPU flags, hardware interrupts/exceptions and native rendering remain outside.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
