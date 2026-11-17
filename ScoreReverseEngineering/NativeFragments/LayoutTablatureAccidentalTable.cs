using System;
namespace ScoreModern.RecoveredFragments {
 public static class LayoutTablatureAccidentalTable {
  //2C0C lookup DS:4062+2*delta; original finite signed remainder domain.
  //Original unpacked image with EXEPACK relocation, verified against live DS.
  public const int FirstDelta=-23;
  static readonly ushort[] raw={79,85,68,0,1,2,0,10,20,73,87,80,68,85,70,19787,20559,21330,22612,11610,20782,4,1,3,2,5,10512,0,12127,0,10512,70,12127,74,10512,122,12127,74,10512,176,12127,0,10512,224,12127,0,10512};
  public static short[] Recover(ushort loadSegment){var result=new short[raw.Length];for(int i=0;i<raw.Length;i++)result[i]=unchecked((short)raw[i]);result[28]=unchecked((short)(raw[28]+loadSegment));result[32]=unchecked((short)(raw[32]+loadSegment));result[36]=unchecked((short)(raw[36]+loadSegment));result[40]=unchecked((short)(raw[40]+loadSegment));result[44]=unchecked((short)(raw[44]+loadSegment));return result;}
 }
}
