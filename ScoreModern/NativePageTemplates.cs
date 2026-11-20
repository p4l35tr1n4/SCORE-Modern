using System;
using System.Collections.Generic;
using System.Linq;

namespace ScoreModern {
 internal sealed partial class NativePageTemplates {
  readonly byte[] bank;
  internal NativePageTemplates(byte[] source){if(source==null||source.Length<10756)throw new ArgumentException("PAGE requires its seven by 32 staff template slots.");bank=(byte[])source.Clone();}
  internal byte[] Bytes{get{return (byte[])bank.Clone();}}
  int Offset(int type,int staff){if(type<1||type>7||staff<1||staff>32)throw new NotSupportedException("PAGE template slot is outside the original bank.");return (type-1)*1536+(staff-1)*48+4;}
  float Get(int type,int staff,int parameter){return BitConverter.ToSingle(bank,Offset(type,staff)+parameter*4);}
  void Set(int type,int staff,int parameter,float value){Array.Copy(BitConverter.GetBytes(value),0,bank,Offset(type,staff)+parameter*4,4);}
  Item Copy(int type,int staff){float raw=Get(type,staff,0);if(Single.IsNaN(raw)||Single.IsInfinity(raw)||raw<1||raw>=12)throw new NotSupportedException("PAGE template record length is outside its original 12-word slot.");int count=(int)Math.Truncate(raw);var bytes=new byte[(count+1)*4];Array.Copy(bank,Offset(type,staff),bytes,0,bytes.Length);return new Item(bytes);}
  // PAGE 084F:007D..033C. The caller has already removed Staff records
  // and executed the separate initial bar/time cleanup.
  internal List<Item> Insert(IList<Item> input){
   var rows=input.Select(item=>item.Clone()).ToList();int bottom=BitConverter.ToInt16(bank,0),top=BitConverter.ToInt16(bank,2);
   if(bottom<1||top>32||bottom>top)throw new NotSupportedException("PAGE template staff bounds are invalid.");
   for(int type=1;type<=7;type++)for(int staff=bottom;staff<=top;staff++){
    if((int)Math.Truncate(Get(type,staff,0))==0)continue;
    if(type>=6){
     for(int n=0;n<rows.Count;n++){
      var item=rows[n];if(item.Staff!=staff)continue;if(item.Code<=2)break;
      if(item.Code==17&&type==6){rows.RemoveAt(n--);continue;}
      if(item.Code==18&&type==7){Set(type,staff,0,0);break;}
     }
     if((int)Math.Truncate(Get(type,staff,0))==0)continue;
    }
    if(type==5){Set(6,staff,6,Get(5,staff,5));float count=Get(6,staff,0);if(BitConverter.ToInt32(BitConverter.GetBytes(count),0)!=0&&count<6)Set(6,staff,0,6);}
    rows.Add(Copy(type,staff));if(type==7)Set(type,staff,0,0);
    if(type==5)DeduplicateClefs(rows,staff);
   }
   return rows;
  }
  // PAGE resident 0B45:0A57..0C59. The appended starting clef is kept;
  // earlier matching clefs are removed or marked as changes with P4+100.
  void DeduplicateClefs(List<Item> rows,int staff){
   float code=Get(5,staff,1),owner=Get(5,staff,2);
   for(int n=0;n<rows.Count-1;n++){
    var item=rows[n];if(item.P(1)!=code||item.P(2)!=owner||code!=3)continue;float value=item.Count>=5?item.P(5):0;
    if(value==Get(5,staff,5)){rows.RemoveAt(n--);continue;}
    Set(5,staff,5,value);if(item.Count<4)item.Set(4,100);else if(item.P(4)<100)item.Set(4,(float)((double)item.P(4)+100));
   }
  }
 }
}
