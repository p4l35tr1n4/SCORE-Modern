using System;
using System.Collections.Generic;

namespace ScoreModern {
 internal sealed class NativePageCoordinateBank {
  internal float[] Words;internal int[] Starts,Lengths;internal short[] References;
  internal List<Item> Items(){var result=new List<Item>();for(int n=0;n<Starts.Length;n++){var bytes=new byte[Lengths[n]*4];Buffer.BlockCopy(Words,(Starts[n]-1)*4,bytes,0,bytes.Length);result.Add(new Item(bytes));}return result;}
 }
 internal static class NativePageCoordinates {
  // PAGE 096E:0001..05C8: complete-bank mode-zero collector. PAGE's version
  // differs from Main J's collector, notably for Code 4 compound endpoints.
  internal static NativePageCoordinateBank Collect(IList<Item> items,float left,float right){
   var bank=new NativePageCoordinateBank{Words=new float[14200],Starts=new int[items.Count],Lengths=new int[items.Count]};var references=new List<short>();int at=0;
   for(int n=0;n<items.Count;n++){
    var item=items[n];if(item.Count<3||at+item.Count+1>bank.Words.Length)throw new NotSupportedException("PAGE coordinate bank exceeds the original extent or has a short record.");
    bank.Starts[n]=at+1;bank.Lengths[n]=item.Count+1;Buffer.BlockCopy(item.Bytes,0,bank.Words,at*4,item.Bytes.Length);int start=at;at+=item.Count+1;
    if(left<=bank.Words[start+3]&&bank.Words[start+3]<=right)references.Add((short)(start+4));
    if(item.Count<6||item.Code<4||item.Code>8)continue;
    // Zero-ended arpeggios/vertical signs keep their discriminator during fitting.
    if(item.Code==4&&item.P(7)<0&&item.P(6)==0)continue;
    if(item.Code==8&&bank.Words[start+6]==0)bank.Words[start+6]=200;
    Action<int> extra=parameter=>{float value=bank.Words[start+parameter];if(left<=value&&value<=right)references.Add((short)-(start+parameter+1));};
    extra(6);if(item.Code<6||item.Count<8)continue;
    if(item.Code==7){if(item.P(7)>=20&&item.P(8)>0)extra(8);continue;}
    if(item.Code!=6||item.Count<11)continue;
    if(item.P(11)>0)extra(11);if(item.P(7)<0||item.Count<12)continue;
    if(item.P(12)>0)extra(12);if(item.Count<14||item.P(13)==0)continue;
    if(item.P(14)>0)extra(14);if(item.Count>=15&&item.P(15)>0)extra(15);
   }
   bank.References=references.ToArray();return bank;
  }
  // PAGE 2F23:0731..0797 uses the collector's reusable ordered references.
  internal static List<Item> Rebase(IList<Item> items,float cut){var bank=Collect(items,0,20000);float delta=(float)((double)cut-10);foreach(short reference in bank.References){int index=Math.Abs((int)reference)-1;bank.Words[index]=(float)((double)bank.Words[index]-delta);}return bank.Items();}
  // PAGE 0B45:0986..0A54: retained REAL4 ratio, then retained REAL4 offset
  // including its literal .001 before transforming the collected words.
  internal static List<Item> Fit(IList<Item> items,float oldLeft,float oldRight,float newLeft,float newRight){
   if(oldRight==oldLeft)throw new NotSupportedException("PAGE has a zero-width affine interval.");var bank=Collect(items,oldLeft,oldRight);float ratio=(float)(((double)newRight-newLeft)/((double)oldRight-oldLeft));float offset=(float)((double)newLeft-(double)oldLeft*ratio+0.0010000000474974513);
   foreach(short reference in bank.References){int index=Math.Abs((int)reference)-1;bank.Words[index]=(float)((double)bank.Words[index]*ratio+offset);}return bank.Items();
  }
 }
}
