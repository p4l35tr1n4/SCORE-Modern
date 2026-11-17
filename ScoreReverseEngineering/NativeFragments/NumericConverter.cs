using System;
namespace ScoreModern.RecoveredFragments {
 public interface INumericConverterContext {
  byte ReadByte(ushort segment,ushort offset);
  void WriteWord(ushort segment,ushort offset,ushort value);
 }
 public struct NumericConverterResult {
  public readonly ushort AX,DX,Status;
  public NumericConverterResult(ushort ax,ushort dx,ushort status){AX=ax;DX=dx;Status=status;}
 }
 public static class NumericConverter {
  // Literal data/result translation of original 1F08:1BD7 and 1C95.
  // Explicit segmented memory; no host number parser or ABI adapter.
  static byte Load(INumericConverterContext m,ushort segment,ref ushort si){byte b=m.ReadByte(segment,si);si=unchecked((ushort)(si+1));return b;}
  static ushort Classify(ref byte al,byte radix,out bool carry){
   if(al>=0x61)al=(byte)(al&0xdf);
   byte old=al;al=unchecked((byte)(al-0x30));carry=old<0x30;
   if(carry)return unchecked((ushort)(short)(sbyte)al);
   if(al>=0x0a){
    if(al<0x11){carry=true;return unchecked((ushort)(short)(sbyte)al);}
    al=unchecked((byte)(al-7));
   }
   carry=!(al<radix);return unchecked((ushort)(short)(sbyte)al);
  }
  public static NumericConverterResult Execute(INumericConverterContext m,ushort statusSegment,ushort statusOffset,ushort inputSegment,ushort inputOffset){
   if(m==null)throw new ArgumentNullException("m");
   ushort si=inputOffset,bx=0,di=0,cx=10,status=0,ax=0,oldHigh=0,carryWord=0,addend=0,prior=0;
   uint product=0,sum=0;bool negative=false,carry=false;byte al=Load(m,inputSegment,ref si),old=0;
   if(al==0x2b||al==0x2d){negative=al==0x2d;al=Load(m,inputSegment,ref si);}
  DecimalFast:
   old=al;al=unchecked((byte)(al-0x30));
   if(old<0x30)goto Terminal;
   if(al>=0x0a)goto StatusOne;
   ax=unchecked((ushort)(short)(sbyte)al);
   product=(uint)bx*cx;sum=(product&65535)+ax;bx=(ushort)sum;
   di=unchecked((ushort)(di+(product>>16)+(sum>>16)));
   al=Load(m,inputSegment,ref si);
   if(di==0)goto DecimalFast;
  Slow:
   ax=Classify(ref al,(byte)cx,out carry);if(carry)goto Terminal;
   product=(uint)bx*cx;sum=(product&65535)+ax;bx=(ushort)sum;
   carryWord=unchecked((ushort)((product>>16)+(sum>>16)));
   oldHigh=di;di=carryWord;product=(uint)oldHigh*cx;
   if((product>>16)!=0)goto StatusOne;
   addend=(ushort)product;prior=di;di=unchecked((ushort)(di+addend));
   if(((~(prior^addend)&(prior^di))&0x8000)!=0)goto StatusOne;
   al=Load(m,inputSegment,ref si);goto Slow;
  Terminal:
   if(al==0xf3)goto Radix;
   status=0;si=unchecked((ushort)(si-1));if(m.ReadByte(inputSegment,si)!=0)status=4;goto Finish;
  Radix:
   if(cx!=10){status=2;goto Finish;}
   if(bx==0)cx=16;
   else{cx=bx;if((byte)cx>0x24||(byte)cx<2){status=3;goto Finish;}}
   al=Load(m,inputSegment,ref si);di=0;bx=0;
  RadixFast:
   ax=Classify(ref al,(byte)cx,out carry);if(carry)goto Terminal;
   product=(uint)bx*cx;sum=(product&65535)+ax;bx=(ushort)sum;
   di=unchecked((ushort)(di+(product>>16)+(sum>>16)));
   al=Load(m,inputSegment,ref si);if(di==0)goto RadixFast;goto Slow;
  StatusOne:
   status=1;
  Finish:
   m.WriteWord(statusSegment,statusOffset,status);
   ax=bx;oldHigh=di;
   if(negative){ax=unchecked((ushort)-ax);oldHigh=unchecked((ushort)-(oldHigh+(bx!=0?1:0)));}
   return new NumericConverterResult(ax,oldHigh,status);
  }
 }
}
