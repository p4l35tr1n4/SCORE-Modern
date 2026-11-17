// Generated PAGE computation statements; no executable bytes embedded.
// Original entry/output tests determine scope; see PAGE-ASSEMBLY-EVIDENCE.md.
using System;
namespace ScoreModern {
partial class NativePageAutomaticMachine {
 bool Instruction(){switch(((int)CS<<16)|IP){
case 0x3197000b:{Push(BP);IP=0xc;return true;} // PUSH BP
case 0x3197000c:{BP=unchecked((ushort)(SP));IP=0xe;return true;} // MOV BP,SP
case 0x3197000e:{SP=unchecked((ushort)(Sub(SP,42)));IP=0x11;return true;} // SUB SP,0x2a
case 0x31970011:{Push(DI);IP=0x12;return true;} // PUSH DI
case 0x31970012:{Push(SI);IP=0x13;return true;} // PUSH SI
case 0x31970013:{Sub(M16(DS,(0x43f4)),0);IP=0x18;return true;} // CMP word ptr [0x43f4],0x0
case 0x31970018:{IP=(ushort)((!ZF&&SF==OF)?0x20:0x1a);return true;} // JG 0xe000:0020
case 0x3197001a:{M16(DS,(0x43f4),1);IP=0x20;return true;} // MOV word ptr [0x43f4],0x1
case 0x31970020:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x23;return true;} // MOV AX,[0x43f4]
case 0x31970023:{M16(DS,(0x9ee8),AX);IP=0x26;return true;} // MOV [0x9ee8],AX
case 0x31970026:{M16(DS,(0x9e76),0);IP=0x2c;return true;} // MOV word ptr [0x9e76],0x0
case 0x3197002c:{ES=unchecked((ushort)(M16(DS,(0x25be))));IP=0x30;return true;} // MOV ES,word ptr [0x25be]
case 0x31970030:{AX=unchecked((ushort)(M16(ES,(0x0))));IP=0x34;return true;} // MOV AX,ES:[0x0]
case 0x31970034:{M16(DS,(0x9e6a),AX);IP=0x37;return true;} // MOV [0x9e6a],AX
case 0x31970037:{DX=(ushort)(((short)AX)<0?65535:0);IP=0x38;return true;} // CWD
case 0x31970038:{CX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x3c;return true;} // MOV CX,word ptr [0x43f4]
case 0x3197003c:{Divide(CX);IP=0x3e;return true;} // IDIV CX
case 0x3197003e:{M16(DS,(0x9ee6),AX);IP=0x41;return true;} // MOV [0x9ee6],AX
case 0x31970041:{M16(DS,(0x9efa),65535);IP=0x47;return true;} // MOV word ptr [0x9efa],0xffff
case 0x31970047:{M16(DS,(0x9efe),10000);IP=0x4d;return true;} // MOV word ptr [0x9efe],0x2710
case 0x3197004d:{M16(DS,(0x9ece),0);IP=0x53;return true;} // MOV word ptr [0x9ece],0x0
case 0x31970053:{M16(DS,(0x9ed4),0);IP=0x59;return true;} // MOV word ptr [0x9ed4],0x0
case 0x31970059:{M16(DS,(0x9eee),65535);IP=0x5f;return true;} // MOV word ptr [0x9eee],0xffff
case 0x3197005f:{M16(DS,(0x9ee2),10000);IP=0x65;return true;} // MOV word ptr [0x9ee2],0x2710
case 0x31970065:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x68;return true;} // LES BX,[BP + 0x16]
case 0x31970068:{M16(ES,(BX),1);IP=0x6d;return true;} // MOV word ptr ES:[BX],0x1
case 0x3197006d:{M16(DS,(0x9e6e),1);IP=0x73;return true;} // MOV word ptr [0x9e6e],0x1
case 0x31970073:{int a=(BP + 0x1a);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x76;return true;} // LES BX,[BP + 0x1a]
case 0x31970076:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0x79;return true;} // MOV AX,word ptr ES:[BX]
case 0x31970079:{M16(SS,(BP - 0xc),AX);IP=0x7c;return true;} // MOV word ptr [BP + -0xc],AX
case 0x3197007c:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0x7f;return true;} // MOV AX,[0x9e76]
case 0x3197007f:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x81;return true;} // SHL AX,0x1
case 0x31970081:{M16(SS,(BP - 0x16),AX);IP=0x84;return true;} // MOV word ptr [BP + -0x16],AX
case 0x31970084:{AX=unchecked((ushort)(M16(SS,(BP + 0xe))));IP=0x87;return true;} // MOV AX,word ptr [BP + 0xe]
case 0x31970087:{DX=unchecked((ushort)(M16(SS,(BP + 0x10))));IP=0x8a;return true;} // MOV DX,word ptr [BP + 0x10]
case 0x3197008a:{AX=unchecked((ushort)(Add(AX,M16(SS,(BP - 0x16)))));IP=0x8d;return true;} // ADD AX,word ptr [BP + -0x16]
case 0x3197008d:{AX=unchecked((ushort)(Sub(AX,2)));IP=0x90;return true;} // SUB AX,0x2
case 0x31970090:{M16(SS,(BP - 0x10),AX);IP=0x93;return true;} // MOV word ptr [BP + -0x10],AX
case 0x31970093:{M16(SS,(BP - 0xe),DX);IP=0x96;return true;} // MOV word ptr [BP + -0xe],DX
case 0x31970096:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x99;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x31970099:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x9c;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x3197009c:{AX=unchecked((ushort)(Add(AX,M16(SS,(BP - 0x16)))));IP=0x9f;return true;} // ADD AX,word ptr [BP + -0x16]
case 0x3197009f:{M16(SS,(BP - 0x14),AX);IP=0xa2;return true;} // MOV word ptr [BP + -0x14],AX
case 0x319700a2:{M16(SS,(BP - 0x12),DX);IP=0xa5;return true;} // MOV word ptr [BP + -0x12],DX
case 0x319700a5:{M16(DS,(0x9e7a),1);IP=0xab;return true;} // MOV word ptr [0x9e7a],0x1
case 0x319700ab:{AX=unchecked((ushort)(M16(DS,(0x9e6a))));IP=0xae;return true;} // MOV AX,[0x9e6a]
case 0x319700ae:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xaf;return true;} // CWD
case 0x319700af:{CX=unchecked((ushort)(M16(DS,(0x9ee8))));IP=0xb3;return true;} // MOV CX,word ptr [0x9ee8]
case 0x319700b3:{Divide(CX);IP=0xb5;return true;} // IDIV CX
case 0x319700b5:{M16(DS,(0x9ee4),AX);IP=0xb8;return true;} // MOV [0x9ee4],AX
case 0x319700b8:{AX=unchecked((ushort)(M16(SS,(BP + 0x1e))));IP=0xbb;return true;} // MOV AX,word ptr [BP + 0x1e]
case 0x319700bb:{DX=unchecked((ushort)(M16(SS,(BP + 0x20))));IP=0xbe;return true;} // MOV DX,word ptr [BP + 0x20]
case 0x319700be:{CX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0xc2;return true;} // MOV CX,word ptr [0x9e6e]
case 0x319700c2:{CX=unchecked((ushort)(Shl(CX,1)));IP=0xc4;return true;} // SHL CX,0x1
case 0x319700c4:{AX=unchecked((ushort)(Add(AX,CX)));IP=0xc6;return true;} // ADD AX,CX
case 0x319700c6:{AX=unchecked((ushort)(Sub(AX,2)));IP=0xc9;return true;} // SUB AX,0x2
case 0x319700c9:{M16(SS,(BP - 0xa),AX);IP=0xcc;return true;} // MOV word ptr [BP + -0xa],AX
case 0x319700cc:{M16(SS,(BP - 0x8),DX);IP=0xcf;return true;} // MOV word ptr [BP + -0x8],DX
case 0x319700cf:{int a=(BP - 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xd2;return true;} // LES BX,[BP + -0xa]
case 0x319700d2:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0xd5;return true;} // MOV AX,word ptr ES:[BX]
case 0x319700d5:{M16(DS,(0x9e72),AX);IP=0xd8;return true;} // MOV [0x9e72],AX
case 0x319700d8:{AX=unchecked((ushort)(M16(SS,(BP - 0xc))));IP=0xdb;return true;} // MOV AX,word ptr [BP + -0xc]
case 0x319700db:{M16(SS,(BP - 0x2),AX);IP=0xde;return true;} // MOV word ptr [BP + -0x2],AX
case 0x319700de:{AX=unchecked((ushort)(BX));IP=0xe0;return true;} // MOV AX,BX
case 0x319700e0:{M16(SS,(BP - 0x6),AX);IP=0xe3;return true;} // MOV word ptr [BP + -0x6],AX
case 0x319700e3:{M16(SS,(BP - 0x4),DX);IP=0xe6;return true;} // MOV word ptr [BP + -0x4],DX
case 0x319700e6:{SI=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0xea;return true;} // MOV SI,word ptr [0x9e7e]
case 0x319700ea:{DI=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0xee;return true;} // MOV DI,word ptr [0x9e6e]
case 0x319700ee:{IP=(ushort)((true)?0x10f:0xf0);return true;} // JMP 0xe000:010f
case 0x319700f0:{int a=(BP - 0x6);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xf3;return true;} // LES BX,[BP + -0x6]
case 0x319700f3:{SI=unchecked((ushort)(M16(ES,(BX))));IP=0xf6;return true;} // MOV SI,word ptr ES:[BX]
case 0x319700f6:{AX=unchecked((ushort)(SI));IP=0xf8;return true;} // MOV AX,SI
case 0x319700f8:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xf9;return true;} // CWD
case 0x319700f9:{AX=unchecked((ushort)(Sub(AX,DX)));IP=0xfb;return true;} // SUB AX,DX
case 0x319700fb:{AX=unchecked((ushort)(Sar(AX,1)));IP=0xfd;return true;} // SAR AX,0x1
case 0x319700fd:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9e72)))));IP=0x101;return true;} // ADD AX,word ptr [0x9e72]
case 0x31970101:{Sub(AX,M16(DS,(0x9ee4)));IP=0x105;return true;} // CMP AX,word ptr [0x9ee4]
case 0x31970105:{IP=(ushort)((SF==OF)?0x119:0x107);return true;} // JGE 0xe000:0119
case 0x31970107:{M16(DS,(0x9e7a),Add(M16(DS,(0x9e7a)),1));IP=0x10b;return true;} // INC word ptr [0x9e7a]
case 0x3197010b:{M16(DS,(0x9e72),Add(M16(DS,(0x9e72)),SI));IP=0x10f;return true;} // ADD word ptr [0x9e72],SI
case 0x3197010f:{M16(SS,(BP - 0x6),Add(M16(SS,(BP - 0x6)),2));IP=0x113;return true;} // ADD word ptr [BP + -0x6],0x2
case 0x31970113:{DI=unchecked((ushort)(Add(DI,1)));IP=0x114;return true;} // INC DI
case 0x31970114:{Sub(M16(SS,(BP - 0x2)),DI);IP=0x117;return true;} // CMP word ptr [BP + -0x2],DI
case 0x31970117:{IP=(ushort)((SF==OF)?0xf0:0x119);return true;} // JGE 0xe000:00f0
case 0x31970119:{M16(DS,(0x9e7e),SI);IP=0x11d;return true;} // MOV word ptr [0x9e7e],SI
case 0x3197011d:{M16(DS,(0x9e6e),DI);IP=0x121;return true;} // MOV word ptr [0x9e6e],DI
case 0x31970121:{M16(SS,(BP - 0x10),Add(M16(SS,(BP - 0x10)),2));IP=0x125;return true;} // ADD word ptr [BP + -0x10],0x2
case 0x31970125:{M16(SS,(BP - 0x14),Add(M16(SS,(BP - 0x14)),2));IP=0x129;return true;} // ADD word ptr [BP + -0x14],0x2
case 0x31970129:{M16(DS,(0x9e76),Add(M16(DS,(0x9e76)),1));IP=0x12d;return true;} // INC word ptr [0x9e76]
case 0x3197012d:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0x130;return true;} // MOV AX,[0x9e72]
case 0x31970130:{M16(DS,(0x9e6a),Sub(M16(DS,(0x9e6a)),AX));IP=0x134;return true;} // SUB word ptr [0x9e6a],AX
case 0x31970134:{M16(DS,(0x9ee8),Sub(M16(DS,(0x9ee8)),1));IP=0x138;return true;} // DEC word ptr [0x9ee8]
case 0x31970138:{int a=(BP - 0x10);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x13b;return true;} // LES BX,[BP + -0x10]
case 0x3197013b:{M16(ES,(BX),AX);IP=0x13e;return true;} // MOV word ptr ES:[BX],AX
case 0x3197013e:{Sub(M16(DS,(0x9e76)),200);IP=0x144;return true;} // CMP word ptr [0x9e76],0xc8
case 0x31970144:{IP=(ushort)((SF!=OF)?0x157:0x146);return true;} // JL 0xe000:0157
case 0x31970146:{AX=unchecked((ushort)(2302));IP=0x149;return true;} // MOV AX,0x8fe
case 0x31970149:{Push(DS);IP=0x14a;return true;} // PUSH DS
case 0x3197014a:{Push(AX);IP=0x14b;return true;} // PUSH AX
case 0x3197014b:{AX=unchecked((ushort)(33));IP=0x14e;return true;} // MOV AX,0x21
case 0x3197014e:{Push(AX);IP=0x14f;return true;} // PUSH AX
case 0x3197014f:{External(0x29bb,0x306b);IP=0x154;return true;} // CALLF 0x2000:cc1b
case 0x31970154:{SP=unchecked((ushort)(Add(SP,6)));IP=0x157;return true;} // ADD SP,0x6
case 0x31970157:{int a=(BP - 0x14);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x15a;return true;} // LES BX,[BP + -0x14]
case 0x3197015a:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x15d;return true;} // MOV AX,[0x9e6e]
case 0x3197015d:{M16(ES,(BX),AX);IP=0x160;return true;} // MOV word ptr ES:[BX],AX
case 0x31970160:{Sub(M16(DS,(0x9ee8)),0);IP=0x165;return true;} // CMP word ptr [0x9ee8],0x0
case 0x31970165:{IP=(ushort)((ZF||SF!=OF)?0x16a:0x167);return true;} // JLE 0xe000:016a
case 0x31970167:{IP=(ushort)((true)?0xa5:0x16a);return true;} // JMP 0xe000:00a5
case 0x3197016a:{M16(DS,(0x43f2),0);IP=0x170;return true;} // MOV word ptr [0x43f2],0x0
case 0x31970170:{M16(DS,(0x43f0),10000);IP=0x176;return true;} // MOV word ptr [0x43f0],0x2710
case 0x31970176:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x179;return true;} // MOV AX,[0x43f4]
case 0x31970179:{M16(SS,(BP - 0x16),AX);IP=0x17c;return true;} // MOV word ptr [BP + -0x16],AX
case 0x3197017c:{M16(DS,(0x9e76),1);IP=0x182;return true;} // MOV word ptr [0x9e76],0x1
case 0x31970182:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0x185;return true;} // MOV AX,[0x9e76]
case 0x31970185:{Sub(M16(SS,(BP - 0x16)),AX);IP=0x188;return true;} // CMP word ptr [BP + -0x16],AX
case 0x31970188:{IP=(ushort)((SF!=OF)?0x1bf:0x18a);return true;} // JL 0xe000:01bf
case 0x3197018a:{BX=unchecked((ushort)(AX));IP=0x18c;return true;} // MOV BX,AX
case 0x3197018c:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x18e;return true;} // SHL BX,0x1
case 0x3197018e:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x191;return true;} // LES SI,[BP + 0xe]
case 0x31970191:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x195;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970195:{M16(DS,(0x9e72),AX);IP=0x198;return true;} // MOV [0x9e72],AX
case 0x31970198:{Sub(M16(DS,(0x43f2)),AX);IP=0x19c;return true;} // CMP word ptr [0x43f2],AX
case 0x3197019c:{IP=(ushort)((SF==OF)?0x1a7:0x19e);return true;} // JGE 0xe000:01a7
case 0x3197019e:{M16(DS,(0x43f2),AX);IP=0x1a1;return true;} // MOV [0x43f2],AX
case 0x319701a1:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0x1a4;return true;} // MOV AX,[0x9e76]
case 0x319701a4:{M16(DS,(0x9eec),AX);IP=0x1a7;return true;} // MOV [0x9eec],AX
case 0x319701a7:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0x1aa;return true;} // MOV AX,[0x9e72]
case 0x319701aa:{Sub(M16(DS,(0x43f0)),AX);IP=0x1ae;return true;} // CMP word ptr [0x43f0],AX
case 0x319701ae:{IP=(ushort)((ZF||SF!=OF)?0x1b9:0x1b0);return true;} // JLE 0xe000:01b9
case 0x319701b0:{M16(DS,(0x43f0),AX);IP=0x1b3;return true;} // MOV [0x43f0],AX
case 0x319701b3:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0x1b6;return true;} // MOV AX,[0x9e76]
case 0x319701b6:{M16(DS,(0x9ed8),AX);IP=0x1b9;return true;} // MOV [0x9ed8],AX
case 0x319701b9:{M16(DS,(0x9e76),Add(M16(DS,(0x9e76)),1));IP=0x1bd;return true;} // INC word ptr [0x9e76]
case 0x319701bd:{IP=(ushort)((!OF)?0x182:0x1bf);return true;} // JNO 0xe000:0182
case 0x319701bf:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x1c2;return true;} // MOV AX,[0x43f2]
case 0x319701c2:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x43f0)))));IP=0x1c6;return true;} // SUB AX,word ptr [0x43f0]
case 0x319701c6:{M16(DS,(0x9e6e),AX);IP=0x1c9;return true;} // MOV [0x9e6e],AX
case 0x319701c9:{AX=unchecked((ushort)(M16(DS,(0x9efe))));IP=0x1cc;return true;} // MOV AX,[0x9efe]
case 0x319701cc:{Sub(M16(DS,(0x43f2)),AX);IP=0x1d0;return true;} // CMP word ptr [0x43f2],AX
case 0x319701d0:{IP=(ushort)((SF!=OF)?0x1de:0x1d2);return true;} // JL 0xe000:01de
case 0x319701d2:{AX=unchecked((ushort)(M16(DS,(0x9ee2))));IP=0x1d5;return true;} // MOV AX,[0x9ee2]
case 0x319701d5:{Sub(M16(DS,(0x9e6e)),AX);IP=0x1d9;return true;} // CMP word ptr [0x9e6e],AX
case 0x319701d9:{IP=(ushort)((SF!=OF)?0x1de:0x1db);return true;} // JL 0xe000:01de
case 0x319701db:{IP=(ushort)((true)?0x2f0:0x1de);return true;} // JMP 0xe000:02f0
case 0x319701de:{AX=unchecked((ushort)(M16(DS,(0x9efa))));IP=0x1e1;return true;} // MOV AX,[0x9efa]
case 0x319701e1:{Sub(M16(DS,(0x43f0)),AX);IP=0x1e5;return true;} // CMP word ptr [0x43f0],AX
case 0x319701e5:{IP=(ushort)((ZF||SF!=OF)?0x1ed:0x1e7);return true;} // JLE 0xe000:01ed
case 0x319701e7:{AX=unchecked((ushort)(M16(DS,(0x43f0))));IP=0x1ea;return true;} // MOV AX,[0x43f0]
case 0x319701ea:{M16(DS,(0x9efa),AX);IP=0x1ed;return true;} // MOV [0x9efa],AX
case 0x319701ed:{AX=unchecked((ushort)(M16(DS,(0x9efe))));IP=0x1f0;return true;} // MOV AX,[0x9efe]
case 0x319701f0:{Sub(M16(DS,(0x43f2)),AX);IP=0x1f4;return true;} // CMP word ptr [0x43f2],AX
case 0x319701f4:{IP=(ushort)((SF==OF)?0x1fc:0x1f6);return true;} // JGE 0xe000:01fc
case 0x319701f6:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x1f9;return true;} // MOV AX,[0x43f2]
case 0x319701f9:{M16(DS,(0x9efe),AX);IP=0x1fc;return true;} // MOV [0x9efe],AX
case 0x319701fc:{AX=unchecked((ushort)(M16(DS,(0x9ee2))));IP=0x1ff;return true;} // MOV AX,[0x9ee2]
case 0x319701ff:{Sub(M16(DS,(0x9e6e)),AX);IP=0x203;return true;} // CMP word ptr [0x9e6e],AX
case 0x31970203:{IP=(ushort)((SF==OF)?0x20b:0x205);return true;} // JGE 0xe000:020b
case 0x31970205:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x208;return true;} // MOV AX,[0x9e6e]
case 0x31970208:{M16(DS,(0x9ee2),AX);IP=0x20b;return true;} // MOV [0x9ee2],AX
case 0x3197020b:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x20e;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x3197020e:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x211;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x31970211:{AX=unchecked((ushort)(Add(AX,2)));IP=0x214;return true;} // ADD AX,0x2
case 0x31970214:{Push(DX);IP=0x215;return true;} // PUSH DX
case 0x31970215:{Push(AX);IP=0x216;return true;} // PUSH AX
case 0x31970216:{AX=unchecked((ushort)(M16(SS,(BP + 0x12))));IP=0x219;return true;} // MOV AX,word ptr [BP + 0x12]
case 0x31970219:{DX=unchecked((ushort)(M16(SS,(BP + 0x14))));IP=0x21c;return true;} // MOV DX,word ptr [BP + 0x14]
case 0x3197021c:{AX=unchecked((ushort)(Add(AX,2)));IP=0x21f;return true;} // ADD AX,0x2
case 0x3197021f:{Push(DX);IP=0x220;return true;} // PUSH DX
case 0x31970220:{Push(AX);IP=0x221;return true;} // PUSH AX
case 0x31970221:{AX=unchecked((ushort)(17396));IP=0x224;return true;} // MOV AX,0x43f4
case 0x31970224:{Push(DS);IP=0x225;return true;} // PUSH DS
case 0x31970225:{Push(AX);IP=0x226;return true;} // PUSH AX
case 0x31970226:{Push(CS);Push(0x22b);CS=0xfe0;IP=0xe;return true;} // CALLF 0x0000:fe0e
case 0x3197022b:{AX=unchecked((ushort)(M16(DS,(0x9ed8))));IP=0x22e;return true;} // MOV AX,[0x9ed8]
case 0x3197022e:{Sub(M16(DS,(0x9eec)),AX);IP=0x232;return true;} // CMP word ptr [0x9eec],AX
case 0x31970232:{IP=(ushort)((SF==OF)?0x237:0x234);return true;} // JGE 0xe000:0237
case 0x31970234:{IP=(ushort)((true)?0x2db:0x237);return true;} // JMP 0xe000:02db
case 0x31970237:{Sub(M16(DS,(0x9eec)),1);IP=0x23c;return true;} // CMP word ptr [0x9eec],0x1
case 0x3197023c:{IP=(ushort)((!ZF&&SF==OF)?0x241:0x23e);return true;} // JG 0xe000:0241
case 0x3197023e:{IP=(ushort)((true)?0x16a:0x241);return true;} // JMP 0xe000:016a
case 0x31970241:{M16(DS,(0x9ec8),0);IP=0x247;return true;} // MOV word ptr [0x9ec8],0x0
case 0x31970247:{M16(DS,(0x9ed0),65535);IP=0x24d;return true;} // MOV word ptr [0x9ed0],0xffff
case 0x3197024d:{M16(DS,(0x9eca),1);IP=0x253;return true;} // MOV word ptr [0x9eca],0x1
case 0x31970253:{SI=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x257;return true;} // MOV SI,word ptr [0x9eec]
case 0x31970257:{SI=unchecked((ushort)(Add(SI,M16(DS,(0x9ec8)))));IP=0x25b;return true;} // ADD SI,word ptr [0x9ec8]
case 0x3197025b:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x25d;return true;} // SHL SI,0x1
case 0x3197025d:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x260;return true;} // LES BX,[BP + 0x16]
case 0x31970260:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x264;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970264:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9ec8)))));IP=0x268;return true;} // SUB AX,word ptr [0x9ec8]
case 0x31970268:{M16(DS,(0x9e72),AX);IP=0x26b;return true;} // MOV [0x9e72],AX
case 0x3197026b:{BX=unchecked((ushort)(AX));IP=0x26d;return true;} // MOV BX,AX
case 0x3197026d:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x26f;return true;} // SHL BX,0x1
case 0x3197026f:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x272;return true;} // LES SI,[BP + 0x1e]
case 0x31970272:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x276;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970276:{M16(DS,(0x9ed6),AX);IP=0x279;return true;} // MOV [0x9ed6],AX
case 0x31970279:{BX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x27d;return true;} // MOV BX,word ptr [0x9eec]
case 0x3197027d:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x27f;return true;} // SHL BX,0x1
case 0x3197027f:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x282;return true;} // LES SI,[BP + 0xe]
case 0x31970282:{M16(ES,(BX + SI - 0x2),Sub(M16(ES,(BX + SI - 0x2)),AX));IP=0x286;return true;} // SUB word ptr ES:[BX + SI + -0x2],AX
case 0x31970286:{AX=unchecked((ushort)(M16(DS,(0x9ed0))));IP=0x289;return true;} // MOV AX,[0x9ed0]
case 0x31970289:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9eec)))));IP=0x28d;return true;} // ADD AX,word ptr [0x9eec]
case 0x3197028d:{M16(DS,(0x9ef8),AX);IP=0x290;return true;} // MOV [0x9ef8],AX
case 0x31970290:{BX=unchecked((ushort)(AX));IP=0x292;return true;} // MOV BX,AX
case 0x31970292:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x294;return true;} // SHL BX,0x1
case 0x31970294:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0x297;return true;} // MOV AX,[0x9ed6]
case 0x31970297:{M16(ES,(BX + SI - 0x2),Add(M16(ES,(BX + SI - 0x2)),AX));IP=0x29b;return true;} // ADD word ptr ES:[BX + SI + -0x2],AX
case 0x3197029b:{SI=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x29f;return true;} // MOV SI,word ptr [0x9eec]
case 0x3197029f:{SI=unchecked((ushort)(Add(SI,M16(DS,(0x9ec8)))));IP=0x2a3;return true;} // ADD SI,word ptr [0x9ec8]
case 0x319702a3:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x2a5;return true;} // SHL SI,0x1
case 0x319702a5:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x2a8;return true;} // LES BX,[BP + 0x16]
case 0x319702a8:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0x2ab;return true;} // MOV AX,[0x9e72]
case 0x319702ab:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9eca)))));IP=0x2af;return true;} // ADD AX,word ptr [0x9eca]
case 0x319702af:{M16(ES,(BX + SI - 0x2),AX);IP=0x2b3;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x319702b3:{AX=unchecked((ushort)(M16(DS,(0x9ef8))));IP=0x2b6;return true;} // MOV AX,[0x9ef8]
case 0x319702b6:{M16(DS,(0x9eec),AX);IP=0x2b9;return true;} // MOV [0x9eec],AX
case 0x319702b9:{Sub(M16(DS,(0x9ec8)),0);IP=0x2be;return true;} // CMP word ptr [0x9ec8],0x0
case 0x319702be:{IP=(ushort)((!ZF)?0x2cc:0x2c0);return true;} // JNZ 0xe000:02cc
case 0x319702c0:{AX=unchecked((ushort)(M16(DS,(0x9ed8))));IP=0x2c3;return true;} // MOV AX,[0x9ed8]
case 0x319702c3:{Sub(M16(DS,(0x9eec)),AX);IP=0x2c7;return true;} // CMP word ptr [0x9eec],AX
case 0x319702c7:{IP=(ushort)((!ZF&&SF==OF)?0x253:0x2c9);return true;} // JG 0xe000:0253
case 0x319702c9:{IP=(ushort)((true)?0x16a:0x2cc);return true;} // JMP 0xe000:016a
case 0x319702cc:{AX=unchecked((ushort)(M16(DS,(0x9ed8))));IP=0x2cf;return true;} // MOV AX,[0x9ed8]
case 0x319702cf:{Sub(M16(DS,(0x9eec)),AX);IP=0x2d3;return true;} // CMP word ptr [0x9eec],AX
case 0x319702d3:{IP=(ushort)((SF==OF)?0x2d8:0x2d5);return true;} // JGE 0xe000:02d8
case 0x319702d5:{IP=(ushort)((true)?0x253:0x2d8);return true;} // JMP 0xe000:0253
case 0x319702d8:{IP=(ushort)((true)?0x16a:0x2db);return true;} // JMP 0xe000:016a
case 0x319702db:{M16(DS,(0x9ec8),1);IP=0x2e1;return true;} // MOV word ptr [0x9ec8],0x1
case 0x319702e1:{M16(DS,(0x9ed0),1);IP=0x2e7;return true;} // MOV word ptr [0x9ed0],0x1
case 0x319702e7:{M16(DS,(0x9eca),0);IP=0x2ed;return true;} // MOV word ptr [0x9eca],0x0
case 0x319702ed:{IP=(ushort)((true)?0x253:0x2f0);return true;} // JMP 0xe000:0253
case 0x319702f0:{Push(M16(SS,(BP + 0x18)));IP=0x2f3;return true;} // PUSH word ptr [BP + 0x18]
case 0x319702f3:{Push(M16(SS,(BP + 0x16)));IP=0x2f6;return true;} // PUSH word ptr [BP + 0x16]
case 0x319702f6:{Push(M16(SS,(BP + 0x20)));IP=0x2f9;return true;} // PUSH word ptr [BP + 0x20]
case 0x319702f9:{Push(M16(SS,(BP + 0x1e)));IP=0x2fc;return true;} // PUSH word ptr [BP + 0x1e]
case 0x319702fc:{Push(M16(SS,(BP + 0x14)));IP=0x2ff;return true;} // PUSH word ptr [BP + 0x14]
case 0x319702ff:{Push(M16(SS,(BP + 0x12)));IP=0x302;return true;} // PUSH word ptr [BP + 0x12]
case 0x31970302:{Push(M16(SS,(BP + 0x10)));IP=0x305;return true;} // PUSH word ptr [BP + 0x10]
case 0x31970305:{Push(M16(SS,(BP + 0xe)));IP=0x308;return true;} // PUSH word ptr [BP + 0xe]
case 0x31970308:{Push(CS);Push(0x30d);CS=0x3197;IP=0x13f6;return true;} // CALLF 0x3000:2d66
case 0x3197030d:{M16(DS,(0x9ede),10000);IP=0x313;return true;} // MOV word ptr [0x9ede],0x2710
case 0x31970313:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x316;return true;} // MOV AX,[0x43f4]
case 0x31970316:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x317;return true;} // DEC AX
case 0x31970317:{M16(DS,(0x9ef2),AX);IP=0x31a;return true;} // MOV [0x9ef2],AX
case 0x3197031a:{AX=unchecked((ushort)(40670));IP=0x31d;return true;} // MOV AX,0x9ede
case 0x3197031d:{Push(DS);IP=0x31e;return true;} // PUSH DS
case 0x3197031e:{Push(AX);IP=0x31f;return true;} // PUSH AX
case 0x3197031f:{Push(M16(SS,(BP + 0x10)));IP=0x322;return true;} // PUSH word ptr [BP + 0x10]
case 0x31970322:{Push(M16(SS,(BP + 0xe)));IP=0x325;return true;} // PUSH word ptr [BP + 0xe]
case 0x31970325:{Push(CS);Push(0x32a);CS=0x3197;IP=0x14c6;return true;} // CALLF 0x3000:2e36
case 0x3197032a:{AX=unchecked((ushort)(M16(DS,(0x9ef2))));IP=0x32d;return true;} // MOV AX,[0x9ef2]
case 0x3197032d:{M16(SS,(BP - 0x14),AX);IP=0x330;return true;} // MOV word ptr [BP + -0x14],AX
case 0x31970330:{M16(DS,(0x9e6e),1);IP=0x336;return true;} // MOV word ptr [0x9e6e],0x1
case 0x31970336:{IP=(ushort)((true)?0x3e9:0x339);return true;} // JMP 0xe000:03e9
case 0x31970339:{AX=unchecked((ushort)(M16(DS,(0x9ef2))));IP=0x33c;return true;} // MOV AX,[0x9ef2]
case 0x3197033c:{Sub(M16(DS,(0x9e6e)),AX);IP=0x340;return true;} // CMP word ptr [0x9e6e],AX
case 0x31970340:{IP=(ushort)((!ZF)?0x345:0x342);return true;} // JNZ 0xe000:0345
case 0x31970342:{IP=(ushort)((true)?0x3e0:0x345);return true;} // JMP 0xe000:03e0
case 0x31970345:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0x348;return true;} // MOV AX,[0x9ecc]
case 0x31970348:{M16(DS,(0x9edc),AX);IP=0x34b;return true;} // MOV [0x9edc],AX
case 0x3197034b:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x34e;return true;} // MOV AX,[0x9e6e]
case 0x3197034e:{AX=unchecked((ushort)(Add(AX,1)));IP=0x34f;return true;} // INC AX
case 0x3197034f:{M16(DS,(0x9e72),AX);IP=0x352;return true;} // MOV [0x9e72],AX
case 0x31970352:{AX=unchecked((ushort)(M16(DS,(0x9ef2))));IP=0x355;return true;} // MOV AX,[0x9ef2]
case 0x31970355:{Sub(M16(DS,(0x9e72)),AX);IP=0x359;return true;} // CMP word ptr [0x9e72],AX
case 0x31970359:{IP=(ushort)((ZF||SF!=OF)?0x35e:0x35b);return true;} // JLE 0xe000:035e
case 0x3197035b:{IP=(ushort)((true)?0x3e0:0x35e);return true;} // JMP 0xe000:03e0
case 0x3197035e:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0x361;return true;} // MOV AX,[0x9e72]
case 0x31970361:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x363;return true;} // SHL AX,0x1
case 0x31970363:{M16(SS,(BP - 0xc),AX);IP=0x366;return true;} // MOV word ptr [BP + -0xc],AX
case 0x31970366:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x369;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x31970369:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x36c;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x3197036c:{AX=unchecked((ushort)(Add(AX,M16(SS,(BP - 0xc)))));IP=0x36f;return true;} // ADD AX,word ptr [BP + -0xc]
case 0x3197036f:{M16(SS,(BP - 0xa),AX);IP=0x372;return true;} // MOV word ptr [BP + -0xa],AX
case 0x31970372:{M16(SS,(BP - 0x8),DX);IP=0x375;return true;} // MOV word ptr [BP + -0x8],DX
case 0x31970375:{AX=unchecked((ushort)(M16(SS,(BP + 0xe))));IP=0x378;return true;} // MOV AX,word ptr [BP + 0xe]
case 0x31970378:{DX=unchecked((ushort)(M16(SS,(BP + 0x10))));IP=0x37b;return true;} // MOV DX,word ptr [BP + 0x10]
case 0x3197037b:{AX=unchecked((ushort)(Add(AX,M16(SS,(BP - 0xc)))));IP=0x37e;return true;} // ADD AX,word ptr [BP + -0xc]
case 0x3197037e:{M16(SS,(BP - 0x6),AX);IP=0x381;return true;} // MOV word ptr [BP + -0x6],AX
case 0x31970381:{M16(SS,(BP - 0x4),DX);IP=0x384;return true;} // MOV word ptr [BP + -0x4],DX
case 0x31970384:{int a=(BP - 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x387;return true;} // LES BX,[BP + -0xa]
case 0x31970387:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0x38a;return true;} // MOV AX,word ptr ES:[BX]
case 0x3197038a:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x38b;return true;} // DEC AX
case 0x3197038b:{M16(DS,(0x9ed2),AX);IP=0x38e;return true;} // MOV [0x9ed2],AX
case 0x3197038e:{AX=unchecked((ushort)(M16(DS,(0x9edc))));IP=0x391;return true;} // MOV AX,[0x9edc]
case 0x31970391:{BX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0x395;return true;} // MOV BX,word ptr [0x9ed2]
case 0x31970395:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x397;return true;} // SHL BX,0x1
case 0x31970397:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x39a;return true;} // LES SI,[BP + 0x1e]
case 0x3197039a:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX + SI - 0x2)))));IP=0x39e;return true;} // SUB AX,word ptr ES:[BX + SI + -0x2]
case 0x3197039e:{M16(DS,(0x9ed6),AX);IP=0x3a1;return true;} // MOV [0x9ed6],AX
case 0x319703a1:{Sub(M16(DS,(0x43f0)),AX);IP=0x3a5;return true;} // CMP word ptr [0x43f0],AX
case 0x319703a5:{IP=(ushort)((!ZF&&SF==OF)?0x3e0:0x3a7);return true;} // JG 0xe000:03e0
case 0x319703a7:{Sub(M16(DS,(0x43f2)),AX);IP=0x3ab;return true;} // CMP word ptr [0x43f2],AX
case 0x319703ab:{IP=(ushort)((SF!=OF)?0x3e0:0x3ad);return true;} // JL 0xe000:03e0
case 0x319703ad:{BX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0x3b1;return true;} // MOV BX,word ptr [0x9ed2]
case 0x319703b1:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x3b3;return true;} // SHL BX,0x1
case 0x319703b3:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x3b7;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x319703b7:{int a=(BP - 0x6);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x3ba;return true;} // LES BX,[BP + -0x6]
case 0x319703ba:{AX=unchecked((ushort)(Add(AX,M16(ES,(BX)))));IP=0x3bd;return true;} // ADD AX,word ptr ES:[BX]
case 0x319703bd:{M16(DS,(0x9edc),AX);IP=0x3c0;return true;} // MOV [0x9edc],AX
case 0x319703c0:{Sub(M16(DS,(0x43f2)),AX);IP=0x3c4;return true;} // CMP word ptr [0x43f2],AX
case 0x319703c4:{IP=(ushort)((SF!=OF)?0x3c9:0x3c6);return true;} // JL 0xe000:03c9
case 0x319703c6:{IP=(ushort)((true)?0x449:0x3c9);return true;} // JMP 0xe000:0449
case 0x319703c9:{M16(SS,(BP - 0xa),Add(M16(SS,(BP - 0xa)),2));IP=0x3cd;return true;} // ADD word ptr [BP + -0xa],0x2
case 0x319703cd:{M16(SS,(BP - 0x6),Add(M16(SS,(BP - 0x6)),2));IP=0x3d1;return true;} // ADD word ptr [BP + -0x6],0x2
case 0x319703d1:{M16(DS,(0x9e72),Add(M16(DS,(0x9e72)),1));IP=0x3d5;return true;} // INC word ptr [0x9e72]
case 0x319703d5:{IP=(ushort)((OF)?0x3e0:0x3d7);return true;} // JO 0xe000:03e0
case 0x319703d7:{AX=unchecked((ushort)(M16(DS,(0x9ef2))));IP=0x3da;return true;} // MOV AX,[0x9ef2]
case 0x319703da:{Sub(M16(DS,(0x9e72)),AX);IP=0x3de;return true;} // CMP word ptr [0x9e72],AX
case 0x319703de:{IP=(ushort)((ZF||SF!=OF)?0x384:0x3e0);return true;} // JLE 0xe000:0384
case 0x319703e0:{M16(DS,(0x9e6e),Add(M16(DS,(0x9e6e)),1));IP=0x3e4;return true;} // INC word ptr [0x9e6e]
case 0x319703e4:{IP=(ushort)((!OF)?0x3e9:0x3e6);return true;} // JNO 0xe000:03e9
case 0x319703e6:{IP=(ushort)((true)?0x58b:0x3e9);return true;} // JMP 0xe000:058b
case 0x319703e9:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x3ec;return true;} // MOV AX,[0x9e6e]
case 0x319703ec:{Sub(M16(SS,(BP - 0x14)),AX);IP=0x3ef;return true;} // CMP word ptr [BP + -0x14],AX
case 0x319703ef:{IP=(ushort)((SF==OF)?0x3f4:0x3f1);return true;} // JGE 0xe000:03f4
case 0x319703f1:{IP=(ushort)((true)?0x58b:0x3f4);return true;} // JMP 0xe000:058b
case 0x319703f4:{SI=unchecked((ushort)(AX));IP=0x3f6;return true;} // MOV SI,AX
case 0x319703f6:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x3f8;return true;} // SHL SI,0x1
case 0x319703f8:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x3fb;return true;} // LES BX,[BP + 0xe]
case 0x319703fb:{AX=unchecked((ushort)(M16(DS,(0x9ee6))));IP=0x3fe;return true;} // MOV AX,[0x9ee6]
case 0x319703fe:{Sub(M16(ES,(BX + SI - 0x2)),AX);IP=0x402;return true;} // CMP word ptr ES:[BX + SI + -0x2],AX
case 0x31970402:{IP=(ushort)((ZF||SF!=OF)?0x3e0:0x404);return true;} // JLE 0xe000:03e0
case 0x31970404:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x407;return true;} // LES BX,[BP + 0x16]
case 0x31970407:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x40a;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x3197040a:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x40b;return true;} // DEC AX
case 0x3197040b:{M16(DS,(0x9e6c),AX);IP=0x40e;return true;} // MOV [0x9e6c],AX
case 0x3197040e:{Sub(M16(ES,(BX + SI - 0x2)),AX);IP=0x412;return true;} // CMP word ptr ES:[BX + SI + -0x2],AX
case 0x31970412:{IP=(ushort)((ZF)?0x3e0:0x414);return true;} // JZ 0xe000:03e0
case 0x31970414:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x416;return true;} // SHL AX,0x1
case 0x31970416:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x419;return true;} // LES BX,[BP + 0x1e]
case 0x31970419:{BX=unchecked((ushort)(Add(BX,AX)));IP=0x41b;return true;} // ADD BX,AX
case 0x3197041b:{DI=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x41f;return true;} // MOV DI,word ptr ES:[BX + -0x2]
case 0x3197041f:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x422;return true;} // LES BX,[BP + 0xe]
case 0x31970422:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x426;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970426:{AX=unchecked((ushort)(Sub(AX,DI)));IP=0x428;return true;} // SUB AX,DI
case 0x31970428:{M16(DS,(0x9ec8),AX);IP=0x42b;return true;} // MOV [0x9ec8],AX
case 0x3197042b:{Sub(M16(DS,(0x43f0)),AX);IP=0x42f;return true;} // CMP word ptr [0x43f0],AX
case 0x3197042f:{IP=(ushort)((!ZF&&SF==OF)?0x3e0:0x431);return true;} // JG 0xe000:03e0
case 0x31970431:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x434;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x31970434:{AX=unchecked((ushort)(Add(AX,DI)));IP=0x436;return true;} // ADD AX,DI
case 0x31970436:{M16(DS,(0x9ecc),AX);IP=0x439;return true;} // MOV [0x9ecc],AX
case 0x31970439:{Sub(M16(DS,(0x43f2)),AX);IP=0x43d;return true;} // CMP word ptr [0x43f2],AX
case 0x3197043d:{IP=(ushort)((SF==OF)?0x442:0x43f);return true;} // JGE 0xe000:0442
case 0x3197043f:{IP=(ushort)((true)?0x339:0x442);return true;} // JMP 0xe000:0339
case 0x31970442:{Push(ES);IP=0x443;return true;} // PUSH ES
case 0x31970443:{Push(BX);IP=0x444;return true;} // PUSH BX
case 0x31970444:{Push(CS);Push(0x449);CS=0xc6a;IP=0x3d2;return true;} // CALLF 0x0000:ca72
case 0x31970449:{SI=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x44d;return true;} // MOV SI,word ptr [0x9e6e]
case 0x3197044d:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x44f;return true;} // SHL SI,0x1
case 0x3197044f:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x452;return true;} // LES BX,[BP + 0xe]
case 0x31970452:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0x455;return true;} // MOV AX,[0x9ec8]
case 0x31970455:{M16(ES,(BX + SI - 0x2),AX);IP=0x459;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970459:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0x45c;return true;} // MOV AX,[0x9ecc]
case 0x3197045c:{M16(ES,(BX + SI),AX);IP=0x45f;return true;} // MOV word ptr ES:[BX + SI],AX
case 0x3197045f:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x462;return true;} // LES BX,[BP + 0x16]
case 0x31970462:{M16(ES,(BX + SI),Sub(M16(ES,(BX + SI)),1));IP=0x465;return true;} // DEC word ptr ES:[BX + SI]
case 0x31970465:{IP=(ushort)((true)?0x31a:0x468);return true;} // JMP 0xe000:031a
case 0x31970468:{Sub(M16(DS,(0x9e6e)),2);IP=0x46d;return true;} // CMP word ptr [0x9e6e],0x2
case 0x3197046d:{IP=(ushort)((!ZF)?0x472:0x46f);return true;} // JNZ 0xe000:0472
case 0x3197046f:{IP=(ushort)((true)?0x502:0x472);return true;} // JMP 0xe000:0502
case 0x31970472:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0x475;return true;} // MOV AX,[0x9ecc]
case 0x31970475:{M16(DS,(0x9edc),AX);IP=0x478;return true;} // MOV [0x9edc],AX
case 0x31970478:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x47b;return true;} // MOV AX,[0x9e6e]
case 0x3197047b:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x47c;return true;} // DEC AX
case 0x3197047c:{M16(DS,(0x9e72),AX);IP=0x47f;return true;} // MOV [0x9e72],AX
case 0x3197047f:{Sub(AX,2);IP=0x482;return true;} // CMP AX,0x2
case 0x31970482:{IP=(ushort)((SF!=OF)?0x502:0x484);return true;} // JL 0xe000:0502
case 0x31970484:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x486;return true;} // SHL AX,0x1
case 0x31970486:{M16(SS,(BP - 0xc),AX);IP=0x489;return true;} // MOV word ptr [BP + -0xc],AX
case 0x31970489:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x48c;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x3197048c:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x48f;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x3197048f:{AX=unchecked((ushort)(Add(AX,M16(SS,(BP - 0xc)))));IP=0x492;return true;} // ADD AX,word ptr [BP + -0xc]
case 0x31970492:{AX=unchecked((ushort)(Sub(AX,2)));IP=0x495;return true;} // SUB AX,0x2
case 0x31970495:{M16(SS,(BP - 0xa),AX);IP=0x498;return true;} // MOV word ptr [BP + -0xa],AX
case 0x31970498:{M16(SS,(BP - 0x8),DX);IP=0x49b;return true;} // MOV word ptr [BP + -0x8],DX
case 0x3197049b:{AX=unchecked((ushort)(M16(SS,(BP + 0xe))));IP=0x49e;return true;} // MOV AX,word ptr [BP + 0xe]
case 0x3197049e:{DX=unchecked((ushort)(M16(SS,(BP + 0x10))));IP=0x4a1;return true;} // MOV DX,word ptr [BP + 0x10]
case 0x319704a1:{AX=unchecked((ushort)(Add(AX,M16(SS,(BP - 0xc)))));IP=0x4a4;return true;} // ADD AX,word ptr [BP + -0xc]
case 0x319704a4:{AX=unchecked((ushort)(Sub(AX,4)));IP=0x4a7;return true;} // SUB AX,0x4
case 0x319704a7:{M16(SS,(BP - 0x6),AX);IP=0x4aa;return true;} // MOV word ptr [BP + -0x6],AX
case 0x319704aa:{M16(SS,(BP - 0x4),DX);IP=0x4ad;return true;} // MOV word ptr [BP + -0x4],DX
case 0x319704ad:{int a=(BP - 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x4b0;return true;} // LES BX,[BP + -0xa]
case 0x319704b0:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0x4b3;return true;} // MOV AX,word ptr ES:[BX]
case 0x319704b3:{M16(DS,(0x9ed2),AX);IP=0x4b6;return true;} // MOV [0x9ed2],AX
case 0x319704b6:{AX=unchecked((ushort)(M16(DS,(0x9edc))));IP=0x4b9;return true;} // MOV AX,[0x9edc]
case 0x319704b9:{BX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0x4bd;return true;} // MOV BX,word ptr [0x9ed2]
case 0x319704bd:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x4bf;return true;} // SHL BX,0x1
case 0x319704bf:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x4c2;return true;} // LES SI,[BP + 0x1e]
case 0x319704c2:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX + SI - 0x2)))));IP=0x4c6;return true;} // SUB AX,word ptr ES:[BX + SI + -0x2]
case 0x319704c6:{M16(DS,(0x9ed6),AX);IP=0x4c9;return true;} // MOV [0x9ed6],AX
case 0x319704c9:{Sub(M16(DS,(0x43f0)),AX);IP=0x4cd;return true;} // CMP word ptr [0x43f0],AX
case 0x319704cd:{IP=(ushort)((!ZF&&SF==OF)?0x502:0x4cf);return true;} // JG 0xe000:0502
case 0x319704cf:{Sub(M16(DS,(0x43f2)),AX);IP=0x4d3;return true;} // CMP word ptr [0x43f2],AX
case 0x319704d3:{IP=(ushort)((SF!=OF)?0x502:0x4d5);return true;} // JL 0xe000:0502
case 0x319704d5:{BX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0x4d9;return true;} // MOV BX,word ptr [0x9ed2]
case 0x319704d9:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x4db;return true;} // SHL BX,0x1
case 0x319704db:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x4df;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x319704df:{int a=(BP - 0x6);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x4e2;return true;} // LES BX,[BP + -0x6]
case 0x319704e2:{AX=unchecked((ushort)(Add(AX,M16(ES,(BX)))));IP=0x4e5;return true;} // ADD AX,word ptr ES:[BX]
case 0x319704e5:{M16(DS,(0x9edc),AX);IP=0x4e8;return true;} // MOV [0x9edc],AX
case 0x319704e8:{Sub(M16(DS,(0x43f2)),AX);IP=0x4ec;return true;} // CMP word ptr [0x43f2],AX
case 0x319704ec:{IP=(ushort)((SF==OF)?0x56d:0x4ee);return true;} // JGE 0xe000:056d
case 0x319704ee:{M16(SS,(BP - 0xa),Add(M16(SS,(BP - 0xa)),-2));IP=0x4f2;return true;} // ADD word ptr [BP + -0xa],-0x2
case 0x319704f2:{M16(SS,(BP - 0x6),Add(M16(SS,(BP - 0x6)),-2));IP=0x4f6;return true;} // ADD word ptr [BP + -0x6],-0x2
case 0x319704f6:{M16(DS,(0x9e72),Add(M16(DS,(0x9e72)),-1));IP=0x4fb;return true;} // ADD word ptr [0x9e72],-0x1
case 0x319704fb:{Sub(M16(DS,(0x9e72)),2);IP=0x500;return true;} // CMP word ptr [0x9e72],0x2
case 0x31970500:{IP=(ushort)((SF==OF)?0x4ad:0x502);return true;} // JGE 0xe000:04ad
case 0x31970502:{M16(DS,(0x9e6e),Add(M16(DS,(0x9e6e)),-1));IP=0x507;return true;} // ADD word ptr [0x9e6e],-0x1
case 0x31970507:{Sub(M16(DS,(0x9e6e)),2);IP=0x50c;return true;} // CMP word ptr [0x9e6e],0x2
case 0x3197050c:{IP=(ushort)((SF==OF)?0x511:0x50e);return true;} // JGE 0xe000:0511
case 0x3197050e:{IP=(ushort)((true)?0x5a4:0x511);return true;} // JMP 0xe000:05a4
case 0x31970511:{SI=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x515;return true;} // MOV SI,word ptr [0x9e6e]
case 0x31970515:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x517;return true;} // SHL SI,0x1
case 0x31970517:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x51a;return true;} // LES BX,[BP + 0xe]
case 0x3197051a:{AX=unchecked((ushort)(M16(DS,(0x9ee6))));IP=0x51d;return true;} // MOV AX,[0x9ee6]
case 0x3197051d:{Sub(M16(ES,(BX + SI - 0x2)),AX);IP=0x521;return true;} // CMP word ptr ES:[BX + SI + -0x2],AX
case 0x31970521:{IP=(ushort)((ZF||SF!=OF)?0x502:0x523);return true;} // JLE 0xe000:0502
case 0x31970523:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x526;return true;} // LES BX,[BP + 0x16]
case 0x31970526:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x52a;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x3197052a:{M16(DS,(0x9e6c),AX);IP=0x52d;return true;} // MOV [0x9e6c],AX
case 0x3197052d:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x52e;return true;} // DEC AX
case 0x3197052e:{Sub(M16(ES,(BX + SI - 0x4)),AX);IP=0x532;return true;} // CMP word ptr ES:[BX + SI + -0x4],AX
case 0x31970532:{IP=(ushort)((ZF)?0x502:0x534);return true;} // JZ 0xe000:0502
case 0x31970534:{AX=unchecked((ushort)(M16(DS,(0x9e6c))));IP=0x537;return true;} // MOV AX,[0x9e6c]
case 0x31970537:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x539;return true;} // SHL AX,0x1
case 0x31970539:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x53c;return true;} // LES BX,[BP + 0x1e]
case 0x3197053c:{BX=unchecked((ushort)(Add(BX,AX)));IP=0x53e;return true;} // ADD BX,AX
case 0x3197053e:{DI=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x542;return true;} // MOV DI,word ptr ES:[BX + -0x2]
case 0x31970542:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x545;return true;} // LES BX,[BP + 0xe]
case 0x31970545:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x549;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970549:{AX=unchecked((ushort)(Sub(AX,DI)));IP=0x54b;return true;} // SUB AX,DI
case 0x3197054b:{M16(DS,(0x9ec8),AX);IP=0x54e;return true;} // MOV [0x9ec8],AX
case 0x3197054e:{Sub(M16(DS,(0x43f0)),AX);IP=0x552;return true;} // CMP word ptr [0x43f0],AX
case 0x31970552:{IP=(ushort)((!ZF&&SF==OF)?0x502:0x554);return true;} // JG 0xe000:0502
case 0x31970554:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x4))));IP=0x558;return true;} // MOV AX,word ptr ES:[BX + SI + -0x4]
case 0x31970558:{AX=unchecked((ushort)(Add(AX,DI)));IP=0x55a;return true;} // ADD AX,DI
case 0x3197055a:{M16(DS,(0x9ecc),AX);IP=0x55d;return true;} // MOV [0x9ecc],AX
case 0x3197055d:{Sub(M16(DS,(0x43f2)),AX);IP=0x561;return true;} // CMP word ptr [0x43f2],AX
case 0x31970561:{IP=(ushort)((SF==OF)?0x566:0x563);return true;} // JGE 0xe000:0566
case 0x31970563:{IP=(ushort)((true)?0x468:0x566);return true;} // JMP 0xe000:0468
case 0x31970566:{Push(ES);IP=0x567;return true;} // PUSH ES
case 0x31970567:{Push(BX);IP=0x568;return true;} // PUSH BX
case 0x31970568:{Push(CS);Push(0x56d);CS=0xc6a;IP=0x3d2;return true;} // CALLF 0x0000:ca72
case 0x3197056d:{SI=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x571;return true;} // MOV SI,word ptr [0x9e6e]
case 0x31970571:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x573;return true;} // SHL SI,0x1
case 0x31970573:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x576;return true;} // LES BX,[BP + 0xe]
case 0x31970576:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0x579;return true;} // MOV AX,[0x9ec8]
case 0x31970579:{M16(ES,(BX + SI - 0x2),AX);IP=0x57d;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x3197057d:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0x580;return true;} // MOV AX,[0x9ecc]
case 0x31970580:{M16(ES,(BX + SI - 0x4),AX);IP=0x584;return true;} // MOV word ptr ES:[BX + SI + -0x4],AX
case 0x31970584:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x587;return true;} // LES BX,[BP + 0x16]
case 0x31970587:{M16(ES,(BX + SI - 0x2),Add(M16(ES,(BX + SI - 0x2)),1));IP=0x58b;return true;} // INC word ptr ES:[BX + SI + -0x2]
case 0x3197058b:{AX=unchecked((ushort)(40670));IP=0x58e;return true;} // MOV AX,0x9ede
case 0x3197058e:{Push(DS);IP=0x58f;return true;} // PUSH DS
case 0x3197058f:{Push(AX);IP=0x590;return true;} // PUSH AX
case 0x31970590:{Push(M16(SS,(BP + 0x10)));IP=0x593;return true;} // PUSH word ptr [BP + 0x10]
case 0x31970593:{Push(M16(SS,(BP + 0xe)));IP=0x596;return true;} // PUSH word ptr [BP + 0xe]
case 0x31970596:{Push(CS);Push(0x59b);CS=0x3197;IP=0x14c6;return true;} // CALLF 0x3000:2e36
case 0x3197059b:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x59e;return true;} // MOV AX,[0x43f4]
case 0x3197059e:{M16(DS,(0x9e6e),AX);IP=0x5a1;return true;} // MOV [0x9e6e],AX
case 0x319705a1:{IP=(ushort)((true)?0x507:0x5a4);return true;} // JMP 0xe000:0507
case 0x319705a4:{Push(M16(SS,(BP + 0x10)));IP=0x5a7;return true;} // PUSH word ptr [BP + 0x10]
case 0x319705a7:{Push(M16(SS,(BP + 0xe)));IP=0x5aa;return true;} // PUSH word ptr [BP + 0xe]
case 0x319705aa:{Push(CS);Push(0x5af);CS=0xc6a;IP=0x3d2;return true;} // CALLF 0x0000:ca72
case 0x319705af:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x5b2;return true;} // MOV AX,[0x43f2]
case 0x319705b2:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x43f0)))));IP=0x5b6;return true;} // SUB AX,word ptr [0x43f0]
case 0x319705b6:{M16(DS,(0x9ede),AX);IP=0x5b9;return true;} // MOV [0x9ede],AX
case 0x319705b9:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x5bc;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x319705bc:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x5bf;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x319705bf:{AX=unchecked((ushort)(Add(AX,2)));IP=0x5c2;return true;} // ADD AX,0x2
case 0x319705c2:{Push(DX);IP=0x5c3;return true;} // PUSH DX
case 0x319705c3:{Push(AX);IP=0x5c4;return true;} // PUSH AX
case 0x319705c4:{AX=unchecked((ushort)(M16(SS,(BP + 0x12))));IP=0x5c7;return true;} // MOV AX,word ptr [BP + 0x12]
case 0x319705c7:{DX=unchecked((ushort)(M16(SS,(BP + 0x14))));IP=0x5ca;return true;} // MOV DX,word ptr [BP + 0x14]
case 0x319705ca:{AX=unchecked((ushort)(Add(AX,2)));IP=0x5cd;return true;} // ADD AX,0x2
case 0x319705cd:{Push(DX);IP=0x5ce;return true;} // PUSH DX
case 0x319705ce:{Push(AX);IP=0x5cf;return true;} // PUSH AX
case 0x319705cf:{AX=unchecked((ushort)(17396));IP=0x5d2;return true;} // MOV AX,0x43f4
case 0x319705d2:{Push(DS);IP=0x5d3;return true;} // PUSH DS
case 0x319705d3:{Push(AX);IP=0x5d4;return true;} // PUSH AX
case 0x319705d4:{Push(CS);Push(0x5d9);CS=0xfe0;IP=0xe;return true;} // CALLF 0x0000:fe0e
case 0x319705d9:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x5dc;return true;} // MOV AX,[0x43f2]
case 0x319705dc:{AX=unchecked((ushort)(Add(AX,5)));IP=0x5df;return true;} // ADD AX,0x5
case 0x319705df:{M16(DS,(0x9eec),AX);IP=0x5e2;return true;} // MOV [0x9eec],AX
case 0x319705e2:{M16(DS,(0x9eda),1);IP=0x5e8;return true;} // MOV word ptr [0x9eda],0x1
case 0x319705e8:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x5eb;return true;} // MOV AX,[0x43f2]
case 0x319705eb:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x43f0)))));IP=0x5ef;return true;} // SUB AX,word ptr [0x43f0]
case 0x319705ef:{M16(DS,(0x9e6c),AX);IP=0x5f2;return true;} // MOV [0x9e6c],AX
case 0x319705f2:{AX=unchecked((ushort)(M16(DS,(0x9ede))));IP=0x5f5;return true;} // MOV AX,[0x9ede]
case 0x319705f5:{Sub(M16(DS,(0x9e6c)),AX);IP=0x5f9;return true;} // CMP word ptr [0x9e6c],AX
case 0x319705f9:{IP=(ushort)((!ZF&&SF==OF)?0x621:0x5fb);return true;} // JG 0xe000:0621
case 0x319705fb:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x5fe;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x319705fe:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x601;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x31970601:{AX=unchecked((ushort)(Add(AX,2)));IP=0x604;return true;} // ADD AX,0x2
case 0x31970604:{Push(DX);IP=0x605;return true;} // PUSH DX
case 0x31970605:{Push(AX);IP=0x606;return true;} // PUSH AX
case 0x31970606:{AX=unchecked((ushort)(M16(SS,(BP + 0x12))));IP=0x609;return true;} // MOV AX,word ptr [BP + 0x12]
case 0x31970609:{DX=unchecked((ushort)(M16(SS,(BP + 0x14))));IP=0x60c;return true;} // MOV DX,word ptr [BP + 0x14]
case 0x3197060c:{AX=unchecked((ushort)(Add(AX,2)));IP=0x60f;return true;} // ADD AX,0x2
case 0x3197060f:{Push(DX);IP=0x610;return true;} // PUSH DX
case 0x31970610:{Push(AX);IP=0x611;return true;} // PUSH AX
case 0x31970611:{AX=unchecked((ushort)(17396));IP=0x614;return true;} // MOV AX,0x43f4
case 0x31970614:{Push(DS);IP=0x615;return true;} // PUSH DS
case 0x31970615:{Push(AX);IP=0x616;return true;} // PUSH AX
case 0x31970616:{Push(CS);Push(0x61b);CS=0xfe0;IP=0xe;return true;} // CALLF 0x0000:fe0e
case 0x3197061b:{AX=unchecked((ushort)(M16(DS,(0x9e6c))));IP=0x61e;return true;} // MOV AX,[0x9e6c]
case 0x3197061e:{M16(DS,(0x9ede),AX);IP=0x621;return true;} // MOV [0x9ede],AX
case 0x31970621:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x624;return true;} // MOV AX,[0x43f4]
case 0x31970624:{M16(SS,(BP - 0xc),AX);IP=0x627;return true;} // MOV word ptr [BP + -0xc],AX
case 0x31970627:{M16(DS,(0x9e6e),1);IP=0x62d;return true;} // MOV word ptr [0x9e6e],0x1
case 0x3197062d:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x630;return true;} // MOV AX,[0x9e6e]
case 0x31970630:{Sub(M16(SS,(BP - 0xc)),AX);IP=0x633;return true;} // CMP word ptr [BP + -0xc],AX
case 0x31970633:{IP=(ushort)((SF!=OF)?0x64b:0x635);return true;} // JL 0xe000:064b
case 0x31970635:{BX=unchecked((ushort)(AX));IP=0x637;return true;} // MOV BX,AX
case 0x31970637:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x639;return true;} // SHL BX,0x1
case 0x31970639:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x63c;return true;} // LES SI,[BP + 0xe]
case 0x3197063c:{AX=unchecked((ushort)(M16(DS,(0x43f0))));IP=0x63f;return true;} // MOV AX,[0x43f0]
case 0x3197063f:{Sub(M16(ES,(BX + SI - 0x2)),AX);IP=0x643;return true;} // CMP word ptr ES:[BX + SI + -0x2],AX
case 0x31970643:{IP=(ushort)((ZF)?0x64b:0x645);return true;} // JZ 0xe000:064b
case 0x31970645:{M16(DS,(0x9e6e),Add(M16(DS,(0x9e6e)),1));IP=0x649;return true;} // INC word ptr [0x9e6e]
case 0x31970649:{IP=(ushort)((!OF)?0x62d:0x64b);return true;} // JNO 0xe000:062d
case 0x3197064b:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x64e;return true;} // MOV AX,[0x9e6e]
case 0x3197064e:{Sub(M16(DS,(0x43f4)),AX);IP=0x652;return true;} // CMP word ptr [0x43f4],AX
case 0x31970652:{IP=(ushort)((!ZF)?0x657:0x654);return true;} // JNZ 0xe000:0657
case 0x31970654:{IP=(ushort)((true)?0x888:0x657);return true;} // JMP 0xe000:0888
case 0x31970657:{SI=unchecked((ushort)(AX));IP=0x659;return true;} // MOV SI,AX
case 0x31970659:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x65b;return true;} // SHL SI,0x1
case 0x3197065b:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x65e;return true;} // LES BX,[BP + 0x16]
case 0x3197065e:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x661;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x31970661:{M16(DS,(0x9e6c),AX);IP=0x664;return true;} // MOV [0x9e6c],AX
case 0x31970664:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x666;return true;} // SHL AX,0x1
case 0x31970666:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x669;return true;} // LES BX,[BP + 0x1e]
case 0x31970669:{BX=unchecked((ushort)(Add(BX,AX)));IP=0x66b;return true;} // ADD BX,AX
case 0x3197066b:{DI=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x66f;return true;} // MOV DI,word ptr ES:[BX + -0x2]
case 0x3197066f:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x672;return true;} // LES BX,[BP + 0xe]
case 0x31970672:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x676;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970676:{AX=unchecked((ushort)(Add(AX,DI)));IP=0x678;return true;} // ADD AX,DI
case 0x31970678:{M16(DS,(0x9ec8),AX);IP=0x67b;return true;} // MOV [0x9ec8],AX
case 0x3197067b:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x67e;return true;} // MOV AX,[0x9eec]
case 0x3197067e:{Sub(M16(DS,(0x9ec8)),AX);IP=0x682;return true;} // CMP word ptr [0x9ec8],AX
case 0x31970682:{IP=(ushort)((ZF||SF!=OF)?0x687:0x684);return true;} // JLE 0xe000:0687
case 0x31970684:{IP=(ushort)((true)?0x888:0x687);return true;} // JMP 0xe000:0888
case 0x31970687:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x68a;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x3197068a:{AX=unchecked((ushort)(Sub(AX,DI)));IP=0x68c;return true;} // SUB AX,DI
case 0x3197068c:{M16(DS,(0x9edc),AX);IP=0x68f;return true;} // MOV [0x9edc],AX
case 0x3197068f:{Sub(M16(DS,(0x43f0)),AX);IP=0x693;return true;} // CMP word ptr [0x43f0],AX
case 0x31970693:{IP=(ushort)((ZF||SF!=OF)?0x698:0x695);return true;} // JLE 0xe000:0698
case 0x31970695:{IP=(ushort)((true)?0x7bc:0x698);return true;} // JMP 0xe000:07bc
case 0x31970698:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0x69b;return true;} // MOV AX,[0x9ec8]
case 0x3197069b:{M16(ES,(BX + SI - 0x2),AX);IP=0x69f;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x3197069f:{AX=unchecked((ushort)(M16(DS,(0x9edc))));IP=0x6a2;return true;} // MOV AX,[0x9edc]
case 0x319706a2:{M16(ES,(BX + SI),AX);IP=0x6a5;return true;} // MOV word ptr ES:[BX + SI],AX
case 0x319706a5:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x6a8;return true;} // LES BX,[BP + 0x16]
case 0x319706a8:{AX=unchecked((ushort)(M16(DS,(0x9e6c))));IP=0x6ab;return true;} // MOV AX,[0x9e6c]
case 0x319706ab:{AX=unchecked((ushort)(Add(AX,1)));IP=0x6ac;return true;} // INC AX
case 0x319706ac:{M16(ES,(BX + SI),AX);IP=0x6af;return true;} // MOV word ptr ES:[BX + SI],AX
case 0x319706af:{Push(M16(SS,(BP + 0x10)));IP=0x6b2;return true;} // PUSH word ptr [BP + 0x10]
case 0x319706b2:{Push(M16(SS,(BP + 0xe)));IP=0x6b5;return true;} // PUSH word ptr [BP + 0xe]
case 0x319706b5:{Push(CS);Push(0x6ba);CS=0xc6a;IP=0x3d2;return true;} // CALLF 0x0000:ca72
case 0x319706ba:{IP=(ushort)((true)?0x5d9:0x6bd);return true;} // JMP 0xe000:05d9
case 0x319706bd:{BX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x6c1;return true;} // MOV BX,word ptr [0x9e6e]
case 0x319706c1:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x6c3;return true;} // SHL BX,0x1
case 0x319706c3:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x6c6;return true;} // LES SI,[BP + 0xe]
case 0x319706c6:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0x6c9;return true;} // MOV AX,[0x9ec8]
case 0x319706c9:{M16(ES,(BX + SI - 0x2),AX);IP=0x6cd;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x319706cd:{AX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0x6d0;return true;} // MOV AX,[0x9e7e]
case 0x319706d0:{M16(SS,(BP - 0xa),AX);IP=0x6d3;return true;} // MOV word ptr [BP + -0xa],AX
case 0x319706d3:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x6d6;return true;} // MOV AX,[0x9e6e]
case 0x319706d6:{AX=unchecked((ushort)(Add(AX,1)));IP=0x6d7;return true;} // INC AX
case 0x319706d7:{M16(DS,(0x9e70),AX);IP=0x6da;return true;} // MOV [0x9e70],AX
case 0x319706da:{IP=(ushort)((true)?0x746:0x6dc);return true;} // JMP 0xe000:0746
case 0x319706dc:{AX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x6df;return true;} // MOV AX,[0x9e70]
case 0x319706df:{AX=unchecked((ushort)(Add(AX,1)));IP=0x6e0;return true;} // INC AX
case 0x319706e0:{M16(DS,(0x9e78),AX);IP=0x6e3;return true;} // MOV [0x9e78],AX
case 0x319706e3:{SI=unchecked((ushort)(AX));IP=0x6e5;return true;} // MOV SI,AX
case 0x319706e5:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x6e7;return true;} // SHL SI,0x1
case 0x319706e7:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x6ea;return true;} // LES BX,[BP + 0x16]
case 0x319706ea:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x6ee;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x319706ee:{M16(DS,(0x9e80),AX);IP=0x6f1;return true;} // MOV [0x9e80],AX
case 0x319706f1:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x6f3;return true;} // SHL AX,0x1
case 0x319706f3:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x6f6;return true;} // LES BX,[BP + 0x1e]
case 0x319706f6:{BX=unchecked((ushort)(Add(BX,AX)));IP=0x6f8;return true;} // ADD BX,AX
case 0x319706f8:{DI=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x6fc;return true;} // MOV DI,word ptr ES:[BX + -0x2]
case 0x319706fc:{M16(DS,(0x9e74),Add(M16(DS,(0x9e74)),DI));IP=0x700;return true;} // ADD word ptr [0x9e74],DI
case 0x31970700:{AX=unchecked((ushort)(M16(DS,(0x9e74))));IP=0x703;return true;} // MOV AX,[0x9e74]
case 0x31970703:{Sub(M16(DS,(0x43f0)),AX);IP=0x707;return true;} // CMP word ptr [0x43f0],AX
case 0x31970707:{IP=(ushort)((SF!=OF)?0x730:0x709);return true;} // JL 0xe000:0730
case 0x31970709:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x70c;return true;} // LES BX,[BP + 0xe]
case 0x3197070c:{M16(ES,(BX + SI - 0x2),Sub(M16(ES,(BX + SI - 0x2)),DI));IP=0x710;return true;} // SUB word ptr ES:[BX + SI + -0x2],DI
case 0x31970710:{M16(DS,(0x9e80),Add(M16(DS,(0x9e80)),1));IP=0x714;return true;} // INC word ptr [0x9e80]
case 0x31970714:{AX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0x717;return true;} // MOV AX,[0x9e80]
case 0x31970717:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x719;return true;} // SHL AX,0x1
case 0x31970719:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x71c;return true;} // LES BX,[BP + 0x1e]
case 0x3197071c:{BX=unchecked((ushort)(Add(BX,AX)));IP=0x71e;return true;} // ADD BX,AX
case 0x3197071e:{AX=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x722;return true;} // MOV AX,word ptr ES:[BX + -0x2]
case 0x31970722:{M16(DS,(0x9e74),Add(M16(DS,(0x9e74)),AX));IP=0x726;return true;} // ADD word ptr [0x9e74],AX
case 0x31970726:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x729;return true;} // LES BX,[BP + 0x16]
case 0x31970729:{AX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0x72c;return true;} // MOV AX,[0x9e80]
case 0x3197072c:{M16(ES,(BX + SI - 0x2),AX);IP=0x730;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970730:{BX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x734;return true;} // MOV BX,word ptr [0x9e70]
case 0x31970734:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x736;return true;} // SHL BX,0x1
case 0x31970736:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x739;return true;} // LES SI,[BP + 0xe]
case 0x31970739:{AX=unchecked((ushort)(M16(DS,(0x9e74))));IP=0x73c;return true;} // MOV AX,[0x9e74]
case 0x3197073c:{M16(ES,(BX + SI - 0x2),AX);IP=0x740;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970740:{M16(DS,(0x9e70),Add(M16(DS,(0x9e70)),1));IP=0x744;return true;} // INC word ptr [0x9e70]
case 0x31970744:{IP=(ushort)((OF)?0x794:0x746);return true;} // JO 0xe000:0794
case 0x31970746:{AX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x749;return true;} // MOV AX,[0x9e70]
case 0x31970749:{Sub(M16(SS,(BP - 0xa)),AX);IP=0x74c;return true;} // CMP word ptr [BP + -0xa],AX
case 0x3197074c:{IP=(ushort)((SF!=OF)?0x794:0x74e);return true;} // JL 0xe000:0794
case 0x3197074e:{SI=unchecked((ushort)(AX));IP=0x750;return true;} // MOV SI,AX
case 0x31970750:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x752;return true;} // SHL SI,0x1
case 0x31970752:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x755;return true;} // LES BX,[BP + 0x16]
case 0x31970755:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x759;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970759:{M16(DS,(0x9e7c),AX);IP=0x75c;return true;} // MOV [0x9e7c],AX
case 0x3197075c:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x75f;return true;} // LES BX,[BP + 0xe]
case 0x3197075f:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x763;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970763:{BX=unchecked((ushort)(M16(DS,(0x9e7c))));IP=0x767;return true;} // MOV BX,word ptr [0x9e7c]
case 0x31970767:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x769;return true;} // SHL BX,0x1
case 0x31970769:{int a=(BP + 0x1e);ushort sg=SS;DI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x76c;return true;} // LES DI,[BP + 0x1e]
case 0x3197076c:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX + DI - 0x2)))));IP=0x770;return true;} // SUB AX,word ptr ES:[BX + DI + -0x2]
case 0x31970770:{M16(DS,(0x9e74),AX);IP=0x773;return true;} // MOV [0x9e74],AX
case 0x31970773:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x776;return true;} // LES BX,[BP + 0x16]
case 0x31970776:{AX=unchecked((ushort)(M16(DS,(0x9e7c))));IP=0x779;return true;} // MOV AX,[0x9e7c]
case 0x31970779:{AX=unchecked((ushort)(Add(AX,1)));IP=0x77a;return true;} // INC AX
case 0x3197077a:{M16(ES,(BX + SI - 0x2),AX);IP=0x77e;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x3197077e:{AX=unchecked((ushort)(M16(DS,(0x9e74))));IP=0x781;return true;} // MOV AX,[0x9e74]
case 0x31970781:{Sub(M16(DS,(0x43f0)),AX);IP=0x785;return true;} // CMP word ptr [0x43f0],AX
case 0x31970785:{IP=(ushort)((ZF||SF!=OF)?0x78a:0x787);return true;} // JLE 0xe000:078a
case 0x31970787:{IP=(ushort)((true)?0x6dc:0x78a);return true;} // JMP 0xe000:06dc
case 0x3197078a:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x78d;return true;} // LES BX,[BP + 0xe]
case 0x3197078d:{M16(ES,(BX + SI - 0x2),AX);IP=0x791;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970791:{IP=(ushort)((true)?0x6af:0x794);return true;} // JMP 0xe000:06af
case 0x31970794:{SI=unchecked((ushort)(M16(DS,(0x9e78))));IP=0x798;return true;} // MOV SI,word ptr [0x9e78]
case 0x31970798:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x79a;return true;} // SHL SI,0x1
case 0x3197079a:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x79d;return true;} // LES BX,[BP + 0x16]
case 0x3197079d:{AX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0x7a0;return true;} // MOV AX,[0x9e80]
case 0x319707a0:{AX=unchecked((ushort)(Add(AX,1)));IP=0x7a1;return true;} // INC AX
case 0x319707a1:{M16(ES,(BX + SI - 0x2),AX);IP=0x7a5;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x319707a5:{BX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0x7a9;return true;} // MOV BX,word ptr [0x9e80]
case 0x319707a9:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x7ab;return true;} // SHL BX,0x1
case 0x319707ab:{int a=(BP + 0x1e);ushort sg=SS;DI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x7ae;return true;} // LES DI,[BP + 0x1e]
case 0x319707ae:{AX=unchecked((ushort)(M16(ES,(BX + DI - 0x2))));IP=0x7b2;return true;} // MOV AX,word ptr ES:[BX + DI + -0x2]
case 0x319707b2:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x7b5;return true;} // LES BX,[BP + 0xe]
case 0x319707b5:{M16(ES,(BX + SI - 0x2),Sub(M16(ES,(BX + SI - 0x2)),AX));IP=0x7b9;return true;} // SUB word ptr ES:[BX + SI + -0x2],AX
case 0x319707b9:{IP=(ushort)((true)?0x6af:0x7bc);return true;} // JMP 0xe000:06af
case 0x319707bc:{AX=unchecked((ushort)(M16(DS,(0x9ef2))));IP=0x7bf;return true;} // MOV AX,[0x9ef2]
case 0x319707bf:{Sub(M16(DS,(0x9e6e)),AX);IP=0x7c3;return true;} // CMP word ptr [0x9e6e],AX
case 0x319707c3:{IP=(ushort)((!ZF)?0x7c8:0x7c5);return true;} // JNZ 0xe000:07c8
case 0x319707c5:{IP=(ushort)((true)?0x888:0x7c8);return true;} // JMP 0xe000:0888
case 0x319707c8:{M16(SS,(BP - 0x6),AX);IP=0x7cb;return true;} // MOV word ptr [BP + -0x6],AX
case 0x319707cb:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x7ce;return true;} // MOV AX,[0x9e6e]
case 0x319707ce:{AX=unchecked((ushort)(Add(AX,1)));IP=0x7cf;return true;} // INC AX
case 0x319707cf:{M16(DS,(0x9e7e),AX);IP=0x7d2;return true;} // MOV [0x9e7e],AX
case 0x319707d2:{IP=(ushort)((true)?0x802:0x7d4);return true;} // JMP 0xe000:0802
case 0x319707d4:{SI=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0x7d8;return true;} // MOV SI,word ptr [0x9e7e]
case 0x319707d8:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x7da;return true;} // SHL SI,0x1
case 0x319707da:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x7dd;return true;} // LES BX,[BP + 0xe]
case 0x319707dd:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x7e0;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x319707e0:{BX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0x7e4;return true;} // MOV BX,word ptr [0x9ed2]
case 0x319707e4:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x7e6;return true;} // SHL BX,0x1
case 0x319707e6:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x7e9;return true;} // LES SI,[BP + 0x1e]
case 0x319707e9:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX + SI - 0x2)))));IP=0x7ed;return true;} // SUB AX,word ptr ES:[BX + SI + -0x2]
case 0x319707ed:{M16(DS,(0x9edc),AX);IP=0x7f0;return true;} // MOV [0x9edc],AX
case 0x319707f0:{Sub(M16(DS,(0x43f0)),AX);IP=0x7f4;return true;} // CMP word ptr [0x43f0],AX
case 0x319707f4:{IP=(ushort)((!ZF&&SF==OF)?0x7f9:0x7f6);return true;} // JG 0xe000:07f9
case 0x319707f6:{IP=(ushort)((true)?0x6bd:0x7f9);return true;} // JMP 0xe000:06bd
case 0x319707f9:{M16(DS,(0x9e7e),Add(M16(DS,(0x9e7e)),1));IP=0x7fd;return true;} // INC word ptr [0x9e7e]
case 0x319707fd:{IP=(ushort)((!OF)?0x802:0x7ff);return true;} // JNO 0xe000:0802
case 0x319707ff:{IP=(ushort)((true)?0x888:0x802);return true;} // JMP 0xe000:0888
case 0x31970802:{AX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0x805;return true;} // MOV AX,[0x9e7e]
case 0x31970805:{Sub(M16(SS,(BP - 0x6)),AX);IP=0x808;return true;} // CMP word ptr [BP + -0x6],AX
case 0x31970808:{IP=(ushort)((SF!=OF)?0x888:0x80a);return true;} // JL 0xe000:0888
case 0x3197080a:{SI=unchecked((ushort)(AX));IP=0x80c;return true;} // MOV SI,AX
case 0x3197080c:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x80e;return true;} // SHL SI,0x1
case 0x3197080e:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x811;return true;} // LES BX,[BP + 0x16]
case 0x31970811:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x814;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x31970814:{M16(DS,(0x9ed2),AX);IP=0x817;return true;} // MOV [0x9ed2],AX
case 0x31970817:{BX=unchecked((ushort)(AX));IP=0x819;return true;} // MOV BX,AX
case 0x31970819:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x81b;return true;} // SHL BX,0x1
case 0x3197081b:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x81e;return true;} // LES SI,[BP + 0x1e]
case 0x3197081e:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x822;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970822:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9edc)))));IP=0x826;return true;} // ADD AX,word ptr [0x9edc]
case 0x31970826:{M16(DS,(0x9ed6),AX);IP=0x829;return true;} // MOV [0x9ed6],AX
case 0x31970829:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x82c;return true;} // MOV AX,[0x9eec]
case 0x3197082c:{Sub(M16(DS,(0x9ed6)),AX);IP=0x830;return true;} // CMP word ptr [0x9ed6],AX
case 0x31970830:{IP=(ushort)((!ZF&&SF==OF)?0x888:0x832);return true;} // JG 0xe000:0888
case 0x31970832:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0x835;return true;} // MOV AX,[0x9ed6]
case 0x31970835:{Sub(M16(DS,(0x43f0)),AX);IP=0x839;return true;} // CMP word ptr [0x43f0],AX
case 0x31970839:{IP=(ushort)((SF!=OF)?0x7d4:0x83b);return true;} // JL 0xe000:07d4
case 0x3197083b:{AX=unchecked((ushort)(SI));IP=0x83d;return true;} // MOV AX,SI
case 0x3197083d:{DX=unchecked((ushort)(ES));IP=0x83f;return true;} // MOV DX,ES
case 0x3197083f:{CX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0x843;return true;} // MOV CX,word ptr [0x9ed2]
case 0x31970843:{CX=unchecked((ushort)(Shl(CX,1)));IP=0x845;return true;} // SHL CX,0x1
case 0x31970845:{AX=unchecked((ushort)(Add(AX,CX)));IP=0x847;return true;} // ADD AX,CX
case 0x31970847:{M16(SS,(BP - 0x1a),AX);IP=0x84a;return true;} // MOV word ptr [BP + -0x1a],AX
case 0x3197084a:{M16(SS,(BP - 0x18),DX);IP=0x84d;return true;} // MOV word ptr [BP + -0x18],DX
case 0x3197084d:{int a=(BP - 0x1a);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x850;return true;} // LES BX,[BP + -0x1a]
case 0x31970850:{SI=unchecked((ushort)(M16(ES,(BX))));IP=0x853;return true;} // MOV SI,word ptr ES:[BX]
case 0x31970853:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0x856;return true;} // MOV AX,[0x9ed6]
case 0x31970856:{AX=unchecked((ushort)(Add(AX,SI)));IP=0x858;return true;} // ADD AX,SI
case 0x31970858:{M16(DS,(0x9ed8),AX);IP=0x85b;return true;} // MOV [0x9ed8],AX
case 0x3197085b:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x85e;return true;} // MOV AX,[0x9eec]
case 0x3197085e:{Sub(M16(DS,(0x9ed8)),AX);IP=0x862;return true;} // CMP word ptr [0x9ed8],AX
case 0x31970862:{IP=(ushort)((!ZF&&SF==OF)?0x888:0x864);return true;} // JG 0xe000:0888
case 0x31970864:{DI=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0x868;return true;} // MOV DI,word ptr [0x9e7e]
case 0x31970868:{DI=unchecked((ushort)(Shl(DI,1)));IP=0x86a;return true;} // SHL DI,0x1
case 0x3197086a:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x86d;return true;} // LES BX,[BP + 0xe]
case 0x3197086d:{AX=unchecked((ushort)(M16(ES,(BX + DI))));IP=0x870;return true;} // MOV AX,word ptr ES:[BX + DI]
case 0x31970870:{int a=(BP - 0x1a);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x873;return true;} // LES BX,[BP + -0x1a]
case 0x31970873:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX - 0x2)))));IP=0x877;return true;} // SUB AX,word ptr ES:[BX + -0x2]
case 0x31970877:{AX=unchecked((ushort)(Sub(AX,SI)));IP=0x879;return true;} // SUB AX,SI
case 0x31970879:{M16(DS,(0x9edc),AX);IP=0x87c;return true;} // MOV [0x9edc],AX
case 0x3197087c:{Sub(M16(DS,(0x43f0)),AX);IP=0x880;return true;} // CMP word ptr [0x43f0],AX
case 0x31970880:{IP=(ushort)((SF==OF)?0x885:0x882);return true;} // JGE 0xe000:0885
case 0x31970882:{IP=(ushort)((true)?0x6bd:0x885);return true;} // JMP 0xe000:06bd
case 0x31970885:{IP=(ushort)((true)?0x7f9:0x888);return true;} // JMP 0xe000:07f9
case 0x31970888:{Sub(M16(DS,(0x9e6e)),1);IP=0x88d;return true;} // CMP word ptr [0x9e6e],0x1
case 0x3197088d:{IP=(ushort)((!ZF)?0x892:0x88f);return true;} // JNZ 0xe000:0892
case 0x3197088f:{IP=(ushort)((true)?0xb28:0x892);return true;} // JMP 0xe000:0b28
case 0x31970892:{AX=unchecked((ushort)(M16(DS,(0x9ece))));IP=0x895;return true;} // MOV AX,[0x9ece]
case 0x31970895:{Sub(M16(DS,(0x9e6e)),AX);IP=0x899;return true;} // CMP word ptr [0x9e6e],AX
case 0x31970899:{IP=(ushort)((!ZF)?0x8ab:0x89b);return true;} // JNZ 0xe000:08ab
case 0x3197089b:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x89e;return true;} // MOV AX,[0x9eec]
case 0x3197089e:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9ed8)))));IP=0x8a2;return true;} // SUB AX,word ptr [0x9ed8]
case 0x319708a2:{Sub(AX,M16(DS,(0x9eee)));IP=0x8a6;return true;} // CMP AX,word ptr [0x9eee]
case 0x319708a6:{IP=(ushort)((!ZF)?0x8ab:0x8a8);return true;} // JNZ 0xe000:08ab
case 0x319708a8:{IP=(ushort)((true)?0xb28:0x8ab);return true;} // JMP 0xe000:0b28
case 0x319708ab:{AX=unchecked((ushort)(M16(DS,(0x9ede))));IP=0x8ae;return true;} // MOV AX,[0x9ede]
case 0x319708ae:{Sub(M16(DS,(0x9ee0)),AX);IP=0x8b2;return true;} // CMP word ptr [0x9ee0],AX
case 0x319708b2:{IP=(ushort)((ZF)?0x8bc:0x8b4);return true;} // JZ 0xe000:08bc
case 0x319708b4:{M16(DS,(0x9ef6),0);IP=0x8ba;return true;} // MOV word ptr [0x9ef6],0x0
case 0x319708ba:{IP=(ushort)((true)?0x8ca:0x8bc);return true;} // JMP 0xe000:08ca
case 0x319708bc:{M16(DS,(0x9ef6),Add(M16(DS,(0x9ef6)),1));IP=0x8c0;return true;} // INC word ptr [0x9ef6]
case 0x319708c0:{Sub(M16(DS,(0x9ef6)),10);IP=0x8c5;return true;} // CMP word ptr [0x9ef6],0xa
case 0x319708c5:{IP=(ushort)((!ZF)?0x8ca:0x8c7);return true;} // JNZ 0xe000:08ca
case 0x319708c7:{IP=(ushort)((true)?0xb95:0x8ca);return true;} // JMP 0xe000:0b95
case 0x319708ca:{AX=unchecked((ushort)(M16(DS,(0x9ede))));IP=0x8cd;return true;} // MOV AX,[0x9ede]
case 0x319708cd:{M16(DS,(0x9ee0),AX);IP=0x8d0;return true;} // MOV [0x9ee0],AX
case 0x319708d0:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x8d3;return true;} // MOV AX,[0x9e6e]
case 0x319708d3:{M16(DS,(0x9ece),AX);IP=0x8d6;return true;} // MOV [0x9ece],AX
case 0x319708d6:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x8d9;return true;} // MOV AX,[0x9eec]
case 0x319708d9:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9ed8)))));IP=0x8dd;return true;} // SUB AX,word ptr [0x9ed8]
case 0x319708dd:{M16(DS,(0x9eee),AX);IP=0x8e0;return true;} // MOV [0x9eee],AX
case 0x319708e0:{SI=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x8e4;return true;} // MOV SI,word ptr [0x9e6e]
case 0x319708e4:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x8e6;return true;} // SHL SI,0x1
case 0x319708e6:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x8e9;return true;} // LES BX,[BP + 0x16]
case 0x319708e9:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x8ed;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x319708ed:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x8ee;return true;} // DEC AX
case 0x319708ee:{M16(DS,(0x9e6c),AX);IP=0x8f1;return true;} // MOV [0x9e6c],AX
case 0x319708f1:{AX=unchecked((ushort)(Shl(AX,1)));IP=0x8f3;return true;} // SHL AX,0x1
case 0x319708f3:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x8f6;return true;} // LES BX,[BP + 0x1e]
case 0x319708f6:{BX=unchecked((ushort)(Add(BX,AX)));IP=0x8f8;return true;} // ADD BX,AX
case 0x319708f8:{DI=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x8fc;return true;} // MOV DI,word ptr ES:[BX + -0x2]
case 0x319708fc:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x8ff;return true;} // LES BX,[BP + 0xe]
case 0x319708ff:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0x903;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970903:{AX=unchecked((ushort)(Add(AX,DI)));IP=0x905;return true;} // ADD AX,DI
case 0x31970905:{M16(DS,(0x9ec8),AX);IP=0x908;return true;} // MOV [0x9ec8],AX
case 0x31970908:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0x90b;return true;} // MOV AX,[0x9eec]
case 0x3197090b:{Sub(M16(DS,(0x9ec8)),AX);IP=0x90f;return true;} // CMP word ptr [0x9ec8],AX
case 0x3197090f:{IP=(ushort)((ZF||SF!=OF)?0x914:0x911);return true;} // JLE 0xe000:0914
case 0x31970911:{IP=(ushort)((true)?0xb28:0x914);return true;} // JMP 0xe000:0b28
case 0x31970914:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x4))));IP=0x918;return true;} // MOV AX,word ptr ES:[BX + SI + -0x4]
case 0x31970918:{AX=unchecked((ushort)(Sub(AX,DI)));IP=0x91a;return true;} // SUB AX,DI
case 0x3197091a:{M16(DS,(0x9edc),AX);IP=0x91d;return true;} // MOV [0x9edc],AX
case 0x3197091d:{Sub(M16(DS,(0x43f0)),AX);IP=0x921;return true;} // CMP word ptr [0x43f0],AX
case 0x31970921:{IP=(ushort)((ZF||SF!=OF)?0x926:0x923);return true;} // JLE 0xe000:0926
case 0x31970923:{IP=(ushort)((true)?0xa5b:0x926);return true;} // JMP 0xe000:0a5b
case 0x31970926:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0x929;return true;} // MOV AX,[0x9ec8]
case 0x31970929:{M16(ES,(BX + SI - 0x2),AX);IP=0x92d;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x3197092d:{AX=unchecked((ushort)(M16(DS,(0x9edc))));IP=0x930;return true;} // MOV AX,[0x9edc]
case 0x31970930:{M16(ES,(BX + SI - 0x4),AX);IP=0x934;return true;} // MOV word ptr ES:[BX + SI + -0x4],AX
case 0x31970934:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x937;return true;} // LES BX,[BP + 0x16]
case 0x31970937:{AX=unchecked((ushort)(M16(DS,(0x9e6c))));IP=0x93a;return true;} // MOV AX,[0x9e6c]
case 0x3197093a:{IP=(ushort)((true)?0x78d:0x93d);return true;} // JMP 0xe000:078d
case 0x3197093d:{BX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x941;return true;} // MOV BX,word ptr [0x9e6e]
case 0x31970941:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x943;return true;} // SHL BX,0x1
case 0x31970943:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x946;return true;} // LES SI,[BP + 0xe]
case 0x31970946:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0x949;return true;} // MOV AX,[0x9ec8]
case 0x31970949:{M16(ES,(BX + SI - 0x2),AX);IP=0x94d;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x3197094d:{AX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0x950;return true;} // MOV AX,[0x9e7e]
case 0x31970950:{M16(SS,(BP - 0x2),AX);IP=0x953;return true;} // MOV word ptr [BP + -0x2],AX
case 0x31970953:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x956;return true;} // MOV AX,[0x9e6e]
case 0x31970956:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x957;return true;} // DEC AX
case 0x31970957:{M16(DS,(0x9e70),AX);IP=0x95a;return true;} // MOV [0x9e70],AX
case 0x3197095a:{IP=(ushort)((true)?0x9ed:0x95d);return true;} // JMP 0xe000:09ed
case 0x3197095d:{AX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x960;return true;} // MOV AX,[0x9e70]
case 0x31970960:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x961;return true;} // DEC AX
case 0x31970961:{M16(DS,(0x9e78),AX);IP=0x964;return true;} // MOV [0x9e78],AX
case 0x31970964:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x967;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x31970967:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x96a;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x3197096a:{CX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x96e;return true;} // MOV CX,word ptr [0x9e70]
case 0x3197096e:{CX=unchecked((ushort)(Shl(CX,1)));IP=0x970;return true;} // SHL CX,0x1
case 0x31970970:{AX=unchecked((ushort)(Add(AX,CX)));IP=0x972;return true;} // ADD AX,CX
case 0x31970972:{AX=unchecked((ushort)(Sub(AX,2)));IP=0x975;return true;} // SUB AX,0x2
case 0x31970975:{M16(SS,(BP - 0x1a),AX);IP=0x978;return true;} // MOV word ptr [BP + -0x1a],AX
case 0x31970978:{M16(SS,(BP - 0x18),DX);IP=0x97b;return true;} // MOV word ptr [BP + -0x18],DX
case 0x3197097b:{int a=(BP - 0x1a);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x97e;return true;} // LES BX,[BP + -0x1a]
case 0x3197097e:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0x981;return true;} // MOV AX,word ptr ES:[BX]
case 0x31970981:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x982;return true;} // DEC AX
case 0x31970982:{M16(DS,(0x9e80),AX);IP=0x985;return true;} // MOV [0x9e80],AX
case 0x31970985:{AX=unchecked((ushort)(M16(SS,(BP + 0x1e))));IP=0x988;return true;} // MOV AX,word ptr [BP + 0x1e]
case 0x31970988:{DX=unchecked((ushort)(M16(SS,(BP + 0x20))));IP=0x98b;return true;} // MOV DX,word ptr [BP + 0x20]
case 0x3197098b:{CX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0x98f;return true;} // MOV CX,word ptr [0x9e80]
case 0x3197098f:{CX=unchecked((ushort)(Shl(CX,1)));IP=0x991;return true;} // SHL CX,0x1
case 0x31970991:{AX=unchecked((ushort)(Add(AX,CX)));IP=0x993;return true;} // ADD AX,CX
case 0x31970993:{M16(SS,(BP - 0x1e),AX);IP=0x996;return true;} // MOV word ptr [BP + -0x1e],AX
case 0x31970996:{M16(SS,(BP - 0x1c),DX);IP=0x999;return true;} // MOV word ptr [BP + -0x1c],DX
case 0x31970999:{int a=(BP - 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x99c;return true;} // LES BX,[BP + -0x1e]
case 0x3197099c:{AX=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x9a0;return true;} // MOV AX,word ptr ES:[BX + -0x2]
case 0x319709a0:{M16(DS,(0x9e74),Add(M16(DS,(0x9e74)),AX));IP=0x9a4;return true;} // ADD word ptr [0x9e74],AX
case 0x319709a4:{AX=unchecked((ushort)(M16(DS,(0x9e74))));IP=0x9a7;return true;} // MOV AX,[0x9e74]
case 0x319709a7:{Sub(M16(DS,(0x43f0)),AX);IP=0x9ab;return true;} // CMP word ptr [0x43f0],AX
case 0x319709ab:{IP=(ushort)((SF!=OF)?0x9d6:0x9ad);return true;} // JL 0xe000:09d6
case 0x319709ad:{AX=unchecked((ushort)(M16(ES,(BX - 0x4))));IP=0x9b1;return true;} // MOV AX,word ptr ES:[BX + -0x4]
case 0x319709b1:{M16(DS,(0x9e74),Add(M16(DS,(0x9e74)),AX));IP=0x9b5;return true;} // ADD word ptr [0x9e74],AX
case 0x319709b5:{int a=(BP - 0x1a);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x9b8;return true;} // LES BX,[BP + -0x1a]
case 0x319709b8:{AX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0x9bb;return true;} // MOV AX,[0x9e80]
case 0x319709bb:{M16(ES,(BX),AX);IP=0x9be;return true;} // MOV word ptr ES:[BX],AX
case 0x319709be:{int a=(BP - 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x9c1;return true;} // LES BX,[BP + -0x1e]
case 0x319709c1:{AX=unchecked((ushort)(M16(ES,(BX - 0x2))));IP=0x9c5;return true;} // MOV AX,word ptr ES:[BX + -0x2]
case 0x319709c5:{BX=unchecked((ushort)(M16(DS,(0x9e78))));IP=0x9c9;return true;} // MOV BX,word ptr [0x9e78]
case 0x319709c9:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x9cb;return true;} // SHL BX,0x1
case 0x319709cb:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x9ce;return true;} // LES SI,[BP + 0xe]
case 0x319709ce:{M16(ES,(BX + SI - 0x2),Sub(M16(ES,(BX + SI - 0x2)),AX));IP=0x9d2;return true;} // SUB word ptr ES:[BX + SI + -0x2],AX
case 0x319709d2:{M16(DS,(0x9e80),Sub(M16(DS,(0x9e80)),1));IP=0x9d6;return true;} // DEC word ptr [0x9e80]
case 0x319709d6:{BX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x9da;return true;} // MOV BX,word ptr [0x9e70]
case 0x319709da:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x9dc;return true;} // SHL BX,0x1
case 0x319709dc:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x9df;return true;} // LES SI,[BP + 0xe]
case 0x319709df:{AX=unchecked((ushort)(M16(DS,(0x9e74))));IP=0x9e2;return true;} // MOV AX,[0x9e74]
case 0x319709e2:{M16(ES,(BX + SI - 0x2),AX);IP=0x9e6;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x319709e6:{M16(DS,(0x9e70),Add(M16(DS,(0x9e70)),-1));IP=0x9eb;return true;} // ADD word ptr [0x9e70],-0x1
case 0x319709eb:{IP=(ushort)((OF)?0xa32:0x9ed);return true;} // JO 0xe000:0a32
case 0x319709ed:{AX=unchecked((ushort)(M16(DS,(0x9e70))));IP=0x9f0;return true;} // MOV AX,[0x9e70]
case 0x319709f0:{Sub(M16(SS,(BP - 0x2)),AX);IP=0x9f3;return true;} // CMP word ptr [BP + -0x2],AX
case 0x319709f3:{IP=(ushort)((!ZF&&SF==OF)?0xa32:0x9f5);return true;} // JG 0xe000:0a32
case 0x319709f5:{SI=unchecked((ushort)(AX));IP=0x9f7;return true;} // MOV SI,AX
case 0x319709f7:{SI=unchecked((ushort)(Shl(SI,1)));IP=0x9f9;return true;} // SHL SI,0x1
case 0x319709f9:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x9fc;return true;} // LES BX,[BP + 0x16]
case 0x319709fc:{AX=unchecked((ushort)(M16(ES,(BX + SI))));IP=0x9ff;return true;} // MOV AX,word ptr ES:[BX + SI]
case 0x319709ff:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xa00;return true;} // DEC AX
case 0x31970a00:{M16(DS,(0x9e7c),AX);IP=0xa03;return true;} // MOV [0x9e7c],AX
case 0x31970a03:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa06;return true;} // LES BX,[BP + 0xe]
case 0x31970a06:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xa0a;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970a0a:{BX=unchecked((ushort)(M16(DS,(0x9e7c))));IP=0xa0e;return true;} // MOV BX,word ptr [0x9e7c]
case 0x31970a0e:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xa10;return true;} // SHL BX,0x1
case 0x31970a10:{int a=(BP + 0x1e);ushort sg=SS;DI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa13;return true;} // LES DI,[BP + 0x1e]
case 0x31970a13:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX + DI - 0x2)))));IP=0xa17;return true;} // SUB AX,word ptr ES:[BX + DI + -0x2]
case 0x31970a17:{M16(DS,(0x9e74),AX);IP=0xa1a;return true;} // MOV [0x9e74],AX
case 0x31970a1a:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa1d;return true;} // LES BX,[BP + 0x16]
case 0x31970a1d:{AX=unchecked((ushort)(M16(DS,(0x9e7c))));IP=0xa20;return true;} // MOV AX,[0x9e7c]
case 0x31970a20:{M16(ES,(BX + SI),AX);IP=0xa23;return true;} // MOV word ptr ES:[BX + SI],AX
case 0x31970a23:{AX=unchecked((ushort)(M16(DS,(0x9e74))));IP=0xa26;return true;} // MOV AX,[0x9e74]
case 0x31970a26:{Sub(M16(DS,(0x43f0)),AX);IP=0xa2a;return true;} // CMP word ptr [0x43f0],AX
case 0x31970a2a:{IP=(ushort)((ZF||SF!=OF)?0xa2f:0xa2c);return true;} // JLE 0xe000:0a2f
case 0x31970a2c:{IP=(ushort)((true)?0x95d:0xa2f);return true;} // JMP 0xe000:095d
case 0x31970a2f:{IP=(ushort)((true)?0x78a:0xa32);return true;} // JMP 0xe000:078a
case 0x31970a32:{BX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0xa36;return true;} // MOV BX,word ptr [0x9e80]
case 0x31970a36:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xa38;return true;} // SHL BX,0x1
case 0x31970a38:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa3b;return true;} // LES SI,[BP + 0x1e]
case 0x31970a3b:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xa3f;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970a3f:{BX=unchecked((ushort)(M16(DS,(0x9e78))));IP=0xa43;return true;} // MOV BX,word ptr [0x9e78]
case 0x31970a43:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xa45;return true;} // SHL BX,0x1
case 0x31970a45:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa48;return true;} // LES SI,[BP + 0xe]
case 0x31970a48:{M16(ES,(BX + SI - 0x2),Sub(M16(ES,(BX + SI - 0x2)),AX));IP=0xa4c;return true;} // SUB word ptr ES:[BX + SI + -0x2],AX
case 0x31970a4c:{BX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0xa50;return true;} // MOV BX,word ptr [0x9e7e]
case 0x31970a50:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xa52;return true;} // SHL BX,0x1
case 0x31970a52:{int a=(BP + 0x16);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa55;return true;} // LES SI,[BP + 0x16]
case 0x31970a55:{AX=unchecked((ushort)(M16(DS,(0x9e80))));IP=0xa58;return true;} // MOV AX,[0x9e80]
case 0x31970a58:{IP=(ushort)((true)?0x78d:0xa5b);return true;} // JMP 0xe000:078d
case 0x31970a5b:{Sub(M16(DS,(0x9e6e)),2);IP=0xa60;return true;} // CMP word ptr [0x9e6e],0x2
case 0x31970a60:{IP=(ushort)((!ZF&&SF==OF)?0xa65:0xa62);return true;} // JG 0xe000:0a65
case 0x31970a62:{IP=(ushort)((true)?0xb28:0xa65);return true;} // JMP 0xe000:0b28
case 0x31970a65:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0xa68;return true;} // MOV AX,[0x9e6e]
case 0x31970a68:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xa69;return true;} // DEC AX
case 0x31970a69:{M16(DS,(0x9e7e),AX);IP=0xa6c;return true;} // MOV [0x9e7e],AX
case 0x31970a6c:{IP=(ushort)((true)?0xa99:0xa6e);return true;} // JMP 0xe000:0a99
case 0x31970a6e:{BX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0xa72;return true;} // MOV BX,word ptr [0x9e7e]
case 0x31970a72:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xa74;return true;} // SHL BX,0x1
case 0x31970a74:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa77;return true;} // LES SI,[BP + 0xe]
case 0x31970a77:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x4))));IP=0xa7b;return true;} // MOV AX,word ptr ES:[BX + SI + -0x4]
case 0x31970a7b:{BX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0xa7f;return true;} // MOV BX,word ptr [0x9ed2]
case 0x31970a7f:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xa81;return true;} // SHL BX,0x1
case 0x31970a81:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xa84;return true;} // LES SI,[BP + 0x1e]
case 0x31970a84:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX + SI - 0x2)))));IP=0xa88;return true;} // SUB AX,word ptr ES:[BX + SI + -0x2]
case 0x31970a88:{M16(DS,(0x9edc),AX);IP=0xa8b;return true;} // MOV [0x9edc],AX
case 0x31970a8b:{Sub(M16(DS,(0x43f0)),AX);IP=0xa8f;return true;} // CMP word ptr [0x43f0],AX
case 0x31970a8f:{IP=(ushort)((!ZF&&SF==OF)?0xa94:0xa91);return true;} // JG 0xe000:0a94
case 0x31970a91:{IP=(ushort)((true)?0x93d:0xa94);return true;} // JMP 0xe000:093d
case 0x31970a94:{M16(DS,(0x9e7e),Add(M16(DS,(0x9e7e)),-1));IP=0xa99;return true;} // ADD word ptr [0x9e7e],-0x1
case 0x31970a99:{Sub(M16(DS,(0x9e7e)),2);IP=0xa9e;return true;} // CMP word ptr [0x9e7e],0x2
case 0x31970a9e:{IP=(ushort)((SF==OF)?0xaa3:0xaa0);return true;} // JGE 0xe000:0aa3
case 0x31970aa0:{IP=(ushort)((true)?0xb28:0xaa3);return true;} // JMP 0xe000:0b28
case 0x31970aa3:{BX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0xaa7;return true;} // MOV BX,word ptr [0x9e7e]
case 0x31970aa7:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xaa9;return true;} // SHL BX,0x1
case 0x31970aa9:{int a=(BP + 0x16);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xaac;return true;} // LES SI,[BP + 0x16]
case 0x31970aac:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xab0;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970ab0:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xab1;return true;} // DEC AX
case 0x31970ab1:{M16(DS,(0x9ed2),AX);IP=0xab4;return true;} // MOV [0x9ed2],AX
case 0x31970ab4:{BX=unchecked((ushort)(AX));IP=0xab6;return true;} // MOV BX,AX
case 0x31970ab6:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xab8;return true;} // SHL BX,0x1
case 0x31970ab8:{int a=(BP + 0x1e);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xabb;return true;} // LES SI,[BP + 0x1e]
case 0x31970abb:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xabf;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970abf:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9edc)))));IP=0xac3;return true;} // ADD AX,word ptr [0x9edc]
case 0x31970ac3:{M16(DS,(0x9ed6),AX);IP=0xac6;return true;} // MOV [0x9ed6],AX
case 0x31970ac6:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0xac9;return true;} // MOV AX,[0x9eec]
case 0x31970ac9:{Sub(M16(DS,(0x9ed6)),AX);IP=0xacd;return true;} // CMP word ptr [0x9ed6],AX
case 0x31970acd:{IP=(ushort)((!ZF&&SF==OF)?0xb28:0xacf);return true;} // JG 0xe000:0b28
case 0x31970acf:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0xad2;return true;} // MOV AX,[0x9ed6]
case 0x31970ad2:{Sub(M16(DS,(0x43f0)),AX);IP=0xad6;return true;} // CMP word ptr [0x43f0],AX
case 0x31970ad6:{IP=(ushort)((SF!=OF)?0xa6e:0xad8);return true;} // JL 0xe000:0a6e
case 0x31970ad8:{AX=unchecked((ushort)(SI));IP=0xada;return true;} // MOV AX,SI
case 0x31970ada:{DX=unchecked((ushort)(ES));IP=0xadc;return true;} // MOV DX,ES
case 0x31970adc:{CX=unchecked((ushort)(M16(DS,(0x9ed2))));IP=0xae0;return true;} // MOV CX,word ptr [0x9ed2]
case 0x31970ae0:{CX=unchecked((ushort)(Shl(CX,1)));IP=0xae2;return true;} // SHL CX,0x1
case 0x31970ae2:{AX=unchecked((ushort)(Add(AX,CX)));IP=0xae4;return true;} // ADD AX,CX
case 0x31970ae4:{M16(SS,(BP - 0x1e),AX);IP=0xae7;return true;} // MOV word ptr [BP + -0x1e],AX
case 0x31970ae7:{M16(SS,(BP - 0x1c),DX);IP=0xaea;return true;} // MOV word ptr [BP + -0x1c],DX
case 0x31970aea:{int a=(BP - 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xaed;return true;} // LES BX,[BP + -0x1e]
case 0x31970aed:{AX=unchecked((ushort)(M16(ES,(BX - 0x4))));IP=0xaf1;return true;} // MOV AX,word ptr ES:[BX + -0x4]
case 0x31970af1:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9ed6)))));IP=0xaf5;return true;} // ADD AX,word ptr [0x9ed6]
case 0x31970af5:{M16(DS,(0x9ed8),AX);IP=0xaf8;return true;} // MOV [0x9ed8],AX
case 0x31970af8:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0xafb;return true;} // MOV AX,[0x9eec]
case 0x31970afb:{Sub(M16(DS,(0x9ed8)),AX);IP=0xaff;return true;} // CMP word ptr [0x9ed8],AX
case 0x31970aff:{IP=(ushort)((!ZF&&SF==OF)?0xb28:0xb01);return true;} // JG 0xe000:0b28
case 0x31970b01:{BX=unchecked((ushort)(M16(DS,(0x9e7e))));IP=0xb05;return true;} // MOV BX,word ptr [0x9e7e]
case 0x31970b05:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xb07;return true;} // SHL BX,0x1
case 0x31970b07:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xb0a;return true;} // LES SI,[BP + 0xe]
case 0x31970b0a:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x4))));IP=0xb0e;return true;} // MOV AX,word ptr ES:[BX + SI + -0x4]
case 0x31970b0e:{int a=(BP - 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xb11;return true;} // LES BX,[BP + -0x1e]
case 0x31970b11:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX - 0x4)))));IP=0xb15;return true;} // SUB AX,word ptr ES:[BX + -0x4]
case 0x31970b15:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX - 0x2)))));IP=0xb19;return true;} // SUB AX,word ptr ES:[BX + -0x2]
case 0x31970b19:{M16(DS,(0x9edc),AX);IP=0xb1c;return true;} // MOV [0x9edc],AX
case 0x31970b1c:{Sub(M16(DS,(0x43f0)),AX);IP=0xb20;return true;} // CMP word ptr [0x43f0],AX
case 0x31970b20:{IP=(ushort)((SF==OF)?0xb25:0xb22);return true;} // JGE 0xe000:0b25
case 0x31970b22:{IP=(ushort)((true)?0x93d:0xb25);return true;} // JMP 0xe000:093d
case 0x31970b25:{IP=(ushort)((true)?0xa94:0xb28);return true;} // JMP 0xe000:0a94
case 0x31970b28:{AX=unchecked((ushort)(M16(DS,(0x9ed4))));IP=0xb2b;return true;} // MOV AX,[0x9ed4]
case 0x31970b2b:{Sub(M16(DS,(0x9e6e)),AX);IP=0xb2f;return true;} // CMP word ptr [0x9e6e],AX
case 0x31970b2f:{IP=(ushort)((!ZF)?0xb3e:0xb31);return true;} // JNZ 0xe000:0b3e
case 0x31970b31:{AX=unchecked((ushort)(M16(DS,(0x9eec))));IP=0xb34;return true;} // MOV AX,[0x9eec]
case 0x31970b34:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9ed8)))));IP=0xb38;return true;} // SUB AX,word ptr [0x9ed8]
case 0x31970b38:{Sub(AX,M16(DS,(0x9eee)));IP=0xb3c;return true;} // CMP AX,word ptr [0x9eee]
case 0x31970b3c:{IP=(ushort)((ZF)?0xb95:0xb3e);return true;} // JZ 0xe000:0b95
case 0x31970b3e:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0xb41;return true;} // MOV AX,[0x9e6e]
case 0x31970b41:{M16(DS,(0x9ed4),AX);IP=0xb44;return true;} // MOV [0x9ed4],AX
case 0x31970b44:{Sub(M16(DS,(0x43f4)),AX);IP=0xb48;return true;} // CMP word ptr [0x43f4],AX
case 0x31970b48:{IP=(ushort)((ZF)?0xb95:0xb4a);return true;} // JZ 0xe000:0b95
case 0x31970b4a:{AX=unchecked((ushort)(Add(AX,1)));IP=0xb4b;return true;} // INC AX
case 0x31970b4b:{M16(DS,(0x9e72),AX);IP=0xb4e;return true;} // MOV [0x9e72],AX
case 0x31970b4e:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0xb51;return true;} // MOV AX,[0x43f4]
case 0x31970b51:{Sub(M16(DS,(0x9e72)),AX);IP=0xb55;return true;} // CMP word ptr [0x9e72],AX
case 0x31970b55:{IP=(ushort)((!ZF&&SF==OF)?0xb95:0xb57);return true;} // JG 0xe000:0b95
case 0x31970b57:{AX=unchecked((ushort)(M16(SS,(BP + 0xe))));IP=0xb5a;return true;} // MOV AX,word ptr [BP + 0xe]
case 0x31970b5a:{DX=unchecked((ushort)(M16(SS,(BP + 0x10))));IP=0xb5d;return true;} // MOV DX,word ptr [BP + 0x10]
case 0x31970b5d:{CX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xb61;return true;} // MOV CX,word ptr [0x9e72]
case 0x31970b61:{CX=unchecked((ushort)(Shl(CX,1)));IP=0xb63;return true;} // SHL CX,0x1
case 0x31970b63:{AX=unchecked((ushort)(Add(AX,CX)));IP=0xb65;return true;} // ADD AX,CX
case 0x31970b65:{AX=unchecked((ushort)(Sub(AX,2)));IP=0xb68;return true;} // SUB AX,0x2
case 0x31970b68:{M16(SS,(BP - 0x24),AX);IP=0xb6b;return true;} // MOV word ptr [BP + -0x24],AX
case 0x31970b6b:{M16(SS,(BP - 0x22),DX);IP=0xb6e;return true;} // MOV word ptr [BP + -0x22],DX
case 0x31970b6e:{int a=(BP - 0x24);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xb71;return true;} // LES BX,[BP + -0x24]
case 0x31970b71:{AX=unchecked((ushort)(M16(DS,(0x43f0))));IP=0xb74;return true;} // MOV AX,[0x43f0]
case 0x31970b74:{Sub(M16(ES,(BX)),AX);IP=0xb77;return true;} // CMP word ptr ES:[BX],AX
case 0x31970b77:{IP=(ushort)((!ZF)?0xb82:0xb79);return true;} // JNZ 0xe000:0b82
case 0x31970b79:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xb7c;return true;} // MOV AX,[0x9e72]
case 0x31970b7c:{M16(DS,(0x9e6e),AX);IP=0xb7f;return true;} // MOV [0x9e6e],AX
case 0x31970b7f:{IP=(ushort)((true)?0x64b:0xb82);return true;} // JMP 0xe000:064b
case 0x31970b82:{M16(SS,(BP - 0x24),Add(M16(SS,(BP - 0x24)),2));IP=0xb86;return true;} // ADD word ptr [BP + -0x24],0x2
case 0x31970b86:{M16(DS,(0x9e72),Add(M16(DS,(0x9e72)),1));IP=0xb8a;return true;} // INC word ptr [0x9e72]
case 0x31970b8a:{IP=(ushort)((OF)?0xb95:0xb8c);return true;} // JO 0xe000:0b95
case 0x31970b8c:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0xb8f;return true;} // MOV AX,[0x43f4]
case 0x31970b8f:{Sub(M16(DS,(0x9e72)),AX);IP=0xb93;return true;} // CMP word ptr [0x9e72],AX
case 0x31970b93:{IP=(ushort)((ZF||SF!=OF)?0xb6e:0xb95);return true;} // JLE 0xe000:0b6e
case 0x31970b95:{Push(M16(SS,(BP + 0x18)));IP=0xb98;return true;} // PUSH word ptr [BP + 0x18]
case 0x31970b98:{Push(M16(SS,(BP + 0x16)));IP=0xb9b;return true;} // PUSH word ptr [BP + 0x16]
case 0x31970b9b:{Push(M16(SS,(BP + 0x20)));IP=0xb9e;return true;} // PUSH word ptr [BP + 0x20]
case 0x31970b9e:{Push(M16(SS,(BP + 0x1e)));IP=0xba1;return true;} // PUSH word ptr [BP + 0x1e]
case 0x31970ba1:{Push(M16(SS,(BP + 0x14)));IP=0xba4;return true;} // PUSH word ptr [BP + 0x14]
case 0x31970ba4:{Push(M16(SS,(BP + 0x12)));IP=0xba7;return true;} // PUSH word ptr [BP + 0x12]
case 0x31970ba7:{Push(M16(SS,(BP + 0x10)));IP=0xbaa;return true;} // PUSH word ptr [BP + 0x10]
case 0x31970baa:{Push(M16(SS,(BP + 0xe)));IP=0xbad;return true;} // PUSH word ptr [BP + 0xe]
case 0x31970bad:{Push(CS);Push(0xbb2);CS=0x3197;IP=0x13f6;return true;} // CALLF 0x3000:2d66
case 0x31970bb2:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0xbb5;return true;} // MOV AX,[0x43f4]
case 0x31970bb5:{M16(SS,(BP - 0x24),AX);IP=0xbb8;return true;} // MOV word ptr [BP + -0x24],AX
case 0x31970bb8:{M16(DS,(0x9e76),2);IP=0xbbe;return true;} // MOV word ptr [0x9e76],0x2
case 0x31970bbe:{IP=(ushort)((true)?0xc0b:0xbc0);return true;} // JMP 0xe000:0c0b
case 0x31970bc0:{BX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xbc4;return true;} // MOV BX,word ptr [0x9e72]
case 0x31970bc4:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xbc6;return true;} // SHL BX,0x1
case 0x31970bc6:{int a=(BP + 0x1e);ushort sg=SS;DI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xbc9;return true;} // LES DI,[BP + 0x1e]
case 0x31970bc9:{AX=unchecked((ushort)(M16(ES,(BX + DI - 0x2))));IP=0xbcd;return true;} // MOV AX,word ptr ES:[BX + DI + -0x2]
case 0x31970bcd:{M16(DS,(0x9e7e),AX);IP=0xbd0;return true;} // MOV [0x9e7e],AX
case 0x31970bd0:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0xbd3;return true;} // MOV AX,[0x9ed6]
case 0x31970bd3:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9ecc)))));IP=0xbd7;return true;} // SUB AX,word ptr [0x9ecc]
case 0x31970bd7:{Sub(AX,M16(DS,(0x9e7e)));IP=0xbdb;return true;} // CMP AX,word ptr [0x9e7e]
case 0x31970bdb:{IP=(ushort)((ZF||SF!=OF)?0xc05:0xbdd);return true;} // JLE 0xe000:0c05
case 0x31970bdd:{SI=unchecked((ushort)(M16(DS,(0x9e76))));IP=0xbe1;return true;} // MOV SI,word ptr [0x9e76]
case 0x31970be1:{SI=unchecked((ushort)(Shl(SI,1)));IP=0xbe3;return true;} // SHL SI,0x1
case 0x31970be3:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xbe6;return true;} // LES BX,[BP + 0xe]
case 0x31970be6:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0xbe9;return true;} // MOV AX,[0x9ecc]
case 0x31970be9:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9e7e)))));IP=0xbed;return true;} // ADD AX,word ptr [0x9e7e]
case 0x31970bed:{M16(ES,(BX + SI - 0x4),AX);IP=0xbf1;return true;} // MOV word ptr ES:[BX + SI + -0x4],AX
case 0x31970bf1:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0xbf4;return true;} // MOV AX,[0x9ed6]
case 0x31970bf4:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9e7e)))));IP=0xbf8;return true;} // SUB AX,word ptr [0x9e7e]
case 0x31970bf8:{M16(ES,(BX + SI - 0x2),AX);IP=0xbfc;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970bfc:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xbff;return true;} // LES BX,[BP + 0x16]
case 0x31970bff:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xc02;return true;} // MOV AX,[0x9e72]
case 0x31970c02:{AX=unchecked((ushort)(Add(AX,1)));IP=0xc03;return true;} // INC AX
case 0x31970c03:{IP=(ushort)((true)?0xc75:0xc05);return true;} // JMP 0xe000:0c75
case 0x31970c05:{M16(DS,(0x9e76),Add(M16(DS,(0x9e76)),1));IP=0xc09;return true;} // INC word ptr [0x9e76]
case 0x31970c09:{IP=(ushort)((OF)?0xc7c:0xc0b);return true;} // JO 0xe000:0c7c
case 0x31970c0b:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0xc0e;return true;} // MOV AX,[0x9e76]
case 0x31970c0e:{Sub(M16(SS,(BP - 0x24)),AX);IP=0xc11;return true;} // CMP word ptr [BP + -0x24],AX
case 0x31970c11:{IP=(ushort)((SF!=OF)?0xc7c:0xc13);return true;} // JL 0xe000:0c7c
case 0x31970c13:{SI=unchecked((ushort)(AX));IP=0xc15;return true;} // MOV SI,AX
case 0x31970c15:{SI=unchecked((ushort)(Shl(SI,1)));IP=0xc17;return true;} // SHL SI,0x1
case 0x31970c17:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc1a;return true;} // LES BX,[BP + 0x16]
case 0x31970c1a:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xc1e;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970c1e:{M16(DS,(0x9e72),AX);IP=0xc21;return true;} // MOV [0x9e72],AX
case 0x31970c21:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc24;return true;} // LES BX,[BP + 0xe]
case 0x31970c24:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xc28;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970c28:{M16(DS,(0x9ed6),AX);IP=0xc2b;return true;} // MOV [0x9ed6],AX
case 0x31970c2b:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x4))));IP=0xc2f;return true;} // MOV AX,word ptr ES:[BX + SI + -0x4]
case 0x31970c2f:{M16(DS,(0x9ecc),AX);IP=0xc32;return true;} // MOV [0x9ecc],AX
case 0x31970c32:{Sub(M16(DS,(0x9ed6)),AX);IP=0xc36;return true;} // CMP word ptr [0x9ed6],AX
case 0x31970c36:{IP=(ushort)((SF==OF)?0xbc0:0xc38);return true;} // JGE 0xe000:0bc0
case 0x31970c38:{BX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xc3c;return true;} // MOV BX,word ptr [0x9e72]
case 0x31970c3c:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xc3e;return true;} // SHL BX,0x1
case 0x31970c3e:{int a=(BP + 0x1e);ushort sg=SS;DI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc41;return true;} // LES DI,[BP + 0x1e]
case 0x31970c41:{AX=unchecked((ushort)(M16(ES,(BX + DI - 0x4))));IP=0xc45;return true;} // MOV AX,word ptr ES:[BX + DI + -0x4]
case 0x31970c45:{M16(DS,(0x9e7e),AX);IP=0xc48;return true;} // MOV [0x9e7e],AX
case 0x31970c48:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0xc4b;return true;} // MOV AX,[0x9ecc]
case 0x31970c4b:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9ed6)))));IP=0xc4f;return true;} // SUB AX,word ptr [0x9ed6]
case 0x31970c4f:{Sub(AX,M16(DS,(0x9e7e)));IP=0xc53;return true;} // CMP AX,word ptr [0x9e7e]
case 0x31970c53:{IP=(ushort)((SF!=OF)?0xc05:0xc55);return true;} // JL 0xe000:0c05
case 0x31970c55:{int a=(BP + 0xe);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc58;return true;} // LES BX,[BP + 0xe]
case 0x31970c58:{AX=unchecked((ushort)(M16(DS,(0x9ecc))));IP=0xc5b;return true;} // MOV AX,[0x9ecc]
case 0x31970c5b:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x9e7e)))));IP=0xc5f;return true;} // SUB AX,word ptr [0x9e7e]
case 0x31970c5f:{M16(ES,(BX + SI - 0x4),AX);IP=0xc63;return true;} // MOV word ptr ES:[BX + SI + -0x4],AX
case 0x31970c63:{AX=unchecked((ushort)(M16(DS,(0x9ed6))));IP=0xc66;return true;} // MOV AX,[0x9ed6]
case 0x31970c66:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9e7e)))));IP=0xc6a;return true;} // ADD AX,word ptr [0x9e7e]
case 0x31970c6a:{M16(ES,(BX + SI - 0x2),AX);IP=0xc6e;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970c6e:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc71;return true;} // LES BX,[BP + 0x16]
case 0x31970c71:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xc74;return true;} // MOV AX,[0x9e72]
case 0x31970c74:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xc75;return true;} // DEC AX
case 0x31970c75:{M16(ES,(BX + SI - 0x2),AX);IP=0xc79;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970c79:{IP=(ushort)((true)?0xbb2:0xc7c);return true;} // JMP 0xe000:0bb2
case 0x31970c7c:{M16(DS,(0x9efc),0);IP=0xc82;return true;} // MOV word ptr [0x9efc],0x0
case 0x31970c82:{M16(DS,(0x9e6e),1);IP=0xc88;return true;} // MOV word ptr [0x9e6e],0x1
case 0x31970c88:{int a=(BP + 0x1a);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc8b;return true;} // LES BX,[BP + 0x1a]
case 0x31970c8b:{SI=unchecked((ushort)(M16(ES,(BX))));IP=0xc8e;return true;} // MOV SI,word ptr ES:[BX]
case 0x31970c8e:{DI=unchecked((ushort)(M16(DS,(0x43f4))));IP=0xc92;return true;} // MOV DI,word ptr [0x43f4]
case 0x31970c92:{DI=unchecked((ushort)(Shl(DI,1)));IP=0xc94;return true;} // SHL DI,0x1
case 0x31970c94:{int a=(BP + 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xc97;return true;} // LES BX,[BP + 0x16]
case 0x31970c97:{AX=unchecked((ushort)(SI + 0x1));IP=0xc9a;return true;} // LEA AX,[SI + 0x1]
case 0x31970c9a:{M16(ES,(BX + DI),AX);IP=0xc9d;return true;} // MOV word ptr ES:[BX + DI],AX
case 0x31970c9d:{ES=unchecked((ushort)(M16(DS,(0x25be))));IP=0xca1;return true;} // MOV ES,word ptr [0x25be]
case 0x31970ca1:{AX=unchecked((ushort)(M16(ES,(0x0))));IP=0xca5;return true;} // MOV AX,ES:[0x0]
case 0x31970ca5:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xca6;return true;} // CWD
case 0x31970ca6:{CX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0xcaa;return true;} // MOV CX,word ptr [0x43f4]
case 0x31970caa:{Divide(CX);IP=0xcac;return true;} // IDIV CX
case 0x31970cac:{M16(DS,(0x9ee4),AX);IP=0xcaf;return true;} // MOV [0x9ee4],AX
case 0x31970caf:{Push(M16(SS,(BP + 0x10)));IP=0xcb2;return true;} // PUSH word ptr [BP + 0x10]
case 0x31970cb2:{Push(M16(SS,(BP + 0xe)));IP=0xcb5;return true;} // PUSH word ptr [BP + 0xe]
case 0x31970cb5:{Push(CS);Push(0xcba);CS=0xc6a;IP=0x3d2;return true;} // CALLF 0x0000:ca72
case 0x31970cba:{Push(M16(DS,(0x43f2)));IP=0xcbe;return true;} // PUSH word ptr [0x43f2]
case 0x31970cbe:{Push(M16(DS,(0x43f0)));IP=0xcc2;return true;} // PUSH word ptr [0x43f0]
case 0x31970cc2:{Push(M16(DS,(0x9ee4)));IP=0xcc6;return true;} // PUSH word ptr [0x9ee4]
case 0x31970cc6:{AX=unchecked((ushort)(2360));IP=0xcc9;return true;} // MOV AX,0x938
case 0x31970cc9:{Push(AX);IP=0xcca;return true;} // PUSH AX
case 0x31970cca:{External(0x29bb,0x3);IP=0xccf;return true;} // CALLF 0x2000:9bb3
case 0x31970ccf:{SP=unchecked((ushort)(Add(SP,8)));IP=0xcd2;return true;} // ADD SP,0x8
case 0x31970cd2:{M16(SS,(BP - 0x1e),SI);IP=0xcd5;return true;} // MOV word ptr [BP + -0x1e],SI
case 0x31970cd5:{M16(DS,(0x9e72),1);IP=0xcdb;return true;} // MOV word ptr [0x9e72],0x1
case 0x31970cdb:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xcde;return true;} // MOV AX,[0x9e72]
case 0x31970cde:{Sub(M16(SS,(BP - 0x1e)),AX);IP=0xce1;return true;} // CMP word ptr [BP + -0x1e],AX
case 0x31970ce1:{IP=(ushort)((SF!=OF)?0xd21:0xce3);return true;} // JL 0xe000:0d21
case 0x31970ce3:{SI=unchecked((ushort)(AX));IP=0xce5;return true;} // MOV SI,AX
case 0x31970ce5:{SI=unchecked((ushort)(Shl(SI,1)));IP=0xce7;return true;} // SHL SI,0x1
case 0x31970ce7:{int a=(BP + 0x1e);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xcea;return true;} // LES BX,[BP + 0x1e]
case 0x31970cea:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xcee;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970cee:{M16(DS,(0x9e76),AX);IP=0xcf1;return true;} // MOV [0x9e76],AX
case 0x31970cf1:{int a=(BP + 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xcf4;return true;} // LES BX,[BP + 0xa]
case 0x31970cf4:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xcf8;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970cf8:{ES=unchecked((ushort)(M16(DS,(0x25be))));IP=0xcfc;return true;} // MOV ES,word ptr [0x25be]
case 0x31970cfc:{Sub(AX,M16(ES,(0x2)));IP=0xd01;return true;} // CMP AX,word ptr ES:[0x2]
case 0x31970d01:{IP=(ushort)((SF!=OF)?0xd0b:0xd03);return true;} // JL 0xe000:0d0b
case 0x31970d03:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0xd06;return true;} // MOV AX,[0x9e76]
case 0x31970d06:{AX=unchecked((ushort)(Sub(0,AX)));IP=0xd08;return true;} // NEG AX
case 0x31970d08:{M16(DS,(0x9e76),AX);IP=0xd0b;return true;} // MOV [0x9e76],AX
case 0x31970d0b:{BX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xd0f;return true;} // MOV BX,word ptr [0x9e72]
case 0x31970d0f:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xd11;return true;} // SHL BX,0x1
case 0x31970d11:{int a=(BP + 0x6);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xd14;return true;} // LES SI,[BP + 0x6]
case 0x31970d14:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0xd17;return true;} // MOV AX,[0x9e76]
case 0x31970d17:{M16(ES,(BX + SI - 0x2),AX);IP=0xd1b;return true;} // MOV word ptr ES:[BX + SI + -0x2],AX
case 0x31970d1b:{M16(DS,(0x9e72),Add(M16(DS,(0x9e72)),1));IP=0xd1f;return true;} // INC word ptr [0x9e72]
case 0x31970d1f:{IP=(ushort)((!OF)?0xcdb:0xd21);return true;} // JNO 0xe000:0cdb
case 0x31970d21:{M16(DS,(0x9efc),0);IP=0xd27;return true;} // MOV word ptr [0x9efc],0x0
case 0x31970d27:{M16(DS,(0x9ece),0);IP=0xd2d;return true;} // MOV word ptr [0x9ece],0x0
case 0x31970d2d:{Sub(M16(DS,(0x3d00)),0);IP=0xd32;return true;} // CMP word ptr [0x3d00],0x0
case 0x31970d32:{IP=(ushort)((ZF)?0xd40:0xd34);return true;} // JZ 0xe000:0d40
case 0x31970d34:{M16(DS,(0x3d00),1);IP=0xd3a;return true;} // MOV word ptr [0x3d00],0x1
case 0x31970d3a:{AX=unchecked((ushort)(M16(DS,(0x3d02))));IP=0xd3d;return true;} // MOV AX,[0x3d02]
case 0x31970d3d:{M16(DS,(0x41c2),AX);IP=0xd40;return true;} // MOV [0x41c2],AX
case 0x31970d40:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0xd43;return true;} // MOV AX,[0x43f4]
case 0x31970d43:{M16(SS,(BP - 0x1a),AX);IP=0xd46;return true;} // MOV word ptr [BP + -0x1a],AX
case 0x31970d46:{M16(DS,(0x9e72),1);IP=0xd4c;return true;} // MOV word ptr [0x9e72],0x1
case 0x31970d4c:{IP=(ushort)((true)?0xf89:0xd4f);return true;} // JMP 0xe000:0f89
case 0x31970d4f:{AX=unchecked((ushort)(M16(DS,(0x9eea))));IP=0xd52;return true;} // MOV AX,[0x9eea]
case 0x31970d52:{Sub(M16(SS,(BP - 0x2a)),AX);IP=0xd55;return true;} // CMP word ptr [BP + -0x2a],AX
case 0x31970d55:{IP=(ushort)((SF==OF)?0xd5a:0xd57);return true;} // JGE 0xe000:0d5a
case 0x31970d57:{IP=(ushort)((true)?0xe5a:0xd5a);return true;} // JMP 0xe000:0e5a
case 0x31970d5a:{BX=unchecked((ushort)(AX));IP=0xd5c;return true;} // MOV BX,AX
case 0x31970d5c:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xd5e;return true;} // SHL BX,0x1
case 0x31970d5e:{int a=(BP + 0x6);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xd61;return true;} // LES SI,[BP + 0x6]
case 0x31970d61:{AX=unchecked((ushort)(M16(ES,(BX + SI - 0x2))));IP=0xd65;return true;} // MOV AX,word ptr ES:[BX + SI + -0x2]
case 0x31970d65:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xd66;return true;} // CWD
case 0x31970d66:{AX=unchecked((ushort)(AX^DX));ZF=AX==0;SF=(AX&32768)!=0;OF=false;IP=0xd68;return true;} // XOR AX,DX
case 0x31970d68:{AX=unchecked((ushort)(Sub(AX,DX)));IP=0xd6a;return true;} // SUB AX,DX
case 0x31970d6a:{M16(DS,(0x9ec8),AX);IP=0xd6d;return true;} // MOV [0x9ec8],AX
case 0x31970d6d:{IP=0xd6e;return true;} // NOP
case 0x31970d6e:{fp.Push((short)M16(DS,(0x9ec8)));IP=0xd72;return true;} // FILD word ptr [0x9ec8]
case 0x31970d72:{IP=0xd73;return true;} // NOP
case 0x31970d73:{M32(DS,(0x9e66),fp.Peek());IP=0xd77;return true;} // FST float ptr [0x9e66]
case 0x31970d77:{IP=0xd78;return true;} // NOP
case 0x31970d78:{fp.Push(fp.Pop()/M32(DS,(0x9e62)));IP=0xd7c;return true;} // FDIV float ptr [0x9e62]
case 0x31970d7c:{IP=0xd7d;return true;} // NOP
case 0x31970d7d:{fp.Push(fp.Pop()+M32(DS,(0x25c6)));IP=0xd81;return true;} // FADD float ptr [0x25c6]
case 0x31970d81:{External(0x23b3,0xc50);IP=0xd86;return true;} // CALLF 0x2000:4780
case 0x31970d86:{M16(DS,(0x9ef4),AX);IP=0xd89;return true;} // MOV [0x9ef4],AX
case 0x31970d89:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xd8a;return true;} // DEC AX
case 0x31970d8a:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xd8b;return true;} // CWD
case 0x31970d8b:{AX=unchecked((ushort)(Sub(AX,DX)));IP=0xd8d;return true;} // SUB AX,DX
case 0x31970d8d:{AX=unchecked((ushort)(Sar(AX,1)));IP=0xd8f;return true;} // SAR AX,0x1
case 0x31970d8f:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9ef0)))));IP=0xd93;return true;} // ADD AX,word ptr [0x9ef0]
case 0x31970d93:{M16(DS,(0x9eca),AX);IP=0xd96;return true;} // MOV [0x9eca],AX
case 0x31970d96:{AX=unchecked((ushort)(M16(DS,(0x9ef4))));IP=0xd99;return true;} // MOV AX,[0x9ef4]
case 0x31970d99:{M16(DS,(0x9ef0),Add(M16(DS,(0x9ef0)),AX));IP=0xd9d;return true;} // ADD word ptr [0x9ef0],AX
case 0x31970d9d:{Sub(M16(DS,(0x9ef0)),69);IP=0xda2;return true;} // CMP word ptr [0x9ef0],0x45
case 0x31970da2:{IP=(ushort)((ZF||SF!=OF)?0xdaa:0xda4);return true;} // JLE 0xe000:0daa
case 0x31970da4:{M16(DS,(0x9ef0),69);IP=0xdaa;return true;} // MOV word ptr [0x9ef0],0x45
case 0x31970daa:{Sub(M16(DS,(0x9ec8)),100);IP=0xdaf;return true;} // CMP word ptr [0x9ec8],0x64
case 0x31970daf:{IP=(ushort)((SF!=OF)?0xddf:0xdb1);return true;} // JL 0xe000:0ddf
case 0x31970db1:{Sub(M16(DS,(0x9ec8)),10);IP=0xdb6;return true;} // CMP word ptr [0x9ec8],0xa
case 0x31970db6:{IP=(ushort)((SF!=OF)?0xe06:0xdb8);return true;} // JL 0xe000:0e06
case 0x31970db8:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0xdbb;return true;} // MOV AX,[0x9ec8]
case 0x31970dbb:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xdbc;return true;} // CWD
case 0x31970dbc:{CX=unchecked((ushort)(100));IP=0xdbf;return true;} // MOV CX,0x64
case 0x31970dbf:{Divide(CX);IP=0xdc1;return true;} // IDIV CX
case 0x31970dc1:{M16(DS,(0x9ef4),AX);IP=0xdc4;return true;} // MOV [0x9ef4],AX
case 0x31970dc4:{BX=unchecked((ushort)(M16(DS,(0x9eca))));IP=0xdc8;return true;} // MOV BX,word ptr [0x9eca]
case 0x31970dc8:{AL=unchecked((ushort)(M16(DS,(0x9ef4))));IP=0xdcb;return true;} // MOV AL,[0x9ef4]
case 0x31970dcb:{AL=unchecked((ushort)(Add(AL,48)));IP=0xdcd;return true;} // ADD AL,0x30
case 0x31970dcd:{M8(DS,(BX + 0x9e81),AL);IP=0xdd1;return true;} // MOV byte ptr [BX + 0x9e81],AL
case 0x31970dd1:{M16(DS,(0x9eca),Add(M16(DS,(0x9eca)),1));IP=0xdd5;return true;} // INC word ptr [0x9eca]
case 0x31970dd5:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0xdd8;return true;} // MOV AX,[0x9ec8]
case 0x31970dd8:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xdd9;return true;} // CWD
case 0x31970dd9:{Divide(CX);IP=0xddb;return true;} // IDIV CX
case 0x31970ddb:{M16(DS,(0x9ec8),DX);IP=0xddf;return true;} // MOV word ptr [0x9ec8],DX
case 0x31970ddf:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0xde2;return true;} // MOV AX,[0x9ec8]
case 0x31970de2:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xde3;return true;} // CWD
case 0x31970de3:{CX=unchecked((ushort)(10));IP=0xde6;return true;} // MOV CX,0xa
case 0x31970de6:{Divide(CX);IP=0xde8;return true;} // IDIV CX
case 0x31970de8:{M16(DS,(0x9ef4),AX);IP=0xdeb;return true;} // MOV [0x9ef4],AX
case 0x31970deb:{BX=unchecked((ushort)(M16(DS,(0x9eca))));IP=0xdef;return true;} // MOV BX,word ptr [0x9eca]
case 0x31970def:{AL=unchecked((ushort)(M16(DS,(0x9ef4))));IP=0xdf2;return true;} // MOV AL,[0x9ef4]
case 0x31970df2:{AL=unchecked((ushort)(Add(AL,48)));IP=0xdf4;return true;} // ADD AL,0x30
case 0x31970df4:{M8(DS,(BX + 0x9e81),AL);IP=0xdf8;return true;} // MOV byte ptr [BX + 0x9e81],AL
case 0x31970df8:{M16(DS,(0x9eca),Add(M16(DS,(0x9eca)),1));IP=0xdfc;return true;} // INC word ptr [0x9eca]
case 0x31970dfc:{AX=unchecked((ushort)(M16(DS,(0x9ec8))));IP=0xdff;return true;} // MOV AX,[0x9ec8]
case 0x31970dff:{DX=(ushort)(((short)AX)<0?65535:0);IP=0xe00;return true;} // CWD
case 0x31970e00:{Divide(CX);IP=0xe02;return true;} // IDIV CX
case 0x31970e02:{M16(DS,(0x9ef4),DX);IP=0xe06;return true;} // MOV word ptr [0x9ef4],DX
case 0x31970e06:{BX=unchecked((ushort)(M16(DS,(0x9eca))));IP=0xe0a;return true;} // MOV BX,word ptr [0x9eca]
case 0x31970e0a:{AL=unchecked((ushort)(M16(DS,(0x9ef4))));IP=0xe0d;return true;} // MOV AL,[0x9ef4]
case 0x31970e0d:{AL=unchecked((ushort)(Add(AL,48)));IP=0xe0f;return true;} // ADD AL,0x30
case 0x31970e0f:{M8(DS,(BX + 0x9e81),AL);IP=0xe13;return true;} // MOV byte ptr [BX + 0x9e81],AL
case 0x31970e13:{M16(DS,(0x9eca),197);IP=0xe19;return true;} // MOV word ptr [0x9eca],0xc5
case 0x31970e19:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0xe1c;return true;} // MOV AX,[0x9e76]
case 0x31970e1c:{Sub(M16(DS,(0x9eea)),AX);IP=0xe20;return true;} // CMP word ptr [0x9eea],AX
case 0x31970e20:{IP=(ushort)((!ZF)?0xe28:0xe22);return true;} // JNZ 0xe000:0e28
case 0x31970e22:{M16(DS,(0x9eca),180);IP=0xe28;return true;} // MOV word ptr [0x9eca],0xb4
case 0x31970e28:{BX=unchecked((ushort)(M16(DS,(0x9ef0))));IP=0xe2c;return true;} // MOV BX,word ptr [0x9ef0]
case 0x31970e2c:{AL=unchecked((ushort)(M16(DS,(0x9eca))));IP=0xe2f;return true;} // MOV AL,[0x9eca]
case 0x31970e2f:{M8(DS,(BX + 0x9e81),AL);IP=0xe33;return true;} // MOV byte ptr [BX + 0x9e81],AL
case 0x31970e33:{BX=unchecked((ushort)(M16(DS,(0x9eea))));IP=0xe37;return true;} // MOV BX,word ptr [0x9eea]
case 0x31970e37:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xe39;return true;} // SHL BX,0x1
case 0x31970e39:{int a=(BP + 0x6);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xe3c;return true;} // LES SI,[BP + 0x6]
case 0x31970e3c:{Sub(M16(ES,(BX + SI - 0x2)),0);IP=0xe41;return true;} // CMP word ptr ES:[BX + SI + -0x2],0x0
case 0x31970e41:{IP=(ushort)((SF==OF)?0xe51:0xe43);return true;} // JGE 0xe000:0e51
case 0x31970e43:{BX=unchecked((ushort)(M16(DS,(0x9ef0))));IP=0xe47;return true;} // MOV BX,word ptr [0x9ef0]
case 0x31970e47:{DI=unchecked((ushort)(BX + 0x9e81));IP=0xe4b;return true;} // LEA DI,[BX + 0x9e81]
case 0x31970e4b:{SI=unchecked((ushort)(2299));IP=0xe4e;return true;} // MOV SI,0x8fb
case 0x31970e4e:{Push(DS);IP=0xe4f;return true;} // PUSH DS
case 0x31970e4f:{ES=unchecked((ushort)(Pop()));IP=0xe50;return true;} // POP ES
case 0x31970e50:{Move(1);IP=0xe51;return true;} // MOVSB ES:DI,SI
case 0x31970e51:{M16(DS,(0x9eea),Add(M16(DS,(0x9eea)),1));IP=0xe55;return true;} // INC word ptr [0x9eea]
case 0x31970e55:{IP=(ushort)((OF)?0xe5a:0xe57);return true;} // JO 0xe000:0e5a
case 0x31970e57:{IP=(ushort)((true)?0xd4f:0xe5a);return true;} // JMP 0xe000:0d4f
case 0x31970e5a:{Sub(M16(DS,(0x9ef0)),69);IP=0xe5f;return true;} // CMP word ptr [0x9ef0],0x45
case 0x31970e5f:{IP=(ushort)((SF==OF)?0xe85:0xe61);return true;} // JGE 0xe000:0e85
case 0x31970e61:{AX=unchecked((ushort)(M16(DS,(0x9ef0))));IP=0xe64;return true;} // MOV AX,[0x9ef0]
case 0x31970e64:{AX=unchecked((ushort)(Add(AX,1)));IP=0xe65;return true;} // INC AX
case 0x31970e65:{M16(DS,(0x9eca),AX);IP=0xe68;return true;} // MOV [0x9eca],AX
case 0x31970e68:{Sub(AX,69);IP=0xe6b;return true;} // CMP AX,0x45
case 0x31970e6b:{IP=(ushort)((!ZF&&SF==OF)?0xe85:0xe6d);return true;} // JG 0xe000:0e85
case 0x31970e6d:{SI=unchecked((ushort)(AX));IP=0xe6f;return true;} // MOV SI,AX
case 0x31970e6f:{Push(SI);IP=0xe70;return true;} // PUSH SI
case 0x31970e70:{DI=unchecked((ushort)(SI + 0x9e81));IP=0xe74;return true;} // LEA DI,[SI + 0x9e81]
case 0x31970e74:{SI=unchecked((ushort)(2298));IP=0xe77;return true;} // MOV SI,0x8fa
case 0x31970e77:{Push(DS);IP=0xe78;return true;} // PUSH DS
case 0x31970e78:{ES=unchecked((ushort)(Pop()));IP=0xe79;return true;} // POP ES
case 0x31970e79:{Move(1);IP=0xe7a;return true;} // MOVSB ES:DI,SI
case 0x31970e7a:{SI=unchecked((ushort)(Pop()));IP=0xe7b;return true;} // POP SI
case 0x31970e7b:{SI=unchecked((ushort)(Add(SI,1)));IP=0xe7c;return true;} // INC SI
case 0x31970e7c:{Sub(SI,69);IP=0xe7f;return true;} // CMP SI,0x45
case 0x31970e7f:{IP=(ushort)((ZF||SF!=OF)?0xe6f:0xe81);return true;} // JLE 0xe000:0e6f
case 0x31970e81:{M16(DS,(0x9eca),SI);IP=0xe85;return true;} // MOV word ptr [0x9eca],SI
case 0x31970e85:{DI=unchecked((ushort)(2347));IP=0xe88;return true;} // MOV DI,0x92b
case 0x31970e88:{SI=unchecked((ushort)(2294));IP=0xe8b;return true;} // MOV SI,0x8f6
case 0x31970e8b:{Push(DS);IP=0xe8c;return true;} // PUSH DS
case 0x31970e8c:{ES=unchecked((ushort)(Pop()));IP=0xe8d;return true;} // POP ES
case 0x31970e8d:{Move(2);IP=0xe8e;return true;} // MOVSW ES:DI,SI
case 0x31970e8e:{Move(2);IP=0xe8f;return true;} // MOVSW ES:DI,SI
case 0x31970e8f:{Sub(M16(DS,(0x9ed6)),10);IP=0xe94;return true;} // CMP word ptr [0x9ed6],0xa
case 0x31970e94:{IP=(ushort)((SF!=OF)?0xe9e:0xe96);return true;} // JL 0xe000:0e9e
case 0x31970e96:{DI=unchecked((ushort)(2347));IP=0xe99;return true;} // MOV DI,0x92b
case 0x31970e99:{SI=unchecked((ushort)(2290));IP=0xe9c;return true;} // MOV SI,0x8f2
case 0x31970e9c:{Move(2);IP=0xe9d;return true;} // MOVSW ES:DI,SI
case 0x31970e9d:{Move(2);IP=0xe9e;return true;} // MOVSW ES:DI,SI
case 0x31970e9e:{Sub(M16(DS,(0x9ed6)),100);IP=0xea3;return true;} // CMP word ptr [0x9ed6],0x64
case 0x31970ea3:{IP=(ushort)((SF!=OF)?0xeaf:0xea5);return true;} // JL 0xe000:0eaf
case 0x31970ea5:{DI=unchecked((ushort)(2347));IP=0xea8;return true;} // MOV DI,0x92b
case 0x31970ea8:{SI=unchecked((ushort)(2286));IP=0xeab;return true;} // MOV SI,0x8ee
case 0x31970eab:{Push(DS);IP=0xeac;return true;} // PUSH DS
case 0x31970eac:{ES=unchecked((ushort)(Pop()));IP=0xead;return true;} // POP ES
case 0x31970ead:{Move(2);IP=0xeae;return true;} // MOVSW ES:DI,SI
case 0x31970eae:{Move(2);IP=0xeaf;return true;} // MOVSW ES:DI,SI
case 0x31970eaf:{BX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xeb3;return true;} // MOV BX,word ptr [0x9e72]
case 0x31970eb3:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xeb5;return true;} // SHL BX,0x1
case 0x31970eb5:{int a=(BP + 0xe);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xeb8;return true;} // LES SI,[BP + 0xe]
case 0x31970eb8:{Push(M16(ES,(BX + SI - 0x2)));IP=0xebc;return true;} // PUSH word ptr ES:[BX + SI + -0x2]
case 0x31970ebc:{Push(M16(DS,(0x9ed6)));IP=0xec0;return true;} // PUSH word ptr [0x9ed6]
case 0x31970ec0:{AX=unchecked((ushort)(2336));IP=0xec3;return true;} // MOV AX,0x920
case 0x31970ec3:{Push(DS);IP=0xec4;return true;} // PUSH DS
case 0x31970ec4:{Push(AX);IP=0xec5;return true;} // PUSH AX
case 0x31970ec5:{AX=unchecked((ushort)(24));IP=0xec8;return true;} // MOV AX,0x18
case 0x31970ec8:{Push(AX);IP=0xec9;return true;} // PUSH AX
case 0x31970ec9:{External(0x29bb,0x2b66);IP=0xece;return true;} // CALLF 0x2000:c716
case 0x31970ece:{SP=unchecked((ushort)(Add(SP,6)));IP=0xed1;return true;} // ADD SP,0x6
case 0x31970ed1:{Push(AX);IP=0xed2;return true;} // PUSH AX
case 0x31970ed2:{AX=unchecked((ushort)(2370));IP=0xed5;return true;} // MOV AX,0x942
case 0x31970ed5:{Push(AX);IP=0xed6;return true;} // PUSH AX
case 0x31970ed6:{External(0x29bb,0x3);IP=0xedb;return true;} // CALLF 0x2000:9bb3
case 0x31970edb:{SP=unchecked((ushort)(Add(SP,8)));IP=0xede;return true;} // ADD SP,0x8
case 0x31970ede:{AX=unchecked((ushort)(M16(DS,(0x9ece))));IP=0xee1;return true;} // MOV AX,[0x9ece]
case 0x31970ee1:{Sub(M16(DS,(0x41c2)),AX);IP=0xee5;return true;} // CMP word ptr [0x41c2],AX
case 0x31970ee5:{IP=(ushort)((!ZF&&SF==OF)?0xf5e:0xee7);return true;} // JG 0xe000:0f5e
case 0x31970ee7:{AX=unchecked((ushort)(M16(DS,(0x41c2))));IP=0xeea;return true;} // MOV AX,[0x41c2]
case 0x31970eea:{M16(DS,(0x9ef0),AX);IP=0xeed;return true;} // MOV [0x9ef0],AX
case 0x31970eed:{Sub(M16(DS,(0x3d00)),0);IP=0xef2;return true;} // CMP word ptr [0x3d00],0x0
case 0x31970ef2:{IP=(ushort)((ZF)?0xf05:0xef4);return true;} // JZ 0xe000:0f05
case 0x31970ef4:{M16(DS,(0x3d00),Add(M16(DS,(0x3d00)),1));IP=0xef8;return true;} // INC word ptr [0x3d00]
case 0x31970ef8:{BX=unchecked((ushort)(M16(DS,(0x3d00))));IP=0xefc;return true;} // MOV BX,word ptr [0x3d00]
case 0x31970efc:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xefe;return true;} // SHL BX,0x1
case 0x31970efe:{AX=unchecked((ushort)(M16(DS,(BX + 0x3d00))));IP=0xf02;return true;} // MOV AX,word ptr [BX + 0x3d00]
case 0x31970f02:{M16(DS,(0x41c2),AX);IP=0xf05;return true;} // MOV [0x41c2],AX
case 0x31970f05:{M16(DS,(0x9ece),0);IP=0xf0b;return true;} // MOV word ptr [0x9ece],0x0
case 0x31970f0b:{M16(DS,(0x9efc),Add(M16(DS,(0x9efc)),1));IP=0xf0f;return true;} // INC word ptr [0x9efc]
case 0x31970f0f:{ES=unchecked((ushort)(M16(DS,(0x25c0))));IP=0xf13;return true;} // MOV ES,word ptr [0x25c0]
case 0x31970f13:{BX=unchecked((ushort)(M16(ES,(0x0))));IP=0xf18;return true;} // MOV BX,word ptr ES:[0x0]
case 0x31970f18:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xf1a;return true;} // SHL BX,0x1
case 0x31970f1a:{BX=unchecked((ushort)(BX + 0x2));IP=0xf1e;return true;} // LEA BX,[BX + 0x2]
case 0x31970f1e:{M16(ES,(BX - 0x2),0);IP=0xf24;return true;} // MOV word ptr ES:[BX + -0x2],0x0
case 0x31970f24:{M16(ES,(0x0),Add(M16(ES,(0x0)),1));IP=0xf29;return true;} // INC word ptr ES:[0x0]
case 0x31970f29:{BX=unchecked((ushort)(M16(ES,(0x0))));IP=0xf2e;return true;} // MOV BX,word ptr ES:[0x0]
case 0x31970f2e:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xf30;return true;} // SHL BX,0x1
case 0x31970f30:{BX=unchecked((ushort)(BX + 0x2));IP=0xf34;return true;} // LEA BX,[BX + 0x2]
case 0x31970f34:{M16(ES,(BX - 0x2),0);IP=0xf3a;return true;} // MOV word ptr ES:[BX + -0x2],0x0
case 0x31970f3a:{Push(M16(DS,(0x9ef0)));IP=0xf3e;return true;} // PUSH word ptr [0x9ef0]
case 0x31970f3e:{Push(M16(DS,(0x41c4)));IP=0xf42;return true;} // PUSH word ptr [0x41c4]
case 0x31970f42:{AX=unchecked((ushort)(2384));IP=0xf45;return true;} // MOV AX,0x950
case 0x31970f45:{Push(AX);IP=0xf46;return true;} // PUSH AX
case 0x31970f46:{External(0x29bb,0x3);IP=0xf4b;return true;} // CALLF 0x2000:9bb3
case 0x31970f4b:{SP=unchecked((ushort)(Add(SP,6)));IP=0xf4e;return true;} // ADD SP,0x6
case 0x31970f4e:{M16(DS,(0x41c4),Add(M16(DS,(0x41c4)),1));IP=0xf52;return true;} // INC word ptr [0x41c4]
case 0x31970f52:{AX=unchecked((ushort)(M16(DS,(0x41c2))));IP=0xf55;return true;} // MOV AX,[0x41c2]
case 0x31970f55:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9efc)))));IP=0xf59;return true;} // ADD AX,word ptr [0x9efc]
case 0x31970f59:{Sub(AX,11);IP=0xf5c;return true;} // CMP AX,0xb
case 0x31970f5c:{IP=(ushort)((!ZF&&SF==OF)?0xf65:0xf5e);return true;} // JG 0xe000:0f65
case 0x31970f5e:{Sub(M16(DS,(0x9efc)),11);IP=0xf63;return true;} // CMP word ptr [0x9efc],0xb
case 0x31970f63:{IP=(ushort)((SF!=OF)?0xf79:0xf65);return true;} // JL 0xe000:0f79
case 0x31970f65:{M16(DS,(0x9efc),0);IP=0xf6b;return true;} // MOV word ptr [0x9efc],0x0
case 0x31970f6b:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xf6e;return true;} // MOV AX,[0x9e72]
case 0x31970f6e:{Sub(M16(DS,(0x43f4)),AX);IP=0xf72;return true;} // CMP word ptr [0x43f4],AX
case 0x31970f72:{IP=(ushort)((ZF)?0xf79:0xf74);return true;} // JZ 0xe000:0f79
case 0x31970f74:{External(0xff5,0x1377);IP=0xf79;return true;} // CALLF 0x1000:12c7
case 0x31970f79:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0xf7c;return true;} // MOV AX,[0x9e76]
case 0x31970f7c:{AX=unchecked((ushort)(Add(AX,1)));IP=0xf7d;return true;} // INC AX
case 0x31970f7d:{M16(DS,(0x9e6e),AX);IP=0xf80;return true;} // MOV [0x9e6e],AX
case 0x31970f80:{M16(DS,(0x9e72),Add(M16(DS,(0x9e72)),1));IP=0xf84;return true;} // INC word ptr [0x9e72]
case 0x31970f84:{IP=(ushort)((!OF)?0xf89:0xf86);return true;} // JNO 0xe000:0f89
case 0x31970f86:{IP=(ushort)((true)?0x1043:0xf89);return true;} // JMP 0xe000:1043
case 0x31970f89:{AX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xf8c;return true;} // MOV AX,[0x9e72]
case 0x31970f8c:{Sub(M16(SS,(BP - 0x1a)),AX);IP=0xf8f;return true;} // CMP word ptr [BP + -0x1a],AX
case 0x31970f8f:{IP=(ushort)((SF==OF)?0xf94:0xf91);return true;} // JGE 0xe000:0f94
case 0x31970f91:{IP=(ushort)((true)?0x1043:0xf94);return true;} // JMP 0xe000:1043
case 0x31970f94:{M16(DS,(0x9ece),Add(M16(DS,(0x9ece)),1));IP=0xf98;return true;} // INC word ptr [0x9ece]
case 0x31970f98:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0xf9b;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x31970f9b:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0xf9e;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x31970f9e:{CX=unchecked((ushort)(M16(DS,(0x9e72))));IP=0xfa2;return true;} // MOV CX,word ptr [0x9e72]
case 0x31970fa2:{CX=unchecked((ushort)(Shl(CX,1)));IP=0xfa4;return true;} // SHL CX,0x1
case 0x31970fa4:{AX=unchecked((ushort)(Add(AX,CX)));IP=0xfa6;return true;} // ADD AX,CX
case 0x31970fa6:{M16(SS,(BP - 0x28),AX);IP=0xfa9;return true;} // MOV word ptr [BP + -0x28],AX
case 0x31970fa9:{M16(SS,(BP - 0x26),DX);IP=0xfac;return true;} // MOV word ptr [BP + -0x26],DX
case 0x31970fac:{int a=(BP - 0x28);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0xfaf;return true;} // LES BX,[BP + -0x28]
case 0x31970faf:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0xfb2;return true;} // MOV AX,word ptr ES:[BX]
case 0x31970fb2:{AX=unchecked((ushort)(Sub(AX,M16(ES,(BX - 0x2)))));IP=0xfb6;return true;} // SUB AX,word ptr ES:[BX + -0x2]
case 0x31970fb6:{M16(DS,(0x9e76),AX);IP=0xfb9;return true;} // MOV [0x9e76],AX
case 0x31970fb9:{M16(ES,(BX - 0x2),AX);IP=0xfbd;return true;} // MOV word ptr ES:[BX + -0x2],AX
case 0x31970fbd:{ES=unchecked((ushort)(M16(DS,(0x25c0))));IP=0xfc1;return true;} // MOV ES,word ptr [0x25c0]
case 0x31970fc1:{BX=unchecked((ushort)(M16(ES,(0x0))));IP=0xfc6;return true;} // MOV BX,word ptr ES:[0x0]
case 0x31970fc6:{BX=unchecked((ushort)(Shl(BX,1)));IP=0xfc8;return true;} // SHL BX,0x1
case 0x31970fc8:{BX=unchecked((ushort)(BX + 0x2));IP=0xfcc;return true;} // LEA BX,[BX + 0x2]
case 0x31970fcc:{M16(ES,(BX - 0x2),AX);IP=0xfd0;return true;} // MOV word ptr ES:[BX + -0x2],AX
case 0x31970fd0:{M16(ES,(0x0),Add(M16(ES,(0x0)),1));IP=0xfd5;return true;} // INC word ptr ES:[0x0]
case 0x31970fd5:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9e6e)))));IP=0xfd9;return true;} // ADD AX,word ptr [0x9e6e]
case 0x31970fd9:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xfda;return true;} // DEC AX
case 0x31970fda:{M16(DS,(0x9e76),AX);IP=0xfdd;return true;} // MOV [0x9e76],AX
case 0x31970fdd:{ES=unchecked((ushort)(M16(DS,(0x25be))));IP=0xfe1;return true;} // MOV ES,word ptr [0x25be]
case 0x31970fe1:{AX=unchecked((ushort)(M16(ES,(0x4))));IP=0xfe5;return true;} // MOV AX,ES:[0x4]
case 0x31970fe5:{AX=unchecked((ushort)(Add(AX,M16(DS,(0x9e6e)))));IP=0xfe9;return true;} // ADD AX,word ptr [0x9e6e]
case 0x31970fe9:{AX=unchecked((ushort)(Sub(AX,1)));IP=0xfea;return true;} // DEC AX
case 0x31970fea:{M16(DS,(0x9ed6),AX);IP=0xfed;return true;} // MOV [0x9ed6],AX
case 0x31970fed:{M16(DS,(0x9efc),Add(M16(DS,(0x9efc)),1));IP=0xff1;return true;} // INC word ptr [0x9efc]
case 0x31970ff1:{IP=0xff2;return true;} // NOP
case 0x31970ff2:{fp.Push((short)M16(DS,(0x43f2)));IP=0xff6;return true;} // FILD word ptr [0x43f2]
case 0x31970ff6:{IP=0xff7;return true;} // NOP
case 0x31970ff7:{fp.Push(fp.Pop()/M32(DS,(0x25c2)));IP=0xffb;return true;} // FDIV float ptr [0x25c2]
case 0x31970ffb:{IP=0xffc;return true;} // NOP
case 0x31970ffc:{M32(DS,(0x9e62),fp.Peek());fp.Pop();IP=0x1000;return true;} // FSTP float ptr [0x9e62]
case 0x31971000:{IP=0x1001;return true;} // NOP
case 0x31971001:{IP=0x1002;return true;} // WAIT
case 0x31971002:{M16(DS,(0x9eea),2);IP=0x1008;return true;} // MOV word ptr [0x9eea],0x2
case 0x31971008:{BX=unchecked((ushort)(M16(DS,(0x9eea))));IP=0x100c;return true;} // MOV BX,word ptr [0x9eea]
case 0x3197100c:{DI=unchecked((ushort)(BX + 0x9e81));IP=0x1010;return true;} // LEA DI,[BX + 0x9e81]
case 0x31971010:{SI=unchecked((ushort)(2301));IP=0x1013;return true;} // MOV SI,0x8fd
case 0x31971013:{Push(DS);IP=0x1014;return true;} // PUSH DS
case 0x31971014:{ES=unchecked((ushort)(Pop()));IP=0x1015;return true;} // POP ES
case 0x31971015:{Move(1);IP=0x1016;return true;} // MOVSB ES:DI,SI
case 0x31971016:{M16(DS,(0x9eea),Add(M16(DS,(0x9eea)),1));IP=0x101a;return true;} // INC word ptr [0x9eea]
case 0x3197101a:{Sub(M16(DS,(0x9eea)),69);IP=0x101f;return true;} // CMP word ptr [0x9eea],0x45
case 0x3197101f:{IP=(ushort)((ZF||SF!=OF)?0x1008:0x1021);return true;} // JLE 0xe000:1008
case 0x31971021:{DI=unchecked((ushort)(40578));IP=0x1024;return true;} // MOV DI,0x9e82
case 0x31971024:{SI=unchecked((ushort)(2300));IP=0x1027;return true;} // MOV SI,0x8fc
case 0x31971027:{Move(1);IP=0x1028;return true;} // MOVSB ES:DI,SI
case 0x31971028:{M16(DS,(0x9ef0),1);IP=0x102e;return true;} // MOV word ptr [0x9ef0],0x1
case 0x3197102e:{M16(DS,(0x9eca),1);IP=0x1034;return true;} // MOV word ptr [0x9eca],0x1
case 0x31971034:{AX=unchecked((ushort)(M16(DS,(0x9e76))));IP=0x1037;return true;} // MOV AX,[0x9e76]
case 0x31971037:{M16(SS,(BP - 0x2a),AX);IP=0x103a;return true;} // MOV word ptr [BP + -0x2a],AX
case 0x3197103a:{AX=unchecked((ushort)(M16(DS,(0x9e6e))));IP=0x103d;return true;} // MOV AX,[0x9e6e]
case 0x3197103d:{M16(DS,(0x9eea),AX);IP=0x1040;return true;} // MOV [0x9eea],AX
case 0x31971040:{IP=(ushort)((true)?0xd4f:0x1043);return true;} // JMP 0xe000:0d4f
case 0x31971043:{AX=unchecked((ushort)(M16(SS,(BP + 0x16))));IP=0x1046;return true;} // MOV AX,word ptr [BP + 0x16]
case 0x31971046:{DX=unchecked((ushort)(M16(SS,(BP + 0x18))));IP=0x1049;return true;} // MOV DX,word ptr [BP + 0x18]
case 0x31971049:{CX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x104d;return true;} // MOV CX,word ptr [0x43f4]
case 0x3197104d:{CX=unchecked((ushort)(Shl(CX,1)));IP=0x104f;return true;} // SHL CX,0x1
case 0x3197104f:{AX=unchecked((ushort)(Add(AX,CX)));IP=0x1051;return true;} // ADD AX,CX
case 0x31971051:{M16(SS,(BP - 0x28),AX);IP=0x1054;return true;} // MOV word ptr [BP + -0x28],AX
case 0x31971054:{M16(SS,(BP - 0x26),DX);IP=0x1057;return true;} // MOV word ptr [BP + -0x26],DX
case 0x31971057:{int a=(BP - 0x28);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x105a;return true;} // LES BX,[BP + -0x28]
case 0x3197105a:{M16(ES,(BX),0);IP=0x105f;return true;} // MOV word ptr ES:[BX],0x0
case 0x3197105f:{M16(ES,(BX + 0x2),0);IP=0x1065;return true;} // MOV word ptr ES:[BX + 0x2],0x0
case 0x31971065:{M16(DS,(0x47b0),0);IP=0x106b;return true;} // MOV word ptr [0x47b0],0x0
case 0x3197106b:{SI=unchecked((ushort)(Pop()));IP=0x106c;return true;} // POP SI
case 0x3197106c:{DI=unchecked((ushort)(Pop()));IP=0x106d;return true;} // POP DI
case 0x3197106d:{SP=unchecked((ushort)(BP));IP=0x106f;return true;} // MOV SP,BP
case 0x3197106f:{BP=unchecked((ushort)(Pop()));IP=0x1070;return true;} // POP BP
case 0x31971070:{IP=Pop();CS=Pop();SP=(ushort)(SP+28);if(CS!=0x3197&&CS!=0xc6a&&CS!=0xfe0)return false;return true;} // RETF 0x1c
case 0x319713f6:{Push(BP);IP=0x13f7;return true;} // PUSH BP
case 0x319713f7:{BP=unchecked((ushort)(SP));IP=0x13f9;return true;} // MOV BP,SP
case 0x319713f9:{SP=unchecked((ushort)(Sub(SP,24)));IP=0x13fc;return true;} // SUB SP,0x18
case 0x319713fc:{Push(DI);IP=0x13fd;return true;} // PUSH DI
case 0x319713fd:{Push(SI);IP=0x13fe;return true;} // PUSH SI
case 0x319713fe:{M16(DS,(0x9f30),1);IP=0x1404;return true;} // MOV word ptr [0x9f30],0x1
case 0x31971404:{AX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x1407;return true;} // MOV AX,[0x43f4]
case 0x31971407:{AX=unchecked((ushort)(Add(AX,1)));IP=0x1408;return true;} // INC AX
case 0x31971408:{M16(SS,(BP - 0x2),AX);IP=0x140b;return true;} // MOV word ptr [BP + -0x2],AX
case 0x3197140b:{M16(DS,(0x9f32),2);IP=0x1411;return true;} // MOV word ptr [0x9f32],0x2
case 0x31971411:{Sub(AX,2);IP=0x1414;return true;} // CMP AX,0x2
case 0x31971414:{IP=(ushort)((SF==OF)?0x1419:0x1416);return true;} // JGE 0xe000:1419
case 0x31971416:{IP=(ushort)((true)?0x14be:0x1419);return true;} // JMP 0xe000:14be
case 0x31971419:{DI=unchecked((ushort)(4));IP=0x141c;return true;} // MOV DI,0x4
case 0x3197141c:{AX=unchecked((ushort)(M16(SS,(BP + 0x6))));IP=0x141f;return true;} // MOV AX,word ptr [BP + 0x6]
case 0x3197141f:{DX=unchecked((ushort)(M16(SS,(BP + 0x8))));IP=0x1422;return true;} // MOV DX,word ptr [BP + 0x8]
case 0x31971422:{M16(SS,(BP - 0x12),AX);IP=0x1425;return true;} // MOV word ptr [BP + -0x12],AX
case 0x31971425:{M16(SS,(BP - 0x10),DX);IP=0x1428;return true;} // MOV word ptr [BP + -0x10],DX
case 0x31971428:{AX=unchecked((ushort)(M16(SS,(BP + 0xa))));IP=0x142b;return true;} // MOV AX,word ptr [BP + 0xa]
case 0x3197142b:{DX=unchecked((ushort)(M16(SS,(BP + 0xc))));IP=0x142e;return true;} // MOV DX,word ptr [BP + 0xc]
case 0x3197142e:{AX=unchecked((ushort)(Add(AX,2)));IP=0x1431;return true;} // ADD AX,0x2
case 0x31971431:{M16(SS,(BP - 0x16),AX);IP=0x1434;return true;} // MOV word ptr [BP + -0x16],AX
case 0x31971434:{M16(SS,(BP - 0x14),DX);IP=0x1437;return true;} // MOV word ptr [BP + -0x14],DX
case 0x31971437:{AX=unchecked((ushort)(M16(SS,(BP - 0x2))));IP=0x143a;return true;} // MOV AX,word ptr [BP + -0x2]
case 0x3197143a:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x143b;return true;} // DEC AX
case 0x3197143b:{M16(SS,(BP - 0x18),AX);IP=0x143e;return true;} // MOV word ptr [BP + -0x18],AX
case 0x3197143e:{M16(DS,(0x9f32),Add(M16(DS,(0x9f32)),AX));IP=0x1442;return true;} // ADD word ptr [0x9f32],AX
case 0x31971442:{SI=unchecked((ushort)(M16(DS,(0x9f30))));IP=0x1446;return true;} // MOV SI,word ptr [0x9f30]
case 0x31971446:{int a=(BP + 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x1449;return true;} // LES BX,[BP + 0xa]
case 0x31971449:{AX=unchecked((ushort)(M16(ES,(BX + DI - 0x2))));IP=0x144d;return true;} // MOV AX,word ptr ES:[BX + DI + -0x2]
case 0x3197144d:{M16(SS,(BP - 0xe),AX);IP=0x1450;return true;} // MOV word ptr [BP + -0xe],AX
case 0x31971450:{int a=(BP + 0x12);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x1453;return true;} // LES BX,[BP + 0x12]
case 0x31971453:{M16(ES,(BX + DI - 0x2),AX);IP=0x1457;return true;} // MOV word ptr ES:[BX + DI + -0x2],AX
case 0x31971457:{M16(DS,(0x9f36),0);IP=0x145d;return true;} // MOV word ptr [0x9f36],0x0
case 0x3197145d:{AX=unchecked((ushort)(Sub(AX,1)));IP=0x145e;return true;} // DEC AX
case 0x3197145e:{M16(SS,(BP - 0x4),AX);IP=0x1461;return true;} // MOV word ptr [BP + -0x4],AX
case 0x31971461:{M16(DS,(0x9f34),SI);IP=0x1465;return true;} // MOV word ptr [0x9f34],SI
case 0x31971465:{AX=unchecked((ushort)(SI));IP=0x1467;return true;} // MOV AX,SI
case 0x31971467:{Sub(M16(SS,(BP - 0x4)),AX);IP=0x146a;return true;} // CMP word ptr [BP + -0x4],AX
case 0x3197146a:{IP=(ushort)((SF!=OF)?0x149b:0x146c);return true;} // JL 0xe000:149b
case 0x3197146c:{AX=unchecked((ushort)(M16(SS,(BP + 0xe))));IP=0x146f;return true;} // MOV AX,word ptr [BP + 0xe]
case 0x3197146f:{DX=unchecked((ushort)(M16(SS,(BP + 0x10))));IP=0x1472;return true;} // MOV DX,word ptr [BP + 0x10]
case 0x31971472:{BX=unchecked((ushort)(SI));IP=0x1474;return true;} // MOV BX,SI
case 0x31971474:{BX=unchecked((ushort)(Shl(BX,1)));IP=0x1476;return true;} // SHL BX,0x1
case 0x31971476:{AX=unchecked((ushort)(Add(AX,BX)));IP=0x1478;return true;} // ADD AX,BX
case 0x31971478:{AX=unchecked((ushort)(Sub(AX,2)));IP=0x147b;return true;} // SUB AX,0x2
case 0x3197147b:{M16(SS,(BP - 0x8),AX);IP=0x147e;return true;} // MOV word ptr [BP + -0x8],AX
case 0x3197147e:{M16(SS,(BP - 0x6),DX);IP=0x1481;return true;} // MOV word ptr [BP + -0x6],DX
case 0x31971481:{CX=unchecked((ushort)(M16(SS,(BP - 0x4))));IP=0x1484;return true;} // MOV CX,word ptr [BP + -0x4]
case 0x31971484:{CX=unchecked((ushort)(Sub(CX,SI)));IP=0x1486;return true;} // SUB CX,SI
case 0x31971486:{CX=unchecked((ushort)(Add(CX,1)));IP=0x1487;return true;} // INC CX
case 0x31971487:{M16(DS,(0x9f34),Add(M16(DS,(0x9f34)),CX));IP=0x148b;return true;} // ADD word ptr [0x9f34],CX
case 0x3197148b:{int a=(BP - 0x8);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x148e;return true;} // LES BX,[BP + -0x8]
case 0x3197148e:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0x1491;return true;} // MOV AX,word ptr ES:[BX]
case 0x31971491:{M16(DS,(0x9f36),Add(M16(DS,(0x9f36)),AX));IP=0x1495;return true;} // ADD word ptr [0x9f36],AX
case 0x31971495:{M16(SS,(BP - 0x8),Add(M16(SS,(BP - 0x8)),2));IP=0x1499;return true;} // ADD word ptr [BP + -0x8],0x2
case 0x31971499:{CX--;IP=(ushort)(CX!=0?0x148b:0x149b);return true;} // LOOP 0xe000:148b
case 0x3197149b:{int a=(BP - 0x12);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x149e;return true;} // LES BX,[BP + -0x12]
case 0x3197149e:{AX=unchecked((ushort)(M16(DS,(0x9f36))));IP=0x14a1;return true;} // MOV AX,[0x9f36]
case 0x319714a1:{M16(ES,(BX),AX);IP=0x14a4;return true;} // MOV word ptr ES:[BX],AX
case 0x319714a4:{int a=(BP - 0x16);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x14a7;return true;} // LES BX,[BP + -0x16]
case 0x319714a7:{SI=unchecked((ushort)(M16(ES,(BX))));IP=0x14aa;return true;} // MOV SI,word ptr ES:[BX]
case 0x319714aa:{DI=unchecked((ushort)(Add(DI,2)));IP=0x14ad;return true;} // ADD DI,0x2
case 0x319714ad:{M16(SS,(BP - 0x12),Add(M16(SS,(BP - 0x12)),2));IP=0x14b1;return true;} // ADD word ptr [BP + -0x12],0x2
case 0x319714b1:{M16(SS,(BP - 0x16),Add(M16(SS,(BP - 0x16)),2));IP=0x14b5;return true;} // ADD word ptr [BP + -0x16],0x2
case 0x319714b5:{M16(SS,(BP - 0x18),Sub(M16(SS,(BP - 0x18)),1));IP=0x14b8;return true;} // DEC word ptr [BP + -0x18]
case 0x319714b8:{IP=(ushort)((!ZF)?0x1446:0x14ba);return true;} // JNZ 0xe000:1446
case 0x319714ba:{M16(DS,(0x9f30),SI);IP=0x14be;return true;} // MOV word ptr [0x9f30],SI
case 0x319714be:{SI=unchecked((ushort)(Pop()));IP=0x14bf;return true;} // POP SI
case 0x319714bf:{DI=unchecked((ushort)(Pop()));IP=0x14c0;return true;} // POP DI
case 0x319714c0:{SP=unchecked((ushort)(BP));IP=0x14c2;return true;} // MOV SP,BP
case 0x319714c2:{BP=unchecked((ushort)(Pop()));IP=0x14c3;return true;} // POP BP
case 0x319714c3:{IP=Pop();CS=Pop();SP=(ushort)(SP+16);if(CS!=0x3197&&CS!=0xc6a&&CS!=0xfe0)return false;return true;} // RETF 0x10
case 0x319714c6:{Push(BP);IP=0x14c7;return true;} // PUSH BP
case 0x319714c7:{BP=unchecked((ushort)(SP));IP=0x14c9;return true;} // MOV BP,SP
case 0x319714c9:{AX=unchecked((ushort)(M16(DS,(0x43f0))));IP=0x14cc;return true;} // MOV AX,[0x43f0]
case 0x319714cc:{M16(DS,(0x9f3a),AX);IP=0x14cf;return true;} // MOV [0x9f3a],AX
case 0x319714cf:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x14d2;return true;} // MOV AX,[0x43f2]
case 0x319714d2:{M16(DS,(0x9f3c),AX);IP=0x14d5;return true;} // MOV [0x9f3c],AX
case 0x319714d5:{Push(M16(SS,(BP + 0x8)));IP=0x14d8;return true;} // PUSH word ptr [BP + 0x8]
case 0x319714d8:{Push(M16(SS,(BP + 0x6)));IP=0x14db;return true;} // PUSH word ptr [BP + 0x6]
case 0x319714db:{Push(CS);Push(0x14e0);CS=0xc6a;IP=0x3d2;return true;} // CALLF 0x0000:ca72
case 0x319714e0:{AX=unchecked((ushort)(M16(DS,(0x43f2))));IP=0x14e3;return true;} // MOV AX,[0x43f2]
case 0x319714e3:{AX=unchecked((ushort)(Sub(AX,M16(DS,(0x43f0)))));IP=0x14e7;return true;} // SUB AX,word ptr [0x43f0]
case 0x319714e7:{M16(DS,(0x9f38),AX);IP=0x14ea;return true;} // MOV [0x9f38],AX
case 0x319714ea:{int a=(BP + 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x14ed;return true;} // LES BX,[BP + 0xa]
case 0x319714ed:{Sub(M16(ES,(BX)),AX);IP=0x14f0;return true;} // CMP word ptr ES:[BX],AX
case 0x319714f0:{IP=(ushort)((SF==OF)?0x1500:0x14f2);return true;} // JGE 0xe000:1500
case 0x319714f2:{AX=unchecked((ushort)(M16(DS,(0x9f3a))));IP=0x14f5;return true;} // MOV AX,[0x9f3a]
case 0x319714f5:{M16(DS,(0x43f0),AX);IP=0x14f8;return true;} // MOV [0x43f0],AX
case 0x319714f8:{AX=unchecked((ushort)(M16(DS,(0x9f3c))));IP=0x14fb;return true;} // MOV AX,[0x9f3c]
case 0x319714fb:{M16(DS,(0x43f2),AX);IP=0x14fe;return true;} // MOV [0x43f2],AX
case 0x319714fe:{IP=(ushort)((true)?0x1509:0x1500);return true;} // JMP 0xe000:1509
case 0x31971500:{int a=(BP + 0xa);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x1503;return true;} // LES BX,[BP + 0xa]
case 0x31971503:{AX=unchecked((ushort)(M16(DS,(0x9f38))));IP=0x1506;return true;} // MOV AX,[0x9f38]
case 0x31971506:{M16(ES,(BX),AX);IP=0x1509;return true;} // MOV word ptr ES:[BX],AX
case 0x31971509:{SP=unchecked((ushort)(BP));IP=0x150b;return true;} // MOV SP,BP
case 0x3197150b:{BP=unchecked((ushort)(Pop()));IP=0x150c;return true;} // POP BP
case 0x3197150c:{IP=Pop();CS=Pop();SP=(ushort)(SP+8);if(CS!=0x3197&&CS!=0xc6a&&CS!=0xfe0)return false;return true;} // RETF 0x8
case 0x0c6a03d2:{Push(BP);IP=0x3d3;return true;} // PUSH BP
case 0x0c6a03d3:{BP=unchecked((ushort)(SP));IP=0x3d5;return true;} // MOV BP,SP
case 0x0c6a03d5:{SP=unchecked((ushort)(Sub(SP,8)));IP=0x3d8;return true;} // SUB SP,0x8
case 0x0c6a03d8:{Push(SI);IP=0x3d9;return true;} // PUSH SI
case 0x0c6a03d9:{int a=(BP + 0x6);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x3dc;return true;} // LES BX,[BP + 0x6]
case 0x0c6a03dc:{AX=unchecked((ushort)(M16(ES,(BX))));IP=0x3df;return true;} // MOV AX,word ptr ES:[BX]
case 0x0c6a03df:{M16(DS,(0x43f0),AX);IP=0x3e2;return true;} // MOV [0x43f0],AX
case 0x0c6a03e2:{M16(DS,(0x43f2),AX);IP=0x3e5;return true;} // MOV [0x43f2],AX
case 0x0c6a03e5:{M16(DS,(0x503e),1);IP=0x3eb;return true;} // MOV word ptr [0x503e],0x1
case 0x0c6a03eb:{Sub(M16(DS,(0x43f4)),1);IP=0x3f0;return true;} // CMP word ptr [0x43f4],0x1
case 0x0c6a03f0:{IP=(ushort)((SF!=OF)?0x42a:0x3f2);return true;} // JL 0xe000:042a
case 0x0c6a03f2:{AX=unchecked((ushort)(BX));IP=0x3f4;return true;} // MOV AX,BX
case 0x0c6a03f4:{DX=unchecked((ushort)(ES));IP=0x3f6;return true;} // MOV DX,ES
case 0x0c6a03f6:{M16(SS,(BP - 0x6),AX);IP=0x3f9;return true;} // MOV word ptr [BP + -0x6],AX
case 0x0c6a03f9:{M16(SS,(BP - 0x4),DX);IP=0x3fc;return true;} // MOV word ptr [BP + -0x4],DX
case 0x0c6a03fc:{CX=unchecked((ushort)(M16(DS,(0x43f4))));IP=0x400;return true;} // MOV CX,word ptr [0x43f4]
case 0x0c6a0400:{AX=unchecked((ushort)(CX));IP=0x402;return true;} // MOV AX,CX
case 0x0c6a0402:{M16(DS,(0x503e),Add(M16(DS,(0x503e)),AX));IP=0x406;return true;} // ADD word ptr [0x503e],AX
case 0x0c6a0406:{int a=(BP - 0x6);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x409;return true;} // LES BX,[BP + -0x6]
case 0x0c6a0409:{SI=unchecked((ushort)(M16(ES,(BX))));IP=0x40c;return true;} // MOV SI,word ptr ES:[BX]
case 0x0c6a040c:{Sub(M16(DS,(0x43f0)),SI);IP=0x410;return true;} // CMP word ptr [0x43f0],SI
case 0x0c6a0410:{IP=(ushort)((ZF||SF!=OF)?0x416:0x412);return true;} // JLE 0xe000:0416
case 0x0c6a0412:{M16(DS,(0x43f0),SI);IP=0x416;return true;} // MOV word ptr [0x43f0],SI
case 0x0c6a0416:{Sub(M16(DS,(0x43f2)),SI);IP=0x41a;return true;} // CMP word ptr [0x43f2],SI
case 0x0c6a041a:{IP=(ushort)((SF==OF)?0x420:0x41c);return true;} // JGE 0xe000:0420
case 0x0c6a041c:{M16(DS,(0x43f2),SI);IP=0x420;return true;} // MOV word ptr [0x43f2],SI
case 0x0c6a0420:{M16(SS,(BP - 0x6),Add(M16(SS,(BP - 0x6)),2));IP=0x424;return true;} // ADD word ptr [BP + -0x6],0x2
case 0x0c6a0424:{CX--;IP=(ushort)(CX!=0?0x406:0x426);return true;} // LOOP 0xe000:0406
case 0x0c6a0426:{M16(DS,(0x5040),SI);IP=0x42a;return true;} // MOV word ptr [0x5040],SI
case 0x0c6a042a:{SI=unchecked((ushort)(Pop()));IP=0x42b;return true;} // POP SI
case 0x0c6a042b:{SP=unchecked((ushort)(BP));IP=0x42d;return true;} // MOV SP,BP
case 0x0c6a042d:{BP=unchecked((ushort)(Pop()));IP=0x42e;return true;} // POP BP
case 0x0c6a042e:{IP=Pop();CS=Pop();SP=(ushort)(SP+4);if(CS!=0x3197&&CS!=0xc6a&&CS!=0xfe0)return false;return true;} // RETF 0x4
case 0x0fe0000e:{Push(BP);IP=0xf;return true;} // PUSH BP
case 0x0fe0000f:{Push(DS);IP=0x10;return true;} // PUSH DS
case 0x0fe00010:{BP=unchecked((ushort)(SP));IP=0x12;return true;} // MOV BP,SP
case 0x0fe00012:{Push(DI);IP=0x13;return true;} // PUSH DI
case 0x0fe00013:{Push(SI);IP=0x14;return true;} // PUSH SI
case 0x0fe00014:{DF=false;IP=0x15;return true;} // CLD
case 0x0fe00015:{int a=(BP + 0x8);ushort sg=SS;BX=unchecked((ushort)(M16(sg,a)));DS=M16(sg,a+2);IP=0x18;return true;} // LDS BX,[BP + 0x8]
case 0x0fe00018:{CX=unchecked((ushort)(M16(DS,(BX))));IP=0x1a;return true;} // MOV CX,word ptr [BX]
case 0x0fe0001a:{Sub(CX,0);IP=0x1d;return true;} // CMP CX,0x0
case 0x0fe0001d:{IP=(ushort)((!ZF&&SF==OF)?0x22:0x1f);return true;} // JG 0xe000:0022
case 0x0fe0001f:{DF=true;IP=0x20;return true;} // STD
case 0x0fe00020:{CX=unchecked((ushort)(Sub(0,CX)));IP=0x22;return true;} // NEG CX
case 0x0fe00022:{int a=(BP + 0xc);ushort sg=SS;DI=unchecked((ushort)(M16(sg,a)));ES=M16(sg,a+2);IP=0x25;return true;} // LES DI,[BP + 0xc]
case 0x0fe00025:{int a=(BP + 0x10);ushort sg=SS;SI=unchecked((ushort)(M16(sg,a)));DS=M16(sg,a+2);IP=0x28;return true;} // LDS SI,[BP + 0x10]
case 0x0fe00028:{while(CX!=0){Move(2);CX--;} IP=0x2a;return true;} // MOVSW.REP ES:DI,SI
case 0x0fe0002a:{DF=false;IP=0x2b;return true;} // CLD
case 0x0fe0002b:{SI=unchecked((ushort)(Pop()));IP=0x2c;return true;} // POP SI
case 0x0fe0002c:{DI=unchecked((ushort)(Pop()));IP=0x2d;return true;} // POP DI
case 0x0fe0002d:{DS=unchecked((ushort)(Pop()));IP=0x2e;return true;} // POP DS
case 0x0fe0002e:{BP=unchecked((ushort)(Pop()));IP=0x2f;return true;} // POP BP
case 0x0fe0002f:{IP=Pop();CS=Pop();SP=(ushort)(SP+12);if(CS!=0x3197&&CS!=0xc6a&&CS!=0xfe0)return false;return true;} // RETF 0xc
default:throw new Exception("Untranslated PAGE instruction "+CS.ToString("x4")+":"+IP.ToString("x4"));}}
}
}
