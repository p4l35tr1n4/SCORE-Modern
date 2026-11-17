using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
class CharacterAlternateOutputRuntimeProof {
 class Record {public int Index;public ushort CS,IP;public CharacterBiosWords Words;}
 static Record Parse(string line){string[] a=line.Split(' ');return new Record{Index=int.Parse(a[0]),Words=new CharacterBiosWords{AX=H(a[1]),BX=H(a[2]),CX=H(a[3]),DX=H(a[4]),SS=H(a[5]),SP=H(a[6])},CS=H(a[7]),IP=H(a[8])};}
 static ushort H(string s){return Convert.ToUInt16(s,16);}
 static bool Equal(CharacterBiosWords a,CharacterBiosWords b){return a.AX==b.AX&&a.BX==b.BX&&a.CX==b.CX&&a.DX==b.DX&&a.SS==b.SS&&a.SP==b.SP;}
 class Replay:ICharacterBiosContext {
  public byte[] Memory;public ushort Segment;public string Folder;public List<Record> Records;public int Call,Compared;
  public void WriteWord(ushort s,ushort o,ushort v){if(s!=Segment)throw new Exception("Segment");Memory[o]=(byte)v;Memory[(ushort)(o+1)]=(byte)(v>>8);}
  public ushort ReadWord(ushort s,ushort o){if(s!=Segment)throw new Exception("Segment");return (ushort)(Memory[o]|Memory[(ushort)(o+1)]<<8);}
  public void Compare(byte[] expected){if(expected.Length!=65536)throw new Exception("Capture length");for(int i=0;i<65536;i++)if(Memory[i]!=expected[i])throw new Exception("Stack memory mismatch at "+i.ToString("X4"));Compared+=65536;}
  public void Interrupt10(ref CharacterBiosWords w){Record request=Records[Call*2],result=Records[Call*2+1];if(request.CS!=0x1f1e||request.IP!=0x235)throw new Exception("Request boundary");CharacterBiosWords adjusted=w;adjusted.SP=(ushort)(adjusted.SP-6);if(!Equal(adjusted,request.Words))throw new Exception("Request words "+Call);
   byte[] before=File.ReadAllBytes(Path.Combine(Folder,"event-"+request.Index+"-ss.bin"));
   // Interrupt frames/flags are outside the native projection. Supply their six
   // recorded bytes as an explicit harness effect, then compare the full segment.
   for(int n=0;n<6;n++){ushort at=(ushort)(request.Words.SP+n);Memory[at]=before[at];}
   Compare(before);Memory=File.ReadAllBytes(Path.Combine(Folder,"event-"+result.Index+"-ss.bin"));w=result.Words;Call++;
  }
 }
 static int Main(string[] args){try{
  string p=args[1];byte[] original=File.ReadAllBytes(args[0]),live=File.ReadAllBytes(Path.Combine(p,"alternate-code.bin"));if(live.Length!=50)throw new Exception("Live code length");for(int i=0;i<50;i++)if(original[0x16fa0+0x780+i]!=live[i])throw new Exception("Live code");
  string[] lines=File.ReadAllLines(Path.Combine(p,"replay-words.txt"));Record entry=Parse(lines[0]);List<Record> events=new List<Record>();for(int i=1;i<lines.Length;i++)events.Add(Parse(lines[i]));if(events.Count!=12)throw new Exception("Events");Replay replay=new Replay{Folder=p,Segment=entry.Words.SS,Memory=File.ReadAllBytes(Path.Combine(p,"branch-ss.bin")),Records=events};CharacterBiosWords w=CharacterAlternateOutput.Run(replay,entry.Words);Record exit=events[10];if(replay.Call!=5||exit.CS!=0x1f1e||exit.IP!=0x7ba||!Equal(w,exit.Words))throw new Exception("Exit result");replay.Compare(File.ReadAllBytes(Path.Combine(p,"event-"+exit.Index+"-ss.bin")));
  Console.WriteLine("PASS original alternate-output replay: 50 live code bytes, five request/return word states and exit words, "+replay.Compared+" stack-segment byte comparisons. The harness explicitly supplies six recorded interrupt-frame bytes per call and measured BIOS return effects; no native interrupt/flags or BIOS implementation claimed.");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
