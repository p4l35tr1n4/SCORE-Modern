using System;using System.Collections.Generic;using System.Windows.Forms;
namespace ScoreModern {
 // Original2FC5:0094..0153. Keyboard queue ownership is independent of transport.
 internal sealed class NativePrintFlow {
  internal bool Disabled,Paused,ExitRequested,FinishCurrent;
  internal int Item;internal readonly List<object> TypeAhead=new List<object>();
  internal Action Changed;
  internal NativePrintFlow(short desc){Disabled=desc!=0;}
  internal void Character(char value){
   if(Disabled){Queue(value);return;}
   if(Paused){switch(Char.ToLowerInvariant(value)){case 'x':ExitRequested=true;Paused=false;break;case 'p':FinishCurrent=true;Paused=false;break;case 'c':Paused=false;break;}Notify();return;}
   if(value=='G'||value=='g')Disabled=true;else if(value==27)Paused=true;
   Notify();
  }
  internal void Key(Keys value){if(Disabled){Queue(value);return;}if(value==Keys.Escape)Character((char)27);}
  void Queue(object value){if(TypeAhead.Count<15)TypeAhead.Add(value);}
  void Notify(){if(Changed!=null)Changed();}
 }
 internal sealed class NativePrintFinishException:Exception{}
 internal sealed class NativePrintExitException:Exception{}
 public static class NativePrintBehaviour {
  static bool retainIncomplete=NativeSettings.Read("print-incomplete-eps")=="retain";
  static bool temporaryGo=NativeSettings.Read("print-go-suppression")=="job";
  static bool scorlasEmpty=NativeSettings.Read("scorlas-empty-bounds")=="original";
  internal static bool RetainIncomplete{get{return NativeScoreBehaviour.Enabled||retainIncomplete;}}
  internal static bool TemporaryGo{get{return !NativeScoreBehaviour.Enabled&&temporaryGo;}}
  internal static bool OriginalScorlasEmpty{get{return NativeScoreBehaviour.Enabled||scorlasEmpty;}}
  internal static void SetScorlasEmpty(bool value,bool save){if(save)NativeSettings.WriteAllText("scorlas-empty-bounds",value?"original":"reject");scorlasEmpty=value;}
  internal static void AddMenu(ContextMenuStrip menu){
   var retain=new ToolStripMenuItem("Printing: retain incomplete EPS when exited (original)");retain.ToolTipText="SCORE leaves an unfinished PostScript stream after X. Off protects an existing EPS and discards the incomplete replacement; Strict SCORE retains the original side effect.";retain.Click+=(s,e)=>{retainIncomplete=!retainIncomplete;NativeSettings.WriteAllText("print-incomplete-eps",retainIncomplete?"retain":"discard");};menu.Items.Add(retain);
   var reset=new ToolStripMenuItem("Printing: reset Go suppression after this job (Tom's Help)");reset.ToolTipText="Measured SCORE keeps DESC=1 after G; Tom's Help describes suppression for one run. Off preserves the executable; Strict SCORE always preserves it.";reset.Click+=(s,e)=>{temporaryGo=!temporaryGo;NativeSettings.WriteAllText("print-go-suppression",temporaryGo?"job":"retained");};menu.Items.Add(reset);
   var empty=new ToolStripMenuItem("Printing: reproduce SCORLAS empty-file bounds (original)");empty.ToolTipText="Off rejects empty files in a tiled job. On reproduces verified original empty-file scan sentinels and their reversed EPS bounding box. Strict SCORE enables original behavior; unverified empty-file positions remain guarded.";empty.Click+=(s,e)=>SetScorlasEmpty(!scorlasEmpty,true);menu.Items.Add(empty);
   menu.Opening+=(s,e)=>{retain.Checked=RetainIncomplete;retain.Enabled=!NativeScoreBehaviour.Enabled;reset.Checked=TemporaryGo;reset.Enabled=!NativeScoreBehaviour.Enabled;empty.Checked=OriginalScorlasEmpty;empty.Enabled=!NativeScoreBehaviour.Enabled;};
  }
 }
}
