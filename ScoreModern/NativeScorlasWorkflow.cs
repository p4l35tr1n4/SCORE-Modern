using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal static class NativeScorlasWorkflow {
  internal static string Export(string root,IList<string> files,string destination,NativePrintJob settings){
   return ExportPrepared(root,files,destination,settings,true);
  }
  internal static string ExportPrepared(string root,IList<string> files,string destination,NativePrintJob settings,bool resolveOffsets){
   if(files==null||files.Count==0)throw new InvalidDataException("Add at least one SCORE file.");
   if(files.Any(file=>String.Equals(Path.GetFullPath(file),Path.GetFullPath(destination),StringComparison.OrdinalIgnoreCase)))throw new InvalidDataException("The output filename would overwrite an input score.");
   var pages=new List<Canvas>();
   try{
    foreach(string file in files)pages.Add(new Canvas{Score=Score.Load(file),Library=new DrawingLibrary(root),TextLayout=new ScoreText(root)});
    var job=(settings??NativePrintJob.Defaults(root)).CopyConfiguration();
    if(resolveOffsets)job.Resolve(pages[pages.Count-1]);
    NativeEpsExport.WriteScorlasNamed(pages,root,destination,job,files);
    var messages=new List<string>{"Tiled page saved: "+destination};
    if(job.OffsetWarning!=null)messages.Add(job.OffsetWarning);
    if(pages.Any(page=>page.Score.Items.Count==0))messages.Add("Original empty-file scan bounds were reproduced; the EPS bounding box can be reversed.");
    foreach(var page in pages)messages.AddRange(page.ApproximatedItems);
    return String.Join(Environment.NewLine,messages.ToArray());
   }finally{foreach(var page in pages)page.Dispose();}
  }
 }

 internal sealed class ScorlasFileList:ListBox {
  int dropSlot=-1;
  internal int DropSlot{get{return dropSlot;}set{if(dropSlot==value)return;dropSlot=value;Invalidate();}}
  internal ScorlasFileList(){DrawMode=DrawMode.OwnerDrawFixed;AllowDrop=true;RefreshMetrics();}
  protected override void OnFontChanged(EventArgs e){base.OnFontChanged(e);RefreshMetrics();}
  internal void RefreshMetrics(){ItemHeight=Math.Max(1,TextRenderer.MeasureText("Ag",Font).Height+4);HorizontalExtent=Items.Cast<object>().Select(item=>TextRenderer.MeasureText(Convert.ToString(item),Font).Width+8).DefaultIfEmpty(0).Max();Invalidate();}
  protected override void OnDrawItem(DrawItemEventArgs e){
   e.DrawBackground();
   if(e.Index>=0&&e.Index<Items.Count){TextRenderer.DrawText(e.Graphics,Convert.ToString(Items[e.Index]),e.Font,e.Bounds,e.ForeColor,TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.NoPrefix|TextFormatFlags.SingleLine);e.DrawFocusRectangle();
    if(e.Index==dropSlot||e.Index+1==dropSlot)using(var pen=new Pen(SystemColors.Highlight,2)){int y=e.Index==dropSlot?e.Bounds.Top:e.Bounds.Bottom-1;e.Graphics.DrawLine(pen,e.Bounds.Left,y,e.Bounds.Right,y);}
   }
  }
 }

 internal sealed class NativeScorlasDialog:ScoreDialog {
  readonly string root;
  NativePrintJob settings;
  readonly ScorlasFileList files=new ScorlasFileList{Dock=DockStyle.Fill,IntegralHeight=false,HorizontalScrollbar=true,SelectionMode=SelectionMode.MultiExtended};
  readonly ComboBox sortBy=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList},sortDirection=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};
  readonly Button sort=new Button{Text="Sort",AutoSize=true};
  Point dragStart;bool dragArmed;
  readonly ComboBox pageSelector=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=180};
  readonly List<List<string>> batchPages=new List<List<string>>{new List<string>()};
  int currentPage;bool switchingPages,explicitOffsets;
  void StorePage(){if(!switchingPages)batchPages[currentPage]=files.Items.Cast<string>().ToList();}
  void RefreshPageSelector(){switchingPages=true;try{pageSelector.Items.Clear();for(int n=0;n<batchPages.Count;n++)pageSelector.Items.Add("Page "+(n+1)+" ("+batchPages[n].Count+" files)");pageSelector.SelectedIndex=currentPage;}finally{switchingPages=false;}}
  void SelectPage(){if(switchingPages||pageSelector.SelectedIndex<0)return;StorePage();currentPage=pageSelector.SelectedIndex;switchingPages=true;try{ReplaceRows(batchPages[currentPage],new int[0]);}finally{switchingPages=false;}UpdateActions();}
  void QuickBatch(){StorePage();using(var dialog=new NativeScorlasQuickBatchDialog(files.Items.Count>0?(string)files.Items[0]:""))if(dialog.ShowDialog(this)==DialogResult.OK){batchPages.Clear();batchPages.AddRange(dialog.Pages.Select(page=>new List<string>(page)));currentPage=0;switchingPages=true;try{ReplaceRows(batchPages[0],new int[0]);}finally{switchingPages=false;}RefreshPageSelector();UpdateActions();result.Text="Batch defined. Select a page to edit its files, sorting or order before export.";}}

  sealed class FileDrag{internal NativeScorlasDialog Owner;internal int[] Indices;internal string[] Snapshot;}
  sealed class SortRow{internal string Path,Key;internal long Number;internal int Index;internal bool Selected;}
  readonly Label settingsLine=new Label{AutoSize=true};
  readonly TextBox result=new TextBox{Dock=DockStyle.Fill,Multiline=true,ReadOnly=true,ScrollBars=ScrollBars.Vertical,Height=62};
  readonly Button up=new Button{Text="Move up",AutoSize=true},down=new Button{Text="Move down",AutoSize=true},remove=new Button{Text="Remove",AutoSize=true},export=new Button{Text="Export tiled page...",AutoSize=true};
  internal NativeScorlasDialog(string source,NativePrintJob retained):base("scorlas"){
   root=source;settings=(retained??NativePrintJob.Defaults(root)).CopyConfiguration();
   Text="SCORLAS — tiled page";Font=SystemFonts.MessageBoxFont;StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(820,520);MinimumSize=new Size(560,430);
   var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=1,RowCount=7};
   layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   for(int n=0;n<7;n++)layout.RowStyles.Add(new RowStyle(n==3?SizeType.Percent:SizeType.AutoSize,n==3?100:0));
   var pageActions=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,WrapContents=true};
   pageActions.Controls.Add(new Label{Text="Page",AutoSize=true,Margin=new Padding(3,7,3,3)});
   pageSelector.AccessibleName="Page to tile";
   var actions=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,WrapContents=true};
   var add=new Button{Text="Add files...",AutoSize=true};add.Click+=(s,e)=>Try(AddFiles);
   var quick=new Button{Text="Quick batch...",AutoSize=true};quick.Click+=(s,e)=>Try(QuickBatch);pageSelector.SelectedIndexChanged+=(s,e)=>SelectPage();
   var readNames=new Button{Text="Read name list...",AutoSize=true};readNames.Click+=(s,e)=>Try(ReadNames);var saveNames=new Button{Text="Save name list...",AutoSize=true};saveNames.Click+=(s,e)=>Try(SaveNames);
   pageActions.Controls.AddRange(new Control[]{pageSelector,quick,readNames,saveNames});
   actions.Controls.AddRange(new Control[]{add,remove,up,down});
   RefreshPageSelector();
   sortBy.Items.AddRange(new object[]{"Filename (natural)","Full path (natural)","Date modified","File size","Reverse current order"});sortBy.SelectedIndex=0;
   sortDirection.Items.AddRange(new object[]{"Ascending","Descending"});sortDirection.SelectedIndex=0;
   sortBy.AccessibleName="SCORLAS sort by";sortDirection.AccessibleName="SCORLAS sort direction";
   var hints=new ToolTip();hints.SetToolTip(sortBy,"Sorting is applied only when you click Sort. Natural sorting compares numeric filename parts as numbers, so page2 precedes page10.");hints.SetToolTip(files,"Drag files to the insertion line to reorder them. The last listed file prints first. Move up/down are also available.");Disposed+=(s,e)=>hints.Dispose();
   sortBy.Width=sortBy.Items.Cast<object>().Max(item=>TextRenderer.MeasureText(item.ToString(),sortBy.Font).Width)+SystemInformation.VerticalScrollBarWidth+16;
   sortDirection.Width=sortDirection.Items.Cast<object>().Max(item=>TextRenderer.MeasureText(item.ToString(),sortDirection.Font).Width)+SystemInformation.VerticalScrollBarWidth+16;
   var sorting=new FlowLayoutPanel{AutoSize=true,WrapContents=false,Margin=new Padding(0)};
   sorting.Controls.AddRange(new Control[]{new Label{Text="Sort by",AutoSize=true,Margin=new Padding(8,7,3,3)},sortBy,sortDirection,sort});
   actions.Controls.Add(sorting);
   sort.Click+=(s,e)=>Try(SortFiles);sortBy.SelectedIndexChanged+=(s,e)=>UpdateActions();
   var explanation=new Label{AutoSize=true,Text="Files on this page, top to bottom. Drag to reorder. SCORE prints the last file first."};
   layout.Controls.Add(pageActions,0,0);
   layout.Controls.Add(explanation,0,1);
   layout.Controls.Add(actions,0,2);
   layout.Controls.Add(files,0,3);
   remove.Click+=(s,e)=>{foreach(int index in files.SelectedIndices.Cast<int>().OrderByDescending(index=>index))files.Items.RemoveAt(index);files.RefreshMetrics();UpdateActions();};
   up.Click+=(s,e)=>MoveEntry(-1);down.Click+=(s,e)=>MoveEntry(1);files.SelectedIndexChanged+=(s,e)=>UpdateActions();
   InstallDragReordering();
   var configuration=new TableLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,ColumnCount=2};configuration.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));configuration.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));configuration.Controls.Add(settingsLine,0,0);
   var read=new Button{Text="Read PRINT.SCR...",AutoSize=true};read.Click+=(s,e)=>Try(ReadSettings);configuration.Controls.Add(read,1,0);layout.Controls.Add(configuration,0,4);layout.Controls.Add(result,0,5);
   var buttons=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft};var close=new Button{Text="Close",AutoSize=true,DialogResult=DialogResult.Cancel};var stats=new Button{Text="Statistics...",AutoSize=true};stats.Click+=(s,e)=>Try(Statistics);buttons.Controls.Add(close);buttons.Controls.Add(export);var preview=new Button{Text="Print preview...",AutoSize=true};preview.Click+=(s,e)=>Try(()=>WindowsPrint(true));var print=new Button{Text="Print...",AutoSize=true};print.Click+=(s,e)=>Try(()=>WindowsPrint(false));buttons.Controls.Add(print);buttons.Controls.Add(preview);buttons.Controls.Add(stats);layout.Controls.Add(buttons,0,6);CancelButton=close;
   export.Click+=(s,e)=>Try(Export);Controls.Add(layout);Resize+=(s,e)=>{explanation.MaximumSize=new Size(Math.Max(100,layout.ClientSize.Width-layout.Padding.Horizontal),0);settingsLine.MaximumSize=new Size(Math.Max(100,configuration.ClientSize.Width-read.Width-12),0);};
   UpdateSettings();UpdateActions();
  }
  void Try(Action action){try{action();}catch(Exception error){if(!(error is IOException||error is UnauthorizedAccessException||error is ArgumentException||error is NotSupportedException||error is OverflowException||error is OperationCanceledException||error is TimeoutException||error is System.Drawing.Printing.InvalidPrinterException||error is InvalidOperationException))throw;result.Text=error.Message;}}
  void UpdateActions(){StorePage();if(!switchingPages&&pageSelector.Items.Count==batchPages.Count){switchingPages=true;try{pageSelector.Items[currentPage]="Page "+(currentPage+1)+" ("+files.Items.Count+" files)";}finally{switchingPages=false;}}export.Text=batchPages.Count>1?"Export batch...":"Export tiled page...";remove.Enabled=files.SelectedIndices.Count>0;up.Enabled=files.SelectedIndices.Count==1&&files.SelectedIndex>0;down.Enabled=files.SelectedIndices.Count==1&&files.SelectedIndex>=0&&files.SelectedIndex<files.Items.Count-1;export.Enabled=batchPages.All(page=>page.Count>0);sort.Enabled=sortBy.Enabled=files.Items.Count>1;sortDirection.Enabled=files.Items.Count>1&&sortBy.SelectedIndex!=4;}
  void AddFiles(){using(var dialog=new OpenFileDialog{Title="Add files in top-to-bottom order",Filter="SCORE music and pages (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",Multiselect=true,InitialDirectory=Directory.GetCurrentDirectory(),RestoreDirectory=true})if(dialog.ShowDialog(this)==DialogResult.OK){foreach(string file in dialog.FileNames)files.Items.Add(Path.GetFullPath(file));files.RefreshMetrics();UpdateActions();}}
  void MoveEntry(int delta){if(files.SelectedIndices.Count!=1)return;int index=files.SelectedIndex,next=index+delta;if(next<0||next>=files.Items.Count)return;MoveRows(new[]{index},delta<0?next:next+1);}
  internal static int NaturalCompare(string left,string right){
   int a=0,b=0;while(a<left.Length&&b<right.Length){
    if(left[a]>='0'&&left[a]<='9'&&right[b]>='0'&&right[b]<='9'){
     int ae=a,be=b;while(ae<left.Length&&left[ae]>='0'&&left[ae]<='9')ae++;while(be<right.Length&&right[be]>='0'&&right[be]<='9')be++;
     int az=a,bz=b;while(az<ae&&left[az]=='0')az++;while(bz<be&&right[bz]=='0')bz++;
     int c=(ae-az).CompareTo(be-bz);if(c!=0)return c;for(int n=0;n<ae-az;n++){c=left[az+n].CompareTo(right[bz+n]);if(c!=0)return c;}a=ae;b=be;
    }else{int c=Char.ToUpperInvariant(left[a]).CompareTo(Char.ToUpperInvariant(right[b]));if(c!=0)return c;a++;b++;}
   }return (left.Length-a).CompareTo(right.Length-b);
  }
  void ReplaceRows(IList<string> paths,IEnumerable<int> selected){int top=files.TopIndex;var keep=selected.ToArray();files.BeginUpdate();try{files.Items.Clear();foreach(string path in paths)files.Items.Add(path);files.ClearSelected();foreach(int index in keep)files.SetSelected(index,true);files.DropSlot=-1;files.RefreshMetrics();if(files.Items.Count>0){files.TopIndex=Math.Min(top,files.Items.Count-1);if(keep.Length>0){int first=keep.Min(),visible=Math.Max(1,files.ClientSize.Height/files.ItemHeight);if(first<files.TopIndex)files.TopIndex=first;else if(first>=files.TopIndex+visible)files.TopIndex=Math.Max(0,first-visible+1);}}}finally{files.EndUpdate();}UpdateActions();}
  void SortFiles(){
   int key=sortBy.SelectedIndex;var rows=files.Items.Cast<string>().Select((path,index)=>new SortRow{Path=path,Index=index,Selected=files.GetSelected(index),Key=key==0?Path.GetFileName(path):path}).ToList();
   if(key==4)rows.Reverse();else{
    if(key==2||key==3)foreach(var row in rows){var info=new FileInfo(row.Path);if(!info.Exists)throw new FileNotFoundException("Cannot sort a missing file.",row.Path);row.Number=key==2?info.LastWriteTimeUtc.Ticks:info.Length;}
    bool descending=sortDirection.SelectedIndex==1;rows.Sort((left,right)=>{int c=key<2?NaturalCompare(left.Key,right.Key):left.Number.CompareTo(right.Number);if(descending)c=-c;return c==0?left.Index.CompareTo(right.Index):c;});
   }
   ReplaceRows(rows.Select(row=>row.Path).ToArray(),rows.Select((row,index)=>new{row,index}).Where(value=>value.row.Selected).Select(value=>value.index));
  }
  void MoveRows(int[] indices,int slot){
   var before=files.Items.Cast<string>().ToArray();var selected=indices.Distinct().OrderBy(index=>index).ToArray();if(selected.Length==0)return;if(slot<0||slot>before.Length||selected.Any(index=>index<0||index>=before.Length))throw new ArgumentOutOfRangeException("indices");
   var moving=new HashSet<int>(selected);var result=before.Where((path,index)=>!moving.Contains(index)).ToList();int insertion=slot-selected.Count(index=>index<slot);result.InsertRange(insertion,selected.Select(index=>before[index]));ReplaceRows(result,Enumerable.Range(insertion,selected.Length));
  }
  FileDrag ReadDrag(IDataObject data){if(data==null||!data.GetDataPresent(typeof(FileDrag)))return null;var value=data.GetData(typeof(FileDrag)) as FileDrag;return value!=null&&value.Owner==this&&value.Snapshot.SequenceEqual(files.Items.Cast<string>())?value:null;}
  int DropSlot(Point point){int index=files.IndexFromPoint(point);if(index<0)return point.Y<0?files.TopIndex:files.Items.Count;var bounds=files.GetItemRectangle(index);return point.Y<bounds.Top+bounds.Height/2?index:index+1;}
  void InstallDragReordering(){
   files.MouseDown+=(s,e)=>{dragArmed=e.Button==MouseButtons.Left&&files.IndexFromPoint(e.Location)>=0;dragStart=e.Location;};files.MouseUp+=(s,e)=>dragArmed=false;
   files.MouseMove+=(s,e)=>{if(!dragArmed||(e.Button&MouseButtons.Left)==0)return;var size=SystemInformation.DragSize;if(new Rectangle(dragStart.X-size.Width/2,dragStart.Y-size.Height/2,size.Width,size.Height).Contains(e.Location))return;dragArmed=false;int[] selected=files.SelectedIndices.Cast<int>().ToArray();if(selected.Length==0)return;try{files.DoDragDrop(new FileDrag{Owner=this,Indices=selected,Snapshot=files.Items.Cast<string>().ToArray()},DragDropEffects.Move);}finally{files.DropSlot=-1;}};
   files.DragEnter+=(s,e)=>e.Effect=ReadDrag(e.Data)==null?DragDropEffects.None:DragDropEffects.Move;
   files.DragOver+=(s,e)=>{if(ReadDrag(e.Data)==null){e.Effect=DragDropEffects.None;return;}e.Effect=DragDropEffects.Move;Point p=files.PointToClient(new Point(e.X,e.Y));if(p.Y<files.ItemHeight&&files.TopIndex>0)files.TopIndex--;else if(p.Y>files.ClientSize.Height-files.ItemHeight&&files.TopIndex<files.Items.Count-1)files.TopIndex++;files.DropSlot=DropSlot(p);};
   files.DragLeave+=(s,e)=>files.DropSlot=-1;files.DragDrop+=(s,e)=>{var value=ReadDrag(e.Data);if(value!=null)MoveRows(value.Indices,DropSlot(files.PointToClient(new Point(e.X,e.Y))));files.DropSlot=-1;};
  }
  void UpdateSettings(){settingsLine.Text=String.Format(CultureInfo.InvariantCulture,"Print size {0}; paper {1} × {2} in; freeze {3}; rotation {4}; mirror {5}; captions {6}. Settings apply to every batch page.",settings.Size,settings.PaperWidth,settings.PaperHeight,settings.Freeze?"on":"off",settings.Rotate?"on":"off",settings.Mirror?"on":"off",settings.Header);}
  void ReadSettings(){using(var dialog=new OpenFileDialog{Title="Read SCORLAS print settings",Filter="SCORE print settings (*.scr)|*.scr|All files (*.*)|*.*",FileName="PRINT.SCR",InitialDirectory=NativeLibraryLocation.DirectoryFor(root,false),RestoreDirectory=true})if(dialog.ShowDialog(this)==DialogResult.OK){settings=NativePrintJob.Read(dialog.FileName,settings);explicitOffsets=true;UpdateSettings();}}
  void Export(){
   StorePage();
   if(batchPages.Count>1){ExportBatch();return;}
   using(var dialog=new SaveFileDialog{Title="Export SCORLAS tiled page",Filter="PostScript and EPS (*.ps;*.eps)|*.ps;*.eps",DefaultExt="ps",FileName="TILED.PS",InitialDirectory=NativeOutputLocation.EnsureFolder(root),RestoreDirectory=true,OverwritePrompt=true}){
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;string destination=NativeOutputLocation.RequireWritableDestination(root,dialog.FileName);bool previous=UseWaitCursor;UseWaitCursor=true;export.Enabled=false;result.Text="Preparing tiled page...";result.Update();
    try{result.Text=NativeScorlasWorkflow.ExportPrepared(root,files.Items.Cast<string>().ToArray(),destination,settings,!explicitOffsets);}finally{UseWaitCursor=previous;UpdateActions();}
   }
  }

  void WindowsPrint(bool preview){StorePage();NativeScorlasWindowsPrint.Show(this,root,batchPages,settings,!explicitOffsets,preview);}
  void ReadNames(){
   using(var open=new OpenFileDialog{Title="Read original SCORLAS name list",Filter="SCORLAS lists (*.txt;*.tmp)|*.txt;*.tmp|All files (*.*)|*.*",RestoreDirectory=true})if(open.ShowDialog(this)==DialogResult.OK)
   using(var folder=new FolderBrowserDialog{Description="Choose the folder containing the listed SCORE files. Filenames are resolved in this folder; saved DOS paths are not used.",SelectedPath=Directory.GetCurrentDirectory()})if(folder.ShowDialog(this)==DialogResult.OK){
    var pages=NativeScorlasNames.Read(open.FileName,name=>Path.Combine(folder.SelectedPath,Path.GetFileName(name.Replace('/',Path.DirectorySeparatorChar))));
    batchPages.Clear();batchPages.AddRange(pages);currentPage=0;switchingPages=true;try{ReplaceRows(batchPages[0],new int[0]);}finally{switchingPages=false;}RefreshPageSelector();UpdateActions();int missing=pages.SelectMany(page=>page).Count(file=>!File.Exists(file));result.Text="Read "+pages.Count+" pages; "+missing+" missing files. Check each page before output.";
   }
  }
  void SaveNames(){StorePage();using(var save=new SaveFileDialog{Title="Save original SCORLAS name list",Filter="SCORLAS names (*.txt)|*.txt|All files (*.*)|*.*",DefaultExt="txt",FileName="SCORLAS-LIST.TXT",RestoreDirectory=true})if(save.ShowDialog(this)==DialogResult.OK){NativeScorlasNames.Write(save.FileName,batchPages);result.Text="Saved name list: "+save.FileName;}}
  void Statistics(){
   StorePage();string content=NativeScorlasStatistics.Create(root,batchPages,settings);
   using(var dialog=new ScoreDialog("scorlas-statistics"){Text="SCORLAS statistics",ClientSize=new Size(800,500),Font=SystemFonts.MessageBoxFont,StartPosition=FormStartPosition.CenterParent})
   using(var font=new Font("Consolas",10)){
    var text=new TextBox{Dock=DockStyle.Fill,Multiline=true,ReadOnly=true,WordWrap=false,ScrollBars=ScrollBars.Both,Font=font,Text=content};var menu=new ContextMenuStrip();menu.Items.Add("Copy",null,(s,e)=>text.Copy());menu.Items.Add("Select all",null,(s,e)=>text.SelectAll());menu.Items.Add(new ToolStripSeparator());menu.Items.Add("Save as...",null,(s,e)=>{using(var save=new SaveFileDialog{Title="Save SCORLAS statistics",Filter="Text (*.txt)|*.txt|All files (*.*)|*.*",DefaultExt="txt",RestoreDirectory=true})if(save.ShowDialog(dialog)==DialogResult.OK)Try(()=>File.WriteAllText(save.FileName,content,System.Text.Encoding.GetEncoding(437,System.Text.EncoderFallback.ExceptionFallback,System.Text.DecoderFallback.ExceptionFallback)));});text.ContextMenuStrip=menu;dialog.Controls.Add(text);dialog.Disposed+=(s,e)=>menu.Dispose();dialog.AddTextOutputCloseButton();dialog.ShowDialog(this);
   }
  }
  void ExportBatch(){
   using(var dialog=new SaveFileDialog{Title="First SCORLAS batch output (numeric suffix increments)",Filter="PostScript and EPS (*.ps;*.eps)|*.ps;*.eps",DefaultExt="ps",FileName="TILED01.PS",InitialDirectory=NativeOutputLocation.EnsureFolder(root),RestoreDirectory=true,OverwritePrompt=false}){
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    string first=NativeOutputLocation.RequireWritableDestination(root,dialog.FileName);var outputs=NativeScorlasBatch.OutputNames(first,batchPages.Count,true);
    var missing=batchPages.SelectMany(page=>page).Where(file=>!File.Exists(file)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();if(missing.Length>0)throw new FileNotFoundException("Resolve missing batch inputs before exporting:"+Environment.NewLine+String.Join(Environment.NewLine,missing));
    string preview=String.Join(Environment.NewLine,outputs.Take(12).Select((file,n)=>"Page "+(n+1)+": "+file).ToArray());if(outputs.Length>12)preview+=Environment.NewLine+"... "+(outputs.Length-12)+" more pages";
    int existing=outputs.Count(File.Exists);if(MessageBox.Show(this,preview+Environment.NewLine+Environment.NewLine+existing+" existing outputs will be replaced. Export this batch?","SCORLAS batch outputs",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)!=DialogResult.OK)return;
    bool previous=UseWaitCursor;UseWaitCursor=true;export.Enabled=false;result.Text="Preparing batch...";result.Update();try{using(var progress=new NativeScorlasProgress()){Enabled=false;try{progress.Show(this);result.Text=NativeScorlasBatch.ExportControlled(root,batchPages,first,settings,true,!explicitOffsets,()=>progress.Cancelled,progress.Report);}finally{Enabled=true;progress.Close();}}}finally{UseWaitCursor=previous;UpdateActions();}
   }
  }

 }

 public sealed partial class Editor {
  void ShowScorlas(){using(var dialog=new NativeScorlasDialog(source,printJob))dialog.ShowDialog(this);}
 }
}
