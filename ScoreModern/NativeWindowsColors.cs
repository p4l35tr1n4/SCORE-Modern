using System;using System.Drawing;using System.Globalization;using System.Windows.Forms;
namespace ScoreModern {
 internal static class NativeWindowsColors {
  internal static Color ReadColor(string key,Color fallback){int value;return Int32.TryParse(NativeSettings.Read(key),NumberStyles.HexNumber,CultureInfo.InvariantCulture,out value)?Color.FromArgb(unchecked((int)0xff000000)|value):fallback;}
  internal static void Apply(NativeScreenPalette palette,bool hasPreference){if(NativeScoreBehaviour.Enabled)return;palette.Graphics=ReadColor("colour-notation",palette.Graphics);palette.Background=ReadColor("colour-background",palette.Background);palette.Text=ReadColor("colour-text",palette.Text);palette.Edit=ReadColor("colour-edit",palette.Edit);}
  internal static string Status(string root){return (NativeScoreBehaviour.Enabled?"PREF.SCR COLO is effective; Windows choices retained.":"Windows RGB choices override where set; other colours use COLO/defaults.");}
  internal static Color DuplicateFontColor{get{return ReadColor("colour-duplicate-font",Color.FromArgb(255,232,232));}}
  internal static Color TextOutputBackground{get{return ReadColor("colour-output-background",SystemColors.Window);}}
  internal static Color TextOutputText{get{return ReadColor("colour-output-text",SystemColors.WindowText);}}
  static void Refresh(){foreach(Form form in Application.OpenForms){var editor=form as Editor;if(editor!=null)editor.RefreshScreenColors();}}
  internal static void AddMenu(ContextMenuStrip menu){var group=new ToolStripMenuItem("Windows screen colours");menu.Items.Add(group);group.DropDownItems.Add("Choose duplicate font colour...",null,(s,e)=>{using(var d=new ColorDialog{FullOpen=true,Color=DuplicateFontColor})if(d.ShowDialog(menu.FindForm())==DialogResult.OK)NativeSettings.WriteAllText("colour-duplicate-font",d.Color.ToArgb().ToString("X8"));});foreach(string role in new[]{"background","text","edit","notation","output-background","output-text"}){string key="colour-"+role;group.DropDownItems.Add("Choose "+role+" colour...",null,(s,e)=>{using(var d=new ColorDialog{FullOpen=true,Color=role=="output-background"?TextOutputBackground:role=="output-text"?TextOutputText:ReadColor(key,role=="background"?Color.White:Color.Black)})if(d.ShowDialog(menu.FindForm())==DialogResult.OK){NativeSettings.WriteAllText(key,d.Color.ToArgb().ToString("X8"));Refresh();}});if(!role.StartsWith("output-"))group.DropDownItems.Add("Use PREF.SCR "+role+" colour",null,(s,e)=>{NativeSettings.WriteAllText(key,null);Refresh();});}}


 }
 public sealed partial class Editor {internal void RefreshScreenColors(){ApplyPreferenceColors();RefreshPageFrame();}}
}
