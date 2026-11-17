using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class DirectionalStringProof {
 struct Event {public byte Op;public int Address;public ushort Value;public Event(byte op,int a,ushort v){Op=op;Address=a;Value=v;}}
 class Memory:IStringBlockMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<Event> Events=new List<Event>();int seed;
  public Memory(int n){seed=n;}
  byte Raw(int a){byte b;if(Bytes.TryGetValue(a,out b))return b;return (byte)(((a&1)==0?seed:seed>>8)^(a*17)^(a>>8));}
  public byte ReadByte(ushort s,ushort o){int a=(s<<4)+o;byte b=Raw(a);Events.Add(new Event(1,a,b));return b;}
  public ushort ReadWord(ushort s,ushort o){int a=(s<<4)+o;ushort w=(ushort)(Raw(a)|(Raw(a+1)<<8));Events.Add(new Event(2,a,w));return w;}
  public void WriteByte(ushort s,ushort o,byte b){int a=(s<<4)+o;Bytes[a]=b;Events.Add(new Event(3,a,b));}
  public void WriteWord(ushort s,ushort o,ushort w){int a=(s<<4)+o;Bytes[a]=(byte)w;Bytes[a+1]=(byte)(w>>8);Events.Add(new Event(4,a,w));}
 }
 struct Registers {public ushort SI,DI,CX;}
 static Registers Reference(byte[] code,Memory m,ushort ds,ushort si,ushort es,ushort di,ushort ax,ushort cx,bool df){
  int pc=0;bool carry=false;
  while(pc<code.Length){byte op=code[pc++];switch(op){
   case 0xd1:if(code[pc++]!=0xe9)throw new Exception("SHR");carry=(cx&1)!=0;cx>>=1;break;
   case 0xf3:byte transfer=code[pc++];while(cx!=0){
    if(transfer==0xa5){ushort w=m.ReadWord(ds,si);m.WriteWord(es,di,w);si=(ushort)(si+(df?-2:2));}
    else if(transfer==0xab)m.WriteWord(es,di,ax);else throw new Exception("REP");
    di=(ushort)(di+(df?-2:2));cx--;
   }break;
   case 0x73:sbyte d=(sbyte)code[pc++];if(!carry)pc+=d;break;
   case 0xa4:byte b=m.ReadByte(ds,si);m.WriteByte(es,di,b);si=(ushort)(si+(df?-1:1));di=(ushort)(di+(df?-1:1));break;
   case 0xaa:m.WriteByte(es,di,(byte)ax);di=(ushort)(di+(df?-1:1));break;
   default:throw new Exception("Opcode");
  }}return new Registers{SI=si,DI=di,CX=cx};
 }
 static byte[] copyCode,fillCode;static int comparisons;
 static void Compare(Memory a,Memory b){
  if(a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Extent");
  for(int i=0;i<a.Events.Count;i++){Event x=a.Events[i],y=b.Events[i];if(x.Op!=y.Op||x.Address!=y.Address||x.Value!=y.Value)throw new Exception("Memory order");}
  foreach(var p in a.Bytes)if(!b.Bytes.ContainsKey(p.Key)||b.Bytes[p.Key]!=p.Value)throw new Exception("Memory");
 }
 static void Check(int seed,ushort count,bool df,int layout){
  ushort ds=0x2000,es=0x3000,si=0x4000,di=0x4000;
  if(layout==1){es=0x1fff;di=0x4010;}if(layout>=2){es=ds;di=(ushort)(si+new int[]{0,0,0,1,2,-1,-2,16}[layout]);}
  Memory a=new Memory(seed),b=new Memory(seed);Registers r=Reference(copyCode,b,ds,si,es,di,(ushort)seed,count,df);StringCopyResult n=DirectionalStringBlocks.Copy(a,ds,si,es,di,count,df);
  if(r.SI!=n.SI||r.DI!=n.DI||r.CX!=n.CX)throw new Exception("Copy registers");Compare(a,b);comparisons++;
  a=new Memory(seed);b=new Memory(seed);r=Reference(fillCode,b,ds,si,es,di,(ushort)seed,count,df);StringFillResult f=DirectionalStringBlocks.Fill(a,es,di,(ushort)seed,count,df);
  if(r.DI!=f.DI||r.CX!=f.CX)throw new Exception("Fill registers");Compare(a,b);comparisons++;
 }
 class Captured:IStringBlockMemory {
  public Dictionary<ushort,byte[]> Segments=new Dictionary<ushort,byte[]>();
  public byte ReadByte(ushort s,ushort o){return Segments[s][o];}
  public ushort ReadWord(ushort s,ushort o){return (ushort)(ReadByte(s,o)|(ReadByte(s,(ushort)(o+1))<<8));}
  public void WriteByte(ushort s,ushort o,byte v){Segments[s][o]=v;}
  public void WriteWord(ushort s,ushort o,ushort v){WriteByte(s,o,(byte)v);WriteByte(s,(ushort)(o+1),(byte)(v>>8));}
 }
 static void Runtime(string[] a){
  string folder=a[1],kind=a[2];ushort ds=Convert.ToUInt16(a[3],16),es=Convert.ToUInt16(a[4],16),si=Convert.ToUInt16(a[5],16),di=Convert.ToUInt16(a[6],16),ax=Convert.ToUInt16(a[7],16),cx=Convert.ToUInt16(a[8],16);bool df=a[9]=="1";ushort nextSi=Convert.ToUInt16(a[10],16),nextDi=Convert.ToUInt16(a[11],16),nextCx=Convert.ToUInt16(a[12],16);
  byte[] actualCode=File.ReadAllBytes(Path.Combine(folder,kind+"-code.bin")),expectedCode=kind=="copy"?copyCode:fillCode;if(actualCode.Length!=7)throw new Exception("Runtime code length");for(int i=0;i<7;i++)if(actualCode[i]!=expectedCode[i])throw new Exception("Runtime code");
  Captured m=new Captured();m.Segments[ds]=File.ReadAllBytes(Path.Combine(folder,kind+"-entry-ds.bin"));if(es!=ds)m.Segments[es]=File.ReadAllBytes(Path.Combine(folder,kind+"-entry-es.bin"));
  if(kind=="copy"){StringCopyResult n=DirectionalStringBlocks.Copy(m,ds,si,es,di,cx,df);if(n.SI!=nextSi||n.DI!=nextDi||n.CX!=nextCx)throw new Exception("Runtime copy registers");}
  else{StringFillResult n=DirectionalStringBlocks.Fill(m,es,di,ax,cx,df);if(n.DI!=nextDi||n.CX!=nextCx)throw new Exception("Runtime fill registers");}
  int count=0;foreach(ushort seg in m.Segments.Keys){byte[] expected=File.ReadAllBytes(Path.Combine(folder,kind+"-exit-"+(seg==ds?"ds":"es")+".bin"));for(int i=0;i<65536;i++)if(m.Segments[seg][i]!=expected[i])throw new Exception("Runtime memory "+seg.ToString("X4")+":"+i.ToString("X4"));count+=65536;}
  Console.WriteLine("PASS original "+kind+" runtime: "+count+" captured DS/ES bytes, updated index/count words, exact 7 code bytes; zero exclusions.");
 }
 static void Main(string[] a){
  byte[] original=File.ReadAllBytes(a[0]);copyCode=new byte[7];fillCode=new byte[7];Array.Copy(original,0x1f080+0x115,copyCode,0,7);Array.Copy(original,0x1f080+0xef,fillCode,0,7);
  if(BitConverter.ToString(copyCode)!="D1-E9-F3-A5-73-01-A4"||BitConverter.ToString(fillCode)!="D1-E9-F3-AB-73-01-AA")throw new Exception("Original identity");
  if(a.Length>1){Runtime(a);return;}
  for(int count=0;count<=513;count++)for(int df=0;df<2;df++)for(int layout=0;layout<8;layout++)Check(count*131+layout,(ushort)count,df!=0,layout);
  for(int seed=0;seed<65536;seed++)Check(seed,(ushort)(seed%17),(seed&8)!=0,seed%8);
  foreach(ushort count in new ushort[]{1023,1024,4095,4096,32767,32768,65534,65535})for(int df=0;df<2;df++)for(int layout=0;layout<2;layout++)Check(0xa55a,count,df!=0,layout);
  Console.WriteLine("PASS "+comparisons+" original-instruction comparisons across copy/fill; both DF values, odd/even/zero/large counts, all 65536 fill words, ordered word/byte operations and overlap/segment alias layouts.");
  Console.WriteLine("Scope: memory and index/count effects. Model tests do not establish hardware limit/exception behavior, interrupts, full flags/ABI or GUI equivalence.");
 }
}
