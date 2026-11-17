using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class LinePreparationProof {
 class Memory:IHorizontalRasterMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(LinePreparationWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.DI+":"+w.ES;}
 static LinePreparationWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort ax){int pc=0;ushort bx=0,cx=0,dx=0,di=0,es=0;bool sf=false,of=false;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<64){switch(f()){
 case 0x8b:byte mov=f();if(mov==0x0e)cx=m.ReadWord(ds,word());else if(mov==0x16)dx=m.ReadWord(ds,word());else if(mov==0x3e)di=m.ReadWord(ds,word());else if(mov==0xdf)bx=di;else throw new System.Exception("MOV");break;
 case 0x89:byte store=f();ushort at=word();if(store==0x0e)m.WriteWord(ds,at,cx);else if(store==0x16)m.WriteWord(ds,at,dx);else if(store==0x3e)m.WriteWord(ds,at,di);else throw new System.Exception("Store");break;
 case 0xa3:m.WriteWord(ds,word(),ax);break;
 case 0xc7:if(f()!=6)throw new System.Exception("Immediate");ushort addr=word();m.WriteWord(ds,addr,word());break;
 case 0x8e:if(f()!=0xc7)throw new System.Exception("ES");es=di;break;
 case 0x2b:if(f()!=0xd8)throw new System.Exception("SUB");ushort a=bx;bx=(ushort)(bx-ax);sf=(bx&0x8000)!=0;of=((a^ax)&(a^bx)&0x8000)!=0;break;
 case 0x7d:sbyte delta=(sbyte)f();if(sf==of)pc+=delta;break;
 case 0xf7:if(f()!=0xdb)throw new System.Exception("NEG");bx=(ushort)(-bx);break;
 case 0x87:byte swap=f();if(swap==6){ushort o=word(),v=m.ReadWord(ds,o);m.WriteWord(ds,o,ax);ax=v;}else if(swap==0xca){ushort v=dx;dx=cx;cx=v;}else throw new System.Exception("XCHG");break;
 default:throw new System.Exception("Opcode");
 }}if(pc!=64)throw new System.Exception("Boundary");return new LinePreparationWords{AX=ax,BX=bx,CX=cx,DX=dx,DI=di,ES=es};}
 class Captured:IHorizontalRasterMemory {public byte[] Data;public ushort DS;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){throw new Exception("Unexpected video effect");}}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[64];Array.Copy(raw,0x18070+0x74c,code,0,64);
 if(args.Length>1){string p=args[1],prefix=Path.Combine(p,"case-"+args[2]);byte[] live=File.ReadAllBytes(prefix+"-code.bin");if(live.Length!=64)throw new Exception("Live length");for(int i=0;i<64;i++)if(live[i]!=code[i])throw new Exception("Live code");Captured m=new Captured{DS=H(args[3]),Data=File.ReadAllBytes(prefix+"-before-ds.bin")};var r=LinePreparation.Execute(m,m.DS,H(args[4]));var e=new LinePreparationWords{AX=H(args[5]),BX=H(args[6]),CX=H(args[7]),DX=H(args[8]),DI=H(args[9]),ES=H(args[10])};if(Show(r)!=Show(e))throw new Exception("Runtime words "+Show(r)+" != "+Show(e));byte[] after=File.ReadAllBytes(prefix+"-after-ds.bin");if(m.Data.Length!=65536||after.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("DS "+i);Console.WriteLine("PASS original line preparation case "+args[2]+": 64 live bytes, six result words and all 65536 DS bytes.");return 0;}
 int count=0;ushort ds=0x2000;for(int y=0;y<65536;y++)for(int k=0;k<4;k++){ushort[] inputs={0,0x7fff,0x8000,0xffff};ushort ax=inputs[k];Memory seed=new Memory();seed.Seed(ds,0xac40,(ushort)(y^0x1234));seed.Seed(ds,0xac44,(ushort)(y^0xabcd));seed.Seed(ds,0xac46,(ushort)y);seed.Seed(ds,0x4a26,(ushort)(y*13));Memory a=seed.Copy(),b=seed.Copy();var n=LinePreparation.Execute(a,ds,ax);var q=Interpret(code,b,ds,ax);if(Show(n)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model state");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effect");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Memory");count++;}
 Console.WriteLine("PASS "+count+" instruction-model comparisons: all old-Y words with four signed boundary new-Y words; both branches, subtraction overflow, ordered reads/writes and six result words.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
