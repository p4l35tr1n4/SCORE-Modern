using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeScanStructuralVectors {
  // Bounded adapters to already recovered staff/barline geometry. The scan's
  // ordinary barline path uses one centre line; the printed adapter adds its
  // second line for EPS thickness. Source parameters stay separate from the
  // caller's carried working P20.
  static float F(double value){return (float)value;}
  public static List<PrimitiveTextPoint> Repeat(Item item,NativePageDocument doc,NativeDatGlyphs dat){
   if(item==null||doc==null||dat==null||item.Code!=14||item.P(5)<3||item.P(5)>6||item.P(5)!=(int)item.P(5)||item.P(4)<1||item.P(4)!=(int)item.P(4))throw new NotSupportedException("Requires a recovered default repeat barline.");
   for(int n=6;n<=20;n++)if(item.P(n)!=0&&!(n==12&&item.P(n)==1))throw new NotSupportedException("Repeat-barline auxiliary control is not yet verified for DAT display.");
   var page=doc.Contexts[item];int high=page.Staff+(int)item.P(4)-1,kind=(int)item.P(5);if(high>doc.MaximumDefinedStaff)throw new NotSupportedException("Repeat-barline span exceeds the defined staves.");float scale=page.Scale,x=page.Prepared.X,bottom=doc.Tables.Vertical(page.Staff,3),top=doc.Tables.Vertical(high,11),six=F(scale*6.0);
   var points=new List<PrimitiveTextPoint>();Action<float> line=xx=>{points.Add(new PrimitiveTextPoint{X=xx,Y=bottom,Operation=1});points.Add(new PrimitiveTextPoint{X=xx,Y=top});};
   Action<float> dots=xx=>{for(int staff=page.Staff;staff<=high;staff++){float localScale=doc.Tables.Scales[staff],y=F((double)doc.Tables.Bases[staff]+42*localScale);points.AddRange(dat.RepeatDot(localScale,xx,y));y=F((double)y+14*localScale);points.AddRange(dat.RepeatDot(localScale,xx,y));}};
   Action<float> heavy=xx=>{float left=F((double)xx-six);points.Add(new PrimitiveTextPoint{X=left,Y=bottom,Operation=1});points.Add(new PrimitiveTextPoint{X=xx,Y=top});points.Add(new PrimitiveTextPoint{X=xx,Y=top,Operation=2});points.Add(new PrimitiveTextPoint{X=xx,Y=bottom});points.Add(new PrimitiveTextPoint{X=left,Y=bottom});points.Add(new PrimitiveTextPoint{X=left,Y=top});points.Add(new PrimitiveTextPoint{X=xx,Y=top});};
   if(kind==4||kind==5){line(F((double)x+scale*7.800000190734863));dots(F((double)x+19.5*scale));}
   if(kind==3||kind==5){dots(F((double)x-25.5*scale));line(F((double)x-scale*13.800000190734863));}
   if(kind==6){dots(F((double)x+scale*-18.360000610351562));dots(F((double)x+scale*25.200000762939453));}
   heavy(x);if(kind==6)heavy(F((double)x+scale*13));return points;
  }
  // P8 tile guide. The default F6 caller clears working P8 for staff 1;
  // screen preparation retains it. Other staffs retain it in the tested F6 path.
  public static List<PrimitiveTextPoint> ScreenGuide(Item source,NativePageDocument doc){
   var points=new List<PrimitiveTextPoint>();PageStaffResult staff;
   if(source.Code==8&&BitConverter.ToInt32(BitConverter.GetBytes(source.P(8)),0)!=0&&doc.StaffDrawings.TryGetValue(source,out staff)){
    if(!doc.GuideMeasurement.HasValue)throw new NotSupportedException("Tile guide measurement code is not supported.");float unit=doc.GuideMeasurement.Value==1?1574.800048828125f:4000f;float y=F((double)source.P(8)*unit+staff.Bottom);
    points.Add(new PrimitiveTextPoint{X=doc.Contexts[source].Prepared.X,Y=y,Operation=1});points.Add(new PrimitiveTextPoint{X=30000,Y=y});
   }return points;
  }
  public static List<PrimitiveTextPoint> Prepare(Item source,NativePageDocument doc){return Prepare(source,doc,null);}
  public static List<PrimitiveTextPoint> Prepare(Item source,NativePageDocument doc,DrawingLibrary library){
   if(source==null||doc==null)throw new ArgumentNullException();var points=new List<PrimitiveTextPoint>();
   if(source.Code==8){PageStaffResult staff;if(!doc.StaffDrawings.TryGetValue(source,out staff))throw new NotSupportedException("Staff plan unavailable.");foreach(var line in staff.Lines){points.Add(new PrimitiveTextPoint{X=line.Left,Y=line.Y,Operation=1});points.Add(new PrimitiveTextPoint{X=line.Right,Y=line.Y});}return points;}
   if(source.Code==14)return NativeDatBarlines.Prepare(source,doc,library);
   throw new NotSupportedException("Requires a recovered staff, ordinary barline or bracket scan plan.");
  }
 }
}
