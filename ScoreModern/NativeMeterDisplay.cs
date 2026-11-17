using System;using System.Collections.Generic;using System.Drawing;using System.Globalization;
namespace ScoreModern {
 public struct NativeMeterNumber {public int Number;public float X,Y,Size;}
 public static class NativeMeterDisplay {
  static float F(double value){return (float)value;}
  internal static bool ValidSize(float size){return !Single.IsNaN(size)&&!Single.IsInfinity(size)&&Math.Abs(size)<=32767;}
  public static List<NativeMeterNumber> Prepare(Item item,NativeNotationContext page,bool printed=false){
   if(item.Code!=18||page==null)return null;float numerator=item.P(5),denominator=item.P(6),size=item.P(7);
   if(numerator<0||numerator>=90||denominator<0||denominator>=100)return null;numerator=(short)(int)numerator;denominator=(short)(int)denominator;bool zeroMeter=numerator==0&&denominator==0;if(zeroMeter)denominator=1;
   if(!ValidSize(size))return null;if(BitConverter.ToInt32(BitConverter.GetBytes(size),0)==Int32.MinValue)return null;
   // 0B0D:06BA uses the carried compound-meter origin at DS:C558.
   bool separateDenominator=item.P(9)==-1;
   for(int n=9;n<=12;n++)if(!ValidSize(item.P(n)))return null;
   for(int n=8;n<=20;n++)if(n!=9&&n!=10&&n!=11&&n!=12&&n!=13&&n!=14&&n!=15&&n!=16&&item.P(n)!=0)return null;if(!VerifiedJustificationControl(item.P(14)))return null;
   if(!VerifiedReservedControl(item.P(16)))return null;
   if(!ValidFont(item.P(13))||Single.IsNaN(item.P(15))||Single.IsInfinity(item.P(15)))return null;
   float origin=F((double)page.Prepared.X+item.P(15)*150);
   size=F((size==0?1:(double)size)*1.25);float centered=F((double)page.Scale*size*7.599999904632568+origin),topX=!separateDenominator&&numerator<10&&denominator>=10?centered:origin,bottomX=denominator<10&&numerator>=10?centered:origin;var result=new List<NativeMeterNumber>();
   if(numerator==0){float position=F((double)size*4.079999923706055+item.P(4));result.Add(new NativeMeterNumber{Number=zeroMeter?0:(int)denominator,X=origin,Y=page.Vertical(position),Size=size});}
   else {float top=F((double)size*5.71999979019165+item.P(4));result.Add(new NativeMeterNumber{Number=(int)numerator,X=topX,Y=page.Vertical(top),Size=size});if(denominator>0){
    if(separateDenominator){
     float offset=denominator>=10?2.4000000953674316f:3.799999952316284f;
     if(page.PreviousCompoundMeterNumerator>=10)offset=F((double)offset+3.4000000953674316);
     bottomX=F(((double)offset+item.P(10))*page.Scale*size*4.800000190734863+page.PreviousCompoundMeterX);
    }
    float bottom=F((double)size*2.4800000190734863+item.P(4));result.Add(new NativeMeterNumber{Number=(int)denominator,X=bottomX,Y=page.Vertical(bottom),Size=size});}}
   return result;
  }
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,ScoreText text,DrawingLibrary library){
   if(text==null)return false;
   if(item.P(8)!=0)return DrawCompound(g,item,page,text,library);
   var numbers=Prepare(item,page,!NativeNotationView.UsesDat);if(numbers==null)return false;
   foreach(var number in numbers)DrawNumber(g,item,page,text,number);
   return true;
  }
  // The paired DOS F6 and DAT fixtures preserve P16 with these values
  // without changing either the ordinary or compound numeric drawing.
  internal static bool VerifiedReservedControl(float value){
   return value==0||value==-2||value==2||value==1.25f||value==-1.25f||value==.5f||value==-1;
  }
  internal static bool VerifiedCompoundP17(float value){return value==0||value==.5f||value==2||value==-1||value==-2||value==1.25f||value==-1.25f||value==1||value==-.5f||value==3||value==-3||value==.25f||value==-.25f;}
  // The compound path tests the converted signed word at P12.
  internal static bool SuppressCompoundPlus(float value){return (short)(int)value!=0;}
  // Original spacing selection at 2CB5:075D tests the stored P14 bits.
  // The meter drawing route does not use its numeric magnitude.
  internal static bool VerifiedJustificationControl(float value){
   return !Single.IsNaN(value)&&!Single.IsInfinity(value);
  }
  static bool ValidFont(float font){return font==0||font>=1000&&font<=1089&&font==(int)font;}
  static void DrawNumber(Graphics g,Item item,NativeNotationContext page,ScoreText text,NativeMeterNumber number){
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Projection=page.Projection,Prepared=page.Prepared};context.Prepared.X=number.X;context.Prepared.Y=number.Y;
    float font=item.P(13)==0?94:item.P(13);
    var body=Item.Make(16,item.P(2),item.P(3),item.P(4),1,number.Size,0,font,0,0,0,1,0);text.Set(body,number.Number.ToString(CultureInfo.InvariantCulture),page.Scale);
    var eps=NativeEpsExport.For(g);if(eps!=null)eps.Comment("SCORE meter number "+number.Number+" x "+number.X.ToString("R",CultureInfo.InvariantCulture)+" y "+number.Y.ToString("R",CultureInfo.InvariantCulture)+" size "+number.Size.ToString("R",CultureInfo.InvariantCulture)+" font "+font.ToString("R",CultureInfo.InvariantCulture));
    RenderResourceAudit.Record("meter numeral",number.Number.ToString(CultureInfo.InvariantCulture),font==94?NativeLibraryLocation.FileFor(text.RootDirectory,"MUSI0.DRW"):"FM"+((int)font-1000).ToString("00",CultureInfo.InvariantCulture)+".PSC","recovered numeric meter branch; signed-word digit selection");text.Draw(g,body,body.Text,0,0,page.Scale/5,page.Scale,context);
  }
  static bool DrawCompound(Graphics g,Item i,NativeNotationContext p,ScoreText text,DrawingLibrary library){
   if(i.Code!=18||p==null||library==null)return false;
   for(int field=3;field<=20;field++)if(Single.IsNaN(i.P(field))||Single.IsInfinity(i.P(field))||Math.Abs(i.P(field))>32767)return false;
   if(!VerifiedReservedControl(i.P(16)))return false;
   if(!VerifiedCompoundP17(i.P(17)))return false;
   for(int field=18;field<=20;field++)if(i.P(field)!=0)return false;
   if(!ValidFont(i.P(13))||!VerifiedJustificationControl(i.P(14)))return false;
   int a=(int)i.P(5),b=(int)i.P(6),c=(int)i.P(8),d=(int)i.P(9);
   if(a<0||a>=90||b<0||b>=90||c<1||c>=90||d< -1||d>=90||
    !ValidSize(i.P(7)))return false;
   float rawSize=i.P(7)==0?1:i.P(7),size=F((double)rawSize*1.25),origin=F((double)p.Prepared.X+i.P(15)*150),unit=F((double)p.Scale*rawSize*6);
   float top=p.Vertical(F((double)size*(a==0?4.079999923706055:5.71999979019165)+i.P(4)));
   float bottom=p.Vertical(F((double)size*2.4800000190734863+i.P(4)));
   bool suppress=SuppressCompoundPlus(i.P(12));
   if(!suppress&&!library.Symbols.ContainsKey(38))return false;
   float plus=suppress?origin:F(((a>9||d>0&&b>9?6.599999904632568:3.5)+i.P(10))*unit+origin);
   float extra=F(((double)i.P(11)+4.199999809265137)*unit),second=F((double)plus+extra);
   float centered=F((double)p.Scale*size*7.599999904632568+origin);
   DrawNumber(g,i,p,text,new NativeMeterNumber{Number=a==0?b:a,X=a>0&&d>0&&a<10&&b>=10?centered:origin,Y=top,Size=size});
   float denominatorX=b<10&&a>=10?centered:origin;
   if(d<=0){
    float offset=b>=10?2.4000000953674316f:3.799999952316284f;
    if(a>=10)offset=F((double)offset+3.4000000953674316);
    denominatorX=F(((double)offset+i.P(10))*p.Scale*size*4.800000190734863+origin);
   }
   if(a!=0&&b>0)DrawNumber(g,i,p,text,new NativeMeterNumber{Number=b,X=denominatorX,Y=bottom,Size=size});
   if(!suppress){
    var glyph=new PrimitiveGlyphRequest{Glyph=38,X=plus,Y=p.Vertical(F((d>0?4.71999979019165:6.599999904632568)*size+i.P(4))),Horizontal=size,Vertical=size};
    if(!NativePrimitiveGlyphs.Draw(g,p,library,null,glyph))return false;
   }
   float secondCentered=F((double)p.Scale*size*7.599999904632568+second);
   DrawNumber(g,i,p,text,new NativeMeterNumber{Number=c,X=c<10&&d>=10?secondCentered:second,Y=top,Size=size});
   if(d>0)DrawNumber(g,i,p,text,new NativeMeterNumber{Number=d,X=d<10&&c>=10?secondCentered:second,Y=bottom,Size=size});
   return true;
  }
 }
}
