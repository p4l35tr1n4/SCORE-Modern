using System;using System.Collections.Generic;using System.Drawing;using System.Globalization;using System.Text;using System.Text.RegularExpressions;using System.IO;using System.Runtime.CompilerServices;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 internal static class NativeRawPostScript {
  internal const string Aliases="/m /moveto load def /l /lineto load def /tr /translate load def /aw /awidthshow load def\n/e /eofill load def /s /stroke load def /g /gsave load def /r /grestore load def\n/f /findfont load def /sf /setfont load def /mkf /makefont load def /lw /setlinewidth load def\n";
  static readonly HashSet<string> PureOperators=new HashSet<string>("gsave grestore moveto lineto curveto stroke closepath newpath translate rotate scale setlinewidth setgray setrgbcolor setcmykcolor setlinejoin setlinecap fill eofill arc arcn m l tr e s g r lw".Split(' '),StringComparer.Ordinal);
  [ThreadStatic]static StringBuilder definitions;
  internal static void BeginFrame(){definitions=new StringBuilder();}
  internal static string Normalize(string source){
   var output=new StringBuilder();for(int p=0;p<source.Length;p++){
    if(source[p]=='_'){int face,length;if(!ScoreText.FontControl(source,p,out face,out length)){
     if(!NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Malformed raw-font prefix. Original invalid-state reproduction is required for SCORE's unconditional prefix consumption.");
     p+=Math.Min(source.Length-p-1,p+1<source.Length&&source[p+1]=='_'?3:2);continue;
    }}output.Append(source[p]);
   }return output.ToString();
  }
  static bool Definition(string value){return Regex.IsMatch(value.Trim(),@"^/[A-Za-z][A-Za-z0-9]*\s+[-+]?(?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+)\s+def$");}
  // Original T191 emits these Code16 bytes verbatim, independently of P3/P4.
  // Self-contained blocks use the small cached preview. Programs retaining
  // graphics state use one ordered rendering of the shared SCORE EPS scene.
  internal static bool IsRaw(Item item){if(item==null||item.Code!=16)return false;if(item.P(8)==99)return true;var font=TextConverterFontState.Begin(unchecked((short)(int)item.P(8)),0);if(font.Forced)return false;string source=item.Text;for(int p=0;p<source.Length;p++){int word,length;if(ScoreText.FontControl(source,p,out word,out length)){font.Accept((short)(1000+word));if(font.Word%100==99)return true;p+=length-1;}}return false;}
  static bool Comment(Item item){string source=Source(item);if(item.P(8)!=99){source=source.TrimStart();if(!source.StartsWith("_99",StringComparison.Ordinal))return false;source=source.Substring(3);if(source.IndexOf('_')>=0)return false;}if(source.IndexOfAny(new[]{'[',']','{','}','|'})>=0)return false;foreach(string line in source.Replace("{","\n").Replace("}","\n").Split('\n')){string value=line.Trim();if(value.Length!=0&&!value.StartsWith("%",StringComparison.Ordinal))return false;}return true;}
  internal static bool RequiresScene(Score score){foreach(var item in score.Items)if(IsRaw(item)&&!IsBlock(item)&&!Comment(item))return true;return false;}
  static string Source(Item item){int count=(int)item.P(12);if(item.P(12)!=count||count<0||count>160||item.Bytes.Length<56+count)throw new NotSupportedException("Raw PostScript count exceeds the verified stored-byte domain.");return Encoding.GetEncoding(28591).GetString(item.Bytes,56,count);}
  internal static bool IsBlock(Item item){
   if(!IsRaw(item))return false;string value=Source(item).Trim();if(Comment(item)||Definition(value))return true;
   if(item.P(8)!=99||item.P(9)!=0||!value.StartsWith("gsave ",StringComparison.Ordinal)||!value.EndsWith(" grestore",StringComparison.Ordinal))return false;
   if(!Regex.IsMatch(value,@"(?:^|\s)newpath(?:\s|$)"))return false;
   foreach(string token in value.Split((char[])null,StringSplitOptions.RemoveEmptyEntries)){if(PureOperators.Contains(token))continue;double number;if(!Double.TryParse(token,NumberStyles.Float,CultureInfo.InvariantCulture,out number)||Double.IsNaN(number)||Double.IsInfinity(number))return false;}return true;
  }
  static string Style(){return "0 setgray 1 setlinejoin 0 setlinecap "+NativePrintContext.DefaultLineWidth.ToString("F4",CultureInfo.InvariantCulture)+" setlinewidth\n";}
  static string Program(Item item){var job=NativePrintJob.Active;return Aliases+(definitions==null?"":definitions.ToString())+"gsave\n"+(job==null?".018 dup scale 2100 27250 translate\n":job.RawProgram())+Style()+Source(item)+"\ngrestore\n";}
  static string sceneKey,sceneProgram,sceneFailureKey,sceneFailureMessage;static Dictionary<string,string> sceneFonts;
  static string SceneFontMap(){var map=new StringBuilder();if(sceneFonts!=null)foreach(var font in sceneFonts){var info=new FileInfo(font.Value);if(!info.Exists)throw new FileNotFoundException("Selected Type1 font was removed",font.Value);map.Append("% ").Append(info.Length).Append(" ").Append(info.LastWriteTimeUtc.Ticks).Append("\n/").Append(font.Key).Append(" (").Append(font.Value.Replace("\\","/").Replace("(","\\(").Replace(")","\\)")).Append(") ;\n");}return map.ToString();}

  internal static void DrawScene(Canvas canvas,Graphics graphics){
   if(!GhostscriptTextRenderer.Enabled)throw new NotSupportedException("Stateful PostScript text requires the optional Ghostscript renderer.");
   var preferences=NativePreferences.Read(canvas.Library.Root);var stamp=new StringBuilder();var names=new List<string>(preferences.Entries.Keys);names.Sort(StringComparer.Ordinal);foreach(string name in names)stamp.Append(name).Append(":").Append(preferences.Text(name)).Append("|");if(NativeFontInitialization.External)stamp.Append(NativeFontInitialization.Read(canvas.Library.Root).Program);
   string key=stamp.ToString()+"|"+NativeLibraryLocation.RevisionFor(canvas.Library.Root)+"|"+Convert.ToBase64String(canvas.Score.Encode())+"|"+RuntimeHelpers.GetHashCode(canvas.Library)+"|"+NativeTupletFonts.ResourceRevision+"|"+NativePrintContext.DefaultLineWidth+"|"+NativePrintContext.BeamWidth+"|"+NativePrintContext.Unit+"|"+NativeFontInitialization.External+"|"+(NativePrintJob.Active==null?"":NativePrintJob.Active.RawProgram());
   if(key==sceneFailureKey)throw new InvalidOperationException(sceneFailureMessage);
   if(key!=sceneKey){string path=Path.Combine(Path.GetTempPath(),"ScoreModern-literal-"+Guid.NewGuid().ToString("N")+".eps");try{using(var clone=new Canvas()){clone.Score=canvas.Score.Clone();clone.Library=canvas.Library;clone.TextLayout=canvas.TextLayout;using(NativeItemBounds.Suspend())NativeEpsExport.Write(clone,canvas.Library.Root,path);}sceneProgram=File.ReadAllText(path,Encoding.GetEncoding(28591));sceneFonts=new Dictionary<string,string>();foreach(Match font in Regex.Matches(sceneProgram,@"/([A-Za-z0-9_.-]+)\s+findfont")){string name=font.Groups[1].Value;if(name=="SMfont"||name=="SMaccent"||name=="SMspecial"||name=="SMinitial")continue;if(!sceneFonts.ContainsKey(name))sceneFonts[name]=NativeTupletFonts.ResolvePath(name);}sceneKey=key;sceneFailureKey=null;sceneFailureMessage=null;}catch(Exception error){sceneFailureKey=key;sceneFailureMessage=error.Message;throw;}finally{File.Delete(path);}}
   RenderResourceAudit.Record("stateful PostScript scene","Code16 P8=99","shared SCORE EPS renderer","whole scene interpreted in order by optional Ghostscript");
   PostScriptProgramPreview.Paint(graphics,sceneProgram,canvas.PageWidth,canvas.PageHeight,null,null,SceneFontMap(),canvas);
  }
  internal static void DrawItems(Graphics graphics,List<Item> items,float width,float height,ScoreText text,NativePageDocument document){
   if(NativeEpsExport.For(graphics)==null){Draw(graphics,items,width,height);return;}
   foreach(var item in items){int number=document.Contexts[item].SourceScore.Items.IndexOf(item)+1;if(NativePrintExecution.Current!=null)NativePrintExecution.Before(NativePrintExecution.Current.Canvas,graphics,number);NativeEpsExport.For(graphics).ItemMarker(number);if(RenderResourceAudit.Current!=null)RenderResourceAudit.Current.Item(item);var plan=text.RawActions(item,document.Contexts[item],GhostscriptTextRenderer.DeviceMatrix(graphics));using(NativeEpsExport.For(graphics).LiteralTextRotation(plan.X,plan.Y,plan.Rotation)){foreach(var write in plan.Writes)write(graphics);plan.Vectors(graphics);}}
  }
  internal static void Draw(Graphics graphics,List<Item> items,float width,float height){
   var eps=NativeEpsExport.For(graphics);var programs=new List<string>();var painted=new List<Item>();var all=new StringBuilder();
   foreach(var item in items){
    if(RenderResourceAudit.Current!=null)RenderResourceAudit.Current.Item(item);
    RenderResourceAudit.Record("raw PostScript block","Code16 P8=99","Code16 text bytes","original literal output; measured blocks/dictionary definitions, separate packed DAT screen route");
    if(Comment(item))continue;
    if(Definition(Source(item))){if(definitions==null)definitions=new StringBuilder();definitions.AppendLine(Source(item));if(eps!=null)eps.ScoreLiteralProgram(Source(item));continue;}
    if(eps!=null)eps.ScoreLiteralProgram(Source(item).Replace("{","\n").Replace("}","\n"));
    else{string program=Program(item);programs.Add(program);painted.Add(item);all.Append(program);}
   }
   if(eps==null&&painted.Count!=0)PostScriptProgramPreview.Paint(graphics,all.ToString(),width,height,painted,programs);
  }
 }
 internal sealed class RawTextPlan {internal readonly List<Action<Graphics>> Writes=new List<Action<Graphics>>();internal Action<Graphics> Vectors;internal float X,Y,Rotation;}
 public sealed partial class ScoreText {
  internal RawTextPlan RawActions(Item item,NativeNotationContext page,float[] world){
   var layout=PrepareLayout(item,item.Text,page.Scale,0);var tokens=PrintedRenderingTokens(layout,(short)item.P(8),0);var values=PrintedValues(layout,page.Scale);
   var origin=TextParameterPreparation.Prepare(TextParameters(item),page.Prepared.X,100,.2800000011920929f,150);
   var vectors=new PostScriptTextPlan{Layout=layout,Page=page,X=origin.Origin,Y=page.Prepared.Y,Staff=page.Scale};var result=new RawTextPlan{X=NativeNotationContext.RawWord(origin.Origin),Y=NativeNotationContext.RawWord(page.Prepared.Y),Rotation=values.Rotation};layout.Parameters.Rotation=0;values.Rotation=0;var program=new StringBuilder();float x=origin.Origin;PostScriptFontRun current=null;
   Action flush=()=>{if(program.Length!=0){string value=program.ToString();result.Writes.Add(g=>NativeEpsExport.For(g).ScoreLiteralProgram(value));program.Clear();}};
   foreach(var token in tokens){
    if(token.TabX.HasValue){flush();x=token.TabX.Value;current=null;continue;}if(token.AdvanceOnly){x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);continue;}
    if(token.Code>=1000){flush();current=null;continue;}if(PrintedModifier(token.Code))continue;
    if(token.Code<0)throw new NotSupportedException("Raw PostScript contains an unverified SCORE character control.");
    if(PrintedMusic(token.Code)||token.FontWord%100>=90&&token.FontWord%100!=99){program.Append('\n');current=null;vectors.Parts.Add(new PostScriptTextPart{Vector=token,X=x});}
    else if(token.FontWord%100==99){current=null;if(token.Character<32||token.Character>126||token.Code==800)throw new NotSupportedException("Extended character in SCORE raw PostScript font99 is not verified.");program.Append(token.Character);}
    else {
     flush();if(token.Character>255)throw new NotSupportedException("SCORE Type1 character exceeds byte range.");
     if(current==null||current.RequestedWord!=token.FontWord){var selected=PrintedFontCarry.Select(MissingPrintedFace(token.FontWord%100)?null:ExternalFontName(token.FontWord%100),values.EmX,values.EmY,token.FontWord>=100);current=new PostScriptFontRun{Name=selected.Name,RequestedWord=token.FontWord,Word=(short)(token.FontWord%100+(selected.Outline?100:0)),X=NativeNotationContext.RawWord(x),Y=NativeNotationContext.RawWord(page.Prepared.Y),EmX=selected.EmX,EmY=selected.EmY,Letter=values.Letter,Space=values.Space,Rotation=values.Rotation,PivotX=NativeNotationContext.RawWord(origin.Origin),PivotY=NativeNotationContext.RawWord(page.Prepared.Y),World=world};var font=current;result.Writes.Add(g=>GhostscriptTextRenderer.Paint(g,font));}
     current.Bytes.Add((byte)token.Character);if(token.GlyphName!=null)current.ExtraEncoding[(byte)token.Character]=token.GlyphName;
    }
    x=TextMetricArithmetic.AddPosition(1,x,token.AdjustedAdvance,150);
   }
   flush();result.Vectors=g=>PaintPostScript(g,vectors);return result;
  }
 }
}
