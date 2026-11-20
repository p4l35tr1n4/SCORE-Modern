using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class NativeTextMemory:IScreenHookMemory {
  public readonly byte[] Bda=new byte[256];
  public NativeTextMemory(){Bda[0x49]=0x12;Bda[0x4a]=80;Bda[0x84]=29;Bda[0x85]=16;Bda[0x60]=7;Bda[0x61]=6;}
  public byte ReadByte(ushort s,ushort o){if(s!=0x40||o>=256)throw new InvalidOperationException("Display memory outside BIOS data area.");return Bda[o];}
  public ushort ReadWord(ushort s,ushort o){return (ushort)(ReadByte(s,o)|(ReadByte(s,(ushort)(o+1))<<8));}
  public void WriteByte(ushort s,ushort o,byte v){if(s!=0x40||o>=256)throw new InvalidOperationException("Display memory outside BIOS data area.");Bda[o]=v;}
  public void WriteWord(ushort s,ushort o,ushort v){WriteByte(s,o,(byte)v);WriteByte(s,(ushort)(o+1),(byte)(v>>8));}
  public void OrByte(ushort s,ushort o,byte v){throw new NotSupportedException("Direct video memory is not a text service.");}
 }
 public sealed class NativeTextSession {
  public readonly NativeTextMemory Memory=new NativeTextMemory();public readonly Mode12CellSurface Surface=new Mode12CellSurface();readonly Mode12VideoServices video;
  public NativeTextSession(){video=new Mode12VideoServices(Surface);}
  public void Clear(){video.ExecuteService(Memory,new SignedDivide16State{AX=0x600,DX=0x1d4f});Move(0,0);}
  public void Move(int row,int column){video.ExecuteService(Memory,new SignedDivide16State{AX=0x200,DX=(ushort)((row<<8)|column)});}
  public void Write(byte[] bytes){foreach(byte b in bytes)video.ExecuteService(Memory,new SignedDivide16State{AX=(ushort)(0xe00|b),BX=7});}
 }
 public static class NativeInterfaceText {
  static readonly Dictionary<string,Mode12CellSurface> cache=new Dictionary<string,Mode12CellSurface>();
  static readonly Encoding cp=Encoding.GetEncoding(437,EncoderFallback.ExceptionFallback,DecoderFallback.ExceptionFallback);
  static ExternalVgaFont vga;public static string VgaPath{get;private set;}public static bool UseVga{get;private set;}
  static string Settings {get{return "display";}}
  public static void Load(){if(!NativeSettings.Exists(Settings))return;try{string[] lines=NativeSettings.ReadAllLines(Settings);if(lines.Length>1&&lines[0]=="vga")SetVga(lines[1],false);}catch(Exception){UseVga=false;vga=null;}}
  public static void SetVga(string path,bool save){var font=new ExternalVgaFont(File.ReadAllBytes(path));vga=font;VgaPath=Path.GetFullPath(path);UseVga=true;if(save)NativeSettings.WriteAllLines(Settings,new[]{"vga",VgaPath});Invalidate();}
  public static void SetConsolas(bool save){UseVga=false;if(save)NativeSettings.WriteAllText(Settings,"consolas\r\n");Invalidate();}
  static void Invalidate(){foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void ChooseFont(IWin32Window owner){using(var d=new OpenFileDialog{Title="Choose an external 8x16 VGA font",Filter="Raw 8x16 VGA font (4096 bytes)|*.f16;*.bin|All files|*.*"})if(d.ShowDialog(owner)==DialogResult.OK){try{SetVga(d.FileName,true);}catch(Exception e){MessageBox.Show(owner,e.Message,"Font could not be loaded");}}}
  public static void Install(Form form){var menu=new PreferencesActionsMenu{Tag=form};var ui=new ToolStripMenuItem("Interface text: choose UI font...");ui.Click+=(s,e)=>NativeUiFont.Choose(form);menu.Items.Add(ui);menu.Opening+=(s,e)=>ui.ToolTipText=NativeUiFont.Family;menu.Items.Add("Interface text: reset to Consolas",null,(s,e)=>{NativeUiFont.Set("Consolas",true);SetConsolas(true);});menu.Items.Add("Use external VGA font...",null,(s,e)=>ChooseFont(form));NativeBeamDisplay.AddMenu(menu);NativeApproximation.AddMenu(menu);GhostscriptTextRenderer.AddMenu(menu);IncludedPostScriptDisplay.AddMenu(menu);AttachMenu(form,menu);}
  internal static void AttachMenu(Form form,ContextMenuStrip menu){form.ContextMenuStrip=menu;Attach(form,menu);}
  static void Attach(Control c,ContextMenuStrip menu){foreach(Control child in c.Controls){if(!(child is TextBoxBase))child.ContextMenuStrip=menu;Attach(child,menu);}}
  internal static StringFormat WindowsFormat(bool wrap=false){var format=(StringFormat)StringFormat.GenericTypographic.Clone();format.FormatFlags|=StringFormatFlags.MeasureTrailingSpaces;if(!wrap)format.FormatFlags|=StringFormatFlags.NoWrap;return format;}
  internal static float TextWidth(Graphics graphics,string text,Font font){if(String.IsNullOrEmpty(text))return 0;if(UseVga)return text.Length*graphics.MeasureString("M",font,100,StringFormat.GenericTypographic).Width;using(var format=WindowsFormat())return graphics.MeasureString(text,font,Int32.MaxValue,format).Width;}
  internal static RectangleF TextSpan(Graphics graphics,string text,Font font,int start,int length,RectangleF bounds){start=Math.Max(0,Math.Min(start,text.Length));length=Math.Max(0,Math.Min(length,text.Length-start));if(UseVga){float cell=graphics.MeasureString("M",font,100,StringFormat.GenericTypographic).Width;return new RectangleF(bounds.X+start*cell,bounds.Y,length*cell,bounds.Height);}if(length==0)return new RectangleF(bounds.X+TextWidth(graphics,text.Substring(0,start),font),bounds.Y,0,bounds.Height);using(var format=WindowsFormat()){format.SetMeasurableCharacterRanges(new[]{new CharacterRange(start,length)});var regions=graphics.MeasureCharacterRanges(text,font,bounds,format);try{var measured=regions[0].GetBounds(graphics);return new RectangleF(measured.X,bounds.Y,measured.Width,bounds.Height);}finally{foreach(var region in regions)region.Dispose();}}}
  public static void Draw(Graphics g,string text,Font font,Brush brush,float x,float y){Draw(g,text,font,brush,new PointF(x,y));}
  public static void Draw(Graphics g,string text,Font font,Brush brush,PointF point){Draw(g,text,font,brush,new RectangleF(point.X,point.Y,10000,font.GetHeight(g)*2));}
  public static void Draw(Graphics g,string text,Font font,Brush brush,RectangleF bounds,bool wrap=false){
   if(String.IsNullOrEmpty(text)||bounds.Width<=0||bounds.Height<=0)return;
   if(!UseVga){var clip=g.Save();try{g.SetClip(bounds,CombineMode.Intersect);using(var format=WindowsFormat(wrap))g.DrawString(text,font,brush,bounds,format);}finally{g.Restore(clip);}return;}byte[] bytes;
   try{bytes=cp.GetBytes(text.Replace("\t"," "));if(bytes.Any(b=>b<32&&b!=10&&b!=13))throw new EncoderFallbackException();}catch(EncoderFallbackException){g.DrawString(text,font,brush,bounds,StringFormat.GenericTypographic);return;}
   // The Windows text pane is not an 80-column BIOS surface. Keep its explicit
   // line breaks, and wrap only at the supplied control width when requested.
   float cell=g.MeasureString("M",font,100,StringFormat.GenericTypographic).Width;float line=font.GetHeight(g);
   int columns=Math.Max(1,(int)(bounds.Width/cell)),row=0,column=0;var saved=g.Save();try{g.SetClip(bounds,CombineMode.Intersect);
    for(int index=0;index<bytes.Length;index++){byte value=bytes[index];if(value==13||value==10){if(value==13&&index+1<bytes.Length&&bytes[index+1]==10)index++;row++;column=0;continue;}
     if(wrap&&column>=columns){column=0;row++;}
     float y=bounds.Y+row*line;if(y>=bounds.Bottom)break;
     DrawGlyph(g,new Mode12Glyph(value,7),bounds.X+column*cell,y,cell,line,font,brush,UseVga?vga:null);column++;
    }
   }finally{g.Restore(saved);}
  }
  public static int MeasureRows(string text){if(String.IsNullOrEmpty(text))return 1;try{var session=new NativeTextSession();session.Write(cp.GetBytes(text));for(int row=29;row>=0;row--)for(int col=0;col<80;col++)if(session.Surface.Cell(row,col).Any(v=>v.Character!=32))return row+1;return 1;}catch(EncoderFallbackException){return Math.Min(30,Math.Max(1,text.Split('\n').Sum(x=>Math.Max(1,(x.Length+79)/80))));}}
  public static void DrawSurface(Graphics g,Mode12CellSurface cells,PointF origin,float cellWidth,float cellHeight,Font font,Brush foreground,ExternalVgaFont external,int rows,bool singleLine=false){
   g.TextRenderingHint=TextRenderingHint.AntiAliasGridFit;
   for(int row=0;row<rows;row++)for(int col=0;col<80;col++){
    var layers=cells.Cell(row,col);if(layers.Length==0)continue;float x=origin.X+(singleLine?col+row*80:col)*cellWidth,y=origin.Y+(singleLine?0:row*cellHeight);
    if(layers.Length>1||layers.Any(a=>(a.Color&0x80)!=0)){DrawComposite(g,layers,x,y,cellWidth,cellHeight,font,foreground,external);continue;}
    DrawGlyph(g,layers[0],x,y,cellWidth,cellHeight,font,foreground,external);

   }
  }
  static void DrawGlyph(Graphics g,Mode12Glyph glyph,float x,float y,float width,float height,Font font,Brush brush,ExternalVgaFont external){if((glyph.Color&7)==0||glyph.Character==32)return;if(external!=null){for(int n=0;n<16;n++){byte bits=external.Row(glyph.Character,n);for(int b=0;b<8;b++)if((bits&(128>>b))!=0)g.FillRectangle(brush,x+b*width/8,y+n*height/16,width/8+.01f,height/16+.01f);}}else{string value=Encoding.GetEncoding(437).GetString(new[]{glyph.Character});if(glyph.Character==7)value="\u2022";g.DrawString(value,font,brush,x,y,StringFormat.GenericTypographic);}}
  static void DrawComposite(Graphics target,Mode12Glyph[] layers,float x,float y,float width,float height,Font font,Brush brush,ExternalVgaFont external){int w=Math.Max(1,(int)Math.Ceiling(width)),h=Math.Max(1,(int)Math.Ceiling(height));float[] coverage=new float[w*h];using(var mask=new Bitmap(w,h,System.Drawing.Imaging.PixelFormat.Format32bppArgb))using(var g=Graphics.FromImage(mask))using(var result=new Bitmap(w,h,System.Drawing.Imaging.PixelFormat.Format32bppArgb)){foreach(var layer in layers){g.Clear(Color.Transparent);g.TextRenderingHint=TextRenderingHint.AntiAliasGridFit;DrawGlyph(g,layer,0,0,width,height,font,Brushes.White,external);for(int py=0;py<h;py++)for(int px=0;px<w;px++){int at=py*w+px;float value=mask.GetPixel(px,py).A/255f;coverage[at]=(layer.Color&0x80)!=0?coverage[at]+value-2*coverage[at]*value:value;}}Color color=brush is SolidBrush?((SolidBrush)brush).Color:Color.Black;for(int py=0;py<h;py++)for(int px=0;px<w;px++)result.SetPixel(px,py,Color.FromArgb(Math.Max(0,Math.Min(255,(int)(coverage[py*w+px]*255))),color));target.DrawImageUnscaled(result,(int)x,(int)y);}}

 }
 public sealed class NativeTextLabel:Label {
  public bool Wrapped;internal int SelectedMessageRow=-1;
  internal int LinePitch;
  const TextFormatFlags MessageLineFlags=TextFormatFlags.NoPadding|TextFormatFlags.NoPrefix|TextFormatFlags.SingleLine|TextFormatFlags.ExpandTabs;

  System.Collections.Generic.List<string> MessageLines(int width){
   if(NativeInterfaceText.UseVga)return VgaLines(width);
   var lines=new System.Collections.Generic.List<string>();
   foreach(string paragraph in Text.Replace("\r\n","\n").Replace('\r','\n').Split('\n')){
    string remaining=paragraph;
    while(TextRenderer.MeasureText(remaining,Font,Size.Empty,MessageLineFlags).Width>width&&remaining.Length>1){
     int low=1,high=remaining.Length-1;
     while(low<high){
      int middle=(low+high+1)/2;
      if(TextRenderer.MeasureText(remaining.Substring(0,middle),Font,Size.Empty,MessageLineFlags).Width<=width)low=middle;
      else high=middle-1;
     }
     int space=remaining.LastIndexOf(' ',low,low+1);
     int cut=space>0?space:low;
     lines.Add(remaining.Substring(0,cut));
     remaining=remaining.Substring(cut).TrimStart(' ');
    }
    lines.Add(remaining);
   }
   return lines;
  }

  void PaintMessageRows(Graphics graphics){
   var lines=MessageLines(ClientSize.Width);
   using(var ink=new SolidBrush(ForeColor)){
    for(int index=0;index<lines.Count;index++){
     var row=new Rectangle(0,index*LinePitch,ClientSize.Width,LinePitch);
     if(!row.IntersectsWith(Rectangle.Ceiling(graphics.VisibleClipBounds)))continue;
     Color color=ForeColor;if(index==SelectedMessageRow){using(var fill=new SolidBrush(HighlightColor))graphics.FillRectangle(fill,row);color=HighlightColor.GetBrightness()>.5f?Color.Black:Color.White;}
     if(NativeInterfaceText.UseVga){using(var foreground=new SolidBrush(color))NativeInterfaceText.Draw(graphics,lines[index],Font,foreground,row);}
     else TextRenderer.DrawText(graphics,lines[index],Font,row,color,MessageLineFlags);
    }
   }
  }
  internal bool FitInputRow;
  internal int HighlightStart,HighlightLength;
  internal Color HighlightColor;
  internal Font FitInputFont(Graphics graphics,out float cell){
   cell=graphics.MeasureString("M",Font,100,StringFormat.GenericTypographic).Width;
   float width=NativeInterfaceText.TextWidth(graphics,Text,Font);if(width<=ClientSize.Width||ClientSize.Width<=0)return null;
   var fitted=new Font(Font.FontFamily,Font.Size*ClientSize.Width/width,Font.Style,Font.Unit);
   cell=graphics.MeasureString("M",fitted,100,StringFormat.GenericTypographic).Width;
   return fitted;
  }
  internal void PaintInputRow(Graphics graphics){
   float cell;
   using(var fitted=FitInputFont(graphics,out cell)){
    Font font=fitted??Font;
    using(var foreground=new SolidBrush(ForeColor))
     NativeInterfaceText.Draw(graphics,Text,font,foreground,ClientRectangle);
    if(HighlightLength==0)return;
    var field=NativeInterfaceText.TextSpan(graphics,Text,font,HighlightStart,HighlightLength,ClientRectangle);
    var state=graphics.Save();
    try{
     graphics.SetClip(field,CombineMode.Intersect);
     using(var background=new SolidBrush(HighlightColor))graphics.FillRectangle(background,field);
     Color ink=HighlightColor.GetBrightness()>.5f?Color.Black:Color.White;
     using(var foreground=new SolidBrush(ink))
      NativeInterfaceText.Draw(graphics,Text,font,foreground,ClientRectangle);
    }finally{graphics.Restore(state);}
   }
  }
  System.Collections.Generic.List<string> VgaLines(int width){float cell;using(var g=CreateGraphics())cell=g.MeasureString("M",Font,100,StringFormat.GenericTypographic).Width;int columns=Math.Max(1,(int)(width/cell));var result=new System.Collections.Generic.List<string>();foreach(string paragraph in (Text??"").Replace("\r","").Split('\n')){string rest=paragraph;while(rest.Length>columns){int cut=rest.LastIndexOf(' ',columns-1,columns);if(cut<=0)cut=columns;result.Add(rest.Substring(0,cut));rest=rest.Substring(cut).TrimStart(' ');}result.Add(rest);}return result;}
  public int ContentHeight(int width){if(!Wrapped)return Font.Height;if(LinePitch>0)return MessageLines(width).Count*LinePitch;if(NativeInterfaceText.UseVga)return (int)Math.Ceiling(VgaLines(width).Count*Font.GetHeight());return TextRenderer.MeasureText(Text??"",Font,new Size(Math.Max(1,width),Int32.MaxValue),TextFormatFlags.WordBreak|TextFormatFlags.NoPrefix|TextFormatFlags.TextBoxControl).Height;}
  public NativeTextLabel(){DoubleBuffered=true;}
  protected override void OnPaint(PaintEventArgs e){e.Graphics.Clear(BackColor);if(FitInputRow){PaintInputRow(e.Graphics);return;}if(Wrapped&&LinePitch>0){PaintMessageRows(e.Graphics);return;}if(Wrapped&&NativeInterfaceText.UseVga){float y=0;using(var brush=new SolidBrush(ForeColor))foreach(string line in VgaLines(ClientSize.Width)){NativeInterfaceText.Draw(e.Graphics,line,Font,brush,new RectangleF(0,y,ClientSize.Width,Font.GetHeight()));y+=Font.GetHeight();}return;}if(Wrapped&&!NativeInterfaceText.UseVga){TextRenderer.DrawText(e.Graphics,Text,Font,ClientRectangle,ForeColor,TextFormatFlags.WordBreak|TextFormatFlags.NoPrefix|TextFormatFlags.TextBoxControl);return;}using(var brush=new SolidBrush(ForeColor))NativeInterfaceText.Draw(e.Graphics,Text,Font,brush,new RectangleF(0,0,ClientSize.Width,ClientSize.Height),Wrapped);}
 }
 public sealed class NativeTextButton:Button {
  public bool ScoreFunction;protected override void WndProc(ref Message m){if(ScoreFunction&&m.Msg==0x7b&&m.LParam.ToInt64()!=-1)return;base.WndProc(ref m);}
  internal bool DrawMenuCell;
  protected override void OnPaint(PaintEventArgs e){
   e.Graphics.Clear(BackColor);
   Color ink=ForeColor;
   if(DrawMenuCell){
    var border=Color.FromArgb((ForeColor.R+BackColor.R)/2,(ForeColor.G+BackColor.G)/2,(ForeColor.B+BackColor.B)/2);
    ControlPaint.DrawBorder(e.Graphics,ClientRectangle,border,ButtonBorderStyle.Solid);
    if(!Enabled)ink=border;
   }
   float w=NativeInterfaceText.TextWidth(e.Graphics,Text,Font);
   using(var brush=new SolidBrush(ink))NativeInterfaceText.Draw(e.Graphics,Text,Font,brush,
    new RectangleF(Math.Max(0,(Width-w)/2),Math.Max(0,(Height-Font.GetHeight(e.Graphics))/2),Width,Height));
   if(Focused)ControlPaint.DrawFocusRectangle(e.Graphics,new Rectangle(2,2,Width-4,Height-4));
  }
 }
 public sealed class NativeCommandBox:TextBox {
  string priorPhysicalDisplay="";
  readonly System.Collections.Generic.List<bool> erasedInk=new System.Collections.Generic.List<bool>();
  internal string PhysicalText{get{char[] value=Text.ToCharArray();for(int n=0;n<value.Length&&n<erasedInk.Count;n++)if(erasedInk[n])value[n]=' ';return new String(value);}}
  internal void ErasePhysicalLine(){if(!ScoreEditing)return;while(erasedInk.Count<TextLength)erasedInk.Add(false);for(int n=0;n<TextLength;n++)erasedInk[n]=true;}
  protected override void OnTextChanged(EventArgs e){
   if(erasedInk!=null){string next=Text;int prefix=0;while(prefix<priorPhysicalDisplay.Length&&prefix<next.Length&&priorPhysicalDisplay[prefix]==next[prefix])prefix++;int suffix=0;while(suffix<priorPhysicalDisplay.Length-prefix&&suffix<next.Length-prefix&&priorPhysicalDisplay[priorPhysicalDisplay.Length-1-suffix]==next[next.Length-1-suffix])suffix++;while(erasedInk.Count<priorPhysicalDisplay.Length)erasedInk.Add(false);erasedInk.RemoveRange(prefix,priorPhysicalDisplay.Length-prefix-suffix);erasedInk.InsertRange(prefix,new bool[next.Length-prefix-suffix]);priorPhysicalDisplay=next;}
   base.OnTextChanged(e);
  }
  internal void ReplayCharacter(char value){var key=new KeyPressEventArgs(value);OnKeyPress(key);if(!key.Handled&&!Char.IsControl(value))SelectedText=value.ToString();}
  internal void ReplayEditingKey(Keys value){
   var key=new KeyEventArgs(value);OnKeyDown(key);if(key.Handled||key.SuppressKeyPress)return;
   int start=SelectionStart,length=SelectionLength;switch(value&Keys.KeyCode){
    case Keys.Back:if(length>0)SelectedText="";else if(start>0){Select(start-1,1);SelectedText="";}break;
    case Keys.Delete:if(length>0)SelectedText="";else if(start<TextLength){Select(start,1);SelectedText="";}break;
    case Keys.Left:Select(Math.Max(0,start-(length==0?1:0)),0);break;
    case Keys.Right:Select(Math.Min(TextLength,start+(length==0?1:length)),0);break;
    case Keys.Home:Select(0,0);break;case Keys.End:Select(TextLength,0);break;
   }
  }
  internal bool CaptureVerticalNavigation;
  internal bool ScoreEditing,ScoreInsert;
  internal Action ScoreLineComplete;
  protected override void OnKeyDown(KeyEventArgs e){
   base.OnKeyDown(e);
   if(!e.Handled&&!e.SuppressKeyPress&&ScoreEditing&&e.KeyCode==Keys.Insert&&e.Modifiers==Keys.None){
    ScoreInsert=!ScoreInsert;e.Handled=e.SuppressKeyPress=true;
   }
  }
  protected override void OnKeyPress(KeyPressEventArgs e){
   base.OnKeyPress(e);
   if(!e.Handled&&!Char.IsControl(e.KeyChar)){int length=SelectionLength==0?1:SelectionLength;for(int n=SelectionStart;n<SelectionStart+length&&n<erasedInk.Count;n++)erasedInk[n]=false;}
   // Original 0F4F:006F resets AC80 at each reader entry;
   // 1245..12A9 toggles it with Insert. Ordinary typing overwrites.
   if(!e.Handled&&ScoreEditing&&!ScoreInsert&&!Char.IsControl(e.KeyChar)&&SelectionLength==0&&SelectionStart<TextLength){
    SelectionLength=1;SelectedText=e.KeyChar.ToString();e.Handled=true;
   }
  }
  protected override bool IsInputKey(Keys key){
   Keys code=key&Keys.KeyCode;
   return CaptureVerticalNavigation&&(code==Keys.Up||code==Keys.Down)||base.IsInputKey(key);
  }
  protected override void WndProc(ref Message m){base.WndProc(ref m);if(m.Msg==0x0102&&m.WParam.ToInt64()>=32&&ScoreEditing&&TextLength==157&&SelectionStart==157&&ScoreLineComplete!=null)ScoreLineComplete();if((m.Msg==0x000f||m.Msg==0x0318)&&NativeInterfaceText.UseVga){using(var g=m.Msg==0x0318&&m.WParam!=IntPtr.Zero?Graphics.FromHdc(m.WParam):Graphics.FromHwnd(Handle)){g.Clear(BackColor);int first=Math.Max(0,GetCharIndexFromPosition(new Point(1,1)));using(var b=new SolidBrush(ForeColor))NativeInterfaceText.Draw(g,Text.Substring(Math.Min(first,Text.Length)),Font,b,new RectangleF(0,0,Width,Height));if(SelectionLength>0){Point start=GetPositionFromCharIndex(SelectionStart),end=GetPositionFromCharIndex(Math.Min(Text.Length,SelectionStart+SelectionLength));RectangleF selected=new RectangleF(start.X,0,Math.Max(0,end.X-start.X),Height);g.FillRectangle(SystemBrushes.Highlight,selected);NativeInterfaceText.Draw(g,SelectedText,Font,SystemBrushes.HighlightText,selected);}else if(Focused){Point caret=GetPositionFromCharIndex(SelectionStart);using(var pen=new Pen(ForeColor))g.DrawLine(pen,caret.X,1,caret.X,Height-2);}}}}
 }
}
