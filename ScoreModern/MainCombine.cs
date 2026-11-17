using System;using System.IO;using System.Linq;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  string combineFirstPath;
  bool ChooseCombineInput(){
   if(!NativeDiskDialogChoice.Files||originalCommandFileQueue!=null)return false;
   string chosen=ChooseCommandFile("Open first score to combine",false,ScoreFileFilter);
   if(chosen==null){ClearMainGroup();Message("");}
   else SubmitMainCombine(chosen);
   return true;
  }
  void BeginMainCombine(){groupCommand="COM";layoutPrompt=dirty&&score.Items.Count>0?LayoutPrompt.Discard:LayoutPrompt.FileName;if(layoutPrompt==LayoutPrompt.FileName&&ChooseCombineInput())return;Message(layoutPrompt==LayoutPrompt.Discard?"Warning: Unsaved Changes will be lost.\nLoad new file? (Y/N)":"Enter 1st input name.ext ->");}
  void SubmitMainCombine(string text){
   if(layoutPrompt==LayoutPrompt.Discard){if(!text.Trim().Equals("Y",StringComparison.OrdinalIgnoreCase)){ClearMainGroup();Message("");return;}layoutPrompt=LayoutPrompt.FileName;if(ChooseCombineInput())return;Message("Enter 1st input name.ext ->");return;}
   if(layoutPrompt==LayoutPrompt.FileName){if(String.IsNullOrWhiteSpace(text))throw new NotSupportedException("Original COM blank first-name behavior remains under verification.");combineFirstPath=Resolve(text.Trim(),Directory.GetCurrentDirectory());layoutPrompt=LayoutPrompt.FileCount;Message("Enter total number of files to combine (\"B\"=backup) ->");return;}
   if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){ClearMainGroup();Message("");return;}var values=LayoutNumbers(text);short count=values.Length==0?(short)0:TransposeWord(values[0].ToString("R",System.Globalization.CultureInfo.InvariantCulture));if(count<1){ClearMainGroup();Message("");return;}if(count>32)throw new NotSupportedException("COM input count beyond the verified group table remains under verification.");
   string path=combineFirstPath;
   var groups=new List<Score>();
   for(int i=0;i<count;i++){
    var group=Score.Load(path);
    if(group.Items.Count==0){ClearMainGroup();throw new NotSupportedException("An empty COM input produces invalid records in the original SCORE; the current document has been preserved.");}
    groups.Add(group);
    if(i+1<count){
     // SCOR4 COM advances A.MUS to B.MUS even when the character after the stem is a period.
     string name=OriginalFilenameSequence.Step(Path.GetFileNameWithoutExtension(path),Path.GetExtension(path),true,false,scor4Interface?'A':(char?)null);
     path=Path.Combine(Path.GetDirectoryName(path),name);
    }
   }
   // The original append reader converts text in the loaded record stream.
   var loaded=new Score{Items=new List<Item>(),Trailer=groups[groups.Count-1].Trailer};
   foreach(var group in groups){
    // A later file's count word overwrites any earlier text terminator at
    // the append boundary. Keep the shared staff context for width refresh.
    var first=group.Items[0];
    var countByte=first.Bytes[0];
    bool appending=loaded.Items.Count!=0;
    loaded.Items.AddRange(group.Items);
    NativeTextCache.Refresh(loaded,canvas.TextLayout);
    if(appending)first.Bytes[0]=countByte;
   }
   int cursor=0;
   for(int i=0;i<groups.Count;i++){
    int n=groups[i].Items.Count;
    groups[i].Items=loaded.Items.Skip(cursor).Take(n).Select(item=>item.Clone()).ToList();
    cursor+=n;
   }
   var combined=NativeCombine.Project(groups,NativeMeasurements.Get(groups[groups.Count-1])==1);var rows=combined.Items.Select(TransposeRow).ToList();int[] order=InputTranspositionOrder.OrderForMain(rows,false);combined.Items=order.Select(i=>combined.Items[i]).ToList();foreach(var item in combined.Items.Where(item=>item.Code==16).ToArray())NativeTextTerminator.Refresh(combined,item);
   bool trackDisplay=CanTrackDisplayRedraw();
   Checkpoint();score=combined;originalLayoutMarksModified=true;current=path;score.SourcePath=path;dirty=true;ClearMainGroup();RefreshAll(null);
   if(trackDisplay)RebuildTrackedDisplay();
   Message("*** Sorting by staff number.");
  }
 }
}
