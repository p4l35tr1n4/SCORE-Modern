using System;
namespace ScoreModern.RecoveredFragments {
 // Original relative 2486:004A..01AB, observed at runtime 2CAA.
 // This models arithmetic for the P11 != -9999 branch only. It does not
 // format its decimal output, search for a file, or execute PostScript.
 public sealed class IncludedPostScriptVariables {
  public float GlobalSize,LineWidth,LeftOffset,BottomOffset,StaffLevel,PointsLevel,StaffSize,PP3,PP4,PP5,PP6;
  static float F(double value){return (float)value;}
  public static IncludedPostScriptVariables Prepare(PageItemContext item,float staffLevel,float p5,float p6,float printSize,float lineWidth,int leftOffset,int bottomOffset){
   var r=new IncludedPostScriptVariables();r.GlobalSize=F((double)printSize/0.017999999225139618);r.StaffSize=F((double)item.Scale/25);r.LineWidth=F((double)lineWidth*printSize);r.LeftOffset=F((double)leftOffset*printSize);r.BottomOffset=F((double)bottomOffset*printSize);r.StaffLevel=staffLevel;r.PointsLevel=F(((double)bottomOffset+staffLevel)*0.017999999225139618);r.PP3=item.X;r.PP4=item.Y;
   float step=F((double)r.StaffSize*175);r.PP5=F((double)step*p5);r.PP6=F((double)p6*step);return r;
  }
 }
}
