using System;using System.Collections.Generic;using System.Linq;
namespace ScoreModern {
 // Original PAGE extraction stage, before bar assembly/page layout. The first
 // measured four-system part preserves every non-staff record word except
 // staff IDs and bar spans, and performs a stable horizontal sort.
 public static class NativePageParts {
  public sealed class Segment {public int SourceStaff;public Score Score;}
  internal sealed class MissingInstrumentOpening {
   internal readonly float Clef;
   internal readonly short? Key, MeterTop, MeterBottom;
   internal MissingInstrumentOpening(float clef,short? key,short? meterTop,short? meterBottom){
    Clef=clef;Key=key;MeterTop=meterTop;MeterBottom=meterBottom;
   }
  }
  sealed class WholeMeasure {public Item Item;public float Left,Right;}
  static void ValidateSourceCapacity(Score input){if(input==null)throw new ArgumentNullException("input");if(input.Items.Count>1420||input.Items.Sum(i=>i.Bytes.Length/4)>14200)throw new NotSupportedException("PAGE input exceeds SCORE 3.11's original item/parameter capacities. Split the source before extracting parts; original over-capacity effects are not reproduced.");}
  static bool OrdinaryCueWave(Item item){return item.Code==4&&item.Count>=7&&item.P(7)<0&&item.P(6)==0;}
  static bool OrdinaryCueLine(Item item){return item.Code==4&&item.Count>=6&&item.P(7)>=0&&item.P(6)>item.P(3);}
  static bool OrdinaryGroupedCueSlur(Item item){
   return item.Code==5&&(item.Count==7||item.Count>=8&&item.Count<=20&&item.P(8)>=-4&&
    item.P(8)<=-1&&item.P(8)==Math.Truncate(item.P(8)))&&item.P(6)>item.P(3)&&item.P(7)!=0;
  }
  static bool SupportedGroupedCue(Item item,bool upper){
   if(OrdinaryCueLine(item)||OrdinaryGroupedCueSlur(item)||OrdinaryCueWave(item)||item.Code==9||item.Code==11||item.Code==15)return true;
   if(item.Code==1)return item.P(7)>0&&!Single.IsNaN(item.P(7))&&!Single.IsInfinity(item.P(7));
   if(item.Code==6)return item.Count>=7&&item.P(7)>=10&&item.P(7)<=19&&item.P(7)==Math.Truncate(item.P(7));
   if(ShortGroupedCueRest(item))return true;
   return upper&&item.Code==2&&item.P(5)==-2&&item.P(7)==4;
  }
  static bool ShortGroupedCueRest(Item item){
   return item.Code==2&&item.Count==7&&item.P(4)==100&&item.P(5)==-1&&
    item.P(6)==0&&(item.P(7)==.25f||item.P(7)==.5f);
  }
  static void FitGroupedCueBeam(Item beam,IEnumerable<Item> copies){
   // 09CA:151D selects the greatest P4 among cue notes within .1 of
   // each endpoint. The first copy is fitted by 09CA:12D2–1466.
   float left=GroupedCueBeamPitch(beam.P(3),copies);
   float right=GroupedCueBeamPitch(beam.P(6),copies);
   double span=((double)beam.P(6)-beam.P(3))/4;
   float limit=(float)span;
   if(span<1&&limit!=0)limit=1;
   if(Math.Abs((double)right-left)>=limit){
    if(right>left)left=(float)((double)right-limit);
    else right=(float)((double)left-limit);
   }
   beam.Set(4,(float)((double)left+(left<0?-100:100)));
   beam.Set(5,right);
  }
  static float GroupedCueBeamPitch(float x,IEnumerable<Item> copies){
   var notes=copies.Where(item=>item.Code==1&&Math.Abs((double)item.P(3)-x)<=.10000000149011612f);
   return notes.Select(item=>item.P(4)).DefaultIfEmpty(-999).Max()%100;
  }
  static void FitGroupedCueSlur(Item slur,IEnumerable<Item> copies){
   // 09CA:14B4 skips equal endpoints. 162A picks the lowest note within
   // three units and moves the slur endpoint below that note.
   if(slur.P(4)==slur.P(5))return;
   slur.Set(4,GroupedCueSlurPitch(slur.P(3),slur.P(4),copies));
   slur.Set(5,GroupedCueSlurPitch(slur.P(6),slur.P(5),copies));
  }
  static float GroupedCueSlurPitch(float x,float original,IEnumerable<Item> copies){
   float lowest=copies.Where(item=>item.Code==1&&Math.Abs((double)item.P(3)-x)<=3)
    .Select(item=>item.P(4)).DefaultIfEmpty(999).Min();
   if(lowest==999)lowest=original;
   int position=(int)lowest%100-2;
   return position>1&&position%2!=0?(float)((double)position+.4f):position;
  }
  static List<Item> CueItems(Score input,Item staff,Item rest,bool upperGrouped=false,ISet<Item> consumed=null,bool allowAlreadyMoved=false,bool grouped=false,bool scor4=false,bool modern=false,bool constructor=false,HashSet<int> ancestry=null){
   if(ancestry==null)ancestry=new HashSet<int>{(int)rest.P(13)};
   bool page4ShortMeasure=scor4&&!grouped&&rest.P(7)==3;
   if(rest.P(13)<1||rest.P(13)>32767||rest.P(13)!=Math.Truncate(rest.P(13))||Single.IsNaN(rest.P(7))||Single.IsInfinity(rest.P(7))||rest.P(7)<=0)throw new NotSupportedException("PAGE cue requires a valid instrument ID and a positive rhythmic duration.");
   var spans=input.Items.Where(i=>i.Code==14&&i.P(3)>0&&i.Staff<=staff.Staff&&i.Staff+i.P(4)>staff.Staff).Select(i=>new{Bottom=i.Staff,Top=i.Staff+(int)i.P(4)-1}).Distinct().ToArray();
   if(spans.Length!=1)throw new NotSupportedException("PAGE cue needs one verified system span.");
   var sources=input.Items.Where(i=>i.Code==8&&i.P(9)==rest.P(13)&&
    (page4ShortMeasure||i.Staff>=spans[0].Bottom&&i.Staff<=spans[0].Top)).ToArray();if(sources.Length!=1)throw new NotSupportedException("PAGE cue source is missing or ambiguous.");
   var bars=input.Items.Where(i=>i.Code==14&&i.Staff<=staff.Staff&&i.Staff+i.P(4)>staff.Staff&&i.P(3)>0).Select(i=>i.P(3)).Distinct().OrderBy(x=>x).ToArray();var ends=bars.Where(x=>x>rest.P(3)).ToArray();if(ends.Length==0)throw new NotSupportedException("PAGE cue has no ending bar.");float right=upperGrouped?199.60000610351562f:ends[0];var copies=new List<Item>();
   // 09CA:0A71..0B09: lower staff scans its following bars with the
   // original .4 bias. A later group member sees no lower-staff bar and
   // uses the literal 199.6 end instead (source416).
   double left=page4ShortMeasure?(double)rest.P(3)-.4f:rest.P(3);
   foreach(var item in input.Items.Where(i=>i.Staff==sources[0].Staff&&i.Code!=8&&i.Code!=14&&i.P(3)>=left&&(upperGrouped?i.P(3)<=right:i.P(3)<right)&&(consumed==null||!consumed.Contains(i)))){
    if(item.Code!=1&&item.Code!=2&&item.Code!=3&&item.Code!=6&&item.Code!=10&&item.Code!=17&&item.Code!=16&&item.Code!=9&&item.Code!=11&&item.Code!=15&&!OrdinaryCueLine(item)&&!OrdinaryCueWave(item)&&
     (!(grouped||scor4||modern||constructor)||!OrdinaryGroupedCueSlur(item)))throw new NotSupportedException("PAGE cue attachment and structural controls need verification.");
    if(modern&&item.Code==2&&item.P(13)!=0){int id=(int)item.P(13);if(ancestry.Contains(id))throw new System.IO.InvalidDataException("Cyclic cue instrument references cannot be extracted.");var next=new HashSet<int>(ancestry);next.Add(id);copies.AddRange(CueItems(input,staff,item,upperGrouped,consumed,allowAlreadyMoved,grouped,scor4,true,constructor,next));continue;}
    var copy=item.Clone();
    copy.Set(2,staff.Staff);
    if(copy.Code==1||copy.Code==2||copy.Code==6)
     copy.Set(4,(float)((double)(copy.P(4)%100)+(copy.P(4)<0?-100:100)));
    if(copy.Code==1){
     if(copy.P(5)<0||copy.P(5)>=30||copy.P(5)!=Math.Truncate(copy.P(5)))
      throw new NotSupportedException("PAGE cue note direction controls need verification.");
     copy.Set(5,10+(int)copy.P(5)%10);
     if(copy.Count>=8)copy.Set(8,0);
     if(copy.Count>=11&&copy.P(11)>0)copy.Set(11,-copy.P(11));
    }
    if(copy.Code==6){
     if(copy.Count<7||(copy.P(7)<10||copy.P(7)>=30||copy.P(7)!=Math.Truncate(copy.P(7))))
      throw new NotSupportedException("PAGE cue beam controls need verification.");
     if(page4ShortMeasure){
      copy.Set(4,(float)((double)copy.P(4)+1));
      copy.Set(5,(float)((double)copy.P(5)-1));
     }
     copy.Set(7,10+(int)copy.P(7)%10);
    }
    if(OrdinaryGroupedCueSlur(copy)&&copy.P(7)>0){
     // Original 09CA:0FD7–10D7 transforms the source before copying it.
     float offset=copy.P(8)==-2||copy.P(8)==-3?1.8f:3.8f;
     if(copy.P(8)==-4)copy.Set(8,-1);
     copy.Set(7,-copy.P(7));
     copy.Set(4,(float)((double)copy.P(4)-offset));
     copy.Set(5,(float)((double)copy.P(5)-offset));
    }
    copies.Add(copy);
    if(consumed!=null)consumed.Add(item);
   }
   return copies;
  }
  static void Consolidate(Score page,List<WholeMeasure> measures,bool paired=false,bool missingInstrument=false){
   var ordered=measures.OrderBy(m=>m.Left).ToArray();
   for(int at=0;at<ordered.Length;){int end=at+1;while(end<ordered.Length&&ordered[end-1].Right==ordered[end].Left&&!page.Items.Any(i=>(i.Code==18||i.Code==16||i.Code==9||missingInstrument&&(i.Code==5||i.Code==10))&&i.P(3)>=ordered[end].Left&&i.P(3)<ordered[end].Right||i.Code==14&&i.P(3)==ordered[end].Left&&i.P(5)>=1&&i.P(5)<=7))end++;int count=end-at;if(count>1){
    float left=ordered[at].Left,right=ordered[end-1].Right;var restSet=new HashSet<Item>(ordered.Skip(at).Take(count).Select(m=>m.Item));
    foreach(var item in page.Items.Where(i=>i.P(3)>=left&&i.P(3)<right&&!restSet.Contains(i))){
     if(item.Code==8)continue;
     if(item.Code==10||item.Code==9&&item.P(3)<ordered[at].Right)continue;
     if((item.Code==3||item.Code==17||item.Code==18||item.Code==16||missingInstrument&&(item.Code==5||item.Code==10))&&item.P(3)<ordered[at].Right)continue;
     if(item.Code==14){if(item.P(3)==left)continue;if(Enumerable.Range(5,Math.Max(0,item.Count-4)).Any(p=>p==12?item.P(p)!=0&&item.P(p)!=1:p==5&&item.P(3)==left?item.P(p)!=0&&item.P(p)!=1&&(!paired||item.P(p)!=8):item.P(p)!=0))throw new NotSupportedException("PAGE rest consolidation across a controlled barline requires verification.");continue;}
     throw new NotSupportedException("PAGE rest-run annotations require the remaining assembly stage.");
    }
    var first=ordered[at].Item;
    // DOS PAGE repeats the first measure duration for the entire rest run.
    float duration=0;for(int n=at;n<end;n++)duration=(float)((double)duration+first.P(17));first.Set(5,count>=10?-5:-10-count);first.Set(7,Math.Min(count,6));first.Set(8,count);first.Set(17,duration);
    page.Items.RemoveAll(i=>i.Code==10&&i.P(3)>=left&&i.P(3)<right||restSet.Contains(i)&&!Object.ReferenceEquals(i,first)||i.Code==14&&i.P(3)>left&&i.P(3)<right);
   }at=end;}
  }
  static bool Global(Item item){return (item.Code==9||item.Code==16||item.Code==10)&&(item.P(6)>=100&&item.P(6)<200||item.P(6)>=300&&item.P(6)<400)||item.Code==5&&(item.P(8)==1||item.P(8)==2)&&item.P(5)>=100&&item.P(5)<200;}
  static void MoveGroupedEndings(Score output){
   if(!output.Items.Any(item=>item.Code==5&&(item.P(8)==1||item.P(8)==2)))return;
   // 0E71:00AD–0169 scans horizontally from an ending. A note or rest
   // terminates the search. A bar moves the ending just past that bar,
   // then PAGE sorts the current index and starts the scan again.
   var ordered=NativePageSort.Apply(output.Items,false);
   for(int index=0;index<ordered.Count;index++){
    var ending=ordered[index];
    if(ending.Code!=5||ending.P(8)!=1&&ending.P(8)!=2)continue;
    for(int next=index+1;next<ordered.Count;next++){
     var item=ordered[next];
     if(item.Code<=2)break;
     if(item.Code!=14)continue;
     float afterBar=(float)((double)item.P(3)+.009999999776482582f);
     if(ending.P(3)==afterBar)throw new NotSupportedException("PAGE cannot advance this ending past its next bar.");
     ending.Set(3,afterBar);
     ordered=NativePageSort.Apply(ordered,false);
     index=-1;
     break;
    }
   }
   output.Items=ordered;
  }
  static void RebuildGroupOpening(Score output,int staffCount){
   // PAGE 0E71:016C..01F8 removes opening bars until a note/rest or
   // a P5=1 bar. 09CA:08DE..0A04 then constructs a fresh bar and brace.
   // Measured at both sides of both stages in source377/378, including
   // an input with no opening records and one already containing them.
   var headers=NativePageSort.Apply(output.Items,false).Where(i=>i.Code==8).ToArray();
   if(headers.Length!=staffCount)throw new NotSupportedException("PAGE opening needs one header per part staff.");
   RemoveOpeningBars(output,1);
   // The horizontal scan at 09CA:0600–0626 retains each header's P3,
   // so the last header supplies the new opening bar and brace.
   float left=headers[headers.Length-1].P(3);
   output.Items.Add(Item.Make(14,1,left,staffCount));output.Items.Add(Item.Make(14,1,left,staffCount,8));
   var ordered=output.Items.OrderBy(i=>i.Staff).ThenBy(i=>i.P(3)).ToArray();output.Items.Clear();output.Items.AddRange(ordered);
  }
  static void RemoveOpeningBars(Score page,int retainedStyle){
   var opening=page.Items.OrderBy(i=>i.P(3)).TakeWhile(i=>i.Code!=1&&i.Code!=2&&!(i.Code==14&&i.P(5)==retainedStyle));
   var bars=new HashSet<Item>(opening.Where(i=>i.Code==14));
   page.Items.RemoveAll(bars.Contains);
  }
  public static List<Segment> PrepareStaffGroups(Score input,int instrument,int staffCount,bool omitLeftItems=false){
   return PrepareStaffGroupsWithMissing(input,instrument,staffCount,omitLeftItems,null,true);
  }
  internal static List<Segment> PrepareStaffGroupsWithMissing(Score input,int instrument,int staffCount,bool omitLeftItems,Func<int,MissingInstrumentOpening> chooseOpening,bool firstSourceSegment,bool scor4=false,bool modernSystems=false){
   if(staffCount==1)return PrepareSingleStaffWithMissing(input,instrument,omitLeftItems,chooseOpening,firstSourceSegment,scor4);
   ValidateSourceCapacity(input);if(staffCount<2||staffCount>32)throw new NotSupportedException("PAGE staff groups leave the original 1â€“32 staff range.");
   if(input.Items.Any(i=>(i.Code==9||i.Code==16||i.Code==10)&&i.P(6)>=400||i.Code==5&&i.P(8)>0&&i.P(5)>=100&&(i.Count!=8||i.P(8)!=1&&i.P(8)!=2&&i.P(8)!=4||i.P(5)>=300)))throw new NotSupportedException("PAGE grouped global controls outside the verified bands.");
   var result=new List<Segment>();
   foreach(var selected in GroupedStaffHeaders(input,instrument,staffCount)){
    var top=selected[0];var bottom=selected[staffCount-1];
    var output=new Score{Trailer=(byte[])input.Trailer.Clone()};
    // The source system can span more staves than the requested part.
    // Measured lower and upper pairs (391/393) retain full system bars;
    // 0E71:0C31..0CA6 writes staff 1 and the selected count into P2/P4.
    var fullBars=new HashSet<Item>(input.Items.Where(i=>i.Code==14&&i.Staff<=bottom.Staff&&i.P(4)>=1&&i.P(4)==Math.Truncate(i.P(4))&&i.Staff+i.P(4)>top.Staff));
    var topGlobals=new HashSet<Item>();var repeatedGlobals=new HashSet<Item>();int repeatedStaff=0;
    if(input.Items.Any(Global)){
     var spans=fullBars.Where(i=>i.P(3)>0).Select(i=>new{Bottom=i.Staff,Top=i.Staff+(int)i.P(4)-1}).Distinct().ToArray();
     if(spans.Length!=1)throw new NotSupportedException("PAGE grouped text/number controls need one verified system span.");
     var span=spans[0];
     repeatedStaff=staffCount-(bottom.Staff-span.Bottom);
     foreach(var item in input.Items.Where(i=>Global(i)&&i.Staff>=span.Bottom&&i.Staff<=span.Top)){
      if(item.Staff<bottom.Staff){repeatedGlobals.Add(item);continue;}
      bool bringToTop=item.Code==5?item.Staff>top.Staff:
       item.P(6)>=300&&(item.Staff!=top.Staff||bottom.Staff>span.Bottom)||item.P(6)<200&&item.Staff>top.Staff;
      if(bringToTop)topGlobals.Add(item);
     }
    }
    var localBars=new HashSet<Item>(input.Items.Where(i=>i.Code==14&&i.Staff<=top.Staff&&
     (i.Staff>=bottom.Staff||i.Staff+i.P(4)>bottom.Staff)&&!fullBars.Contains(i)));
    if(localBars.Any(i=>i.Staff<bottom.Staff||i.P(4)<1||i.P(4)!=Math.Truncate(i.P(4))||
     i.Staff+i.P(4)>top.Staff+1||i.P(3)<=0||i.Count>5||(i.P(5)!=0&&i.P(5)!=2)))
     throw new NotSupportedException("PAGE partial bar span or controls need original verification.");
    var requests=input.Items.Where(i=>i.Code==2&&i.Staff>=bottom.Staff&&i.Staff<=top.Staff&&i.P(13)!=0).ToArray();
    bool independentRequests=requests.Length>1&&requests.Select(c=>c.P(13)).Distinct().Count()==requests.Length&&requests.Select(c=>c.Staff).Distinct().Count()==requests.Length;
    bool sharedSourceRequests=requests.Length>1&&requests.All(c=>c.P(13)==requests[0].P(13))&&requests.Select(c=>c.Staff).Distinct().Count()==requests.Length;
    bool mixedRequests=requests.Length==2&&requests[0].P(13)==requests[1].P(13)&&
     requests.Count(c=>c.Staff==bottom.Staff)==1;
    bool sharedRequest=(mixedRequests||sharedSourceRequests)&&requests.All(c=>c.P(3)==requests[0].P(3));
    bool sameUpperStaff=requests.Length==2&&requests[0].Staff!=bottom.Staff&&
     requests[0].Staff==requests[1].Staff&&requests[0].P(13)==requests[1].P(13)&&requests[0].P(3)!=requests[1].P(3);
    if(requests.Length>1&&!independentRequests&&!sharedSourceRequests&&!mixedRequests&&!sameUpperStaff&&requests.Any(c=>c.Staff!=bottom.Staff))
     throw new NotSupportedException("PAGE cue requests in this staff group need original verification.");
    var requestsAtPosition=new Dictionary<float,Item>();
    var cueCopies=new Dictionary<Item,List<Item>>();
    var measuredRestCues=new HashSet<Item>();
    // The lower cue rewrites its source records in place. They are no longer
    // on the cue source staff when the later request scans the same measure.
    var consumed=mixedRequests||sharedSourceRequests||sameUpperStaff?new HashSet<Item>():null;
    foreach(var cue in requests.OrderBy(c=>c.Staff).ThenBy(c=>c.P(3))){
     var target=selected[top.Staff-cue.Staff];bool upper=cue.Staff!=bottom.Staff;
     bool laterSameStaff=sameUpperStaff&&cueCopies.Count!=0;
     var copies=CueItems(input,target,cue,upper,consumed,laterSameStaff,true);
     var nominalCopies=laterSameStaff?CueItems(input,target,cue,upper,grouped:true):copies;
     float end=fullBars.Where(bar=>bar.P(3)>cue.P(3)).Min(bar=>bar.P(3));
     var firstMeasure=nominalCopies.Where(item=>item.P(3)<end).ToArray();
     bool restCue=(requests.Length==1||mixedRequests&&!sharedRequest||laterSameStaff)&&firstMeasure.Length==1&&firstMeasure[0].Code==2&&
      firstMeasure[0].P(3)==cue.P(3)&&firstMeasure[0].P(4)==100&&
      firstMeasure[0].P(5)==-2&&firstMeasure[0].P(7)==4;
     if(cue.Count<13||laterSameStaff&&(!restCue||copies.Count!=0)||copies.Any(i=>!SupportedGroupedCue(i,upper)&&(!restCue||i!=firstMeasure[0]))||
      !sharedRequest&&!independentRequests&&requestsAtPosition.ContainsKey(cue.P(3)))
      throw new NotSupportedException("PAGE grouped cue attachment, rhythm or measure needs original verification.");
     // The source measure contains a matching whole rest. DOS retains the
     // ordinary rest on each staff and can still emit later cue notes.
     if(restCue)measuredRestCues.Add(cue);
     else if(!requestsAtPosition.ContainsKey(cue.P(3)))requestsAtPosition.Add(cue.P(3),cue);
     cueCopies.Add(cue,copies);
    }
    foreach(var staff in selected.Reverse()){
     // Part extraction constructs a new seven-parameter staff header.
     // The original discards tiling, absolute placement, thickness and
     // numbering controls from the source staff.
     var records=new List<Item>();var wholeMeasures=new List<WholeMeasure>();int mapped=staff.Staff-bottom.Staff+1;records.Add(Item.Make(8,mapped,staff.P(3),0,.9f,staff.P(6),staff.P(7)));
     Action<bool> addCue=fitAttachments=>{
      foreach(var cue in requests.Where(c=>c.Staff==staff.Staff)){
       var copies=cueCopies[cue];
       foreach(var copy in copies.Where(i=>i.Code==1||i.Code==6||OrdinaryCueLine(i)||OrdinaryCueWave(i)||
        ShortGroupedCueRest(i)||OrdinaryGroupedCueSlur(i)||i.Code==9||i.Code==11||i.Code==15)){
        var item=copy.Clone();item.Set(2,mapped);
        if(fitAttachments&&item.Code==6)FitGroupedCueBeam(item,copies);
        if(fitAttachments&&item.Code==5)FitGroupedCueSlur(item,copies);
        records.Add(item);
       }
      }
     };
     addCue(true);
     var ownRecords=input.Items.Where(i=>i.Code!=8&&i.Code!=14&&i.Staff==staff.Staff&&!topGlobals.Contains(i)||
      (mapped==staffCount||mapped==repeatedStaff)&&repeatedGlobals.Contains(i)||
      mapped==1&&(fullBars.Contains(i)||localBars.Contains(i)));
     // The later shared-item pass follows the local records at an equal X.
     var sharedRecords=input.Items.Where(i=>mapped==staffCount&&topGlobals.Contains(i));
     foreach(var source in ownRecords.Concat(sharedRecords)){
      if(omitLeftItems&&(double)source.P(3)<(double)staff.P(3)-1)continue;
      // 09CA:0192..0229 is the selected part pass; the caller's
      // 2F7A:0AFE..0D73 scan adds eligible records from the other staves.
      if((source.Code==9||source.Code==10||source.Code==16)&&source.P(6)>=200&&!topGlobals.Contains(source)&&!repeatedGlobals.Contains(source))continue;
      if(source.Code==5&&(source.P(8)==1||source.P(8)==2||source.P(8)==4)&&source.P(5)>=200)continue;
      var item=source.Clone();item.Set(2,mapped);
      if(measuredRestCues.Contains(source)){
       item.Set(13,0);
       if(mapped==1)item.Set(4,100);
      }
      if((item.Code==9||item.Code==10||item.Code==16)&&item.P(6)>=300)item.Set(6,0);
      if(item.Code==14){item.Set(4,staffCount);if(item.P(3)==0)item.Set(3,staff.P(3));}
      // The grouped constructor copies the cue at the request and again
      // when it processes the source staff. Canonicalization keeps the
      // lower request rest, deleting an adjacent matching upper rest.
      Item request;
      if(source.Code==2&&source.P(9)<=0&&requests.Contains(source)){item.Set(4,-8);records.Add(item);continue;}
      if(source.Code==2&&requestsAtPosition.TryGetValue(source.P(3),out request)&&(source==request&&source.P(9)>0||source.Count<13&&fullBars.Any(b=>b.P(3)>source.P(3)&&b.P(3)<199.6f)&&source.P(9)>0&&source.P(5)==-2&&source.P(7)==4)){

       if(source==request){if(mapped==1){item.Set(4,-8);records.Add(item);}continue;}
       if(mapped==1)records.Add(item);
       continue;
      }
      if(item.Code==2&&item.P(5)==-2){var bars=fullBars.Where(i=>i.P(3)>0).Select(i=>i.P(3)).OrderBy(x=>x).ToArray();var ends=bars.Where(x=>x>item.P(3)).ToArray();if(ends.Length==0)throw new NotSupportedException("PAGE grouped whole rest lacks ending bar.");float right=ends[0],left=bars.Where(x=>x<=item.P(3)).DefaultIfEmpty(staff.P(3)).Max();if(!input.Items.Any(i=>i.Code==1&&i.Staff>=bottom.Staff&&i.Staff<=top.Staff&&i.P(3)>=left&&i.P(3)<right)){
       // A cue copied past its opening measure suppresses upper rests
       // in each eligible measure it reaches. PAGE preserves extended
       // request-bank records and the final 199.6/200-unit span.
       // The lowest rest remains ordinary.
       if(cueCopies.Values.Any(copies=>copies.Any(note=>note.Code==1&&note.P(3)>=left&&note.P(3)<right))){
        if(mapped==1||source.Count>=13||source.P(9)<=0||right>=199.6f)records.Add(item);
        continue;
       }
       var paired=input.Items.Where(i=>i.Code==2&&i.Staff>=bottom.Staff&&i.Staff<=top.Staff&&i.P(3)>=left&&i.P(3)<right).ToArray();
       if(paired.Length!=staffCount||paired.Select(i=>i.Staff).Distinct().Count()!=staffCount||paired.Any(i=>i.P(4)!=0||i.P(5)!=-2||i.P(6)!=0||i.P(7)!=3&&i.P(7)!=4||i.P(8)!=0||Enumerable.Range(10,Math.Max(0,i.Count-9)).Any(p=>i.P(p)!=0&&(p!=13||!measuredRestCues.Contains(i)))))throw new NotSupportedException("PAGE controlled grouped whole rests need verification.");
       item.Set(7,2);item.Set(8,1);item.Set(9,-1);item.Set(17,source.P(7));wholeMeasures.Add(new WholeMeasure{Item=item,Left=left,Right=right});
      }}
      records.Add(item);
     }
     addCue(false);
     var consolidation=new Score();consolidation.Items.AddRange(records);var extraBars=new HashSet<Item>();if(mapped>1)foreach(var bar in input.Items.Where(fullBars.Contains)){var copy=bar.Clone();copy.Set(2,mapped);copy.Set(4,staffCount);if(copy.P(3)==0)copy.Set(3,staff.P(3));consolidation.Items.Add(copy);extraBars.Add(copy);}Consolidate(consolidation,wholeMeasures,true);output.Items.AddRange(consolidation.Items.Where(i=>!extraBars.Contains(i)).OrderBy(i=>i.P(3)));
    }
    MoveGroupedEndings(output);
    RebuildGroupOpening(output,staffCount);result.Add(new Segment{SourceStaff=bottom.Staff,Score=output});
   }
   return AddMissingInstrumentSegments(input,instrument,staffCount,result,UnmatchedDeclaredSystems(input,instrument),chooseOpening,firstSourceSegment);
  }
  static IEnumerable<Item[]> GroupedStaffHeaders(Score input,int instrument,int staffCount){
   var headers=input.Items.Where(item=>item.Code==8).ToLookup(item=>item.Staff);
   var widths=new int[33];
   // PAGE 2F7A:06F3–084F takes the largest Code 14 span at each bottom
   // staff, then walks the nonoverlapping spans from the bottom up.
   foreach(var bar in input.Items.Where(item=>item.Code==14&&item.Staff>=1&&item.Staff<=32)){
    float span=Math.Abs(bar.P(4));
    if(Single.IsNaN(span)||span>=34-bar.Staff)
     throw new NotSupportedException("PAGE bar span crosses the staff table.");
    int width=(int)span;
    widths[bar.Staff]=Math.Max(widths[bar.Staff],Math.Max(width,1));
   }
   var parts=new List<Item[]>();
   for(int bottom=1;bottom<=32;){
    int width=widths[bottom];
    if(width==0){bottom++;continue;}
    int top=bottom+width-1;
    // 2F7A:09DC–0AF0 selects the first matching instrument in that
    // system. The requested size then takes consecutive staves from it.
    int first=bottom;
    while(first<=top&&!headers[first].Any(item=>item.P(9)==instrument))first++;
    if(first<=top){
     if(first+staffCount-1>top)throw new NotSupportedException("PAGE part extends beyond its source system.");
     var group=new Item[staffCount];
     for(int n=0;n<staffCount;n++){
      var candidates=headers[first+n].ToArray();
      if(candidates.Length!=1)throw new NotSupportedException("PAGE part requires one source header per staff.");
      group[staffCount-1-n]=candidates[0];
     }
     parts.Add(group);
    }
    bottom=top+1;
   }
   parts.Reverse();
   return parts;
  }
  public static List<Segment> PrepareSingleStaffSegments(Score input,int instrument,bool omitLeftItems=false){
   return PrepareSingleStaffWithMissing(input,instrument,omitLeftItems,null,true);
  }
  struct StaffSpan {
   internal readonly int Bottom,Top;
   internal StaffSpan(Item bar){Bottom=bar.Staff;Top=bar.Staff+(int)bar.P(4)-1;}
  }
  static StaffSpan[] SingleStaffSystemSpans(Score input,int staff,bool page4CuePart){
   var bars=input.Items.Where(i=>i.Code==14&&i.P(4)>=1&&i.P(4)==Math.Truncate(i.P(4))&&i.Staff<=staff&&i.Staff+i.P(4)>staff);
   var spans=bars.Where(i=>i.P(3)>0).Select(i=>new StaffSpan(i)).Distinct().ToArray();
   if(page4CuePart&&(spans.Length==0||spans.Length==1&&spans[0].Bottom==spans[0].Top)){
    var openings=bars.Where(i=>i.P(3)==0&&i.P(5)==9).Select(i=>new StaffSpan(i)).Distinct().ToArray();
    if(openings.Length==1)spans=openings;
   }
   return spans;
  }
  internal static List<Segment> PrepareSingleStaffWithMissing(Score input,int instrument,bool omitLeftItems,Func<int,MissingInstrumentOpening> chooseOpening,bool firstSourceSegment,bool scor4=false,bool modernSystems=false){
   ValidateSourceCapacity(input);if(instrument<1||instrument>32767)throw new NotSupportedException("PAGE instrument ID is outside the verified signed-word domain.");
   foreach(var item in input.Items){
    if(item.Code==5&&item.P(5)>=100&&(item.P(5)>=300||item.P(8)!=1&&item.P(8)!=2&&item.P(8)!=4))throw new NotSupportedException("PAGE bracket/slur inclusion controls outside the measured ending routes.");
    if(item.Code==10&&item.P(6)>=400)throw new NotSupportedException("PAGE number control beyond the measured include/exclude bands.");
    if(item.Code==16&&item.P(6)>=400)throw new NotSupportedException("PAGE text control beyond the measured include/exclude bands.");
   }
   var staves=input.Items.Where(i=>i.Code==8&&i.P(9)==instrument).OrderByDescending(i=>i.Staff).ToArray();if(staves.Select(i=>i.Staff).Distinct().Count()!=staves.Length)throw new NotSupportedException("PAGE duplicate staff definitions require verification.");var result=new List<Segment>();
   var repeatedInstrumentOwners=new HashSet<int>();
   var skippedInstrumentStaves=new HashSet<int>();
   var missingSystems=UnmatchedDeclaredSystems(input,instrument);
   var page4CueStaves=new HashSet<int>(scor4?input.Items.Where(i=>i.Code==2&&i.P(13)!=0&&i.P(7)==3).Select(i=>i.Staff):Enumerable.Empty<int>());
   bool page4CuePart=staves.Any(i=>page4CueStaves.Contains(i.Staff));
   foreach(var global in input.Items.Where(Global)){
    var bounds=SingleStaffSystemSpans(input,global.Staff,page4CuePart);
    if(bounds.Length!=1)
     throw new NotSupportedException("PAGE cannot assign a shared item on staff "+global.Staff+" to one system. The barlines do not identify a unique staff span.");
    var span=bounds[0];
    var matches=staves.Where(i=>i.Staff>=span.Bottom&&i.Staff<=span.Top).OrderBy(i=>i.Staff).ToArray();
    if(matches.Length==0)missingSystems[span.Bottom]=span.Top;
    if(matches.Length>1){
     repeatedInstrumentOwners.Add(matches[0].Staff);
     skippedInstrumentStaves.UnionWith(matches.Skip(1).Select(i=>i.Staff));
    }
   }
   staves=staves.Where(i=>!skippedInstrumentStaves.Contains(i.Staff)).ToArray();
   foreach(var staff in staves){
    if(staff.Staff<1||staff.Staff>32)
     throw new NotSupportedException("PAGE staff number must be between 1 and 32.");
    if(Single.IsNaN(staff.P(5))||Single.IsInfinity(staff.P(5))||staff.P(5)<0)throw new NotSupportedException("PAGE extraction requires a nonnegative finite source staff size; zero uses SCORE's default size.");
    // Original PAGE writes its initial .9 size into the extracted segment;
    // the later S setting belongs to final page assembly, not this file.
    var page=new Score{Trailer=(byte[])input.Trailer.Clone()};var header=Item.Make(8,1,staff.P(3),0,.9f,staff.P(6),staff.P(7));page.Items.Add(header);var wholeMeasures=new List<WholeMeasure>();
    int groupBottom=staff.Staff,groupTop=staff.Staff;
    if(input.Items.Any(Global)){
     var spans=SingleStaffSystemSpans(input,staff.Staff,page4CueStaves.Contains(staff.Staff));
     if(spans.Length!=1)throw new NotSupportedException("PAGE global text needs one verified system bar-span boundary.");
     groupBottom=spans[0].Bottom;groupTop=spans[0].Top;
     if(staves.Count(i=>i.Staff>=groupBottom&&i.Staff<=groupTop)!=1)throw new NotSupportedException("PAGE global text with duplicate selected instruments in a system requires verification.");
    }
    var coveringBars=input.Items.Where(i=>i.Code==14&&i.Staff<=staff.Staff&&i.Staff+i.P(4)>staff.Staff).ToArray();
    bool page4SixEightCues=page4CueStaves.Contains(staff.Staff);
    // In the SCORE 4 cue-extraction tests, a system bar supplied the
    // closing position. Interior bars survived only on the selected staff.
    var closingSystemBar=page4SixEightCues?coveringBars.OrderBy(i=>i.P(3)).LastOrDefault():null;
    var inheritedBars=new HashSet<Item>(page4SixEightCues?
     coveringBars.Where(i=>ReferenceEquals(i,closingSystemBar)):coveringBars);
    var sourceItems=input.Items.Where(i=>i.Code!=8&&(i.Staff==staff.Staff||inheritedBars.Contains(i)||Global(i)&&i.Staff>=groupBottom&&i.Staff<=groupTop)).ToList();
    var fittedCueCopies=new HashSet<Item>();var emptyCueRequests=new HashSet<Item>();var cueQueue=new Queue<Tuple<Item,HashSet<int>>>();foreach(var request in sourceItems.Where(i=>i.Code==2&&i.P(13)!=0))cueQueue.Enqueue(Tuple.Create(request,new HashSet<int>{(int)request.P(13)}));
    while(cueQueue.Count>0){var queued=cueQueue.Dequeue();var rest=queued.Item1;
     bool cueConstructor=staff.Staff==staves.Min(h=>h.Staff);var cues=CueItems(input,staff,rest,scor4:scor4,modern:modernSystems,constructor:cueConstructor,ancestry:queued.Item2);
     if(cues.Count==0)emptyCueRequests.Add(rest);
     if((cueConstructor||modernSystems)&&!page4SixEightCues){
      // The first selected staff takes PAGE's constructor pass before the
      // following copy pass. Modern Parts emits one fitted copy per request.
      var fitted=cues.Select(i=>i.Clone()).ToList();foreach(var cue in fitted){if(cue.Code==6)FitGroupedCueBeam(cue,cues);if(cue.Code==5)FitGroupedCueSlur(cue,cues);fittedCueCopies.Add(cue);}
      sourceItems.InsertRange(sourceItems.IndexOf(rest),fitted);if(!modernSystems)sourceItems.AddRange(cues);
     }else sourceItems.InsertRange(sourceItems.IndexOf(rest),cues);
     if(!modernSystems)foreach(var child in cues.Where(i=>i.Code==2&&i.P(13)!=0)){int id=(int)child.P(13);if(queued.Item2.Contains(id))throw new System.IO.InvalidDataException("Cyclic cue instrument references cannot be extracted.");var next=new HashSet<int>(queued.Item2);next.Add(id);cueQueue.Enqueue(Tuple.Create(child,next));}
     if(sourceItems.Count>1420||sourceItems.Sum(i=>i.Bytes.Length/4)>14200)throw new NotSupportedException("Expanded cues exceed the original SCORE item/parameter capacity.");
    }
    if(!sourceItems.Any(i=>i.Code==18)){
     var meterSpans=input.Items.Where(i=>i.Code==14&&i.P(3)>0&&i.Staff<=staff.Staff&&i.Staff+i.P(4)>staff.Staff).Select(i=>new{Bottom=i.Staff,Top=i.Staff+(int)i.P(4)-1}).Distinct().ToArray();
     if(meterSpans.Length==1){var inherited=input.Items.Where(i=>i.Code==18&&i.Staff>=meterSpans[0].Bottom&&i.Staff<=meterSpans[0].Top).ToArray();if(inherited.Length>1)throw new NotSupportedException("PAGE competing inherited meter records require verification.");if(inherited.Length==1)sourceItems.Add(inherited[0]);}
    }
    foreach(var source in sourceItems.OrderBy(i=>i.P(3))){
     if((source.Code==9||source.Code==16||source.Code==10)&&source.P(6)>=200&&source.P(6)<300)continue;
     if(source.Code==9&&source.P(6)>=300&&source.Staff==staff.Staff)continue;
     // With a repeated instrument ID, DOS extracts the first staff and
     // copies +300 items from the other staves in that system (1700/1701).
     if((source.Code==9||source.Code==16||source.Code==10)&&source.P(6)>=300&&source.Staff==staff.Staff&&repeatedInstrumentOwners.Contains(staff.Staff))continue;
     if(source.Code==5&&source.P(8)>0&&source.P(5)>=200&&source.P(5)<300)continue;
     if(Single.IsNaN(source.P(3))||Single.IsInfinity(source.P(3)))throw new NotSupportedException("PAGE nonfinite item position is not supported.");
     if(omitLeftItems&&(double)source.P(3)<(double)staff.P(3)-1)continue;
     if(source.Code==14&&source.P(3)==0&&source.P(5)!=4)continue;
     var item=source.Clone();item.Set(2,1);
     if(source.Code==2&&source.P(13)!=0){if(!fittedCueCopies.Contains(source))item.Set(4,-8);if(emptyCueRequests.Contains(source)){if(modernSystems)item.Set(4,0);item.Set(13,0);item.Set(7,2);item.Set(8,1);item.Set(9,-1);item.Set(17,source.P(7));}}
     else if(source.Code==2&&source.P(5)<0&&source.P(5)!=-1){
      var bars=input.Items.Where(i=>i.Code==14&&i.Staff<=staff.Staff&&i.Staff+i.P(4)>staff.Staff&&i.P(3)>0).Select(i=>i.P(3)).Distinct().OrderBy(x=>x).ToArray();
      var ends=bars.Where(x=>x>source.P(3)).ToArray();if(ends.Length==0)throw new NotSupportedException("PAGE bounded rest has no verified ending bar.");
      float right=ends[0],left=bars.Where(x=>x<=source.P(3)).DefaultIfEmpty(staff.P(3)).Max();
      bool shared=input.Items.Any(i=>!Object.ReferenceEquals(i,source)&&i.Staff==staff.Staff&&(i.Code==1||i.Code==2)&&i.P(3)>=left&&i.P(3)<right);
      if(!shared){
       float duration=source.P(7);
       if(source.Count<7||source.P(5)!=-2||source.P(4)!=0||source.P(6)!=0||
        duration!=.5f&&duration!=1&&duration!=1.5f&&duration!=2&&duration!=2.5f&&duration!=3&&duration!=3.5f&&duration!=4&&duration!=5&&duration!=6&&duration!=8||
        source.P(8)!=0||
        Single.IsNaN(source.P(9))||Single.IsInfinity(source.P(9))||
        Enumerable.Range(10,Math.Max(0,source.Count-9)).Any(p=>source.P(p)!=0))
        throw new NotSupportedException("PAGE isolated whole-rest controls require the remaining rest-assembly stage.");
       // PAGE retains the whole-rest duration in P17,
       // marks the whole rest as one measure, and clears its old endpoint.
       item.Set(7,2);item.Set(8,1);item.Set(9,-1);item.Set(17,duration);
       wholeMeasures.Add(new WholeMeasure{Item=item,Left=left,Right=right});
      }
     }
     if((item.Code==9||item.Code==10||item.Code==16)&&item.P(6)>=300)item.Set(6,0);
     if(item.Code==14){
      float span=item.P(4);
      if(span==0||span<short.MinValue||span>short.MaxValue||span!=Math.Truncate(span))throw new NotSupportedException("PAGE bar span requires a signed nonzero word.");
      item.Set(4,1);
      if(page4SixEightCues&&item.P(5)==4&&item.P(12)==1)item.Set(12,0);
     }
     page.Items.Add(item);
    }
    // PAGE's horizontal sort includes the staff header. Negative-position
    // text can therefore precede it in the intermediate extraction file.
    // Original single-staff extraction retains an opening start-repeat,
    // including P5=4 before the first rhythmic item (test 1667).
    RemoveOpeningBars(page,4);
    var closingBar=page.Items.Where(i=>i.Code==14).OrderBy(i=>i.P(3)).LastOrDefault();
    if(closingBar!=null&&closingBar.P(5)>7)
     throw new NotSupportedException("PAGE needs a closing measure barline after the final brace, bracket or extended dashed bar.");
    Consolidate(page,wholeMeasures);var sorted=page.Items.OrderBy(i=>i.P(3)).ToArray();page.Items.Clear();page.Items.AddRange(sorted);
    result.Add(new Segment{SourceStaff=staff.Staff,Score=page});
   }
   return AddMissingInstrumentSegments(input,instrument,1,result,missingSystems,chooseOpening,firstSourceSegment);
  }
  static SortedDictionary<int,int> UnmatchedDeclaredSystems(Score input,int instrument){
   var systems=new SortedDictionary<int,int>();
   var headers=new HashSet<int>(input.Items.Where(i=>i.Code==8).Select(i=>i.Staff));
   var staves=input.Items.Where(i=>i.Code==8&&i.P(9)==instrument).ToArray();
   var spans=input.Items.Where(i=>i.Code==14&&i.P(3)>0&&i.P(4)>=1&&i.P(4)<=32&&i.P(4)==Math.Truncate(i.P(4)))
    .Select(i=>new{Bottom=i.Staff,Top=i.Staff+(int)i.P(4)-1}).Distinct().ToArray();
   foreach(var span in spans){
    if(spans.Any(other=>other!=span&&other.Bottom<=span.Top&&other.Top>=span.Bottom)||
     !Enumerable.Range(span.Bottom,span.Top-span.Bottom+1).All(headers.Contains)||
     staves.Any(i=>i.Staff>=span.Bottom&&i.Staff<=span.Top))continue;
    systems.Add(span.Bottom,span.Top);
   }
   return systems;
  }
  static List<Segment> AddMissingInstrumentSegments(Score input,int instrument,int staffCount,List<Segment> result,SortedDictionary<int,int> missingSystems,Func<int,MissingInstrumentOpening> chooseOpening,bool firstSourceSegment){
   if(missingSystems.Count!=0){
    int topStaff=result.Select(segment=>segment.SourceStaff).DefaultIfEmpty(0).Max();
    foreach(var span in missingSystems.Reverse()){
     MissingInstrumentOpening opening=null;
     if(firstSourceSegment&&span.Value>topStaff){
      opening=chooseOpening==null?new MissingInstrumentOpening(0,null,null,null):chooseOpening(instrument);
      if(opening==null)throw new OperationCanceledException();
      firstSourceSegment=false;
     }
     result.Add(MissingInstrumentSegment(input,span.Key,span.Value,opening,topStaff>span.Value?staffCount:0,staffCount));
    }
   }
   return result.OrderByDescending(segment=>segment.SourceStaff).ToList();
  }
  static Segment MissingInstrumentSegment(Score input,int bottom,int top,MissingInstrumentOpening opening,int barSpan,int staffCount){
   var bars=input.Items.Where(i=>i.Code==14&&i.Staff==bottom&&i.P(4)==top-bottom+1&&i.P(3)>0).OrderBy(i=>i.P(3)).ToArray();
   var globals=input.Items.Where(i=>Global(i)&&i.Staff>=bottom&&i.Staff<=top).ToArray();
   var meters=input.Items.Where(i=>i.Code==18&&i.Staff>=bottom&&i.Staff<=top)
    .OrderBy(i=>i.P(3)).ThenBy(i=>i.Staff).GroupBy(i=>i.P(3)).Select(g=>g.First()).ToArray();
   bool openingMeter=meters.Length!=0&&bars.Length!=0&&meters[0].P(3)<bars[0].P(3);
   if(bars.Length==0||bars.Any(i=>i.Count!=4)||!openingMeter&&bars.Length<2||meters.Any(i=>i.Count!=6))
    throw new NotSupportedException("PAGE replacement systems need ordinary barlines and a measured first rest interval.");
   var page=new Score();
   if(opening!=null){
    page.Items.Add(Item.Make(8,1,0,0,.9f));
    page.Items.Add(Item.Make(3,1,1,0,opening.Clef));
    if(opening.Key.HasValue)page.Items.Add(Item.Make(17,1,2,0,opening.Key.Value,opening.Clef));
    if(opening.MeterTop.HasValue)page.Items.Add(Item.Make(18,1,3,0,opening.MeterTop.Value,opening.MeterBottom.GetValueOrDefault()));
   }
   // 2E83 uses the first structural position as the left boundary. With
   // no opening meter, the first bar becomes that boundary (case 1735).
   foreach(var source in meters){var meter=source.Clone();meter.Set(2,1);page.Items.Add(meter);}
   var sharedRecords=new HashSet<Item>();
   foreach(var source in globals){
    var item=source.Clone();item.Set(2,1);
    if((item.Code==9||item.Code==10||item.Code==16)&&item.P(6)>=300)item.Set(6,0);
    sharedRecords.Add(item);page.Items.Add(item);
   }
   var measures=new List<WholeMeasure>();
   float left=openingMeter?meters[0].P(3):bars[0].P(3);
   foreach(var sourceBar in bars.Skip(openingMeter?0:1)){
    float right=sourceBar.P(3);
    // PAGE 2E83:02AD..02C1 bisects adjacent positions. The generated
    // rest always carries three beats, even after 2/4, 5/8 or 5/4.
    var rest=Item.Make(2,1,(float)((double)left+((double)right-left)/2),0,-2,0,2,1,-1);
    rest.Set(17,3);
    page.Items.Add(rest);page.Items.Add(Item.Make(14,1,right,barSpan));
    measures.Add(new WholeMeasure{Item=rest,Left=left,Right=right});
    left=right;
   }
   // 0E71 restarts its sort when an ending encounters a bar before the
   // next rest. Case 1745 repeats the same final write at bar X + .01.
   if(globals.Any(i=>i.Code==5&&measures.Any(m=>i.P(3)>m.Item.P(3)&&i.P(3)<m.Right)))
    throw new NotSupportedException("PAGE cannot advance this replacement ending past its next bar.");
   Consolidate(page,measures,missingInstrument:true);
   var records=page.Items.OrderBy(i=>i.P(3)).ToArray();
   var output=new Score{Trailer=(byte[])input.Trailer.Clone()};
   for(int staff=1;staff<=staffCount;staff++){
    foreach(var record in records){
     if(record.Code==14&&staff!=1||sharedRecords.Contains(record)&&staff!=staffCount)continue;
     var item=record.Clone();item.Set(2,staff);output.Items.Add(item);
    }
   }
   if(staffCount>1&&opening!=null)RebuildGroupOpening(output,staffCount);
   return new Segment{SourceStaff=top,Score=output};
  }
 }
}
