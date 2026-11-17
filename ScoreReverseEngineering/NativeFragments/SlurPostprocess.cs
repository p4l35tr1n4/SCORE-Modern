using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class SlurWordSegment {
  readonly byte[] memory;readonly int origin;public int Length{get{return 32768;}}
  public SlurWordSegment(byte[] bytes,int address){memory=bytes;origin=address;}
  public short this[int index]{get{int a=(origin+((index&32767)*2))&0xfffff;return unchecked((short)(memory[a]|memory[(a+1)&0xfffff]<<8));}set{int a=(origin+((index&32767)*2))&0xfffff;memory[a]=(byte)value;memory[(a+1)&0xfffff]=(byte)(value>>8);}}
 }
 public sealed class SlurVectorBuffer {
  public readonly SlurWordSegment X,Y;public int Capacity=31000;
  public SlurVectorBuffer():this(0,4063){}
  public SlurVectorBuffer(ushort xSegment,ushort ySegment){var memory=new byte[1048576];X=new SlurWordSegment(memory,xSegment*16);Y=new SlurWordSegment(memory,ySegment*16);}
public int Base=16,End,CurveEnd,SampleCount;public float Angle,Sin,Cos;
  public void Copy(int destination,int source){X[destination&32767]=X[source&32767];Y[(destination-1)&32767]=Y[(source-1)&32767];}
  public void Move(int destination,int source,int count){int step=count>0?1:-1;int n=Math.Abs(count);if(n>32768)throw new NotSupportedException("Slur move count exceeds segment.");for(int i=0;i<n;i++)X[(destination+step*i)&32767]=X[(source+step*i)&32767];for(int i=0;i<n;i++)Y[(destination-1+step*i)&32767]=Y[(source-1+step*i)&32767];}
  public short[] ExtractX(){var a=new short[Math.Max(0,End-Base)];for(int i=0;i<a.Length;i++)a[i]=X[Base+i];return a;}
  public short[] ExtractY(){var a=new short[Math.Max(0,End-Base-1)];for(int i=0;i<a.Length;i++)a[i]=Y[Base+i];return a;}
 }
 public static class SlurPostprocess {
  static float F(double v){return (float)v;}static int T(double v){return checked((int)Math.Truncate(v));}static int W(float v){return unchecked((short)T(v));}static bool P(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  public static SlurVectorBuffer From(SlurSampleResult sample,SlurThicknessResult thickness,bool printed){var b=new SlurVectorBuffer();short[] x=printed?thickness.X:sample.X,y=printed?thickness.Y:sample.Y;b.End=b.Base+x.Length+1;b.CurveEnd=b.Base+sample.Count;b.SampleCount=sample.Count;b.X[b.Base]=unchecked((short)(printed?0x8046:0x8045));for(int i=0;i<x.Length;i++)b.X[b.Base+i+1]=x[i];for(int i=0;i<y.Length;i++)b.Y[b.Base+i]=y[i];return b;}
  // Relative 1149:0AB3..0F15. Coordinates and control words share the X buffer.
  public static void Pattern(SlurVectorBuffer b,float[] p,float scale,bool printed){
   int control=W(p[11]);if(control==0)return;int start=b.Base,n=b.SampleCount;
   if(p[11]<0){
    int scratch=b.Capacity-399;if(b.X[0]>b.Capacity-400)return;int count=b.CurveEnd-start;for(int k=0;k<count;k++)b.Copy(scratch+k,start+1+k);
    float size=scale;if(p[13]>0)size=F((double)p[13]*size);
    float[] table={-1.6f,-1.6f,-.6f,.6f,1.6f,1.6f,.6f,-.6f,-1.6f,-1.6f,-.6f};short[] offsets=new short[11];for(int k=0;k<11;k++)offsets[k]=unchecked((short)Math.Round((double)table[k]*size,MidpointRounding.AwayFromZero));
    b.End=start-1;for(int k=0;k<count;k++){
     b.End++;b.X[b.End]=unchecked((short)0x8046);int x=b.X[scratch+k],y=b.Y[scratch+k-1];
     for(int j=0;j<9;j++){b.End++;b.X[b.End]=unchecked((short)(x+offsets[j]));b.Y[b.End-1]=unchecked((short)(y+offsets[j+2]));}
    }
    b.CurveEnd=b.End;b.End++;return;
   }
   if(!printed){int extra=(n-1)/2;b.End+=extra;b.CurveEnd+=extra;for(int i=b.CurveEnd;i>=start+4;i-=3){int source=i-extra;b.Copy(i,source);b.Copy(i-1,source-1);b.X[i-2]=unchecked((short)0x8045);extra--;}return;}
   int originalEnd=b.End,last=originalEnd-2,output=originalEnd;b.Copy(output,last);output++;b.Copy(output,start+1);int lo=start+1,hi=originalEnd-2;output++;b.X[output]=unchecked((short)0x8046);hi-=2;
   for(int guard=0;hi>b.CurveEnd&&guard<10000;guard++){
    lo+=2;int source=lo;for(int j=1;j<=5;j++){output++;b.Copy(output,source);source++;if(j==2)source=hi;if(j==4)source=lo;}
    output++;b.X[output]=unchecked((short)0x8046);hi-=2;
   }
   int tail=b.CurveEnd-1;for(int j=1;j<=4;j++){output++;b.Copy(output,tail);tail++;if(j==3)tail=b.CurveEnd-1;}
   int countOut=output-originalEnd+1;b.Move(start+3,originalEnd,countOut);b.End=start+3+countOut;
  }
  // Relative 1149:0F15..1132; effective start and stop are working values.
  public static void Trim(SlurVectorBuffer b,float[] p,float start,float stop,bool printed){
   if((!P(start)&&!P(stop))||p[11]<0)return;int total=b.CurveEnd-b.Base;
   int first=T((double)(float)total*start+.51f),last=T((double)(float)total*stop+.5);if(last==0)last=total;int count=last-first;
   if(printed){
    if(W(p[11])!=0){first/=2;last/=2;if(first>0)first=b.Base+first*6;last=b.Base+last*6-1;if(last>b.End)last=b.End;if(first>0){count=last-first;b.Move(b.Base+1,first,count);b.End=b.Base+1+count;}else b.End=last;return;}
    if(first>0){int right=b.End-first,source=b.Base+first+1;b.Copy(right,source);if(last!=total){count+=2;int other=b.End-last,target=b.Base+last+1;b.Move(target,other,count);count=count*2-3;}else count=right-source+1;b.Move(b.Base+1,source,count);b.End=b.Base+1+count;return;}
    int from=b.End-count,to=b.Base+count+1;b.Move(to,from,count);count*=2;b.End=b.Base+1+count;return;
   }
   b.Move(b.Base+1,b.Base+1+first,count);b.End=b.Base+1+count;
  }
  // Relative 0614:0989..0B5A, called with a negative buffer index (radians).
  public static void Rotate(SlurVectorBuffer b,SlurOriginState o){
   if(!P(o.DeltaY))return;b.Angle=F(Math.Atan2(o.DeltaY,o.HorizontalSpan));double angle=-(double)b.Angle;b.Sin=F(Math.Sin(angle));b.Cos=F(Math.Cos(F(angle)));int ox=T(o.LeftX),oy=T(o.LeftY);
   for(int i=b.Base;i<b.End;i++){
    if(b.X[i]<-32600)continue;float x=F(b.X[i]-ox),y=F(b.Y[i-1]-oy);int nx=T((double)x*b.Cos+(double)y*b.Sin+ox),ny=T((double)y*b.Cos-(double)x*b.Sin+oy);b.X[i]=(short)Math.Max(-32750,Math.Min(32750,nx));b.Y[i-1]=(short)Math.Max(-32750,Math.Min(32750,ny));
   }
  }
 }
}
