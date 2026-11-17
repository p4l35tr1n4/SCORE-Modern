using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal sealed partial class NativePagePartsDialog:ScoreDialog {
  readonly ScorlasFileList files=new ScorlasFileList{Dock=DockStyle.Fill,IntegralHeight=false,HorizontalScrollbar=true,SelectionMode=SelectionMode.MultiExtended};
  readonly NumericUpDown instrument=new NumericUpDown{Minimum=1,Maximum=32767,Value=1,Width=85};
  readonly NumericUpDown staves=new NumericUpDown{Minimum=1,Maximum=32,Value=1,Width=65};
  readonly NumericUpDown size=new NumericUpDown{Minimum=.01m,Maximum=10,Value=.9m,DecimalPlaces=2,Increment=.05m,Width=75};
  readonly NumericUpDown height=new NumericUpDown{Minimum=.01m,Maximum=100,Value=10,DecimalPlaces=2,Increment=.25m,Width=75};
  readonly NumericUpDown firstIndent=new NumericUpDown{Minimum=0,Maximum=199,Width=65};
  readonly NumericUpDown otherIndent=new NumericUpDown{Minimum=0,Maximum=199,Width=65};
  readonly NumericUpDown firstPage=new NumericUpDown{Minimum=0,Maximum=32767,Value=1,Width=75};
  readonly CheckBox centerNumbers=new CheckBox{Text="Center page numbers",AutoSize=true};
  readonly CheckBox restLabels=new CheckBox{Text="Number one-bar rests",Checked=true,AutoSize=true};
  readonly ComboBox transposeWholeTone=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,DrawMode=DrawMode.OwnerDrawFixed,Width=280};
  readonly ComboBox transposeClef=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=105};
  static readonly short[] transposeIntervals={0,2,-2,2,7,12,-7,-12,1,-1,2,2,2,2};
  readonly CheckBox transposeAddKey=new CheckBox{Text="Add key if absent",AutoSize=true,Enabled=false};
  readonly CheckBox normalizeStems=new CheckBox{Text="Normalize stems",AutoSize=true,Enabled=false};
  readonly TextBox turnControls=new TextBox{Width=85,Text="1, 4",Visible=false};
  readonly Label turnLabel=new Label{Text="Turn gap (quarter notes), requested pages",AutoSize=true,Visible=false,Margin=new Padding(3,6,3,3)};
  readonly ToolTip optionTips=new ToolTip();
  readonly ComboBox restStyle=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=100};
  readonly NumericUpDown startMeasure=new NumericUpDown{Minimum=-1,Maximum=32767,Width=72};
  readonly CheckBox everyMeasure=new CheckBox{Text="Number every measure",AutoSize=true,Enabled=false};
  readonly TextBox manualLines=new TextBox{Width=180};
  readonly TextBox linesPerPage=new TextBox{Width=135};
  readonly TextBox fixedProposal=new TextBox{Width=90};
  readonly TextBox barRange=new TextBox{Width=90};
  readonly CheckBox oldMarkedPrecision=new CheckBox{Text="Use Old Page Data precision (three bars)",AutoSize=true,Visible=false};
  readonly ComboBox operation=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=165};
  readonly Label rangeLabel=new Label{AutoSize=true,Margin=new Padding(3,6,3,3)};
  readonly CheckBox omit=new CheckBox{Text="Omit items more than 1 step left of the staff",Checked=true,AutoSize=true};
  readonly ComboBox names=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=175};
  readonly TextBox report=new TextBox{Multiline=true,ReadOnly=true,WordWrap=false,Dock=DockStyle.Fill,ScrollBars=ScrollBars.Both};
  readonly Button preview=new Button{Text="Preview layout",AutoSize=true,Enabled=false};
  readonly Button save=new Button{Text="Save pages...",AutoSize=true,Enabled=false};
  NativePagePreparedPart prepared;
  internal NativePageFileBank OutputBank=new NativePageFileBank(new byte[14200*4]);
  string resourceRoot;

  readonly bool scor4;
  internal NativePagePartsDialog(string current,bool scor4=false):base("page-parts"){
   this.scor4=scor4;
   Text=(scor4?"PAGE4":"PAGE")+" — Layout and Parts";Font=SystemFonts.MessageBoxFont;StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(730,510);MinimumSize=new Size(570,410);
   var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=1,RowCount=7};layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   for(int n=0;n<7;n++)layout.RowStyles.Add(n==2||n==5?new RowStyle(SizeType.Percent,n==2?45:55):new RowStyle(SizeType.AutoSize));
   layout.Controls.Add(new Label{AutoSize=true,Text="Lay out full-score sections or extract instrument parts as PAG files. List input scores in reading order."},0,0);
   var actions=Row();operation.Items.AddRange(new object[]{"Automatic Parts","Layout marked section","Find page turns","Old Page Data: page turns","Layout full score","Old Page Data: full score"});operation.SelectedIndex=0;Field(actions,"Operation",operation);actions.Controls.Add(Button("Add files...",Add));actions.Controls.Add(Button("Remove",Remove));actions.Controls.Add(Button("Move up",()=>MoveFile(-1)));actions.Controls.Add(Button("Move down",()=>MoveFile(1)));layout.Controls.Add(actions,0,1);layout.Controls.Add(files,0,2);
   var fields=Row();Field(fields,"Instrument ID",instrument);Field(fields,"Staves per system",staves);Field(fields,"Staff size",size);Field(fields,"Page height (inches)",height);layout.Controls.Add(fields,0,3);
   var options=Row();Field(options,"First line indent",firstIndent);Field(options,"Other line indent",otherIndent);Field(options,"First page number (0 = none)",firstPage);options.Controls.Add(centerNumbers);restStyle.Items.AddRange(new object[]{"American","European"});restStyle.SelectedIndex=0;Field(options,"Multi-bar rests",restStyle);options.Controls.Add(restLabels);options.Controls.Add(omit);names.Items.AddRange(new object[]{"SCORE sequence","Windows numeric sequence"});names.SelectedIndex=0;names.Enabled=!NativeScoreBehaviour.Enabled;Field(options,"Page filenames",names);layout.Controls.Add(options,0,4);
   Field(options,"First measure (−1 = pickup, 0 = none)",startMeasure);options.Controls.Add(everyMeasure);
   ConfigureTranspositions();
   Field(options,"Transpose",transposeWholeTone);
   transposeClef.Items.AddRange(new object[]{"Keep","Treble","Bass","Alto","Tenor"});
   transposeClef.SelectedIndex=0;
   Field(options,"Change clef",transposeClef);
   optionTips.SetToolTip(transposeWholeTone,"Transpose the selected part. Available clef, key, and stem controls depend on the interval.");
   options.Controls.Add(transposeAddKey);
   optionTips.SetToolTip(transposeAddKey,"Add a key signature where the transposed part has none.");
   options.Controls.Add(normalizeStems);
   Field(options,"Bars per line (optional; 0 ends a page)",manualLines);
   Field(options,"Lines per page (optional)",linesPerPage);
   Field(options,"Fixed proposal (pages, lines per page)",fixedProposal);
   options.Controls.Add(rangeLabel);options.Controls.Add(barRange);options.Controls.Add(oldMarkedPrecision);options.Controls.Add(turnLabel);options.Controls.Add(turnControls);turnControls.AccessibleName=turnLabel.Text;EnablePlanningControls();
   layout.Controls.Add(report,0,5);
   var footer=new TableLayoutPanel{Dock=DockStyle.Fill,AutoSize=true,ColumnCount=2,RowCount=1,Margin=Padding.Empty};
   footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
   footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   var settingsButtons=Row();
   settingsButtons.Controls.AddRange(new Control[]{Button("Open settings...",OpenSettings),Button("Save settings...",SaveSettings)});
   footer.Controls.Add(settingsButtons,0,0);
   var buttons=Row();buttons.FlowDirection=FlowDirection.RightToLeft;
   var close=new Button{Text="Close",AutoSize=true,DialogResult=DialogResult.Cancel};
   buttons.Controls.AddRange(new Control[]{close,save,preview});
   footer.Controls.Add(buttons,1,0);
   layout.Controls.Add(footer,0,6);Controls.Add(layout);CancelButton=close;
   foreach(var value in new[]{instrument,staves,size,height,firstIndent,otherIndent,firstPage})value.ValueChanged+=(s,e)=>InvalidatePlan();
   foreach(var check in new[]{omit,centerNumbers,restLabels})check.CheckedChanged+=(s,e)=>InvalidatePlan();
   transposeWholeTone.SelectedIndexChanged+=(s,e)=>{EnablePlanningControls();InvalidatePlan();};
   transposeClef.SelectedIndexChanged+=(s,e)=>{EnablePlanningControls();InvalidatePlan();};
   transposeAddKey.CheckedChanged+=(s,e)=>InvalidatePlan();
   normalizeStems.CheckedChanged+=(s,e)=>{EnablePlanningControls();InvalidatePlan();};
   barRange.TextChanged+=(s,e)=>InvalidatePlan();
   oldMarkedPrecision.CheckedChanged+=(s,e)=>InvalidatePlan();
   turnControls.TextChanged+=(s,e)=>InvalidatePlan();
   restStyle.SelectedIndexChanged+=(s,e)=>InvalidatePlan();
   startMeasure.ValueChanged+=(s,e)=>{everyMeasure.Enabled=startMeasure.Value>0;InvalidatePlan();};everyMeasure.CheckedChanged+=(s,e)=>InvalidatePlan();
   foreach(var field in new[]{manualLines,linesPerPage,fixedProposal})field.TextChanged+=(s,e)=>{
    EnablePlanningControls();
    InvalidatePlan();
   };
   operation.SelectedIndexChanged+=(s,e)=>{EnablePlanningControls();InvalidatePlan();};
   preview.Click+=(s,e)=>Try(Preview);save.Click+=(s,e)=>Try(Save);
   InstallFileReordering();
   files.KeyDown+=(sender,e)=>{
    if(e.KeyData!=Keys.Delete)return;
    e.SuppressKeyPress=true;
    if(files.SelectedIndices.Count>0)Remove();
   };
   if(!String.IsNullOrEmpty(current)&&File.Exists(current))files.Items.Add(Path.GetFullPath(current));InvalidatePlan();
  }
  protected override void Dispose(bool disposing){if(disposing)optionTips.Dispose();base.Dispose(disposing);}
  void ConfigureTranspositions(){
   // Keep the indices stored by existing PAGE settings files.
   string[] descriptions={
    "None","B flat (Clar., Trpt., Sop. Sax)|up major 2","D (Clar., Trpt.)|down major 2",
    "Bass clef|up major 2","F (Horn, English Horn)|up perfect 5","|octave up",
    "|down perfect 5","|octave down","|up minor 2","|down minor 2","Normalize stems|up major 2",
    "Treble clef|up major 2","Alto clef|up major 2","Tenor clef|up major 2",
    "A flat (Clar. [high])|down major 6","F (Trpt. [high])|down perfect 4",
    "E (Trpt. [high])|down major 3","E flat (Clar., Trpt. [high])|down minor 3",
    "A (Clar., Trpt.)|up minor 3","G (Flute, Horn)|up perfect 4","E (Horn)|up minor 6",
    "E flat (Alto Sax, Horn)|up major 6","D (Horn)|up minor 7",
    "B flat (Tenor Sax, Bass Clar.)|up major 9","E flat (Baritone Sax)|up major 13"
   };
   transposeWholeTone.Items.AddRange(descriptions.Select(label=>label.TrimStart('|').Replace('|',' ')).ToArray());
   transposeWholeTone.DropDownWidth=Math.Max(transposeWholeTone.Width,
    transposeWholeTone.Items.Cast<string>().Max(label=>TextRenderer.MeasureText(label,Font).Width)+48);
   transposeWholeTone.DrawItem+=(sender,args)=>{
    args.DrawBackground();
    if(args.Index<0)return;
    string[] parts=descriptions[args.Index].Split('|');
    var flags=TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.SingleLine|TextFormatFlags.NoPadding;
    var bounds=args.Bounds;
    bounds.X+=3;
    bounds.Width-=3;
    if(parts[0].Length>0){
     TextRenderer.DrawText(args.Graphics,parts[0],args.Font,bounds,args.ForeColor,flags);
     int width=TextRenderer.MeasureText(args.Graphics,parts[0]+" ",args.Font,Size.Empty,flags).Width;
     bounds.X+=width;
     bounds.Width-=width;
    }
    if(parts.Length==2&&bounds.Width>0)
     using(var bold=new Font(args.Font,args.Font.Style|FontStyle.Bold))
      TextRenderer.DrawText(args.Graphics,parts[1],bold,bounds,args.ForeColor,flags);
    args.DrawFocusRectangle();
   };
   transposeWholeTone.SelectedIndex=0;
  }
  internal void EnableStemNormalization(string root){
   if(String.IsNullOrWhiteSpace(root)||resourceRoot!=null)return;
   resourceRoot=root;
  }

  static FlowLayoutPanel Row(){return new FlowLayoutPanel{Dock=DockStyle.Fill,AutoSize=true,WrapContents=true};}
  static void Field(FlowLayoutPanel row,string title,Control field){row.Controls.Add(new Label{Text=title,AutoSize=true,Margin=new Padding(3,6,3,3)});field.AccessibleName=title;row.Controls.Add(field);}
  Button Button(string title,Action action){var b=new Button{Text=title,AutoSize=true};b.Click+=(s,e)=>Try(action);return b;}
  void Try(Action action){try{action();}catch(Exception e){if(!(e is IOException||e is UnauthorizedAccessException||e is ArgumentException||e is NotSupportedException||e is InvalidOperationException||e is OverflowException))throw;report.Text=e.Message;}}
  void EnablePlanningControls(){
   bool parts=operation.SelectedIndex==0,old=operation.SelectedIndex==3||operation.SelectedIndex==5,turn=operation.SelectedIndex==2||operation.SelectedIndex==3,multi=operation.SelectedIndex==4;
   turnLabel.Visible=turnControls.Visible=turn;
   rangeLabel.Visible=barRange.Visible=parts||operation.SelectedIndex==1;
   oldMarkedPrecision.Visible=operation.SelectedIndex==1;
   foreach(var c in new Control[]{size,height,centerNumbers,firstPage})c.Enabled=!old;
   rangeLabel.Text=parts?"Use measures (first, last; optional)":"Marked boundaries (start, end)";
   barRange.AccessibleName=rangeLabel.Text;
   foreach(var c in new Control[]{instrument,firstIndent,otherIndent,startMeasure,omit,restStyle,restLabels,transposeWholeTone})c.Enabled=parts;
   staves.Enabled=parts||multi;
   int transpose=transposeWholeTone.SelectedIndex;
   transposeClef.Enabled=parts&&transpose>0&&transpose!=3&&(transpose<10||transpose>13);
   normalizeStems.Enabled=transposeClef.Enabled;
   transposeAddKey.Enabled=parts&&transpose>0&&transpose!=10;
   everyMeasure.Enabled=parts&&startMeasure.Value>0;
   manualLines.Enabled=parts&&String.IsNullOrWhiteSpace(linesPerPage.Text)&&String.IsNullOrWhiteSpace(fixedProposal.Text);
   linesPerPage.Enabled=parts&&String.IsNullOrWhiteSpace(manualLines.Text)&&String.IsNullOrWhiteSpace(fixedProposal.Text);
   fixedProposal.Enabled=parts&&String.IsNullOrWhiteSpace(manualLines.Text)&&String.IsNullOrWhiteSpace(linesPerPage.Text);
  }
  void InvalidatePlan(){files.RefreshMetrics();prepared=null;int route=operation.SelectedIndex;preview.Enabled=route==0||route==4?files.Items.Count>0:files.Items.Count==1&&(route!=3&&route!=5||Path.GetFileName((string)files.Items[0]).Equals("BARS.TMP",StringComparison.OrdinalIgnoreCase));save.Enabled=false;report.Text=route==5?"Open BARS.TMP with its source scores in the same folder.":route==4?"Add scores in reading order and set the number of staves per system.":route==3?"Open the original BARS.TMP with its score in the same folder. The verified route restores a single-staff, ten-inch job and proposes page turns.":route==2?"Choose one single-staff score. The verified PAGE turn layout uses one line per page and starts at page 1.":route==1?"Choose one score. Enter the mark to start after (0 for the beginning) and the mark to stop at. To select by bar number, enter N followed by the first and last bars.":preview.Enabled?"Preview the layout before saving.":"Add one or more input scores.";}
  void Add(){bool old=operation.SelectedIndex==3||operation.SelectedIndex==5;using(var dialog=new OpenFileDialog{Title=old?"Open PAGE Old Page Data":"Choose PAGE input scores",Filter=old?"Old Page Data (BARS.TMP)|BARS.TMP":"SCORE scores (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",Multiselect=!old,RestoreDirectory=true})if(dialog.ShowDialog(this)==DialogResult.OK){if(old)files.Items.Clear();foreach(string path in dialog.FileNames)files.Items.Add(path);InvalidatePlan();}}
  void Remove(){foreach(int n in files.SelectedIndices.Cast<int>().OrderByDescending(x=>x).ToArray())files.Items.RemoveAt(n);InvalidatePlan();}
  void MoveFile(int direction){int index=files.SelectedIndex;ReorderFile(index,index+direction);}
  void Preview(){
   InvalidatePlan();UseWaitCursor=true;
   try{
    if(operation.SelectedIndex==5){
     if(files.Items.Count!=1)throw new ArgumentException("Choose one original BARS.TMP file.");
     prepared=NativePageScoreLayout.PrepareOld((string)files.Items[0]);
     ShowPlan();
    }else if(operation.SelectedIndex==4){
     prepared=NativePageScoreLayout.Prepare(files.Items.Cast<string>(),(int)staves.Value,(float)size.Value,(float)height.Value,(int)firstPage.Value,centerNumbers.Checked);
     ShowPlan();
    }else if(operation.SelectedIndex>=2){
     var fields=turnControls.Text.Split(new[]{' ', ',', '\t'},StringSplitOptions.RemoveEmptyEntries);
     float quarters;int requested;
     if(files.Items.Count!=1||fields.Length!=2||!Single.TryParse(fields[0],System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out quarters)||!Int32.TryParse(fields[1],out requested))
      throw new ArgumentException("Enter a turn gap in quarter notes and a requested page count.");
     prepared=operation.SelectedIndex==3?NativePageTurnPlan.PrepareOld((string)files.Items[0],quarters,requested):NativePageTurnPlan.Prepare((string)files.Items[0],quarters,requested,(float)size.Value,(float)height.Value,(int)firstPage.Value,centerNumbers.Checked);
     ShowPlan();
    }else if(operation.SelectedIndex==1){
     var marked=barRange.Text.Split(new[]{' ', ',', '\t'},StringSplitOptions.RemoveEmptyEntries);
     bool byNumber=marked.Length==3&&marked[0].Equals("N",StringComparison.OrdinalIgnoreCase);
     int first=byNumber||marked.Length==3&&marked[0].Equals("M",StringComparison.OrdinalIgnoreCase)?1:0;
     int start,end;
     if(files.Items.Count!=1||marked.Length!=first+2||!Int32.TryParse(marked[first],out start)||!Int32.TryParse(marked[first+1],out end))
      throw new ArgumentException("Choose one score and enter two marked boundaries or N followed by the first and last bar numbers.");
     var source=(string)files.Items[0];
     prepared=byNumber
      ?NativePageMarkedLayout.PrepareNumbered(source,start,end,(float)size.Value,(float)height.Value,(int)firstPage.Value,centerNumbers.Checked,oldMarkedPrecision.Checked)
      :NativePageMarkedLayout.Prepare(source,start,end,(float)size.Value,(float)height.Value,(int)firstPage.Value,centerNumbers.Checked,oldMarkedPrecision.Checked);
     ShowPlan();
    }else{
    var proposal=fixedProposal.Text.Split(new[]{' ', ',', '\t'},StringSplitOptions.RemoveEmptyEntries);
    int pageCount=0,lines=0;
    if(proposal.Length!=0&&(proposal.Length!=2||!Int32.TryParse(proposal[0],out pageCount)||!Int32.TryParse(proposal[1],out lines)||pageCount<1||lines<1))throw new ArgumentException("Enter the page count and lines per page as two positive integers.");
    var range=barRange.Text.Split(new[]{' ', ',', '\t'},StringSplitOptions.RemoveEmptyEntries);
    int from=0,to=0;
    if(range.Length!=0&&(range.Length!=2||!Int32.TryParse(range[0],out from)||!Int32.TryParse(range[1],out to)||from<1||to<from))throw new ArgumentException("Enter the first and last measure numbers.");
    int transpose=transposeWholeTone.SelectedIndex;
    short interval=transpose<transposeIntervals.Length?transposeIntervals[transpose]:
     InputTranspositionInterval.Menu(new short[]{1,2,3,4,7,8,10,11,12,13,14}[transpose-transposeIntervals.Length]);
    bool normalize=transpose==10||normalizeStems.Enabled&&normalizeStems.Checked;
    if(normalize&&resourceRoot==null)throw new InvalidOperationException("PAGE stem normalization requires SCORE library resources.");
    short? clef=null;
    switch(transpose){case 3:clef=1;break;case 11:clef=0;break;case 12:clef=2;break;case 13:clef=3;break;}
    if(transposeClef.Enabled&&transposeClef.SelectedIndex>0)clef=(short)(transposeClef.SelectedIndex-1);
    prepared=NativePagePartsWorkflow.PrepareWithMissingInstrument(files.Items.Cast<string>(),(int)instrument.Value,(int)staves.Value,omit.Checked,(float)size.Value,(float)height.Value,(int)firstIndent.Value,(int)otherIndent.Value,(int)firstPage.Value,centerNumbers.Checked,restLabels.Checked,restStyle.SelectedIndex==1,(int)startMeasure.Value,everyMeasure.Enabled&&everyMeasure.Checked,manualLines.Text,linesPerPage.Text,pageCount,lines,from,to,interval,normalize?resourceRoot:null,transposeAddKey.Enabled&&transposeAddKey.Checked,clef,ChooseMissingInstrumentOpening,scor4);
    ShowPlan();
    }
   }
   catch(OperationCanceledException){return;}
   finally{UseWaitCursor=false;}
   if(prepared.Warnings.Count>0)report.AppendText(Environment.NewLine+Environment.NewLine+String.Join(Environment.NewLine+Environment.NewLine,prepared.Warnings));
  }
  NativePageParts.MissingInstrumentOpening ChooseMissingInstrumentOpening(int id){
   UseWaitCursor=false;
   using(var dialog=new Form{Text="PAGE — Missing instrument",Font=Font,StartPosition=FormStartPosition.CenterParent,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,FormBorderStyle=FormBorderStyle.FixedDialog,MinimizeBox=false,MaximizeBox=false,ShowInTaskbar=false})
   using(var fields=new TableLayoutPanel{ColumnCount=2,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,Padding=new Padding(12),Dock=DockStyle.Fill}){
    var explanation=new Label{Text="Instrument "+id+" is absent from a system. Choose the opening symbols for its replacement rests.",MaximumSize=new Size(340,0),AutoSize=true};
    fields.Controls.Add(explanation,0,0);fields.SetColumnSpan(explanation,2);
    var clef=new TextBox{Text="0",Width=115};
    var key=new TextBox{Width=115};
    var meter=new TextBox{Width=115};
    fields.Controls.Add(new Label{Text="Clef number (0 = treble)",AutoSize=true,Anchor=AnchorStyles.Left},0,1);fields.Controls.Add(clef,1,1);
    fields.Controls.Add(new Label{Text="Key signature (blank for none)",AutoSize=true,Anchor=AnchorStyles.Left},0,2);fields.Controls.Add(key,1,2);
    fields.Controls.Add(new Label{Text="Meter numbers (blank for none)",AutoSize=true,Anchor=AnchorStyles.Left},0,3);fields.Controls.Add(meter,1,3);
    var buttons=new FlowLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,Anchor=AnchorStyles.Right,WrapContents=false};
    var accept=new Button{Text="Continue",AutoSize=true};
    var cancel=new Button{Text="Cancel",AutoSize=true,DialogResult=DialogResult.Cancel};
    buttons.Controls.Add(accept);buttons.Controls.Add(cancel);fields.Controls.Add(buttons,0,4);fields.SetColumnSpan(buttons,2);
    dialog.Controls.Add(fields);dialog.AcceptButton=accept;dialog.CancelButton=cancel;
    NativePageParts.MissingInstrumentOpening opening=null;
    accept.Click+=(sender,args)=>{
     var words=meter.Text.Split(new[]{' ','\t','/'},StringSplitOptions.RemoveEmptyEntries);
     float clefValue;
     short keyValue=0,top=0,bottom=0;
     if(!Single.TryParse(clef.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out clefValue)||!(clefValue>=short.MinValue&&clefValue<=short.MaxValue)||
      !String.IsNullOrWhiteSpace(key.Text)&&!Int16.TryParse(key.Text,out keyValue)||
      words.Length>2||words.Length>0&&!Int16.TryParse(words[0],out top)||words.Length==2&&!Int16.TryParse(words[1],out bottom)){
      MessageBox.Show(dialog,"Enter a clef number, an optional key number and up to two meter numbers.","PAGE opening symbols",MessageBoxButtons.OK,MessageBoxIcon.Warning);
      return;
     }
     opening=new NativePageParts.MissingInstrumentOpening(clefValue,String.IsNullOrWhiteSpace(key.Text)?(short?)null:keyValue,words.Length==0?(short?)null:top,bottom);
     dialog.DialogResult=DialogResult.OK;
    };
    return dialog.ShowDialog(this)==DialogResult.OK?opening:null;
   }
  }
  void ShowPlan(){report.Text=String.Join(Environment.NewLine,prepared.Plan.Select((p,n)=>"Page "+p.Number+"   Bars per line: "+String.Join(", ",p.Bars)+"   "+prepared.Files[n].Length+" bytes"));save.Enabled=true;}
  void Save(){
   if(prepared==null)return;bool original=NativeScoreBehaviour.Enabled||names.SelectedIndex==0;
   using(var dialog=new SaveFileDialog{Title="First PAGE output",Filter="SCORE pages (*.pag)|*.pag",DefaultExt="pag",AddExtension=true,FileName="PART01.PAG",RestoreDirectory=true,OverwritePrompt=false}){
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    var paths=NativePagePartsWorkflow.OutputNames(prepared,dialog.FileName,original);int existing=paths.Count(File.Exists);
    string listing=String.Join(Environment.NewLine,paths.Take(12));if(paths.Length>12)listing+=Environment.NewLine+"... "+(paths.Length-12)+" more pages";
    if(MessageBox.Show(this,listing+Environment.NewLine+Environment.NewLine+existing+" existing outputs will be replaced. Save these pages?","PAGE outputs",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)!=DialogResult.OK)return;
    SavePrepared(dialog.FileName,original,existing!=0);report.Text=String.Join(Environment.NewLine,paths.Select(p=>"Saved "+p));
   }
  }
  void SavePrepared(string path,bool original,bool replace){
   var next=OutputBank.Clone();
   var contents=prepared.Files.Select(next.RetainPage).ToArray();
   NativePagePartsWorkflow.SavePages(prepared,path,original,replace,contents);
   OutputBank=next;
  }
 }

 public sealed partial class Editor {
  NativePageFileBank pageOutputBank=new NativePageFileBank(new byte[14200*4]);
  void ShowPageParts(){using(var dialog=new NativePagePartsDialog(score==null?null:score.SourcePath,scor4Interface)){dialog.OutputBank=pageOutputBank;dialog.EnableStemNormalization(source);dialog.ShowDialog(this);pageOutputBank=dialog.OutputBank;}}
 }
}
