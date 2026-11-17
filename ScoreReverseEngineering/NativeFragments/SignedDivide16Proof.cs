using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using ScoreModern.RecoveredFragments;
class SignedDivide16Proof {
 class Memory:IHorizontalRasterMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Writes=new List<string>();
  public void Load(ushort seg,byte[] b){for(int i=0;i<b.Length;i++){int p=(seg<<4)+i;byte old;if(Bytes.TryGetValue(p,out old)&&old!=b[i])throw new Exception("Alias inconsistency");Bytes[p]=b[i];}}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Bytes.TryGetValue((s<<4)+o,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);return (ushort)(a|b<<8);}
  public void WriteWord(ushort s,ushort o,ushort v){Writes.Add(s+":"+o+":"+v);Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public void OrByte(ushort s,ushort o,byte b){throw new Exception("Video effect");}
  public void Compare(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Length");for(int i=0;i<b.Length;i++)if(Bytes[(seg<<4)+i]!=b[i])throw new Exception("Segment byte "+i);}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static string Show(SignedDivide16State s){return s.AX+":"+s.BX+":"+s.CX+":"+s.DX+":"+s.SI+":"+s.DI+":"+s.BP+":"+s.CS+":"+s.IP+":"+s.DS+":"+s.ES+":"+s.SS+":"+s.SP+":"+s.EFlags;}
 static SignedDivide16State Parse(string[] a,int i){return new SignedDivide16State{AX=H(a[i]),BX=H(a[i+1]),CX=H(a[i+2]),DX=H(a[i+3]),SI=H(a[i+4]),DI=H(a[i+5]),BP=H(a[i+6]),CS=H(a[i+7]),IP=H(a[i+8]),DS=H(a[i+9]),ES=H(a[i+10]),SS=H(a[i+11]),SP=H(a[i+12]),EFlags=Convert.ToUInt32(a[i+13],16)};}
 static int Main(string[] args){try{
 if(args.Length>0){string p=args[0];SignedDivide16State before=Parse(args,1),expected=Parse(args,15);Memory m=new Memory();m.Load(before.DS,File.ReadAllBytes(Path.Combine(p,"fault-patched-ds.bin")));m.Load(before.SS,File.ReadAllBytes(Path.Combine(p,"fault-patched-ss.bin")));m.Load(0,File.ReadAllBytes(Path.Combine(p,"vector-zero.bin")));var r=SignedDivide16.Execute(m,before);if(r.Fault==SignedDivide16Fault.None||r.DefinedEFlagsMask!=0xffffffffu||Show(r.State)!=Show(expected))throw new Exception("Delivery state "+Show(r.State)+" != "+Show(expected));m.Compare(before.DS,File.ReadAllBytes(Path.Combine(p,"handler-ds.bin")));m.Compare(before.SS,File.ReadAllBytes(Path.Combine(p,"handler-ss.bin")));if(m.Writes.Count!=3)throw new Exception("Frame writes");Console.WriteLine("PASS signed divide-error delivery: all recorded registers/full EFLAGS, all 65536 DS and 65536 SS bytes, six-byte exception frame; "+r.Fault);return 0;}
 int count=0;int[] values={int.MinValue,-32768,32767,65536};for(int divisor=0;divisor<65536;divisor++)foreach(int n in values){SignedDivide16State s=new SignedDivide16State{AX=(ushort)n,DX=(ushort)(n>>16),CX=(ushort)divisor,CS=0x202b,IP=0x8d3,SS=0x871e,SP=(ushort)(divisor^0xe216),EFlags=0x7312};Memory m=new Memory();m.Load(0,new byte[]{0x66,0x2c,0xdd,0x20});BigInteger q=0;SignedDivide16Fault expected=SignedDivide16Fault.None;if((short)divisor==0)expected=SignedDivide16Fault.ZeroDivisor;else{q=new BigInteger(n)/new BigInteger((short)divisor);if(q<short.MinValue||q>short.MaxValue)expected=SignedDivide16Fault.QuotientOverflow;}var r=SignedDivide16.Execute(m,s);if(r.Fault!=expected)throw new Exception("Cause");if(expected==SignedDivide16Fault.None){if(r.State.AX!=(ushort)(int)q||r.State.DX!=(ushort)(int)(new BigInteger(n)%new BigInteger((short)divisor))||r.State.IP!=0x8d5||m.Writes.Count!=0||r.DefinedEFlagsMask!=~0x8d5u)throw new Exception("Success");}else{ushort sp=(ushort)(s.SP-6);if(r.State.CS!=0x20dd||r.State.IP!=0x2c66||r.State.SP!=sp||r.State.EFlags!=0x7012||m.ReadWord(s.SS,sp)!=s.IP||m.ReadWord(s.SS,(ushort)(sp+2))!=s.CS||m.ReadWord(s.SS,(ushort)(sp+4))!=(ushort)s.EFlags)throw new Exception("Frame projection");}count++;}
 Console.WriteLine("PASS "+count+" BigInteger signed-division comparisons across all divisor words and four boundary numerators; frame wrap/model checks. Only original fixtures establish runtime delivery equivalence.");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
