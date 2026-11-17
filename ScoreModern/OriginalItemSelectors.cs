using System;using System.Globalization;using System.IO;
namespace ScoreModern {
 public sealed partial class Editor {
  static string SeparateSelectorArguments(string text){foreach(string prefix in new[]{"COL","MU","MD","SC","ED","ST","CTN","CT","CD","JR","PT","CN","I","N"}){if(text.Length<=prefix.Length||!text.StartsWith(prefix,StringComparison.OrdinalIgnoreCase))continue;char next=text[prefix.Length];if(next>='0'&&next<='9'||next=='-'||next=='+'||next=='.')return text.Substring(0,prefix.Length)+" "+text.Substring(prefix.Length);}return text;}
  bool quietPromptCancelled;
  bool OriginalQuietPromptBoundary(string text){if(!quietPromptCancelled)return false;quietPromptCancelled=false;if(!text.StartsWith("I ",StringComparison.OrdinalIgnoreCase))return false;int number=ReadOriginalItemNumber(text.Substring(2).Trim());if(number<=0)return false;if(!NativeEditCompatibility.ReproduceInvalidStates)throw new InvalidOperationException("The original item-0 transition after canceling an N prompt is prevented. Repeat I, or enable original invalid-state reproduction.");var context=canvas.Selected;if(context==null)return false;var entry=context.Clone();lastEditedIndex=score.Items.IndexOf(context);originalItemZero=true;direction=-1;editOriginal=null;editIndex=-1;canvas.EditOriginal=entry;canvas.EditModified=false;canvas.SelectionContextSource=context;canvas.BlockItemEditing=true;ItemTypeout(false);mode=ScreenMode.Edit;RefreshAll(entry.Clone());LayoutClassic();canvas.Focus();return true;}
  Item selectionReturnItem;int selectionReturnDirection;bool selectionReturnQuiet;
  bool RestoreOriginalItemPrompt(){if(!selectionPending||selectionReturnItem==null)return false;var item=selectionReturnItem;selectionReturnItem=null;direction=selectionReturnDirection;ItemTypeout(selectionReturnQuiet);BeginEdit(item);quietPromptCancelled=selectionReturnQuiet;return true;}
  bool quietItemTypeout,quietHeaderWasEdit;Item quietHeaderItem;string quietHeaderContext;
  void ItemTypeout(bool quiet){if(quiet&&!quietItemTypeout){quietHeaderWasEdit=mode==ScreenMode.Edit;quietHeaderItem=parameters.Item==null?null:parameters.Item.Clone();quietHeaderContext=contextLine.Text;}quietItemTypeout=quiet;}
  static int ReadOriginalItemNumber(string text){float number;if(!Single.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out number)||Single.IsNaN(number)||Single.IsInfinity(number)||number< -1||number>32767)throw new NotSupportedException("This item-number form is outside the verified selector range (-1 to 32767).");return (int)number;}
  void SelectOriginalItem(string verb,string rest){
   if(mode==ScreenMode.Edit&&canvas.EditModified&&!originalItemZero){WaitForEditAcknowledgment();return;}
   ItemTypeout(verb=="N");
   if(score.Items.Count==0){Message("There are no items to edit.");return;}
   int number=rest.Length==0?0:ReadOriginalItemNumber(rest);
   if(number==0&&mode==ScreenMode.Edit&&canvas.Selected!=null){LayoutClassic();UpdateClassic();return;}
   if(number<0){selectionReturnItem=canvas.Selected;selectionReturnDirection=direction;selectionReturnQuiet=quietItemTypeout;ExitEdit(false);selectionPending=true;mode=ScreenMode.EditMenu;Message("Enter item number or click mouse on item.");command.Clear();command.Focus();LayoutClassic();return;}
   int index=number==0?Math.Max(0,Math.Min(lastEditedIndex,score.Items.Count-1)):Math.Min(number,score.Items.Count)-1;
   if(mode==ScreenMode.Edit&&canvas.Selected!=null&&score.Items.Contains(canvas.Selected))lastEditedIndex=score.Items.IndexOf(canvas.Selected);
   selectionReturnItem=null;editFilter=null;NativeEditPositionGuide.Set(canvas,null);direction=1;editOriginal=null;BeginEdit(score.Items[index]);
  }
 }
}
