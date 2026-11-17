using System;
namespace ScoreModern.RecoveredFragments {
 public static class BufferedCoordinateDecode {
  static void Divide(ref SignedDivide16State s,short divisor){
   int dividend=(int)(((uint)s.DX<<16)|s.AX);
   if(divisor==0)throw new DivideByZeroException("Original decoder divide fault requires external delivery.");
   long q=(long)dividend/divisor;if(q<short.MinValue||q>short.MaxValue)throw new OverflowException("Original decoder quotient fault requires external delivery.");
   s.AX=unchecked((ushort)q);s.DX=unchecked((ushort)(dividend%divisor));
  }
  // Relative 1BF3:004F..006E, normal coordinate path through the call site.
  // Full successful-exit flags are defined by the final ADD. Fault delivery
  // and the enclosing loop/driver call are outside this entry point.
  public static SignedDivide16State Execute(IPointBufferMemory m,SignedDivide16State s){
   if(m==null)throw new ArgumentNullException("m");if(s.IP!=0x4f)throw new ArgumentException("Decoder coordinate entry must be 004F.");
   s.DX=(short)s.AX<0?(ushort)65535:(ushort)0;Divide(ref s,(short)m.ReadWord(s.DS,0x4b7e));
   ushort add=m.ReadWord(s.DS,0x4b7a);s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,add);s.AX=unchecked((ushort)(s.AX+add));m.WriteWord(s.DS,0xac40,s.AX);
   s.EFlags=RasterStatusFlags.Dec16(s.EFlags,s.SI);s.SI=unchecked((ushort)(s.SI-1));s.EFlags=RasterStatusFlags.Dec16(s.EFlags,s.SI);s.SI=unchecked((ushort)(s.SI-1));
   s.BX=m.ReadWord(s.SS,unchecked((ushort)(s.BP+6)));s.ES=m.ReadWord(s.SS,unchecked((ushort)(s.BP+8)));
   s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX+s.SI)));s.DX=(short)s.AX<0?(ushort)65535:(ushort)0;Divide(ref s,(short)s.DI);
   s.EFlags=RasterStatusFlags.Sub16(s.EFlags,0,s.AX);s.AX=unchecked((ushort)(0-s.AX));add=m.ReadWord(s.DS,0x4b7c);
   s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,add);s.AX=unchecked((ushort)(s.AX+add));m.WriteWord(s.DS,0xac42,s.AX);s.IP=0x6f;return s;
  }
 }
}
