using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutCoordinateCollectionResult {
  public short[] References;public short FirstItem,LastItem;
 }
 // Main J's mode-zero path of original 11DA:000F/04C4. This is a word-reference
 // collector, not a geometric hit test. Nonzero collection modes are separate.
 public static class LayoutCoordinateCollection {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Original layout collector conversion leaves the verified signed-word domain.");return (short)Math.Truncate(f);}
  public static LayoutCoordinateCollectionResult Collect(float[] words,int[] itemStarts,short bottom,short top,float left,float right,float tolerance,short first,short last,short staffTableThreshold,float codeFilter,float emptyStaffControl,float compoundBarControl,int capacity,short mode=0) {
   if(mode!=0&&mode!=1&&mode!=2&&mode!=-1)throw new NotSupportedException("Original layout collection mode remains unverified.");
   if(words==null||itemStarts==null||first<1||Math.Abs((int)last)>itemStarts.Length||capacity<0)throw new NotSupportedException("Original layout collection would leave its buffers.");
   if(top<bottom)top=bottom;
   if(bottom==1&&staffTableThreshold<top)bottom=9999;
   short filter=Word(codeFilter);if(left>right){float t=left;left=right;right=t;}
   left=(float)((double)left-tolerance);right=(float)((double)right+tolerance);
   var references=new List<short>();short firstSelected=0;int limit=Math.Abs((int)last);
   Action<int> append=delegate(int value){if(references.Count>=capacity||value< -32768||value>32767)throw new NotSupportedException("Original layout coordinate-list capacity exceeded.");references.Add((short)value);};
   for(int i=first-1;i<limit;i++) {
    int w=itemStarts[i],start=w-1;if(start<0||start+3>=words.Length)throw new NotSupportedException("Original layout item leaves its buffer.");
    int count=Word(words[start]),code=Word(words[start+1]),staff=Word(words[start+2]);
    if(count<3||start+count>=words.Length)throw new NotSupportedException("Original short layout record requires its separate extension path.");
    if(bottom!=9999&&(staff<bottom||staff>top)) {if(staff>top&&last<0)last=(short)i;continue;}
    if(filter!=0&&filter!=code)continue;
    bool selected=left<=words[start+3]&&words[start+3]<=right;
    if(selected){append(w+3);if(firstSelected==0)firstSelected=(short)(i+1);}
    if(mode==1||mode>0&&!selected)continue;
    if(count<6||code<4||code>8)continue;
    // Original PAGE 227E:0219..024E addresses (one-based count index*4)+24-4: P6.
    // Staff size P5 is separate and must never receive the empty-end control.
    if(code==8&&Bits(words[start+6])==0)words[start+6]=emptyStaffControl;
    if(code==4&&count>=7&&Bits(words[start+7])<0&&Bits(words[start+6])==0)continue;
    Action<int> extra=delegate(int parameter){float x=words[start+parameter];if(left<=x&&x<=right){append(-(w+parameter));if(firstSelected==0)firstSelected=(short)(i+1);}};
    extra(6);if(count<8)continue;
    if(code==4) {
     if(words[start+7]!=compoundBarControl)continue;
     extra(8);for(int p=10;p<=count&&p<16;p+=2)extra(p);
    } else if(code==7) {
     if(Word(words[start+7])>=20&&Bits(words[start+8])>0)extra(8);
    } else if(code==6) {
     if(count<11)continue;
     if(Bits(words[start+11])>0)extra(11);
     if(Bits(words[start+7])<0)continue;
     if(count>=12&&Bits(words[start+12])>0)extra(12);
     if(count>=14&&Bits(words[start+13])!=0) {
      if(Bits(words[start+14])>0)extra(14);
      if(count>=15&&Bits(words[start+15])>0)extra(15);
     }
    }
   }
   return new LayoutCoordinateCollectionResult{References=references.ToArray(),FirstItem=firstSelected,LastItem=last};
  }
 }
}
