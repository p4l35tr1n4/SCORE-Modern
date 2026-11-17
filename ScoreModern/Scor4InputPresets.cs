using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
    public sealed partial class Editor {
        List<Item> pendingScor4Preset;
        readonly int[] scor4PresetClefs = new int[33];
        const string Scor4PresetWarning = "Warning: This action will cause existing staves to be overwritten.\r\n"
            + "Press ESC to abort, any other key to continue";

        void LoadScor4Preset(int number) {
            string name = "S" + number.ToString("00", System.Globalization.CultureInfo.InvariantCulture) + ".SET";
            string path = NativeLibraryLocation.FileFor(source, name);
            if (!File.Exists(path))
                throw new FileNotFoundException("The SCOR4 preset " + name + " is missing. Select the SCORE 4 LIB for all resources.", path);

            bool defaultGeometry = inputSetup.Staff == 1 && inputSetup.LeftEnd == 0 && inputSetup.Vertical == 0 &&
                inputSetup.Size == 1 && inputSetup.RightEnd == 200;

            var preset = Score.Load(path);
            var items = defaultGeometry ? preset.Items : PlaceInputPreset(preset);
            preset.Items = items;
            if (!defaultGeometry && items.Any(item => item.Code == 16)) {
                var refreshed = NativeTextCache.Refresh(preset, canvas.TextLayout);
                textCacheIssues = String.Join(Environment.NewLine, refreshed.Unresolved);
            }
            var staffs = new HashSet<int>(items.Where(item => item.Code == 8).Select(item => item.Staff));
            if (inputSetup.Key != "C") {
                var lastPresetItem = items.LastOrDefault();
                AppendScor4PresetKey(items, staffs);
                NativeTextTerminator.Refresh(preset, lastPresetItem);
            }
            if (score.Items.Any(item => item.Code == 8 && staffs.Contains(item.Staff))) {
                pendingScor4Preset = items;
                command.Clear();
                UpdateMusicInput();
                return;
            }
            CommitInputPreset(items);
        }

        void AppendScor4PresetKey(List<Item> items, IEnumerable<int> staffs) {
            int key = InputPitchDefaults().KeySignature;
            var clefs = items.Where(item => item.Code == 3).ToLookup(item => item.Staff);
            if (items.Any(item => item.Code == 17))
                throw new NotSupportedException("Replacing a key already in a SCOR4 preset needs original verification.");
            if (key < -7 || key > 7)
                throw new InvalidDataException("The preset key must be one of SCORE's fifteen ordinary signatures.");
            var rows = items.Select(InputParameters).ToArray();
            foreach (int staff in staffs.OrderBy(value => value)) {
                var clef = clefs[staff].ToArray();
                if (staff < 1 || staff > 32 || clef.Length > 1 || clef.Any(item => !IsOrdinaryScor4PresetClef(item)))
                    throw new NotSupportedException("This preset clef configuration still needs original verification.");
                var bounds = InputSetupBounds.Find(rows, new InputSetupState {
                    Staff = staff, Size = inputSetup.Size, LeftEnd = inputSetup.LeftEnd
                });
                float position = (float)((double)bounds.Left - 2.5 * inputSetup.Size);
                // A staff without a clef uses its own prior preset value.
                // DOS retained that value across both RS and an intervening file load.
                int clefCode = clef.Length == 0 ? scor4PresetClefs[staff] : bounds.Clef;
                items.Add(Item.Make(17, staff, position, 0, key, clefCode));
            }
        }

        static bool IsOrdinaryScor4PresetClef(Item item) {
            float code = item.P(5);
            return code >= 0 && code <= 4 && code == Math.Truncate(code);
        }

        void RememberScor4PresetClefs(IEnumerable<Item> items) {
            foreach (var item in items.Where(item => item.Code == 3 && item.Staff >= 1 && item.Staff <= 32 && IsOrdinaryScor4PresetClef(item)))
                scor4PresetClefs[item.Staff] = (int)item.P(5);
        }

        bool HandleScor4PresetConfirmation(KeyEventArgs key) {
            if (pendingScor4Preset == null) return false;
            key.SuppressKeyPress = true;
            if (key.KeyCode == Keys.ControlKey || key.KeyCode == Keys.ShiftKey || key.KeyCode == Keys.Menu)
                return true;
            var items = pendingScor4Preset;
            pendingScor4Preset = null;
            Safe(() => {
                if (key.KeyCode == Keys.Escape) UpdateMusicInput();
                else CommitInputPreset(items);
            });
            return true;
        }
    }
}
