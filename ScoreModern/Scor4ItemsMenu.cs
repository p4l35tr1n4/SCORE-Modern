using System;
using System.Linq;

namespace ScoreModern {
    public sealed partial class Editor {
        bool scor4ExternalItemInsertion;
        static readonly string[][] Scor4ItemsLabels = {
            new[]{"Note", "Rest", "Clef", "Line", "Slur", "Beam", "Bar", "Staff", "<---", "--->",
                "Code9", "Number", "Cres<", "Decr>", "KeySig", "Meter", "End1", "End2", "8va/15", "Text"},
            new[]{"Harp", "PedA", "PedB", "PedC", "Arpeg", "Box", "Circl", "Guitr", "<---", "",
                "TREM1", "TREM2", "TREM3", "TREM4", "Trill", "Accord", "Bends", "Code11", "Code15", ""}
        };
        static readonly string[] Scor4BarEditLabels = {
            "-Staff", "+Staff", "1Staff", "NormBar", "DblBar", "EndBar", "RptEnd", "RptBeg", "RptBoth", "User>"
        };
        static readonly string[][] Scor4MoreBarLabels = {
            new[]{"-Offs", "+Offs", "NoOff", "Invis", "HeavyRpt", "Brace", "Brakt", "SubBk", "<---", "--->",
                "TopBrk", "BotBrk", "MidBrk", "+BarSpc", "0BarSpc", "+DotSpc", "0DotSpc", "+P17", "0P17", "Dash"},
            new[]{"-LowDt", "+LowDt", "-HiDot", "+HiDot", "0Dots", "-Thick", "+Thick", "NrmThk", "<---", "",
                "-PrtBot", "+PrtBot", "0PrtBot", "-PrtTop", "+PrtTop", "0PrtTop", "+DshSiz", "0DshSiz", "+DshSpc", "0DhSp"}
        };
        static readonly string[] Scor4NumberEditLabels = {
            "NoBox/C", "Box", "Circle", "Font>", "-10", "+10", "-1", "+1", "", "User>"
        };
        static readonly string[] Scor4NumberDetailsLabels = {
            "", "", "", "", "", "", "JstOn", "JsOff", "<---", "",
            "-OffSt", "+OffSt", "NoOff", "-Size", "+Size", "Size 1", "+Spread", "NoSpred", "Num.1", "Ltr A"
        };
        static readonly string[][] Scor4NumberFontsLabels = {
            new[]{"TmsRm","TmsBd","TmsIt","TBdIt","Helv","HlvBd","HlvOb","HBdOb","<---","--->",
                "HelvNrw","HelvNBd","HelvNOb","HlvNBOb","PaltnRm","PaltnBd","PaltnIt","PalBdIt","",""},
            new[]{"AvGBk","AvBOb","AvDem","AvDOb","BkmLt","BkLIt","BkDem","BkDIt","<---","",
                "NwCnSRm","NwCnSBd","NwCnSIt","NwCBdIt","ZapfChn","Courier","CurBld","CurObl","CurBlOb","Music"}
        };
        static readonly string[] Scor4MeterFontsLabels = Scor4NumberFontsLabels[0]
            .Take(18).Concat(new[]{"OutLine","P8=0"}).ToArray();
        static readonly string[] Scor4StaffEditLabels = {
            "-Line", "+Line", "5-Line", "Siz-.05", "Siz+.05", "Siz=1", "BtmLin#", "", "0/200", "User>"
        };
        static readonly string[] Scor4LibrarySymbolEditLabels = {
            "-OffSt", "+OffSt", "NoOff", "-HorzSz", "+HorzSz", "P6=1", "-VertSz", "+VertSz", "P7=0", "User>"
        };
        static readonly string[][] Scor4SymbolEditLabels = {
            new[]{"-Rot","+Rota","0Rota","","P5-1","P5+1","P5-10","P5+10","<---","--->",
                "","","","","","","","","",""},
            new[]{"pppp","ppp","pp","p","mp","mf","f","ff","<---","--->",
                "fff","ffff","fp","sf","sfz","sff","sffz","sfff","-P5","+P5"},
            new[]{"fz","rf","rfz","sfpp","sfp","sffp","mfpp","mfp","<---","--->",
                "fpp","ffp","sfffz","sffffz","sfzp","sffzp","sfffzp","8 arrow","cresc","decrs"},
            new[]{"pizz.","arco","dim.","accel","rit.","rall.","a tmp","espr.","<---","--->",
                "perc.","early","poco","molto","meno","piu","mosso","subto","gliss","SCORE"},
            new[]{"ped","*","p(ed)","turn","segno","frmta","frmt□","frmt^","<---","",
                "(>)","orn.1","orn.2","diamx","VrtBk","(b)","micro1","micro2","-P5","+P5"}
        };
        static readonly float[][] Scor4SymbolValues = {
            new[]{50f,51f,52f,53f,54f,55f,56f,57f,0f,0f,58f,59f,60f,61f,62f,160f,129f,161f},
            new[]{123f,127f,128f,162f,163f,164f,165f,166f,0f,0f,167f,169f,280f,281f,282f,283f,284f,69f,63f,111f},
            new[]{66f,67f,64f,68f,65f,113f,78f,79f,0f,0f,600f,1000f,86f,73f,80f,81f,114f,85f,72f,199f},
            new[]{70f,71f,120f,75f,74f,31f,213f,214f,0f,0f,92f,93f,94f,95f,96f,97f,145f,182f}
        };
        static readonly string[] Scor4OctaveEditLabels = {
            "8va", "8", "8ba", "-88", "15ma", "15", "RtBrk", "", "Centr", "User>"
        };
        static readonly string[] Scor4OctaveOffsetLabels = {
            "-LfOff", "+LfOff", "0LfOff", "-RtOff", "+RtOff", "0RtOff", "", "", "<---", "",
            "", "", "", "", "", "", "", "", "", ""
        };
        static readonly string[] Scor4HarpEditLabels = {
            "D Ped", "C Ped", "B Ped", "E Ped", "F Ped", "G Ped", "A Ped", "", "Centr", "User>"
        };
        static readonly string[] Scor4GraphicPedalEditLabels = {
            "LfPed", "MidPed", "RtPed", "-LfLin", "+LfLin", "-RtLin", "+RtLin", "-^-Peds", "Centr", "User>"
        };
        static readonly string[] Scor4PedalLineEditLabels = {
            "-^v-", "-^-", "-Point", "+Point", "0Point", "LfBrk", "RtBrk", "GrphPed", "Centr", "User>"
        };
        static readonly string[] Scor4TrillEditLabels = {
            "-LfOff", "+LfOff", "0LfOff", "-RtOff", "+RtOff", "0RtOff", "Wgl Y/N", "Acci", "Centr", "User>"
        };
        static readonly string[] Scor4ShapeEditLabels = {
            "-Thick", "+Thick", "NoThik", "Box/Cir", "-XSize", "+XSize", "-YSize", "+YSize", "P7=0", "P7=P6"
        };
        static readonly string[] Scor4GuitarEditLabels = {
            "Dots", "ChrdNam", "Barre", "FingNum", "P5=-1", "P5=-2", "P5=-3", "-OffSet", "+OffSet", "NoOff"
        };
        static readonly string[] Scor4AccordionEditLabels = {
            "-Thick", "+Thick", "NoThk", "-Size", "+Size", "", "-OffSt", "+OffSt", "NoOff", "User>"
        };
        static readonly string[] Scor4AccordionDetailsLabels = {
            "1-Lin","2-Lin","3-Lin","","","","","","<---","",
            "TopDot","Dot2","Dot3","Dot4","","LeftDot","RghtDot","","",""
        };
        static readonly string[] Scor4BendEditLabels = {
            "-HorzSz", "+HorzSz", "P6=1", "-VertSz", "+VertSz", "P7=1", "-Flatn", "+Flatn", "P11=0", "P7=P6"
        };
        static readonly string[] Scor4ExternalDrawingEditLabels = {
            "-HorzSz", "+HorzSz", "P6=1", "-VertSz", "+VertSz", "P7=0", "-Rotate", "+Rotate", "0Rotate", "User>"
        };
        static readonly string[] Scor4ExternalPostScriptEditLabels = {
            "-HorzSz", "+HorzSz", "P5=1", "-VertSz", "+VertSz", "P6=0", "-Rotate", "+Rotate", "0Rotate", "User>"
        };
        static readonly string[] Scor4ExternalNamesLabels = {
            "", "Dir", "NewNm", "", "", "", "", "", "<---", "",
            "", "", "", "", "", "", "", "", "", ""
        };
        static readonly string[] Scor4KeyEditLabels = {
            "-OffSt", "+OffSt", "NoOff", "1 Flat", "+Flat", "1 Sharp", "+Sharp", "Natural", "0 Natrl", "User>"
        };
        static readonly string[] Scor4MeterEditLabels = {
            "-OffSt", "+OffSt", "NoOff", "2/4", "3/4", "4/4", "5/4", "Cut", "C", "User>"
        };
        static readonly string[] Scor4KeyDetailsLabels = {
            "Treb","Bass","Alto","Tenor","CueSz","-Spc","+Spc","0Spc","<---","0Acci",
            "Acci1","Acci2","Acci3","Acci4","Acci5","Acci6","AcciDn","AcciUp","JustOff","JstOn"
        };
        static readonly string[] Scor4MeterDetailsLabels = {
            "3/8","5/8","6/8","9/8","12/8","-Size","+Size","NrmSz","<---","--->",
            "1 Alone","+Num","3/16","6/16","9/16","-TopNum","+TopNum","Font>","JustOff","JstOn"
        };
        static readonly string[] Scor4DoubleMeterLabels = {
            "-TopLf","+TopLf","","0 Dbl","","-RtTop","+RtTop","","<---","",
            "BotLf 2","BotLf 4","BotLf 8","BotLf16","+on/off","RtBot 2","RtBot 4","RtBot 8","RtBot16","RtBt0"
        };
        int scor4KeyAccidentalParameter;

        // SCORE 4.01 saved-record capture ZITEM4.MUS, ordinary Items bank.
        static readonly float[][] Scor4ItemDefaults = {
            new[]{1f, 0f, 100f, 1f, 10f, 0f, 1f},
            new[]{2f, 0f, 100f, 0f, 0f, 0f, 1f},
            new[]{3f, 0f, 100f},
            new[]{4f, 0f, 100f, 0f, 0f, 120f},
            new[]{5f, 0f, 100f, 14f, 14f, 120f, 3f, -1f},
            new[]{6f, 0f, 100f, 8f, 8f, 120f, 11f},
            new[]{14f, 0f, 100f, 1f}
        };
        static readonly float[][] Scor4AltItemDefaults = {
            new[]{9f, 0f, 100f, -2f, 54f},
            new[]{10f, 0f, 100f, 13f, 1f},
            new[]{4f, 0f, 100f, -1f, 999f, 120f},
            new[]{4f, 0f, 100f, -1f, 999f, 120f, -1f},
            new[]{17f, 0f, 100f, 0f, 2f},
            new[]{18f, 0f, 100f, 0f, 4f, 4f},
            new[]{5f, 0f, 100f, 15f, 15f, 120f, 5f, 1f},
            new[]{5f, 0f, 100f, 15f, 15f, 120f, 5f, 2f},
            new[]{7f, 0f, 100f, 15f, 0f, 120f, 8f, 0f, 0f, 0f, 0f, -1f}
        };
        static readonly float[][] Scor4SecondItemDefaults = {
            new[]{7f, 0f, 100f, -1f, 1f, 0f, 1f, 111f, 1111f},
            new[]{7f, 0f, 100f, -1f, 0f, 140f, 20f},
            new[]{7f, 0f, 100f, -1f, 0f, 140f, 21f},
            new[]{7f, 0f, 100f, -1f, 0f, 140f, 22f},
            new[]{4f, 0f, 100f, 0f, 12f, 0f, -1f},
            new[]{12f, 0f, 100f, 15f, 0f, 10f, 10f},
            new[]{12f, 0f, 100f, 15f, 1f, 10f, 10f},
            new[]{12f, 0f, 100f, 16f, -4f, 1f, 10f},
            null, null,
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 11f},
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 12f},
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 13f},
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 14f},
            new[]{7f, 0f, 100f, 15f, 0f, 120f},
            new[]{12f, 0f, 100f, 16f, 10f, 0f, 3f},
            new[]{12f, 0f, 100f, 16f, 2f, 2f, 2f},
            null, null, null
        };

        static string[] Scor4EditLabelsForItem(Item item) {
            int code = item.Code;
            if (code >= 1 && code <= Scor4EditLower.Length) return Scor4EditLower[code - 1];
            switch (code) {
                case 7:
                    if (IsScor4OctaveMark(item)) return Scor4OctaveEditLabels;
                    if (IsScor4HarpDiagram(item)) return Scor4HarpEditLabels;
                    if (item.P(7) == 20) return Scor4GraphicPedalEditLabels;
                    if (item.P(7) == 21 || item.P(7) == 22) return Scor4PedalLineEditLabels;
                    return item.P(7) == 0 ? Scor4TrillEditLabels : null;
                case 8: return Scor4StaffEditLabels;
                case 9: return Scor4LibrarySymbolEditLabels;
                case 10: return Scor4NumberEditLabels;
                case 11: return Scor4ExternalDrawingEditLabels;
                case 12:
                    if (IsScor4GuitarGrid(item)) return Scor4GuitarEditLabels;
                    if (item.P(5) == 10) return Scor4AccordionEditLabels;
                    if (item.P(5) == 2) return Scor4BendEditLabels;
                    return item.P(5) == 0 || item.P(5) == 1 ? Scor4ShapeEditLabels : null;
                case 14: return Scor4BarEditLabels;
                case 15: return Scor4ExternalPostScriptEditLabels;
                case 16: return Scor4LibrarySymbolEditLabels;
                case 17: return Scor4KeyEditLabels;
                case 18: return Scor4MeterEditLabels;
                default: return null;
            }
        }

        Action Scor4StaffEditAction(int index) {
            if (index <= 12) return () => AdjustScor4StaffLines(index);
            if (index <= 15) return () => {
                float size = canvas.Selected.P(5);
                if (size == 0) size = 1;
                float next = index == 15 ? 0 : size + (index == 13 ? -.05f : .05f);
                EditOriginalParameters(new[]{5f,next});
            };
            if (index == 16) return () => {
                float packed = canvas.Selected.P(7);
                if (packed != Math.Truncate(packed) || packed < 0 || packed >= 500)
                    throw new NotSupportedException("Cycling the staff bottom line from this setting still needs DOS verification.");
                int origin = (int)packed / 100;
                int lines = (int)packed % 100;
                int newOrigin = origin == 0 ? 1 : lines == 0 ? origin : origin + 1;
                EditOriginalParameters(new[]{7f,newOrigin * 100 + Math.Max(lines,1)});
            };
            if (index == 18) return () => EditOriginalParameters(new[]{3f,0f,6f,0f});
            return null;
        }

        Action Scor4ExternalDrawingEditAction(int index) {
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.ExternalNames);
            if (index < 10 || index > 18) return null;
            return () => {
                var item = canvas.Selected;
                bool postScript = item.Code == 15;
                int control = index - 10;
                if (control >= 6) {
                    int rotation = postScript ? 12 : 9;
                    EditOriginalParameters(control == 8
                        ? new[]{(float)rotation,0f}
                        : new[]{rotation * 100f,control == 6 ? -5f : 5f});
                    return;
                }
                int horizontal = postScript ? 5 : 6;
                int parameter = horizontal + control / 3;
                float size = item.P(parameter);
                if (size == 0 && control >= 3) size = item.P(horizontal);
                if (size == 0) size = 1;
                float next = control % 3 == 2 ? (postScript && control == 2 ? 1f : 0f)
                    : size + (control % 3 == 0 ? -.1f : .1f);
                EditOriginalParameters(new[]{(float)parameter,next});
            };
        }

        Action Scor4ExternalNamesAction(int index) {
            bool postScript = canvas.Selected.Code == 15;
            if (index == 1) return () => ShowScoreDirectory(postScript ? "*.EPS" : "*.DRW");
            if (index == 2) return () => EditOriginalParameters(new[]{13f,postScript ? -1f : 1f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            return null;
        }

        static bool IsScor4HarpDiagram(Item item) {
            return item.P(7) == 1 && item.P(8) >= 111 && item.P(8) <= 333 &&
                item.P(9) >= 1111 && item.P(9) <= 3333;
        }

        static readonly string[] Scor4HarpDetailsLabels = {
            "Size 1","-Size","+Size","-Offset","+Offset","0Offset","","","<---","",
            "","","","","","","","","",""
        };
        static readonly string[] Scor4PedalDetailsLabels = {
            "-Siz","+Siz","","","LfBrk","RtBrk","","","<---","",
            "-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","-PdOff","+PdOff","0PdOff",""
        };

        Action Scor4HarpDetailsAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 0) return () => EditOriginalParameters(new[]{5f,1f});
            if (index == 1 || index == 2)
                return () => EditOriginalParameters(new[]{500f,index == 1 ? -.1f : .1f});
            if (index == 3 || index == 4)
                return () => EditOriginalParameters(new[]{1500f,index == 3 ? -.5f : .5f});
            if (index == 5) return () => EditOriginalParameters(new[]{15f,0f});
            return null;
        }

        void CycleScor4PedalBracket(int parameter) {
            float[] cycle = {0,2,102,-2};
            int position = Array.IndexOf(cycle,canvas.Selected.P(parameter));
            if (position < 0)
                throw new NotSupportedException("Cycling an existing pedal bracket still needs SCORE 4 verification.");
            EditOriginalParameters(new[]{(float)parameter,cycle[(position+1)%cycle.Length]});
        }

        Action Scor4PedalDetailsAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 0 || index == 1)
                return () => EditOriginalParameters(new[]{900f,index == 0 ? -.1f : .1f});
            if (index == 4 || index == 5) return () => CycleScor4PedalBracket(index+7);
            if (index >= 10 && index <= 15) {
                int parameter = index < 13 ? 17 : 18, operation = (index-10)%3;
                return () => {
                    if (operation != 2) {
                        EditOriginalParameters(new[]{16f,1f,parameter*100f,operation == 0 ? -.5f : .5f});
                        return;
                    }
                    int otherEndpoint = parameter == 17 ? 18 : 17;
                    EditOriginalParameters(canvas.Selected.P(otherEndpoint) == 0
                        ? new[]{(float)parameter,0f,16f,0f}
                        : new[]{(float)parameter,0f});
                };
            }
            if (index >= 16 && index <= 18)
                return () => EditOriginalParameters(index == 18
                    ? new[]{15f,0f} : new[]{1500f,index == 16 ? -.5f : .5f});
            return null;
        }

        Action Scor4HarpDiagramAction(int index) {
            if (index == 18) return () => Execute("CN");
            if (index < 10 || index > 16) return null;
            int pedal = index - 10;
            int parameter = pedal < 3 ? 8 : 9;
            int place = new[]{100,10,1,1000,100,10,1}[pedal];
            return () => {
                float control = canvas.Selected.P(parameter);
                int value = (int)control / place % 10;
                if (value < 1 || value > 3)
                    throw new NotSupportedException("This harp pedal position still needs SCORE 4 verification.");
                EditOriginalParameters(new[]{parameter * 100f,value == 3 ? -2f * place : place});
            };
        }

        Action Scor4GraphicPedalAction(int index) {
            if (index == 18) return () => Execute("CN");
            if (index >= 10 && index <= 12) return () => {
                int place = new[]{100,10,1}[index - 10];
                int digit = (int)canvas.Selected.P(5) / place % 10;
                EditOriginalParameters(new[]{500f,digit == 0 ? (float)place : -place});
            };
            if (index == 13 || index == 14) return () => {
                float position = canvas.Selected.P(8);
                if (position == 0) position = canvas.Selected.P(3) - 1.5f;
                EditOriginalParameters(new[]{8f,position + (index == 13 ? -.5f : .5f)});
            };
            if (index == 15 || index == 16)
                return () => EditOriginalParameters(new[]{600f,index == 15 ? -.5f : .5f});
            if (index == 17) return () => EditOriginalParameters(new[]{5f,0f,7f,21f,8f,0f});
            return null;
        }

        Action Scor4PedalLineAction(int index) {
            if (index == 18) return () => Execute("CN");
            if (index == 10 || index == 11) return () => {
                float point = canvas.Selected.P(8);
                if (point == 0) point = canvas.Selected.P(3) + 20;
                EditOriginalParameters(new[]{7f,index == 10 ? 21f : 22f,8f,point});
            };
            if (index == 12 || index == 13) return () => EditOriginalParameters(new[]{800f,index == 12 ? -.5f : .5f});
            if (index == 14) return () => EditOriginalParameters(new[]{8f,0f});
            if (index == 15 || index == 16) return () => CycleScor4PedalBracket(index-4);
            if (index == 17) return () => EditOriginalParameters(new[]{5f,0f,7f,20f,8f,0f,11f,0f,12f,0f});
            return null;
        }

        Action Scor4ShapeAction(int index) {
            if (index < 10 || index > 19) return null;
            if (index == 10) return () => {
                float thickness = canvas.Selected.P(8);
                if (thickness > 0) EditOriginalParameters(new[]{800f,-1f});
            };
            if (index == 11) return () => EditOriginalParameters(new[]{800f,1f});
            if (index == 12) return () => EditOriginalParameters(new[]{8f,0f});
            if (index == 13) return () => EditOriginalParameters(new[]{5f,1f - canvas.Selected.P(5)});
            if (index == 18) return () => EditOriginalParameters(new[]{7f,0f});
            if (index == 19) return () => EditOriginalParameters(new[]{7f,canvas.Selected.P(6)});
            int parameter = index < 16 ? 6 : 7;
            return () => EditOriginalParameters(new[]{parameter * 100f,index % 2 == 0 ? -.2f : .2f});
        }

        static readonly string[] Scor4CircleDetailsLabels = {
            "Arc-A","Arc+A","Arc-B","Arc+B","NoArc","","","","<---","--->",
            "Circle","Triangl","Pentagn","Hexagon","Octagon","","VrtOval","HrzOval","","User>"
        };
        static readonly string[] Scor4ShapeOffsetLabels = {
            "-Offs","+Offs","NoOff","","-Rotat","+Rotat","NoRot","","<---","",
            "","","","","","","","","",""
        };
        static readonly string[] Scor4BendDetailLabels = Scor4ShapeOffsetLabels.Take(10)
            .Concat(new[]{"","NoArrw","NormArw","-Arrow","+Arrow","NormLng","+ArwLng","-Thick","+Thick","NoThk"}).ToArray();

        Action Scor4CircleDetailsAction(int index) {
            if (index >= 0 && index <= 3) return () => {
                int parameter = index < 2 ? 10 : 11;
                float angle = canvas.Selected.P(parameter) + (index % 2 == 0 ? -5 : 5);
                if (angle < 0) angle += 360;
                if (angle > 360) angle -= 360;
                EditOriginalParameters(new[]{(float)parameter,angle});
            };
            if (index == 4) return () => EditOriginalParameters(new[]{10f,0f,11f,0f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.ShapeOffsets);
            if (index >= 10 && index <= 14) {
                float rotation = new[]{0f,120f,72f,60f,45f}[index-10];
                return () => EditOriginalParameters(new[]{12f,rotation});
            }
            if (index >= 16 && index <= 17) return () => {
                float width = canvas.Selected.P(6), height = canvas.Selected.P(7);
                if (index == 16) EditOriginalParameters(new[]{7f,1.5f*width});
                else if (height == 0) EditOriginalParameters(new[]{7f,width,6f,1.5f*width});
                else EditOriginalParameters(new[]{6f,1.5f*height});
            };
            if (index == 19) return OpenScor4UserMenu;
            return null;
        }

        Action Scor4ShapeOffsetAction(int index) {
            if (index >= 0 && index <= 2) return () => EditOriginalParameters(index == 2
                ? new[]{15f,0f} : new[]{1500f,index == 0 ? -.5f : .5f});
            if (index >= 4 && index <= 6) return () => EditOriginalParameters(index == 6
                ? new[]{9f,0f} : new[]{900f,index == 4 ? -5f : 5f});
            if (index == 8) return () => SetScor4NoteBank(canvas.Selected.P(5) == 1
                ? Scor4NoteBank.CircleDetails : Scor4NoteBank.Item);
            return null;
        }

        Action Scor4BendDetailAction(int index) {
            switch (index) {
                case 11: return () => EditOriginalParameters(new[]{12f,-1f});
                case 12: return () => EditOriginalParameters(new[]{12f,0f});
                case 13: return () => EditOriginalParameters(new[]{1200f,-.2f});
                case 14: return () => EditOriginalParameters(new[]{1200f,.2f});
                case 15: return () => EditOriginalParameters(new[]{13f,0f});
                case 16: return () => EditOriginalParameters(new[]{1300f,.2f});
                case 17: return () => {
                    if (canvas.Selected.P(8) > 0) EditOriginalParameters(new[]{800f,-1f});
                };
                case 18: return () => EditOriginalParameters(new[]{800f,1f});
                case 19: return () => EditOriginalParameters(new[]{8f,0f});
                default: return null;
            }
        }

        Action Scor4AccordionAction(int index) {
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.AccordionDetails);
            if (index == 10) return () => {
                if (canvas.Selected.P(10) > 0) EditOriginalParameters(new[]{1000f,-1f});
            };
            if (index == 11 || index == 12)
                return () => EditOriginalParameters(index == 11 ? new[]{1000f,1f} : new[]{10f,0f});
            if (index == 13 || index == 14) return () => {
                float size = canvas.Selected.P(6);
                if (size == 0) size = 1f;
                EditOriginalParameters(new[]{6f,size + (index == 13 ? -.2f : .2f)});
            };
            if (index == 15) return () => EditOriginalParameters(new[]{7f,0f});
            if (index >= 16 && index <= 18)
                return () => EditOriginalParameters(index == 18 ? new[]{15f,0f} : new[]{1500f,index == 16 ? -.5f : .5f});
            return null;
        }

        Action Scor4AccordionDetailsAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index < 3) return () => EditOriginalParameters(new[]{7f,index+2f,8f,0f,9f,0f});
            if (index >= 10 && index <= 13 || index == 15 || index == 16) {
                int parameter = index <= 13 ? 8 : 9;
                int place = index <= 13 ? new[]{1,10,100,1000}[index-10] : index == 15 ? 100 : 1;
                return () => {
                    float value = canvas.Selected.P(parameter);
                    int digit = (int)value/place%10;
                    EditOriginalParameters(new[]{parameter*100f,digit == 0 ? (float)place : -place});
                };
            }
            return null;
        }

        Action Scor4BendAction(int index) {
            if (index >= 10 && index <= 15) return () => {
                int control = index - 10;
                int parameter = control < 3 ? 6 : 7;
                if (control % 3 == 2) {
                    EditOriginalParameters(new[]{(float)parameter,parameter == 6 ? 0f : 1f});
                    return;
                }
                float size = canvas.Selected.P(parameter);
                if (size == 0) size = 1f;
                EditOriginalParameters(new[]{(float)parameter,size + (control % 3 == 0 ? -.1f : .1f)});
            };
            if (index == 16 || index == 17) return () => EditOriginalParameters(new[]{1100f,index == 16 ? -5f : 5f});
            if (index == 18) return () => EditOriginalParameters(new[]{11f,0f});
            if (index == 19) return () => EditOriginalParameters(new[]{7f,canvas.Selected.P(6)});
            return null;
        }

        static bool IsScor4OctaveMark(Item item) {
            float kind = item.P(7);
            return (item.P(12) == -1 || item.P(12) == -2) &&
                (kind == 8 || kind == 88 || kind == -8 || kind == -88 || kind == 15 || kind == 155);
        }

        Action Scor4OctaveEditAction(int index) {
            if (index == 16) return () => EditOriginalParameters(new[]{12f,canvas.Selected.P(12) == -2 ? -1f : -2f});
            return () => EditOriginalParameters(new[]{7f,new[]{8f,88f,-8f,-88f,15f,155f}[index - 10]});
        }

        Action Scor4OctaveOffsetAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index < 2) return () => EditOriginalParameters(new[]{1500f,index == 0 ? -.5f : .5f});
            if (index == 2) return () => EditOriginalParameters(new[]{15f,0f});
            if (index < 5) return () => EditOriginalParameters(new[]{16f,1f,1800f,index == 3 ? -.5f : .5f});
            if (index == 5) return () => EditOriginalParameters(new[]{16f,0f,18f,0f});
            return null;
        }

        Action Scor4TrillEditAction(int index) {
            if (index <= 15) return Scor4OctaveOffsetAction(index - 10);
            if (index == 16) return () => {
                float right = canvas.Selected.P(6) == 0 ? canvas.Selected.P(3) + 20 : 0;
                EditOriginalParameters(new[]{6f,right});
            };
            if (index == 17) return () => {
                float accidental = canvas.Selected.P(8);
                if (accidental < 0 || accidental > 3 || accidental != Math.Truncate(accidental))
                    throw new NotSupportedException("This trill accidental is outside the ordinary cycle.");
                EditOriginalParameters(new[]{8f,accidental == 3 ? 0 : accidental + 1});
            };
            if (index == 18) return () => Execute("CN");
            return null;
        }

        void SetScor4SymbolEditBank(int bank) {
            scor4SymbolEditBank = bank;
            UpdateClassic();
        }

        Action Scor4LibrarySymbolAction(int index) {
            if (index == 3) return () => {
                float vertical = canvas.Selected.P(7);
                float horizontal = Math.Abs(canvas.Selected.P(6));
                if (horizontal == 0) horizontal = 1;
                EditOriginalParameters(new[]{7f,vertical == 0 ? -horizontal : -vertical});
            };
            if (index == 9) return () => SetScor4SymbolEditBank(1);
            if (index < 12) return () => EditOriginalParameters(new[]{1500f,index == 10 ? -.5f : .5f});
            if (index == 12) return () => EditOriginalParameters(new[]{15f,0f});
            int parameter = index < 16 ? 6 : 7;
            return () => {
                float size = canvas.Selected.P(parameter);
                if (size == 0) size = 1;
                float next = index == 15 || index == 18 ? 0 : size + (index == 13 || index == 16 ? -.1f : .1f);
                EditOriginalParameters(new[]{(float)parameter,next});
            };
        }

        Action Scor4MoreSymbolAction(int index) {
            if (index == 8) return () => SetScor4SymbolEditBank(scor4SymbolEditBank - 1);
            if (index == 9 && scor4SymbolEditBank < Scor4SymbolEditLabels.Length)
                return () => SetScor4SymbolEditBank(scor4SymbolEditBank + 1);
            if (scor4SymbolEditBank == 1) {
                if (index < 2) return () => EditOriginalParameters(new[]{900f,index == 0 ? -5f : 5f});
                if (index == 2) return () => EditOriginalParameters(new[]{9f,0f});
                if (index >= 4 && index <= 7)
                    return () => EditOriginalParameters(new[]{500f,new[]{-1f,1f,-10f,10f}[index - 4]});
                if (index == 15 || index == 16) return () => MoveScor4ItemStaff(index == 15 ? -1 : 1);
                return null;
            }
            if ((scor4SymbolEditBank == 2 || scor4SymbolEditBank == 5) && index >= 18)
                return () => EditOriginalParameters(new[]{500f,index == 18 ? -1f : 1f});
            if (index == 9) return null;
            if (scor4SymbolEditBank == 5 && index == 5)
                return () => EditOriginalParameters(new[]{5f,31f,6f,.25f});
            float symbol = Scor4SymbolValues[scor4SymbolEditBank - 2][index];
            return () => EditOriginalParameters(new[]{5f,symbol});
        }

        Action Scor4NumberEditAction(int index) {
            if (index == 3) return () => Execute("CM");
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.NumberDetails);
            if (index <= 12) return () => EditOriginalParameters(new[]{8f,new[]{0f,2f,1f}[index - 10]});
            if (index == 13) return () => SetScor4NoteBank(Scor4NoteBank.NumberFonts);
            if (index >= 14 && index <= 17)
                return () => EditOriginalParameters(new[]{500f,new[]{-10f,10f,-1f,1f}[index - 14]});
            return null;
        }

        Action Scor4NumberDetailsAction(int index) {
            if (index == 6 || index == 7) return () => EditOriginalParameters(new[]{16f,index == 6 ? 1f : 0f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 10 || index == 11) return () => EditOriginalParameters(new[]{1500f,index == 10 ? -.5f : .5f});
            if (index == 12) return () => EditOriginalParameters(new[]{15f,0f});
            if (index >= 13 && index <= 15) return () => {
                float size = canvas.Selected.P(6);
                if (size == 0) size = 1;
                EditOriginalParameters(new[]{6f,index == 15 ? 0 : size + (index == 13 ? -.1f : .1f)});
            };
            if (index == 16) return () => {
                float spread = canvas.Selected.P(14);
                EditOriginalParameters(new[]{14f,(spread == 0 ? 1 : spread) + .02f});
            };
            if (index == 17) return () => EditOriginalParameters(new[]{14f,0f});
            if (index == 18 || index == 19) return () => EditOriginalParameters(new[]{5f,index == 18 ? 1f : 1001f});
            return null;
        }

        Action Scor4NumberFontsAction(int index) {
            bool more = scor4NoteBank == Scor4NoteBank.MoreNumberFonts;
            bool meter = canvas.Selected.Code == 18;
            bool tablature = canvas.Selected.Code == 1;
            bool chord = canvas.Selected.Code == 12;
            bool text = canvas.Selected.Code == 16;
            int parameter = meter ? 13 : tablature ? 12 : chord ? 16 : text ? 8 : 7;
            var parent = meter ? Scor4NoteBank.MeterDetails : tablature ? Scor4NoteBank.Tab :
                chord ? Scor4NoteBank.GuitarName : text ? Scor4NoteBank.TextDetails : Scor4NoteBank.Item;
            if (index == 8) return () => SetScor4NoteBank(more ? Scor4NoteBank.NumberFonts : parent);
            if (index == 9 && !more) return () => SetScor4NoteBank(Scor4NoteBank.MoreNumberFonts);
            if (index == 19 && more)
                return chord ? null : (Action)(() => EditOriginalParameters(new[]{(float)parameter,meter || text ? 1094f : tablature ? 94f : 6f}));
            if ((meter || text) && !more && index == 19)
                return () => EditOriginalParameters(new[]{(float)parameter,0f});
            if (text && !more && index == 18) return () => {
                var item = canvas.Selected;
                int fontOverride = (int)item.P(8);
                int font;
                if (fontOverride >= 1000) font = fontOverride % 100;
                else if (fontOverride >= 90) font = fontOverride % 10;
                else if (!ScoreText.FontCode(item.Text,0,out font))
                    throw new NotSupportedException("The SCORE outline box needs an explicit font override or leading font code.");
                EditOriginalParameters(new[]{8f,1100f+font});
            };
            if (index >= 8 && index < 10 || index >= 18 && !more) return null;
            int slot = (more ? 16 : 0) + (index < 8 ? index : index - 2);
            return () => EditOriginalParameters(new[]{(float)parameter,tablature ? slot : chord ? 10f*slot : 1000f + slot});
        }

        Action Scor4SignatureAction(int code,int index) {
            if (index < 12) return () => EditOriginalParameters(new[]{1500f,index == 10 ? -.5f : .5f});
            if (index == 12) return () => EditOriginalParameters(new[]{15f,0f});
            if (code == 18) return () => EditOriginalParameters(index <= 16
                ? new[]{5f,index - 11f,6f,4f}
                : new[]{5f,index == 17 ? 98f : 99f,6f,0f});
            if (index == 13 || index == 15) return () => EditOriginalParameters(new[]{5f,index == 13 ? -1f : 1f});
            return () => {
                float key = canvas.Selected.P(5);
                float magnitude = Math.Abs(key);
                float sign = key < 0 ? -1 : 1;
                float next;
                if (index == 14 || index == 16) next = key + (index == 14 ? -1f : 1f);
                else if (index == 17) next = sign * (magnitude % 100 + 100);
                else next = sign * (magnitude % 100);
                EditOriginalParameters(new[]{5f,next});
            };
        }

        Action Scor4KeyDetailsAction(int index) {
            if (index < 4) return () => EditOriginalParameters(new[]{6f,(float)index});
            if (index == 4) return () => {
                float clef = canvas.Selected.P(6);
                EditOriginalParameters(new[]{6f,clef >= 100 ? clef - 100 : clef + 100});
            };
            if (index >= 5 && index <= 7) return () => {
                float spacing = canvas.Selected.P(7);
                if (spacing == 0) spacing = 1;
                EditOriginalParameters(new[]{7f,index == 7 ? 0 : spacing + (index == 5 ? -.05f : .05f)});
            };
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => EditOriginalParameters(new[]{8f,0f,9f,0f,10f,0f,11f,0f,12f,0f,13f,0f});
            if (index >= 10 && index <= 15) return () => {
                int parameter = index - 2;
                float value = canvas.Selected.P(parameter);
                if (value < 0 || value >= 400 || value != Math.Truncate(value))
                    throw new NotSupportedException("Cycling this irregular key accidental still needs DOS verification.");
                float next = value == 0 ? 108 : value >= 300 ? 0 : value + 100;
                EditOriginalParameters(new[]{(float)parameter,next});
                scor4KeyAccidentalParameter = parameter;
            };
            if (index == 16 || index == 17) return () => {
                int parameter = scor4KeyAccidentalParameter;
                if (parameter == 0 || canvas.Selected.P(parameter) == 0) return;
                EditOriginalParameters(new[]{parameter * 100f,index == 16 ? -1f : 1f});
            };
            return () => EditOriginalParameters(new[]{14f,index == 18 ? 1f : 0f});
        }

        Action Scor4MeterDetailsAction(int index) {
            if (index < 5) return () => EditOriginalParameters(new[]{5f,new[]{3f,5f,6f,9f,12f}[index],6f,8f});
            if (index == 5 || index == 6) return () => {
                float size = canvas.Selected.P(7);
                if (size == 0) size = 1;
                double delta = index == 5 ? -.2 : .2;
                // The menu writes a three-place size before the parameter is stored.
                float next = (float)Math.Round(size + delta,3,MidpointRounding.AwayFromZero);
                EditOriginalParameters(new[]{7f,next,400f,index == 5 ? .62f : -.62f});
            };
            if (index == 7) return () => EditOriginalParameters(new[]{4f,0f,7f,0f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.DoubleMeter);
            if (index == 10) return () => EditOriginalParameters(new[]{5f,0f,6f,1f});
            if (index == 11) return () => EditOriginalParameters(new[]{600f,1f});
            if (index >= 12 && index <= 14) return () => EditOriginalParameters(new[]{5f,3f * (index - 11),6f,16f});
            if (index == 15 || index == 16) return () => EditOriginalParameters(new[]{500f,index == 15 ? -1f : 1f});
            if (index == 17) return () => SetScor4NoteBank(Scor4NoteBank.NumberFonts);
            if (index == 18 || index == 19) return () => EditOriginalParameters(new[]{14f,index == 18 ? 1f : 0f});
            return null;
        }

        Action Scor4DoubleMeterAction(int index) {
            if (index == 0 || index == 1) return () => EditOriginalParameters(new[]{500f,index == 0 ? -1f : 1f});
            if (index == 3) return () => EditOriginalParameters(new[]{8f,0f,9f,0f});
            if (index == 5 || index == 6) return () => EditOriginalParameters(new[]{800f,index == 5 ? -1f : 1f});
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.MeterDetails);
            if (index >= 10 && index <= 13) return () => EditOriginalParameters(new[]{6f,(float)(2 << (index - 10))});
            if (index == 14) return () => EditOriginalParameters(new[]{12f,canvas.Selected.P(12) == 0 ? 1f : 0f});
            if (index >= 15) return () => EditOriginalParameters(new[]{9f,index == 19 ? 0f : (float)(2 << (index - 15))});
            return null;
        }

        void AdjustScor4StaffLines(int index) {
            float lines = canvas.Selected.P(7);
            if (index == 12) {
                EditOriginalParameters(new[]{7f,0f});
                return;
            }
            if (lines < -1 || lines != Math.Truncate(lines) || lines % 100 > 6 || lines >= 600)
                throw new NotSupportedException("Changing this special staff line count still needs DOS verification.");
            EditOriginalParameters(new[]{7f,lines == -1 ? 0 : lines + (index == 10 ? -1 : 1)});
        }

        Action Scor4BarEditAction(int index) {
            if (index == 3) return () => EditOriginalParameters(new[]{3f,canvas.Selected.P(3) == 0 ? 200f : 0f});
            if (index == 10 || index == 11) return () => EditOriginalParameters(new[]{400f,index == 10 ? -1f : 1f});
            if (index == 12) return () => EditOriginalParameters(new[]{4f,1f});
            float style = new[]{0f,1f,2f,4f,3f,5f}[index - 13];
            return () => EditOriginalParameters(new[]{5f,style});
        }

        void SetScor4BarEditBank(int bank) {
            scor4BarEditBank = bank;
            UpdateClassic();
        }

        Action Scor4MoreBarAction(int index) {
            if (index == 8) return () => SetScor4BarEditBank(scor4BarEditBank - 1);
            if (scor4BarEditBank == 2) return Scor4FineBarAction(index);
            if (index == 9) return () => SetScor4BarEditBank(2);
            if (index < 2) return () => EditOriginalParameters(new[]{1500f,index == 0 ? -.5f : .5f});
            if (index == 2) return () => EditOriginalParameters(new[]{15f,0f});
            if (index == 3) return () => EditOriginalParameters(new[]{4f,-Math.Max(1,Math.Abs(canvas.Selected.P(4))),5f,0f});
            if (index >= 4 && index <= 7) return () => {
                float style = new[]{6f,8f,9f,10f}[index - 4];
                EditOriginalParameters(new[]{4f,Math.Abs(canvas.Selected.P(4)),5f,style});
            };
            if (index >= 10 && index <= 12)
                return () => EditOriginalParameters(new[]{5f,9f,8f,index - 9f});
            if (index == 13 || index == 14)
                return () => EditOriginalParameters(index == 13 ? new[]{1300f,.2f} : new[]{13f,0f});
            if (index == 15 || index == 16)
                return () => EditOriginalParameters(index == 15 ? new[]{1400f,.2f} : new[]{14f,0f});
            if (index == 17 || index == 18)
                return () => EditOriginalParameters(index == 17 ? new[]{1700f,.05f} : new[]{17f,0f});
            if (index == 19) return () => EditOriginalParameters(new[]{5f,7f,8f,0f,9f,0f});
            return null;
        }

        Action Scor4FineBarAction(int index) {
            if (index < 4) return () => {
                float value = canvas.Selected.P(16);
                if (value == 0) value = 608;
                float offset = new[]{-200f,200f,-2f,2f}[index];
                EditOriginalParameters(new[]{16f,value + offset});
            };
            if (index == 4) return () => EditOriginalParameters(new[]{16f,0f});
            if (index >= 5 && index <= 7) return () => {
                float value = canvas.Selected.P(6);
                EditOriginalParameters(new[]{6f,index == 7 ? 0 : Math.Max(0,value + (index == 5 ? -.2f : .2f))});
            };
            if (index >= 10 && index <= 15) return () => {
                int parameter = index < 13 ? 10 : 11;
                int action = (index - 10) % 3;
                float value = canvas.Selected.P(parameter);
                if (value == 0) value = parameter == 10 ? 3 : 11;
                EditOriginalParameters(new[]{(float)parameter,action == 2 ? 0 : value + (action == 0 ? -1f : 1f)});
            };
            if (index >= 16) return () => {
                if (canvas.Selected.P(5) != 7) return;
                int parameter = index < 18 ? 8 : 9;
                bool reset = index % 2 != 0;
                EditOriginalParameters(new[]{reset ? parameter : parameter * 100f,reset ? 0f : .2f});
            };
            return null;
        }

        void SetScor4ItemsBank(int bank) {
            scor4ItemsBank = bank;
            UpdateClassic();
        }

        Action Scor4ItemsAction(int index) {
            if (index == 8) return ReturnScor4Bank;
            if (scor4ItemsBank == 2) {
                if (index == 17 || index == 18) return () => BeginScor4ExternalItem(index == 18);
                float[] defaults = Scor4SecondItemDefaults[index];
                return defaults == null ? (Action)null : () => InsertScor4Item(defaults);
            }
            if (index == 9) return () => SetScor4ItemsBank(2);
            if (index == 7) return InsertScor4Staff;
            if (index == 19) return () => { SetScor4ItemsBank(0); Execute("T"); };
            if (index < Scor4ItemDefaults.Length) return () => InsertScor4Item(Scor4ItemDefaults[index]);
            if (index >= 10 && index - 10 < Scor4AltItemDefaults.Length)
                return () => InsertScor4Item(Scor4AltItemDefaults[index - 10]);
            return null;
        }

        void InsertScor4Item(float[] defaults) {
            float[] parameters = (float[])defaults.Clone();
            parameters[1] = Scor4InsertionStaff();
            AppendScor4ItemForEdit(Item.Make(parameters));
        }

        int Scor4InsertionStaff() {
            if (score.Items.Count == 0) return 1;
            int[] staves = score.Items.Where(item => item.Code == 8).Select(item => item.Staff).Distinct().ToArray();
            return staves.Length == 1 ? staves[0] : Scor4PlacementStaff();
        }

        void BeginScor4ExternalItem(bool postScript) {
            int staff = Scor4InsertionStaff();
            Item pending = postScript ? Item.Make(15, staff, 1, 1, 1) : Item.Make(11, staff, 100, 15);
            pending.Set(13, 0);
            BeginExternalDrawingName(pending);
            scor4ExternalItemInsertion = true;
            string name = postScript ? "PostScript program" : "library";
            string resetHint = postScript ? "(To reset PostScript program name: Enter EDIT MODE, set P13 to 1.)" + Environment.NewLine : "";
            Message(resetHint + "Enter " + name + " name (\"-\"=cancel, \"*\"=dir.) ->");
        }

        int Scor4PlacementStaff() {
            int origin, divisor;
            if (canvas.HasZoomCommand && canvas.Scor4LiteralZoom) {
                canvas.LayoutScore();
                origin = NativeDosViewport.OriginY;
                divisor = NativeDosViewport.Divisor;
            } else {
                if (canvas.HasZoomCommand || NativeDosViewport.ZoomFactor != 1)
                    throw new NotSupportedException("SCOR4 Items placement in this viewport has not been verified.");
                short? top = NativeDosViewport.Scor4TopOrigin(canvas);
                if (!top.HasValue) throw new NotSupportedException("SCOR4 Items placement requires a measured initial graphics frame.");
                divisor = 50;
                origin = top.Value;
            }
            // 25C2:00B2–0138 reads each of the 32 baseline entries and keeps the first minimum.
            int bestStaff = 1, bestDistance = 32500;
            for (int staff = 1; staff <= 32; staff++) {
                int baseline = checked((int)canvas.PageDocument.Tables.Bases[staff]);
                int distance = Math.Abs(origin - baseline / divisor - 430);
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                bestStaff = staff;
            }
            return bestStaff;
        }

        int NextScor4Staff() {
            var occupied = score.Items.Where(item => item.Code == 8).Select(item => item.Staff).ToArray();
            int number = Enumerable.Range(1, 32).FirstOrDefault(staff => !occupied.Contains(staff));
            if (number == 0) throw new NotSupportedException("All 32 SCORE staff positions are in use.");
            return number;
        }

        void InsertScor4Staff() {
            AppendScor4ItemForEdit(Item.Make(8, NextScor4Staff(), 0, 0, 0, 200));
        }

        bool ExecuteScor4ItemCommand(string verb, string arguments) {
            if (!scor4Interface || arguments.Length != 0) return false;
            switch (verb) {
                case "NOTE": InsertScor4Item(Scor4ItemDefaults[0]); return true;
                case "NUM": InsertScor4Item(Scor4AltItemDefaults[1]); return true;
                // The supplied 4.01 executable resolves bare MET to a meter item.
                // Its METR/METRIC route continues to select measurement units.
                case "MET": InsertScor4Item(Scor4AltItemDefaults[5]); return true;
                case "LINE": InsertScor4Item(Scor4ItemDefaults[3]); return true;
                case "SLUR": InsertScor4Item(Scor4ItemDefaults[4]); return true;
                case "HARP": InsertScor4Item(new[]{7f,0f,100f,-1f,0f,0f,1f,111f,1111f}); return true;
                case "PEDA": InsertScor4Item(Scor4SecondItemDefaults[1]); return true;
                case "PEDB": InsertScor4Item(Scor4SecondItemDefaults[2]); return true;
                case "PEDC": InsertScor4Item(Scor4SecondItemDefaults[3]); return true;
                case "BOX": InsertScor4Item(new[]{12f,0f,100f,15f,0f,10f}); return true;
                case "CIR": InsertScor4Item(new[]{12f,0f,100f,15f,1f,10f}); return true;
                // Typing STAFF omits the explicit right edge used by the Items F8 button.
                case "STAFF": AppendScor4ItemForEdit(Item.Make(8, NextScor4Staff(), 0)); return true;
                default: return false;
            }
        }

        void AppendScor4ItemForEdit(Item itemToEdit) {
            using (var storage = BeginParameterMutation()) {
                Checkpoint();
                score.Items.Add(itemToEdit);
                dirty = true;
                scor4ItemsBank = 0;
                direction = 1;
                BeginEdit(itemToEdit);
                RefreshAll(itemToEdit);
                storage.Complete();
            }
        }
    }
}
