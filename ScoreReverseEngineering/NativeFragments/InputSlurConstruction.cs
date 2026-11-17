using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public static class InputSlurConstruction {
  static float F(double v){return (float)v;}static int W(float v){return unchecked((short)(int)Math.Truncate(v));}static bool BitsEqual(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}
  public static float InitialHeight(float left,float right,float y1,float y2,float offset,float height){if(offset<1){float span=F((double)right-left);if(offset< -1&&offset> -4)span=F((double)offset*2+span);float result=F((double)span/20+Math.Abs((double)y2-y1)*0.10000000149011612+0.8999999761581421);if(BitConverter.ToInt32(BitConverter.GetBytes(height),0)<0)result=-result;return result;}return height;}
  static float AvoidLine(float y,int direction){if(y%2==1){int i=W(y);if(direction==10){if(i<=11)y=F(1.0+y);}else if(i>=3)y=F((double)y-1);}return y;}
  static int Stem(float[] p){int stem=(W(p[4])%100)/10*10;return stem!=0?stem:W(p[3])>6?20:10;}
  static float Attach(float[] p,float pitch,int stem,int direction){float space=F((double)(p[7]%100)+8);if(W(p[4])%100<10)space=2;return stem<20?(direction==20?F((double)pitch-2):F((double)pitch+space)):(direction==20?F((double)pitch-space):F((double)pitch+2));}
  static readonly int[] marks={4,5,6,7,8,9,10,19,30,31,32,33,34,35};
  static float CorrectMark(float[] note,float y,int direction,int stem,int number){float mark=note[10];int word=W(mark);if(word==0||Math.Abs((double)mark-word)>0.10000000149011612||Array.IndexOf(marks,Math.Abs(word))<0)return y;float shift=1.5f;if(word<1){if(stem!=direction)return y;}else{if(stem==direction)return y;if(number==0){if(Math.Abs(word)!=6&&Math.Abs(word)!=7){if(mark%1<=0.20000000298023224)note[10]=F((double)mark+0.18000000715255737);return y;}int pitch=W(note[3]%100);if(pitch%2==1&&((pitch<9&&stem==20)||(pitch>5&&stem==10)))shift=2.5f;}}if(direction==20)shift=-shift;return F((double)shift+y);}
  // Section58 slur/parallel-tie construction. Cross-staff endpoint mutation
  // is a separate routine. Existing input records are cloned.
  public static List<float[]> Build(IList<float[]> original,InputEndpointResult e,float staff,int boundary,short tiesMode,float end,InputEndpointStaffContext context=null){
   var rows=new List<float[]>();foreach(var p in original){if(p.Length!=15)throw new ArgumentException("Fifteen words required.");if(p[0]==1&&p[11]!=0&&context==null)throw new NotSupportedException("Cross-staff slur branch required.");rows.Add((float[])p.Clone());}
   int first=e.StartItem-1,last=e.EndItem-1;if(first<0||last<0||first>=rows.Count||last>=rows.Count)throw new ArgumentOutOfRangeException("e");int ties=tiesMode;if(first+1<rows.Count&&rows[first+1][2]==rows[first][2])ties=0;
   float pitch1=e.StartPitch,pitch2=e.EndPitch;int stem1=Stem(rows[e.StartNote==99?last:first]),stem2=Stem(rows[e.EndNote==99?first:last]);if(e.StartNote==99)pitch1=pitch2;if(e.EndNote==99)pitch2=pitch1;
   int direction=e.Stem;if(direction==0){if(e.StartFlag!=0)direction=30-stem1;else{direction=20;for(int i=first;i<=last;i++)if(rows[i][0]==1){int s=W(rows[i][4])%100;if(s<10&&rows[i][3]%100>6)break;if(s>19){direction=10;break;}}}}
   float x1=e.StartX,x2=e.EndX,y1=0,y2=0,height=0,offset=0;int mode=0,number=e.Number;if(e.EndNote>=boundary)x2=end;
   bool tiePath=e.StartNote==0||(e.StartNote!=99&&e.EndNote!=99&&(e.EndNote>=boundary||(number==0&&BitsEqual(pitch1,pitch2)&&e.EndNote-e.StartNote==1)));
   if(e.StartNote==0||e.StartNote==99)x1=F((double)rows[first][2]-5);
   bool attach=true;
   if(tiePath){mode=1;if(ties>0)offset=0.4000000059604645f;else if(!((double)x2-x1<10&&ties>=0)&&W(rows[last][4])%10==0){float shift=W(pitch1)%2!=0?1.2000000476837158f:.5f;if(direction==20)shift=-shift;y1=y2=F((double)shift+pitch1);offset=rows[first][8]>=10?-3:-2;attach=false;}else offset=-1;}
   else offset=-1;
   if(attach){if(e.EndNote>=boundary)x2=F((double)x2-1.5);if(stem1==stem2){y1=Attach(rows[first],pitch1,stem1,direction);y2=Attach(rows[last],pitch2,stem2,direction);if(stem1==direction)offset=direction==10?-4:0;}
    else{float difference=F((double)pitch1-pitch2);if(direction==stem1){y1=y2=Attach(rows[last],pitch2,stem2,direction);if(difference>0&&direction==10)y1=F((double)pitch1+2);if(difference<0&&direction==20)y1=F((double)pitch1-2);}else{y1=y2=Attach(rows[first],pitch1,stem1,direction);if(difference>0&&direction==20)y2=F((double)pitch2-2);if(difference<0&&direction==10)y2=F((double)pitch2+2);}}
    if(mode!=1){y1=AvoidLine(y1,direction);y2=AvoidLine(y2,direction);}if(number!=0){offset=3;height=2;}else height=InitialHeight(x1,x2,y1,y2,offset,0);
   }else height=InitialHeight(x1,x2,y1,y2,offset,0);
   if(direction==20)height=-height;
   if(mode==0){y1=CorrectMark(rows[first],y1,direction,stem1,number);y2=CorrectMark(rows[last],y2,direction,stem2,number);if(e.EndNote-e.StartNote>1)AvoidInterior(rows,first,last,x1,x2,direction,number,ref y1,ref y2,ref height);}
   var result=new float[15];result[0]=5;result[1]=staff;result[2]=x1;result[3]=y1;result[4]=y2;result[5]=x2;result[6]=height;result[7]=offset;result[8]=number;int originalCount=rows.Count;rows.Add(result);
   if(mode>=1){int members=1;while(first+members<originalCount&&rows[first+members][2]==rows[first][2])members++;if(members>1){int remaining=members,threshold=members/2+1,pointer=first;while(remaining>1){if(remaining==threshold){height=-height;direction=30-direction;}remaining--;pointer++;float pitch=rows[pointer][3]%100;float shift=W(pitch)%2!=0?1.2000000476837158f:.5f;if(direction==20)shift=-shift;float y=F((double)pitch+shift);var parallel=(float[])result.Clone();parallel[3]=parallel[4]=y;parallel[6]=height;rows.Add(parallel);}}}
   return rows;
  }
  static void AvoidInterior(List<float[]> rows,int first,int last,float x1,float x2,int direction,int number,ref float y1,ref float y2,ref float height){if(x1==x2)throw new NotSupportedException("Zero horizontal slur span requires the fault branch.");int start=first;
   while(true){float slope=F(((double)y2-y1)/((double)x2-x1)),baseline=F((double)y1-(direction==20?-2.5:2.5));bool restart=false;for(int i=start+1;i<last;i++){var p=rows[i];if(p[0]!=1)continue;int stem=(W(p[4])%100)/10*10;if(stem==direction)continue;float line=F(((double)p[2]-x1)*slope+baseline+height),mark=0;if(p[10]>0){float part=F((double)(p[10]%1)*10);mark=stem==10?F(-(double)part-2):F(2.0+part);}float pitch=p[3];if(pitch<100){if(pitch<=-100)pitch=F(100.0+pitch);}else pitch=F((double)pitch-100);float obstacle=F((double)pitch+mark);if(direction==20?obstacle<line:line<obstacle){float difference=F((double)obstacle-line);if(number==0)height=F((double)difference+height);else{float shift=F(.5+difference);if(direction==20)shift=F((double)shift-1);y1=F((double)shift+y1);y2=F((double)shift+y2);start=i;restart=true;break;}}}if(!restart)return;
   }
  }
 }
}
