using System;using System.Windows.Forms;
namespace ScoreModern {public sealed partial class Editor {
 string lastSharedLine="";
 void RecordSharedLine(string value){if(musicInput!=null||originalTextSetupActive||originalGroupFlipQuestion||mode==ScreenMode.EditGroupMenu&&groupCommand==null)return;lastSharedLine=value;((NativeCommandBox)command).ScoreInsert=false;}
 bool SharedReaderKey(Keys keys){
  if((keys&Keys.Modifiers)!=Keys.Control||((keys&Keys.KeyCode)!=Keys.Q&&((keys&Keys.KeyCode)!=Keys.R||!NativeKeyboardOptions.RecallR)))return false;
  if(originalGroupFlipQuestion||mode==ScreenMode.EditGroupMenu&&groupCommand==null||musicInput!=null||inlineTextActive||originalTextSetupActive||parameters.EditingValue||setupEditField!=0||letterHelpPages!=null||mode==ScreenMode.Print||pendingEditAcknowledgement||pendingLetterSave!=null||pendingLetterLoad!=null||pendingDiskDelete!=null||pendingDriveChange||commandFileStage==CommandFileStage.Run||!command.Enabled)return false;
  // Original Main Ctrl+Q restores the previous physical line only at its start.
  // A submitted blank line replaces that history with an empty line.
  if(command.TextLength==0){command.Text=lastSharedLine;command.SelectionStart=command.TextLength;command.Focus();}return true;
 }
}}
