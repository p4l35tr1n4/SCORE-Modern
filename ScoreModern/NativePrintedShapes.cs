using System;using System.Drawing;using System.Drawing.Drawing2D;using System.Collections.Generic;using System.Globalization;
namespace ScoreModern {
 public static class NativePrintedShapes {
  static float F(double v){return (float)v;}
  static bool Finite(float v){return !Single.IsNaN(v)&&!Single.IsInfinity(v);}
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library){
   if(page==null||item.Code!=12)return false;
   if(item.P(5)==10)return NativeAccordionButton.Draw(g,item,page,library);
   if(item.P(5)==2)return NativeGuitarBend.Draw(g,item,page,library);
   if(item.P(5)!=0&&item.P(5)!=1)return false;
   for(int n=6;n<=13;n++)if(!Finite(item.P(n)))return false;
   for(int n=14;n<=20;n++)if(item.P(n)!=0)return false;
   bool circle=item.P(5)==1;
   float horizontal=item.P(6),vertical=item.P(7)==0?horizontal:item.P(7),thickness=item.P(8),rotation=item.P(9);
   if(horizontal==0||vertical==0||Math.Abs(thickness)>32767||Math.Abs(rotation)>32767)return false;
   if(item.P(13)!=0&&item.P(13)!=1)return false;
   bool filled=item.P(13)==1;
   float width=NativePrintContext.DefaultLineWidth;
   // Original width selector truncates P8*printer basis before width clamping.
   if(thickness!=0)width=NativePrintContext.FixedWidth(unchecked((short)(int)F((double)thickness*NativePrintContext.Basis)));
   var pivot=new PointF(NativeNotationContext.Word(page.Prepared.X,true),NativeNotationContext.Word(page.Prepared.Y,false));
   rotation=(int)rotation==0?0:F(Math.Round(-rotation,2,MidpointRounding.AwayFromZero));
   List<BarlinePoint> points;
   if(circle){
    int step=(int)item.P(12),start=(int)item.P(10),stop=item.P(11)==0?360:(int)item.P(11);
    if(start<0)start+=360;
    if(library==null||step<0||step>360||start<0||start>360||stop<0||stop>360)return false;
    if(step==0){
     if(stop<start)stop+=360;
     float radius=F((double)page.Scale*3.5*horizontal),ratio=F((double)vertical/horizontal);
     // SCORE measures arc endpoints clockwise from the top. Its default
     // printer path fills P13 arcs without adding an outline stroke.
     DrawArc(g,page,page.Prepared.X,page.Prepared.Y,radius,ratio,90-stop,90-start,width,filled,!filled,pivot,rotation);
     if(thickness!=0)NativePrintContext.RestoreDefaultLineWidth();
     return true;
    }
    if(stop<start)stop+=360;
    points=NativeDatDrawing.Get(library).ShapeCircle(page.Scale,page.Prepared.X,page.Prepared.Y,horizontal,vertical,filled,step,start,stop)
     .ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation));
   }else{
    if(item.P(11)!=0||item.P(12)!=0||Math.Abs(item.P(10))>32767)return false;
    float factor=F(page.Scale*3.5),dx=F((double)factor*horizontal),dy=F((double)factor*vertical);
    float left=F((double)page.Prepared.X-dx),right=F((double)page.Prepared.X+dx);
    float bottom=F((double)page.Prepared.Y-dy),top=F((double)page.Prepared.Y+dy);
    // Direct Code12 tests the cached signed word before applying raw shear.
    // Fractional magnitudes below one are ignored; harp delegation differs.
    float shear=(short)(int)item.P(10)!=0?F((double)page.Scale*item.P(10)*7):0;
    points=new List<BarlinePoint>{
     new BarlinePoint(left,bottom,filled?2:1),new BarlinePoint(right,bottom,0),
     new BarlinePoint(F((double)right+shear),top,0),new BarlinePoint(F((double)left+shear),top,0),
     new BarlinePoint(left,bottom,0),new BarlinePoint(right,bottom,0)
    };
   }
   RenderResourceAudit.Record("printed shape",circle?"circle or arc":"centred rectangle","original Code12 print path","SCORE coordinates, fill and printer rotation");
   NativeNoteDetails.Path(g,page,points,width,false,pivot,rotation);
   if(thickness!=0)NativePrintContext.RestoreDefaultLineWidth();
   return true;
  }
  internal static void DrawArc(Graphics g,NativeNotationContext page,float x,float y,float radius,float ratio,int start,int stop,float width,bool fill,bool stroke,PointF pivot=default(PointF),float rotation=0,bool exactDecimal=false){
   var culture=CultureInfo.InvariantCulture;
   Func<float,string> coordinate=value=>exactDecimal?ScoreModern.RecoveredFragments.SingleFixedDecimal.Format(value,1):value.ToString("F1",culture);
   string sx=coordinate(x),sy=coordinate(F((double)y/ratio)),sr=coordinate(radius);
   string sv=ratio.ToString("F5",culture),si=F(1.0/ratio).ToString("F5",culture);
   float xx=Single.Parse(sx,culture),yy=Single.Parse(sy,culture),rr=Single.Parse(sr,culture);
   var state=g.Save();
   try{
    using(var projection=page.Projection.Matrix())g.MultiplyTransform(projection,MatrixOrder.Prepend);
    if(rotation!=0){g.TranslateTransform(pivot.X,pivot.Y);g.RotateTransform(rotation);g.TranslateTransform(-pivot.X,-pivot.Y);}
    using(var path=new GraphicsPath(FillMode.Alternate))using(var pen=new Pen(Color.Black,width)){
     float magnitude=Math.Abs(rr);if(stop!=start)path.AddArc(xx-magnitude,yy-magnitude,magnitude*2,magnitude*2,start+(rr<0?180:0),stop-start);
     using(var scale=new Matrix(1,0,0,Single.Parse(sv,culture),0,0))path.Transform(scale);
     if(fill&&path.PointCount>0)g.TrackedFillPath(Brushes.Black,path);
     if(stroke&&path.PointCount>0)g.TrackedDrawPath(pen,path);
    }
   }finally{g.Restore(state);}
   var eps=NativeEpsExport.For(g);
   if(eps!=null)eps.ScoreArc(sx,sy,sr,sv,si,start,stop,width,fill,stroke,pivot,rotation);
  }
 }
}

