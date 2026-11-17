using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class EqualStepRasterProof {
 class Memory:IHorizontalRasterMemory {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();public List<long> Events=new List<long>();public int Pattern;
  public void Seed(ushort s,ushort o,ushort v){Bytes[(s<<4)+o]=(byte)v;Bytes[(s<<4)+(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){int p=(s<<4)+o;byte a,b;Bytes.TryGetValue(p,out a);Bytes.TryGetValue((s<<4)+(ushort)(o+1),out b);ushort v=(ushort)(a|b<<8);Events.Add((1L<<56)|((long)p<<16)|v);return v;}
  public void WriteWord(ushort s,ushort o,ushort v){Events.Add((2L<<56)|((long)((s<<4)+o)<<16)|v);Seed(s,o,v);}
  public void OrByte(ushort s,ushort o,byte mask){int p=(s<<4)+o;byte old;if(!Bytes.TryGetValue(p,out old))old=(byte)((p*37+Pattern*13)^(p>>8));Bytes[p]=(byte)(old|mask);Events.Add((3L<<56)|((long)p<<16)|mask);}
  public Memory Copy(){Memory m=new Memory{Pattern=Pattern};foreach(var p in Bytes)m.Bytes[p.Key]=p.Value;return m;}
 }
 static string Show(VerticalRasterWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP;}
 static VerticalRasterWords Interpret(byte[] code,IHorizontalRasterMemory m,ushort ds,ushort es,ushort ax,ushort cx,ushort dx,ushort si){int pc=0,steps=0;ushort bx=0,di=0,bp=0;bool less=false,equal=false;Func<byte> f=()=>code[pc++];Func<ushort> word=()=>{byte l=f();return (ushort)(l|f()<<8);};while(pc<code.Length&&steps++<2000000){switch(f()){
 case 0x8b:byte op=f();if(op==0xd8)bx=ax;else if(op==0xe8)bp=ax;else if(op==0xbf)di=m.ReadWord(ds,(ushort)(bx+word()));else if(op==0x0e)cx=m.ReadWord(ds,word());else throw new Exception("MOV");break;
 case 0xd1:if(f()!=0xe3)throw new Exception("SHL");bx=(ushort)(bx<<1);break;
 case 0xa1:ax=m.ReadWord(ds,word());break;case 0xb1:cx=(ushort)((cx&0xff00)|f());break;
 case 0xf6:if(f()!=0xf1)throw new Exception("DIV opcode");int v=cx&255;if(v==0||ax/v>255)throw new Exception("Excluded fault");ax=(ushort)((ax/v)|((ax%v)<<8));break;
 case 0x8a:if(f()!=0xcc)throw new Exception("MOV CL");cx=(ushort)((cx&0xff00)|(ax>>8));break;
 case 0xb2:dx=(ushort)((dx&0xff00)|f());break;
 case 0xd2:if(f()!=0xca)throw new Exception("ROR");for(int n=0;n<(cx&255);n++){byte b=(byte)dx;dx=(ushort)((dx&0xff00)|(b>>1)|((b&1)<<7));}break;
 case 0xd0:byte rot=f(),r=(byte)dx;if(rot==0xca)dx=(ushort)((dx&0xff00)|(r>>1)|((r&1)<<7));else if(rot==0xc2)dx=(ushort)((dx&0xff00)|((r<<1)&255)|(r>>7));else throw new Exception("Rotate1");break;
 case 0x2a:if(f()!=0xe4)throw new Exception("SUB AH");ax=(ushort)(ax&255);break;
 case 0x3b:if(f()!=6)throw new Exception("CMP word");ushort rhs=m.ReadWord(ds,word());less=(short)ax<(short)rhs;equal=ax==rhs;break;
 case 0x7f:sbyte jg=(sbyte)f();if(!less&&!equal)pc+=jg;break;case 0x75:sbyte jne=(sbyte)f();if(!equal)pc+=jne;break;
 case 0x41:cx=(ushort)(cx+1);break;case 0x45:bp=(ushort)(bp+1);break;case 0x4d:bp=(ushort)(bp-1);break;
 case 0x03:byte add=f();if(add==0xfd)di=(ushort)(di+bp);else if(add==0x1e)bx=(ushort)(bx+m.ReadWord(ds,word()));else throw new Exception("ADD");break;
 case 0x26:if(f()!=8||f()!=0x15)throw new Exception("OR");m.OrByte(es,di,(byte)dx);break;
 case 0x80:if(f()!=0xfa)throw new Exception("CMP byte");equal=(byte)dx==f();break;
 case 0xe2:sbyte d=(sbyte)f();cx=(ushort)(cx-1);if(cx!=0)pc+=d;break;
 case 0xea:if(word()!=0xb16||word()!=0x1807)throw new Exception("Far exit");pc=code.Length;break;
 case 0xe9:short jump=(short)word();if(0xa3d+pc+jump!=0xb16)throw new Exception("Near exit");pc=code.Length;break;
 default:throw new Exception("Opcode at "+(pc-1));
 }}if(pc!=code.Length)throw new Exception("Exit");return new VerticalRasterWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,BP=bp};}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 class Captured:IHorizontalRasterMemory {public byte[] Data,Plane;public ushort DS,ES;public int Writes;public ushort ReadWord(ushort s,ushort o){if(s!=DS)throw new Exception("DS");return (ushort)(Data[o]|Data[(ushort)(o+1)]<<8);}public void WriteWord(ushort s,ushort o,ushort v){if(s!=DS)throw new Exception("DS write");Data[o]=(byte)v;Data[(ushort)(o+1)]=(byte)(v>>8);}public void OrByte(ushort s,ushort o,byte v){if(s!=ES)throw new Exception("Video segment");Plane[o]|=v;Writes++;}}
 static int Main(string[] args){try{byte[] raw=File.ReadAllBytes(args[0]),code=new byte[89];Array.Copy(raw,0x18070+0xa3d,code,0,89);
 if(args.Length>1){string p=args[1];byte[] live=File.ReadAllBytes(Path.Combine(p,"raster-code.bin")),relocated=(byte[])code.Clone();foreach(int at in new[]{0xa77}){int i=at-0xa3d+3;relocated[i]=0x2b;relocated[i+1]=0x20;}if(live.Length!=89)throw new Exception("Live length");for(int i=0;i<89;i++)if(live[i]!=relocated[i])throw new Exception("Live code");Captured m=new Captured{DS=H(args[2]),ES=H(args[3]),Data=File.ReadAllBytes(Path.Combine(p,"raster-before-ds.bin")),Plane=File.ReadAllBytes(Path.Combine(p,"raster-before-plane2.bin"))};var r=EqualStepRaster.Execute(m,m.DS,m.ES,H(args[4]),H(args[5]),H(args[6]),H(args[7]));var e=new VerticalRasterWords{AX=H(args[8]),BX=H(args[9]),CX=H(args[10]),DX=H(args[11]),SI=H(args[12]),DI=H(args[13]),BP=H(args[14])};if(Show(r)!=Show(e))throw new Exception("Runtime words "+Show(r)+" != "+Show(e));byte[] after=File.ReadAllBytes(Path.Combine(p,"raster-after-ds.bin")),plane=File.ReadAllBytes(Path.Combine(p,"raster-after-plane2.bin"));if(m.Data.Length!=65536||m.Plane.Length!=65536||after.Length!=65536||plane.Length!=65536)throw new Exception("Capture lengths");for(int i=0;i<65536;i++)if(m.Data[i]!=after[i]||m.Plane[i]!=plane[i])throw new Exception("Runtime memory "+i);Console.WriteLine("PASS original equal-step block: 89 relocated code bytes, seven result words, all 65536 DS and 65536 plane2 bytes; "+m.Writes+" explicit plane2 OR effects.");return 0;}
 int count=0;for(int x=0;x<640;x++)for(int k=0;k<32;k++){ushort ds=0x2000,es=(ushort)((k&1)==0?0x4000:ds),row=(ushort)(k*1021),cx=(ushort)(x^0xa55a),dx=(ushort)(k*701),si=(ushort)(x*13);Memory seed=new Memory{Pattern=k};seed.Seed(ds,0x4b68,(ushort)x);seed.Seed(ds,0x4b6c,(ushort)(x+((k&4)==0?3:-3)));seed.Seed(ds,0x4b72,(ushort)(k%16));seed.Seed(ds,0x4b78,(ushort)((k&2)==0?2:0xfffe));Memory a=seed.Copy(),b=seed.Copy();var n=EqualStepRaster.Execute(a,ds,es,row,cx,dx,si);var q=Interpret(code,b,ds,es,row,cx,dx,si);if(Show(n)!=Show(q)||a.Events.Count!=b.Events.Count||a.Bytes.Count!=b.Bytes.Count)throw new Exception("Model state");for(int i=0;i<a.Events.Count;i++)if(a.Events[i]!=b.Events[i])throw new Exception("Ordered effect");foreach(var v in a.Bytes)if(b.Bytes[v.Key]!=v.Value)throw new Exception("Model memory");count++;}
 Console.WriteLine("PASS "+count+" instruction-model cases: 640 columns, both direction branches, masks, word wrapping, row strides and data/video aliases. Ordinary RAM OR model; successful DIV paths only.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
