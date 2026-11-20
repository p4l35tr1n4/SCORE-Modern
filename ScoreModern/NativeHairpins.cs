using System;using System.Collections.Generic;using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed class NativeHairpinPlan {public List<BarlinePoint> Points=new List<BarlinePoint>();public short WidthSelector;public float Width;}
 public static class NativeHairpins {
  static float F(double x){return (float)x;}
  // Relative 04ED:0009..008F, 01B3..026D, 04F2..056B.
  // Printed P9 rotation uses the same buffered point transform as the DOS screen path.
  internal static void ValidateTrims(Item item){if(NativeSlurDisplay.RejectInvalidTrims&&(item.P(14)<0||item.P(14)>1||item.P(15)<0||item.P(15)>1||item.P(14)!=0&&item.P(15)!=0&&item.P(14)>item.P(15)))throw new NotSupportedException("Hairpin trim range is reversed or outside0-1. The original trim-behaviour option retains its measured extrapolated geometry.");}
  internal static bool SupportedAuxiliary(Item item){
   // Original SCOR4 AUX0/AUX1: P8 is unused for hairpins (tested 0, +/-.4, 1, 5, 99).
   foreach(int n in new[]{11,12,19,20})if(item.P(n)!=0)return false;
   return item.P(16)==1||item.P(16)==0&&item.P(17)==0&&item.P(18)==0;
  }
  public static NativeHairpinPlan Prepare(Item i,NativeNotationContext p){
   if(p==null||i.Code!=4||Single.IsNaN(i.P(5))||i.P(5)<999||i.P(5)>32767||i.P(7)<-32767||i.P(7)>32767||Single.IsNaN(i.P(7))||i.P(10)<-32767||i.P(10)>32767||Single.IsNaN(i.P(10))||Single.IsNaN(i.P(6))||Single.IsInfinity(i.P(6)))return null;
   if(!SupportedAuxiliary(i))return null;
   if(Single.IsNaN(i.P(9))||Math.Abs(i.P(9))>32767||Single.IsNaN(i.P(13))||Math.Abs(i.P(13))>32767)return null;
   ValidateTrims(i);
   short selector=(short)((int)i.P(7)==-1?0:(int)i.P(10));var result=new NativeHairpinPlan{WidthSelector=selector,Width=NativePrintContext.Width((short)(selector*2))};foreach(var point in NativeDatLines.Hairpin(i,p))result.Points.Add(new BarlinePoint(point.X,point.Y,point.Operation));return result;
  }
  public static bool Draw(Graphics g,Item i,NativeNotationContext p){var plan=Prepare(i,p);if(plan==null)return false;NativeNoteDetails.Path(g,p,plan.Points,plan.Width);if(plan.WidthSelector!=0)NativePrintContext.RestoreDefaultLineWidth();return true;}
 }
}
