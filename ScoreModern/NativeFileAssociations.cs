using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScoreModern {
 internal static class NativeFileAssociations {
  const string TypeDescription="SCORE Notation File";
  const string Id="ScoreModern.Score";
  const string Classes=@"Software\Classes";
  internal static string OpenCommand(string executable){return "\""+Path.GetFullPath(executable)+"\" --open \"%1\"";}

  internal static string[] ParseExtensions(string text){
   var result=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
   foreach(string token in Regex.Split(text.Trim(),@"[\s,;]+")){
    if(token.Length==0)continue;
    string extension=token.StartsWith(".")?token:"."+token;
    if(!Regex.IsMatch(extension,@"\A\.[A-Za-z0-9][A-Za-z0-9_-]{0,31}\z"))
     throw new ArgumentException("Enter file extensions such as .mus, .pag or .score.");
    if(new[]{".exe",".com",".bat",".cmd",".msi",".ps1",".vbs",".js",".lnk",".scr",".dll"}.Contains(extension,StringComparer.OrdinalIgnoreCase))
     throw new ArgumentException(extension+" is reserved for Windows programs or scripts.");
    result.Add(extension.ToLowerInvariant());
   }
   return result.OrderBy(extension=>extension,StringComparer.OrdinalIgnoreCase).ToArray();
  }

  static string AppKey(string executable){return @"Applications\"+Path.GetFileName(executable);}
  static string RegisteredCommand(RegistryKey classes,string name){
   using(var command=classes.OpenSubKey(name+@"\shell\open\command"))return command==null?null:command.GetValue("") as string;
  }
  internal static string[] Read(RegistryKey classes,string executable){
   if(classes==null||!String.Equals(RegisteredCommand(classes,AppKey(executable)),OpenCommand(executable),StringComparison.OrdinalIgnoreCase))
    return new string[0];
   using(var types=classes.OpenSubKey(AppKey(executable)+@"\SupportedTypes"))
    return types==null?new string[0]:types.GetValueNames().Where(name=>name.StartsWith(".")).OrderBy(name=>name,StringComparer.OrdinalIgnoreCase).ToArray();
  }

  internal static void Apply(RegistryKey classes,string executable,string[] extensions){
   var selected=ParseExtensions(String.Join(" ",extensions));
   string app=AppKey(executable),open=OpenCommand(executable);
   string documentIcon=InstallDocumentIcon();
   var previous=Read(classes,executable);
   using(var key=classes.CreateSubKey(Id)){
    SetTypeDescription(key);
    using(var icon=key.CreateSubKey("DefaultIcon"))icon.SetValue("",documentIcon);
    using(var command=key.CreateSubKey(@"shell\open\command"))command.SetValue("",open);
   }
   using(var key=classes.CreateSubKey(app)){
    SetTypeDescription(key);
    key.SetValue("FriendlyAppName","SCORE Modern");
    using(var icon=key.CreateSubKey("DefaultIcon"))icon.SetValue("",documentIcon);
    using(var command=key.CreateSubKey(@"shell\open\command"))command.SetValue("",open);
    using(var types=key.CreateSubKey("SupportedTypes")){
     foreach(string extension in types.GetValueNames().Except(selected,StringComparer.OrdinalIgnoreCase))types.DeleteValue(extension,false);
     foreach(string extension in selected)types.SetValue(extension,"");
    }
   }
   foreach(string extension in previous.Except(selected,StringComparer.OrdinalIgnoreCase))
    using(var key=classes.OpenSubKey(extension+@"\OpenWithProgids",true))
     if(key!=null)key.DeleteValue(Id,false);
   foreach(string extension in selected)
    using(var key=classes.CreateSubKey(extension+@"\OpenWithProgids"))
     key.SetValue(Id,new byte[0],RegistryValueKind.None);
  }

  static void SetTypeDescription(RegistryKey key){key.SetValue("",TypeDescription);key.SetValue("FriendlyTypeName",TypeDescription);}

  static string InstallDocumentIcon(){
   byte[] icon;
   using(var resource=typeof(NativeFileAssociations).Assembly.GetManifestResourceStream("ScoreModern.Document.ico"))
   using(var buffer=new MemoryStream()){
    if(resource==null)throw new InvalidDataException("The SCORE Modern document icon is missing.");
    resource.CopyTo(buffer);
    icon=buffer.ToArray();
   }
   string folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"SCORE Modern");
   Directory.CreateDirectory(folder);
   string path=Path.Combine(folder,"SM-file-icon.ico");
   if(!File.Exists(path)||!File.ReadAllBytes(path).SequenceEqual(icon))ScoreFileStorage.Write(path,icon);
   return "\""+path+"\",0";
  }

  [System.Runtime.InteropServices.DllImport("shell32.dll",EntryPoint="SHChangeNotify",ExactSpelling=true)]
  static extern void Notify(uint events,uint flags,IntPtr first,IntPtr second);

  internal static void Show(Form owner){
   using(var dialog=new ScoreDialog("file-associations")){
    dialog.Text="File associations";
    dialog.StartPosition=FormStartPosition.CenterParent;
    dialog.ClientSize=new Size(600,190);
    var panel=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=1};
    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
    var help=new Label{Text="Choose the file extensions SCORE Modern offers to open for your Windows account. Set your preferred defaults in Windows Default apps.",Dock=DockStyle.Fill,AutoSize=true};
    var label=new Label{Text="Extensions (separate with spaces or commas)",AutoSize=true};
    var input=new TextBox{Dock=DockStyle.Top,AccessibleName="File extensions"};
    string executable=Application.ExecutablePath;
    using(var classes=Registry.CurrentUser.OpenSubKey(Classes)){
     var registered=Read(classes,executable);
     input.Text=registered.Length==0&&
      (classes==null||RegisteredCommand(classes,AppKey(executable))==null)?".mus, .pag":String.Join(", ",registered);
    }
    var buttons=new FlowLayoutPanel{Dock=DockStyle.Fill,AutoSize=true};
    var apply=new Button{Text="Apply",AutoSize=true};
    apply.Click+=(s,e)=>{
     try{
      var selected=ParseExtensions(input.Text);
      using(var classes=Registry.CurrentUser.CreateSubKey(Classes))Apply(classes,executable,selected);
      Notify(0x08000000,0,IntPtr.Zero,IntPtr.Zero);
      input.Text=String.Join(", ",selected);
      MessageBox.Show(dialog,"Open with registrations updated.","SCORE Modern");
     }catch(Exception error){MessageBox.Show(dialog,error.Message,"Registration failed");}
    };
    var defaults=new Button{Text="Windows Default apps...",AutoSize=true};
    defaults.Click+=(s,e)=>{try{Process.Start(new ProcessStartInfo("ms-settings:defaultapps"){UseShellExecute=true});}catch(Exception error){MessageBox.Show(dialog,error.Message);}};
    var close=new Button{Text="Close",DialogResult=DialogResult.Cancel,AutoSize=true};
    buttons.Controls.AddRange(new Control[]{apply,defaults,close});
    panel.Controls.Add(help);panel.Controls.Add(label);panel.Controls.Add(input);panel.Controls.Add(buttons);
    dialog.Controls.Add(panel);
    dialog.AcceptButton=apply;
    dialog.CancelButton=close;
    dialog.ShowDialog(owner);
   }
  }
 }
}
