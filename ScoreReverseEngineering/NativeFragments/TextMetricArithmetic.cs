namespace ScoreModern.RecoveredFragments {
 public struct TextMetricScale {public float Character,ExtraSpace;}
 public static class TextMetricArithmetic {
  // Relative 14A6:0872..0882: standard-font metric adaptation for the
  // primitive stroke path. No binary32 store occurs between multiply/divide.
  public static float StrokeHorizontal(float metric,float horizontal,float primitiveMetric){
   if(float.IsNaN(metric)||float.IsInfinity(metric)||float.IsNaN(horizontal)||float.IsInfinity(horizontal)||float.IsNaN(primitiveMetric)||float.IsInfinity(primitiveMetric)||primitiveMetric==0)throw new System.NotSupportedException("Requires finite metrics and nonzero primitive metric.");
   return (float)((double)metric*horizontal/primitiveMetric);
  }
  // Numerical projection of relative 020F:0052..007F and 050C..0587,
  // plus metric normalization at 14A6:0D6B..0D9C. External metrics and
  // measured constants are arguments; no font data or parser is embedded.
  // Binary32 stores are explicit. This is not an x87 flags/exception model.
  public static TextMetricScale Prepare(float size,float staff,float divisor,float factor,float extraSpace){
   double initial=(double)size*staff/divisor;
   float stored=(float)initial;
   return new TextMetricScale{Character=(float)((double)factor*stored),ExtraSpace=(float)(initial*extraSpace)};
  }
  public static float Normalize(short width,float divisor){return (float)((double)width/divisor);}
  public static float Advance(float metric,TextMetricScale scale,bool space){
   float result=(float)((double)scale.Character*metric);
   return space?(float)((double)result+scale.ExtraSpace):result;
  }
  public static float Accumulate(float total,float advance){return (float)((double)total+advance);}
  public static float Finish(float total,float multiplier){return (float)((double)multiplier*total);}
  // Relative 14A6:0B16..0B36. The index is the state at this leaf's entry;
  // the output callee may replace it before this leaf is reached.
  public static float AddPosition(short index,float x,float advance,float units){return index>160?x:(float)((double)advance*units+x);}
 }
}
