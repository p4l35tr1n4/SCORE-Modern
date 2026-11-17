using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputAttachedMarks {
  // Default-side attached-mark numeric branch, section60 0D9F/0DE2.
  // Opposite-side marks, Code9 additions and ranges are caller branches.
  public static float DefaultValue(int mark,float pitch,float stem){
   if(mark<1||mark>36)throw new ArgumentOutOfRangeException("mark");
   if(mark!=4&&mark!=5&&mark!=8&&mark!=9&&mark!=10&&mark!=19)return mark;
   int y=((int)Math.Truncate(pitch))%100;float offset=0;
   if(stem%100<20){if(y<4){if(mark==5)offset=0.05000000074505806f;}else{offset=(float)((double)y/10-(mark==5?0.3499999940395355f:0.4000000059604645f));if(offset>0.8999999761581421f)offset=0.8999999761581421f;}}
   else{if(y>10){if(mark==5)offset=0.05000000074505806f;}else{offset=(float)((mark==5?1.0499999523162842f:1f)-(double)y/10);if(offset>0.8999999761581421f)offset=0.8999999761581421f;}}
   return (float)((double)mark+offset);
  }
 }
}
