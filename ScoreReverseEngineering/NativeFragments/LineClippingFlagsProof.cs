using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class LineClippingFlagsProof {
 class Memory:IHorizontalRasterMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(LineClippingWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.Exit;}
 public static HashSet<int> Visited=new HashSet<int>();
 static LineClippingFlagResult Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di,uint inputFlags){uint fullFlags=inputFlags;ushort[] r={ax,cx,dx,bx,0,0,si,di};int pc=0x78c,steps=0;bool sf=false,of=false,z=false;Func<byte> f=()=>code[pc++-0x78c];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};Action<ushort,ushort,bool> flags=(a,b,add)=>{ushort v=add?(ushort)(a+b):(ushort)(a-b);sf=(v&0x8000)!=0;z=v==0;of=(((add?~(a^b):(a^b))&(a^v)&0x8000)!=0);
 uint bits=0;if(add?(uint)a+b>65535:a<b)bits|=1;int ones=0;for(int bit=0;bit<8;bit++)if((v&(1<<bit))!=0)ones++;if((ones&1)==0)bits|=4;if(((a^b^v)&16)!=0)bits|=16;if(z)bits|=64;if(sf)bits|=128;if(of)bits|=2048;fullFlags=(fullFlags&~0x8d5u)|bits;};
 while(pc>=0x78c&&pc<0x981&&steps++<2000000){Visited.Add(pc);byte op=f();switch(op){
 case 0x8b:case 0x89:case 0x3b:case 0x39:case 0x03:case 0x2b:case 0x87:{byte mod=f();int reg=(mod>>3)&7,rm=mod&7;bool mem=(mod&0xc0)==0;if(mem&&rm!=6)throw new Exception("Addressing");if(!mem&&(mod&0xc0)!=0xc0)throw new Exception("Mode");ushort at=mem?word():(ushort)0;Func<ushort> read=()=>mem?m.ReadWord(ds,at):r[rm];Action<ushort> write=v=>{if(mem)m.WriteWord(ds,at,v);else r[rm]=v;};ushort a,b;
 if(op==0x8b)r[reg]=read();else if(op==0x89)write(r[reg]);else if(op==0x87){a=read();write(r[reg]);r[reg]=a;}else if(op==0x39){a=read();flags(a,r[reg],false);}else if(op==0x3b){a=read();flags(r[reg],a,false);}else {a=r[reg];b=read();flags(a,b,op==3);r[reg]=op==3?(ushort)(a+b):(ushort)(a-b);}break;}
 case 0xa1:r[0]=m.ReadWord(ds,word());break;case 0xa3:m.WriteWord(ds,word(),r[0]);break;
 case 0x83:{byte v=f();if(v!=0xfb)throw new Exception("Immediate CMP");flags(r[3],(ushort)(short)(sbyte)f(),false);break;}
 case 0xf7:{byte v=f();int reg=v&7,group=(v>>3)&7;if((v&0xc0)!=0xc0)throw new Exception("F7 mode");if(group==3){ushort a=r[reg];flags(0,a,false);r[reg]=(ushort)(-a);}else if(group==5){int p=(short)r[0]*(short)r[reg];r[0]=(ushort)p;r[2]=(ushort)(p>>16);}else if(group==7){int n=unchecked((int)(((uint)r[2]<<16)|r[0])),d=(short)r[reg];if(d==0||(n==int.MinValue&&d==-1))throw new NotSupportedException("IDIV");int q=n/d;if(q<short.MinValue||q>short.MaxValue)throw new NotSupportedException("IDIV");r[0]=(ushort)q;r[2]=(ushort)(n%d);}else throw new Exception("F7 group");break;}
 case 0x7d:case 0x7c:case 0x7e:case 0x7f:case 0x74:case 0x75:{sbyte d=(sbyte)f();bool branch=op==0x7d?sf==of:op==0x7c?sf!=of:op==0x7e?z||sf!=of:op==0x7f?!z&&sf==of:op==0x74?z:!z;if(branch)pc+=d;break;}
 case 0xeb:{sbyte d=(sbyte)f();pc+=d;break;}case 0xe9:{short d=(short)word();pc+=d;break;}
 case 0xea:{ushort target=word(),seg=word();if(seg!=0x1807)throw new Exception("Far segment");pc=target;break;}
 default:throw new Exception("Opcode "+op.ToString("X2")+" at "+(pc-1).ToString("X4"));
 }}if(pc!=0x981&&pc!=0xa96&&pc!=0xac6&&pc!=0xb16)throw new Exception("Exit "+pc.ToString("X4"));return new LineClippingFlagResult{Words=new LineClippingWords{AX=r[0],BX=r[3],CX=r[1],DX=r[2],SI=r[6],DI=r[7],Exit=(ushort)pc},EFlags=fullFlags};}
 class Captured:IHorizontalRasterMemory{public byte[] Data;public ushort DS;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte b){throw new Exception("Video effect");}}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[501];Array.Copy(raw,0x18070+0x78c,code,0,501);
 if(args.Length>1){string p=Path.Combine(args[1],"case-"+args[2]);Captured m=new Captured{DS=H(args[3]),Data=File.ReadAllBytes(p+"-before-ds.bin")};var r=LineClipping.ExecuteWithFlags(m,m.DS,H(args[4]),H(args[5]),H(args[6]),H(args[7]),H(args[8]),H(args[9]),Convert.ToUInt32(args[10],16));uint expected=Convert.ToUInt32(args[11],16);if(r.EFlags!=expected)throw new Exception("Flags "+r.EFlags.ToString("X8")+" != "+expected.ToString("X8"));Console.WriteLine("PASS clipping full EFLAGS: "+expected.ToString("X8"));return 0;}
 Random random=new Random(1807);int count=0;for(int n=0;n<100000;n++){ushort ds=0x2000;int x1=random.Next(-1000,1001),x2=random.Next(-1000,1001),y1=random.Next(-1000,1001),y2=random.Next(-1000,1001);if(n%4==0)x2=x1;if(n%4==1)y2=y1;if(y1>y2){int t=y1;y1=y2;y2=t;t=x1;x1=x2;x2=t;}Memory seed=new Memory();seed.Seed(ds,0x4b68,(ushort)x1);seed.Seed(ds,0x4b6c,(ushort)x2);seed.Seed(ds,0x4b6a,(ushort)y1);seed.Seed(ds,0x4b6e,(ushort)y2);seed.Seed(ds,0x4a72,1);seed.Seed(ds,0x4a76,97);seed.Seed(ds,0x4a7a,461);seed.Seed(ds,0x4a7c,462);seed.Seed(ds,0x4a7e,638);seed.Seed(ds,0x4a80,639);ushort ax=(ushort)y1,bx=(ushort)(y2-y1),cx=(ushort)x1,dx=(ushort)x2,si=(ushort)n,di=(ushort)(n*7);uint flags=(uint)random.Next()|2;Memory a=seed.Copy(),b=seed.Copy();var p=LineClipping.ExecuteWithFlags(a,ds,ax,bx,cx,dx,si,di,flags);var q=Interpret(code,b,ds,ax,bx,cx,dx,si,di,flags);if(Show(p.Words)!=Show(q.Words)||p.EFlags!=q.EFlags||a.Events.Count!=b.Events.Count)throw new Exception("Model flags case "+n+" "+p.EFlags.ToString("X8")+" != "+q.EFlags.ToString("X8"));for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Events");count++;}
 Console.WriteLine("PASS "+count+" instruction-model full-EFLAGS and word comparisons, varying preserved input bits. Intermediate undefined IMUL/IDIV flags do not reach normal exits.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
