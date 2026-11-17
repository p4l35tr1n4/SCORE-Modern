using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class LineDriverBodyProof {
 class Memory:IHorizontalRasterMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public byte[] Plane;public ushort ES;public int OrEffects;
  public void Load(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Capture length");for(int i=0;i<65536;i++){int p=(seg<<4)+i;byte old;if(Bytes.TryGetValue(p,out old)&&old!=b[i])throw new Exception("DS/SS alias mismatch");Bytes[p]=b[i];}}
  public ushort ReadWord(ushort s,ushort o){byte a,b;if(!Bytes.TryGetValue((s<<4)+o,out a)||!Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b))throw new Exception("Uncaptured word read");return (ushort)(a|b<<8);}
  public void WriteWord(ushort s,ushort o,ushort v){int p=(s<<4)+o,q=(s<<4)+(ushort)(o+1);if(!Bytes.ContainsKey(p)||!Bytes.ContainsKey(q))throw new Exception("Uncaptured word write");Bytes[p]=(byte)v;Bytes[q]=(byte)(v>>8);}
  public void OrByte(ushort s,ushort o,byte mask){if(s!=ES)throw new Exception("Unverified video segment");Plane[o]|=mask;OrEffects++;}
  public void Compare(ushort seg,byte[] b){if(b.Length!=65536)throw new Exception("Length");for(int i=0;i<65536;i++)if(Bytes[(seg<<4)+i]!=b[i])throw new Exception("Segment byte "+i.ToString("X4"));}
 }
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static SignedDivide16State Parse(string[] a,int i){return new SignedDivide16State{AX=H(a[i]),BX=H(a[i+1]),CX=H(a[i+2]),DX=H(a[i+3]),SI=H(a[i+4]),DI=H(a[i+5]),BP=H(a[i+6]),CS=H(a[i+7]),IP=H(a[i+8]),DS=H(a[i+9]),ES=H(a[i+10]),SS=H(a[i+11]),SP=H(a[i+12]),EFlags=Convert.ToUInt32(a[i+13],16)};}
 static string Words(SignedDivide16State s){return s.AX+":"+s.BX+":"+s.CX+":"+s.DX+":"+s.SI+":"+s.DI+":"+s.BP+":"+s.CS+":"+s.IP+":"+s.DS+":"+s.ES+":"+s.SS+":"+s.SP;}
 static int Main(string[] a){try{string prefix=Path.Combine(a[0],"case-"+a[1]);var before=Parse(a,2);var after=Parse(a,16);Memory m=new Memory{ES=H(a[30]),Plane=File.ReadAllBytes(prefix+"-before-plane2.bin")};if(m.Plane.Length!=65536)throw new Exception("Plane length");m.Load(before.DS,File.ReadAllBytes(prefix+"-before-ds.bin"));m.Load(before.SS,File.ReadAllBytes(prefix+"-before-ss.bin"));bool returned=a.Length>31&&a[31]=="return";var result=returned?LineDriverBody.ExecuteThroughReturn(m,before):LineDriverBody.Execute(m,before);if(result.RetfExecuted!=returned)throw new Exception("Return projection");if(result.Fault!=SignedDivide16Fault.None||Words(result.State)!=Words(after))throw new Exception("Body words "+Words(result.State)+" != "+Words(after));if(((result.State.EFlags^after.EFlags)&result.DefinedEFlagsMask)!=0)throw new Exception("Defined flag bits");m.Compare(before.DS,File.ReadAllBytes(prefix+(returned?"-returned-ds.bin":"-after-ds.bin")));m.Compare(before.SS,File.ReadAllBytes(prefix+(returned?"-returned-ss.bin":"-after-ss.bin")));byte[] plane=File.ReadAllBytes(prefix+(returned?"-returned-plane2.bin":"-after-plane2.bin"));if(plane.Length!=65536)throw new Exception("Plane length");for(int i=0;i<65536;i++)if(plane[i]!=m.Plane[i])throw new Exception("Plane byte "+i.ToString("X4"));Console.WriteLine("PASS original full line body case "+a[1]+": all 13 word registers/PC, preserved control flags, 196608 DS/SS/plane view bytes; clip="+result.ClippingExit.ToString("X4")+", raster="+result.RasterEntry.ToString("X4")+", OR effects="+m.OrEffects+". Raster arithmetic flags and general VGA bus remain outside projection; RETF="+returned+".");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
