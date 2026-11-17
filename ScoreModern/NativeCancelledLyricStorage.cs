using System;using System.Linq;using System.Collections.Generic;
namespace ScoreModern {public sealed partial class Editor {
 // Cancel restores the indexed item count, but leaves allocated lyric records
 // and the RN end pointer. Safe mode discards them; original mode saves them.
 void CaptureCancelledLyricStorage(OriginalTextEntry entry){
  bool repeated=originalCancelledLyricBuffer&&score.CancelledSaveRecords!=null;var active=ParameterBytes(score);bool established=!repeated||score.CancelledSaveValid&&score.CancelledSaveBaseline!=null&&score.CancelledSaveBaseline.SequenceEqual(active);var retained=parameterStorage.Clone();if(parameterStorageDirty||!retained.Matches(active))retained.Opaque(active);
  int count=Math.Min(entry.Units.Count,entry.NextUnit+1);var records=entry.Units.Take(count).Select(i=>i.Clone()).ToList();
  if(count>1)records[count-1].Set(3,(float)((double)records[count-2].P(3)+records[count-2].P(13)+2));
  int offset=active.Length+(repeated?score.CancelledSaveRecords.Sum(i=>i.Bytes.Length):0);foreach(var item in records){retained.WriteKnown(offset,item.Bytes);retained.TextInputPadding(offset+56+(int)item.P(12));offset+=item.Bytes.Length;}
  score.CancelledSaveRecords=repeated?score.CancelledSaveRecords.Concat(records).ToList():records;score.CancelledSaveBaseline=active;score.CancelledSaveUndefinedWord=retained.ReadKnown(offset,4);score.CancelledSaveValid=established;parameterStorage=retained;parameterStorageDirty=false;
 }
 bool CancelledLyricSaveVerified(){return score.CancelledSaveValid&&score.CancelledSaveRecords!=null&&score.CancelledSaveUndefinedWord!=null&&score.CancelledSaveBaseline!=null&&score.CancelledSaveBaseline.SequenceEqual(ParameterBytes(score));}
 void RecordCancelledLyricSave(byte[] trailer){int offset=ParameterBytes(score).Length;foreach(var item in score.CancelledSaveRecords){parameterStorage.WriteKnown(offset,item.Bytes);offset+=item.Bytes.Length;}parameterStorage.WriteKnown(offset,trailer);}
 void DiscardCancelledLyricStorage(){score.CancelledSaveRecords=null;score.CancelledSaveBaseline=score.CancelledSaveUndefinedWord=null;score.CancelledSaveValid=false;originalCancelledLyricBuffer=false;}
}}
