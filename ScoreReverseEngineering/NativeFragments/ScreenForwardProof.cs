using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class ScreenForwardProof {
 class Memory:IScreenForwardMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Bytes.TryGetValue((s<<4)+o,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|(b<<8));Events.Add(((s<<4)+o)+":"+v);return v;}
  public Memory Copy(){Memory m=new Memory();foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static ScreenForwardResult Reference(byte[] c,Memory m,ushort ss,ushort sp,ushort codeSegment){
  int pc=0;ushort dx=0,cx=0,bx=0,ds=0;Func<byte> f=()=>c[pc++];Func<ushort> pop=()=>{ushort v=m.ReadWord(ss,sp);sp=(ushort)(sp+2);return v;};
  while(pc<c.Length){byte op=f();switch(op){case 0x5a:dx=pop();break;case 0x59:cx=pop();break;case 0x5b:bx=pop();break;case 0x1f:ds=pop();break;case 0x2e:if(f()!=0xff||f()!=0x2e)throw new Exception("Far transfer");ushort at=(ushort)(f()|(f()<<8));ushort ip=m.ReadWord(codeSegment,at),cs=m.ReadWord(codeSegment,(ushort)(at+2));return new ScreenForwardResult(dx,cx,bx,ds,sp,ip,cs);default:throw new Exception("Opcode");}}
  throw new Exception("Missing target");
 }
 static bool Equal(ScreenForwardResult a,ScreenForwardResult b){return a.DX==b.DX&&a.CX==b.CX&&a.BX==b.BX&&a.DS==b.DS&&a.SP==b.SP&&a.IP==b.IP&&a.CS==b.CS;}
 class Captured:IScreenForwardMemory {
  public Dictionary<ushort,byte[]> Segments=new Dictionary<ushort,byte[]>();
  public ushort ReadWord(ushort s,ushort o){byte[] b=Segments[s];return (ushort)(b[o]|(b[(ushort)(o+1)]<<8));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static void Run(string[] a){byte[] original=File.ReadAllBytes(a[0]),code=new byte[9];Array.Copy(original,0x16fa0+0x2a5,code,0,9);if(BitConverter.ToString(code)!="5A-59-5B-1F-2E-FF-2E-CC-01")throw new Exception("Original identity");
  if(a.Length>1){string folder=a[1];byte[] live=File.ReadAllBytes(Path.Combine(folder,"restore-code.bin"));if(live.Length!=9)throw new Exception("Runtime length");for(int i=0;i<9;i++)if(code[i]!=live[i])throw new Exception("Runtime code");ushort ss=H(a[2]),sp=H(a[3]),cs=H(a[4]);Captured m=new Captured();m.Segments[ss]=File.ReadAllBytes(Path.Combine(folder,"restore-entry-ss.bin"));m.Segments[cs]=File.ReadAllBytes(Path.Combine(folder,"restore-entry-code-segment.bin"));ScreenForwardResult r=ScreenHookForwarding.RestoreAndResolve(m,ss,sp,cs),e=new ScreenForwardResult(H(a[5]),H(a[6]),H(a[7]),H(a[8]),H(a[9]),H(a[10]),H(a[11]));if(!Equal(r,e))throw new Exception("Runtime restored values/target");int count=0;foreach(var p in m.Segments){byte[] expected=File.ReadAllBytes(Path.Combine(folder,p.Key==ss?"bios-entry-ss.bin":"bios-entry-code-segment.bin"));for(int i=0;i<65536;i++)if(p.Value[i]!=expected[i])throw new Exception("Runtime memory mutation");count+=65536;}Console.WriteLine("PASS original restoration/forwarding: exact 9 code bytes; restored DX/CX/BX/DS/SP and CS:IP target; "+count+" captured stack/code bytes unchanged, zero exclusions.");return;}
  int cases=0;for(int n=0;n<65536;n++)for(int layout=0;layout<8;layout++){
   ushort ss=0x2000,cs=new ushort[]{0x1f08,0x2000,0x2000,0x2000,0x1fff,0x2001,0x1f08,0x2000}[layout],sp=new ushort[]{0x8000,0x1cc,0x1ca,0x1c6,0x1bc,0x1dc,0xfffa,0}[layout];Memory seed=new Memory();seed.Seed(ss,sp,(ushort)n);seed.Seed(ss,(ushort)(sp+2),(ushort)(n^0xa55a));seed.Seed(ss,(ushort)(sp+4),(ushort)(n*31+3));seed.Seed(ss,(ushort)(sp+6),(ushort)(n*13+7));seed.Seed(cs,0x1cc,(ushort)(n^0x1234));seed.Seed(cs,0x1ce,(ushort)(n*17+11));Memory actual=seed.Copy(),reference=seed.Copy();ScreenForwardResult r=ScreenHookForwarding.RestoreAndResolve(actual,ss,sp,cs),e=Reference(code,reference,ss,sp,cs);if(!Equal(r,e)||actual.Events.Count!=reference.Events.Count)throw new Exception("Result/extent");for(int i=0;i<actual.Events.Count;i++)if(actual.Events[i]!=reference.Events[i])throw new Exception("Read order");cases++;
  }
  Console.WriteLine("PASS "+cases+" original-instruction restoration/forwarding comparisons: word patterns, eight overlapping vector/frame/segment layouts, stack-offset wrap and ordered reads.");
  Console.WriteLine("Scope: restored-word/target projection only, not BIOS execution, IRET, complete flags/interrupt ABI or rendering.");
 }
 static int Main(string[] a){try{Run(a);return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
}
