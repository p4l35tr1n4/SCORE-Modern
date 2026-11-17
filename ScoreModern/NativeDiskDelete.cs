using System;using System.IO;using System.Windows.Forms;
namespace ScoreModern {public sealed partial class Editor {
 string pendingDiskDelete;
 void StartDiskDelete(string name){if(String.IsNullOrWhiteSpace(name))throw new InvalidDataException("Enter the exact filename to delete.");pendingDiskDelete=NativeOutputLocation.RequireWritableDestination(source,Path.GetFullPath(name));Message("Delete file "+name+"\r\nIs that correct? (Y/N)");}
 bool SubmitDiskDelete(string text){if(pendingDiskDelete==null)return false;if(text.Equals("Y",StringComparison.OrdinalIgnoreCase)){string path=pendingDiskDelete;pendingDiskDelete=null;File.Delete(path);command.Clear();Message("");}else if(text.Equals("N",StringComparison.OrdinalIgnoreCase)||text=="\u001b"){pendingDiskDelete=null;command.Clear();Message("");}return true;}
 bool DiskDeleteKey(KeyEventArgs key){if(pendingDiskDelete==null)return false;if(key.KeyCode==Keys.Escape){pendingDiskDelete=null;command.Clear();Message("");key.SuppressKeyPress=true;return true;}if(key.Control||key.Alt||key.KeyCode>=Keys.F1&&key.KeyCode<=Keys.F24){key.SuppressKeyPress=true;return true;}return false;}
}}
