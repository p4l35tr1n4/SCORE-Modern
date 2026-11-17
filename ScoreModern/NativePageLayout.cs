using System;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class NativePageProjection {
  // Standard Letter profile observed in the original F6 menu and emitted EPS:
  // size .01800, lmar 2100, bmar 27250. Canvas units are raw SCORE units / 30.
  public readonly float Width=34000/30f,Height=44000/30f,OffsetX=2100/30f,OffsetY=(44000-27250)/30f;
  public PointF Point(float x,float y){return new PointF(OffsetX+x/30f,OffsetY-y/30f);}
  public Matrix Matrix(){return new Matrix(1/30f,0,0,-1/30f,OffsetX,OffsetY);}
 }
 public sealed class NativeNotationContext {
  public Score SourceScore;public short StaffLedgerWidthControl,PreviousCompoundMeterNumerator;public int Staff;public float RawBase,PreviousCompoundMeterX;public float[] StaffBases,StaffScales;public PageItemContext Prepared;public NativePageProjection Projection;
  public float Scale{get{return Prepared.Scale;}}
  public float Vertical(float value){return (float)((double)value*Scale*7+RawBase);}
  public PointF Point(float x,float y){return Projection.Point(x,y);}
  // Cached PostScript origins use the converter's low signed word directly,
  // bypassing the separate vector-buffer clipping step (N141/N143).
  public static short RawWord(float value){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out n))throw new NotSupportedException("Cached origin outside the finite conversion domain.");return unchecked((short)n);}
  public static short Word(float value,bool horizontal){long n;uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(value),0);if(!FiniteBinary32Integer64.TryConvert(bits,out n))throw new NotSupportedException("Coordinate outside the verified finite conversion domain.");int v=unchecked((int)n);if(horizontal)v=Math.Max(-32600,Math.Min(32760,v));else v=Math.Min(32750,v);return unchecked((short)v);}
 } 
 public sealed class NativePageDocument {
  public int MaximumDefinedStaff;public readonly short? GuideMeasurement;
  // External geometry field starts at 4000/300; successful original printing
  // leaves its output unit here for subsequent screen redraws (V158/V162).
  public float DatBarlineUnit=13.333333015441895f;
  readonly Score guideSource;readonly byte[][] guideRecords;
  static short? ReadGuideMeasurement(Score score){try{return NativeMeasurements.Get(score);}catch(NotSupportedException){return null;}catch(System.IO.InvalidDataException){return null;}}
  bool SameGuideRecords(Score score){if(!Object.ReferenceEquals(guideSource,score)||guideRecords.Length!=score.Items.Count)return false;for(int n=0;n<guideRecords.Length;n++){byte[] a=guideRecords[n],b=score.Items[n].Bytes;if(a.Length!=b.Length)return false;for(int j=0;j<a.Length;j++)if(a[j]!=b[j])return false;}return true;}
  public readonly NativePageProjection Projection=new NativePageProjection();public readonly PageCoordinates Tables=new PageCoordinates();
  public readonly Dictionary<Item,NativeNotationContext> Contexts=new Dictionary<Item,NativeNotationContext>();
  public readonly Dictionary<Item,PageStaffResult> StaffDrawings=new Dictionary<Item,PageStaffResult>();
  public readonly List<string> Issues=new List<string>();
  static short W(float v){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(v),0),out n))throw new NotSupportedException("Parameter outside the verified finite word conversion domain.");return unchecked((short)n);}
  static float[] Parameters(Item item){var p=new float[21];for(int j=1;j<=20;j++)p[j]=item.P(j);return p;}
  internal readonly float LastCompoundMeterX;
  internal readonly short LastCompoundMeterNumerator;
  public NativePageDocument(Score score):this(score,null,false){}
  internal NativePageDocument(Score score,NativePageDocument preceding):this(score,preceding,false){}
  internal NativePageDocument(Score score,NativePageDocument preceding,bool rebuildGuideMeasurement){
   if(preceding!=null)DatBarlineUnit=preceding.DatBarlineUnit;
   GuideMeasurement=!rebuildGuideMeasurement&&preceding!=null&&preceding.SameGuideRecords(score)?preceding.GuideMeasurement:ReadGuideMeasurement(score);guideSource=score;guideRecords=new byte[score.Items.Count][];for(int n=0;n<guideRecords.Length;n++)guideRecords[n]=(byte[])score.Items[n].Bytes.Clone();
   foreach(var item in score.Items)if(item.Code==8)MaximumDefinedStaff=Math.Max(MaximumDefinedStaff,item.Staff);
   // The original file-order prepass supplies P4/P5/P10, explicit cached staff,
   // and its observed initial reference-cache word 0. It draws no staff lines.
   foreach(var item in score.Items){if(W(item.P(1))!=8)continue;var p=new float[21];p[4]=item.P(4);p[5]=item.P(5);p[10]=item.P(10);int staff=W(item.P(2));if(staff==0)staff=1;try{Tables.SetStaff(p,staff,0,false);}catch(NotSupportedException e){Issues.Add(e.Message);}}
   // Drawing dispatch updates a staff again at its actual record. Snapshot each
   // context so duplicate definitions cannot move objects already encountered.
   var ledgerWidths=new short[33];
   float previousCompoundMeterX=preceding==null?0:preceding.LastCompoundMeterX;
   short previousCompoundMeterNumerator=preceding==null?(short)0:preceding.LastCompoundMeterNumerator;
   var beforeFirstCompoundMeter=new List<NativeNotationContext>();bool seenCompoundMeter=false;
   foreach(var item in score.Items){int code=W(item.P(1)),staff=W(item.P(2));if(staff==0)staff=1;var p=Parameters(item);try{
    var prepared=Tables.Prepare(p,staff,code);var context=new NativeNotationContext{SourceScore=score,PreviousCompoundMeterX=previousCompoundMeterX,PreviousCompoundMeterNumerator=previousCompoundMeterNumerator,StaffLedgerWidthControl=ledgerWidths[staff],Staff=staff,RawBase=Tables.Bases[staff],StaffBases=(float[])Tables.Bases.Clone(),StaffScales=(float[])Tables.Scales.Clone(),Prepared=prepared,Projection=Projection};Contexts[item]=context;
    if(code==18&&W(p[5])<=90&&W(p[8])!=0){
     previousCompoundMeterX=(float)((double)prepared.X+p[15]*150);
     previousCompoundMeterNumerator=W(p[5]);
     seenCompoundMeter=true;
    }else if(!seenCompoundMeter&&code==18&&W(p[9])<0)beforeFirstCompoundMeter.Add(context);
    if(code==8){var drawing=Tables.SetStaff(p,staff,W(p[10]),false);StaffDrawings[item]=drawing;ledgerWidths[staff]=W(p[11])==99?(short)99:drawing.WidthSelector;}
   }catch(NotSupportedException e){Issues.Add(e.Message);}}
   LastCompoundMeterX=previousCompoundMeterX;
   LastCompoundMeterNumerator=previousCompoundMeterNumerator;
   // The original redraw after load leaves DS:C558/C560 set to the last
   // compound meter. The next draw consumes those values until its first compound.
   foreach(var context in beforeFirstCompoundMeter){
    context.PreviousCompoundMeterX=previousCompoundMeterX;
    context.PreviousCompoundMeterNumerator=previousCompoundMeterNumerator;
   }
  }
  public void DrawStaff(Graphics g,Item item){PageStaffResult staff;if(!StaffDrawings.TryGetValue(item,out staff))return;using(var transform=Projection.Matrix()){
   var state=g.Save();try{g.MultiplyTransform(transform,MatrixOrder.Prepend);using(var pen=new Pen(Color.Black,NativePrintContext.Width(staff.WidthSelector))){pen.LineJoin=LineJoin.Round;using(var path=new System.Drawing.Drawing2D.GraphicsPath()){foreach(var line in staff.Lines){short x1=NativeNotationContext.Word(line.Left,true),x2=NativeNotationContext.Word(line.Right,true),y=NativeNotationContext.Word(line.Y,false);path.StartFigure();path.AddLine(x1,y,x2,y);}g.TrackedDrawPath(pen,path);var eps=NativeEpsExport.For(g);if(eps!=null)eps.ScorePath(path,false,pen.Width);}}}finally{g.Restore(state);}
   }
  }

 }
}
