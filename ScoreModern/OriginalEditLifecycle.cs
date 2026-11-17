using System;
namespace ScoreModern {
 public sealed partial class Editor {
  Item deletedItem;bool replayDeletedNote,deletedNoteWasInterior;bool pendingEditAcknowledgement,originalItemZero;
  void WaitForEditAcknowledgment(){pendingEditAcknowledgement=true;Message("Cannot perform function until done editing current item.");}
  bool AcknowledgeEditPause(){if(AcknowledgeOriginalLyricEmptyWarning())return true;if(!pendingEditAcknowledgement)return false;pendingEditAcknowledgement=false;CommitMarginFontWarning();CommitRippleDeletion();CommitBeamTemporary();Message("");command.Clear();if(!canvas.Focused)command.Focus();ResumeOriginalCommandFileAcknowledgement();return true;}
  static Item NewEditedRecord(Item source){var item=source.Clone();item.TrimEditedNumericParameters();Array.Copy(BitConverter.GetBytes((float)item.Count),item.Bytes,4);return item;}
  enum CopyContinuation { Exit,Last,Copy,Next,Item }
  void CopyEditedItem(bool selectLast=false){CopyEditedItem(selectLast?CopyContinuation.Last:CopyContinuation.Exit);}
  void CopyEditedItem(CopyContinuation continuation,int target=-1){
   if(mode!=ScreenMode.Edit||canvas.Selected==null)return;
   int index=score.Items.IndexOf(canvas.Selected);if(index<0||editOriginal==null)return;bool copied=canvas.EditModified;
   bool enterZero=continuation==CopyContinuation.Next&&copied&&index==0&&direction<0;if(enterZero&&!NativeEditCompatibility.ReproduceInvalidStates)throw new InvalidOperationException("Backward CX before item 1 is prevented. Original invalid-state reproduction is available in the Editing compatibility option.");if(continuation==CopyContinuation.Next){target=copied?NextEditingIndex(index+direction,direction):index;if(copied&&target<0&&direction>0&&(editFilter==null||editFilter.Matches(canvas.Selected)))target=score.Items.Count;if(!enterZero&&(target<0||target>=score.Items.Count+(copied?1:0)))throw new NotSupportedException("CX target is outside the verified item range.");}
   if(continuation==CopyContinuation.Item&&(target<0||target>=score.Items.Count+(copied?1:0)))throw new NotSupportedException("Copy target is outside the existing list and pending copy.");
   Item appended=null,entry=editOriginal.Clone();
   if(copied){
    var selected=canvas.Selected;
    bool continueWithCopy=continuation!=CopyContinuation.Last&&
     (selected.Code==1||TrackedLine(selected)||TrackedTrill(selected)||TrackedClef(selected)||TrackedMeter(selected)||TrackedBarline(selected)||TrackedInvisibleItem(selected));
    bool tracked=!enterZero&&(continuation==CopyContinuation.Exit||continueWithCopy)&&CanPromoteDisplayCopy(selected,index);
    var copy=NewEditedRecord(selected);var original=editOriginal.Clone();Checkpoint();
    score.Items[index]=original;score.Items.Add(copy);appended=copy;dirty=true;
    if(scor4Interface&&score.Items.Count>1)
     NativeTextTerminator.Refresh(score,score.Items[score.Items.Count-2]);
    lastEditedIndex=score.Items.Count-1;canvas.Selected=copy;TrackDisplayLifecycle(tracked,promotePreview:true);
   }
   else lastEditedIndex=index;
   if(enterZero){originalItemZero=true;lastEditedIndex=index;editOriginal=null;editIndex=-1;canvas.EditOriginal=entry;canvas.EditModified=true;canvas.SelectionContextSource=appended;canvas.BlockItemEditing=true;mode=ScreenMode.Edit;RefreshAll(appended.Clone());LayoutClassic();canvas.Focus();return;}
   if(continuation==CopyContinuation.Exit){canvas.EditModified=false;ExitEdit(false);return;}
   if(continuation==CopyContinuation.Last)target=score.Items.Count-1;
   if(continuation==CopyContinuation.Copy)target=copied?score.Items.Count-1:index;
   if(continuation==CopyContinuation.Item)direction=1;
   editOriginal=null;editIndex=-1;canvas.EditModified=false;BeginEdit(score.Items[target]);RefreshAll(canvas.Selected);
  }
  void ExitToItem(int target){if(mode!=ScreenMode.Edit||canvas.Selected==null||target<0||target>=score.Items.Count)throw new NotSupportedException("X-number requires an existing item while editing.");var item=score.Items[target];ExitEdit(false);direction=1;BeginEdit(item);}
  void DeleteEditedItem(){
   if(mode!=ScreenMode.Edit||canvas.Selected==null)return;
   if(canvas.EditModified){WaitForEditAcknowledgment();return;}
   int index=score.Items.IndexOf(canvas.Selected);if(index<0)return;bool tracked=CanTrackDisplayRemoval(canvas.Selected,index,false);var beforeDisplay=physicalDisplayLast;replayDeletedNote=tracked&&canvas.Selected.Code==1;deletedNoteWasInterior=index<score.Items.Count-1;originalRippleDeletedHistory=false;var removed=canvas.Selected.Clone();removed.TrimEditedNumericParameters();Checkpoint();score.Items.RemoveAt(index);deletedItem=removed;dirty=true;editOriginal=null;editIndex=-1;canvas.EditOriginal=null;canvas.EditModified=false;TrackDisplayLifecycle(tracked,index+1);if(replayDeletedNote)NativeDosViewport.DeleteReplay(canvas,beforeDisplay,index+1,physicalDisplayLast);
   int next=NextEditingIndex(index+(direction<0?-1:0),direction);if(next>=0&&next<score.Items.Count){BeginEdit(score.Items[next]);RefreshAll(canvas.Selected);}else{lastEditedIndex=score.Items.Count>0?Math.Min(index,score.Items.Count-1):-1;canvas.Selected=null;ExitEdit(false);}
  }
  void UndeleteItem(){
   if(mode!=ScreenMode.Edit&&mode!=ScreenMode.Main){Message("Undelete in this menu still needs original verification.");return;}
   if(canvas.EditModified){WaitForEditAcknowledgment();return;}if(originalRippleDeletedHistory){if(NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Original UND after whole-line ripple retains inconsistent deleted-buffer data; that history still requires physical-storage verification.");pendingEditAcknowledgement=true;Message("Undelete after whole-line ripple is prevented because SCORE retains inconsistent deleted-buffer data."+Environment.NewLine+"Press any key to continue.");return;}if(deletedItem==null)return;bool tracked=CanTrackDisplayRemoval(deletedItem,-1,true);var restored=NewEditedRecord(deletedItem);Checkpoint();score.Items.Add(restored);lastEditedIndex=score.Items.Count-1;if(mode==ScreenMode.Main)direction=1;dirty=true;editOriginal=null;editIndex=-1;deletedItem=null;TrackDisplayLifecycle(tracked,appendCode:restored.Code);if(tracked&&replayDeletedNote&&restored.Code==1)NativeDosViewport.AppendReplay(canvas,physicalDisplayLast,deletedNoteWasInterior);replayDeletedNote=false;BeginEdit(restored);RefreshAll(restored);
  }
 }
}
