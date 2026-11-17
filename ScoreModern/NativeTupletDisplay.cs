using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeTupletDisplay {

  static short Word(float value,bool horizontal){int n=unchecked((int)Math.Truncate(value));if(horizontal)n=Math.Max(-32600,Math.Min(32760,n));else n=Math.Min(32750,n);return unchecked((short)n);}
  public static bool Draw(Graphics g,Item item,float staffStep,float staffBase,ScoreText text,NativeNotationContext page=null){
   if(unchecked((short)(int)Math.Truncate(item.P(8)))!=3)return false;
   float[] p=new float[21];for(int n=1;n<=20;n++)p[n]=item.P(n);float scale=page==null?5*staffStep:page.Scale,vertical=7*scale;
   var context=new TupletBracketContext{Scale=scale,LeftX=(float)((double)p[3]*150),LeftY=page==null?(float)((double)p[4]*vertical):page.Prepared.Y,RightY=page==null?(float)((double)p[5]*vertical):page.Vertical(p[5])};
   var bracket=TupletBracket.Build(p,context);
   using(var path=new GraphicsPath()){
    bool have=false;PointF previous=new PointF();
    foreach(var point in bracket.Points){PointF next=new PointF(Word(point.X,true),Word(point.Y,false));if(point.Tag>0){path.StartFigure();have=false;}if(have)path.AddLine(previous,next);previous=next;have=true;}
    var state=g.Save();try{using(var transform=page==null?new Matrix(1/30f,0,0,-1/35f,120,staffBase+3*staffStep):page.Projection.Matrix()){g.MultiplyTransform(transform,MatrixOrder.Prepend);}
     // Measured default wdl in the supplied original PostScript configuration.
     // Raw-coordinate transform preserves its anisotropic canvas projection.
     using(var pen=new Pen(Color.Black,NativePrintContext.DefaultLineWidth)){pen.LineJoin=LineJoin.Round;pen.StartCap=LineCap.Flat;pen.EndCap=LineCap.Flat;if(path.PointCount>0)g.VectorDrawPath(pen,path);}
    }finally{g.Restore(state);}
   }
   if(bracket.Number.HasValue&&text!=null){var label=bracket.Number.Value;var position=page==null?new PointF(120+(int)label.X/30f,staffBase+3*staffStep-(int)label.Y/35f):page.Point((int)label.X,(int)label.Y);NativeTupletFonts.Draw(g,text.ExternalFontName(2),label.Number,position.X,position.Y,TupletGeometry.PrintedEm(label.FontScale,scale),page!=null);}
   return true;
  }
 }
}

