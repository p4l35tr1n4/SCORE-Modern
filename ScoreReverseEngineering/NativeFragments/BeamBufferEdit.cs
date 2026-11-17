using System;
namespace ScoreModern.RecoveredFragments {
 public static class BeamBufferEdit {
  // Original relative 00FD:0632..06C3. Includes both signed guards and
  // ordered segment-pointer reloads. Does not interpret the resulting shape.
  public static SignedDivide16State Execute(IPointBufferMemory m,SignedDivide16State s){
   if(m==null)throw new ArgumentNullException("m");
   if(s.IP!=0x632)throw new ArgumentException("Beam edit entry must be 0632.");
   s.ES=m.ReadWord(s.DS,0x6650);s.AX=m.ReadWord(s.ES,0);m.WriteWord(s.DS,0x8202,s.AX);
   s.SI=unchecked((ushort)(s.AX<<1));s.ES=m.ReadWord(s.DS,0x6652);s.BX=unchecked((ushort)(s.SI-2));
   s.DI=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));s.EFlags=RasterStatusFlags.Sub16(s.EFlags,s.DI,0x7f58);
   if((short)s.DI>0x7f58){s.IP=0x6c4;return s;}
   s.BX=unchecked((ushort)(s.SI-4));ushort other=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));
   s.EFlags=RasterStatusFlags.Sub16(s.EFlags,other,0x7f58);
   if((short)other>0x7f58){s.IP=0x6c4;return s;}
   s.ES=m.ReadWord(s.DS,0x6650);s.BX=s.SI;s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));
   s.BX=unchecked((ushort)(s.SI+2));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.ES=m.ReadWord(s.DS,0x6652);s.BX=s.SI;s.AX=m.ReadWord(s.DS,0xbf84);
   s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,s.DI);s.AX=unchecked((ushort)(s.AX+s.DI));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.ES=m.ReadWord(s.DS,0x6650);s.BX=unchecked((ushort)(s.SI-2));s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));
   s.BX=unchecked((ushort)(s.SI+4));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.SI=unchecked((ushort)(m.ReadWord(s.DS,0x8202)<<1));s.ES=m.ReadWord(s.DS,0x6652);s.BX=unchecked((ushort)(s.SI-4));
   s.AX=m.ReadWord(s.ES,unchecked((ushort)(s.BX-2)));ushort delta=m.ReadWord(s.DS,0xbf84);
   s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,delta);s.AX=unchecked((ushort)(s.AX+delta));
   s.BX=unchecked((ushort)(s.SI+2));m.WriteWord(s.ES,unchecked((ushort)(s.BX-2)),s.AX);
   s.ES=m.ReadWord(s.DS,0x6650);s.AX=m.ReadWord(s.DS,0x8202);s.EFlags=RasterStatusFlags.Add16(s.EFlags,s.AX,2);
   s.AX=unchecked((ushort)(s.AX+2));m.WriteWord(s.ES,0,s.AX);s.IP=0x6c4;return s;
  }
 }
}
