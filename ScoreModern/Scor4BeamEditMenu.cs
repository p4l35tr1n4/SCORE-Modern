using System;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
    public sealed partial class Editor {
        static readonly string[][] Scor4BeamEditLabels = {
            new[]{"Spl1","Spl2","Spl3","Spl4","Mini+-","P5=P4","Lft0","Rht0","<---","--->",
                "1Beam","+Beam","-Num","+Num","Lf-Nt","Lf+Nt","Rt-Nt","Rt+Nt","+-P10","+-P13"},
            new[]{"TRM1","TREM2","TREM3","TREM4","P7-","P7+","P10-","P10+","<---","--->",
                "P11-","P11+","P12-","P12+","P13-","P13+","P14-","P14+","P15-","P15+"},
            new[]{"-.1","+.1","-.02","+.02","-1","+1","-.2","+.2","<---","Flip1",
                "","","","","","","","0 Horz","0 Vert","Flip2"}
        };

        void SetScor4BeamEditBank(int bank) {
            scor4BeamEditBank = bank;
            UpdateClassic();
        }

        void AdjustScor4SelectedBeam(bool french, bool includeOpposite) {
            var beam = canvas.Selected;
            int beamIndex = score.Items.IndexOf(beam);
            var original = score.Items.Select(InputParameters).ToArray();
            var adjusted = BeamStemAdjustment.ApplyScor4Selected(original, beamIndex, french, includeOpposite);
            ApplyScor4SelectedBeamChanges(beam, original, adjusted);
        }

        void ConfigureScor4SelectedBeam(bool includeOpposite) {
            var beam = canvas.Selected;
            var original = score.Items.Select(InputParameters).ToArray();
            var configured = BeamStemAdjustment.ConfigureScor4Selected(original, score.Items.IndexOf(beam), includeOpposite);
            ApplyScor4SelectedBeamChanges(beam, original, configured);
        }

        void ApplyScor4SelectedBeamChanges(Item beam, float[][] original, System.Collections.Generic.IList<float[]> adjusted) {
            var changed = Enumerable.Range(0, original.Length).Where(i =>
                !ReferenceEquals(adjusted[i], original[i])).Select(i => new {
                    Item = score.Items[i], Updated = Item.Make(adjusted[i]).Bytes
                }).Where(change => !change.Item.Bytes.SequenceEqual(change.Updated)).ToArray();
            if (changed.Length == 0) return;
            Checkpoint();
            foreach (var change in changed) change.Item.Bytes = change.Updated;
            dirty = true;
            RefreshAll(beam);
        }

        bool ExecuteScor4SelectedBeamCommand(string verb, string arguments) {
            var item = canvas.Selected;
            if (!scor4Interface || item == null || (item.Code != 1 && item.Code != 6)) return false;
            if (verb != "ADJ" && verb != "ADF" && verb != "ADX" && verb != "ADJX" && verb != "ADFX") return false;
            RequireNoLetterArguments(verb, arguments);
            bool french = verb == "ADF" || verb == "ADFX";
            bool opposite = verb == "ADX" || verb == "ADJX" || verb == "ADFX";
            if (item.Code == 1) AdjustScor4SelectedNoteStem(french, opposite);
            else AdjustScor4SelectedBeam(french, opposite);
            return true;
        }

        void AdjustScor4SelectedNoteStem(bool french, bool includeOpposite) {
            var note = canvas.Selected;
            int direction = CenterWord(note.P(5)) % 100 / 10;
            if (direction < 1 || direction > 2) return;
            float position = note.P(3);
            var beams = score.Items.Where(item => item.Code == 6 && item.Staff == note.Staff &&
                item.P(3) <= position && item.P(6) >= position && item.P(7) > 0 &&
                (includeOpposite || CenterWord(item.P(7)) / 10 == direction)).Take(2).ToArray();
            if (beams.Length == 0) return;
            if (beams.Length != 1)
                throw new NotSupportedException("Overlapping beams for this note still need original verification.");
            var original = score.Items.Select(InputParameters).ToArray();
            int selected = score.Items.IndexOf(note);
            var adjusted = BeamStemAdjustment.ApplyScor4Selected(original, score.Items.IndexOf(beams[0]), french, includeOpposite, selected);
            if (!ReferenceEquals(adjusted[selected], original[selected]))
                EditOriginalParameters(new[]{8f, adjusted[selected][7]});
        }

        Action Scor4BeamEditAction(int index) {
            if (index == 8) return () => SetScor4BeamEditBank(scor4BeamEditBank - 1);
            if (index == 9 && scor4BeamEditBank < Scor4BeamEditLabels.Length)
                return () => SetScor4BeamEditBank(scor4BeamEditBank + 1);
            if (scor4BeamEditBank == 1) {
                if (index >= 0 && index <= 3) return () => SetScor4BeamParameter(8, -index - 1);
                if (index == 4) return () => SetScor4ItemCueSize(Math.Abs(canvas.Selected.P(4)) < 100);
                if (index == 5) return () => SetScor4BeamParameter(5, canvas.Selected.P(4));
                if (index == 6 || index == 7) return () => SetScor4BeamParameter(index + 8, 0);
                if (index == 10) return () => SetScor4BeamForm(1, false);
                if (index == 11) return IncreaseScor4BeamCount;
                if (index == 12 || index == 13)
                    return () => EditOriginalParameters(new[]{800f,index == 12 ? -1f : 1f});
                if (index >= 14 && index <= 17)
                    return () => SetScor4BeamParameter(index < 16 ? 14 : 15, index % 2 == 0 ? 20 : 10);
                if (index == 18 || index == 19)
                    return () => FlipScor4PartialBeam(index == 18 ? 10 : 13);
                return null;
            }
            if (scor4BeamEditBank == 2) {
                if (index >= 0 && index <= 3) return () => SetScor4BeamForm(index + 1, true);
                if (index >= 4 && index <= 7)
                    return () => ChangeScor4BeamParameter(index < 6 ? 7 : 10, index % 2 == 0 ? -1 : 1);
                if (index >= 10)
                    return () => ChangeScor4BeamParameter(11 + (index - 10) / 2, index % 2 == 0 ? -1 : 1);
                return null;
            }
            if (index >= 0 && index <= 7) {
                float[] steps = {-.1f,.1f,-.02f,.02f,-1f,1f,-.2f,.2f};
                return () => {
                    if (canvas.Selected.P(8) > 0)
                        ChangeScor4BeamParameter(index < 4 ? 16 : 19, steps[index]);
                };
            }
            if (index == 17 || index == 18)
                return () => SetScor4BeamParameter(index == 17 ? 16 : 19, 0);
            if (index == 9 || index == 19)
                return () => FlipScor4InnerBeam(index == 9 ? 11 : 14);
            return null;
        }

        void FlipScor4InnerBeam(int parameter) {
            var beam = canvas.Selected;
            float left = beam.P(parameter), right = beam.P(parameter + 1);
            // The original 999 control exchanges a positive endpoint
            // with the opposite zero endpoint of an inner beam stub.
            if (left > 0 && right == 0 || left == 0 && right > 0)
                EditOriginalParameters(new[]{(float)parameter, right, parameter + 1f, left});
        }

        void SetScor4BeamParameter(int parameter, float value) {
            EditOriginalParameters(new[]{(float)parameter, value});
        }

        void FlipScor4PartialBeam(int parameter) {
            // SCOR4 2429:0DB3–0E37 works on signed integer parameter words.
            int beamCount = CenterWord(canvas.Selected.P(7)) % 10;
            int control = CenterWord(canvas.Selected.P(parameter));
            int level = control / 10;
            int next = control < 0
                ? beamCount * 10 - level
                : -11 * (level + control % 10 - beamCount);
            SetScor4BeamParameter(parameter,unchecked((short)next));
        }

        void IncreaseScor4BeamCount() {
            float value = canvas.Selected.P(7);
            if (value < 11 || value >= 29 || value >= 19 && value < 21)
                throw new NotSupportedException("Adding beams outside the ordinary beam-count controls still needs DOS verification.");
            ChangeScor4BeamParameter(7, 1);
        }

        void SetScor4BeamForm(int count, bool tremolo) {
            var beam = canvas.Selected;
            int stem = Math.Abs((int)beam.P(7)) / 10;
            bool extendedTremolo = tremolo && (beam.P(17) != 0 || beam.P(18) != 0 || beam.P(19) != 0);
            if (stem < 1 || stem > 2 || beam.Count > 19 || extendedTremolo)
                throw new NotSupportedException("Changing beam form outside the checked stem and parameter range still needs DOS verification.");
            if (tremolo) {
                EditOriginalParameters(new[]{7f,-stem * 10f,10f,10f + count,
                    11f,0f,12f,0f,13f,0f,14f,0f,15f,0f,16f,0f});
            } else {
                EditOriginalParameters(new[]{7f,stem * 10f + 1f,8f,0f,9f,0f,10f,0f,
                    11f,0f,12f,0f,13f,0f,14f,0f,15f,0f,16f,0f,17f,0f,18f,0f,19f,0f});
            }
        }

        void ChangeScor4BeamParameter(int parameter, float amount) {
            float value = canvas.Selected.P(parameter);
            if(parameter == 16 && value == 0) value = .5f;
            SetScor4BeamParameter(parameter, (float)((double)value + amount));
        }
    }
}
