namespace ScoreModern.RecoveredFragments {
 public struct LinePreparationWords {public ushort AX,BX,CX,DX,DI,ES;}
 public static class LinePreparation {
  // Relative 1807:074C..078B; entry follows the three stack pushes.
  public static LinePreparationWords Execute(IHorizontalRasterMemory m,ushort ds,ushort ax){
   ushort cx=m.ReadWord(ds,0xac40);m.WriteWord(ds,0x4b68,cx);m.WriteWord(ds,0x4b6a,ax);
   ushort dx=m.ReadWord(ds,0xac44);m.WriteWord(ds,0x4b6c,dx);
   ushort di=m.ReadWord(ds,0xac46);m.WriteWord(ds,0x4b6e,di);ushort bx=di;
   m.WriteWord(ds,0x4b78,2);di=m.ReadWord(ds,0x4a26);ushort es=di;
   ushort original=bx;bx=(ushort)(bx-ax);
   // JGE after SUB compares signed operands, including its overflow flag.
   if((short)original<(short)ax){
    bx=(ushort)(-bx);ushort old=m.ReadWord(ds,0x4b6e);m.WriteWord(ds,0x4b6e,ax);ax=old;
    m.WriteWord(ds,0x4b6a,ax);ushort x=dx;dx=cx;cx=x;
    m.WriteWord(ds,0x4b68,cx);m.WriteWord(ds,0x4b6c,dx);
   }
   return new LinePreparationWords{AX=ax,BX=bx,CX=cx,DX=dx,DI=di,ES=es};
  }
 }
}
