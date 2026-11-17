using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class CharacterRoutineRuntimeProof {
 class Record {public string Name;public CharacterRoutineWords Words;public ushort CS,IP;}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static Record Parse(string s){string[] a=s.Split(' ');return new Record{Name=a[0],Words=new CharacterRoutineWords{AX=H(a[1]),BX=H(a[2]),CX=H(a[3]),DX=H(a[4]),SI=H(a[5]),DI=H(a[6]),BP=H(a[7]),DS=H(a[8]),ES=H(a[9]),SS=H(a[10]),SP=H(a[11])},CS=H(a[12]),IP=H(a[13])};}
 static string Show(CharacterRoutineWords w){return w.AX+":"+w.BX+":"+w.CX+":"+w.DX+":"+w.SI+":"+w.DI+":"+w.BP+":"+w.DS+":"+w.ES+":"+w.SS+":"+w.SP;}
 class Replay:ICharacterRoutineContext {
  public string Folder,Mode;public ushort Segment;public byte[] Memory;public List<Record> Records;public int Calls,ByteReads,Compared;public bool Patched;public Record CursorPatch;
  byte[] Load(string name){byte[] b=File.ReadAllBytes(Path.Combine(Folder,name));if(b.Length!=65536)throw new Exception("Capture length");return b;}
  public void Compare(byte[] b){for(int i=0;i<65536;i++)if(Memory[i]!=b[i])throw new Exception("Memory mismatch "+i.ToString("X4"));Compared+=65536;}
  public byte ReadByte(ushort s,ushort o){if(s!=Segment)throw new Exception("Segment");ByteReads++;if(Mode=="flag"&&ByteReads==2){
    // Replay the explicitly approved external test patch at 0770, before its
    // second character read. It is not part of the native routine.
    Compare(Load("pre-patch-ds.bin"));byte[] expected=(byte[])Memory.Clone();expected[0x8164]=1;expected[0x8165]=0;byte[] actual=Load("post-patch-ds.bin");for(int i=0;i<65536;i++)if(expected[i]!=actual[i])throw new Exception("External patch extent");Memory=actual;Patched=true;
   }return Memory[o];}
  public ushort ReadWord(ushort s,ushort o){if(s!=Segment)throw new Exception("Segment");return (ushort)(Memory[o]|Memory[(ushort)(o+1)]<<8);}
  public void WriteWord(ushort s,ushort o,ushort v){if(s!=Segment)throw new Exception("Segment");Memory[o]=(byte)v;Memory[(ushort)(o+1)]=(byte)(v>>8);}
  public void Interrupt10(ref CharacterRoutineWords w,ushort returnOffset){Record request=Records[Calls*2],result=Records[Calls*2+1];CharacterRoutineWords adjusted=w;adjusted.SP=(ushort)(adjusted.SP-6);if(Show(adjusted)!=Show(request.Words)||request.CS!=0x1f1e||request.IP!=0x235||result.CS!=0x1f1e||result.IP!=returnOffset)throw new Exception("Call boundary "+Calls);
   byte[] before=Load(request.Name+"-ss.bin");for(int n=0;n<6;n++){ushort at=(ushort)(request.Words.SP+n);Memory[at]=before[at];}Compare(before);
   ushort ip=(ushort)(before[request.Words.SP]|before[(ushort)(request.Words.SP+1)]<<8),cs=(ushort)(before[(ushort)(request.Words.SP+2)]|before[(ushort)(request.Words.SP+3)]<<8);if(ip!=returnOffset||cs!=0x1f1e)throw new Exception("Recorded IRQ frame");
   Memory=Load(result.Name+"-ss.bin");w=result.Words;
   if(Mode=="cursor"&&Calls==0){CharacterRoutineWords expected=w;expected.DX=(ushort)((w.DX&0xff00)|Memory[0x4b98]);if(Show(expected)!=Show(CursorPatch.Words)||CursorPatch.CS!=result.CS||CursorPatch.IP!=result.IP)throw new Exception("Cursor patch extent");w=CursorPatch.Words;Patched=true;}
   Calls++;
  }
 }
 static int Main(string[] args){try{string p=args[1],mode=args.Length>2?args[2]:"flag";if(mode!="flag"&&mode!="ordinary"&&mode!="cursor")throw new Exception("Mode");byte[] raw=File.ReadAllBytes(args[0]),code=File.ReadAllBytes(Path.Combine(p,"routine-code.bin"));if(code.Length!=132)throw new Exception("Code length");for(int i=0;i<132;i++)if(raw[0x16fa0+0x73b+i]!=code[i])throw new Exception("Live code");string[] lines=File.ReadAllLines(Path.Combine(p,"routine-replay-words.txt"));Record entry=Parse(lines[0]);List<Record> events=new List<Record>();for(int i=1;i<lines.Length;i++)events.Add(Parse(lines[i]));int count=mode=="flag"?14:mode=="ordinary"?5:8,calls=mode=="flag"?6:mode=="ordinary"?2:3;if(events.Count!=count)throw new Exception("Event count");Replay replay=new Replay{Mode=mode,Folder=p,Segment=entry.Words.SS,Memory=File.ReadAllBytes(Path.Combine(p,"routine-entry-ss.bin")),Records=events};if(mode=="cursor")replay.CursorPatch=Parse(File.ReadAllText(Path.Combine(p,"cursor-patch-words.txt")).Trim());CharacterRoutineResult r=CharacterOutputRoutine.Run(replay,entry.Words);Record end=events[events.Count-1];if(replay.Calls!=calls||replay.Patched!=(mode!="ordinary")||Show(r.Words)!=Show(end.Words)||r.CS!=end.CS||r.IP!=end.IP)throw new Exception("Routine return");replay.Compare(File.ReadAllBytes(Path.Combine(p,end.Name+"-ss.bin")));Console.WriteLine("PASS composed original-runtime "+mode+" replay: 132 live code bytes, "+calls+" BIOS request/return word states, far return/result words, "+replay.Compared+" stack/data byte comparisons. Recorded interrupt frames, measured BIOS effects and any approved test patch are explicit harness inputs; flags/BIOS/rendering remain external.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
