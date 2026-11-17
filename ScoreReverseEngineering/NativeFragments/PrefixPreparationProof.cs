using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class PrefixPreparationProof {
 class Memory:IPrefixPreparationMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();
  byte Raw(ushort s,ushort o){byte b;return Bytes.TryGetValue((s<<4)+o,out b)?b:(byte)0;}
  public byte ReadByte(ushort s,ushort o){byte b=Raw(s,o);Events.Add("B"+((s<<4)+o)+":"+b);return b;}
  public ushort ReadWord(ushort s,ushort o){ushort v=(ushort)(Raw(s,o)|(Raw(s,(ushort)(o+1))<<8));Events.Add("R"+((s<<4)+o)+":"+v);return v;}
  public void WriteByte(ushort s,ushort o,byte b){Bytes[(s<<4)+o]=b;Events.Add("b"+((s<<4)+o)+":"+b);}
  public void WriteWord(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);Events.Add("W"+((s<<4)+o)+":"+v);}
  public Memory Copy(){Memory m=new Memory();foreach(var x in Bytes)m.Bytes[x.Key]=x.Value;return m;}
 }
 static PrefixPreparationResult Reference(byte[] code,Memory m,ushort ss,ushort ds,ushort cs,ushort sp,ushort bp,ushort si,ushort di){
  int pc=0;ushort ax=0xaaaa,bx=0;bool zero=false;Func<byte> f=()=>code[pc++];Func<ushort> w=()=>{byte b=f();return (ushort)(b|(f()<<8));};Action<ushort> push=v=>{sp=(ushort)(sp-2);m.WriteWord(ss,sp,v);};
  while(pc<code.Length){byte op=f(),q,d;ushort at;switch(op){
   case 0x55:push(bp);break;case 0x57:push(di);break;case 0x56:push(si);break;case 0x50:push(ax);break;case 0x1e:push(ds);break;
   case 0x8b:q=f();if(q==0xec)bp=sp;else if(q==0x1e)bx=m.ReadWord(ds,w());else if(q==0xc7)ax=di;else throw new Exception("MOV");break;
   case 0x83:if(f()!=0xec)throw new Exception("SUB");sp=(ushort)(sp-(sbyte)f());break;
   case 0xbe:si=w();break;case 0xbf:di=w();break;
   case 0x80:if(f()!=0x7f)throw new Exception("CMP byte");at=(ushort)(bx+(sbyte)f());zero=m.ReadByte(ds,at)==f();break;
   case 0x75:d=f();if(!zero)pc+=(sbyte)d;break;case 0x74:d=f();if(zero)pc+=(sbyte)d;break;case 0xeb:d=f();pc+=(sbyte)d;break;
   case 0xb0:ax=(ushort)((ax&0xff00)|f());break;
   case 0x8a:q=f();d=f();if(q==0x47)ax=(ushort)((ax&0xff00)|m.ReadByte(ds,(ushort)(bx+(sbyte)d)));else if(q==0x46)ax=(ushort)((ax&0xff00)|m.ReadByte(ss,(ushort)(bp+(sbyte)d)));else throw new Exception("Byte read");break;
   case 0x88:if(f()!=0x46)throw new Exception("Byte store");d=f();m.WriteByte(ss,(ushort)(bp+(sbyte)d),(byte)ax);break;
   case 0x98:ax=unchecked((ushort)(short)(sbyte)ax);break;
   case 0x3d:zero=ax==w();break;
   case 0x9a:ushort target=w(),segment=w();if(target!=0x312e||segment!=0x18b9)throw new Exception("External target");push(cs);push((ushort)(0x33fa+pc));return new PrefixPreparationResult(ax,bp,sp,si,di);
   default:throw new Exception("Opcode "+op.ToString("X2")+" at "+pc.ToString("X4"));
  }}throw new Exception("No call boundary");
 }
 class Captured:IPrefixPreparationMemory {
  public byte[] Data;public Captured(byte[] b){Data=(byte[])b.Clone();}
  void Check(ushort s){if(s!=0x871e)throw new Exception("Uncaptured segment");}
  public byte ReadByte(ushort s,ushort o){Check(s);return Data[o];}
  public ushort ReadWord(ushort s,ushort o){return (ushort)(ReadByte(s,o)|(ReadByte(s,(ushort)(o+1))<<8));}
  public void WriteByte(ushort s,ushort o,byte b){Check(s);Data[o]=b;}
  public void WriteWord(ushort s,ushort o,ushort v){WriteByte(s,o,(byte)v);WriteByte(s,(ushort)(o+1),(byte)(v>>8));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static void Main(string[] a){
  byte[] source=File.ReadAllBytes(a[0]),code=new byte[95];Array.Copy(source,0x1f080+0x33fa,code,0,95);
  if(BitConverter.ToString(code).Replace("-","").ToLowerInvariant()!="558bec83ec065756be0200bf91638b1e5962807f02007504b001eb078b1e59628a47028846fa8a4604983d200074213d2b0074193d300074073d31007407eb10be0400eb0bbf8b63be0600eb03be0100568bc71e508a46fa98509a2e31b918")throw new Exception("Original code identity");
  if(a.Length>1){byte[] actualCode=File.ReadAllBytes(a[1]),expected=(byte[])code.Clone();ushort cs=H(a[4]);ushort callee=(ushort)(0x18b9+cs-0x1f08);expected[93]=(byte)callee;expected[94]=(byte)(callee>>8);if(actualCode.Length!=95)throw new Exception("Runtime length");for(int i=0;i<95;i++)if(actualCode[i]!=expected[i])throw new Exception("Runtime code identity");Captured m=new Captured(File.ReadAllBytes(a[2]));PrefixPreparationResult r=PrefixWritePreparation.Prepare(m,0x871e,0x871e,cs,H(a[5]),H(a[6]),H(a[7]),H(a[8]));if(r.AX!=H(a[9])||r.BP!=H(a[10])||r.SP!=H(a[11])||r.SI!=H(a[12])||r.DI!=H(a[13]))throw new Exception("Runtime register boundary");byte[] after=File.ReadAllBytes(a[3]);for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("Original DS mismatch "+i.ToString("X4"));Console.WriteLine("PASS original prefix preparation: all 65536 DS bytes including physical frame/call writes, AX/BP/SP/SI/DI and exact 95 relocated code bytes; zero exclusions.");return;}
  int count=0;for(int prefix=0;prefix<256;prefix++)for(int handle=0;handle<256;handle++)for(int layout=0;layout<8;layout++){
   ushort ds=0x2000,ss=layout==1?(ushort)0x1fff:(layout==6?(ushort)0x3000:ds),sp=new ushort[]{0x8000,0x8010,0x625b,0x6396,0x9006,0x6267,0x8000,0x000e}[layout];int seed=(prefix<<8)|handle;ushort bp=(ushort)(seed^0xbeef),si=(ushort)(seed*13),di=(ushort)(seed^0xa55a);
   Memory initial=new Memory();initial.WriteWord(ds,0x6259,0x9000);initial.WriteByte(ds,0x9002,(byte)handle);initial.WriteWord(ss,sp,0x4816);initial.WriteWord(ss,(ushort)(sp+2),(ushort)prefix);Memory actual=initial.Copy(),reference=initial.Copy();PrefixPreparationResult r=PrefixWritePreparation.Prepare(actual,ss,ds,0x1f08,sp,bp,si,di),e=Reference(code,reference,ss,ds,0x1f08,sp,bp,si,di);
   if(r.AX!=e.AX||r.BP!=e.BP||r.SP!=e.SP||r.SI!=e.SI||r.DI!=e.DI||actual.Events.Count!=reference.Events.Count||actual.Bytes.Count!=reference.Bytes.Count)throw new Exception("Result/extent");for(int i=0;i<actual.Events.Count;i++)if(actual.Events[i]!=reference.Events[i])throw new Exception("Memory event");foreach(var p in actual.Bytes)if(!reference.Bytes.ContainsKey(p.Key)||reference.Bytes[p.Key]!=p.Value)throw new Exception("Memory");count++;
  }
  Console.WriteLine("PASS "+count+" original-instruction prefix preparation cases: all prefix/handle bytes, eight frame/descriptor/segment layouts, ordered memory and call-boundary register words.");
 }
}
