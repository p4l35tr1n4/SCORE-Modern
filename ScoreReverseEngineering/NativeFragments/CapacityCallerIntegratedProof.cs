using System;using System.IO;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
class CapacityCallerIntegratedProof {
 class Memory:ICapacityStringMemory,ICapacityKeyMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public void Load(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++){int p=(seg<<4)+i;byte old;if(Bytes.TryGetValue(p,out old)&&old!=b[i])throw new Exception("Physical alias mismatch");Bytes[p]=b[i];}}
  public ushort ReadWord(ushort s,ushort o){byte a,b;if(!Bytes.TryGetValue((s<<4)+o,out a)||!Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b))throw new Exception("Uncaptured word read");return (ushort)(a|b<<8);}
  public void WriteWord(ushort s,ushort o,ushort v){int p=(s<<4)+o,q=(s<<4)+(ushort)(o+1);if(!Bytes.ContainsKey(p)||!Bytes.ContainsKey(q))throw new Exception("Uncaptured word write");Bytes[p]=(byte)v;Bytes[q]=(byte)(v>>8);}
  public byte ReadByte(ushort s,ushort o){return Bytes[(s<<4)+o];}
  public void WriteByte(ushort s,ushort o,byte v){int p=(s<<4)+o;if(!Bytes.ContainsKey(p))throw new Exception("Uncaptured byte write");Bytes[p]=v;}
  public void OrByte(ushort s,ushort o,byte b){throw new Exception("Video effect");}
  public void Compare(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++)if(Bytes[(seg<<4)+i]!=b[i])throw new Exception("Segment "+seg.ToString("X4")+" byte "+i.ToString("X4"));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}static SignedDivide16State Parse(string[] a,int i){return new SignedDivide16State{AX=H(a[i]),BX=H(a[i+1]),CX=H(a[i+2]),DX=H(a[i+3]),SI=H(a[i+4]),DI=H(a[i+5]),BP=H(a[i+6]),CS=H(a[i+7]),IP=H(a[i+8]),DS=H(a[i+9]),ES=H(a[i+10]),SS=H(a[i+11]),SP=H(a[i+12]),EFlags=Convert.ToUInt32(a[i+13],16)};}
 static string Show(SignedDivide16State s){return s.AX+":"+s.BX+":"+s.CX+":"+s.DX+":"+s.SI+":"+s.DI+":"+s.BP+":"+s.CS+":"+s.IP+":"+s.DS+":"+s.ES+":"+s.SS+":"+s.SP+":"+s.EFlags;}

 static SignedDivide16State State(string d,string n){return Parse(File.ReadAllText(Path.Combine(d,n+"-state.txt")).Split((char[])null,StringSplitOptions.RemoveEmptyEntries),0);}
 static void Check(Memory m,string d,string n,SignedDivide16State s){if(Show(s)!=Show(State(d,n)))throw new Exception(n+" CPU: "+Show(s)+" expected "+Show(State(d,n)));m.Compare(s.DS,File.ReadAllBytes(Path.Combine(d,n+"-ds.bin")));m.Compare(s.SS,File.ReadAllBytes(Path.Combine(d,n+"-ss.bin")));}

 class Effects:ICapacityInterruptEffects,ICapacityCharacterEffects {
  public string Dir;public int Count;
  SignedDivide16State Replay(string kind,IHorizontalRasterMemory memory,SignedDivide16State s){string n="effect-"+Count;if(File.ReadAllText(Path.Combine(Dir,n+"-kind.txt"))!=kind)throw new Exception("Effect order "+Count);Memory m=(Memory)memory;Check(m,Dir,n+"-entry",s);var after=State(Dir,n+"-returned");Memory captured=new Memory();captured.Load(after.DS,File.ReadAllBytes(Path.Combine(Dir,n+"-returned-ds.bin")));captured.Load(after.SS,File.ReadAllBytes(Path.Combine(Dir,n+"-returned-ss.bin")));foreach(var b in captured.Bytes)m.Bytes[b.Key]=b.Value;Count++;return after;}
  public SignedDivide16State Interrupt(byte number,IHorizontalRasterMemory memory,SignedDivide16State s){return Replay("INT"+number.ToString("X2"),memory,s);}
  public SignedDivide16State CallCharacter(ICapacityStringMemory memory,SignedDivide16State s){return Replay("CHAR",memory,s);}
 }
 static int Main(string[] args){try{string d=args[0];var s=State(d,"comparison");Memory m=new Memory();m.Load(s.DS,File.ReadAllBytes(Path.Combine(d,"comparison-ds.bin")));m.Load(s.SS,File.ReadAllBytes(Path.Combine(d,"comparison-ss.bin")));byte[] header=File.ReadAllBytes(Path.Combine(d,"measured-count.bin"));for(int i=0;i<2;i++){int p=(s.ES<<4)+i;byte old;if(m.Bytes.TryGetValue(p,out old)&&old!=header[i])throw new Exception("Header alias");m.Bytes[p]=header[i];}Effects fx=new Effects{Dir=d};var native=new CapacityMessageNativeEffects(fx,fx,0x1f1e,0x1f1e);var result=BufferedPointCaller.ExecuteCapacityAtComparisonThroughReturn(m,native,s,0x1f1e,0x1dbb);if(result.Outcome!=BufferedPointOutcome.Completed||!result.RetfExecuted||fx.Count!=int.Parse(args[1]))throw new Exception("Outcome/effects");Check(m,d,"caller-returned",result.State);Console.WriteLine("PASS native point-caller capacity comparison through return, nested message/follow-up/callees; full CPU/DS/SS with explicit BIOS/character effects.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}} }
