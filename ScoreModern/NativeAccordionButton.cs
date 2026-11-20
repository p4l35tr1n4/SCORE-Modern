using System;
using System.Collections.Generic;
using System.Drawing;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeAccordionButton {
  sealed class Plan {
   public float X,Y,Size,DotSize;
   public float Thickness;
   public readonly List<PointF> Dots=new List<PointF>();
   public readonly List<PrimitiveTextPoint> Dividers=new List<PrimitiveTextPoint>();
  }
  static float F(double value){return (float)value;}
  // Code12 relative 0273:0A2E–100F. Dot digits select circles in
  // emission order. The printer handles short-increment circles immediately.
  static Plan Prepare(Item i,NativeNotationContext p){
   int divisions=(short)i.P(7);
   if(divisions==0)divisions=3;
   if(divisions<2||divisions>4)return null;
   float size=i.P(6)==0?1:i.P(6);
   if(Single.IsNaN(size)||Single.IsInfinity(size)||Math.Abs(size)>32767||Single.IsNaN(i.P(10))||Single.IsInfinity(i.P(10))||Math.Abs(i.P(10))>32767)return null;
   for(int n=11;n<=20;n++)if(n!=15&&i.P(n)!=0)return null;
   int vertical=(short)i.P(8),horizontal=(short)i.P(9);
   if(vertical<0||vertical>(divisions==4?9999:999)||horizontal<0||horizontal>999)return null;
   float x=F((double)p.Prepared.X+i.P(15)*150),y=p.Prepared.Y;
   var plan=new Plan{X=x,Y=y,Size=size,Thickness=i.P(10),DotSize=F((double)size*(divisions==2?1.8f:1.4f))};
   if(divisions==4){
    // 0273:0CE5–0DAE stores the initial position, the increment and
    // each subsequent dot position separately. Draw the stored rows bottom up.
    float position=F(i.P(4)-(double)size*2.95f),advance=F((double)size*1.98f);
    for(int place=1000;place>0;place/=10){
     if(vertical/place%10!=0)plan.Dots.Add(new PointF(x,p.Vertical(position)));
     position=F((double)position+advance);
    }
    float scaled=F((double)size*p.Scale/25);
    float horizontalOffset=F((double)scaled*2.45f*150);
    if(horizontal%10!=0)plan.Dots.Add(new PointF(F((double)x+horizontalOffset),y));
    if(horizontal/100!=0)plan.Dots.Add(new PointF(F((double)x-horizontalOffset),y));
    float middle=F((double)scaled*4.66f),outer=F((double)scaled*4.05f);
    float shift=F((double)size*1.95f);
    AddDivider(plan,x,y,middle);
    AddDivider(plan,x,p.Vertical(F((double)i.P(4)+shift)),outer);
    AddDivider(plan,x,p.Vertical(F((double)i.P(4)-shift)),outer);
    return plan;
   }
   float displacement=F((double)size*(divisions==2?2f:2.55f));
   if(vertical/(divisions==2?10:100)!=0)plan.Dots.Add(new PointF(x,p.Vertical(F((double)i.P(4)-displacement))));
   if(vertical%10!=0)plan.Dots.Add(new PointF(x,p.Vertical(F((double)i.P(4)+displacement))));
   if(divisions==3){
    bool middle=vertical%100/10!=0;
    if(middle||horizontal%100/10!=0)plan.Dots.Add(new PointF(x,y));
    float offset=F((double)(middle?2.875f:2.45f)*F((double)size*p.Scale/25)*150);
    if(horizontal%10!=0)plan.Dots.Add(new PointF(F((double)x+offset),y));
    if(horizontal/100!=0)plan.Dots.Add(new PointF(F((double)x-offset),y));
   }
   float half=F((double)F((double)size*p.Scale/25)*(divisions==2?4.66f:4.35f)*150);
   Action<float> line=yy=>{
    plan.Dividers.Add(new PrimitiveTextPoint{X=F((double)x-half),Y=yy,Operation=1});
    plan.Dividers.Add(new PrimitiveTextPoint{X=F((double)x+half),Y=yy});
   };
   if(divisions==2)line(y);
   else{
    float shift=F((double)size*1.35f);
    line(p.Vertical(F((double)i.P(4)+shift)));
    line(p.Vertical(F((double)i.P(4)-shift)));
   }
   return plan;
  }
  static void AddDivider(Plan plan,float x,float y,float half){
   plan.Dividers.Add(new PrimitiveTextPoint{X=F(x-(double)half*150),Y=y,Operation=1});
   plan.Dividers.Add(new PrimitiveTextPoint{X=F(x+(double)half*150),Y=y});
  }
  internal static List<PrimitiveTextPoint> Scan(Item i,NativeNotationContext p,NativeDatGlyphs dat){
   var plan=Prepare(i,p);
   if(plan==null)throw new NotSupportedException("Accordion controls outside the measured geometry.");
   float radius=F((double)plan.Size*8);
   var points=dat.Circle(p.Scale,plan.X,plan.Y,radius,radius,false,8);
   int copies=(int)F((double)plan.Thickness*p.Scale/25);
   if(copies>0){
    float outer=F((double)radius+7.142899990081787f*copies/p.Scale);
    points.AddRange(dat.Circle(p.Scale,plan.X,plan.Y,outer,outer,false,8));
   }
   points.AddRange(plan.Dividers);
   foreach(var dot in plan.Dots)points.AddRange(dat.Circle(p.Scale,dot.X,dot.Y,plan.DotSize,plan.DotSize,true,12));
   return points;
  }
  internal static bool Draw(Graphics g,Item i,NativeNotationContext p,DrawingLibrary library){
   if(library==null)return false;
   var plan=Prepare(i,p);
   if(plan==null)return false;
   NativePrintedShapes.DrawArc(g,p,plan.X,plan.Y,F((double)plan.Size*p.Scale*28),1,-270,90,NativePrintContext.Width(unchecked((short)(int)F((double)F(plan.Thickness+1)*NativePrintContext.Basis))),false,true,exactDecimal:true);
   var buffered=plan.Dividers.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation));
   float radius=F((double)plan.DotSize*p.Scale*3.5);
   for(int n=0;n<plan.Dots.Count;n++){
    var dot=plan.Dots[n];
    int step=1<<n;
    if(step<=5)NativePrintedShapes.DrawArc(g,p,dot.X,dot.Y,radius,1,-270,90,NativePrintContext.DefaultLineWidth,true,false,exactDecimal:true);
    else{
     var points=NativeDatDrawing.Get(library).Circle(p.Scale,dot.X,dot.Y,plan.DotSize,plan.DotSize,true,step);
     buffered.AddRange(points.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)));
    }
   }
   NativeNoteDetails.Path(g,p,buffered,NativePrintContext.DefaultLineWidth);
   return true;
  }
 }
}
