using System;

namespace ScoreModern {
    public sealed partial class Editor {
        static readonly string[] Scor4LineMore = {
            "Vert","Horz","Dsh-","Dsh+","Dsh0","Spc-","Spc+","Spc0","<---","--->",
            "LfBr-","LfBr+","LfBr0","RtBr-","RtBr+","RtBr0","Lft<","Rht>","Arp>Dn","Arp>Up"
        };
        static readonly string[] Scor4LineFine = {
            "","","Thick","0Thk","Thn~~","Thn^^","Thk^^","P5=P4","<---","",
            "","","-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","Rotate","0Rota"
        };

        Action Scor4LineAction(int index) {
            switch (index) {
                case 10: return () => EditOriginalParameters(new[]{7f,0f});
                case 11: return () => EditOriginalParameters(new[]{7f,1f});
                case 12: return () => EditOriginalParameters(new[]{7f,-4f});
                case 13: return () => EditOriginalParameters(new[]{7f,-1f});
                case 14: return SetScor4LineArpeggio;
                case 15: return () => SetScor4LineHairpin(false);
                case 16: return () => SetScor4LineHairpin(true);
                case 17: return () => EditOriginalParameters(new[]{1300f,10f});
                case 18: return () => Execute("CN");
                default: return null;
            }
        }

        void SetScor4LineArpeggio() {
            var line = canvas.Selected;
            float top = line.P(5);
            if (top == line.P(4)) top += 8;
            EditOriginalParameters(new[]{5f,top,6f,0f,7f,-1f});
        }

        void SetScor4LineHairpin(bool closing) {
            var line = canvas.Selected;
            float right = line.P(6);
            if (right == 0) right = line.P(3) + 10;
            EditOriginalParameters(new[]{5f,999f,6f,right,7f,closing ? -1f : 0f});
        }

        Action Scor4LineMoreAction(int index) {
            switch (index) {
                case 0: return () => EditOriginalParameters(new[]{13f,270f});
                case 1: return () => EditOriginalParameters(new[]{5f,canvas.Selected.P(4),13f,0f});
                case 2: case 3: case 4:
                    return () => SetScor4LineSpacing(8,index - 2);
                case 5: case 6: case 7:
                    return () => SetScor4LineSpacing(9,index - 5);
                case 9: return () => SetScor4NoteBank(Scor4NoteBank.LineFine);
                case 10: case 11: case 12:
                    return () => SetScor4LineBracket(11,index - 10);
                case 13: case 14: case 15:
                    return () => SetScor4LineBracket(12,index - 13);
                case 16: return () => EditOriginalParameters(new[]{11f,102f});
                case 17: return () => EditOriginalParameters(new[]{12f,102f});
                case 18: case 19:
                    return () => {
                        if (canvas.Selected.P(6) == 0 && canvas.Selected.P(7) == -1)
                            EditOriginalParameters(new[]{12f,index == 18 ? -1f : 1f});
                    };
                default: return null;
            }
        }

        Action Scor4LineFineAction(int index) {
            switch (index) {
                case 2: return () => EditOriginalParameters(new[]{1000f,1f});
                case 3: return () => EditOriginalParameters(new[]{10f,0f});
                case 4: return () => EditOriginalParameters(new[]{5f,canvas.Selected.P(4),7f,-2f});
                case 5: return () => EditOriginalParameters(new[]{5f,canvas.Selected.P(4),7f,-3f});
                case 6: return () => EditOriginalParameters(new[]{5f,canvas.Selected.P(4),7f,-3f,8f,2f,11f,.8f});
                case 7: return () => EditOriginalParameters(new[]{5f,canvas.Selected.P(4)});
                case 8: return () => SetScor4NoteBank(Scor4NoteBank.Item);
                case 12: case 13: case 14: case 15: case 16: case 17:
                    return () => EditScor4LineOffset(index);
                case 18: return () => EditOriginalParameters(new[]{1300f,10f});
                case 19: return () => EditOriginalParameters(new[]{13f,0f});
                default: return null;
            }
        }

        void EditScor4LineOffset(int index) {
            bool left = index < 15;
            int parameter = left ? 17 : 18;
            int otherParameter = left ? 18 : 17;
            int action = (index - 12) % 3;
            var line = canvas.Selected;
            float offset = action == 2 ? 0 : line.P(parameter) + (action == 0 ? -.5f : .5f);
            float active = action == 2 && line.P(otherParameter) == 0 ? 0 : 1;
            float leftMode = left && action == 2 ? 0 : line.P(15);
            EditOriginalParameters(new[]{15f,leftMode,16f,active,(float)parameter,offset});
        }

        void SetScor4LineSpacing(int parameter,int operation) {
            float value = operation == 2 ? 0 : Math.Max(0,canvas.Selected.P(parameter) + (operation == 0 ? -.1f : .1f));
            EditOriginalParameters(new[]{(float)parameter,value});
        }

        void SetScor4LineBracket(int parameter,int operation) {
            float value = operation == 2 ? 0 : canvas.Selected.P(parameter) + (operation == 0 ? -.5f : .5f);
            EditOriginalParameters(new[]{(float)parameter,value});
        }
    }
}
