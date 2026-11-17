using System;
namespace ScoreModern.RecoveredFragments {
 // The original RN workspace has14200 real words. A fresh measured VM
 // starts with zero words; loading overlays the file, packing overwrites
 // its active prefix, and saving retains the word immediately after it.
 public sealed class RetainedParameterStorage {
  public const int Capacity=14200*4;
  readonly byte[] data=new byte[Capacity+24];
  readonly bool[] known=new bool[Capacity+24];
  int length;
  public RetainedParameterStorage(){for(int n=0;n<Capacity;n++)known[n]=true;}
  public RetainedParameterStorage Clone(){var result=new RetainedParameterStorage();Array.Copy(data,result.data,data.Length);Array.Copy(known,result.known,known.Length);result.length=length;return result;}
  public void Forget(){Array.Clear(known,0,known.Length);}
  public void Opaque(byte[] active){Forget();length=Math.Min(active.Length,Capacity);Array.Copy(active,data,length);}
  public bool Matches(byte[] active){if(active.Length!=length)return false;for(int n=0;n<length;n++)if(active[n]!=data[n])return false;return true;}
  public void Load(byte[] payload,int trailerBytes,byte[] producer,byte[] active){
   if(payload.Length>Capacity||active.Length>Capacity){Opaque(active);return;}
   int original=payload.Length-trailerBytes;if(original<0){Opaque(active);return;}
   Array.Copy(payload,data,payload.Length);for(int n=0;n<payload.Length;n++)known[n]=true;
   if(trailerBytes==24&&producer!=null){Array.Copy(producer,0,data,original+4,4);}else for(int n=original+4;n<original+trailerBytes;n++)known[n]=false;
   Array.Copy(active,data,active.Length);for(int n=0;n<active.Length;n++)known[n]=true;length=active.Length;
   // Short-record expansion has its own loader continuation; do not infer
   // how it relocates the trailing storage from the logical records alone.
   if(original!=length)for(int n=length;n<Math.Min(known.Length,length+24);n++)known[n]=false;
  }
  public void Pack(byte[] active){
   if(active.Length>Capacity){Opaque(active);return;}
   Array.Copy(active,data,active.Length);for(int n=0;n<active.Length;n++)known[n]=true;length=active.Length;
   // Recorded routines reuse these following words as scratch. The first
   // trailing word itself is retained; the other five are written on SA.
   for(int n=length+4;n<Math.Min(known.Length,length+24);n++)known[n]=false;
  }
  //3054:0591..05C0 writes three literal spaces after the text bytes,
  //including bytes beyond the last packed record. Unknown untouched bytes stay unknown.
  public void TextInputPadding(int offset){if(offset<0||offset+3>Capacity)return;for(int n=offset;n<offset+3;n++){data[n]=32;known[n]=true;}}
  public void WriteKnown(int offset,byte[] bytes){if(offset<0||offset>data.Length-bytes.Length)throw new NotSupportedException("RN write exceeds its retained storage.");Array.Copy(bytes,0,data,offset,bytes.Length);for(int n=offset;n<offset+bytes.Length;n++)known[n]=true;}
  public void CopyBytes(int source,int destination,int count){if(source<0||destination<0||count<0||source>data.Length-count||destination>data.Length-count)throw new NotSupportedException("RN copy exceeds its retained storage.");Array.Copy(data,source,data,destination,count);Array.Copy(known,source,known,destination,count);}
  public void CommitReplay(byte[] active){if(active.Length>Capacity)throw new NotSupportedException("RN replay exceeds capacity.");WriteKnown(0,active);length=active.Length;}
  public byte[] ReadKnown(int offset,int count){if(offset<0||count<0||offset>data.Length-count)return null;for(int n=offset;n<offset+count;n++)if(!known[n])return null;var result=new byte[count];Array.Copy(data,offset,result,0,count);return result;}
  public byte[] UndefinedWord(){if(length+4>data.Length)return null;for(int n=length;n<length+4;n++)if(!known[n])return null;var word=new byte[4];Array.Copy(data,length,word,0,4);return word;}
  public void Saved(byte[] trailer){if(length+trailer.Length>data.Length)return;Array.Copy(trailer,0,data,length,trailer.Length);for(int n=length;n<length+trailer.Length;n++)known[n]=true;}
 }
}
