using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScoreModern {
public sealed partial class ScoreText {
    sealed class Face {public string Name;public short[] Widths,RightEdges;}
    readonly Dictionary<int,Face> faces=new Dictionary<int,Face>();
    readonly List<int> originalAlignmentMetricFaces=new List<int>();
    readonly object originalMetricHistoryLock=new object();
    int originalMetricNextSlot;bool originalMetricEvicted;
    readonly short?[] originalConverterWords=InitialConverterWords();
    static short?[] InitialConverterWords(){var words=new short?[162];for(int n=0;n<words.Length;n++)words[n]=0;return words;}
    void RecordOriginalMetricFace(int face){if(face<0||face>=90||!faces.ContainsKey(face))return;lock(originalMetricHistoryLock){if(originalAlignmentMetricFaces.Contains(face))return;if(originalAlignmentMetricFaces.Count<10){originalAlignmentMetricFaces.Add(face);originalMetricNextSlot=originalAlignmentMetricFaces.Count%10;}else{originalAlignmentMetricFaces[originalMetricNextSlot]=face;originalMetricNextSlot=(originalMetricNextSlot+1)%10;originalMetricEvicted=true;}}}
    internal void RecordOriginalConverterPass(NativeTextLayout layout){lock(originalMetricHistoryLock){bool tab=false;foreach(var token in layout.Tokens)if(token.TabX.HasValue)tab=true;if(tab){for(int n=1;n<=Math.Min(161,layout.RawCharacterCount);n++)originalConverterWords[n]=null;}else{if(layout.Tokens.Count>161)throw new NotSupportedException("Original text conversion exceeds the verified token buffer.");for(int n=0;n<layout.Tokens.Count;n++)originalConverterWords[n+1]=layout.Tokens[n].Code;}}}
    internal void RecordOriginalGlyphPass(NativeTextLayout layout){RecordOriginalConverterPass(layout);foreach(var token in layout.Tokens)RecordOriginalMetricFace(token.DatMetricFace);}
    internal MetricHistoryScope PreserveAlignmentMetricHistory(){List<int> before;int next;bool evicted;short?[] converterBefore;lock(originalMetricHistoryLock){converterBefore=(short?[])originalConverterWords.Clone();before=new List<int>(originalAlignmentMetricFaces);next=originalMetricNextSlot;evicted=originalMetricEvicted;}return new MetricHistoryScope(()=>{lock(originalMetricHistoryLock){originalAlignmentMetricFaces.Clear();originalAlignmentMetricFaces.AddRange(before);originalMetricNextSlot=next;originalMetricEvicted=evicted;Array.Copy(converterBefore,originalConverterWords,converterBefore.Length);}});}
    internal sealed class MetricHistoryScope:IDisposable {Action restore;internal MetricHistoryScope(Action value){restore=value;}internal void Commit(){restore=null;}public void Dispose(){if(restore!=null){var action=restore;restore=null;action();}}}

    const string Characters="0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,()abcdefghijklmnopqrstuvwxyz:;?!+-*/=`'";
    public string RootDirectory {get;private set;}
    public ScoreText(string root) {
        RootDirectory=Path.GetFullPath(root);
        for(int n=0;n<=89;n++) {
            string path=NativeLibraryLocation.FileFor(root,"FM"+n.ToString("00")+".PSC");if(!File.Exists(path))continue;
            try {var records=FortranRecords.Read(File.ReadAllBytes(path));byte[] b=records[0];if(b.Length<354)continue;var face=new Face{Name=Encoding.ASCII.GetString(b,0,28).Trim(),Widths=new short[163]};for(int j=0;j<163;j++)face.Widths[j]=BitConverter.ToInt16(b,28+j*2);if(records.Count>1&&records[1].Length==326){face.RightEdges=new short[163];for(int j=0;j<163;j++)face.RightEdges[j]=BitConverter.ToInt16(records[1],j*2);}faces[n]=face;}catch(InvalidDataException){}
        }
    }
    public string ExternalFontName(int id){Face face;return faces.TryGetValue(id,out face)?face.Name:null;}
    static float Size(float value){value=Math.Abs(value)%100;return value==0?1:value;}
    int Override(Item item,int font){int n=(int)item.P(8);if(n>=1000)return n%100;if(n>=90)return n%10;return font;}
    public static bool FontCode(string text,int p,out int id){id=0;if(p+2>=text.Length||text[p]!='_'||!Char.IsDigit(text[p+1])||!Char.IsDigit(text[p+2]))return false;id=(text[p+1]-'0')*10+text[p+2]-'0';return true;}
    float Advance(char c,int id) {
        Face face;int index=Characters.IndexOf(c);if(c==' ')index=94;
        if(faces.TryGetValue(id,out face)&&index>=0&&index<face.Widths.Length&&face.Widths[index]>1)return face.Widths[index]/1000f;
        return c==' '?.25f:c=='"'?.408f:c=='&'?.778f:c=='W'?.944f:.5f;
    }
    public float Measure(Item item,string text,float staffSize) {
        var layout=PrepareMetricLayout(item,text,(float)((double)staffSize*25),0);RecordOriginalConverterPass(layout);return layout.Width;
    }
    public void Set(Item item,string text,float staffSize) {Set(item,text,staffSize,160);}
    static string EncodeLegacyAccents(string text) {
        const string letters = "aeiouAEIOU";
        string[] accents = { "áéíóúÁÉÍÓÚ", "àèìòùÀÈÌÒÙ", "âêîôûÂÊÎÔÛ", "äëïöüÄËÏÖÜ" };
        string[] controls = { "<<", ">>", "^^", "%%" };
        var encoded = new StringBuilder();
        foreach (char character in text.Normalize(NormalizationForm.FormC)) {
            if (character == 'ç' || character == 'Ç') {
                encoded.Append(character == 'ç' ? "##c" : "##C");
                continue;
            }
            int tilde = "ãñõÃÑÕ".IndexOf(character);
            if (tilde >= 0) {
                encoded.Append('~').Append("anoANO"[tilde]);
                continue;
            }
            int group = 0;
            int letter = -1;
            while (group < accents.Length && (letter = accents[group].IndexOf(character)) < 0) group++;
            if (letter >= 0) encoded.Append(controls[group]).Append(letters[letter]);
            else encoded.Append(character);
        }
        return encoded.ToString();
    }
    internal void Set(Item item,string text,float staffSize,int maximumBytes) {
        if (item.P(8) != 99) text = EncodeLegacyAccents(text);
        var encoding=Encoding.GetEncoding(437,EncoderFallback.ExceptionFallback,DecoderFallback.ExceptionFallback);byte[] chars=encoding.GetBytes(text);if(chars.Length>maximumBytes)throw new InvalidDataException("SCORE text exceeds the verified "+maximumBytes+"-byte limit for this entry route.");
        int words=13+(chars.Length+3)/4;byte[] bytes=new byte[(words+1)*4];Array.Copy(item.Bytes,bytes,Math.Min(56,item.Bytes.Length));Array.Copy(BitConverter.GetBytes((float)words),bytes,4);Array.Copy(chars,0,bytes,56,chars.Length);for(int padding=56+chars.Length;padding<bytes.Length;padding++)bytes[padding]=32;
        item.Bytes=bytes;item.Set(12,chars.Length);item.Set(13,Measure(item,text,staffSize));
    }
}
}
