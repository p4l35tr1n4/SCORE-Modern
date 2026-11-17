using System;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintContext {
  sealed class Profile:IDisposable {public Profile Previous;public string Root;public short Basis=2,Beam=-2;public float Unit=SlurLineWidth.DefaultUnit,LineUnit=SlurLineWidth.DefaultUnit;public float? CarriedLineWidth;public NativePrintedChordGrid.State Grid;public bool Stroke;public int Procedures=2000;public void Dispose(){current=Previous;}}
  [ThreadStatic]static Profile current;
  public static IDisposable Begin(string root){var next=new Profile{Previous=current,Root=root};if(!String.IsNullOrEmpty(root)){var data=NativePreferences.Read(root);short resolution=data.Word("RESOLUTION",300);if(resolution<=0)throw new NotSupportedException("PREF.SCR RESOLUTION must be positive.");next.Basis=(short)Math.Round((double)resolution/150,MidpointRounding.AwayFromZero);float unit=(float)(4000.0/resolution);next.Unit=(float)((double)unit+.01);next.LineUnit=next.Unit;next.Beam=data.Word("BMTH",-1);short stroke=data.Word("SETSTROKE",0);if(stroke!=0&&stroke!=1)throw new NotSupportedException("SETSTROKE requires a verified0/1 value.");next.Stroke=stroke!=0;next.Procedures=data.Word("PROCEDURES",2000);}var job=NativePrintJob.Active;if(job!=null){next.Basis=job.LineWidth;next.Unit=job.GeometryUnit;next.LineUnit=job.Unit;next.Stroke=job.Stroke;}current=next;return next;}
  internal static NativePrintedChordGrid.State GridState(Score source,ScoreText text){if(current==null||source==null)return null;if(current.Grid==null)current.Grid=NativePrintedChordGrid.InitialState(new NativePageDocument(source),source,text);return current.Grid;}
  public static string Root{get{return current==null?null:current.Root;}}
  internal static float ExportGeometryUnit{get{return current==null?SlurLineWidth.DefaultUnit:current.Unit;}}
  public static float Unit{get{return NativeNotationView.UsesDat||current==null?SlurLineWidth.DefaultUnit:current.Unit;}}
  public static float LineUnit{get{return NativeNotationView.UsesDat||current==null?SlurLineWidth.DefaultUnit:current.LineUnit;}}
  public static short Basis{get{return NativeNotationView.UsesDat||current==null?(short)2:current.Basis;}}
  public static float UnclampedWidth(short selector){return SlurLineWidth.RawField(selector,Basis,LineUnit);}
  public static float RawWidth(short selector){return SlurLineWidth.Field(selector,Basis,LineUnit);}
  public static float Width(short selector){if(selector==0&&current!=null&&!NativeNotationView.UsesDat&&current.CarriedLineWidth.HasValue)return current.CarriedLineWidth.Value;return FixedWidth(selector);}
  public static float FixedWidth(short selector){return Single.Parse(SlurLineWidth.Format(RawWidth(selector)),CultureInfo.InvariantCulture);}
  internal static bool HasCarriedLineWidth{get{return current!=null&&!NativeNotationView.UsesDat&&current.CarriedLineWidth.HasValue;}}
  internal static void CarryLineWidth(float width){if(current!=null)current.CarriedLineWidth=width;}
  internal static void RestoreDefaultLineWidth(){if(current!=null)current.CarriedLineWidth=null;}
  public static float DefaultLineWidth{get{return Width(0);}}
  public static float BeamWidth{get{return Width(current==null?(short)-2:current.Beam);}}
  public static float PrintedDefault{get{return Single.Parse(SlurLineWidth.Format(SlurLineWidth.Field(0,current==null?(short)2:current.Basis,current==null?SlurLineWidth.DefaultUnit:current.LineUnit)),CultureInfo.InvariantCulture);}}
  public static bool StrokeAdjustment{get{return current!=null&&current.Stroke;}}
  // Original 23E8:0008..001E skips a negative budget and treats zero
  // as the default 2000-word budget.
  public static int ProcedureLimit{get{return current==null||current.Procedures==0?2000:current.Procedures;}}
 }
}
