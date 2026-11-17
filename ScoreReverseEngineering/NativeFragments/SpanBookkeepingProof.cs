using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class SpanBookkeepingProof {
 class Memory:ISpanBookkeepingMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();
  byte Raw(ushort s,ushort o){byte b;return Bytes.TryGetValue((s<<4)+o,out b)?b:(byte)0;}
  public byte ReadByte(ushort s,ushort o){byte b=Raw(s,o);Events.Add("B"+((s<<4)+o)+":"+b);return b;}
  public ushort ReadWord(ushort s,ushort o){ushort b=(ushort)(Raw(s,o)|(Raw(s,(ushort)(o+1))<<8));Events.Add("R"+((s<<4)+o)+":"+b);return b;}
  public void WriteByte(ushort s,ushort o,byte b){Bytes[(s<<4)+o]=b;Events.Add("b"+((s<<4)+o)+":"+b);}
  public void WriteWord(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);Events.Add("W"+((s<<4)+o)+":"+v);}
  public Memory Copy(){Memory m=new Memory();foreach(var x in Bytes)m.Bytes[x.Key]=x.Value;return m;}
 }
 static ushort Reference(byte[] code,Memory m,ushort ss,ushort bp,ushort ds,ushort si,ushort di){
  int pc=0;ushort bx=0;bool positive=false;Func<byte> f=()=>code[pc++];Func<ushort> w=()=>{byte b=f();return (ushort)(b|(f()<<8));};
  while(pc<code.Length){byte op=f(),q,d;ushort at,v;switch(op){
   case 0x01:q=f();d=f();if(q==0x7e){at=(ushort)(bp+(sbyte)d);v=m.ReadWord(ss,at);m.WriteWord(ss,at,(ushort)(v+di));}else if(q==0x7f){at=(ushort)(bx+(sbyte)d);v=m.ReadWord(ds,at);m.WriteWord(ds,at,(ushort)(v+di));}else throw new Exception("ADD");break;
   case 0x8b:if(f()!=0x1e)throw new Exception("MOV");bx=m.ReadWord(ds,w());break;
   case 0x2b:if(f()!=0xf7)throw new Exception("SUB");si=(ushort)(si-di);break;
   case 0x0b:if(f()!=0xff)throw new Exception("OR");positive=(short)di>0;break;
   case 0x7e:sbyte delta=(sbyte)f();if(!positive)pc+=delta;break;
   case 0x80:if(f()!=0x4f)throw new Exception("OR byte");at=(ushort)(bx+(sbyte)f());byte mask=f();byte flags=m.ReadByte(ds,at);m.WriteByte(ds,at,(byte)(flags|mask));break;
   default:throw new Exception("Opcode");
  }}return si;
 }
 class Captured:ISpanBookkeepingMemory {
  public byte[] Data;public Captured(byte[] b){Data=(byte[])b.Clone();}
  public byte ReadByte(ushort s,ushort o){if(s!=0x871e)throw new Exception("Segment");return Data[o];}
  public ushort ReadWord(ushort s,ushort o){return (ushort)(ReadByte(s,o)|(ReadByte(s,(ushort)(o+1))<<8));}
  public void WriteByte(ushort s,ushort o,byte b){if(s!=0x871e)throw new Exception("Segment");Data[o]=b;}
  public void WriteWord(ushort s,ushort o,ushort w){WriteByte(s,o,(byte)w);WriteByte(s,(ushort)(o+1),(byte)(w>>8));}
 }
 static void Main(string[] a){
  byte[] original=File.ReadAllBytes(a[0]),code=new byte[24];Array.Copy(original,0x1f080+0x11f,code,0,24);
  if(BitConverter.ToString(code)!="01-7E-04-8B-1E-59-62-01-7F-0A-2B-F7-0B-FF-7E-08-8B-1E-59-62-80-4F-04-01")throw new Exception("Original code");
  if(a.Length>1){byte[] runtime=File.ReadAllBytes(Path.Combine(a[1],"code.bin"));if(runtime.Length!=24)throw new Exception("Runtime length");for(int i=0;i<24;i++)if(runtime[i]!=code[i])throw new Exception("Runtime code");Captured m=new Captured(File.ReadAllBytes(Path.Combine(a[1],"before-ds.bin")));ushort si=ExecuteRuntime(m,a);byte[] after=File.ReadAllBytes(Path.Combine(a[1],"after-ds.bin"));for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("Runtime DS "+i.ToString("X4"));Console.WriteLine("PASS original bookkeeping runtime: exact 24 code bytes; all 65536 DS bytes and updated SI; zero exclusions.");return;}
  int count=0;for(int di=0;di<65536;di++)for(int layout=0;layout<8;layout++){
   ushort ds=0x2000,ss=layout==5?(ushort)0x1fff:ds,arg=0x8000,obj=0x9000;if(layout==1)arg=0x6259;if(layout==2||layout==7)arg=0x900a;if(layout==3)obj=0x624f;if(layout==4)obj=0x6255;if(layout==5)arg=0x8010;if(layout==6)arg=0x625a;
   Memory seed=new Memory();seed.WriteWord(ds,0x6259,obj);seed.WriteWord(ds,(ushort)(obj+10),(ushort)(di^0x5678));seed.WriteByte(ds,(ushort)(obj+4),(byte)(di^0xa4));seed.WriteWord(ss,arg,(ushort)(di*13));seed.Events.Clear();Memory actual=seed.Copy(),reference=seed.Copy();ushort si=(ushort)(di*31+17);
   ushort expected=Reference(code,reference,ss,(ushort)(arg-4),ds,si,(ushort)di),result=SpanBookkeeping.Execute(actual,ss,arg,ds,si,(ushort)di);
   if(result!=expected||actual.Bytes.Count!=reference.Bytes.Count||actual.Events.Count!=reference.Events.Count)throw new Exception("Extent/result");for(int i=0;i<actual.Events.Count;i++)if(actual.Events[i]!=reference.Events[i])throw new Exception("Memory order");foreach(var p in actual.Bytes)if(!reference.Bytes.ContainsKey(p.Key)||reference.Bytes[p.Key]!=p.Value)throw new Exception("Memory");count++;
  }
  Console.WriteLine("PASS "+count+" original-instruction bookkeeping comparisons: all DI words, signed branch, eight pointer/descriptor alias layouts and ordered memory effects.");
 }
 static ushort ExecuteRuntime(Captured m,string[] a){ushort ss=Convert.ToUInt16(a[2],16),arg=Convert.ToUInt16(a[3],16),ds=Convert.ToUInt16(a[4],16),si=Convert.ToUInt16(a[5],16),di=Convert.ToUInt16(a[6],16),expected=Convert.ToUInt16(a[7],16);ushort result=SpanBookkeeping.Execute(m,ss,arg,ds,si,di);if(result!=expected)throw new Exception("Runtime SI");return result;}
}
