using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScoreModern {
 internal sealed class NativePageFileBank {
  readonly byte[] bank;
  // The initial physical bank is explicit. A zero-filled native workspace
  // and a captured/recovered original retained workspace are distinct cases.
  internal NativePageFileBank(byte[] retained){if(retained==null||retained.Length!=14200*4)throw new ArgumentException("PAGE requires its explicit 14200-word output workspace.");bank=(byte[])retained.Clone();}
  internal NativePageFileBank Clone(){return new NativePageFileBank(bank);}
  internal byte[] RetainPage(byte[] encoded){
   int payload=encoded.Length-2;
   int unwritten=payload-20;
   var result=(byte[])encoded.Clone();
   Array.Copy(bank,unwritten,result,unwritten+2,4);
   Array.Copy(result,2,bank,0,payload);
   return result;
  }
  // PAGE 2E8F:0665..076B, binary kind 3. The first word after the records
  // is deliberately unwritten; version/measurement/count/end follow it.
  // This older PAGE footer is separate from SCORE's six-word save trailer.
  internal byte[] Encode(IList<Item> records,short measurement){
   if(measurement!=0&&measurement!=1)throw new NotSupportedException("PAGE measurement is outside the verified inch/metric codes.");int words=records.Sum(item=>item.Bytes.Length/4);if(words+5>14200)throw new NotSupportedException("PAGE output exceeds its original parameter bank.");int at=0;foreach(var item in records){Array.Copy(item.Bytes,0,bank,at,item.Bytes.Length);at+=item.Bytes.Length;}
   float[] tail={3,measurement,4,-9999};for(int n=0;n<tail.Length;n++)Array.Copy(BitConverter.GetBytes(tail[n]),0,bank,(words+1+n)*4,4);
   using(var stream=new MemoryStream())using(var writer=new BinaryWriter(stream)){writer.Write((ushort)(words+5));writer.Write(bank,0,(words+5)*4);return stream.ToArray();}
  }
 }
}
