using System;using System.Drawing;using System.Globalization;using System.Windows.Forms;
namespace ScoreModern {
 internal sealed class NativeRulerStrip:Control {
  readonly Canvas canvas;readonly bool horizontal;
  internal NativeRulerStrip(Canvas value,bool top){canvas=value;horizontal=top;DoubleBuffered=true;SetStyle(ControlStyles.ResizeRedraw,true);TabStop=false;}
  PointF At(float x,float y){return NativeWorkspaceGuides.Coordinates(canvas,canvas.PointToClient(PointToScreen(new Point((int)x,(int)y))));}
  void DrawMinorTicks(Graphics g,Pen pen,double start,double end,int length,double majorStep,float dpi){
   if(!NativeWorkspaceGuides.DenseRulerTicks||start==end)return;
   double pixelsPerMajor=majorStep*length/Math.Abs(end-start);
   int subdivisions=pixelsPerMajor>=90*dpi?10:pixelsPerMajor>=45*dpi?4:pixelsPerMajor>=18*dpi?2:1;
   if(subdivisions==1)return;
   double minorStep=majorStep/subdivisions;
   double min=Math.Min(start,end),max=Math.Max(start,end);
   double first=Math.Ceiling(min/minorStep),last=Math.Floor(max/minorStep);
   int count=(int)Math.Min(Math.Max(0,last-first+1),length/Math.Max(1,7*dpi)+2);
   for(int offset=0;offset<count;offset++){
    double tick=first+offset;
    if(tick%subdivisions==0)continue;
    float position=(float)((tick*minorStep-start)/(end-start)*length);
    float depth=(tick%(subdivisions/2.0)==0?8:4)*dpi;
    if(horizontal)g.DrawLine(pen,position,Height-depth,position,Height);
    else g.DrawLine(pen,Width-depth,position,Width,position);
   }
  }
  void DrawReadout(Graphics graphics,Font font,Brush brush,Point? pointer,float dpi,float row){
   string label=NativeWorkspaceGuides.Units;
   if(label=="SCORE units"){
    int staff=!NativeWorkspaceGuides.StaffOne&&canvas.Selected!=null?canvas.Selected.Staff:1;
    label+=" (staff "+staff.ToString(CultureInfo.InvariantCulture)+")";
   }
   float left=4*dpi,right=Width-4*dpi;
   if(right<=left)return;
   using(var format=new StringFormat{Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap}){
    graphics.DrawString(label,font,brush,new RectangleF(left,0,right-left,row),format);
    float coordinatesLeft=left+graphics.MeasureString(label,font).Width+10*dpi;
    if(!pointer.HasValue||coordinatesLeft>=right)return;
    var value=NativeWorkspaceGuides.Coordinates(canvas,pointer.Value);
    string coordinates="x "+value.X.ToString("0.###",CultureInfo.InvariantCulture)+
     "   y "+value.Y.ToString("0.###",CultureInfo.InvariantCulture);
    graphics.DrawString(coordinates,font,brush,new RectangleF(coordinatesLeft,0,right-coordinatesLeft,row),format);
   }
  }
  protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);var g=e.Graphics;g.Clear(NativeWorkspaceGuides.RulerColor);if(canvas.PageDocument==null||Width<1||Height<1)return;Point? pointer=canvas.guidePointer;float dpi=NativeDisplay.Scale(this);using(var font=new Font(SystemFonts.MessageBoxFont.FontFamily,11*dpi,GraphicsUnit.Pixel))using(var pen=new Pen(Color.DimGray))using(var text=new SolidBrush(NativeWorkspaceGuides.RulerColor.GetBrightness()<.5f?Color.White:Color.Black)){
   var a=At(0,0);var b=At(Width,Height);float start=horizontal?a.X:a.Y,end=horizontal?b.X:b.Y;int length=horizontal?Width:Height;double step=NativeWorkspaceGuides.Tick(end-start,length,dpi);float row=Height/2f;
   DrawMinorTicks(g,pen,start,end,length,step,dpi);
   int decimals=Math.Max(3,Math.Min(9,(int)Math.Ceiling(-Math.Log10(step))));
   string tickFormat="0."+new string('#',decimals);
   float majorDepth=(NativeWorkspaceGuides.DenseRulerTicks?12:5)*dpi;
   if(end!=start)for(double v=Math.Ceiling(Math.Min(start,end)/step)*step,n=0;v<=Math.Max(start,end)&&n++<200;v+=step){float position=(float)((v-start)/(end-start)*length);string label=v.ToString(tickFormat,CultureInfo.InvariantCulture);if(horizontal){g.DrawLine(pen,position,Height-majorDepth,position,Height);var size=g.MeasureString(label,font);if(position+size.Width+2<=Width)g.DrawString(label,font,text,position+2,row);}else{g.DrawLine(pen,Width-majorDepth,position,Width,position);var state=g.Save();g.TranslateTransform(2,position-2);g.RotateTransform(-90);g.DrawString(label,font,text,0,0);g.Restore(state);}}
   if(horizontal){DrawReadout(g,font,text,pointer,dpi,row);g.DrawLine(pen,0,Height-1,Width,Height-1);}else g.DrawLine(pen,Width-1,0,Width-1,Height);
   if(NativeWorkspaceGuides.Crosshair&&pointer.HasValue){var p=PointToClient(canvas.PointToScreen(pointer.Value));using(var marker=new Pen(text.Color,Math.Max(1,dpi))){if(horizontal&&p.X>=0&&p.X<Width)g.DrawLine(marker,p.X,row,p.X,Height);if(!horizontal&&p.Y>=0&&p.Y<Height)g.DrawLine(marker,0,p.Y,Width,p.Y);}}
  }}
 }
 public sealed partial class Editor {
  Panel rulerHost,rulerTop;NativeRulerStrip horizontalRuler,verticalRuler;Panel rulerCorner;
  Control CreateRulerWorkspace(){rulerHost=new Panel{Dock=DockStyle.Fill};rulerTop=new Panel{Dock=DockStyle.Top};rulerCorner=new Panel{Dock=DockStyle.Left};horizontalRuler=new NativeRulerStrip(canvas,true){Dock=DockStyle.Fill};verticalRuler=new NativeRulerStrip(canvas,false){Dock=DockStyle.Left};rulerTop.Controls.Add(horizontalRuler);rulerTop.Controls.Add(rulerCorner);rulerHost.Controls.Add(viewport);rulerHost.Controls.Add(verticalRuler);rulerHost.Controls.Add(rulerTop);canvas.GuidesChanged=()=>{horizontalRuler.Invalidate();verticalRuler.Invalidate();};viewport.Scroll+=(s,e)=>canvas.NotifyGuides();viewport.Resize+=(s,e)=>canvas.NotifyGuides();RefreshRulerLayout();return rulerHost;}
  void RefreshRulerLayout(){if(rulerHost==null)return;float dpi=NativeDisplay.Scale(this);rulerTop.Height=(int)Math.Ceiling(NativeWorkspaceGuides.RulerHeight*dpi);verticalRuler.Width=rulerCorner.Width=(int)Math.Ceiling(NativeWorkspaceGuides.RulerWidth*dpi);rulerTop.Visible=verticalRuler.Visible=NativeWorkspaceGuides.Rulers;rulerCorner.BackColor=NativeWorkspaceGuides.RulerColor;canvas.NotifyGuides();}
 }
}
