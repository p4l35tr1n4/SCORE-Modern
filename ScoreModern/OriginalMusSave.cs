using System;using System.IO;using System.Linq;
namespace ScoreModern {
 public static class OriginalMusSave {
  // Recorded DOS save: the 20-byte older trailer gains the external producer
  // DWORD after its first word; the 24-byte trailer carries preceding-word count 5.
  // DOS SCORE3.11 stamps version3.0, including on the measured WinScore6 input.
  // Measurement and undefined trailer bytes are preserved.
  internal static uint ReadProducer(string root){string path=NativeLibraryLocation.FileFor(root,"SCOR3_10.DAT");byte[] data=File.ReadAllBytes(path);if(data.Length<4)throw new InvalidDataException("External SCORE producer data is truncated.");uint producer=BitConverter.ToUInt32(data,0);if(producer<1000000||producer>9999999)throw new InvalidDataException("External SCORE producer identifier is outside the verified seven-digit format.");return producer;}
  public static Score Prepare(Score score,string root){if(score==null)throw new ArgumentNullException("score");return Prepare(score,ReadProducer(root));}
  internal static Score Prepare(Score score,uint producer){var copy=score.Clone();byte[] old=copy.Trailer,next=new byte[24];if(old.Length==20){Array.Copy(old,0,next,0,4);Array.Copy(old,4,next,8,16);}else if(old.Length==24)Array.Copy(old,next,24);else throw new InvalidDataException("MUS trailer outside the verified save formats.");if(score.RetainedUndefinedWord!=null&&score.RetainedUndefinedWord.Length==4)Array.Copy(score.RetainedUndefinedWord,0,next,0,4);Array.Copy(BitConverter.GetBytes(producer),0,next,4,4);Array.Copy(BitConverter.GetBytes(3f),0,next,8,4);Array.Copy(BitConverter.GetBytes(5f),0,next,16,4);if(NativeEditCompatibility.ReproduceInvalidStates&&score.CancelledSaveRecords!=null){if(!score.CancelledSaveValid||score.CancelledSaveUndefinedWord==null)throw new NotSupportedException("Cancelled lyric physical save is not established for this state.");copy.Items.AddRange(score.CancelledSaveRecords.Select(i=>i.Clone()));Array.Copy(score.CancelledSaveUndefinedWord,0,next,0,4);copy.CancelledSaveRecords=null;}copy.Trailer=next;return copy;}
 }
}
