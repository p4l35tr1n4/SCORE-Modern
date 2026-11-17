using System;
namespace ScoreModern.RecoveredFragments {
 public struct LineClippingWords {public ushort AX,BX,CX,DX,SI,DI,Exit;}
 public struct LineClippingFlagResult {public LineClippingWords Words;public uint EFlags;}
 public sealed class LineClippingDivideFault:System.Exception {
  public LineClippingWords Words;public uint EFlags;public bool HasEFlags;
  public LineClippingDivideFault():base("Original clipping IDIV requests vector-zero delivery."){}
 }
 public static class LineClipping {
  static void Multiply(ref ushort ax,ref ushort dx,ushort b,FlagState flags){int n=(short)ax*(short)b;ax=(ushort)n;dx=(ushort)(n>>16);
   // Explicit DOSBox-X 2026.10.01 normal-core policy for architecturally undefined IMUL flags.
   if(flags!=null){uint bits=0;if(ax==0)bits|=64;if((ax&0x8000)!=0)bits|=128;if(n<short.MinValue||n>short.MaxValue)bits|=0x801;flags.Value=(flags.Value&~0x8c1u)|bits;}}
  static void Divide(ref ushort ax,ref ushort dx,ushort b,ushort site,ushort bx,ushort si,ushort di,FlagState flags){int n=unchecked((int)(((uint)dx<<16)|ax));int d=(short)b;
   bool bad=d==0||(n==int.MinValue&&d==-1);int q=bad?0:n/d;
   if(bad||q<short.MinValue||q>short.MaxValue)throw new LineClippingDivideFault{Words=new LineClippingWords{AX=ax,BX=bx,CX=b,DX=dx,SI=si,DI=di,Exit=site},EFlags=flags==null?0:flags.Value,HasEFlags=flags!=null};
   dx=(ushort)(n%d);ax=(ushort)q;}
  sealed class FlagState {
   public uint Value;
   public void Subtract(ushort a,ushort b){ushort r=(ushort)(a-b);uint bits=0;if(a<b)bits|=1;if(((a^b^r)&16)!=0)bits|=16;if(r==0)bits|=64;if((r&0x8000)!=0)bits|=128;if(((a^b)&(a^r)&0x8000)!=0)bits|=0x800;
    int parity=r&255;parity^=parity>>4;parity^=parity>>2;parity^=parity>>1;if((parity&1)==0)bits|=4;Value=(Value&~0x8d5u)|bits;}
  }
  // Original relative 1807:078C..0980. Normal exits have defined arithmetic flags.
  public static LineClippingFlagResult ExecuteWithFlags(IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di,uint eflags){
   FlagState f=new FlagState{Value=eflags};LineClippingWords words=ExecuteCore(m,ds,ax,bx,cx,dx,si,di,f);return new LineClippingFlagResult{Words=words,EFlags=f.Value};
  }
  public static LineClippingWords Execute(IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di){
   return ExecuteCore(m,ds,ax,bx,cx,dx,si,di,null);
  }
  static LineClippingWords ExecuteCore(IHorizontalRasterMemory m,ushort ds,ushort ax,ushort bx,ushort cx,ushort dx,ushort si,ushort di,FlagState flags){
   System.Func<ushort,ushort,int> compare=(a,b)=>{if(flags!=null)flags.Subtract(a,b);return ((short)a).CompareTo((short)b);};
   int iterations=0;ushort temp;bool positive;
   if(compare(ax,m.ReadWord(ds,0x4a7c))>=0)goto Reject;
   cx=m.ReadWord(ds,0x4b6e);if(compare(cx,m.ReadWord(ds,0x4a76))<0)goto Reject;
   cx=m.ReadWord(ds,0x4b68);if(compare(cx,dx)<=0)goto Ordered;
   temp=dx;dx=cx;cx=temp;
  Ordered:
   if(compare(cx,m.ReadWord(ds,0x4a80))>=0)goto Reject;
   if(compare(dx,m.ReadWord(ds,0x4a72))<0)goto Reject;
   if(compare(cx,dx)==0)goto Vertical;
   if(compare(bx,0)!=0)goto General;
   if(compare(cx,m.ReadWord(ds,0x4a72))<0)cx=m.ReadWord(ds,0x4a72);
   if(compare(dx,m.ReadWord(ds,0x4a80))>=0)dx=m.ReadWord(ds,0x4a7e);
   m.WriteWord(ds,0x4b68,cx);m.WriteWord(ds,0x4b6c,dx);goto Horizontal;
  Vertical:
   if(compare(ax,m.ReadWord(ds,0x4a76))<0){ax=m.ReadWord(ds,0x4a76);m.WriteWord(ds,0x4b6a,ax);}
   bx=m.ReadWord(ds,0x4b6e);
   if(compare(bx,m.ReadWord(ds,0x4a7c))>=0){bx=m.ReadWord(ds,0x4a7a);m.WriteWord(ds,0x4b6e,bx);}
   goto VerticalExit;
  General:
   if(compare(bx,0)==0)goto Horizontal;
   dx=m.ReadWord(ds,0x4b6c);temp=m.ReadWord(ds,0x4b68);positive=(short)temp<(short)dx;if(flags!=null)flags.Subtract(dx,temp);dx=(ushort)(dx-temp);
   if(dx==0){bx=m.ReadWord(ds,0x4b6e);goto VerticalExit;}
   m.WriteWord(ds,0x4b72,bx);si=bx;
   if(!positive)dx=(ushort)(-dx);
   m.WriteWord(ds,0x4b70,dx);di=dx;
  Reclip:
   if(++iterations>65536)throw new NotSupportedException("Non-terminating clipping projection boundary.");
   dx=m.ReadWord(ds,0x4b6e);ax=m.ReadWord(ds,0x4b6a);bx=m.ReadWord(ds,0x4b6c);cx=m.ReadWord(ds,0x4b68);
   di=(ushort)(cx-bx);si=(ushort)(ax-dx);
   if(compare(cx,m.ReadWord(ds,0x4a80))>=0)goto FirstRight;
   if(compare(cx,m.ReadWord(ds,0x4a72))<0)goto FirstLeft;
   if(compare(bx,m.ReadWord(ds,0x4a80))>=0)goto SecondRight;
   if(compare(bx,m.ReadWord(ds,0x4a72))<0)goto SecondLeft;
   if(compare(ax,m.ReadWord(ds,0x4a7c))>=0)goto FirstBottom;
   if(compare(ax,m.ReadWord(ds,0x4a76))<0)goto FirstTop;
   if(compare(dx,m.ReadWord(ds,0x4a7c))>=0)goto SecondBottom;
   if(compare(dx,m.ReadWord(ds,0x4a76))<0)goto SecondTop;
   dx=si;dx=(ushort)(-dx);m.WriteWord(ds,0x4b72,dx);bx=di;
   if(compare(bx,0)<=0){if(flags!=null)flags.Subtract(0,bx);bx=(ushort)(-bx);}
   m.WriteWord(ds,0x4b70,bx);di=bx;dx=di;bx=si;goto Sloped;
  FirstRight:
   cx=m.ReadWord(ds,0x4a7e);if(compare(m.ReadWord(ds,0x4b6c),cx)>0)goto Reject;
  AdjustFirstX:
   m.WriteWord(ds,0x4b68,cx);ax=cx;if(flags!=null)flags.Subtract(ax,bx);ax=(ushort)(ax-bx);cx=si;Multiply(ref ax,ref dx,cx,flags);cx=di;Divide(ref ax,ref dx,cx,0x08d3,bx,si,di,flags);
   ax=(ushort)(ax+m.ReadWord(ds,0x4b6e));m.WriteWord(ds,0x4b6a,ax);goto Reclip;
  FirstLeft:
   cx=m.ReadWord(ds,0x4a72);if(compare(m.ReadWord(ds,0x4b6c),cx)<0)goto Reject;goto AdjustFirstX;
  SecondRight:
   bx=m.ReadWord(ds,0x4a7e);if(compare(m.ReadWord(ds,0x4b68),bx)>0)goto Reject;
  AdjustSecondX:
   m.WriteWord(ds,0x4b6c,bx);ax=cx;if(flags!=null)flags.Subtract(ax,bx);ax=(ushort)(ax-bx);cx=si;Multiply(ref ax,ref dx,cx,flags);cx=di;Divide(ref ax,ref dx,cx,0x0908,bx,si,di,flags);
   ax=(ushort)(ax-m.ReadWord(ds,0x4b6a));ax=(ushort)(-ax);m.WriteWord(ds,0x4b6e,ax);goto Reclip;
  SecondLeft:
   bx=m.ReadWord(ds,0x4a72);if(compare(m.ReadWord(ds,0x4b68),bx)<0)goto Reject;goto AdjustSecondX;
  FirstBottom:
   ax=m.ReadWord(ds,0x4a7a);if(compare(m.ReadWord(ds,0x4b6e),ax)>0)goto Reject;
  AdjustFirstY:
   m.WriteWord(ds,0x4b6a,ax);if(flags!=null)flags.Subtract(ax,dx);ax=(ushort)(ax-dx);cx=di;Multiply(ref ax,ref dx,cx,flags);cx=si;Divide(ref ax,ref dx,cx,0x0936,bx,si,di,flags);
   ax=(ushort)(ax+bx);m.WriteWord(ds,0x4b68,ax);goto Reclip;
  FirstTop:
   ax=m.ReadWord(ds,0x4a76);if(compare(m.ReadWord(ds,0x4b6e),ax)<0)goto Reject;goto AdjustFirstY;
  SecondBottom:
   dx=m.ReadWord(ds,0x4a7a);if(compare(m.ReadWord(ds,0x4b6a),dx)>0)goto Reject;
  AdjustSecondY:
   m.WriteWord(ds,0x4b6e,dx);if(flags!=null)flags.Subtract(ax,dx);ax=(ushort)(ax-dx);cx=di;Multiply(ref ax,ref dx,cx,flags);cx=si;Divide(ref ax,ref dx,cx,0x0964,bx,si,di,flags);
   ax=(ushort)(ax-m.ReadWord(ds,0x4b68));ax=(ushort)(-ax);m.WriteWord(ds,0x4b6c,ax);goto Reclip;
  SecondTop:
   dx=m.ReadWord(ds,0x4a76);if(compare(m.ReadWord(ds,0x4b6a),dx)<0)goto Reject;goto AdjustSecondY;
  Horizontal:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0ac6};
  VerticalExit:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0a96};
  Sloped:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0981};
  Reject:return new LineClippingWords{AX=ax,BX=bx,CX=cx,DX=dx,SI=si,DI=di,Exit=0x0b16};
  }
 }
}
