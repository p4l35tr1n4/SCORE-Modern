using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurGeometryResult {public SlurVectorBuffer Buffer;public TupletPlacement? Number;public int Root;}
 public static class SlurGeometry {
  static float F(double v){return (float)v;}static int W(float v){return unchecked((short)(int)Math.Truncate(v));}static bool P(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static void Distance(SlurOriginState o,float[] p,bool trimmed){
   float span=F((double)o.OriginalRight-o.OriginalLeft);o.HorizontalSpan=span;
   if(!trimmed&&W(p[12])>0){if(W(p[12])==1){o.RightX=F((double)span+o.RightX);o.StopFraction=.5f;}else{o.LeftX=F((double)o.LeftX-span);o.EffectiveStartFraction=.5f;o.LeftY=F((double)o.LeftY-o.DeltaY);}span=F(2.0*span);o.DeltaY=F(2.0*o.DeltaY);}
   o.Length=span;if(BitConverter.ToInt32(BitConverter.GetBytes(p[4]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(p[5]),0))o.Length=F(Math.Sqrt((double)o.DeltaY*o.DeltaY+(double)span*span));
  }
  static void Put(SlurVectorBuffer b,SlurSampleResult sample,SlurThicknessResult thick,bool printed){short[] x=printed?thick.X:sample.X,y=printed?thick.Y:sample.Y;b.X[b.Base&32767]=unchecked((short)(printed?0x8046:0x8045));for(int j=0;j<x.Length;j++){b.X[(b.Base+j+1)&32767]=x[j];b.Y[(b.Base+j)&32767]=y[j];}b.End=unchecked((short)(b.Base+x.Length+1));b.CurveEnd=unchecked((short)(b.Base+sample.Count));b.SampleCount=sample.Count;}
  public static SlurGeometryResult Build(float[] parameters,SlurOriginContext context,bool printed,float configuredControl,SlurVectorBuffer caller,bool rejectInvalidTrims){
   if(parameters==null||parameters.Length<21)throw new ArgumentException("Missing slur parameters.");
   float[] p=(float[])ScoreVersionControls.SlurParameters(context.Scor4,parameters).Clone();bool straight=W(p[9])==99;
   if(rejectInvalidTrims&&(p[14]<0||p[14]>1||p[15]<0||p[15]>1||(p[15]!=0&&p[14]>p[15])))throw new ArgumentException("Slur trim must be an ordered interval within 0..1.");
   bool widthPrefix=printed&&(straight?W(p[13])!=0:W(p[11])==0);var b=caller??new SlurVectorBuffer{End=printed?(straight?1:(widthPrefix?2:1)):20};int root=b.End;b.Base=root;b.X[0]=unchecked((short)root);if(widthPrefix){int prefix=straight?root:root-1;b.X[prefix]=unchecked((short)0x80a6);b.Y[prefix-1]=unchecked((short)(straight?W(p[13])*2:-1));if(straight){b.Base=root+1;b.X[0]=unchecked((short)b.Base);}}var o=SlurOrigins.Prepare(p,context);if(ScoreVersionControls.SlurEndpointOverride(context.Scor4,p)){p[16]=p[17]=p[18]=0;}int number=W(p[16]),split=number;float gap=0;
   if(!straight&&number>0&&W(p[11])!=0){number=-number;split=number;}
   if(straight){
    Distance(o,p,P(o.EffectiveStartFraction)||P(o.StopFraction));int at=b.Base;b.X[at]=unchecked((short)0x8045);
    b.X[at+1]=unchecked((short)(int)Math.Truncate(o.LeftX));b.X[at+3]=unchecked((short)(int)Math.Truncate((double)o.LeftX+o.Length));b.X[at+2]=unchecked((short)(int)Math.Truncate((double)o.Length*o.CurveCenter+o.LeftX));
    int lineY=(int)Math.Truncate(o.LeftY);b.Y[at]=unchecked((short)lineY);b.Y[at+2]=unchecked((short)lineY);b.Y[at+1]=unchecked((short)(int)Math.Truncate((double)lineY+o.Height));b.End=unchecked((short)(at+4));b.CurveEnd=unchecked((short)(at+3));b.SampleCount=3;
   }else for(int pass=0;pass<2;pass++){
    bool trimmed=P(o.EffectiveStartFraction)||P(o.StopFraction);Distance(o,p,trimmed);
    if(split>0){o.CurveCenter=.5f;double value=(double)context.Scale*(Math.Abs(number)<10?11.4f:16.2f)/o.Length;gap=F(value);o.StopFraction=F(.5-value);}
    var sample=SlurSampling.BuildState(p,context,o,printed,trimmed);SlurThicknessResult thick=null;
    if(printed){thick=SlurThickness.Build(p,context,sample,configuredControl);p[13]=thick.EffectiveControl;SlurThickness.Reflect(p,sample,thick.Y,true);}else SlurThickness.Reflect(p,sample,sample.Y,false);
    Put(b,sample,thick,printed);SlurPostprocess.Pattern(b,p,context.Scale,printed);SlurPostprocess.Trim(b,p,o.EffectiveStartFraction,o.StopFraction,printed);
    if(split>0){split=0;o.StopFraction=0;o.EffectiveStartFraction=F((double)gap+.5);b.Base=b.End;continue;}break;
   }
   b.Base=root;b.X[0]=unchecked((short)b.End);SlurPostprocess.Rotate(b,o);var result=new SlurGeometryResult{Buffer=b,Root=root};
   if(number==0)return result;short textNumber=unchecked((short)Math.Abs(number));bool negativeHeight=BitConverter.ToInt32(BitConverter.GetBytes(p[7]),0)<0;float offset;
   if(number>0)offset=P(o.DeltaY)?9:0;else offset=P(o.DeltaY)?(negativeHeight?-5:23):(negativeHeight?-14:14);
   float h=F((double)o.Height-(double)context.Scale*offset),left=o.LeftX,y,along;
   if(P(o.DeltaY)){
    float sin=F(Math.Sin(b.Angle)),cos=F(Math.Cos(b.Angle));double half=(double)o.HorizontalSpan*.5;float savedHalf=F(half);
    left=F(half*cos-(double)h*sin+left);y=F((double)h*cos+(double)savedHalf*sin+o.LeftY);along=0;
   }else{
    y=F((double)h+o.LeftY);along=F((double)o.Length*o.CurveCenter);int control=W(p[11]);if(control< -3)control/=2;control=Math.Abs(control);
    if(control!=0){if(control==3)return result;float vertical=F(-(double)context.Scale*14);int leg=W(p[7]);if(control==2)leg=-leg;if(leg<0)vertical=-vertical;y=F((double)y+vertical);}
    y=F((double)y-(double)context.Scale*9);
   }
   y=F((double)p[18]*context.Scale*7+y);left=F((double)p[17]*context.Scale*6+left);
   result.Number=new TupletPlacement{Number=textNumber,FontScale=.875f,X=TupletGeometry.Center(textNumber,context.Scale,along,left),Y=y};return result;
  }
 }
}
