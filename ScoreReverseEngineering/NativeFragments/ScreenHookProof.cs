using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using ScoreModern.RecoveredFragments;
class ScreenHookProof {
 class Memory:IStringBlockMemory {
  public Dictionary<ushort,byte> Bytes=new Dictionary<ushort,byte>();public List<string> Events=new List<string>();
  byte Raw(ushort o){byte b;return Bytes.TryGetValue(o,out b)?b:(byte)0;}
  public byte ReadByte(ushort s,ushort o){if(s!=0x2000)throw new Exception("Segment");byte b=Raw(o);Events.Add("B"+o+":"+b);return b;}
  public ushort ReadWord(ushort s,ushort o){if(s!=0x2000)throw new Exception("Segment");ushort w=(ushort)(Raw(o)|(Raw((ushort)(o+1))<<8));Events.Add("R"+o+":"+w);return w;}
  public void WriteByte(ushort s,ushort o,byte b){if(s!=0x2000)throw new Exception("Segment");Bytes[o]=b;Events.Add("b"+o+":"+b);}
  public void WriteWord(ushort s,ushort o,ushort w){if(s!=0x2000)throw new Exception("Segment");Bytes[o]=(byte)w;Bytes[(ushort)(o+1)]=(byte)(w>>8);Events.Add("W"+o+":"+w);}
  public Memory Copy(){Memory m=new Memory();foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static void Compare(Memory a,Memory b){if(a.Bytes.Count!=b.Bytes.Count||a.Events.Count!=b.Events.Count)throw new Exception("Memory extent");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Memory order");foreach(var p in a.Bytes)if(!b.Bytes.ContainsKey(p.Key)||b.Bytes[p.Key]!=p.Value)throw new Exception("Memory values");}
 class Flags {public bool Zero,Less;public void Cmp(byte a,byte b){byte r=(byte)(a-b);Zero=r==0;bool sf=(r&128)!=0,of=((a^b)&(a^r)&128)!=0;Less=sf!=of;}}
 static bool GateReference(byte[] c,Memory m,ushort ax){
  int pc=0;Flags flags=new Flags();Func<byte> f=()=>c[pc++];Func<ushort> w=()=>{byte b=f();return (ushort)(b|(f()<<8));};
  while(pc<c.Length){byte op=f(),q,d;ushort at;switch(op){
   case 0x80:q=f();if(q==0xfc)flags.Cmp((byte)(ax>>8),f());else if(q==0x3e){at=w();byte imm=f();flags.Cmp(m.ReadByte(0x2000,at),imm);}else throw new Exception("CMP gate");break;
   case 0x3c:flags.Cmp((byte)ax,f());break;
   case 0xa2:m.WriteByte(0x2000,w(),(byte)ax);break;
   case 0x75:d=f();if(!flags.Zero)pc+=(sbyte)d;break;case 0x74:d=f();if(flags.Zero)pc+=(sbyte)d;break;
   case 0x7e:d=f();if(flags.Zero||flags.Less)pc+=(sbyte)d;break;case 0xeb:d=f();pc+=(sbyte)d;break;
   default:throw new Exception("Gate opcode");
  }}return pc==c.Length;
 }
 static bool DecisionReference(byte[] c,Memory m,byte al,ushort dx){
  int pc=0;Flags flags=new Flags();Func<byte> f=()=>c[pc++];Func<ushort> w=()=>{byte b=f();return (ushort)(b|(f()<<8));};
  while(pc<c.Length){byte op=f(),q,d;ushort at;switch(op){
   case 0x3c:flags.Cmp(al,f());break;
   case 0x74:d=f();if(flags.Zero)pc+=(sbyte)d;break;
   case 0x3a:q=f();at=w();if(q==0x16)flags.Cmp((byte)dx,m.ReadByte(0x2000,at));else if(q==0x36)flags.Cmp((byte)(dx>>8),m.ReadByte(0x2000,at));else throw new Exception("Decision CMP");break;
   case 0x7c:d=f();if(flags.Less)pc+=(sbyte)d;break;
   default:throw new Exception("Decision opcode");
  }}return pc==c.Length;
 }
 static ushort RowReference(byte[] c,Memory m){int pc=0;ushort dx=0;Func<byte> f=()=>c[pc++];Func<ushort> w=()=>{byte b=f();return (ushort)(b|(f()<<8));};while(pc<c.Length){byte op=f();switch(op){case 0x8b:if(f()!=0x16)throw new Exception("MOV DX");dx=m.ReadWord(0x2000,w());break;case 0xfe:if(f()!=0xce)throw new Exception("DEC DH");dx=(ushort)((dx&255)|((byte)((dx>>8)-1)<<8));break;case 0x89:if(f()!=0x16)throw new Exception("Store DX");m.WriteWord(0x2000,w(),dx);break;default:throw new Exception("Row opcode");}}return dx;}
 static byte[] gate,decision,row;static int gateCases,decisionCases,rowCases;
 static void Gate(byte mode,ushort ax){Memory initial=new Memory();initial.WriteByte(0x2000,0x4ba2,mode);Memory a=initial.Copy(),b=initial.Copy();if(ScreenHookDecisions.ShouldProbeCursor(a,0x2000,ax)!=GateReference(gate,b,ax))throw new Exception("Gate branch");Compare(a,b);gateCases++;}
 static void Decision(byte al,ushort dx,byte col,byte bottom){Memory initial=new Memory();initial.WriteByte(0x2000,0x4b9a,col);initial.WriteByte(0x2000,0x4b9b,bottom);Memory a=initial.Copy(),b=initial.Copy();if(ScreenHookDecisions.ShouldScroll(a,0x2000,al,dx)!=DecisionReference(decision,b,al,dx))throw new Exception("Scroll branch");Compare(a,b);decisionCases++;}
 class Captured:IStringBlockMemory {
  public byte[] Data;ushort ds;public Captured(byte[] b,ushort s){Data=(byte[])b.Clone();ds=s;}
  public byte ReadByte(ushort s,ushort o){if(s!=ds)throw new Exception("Uncaptured segment");return Data[o];}
  public ushort ReadWord(ushort s,ushort o){return (ushort)(ReadByte(s,o)|(ReadByte(s,(ushort)(o+1))<<8));}
  public void WriteByte(ushort s,ushort o,byte b){if(s!=ds)throw new Exception("Uncaptured segment");Data[o]=b;}
  public void WriteWord(ushort s,ushort o,ushort w){WriteByte(s,o,(byte)w);WriteByte(s,(ushort)(o+1),(byte)(w>>8));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static void Runtime(string[] a){string kind=a[1];byte[] c=kind=="gate"?gate:kind=="decision"?decision:row;byte[] live=File.ReadAllBytes(a[2]);if(live.Length!=c.Length)throw new Exception("Runtime code length");for(int i=0;i<c.Length;i++)if(live[i]!=c[i])throw new Exception("Runtime code identity");ushort ds=H(a[5]);Captured m=new Captured(File.ReadAllBytes(a[3]),ds);
  if(kind=="gate"){bool result=ScreenHookDecisions.ShouldProbeCursor(m,ds,H(a[6]));if(result!=(a[7]=="1"))throw new Exception("Runtime gate");}
  else if(kind=="decision"){bool result=ScreenHookDecisions.ShouldScroll(m,ds,(byte)H(a[6]),H(a[7]));if(result!=(a[8]=="1"))throw new Exception("Runtime decision");}
  else{ushort dx=ScreenHookDecisions.DecrementSavedRow(m,ds);if(dx!=H(a[6]))throw new Exception("Runtime row DX");}
  byte[] after=File.ReadAllBytes(a[4]);for(int i=0;i<65536;i++)if(m.Data[i]!=after[i])throw new Exception("Runtime DS mismatch "+i.ToString("X4"));Console.WriteLine("PASS original "+kind+" block: exact code bytes, branch/result and all 65536 DS bytes, zero exclusions.");
 }
 static void Run(string[] a){byte[] source=File.ReadAllBytes(a[0]);using(SHA256 sha=SHA256.Create()){if(BitConverter.ToString(sha.ComputeHash(source)).Replace("-","").ToLowerInvariant()!="392b955d45cfe0310894035e679537e35e12e224da40b0ebace6b4cc42d8e9d4")throw new Exception("Original main hash");}gate=new byte[35];decision=new byte[16];row=new byte[10];Array.Copy(source,0x16fa0+0x23f,gate,0,35);Array.Copy(source,0x16fa0+0x270,decision,0,16);Array.Copy(source,0x16fa0+0x29a,row,0,10);
  if(a.Length>1){Runtime(a);return;}
  foreach(byte mode in new byte[]{0,2,3,0x12,0x7f,0x80,0xfe,0xff})for(int ax=0;ax<65536;ax++)Gate(mode,(ushort)ax);
  for(int mode=0;mode<256;mode++)foreach(byte ah in new byte[]{0,3,0x0e,0xff,1})foreach(byte al in new byte[]{0,7,8,10,13,0x4d,0x80,0xff})Gate((byte)mode,(ushort)((ah<<8)|al));
  byte[][] bounds={new byte[]{0,0},new byte[]{79,4},new byte[]{127,127},new byte[]{128,255}};
  foreach(byte[] b in bounds)foreach(byte al in new byte[]{0x41,0x0a})for(int dx=0;dx<65536;dx++)Decision(al,(ushort)dx,b[0],b[1]);
  for(int al=0;al<256;al++)foreach(ushort dx in new ushort[]{0,0x044f,0x0450,0x807f,0x7f80,0xffff,0x8000,0xff00})foreach(byte[] b in bounds)Decision((byte)al,dx,b[0],b[1]);
  for(int value=0;value<65536;value++){Memory initial=new Memory();initial.WriteWord(0x2000,0x4b9e,(ushort)value);Memory x=initial.Copy(),y=initial.Copy();if(ScreenHookDecisions.DecrementSavedRow(x,0x2000)!=RowReference(row,y))throw new Exception("Row result");Compare(x,y);rowCases++;}
  Console.WriteLine("PASS "+gateCases+" gate, "+decisionCases+" scroll-decision and "+rowCases+" row-update original-instruction comparisons; total "+(gateCases+decisionCases+rowCases)+". Ordered memory, signed comparisons, LF bypass and byte wrap checked.");
  Console.WriteLine("Scope: data/branch projections and row-result word; no complete interrupt flags/frame, BIOS-call/forwarding or rendering implementation. Model cases are not DOS execution recordings for every input.");
 }
 static int Main(string[] a){try{Run(a);return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
}
