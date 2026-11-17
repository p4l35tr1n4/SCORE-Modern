using System;
namespace ScoreModern.RecoveredFragments {
 // Measured 0A33:0090..0096: LEA DI,[BX+SI]; MOV SI,0136; MOVSB.
 // DS:0136 is the measured ASCII space. This models that one byte write,
 // including 16-bit address addition, not the surrounding text parser.
 public static class InputTextTerminatorWrite {
  public const byte Space=0x20;
  public static ushort Offset(ushort destination,ushort length){return unchecked((ushort)(destination+length));}
  public static ushort Apply(byte[] segment,ushort destination,ushort length){
   if(segment==null||segment.Length!=65536)throw new ArgumentException("A complete measured 16-bit destination segment is required.");
   ushort offset=Offset(destination,length);segment[offset]=Space;return offset;
  }
 }
}
