using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;using System.Drawing;using System.Drawing.Drawing2D;using System.Globalization;using System.Text;
namespace ScoreModern {
 public static class NativePrintedFilledWave {
  // Original SCORE.EXE REAL4 algorithm tables, offsets168194/168434;35 entries.
  static readonly float[] X={13f,7f,-1f,30f,39f,46f,52f,59f,65f,74f,87f,116f,125f,127f,131f,134f,138f,143f,148f,156f,124f,117f,110f,104f,97f,90f,83f,72f,40f,32f,27f,23f,19f,17f,13f};
  static readonly float[] Y={24f,18.5f,26f,57f,64f,67f,68f,68f,67f,63f,54f,29f,22f,21f,20f,20f,21f,25f,30f,22f,-10f,-16f,-19f,-20f,-20f,-19f,-16f,-8f,21f,27f,29f,29f,28f,27f,24f};
  static float F(double v){return (float)v;}
  static string R(float v){return ScoreModern.RecoveredFragments.SingleFixedDecimal.Format(v,1);}
  static float Q(float v){return Single.Parse(R(v),CultureInfo.InvariantCulture);}
  public static bool Draw(Graphics g,Item i,NativeNotationContext p){
   if(i.Code!=4||i.P(7)!=-1||i.P(5)>=999)return false;
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Math.Abs(i.P(n))>32767)return false;
   for(int n=10;n<=20;n++)if(n!=10&&n!=12&&n!=13&&n!=15&&i.P(n)!=0)return false;
   float width=i.P(8)>0?i.P(8):1;
   float height=i.P(9)==0?width:i.P(9);
   bool vertical=i.P(6)==0||i.P(3)==i.P(6);
   if(vertical||i.P(13)!=0)return DrawBuffered(g,i,p,width,height,vertical);
   if(i.P(3)>=i.P(6))return true;
   return DrawHorizontal(g,p,p.Prepared.X,p.Prepared.Y,F(i.P(6)*150.0),width,(short)i.P(12)!=0,height);
  }
  internal static bool DrawHorizontal(Graphics g,NativeNotationContext p,float x,float baseline,float end,float size,bool arrow=false,float? height=null){
   // Original 04ED:0A0C keeps the scale product in ST(0) for X,
   // then reloads the stored REAL4 at DS:C050 before multiplying for Y.
   double staffFactor=(double)p.Scale*.08250000327825546f;
   float horizontal=F(staffFactor*size),vertical=F((double)F(staffFactor)*(height??size));
   float rawStep=F(horizontal*146.0),step=Q(rawStep);
   float left=Q(x),right=Q(end),y=Q(baseline);if(!(step>0)||right<left)return false;
   int count=(int)Math.Floor(((double)right-left)/step)+1;if(count>30000)return false;
   var vertices=new PointF[X.Length];for(int n=0;n<X.Length;n++)vertices[n]=new PointF(Q(F(X[n]*(double)horizontal)),Q(F(Y[n]*(double)vertical)));
   float width=NativePrintContext.Width(-1);var state=g.Save();try{using(var matrix=p.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);using(var pen=new Pen(Color.Black,width)){pen.LineJoin=LineJoin.Round;for(int repeat=0;repeat<count;repeat++)using(var path=new GraphicsPath(FillMode.Alternate)){var points=new PointF[vertices.Length];for(int n=0;n<points.Length;n++)points[n]=new PointF(F(left+(double)repeat*step+vertices[n].X),F(y+(double)vertices[n].Y));path.AddLines(points);g.TrackedFillPath(Brushes.Black,path);g.TrackedDrawPath(pen,path);}}}finally{g.Restore(state);}
   var eps=NativeEpsExport.For(g);if(eps!=null){var b=new StringBuilder(eps.DefaultInkProgram+"1 setlinejoin 0 setlinecap\n");b.Append(width.ToString("F4",CultureInfo.InvariantCulture)).Append(" setlinewidth\ngsave ").Append(R(left)).Append(' ').Append(R(y)).Append(" translate\n").Append(R(left)).Append(' ').Append(R(step)).Append(' ').Append(R(right)).Append(" { pop newpath\n");for(int n=0;n<vertices.Length;n++)b.Append(R(vertices[n].X)).Append(' ').Append(R(vertices[n].Y)).Append(n==0?" moveto\n":" lineto\n");b.Append("gsave eofill grestore stroke\n").Append(R(step)).Append(" 0 translate } for grestore\n");eps.ScoreFilledWaveProgram(b.ToString(),count);}
   if(arrow)DrawArrow(g,p,x,baseline,end,horizontal,vertical,rawStep,width);
   NativePrintContext.RestoreDefaultLineWidth();
   RenderResourceAudit.Record("printed filled waveform","Code4 P7=-1","original executable REAL4 tables","printed decimal table and repeated fill/stroke; horizontal measured domain");return true;
  }
  static bool DrawBuffered(Graphics g,Item i,NativeNotationContext p,float width,float height,bool vertical){
   float x=p.Prepared.X,y=p.Prepared.Y,right=F(i.P(6)*150.0),angle=i.P(13);
   if(vertical){
    x=F((double)x+i.P(15)*150.0);
    angle=-90;
    float dy=F((double)i.P(5)-i.P(4));
    // SCORE 04ED:0A82–0AA5 lowers the printed baseline by DS:6828.
    y=F((double)y-p.Scale*(double).8f);
    if((short)i.P(12)<0){dy=-dy;y=F((double)p.Vertical(i.P(5))+p.Scale*14);}
    float length=F((double)p.Scale*dy*7);
    if(length<0){length=-length;angle=-angle;}
    right=F((double)x+length);
   }
   double unit=(double)p.Scale*.08250000327825546f;
   float h=F(unit*width),v=F((double)F(unit)*height),step=F((double)h*146);
   if(!(step>0))return false;if(right<=x)return true;
   var points=new List<PrimitiveTextPoint>();
   float origin=x;
   int count=0;
   do{
    if(++count>850)return false;
    AddContour(points,X,Y,origin,y,h,v);
    double next=(double)origin+step;
    origin=F(next);
    if(next>=right)break;
   }while(true);
   if((short)i.P(12)!=0)AddContour(points,ArrowX,ArrowY,F((double)origin-step),y,h,v);
   NativeDatLines.Rotate(points,angle,x,y);
   NativeNoteDetails.Path(g,p,points.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),NativePrintContext.Width(-1));
   NativePrintContext.RestoreDefaultLineWidth();
   RenderResourceAudit.Record("printed filled waveform","Code4 P7=-1","original executable REAL4 tables","buffered contour and word rotation");
   return true;
  }
  static void AddContour(List<PrimitiveTextPoint> points,float[] xs,float[] ys,float x,float y,float h,float v){
   for(int n=0;n<xs.Length;n++)points.Add(new PrimitiveTextPoint{
    X=NativeNotationContext.Word(F((double)x+(double)xs[n]*h),true),
    Y=NativeNotationContext.Word(F((double)y+(double)ys[n]*v),false),
    Operation=(short)(n==0?2:0)
   });
  }
  // The thirteen entries immediately after each waveform table in SCORE.EXE
  // (file offsets 168334 and 168574) form the filled end arrow.
  static readonly float[] ArrowX={148,152,154,159,201,148,268,148,201,163,158,155,148};
  static readonly float[] ArrowY={29,32,33,34,34,106,29,-47,25,25,24,22,29};
  static void DrawArrow(Graphics g,NativeNotationContext page,float x,float y,float right,float horizontal,float vertical,float step,float width){
   int repeats=(int)(((double)right-x)/step+1);
   float next=F(x+(double)repeats*step);
   float last=F((double)next-step);
   var points=new System.Collections.Generic.List<BarlinePoint>();
   for(int n=0;n<ArrowX.Length;n++)
    points.Add(new BarlinePoint(F((double)last+ArrowX[n]*(double)horizontal),
     F((double)y+ArrowY[n]*(double)vertical),n==0?2:0));
   NativeNoteDetails.Path(g,page,points,width);
  }
 }
}
