using System;
namespace ScoreModern.RecoveredFragments {
 public static class LayoutVerticalContour {
  static short W(int value){return unchecked((short)value);}
  static void Put(short[] minimum,short[] maximum,int bin,short low,short high){if(bin<0||bin>100)throw new NotSupportedException("Original VJ contour writes beyond the 101-cell table.");if(maximum[bin]<high)maximum[bin]=high;if(low<minimum[bin])minimum[bin]=low;}
  // Original 2C5C:057A..0892. Inputs are original one-based X/Y word
  // streams and item-vector offsets, not pixels or bounding rectangles.
  public static void Apply(short[] x,short[] y,int first,int next,bool skipGuide,bool initialize,short[] minimum,short[] maximum,short binWidth,short bias){
   if(x==null||y==null||minimum==null||maximum==null||minimum.Length<101||maximum.Length<101||binWidth<=0)throw new NotSupportedException("Original VJ contour input leaves its verified finite domain.");
   if(initialize)for(int i=0;i<101;i++){minimum[i]=32760;maximum[i]=-32760;}
   if(first==next)return;int start=first+(skipGuide?4:1),end=next-2;
   for(int i=start;i<=end;i++){
    if(i<1||i+1>=x.Length||i>=y.Length)throw new NotSupportedException("Original VJ vector read leaves its bank.");
    short rawA=x[i],rawB=x[i+1];if(rawA< -32600||rawB< -32600||(rawA<0&&rawB<0)||(rawA>30000&&rawB>30000))continue;
    int a=W(rawA+bias)/binWidth,b=W(rawB+bias)/binWidth;short high=y[i-1],low=y[i];
    if(a==b){if(high<low){short t=high;high=low;low=t;}}
    else if(high!=low){
     short distance=W((b-a)*binWidth);if(distance<1){a=b;short t=high;high=low;low=t;distance=W(-distance);}
     if(distance<=0)throw new NotSupportedException("Original VJ interpolation distance is not a positive finite word.");
     float slope=(float)((double)W(low-high)/distance);int count=W(distance+binWidth)/binWidth;short step=0;
     for(int n=0;n<count;n++,a++,step=W(step+binWidth))if(a>=0&&a<101){double value=(double)step*slope+high;if(value< -32768||value>=32768)throw new NotSupportedException("Original VJ interpolation conversion leaves signed16 domain.");short point=W((int)Math.Truncate(value));Put(minimum,maximum,a,point,point);}continue;
    }else if(a>b){int t=a;a=b;b=t;}
    for(int bin=a;;bin++){if(bin>=0){Put(minimum,maximum,bin,low,high);if(bin==b||bin==100)break;}if(bin>32767)throw new NotSupportedException("Original VJ contour iteration leaves its finite domain.");}
   }
  }
 }
}
