using System;
namespace ScoreModern{
 public sealed partial class ScoreText{
  // Original relative020F converter: ASCII lookup, repeated controls, and its
  // two look-ahead directories. Glyph coordinates/metrics remain external.
  static int DatCharacter(string text,ref int p){char c=text[p];if(c=='\0')return 0;int code=Characters.IndexOf(c);if(c==' ')code=94;else if(c=='[')code=88;else switch(c){case ']':code=89;break;case '"':code=78;break;case '#':code=132;break;case '$':code=133;break;case '%':code=134;break;case '&':code=135;break;case '<':code=138;break;case '>':code=139;break;case '@':code=140;break;case '\\':code=73;break;case '^':code=85;break;case '{':code=90;break;case '|':code=92;break;case '}':code=91;break;case '~':code=126;break;}
   if(p+1<text.Length&&text[p+1]==c){int replacement=code==78?77:code==132?87:code==134?86:code==138?83:code==139?84:-1;if(replacement>=0){p++;return replacement;}if(code==85||code==86){p++;return code;}}
   if(p+1<text.Length&&code==68){string keys="-01239AELO[\\]acdeilorst{|}fm8";int[] values={137,163,80,81,82,164,141,142,143,144,145,146,147,148,149,162,150,151,152,153,154,155,156,157,158,159,160,161,166};int at=keys.IndexOf(text[p+1]);if(at>=0){p++;return values[at];}}
   if(p+1<text.Length&&code==126){int at="aAnNoO".IndexOf(text[p+1]);if(at>=0){p++;return 95+at;}}
   return code;
  }
 }
}
