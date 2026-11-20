using System;
namespace ScoreModern.RecoveredFragments {
 // Original commercial SCOR4 controls. Callers explicitly select the version;
 // the default remains SCORE 3.11. No process-wide version state is retained.
 public static class ScoreVersionControls {
  public static LayoutSymbolSpacingResult NumberSpacing(float size,float offset,float staffScale){
   if(Single.IsNaN(size)||Single.IsInfinity(size)||Single.IsNaN(offset)||Single.IsInfinity(offset)||!(staffScale>0))throw new NotSupportedException("Nonfinite number justification controls.");
   // Original SCOR4 J fixtures SHORTO/SHORTM/NRIGHT/NSIZE/NTWO:
   // fixed left/right allowances .2 and 4, independent of size/digit count.
   // Keep division extended until each final binary32 assignment.

   double shift=(double)offset/staffScale;
   return new LayoutSymbolSpacingResult{Left=(float)((double).2f-shift),Right=(float)((double)4f+shift)};
  }
  public static bool SlurEndpointMode(bool scor4,float p16){return scor4&&p16==1;}
  public static short SlurSubtype(bool scor4,float p8,float p16){return SlurEndpointMode(scor4,p16)?(short)0:unchecked((short)(int)Math.Truncate(p8));}
  public static bool SlurEndpointOverride(bool scor4,float[] p){return p!=null&&p.Length>18&&SlurEndpointMode(scor4,p[16]);}
  public static float[] SlurParameters(bool scor4,float[] parameters){if(!SlurEndpointOverride(scor4,parameters))return parameters;var p=(float[])parameters.Clone();p[8]=0;return p;}
  public static float DrawingOffset(bool scor4,float value){return scor4?Horizontal(value):0;}
  public static float HarpOffset(bool scor4,float value){return scor4?Horizontal(value):0;}
  public static float Horizontal(float value){if(Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("Nonfinite SCORE horizontal offset.");return (float)((double)value*150);}
  public static bool MeterThicknessSupported(bool scor4,float value){return scor4?!Single.IsNaN(value)&&!Single.IsInfinity(value)&&value>=-32768&&value<=32767:value==0||value==-2||value==2||value==1.25f||value==-1.25f||value==.5f||value==-1;}
  public static short MeterThicknessSelector(bool scor4,float value){if(!MeterThicknessSupported(scor4,value))throw new NotSupportedException("Meter outline thickness leaves the verified word domain.");return scor4?unchecked((short)(int)Math.Truncate(value)):(short)0;}
 }
}
