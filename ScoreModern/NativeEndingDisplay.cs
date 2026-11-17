using System;using System.IO;using System.Drawing;using System.Drawing.Drawing2D;using System.Windows.Forms;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeEndingDisplay {
  static bool configuredTomsTenth;public static bool TomsTenth{get{return NativeScoreBehaviour.Enabled?false:configuredTomsTenth;}private set{configuredTomsTenth=value;}}
  static string Settings{get{return "endings";}}
  public static void Load(){TomsTenth=NativeSettings.Exists(Settings)&&NativeSettings.ReadAllText(Settings).Trim()=="toms";}
  public static void Set(bool enabled,bool save){TomsTenth=enabled;if(save)NativeSettings.WriteAllText(Settings,enabled?"toms":"original");foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void AddMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("Endings: P8=10 follows Tom's Help");item.Click+=(s,e)=>Set(!TomsTenth,true);menu.Items.Add(item);menu.Opening+=(s,e)=>{item.Checked=TomsTenth;item.ToolTipText="Default: original P8=10 slur. Optional: ending path with default number 10; positive P9 overrides the number.";};}
  static short Word(float v,bool x){int n=unchecked((int)Math.Truncate(v));return unchecked((short)(x?Math.Max(-32600,Math.Min(32760,n)):Math.Min(32750,n)));}
  public static bool Draw(Graphics g,Item item,float step,float staffBase,ScoreText text,DrawingLibrary library,NativeNotationContext page=null){
   short subtype=unchecked((short)(int)Math.Truncate(item.P(8)));if(subtype<1||subtype==3||subtype>9&&!(subtype==10&&TomsTenth))return false;
   float[] p=new float[21];for(int j=1;j<=20;j++)p[j]=item.P(j);float scale=page==null?5*step:page.Scale,origin=staffBase+3*step;
   bool printed=NativeBeamDisplay.Appearance==BeamAppearance.Printed;
   var result=EndingBracket.Build(p,new TupletBracketContext{Scale=scale,LeftX=(float)((double)p[3]*150),LeftY=page==null?(float)((double)p[4]*7*scale):page.Prepared.Y,RightY=page==null?(float)((double)p[5]*7*scale):page.Vertical(p[5])},printed,TomsTenth);
   var state=g.Save();try{using(var matrix=page==null?new Matrix(1/30f,0,0,-1/35f,120,origin):page.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);
    using(var path=new GraphicsPath())using(var pen=new Pen(Color.Black,printed?NativePrintContext.DefaultLineWidth:50)){
     pen.LineJoin=LineJoin.Round;bool have=false;PointF previous=new PointF();foreach(var q in result.Points){var next=new PointF(Word(q.X,true),Word(q.Y,false));if(q.Tag>0){path.StartFigure();have=false;}if(have)path.AddLine(previous,next);previous=next;have=true;}if(path.PointCount>0)g.VectorDrawPath(pen,path);
    }
   }finally{g.Restore(state);}
   if(result.Number.HasValue&&text!=null){var n=result.Number.Value;var position=page==null?new PointF(120+(int)n.X/30f,origin-(int)n.Y/35f):page.Point((int)n.X,(int)n.Y);NativeTupletFonts.Draw(g,text.ExternalFontName(0),n.Number,position.X,position.Y,TupletGeometry.PrintedEm(n.FontScale,scale),page!=null);}
   if(result.HasDot&&library!=null){Glyph dot;if(library.Symbols.TryGetValue(24,out dot)){
    var stateDot=g.Save();try{using(var matrix=page==null?new Matrix(1/30f,0,0,-1/35f,120,origin):page.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);
     using(var path=new GraphicsPath())using(var pen=new Pen(Color.Black,printed?NativePrintContext.DefaultLineWidth:50)){pen.LineJoin=LineJoin.Round;PointF previous=new PointF();bool have=false;for(int k=0;k<dot.Points.Length;k++){
      var q=EndingBracket.DotPoint(dot.Points[k].X,dot.Points[k].Y,scale,result);var next=new PointF(Word(q.X,true),Word(q.Y,false));if(dot.Operations[k]>0){path.StartFigure();have=false;}if(have)path.AddLine(previous,next);previous=next;have=true;
     }if(path.PointCount>0){g.VectorFillPath(Brushes.Black,path);g.VectorDrawPath(pen,path);}}
    }finally{g.Restore(stateDot);}
   }}
   return true;
  }
 }
}

