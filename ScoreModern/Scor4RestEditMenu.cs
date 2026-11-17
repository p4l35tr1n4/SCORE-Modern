using System;

namespace ScoreModern {
    public sealed partial class Editor {
        static readonly float[] Scor4BinaryDurations = {4,2,1,.5f,.25f,.125f,.0625f};
        static readonly string[] Scor4RestRhythmLabels = {
            "Whole","1/2","1/4","1/8","1/16","1/32","Dot","RPbar","<---","--->",
            "1/64","Tripl","Quint","-SpDt","+SpDt","","","Dotx2","CueSz","NrmSz"
        };
        static readonly string[] Scor4RestNumberedLabels = {
            "WhB1","WhB2","WhB3","WhB4","WhB5","WhB6","WhB7","WhB8","<---","DW",
            "Eur1","Eur2","Eur3","Eur4","Eur5","Eur6","Eur7","Eur8","NoNum",""
        };

        Action Scor4RestPositionAction(int index) {
            switch (index) {
                case 10: return () => SetScor4NoteBank(Scor4NoteBank.RestRhythm);
                case 11: return () => EditOriginalParameters(new[]{400f,-1f});
                case 12: return () => EditOriginalParameters(new[]{400f,1f});
                case 13: return () => EditOriginalParameters(new[]{8f,-1f});
                case 14: return () => EditOriginalParameters(new[]{6f,-1f});
                case 15: return () => EditOriginalParameters(new[]{1000f,-.5f});
                case 16: return () => EditOriginalParameters(new[]{1000f,.5f});
                case 17: return () => EditOriginalParameters(new[]{10f,0f});
                default: return null;
            }
        }

        Action Scor4RestRhythmAction(int index) {
            if (index < 6 || index == 10) return () => SetScor4RestRhythm(index == 10 ? 6 : index);
            if (index == 6 || index == 17) return () => DotScor4Rest(index == 6 ? 1 : 2);
            if (index == 7) return () => EditOriginalParameters(new[]{5f,-4f,6f,0f,8f,0f,9f,-1f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.RestNumbered);
            if (index == 11 || index == 12) return () => SubdivideScor4Duration(index == 11 ? 3 : 5);
            if (index == 13 || index == 14) return () => ChangeScor4RestDotSpacing(index == 13 ? -.05f : .05f);
            if (index == 18 || index == 19) return () => SetScor4ItemCueSize(index == 18);
            return null;
        }

        Action Scor4RestNumberedAction(int index) {
            if (index < 8) return () => SetScor4NumberedRest(index + 1,false);
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.RestRhythm);
            if (index == 9) return () => EditOriginalParameters(new[]{5f,-3f,6f,0f,7f,8f});
            if (index >= 10 && index <= 17) return () => SetScor4NumberedRest(index - 9,true);
            if (index == 18) return () => EditOriginalParameters(new[]{8f,0f});
            return null;
        }

        void SetScor4NumberedRest(int count, bool european) {
            float style = european ? -10 - count : count == 1 ? -2 : -5;
            EditOriginalParameters(new[]{5f,style,6f,0f,8f,(float)count,9f,-1f});
        }

        void SetScor4RestRhythm(int index) {
            EditOriginalParameters(new[]{5f,index - 2f,6f,0f,7f,Scor4BinaryDurations[index],9f,0f});
        }

        void SubdivideScor4Duration(int divisor) {
            float duration = canvas.Selected.P(7);
            if (Array.IndexOf(Scor4BinaryDurations,duration) < 0) return;
            EditOriginalParameters(new[]{7f,(float)((double)duration * (divisor - 1) / divisor)});
        }

        void DotScor4Rest(int dots) {
            int prior = (int)canvas.Selected.P(6);
            if (prior == dots) return;
            if (canvas.Selected.P(6) < 0 || prior >= 2)
                throw new NotSupportedException("Changing the dots on this rest still needs DOS verification.");
            double duration = canvas.Selected.P(7);
            if (prior == 1) duration /= 1.5;
            EditOriginalParameters(new[]{6f,(float)dots,7f,(float)(duration * (dots == 1 ? 1.5 : 1.75))});
        }

        void ChangeScor4RestDotSpacing(float increment) {
            float prior = canvas.Selected.P(6);
            if (prior < 0) return;
            float dots = (int)prior;
            EditOriginalParameters(new[]{6f,Math.Max(dots,prior + increment)});
        }
    }
}
