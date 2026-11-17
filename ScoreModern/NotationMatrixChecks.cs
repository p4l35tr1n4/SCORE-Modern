using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Globalization;
namespace ScoreModern {
 public sealed partial class Editor {
  internal void CheckNotationMatrixCase(string directory){
   directory=Path.GetFullPath(directory);
   var previousView=NativeNotationView.Mode;short previousViewX=NativeDosViewport.OriginX,previousViewY=NativeDosViewport.OriginY,previousDivisor=NativeDosViewport.Divisor;float previousZoom=NativeDosViewport.ZoomFactor;string caseView=Path.Combine(directory,"notation-view.txt");if(File.Exists(caseView)){NativeDosViewport.Reset();NativeNotationView.Set((NotationView)Enum.Parse(typeof(NotationView),File.ReadAllText(caseView).Trim()),false);}bool previousArrays=NativeArrayDiagnostics.AllowLogicalArrays;NativeArrayDiagnostics.Set(File.Exists(Path.Combine(directory,"native-logical-arrays.txt")),false);string previousDirectory=Directory.GetCurrentDirectory();try{if(File.Exists(Path.Combine(directory,"use-case-working-directory.txt")))Directory.SetCurrentDirectory(directory);CheckNotationMatrixCaseCore(directory);}finally{NativeDosViewport.OriginX=previousViewX;NativeDosViewport.OriginY=previousViewY;NativeDosViewport.Divisor=previousDivisor;NativeDosViewport.ZoomFactor=previousZoom;NativeDosViewport.DisplayChanged();NativeNotationView.Set(previousView,false);if(canvas.TextLayout!=null)canvas.TextLayout.ResetDatMetricVerification();NativeArrayDiagnostics.Set(previousArrays,false);Directory.SetCurrentDirectory(previousDirectory);}
  }
  void CheckNotationMatrixCaseCore(string directory){
   string resets=Path.Combine(directory,"reset-files.txt");if(File.Exists(resets))foreach(string name in File.ReadAllLines(resets)){string root=Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,target=Path.GetFullPath(Path.Combine(directory,name));if(!target.StartsWith(root,StringComparison.OrdinalIgnoreCase))throw new Exception("File reset leaves private case directory.");File.Delete(target);}
   string seeds=Path.Combine(directory,"seed-files.tsv");if(File.Exists(seeds))foreach(string row in File.ReadAllLines(seeds)){string[] fields=row.Split('\t');if(fields.Length!=2)throw new Exception("Malformed private file seed.");string root=Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;string from=Path.GetFullPath(Path.Combine(directory,fields[0])),to=Path.GetFullPath(Path.Combine(directory,fields[1]));if(!from.StartsWith(root,StringComparison.OrdinalIgnoreCase)||!to.StartsWith(root,StringComparison.OrdinalIgnoreCase)||String.Equals(from,to,StringComparison.OrdinalIgnoreCase))throw new Exception("File seed leaves private case directory.");File.Copy(from,to,true);}

   if(File.Exists(Path.Combine(directory,"print-workspace-fresh.txt"))){printJob=null;printStage=OriginalPrintStage.None;printMenuFile="";printMenuDirty=false;printMenuNotice="";}
   if(letterHelpPages!=null)HandleLetterHelpKey(new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Escape));
   if(musicInput!=null)LeaveMusicInput();originalInputSpacing=999;ClearMainGroup();pendingEditAcknowledgement=false;if(File.Exists(Path.Combine(directory,"input-motives-fresh.txt")))inputMotives.Clear();
   if(File.Exists(Path.Combine(directory,"text-workspace-fresh.txt"))){Array.Copy(new[]{1f,5f,-3f,1f,1f,0f},originalTextSetup,6);originalTextSetupField=0;originalTextFont=0;originalTextTabNotified=false;}
   if(File.Exists(Path.Combine(directory,"layout-workspace-fresh.txt"))){layoutWorkspace.ResetForIsolatedTest();lineupWorkspace=new ScoreModern.RecoveredFragments.LayoutLineupWorkspace();textJustificationWorkspace=new ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace();Array.Clear(verticalOffsets,0,verticalOffsets.Length);}
   if(File.Exists(Path.Combine(directory,"transposition-workspace-fresh.txt"))){
    transposeWorkspace.Collector.FirstNotePosition=0;transposeWorkspace.Clefs.RetainedStaff=0x2020;transposeWorkspace.Clefs.RetainedTarget=0x2020;
    Array.Clear(transposeWorkspace.PitchWords,0,200);Array.Clear(transposeWorkspace.AccidentalMarkers,0,200);transposeWorkspace.KindZeroGuard=0;transposeWorkspace.Window=new ScoreModern.RecoveredFragments.TranspositionRelationshipWindow();transposeWorkspace.AlreadySorted=false;
   }
   // Compatibility is already selected when a real user opens the file.
   // Apply it before load-time cache/converter writes, not after them.
   NativeEditCompatibility.Set(File.Exists(Path.Combine(directory,"original-invalid-states.txt")),false);
   NativeEditCompatibility.SetQuarterToneWidths(File.Exists(Path.Combine(directory,"original-quarter-tone-spacing.txt")),false);
   if(File.Exists(Path.Combine(directory,"graphical-menu-workspace-fresh.txt"))){ClearGraphicalMenuPrompt();Array.Clear(graphicalMenuStaffs,0,graphicalMenuStaffs.Length);}
   SetDocument(Score.Load(Path.Combine(directory,"input.MUS")),"");
   var loadedItems=score.Items.Select(item=>item.Clone()).ToArray();
   byte[] before=score.Encode();
   string retained=Path.Combine(directory,"original-grace-word.txt");
   string header=Path.Combine(directory,"original-grace-header.txt");
   NativeEditCompatibility.SetGrace(File.Exists(retained)||File.Exists(header),false);
   testGraceWord=File.Exists(retained)?(short?)Int16.Parse(File.ReadAllText(retained).Trim(),CultureInfo.InvariantCulture):null;
   originalInputStageHeader=File.Exists(header)?Int32.Parse(File.ReadAllText(header).Trim(),CultureInfo.InvariantCulture):0;
   NativeEditCompatibility.Set(File.Exists(Path.Combine(directory,"original-invalid-states.txt")),false);
   string previousStaff=Path.Combine(directory,"previous-notation-staff.txt");
   originalNotationStaff=File.Exists(previousStaff)?Single.Parse(File.ReadAllText(previousStaff).Trim(),CultureInfo.InvariantCulture):1;
   originalNotationRange=new[]{(float)(short)Math.Truncate(originalNotationStaff),0,220,0,(float)(short)Math.Truncate(originalNotationStaff)};
   bool refusal=File.Exists(Path.Combine(directory,"expected.refusal")),rejected=false;
   var commandTimer=System.Diagnostics.Stopwatch.StartNew();
   foreach(string line in File.ReadAllLines(Path.Combine(directory,"commands.txt"))){
    try{
     if(line.StartsWith("@EXPECT-NOTSUPPORTED ",StringComparison.Ordinal)){var unchanged=score.Encode();bool refused=false;try{Execute(line.Substring("@EXPECT-NOTSUPPORTED ".Length));}catch(NotSupportedException){refused=true;}if(!refused||!score.Encode().SequenceEqual(unchanged))throw new Exception("Expected bounded refusal without record mutation.");
     }else if(line.StartsWith("@VERIFY-DAT-BUFFERS ",StringComparison.Ordinal)){NativeDatBufferChecks.Compare(canvas,line.Substring("@VERIFY-DAT-BUFFERS ".Length));
     }else if(line.StartsWith("@EXPECT-PARAM ",StringComparison.Ordinal)){var values=line.Substring(14).Split(' ');int parameter=Int32.Parse(values[0]);float wanted=Single.Parse(values[1],CultureInfo.InvariantCulture);if(canvas.Selected==null||BitConverter.ToInt32(BitConverter.GetBytes(canvas.Selected.P(parameter)),0)!=BitConverter.ToInt32(BitConverter.GetBytes(wanted),0))throw new Exception("Active working parameter differs from original.");
     }else if(line=="@DRAWING-PROMPT-GUARD"){if(externalDrawingNameItem==null)throw new Exception("Expected active drawing-name request.");byte[] unchanged=score.Encode();Item selected=canvas.Selected;Function(3);ScoreMouse(1,System.Windows.Forms.Keys.None,new System.Drawing.PointF(300,300),new System.Drawing.PointF(300,300),true);OnKeyDown(new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Control|System.Windows.Forms.Keys.S));if(externalDrawingNameItem==null||!score.Encode().SequenceEqual(unchanged)||canvas.Selected!=selected||parameters.CanEditInline())throw new Exception("Interaction escaped drawing-name input.");
     }else if(line.StartsWith("@EXPECT-VIEWPORT ",StringComparison.Ordinal)){var wanted=line.Substring(17).Split(' ').Select(Int16.Parse).ToArray();if(wanted.Length!=3||NativeDosViewport.OriginX!=wanted[0]||NativeDosViewport.OriginY!=wanted[1]||NativeDosViewport.Divisor!=wanted[2])throw new Exception("Original viewport state differs: "+NativeDosViewport.OriginX+","+NativeDosViewport.OriginY+","+NativeDosViewport.Divisor);
     }else if(line.StartsWith("@VERIFY-EDI-INVERSE ",StringComparison.Ordinal)){var parts=line.Substring("@VERIFY-EDI-INVERSE ".Length).Split(new[]{' '},2);var result=ScoreModern.RecoveredFragments.InputEditInverse.Project(score.Items.Select(InputParameters).ToList(),Int32.Parse(parts[0],CultureInfo.InvariantCulture),0,200);var actualLines=new[]{result.Header,result.Positions}.Concat(result.Lines).ToArray();var expectedLines=File.ReadAllLines(parts[1]);if(!actualLines.SequenceEqual(expectedLines))throw new Exception("EDI inverse transcript differs from recorded original: "+String.Join(" | ",actualLines));
     }else if(line.StartsWith("@EXPECT-FUNCTION ",StringComparison.Ordinal)){var parts=line.Substring(17).Split(new[]{' '},2);int number=Int32.Parse(parts[0],CultureInfo.InvariantCulture);if(number<1||number>functionKeys.Length||!functionKeys[number-1].Text.EndsWith(parts[1],StringComparison.Ordinal))throw new Exception("Function-key label differs from original: "+line);
     }else if(line.StartsWith("@INLINE-TYPE ",StringComparison.Ordinal)){if(!inlineTextActive)throw new Exception("No active inline text editor.");foreach(char ch in line.Substring(13))OriginalInlineTextCharacter(new System.Windows.Forms.KeyPressEventArgs(ch));
     }else if(line.StartsWith("@TEXT-TYPE ",StringComparison.Ordinal)){if(originalTextDraft==null)throw new Exception("No pending text entry.");command.AppendText(line.Substring(11));
     }else if(line.StartsWith("@TEXT-COMMIT ",StringComparison.Ordinal)){if(originalTextDraft==null)throw new Exception("No pending text entry.");string entered=command.Text+line.Substring(13);command.Clear();Execute(entered);
     }else if(line.StartsWith("@EXPECT-COMMAND-TEXT ",StringComparison.Ordinal)){if(command.Text!=line.Substring("@EXPECT-COMMAND-TEXT ".Length))throw new Exception("Inline command field differs: "+command.Text);
     }else if(line.StartsWith("@EXPECT-INPUT-SP ",StringComparison.Ordinal)){float expectedSpacing=Single.Parse(line.Substring("@EXPECT-INPUT-SP ".Length),CultureInfo.InvariantCulture);if(!SameTextWord(expectedSpacing,originalInputSpacing))throw new Exception("Input spacing state differs from original DS8784.");
     }else if(line.StartsWith("@EXPECT-TAB-WARNING-GUARD ",StringComparison.Ordinal)){if(originalTextTabNotified!=Boolean.Parse(line.Substring("@EXPECT-TAB-WARNING-GUARD ".Length)))throw new Exception("Text-tab warning guard differs from original DS013C.");
     }else if(line.StartsWith("@EXPECT-TEXT-ENTRY ",StringComparison.Ordinal)){string actual=originalTextDraft==null?"None":originalTextDraft.Units==null?"Text":"Position";if(actual!=line.Substring(19))throw new Exception("Text input phase "+actual+" differs from original.");
     }else if(line.StartsWith("@EXPECT-TEXT-P4 ",StringComparison.Ordinal)){float wanted=Single.Parse(line.Substring(16),CultureInfo.InvariantCulture);if(originalTextDraft==null||BitConverter.ToInt32(BitConverter.GetBytes(originalTextDraft.Item.P(4)),0)!=BitConverter.ToInt32(BitConverter.GetBytes(wanted),0))throw new Exception("Pending text vertical position differs from original.");
     }else if(line.StartsWith("@EXPECT-RECORDS ",StringComparison.Ordinal)){var wanted=Score.Load(line.Substring("@EXPECT-RECORDS ".Length));if(score.Items.Count!=wanted.Items.Count||score.Items.Where((item,index)=>!item.Bytes.SequenceEqual(wanted.Items[index].Bytes)).Any()){File.WriteAllBytes(Path.Combine(directory,Path.GetFileName(line.Substring("@EXPECT-RECORDS ".Length))+".actual.MUS"),score.Encode());throw new Exception("Intermediate original records differ: "+line.Substring("@EXPECT-RECORDS ".Length));}
     }else if(line.StartsWith("@EXPECT-DIRTY ",StringComparison.Ordinal)){if(dirty!=Boolean.Parse(line.Substring("@EXPECT-DIRTY ".Length)))throw new Exception("Intermediate modified state differs.");
     }else if(line.StartsWith("@EXPECT-SELECTED ",StringComparison.Ordinal)){int wanted=Int32.Parse(line.Substring(17),CultureInfo.InvariantCulture);int actual=canvas.Selected==null?0:score.Items.IndexOf(canvas.Selected)+1;if(actual!=wanted)throw new Exception("Selected item "+actual+" != original "+wanted);
     }else if(line.StartsWith("@EXPECT-PRINT-BOTTOM-WORD ",StringComparison.Ordinal)){if(printJob.Bottom!=Int32.Parse(line.Substring(26),CultureInfo.InvariantCulture))throw new Exception("Print bottom differs from measured original word.");
     }else if(line.StartsWith("@EXPECT-PRINT-SIZE-BYTES ",StringComparison.Ordinal)){string bits=BitConverter.ToString(BitConverter.GetBytes(printJob.Size)).Replace("-","").ToLowerInvariant();if(bits!=line.Substring(25))throw new Exception("Print size bits "+bits+" != "+line.Substring(25));
     }else if(line.StartsWith("@EXPECT-PRINT-MESSAGE ",StringComparison.Ordinal)){if(mode!=ScreenMode.Print||!OriginalPrintMenuText().Contains(line.Substring(22)))throw new Exception("Original print menu text differs: "+line.Substring(22));
     }else if(line.StartsWith("@PRINT-INPUT ",StringComparison.Ordinal)){if(originalPrintEntry==null||mode!=ScreenMode.Print)throw new Exception("No active print entry.");originalPrintEntry.Text=line.Substring(13);OnKeyDown(new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Enter));
     }else if(line.StartsWith("@EXPECT-CURRENT-DIRECTORY ",StringComparison.Ordinal)){if(!String.Equals(Path.GetFullPath(Directory.GetCurrentDirectory()).TrimEnd(Path.DirectorySeparatorChar),Path.GetFullPath(line.Substring(26)).TrimEnd(Path.DirectorySeparatorChar),StringComparison.OrdinalIgnoreCase))throw new Exception("Wrong current working directory.");
     }else if(line.StartsWith("@EXPECT-CURRENT-DRIVE ",StringComparison.Ordinal)){if(Char.ToUpperInvariant(Path.GetPathRoot(Directory.GetCurrentDirectory())[0])!=line.Substring(22)[0])throw new Exception("Wrong current drive after DRV.");
     }else if(line=="@EXPECT-INPUT-COMPLETE"){if(musicInput==null||!musicInput.SaveQuestion)throw new Exception("Input replay incomplete; field="+(musicInput==null?-1:musicInput.Correction)+"; rejected="+(musicInput==null?"none":musicInput.RejectedLine)+"; error="+(musicInput==null?"none":musicInput.InputError));
      }else if(line.StartsWith("@EXPECT-MODE ",StringComparison.Ordinal)){if(mode.ToString()!=line.Substring(13))throw new Exception("Mode "+mode+" != "+line.Substring(13)+"; input="+command.Text+"; response="+responseLine.Text+"; input-error="+(musicInput==null?"none":musicInput.InputError));
     }else if(line.StartsWith("@EXPECT-MEASUREMENT ",StringComparison.Ordinal)){if(NativeMeasurements.Get(score)!=Int16.Parse(line.Substring(20),CultureInfo.InvariantCulture))throw new Exception("Measurement setting differs.");
     }else if(line.StartsWith("@EXPECT-HELP-PAGE ",StringComparison.Ordinal)){// Original screen capture strips trailing spaces from each80-cell row.
      Func<string,string> screenRows=value=>String.Join("\n",value.Replace("\r","").TrimEnd('\n').Split('\n').Select(row=>row.TrimEnd(' ')));string wanted=screenRows(File.ReadAllText(line.Substring(18)));string actual=screenRows(responseLine.Text);if(actual!=wanted){File.WriteAllText(Path.Combine(directory,"actual-help.txt"),actual);throw new Exception("Help page differs from original text-mode capture: "+Path.GetFileName(line.Substring(18)));}
     }else if(line=="@HELP-INTERACTION-GUARD"){if(letterHelpPages==null)throw new Exception("Expected active help viewer.");byte[] saved=score.Encode();var selected=canvas.Selected;Function(2);ScoreMouse(1,System.Windows.Forms.Keys.None,new System.Drawing.PointF(300,300),new System.Drawing.PointF(0,0),true);if(parameters.CanEditInline())throw new Exception("Parameter editor enabled beneath help.");if(!score.Encode().SequenceEqual(saved)||canvas.Selected!=selected||musicInput!=null||letterHelpPages==null)throw new Exception("Interaction escaped active help.");
     }else if((line.StartsWith("@HELP-TYPE ",StringComparison.Ordinal)||line.StartsWith("@COMMAND-TYPE ",StringComparison.Ordinal))){foreach(char value in line.Substring(line.StartsWith("@COMMAND",StringComparison.Ordinal)?14:11)){var key=new System.Windows.Forms.KeyPressEventArgs(value);typeof(System.Windows.Forms.Control).GetMethod("OnKeyPress",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(command,new object[]{key});if(!key.Handled)throw new Exception("Help character escaped its active help context.");}
     }else if(line.StartsWith("@EDITOR-KEY ",StringComparison.Ordinal)){var key=(System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys),line.Substring(12),true);OnKeyDown(new System.Windows.Forms.KeyEventArgs(key));
     }else if(line.StartsWith("@EXPECT-REJECTED ",StringComparison.Ordinal)){byte[] unchanged=score.Encode();bool blocked=false;try{Execute(line.Substring(17));}catch(NotSupportedException){blocked=true;}if(!blocked||!unchanged.SequenceEqual(score.Encode()))throw new Exception("Expected guarded command rejection without document mutation.");
     }else if(line.StartsWith("@EXPECT-HEIGHT-BITS ",StringComparison.Ordinal)){uint actual=BitConverter.ToUInt32(BitConverter.GetBytes(originalPageHeight),0),wanted=UInt32.Parse(line.Substring("@EXPECT-HEIGHT-BITS ".Length).Replace("0x",""),NumberStyles.HexNumber,CultureInfo.InvariantCulture);if(actual!=wanted)throw new Exception("Height bits differ: "+actual.ToString("X8")+" != "+wanted.ToString("X8"));
     }else if(line.StartsWith("@EXPECT-DP-PENDING ",StringComparison.Ordinal)){string actual=String.Join(",",disappearingStaffs.HiddenStaffs());if(actual!=line.Substring(19).Trim())throw new Exception("Pending DP table differs: "+actual);
     }else if(line.StartsWith("@EXPECT-DP-ACTIVE ",StringComparison.Ordinal)){string actual=String.Join(",",canvas.DisappearingStaffs.OrderBy(n=>n));if(actual!=line.Substring(18).Trim())throw new Exception("Active DP table differs: "+actual);
     }else if(line.StartsWith("@CHECK-DAT-FONT-PLANE ",StringComparison.Ordinal)){CheckDatFontPlane(line.Substring(22));
     }else if(line.StartsWith("@EXPECT-MOUSE-HIDDEN ",StringComparison.Ordinal)){if(originalMouseVisibility.Hidden!=Boolean.Parse(line.Substring(21)))throw new Exception("Mouse visibility differs from original capture.");
     }else if(line.StartsWith("@DAT-METRIC-BITS ",StringComparison.Ordinal)){var words=line.Substring(17).Split(' ');canvas.TextLayout.SetDatMetricBitsForVerification(Int32.Parse(words[0]),UInt32.Parse(words[1].Replace("0x",""),NumberStyles.HexNumber));File.AppendAllText(Path.Combine(directory,"verification-mutations.log"),line+Environment.NewLine);
     }else if(line.StartsWith("@EXPECT-COLORS ",StringComparison.Ordinal)){string actual=String.Join(" ",CurrentCommandColors());if(actual!=line.Substring(15))throw new Exception("COL state differs: "+actual);var palette=NativeScreenPalette.Read(source);if(canvas.BackColor!=palette.Background||top.ForeColor!=palette.Text||functionStrip.ForeColor!=palette.Text)throw new Exception("COL controls did not update.");
     }else if(line.StartsWith("@EXPECT-MENU-STAFFS ",StringComparison.Ordinal)){if(String.Join(" ",graphicalMenuStaffs.Skip(1))!=line.Substring(20))throw new Exception("Graphical menu associations differ from original.");
     }else if(line.StartsWith("@COMMAND-CHAR ",StringComparison.Ordinal)){string value=line.Substring(14);if(value.Length!=1)throw new Exception("Expected one confirmation character.");var key=new System.Windows.Forms.KeyPressEventArgs(value[0]);command.GetType().GetMethod("OnKeyPress",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(command,new object[]{key});if(!key.Handled)throw new Exception("Confirmation character was not consumed.");
     }else if(line=="@CHECK-ORIGINAL-ZOOMS"){CheckOriginalZooms();
     }else if(line=="@CHECK-DP-DISPLAY"){CheckDisappearingDisplay();
     }else if(line.StartsWith("@EXPECT-TEXT-TAB-PAUSE ",StringComparison.Ordinal)){if((originalTextTabContinuation!=null)!=Boolean.Parse(line.Substring(23)))throw new Exception("Text-tab acknowledgment boundary differs.");
      }else if(line.StartsWith("@EXPECT-TEXT-TAB-NOTIFIED ",StringComparison.Ordinal)){if(originalTextTabNotified!=Boolean.Parse(line.Substring(26)))throw new Exception("Text-tab notification state differs.");
      }else if(line.StartsWith("@EXPECT-PRINT-EXTENTS ",StringComparison.Ordinal)){var ext=NativePrintMinimum.Extents(canvas);string actual=String.Join(" ",new[]{ext.Left.ToString(CultureInfo.InvariantCulture),ext.Bottom.ToString(CultureInfo.InvariantCulture),ext.Right.ToString(CultureInfo.InvariantCulture),ext.Top.ToString(CultureInfo.InvariantCulture)});if(actual!=line.Substring(22).Trim())throw new Exception("Original F6 extents differ: "+actual);continue;}else if(line.StartsWith("@EXPECT-MESSAGE ",StringComparison.Ordinal)){if(!responseLine.Text.Contains(line.Substring(16)))throw new Exception("Expected message absent: "+line.Substring(16));
     }else if(line=="@EXPECT-MAIN"){if(groupCommand!=null)throw new Exception("Expected Main boundary; pending group="+groupCommand);
     }else if(line.StartsWith("@EXPECT-LAYOUT-PROMPT ",StringComparison.Ordinal)){
      if(!IsLayoutGroup||!String.Equals(layoutPrompt.ToString(),line.Substring(22),StringComparison.Ordinal))throw new Exception("Unexpected layout prompt: "+layoutPrompt+", group="+groupCommand);
     }else if(line.StartsWith("@EXPECT-TRANSPOSE-PROMPT ",StringComparison.Ordinal)){
      if(!IsTransposeGroup||!String.Equals(transposePrompt.ToString(),line.Substring(25),StringComparison.Ordinal))throw new Exception("Unexpected transposition prompt after key event: "+transposePrompt+", group="+groupCommand);
     }else if(line.StartsWith("@EXPECT-ITEMS ",StringComparison.Ordinal)){if(score.Items.Count!=Int32.Parse(line.Substring(14),CultureInfo.InvariantCulture))throw new Exception("Item count differs at original boundary.");
     }else if(line.StartsWith("@FORM-KEY ",StringComparison.Ordinal)){var keys=(System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys),line.Substring(10),true);var key=new System.Windows.Forms.KeyEventArgs(keys);OnKeyDown(key);if(!key.SuppressKeyPress)throw new Exception("Expected form key was not consumed.");
     }else if(line.StartsWith("@EDIT-KEY ",StringComparison.Ordinal)){
      if(mode!=ScreenMode.Edit)throw new Exception("Expected item-edit key context.");canvas.Focus();var keys=(System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys),line.Substring(10),true);var e=new System.Windows.Forms.KeyEventArgs(keys);OnKeyDown(e);if(!e.SuppressKeyPress)throw new Exception("Item-edit key was not consumed: "+keys);
     }else if(line.StartsWith("@KEY ",StringComparison.Ordinal)){
      var key=(System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys),line.Substring(5),true);
      var e=new System.Windows.Forms.KeyEventArgs(key);
      if(!HandleMainGroupKey(e)||!e.SuppressKeyPress)throw new Exception("Expected group key was not consumed: "+key);
     }else Execute(line);
    }catch(NotSupportedException){if(!refusal)throw;rejected=true;break;}
   }
   if(refusal){if(!rejected)throw new Exception("Measured original refusal was not rejected.");if(!score.Encode().SequenceEqual(before))throw new Exception("Default refusal retained a partial edit.");if(File.Exists(Path.Combine(directory,"verify-main-closed.txt"))&&groupCommand!=null)throw new Exception("Refusal left a pending command.");return;}
   string dirtyExpectation=Path.Combine(directory,"expected-dirty.txt");if(File.Exists(dirtyExpectation)&&dirty!=Boolean.Parse(File.ReadAllText(dirtyExpectation).Trim()))throw new Exception("Modified flag differs from measured original command state.");
   string absentChecks=Path.Combine(directory,"absent-output-files.txt");if(File.Exists(absentChecks))foreach(string path in File.ReadAllLines(absentChecks))if(File.Exists(path))throw new Exception("Unexpected generated output: "+path);
   string fileChecks=Path.Combine(directory,"output-files.tsv");if(File.Exists(fileChecks))foreach(string pair in File.ReadAllLines(fileChecks)){var paths=pair.Split('\t');if(paths.Length!=2&&paths.Length!=3||!File.Exists(paths[0]))throw new Exception("Generated file missing or malformed comparison: "+paths[0]);byte[] actualFile=File.ReadAllBytes(paths[0]),expectedFile=File.ReadAllBytes(paths[1]);if(paths.Length==3){if(paths[2]!="defined-mus"||Score.Load(paths[0]).Trailer.Length!=24||Score.Load(paths[1]).Trailer.Length!=24||actualFile.Length!=expectedFile.Length)throw new Exception("Invalid defined-MUS comparison.");int undefined=actualFile.Length-24;var differences=Enumerable.Range(0,actualFile.Length).Where(n=>actualFile[n]!=expectedFile[n]).ToArray();if(differences.Any(n=>n<undefined||n>=undefined+4))throw new Exception("Defined generated MUS bytes differ: "+paths[0]);File.WriteAllText(Path.Combine(directory,"output-defined-comparison.txt"),differences.Length==0?"Full-file byte equality.":"Every defined byte matches; "+differences.Length+" differing bytes in the undefined trailer word, which native saving preserves.");}else if(!actualFile.SequenceEqual(expectedFile))throw new Exception("Generated file differs: "+paths[0]);}
   commandTimer.Stop();File.WriteAllText(Path.Combine(directory,"command-performance.txt"),"Elapsed command sequence ms="+commandTimer.Elapsed.TotalMilliseconds.ToString("F3",CultureInfo.InvariantCulture)+Environment.NewLine+"Final items="+score.Items.Count+Environment.NewLine+"Includes command redraw and prompt processing; excludes initial load and verification save/undo."+Environment.NewLine);
   var expected=Score.Load(Path.Combine(directory,"expected.MUS"));
   File.WriteAllBytes(Path.Combine(directory,"actual.MUS"),score.Encode());
   if(score.Items.Count!=expected.Items.Count)throw new Exception("Record count "+score.Items.Count+" != "+expected.Items.Count);
   var errors=new List<string>();var paddingDifferences=new List<string>();bool definedTextOnly=File.Exists(Path.Combine(directory,"undefined-text-padding.txt"));
   for(int i=0;i<score.Items.Count;i++){
    var a=score.Items[i];var b=expected.Items[i];
    if(a.Count!=b.Count){errors.Add("Item "+(i+1)+" length "+a.Count+" != "+b.Count);continue;}
    for(int p=1;p<=a.Count;p++)if(BitConverter.ToInt32(BitConverter.GetBytes(a.P(p)),0)!=BitConverter.ToInt32(BitConverter.GetBytes(b.P(p)),0)){
     // Explicit per-fixture scope only: original ripple copies partial text
     // into retained scratch storage. Bytes after P12's text end are not
     // characters or parameters. Preserve/report their actual differences.
     int end=56+(int)b.P(12);bool paddingOnly=definedTextOnly&&a.Code==16&&b.Code==16&&a.P(12)==b.P(12)&&end>=56&&end<=a.Bytes.Length&&p>=14;
     if(paddingOnly)for(int at=p*4;at<p*4+4;at++)if(a.Bytes[at]!=b.Bytes[at]){if(at<end)paddingOnly=false;else paddingDifferences.Add("Item "+(i+1)+" byte "+at+": native="+a.Bytes[at].ToString("X2")+" original="+b.Bytes[at].ToString("X2"));}
     if(!paddingOnly)errors.Add("Item "+(i+1)+" staff "+a.Staff+" P"+p+" "+a.P(p).ToString("R",CultureInfo.InvariantCulture)+" != "+b.P(p).ToString("R",CultureInfo.InvariantCulture));
    }
   }
   File.WriteAllLines(Path.Combine(directory,"comparison.txt"),errors);if(definedTextOnly)File.WriteAllLines(Path.Combine(directory,"text-padding-comparison.txt"),new[]{"All record counts, numeric parameters, cached widths and P12-defined text bytes remain exact. Recorded differences below are outside the defined text string, not full-file equality."}.Concat(paddingDifferences));
   if(errors.Count>0)throw new Exception(errors.Count+" word/length differences: "+String.Join("; ",errors.Take(8)));
   if(File.Exists(Path.Combine(directory,"verify-main-closed.txt"))&&groupCommand!=null)throw new Exception("Command did not return to Main.");
   string messageExpectation=Path.Combine(directory,"expected-message-lines.txt");if(File.Exists(messageExpectation))foreach(string wanted in File.ReadAllLines(messageExpectation))if(!responseLine.Text.Split(new[]{"\r\n","\n"},StringSplitOptions.None).Contains(wanted))throw new Exception("Expected original message line absent: "+wanted);
   if(File.Exists(Path.Combine(directory,"verify-defined-save.txt"))){
    string saved=Path.Combine(directory,"native-save.MUS");Execute("SA "+saved);if(pendingLetterSave!=null)Execute("Y");
    byte[] actual=File.ReadAllBytes(saved),original=File.ReadAllBytes(Path.Combine(directory,"expected.MUS"));
    if(dirty)throw new Exception("Successful save did not clear the modified state.");
    if(actual.Length!=original.Length)throw new Exception("Saved file lengths differ.");
    int undefinedStart=actual.Length-24;
    var differences=Enumerable.Range(0,actual.Length).Where(index=>actual[index]!=original[index]).ToArray();
    if(differences.Any(index=>index<undefinedStart||index>=undefinedStart+4))throw new Exception("Defined saved bytes differ at "+differences.First(index=>index<undefinedStart||index>=undefinedStart+4));
    var reloaded=Score.Load(saved);if(!reloaded.Encode().SequenceEqual(actual))throw new Exception("Saved score reload changed bytes.");
    File.WriteAllText(Path.Combine(directory,"save-comparison.txt"),differences.Length==0?"Full-file byte equality; reload preserves bytes.":"Defined bytes equal; source undefined trailer word preserved; reload preserves bytes.");
   }
   if(File.Exists(Path.Combine(directory,"verify-undo.txt"))){
    string undoCountFile=Path.Combine(directory,"verify-undo-count.txt");int undoCount=File.Exists(undoCountFile)?Int32.Parse(File.ReadAllText(undoCountFile).Trim(),CultureInfo.InvariantCulture):1;
    if(undoCount<1||undoCount>100)throw new Exception("Invalid isolated undo verification count.");
    for(int n=1;n<=undoCount;n++){Undo(false);string intermediate=Path.Combine(directory,"undo-"+n+".MUS");if(File.Exists(intermediate)){var wanted=Score.Load(intermediate);if(score.Items.Count!=wanted.Items.Count||score.Items.Where((item,index)=>!item.Bytes.SequenceEqual(wanted.Items[index].Bytes)).Any()){File.WriteAllBytes(Path.Combine(directory,"undo-actual-"+n+".MUS"),score.Encode());throw new Exception("Undo intermediate "+n+" differs from original records.");}}}
    if(score.Items.Count!=loadedItems.Length||score.Items.Where((item,index)=>!item.Bytes.SequenceEqual(loadedItems[index].Bytes)).Any())throw new Exception("Undo did not restore loaded pre-command records.");
   }
  }
 }
 public static class NotationMatrixChecks {
  public static int Run(string root,string directory){
   int passed=0,failed=0;var results=new List<string>();
   using(var editor=new Editor(root,Path.Combine(root,"MUSDAT","PRACTICE.MUS"))){
    foreach(string test in Directory.GetDirectories(directory).OrderBy(x=>x)){
     if(!File.Exists(Path.Combine(test,"commands.txt")))continue;
     try{editor.CheckNotationMatrixCase(test);passed++;results.Add(Path.GetFileName(test)+"\tPASS");Console.WriteLine("PASS "+Path.GetFileName(test));}
     catch(Exception e){failed++;results.Add(Path.GetFileName(test)+"\tFAIL\t"+e.Message.Replace('\t',' ').Replace('\n',' '));Console.WriteLine("FAIL "+Path.GetFileName(test)+": "+e.Message);}
    }
   }
   File.WriteAllLines(Path.Combine(directory,"matrix-results.tsv"),results);
   Console.WriteLine("Notation matrix: "+passed+" passed, "+failed+" failed.");return failed==0&&passed>0?0:1;
  }
 }
}
