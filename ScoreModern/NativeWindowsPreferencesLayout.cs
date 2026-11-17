using System;using System.Collections.Generic;using System.Drawing;using System.Linq;using System.Windows.Forms;
namespace ScoreModern {
 public sealed partial class Editor {
  sealed class PreferencePlacement {
   internal string Category,Section,Description;
   internal PreferencePlacement(string category,string section,string description=""){Category=category;Section=section;Description=description;}
  }
  static PreferencePlacement PlacePreference(string text,string path){
   if(text.StartsWith("Mouse menus:"))return new PreferencePlacement("SCORE compatibility","Mouse menu controls");
   if(text.StartsWith("Keyboard:"))return new PreferencePlacement("SCORE compatibility","Keyboard and window controls","Original SCORE keys are the default. These documented alternatives are disabled by Strict SCORE behaviour.");
   if(text=="Strict SCORE behaviour")return new PreferencePlacement("SCORE compatibility","Strict SCORE behaviour","The preset overrides alternative operation choices while enabled. Your individual choices are retained. Windows display quality and font rendering remain available.");
   if(text=="Show command lists in copyable dialogs")return new PreferencePlacement("Display & text","Command output");
   if(text=="Recent files shown..."||text.StartsWith("Use Windows file dialogs"))return new PreferencePlacement("Files & resources","File dialogs and recent files");
   if(text.StartsWith("F7 "))return new PreferencePlacement("Files & resources","F7 file and folder dialogs","F7 opens the Disk menu. File dialogs apply to Load, Save, Append and Read; folder dialogs apply to Directory, Drive and Set path. Strict SCORE behaviour uses SCORE prompts.");
   if(text.StartsWith("Printing:"))return new PreferencePlacement("SCORE compatibility","Printing side effects");
   if(text.StartsWith("F6 menu:"))return new PreferencePlacement("Fonts & output","F6 print and export menu","Choose the menu shown when F6 opens. Strict SCORE behaviour uses the SCORE-format menu.");
   if(text.StartsWith("Interface text:")||text.StartsWith("Use external VGA")||text=="Compact text"||text=="Text area font size...")return new PreferencePlacement("Display & text","Interface font and text size","These settings affect interface text. Smaller text leaves more room for graphics; notation font metrics are separate.");
   if(text.StartsWith("Startup: "))return new PreferencePlacement("Display & text","Title screen");
   if(text.StartsWith("Notation:")||text.StartsWith("DOS viewport")||path.Contains("DOS viewport zoom"))return new PreferencePlacement("Display & text","Notation display","Select high-resolution vectors, DAT shapes or the original DOS viewport.");
   if(text.StartsWith("Z/ZZ framing:"))return new PreferencePlacement("Display & text","Zoom framing");
   if(text.StartsWith("Mouse click-points:"))return new PreferencePlacement("Input & selection","Hover indicators");
   if(text.StartsWith("Mouse selection:"))return new PreferencePlacement("SCORE compatibility","Mouse selection rules");
   if(text.StartsWith("F2 field"))return new PreferencePlacement("Input & selection","Music input");
   if(text.StartsWith("MIDI input"))return new PreferencePlacement("Input & selection","MIDI input","Select the Windows MIDI receiver used during F2 entry. SCORE's MIDK preference remains on the SCORE tab.");
   if(text.StartsWith("Drawing filenames:"))return new PreferencePlacement("SCORE compatibility","Named drawing files","Keep SCORE3.11 filename interpretation, or accept the full .DRW names documented by Tom. Strict SCORE retains the original interpretation.");
   if(text.StartsWith("Type 1 fonts")||text=="Font resources..."||text.StartsWith("Choose duplicate Type 1")||text.StartsWith("FONTCONV")||text.StartsWith("Font initialisation:"))return new PreferencePlacement("Fonts & output","Type 1 fonts and metrics","Use external Type 1 outlines and SCORE PSC metrics. Windows font substitution requires your approval.");
   if(path.Contains("Text rendering"))return new PreferencePlacement("Fonts & output","Type 1 rendering");
   if(text.StartsWith("Allow approximate engraving"))return new PreferencePlacement("Fonts & output","Unavailable engraving","Approximation is optional and does not establish SCORE equivalence. Strict SCORE behaviour disables it.");
   if(path.Contains("Windows screen colours")||text.StartsWith("Non-page background")||text=="Use COLO for non-page background")return new PreferencePlacement("Workspace","Screen colours","Screen colours do not change monochrome print or EPS output.");
   if(path.Contains("Workspace guides"))return new PreferencePlacement("Workspace",text.IndexOf("ruler",StringComparison.OrdinalIgnoreCase)>=0?"Rulers and pointer readout":"Page and notation padding");
   if(text.StartsWith("Page frame:"))return new PreferencePlacement("Workspace","Page frame");
   if(text.StartsWith("Startup SCORE folder")||text=="Reopen last file on startup")return new PreferencePlacement("Files & resources","Startup");
   if(path.Contains("Library (LIB)"))return new PreferencePlacement("Files & resources","SCORE resources");
   if(text.StartsWith("Save and export folder"))return new PreferencePlacement("Files & resources","Save and export location");
   if(text.StartsWith("Map DOS PostScript paths"))return new PreferencePlacement("Files & resources","Included PostScript paths","Existing Windows files take priority. Otherwise DOS drive/root paths map into the selected SCORE folder; literal Windows paths remain optional.");
   if(text.StartsWith("Show fallback LIB")||text.StartsWith("Reload SCORE defaults")||text.StartsWith("Preferences: "))return new PreferencePlacement("Files & resources","Resource paths and defaults");
   if(text=="File associations...")return new PreferencePlacement("Files & resources","Windows file associations");
   if(text.StartsWith("Beams/slurs:")||text.StartsWith("Endings:")||text.StartsWith("Slurs:")||text.StartsWith("Slurs and hairpins:")||text.StartsWith("Justification:"))return new PreferencePlacement("SCORE compatibility","Notation rules");
   if(text.StartsWith("Editing:")||text.StartsWith("Grace-chord flips:"))return new PreferencePlacement("SCORE compatibility","Editing and recovery");
   if(text.StartsWith("TAB:")||text.StartsWith("EDI:")||text.StartsWith("DPY:")||text.StartsWith("Z/ZZ defaults:"))return new PreferencePlacement("SCORE compatibility","Command semantics");
   if(text.StartsWith("PostScript files:")||text.StartsWith("Cache-only text"))return new PreferencePlacement("SCORE compatibility","External-resource semantics");
   return new PreferencePlacement("SCORE compatibility","Other settings");
  }
  static string PreferenceChoice(string text,string path){
   if(text.StartsWith("Keyboard: Item PgUp/PgDn:"))return "Item PgUp / PgDn step";
   if(text.StartsWith("Keyboard: Item Tab:"))return "Item Tab step";
   if(text.StartsWith("Notation:"))return "Notation display";
   if(path.Contains("DOS viewport zoom"))return "DOS viewport zoom";
   if(text.StartsWith("Beams/slurs:"))return "Beam and slur geometry";
   if(text.StartsWith("Z/ZZ framing:"))return "Zoom framing";
   if(text.StartsWith("Z/ZZ defaults:"))return "Z / ZZ defaults";
   if(text=="Mouse selection: original DOS grid"||text=="Mouse selection: high-resolution distances")return "Selection coordinates";
   if(path.Contains("Text rendering"))return "Text renderer";
   if(text.StartsWith("Page frame:"))return "Page size";
   if(text.StartsWith("Ruler units:"))return "Ruler units";
   if(text.StartsWith("SCORE vertical ruler:"))return "SCORE vertical origin";
   if(text.StartsWith("Colour priority:"))return "Colour priority";
   if(text.StartsWith("F6 menu:"))return "F6 menu";
   if(text=="TAB: original quarter-tone table behaviour"||text=="TAB: reject quarter-tone conversion")return "TAB quarter-tone conversion";
   return null;
  }
  static bool PreferenceToggle(string text){
   return text.StartsWith("Printing:")||text.StartsWith("Mouse menus:")||text.StartsWith("Keyboard:")||text.StartsWith("Drawing filenames:")||text=="MIDI input: group held notes into chords"||text=="Show command lists in copyable dialogs"||text.StartsWith("Use Windows file dialogs")||text.StartsWith("DPY:")||text=="Strict SCORE behaviour"||text=="Use selected LIB for all resources"||text=="Reopen last file on startup"||text=="Compact text"||text.StartsWith("Show fallback LIB")||text.StartsWith("Startup: original")||text.StartsWith("F7 files:")||text.StartsWith("F7 folders:")||text.StartsWith("Font initialisation:")||text.StartsWith("Allow approximate engraving")||text.StartsWith("Mouse click-points:")||text=="Mouse selection: original unrestricted clicks"||text.StartsWith("Editing:")||text.StartsWith("Grace-chord flips:")||text.StartsWith("Endings:")||text.StartsWith("Slurs: reject")||text.StartsWith("Slurs and hairpins: reject")||text.StartsWith("Map DOS PostScript paths")||text.StartsWith("Justification:")||text.StartsWith("Cache-only text")||text.StartsWith("PostScript files:")||text.StartsWith("Preferences:")||text.StartsWith("TAB: reject adjacent")||text.StartsWith("EDI:")||text.StartsWith("Gap around ")||text=="Show margin rulers"||text=="Ruler mouse-position lines";
  }
  static string PreferenceActionLabel(string text){
   if(text=="Use COLO for non-page background")return "Reset to PREF.SCR";
   if(text.IndexOf("colour",StringComparison.OrdinalIgnoreCase)>=0)return text.StartsWith("Use ")?"Reset to PREF.SCR":"Choose colour...";
   if(text.StartsWith("Use SCORE"))return "Reset to default";
   if(text.StartsWith("Reload"))return "Reload";
   if(text.IndexOf("reset",StringComparison.OrdinalIgnoreCase)>=0)return "Reset";
   if(text=="Font resources...")return "Inspect...";
   if(text.StartsWith("Create"))return "Create / replace...";
   if(text.StartsWith("Choose"))return "Choose...";
   if(text.IndexOf("folder",StringComparison.OrdinalIgnoreCase)>=0)return "Browse...";
   if(text.StartsWith("DOS viewport:"))return "Reset view";
   if(text.Contains("size")||text.Contains("duration")||text.Contains("height")||text.Contains("width")||text.Contains("segment"))return "Change...";
   return "Configure...";
  }
  sealed class PreferenceSection:GroupBox {
   internal readonly TableLayoutPanel Rows=new TableLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,Dock=DockStyle.Top,ColumnCount=2,Padding=new Padding(4,0,4,2)};
   internal PreferenceSection(string title,string description){Text=title;AutoSize=true;AutoSizeMode=AutoSizeMode.GrowAndShrink;Padding=new Padding(4,8,4,2);Margin=new Padding(0,1,0,4);Width=680;Rows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,250));Rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));Controls.Add(Rows);}
   internal void AddFull(Control c){int r=Rows.RowCount++;Rows.RowStyles.Add(new RowStyle(SizeType.AutoSize));Rows.Controls.Add(c,0,r);Rows.SetColumnSpan(c,2);}
   internal void AddField(Control label,Control value){int r=Rows.RowCount++;Rows.RowStyles.Add(new RowStyle(SizeType.AutoSize));Rows.Controls.Add(label,0,r);Rows.Controls.Add(value,1,r);}
   internal Label AddField(string text,Control value){var label=new Label{Text=text,AutoEllipsis=true,Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft,Height=22,Margin=new Padding(2,1,8,1),UseMnemonic=false};AddField(label,value);return label;}
  }
  sealed class PreferenceChoiceItem {internal ToolStripMenuItem Item;internal PreferenceChoiceItem(ToolStripMenuItem item){Item=item;}public override string ToString(){string t=Item.Text;int colon=t.IndexOf(':');return colon>=0?t.Substring(colon+1).Trim():t;}}
  Action PopulateWindowsPreferences(TabPage page,Action apply){
   page.SuspendLayout();using(NativeSettings.SnapshotRead())windowsPreferencesMenu.RefreshActions();var tabs=new TabControl{Dock=DockStyle.Fill,Multiline=true,Padding=new Point(15,7)};
   var panels=new Dictionary<string,FlowLayoutPanel>();var sections=new Dictionary<string,PreferenceSection>();
   foreach(string name in new[]{"SCORE profiles","Files & resources","Display & text","Workspace","Input & selection","Fonts & output","SCORE compatibility"}){var tab=new TabPage(name);var panel=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(6)};tab.Controls.Add(panel);tabs.TabPages.Add(tab);panels.Add(name,panel);}
   var tooltip=new ToolTip{AutoPopDelay=30000,InitialDelay=400,ReshowDelay=100,ShowAlways=true};var errors=new ErrorProvider{BlinkStyle=ErrorBlinkStyle.NeverBlink};page.Disposed+=(s,e)=>{tooltip.Dispose();errors.Dispose();};
   var refreshers=new List<Action>();var choices=new Dictionary<string,ComboBox>();var leaves=new List<Tuple<ToolStripMenuItem,string>>();bool refreshing=false;TextBox vgaField=null;
   Action<ToolStripItemCollection,string> collect=null;collect=(items,parent)=>{foreach(ToolStripItem raw in items){var i=raw as ToolStripMenuItem;if(i==null)continue;string path=parent.Length==0?i.Text:parent+" / "+i.Text;if(i.HasDropDownItems)collect(i.DropDownItems,path);else leaves.Add(Tuple.Create(i,path));}};collect(windowsPreferencesMenu.Items,"");
   var toggles=leaves.Where(x=>PreferenceToggle(x.Item1.Text)).GroupBy(x=>x.Item1.Text).ToDictionary(g=>g.Key,g=>g.First().Item1);
   var combined=new HashSet<string>(leaves.Select(x=>InlinePreference(x.Item1.Text)).Where(v=>v!=null&&v.EnableSetting!=null&&v.EnableSetting!="Show margin rulers").Select(v=>v.EnableSetting));
   Action refresh=null;Action<ToolStripMenuItem> invoke=i=>Safe(()=>{if(i.Enabled)i.PerformClick();if(refresh!=null)refresh();});
   Func<ToolStripMenuItem,CheckBox> checkbox=i=>{var c=new CheckBox{Text=i.Text,AutoSize=false,AutoEllipsis=true,Height=22,Dock=DockStyle.Fill,Margin=new Padding(2,1,2,1),UseMnemonic=false,Tag=i};tooltip.SetToolTip(c,CompatibilityHelp(i));c.CheckedChanged+=(s,e)=>{if(!refreshing)invoke(i);};refreshers.Add(()=>{c.Checked=i.Checked;c.Enabled=i.Enabled;});return c;};
   Func<PreferencePlacement,PreferenceSection> getSection=p=>{string key=p.Category+"|"+p.Section;PreferenceSection sec;if(!sections.TryGetValue(key,out sec)){sec=new PreferenceSection(p.Section,p.Description);sections.Add(key,sec);panels[p.Category].Controls.Add(sec);tooltip.SetToolTip(sec,p.Description);}return sec;};
   foreach(var pair in leaves){var item=pair.Item1;string text=item.Text,path=pair.Item2;
    if(text=="Native vectors"&&path.Contains("Text rendering"))continue;
    if(path.Contains("Library (LIB)")&&text!="Choose serial DAT...")continue;
    if(text.StartsWith("Colour priority:")||text=="Mouse click-points: outline selection area"||combined.Contains(text)||text.StartsWith("Use PREF.SCR ")||text=="Use COLO for non-page background")continue;
    var section=getSection(PlacePreference(text,path));if(text=="MIDI input device..."){var midi=CreateMidiPreferenceSelector(refreshers);section.AddField("MIDI input device",midi);choices.Add("MIDI input device",midi);tooltip.SetToolTip(midi,"Windows MIDI input port for F2 pitch and live rhythm entry. Disabled closes the receiver. Duplicate identities are rejected; devices are refreshed in the background.");continue;}var value=InlinePreference(text);
    if(value!=null&&value.Label=="Interface font family"){var families=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=150,AccessibleName=value.Label,Margin=new Padding(2,1,2,1)};using(var fonts=new System.Drawing.Text.InstalledFontCollection())families.Items.AddRange(fonts.Families.Where(f=>f.IsStyleAvailable(FontStyle.Regular)).Select(f=>f.Name).Distinct().OrderBy(n=>n).Cast<object>().ToArray());families.DropDownWidth=400;section.AddField(value.Label,families);tooltip.SetToolTip(families,value.Help);families.SelectedIndexChanged+=(s,e)=>{if(!refreshing&&families.SelectedItem!=null){Safe(()=>value.Write((string)families.SelectedItem));if(refresh!=null)refresh();}};refreshers.Add(()=>{families.SelectedItem=value.Read();families.Enabled=item.Enabled;});continue;}
    if(value!=null){
     var line=new TableLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,Anchor=AnchorStyles.Left,ColumnCount=2,RowCount=1,Margin=Padding.Empty};line.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));line.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
     var field=new TextBox{Width=value.Folder||value.File?222:63,Margin=new Padding(2,1,2,1),Tag=item,AccessibleName=value.Label};line.Controls.Add(field,0,0);if(text.StartsWith("Use external VGA"))vgaField=field;Control caption;
     ToolStripMenuItem enable=null;if(value.EnableSetting!=null)toggles.TryGetValue(value.EnableSetting,out enable);
     if(enable!=null&&value.EnableSetting!="Show margin rulers")caption=checkbox(enable);else caption=new Label{Text=value.Label,AutoEllipsis=true,Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft,Height=22,Margin=new Padding(2,1,8,1),UseMnemonic=false};
     if(value.Active!=null||text=="Text area font size..."){var active=new CheckBox{Text=value.Label,AutoSize=false,AutoEllipsis=true,Height=22,Dock=DockStyle.Fill,Margin=new Padding(2,1,8,1)};caption=active;active.CheckedChanged+=(s,e)=>{if(refreshing)return;Safe(()=>{if(value.Activate!=null)value.Activate(active.Checked);else SetTextAreaFontSize(active.Checked?float.Parse(field.Text,System.Globalization.CultureInfo.InvariantCulture):0,true);});if(refresh!=null)refresh();};refreshers.Add(()=>active.Checked=value.Active!=null?value.Active():textAreaFontPoints>0);}
     tooltip.SetToolTip(caption,value.Help);tooltip.SetToolTip(field,value.Help+"\n"+(value.Folder?"Enter an existing absolute folder path, or Browse.":CompatibilityHelp(item)));
     if(value.Units.Length>0)line.Controls.Add(new Label{Text=value.Units,AutoSize=true,Margin=new Padding(2,5,1,1)},1,0);
     Action commit=()=>{if(refreshing||!field.Enabled)return;try{string current=value.Read();if(field.Text!=current)value.Write(field.Text.Trim());errors.SetError(field,"");if(refresh!=null)refresh();}catch(Exception ex){errors.SetError(field,ex.Message);}};
     field.Validated+=(s,e)=>commit();field.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){commit();e.SuppressKeyPress=true;}else if(e.KeyCode==Keys.Escape){field.Text=value.Read();errors.SetError(field,"");e.SuppressKeyPress=true;}};
     if(value.Folder||value.File){var browse=new Button{Text="Browse…",Size=new Size(65,24),AutoSize=false,Margin=new Padding(3,0,1,0),Tag="folder-browse"};line.Controls.Add(browse,1,0);browse.Click+=(s,e)=>{if(value.File){using(var d=new OpenFileDialog{Title=value.Label,FileName=value.Read(),Filter=value.Filter})if(d.ShowDialog(page.FindForm())==DialogResult.OK){Safe(()=>value.Write(d.FileName));if(refresh!=null)refresh();}}else using(var d=new FolderBrowserDialog{SelectedPath=value.Read(),Description=value.Label})if(d.ShowDialog(page.FindForm())==DialogResult.OK){field.Text=d.SelectedPath;commit();}};}
     section.AddField(caption,line);refreshers.Add(()=>{if(!field.Focused)field.Text=value.Read();field.Enabled=item.Enabled&&(enable==null||enable.Enabled&&enable.Checked)&&(value.Active==null||value.Active())&&(text!="Text area font size..."||textAreaFontPoints>0);caption.Enabled=item.Enabled;});continue;
    }
    bool colour=text.IndexOf("colour",StringComparison.OrdinalIgnoreCase)>=0;
    if(colour&&(text.StartsWith("Choose ")||text.StartsWith("Non-page background")||text.StartsWith("Ruler background")||text.StartsWith("F2 field"))){
     string role=text.StartsWith("Choose ")?text.Substring(7).Split(' ')[0]:text.StartsWith("Non-page")?"workspace":text.StartsWith("Ruler")?"ruler":"highlight";
     var line=new FlowLayoutPanel{AutoSize=true,WrapContents=false,Margin=Padding.Empty};var swatch=new Label{BorderStyle=BorderStyle.FixedSingle,Width=52,Height=23,Cursor=Cursors.Hand,Margin=new Padding(2,1,4,1),AccessibleName=role+" colour"};var code=new Label{AutoSize=true,Margin=new Padding(1,5,6,1)};var reset=new Button{Text="Reset",Size=new Size(65,24),AutoSize=false,Margin=Padding.Empty};line.Controls.AddRange(new Control[]{swatch,code,reset});
     Func<Color> read=()=>{var p=NativeScreenPalette.Read(source);if(role=="duplicate")return NativeWindowsColors.DuplicateFontColor;if(role=="workspace")return NativeScoreBehaviour.Enabled?p.Background:workspaceColor??p.Background;if(role=="ruler")return NativeWorkspaceGuides.RulerColor;if(role=="highlight")return inputHighlightColor;return role=="background"?p.Background:role=="text"?p.Text:role=="edit"?p.Edit:p.Graphics;};
     swatch.Click+=(s,e)=>Safe(()=>{if(!swatch.Enabled)return;using(var picker=new ColorDialog{Color=read(),FullOpen=true})if(picker.ShowDialog(page.FindForm())==DialogResult.OK){if(role=="workspace"){workspaceColor=picker.Color;NativeSettings.WriteAllText("workspace",picker.Color.ToArgb().ToString(System.Globalization.CultureInfo.InvariantCulture));}else if(role=="ruler"){NativeWorkspaceGuides.RulerColor=picker.Color;NativeSettings.WriteAllText("ruler-background",picker.Color.ToArgb().ToString("X8"));}else if(role=="highlight"){inputHighlightColor=picker.Color;NativeSettings.WriteAllText("input-highlight",picker.Color.ToArgb().ToString(System.Globalization.CultureInfo.InvariantCulture));if(musicInput!=null)UpdateMusicInput();}else NativeSettings.WriteAllText(role=="duplicate"?"colour-duplicate-font":"colour-"+role,picker.Color.ToArgb().ToString("X8"));RefreshScreenColors();if(refresh!=null)refresh();}});reset.Click+=(s,e)=>Safe(()=>{if(role=="workspace"){workspaceColor=null;NativeSettings.WriteAllText("workspace",null);}else if(role=="ruler"){NativeWorkspaceGuides.RulerColor=Color.FromArgb(244,244,244);NativeSettings.WriteAllText("ruler-background",null);}else if(role=="highlight"){inputHighlightColor=SystemColors.Highlight;NativeSettings.WriteAllText("input-highlight",null);if(musicInput!=null)UpdateMusicInput();}else NativeSettings.WriteAllText(role=="duplicate"?"colour-duplicate-font":"colour-"+role,null);RefreshScreenColors();if(refresh!=null)refresh();});
     section.AddField(role=="duplicate"?"Duplicate font rows":role=="workspace"?"Non-page background":role=="ruler"?"Ruler background":role=="highlight"?"Edit / F2 highlight":Char.ToUpper(role[0])+role.Substring(1),line);tooltip.SetToolTip(swatch,"Click the current colour to choose another. Windows colours override COLO except in Strict SCORE mode.");tooltip.SetToolTip(reset,"Remove this Windows override and use the SCORE palette or default colour.");refreshers.Add(()=>{Color c=read();swatch.BackColor=c;code.Text="#"+(c.ToArgb()&0xffffff).ToString("X6");swatch.Enabled=reset.Enabled=!NativeScoreBehaviour.Enabled;});continue;
    }
    string choice=PreferenceChoice(text,path);
    if(choice!=null){string key=section.Text+"|"+choice;ComboBox combo;if(!choices.TryGetValue(key,out combo)){combo=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Anchor=AnchorStyles.Left,Margin=new Padding(2,1,2,1),Tag=new List<ToolStripMenuItem>(),AccessibleName=choice};choices.Add(key,combo);var caption=section.AddField(choice,combo);var retained=combo;tooltip.SetToolTip(caption,CompatibilityHelp(item));tooltip.SetToolTip(combo,CompatibilityHelp(item));combo.SelectedIndexChanged+=(s,e)=>{if(!refreshing&&retained.SelectedItem!=null)invoke(((PreferenceChoiceItem)retained.SelectedItem).Item);};refreshers.Add(()=>{int selected=-1;bool enabled=true;for(int n=0;n<retained.Items.Count;n++){var option=((PreferenceChoiceItem)retained.Items[n]).Item;if(option.Checked)selected=n;enabled&=option.Enabled;}retained.SelectedIndex=selected;retained.Enabled=enabled;});}combo.Items.Add(new PreferenceChoiceItem(item));continue;}
    if(PreferenceToggle(text)){section.AddFull(checkbox(item));continue;}
    var action=new LinkLabel{Text=text.TrimEnd('.','…'),AutoEllipsis=true,Height=22,Dock=DockStyle.Fill,Margin=new Padding(2,1,2,1),UseMnemonic=false,TextAlign=ContentAlignment.MiddleLeft,Tag=item};action.LinkClicked+=(s,e)=>invoke(item);section.AddFull(action);tooltip.SetToolTip(action,CompatibilityHelp(item));refreshers.Add(()=>action.Enabled=item.Enabled);
   }
   Action fitChoices=()=>{foreach(var c in choices.Values){int width=40;foreach(var option in c.Items)width=Math.Max(width,TextRenderer.MeasureText(option.ToString(),c.Font).Width+SystemInformation.VerticalScrollBarWidth+12);if(c.MinimumSize.Width!=width)c.MinimumSize=new Size(width,0);if(c.Width!=width)c.Width=width;c.DropDownWidth=width;} ComboBox framing;if(vgaField!=null&&choices.TryGetValue("Zoom framing|Zoom framing",out framing)&&vgaField.Width!=framing.Width)vgaField.Width=framing.Width;};fitChoices();page.Layout+=(s,e)=>fitChoices();
   PreferenceSection strict;
   if(sections.TryGetValue("SCORE compatibility|Strict SCORE behaviour",out strict)){
    panels["SCORE compatibility"].Controls.SetChildIndex(strict,0);
    var explain=new LinkLabel{Text="Explain compatibility settings…",AutoSize=true,Margin=new Padding(2,4,2,4)};
    explain.LinkClicked+=(s,e)=>Safe(()=>OpenModernHelp("COMPAT",true));
    strict.AddFull(explain);
   }
   Action arrange=()=>{foreach(var panel in panels.Values){int width=Math.Max(200,panel.ClientSize.Width-panel.Padding.Horizontal-SystemInformation.VerticalScrollBarWidth-2);foreach(Control child in panel.Controls){child.Width=width;child.MinimumSize=new Size(width,0);child.MaximumSize=new Size(width,0);var sec=child as PreferenceSection;if(sec!=null){int inner=Math.Max(100,width-sec.Padding.Horizontal);sec.Rows.MinimumSize=new Size(inner,0);sec.Rows.MaximumSize=new Size(inner,0);sec.Rows.Width=inner;}}}};
   refresh=()=>{if(refreshing||page.IsDisposed)return;refreshing=true;try{using(NativeSettings.SnapshotRead()){windowsPreferencesMenu.RefreshActions();foreach(var update in refreshers)update();}}finally{refreshing=false;}};
   refreshWindowsPreferencesLayout=refresh;
   foreach(var panel in panels.Values)panel.SizeChanged+=(s,e)=>arrange();tabs.SelectedIndexChanged+=(s,e)=>arrange();
   var profileControls=panels["SCORE profiles"];
   var profileOptions=new PreferenceSection("Interface and menu", "These settings select the active controls and menu layout.");
   profileOptions.AddFull(BuildScor4ProfileSelector(refreshers));profileControls.Controls.Add(profileOptions);
   var classicLibrary=new PreferenceSection("SCORE 3.11 resources","Blank uses the startup library or the path in PREF.SCR.");
   classicLibrary.AddFull(BuildProfileLibrarySelector(refreshers,false));profileControls.Controls.Add(classicLibrary);
   var scor4Library=new PreferenceSection("SCOR4 resources","Blank inherits the SCORE 3.11 library. A SCORE 4 LIB supplies the SCORE 4 presets and defaults.");
   scor4Library.AddFull(BuildProfileLibrarySelector(refreshers,true));profileControls.Controls.Add(scor4Library);
   var guidance=new Label{AutoSize=true,Text="All resources includes preference defaults, DAT glyphs, and font metrics."+Environment.NewLine+
    "Windows display, file and print dialog settings apply to both profiles."};
   profileControls.Controls.Add(guidance);
   tooltip.SetToolTip(classicLibrary,"Leave the field blank to use the installation defaults. All resources is optional.");
   tooltip.SetToolTip(scor4Library,"Leave the field blank to inherit the classic LIB. Choose All resources for SCORE 4 presets and PREF-4.SCR.");
   var footer=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Bottom,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(5),WrapContents=false};var close=new Button{Text="Close",Width=90,DialogResult=DialogResult.Cancel};var save=new Button{Text="Apply",Width=90};save.Click+=(s,e)=>apply();footer.Controls.Add(close);footer.Controls.Add(save);page.Controls.Add(tabs);page.Controls.Add(footer);page.Disposed+=(s,e)=>{if(refreshWindowsPreferencesLayout==refresh)refreshWindowsPreferencesLayout=null;};refresh();page.ResumeLayout(true);arrange();return refresh;
  }
  Action refreshWindowsPreferencesLayout;
 }
}
