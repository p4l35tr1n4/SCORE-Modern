using System;
using System.Globalization;
using System.IO;
namespace ScoreModern.RecoveredFragments {
 public static class InputRhythmToken {
  // Candidate transcription of the section 67 rhythm-token leaf. Slash repetition and list controls
  // belong to the caller; this method accepts one complete duration expression.
  public static float Parse(string text){return Parse(text,false);}
  public static float Parse(string text,bool originalMantissaWrap){
   if(text==null)throw new ArgumentNullException("text");text=text.Trim().ToUpperInvariant();if(text.Length==0)throw new InvalidDataException("Missing rhythm.");
   int at=0;float sum=0,packedDots=0;
   while(at<text.Length){
    float duration;bool triplet=false;int start=at;
    if(text[at]=='T'){triplet=true;at++;if(at==text.Length||text[at]=='.'){duration=(float)(4.0/12.0);goto dotted;}}
    string letters="GSEQHWD";float[] denominators={.0625f,16,8,4,2,1,.5f};int letter=letters.IndexOf(text[at]);if(triplet&&text[at]=='T'){at++;duration=(float)(4.0/48.0);goto dotted;}
    if(letter>=0){at++;float denominator=denominators[letter];if(triplet)denominator=(float)((double)denominator*1.5);duration=(float)(4.0/denominator);}
    else{if(triplet)throw new InvalidDataException("A triplet prefix requires a rhythm letter.");float first=Number(text,ref at,originalMantissaWrap);if(at<text.Length&&(text[at]==':'||text[at]=='\'')){at++;float second=Number(text,ref at,originalMantissaWrap);if(first==0)throw new InvalidDataException("Zero rhythm ratio divisor.");duration=(float)((double)second/first);}else{duration=first==0?0:(float)(4.0/first);}}
    dotted: int dots=0;while(at<text.Length&&text[at]=='.'){dots++;at++;}
    float extra=duration;for(int n=0;n<dots;n++){extra=(float)(.5*(double)extra);duration=(float)((double)duration+extra);}
    if(sum==0)packedDots=dots*1000;else packedDots=0;sum=(float)((double)sum+duration);
    if(at==start||float.IsNaN(sum)||float.IsInfinity(sum))throw new InvalidDataException("Invalid rhythm.");
   }
   return (float)((double)sum+packedDots);
  }
  // Positive finite branch of the original resident real reader16D3:05EF..0799:
  // accumulate signed32 digits, store the mantissa and power-of-ten as
  // binary32, then divide. No host float-parser substitution is used.
  static float Number(string text,ref int at,bool originalMantissaWrap){
   int start=at,digits=0,decimals=0,mantissa=0;
   // The rhythm caller consumes a standalone 12-digit token as zero
   // when the original real reader rejects its length.
   if(at==0&&text.Length==12&&Array.TrueForAll(text.ToCharArray(),c=>c>='0'&&c<='9')){
    at=text.Length;
    return 0;
   }
   bool point=false;
   while(at<text.Length){
    char c=text[at];
    if(c>='0'&&c<='9'){
     long next=(long)mantissa*10+c-'0';
     if(!originalMantissaWrap&&next>Int32.MaxValue)
      throw new NotSupportedException("Rhythm decimal mantissa leaves the recovered positive signed32 branch.");
     mantissa=unchecked((int)next);
     digits++;
     if(point)decimals++;
     at++;
    }else if(c=='.'&&!point&&at+1<text.Length&&text[at+1]>='0'&&text[at+1]<='9'){
     point=true;
     at++;
    }else break;
    if(at-start>=12)throw new NotSupportedException("Rhythm number reaches the original real-reader length boundary.");
   }
   if(digits==0||mantissa<0)throw new InvalidDataException("Invalid rhythm token.");
   float storedMantissa=mantissa,storedPower=(float)Math.Pow(10,decimals);
   return(float)((double)storedMantissa/storedPower);
  }

 }
}
