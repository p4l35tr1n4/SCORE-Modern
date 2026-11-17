using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatRestAttachments {
  static float F(double x){return (float)x;}
  public static List<PrimitiveTextPoint> PrepareItem(Item source,NativeNotationContext page,NativeDatGlyphs dat,DrawingLibrary library,ScoreText text){
   // The original exits before preparing attachments when the body is invisible.
   int kind=(short)(int)source.P(5);if(kind<=-20||(kind>=-10&&kind<=-8))return new List<PrimitiveTextPoint>();
   if(Invisible(source,kind))return new List<PrimitiveTextPoint>();
   var item=source.Clone();if(item.Count>=13)item.Set(13,0);if(item.Count>=17)item.Set(17,0);item.Set(5,kind);
   // Repeat signs and narrow multibar bodies skip the ordinary dot/guide pass.
   if((kind==-4&&item.P(6)>=-32768&&item.P(6)<=32767&&item.P(6)!=0)||
      ((kind==-6||kind==-7)&&item.P(6)>0&&item.P(6)<=32767))item.Set(6,0);
   if(kind==0&&item.P(8)==0){
    // These attachment-only fields do not change the unnumbered quarter-rest buffer.
    item.Set(11,0);item.Set(12,0);item.Set(14,0);item.Set(16,0);
   }
   bool explicitSize=BitConverter.ToInt32(BitConverter.GetBytes(item.P(15)),0)!=0;float scale=explicitSize?F((double)page.Scale*item.P(15)):page.Scale;
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};
   
   float effective=explicitSize?scale:Math.Abs((int)item.P(4))>=100?F((double)scale*.699999988079071):scale;
   if(item.P(10)!=0){float shift=F((double)item.P(10)*effective/25);if(kind!=-5)context.Prepared.X=F((double)shift*150+context.Prepared.X);item.Set(3,F((double)item.P(3)+shift));item.Set(10,0);}
   return NativeScanMusicVectors.Prepare(item,context,dat,library,text);
  }
  internal static bool Invisible(Item item,int kind){
   return item.P(6)<=-1&&item.P(6)>=-32768&&(kind>=-3&&kind<=10||kind>=-19&&kind<=-11);
  }
  internal static float Pitch(Item item){return item.P(15)!=0?item.P(4):item.P(4)%100;}
  internal static float EffectiveScale(Item i,NativeNotationContext p){return F((double)p.Scale*(i.P(15)!=0?i.P(15):Math.Abs((int)i.P(4))>=100?.699999988079071:1));}
  internal static float NumberWidth(int n){
   int units=n%10,tens=n%100/10,hundreds=n/100;
   if(n<10)return n==1?1.2549999952316284f:1.5499999523162842f;
   if(n<100)return n==11?2.6500000953674316f:units==1?2.950000047683716f:tens==1?3.049999952316284f:3.25f;
   if(hundreds==1&&tens==1)return units==1?4.300000190734863f:4.550000190734863f;
   if(hundreds==1||tens==1)return units==1?4.449999809265137f:hundreds==1?4.550000190734863f:4.699999809265137f;
   return units==1?4.599999904632568f:4.900000095367432f;
  }
  internal static short NumberFont(Item i,DrawingLibrary library){
   int control=(int)i.P(14);if(i.P(14)!=control)throw new NotSupportedException("Fractional rest numeral font controls need verification.");
   if(control==0){
    if(library==null)throw new NotSupportedException("External rest numeral preference unavailable.");
    control=NativePreferences.Read(library.Root).Word("REST",6);
   }
   int[] faces={2,98,90,92,0,94,91,93,1,1,3,0};
   if(control>=1&&control<=12){int face=faces[control-1];return (short)(face<90?1000+face:face);}
   if(control>=1000&&control<=1089)return (short)control;
   throw new NotSupportedException("Rest numeral font outside the measured DAT path.");
  }
  internal static bool CueEuropeanNumber(Item i){return i.P(5)>=-19&&i.P(5)<=-11&&i.P(8)>0&&i.P(15)==0&&Math.Abs((int)i.P(4))>=100;}
  public static List<PrimitiveTextPoint> EuropeanBody(Item i,NativeNotationContext page,NativeDatGlyphs dat,DrawingLibrary library=null,ScoreText text=null){
   bool cueNumber=CueEuropeanNumber(i);
   if(i.P(8)!=0&&!cueNumber||(i.P(9)<0&&i.P(9)!=-1)||i.P(6)<0||Single.IsNaN(i.P(6))||Single.IsInfinity(i.P(6)))throw new NotSupportedException("European rest controls outside the measured DAT path.");
   for(int n=10;n<=20;n++)if(i.P(n)!=0&&n!=15&&!(cueNumber&&(n==11||n==12||n==14||n==16)))throw new NotSupportedException("European rest auxiliary controls outside the measured DAT path.");
   int count=-(int)i.P(5)-10;if(count<1||count>9||i.P(5)!=(int)i.P(5))throw new NotSupportedException("European rest type outside the measured DAT path.");
   float spacing=F((double)(i.P(6)==0?1:i.P(6))*(i.P(15)!=0?F((double)page.Scale*i.P(15)):page.Scale)),origin=i.P(9)>0?F((double)i.P(9)*150):page.Prepared.X;var result=new List<PrimitiveTextPoint>();
   Action<int,int> add=(bars,offset)=>{float x=F((double)origin+offset*spacing);if(bars==1){var body=Item.Make(2,i.P(2),i.P(3),i.P(4),-2,0,4);body.Set(15,i.P(15));if(cueNumber){body.Set(8,i.P(8));body.Set(11,i.P(11));body.Set(12,i.P(12));body.Set(14,i.P(14));body.Set(16,i.P(16));}var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=x;result.AddRange(cueNumber?Number(body,context,dat,library,text):NativeScanMusicVectors.Prepare(body,context,dat));}else result.AddRange(dat.ExternalDrawing(bars==2?116:117,.8299999833106995f,.8299999833106995f,EffectiveScale(i,page),x,page.Vertical(8)));};
   EuropeanComponents(count,add);return result;
  }
  internal static void EuropeanComponents(int count,Action<int,int> add){
   switch(count){case 1:add(1,0);break;case 2:add(2,0);break;case 3:add(2,-24);add(1,24);break;case 4:add(4,0);break;case 5:add(4,-24);add(1,24);break;case 6:add(4,-24);add(2,24);break;case 7:add(4,-36);add(2,0);add(1,36);break;case 8:add(4,-24);add(4,24);break;case 9:add(4,-36);add(4,0);add(1,36);break;}
   
  }
  internal static bool TryUnnumberedLongOrigin(Item item,NativeNotationContext page,out float origin){
   // The unnumbered form requests glyph 38 at P9 instead of constructing a span.
   origin=page.Prepared.X;
   if(item.P(5)!=-5||item.P(6)!=0||item.P(8)!=0)return false;
   float end=item.P(9);
   if(Single.IsNaN(end)||Single.IsInfinity(end)||end<0)return false;
   if(Single.IsNaN(item.P(4))||Single.IsInfinity(item.P(4))||Math.Abs(item.P(4))>32767)return false;
   if(end==0&&item.P(4)!=0)return false;
   for(int field=10;field<=20;field++){
    float value=item.P(field);
    if(value==0)continue;
    if(end==0||field!=10&&field!=11&&field!=12&&field!=14&&field!=15&&field!=16)return false;
    if(Single.IsNaN(value)||Single.IsInfinity(value)||Math.Abs(value)>32767)return false;
   }
   if(end>0)origin=F((double)end*150);
   return true;
  }
  internal static List<PrimitiveTextPoint> LongBody(Item i,NativeNotationContext page){
   if((i.P(9)<0&&i.P(9)!=-1))throw new NotSupportedException("Long rest needs the measured explicit endpoints and default pitch.");
   float left=page.Prepared.X,right=F((double)i.P(9)*150),low=page.Vertical(F(5.0+Pitch(i))),high=page.Vertical(F(9.0+Pitch(i))),top=page.Vertical(F(7.75+Pitch(i))),bottom=page.Vertical(F(6.25+Pitch(i)));
   return new List<PrimitiveTextPoint>{new PrimitiveTextPoint{X=left,Y=low,Operation=1},new PrimitiveTextPoint{X=left,Y=high},new PrimitiveTextPoint{X=right,Y=low,Operation=1},new PrimitiveTextPoint{X=right,Y=high},new PrimitiveTextPoint{X=left,Y=top,Operation=2},new PrimitiveTextPoint{X=right,Y=top},new PrimitiveTextPoint{X=right,Y=bottom},new PrimitiveTextPoint{X=left,Y=bottom}};
  }
  public static List<PrimitiveTextPoint> Number(Item i,NativeNotationContext page,NativeDatGlyphs dat,DrawingLibrary library,ScoreText text){
   bool european=i.P(5)>=-19&&i.P(5)<=-11;if((!european&&(i.P(5)<-7||i.P(5)>10))||i.P(5)!=(int)i.P(5)||(!european&&(Single.IsNaN(i.P(6))||i.P(6)<0||i.P(6)>32767))||(european&&(i.P(6)<0||Single.IsNaN(i.P(6))||Single.IsInfinity(i.P(6))))||i.P(8)<1||i.P(8)>999||i.P(8)!=(int)i.P(8)||text==null)throw new NotSupportedException("Rest numeral body outside the measured DAT path.");
   for(int n=9;n<=20;n++)if(i.P(n)!=0&&n!=11&&n!=12&&n!=14&&n!=15&&n!=16&&!(n==9&&(i.P(n)==-1||(i.P(5)<=-2&&i.P(n)>0))))throw new NotSupportedException("Rest numeral auxiliary controls outside the measured DAT path.");
   if(CueEuropeanNumber(i))return EuropeanBody(i,page,dat,library,text);
   NativeNotationContext context;var body=NumeralText(i,page,library,text,out context);string value=((int)i.P(8)).ToString(System.Globalization.CultureInfo.InvariantCulture);
   var points=text.ScanStrokeText(body,value,context,0);
   var rest=i.Clone();rest.Set(8,0);rest.Set(11,0);rest.Set(12,0);rest.Set(14,0);rest.Set(16,0);if(i.P(5)==-5){var shape=LongBody(i,page);shape.AddRange(points);return shape;}var bodyPoints=NativeScanMusicVectors.Prepare(rest,page,dat,library,text);
   // Original 0F73:0C0F..0C98 emits the screen guide before the numeral.
   if(i.P(6)>=5&&i.P(5)>=-2){points.InsertRange(0,bodyPoints.GetRange(0,2));bodyPoints.RemoveRange(0,2);}
   points.AddRange(bodyPoints);return points;
  }
  internal static Item NumeralText(Item i,NativeNotationContext page,DrawingLibrary library,ScoreText text,out NativeNotationContext context){
   short font=NumberFont(i,library);float size=i.P(12)==0?1:i.P(12);if(Single.IsNaN(size)||Single.IsInfinity(size))throw new NotSupportedException("Rest numeral size outside the measured finite domain.");
   float cue=i.P(15)==0&&Math.Abs((int)i.P(4))>=100?.699999988079071f:1,scale=EffectiveScale(i,page),width=F((double)NumberWidth((int)i.P(8))*size);
   float origin=i.P(9)>0?F((double)i.P(9)*150):page.Prepared.X;if(i.P(5)==-5){LongBody(i,page);origin=F((double)i.P(9)*150);origin=F(((double)origin-page.Prepared.X)*.5-(double)scale*8+page.Prepared.X);}
   float shift=F(((i.P(5)==-4?3.4000000953674316:1.4199999570846558)-(double)width)*scale*cue*6),x=F((double)i.P(16)*150+origin+shift),height=F((double)(i.P(15)!=0?i.P(4):i.P(4)%100)+i.P(11)+12),y=page.Vertical(height);
   float h=F((double)size*cue*1.2000000476837158*(i.P(15)==0?1:i.P(15)));var body=Item.Make(16,i.P(2),i.P(3),i.P(4),i.P(14)==0?1.0499999523162842f:1,h,0,font,0,0,0,1,0);
   string value=((int)i.P(8)).ToString(System.Globalization.CultureInfo.InvariantCulture);
   context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};context.Prepared.X=x;context.Prepared.Y=y;
   text.Set(body,value,page.Scale);return body;
  }
  public static List<PrimitiveTextPoint> Fermata(Item i,NativeNotationContext page,NativeDatGlyphs dat,DrawingLibrary library,ScoreText text){
   if((i.P(5)<-4||i.P(5)>10||i.P(5)!=(int)i.P(5))||Single.IsNaN(i.P(6))||i.P(6)<0||i.P(6)>32767||i.P(10)!=0)throw new NotSupportedException("Rest fermata body controls outside the measured request.");
   for(int n=13;n<=20;n++)if(i.P(n)!=0&&n!=14&&n!=15&&n!=16)throw new NotSupportedException("Rest fermata auxiliary controls outside the measured request.");
   var body=i.Clone();body.Set(8,0);body.Set(11,0);body.Set(12,0);body.Set(14,0);body.Set(16,0);var bodyPoints=NativeScanMusicVectors.Prepare(body,page,dat,library,text);
   float size=i.P(12)==0?1:i.P(12);if(Single.IsNaN(size)||Single.IsInfinity(size)||Single.IsNaN(i.P(11))||Single.IsInfinity(i.P(11)))throw new NotSupportedException("Rest fermata size/height outside the measured finite domain.");
   float cue=i.P(15)==0&&Math.Abs((int)i.P(4))>=100?.699999988079071f:1,scale=EffectiveScale(i,page),factor=F((double)scale*6);
   float offset=i.P(5)<0?1.4199999570846558f:i.P(5)==0?.949999988079071f:i.P(5)==1?1.350000023841858f:1.149999976158142f;
   float shift=F((double)factor*offset),width=F((double)size*factor*1.690000057220459),origin=i.P(9)>0?F((double)i.P(9)*150):page.Prepared.X;
   float x=F((double)i.P(16)*150+origin+shift-width),pitch=Pitch(i),height=F((i.P(8)==-2?1.0:13.0)+(pitch>0?pitch:0)+i.P(11));float y=page.Vertical(height);
   float glyphSize=F((double)size*cue*(i.P(15)==0?1:i.P(15)));var points=dat.Expand(31,glyphSize,i.P(8)==-2?-glyphSize:glyphSize,page.Scale,x,y);
   // Original rest dispatch 0F73:0C0F..0CA8 appends the guide before
   // preparing the fermata; the ordinary body follows the attachment.
   if(i.P(6)>=5&&i.P(5)!=-4){points.InsertRange(0,bodyPoints.GetRange(0,2));bodyPoints.RemoveRange(0,2);}
   points.AddRange(bodyPoints);return points;
  }
 }
}
