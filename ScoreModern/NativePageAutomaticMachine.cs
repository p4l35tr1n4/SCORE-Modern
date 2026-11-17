using System;
using System.Collections.Generic;
// PAGE automatic-layout computation projection. Original entry/return arrays
// and retained scratch verified in the natural test and twelve finite replays.
// Description formatting, DOS output and runtime ABI are separate operations.
namespace ScoreModern {
partial class NativePageAutomaticMachine {
 internal readonly byte[] Memory=new byte[1048576];readonly bool[] loaded=new bool[1048576];
 internal ushort AX,BX,CX,DX,SI,DI,BP,DS,ES,SS,SP,CS,IP;internal bool ZF,SF,OF,DF;internal int Steps;readonly Stack<double> fp=new Stack<double>();
 internal void Load(int segment,byte[] data){int start=segment*16;for(int n=0;n<data.Length;n++){int at=(start+n)&0xfffff;Memory[at]=data[n];loaded[at]=true;}}
 int Address(int seg,int off){return ((seg<<4)+(off&65535))&0xfffff;}
 byte M8(int seg,int off){int a=Address(seg,off);if(!loaded[a])throw new Exception("Uncaptured read "+seg.ToString("x4")+":"+(off&65535).ToString("x4"));return Memory[a];}
 ushort M16(int seg,int off){return (ushort)(M8(seg,off)|(M8(seg,off+1)<<8));}
 void M8(int seg,int off,int value){int a=Address(seg,off);Memory[a]=(byte)value;loaded[a]=true;}
 void M16(int seg,int off,int value){M8(seg,off,value);M8(seg,off+1,value>>8);}
 float M32(int seg,int off){byte[] b={M8(seg,off),M8(seg,off+1),M8(seg,off+2),M8(seg,off+3)};return BitConverter.ToSingle(b,0);}
 void M32(int seg,int off,double value){byte[] b=BitConverter.GetBytes((float)value);for(int n=0;n<4;n++)M8(seg,off+n,b[n]);}
 ushort AL{get{return (byte)AX;}set{AX=(ushort)((AX&0xff00)|(value&255));}}
 ushort AH{get{return (ushort)(AX>>8);}set{AX=(ushort)((AX&255)|((value&255)<<8));}}
 ushort CL{get{return (byte)CX;}set{CX=(ushort)((CX&0xff00)|(value&255));}}
 void Push(int value){SP=(ushort)(SP-2);M16(SS,SP,value);}ushort Pop(){ushort value=M16(SS,SP);SP=(ushort)(SP+2);return value;}
 ushort Add(int a,int b){int result=(a+b)&65535;ZF=result==0;SF=(result&32768)!=0;OF=((~(a^b)&(a^result))&32768)!=0;return (ushort)result;}
 ushort Sub(int a,int b){int result=(a-b)&65535;ZF=result==0;SF=(result&32768)!=0;OF=(((a^b)&(a^result))&32768)!=0;return (ushort)result;}
 ushort Shl(int a,int count){int result=a;for(int n=0;n<count;n++){int old=result;result=(result<<1)&65535;OF=((old^result)&32768)!=0;}ZF=result==0;SF=(result&32768)!=0;return (ushort)result;}
 ushort Sar(int a,int count){short result=(short)a;for(int n=0;n<count;n++)result=(short)(result>>1);OF=false;ZF=result==0;SF=result<0;return (ushort)result;}
 void Divide(int value){int divisor=(short)value;if(divisor==0)throw new Exception("Original divide fault");int dividend=unchecked((int)(((uint)DX<<16)|AX));long q=(long)dividend/divisor;if(q<short.MinValue||q>short.MaxValue)throw new Exception("Original quotient overflow");AX=(ushort)q;DX=(ushort)(dividend%divisor);}
 void Move(int size){for(int n=0;n<size;n++)M8(ES,DI+n,M8(DS,SI+n));int delta=DF?-size:size;SI=(ushort)(SI+delta);DI=(ushort)(DI+delta);}
 void External(int segment,int offset){
  if(segment==0x23b3&&offset==0xc50){double v=fp.Pop();if(Double.IsNaN(v)||Double.IsInfinity(v)||v<Int64.MinValue||v>=9223372036854775808.0)throw new Exception("Unverified original integer conversion");AX=unchecked((ushort)(long)Math.Truncate(v));return;}
  if(segment==0x29bb&&offset==3)return; // printf; caller explicitly removes its arguments.
  if(segment==0x29bb&&offset==0x2b66)return; // Description-string formatter: analysis projection excludes its output/heap/ABI.
  if(segment==0xff5&&offset==0x1377)return; // output heading; no notation mutations.
  throw new Exception("Unverified external PAGE call "+segment.ToString("x4")+":"+offset.ToString("x4")+" at "+CS.ToString("x4")+":"+IP.ToString("x4"));
 }
 internal void Run(int limit=2000000){for(Steps=0;Steps<limit;Steps++)if(!Instruction())return;throw new Exception("PAGE translated instruction limit");}
}

}
