using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatBarlines {
  static float F(double x){return (float)x;}static short W(float x){return unchecked((short)(int)Math.Truncate(x));}static bool Present(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0)!=0;}
  // Original Code14 working-word dispatch; DAT rules remain separate from printed preparation.
  // P6 reads the mutable external unit field, including its retained post-print value.
  public static List<PrimitiveTextPoint> Prepare(Item source,NativePageDocument doc,DrawingLibrary library){
   return Prepare(source,doc,library,false);
  }
  internal static List<PrimitiveTextPoint> Prepare(Item source,NativePageDocument doc,DrawingLibrary library,bool drawingVectors){
   for(int n=3;n<=20;n++)if(Single.IsNaN(source.P(n))||Single.IsInfinity(source.P(n))||Math.Abs(source.P(n))>32767)throw new NotSupportedException("DAT barline outside the finite word domain.");
   for(int n=18;n<=20;n++)if(source.P(n)!=0)throw new NotSupportedException("DAT barline extended controls need verification.");
   var page=doc.Contexts[source];int span=W(source.P(4)),kind=W(source.P(5));bool hidden=span<0;if(hidden){span=-span;kind=7;}if(span==0)span=1;int high=Math.Min(32,page.Staff+span-1);if(high<page.Staff)throw new NotSupportedException("DAT barline staff bounds need verification.");
   float scale=page.Scale,x=F((double)page.Prepared.X+(Present(source.P(15))?source.P(15):source.P(7))*150),lo=Present(source.P(10))?source.P(10):kind>=9?(kind==10?7:1):3,hi=Present(source.P(11))?source.P(11):kind>=9?(kind==10?7:13):11;
   float bottom=doc.Tables.Vertical(page.Staff,lo),top=doc.Tables.Vertical(high,hi);var points=new List<PrimitiveTextPoint>();Action<float,float,short> add=(xx,yy,op)=>points.Add(new PrimitiveTextPoint{X=xx,Y=yy,Operation=op});
   Action<float,double> heavy=(right,width)=>{float left=F((double)right-width);add(left,bottom,1);add(right,top,0);add(right,top,2);add(right,bottom,0);add(left,bottom,0);add(left,top,0);add(right,top,0);};
   if(kind==0){if(!Present(source.P(6))){add(x,bottom,1);add(x,top,0);}else heavy(x,((double)source.P(6)+1)*doc.DatBarlineUnit);return points;}
   if(kind==-1){heavy(x,scale*6.0);return points;}
   // The shared line supplier compares the original P5 word against 999.
   // At this boundary it emits a three-point path around the lower endpoint.
   if(kind>=999){
    if(!DefaultCenteredDash(source))throw new NotSupportedException("DAT high barline controls need verification.");
    points.AddRange(CenteredDash(x,bottom,scale));
    return points;
   }
   if(kind==7||kind>=11){
    short width=W(source.P(6));
    // SH sends twice the requested dash width to the vector copier.
    if(drawingVectors&&!hidden)width=width==0?(short)2:unchecked((short)(width*2));
    float pitch=F(((double)top-doc.Tables.Bases[page.Staff])/(scale*7.0));var body=Item.Make(4,source.P(2),source.P(3),3,pitch,source.P(3),1,hidden?.10000000149011612f:Present(source.P(8))?source.P(8):1,hidden?1.5f:Present(source.P(9))?source.P(9):2,width);
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=doc.Tables.Bases[page.Staff],StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=x;context.Prepared.Y=bottom;return NativeDatLines.Dashes(body,context);
   }
   if(kind==8){if(library==null)throw new NotSupportedException("DAT brace needs external DAT resources.");float v=F(((double)top-bottom)*.005494500044733286/scale),h=Present(source.P(6))?source.P(6):F(((double)v-1)/2+1);return new NativeDatGlyphs(library.Root,drawingVectors).Expand(5,h,v,scale,x,bottom);}
   if(kind>=1&&kind<=6){
    float drawScale=Present(source.P(17))?F((double)source.P(17)*25):scale,unit=F((double)drawScale*6),spacing=F((double)drawScale*6*source.P(13)),dotExtra=F((double)source.P(14)*unit);
    Action<float> line=xx=>{add(xx,bottom,1);add(xx,top,0);};
    Action<float,float> dots=(origin,shift)=>{float xx=F((double)shift*drawScale+origin);int packed=W(source.P(16)),lower=!Present(source.P(16))?6:packed/100,delta=!Present(source.P(16))?2:packed%100-lower;if(delta==0)delta=2;
     for(int staff=page.Staff;staff<=high;staff++){float local=doc.Tables.Scales[staff],yy=F((double)doc.Tables.Bases[staff]+unchecked((short)(lower*7))*(double)local);points.AddRange(new NativeDatGlyphs(library.Root).RepeatDot(local,xx,yy));yy=F((double)yy+unchecked((short)(delta*7))*(double)local);points.AddRange(new NativeDatGlyphs(library.Root).RepeatDot(local,xx,yy));}};
    if(kind==1){line(x);line(F((double)x-drawScale*7.5-spacing));return points;}
    if(kind==2){line(F((double)x-drawScale*13.800000190734863-spacing));heavy(x,(double)drawScale*6);return points;}
    if(library==null)throw new NotSupportedException("DAT repeat dots need external DAT resources.");
    float rightOrigin=F((double)x+spacing+dotExtra);
    if(kind==4||kind==5){line(F((double)drawScale*7.800000190734863+x+spacing));dots(rightOrigin,19.5f);}
    if(kind==3||kind==5){float leftOrigin=kind==5?F((double)F((double)rightOrigin-spacing-dotExtra)-spacing-dotExtra):F((double)x-spacing-dotExtra);dots(leftOrigin,-25.5f);line(F((double)x-drawScale*13.800000190734863-spacing));}
    if(kind==6){float leftOrigin=F((double)x-dotExtra);dots(leftOrigin,-18.360000610351562f);dots(F((double)leftOrigin+spacing+dotExtra+dotExtra),25.200000762939453f);}
    heavy(x,(double)drawScale*6);if(kind==6)heavy(F((double)drawScale*13+x+spacing),(double)drawScale*6);return points;
   }
   if(kind==9||kind==10){
    var clone=source.Clone();clone.Set(4,high-page.Staff+1);clone.Set(5,kind);clone.Set(6,0);clone.Set(8,W(source.P(8)));clone.Set(7,0);clone.Set(9,0);for(int n=12;n<=20;n++)clone.Set(n,0);var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=x;if(Present(source.P(17)))context.Prepared.Scale=F((double)source.P(17)*25);var plan=NativePrintedBarlines.Prepare(clone,context,doc,true);if(plan==null)throw new NotSupportedException("DAT bracket controls need verification.");
    foreach(var point in plan.Points)add(point.X,point.Y,(short)point.Operation);return points;
   }
   throw new NotSupportedException("DAT barline type needs verification.");
  }
  internal static IEnumerable<PrimitiveTextPoint> CenteredDash(float x,float bottom,float scale){
   float half=F(2.299999952316284f*(double)scale*3.5);
   short xx=NativeNotationContext.Word(x,true);
   yield return new PrimitiveTextPoint{X=xx,Y=NativeNotationContext.Word(F((double)bottom+half),false),Operation=1};
   yield return new PrimitiveTextPoint{X=xx,Y=NativeNotationContext.Word(bottom,false),Operation=0};
   yield return new PrimitiveTextPoint{X=xx,Y=NativeNotationContext.Word(F((double)bottom-half),false),Operation=0};
  }
  internal static bool DefaultCenteredDash(Item item){
   float style=item.P(5);
   return item.Staff==1&&(item.P(4)==1||item.P(4)==2)&&item.Count==5&&
    style>=999&&style<=32766;
  }
 }
}
