using System;using System.Collections.Generic;using System.Drawing;using System.Threading;using System.Windows.Forms;
namespace ScoreModern {
 internal sealed class NativePrintExecution:IDisposable {
  [ThreadStatic]internal static NativePrintExecution Current;
  internal static Action<NativePrintExecution,int> TestInput=null;
  internal readonly NativePrintFlow Flow;internal readonly Canvas Canvas;
  readonly NativePrintExecution previous;readonly Action update;int last;
  internal NativePrintExecution(Canvas canvas,short desc,Action update){Canvas=canvas;Flow=new NativePrintFlow(desc);this.update=update;Flow.Changed=update;previous=Current;Current=this;}
  internal static void Before(Canvas canvas,Graphics graphics,int item){var run=Current;if(run==null||run.Canvas!=canvas||NativeEpsExport.For(graphics)==null&&!NativeNotationView.IsPrinting)return;run.Before(item);}
  void Before(int item){
   if(Flow.ExitRequested)throw new NativePrintExitException();
   if(Flow.FinishCurrent&&item>Flow.Item)throw new NativePrintFinishException();
   if(item==last)return;last=item;Flow.Item=item;if(update!=null)update();
   if(TestInput!=null)TestInput(this,item);
   if(!Program.CheckMode)Application.DoEvents();
   while(Flow.Paused&&!Flow.ExitRequested){if(Program.CheckMode)throw new InvalidOperationException("Print test paused without a supplied continuation.");Application.DoEvents();Thread.Sleep(10);}
   if(Flow.ExitRequested)throw new NativePrintExitException();
  }
  public void Dispose(){Current=previous;}
 }
 public sealed partial class Editor {
  NativePrintExecution printExecution;bool closeAfterPrint;
  bool PrintingKey(Keys key){if(printExecution==null)return false;if(key==(Keys.Alt|Keys.F4)){closeAfterPrint=true;printExecution.Flow.ExitRequested=true;return true;}
   Keys code=key&Keys.KeyCode;if(code==Keys.ShiftKey||code==Keys.ControlKey||code==Keys.Menu||code==Keys.LWin||code==Keys.RWin)return true;if(code==Keys.Enter||code==Keys.Escape||code==Keys.Back||code==Keys.Tab||code==Keys.Delete||code==Keys.Insert||code==Keys.Home||code==Keys.End||code==Keys.Left||code==Keys.Right||code==Keys.Up||code==Keys.Down||code==Keys.PageUp||code==Keys.PageDown||code>=Keys.F1&&code<=Keys.F24||(key&(Keys.Control|Keys.Alt))!=0){printExecution.Flow.Key(key);return true;}return false;
  }
  bool PrintingCharacter(KeyPressEventArgs key){if(printExecution==null)return false;printExecution.Flow.Character(key.KeyChar);key.Handled=true;return true;}
  void PrintProgress(){if(printExecution==null)return;var flow=printExecution.Flow;if(flow.Disabled&&!NativePrintBehaviour.TemporaryGo)printJob.Desc=1;responseLine.Text="**** Printing "+score.Items.Count+" items. ****"+Environment.NewLine+"Sending Item # "+flow.Item+(flow.Paused?Environment.NewLine+"X = Exit printing, P = Print now, C = Continue.":flow.Disabled?Environment.NewLine+"Escape disabled; subsequent keys are queued until printing finishes.":"");LayoutTextContents(true);}
  void ExecuteControlledPrint(Action output){
   if(printExecution!=null)throw new InvalidOperationException("A print job is already running.");
   var pending=new List<object>();bool exited=false;Exception failure=null;bool enabled=canvas.Enabled,context=ContextMenuStrip.Enabled,functions=functionStrip.Enabled;
   FormClosingEventHandler closing=(sender,args)=>{args.Cancel=true;closeAfterPrint=true;if(printExecution!=null)printExecution.Flow.ExitRequested=true;};FormClosing+=closing;
   try{canvas.Enabled=false;ContextMenuStrip.Enabled=false;functionStrip.Enabled=false;if(originalPrintPanel!=null)originalPrintPanel.Visible=false;command.Clear();command.Focus();
    using(var run=new NativePrintExecution(canvas,printJob.Desc,PrintProgress)){printExecution=run;try{output();}catch(NativePrintFinishException){}catch(NativePrintExitException){exited=true;}finally{pending.AddRange(run.Flow.TypeAhead);if(run.Flow.Disabled&&!NativePrintBehaviour.TemporaryGo)printJob.Desc=1;}}
   }catch(Exception error){failure=error;}finally{printExecution=null;FormClosing-=closing;canvas.Enabled=enabled;ContextMenuStrip.Enabled=context;functionStrip.Enabled=functions;canvas.Invalidate();}
   if(failure!=null){RefreshOriginalPrintMenu();throw failure;}
   mode=ScreenMode.Main;printStage=OriginalPrintStage.None;
   Action completed=()=>{UpdateClassic();LayoutClassic();Message(exited?"Printing exited.":"Output = "+(String.IsNullOrEmpty(printJob.Output)?"Laser":printJob.Output));command.Focus();};
   if(!BeginExternalDrawingRedraw(completed))completed();
   if(closeAfterPrint){closeAfterPrint=false;Close();return;}
   foreach(object value in pending){if(IsDisposed)break;if(value is char){char c=(char)value;if(c==13){string text=((NativeCommandBox)command).PhysicalText;RecordSharedLine(text);command.Clear();Safe(()=>Execute(text));}else ((NativeCommandBox)command).ReplayCharacter(c);}else{Keys key=(Keys)value;if(key==Keys.Enter){string text=((NativeCommandBox)command).PhysicalText;RecordSharedLine(text);command.Clear();Safe(()=>Execute(text));}else{var message=new Message();if(!ProcessCmdKey(ref message,key)){var e=new KeyEventArgs(key);OnKeyDown(e);if(!e.Handled&&!e.SuppressKeyPress)((NativeCommandBox)command).ReplayEditingKey(key);}}}}
  }
 }
}
