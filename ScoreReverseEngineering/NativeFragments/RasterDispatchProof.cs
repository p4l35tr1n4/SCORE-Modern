using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class RasterDispatchProof {
 class Memory:IHorizontalRasterMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(RasterDispatchWords s){return s.BX+":"+s.CX+":"+s.DX+":"+s.Exit+":"+s.EFlags;}
 static RasterDispatchWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort bx,ushort cx,ushort dx,uint ef){int pc=0;bool less=false,equal=false;System.Action<ushort,ushort> cmp=(a,b)=>{int difference=(short)a-(short)b;less=difference<0;equal=a==b;ushort r=(ushort)(a-b);uint f=0;if(a<b)f|=1;if(((a^b^r)&16)!=0)f|=16;if(r==0)f|=64;if((r&0x8000)!=0)f|=128;if(((a^b)&(a^r)&0x8000)!=0)f|=2048;int bits=0;for(int i=0;i<8;i++)bits+=(r>>i)&1;if(bits%2==0)f|=4;ef=(ef&~0x8d5u)|f;};System.Func<byte> read=()=>code[pc++];System.Func<ushort> word=()=>{byte l=read();return (ushort)(l|read()<<8);};while(pc<55){switch(read()){
 case 0x3b:byte comp=read();if(comp==0xd3)cmp(dx,bx);else if(comp==0x0e)cmp(cx,m.ReadWord(ds,word()));else throw new System.Exception("CMP");break;
 case 0x75:sbyte jne=(sbyte)read();if(!equal)pc+=jne;break;case 0x7c:sbyte jl=(sbyte)read();if(less)pc+=jl;break;
 case 0xea:if(word()!=0xa3d||word()!=0x1807)throw new System.Exception("Far exit");return new RasterDispatchWords{BX=bx,CX=cx,DX=dx,Exit=0xa3d,EFlags=ef};
 case 0x89:byte store=read();ushort at=word();if(store==0x16)m.WriteWord(ds,at,dx);else if(store==0x0e)m.WriteWord(ds,at,cx);else if(store==0x1e)m.WriteWord(ds,at,bx);else throw new System.Exception("Store");break;
 case 0xd1:if(read()!=0xea)throw new System.Exception("SHR");dx=(ushort)(dx>>1);break;
 case 0x8b:byte load=read();ushort o=word();if(load==0x1e)bx=m.ReadWord(ds,o);else if(load==0x0e)cx=m.ReadWord(ds,o);else throw new System.Exception("Load");break;
 case 0x87:byte sw=read();ushort offset=word(),old=m.ReadWord(ds,offset);if(sw==0x0e){m.WriteWord(ds,offset,cx);cx=old;}else if(sw==0x1e){m.WriteWord(ds,offset,bx);bx=old;}else throw new System.Exception("Exchange");break;
 case 0xc7:if(read()!=6)throw new System.Exception("Immediate");ushort dst=word();m.WriteWord(ds,dst,word());break;
 default:throw new System.Exception("Opcode");
 }}if(pc!=55)throw new System.Exception("Boundary");return new RasterDispatchWords{BX=bx,CX=cx,DX=dx,Exit=0x9b8,EFlags=ef};}
 class Captured:IHorizontalRasterMemory {public byte[] Data;public ushort DS;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){throw new Exception("Unexpected video effect");}}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[55];Array.Copy(raw,0x18070+0x981,code,0,55);
 if(args.Length>1){Captured m=new Captured{DS=H(args[3]),Data=File.ReadAllBytes(args[1])};var a=RasterDispatch.Execute(m,m.DS,H(args[4]),H(args[5]),H(args[6]),Convert.ToUInt32(args[7],16));var b=new RasterDispatchWords{BX=H(args[8]),CX=H(args[9]),DX=H(args[10]),Exit=H(args[11]),EFlags=Convert.ToUInt32(args[12],16)};if(Show(a)!=Show(b))throw new Exception("Runtime words/flags "+Show(a)+" != "+Show(b));byte[] after=File.ReadAllBytes(args[2]);if(m.Data.Length!=65536||after.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("DS");Console.WriteLine("PASS original dispatch: BX/CX/DX, full EFLAGS, exit and all DS bytes.");return 0;}
 int count=0;ushort ds=0x2000;for(int n=0;n<65536;n++)for(int k=0;k<4;k++){ushort dx=(ushort)n,bx=(ushort)(k==0?n:n^0xaaaa),cx=(ushort)(n*17),left=new ushort[]{0,0x7fff,0x8000,0xffff}[k];Memory seed=new Memory();seed.Seed(ds,0x4b68,left);seed.Seed(ds,0x4b6c,(ushort)n);seed.Seed(ds,0x4b6a,(ushort)(n^0x7654));seed.Seed(ds,0x4b6e,(ushort)(n^0xabcd));Memory a=seed.Copy(),b=seed.Copy();uint ef=unchecked((uint)(n*65537+0xa55a));var p=RasterDispatch.Execute(a,ds,bx,cx,dx,ef);var q=Interpret(code,b,ds,bx,cx,dx,ef);if(Show(p)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model state");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Event order");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Memory");count++;}
 Console.WriteLine("PASS "+count+" dispatch instruction-model comparisons: all DX words, equality path, signed endpoint boundary comparisons, full EFLAGS and ordered memory effects.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
