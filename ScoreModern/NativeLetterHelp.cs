using System;using System.IO;using System.Text;using System.Linq;using System.Collections.Generic;using System.Windows.Forms;
namespace ScoreModern {
 public sealed partial class Editor {
  const string LetterHelpFooter="Use PgDn, PgUp, Home, End, Esc=exit.  ?n=more help (n=number or letter).";
  Panel letterHelpOverlay;NativeTextLabel letterHelpBody;
  List<string[]> letterHelpPages;int letterHelpPage;string letterHelpPreviousMessage;bool letterHelpTopicPending,letterHelpIsType,letterHelpIsParameters;
  bool SubmitLetterHelp(string input){
   if(pendingDiskFilename!=null||pendingLetterSave!=null||pendingLetterLoad!=null||pendingDiskDelete!=null||pendingDriveChange||drawExportStage!=DrawExportStage.None||pendingParameterExport||musicInput!=null||groupCommand!=null||pendingAppendPath!=null||pendingAppendScore!=null||pendingEditAcknowledgement||inlineTextActive||originalItemZero)return false;
   if(letterHelpIsType||letterHelpIsParameters){AdvanceTypeListing();return true;}
   string request=input.Trim();if(!request.StartsWith("?",StringComparison.Ordinal)){if(letterHelpPages==null)return false;ShowLetterHelpPage();return true;}
   string topic=request.Substring(1).ToUpperInvariant(),name;
   int number;
   if(topic.Length>0&&Char.IsWhiteSpace(topic[0]))topic="";
   if(topic.Length==0)name=ModernReferenceTopic();
   else if(topic=="PREF"||topic=="PRINT")name=topic;
   else if(topic.Length==1&&topic[0]>='A'&&topic[0]<='Z')name=topic;
   else if(topic.Length<=2&&topic.All(c=>c>='0'&&c<='9')&&Int32.TryParse(topic,out number)&&number>=1&&number<=99)name="CODE"+number.ToString("00");
   else throw new NotSupportedException("Help topics accept a letter, a two-digit code number, PREF, or PRINT.");
   ShowHelpTopic(name);
   return true;
  }
  void ShowHelpTopic(string topic){
   if(!OpenModernHelp(topic))Message("Help topic unavailable: "+topic+". Check the Help folder beside SCORE Modern.");
  }
  void ShowLetterHelpPage(){if(letterHelpPages==null)return;Message(String.Join(Environment.NewLine,letterHelpPages[letterHelpPage])+(letterHelpIsType||letterHelpIsParameters?"":Environment.NewLine+LetterHelpFooter));textWindow.AutoScrollPosition=System.Drawing.Point.Empty;
   if(letterHelpOverlay==null){letterHelpOverlay=new Panel{AutoScroll=true};letterHelpBody=new NativeTextLabel{Wrapped=true,AutoSize=false};letterHelpOverlay.Controls.Add(letterHelpBody);Controls.Add(letterHelpOverlay);Resize+=(sender,args)=>UpdateLetterHelpOverlay();}
   UpdateLetterHelpOverlay();letterHelpOverlay.AutoScrollPosition=System.Drawing.Point.Empty;letterHelpOverlay.Visible=true;letterHelpOverlay.BringToFront();command.Focus();}
  void UpdateLetterHelpOverlay(){if(letterHelpOverlay==null)return;if(letterHelpPages==null){letterHelpOverlay.Visible=false;return;}int top=PointToClient(fileLine.PointToScreen(new System.Drawing.Point(0,fileLine.Height))).Y;letterHelpOverlay.Bounds=new System.Drawing.Rectangle(0,top,ClientSize.Width,Math.Max(1,ClientSize.Height-top));letterHelpOverlay.BackColor=responseLine.BackColor;letterHelpBody.Font=classicFont;letterHelpBody.LinePitch=textRowHeight;letterHelpBody.ForeColor=responseLine.ForeColor;letterHelpBody.BackColor=responseLine.BackColor;letterHelpBody.Text=responseLine.Text;int width=Math.Max(1,letterHelpOverlay.ClientSize.Width-24-SystemInformation.VerticalScrollBarWidth);letterHelpBody.SetBounds(12,4,width,letterHelpBody.ContentHeight(width));}
  void HideLetterHelp(){letterHelpIsParameters=false;letterHelpIsType=false;letterHelpPages=null;letterHelpTopicPending=false;if(letterHelpOverlay!=null)letterHelpOverlay.Visible=false;}

  bool HandleLetterHelpCharacter(char value){if(letterHelpPages==null)return false;if(letterHelpIsParameters){AdvanceTypeListing();return true;}if(letterHelpIsType){if(letterHelpPage==letterHelpPages.Count-1)AdvanceTypeListing();return true;}if(letterHelpTopicPending){letterHelpTopicPending=false;if(value>32&&value<127)SubmitLetterHelp("?"+value);return true;}if(value=='?')letterHelpTopicPending=true;return true;}
  bool HandleLetterHelpKey(KeyEventArgs key){if(letterHelpPages==null)return false;
   if(letterHelpIsParameters){if(key.KeyCode==Keys.ShiftKey||key.KeyCode==Keys.ControlKey||key.KeyCode==Keys.Menu||key.KeyCode==Keys.LWin||key.KeyCode==Keys.RWin){key.SuppressKeyPress=true;return true;}if(key.KeyCode==Keys.Escape){HideLetterHelp();Message("");command.Clear();}else AdvanceTypeListing();key.SuppressKeyPress=true;return true;}
   if(letterHelpIsType){if(key.KeyCode==Keys.ShiftKey||key.KeyCode==Keys.ControlKey||key.KeyCode==Keys.Menu||key.KeyCode==Keys.LWin||key.KeyCode==Keys.RWin){key.SuppressKeyPress=true;return true;}if(key.KeyCode==Keys.Escape){HideLetterHelp();Message("");command.Clear();}else if(key.KeyCode==Keys.Enter||letterHelpPage==letterHelpPages.Count-1)AdvanceTypeListing();key.SuppressKeyPress=true;return true;}
   if(key.Modifiers!=Keys.None){if(key.Control&&new[]{Keys.C,Keys.V,Keys.X,Keys.A}.Contains(key.KeyCode))return false;key.SuppressKeyPress=true;return true;}
   if(key.Modifiers==Keys.None){switch(key.KeyCode){case Keys.Enter:key.SuppressKeyPress=true;return true;case Keys.Escape:HideLetterHelp();Message(letterHelpPreviousMessage??"");command.Clear();if(mode==ScreenMode.Print)RefreshOriginalPrintMenu();else command.Focus();key.SuppressKeyPress=true;return true;case Keys.PageDown:letterHelpPage=Math.Min(letterHelpPages.Count-1,letterHelpPage+1);break;case Keys.PageUp:letterHelpPage=Math.Max(0,letterHelpPage-1);break;case Keys.Home:letterHelpPage=0;break;case Keys.End:letterHelpPage=letterHelpPages.Count-1;break;default:if(key.KeyCode>=Keys.F1&&key.KeyCode<=Keys.F12){key.SuppressKeyPress=true;return true;}return false;}ShowLetterHelpPage();key.SuppressKeyPress=true;return true;}return false;
  }
 }
}
