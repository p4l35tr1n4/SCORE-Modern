using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScoreModern {
 internal sealed class NativeItemPaint : IDisposable {
  [ThreadStatic] static NativeItemPaint current;
  readonly Graphics target;
  readonly List<Action> replay=new List<Action>();
  readonly List<IDisposable> resources=new List<IDisposable>();

  internal NativeItemPaint(Graphics target){
   if(current!=null)throw new InvalidOperationException("Nested engraving item.");
   this.target=target;
   current=this;
  }
  static NativeItemPaint For(Graphics g){return current!=null&&current.target==g?current:null;}
  T Own<T>(T value) where T:IDisposable {resources.Add(value);return value;}
  sealed class PaintState {
   internal Matrix Transform;
   internal Region Clip;
   internal GraphicsUnit Unit;
   internal float Scale;
   internal SmoothingMode Smoothing;
   internal PixelOffsetMode Offset;
   internal InterpolationMode Interpolation;
   internal CompositingMode Compositing;
   internal CompositingQuality Quality;
  }
  PaintState last;
  void Add(Action paint){
   var unit=target.PageUnit;
   var scale=target.PageScale;
   var smoothing=target.SmoothingMode;
   var offset=target.PixelOffsetMode;
   var interpolation=target.InterpolationMode;
   var compositing=target.CompositingMode;
   var quality=target.CompositingQuality;
   var transform=target.Transform;
   var clip=target.Clip;
   if(last!=null&&unit==last.Unit&&scale==last.Scale&&smoothing==last.Smoothing&&
      offset==last.Offset&&interpolation==last.Interpolation&&compositing==last.Compositing&&
      quality==last.Quality&&transform.Equals(last.Transform)&&clip.Equals(last.Clip,target)){
    transform.Dispose();
    clip.Dispose();
   }else{
    var state=new PaintState{Transform=Own(transform),Clip=Own(clip),Unit=unit,Scale=scale,
     Smoothing=smoothing,Offset=offset,Interpolation=interpolation,Compositing=compositing,Quality=quality};
    last=state;
    replay.Add(()=>{
     target.PageUnit=state.Unit;target.PageScale=state.Scale;
     target.Transform=state.Transform;target.Clip=state.Clip;
     target.SmoothingMode=state.Smoothing;target.PixelOffsetMode=state.Offset;
     target.InterpolationMode=state.Interpolation;target.CompositingMode=state.Compositing;
     target.CompositingQuality=state.Quality;
    });
   }
   replay.Add(paint);
  }
  internal static void Paint<T>(Graphics g,T resource,Action<T> paint) where T:class,ICloneable,IDisposable {
   var stage=For(g);
   if(stage==null){paint(resource);return;}
   var copy=stage.Own((T)resource.Clone());
   stage.Add(()=>paint(copy));
  }
  internal static void Path<T>(Graphics g,GraphicsPath path,T resource,Action<GraphicsPath,T> paint)
   where T:class,ICloneable,IDisposable {
   var stage=For(g);
   var copy=stage==null?path:stage.Own((GraphicsPath)path.Clone());
   Paint(g,resource,value=>paint(copy,value));
  }
  internal void Commit(){
   if(replay.Count==0)return;
   var state=target.Save();
   try{foreach(var paint in replay)paint();}
   finally{target.Restore(state);}
  }
  public void Dispose(){current=null;foreach(var resource in resources)resource.Dispose();}
 }
}
