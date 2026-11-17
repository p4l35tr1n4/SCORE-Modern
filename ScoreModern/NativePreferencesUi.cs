using System;using System.IO;using System.Linq;using System.Drawing;using System.Windows.Forms;
using System.Collections.Generic;

namespace ScoreModern {

 public sealed partial class Editor {

  string PreferenceUse(string name){

   switch(name){case "CODE9":return "External CODE9.DAT text terms; enter Code9 P5=3000–3069 or use the workspace insertion selector.";case "GUITAR":return "External GUITAR.DAT chord names/fingerings; Code12 P5=-4/-5/-6 or workspace insertion selector.";case "LIBRARY":return "DOS path mapping; explicit LIB choice takes priority. Unavailable consumers are reported separately.";case "MOUSE":return "0 pans with the middle button. 1 selects with the middle button and pans with all three. Default "+(scor4Interface?"1 in the supplied SCOR4.":"0 in SCORE 3.11.");case "SLUR":return "Slur thickness; existing per-item controls still apply.";case "REST":return "DAT numbered-rest font; values outside verified font routes are reported.";case "RESOLUTION":return "Print/WYSIWYG resolution in dots per inch; positive signed16 integer, default 300. Active print parameters can override it; DOS geometry is independent.";case "BMTH":return "Printed beam-width selector (signed16 integer), default -1. Zero retains prior thickness when reading preferences. DOS raster geometry is independent.";case "SETSTROKE":return "Print/WYSIWYG stroke adjustment: 0 off (default), 1 on. Active print parameters can override it; DOS geometry is independent.";case "PROCEDURES":return "Original glyph-definition cache budget in words (signed16 integer); 0 uses 2000, negative disables the budget. Affects printed coordinate rounding.";case "MEASUREMENT":return "New-document measurement: 0 inches, 1 centimetres. Opened-file trailer is retained; METRIC/INCHES command changes are saved in MUS.";case "COLOR":return "Screen notation/background/normal/edit DOS palette indices (0–15); edit optional (default 9). Priority: "+(NativeScoreBehaviour.Enabled?"PREF.SCR in Strict SCORE mode":"Windows overrides where set")+". "+NativeWindowsColors.Status(source)+" Print/EPS remain monochrome.";case "BEAM":return "F2 beam-entry default; recovered transaction receives this value divided by 100 (default .50). Existing item parameters are retained.";case "TIES":return "F2 tie/slur-entry control, signed16 integer (default 1); fractional values are not accepted by this recovered route.";case "DYNAMICS":return "F2 dynamics-entry control (default 0); passed to the recovered input transaction. Existing item parameters are retained.";case "MIDK":return "First MIDI pitch control key when a Windows MIDI input device is selected.";case "AFTC":return "J minimum space after a clef to the next note/rest, in absolute SCORE horizontal units, independent of staff size. Does not affect LI.";case "AFTB":return "J minimum space after a barline to the next note/rest, in absolute SCORE horizontal units, independent of staff size. Does not affect LI.";case "AFTK":return "J minimum space after a key signature to the next note/rest, in absolute SCORE horizontal units, independent of staff size. Does not affect LI.";case "AFTM":return "J minimum space after a time signature to the next note/rest, in absolute SCORE horizontal units, independent of staff size. Does not affect LI.";case "DASH":case "XDASH":return "Used by recovered lyric dash generation; unsupported text controls are reported.";case "SCREEN":case "MIDI":case "IRQ":case "KEYDISK":case "OUTPUT":case "DESC":return "Legacy DOS hardware/print-device setting; native display and Windows printer controls apply.";default:return "Not applied: unsupported preference.";}

  }

  PreferencesActionsMenu windowsPreferencesMenu;

  ScoreDialog preparedPreferences;

  ScorePreferences preparedPreferencesData;

  string preparedPreferencesRoot;

  bool preparingPreferences,showingPreferences;

  Action resetPreferencesSession;

  Action refreshPreferencesSession;

  Action warmPreferencesSession;
  System.Windows.Forms.Timer preferencesRefreshTimer;
  bool preferencesPollRunning;string preferencesWatchStamp;
  Action refreshPreparedScoreData;



  void QueuePreferencesPreload(){

   if(Program.CheckMode||IsDisposed||!IsHandleCreated||preparedPreferences!=null||preparingPreferences||showingPreferences)return;

   preparingPreferences=true;

   BeginInvoke(new Action(()=>{

    preparingPreferences=false;

    if(IsDisposed||preparedPreferences!=null||showingPreferences)return;

    try{

     var data=NativePreferences.Read(source);

     preparedPreferences=CreatePreferencesDialog(data);

     preparedPreferencesData=data;

     preparedPreferencesRoot=source;

     var prepared=preparedPreferences;

     TryWarmPreferences();
     prepared.PrepareForDisplay();
     StartPreferencesRefresh();

    }catch(IOException){}

     catch(UnauthorizedAccessException){}

     catch(NotSupportedException){}

     catch(InvalidOperationException){}

     catch(ArgumentException){}

   }));

  }

  void StartPreferencesRefresh(){
   if(preferencesRefreshTimer!=null)return;
   preferencesRefreshTimer=new System.Windows.Forms.Timer{Interval=1500};
   preferencesRefreshTimer.Tick+=(s,e)=>PollPreferencesFiles();preferencesRefreshTimer.Start();
   Disposed+=(s,e)=>{preferencesRefreshTimer.Stop();preferencesRefreshTimer.Dispose();};
  }
  void QueuePreferencesRefresh(){if(IsDisposed||!IsHandleCreated)return;BeginInvoke(new Action(()=>{if(IsDisposed)return;try{if(!showingPreferences&&refreshPreparedScoreData!=null){refreshPreparedScoreData();if(resetPreferencesSession!=null)resetPreferencesSession();}if(refreshPreferencesSession!=null)refreshPreferencesSession();}catch(IOException){}catch(UnauthorizedAccessException){}catch(NotSupportedException){}catch(InvalidOperationException){}}));}
  void PollPreferencesFiles(){
   if(IsDisposed||preferencesPollRunning)return;preferencesPollRunning=true;
   string root=source;string[] paths={NativeSettings.FilePath,NativePreferences.OriginalSourcePath(root)};
   System.Threading.Tasks.Task.Run(()=>{try{return String.Join("|",paths.Select(p=>{var f=new FileInfo(p);return f.Exists?f.Length+":"+f.LastWriteTimeUtc.Ticks:"absent";}));}catch(IOException){return "unavailable";}catch(UnauthorizedAccessException){return "unavailable";}}).ContinueWith(task=>{
    if(IsDisposed||!IsHandleCreated)return;try{BeginInvoke(new Action(()=>{preferencesPollRunning=false;if(root!=source){DisposePreparedPreferences();QueuePreferencesPreload();return;}if(preferencesWatchStamp==task.Result)return;preferencesWatchStamp=task.Result;QueuePreferencesRefresh();}));}catch(InvalidOperationException){}
   });
  }

  void TryWarmPreferences(){try{if(warmPreferencesSession!=null)warmPreferencesSession();}

   catch(IOException){}catch(UnauthorizedAccessException){}catch(NotSupportedException){}catch(InvalidOperationException){}catch(ArgumentException){}

  }

  void DisposePreparedPreferences(){

   if(preparedPreferences!=null)preparedPreferences.Dispose();

   preparedPreferences=null;

   preparedPreferencesData=null;

   preparedPreferencesRoot=null;

   resetPreferencesSession=null;

   refreshPreferencesSession=null;

   warmPreferencesSession=null;
   refreshPreparedScoreData=null;

  }

  void ShowPreferences(){

   if(!String.Equals(preparedPreferencesRoot,source,StringComparison.OrdinalIgnoreCase))DisposePreparedPreferences();
   var data=preparedPreferencesData??NativePreferences.Read(source);



   // Keep the control tree. Reset values, rather than rebuilding hundreds of

   // auto-sized controls after every close (including cancelled edits).

   if(preparedPreferences==null){preparedPreferences=CreatePreferencesDialog(data);preparedPreferencesData=data;preparedPreferencesRoot=source;}

   // Values and handles are prepared before this hot path. No full menu refresh or layout here.
   if(warmPreferencesSession!=null&&refreshWindowsPreferencesLayout==null)TryWarmPreferences();

   showingPreferences=true;

   try{

    preparedPreferences.ShowDialog(this);

   }finally{

    showingPreferences=false;
    if(resetPreferencesSession!=null)resetPreferencesSession();
    QueuePreferencesPreload();

   }

  }

  ScoreDialog CreatePreferencesDialog(ScorePreferences data){

   var defaults=NativePreferences.Defaults(source);

   var dialog=new ScoreDialog("preferences"){Text="Preferences",Width=1000,Height=650,MinimumSize=new Size(590,400),StartPosition=FormStartPosition.CenterParent};

   dialog.SuspendLayout();

   try{

    var tabs=new TabControl{Dock=DockStyle.Fill};

    var scoreTab=new TabPage(scor4Interface?"SCOR4":"SCORE 3.11");

    var windowsTab=new TabPage("Windows");

    tabs.TabPages.Add(scoreTab);

    tabs.TabPages.Add(windowsTab);

    var utilitiesTab=new TabPage("TMB utilities");
    Action resetUtilities=PopulateTmbPreferences(utilitiesTab,dialog);
    tabs.TabPages.Add(utilitiesTab);

    var table=new DataGridView{Dock=DockStyle.Fill,AllowUserToAddRows=false,AllowUserToDeleteRows=false,

     AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.None,RowHeadersVisible=false};

    table.DefaultCellStyle.WrapMode=DataGridViewTriState.False;
    table.RowTemplate.Height=27;
    table.CellToolTipTextNeeded+=(s,e)=>{if(e.RowIndex>=0&&e.ColumnIndex>=0)e.ToolTipText=Convert.ToString(table.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);};

    table.Columns.Add("key","Preference");

    table.Columns.Add("value","Value");

    table.Columns.Add("source","Source");

    table.Columns.Add("effect","Current use");

    for(int i=0;i<table.Columns.Count;i++)table.Columns[i].ReadOnly=i!=1;

    table.Columns[0].FillWeight=22;

    table.Columns[1].FillWeight=50;

    table.Columns[2].FillWeight=24;

    table.Columns[3].FillWeight=80;

    var editableDefaults=new[]{"LIBRARY","MOUSE","DASH","XDASH"};
    foreach(string name in data.Entries.Keys.Concat(editableDefaults).Distinct().OrderBy(name=>name,StringComparer.OrdinalIgnoreCase)){

     string value=data.Text(name),original=defaults.Text(name);

     table.Rows.Add(name,value,value!=original?"ScoreModern.ini override":original!=null?Path.GetFileName(defaults.Path):"Default",PreferenceUse(name));

    }

    table.CellValueChanged+=(sender,args)=>{

     if(args.RowIndex<0||args.ColumnIndex!=1)return;

     var row=table.Rows[args.RowIndex];

     string original=defaults.Text((string)row.Cells[0].Value);

     string value=(string)row.Cells[1].Value;

     row.Cells[2].Value=value!=original?"ScoreModern.ini override":original!=null?Path.GetFileName(defaults.Path):"Default";

    };

    string library=NativeLibraryLocation.DirectoryFor(source,true);


    var selectedLibrary=NativeLibraryLocation.Get(source);

    string librarySource=selectedLibrary==null?"preference path":selectedLibrary.All?"folder override, all resources":"folder override, drawings only";

    var paths=new Label{Dock=DockStyle.Top,AutoSize=true,Text="SCORE defaults: "+data.Path+

     "\nActive drawing LIB: "+library+" ("+librarySource+")"+

     (NativePreferences.FollowPaths?"":"\nPREF.SCR resource paths are disabled.")};

    bool displayedScor4=scor4Interface;
    var profileDrafts=new Dictionary<bool,Dictionary<string,string>>();

    var buttons=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,WrapContents=false,FlowDirection=FlowDirection.RightToLeft};

    var close=new Button{Text="Close",AutoSize=true,DialogResult=DialogResult.Cancel};

    var apply=new Button{Text="Apply",AutoSize=true};

    var reset=new Button{Text="Reset selected",AutoSize=true,Margin=new Padding(2),Padding=Padding.Empty};

    var browse=new Button{Text="Browse...",AutoSize=true,Margin=new Padding(2),Padding=Padding.Empty};
    browse.Visible=false;table.CurrentCellChanged+=(s,e)=>{string n=table.CurrentRow==null?"":Convert.ToString(table.CurrentRow.Cells[0].Value);browse.Visible=n=="LIBRARY";};
    bool usePreferenceLibrary=false;
    var readPreset=new Button{Text="Read PREF file...",AutoSize=true,Margin=new Padding(2),Padding=Padding.Empty};
    var savePreset=new Button{Text="Save PREF file...",AutoSize=true,Margin=new Padding(2),Padding=Padding.Empty};
    readPreset.Click+=(sender,args)=>Safe(()=>{
     if(!table.EndEdit())return;
     using(var picker=new OpenFileDialog{Title="Read SCORE preferences",InitialDirectory=Directory.GetCurrentDirectory(),Filter="SCORE preferences|PREF.*;*.SCR;*.MSCR|All files|*.*"}){
      if(picker.ShowDialog(dialog)!=DialogResult.OK)return;
      var imported=ReadPreferencePreset(table,picker.FileName);
      usePreferenceLibrary|=imported.Entries.ContainsKey("LIBRARY");
      if(imported.Issues.Count!=0)MessageBox.Show(dialog,String.Join("\r\n",imported.Issues.ToArray()),"Preference file",MessageBoxButtons.OK,MessageBoxIcon.Information);
     }
    });
    savePreset.Click+=(sender,args)=>Safe(()=>{
     if(!table.EndEdit())return;
     using(var picker=new SaveFileDialog{Title="Save SCORE preferences",InitialDirectory=Directory.GetCurrentDirectory(),FileName="PREF.SCR",Filter="SCORE preferences|*.SCR|All files|*.*",OverwritePrompt=true}){
      if(picker.ShowDialog(dialog)!=DialogResult.OK)return;
      SavePreferencePreset(table,picker.FileName);
     }
    });




    refreshPreparedScoreData=()=>{
     bool profileChanged=displayedScor4!=scor4Interface;
     var nextData=NativePreferences.Read(source);
     bool reloadTable=profileChanged||!Object.ReferenceEquals(data,nextData);
     if(reloadTable){
      table.EndEdit();
      profileDrafts[displayedScor4]=table.Rows.Cast<DataGridViewRow>()
       .Where(row=>!Equals(row.Cells[1].Value,data.Text((string)row.Cells[0].Value)))
       .ToDictionary(row=>(string)row.Cells[0].Value,row=>(string)row.Cells[1].Value,StringComparer.OrdinalIgnoreCase);
      displayedScor4=scor4Interface;
      if(profileChanged)usePreferenceLibrary=false;
     }
     data=nextData;defaults=NativePreferences.Defaults(source);preparedPreferencesData=data;
     if(reloadTable){
      table.Rows.Clear();
      Dictionary<string,string> draft;
      if(!profileDrafts.TryGetValue(displayedScor4,out draft))draft=new Dictionary<string,string>();
      foreach(string name in data.Entries.Keys.Concat(draft.Keys).Concat(editableDefaults).Distinct(StringComparer.OrdinalIgnoreCase)
       .OrderBy(n=>n,StringComparer.OrdinalIgnoreCase)){
       string value;
       if(!draft.TryGetValue(name,out value))value=data.Text(name);
       string original=defaults.Text(name);
       table.Rows.Add(name,value,value!=original?"ScoreModern.ini override":original!=null?Path.GetFileName(defaults.Path):"Default",PreferenceUse(name));
      }
     }else foreach(string name in data.Entries.Keys.OrderBy(n=>n,StringComparer.OrdinalIgnoreCase))
      if(!table.Rows.Cast<DataGridViewRow>().Any(r=>Equals(r.Cells[0].Value,name)))table.Rows.Add(name,data.Text(name),"",PreferenceUse(name));
     scoreTab.Text=scor4Interface?"SCOR4":"SCORE 3.11";
     var selection=NativeLibraryLocation.Get(source);
     paths.Text="SCORE defaults: "+data.Path+"\nActive drawing LIB: "+NativeLibraryLocation.DirectoryFor(source,true)+
      (selection==null?" (preference path)":selection.All?" (all resources)":" (drawings only)")+
      (NativePreferences.FollowPaths?"":"\nPREF resource paths are disabled.");
    };
    resetPreferencesSession=()=>{

     table.CancelEdit();table.SuspendLayout();

     try{foreach(DataGridViewRow row in table.Rows){string name=(string)row.Cells[0].Value;row.Cells[1].Value=data.Text(name);}}

     finally{table.ResumeLayout(false);}

     usePreferenceLibrary=false;;

     resetUtilities();
     profileDrafts.Clear();

     dialog.DialogResult=DialogResult.None;

    };

    reset.Click+=(sender,args)=>{

     table.EndEdit();

     if(table.CurrentRow==null)return;

     string name=(string)table.CurrentRow.Cells[0].Value;

     table.CurrentRow.Cells[1].Value=defaults.Text(name);

     if(name=="LIBRARY")usePreferenceLibrary=true;


    };

    browse.Click+=(sender,args)=>Safe(()=>{

     table.EndEdit();

     if(table.CurrentRow==null)return;

     string name=(string)table.CurrentRow.Cells[0].Value;

     if(name!="LIBRARY")return;

     using(var picker=new FolderBrowserDialog{Description="Choose the "+name+" folder",SelectedPath=library}){

      if(picker.ShowDialog(dialog)!=DialogResult.OK)return;

      table.CurrentRow.Cells[1].Value=picker.SelectedPath;

      usePreferenceLibrary|=name=="LIBRARY";

     }

    });

    Action refreshWindows=null;

    Action applyPreferences=()=>Safe(()=>{

     if(!table.EndEdit())return;

     var overrides=new System.Collections.Generic.List<string>();

     bool pathChanged=false,hasChanges=false;

     foreach(DataGridViewRow row in table.Rows){

      string name=(string)row.Cells[0].Value,value=(string)row.Cells[1].Value;

      bool changed=value!=data.Text(name);

      hasChanges|=changed;

      pathChanged|=changed&&name=="LIBRARY";

      usePreferenceLibrary|=changed&&name=="LIBRARY";

      if(value!=null&&value!=defaults.Text(name))overrides.Add(NativePreferences.Directive(name,value));

     }

     if(hasChanges||usePreferenceLibrary||pathChanged){

      SaveAndApplyPreferences(String.Join("\r\n",overrides),usePreferenceLibrary,pathChanged);

      data=NativePreferences.Read(source);preparedPreferencesData=data;

      defaults=NativePreferences.Defaults(source);

      usePreferenceLibrary=false;


      foreach(DataGridViewRow row in table.Rows){

       string name=(string)row.Cells[0].Value;

       row.Cells[1].Value=data.Text(name);

      }

      if(refreshWindows!=null)refreshWindows();

     }

    });

    apply.Click+=(sender,args)=>applyPreferences();

    buttons.Controls.Add(close);

    buttons.Controls.Add(apply);

    buttons.Controls.Add(reset);

    buttons.Controls.Add(browse);
    buttons.Controls.Add(savePreset);
    buttons.Controls.Add(readPreset);
    Action fitScoreButtons=()=>{
     var size=new System.Collections.Generic.Dictionary<Button,Size>();
     foreach(Button button in buttons.Controls){
      var caption=TextRenderer.MeasureText(button.Text,button.Font);
      size.Add(button,new Size(caption.Width+12,button.Font.Height+9));
     }
     foreach(var entry in size)if(entry.Key.Size!=entry.Value||entry.Key.AutoSize){entry.Key.AutoSize=false;entry.Key.Size=entry.Value;}
    };
    foreach(Button button in buttons.Controls)button.Margin=new Padding(2);
    buttons.HandleCreated+=(s,e)=>fitScoreButtons();
    buttons.FontChanged+=(s,e)=>fitScoreButtons();
    buttons.Layout+=(s,e)=>fitScoreButtons();

    scoreTab.Controls.Add(table);

    scoreTab.Controls.Add(paths);

    scoreTab.Controls.Add(buttons);



    Action warmWindows=()=>{

      if(refreshWindows==null)refreshWindows=PopulateWindowsPreferences(windowsTab,applyPreferences);

      else refreshWindows();

    };

    warmPreferencesSession=warmWindows;
    warmWindows();

    tabs.SelectedIndexChanged+=(sender,args)=>{if(tabs.SelectedTab==windowsTab)warmWindows();};

    refreshPreferencesSession=()=>{if(refreshWindows!=null)refreshWindows();};

    var portableButtons=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,Padding=new Padding(5)};
    var exportPortable=new Button{Text="Save portable INI…",AutoSize=true};
    var importPortable=new Button{Text="Load portable INI…",AutoSize=true};
    exportPortable.Click+=(sender,args)=>Safe(()=>{
     using(var picker=new SaveFileDialog{Title="Save settings without file locations",FileName="ScoreModern.ini",Filter="INI settings (*.ini)|*.ini",RestoreDirectory=true}){
      if(picker.ShowDialog(dialog)==DialogResult.OK)NativeSettings.ExportPortable(picker.FileName,source);
     }
    });
    importPortable.Click+=(sender,args)=>Safe(()=>{
     using(var picker=new OpenFileDialog{Title="Load portable settings",Filter="INI settings (*.ini)|*.ini",RestoreDirectory=true}){
      if(picker.ShowDialog(dialog)!=DialogResult.OK)return;
      if(MessageBox.Show(dialog,"Replace the portable preferences with those in this file? Local folders, devices and file history are kept. Restart SCORE Modern to use the imported preferences.","Load portable settings",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
      NativeSettings.ImportPortable(picker.FileName,source);
      MessageBox.Show(dialog,"Settings loaded. Restart SCORE Modern to use them.","Portable settings");
      dialog.DialogResult=DialogResult.Cancel;
     }
    });
    portableButtons.Controls.Add(exportPortable);
    portableButtons.Controls.Add(importPortable);
    dialog.Controls.Add(tabs);
    dialog.Controls.Add(portableButtons);

    dialog.CancelButton=close;

    dialog.Shown+=(sender,args)=>table.AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.None;

    dialog.ResumeLayout(false);

    return dialog;

   }catch{dialog.Dispose();throw;}

  }

  ScorePreferences ReadPreferencePreset(DataGridView table,string path){
   var imported=NativePreferences.Parse(File.ReadAllLines(path),path);if(imported.Entries.Count==0)throw new InvalidDataException("No SCORE preference directives were found in "+path);
   foreach(var entry in imported.Entries.Values){var row=table.Rows.Cast<DataGridViewRow>().FirstOrDefault(candidate=>String.Equals((string)candidate.Cells[0].Value,entry.Name,StringComparison.OrdinalIgnoreCase));if(row==null)table.Rows.Add(entry.Name,entry.Value,"ScoreModern.ini override",PreferenceUse(entry.Name));else row.Cells[1].Value=entry.Value;}
   table.Sort(table.Columns[0],System.ComponentModel.ListSortDirection.Ascending);return imported;
  }
  void SavePreferencePreset(DataGridView table,string path){
   string protectedSource=Path.GetFullPath(NativePreferences.OriginalSourcePath(source));if(String.Equals(Path.GetFullPath(path),protectedSource,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("The original PREF.SCR is preserved. Choose a separate preference filename.");
   var lines=table.Rows.Cast<DataGridViewRow>().Where(row=>row.Cells[1].Value!=null).OrderBy(row=>(string)row.Cells[0].Value,StringComparer.OrdinalIgnoreCase).Select(row=>NativePreferences.Directive((string)row.Cells[0].Value,(string)row.Cells[1].Value));File.WriteAllLines(path,lines,new System.Text.UTF8Encoding(false));
  }

  void SaveAndApplyPreferences(string overrides,bool usePreferenceLibrary,bool usePaths){

   var selection=NativeLibraryLocation.Get(source);

   bool oldFollow=NativePreferences.FollowPaths;

   string key=NativePreferences.SettingKey(source),previous=NativeSettings.Read(key);

   NativeSettings.WriteAllText(key,overrides);

   try{

    if(usePaths||usePreferenceLibrary)NativePreferences.SetFollowPaths(true,false);

    NativePreferences.Clear();

    if(usePaths||usePreferenceLibrary){

     var data=NativePreferences.Read(source);

     foreach(string name in new[]{"LIBRARY"}){

      string raw=data.Text(name);

      if(String.IsNullOrWhiteSpace(raw)||raw.Length==1&&Char.IsLetter(raw[0]))continue;

      string requested=NativePreferences.MapPath(source,raw);

      if(!Directory.Exists(requested))throw new DirectoryNotFoundException(name+" folder not found: "+requested);

     }

    }

    if(usePreferenceLibrary)ApplyLibraryFolder(null,false,true);

    else ReloadPreferences();

    if(usePaths||usePreferenceLibrary)NativePreferences.SetFollowPaths(true,true);

    ApplyPreferenceColors();

    Message("Preferences applied. LIB: "+NativeLibraryLocation.DirectoryFor(source,true));

   }catch{

    NativeSettings.WriteAllText(key,previous);

    NativePreferences.SetFollowPaths(oldFollow,true);

    ApplyLibraryFolder(selection==null?null:selection.Folder,selection!=null&&selection.All,true);

    throw;

   }

  }

    void ReloadPreferences(){var previous=NativePreferences.Read(source);NativePreferences.Clear();try{NativePreferences.Validate(source);var choice=NativeLibraryLocation.Get(source);ApplyLibraryFolder(choice==null?null:choice.Folder,choice!=null&&choice.All,false);ApplyPreferenceColors();canvas.Invalidate();Message("Preferences reloaded from "+NativePreferences.Read(source).Path);}catch{NativePreferences.Restore(source,previous);ApplyPreferenceColors();throw;}}

  void AddPreferenceCompatibilityMenu(ContextMenuStrip menu){var reload=new ToolStripMenuItem("Reload SCORE defaults");reload.Click+=(s,e)=>Safe(ReloadPreferences);menu.Items.Add(reload);var paths=new ToolStripMenuItem("Preferences: follow original resource paths");paths.Click+=(s,e)=>Safe(()=>{bool previous=NativePreferences.FollowPaths;var previousData=NativePreferences.Read(source);try{NativePreferences.SetFollowPaths(!previous,false);ReloadPreferences();NativePreferences.SetFollowPaths(!previous,true);}catch{NativePreferences.SetFollowPaths(previous,false);NativePreferences.Restore(source,previousData);throw;}});menu.Items.Add(paths);menu.Opening+=(s,e)=>paths.Checked=NativePreferences.FollowPaths;}

 }

}

