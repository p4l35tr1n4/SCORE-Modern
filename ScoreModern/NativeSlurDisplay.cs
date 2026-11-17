using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeSlurDisplay {
  static bool configuredRejectInvalidTrims;public static bool RejectInvalidTrims{get{return NativeScoreBehaviour.Enabled?false:configuredRejectInvalidTrims;}private set{configuredRejectInvalidTrims=value;}}public static string Status{get;internal set;}
  static string Settings{get{return "slurs";}}
  static NativeSlurDisplay(){SetReject(true,false);}
  public static void Load(){SetReject(true,false);try{if(NativeSettings.Exists(Settings)&&NativeSettings.ReadAllText(Settings).Trim()=="policy1:original")SetReject(false,false);}catch(IOException){}catch(UnauthorizedAccessException){}}
  public static void SetReject(bool reject,bool save){RejectInvalidTrims=reject;Status=reject?"Default: reject reversed or out-of-range trims":"Compatibility: original trim-buffer effects";if(save)NativeSettings.WriteAllText(Settings,reject?"policy1:reject":"policy1:original");foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static float ReadThickness(string root){return String.IsNullOrEmpty(root)?1:NativePreferences.Read(root).Number("SLUR",1);}
  public static void AddMenu(ContextMenuStrip menu){NativeEndingDisplay.AddMenu(menu);var item=new ToolStripMenuItem("Slurs and hairpins: reject invalid trim ranges");item.Click+=(s,e)=>SetReject(!RejectInvalidTrims,true);menu.Items.Add(item);menu.Opening+=(s,e)=>{item.Checked=RejectInvalidTrims;item.ToolTipText=Status;};}
 }
 public sealed class NativeSlurFrame {
  readonly bool printed=NativeBeamDisplay.Appearance==BeamAppearance.Printed;
  sealed class StaffBuffer {public SlurVectorBuffer Buffer=new SlurVectorBuffer{End=20};public float Origin;public NativeNotationContext Page;public readonly List<TupletPlacement> Labels=new List<TupletPlacement>();public readonly List<Item> LabelOwners=new List<Item>();public readonly List<float> Scales=new List<float>();}
  readonly Dictionary<int,StaffBuffer> staffs=new Dictionary<int,StaffBuffer>();
  

  static int Word(float v){return unchecked((short)(int)Math.Truncate(v));}
  float? configuredControl;public int Records{get;private set;}public int Failures{get;private set;}
  public bool Add(Graphics g,Item item,float staffStep,float staffBase,ScoreText text,NativeNotationContext page=null){
   int offset=Word(item.P(8));if(offset>0&&offset<10)return false;
   float[] p=new float[21];for(int n=1;n<=20;n++)p[n]=item.P(n);float scale=page==null?5*staffStep:page.Scale;
   // Keep the verified word geometry in a local coordinate window. Applying
   // full-page Windows offsets before 16-bit storage would wrap lower staves.
   // Shared ordered buffer effects are retained within each staff projection.
   int key=page==null?item.Staff:0;StaffBuffer staff;if(!staffs.TryGetValue(key,out staff)){staff=new StaffBuffer();staffs.Add(key,staff);}var buffer=staff.Buffer;staff.Page=page;staff.Origin=page==null?staffBase+3*staffStep-20000/35f:page.Projection.OffsetY;
   var context=new SlurOriginContext{Scale=scale,LeftX=(float)((double)p[3]*150),LeftY=page==null?(float)((double)p[4]*7*scale-20000):page.Prepared.Y,RightY=page==null?(float)((double)p[5]*7*scale-20000):page.Vertical(p[5])};
   bool straight=Word(p[9])==99;bool widthPrefix=straight?Word(p[13])!=0:Word(p[11])==0;if(printed)buffer.End=straight?1:(widthPrefix?2:1);
   try{
    if(!configuredControl.HasValue)configuredControl=NativeSlurDisplay.ReadThickness(text==null?null:text.RootDirectory);
    var result=SlurGeometry.Build(p,context,printed,configuredControl.Value,buffer,NativeSlurDisplay.RejectInvalidTrims);Records++;
    if(!printed&&NativeItemBounds.Active){var measurement=g.Save();try{g.TranslateTransform(0,staff.Origin);DrawBuffer(g,buffer,result.Root,50,false,false,staff.Page,true);}finally{g.Restore(measurement);}}
    if(printed){var state=g.Save();try{g.TranslateTransform(0,staff.Origin);DrawBuffer(g,buffer,result.Root,NativePrintContext.Width((short)(!straight&&Word(p[11])==0?-1:0)),Word(p[11])==0&&!result.Number.HasValue,true,page);}finally{g.Restore(state);}}
    if(result.Number.HasValue){if(printed){var state=g.Save();try{g.TranslateTransform(0,staff.Origin);DrawLabel(g,text,result.Number.Value,scale,page);}finally{g.Restore(state);}}else {staff.Labels.Add(result.Number.Value);staff.LabelOwners.Add(item);staff.Scales.Add(scale);}}
   }catch(ArgumentException e){Failures++;if(!NativeNotationView.UsesDat)throw;Failure(g,item,staffBase,e.Message);}catch(NotSupportedException e){Failures++;if(!NativeNotationView.UsesDat)throw;Failure(g,item,staffBase,e.Message);}
   return true;
  }
  static void Failure(Graphics g,Item item,float staffBase,string message){NativeSlurDisplay.Status=message;using(var font=new Font("Consolas",9,GraphicsUnit.Pixel))g.VectorDrawString("[slur: "+message+"]",font,Brushes.Firebrick,120+item.P(3)*5,staffBase-30);}
  static void DrawLabel(Graphics g,ScoreText text,TupletPlacement label,float scale,NativeNotationContext page=null){if(text!=null)NativeTupletFonts.Draw(g,text.ExternalFontName(2),label.Number,(page==null?120:page.Projection.OffsetX)+(int)label.X/30f,-(int)label.Y/(page==null?35f:30f),TupletGeometry.PrintedEm(label.FontScale,scale),page!=null);}
  public void Finish(Graphics g,ScoreText text){if(printed)return;foreach(var staff in staffs.Values){var state=g.Save();try{g.TranslateTransform(0,staff.Origin);DrawBuffer(g,staff.Buffer,20,50,false,false,staff.Page);for(int j=0;j<staff.Labels.Count;j++){NativeItemBounds.Select(staff.LabelOwners[j]);DrawLabel(g,text,staff.Labels[j],staff.Scales[j],staff.Page);NativeItemBounds.Select(null);}}finally{g.Restore(state);}}}
  static void DrawBuffer(Graphics g,SlurVectorBuffer b,int root,float width,bool restore,bool printed,NativeNotationContext page=null,bool observeOnly=false){
   var requests=SlurVectorCommands.Decode(b,root,restore,printed);var state=g.Save();try{using(var matrix=new Matrix(1/30f,0,0,page==null?-1/35f:-1/30f,page==null?120:page.Projection.OffsetX,0))g.MultiplyTransform(matrix,MatrixOrder.Prepend);
    using(var pen=new Pen(Color.Black,width))using(var path=new GraphicsPath(FillMode.Alternate)){pen.LineJoin=LineJoin.Round;pen.StartCap=LineCap.Flat;pen.EndCap=LineCap.Flat;bool fill=false,have=false;PointF previous=new PointF();
     Action flush=()=>{if(path.PointCount>0){if(observeOnly){NativeItemBounds.Path(g,path,pen);}else if(printed&&page!=null){if(fill)g.TrackedFillPath(Brushes.Black,path);g.TrackedDrawPath(pen,path);var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScorePath(path,fill,pen.Width);}else{if(fill)g.VectorFillPath(Brushes.Black,path);g.VectorDrawPath(pen,path);}}path.Reset();have=false;};
     foreach(var request in requests){
      if(request.Kind==98||request.Kind==99){flush();pen.Width=NativePrintContext.FixedWidth(request.Kind==98?request.Y:(short)0);if(printed&&!observeOnly){if(request.Kind==99)NativePrintContext.RestoreDefaultLineWidth();else NativePrintContext.CarryLineWidth(pen.Width);}fill=false;continue;}
      var next=new PointF(request.X,request.Y);
      if(request.Kind==1||request.Kind==2){flush();fill=request.Kind==2;path.StartFigure();previous=next;have=true;}
      else if(request.Kind==3){path.StartFigure();previous=next;have=true;}
      else {if(have)path.AddLine(previous,next);previous=next;have=true;}
     }flush();
    }
   }finally{g.Restore(state);}
  }
 }
}


