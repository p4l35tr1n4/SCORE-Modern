using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeDrawBankBrowser : ScoreDialog {
  readonly ListBox files=new ListBox{Dock=DockStyle.Fill,IntegralHeight=false};
  readonly FlowLayoutPanel symbols=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false,AutoScroll=true,Padding=new Padding(8)};
  readonly Label location=new Label{AutoSize=true,AutoEllipsis=true,Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft};
  readonly Label status=new Label{AutoSize=true,Dock=DockStyle.Top,Padding=new Padding(8)};
  readonly string action;
  string folder;
  internal Glyph[] Entries{get;private set;}
  internal string SelectedPath{get;private set;}
  internal int SelectedSlot{get;private set;}

  internal NativeDrawBankBrowser(string directory,string current,bool append):base("DRAW-banks"){
   action=append?"Append":"Open";
   Text=action+" DRAW symbol";StartPosition=FormStartPosition.CenterParent;
   ClientSize=new Size(870,310);MinimumSize=new Size(490,235);
   var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(8),ColumnCount=2,RowCount=2};
   layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,195));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
   var header=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,AutoSize=true};
   header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
   var browse=new Button{Text="Browse folder...",AutoSize=true};
   browse.Click+=(s,e)=>{using(var picker=new FolderBrowserDialog{Description="Choose a folder containing SCORE drawing banks",SelectedPath=folder})
     if(picker.ShowDialog(this)==DialogResult.OK)LoadFolder(picker.SelectedPath,null);};
   header.Controls.Add(location,0,0);header.Controls.Add(browse,1,0);
   layout.Controls.Add(header,0,0);layout.SetColumnSpan(header,2);
   files.AccessibleName="Drawing bank filenames";symbols.AccessibleName="Symbols in the selected bank";
   layout.Controls.Add(files,0,1);
   var gallery=new Panel{Dock=DockStyle.Fill};
   gallery.Controls.Add(symbols);gallery.Controls.Add(status);layout.Controls.Add(gallery,1,1);
   Controls.Add(layout);
   files.SelectedIndexChanged+=(s,e)=>LoadBank();
   LoadFolder(Directory.Exists(directory)?directory:Directory.GetCurrentDirectory(),current);
  }

  void LoadFolder(string path,string current){
   string[] banks;
   try{banks=Directory.EnumerateFiles(path,"*.drw").Select(Path.GetFileName).OrderBy(n=>n,StringComparer.OrdinalIgnoreCase).ToArray();}
   catch(IOException ex){status.Text=ex.Message;return;}
   catch(UnauthorizedAccessException ex){status.Text=ex.Message;return;}
   folder=path;location.Text=path;location.AccessibleName=path;
   files.BeginUpdate();
   try{files.Items.Clear();files.Items.AddRange(banks);}
   finally{files.EndUpdate();}
   if(banks.Length==0){ClearBank("No DRW banks in this folder.");return;}
   files.SelectedIndex=Math.Max(0,Array.FindIndex(banks,n=>String.Equals(n,Path.GetFileName(current),StringComparison.OrdinalIgnoreCase)));
  }

  void ClearBank(string message){
   Entries=null;SelectedPath=null;status.Text=message;
   while(symbols.Controls.Count>0)symbols.Controls[0].Dispose();
   symbols.AutoScrollPosition=Point.Empty;
  }

  void LoadBank(){
   ClearBank("Select a drawing bank.");
   if(files.SelectedItem==null)return;
   string path=Path.Combine(folder,(string)files.SelectedItem);
   Glyph[] entries;
   try{entries=DrawingLibrary.Load(path);}
   catch(InvalidDataException ex){status.Text=ex.Message;return;}
   catch(IOException ex){status.Text=ex.Message;return;}
   catch(UnauthorizedAccessException ex){status.Text=ex.Message;return;}
   Entries=entries;SelectedPath=path;
   status.Text=entries.Any(g=>g.Name.Length!=0)?action+" a symbol by clicking its preview.":"This bank has no named symbols.";
   symbols.SuspendLayout();
   try{
    float dpi=IsHandleCreated?NativeDisplay.Scale(this):1;
    for(int i=0;i<entries.Length;i++){
     int slot=i;var glyph=entries[i];if(glyph.Name.Length==0)continue;
     var tile=new GlyphTile(glyph){Size=new Size((int)Math.Round(112*dpi),(int)Math.Round(132*dpi)),Margin=new Padding((int)Math.Round(4*dpi)),AccessibleName=glyph.Name};
     tile.Click+=(s,e)=>{SelectedSlot=slot;DialogResult=DialogResult.OK;};
     symbols.Controls.Add(tile);
    }
   }finally{symbols.ResumeLayout(true);}
  }

  sealed class GlyphTile:Button {
   readonly Glyph glyph;
   internal GlyphTile(Glyph glyph){this.glyph=glyph;Text=glyph.Name;DoubleBuffered=true;}
   protected override void OnPaint(PaintEventArgs e){
    var g=e.Graphics;
    ButtonRenderer.DrawButton(g,ClientRectangle,Focused?System.Windows.Forms.VisualStyles.PushButtonState.Hot:System.Windows.Forms.VisualStyles.PushButtonState.Normal);
    int inset=Math.Max(5,Font.Height/3),captionHeight=Font.Height+inset;
    var area=new RectangleF(inset,inset,ClientSize.Width-inset*2,ClientSize.Height-captionHeight-inset*2);
    NativeSymbolPreview.Draw(g,glyph,area);
    TextRenderer.DrawText(g,glyph.Name,Font,new Rectangle(0,Height-captionHeight,Width,captionHeight),SystemColors.ControlText,
     TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.NoPrefix);
    if(Focused&&ShowFocusCues)ControlPaint.DrawFocusRectangle(g,Rectangle.Inflate(ClientRectangle,-3,-3));
   }
  }
 }
}
