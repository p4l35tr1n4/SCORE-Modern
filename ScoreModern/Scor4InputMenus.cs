using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
    public sealed partial class Editor {
        bool scor4RhythmSecond;
        short scor4RhythmWord;
        int scor4MarksBank;
        enum Scor4PitchBank { Main, More, Clefs, KeySignatures, Meter, Bars }
        Scor4PitchBank scor4PitchBank;
        static readonly string[] Scor4PitchLabels = {
            "Clef","StmUp","StmDn","StmOr","M","Rest","RsWhl","RsInv","<esc>","-->",
            "Flat","Sharp","Natural","DblFlt","DblShp",":","BackSpc","/",";","(acc)"
        };
        static readonly string[] Scor4PitchMore = {
            "","","PrxOn","PrOff","","KySig","Meter","Bars","<--","",
            "","","","","","StafUp","StafDn","StafOr","",""
        };
        static readonly string[] Scor4PitchClefs = {
            "Treb","Bass","Alto","Tenor","Perc","","","","<--","",
            "-Treb","-Bass","-Alto","-Tenor","","","","","",""
        };
        static readonly string[] Scor4PitchKeys = {
            "1 Flat","2 Flats","3 Flats","4 Flats","5 Flats","6 Flats","7 Flats","","<--","",
            "1 Sharp","2 Shrps","3 Shrps","4 Shrps","5 Shrps","6 Shrps","7 Shrps","Natural","BackSpc",""
        };
        static readonly string[] Scor4PitchMeters = {
            "3/16","2/4","3/4","4/4","5/4","6/4","C","Cut","<--","9/16",
            "6/16","2/8","3/8","4/8","5/8","6/8","7/8","8/8","9/8","12/8"
        };
        static readonly string[] Scor4PitchBars = {
            "M","MD","MH","MR","ML","MRD","MS","MI","<--","",
            "1 Staff","2 Stvs","3 Stvs","4 Stvs","5 Stvs","6 Stvs","7 Stvs","8 Stvs","9 Stvs",""
        };
        static readonly string[] Scor4PrimaryPitchTokens={"","SU/","SD/","SO/","M","R","RW","RI"};
        static readonly string[] Scor4PitchAccidentals={"F","S","N","FF","SS",":"};
        static readonly string[] Scor4PitchClefTokens={"TR","BA","AL","TE","PR"};
        static readonly string[] Scor4RhythmLabels = {
            "DblWh", "Whole", "1/2", "1/4", "1/8", "1/16", "1/32", "1/64", "<esc>", "-->",
            "Quint", "Triple", "1/2.", "1/4.", "1/8.", "Dot", "BackSpc", "/", ";", "1/12"
        };
        static readonly string[] Scor4RhythmMore = {
            "", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "<--", "Grace",
            "X9", "X10", "X12", "X16", "X20", "X24", "X28", "X32", "X35", "X40"
        };
        static readonly string[] Scor4BeamLabels = {
            "1B;","2B;","3B;","4B;","5","6","3","Go","<esc>","Above",
            "1 Pkup","2 Pkup","3 Pkup","","AutoBms","","BackSpc","/",";","Below"
        };
        static readonly string[] Scor4SlurLabels = {
            "","","","","5","6","3","Go","<esc>","Above",
            "","","","","","","BackSpc","/",";","Below"
        };
        static readonly string[][] Scor4SlurEditLabels = {
            new[]{"LftHf","RhtHf","ChDir","0P12","Dash","Dots","End1","End2","<--","-->",
                "P8-.05","P8+.05","-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","P8-1","P8+1"},
            new[]{"","","CNLeft","","CNRight","","","","<--","Mid",
                "","","","","","","","","",""}
        };

        Action Scor4PrimarySlurAction(int index) {
            if (index == 10 || index == 11)
                return () => EditOriginalParameters(new[]{8f,index == 10 ? -1f : -2f});
            if (index == 12) return () => EditOriginalParameters(new[]{7f,3f,8f,3f});
            return () => {
                if (canvas.Selected.P(8) != 3)
                    throw new NotSupportedException("The tuplet increment outside the checked bracket form still needs DOS verification.");
                EditOriginalParameters(new[]{900f,1f});
            };
        }

        void SetScor4SlurEditBank(int bank) {
            scor4SlurEditBank = bank;
            UpdateClassic();
        }

        Action Scor4SlurEditAction(int index) {
            if (index == 8) return () => SetScor4SlurEditBank(0);
            if (scor4SlurEditBank == 1) {
                if (index >= 0 && index <= 3) {
                    float[] pieces = {1,2,-1,0};
                    return () => EditOriginalParameters(new[]{12f,pieces[index]});
                }
                if (index == 4 || index == 5)
                    return () => EditOriginalParameters(new[]{8f,0f,11f,index == 4 ? 1f : -1f});
                if (index == 6 || index == 7)
                    return () => EditOriginalParameters(new[]{7f,5f,8f,index - 5f,11f,0f});
                if (index == 10 || index == 11)
                    return () => {
                        float value = canvas.Selected.P(8);
                        if (index == 10 ? value > 0 : value < .95f)
                            EditOriginalParameters(new[]{800f,index == 10 ? -.05f : .05f});
                    };
                if (index >= 12 && index <= 17)
                    return () => EditScor4SlurOffset(index);
                if (index == 18 || index == 19)
                    return () => {
                        float value = canvas.Selected.P(8);
                        if (value >= 0) EditOriginalParameters(new[]{8f,-1f});
                        else EditOriginalParameters(new[]{800f,index == 18 ? -1f : 1f});
                    };
                return index == 9 ? (Action)(() => SetScor4SlurEditBank(2)) : null;
            }
            switch (index) {
                case 2: case 3: return () => CenterScor4Slur(true, "");
                case 4: case 5: return () => CenterScor4Slur(false, "");
                case 9: return () => {
                    editEndpoint = editEndpoint == 3 ? 0 : 3;
                    editParameter = 0;
                    UpdateClassic();
                };
                default: return null;
            }
        }

        void EditScor4SlurOffset(int index) {
            int parameter = index < 15 ? 17 : 18;
            int otherParameter = parameter == 17 ? 18 : 17;
            int action = (index - 12) % 3;
            float offset = action == 2 ? 0 : canvas.Selected.P(parameter) + (action == 0 ? -.5f : .5f);
            float useOffsets = action == 2 && canvas.Selected.P(otherParameter) == 0 ? 0 : 1;
            EditOriginalParameters(new[]{16f,useOffsets,(float)parameter,offset});
        }
        static readonly string[][] Scor4MarksLabels = {
            new[]{"Wedge","Accnt","Tenut","Stacc","Ferma","AccSt","Harm","Go","<esc>","-->"},
            new[]{"HvyWd","ACtTn","TenSt","WdgSt","C <","C- >","DnBow","UpBow","<--","-->"},
            new[]{"ppp","pp","p","mp","mf","f","pizz","arco","<--","-->"},
            new[]{"ff","fff","fp","sf","sfz","","","","<--","-->"},
            new[]{"cresc","dim","rit","accel","8va","8ba","","","<--","-->"},
            new[]{"Fing1","Fing2","Fing3","Fing4","Fing5","FictFl","FictSh","FictNt","<--","-->"},
            new[]{"Trill","TrFlt","TrShp","TrNat","","","","","<--","-->"},
            new[]{"","","","","","","","","<--",""}
        };
        static readonly string[][] Scor4MarksTokens = {
            new[]{"W","A","T","S","FE","AS","H"},
            new[]{"HW","ACT","TS","WS","C","C-","D","U"},
            new[]{"PPP","PP","P","MP","MF","F","PZ","AO"},
            new[]{"FF","FFF","FP","SF","SFZ"},
            new[]{"CR","DI","RI","AC","O","OB"},
            new[]{"F1","F2","F3","F4","F5","FIF","FIS","FIN"},
            new[]{"TR","TRF","TRS","TRN"},
            new string[0]
        };

        bool Scor4InputStageActive(InputStaffStage stage) {
            return scor4Interface && musicInput != null && musicInput.Transaction != null &&
                musicInput.Transaction.Stage == stage && !musicInput.SpacingPause &&
                !musicInput.AbandonQuestion && !musicInput.FilenameQuestion &&
                musicInput.Correction < 0 && musicInput.MotiveReplacementLine == null;
        }
        bool Scor4RhythmActive { get { return Scor4InputStageActive(InputStaffStage.Rhythms); } }
        bool Scor4PitchActive { get { return Scor4InputStageActive(InputStaffStage.Pitches); } }
        bool Scor4MarksActive { get { return Scor4InputStageActive(InputStaffStage.Marks); } }
        bool Scor4BeamActive { get { return Scor4InputStageActive(InputStaffStage.Beams); } }
        bool Scor4SlurActive { get { return Scor4InputStageActive(InputStaffStage.Slurs); } }
        bool Scor4StaffMenusActive { get { return Scor4PitchActive||Scor4RhythmActive||Scor4MarksActive||Scor4BeamActive||Scor4SlurActive; } }

        string[] CurrentScor4PitchLabels() {
            switch(scor4PitchBank){
                case Scor4PitchBank.Clefs:return Scor4PitchClefs;
                case Scor4PitchBank.More:return Scor4PitchMore;
                case Scor4PitchBank.KeySignatures:return Scor4PitchKeys;
                case Scor4PitchBank.Meter:return Scor4PitchMeters;
                case Scor4PitchBank.Bars:return Scor4PitchBars;
                default:return Scor4PitchLabels;
            }
        }

        void SetScor4PitchBank(Scor4PitchBank bank) {
            scor4PitchBank=bank;
            UpdateMusicInput();
        }

        void EnterScor4PitchAndReturn(string token) {
            AppendMusicInputFunctionToken(token);
            SetScor4PitchBank(Scor4PitchBank.Main);
        }

        Action Scor4PitchAction(int index) {
            if(scor4PitchBank==Scor4PitchBank.Bars){
                if(index<8)return ()=>AppendMusicInputFunctionToken(Scor4PitchBars[index]);
                if(index==8)return ()=>SetScor4PitchBank(Scor4PitchBank.Main);
                if(index>=10&&index<=18)return ()=>{
                    StoreInputMouseToken((index-9).ToString(CultureInfo.InvariantCulture));
                    SetScor4PitchBank(Scor4PitchBank.Main);
                };
                return null;
            }
            if(scor4PitchBank==Scor4PitchBank.Meter){
                if(index==8)return ()=>SetScor4PitchBank(Scor4PitchBank.Main);
                string meter=index==6?"COM":index==7?"CUT":Scor4PitchMeters[index].Replace('/',' ');
                return ()=>EnterScor4PitchAndReturn(meter);
            }
            if(scor4PitchBank==Scor4PitchBank.KeySignatures){
                if(index<7||index>=10&&index<17)
                    return ()=>AppendMusicInputFunctionToken("K"+(index%10+1).ToString(CultureInfo.InvariantCulture)+(index<10?"F":"S"));
                if(index==8)return ()=>SetScor4PitchBank(Scor4PitchBank.Main);
                if(index==18)return ()=>((NativeCommandBox)command).ReplayEditingKey(Keys.Back);
                return null;
            }
            if(scor4PitchBank==Scor4PitchBank.Clefs){
                if(index<5)return ()=>EnterScor4PitchAndReturn(Scor4PitchClefTokens[index]);
                if(index>=10&&index<14)return ()=>EnterScor4PitchAndReturn("-"+Scor4PitchClefTokens[index-10]);
                if(index==8)return ()=>SetScor4PitchBank(Scor4PitchBank.Main);
                return null;
            }
            if(scor4PitchBank==Scor4PitchBank.More){
                if(index==2||index==3)return ()=>AppendMusicInputFunctionToken(index==2?"P":"O");
                if(index==5)return ()=>SetScor4PitchBank(Scor4PitchBank.KeySignatures);
                if(index==6)return ()=>SetScor4PitchBank(Scor4PitchBank.Meter);
                if(index==7)return ()=>SetScor4PitchBank(Scor4PitchBank.Bars);
                if(index==8)return ()=>SetScor4PitchBank(Scor4PitchBank.Main);
                if(index>=15&&index<=17)
                    return ()=>AppendMusicInputFunctionToken(index==15?"S+/":index==16?"S-/":"S0/");
                return null;
            }
            if(index==0)return ()=>SetScor4PitchBank(Scor4PitchBank.Clefs);
            if(index<8)return ()=>AppendMusicInputFunctionToken(Scor4PrimaryPitchTokens[index]);
            if(index==8)return EscapeMusicInput;
            if(index==9)return ()=>SetScor4PitchBank(Scor4PitchBank.More);
            if(index>=10&&index<16)return ()=>StoreInputMouseToken(Scor4PitchAccidentals[index-10]);
            if(index==16)return ()=>((NativeCommandBox)command).ReplayEditingKey(Keys.Back);
            if(index==17)return ()=>StoreInputMouseToken("/");
            if(index==18)return Scor4InputSemicolon;
            return null;
        }

        string[] CurrentScor4MarksLabels() {
            var labels=new string[20];
            Array.Copy(Scor4MarksLabels[scor4MarksBank],labels,10);
            for(int i=10;i<20;i++)labels[i]="";
            if(scor4MarksBank<=5)
                for(int i=0;i<Math.Min(6,Scor4MarksTokens[scor4MarksBank].Length);i++)labels[10+i]="-";
            else if(scor4MarksBank==6)
                Array.Copy(new[]{"Tr~~~","TrF~~","TrS~~","TrN~~"},0,labels,10,4);
            else
                Array.Copy(new[]{"TM1(8)","TM2(16)","TM3(32)","TM4(64)"},0,labels,10,4);
            labels[16]="BackSpc";labels[17]="/";labels[18]=";";
            return labels;
        }

        void MoveScor4MarksBank(int offset) {
            scor4MarksBank+=offset;
            UpdateMusicInput();
        }

        Action Scor4MarksAction(int index) {
            var tokens=Scor4MarksTokens[scor4MarksBank];
            if(index<tokens.Length)return ()=>AppendMusicInputFunctionToken(tokens[index]+" ");
            if(index==7&&scor4MarksBank==0)return Scor4GoInput;
            if(index==8)return scor4MarksBank==0?(Action)EscapeMusicInput:()=>MoveScor4MarksBank(-1);
            if(index==9&&scor4MarksBank<Scor4MarksLabels.Length-1)return ()=>MoveScor4MarksBank(1);
            if(index>=10&&index<16){
                int token=index-10;
                if(scor4MarksBank<=5&&token<tokens.Length)
                    return ()=>AppendMusicInputFunctionToken("-"+tokens[token]+" ");
                if(scor4MarksBank==6&&token<tokens.Length)
                    return ()=>AppendMusicInputFunctionToken(tokens[token]+"W ");
                if(scor4MarksBank==7&&token<4)
                    return ()=>AppendMusicInputFunctionToken("TM"+(token+1)+" ");
            }
            if(index==16)return ()=>((NativeCommandBox)command).ReplayEditingKey(Keys.Back);
            if(index==17)return ()=>StoreInputMouseToken("/");
            if(index==18)return Scor4InputSemicolon;
            return null;
        }

        void Scor4GoInput() {
            AppendMusicInputFunctionToken("G");
            string text=((NativeCommandBox)command).PhysicalText;
            command.Clear();
            SubmitMusicInput(text);
        }

        void Scor4InputSemicolon() {
            int before=command.SelectionStart;
            StoreInputMouseToken(";");
            int after=command.SelectionStart;
            if(after!=before+1||command.Text.Substring(after).Trim().Length!=0)return;
            string text=((NativeCommandBox)command).PhysicalText;
            command.Clear();
            SubmitMusicInput(text);
        }

        void Scor4AutoBeam(int group) {
            string draft=command.Text.Trim();
            string pickup=group!=0&&(draft=="-1"||draft=="-2"||draft=="-3")?draft.Substring(1):"";
            string line=(group==0?"AB":group.ToString(CultureInfo.InvariantCulture)+"B")+pickup+";";
            command.Clear();
            SubmitMusicInput(line);
        }

        void AddScor4EndpointControl(int offset) {
            int caret=command.SelectionStart;
            Match word=Regex.Match(command.Text.Substring(0,caret),@"[+]?[0-9]+$");
            int note;
            if(!word.Success||!Int32.TryParse(word.Value,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out note)||
               note<1||note>Int16.MaxValue-offset)return;
            if(word.Index>0&&command.Text[word.Index-1]=='-')return;
            command.Select(word.Index,word.Length);
            command.SelectedText=(note+offset).ToString(CultureInfo.InvariantCulture);
        }

        Action Scor4EndpointAction(int index,bool beams) {
            if(beams&&index<4)return ()=>Scor4AutoBeam(index+1);
            if(index>=4&&index<=6)return ()=>AddScor4EndpointControl(index==4?500:index==5?600:300);
            if(index==7)return Scor4GoInput;
            if(index==8)return EscapeMusicInput;
            if(index==9||index==19)return ()=>StoreInputMouseToken(index==9?"+":"-");
            if(beams&&index>=10&&index<=12)return ()=>StoreInputMouseToken("-"+(index-9).ToString(CultureInfo.InvariantCulture));
            if(beams&&index==14)return ()=>Scor4AutoBeam(0);
            if(index==16)return ()=>((NativeCommandBox)command).ReplayEditingKey(Keys.Back);
            if(index==17)return ()=>StoreInputMouseToken("/");
            if(index==18)return Scor4InputSemicolon;
            return null;
        }

        void SetScor4RhythmBank(bool second) {
            scor4RhythmSecond = second;
            UpdateMusicInput();
        }

        Action Scor4RhythmAction(int index) {
            if (scor4RhythmSecond) {
                if (index == 8) return () => SetScor4RhythmBank(false);
                if (index == 9) return () => {
                    AppendMusicInputFunctionToken("G");
                    SetScor4RhythmBank(false);
                };
                string label = Scor4RhythmMore[index];
                if (label.StartsWith("X", StringComparison.Ordinal))
                    return () => {
                        StoreInputMouseToken(label);
                        SetScor4RhythmBank(false);
                    };
                return null;
            }
            if (index == 0) return () => AppendMusicInputFunctionToken("D");
            if (index > 0 && index < 8) return () => MusicInputFunction(index + 1, Keys.None);
            if (index == 8) return EscapeMusicInput;
            if (index == 9) return () => SetScor4RhythmBank(true);
            if (index == 10 || index == 11)
                return () => ConvertScor4Rhythm(index == 10);
            if (index >= 12 && index <= 14)
                return () => {
                    scor4RhythmWord = (short)(1 << (index - 11));
                    AppendMusicInputFunctionToken(scor4DupleRhythms[index - 11] + ".");
                };
            if (index == 15) return () => StoreInputMouseToken(".");
            if (index == 16) return () => ((NativeCommandBox)command).ReplayEditingKey(Keys.Back);
            if (index == 17) return () => StoreInputMouseToken("/");
            if (index == 18) return Scor4InputSemicolon;
            if (index == 19) return () => AppendMusicInputFunctionToken("12");
            return null;
        }

        void ConvertScor4Rhythm(bool quintuplet) {
            int caret = command.SelectionStart;
            // SCOR4 2429:08F5–09D6 uses DS:A2C6 from the last function-key entry.
            // Triple also checks the preceding character before changing that word.
            if (!quintuplet && (caret == 0 || command.Text[caret - 1] < '1' || command.Text[caret - 1] > '9'))
                return;
            short previous = scor4RhythmWord;
            scor4RhythmWord = quintuplet
                ? (short)(unchecked((short)(previous * 5)) / 4)
                : unchecked((short)(previous + previous / 2));
            if (quintuplet && scor4RhythmWord < 5) return;

            var entry = (NativeCommandBox)command;
            entry.ReplayEditingKey(Keys.Back);
            if (previous > 9 && (quintuplet || scor4RhythmWord != 1))
                entry.ReplayEditingKey(Keys.Back);
            int width = scor4RhythmWord > 9 ? 2 : 1;
            string token = !quintuplet && scor4RhythmWord == 1
                ? "1.5" : scor4RhythmWord.ToString(CultureInfo.InvariantCulture);
            if (token.Length > width && token != "1.5") token = new string('*', width);
            StoreInputMouseToken(token);
            command.Focus();
            UpdateMusicInput();
        }
    }
}
