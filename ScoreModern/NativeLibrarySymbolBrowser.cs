using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeLibrarySymbolBrowser : ScoreDialog {
  sealed class Bank {
   internal string Path,Error;
   internal int First;
   internal Glyph[] Slots;
  }
  readonly List<Bank> banks=new List<Bank>();
  readonly DataGridView grid=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,
   AllowUserToResizeRows=false,RowHeadersVisible=false,MultiSelect=false,SelectionMode=DataGridViewSelectionMode.CellSelect,
   BackgroundColor=SystemColors.Window,ClipboardCopyMode=DataGridViewClipboardCopyMode.Disable};
  readonly ContextMenuStrip actions=new ContextMenuStrip();
  readonly string directory;
  readonly Label caption=new Label{Dock=DockStyle.Top,AutoSize=true,Padding=new Padding(5),AutoEllipsis=true};

  internal NativeLibrarySymbolBrowser(string root):base("library-symbol-browser"){
   directory=NativeLibraryLocation.DirectoryFor(root,true);
   Text="LIB Symbol Browser";StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(1150,650);MinimumSize=new Size(470,300);

   grid.AccessibleName="LIB drawing banks and symbols";
   grid.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Bank file",Width=145,Frozen=true,SortMode=DataGridViewColumnSortMode.NotSortable});
   for(int i=0;i<10;i++)grid.Columns.Add(new DataGridViewTextBoxColumn{
    HeaderText="Slot "+i.ToString(CultureInfo.InvariantCulture),Width=190,SortMode=DataGridViewColumnSortMode.NotSortable});
   grid.RowTemplate.Height=190;
   var files=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
   foreach(var folder in new[]{Path.Combine(root,"LIB"),directory}.Distinct(StringComparer.OrdinalIgnoreCase))
    if(Directory.Exists(folder))foreach(var path in Directory.EnumerateFiles(folder,"*.drw"))files[Path.GetFileName(path)]=path;
   foreach(var file in files.OrderBy(pair=>pair.Key,StringComparer.OrdinalIgnoreCase)){
    int first;DrawingLibrary.TryFirstSymbol(file.Value,out first);
    var bank=new Bank{Path=file.Value,First=first};Read(bank);
    if(IsEmpty(bank))continue;
    banks.Add(bank);
    int row=grid.Rows.Add();
    FillRow(row);
   }
   UpdateCaption();
   grid.CellPainting+=PaintTile;
   var copy=actions.Items.Add("Copy symbol number",null,(s,e)=>CopyNumber());
   var edit=actions.Items.Add("Edit in DRAW...",null,(s,e)=>EditSymbol());
   actions.Opening+=(s,e)=>{
    int row,slot;var bank=Selected(out row,out slot);
    e.Cancel=bank==null;
    edit.Enabled=bank!=null&&bank.Slots!=null&&bank.Slots[slot].Name.Length!=0;
    copy.Enabled=bank!=null&&bank.First>=0;
   };
   grid.ContextMenuStrip=actions;
   grid.CellMouseDown+=(s,e)=>{
    if(e.Button!=MouseButtons.Right)return;
    grid.CurrentCell=e.RowIndex>=0&&e.ColumnIndex>=0?grid[e.ColumnIndex,e.RowIndex]:null;
   };
   Controls.Add(grid);Controls.Add(caption);
  }

  static bool IsEmpty(Bank bank){
   return bank.Slots!=null&&bank.Slots.All(slot=>slot.Name.Length==0);
  }

  void UpdateCaption(){
   caption.Text=banks.Count==0?directory+" (no drawing symbols found)":directory;
  }

  static void Read(Bank bank){
   bank.Error=null;bank.Slots=null;
   try{bank.Slots=DrawingLibrary.Load(bank.Path);}
   catch(IOException ex){bank.Error=ex.Message;}
   catch(UnauthorizedAccessException ex){bank.Error=ex.Message;}
   catch(InvalidDataException ex){bank.Error=ex.Message;}
  }

  void FillRow(int row){
   var bank=banks[row];var cells=grid.Rows[row].Cells;
   cells[0].Value=Path.GetFileName(bank.Path);
   cells[0].ToolTipText=bank.Error==null?bank.Path:bank.Path+"\r\n"+bank.Error;
   for(int i=0;i<10;i++){
    string name=bank.Slots==null?"Unreadable":bank.Slots[i].Name.Length==0?"Empty":bank.Slots[i].Name;
    cells[i+1].Value=(bank.First<0?"Unnumbered":Number(bank,i))+"\r\n"+name;
    cells[i+1].ToolTipText=(string)cells[i+1].Value;
   }
  }

  static string Number(Bank bank,int slot){return (bank.First+slot).ToString(CultureInfo.InvariantCulture);}

  void PaintTile(object sender,DataGridViewCellPaintingEventArgs e){
   if(e.RowIndex<0||e.ColumnIndex<1)return;
   var bank=banks[e.RowIndex];
   var glyph=bank.Slots==null?null:bank.Slots[e.ColumnIndex-1];
   e.Paint(e.ClipBounds,DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
   var box=e.CellBounds;
   int line=TextRenderer.MeasureText("Ag",e.CellStyle.Font).Height;
   int pad=Math.Max(4,line/5);
   var art=new Rectangle(box.X+pad,box.Y+pad,box.Width-pad*2,box.Height-line*2-pad*2);
   if(glyph!=null)NativeSymbolPreview.Draw(e.Graphics,glyph,art);
   string[] label=((string)e.FormattedValue??"").Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries);
   var colour=(e.State&DataGridViewElementStates.Selected)!=0?e.CellStyle.SelectionForeColor:e.CellStyle.ForeColor;
   for(int i=0;i<label.Length;i++)TextRenderer.DrawText(e.Graphics,label[i],e.CellStyle.Font,
    new Rectangle(box.X+2,box.Bottom-(label.Length-i)*line-2,box.Width-4,line),colour,
    TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.NoPrefix|TextFormatFlags.EndEllipsis);
   e.Handled=true;
  }

  Bank Selected(out int row,out int slot){
   var cell=grid.CurrentCell;row=cell==null?-1:cell.RowIndex;slot=cell==null?-1:cell.ColumnIndex-1;
   return row>=0&&slot>=0?banks[row]:null;
  }

  void CopyNumber(){
   int row,slot;var bank=Selected(out row,out slot);
   if(bank==null||bank.First<0)return;
   try{Clipboard.SetText(Number(bank,slot));}
   catch(System.Runtime.InteropServices.ExternalException ex){MessageBox.Show(this,ex.Message,"Copy symbol number");}
  }

  void EditSymbol(){
   int row,slot;var bank=Selected(out row,out slot);
   if(bank==null||bank.Slots==null||bank.Slots[slot].Name.Length==0)return;
   using(var draw=new NativeDrawDialog(directory){Icon=Icon}){
    draw.OpenBankSlot(bank.Path,bank.Slots,slot);
    draw.ShowDialog(this);
   }
   Read(bank);
   if(IsEmpty(bank)){
    banks.RemoveAt(row);
    grid.Rows.RemoveAt(row);
    UpdateCaption();
   }else{
    FillRow(row);
    grid.InvalidateRow(row);
   }
  }

  protected override void Dispose(bool disposing){if(disposing)actions.Dispose();base.Dispose(disposing);}
 }

 public sealed partial class Editor {
  void ShowLibrarySymbolBrowser(){
   using(var browser=new NativeLibrarySymbolBrowser(source){Icon=Icon})browser.ShowDialog(this);
  }
 }
}
