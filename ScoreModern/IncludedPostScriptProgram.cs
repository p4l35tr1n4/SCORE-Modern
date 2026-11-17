using System;using System.Text;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // Preparation only. File resolution and canvas execution are deliberately
 // separate; a caller must supply the selected external program's contents.
 public static class IncludedPostScriptProgram {
  static void Variable(StringBuilder b,string name,float value,int decimals){b.Append('/').Append(name).Append(' ').Append(SingleFixedDecimal.Format(value,decimals)).Append(" def\n");}
  public static string Prepare(Item item,NativeNotationContext page,string source){
   if(item==null||item.Code!=15||page==null)throw new ArgumentException("Included PostScript item/context required.");
   if(item.P(13)!=0&&!item.ExternalDrawingResetResolved)throw new NotSupportedException("Included PostScript filename-reset controls remain unverified.");
   item=IncludedPostScriptDisplay.WorkingItem(item);
   var job=NativePrintJob.Active;
   float scale=job==null?.018f:job.Scale;
   int left=job==null?2100:job.LeftMargin,bottom=job==null?27250:job.BottomMargin;
   if(unchecked((short)(int)item.P(11))==-9999){var frame=IncludedEpsTransform.Prepare(page.Prepared,page.RawBase,item.P(4),item.P(5),item.P(6),item.P(7),item.P(8),item.P(12),scale,left,bottom);var eps=new StringBuilder();eps.Append(SingleFixedDecimal.Format(frame.X,3)).Append(' ').Append(SingleFixedDecimal.Format(frame.Y,3)).Append(" translate\n");if(frame.Rotate)eps.Append(SingleFixedDecimal.Format(frame.Rotation,2)).Append(" rotate\n");eps.Append(SingleFixedDecimal.Format(frame.ScaleX,3)).Append(' ').Append(SingleFixedDecimal.Format(frame.ScaleY,3)).Append(" scale\n").Append(SingleFixedDecimal.Format(frame.BoxX,3)).Append(' ').Append(SingleFixedDecimal.Format(frame.BoxY,3)).Append(" translate\n");int end=source.IndexOf((char)26);eps.Append(end<0?source:source.Substring(0,end)).Append('\n');return eps.ToString();}
   var v=IncludedPostScriptVariables.Prepare(page.Prepared,page.RawBase,item.P(5),item.P(6),scale,NativePrintContext.RawWidth(0),left,bottom);var b=new StringBuilder();
   Variable(b,"globalsize",v.GlobalSize,3);Variable(b,"lnwd",v.LineWidth,3);Variable(b,"loff",v.LeftOffset,3);Variable(b,"boff",v.BottomOffset,3);Variable(b,"stafflevel",v.StaffLevel,2);Variable(b,"pointslevel",v.PointsLevel,3);
   for(int p=3;p<=12;p++)Variable(b,"p"+p,item.P(p),3);Variable(b,"staffsize",v.StaffSize,3);Variable(b,"pp3",v.PP3,3);Variable(b,"pp4",v.PP4,3);Variable(b,"pp5",v.PP5,3);Variable(b,"pp6",v.PP6,3);
   int eof=source.IndexOf((char)26);if(eof>=0)source=source.Substring(0,eof);b.Append(source).Append('\n');return b.ToString();
  }
  public static string Wrap(string body){
   var job=NativePrintJob.Active;
   // The original restores the page matrix before inclusion, removing the
   // notation-only mirror. SCORE inside an included program keeps rotation.
   string transform=job==null?".018 dup scale 2100 27250 translate\n":job.RawProgram(false);
   // The original writes a separate 600-point rotation before the included
   // file. A PS program can then invoke the 612-point SCORE transform itself.
   string orientation=job!=null&&job.Rotate?"600 -6 translate 90 rotate\n":"";
   return "gsave 200 dict begin /SMprogramSave save def\n/SCORE {"+transform+
    NativePrintContext.PrintedDefault.ToString("F4",System.Globalization.CultureInfo.InvariantCulture)+
    " setlinewidth 1 setlinejoin} def\n/showpage {} def\n"+orientation+body+"\nSMprogramSave restore end grestore\n";
  }
 }
}
