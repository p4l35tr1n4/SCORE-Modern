using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class CharacterAlternateProof {
 class Context:ICharacterBiosContext {
  public Dictionary<int,byte> Memory=new Dictionary<int,byte>();public List<string> Events=new List<string>();public int Pattern,Call,Seed;
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add("W:"+s+":"+o+":"+v);Memory[(s<<4)+o]=(byte)v;Memory[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Memory.TryGetValue((s<<4)+o,out a);Memory.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add("R:"+s+":"+o+":"+v);return v;}
  public void Interrupt10(ref CharacterBiosWords w){Events.Add("I:"+Show(w));int c=Call++;
   if(Pattern==0){if(c==3){w.CX=(ushort)(Seed^0xa55a);w.DX=(ushort)Seed;}return;}
   if(Pattern>=2)WriteWord(w.SS,w.SP,(ushort)(Seed+c*31));
   w.AX=(ushort)(Seed+c*777);w.BX=(ushort)(Seed^c*257);w.CX=(ushort)(Seed+c*131);w.DX=(ushort)(Seed+c*17);
   if(Pattern==3){w.SS=(ushort)(0x2000+c);w.SP=(ushort)(Seed-c*2);}
  }
 }
 static string Show(CharacterBiosWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SS+":"+w.SP;}
 static CharacterBiosWords Interpret(byte[] code,Context c,CharacterBiosWords w){int pc=0;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length){byte op=f();switch(op){
  case 0x50:w.SP=(ushort)(w.SP-2);c.WriteWord(w.SS,w.SP,w.AX);break;
  case 0x58:w.AX=c.ReadWord(w.SS,w.SP);w.SP=(ushort)(w.SP+2);break;
  case 0xb8:w.AX=word();break;case 0xb9:w.CX=word();break;case 0xbb:w.BX=word();break;
  case 0xb4:w.AX=(ushort)((w.AX&255)|f()<<8);break;case 0xb7:w.BX=(ushort)((w.BX&255)|f()<<8);break;
  case 0xcd:if(f()!=0x10)throw new Exception("Interrupt");c.Interrupt10(ref w);break;
  case 0xfe:if(f()!=0xc2)throw new Exception("INC");w.DX=(ushort)((w.DX&0xff00)|(byte)((w.DX&255)+1));break;
  case 0xeb:int d=(sbyte)f();if(0x780+pc+d!=0x7ba)throw new Exception("End target");return w;
  default:throw new Exception("Opcode "+op);
 }}throw new Exception("No branch exit");}
 static void Run(string[] args){byte[] original=File.ReadAllBytes(args[0]),code=new byte[50];Array.Copy(original,0x16fa0+0x780,code,0,50);if(code[0]!=0x50||code[48]!=0xeb||code[49]!=8)throw new Exception("Code boundary");int count=0;
  for(int n=0;n<65536;n++)for(int pattern=0;pattern<4;pattern++){
   CharacterBiosWords w=new CharacterBiosWords{AX=(ushort)n,BX=(ushort)(n^0xa55a),CX=(ushort)(n*13),DX=(ushort)n,SS=0x2000,SP=(ushort)n};Context a=new Context{Seed=n,Pattern=pattern},b=new Context{Seed=n,Pattern=pattern};CharacterBiosWords x=CharacterAlternateOutput.Run(a,w),y=Interpret(code,b,w);
   if(Show(x)!=Show(y)||a.Events.Count!=b.Events.Count||a.Memory.Count!=b.Memory.Count||a.Call!=5)throw new Exception("Result/extent");for(int k=0;k<a.Events.Count;k++)if(a.Events[k]!=b.Events[k])throw new Exception("Ordered effect");foreach(var kv in a.Memory)if(b.Memory[kv.Key]!=kv.Value)throw new Exception("Memory");count++;
  }
  Console.WriteLine("PASS "+count+" original-instruction alternate-output comparisons: every AX/DX/SP word, four explicit BIOS effect doubles, stack word replacement/aliasing, ordered calls and DL wrap.");Console.WriteLine("Scope: AX/BX/CX/DX/SS/SP and explicit memory effects only. BIOS effects are test doubles; no native rendering, complete flags, interrupt-frame or epilogue ABI.");
 }
 static int Main(string[] args){try{Run(args);return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
