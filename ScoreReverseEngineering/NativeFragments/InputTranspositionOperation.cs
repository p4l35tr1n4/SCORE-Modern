using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public sealed class TranspositionRequest {
  public float Staff,Left,Right;public short Semitones;public short? Clef;
  public short[] ClefChanges;
  public bool AddKey,ForceKeyQuestion;
 }
 public sealed class TranspositionWorkspace {
  public readonly TranspositionCollectorState Collector;
  public readonly TranspositionClefState Clefs;
  public readonly short[] PitchWords,AccidentalMarkers;
  public short KindZeroGuard;public bool AlreadySorted;
  public TranspositionRelationshipWindow Window;
  public TranspositionWorkspace(TranspositionCollectorState collector,TranspositionClefState clefs,short[] pitchWords,short[] markers,short guard,TranspositionRelationshipWindow window,bool sorted){
   if(collector==null||clefs==null||pitchWords==null||pitchWords.Length!=200||markers==null||markers.Length!=200)throw new ArgumentException("Explicit original transposition workspace required.");
   Collector=collector;Clefs=clefs;PitchWords=pitchWords;AccidentalMarkers=markers;KindZeroGuard=guard;Window=window;AlreadySorted=sorted;
  }
 }
 public sealed class TranspositionOperationResult {
  public List<float[]> Rows;public TranspositionCollectorResult Collection;
  public TranspositionInterval Interval;public TranspositionMainPassResult Engine;
  public bool SortMarksModified,KeyQuestion,InsertedKey,WarningRequired;
 }
 public sealed class TranspositionOperationSession {
  internal TranspositionRequest Request;internal TranspositionWorkspace Workspace;
  internal short[] Pitches;internal short Staff;internal bool Finished;
  public TranspositionOperationResult Result;
 }
 // Recovered Main argument-zero composition. Interactive prompts and final
 // optional normalization belong to the caller. Retained words are supplied,
 // not synthesized from a presumed application history.
 public static class InputTranspositionOperation {
  static short Word(float value){long v;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out v))throw new NotSupportedException("Non-finite transposition staff.");return unchecked((short)v);}
  public static TranspositionOperationResult Apply(IList<float[]> source,TranspositionRequest request,TranspositionWorkspace workspace){
   var session=Begin(source,request,workspace);
   if(request.ClefChanges!=null){
    if(request.Clef.HasValue)throw new ArgumentException("Use one clef choice or an explicit ordered choice sequence.");
    foreach(short choice in request.ClefChanges)ChangeClef(session,choice);
   }else if(request.Clef.HasValue)ChangeClef(session,request.Clef.Value);
   return Finish(session,request.AddKey);
  }
  public static TranspositionOperationSession Begin(IList<float[]> source,TranspositionRequest request,TranspositionWorkspace workspace){
   if(request==null||workspace==null)throw new ArgumentNullException();
   var result=new TranspositionOperationResult();result.SortMarksModified=InputTranspositionOrder.MarksModified(source.Count,workspace.AlreadySorted);
   var order=InputTranspositionOrder.OrderForMain(source,workspace.AlreadySorted);var rows=new List<float[]>();foreach(int i in order)rows.Add((float[])source[i].Clone());result.Rows=rows;
   short staff=Word(request.Staff);var interval=InputTranspositionInterval.Convert(request.Semitones);result.Interval=interval;
   var collection=InputTranspositionCollector.Build(rows,staff,request.Left,request.Right,9,workspace.Collector);result.Collection=collection;
   var entries=collection.FirstPass.Records;int count=entries.Count;
   var pitches=new short[count];Array.Copy(workspace.PitchWords,pitches,count);
   for(int i=0;i<count;i++){entries[i].Kind=collection.Partition.Kinds[i];if(entries[i].Pitch.HasValue)pitches[i]=entries[i].Pitch.Value;}
   result.KeyQuestion=count!=0&&InputTranspositionKeyInsertion.ShouldPrompt(request.ForceKeyQuestion,collection.FirstPass.FirstKeyState,interval.SemitoneClass);
   return new TranspositionOperationSession{Request=request,Workspace=workspace,Pitches=pitches,Staff=staff,Result=result};
  }
  public static void ChangeClef(TranspositionOperationSession session,short choice){
   if(session==null||session.Finished)throw new InvalidOperationException("No active transposition operation.");
   if(session.Pitches.Length==0)return;
   // Preserve each intermediate binary32 store when B revisits the question.
   InputTranspositionClefs.Apply(session.Result.Rows,session.Result.Collection.FirstPass.Records,session.Pitches,session.Request.Staff,choice,session.Workspace.Clefs);
   Array.Copy(session.Pitches,session.Workspace.PitchWords,session.Pitches.Length);
  }
  public static TranspositionOperationResult Finish(TranspositionOperationSession session,bool addKey){
   return FinishCore(session,addKey,true);
  }
  public static TranspositionOperationResult FinishCore(TranspositionOperationSession session,bool addKey,bool mainAttachments){
   if(session==null||session.Finished)throw new InvalidOperationException("No active transposition operation.");
   session.Finished=true;
   var result=session.Result;var request=session.Request;var workspace=session.Workspace;
   var rows=result.Rows;var collection=result.Collection;var interval=result.Interval;var entries=collection.FirstPass.Records;var pitches=session.Pitches;int count=entries.Count;short staff=session.Staff;
   if(count==0)return result;
   short target=interval.Key,diatonic=interval.Diatonic,forcedKey=0;
   result.KeyQuestion=InputTranspositionKeyInsertion.ShouldPrompt(request.ForceKeyQuestion,collection.FirstPass.FirstKeyState,interval.SemitoneClass);
   result.InsertedKey=InputTranspositionKeyInsertion.ShouldInsert(addKey,collection.FirstPass.FirstKeyState,interval.SemitoneClass);
   if(result.InsertedKey){InputTranspositionKey.Convert(interval.Semitones,0,out diatonic,out target);forcedKey=target;InputTranspositionKeyInsertion.Append(rows,request.Staff,target,entries[0].Clef);}
   // Parent D8CE captures this shift before keys inside the engine change it.
   short attachmentShift=diatonic;
   var state=new TranspositionMainState{Semitones=interval.Semitones,TargetKey=target,Diatonic=diatonic,KeyOverride=forcedKey,KindZeroGuard=workspace.KindZeroGuard,Sentinel=-999,ErasedCode=13,RelationshipMargin=3,RelationshipWidth=6,InitialWindow=workspace.Window,AccidentalMarkers=workspace.AccidentalMarkers};
   result.Engine=InputTranspositionMainPass.Apply(rows,entries,pitches,collection.FirstPass.RelationshipItems,state);workspace.Window=result.Engine.Tail.Window;Array.Copy(pitches,workspace.PitchWords,count);
   bool warning;InputTranspositionEmbeddedControl.Apply(rows,staff,interval.SemitoneClass,out warning);result.WarningRequired=warning;
   if(mainAttachments)InputTranspositionAttachments.Apply(rows,staff,request.Left,request.Right,attachmentShift,InputTranspositionAttachments.ShouldApply((short)collection.FirstPass.RelationshipItems.Count,collection.FirstPass.HasExtendedBeam));
   // Main X/nonzero sort intentionally leaves AlreadySorted unchanged.
   return result;
  }
 }
}
