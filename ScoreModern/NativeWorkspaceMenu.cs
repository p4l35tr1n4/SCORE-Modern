using System.Drawing;
using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  void ShowWorkspaceMenu(){
   var menu=viewport.ContextMenuStrip;
   if(menu==null)return;
   Point point=viewport.PointToClient(Cursor.Position);
   if(!viewport.ClientRectangle.Contains(point))
    point=new Point(viewport.ClientSize.Width/2,viewport.ClientSize.Height/2);
   menu.Show(viewport,point);
  }

  void InstallWorkspaceMenu(){
   var menu=new ContextMenuStrip();
   menu.Items.Add("Open...",null,(s,e)=>Safe(Open));
   var recent=new ToolStripMenuItem("Recent");
   menu.Items.Add(recent);
   var save=menu.Items.Add("Save",null,(s,e)=>Safe(()=>SaveCurrent()));
   var saveAs=menu.Items.Add("Save as...",null,(s,e)=>Safe(()=>SaveCopy()));
   var print=menu.Items.Add("Print...",null,(s,e)=>Safe(Print));
   menu.Items.Add(new ToolStripSeparator());
   var page=menu.Items.Add("PAGE...",null,(s,e)=>Safe(ShowPageParts));
   var scorlas=menu.Items.Add("SCORLAS...",null,(s,e)=>Safe(ShowScorlas));
   var just=menu.Items.Add("JUST...",null,(s,e)=>Safe(ShowNativeJust));
   var draw=menu.Items.Add("DRAW...",null,(s,e)=>Safe(ShowNativeDraw));
   menu.Items.Add("FONTCONV...",null,(s,e)=>ShowPscGenerator(null));
   var symbols=menu.Items.Add("Symbol Browser...",null,(s,e)=>Safe(ShowLibrarySymbolBrowser));
   var tmbSeparator=new ToolStripSeparator();
   menu.Items.Add(tmbSeparator);
   var tmbEntries=new System.Collections.Generic.Dictionary<string,ToolStripItem>();
   foreach(string name in TmbUtility.Names){
    string caption=name=="ES"?"EDITSCOR (ES)":name;
    var entry=menu.Items.Add(caption+(name=="BEAM"?"":"..."),null,(s,e)=>Safe(()=>{
     foreach(var tool in TmbUtility.Read())
      if(tool.Name==name&&tool.Enabled){RunTmbUtility(tool);break;}
    }));
    entry.Name=caption;
    tmbEntries.Add(name,entry);
   }
   var separator=new ToolStripSeparator();var preferences=new ToolStripMenuItem("Preferences...",null,(s,e)=>Safe(ShowPreferences)){ShortcutKeyDisplayString="Alt+P"};menu.Items.Add(separator);menu.Items.Add(preferences);
   System.Action refresh=()=>{
    menu.Items.Remove(separator);menu.Items.Remove(preferences);menu.Items.Add(separator);menu.Items.Add(preferences);
    while(recent.DropDownItems.Count>0)recent.DropDownItems[0].Dispose();
    foreach(string path in NativeRecentFiles.Visible()){
     string id="recent:"+path;
     var entry=new ToolStripMenuItem(System.IO.Path.GetFileName(path).Replace("&","&&")){
      Name=id,ToolTipText=path,ShortcutKeyDisplayString=ContextShortcutDisplay(id)};
     entry.Click+=(sender,args)=>Safe(()=>LoadFile(path));
     recent.DropDownItems.Add(entry);
    }
    recent.Visible=NativeRecentFiles.Count>0;
    recent.Enabled=recent.DropDownItems.Count>0;
    save.Enabled=saveAs.Enabled=score!=null&&mode!=ScreenMode.Edit;
    print.Enabled=score!=null;
    bool anyTmb=false,canRunTmb=CanRunTmbUtility();
    foreach(var tool in TmbUtility.Read()){
     tmbEntries[tool.Name].Visible=tool.Enabled;
     tmbEntries[tool.Name].Enabled=canRunTmb;
     anyTmb|=tool.Enabled;
    }
    tmbSeparator.Visible=anyTmb;
    just.Text=scor4Interface?"JUST4...":"JUST...";
    page.Enabled=scorlas.Enabled=just.Enabled=draw.Enabled=symbols.Enabled=mode==ScreenMode.Main&&musicInput==null&&groupCommand==null&&resourceEntry==null;
   };
   InitializeContextShortcuts(menu,refresh);
   menu.Opening+=(s,e)=>{
    refresh();
    var preferencesKey=Keys.Alt|Keys.P;
    bool preferencesKeyReassigned=ShortcutActive(preferencesKey)&&contextShortcuts.ContainsValue(preferencesKey);
    foreach(var pair in shortcutMenuItems)
     ((ToolStripMenuItem)pair.Value).ShortcutKeyDisplayString=ContextShortcutDisplay(pair.Key,
      pair.Value==preferences&&!preferencesKeyReassigned?"Alt+P":"");
   };
   NativeInterfaceText.AttachMenu(this,menu);
   canvas.ContextMenuStrip=null;
   // Canvas has its own multi-button SCORE gestures and blocks WM_CONTEXTMENU.
   // The text area and exposed workspace share the same application menu.
   Disposed+=(s,e)=>menu.Dispose();
  }
 }
}

