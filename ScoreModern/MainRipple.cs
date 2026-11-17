using System;using System.Linq;using System.Collections.Generic;using System.Globalization;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public sealed partial class Editor {
  Score rippleWorking;short rippleStaves,rippleSource,rippleDestination,rippleBars;int rippleStage;List<float> rippleInput=new List<float>();
  void SubmitMainRipple(string text){if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){ClearMainGroup();rippleWorking=null;Message("");return;}var values=LayoutNumbers(text);if(values.Length>20)throw new NotSupportedException("Original RIP argument capacity exceeded.");if(rippleInput.Count==0&&values.Length>=4&&TransposeWord(values[3].ToString("R",CultureInfo.InvariantCulture))!=0){rippleInput.AddRange(values.Take(4));}else if(values.Length>0)rippleInput.Add(values[0]);else rippleInput.Add(0);
   if(rippleInput.Count<4){string[] prompts={"How many staves in a system? (B=backup) ->","Type staff number of bottom of source system. ->","Type staff number of bottom of destination system. ->","How many bars to move? ->"};Message(prompts[rippleInput.Count]);return;}
   Func<int,short> word=i=>TransposeWord(rippleInput[i].ToString("R",CultureInfo.InvariantCulture));rippleStaves=word(0);rippleSource=word(1);rippleDestination=word(2);rippleBars=word(3);var rows=score.Items.Select(TransposeRow).ToList();int[] order=InputTranspositionOrder.OrderForMain(rows,false);var words=new float[16384];var starts=new int[order.Length];int at=0;foreach(int index in order){var item=score.Items[index];int slot=Array.IndexOf(order,index);starts[slot]=at+1;if(at+item.Bytes.Length/4>words.Length)throw new NotSupportedException("Original RIP source word capacity exceeded.");Buffer.BlockCopy(item.Bytes,0,words,at*4,item.Bytes.Length);at+=item.Bytes.Length/4;}if(at+score.Trailer.Length/4<=words.Length)Buffer.BlockCopy(score.Trailer,0,words,at*4,score.Trailer.Length);var result=LayoutRipple.Prepare(words,starts,rippleStaves,rippleSource,rippleDestination,rippleBars);rippleWorking=score.Clone();rippleWorking.Items=new List<Item>();foreach(int w in result.Starts){int n=(int)Math.Truncate(words[w-1]);byte[] bytes=new byte[(n+1)*4];Buffer.BlockCopy(words,(w-1)*4,bytes,0,bytes.Length);rippleWorking.Items.Add(new Item(bytes));}rippleStage=0;if(!result.Applied){FinishMainRipple();return;}StartRippleLineup();
  }
  void StartRippleLineup(){short bottom=rippleStage==0?rippleSource:rippleDestination;var scales=new PageCoordinates();foreach(var item in rippleWorking.Items.Where(item=>item.Code==8)){var p=new float[Math.Max(21,item.Count+1)];for(int j=1;j<=item.Count;j++)p[j]=item.P(j);scales.SetStaff(p,NativeMeasurements.Get(rippleWorking)==1);}layoutScales=scales.Scales;layoutTranscript=new List<string>();layoutPreparation=new LayoutParentPreparation(rippleWorking.Items.Select(TransposeRow).ToList(),bottom,(short)(bottom+rippleStaves-1),0,200,false,true);ContinueMainJustification();}
  void ContinueRippleLineup(List<Item> next){
   rippleWorking.Items=next;rippleStage++;if(rippleStage<2){StartRippleLineup();return;}
   int low=Math.Min(rippleSource,rippleDestination),high=Math.Max(rippleSource,rippleDestination);
   // 2C0C tail compares P1 with the literal 3: these are clefs, not staves.
   for(int k=0;k<rippleStaves;k++){
    var a=rippleWorking.Items.FirstOrDefault(item=>item.Staff==low+k&&item.Code==3);
    var b=rippleWorking.Items.LastOrDefault(item=>item.Staff==high+k&&item.Code==3);
    if(a==null||b==null)continue;float first=a.Count>=5?a.P(5):0,second=b.Count>=5?b.P(5):0;
    if(BitConverter.ToInt32(BitConverter.GetBytes(first),0)!=BitConverter.ToInt32(BitConverter.GetBytes(second),0)){
     if(a.Count<5)a.Set(a.Count+2,0);a.Set(4,0);a.Set(5,second);
    }
   }
   FinishMainRipple();
  }
  void FinishMainRipple(){
   bool trackDisplay=rippleWorking.Items.Count==score.Items.Count&&CanTrackDisplayRedraw();
   if(rippleStage<2){
    // Even when the source and destination coincide, RIP sorts the item
    // table and marks the score changed. Its display banks are left intact.
    Checkpoint();score.Items=rippleWorking.Items;originalLayoutMarksModified|=InputTranspositionOrder.MarksModified(score.Items.Count,false);
    dirty=true;rippleWorking=null;ClearMainGroup();
    if(trackDisplay&&rippleSource==rippleDestination)RecordUnchangedTrackedDisplay();
    RefreshAll(null);
    Message("");return;
   }
   var restOrdinals=new List<int>();
   for(int staff=Math.Min(rippleSource,rippleDestination);staff<=Math.Max(rippleSource,rippleDestination)+rippleStaves-1;staff++)
   {
    var centered=NativeRestCentering.Plan(rippleWorking,staff);
    NativeRestCentering.Apply(centered);
    if(trackDisplay)restOrdinals.AddRange(centered.Select(change=>rippleWorking.Items.IndexOf(change.Item)+1));
   }
   Checkpoint();score.Items=rippleWorking.Items;originalLayoutMarksModified|=InputTranspositionOrder.MarksModified(score.Items.Count,false);
   dirty=true;rippleWorking=null;ClearMainGroup();RefreshTextCaches();RefreshTransposeTextTerminators();RefreshAll(null);
   // RIP replaces each centered rest using the earlier display pointer
   // order, then redraws the full score.
   if(trackDisplay)ReplayTrackedDisplayReplacements(restOrdinals,2,true);
   Message("");
  }
 }
}
