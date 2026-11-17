using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeJustDialog : ScoreDialog {
  readonly string root;
  readonly bool scor4;
  readonly ListBox inputs=new ListBox{Dock=DockStyle.Fill,IntegralHeight=false,HorizontalScrollbar=true};
  readonly Label status=new Label{AutoSize=true,Text="Select one or more SCORE files."};
  readonly ComboBox operation=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=245};
  readonly NumericUpDown height=new NumericUpDown{DecimalPlaces=3,Minimum=.001m,Maximum=999,Value=1,Width=78,Enabled=false};
  readonly FlowLayoutPanel limits=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,Visible=false};
  readonly CheckBox customLimits=new CheckBox{Text="Set horizontal limits",AutoSize=true};
  readonly NumericUpDown left=new NumericUpDown{DecimalPlaces=6,Minimum=-200,Maximum=300,Width=100,Enabled=false};
  readonly NumericUpDown right=new NumericUpDown{DecimalPlaces=6,Minimum=-200,Maximum=300,Value=200,Width=100,Enabled=false};

  internal NativeJustDialog(string root,bool scor4=false){
   this.root=root;this.scor4=scor4;
   Text=scor4?"JUST4":"JUST";StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(650,330);MinimumSize=new Size(480,245);
   var panel=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=4,Padding=new Padding(12)};
   panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   panel.RowStyles.Add(new RowStyle(SizeType.Percent,100));
   panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   var actions=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill};
   var heightLabel=new Label{Text="New height (file units)",AutoSize=true,Visible=false};
   var choose=new Button{Text="Open scores...",AutoSize=true};
   var numbered=new Button{Text="Open numbered series...",AutoSize=true};
   operation.Items.AddRange(new object[]{"Line up (L)","Height (H)","Justify (J)","Line up and justify (JL)",
    "Justify and height (JH)","Line up and height (LH)","Line up, justify and height (JLH)"});
   operation.SelectedIndex=0;
   operation.SelectedIndexChanged+=(sender,args)=>{
    height.Enabled=operation.SelectedIndex==1||operation.SelectedIndex>=4;
    height.Visible=heightLabel.Visible=height.Enabled;
    limits.Visible=operation.SelectedIndex==2||operation.SelectedIndex==4||
     operation.SelectedIndex==3||operation.SelectedIndex==6;
   };
   limits.Controls.AddRange(new Control[]{customLimits,new Label{Text="Left",AutoSize=true},left,new Label{Text="Right",AutoSize=true},right});
   customLimits.CheckedChanged+=(sender,args)=>left.Enabled=right.Enabled=customLimits.Checked;
   var layout=new Button{Text="Apply and save...",AutoSize=true,Enabled=false};
   var close=new Button{Text="Close",AutoSize=true};
   height.Visible=false;
   actions.Controls.AddRange(new Control[]{choose,numbered,operation,heightLabel,height,layout,close});
   panel.Controls.Add(status,0,0);panel.Controls.Add(inputs,0,1);panel.Controls.Add(limits,0,2);panel.Controls.Add(actions,0,3);
   Controls.Add(panel);
   close.Click+=(sender,args)=>Close();
   choose.Click+=(sender,args)=>{
    using(var dialog=new OpenFileDialog{Filter="SCORE files (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",Multiselect=true,RestoreDirectory=true}){
     if(dialog.ShowDialog(this)!=DialogResult.OK)return;
     SetInputs(dialog.FileNames,layout);
    }
   };
   numbered.Click+=(sender,args)=>ChooseNumbered(layout);
   layout.Click+=(sender,args)=>{
    try{SaveLayout();}
    catch(Exception error){
     if(!(error is IOException||error is InvalidDataException||error is NotSupportedException||
      error is UnauthorizedAccessException||error is ArgumentException||error is InvalidOperationException))throw;
     MessageBox.Show(this,error.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
   };
  }

  void SetInputs(string[] files,Button apply){
   inputs.Items.Clear();inputs.Items.AddRange(files.Cast<object>().ToArray());
   apply.Enabled=inputs.Items.Count!=0;
   status.Text=inputs.Items.Count+" selected";
  }

  void ChooseNumbered(Button apply){
   using(var file=new OpenFileDialog{Filter="SCORE files (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",
    Title="Choose the first input file",RestoreDirectory=true}){
    if(file.ShowDialog(this)!=DialogResult.OK)return;
    using(var dialog=new ScoreDialog("JUST-series"){Text="Numbered input series",ClientSize=new Size(330,85),
     StartPosition=FormStartPosition.CenterParent}){
     var row=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12)};
     var count=new NumericUpDown{Minimum=1,Maximum=100,Value=2};
     var open=new Button{Text="Open",AutoSize=true,DialogResult=DialogResult.OK};
     row.Controls.AddRange(new Control[]{new Label{Text="Number of files",AutoSize=true},count,open});
     dialog.Controls.Add(row);dialog.AcceptButton=open;
     if(dialog.ShowDialog(this)!=DialogResult.OK)return;
     try{
      var files=NativePageFilenameSequence.OriginalNames(file.FileName,(int)count.Value);
      var missing=files.FirstOrDefault(path=>!File.Exists(path));
      if(missing!=null)throw new FileNotFoundException("File not found: "+missing);
      SetInputs(files,apply);
     }catch(Exception error){
      if(!(error is IOException||error is NotSupportedException||error is ArgumentException||
       error is InvalidOperationException))throw;
      MessageBox.Show(this,error.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
     }
    }
   }
  }

  void SaveLayout(){
   string[] source=inputs.Items.Cast<string>().ToArray();
   if(source.Length==0)return;
   bool adjustHeight=height.Enabled;
   bool justify=operation.SelectedIndex==2||operation.SelectedIndex==4;
   bool combined=operation.SelectedIndex==3||operation.SelectedIndex==6;
   bool lineUp=operation.SelectedIndex==0||operation.SelectedIndex==5;
   var destinations=new List<string>();
    using(var dialog=new SaveFileDialog{Filter="SCORE files (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",
     DefaultExt=Path.GetExtension(source[0]).TrimStart('.'),InitialDirectory=NativeOutputLocation.Folder(root),
     Title=source.Length==1?"Save JUST output":"Choose the first output filename",
     FileName=source.Length==1?Path.GetFileNameWithoutExtension(source[0])+"-JUST"+Path.GetExtension(source[0]):
      "JUST1"+Path.GetExtension(source[0]),RestoreDirectory=true,OverwritePrompt=source.Length==1}){
     if(dialog.ShowDialog(this)!=DialogResult.OK)return;
     if(source.Length==1)destinations.Add(dialog.FileName);
     else destinations.AddRange(NativePageFilenameSequence.OriginalNames(dialog.FileName,source.Length));
    }
   if(destinations.Intersect(source,StringComparer.OrdinalIgnoreCase).Any())
    throw new IOException("Choose a different destination to preserve the input files.");
   if(source.Length>1&&destinations.Any(File.Exists)&&MessageBox.Show(this,"Replace the existing output files?",Text,
    MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)!=DialogResult.Yes)return;
   var scores=source.Select(Score.Load).ToArray();
   var notices=new List<string>();
   if(justify||combined||lineUp)
    scores=NativeJustLayout.ApplyHorizontalBatchWithMessages(scores,lineUp||combined,justify||combined,
     (justify||combined)&&customLimits.Checked?(float?)left.Value:null,
     (justify||combined)&&customLimits.Checked?(float?)right.Value:null,
     notices.Add,scor4);
   if(adjustHeight)scores=NativeJustLayout.AdjustHeightBatch(scores,(float)height.Value);
   var results=scores.Select((score,index)=>NativeJustLayout.EncodeOutput(score,index!=0)).ToArray();
   int written=0;
   try{
    for(;written<results.Length;written++)ScoreFileStorage.Write(destinations[written],results[written]);
   }finally{
    status.Text=String.Join(Environment.NewLine,new[]{"Saved "+written+" of "+results.Length+" files."}.Concat(notices));
   }
  }
 }

 public sealed partial class Editor {
  void ShowNativeJust(){using(var dialog=new NativeJustDialog(source,scor4Interface){Icon=Icon})dialog.ShowDialog(this);}
  void ShowNativeDraw(){using(var dialog=new NativeDrawDialog(NativeLibraryLocation.DirectoryFor(source,true)){Icon=Icon})dialog.ShowDialog(this);}
 }
}
