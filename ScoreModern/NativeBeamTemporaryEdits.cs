using System;using System.Collections.Generic;using System.Linq;
namespace ScoreModern {
 public sealed class BeamTemporaryEditPlan {public float[] Operations;public Item Pending;}
 // Recovered from the live 2C0C:1164..1E66 edit-command builder; see Code6 debugger evidence.
 public static class NativeBeamTemporaryEdits {
  static int W(float v){if(Single.IsNaN(v)||Single.IsInfinity(v)||v<short.MinValue||v>short.MaxValue)throw new NotSupportedException("Beam edit control is outside the finite signed-word domain.");return unchecked((short)(int)Math.Truncate(v));}
  static bool Same(float a,float b){return BitConverter.ToInt32(BitConverter.GetBytes(a),0)==BitConverter.ToInt32(BitConverter.GetBytes(b),0);}
  static int Merge(int current,int incoming){return current==0?incoming:3;}
  static Item PendingPartial(Item beam,float main,float descriptor,float left,float right){var p=new float[12];for(int n=0;n<p.Length;n++)p[n]=beam.P(n+1);p[6]=main;p[7]=0;p[9]=descriptor;p[10]=left;p[11]=right;return Item.Make(p);}
  public static BeamTemporaryEditPlan Prepare(IList<Item> items,Item beam,float parameter,float value){
   if(beam==null||beam.Code!=6||value>=0||(parameter!=8&&parameter!=9))return null;
   if(parameter==9){if(W(value)>=0)throw new NotSupportedException("Sub-unit negative P9 controls enter an original retained-neighbor split path and are prevented.");int direction=Math.Abs(W(beam.P(7))/10),primary=0,secondary=0,tertiary=0;float previous=-9999;
    float a=beam.P(11)==-1?beam.P(3):beam.P(11),b=beam.P(12)==-1?beam.P(6):beam.P(12),c=beam.P(14)==-1?beam.P(3):beam.P(14),d=beam.P(15)==-1?beam.P(6):beam.P(15);
    foreach(var note in items){if(note.Code!=1||note.Staff!=beam.Staff||note.Count<5||note.P(3)<beam.P(3)||note.P(3)>beam.P(6)||Same(note.P(3),previous))continue;int stem=W(note.P(5))%100/10;if(stem==0||stem==direction)continue;float x=note.P(3);if(Same(x,beam.P(3)))primary=Merge(primary,1);if(Same(x,beam.P(6)))primary=Merge(primary,2);if(Same(x,a))secondary=Merge(secondary,1);if(Same(x,b))secondary=Merge(secondary,2);if(Same(x,c))tertiary=Merge(tertiary,1);if(Same(x,d))tertiary=Merge(tertiary,2);previous=x;}
    return new BeamTemporaryEditPlan{Operations=new[]{9f,(float)(primary*100+secondary*10+tertiary)}};
   }
   int type=W(beam.P(7));if(type<11||type>29||BitConverter.ToInt32(BitConverter.GetBytes(beam.P(13)),0)!=0)throw new NotSupportedException("Splitting an existing partial-beam arrangement requires its remaining original edit verification.");
   // Original converts the finite split control to a signed word before grouping.
   // Original -3.25/-3.5/-3.75/-333.5 traces match the integer split records.
   if(value>-1)throw new NotSupportedException("Sub-unit negative beam controls still require original verification.");int packed=Math.Abs(W(value));var xs=new List<float>();float last=-9999;
   foreach(var note in items)if(note.Code==1&&note.Staff==beam.Staff&&note.P(3)>=beam.P(3)&&note.P(3)<=beam.P(6)&&!Same(note.P(3),last)){last=note.P(3);xs.Add(last);}
   // Original rejects coincident/near-coincident entries while sorting its work bank.
   xs.Sort();for(int n=1;n<xs.Count;n++)if(Math.Abs((float)((double)xs[n]-xs[n-1]))<.10000000149011612f)throw new NotSupportedException("Beam split note positions are too close for SCORE's collector.");
   var groups=new List<int>();if(packed<100){groups.Add(packed);groups.Add(xs.Count-packed);}else{if(packed>9999)throw new NotSupportedException("Beam split exceeds four decimal groups.");foreach(char digit in packed.ToString(System.Globalization.CultureInfo.InvariantCulture))groups.Add(digit-'0');if(groups.Sum()!=xs.Count)throw new NotSupportedException("Beam split groups do not match the collected notes.");}
   if(groups.Count<2||groups.Count>4||groups.Any(n=>n<1))throw new NotSupportedException("Beam split groups must contain at least one collected note.");
   int one=groups[0],two=one+groups[1];if(two>xs.Count)throw new NotSupportedException("Beam split leaves the collected phrase.");float main=type/10*10+1,descriptor=type%10+9;
   int oldPartial=W(beam.P(10));if(oldPartial!=0){
    if(groups.Count!=2)throw new NotSupportedException("SCORE refuses decimal multi-group splitting when a secondary descriptor is already present.");
    float a=BitConverter.ToInt32(BitConverter.GetBytes(beam.P(11)),0)<0?beam.P(3):beam.P(11),b=BitConverter.ToInt32(BitConverter.GetBytes(beam.P(12)),0)<0?beam.P(6):beam.P(12),end=xs[one-1],next=xs[one];int sum=oldPartial/10+oldPartial%10;
    float resultMain=main,first=beam.P(10),left=beam.P(11),right=end,second=oldPartial,secondLeft=next,secondRight=b;Item extra=null;
    if(a<=end){
     if(a==end)right=0;
     else if(b<next){
      if(end==b){first=sum+9;right=beam.P(12);second=descriptor;secondRight=-1;}
      else{right=beam.P(12);second=descriptor;secondLeft=-1;secondRight=end;extra=PendingPartial(beam,main,descriptor,next,xs[xs.Count-1]);}
      if(Same(secondLeft,secondRight))throw new NotSupportedException("SCORE refuses this split because its generated partial-beam endpoints coincide.");
    return new BeamTemporaryEditPlan{Operations=new[]{7f,resultMain,10f,first,11f,left,12f,right,13f,second,14f,secondLeft,15f,secondRight},Pending=extra};
     }
     resultMain=beam.P(7);if(a!=end&&b==next)secondLeft=0;
    }else if(next==a){first=descriptor;left=-1;second=sum+9;}
    else{right=beam.P(12);second=descriptor;extra=PendingPartial(beam,main,descriptor,xs[0],end);}
    if(Same(secondLeft,secondRight))throw new NotSupportedException("SCORE refuses this split because its generated partial-beam endpoints coincide.");
    return new BeamTemporaryEditPlan{Operations=new[]{7f,resultMain,10f,first,11f,left,12f,right,13f,second,14f,secondLeft,15f,secondRight},Pending=extra};
   }
   var result=new BeamTemporaryEditPlan{Operations=new[]{7f,main,10f,descriptor,11f,-1f,12f,groups[0]==1?0:xs[one-1],13f,descriptor,14f,groups[1]==1?0:xs[one],15f,groups.Count==2?-1:xs[two-1]}};
   if(groups.Count>2){int length=groups.Count==3?12:15;var p=new float[length];for(int n=0;n<length;n++)p[n]=beam.P(n+1);p[6]=main;p[7]=0;p[9]=descriptor;p[10]=groups[2]==1?0:xs[two];p[11]=xs[two+groups[2]-1];if(groups.Count==4){p[12]=descriptor;p[13]=groups[3]==1?0:xs[two+groups[2]];p[14]=xs[xs.Count-1];}result.Pending=Item.Make(p);}
   return result;
  }
 }
}

namespace ScoreModern {
 public sealed partial class Editor {
  BeamTemporaryEditPlan pendingBeamTemporary;Item pendingBeamOwner;
  void ApplyBeamOperations(Item owner,float[] operations){for(int n=0;n<operations.Length;n+=2)owner.Set((int)operations[n],operations[n+1]);}
  void StageBeamTemporary(BeamTemporaryEditPlan plan,float[] entered){pendingBeamTemporary=plan;pendingBeamOwner=canvas.Selected;pendingEditAcknowledgement=true;canvas.BlockItemEditing=true;Message(String.Join(" ",Array.ConvertAll(entered,v=>v.ToString("R",System.Globalization.CultureInfo.InvariantCulture)))+Environment.NewLine+"**** Adding beam item - eXit edit to see it.");}
  void CommitBeamTemporary(){var plan=pendingBeamTemporary;if(plan==null)return;var owner=pendingBeamOwner;pendingBeamTemporary=null;pendingBeamOwner=null;canvas.BlockItemEditing=false;if(owner==null||owner!=canvas.Selected||!score.Items.Contains(owner))throw new InvalidOperationException("The pending beam owner changed.");Checkpoint();ApplyBeamOperations(owner,plan.Operations);score.Items.Add(plan.Pending.Clone());dirty=true;RefreshAll(owner);UpdateClassic();}
 }
}
