using System;using System.Drawing;using System.Drawing.Drawing2D;using System.Collections.Generic;using System.Globalization;
namespace ScoreModern {
 public static class NativePrintedRepeatBars {
  static float F(double v){return (float)v;}static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static void Dot(Graphics g,NativeNotationContext page,float scale,float x,float y){
   float radius=F((double)scale*3.5*.5799999833106995f);string sx=x.ToString("F1",CultureInfo.InvariantCulture),sy=y.ToString("F1",CultureInfo.InvariantCulture),sr=radius.ToString("F1",CultureInfo.InvariantCulture);
   float xx=Single.Parse(sx,CultureInfo.InvariantCulture),yy=Single.Parse(sy,CultureInfo.InvariantCulture),rr=Single.Parse(sr,CultureInfo.InvariantCulture);
   var state=g.Save();try{using(var matrix=page.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);using(var path=new GraphicsPath(FillMode.Alternate)){path.AddEllipse(xx-rr,yy-rr,rr*2,rr*2);g.TrackedFillPath(Brushes.Black,path);var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScoreRepeatDot(sx,sy,sr);}}finally{g.Restore(state);}
  }
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,NativePageDocument doc){
   if(item.Code!=14||page==null||doc==null||item.P(4)<0||item.P(4)!=(int)item.P(4))return false;
   int kind=(int)item.P(5);if(item.P(5)!=kind&&item.P(5)!=998.75f&&!NativeDatBarlines.DefaultCenteredDash(item)||kind<3||kind==8||kind==9||kind==10)return false;
   bool dashed=kind>=7;
   for(int n=6;n<=20;n++)if(Single.IsNaN(item.P(n))||Single.IsInfinity(item.P(n)))return false;
   for(int n=6;n<=20;n++)if(n!=6&&n!=7&&n!=10&&n!=11&&n!=12&&n!=15&&(dashed?(n!=8&&n!=9&&n!=17):(n!=13&&n!=14&&n!=16&&n!=17))&&item.P(n)!=0)return false;
   if(item.P(12)!=0&&item.P(12)!=1)return false;
   if(dashed&&(item.P(6)<short.MinValue||item.P(6)>short.MaxValue||Math.Abs(item.P(8))>32767||Math.Abs(item.P(9))>32767))return false;
   if(!dashed&&(item.P(16)<short.MinValue||item.P(16)>short.MaxValue))return false;
   int high=page.Staff+Math.Max(1,(int)item.P(4))-1;if(high>doc.MaximumDefinedStaff||high>32)return false;
   float scale=page.Scale,x=NativePrintedBarlines.HorizontalOrigin(item,page),bottom=doc.Tables.Vertical(page.Staff,item.P(10)==0?(kind>=11?1:3):item.P(10)),top=doc.Tables.Vertical(high,item.P(11)==0?(kind>=11?13:11):item.P(11));var points=new List<BarlinePoint>();
   if(dashed){
    if(kind>=999){
     if(!NativeDatBarlines.DefaultCenteredDash(item))return false;
     foreach(var q in NativeDatBarlines.CenteredDash(x,bottom,scale))points.Add(new BarlinePoint(q.X,q.Y,q.Operation));
    }else{
     var body=Item.Make(4,item.P(2),item.P(3),3,F(((double)top-page.RawBase)/(scale*7.0)),item.P(3),1,Present(item.P(8))?item.P(8):1,Present(item.P(9))?item.P(9):2);
     var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};
     context.Prepared.X=x;context.Prepared.Y=bottom;
     foreach(var q in NativeDatLines.Dashes(body,context))points.Add(new BarlinePoint(q.X,q.Y,q.Operation));
    }
    short widthWord=(short)(int)Math.Truncate(item.P(6));if(widthWord==0)widthWord=1;
    NativeNoteDetails.Path(g,page,points,NativePrintContext.Width(unchecked((short)(2*widthWord))),true);
    RenderResourceAudit.Record("printed dashed bar","7","original shared vertical line supplier","original barline controls and printer thickness selector");return true;
   }
   if(Present(item.P(17)))scale=F((double)item.P(17)*25);
   float unit=F((double)scale*6),spacing=F((double)scale*6*item.P(13)),dotExtra=F((double)item.P(14)*unit);
   Action<float> line=xx=>{points.Add(new BarlinePoint(xx,bottom,1));points.Add(new BarlinePoint(xx,top,0));float next=F((double)xx+NativePrintContext.Unit);points.Add(new BarlinePoint(next,bottom,1));points.Add(new BarlinePoint(next,top,0));};
   Action<float> heavy=right=>{float left=F((double)right-scale*6);points.Add(new BarlinePoint(left,bottom,1));points.Add(new BarlinePoint(right,top,0));points.Add(new BarlinePoint(right,top,2));points.Add(new BarlinePoint(right,bottom,0));points.Add(new BarlinePoint(left,bottom,0));points.Add(new BarlinePoint(left,top,0));points.Add(new BarlinePoint(right,top,0));};
   int packed=(short)(int)Math.Truncate(item.P(16));bool explicitDots=Present(item.P(16));int lower=explicitDots?packed/100:6,delta=explicitDots?packed%100-lower:2;if(delta==0)delta=2;
   Action<float,float> dots=(origin,offset)=>{
    float xx=F((double)origin+offset*scale);
    for(int staff=page.Staff;staff<=high;staff++){
     float local=doc.Tables.Scales[staff],yy=F((double)doc.Tables.Bases[staff]+lower*7*(double)local);
     Dot(g,page,local,xx,yy);
     Dot(g,page,local,xx,F((double)yy+delta*7*(double)local));
    }
   };
   float rightOrigin=F((double)x+spacing+dotExtra);
   if(kind==4||kind==5){line(F((double)scale*7.800000190734863+x+spacing));dots(rightOrigin,19.5f);}
   if(kind==3||kind==5){
    float leftOrigin=kind==5?F((double)F((double)rightOrigin-spacing-dotExtra)-spacing-dotExtra):F((double)x-spacing-dotExtra);
    dots(leftOrigin,-25.5f);
    line(F((double)x-scale*13.800000190734863-spacing));
   }
   if(kind==6){
    float leftOrigin=F((double)x-dotExtra);
    dots(leftOrigin,-18.360000610351562f);
    dots(F((double)leftOrigin+spacing+dotExtra+dotExtra),25.200000762939453f);
   }
   heavy(x);
   if(kind==6)heavy(F((double)scale*13+x+spacing));
   RenderResourceAudit.Record("printed repeat bar",kind.ToString(),"original Code14 geometry","default repeat construction; printed circles and printer-unit thin lines");NativeNoteDetails.Path(g,page,points,NativePrintContext.DefaultLineWidth);return true;
  }
 }
}

