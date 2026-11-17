using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScoreModern {
 internal static class NativePageScoreLayout {
  internal static NativePagePreparedPart Prepare(IEnumerable<string> paths,int staves,float size,float height,int firstPage,bool centered){
   string[] sources=paths.Select(Path.GetFullPath).ToArray();
   if(sources.Length==0)throw new ArgumentException("Choose at least one input score.");
   return PrepareSources(sources,staves,size,height,firstPage,centered,null,null,null);
  }

  internal static NativePagePreparedPart PrepareOld(string path){
   path=Path.GetFullPath(path);
   var rows=File.ReadAllLines(path).Where(line=>!String.IsNullOrWhiteSpace(line)).ToArray();
   if(rows.Length!=8)throw new InvalidDataException("Old Page Data has an unexpected row count.");
   Func<string,string[]> fields=line=>line.Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
   var header=fields(rows[0]);var counts=fields(rows[1]);var names=fields(rows[2]);var weights=fields(rows[3]);var positions=fields(rows[4]);var units=fields(rows[6]);
   int files,staves,maximum,totalBars,count;
   if(header.Length!=5||!Int32.TryParse(header[0],out files)||files<1||files!=names.Length||files!=counts.Length||
    !Int32.TryParse(header[1],out totalBars)||totalBars!=weights.Length||!Int32.TryParse(header[2],out maximum)||maximum!=32||
    !Int32.TryParse(header[3],out staves)||staves<1||staves>32||units.Length!=32||weights.Length<1||weights.Length>1000||positions.Length!=weights.Length||
    names.Any(name=>Path.GetFileName(name)!=name||name.IndexOf(':')>=0)||
    counts.Any(value=>!Int32.TryParse(value,out count)||count<1||count>1000)||counts.Sum(value=>Int32.Parse(value))!=weights.Length)
    throw new InvalidDataException("Old Page Data has invalid source file or staff fields.");
   float firstUnit;
   if(!Single.TryParse(units[0],System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out firstUnit)||
    !(firstUnit>0)||Single.IsInfinity(firstUnit))throw new InvalidDataException("Old Page Data has an invalid staff size.");
   var savedWidths=new short[weights.Length];
   var savedPositions=new float[positions.Length];
   var culture=System.Globalization.CultureInfo.InvariantCulture;
   for(int n=0;n<weights.Length;n++){
    if(!Int16.TryParse(weights[n],out savedWidths[n])||
     !Single.TryParse(positions[n],System.Globalization.NumberStyles.Float,culture,out savedPositions[n])||
     Single.IsNaN(savedPositions[n])||Single.IsInfinity(savedPositions[n]))
     throw new InvalidDataException("Old Page Data has invalid bar widths or positions.");
   }
   var sources=names.Select(name=>Path.Combine(Path.GetDirectoryName(path),name)).ToArray();
   return PrepareSources(sources,staves,firstUnit/25,10,1,false,savedWidths,savedPositions,counts.Select(value=>Int32.Parse(value)).ToArray());
  }

  static NativePagePreparedPart PrepareSources(string[] sources,int staves,float size,float height,int firstPage,bool centered,short[] savedWidths,float[] savedPositions,int[] savedFileBars){
   short measurement=0;
   var systems=new List<Score>();
   var weights=new List<short>();
   var leading=new List<short>();
   float scale=(float)((double)size*.01f);
   for(int file=0;file<sources.Length;file++){
    int firstBar=weights.Count;
    var score=Score.Load(sources[file]);
    measurement=NativeMeasurements.Get(score);
    var staffNumbers=score.Items.Where(item=>item.Code==8).Select(item=>item.Staff).OrderBy(n=>n).ToArray();
    if(staves<1||staffNumbers.Length<staves||staffNumbers.Length%staves!=0||
     staffNumbers.Where((number,index)=>number!=index+1).Any()||
     score.Items.Any(item=>item.P(2)!=item.Staff||item.Staff<1||item.Staff>staffNumbers.Length))
     throw new NotSupportedException("PAGE layout requires complete groups of consecutively numbered staves.");
    // PAGE writes DECAA.TMP onward from the top system down. Only the
    // staff number changes in each extracted record before the PAGE sort.
    for(int high=staffNumbers.Length;high>=staves;high-=staves){
     int low=high-staves;
     var rows=score.Items.Where(item=>item.Staff>low&&item.Staff<=high).Select(item=>{
      var copy=item.Clone();
      copy.Set(2,item.Staff-low);
      return copy;
     }).ToList();
     foreach(var header in rows.Where(item=>item.Code==8&&item.P(5)==0))header.Set(5,1);
     var system=new Score{Items=NativePageSort.Apply(rows,false),Trailer=(byte[])score.Trailer.Clone()};
     systems.Add(system);
     var spacing=NativePageBarPlan.PrepareLayoutFile(system);
     if(spacing.Widths.Length!=NativePageBarPlan.Prepare(new[]{system}).FileBars[0]||
      spacing.Widths.Any(width=>width<0))
      throw new NotSupportedException("PAGE score spacing needs a matching unmarked rhythm and spacing plan.");
     for(int n=0;n<spacing.Widths.Length;n++){
      int weight=spacing.Widths[n];
      if(savedWidths!=null&&(weights.Count>=savedWidths.Length||weight!=savedWidths[weights.Count]||
       Math.Abs((double)spacing.Positions[n]-savedPositions[weights.Count])>.0051))
       throw new InvalidDataException("The source score no longer matches Old Page Data.");
      weights.Add(checked((short)Math.Truncate((double)weight*scale+.5)));
      leading.Add(checked((short)(n<spacing.Leading.Length?spacing.Leading[n]:0)));
     }
    }
    if(savedFileBars!=null&&weights.Count-firstBar!=savedFileBars[file])
     throw new InvalidDataException("The source score no longer matches its saved bar count.");
   }
   if(savedWidths!=null&&weights.Count!=savedWidths.Length)throw new InvalidDataException("The source score no longer matches Old Page Data.");
   int total=weights.Sum(weight=>(int)weight);
   if(total<1||total>32767)throw new NotSupportedException("PAGE total weight leaves its positive signed-word domain.");
   int lines=Math.Max(1,(int)(total/200.0+.6f));
   int capacity=Math.Max(1,(int)((double)height/(float)((double)staves*size)));
   var proposal=new NativePageAutomaticLayout().Apply(weights.ToArray(),leading.ToArray(),lines,capacity,height,1);
   var groups=new List<int[]>();
   for(int n=0;n<proposal.Bars.Length;n+=capacity)
    groups.Add(proposal.Bars.Skip(n).Take(capacity).Select(count=>(int)count).ToArray());
   var plan=NativePageDefaultPartPlan.NumberedPages(groups,height,firstPage,centered);
   var warnings=new List<string>();
   var pages=NativePagePartPages.AssembleTransposed(systems,staves,size,plan,new float[4],1,(short)1,null,warnings.Add,null,0,0,null,true,savedPositions);
   var writer=new NativePageFileBank(new byte[14200*4]);
   return new NativePagePreparedPart(plan,pages.Select(page=>writer.Encode(page,measurement)).ToArray(),sources,warnings.AsReadOnly());
  }
 }
}
