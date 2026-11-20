using System;using System.Windows.Forms;
namespace ScoreModern {
 internal sealed class NativePrintCommandBox:TextBox {
  internal Func<bool> AppendOnly;
  bool OriginalReader{get{return AppendOnly!=null&&AppendOnly();}}
  // Original2E2C:145E..1538 appends printable bytes, ignores navigation,
  // and backspaces the final byte. It is independent of the shared line editor.
  protected override void OnKeyDown(KeyEventArgs e){
   base.OnKeyDown(e);if(e.Handled||e.SuppressKeyPress||!OriginalReader)return;
   if(e.Modifiers==Keys.None&&e.KeyCode==Keys.Back){if(TextLength>0)Text=Text.Substring(0,TextLength-1);SelectionStart=TextLength;e.Handled=e.SuppressKeyPress=true;return;}
   if((e.Modifiers&(Keys.Control|Keys.Alt))!=0||e.KeyCode==Keys.Left||e.KeyCode==Keys.Right||e.KeyCode==Keys.Up||e.KeyCode==Keys.Down||e.KeyCode==Keys.Home||e.KeyCode==Keys.End||e.KeyCode==Keys.Insert||e.KeyCode==Keys.Delete||e.KeyCode==Keys.PageUp||e.KeyCode==Keys.PageDown||e.KeyCode==Keys.Tab)e.Handled=e.SuppressKeyPress=true;
  }
  protected override void OnKeyPress(KeyPressEventArgs e){base.OnKeyPress(e);if(e.Handled||!OriginalReader)return;if(e.KeyChar>=32&&e.KeyChar<=255){Text+=e.KeyChar;SelectionStart=TextLength;e.Handled=true;}else if(e.KeyChar!=13)e.Handled=true;}
 }
}
