using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScoreModern {
    public sealed partial class Editor {
        Action PopulateTmbPreferences(TabPage tab, Form dialog) {
            var table = new DataGridView {Dock = DockStyle.Fill, RowHeadersVisible = false,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells};
            table.Columns.Add(new DataGridViewCheckBoxColumn {HeaderText = "Show", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells});
            table.Columns.Add(new DataGridViewTextBoxColumn {HeaderText = "Utility", ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells});
            table.Columns.Add(new DataGridViewTextBoxColumn {HeaderText = "Executable", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill});
            table.Columns.Add(new DataGridViewButtonColumn {Text = "Browse...", UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells});
            table.Columns.Add(new DataGridViewTextBoxColumn {HeaderText = "Support folder", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill});
            table.Columns.Add(new DataGridViewButtonColumn {Text = "Browse...", UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells});
            foreach (DataGridViewColumn column in table.Columns) column.SortMode = DataGridViewColumnSortMode.NotSortable;

            var identity = new TextBox {Dock = DockStyle.Fill, AccessibleName = "TMB producer DAT"};
            var identityBrowse = new Button {Text = "Browse...", AutoSize = true};
            var identityRow = new TableLayoutPanel {Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3};
            identityRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            identityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            identityRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            identityRow.Controls.Add(new Label {Text = "Producer DAT", AutoSize = true, Anchor = AnchorStyles.Left}, 0, 0);
            identityRow.Controls.Add(identity, 1, 0);
            identityRow.Controls.Add(identityBrowse, 2, 0);
            identityBrowse.Click += (sender, args) => {
                using (var picker = new OpenFileDialog {Title = "Choose the producer DAT for TMB utilities",
                    Filter = "SCORE producer DAT (*.dat)|*.dat", FileName = identity.Text, RestoreDirectory = true})
                    if (picker.ShowDialog(dialog) == DialogResult.OK) identity.Text = picker.FileName;
            };

            Action reload = () => {
                identity.Text = NativeSettings.Read(TmbUtility.IdentitySettingKey) ?? "";
                table.CancelEdit();
                table.Rows.Clear();
                foreach (var tool in TmbUtility.Read()) {
                    int row = table.Rows.Add(tool.Enabled, tool.Caption, tool.Executable, null, tool.Support, null);
                    table.Rows[row].Tag = tool.Name;
                    table.Rows[row].Cells[4].ToolTipText = "Leave blank to use the executable's folder.";
                }
            };
            table.CellContentClick += (sender, args) => Safe(() => {
                if (args.RowIndex < 0 || (args.ColumnIndex != 3 && args.ColumnIndex != 5)) return;
                var row = table.Rows[args.RowIndex];
                string executable = Convert.ToString(row.Cells[2].Value);
                string support = Convert.ToString(row.Cells[4].Value);
                if (args.ColumnIndex == 3) {
                    using (var picker = new OpenFileDialog {Title = "Choose " + row.Cells[1].Value,
                        Filter = "Windows executables (*.exe)|*.exe", FileName = executable, RestoreDirectory = true})
                        if (picker.ShowDialog(dialog) == DialogResult.OK) row.Cells[2].Value = picker.FileName;
                } else {
                    if (String.IsNullOrWhiteSpace(support) && File.Exists(executable)) support = Path.GetDirectoryName(executable);
                    using (var picker = new FolderBrowserDialog {Description = "Choose supporting files folder", SelectedPath = support})
                        if (picker.ShowDialog(dialog) == DialogResult.OK) row.Cells[4].Value = picker.SelectedPath;
                }
            });

            var buttons = new FlowLayoutPanel {Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft};
            var close = new Button {Text = "Close", AutoSize = true};
            var apply = new Button {Text = "Apply", AutoSize = true};
            close.Click += (sender, args) => dialog.Close();
            apply.Click += (sender, args) => Safe(() => {
                if (!table.EndEdit()) return;
                var settings = table.Rows.Cast<DataGridViewRow>().Select(row => new TmbUtility {
                    Name = (string)row.Tag, Enabled = Equals(row.Cells[0].Value, true),
                    Executable = Convert.ToString(row.Cells[2].Value).Trim(),
                    Support = Convert.ToString(row.Cells[4].Value).Trim()
                }).ToArray();
                foreach (var tool in settings) {
                    if (tool.Executable.IndexOfAny(new[]{'\t', '\r', '\n'}) >= 0 ||
                        tool.Support.IndexOfAny(new[]{'\t', '\r', '\n'}) >= 0)
                        throw new InvalidDataException("Enter a single executable path and support folder per utility.");
                    if (tool.Enabled) {
                        tool.Validate();
                        if (tool.Support.Length > 0) tool.Support = Path.GetFullPath(tool.Support);
                        tool.Executable = Path.GetFullPath(tool.Executable);
                    }
                }
                string producer = identity.Text.Trim();
                if (producer.Length != 0) {
                    NativeScoreIdentity.Data data;
                    if (!NativeScoreIdentity.TryRead(producer, out data))
                        throw new InvalidDataException("Choose a verified serial-bearing DAT for the TMB producer.");
                    producer = data.Path;
                }
                TmbUtility.Save(settings, producer);
                reload();
                Message("TMB utility preferences applied.");
            });
            buttons.Controls.Add(close);
            buttons.Controls.Add(apply);
            tab.Controls.Add(table);
            tab.Controls.Add(identityRow);
            tab.Controls.Add(new Label {Dock = DockStyle.Top, AutoSize = true,
                Text = "Enable utilities for the context menu. Blank support folders use the executable's folder.\r\n" +
                    "A blank producer DAT uses the active SCORE library's verified DAT."});
            tab.Controls.Add(buttons);
            reload();
            return reload;
        }
    }
}
