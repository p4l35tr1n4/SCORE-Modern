using System;using System.IO;using System.Collections.Generic;using System.Web.Script.Serialization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatBufferChecks {
  public static void Compare(Canvas canvas,string folder){
   var events=(object[])new JavaScriptSerializer{MaxJsonLength=Int32.MaxValue}.DeserializeObject(File.ReadAllText(Path.Combine(folder,"events.json")));var mode=NativeNotationView.Mode;int records=0,coordinates=0,markers=0;
   try{NativeNotationView.Set(NotationView.Dat,false);canvas.LayoutScore();var dat=new NativeDatGlyphs(canvas.Library.Root);var cache=new NativeDatHeadCache();short font=0;
    for(int ordinal=1;ordinal<=canvas.Score.Items.Count;ordinal++){var item=canvas.Score.Items[ordinal-1];Dictionary<string,object> capture=null;foreach(Dictionary<string,object> e in events)if(Convert.ToInt32(e["item"])==ordinal){capture=e;break;}if(capture==null)throw new Exception("Original DAT buffer missing item "+ordinal);
     short next;var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,cache,font,out next);font=next;string stem=((string)capture["ds_file"]).Split('-')[0];var xs=File.ReadAllBytes(Path.Combine(folder,stem+"-x.bin"));var ys=File.ReadAllBytes(Path.Combine(folder,stem+"-y.bin"));int at=1,count=Convert.ToInt32(capture["count"]);
     foreach(var point in points){if(point.CommandOnly||point.Operation>0){if(at>=count||BitConverter.ToInt16(xs,at*2)!=unchecked((short)(-32700+point.Operation)))throw new Exception("Original DAT marker differs "+ordinal+":"+at);at++;markers++;if(point.CommandOnly)continue;}
      if(at>=count||BitConverter.ToInt16(xs,at*2)!=NativeNotationContext.Word(point.X,true)||BitConverter.ToInt16(ys,(at-1)*2)!=NativeNotationContext.Word(point.Y,false))throw new Exception("Original DAT coordinate differs "+ordinal+":"+at);at++;coordinates+=2;}
     if(at!=count)throw new Exception("Original DAT count differs item "+ordinal);records++;
    }
   }finally{NativeNotationView.Set(mode,false);}
   File.WriteAllText(Path.Combine(folder,"native-controller-comparison.txt"),String.Format("PASS {0} complete original buffers, {1} coordinate words, {2} markers.",records,coordinates,markers));
  }
 }
}
