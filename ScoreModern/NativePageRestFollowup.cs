using System;
using System.Collections.Generic;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal static class NativePageRestFollowup {
  static short Word(float value){long result;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out result))throw new NotSupportedException("Non-finite PAGE rest controls require the original error path.");return unchecked((short)result);}
  static readonly float[] leftOffsets={0,1.2f,2.2f,4.4f,1,4.4f,3,0};
  static readonly float[] rightOffsets={0,0,0,0,3.6f,3.4f,4,0};
  // PAGE 0C16:000D..0543. Raw retained controls stay explicit until their
  // separate menu producers are verified; this routine does not name them.
  internal static List<Item> Apply(IList<Item> input,float[] staffUnits,float retained43E0,short retained43E4){
   var rows=input.Select(item=>item.Clone()).ToList();
   foreach(var rest in rows){
    if(rest.Code!=2||rest.Count<5)continue;short mode=Word(rest.P(5));if(mode>-2)continue;int oldCount=rest.Count;
    if(oldCount>=8){if(mode==-2&&rest.P(8)==1)rest.Set(8,retained43E0);if(mode<=-12&&retained43E4==1){mode=rest.P(8)>1?(short)-5:(short)-2;rest.Set(5,mode);}}
    float x=rest.P(3),staff=rest.P(2),right=9999,left=-9999;short rightStyle=0,leftStyle=0;
    foreach(var bar in rows){if(bar.Code!=14)continue;float bottom=bar.P(2);if(bottom>staff)continue;float bx=bar.Count>=3?bar.P(3):0,span=bar.Count>=4?bar.P(4):1;if((double)bottom+span<=staff)continue;
     if(bx<right&&bx>x){right=bx;rightStyle=bar.Count>=5?Word(bar.P(5)):(short)0;}
     if(bx>left&&bx<x){left=bx;leftStyle=bar.Count>=5?Word(bar.P(5)):(short)0;}
    }
    if(right==9999)continue;if((double)x-left>9){left=(float)((double)x-3);leftStyle=0;}
    int staffWord=Word(staff);if(staffWord<1||staffUnits==null||staffWord>=staffUnits.Length)throw new NotSupportedException("PAGE rest staff leaves its explicit unit table.");float unit=staffUnits[staffWord],value;
    if(mode==-5){
     if(leftStyle<0||leftStyle>=rightOffsets.Length||rightStyle<0||rightStyle>=leftOffsets.Length)throw new NotSupportedException("PAGE rest bar style reads outside the recovered offset tables.");
     right=(float)((double)right-leftOffsets[rightStyle]);left=(float)((double)left+rightOffsets[leftStyle]);value=(float)((double)right-x+left);float scale=(float)((double)unit/25);
     if(((double)right-value)/scale>3.5)value=(float)((double)right-(double)scale*3.5);
    }else{float shift=mode==-4?0.137f:0.056f;value=(float)(((double)right-left)*0.5-(double)unit*shift+left);}
    if(oldCount<8)rest.Set(8,0);rest.Set(9,value);
   }return rows;
  }
 }
}
