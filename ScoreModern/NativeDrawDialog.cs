using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeDrawDialog : ScoreDialog {
  readonly DataGridView points=new DataGridView{Dock=DockStyle.Fill,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,RowHeadersVisible=true};
  sealed class DrawCanvas:Panel { internal DrawCanvas(){SetStyle(ControlStyles.Selectable,true);TabStop=true;} }
  readonly Panel preview=new DrawCanvas{Dock=DockStyle.Fill,BackColor=Color.White};
  readonly TextBox itemName=new TextBox{Text="SHAPE",MaxLength=5,Width=68};
  readonly NumericUpDown operation=new NumericUpDown{Minimum=-3,Maximum=3,Value=1,Width=55};
  readonly NumericUpDown fillStep=new NumericUpDown{Minimum=1,Maximum=100,Value=3,Width=55};
  readonly NumericUpDown gridScale=new NumericUpDown{Minimum=.01m,Maximum=100,DecimalPlaces=2,Value=1,Width=59};
  readonly CheckBox showGrid=new CheckBox{Text="Staff grid",AutoSize=true};
  readonly CheckBox showOverlay=new CheckBox{Text="Show overlay",AutoSize=true,Enabled=false};
  readonly NumericUpDown zoomControl=new NumericUpDown{Minimum=.1m,Maximum=32,DecimalPlaces=2,Increment=.25m,Value=1,Width=61};
  readonly CheckBox plot=new CheckBox{Text="Plot points",AutoSize=true};
  readonly Label count=new Label{AutoSize=true};
  readonly NativeDrawCurve curve=new NativeDrawCurve();
  readonly string library;
  string bankPath;
  Glyph[] bank;
  Glyph overlay;
  Glyph curveAnchors;
  int fittedFirst,fittedLength;
  bool showAnchors;
  readonly Stack<Tuple<Glyph,bool>> undo=new Stack<Tuple<Glyph,bool>>();
  readonly Stack<Tuple<Glyph,bool>> redo=new Stack<Tuple<Glyph,bool>>();
  bool restoringHistory;
  Glyph savedDrawing;
  int dragPoint=-1;
  bool dragRecorded,keepFrame;
  Point dragStart;
  PointF dragOriginal;
  readonly CheckBox showPoints=new CheckBox{Text="Points",AutoSize=true,Checked=true};
  readonly CheckBox snapToGrid=new CheckBox{Text="Snap to grid",AutoSize=true};
  readonly CheckBox showSquareGrid=new CheckBox{Text="Square grid",AutoSize=true};
  readonly NumericUpDown gridSpacing=new NumericUpDown{Minimum=1,Maximum=32767,Value=10,Width=65,AccessibleName="Square grid spacing in DRAW units"};
  Point lastMove;
  int[] lastMoveRows,lastCopyRows;
  bool canRepeatMove;
  int slot=-1;
  bool dirty,loading,showFill;
  float zoom=1;
  PointF origin;
  RectangleF previewBounds;
  Point panStart;
  PointF pan;
  bool panning;

  internal NativeDrawDialog(string library){
   this.library=library;
   Text="DRAW";StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(900,590);MinimumSize=new Size(600,400);
   foreach(string field in new[]{"X","Y","Operation"}){
    int column=points.Columns.Add(field,field);
    points.Columns[column].SortMode=DataGridViewColumnSortMode.NotSortable;
   }
   var body=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=3,Padding=new Padding(10)};
   body.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   body.RowStyles.Add(new RowStyle(SizeType.Percent,100));
   body.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   var menu=new MenuStrip{Dock=DockStyle.Fill,AutoSize=true};
   var file=new ToolStripMenuItem("File");
   var edit=new ToolStripMenuItem("Edit");
   var construct=new ToolStripMenuItem("Construct");
   var view=new ToolStripMenuItem("View");
   menu.Items.AddRange(new ToolStripItem[]{file,edit,construct,view});
   AddAction(file,"New",NewDrawing);
   AddAction(file,"Open bank...",()=>Open(false));
   AddAction(file,"Append item...",()=>Open(true));
   AddAction(file,"Read vectors...",()=>WithFile(false,false,ReadVectors));
   AddAction(file,"Write vectors...",()=>WithFile(true,false,path=>{
    var glyph=Current();
    ScoreFileStorage.Write(path,NativeDrawDocument.WriteVectors(glyph));
    savedDrawing=glyph;dirty=false;
   }));
   AddAction(file,"Save to bank...",()=>WithFile(true,true,SaveBank));
   AddAction(file,"Rename bank item...",RenameBankItem);
   AddAction(file,"Delete bank item...",DeleteBankItem);
   AddAction(file,"Delete drawing file...",DeleteDrawingFile);
   AddAction(edit,"Move...",()=>TransformDrawing(false));
   AddAction(edit,"Add point...",()=>InsertPoint(false));
   AddAction(edit,"Insert after selected point...",()=>InsertPoint(true));
   AddAction(edit,"Close contour",CloseContour);
   AddAction(edit,"Repeat last move",()=>RepeatMove(false));
   AddAction(edit,"Reverse last move",()=>RepeatMove(true));
   AddAction(edit,"Move to origin",MoveToOrigin);
   AddAction(edit,"Resize...",()=>TransformDrawing(true));
   AddAction(edit,"Rotate...",RotateDrawing);
   AddAction(edit,"Move selected points...",()=>TransformDrawing(false,true));
   AddAction(edit,"Copy selected points...",()=>TransformDrawing(false,true,true));
   AddAction(edit,"Copy previous group...",()=>{if(lastCopyRows!=null)TransformDrawing(false,true,true,lastCopyRows);});
   AddAction(edit,"Resize selected points...",()=>TransformDrawing(true,true));
   AddAction(edit,"Rotate selected points...",()=>RotateDrawing(true));
   AddAction(edit,"Delete selected points",DeleteGroup);
   AddAction(edit,"Undo point edit",UndoPoint,Keys.Control|Keys.Z);
   AddAction(edit,"Redo point edit",RedoPoint,Keys.Control|Keys.Y);
   AddAction(edit,"Clean points",()=>{var glyph=Current();NativeDrawDocument.Clean(glyph);ApplyGlyph(glyph);});
   AddAction(edit,"Reorder contours",()=>{var glyph=Current();NativeDrawDocument.Reorder(glyph);ApplyGlyph(glyph);});
   AddAction(construct,"Fit curve...",FitCurve);
   AddAction(construct,"Restore construction points",RestoreConstructionPoints);
   AddAction(construct,"Thicken...",Thicken);
   AddAction(construct,"Circle or arc...",AddArc);
   var colours=new ToolStripMenuItem("Graphics colours");
   for(int i=0;i<colourNames.Length;i++){
    int role=i;
    var choice=new ToolStripMenuItem(colourNames[i]);
    choice.Click+=(sender,args)=>{using(var picker=new ColorDialog{Color=drawingColours[role],FullOpen=true})if(picker.ShowDialog(this)==DialogResult.OK)SetDrawingColour(role,picker.Color);};
    colours.DropDownItems.Add(choice);
   }
   colours.DropDownItems.Add(new ToolStripSeparator());
   AddAction(colours,"Reset all colours",()=>{for(int i=0;i<colourNames.Length;i++)SetDrawingColour(i,null);});
   view.DropDownItems.Add(colours);
   for(int i=0;i<colourNames.Length;i++){
    uint argb;
    if(UInt32.TryParse(NativeSettings.Read(DrawingColourKey(i)),System.Globalization.NumberStyles.HexNumber,null,out argb))drawingColours[i]=Color.FromArgb(unchecked((int)argb));
   }
   preview.BackColor=drawingColours[0];
   AddAction(view,"Store overlay",StoreOverlay);
   AddAction(view,"Toggle construction points",()=>{showAnchors=!showAnchors;preview.Invalidate();});
   AddAction(view,"Fill preview",()=>{Current();showFill=true;preview.Invalidate();});
   AddAction(view,"Redraw",ClearFill);
   AddAction(view,"Reset view",()=>{keepFrame=false;pan=PointF.Empty;zoomControl.Value=1;ClearFill();});
   var split=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=1};
   split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,63));
   split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,37));
   split.Controls.Add(preview,0,0);split.Controls.Add(points,1,0);
   var controls=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill};
   controls.Controls.AddRange(new Control[]{BottomField("Item",itemName),plot,BottomField("Operation",operation),count,
    BottomField("Fill step (px)",fillStep),BottomField("Staff scale",gridScale,showGrid),
    BottomField("Spacing",gridSpacing,showSquareGrid),snapToGrid,showPoints,showOverlay,BottomField("Zoom",zoomControl)});
   body.Controls.Add(menu,0,0);body.Controls.Add(split,0,1);body.Controls.Add(controls,0,2);
   Controls.Add(body);
   MainMenuStrip=menu;
   preview.Paint+=(sender,e)=>PaintDrawing(e.Graphics);
   preview.MouseDown+=(sender,e)=>{
    if(e.Button==MouseButtons.Right){panStart=e.Location;panning=true;preview.Capture=true;}
    else if(e.Button==MouseButtons.Middle)Attempt(()=>{if(plot.Checked)DeleteLastPoint();else SelectPointAt(e.Location);});
    else if(e.Button==MouseButtons.Left)Attempt(()=>BeginPointDrag(e));
   };
   preview.MouseMove+=(sender,e)=>{
    if(dragPoint>=0){MovePointDrag(e.Location);return;}
    if(!panning)return;
    pan=new PointF(pan.X+e.X-panStart.X,pan.Y+e.Y-panStart.Y);
    panStart=e.Location;
    ClearFill();
   };
   preview.MouseUp+=(sender,e)=>{
    if(e.Button==MouseButtons.Left&&dragPoint>=0){MovePointDrag(e.Location);dragPoint=-1;preview.Capture=false;}
    if(e.Button==MouseButtons.Right){panning=false;preview.Capture=false;}
   };
   preview.MouseCaptureChanged+=(sender,e)=>{if(!preview.Capture){dragPoint=-1;panning=false;}};
   showPoints.CheckedChanged+=(sender,e)=>preview.Invalidate();
   showSquareGrid.CheckedChanged+=(sender,e)=>{UpdateGridControls();preview.Invalidate();};
   snapToGrid.CheckedChanged+=(sender,e)=>UpdateGridControls();
   gridSpacing.Enabled=false;
   gridSpacing.ValueChanged+=(sender,e)=>preview.Invalidate();
   points.SelectionChanged+=(sender,e)=>preview.Invalidate();
   preview.Resize+=(sender,e)=>ClearFill();
   points.CellValueChanged+=(sender,e)=>Changed();
   points.RowsRemoved+=(sender,e)=>Changed();
   points.UserAddedRow+=(sender,e)=>Changed();
   points.CellBeginEdit+=(sender,e)=>RememberPoint(e.RowIndex==points.Rows.Count-2?e.RowIndex:-1);
   points.UserDeletingRow+=(sender,e)=>RememberPoint();
   itemName.TextChanged+=(sender,e)=>Changed();
   plot.CheckedChanged+=(sender,e)=>preview.Invalidate();
   fillStep.ValueChanged+=(sender,e)=>ClearFill();
   showGrid.CheckedChanged+=(sender,e)=>ClearFill();
   gridScale.ValueChanged+=(sender,e)=>ClearFill();
   showOverlay.CheckedChanged+=(sender,e)=>preview.Invalidate();
   zoomControl.ValueChanged+=(sender,e)=>ClearFill();
   count.Text="0 / 4000";
  }

  void UpdateGridControls(){gridSpacing.Enabled=showSquareGrid.Checked||snapToGrid.Checked;}

  static Control BottomField(string caption,Control field,CheckBox toggle=null){
   var pair=new TableLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,ColumnCount=2,RowCount=1,Margin=Padding.Empty};
   Control label=toggle??(Control)new Label{Text=caption,AutoSize=true};
   label.Anchor=AnchorStyles.Left;field.Anchor=AnchorStyles.Left;
   pair.Controls.Add(label,0,0);pair.Controls.Add(field,1,0);return pair;
  }

  void AddAction(ToolStripMenuItem menu,string label,Action action,Keys shortcut=Keys.None){
   var item=new ToolStripMenuItem(label){ShortcutKeys=shortcut};
   item.Click+=(s,e)=>Attempt(action);
   menu.DropDownItems.Add(item);
  }

  void Attempt(Action action){
   try{action();}
   catch(InvalidDataException error){MessageBox.Show(this,error.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);}
   catch(IOException error){MessageBox.Show(this,error.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);}
   catch(UnauthorizedAccessException error){MessageBox.Show(this,error.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);}
  }

  void Changed(){
   if(loading)return;
   if(!restoringHistory)redo.Clear();
   dirty=true;
   canRepeatMove=false;
   lastCopyRows=null;
   curveAnchors=null;
   showAnchors=false;
   count.Text=points.Rows.Cast<DataGridViewRow>().Count(row=>!row.IsNewRow)+" / 4000";
   ClearFill();
  }

  void ClearFill(){
   showFill=false;
   preview.Invalidate();
  }

  bool ConfirmDiscard(){
   return !dirty||MessageBox.Show(this,"Discard the drawing changes?",Text,MessageBoxButtons.YesNo,
    MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.Yes;
  }

  Glyph Current(){
   points.EndEdit();
   return ReadGrid();
  }

  Glyph ReadGrid(){return ReadGridSnapshot(-1);}
  Glyph ReadGridSnapshot(int pendingRow){
   var xy=new List<PointF>();
   var op=new List<short>();
   foreach(DataGridViewRow row in points.Rows){
    if(row.IsNewRow)continue;
    short x,y,o;
    if(!CellWord(row,0,out x)||!CellWord(row,1,out y)||!CellWord(row,2,out o)||o< -3||o>3)
     throw new InvalidDataException("Enter integer X, Y and an operation from Ã¢Ë†â€™3 to 3 on row "+(row.Index+1)+".");
    xy.Add(new PointF(x,y));op.Add(o);
   }
   if(xy.Count>NativeDrawDocument.Capacity)throw new InvalidDataException("DRAW supports up to 4,000 vectors.");
   return new Glyph{Name=itemName.Text,Source=bankPath,Points=xy.ToArray(),Operations=op.ToArray()};
  }

  static bool CellWord(DataGridViewRow row,int column,out short word){
   return Int16.TryParse(Convert.ToString(row.Cells[column].Value,CultureInfo.InvariantCulture),
    NumberStyles.Integer,CultureInfo.InvariantCulture,out word);
  }

  void ShowGlyph(Glyph glyph){
   keepFrame=false;
   canRepeatMove=false;
   lastCopyRows=null;
   loading=true;
   try{
    points.Rows.Clear();
    for(int i=0;i<glyph.Points.Length;i++)points.Rows.Add(glyph.Points[i].X,glyph.Points[i].Y,glyph.Operations[i]);
    itemName.Text=glyph.Name;
   }finally{loading=false;}
   Changed();
  }

  void NewDrawing(){
   if(!ConfirmDiscard())return;
   bankPath=null;bank=null;slot=-1;
   undo.Clear();savedDrawing=null;ShowGlyph(new Glyph{Name="SHAPE",Points=new PointF[0],Operations=new short[0]});
   dirty=false;
  }

  void WithFile(bool save,bool isBank,Action<string> action){
   using(FileDialog dialog=save?(FileDialog)new SaveFileDialog{OverwritePrompt=!isBank}:new OpenFileDialog()){
    dialog.InitialDirectory=Directory.Exists(library)?library:null;
    dialog.RestoreDirectory=true;
    dialog.Filter=isBank?"SCORE drawing bank (*.drw)|*.drw":"ASCII vector list (*.txt)|*.txt|All files (*.*)|*.*";
    dialog.DefaultExt=isBank?"drw":"txt";
    if(save&&isBank&&bankPath!=null)dialog.FileName=bankPath;
    if(dialog.ShowDialog(this)==DialogResult.OK)action(dialog.FileName);
   }
  }

  void Open(bool append){
   using(var browser=new NativeDrawBankBrowser(bankPath==null?library:Path.GetDirectoryName(bankPath),bankPath,append)){
    if(browser.ShowDialog(this)!=DialogResult.OK)return;
    var entries=browser.Entries;
    var next=entries[browser.SelectedSlot];
    if(append){
     var current=Current();
     if(current.Points.Length+next.Points.Length>NativeDrawDocument.Capacity)
      throw new InvalidDataException("The appended drawing exceeds 4,000 vectors.");
     current.Points=current.Points.Concat(next.Points).ToArray();
     current.Operations=current.Operations.Concat(next.Operations).ToArray();
     ApplyGlyph(current);
    }else if(ConfirmDiscard()){
     OpenBankSlot(browser.SelectedPath,entries,browser.SelectedSlot);
    }
   }
  }

  internal void OpenBankSlot(string path,Glyph[] entries,int index){
   if(index<0||index>=entries.Length||String.IsNullOrEmpty(entries[index].Name))
    throw new InvalidDataException("The selected drawing slot is empty.");
   bankPath=path;bank=entries;slot=index;
   undo.Clear();savedDrawing=null;ShowGlyph(entries[index]);dirty=false;
  }

  void ReadVectors(string path){
   if(!ConfirmDiscard())return;
   var next=NativeDrawDocument.ReadVectors(path);
   bankPath=null;bank=null;slot=-1;
   undo.Clear();savedDrawing=null;ShowGlyph(next);
  }

  void SaveBank(string path){
   var glyph=Current();
   glyph.Name=NativeDrawDocument.ValidateName(glyph.Name);
   var entries=File.Exists(path)?DrawingLibrary.Load(path):new Glyph[10];
   int target=Array.FindIndex(entries,entry=>entry!=null&&String.Equals(entry.Name,glyph.Name,StringComparison.OrdinalIgnoreCase));
   if(target<0)target=Array.FindIndex(entries,entry=>entry==null||entry.Name.Length==0);
   if(target<0)throw new InvalidDataException("The drawing bank has no free slots.");
   if(entries[target]!=null&&entries[target].Name.Length!=0&&
      MessageBox.Show(this,"Replace item "+entries[target].Name+" in this bank?",Text,MessageBoxButtons.YesNo,
       MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)!=DialogResult.Yes)return;
   entries[target]=glyph;
   ScoreFileStorage.Write(path,NativeDrawDocument.WriteBank(entries));
   bankPath=path;bank=entries;slot=target;
   itemName.Text=glyph.Name;savedDrawing=glyph;dirty=false;
  }

  bool HasBankItem(){
   if(bankPath!=null&&slot>=0)return true;
   MessageBox.Show(this,"Open the bank item first.",Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
   return false;
  }

  void RenameBankItem(){
   if(!HasBankItem())return;
   using(var dialog=new ScoreDialog("DRAW-rename"){Text="Rename bank item",ClientSize=new Size(280,75),
    StartPosition=FormStartPosition.CenterParent}){
    var fields=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
    var name=new TextBox{MaxLength=5,Width=85,Text=bank[slot].Name};
    var rename=new Button{Text="Rename",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.AddRange(new Control[]{name,rename});
    dialog.Controls.Add(fields);dialog.AcceptButton=rename;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    bool wasDirty=dirty;
    ScoreFileStorage.Write(bankPath,NativeDrawDocument.RenameItem(bankPath,slot,name.Text));
    bank=DrawingLibrary.Load(bankPath);
    itemName.Text=bank[slot].Name;
    dirty=wasDirty;
   }
  }

  void DeleteBankItem(){
   if(!HasBankItem()||!ConfirmDiscard())return;
   if(MessageBox.Show(this,"Delete "+bank[slot].Name+" from this bank?",Text,
    MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)!=DialogResult.Yes)return;
   ScoreFileStorage.Write(bankPath,NativeDrawDocument.DeleteItem(bankPath,slot));
   bank=DrawingLibrary.Load(bankPath);
   slot=-1;
   undo.Clear();savedDrawing=null;ShowGlyph(new Glyph{Name="SHAPE",Points=new PointF[0],Operations=new short[0]});
   dirty=false;
  }

  void DeleteDrawingFile(){
   using(var file=new OpenFileDialog{Title="Choose a drawing file to delete",
    Filter="DRAW files (*.drw;*.txt)|*.drw;*.txt",InitialDirectory=Directory.Exists(library)?library:null,
    RestoreDirectory=true}){
    if(file.ShowDialog(this)!=DialogResult.OK)return;
    string extension=Path.GetExtension(file.FileName);
    if(!String.Equals(extension,".drw",StringComparison.OrdinalIgnoreCase)&&
     !String.Equals(extension,".txt",StringComparison.OrdinalIgnoreCase))
     throw new InvalidDataException("Choose a DRAW bank or ASCII vector file.");
    if(MessageBox.Show(this,"Permanently delete "+file.FileName+"?",Text,
     MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2)!=DialogResult.Yes)return;
    File.Delete(file.FileName);
    if(String.Equals(file.FileName,bankPath,StringComparison.OrdinalIgnoreCase)){
     bankPath=null;bank=null;slot=-1;
     dirty=points.Rows.Count>1;
    }
   }
  }

  static readonly string[] colourNames={"Background","Border","Origin axes","Staff grid","Outline","Overlay","Construction points","Fill","Square grid","Editable points","Selected point"};
  static readonly Color[] defaultColours={Color.White,SystemColors.WindowFrame,Color.Gainsboro,Color.Gray,Color.FromArgb(30,32,37),Color.FromArgb(30,32,37),Color.DimGray,Color.Black,Color.FromArgb(220,230,240),Color.FromArgb(52,104,165),Color.FromArgb(210,85,35)};
  readonly Color[] drawingColours=(Color[])defaultColours.Clone();
  static string DrawingColourKey(int role){return "draw-colour-"+colourNames[role].ToLowerInvariant().Replace(' ','-');}
  void SetDrawingColour(int role,Color? value){
   NativeSettings.WriteAllText(DrawingColourKey(role),value.HasValue?value.Value.ToArgb().ToString("X8"):null);
   drawingColours[role]=value??defaultColours[role];
   preview.BackColor=drawingColours[0];
   preview.Invalidate();
  }
  void PaintDrawing(Graphics graphics){
   using(var border=new Pen(drawingColours[1]))graphics.DrawRectangle(border,0,0,preview.Width-1,preview.Height-1);
   Glyph glyph;
   try{glyph=ReadGrid();}catch(InvalidDataException){return;}
   var bounds=glyph.Bounds;
   if(!plot.Checked&&!keepFrame&&dragPoint<0)previewBounds=bounds;
   zoom=(float)zoomControl.Value*Math.Min((preview.Width-32)/Math.Max(200,previewBounds.Width),
    (preview.Height-32)/Math.Max(200,previewBounds.Height));
   if(zoom<=0)return;
   origin=new PointF(pan.X+preview.Width/2f-(previewBounds.Left+previewBounds.Width/2)*zoom,
    pan.Y+preview.Height/2f+(previewBounds.Top+previewBounds.Height/2)*zoom);
   using(var axes=new Pen(drawingColours[2])){graphics.DrawLine(axes,origin.X,0,origin.X,preview.Height);
   graphics.DrawLine(axes,0,origin.Y,preview.Width,origin.Y);}
   if(showSquareGrid.Checked)PaintSquareGrid(graphics);
   if(showGrid.Checked)PaintGrid(graphics);
   if(showOverlay.Checked&&overlay!=null)overlay.Draw(graphics,origin.X,origin.Y,zoom,zoom,0,true,drawingColours[5]);
   glyph.Draw(graphics,origin.X,origin.Y,zoom,zoom,0,true,drawingColours[4]);
   if(showAnchors&&curveAnchors!=null)
    using(var anchor=new Pen(drawingColours[6]))foreach(PointF p in curveAnchors.Points)
     graphics.DrawEllipse(anchor,origin.X+p.X*zoom-3,origin.Y-p.Y*zoom-3,6,6);
   if(showFill)
    using(var fill=new Pen(drawingColours[7]))foreach(var line in NativeDrawDocument.FillPreview(glyph,origin.X,origin.Y,zoom,zoom,(float)fillStep.Value,preview.Width))
     graphics.DrawLine(fill,line.Item1,line.Item2,line.Item1,line.Item3);
   if(showPoints.Checked){
    float radius=PointRadius;
    using(var normal=new SolidBrush(drawingColours[9]))
    using(var selected=new SolidBrush(drawingColours[10])){
     int active=points.CurrentCell==null?-1:points.CurrentCell.RowIndex;
     for(int i=0;i<glyph.Points.Length;i++)if(i!=active)DrawPointMarker(graphics,normal,glyph.Points[i],radius);
     if(active>=0&&active<glyph.Points.Length)DrawPointMarker(graphics,selected,glyph.Points[active],radius);
    }
   }
  }

  float PointRadius{get{return Math.Max(3,3*NativeDisplay.Scale(preview));}}
  void DrawPointMarker(Graphics g,Brush ink,PointF p,float r){
   g.FillEllipse(ink,origin.X+p.X*zoom-r,origin.Y-p.Y*zoom-r,2*r,2*r);
  }
  void PaintSquareGrid(Graphics g){
   double step=(double)gridSpacing.Value*zoom;
   if(step<5)step*=Math.Ceiling(5/step);
   using(var pen=new Pen(drawingColours[8])){
    for(double x=origin.X%step;x<preview.Width;x+=step)if(x>=0)g.DrawLine(pen,(float)x,0,(float)x,preview.Height);
    for(double y=origin.Y%step;y<preview.Height;y+=step)if(y>=0)g.DrawLine(pen,0,(float)y,preview.Width,(float)y);
   }
  }

  void BeginPointDrag(MouseEventArgs e){
   var glyph=Current();
   int hit=-1;
   float best=(PointRadius+2)*(PointRadius+2);
   for(int i=0;i<glyph.Points.Length;i++){
    PointF p=glyph.Points[i];
    float dx=e.X-(origin.X+p.X*zoom),dy=e.Y-(origin.Y-p.Y*zoom),dist=dx*dx+dy*dy;
    if(dist<best){hit=i;best=dist;}
   }
   if(showPoints.Checked&&hit>=0){
    points.CurrentCell=points.Rows[hit].Cells[0];
    preview.Focus();
    dragPoint=hit;dragStart=e.Location;dragOriginal=glyph.Points[hit];
    dragRecorded=false;preview.Capture=true;preview.Invalidate();
   }else PlotPoint(e);
  }
  void MovePointDrag(Point location){
   if(zoom<=0||dragPoint<0||dragPoint>=points.Rows.Count)return;
   if(!dragRecorded&&location==dragStart)return;
   short x=DragCoordinate(CanvasCoordinate(dragOriginal.X+(location.X-dragStart.X)/zoom));
   short y=DragCoordinate(CanvasCoordinate(dragOriginal.Y-(location.Y-dragStart.Y)/zoom));
   var row=points.Rows[dragPoint];
   short oldX,oldY;
   if(CellWord(row,0,out oldX)&&CellWord(row,1,out oldY)&&x==oldX&&y==oldY)return;
   if(!dragRecorded){RememberPoint();dragRecorded=true;keepFrame=true;}
   WritePoint(row,x,y);
  }

  void WritePoint(DataGridViewRow row,short x,short y){
   loading=true;
   try{row.Cells[0].Value=x;row.Cells[1].Value=y;}finally{loading=false;}
   Changed();
  }

  double CanvasCoordinate(double value){
   double step=snapToGrid.Checked?(double)gridSpacing.Value:1;
   double nearest=Math.Round(value/step,MidpointRounding.AwayFromZero);
   return Math.Max(Math.Ceiling(Int16.MinValue/step),Math.Min(Math.Floor(Int16.MaxValue/step),nearest))*step;
  }
  static short DragCoordinate(double v){return (short)Math.Max(Int16.MinValue,Math.Min(Int16.MaxValue,Math.Round(v,MidpointRounding.AwayFromZero)));}


  void PaintGrid(Graphics graphics){
   float step=zoom*(float)gridScale.Value;
   using(var grid=new Pen(drawingColours[3])){
   for(int row=21;row<=77;row+=14)
    graphics.DrawLine(grid,origin.X-25*step,origin.Y-row*step,origin.X+75*step,origin.Y-row*step);
   for(int column=0;column<=64;column+=16)
    graphics.DrawLine(grid,origin.X+column*step,origin.Y-91*step,origin.X+column*step,origin.Y+21*step);
   graphics.DrawLine(grid,origin.X-10*step,origin.Y-7*step,origin.X+30*step,origin.Y-7*step);
   graphics.DrawLine(grid,origin.X-4*step,origin.Y,origin.X+4*step,origin.Y);}
  }

  void PlotPoint(MouseEventArgs e){
   if(!plot.Checked||e.Button!=MouseButtons.Left||zoom<=0)return;
   if(Current().Points.Length>=NativeDrawDocument.Capacity)throw new InvalidDataException("The drawing has 4,000 vectors.");
   double rawX=(e.X-origin.X)/zoom,rawY=(origin.Y-e.Y)/zoom;
   if(rawX<Int16.MinValue-.5||rawX>Int16.MaxValue+.5||rawY<Int16.MinValue-.5||rawY>Int16.MaxValue+.5)return;
   double x=CanvasCoordinate(rawX),y=CanvasCoordinate(rawY);
   if(x<Int16.MinValue||x>Int16.MaxValue||y<Int16.MinValue||y>Int16.MaxValue)return;
   RememberPoint();
   points.Rows.Add((short)x,(short)y,(short)operation.Value);
   operation.Value=0;
   Changed();
  }

  void SelectPointAt(Point location){
   var glyph=Current();
   if(glyph.Points.Length==0||zoom<=0)return;
   float x=(location.X-origin.X)/zoom,y=(origin.Y-location.Y)/zoom;
   int closest=0;
   double best=Double.PositiveInfinity;
   for(int index=0;index<glyph.Points.Length;index++){
    var p=glyph.Points[index];
    double dx=(double)p.X-x,dy=(double)p.Y-y,distance=dx*dx+dy*dy;
    if(distance<best){closest=index;best=distance;}
   }
   points.CurrentCell=points.Rows[closest].Cells[0];
   points.Focus();
  }

  void InsertPoint(bool insert){
   var glyph=Current();
   if(glyph.Points.Length==NativeDrawDocument.Capacity)
    throw new InvalidDataException("The drawing has 4,000 vectors.");
   int index=insert?SelectedRows(glyph,1).Last()+1:glyph.Points.Length;
   PointF basis=index==0?PointF.Empty:glyph.Points[index-1];
   using(var dialog=new ScoreDialog("DRAW-point"){Text=insert?"Insert point":"Add point",ClientSize=new Size(485,125),
    StartPosition=FormStartPosition.CenterParent}){
    var fields=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
    var x=new NumericUpDown{Minimum=Int16.MinValue,Maximum=Int16.MaxValue,Width=68};
    var y=new NumericUpDown{Minimum=Int16.MinValue,Maximum=Int16.MaxValue,Width=68};
    var z=new NumericUpDown{Minimum=-3,Maximum=3,Value=index==0?1:0,Width=55};
    var relative=new CheckBox{Text="Relative to preceding point",AutoSize=true};
    var accept=new Button{Text="Add",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.AddRange(new Control[]{new Label{Text="X",AutoSize=true},x,new Label{Text="Y",AutoSize=true},y,
     new Label{Text="Operation",AutoSize=true},z,relative,accept});
    dialog.Controls.Add(fields);dialog.AcceptButton=accept;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    double px=(double)x.Value+(relative.Checked?basis.X:0);
    double py=(double)y.Value+(relative.Checked?basis.Y:0);
    if(px<Int16.MinValue||px>Int16.MaxValue||py<Int16.MinValue||py>Int16.MaxValue)
     throw new InvalidDataException("The point leaves DRAW's coordinate range.");
    RememberPoint();
    points.Rows.Insert(index,(short)px,(short)py,(short)z.Value);
    points.CurrentCell=points.Rows[index].Cells[0];
    Changed();
   }
  }

  void CloseContour(){
   var glyph=Current();
   int first=Array.FindLastIndex(glyph.Operations,op=>op>0);
   if(first<0)return;
   if(glyph.Points.Length==NativeDrawDocument.Capacity)
    throw new InvalidDataException("The drawing has 4,000 vectors.");
   RememberPoint();
   PointF start=glyph.Points[first];
   points.Rows.Add(start.X,start.Y,0);
   Changed();
  }

  void DeleteLastPoint(){
   var glyph=Current();
   if(glyph.Points.Length==0)return;
   RememberPoint();
   points.Rows.RemoveAt(glyph.Points.Length-1);
  }

  void TransformDrawing(bool resize,bool selected=false,bool copy=false,int[] rememberedRows=null){
   var glyph=Current();
   int[] rows=selected?rememberedRows??SelectedRows(glyph,1):null;
   Glyph target=selected?Group(glyph,rows):glyph;
   string title=copy?"Copy points":resize?"Resize drawing":"Move drawing";
   Point movement=Point.Empty;
   using(var dialog=new ScoreDialog(title){Text=title,ClientSize=new Size(330,135),StartPosition=FormStartPosition.CenterParent}){
    var fields=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
    var x=new NumericUpDown{Minimum=Int16.MinValue,Maximum=Int16.MaxValue,DecimalPlaces=resize?3:0,Value=resize?1:0};
    var y=new NumericUpDown{Minimum=Int16.MinValue,Maximum=Int16.MaxValue,DecimalPlaces=x.DecimalPlaces,Value=x.Value};
    var apply=new Button{Text="Apply",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.AddRange(new Control[]{new Label{Text="X",AutoSize=true},x,new Label{Text="Y",AutoSize=true},y,apply});
    dialog.Controls.Add(fields);dialog.AcceptButton=apply;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    if(resize)NativeDrawDocument.Resize(target,(float)x.Value,(float)y.Value);
    else{
     movement=new Point((int)x.Value,(int)y.Value);
     NativeDrawDocument.Move(target,movement.X,movement.Y);
    }
   }
   if(copy){
    if(glyph.Points.Length+target.Points.Length>NativeDrawDocument.Capacity)
     throw new InvalidDataException("The copied group exceeds DRAW's 4,000-vector capacity.");
    if(target.Operations[0]<=0)target.Operations[0]=1;
    glyph.Points=glyph.Points.Concat(target.Points).ToArray();
    glyph.Operations=glyph.Operations.Concat(target.Operations).ToArray();
   }else if(selected)Array.Copy(target.Points,0,glyph.Points,rows[0],rows.Length);
   ApplyGlyph(glyph);
   if(copy)lastCopyRows=rows;
   else if(!resize){
    lastMove=movement;lastMoveRows=rows;canRepeatMove=true;
   }
  }

  void RepeatMove(bool reverse){
   if(!canRepeatMove)return;
   var glyph=Current();
   int[] rows=lastMoveRows;
   int direction=reverse?-1:1;
   if(rows==null)NativeDrawDocument.Move(glyph,lastMove.X*direction,lastMove.Y*direction);
   else{
    var group=Group(glyph,rows);
    NativeDrawDocument.Move(group,lastMove.X*direction,lastMove.Y*direction);
    Array.Copy(group.Points,0,glyph.Points,rows[0],rows.Length);
   }
   ApplyGlyph(glyph);
   lastMoveRows=rows;canRepeatMove=true;
  }

  void MoveToOrigin(){
   var glyph=Current();
   if(glyph.Points.Length==0)return;
   NativeDrawDocument.Move(glyph,-(int)glyph.Bounds.Left,-(int)glyph.Bounds.Top);
   ApplyGlyph(glyph);
  }

  void RotateDrawing(){RotateDrawing(false);}

  void RotateDrawing(bool selected){
   var glyph=Current();
   int[] rows=selected?SelectedRows(glyph,1):null;
   Glyph target=selected?Group(glyph,rows):glyph;
   using(var dialog=new ScoreDialog("DRAW-rotation"){Text="Rotate drawing",ClientSize=new Size(300,110),StartPosition=FormStartPosition.CenterParent}){
    var fields=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
    var angle=new NumericUpDown{Minimum=-360,Maximum=360,DecimalPlaces=3};
    var apply=new Button{Text="Rotate",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.AddRange(new Control[]{new Label{Text="Degrees",AutoSize=true},angle,apply});
    dialog.Controls.Add(fields);dialog.AcceptButton=apply;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    NativeDrawDocument.Rotate(target,(float)angle.Value);
   }
   if(selected)Array.Copy(target.Points,0,glyph.Points,rows[0],rows.Length);
   ApplyGlyph(glyph);
  }

  void FitCurve(){
   var glyph=Current();
   var rows=SelectedRows(glyph,2);
   Glyph anchors=Group(glyph,rows);
   int originalLength=glyph.Points.Length;
   using(var dialog=new ScoreDialog("DRAW-curve"){Text="Fit curve",ClientSize=new Size(300,85),
    StartPosition=FormStartPosition.CenterParent}){
    var fields=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
    var divisions=new NumericUpDown{Minimum=1,Maximum=NativeDrawDocument.Capacity,Value=20};
    var fit=new Button{Text="Fit",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.AddRange(new Control[]{new Label{Text="Points per segment",AutoSize=true},divisions,fit});
    dialog.Controls.Add(fields);dialog.AcceptButton=fit;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    curve.Replace(glyph,rows[0],rows[rows.Length-1],(int)divisions.Value);
   }
   ApplyGlyph(glyph);
   curveAnchors=anchors;
   fittedFirst=rows[0];
   fittedLength=glyph.Points.Length-originalLength+rows.Length;
   showAnchors=true;
  }

  void RestoreConstructionPoints(){
   if(curveAnchors==null)return;
   var glyph=Current();
   int count=curveAnchors.Points.Length;
   int start=fittedFirst,end=start+fittedLength;
   glyph.Points=glyph.Points.Take(start).Concat(curveAnchors.Points).Concat(glyph.Points.Skip(end)).ToArray();
   glyph.Operations=glyph.Operations.Take(start).Concat(curveAnchors.Operations)
    .Concat(glyph.Operations.Skip(end)).ToArray();
   ApplyGlyph(glyph);
   points.ClearSelection();
   for(int index=start;index<start+count;index++)
    foreach(DataGridViewCell cell in points.Rows[index].Cells)cell.Selected=true;
  }

  void Thicken(){
   var glyph=Current();
   using(var dialog=new ScoreDialog("DRAW-thickness"){Text="Thicken drawing",ClientSize=new Size(440,85),
    StartPosition=FormStartPosition.CenterParent}){
    var fields=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
    var width=new NumericUpDown{Minimum=-32000,Maximum=32000,DecimalPlaces=3,Value=5,Width=75};
    var taper=new NumericUpDown{Minimum=-100,Maximum=100,DecimalPlaces=3,Width=75};
    var apply=new Button{Text="Thicken",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.AddRange(new Control[]{new Label{Text="Thickness",AutoSize=true},width,
     new Label{Text="Taper fraction",AutoSize=true},taper,apply});
    dialog.Controls.Add(fields);dialog.AcceptButton=apply;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    NativeDrawThickening.Apply(glyph,(float)width.Value,(float)taper.Value);
   }
   ApplyGlyph(glyph);
  }

  void AddArc(){
   var glyph=Current();
   using(var dialog=new ScoreDialog("DRAW-arc"){Text="Circle or arc",ClientSize=new Size(375,260),
    StartPosition=FormStartPosition.CenterParent}){
    var fields=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=8,Padding=new Padding(12)};
    string[] labels={"First degree","Last degree","First point X","First point Y","Radius X","Radius Y","Degree increment"};
    int[] defaults={0,0,0,0,50,0,3};
    var numbers=new NumericUpDown[labels.Length];
    for(int i=0;i<labels.Length;i++){
     numbers[i]=new NumericUpDown{Minimum=i==6?1:i==1?0:i==0?-359:-32000,
      Maximum=i==0||i==6?360:i==1?361:32000,Value=defaults[i],DecimalPlaces=i==4||i==5?3:0};
     fields.Controls.Add(new Label{Text=labels[i],Anchor=AnchorStyles.Left,AutoSize=true},0,i);
     fields.Controls.Add(numbers[i],1,i);
    }
    var apply=new Button{Text="Add",AutoSize=true,DialogResult=DialogResult.OK};
    fields.Controls.Add(apply,1,labels.Length);
    dialog.Controls.Add(fields);dialog.AcceptButton=apply;
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    NativeDrawDocument.AddArc(glyph,(int)numbers[0].Value,(int)numbers[1].Value,
     new PointF((float)numbers[2].Value,(float)numbers[3].Value),(float)numbers[4].Value,
     (float)numbers[5].Value,(int)numbers[6].Value);
   }
   ApplyGlyph(glyph);
  }

  int[] SelectedRows(Glyph glyph,int minimum){
   var rows=points.SelectedCells.Cast<DataGridViewCell>().Select(cell=>cell.RowIndex)
    .Where(index=>index<glyph.Points.Length).Distinct().OrderBy(index=>index).ToArray();
   if(rows.Length<minimum||rows[rows.Length-1]-rows[0]+1!=rows.Length)
    throw new InvalidDataException("Select a group of consecutive point rows.");
   return rows;
  }

  void StoreOverlay(){
   var glyph=Current();
   var rows=SelectedRows(glyph,2);
   overlay=Group(glyph,rows);
   if(overlay.Operations[0]<=0)overlay.Operations[0]=1;
   showOverlay.Enabled=true;
   showOverlay.Checked=true;
   preview.Invalidate();
  }

  static Glyph Group(Glyph glyph,int[] rows){
   return new Glyph{Points=rows.Select(index=>glyph.Points[index]).ToArray(),
    Operations=rows.Select(index=>glyph.Operations[index]).ToArray()};
  }

  void DeleteGroup(){
   var glyph=Current();
   var rows=SelectedRows(glyph,1);
   int first=rows[0],past=first+rows.Length;
   glyph.Points=glyph.Points.Take(first).Concat(glyph.Points.Skip(past)).ToArray();
   glyph.Operations=glyph.Operations.Take(first).Concat(glyph.Operations.Skip(past)).ToArray();
   ApplyGlyph(glyph);
  }

  void RememberPoint(int pendingRow=-1){
   if(loading)return;
   try{
    var glyph=ReadGridSnapshot(pendingRow);
    if(undo.Count>0&&EqualDrawing(undo.Peek().Item1,glyph))return;
    undo.Push(Tuple.Create(glyph,dirty));
   }
   catch(InvalidDataException){}
  }

  static bool EqualDrawing(Glyph a,Glyph b){return a.Name==b.Name&&a.Points.SequenceEqual(b.Points)&&a.Operations.SequenceEqual(b.Operations);}
  void ApplyGlyph(Glyph glyph){RememberPoint();ShowGlyph(glyph);}
  void UndoPoint(){RestoreHistory(undo,redo);}
  void RedoPoint(){RestoreHistory(redo,undo);}

  void RestoreHistory(Stack<Tuple<Glyph,bool>> from,Stack<Tuple<Glyph,bool>> to){
   if(points.IsCurrentCellInEditMode)points.CancelEdit();
   Glyph current=null;
   try{current=ReadGrid();}catch(InvalidDataException){}
   while(from.Count>0){
    var previous=from.Pop();
    if(current!=null&&EqualDrawing(previous.Item1,current))continue;
    if(current!=null)to.Push(Tuple.Create(current,dirty));
    int selected=points.CurrentCell==null?-1:points.CurrentCell.RowIndex;
    restoringHistory=true;
    try{
     ShowGlyph(previous.Item1);
     if(selected>=0&&selected<previous.Item1.Points.Length)points.CurrentCell=points.Rows[selected].Cells[0];
     dirty=savedDrawing==null?previous.Item2:!EqualDrawing(previous.Item1,savedDrawing);
    }finally{restoringHistory=false;}
    break;
   }
  }

  void NudgePoint(Keys direction,int step){
   var cell=points.CurrentCell;
   if(cell==null||cell.OwningRow.IsNewRow)return;
   short oldX,oldY;
   var row=cell.OwningRow;
   if(!CellWord(row,0,out oldX)||!CellWord(row,1,out oldY))return;
   int dx=direction==Keys.Left?-step:direction==Keys.Right?step:0;
   int dy=direction==Keys.Down?-step:direction==Keys.Up?step:0;
   short x=DragCoordinate(oldX+dx),y=DragCoordinate(oldY+dy);
   if(x==oldX&&y==oldY)return;
   RememberPoint();
   keepFrame=true;
   WritePoint(row,x,y);
  }

  protected override bool ProcessCmdKey(ref Message m,Keys key){
   if(!itemName.Focused){
    if(key==(Keys.Control|Keys.Z)){Attempt(UndoPoint);return true;}
    if(key==(Keys.Control|Keys.Y)||key==(Keys.Control|Keys.Shift|Keys.Z)){Attempt(RedoPoint);return true;}
   }
   Keys direction=key&Keys.KeyCode;
   Keys modifiers=key&Keys.Modifiers;
   if(preview.Focused&&(modifiers==Keys.None||modifiers==Keys.Shift)&&
    (direction==Keys.Left||direction==Keys.Right||direction==Keys.Up||direction==Keys.Down)){
    NudgePoint(direction,modifiers==Keys.Shift?10:1);
    return true;
   }
   return base.ProcessCmdKey(ref m,key);
  }

  protected override void OnFormClosing(FormClosingEventArgs e){
   if(!ConfirmDiscard())e.Cancel=true;
   base.OnFormClosing(e);
  }
 }
}
