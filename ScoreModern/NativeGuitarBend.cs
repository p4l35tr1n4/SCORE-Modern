using System;
using System.Collections.Generic;
using System.Drawing;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeGuitarBend {
  // Original Code12 relative 0273:0008–052F. The arc uses the external DAT
  // table for its centre. The printed path delegates the same arc to PostScript.
  sealed class Bend {
   public bool Straight;
   public float X,Y,Horizontal,Vertical;
   public int EndAngle;
   public List<PrimitiveTextPoint> Body,Arrow;
  }
  static float F(double value){return (float)value;}
  static Bend Prepare(Item i,NativeNotationContext p,NativeDatGlyphs dat,bool printed){
   if(Single.IsNaN(i.P(8))||Single.IsInfinity(i.P(8))||Single.IsNaN(i.P(9))||Single.IsInfinity(i.P(9))||(i.P(10)!=0&&i.P(10)!=1)||Single.IsNaN(i.P(11))||Single.IsInfinity(i.P(11))||(short)(int)i.P(11)<0||(short)(int)i.P(11)>90)return null;
   for(int n=14;n<=20;n++)if(i.P(n)!=0)return null;
   float horizontal=i.P(6)==0?1:i.P(6),vertical=i.P(7)==0?horizontal:i.P(7);
   float arrowWidth=i.P(12)==0?1:i.P(12),arrowHeight=i.P(13)==0?arrowWidth:i.P(13);
   if(horizontal==0||vertical==0||Single.IsNaN(horizontal)||Single.IsNaN(vertical)||Single.IsInfinity(horizontal)||Single.IsInfinity(vertical)||
    (arrowWidth!=-1&&!(arrowWidth>0))||Single.IsInfinity(arrowWidth)||Single.IsNaN(arrowHeight)||Single.IsInfinity(arrowHeight))return null;
   if(i.P(10)==1?horizontal<0:vertical<0)arrowHeight=-arrowHeight;
   float x=p.Prepared.X,y=p.Prepared.Y;
   var bend=new Bend{Straight=i.P(10)==1,Arrow=new List<PrimitiveTextPoint>()};
   if(bend.Straight){
    float end=F((double)x+p.Scale*horizontal*35);
    bend.Body=new List<PrimitiveTextPoint>{Point(x,y,1),Point(end,y,0)};
    if(arrowWidth!=-1){
     float unit=F((double)p.Scale*6),tip=F((double)end+F((double)arrowHeight*unit));
     float half=F((double)arrowWidth*unit*.5550000071525574f);
     bend.Arrow.AddRange(new[]{Point(tip,y,2),Point(end,F((double)y+half),0),Point(end,F((double)y-half),0),Point(tip,y,0)});
    }
   }else{
    bend.EndAngle=180-(int)i.P(11);
    float tx,ty;
    dat.CircleTablePoint(bend.EndAngle,out tx,out ty);
    float unit=F((double)p.Scale*6.999599933624268f);
    float offsetX=F((double)tx*horizontal/20),offsetY=F((double)ty*vertical/20);
    if((short)(int)i.P(9)!=0){double radians=i.P(9)/57.2957763671875;float sin=F(Math.Sin(radians)),cos=F(Math.Cos(F(radians)));float rx=F((double)cos*offsetX+(double)sin*offsetY),ry=F((double)cos*offsetY-(double)sin*offsetX);offsetX=rx;offsetY=ry;}
    bend.X=F((double)x-offsetX*unit);
    float pitch=F((double)i.P(4)-offsetY);
    bend.Y=p.Vertical(pitch);
    bend.Horizontal=F((double)horizontal*10);
    bend.Vertical=F((double)vertical*10);
    if(!printed)bend.Body=dat.Circle(p.Scale,bend.X,bend.Y,bend.Horizontal,bend.Vertical,false,1,90,bend.EndAngle);
    if(arrowWidth!=-1){
     float arrowX=F((double)bend.X+horizontal*unit*5);
     float dy=F((double).28f*arrowHeight);
     float arrowY=p.Vertical(F((double)pitch+dy));
     bend.Arrow=dat.Circle(p.Scale,arrowX,arrowY,F((double)arrowWidth*1.2f),F((double)arrowHeight*1.2f),true,120);
    }
   }
   return bend;
  }
  static PrimitiveTextPoint Point(float x,float y,short operation){return new PrimitiveTextPoint{X=x,Y=y,Operation=operation};}
  internal static List<PrimitiveTextPoint> Scan(Item i,NativeNotationContext p,NativeDatGlyphs dat){
   var bend=Prepare(i,p,dat,false);
   if(bend==null)throw new NotSupportedException("Guitar bend controls outside the measured geometry.");
   // B125: curved bodies delegate the Code12 circle width once, then append
   // their arrow; the straight body does not use P8 for DAT thickening.
   if(!bend.Straight){short extra=unchecked((short)(int)F((double)i.P(8)*p.Scale/25));if(extra>0){float expansion=F((double)7.142899990081787f*extra/p.Scale);bend.Body.AddRange(dat.Circle(p.Scale,bend.X,bend.Y,F((double)bend.Horizontal+expansion),F((double)bend.Vertical+expansion),false,1,90,bend.EndAngle));}}
   bend.Body.AddRange(bend.Arrow);
   if((short)(int)i.P(9)!=0){for(int n=0;n<bend.Body.Count;n++){var q=bend.Body[n];q.X=NativeNotationContext.Word(q.X,true);q.Y=NativeNotationContext.Word(q.Y,false);bend.Body[n]=q;}NativeDatLines.Rotate(bend.Body,i.P(9),bend.Straight?p.Prepared.X:bend.X,bend.Straight?p.Prepared.Y:bend.Y);}
   return bend.Body;
  }
  internal static bool Draw(Graphics g,Item i,NativeNotationContext p,DrawingLibrary library){
   if(library==null)return false;
   var bend=Prepare(i,p,NativeDatDrawing.Get(library),true);
   if(bend==null)return false;
   if(bend.Straight){
    if(bend.Arrow.Count>0&&(short)(int)i.P(9)!=0){foreach(var list in new[]{bend.Body,bend.Arrow}){for(int n=0;n<list.Count;n++){var q=list[n];q.X=NativeNotationContext.Word(q.X,true);q.Y=NativeNotationContext.Word(q.Y,false);list[n]=q;}NativeDatLines.Rotate(list,i.P(9),p.Prepared.X,p.Prepared.Y);}}
    float width=i.P(8)>0?NativePrintContext.Width(unchecked((short)(int)i.P(8))):NativePrintContext.DefaultLineWidth;
    NativeNoteDetails.Path(g,p,bend.Body.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),width);
    if(bend.Arrow.Count>0)NativeNoteDetails.Path(g,p,bend.Arrow.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),width);
    if(i.P(8)>0){if(bend.Arrow.Count==0)NativePrintContext.CarryLineWidth(width);else NativePrintContext.RestoreDefaultLineWidth();}
   }else{
    var pivot=new PointF(NativeNotationContext.Word(bend.X,true),NativeNotationContext.Word(bend.Y,false));float rotation=(short)(int)i.P(9)==0?0:F(Math.Round(-i.P(9),2,MidpointRounding.AwayFromZero));
    float width=i.P(8)==0?NativePrintContext.DefaultLineWidth:NativePrintContext.Width(unchecked((short)(int)F((double)i.P(8)*NativePrintContext.Basis)));
    NativePrintedShapes.DrawArc(g,p,bend.X,bend.Y,F((double)bend.Horizontal*p.Scale*3.5),F((double)bend.Vertical/bend.Horizontal),90-bend.EndAngle,0,width,false,true,pivot,rotation);
    if(bend.Arrow.Count>0)NativeNoteDetails.Path(g,p,bend.Arrow.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),0,false,pivot,rotation);
    if(i.P(8)!=0||bend.Arrow.Count>0)NativePrintContext.RestoreDefaultLineWidth();
   }
   return true;
  }
 }
}
