using System;using System.IO;
namespace ScoreModern {
 internal static class OriginalFilenameSequence {
  // Resident 16D3:0941-0A5D; caller 2E61:0495-05BB. The second
  // control byte is deliberately retained for NX and explicitly set for NB.
  internal static string Step(string stem,string extension,bool forward,bool reset,char? control=null){
   if(stem.Length==0||stem.Length>21||extension.Length!=4)throw new NotSupportedException("Original filename stepping requires a 1-21 character stem and a three-character extension.");
   char[] name=stem.ToCharArray(),suffix=extension.ToCharArray();int cursor=name.Length-1,direction=1;
   char second=control??(forward?(stem+extension)[1]:(reset?',':'.'));
   Func<int,char> read=i=>i<0?suffix[suffix.Length+i]:name[i];
   Action<int,char> write=(i,c)=>{if(i<0)suffix[suffix.Length+i]=c;else name[i]=c;};
   if(reset){char c=read(cursor);write(cursor,c>='0'&&c<='9'?'0':Char.IsLower(c)?'a':'A');if(second==',')return new string(name)+new string(suffix);cursor--;}
   else if(second=='.')direction=-1;
   while(true){
    if(cursor< -suffix.Length)throw new NotSupportedException("Original filename carry would leave the verified extension/stem buffer.");
    char c=read(cursor);bool lower=c>='a'&&c<='z',letter=lower||c>='A'&&c<='Z';char lo=letter?(lower?'a':'A'):'0',hi=letter?(lower?'z':'Z'):'9';
    int changed=(byte)(c+direction);
    if(direction>0?changed<=hi:changed>=lo){write(cursor,(char)changed);break;}
    write(cursor,direction>0?lo:hi);if(cursor==0)break;cursor--;
   }
   return new string(name)+new string(suffix);
  }
  internal static string Target(string folder,string stem,string extension,bool forward){
   string primary=Step(stem,extension,forward,false),path=Path.Combine(folder,primary);
   if(File.Exists(path))return path;
   // The first call mutates the common filename before the second probe.
   string next=Step(Path.GetFileNameWithoutExtension(primary),Path.GetExtension(primary),forward,true);
   return Path.Combine(folder,next);
  }
 }
}
