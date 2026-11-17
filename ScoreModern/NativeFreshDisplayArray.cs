using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // Explicit first-frame model: the measured original startup Y allocation is
 // zero. Cached-head copies carry otherwise-unused marker Y words too.
 // This is not a model of arbitrary retained redraw/edit/allocator history.
 public sealed class NativeFreshDisplayArray {
  public short[] X,Y;public int[] Pointers;
  public static NativeFreshDisplayArray Prepare(Canvas canvas){
   if(canvas==null||canvas.Score==null)throw new ArgumentNullException("canvas");
   var prior=NativeNotationView.Mode;var xs=new List<short>();var ys=new List<short>();var pointers=new List<int>();
   using(canvas.TextLayout.PreserveAlignmentMetricHistory())try{
    NativeNotationView.Set(NotationView.Dat,false);canvas.LayoutScore();var dat=new NativeDatGlyphs(canvas.Library.Root);var cache=new NativeDatHeadCache(true);short font=0;
    foreach(var item in canvas.Score.Items){pointers.Add(xs.Count+1);short next;var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,cache,font,out next);font=next;
     if(item.Code==8){if(item.Staff==1)points.InsertRange(0,NativeScanStructuralVectors.ScreenGuide(item,canvas.PageDocument));if(!canvas.TextLayout.ShowActualDatFonts)foreach(var number in NativeDosOverlays.Numbers(canvas.Score,canvas.PageDocument,canvas.TextLayout,canvas.DisappearingStaffs))if(number.Staff==item.Staff)points.AddRange(number.Points);}
     foreach(var p in points){if(p.Operation>0||p.CommandOnly){xs.Add(unchecked((short)(p.Operation-32700)));ys.Add(p.RetainedMarkerY??0);}if(!p.CommandOnly){xs.Add(NativeNotationContext.Word(p.X,true));ys.Add(NativeNotationContext.Word(p.Y,false));}if(xs.Count>31000)throw new NotSupportedException("Fresh display array exceeds the original vector capacity.");}
    }
   }finally{NativeNotationView.Set(prior,false);}
   pointers.Add(xs.Count+1);var x=new short[xs.Count+1];x[0]=checked((short)x.Length);xs.CopyTo(x,1);return new NativeFreshDisplayArray{X=x,Y=ys.ToArray(),Pointers=pointers.ToArray()};
  }
 }
}
