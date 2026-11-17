using System;using System.IO;using System.Text;using System.Linq;using System.Text.RegularExpressions;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using System.Windows.Forms;using ScoreModern.RecoveredFragments;
using System.Globalization;
namespace ScoreModern {
 public sealed class OriginalTitle {
  public string Source,Version;public int Items,Parameters,Vectors;public string[] Rows;
  static int Find(byte[] bytes,byte[] pattern,int start){for(int n=start;n<=bytes.Length-pattern.Length;n++){int j=0;while(j<pattern.Length&&bytes[n+j]==pattern[j])j++;if(j==pattern.Length)return n;}return -1;}
  static int Breaks(byte[] bytes,int start,int end){int rows=0;for(int n=start;n<end;n++)if(bytes[n]==0x12)rows++;return rows;}
  public static OriginalTitle Read(string root){return NativeEmbeddedTitle.Read(root);}
  public static OriginalTitle ReadExternal(string root){string path=Path.Combine(root,"SCORE.EXE");byte[] bytes=File.ReadAllBytes(path);string binary=Encoding.GetEncoding(28591).GetString(bytes);int name=Find(bytes,Encoding.ASCII.GetBytes("SCORE (tm)"),0);if(name<12)throw new NotSupportedException("Original SCORE title format not found.");int start=name-12;if(bytes[start]!=0x12||bytes[start+1]!=0x12||bytes[start+2]!=0x12)throw new NotSupportedException("Original title prefix outside the verified format.");var versions=Regex.Matches(binary,@"\b[0-9]\.[0-9]{2}-[0-9]+\b");if(versions.Count!=1)throw new NotSupportedException("Original title version is ambiguous.");var limits=Regex.Match(binary,@"\xC7\x06\xD8\x80(?<vectors>..)\xC7\x06\xDE\x80..\xC7\x06\xD0\x80(?<parameters>..)\xC7\x06\x4E\xAC(?<items>..)",RegexOptions.Singleline);if(!limits.Success)throw new NotSupportedException("Original title limits outside the verified executable initialiser.");var result=new OriginalTitle{Source=path,Version=versions[0].Value};result.Items=BitConverter.ToUInt16(bytes,limits.Groups["items"].Index);result.Parameters=BitConverter.ToUInt16(bytes,limits.Groups["parameters"].Index);result.Vectors=BitConverter.ToUInt16(bytes,limits.Groups["vectors"].Index);
   int final=Find(bytes,Encoding.ASCII.GetBytes("**** reading SCORE data ****"),name);if(final<0)throw new NotSupportedException("Original title loading line is missing.");var rows=new SortedDictionary<int,string>();var parts=Regex.Matches(binary.Substring(start,final-start),@"\xB2\x20(?<indent>..)\xB0(?<text>[ -~]+)",RegexOptions.Singleline);if(parts.Count!=11)throw new NotSupportedException("Original title line count outside the verified format.");foreach(Match part in parts){int at=start+part.Index,indent=BitConverter.ToUInt16(bytes,start+part.Groups["indent"].Index)-1;string text=part.Groups["text"].Value;int following=start+part.Groups["text"].Index+text.Length;if(text.EndsWith("f")&&bytes[following]==1&&bytes[following+1]==13)text=text.Substring(0,text.Length-1)+result.Version.PadRight(13);if(indent<0||indent+text.Length>80)throw new NotSupportedException("Original title line exceeds80 columns.");rows[Breaks(bytes,start,at)]=new string(' ',indent)+text;}
   int limitsAt=Find(bytes,Encoding.ASCII.GetBytes("    Program limits: "),name)-2;if(limitsAt<start||bytes[limitsAt]!=0x22)throw new NotSupportedException("Original limit caption outside the verified format.");int cursor=limitsAt;var line=new StringBuilder();int number=0;int[] values={result.Items,result.Parameters,result.Vectors};while(cursor<final&&bytes[cursor]!=0x12){int op=bytes[cursor++];if(op==0x22){int count=bytes[cursor++];line.Append(Encoding.ASCII.GetString(bytes,cursor,count));cursor+=count;}else if(op==0x68&&bytes[cursor++]==1&&number<3){int width=bytes[cursor++];line.Append(values[number++].ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(width));}else throw new NotSupportedException("Original limit formatter outside the verified subset.");}if(number!=3)throw new NotSupportedException("Original limit fields are incomplete.");int limitRow=Breaks(bytes,start,limitsAt);rows[limitRow]=line.ToString().Substring(1);int loadRow=limitRow;while(bytes[cursor]==0x12){loadRow++;cursor++;}if(bytes[cursor++]!=0x22)throw new NotSupportedException("Original loading formatter is missing.");int length=bytes[cursor++];rows[loadRow]=Encoding.ASCII.GetString(bytes,cursor,length).Substring(1);int scroll=Math.Max(0,rows.Keys.Max()-24);result.Rows=Enumerable.Range(0,25).Select(n=>rows.ContainsKey(n+scroll)?rows[n+scroll]:"").ToArray();return result;
  }
 }
 public sealed class NativeStartupSplash:Form {
  static NativeStartupSplash active;static System.Diagnostics.Stopwatch shownFor;static Timer closeTimer;public static string Status="";public static bool ShowOnStartup=true;readonly OriginalTitle title;readonly Bitmap logo;readonly ExternalVgaFont titleFont;public readonly string LogoStatus;
  static string Settings{get{return "splash";}}
  const string DurationSetting="splash-seconds";
  const decimal DefaultDuration=3,MaximumDuration=60;
  public static decimal DurationSeconds{get;private set;}
  public static void LoadSettings(){
   ShowOnStartup=true;DurationSeconds=DefaultDuration;
   try{
    if(NativeSettings.Exists(Settings))ShowOnStartup=NativeSettings.ReadAllText(Settings).Trim()!="off";
    decimal seconds;
    if(Decimal.TryParse(NativeSettings.Read(DurationSetting),NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out seconds)
     &&seconds>=0&&seconds<=MaximumDuration)DurationSeconds=seconds;
   }catch(IOException){}catch(UnauthorizedAccessException){}
  }
  public static void SetEnabled(bool enabled,bool save){ShowOnStartup=enabled;if(save)NativeSettings.WriteAllText(Settings,enabled?"on":"off");}
  public NativeStartupSplash(OriginalTitle original){title=original;titleFont=LoadTitleFont();DoubleBuffered=true;BackColor=Color.Black;ForeColor=Color.LightGray;FormBorderStyle=FormBorderStyle.None;ShowInTaskbar=false;TopMost=true;StartPosition=FormStartPosition.CenterScreen;Text=BuildVersion.Product+" — loading";float dpi=NativeDisplay.Scale(this);var area=Screen.FromControl(this).WorkingArea;float width=Math.Min(960*dpi,Math.Min(area.Width*.85f,(area.Height*.85f-30)*4/3));ClientSize=new Size((int)width,(int)(width*.75f)+FooterHeight);using(var resource=typeof(NativeStartupSplash).Assembly.GetManifestResourceStream("ScoreModern.TitleLogo.png")){
   if(resource==null)throw new InvalidDataException("Embedded title logo is missing.");
   using(var image=Image.FromStream(resource))logo=new Bitmap(image);
  }
  if(logo.Width!=1000||logo.Height!=427)throw new InvalidDataException("Unexpected embedded title logo dimensions.");
  LogoStatus="Logo: embedded user-supplied PNG; RGB inverted, original alpha preserved";
 }

  protected override void OnShown(EventArgs e){var area=Screen.FromPoint(Cursor.Position).WorkingArea;Location=new Point(area.Left+(area.Width-Width)/2,area.Top+(area.Height-Height)/2);base.OnShown(e);}
  const int TitleTextGrey=170;
  float LogoGrey{get{return Math.Min(255,TitleTextGrey*1.10f)/255f;}}
  public void PaintLogo(Graphics target,RectangleF area){
   // Retain exactly the previous LIBRT position9 footprint (494 by199),
   // right/top alignment and viewport box. The supplied PNG stays unchanged.
   float scale=Math.Min(area.Width/494f,area.Height/199f);
   var destination=new RectangleF(area.Right-494*scale,area.Top,494*scale,199*scale);
   var state=target.Save();
   try{
    target.InterpolationMode=InterpolationMode.HighQualityBicubic;
    target.PixelOffsetMode=PixelOffsetMode.HighQuality;
    using(var attributes=new System.Drawing.Imaging.ImageAttributes()){
     attributes.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(new float[][]{
      new float[]{-LogoGrey,0,0,0,0},new float[]{0,-LogoGrey,0,0,0},new float[]{0,0,-LogoGrey,0,0},
      new float[]{0,0,0,1,0},new float[]{LogoGrey,LogoGrey,LogoGrey,0,1}}));
     target.DrawImage(logo,new PointF[]{new PointF(destination.Left,destination.Top),
      new PointF(destination.Right,destination.Top),new PointF(destination.Left,destination.Bottom)},
      new RectangleF(8,11,985,403),GraphicsUnit.Pixel,attributes);
    }
   }finally{target.Restore(state);}
  }
  protected override void Dispose(bool disposing){if(disposing&&logo!=null)logo.Dispose();base.Dispose(disposing);}


  internal static OriginalTitle ReadSelectedTitle(string root){
   return Editor.ReadScor4InterfacePreference()?NativeEmbeddedTitle.ReadScor4():OriginalTitle.Read(root);
  }
  public static void ShowFor(string root){
   if(!ShowOnStartup||Program.CheckMode)return;
   try{
    var title=ReadSelectedTitle(root);
    active=new NativeStartupSplash(title);
    active.Show();shownFor=System.Diagnostics.Stopwatch.StartNew();active.Refresh();Application.DoEvents();
    Status="Embedded SCORE "+title.Version+" title. "+active.LogoStatus;
   }catch(IOException e){Status="Original splash unavailable: "+e.Message;}
    catch(NotSupportedException e){Status="Original splash unavailable: "+e.Message;}
    catch(UnauthorizedAccessException e){Status="Original splash unavailable: "+e.Message;}
  }
  public static void CloseAfterMinimum(){if(active==null)return;long remaining=(long)(DurationSeconds*1000)-shownFor.ElapsedMilliseconds;if(remaining<=0){CloseActive();return;}if(closeTimer!=null)closeTimer.Dispose();closeTimer=new Timer{Interval=(int)remaining};closeTimer.Tick+=(s,e)=>CloseActive();closeTimer.Start();}
  public static void CloseActive(){if(closeTimer!=null){closeTimer.Stop();closeTimer.Dispose();closeTimer=null;}var form=active;active=null;if(form!=null){form.Close();form.Dispose();}}
  internal static void SetDuration(decimal value){if(value<0||value>MaximumDuration)throw new ArgumentOutOfRangeException("value");NativeSettings.WriteAllText(DurationSetting,value.ToString(CultureInfo.InvariantCulture));DurationSeconds=value;}
  static void ChooseDuration(){
   using(var dialog=new ScoreDialog("TitleScreenDuration")){
    dialog.Text="Title screen duration";dialog.StartPosition=FormStartPosition.CenterParent;
    dialog.ClientSize=new Size(345,108);dialog.MaximizeBox=false;dialog.MinimizeBox=false;
    var content=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,Padding=new Padding(8)};
    content.Controls.Add(new Label{AutoSize=true,Text="Minimum display time in seconds (0–60)"});
    var seconds=new NumericUpDown{Minimum=0,Maximum=MaximumDuration,DecimalPlaces=1,Increment=.5m,Value=DurationSeconds};
    content.Controls.Add(seconds);
    var actions=new FlowLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,WrapContents=false};
    var ok=new Button{AutoSize=true,Text="OK",DialogResult=DialogResult.OK};
    var cancel=new Button{AutoSize=true,Text="Cancel",DialogResult=DialogResult.Cancel};
    actions.Controls.Add(ok);actions.Controls.Add(cancel);content.Controls.Add(actions);
    dialog.Controls.Add(content);dialog.AcceptButton=ok;dialog.CancelButton=cancel;
    if(dialog.ShowDialog(Form.ActiveForm)!=DialogResult.OK)return;
    NativeSettings.WriteAllText(DurationSetting,seconds.Value.ToString(CultureInfo.InvariantCulture));
    DurationSeconds=seconds.Value;
   }
  }
  public static void AddMenu(ContextMenuStrip menu){
   var item=new ToolStripMenuItem("Startup: original SCORE title splash");
   item.Click+=(s,e)=>SetEnabled(!ShowOnStartup,true);menu.Items.Add(item);
   var duration=new ToolStripMenuItem();
   duration.Click+=(s,e)=>ChooseDuration();menu.Items.Add(duration);
   Action refresh=()=>{item.Checked=ShowOnStartup;item.ToolTipText=Status;duration.Text="Startup: title screen duration ("+DurationSeconds.ToString("0.#",CultureInfo.CurrentCulture)+" s)...";};
   refresh();menu.Opening+=(s,e)=>refresh();
  }
  static ExternalVgaFont LoadTitleFont(){
   string local=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"VGA Fonts","VGA8.F16");
   foreach(string candidate in new[]{NativeInterfaceText.VgaPath,local}){
    if(String.IsNullOrEmpty(candidate)||!File.Exists(candidate))continue;
    try{return new ExternalVgaFont(File.ReadAllBytes(candidate));}
    catch(IOException){}catch(UnauthorizedAccessException){}catch(ArgumentException){}
   }
   return null;
  }
  public static void PaintTitle(Graphics target,Rectangle bounds,OriginalTitle title){
   PaintTitle(target,bounds,title,LoadTitleFont());
  }
  static void PaintTitle(Graphics target,Rectangle bounds,OriginalTitle title,ExternalVgaFont vga){
   var session=new NativeTextSession();
   session.Write(Encoding.GetEncoding(437).GetBytes(String.Join("\r\n",title.Rows)));
   var state=target.Save();
   try{
    target.SetClip(bounds,CombineMode.Intersect);
    target.FillRectangle(Brushes.Black,bounds);
    float cellWidth=bounds.Width/80f,cellHeight=bounds.Height/25f;
    target.TextRenderingHint=System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
    using(var font=new Font("Consolas",cellHeight*5/6,GraphicsUnit.Pixel))
    using(var brush=new SolidBrush(Color.FromArgb(TitleTextGrey,TitleTextGrey,TitleTextGrey)))
     NativeInterfaceText.DrawSurface(target,session.Surface,bounds.Location,cellWidth,cellHeight,font,brush,vga,25);
   }finally{target.Restore(state);}
  }
  int FooterHeight{get{return (int)Math.Ceiling(36*NativeDisplay.Scale(this));}}
  protected override void OnPaint(PaintEventArgs e){
   base.OnPaint(e);
   float dpi=NativeDisplay.Scale(this);
   int edge=Math.Max(1,(int)Math.Round(dpi));
   var bounds=new Rectangle(edge,edge,ClientSize.Width-2*edge,ClientSize.Height-FooterHeight-edge);
   PaintTitle(e.Graphics,bounds,title,titleFont);
   float margin=16*dpi;
   PaintLogo(e.Graphics,new RectangleF(ClientSize.Width*.70f,margin,ClientSize.Width*.30f-margin,ClientSize.Height*.16f));
   e.Graphics.TextRenderingHint=System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
   using(var font=new Font("Consolas",14*dpi,GraphicsUnit.Pixel))
    using(var format=new StringFormat{Alignment=StringAlignment.Far})e.Graphics.DrawString(BuildVersion.DisplayVersion,font,Brushes.DarkGray,new RectangleF(12*dpi,ClientSize.Height-FooterHeight+8*dpi,ClientSize.Width-24*dpi,FooterHeight-8*dpi),format);
   using(var border=new Pen(Color.FromArgb(66,66,66),edge)){
    border.Alignment=PenAlignment.Inset;
    e.Graphics.DrawRectangle(border,0,0,ClientSize.Width-1,ClientSize.Height-1);
   }
  }
 }
}
