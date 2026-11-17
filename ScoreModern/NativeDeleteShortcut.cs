using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  bool deleteKeyMatchesF7;
  bool DeleteKeyMatchesF7 {get{return deleteKeyMatchesF7&&!NativeScoreBehaviour.Enabled;}}
  void AddDeleteShortcutMenu(ContextMenuStrip menu){
   deleteKeyMatchesF7=NativeSettings.Read("delete-key-as-f7")=="on";
   var item=new ToolStripMenuItem("Editing: Delete key acts as F7=DEL");
   item.ToolTipText="Optional item-delete shortcut, only while F7=DEL is active. Off by default; disabled by Strict SCORE behaviour.";
   item.Click+=(s,e)=>Safe(()=>{
    bool next=!deleteKeyMatchesF7;
    NativeSettings.WriteAllText("delete-key-as-f7",next?"on":"off");
    deleteKeyMatchesF7=next;
   });
   menu.Items.Add(item);
   menu.Opening+=(s,e)=>item.Checked=DeleteKeyMatchesF7;
  }
 }
}
