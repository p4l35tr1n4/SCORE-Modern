using System;using System.IO;using System.Linq;using System.Windows.Forms;
namespace ScoreModern {
 public sealed partial class Editor {
  internal void CheckFilenameSequences(string cases){
   unsavedDecision=()=>DialogResult.No;
   try{NativeScoreBehaviour.Set(true,false);int count=0;
    foreach(string row in File.ReadAllLines(cases)){
     string[] p=row.Split('\t');if(p.Length!=6)throw new Exception("Malformed filename fixture.");
     SetDocument(Score.Load(p[1]),p[1]);Execute(p[2]);
     if(!String.Equals(Path.GetFileName(current),p[3],StringComparison.OrdinalIgnoreCase))throw new Exception(p[0]+": loaded "+Path.GetFileName(current)+" instead of "+p[3]);
     if(p[4].Length>0&&!responseLine.Text.Contains(p[4]))throw new Exception(p[0]+": original missing-file diagnostic target differs.");
     var expected=Score.Load(p[5]);if(score.Items.Count!=expected.Items.Count||!score.Items.Zip(expected.Items,(a,b)=>a.Bytes.SequenceEqual(b.Bytes)).All(x=>x))throw new Exception(p[0]+": original loaded item words differ.");
     count++;Console.WriteLine("PASS "+p[0]);
    }
    if(count==0)throw new Exception("No original filename fixtures.");Console.WriteLine("PASS "+count+" original NX/NB filename, diagnostic and complete-record comparisons.");
   }finally{NativeScoreBehaviour.Set(false,false);}
  }
 }
 public static class FilenameSequenceChecks {
  public static int Run(string root,string cases){
   try{
    using(var editor=new Editor(root,Path.Combine(root,"MUSDAT","PRACTICE.MUS"))){editor.CheckFilenameSequences(cases);}
    return 0;
   }catch(Exception error){Console.Error.WriteLine(error);return 1;}
  }
 }
}
