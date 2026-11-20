using System;using System.Linq;using System.Collections.Generic;using System.Drawing.Printing;using System.IO;using System.Drawing;using System.Drawing.Drawing2D;using System.Windows.Forms;
namespace ScoreModern {
 public sealed partial class Editor {
  NativePrintJob printJob;
  static PaperSource ManualSource(PrinterSettings settings){
   return settings.PaperSources.Cast<PaperSource>().FirstOrDefault(source=>
    source.Kind==PaperSourceKind.Manual||source.Kind==PaperSourceKind.ManualFeed);
  }
  void Print(){
   if(score==null)return;
   if(printJob==null)printJob=NativePrintJob.Defaults(source);
   var job=printJob;
   job.Resolve(canvas);
   job.ValidateOutput();
   var captions=new NativePrintCaptions(job,score);
   float pageHeight=job.PaperHeight*72;
   using(var document=new PrintDocument()){
    document.DocumentName=String.IsNullOrEmpty(current)?"SCORE":Path.GetFileName(current);
    document.DefaultPageSettings.PaperSize=new PaperSize("SCORE",
     (int)Math.Round(job.PaperWidth*100),(int)Math.Round(job.PaperHeight*100));
    if(job.Manual&&document.PrinterSettings.IsValid){
     var manual=ManualSource(document.PrinterSettings);
     if(manual!=null)document.DefaultPageSettings.PaperSource=manual;
    }
    document.PrintPage+=(sender,args)=>{
     var g=args.Graphics;
     var state=g.Save();
     try{
      g.PageUnit=GraphicsUnit.Point;
      g.TranslateTransform(-args.PageSettings.HardMarginX*.72f,-args.PageSettings.HardMarginY*.72f);
      g.SmoothingMode=SmoothingMode.AntiAlias;
      captions.Draw(g,pageHeight);
      RenderControlledPrintPage(g,job,()=>args.Cancel=true);
     }catch{args.Cancel=true;throw;}
     finally{g.Restore(state);}
    };
    var driverNotes=new List<string>();
    if(job.Tray!=0)driverNotes.Add("SCORE requests PostScript tray "+job.Tray+
     ". Select the corresponding paper source in the Windows printer properties.");
    if(job.Manual)driverNotes.Add("SCORE requests manual feed. Check the selected printer's paper source.");
    if(driverNotes.Count!=0&&MessageBox.Show(this,String.Join("\r\n",driverNotes.ToArray()),
     "SCORE print settings",MessageBoxButtons.OKCancel,MessageBoxIcon.Information)!=DialogResult.OK)return;
    using(var dialog=new PrintDialog{Document=document,UseEXDialog=true}){
     if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    }
    using(var bitmap=new Bitmap(1,1))
    using(var proof=Graphics.FromImage(bitmap))
    using(job.Begin()){
     captions.Draw(proof,pageHeight);
     NativeNotationView.Print(canvas,proof);
    }
    var warnings=new List<string>(canvas.ApproximatedItems);
    if(job.OffsetWarning!=null)warnings.Insert(0,job.OffsetWarning);
    if(warnings.Count==0||MessageBox.Show(this,String.Join("\r\n",warnings.ToArray()),
     "SCORE Modern print warnings",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning)==DialogResult.OK)document.Print();
   }
  }
  void RenderControlledPrintPage(Graphics graphics,NativePrintJob job,Action cancel){
   Action render=()=>{using(job.Begin())using(var matrix=job.CanvasMatrix()){graphics.MultiplyTransform(matrix,MatrixOrder.Prepend);NativeNotationView.Print(canvas,graphics);}};
   if(mode==ScreenMode.Print&&UsesOriginalPrintMenu)ExecuteControlledPrint(()=>{try{render();}catch(NativePrintExitException){cancel();throw;}});else render();
  }
  void LoadPrintSettings(){using(var dialog=new OpenFileDialog{Title="Read SCORE print parameters",Filter="SCORE print settings (*.SCR)|*.SCR|All files (*.*)|*.*",FileName="PRINT.SCR",InitialDirectory=NativeLibraryLocation.DirectoryFor(source,false)})if(dialog.ShowDialog(this)==DialogResult.OK){var loaded=NativePrintJob.Read(dialog.FileName,printJob);printJob=loaded;RefreshPageFrame();Message("Print parameters read from "+loaded.Source+"\r\nSize="+loaded.Size+" Paper="+loaded.PaperWidth+" x "+loaded.PaperHeight+" Freeze="+loaded.Freeze+" Rotate="+loaded.Rotate+"\r\n"+"Settings are applied to EPS and printing; unsupported output controls are reported before output.");}}
  void SavePrintSettings(){
   if(printJob==null)printJob=NativePrintJob.Defaults(source);
   using(var dialog=new SaveFileDialog{Title="Save SCORE print parameters",
    Filter="SCORE print settings (*.SCR)|*.SCR",DefaultExt="SCR",FileName="PRINT.SCR",
    InitialDirectory=NativeOutputLocation.EnsureFolder(source),RestoreDirectory=true}){
    if(dialog.ShowDialog(this)!=DialogResult.OK)return;
    string destination=NativeOutputLocation.RequireWritableDestination(source,dialog.FileName);
    printJob.Write(destination);
    Message("Print parameters saved to "+destination);
   }
  }
  string PrintExportName(){
   string output=printJob.Output;
   if(!String.IsNullOrWhiteSpace(output)&&!output.Equals("Laser",StringComparison.OrdinalIgnoreCase)){
    string file=Path.GetFileName(output);
    if(!String.IsNullOrWhiteSpace(file))return file;
   }
   return String.IsNullOrEmpty(current)?"SCORE.EPS":Path.ChangeExtension(Path.GetFileName(current),"EPS");
  }
  void ExportPrintEps(){if(printJob==null)printJob=NativePrintJob.Defaults(source);using(var dialog=new SaveFileDialog{Title="Export SCORE EPS",Filter="Encapsulated PostScript (*.EPS)|*.EPS",InitialDirectory=NativeOutputLocation.EnsureFolder(source),RestoreDirectory=true,FileName=PrintExportName()})if(dialog.ShowDialog(this)==DialogResult.OK){if(printJob!=null)printJob.Resolve(canvas);using(var scope=printJob==null?null:printJob.Begin())NativeEpsExport.Write(canvas,source,NativeOutputLocation.RequireWritableDestination(source,dialog.FileName));var message="EPS saved: "+dialog.FileName;
   if(printJob!=null&&printJob.OffsetWarning!=null)message+="\r\n"+printJob.OffsetWarning;
   if(canvas.ApproximatedItems.Count>0)message+="\r\n"+String.Join("\r\n",canvas.ApproximatedItems.ToArray());
   Message(message);}}
 }
}
