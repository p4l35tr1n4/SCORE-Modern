using System;using System.Drawing;using System.Linq;using System.Windows.Forms;
namespace ScoreModern {
 internal static class NativeInformationNotice {
  internal const string SettingsKey="dismissed-information";
  internal static bool IsSuppressed(string key){return (NativeSettings.Read(SettingsKey)??"").Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).Contains(key);}
  internal static void Suppress(string key){var values=(NativeSettings.Read(SettingsKey)??"").Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).Concat(new[]{key}).Distinct().OrderBy(s=>s);NativeSettings.WriteAllText(SettingsKey,String.Join("\n",values));}
  internal static void Reset(){NativeSettings.WriteAllText(SettingsKey,null);}
  internal static void Show(IWin32Window owner,string key,string title,string message){
   if(IsSuppressed(key)||Program.CheckMode)return;
   using(var dialog=new ScoreDialog("information-"+key){Text=title,StartPosition=FormStartPosition.CenterParent,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,MinimizeBox=false,MaximizeBox=false}){
    var panel=new TableLayoutPanel{Dock=DockStyle.Fill,AutoSize=true,ColumnCount=1,Padding=new Padding(18)};
    var text=new Label{Text=message,AutoSize=true,MaximumSize=new Size(560,0),UseMnemonic=false,Margin=new Padding(0,0,0,18)};
    var dismiss=new CheckBox{Text="Don't show me this again",AutoSize=true,Margin=new Padding(0,0,0,14),AccessibleName="Don't show me this again"};
    var buttons=new FlowLayoutPanel{AutoSize=true,Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft,Margin=Padding.Empty};
    var ok=new Button{Text="OK",AutoSize=true,DialogResult=DialogResult.OK};buttons.Controls.Add(ok);panel.Controls.Add(text);panel.Controls.Add(dismiss);panel.Controls.Add(buttons);dialog.Controls.Add(panel);dialog.AcceptButton=ok;dialog.CancelButton=ok;
    if(dialog.ShowDialog(owner)==DialogResult.OK&&dismiss.Checked)Suppress(key);
   }
  }
 }
}
