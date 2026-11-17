using System;
namespace ScoreModern {
 internal static class NativePageSpacingHelperCalls {
  static float F(byte[] d,int a){return BitConverter.ToSingle(d,a&65535);}
  static short W(byte[] d,int a){return BitConverter.ToInt16(d,a&65535);}
  static void F(byte[] d,int a,double v){Buffer.BlockCopy(BitConverter.GetBytes((float)v),0,d,a&65535,4);}
  // Actual PAGE 02F1 -> 1E20:0A47: seven pointers to a six-parameter
  // helper. Parameters are DS:779A and count DS:778E, NOT the item.
  // Capture337 verifies entry stack, writes and return stack extent.
  internal static void Meter(byte[] d){
   if(d==null||d.Length!=65536)throw new ArgumentException("PAGE helper requires an explicit complete workspace.");
   F(d,0x778a,0);F(d,0x7778,7.5);int previous=W(d,0x777e);
   if(previous==14){F(d,0x7786,(double)F(d,0x7786)-1.5f);F(d,0x778a,.5);}
   if(previous==1)F(d,0x778a,2);if(previous==17)F(d,0x778a,-1.5);
   Func<int,float> p=n=>F(d,0x779a+(n-1)*4);float scratch=p(5);
   if((p(6)>=10||scratch>=10)&&scratch<=90)F(d,0x7778,(double)F(d,0x7778)+2.5f);
   int last=W(d,0x778e),scanned=0;while(p(last)==0){last=unchecked((short)(last-1));if(++scanned>16384)throw new NotSupportedException("PAGE meter helper has no nonzero word in its wrapped scan.");}
   if(last>=8){scratch=0;
    if(last<12||p(12)==0){scratch=4.5f;if(last>=10&&p(10)!=0)scratch=(float)((double)p(10)+scratch);}
    if(last>=11&&p(11)!=0)scratch=(float)((double)p(11)+scratch);
    float extra=p(8)>10||p(9)>10?6:3.5f;F(d,0x7778,(double)F(d,0x7778)+scratch+extra);
   }
  }
 }
}
