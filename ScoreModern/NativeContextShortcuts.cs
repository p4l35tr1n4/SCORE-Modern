using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  const string ContextShortcutSetting="context-menu-shortcuts";
  Action refreshShortcutMenu;
  readonly Dictionary<string,ToolStripItem> shortcutMenuItems=new Dictionary<string,ToolStripItem>(StringComparer.OrdinalIgnoreCase);
  readonly Dictionary<string,Keys> contextShortcuts=new Dictionary<string,Keys>(StringComparer.OrdinalIgnoreCase);
  bool overrideScoreShortcuts;

  static bool FunctionShortcut(Keys code){return code>=Keys.F1&&code<=Keys.F24;}
  static bool ValidContextShortcut(Keys key){
   Keys code=key&Keys.KeyCode;
   bool letterOrDigit=(code>=Keys.A&&code<=Keys.Z)||(code>=Keys.D0&&code<=Keys.D9);
   return key!=(Keys.Alt|Keys.F4)&&(FunctionShortcut(code)||letterOrDigit&&(key&(Keys.Control|Keys.Alt))!=0);
  }
  static bool ConflictsWithScore(Keys key){
   Keys code=key&Keys.KeyCode, modifiers=key&Keys.Modifiers;
   if(code>=Keys.F1&&code<=Keys.F12)return true;
   // Include bindings in both profiles and in item editing, F2 and the overview.
   if((modifiers&Keys.Control)!=0&&"ACFIOQRSVXYZ".IndexOf((char)code)>=0)return true;
   if(modifiers==Keys.Alt)
    return "BCDFGHIMOPSUY".IndexOf((char)code)>=0||code>=Keys.D0&&code<=Keys.D9;
   return key==(Keys.Control|Keys.Alt|Keys.D)||key==(Keys.Alt|Keys.Shift|Keys.M);
  }

  void InitializeContextShortcuts(ContextMenuStrip menu,Action refresh){
   refreshShortcutMenu=refresh;
   foreach(ToolStripItem item in menu.Items)
    if(item is ToolStripMenuItem)
     shortcutMenuItems.Add(String.IsNullOrEmpty(item.Name)?item.Text:item.Name,item);
   var saved=(NativeSettings.Read(ContextShortcutSetting)??"").Split('\n');
   overrideScoreShortcuts=saved[0].Trim()=="override";
   foreach(string line in saved.Skip(1)){
    int tab=line.IndexOf('\t'), value;
    if(tab<1||!Int32.TryParse(line.Substring(0,tab),out value))continue;
    var key=(Keys)value;
    if(ValidContextShortcut(key)&&!contextShortcuts.ContainsValue(key))
     contextShortcuts[line.Substring(tab+1).TrimEnd('\r')]=key;
   }
  }

  void SaveContextShortcuts(){
   NativeSettings.WriteAllText(ContextShortcutSetting,(overrideScoreShortcuts?"override":"protect")+"\n"+
    String.Join("\n",contextShortcuts.Select(pair=>((int)pair.Value).ToString(CultureInfo.InvariantCulture)+"\t"+pair.Key)));
  }

  bool ShortcutActive(Keys key){
   return !ConflictsWithScore(key)||overrideScoreShortcuts&&!NativeScoreBehaviour.Enabled;
  }
  static string ShortcutText(Keys key){return new KeysConverter().ConvertToString(key);}
  string ContextShortcutDisplay(string id,string fallback=""){
   Keys key;
   return contextShortcuts.TryGetValue(id,out key)&&ShortcutActive(key)?ShortcutText(key):fallback;
  }

  bool RunContextShortcut(Keys keys){
   if(!contextShortcuts.ContainsValue(keys)||!ShortcutActive(keys))return false;
   string id=contextShortcuts.First(pair=>pair.Value==keys).Key;
   refreshShortcutMenu();
   ToolStripItem item;
   if(!shortcutMenuItems.TryGetValue(id,out item)){
    var recent=(ToolStripMenuItem)shortcutMenuItems["Recent"];
    item=recent.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(entry=>String.Equals(entry.Name,id,StringComparison.OrdinalIgnoreCase));
    if(item==null||!recent.Available||!recent.Enabled)return false;
   }
   if(!item.Available||!item.Enabled)return false;
   if(id=="Recent"){
    ShowWorkspaceMenu();
    ((ToolStripMenuItem)item).ShowDropDown();
   }else item.PerformClick();
   return true;
  }

  sealed class ContextShortcutCapture:TextBox {
   internal Keys Captured;
   internal ContextShortcutCapture(){ReadOnly=true;Width=180;AccessibleName="Press shortcut keys";}
   void CaptureKey(Keys key){
    var code=key&Keys.KeyCode;
    if(code==Keys.ShiftKey||code==Keys.ControlKey||code==Keys.Menu)return;
    Captured=key;
    Text=ShortcutText(key);
   }
   protected override bool ProcessCmdKey(ref System.Windows.Forms.Message message,Keys keyData){
    if(keyData==Keys.Tab||keyData==(Keys.Shift|Keys.Tab)||keyData==Keys.Escape)
     return base.ProcessCmdKey(ref message,keyData);
    CaptureKey(keyData);
    return true;
   }
   protected override void OnKeyDown(KeyEventArgs e){CaptureKey(e.KeyData);e.SuppressKeyPress=true;}
  }

  void PopulateContextShortcutPreferences(TabPage page){
   var table=new DataGridView {Dock=DockStyle.Fill,ReadOnly=true,MultiSelect=false,AllowUserToAddRows=false,
    AllowUserToDeleteRows=false,RowHeadersVisible=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,
    AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
   table.Columns.Add("item","Context menu item");
   table.Columns.Add("shortcut","Assigned shortcut");
   table.Columns[1].FillWeight=35;
   var capture=new ContextShortcutCapture();
   var assign=new Button{Text="Assign",AutoSize=true};
   var clear=new Button{Text="Clear",AutoSize=true};
   var overrides=new CheckBox{Text="Allow shortcuts to override SCORE defaults",AutoSize=true};
   var status=new Label{AutoSize=true,Text="Select an item, click the shortcut field, then press a key combination."};
   var panel=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,WrapContents=true,Padding=new Padding(6)};
   panel.Controls.AddRange(new Control[]{capture,assign,clear,overrides,status});
   page.Controls.Add(table);
   page.Controls.Add(panel);
   Action fill=()=>{
    var selected=table.CurrentRow==null?null:table.CurrentRow.Tag as string;
    table.Rows.Clear();
    foreach(var pair in shortcutMenuItems){
     table.Rows[table.Rows.Add(pair.Value.Text,ContextShortcutDisplayForEditor(pair.Key))].Tag=pair.Key;
     if(pair.Key!="Recent")continue;
     var paths=NativeRecentFiles.Visible().Concat(contextShortcuts.Keys
      .Where(id=>id.StartsWith("recent:",StringComparison.OrdinalIgnoreCase)).Select(id=>id.Substring(7)))
      .Distinct(StringComparer.OrdinalIgnoreCase);
     foreach(string path in paths){
      string id="recent:"+path;
      var row=table.Rows[table.Rows.Add("Recent › "+System.IO.Path.GetFileName(path),ContextShortcutDisplayForEditor(id))];
      row.Tag=id;
      row.Cells[0].ToolTipText=path;
     }
    }
    foreach(DataGridViewRow row in table.Rows)
     if((string)row.Tag==selected){table.CurrentCell=row.Cells[0];break;}
   };
   overrides.Checked=overrideScoreShortcuts;
   overrides.CheckedChanged+=(sender,args)=>Safe(()=>{
    if(overrides.Checked==overrideScoreShortcuts)return;
    if(!overrides.Checked&&contextShortcuts.Values.Any(ConflictsWithScore)){
     overrides.Checked=true;
     status.Text="Clear the shortcuts marked as SCORE conflicts before turning off override.";
     return;
    }
    overrideScoreShortcuts=overrides.Checked;
    SaveContextShortcuts();
    fill();
    status.Text="Saved.";
   });
   assign.Click+=(sender,args)=>Safe(()=>{
    if(table.CurrentRow==null)return;
    var key=capture.Captured;
    if(!ValidContextShortcut(key)){status.Text="Use Ctrl or Alt with a letter or digit, or a function key. Alt+F4 is reserved.";return;}
    if(!overrideScoreShortcuts&&ConflictsWithScore(key)){status.Text="This shortcut is reserved by SCORE or the editor.";return;}
    string id=(string)table.CurrentRow.Tag;
    if(contextShortcuts.Any(pair=>pair.Value==key&&!String.Equals(pair.Key,id,StringComparison.OrdinalIgnoreCase))){
     status.Text="This shortcut already belongs to another menu item.";
     return;
    }
    contextShortcuts[id]=key;
    SaveContextShortcuts();
    fill();
    status.Text="Saved.";
   });
   clear.Click+=(sender,args)=>Safe(()=>{
    if(table.CurrentRow==null)return;
    contextShortcuts.Remove((string)table.CurrentRow.Tag);
    SaveContextShortcuts();
    fill();
    status.Text="Saved.";
   });
   page.VisibleChanged+=(sender,args)=>{if(page.Visible)fill();};
   fill();
  }

  string ContextShortcutDisplayForEditor(string id){
   Keys key;
   if(!contextShortcuts.TryGetValue(id,out key))return "";
   return ShortcutText(key)+(ConflictsWithScore(key)?" (SCORE conflict)":"");
  }
 }
}
