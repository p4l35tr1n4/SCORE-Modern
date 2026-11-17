using System;

namespace ScoreModern {
    public sealed partial class Editor {
        static readonly string[] Scor4TextDetailsLabels = {
            "-Spc","+Spc","NrmSp","Font>","CTN","CTx","<EdTx",">EdTx","<---","--->",
            "-ChrSp","+ChrSp","NrmChSp","-Parts","+Parts","0Parts","-Rotate","+Rotate","0Rotate","JRx"
        };
        static readonly string[] Scor4TextPointsLabels = {
            "-Pnt","Pnt10","Pnt16","Pnt24","+Pnt","","AbsSz","Abort","<---","",
            "Rip-1","Rip-2","Rip-3","Rip-4","Rip 1","Rip 2","Rip 3","Rip 4","Ripx-1","Ripx1"
        };

        Action Scor4TextPrimaryAction(int index) {
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.TextDetails);
            if (index >= 10 && index <= 12) return () => EditOriginalParameters(index == 12
                ? new[]{11f,0f} : new[]{1100f,index == 10 ? -.5f : .5f});
            if (index >= 13 && index <= 18) return () => {
                var item = canvas.Selected;
                int parameter = index <= 15 ? 6 : 7;
                float size = item.P(parameter);
                if (size == 0 && parameter == 7) size = item.P(6)%100f;
                if (size == 0) size = 1;
                float next = index == 15 || index == 18 ? 0f : size + (index == 13 || index == 16 ? -.1f : .1f);
                EditOriginalParameters(new[]{(float)parameter,next});
            };
            return null;
        }

        Action Scor4TextDetailsAction(int index) {
            if (index < 3) return () => EditOriginalParameters(index == 2
                ? new[]{10f,0f} : new[]{1000f,index == 0 ? -.1f : .1f});
            if (index == 3) return () => SetScor4NoteBank(Scor4NoteBank.NumberFonts);
            if (index == 4) return () => Execute("CTN");
            if (index == 5) return () => Execute("CT");
            if (index == 6 || index == 7) return () => {
                EditText();
                if (inlineTextActive && index == 6) inlineText.Select(InlineTextPrefix(),0);
            };
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.TextPoints);
            if (index >= 10 && index <= 12) return () => EditOriginalParameters(index == 12
                ? new[]{5f,1f} : new[]{500f,index == 10 ? -.01f : .01f});
            if (index >= 13 && index <= 15) return () => {
                double size = Math.Round(canvas.Selected.P(6)%100f,3,MidpointRounding.AwayFromZero);
                EditOriginalParameters(new[]{6f,(float)(size+(index == 13 ? 200f : index == 14 ? 100f : 0f))});
            };
            if (index >= 16 && index <= 18) return () => EditOriginalParameters(index == 18
                ? new[]{9f,0f} : new[]{900f,index == 16 ? -5f : 5f});
            if (index == 19) return () => Execute("JR");
            return null;
        }

        Action Scor4TextPointsAction(int index) {
            if (index == 0 || index == 4) return () => {
                var item = canvas.Selected;
                float scale = canvas.PageDocument.Tables.Scales[item.Staff];
                if (scale == 0) throw new NotSupportedException("Point-size adjustment requires a defined staff size.");
                float size = item.P(6);
                if (size%100 == 0) size += 1f;
                float next = (float)(size + (index == 0 ? -1.84375 : 1.84375)/scale);
                EditOriginalParameters(new[]{6f,next});
            };
            if (index >= 1 && index <= 3)
                return () => Execute("PT "+new[]{10,16,24}[index-1]);
            if (index == 6) return () => Execute("SZ");
            if (index == 7) return AbortCurrent;
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.TextDetails);
            if (index >= 10 && index <= 17)
                return () => Execute("RIP "+(index < 14 ? 9-index : index-13));
            if (index >= 18) return () => Execute(index == 18 ? "RIPX -1" : "RIPX 1");
            return null;
        }
    }
}
