using System;using System.Collections.Generic;using System.Globalization;using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // Original 18B1:0335..06DB: sorted hundredth-position anchors, signed
 // remainder height controls, font 98, and the repeating units/tens labels.
 public sealed partial class Canvas {
  public readonly List<PrimitiveTextPoint> InputNumberPoints=new List<PrimitiveTextPoint>();
  NativeNotationContext inputNumberContext;
  public void SetInputNumbers(IList<float[]> records,int staff,bool visible){
   InputNumberPoints.Clear();inputNumberContext=null;
   if(!visible||PageDocument==null||TextLayout==null){Invalidate();return;}
   short height;var anchors=CollectInputNumberAnchors(records,staff,out height);
   float scale=PageDocument.Tables.Scales[staff],rawBase=PageDocument.Tables.Bases[staff];
   inputNumberContext=new NativeNotationContext{Staff=staff,RawBase=rawBase,Projection=PageDocument.Projection,Prepared=new PageItemContext{Scale=scale}};
   float y=inputNumberContext.Vertical(unchecked((short)(height+2))),size=(float)(20.0/scale);
   for(int i=0;i<anchors.Count;i++){
    int number=(i+1)%10;AddInputNumber(number,(float)(anchors[i]*1.5),y,size,staff);
    if(number==0)AddInputNumber(((i+1)/10)*10,(float)(anchors[i]*1.5),(float)((double)y+600),size,staff);
   }
   Invalidate();
  }
  public static List<short> CollectInputNumberAnchors(IList<float[]> records,int staff,out short height){
   var anchors=new List<short>();height=11;
   foreach(var p in records){if(p.Length<5||(short)p[0]!=1||(short)p[1]!=staff)continue;
    short h=(short)((short)p[3]%100),stem=(short)((short)p[4]%100);
    if(stem>=10&&stem<20)h=unchecked((short)(h+8));if(h>height)height=h;
    short x=unchecked((short)(int)((double)p[2]*100));
    if(anchors.Count>0&&unchecked((short)(anchors[anchors.Count-1]+1))<x){anchors.Add(x);continue;}
    int at=anchors.Count;
    while(at>0){short delta=unchecked((short)(x-anchors[at-1]));if(delta>1)break;if(delta>=-1){at=-1;break;}at--;}
    if(at>=0)anchors.Insert(at,x);
   }
   return anchors;
  }
  void AddInputNumber(int number,float x,float y,float size,int staff){
   var position=new NativeNotationContext{Staff=staff,RawBase=inputNumberContext.RawBase,Projection=PageDocument.Projection,Prepared=new PageItemContext{Scale=inputNumberContext.Scale,X=x,Y=y}};
   var body=Item.Make(16,staff,0,0,1,size,0,98,0,0,0,1,0);
   InputNumberPoints.AddRange(TextLayout.ScanStrokeText(body,number.ToString(CultureInfo.InvariantCulture),position,0));
  }
  void DrawInputNumbers(Graphics g){if(!NativeNotationView.IsPrinting&&NativeEpsExport.For(g)==null&&inputNumberContext!=null)NativeDatDrawing.Points(g,inputNumberContext,InputNumberPoints);}
  internal byte[] DosInputNumbers(byte[] plane){
   if(InputNumberPoints.Count==0||NativeNotationView.IsPrinting)return plane;
   var raster=new NativeDosRaster(NativeDosViewport.Divisor,NativeDosViewport.Divisor,NativeDosViewport.OriginX,NativeDosViewport.OriginY);
   Array.Copy(plane,raster.Plane,plane.Length);raster.Draw(InputNumberPoints);return raster.Plane;
  }
 }
 public sealed partial class Editor {
  void UpdateInputNumbers(){var tx=musicInput==null?null:musicInput.Transaction;
   bool visible=tx!=null&&tx.Stage>=InputStaffStage.Marks&&tx.Stage<=InputStaffStage.Slurs&&!musicInput.SaveQuestion&&!musicInput.FilenameQuestion;
   canvas.SetInputNumbers(tx==null?new List<float[]>():tx.WorkingRecords,tx==null?1:(int)tx.OriginalStaff,visible);
  }
 }
}

