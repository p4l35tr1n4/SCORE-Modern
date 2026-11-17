using System;using System.Linq;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  Score separateWorking;short separateBottom,separateTop;float separateLeft,separateRight;bool separateBottomChoice,separateNormalize,separateDisplayPrepared;
  const string SeparateRangePrompt="Type bottom staff number, left pos., right pos., top staff num. --";
  void BackoutMainSeparate(){
   var sorted=separateWorking;separateWorking=null;ClearMainGroup();
   if(sorted!=null&&InputTranspositionOrder.MarksModified(sorted.Items.Count,false)){
    bool trackDisplay=separateDisplayPrepared&&CanTrackDisplayRedraw();
    Checkpoint();score.Items=sorted.Items;originalLayoutMarksModified=true;dirty=true;RefreshTransposeTextTerminators();
    separateDisplayPrepared=false;
    if(trackDisplay)RecordUnchangedTrackedDisplay();
    RefreshAll(null);
   }
   Message("");
  }
  void SubmitMainSeparate(string text){
   if(layoutPrompt==LayoutPrompt.Range){if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){ClearMainGroup();Message("");return;}var p=LayoutNumbers(text);if(p.Length>20)throw new NotSupportedException("Original SEP argument capacity exceeded.");separateBottom=p.Length>0?TransposeWord(p[0].ToString("R",System.Globalization.CultureInfo.InvariantCulture)):(short)0;separateTop=p.Length>3?TransposeWord(p[3].ToString("R",System.Globalization.CultureInfo.InvariantCulture)):(short)0;if(separateTop<separateBottom)separateTop=separateBottom;if(separateBottom>32){separateBottom=1;separateTop=32;}if(separateBottom<1||separateTop>32)throw new NotSupportedException("Original SEP staff scope is outside the verified table.");separateLeft=p.Length>1?p[1]:0;separateRight=p.Length>2&&BitConverter.ToInt32(BitConverter.GetBytes(p[2]),0)!=0?p[2]:220;var rows=score.Items.Select(TransposeRow).ToList();int[] order=InputTranspositionOrder.OrderForMain(rows,false);separateWorking=score.Clone();separateWorking.Items=order.Select(i=>score.Items[i].Clone()).ToList();LayoutSeparateParts.Validate(separateWorking.Items.Select(TransposeRow).ToList(),separateBottom,separateTop,separateLeft,separateRight);
    separateDisplayPrepared=PrepareTrackedSeparateDisplay(separateWorking);
    layoutPrompt=LayoutPrompt.Part;Message("*** Sorting by staff number.\nT = delete top notes, B = delete bottom notes ->");return;}
   if(layoutPrompt==LayoutPrompt.Part){separateBottomChoice=text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase);layoutPrompt=LayoutPrompt.Normalize;Message("Normalize stem directions. Y/N ? [Enter=Y] ->");return;}
   if(layoutPrompt==LayoutPrompt.Normalize){separateNormalize=!text.Trim().Equals("N",StringComparison.OrdinalIgnoreCase);layoutPrompt=LayoutPrompt.Confirm;Message(GroupDeletePrompt);return;}
   if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){BackoutMainSeparate();return;}if(!String.IsNullOrWhiteSpace(text))throw new NotSupportedException("Only the measured Enter/B SEP confirmation is implemented.");
   var originalItems=separateWorking.Items;var result=LayoutSeparateParts.Apply(originalItems.Select(TransposeRow).ToList(),separateBottom,separateTop,separateLeft,separateRight,separateBottomChoice);var next=new List<Item>();for(int i=0;i<result.Rows.Count;i++){var item=Item.Make(result.Rows[i]);var original=originalItems[result.Origins[i]];if(item.Count==original.Count)Array.Copy(original.Bytes,0,item.Bytes,0,4);next.Add(item);}separateWorking.Items=next;
   bool trackedNormalization=separateNormalize&&result.ChangedStaffs.All(staff=>SupportedSeparateDisplayNormalization(next,staff));
   bool trackDisplay=separateDisplayPrepared&&(!separateNormalize||trackedNormalization)&&CanTrackDisplayRedraw();
   try{if(separateNormalize&&result.ChangedStaffs.Count>0){
    if(separateWorking.Items.Any(item=>item.Code==1&&result.ChangedStaffs.Contains((short)item.Staff)&&item.P(3)>=0&&item.P(3)<=220&&item.Count>=10&&BitConverter.ToInt32(BitConverter.GetBytes(item.P(10)),0)!=0))throw new NotSupportedException("SCORE reports: WILL NOT FLIP OFFSET NOTE. SEP normalization is refused without publishing a partial edit.");
    NormalizeSeparateCandidate(result.ChangedStaffs);
   }}catch{
    if(separateDisplayPrepared)InvalidateDisplayStorage();
    separateDisplayPrepared=false;separateWorking=null;ClearMainGroup();throw;
   }
   trackDisplay&=separateWorking.Items.Count==next.Count;
   Checkpoint();score.Items=separateWorking.Items;originalLayoutMarksModified|=InputTranspositionOrder.MarksModified(score.Items.Count,false);dirty=true;separateWorking=null;separateDisplayPrepared=false;ClearMainGroup();RefreshTransposeTextTerminators();RefreshAll(null);
   if(trackDisplay){
    // SEP compacts the sorted display before the full redraw.
    var live=originalItems.ToList();
    var survivors=result.Origins.Select((origin,index)=>new{origin,item=next[index]}).ToDictionary(pair=>pair.origin,pair=>pair.item);
    int removed=0;
    foreach(var staff in originalItems.Select(item=>item.Staff).Distinct()){
     for(int index=0;index<originalItems.Count;index++){
      if(originalItems[index].Staff!=staff)continue;
      Item survivor;
      if(survivors.TryGetValue(index,out survivor)){live[index-removed]=survivor;continue;}
      physicalDisplayStorage.DeleteItem(index+1-removed);
      live.RemoveAt(index-removed);
      removed++;
     }
     if(trackedNormalization&&result.ChangedStaffs.Contains((short)staff))
      physicalDisplayStorage.WriteSeparatedChordNormalizationScratch(live,staff);
    }
    RebuildTrackedDisplay();
   }
   Message("");
  }
  static bool SupportedSeparateDisplayNormalization(List<Item> rows,int staff){
   if(rows.Any(item=>item.Staff==staff&&(item.Code==5||item.Code==6)))return false;
   var notes=rows.Where(item=>item.Staff==staff&&item.Code==1&&item.P(3)>=0&&item.P(3)<=220).ToArray();
   return notes.Length<=5&&notes.Length>0&&notes.All(item=>item.P(7)==1&&item.P(6)==0&&item.P(9)==0&&item.Count<=9)&&
    notes.GroupBy(item=>item.P(3)).All(chord=>chord.Count()<=2&&chord.Count(item=>item.P(5)==10||item.P(5)==20)==1&&
     chord.All(item=>item.P(5)==0||item.P(5)==10||item.P(5)==20));
  }
  void NormalizeSeparateCandidate(IList<short> staffs){var priorScore=score;string priorGroup=groupCommand;var priorRange=groupRange;bool priorDirty=dirty;int depth=undo.Count;var priorRedo=redo.ToArray();try{score=separateWorking;foreach(short staff in staffs){groupRange=new float[]{staff,0,220,0,staff};groupCommand=EnharmonicNormalize;CommitOriginalFlip("FLN",true);}separateWorking=score;}finally{while(undo.Count>depth)undo.Pop();redo.Clear();for(int i=priorRedo.Length-1;i>=0;i--)redo.Push(priorRedo[i]);score=priorScore;groupCommand=priorGroup;groupRange=priorRange;dirty=priorDirty;RefreshAll(null);}}
 }
}

