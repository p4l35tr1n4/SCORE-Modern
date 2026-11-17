using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Parent TR 2F87:0554..0790, before the optional normalization call.
 // Uses overlap bounds and packed/odd-value rules from the original pass.
 public static class InputTranspositionAttachments {
  // Parent DS:8198 retains the collector relationship count across the
  // measured Main engine; bank:30F4 supplies the extended-beam flag.
  public static bool ShouldApply(short relationshipCount,bool extendedBeam){return relationshipCount!=0||extendedBeam;}
  static uint Bits(float f){return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);}
  static short Word(double v){if(Double.IsNaN(v)||Double.IsInfinity(v)||v<=Int64.MinValue||v>=Int64.MaxValue)throw new NotSupportedException("Attachment value outside finite integer conversion.");return unchecked((short)(long)Math.Truncate(v));}
  public static void Apply(IList<float[]> rows,short staff,float left,float right,float diatonic,bool enabled){
   if(!enabled)return;short lower=Word((double)left-4f),upper=Word((double)right+4f);
   foreach(var row in rows){
    if(row.Length<6||Word(row[1])!=staff)continue;short code=Word(row[0]);if(code!=5&&code!=6)continue;
    if(Word(row[2])>upper||Word(row[5])<lower)continue;
    bool slur=code==5;float direction=row.Length>=7&&(Bits(row[6])&0x80000000)!=0?-1f:1f;
    float value=(float)((double)row[4]+diatonic);
    if(slur&&value%2f==1f)value=(float)((double)value+direction);
    row[4]=value;
    value=row[3];
    // 0752..0788 stores remainder+shift at 8190, then tests its
    // high word at 8192. The new remainder chooses the packed sign,
    // including when transposition crosses zero; not the old P4 sign.
    if(Math.Abs(value)>=100f){float remainder=(float)((double)value%100f+diatonic);value=(Bits(remainder)&0x80000000)!=0?-100f:100f;value=(float)((double)value+remainder);}
    else value=(float)((double)value+diatonic);
    if(slur&&value%2f==1f)value=(float)((double)value+direction);
    row[3]=value;
   }
  }
 }
}
