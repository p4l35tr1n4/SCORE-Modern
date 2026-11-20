using System.Windows.Forms;

namespace ScoreModern {
 public sealed partial class Editor {
  static readonly string[] EditGroupCommands={"M","C","A","DE","J","LI","FLO","TR"};

  void ChooseEditGroupOperation(int key) {
   if(key<1||key>EditGroupCommands.Length)return;
   ApplyOriginalGroupField(command.TextLength!=0||!originalGroupValueBox.Visible?command.Text:originalGroupValueBox.Text);
   CloseOriginalGroupValue();
   string verb=EditGroupCommands[key-1];
   if(key==3&&originalGroupFields[3]==0)throw new System.IO.InvalidDataException("Code# must be specified for Alter.");
   if(key<=4){groupCommand=verb;groupRange=new float[5];for(int n=0;n<5;n++)groupRange[n]=originalGroupFields[n];if(groupRange[4]==0)groupRange[4]=groupRange[0];command.Clear();Message((key==1?"Move":key==2?"Copy":key==3?"Alter":"Delete")+"\r\n"+(key<=2?GroupDestinationPrompt:key==3?GroupAlterPrompt:GroupDeletePrompt));command.Focus();return;}
   if(key==5||key==6){groupCommand=verb;groupRange=null;layoutTextCheckpoint=false;layoutRequestedStaff=originalGroupFields[0];layoutRequestedTop=originalGroupFields[4];layoutBottom=layoutRequestedStaff>32?(short)1:layoutRequestedStaff;layoutTop=layoutRequestedStaff>32?(short)32:originalGroupFields[4]==0?layoutBottom:originalGroupFields[4];layoutLeft=originalGroupFields[1];layoutRight=originalGroupFields[2];layoutPrompt=LayoutPrompt.Confirm;command.Clear();Message((key==5?"Justify":"Lineup")+"\r\n"+GroupDeletePrompt);command.Focus();return;}
   if(key==7){groupCommand="FLO";groupRange=new float[5];for(int n=0;n<5;n++)groupRange[n]=originalGroupFields[n];if(groupRange[4]==0)groupRange[4]=groupRange[0];originalNotationRange=(float[])groupRange.Clone();originalGroupFlipQuestion=true;command.Clear();Message("Flip Stems, Beams and Slurs.\r\nPress \"U\" for up, \"D\" for down, \"N\" for normal or <ENTER> for opposite. ->");command.Focus();return;}
   if(key==8){command.Clear();ExecuteMainTranspose("TR",originalGroupFields[0].ToString(System.Globalization.CultureInfo.InvariantCulture));command.Focus();return;}
  }

  void ToggleEditGraphicMenu(int number) {
   if(pendingGraphicalMenu!=0)return;
   selectionPending=false;
   mode=ScreenMode.Main;
   UpdateClassic();
   string verb=graphicalMenuStaffs[number]==0?"MU":"MD";
   ExecuteOriginalGraphicalMenu(verb+" "+number);
   command.Focus();
  }
 }
}
