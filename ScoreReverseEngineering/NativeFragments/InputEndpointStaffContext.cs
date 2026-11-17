using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputEndpointStaffContext {
  readonly float[] bases,scales;readonly int staff;
  internal readonly bool Scor4;
  public InputEndpointStaffContext(float[] bases,float[] scales,int staff):this(bases,scales,staff,false){}
  public InputEndpointStaffContext(float[] bases,float[] scales,int staff,bool scor4){if(bases==null||scales==null||bases.Length<33||scales.Length<33||staff<1||staff>32)throw new ArgumentException("SCORE staff table context required.");this.bases=(float[])bases.Clone();this.scales=(float[])scales.Clone();this.staff=staff;Scor4=scor4;}
  public InputEndpointStaffContext ForStaff(int value){return new InputEndpointStaffContext(bases,scales,value,Scor4);}
  // Resident 09D2:054E..060D: P12 is truncated, boundary controls are
  // cleared in the working record, and the endpoint pitch is shifted using
  // the raw staff baselines and the active (DS:8040) staff scale.
  // Overlay2/index17 stem completion uses a separate, stored-float
  // magnitude and the note direction, after the beam-direction negation.
  public void AdjustBeamStem(float[] row,int direction,ref float extension){
   if(row.Length<12)return;int control=(short)Math.Truncate(row[11]);if(control<1||control>2)return;
   if(staff==1&&control==2||staff==32&&control==1){row[11]=0;return;}
   int target=staff-(control*2-3);float scale=scales[staff];
   if(!(scale>0)||Single.IsInfinity(scale))throw new NotSupportedException("Cross-staff stem scale outside the positive finite domain.");
   float distance=(float)(((double)bases[staff]-bases[target])/((double)scale*7));
   if((long)distance<0)distance=-distance;if(direction==control)distance=-distance;
   extension=(float)((double)extension+distance);
  }
  public void Adjust(float[] row,ref float pitch){int control=(short)Math.Truncate(row[11]);if(control==0)return;if(control!=1&&control!=2)throw new NotSupportedException("Cross-staff control outside the F2 S+/S- domain.");if(staff==1&&control==2||staff==32&&control==1){row[11]=0;return;}int target=staff-(control*2-3);float scale=scales[staff];if(!(scale>0)||Single.IsInfinity(scale))throw new NotSupportedException("Cross-staff endpoint scale outside the positive finite domain.");pitch=(float)(((double)bases[target]-bases[staff])/((double)scale*7)+pitch);}
 }
}
