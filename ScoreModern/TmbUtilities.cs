using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScoreModern {
    internal sealed class TmbUtility {
        internal static readonly string[] Names = {"ACCS", "BEAM", "ES", "LABELS", "LJ", "SEP", "TRANSPOSE", "VJ"};
        const string SettingsKey = "tmb-utilities";
        internal const string IdentitySettingKey = "tmb-identity-dat";
        internal const string KeepOutputSettingKey = "tmb-keep-output";
        internal static bool KeepOutputOpen { get { return NativeSettings.Read(KeepOutputSettingKey)=="true"; } }
        internal const string ScoreFile = "SMX.MUS";
        internal string Name, Executable, Support;
        internal bool Enabled;
        internal string Caption { get { return Name == "ES" ? "EDITSCOR (ES)" : Name; } }

        internal static TmbUtility[] Read() {
            string saved = NativeSettings.Read(SettingsKey) ?? "";
            var rows = saved.Split('\n').Select(line => line.TrimEnd('\r').Split('\t')).ToArray();
            return Names.Select(name => {
                var row = rows.FirstOrDefault(fields => fields.Length == 4 && fields[0] == name);
                return new TmbUtility {
                    Name = name, Enabled = row != null && row[1] == "1",
                    Executable = row == null ? "" : row[2],
                    Support = row == null ? "" : row[3]
                };
            }).ToArray();
        }

        internal static void Save(IEnumerable<TmbUtility> utilities, string identity) {
            NativeSettings.WriteBundle(new Dictionary<string,string> {
                {SettingsKey, String.Join("\n", utilities.Select(tool =>
                    String.Join("\t", tool.Name, tool.Enabled ? "1" : "0", tool.Executable, tool.Support)))},
                {IdentitySettingKey, identity}
            });
        }

        internal string SupportFolder {
            get { return String.IsNullOrWhiteSpace(Support) ? Path.GetDirectoryName(Path.GetFullPath(Executable)) : Path.GetFullPath(Support); }
        }

        internal void Validate() {
            if (!File.Exists(Executable) || !Path.GetExtension(Executable).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Choose the executable for " + Caption + " in Preferences > TMB utilities.");
            if (!Directory.Exists(SupportFolder))
                throw new DirectoryNotFoundException("The support folder for " + Caption + " does not exist.");
        }

        // Keep the executable, support files, working MUS and the program's backups together.
        internal string Stage(string folder) {
            Validate();
            string executable = Path.GetFullPath(Executable);
            string target = Path.Combine(folder, Path.GetFileName(executable));
            foreach (string file in Directory.EnumerateFiles(SupportFolder)) {
                string name = Path.GetFileName(file);
                string extension = Path.GetExtension(file);
                if (name.Equals(Path.GetFileName(target), StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".MUS", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".PAG", StringComparison.OrdinalIgnoreCase)) continue;
                File.Copy(file, Path.Combine(folder, name));
            }
            File.Copy(executable, target);
            return target;
        }

        internal string Options(IWin32Window owner, string folder, string root, bool metric) {
            switch (Name) {
                case "ACCS": {
                    string kern = ResolveAccsInput(owner, folder, "K", "ACCS.BIN", "KERNSMX.BIN", "Choose the ACCS kerning file for your SCORE library");
                    if (kern == null) return null;
                    string config = ResolveAccsInput(owner, folder, "CFG", "CFG-10.BIN", "CFGSMX.BIN", "Choose the ACCS configuration file");
                    return config == null ? null : "/B /S /K " + kern + " /CFG " + config + " /N";
                }
                case "BEAM": return "/B /S /N";
                case "ES": {
                    string macro = PickInput(owner, folder, "Choose an EDITSCOR macro", "EDITSCOR macro|*.mac;*.txt|All files|*.*");
                    return macro == null ? null : "/M " + macro + " /PSCPATH " + Quote(NativeLibraryLocation.DirectoryFor(root, false)) + " /N";
                }
                case "LABELS": {
                    string macro = PickInput(owner, folder, "Choose a LABELS control file", "LABELS control file|*.lbl|All files|*.*");
                    if (macro == null) return null;
                    string height = IniArgument(folder, "LABELS.INI", "CAPHEIGHT");
                    decimal capHeight;
                    if (!TryPositiveDecimal(height, out capHeight)) {
                        string value;
                        if (Choose(owner, Caption, "Font metric", new[]{"CapHeight"},
                            "Enter CapHeight from the AFM for the font used by LABELS (normally slot _00).", out value) == null)
                            return null;
                        if (!TryPositiveDecimal(value, out capHeight))
                            throw new InvalidDataException("Enter a positive AFM CapHeight using a decimal point.");
                    }
                    return "/M " + macro + " /CAPHEIGHT " + capHeight.ToString(CultureInfo.InvariantCulture) +
                        " /LIBPATH " + Quote(NativeLibraryLocation.DirectoryFor(root, true)) +
                        " /PSCPATH " + Quote(NativeLibraryLocation.DirectoryFor(root, false)) + (metric ? " /METRIC /N" : " /INCH /N");
                }
                case "LJ": {
                    string mode = Choose(owner, Caption, "Operation", new[]{"Line up and justify", "Line up", "Justify"});
                    return mode == null ? null : (mode == "Line up" ? "/L" : mode == "Justify" ? "/J" : "/LJ") + " /AUTOSELECT /B /N";
                }
                case "SEP": {
                    string value;
                    string operation = Choose(owner, Caption, "Voice to delete", new[]{"Top", "Bottom"}, "Staff numbers (space separated)", out value);
                    if (operation == null) return null;
                    var staves = value.Split(new[]{' ', '\t', ','}, StringSplitOptions.RemoveEmptyEntries);
                    if (staves.Length == 0 || staves.Any(staff => {int n;return !Int32.TryParse(staff, NumberStyles.None, CultureInfo.InvariantCulture, out n) || n < 1 || n > 32;}))
                        throw new InvalidDataException("Enter one or more staff numbers from 1 through 32.");
                    return "/MS " + String.Join(" ", staves) + (operation == "Top" ? " /TOP /N" : " /BOTTOM /N");
                }
                case "TRANSPOSE": {
                    string value;
                    string operation = Choose(owner, Caption, "Identify staff by", new[]{"Staff number (P2)", "Staff ID (P9)", "Transpose macro"},
                        "ID, semitones, clef P5 (-1 to keep), key (0 to edit, -1 to keep)", out value);
                    if (operation == null) return null;
                    if (operation == "Transpose macro") {
                        string macro = PickInput(owner, folder, "Choose a transpose macro", "Transpose macro|*.mac;*.txt|All files|*.*");
                        return macro == null ? null : "/M " + macro + " /N";
                    }
                    var words = value.Split(new[]{' ', '\t', ','}, StringSplitOptions.RemoveEmptyEntries);
                    int parsed;
                    if (words.Length != 4 || words.Any(word => !Int32.TryParse(word, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out parsed)) ||
                        (words[3] != "0" && words[3] != "-1"))
                        throw new InvalidDataException("Enter four integers, for example 1 2 -1 0.");
                    return (operation == "Staff number (P2)" ? "/P2 " : "/P9 ") + String.Join(" ", words) + " /N";
                }
                case "VJ": {
                    string value;
                    string operation = Choose(owner, Caption, "Operation", new[]{"Set system height", "Report current system height"},
                        "Target height in " + (metric ? "centimetres" : "inches") + " (Set system height only)", out value);
                    if (operation == null) return null;
                    string option;
                    if (operation == "Set system height") {
                        decimal height;
                        if (!TryPositiveDecimal(value, out height))
                            throw new InvalidDataException("Enter a positive system height using a decimal point.");
                        option = "/HS " + height.ToString(CultureInfo.InvariantCulture);
                    } else option = "/HSR";
                    return option + (metric ? " /METRIC /N" : " /INCH /N");
                }
                default: throw new InvalidOperationException("Unknown TMB utility.");
            }
        }

        static string Quote(string path) {
            int trailingBackslashes = path.Length - path.TrimEnd('\\').Length;
            return "\"" + path + new string('\\', trailingBackslashes) + "\"";
        }

        static bool TryPositiveDecimal(string value, out decimal result) {
            return Decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result) && result > 0;
        }

        static string IniArgument(string folder, string iniName, string option) {
            string path = Path.Combine(folder, iniName);
            if (!File.Exists(path)) return null;
            string line = File.ReadLines(path).Skip(1).FirstOrDefault();
            if (line == null) return null;
            var match = Regex.Matches(line, @"(?:^|\s)/" + option + @"\s+(?:""([^""]+)""|([^\s/][^\s]*))",
                RegexOptions.IgnoreCase).Cast<Match>().LastOrDefault();
            return match == null ? null : match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
        }

        string ResolveAccsInput(IWin32Window owner, string folder, string option, string defaultName, string stagedName, string prompt) {
            string configured = IniArgument(folder, "ACCS.INI", option);
            string path = configured ?? defaultName;
            if (!Path.IsPathRooted(path)) path = Path.Combine(folder, path);
            if (File.Exists(path)) {
                if (!Path.IsPathRooted(configured ?? defaultName) && File.Exists(Path.Combine(folder, Path.GetFileName(path))))
                    return Quote(configured ?? defaultName);
                File.Copy(path, Path.Combine(folder, stagedName));
                return stagedName;
            }
            using (var picker = new OpenFileDialog {Title = prompt, Filter = "ACCS binary files|*.BIN|All files|*.*",
                FileName = Path.GetFileName(path), InitialDirectory = SupportFolder, RestoreDirectory = true}) {
                if (picker.ShowDialog(owner) != DialogResult.OK) return null;
                string staged = Path.Combine(folder, stagedName);
                if (File.Exists(staged)) throw new InvalidDataException("The support folder contains the reserved file " + stagedName + ".");
                File.Copy(picker.FileName, staged);
                return stagedName;
            }
        }

        internal static int RunProcess(string executable, string folder, string options,
            ConcurrentQueue<string> output, CancellationToken cancellation) {
            var start = new ProcessStartInfo(executable, ScoreFile + " " + options) {
                WorkingDirectory = folder, UseShellExecute = false, CreateNoWindow = true,
                RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true
            };
            using (var process = new Process {StartInfo = start}) {
                DataReceivedEventHandler capture = (sender, args) => { if (args.Data != null) output.Enqueue(args.Data); };
                process.OutputDataReceived += capture;
                process.ErrorDataReceived += capture;
                cancellation.ThrowIfCancellationRequested();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.StandardInput.Close();
                while (!process.WaitForExit(100)) {
                    if (!cancellation.IsCancellationRequested) continue;
                    try { process.Kill(); } catch (InvalidOperationException) { }
                    process.WaitForExit();
                    cancellation.ThrowIfCancellationRequested();
                }
                process.WaitForExit();
                cancellation.ThrowIfCancellationRequested();
                return process.ExitCode;
            }
        }

        string PickInput(IWin32Window owner, string folder, string title, string filter) {
            using (var picker = new OpenFileDialog {Title = title, Filter = filter, InitialDirectory = SupportFolder, RestoreDirectory = true}) {
                if (picker.ShowDialog(owner) != DialogResult.OK) return null;
                string name = Path.GetFileName(picker.FileName);
                // Keep the working score and staged executable reserved.
                if (name.Equals(ScoreFile, StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(name).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Choose a macro or control file.");
                string extension = Path.GetExtension(name);
                string stagedName = "SMXINPUT" + extension;
                if (File.Exists(Path.Combine(folder, stagedName)))
                    throw new InvalidDataException("The support folder contains the reserved file " + stagedName + ".");
                File.Copy(picker.FileName, Path.Combine(folder, stagedName));
                return Quote(stagedName);
            }
        }

        static string Choose(IWin32Window owner, string title, string prompt, string[] options) {
            string value;
            return Choose(owner, title, prompt, options, null, out value);
        }

        static string Choose(IWin32Window owner, string title, string prompt, string[] options, string detail, out string value) {
            using (var dialog = new ScoreDialog("tmb-options") {Text = title, AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink, StartPosition = FormStartPosition.CenterParent}) {
                var fields = new FlowLayoutPanel {AutoSize = true, FlowDirection = FlowDirection.TopDown,
                    WrapContents = false, Padding = new Padding(12), Dock = DockStyle.Fill};
                var choice = new ComboBox {DropDownStyle = ComboBoxStyle.DropDownList, Width = 300};
                choice.Items.AddRange(options);
                choice.SelectedIndex = 0;
                var input = new TextBox {Width = 300};
                fields.Controls.Add(new Label {Text = prompt, AutoSize = true});
                fields.Controls.Add(choice);
                if (detail != null) {
                    fields.Controls.Add(new Label {Text = detail, AutoSize = true});
                    fields.Controls.Add(input);
                }
                var buttons = new FlowLayoutPanel {AutoSize = true};
                var run = new Button {Text = "Run", AutoSize = true, DialogResult = DialogResult.OK};
                var cancel = new Button {Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel};
                buttons.Controls.Add(run);
                buttons.Controls.Add(cancel);
                fields.Controls.Add(buttons);
                dialog.Controls.Add(fields);
                dialog.AcceptButton = run;
                dialog.CancelButton = cancel;
                bool accepted = dialog.ShowDialog(owner) == DialogResult.OK;
                value = input.Text.Trim();
                return accepted ? (string)choice.SelectedItem : null;
            }
        }
    }

    public sealed partial class Editor {
        Score PrepareTmbInput() {
            string path = NativeSettings.Read(TmbUtility.IdentitySettingKey);
            if (String.IsNullOrWhiteSpace(path))
                return OriginalMusSave.Prepare(score, NativeScoreIdentity.Require(source, false).Serial);
            NativeScoreIdentity.Data identity;
            if (!NativeScoreIdentity.TryRead(path, out identity))
                throw new InvalidDataException("Choose a verified serial-bearing DAT in Preferences > TMB utilities.");
            return OriginalMusSave.Prepare(score, identity.Serial);
        }

        bool CanRunTmbUtility() {
            return score != null && score.Items.Count != 0 && mode == ScreenMode.Main &&
                musicInput == null && groupCommand == null && resourceEntry == null &&
                originalTextDraft == null && !originalTextSetupActive && externalDrawingNameItem == null &&
                !selectionPending && pendingDiskFilename == null && pendingLetterLoad == null &&
                pendingLetterSave == null && pendingScor4Reload == null && !pendingScor4Exit &&
                commandFileStage == CommandFileStage.None && !commandFileInputQuestion;
        }

        async void RunTmbUtility(TmbUtility tool) {
            if (!CanRunTmbUtility() || !grid.EndEdit()) return;

            string folder = Path.Combine(Path.GetTempPath(), "ScoreModern-TMB-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try {
                string executable = tool.Stage(folder);
                string options = tool.Options(this, folder, source, NativeMeasurements.Get(score) == 1);
                if (options == null) return;
                string input = Path.Combine(folder, TmbUtility.ScoreFile);
                var initial = PrepareTmbInput().Encode();
                File.WriteAllBytes(input, initial);
                if (TmbUtility.KeepOutputOpen) {
                    RunTmbWithOutput(tool, executable, folder, options, input, initial);
                    return;
                }
                var lines = new ConcurrentQueue<string>();
                int? exit = await RunTmbInBackground(tool, executable, folder, options, lines);
                string output = String.Join(Environment.NewLine, lines);
                if (!exit.HasValue) { Message(tool.Caption + " cancelled."); return; }
                string notice = LoadTmbResult(tool, input, initial, exit.Value, output);
                if (notice != null)
                    ShowCopyableList(tool.Caption, "tmb-output", notice + Environment.NewLine + output);
            } catch (Exception error) {
                Safe(() => { throw error; });
            } finally {
                // Only remove the private, uniquely named directory created above.
                try {
                    if (Directory.Exists(folder) && (File.GetAttributes(folder) & FileAttributes.ReparsePoint) == 0)
                        Directory.Delete(folder, true);
                } catch (IOException) { } catch (UnauthorizedAccessException) { }
            }
        }

        string LoadTmbResult(TmbUtility tool, string input, byte[] initial, int exit, string output) {
            if (exit != 0 || !File.Exists(input) ||
                output.IndexOf("could not process some or all", StringComparison.OrdinalIgnoreCase) >= 0 ||
                output.IndexOf("!!!! ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
                return "The utility did not return a usable score. Exit code " + exit;
            if (File.ReadAllBytes(input).SequenceEqual(initial))
                return "The utility returned no changes to the score.";
            ApplyTmbOutput(input);
            Message(tool.Caption + " result loaded. Use Undo to restore the previous score.");
            return null;
        }

        void RunTmbWithOutput(TmbUtility tool, string executable, string folder, string options,
            string input, byte[] initial) {
            using (var cancellation = new CancellationTokenSource())
            using (var dialog = new ScoreDialog("tmb-output") {Text = tool.Caption, Width = 700, Height = 360,
                MinimumSize = new Size(360, 220), StartPosition = FormStartPosition.CenterParent})
            using (var timer = new System.Windows.Forms.Timer {Interval = 100}) {
                var log = new TextBox {Dock = DockStyle.Fill, ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical};
                var close = new Button {Text = "Cancel run", Dock = DockStyle.Bottom, AutoSize = true};
                var lines = new ConcurrentQueue<string>();
                bool finished = false;
                Action drain = () => {
                    string line;
                    while (lines.TryDequeue(out line)) log.AppendText(line + Environment.NewLine);
                };
                dialog.Controls.Add(log);
                dialog.Controls.Add(close);
                dialog.SetTextOutputCancelButton(close);
                close.Click += (sender, args) => dialog.Close();
                dialog.FormClosing += (sender, args) => {
                    if (finished) return;
                    args.Cancel = true;
                    cancellation.Cancel();
                    close.Enabled = false;
                };
                timer.Tick += (sender, args) => drain();
                dialog.Shown += async (sender, args) => {
                    string notice = null;
                    try {
                        timer.Start();
                        int exit = await Task.Run(() => TmbUtility.RunProcess(executable, folder, options, lines, cancellation.Token));
                        drain();
                        notice = LoadTmbResult(tool, input, initial, exit, log.Text);
                    } catch (OperationCanceledException) {
                        Message(tool.Caption + " cancelled.");
                    } catch (Exception error) {
                        notice = error.Message;
                    } finally {
                        timer.Stop();
                        drain();
                        finished = true;
                    }
                    if (cancellation.IsCancellationRequested) { dialog.Close(); return; }
                    if (notice != null) log.AppendText(notice + Environment.NewLine);
                    if (log.TextLength == 0) log.Text = "The utility produced no text output.";
                    close.Text = "Close";
                    close.Enabled = true;
                };
                dialog.ShowDialog(this);
            }
        }

        void ApplyTmbOutput(string input) {
            var next = PrepareOriginalLoadedScore(Score.Load(input));
            next.SourcePath = current ?? "";
            using (canvas.TextLayout.PreserveAlignmentMetricHistory())
            using (var probe = new Canvas {Score = next, Library = canvas.Library, TextLayout = canvas.TextLayout})
                probe.LayoutScore();
            Checkpoint();
            score = next;
            originalLayoutMarksModified = false;
            dirty = true;
            RefreshAll(null);
            LoadParameterStorage();
            ObserveDisplayLoad(false);
        }

        async Task<int?> RunTmbInBackground(TmbUtility tool, string executable, string folder,
            string options, ConcurrentQueue<string> lines) {
            using (var cancellation = new CancellationTokenSource()) {
                FormClosingEventHandler stop = (sender, args) => {
                    args.Cancel = true;
                    cancellation.Cancel();
                };
                Message(tool.Caption + " running...");
                FormClosing += stop;
                Enabled = false;
                try {
                    return await Task.Run(() => TmbUtility.RunProcess(executable, folder, options, lines, cancellation.Token));
                } catch (OperationCanceledException) {
                    return null;
                } finally {
                    FormClosing -= stop;
                    Enabled = true;
                    command.Focus();
                }
            }
        }
    }
}
