using System;using System.IO;using System.Windows.Forms;
namespace ScoreModern {
 // Effective overrides preserve individual preferences while this preset is on.
 public static class NativeScoreBehaviour {
  public static bool Enabled{get;private set;}
  public static void Load(){Enabled=false;try{Enabled=NativeSettings.Read("score-behaviour")=="original";}catch(IOException){}catch(UnauthorizedAccessException){}}
  static readonly System.Collections.Generic.Dictionary<ToolStripMenuItem,bool> lockedItems=new System.Collections.Generic.Dictionary<ToolStripMenuItem,bool>();
  public static void Set(bool value,bool save){if(save)NativeSettings.WriteAllText("score-behaviour",value?"original":"custom");Enabled=value;if(!value){foreach(var entry in lockedItems)if(!entry.Key.IsDisposed)entry.Key.Enabled=entry.Value;lockedItems.Clear();}foreach(Form form in Application.OpenForms){var editor=form as Editor;if(editor!=null){editor.RefreshZoomFraming();editor.RefreshScreenColors();}form.Invalidate(true);}}
  internal static bool Locked(string text){return text.StartsWith("Printing:")||text.StartsWith("Mouse menus:")||text.StartsWith("Keyboard:")||text.StartsWith("DPY:")||text.StartsWith("Mouse selection:")||text.StartsWith("Mouse click-points:")||text.StartsWith("Editing:")||text.StartsWith("Grace-chord flips:")||text.StartsWith("Endings:")||(text.StartsWith("Slurs: reject")||text.StartsWith("Slurs and hairpins: reject"))||text.StartsWith("Cache-only text")||text.StartsWith("PostScript files:")||text.StartsWith("Allow approximate engraving");}
  static void LockItems(ToolStripItemCollection items){if(!Enabled)return;foreach(ToolStripItem entry in items){var item=entry as ToolStripMenuItem;if(item==null)continue;if(Locked(item.Text)){if(!lockedItems.ContainsKey(item))lockedItems[item]=item.Enabled;item.Enabled=false;}if(item.HasDropDownItems)LockItems(item.DropDownItems);}}
  public static void AddMenu(ContextMenuStrip menu,Action changed){var item=new ToolStripMenuItem("Strict SCORE behaviour");item.ToolTipText="Enforces available SCORE operation: original selection and state-dependent edits; no approximate engraving or alternative ending rules. Keeps Windows/4K and Type 1/Ghostscript rendering. This preset does not certify unverified commands or fault histories.";item.Click+=(s,e)=>{Set(!Enabled,true);changed();};menu.Items.Add(item);menu.Opening+=(s,e)=>{item.Checked=Enabled;LockItems(menu.Items);};}
 }
}
