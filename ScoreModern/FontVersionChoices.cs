using System;using System.IO;using System.Linq;using System.Text;using System.Collections.Generic;using System.Security.Cryptography;using System.Text.RegularExpressions;using System.Drawing;using System.Windows.Forms;
namespace ScoreModern {
 public static class FontVersionChoices {
  public static readonly Dictionary<string,string[]> Available=new Dictionary<string,string[]>(StringComparer.Ordinal);
  public static readonly Dictionary<string,string[]> Pending=new Dictionary<string,string[]>(StringComparer.Ordinal);
  static readonly Dictionary<string,string> choices=new Dictionary<string,string>(StringComparer.Ordinal);static bool loaded;
  static string Settings{get{return "font-choices";}}
  static string Key(string name){return NativeTupletFonts.DirectoryPath+"\n"+name;}
  static string Hash(string path){using(var sha=SHA256.Create())return Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path)));}
  internal static string ReadableRows(string value){
   return String.Join("\n",value.Replace("\r\n","\n").Split('\n').Select(row=>{
    string[] fields=row.Split('\t');
    if(fields.Length!=2)return row;
    try{return Encoding.UTF8.GetString(Convert.FromBase64String(fields[0])).Replace('\n','\t')+"\t"+
     Encoding.UTF8.GetString(Convert.FromBase64String(fields[1])).Replace('\n','\t');}
    catch(FormatException){return row;}
   }));
  }
  static void Load(){if(loaded)return;loaded=true;try{
   string value=NativeSettings.Read(Settings);
   if(value==null)return;
   foreach(string row in ReadableRows(value).Split('\n')){
    string[] fields=row.Split('\t');
    if(fields.Length==4)choices[fields[0]+"\n"+fields[1]]=fields[2]+"\n"+fields[3];
   }
  }catch(IOException){}catch(UnauthorizedAccessException){}}
  public static string Selected(string name,IList<string> candidates){Load();string entry;if(!choices.TryGetValue(Key(name),out entry))return null;int split=entry.LastIndexOf('\n');if(split<0)return null;string path=entry.Substring(0,split);return candidates.Contains(path,StringComparer.OrdinalIgnoreCase)&&Hash(path)==entry.Substring(split+1)?path:null;}
  public static void Select(string name,string path){path=Path.GetFullPath(path);Load();choices[Key(name)]=path+"\n"+Hash(path);NativeSettings.WriteAllText(Settings,String.Join("\n",choices.Select(x=>(x.Key+"\n"+x.Value).Replace('\n','\t'))));Pending.Remove(name);NativeTupletFonts.ClearResolvedFonts();}
  static string Literal(string header,string key){var m=Regex.Match(header,@"/"+key+@"\s*\(");if(!m.Success)return "(not present)";var text=new StringBuilder();int depth=1;for(int p=m.Index+m.Length;p<header.Length;p++){char c=header[p];if(c=='\\'&&p+1<header.Length){c=header[++p];if(c>='0'&&c<='7'){int value=c-'0',digits=1;while(digits<3&&p+1<header.Length&&header[p+1]>='0'&&header[p+1]<='7'){value=value*8+header[++p]-'0';digits++;}text.Append((char)(value&255));continue;}if(c=='n')c='\n';else if(c=='r')c='\r';else if(c=='t')c='\t';else if(c=='\n')continue;else if(c=='\r'){if(p+1<header.Length&&header[p+1]=='\n')p++;continue;}text.Append(c);continue;}if(c=='(')depth++;if(c==')'){depth--;if(depth==0)break;}text.Append(c);}return text.ToString();}
  public static string Description(string path){byte[] b=File.ReadAllBytes(path);int length=b.Length>6&&b[0]==128&&b[1]==1?BitConverter.ToInt32(b,2):Math.Min(b.Length,8192);int at=b.Length>6&&b[0]==128&&b[1]==1?6:0;if(length<0||length>b.Length-at)throw new InvalidDataException("Invalid PFB header length.");string header=Encoding.GetEncoding(28591).GetString(b,at,length);string comments=String.Join("\r\n",header.Split('\n').Select(x=>x.TrimEnd('\r')).Where(x=>x.StartsWith("%")&&!x.StartsWith("%!")).ToArray());string afm=Path.ChangeExtension(path,"afm");if(File.Exists(afm))comments+="\r\n"+String.Join("\r\n",File.ReadAllLines(afm,Encoding.GetEncoding(28591)).Where(x=>x.StartsWith("Comment ")).ToArray());var font=Regex.Match(header,@"/FontName\s+/([^\s]+)");return "File: "+path+"\r\nFont: "+(font.Success?font.Groups[1].Value:"(not present)")+"\r\nFamily: "+Literal(header,"FamilyName")+"\r\nVersion: "+Literal(header,"version")+"\r\nNotice: "+Literal(header,"Notice")+"\r\nComments:\r\n"+(String.IsNullOrWhiteSpace(comments)?"(not present)":comments);}
 }
 public sealed partial class Editor {
  readonly HashSet<string> fontChoiceOffers=new HashSet<string>();bool choosingFontVersion;
  internal void ShowFontVersionChoices(){foreach(var pair in FontVersionChoices.Available.ToArray())Safe(()=>ChooseFontVersion(pair.Key,pair.Value));fontChoiceOffers.Clear();RefreshAll(canvas.Selected);}
  void QueueFontVersionChoices(){if(Program.CheckMode||!IsHandleCreated||choosingFontVersion)return;var pending=FontVersionChoices.Pending.ToArray();if(!pending.Any())return;choosingFontVersion=true;BeginInvoke(new Action(()=>{try{foreach(var conflict in pending){string key=NativeTupletFonts.DirectoryPath+"\n"+conflict.Key+"\n"+String.Join("\n",conflict.Value);if(!fontChoiceOffers.Add(key))continue;Safe(()=>ChooseFontVersion(conflict.Key,conflict.Value));}RefreshAll(canvas.Selected);}finally{choosingFontVersion=false;}}));}
  void ChooseFontVersion(string name,string[] paths){using(var dialog=new ScoreDialog("font-version"){Text="Choose Type 1 font: "+name,Width=1050,Height=700,StartPosition=FormStartPosition.CenterParent}){var list=new ListBox{Dock=DockStyle.Top,Height=130,HorizontalScrollbar=true};list.Items.AddRange(paths);var details=new TextBox{Dock=DockStyle.Fill,Multiline=true,ReadOnly=true,ScrollBars=ScrollBars.Both,WordWrap=false};list.SelectedIndexChanged+=(s,e)=>{if(list.SelectedIndex>=0){try{details.Text=FontVersionChoices.Description(paths[list.SelectedIndex]);}catch(Exception error){details.Text="Font metadata could not be read: "+error.Message+"\r\nFile: "+paths[list.SelectedIndex];}}};var buttons=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,FlowDirection=FlowDirection.RightToLeft};var use=new Button{Text="Use selected font",AutoSize=true,DialogResult=DialogResult.OK};var cancel=new Button{Text="Leave unresolved",AutoSize=true,DialogResult=DialogResult.Cancel};buttons.Controls.Add(use);buttons.Controls.Add(cancel);dialog.Controls.Add(details);dialog.Controls.Add(list);dialog.Controls.Add(buttons);dialog.AcceptButton=use;dialog.CancelButton=cancel;list.SelectedIndex=0;if(dialog.ShowDialog(this)==DialogResult.OK&&list.SelectedIndex>=0)FontVersionChoices.Select(name,paths[list.SelectedIndex]);}}
 }
}
