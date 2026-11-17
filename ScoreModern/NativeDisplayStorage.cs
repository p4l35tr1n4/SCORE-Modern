using System;using System.Collections.Generic;using System.IO;using System.Diagnostics;using System.Linq;
namespace ScoreModern {public sealed partial class Editor {
 NativeRetainedDisplayArray physicalDisplayStorage=new NativeRetainedDisplayArray();
 NativeFreshDisplayArray physicalDisplayLast;
 bool physicalDisplayKnown=true,physicalDisplayHasLoad,physicalDisplayActualFonts,physicalDisplayReferenceRedrawPending;
 DrawingLibrary physicalDisplayLibrary;int physicalDisplayLibraryRevision;
 byte[] physicalDisplayMusic;string physicalDisplayHidden="";
 double physicalDisplayLoadMilliseconds;
 void InvalidateDisplayStorage(){physicalDisplayKnown=false;physicalDisplayReferenceRedrawPending=false;physicalDisplayLast=null;physicalDisplayNotePending=false;physicalDisplayWorkingMusic=null;physicalDisplayWorkingScore=null;canvas.RetainedDisplayCenter=null;}
 void ObserveDisplayModeRebuild(){
  if(!physicalDisplayKnown||physicalDisplayLast==null)return;
  if(physicalDisplayNotePending||physicalDisplayLibrary!=canvas.Library||
     physicalDisplayLibraryRevision!=NativeLibraryLocation.RevisionFor(canvas.Library.Root)||
     !physicalDisplayMusic.SequenceEqual(score.Encode())){
   InvalidateDisplayStorage();
   return;
  }
  try{
   using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout}){
    probe.DisappearingStaffs.UnionWith(canvas.DisappearingStaffs);
    physicalDisplayLast=physicalDisplayStorage.Produce(probe);
   }
   physicalDisplayHidden=String.Join(",",canvas.DisappearingStaffs.OrderBy(n=>n));
   physicalDisplayActualFonts=canvas.TextLayout.ShowActualDatFonts;
   physicalDisplayReferenceRedrawPending=false;
   canvas.RetainedDisplayCenter=physicalDisplayStorage.RetainedLastItemPoint();
  }
  catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
 }
 void ObserveDisplayLoad(bool restart=false){
  canvas.RetainedDisplayCenter=null;
  physicalDisplayReferenceRedrawPending=false;
  if(!restart)canvas.RetainedRestartPoint=null;
  if(!physicalDisplayKnown)return;
  if(physicalDisplayNotePending){InvalidateDisplayStorage();return;}
  if(physicalDisplayHasLoad&&(physicalDisplayLibrary!=canvas.Library||physicalDisplayLibraryRevision!=NativeLibraryLocation.RevisionFor(canvas.Library.Root)||physicalDisplayActualFonts!=canvas.TextLayout.ShowActualDatFonts||physicalDisplayHidden!=String.Join(",",canvas.DisappearingStaffs.OrderBy(n=>n)))){InvalidateDisplayStorage();return;}
  if(!restart&&physicalDisplayHasLoad&&String.IsNullOrEmpty(current)){InvalidateDisplayStorage();return;}
  if(restart&&physicalDisplayHasLoad){
   var lastItemPoint=physicalDisplayStorage.RetainedLastItemPoint();
   if(lastItemPoint.HasValue)canvas.RetainedRestartPoint=lastItemPoint;
   // Confirmed DOS RS empties the active range while retaining both banks.
   physicalDisplayLast=physicalDisplayStorage.Restart();
   physicalDisplayMusic=score.Encode();
   return;
  }
  var timer=Stopwatch.StartNew();
  try{using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout})physicalDisplayLast=physicalDisplayStorage.Produce(probe);physicalDisplayLibrary=canvas.Library;physicalDisplayLibraryRevision=NativeLibraryLocation.RevisionFor(canvas.Library.Root);physicalDisplayActualFonts=canvas.TextLayout.ShowActualDatFonts;physicalDisplayHidden=String.Join(",",canvas.DisappearingStaffs.OrderBy(n=>n));physicalDisplayMusic=score.Encode();physicalDisplayHasLoad=true;}
  catch(Exception error){if(!(error is NotSupportedException||error is IOException||error is InvalidDataException||error is ArgumentException||error is OverflowException))throw;InvalidateDisplayStorage();}
  finally{physicalDisplayLoadMilliseconds=timer.Elapsed.TotalMilliseconds;}
 }
 bool CanTrackDisplayRedraw(){
  return TrackedDisplayArray()!=null&&DisplayResourcesMatch()&&!canvas.DisappearingStaffs.Any();
 }
 void ObserveInputDisplay(bool setup){
  if(musicInput==null||!musicInput.TrackDisplay)return;
  // The setup checkpoint clears the tracking flag but leaves the banks
  // intact. TrackDisplay was captured before that checkpoint.
  if((!setup&&!physicalDisplayKnown)||!DisplayResourcesMatch()){musicInput.TrackDisplay=false;return;}
  try{
   var displayScore=score.Clone();
   // F2 normalizes reference positions in the parameter bank while retaining
   // their old drawing coordinates until Main next rebuilds the display.
   foreach(var position in musicInput.ReferenceDisplayPositions)
    displayScore.Items[position.Key].Set(3,position.Value);
   using(var probe=new Canvas{Score=displayScore,Library=canvas.Library,TextLayout=canvas.TextLayout}){
    physicalDisplayLast=physicalDisplayStorage.Produce(probe);
    if(setup)physicalDisplayStorage.WriteInputStaffGuides(probe,inputSetup.Staff,inputSetup.LeftPosition);
    else physicalDisplayStorage.WriteInputScratch(canvas.InputNumberPoints);
   }
   physicalDisplayMusic=score.Encode();
   physicalDisplayKnown=true;
   physicalDisplayReferenceRedrawPending=musicInput.ReferenceDisplayPositions.Count!=0;
  }catch(Exception error){if(!DisplayScanFailure(error))throw;musicInput.TrackDisplay=false;InvalidateDisplayStorage();}
 }
 void ReplayInputDisplayPreviews(IEnumerable<List<float[]>> previews){
  // A replay visits the pitch and rhythm drawings before producing the
  // finished staff. Those writes can survive in later marker Y words.
  if(!physicalDisplayKnown||!DisplayResourcesMatch()){InvalidateDisplayStorage();return;}
  try{
   using(var probe=new Canvas{Library=canvas.Library,TextLayout=canvas.TextLayout}){
    foreach(var preview in previews){
     var displayScore=score.Clone();
     displayScore.Items=preview.Select(Item.Make).ToList();
     probe.Score=displayScore;
     physicalDisplayStorage.Produce(probe);
    }
   }
  }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
 }
 void RebuildTrackedDisplay(System.Collections.Generic.IEnumerable<int> centeredRests=null){
  if(!DisplayResourcesMatch())return;
  try{
   using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout}){
    physicalDisplayLast=physicalDisplayStorage.Produce(probe);
    if(centeredRests!=null)physicalDisplayStorage.WriteCenteredRestPreviews(probe,centeredRests);
   }
   physicalDisplayMusic=score.Encode();
   physicalDisplayKnown=true;
   physicalDisplayReferenceRedrawPending=false;
  }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
 }
 void ReplayTrackedDisplayReplacements(System.Collections.Generic.IEnumerable<int> ordinals,int code,bool redraw){
  if(!DisplayResourcesMatch())return;
  try{
   using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout}){
    foreach(int ordinal in ordinals)physicalDisplayStorage.CommitItem(probe,ordinal,code);
    physicalDisplayLast=redraw?physicalDisplayStorage.Produce(probe):physicalDisplayStorage.ReadActive();
   }
   physicalDisplayMusic=score.Encode();
   physicalDisplayKnown=true;
  }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
 }
 bool PrepareTrackedSeparateDisplay(Score sorted){
  if(!CanTrackDisplayRedraw())return false;
  try{
   using(var probe=new Canvas{Score=sorted.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout})
    physicalDisplayLast=physicalDisplayStorage.Produce(probe);
   return true;
  }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();return false;}
 }
 bool PrepareTrackedItemReorder(int from,int to){
  if(!CanTrackDisplayRedraw())return false;
  try{
   using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout})
    physicalDisplayStorage.ReorderItem(probe,from,to);
   return true;
  }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();return false;}
 }
 void RecordUnchangedTrackedDisplay(){
  physicalDisplayLast=physicalDisplayStorage.ReadActive();
  physicalDisplayMusic=score.Encode();
  physicalDisplayKnown=true;
 }
 // Original 14DF:0C11..0CB4 writes the post-transposition display and
 // leaves unused words alone when the stem-normalization prompt is declined.
 // The retained DAT producer gives the same full banks for ordinary pitch
 // changes and the measured single-sharp insertion.
 void ObserveDisplayTranspose(Score before,Score normalizationInput=null,int normalizedStaff=0,bool retainedLastWord=false){
  if(before==null||!DisplayResourcesMatch()||before.Items.Count!=score.Items.Count)return;
  bool normalized=normalizationInput!=null;
  if(normalized&&(normalizationInput.Items.Count!=before.Items.Count||
     before.Items.Any(i=>i.Code!=1&&i.Code!=2&&i.Code!=8)))return;
  bool changed=false;
  for(int n=0;n<score.Items.Count;n++){
   var old=before.Items[n];var item=normalized?normalizationInput.Items[n]:score.Items[n];
   if(old.Bytes.SequenceEqual(item.Bytes))continue;
   if(!NativeRetainedDisplayArray.SupportsNote(old)||item.Code!=1||old.Count!=item.Count||
      old.Staff!=item.Staff||old.P(5)!=item.P(5)&&!(old.P(5)==10&&item.P(5)==11)||
      Math.Abs(item.P(4)-old.P(4))!=1||
      before.Items.Count(i=>i.Code==8&&i.Staff==item.Staff&&i.P(5)==1)!=1)return;
   for(int p=1;p<=old.Count;p++)if(p!=4&&p!=5&&old.P(p)!=item.P(p))return;
   changed=true;
  }
  if(!changed)return;
  if(normalized){
   for(int n=0;n<score.Items.Count;n++){
    if(retainedLastWord&&n==score.Items.Count-1)continue;
    var previous=normalizationInput.Items[n];var item=score.Items[n];
    if(previous.Bytes.SequenceEqual(item.Bytes))continue;
    if(item.Code!=1||previous.Code!=1||item.Staff!=normalizedStaff||previous.Staff!=normalizedStaff||
       !NativeRetainedDisplayArray.SupportsNote(before.Items[n])||(item.P(5)!=10&&item.P(5)!=20)||
       item.P(8)!=0||item.P(9)!=0&&item.P(9)!=previous.P(9)||item.Count>11)return;
    for(int p=1;p<=Math.Max(item.Count,previous.Count);p++)
     if(p!=5&&p!=8&&p!=9&&previous.P(p)!=item.P(p))return;
   }
   if(normalizationInput.Items.Any(i=>i.Staff==normalizedStaff&&i.Code==1&&
      (!NativeRetainedDisplayArray.SupportsNote(i)||i.P(3)<-10||i.P(3)>200)))return;
   if(!physicalDisplayStorage.WriteSingleNoteNormalizationScratch(normalizationInput,normalizedStaff))return;
  }
  RebuildTrackedDisplay();
 }
 NativeFreshDisplayArray TrackedDisplayArray(){
  if(!physicalDisplayKnown||physicalDisplayLast==null||physicalDisplayNotePending)return null;
  // SH/SHX and DP change what redraw shows without rewriting the retained
  // arrays. Producers such as editing still require matching display state.
  if(physicalDisplayLibrary!=canvas.Library||physicalDisplayLibraryRevision!=NativeLibraryLocation.RevisionFor(canvas.Library.Root)||!physicalDisplayMusic.SequenceEqual(score.Encode())){InvalidateDisplayStorage();return null;}
  return physicalDisplayLast;
 }
}}
