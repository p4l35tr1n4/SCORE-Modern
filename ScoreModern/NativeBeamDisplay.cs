using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeBeamDisplay {
  public static BeamAppearance Appearance{get;private set;}
  static string Settings{get{return "beams";}}
  public static void Load(){Appearance=BeamAppearance.Printed;try{if(NativeSettings.Exists(Settings)&&NativeSettings.ReadAllText(Settings).Trim()=="dos")Appearance=BeamAppearance.DosScreen;}catch(IOException){}}
  public static void Set(BeamAppearance value,bool save){Appearance=value;if(save)NativeSettings.WriteAllText(Settings,value==BeamAppearance.Printed?"printed":"dos");foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void AddMenu(ContextMenuStrip menu){NativeNotationView.AddMenu(menu);
   menu.Items.Add(new ToolStripSeparator());var printed=new ToolStripMenuItem("Beams/slurs: printed / EPS");var dos=new ToolStripMenuItem("Beams/slurs: DOS screen geometry");
   printed.Click+=(s,e)=>Set(BeamAppearance.Printed,true);dos.Click+=(s,e)=>Set(BeamAppearance.DosScreen,true);menu.Items.Add(printed);menu.Items.Add(dos);NativeTupletFonts.AddMenu(menu);NativeSlurDisplay.AddMenu(menu);NativeEditCompatibility.AddMenu(menu);
   menu.Opening+=(s,e)=>{printed.Checked=Appearance==BeamAppearance.Printed;dos.Checked=Appearance==BeamAppearance.DosScreen;};
  }
  static float Pitch(float value){return Math.Abs(value)>=100&&Math.Abs(value)<200?value-Math.Sign(value)*100:value;}
  public static bool Draw(Graphics g,Item item,float staffStep,float staffBase){return Draw(g,item,staffStep,staffBase,null);}
  public static bool Draw(Graphics g,Item item,float staffStep,float staffBase,ScoreText text,NativeNotationContext page=null){
   int code=(int)item.P(7);if(code==0||(Math.Abs(code)/10!=1&&Math.Abs(code)/10!=2))return false;
   float[] p=new float[21];for(int n=1;n<=20;n++)p[n]=item.P(n);
   // Normalize the original caller's coordinate space to the existing canvas:
   // horizontal SCORE units remain five canvas units; one vertical scale step
   // remains staffStep. The recovered routine itself is shared with the proofs.
   float scale=page==null?5*staffStep:page.Scale,verticalStep=7*scale;
   var context=new BeamGeometryContext{Scale=scale,LeftVertical=page==null?(float)((double)Pitch(p[4])*verticalStep):page.Prepared.Y,Vertical=v=>page==null?(float)((double)Pitch(v)*verticalStep):page.Vertical(v)};
   BeamReference reference;var shapes=BeamGeometry.Build(p,context,Appearance,out reference);
   Action drawLabel=()=>{
    short number=unchecked((short)(int)Math.Truncate(p[8]));
    if(number>0&&text!=null){
     var t=TupletGeometry.Build(number,Math.Abs(code)/10,reference.Cue,p[16],p[19],reference.StaffScale,reference.LeftX,reference.LeftY,reference.Dx,reference.Dy);
     var position=page==null?new PointF(120+(int)t.X/30f,staffBase+3*staffStep-(int)t.Y/35f):page.Point((int)t.X,(int)t.Y);NativeTupletFonts.Draw(g,text.ExternalFontName(2),t.Number,position.X,position.Y,TupletGeometry.PrintedEm(t.FontScale,reference.StaffScale),page!=null);
    }
   };
   if(Appearance==BeamAppearance.Printed)drawLabel();
   foreach(var shape in shapes){
    if(Appearance==BeamAppearance.Printed&&page!=null){var raw=new System.Collections.Generic.List<BarlinePoint>();for(int n=0;n<shape.Vertices.Length;n++){var v=shape.Vertices[n];raw.Add(new BarlinePoint(NativeNotationContext.Word(v.X,true),NativeNotationContext.Word(v.Y,false),n==0?2:0));}if(raw.Count>1){raw.Add(new BarlinePoint(raw[0].X,raw[0].Y,0));raw.Add(raw[1]);}NativeNoteDetails.Path(g,page,raw,NativePrintContext.BeamWidth);continue;}
    PointF[] points=shape.Vertices.Select(v=>page==null?new PointF(120+((int)v.X)/30f,staffBase+3*staffStep-((int)v.Y)/35f):page.Point(NativeNotationContext.Word(v.X,true),NativeNotationContext.Word(v.Y,false))).ToArray();
    if(Appearance==BeamAppearance.Printed){g.VectorFillPolygon(Brushes.Black,points,FillMode.Alternate);using(var pen=new Pen(Color.Black,0))g.VectorDrawPolygon(pen,points);}
    else {using(var pen=new Pen(Color.Black,50/35f))g.VectorDrawLines(pen,new[]{points[2],points[1],points[0],points[3]});}
   }
   if(Appearance!=BeamAppearance.Printed)drawLabel();
   return true;
  }
 }
}
