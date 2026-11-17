using System;using System.Globalization;
namespace ScoreModern { public sealed partial class Editor {
 void OriginalAbsoluteSize(string argument){
  var item=canvas.Selected;if(item==null)return;
  if(item.Code>=4&&item.Code<=6||item.Code==8||item.Code==17)return;
  if(item.Code<1||item.Code>18)throw new NotSupportedException("SZ item code outside the recovered dispatch table.");
  float wanted=0;if(argument.Length!=0&&(!Single.TryParse(argument,NumberStyles.Float,CultureInfo.InvariantCulture,out wanted)||Single.IsNaN(wanted)||Single.IsInfinity(wanted)))throw new InvalidOperationException("SZ requires a finite absolute size.");
  canvas.LayoutScore();int staff=CenterWord(item.P(2));if(staff<1||staff>32)throw new NotSupportedException("SZ requires a recovered staff-size entry.");
  float factor=(float)((double)canvas.PageDocument.Tables.Scales[staff]/25);if(factor==0)factor=1;
  int parameter=item.Code<=2?15:item.Code==18?7:6;
  if(item.Code==7)parameter=item.P(7)==20?9:5;
  if(item.Code==14){
   if(!(item.P(5)>0))return;
   if(wanted==0){ShowAbsoluteSize(item.P(17)==0?factor:item.P(17),factor);return;}
   EditOriginalParameters(new[]{17f,wanted});return;
  }
  // 2FAC:10EB..122C preserves the field's integer hundreds independently.
  if(wanted==0){float size=(float)(((double)item.P(parameter)%100)*factor);if(size==0)size=factor;ShowAbsoluteSize(size,factor);return;}
  int hundreds=CenterWord(item.P(parameter))/100*100;
  float value=(float)((double)hundreds+(double)wanted/factor);
  if(item.Code==12&&BitConverter.ToInt32(BitConverter.GetBytes(item.P(5)),0)>=0&&BitConverter.ToInt32(BitConverter.GetBytes(item.P(7)),0)!=0){
   float old=item.P(6);if(BitConverter.ToInt32(BitConverter.GetBytes(old),0)==0)old=1;
   if(old==0)throw new NotSupportedException("SZ negative-zero shape denominator requires original exception-state verification.");
   float height=(float)((double)item.P(7)*value/old);if(Single.IsNaN(height)||Single.IsInfinity(height))throw new NotSupportedException("SZ shape ratio exceeds the verified finite domain.");
   EditOriginalParameters(new[]{(float)parameter,value,7f,height});
  }else EditOriginalParameters(new[]{(float)parameter,value});
 }
 void ShowAbsoluteSize(float size,float staff){pendingEditAcknowledgement=true;Message("*** Absolute size ="+ExportFixed(size,2).PadLeft(7)+"    Staff size ="+ExportFixed(staff,2).PadLeft(5)+Environment.NewLine+"Press any key to continue.");}
}}
