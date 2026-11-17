using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutTablaturePitchPlan {
  public int Absolute,Strings;public int[] Tuning;
  public int SelectedString(int requested){if(requested<0||requested>Strings)throw new NotSupportedException("TAB requested string outside the recovered table.");int preferred=Strings;for(int n=1;n<=Strings;n++)if(Tuning[n]<=Absolute){preferred=n;break;}return Math.Max(preferred,requested);}
 }
 public static class LayoutTablaturePitch {
  //1BA1:0007..0318: original pitch, clef, accidental and tuning word tables.
  //Main TAB supplies the head selected at2C0C:0539..05C8. TP supplies its
  //edited head; its measured caller supplies zero tuning/capo arguments.
  // Drawing caller 1BA1:019B..02A9 has tuning replacement and capo rules
  // distinct from the previously measured TAB command input normalization.
  public static LayoutTablaturePitchPlan PrepareDrawing(int head,int pitch,int accidental,short[] packed,short capo){
   var plan=Prepare(head,pitch,accidental,null);bool bass=head/10==1;
   if(bass)for(int n=1;n<=plan.Strings;n++)plan.Tuning[n]=unchecked((short)(plan.Tuning[n]+12));
   for(int pair=0;packed!=null&&pair<packed.Length;pair++){
    int[] values={packed[pair]/100,packed[pair]%100};
    for(int part=0;part<2;part++){int index=pair*2+part+1,value=values[part];if(index>plan.Strings||value==0)continue;
     if(value>32)plan.Tuning[index]=value;else{if(value>=21)value=20-value;plan.Tuning[index]=unchecked((short)(plan.Tuning[index]+value));}
    }
   }
   for(int n=1;n<=plan.Strings;n++)plan.Tuning[n]=unchecked((short)(plan.Tuning[n]+capo-(bass?12:0)));
   return plan;
  }
  public static LayoutTablaturePitchPlan Prepare(int head,int pitch,int accidental,short[] packed){
   int variant=head/10,strings=head%10,octave=0;
   if(variant==8||variant==15||variant==9){octave=variant==8?7:variant==15?14:-7;variant=0;}
   if(variant<0||variant>=4||strings<1||strings>8||accidental<0||accidental>9)throw new NotSupportedException("TAB pitch plan outside the recovered automatic tables.");
   int[] clefs={0,-12,-6,-8},accidentals={0,-1,1,0,-2,2,0,2,4,5},degrees={2,0,2,4,5,7,9,11},table={40,76,71,67,62,57,52,47,47,0};
   int diatonic=pitch+octave+clefs[variant],degree=(diatonic+6)%7+1;if(degree<=0)degree+=7;
   int absolute=(diatonic+34)/7*12+degrees[degree]+accidentals[accidental],shift=6-strings;if(variant==1)shift=(shift+1)/2+1;shift=Math.Max(0,shift);var tuning=new int[9];
   for(int n=1;n<=strings;n++){if(n+shift>=table.Length)throw new NotSupportedException("TAB tuning index outside the recovered table.");tuning[n]=table[n+shift];}
   if(variant==1&&strings==6)tuning[1]=72;
   if(variant==0&&strings==4){tuning[1]=76;tuning[2]=69;tuning[3]=62;tuning[4]=55;}
   int capo=variant==1?-12:0;for(int n=1;n<=strings;n++)tuning[n]=unchecked((short)(tuning[n]+capo));
   if(packed!=null)for(int pair=0;pair<packed.Length;pair++){int left=packed[pair]/100,right=packed[pair]%100;if(left>=10)left=20-left;if(right>=10)right=20-right;int first=pair*2+1;if(first<=strings)tuning[first]=unchecked((short)(tuning[first]+left));if(first+1<=strings)tuning[first+1]=unchecked((short)(tuning[first+1]+right));}
   return new LayoutTablaturePitchPlan{Absolute=absolute,Strings=strings,Tuning=tuning};
  }
 }
}
