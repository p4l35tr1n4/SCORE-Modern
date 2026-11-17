using System;using System.Collections.Generic;using System.Drawing;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 // Original 1BA1:0007..05BA; N131 entry/branch and text-converter snapshots.
 public static class NativeTablatureDrawing {
  static float F(double x){return (float)x;}
  public static bool IsTab(Item i){return i.Code==1&&i.P(6)<=-4;}
  static Item Prepare(Item i,NativeNotationContext page,bool dat,out NativeNotationContext position,out string value){
   position=null;value=null;if(!IsTab(i)||page==null)return null;
   for(int k=3;k<=19;k++)if(Single.IsNaN(i.P(k))||Single.IsInfinity(i.P(k))||Math.Abs(i.P(k))>32767)return null;
   int head=-(int)i.P(6),strings=head%10,variant=head/10,requested=(int)i.P(8),fret=(int)i.P(9),fontWord=(int)i.P(12);if(strings<4||strings>8||requested>strings||fontWord<0||fontWord%100>89)return null;
   float pitch;
   if(requested<0){if(!dat){value="";return Item.Make(16,i.P(2),i.P(3),i.P(4));}value="(Tie)";pitch=.2f;fret=99;fontWord=100;}
   else {
    if(variant==8||variant==9||variant==15||variant<4){
     // Automatic pitch selection uses the separately recovered original tables.
     // N133 verifies packed retuning/capo and the literal negative-P14 path.
     LayoutTablaturePitchPlan plan;try{plan=LayoutTablaturePitch.PrepareDrawing(head,(int)(i.P(4)%100),(int)i.P(5)%10,new short[]{(short)(int)i.P(16),(short)(int)i.P(17),(short)(int)i.P(18),i.P(19)>=100?(short)(int)i.P(18):(short)0},i.P(19)<100?(short)(int)i.P(19):(short)0);}catch(NotSupportedException){return null;}
     requested=plan.SelectedString(requested);if(fret>=0)fret=unchecked((short)(plan.Absolute-plan.Tuning[requested]));
    }else if(requested==0)return null;
    pitch=F(F((strings-requested)*2.0+i.P(14))+2.200000047683716);
    if(fret>=1001){int letter=fret%1000+16;if(fret>=2000)letter+=32;value=((char)(letter+48)).ToString();pitch=F((double)pitch+.20000000298023224);fret=0;}
    else if(fret==-1)value="X";
    else if(fret>=0&&fret<=99)value=(fret/10==0?"":((char)(fret/10+48)).ToString())+((char)(fret%10+48)).ToString();
    else return null;
    // The original scan converter receives the extra .2 staff step for
    // numeric/X text. Printed numeric baselines remove that same offset.
    if(dat&&(fret!=0||value=="0"))pitch=F((double)pitch+.20000000298023224);
    if(fontWord>=100)value=fret==99?"(Tie)":"("+value+")";
   }
   float size=F(.550000011920929*(i.P(15)==0?1:i.P(15)));if(Math.Abs((int)i.P(4))>=100)size=F((double)size*.8180000185966492);
   int font=fontWord%100==0?1005:1000+fontWord%100;
   float shift=0;if(fret>=10)shift=F((double)shift-4);if(fontWord>=100)shift=F((double)shift-4);
   position=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Projection=page.Projection,Prepared=page.Prepared};position.Prepared.Scale=F((double)page.Scale*(i.P(15)==0?1:i.P(15)));position.Prepared.X=F((double)page.Prepared.X+F((double)shift*position.Scale));position.Prepared.Y=page.Vertical(pitch);
   return Item.Make(16,i.P(2),i.P(3),pitch,.949999988079071f,size,0,font,i.P(13),0,0,1,0);
  }
  public static List<PrimitiveTextPoint> Scan(Item i,NativeNotationContext page,ScoreText text,short previousFont,out short finalFont){NativeNotationContext p;string value;var body=Prepare(i,page,true,out p,out value);if(body==null)throw new NotSupportedException("Tablature controls outside the runtime-verified text route.");text.Set(body,value,p.Scale);return text.ScanStrokeText(body,value,p,previousFont,out finalFont);}
  public static bool Draw(Graphics g,Item i,NativeNotationContext page,ScoreText text){NativeNotationContext p;string value;var body=Prepare(i,page,false,out p,out value);if(body==null)return false;if(value.Length==0)return true;text.Set(body,value,p.Scale);text.Draw(g,body,value,0,0,p.Scale/5,p.Scale,p);return true;}
 }
}
