using System;
static class IndependentFlagArithmetic {
 public static uint Apply(uint prior,ushort a,ushort b,int width,bool add){int mask=width==8?255:65535,sign=width==8?128:32768;a=(ushort)(a&mask);b=(ushort)(b&mask);int n=add?a+b:a-b;int result=n&mask;int sa=a>=sign?a-(mask+1):a,sb=b>=sign?b-(mask+1):b,signed=add?sa+sb:sa-sb;uint bits=0;if(add?n>mask:n<0)bits|=1;if(add?(a&15)+(b&15)>15:(a&15)<(b&15))bits|=16;if(signed < -sign || signed>=sign)bits|=2048;return Result(prior,bits,(ushort)result,width);}
 public static uint Result(uint prior,uint bits,ushort result,int width){int mask=width==8?255:65535,sign=width==8?128:32768;int r=result&mask;if(r==0)bits|=64;if((r&sign)!=0)bits|=128;bool odd=false;for(int n=0;n<8;n++)if((r&(1<<n))!=0)odd=!odd;if(!odd)bits|=4;return (prior&~0x8d5u)|bits;}
}
