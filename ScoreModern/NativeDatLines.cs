using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatLines {
  static float F(double v){return (float)v;}
  static short W(float v){return unchecked((short)(int)Math.Truncate(v));}
  static bool BitsEqual(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}
  static float B(uint bits){return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);}
  // Recovered Code4 waveform algorithm tables: original runtime DS:039A/0492.
  static readonly float[] WaveX={B(0xC1120000u),B(0x00000000u),B(0x4184B852u),B(0x4204B958u),B(0x42471687u),B(0x4284B9DBu),B(0x42A5E873u),B(0x42C71687u),B(0x42E845A2u),B(0x431742D1u),B(0x4382A168u),B(0x431742D1u),B(0x4382A168u),B(0x431742D1u)};
  static readonly float[] WaveY={B(0x00000000u),B(0x00000000u),B(0x411AD917u),B(0x41E845A2u),B(0x421AD917u),B(0x421AD917u),B(0x41E845A2u),B(0x411AD917u),B(0x00000000u),B(0x00000000u),B(0x00000000u),B(0x42680000u),B(0x00000000u),B(0xC2680000u)};
  static bool Present(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0)!=0;}
  static void Add(List<PrimitiveTextPoint> a,float x,float y,short op){a.Add(new PrimitiveTextPoint{X=NativeNotationContext.Word(x,true),Y=NativeNotationContext.Word(y,false),Operation=op});}
  internal static void Hook(List<PrimitiveTextPoint> a,float x,float y,float control,float unit,ref short op){
   if(control<=100){Add(a,x,F((double)y+control*(double)unit),op);op=0;return;}
   if(control>=200){double h=F((double)control-200)*(double)unit;Add(a,x,F((double)y-h),op);op=0;Add(a,x,F((double)y+h),0);return;}
   float height=F(((double)control-100)*unit);if(op==0)height=-height;float spread=F((double)height*.375);float arm=F((double)x+height);Add(a,arm,F((double)y+spread),1);Add(a,x,y,0);Add(a,arm,F((double)y-spread),0);
  }
  internal static void Rotate(List<PrimitiveTextPoint> a,float degrees,float x,float y){
   if(!Present(degrees))return;// Original 0E38:09DA..09FC converts each pivot to a signed dword, not a coordinate word.
   if(Single.IsNaN(x)||Single.IsNaN(y)||Single.IsInfinity(x)||Single.IsInfinity(y)||(double)x<Int32.MinValue||(double)x>Int32.MaxValue||(double)y<Int32.MinValue||(double)y>Int32.MaxValue)throw new NotSupportedException("DAT rotation pivot outside the verified signed-dword conversion domain.");int ox=(int)Math.Truncate(x),oy=(int)Math.Truncate(y);double radians=degrees/57.2957763671875;float sin=F(Math.Sin(radians)),cos=F(Math.Cos(F(radians)));
   for(int n=0;n<a.Count;n++){var q=a[n];float dx=q.X-ox,dy=q.Y-oy;double xx=(double)dx*cos+(double)dy*sin+ox,yy=(double)dy*cos-(double)dx*sin+oy;if(xx< -32600)throw new NotSupportedException("Line rotation creates a reserved SCORE command word at the negative coordinate boundary; that invalid buffer state is guarded.");q.X=NativeLibrarySymbols.RotatedWord(xx);q.Y=NativeLibrarySymbols.RotatedWord(yy);a[n]=q;}
  }
  // Original relative1614:0EFC: copy original buffered path at alternating14-word offsets.
  internal static void Thickness(List<PrimitiveTextPoint> points,short encoded){
   if(encoded<=0)return;
   int vertical=encoded/100,horizontal=encoded%100;var original=points.ToArray();
   if((long)original.Length*(1+vertical+horizontal)>30000)throw new NotSupportedException("DAT thickness exceeds the verified buffer domain.");
   int words=0,copy=0;
   foreach(var p in original){if(p.Operation>0||p.CommandOnly)words++;if(!p.CommandOnly)words++;}
   for(int axis=0;axis<2;axis++){int count=axis==0?vertical:horizontal;for(int n=1;n<=count;n++){
    int offset=((n+1)/2)*14*(n%2==1?1:-1);
    copy++;
    foreach(var source in original){
     var q=source;
     if(axis==0)q.Y=unchecked((short)((short)q.Y+offset));else q.X=unchecked((short)((short)q.X+offset));
     if(q.Operation>0||q.CommandOnly)q.MarkerWordBack=copy*words;
     points.Add(q);
    }
   }}
  }
  internal static bool SupportedStraightAuxiliary(Item item){
   // P19=-1 is retained in supplied SCOR4 line records. Original SCOR4 EPS
   // tests confirm that it does not alter the printed straight-line path.
   // P15 is unused on straight lines; original AUX0/AUX1 EPS agrees for signed values.
   if(item.P(20)!=0||item.P(19)!=0&&item.P(19)!=-1)return false;
   return item.P(16)==1||item.P(16)==0&&item.P(17)==0&&item.P(18)==0;
  }
  static void Validate(Item i,NativeNotationContext p){
   bool straight=i.Code==4&&i.P(7)==0&&i.P(5)<999;
   if(straight&&!SupportedStraightAuxiliary(i))throw new NotSupportedException("DAT straight-line auxiliary controls need verification.");
   for(int n=8;n<=20;n++){if(n==10||straight&&n>=15)continue;if(n==8||n==9||n==11||n==12||n==13||n==14)continue;if(i.P(n)!=0)throw new NotSupportedException("DAT line auxiliary controls need verification.");}
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT line inputs outside the measured finite domain.");if(p.Scale<=0)throw new NotSupportedException("DAT line staff scale needs verification.");
  }
  public static List<PrimitiveTextPoint> Hairpin(Item i,NativeNotationContext p){
   NativeHairpins.ValidateTrims(i);
   if(!NativeHairpins.SupportedAuxiliary(i))throw new NotSupportedException("DAT hairpin auxiliary controls need verification.");
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT hairpin inputs outside the measured finite domain.");
   float spread=i.P(7);if(!Present(spread))spread=2.299999952316284f;if(spread==-1)spread=-2.299999952316284f;
   // SCOR4 P16=1: P17/P18 are signed SCORE-unit displacements of
   // the respective horizontal endpoints, independent of staff size.
   float left=p.Prepared.X,right=F(i.P(6)*150.0),y=p.Prepared.Y;
   if(i.P(16)==1){left=F((double)left+i.P(17)*150.0);right=F((double)right+i.P(18)*150.0);}
   float half=F(Math.Abs((double)spread)*p.Scale*3.5);bool negative=BitConverter.ToInt32(BitConverter.GetBytes(spread),0)<0;
   float open=negative?left:right,tip=negative?right:left,dx=F((double)tip-open),start=i.P(14),stop=i.P(15),sx=Present(start)?F((double)dx*start):0,ex=Present(stop)?F((double)dx*stop):0,sh=Present(start)?F((double)start*half):0,eh=Present(stop)?F((double)stop*half):0;var a=new List<PrimitiveTextPoint>();
   if(!negative&&Present(start)){
    float from=F((double)tip-sx),to=Present(stop)?F((double)tip-ex):open,toHeight=Present(stop)?eh:half;
    Add(a,from,F((double)y+sh),1);Add(a,to,F((double)y+toHeight),0);Add(a,from,F((double)y-sh),1);Add(a,to,F((double)y-toHeight),0);
   }else if(negative&&Present(stop)){
    float from=F((double)open+ex),to=Present(start)?F((double)open+sx):open,fromHeight=F((double)half-eh),toHeight=Present(start)?F((double)half-sh):half;
    Add(a,from,F((double)y+half-eh),1);Add(a,to,F((double)y+toHeight),0);Add(a,from,F((double)y-fromHeight),1);Add(a,to,F((double)y-toHeight),0);
   }else{
    if(negative&&Present(start)){open=F((double)open+sx);half=F((double)half-sh);}else if(!negative&&Present(stop)){open=F((double)tip-ex);half=eh;}
    Add(a,open,F((double)y+half),1);Add(a,tip,y,0);Add(a,open,F((double)y-half),0);
   }
   Rotate(a,W(i.P(9))!=0?i.P(9):i.P(13),left,y);return a;
  }
  public static List<PrimitiveTextPoint> MultiPoint(Item i,NativeNotationContext p){
   for(int n=18;n<=20;n++)if(i.P(n)!=0)throw new NotSupportedException("DAT multipoint auxiliary controls need verification.");for(int n=3;n<=17;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT multipoint input outside the measured finite domain.");
   var a=new List<PrimitiveTextPoint>();Add(a,p.Prepared.X,p.Prepared.Y,1);for(int n=8;n<18;n+=2){if(!Present(i.P(n)))break;Add(a,F((double)i.P(n)*150),p.Vertical(i.P(n+1)),0); }Add(a,F((double)i.P(6)*150),p.Vertical(i.P(5)),0);return a;
  }
  public static List<PrimitiveTextPoint> Straight(Item i,NativeNotationContext p){
   Validate(i,p);var a=new List<PrimitiveTextPoint>();float x=p.Prepared.X,y=p.Prepared.Y,rightX=F((double)i.P(6)*150),rightY=p.Vertical(i.P(5)),unit=F((double)p.Scale*7);short op=1;
   if(i.P(16)==1){x=F((double)x+i.P(17)*150.0);rightX=F((double)rightX+i.P(18)*150.0);}
   if(Present(i.P(11)))Hook(a,x,y,i.P(11),unit,ref op);Add(a,x,y,op);Add(a,rightX,rightY,0);op=0;if(Present(i.P(12)))Hook(a,rightX,rightY,i.P(12),unit,ref op);
   Thickness(a,unchecked((short)(W(i.P(10))*(Math.Abs((double)rightX-x)>=Math.Abs((double)rightY-y)?100:1))));Rotate(a,W(i.P(9))!=0?i.P(9):i.P(13),x,y);return a;
  }
  public static List<PrimitiveTextPoint> Waves(Item i,NativeNotationContext p){
   int kind=W(i.P(7));if(kind>=0)throw new NotSupportedException("DAT waveform type outside the verified route.");
   foreach(int n in new[]{10,11,14,16,17,18,19,20})if(!(n==10||n==11&&kind==-3)&&i.P(n)!=0)throw new NotSupportedException("DAT waveform auxiliary controls need verification.");
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n))||Math.Abs(i.P(n))>32767)throw new NotSupportedException("DAT waveform input outside the measured finite domain.");
   if(p.Scale<=0)throw new NotSupportedException("DAT waveform staff scale needs verification.");
   float width=i.P(8)>0?i.P(8):1,height=Present(i.P(9))?i.P(9):width,unit=F((double)p.Scale*.08250000327825546f),sx=F((double)unit*width),sy=F((double)unit*height);
   float x=p.Prepared.X,y=p.Prepared.Y,right=F((double)i.P(6)*150),angle=i.P(13);bool vertical=BitsEqual(i.P(3),i.P(6))||!Present(i.P(6));
   if(vertical){if(Present(i.P(15)))x=F((double)i.P(15)*150+x);angle=-90;float dy=F((double)i.P(5)-i.P(4));if(W(i.P(12))<0){dy=-dy;y=F((double)p.Vertical(i.P(5))+p.Scale*14);}float length=F((double)p.Scale*dy*7);if(length<0){length=-length;angle=-angle;}right=F((double)x+length);}
   var a=new List<PrimitiveTextPoint>();if(!(right>x))return a;float originX=x,originY=y;
   if(kind==-3){
    float step=F((double)sx*72),rise=F((double)sy*84),yy=y,shift=0,drop=0;
    if(Present(i.P(11))){float thick=F(p.Scale/25.0*i.P(11)*50);double radians=Math.Atan((double)sy/sx);float storedAngle=F(radians);shift=F(thick/Math.Sin(radians));drop=F(Math.Tan(storedAngle)*(double)shift*7/6);if(Single.IsNaN(shift)||Single.IsInfinity(shift)||Single.IsNaN(drop)||Single.IsInfinity(drop))throw new NotSupportedException("DAT triangular thickening singularity needs fault verification.");}
    if(!(step>0))throw new NotSupportedException("DAT triangular waveform step needs verification.");Add(a,x,y,1);
    for(int n=0;n<30000;n++){x=F((double)x+step);yy=yy==y?F((double)y+rise):y;if(!(yy==y&&Present(i.P(11))))Add(a,x,yy,0);
     if(yy==y&&Present(i.P(11))){Add(a,x,y,2);float shifted=F((double)x-shift);Add(a,shifted,F((double)y-drop),0);Add(a,F((double)shifted-step),F((double)F((double)y+rise)-drop),0);Add(a,F((double)x-step),F((double)y+rise),0);Add(a,x,y,0);Add(a,x,y,1);}
     if(!(x<right)){Thickness(a,unchecked((short)(W(i.P(10))*100)));Rotate(a,angle,originX,originY);return a;}}
    throw new NotSupportedException("DAT triangular waveform exceeds the verified buffer domain.");}
   float delta=F((double)sx*146);if(!(delta>0))throw new NotSupportedException("DAT waveform step needs verification.");int start=0,end=8;short op=1;
   for(int n=0;n<10000;n++){for(int k=start;k<=end;k++){Add(a,F((double)WaveX[k]*sx+x),F((double)WaveY[k]*sy+y),op);op=0;}if(a.Count>30000)throw new NotSupportedException("DAT waveform exceeds the verified buffer domain.");x=F((double)x+delta);start=1;if(!(x<right)){Thickness(a,unchecked((short)((kind==-1?0:W(i.P(10)))*100)));Rotate(a,angle,originX,originY);return a;}if(!((double)x+delta<right))end=W(i.P(12))==0?9:13;}
   throw new NotSupportedException("DAT waveform loop exceeds the verified buffer domain.");
  }
  public static List<PrimitiveTextPoint> Dashes(Item i,NativeNotationContext p){
   if(i.Code!=4||i.P(5)>=999||(W(i.P(7))<=0||W(i.P(7))>=100))throw new NotSupportedException("Requires the recovered DAT dash route.");
   Validate(i,p);
   foreach(float v in new[]{i.P(3),i.P(4),i.P(5),i.P(6),i.P(8),i.P(9),p.Scale})if(Single.IsNaN(v)||Single.IsInfinity(v)||Math.Abs(v)>32767)throw new NotSupportedException("DAT dash inputs outside the measured finite domain.");
   if(p.Scale<=0)throw new NotSupportedException("DAT dash staff scale needs verification.");
   float unit=F((double)p.Scale*7),length=F((i.P(8)>0?i.P(8):1)*(double)unit),gap=F(i.P(9)*(double)unit);if(!(gap>0))gap=length;
   if(!(length>0)||!(gap>0))throw new NotSupportedException("DAT dash spacing underflow needs original fault verification.");
   float x=p.Prepared.X,y=p.Prepared.Y,rightX=F((double)i.P(6)*150),rightY=p.Vertical(i.P(5));var points=new List<PrimitiveTextPoint>();
   Action<float,float,short> point=(xx,yy,op)=>points.Add(new PrimitiveTextPoint{X=NativeNotationContext.Word(xx,true),Y=NativeNotationContext.Word(yy,false),Operation=op});
   if(BitsEqual(i.P(3),i.P(6))){
    float dy=F((double)rightY-y);double period=(double)length+gap;int count=(int)Math.Truncate(dy/period);if(count<=0)return points;if(count>10000)throw new NotSupportedException("DAT dash count exceeds the verified buffer domain.");
    double ratio=(count+1.0)*length/gap;float savedRatio=F(ratio);double total=ratio*dy/((double)savedRatio+count);float savedTotal=F(total);length=F(total/(count+1.0));gap=F(((double)dy-savedTotal)/count);
    for(int n=0;n<=10000;n++){point(x,y,1);y=F((double)y+length);point(x,y,0);double next=(double)y+gap;y=F(next);if(!(next<rightY)){Thickness(points,W(i.P(10)));return points;}}throw new NotSupportedException("DAT vertical dash loop exceeds the verified buffer domain.");
   }
   short firstOp=1;if(Present(i.P(11)))Hook(points,x,y,i.P(11),unit,ref firstOp);
   for(int n=0;n<=10000;n++){point(x,y,firstOp);firstOp=1;x=F((double)x+length);point(x,y,0);double next=(double)x+gap;x=F(next);if(!(next<rightX))break;if(n==10000)throw new NotSupportedException("DAT horizontal dash loop exceeds the verified buffer domain.");}
   short endOp=0;if(Present(i.P(12)))Hook(points,F((double)x-gap),y,i.P(12),unit,ref endOp);
   Thickness(points,unchecked((short)(W(i.P(10))*100)));
   if(W(i.P(4))!=W(i.P(5))&&!Present(i.P(13))){float slope=F(((double)rightY-y)/((double)x-gap-p.Prepared.X));if(Single.IsNaN(slope)||Single.IsInfinity(slope))throw new NotSupportedException("DAT dash slope singularity needs original fault verification.");short origin=(short)points[0].X;for(int n=0;n<points.Count;n++){var q=points[n];short dx=unchecked((short)((short)q.X-origin));short yy=unchecked((short)((short)q.Y+(short)(long)Math.Truncate((double)dx*slope)));if(yy>32750)throw new NotSupportedException("DAT sloped dash postprocessing exceeds the verified coordinate domain.");q.Y=yy;points[n]=q;}}
   Rotate(points,i.P(13),p.Prepared.X,p.Prepared.Y);return points;
  }
 }
}
