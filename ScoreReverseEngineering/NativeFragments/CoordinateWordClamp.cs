namespace ScoreModern.RecoveredFragments {
 public static class CoordinateWordClamp {
  // Relative 0C25:0792..07EA. Registers are untouched; EFLAGS and the
  // literal working words are projected without assigning geometric roles.
  public static uint Execute(IHorizontalRasterMemory m,ushort ds,uint flags){
   ushort high=m.ReadWord(ds,0xc592);flags=RasterStatusFlags.Sub16(flags,high,0);
   if((short)high<0)goto FirstLower;
   if((short)high>0)goto FirstUpper;
   ushort low=m.ReadWord(ds,0xc590);flags=RasterStatusFlags.Sub16(flags,low,0x7ff8);
   if(low<0x7ff8)goto FirstLower;
  FirstUpper:
   m.WriteWord(ds,0xc590,0x7ff8);m.WriteWord(ds,0xc592,0);goto Second;
  FirstLower:
   high=m.ReadWord(ds,0xc592);flags=RasterStatusFlags.Sub16(flags,high,0xffff);
   if((short)high> -1)goto Second;
   if((short)high< -1)goto FirstLowerWrite;
   low=m.ReadWord(ds,0xc590);flags=RasterStatusFlags.Sub16(flags,low,0x80a8);
   if(low>=0x80a8)goto Second;
  FirstLowerWrite:
   m.WriteWord(ds,0xc590,0x80a8);m.WriteWord(ds,0xc592,0xffff);
  Second:
   high=m.ReadWord(ds,0xc596);flags=RasterStatusFlags.Sub16(flags,high,0);
   if((short)high<0)return flags;
   if((short)high>0)goto SecondUpper;
   low=m.ReadWord(ds,0xc594);flags=RasterStatusFlags.Sub16(flags,low,0x7fee);
   if(low<=0x7fee)return flags;
  SecondUpper:
   m.WriteWord(ds,0xc594,0x7fee);m.WriteWord(ds,0xc596,0);return flags;
  }
 }
}
