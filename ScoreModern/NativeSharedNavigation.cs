using System;using System.Globalization;using System.Windows.Forms;
namespace ScoreModern { public sealed partial class Editor {
 bool SharedNavigationKey(Keys keys){
  if(pendingFilteredEdit!=null||musicInput!=null||originalTextDraft!=null||originalTextSetupActive||inlineTextActive||letterHelpPages!=null||canvas.BlockItemEditing||selectionPending||groupCommand!=null||pendingDiskFilename!=null||pendingDriveChange||pendingLetterLoad!=null||pendingLetterSave!=null||pendingDiskDelete!=null||drawExportStage!=DrawExportStage.None||pendingParameterExport||pendingAppendPath!=null||commandFileStage!=CommandFileStage.None||commandFileInputQuestion||tabCommandStage!=TabCommandStage.None||pageHeightStage!=PageHeightStage.None)return false;
  bool itemEdit=mode==ScreenMode.Edit;
  if(itemEdit&&(originalItemZero||parameters.EditingValue||setupEditField!=0||pendingEditAcknowledgement||externalDrawingNameItem!=null))return false;
  if(mode!=ScreenMode.Main&&mode!=ScreenMode.EditMenu&&mode!=ScreenMode.Disk&&mode!=ScreenMode.EditGroupMenu&&!itemEdit)return false;
  var code=keys&Keys.KeyCode;var modifiers=keys&Keys.Modifiers;
  if(scor4Interface&&mode==ScreenMode.Main&&modifiers==(Keys.Control|Keys.Alt)&&(code==Keys.PageUp||code==Keys.PageDown)){
   ZoomScor4Screen(code==Keys.PageUp);return true;
  }
  if(modifiers==Keys.Control&&(code==Keys.PageUp||code==Keys.PageDown)){
   if(itemEdit&&(canvas.EditModified||canvas.Selected==null))return true;
   float old=NativeDosViewport.ZoomFactor,next;
   // Live original0F4F:144F..1560: 1/4 above one, 1/8 below;
   // the keyboard path supplies horizontal percent50 below one.
   if(code==Keys.PageUp){if(old>=16)return true;next=(float)((double)old+(old<1?.125:.25));}
   else{if(old<=.20000000298023224f)return true;next=(float)((double)old-(old>1?.25:.125));}
   if(mode!=ScreenMode.Main&&mode!=ScreenMode.EditGroupMenu){if(!itemEdit&&score.Items.Count>0)NativeDosViewport.Remember(score.Items[Math.Max(0,Math.Min(lastEditedIndex,score.Items.Count-1))]);ApplyMouseCamera(()=>NativeDosViewport.Center(canvas,next));}else ApplyMouseCamera(()=>NativeDosViewport.Command(canvas,"Z",next.ToString("R",CultureInfo.InvariantCulture)+(next<1?" 50 0":" 0 0")));return true;
  }
  if(itemEdit)return false;
  string direction=null;float percent=0;
  if(modifiers==Keys.Control){if(code==Keys.Left){direction="ZL";percent=10;}else if(code==Keys.Right){direction="ZR";percent=10;}}
  else if(modifiers==Keys.None){switch(code){case Keys.PageUp:direction="ZU";percent=50;break;case Keys.PageDown:direction="ZD";percent=50;break;case Keys.Up:direction="ZU";percent=NativeKeyboardOptions.MainArrowPercent;break;case Keys.Down:direction="ZD";percent=NativeKeyboardOptions.MainArrowPercent;break;case Keys.F9:direction="ZL";percent=40;break;case Keys.F10:direction="ZR";percent=40;break;case Keys.Home:if(NativeKeyboardOptions.ClearHome)ClearCommandTextWindow();else ApplyMouseCamera(()=>NativeDosViewport.Top(canvas));return true;case Keys.End:ApplyMouseCamera(()=>NativeDosViewport.Command(canvas,"Z","1"));return true;}}
  if(direction!=null){ApplyMouseCamera(()=>NativeDosViewport.Pan(direction,percent));return true;}
  if(mode==ScreenMode.Main&&code==Keys.Oemtilde&&(modifiers==Keys.None||modifiers==Keys.Shift)){if(score.Items.Count!=0)ApplyMouseCamera(()=>NativeDosViewport.Center(canvas,1));return true;}
  return false;
 }
}}
