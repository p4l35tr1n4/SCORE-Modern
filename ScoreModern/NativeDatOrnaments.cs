using System;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatOrnaments {
  // Code7 relative13E8:0003..0BEE. Requests were measured at the original
  // primitive, DRW/text and line delegates, then checked as complete buffers.
  static float F(double x){return (float)x;}
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext p,NativeDatGlyphs dat,DrawingLibrary library,ScoreText text){return Prepare(i,p,dat,library,text,false);}
  internal static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext p,NativeDatGlyphs dat,DrawingLibrary library,ScoreText text,bool printedHarp){
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n)))throw new NotSupportedException("DAT ornament nonfinite controls need fault verification.");
   short type=unchecked((short)(int)i.P(7));float size=i.P(5)==0?1:i.P(5);var points=new List<PrimitiveTextPoint>();
   if(type==0){
    foreach(int n in new[]{10,14,16,17,18,19,20})if(i.P(n)!=0)throw new NotSupportedException("DAT trill auxiliary controls need verification.");
    float x=F((double)p.Prepared.X+i.P(15)*150);
    var q=NativeTrillGlyph.Prepare(i.P(5),x,p.Prepared.Y,p.Scale);points.AddRange(dat.Expand(q.Glyph,q.Horizontal,q.Vertical,p.Scale,q.X,q.Y));int accidental=(short)(int)i.P(8);
    if(accidental>=1&&accidental<=3){float h=F((double)size*.6499999761581421),dy=F((double)size*p.Scale*23),dx=F((double)size*(p.Scale*7)*(accidental==1?3:3.5999999046325684));points.AddRange(dat.Expand(accidental+5,h,h,p.Scale,F((double)x+dy),F((double)q.Y+dx)));}
    if((short)(int)i.P(6)!=0){var context=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=p.Prepared,Projection=p.Projection};float wiggle=i.P(9)==0?1:i.P(9),height=F((double)i.P(11)-wiggle+i.P(4)+2.200000047683716f);height=F((double)height+.4f);context.Prepared.X=F((double)x+size*p.Scale*22.850000381469727);context.Prepared.Y=p.Vertical(height);var wave=Item.Make(4,i.P(2),i.P(3),i.P(4),height,i.P(6),-1,F((double)wiggle*size),0,0,0,i.P(12),i.P(13));points.AddRange(NativeDatLines.Waves(wave,context));}
    return points;
   }
   if(type==8||type==-8||type==15||type==88){
    var labelPage=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=p.Prepared,Projection=p.Projection};
    labelPage.Prepared.X=F((double)p.Prepared.X+i.P(15)*150);
    foreach(int n in new[]{14,16,17,18,19,20})if(i.P(n)!=0)throw new NotSupportedException("DAT ottava auxiliary controls need verification.");
    if(type==88){var number=Item.Make(16,i.P(2),i.P(3),i.P(4),0,F(F((double)size*1.100000023841858)*.75),0,1002,0,0,0,1,0);text.Set(number,"8",p.Scale);points.AddRange(text.ScanStrokeText(number,"8",labelPage,0));}
    else{var body=Item.Make(9,i.P(2),i.P(3),i.P(4),type==8?45:type==-8?46:47,F(F((double)size*.75)*1.100000023841858));var aliasPage=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=labelPage.Prepared,Projection=p.Projection};aliasPage.Prepared.X=F((double)aliasPage.Prepared.X+i.P(11)*150);points.AddRange(NativeLibrarySymbols.ScanTextAlias(body,aliasPage,text));}
    if((short)(int)i.P(6)!=0){float height=F((double)i.P(4)+(type==88?F((double)F((double)size*1.100000023841858)*1.5+.4000000059604645):.4000000059604645)),dash=i.P(8),gap=i.P(9);if(dash==0){dash=F((double)size*p.Scale/25);gap=F((double)dash*1.5);}var line=Item.Make(4,i.P(2),i.P(3),height,height,i.P(6),1,dash,gap,i.P(10),i.P(11),i.P(12),i.P(13));var context=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=p.Prepared,Projection=p.Projection};context.Prepared.X=F((double)labelPage.Prepared.X+size*p.Scale*(type==15?62:type==88?18:43));context.Prepared.Y=p.Vertical(height);points.AddRange(NativeDatLines.Dashes(line,context));}
    return points;
   }
   if(type==20||type==-88){
    foreach(int n in new[]{14,16,17,18,19,20})if(i.P(n)!=0)throw new NotSupportedException("DAT piano-pedal auxiliary controls need verification.");
    float factor=i.P(9)==0?.4000000059604645f:F((double)i.P(9)*.4000000059604645),step=F((double)factor*p.Scale*53);short pedals=(short)(int)i.P(5);float origin=F((double)p.Prepared.X+i.P(15)*150);var context=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=p.Prepared,Projection=p.Projection};
    for(int n=0;n<3;n++){int digit=n==0?pedals/100:n==1?pedals%100/10:pedals%10;if(digit==0)continue;context.Prepared.X=F((double)origin+n*step);points.AddRange(NativeLibrarySymbols.ScanGlyph(Item.Make(9,i.P(2),i.P(3),i.P(4),49,factor,factor,0,0,i.P(10)),context,library));}
    context.Prepared.X=origin;points.AddRange(NativeLibrarySymbols.ScanGlyph(Item.Make(9,i.P(2),i.P(3),i.P(4),48,factor,factor),context,library));
    if((short)(int)i.P(6)!=0){context.Prepared.X=i.P(8)!=0?F((double)i.P(8)*150):origin;var line=Item.Make(4,i.P(2),i.P(8)!=0?i.P(8):i.P(3),i.P(4),i.P(4),i.P(6),0,0,0,i.P(10),i.P(11),i.P(12),i.P(13));points.AddRange(NativeDatLines.Straight(line,context));}return points;
   }
   if(type==1){var harp=Harp(i,p,printedHarp,library!=null&&NativeLibraryLocation.IsScor4(library.Root));foreach(var part in harp)points.AddRange(part);return points;}
   if(type==21||type==22){
    foreach(int n in new[]{10,13,14,15,16,17,18,19,20})if(i.P(n)!=0)throw new NotSupportedException("DAT pedal-line auxiliary controls need verification.");
    float lift=i.P(9)==0?2:i.P(9),x=p.Prepared.X,y=p.Prepared.Y,step=F((double)p.Scale*6);Action<float,float,short> add=(xx,yy,op)=>points.Add(new PrimitiveTextPoint{X=xx,Y=yy,Operation=op});short start=1;if(i.P(11)!=0){NativeDatLines.Hook(points,x,y,i.P(11),F((double)p.Scale*7),ref start);if(i.P(11)>=100)start=1;}add(x,y,start);
    if(i.P(8)!=0){x=F((double)i.P(8)*150);add(x,y,0);}x=F((double)x+step);add(x,p.Vertical(F((double)i.P(4)+lift)),0);x=F((double)x+step);
    if(type==21){add(x,p.Vertical(F((double)i.P(4)-lift/2.0)),0);x=F((double)x+step);}add(x,y,0);x=F((double)i.P(6)*150);add(x,y,0);if(i.P(12)!=0){if(i.P(12)>=100){short finish=0;NativeDatLines.Hook(points,x,y,i.P(12),F((double)p.Scale*7),ref finish);}else add(x,p.Vertical(F((double)i.P(4)+i.P(12))),0);}return points;
   }
   throw new NotSupportedException("DAT ornament subtype needs verification: "+type);
  }
  internal static List<PrimitiveTextPoint>[] Harp(Item i,NativeNotationContext p,bool printed,bool scor4=false){
   if(scor4){float offset=ScoreVersionControls.HarpOffset(true,i.P(15));i=i.Clone();i.Set(3,F((double)i.P(3)+i.P(15)));i.Set(15,0);p=new NativeNotationContext{SourceScore=p.SourceScore,Staff=p.Staff,RawBase=p.RawBase,Prepared=p.Prepared,Projection=p.Projection};p.Prepared.X=F((double)p.Prepared.X+offset);}
   for(int n=3;n<=20;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n)))throw new NotSupportedException("Nonfinite harp controls require original fault verification.");
   foreach(int n in new[]{6,13,14,16,17,18,19,20})if(i.P(n)!=0)throw new NotSupportedException("DAT harp auxiliary controls need verification.");
   float size=i.P(5)==0?1:i.P(5),ratio=F((double)size*p.Scale/25),pitch=i.P(4),low=F((double)pitch-size*4),high=F((double)pitch+size*4);
   var context=new NativeNotationContext{Staff=p.Staff,RawBase=p.RawBase,StaffBases=p.StaffBases,Prepared=p.Prepared,Projection=p.Projection};
   context.Prepared.X=F((double)p.Prepared.X+i.P(15)*150);context.Prepared.Y=p.Vertical(low);
   var line=Item.Make(4,i.P(2),i.P(3),low,high,i.P(3),0,0,0,printed?0:1,i.P(11),i.P(12));
   var first=NativeDatLines.Straight(line,context);
   short carried=1;if(Math.Abs((double)F(i.P(3)*150)-context.Prepared.X)>=Math.Abs((double)p.Vertical(high)-context.Prepared.Y))carried=100;
   float left=F((double)i.P(3)-ratio*9.5),right=F((double)i.P(3)+ratio*12);context.Prepared.X=F((double)left*150);context.Prepared.Y=p.Vertical(pitch);
   line=Item.Make(4,i.P(2),left,pitch,pitch,right,0,0,0,printed?0:carried,i.P(11),i.P(12));var second=NativeDatLines.Straight(line,context);
   float origin=F(((double)i.P(3)-ratio*7.5)*150),step=F((double)ratio*375),dx=F(F((double)p.Scale*3.5)*(double)size),dy=F(F((double)p.Scale*3.5)*(double)F((double)size*3)),unit=F((double)p.Vertical(F((double)size*2.5+pitch))-p.Prepared.Y),baseline=F((double)p.Prepared.Y-unit*2),shear=F((double)p.Scale*i.P(10)*7);
   int[] digits={(short)(int)i.P(8)/100,((short)(int)i.P(8)%100)/10,(short)(int)i.P(8)%10,(short)(int)i.P(9)/1000,((short)(int)i.P(9)%1000)/100,((short)(int)i.P(9)%100)/10,(short)(int)i.P(9)%10};var boxes=new List<PrimitiveTextPoint>();
   for(int n=0;n<digits.Length;n++){if(n==3)origin=F(((double)i.P(3)+ratio*2.5)*150);else if(n>0)origin=F((double)origin+step);float y=F((double)baseline+digits[n]*unit);var box=NativeDatSpecialItems.Box(F((double)origin-dx),F((double)y-dy),F((double)origin+dx),F((double)y+dy),true);for(int k=2;k<4;k++){var q=box[k];q.X=F((double)q.X+shear);box[k]=q;}boxes.AddRange(box);}
   return new[]{first,second,boxes};
  }
 }
}
