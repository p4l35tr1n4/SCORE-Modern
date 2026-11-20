using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ScoreModern {
 public static class NativeArrayDiagnostics {
  static bool configured;
  public static bool AllowLogicalArrays { get { return !NativeScoreBehaviour.Enabled && configured; } }
  public static void Set(bool enabled,bool save) { configured=enabled;if(save)NativeSettings.WriteAllText("dpy-native-arrays",enabled?"enabled":"strict"); }
  public static void Load() { Set(false,false);try { Set(NativeSettings.Read("dpy-native-arrays")=="enabled",false); }catch(IOException){}catch(UnauthorizedAccessException){} }
  public static void AddMenu(ContextMenuStrip menu) {
   var item=new ToolStripMenuItem("DPY: allow labelled native logical arrays");
   item.Click+=(s,e)=>Set(!AllowLogicalArrays,true);menu.Items.Add(item);
   menu.Opening+=(s,e)=>{item.Checked=AllowLogicalArrays;item.ToolTipText="Off: reject unverified DOS storage histories. On: show explicitly labelled logical arrays reconstructed from current items; unused DOS storage is not reproduced.";};
  }
 }
 public sealed partial class Editor {
  // Original1DBB:075C..08AA prints IPTR, then each indexed RN record.
  // A tracked contiguous bank can include the original edit and compaction
  // operations whose record order is verified alongside the display bank.
  static string RawArrayNumber(float value) {
   if(Single.IsNaN(value)||Single.IsInfinity(value))
    throw new NotSupportedException("DPY nonfinite array formatting requires original verification.");
   string formatted=ExportFixed(value,3);
   return formatted.Length>11?new string('*',11):formatted.PadLeft(11);
  }
  static IEnumerable<string> RawArrayRows(IEnumerable<string> fields,int perLine) {
   var values=fields.ToArray();
   for(int at=0;at<values.Length;at+=perLine) {
    string line=String.Concat(values.Skip(at).Take(perLine));
    // The original console consumes the first Fortran carriage-control column.
    yield return line.Substring(1);
   }
  }
  void ShowOriginalDisplayArray() {
   // Track the verified startup/load producer history, including untouched
   // marker words. Unmodelled mutation histories retain the strict guard.
   var physical=TrackedDisplayArray();bool logical=NativeArrayDiagnostics.AllowLogicalArrays||physical==null;
   if(logical&&!NativeArrayDiagnostics.AllowLogicalArrays)throw new NotSupportedException("DPY -1 display-array layout is not verified for this state. Explicitly enable labelled native logical arrays to inspect reconstructed vectors.");
   canvas.LayoutScore();var dat=new NativeDatGlyphs(canvas.Library.Root);var cache=new NativeDatHeadCache();short font=0;
   var records=new List<List<string>>();var pointers=new List<int>();int pointer=1;
   if(!logical){pointers=physical.Pointers.ToList();for(int item=0;item<pointers.Count-1;item++){var record=new List<string>();for(int at=pointers[item];at<pointers[item+1];at++)record.Add(physical.X[at].ToString(CultureInfo.InvariantCulture).PadLeft(6)+physical.Y[at-1].ToString(CultureInfo.InvariantCulture).PadLeft(7));records.Add(record);}}
   else {using(canvas.TextLayout.PreserveAlignmentMetricHistory())foreach(var item in score.Items) {
    pointers.Add(pointer);short next;var vectors=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,cache,font,out next);font=next;
    // Main's display buffer retains the staff-number contours in the staff
    // record. The F6 producer deliberately excludes this screen-only suffix.
    if(item.Code==8&&!canvas.TextLayout.ShowActualDatFonts)foreach(var number in NativeDosOverlays.Numbers(score,canvas.PageDocument,canvas.TextLayout,canvas.DisappearingStaffs))if(number.Staff==item.Staff)vectors.AddRange(number.Points);
    var record=new List<string>();
    foreach(var point in vectors) {
     if(point.Operation>0){record.Add((point.Operation-32700).ToString(CultureInfo.InvariantCulture).PadLeft(6)+"      0");pointer++;}
     if(point.CommandOnly)continue;
     record.Add(NativeNotationContext.Word(point.X,true).ToString(CultureInfo.InvariantCulture).PadLeft(6)+NativeNotationContext.Word(point.Y,false).ToString(CultureInfo.InvariantCulture).PadLeft(7));pointer++;
    }
    records.Add(record);
   }
   pointers.Add(pointer);}var pages=new List<string[]>();var page=new List<string>();
   if(logical)page.Add("NATIVE LOGICAL ARRAY — reconstructed display vectors; DOS physical storage is not reproduced.");
   page.Add("DPY array");page.AddRange(RawArrayRows(pointers.ConvertAll(value=>value.ToString(CultureInfo.InvariantCulture).PadLeft(5)),10));int row=page.Count;
   // Original31F6:046E calls the BIOS-row pause helper before every pair,
   // including pairs inside a single long item, rather than only per item.
   foreach(var record in records)foreach(var pair in record){if(row>=20){page.Add("Press any key to continue.");pages.Add(page.ToArray());page.Clear();row=0;if(logical){page.Add("NATIVE LOGICAL ARRAY — continued; DOS physical storage is not reproduced.");row=1;}}page.Add(pair);row++;}
   page.Add("Press any key to continue.");pages.Add(page.ToArray());if(ShowCommandList("Display array",pages))return;letterHelpPages=pages;letterHelpIsType=false;letterHelpIsParameters=true;letterHelpPage=0;letterHelpTopicPending=false;letterHelpPreviousMessage="";ShowLetterHelpPage();
  }
  void ShowOriginalParameterArray() {
   bool logical=!score.RawStorageLayoutVerified&&TrackedDisplayArray()==null;
   if(logical&&!NativeArrayDiagnostics.AllowLogicalArrays)
    throw new NotSupportedException("DPY 1 parameter storage is unverified for this edit history. Save and reload, or enable labelled native logical-array dumps in Preferences.");
   var pointers=new List<int>();int pointer=1;
   foreach(var item in score.Items){pointers.Add(pointer);pointer+=item.Count+1;}
   pointers.Add(pointer);
   var pages=new List<string[]>();var page=new List<string>();
   if(logical)page.Add("NATIVE LOGICAL ARRAY — reconstructed current items; DOS physical storage is not reproduced.");
   page.Add(" IPTR array");
   page.AddRange(RawArrayRows(pointers.Select(value=>value.ToString(CultureInfo.InvariantCulture).PadLeft(5)),10));
   page.Add(" RN array");int row=page.Count;
   foreach(var item in score.Items) {
    if(row>=20){page.Add("Press any key to continue.");pages.Add(page.ToArray());page.Clear();row=0;if(logical){page.Add("NATIVE LOGICAL ARRAY — continued; DOS physical storage is not reproduced.");row=1;}}
    int last=item.P(1)==11?Math.Min(9,item.Count):item.Count;
    var values=new List<float>{BitConverter.ToSingle(item.Bytes,0)};
    for(int n=1;n<=last;n++)values.Add(item.P(n));
    var lines=RawArrayRows(values.Select(RawArrayNumber),7).ToArray();
    page.AddRange(lines);row+=lines.Length;
   }
   page.Add("Press any key to continue.");pages.Add(page.ToArray());
   if(ShowCommandList("Parameter array",pages))return;
   letterHelpPages=pages;letterHelpIsType=false;letterHelpIsParameters=true;
   letterHelpPage=0;letterHelpTopicPending=false;letterHelpPreviousMessage="";
   ShowLetterHelpPage();
  }
 }
}
