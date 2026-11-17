using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class NumericInstructionProof {
 class Mem:INumericConverterContext {
  public Dictionary<int,byte> Bytes=new Dictionary<int,byte>();
  public byte ReadByte(ushort s,ushort p){byte b;return Bytes.TryGetValue((s<<4)+p,out b)?b:(byte)0;}
  public void WB(ushort s,ushort p,byte b){Bytes[(s<<4)+p]=b;}
  public ushort RW(ushort s,ushort p){return (ushort)(ReadByte(s,p)|(ReadByte(s,(ushort)(p+1))<<8));}
  public void WriteWord(ushort s,ushort p,ushort v){WB(s,p,(byte)v);WB(s,(ushort)(p+1),(byte)(v>>8));}
  public Mem Copy(){Mem m=new Mem();foreach(var x in Bytes)m.Bytes[x.Key]=x.Value;return m;}
 }
 class Cpu {
  public ushort AX=0x1357,CX=0x9876,DX=0x2468,BX=0xabcd,SP=0xe000,BP=0xbeef,SI=1,DI=0x7654,DS=0x3000,SS=0x4000,IP=0x1bd7;
  public bool C=true,Z=false,O=true,D=true;byte[] code;Mem m;
  public Cpu(byte[] c,Mem memory){code=c;m=memory;}
  byte F(){int i=IP++-0x1bd7;if(i<0||i>=code.Length)throw new Exception("Code escape "+IP.ToString("X4"));return code[i];}
  ushort W(){byte a=F();return (ushort)(a|(F()<<8));}
  byte AL {get{return (byte)AX;}set{AX=(ushort)((AX&0xff00)|value);}}
  void Push(ushort x){SP=(ushort)(SP-2);m.WriteWord(SS,SP,x);}
  ushort Pop(){ushort x=m.RW(SS,SP);SP=(ushort)(SP+2);return x;}
  ushort Add(ushort a,ushort b,bool carry){uint s=(uint)a+b+(carry?1u:0u);ushort r=(ushort)s;C=s>65535;Z=r==0;O=((~(a^b)&(a^r))&0x8000)!=0;return r;}
  ushort Sub(ushort a,ushort b,int bits){int mask=bits==8?255:65535,sign=bits==8?128:32768;ushort r=(ushort)((a-b)&mask);C=a<b;Z=r==0;O=(((a^b)&(a^r))&sign)!=0;return r;}
  void Logic(ushort x){C=O=false;Z=x==0;}
  void Jump(bool yes){sbyte d=(sbyte)F();if(yes)IP=(ushort)(IP+d);}
  public void Run(){for(int k=0;k<10000;k++){
   if(IP==0x7f00)return;
   byte op=F(),q;ushort v,old;bool saved;uint product;
   switch(op){
    case 0x55:Push(BP);break;case 0x56:Push(SI);break;case 0x57:Push(DI);break;case 0x52:Push(DX);break;case 0x1e:Push(DS);break;
    case 0x59:CX=Pop();break;case 0x1f:DS=Pop();break;case 0x5f:DI=Pop();break;case 0x5e:SI=Pop();break;case 0x5d:BP=Pop();break;
    case 0xfc:D=false;break;
    case 0x8b:q=F();switch(q){case 0xec:BP=SP;break;case 0xd7:DX=DI;break;case 0xc3:AX=BX;break;case 0xc7:AX=DI;break;case 0xfa:DI=DX;break;case 0xcb:CX=BX;break;case 0xe5:SP=BP;break;default:throw new Exception("MOV "+q.ToString("X2"));}break;
    case 0x33:q=F();switch(q){case 0xff:DI=0;break;case 0xdb:BX=0;break;case 0xd2:DX=0;break;case 0xc9:CX=0;break;default:throw new Exception("XOR");}Logic(0);break;
    case 0xc5:q=F();if(q!=0x76)throw new Exception("LDS");v=(ushort)(BP+(sbyte)F());SI=m.RW(SS,v);DS=m.RW(SS,(ushort)(v+2));break;
    case 0xac:AL=m.ReadByte(DS,SI);SI=(ushort)(SI+(D?-1:1));break;
    case 0x3c:Sub(AL,F(),8);break;
    case 0x74:Jump(Z);break;case 0x75:Jump(!Z);break;case 0x72:Jump(C);break;case 0x73:Jump(!C);break;case 0x70:Jump(O);break;case 0x77:Jump(!C&&!Z);break;case 0xe3:Jump(CX==0);break;
    case 0x4a:saved=C;DX=Sub(DX,1,16);C=saved;break;
    case 0x4e:saved=C;SI=Sub(SI,1,16);C=saved;break;
    case 0xb9:CX=W();break;
    case 0x2c:AL=(byte)Sub(AL,F(),8);break;
    case 0x98:AX=unchecked((ushort)(short)(sbyte)AL);break;
    case 0x93:v=AX;AX=BX;BX=v;break;
    case 0xf7:q=F();if(q==0xe1){product=(uint)AX*CX;AX=(ushort)product;DX=(ushort)(product>>16);C=O=DX!=0;}else if(q==0xd8){old=AX;AX=Sub(0,AX,16);C=old!=0;}else if(q==0xda){old=DX;DX=Sub(0,DX,16);C=old!=0;}else throw new Exception("F7");break;
    case 0x03:q=F();if(q==0xd8)BX=Add(BX,AX,false);else if(q==0xf8)DI=Add(DI,AX,false);else throw new Exception("ADD");break;
    case 0x13:if(F()!=0xfa)throw new Exception("ADC");DI=Add(DI,DX,C);break;
    case 0x83:q=F();v=unchecked((ushort)(short)(sbyte)F());if(q==0xd2)DX=Add(DX,v,C);else if(q==0xf9)Sub(CX,v,16);else if(q==0xfb)Sub(BX,v,16);else throw new Exception("83");break;
    case 0x80:q=F();if(q==0x3c)Sub(m.ReadByte(DS,SI),F(),8);else if(q==0xf9)Sub((byte)CX,F(),8);else throw new Exception("80");break;
    case 0x89:if(F()!=0x0c)throw new Exception("Store");m.WriteWord(DS,SI,CX);break;
    case 0xe8:v=W();Push(IP);IP=(ushort)(IP+(short)v);break;
    case 0xeb:Jump(true);break;
    case 0xe9:v=W();IP=(ushort)(IP+(short)v);break;
    case 0xc2:v=W();IP=Pop();SP=(ushort)(SP+v);break;
    case 0xc3:IP=Pop();break;
    case 0x24:AL=(byte)(AL&F());Logic(AL);break;
    case 0x3a:if(F()!=0xc1)throw new Exception("CMP");Sub(AL,(byte)CX,8);break;
    case 0xf5:C=!C;break;
    default:throw new Exception("Unsupported opcode "+op.ToString("X2")+" at "+(IP-1).ToString("X4"));
   }
  }throw new Exception("Instruction limit");}
 }
 static byte[] code;static int cases;
 static void Check(byte[] input,int layout){
  ushort seg=0x2000,ptr=layout==2?(ushort)0xfff8:(ushort)0x1000,status=layout==1?(ushort)(ptr+1):(ushort)0x3000;
  Mem seed=new Mem();seed.WriteWord(seg,status,0xa55a);
  for(int i=0;i<input.Length;i++)seed.WB(seg,(ushort)(ptr+i),input[i]);seed.WB(seg,(ushort)(ptr+input.Length),0);
  seed.WriteWord(0x4000,0xe000,0x7f00);seed.WriteWord(0x4000,0xe002,status);seed.WriteWord(0x4000,0xe004,seg);seed.WriteWord(0x4000,0xe006,ptr);seed.WriteWord(0x4000,0xe008,seg);
  Mem native=seed.Copy(),reference=seed.Copy();Cpu cpu=new Cpu(code,reference);cpu.Run();NumericConverterResult result=NumericConverter.Execute(native,seg,status,seg,ptr);
  if(result.AX!=cpu.AX||result.DX!=cpu.DX||result.Status!=reference.RW(seg,status))throw new Exception("Result mismatch "+BitConverter.ToString(input)+" AX "+result.AX.ToString("X4")+"/"+cpu.AX.ToString("X4")+" DX "+result.DX.ToString("X4")+"/"+cpu.DX.ToString("X4"));
  if(cpu.SP!=0xe00a||cpu.DS!=0x3000)throw new Exception("Reference ABI boundary");
  foreach(var x in native.Bytes){byte b;if(!reference.Bytes.TryGetValue(x.Key,out b)||b!=x.Value)throw new Exception("Memory mismatch");}
  foreach(var x in reference.Bytes){if(x.Key>=0x40000+0xdff4&&x.Key<0x40000+0xe000)continue;byte b;if(!native.Bytes.TryGetValue(x.Key,out b)||b!=x.Value)throw new Exception("Extra data write");}
  cases++;
 }
 static void Text(string s,int layout){Check(Encoding.ASCII.GetBytes(s),layout);}
 static void Main(string[] args){
  byte[] image=File.ReadAllBytes(args[0]);code=new byte[215];Array.Copy(image,0x1f080+0x1bd7,code,0,215);
  byte[] captured=File.ReadAllBytes(args[1]);if(captured.Length!=215)throw new Exception("Code capture length");for(int i=0;i<215;i++)if(code[i]!=captured[i])throw new Exception("Original code mismatch");
  for(int a=0;a<256;a++){Check(new byte[]{(byte)a},a%3);for(int b=0;b<256;b++)Check(new byte[]{(byte)a,(byte)b},(a+b)%3);}
  for(int n=-65536;n<=65536;n++)Text(n.ToString(System.Globalization.CultureInfo.InvariantCulture),(n&0x7fffffff)%3);
  for(int radix=0;radix<=40;radix++)for(int c=0;c<256;c++){byte[] prefix=Encoding.ASCII.GetBytes(radix.ToString()+"#");byte[] input=new byte[prefix.Length+1];Array.Copy(prefix,input,prefix.Length);input[input.Length-1]=(byte)c;Check(input,c%3);}
  foreach(string s in new string[]{"","+","-","#","16#FF","16#ff","0#FF","2#1010","1#1","37#1","258#1","10#1#1","16#1#1","2147483647","2147483648","-2147483648","4294967295","4294967296","999999999999999","12X","12!","65536","655360","0xFF"})for(int i=0;i<3;i++)Text(s,i);
  Random random=new Random(7831);for(int n=0;n<5000;n++){byte[] input=new byte[random.Next(0,17)];random.NextBytes(input);Check(input,n%3);}
  Console.WriteLine("PASS "+cases+" original-instruction comparisons; exact 215-byte identity; AX/DX, status and non-stack memory. Includes all one/two-byte strings, decimal -65536..65536, bases 0..40, malformed/large strings, aliasing and input-offset wrap.");
  Console.WriteLine("Scope: model comparison, not original DOS execution for every input. Full flags/register/stack ABI and application integration remain outside the native interface.");
 }
}
