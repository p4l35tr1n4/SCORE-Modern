using System;using System.IO;using System.Linq;using System.Drawing;using System.Drawing.Drawing2D;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDisplayChecks {
  static void Assert(bool ok,string message){if(!ok)throw new Exception(message);}
  internal static void VerifyEmbeddedResources(){
   var expected = new[]{"ScoreModern.Document.ico","ScoreModern.Program.ico","ScoreModern.TitleLogo.png"};
   var resources = typeof(Editor).Assembly.GetManifestResourceNames().OrderBy(name=>name,StringComparer.Ordinal);
   Assert(resources.SequenceEqual(expected),"Unexpected app resource inventory.");
   Assert(typeof(Mode12VideoServices).Assembly.GetManifestResourceNames().Length==0,
    "Unexpected resource embedded in the recovered routines library.");
  }
  static void VerifyWideText(){using(var actual=new Bitmap(2200,80))using(var expected=new Bitmap(2200,80))using(var a=Graphics.FromImage(actual))using(var e=Graphics.FromImage(expected))using(var font=new Font("Consolas",12,GraphicsUnit.Pixel)){
   a.Clear(Color.White);e.Clear(Color.White);float cell=a.MeasureString("M",font,100,StringFormat.GenericTypographic).Width,line=font.GetHeight(a);
   NativeInterfaceText.Draw(a,new string(' ',100)+"X\r\n"+new string(' ',120)+"Y",font,Brushes.Black,new RectangleF(0,0,2200,80));
   NativeInterfaceText.Draw(e,"X",font,Brushes.Black,new RectangleF(100*cell,0,20,line));NativeInterfaceText.Draw(e,"Y",font,Brushes.Black,new RectangleF(120*cell,line,20,line));
   for(int row=0;row<2;row++){int dark=0;float left=(row==0?100:120)*cell;for(int y=(int)Math.Ceiling(row*line);y<(int)((row+1)*line);y++)for(int x=0;x<2200;x++)if(actual.GetPixel(x,y).R<128){Assert(x>=left-1&&x<=left+2*cell,"Windows text acquired an 80-column break at "+x+","+y);dark++;}Assert(dark>0,"Text beyond column 80 is missing from explicit line "+row);}
  }}
  public static int Verify(string fontPath){try{byte[] bytes=File.ReadAllBytes(fontPath);var glyphs=new ExternalVgaFont(bytes);var raster=new Mode12Plane2Surface(new byte[65536],glyphs);var cells=new Mode12CellSurface();Action<int> compare=n=>{using(var b=new Bitmap(640,480))using(var g=Graphics.FromImage(b))using(var font=new Font("Consolas",12,GraphicsUnit.Pixel)){g.Clear(Color.White);g.SmoothingMode=SmoothingMode.None;NativeInterfaceText.DrawSurface(g,cells,PointF.Empty,8,16,font,Brushes.Black,glyphs,30);for(int y=0;y<480;y++)for(int x=0;x<640;x++){bool expected=(raster.Bytes[y*80+x/8]&(128>>(x&7)))!=0;Assert((b.GetPixel(x,y).R<128)==expected,"Cell projection differs from verified plane-2 semantics: stage "+n+" at "+x+","+y);}}};
   raster.WriteGlyph(4,20,65,7);cells.WriteGlyph(4,20,65,7);compare(0);raster.WriteGlyph(4,20,66,0x87);cells.WriteGlyph(4,20,66,0x87);compare(1);raster.WriteGlyph(4,20,66,0x87);cells.WriteGlyph(4,20,66,0x87);compare(2);raster.WriteGlyph(5,1,67,7);cells.WriteGlyph(5,1,67,7);raster.Scroll(3,0,6,79,1,0);cells.Scroll(3,0,6,79,1,0);compare(3);raster.Scroll(4,2,5,6,0,0);cells.Scroll(4,2,5,6,0,0);compare(4);
   var text=new NativeTextSession();text.Write(System.Text.Encoding.ASCII.GetBytes("A\r\nB"));Assert(text.Surface.Cell(0,0)[0].Character==65&&text.Surface.Cell(1,0)[0].Character==66&&text.Memory.ReadWord(0x40,0x50)==0x101,"Native service-backed text layout differs.");text.Clear();Assert(text.Surface.Cell(0,0).Length==0&&text.Memory.ReadWord(0x40,0x50)==0,"Native clear/cursor reset differs.");NativeInterfaceText.SetVga(fontPath,false);Assert(NativeInterfaceText.UseVga,"External VGA option failed.");VerifyWideText();NativeInterfaceText.SetConsolas(false);VerifyWideText();Assert(!NativeInterfaceText.UseVga,"Consolas default failed.");VerifyEmbeddedResources();Console.WriteLine("PASS native Windows text projection: 1,536,000 pixel comparisons covering normal/XOR/cancellation/scroll/clear; native cursor layout; external VGA option; authorized title logo and program icon.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
  public static int RenderScore(string[] args){try{using(var editor=new Editor(args[1],args[2])){editor.Show();System.Windows.Forms.Application.DoEvents();editor.RenderScore(int.Parse(args[4]),args[3]);editor.Close();}Console.WriteLine("PASS native score render at width "+args[4]);return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 }
}
