using System.Drawing;
namespace ScoreModern {public sealed partial class Editor {
 int postGroupGraphicsTop=-1;
 // Original mode17: 1F1E:03FE decrements the lower text row when>=5;
 // 202B:01AF uses 16*(row+1)+14 in group-window mode. 1F1E:0416
 // increments that row on exit; the normal window uses16*(row+1)+1.
 void UpdateGroupWindowState(){canvas.DosGroupHeader=mode==ScreenMode.EditGroupMenu&&!(NativeKeyboardOptions.GroupFull&&inputFullGraphics);canvas.DosGroupTop=textWindowRows*16-2;}
 void EnterOriginalGroupWindow(){ResetOriginalGroupFields();postGroupGraphicsTop=-1;if(textWindowRows>=7)textWindowRows--;inputDosTextBoundary=true;mode=ScreenMode.EditGroupMenu;LayoutGroupWindow();}
 void LayoutGroupWindow(){Point scroll=viewport.AutoScrollPosition;bool prior=movingTextBoundary;movingTextBoundary=true;try{LayoutClassic();viewport.AutoScrollPosition=new Point(-scroll.X,-scroll.Y);}finally{movingTextBoundary=prior;}UpdateGroupWindowState();canvas.Invalidate();}
 void LeaveOriginalGroupWindow(){CloseOriginalGroupValue();command.Clear();textWindowRows++;postGroupGraphicsTop=(textWindowRows-1)*16+1;inputDosTextBoundary=true;mode=ScreenMode.EditMenu;LayoutGroupWindow();}
}}
