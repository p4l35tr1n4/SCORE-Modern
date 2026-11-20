using System.Windows.Forms;
namespace ScoreModern {public static class NativeMenuMouseOptions {
 static bool helpExit=NativeSettings.Read("menu-three-button-exit")=="help";
 public static bool HelpExit{get{return !NativeScoreBehaviour.Enabled&&helpExit;}}
 public static void AddMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("Mouse menus: three buttons exit Edit Menu as documented");item.ToolTipText="Optional Tom's Help operation. The supplied executable translates all three buttons to F8, which does nothing in Edit Menu; left+middle or right+middle translate to Escape.";item.Click+=(s,e)=>{helpExit=!helpExit;NativeSettings.WriteAllText("menu-three-button-exit",helpExit?"help":"original");};menu.Items.Add(item);menu.Opening+=(s,e)=>{item.Checked=HelpExit;item.Enabled=!NativeScoreBehaviour.Enabled;};}
}}
