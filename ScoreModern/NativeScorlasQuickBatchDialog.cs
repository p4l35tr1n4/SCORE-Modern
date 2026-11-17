using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeScorlasQuickBatchDialog:ScoreDialog {
  readonly TextBox start=new TextBox{Dock=DockStyle.Fill};
  readonly NumericUpDown perPage=new NumericUpDown{Minimum=1,Maximum=1000,Value=2},count=new NumericUpDown{Minimum=1,Maximum=100000,Value=1};
  readonly ComboBox countKind=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList},sequence=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};
  readonly DataGridView preview=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,RowHeadersVisible=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
  readonly Label status=new Label{AutoSize=true};readonly Button use=new Button{Text="Use batch",AutoSize=true,Enabled=false};
  internal List<List<string>> Pages{get;private set;}
  internal NativeScorlasQuickBatchDialog(string initial):base("scorlas-quick-batch"){
   Text="SCORLAS — Quick batch";Font=SystemFonts.MessageBoxFont;ClientSize=new Size(800,500);MinimumSize=new Size(600,400);StartPosition=FormStartPosition.CenterParent;
   var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=1,RowCount=4};layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   var fields=new TableLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,ColumnCount=3};fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));fields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
   fields.Controls.Add(new Label{Text="Starting page/file",AutoSize=true,Anchor=AnchorStyles.Left},0,0);fields.Controls.Add(start,1,0);var browse=new Button{Text="Browse...",AutoSize=true};fields.Controls.Add(browse,2,0);
   fields.Controls.Add(new Label{Text="Filename sequence",AutoSize=true,Anchor=AnchorStyles.Left},0,1);sequence.Items.AddRange(new object[]{"Page number + part letter (MAT001A, MAT001B, MAT002A)","Consecutive numbers (wandh01, wandh02)","Consecutive final characters (SONAA, SONAB)"});sequence.Width=440;fields.Controls.Add(sequence,1,1);
   fields.Controls.Add(new Label{Text="Files per page",AutoSize=true,Anchor=AnchorStyles.Left},0,2);fields.Controls.Add(perPage,1,2);
   countKind.Items.AddRange(new object[]{"Number of pages","Number of files"});countKind.SelectedIndex=0;countKind.Width=150;fields.Controls.Add(countKind,0,3);fields.Controls.Add(count,1,3);var generate=new Button{Text="Preview",AutoSize=true};fields.Controls.Add(generate,2,3);
   preview.Columns.Add("Page","Page");preview.Columns.Add("Files","Files (top to bottom)");preview.Columns.Add("Missing","Missing files");preview.Columns[0].FillWeight=15;preview.Columns[1].FillWeight=60;preview.Columns[2].FillWeight=35;preview.DefaultCellStyle.WrapMode=DataGridViewTriState.True;preview.AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.AllCells;
   layout.Controls.Add(fields,0,0);layout.Controls.Add(preview,0,1);layout.Controls.Add(status,0,2);var buttons=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft};var cancel=new Button{Text="Cancel",AutoSize=true,DialogResult=DialogResult.Cancel};buttons.Controls.AddRange(new Control[]{cancel,use});layout.Controls.Add(buttons,0,3);Controls.Add(layout);CancelButton=cancel;
   Action invalidate=()=>{Pages=null;use.Enabled=false;status.Text="Preview the definition before using it.";preview.Rows.Clear();};start.TextChanged+=(s,e)=>invalidate();perPage.ValueChanged+=(s,e)=>invalidate();count.ValueChanged+=(s,e)=>invalidate();sequence.SelectedIndexChanged+=(s,e)=>invalidate();countKind.SelectedIndexChanged+=(s,e)=>invalidate();
   browse.Click+=(s,e)=>{using(var d=new OpenFileDialog{Filter="SCORE files (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",RestoreDirectory=true})if(d.ShowDialog(this)==DialogResult.OK){start.Text=d.FileName;sequence.SelectedIndex=(int)NativeScorlasBatch.Detect(d.FileName);}};
   generate.Click+=(s,e)=>Generate();use.Click+=(s,e)=>{if(Pages!=null){DialogResult=DialogResult.OK;Close();}};
   start.Text=initial??"";sequence.SelectedIndex=(int)NativeScorlasBatch.Detect(start.Text);status.MaximumSize=new Size(740,0);
  }
  void Generate(){try{
   var pages=NativeScorlasBatch.Define(start.Text,(int)perPage.Value,(int)count.Value,countKind.SelectedIndex==1,(ScorlasInputSequence)sequence.SelectedIndex);preview.Rows.Clear();int missing=0;
   for(int n=0;n<pages.Count;n++){var absent=pages[n].Where(file=>!File.Exists(file)).ToArray();missing+=absent.Length;preview.Rows.Add(n+1,String.Join(", ",pages[n].Select(Path.GetFileName).ToArray()),String.Join(", ",absent.Select(Path.GetFileName).ToArray()));}
   Pages=pages;use.Enabled=true;status.Text=pages.Count+" pages, "+pages.Sum(page=>page.Count)+" files; "+missing+" missing. Each page remains editable. Missing files must be resolved before export.";
  }catch(Exception error){if(!(error is ArgumentException||error is NotSupportedException||error is OverflowException||error is IOException))throw;Pages=null;use.Enabled=false;status.Text=error.Message;}}
 }
}
