using System;using System.IO;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
class ScreenHookInterruptProof {
 class Memory:IScreenHookMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public void Load(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++){int p=(seg<<4)+i;byte old;if(Bytes.TryGetValue(p,out old)&&old!=b[i])throw new Exception("Physical alias mismatch");Bytes[p]=b[i];}}
  public ushort ReadWord(ushort s,ushort o){byte a,b;if(!Bytes.TryGetValue((s<<4)+o,out a)||!Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b))throw new Exception("Uncaptured word read");return (ushort)(a|b<<8);}
  public void WriteWord(ushort s,ushort o,ushort v){int p=(s<<4)+o,q=(s<<4)+(ushort)(o+1);if(!Bytes.ContainsKey(p)||!Bytes.ContainsKey(q))throw new Exception("Uncaptured word write");Bytes[p]=(byte)v;Bytes[q]=(byte)(v>>8);}
  public void WriteByte(ushort s,ushort o,byte v){int p=(s<<4)+o;if(!Bytes.ContainsKey(p))throw new Exception("Uncaptured byte write");Bytes[p]=v;}
  public byte ReadByte(ushort s,ushort o){return Bytes[(s<<4)+o];}
  public void OrByte(ushort s,ushort o,byte b){throw new Exception("Video effect");}
  public void Compare(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++)if(Bytes[(seg<<4)+i]!=b[i])throw new Exception("Segment "+seg.ToString("X4")+" byte "+i.ToString("X4"));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}static SignedDivide16State Parse(string[] a,int i){return new SignedDivide16State{AX=H(a[i]),BX=H(a[i+1]),CX=H(a[i+2]),DX=H(a[i+3]),SI=H(a[i+4]),DI=H(a[i+5]),BP=H(a[i+6]),CS=H(a[i+7]),IP=H(a[i+8]),DS=H(a[i+9]),ES=H(a[i+10]),SS=H(a[i+11]),SP=H(a[i+12]),EFlags=Convert.ToUInt32(a[i+13],16)};}
 static string Show(SignedDivide16State s){return s.AX+":"+s.BX+":"+s.CX+":"+s.DX+":"+s.SI+":"+s.DI+":"+s.BP+":"+s.CS+":"+s.IP+":"+s.DS+":"+s.ES+":"+s.SS+":"+s.SP+":"+s.EFlags;}

 static SignedDivide16State State(string d,string n){return Parse(File.ReadAllText(Path.Combine(d,n+"-state.txt")).Split((char[])null,StringSplitOptions.RemoveEmptyEntries),0);}
 static void Check(Memory m,string d,string n,SignedDivide16State s){if(Show(s)!=Show(State(d,n)))throw new Exception(n+" CPU: "+Show(s)+" expected "+Show(State(d,n)));m.Compare(s.DS,File.ReadAllBytes(Path.Combine(d,n+"-ds.bin")));m.Compare(s.SS,File.ReadAllBytes(Path.Combine(d,n+"-ss.bin")));}

 class Effects:IScreenBiosTransferEffects {
  public string Dir;public int Count;
  public SignedDivide16State TransferToBios(IScreenHookMemory memory,SignedDivide16State s){string n="bios-"+Count;Memory m=(Memory)memory;Check(m,Dir,n+"-entry",s);var after=State(Dir,n+"-returned");Memory captured=new Memory();captured.Load(after.DS,File.ReadAllBytes(Path.Combine(Dir,n+"-returned-ds.bin")));captured.Load(after.SS,File.ReadAllBytes(Path.Combine(Dir,n+"-returned-ss.bin")));foreach(var b in captured.Bytes)m.Bytes[b.Key]=b.Value;Count++;return after;}
 }
 static void Add(Memory m,int p,byte[] bytes){for(int i=0;i<bytes.Length;i++){byte old;if(m.Bytes.TryGetValue(p+i,out old)&&old!=bytes[i])throw new Exception("Metadata alias");m.Bytes[p+i]=bytes[i];}}
 static Memory Init(string d,SignedDivide16State s){Memory m=new Memory();m.Load(s.DS,File.ReadAllBytes(Path.Combine(d,"entry-ds.bin")));m.Load(s.SS,File.ReadAllBytes(Path.Combine(d,"entry-ss.bin")));Add(m,0x40,File.ReadAllBytes(Path.Combine(d,"interrupt-vector.bin")));Add(m,0x1f1e0+0x1cc,File.ReadAllBytes(Path.Combine(d,"saved-vector.bin")));return m;}
 static int Main(string[] args){try{string d=args[0];var s=State(d,"entry");Memory m=Init(d,s);var entered=ScreenHookInterrupt.Enter(m,s);Check(m,d,"hook-entry",entered);Console.WriteLine("PASS software INT10 entry: interrupt frame/IF-TF/IP/vector, full registers and DS/SS.");m=Init(d,s);Effects fx=new Effects{Dir=d};var native=new ScreenHookInterruptEffects(fx,null,0x1f1e,0x235,H(args[2]));var result=native.Interrupt(0x10,m,s);if(fx.Count!=int.Parse(args[1]))throw new Exception("BIOS transfer count");Check(m,d,"returned",result);Console.WriteLine("PASS software INT10 + recursive native SCORE hook through BIOS return: full CPU/DS/SS; only BIOS/IRET effects replayed.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
