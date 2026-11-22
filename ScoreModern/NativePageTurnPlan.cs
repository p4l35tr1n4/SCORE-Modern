using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

namespace ScoreModern {
 internal static class NativePageTurnPlan {
  internal static NativePagePreparedPart Prepare(string path,float quarters,int requested,float size,float height,int firstPage,bool centered,int linesPerPage=1){
   string source=Path.GetFullPath(path);var score=Score.Load(source);
   var headers=score.Items.Where(i=>i.Code==8).OrderBy(i=>i.Staff).ToArray();int staves=headers.Length;
   if(staves<1||staves>32||headers.Where((h,n)=>h.Staff!=n+1).Any()||score.Items.Any(i=>i.Staff<1||i.Staff>staves)||linesPerPage<1||linesPerPage>32/staves)
    throw new NotSupportedException("PAGE turns require one extracted system of consecutive staves and a lines-per-page count within the 32-staff output bank.");
   var raw=NativePageBarPlan.PrepareLayoutFile(score);
   var widths=new short[raw.Widths.Length];var leading=new short[widths.Length];
   for(int n=0;n<widths.Length;n++){
    var value=Math.Truncate(Math.Abs((double)raw.Widths[n])*(float)((double)size*.01f)+.5);
    if(value<1||value>32767)throw new NotSupportedException("PAGE turn bar width leaves the original bank.");
    widths[n]=(short)value;leading[n]=checked((short)Math.Min(raw.Leading[n+1]+raw.Trailing[n],9000));
   }
   return Assemble(source,score,widths,leading,quarters,requested,size,height,firstPage,centered,linesPerPage,staves);
  }
  internal static NativePagePreparedPart PrepareOld(string path,float quarters,int requested,int linesPerPage=1,int firstPage=1){
   path=Path.GetFullPath(path);
   var lines=File.ReadAllLines(path).Where(s=>!String.IsNullOrWhiteSpace(s)).ToArray();
   Func<string,string[]> fields=s=>s.Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
   if(lines.Length!=8)throw new NotSupportedException("Old Page Data currently supports a single SCORE system with its source file beside BARS.TMP.");
   var header=fields(lines[0]);var barCount=fields(lines[1]);var weights=fields(lines[3]);var positions=fields(lines[4]);var turns=fields(lines[5]);var staff=fields(lines[6]);
   int count;
   string name=lines[2].Trim();
   if(header.Length!=5||header[0]!="1"||barCount.Length!=1||!Int32.TryParse(barCount[0],out count)||count<2||count>1000||
    weights.Length!=count||positions.Length!=count||turns.Length!=count||staff.Length!=32||String.IsNullOrEmpty(name)||Path.GetFileName(name)!=name||name.IndexOf(':')>=0)
    throw new InvalidDataException("Old Page Data does not contain a supported single-system job.");
   string source=Path.Combine(Path.GetDirectoryName(path),name);
   var score=Score.Load(source);
   if(score.Items.Count(i=>i.Code==8)!=1||score.Items.Any(i=>i.Staff!=1))throw new NotSupportedException("Old Page Data is verified for a single staff.");
   float size;int inches;
   if(!Single.TryParse(staff[0],NumberStyles.Float,CultureInfo.InvariantCulture,out size)||size!=25f||!Int32.TryParse(header[1],out inches)||inches!=10)
    throw new NotSupportedException("Old Page Data is verified for the original size-1, ten-inch page route.");
   var fresh=NativePageBarPlan.PrepareLayoutFile(score);
   if(fresh.Widths.Length!=count)throw new InvalidDataException("The source score no longer matches Old Page Data.");
   var widths=new short[count];var gaps=new short[count];
   for(int n=0;n<count;n++){
    short weight;float position;
    if(!Int16.TryParse(weights[n],out weight)||weight!=fresh.Widths[n]||!Single.TryParse(positions[n],NumberStyles.Float,CultureInfo.InvariantCulture,out position)||Single.IsNaN(position)||
      Math.Abs((double)position-fresh.Positions[n])>.0051||!Int16.TryParse(turns[n],out gaps[n])||gaps[n]!=Math.Min(fresh.Leading[n+1]+fresh.Trailing[n],9000))
     throw new InvalidDataException("The source score no longer matches the saved bar plan.");
    widths[n]=checked((short)Math.Truncate(Math.Abs((double)weight)*.01f+.5));
   }
   return Assemble(source,score,widths,gaps,quarters,requested,1,10,firstPage,false,linesPerPage);
  }
  static NativePagePreparedPart Assemble(string source,Score score,short[] widths,short[] leading,float quarters,int requested,float size,float height,int firstPage,bool centered,int linesPerPage=1,int staves=1){
   var plan=NativePageDefaultPartPlan.NumberedPages(Propose(widths,leading,leading,quarters,requested,height,linesPerPage,firstPage),height,firstPage,centered);
   var warnings=new List<string>();
   var pages=NativePagePartPages.AssembleTransposed(new[]{score},staves,size,plan,new float[4],1,1,null,warnings.Add,null,0,0,null,true);
   var writer=new NativePageFileBank(new byte[14200*4]);short measurement=NativeMeasurements.Get(score);
   return new NativePagePreparedPart(plan,pages.Select(p=>writer.Encode(p,measurement)).ToArray(),new[]{source},warnings.AsReadOnly());
  }
  // PAGE 3197:108B..13EF chooses candidate ends and page counts;
  // 0FE6:11CC..137A replans each selected segment.
  internal static List<int[]> Propose(short[] widths,short[] turns,short[] leading,float quarters,int requested,float height,int linesPerPage=1,int firstPage=1){
   if(widths==null||turns==null||leading==null||widths.Length<2||widths.Length>1000||turns.Length!=widths.Length||leading.Length!=widths.Length||
    widths.Any(x=>x<=0)||turns.Any(x=>x<0)||requested<1||requested>99||linesPerPage<1||linesPerPage>32||firstPage<0||firstPage>32767||quarters<=0||quarters>300||Single.IsNaN(quarters))
    throw new NotSupportedException("PAGE turn planning requires positive page, line and silence controls within the original capacities.");
   var gaps=(short[])turns.Clone();gaps[gaps.Length-1]=30000;
   int total=widths.Sum(x=>(int)x),start=0,groupPages=firstPage%2==0?2:1,remaining=requested;
   if(total>32767)throw new NotSupportedException("PAGE turn weights exceed the original table.");
   int minimum=(int)((double)quarters*100+.5);if(minimum==0)minimum=200;
   var result=new List<int[]>();var planner=new NativePageAutomaticLayout();
   while(start<widths.Length){
    int desired=remaining>0?remaining:total/200;
    if(desired<1)throw new NotSupportedException("PAGE turn request leaves no valid candidate pages.");
    if(desired<2)groupPages=1;
    remaining-=groupPages;
    int goal=(int)(float)((double)(total*groupPages)/desired),threshold=minimum;
    int end,subtotal;
    while(true){
     end=start;subtotal=widths[end];
     while(subtotal<goal&&end+1<widths.Length)subtotal+=widths[++end];
     if(subtotal<goal)throw new NotSupportedException("SCORE's turn search extends past the last bar.");
     int before=end,after=end,low=subtotal,high=subtotal,step=-1;
     while(gaps[end]<threshold){
      if(subtotal<goal/2){threshold-=50;break;}
      if(++step==0){
       if(++after>=widths.Length)throw new NotSupportedException("SCORE's turn search reads past the bar table.");
       high+=widths[after];subtotal=high;end=after;
      }else{
       if(step>=groupPages)step=-1;
       if(--before<0)throw new NotSupportedException("SCORE's turn search reads before the bar table.");
       low-=widths[before];subtotal=low;end=before;
      }
     }
     if(gaps[end]>=threshold)break;
    }
    total-=subtotal;
    if(total<=200)end=widths.Length-1;
    int count=end-start+1;
    if(count<groupPages*linesPerPage)throw new NotSupportedException("SCORE's turn proposal has an empty page.");
    var layout=planner.Apply(widths.Skip(start).Take(count).ToArray(),leading.Skip(start).Take(count).ToArray(),groupPages*linesPerPage,linesPerPage,height,1);
    result.AddRange(NativePageDefaultPartPlan.GroupLines(layout,count));
    start=end+1;groupPages=2;
   }
   return result;
  }
 }
}
