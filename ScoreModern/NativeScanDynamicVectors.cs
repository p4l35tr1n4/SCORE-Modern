using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeScanDynamicVectors {
  public static List<PrimitiveTextPoint> Prepare(Item item,NativeNotationContext page,string root,ScoreText text=null){short? ignored;return Prepare(item,page,root,text,out ignored);}
  public static List<PrimitiveTextPoint> Prepare(Item item,NativeNotationContext page,string root,ScoreText text,out short? labelFont,bool drawingVectors=false){
   labelFont=null;
   if(item==null||page==null)throw new ArgumentNullException();var points=new List<PrimitiveTextPoint>();
   if(item.Code==4&&item.P(5)<999){
    short type=unchecked((short)(int)Math.Truncate(item.P(7)));
    if(type==0)return NativeDatLines.Straight(item,page);
    if(type<0)return NativeDatLines.Waves(item,page);
    if(type>=100)return NativeDatLines.MultiPoint(item,page);
    return NativeDatLines.Dashes(item,page);
   }
   if(item.Code==4)return NativeDatLines.Hairpin(item,page);
   var parameters=new float[21];for(int n=1;n<=20;n++)parameters[n]=item.P(n);
   if(item.Code==6){var context=new BeamGeometryContext{Scale=page.Scale,LeftVertical=page.Prepared.Y,Vertical=v=>page.Vertical(v)};BeamReference reference;var shapes=BeamGeometry.Build(parameters,context,drawingVectors?BeamAppearance.Printed:BeamAppearance.DosScreen,out reference);foreach(var shape in shapes){int[] order=drawingVectors?new[]{0,1,2,3,0}:new[]{2,1,0,3};for(int at=0;at<order.Length;at++){var p=shape.Vertices[order[at]];points.Add(new PrimitiveTextPoint{X=p.X,Y=p.Y,Operation=(short)(at==0?(drawingVectors?2:1):0)});}}short number=unchecked((short)(int)Math.Truncate(item.P(8)));if(number>0){int direction=Math.Abs((int)unchecked((short)(int)Math.Truncate(item.P(7))))/10;var label=TupletGeometry.Build(number,direction,reference.Cue,item.P(16),item.P(19),reference.StaffScale,reference.LeftX,reference.LeftY,reference.Dx,reference.Dy);points.AddRange(Label(item,page,text,label,1002,reference.StaffScale));labelFont=2;}return points;}
   if(item.Code==5){short subtype=unchecked((short)(int)item.P(8));if(subtype>=1&&subtype<=9&&subtype!=3||subtype==10&&NativeEndingDisplay.TomsTenth){var ending=EndingBracket.Build(parameters,new TupletBracketContext{Scale=page.Scale,LeftX=page.Prepared.X,LeftY=page.Prepared.Y,RightY=page.Vertical(item.P(5))},false,NativeEndingDisplay.TomsTenth);foreach(var p in ending.Points)points.Add(new PrimitiveTextPoint{X=p.X,Y=p.Y,Operation=p.Tag});if(ending.HasDot)points.AddRange(new NativeDatGlyphs(root).Expand(24,1,1,page.Scale,ending.DotX,ending.DotY));if(ending.Number.HasValue){points.AddRange(Label(item,page,text,ending.Number.Value,1000));labelFont=0;}return points;}if(subtype==3){var bracket=TupletBracket.Build(parameters,new TupletBracketContext{Scale=page.Scale,LeftX=page.Prepared.X,LeftY=page.Prepared.Y,RightY=page.Vertical(item.P(5))});foreach(var p in bracket.Points)points.Add(new PrimitiveTextPoint{X=p.X,Y=p.Y,Operation=p.Tag});if(bracket.Number.HasValue){points.AddRange(Label(item,page,text,bracket.Number.Value));labelFont=2;}return points;}var context=new SlurOriginContext{Scale=page.Scale,LeftX=page.Prepared.X,LeftY=page.Prepared.Y,RightY=page.Vertical(item.P(5))};var result=SlurGeometry.Build(parameters,context,drawingVectors,NativeSlurDisplay.ReadThickness(root),new SlurVectorBuffer{End=1},NativeSlurDisplay.RejectInvalidTrims);var b=result.Buffer;short operation=0;for(int n=1;n<b.End;n++){short x=b.X[n];if(x< -32600){if(operation!=0)points.Add(new PrimitiveTextPoint{Operation=operation,CommandOnly=true});operation=(short)(x+32700);continue;}points.Add(new PrimitiveTextPoint{X=x,Y=b.Y[n-1],Operation=operation});operation=0;}if(operation!=0)points.Add(new PrimitiveTextPoint{Operation=operation,CommandOnly=true});if(result.Number.HasValue){points.AddRange(Label(item,page,text,result.Number.Value));labelFont=2;}return points;}
   throw new NotSupportedException("Dynamic DAT item route is not recovered.");
  }
  static List<PrimitiveTextPoint> Label(Item source,NativeNotationContext page,ScoreText text,TupletPlacement label,short font=1002,float? staffScale=null){
   if(text==null)throw new NotSupportedException("DAT label requires its external text resources.");
   var body=Item.Make(16,source.P(2),0,0,1,label.FontScale,0,font,0,0,0,1,0);string value=TupletGeometry.NumberText(label.Number);float scale=staffScale.HasValue?staffScale.Value:page.Scale;text.Set(body,value,scale);
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=label.X;context.Prepared.Y=label.Y;context.Prepared.Scale=scale;
   return text.ScanStrokeText(body,value,context,0);
  }
 }
}
