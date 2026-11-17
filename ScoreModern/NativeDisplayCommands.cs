using System;using System.Linq;using ScoreModern.RecoveredFragments;
namespace ScoreModern {public sealed partial class Editor {
 NotationView? datViewBeforeSh;bool originalShowDatFonts;readonly NativeMouseVisibility originalMouseVisibility=new NativeMouseVisibility();
 readonly LayoutDisappearingStaffs disappearingStaffs=new LayoutDisappearingStaffs();
 bool ExecuteOriginalDisplayCommand(string text){if(mode!=ScreenMode.Main)return false;string value=text.Trim();if(value.Equals("MH",StringComparison.OrdinalIgnoreCase)||value.Equals("MB",StringComparison.OrdinalIgnoreCase)){originalMouseVisibility.Apply(canvas,value.Equals("MB",StringComparison.OrdinalIgnoreCase));Message("");return true;}if(value.Equals("SCX",StringComparison.OrdinalIgnoreCase)||value.Equals("SC",StringComparison.OrdinalIgnoreCase)||value.StartsWith("SC ",StringComparison.OrdinalIgnoreCase)){int staff=0;if(!value.Equals("SCX",StringComparison.OrdinalIgnoreCase)){var scaleNumbers=LayoutNumbers(value.Substring(2));if(scaleNumbers.Length>1)throw new NotSupportedException("Spacing-scale extra arguments need verification.");staff=scaleNumbers.Length==0?(scor4Interface?Scor4PlacementStaff():1):CenterWord(scaleNumbers[0]);if(canvas.PageDocument.MaximumDefinedStaff<1)throw new NotSupportedException("Spacing scale on an empty staff table needs original verification.");if(staff<1||staff>canvas.PageDocument.MaximumDefinedStaff)staff=1;}canvas.SpacingScaleStaff=staff;NativeDosViewport.DisplayChanged();canvas.Invalidate();Message("");return true;}if(value.Equals("SH",StringComparison.OrdinalIgnoreCase)||value.Equals("SHX",StringComparison.OrdinalIgnoreCase)){originalShowDatFonts=value.Equals("SH",StringComparison.OrdinalIgnoreCase);if(originalShowDatFonts){if(NativeNotationView.UsesDat){datViewBeforeSh=NativeNotationView.Mode;NativeNotationView.Set(NotationView.Vector,false);}}else if(datViewBeforeSh.HasValue){NativeNotationView.Set(datViewBeforeSh.Value,false);datViewBeforeSh=null;}Message("");return true;}if(value.Length<2||!value.Substring(0,2).Equals("DP",StringComparison.OrdinalIgnoreCase)||value.Length>2&&!(Char.IsWhiteSpace(value[2])||Char.IsDigit(value[2])||value[2]=='+'||value[2]=='-'||value[2]=='.'))return false;
  var numbers=LayoutNumbers(value.Substring(2));if(numbers.Length==0||BitConverter.ToInt32(BitConverter.GetBytes(numbers[0]),0)==0){var hidden=disappearingStaffs.HiddenStaffs();Message(hidden.Length==0?"":"*** WARNING -- some staves are in \"disappear mode.\""+Environment.NewLine+String.Join(",",hidden.Select(n=>n.ToString().PadLeft(2))));return true;}disappearingStaffs.Apply(numbers);Message("");return true;
 }
 void ApplyOriginalDisplayState(bool applyPendingStaffs=true,bool rebuildPhysicalDisplay=false){
  var hidden=disappearingStaffs.HiddenStaffs();
  bool staffsChanged=applyPendingStaffs&&!canvas.DisappearingStaffs.SetEquals(hidden);
  bool fontsChanged=canvas.TextLayout!=null&&canvas.TextLayout.ShowActualDatFonts!=originalShowDatFonts;
  if(staffsChanged){canvas.DisappearingStaffs.Clear();canvas.DisappearingStaffs.UnionWith(hidden);}
  if(fontsChanged)canvas.TextLayout.ShowActualDatFonts=originalShowDatFonts;
  if(rebuildPhysicalDisplay&&(staffsChanged||physicalDisplayActualFonts!=originalShowDatFonts))ObserveDisplayModeRebuild();
  if(staffsChanged||fontsChanged){canvas.ItemBounds.Clear();NativeDosViewport.DisplayChanged();}
 }
}}
