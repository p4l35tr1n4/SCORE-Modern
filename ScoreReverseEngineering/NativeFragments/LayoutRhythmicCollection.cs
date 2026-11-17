using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class LayoutRhythmicCollectionResult {
  public short Count,NextItem;public float Total;public bool InvalidatedSort;
 }
 // Original 2CB5:000B..0543, with its 0544 meter filter and 05E2
 // simultaneous-duration reducer. Arrays and scratch remain caller-owned.
 public static class LayoutRhythmicCollection {
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){if(Single.IsNaN(f)||Single.IsInfinity(f)||f< -32768||f>=32768)throw new NotSupportedException("Rhythmic collection leaves the verified finite word domain.");return (short)Math.Truncate(f);}
  static bool Excluded(float[] words,int at,int count){return count>=14&&Bits(words[at+14])!=0||count>=15&&Bits(words[at+15])!=0;}
  public static LayoutRhythmicCollectionResult Collect(float[] words,int[] starts,float staff,short inputMode,short cursor,float[] rhythms,float[] positions,float[] scratch,Action<int> acknowledgeNegativePosition=null,bool standaloneJust=false) {
   if(words==null||starts==null||rhythms==null||positions==null||scratch==null||cursor<1)throw new ArgumentException("Invalid rhythmic collector buffers.");
   int target=Word(staff),first=-1,last=-1,next=cursor,count=0,group=0;float total=0,lastX=-990f;bool invalidated=false;
   Func<int,int> extent=delegate(int i){if(i<0||i>=starts.Length)throw new NotSupportedException("Rhythmic record index leaves its bank.");int at=starts[i]-1;if(at<0||at+2>=words.Length)throw new NotSupportedException("Rhythmic record leaves its bank.");int n=Word(words[at]);if(n<2||at+n>=words.Length)throw new NotSupportedException("Rhythmic record extent is invalid.");if(n<3)throw new NotSupportedException("Original short rhythmic record path is not yet verified.");return n;};
   Func<int,int,bool> meterExcluded=delegate(int at,int n){if(n<4)return false;float size=1;if(n>=7&&Bits(words[at+7])!=0)size=words[at+7];return Math.Abs((double)words[at+4]*size)>2;};
   for(int i=cursor-1;i<starts.Length;i++) {
    int n=extent(i),at=starts[i]-1,code=Word(words[at+1]),itemStaff=Word(words[at+2]);
    if(first<0){
     if(itemStaff<target)continue;
     bool ignored=code>3&&code<17&&code!=14;
     if(standaloneJust&&ignored)continue;
     float x=words[at+3];if(Math.Abs((double)x)<=0.01f){if(n>2)words[at+3]=0;x=0;}
     if(ignored)continue;
     if(Bits(x)<0){
      if(acknowledgeNegativePosition==null)throw new NotSupportedException("Original rhythmic collector requests input for a negative first position.");
      // PAGE 21A8:00D2–00EE warns, waits, and resumes at the next item.
      acknowledgeNegativePosition(i+1);
      continue;
     }
     if(itemStaff!=target)break;
     if(code==14&&n>4){int subtype=Word(words[at+5]);if(subtype>7&&subtype<11)continue;}
     if(code>16&&(Excluded(words,at,n)||code==18&&meterExcluded(at,n)))continue;
     first=i;
    } else if(itemStaff>target){next=i+1;last=i-1;break;}
    if(i==starts.Length-1){last=i;/* Original leaves the caller cursor unchanged here. */}
   }
   if(first<0)return new LayoutRhythmicCollectionResult{NextItem=(short)next};
   while(last>first){int at=starts[last]-1;int code=Word(words[at+1]);if(code<3||code==14||code>16)break;last--;}
   if(last<first)return new LayoutRhythmicCollectionResult{NextItem=(short)next};
   {int at=starts[last]-1;if(Math.Abs(200.0-words[at+3])<=0.01f)words[at+3]=200;}
   Action flush=delegate {
    if(count<1)throw new NotSupportedException("Original empty rhythmic reduction would address the preceding table cell.");
    int old=group;group=0;float minimum=scratch[0];
    if(old>=2){int minimumIndex=0;for(int j=1;j<old;j++)if(scratch[j]<=minimum){minimum=scratch[j];minimumIndex=j;}
     if(minimumIndex!=0){scratch[minimumIndex]=scratch[0];scratch[0]=minimum;}
     for(int j=1;j<old;j++){double difference=(double)scratch[j]-minimum;if(difference>0.001f){if(group>=scratch.Length)throw new NotSupportedException("Rhythmic scratch capacity exceeded.");scratch[group++]=(float)difference;}}
    }
    rhythms[count-1]=minimum;
   };
   for(int i=first;i<=last;i++) {
    int n=extent(i),at=starts[i]-1,code=Word(words[at+1]);float x=words[at+3],duration=0;bool add=false;
    if(code<3){if(n<7)continue;duration=words[at+7];if(Bits(duration)<=0)continue;
     if(duration>=64){invalidated=true;if(inputMode!=0)continue;duration=-64;}
     if(Math.Abs((double)x-lastX)>0.1f)add=true;
    }else{
     if(!(code==14||code==3||code>16)||x==lastX)continue;
     if(code>16&&Excluded(words,at,n)||code==18&&meterExcluded(at,n))continue;
     if(code==14&&n>11&&Bits(words[at+12])>0)code=140;
     if(code==3&&n>=5&&words[at+5]>=44)continue;
     duration=-code;add=true;
    }
    if(add){if(count>0)flush();if(duration==99&&group!=0)throw new NotSupportedException("Original rhythmic collector requests input for its pending-duration diagnostic.");
     if(count>=rhythms.Length||count>=positions.Length)throw new NotSupportedException("Rhythmic table capacity exceeded.");positions[count++]=x;lastX=x;
    }
    if(group>=scratch.Length)throw new NotSupportedException("Rhythmic scratch capacity exceeded.");scratch[group++]=duration;
   }
   flush();for(int i=0;i<count;i++)if(Bits(rhythms[i])>0)total=(float)((double)total+rhythms[i]);if(total==0)count=0;
   return new LayoutRhythmicCollectionResult{Count=(short)count,NextItem=(short)next,Total=total,InvalidatedSort=invalidated};
  }
 }
}
