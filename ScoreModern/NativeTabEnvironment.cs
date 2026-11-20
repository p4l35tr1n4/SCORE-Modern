using System;using System.Drawing;using System.Globalization;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeTabEnvironment {
  static bool reject=NativeSettings.Read("tab-quarter-tones")=="reject";
  static ushort loadSegment=ReadSegment();
  static bool rejectDelta=NativeSettings.Read("tab-accidental-table")=="reject";
  public static short[] AccidentalTable(){return !NativeScoreBehaviour.Enabled&&rejectDelta?null:RecoveredFragments.LayoutTablatureAccidentalTable.Recover(loadSegment);}
  internal static ushort LoadSegment{get{return loadSegment;}}
  internal static void SetLoadSegment(ushort value){loadSegment=value;NativeSettings.WriteAllText("tab-dos-load-segment",value.ToString("X4"));}
  static ushort ReadSegment(){ushort value;return UInt16.TryParse(NativeSettings.Read("tab-dos-load-segment"),NumberStyles.HexNumber,CultureInfo.InvariantCulture,out value)?value:(ushort)0x0824;}
  public static bool RejectQuarterTones{get{return !NativeScoreBehaviour.Enabled&&reject;}}
  public static short[] CollectorTable(){return new short[]{0,-1,1,3,-2,2,10512,0,unchecked((short)(12250+loadSegment)),74};}
  //32AC:0108 indexes DS:4364 without a bound check. Original EXEPACK relocation
  //2512 applies to the word at unpacked image83314h (index8); raw word2FDAh.
  //The canonical test VM loads at0824h and reads37FEh. Other logical load
  //segments are explicit configuration, never Windows pointers or host reads.
  internal static void AddMenu(ContextMenuStrip menu){var original=new ToolStripMenuItem("TAB: original quarter-tone table behaviour");var safe=new ToolStripMenuItem("TAB: reject quarter-tone conversion");var context=new ToolStripMenuItem("TAB: DOS load segment (hex)...");original.Click+=(s,e)=>{reject=false;NativeSettings.WriteAllText("tab-quarter-tones","original");};safe.Click+=(s,e)=>{reject=true;NativeSettings.WriteAllText("tab-quarter-tones","reject");};context.ToolTipText="Only affects address-dependent TAB quarter-tone calculations. 0824 matches the verified DOSBox-X reference VM; no Windows memory is accessed.";context.Click+=(s,e)=>{using(var dialog=new ScoreDialog("tab-dos-context"){Text="TAB original DOS context",StartPosition=FormStartPosition.CenterParent,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink}){var panel=new FlowLayoutPanel{AutoSize=true,FlowDirection=FlowDirection.TopDown,Padding=new Padding(12),WrapContents=false};panel.Controls.Add(new Label{AutoSize=true,MaximumSize=new Size(600,0),Text="DOS load segment, hexadecimal (0000–FFFF).\n0824 matches the verified reference VM. This controls the original's address-dependent quarter-tone TAB quirk; it is not a Windows memory address."});var value=new NumericUpDown{Hexadecimal=true,Minimum=0,Maximum=65535,Value=loadSegment,Width=150};panel.Controls.Add(value);var apply=new Button{Text="Apply",DialogResult=DialogResult.OK};panel.Controls.Add(apply);dialog.Controls.Add(panel);dialog.AcceptButton=apply;if(dialog.ShowDialog(menu.FindForm())==DialogResult.OK){loadSegment=(ushort)value.Value;NativeSettings.WriteAllText("tab-dos-load-segment",loadSegment.ToString("X4"));}}};var delta=new ToolStripMenuItem("TAB: reject adjacent accidental-table reads");delta.Click+=(s,e)=>{rejectDelta=!rejectDelta;NativeSettings.WriteAllText("tab-accidental-table",rejectDelta?"reject":"original");};menu.Items.Add(delta);menu.Opening+=(s,e)=>{delta.Checked=!NativeScoreBehaviour.Enabled&&rejectDelta;delta.Enabled=!NativeScoreBehaviour.Enabled;};menu.Items.Add(original);menu.Items.Add(safe);menu.Items.Add(context);menu.Opening+=(s,e)=>{original.Checked=!RejectQuarterTones;safe.Checked=RejectQuarterTones;safe.Enabled=!NativeScoreBehaviour.Enabled;context.Text="TAB: DOS load segment (hex): "+loadSegment.ToString("X4")+"...";};}
 }
}
