using System;using System.IO;using System.Linq;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  short[] decombineBoundaries;List<Score> decombineOutputs;int decombineOutputIndex;string decombineOutputPath;bool decombineOverwrite;
  const string DecombineGroupsPrompt="*** Sorting by staff number.\nType number of staves in each group to be processed.\n(Count from bottom up.  Include all unused staff numbers.)";
  bool ChooseDecombineOutput(){
   if(!NativeDiskDialogChoice.Files||originalCommandFileQueue!=null)return false;
   string chosen=ChooseCommandFile("Save top staff group",true,ScoreFileFilter,"",false);
   SubmitMainDecombine(chosen??"-");
   return true;
  }
  void BeginMainDecombine(){
   if(score.Items.Count<2){ClearMainGroup();Message(score.Items.Count==1?"*** Sorting by staff number.":"");return;}
   var rows=score.Items.Select(TransposeRow).ToList();int[] order=InputTranspositionOrder.OrderForMain(rows,false);var next=order.Select(i=>score.Items[i].Clone()).ToList();
   if(next.Count>=2){
    bool keepDisplay=order.SequenceEqual(Enumerable.Range(0,order.Length))&&CanTrackDisplayRedraw();
    Checkpoint();score.Items=next;originalLayoutMarksModified=true;dirty=true;RefreshTransposeTextTerminators();
    // DEC reads the current parameters into its output projection. An
    // already sorted source retains its current display and pointer banks.
    if(keepDisplay)RecordUnchangedTrackedDisplay();
    RefreshAll(null);
   }
   int maximum=score.Items.Where(item=>item.Code==8).Select(item=>item.Staff).DefaultIfEmpty(0).Max();if(maximum<1){ClearMainGroup();Message("");return;}if(maximum==1){groupCommand="DEC";decombineBoundaries=new short[]{1};decombineOutputs=NativeDecombine.Project(score,decombineBoundaries);decombineOverwrite=false;layoutPrompt=LayoutPrompt.Output;if(ChooseDecombineOutput())return;Message("Enter output name for top staff group (\"-\" = backup) ->");return;}groupCommand="DEC";layoutPrompt=LayoutPrompt.Groups;decombineOutputs=null;decombineOverwrite=false;Message(DecombineGroupsPrompt);
  }
  void SubmitMainDecombine(string text){
   if(layoutPrompt==LayoutPrompt.Groups){if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)||String.IsNullOrWhiteSpace(text)){ClearMainGroup();Message("");return;}var sizes=LayoutNumbers(text);if(sizes.Length>20)throw new NotSupportedException("Original DEC group argument capacity exceeded.");int maximum=score.Items.Where(i=>i.Code==8).Select(i=>i.Staff).DefaultIfEmpty(0).Max();decombineBoundaries=LayoutDecombineGroups.Boundaries((short)maximum,sizes,0);if(decombineBoundaries.Length==0){ClearMainGroup();Message("");return;}decombineOutputs=NativeDecombine.Project(score,decombineBoundaries);layoutPrompt=LayoutPrompt.Output;if(ChooseDecombineOutput())return;Message("Enter output name for top staff group (\"-\" = backup) ->");return;}
   if(layoutPrompt==LayoutPrompt.Continue){ClearMainGroup();RefreshAll(null);Message("");return;}
   if(decombineOverwrite){string answer=text.Trim();if(answer.Equals("N",StringComparison.OrdinalIgnoreCase)){ClearMainGroup();Message("");return;}if(!answer.Equals("Y",StringComparison.OrdinalIgnoreCase)){Message("Write over it? (Y/N) ->");return;}decombineOverwrite=false;WriteDecombineOutput();ContinueDecombineOutputs();return;}
   if(text.Trim()=="-"){decombineOutputs=null;ClearMainGroup();Message("");return;}if(String.IsNullOrWhiteSpace(text)){if(!NativeEditCompatibility.ReproduceInvalidStates){decombineOutputs=null;ClearMainGroup();throw new NotSupportedException("Enter an output filename. Original invalid-state compatibility permits SCORE's extension-only files for a blank name.");}text=".MUS";}string resolved=Resolve(text.Trim(),Directory.GetCurrentDirectory());if(Path.GetFileNameWithoutExtension(resolved).Length==0&&!NativeEditCompatibility.ReproduceInvalidStates){decombineOutputs=null;ClearMainGroup();throw new NotSupportedException("Extension-only DEC filenames require original invalid-state compatibility.");}decombineOutputPath=NativeOutputLocation.RequireWritableDestination(source,resolved);decombineOutputIndex=0;ContinueDecombineOutputs();
  }
  void WriteDecombineOutput(){byte[] bytes=OriginalMusSave.Prepare(decombineOutputs[decombineOutputIndex],source).Encode();ScoreFileStorage.Write(decombineOutputPath,bytes);decombineOutputIndex++;if(decombineOutputIndex<decombineOutputs.Count){string stem=Path.GetFileNameWithoutExtension(decombineOutputPath),extension=Path.GetExtension(decombineOutputPath);string next=stem.Length==0?"."+Path.GetFileNameWithoutExtension(OriginalFilenameSequence.Step(extension.Substring(1),".MUS",true,false)):OriginalFilenameSequence.Step(stem,extension,true,false);decombineOutputPath=NativeOutputLocation.RequireWritableDestination(source,Path.Combine(Path.GetDirectoryName(decombineOutputPath),next));}}
  void ContinueDecombineOutputs(){while(decombineOutputIndex<decombineOutputs.Count){if(File.Exists(decombineOutputPath)){decombineOverwrite=true;Message(decombineOutputPath+Environment.NewLine+"Write over it? (Y/N) ->");return;}WriteDecombineOutput();}layoutPrompt=LayoutPrompt.Continue;RefreshAll(null);Message("Press any key to continue.");}
 }
}
