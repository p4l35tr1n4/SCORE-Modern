using System;

using System.IO;

using System.Text;

using System.Text.RegularExpressions;

using System.Drawing;

using System.Drawing.Drawing2D;

using System.Collections.Generic;

using System.Windows.Forms;

using ScoreModern.RecoveredFragments;

namespace ScoreModern {

 public static class NativeTupletFonts {

  static readonly Dictionary<string,Type1Outline> cache=new Dictionary<string,Type1Outline>();

  public static int ResourceRevision{get;private set;}public static string DirectoryPath{get;private set;}public static string Status{get;private set;}

  static string Settings{get{return "type1";}}

  public static void Load(){Status="Select external Type-1 font folder";if(NativeSettings.Exists(Settings)&&!String.IsNullOrWhiteSpace(NativeSettings.ReadAllText(Settings)))SetDirectory(NativeSettings.ReadAllText(Settings).Trim(),false);}

  public static void SetDirectory(string path,bool save){if(!Directory.Exists(path))throw new DirectoryNotFoundException(path);DirectoryPath=Path.GetFullPath(path);ResourceRevision++;cache.Clear();GhostscriptTextRenderer.Clear();FontVersionChoices.Pending.Clear();FontVersionChoices.Available.Clear();Status=DirectoryPath;if(save)NativeSettings.WriteAllText(Settings,DirectoryPath);foreach(Form f in Application.OpenForms)f.Invalidate(true);}

  public static void AddMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("Type 1 fonts...");item.Click+=(s,e)=>{using(var d=new FolderBrowserDialog{Description="Choose the folder containing your external PostScript Type-1 fonts",SelectedPath=DirectoryPath??""})if(d.ShowDialog()==DialogResult.OK){SetDirectory(d.SelectedPath,true);var editor=(menu.Tag as Editor)??(menu.SourceControl==null?null:menu.SourceControl.FindForm() as Editor);if(editor!=null)editor.QueuePscOfferAfterFontSelection();}};menu.Items.Add(item);var report=new ToolStripMenuItem("Font resources...");report.Click+=(s,e)=>{Editor editor=(menu.Tag as Editor)??(menu.SourceControl==null?null:menu.SourceControl.FindForm() as Editor);if(editor!=null)editor.ShowFontResources();};menu.Items.Add(report);var versions=new ToolStripMenuItem("Choose duplicate Type 1 fonts...");versions.Click+=(s,e)=>{var editor=(menu.Tag as Editor)??(menu.SourceControl==null?null:menu.SourceControl.FindForm() as Editor);if(editor!=null)editor.ShowFontVersionChoices();};menu.Items.Add(versions);var generate=new ToolStripMenuItem("FONTCONV...");generate.Click+=(s,e)=>{var editor=(menu.Tag as Editor)??(menu.SourceControl==null?null:menu.SourceControl.FindForm() as Editor);if(editor!=null)editor.ShowPscGenerator(null);};menu.Items.Add(generate);menu.Opening+=(s,e)=>item.ToolTipText=Status;}

  public static void ClearResolvedFonts(){ResourceRevision++;cache.Clear();GhostscriptTextRenderer.Clear();}
  internal static string[] Type1Files(string folder){var result=new List<string>();foreach(string file in Directory.EnumerateFiles(folder,"*",SearchOption.AllDirectories)){string ext=Path.GetExtension(file);if(ext.Equals(".pfb",StringComparison.OrdinalIgnoreCase)||ext.Equals(".pfa",StringComparison.OrdinalIgnoreCase))result.Add(file);}result.Sort(StringComparer.OrdinalIgnoreCase);return result.ToArray();}
  public static string ResolvePath(string name){

   if(String.IsNullOrEmpty(name)||String.IsNullOrEmpty(DirectoryPath))throw new FileNotFoundException("Select the external Type 1 font folder. No Windows substitute is approved.");

   var files=Type1Files(DirectoryPath);Array.Sort(files,StringComparer.OrdinalIgnoreCase);var candidates=new List<string>();
   foreach(string path in files){byte[] bytes=File.ReadAllBytes(path);string header=Encoding.GetEncoding(28591).GetString(bytes,0,Math.Min(bytes.Length,8192));var match=Regex.Match(header,@"/FontName\s+/([^\s]+)");if(match.Success&&match.Groups[1].Value==name)candidates.Add(path);}
   if(candidates.Count==0)throw new FileNotFoundException("External Type 1 font missing: "+name+". Supply the original PFB/PFA or explicitly approve a specific last-resort substitute. TTF/OTF substitution is disabled.");
   if(candidates.Count==1)return candidates[0];FontVersionChoices.Available[name]=candidates.ToArray();string chosen=FontVersionChoices.Selected(name,candidates);if(chosen!=null)return chosen;FontVersionChoices.Pending[name]=candidates.ToArray();throw new InvalidDataException("Several Type 1 files declare "+name+". Choose a version from the font selection dialog; no version was chosen automatically.");
  }

  public static Type1Outline Find(string name){RenderResourceAudit.Record("Type-1 font lookup",name,DirectoryPath,"external font request");if(String.IsNullOrEmpty(name)||String.IsNullOrEmpty(DirectoryPath)){Status="Type 1 font unavailable: select its external folder; no Windows substitute is approved";RenderResourceAudit.Unavailable(name,Status);return null;}Type1Outline font;if(cache.TryGetValue(name,out font)){if(font==null)RenderResourceAudit.Unavailable(name,Status);return font;}

   try{font=new Type1Outline(ResolvePath(name));cache[name]=font;Status=name+" — "+DirectoryPath;return font;}catch(IOException error){Status=error.Message;RenderResourceAudit.Unavailable(name,Status);return null;}catch(InvalidDataException error){Status=error.Message;RenderResourceAudit.Unavailable(name,Status);return null;}

  }

  public static bool Draw(Graphics g,string face,short number,float x,float y,float em,bool uniform=false){

   PrintedFontCarry.Select(face,em,uniform?em:em*30/35);

   if(GhostscriptTextRenderer.TryDrawNumber(g,face,number,x,y,em,uniform))return true;

   Type1Outline font=Find(face);if(font==null)throw new FileNotFoundException(Status);

   var paths=new List<GraphicsPath>();var widths=new List<float>();

   try{

    foreach(char c in TupletGeometry.NumberText(number)){float width;string glyph=NativeFontInitialization.Glyph(NativePrintContext.Root,c,null);var path=glyph==null?font.Character(c,out width):font.NamedCharacter(glyph,false,out width);paths.Add(path);widths.Add(width);}

    float advance=0;for(int n=0;n<paths.Count;n++){using(var matrix=new Matrix(em/30000,0,0,-em/(uniform?30000:35000),x+advance*em/30000,y)){paths[n].Transform(matrix);if(paths[n].PointCount>0)g.VectorFillPath(Brushes.Black,paths[n]);}advance+=widths[n];}return true;

   }catch(InvalidDataException error){Status=error.Message;throw;}

   finally{foreach(var path in paths)path.Dispose();}

  }

 }

}





