using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
    public sealed partial class Editor {
        const string Scor4ProfileSetting = "interface-profile";
        const string CompactMenuSetting = "compact-bottom-menu";
        const string Scor4DirectoryListName = "#MUSDIR#.TMP";
        readonly TableLayoutPanel scor4Strip = new TableLayoutPanel();
        readonly Button[] scor4Buttons = new Button[20];
        bool scor4Interface, scor4FileMenu, scor4ScreenMenu, scor4FileInfoMenu, scor4CommandTwo, scor4GroupMenu, compactBottomMenu;
        bool scor4EditSecond, scor4SetupSecond;
        bool scor4UserMenu, scor4UserType;
        int scor4ItemsBank;
        int scor4SlurEditBank;
        int scor4BeamEditBank;
        enum Scor4NoteBank { Item, Marks, MoreMarks, MoveMark, Accidentals, NoteRhythm, RestRhythm, RestNumbered, LineFine, Tab, ManualTab, NumberDetails, NumberFonts, MoreNumberFonts, KeyDetails, MeterDetails, DoubleMeter, OctaveOffsets, ExternalNames, GuitarDots, GuitarFingers, GuitarName, GuitarSuffix, GuitarSize, GuitarBarre, AccordionDetails, TextDetails, TextPoints, HarpDetails, PedalDetails, CircleDetails, ShapeOffsets }
        Scor4NoteBank scor4NoteBank;
        Scor4NoteBank scor4MarkReturnBank;
        int scor4BarEditBank;
        int scor4SymbolEditBank;
        Item scor4MenuItem;
        string scor4Directory;
        string[] scor4DirectoryFiles;
        string scor4PreviousFile;
        string pendingScor4Reload;
        bool pendingScor4Path, pendingScor4Exit, confirmedScor4Exit;
        static readonly string[] Scor4ConfirmationLabels = {
            "No", "", "", "", "", "", "", "Yes", "", "",
            "", "", "", "", "", "", "", "", "", ""
        };
        static readonly string[] Scor4MainLabels = {
            "Help", "File>", "<File", "File>", "Input", "Print", "Play", "Exit", "I", "--->",
            "FullPg", "Screen>", "Group>", "EDInput", "Type", "Cut", "Paste", "UNDelet", "Items>", "User>"
        };
        static readonly string[] Scor4UserLabels = {
            "#01", "#02", "#03", "#04", "#05", "#06", "#07", "#08", "<---", "Type",
            "#09", "#10", "#11", "#12", "#13", "#14", ";", ";;", ";;;", ";;;;"
        };
        static readonly string[] Scor4FileLabels = {
            "MkDir", "Open", "SavSm", "Save", "SavNb", "SavNx", "Path", "GtMor", "<---", "--->",
            "NxtBk", "GetNext", "GetPrev", "ReadTxt", "ReStart", "RSM", "Combine", "Decom", "Delete", "Macro"
        };
        static readonly string[] Scor4FileInfoLabels = {
            "PM", "PMX", "Date", "", "", "", "", "", "<---", "",
            "", "", "", "", "", "", "", "", "", ""
        };
        static readonly string[] Scor4ScreenLabels = {
            "Bottm", "Top", "MovDn", "MovUp", "<-Mov", "Mov->", "-Zoom", "+Zoom", "<---", "",
            "Drag Screen", "", "TopDn", "TopUp", "FullScr", "ZoomFit", "Show", "SHX", "Zoom 1", ""
        };
        static readonly string[] Scor4CommandTwoLabels = {
            "SC1", "SC2", "SC3", "SC4", "SC0", "SCX", "Hght", "HS", "<--", "UNDO",
            "Dir *.DRW", "", "Dir *.EPS", "", "OS", "OH", "OT", "Pref", "Del.this file", ""
        };
        static readonly string[] Scor4GroupLabels = {
            "Move", "MvSys", "Copy", "CpSys", "Alter", "DElet", "TRnsp", "TAB", "<--", "LJ",
            "FLIp", "FLUp", "FLDn", "FLNrm", "CnWhl", "ENHrm", "VertJus", "JusText", "CntrDsh", "CuRVe"
        };
        static readonly string[] Scor4SetupLabels = {
            "Treb", "Bass", "Alto", "Tenor", "Perc", "Treb8", "Piano", "Organ", "<esc>", "-->",
            "StrQt", "Voc+Pno", "SATB", "Chr+Pno", "Orch1", "Orch2", "", "Reset", "Start", ""
        };
        static readonly string[] Scor4SetupMore = {
            "Set21", "Set22", "Set23", "Set24", "Set25", "Set26", "Set27", "Set28", "<---", "",
            "Strings", "Perc", "Brass", "WoodWnd", "", "", "", "", "", ""
        };
        static readonly string[] Scor4EditCommon = {
            "Help", "<-|->", "Copy", "Flip", "Centr", "Zm/Mv", "DElet", "Abort", "EndEd", "-->"
        };
        static readonly string[][] Scor4EditLower = {
            new[]{"Rhyth>", "Marks>", "Accis>", "OffLf", "OffRt", "-Offst", "+Offst", "NoOff", "Tab>", "User>"},
            new[]{"Rhyth", "RstDn", "RstUp", "RsFrm", "RsInv", "-Offst", "+Offst", "NoOfs", "", "User>"},
            new[]{"Trebl", "Bass", "Alto", "Tenor", "Perc", "Tab", "", "Mini", "", "User>"},
            new[]{"Line", "Dash", "Dot", "Wiggl", "Arpeg", "Cres<", "Decrs>", "Rotat", "Centr", "User>"},
            new[]{"Slur", "Tie", "Brk3", "Brk+", "CenVt", "Curve", "FLIp", "P5=P4", "Centr", "User>"},
            new[]{"AdjOfs>", "AdjFr", "AdjF?", "AdjSt", "Adj?", "AutoBm", "AutoBm>", "FLIp", "Centr", "User>"}
        };
        static readonly string[] Scor4NoteMore = {
            "NoStm", "Stem-", "Stem+", "StNrm", "CueSz", "NrmSz", "-NtHd", "+NtHd", "<---", "ED",
            "Staff-", "Staff+", "Staff0", "ADJ", "ADJ?", "ADF", "ADF?", "", "CHdRhy", ""
        };
        static readonly string[] Scor4NoteMarks = {
            "Wedge", "Accnt", "Tenut", "Stacc", "Ferma", "Plus", "NoMrk", "LRUD", "<---", "--->",
            "DnBow", "UpBow", "HvyWd", "WdgSt", "AccSt", "TenSt", "AccTn", "MOrd", "InvMo", "FlpMk"
        };
        static readonly float[] Scor4PrimaryMarkCodes = {4, 5, 6, 7, 14, 24, 0};
        static readonly float[] Scor4SecondaryMarkCodes = {11, 12, 19, 8, 9, 10, 25, 17, 18};
        static readonly float[] Scor4FingeringMarkCodes = {31, 32, 33, 34, 35, 13};
        static readonly float[] Scor4FictaTrillDotCodes = {1, 2, 3, 20, 21, 22, 23, 26, 27, 28};
        static readonly string[] Scor4NoteMoreMarks = {
            "Fing1", "Fing2", "Fing3", "Fing4", "Fing5", "Harmo", "", "LRUD", "<---", "",
            "FicFl", "FicSh", "FicNt", "Trill", "TrFlt", "TrShp", "TrNat", "2Dots", "3Dots", "4Dots"
        };
        static readonly string[] Scor4NoteMoveMark = {
            "", "MovDn", "MovUp", "MovLf", "MovRt", "NoMov", "", "", "<---", "FLM",
            "", "", "", "", "", "", "", "", "", ""
        };
        static readonly string[] Scor4NoteAccidentals = {
            "Flat", "Sharp", "Natrl", "DblFl", "DblSh", "NoAcc", "+Space", "NoSpc", "<---", "()",
            "ENH-1", "ENH+1", "", "", "", "", "-Space", "AutoSpc", "", ""
        };
        static readonly string[] Scor4ClefMore = {
            "Trb-8", "Trb+8", "Trb15", "Bas-8", "Tab4", "Tab5", "", "", "<---", "",
            "-Size", "+Size", "0Size", "OffLf", "OffRt", "NoOFf", "", "", "", ""
        };

        bool Scor4SetupActive { get { return scor4Interface && musicInput != null &&
            musicInput.Transaction == null && !musicInput.PositionsQuestion && setupEditField == 0; } }
        bool Scor4ItemActive { get { return scor4Interface && mode == ScreenMode.Edit &&
            canvas.Selected != null && Scor4EditLabelsForItem(canvas.Selected) != null &&
            !originalItemZero && !pendingEditAcknowledgement && pendingBeamTemporary == null &&
            !parameters.EditingValue && editParameter == 0; } }

        internal static bool ReadScor4InterfacePreference() {
            return NativeSettings.Read(Scor4ProfileSetting) == "scor4";
        }

        void InitializeScor4Interface() {
            scor4Interface = ReadScor4InterfacePreference();
            canvas.Scor4Interface = scor4Interface;
            compactBottomMenu = NativeSettings.Read(CompactMenuSetting) == "true";
            scor4Strip.Dock = DockStyle.Bottom;
            scor4Strip.ColumnCount = 11;
            scor4Strip.RowCount = 2;
            scor4Strip.Visible = false;
            for (int row = 0; row < 2; row++)
                scor4Strip.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            for (int column = 0; column < 10; column++)
                scor4Strip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.7f));
            scor4Strip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3));
            for (int i = 0; i < scor4Buttons.Length; i++) {
                int index = i;
                var cell = new NativeTextButton {ScoreFunction = true, DrawMenuCell = true, Dock = DockStyle.Fill, Margin = Padding.Empty,
                    Padding = Padding.Empty, FlatStyle = FlatStyle.Flat, TabStop = false};
                cell.Click += (sender, args) => Safe(() => RunScor4Key(index));
                cell.MouseDown += (sender, args) => {
                    if(args.Button!=MouseButtons.Right)return;
                    if (Scor4ItemActive && index == 5 && Scor4Labels()[index] == "Zm/Mv")
                        Safe(() => MoveScor4ItemStaff(args.X < cell.ClientSize.Width / 2 ? -1 : 1));
                    else if (Scor4RhythmActive && !scor4RhythmSecond && index >= 1 && index <= 7)
                        Safe(() => MusicInputMenuButton(index + 1, args.Button));
                };
                scor4Buttons[i] = cell;
                scor4Strip.Controls.Add(cell, i % 10, i / 10);
            }
            for (int row = 0; row < 2; row++) {
                int bankRow = row;
                var help = new NativeTextButton {ScoreFunction = true, DrawMenuCell = true, Dock = DockStyle.Fill,
                    Margin = Padding.Empty, Text = "?", TabStop = false, AccessibleName = "SCOR4 menu help"};
                help.Click += (sender, args) => Safe(() => ShowScor4BankHelp(bankRow));
                scor4Strip.Controls.Add(help, 10, row);
            }
            Controls.Add(scor4Strip);
        }

        Control BuildScor4ProfileSelector(List<Action> refreshers) {
            var row = new FlowLayoutPanel {AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(5), WrapContents = false};
            var selector = new ComboBox {DropDownStyle = ComboBoxStyle.DropDownList, AccessibleName = "Interface profile", Width = 160};
            selector.Items.AddRange(new[]{"SCORE 3.11", "SCOR4 (preview)"});
            Action refresh = () => selector.SelectedIndex = scor4Interface ? 1 : 0;
            refresh();
            selector.SelectionChangeCommitted += (sender, args) => {
                Safe(() => {
                    SetScor4Interface(selector.SelectedIndex == 1);
                    RefreshProfilePreferenceFields();
                });
                refresh();
            };
            refreshers.Add(refresh);
            row.Controls.Add(new Label {Text = "Interface profile", AutoSize = true, Margin = new Padding(2, 6, 8, 0)});
            row.Controls.Add(selector);
            var compact = new CheckBox {Text = "Compact bottom menu", AutoSize = true, Margin = new Padding(14, 5, 2, 0)};
            bool refreshing = false;
            Action refreshCompact = () => {
                refreshing = true;
                try { compact.Checked = compactBottomMenu; }
                finally { refreshing = false; }
            };
            compact.CheckedChanged += (sender, args) => {
                if (refreshing) return;
                Safe(() => {
                    NativeSettings.WriteAllText(CompactMenuSetting, compact.Checked ? "true" : "false");
                    compactBottomMenu = compact.Checked;
                    LayoutClassic();
                });
                refreshCompact();
            };
            refreshCompact();
            refreshers.Add(refreshCompact);
            row.Controls.Add(compact);
            return row;
        }

        Control BuildProfileLibrarySelector(List<Action> refreshers, bool forScor4) {
            var row = new FlowLayoutPanel {AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(5,0,5,3), WrapContents = false};
            string caption = forScor4 ? "SCORE 4 LIB folder" : "SCORE 3.11 LIB folder";
            var field = new TextBox {Width = 250, AccessibleName = caption};
            var browse = new Button {Text = "Browse…", AutoSize = true};
            var all = new CheckBox {Text = "All resources", AutoSize = true};
            bool refreshing = false;
            Func<NativeLibraryLocation.Selection> selected = () => NativeLibraryLocation.GetSelection(source,forScor4);
            Func<string> currentFolder = () => selected() == null ? "" : selected().Folder;
            Action refresh = () => {
                refreshing = true;
                try {
                    field.Text = currentFolder();
                    all.Enabled = field.Text.Length != 0;
                    all.Checked = selected() != null && selected().All;
                } finally { refreshing = false; }
            };
            Action apply = () => {
                string folder = field.Text.Trim();
                if (!refreshing && !String.Equals(folder,currentFolder(),StringComparison.OrdinalIgnoreCase))
                    Safe(() => {
                        if(forScor4&&selected()==null)SetScor4LibraryFolder(folder);
                        else SetProfileLibraryFolder(forScor4,folder,selected()!=null&&selected().All);
                        RefreshProfilePreferenceFields();
                    });
                refresh();
            };
            all.CheckedChanged += (sender,args) => {
                if(refreshing || !all.Enabled) return;
                Safe(() => {
                    SetProfileLibraryFolder(forScor4,currentFolder(),all.Checked);
                    RefreshProfilePreferenceFields();
                });
                refresh();
            };
            field.Validated += (sender,args) => apply();
            field.KeyDown += (sender,args) => {
                if(args.KeyCode==Keys.Enter){args.SuppressKeyPress=true;apply();}
                else if(args.KeyCode==Keys.Escape){args.SuppressKeyPress=true;refresh();}
            };
            browse.Click += (sender,args) => {
                using(var dialog=new FolderBrowserDialog {Description="Select the "+caption,SelectedPath=currentFolder()})
                    if(dialog.ShowDialog(row.FindForm())==DialogResult.OK){field.Text=dialog.SelectedPath;apply();}
            };
            row.Controls.Add(new Label {Text=caption,AutoSize=true,Margin=new Padding(2,6,8,0)});
            row.Controls.Add(field);
            row.Controls.Add(browse);
            row.Controls.Add(all);
            refreshers.Add(() => {if(!field.Focused)refresh();});
            refresh();
            return row;
        }

        void SetScor4LibraryFolder(string folder) {
            SetProfileLibraryFolder(true,folder,true);
        }

        void SetProfileLibraryFolder(bool forScor4,string folder,bool all) {
            if(!String.IsNullOrEmpty(folder)&&!Directory.Exists(folder))
                throw new DirectoryNotFoundException("The selected LIB folder does not exist.");
            var before=NativeLibraryLocation.GetSelection(source,forScor4);
            bool active=forScor4==scor4Interface || !forScor4&&NativeLibraryLocation.GetSelection(source,true)==null;
            WithLibraryChange(
                ()=>NativeLibraryLocation.SetSelection(source,forScor4,folder,all),
                ()=>NativeLibraryLocation.SetSelection(source,forScor4,before==null?null:before.Folder,before!=null&&before.All),
                active,()=>NativeLibraryLocation.SaveSelection(source,forScor4));
            if(active)ApplyPreferenceColors();
        }

        void RefreshProfilePreferenceFields() {
            if(refreshPreparedScoreData!=null)refreshPreparedScoreData();
            if(refreshPreferencesSession!=null)refreshPreferencesSession();
        }

        void MoveScor4ItemStaff(int step) {
            int staff = canvas.Selected.Staff + step;
            if (staff < 1 || staff > 32) return;
            EditOriginalParameters(new[]{2f, (float)staff});
        }

        void SetScor4Interface(bool enabled) {
            bool changed=enabled!=scor4Interface;
            WithLibraryChange(
                ()=>NativeLibraryLocation.SetProfile(source,enabled),
                ()=>NativeLibraryLocation.SetProfile(source,scor4Interface),
                changed&&NativeLibraryLocation.GetSelection(source,true)!=null,
                ()=>NativeSettings.WriteAllText(Scor4ProfileSetting,enabled?"scor4":"score311"));
            scor4Interface = enabled;
            canvas.Scor4Interface = enabled;
            canvas.Scor4LiteralZoom = false;
            canvas.Scor4DragScreen = false;
            scor4ScreenDragStart = null;
            if(!enabled&&(groupCommand=="CT"||groupCommand=="PST")){
                ClearMainGroup();
                Message("");
            }
            scor4FileMenu = false;
            scor4ScreenMenu = false;
            scor4FileInfoMenu = false;
            scor4CommandTwo = false;
            scor4GroupMenu = false;
            scor4UserMenu = scor4UserType = false;
            scor4ItemsBank = 0;
            pendingScor4Preset = null;
            pendingScor4Reload = null;
            pendingScor4Exit = false;
            if(pendingScor4Path){pendingDriveChange=false;pendingScor4Path=false;}
            scor4SetupSecond = false;
            scor4RhythmSecond = false;
            scor4MarksBank = 0;
            scor4PitchBank = Scor4PitchBank.Main;
            scor4NoteBank = Scor4NoteBank.Item;
            scor4BeamEditBank = 0;
            scor4BarEditBank = 0;
            scor4SymbolEditBank = 0;
            if(changed)ApplyPreferenceColors();
            UpdateClassic();
            LayoutClassic();
        }

        bool Scor4MenuActive {
            get {
                return scor4Interface && (mode == ScreenMode.Main || Scor4ItemActive || Scor4SetupActive || Scor4StaffMenusActive) &&
                    (musicInput == null || Scor4SetupActive || Scor4StaffMenusActive) && !originalTextSetupActive &&
                    originalTextDraft == null && !selectionPending && letterHelpPages == null && !inlineTextActive &&
                    pendingLetterSave == null && pendingLetterLoad == null && resourceEntry == null &&
                    groupCommand == null && drawExportStage == DrawExportStage.None && !pendingParameterExport &&
                    pendingFilteredEdit == null && pendingDiskFilename == null && pendingDriveChange == false &&
                    pendingDiskDelete == null && pendingAppendPath == null && pendingAppendScore == null &&
                    pendingGraphicalMenu == 0 && !pendingCommandColors && originalMarginPlan == null &&
                    originalTextTabContinuation == null && commandFileStage == CommandFileStage.None &&
                    !commandFileInputQuestion && tabCommandStage == TabCommandStage.None &&
                    pageHeightStage == PageHeightStage.None && externalDrawingNameItem == null;
            }
        }

        void UpdateScor4Menu() {
            if (scor4MenuItem != canvas.Selected) {
                scor4MenuItem = canvas.Selected;
                scor4EditSecond = false;
                scor4SlurEditBank = 0;
                scor4BeamEditBank = 0;
                scor4BarEditBank = 0;
                scor4SymbolEditBank = 0;
                scor4NoteBank = Scor4NoteBank.Item;
            }
            bool active = Scor4MenuActive;
            bool showMenu = !inputFullGraphics && !inputMenuHidden;
            functionStrip.Visible = showMenu && !active;
            scor4Strip.Visible = showMenu && active;
            canvas.DosClipBottom = (short)(inputFullGraphics ? 479 : inputMenuHidden ? 478 : active ? 446 : 461);
            if (!active || !showMenu) return;
            var labels = Scor4Labels();
            bool screen = scor4ScreenMenu && !Scor4ItemActive && !Scor4SetupActive;
            scor4Strip.SetColumnSpan(scor4Buttons[10], screen ? 2 : 1);
            for (int i = 0; i < scor4Buttons.Length; i++) {
                scor4Buttons[i].Visible = !(screen && i == 11);
                scor4Buttons[i].Text = labels[i].Length == 0 ? "" : (i >= 10 ? "A" : "") + "F" + (i % 10 + 1) + " " + labels[i];
                scor4Buttons[i].Enabled = Scor4Action(i) != null;
                scor4Buttons[i].BackColor = functionKeys[0].BackColor;
                scor4Buttons[i].ForeColor = functionKeys[0].ForeColor;
            }
        }

        string[] Scor4Labels() {
            if (pendingScor4Exit || pendingScor4Reload != null) return Scor4ConfirmationLabels;
            if (scor4UserMenu) return Scor4UserLabels;
            if (Scor4SetupActive) return scor4SetupSecond ? Scor4SetupMore : Scor4SetupLabels;
            if (Scor4PitchActive) return CurrentScor4PitchLabels();
            if (Scor4RhythmActive) return scor4RhythmSecond ? Scor4RhythmMore : Scor4RhythmLabels;
            if (Scor4MarksActive) return CurrentScor4MarksLabels();
            if (Scor4BeamActive) return Scor4BeamLabels;
            if (Scor4SlurActive) return Scor4SlurLabels;
            if (Scor4ItemActive) {
                int code = canvas.Selected.Code;
                if (code == 5 && scor4SlurEditBank != 0)
                    return Scor4SlurEditLabels[scor4SlurEditBank - 1];
                if (code == 6 && scor4BeamEditBank != 0)
                    return Scor4BeamEditLabels[scor4BeamEditBank - 1];
                if (code == 14 && scor4BarEditBank != 0)
                    return Scor4MoreBarLabels[scor4BarEditBank - 1];
                if (code == 9 && scor4SymbolEditBank != 0)
                    return Scor4SymbolEditLabels[scor4SymbolEditBank - 1];
                if (scor4NoteBank == Scor4NoteBank.Marks) return Scor4NoteMarks;
                if (scor4NoteBank == Scor4NoteBank.MoreMarks) return Scor4NoteMoreMarks;
                if (scor4NoteBank == Scor4NoteBank.MoveMark) return Scor4NoteMoveMark;
                if (scor4NoteBank == Scor4NoteBank.Accidentals) return Scor4NoteAccidentals;
                if (scor4NoteBank == Scor4NoteBank.NoteRhythm) return Scor4NoteRhythmLabels;
                if (scor4NoteBank == Scor4NoteBank.RestRhythm) return Scor4RestRhythmLabels;
                if (scor4NoteBank == Scor4NoteBank.RestNumbered) return Scor4RestNumberedLabels;
                if (scor4NoteBank == Scor4NoteBank.LineFine) return Scor4LineFine;
                if (scor4NoteBank == Scor4NoteBank.Tab) return Scor4NoteTabLabels;
                if (scor4NoteBank == Scor4NoteBank.ManualTab) return Scor4NoteManualTabLabels;
                if (scor4NoteBank == Scor4NoteBank.NumberDetails) return Scor4NumberDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.NumberFonts)
                    return canvas.Selected.Code == 18 || canvas.Selected.Code == 16 ? Scor4MeterFontsLabels : Scor4NumberFontsLabels[0];
                if (scor4NoteBank == Scor4NoteBank.MoreNumberFonts) return Scor4NumberFontsLabels[1];
                if (scor4NoteBank == Scor4NoteBank.KeyDetails) return Scor4KeyDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.MeterDetails) return Scor4MeterDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.DoubleMeter) return Scor4DoubleMeterLabels;
                if (scor4NoteBank == Scor4NoteBank.OctaveOffsets) return Scor4OctaveOffsetLabels;
                if (scor4NoteBank == Scor4NoteBank.HarpDetails) return Scor4HarpDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.PedalDetails) return Scor4PedalDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.CircleDetails) return Scor4CircleDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.ShapeOffsets)
                    return canvas.Selected.P(5) == 2 ? Scor4BendDetailLabels : Scor4ShapeOffsetLabels;
                if (scor4NoteBank == Scor4NoteBank.ExternalNames) return Scor4ExternalNamesLabels;
                if (scor4NoteBank == Scor4NoteBank.GuitarDots) return Scor4GuitarDotsLabels;
                if (scor4NoteBank == Scor4NoteBank.GuitarFingers) return Scor4GuitarFingerLabels;
                if (scor4NoteBank == Scor4NoteBank.GuitarName) return Scor4GuitarNameLabels;
                if (scor4NoteBank == Scor4NoteBank.GuitarSuffix) return Scor4GuitarSuffixLabels;
                if (scor4NoteBank == Scor4NoteBank.GuitarSize) return Scor4GuitarSizeLabels;
                if (scor4NoteBank == Scor4NoteBank.GuitarBarre) return Scor4GuitarBarreLabels;
                if (scor4NoteBank == Scor4NoteBank.AccordionDetails) return Scor4AccordionDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.TextDetails) return Scor4TextDetailsLabels;
                if (scor4NoteBank == Scor4NoteBank.TextPoints) return Scor4TextPointsLabels;
                if (scor4EditSecond) {
                    if (code == 1) return Scor4NoteMore;
                    if (code == 4) return Scor4LineMore;
                    return Scor4ClefMore;
                }
                var labels = Scor4EditCommon.Concat(Scor4EditLabelsForItem(canvas.Selected)).ToArray();
                if (code == 2) labels[3] = "CnWhl";
                else if (code == 3) labels[3] = "Front";
                else if (code == 14) labels[3] = "0/200";
                else if (code == 9 || code == 11 || code == 15 || code == 16) labels[3] = "Flip";
                else if (code == 10 || code == 17 || code == 18) labels[3] = "CntMs";
                else if (code == 12) labels[3] = canvas.Selected.P(5) == 0 || canvas.Selected.P(5) == 1 ? "Fill" :
                    canvas.Selected.P(5) == 2 ? "Flip" : "";
                else if (code >= 4) { labels[3] = "MovLf"; labels[4] = "MovRt"; }
                if (code == 8) labels[9] = "";
                if (code == 7 && canvas.Selected.P(7) == 0) labels[9] = "";
                return labels;
            }
            if (scor4ItemsBank != 0) return Scor4ItemsLabels[scor4ItemsBank - 1];
            return scor4ScreenMenu ? Scor4ScreenLabels : scor4FileInfoMenu ? Scor4FileInfoLabels :
                scor4FileMenu ? Scor4FileLabels : scor4CommandTwo ? Scor4CommandTwoLabels :
                scor4GroupMenu ? Scor4GroupLabels : Scor4MainLabels;
        }

        void ShowScor4BankHelp(int row) {
            var labels = Scor4Labels();
            var entries = Enumerable.Range(row * 10, 10).Where(index => labels[index].Length != 0)
                .Select(index => (row == 0 ? "" : "Alt+") + "F" + (index % 10 + 1) + "   " + labels[index] +
                    (Scor4Action(index) == null ? "   (unavailable)" : ""));
            ShowCopyableList("SCOR4 menu help", "scor4-menu-help", String.Join(Environment.NewLine, entries));
        }

        bool HandleScor4MenuKey(KeyEventArgs key) {
            if (pendingScor4Exit) {
                key.SuppressKeyPress = true;
                if (key.Modifiers == Keys.None) {
                    if (key.KeyCode == Keys.F8) Safe(() => FinishScor4Exit(true));
                    else if (key.KeyCode == Keys.F1 || key.KeyCode == Keys.Escape) FinishScor4Exit(false);
                }
                return true;
            }
            if (HandleScor4ReloadPrompt(key)) return true;
            if (HandleScor4PresetConfirmation(key)) return true;
            if (!Scor4MenuActive) return false;
            if (scor4UserMenu && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                ReturnScor4Bank();
                return true;
            }
            if (Scor4ItemActive && canvas.Selected.Code == 5 && (editEndpoint == 3 || scor4SlurEditBank != 0) && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                if (editEndpoint == 3) {
                    editEndpoint = 0;
                    UpdateClassic();
                } else SetScor4SlurEditBank(0);
                return true;
            }
            if (Scor4ItemActive && scor4NoteBank != Scor4NoteBank.Item && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                Safe(() => RunScor4Key(8));
                return true;
            }
            if (Scor4ItemActive && scor4BeamEditBank != 0 && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                SetScor4BeamEditBank(scor4BeamEditBank - 1);
                return true;
            }
            if (Scor4ItemActive && scor4BarEditBank != 0 && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                SetScor4BarEditBank(scor4BarEditBank - 1);
                return true;
            }
            if (Scor4ItemActive && scor4SymbolEditBank != 0 && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                SetScor4SymbolEditBank(scor4SymbolEditBank - 1);
                return true;
            }
            if (Scor4ItemActive && scor4EditSecond && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                Safe(() => RunScor4Key(8));
                return true;
            }
            if (!Scor4SetupActive && !Scor4ItemActive && !Scor4StaffMenusActive &&
                (scor4FileMenu || scor4ScreenMenu || scor4CommandTwo || scor4GroupMenu || scor4UserMenu || scor4ItemsBank != 0) && key.KeyCode == Keys.Escape) {
                key.SuppressKeyPress = true;
                ReturnScor4Bank();
                return true;
            }
            if (key.KeyCode < Keys.F1 || key.KeyCode > Keys.F10 ||
                (key.Modifiers != Keys.None && key.Modifiers != Keys.Alt) ||
                (key.KeyCode == Keys.F4 && key.Alt && !scor4ScreenMenu && !scor4FileMenu && !scor4GroupMenu && !scor4UserMenu && scor4ItemsBank == 0 &&
                    !Scor4ItemActive && !Scor4SetupActive && !Scor4StaffMenusActive)) return false;
            key.SuppressKeyPress = true;
            Safe(() => RunScor4Key(key.KeyCode - Keys.F1 + (key.Alt ? 10 : 0)));
            return true;
        }

        void ReturnScor4Bank() {
            if (scor4ItemsBank != 0) {
                SetScor4ItemsBank(scor4ItemsBank - 1);
                return;
            }
            if (scor4FileInfoMenu) scor4FileInfoMenu = false;
            else { scor4FileMenu = false; scor4ScreenMenu = false; scor4CommandTwo = false; scor4GroupMenu = false; }
            scor4UserMenu = scor4UserType = false;
            UpdateClassic();
        }

        void RunScor4Key(int index) {
            if (!Scor4MenuActive) return;
            var action = Scor4Action(index);
            if (action == null) Message("This SCOR4 menu action is not available in the preview.");
            else action();
        }

        void SetScor4NoteBank(Scor4NoteBank bank) {
            scor4NoteBank = bank;
            UpdateClassic();
        }

        void BeginScor4MarkMovement() {
            scor4MarkReturnBank = scor4NoteBank;
            SetScor4NoteBank(Scor4NoteBank.MoveMark);
        }

        Action Scor4NoteBankAction(int index) {
            switch (scor4NoteBank) {
                case Scor4NoteBank.TextDetails:
                    return Scor4TextDetailsAction(index);
                case Scor4NoteBank.TextPoints:
                    return Scor4TextPointsAction(index);
                case Scor4NoteBank.AccordionDetails:
                    return Scor4AccordionDetailsAction(index);
                case Scor4NoteBank.GuitarDots:
                case Scor4NoteBank.GuitarFingers:
                    return Scor4GuitarStringAction(index);
                case Scor4NoteBank.GuitarName:
                    return Scor4GuitarNameAction(index);
                case Scor4NoteBank.GuitarSuffix:
                    return Scor4GuitarSuffixAction(index);
                case Scor4NoteBank.GuitarSize:
                    return Scor4GuitarSizeAction(index);
                case Scor4NoteBank.GuitarBarre:
                    return Scor4GuitarBarreAction(index);
                case Scor4NoteBank.ExternalNames:
                    return Scor4ExternalNamesAction(index);
                case Scor4NoteBank.Marks:
                    if (index < Scor4PrimaryMarkCodes.Length)
                        return () => EditOriginalParameters(new[]{11f, Scor4PrimaryMarkCodes[index], 13f, 0f, 14f, 0f});
                    if (index == 7) return BeginScor4MarkMovement;
                    if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
                    if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.MoreMarks);
                    if (index >= 10 && index < 19) return () => SetScor4SecondaryMark(Scor4SecondaryMarkCodes[index - 10]);
                    if (index == 19) return FlipScor4Mark;
                    break;
                case Scor4NoteBank.MoreMarks:
                    if (index < Scor4FingeringMarkCodes.Length) return () => SetScor4SecondaryMark(Scor4FingeringMarkCodes[index]);
                    if (index == 7) return BeginScor4MarkMovement;
                    if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Marks);
                    if (index >= 10) return () => SetScor4SecondaryMark(Scor4FictaTrillDotCodes[index - 10]);
                    break;
                case Scor4NoteBank.MoveMark:
                    if (index >= 1 && index <= 4)
                        return () => Execute(new[]{"MKD", "MKU", "MKL", "MKR"}[index - 1]);
                    if (index == 5) return () => EditOriginalParameters(new[]{13f, 0f, 14f, 0f});
                    if (index == 8) return () => SetScor4NoteBank(scor4MarkReturnBank);
                    if (index == 9) return FlipScor4Mark;
                    break;
                case Scor4NoteBank.Accidentals:
                    if (index < 3) return () => Execute(new[]{"FLT", "SHP", "NAT"}[index]);
                    if (index == 3 || index == 4) return () => ChangeScor4DoubleAccidental(index + 1);
                    if (index == 5) return ClearScor4Accidental;
                    if (index == 6 || index == 7 || index == 16) return () => ChangeScor4AccidentalSpacing(index == 6 ? .02f : index == 16 ? -.02f : 0f);
                    if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
                    if (index == 9) return ToggleScor4AccidentalParentheses;
                    if (index == 10 || index == 11) return () => Execute(index == 10 ? "ENH -1" : "ENH 1");
                    if (index == 17) return AutoSpaceScor4Accidental;
                    break;
                case Scor4NoteBank.RestRhythm:
                    return Scor4RestRhythmAction(index);
                case Scor4NoteBank.NoteRhythm:
                    return Scor4NoteRhythmAction(index);
                case Scor4NoteBank.RestNumbered:
                    return Scor4RestNumberedAction(index);
                case Scor4NoteBank.LineFine:
                    return Scor4LineFineAction(index);
                case Scor4NoteBank.Tab:
                    return Scor4NoteTabAction(index);
                case Scor4NoteBank.ManualTab:
                    return Scor4NoteManualTabAction(index);
                case Scor4NoteBank.NumberDetails:
                    return Scor4NumberDetailsAction(index);
                case Scor4NoteBank.NumberFonts:
                case Scor4NoteBank.MoreNumberFonts:
                    return Scor4NumberFontsAction(index);
                case Scor4NoteBank.KeyDetails:
                    return Scor4KeyDetailsAction(index);
                case Scor4NoteBank.MeterDetails:
                    return Scor4MeterDetailsAction(index);
                case Scor4NoteBank.DoubleMeter:
                    return Scor4DoubleMeterAction(index);
                case Scor4NoteBank.OctaveOffsets:
                    return Scor4OctaveOffsetAction(index);
                case Scor4NoteBank.HarpDetails:
                    return Scor4HarpDetailsAction(index);
                case Scor4NoteBank.PedalDetails:
                    return Scor4PedalDetailsAction(index);
                case Scor4NoteBank.CircleDetails:
                    return Scor4CircleDetailsAction(index);
                case Scor4NoteBank.ShapeOffsets:
                    if (canvas.Selected.P(5) == 2 && index >= 10) return Scor4BendDetailAction(index);
                    return Scor4ShapeOffsetAction(index);
            }
            return null;
        }

        Action Scor4Action(int index) {
            if (pendingScor4Exit) {
                if (index == 0 || index == 7) return () => FinishScor4Exit(index == 7);
                return null;
            }
            if (pendingScor4Reload != null) {
                if (index == 0 || index == 7) return () => FinishScor4Reload(index == 7);
                return null;
            }
            if (scor4UserMenu) return Scor4UserAction(index);
            if (Scor4PitchActive) return Scor4PitchAction(index);
            if (Scor4RhythmActive) return Scor4RhythmAction(index);
            if (Scor4MarksActive) return Scor4MarksAction(index);
            if (Scor4BeamActive || Scor4SlurActive) return Scor4EndpointAction(index,Scor4BeamActive);
            if (Scor4SetupActive) {
                if (pendingScor4Preset != null) return null;
                if (scor4SetupSecond) {
                    if (index >= 0 && index < 8) return () => LoadScor4Preset(21 + index);
                    if (index >= 10 && index < 14) return () => LoadScor4Preset(index + 5);
                    if (index == 8) return () => SetScor4SetupBank(false);
                    return null;
                }
                if (index >= 0 && index < 8) return () => LoadScor4Preset(index + 1);
                if (index >= 10 && index < 16) return () => LoadScor4Preset(index - 1);
                switch (index) {
                    case 8: return EscapeMusicInput;
                    case 9: return () => SetScor4SetupBank(true);
                    case 17: return UndoInputPreset;
                    case 18: return BeginInputStaff;
                }
                return null;
            }
            if (Scor4ItemActive) {
                int code = canvas.Selected.Code;
                if (code == 5 && scor4SlurEditBank != 0) return Scor4SlurEditAction(index);
                if (code == 6 && scor4BeamEditBank != 0) return Scor4BeamEditAction(index);
                if (code == 14 && scor4BarEditBank != 0) return Scor4MoreBarAction(index);
                if (code == 9 && scor4SymbolEditBank != 0) return Scor4MoreSymbolAction(index);
                if (scor4NoteBank != Scor4NoteBank.Item) return Scor4NoteBankAction(index);
                if (scor4EditSecond) {
                    if (index == 8) return () => { scor4EditSecond = false; UpdateClassic(); };
                    if (code == 1) return Scor4NoteMoreAction(index);
                    if (code == 4) return Scor4LineMoreAction(index);
                    if (code == 3 && index < 6) {
                        float value = new[]{.8f, .88f, .15f, 1.8f, 44f, 55f}[index];
                        return () => EditOriginalParameters(new[]{5f, value});
                    }
                    if (code == 3 && index >= 10 && index <= 15)
                        return () => EditScor4ClefSizeAndOffset(index);
                    return null;
                }
                if (code == 3 && index >= 10 && index <= 15) {
                    float value = index == 15 ? 66 : index - 10;
                    return () => EditOriginalParameters(new[]{5f, value});
                }
                if (code == 3 && index == 17) return () => SetScor4ItemCueSize(true);
                if (code == 2 && index >= 10 && index <= 17)
                    return Scor4RestPositionAction(index);
                if (code == 4 && index >= 10 && index <= 18)
                    return Scor4LineAction(index);
                if (code == 5 && index >= 10 && index <= 13)
                    return Scor4PrimarySlurAction(index);
                if (code == 5 && index >= 14 && index <= 18) {
                    if (index == 17) return () => EditOriginalParameters(new[]{5f,canvas.Selected.P(4)});
                    string verb = index == 14 ? "CV" : index == 15 ? "CR" : index == 16 ? "FLI" : "CN";
                    return () => Execute(verb);
                }
                if (code == 6 && index == 10) return () => EditOriginalParameters(new[]{9f,-1f});
                if (code == 6 && index >= 11 && index <= 14)
                    return () => AdjustScor4SelectedBeam(index <= 12, index == 12 || index == 14);
                if (code == 6 && (index == 15 || index == 16)) return () => ConfigureScor4SelectedBeam(index == 16);
                if (code == 6 && index == 17) return () => Execute("FLI");
                if (code == 6 && index == 18) return () => Execute("CN");
                if (code == 8 && index >= 10 && index <= 18)
                    return Scor4StaffEditAction(index);
                if (code == 14 && (index == 3 || index >= 10 && index <= 18))
                    return Scor4BarEditAction(index);
                if (code == 10 && (index == 3 || index == 9 || index >= 10 && index <= 17))
                    return Scor4NumberEditAction(index);
                if ((code == 17 || code == 18) && index >= 10 && index <= 18)
                    return Scor4SignatureAction(code,index);
                if ((code == 17 || code == 18) && index == 3) return () => Execute("CM");
                if (code == 9 && (index == 3 || index == 9 || index >= 10 && index <= 18))
                    return Scor4LibrarySymbolAction(index);
                if ((code == 11 || code == 15) && index >= 9 && index <= 18)
                    return Scor4ExternalDrawingEditAction(index);
                if (code == 12 && (canvas.Selected.P(5) == 0 || canvas.Selected.P(5) == 1) && index >= 10)
                    return Scor4ShapeAction(index);
                if (code == 12 && index >= 9 && canvas.Selected.P(5) == 10 && index <= 18)
                    return Scor4AccordionAction(index);
                if (code == 12 && index >= 10 && canvas.Selected.P(5) == 2)
                    return Scor4BendAction(index);
                if (code == 12 && index >= 9 && IsScor4GuitarGrid(canvas.Selected))
                    return Scor4GuitarPrimaryAction(index);
                if (code == 16 && index >= 9 && index <= 18) return Scor4TextPrimaryAction(index);
                if (code == 7 && IsScor4OctaveMark(canvas.Selected)) {
                    if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.OctaveOffsets);
                    if (index >= 10 && index <= 16) return Scor4OctaveEditAction(index);
                    if (index == 18) return () => Execute("CN");
                }
                if (code == 7 && canvas.Selected.P(7) == 0 && index >= 10 && index <= 18)
                    return Scor4TrillEditAction(index);
                if (code == 7 && index >= 10 && index <= 18) {
                    if (IsScor4HarpDiagram(canvas.Selected)) return Scor4HarpDiagramAction(index);
                    if (canvas.Selected.P(7) == 20) return Scor4GraphicPedalAction(index);
                    if (canvas.Selected.P(7) == 21 || canvas.Selected.P(7) == 22)
                        return Scor4PedalLineAction(index);
                }
                switch (index) {
                    case 0: return Reference;
                    case 1: return () => Function(2);
                    case 2: return () => Execute("CC");
                    case 3:
                        if (code == 12 && (canvas.Selected.P(5) == 0 || canvas.Selected.P(5) == 1))
                            return () => EditOriginalParameters(new[]{13f,canvas.Selected.P(13) == 0 ? 1f : 0f});
                        if (code == 12 && canvas.Selected.P(5) == 2) return () => Execute("FLI");
                        if (code == 1 || code == 11 || code == 15 || code == 16) return () => Execute("FLI");
                        if (code == 2) return () => Execute("CW");
                        if (code == 3) return () => EditOriginalParameters(new[]{3f,1.5f});
                        return code >= 4 && code <= 8 ? (Action)(() => SelectScor4EditEndpoint(1)) : null;
                    case 4:
                        if (code <= 3 || code >= 9) return () => Execute("CN");
                        return code >= 4 && code <= 8 ? (Action)(() => SelectScor4EditEndpoint(2)) : null;
                    case 5: return () => Execute("ZZ");
                    case 6: return () => Function(7);
                    case 7: return AbortCurrent;
                    case 8: return () => ExitEdit(false);
                    case 9:
                        if (code == 12 && canvas.Selected.P(5) == 1)
                            return () => SetScor4NoteBank(Scor4NoteBank.CircleDetails);
                        if (code == 12 && (canvas.Selected.P(5) == 0 || canvas.Selected.P(5) == 2))
                            return () => SetScor4NoteBank(Scor4NoteBank.ShapeOffsets);
                        if (code == 7 && IsScor4HarpDiagram(canvas.Selected))
                            return () => SetScor4NoteBank(Scor4NoteBank.HarpDetails);
                        if (code == 7 && canvas.Selected.P(7) >= 20 && canvas.Selected.P(7) <= 22)
                            return () => SetScor4NoteBank(Scor4NoteBank.PedalDetails);
                        if (code == 2) return () => SetScor4NoteBank(Scor4NoteBank.RestRhythm);
                        if (code == 5) return () => SetScor4SlurEditBank(1);
                        if (code == 6) return () => SetScor4BeamEditBank(1);
                        if (code == 14) return () => SetScor4BarEditBank(1);
                        if (code == 17) return () => { scor4KeyAccidentalParameter=0; SetScor4NoteBank(Scor4NoteBank.KeyDetails); };
                        if (code == 18) return () => SetScor4NoteBank(Scor4NoteBank.MeterDetails);
                        return code == 1 || code == 3 || code == 4 ? (Action)(() => { scor4EditSecond = true; UpdateClassic(); }) : null;
                    case 10: return code == 1 ? (Action)(() => SetScor4NoteBank(Scor4NoteBank.NoteRhythm)) : null;
                    case 11: return code == 1 ? (Action)(() => SetScor4NoteBank(Scor4NoteBank.Marks)) : null;
                    case 12: return code == 1 ? (Action)(() => SetScor4NoteBank(Scor4NoteBank.Accidentals)) : null;
                    case 13: return canvas.Selected.Code == 1 ? (Action)(() => Execute("10 20")) : null;
                    case 14: return canvas.Selected.Code == 1 ? (Action)(() => Execute("10 10")) : null;
                    case 15: case 16:
                        return code == 1 ? (Action)(() => EditOriginalParameters(new[]{1000f,index == 15 ? -.5f : .5f})) : null;
                    case 17: return canvas.Selected.Code == 1 ? (Action)(() => Execute("10 0")) : null;
                    case 18: return code == 1 ? (Action)(() => SetScor4NoteBank(Scor4NoteBank.Tab)) : null;
                    case 19:
                        return Scor4EditLabelsForItem(canvas.Selected)[9] == "User>"
                            ? (Action)OpenScor4UserMenu : null;
                }
                return null;
            }
            if (scor4ItemsBank != 0) return Scor4ItemsAction(index);
            if (scor4CommandTwo) {
                switch (index) {
                    case 0: case 1: case 2: case 3:
                        return () => Execute("SC " + (index + 1).ToString(CultureInfo.InvariantCulture));
                    case 4: return () => Execute("SC");
                    case 5: return () => Execute("SCX");
                    case 6: return () => Execute("H");
                    case 7: return () => Execute("HS");
                    case 8: return ReturnScor4Bank;
                    case 9: return () => Execute("UNDO");
                    case 10: return () => Execute("DIR *.DRW");
                    case 12: return () => Execute("DIR *.EPS");
                    case 14: return () => Execute("OS");
                    case 15: return () => Execute("OH");
                    case 16: return () => Execute("OT");
                    case 17: return ShowPreferences;
                    case 18: return String.IsNullOrEmpty(current) ? (Action)null : () => StartDiskDelete(current);
                }
                return null;
            }
            if (scor4GroupMenu) {
                if (index == 8) return ReturnScor4Bank;
                if (index == 14) return CenterScor4GroupWholeRests;
                if (index == 18) return () => CenterScor4GroupDashes(0);
                string[] verbs = {"M", "MS", "C", "CS", "A", "DE", "TR", "TAB", null, "LJ",
                    "FLI", "FLU", "FLD", "FLN", null, "ENH", "VJ", "JT", null, "CRV"};
                string verb = verbs[index];
                return verb == null ? (Action)null : () => Execute(verb);
            }
            if (scor4ScreenMenu) {
                switch (index) {
                    case 0: return () => SharedNavigationKey(Keys.End);
                    case 1: return () => ApplyMouseCamera(() => NativeDosViewport.Top(canvas));
                    case 2: return () => PanScor4Screen("ZD");
                    case 3: return () => PanScor4Screen("ZU");
                    case 4: return () => PanScor4Screen("ZL");
                    case 5: return () => PanScor4Screen("ZR");
                    case 6: return () => ZoomScor4Screen(false);
                    case 7: return () => ZoomScor4Screen(true);
                    case 8: return ReturnScor4Bank;
                    case 10: return BeginScor4ScreenDrag;
                    case 12: return () => MoveTextBoundary(1);
                    case 13: return () => MoveTextBoundary(-1);
                    case 14: return () => SetTextBoundary(DefaultTextRows);
                    case 15: return () => Execute("ZF");
                    case 16: return () => Execute("SH");
                    case 17: return () => Execute("SHX");
                    case 18: return () => Execute("ZZ 1");
                }
                return null;
            }
            if (scor4FileInfoMenu) {
                switch (index) {
                    case 0: return () => Execute("PM");
                    case 1: return () => Execute("PMX");
                    case 2: return () => Execute("VER");
                    case 8: return ReturnScor4Bank;
                }
                return null;
            }
            if (scor4FileMenu) {
                switch (index) {
                    case 0: return RefreshScor4Files;
                    case 1: return OpenScor4FromMenu;
                    case 2: return () => SaveCurrent();
                    case 3: return () => Scor4DiskPrompt(2,"SA");
                    case 4: return SavePreviousScor4File;
                    case 5: return () => Execute("SNX");
                    case 6: return ChangeScor4Path;
                    case 7: return () => Scor4DiskPrompt(3,"GM");
                    case 8: return ReturnScor4Bank;
                    case 9: return () => { scor4FileInfoMenu = true; UpdateClassic(); };
                    case 10: return () => Execute("NB");
                    case 11: return () => Execute("NX");
                    case 12: return String.IsNullOrEmpty(scor4PreviousFile) ? (Action)null :
                        () => Execute("GP");
                    case 13: return () => Scor4DiskPrompt(4,"RE");
                    case 14: return () => Execute("RS");
                    case 15: return () => Execute("RSM");
                    case 16: return () => Execute("COM");
                    case 17: return () => Execute("DEC");
                    case 18: return () => Scor4DiskPrompt(8,"DEL");
                    case 19: return () => Execute("MA");
                }
            } else {
                switch (index) {
                    case 0: return () => ShowHelpTopic("SCOR4");
                    case 1: return () => { scor4FileMenu = true; UpdateClassic(); };
                    case 2: return () => StepScor4File(-1);
                    case 3: return () => StepScor4File(1);
                    case 4: return () => Function(2);
                    case 5: return () => Function(6);
                    case 7: return BeginScor4Exit;
                    case 8: return () => { if (score.Items.Count > 0) Execute("I"); };
                    case 9: return () => { scor4CommandTwo = true; UpdateClassic(); };
                    case 10: return BeginOverview;
                    case 11: return () => { scor4ScreenMenu = true; UpdateClassic(); };
                    case 12: return () => { scor4GroupMenu = true; UpdateClassic(); };
                    case 13: return () => Execute("EDI");
                    case 14: return BeginScor4TypeListing;
                    case 15: return () => Execute("CT");
                    case 16: return BeginScor4Paste;
                    case 17: return () => Execute("UND");
                    case 18: return () => SetScor4ItemsBank(1);
                    case 19: return OpenScor4UserMenu;
                }
            }
            return null;
        }

        void SelectScor4EditEndpoint(int endpoint) {
            editParameter = 0;
            editEndpoint = endpoint;
            UpdateClassic();
        }

        void SetScor4SetupBank(bool second) {
            scor4SetupSecond = second;
            UpdateMusicInput();
        }

        void BeginScor4Exit() {
            UpdateDirty();
            if (!dirty) { Close(); return; }
            pendingScor4Exit = true;
            command.Clear();
            Message("Warning: Unsaved entries or changes will be lost.\r\nExit SCORE program?  (No = F1 / Yes = F8)");
            UpdateClassic();
            command.Focus();
        }

        void FinishScor4Exit(bool discard) {
            pendingScor4Exit = false;
            Message("");
            UpdateClassic();
            if (!discard) return;
            confirmedScor4Exit = true;
            try { Close(); }
            finally { confirmedScor4Exit = false; }
        }

        string[] Scor4Files() {
            return Directory.EnumerateFiles(Directory.GetCurrentDirectory())
                .Where(IsScor4DirectoryEntry)
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase).ToArray();
        }

        void Scor4DiskPrompt(int windowsAction,string verb) {
            if(!TryWindowsDiskSelection(windowsAction))StartDiskFilename(verb);
        }

        void ChangeScor4Path() {
            if(TryWindowsDiskSelection(7))return;
            StartDriveChange();
            pendingScor4Path=true;
            Message("Path = "+Directory.GetCurrentDirectory()+"\r\nPress new drive letter or <Enter> ->");
            command.Focus();
        }

        bool ContinueScor4Path() {
            if(!pendingScor4Path)return false;
            pendingScor4Path=false;
            pendingDriveChange=false;
            StartDiskFilename("CD");
            Message("Type new directory name or <Enter> ->");
            return true;
        }

        void OpenScor4FromMenu() {
            if(!NativeDiskDialogChoice.Files){ShowScor4Files();return;}
            string path=ChooseWindowsDiskFile("G");
            if(path!=null)OpenScor4File(path);
        }

        void RefreshScor4Files() {
            string directory = Directory.GetCurrentDirectory();
            string[] paths = Scor4Files();
            string[] rows = paths.Select(Scor4DirectoryRow).ToArray();
            if (rows.All(row => row != null)) {
                var encoding = System.Text.Encoding.GetEncoding(437);
                ScoreFileStorage.Write(Path.Combine(directory, Scor4DirectoryListName),
                    encoding.GetBytes(String.Concat(rows)));
            }
            scor4Directory = directory;
            scor4DirectoryFiles = paths;
            Message(scor4DirectoryFiles.Length + " SCORE files in " + scor4Directory);
        }

        static string Scor4DirectoryRow(string path) {
            string stem = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            if (stem.Length == 0 || stem.Length > 8 || extension.Length > 4 ||
                !Path.GetFileName(path).All(c => c >= '!' && c <= '~'))
                return null;
            string suffix = extension.Length == 0 ? "." : extension;
            return stem.ToUpperInvariant().PadRight(8) + suffix.ToUpperInvariant().PadRight(4) + " \r\n";
        }

        void SavePreviousScor4File() {
            if (String.IsNullOrEmpty(current)) {
                StartDiskFilename("SA");
                return;
            }
            string name = OriginalFilenameSequence.Step(Path.GetFileNameWithoutExtension(current),
                Path.GetExtension(current), false, false);
            StartLetterSave(Path.Combine(Path.GetDirectoryName(current), name));
        }

        static bool IsScor4DirectoryEntry(string path) {
            try {
                long length = new FileInfo(path).Length;
                if (length < 22 || length > 262142 || (length - 2) % 4 != 0) return false;
                using (var stream = File.OpenRead(path))
                using (var reader = new BinaryReader(stream)) {
                    if (reader.ReadUInt16() != (length - 2) / 4) return false;
                    stream.Position = length - 4;
                    if (reader.ReadSingle() != -9999) return false;
                }
                Score.Load(path);
                return true;
            } catch (IOException) { return false; }
              catch (InvalidDataException) { return false; }
              catch (UnauthorizedAccessException) { return false; }
        }

        void StepScor4File(int offset) {
            var paths = ActiveScor4FileList();
            if (paths == null) {
                Message("Create the SCORE file directory with DIRS or File → MkDir.");
                return;
            }
            int currentIndex = Array.FindIndex(paths, path => String.Equals(path, current, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0) {
                Message("No " + (offset < 0 ? "previous" : "next") + " file in the current directory.");
                return;
            }
            int next = (currentIndex + offset + paths.Length) % paths.Length;
            OpenScor4File(paths[next]);
        }

        string[] ActiveScor4FileList() {
            string directory = Directory.GetCurrentDirectory();
            if (scor4DirectoryFiles != null && String.Equals(directory, scor4Directory, StringComparison.OrdinalIgnoreCase))
                return scor4DirectoryFiles;

            string listFile = Path.Combine(directory, Scor4DirectoryListName);
            if (!File.Exists(listFile)) return null;
            var paths = new List<string>();
            foreach (string row in File.ReadLines(listFile, System.Text.Encoding.GetEncoding(437))) {
                int dot = row.IndexOf('.');
                if (dot != 8) continue;
                string name = row.Substring(0, dot).TrimEnd() + row.Substring(dot).TrimEnd().TrimEnd('.');
                if (name.Length == 0 || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) continue;
                paths.Add(Path.Combine(directory, name));
            }
            scor4Directory = directory;
            return scor4DirectoryFiles = paths.ToArray();
        }

        void RestartScor4SameFile() {
            if(String.IsNullOrEmpty(current)||!File.Exists(current)){
                Message("No saved file to reload.");
                return;
            }
            OpenScor4File(current);
        }

        void RestartScor4Score() {
            UpdateDirty();
            if (dirty && score.Items.Count != 0) {
                PromptScor4Load("");
                return;
            }
            ResetDocument();
        }

        void PromptScor4Load(string path) {
            pendingScor4Reload=path;
            command.Clear();
            Message("Warning: Unsaved entries or changes will be lost.\r\nClear or Load new file? (No = F1 / Yes = F8)");
            UpdateClassic();
            command.Focus();
        }

        bool HandleScor4ReloadPrompt(KeyEventArgs key) {
            if(pendingScor4Reload==null)return false;
            key.SuppressKeyPress=true;
            if(key.Modifiers!=Keys.None)return true;
            if(key.KeyCode!=Keys.F1&&key.KeyCode!=Keys.F8&&key.KeyCode!=Keys.Escape)return true;
            FinishScor4Reload(key.KeyCode==Keys.F8);
            return true;
        }

        void FinishScor4Reload(bool discard) {
            string path=pendingScor4Reload;
            pendingScor4Reload=null;
            Message("");
            if(discard)Safe(()=>{
                if(path.Length==0)ResetDocument();
                else OpenScor4File(path,true);
            });
            UpdateClassic();
        }

        void OpenScor4File(string path,bool discardConfirmed=false) {
            UpdateDirty();
            if (!discardConfirmed && dirty && score.Items.Count != 0) { PromptScor4Load(path); return; }
            LoadLetterFile(path,false);
        }

        void ShowScor4Files() {
            string directory = Directory.GetCurrentDirectory();
            string[] paths = ActiveScor4FileList();
            if (paths == null) {
                Message("Create the SCORE file directory with DIRS or File → MkDir.");
                return;
            }
            using (var dialog = new ScoreDialog("scor4-files") {Text = "SCOR4 file directory", Width = 520, Height = 430,
                StartPosition = FormStartPosition.CenterParent}) {
                var list = new ListBox {Dock = DockStyle.Fill};
                bool sortByExtension = false, reverse = false;
                Action fill = () => {
                    var alphabetical = paths.OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
                    IEnumerable<string> ordered = sortByExtension ? paths.OrderBy(Path.GetExtension, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase) : alphabetical;
                    paths = (reverse ? ordered.Reverse() : ordered).ToArray();
                    list.BeginUpdate();
                    try {
                        list.Items.Clear();
                        for (int i = 0; i < paths.Length; i++)
                            list.Items.Add((i + 1) + "  " + Path.GetFileName(paths[i]));
                    } finally { list.EndUpdate(); }
                };
                fill();
                var row = new FlowLayoutPanel {Dock = DockStyle.Bottom, AutoSize = true};
                var nameOrNumber = new TextBox {Width = 145, AccessibleName = "File number or name"};
                var open = new Button {Text = "Open", AutoSize = true};
                var cancel = new Button {Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel};
                int selected = -1;
                Action accept = () => {
                    int value;
                    string query = nameOrNumber.Text.Trim();
                    if (query.Length == 0) selected = list.SelectedIndex;
                    else if (Int32.TryParse(query, out value)) selected = value - 1;
                    else {
                        selected = Array.FindIndex(paths, path =>
                            String.Equals(Path.GetFileName(path), query, StringComparison.OrdinalIgnoreCase));
                        if (selected < 0) {
                            var matches = paths.Where(path => String.Equals(Path.GetFileNameWithoutExtension(path),
                                query, StringComparison.OrdinalIgnoreCase)).Take(2).ToArray();
                            if (matches.Length == 1) selected = Array.IndexOf(paths, matches[0]);
                        }
                    }
                    if (selected < 0 || selected >= paths.Length) return;
                    dialog.DialogResult = DialogResult.OK;
                };
                open.Click += (sender, args) => accept();
                list.DoubleClick += (sender, args) => { nameOrNumber.Clear(); accept(); };
                dialog.KeyPreview = true;
                dialog.KeyDown += (sender, args) => {
                    if (args.Modifiers != Keys.None) return;
                    switch (args.KeyCode) {
                        case Keys.F1: case Keys.F2:
                            if (paths.Length != 0) {
                                int page = Math.Max(1, list.ClientSize.Height / list.ItemHeight);
                                int next = list.TopIndex + (args.KeyCode == Keys.F1 ? page : -page);
                                list.TopIndex = Math.Max(0, Math.Min(paths.Length - 1, next));
                            }
                            break;
                        case Keys.F5: sortByExtension = true; fill(); break;
                        case Keys.F7: reverse = !reverse; fill(); break;
                        case Keys.F9: dialog.DialogResult = DialogResult.Cancel; break;
                        default: return;
                    }
                    args.SuppressKeyPress = true;
                };
                row.Controls.Add(nameOrNumber);
                row.Controls.Add(open);
                row.Controls.Add(cancel);
                dialog.Controls.Add(list);
                dialog.Controls.Add(new Label {Text = directory, Dock = DockStyle.Top, AutoSize = true});
                dialog.Controls.Add(row);
                dialog.AcceptButton = open;
                dialog.CancelButton = cancel;
                if (dialog.ShowDialog(this) == DialogResult.OK) OpenScor4File(paths[selected]);
            }
        }
    }
}
