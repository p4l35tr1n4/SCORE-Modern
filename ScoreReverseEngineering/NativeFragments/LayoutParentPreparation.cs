using System;using System.Linq;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 // Projection of 2D31:0155..05E1 before the LI/J core. Duplicate deletion is
 // deliberately interactive: callers must supply each measured Y/N decision.
 public sealed class LayoutParentPreparation {
  public readonly List<float[]> Rows;public short Bottom,Top;public float Left,Right;
  public readonly List<int> Origins;
  public bool Modified,Complete;public int DuplicateIndex=-1;int scan;readonly bool lineup;
  static int Bits(float f){return BitConverter.ToInt32(BitConverter.GetBytes(f),0);}
  static short Word(float f){long n;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(f),0),out n))throw new NotSupportedException("Layout word conversion is outside its finite domain.");return unchecked((short)n);}
  static float P(float[] r,int p){return r.Length>=p?r[p-1]:0;}
  void Sort(){int[] order=InputTranspositionOrder.OrderForMain(Rows,false);var next=order.Select(i=>Rows[i]).ToArray();var origins=order.Select(i=>Origins[i]).ToArray();Rows.Clear();Rows.AddRange(next);Origins.Clear();Origins.AddRange(origins);}
  public LayoutParentPreparation(IList<float[]> rows,short bottom,short top,float left,float right,bool alreadySorted,bool lineupMode=false) {
   lineup=lineupMode;
   Rows=rows.Select(r=>(float[])r.Clone()).ToList();Origins=Enumerable.Range(0,rows.Count).ToList();Bottom=bottom;Top=top;Left=left;Right=right;
   if(!alreadySorted){Modified=InputTranspositionOrder.MarksModified(Rows.Count,false);Sort();}
   float previous=-9999;
   foreach(var row in Rows){int code=Word(P(row,1));if(code==1){previous=P(row,3);continue;}if(code==4&&row.Length>=7&&Bits(P(row,6))==0&&Bits(P(row,7))<0&&Math.Abs((double)previous-P(row,3))<0.1f){row[2]=(float)((double)previous-0.001f);Modified=true;}previous=-9999;}
   Sort();FindDuplicate();
  }
  void FindDuplicate(){
   DuplicateIndex=-1;
   while(scan<Rows.Count-1){var row=Rows[scan];var next=Rows[scan+1];short staff=Word(P(row,2)),code=Word(P(row,1));if(lineup&&staff>=Bottom&&staff<=Top&&code==8){Left=row.Length>=3?row[2]:0;Right=row.Length>=6&&Bits(row[5])!=0?row[5]:200;}if(staff>=Bottom&&staff<=Top&&(code==3||code==8||code==17||code==18)&&P(next,1)==code&&P(next,2)==staff&&Math.Abs((double)P(row,3)-P(next,3))<=0.2f){DuplicateIndex=scan;return;}scan++;}
   FinishAnchors();
  }
  public void AnswerDuplicate(bool delete){if(DuplicateIndex<0||Complete)throw new InvalidOperationException("No layout duplicate decision is pending.");if(delete){Rows.RemoveAt(scan);Origins.RemoveAt(scan);Modified=true;}else scan++;FindDuplicate();}
  void FinishAnchors(){
   if(Rows.Count==0){Complete=true;return;}
   short maximum=Rows[Rows.Count-1].Length<1?(short)1:Word(P(Rows[Rows.Count-1],2));if(maximum<Bottom){Complete=true;return;}if(maximum<Top)Top=maximum;
   int cursor=0;
   for(int staff=Bottom;staff<=Top;staff++) {
    float origin=-9999;int clef=-1;
    for(int i=cursor;i<Rows.Count;i++){
     var row=Rows[i];short code=Word(P(row,1));if(Word(P(row,2))!=staff||(code!=8&&code!=3))continue;
     if(row.Length<3){Array.Resize(ref row,3);Rows[i]=row;Modified=true;}
     if(code==3){clef=i;if(origin==-9999)continue;}else{origin=row[2];if(clef<0)continue;}
     float x=Rows[clef][2];if(Bits(x)==Bits(origin)&&Left<=x&&Right>x){Rows[clef][2]=(float)((double)origin+1.5f);Modified=true;}
     cursor=i+1;break;
    }
   }
   Sort();Complete=true;
  }
 }
}
