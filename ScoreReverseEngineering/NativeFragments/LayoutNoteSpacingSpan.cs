using System;
namespace ScoreModern.RecoveredFragments {
 // Projection of original 2DCE:0352..0E2D, stopping before common table append.
 // The caller supplies explicit workspace values/preferences and raw score
 // words. No captured workspace, resource data or renderer bounds are embedded.
 // Apply returns false for the original early-finalization branch at 13AE.
 public sealed class LayoutNoteSpacingSpan {
  readonly byte[] data;readonly float[] score,positions;readonly short[] references;readonly bool originalQuarterToneWidths;
  readonly float followingBarScale;
  public LayoutNoteSpacingSpan(byte[] workspace,float[] scoreWords,short[] recordReferences,float[] selectedPositions,bool originalQuarterToneWidths=true,float followingBarScale=1){data=workspace;score=scoreWords;references=recordReferences;positions=selectedPositions;this.originalQuarterToneWidths=originalQuarterToneWidths;this.followingBarScale=followingBarScale;if(data==null||data.Length<65536)throw new ArgumentException("A complete explicit workspace is required.");}
  short W(int a){return BitConverter.ToInt16(data,a);}void W(int a,int v){var b=BitConverter.GetBytes(unchecked((short)v));Buffer.BlockCopy(b,0,data,a,2);}
  float F(int a){return BitConverter.ToSingle(data,a);}void F(int a,double v){var b=BitConverter.GetBytes((float)v);Buffer.BlockCopy(b,0,data,a,4);}
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(double f){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes((float)f),0),out n))throw new NotSupportedException("Note-spacing integer conversion is outside the finite converter domain.");return unchecked((short)n);}
  float Raw(int w,int p){int index=w-1+p;if(index<0||index>=score.Length)throw new NotSupportedException("Original note-spacing word reference leaves its buffer.");return score[index];}
  int Ref(int i){if(i<1||i>references.Length)throw new NotSupportedException("Original note-spacing selection reference leaves its list.");return references[i-1];}
  float Pos(int i){if(i<1||i>positions.Length)throw new NotSupportedException("Original note-spacing position leaves its table.");return positions[i-1];}
  void Pos(int i,float x){if(i<1||i>positions.Length)throw new NotSupportedException("Original note-spacing position leaves its table.");positions[i-1]=x;}
  double Mod(double value,double divisor){if(Double.IsNaN(value)||Double.IsInfinity(value)||divisor==0)throw new NotSupportedException("Non-finite original note-spacing remainder is prevented.");return value%divisor;}
  void Load(int w,int countAddress){
   int count=Word(Raw(w,0));if(count<1)throw new NotSupportedException("Original note-spacing record extent is invalid.");W(countAddress,count);W(0xb9c0,Word(Raw(w,2)));Array.Clear(data,0xb9c2,48);
   int last=Math.Min(count,14);W(0xd552,last);W(0xafa2,1);
   for(int p=3;p<=last;p++)F(0xb9c2+(p-3)*4,Raw(w,p));W(0xafa0,last>=3?last+1:3);
   if(F(0xb9ce)<=F(0x77f6)&&Raw(w,1)==F(0x77e0)){F(0xb9ca,0);F(0xb9da,0);}
  }
  public void LoadRecord(int wordIndex,int countAddress){Load(wordIndex,countAddress);}
  public bool Apply(){
   W(0xae9c,Word(F(0xb9ce)));F(0xd4f0,F(0x383e));int style=W(0xae9c);
   if(style<=19&&(style%10==2||style%10==3))F(0xd4f0,F(0x3842));
   W(0xd4ba,W(0xd48a));W(0xae9a,Word(F(0xb9ca)));if(W(0xae9a)>100)F(0xb9ca,Mod(F(0xb9ca),F(0x7766)));
   W(0xd4a0,Word((double)F(0xb9ca)/F(0x776a)));
   if(W(0xd4a0)!=1){
    for(int bound=0;bound<1000;bound++){
     if(W(0xd4a0)!=0||W(0xd4ba)>=W(0xd484))goto Ready;
     W(0xd4ba,W(0xd4ba)+1);W(0xd4a2,Ref(W(0xd4ba)));
     if(Raw(W(0xd4a2),1)!=F(0x774c)||Raw(W(0xd4a2),3)!=F(0xb9c2))goto Ready;
     W(0xd4a0,(Word(Raw(W(0xd4a2),5))%100)/10);if(W(0xd4a0)==1)break;
     if(bound==999)throw new NotSupportedException("Original note-spacing voice search exceeded its list.");
    }
   }
   if(W(0xd4ba)>=W(0xd484))return false;
   W(0xd4a2,Ref(W(0xd4ba)+1));W(0xd4aa,Word(Raw(W(0xd4a2),1)));
   if(W(0xd4aa)==14)goto FollowingBar;
   F(0xd4c8,Raw(W(0xd4a2),3));if(W(0xd4aa)!=1)goto Ready;
   for(int bound=0;bound<1000;bound++){
    W(0xd49e,Word(Raw(W(0xd4a2),5))%100);if(W(0xd49e)>=20)goto FollowingBar;
    if(Bits(Raw(W(0xd4a2),3))!=Bits(F(0xd4c8))||W(0xd49e)>=10||W(0xd4ba)>=W(0xd484))goto Ready;
    W(0xd4ba,W(0xd4ba)+1);W(0xd4a2,Ref(W(0xd4ba)+1));W(0xd4aa,Word(Raw(W(0xd4a2),1)));if(W(0xd4aa)!=1)goto Ready;
    if(bound==999)throw new NotSupportedException("Original note-spacing following-voice search exceeded its list.");
   }
  FollowingBar:
   for(int bound=0;bound<1000;bound++){
    W(0xd4ba,W(0xd4ba)+1);if(W(0xd4ba)>=W(0xd484))goto Extra;
    W(0xd4a2,Ref(W(0xd4ba)+1));W(0xd4aa,Word(Raw(W(0xd4a2),1)));
    if(W(0xd4aa)!=1||Bits(Raw(W(0xd4a2),3))!=Bits(F(0xd4c8)))goto Extra;
    W(0xd49e,Word(Raw(W(0xd4a2),5))%100);if(W(0xd49e)<10)continue;if(W(0xd49e)<20)goto Ready;goto Extra;
   }
   throw new NotSupportedException("Original note-spacing bar search exceeded its list.");
  Extra:
   F(0xd4c8,F(0x776e));if(Bits(F(0xb9da))!=0)F(0xd4c8,F(0x7772));F(0xd4f0,(double)F(0xd4c8)*followingBarScale+F(0xd4f0));
  Ready:
   if(W(0xd4cc)!=W(0xd4dc))W(0xd4dc,0);W(0xd4cc,0);W(0xd4a0,W(0xd48a));F(0xd47e,F(0xb9ca));
   W(0xd49e,W(0xd4a0)+1);
   for(int bound=0;bound<1000;bound++){
    if(W(0xd49e)>W(0xd484)+1)break;W(0xd48a,W(0xd49e));if(W(0xd49e)>W(0xd484)||Raw(Ref(W(0xd49e)),1)!=F(0x774c))break;
    double delta=(double)Pos(W(0xd49e))-Pos(W(0xd49e)-1);F(0xd4c0,delta);if(delta>=F(0x7776))break;Pos(W(0xd49e),Pos(W(0xd49e)-1));W(0xd49e,W(0xd49e)+1);
    if(bound==999)throw new NotSupportedException("Original note-spacing coincidence search exceeded its list.");
   }
   F(0xd4bc,0);W(0xd48a,W(0xd48a)-1);if(Math.Abs((double)F(0xb9c6))>=F(0x7766))F(0xd48e,F(0x777a));
   if(W(0xd4f4)>=15&&Bits(Raw(W(0xd48c),15))!=0)F(0xd48e,(double)Raw(W(0xd48c),15)*F(0xd48e));
   F(0xd4ac,0);F(0xd4c8,0);W(0xd49e,W(0xd4a0));
   for(int bound=0;bound<1000;bound++){
    W(0xd4aa,Ref(W(0xd49e)));
    if(Raw(W(0xd4aa),1)==F(0x774c))ProcessMember();
    W(0xd49e,W(0xd49e)+1);if(W(0xd49e)>W(0xd48a))break;
    if(bound==999)throw new NotSupportedException("Original note-spacing member loop exceeded its list.");
   }
   F(0xd4d4,(double)F(0xd4ac)*F(0xd48e));F(0xd4a6,((double)F(0xd4c8)+F(0xd4f0))*F(0xd48e));
   W(0xd4b4,W(0xd4a0));W(0xd4a2,W(0xd48a));F(0xd4ea,F(0xd4b0));F(0xd49a,F(0xd47e));F(0xd4fa,0);
   if(W(0xd4f4)>=9)F(0xd4fa,Mod(Raw(W(0xd48c),9),F(0x776a)));
   F(0xd492,F(0xb9e6));return true;
  }
  void ProcessMember(){
   W(0xd4ee,0);foreach(int a in new[]{0xd4d0,0xd4c4,0xd496,0xd4bc,0xd486})F(a,0);Load(W(0xd4aa),0xd4ba);
   if(W(0xd4ba)>=9&&Bits(F(0xb9da))!=0){
    F(0xd486,F(0xb9da));W(0xd4e8,Word(F(0xd486)));W(0xd4c0,W(0xd4e8)%10);W(0xd4e8,(W(0xd4e8)%100)/10);
    if(W(0xd4c0)!=0&&Mod(F(0xb9ca),F(0x7766))<F(0x777e))F(0xd4bc,(double)F(0xd48e)*F(0x7782));
    F(0xd486,Mod(F(0xd486),F(0x774c)));if(F(0xd486)>F(0x7786))F(0xd486,(double)F(0x7786)-F(0xd486));
    F(0xd486,(double)W(0xd4e8)*F(0x778a)+(double)F(0xd486)*F(0x776a));if(F(0xd486)>F(0xd4bc))F(0xd4bc,F(0xd486));W(0xd4ee,0);
   }
   F(0xd47a,0);
   if(W(0xd4ba)>=10&&Bits(Raw(W(0xd4aa),10))!=0){
    F(0xd47a,Raw(W(0xd4aa),10));F(0xd4c0,F(0xd47a));
    if(F(0xd4c0)==F(0x776a)||F(0xd4c0)==F(0x777e)){
     F(0xd4c0,F(0x3846));if(F(0xd4f0)==F(0x3842))F(0xd4c0,F(0x383e));
     if(F(0xd47a)>=F(0x777e)){F(0xd4c4,-F(0xd4c4));W(0xd4dc,0);F(0xd4d0,F(0xd4c0));}else F(0xd4c4,F(0xd4c0));
    }else if(Bits(F(0xd4c0))<0)F(0xd4d0,-F(0xd4c0));else{W(0xd4ba,-1);F(0xd4c4,F(0xd4c0));}
   }
   F(0xd4c4,(double)F(0xd4bc)+F(0xd4c4));W(0xae9a,Word(F(0xb9ca)));W(0xd4a4,W(0xae9a)%10);
   if(W(0xd4a4)!=0){
    W(0xd4dc,0);if(W(0xae9a)>100)W(0xd4a4,W(0xd4a4)+9);
    // User-approved default: Tom's Help explicitly gives 106..109 the
    // same symbols as 6..9. Compatibility retains the original adjacent
    // descriptor read in the caller's explicit DOS workspace instead.
    int widthIndex=W(0xd4a4);if(widthIndex>=15&&widthIndex<=18&&!originalQuarterToneWidths)widthIndex-=9;
    if(widthIndex<0||widthIndex>18)throw new NotSupportedException("Original accidental-spacing control leaves the verified table/descriptor domain.");
    F(0xd4c0,F(0x3846+widthIndex*4));
    if(W(0xd4de)==1&&W(0xd4b4)==W(0xd4a2)){
     if(F(0xb9e6)!=F(0xd492)){
      if(F(0xd49a)>=F(0x777e)||F(0xd47e)>=F(0x777e))goto SkipAccidental;
      F(0xd4b6,F(0xb9e6));if(F(0xd4b6)==F(0x774c))F(0xd4b6,-F(0xd4b6));if(F(0xd492)==F(0x774c))F(0xd492,-F(0xd492));
      if(F(0xd4b6)>=F(0xd492))goto SkipAccidental;
     }else if(Math.Abs((double)F(0xd4ea)-F(0xd4b0))>=F(0x778e)){
      bool subtract=F(0xd49a)<F(0x777e)?F(0xd4ea)>F(0xd4b0):(F(0xd4ea)<F(0xd4b0)||Bits(F(0xd4fa))==0);
      if(subtract)F(0xd4c0,(double)F(0xd4c0)-F(0x776e));
     }
    }
    F(0xd4b6,Mod(F(0xb9ca),F(0x774c)));if(F(0xd4b6)>F(0x7786))F(0xd4b6,(double)F(0x7786)-F(0xd4b6));
    F(0xd4d0,(double)F(0xd4b6)*F(0x776a)+F(0xd4c0)+F(0xd4d0));if(W(0xd4ba)<0)F(0xd4d0,(double)F(0xd4d0)-F(0xd47a));
   }
  SkipAccidental:
   F(0xb9c6,Mod(F(0xb9c6),F(0x7766)));W(0xd4cc,0);if(F(0xb9c6)>F(0x7792))W(0xd4cc,1);if(F(0xb9c6)<F(0x7750))W(0xd4cc,-1);
   if(W(0xd4cc)==0||W(0xd4dc)!=W(0xd4cc))W(0xd4dc,W(0xd4cc));else{if(F(0xd4d0)<F(0x7796))F(0xd4d0,F(0x7796));if(W(0xd4ee)!=0)W(0xd4dc,0);}
   if(W(0xae9c)>=10&&W(0xae9c)<=19){F(0xd4d0,(double)F(0x778a)+F(0xd4d0));F(0xd4c4,(double)F(0x779a)+F(0xd4c4));}
   if(F(0xd4d0)>F(0xd4ac))F(0xd4ac,F(0xd4d0));if(F(0xd4c4)>F(0xd4c8))F(0xd4c8,F(0xd4c4));
  }
 }
}
