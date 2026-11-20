using System;using System.Drawing;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeKeyboardOptions {
  static bool clearHome=NativeSettings.Read("keyboard-home")=="clear",fivePercent=NativeSettings.Read("keyboard-main-pan")=="5",recallR=NativeSettings.Read("keyboard-recall-r")=="on";
  static int itemPage=ReadStep("keyboard-item-page",4,new[]{4,5,10}),itemTab=ReadStep("keyboard-item-tab",10,new[]{5,10});
  static int ReadStep(string key,int fallback,int[] allowed){int value;return Int32.TryParse(NativeSettings.Read(key),out value)&&Array.IndexOf(allowed,value)>=0?value:fallback;}
  public static float ItemPageStep{get{return NativeScoreBehaviour.Enabled?4:itemPage;}}
  public static float ItemTabStep{get{return NativeScoreBehaviour.Enabled?10:itemTab;}}
  static bool cancelFilterEscape=NativeSettings.Read("keyboard-filter-escape")=="cancel";
  public static bool FilterEscapeCancels{get{return !NativeScoreBehaviour.Enabled&&cancelFilterEscape;}}
  static bool editOverview=NativeSettings.Read("keyboard-edit-overview")=="on";
  public static bool EditOverview{get{return !NativeScoreBehaviour.Enabled&&editOverview;}}
  static bool textSetupFull=NativeSettings.Read("keyboard-text-setup-full")=="on";
  static bool groupFull=NativeSettings.Read("keyboard-group-full")=="on";
  public static bool GroupFull{get{return !NativeScoreBehaviour.Enabled&&groupFull;}}
  public static bool TextSetupFull{get{return !NativeScoreBehaviour.Enabled&&textSetupFull;}}
  public static bool RecallR{get{return !NativeScoreBehaviour.Enabled&&recallR;}}
  public static bool ClearHome{get{return !NativeScoreBehaviour.Enabled&&clearHome;}}
  public static float MainArrowPercent{get{return !NativeScoreBehaviour.Enabled&&fivePercent?5:10;}}
  internal static void Set(bool clear,bool five,bool save){clearHome=clear;fivePercent=five;if(save){NativeSettings.WriteAllText("keyboard-home",clear?"clear":"top");NativeSettings.WriteAllText("keyboard-main-pan",five?"5":"10");}}
  internal static void AddMenu(ContextMenuStrip menu){
   foreach(int value in new[]{4,5,10}){int step=value;var option=new ToolStripMenuItem("Keyboard: Item PgUp/PgDn: "+step+" scale steps"+(step==4?" (original)":" (Help alternative)"));option.Click+=(s,e)=>{itemPage=step;NativeSettings.WriteAllText("keyboard-item-page",step.ToString());};menu.Items.Add(option);menu.Opening+=(s,e)=>{option.Checked=ItemPageStep==step;option.Enabled=!NativeScoreBehaviour.Enabled;};}
   foreach(int value in new[]{10,5}){int step=value;var option=new ToolStripMenuItem("Keyboard: Item Tab: "+step+" horizontal units"+(step==10?" (original)":" (Help alternative)"));option.Click+=(s,e)=>{itemTab=step;NativeSettings.WriteAllText("keyboard-item-tab",step.ToString());};menu.Items.Add(option);menu.Opening+=(s,e)=>{option.Checked=ItemTabStep==step;option.Enabled=!NativeScoreBehaviour.Enabled;};}
   var cancel=new ToolStripMenuItem("Keyboard: F3 prompt Escape cancels instead of recalling the last item");cancel.Click+=(s,e)=>{cancelFilterEscape=!cancelFilterEscape;NativeSettings.WriteAllText("keyboard-filter-escape",cancelFilterEscape?"cancel":"recall");};menu.Items.Add(cancel);menu.Opening+=(s,e)=>{cancel.Checked=FilterEscapeCancels;cancel.Enabled=!NativeScoreBehaviour.Enabled;};
   var overview=new ToolStripMenuItem("Keyboard: Allow Alt+S overview during item editing");overview.ToolTipText="Optional Windows extension. Original0F4F:10CA bypasses Alt+S in Item Edit.";overview.Click+=(s,e)=>{editOverview=!editOverview;NativeSettings.WriteAllText("keyboard-edit-overview",editOverview?"on":"off");};menu.Items.Add(overview);menu.Opening+=(s,e)=>{overview.Checked=EditOverview;overview.Enabled=!NativeScoreBehaviour.Enabled;};
   var textFull=new ToolStripMenuItem("Keyboard: Allow Alt+F full graphics in text setup");textFull.ToolTipText="Optional Windows extension. The measured original Text setup leaves its graphics bounds unchanged for Alt+F.";textFull.Click+=(s,e)=>{textSetupFull=!textSetupFull;NativeSettings.WriteAllText("keyboard-text-setup-full",textSetupFull?"on":"off");};menu.Items.Add(textFull);menu.Opening+=(s,e)=>{textFull.Checked=TextSetupFull;textFull.Enabled=!NativeScoreBehaviour.Enabled;};
   var group=new ToolStripMenuItem("Keyboard: Allow Alt+F full graphics in Group menu");group.ToolTipText="Optional Windows extension. SCORE's Group menu ignores Alt+F; its other window controls retain the menu's extra header margin.";group.Click+=(s,e)=>{groupFull=!groupFull;NativeSettings.WriteAllText("keyboard-group-full",groupFull?"on":"off");};menu.Items.Add(group);menu.Opening+=(s,e)=>{group.Checked=GroupFull;group.Enabled=!NativeScoreBehaviour.Enabled;};
   var home=new ToolStripMenuItem("Keyboard: Home clears text instead of showing notation top");home.ToolTipText="Optional Tom's Help action. Original SCORE uses Home to show the highest buffered notation point. The pinned file/resource line is retained.";home.Click+=(s,e)=>Set(!clearHome,fivePercent,true);menu.Items.Add(home);
   var arrows=new ToolStripMenuItem("Keyboard: Main Up/Down uses documented 5% instead of original 10%");arrows.Click+=(s,e)=>Set(clearHome,!fivePercent,true);menu.Items.Add(arrows);
   var recall=new ToolStripMenuItem("Keyboard: Ctrl+R also recalls the last line");recall.ToolTipText="Optional CODE38 help spelling. Measured Main Ctrl+R is ignored; Ctrl+Q is the original recall key.";recall.Click+=(s,e)=>{recallR=!recallR;NativeSettings.WriteAllText("keyboard-recall-r",recallR?"on":"off");};menu.Items.Add(recall);
   menu.Opening+=(s,e)=>{recall.Checked=RecallR;recall.Enabled=!NativeScoreBehaviour.Enabled;home.Checked=ClearHome;arrows.Checked=MainArrowPercent==5;home.Enabled=arrows.Enabled=!NativeScoreBehaviour.Enabled;};
  }
 }
 public sealed partial class Editor {
  void ClearCommandTextWindow(){command.Clear();contextLine.Text="";Message("");textWindow.AutoScrollPosition=Point.Empty;command.Focus();}
 }
}
