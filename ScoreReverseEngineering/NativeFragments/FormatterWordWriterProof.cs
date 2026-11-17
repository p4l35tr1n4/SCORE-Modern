using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class FormatterWordWriterProof {
 class Memory:IFormatterWordContext {
  public Dictionary<ushort,byte> Bytes=new Dictionary<ushort,byte>();
  public byte ReadByte(ushort a){byte b;return Bytes.TryGetValue(a,out b)?b:(byte)0;}
  public ushort ReadWord(ushort a){return (ushort)(ReadByte(a)|(ReadByte((ushort)(a+1))<<8));}
  public void WriteByte(ushort a,byte b){Bytes[a]=b;}
  public void WriteWord(ushort a,ushort b){WriteByte(a,(byte)b);WriteByte((ushort)(a+1),(byte)(b>>8));}
 }
 // Reference executes the comparison branches and writes decoded from the
 // original 78 bytes. Local physical stack and preserved registers excluded.
 static void Reference(Memory m,ushort low,ushort high,byte[] code) {
  int pc=0x2a52;ushort si=0;bool z=false,less=false,above=false;
  Func<int,byte> b=a=>code[a-0x2a4b];
  while(true){switch(pc){
   case 0x2a52:si=m.ReadWord(0xd988);pc=0x2a56;break;
   case 0x2a56:z=high==0;less=(short)high<0;pc=0x2a5a;break;
   case 0x2a5a:pc=(!z&&!less)?0x2a72:0x2a5c;break;
   case 0x2a5c:pc=less?0x2a64:0x2a5e;break;
   case 0x2a5e:above=low>0x78;pc=0x2a62;break;
   case 0x2a62:pc=above?0x2a72:0x2a64;break;
   case 0x2a64:z=high==0xffff;less=(short)high < -1;pc=0x2a68;break;
   case 0x2a68:pc=(!z&&!less)?0x2a88:0x2a6a;break;
   case 0x2a6a:pc=less?0x2a72:0x2a6c;break;
   case 0x2a6c:above=low>=0xff88;pc=0x2a70;break;
   case 0x2a70:pc=above?0x2a88:0x2a72;break;
   case 0x2a72:m.WriteByte(si,b(0x2a74));pc=0x2a75;break;
   case 0x2a75:si=(ushort)(si+1);pc=0x2a76;break;
   case 0x2a76:pc=0x2a7e;break;
   case 0x2a7e:m.WriteWord(si,low);pc=0x2a80;break;
   case 0x2a80:m.WriteWord((ushort)(si+2),high);pc=0x2a83;break;
   case 0x2a83:si=(ushort)(si+4);pc=0x2a86;break;
   case 0x2a86:pc=0x2a8e;break;
   case 0x2a88:m.WriteByte(si,(byte)low);pc=0x2a8d;break;
   case 0x2a8d:si=(ushort)(si+1);pc=0x2a8e;break;
   case 0x2a8e:m.WriteWord(0xd988,si);return;
   default:throw new Exception("Unexpected PC");
  }}
 }
 static void Main(string[] args){
  byte[] image=File.ReadAllBytes(args[0]);byte[] code=new byte[78];Array.Copy(image,0x1f080+0x2a4b,code,0,78);
  string hex=BitConverter.ToString(code).Replace("-","").ToLowerInvariant();
  if(hex!="558bec83ec02568b3688d9837e06007f167c06837e0478770e837e06ff7f1e7c06837e04887316c6047f468d5e048b078b5702890489540283c604eb068a4604880446893688d95e8be55dc20400")throw new Exception("Original code mismatch");
  ushort[] highs={0,1,0x7fff,0x8000,0xfffe,0xffff};ushort[] cursors={0x1000,0xd986,0xd987,0xd988,0xd989,0xd98a,0xfffa};int count=0;
  foreach(ushort high in highs)for(int v=0;v<65536;v++){
   Memory a=new Memory(),b=new Memory();ushort cursor=cursors[v%cursors.Length];a.WriteWord(0xd988,cursor);b.WriteWord(0xd988,cursor);
   FormatterWordWriter.Execute(a,(ushort)v,high);Reference(b,(ushort)v,high,code);
   if(a.Bytes.Count!=b.Bytes.Count)throw new Exception("Write extent mismatch");
   foreach(var p in a.Bytes)if(!b.Bytes.ContainsKey(p.Key)||b.Bytes[p.Key]!=p.Value)throw new Exception("Memory mismatch");count++;
  }
  byte[] before=File.ReadAllBytes(args[1]),after=File.ReadAllBytes(args[2]);Memory runtime=new Memory();for(int i=0;i<before.Length;i++)runtime.Bytes[(ushort)i]=before[i];
  FormatterWordWriter.Execute(runtime,9,0);int compared=0;
  // Entry SP=E1BC. PUSH BP writes E1BA..E1BB; PUSH SI writes E1B6..E1B7.
  for(int i=0;i<65536;i++){if((i>=0xe1b6&&i<0xe1b8)||(i>=0xe1ba&&i<0xe1bc))continue;if(runtime.ReadByte((ushort)i)!=after[i])throw new Exception("Original runtime mismatch at "+i.ToString("X4"));compared++;}
  Console.WriteLine("PASS "+count+" branch-reference cases; original 78-byte identity; "+compared+" original runtime DS bytes (4 exact PUSH stack bytes excluded).");
 }
}
