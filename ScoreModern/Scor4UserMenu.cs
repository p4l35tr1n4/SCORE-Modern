using System;
using System.Globalization;
using System.IO;

namespace ScoreModern {
    public sealed partial class Editor {
        bool scor4AdvanceAfterMacro;

        void OpenScor4UserMenu() {
            scor4UserMenu = true;
            UpdateClassic();
        }

        Action Scor4UserAction(int index) {
            if (index < 8 || index >= 10 && index <= 15) {
                int slot = index < 8 ? index + 1 : index - 1;
                return () => SelectScor4Macro("#" + slot.ToString("00", CultureInfo.InvariantCulture));
            }
            if (index == 8) return ReturnScor4Bank;
            if (index == 9) return () => {
                scor4UserType = true;
                Message("Click on box or type file name.");
                command.Clear();
            };
            return () => {
                ReturnScor4Bank();
                Execute(new string(';', index - 15));
            };
        }

        bool SubmitScor4MacroListing(string name) {
            if (!scor4UserMenu || !scor4UserType || String.IsNullOrWhiteSpace(name)) return false;
            SelectScor4Macro(name.Trim());
            return true;
        }

        void SelectScor4Macro(string name) {
            bool list = scor4UserType;
            ReturnScor4Bank();
            string path = Path.GetFullPath(name);
            if (!File.Exists(path)) {
                Message("File not found - " + name);
                return;
            }
            if (list) StartTypeListing(path);
            else if (mode == ScreenMode.Edit) {
                scor4AdvanceAfterMacro = true;
                commandFileReturnDisk = false;
                try { ReadOriginalCommandFile(path,false); }
                catch { scor4AdvanceAfterMacro = false; throw; }
            } else Execute("RE " + path);
        }

        void FinishScor4EditMacro() {
            if (!scor4AdvanceAfterMacro) return;
            scor4AdvanceAfterMacro = false;
            if (mode == ScreenMode.Edit) Execute("");
        }

        void BeginScor4TypeListing() {
            if (NativeDiskDialogChoice.Files) {
                string name = ChooseCommandFile("Open text file", false, "All files (*.*)|*.*");
                if (name != null) StartTypeListing(name);
            } else StartDiskFilename("TYPE");
        }
    }
}
