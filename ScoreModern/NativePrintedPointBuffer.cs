using System;using System.Collections.Generic;using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // Original drawing requests accumulate word-coordinate subpaths until the
 // per-item printer drain. Cached glyph requests print independently.
 // Ledger requests are retained separately until after cached text/glyph calls.
 internal sealed class NativePrintedPointBuffer:IDisposable {
  [ThreadStatic]static NativePrintedPointBuffer current;
  sealed class Run {public NativeNotationContext Page;public float Width,Rotation;public PointF Pivot;public bool Words,InheritedWidth;public List<BarlinePoint> Points=new List<BarlinePoint>();}
  readonly Graphics graphics;readonly List<Run> runs=new List<Run>();readonly List<Action> beforeDrain=new List<Action>();readonly NativePrintedPointBuffer previous;bool completed;
  NativePrintedPointBuffer(Graphics g){graphics=g;previous=current;current=this;}
  internal static NativePrintedPointBuffer Begin(Graphics g){return new NativePrintedPointBuffer(g);}
  internal static bool Append(Graphics g,NativeNotationContext page,IEnumerable<BarlinePoint> points,float width,bool words,PointF pivot,float rotation,bool inheritedWidth){
   var buffer=current;if(buffer==null||buffer.graphics!=g||NativeNotationView.UsesDat)return false;
   Run run=buffer.runs.Count==0?null:buffer.runs[buffer.runs.Count-1];
   if(run==null||run.Page.Projection!=page.Projection||(run.InheritedWidth?NativePrintContext.DefaultLineWidth:run.Width)!=(inheritedWidth?NativePrintContext.DefaultLineWidth:width)||run.Words!=words||run.Pivot!=pivot||run.Rotation!=rotation){run=new Run{Page=page,Width=width,Words=words,Pivot=pivot,Rotation=rotation,InheritedWidth=inheritedWidth};buffer.runs.Add(run);}
   run.InheritedWidth|=inheritedWidth;run.Points.AddRange(points);return true;
  }
  void Drain(){current=previous;try{foreach(var draw in beforeDrain)draw();beforeDrain.Clear();foreach(var run in runs)NativeNoteDetails.Path(graphics,run.Page,run.Points,run.InheritedWidth?NativePrintContext.DefaultLineWidth:run.Width,run.Words,run.Pivot,run.Rotation);runs.Clear();}finally{current=this;}}
  internal static void BeforeDrain(Action draw){if(current==null){Direct(draw);return;}current.beforeDrain.Add(draw);}
  internal static void Direct(Action draw){var saved=current;current=null;try{draw();}finally{current=saved;}}
  internal static void FlushPending(Graphics g){if(current!=null&&current.graphics==g)current.Drain();}
  internal void Complete(){if(completed)return;Drain();completed=true;current=previous;}
  public void Dispose(){if(!completed)current=previous;}
 }
}
