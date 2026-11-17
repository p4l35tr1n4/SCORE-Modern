using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class StringOutputProof {
 class Context:IStringOutputContext {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();public int Pattern,Seed,Calls;
  public void SeedByte(ushort s,ushort o,byte v){Bytes[(s<<4)+o]=v;}
  public void SeedWord(ushort s,ushort o,ushort v){SeedByte(s,o,(byte)v);SeedByte(s,(ushort)(o+1),(byte)(v>>8));}
  public byte ReadByte(ushort s,ushort o){byte v;Bytes.TryGetValue((s<<4)+o,out v);Events.Add("B:"+s+":"+o+":"+v);return v;}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Bytes.TryGetValue((s<<4)+o,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add("R:"+s+":"+o+":"+v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add("W:"+s+":"+o+":"+v);SeedWord(s,o,v);}
  public Context Copy(){Context c=new Context{Pattern=Pattern,Seed=Seed};foreach(var p in Bytes)c.Bytes[p.Key]=p.Value;return c;}
  public CharacterRoutineResult CallCharacter(CharacterRoutineWords w,ushort targetSegment,ushort targetOffset){Events.Add("C:"+targetSegment+":"+targetOffset+":"+Show(w));ushort ip=ReadWord(w.SS,w.SP),cs=ReadWord(w.SS,(ushort)(w.SP+2)),off=ReadWord(w.SS,(ushort)(w.SP+4)),seg=ReadWord(w.SS,(ushort)(w.SP+6));byte ch=ReadByte(seg,off);w.SP=(ushort)(w.SP+8);w.AX=(ushort)((Seed&0xff00)|ch);w.DX=(ushort)(Seed+Calls++);
   if(Pattern==1){w.SI=(ushort)(Seed^0xa55a);w.DS=(ushort)(Seed^0x1357);w.BP=(ushort)Seed;w.ES=0x4000;w.DI=0xfffe;}
   if(Pattern==2){SeedByte(seg,(ushort)(off+1),0);Events.Add("external-byte-clear:"+seg+":"+(ushort)(off+1));}
   if(Pattern==3){cs^=1;ip=0x70c;}
   return new CharacterRoutineResult{Words=w,IP=ip,CS=cs};
  }
 }
 static string Show(CharacterRoutineWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP+":"+w.DS+":"+w.ES+":"+w.SS+":"+w.SP;}
 static StringOutputResult Interpret(byte[] code,Context m,CharacterRoutineWords w,ushort currentSegment){int pc=0,cmp=0,steps=0;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<10000){switch(f()){
  case 0x55:w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,w.BP);break;case 0x8b:if(f()!=0xec)throw new Exception("MOV BP");w.BP=w.SP;break;case 0x57:w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,w.DI);break;
  case 0xc4:if(f()!=0x7e)throw new Exception("LES");ushort a=(ushort)(w.BP+(sbyte)f()),off=m.ReadWord(w.SS,a),seg=m.ReadWord(w.SS,(ushort)(a+2));w.DI=off;w.ES=seg;break;
  case 0x26:if(f()!=0x8a||f()!=5)throw new Exception("ES byte");w.AX=(ushort)((w.AX&0xff00)|m.ReadByte(w.ES,w.DI));break;
  case 0x3c:cmp=(byte)w.AX-f();break;case 0x74:sbyte eq=(sbyte)f();if(cmp==0)pc+=eq;break;case 0xeb:sbyte j=(sbyte)f();pc+=j;break;
  case 0x06:w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,w.ES);break;
  case 0x9a:ushort target=word(),targetSeg=word();w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,currentSegment);w.SP=(ushort)(w.SP-2);m.WriteWord(w.SS,w.SP,(ushort)(0x6f6+pc));CharacterRoutineResult c=m.CallCharacter(w,targetSeg,target);w=c.Words;if(c.CS!=currentSegment||c.IP!=0x6f6+pc)return new StringOutputResult{Result=c,ReachedStringReturn=false};break;
  case 0x47:w.DI=(ushort)(w.DI+1);break;case 0x5f:w.DI=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);break;case 0x5d:w.BP=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);break;
  case 0xca:ushort purge=word(),ip=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);ushort cs=m.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2+purge);return new StringOutputResult{Result=new CharacterRoutineResult{Words=w,IP=ip,CS=cs},ReachedStringReturn=true};
  default:throw new Exception("Opcode");
 }}throw new Exception("No bounded model exit");}
 static void Check(byte[] code,int n,int pattern,int layout){ushort sp=(ushort)n,ss=0x2000,seg=(ushort)(layout==0?0x3000:0x2000),off=new ushort[]{0xfffc,0xac5e,(ushort)(sp-4),(ushort)(sp+4)}[layout];CharacterRoutineWords w=new CharacterRoutineWords{AX=(ushort)(n^0xa55a),BX=(ushort)n,CX=0x7777,DX=0x5555,SI=0x1234,DI=0x7e9a,BP=0xe256,DS=0x2000,ES=0x4000,SS=ss,SP=sp};Context seed=new Context{Seed=n,Pattern=pattern};seed.SeedWord(ss,sp,0x2468);seed.SeedWord(ss,(ushort)(sp+2),0x1357);seed.SeedWord(ss,(ushort)(sp+4),off);seed.SeedWord(ss,(ushort)(sp+6),seg);int length=n%7;for(int k=0;k<length;k++)seed.SeedByte(seg,(ushort)(off+k),(byte)(1+(n+k)%255));seed.SeedByte(seg,(ushort)(off+length),0);Context a=seed.Copy(),b=seed.Copy();StringOutputResult x=StringOutputRoutine.Run(a,w,0x16fa,0x16fa),y=Interpret(code,b,w,0x16fa);if(x.ReachedStringReturn!=y.ReachedStringReturn||x.Result.CS!=y.Result.CS||x.Result.IP!=y.Result.IP||Show(x.Result.Words)!=Show(y.Result.Words)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Result/extent");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effects "+i);foreach(var p in a.Bytes)if(b.Bytes[p.Key]!=p.Value)throw new Exception("Memory");}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[29];Array.Copy(raw,0x16fa0+0x6f6,code,0,29);if(BitConverter.ToString(code)!="55-8B-EC-57-C4-7E-06-26-8A-05-3C-00-74-0A-06-57-9A-3B-07-FA-16-47-EB-EF-5F-5D-CA-04-00")throw new Exception("Original bytes");int count=0;for(int n=0;n<65536;n++)for(int pattern=0;pattern<4;pattern++){Check(code,n,pattern,0);count++;}for(int n=0;n<256;n++)for(int pattern=0;pattern<4;pattern++)for(int layout=1;layout<4;layout++){Check(code,n,pattern,layout);count++;}Console.WriteLine("PASS "+count+" original-instruction string-loop comparisons: empty/short byte sequences, all SP offsets, source offset wrap, aliases, four explicit callee effects including changed registers/source and unexpected far returns.");Console.WriteLine("Scope: word/memory/control projection with external character callee; no text encoding, complete CPU flags/interrupt ABI or rendering.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
