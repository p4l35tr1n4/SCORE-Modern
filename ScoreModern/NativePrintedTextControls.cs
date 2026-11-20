using System;using System.Collections.Generic;using System.Text;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class ScoreText {
  // Original converter DS:0270 and printer DS:2378 (codes95..161).
  const string PrintedBangDirectory="0123456789ADSZadefghijklmnpqsyz";
  static readonly byte[] PrintedSpecialBytes={5,133,26,154,20,148,183,185,186,161,162,163,167,168,169,170,135,179,155,156,7,178,188,166,171,187,174,172,173,175,208,177,182,191,27,165,28,35,36,37,38,96,6,60,62,64,225,234,232,233,91,92,93,241,29,250,245,248,249,134,251,157,123,124,125,227,235};
  static readonly Dictionary<int,string> PrintedAccentEncoding=new Dictionary<int,string>{{0,"aacute"},{1,"agrave"},{2,"acircumflex"},{3,"adieresis"},{4,"ccedilla"},{5,"atilde"},{6,"minus"},{7,"aring"},{8,"eacute"},{9,"egrave"},{10,"ecircumflex"},{11,"edieresis"},{12,"iacute"},{13,"igrave"},{14,"icircumflex"},{15,"idieresis"},{16,"oacute"},{17,"ograve"},{18,"ocircumflex"},{19,"odieresis"},{20,"otilde"},{21,"uacute"},{22,"ugrave"},{23,"ucircumflex"},{24,"udieresis"},{25,"ydieresis"},{26,"ntilde"},{27,"scaron"},{28,"zcaron"},{29,"copyright"},{128,"Aacute"},{129,"Agrave"},{130,"Acircumflex"},{131,"Adieresis"},{132,"Ccedilla"},{133,"Atilde"},{134,"registered"},{135,"Aring"},{136,"Eacute"},{137,"Egrave"},{138,"Ecircumflex"},{139,"Edieresis"},{140,"Iacute"},{141,"Igrave"},{142,"Icircumflex"},{143,"Idieresis"},{144,"Oacute"},{145,"Ograve"},{146,"Ocircumflex"},{147,"Odieresis"},{148,"Otilde"},{149,"Uacute"},{150,"Ugrave"},{151,"Ucircumflex"},{152,"Udieresis"},{153,"Ydieresis"},{154,"Ntilde"},{155,"Scaron"},{156,"Zcaron"},{157,"trademark"},{242,"onequarter"},{243,"onehalf"},{244,"threequarters"},{252,"underscore"},{245,"onesuperior"},{246,"twosuperior"},{247,"threesuperior"}};
  internal static bool PrintedModifier(int code){return code>=83&&code<=87;}
  internal static bool PrintedMusic(int code){return code>=80&&code<=82||code>=88&&code<=92||code>=162&&code<=167;}
  static int PrintedCharacter(string text,ref int p,short font,ref int accent,out char output,out string glyph){
   char c=text[p];output=c=='\0'?'0':c;glyph=null;int code;
   if(c=='!'&&p+1<text.Length&&PrintedBangDirectory.IndexOf(text[p+1])>=0){code=101+PrintedBangDirectory.IndexOf(text[++p]);}
   else if(c==(char)92&&p+1<text.Length&&text[p+1]==(char)92){p++;return 800;}
   else {code=DatCharacter(text,ref p);if(code==77&&font>=100)code=110;}
   if(PrintedModifier(code)){accent=code;return code;}
   bool encoded=false;
   if(accent!=0&&code>=0&&code<80){
    // Original 34D1:0399 / 350D:0003..0083 selects a base offset,
    // then adds the pending modifier, even for non-vowels.
    int baseCode=code<40?code+30:code,offset=-83;
    switch(baseCode){case 44:offset=-75;break;case 48:offset=-71;break;case 53:case 60:offset=-62;break;case 54:offset=-67;break;default:if(baseCode>=64)offset=-61;break;}
    output=(char)((code<40?128:0)+offset+accent);accent=0;encoded=true;
   }else if(code>=95&&code<=161){output=(char)PrintedSpecialBytes[code-95];encoded=true;}
   else if(code==73)output='/';
   else if(code==77)output='"';
   if(encoded)PrintedAccentEncoding.TryGetValue(output,out glyph);
   return code;
  }
  List<NativeTextToken> PrintedRenderingTokens(NativeTextLayout layout,short word,short prior){
   var original=RenderingTokens(layout,word,prior);if(!original.Exists(t=>t.Code==800))return original;
   var result=new List<NativeTextToken>();var run=new List<NativeTextToken>();bool forced=TextConverterFontState.Begin(word,prior).Forced;
   Action flush=()=>{if(!run.Exists(t=>t.Code==800))result.AddRange(run);else{
    var literal=new StringBuilder();var names=new Dictionary<char,string>();
    foreach(var t in run){if(PrintedModifier(t.Code))continue;if(t.Code==800){literal.Append('\\');continue;}char c=t.Character;
     if(t.GlyphName!=null)names[c]=t.GlyphName;
     if(c<32||c>126)literal.Append('\\').Append(Convert.ToString(c,8).PadLeft(3,'0'));
     else {if(c=='('||c==')'||c=='\\')literal.Append('\\');literal.Append(c);}}
    // The raw-only original fixture also loads FONTINIT and reencodes the face.
    foreach(char c in DecodePrintedLiteral(literal.ToString())){string name;if(!names.TryGetValue(c,out name))PrintedAccentEncoding.TryGetValue(c,out name);if(name==null&&c<32)name=".notdef";name=NativeFontInitialization.Glyph(RootDirectory,c,name);result.Add(new NativeTextToken{Code=0,FontWord=run[0].FontWord,Character=c,GlyphName=name});}
    // Preserve the original binary32 X advance sequence at subsequent run origins.
    foreach(var t in run)result.Add(new NativeTextToken{AdvanceOnly=true,AdjustedAdvance=t.AdjustedAdvance});
   }run.Clear();};
   foreach(var t in original){if(t.TabX.HasValue){flush();result.Add(t);continue;}if(t.Code>=1000){if(!forced){flush();result.Add(t);}continue;}if(PrintedMusic(t.Code)||t.FontWord%100>=90){flush();result.Add(t);}else run.Add(t);}flush();return result;
  }
  static string DecodePrintedLiteral(string literal){
   var value=new StringBuilder();int depth=0;
   for(int n=0;n<literal.Length;n++){char c=literal[n];if(c=='\\'){
     if(++n==literal.Length)throw new NotSupportedException("SCORE's paired backslash leaves an unterminated PostScript text run.");c=literal[n];
     if(c>='0'&&c<='7'){int v=c-'0';for(int k=1;k<3&&n+1<literal.Length&&literal[n+1]>='0'&&literal[n+1]<='7';k++)v=v*8+literal[++n]-'0';c=(char)(v&255);}
     else switch(c){case 'n':c='\n';break;case 'r':c='\r';break;case 't':c='\t';break;case 'b':c='\b';break;case 'f':c='\f';break;}
    }else if(c=='(')depth++;else if(c==')'&&--depth<0)throw new NotSupportedException("SCORE's paired backslash closes the PostScript text run early.");value.Append(c);
   }
   if(depth!=0)throw new NotSupportedException("SCORE's paired backslash leaves unbalanced parentheses in the PostScript text run.");return value.ToString();
  }
 }
}
