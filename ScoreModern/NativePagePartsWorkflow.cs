using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

namespace ScoreModern {
 internal sealed class NativePagePreparedPart {
  internal readonly IList<NativePagePartPage> Plan;
  internal readonly IList<byte[]> Files;
  internal readonly string[] Sources;
  internal readonly IList<string> Warnings;
  internal NativePagePreparedPart(IList<NativePagePartPage> plan,IList<byte[]> files,string[] sources,IList<string> warnings){Plan=plan;Files=files;Sources=sources;Warnings=warnings;}
 }

 internal static class NativePagePartsWorkflow {
  // The automatic Parts route has its own original working banks and PAG
  // writer. Keep it separate from Main SCORE Save and from SCORLAS tiling.
  internal static NativePagePreparedPart Prepare(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size){
   return PrepareWithHeight(paths,instrument,staves,omitLeft,size,10f);
  }
  internal static NativePagePreparedPart PrepareWithHeight(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height){
   return PrepareWithLayout(paths,instrument,staves,omitLeft,size,height,0,0);
  }
  internal static NativePagePreparedPart PrepareWithLayout(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent){
   return PrepareWithNumbering(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,1,false);
  }
  internal static NativePagePreparedPart PrepareWithNumbering(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered){
   return PrepareWithRestLabels(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,true);
  }
  internal static NativePagePreparedPart PrepareWithRestLabels(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest){
   return PrepareWithRestControls(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,false);
  }
  internal static NativePagePreparedPart PrepareWithRestControls(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests){
   return PrepareWithMeasureNumbers(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,0,false);
  }
  internal static NativePagePreparedPart PrepareWithMeasureNumbers(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure){
   return PrepareWithLinePlan(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,"");
  }
  internal static NativePagePreparedPart PrepareWithLinePlan(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines){
   return PrepareWithPagePlan(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,"");
  }
  internal static NativePagePreparedPart PrepareWithPagePlan(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage){
   return PrepareFixedProposal(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,0,0);
  }
  internal static NativePagePreparedPart PrepareFixedProposal(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage){
   return PrepareBarRange(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,pageCount,fixedLinesPerPage,0,0);
  }
  internal static NativePagePreparedPart PrepareBarRange(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage,int fromBar,int toBar){
   return PrepareWithTranspose(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,pageCount,fixedLinesPerPage,fromBar,toBar,false);
  }
  internal static NativePagePreparedPart PrepareWithTranspose(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage,int fromBar,int toBar,bool transposeWholeTone){
   return PrepareWithInterval(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,pageCount,fixedLinesPerPage,fromBar,toBar,transposeWholeTone?(short)2:(short)0);
  }
  internal static NativePagePreparedPart PrepareWithInterval(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage,int fromBar,int toBar,short interval){
   return PrepareWithNormalization(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,pageCount,fixedLinesPerPage,fromBar,toBar,interval,null);
  }
  internal static NativePagePreparedPart PrepareWithNormalization(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage,int fromBar,int toBar,short interval,string normalizationRoot){
   return PrepareWithTransposition(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,pageCount,fixedLinesPerPage,fromBar,toBar,interval,normalizationRoot,false);
  }
  internal static NativePagePreparedPart PrepareWithTransposition(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage,int fromBar,int toBar,short interval,string normalizationRoot,bool addKey,short? targetClef=null){
   return PrepareWithMissingInstrument(paths,instrument,staves,omitLeft,size,height,firstIndent,otherIndent,firstPage,centered,numberSingleBarRest,europeanRests,startMeasure,everyMeasure,manualLines,linesPerPage,pageCount,fixedLinesPerPage,fromBar,toBar,interval,normalizationRoot,addKey,targetClef,null);
  }
  internal static NativePagePreparedPart PrepareWithMissingInstrument(IEnumerable<string> paths,int instrument,int staves,bool omitLeft,float size,float height,int firstIndent,int otherIndent,int firstPage,bool centered,bool numberSingleBarRest,bool europeanRests,int startMeasure,bool everyMeasure,string manualLines,string linesPerPage,int pageCount,int fixedLinesPerPage,int fromBar,int toBar,short interval,string normalizationRoot,bool addKey,short? targetClef,Func<int,NativePageParts.MissingInstrumentOpening> chooseOpening,bool scor4=false){
   if(paths==null)throw new ArgumentNullException("paths");
   var sources=paths.Select(Path.GetFullPath).ToArray();if(sources.Length==0)throw new ArgumentException("Choose at least one score file.");
   if(instrument<1||instrument>32767)throw new ArgumentException("Enter an instrument ID from 1 to 32767.");
   if(staves<1||staves>32)throw new ArgumentException("Enter a staff count from 1 to 32.");
   if(size<=0||Single.IsInfinity(size)||Single.IsNaN(size))throw new ArgumentException("Enter a positive staff size.");
   if(height<=0||Single.IsInfinity(height)||Single.IsNaN(height))throw new ArgumentException("Enter a positive page height.");
   if(firstIndent<0||firstIndent>=200||otherIndent<0||otherIndent>=200)throw new ArgumentException("Enter SCORE indents between 0 and 199.");
   if(startMeasure!=0&&(startMeasure< -1||startMeasure>32767||startMeasure==-1&&everyMeasure))
    throw new NotSupportedException("PAGE measure numbering requires a signed-word start. Pickup numbering applies to line openings.");
   bool manual=!String.IsNullOrWhiteSpace(manualLines),regroup=!String.IsNullOrWhiteSpace(linesPerPage);
   if(manual&&regroup||(manual||regroup)&&(pageCount!=0||fixedLinesPerPage!=0))throw new ArgumentException("Choose one manual PAGE layout method.");
   var parts=new List<Score>();short? measurement=null;bool mixedMeasurements=false;
   foreach(string path in sources){
    var score=Score.Load(path);
    short selectedMeasurement=NativeMeasurements.Get(score);
    mixedMeasurements|=measurement.HasValue&&measurement.Value!=selectedMeasurement;
    measurement=selectedMeasurement;
    var segments=NativePageParts.PrepareStaffGroupsWithMissing(score,instrument,staves,omitLeft,chooseOpening,parts.Count==0,scor4);
    parts.AddRange(segments.Select(x=>x.Score));
   }
   if(parts.Count==0)throw new InvalidDataException("No staves have the requested instrument ID in P9.");
   bool range=fromBar!=0||toBar!=0;int skip=0,take=0;
   if(range){
    int actual=NativePagePartWeightCollection.Prepare(parts).BarPlan.FileBars.Sum();
    if(fromBar<1||toBar<fromBar||toBar>actual)throw new NotSupportedException("PAGE range must select existing measures in forward order.");
    skip=fromBar-1;take=toBar-fromBar+1;
   }
   var plan=!manual?NativePageDefaultPartPlan.PrepareRangeProposal(parts,staves,size,height,firstPage,centered,skip,take,pageCount,fixedLinesPerPage,firstIndent,otherIndent):
    NativePageDefaultPartPlan.NumberedPages(ParseLines(manualLines,staves,range?take:NativePagePartWeightCollection.Prepare(parts).BarPlan.FileBars.Sum()),height,firstPage,centered);
   if(regroup)
    plan=NativePageDefaultPartPlan.NumberedPages(RegroupLines(linesPerPage,staves,plan.SelectMany(p=>p.Bars).ToArray()),height,firstPage,centered);
   for(int n=0;n<plan.Count;n++)plan[n].Indents=Enumerable.Range(0,plan[n].Bars.Length).Select(i=>(float)(n==0&&i==0?firstIndent:otherIndent)).ToArray();
   var warnings=new List<string>();
   if(mixedMeasurements)warnings.Add("Input scores use different measurement units. PAGE preserves SCORE coordinates; the output uses "+(measurement.Value==1?"centimeters":"inches")+" from the last input file.");
   var numbering=startMeasure==0?null:new NativePageMeasureNumbers(startMeasure,everyMeasure);
   if(interval!=0&&manual&&range)
    throw new NotSupportedException("PAGE's explicit line plan requires an entire transposed part.");
   if(interval==0&&(addKey||targetClef.HasValue||normalizationRoot!=null))throw new ArgumentException("Select a PAGE transposition before setting its options.");
   var pages=NativePagePartPages.AssembleTransposed(parts,staves,size,plan,new float[4],numberSingleBarRest?1f:0f,europeanRests?(short)2:(short)1,null,warnings.Add,numbering,skip,take,interval==0?null:new NativePageTranspose(interval,normalizationRoot,addKey,targetClef,scor4),scor4:scor4);
   var writer=new NativePageFileBank(new byte[14200*4]);
   // Original PAGE keeps SCORE coordinates and carries the last input's
   // measurement code to the PAG trailer (both mixed orders in source421).
   return new NativePagePreparedPart(plan,pages.Select(page=>writer.Encode(page,measurement.Value)).ToArray(),sources,warnings.AsReadOnly());
  }

  static List<int[]> RegroupLines(string text,int staves,int[] bars){
   var pages=new List<int[]>();int used=0;
   foreach(string value in text.Split(new[]{' ', '\t', '\r', '\n', ','},StringSplitOptions.RemoveEmptyEntries)){
    int n;
    if(!Int32.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out n)||n<1||n>bars.Length-used)throw new ArgumentException("Enter a positive line count for each page, covering all "+bars.Length+" automatic lines.");
    if(n>32/staves)throw new NotSupportedException("A PAGE layout exceeds 32 staves on one page.");
    pages.Add(bars.Skip(used).Take(n).ToArray());used+=n;
   }
   if(used!=bars.Length)throw new ArgumentException("The page grouping covers "+used+" of "+bars.Length+" automatic lines.");
   return pages;
  }

  static List<int[]> ParseLines(string text,int staves,int expectedBars){
   var pages=new List<int[]>();var lines=new List<int>();int used=0;
   foreach(var value in text.Split(new[]{' ', '\t', '\r', '\n', ','},StringSplitOptions.RemoveEmptyEntries)){
    int n;
    if(!Int32.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out n)||n>expectedBars-used)throw new ArgumentException("Enter a positive bar count per line, and zero at page breaks.");
    if(n==0){if(lines.Count==0)throw new ArgumentException("A PAGE layout cannot have an empty page.");pages.Add(lines.ToArray());lines.Clear();}
    else{lines.Add(n);used+=n;if(lines.Count>32/staves)throw new NotSupportedException("A PAGE layout exceeds 32 staves on one page.");}
   }
   if(lines.Count!=0)pages.Add(lines.ToArray());
   if(used!=expectedBars)throw new ArgumentException("The manual PAGE layout accounts for "+used+" of "+expectedBars+" bars.");
   return pages;
  }

  internal static string[] OutputNames(NativePagePreparedPart part,string first,bool original){
   if(part==null)throw new ArgumentNullException("part");
   if(!String.Equals(Path.GetExtension(first),".PAG",StringComparison.OrdinalIgnoreCase))throw new ArgumentException("Choose a .PAG output filename.");
   var names=original?NativePageFilenameSequence.OriginalNames(first,part.Files.Count):part.Files.Count==1?new[]{Path.GetFullPath(first)}:NativeScorlasBatch.OutputNames(first,part.Files.Count,true);
   var sourcePaths=new HashSet<string>(part.Sources,StringComparer.OrdinalIgnoreCase);
   if(names.Any(sourcePaths.Contains))throw new IOException("A PAGE output would replace one of the input scores.");
   if(names.Distinct(StringComparer.OrdinalIgnoreCase).Count()!=names.Length)throw new IOException("The PAGE filenames are not distinct.");
   return names;
  }

  internal static void Save(NativePagePreparedPart part,string first,bool original,bool replace){
   SavePages(part,first,original,replace,part.Files);
  }
  internal static void SavePages(NativePagePreparedPart part,string first,bool original,bool replace,IList<byte[]> contents){
   var names=OutputNames(part,first,original);
   if(!replace&&names.Any(File.Exists))throw new IOException("A PAGE output already exists.");
   string folder=Path.GetDirectoryName(names[0]),token=Guid.NewGuid().ToString("N");
   var staged=new List<string>();var committed=new List<int>();var backups=new Dictionary<int,string>();bool success=false;
   try{
    for(int n=0;n<names.Length;n++){string temp=Path.Combine(folder,".page-"+token+"-"+n+".tmp");staged.Add(temp);File.WriteAllBytes(temp,contents[n]);}
    try{
     for(int n=0;n<names.Length;n++){
      if(File.Exists(names[n])){if(!replace)throw new IOException("A PAGE output already exists: "+names[n]);string backup=staged[n]+".backup";File.Replace(staged[n],names[n],backup);backups.Add(n,backup);}else File.Move(staged[n],names[n]);committed.Add(n);
     }
    }catch(Exception error){
     var failed=new List<string>();
     for(int i=committed.Count-1;i>=0;i--){int n=committed[i];try{string backup;if(backups.TryGetValue(n,out backup))File.Replace(backup,names[n],null);else File.Delete(names[n]);}catch(IOException restore){failed.Add(names[n]+": "+restore.Message);}catch(UnauthorizedAccessException restore){failed.Add(names[n]+": "+restore.Message);}}
     if(failed.Count!=0)throw new IOException("PAGE outputs could not all be restored. Remaining backups are in the output folder."+Environment.NewLine+String.Join(Environment.NewLine,failed),error);
     throw;
    }
    success=true;
   }finally{foreach(string temp in staged)if(File.Exists(temp))File.Delete(temp);if(success)foreach(string backup in backups.Values)if(File.Exists(backup))File.Delete(backup);}
  }
 }
}
