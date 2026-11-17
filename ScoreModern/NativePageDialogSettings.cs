using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed partial class NativePagePartsDialog {
  void InstallFileReordering(){
   Point start=Point.Empty;
   int dragged=-1, active=-1;
   files.MouseDown+=(sender,e)=>{start=e.Location;dragged=e.Button==MouseButtons.Left?files.IndexFromPoint(start):-1;};
   files.MouseMove+=(sender,e)=>{
    if(dragged<0||e.Button!=MouseButtons.Left)return;
    var threshold=SystemInformation.DragSize;
    if(new Rectangle(start.X-threshold.Width/2,start.Y-threshold.Height/2,threshold.Width,threshold.Height).Contains(e.Location))return;
    active=dragged;
    dragged=-1;
    try{files.DoDragDrop(new DataObject("ScoreModern.PAGE.FileRow",""),DragDropEffects.Move);}
    finally{active=-1;files.DropSlot=-1;}
   };
   files.DragOver+=(sender,e)=>{
    if(active<0||!e.Data.GetDataPresent("ScoreModern.PAGE.FileRow")){e.Effect=DragDropEffects.None;return;}
    e.Effect=DragDropEffects.Move;
    var point=files.PointToClient(new Point(e.X,e.Y));
    if(point.Y<files.ItemHeight&&files.TopIndex>0)files.TopIndex--;
    else if(point.Y>files.ClientSize.Height-files.ItemHeight&&files.TopIndex<files.Items.Count-1)files.TopIndex++;
    files.DropSlot=FileDropSlot(point);
   };
   files.DragLeave+=(sender,e)=>files.DropSlot=-1;
   files.DragDrop+=(sender,e)=>{
    if(active<0)return;
    int slot=FileDropSlot(files.PointToClient(new Point(e.X,e.Y)));
    ReorderFile(active,slot>active?slot-1:slot);
   };
  }

  int FileDropSlot(Point point){
   int index=files.IndexFromPoint(point);
   if(index<0)return point.Y<0?files.TopIndex:files.Items.Count;
   var bounds=files.GetItemRectangle(index);
   return point.Y<bounds.Top+bounds.Height/2?index:index+1;
  }

  void ReorderFile(int from,int to){
   if(from<0||to<0||from>=files.Items.Count||to>=files.Items.Count||from==to)return;
   object entry=files.Items[from];
   files.Items.RemoveAt(from);
   files.Items.Insert(to,entry);
   files.ClearSelected();
   files.SelectedIndex=to;
   InvalidatePlan();
  }

  Dictionary<string,Control> SettingsFields(){
   return new Dictionary<string,Control>{
    {"Operation",operation},{"Instrument",instrument},{"StavesPerSystem",staves},
    {"StaffSize",size},{"PageHeight",height},{"FirstIndent",firstIndent},{"OtherIndent",otherIndent},
    {"FirstPage",firstPage},{"CenterNumbers",centerNumbers},{"NumberSingleBarRests",restLabels},
    {"Transpose",transposeWholeTone},{"TransposeClef",transposeClef},{"NormalizeStems",normalizeStems},{"AddKey",transposeAddKey},{"TurnControls",turnControls},
    {"RestStyle",restStyle},{"StartMeasure",startMeasure},{"EveryMeasure",everyMeasure},
    {"BarsPerLine",manualLines},{"LinesPerPage",linesPerPage},{"FixedProposal",fixedProposal},
    {"BarRange",barRange},{"OldMarkedPrecision",oldMarkedPrecision},{"OmitLeftItems",omit},
    {"PageFilenames",names}
   };
  }

  void SaveSettings(){
   using(var dialog=new SaveFileDialog{Title="Save PAGE settings",Filter="PAGE settings (*.scr)|*.scr",FileName="PAGE.SCR",DefaultExt="scr",RestoreDirectory=true}){
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    WriteSettings(dialog.FileName);
    report.AppendText(Environment.NewLine+"Saved settings to "+dialog.FileName);
   }
  }

  void WriteSettings(string path){
   var lines=new List<string>{"[PAGE]","Format=1"};
   foreach(var field in SettingsFields()){
    var number=field.Value as NumericUpDown;
    var choice=field.Value as ComboBox;
    var check=field.Value as CheckBox;
    string value=number!=null?number.Value.ToString(CultureInfo.InvariantCulture):
     choice!=null?choice.SelectedIndex.ToString(CultureInfo.InvariantCulture):
     check!=null?(check.Checked?"true":"false"):field.Value.Text;
    lines.Add(field.Key+"="+value);
   }
   lines.Add("");
   lines.Add("[Files]");
   for(int index=0;index<files.Items.Count;index++)lines.Add((index+1).ToString(CultureInfo.InvariantCulture)+"="+files.Items[index]);
   ScoreFileStorage.Write(path,System.Text.Encoding.UTF8.GetBytes(String.Join(Environment.NewLine,lines)+Environment.NewLine));
  }

  void OpenSettings(){
   using(var dialog=new OpenFileDialog{Title="Open PAGE settings",Filter="PAGE settings (*.scr;*.ini)|*.scr;*.ini|All files (*.*)|*.*",RestoreDirectory=true}){
    if(dialog.ShowDialog(this)==DialogResult.OK)ReadSettings(dialog.FileName);
   }
  }

  void ReadSettings(string path){
   var options=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
   var inputs=new SortedDictionary<int,string>();
   string section="";
   foreach(var line in File.ReadLines(path)){
    string trimmed=line.Trim();
    if(trimmed.Length==0||trimmed.StartsWith(";")||trimmed.StartsWith("#"))continue;
    if(trimmed.StartsWith("[")&&trimmed.EndsWith("]")){section=trimmed;continue;}
    int equals=line.IndexOf('=');
    if(equals<1)throw new InvalidDataException("Invalid line in PAGE settings.");
    string key=line.Substring(0,equals).Trim(),value=line.Substring(equals+1).Trim();
    if(section=="[PAGE]")options[key]=value;
    else if(section=="[Files]"){
     int index;
     if(!Int32.TryParse(key,NumberStyles.None,CultureInfo.InvariantCulture,out index)||index<1||inputs.ContainsKey(index)||value.Length==0)
      throw new InvalidDataException("Invalid file entry in PAGE settings.");
     inputs.Add(index,Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path)),value)));
    }
   }
   string format;
   if(!options.TryGetValue("Format",out format)||format!="1")throw new InvalidDataException("Unsupported PAGE settings format.");
   var changes=new List<Action>();
   foreach(var field in SettingsFields()){
    string value;
    if(!options.TryGetValue(field.Key,out value)){
     if(field.Key=="TransposeClef")value="0";
     else if(field.Key=="NormalizeStems")value="false";
     else throw new InvalidDataException("Missing PAGE setting "+field.Key+".");
    }
    var number=field.Value as NumericUpDown;
    var choice=field.Value as ComboBox;
    var check=field.Value as CheckBox;
    if(number!=null){
     decimal selected;
     if(!Decimal.TryParse(value,NumberStyles.Number,CultureInfo.InvariantCulture,out selected)||selected<number.Minimum||selected>number.Maximum)
      throw new InvalidDataException("Invalid PAGE setting "+field.Key+".");
     changes.Add(()=>number.Value=selected);
    }else if(choice!=null){
     int selected;
     if(!Int32.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out selected)||selected>=choice.Items.Count)
      throw new InvalidDataException("Unavailable PAGE option "+field.Key+".");
     changes.Add(()=>choice.SelectedIndex=selected);
    }else if(check!=null){
     bool selected;
     if(!Boolean.TryParse(value,out selected))throw new InvalidDataException("Invalid PAGE setting "+field.Key+".");
     changes.Add(()=>check.Checked=selected);
    }else{
     var input=field.Value;
     changes.Add(()=>input.Text=value);
    }
   }
   foreach(var change in changes)change();
   files.Items.Clear();
   foreach(string input in inputs.Values)files.Items.Add(input);
   EnablePlanningControls();
   InvalidatePlan();
   var missing=inputs.Values.Where(input=>!File.Exists(input)).ToArray();
   if(missing.Length>0)report.AppendText(Environment.NewLine+"Missing input files:"+Environment.NewLine+String.Join(Environment.NewLine,missing));
  }
 }
}
