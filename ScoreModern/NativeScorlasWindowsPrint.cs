using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeScorlasPrintPage {
  internal string PostScript,Image;internal float WidthPoints,HeightPoints;
 }
 internal static class NativeScorlasWindowsPrint {
  static string Q(string value){return "\""+value.Replace("\"","\\\"")+"\"";}
  internal static NativeScorlasPrintPage Describe(string postscript,NativePrintJob settings){
   string text=File.ReadAllText(postscript,Encoding.GetEncoding(28591));float width=settings.PaperWidth*72,height=settings.PaperHeight*72;
   var requests=Regex.Matches(text,@"statusdict /setpage known \{statusdict begin\s+(\d+)\s+(\d+)\s+1 setpage end\} if");
   if(requests.Count>0){var last=requests[requests.Count-1];width=Single.Parse(last.Groups[1].Value,CultureInfo.InvariantCulture);height=Single.Parse(last.Groups[2].Value,CultureInfo.InvariantCulture);}
   if(width<=0||height<=0||width>14400||height>14400)throw new NotSupportedException("The requested printer page size is outside the supported range.");
   return new NativeScorlasPrintPage{PostScript=postscript,WidthPoints=width,HeightPoints=height};
  }
  internal static void Rasterize(NativeScorlasPrintPage page,string image,int dpi){
   string executable=GhostscriptTextRenderer.Executable;if(!File.Exists(executable))throw new FileNotFoundException("Select the Ghostscript console executable before Windows batch printing. PS/EPS export remains available.",executable);
   if(dpi<72||dpi>2400)throw new NotSupportedException("Windows SCORLAS printing supports 72–2400 dpi.");
   long width=(long)Math.Round(page.WidthPoints*dpi/72),height=(long)Math.Round(page.HeightPoints*dpi/72);if(width*height>100000000)throw new NotSupportedException("This printer raster exceeds 100 million pixels. Reduce print resolution or page size.");
   string source=File.ReadAllText(page.PostScript,Encoding.GetEncoding(28591)),mapFile=image+".Fontmap";var fonts=new HashSet<string>(StringComparer.Ordinal);var map=new StringBuilder();
   foreach(Match match in Regex.Matches(source,@"/([A-Za-z0-9_.-]+)\s+findfont")){string name=match.Groups[1].Value;if(name=="SMfont"||name=="SMaccent"||name=="SMspecial"||name=="SMinitial"||!fonts.Add(name))continue;string path=NativeTupletFonts.ResolvePath(name);map.Append('/').Append(name).Append(" (").Append(path.Replace("\\","/").Replace("(","\\(").Replace(")","\\)")).Append(") ;\n");}
   File.WriteAllText(mapFile,map.ToString(),Encoding.ASCII);
   try{
    var info=new ProcessStartInfo(executable,"-dSAFER -dBATCH -dNOPAUSE -sDEVICE=pnggray -dFIXEDMEDIA -r"+dpi+" -g"+width+"x"+height+" -dTextAlphaBits=4 -dGraphicsAlphaBits=4 "+Q("-sFONTMAP="+mapFile)+" "+Q("-sOutputFile="+image)+" "+Q(page.PostScript)){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};var log=new StringBuilder();
    using(var process=new Process{StartInfo=info}){process.OutputDataReceived+=(s,e)=>{if(e.Data!=null)lock(log)log.AppendLine(e.Data);};process.ErrorDataReceived+=(s,e)=>{if(e.Data!=null)lock(log)log.AppendLine(e.Data);};process.Start();process.BeginOutputReadLine();process.BeginErrorReadLine();GhostscriptTextRenderer.WaitForProcess(process,"SCORLAS printer-page rendering timed out.");if(process.ExitCode!=0||log.ToString().IndexOf("substitut",StringComparison.OrdinalIgnoreCase)>=0)throw new InvalidOperationException("SCORLAS printer-page rendering failed or attempted font substitution: "+log);}
    using(var bitmap=new Bitmap(image))if(bitmap.Width!=width||bitmap.Height!=height)throw new InvalidDataException("The printer raster has an unexpected size.");page.Image=image;
   }finally{File.Delete(mapFile);}
  }
  internal static void Draw(Graphics graphics,NativeScorlasPrintPage page,float hardX,float hardY){
   var state=graphics.Save();try{graphics.PageUnit=GraphicsUnit.Inch;graphics.TranslateTransform(-hardX/100,-hardY/100);graphics.InterpolationMode=InterpolationMode.NearestNeighbor;graphics.PixelOffsetMode=PixelOffsetMode.Half;using(var image=new Bitmap(page.Image))graphics.DrawImage(image,new RectangleF(0,0,page.WidthPoints/72,page.HeightPoints/72),new Rectangle(0,0,image.Width,image.Height),GraphicsUnit.Pixel);}finally{graphics.Restore(state);}
  }
  internal static void Show(Form owner,string root,IList<List<string>> pages,NativePrintJob settings,bool resolveInitialOffsets,bool preview){
   string folder=Path.Combine(Path.GetTempPath(),"ScoreModern-SCORLAS-print-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);var job=settings.CopyConfiguration();
   try{
    var prepared=new List<NativeScorlasPrintPage>();using(var progress=new NativeScorlasProgress()){
     owner.Enabled=false;try{progress.Show(owner);NativeScorlasBatch.ExportControlled(root,pages,Path.Combine(folder,"PAGE01.ps"),job,true,resolveInitialOffsets,()=>progress.Cancelled,progress.Report);
      var names=NativeScorlasBatch.OutputNames(Path.Combine(folder,"PAGE01.ps"),pages.Count,true);for(int n=0;n<names.Length;n++){progress.Report(n,names.Length);if(progress.Cancelled)throw new OperationCanceledException("SCORLAS printing cancelled before sending pages to a printer.");var page=Describe(names[n],job);Rasterize(page,Path.Combine(folder,"page"+n+".png"),job.Resolution);prepared.Add(page);}if(progress.Cancelled)throw new OperationCanceledException("SCORLAS printing cancelled before sending pages to a printer.");
     }finally{owner.Enabled=true;progress.Close();}
    }
    using(var document=new PrintDocument()){
     document.DocumentName="SCORE Modern SCORLAS";document.OriginAtMargins=false;int index=0;document.BeginPrint+=(s,e)=>index=0;
     document.QueryPageSettings+=(s,e)=>{var page=prepared[index];e.PageSettings.Landscape=false;e.PageSettings.PaperSize=new PaperSize("SCORLAS page "+(index+1),(int)Math.Round(page.WidthPoints*100/72),(int)Math.Round(page.HeightPoints*100/72));e.PageSettings.Margins=new Margins(0,0,0,0);};
     document.PrintPage+=(s,e)=>{Draw(e.Graphics,prepared[index],e.PageSettings.HardMarginX,e.PageSettings.HardMarginY);index++;e.HasMorePages=index<prepared.Count;};
     if(preview){using(var dialog=new PrintPreviewDialog{Document=document,Width=1000,Height=750,UseAntiAlias=true})dialog.ShowDialog(owner);}
     else{
      if((job.Tray!=0||job.Manual)&&MessageBox.Show(owner,"SCORE requests "+(job.Manual?"manual feed":"tray "+job.Tray)+". Select the corresponding paper source in printer properties.","SCORLAS printer settings",MessageBoxButtons.OKCancel,MessageBoxIcon.Information)!=DialogResult.OK)return;
      using(var dialog=new PrintDialog{Document=document,UseEXDialog=true,AllowSomePages=false})if(dialog.ShowDialog(owner)!=DialogResult.OK)return;document.Print();
     }
    }
   }finally{foreach(string file in Directory.GetFiles(folder))File.Delete(file);Directory.Delete(folder);}
  }
 }
}
