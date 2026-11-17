using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class PointBufferFlagsRuntimeProof {
 class Memory:IPointBufferMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<string> Events=new List<string>();
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){byte a,b;Bytes.TryGetValue((s<<4)+o,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add("R:"+s+":"+o+":"+v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add("W:"+s+":"+o+":"+v);Seed(s,o,v);}
  public Memory Copy(){Memory m=new Memory();foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(PointBufferWords w){return w.AX+":"+w.BX+":"+w.SI+":"+w.ES+":"+w.EFlags;}
 static PointBufferWords Interpret(byte[] code,Memory m,ushort ds,ushort ss,ushort bp){int pc=0;ushort ax=0,bx=0,si=0,es=0;bool nonpositive=false;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length){switch(f()){
  case 0x8e:if(f()!=6)throw new Exception("MOV ES");es=m.ReadWord(ds,word());break;
  case 0x26:switch(f()){case 0xa1:ax=m.ReadWord(es,word());break;case 0xa3:m.WriteWord(es,word(),ax);break;case 0x8b:if(f()!=0x37)throw new Exception("MOV SI ES");si=m.ReadWord(es,bx);break;case 0x89:if(f()!=0x47)throw new Exception("MOV ES AX");sbyte disp=(sbyte)f();m.WriteWord(es,(ushort)(bx+disp),ax);break;default:throw new Exception("ES opcode");}break;
  case 0xa3:m.WriteWord(ds,word(),ax);break;case 0xa1:ax=m.ReadWord(ds,word());break;
  case 0xc4:if(f()!=0x5e)throw new Exception("LES");ushort at=(ushort)(bp+(sbyte)f()),offset=m.ReadWord(ss,at),segment=m.ReadWord(ss,(ushort)(at+2));bx=offset;es=segment;break;
  case 0x0b:if(f()!=0xf6)throw new Exception("OR");nonpositive=(short)si<=0;break;case 0x7e:sbyte d=(sbyte)f();if(nonpositive)pc+=d;break;
  case 0x8b:byte mov=f();if(mov==0xd8)bx=ax;else if(mov==0x36)si=m.ReadWord(ds,word());else throw new Exception("MOV");break;
  case 0xd1:byte shift=f();if(shift==0xe3)bx=(ushort)(bx<<1);else if(shift==0xe6)si=(ushort)(si<<1);else throw new Exception("SHL");break;
  case 0x8d:byte lea=f();ushort imm=word();if(lea==0x9f)bx=(ushort)(bx+imm);else if(lea==0x84)ax=(ushort)(si+imm);else if(lea==0x9c)bx=(ushort)(si+imm);else throw new Exception("LEA");break;
  case 0xff:if(f()!=6)throw new Exception("INC memory");ushort address=word();m.WriteWord(ds,address,(ushort)(m.ReadWord(ds,address)+1));break;case 0x40:ax=(ushort)(ax+1);break;
  default:throw new Exception("Opcode");
 }}return new PointBufferWords{AX=ax,BX=bx,SI=si,ES=es};}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[93];Array.Copy(raw,0xc250+0x7eb,code,0,93);if(code[0]!=0x8e||code[89]!=0x26||code[90]!=0xa3)throw new Exception("Code boundary");
  if(args.Length>1){string p=args[1];ushort ds=H(args[2]),ss=H(args[3]),bp=H(args[4]),first=H(args[5]),second=H(args[6]);byte[] live=File.ReadAllBytes(Path.Combine(p,"append-code.bin"));if(live.Length!=93)throw new Exception("Code length");for(int i=0;i<93;i++)if(live[i]!=code[i])throw new Exception("Live code");Memory m=new Memory();var segments=new HashSet<ushort>(new ushort[]{ds,first,second});if(ss!=ds)throw new Exception("Fixture requires recorded SS/DS alias");foreach(ushort seg in segments){string file=seg==ds?"append-before-ds.bin":"append-before-"+seg.ToString("x4")+".bin";byte[] b=File.ReadAllBytes(Path.Combine(p,file));if(b.Length!=65536)throw new Exception("Segment length");for(int i=0;i<65536;i++){int at=(seg<<4)+i;byte old;if(m.Bytes.TryGetValue(at,out old)&&old!=b[i])throw new Exception("Before alias inconsistency");m.Bytes[at]=b[i];}}
   PointBufferWords r=PointBufferAppend.ExecuteWithFlags(m,ds,ss,bp,Convert.ToUInt32(args[11],16));if(Show(r)!=Show(new PointBufferWords{AX=H(args[7]),BX=H(args[8]),SI=H(args[9]),ES=H(args[10]),EFlags=Convert.ToUInt32(args[12],16)}))throw new Exception("Runtime words");int compared=0;foreach(ushort seg in segments){string file=seg==ds?"append-after-ds.bin":"append-after-"+seg.ToString("x4")+".bin";byte[] b=File.ReadAllBytes(Path.Combine(p,file));for(int i=0;i<65536;i++)if(m.Bytes[(seg<<4)+i]!=b[i])throw new Exception("Runtime memory");compared+=65536;}Console.WriteLine("PASS original buffer append: 93 live code bytes, AX/BX/SI/ES/full EFLAGS and "+compared+" captured segment bytes including physical aliases, zero exclusions.");return 0;}
  int count=0;for(int n=0;n<65536;n++)for(int layout=0;layout<8;layout++){ushort ds=0x2000,ss=(ushort)(layout==6?0x2000:0x3000),bp=0x8000,first=(ushort)(layout==0||layout==7?0x4000:ds),second=(ushort)(layout==0?0x5000:ds),markerSeg=(ushort)(layout==6?ds:0x6000),markerOff=(ushort)(layout==6?0xc58e:0x10),oldCount=new ushort[]{1,0,0x62c7,0x62c8,0x34ed,0xffff,0x1234,0xffff}[layout];Memory seed=new Memory();seed.Seed(ds,0x69da,first);seed.Seed(ds,0x69dc,second);seed.Seed(ds,0xc590,(ushort)(n^0xa55a));seed.Seed(ds,0xc594,(ushort)(n*17+1));seed.Seed(first,0,oldCount);seed.Seed(ss,(ushort)(bp+6),markerOff);seed.Seed(ss,(ushort)(bp+8),markerSeg);seed.Seed(markerSeg,markerOff,(ushort)n);Memory a=seed.Copy(),b=seed.Copy();PointBufferWords x=PointBufferAppend.Execute(a,ds,ss,bp),y=Interpret(code,b,ds,ss,bp);if(Show(x)!=Show(y)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model result/extent");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Order "+i);foreach(var e in a.Bytes)if(b.Bytes[e.Key]!=e.Value)throw new Exception("Model memory");count++;}
  Console.WriteLine("PASS "+count+" original-instruction buffer-append comparisons: every command word, signed branch, eight count/pointer/data/stack alias layouts, wrapping and ordered reads/writes.");Console.WriteLine("Scope: integer words/memory only, after conversion/clamps. No marker operation names, float conversion, capacity policy, full flags/ABI or renderer.");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
