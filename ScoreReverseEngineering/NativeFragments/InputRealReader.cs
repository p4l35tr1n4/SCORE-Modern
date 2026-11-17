using System;
namespace ScoreModern.RecoveredFragments {
 public static class InputRealReader {
  // Resident16D3:05EF..0799. The caller supplies a cursor; the first
  // nondigit terminates the value (there is no exponent grammar). Sign,
  // signed32 mantissa and decimal power have separate stored words.
  public static float Read(string text,ref int at){
   int beginning=at,p=at,decimalAt=-1,digits=0;int mantissa=0;float sign=1;
   if(p<text.Length&&text[p]=='+')p++;
   while(p<text.Length){
    char c=text[p];
    if(c=='-'){sign=-sign;p++;while(p<text.Length&&text[p]==' ')p++;}
    else if(c=='.'){decimalAt=p;p++;}
    else if(c>='0'&&c<='9'){mantissa=unchecked(mantissa*10+c-'0');digits++;p++;}
    else break;
    if(p-beginning>=12)return 0;
   }
   if(digits==0||mantissa<0)return 0;
   float storedMantissa=mantissa,storedPower=decimalAt<0?1:(float)Math.Pow(10,p-decimalAt-1);
   float value=(float)((double)sign*storedMantissa/storedPower);
   if(value==0&&sign<0)value=-1;
   at=p;return value;
  }
 }
}
