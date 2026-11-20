using System;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class NativeNoteDetailPlan {public float Scale,HeadWidth,StemX,StemStart,StemEnd,LedgerLeft,LedgerRight,LedgerY,LedgerIncrement,DotX,DotY,DotScale;public float SlashX1,SlashY1,SlashX2,SlashY2,NoteX,NoteY,StaffStep,SignedStemLength;public int LedgerCount,DotCount,Pitch,StemKind;public bool Stem,Slash;}
 public static class NativeNoteDetails {
  static float F(double v){return (float)v;}
  internal static float SlashVertical(Item i,NativeNotationContext p,float value){
   int cross=(int)i.P(12),target=p.Staff-cross*2+3;
   if(cross>0&&target>0){if(p.StaffBases==null||p.StaffScales==null||target>=p.StaffBases.Length)throw new NotSupportedException("Cross-staff slash needs the recovered staff scale table.");return F((double)value*p.StaffScales[target]*7+p.StaffBases[target]);}
   return p.Vertical(value);
  }
  public static NativeNoteDetailPlan Prepare(Item i,NativeNotationContext p){return NativeNotationView.UsesDat?PrepareDat(i,p):Prepare(i,p,true);}
  public static NativeNoteDetailPlan PrepareDat(Item i,NativeNotationContext p){if(i!=null&&i.P(6)!=(int)i.P(6)){i=i.Clone();i.Set(6,(int)i.P(6));}return Prepare(i,p,false);}
  static NativeNoteDetailPlan Prepare(Item i,NativeNotationContext p,bool requirePrinted){
   if(p==null||i==null||i.Code!=1)return null;if(requirePrinted&&i.P(6)>=0&&i.P(6)<5&&i.P(6)!=(int)i.P(6)){i=i.Clone();i.Set(6,(int)i.P(6));}float scale,x,y;int rawHead=(int)i.P(6),head=rawHead>=10&&rawHead<20?rawHead-10:rawHead;var projectionItem=i;
   if(requirePrinted&&NativePrintedHeads.Parenthesized(i)){
    i=i.Clone();
    i.Set(6,head);
   }
   if(requirePrinted){if(i.P(6)<-1||i.P(6)>9&&!NativePrintedHeads.Custom(i)||i.P(6)!=(int)i.P(6)||(i.P(4)<=-200||i.P(4)>=200))return null;if(Single.IsNaN(i.P(15))||Single.IsInfinity(i.P(15))||Math.Abs(i.P(15))>32767)return null;if(i.P(12)!=0&&i.P(12)!=1&&i.P(12)!=2)return null;for(int n=12;n<=20;n++)if(n!=12&&n!=13&&n!=14&&n!=15&&n!=16&&n!=17&&n!=18&&n!=19&&i.P(n)!=0)return null;try{NativeScanMusicVectors.HeadProjection(projectionItem,p,out scale,out x,out y);}catch(NotSupportedException){return null;}}
   else {for(int n=16;n<=20;n++)if(i.P(n)!=0&&n!=16&&n!=17&&n!=18&&n!=19)return null;try{NativeScanMusicVectors.HeadProjection(i,p,out scale,out x,out y);}catch(NotSupportedException){return null;}}
   int stem=(int)i.P(5)/10%10;if(stem<0||stem>2||(requirePrinted?(Math.Abs(i.P(8))>=100&&!(Math.Abs(i.P(8))>=100&&Math.Abs(i.P(8))<200&&Math.Abs(i.P(4))>=100)):Math.Abs(i.P(8))>=200)||i.P(9)<0||i.P(9)>=2000)return null;bool explicitStem=stem!=0;if(stem==0)stem=p.Prepared.VerticalInput>6?2:1;
   if(requirePrinted&&(Single.IsNaN(i.P(16))||i.P(16)<-32768||i.P(16)>32767||i.P(16)!=(int)i.P(16)))return null;
   if(Single.IsNaN(i.P(17))||Math.Abs(i.P(17))>32767)return null;
   int pitch=(int)p.Prepared.VerticalInput,flags=(int)i.P(9)%10;float step=F((double)p.Scale*7);
   bool suppressStem=head==2||head==3||(rawHead>=100&&rawHead<=198&&rawHead!=139)||rawHead>=900;
   var v=new NativeNoteDetailPlan{Scale=scale,HeadWidth=F(((rawHead==2||rawHead==3)?19.0:17.0)*scale),Stem=explicitStem&&!suppressStem,StemKind=stem,Pitch=pitch,NoteX=x,NoteY=y,StaffStep=step};
   float extension=F(i.P(8)%100);float extra=flags>1?F((flags-1)*2.0-.5):0,length=F(((double)extra+extension+7)*scale*7);
   if(suppressStem||!explicitStem)length=0;v.SignedStemLength=stem==2?-length:length;v.StemX=stem==2?x:F((double)x+v.HeadWidth);v.StemEnd=F((double)y+v.SignedStemLength);float startFactor=head>=4&&head<9?(head==6?7:0):2.0999999046325684f;v.StemStart=F((double)startFactor*scale*(stem==2?-1:1)+y);
   if(i.P(17)%100!=0)v.StemStart=F((double)y+(i.P(17)%100)*(double)scale*7*(stem==1?1:-1));
   if(explicitStem&&Math.Abs(i.P(8))>=100&&flags>0){v.Slash=true;float dy=F((double)p.Prepared.VerticalInput+(stem==1?1:-1)*F((double)extension+extra)*.6000000238418579),dx=F((double)scale*6);v.SlashX1=F((stem==1?.7829999923706055:-1.8830000162124634)*dx+x);v.SlashX2=F((stem==1?6.5:3.799999952316284)*dx+x);v.SlashY1=SlashVertical(i,p,F((stem==1?2.799999952316284:-5.5)*.6000000238418579+dy));v.SlashY2=SlashVertical(i,p,F((stem==1?5.400000095367432:-2.799999952316284)*.6000000238418579+dy));}
   v.LedgerLeft=F((double)x-F(scale*5.0));v.LedgerRight=F((double)x+v.HeadWidth+F(scale*5.0));v.LedgerIncrement=F((pitch<=1?2.0:-2.0)*step);v.LedgerY=F((double)y+(pitch%2==0?v.LedgerIncrement/2:0));
   if(i.P(7)!=-2&&i.P(7)!=-3)v.LedgerCount=pitch<=1?(1-pitch)/2+1:pitch>=13?(pitch-13)/2+1:0;
   int word=(int)i.P(9);float fraction=F(i.P(9)%1);if(fraction>.8999999761581421)fraction=F(.8999999761581421-fraction);float offset=Math.Abs(pitch%2)==1?step:0;
   if(word>=1000){fraction=F(.2840000092983246+fraction);word-=1000;}if(word>=100){offset=-offset;word-=100;}
   float baseX=flags!=0&&stem==1&&(int)i.P(8)==0?16.200000762939453f:18;
   v.DotX=F((double)x+F(((double)fraction*60+baseX)*scale));if(i.P(19)!=0)offset=F((double)i.P(19)*step+offset);v.DotY=F((double)y+offset);v.DotCount=(int)i.P(9)>=10?word/10:0;v.DotScale=p.Scale==0?0:F((double)scale/p.Scale);return v;
  }
  public static void Path(Graphics g,NativeNotationContext page,IEnumerable<BarlinePoint> points,float width,bool wordCoordinates=false,PointF pivot=default(PointF),float rotation=0,bool inheritedWidth=false){if(NativePrintedPointBuffer.Append(g,page,points,width,wordCoordinates,pivot,rotation,inheritedWidth))return;if(inheritedWidth)width=NativePrintContext.DefaultLineWidth;var state=g.Save();try{using(var matrix=page.Projection.Matrix())g.MultiplyTransform(matrix,MatrixOrder.Prepend);if(rotation!=0){g.TranslateTransform(pivot.X,pivot.Y);g.RotateTransform(rotation);g.TranslateTransform(-pivot.X,-pivot.Y);}using(var path=new GraphicsPath(FillMode.Alternate))using(var pen=new Pen(Color.Black,width)){pen.LineJoin=LineJoin.Round;int kind=1;bool have=false;PointF previous=new PointF();Action flush=()=>{if(path.PointCount>0){if(kind!=1)g.TrackedFillPath(Brushes.Black,path);g.TrackedDrawPath(pen,path);var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScorePath(path,kind!=1,width,pivot,rotation);}else if(have&&kind==1){var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScoreMoveStroke(previous,width,pivot,rotation);}path.Reset();have=false;};foreach(var p in points){if(p.Operation>0){if(p.Operation!=3&&!(p.Operation==1&&kind==1))flush();path.StartFigure();have=false;if(p.Operation!=3)kind=p.Operation==1?1:2;}var next=wordCoordinates?new PointF(p.X,p.Y):new PointF(NativeNotationContext.Word(p.X,true),NativeNotationContext.Word(p.Y,false));if(have)path.AddLine(previous,next);previous=next;have=true;}flush();}}finally{g.Restore(state);}}
  // Original note driver09E7..09EC takes the absolute decoded hundreds for printing.
  internal static short PrintedStemSelector(Item i){return unchecked((short)Math.Abs((int)(i.P(17)/100)));}
  public static bool Stem(Graphics g,Item i,NativeNotationContext p){
   var v=Prepare(i,p);if(v==null)return false;if(!v.Stem&&!v.Slash)return true;
   short selector=PrintedStemSelector(i);
   var points=new List<BarlinePoint>();
   if(v.Slash){
    points.Add(new BarlinePoint(v.SlashX1,v.SlashY1,1));
    points.Add(new BarlinePoint(v.SlashX2,v.SlashY2,0));
    // A grace slash is emitted before SCORE applies the packed stem width.
    if(selector!=0){Path(g,p,points,NativePrintContext.DefaultLineWidth,inheritedWidth:true);points.Clear();}
   }
   if(v.Stem){
    points.Add(new BarlinePoint(v.StemX,v.StemStart,1));
    points.Add(new BarlinePoint(v.StemX,v.StemEnd,0));
   }
   if(points.Count!=0)Path(g,p,points,NativePrintContext.Width(selector),inheritedWidth:selector==0);
   return true;
  }
  public static bool Ledgers(Graphics g,Item i,NativeNotationContext p){var v=Prepare(i,p);if(v==null)return false;if(v.LedgerCount==0)return true;var points=new List<BarlinePoint>();float y=v.LedgerY;for(int n=0;n<v.LedgerCount;n++){points.Add(new BarlinePoint(v.LedgerLeft,y,1));points.Add(new BarlinePoint(v.LedgerRight,y,0));y=F((double)y+v.LedgerIncrement);}short width=p.StaffLedgerWidthControl;if(width==99)width=0;else{width=unchecked((short)(width+2));if(i.P(15)==0&&Math.Abs(i.P(4))>=100)width=unchecked((short)(width-1));}if(!NativeNotationView.UsesDat&&i.P(16)!=0)width=i.P(16)==99?(p.StaffLedgerWidthControl==99?(short)0:p.StaffLedgerWidthControl):unchecked((short)(int)i.P(16));float penWidth=NativeNotationView.UsesDat?(Math.Abs(i.P(4))>=100?NativePrintContext.Width(1):NativePrintContext.Width(2)):NativePrintContext.Width(width);NativePrintedPointBuffer.BeforeDrain(()=>Path(g,p,points,penWidth));if(width!=0)NativePrintContext.RestoreDefaultLineWidth();return true;}
  public static bool Dots(Graphics g,Item i,NativeNotationContext p,DrawingLibrary lib,HashSet<int> cache=null){if(NativeDatDrawing.Dots(g,i,p,lib))return true;var v=Prepare(i,p);if(v==null||lib==null)return false;Glyph glyph;if(!lib.Symbols.TryGetValue(24,out glyph))return false;RenderResourceAudit.Record("printed augmentation dot","24:"+glyph.Name,glyph.Source,"recovered digit/fraction controls; external dot vertices");if(cache!=null&&cache.Contains(24)){float cachedX=v.DotX;for(int dot=0;dot<v.DotCount;dot++){if(!NativePrimitiveGlyphs.Draw(g,p,lib,cache,new PrimitiveGlyphRequest{Glyph=24,X=cachedX,Y=v.DotY,Horizontal=v.DotScale,Vertical=v.DotScale,WidthSelector=0}))return false;cachedX=F((double)cachedX+v.Scale*13);}return true;}var points=new List<BarlinePoint>();float x=v.DotX;for(int dot=0;dot<v.DotCount;dot++){for(int n=0;n<glyph.Points.Length;n++)points.Add(new BarlinePoint(F(glyph.Points[n].X*(double)v.Scale/4+x),F(glyph.Points[n].Y*(double)v.Scale/4+v.DotY),glyph.Operations[n]));x=F((double)x+v.Scale*13);}Path(g,p,points,NativePrintContext.DefaultLineWidth);return true;}
 }
}
