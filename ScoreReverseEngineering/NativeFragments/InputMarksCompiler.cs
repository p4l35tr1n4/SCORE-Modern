using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputMarksResult {public List<float[]> Records;public int FirstOrnament=-1;}
 public static class InputMarksCompiler {
  static float F(double n){return (float)n;}static int W(float n){if(float.IsNaN(n)||float.IsInfinity(n)||n<int.MinValue||n>int.MaxValue)throw new ArgumentOutOfRangeException("n");return (int)Math.Truncate(n);}
  static float P(float[] p,int n){return p[n-1];}
  static float Offset(int kind,int sign,float[] note){int y=W(note[3])%100;float result=0;if(note[4]%100<20){if(kind<50){if(y<4)return kind==5?0.05000000074505806f:0;result=F(y/10.0-(kind==5?0.3499999940395355:0.4000000059604645));}else if(y<=2&&sign>=0)return y-3;}else{if(kind<50){if(y>10)return kind==5?0.05000000074505806f:0;result=F((kind==5?1.0499999523162842:1)-y/10.0);}else if(y>=12&&sign<0)return y-11;}return result>0.8999999761581421f?0.8999999761581421f:result;}
  static int Mapped(int[] map,int index,int count){index=Math.Max(1,index);if(index>=map.Length||map[index]<1||map[index]>count)throw new ArgumentOutOfRangeException("index","Mark lies outside the input note map");return map[index]-1;}
  static float X(IList<float[]> rows,int[] map,int index,float fraction,float beginning,float end){int at=Mapped(map,index,rows.Count);float x=rows[at][2];if(BitConverter.ToInt32(BitConverter.GetBytes(fraction),0)!=0){float target;if(index==0){target=x;x=beginning;}else target=index<map.Length-1?rows[Mapped(map,index+1,rows.Count)][2]:end;x=F(((double)target-x)*fraction+x);}return x;}
  public static InputMarksResult Apply(IList<float[]> original,int[] map,float[] tokens,int[] opposite,float staff,float beginning,float end,float followingStaffScale,float dynamics){
   return Apply(original,map,tokens,opposite,staff,beginning,end,followingStaffScale,dynamics,false);
  }
  public static InputMarksResult Apply(IList<float[]> original,int[] map,float[] tokens,int[] opposite,float staff,float beginning,float end,float followingStaffScale,float dynamics,bool reproduceRetainedDescriptor){
   var rows=new List<float[]>();foreach(var p in original)rows.Add((float[])p.Clone());var output=new InputMarksResult{Records=rows};int kind=0,sign=0;bool literal=false;
   float[] retained=null;
   for(int cursor=0;cursor<tokens.Length;cursor++){
    float value=tokens[cursor];int integer=W(value),absolute=Math.Abs(integer);if(absolute>9998){kind=absolute/10000;if(kind==0)throw new ArgumentException("Zero mark descriptor");literal=integer<0;sign=opposite!=null&&opposite[cursor]!=0?-kind:kind;retained=null;continue;}
    if(kind==0){
     if(!reproduceRetainedDescriptor)throw new NotSupportedException("A reversed mark motive starts with a target. Enter a mark before it or enable original invalid states.");
     // 3262:0017..0030 leaves the descriptor and record template untouched.
     // In an ordinary single-note phrase these are the final rhythm note.
     if(original.Count!=map.Length||original.Count<2||original[0][0]!=3)
      throw new NotSupportedException("The original retained mark descriptor is unverified for this phrase.");
     for(int i=1;i<original.Count;i++){
      var note=original[i];
      if(note[0]!=1||!(note[2]>original[i-1][2])||!(note[3]>=1&&note[3]<=13)||
         !(note[6]>0&&note[6]<64)||note[11]!=0||note[14]!=0)
       throw new NotSupportedException("The original retained mark descriptor is unverified for this phrase.");
     }
     retained=original[original.Count-1];
     kind=W(retained[3]);literal=true;
    }
    bool range=absolute>=1000;int first=range?absolute-1000:absolute,last=first;if(range&&kind>=95&&kind<=99){while(Math.Abs(W(value))>=1000)value=F((double)value-1000);first=Math.Abs(W(value));range=false;}else if(range){if(++cursor>=tokens.Length)throw new ArgumentException("Incomplete mark range");last=W(tokens[cursor]);}float fraction=range?0:F((double)value-first);
    if(kind>=95&&!literal){if(++cursor>=tokens.Length)throw new ArgumentException("Incomplete span mark");int finish=Math.Abs(W(tokens[cursor]));int beginIndex=Mapped(map,first,rows.Count);float startX=X(rows,map,first,fraction,beginning,end),finishX=X(rows,map,finish,F((double)tokens[cursor]-finish),beginning,end);var p=new float[15];p[1]=staff;p[2]=startX;bool below=sign>0;
     bool ottava=kind<97||kind>119;float y=ottava?(kind==95?-1.5f:12.5f):(below?0:14),extreme=below?99:-99,inset=ottava?1.5f:0;
     if(ottava){if(float.IsNaN(followingStaffScale)||float.IsInfinity(followingStaffScale))throw new NotSupportedException("Following staff scale for this octave span is not verified");below=kind==95;p[0]=7;p[4]=1;p[5]=F((double)finishX+followingStaffScale/6.0);if(kind<100){p[6]=kind==95?-8:8;p[11]=kind==95?1:-1;}else p[7]=kind%10;}else{p[0]=4;p[4]=999;p[5]=finishX;if(kind==98)p[6]=-1;}
     extreme=below?99:-99;for(int n=Math.Max(1,first);n<=finish;n++){var note=rows[Mapped(map,n,rows.Count)];float pitch=F((double)note[3]%100),stem=F((double)note[4]%100);if(below){if(stem>=20)pitch=F((double)pitch-7);if(pitch<extreme)extreme=pitch;}else{if(stem<20)pitch=F((double)pitch+7);if(pitch>extreme)extreme=pitch;}}
     if(below&&extreme<4)y=F((double)F((double)extreme-inset)-3.5);else if(!below&&extreme>10)y=F((double)F((double)extreme-inset)+3.5);p[3]=y;rows.Add(p);continue;
    }
    for(int n=Math.Max(1,first);;n++){
     int at=Mapped(map,n,rows.Count);var note=rows[at];if(kind<=36&&!literal){float correction=sign>=0&&Array.IndexOf(new[]{4,5,8,9,10,19},kind)>=0?Offset(kind,sign,note):0;note[10]=F((double)sign+correction);}
     else if(kind<71||literal){int effective=kind==66||kind==67?-sign:sign;float correction=Offset(kind,effective,note);var p=new float[15];p[0]=9;p[1]=staff;p[2]=X(rows,map,n==Math.Max(1,first)?first:n,n==Math.Max(1,first)?fraction:0,beginning,end);p[3]=F((double)correction+(effective<0?14:-2));p[4]=kind;p[5]=1;if(!literal)p[14]=dynamics;
      if(retained!=null)Array.Copy(retained,6,p,6,3);
      rows.Add(p);}
     else{if(output.FirstOrnament<0)output.FirstOrnament=rows.Count;var p=new float[15];p[0]=6;p[1]=staff;p[2]=X(rows,map,n==Math.Max(1,first)?first:n,n==Math.Max(1,first)?fraction:0,beginning,end);p[6]=F(-((double)note[4]%100));p[9]=kind-80;if(note[5]>=2&&note[5]<=3)p[13]=p[6]>=-10?-1.2300000190734863f:1.600000023841858f;float shift=F((kind>91?1.0:2.0)-note[7]);if(W(note[8])%10!=0){if(p[6]>-20)shift=F(1.0+shift);note[7]=F((double)note[7]+.5);}if(p[6]>-20)shift=-shift;p[3]=F((double)(note[3]%100)+shift);rows.Add(p);}
     if(n>=last)break;
    }
   }
   return output;
  }
 }
}
