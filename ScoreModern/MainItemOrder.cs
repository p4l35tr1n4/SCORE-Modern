using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ScoreModern {
 public sealed partial class Editor {
  bool ExecuteMainItemOrder(string verb,string arguments){
   switch(verb){
    case "OH":
     RequireNoLetterArguments(verb,arguments);
     ApplyMainOrder(score.Items.OrderBy(item=>item.P(3)).ThenBy(item=>item.Staff),
      "Sorting by horizontal position.",CanTrackDisplayRedraw());
     return true;
    case "OS":
     RequireNoLetterArguments(verb,arguments);
     ApplyMainOrder(score.Items.OrderBy(item=>item.Staff).ThenBy(item=>item.P(3)),
      "*** Sorting by staff number.",CanTrackDisplayRedraw());
     return true;
    case "OT":
     RequireNoLetterArguments(verb,arguments);
     ApplyMainOrder(score.Items.OrderByDescending(item=>item.Staff)
      .ThenBy(item=>item.Code==16?0:1)
      .ThenByDescending(item=>item.Code==16?item.P(4):0)
      .ThenBy(item=>item.Code==16?item.P(3):0),
      "Sorting text (Code 16) only.",CanTrackDisplayRedraw());
     return true;
    case "RNUM":
     var fields=arguments.Split(new[]{' ',',','\t'},StringSplitOptions.RemoveEmptyEntries);
     int from,to;
     if(fields.Length!=2||!Int32.TryParse(fields[0],NumberStyles.Integer,CultureInfo.InvariantCulture,out from)||
        !Int32.TryParse(fields[1],NumberStyles.Integer,CultureInfo.InvariantCulture,out to)||
        from<1||from>score.Items.Count||to<1||to>score.Items.Count)
      throw new InvalidDataException("RNUM requires two existing item numbers.");
     var order=score.Items.ToList();
     var movedItem=order[from-1];
     order.RemoveAt(from-1);
     order.Insert(to-1,movedItem);
     // RNUM draws the selected item at the scratch end and moves the
     // intervening display words with the record order.
     bool tracked=from!=to&&(NativeRetainedDisplayArray.SupportsNotePreview(movedItem)||
      movedItem.Code>=2&&movedItem.Code<=18&&(movedItem.Code!=15||TrackedBox(movedItem)))
      &&PrepareTrackedItemReorder(from,to);
     ApplyMainOrder(order,"");
     if(tracked)RecordUnchangedTrackedDisplay();
     return true;
    default:return false;
   }
  }

  void ApplyMainOrder(IEnumerable<Item> items,string message,bool rebuildDisplay=false){
   var order=items.ToList();
   if(!order.SequenceEqual(score.Items)){
    using(var storage=BeginParameterMutation()){
     Checkpoint();score.Items=order;dirty=true;RefreshAll(null);storage.Complete();
    }
    if(rebuildDisplay)RebuildTrackedDisplay();
   }
   Message(message);
  }

  void CheckMainItemOrder(){
   var previous=score;
   string previousFile=current;
   try{
    var fixture=NativeMeasurements.New(source);
    fixture.Items=new List<Item>{
     Item.Make(8,1,0,0,1,200),Item.Make(8,2,0,40,1,200),Item.Make(1,2,80,5,10,0,1),
     Item.Make(1,1,60,7,10,0,1),Item.Make(2,2,20),Item.Make(1,1,20,5,10,0,1),
     Item.Make(1,2,20,7,10,0,1),Item.Make(2,1,20)
    };
    var original=fixture.Items.ToArray();
    SetDocument(fixture,"");
    Execute("OH");
    CheckOrder(original,new[]{0,1,5,7,4,6,3,2},"OH");
    Execute("OS");
    CheckOrder(original,new[]{0,5,7,3,1,4,6,2},"OS");
    Execute("OT");
    CheckOrder(original,new[]{1,4,6,2,0,5,7,3},"OT without text");
    Execute("RNUM 3 7");
    CheckOrder(original,new[]{1,4,2,0,5,7,6,3},"RNUM forward");
    Execute("RNUM 7 3");
    CheckOrder(original,new[]{1,4,6,2,0,5,7,3},"RNUM backward");
    Undo(false);
    CheckOrder(original,new[]{1,4,2,0,5,7,6,3},"RNUM undo");
    var word=previous.Items.First(item=>item.Code==16);
    fixture=NativeMeasurements.New(source);
    fixture.Items=original.ToList();
    foreach(var position in new[]{new[]{1,1,70},new[]{3,2,30},new[]{6,2,10},new[]{9,1,30}}){
     var text=word.Clone();
     text.Set(2,position[1]);
     text.Set(3,position[2]);
     fixture.Items.Insert(position[0],text);
    }
    original=fixture.Items.ToArray();
    SetDocument(fixture,"");
    Execute("OT");
    CheckOrder(original,new[]{6,3,2,4,7,10,9,1,0,5,8,11},"OT with text");
    Console.WriteLine("PASS: OH, OS, OT and RNUM order.");
   }finally{SetDocument(previous,previousFile);}
  }

  void CheckOrder(Item[] original,int[] expected,string commandName){
   if(!score.Items.Select(item=>Convert.ToBase64String(item.Bytes))
      .SequenceEqual(expected.Select(index=>Convert.ToBase64String(original[index].Bytes))))
    throw new Exception(commandName+" differs from the original item order.");
  }
 }
}
