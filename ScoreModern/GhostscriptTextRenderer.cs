using System;using System.IO;using System.Text;using System.Text.RegularExpressions;using System.Diagnostics;using System.Drawing;using System.Drawing.Drawing2D;using System.Drawing.Imaging;using System.Collections.Generic;using System.Windows.Forms;
namespace ScoreModern {
 public static class GhostscriptTextRenderer {
  public static bool Enabled;public static string Executable="";public static string Status="Native vector text";public static int ProcessStarts;
  static readonly Dictionary<string,Bitmap> cache=new Dictionary<string,Bitmap>();static long pixels;
  [ThreadStatic]internal static Canvas Screen;
  static readonly HashSet<string> pending=new HashSet<string>();
  static readonly Dictionary<string,string> failures=new Dictionary<string,string>();
  static readonly Queue<Action> work=new Queue<Action>();
  static int workers;
  static volatile int generation;
  static string Settings{get{return "ghostscript";}}
  public static void Load(){if(NativeSettings.Exists(Settings)){var lines=NativeSettings.ReadAllLines(Settings);Enabled=lines.Length>0&&lines[0]=="ghostscript";if(lines.Length>1)Executable=lines[1];}}
  public static void Configure(bool enabled,string executable,bool save){Enabled=enabled;if(executable!=null)Executable=Path.GetFullPath(executable);Clear();if(save)NativeSettings.WriteAllLines(Settings,new[]{Enabled?"ghostscript":"native",Executable});foreach(Form f in Application.OpenForms)f.Invalidate(true);}
  public static void Clear(){foreach(var b in cache.Values)b.Dispose();cache.Clear();pixels=0;generation++;pending.Clear();failures.Clear();lock(work)work.Clear();}
  public static void AddMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("Text rendering");var native=new ToolStripMenuItem("Native vectors");var gs=new ToolStripMenuItem("Ghostscript (external helper)…");native.Click+=(s,e)=>Configure(false,null,true);gs.Click+=(s,e)=>{using(var d=new OpenFileDialog{Filter="Ghostscript console (gswin64c.exe;gswin32c.exe)|gswin64c.exe;gswin32c.exe",Title="Select your installed Ghostscript console executable"})if(d.ShowDialog()==DialogResult.OK)Configure(true,d.FileName,true);};item.DropDownItems.Add(native);item.DropDownItems.Add(gs);menu.Items.Add(item);menu.Opening+=(s,e)=>{native.Checked=!Enabled;gs.Checked=Enabled;item.ToolTipText=Status;};}
  internal static float[] DeviceMatrix(Graphics g){if(stableScreenClip.HasValue&&g.PageScale==1&&(g.PageUnit==GraphicsUnit.Pixel||g.PageUnit==GraphicsUnit.Display)){using(var matrix=g.Transform)return matrix.Elements;}var p=new[]{new PointF(0,0),new PointF(1,0),new PointF(0,1)};g.TransformPoints(CoordinateSpace.Device,CoordinateSpace.World,p);return new[]{p[1].X-p[0].X,p[1].Y-p[0].Y,p[2].X-p[0].X,p[2].Y-p[0].Y,p[0].X,p[0].Y};}
  [ThreadStatic]static Rectangle? stableScreenClip;
  sealed class ScreenClipScope:IDisposable {readonly Rectangle? prior;public ScreenClipScope(Rectangle clip){prior=stableScreenClip;stableScreenClip=clip;}public void Dispose(){stableScreenClip=prior;}}
  internal static IDisposable StableScreenClip(Canvas canvas,int offsetX=0,int offsetY=0){if(!Enabled||!NativeWorkspaceGuides.Rulers)return null;var clip=NativeWorkspaceGuides.Visible(canvas);clip.Offset(-offsetX,-offsetY);return new ScreenClipScope(clip);}
  internal static Rectangle Clip(Graphics g){if(stableScreenClip.HasValue)return stableScreenClip.Value;var r=g.VisibleClipBounds;var p=new[]{new PointF(r.Left,r.Top),new PointF(r.Right,r.Top),new PointF(r.Left,r.Bottom),new PointF(r.Right,r.Bottom)};g.TransformPoints(CoordinateSpace.Device,CoordinateSpace.World,p);float l=p[0].X,t=p[0].Y,b=t,right=l;foreach(var a in p){l=Math.Min(l,a.X);right=Math.Max(right,a.X);t=Math.Min(t,a.Y);b=Math.Max(b,a.Y);}return Rectangle.FromLTRB((int)Math.Floor(l),(int)Math.Floor(t),(int)Math.Ceiling(right),(int)Math.Ceiling(b));}
  internal static bool TryDraw(Graphics g,ScoreText text,Item item,NativeNotationContext page){if(!Enabled||page==null)return false;var previousFont=PrintedFontCarry.Checkpoint;try{if(pixels>32000000)Clear();var plan=text.PreparePostScript(item,page,DeviceMatrix(g));if(plan==null)return false;foreach(var part in plan.Parts)if(part.Font!=null)NativeTupletFonts.ResolvePath(part.Font.Name);if(NativeEpsExport.For(g)==null){var clip=Clip(g);bool ready=true;
   foreach(var part in plan.Parts)if(part.Font!=null)ready=Prepare(part.Font,clip,true)&&ready;
   if(!ready){PrintedFontCarry.Checkpoint=previousFont;Status="Ghostscript text rendering in background";return false;}}text.PaintPostScript(g,plan);Status="Ghostscript text; external Type-1 fonts";return true;}catch(Exception e){if(NativeFontInitialization.External)throw;Status="Native vector fallback: "+e.Message;return false;}}
  internal static bool TryDrawNumber(Graphics g,string face,short number,float x,float y,float em,bool uniform){if(!Enabled)return false;try{NativeTupletFonts.ResolvePath(face);if(pixels>32000000)Clear();var run=new PostScriptFontRun{Name=face,X=x*30,Y=-y*30,EmX=em,EmY=uniform?em:em*30/35,World=DeviceMatrix(g),PageCoordinates=false};run.Bytes.AddRange(Encoding.ASCII.GetBytes(ScoreModern.RecoveredFragments.TupletGeometry.NumberText(number)));if(NativeEpsExport.For(g)==null&&!Prepare(run,Clip(g),true)){Status="Ghostscript text rendering in background";return false;}Paint(g,run);Status="Ghostscript text; external Type-1 fonts";return true;}catch(Exception e){if(NativeFontInitialization.External)throw;Status="Native vector fallback: "+e.Message;return false;}}
  static string Escape(string s){return s.Replace("\\","/").Replace("(","\\(").Replace(")","\\)");}
  static string FontMap(string name,out string identity){string file=NativeTupletFonts.ResolvePath(name);var info=new FileInfo(file);identity=info.FullName+":"+info.Length+":"+info.LastWriteTimeUtc.Ticks;return "/"+name+" ("+Escape(info.FullName)+") ;\n";}
  static string Quote(string s){return "\""+s.Replace("\"","\\\"")+"\"";}
  internal static bool Prepare(PostScriptFontRun run,Rectangle clip,bool allowBackground=false){run.Crop=run.Bounds(clip);if(run.Crop.IsEmpty||run.Crop.Width<=0||run.Crop.Height<=0)return true;if(!File.Exists(Executable))throw new FileNotFoundException("Select Ghostscript console executable");string identity;string map=FontMap(run.Name,out identity);var ps=new StringBuilder();using(var w=new StringWriter(ps,System.Globalization.CultureInfo.InvariantCulture)){w.WriteLine("%!PS\n<< /PageSize ["+run.Crop.Width+" "+run.Crop.Height+"] >> setpagedevice\n"+PostScriptFontRun.CopyFont);var m=run.PixelMatrix();w.WriteLine("["+PostScriptFontRun.N(m[0])+" "+PostScriptFontRun.N(-m[1])+" "+PostScriptFontRun.N(m[2])+" "+PostScriptFontRun.N(-m[3])+" "+PostScriptFontRun.N(m[4]-run.Crop.Left)+" "+PostScriptFontRun.N(run.Crop.Height-m[5]+run.Crop.Top)+"] concat\n"+NativePrintContext.DefaultLineWidth.ToString("F4",System.Globalization.CultureInfo.InvariantCulture)+" setlinewidth 1 setlinejoin 0 setgray");if(NativePrintContext.StrokeAdjustment)w.WriteLine("true setstrokeadjust");run.Write(w);w.WriteLine("showpage");}
   string code=ps.ToString(),executable=Executable;
   string key=executable+new FileInfo(executable).LastWriteTimeUtc.Ticks+identity+code;
   if(cache.TryGetValue(key,out run.Image))return true;
   if(allowBackground&&Screen!=null){
    string failure;
    if(failures.TryGetValue(key,out failure))throw new InvalidOperationException(failure);
    if(pending.Add(key))QueueRender(Screen,key,executable,code,map);
    return false;
   }
   run.Image=RenderImage(executable,code,map,false);
   cache.Add(key,run.Image);
   pixels+=(long)run.Image.Width*run.Image.Height;
   return true;
  }
  static void QueueRender(Canvas canvas,string key,string executable,string code,string map){
   int version=generation;
   Score document=canvas.Score;
   Action render=()=>{
    Bitmap bitmap=null;
    string error=null;
    if(generation==version&&canvas.Score==document){
     try{bitmap=RenderImage(executable,code,map,true);}
     catch(Exception failure){error=failure.Message;}
    }
    try{
     canvas.BeginInvoke(new Action(()=>{
      if(version!=generation||canvas.IsDisposed){if(bitmap!=null)bitmap.Dispose();return;}
      pending.Remove(key);
      if(bitmap!=null){
       if(cache.ContainsKey(key))bitmap.Dispose();
       else{cache.Add(key,bitmap);pixels+=(long)bitmap.Width*bitmap.Height;}
      }
      if(error!=null)failures[key]=error;
      canvas.RefreshGhostscriptScreen();
     }));
    }catch(InvalidOperationException){if(bitmap!=null)bitmap.Dispose();}
   };
   lock(work){
    work.Enqueue(render);
    if(workers>=2)return;
    workers++;
    System.Threading.ThreadPool.QueueUserWorkItem(delegate {
     while(true){
      Action job;
      lock(work){
       if(work.Count==0){workers--;return;}
       job=work.Dequeue();
      }
      job();
     }
    });
   }
  }
  static Bitmap RenderImage(string executable,string code,string map,bool onWorker){
   string dir=Path.Combine(Path.GetTempPath(),"ScoreModern-font-"+Guid.NewGuid().ToString("N"));
   Directory.CreateDirectory(dir);
   try{
    string source=Path.Combine(dir,"text.ps"),output=Path.Combine(dir,"text.png"),fontmap=Path.Combine(dir,"Fontmap");
    File.WriteAllText(source,code,Encoding.ASCII);
    File.WriteAllText(fontmap,map,Encoding.ASCII);
    var info=new ProcessStartInfo(executable,"-dSAFER -dBATCH -dNOPAUSE -sDEVICE=pnggray -r72 -dTextAlphaBits=4 -dGraphicsAlphaBits=4 "+
     Quote("-sFONTMAP="+fontmap)+" "+Quote("-sOutputFile="+output)+" "+Quote(source)){
      UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
    var log=new StringBuilder();
    using(var process=new Process{StartInfo=info}){
     process.OutputDataReceived+=(sender,args)=>{if(args.Data!=null)lock(log)log.AppendLine(args.Data);};
     process.ErrorDataReceived+=(sender,args)=>{if(args.Data!=null)lock(log)log.AppendLine(args.Data);};
     process.Start();
     System.Threading.Interlocked.Increment(ref ProcessStarts);
     process.BeginOutputReadLine();
     process.BeginErrorReadLine();
     if(onWorker)WaitOnWorker(process,"Ghostscript font rendering timed out");
     else WaitForProcess(process,"Ghostscript font rendering timed out");
     if(process.ExitCode!=0||log.ToString().IndexOf("substitut",StringComparison.OrdinalIgnoreCase)>=0)
      throw new InvalidOperationException("Ghostscript failed or attempted font substitution: "+log);
    }
    using(var image=new Bitmap(output))return AlphaMask(image);
   }finally{
    foreach(string file in Directory.GetFiles(dir))File.Delete(file);
    Directory.Delete(dir);
   }
  }
  // Process.WaitForExit can pump STA messages while a canvas frame is active.
  // Drain both streams on a worker and keep nested paints out of that frame.
  internal static void WaitForProcess(Process process,string timeoutMessage){
   var wait=System.Threading.Tasks.Task.Factory.StartNew(()=>WaitOnWorker(process,timeoutMessage),
    System.Threading.CancellationToken.None,System.Threading.Tasks.TaskCreationOptions.DenyChildAttach,
    System.Threading.Tasks.TaskScheduler.Default);
   while(!wait.IsCompleted)System.Threading.Thread.Sleep(10);
   wait.GetAwaiter().GetResult();
  }
  static void WaitOnWorker(Process process,string timeoutMessage){
   if(process.WaitForExit(30000)){process.WaitForExit();return;}
   if(!process.HasExited)process.Kill();
   process.WaitForExit();
   throw new TimeoutException(timeoutMessage);
  }
  static Bitmap AlphaMask(Bitmap gray){
   var bounds=new Rectangle(0,0,gray.Width,gray.Height);
   using(var source=gray.Clone(bounds,PixelFormat.Format32bppArgb)){
    var result=new Bitmap(gray.Width,gray.Height,PixelFormat.Format32bppArgb);
    try{
     var input=source.LockBits(bounds,ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
     try{
      var output=result.LockBits(bounds,ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
      try{
       byte[] pixels=new byte[gray.Width*4],mask=new byte[pixels.Length];
       for(int y=0;y<gray.Height;y++){
        System.Runtime.InteropServices.Marshal.Copy(IntPtr.Add(input.Scan0,y*input.Stride),pixels,0,pixels.Length);
        for(int x=2;x<pixels.Length;x+=4)mask[x+1]=(byte)(255-pixels[x]);
        System.Runtime.InteropServices.Marshal.Copy(mask,0,IntPtr.Add(output.Scan0,y*output.Stride),mask.Length);
       }
      }finally{result.UnlockBits(output);}
     }finally{source.UnlockBits(input);}
     return result;
    }catch{result.Dispose();throw;}
   }
  }
  internal static void Paint(Graphics g,PostScriptFontRun run){var eps=NativeEpsExport.For(g);if(eps!=null){eps.Font(run);return;}if(run.Image==null)return;var state=g.Save();try{g.ResetTransform();g.PageUnit=GraphicsUnit.Pixel;g.PageScale=1;NativeItemPaint.Paint(g,run.Image,image=>g.DrawImageUnscaled(image,run.Crop.Left,run.Crop.Top));NativeItemBounds.Image(g,run.Image,run.Crop.Left,run.Crop.Top);}finally{g.Restore(state);}}
 }
}

