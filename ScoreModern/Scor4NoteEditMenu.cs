using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ScoreModern {
    public sealed partial class Editor {
        static readonly string[] Scor4NoteRhythmLabels = {
            "Whole","1/2","1/4","1/8","1/16","1/32","Dot","Grace","<---","DW",
            "1/64","Tripl","Quint","-SpDt","+SpDt","DtU/D","Sp1Nt","Dotx2","NoTail","+Tail"
        };
        static readonly string[] Scor4NoteTabLabels = {
            "-StrN","+StrN","0StrN","4Strs","5Strs","6Strs","7Strs","8Strs","<---","--->",
            "Bass","Treble","8va","15ma","Tie","X","-Capo","+Capo","Font>","User>"
        };
        static readonly string[] Scor4NoteManualTabLabels = {
            "Manual Tab","","","4Strs","5Strs","6Strs","7Strs","","<---","",
            "TP0","TP3","TP5","TP7","TP9","-StrNum","+StrNum","-FretNm","+FretNm","( )"
        };

        Action Scor4NoteTabAction(int index) {
            if (index < 3) return () => EditOriginalParameters(index == 2 ? new[]{8f,0f} : new[]{800f,index == 0 ? -1f : 1f});
            if (index < 8) return () => EditOriginalParameters(new[]{6f,-index - 1f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.ManualTab);
            if (index >= 10 && index <= 13) return () => {
                int strings = Math.Abs((int)canvas.Selected.P(6)) % 10;
                if (strings < 4 || strings > 8)
                    throw new NotSupportedException("Choose a verified number of strings before changing the tablature pitch basis.");
                int modifier = new[]{10,0,80,150}[index - 10];
                EditOriginalParameters(new[]{6f,-(strings + modifier)});
            };
            if (index == 14 || index == 15) return () => EditOriginalParameters(new[]{index - 6f,-1f});
            if (index == 16 || index == 17) return () => {
                float capo = canvas.Selected.P(19);
                EditOriginalParameters(new[]{19f,Math.Max(0,capo + (index == 16 ? -1f : 1f))});
            };
            if (index == 18) return () => SetScor4NoteBank(Scor4NoteBank.NumberFonts);
            if (index == 19) return OpenScor4UserMenu;
            return null;
        }

        Action Scor4NoteManualTabAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Tab);
            if (index >= 3 && index <= 6) return () => EditOriginalParameters(new[]{6f,-11f * (index + 1)});
            if (index >= 10 && index <= 14) {
                string[] positions = {"0","3","5","7","9"};
                return () => OriginalTabPosition(positions[index - 10],true);
            }
            if (index >= 15 && index <= 18)
                return () => EditOriginalParameters(new[]{index < 17 ? 800f : 900f,index % 2 == 0 ? 1f : -1f});
            if (index == 19) return () => {
                float font = canvas.Selected.P(12);
                if (font < 0 || font >= 200)
                    throw new NotSupportedException("This tablature font and parentheses combination still needs DOS verification.");
                EditOriginalParameters(new[]{12f,font >= 100 ? font - 100 : font + 100});
            };
            return null;
        }

        Action Scor4NoteRhythmAction(int index) {
            if (index < 6 || index == 10) return () => SetScor4NoteRhythm(index == 10 ? 6 : index);
            if (index == 6 || index == 17) return () => DotScor4Note(index == 6 ? 1 : 2);
            if (index == 7) return SetScor4GraceNote;
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => EditOriginalParameters(new[]{6f,3f,7f,8f});
            if (index == 11 || index == 12) return () => SubdivideScor4Duration(index == 11 ? 3 : 5);
            if (index == 13 || index == 14) return () => AdjustScor4NoteDotSpacing(index == 13 ? -.05f : .05f);
            if (index == 15) return ToggleScor4DotSide;
            if (index == 16) return () => EditOriginalParameters(new[]{9f,(int)canvas.Selected.P(9) % 1000 + 1000f});
            if (index == 18) return () => {
                float control = canvas.Selected.P(9);
                EditOriginalParameters(new[]{9f,control - (int)control % 10});
            };
            if (index == 19) return () => EditOriginalParameters(new[]{900f,1f});
            return null;
        }

        void SetScor4NoteRhythm(int index) {
            var note = canvas.Selected;
            float head = Math.Max(0,2 - index);
            float duration = Scor4BinaryDurations[index];
            float pitch = note.P(4), extension = note.P(8);
            if (note.P(7) == 64 || note.P(7) == 65) {
                if (Math.Abs(pitch) >= 100) pitch -= Math.Sign(pitch) * 100;
                extension = 0;
            }
            EditOriginalParameters(new[]{4f,pitch,6f,head,7f,duration,8f,extension,9f,Math.Max(0,index - 2)});
        }

        void DotScor4Note(int dots) {
            var note = canvas.Selected;
            int control = (int)note.P(9);
            int prior = (control / 10) % 10;
            if (prior == dots) return;
            if (control < 0 || prior > 1)
                throw new NotSupportedException("Changing the dots on this note still needs DOS verification.");
            double duration = note.P(7);
            if (prior == 1) duration /= 1.5;
            EditOriginalParameters(new[]{7f,(float)(duration * (dots == 1 ? 1.5 : 1.75)),9f,dots * 10f + control % 10});
        }

        void AdjustScor4NoteDotSpacing(float increment) {
            float control = canvas.Selected.P(9);
            EditOriginalParameters(new[]{9f,Math.Max((int)control,control + increment)});
        }

        void ToggleScor4DotSide() {
            int control = (int)canvas.Selected.P(9);
            float adjustment = control / 100 % 10 == 0 ? 100 : -100;
            EditOriginalParameters(new[]{900f,adjustment});
        }

        void SetScor4GraceNote() {
            var note = canvas.Selected;
            float pitch = note.P(4);
            int stem = (int)note.P(5) / 10 % 10;
            if (Math.Abs(pitch) >= 100 || stem < 1 || stem > 2)
                throw new NotSupportedException("Grace conversion outside the measured stemmed-note cases still needs DOS verification.");
            EditOriginalParameters(new[]{4f,pitch + (pitch < 0 ? -100 : 100),6f,0f,7f,65f,8f,101f,9f,1f});
        }

        void EditScor4ClefSizeAndOffset(int index) {
            if (index < 13) {
                float size = canvas.Selected.P(6);
                if (size == 0) size = 1;
                float next = index == 12 ? 0 : size + (index == 10 ? -.05f : .05f);
                EditOriginalParameters(new[]{6f,next});
                return;
            }
            float offset = index == 15 ? 0 : canvas.Selected.P(15) + (index == 13 ? -.5f : .5f);
            EditOriginalParameters(new[]{15f,offset});
        }

        Action Scor4NoteMoreAction(int index) {
            switch (index) {
                case 0: return ClearScor4NoteStem;
                case 1: return () => EditOriginalParameters(new[]{800f,-.5f});
                case 2: return () => EditOriginalParameters(new[]{800f,.5f});
                case 3: return NormalizeScor4NoteStem;
                case 4: return () => SetScor4ItemCueSize(true);
                case 5: return () => SetScor4ItemCueSize(false);
                case 6: return () => ChangeScor4NoteHead(-1);
                case 7: return () => ChangeScor4NoteHead(1);
                case 9: return () => Execute("ED");
                case 10: return () => EditOriginalParameters(new[]{12f,2f});
                case 11: return () => EditOriginalParameters(new[]{12f,1f});
                case 12: return () => EditOriginalParameters(new[]{12f,0f});
                case 13: return () => AdjustScor4SelectedNoteStem(false,false);
                case 14: return () => AdjustScor4SelectedNoteStem(false,true);
                case 15: return () => AdjustScor4SelectedNoteStem(true,false);
                case 16: return () => AdjustScor4SelectedNoteStem(true,true);
                case 18: return () => ChangeScor4ChordRhythm("");
                default: return null;
            }
        }

        bool ExecuteScor4ChordRhythm(string verb, string arguments) {
            if (!scor4Interface || verb != "CH" || canvas.Selected == null || canvas.Selected.Code != 1)
                return false;
            ChangeScor4ChordRhythm(arguments);
            return true;
        }

        void ChangeScor4ChordRhythm(string arguments) {
            var note = canvas.Selected;
            float duration = note.P(7);
            if (arguments.Length != 0 && (!Single.TryParse(arguments,NumberStyles.Float,CultureInfo.InvariantCulture,out duration)
                || Single.IsNaN(duration) || Single.IsInfinity(duration)))
                throw new InvalidDataException("CH requires one finite duration.");
            var chord = score.Items.Where(item => item.Code == 1 && item.Staff == note.Staff &&
                item.P(3) == note.P(3)).ToArray();
            string response = "******* no rhythm changes made *******";
            int stem = CenterWord(note.P(5)) % 100 / 10;
            if (chord.Length >= 2 && stem != 0) {
                if (chord.Any(item => item != note && CenterWord(item.P(5)) % 100 / 10 != 0)) {
                    if (stem != 1 && stem != 2)
                        throw new NotSupportedException("CH on multiple stems with this stem control has not been verified.");
                    float pitch = note.P(4);
                    chord = chord.Where(item => stem == 1 ? item.P(4) >= pitch : item.P(4) <= pitch).ToArray();
                }
                bool edited = canvas.EditModified;
                Checkpoint();
                foreach (var member in chord) member.Set(7,duration);
                canvas.EditModified = edited;
                dirty = true;
                RefreshAll(note);
                response = "Chord notes rhythm (P7) changed to " + ExportFixed(duration,3).PadLeft(6);
            }
            pendingEditAcknowledgement = true;
            Message(response);
        }

        void ClearScor4NoteStem() {
            float value = canvas.Selected.P(5);
            if (value < 0 || value >= 130)
                throw new NotSupportedException("NoStm outside ordinary note stem and accidental controls still needs DOS verification.");
            int stem = (int)value / 10 % 10;
            EditOriginalParameters(new[]{5f,value - stem * 10});
        }

        void ChangeScor4DoubleAccidental(int accidental) {
            float packed = canvas.Selected.P(5);
            if (!(packed >= 0 && packed < 200))
                throw new NotSupportedException("The double accidental control is outside the measured packed range.");
            EditOriginalParameters(new[]{5f,packed + (accidental - (int)packed % 10)});
        }

        void ClearScor4Accidental() {
            float packed = canvas.Selected.P(5);
            if (!(packed >= 0 && packed < 200))
                throw new NotSupportedException("NoAcc is outside the measured packed accidental range.");
            int accidental = (int)packed % 10;
            int parentheses = packed >= 100 ? 100 : 0;
            EditOriginalParameters(new[]{5f,packed - (parentheses + accidental)});
        }

        void ToggleScor4AccidentalParentheses() {
            float packed = canvas.Selected.P(5);
            if (!(packed >= 0 && packed < 200))
                throw new NotSupportedException("Accidental parentheses are outside the measured packed range.");
            EditOriginalParameters(new[]{5f,packed >= 100 ? packed - 100f : packed + 100f});
        }

        void ChangeScor4AccidentalSpacing(float increment) {
            float packed = canvas.Selected.P(5);
            if (!(packed >= 0 && packed < 200))
                throw new NotSupportedException("Accidental spacing is outside the measured packed range.");
            int control = (int)packed;
            if (control % 10 == 0) return;
            float next = increment == 0 ? control : (float)Math.Max(control,(double)packed + increment);
            EditOriginalParameters(new[]{5f,next >= control + 1 ? control : next});
        }

        void SetScor4SecondaryMark(float mark) {
            EditOriginalParameters(new[]{11f,mark});
        }

        void AutoSpaceScor4Accidental() {
            float packed = canvas.Selected.P(5);
            if (!(packed >= 0 && packed < 200))
                throw new NotSupportedException("Automatic accidental spacing is outside the measured packed range.");
            int control = (int)packed;
            int accidental = control % 10;
            if (accidental < 1 || accidental > 5)
                throw new NotSupportedException("Automatic spacing for this accidental still needs DOS verification.");
            EditOriginalParameters(new[]{5f,control + (accidental == 2 ? .26f : .22f)});
        }

        void FlipScor4Mark() {
            float mark = canvas.Selected.P(11);
            if (mark != 0) EditOriginalParameters(new[]{11f,-mark});
        }

        void SetScor4ItemCueSize(bool cue) {
            float pitch = canvas.Selected.P(4);
            float magnitude = Math.Abs(pitch);
            if (magnitude >= 200)
                throw new NotSupportedException("Cue size outside ordinary and cue pitches still needs DOS verification.");
            float cueOffset = pitch < 0 ? -100f : 100f;
            if (cue && magnitude < 100) EditOriginalParameters(new[]{400f,cueOffset});
            if (!cue && magnitude >= 100) EditOriginalParameters(new[]{400f,-cueOffset});
        }

        void NormalizeScor4NoteStem() {
            var note = canvas.Selected;
            float value = note.P(5);
            if (value < 0 || value >= 130)
                throw new NotSupportedException("Normal stems outside ordinary stem and accidental controls still need DOS verification.");
            int stem = (int)value / 10 % 10;
            if (stem == 0) value += note.P(4) % 100 >= 7 ? 20 : 10;
            EditOriginalParameters(new[]{5f,value,8f,0f});
        }

        void ChangeScor4NoteHead(int increment) {
            float current = canvas.Selected.P(6);
            if (increment < 0 && (current < 0 || current >= 20))
                throw new NotSupportedException("Cycling backward from an extended notehead still needs DOS verification.");
            float head = current + increment;
            if (head < 0) head = 19;
            if (head >= 20) head = 0;
            EditOriginalParameters(new[]{6f,head});
        }
    }
}
