using System;
using System.Drawing;
using System.Globalization;
namespace ScoreModern {
 public struct NativeNumberFrame {
  public int Number,Enclosure;
  public float X,Y,Size;
  public short Font;
 }
 public static class NativeNumberDisplay {
  static readonly short[] Faces={0,2,98,90,92,0,94,91,93,1,1,3,0};
  static float F(double x){return (float)x;}
  internal static string Value(int number){
   if(number==-1)return "/";
   if(number<1000)return number.ToString(CultureInfo.InvariantCulture);
   return number==1000?":00":((char)('A'+number-1001)).ToString();
  }
  internal static Item TextItem(Item item,NativeNumberFrame frame){
   return Item.Make(16,item.P(2),item.P(3),item.P(4),item.P(14),frame.Size,0,frame.Font<90?1000+frame.Font:frame.Font,0,0,0,1,0);
  }
  internal static NativeNotationContext Position(NativeNotationContext page,NativeNumberFrame frame){
   var result=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Projection=page.Projection,Prepared=page.Prepared};
   result.Prepared.X=frame.X;
   result.Prepared.Y=frame.Y;
   return result;
  }
  public static bool Prepare(Item item,NativeNotationContext page,out NativeNumberFrame frame){
   frame=new NativeNumberFrame();
   if(page==null||item.Code!=10)return false;
   for(int n=5;n<=15;n++)if(Single.IsNaN(item.P(n))||Single.IsInfinity(item.P(n)))return false;
   for(int n=16;n<=20;n++)if(item.P(n)!=0)return false;
   int number=(int)item.P(5),font=(int)item.P(7),enclosure=(int)item.P(8);
   if(item.P(5)<-1||item.P(5)>1026||font<0||font!=item.P(7)||font>12&&(font<1000||font>1089)||
    item.P(8)!=enclosure||enclosure<-32768||enclosure>32767||item.P(10)!=(int)item.P(10)||
    item.P(10)<0||item.P(10)>100||Math.Abs(item.P(9))>32767)return false;
   float size=F(item.P(6)%100);
   if(size==0)size=1;
   frame=new NativeNumberFrame{Number=number,Enclosure=enclosure,X=F((double)page.Prepared.X+item.P(15)*150),
    Y=page.Prepared.Y,Size=size,Font=font>=1000?(short)(font-1000):Faces[font]};
   return true;
  }
  internal static RectangleF Enclosure(Item item,NativeNotationContext page,NativeNumberFrame frame,float width){
   float x=F((double)frame.X+width*75);
   if(frame.Font>=90)x=F((double)x-page.Scale*frame.Size*1.2000000476837158);
   float y=page.Vertical(F((double)frame.Size*1.5499999523162842+item.P(4)+item.P(13)));
   bool customWidth=(short)(int)item.P(11)!=0;
   float h,v;
   if(customWidth){
    h=item.P(11);
    v=item.P(12)==0?h:item.P(12);
   }else{
    h=frame.Number<10||frame.Number>1000?5:frame.Number<100?6.5f:9;
    if(frame.Enclosure!=2&&h==5)h=5.5f;
    v=frame.Enclosure==2?5:h;
   }
   float factor=F((double)page.Scale*3.5);
   h=F((double)F((double)h*frame.Size)*factor);
   v=F((double)F((double)v*frame.Size)*factor);
   return new RectangleF(x,y,h,v);
  }
  static void DrawEnclosure(Graphics g,Item item,NativeNotationContext page,NativeNumberFrame frame,float width){
   if(frame.Enclosure==0)return;
   var area=Enclosure(item,page,frame,width);
   short selector=EnclosureWidth(item);
   if(frame.Enclosure!=2){
    NativePrintedShapes.DrawArc(g,page,area.X,area.Y,area.Width,F((double)area.Height/area.Width),-270,90,
     NativePrintContext.FixedWidth(selector),false,true,exactDecimal:true);
    if(frame.Enclosure>2)NativePrintContext.CarryLineWidth(NativePrintContext.FixedWidth(selector));else NativePrintContext.RestoreDefaultLineWidth();
    return;
   }
   float left=F((double)area.X-area.Width),right=F((double)area.X+area.Width),bottom=F((double)area.Y-area.Height),top=F((double)area.Y+area.Height);
   var points=new[]{new BarlinePoint(left,bottom,1),new BarlinePoint(right,bottom,0),new BarlinePoint(right,top,0),
    new BarlinePoint(left,top,0),new BarlinePoint(left,bottom,0),new BarlinePoint(right,bottom,0)};
   NativeNoteDetails.Path(g,page,points,NativePrintContext.FixedWidth(selector));
   NativePrintContext.RestoreDefaultLineWidth();
  }
  static short EnclosureWidth(Item item){return (short)(NativePrintContext.Basis*(item.P(10)==0?1:item.P(10)));}
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,ScoreText text,Item preceding=null){
   NativeNumberFrame frame;
   if(text==null||!Prepare(item,page,out frame))return false;
   var context=Position(page,frame);
   var body=TextItem(item,frame);
   string value=Value(frame.Number);
   text.Set(body,value,page.Scale);
   float width=frame.Enclosure==0?0:text.PrepareLayout(body,value,page.Scale,0,true).Width;
   if(frame.Font>=90&&frame.Enclosure!=0&&frame.Enclosure!=2)DrawEnclosure(g,item,page,frame,width);
   var eps=NativeEpsExport.For(g);
   if(eps!=null)eps.Comment("SCORE bar number "+frame.Number+" x "+frame.X.ToString("R",CultureInfo.InvariantCulture)+" y "+frame.Y.ToString("R",CultureInfo.InvariantCulture)+" size "+frame.Size.ToString("R",CultureInfo.InvariantCulture)+" font "+frame.Font);
   RenderResourceAudit.Record("bar numeral",value,text.ExternalFontName(frame.Font),"recovered Code10 numeric text request");
   // The shared original printer width can be retained by earlier items.
   // Primitive number strokes consume it even across item families.
   if(frame.Font>=90&&(frame.Enclosure==0||frame.Enclosure==2)&&NativePrintContext.HasCarriedLineWidth){
    var plan=text.PreparePostScript(body,context,GhostscriptTextRenderer.DeviceMatrix(g));
    if(plan==null)throw new NotSupportedException("Primitive number has no verified text plan.");
    text.PaintPostScript(g,plan,NativePrintContext.DefaultLineWidth);
   }else text.Draw(g,body,value,0,0,page.Scale/5,page.Scale,context);
   if(frame.Font<90||frame.Enclosure==2)DrawEnclosure(g,item,page,frame,width);
   return true;
  }
 }
}

