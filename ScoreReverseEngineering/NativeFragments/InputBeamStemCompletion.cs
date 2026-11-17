using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputBeamStemCompletion {
  // Overlay2/index17, same-direction beam path, including cue/grace and cross-staff notes. This runs on compact
  // items after input compaction, independently of screen/EPS rendering.
  public static List<float[]> Apply(IList<float[]> items,int firstItem,InputEndpointStaffContext context=null){
   var rows=new List<float[]>();foreach(var p in items)rows.Add((float[])p.Clone());
   int priorStart=0,priorPitch=0;bool hadBeam=false;
   for(int i=firstItem-1;i<rows.Count;i++){
    var beam=rows[i];if(P(beam,1)!=6||beam.Length<7)continue;int descriptor=(short)Math.Truncate(P(beam,7));if(descriptor<0)continue;
    int direction=descriptor/10;if(direction!=1&&direction!=2)throw new NotSupportedException("Beam direction branch");
    if(P(beam,9)!=0||P(beam,11)<0||P(beam,14)<0||P(beam,18)!=0)throw new NotSupportedException("Reversed or interrupted beam branch");
    bool cue=Math.Abs(P(beam,4))>=100;float factor=cue?0.6000000238418579f:1;
    float start=F((double)P(beam,3)-0.009999999776482582),end=F((double)P(beam,6)+0.009999999776482582),pitch=P(beam,4)%100;
    // The 4.01 input saves use the stored endpoints for interpolation. The
    // two-note and asymmetric fixtures distinguish this from the 3.11
    // padded endpoints and binary32 slope/intercept stores.
    bool scor4=context!=null&&context.Scor4;
    double anchor=scor4?P(beam,3):start;
    double span=scor4?(double)P(beam,6)-anchor:F((double)end-start);
    if(span==0)return rows;
    double slope=((double)P(beam,5)-pitch)/span;
    if(!scor4)slope=F(slope);
    double intercept=pitch-anchor*slope;
    if(!scor4)intercept=F(intercept);
    bool repeated=hadBeam&&Bits(start)==priorStart&&Bits(pitch)==priorPitch;
    for(int j=firstItem-1;j<rows.Count;j++){
     var note=rows[j];if(P(note,1)!=1||P(note,2)!=P(beam,2)||note.Length<5||P(note,3)<start||P(note,3)>end)continue;
     int noteDirection=((short)Math.Truncate(P(note,5))%100)/10;if(noteDirection==0)continue;
     if((Math.Abs(P(note,4))>=100)!=cue)continue;
     if(P(note,12)!=0&&context==null||P(note,15)!=0)throw new NotSupportedException("Opposite stem, cross-staff or scaled-note branch");
     bool opposed=noteDirection!=direction;
     float noteX=P(note,3),evaluationX=noteX;
     if(opposed)evaluationX=F((double)noteX+F((double)factor*(direction==1?-2.833329916000366:2.833329916000366)));
     float extension=F((double)evaluationX*slope+intercept-(cue?P(note,4)%100:P(note,4)));
     if(opposed){
      float height=F((double)factor*14);if(direction==1)height=-height;
      extension=F((double)height-extension);
      float depth=F((double)(descriptor%10-1)*1.5);
      depth=F((double)depth+OpposedRangeDepth(beam,11,noteX));
      depth=F((double)depth+OpposedRangeDepth(beam,14,noteX));
      if(direction==2)extension=-extension;
      extension=F((double)F((double)depth*factor)+extension);
     }else if(direction==2)extension=-extension;
     if(context!=null)context.AdjustBeamStem(note,noteDirection,ref extension);
     if(cue&&opposed)extension=F((double)extension+5.599999904632568);
     extension=F((double)extension/factor);
     if(note.Length<8){Array.Resize(ref note,8);rows[j]=note;}
     if(!repeated||P(note,8)<extension)note[7]=extension;
     if(note.Length>8){int flag=(short)Math.Truncate(note[8])%10;if(flag!=0)note[8]=F((double)note[8]-flag);}
    }
    priorStart=Bits(start);priorPitch=Bits(pitch);hadBeam=true;
   }
   return rows;
  }
  // 2EB6:0A16/0B0D: the range's preceding word encodes its depth.
  // A zero endpoint disables it; a negative endpoint uses the beam endpoint.
  static float OpposedRangeDepth(float[] beam,int field,float x){
   if(beam.Length<field+1)return 0;
   float start=P(beam,field),end=P(beam,field+1);
   if(start<0)start=P(beam,3);else if(start==0)return 0;
   if(end<0)end=P(beam,6);else if(end==0)return 0;
   int descriptor=(short)Math.Truncate(P(beam,field-1));
   if(descriptor<0||x<start||x>end)return 0;
   return F((double)(descriptor%10)*1.5);
  }
  static float P(float[] p,int n){return p.Length>=n?p[n-1]:0;}static float F(double x){return (float)x;}static int Bits(float x){return BitConverter.ToInt32(BitConverter.GetBytes(x),0);}
 }
}
