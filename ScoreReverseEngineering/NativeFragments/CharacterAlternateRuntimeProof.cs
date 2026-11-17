using System;
using System.IO;
using ScoreModern.RecoveredFragments;
class CharacterAlternateRuntimeProof {
 class Captured:ICharacterDecisionMemory {
  public byte[] Bytes;public ushort Segment;
  public byte ReadByte(ushort s,ushort o){if(s!=Segment)throw new Exception("Unexpected segment");return Bytes[o];}
  public ushort ReadWord(ushort s,ushort o){return (ushort)(ReadByte(s,o)|(ReadByte(s,unchecked((ushort)(o+1)))<<8));}
 }
 static int Main(string[] args){try{
  string p=args[1];byte[] original=File.ReadAllBytes(args[0]),live=File.ReadAllBytes(Path.Combine(p,"gate-code.bin"));if(live.Length!=16)throw new Exception("Code length");for(int i=0;i<16;i++)if(original[0x16fa0+0x770+i]!=live[i])throw new Exception("Original code mismatch");
  // Explicit words from this recorded fixture only. Do not supply defaults for other contexts.
  ushort ds=Convert.ToUInt16(args[2],16),es=Convert.ToUInt16(args[3],16),di=Convert.ToUInt16(args[4],16),bx=Convert.ToUInt16(args[5],16);if(ds!=es)throw new Exception("This fixture requires the recorded segment alias");
  Captured m=new Captured();m.Segment=ds;m.Bytes=File.ReadAllBytes(Path.Combine(p,"post-patch-ds.bin"));CharacterDecisionResult r=CharacterOutputDecisions.ProbeAlternateGate(m,ds,es,di,bx);
  if(r.AL!=0x20||r.BX!=1||r.Branch)throw new Exception("Recorded alternate branch/AL/BX mismatch");byte[] after=File.ReadAllBytes(Path.Combine(p,"branch-ds.bin")),esCapture=File.ReadAllBytes(Path.Combine(p,"branch-es.bin"));if(m.Bytes.Length!=65536||after.Length!=65536||esCapture.Length!=65536)throw new Exception("Capture length");for(int i=0;i<65536;i++)if(m.Bytes[i]!=after[i]||after[i]!=esCapture[i])throw new Exception("Captured memory mismatch");
  Console.WriteLine("PASS native alternate gate against approved original synthetic fixture: exact 16 code bytes, AL/BX/branch and all 65536 DS bytes, with captured ES alias confirmed; zero exclusions.");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
