using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 public static class NativeApproximation {
  const string Setting="approximate-rendering";
  static bool configuredEnabled;public static bool Enabled{get{return NativeScoreBehaviour.Enabled?false:configuredEnabled;}private set{configuredEnabled=value;}}
  internal static bool Active{get{return Enabled&&!NativeNotationView.UsesDat;}}
  public static void Set(bool enabled,bool save){
   if(save)NativeSettings.WriteAllText(Setting,enabled?"enabled":"disabled");
   Enabled=enabled;
   foreach(Form form in Application.OpenForms)form.Invalidate(true);
  }
  internal static void Load(){
   Enabled=false;
   try{Enabled=NativeSettings.Exists(Setting)&&NativeSettings.ReadAllText(Setting).Trim()=="enabled";}
   catch(IOException){}catch(UnauthorizedAccessException){}
  }
  internal static void AddMenu(ContextMenuStrip menu){
   var item=new ToolStripMenuItem("Allow approximate engraving when SCORE rendering is unavailable");
   item.ToolTipText="Allow a reported substitute in the high-resolution view, DAT shapes, printing or EPS when the selected SCORE rendering is unavailable.";
   item.Click+=(s,e)=>Set(!Enabled,true);
   menu.Items.Add(item);
   menu.Opening+=(s,e)=>item.Checked=Enabled;
  }
  internal static bool IsRenderFailure(Exception error){
   return error is IOException||error is InvalidDataException||error is NotSupportedException||error is ArgumentException;
  }
 }

 public sealed partial class Canvas {
  public readonly List<string> ApproximatedItems=new List<string>();

  void DrawItem(Graphics g,Item item,int index){
   if(!NativeApproximation.Active){
    try{Render(g,item);}
    catch(Exception error){if(!NativeApproximation.IsRenderFailure(error))throw;ReportDrawError(g,index,error);}
    return;
   }

   var font=PrintedFontCarry.Checkpoint;
   RectangleF oldBounds;
   bool hadBounds=ItemBounds.TryGetValue(item,out oldBounds);
   var eps=NativeEpsExport.For(g);
   Exception failure;
   using(var paint=eps==null?new NativeItemPaint(g):null){
    try{
     if(eps!=null)eps.StageItem(()=>Render(g,item));
     else Render(g,item);
     failure=null;
    }catch(Exception error){if(!NativeApproximation.IsRenderFailure(error))throw;failure=error;}
    if(failure==null){
     if(paint!=null)paint.Commit();
     return;
    }
   }
   PrintedFontCarry.Checkpoint=font;
   if(hadBounds)ItemBounds[item]=oldBounds;
   else ItemBounds.Remove(item);
   string route=ApproximateItem(g,item);
   if(route==null){ReportDrawError(g,index,failure);return;}
   string message="Approximate item "+(index+1)+" (Code "+item.Code+") using "+route+". "+failure.Message;
   ApproximatedItems.Add(message);
   if(eps!=null)eps.Comment(message);
   RenderResourceAudit.Record("approximate renderer",route,"",message);
  }

  string ApproximateItem(Graphics g,Item item){
   var page=Context(item);
   if(page==null||item.Code<1||item.Code>18||item.Code==8||item.Code==13||item.Code==15)return null;

   // The DOS screen geometry makes a useful substitute when the verified
   // printed path is unavailable. Keep its actual external glyphs and words.
   List<PrimitiveTextPoint> points=null;
   try{
    short next;
    points=NativeDatDrawing.Prepare(item,page,PageDocument,Library,TextLayout,
     NativeDatDrawing.Get(Library),new NativeDatHeadCache(),0,out next);
   }catch(Exception error){if(!NativeApproximation.IsRenderFailure(error))throw;}
   if(points!=null&&points.Count>1){
    NativeDatDrawing.Points(g,page,points);
    return "external SCORE screen geometry";
   }

   // These items have usable endpoints even if their detailed controls are
   // unverified. Only this compatibility path synthesizes a plain contour.
   if(item.Code<4||item.Code>6)return null;
   if(item.Code==5){
    if(item.P(8)>0&&item.P(8)<10)return null;
    float startTrim=item.P(14),endTrim=item.P(15);
    if(NativeSlurDisplay.RejectInvalidTrims&&
     (startTrim<0||startTrim>1||endTrim<0||endTrim>1||endTrim!=0&&startTrim>endTrim))return null;
   }
   if(item.Code==4&&item.P(5)==999)return null;
   var start=page.Point(page.Prepared.X,page.Prepared.Y);
   var end=page.Point(item.P(6)*150,page.Vertical(item.P(5)));
   if(!Finite(start)||!Finite(end)||start==end)return null;
   using(var pen=new Pen(Color.Black,item.Code==6?Math.Max(1,Step(item)*.7f):1)){
    if(item.Code==5){
     float dx=(end.X-start.X)/3;
     float arch=Math.Max(-12,Math.Min(12,item.P(7)))*Step(item);
     if(float.IsNaN(arch))return null;
     g.VectorDrawBezier(pen,start.X,start.Y,start.X+dx,start.Y-arch,end.X-dx,end.Y-arch,end.X,end.Y);
     return "a simplified curve";
    }
    g.VectorDrawLine(pen,start.X,start.Y,end.X,end.Y);
    return item.Code==6?"a single beam":"a straight line";
   }
  }
  static bool Finite(PointF p){
   return !float.IsNaN(p.X)&&!float.IsNaN(p.Y)&&!float.IsInfinity(p.X)&&!float.IsInfinity(p.Y);
  }
 }
}

