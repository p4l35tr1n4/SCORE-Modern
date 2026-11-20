using System;
namespace ScoreModern.RecoveredFragments {
 // Original 2DCE:000A..1441. Explicit caller-owned buffers retain the original
 // history. Main command selection/undo and preference loading are not done here.
 public sealed class LayoutItemSpacing {
  readonly byte[] data;readonly float[] words,selected,aux,gaps,points;readonly short[] refs;readonly int[] starts;
  public LayoutItemSpacing(byte[] workspace,float[] scoreWords,int[] itemStarts,float[] selectedPositions,short[] selectedReferences,float[] auxiliary,float[] minimumGaps,float[] outputPositions){data=workspace;words=scoreWords;starts=itemStarts;selected=selectedPositions;refs=selectedReferences;aux=auxiliary;gaps=minimumGaps;points=outputPositions;}
  float F(int a){return BitConverter.ToSingle(data,a);}void F(int a,double f){Buffer.BlockCopy(BitConverter.GetBytes((float)f),0,data,a,4);}
  short W(int a){return BitConverter.ToInt16(data,a);}void W(int a,int v){Buffer.BlockCopy(BitConverter.GetBytes(unchecked((short)v)),0,data,a,2);}
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(f),0),out n))throw new NotSupportedException("Spacing conversion leaves its finite integer domain.");return unchecked((short)n);}
  float Raw(int word,int p){int i=word-1+p;if(i<0||i>=words.Length)throw new NotSupportedException("Spacing record word leaves its buffer.");return words[i];}
  static void Bound(int i,float[] a){if(i<0||i>=a.Length)throw new NotSupportedException("Original spacing table leaves its buffer.");}
  void Symbol(LayoutSymbolSpacingResult r){F(0xd4d4,r.Left);F(0xd4a6,r.Right);F(0xafac,r.Scratch);if(r.ResetParameter10Scratch)F(0xb9de,0);}
  public short Apply(short staff,float staffScale,float firstStaff,ref short cursor,float left,float right,float[] afterSymbol,bool originalQuarterToneWidths=true,float? positionJoinTolerance=null,bool standaloneJust=false) {return Apply(staff,staffScale,firstStaff,ref cursor,left,right,afterSymbol,originalQuarterToneWidths,positionJoinTolerance,standaloneJust,false);}
  public short Apply(short staff,float staffScale,float firstStaff,ref short cursor,float left,float right,float[] afterSymbol,bool originalQuarterToneWidths,float? positionJoinTolerance,bool standaloneJust,bool scor4Numbers) {
   if(afterSymbol==null||afterSymbol.Length!=4)throw new ArgumentException("Explicit AFTC/AFTB/AFTK/AFTM values are required.");
   if(standaloneJust&&originalQuarterToneWidths){
    // The old JUST.EXE adjacent descriptors differ from SCORE.EXE's.
    Buffer.BlockCopy(BitConverter.GetBytes(0x00001a94u),0,data,0x3886,4);
    Buffer.BlockCopy(BitConverter.GetBytes(0x4c2d1a94u),0,data,0x388e,4);
   }
   W(0xd4de,0);W(0xd484,0);F(0xd4e4,(double)left-F(0x7748));F(0xd4e0,(double)right+F(0x7748));W(0xd4f6,Word(firstStaff));W(0xd4f8,staff);
   var choice=LayoutSpacingSelection.Collect(words,starts,staff,W(0xd4f6),cursor,left,right,F(0x7748),F(0x774c),F(0x7754),F(0x7758),F(0x775e),F(0x7750),selected.Length,scor4Numbers);
   W(0xd484,choice.Positions.Length);Array.Copy(choice.Positions,selected,choice.Positions.Length);Array.Copy(choice.RecordWords,refs,choice.RecordWords.Length);
   if(choice.Positions.Length<2)return 0;
   LayoutSpacingSelection.Sort(selected,refs,W(0xd484));F(0xd4d8,(double)staffScale*F(0x7762));points[0]=F(0xd4e4);gaps[0]=0;short count=1;F(0xd492,0);W(0xd4dc,0);W(0xd48a,0);
   var note=new LayoutNoteSpacingSpan(data,words,refs,selected,originalQuarterToneWidths,standaloneJust?F(0xd4d8):1);
   for(int iteration=0;iteration<1000;iteration++) {
    W(0xd48a,W(0xd48a)+1);int i=W(0xd48a);if(i<1||i>W(0xd484))throw new NotSupportedException("Original spacing traversal leaves its selection.");
    int w=refs[i-1];W(0xd48c,w);W(0xd4ce,Word(Raw(w,1)));note.LoadRecord(w,0xd4f4);F(0xd4a6,0);F(0xd4d4,0);F(0xd48e,F(0x774c));F(0xd4b0,(double)F(0xb9c6)%F(0x7766));
    int kind=W(0xd4ce),previous=W(0xd4de);bool append=true;
    if(kind<2)append=note.Apply();
    else {
     W(0xd4dc,0);
     if(kind==14){F(0xb9d6,F(0x774c));if(W(0xd4f4)>=17)F(0xb9d6,(double)Raw(w,17)/F(0xd4d8));Symbol(standaloneJust?LayoutSymbolSpacing.JustBar((short)previous,W(0xd4f4),F(0xb9ca)):LayoutSymbolSpacing.Bar((short)previous,W(0xd4f4),F(0xb9ca),F(0xb9d2),F(0xb9d6),F(0xb9ea),F(0xb9ee)));}
     else if(kind==2){int n=W(0xd4f4);var r=LayoutRestSpacing.Apply(F(0xd48e),F(0xb9ce),F(0xb9ca),F(0xb9c6),F(0xd4d4),(short)n,n>=9?Raw(w,9):0,n>=10?Raw(w,10):0,n>=15?Raw(w,15):0);F(0xd48e,r.Multiplier);F(0xd4a6,r.Right);F(0xd4d4,r.Left);if(r.Parameter10Scratch.HasValue)F(0xafac,r.Parameter10Scratch.Value);}
     else if(kind==3)Clef(i,w,previous,count);
     else if(kind==4){F(0xd4d4,F(0x7796));if(previous==14)F(0xd4d4,F(0x77aa));if(previous==3||previous>16)F(0xd4d4,0);F(0xd4a6,F(0x779a));if(Raw(w,0)>=F(0x77ae)){F(0xd4c0,(double)Raw(w,15)/F(0xd4d8));F(0xd4d4,(double)F(0xd4d4)-F(0xd4c0));F(0xd4a6,(double)F(0xd4c0)+F(0x779a));}}
     else if(kind==10&&scor4Numbers){var r=ScoreVersionControls.NumberSpacing(Raw(w,6),Raw(w,15),F(0xd4d8));Symbol(r);}
     else if(kind==17){var r=LayoutSymbolSpacing.Key((short)previous,F(0xb9d2),F(0xb9ca),standaloneJust);Symbol(r);F(0xafb0,r.ScaleScratch);}
     else {int n=W(0xd4f4);var window=new float[words.Length-w];Array.Copy(words,w,window,0,window.Length);var r=LayoutSymbolSpacing.Meter((short)previous,gaps[count-1],window,(short)n,standaloneJust);Symbol(r);gaps[count-1]=r.Minimum;if(n>=8)F(0xafb0,r.ScaleScratch);if(W(0xd48a)==W(0xd484))F(0xd4a6,(double)F(0xd4a6)-F(0x77b2));}
    }
    if(append) {
     if(Math.Abs((double)F(0xb9c2)-points[count-1])>=(positionJoinTolerance??F(0x77b6))){count++;Bound(count-1,points);Bound(count-1,gaps);Bound(count-1,aux);points[count-1]=F(0xb9c2);gaps[count-1]=0;aux[count-1]=0;}
     if(aux[count-1]<F(0xd4d4)||kind==18)aux[count-1]=F(0xd4d4);if(gaps[count-1]<F(0xd4a6))gaps[count-1]=F(0xd4a6);
     if(kind<=2||kind==4) {
      F(0xd4b6,0);
      if(previous==14)F(0xd4b6,(double)afterSymbol[1]/F(0xd4d8)-F(0x77b2));
      if(previous==17)F(0xd4b6,(double)afterSymbol[2]/F(0xd4d8)-F(0x77bc));
      if(previous==18)F(0xd4b6,(double)afterSymbol[3]/F(0xd4d8)-F(0x77c0));
      if(previous==3&&W(0xd482)==0)F(0xd4b6,(double)afterSymbol[0]/F(0xd4d8)-F(0x779a));
      if(Bits(F(0xd4b6))>0)aux[count-1]=(float)((double)F(0xd4b6)+aux[count-1]);
     }
     W(0xd4de,kind);
    }
    if(W(0xd48a)>=W(0xd484))break;
    if(iteration==999)throw new NotSupportedException("Original spacing traversal exceeded its table.");
   }
   cursor=W(0xd48a);
   if(count!=1){Bound(count,aux);aux[count]=0;for(int i=0;i<count;i++)gaps[i]=(float)(((double)gaps[i]+aux[i+1])*F(0xd4d8));W(0xd48a,count+1);}
   return count;
  }
  void Clef(int i,int w,int previous,short count) {
   F(0xd4a6,F(0x779e));W(0xd482,0);if(F(0xb9c6)>=F(0x7766)){F(0xd4a6,F(0x77a2));W(0xd482,1);}
   bool preceding=previous==14;
   if(!preceding){if(previous==1)F(0xd4d4,F(0x774c));if(i==W(0xd484))return;preceding=count==1;}
   if(preceding){F(0xd4c0,gaps[count-1]);if(F(0xd4c0)>=F(0x77a6))F(0xd4c0,(double)F(0xd4c0)-F(0x774c));if(F(0xd4c0)<F(0x77a6))F(0xd4c0,F(0x7750));gaps[count-1]=F(0xd4c0);return;}
   Bound(i,selected);Bound(i-2,selected);double next=selected[i];
   if((double)F(0xd4a6)*F(0xd4d8)>next-F(0xb9c2))return;
   F(0xd4c0,gaps[count-1]);if((double)F(0xb9c2)-selected[i-2]>=F(0xd4c0))return;
   F(0xd4c0,(double)selected[i-2]+F(0xd4c0));if(next-F(0x77aa)<=F(0xd4c0))return;
   F(0xb9c2,F(0xd4c0));words[w+2]=F(0xd4c0);
  }
 }
}
