using System;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using System.Drawing.Imaging;using System.Runtime.CompilerServices;using System.Runtime.InteropServices;
namespace ScoreModern {
 // Observe the paths/images actually drawn; selection anchors are independent.
 public static class NativeItemBounds {
  sealed class Capture:IDisposable {public Canvas Canvas;public Item Item;public readonly Dictionary<Graphics,Matrix> Bases=new Dictionary<Graphics,Matrix>();public void Dispose(){foreach(var m in Bases.Values)m.Dispose();current=null;}}
  [ThreadStatic]static Capture current;
  sealed class InkBox{public Rectangle Bounds;}
  static readonly ConditionalWeakTable<Image,InkBox> Images=new ConditionalWeakTable<Image,InkBox>();
  sealed class Suspended:IDisposable {readonly Capture before;internal Suspended(){before=current;current=null;}public void Dispose(){current=before;}}
  internal static IDisposable Suspend(){return new Suspended();}
  public static bool Active{get{return current!=null;}}
  public static IDisposable Begin(Canvas canvas,bool force=false){if(!force&&!NativeMouseSelector.ShowPoints)return null;if(current!=null)throw new InvalidOperationException("Nested hover-bound capture.");current=new Capture{Canvas=canvas};if(!NativeNotationView.IsDosViewport)canvas.ItemBounds.Clear();return current;}
  public static void Surface(Graphics g){if(current==null||current.Bases.ContainsKey(g))return;var matrix=g.Transform;matrix.Invert();current.Bases[g]=matrix;}
  public static void Select(Item item){if(current!=null)current.Item=item;}
  static void Add(RectangleF box){if(current==null||current.Item==null||box.Width<=0||box.Height<=0)return;RectangleF old;current.Canvas.ItemBounds[current.Item]=current.Canvas.ItemBounds.TryGetValue(current.Item,out old)?RectangleF.Union(old,box):box;}
  public static void Path(Graphics g,GraphicsPath path,Pen pen){if(current==null||current.Item==null||path.PointCount==0)return;Matrix basis;if(!current.Bases.TryGetValue(g,out basis))return;using(var transform=g.Transform)using(var shape=(GraphicsPath)path.Clone()){transform.Multiply(basis,MatrixOrder.Append);if(pen!=null&&pen.Width>0)shape.Widen(pen);shape.Transform(transform);var box=shape.GetBounds();if(pen!=null&&pen.Width==0)box.Inflate(.5f/current.Canvas.RenderScale,.5f/current.Canvas.RenderScale);Add(box);}}
  public static void TrackedDrawPath(this Graphics g,Pen pen,GraphicsPath path){NativeItemPaint.Path(g,path,pen,(copy,p)=>g.DrawPath(p,copy));Path(g,path,pen);}
  public static void TrackedFillPath(this Graphics g,Brush brush,GraphicsPath path){NativeItemPaint.Path(g,path,brush,(copy,b)=>g.FillPath(b,copy));Path(g,path,null);}
  static InkBox Scan(Image image){var bitmap=image as Bitmap;if(bitmap==null)return new InkBox();int left=bitmap.Width,top=bitmap.Height,right=-1,bottom=-1;var bits=bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);try{byte[] row=new byte[bitmap.Width*4];for(int y=0;y<bitmap.Height;y++){Marshal.Copy(IntPtr.Add(bits.Scan0,y*bits.Stride),row,0,row.Length);for(int x=0;x<bitmap.Width;x++)if(row[x*4+3]!=0){left=Math.Min(left,x);right=Math.Max(right,x);top=Math.Min(top,y);bottom=Math.Max(bottom,y);}}}finally{bitmap.UnlockBits(bits);}return new InkBox{Bounds=right<0?Rectangle.Empty:Rectangle.FromLTRB(left,top,right+1,bottom+1)};}
  internal static Rectangle ObservedInk(Image image){return Images.GetValue(image,Scan).Bounds;}
  internal static void DeviceRectangle(Graphics g,Rectangle ink,int x,int y){if(ink.IsEmpty)return;using(var path=new GraphicsPath()){path.AddRectangle(new Rectangle(x+ink.X,y+ink.Y,ink.Width,ink.Height));Path(g,path,null);}}
  public static void Image(Graphics g,Image image,int x,int y){if(current==null||current.Item==null)return;var ink=Images.GetValue(image,Scan).Bounds;if(ink.IsEmpty)return;using(var path=new GraphicsPath()){path.AddRectangle(new Rectangle(x+ink.X,y+ink.Y,ink.Width,ink.Height));Path(g,path,null);}}
  public static RectangleF DosPlane(byte[] plane){int left=640,top=480,right=-1,bottom=-1;for(int y=0;y<462;y++)for(int b=0;b<80;b++){byte value=plane[y*80+b];if(value==0)continue;for(int bit=0;bit<8;bit++)if((value&(128>>bit))!=0){int x=b*8+bit;left=Math.Min(left,x);right=Math.Max(right,x);top=Math.Min(top,y);bottom=Math.Max(bottom,y);}}return right<0?RectangleF.Empty:RectangleF.FromLTRB(left,top-97,right+1,bottom+1-97);}
 }
}
