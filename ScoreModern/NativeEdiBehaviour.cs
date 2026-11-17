using System;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeEdiBehaviour {
  static bool documented=NativeSettings.Read("edi-repeated-marks")=="documented";
  public static bool DocumentedMarks{get{return !NativeScoreBehaviour.Enabled&&documented;}}
  internal static void AddMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("EDI: retain documented multiple-dot/trill marks");item.ToolTipText="Optional: marks26–29 become S2/S3/S4/TR. Strict SCORE uses its measured TL conversion for all four.";item.Click+=(s,e)=>{documented=!documented;NativeSettings.WriteAllText("edi-repeated-marks",documented?"documented":"original");};menu.Items.Add(item);menu.Opening+=(s,e)=>{item.Checked=DocumentedMarks;item.Enabled=!NativeScoreBehaviour.Enabled;};}
 }
}
