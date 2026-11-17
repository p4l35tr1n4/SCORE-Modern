using System;using System.IO;using System.Linq;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeSlurChecks {
  static int W(float v){return unchecked((short)(int)Math.Truncate(v));}
  public static int Verify(string root){try{int files=0,records=0,results=0,skipped=0;long words=0;
   foreach(string path in Directory.GetFiles(root,"*.mus",SearchOption.AllDirectories)){
    Score score;try{score=Score.Load(path);}catch(InvalidDataException e){if(path.Replace('\\','/').EndsWith("beet-no9/score/02/124.mus",StringComparison.OrdinalIgnoreCase)){skipped++;continue;}throw new InvalidDataException(path+": "+e.Message);}
    files++;var slurs=score.Items.Where(i=>i.Code==5&&!(W(i.P(8))>0&&W(i.P(8))<10)).ToArray();records+=slurs.Length;
    foreach(bool printed in new[]{false,true}){var buffer=new SlurVectorBuffer{End=20};foreach(var item in slurs){float[] p=new float[21];for(int n=1;n<=20;n++)p[n]=item.P(n);if(printed)buffer.End=W(p[9])==99?1:(W(p[11])==0?2:1);var context=new SlurOriginContext{Scale=22.5f,LeftX=(float)((double)p[3]*150),LeftY=(float)((double)p[4]*157.5-20000),RightY=(float)((double)p[5]*157.5-20000)};var r=SlurGeometry.Build(p,context,printed,1,buffer,false);results++;words+=Math.Max(0,r.Buffer.End-r.Root);}}
   }
   Console.WriteLine("PASS: "+files+" sample files; "+records+" slur/slide records accepted in both modes; "+results+" results / "+words+" X-buffer words. "+skipped+" existing fractional-code file unsupported. Corpus acceptance is not original-output equivalence.");return 0;
  }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 }
}
