using System;

namespace ScoreModern {
    public sealed partial class Editor {
        static readonly int[] Scor4GuitarStringPlaces = {100000,10000,1000,100,10,1};
        static readonly string[] Scor4GuitarDotsLabels = {
            "Str1","Str2","Str3","Str4","Str5","Str6","St1-6","Clear","<---","",
            "Fret 1","2","3","4","5","6","7","OpenStr","X","Blank"
        };
        static readonly string[] Scor4GuitarFingerLabels = {
            "Str1","Str2","Str3","Str4","Str5","Str6","St1-6","P18=0","<---","",
            "Fing 1","2","3","4","","","Blank","0","T",""
        };
        static readonly string[] Scor4GuitarNameLabels = {
            "-ChdNt","+ChdNt","b/#/n","-ChTyp","+ChTyp","-Intvt","+Intvt","IntAc","<---","--->",
            "Font>","-FntSiz","+FntSiz","0FntSiz","","TxLv-.5","TxLv+.5","TxLv-5","TxLv+5","TxLv0"
        };
        static readonly string[] Scor4GuitarSuffixLabels = {
            "","SusAd","Acci","-Num","+Num","P10=0","","","<---","",
            "-/Note","+/Note","/Acci","-/Num","+/Num","P11=0","","","",""
        };
        static readonly string[] Scor4GuitarSizeLabels = {
            "-Size","+Size","NrmSz","","","-Strs","+Strs","0=6St","<---","",
            "-Wdth","+Wdth","NrmWd","","","-Fret","+Fret","0=5Fr","",""
        };
        static readonly string[] Scor4GuitarBarreLabels = {
            "Lft St1","Str2","Str3","Str4","Str5","","-Fr","+Fr","<---","",
            "BarrRht","Str2","Str3","Str4","Str5","Str6","-FrLev","+FrLev","NoBarre","NoFr"
        };
        int scor4GuitarString = -1;
        bool scor4GuitarAllStrings;

        static bool IsScor4GuitarGrid(Item item) {
            float kind = item.P(5);
            return kind >= -6 && kind <= -1 && kind == Math.Truncate(kind);
        }

        Action Scor4GuitarPrimaryAction(int index) {
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.GuitarSize);
            if (index == 11) return () => SetScor4NoteBank(Scor4NoteBank.GuitarName);
            if (index == 12 && canvas.Selected.P(5) > -4)
                return () => SetScor4NoteBank(Scor4NoteBank.GuitarBarre);
            if (index == 10 || index == 13) {
                if (canvas.Selected.P(5) <= -4) return null;
                return () => {
                    scor4GuitarString = -1;
                    scor4GuitarAllStrings = false;
                    SetScor4NoteBank(index == 10 ? Scor4NoteBank.GuitarDots : Scor4NoteBank.GuitarFingers);
                };
            }
            if (index >= 14 && index <= 16)
                return () => EditOriginalParameters(new[]{5f,13f - index});
            if (index == 17 || index == 18)
                return () => EditOriginalParameters(new[]{1500f,index == 17 ? -.5f : .5f});
            if (index == 19) return () => EditOriginalParameters(new[]{15f,0f});
            return null;
        }

        Action Scor4GuitarBarreAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 5 || index == 9 || index == 10) return null;
            return () => {
                float value = canvas.Selected.P(12);
                if (value < 0 || value >= 6667 || Single.IsNaN(value))
                    throw new NotSupportedException("This packed guitar barre is outside the verified range.");
                int whole = (int)value;
                float next;
                if (index <= 4) next = value - whole/1000*1000 + (index+1)*1000;
                else if (index == 6 || index == 7) {
                    int fret = whole%100;
                    if (index == 6 && fret == 0) return;
                    next = value + (index == 6 ? -1f : 1f);
                } else if (index <= 15) next = value - whole/100%10*100 + (index-9)*100;
                else if (index == 16 || index == 17) {
                    int level = (int)Math.Round((value-whole)*10.0,MidpointRounding.AwayFromZero);
                    if (index == 16 && level == 0) return;
                    next = (float)(whole + (level+(index == 16 ? -1 : 1))/10.0);
                } else if (index == 18) {
                    int level = (int)Math.Round((value-whole)*10.0,MidpointRounding.AwayFromZero);
                    next = (float)(whole%100 + level/10.0);
                } else next = whole/100*100;
                EditOriginalParameters(new[]{12f,next});
            };
        }

        Action Scor4GuitarSizeAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            int control = index%10;
            if (control <= 2) return () => {
                int parameter = index < 10 ? 6 : 17;
                float value = canvas.Selected.P(parameter);
                if (value == 0) value = 1;
                EditOriginalParameters(new[]{(float)parameter,control == 2 ? 0f : value+(control == 0 ? -.1f : .1f)});
            };
            if (control >= 5 && control <= 7) return () => {
                int packed = (int)canvas.Selected.P(14);
                int place = index < 10 ? 10 : 1;
                int count = packed/place%10;
                if (count == 0) count = index < 10 ? 6 : 5;
                int next = control == 7 ? 0 : count+(control == 5 ? -1 : 1);
                if (next < 0 || next > 9)
                    throw new NotSupportedException("This guitar grid limit still needs SCORE 4 verification.");
                int remaining = packed - (packed/place%10)*place;
                EditOriginalParameters(new[]{14f,remaining+next*place});
            };
            return null;
        }

        Action Scor4GuitarNameAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.Item);
            if (index == 9) return () => SetScor4NoteBank(Scor4NoteBank.GuitarSuffix);
            if (index == 10) return () => SetScor4NoteBank(Scor4NoteBank.NumberFonts);
            if (index >= 0 && index <= 2) return () => {
                int root = (int)canvas.Selected.P(7);
                int note = root / 10;
                int accidental = root % 10;
                if (note < 1 || note > 7 || accidental < 0 || accidental > 2)
                    throw new NotSupportedException("This chord spelling is outside the verified SCORE 4 controls.");
                if (index == 2) accidental = (accidental+1)%3;
                else note = (note + (index == 0 ? 5 : 7))%7+1;
                EditOriginalParameters(new[]{7f,note*10f+accidental});
            };
            if (index == 3 || index == 4) return () => {
                int type = (int)canvas.Selected.P(8);
                if (type < 0 || type > 9)
                    throw new NotSupportedException("This chord type needs the dedicated preset bank.");
                EditOriginalParameters(new[]{8f,(float)((type + (index == 3 ? 9 : 1))%10)});
            };
            if (index >= 5 && index <= 7 && canvas.Selected.P(5)>-4)
                return () => CycleScor4GuitarWord(9,index == 7 ? 100 : 1,index == 7 ? 7 : 14,index == 5 ? -1 : 1);
            if (index >= 11 && index <= 13) return () => {
                float value = canvas.Selected.P(16);
                float size = value%10;
                float font = value-size;
                if (size == 0) size = 1f;
                EditOriginalParameters(new[]{16f,index == 13 ? font : font+size+(index == 11 ? -.1f : .1f)});
            };
            if (index >= 15) return () => {
                if (index == 19) EditOriginalParameters(new[]{19f,0f});
                else EditOriginalParameters(new[]{1900f,new[]{-.5f,.5f,-5f,5f}[index-15]});
            };
            return null;
        }

        Action Scor4GuitarSuffixAction(int index) {
            if (index == 8) return () => SetScor4NoteBank(Scor4NoteBank.GuitarName);
            if (index == 1) return () => CycleScor4GuitarWord(10,1000,4,1);
            if (index == 2 || index == 12)
                return () => CycleScor4GuitarWord(index == 2 ? 10 : 11,100,4,1);
            if (index == 3 || index == 4 || index == 13 || index == 14)
                return () => CycleScor4GuitarWord(index < 10 ? 10 : 11,1,14,index%10 == 3 ? -1 : 1);
            if (index == 5 || index == 15)
                return () => EditOriginalParameters(new[]{index == 5 ? 10f : 11f,0f});
            if (index == 10 || index == 11) return () => {
                int value = (int)canvas.Selected.P(11);
                int letter = value/1000;
                int next = index == 10 ? (letter <= 1 ? 7 : letter-1) : letter%7+1;
                EditOriginalParameters(new[]{11f,value%1000+next*1000f});
            };
            return null;
        }

        void CycleScor4GuitarWord(int parameter,int place,int limit,int direction) {
            float value = canvas.Selected.P(parameter);
            int span = place == 1 ? 100 : place*10;
            int digit = (int)value%span/place;
            if (value < 0 || value != Math.Truncate(value) || digit >= limit)
                throw new NotSupportedException("This packed chord-name control is outside the verified range.");
            int next = (digit+direction+limit)%limit;
            EditOriginalParameters(new[]{(float)parameter,value+(next-digit)*place});
        }

        Action Scor4GuitarStringAction(int index) {
            bool finger = scor4NoteBank == Scor4NoteBank.GuitarFingers;
            int parameter = finger ? 18 : 13;
            if (index <= 6) return () => {
                scor4GuitarAllStrings = index == 6;
                scor4GuitarString = scor4GuitarAllStrings ? 0 : index;
                Message(finger ? "***** Click on Fingering Number *****" : "***** Click on Fret Number *****");
            };
            if (index == 8) return () => {
                scor4GuitarString = -1;
                scor4GuitarAllStrings = false;
                SetScor4NoteBank(Scor4NoteBank.Item);
            };
            if (index == 7) return () => EditOriginalParameters(new[]{(float)parameter,0f});
            if (index < 10 || (finger && (index == 14 || index == 15 || index == 19))) return null;
            return () => {
                if (scor4GuitarString < 0) return;
                int digit = (index - 9) % 10;
                int place = Scor4GuitarStringPlaces[scor4GuitarString];
                float stored = canvas.Selected.P(parameter);
                if (stored < 0 || stored > 999999 || stored != Math.Truncate(stored))
                    throw new NotSupportedException("The packed guitar-string control is outside the original six digits.");
                int previous = (int)stored / place % 10;
                EditOriginalParameters(new[]{(float)parameter,stored + (digit - previous) * place});
                if (scor4GuitarAllStrings && scor4GuitarString < 5) scor4GuitarString++;
                else {
                    scor4GuitarString = -1;
                    scor4GuitarAllStrings = false;
                    Message("");
                }
            };
        }
    }
}
