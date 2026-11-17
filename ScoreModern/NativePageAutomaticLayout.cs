using System;
using System.Linq;
using System.Collections.Generic;
namespace ScoreModern {
 internal sealed class NativePageAutomaticResult {
  internal short[] Bars,Widths,PageList;
 }
 internal sealed class NativePageAutomaticLayout {
  readonly NativePageAutomaticMachine machine=new NativePageAutomaticMachine();
  static void W(byte[] data,int at,int value){Buffer.BlockCopy(BitConverter.GetBytes(unchecked((short)value)),0,data,at,2);}
  static void F(byte[] data,int at,float value){Buffer.BlockCopy(BitConverter.GetBytes(value),0,data,at,4);}
  void Word(int segment,int offset,int value){W(machine.Memory,segment*16+offset,value);}
  void Float(int segment,int offset,float value){F(machine.Memory,segment*16+offset,value);}
  short Read(int segment,int offset){return BitConverter.ToInt16(machine.Memory,segment*16+offset);}
  internal NativePageAutomaticLayout(){machine.Load(0x1000,new byte[65536]);machine.Load(0x2000,new byte[65536]);machine.Load(0x3000,new byte[65536]);machine.Load(0x4000,new byte[512]);machine.Load(0x5000,new byte[128]);machine.Load(0x5100,new byte[4096]);}
  // Retained native work banks mirror the recovered utility's working-state
  // structure. No original heap or executable is read by this calculation.
  internal NativePageAutomaticResult Apply(short[] weights,short[] leading,int lines,int linesPerPage,float pageHeight,float coordinateUnits,bool allowOriginalInvalid=false,int? proposalTotal=null){
   if(weights==null||leading==null||weights.Length==0||weights.Length>1000||leading.Length!=weights.Length||lines<1||lines>=200||linesPerPage<1||linesPerPage>32767)throw new NotSupportedException("PAGE automatic layout leaves its verified capacities.");
   int total=proposalTotal??weights.Sum(x=>(int)x);if(total<1||total>32767||weights.Any(x=>x<0))throw new NotSupportedException("PAGE automatic weights leave the positive signed-word domain.");
   if(Single.IsNaN(pageHeight)||Single.IsInfinity(pageHeight)||pageHeight<=0||Single.IsNaN(coordinateUnits)||Single.IsInfinity(coordinateUnits)||coordinateUnits<=0)throw new ArgumentException("PAGE height and coordinate units must be finite and positive.");
   Word(0x1000,0x25be,0x5000);Word(0x1000,0x25c0,0x5100);Float(0x1000,0x25c2,69);Float(0x1000,0x25c6,.3f);Word(0x1000,0x43f4,lines);Word(0x1000,0x41c0,1);Word(0x1000,0x41c2,linesPerPage);Word(0x1000,0x41c4,1);Word(0x1000,0x3d00,0);Word(0x4000,0,weights.Length);Word(0x5000,0,total);Word(0x5000,2,30000);Word(0x5000,4,1);Float(0x5000,6,pageHeight);Float(0x5000,10,coordinateUnits);Float(0x5000,14,pageHeight);Word(0x5100,0,1);
   for(int n=0;n<weights.Length;n++){Word(0x2000,0xbb8+n*2,weights[n]);Word(0x1000,0x2d40+n*2,leading[n]);}
   Word(0x1000,0xf000,0);Word(0x1000,0xf002,0xffff);int[] args={0x1388,0x2000,0x2d40,0x1000,0,0x3000,0x3e8,0x2000,0x1b58,0x2000,0,0x4000,0xbb8,0x2000};for(int n=0;n<args.Length;n++)Word(0x1000,0xf004+n*2,args[n]);
   machine.DS=machine.SS=0x1000;machine.SP=0xf000;machine.CS=0x3197;machine.IP=0xb;machine.DF=false;machine.Run();
   var bars=Enumerable.Range(0,lines).Select(n=>Read(0x2000,0x1b58+n*2)).ToArray();var widths=Enumerable.Range(0,lines).Select(n=>Read(0x3000,n*2)).ToArray();int pageWords=Read(0x5100,0)+1;if(pageWords<1||pageWords>2048)throw new NotSupportedException("PAGE page-list extent leaves its original bank.");var pages=Enumerable.Range(0,pageWords).Select(n=>Read(0x5100,n*2)).ToArray();
   if(!allowOriginalInvalid&&(bars.Any(x=>x<1)||bars.Sum(x=>(int)x)!=weights.Length))throw new InvalidOperationException("PAGE produced an empty or out-of-range line. Change the requested number of lines.");
   return new NativePageAutomaticResult{Bars=bars,Widths=widths,PageList=pages};
  }
 }
}
