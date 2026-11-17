using System;
using System.IO;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {public sealed partial class Editor {
 RetainedParameterStorage parameterStorage=new RetainedParameterStorage();
 bool parameterStorageDirty;
 int parameterStorageDepth,parameterStorageGeneration;
 static byte[] ParameterBytes(Score value){using(var stream=new MemoryStream()){foreach(var item in value.Items)stream.Write(item.Bytes,0,item.Bytes.Length);return stream.ToArray();}}
 void LoadParameterStorage(){
  var active=ParameterBytes(score);byte[] producer=null;
  try{producer=BitConverter.GetBytes(OriginalMusSave.ReadProducer(source));}catch(IOException){}catch(InvalidDataException){}catch(UnauthorizedAccessException){}
  if(score.OriginalFilePayload!=null)parameterStorage.Load(score.OriginalFilePayload,score.Trailer.Length,producer,active);else parameterStorage.Opaque(active);
  parameterStorageDirty=false;score.RetainedUndefinedWord=parameterStorage.UndefinedWord();
 }
 float? VerifiedParameterTrailingWord(){if(score.Trailer.Length!=24||!score.RawStorageLayoutVerified||parameterStorageDirty||!parameterStorage.Matches(ParameterBytes(score)))return null;var bytes=parameterStorage.UndefinedWord();return bytes==null?(float?)null:BitConverter.ToSingle(bytes,0);}
 void InvalidateParameterStorage(){InvalidateDisplayStorage();if(score!=null&&score.CancelledSaveRecords!=null)score.CancelledSaveValid=false;if(tabCommandStage==TabCommandStage.None)InvalidateTabFrequencyHistory();parameterStorageGeneration++;if(parameterStorageDepth!=0)return;parameterStorageDirty=true;parameterStorage.Forget();if(score!=null)score.RetainedUndefinedWord=null;}
 void PrepareParameterSave(){var active=ParameterBytes(score);if(parameterStorageDirty||!parameterStorage.Matches(active)){parameterStorage.Opaque(active);parameterStorageDirty=false;score.RetainedUndefinedWord=null;}}
 void RetainTextInputPadding(){if(score.Items.Count==0)return;var item=score.Items[score.Items.Count-1];if(item.Code!=16||item.Count<13)return;float textLength=item.P(12);if(Single.IsNaN(textLength)||Single.IsInfinity(textLength)||textLength<0||textLength>item.Bytes.Length-56||textLength!=(int)textLength)return;parameterStorage.TextInputPadding(ParameterBytes(score).Length-item.Bytes.Length+56+(int)textLength);score.RetainedUndefinedWord=parameterStorage.UndefinedWord();}
 void RecordParameterSave(byte[] trailer){if(NativeEditCompatibility.ReproduceInvalidStates&&score.CancelledSaveRecords!=null){RecordCancelledLyricSave(trailer);return;}parameterStorage.Saved(trailer);score.RetainedUndefinedWord=parameterStorage.UndefinedWord();if(score.CancelledSaveRecords!=null)DiscardCancelledLyricStorage();}
 ParameterMutation BeginParameterMutation(){
  var active=ParameterBytes(score);if(parameterStorageDirty||!parameterStorage.Matches(active)){parameterStorage.Opaque(active);parameterStorageDirty=false;score.RetainedUndefinedWord=null;}
  parameterStorageDepth++;return new ParameterMutation(this,active,parameterStorageGeneration,mode==ScreenMode.Edit&&canvas.EditModified);
 }
 sealed class ParameterMutation:IDisposable {
  Editor owner;readonly byte[] before;readonly int generation;readonly bool working;
  internal ParameterMutation(Editor value,byte[] bytes,int counter,bool edit){owner=value;before=bytes;generation=counter;working=edit;}
  internal void Complete(){
   var editor=owner;if(editor==null)return;owner=null;editor.parameterStorageDepth--;var active=ParameterBytes(editor.score);
   bool changed=before.Length!=active.Length;for(int n=0;!changed&&n<active.Length;n++)changed=before[n]!=active[n];
   if(working){editor.parameterStorage.Opaque(active);editor.score.RetainedUndefinedWord=null;return;}
   if(changed||generation!=editor.parameterStorageGeneration)editor.parameterStorage.Pack(active);
   editor.score.RetainedUndefinedWord=editor.parameterStorage.UndefinedWord();
   if((changed||generation!=editor.parameterStorageGeneration)&&editor.groupCommand==null&&editor.musicInput==null&&editor.originalTextDraft==null&&active.Length<=RetainedParameterStorage.Capacity)editor.score.RawStorageLayoutVerified=true;
  }
  public void Dispose(){if(owner==null)return;var editor=owner;owner=null;editor.parameterStorageDepth--;editor.parameterStorageDirty=true;editor.parameterStorage.Forget();editor.score.RetainedUndefinedWord=null;}
 }
}}
