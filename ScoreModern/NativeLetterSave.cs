using System;using System.IO;using System.Windows.Forms;
namespace ScoreModern {public sealed partial class Editor {
 string pendingLetterSave;
 void StartLetterSave(string name){if(!CanSaveDocument())return;if(String.IsNullOrWhiteSpace(name)&&NativeDiskDialogChoice.Files)name=ChooseWindowsDiskFile("SA");if(String.IsNullOrWhiteSpace(name)){Message("");return;}string path=NativeOutputLocation.RequireWritableDestination(source,Resolve(name,Directory.GetCurrentDirectory()));if(File.Exists(path)){pendingLetterSave=path;Message(path+"       exists.  Write over old file? ->");}else SaveNamed(path,false);}
 bool SubmitLetterSave(string text){if(pendingLetterSave==null)return false;if(text.Equals("Y",StringComparison.OrdinalIgnoreCase)){string path=pendingLetterSave;pendingLetterSave=null;command.Clear();SaveNamed(path,false);}else if(text.Equals("N",StringComparison.OrdinalIgnoreCase)||text=="\u001b"){pendingLetterSave=null;command.Clear();Message("");}return true;}
 bool LetterSaveKey(KeyEventArgs key){if(pendingLetterSave==null)return false;if(key.KeyCode==Keys.Escape){pendingLetterSave=null;command.Clear();Message("");key.SuppressKeyPress=true;return true;}if(key.Control||key.Alt||key.KeyCode>=Keys.F1&&key.KeyCode<=Keys.F24){key.SuppressKeyPress=true;return true;}return false;}
}}
