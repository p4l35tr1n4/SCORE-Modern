using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
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
   var symbols=menu.Items.Add("Symbol Browser...",null,(s,e)=>Safe(ShowLibrarySymbolBrowser));
   var tmbSeparator=new ToolStripSeparator();
   menu.Items.Add(tmbSeparator);
   var tmbEntries=new System.Collections.Generic.Dictionary<string,ToolStripItem>();
   foreach(string name in TmbUtility.Names){
    string caption=name=="ES"?"EDITSCOR (ES)":name;
    tmbEntries.Add(name,menu.Items.Add(caption,null,(s,e)=>Safe(()=>{
     foreach(var tool in TmbUtility.Read())
      if(tool.Name==name&&tool.Enabled){RunTmbUtility(tool);break;}
    })));
   }
   var separator=new ToolStripSeparator();var preferences=new ToolStripMenuItem("Preferences...",null,(s,e)=>Safe(ShowPreferences));menu.Items.Add(separator);menu.Items.Add(preferences);
   menu.Opening+=(s,e)=>{
    menu.Items.Remove(separator);menu.Items.Remove(preferences);menu.Items.Add(separator);menu.Items.Add(preferences);
    while(recent.DropDownItems.Count>0)recent.DropDownItems[0].Dispose();
    foreach(string path in NativeRecentFiles.Visible()){
     var entry=new ToolStripMenuItem(System.IO.Path.GetFileName(path).Replace("&","&&")){ToolTipText=path};
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
   NativeInterfaceText.AttachMenu(this,menu);
   canvas.ContextMenuStrip=null;
   // Canvas has its own multi-button SCORE gestures and blocks WM_CONTEXTMENU.
   // The text area and exposed workspace share the same application menu.
   Disposed+=(s,e)=>menu.Dispose();
  }
 }
}

