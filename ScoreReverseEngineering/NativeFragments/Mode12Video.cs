using System;
using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class RealModeIret16 {
  // Recorded DOSBox-X 386-class real-mode 16-bit IRET flag mask.
  public static SignedDivide16State Execute(IHorizontalRasterMemory m,SignedDivide16State s){ushort ip=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);ushort cs=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);ushort flags=m.ReadWord(s.SS,s.SP);s.SP=(ushort)(s.SP+2);s.IP=ip;s.CS=cs;s.EFlags=(s.EFlags&~0x7fd5u)|((uint)flags&0x7fd5u)|2u;return s;}
 }
 public interface IMode12Surface {void WriteGlyph(int row,int column,byte character,byte color);void Scroll(int top,int left,int bottom,int right,int lines,byte background);}
 public interface IMode12GlyphSource {byte Row(byte character,int line);}
 public sealed class ExternalVgaFont:IMode12GlyphSource {
  readonly byte[] bytes;public ExternalVgaFont(byte[] data){if(data==null||data.Length!=4096)throw new ArgumentException("An external 256-glyph 8x16 font (4096 bytes) is required.");bytes=(byte[])data.Clone();}
  public byte Row(byte character,int line){if(line<0||line>=16)throw new ArgumentOutOfRangeException("line");return bytes[character*16+line];}
 }
 public sealed class Mode12Plane2Surface:IMode12Surface {
  public readonly byte[] Bytes;readonly IMode12GlyphSource font;
  public Mode12Plane2Surface(byte[] initial,IMode12GlyphSource glyphs){if(initial==null||initial.Length!=65536||glyphs==null)throw new ArgumentException("Plane-2 reference bytes and external glyph source required.");Bytes=(byte[])initial.Clone();font=glyphs;}
  public void WriteGlyph(int row,int column,byte character,byte color){if(row<0||row>=30||column<0||column>=80)throw new NotSupportedException("Glyph outside verified mode-12 viewport.");for(int y=0;y<16;y++){int at=(row*16+y)*80+column;byte bits=(color&4)!=0?font.Row(character,y):(byte)0;Bytes[at]=(byte)((color&0x80)!=0?(Bytes[at]^bits):bits);}}
  public void Scroll(int top,int left,int bottom,int right,int lines,byte background){int height=bottom-top+1;int shift=lines==0?height:Math.Min(lines,height);byte fill=(background&4)!=0?(byte)255:(byte)0;for(int y=top*16;y<=(bottom+1)*16-1;y++)for(int x=left;x<=right;x++)Bytes[y*80+x]=y+shift*16<(bottom+1)*16?Bytes[(y+shift*16)*80+x]:fill;}
 }
 public struct Mode12Glyph {public byte Character,Color;public Mode12Glyph(byte c,byte a){Character=c;Color=a;}}
 public sealed class Mode12CellSurface:IMode12Surface {
  readonly List<Mode12Glyph>[] cells=new List<Mode12Glyph>[80*30];readonly byte[] backgrounds=new byte[80*30];public long Revision{get;private set;}
  public Mode12CellSurface(){for(int i=0;i<cells.Length;i++)cells[i]=new List<Mode12Glyph>();}
  public byte Background(int row,int column){return backgrounds[row*80+column];}
  public Mode12Glyph[] Cell(int row,int column){return cells[row*80+column].ToArray();}
  public void WriteGlyph(int row,int column,byte character,byte color){if(row<0||row>=30||column<0||column>=80)throw new NotSupportedException("Glyph outside verified viewport.");var cell=cells[row*80+column];if((color&0x80)==0){cell.Clear();backgrounds[row*80+column]=0;}cell.Add(new Mode12Glyph(character,color));Revision++;}
  public void Scroll(int top,int left,int bottom,int right,int lines,byte background){int shift=lines==0?bottom-top+1:Math.Min(lines,bottom-top+1);for(int r=top;r<=bottom;r++)for(int c=left;c<=right;c++){var cell=cells[r*80+c];cell.Clear();if(r+shift<=bottom){cell.AddRange(cells[(r+shift)*80+c]);backgrounds[r*80+c]=backgrounds[(r+shift)*80+c];}else backgrounds[r*80+c]=background;}Revision++;}
  public void Clear(){foreach(var c in cells)c.Clear();Array.Clear(backgrounds,0,backgrounds.Length);Revision++;}
 }
 public sealed class Mode12VideoServices {
  readonly IMode12Surface surface;public Mode12VideoServices(IMode12Surface output){if(output==null)throw new ArgumentNullException("output");surface=output;}
  void Cursor(IScreenHookMemory m,int row,int col){m.WriteWord(0x40,0x50,(ushort)((row<<8)|col));}
  public SignedDivide16State ExecuteService(IScreenHookMemory m,SignedDivide16State s){
   if(m.ReadByte(0x40,0x49)!=0x12||m.ReadWord(0x40,0x4a)!=80||m.ReadByte(0x40,0x84)!=29||m.ReadWord(0x40,0x85)!=16||m.ReadByte(0x40,0x62)!=0)throw new NotSupportedException("Only measured 640x480 mode 12h / page zero / 8x16 cells is implemented.");
   byte ah=(byte)(s.AX>>8),al=(byte)s.AX;ushort cursor=m.ReadWord(0x40,0x50);int row=cursor>>8,col=cursor&255;
   if(ah==3){if((s.BX>>8)!=0)throw new NotSupportedException("Cursor page outside verified scope.");s.DX=cursor;s.CX=m.ReadWord(0x40,0x60);return s;}
   if(ah==2){if((s.BX>>8)!=0||s.DX>>8>=30||(s.DX&255)>=80)throw new NotSupportedException("Cursor outside verified viewport.");Cursor(m,s.DX>>8,s.DX&255);return s;}
   if(ah==6){if(al>127)throw new NotSupportedException("Signed scroll count outside verified scope.");int top=s.CX>>8,left=s.CX&255,bottom=Math.Min(29,s.DX>>8),right=Math.Min(79,s.DX&255);if(top<=bottom&&left<=right)surface.Scroll(top,left,bottom,right,al,(byte)(s.BX>>8));return s;}
   if(row>=30||col>=80)throw new NotSupportedException("Current cursor outside verified viewport.");
   if(ah==9||ah==10){if((s.BX>>8)!=0)throw new NotSupportedException("Glyph page outside verified scope.");for(int n=0;n<s.CX;n++){surface.WriteGlyph(row,col,al,(byte)s.BX);if(++col==80){col=0;row++;}}return s;}
   if(ah==14){switch(al){case 8:if(col>0)col--;break;case 13:col=0;break;case 10:row++;break;default:if(al<32)throw new NotSupportedException("Control character not yet verified.");surface.WriteGlyph(row,col,al,(byte)s.BX);col++;break;}if(col==80){col=0;row++;}if(row==30){surface.Scroll(0,0,29,79,1,0);row--;}Cursor(m,row,col);return s;}
   throw new NotSupportedException("Video service AH="+ah.ToString("X2")+" not verified.");
  }
 }
 public sealed class Mode12BiosTransfer:IScreenBiosTransferEffects {
  readonly Mode12VideoServices video;readonly ushort callbackSegment,iretOffset;
  public Mode12BiosTransfer(Mode12VideoServices services,ushort measuredCallbackSegment,ushort measuredIretOffset){if(services==null)throw new ArgumentNullException("services");video=services;callbackSegment=measuredCallbackSegment;iretOffset=measuredIretOffset;}
  public SignedDivide16State TransferToBios(IScreenHookMemory m,SignedDivide16State state){state=video.ExecuteService(m,state);state.CS=callbackSegment;state.IP=iretOffset;return RealModeIret16.Execute(m,state);}
 }
}
