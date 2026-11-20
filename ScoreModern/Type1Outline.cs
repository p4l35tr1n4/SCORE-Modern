using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ScoreModern {
 // Restricted external Type-1 reader. Executes charstring drawing operators only;
 // PostScript programs are never executed. Unsupported operators fail explicitly.
 public sealed class Type1Outline {
  static readonly Encoding Latin=Encoding.GetEncoding(28591);
  readonly Dictionary<int,byte[]> subrs=new Dictionary<int,byte[]>();
  readonly Dictionary<string,byte[]> glyphs=new Dictionary<string,byte[]>();
  readonly Dictionary<int,string> characterNames=new Dictionary<int,string>();
  double[] fontMatrix=new double[6];
  int lenIV=4;public string FontName{get;private set;}
  static bool Equal(byte[] a,byte[] b){if(a.Length!=b.Length)return false;for(int j=0;j<a.Length;j++)if(a[j]!=b[j])return false;return true;}
  static byte[] Decrypt(byte[] b,int seed,int skip){if(skip<0)return b;byte[] r=new byte[Math.Max(0,b.Length-skip)];for(int i=0;i<b.Length;i++){byte c=b[i],v=(byte)(c^(seed>>8));seed=((c+seed)*52845+22719)&65535;if(i>=skip)r[i-skip]=v;}return r;}
  public Type1Outline(string path){
   byte[] b=File.ReadAllBytes(path);if(b.Length>8000000)throw new InvalidDataException("Type-1 file too large.");
   var ascii=new MemoryStream();var encrypted=new MemoryStream();int p=0;
   if(b.Length>0&&b[0]==128){
    while(p<b.Length){if(b[p++]!=128||p>=b.Length)throw new InvalidDataException("Invalid PFB record.");int t=b[p++];if(t==3)break;if((t!=1&&t!=2)||p+4>b.Length)throw new InvalidDataException("Invalid PFB type.");int n=BitConverter.ToInt32(b,p);p+=4;if(n<0||n>b.Length-p)throw new InvalidDataException("Invalid PFB length.");if(t==1)ascii.Write(b,p,n);else encrypted.Write(b,p,n);p+=n;}
   }else{
    // Adobe Type1 Font Format chapters1/7: PFA transports the same eexec
    // cipher bytes in hexadecimal, followed by512ASCII zeros/cleartomark.
    // This only decodes a container; no PostScript program is executed.
    string program=Latin.GetString(b);var boundary=Regex.Match(program,@"(?<!\S)currentfile\s+eexec(?=\s)");
    if(!program.StartsWith("%!")||!boundary.Success)throw new InvalidDataException("Invalid Type1 PFA header.");
    int endHeader=boundary.Index+boundary.Length;var trailer=Regex.Match(program.Substring(endHeader),@"(?:[ \t\r\n]*0){512}[ \t\r\n]*cleartomark\b");
    if(!trailer.Success)throw new InvalidDataException("PFA needs its512-zero eexec trailer.");
    string hex=Regex.Replace(program.Substring(endHeader,trailer.Index),@"[ \t\r\n]","");
    if(hex.Length<8||(hex.Length&1)!=0||!Regex.IsMatch(hex,@"\A[0-9A-Fa-f]+\z"))throw new InvalidDataException("Only hexadecimal PFA eexec data is supported.");
    ascii.Write(b,0,endHeader);for(int n=0;n<hex.Length;n+=2)encrypted.WriteByte(Convert.ToByte(hex.Substring(n,2),16));
   }
   string header=Latin.GetString(ascii.ToArray());var name=Regex.Match(header,@"/FontName\s+/([^\s]+)");if(!name.Success)throw new InvalidDataException("Missing Type-1 FontName.");FontName=name.Groups[1].Value;
   // FontMatrix is supplied by the external font. The provided Base-35
   // faces include both oblique shear and narrow horizontal scaling.
   var matrix=Regex.Match(header,@"/FontMatrix\s*\[([^\]]+)\]");if(!matrix.Success)throw new InvalidDataException("Missing Type-1 FontMatrix.");string[] terms=matrix.Groups[1].Value.Split((char[])null,StringSplitOptions.RemoveEmptyEntries);if(terms.Length!=6)throw new InvalidDataException("Invalid Type-1 FontMatrix.");for(int j=0;j<6;j++)fontMatrix[j]=double.Parse(terms[j],System.Globalization.CultureInfo.InvariantCulture);if(fontMatrix[1]!=0)throw new InvalidDataException("Vertical Type-1 advances are not supported.");
   byte[] clear=Decrypt(encrypted.ToArray(),55665,4);string text=Latin.GetString(clear);var iv=Regex.Match(text,@"/lenIV\s+(-?\d+)");if(iv.Success)lenIV=int.Parse(iv.Groups[1].Value);if(lenIV < -1 || lenIV>64)throw new InvalidDataException("Unsupported lenIV.");
   int start=text.IndexOf("/Subrs",StringComparison.Ordinal),end=text.IndexOf("/CharStrings",StringComparison.Ordinal);if(start<0||end<start)throw new InvalidDataException("Missing charstring dictionaries.");
   var subPattern=new Regex(@"dup\s+(\d+)\s+(\d+)\s+(?:RD|-\|)\s");int cursor=start;
   while(cursor<end){var m=subPattern.Match(text,cursor);if(!m.Success||m.Index>=end)break;int n=int.Parse(m.Groups[2].Value),at=m.Index+m.Length;if(n<0||at+n>clear.Length)throw new InvalidDataException("Bad subroutine length.");byte[] data=new byte[n];Array.Copy(clear,at,data,0,n);int index=int.Parse(m.Groups[1].Value);byte[] plain=Decrypt(data,4330,lenIV),prior;if(subrs.TryGetValue(index,out prior)&&!Equal(prior,plain))throw new InvalidDataException("Conflicting nested Type-1 subroutines.");subrs[index]=plain;cursor=at+n;}
   var glyphPattern=new Regex(@"/([^\s/]+)\s+(\d+)\s+(?:RD|-\|)\s");cursor=end;
   while(cursor<clear.Length){var m=glyphPattern.Match(text,cursor);if(!m.Success)break;int n=int.Parse(m.Groups[2].Value),at=m.Index+m.Length;if(n<0||at+n>clear.Length)throw new InvalidDataException("Bad glyph length.");byte[] data=new byte[n];Array.Copy(clear,at,data,0,n);glyphs.Add(m.Groups[1].Value,Decrypt(data,4330,lenIV));cursor=at+n;}
   string metrics=System.IO.Path.ChangeExtension(path,".afm");
   if(File.Exists(metrics)){string afm=File.ReadAllText(metrics,Latin);var afmName=Regex.Match(afm,@"(?m)^FontName\s+(\S+)");if(!afmName.Success||afmName.Groups[1].Value!=FontName)throw new InvalidDataException("External AFM/Type1 FontName mismatch.");foreach(Match entry in Regex.Matches(afm,@"(?m)^C\s+(\d+)\s*;[^\r\n]*?\bN\s+(\S+)\s*;")){int code=int.Parse(entry.Groups[1].Value);if(code<=255)characterNames[code]=entry.Groups[2].Value;}}
  }
  public GraphicsPath NamedCharacter(string name,bool outline,out float advance){return Glyph(name,out advance,outline);}
  public GraphicsPath Digit(int digit,out float advance){if(digit<0||digit>9)throw new ArgumentOutOfRangeException("digit");string[] names={"zero","one","two","three","four","five","six","seven","eight","nine"};return Glyph(names[digit],out advance);}
  public GraphicsPath Character(char character,out float advance){string name;if(characterNames.TryGetValue(character,out name))return Glyph(name,out advance);if(character>='0'&&character<='9')return Digit(character-'0',out advance);if(!characterNames.TryGetValue(character,out name))throw new InvalidDataException("External AFM has no character mapping for "+(int)character);return Glyph(name,out advance);}
  public GraphicsPath OutlineCharacter(char character,out float advance){string name;if(!characterNames.TryGetValue(character,out name))throw new InvalidDataException("External AFM has no character mapping for "+(int)character);return Glyph(name,out advance,true);}
  GraphicsPath Glyph(string name,out float advance){return Glyph(name,out advance,false);}
  GraphicsPath Glyph(string name,out float advance,bool outline){byte[] b;if(!glyphs.TryGetValue(name,out b))throw new InvalidDataException("Missing Type-1 glyph: "+name);var vm=new CharString(subrs,outline,glyphs,0);try{vm.Run(b,0);advance=(float)(vm.Width*fontMatrix[0]*1000);if(vm.Path.PointCount>0)using(var matrix=new Matrix((float)(fontMatrix[0]*1000),(float)(fontMatrix[1]*1000),(float)(fontMatrix[2]*1000),(float)(fontMatrix[3]*1000),(float)(fontMatrix[4]*1000),(float)(fontMatrix[5]*1000)))vm.Path.Transform(matrix);return vm.Path;}catch{vm.Path.Dispose();throw;}}
  sealed class CharString {
   readonly Dictionary<string,byte[]> glyphs;readonly int compositeDepth;readonly Dictionary<int,byte[]> subrs;readonly List<double> stack=new List<double>(),post=new List<double>();readonly List<PointF> flex=new List<PointF>();
   public readonly GraphicsPath Path=new GraphicsPath(FillMode.Winding);public double Width;double sideBearing,x,y;bool figure,flexing;int steps;readonly bool outline;PointF start;
   public CharString(Dictionary<int,byte[]> s,bool unfilled,Dictionary<string,byte[]> characters,int depth){subrs=s;outline=unfilled;glyphs=characters;compositeDepth=depth;}
   static string StandardCompositeName(int code){if(code>=65&&code<=90||code>=97&&code<=122)return ((char)code).ToString();if(code==245)return "dotlessi";switch(code){case 193:return "grave";case 194:return "acute";case 195:return "circumflex";case 196:return "tilde";case 197:return "macron";case 198:return "breve";case 199:return "dotaccent";case 200:return "dieresis";case 202:return "ring";case 203:return "cedilla";case 205:return "hungarumlaut";case 206:return "ogonek";case 207:return "caron";}throw new InvalidDataException("Unverified Type-1 composite StandardEncoding code: "+code);}
   double Pop(){if(stack.Count==0)throw new InvalidDataException("Charstring stack underflow.");double v=stack[stack.Count-1];stack.RemoveAt(stack.Count-1);return v;}
   void Count(int n){if(stack.Count!=n)throw new InvalidDataException("Invalid charstring operands.");}
   void Move(double dx,double dy){if(!flexing&&figure)CloseContour();x+=dx;y+=dy;if(!flexing){Path.StartFigure();figure=false;start=new PointF((float)x,(float)y);}}
   void CloseContour(){if(figure){if(outline){var last=new PointF((float)x,(float)y);if(last!=start)Path.AddLine(last,start);}else Path.CloseFigure();}figure=false;}
   void Line(double dx,double dy){double nx=x+dx,ny=y+dy;Path.AddLine((float)x,(float)y,(float)nx,(float)ny);x=nx;y=ny;figure=true;}
   void Curve(double a,double b,double c,double d,double e,double f){double x1=x+a,y1=y+b,x2=x1+c,y2=y1+d,nx=x2+e,ny=y2+f;Path.AddBezier((float)x,(float)y,(float)x1,(float)y1,(float)x2,(float)y2,(float)nx,(float)ny);x=nx;y=ny;figure=true;}
   public void Run(byte[] data,int depth){if(depth>20)throw new InvalidDataException("Charstring recursion limit.");for(int p=0;p<data.Length;){if(++steps>100000)throw new InvalidDataException("Charstring instruction limit.");int op=data[p++];if(op>=32){double v;if(op<=246)v=op-139;else if(op<=250){if(p>=data.Length)throw new InvalidDataException();v=(op-247)*256+data[p++]+108;}else if(op<=254){if(p>=data.Length)throw new InvalidDataException();v=-(op-251)*256-data[p++]-108;}else{if(p+4>data.Length)throw new InvalidDataException();v=unchecked((int)((uint)data[p]<<24|(uint)data[p+1]<<16|(uint)data[p+2]<<8|data[p+3]));p+=4;}if(stack.Count>=48)throw new InvalidDataException("Charstring operand limit.");stack.Add(v);continue;}
    if(op==10){int n=(int)Pop();byte[] sub;if(!subrs.TryGetValue(n,out sub))throw new InvalidDataException("Missing charstring subroutine.");Run(sub,depth+1);continue;}if(op==11)return;
    if(op==12){if(p>=data.Length)throw new InvalidDataException();int esc=data[p++];if(esc==6){
      Count(5);if(compositeDepth>=20)throw new InvalidDataException("Type-1 composite recursion limit.");
      string baseName=StandardCompositeName((int)stack[3]),accentName=StandardCompositeName((int)stack[4]);byte[] baseBytes,accentBytes;
      if(!glyphs.TryGetValue(baseName,out baseBytes)||!glyphs.TryGetValue(accentName,out accentBytes))throw new InvalidDataException("Missing Type-1 composite component.");
      var basis=new CharString(subrs,outline,glyphs,compositeDepth+1);var accent=new CharString(subrs,outline,glyphs,compositeDepth+1);
      try{basis.Run(baseBytes,0);accent.Run(accentBytes,0);if(basis.Path.PointCount>0)Path.AddPath(basis.Path,false);if(accent.Path.PointCount>0){using(var matrix=new Matrix()){matrix.Translate((float)(sideBearing+stack[1]-stack[0]),(float)stack[2]);accent.Path.Transform(matrix);}Path.AddPath(accent.Path,false);}}finally{basis.Path.Dispose();accent.Path.Dispose();}stack.Clear();return;
     }if(esc==12){double b=Pop(),a=Pop();stack.Add(a/b);continue;}if(esc==17){if(post.Count==0)throw new InvalidDataException("Empty OtherSubr result.");stack.Add(post[post.Count-1]);post.RemoveAt(post.Count-1);continue;}
     if(esc==16){int n=(int)Pop(),argc=(int)Pop();if(argc<0||argc>stack.Count)throw new InvalidDataException("Invalid OtherSubr arguments.");double[] args=stack.GetRange(stack.Count-argc,argc).ToArray();stack.RemoveRange(stack.Count-argc,argc);
      if(n==1&&argc==0){flexing=true;flex.Clear();flex.Add(new PointF((float)x,(float)y));}
      else if(n==2&&argc==0){flex.Add(new PointF((float)x,(float)y));}
      else if(n==0&&argc==3){if(!flexing||flex.Count!=8)throw new InvalidDataException("Unsupported flex sequence.");var a=flex[0];Path.AddBezier(a,flex[2],flex[3],flex[4]);Path.AddBezier(flex[4],flex[5],flex[6],flex[7]);flexing=false;figure=true;post.Add(y);post.Add(x);}
      else if(n==3&&argc==1)post.Add(args[0]);else throw new InvalidDataException("Unsupported Type-1 OtherSubr "+n);continue;}
     if(esc==33){Count(2);x=stack[0];y=stack[1];stack.Clear();continue;}if(esc==0||esc==1||esc==2){stack.Clear();continue;}throw new InvalidDataException("Unsupported Type-1 escape "+esc);
    }
    if(op==1||op==3){if(stack.Count%2!=0)throw new InvalidDataException();stack.Clear();continue;}
    switch(op){case 4:Count(1);Move(0,stack[0]);break;case 5:Count(2);Line(stack[0],stack[1]);break;case 6:Count(1);Line(stack[0],0);break;case 7:Count(1);Line(0,stack[0]);break;case 8:Count(6);Curve(stack[0],stack[1],stack[2],stack[3],stack[4],stack[5]);break;case 9:Count(0);CloseContour();break;case 13:Count(2);sideBearing=x=stack[0];y=0;Width=stack[1];break;case 14:Count(0);if(figure||(outline&&Path.PointCount>0))Path.CloseFigure();return;case 21:Count(2);Move(stack[0],stack[1]);break;case 22:Count(1);Move(stack[0],0);break;case 30:Count(4);Curve(0,stack[0],stack[1],stack[2],stack[3],0);break;case 31:Count(4);Curve(stack[0],0,stack[1],stack[2],0,stack[3]);break;default:throw new InvalidDataException("Unsupported Type-1 operator "+op);}
    stack.Clear();
   }}
  }
 }
}
