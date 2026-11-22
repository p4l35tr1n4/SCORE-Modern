using ScoreModern.RecoveredFragments;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScoreModern {
public sealed class Item {
    public byte[] Bytes;
    internal int ConsumedCenteringParameterCount;
    internal bool ExternalDrawingResetResolved,EditingExternalDrawingReset;
    internal string PartResourcePath;
    internal string ExternalDrawingNameOverride;internal float[] ExternalPostScriptBoxOverride;
    public Item(byte[] b) { Bytes=b; }
    public int Count { get { return Bytes.Length/4-1; } }
    public float P(int n) { if(n==13&&EditingExternalDrawingReset)return 0;return n<=Count ? BitConverter.ToSingle(Bytes,n*4):0; }
    public void Set(int n,float v) {
        if(EditingExternalDrawingReset){bool postScript=Code==15;EditingExternalDrawingReset=false;if(!postScript&&Count>=13)Array.Copy(BitConverter.GetBytes(0f),0,Bytes,52,4);}
        if(n>Count) { var b=new byte[(n+1)*4]; Array.Copy(Bytes,b,Bytes.Length); Bytes=b; Array.Copy(BitConverter.GetBytes((float)n),0,Bytes,0,4); }
        Array.Copy(BitConverter.GetBytes(v),0,Bytes,n*4,4);
    }
    public int Code { get { return (int)Math.Truncate(P(1)); } }
    public int Staff { get { return (int)Math.Truncate(P(2)); } }
    public string Text { get { if(Code!=16 || Count<13)return ""; int n=Math.Max(0,Math.Min((int)P(12),Bytes.Length-56)); return Encoding.GetEncoding(437).GetString(Bytes,56,n); } }
    public static Item Make(params float[] p) { var i=new Item(new byte[(p.Length+1)*4]); Array.Copy(BitConverter.GetBytes((float)p.Length),i.Bytes,4); for(int j=0;j<p.Length;j++)i.Set(j+1,p[j]); return i; }
    public void TrimEditedNumericParameters(){if(Code==16){float length=P(12);if(Count>=13&&!Single.IsNaN(length)&&!Single.IsInfinity(length)&&length>=0&&length<=Bytes.Length-56){int textWords=13+((int)length+3)/4;if(textWords<Count){var text=new byte[(textWords+1)*4];Array.Copy(Bytes,text,text.Length);Array.Copy(BitConverter.GetBytes((float)textWords),text,4);Bytes=text;}}return;}if(Code<1||Code>18||Code==11||Code==15||Code==16)return;int count=Count;while(count>Math.Max(1,ConsumedCenteringParameterCount)){int at=count*4;if((Bytes[at]|Bytes[at+1]|Bytes[at+2]|Bytes[at+3])!=0)break;count--;}if(count==Count)return;var trimmed=new byte[(count+1)*4];Array.Copy(Bytes,trimmed,trimmed.Length);Array.Copy(BitConverter.GetBytes((float)count),trimmed,4);Bytes=trimmed;}
    internal bool HasVerifiedShortDrawingName {
        get { return Code==11&&Count==19&&P(13)==0&&Bytes.Skip(62).Take(14).All(value=>value==0||value==32); }
    }
    internal void RestoreShortDrawingPadding() {
        for(int at=76;at<80;at++)Bytes[at]=32;
    }
    public Item Clone() { return new Item((byte[])Bytes.Clone()){ConsumedCenteringParameterCount=ConsumedCenteringParameterCount,ExternalDrawingResetResolved=ExternalDrawingResetResolved,EditingExternalDrawingReset=EditingExternalDrawingReset,PartResourcePath=PartResourcePath,ExternalDrawingNameOverride=ExternalDrawingNameOverride,ExternalPostScriptBoxOverride=ExternalPostScriptBoxOverride==null?null:(float[])ExternalPostScriptBoxOverride.Clone()}; }
    public static string Name(int code) { string[] names={"Unknown","Note","Rest","Clef","Line / hairpin","Slur / bracket","Beam","Trill / pedal","Staff","Library symbol","Number","User symbol","Shape","Invisible item","Barline","PostScript","Text","Key signature","Time signature"}; return code>=0&&code<names.Length?names[code]:"Code "+code; }
}
public sealed class Score {
    public string SourcePath="";internal List<Item> CancelledSaveRecords;internal byte[] CancelledSaveBaseline,CancelledSaveUndefinedWord;internal bool CancelledSaveValid;internal bool RawStorageLayoutVerified;internal byte[] OriginalFilePayload,RetainedUndefinedWord;
    public List<Item> Items=new List<Item>(); public byte[] Trailer;
    public static Score Load(string path) {
        byte[] b=File.ReadAllBytes(path);
        // JUST 3.01 writes the second and later files of its height batch
        // as a single Fortran record containing the complete MUS payload.
        if(b.Length>=3&&b[0]==0x4b&&
          ((b.Length-2)%4!=0||BitConverter.ToUInt16(b,0)!=(b.Length-2)/4)){
            var framed=FortranRecords.Read(b);
            if(framed.Count!=1)throw new InvalidDataException("Expected one framed SCORE record.");
            b=framed[0];
        }
        if(b.Length<22 || (b.Length-2)%4!=0)throw new InvalidDataException("Not a supported SCORE binary file.");
        if(BitConverter.ToUInt16(b,0)!=(b.Length-2)/4)throw new InvalidDataException("The SCORE word count does not match the file size.");
        if(BitConverter.ToSingle(b,b.Length-4)!=-9999)throw new InvalidDataException("Missing SCORE end marker.");
        var s=new Score{SourcePath=Path.GetFullPath(path)}; int o=2;
        while(b.Length-o>24) {
            float f=BitConverter.ToSingle(b,o); if(float.IsNaN(f)||float.IsInfinity(f))throw new InvalidDataException("Invalid record length at "+o);
            int n=(int)Math.Round(f); if(n<1||n>4096 || Math.Abs(f-n)>.1 || o+(n+1)*4>b.Length-20)throw new InvalidDataException("Invalid record at byte "+o);
            byte[] r=new byte[(n+1)*4]; Array.Copy(b,o,r,0,r.Length); var item=new Item(r);
            if(item.Code<1||item.Code>99 || float.IsNaN(item.P(1)) || float.IsInfinity(item.P(1)))throw new InvalidDataException("Invalid object code at "+o);
            s.Items.Add(item); o+=r.Length;
        }
        if(b.Length-o!=20&&b.Length-o!=24)throw new InvalidDataException("Unsupported footer.");
        s.Trailer=new byte[b.Length-o]; Array.Copy(b,o,s.Trailer,0,s.Trailer.Length);s.OriginalFilePayload=new byte[b.Length-2];Array.Copy(b,2,s.OriginalFilePayload,0,s.OriginalFilePayload.Length);s.RawStorageLayoutVerified=true;NativePartResourceIndex.Bind(s,path); return s;
    }
    public byte[] Encode() { using(var m=new MemoryStream())using(var w=new BinaryWriter(m)) { int words=(Items.Sum(i=>i.Bytes.Length)+Trailer.Length)/4; if(words>65535)throw new InvalidDataException("Score exceeds this format's 16-bit word count."); w.Write((ushort)words); foreach(var i in Items)w.Write(i.Bytes); w.Write(Trailer); return m.ToArray(); } }
    public Score Clone() { return new Score {SourcePath=SourcePath,OriginalFilePayload=OriginalFilePayload,CancelledSaveRecords=CancelledSaveRecords==null?null:CancelledSaveRecords.Select(i=>i.Clone()).ToList(),CancelledSaveBaseline=CancelledSaveBaseline==null?null:(byte[])CancelledSaveBaseline.Clone(),CancelledSaveUndefinedWord=CancelledSaveUndefinedWord==null?null:(byte[])CancelledSaveUndefinedWord.Clone(),CancelledSaveValid=CancelledSaveValid,Items=Items.Select(i=>i.Clone()).ToList(),Trailer=(byte[])Trailer.Clone()}; }
    public static Score New() { return new Score {Items=new List<Item>{Item.Make(8,1,0,0,1,200),Item.Make(3,1,2),Item.Make(18,1,14,0,4,4),Item.Make(14,1,200,1,2)},Trailer=new byte[20] {0,0,0,0,0,0,128,63,0,0,0,0,0,0,160,64,0,60,28,198}}; }
}
public sealed partial class Canvas : Control {
    internal Point? RetainedRestartPoint {get;set;}
    internal Point? RetainedDisplayCenter {get;set;}
    public int SpacingScaleStaff;
    public readonly HashSet<int> DisappearingStaffs=new HashSet<int>();
    internal bool DisplayHidden(Item item){return !NativeNotationView.IsPrinting&&NativeEpsExport.Current==null&&DisappearingStaffs.Contains(item.Staff);}
    public readonly List<string> DrawingIssues=new List<string>();public Action<string> DisplayIssue;string lastReportedIssue="";
    public float? EditGuidePosition{get;set;}public Score Score; public Item Selected;public Item SelectionContextSource;public bool BlockItemEditing;public Item EditOriginal;public bool EditModified; public DrawingLibrary Library; public ScoreText TextLayout; float zoom=1; public float Zoom{get{return zoom;}set{zoom=value;}} public Action<int,Keys,PointF,PointF,bool> MouseGesture;int gestureButtons,gestureHeld,gestureLatchedButtons;public bool MouseOperationLocked;public Action MouseGestureCancelled;Keys gestureModifiers;PointF gestureStart;public Action<Item> Picked; public Action Changed;
    public bool FlatDisplay; public float DisplayDpi=1; public float RenderScale {get{return Zoom*DisplayDpi;}}
    public PointF CameraOffset{get;set;}public int CropTop,DosTextClipOverride=-1;public bool DosFullGraphics,DosGroupHeader;public int DosGroupTop=94;public short DosClipBottom=461;public short DosClipTop{get{int top=DosTextClipOverride>=0?DosTextClipOverride:(int)Math.Max(0,Math.Min(461,Math.Ceiling(97+CropTop/RenderScale)));return (short)(DosGroupHeader?DosGroupTop:top);}}public PointF? HoverPoint{get;set;}public readonly Dictionary<Item,RectangleF> ItemBounds=new Dictionary<Item,RectangleF>();
    public float ViewWidth{get{return NativeNotationView.IsDosViewport?640:ScreenWidth;}}public float ViewHeight{get{return NativeNotationView.IsDosViewport?DosClipBottom-96:ScreenHeight;}}
    public float PageHeight=600; Point dragStart; float dragX,dragY; bool dragging,viewportPan,mainSelectionPending;short panX,panY; public Action BeforeDrag;
    public Canvas() { DoubleBuffered=true; BackColor=Color.FromArgb(232,236,242); TabStop=true; }
    public NativePageDocument PageDocument;internal bool RebuildGuideMeasurement;public float PageWidth=34000/30f;
    public void LayoutScore() {using(var metricHistory=TextLayout==null?null:TextLayout.PreserveAlignmentMetricHistory()){
        if(Score==null)return;PageDocument=new NativePageDocument(Score,PageDocument,RebuildGuideMeasurement);RebuildGuideMeasurement=false;foreach(var versionContext in PageDocument.Contexts.Values)versionContext.Scor4=Scor4Interface;PageWidth=PageDocument.Projection.Width;PageHeight=PageDocument.Projection.Height;if(Selected!=null&&SelectionContextSource!=null&&!PageDocument.Contexts.ContainsKey(Selected)){NativeNotationContext selectedContext;if(PageDocument.Contexts.TryGetValue(SelectionContextSource,out selectedContext))PageDocument.Contexts[Selected]=selectedContext;}
        LayoutView();
    }}
    internal void LayoutView(){
        if(Score==null||PageDocument==null)return;
        LayoutOriginalZoomFrame();UpdateScreenFrame();
        Size=new Size((int)Math.Ceiling((ViewWidth+((NativeNotationView.IsDosViewport||UsesOriginalZoomFrame)?0:Math.Max(0,CameraOffset.X)))*RenderScale),Math.Max(1,(int)Math.Ceiling((ViewHeight+((NativeNotationView.IsDosViewport||UsesOriginalZoomFrame)?0:Math.Max(0,CameraOffset.Y)))*RenderScale)-CropTop));if(UsesOriginalZoomFrame&&Parent!=null)Size=Parent.ClientSize;else if(NativeWorkspaceGuides.Rulers&&!NativeNotationView.IsDosViewport&&Parent!=null)Size=new Size(Math.Max(Width,Parent.ClientSize.Width),Math.Max(Height,Parent.ClientSize.Height));Invalidate();
    }
    NativeNotationContext Context(Item i){NativeNotationContext context;return PageDocument!=null&&PageDocument.Contexts.TryGetValue(i,out context)?context:null;}
    float Step(Item i){var c=Context(i);return c==null?25*7/30f:c.Scale*7/30f;}
    float Base(Item i){var c=Context(i);return c==null?PageHeight: c.Point(0,(float)((double)c.RawBase+c.Scale*21)).Y;}
    float X(float x){return PageDocument==null?70+x*5:PageDocument.Projection.Point((float)((double)x*150),0).X;}
    float Y(Item i,float p){var c=Context(i);if(c==null)return Base(i);if(i.Code<=3||i.Code==6)p=(float)((double)p%100);return c.Point(0,c.Vertical(p)).Y;}
    public PointF Position(Item i){var c=Context(i);if(c==null)return new PointF(X(i.P(3)),Y(i,i.P(4)));if(NativeNotationView.IsDosViewport){var point=NativeEditCursorPosition.Get(i,c,NativeDosViewport.Divisor,NativeDosViewport.Divisor,NativeDosViewport.OriginX,NativeDosViewport.OriginY);return new PointF(point.X,point.Y-97);}if(i.Code==8){PageStaffResult staff;if(PageDocument.StaffDrawings.TryGetValue(i,out staff))return c.Point((float)((double)i.P(3)*150),staff.Bottom);}return c.Point(c.Prepared.X,c.Prepared.Y);}
    public Item HitTest(PointF nativePoint){using(var metricHistory=TextLayout==null?null:TextLayout.PreserveAlignmentMetricHistory())return NativeMouseSelector.HitTest(this,nativePoint);}
    [ThreadStatic] static bool paintingScore;
    protected override void OnPaint(PaintEventArgs e){using(var metricHistory=TextLayout==null?null:TextLayout.PreserveAlignmentMetricHistory()){
        // A modal/native message pump can dispatch WM_PAINT during an active score frame.
        if(paintingScore||NativePrintExecution.Current!=null&&(NativeEpsExport.Current!=null||NativeNotationView.IsPrinting))return;
        paintingScore=true;
        try{
            if(!Program.CheckMode&&IsHandleCreated&&GhostscriptTextRenderer.Enabled)
                GhostscriptTextRenderer.Screen=this;
            PaintScoreScreen(e);
        }finally{
            GhostscriptTextRenderer.Screen=null;
            paintingScore=false;
        }
    }}
    System.Windows.Forms.Timer ghostscriptRefresh;
    System.Windows.Forms.Timer dosCursorTimer;
    bool dosCursorVisible=true;
    internal bool DosCursorVisible {
        get {
            if(Program.CheckMode||EditOriginal==null||Selected==null)return true;
            if(dosCursorTimer==null){
                dosCursorTimer=new System.Windows.Forms.Timer{Interval=50};
                dosCursorTimer.Tick+=(s,e)=>{
                    if(!NativeNotationView.IsDosViewport||EditOriginal==null||Selected==null){dosCursorTimer.Stop();return;}
                    bool visible=NativeDosEditCursor.VisibleAt(DateTime.Now.Millisecond/10);
                    if(visible!=dosCursorVisible){dosCursorVisible=visible;Invalidate();}
                };
                Disposed+=(s,e)=>dosCursorTimer.Dispose();
            }
            if(!dosCursorTimer.Enabled){dosCursorVisible=NativeDosEditCursor.VisibleAt(DateTime.Now.Millisecond/10);dosCursorTimer.Start();}
            return dosCursorVisible;
        }
    }
    internal void RefreshGhostscriptScreen(){
        if(IsDisposed)return;
        if(ghostscriptRefresh==null){
            ghostscriptRefresh=new System.Windows.Forms.Timer{Interval=250};
            ghostscriptRefresh.Tick+=(s,e)=>{ghostscriptRefresh.Stop();if(GhostscriptTextRenderer.Enabled)Invalidate();};
            Disposed+=(s,e)=>ghostscriptRefresh.Dispose();
        }
        ghostscriptRefresh.Start();
    }
    void PaintScore(PaintEventArgs e,bool includeHover) {if(UsesOriginalZoomFrame){e.Graphics.Clear(Parent==null?BackColor:Parent.BackColor);var frame=new RectangleF(0,0,ViewWidth*RenderScale,ViewHeight*RenderScale);using(var background=new SolidBrush(BackColor))e.Graphics.FillRectangle(background,frame);e.Graphics.SetClip(frame,CombineMode.Intersect);} e.Graphics.ScaleTransform(RenderScale,RenderScale);e.Graphics.TranslateTransform(0,-CropTop/RenderScale);if(!NativeNotationView.IsDosViewport)e.Graphics.TranslateTransform(CameraOffset.X+ScreenOrigin.X,CameraOffset.Y+ScreenOrigin.Y);PaintScreenPaper(e.Graphics); try{using(NativeItemBounds.Begin(this)){var palette=Library==null?null:NativeScreenPalette.Read(Library.Root);if(FlatDisplay&&palette!=null&&(palette.Graphics.ToArgb()!=Color.Black.ToArgb()||palette.Background.ToArgb()!=Color.White.ToArgb())){Rectangle clip=e.ClipRectangle;if(clip.Width>0&&clip.Height>0)using(var tile=new Bitmap(clip.Width,clip.Height))using(var graphics=Graphics.FromImage(tile)){graphics.Clear(ScreenFraming?Color.Transparent:Color.White);using(var matrix=e.Graphics.Transform){matrix.Translate(-clip.X,-clip.Y,MatrixOrder.Append);graphics.Transform=matrix;}using(var stable=GhostscriptTextRenderer.StableScreenClip(this,clip.X,clip.Y)){Draw(graphics,false);NativeDosOverlays.DrawVector(this,graphics);}var state=e.Graphics.Save();try{e.Graphics.ResetTransform();palette.Remap(e.Graphics,tile,clip);}finally{e.Graphics.Restore(state);}}}else {using(var stable=GhostscriptTextRenderer.StableScreenClip(this)){Draw(e.Graphics,!FlatDisplay&&!ScreenFraming);NativeDosOverlays.DrawVector(this,e.Graphics);}}}}catch(NotSupportedException error){DrawingIssues.Clear();DrawingIssues.Add(error.Message);}if(includeHover){NativeMouseSelector.DrawPoints(this,e.Graphics);NotifyGuides();}string issue=String.Join("\r\n",DrawingIssues.Distinct().ToArray());if(issue.Length>0&&!NativeNotationView.UsesDat)issue="Engraving incomplete. Print and EPS require every item to be rendered.\r\n"+issue;if(ApproximatedItems.Count>0)issue+=(issue.Length==0?"":"\r\n")+String.Join("\r\n",ApproximatedItems.ToArray());if(issue!=lastReportedIssue){lastReportedIssue=issue;if(DisplayIssue!=null)DisplayIssue(issue);}if(FlatDisplay&&Selected!=null&&!NativeNotationView.IsDosViewport){PointF p=Position(Selected);using(var pen=new Pen(Color.FromArgb(30,70,130),1))e.Graphics.DrawRectangle(pen,p.X-3,p.Y-8,18,16);} }
    NativeSlurFrame slurFrame;
    HashSet<int> librarySymbolCache;
    internal HashSet<int> CompositeLibraryCache;
    public void Draw(Graphics g,bool paper){using(var metricHistory=TextLayout==null?null:TextLayout.PreserveAlignmentMetricHistory())using(NativePrintContext.Begin(Library==null?null:Library.Root))DrawScene(g,paper);}
    void DrawScene(Graphics g,bool paper) {
        NativeItemBounds.Surface(g);NativeItemBounds.Select(null);
        if(NativeDosViewport.Draw(this,g,paper))return;
        if(NativeNotationView.DrawPixelPage(this,g,paper))return;
        g.SmoothingMode=NativeNotationView.PixelPass?SmoothingMode.None:SmoothingMode.AntiAlias; g.TextRenderingHint=NativeNotationView.PixelPass?System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit:System.Drawing.Text.TextRenderingHint.AntiAlias;
        if(paper) { using(var shadow=new SolidBrush(Color.FromArgb(210,215,223)))g.VectorFillRectangle(shadow,8,8,PageWidth,PageHeight); g.VectorFillRectangle(Brushes.White,0,0,PageWidth,PageHeight); }
        if(Score==null)return;
        DrawingIssues.Clear();
        ApproximatedItems.Clear();
        if(!NativeNotationView.UsesDat&&NativeEpsExport.For(g)==null&&NativeRawPostScript.RequiresScene(Score)){try{NativeRawPostScript.DrawScene(this,g);DrawInputNumbers(g);}catch(Exception error){ReportDrawError(g,Score.Items.FindIndex(item=>NativeRawPostScript.IsRaw(item)),error);}return;}
        if(NativeNotationView.IsPrinting&&(PageDocument==null||PageDocument.Issues.Count!=0))
            throw new NotSupportedException("Cannot print without verified SCORE page geometry.");
        if(!NativeNotationView.UsesDat)PrintedFontCarry.Begin();NativeDatDrawing.BeginFrame();NativeRawPostScript.BeginFrame();NativeDatDrawing.Report=message=>DrawingIssues.Add(message);librarySymbolCache=CompositeLibraryCache??(Library==null?null:NativeLibrarySymbols.Cache(Score,Library));slurFrame=new NativeSlurFrame();try {
        for(int index=0;index<Score.Items.Count;index++){var i=Score.Items[index];if(DisplayHidden(i))continue;NativePrintExecution.Before(this,g,index+1);NativeItemBounds.Select(i);if(NativeEpsExport.For(g)!=null)NativeEpsExport.For(g).ItemMarker(index+1);if(!NativeNotationView.UsesDat&&(NativeRawPostScript.IsBlock(i)||NativeEpsExport.For(g)!=null&&NativeRawPostScript.IsRaw(i))){int rawStart=index;var rawItems=new List<Item>();do{var rawItem=Score.Items[index];if(!DisplayHidden(rawItem))rawItems.Add(rawItem);index++;}while(index<Score.Items.Count&&(NativeRawPostScript.IsBlock(Score.Items[index])||NativeEpsExport.For(g)!=null&&NativeRawPostScript.IsRaw(Score.Items[index])));index--;if(rawItems.Count>1)NativeItemBounds.Select(null);try{NativeRawPostScript.DrawItems(g,rawItems,PageWidth,PageHeight,TextLayout,PageDocument);}catch(Exception error){ReportDrawError(g,rawStart,error);}continue;}if(i.Code==15&&NativeNotationView.UsesDat){if(RenderResourceAudit.Current!=null)RenderResourceAudit.Current.Item(i);try{if(NativeDatDrawing.Item(g,i,Context(i),PageDocument,Library,TextLayout))continue;}catch(Exception error){if(!NativeApproximation.IsRenderFailure(error))throw;ReportDrawError(g,index,error);continue;}}if(i.Code==15){if(NativeNotationView.UsesDat){DrawingIssues.Add("DAT included PostScript: original screen representation is not yet verified; existing PostScript preview used.");RenderResourceAudit.Unavailable("DAT included PostScript","Original screen representation not yet verified; existing PostScript preview used.");}int includedStart=index;bool includedFailed=false;var body=new StringBuilder();var includedItems=new List<Item>();var includedPrograms=new List<string>();do{var ps=Score.Items[index];if(DisplayHidden(ps)){index++;continue;}if(RenderResourceAudit.Current!=null)RenderResourceAudit.Current.Item(ps);try{string path=IncludedPostScriptDisplay.Resolve(Library==null?null:Library.Root,Score.SourcePath,ps,Scor4Interface);string program=IncludedPostScriptProgram.Wrap(IncludedPostScriptProgram.Prepare(ps,Context(ps),Encoding.GetEncoding(28591).GetString(File.ReadAllBytes(path))));body.Append(program);includedItems.Add(ps);includedPrograms.Add(program);RenderResourceAudit.Record("included PostScript",IncludedPostScriptDisplay.FileName(ps),path,"external PostScript/EPS; recovered numeric declarations and transform controls");}catch(Exception e){includedFailed=true;ReportDrawError(g,index,e);}index++;}while(index<Score.Items.Count&&Score.Items[index].Code==15&&NativePrintExecution.Current==null);index--;if(index!=includedStart)NativeItemBounds.Select(includedItems.Count==1?includedItems[0]:null);if(body.Length>0&&(!includedFailed||NativeNotationView.UsesDat))try{var eps=NativeEpsExport.For(g);if(eps!=null)eps.Program(body.ToString());else PostScriptProgramPreview.Paint(g,body.ToString(),PageWidth,PageHeight,includedItems,includedPrograms);}catch(Exception e){ReportDrawError(g,includedStart,e);}IncludedPostScriptDisplay.Status=DrawingIssues.Count==0?"External PostScript programs rendered":String.Join("; ",DrawingIssues.ToArray());continue;}if(RenderResourceAudit.Current!=null)RenderResourceAudit.Current.Item(i);DrawItem(g,i,index);}
        NativeItemBounds.Select(null);if(NativeEpsExport.For(g)!=null)NativeEpsExport.For(g).ItemMarker(0);slurFrame.Finish(g,TextLayout);DrawInputNumbers(g);
        }finally{slurFrame=null;NativeDatDrawing.Report=null;}
        if(paper&&Selected!=null) { PointF p=Position(Selected); using(var pen=new Pen(Color.FromArgb(37,99,235),1.5f))g.VectorDrawRectangle(pen,p.X-8,p.Y-15,24,30); }
    }
    void ReportDrawError(Graphics g,int index,Exception error){
        if(error is NativePrintExitException||error is NativePrintFinishException)throw error;
        string message="Item "+(index+1)+" (Code "+Score.Items[index].Code+"): "+error.Message;
        DrawingIssues.Add(message);
        RenderResourceAudit.Unavailable("score item",message);
        if(NativeEpsExport.For(g)!=null||NativeNotationView.IsPrinting)throw new InvalidDataException(message,error);
    }
    static NotSupportedException Declined(string route){
        return new NotSupportedException("Verified SCORE rendering unavailable for "+route+".");
    }
    public bool Supported(Item i) {if(i.Code==11)return i.P(13)==0&&i.P(5)>=0&&i.P(5)<10&&i.P(8)==0&&i.P(10)==0;if(i.Code==15)return i.P(13)==0;if(i.Code==9){if(NativeCode9Terms.IsTerm((int)i.P(5)))return true;Glyph glyph;return Library!=null&&Library.Symbols.TryGetValue((int)i.P(5),out glyph)&&glyph.Points.Length>0;}if(i.Code==7)return new[]{0,8,-8,15,88,20,21,22}.Contains((int)i.P(7));if(i.Code==12)return i.P(5)==0||i.P(5)==1;return i.Code>=1&&i.Code<=18&&i.Code!=11&&i.Code!=15;}
    new float Scale(float value) {value=Math.Abs(value)%100;return value==0?1:value;}
    void Render(Graphics g,Item i) {
        if(i.Code==13||i.Code==19){RenderResourceAudit.Record("invisible layout item",i.Code.ToString(),"Tom's Help Codes 13 and 19; DAT nonpainting dispatch","no painted geometry; retained in the score and available to non-rendering consumers");return;}
        var page=Context(i);if(page==null&&!NativeNotationView.UsesDat)throw new NotSupportedException("SCORE page geometry is unavailable.");if(RenderResourceAudit.Route("NativeDatDrawing.Item",()=>NativeDatDrawing.Item(g,i,page,PageDocument,Library,TextLayout)))return;float x=page==null?X(i.P(3)):page.Point(page.Prepared.X,0).X,y=Y(i,i.P(4)),s=Step(i),b=Base(i);int code=i.Code;
        if(code==8) {if(PageDocument!=null)PageDocument.DrawStaff(g,i);}
        else if(NativeTablatureDrawing.IsTab(i)) {if(!RenderResourceAudit.Route("NativeTablatureDrawing.Draw",()=>NativeTablatureDrawing.Draw(g,i,page,TextLayout)))throw Declined("tablature note");}
        else if(NativePrintedTextHeads.IsAlias(i)){if(!RenderResourceAudit.Route("NativePrintedTextHeads.Draw",()=>NativePrintedTextHeads.Draw(g,i,page,Library,TextLayout,librarySymbolCache)))throw Declined("custom text-head state");}
        else if(code==1) {
           using(var printedBuffer=NativePrintedPointBuffer.Begin(g)){
            float p=page==null?i.P(4):page.Prepared.VerticalInput;
            int head=(int)Math.Abs(i.P(6))%10,headGlyph=i.P(6)>=100?(int)i.P(6):11+head;
            int acc=(int)Math.Abs(i.P(5))%10;
            Action drawAccidental=()=>{
                if(!RenderResourceAudit.Route("NativeNoteAttachments.DrawAccidental",()=>NativeNoteAttachments.DrawAccidental(g,i,page,Library,librarySymbolCache)))throw Declined("note accidental");
            };
            bool fingeringAfterHead=librarySymbolCache!=null&&librarySymbolCache.Contains(headGlyph);
            Func<bool> drawFingering=()=>RenderResourceAudit.Route("NativeNoteAttachments.DrawFingering",()=>NativeNoteAttachments.DrawFingering(g,i,page,TextLayout));
            bool fingering=!fingeringAfterHead&&drawFingering();
            bool joinedStem=head==5&&RenderResourceAudit.Route("NativePrintedHeads.DrawJoinedStem",
                ()=>NativePrintedHeads.DrawJoinedStem(g,i,page,Library,librarySymbolCache));
            if(!joinedStem&&(head!=7||!NativeNotationView.UsesDat)&&!RenderResourceAudit.Route("NativePrintedHeads.Draw",()=>NativePrintedHeads.Draw(g,i,page,Library,librarySymbolCache,TextLayout)))throw Declined("notehead");
            if(!fingering&&fingeringAfterHead)fingering=drawFingering();
            if(!RenderResourceAudit.Route("NativeNoteDetails.Ledgers",()=>NativeNoteDetails.Ledgers(g,i,page))&&(p<=1||p>=13)&&i.P(7)!=-2&&i.P(7)!=-3)throw Declined("ledger lines");
            if(!joinedStem&&!RenderResourceAudit.Route("NativeNoteDetails.Stem",()=>NativeNoteDetails.Stem(g,i,page)))throw Declined("note stem");
            int tails=(int)Math.Abs(i.P(9))%10;
            if(tails>0&&!RenderResourceAudit.Route("NativePrintedFlags.Draw",()=>NativePrintedFlags.Draw(g,i,page,Library,librarySymbolCache)))throw Declined("note flags");
            if(acc>0)drawAccidental();
            if(!RenderResourceAudit.Route("NativeNoteDetails.Dots",()=>NativeNoteDetails.Dots(g,i,page,Library,librarySymbolCache))&&(int)Math.Abs(i.P(9))/10>0)throw Declined("augmentation dots");
            if(!fingering&&i.P(11)!=0&&!RenderResourceAudit.Route("NativeNoteAttachments.DrawMarks",
                ()=>NativeNoteAttachments.DrawMarks(g,i,page,Library,librarySymbolCache)))
                throw new NotSupportedException("Verified SCORE rendering unavailable for note mark.");
            printedBuffer.Complete();
           }
        }
        else if(code==2&&RenderResourceAudit.Route("NativePrintedRests.Draw",()=>NativePrintedRests.Draw(g,i,page,Library,librarySymbolCache,TextLayout))) {}
        else if(code==3&&RenderResourceAudit.Route("NativePrintedClefs.Draw",()=>NativePrintedClefs.Draw(g,i,page,Library,TextLayout,librarySymbolCache))) {}
        else if((code==2||code==3||code==18)&&RenderResourceAudit.Route("NativePrintedSymbols.Draw",()=>NativePrintedSymbols.Draw(g,i,page,Library,librarySymbolCache))) {}
        else if(code==14&&RenderResourceAudit.Route("NativePrintedRepeatBars.Draw",()=>NativePrintedRepeatBars.Draw(g,i,page,PageDocument))) {}
        else if(code==14&&RenderResourceAudit.Route("NativePrintedBarlines.Draw",()=>NativePrintedBarlines.Draw(g,i,page,PageDocument,Library))) {}
        else if(code==4&&RenderResourceAudit.Route("NativePrintedLines.Draw",()=>NativePrintedLines.Draw(g,i,page))) {}
        else if(code==4&&RenderResourceAudit.Route("NativeHairpins.Draw",()=>NativeHairpins.Draw(g,i,page))) {}
        else if(code==5&&RenderResourceAudit.Route("NativeEndingDisplay.Draw",()=>NativeEndingDisplay.Draw(g,i,s,b,TextLayout,Library,page))) {}
        else if(code==5&&RenderResourceAudit.Route("NativeTupletDisplay.Draw",()=>NativeTupletDisplay.Draw(g,i,s,b,TextLayout,page))) {}
        else if(code==5&&slurFrame!=null&&slurFrame.Add(g,i,s,b,TextLayout,page)) {}
        else if(code==6&&RenderResourceAudit.Route("NativeBeamDisplay.Draw",()=>NativeBeamDisplay.Draw(g,i,s,b,TextLayout,page))) {}
        else if(code==16&&TextLayout!=null)TextLayout.Draw(g,i,i.Text,x,y,page==null?s:page.Scale/5,page==null?(float?)null:page.Scale,page);
        else if(code==18&&RenderResourceAudit.Route("NativeMeterDisplay.Draw",()=>NativeMeterDisplay.Draw(g,i,page,TextLayout,Library))) {}
        else if(code==17&&RenderResourceAudit.Route("NativeKeySignatures.Draw",()=>NativeKeySignatures.Draw(g,i,page,Library,librarySymbolCache))) {}
        else if(code==7&&RenderResourceAudit.Route("NativeTrillGlyph.Draw",()=>NativeTrillGlyph.Draw(g,i,page,Library,librarySymbolCache))) {}
        else if(code==7&&RenderResourceAudit.Route("NativePrintedOrnaments.Draw",()=>NativePrintedOrnaments.Draw(g,i,page,Library,TextLayout,librarySymbolCache))) {}
        else if(code==7)throw new NotSupportedException("SCORE ornament controls outside the verified routes.");
        else if(code==11&&RenderResourceAudit.Route("NativeUserDrawingDisplay.Draw",()=>NativeUserDrawingDisplay.Draw(g,i,page,Library))) {}
        else if(code==12&&RenderResourceAudit.Route("NativePrintedShapes.Draw",()=>NativePrintedShapes.Draw(g,i,page,Library))) {}
        else if(code==12&&RenderResourceAudit.Route("NativePrintedChordGrid.Draw",()=>NativePrintedChordGrid.Draw(g,i,page,TextLayout,Library))) {}
        else if(code==12)throw new NotSupportedException("SCORE shape controls outside the verified routes.");
        else if(code==9&&RenderResourceAudit.Route("NativeLibrarySymbols.Draw",()=>NativeLibrarySymbols.Draw(g,i,page,Library,TextLayout,librarySymbolCache))) {}
        else if(code==9)throw new NotSupportedException("SCORE library-symbol controls outside the verified routes.");
        else if(code==10){
            int index=Score.Items.IndexOf(i);
            var preceding=index>0?Score.Items[index-1]:null;
            if(!RenderResourceAudit.Route("NativeNumberDisplay.Draw",()=>NativeNumberDisplay.Draw(g,i,page,TextLayout,preceding)))
                throw new NotSupportedException("SCORE number controls outside the verified routes.");
        }
        else throw Declined("Code "+code+" controls");
    }
    void UpdateHover(PointF? location){
        bool changed=NativeMouseSelector.HoverChanged(this,HoverPoint,location);
        HoverPoint=location;
        if(changed){
            hoverInvalidation=true;
            try{Invalidate();}finally{hoverInvalidation=false;}
        }
    }
    protected override void OnMouseLeave(EventArgs e){base.OnMouseLeave(e);GuidePointer(null);UpdateHover(null);}
    protected override void OnMouseCaptureChanged(EventArgs e){base.OnMouseCaptureChanged(e);if(!Capture&&MouseGesture!=null){gestureHeld=0;gestureButtons=0;gestureLatchedButtons=0;MouseOperationLocked=false;if(MouseGestureCancelled!=null)MouseGestureCancelled();}}
    static int MouseBit(MouseButtons button){return button==MouseButtons.Left?1:button==MouseButtons.Right?2:button==MouseButtons.Middle?4:0;}
    protected override void WndProc(ref Message m){if(m.Msg==0x7b&&MouseGesture!=null&&m.LParam.ToInt64()!=-1)return;base.WndProc(ref m);}
    protected override void OnMouseDown(MouseEventArgs e) {if(WorkspaceMouse(e))return;base.OnMouseDown(e);var viewport=MouseGesture==null?null:Parent as ScrollableControl;Point scroll=viewport==null?Point.Empty:viewport.AutoScrollPosition;Focus();if(viewport!=null)viewport.AutoScrollPosition=new Point(-scroll.X,-scroll.Y);if(Score==null)return;if(MouseGesture!=null){if(gestureHeld==0){gestureStart=e.Location;gestureModifiers=ModifierKeys;gestureButtons=0;gestureLatchedButtons=0;MouseOperationLocked=false;}if(MouseBit(e.Button)==0)return;gestureHeld|=MouseBit(e.Button);gestureButtons=Math.Max(gestureButtons,gestureHeld);Capture=true;return;}if(NativeNotationView.IsDosViewport&&e.Button==MouseButtons.Middle){viewportPan=true;dragStart=e.Location;panX=NativeDosViewport.OriginX;panY=NativeDosViewport.OriginY;Capture=true;return;}if(BlockItemEditing)return;if(Selected==null&&e.Button==MouseButtons.Left&&ModifierKeys==Keys.None){mainSelectionPending=true;Capture=true;return;}Item next;try{next=HitTest(e.Location);}catch(NotSupportedException error){if(DisplayIssue!=null)DisplayIssue(error.Message);return;}Selected=next;if(Picked!=null)Picked(Selected);Invalidate();if(Selected!=null&&e.Button==MouseButtons.Left){dragStart=e.Location;dragX=Selected.P(3);dragY=Selected.P(4);dragging=false;Capture=true;}}
    protected override void OnMouseMove(MouseEventArgs e) {base.OnMouseMove(e);GuidePointer(e.Location);UpdateHover(e.Location);if(MouseGesture!=null){if(gestureHeld!=0){MouseGesture(gestureLatchedButtons==0?gestureButtons:gestureLatchedButtons,gestureModifiers,e.Location,gestureStart,false);if(MouseOperationLocked&&gestureLatchedButtons==0)gestureLatchedButtons=gestureButtons;}return;}if(viewportPan&&Capture){NativeDosViewport.OriginX=unchecked((short)(panX+(e.X-dragStart.X)/RenderScale));NativeDosViewport.OriginY=unchecked((short)(panY+(e.Y-dragStart.Y)/RenderScale));Invalidate();return;}if(mainSelectionPending||BlockItemEditing||!Capture||Selected==null||e.Button!=MouseButtons.Left)return; if(!dragging&&Math.Abs(e.X-dragStart.X)+Math.Abs(e.Y-dragStart.Y)>5) {if(BeforeDrag!=null)BeforeDrag();dragging=true;}if(dragging){Selected.Set(3,dragX+(e.X-dragStart.X)/RenderScale*(NativeNotationView.IsDosViewport?NativeDosViewport.Divisor/150f:1/5f));Selected.Set(4,dragY-(e.Y-dragStart.Y)/RenderScale*(NativeNotationView.IsDosViewport?NativeDosViewport.Divisor/(Context(Selected).Scale*7):1/Step(Selected)));if(NativeNotationView.IsDosViewport)LayoutScore();Invalidate();} }
    protected override void OnMouseUp(MouseEventArgs e) {base.OnMouseUp(e);if(MouseGesture!=null){gestureHeld&=~MouseBit(e.Button);if(gestureHeld==0&&gestureButtons!=0){int buttons=gestureLatchedButtons==0?gestureButtons:gestureLatchedButtons;gestureButtons=0;MouseGesture(buttons,gestureModifiers,e.Location,gestureStart,true);gestureLatchedButtons=0;MouseOperationLocked=false;Capture=false;}return;}if(viewportPan){viewportPan=false;Capture=false;return;}if(mainSelectionPending&&e.Button==MouseButtons.Left){mainSelectionPending=false;Capture=false;try{var item=HitTest(e.Location);if(Picked!=null)Picked(item);else Selected=item;Invalidate();}catch(NotSupportedException error){if(DisplayIssue!=null)DisplayIssue(error.Message);}return;}Capture=false;if(dragging){dragging=false;if(Changed!=null)Changed();} }
}
public sealed partial class Editor : Form {
    static readonly Icon ProgramIcon = LoadProgramIcon();

    static Icon LoadProgramIcon() {
        using(var stream=typeof(Editor).Assembly.GetManifestResourceStream("ScoreModern.Program.ico"))
            return new Icon(stream);
    }
    Score score; string current=""; bool dirty,updating; string source; Canvas canvas=new Canvas(); Panel viewport=new Panel(); ListView list=new ListView(); DataGridView grid=new DataGridView(); Label status=new Label(); Label detail=new Label(); ComboBox files=new ComboBox(); ComboBox types=new ComboBox(); NumericUpDown staff=new NumericUpDown(); ToolStrip tools=new ToolStrip(); Stack<Score> undo=new Stack<Score>(),redo=new Stack<Score>();
    public Editor(string root,string initial) {
        Icon=ProgramIcon;
        root=Path.GetFullPath(root);
        NativeLibraryLocation.LoadScor4(root);
        NativeLibraryLocation.SetProfile(root,ReadScor4InterfacePreference());
        NativeScoreIdentity.Require(root,!Program.CheckMode);NativePreferences.Validate(root);source=root;canvas.Library=new DrawingLibrary(root);canvas.TextLayout=new ScoreText(root);
        InitializeClassic(initial);
    }
    void Button(string text,Action a) { var b=new ToolStripButton(text);b.Click+=(s,e)=>Safe(a);tools.Items.Add(b); }
    void Safe(Action a) {try{a();}catch(Exception e){NativeStartupSplash.CloseActive();if(Program.CheckMode)throw;MessageBox.Show(this,e.Message,"SCORE Modern",MessageBoxButtons.OK,MessageBoxIcon.Error);} }
    bool MayDiscard() {UpdateDirty();if(!dirty)return true;var r=unsavedDecision!=null?unsavedDecision():MessageBox.Show(this,"Save your changes before continuing?","Unsaved changes",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);if(r==DialogResult.Cancel)return false;if(r==DialogResult.Yes){try{return SaveCurrent();}catch(Exception error){ReportDocumentError(error);return false;}}return true;}
    void LoadFile(string path,bool discardConfirmed=false,Score prepared=null) {Safe(()=>{path=Path.GetFullPath(path);var next=prepared??Score.Load(path);using(canvas.TextLayout.PreserveAlignmentMetricHistory())using(var probe=new Canvas()){probe.Score=next;probe.Library=canvas.Library;probe.TextLayout=canvas.TextLayout;probe.LayoutScore();}if(!discardConfirmed&&!MayDiscard())return;string prior=current;SetDocument(next,path);if(scor4Interface&&!String.IsNullOrEmpty(prior)&&String.Equals(current,path,StringComparison.OrdinalIgnoreCase)&&!String.Equals(prior,current,StringComparison.OrdinalIgnoreCase))scor4PreviousFile=prior;Array.Clear(graphicalMenuStaffs,0,graphicalMenuStaffs.Length);});}
    public void StartupNotice(string message){if(String.IsNullOrEmpty(message))return;Message(String.IsNullOrEmpty(responseLine.Text)?message:responseLine.Text+"\r\n"+message);}
    void NewScore() {if(externalDrawingNameItem!=null){command.Focus();return;}if(!MayDiscard())return;SetDocument(NativeMeasurements.New(source),"");}
    void RestartScore() {
        if(!MayDiscard())return;
        ResetDocument();
    }
    void ResetDocument() {
        var empty=NativeMeasurements.New(source);
        if(score!=null)empty.Trailer=(byte[])score.Trailer.Clone();
        empty.Items.Clear();
        SetDocument(empty,scor4Interface?"":current??"",restartDisplay:true);
    }
    void Open() {if(externalDrawingNameItem!=null){command.Focus();return;}using(var d=new OpenFileDialog{Filter="SCORE documents (*.mus;*.pag)|*.mus;*.pag|All files (*.*)|*.*",InitialDirectory=NativeOpenFolder.InitialDirectory(source),RestoreDirectory=true})if(d.ShowDialog(this)==DialogResult.OK)LoadFile(d.FileName);}
    bool SaveCopy() {if(!CanSaveDocument()||score==null||!grid.EndEdit())return false;if(savePathSelection!=null){string chosen=savePathSelection();return !String.IsNullOrEmpty(chosen)&&SaveNamed(chosen,false);}using(var d=new SaveFileDialog{Filter="SCORE music (*.mus)|*.mus",FileName=String.IsNullOrEmpty(current)?"Untitled.MUS":Path.GetFileNameWithoutExtension(current)+"-edited.MUS",InitialDirectory=NativeOutputLocation.EnsureFolder(source),RestoreDirectory=true}) {if(d.ShowDialog(this)!=DialogResult.OK)return false;return SaveNamed(d.FileName,false);} }
    void Checkpoint() {if(originalCancelledLyricBuffer&&NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Editing after cancelled lyrics can reuse the wrong item in SCORE. Reload the score, or turn off original invalid-state behaviour, before continuing.");InvalidateParameterStorage();if(score!=null)score.RawStorageLayoutVerified=false;if(mode==ScreenMode.Edit&&canvas.Selected!=null)canvas.EditModified=true;undo.Push(score.Clone());redo.Clear();}
    void PickSymbol() {using(var picker=new SymbolPicker(canvas.Library)){if(picker.ShowDialog(this)!=DialogResult.OK)return;Checkpoint();Item item=Item.Make(9,(float)staff.Value,50,-3,picker.SymbolNumber,1);score.Items.Add(item);dirty=true;RefreshAll(item);}}
    void ApplyTextValue(string value){Item original=canvas.Selected!=null&&canvas.Selected.Code==16?canvas.Selected:null;Item next=original==null?Item.Make(16,(float)staff.Value,50,20,1,1):original.Clone();Item st=score.Items.FirstOrDefault(i=>i.Code==8&&i.Staff==next.Staff);float size=st==null||st.P(5)==0?1:Math.Abs(st.P(5));canvas.TextLayout.Set(next,value,size);using(var display=BeginDisplayTextChange(original,next)){
     Checkpoint();
     if(original==null)score.Items.Add(next);else score.Items[score.Items.IndexOf(original)]=next;
     dirty=true;RefreshAll(next);display.Complete();
    }}
    void Undo(bool forward) {if(externalDrawingNameItem!=null){command.Focus();return;}var from=forward?redo:undo;var to=forward?undo:redo;if(from.Count==0)return;to.Push(score.Clone());var trailer=(byte[])score.Trailer.Clone();score=from.Pop();originalLayoutMarksModified=false;score.Trailer=trailer;score.SourcePath=String.IsNullOrEmpty(current)?"":Path.GetFullPath(current);ClearEditing();deletedItem=null;dirty=true;RefreshAll(null);LayoutClassic();}
    void RefreshAll(Item selected) {RefreshAllCore(selected,true);}
    void RefreshAllCore(Item selected,bool applyPendingStaffs) {if(mode==ScreenMode.Edit&&selected!=null&&selected.Code==16&&score.Items.Contains(selected))RefreshTextCaches(selected);ApplyOriginalDisplayState(applyPendingStaffs);UpdateDirty();updating=true;canvas.Score=score;canvas.Selected=selected;canvas.LayoutScore();list.BeginUpdate();list.Items.Clear();int n=0;foreach(var i in score.Items) {var row=new ListViewItem((++n).ToString());row.SubItems.Add(Item.Name(i.Code));row.SubItems.Add(i.Staff.ToString());row.Tag=i;list.Items.Add(row);if(i==selected)row.Selected=true;}list.EndUpdate();updating=false;SelectItem(selected);Text=(dirty?"* ":"")+(String.IsNullOrEmpty(current)?"Untitled":Path.GetFileName(current))+" — SCORE Modern "+BuildVersion.Version;int approx=score.Items.Count(i=>!canvas.Supported(i));status.Text=score.Items.Count+" objects  ·  "+score.Items.Select(i=>i.Staff).Distinct().Count()+" staves  ·  "+approx+" unsupported objects retained  ·  "+canvas.Library.Symbols.Values.Count(g=>g.Points.Length>0)+" symbols  ·  "+canvas.Library.Warnings.Count+" library warnings  ·  Approximate engraving"; }
    string ParamName(Item i,int n) {if(n==1)return "P1 · Object code";if(n==2)return "P2 · Staff";if(n==3)return "P3 · Horizontal";if(n==4)return "P4 · Vertical";if(i.Code==1) {string[] a={"Stem / accidental","Notehead","Duration","Stem length","Dots / flags"};if(n<=9)return "P"+n+" · "+a[n-5];}if(i.Code==8&&n==5)return "P5 · Staff scale";if(i.Code==8&&n==6)return "P6 · Right edge";return "P"+n; }
    void SelectItem(Item i) {if(score!=null)foreach(var item in score.Items)item.EditingExternalDrawingReset=ReferenceEquals(item,i)&&mode==ScreenMode.Edit&&item.Count>=13&&item.ExternalDrawingResetResolved&&(item.Code==11&&BitConverter.ToSingle(item.Bytes,52)==1||item.Code==15&&BitConverter.ToSingle(item.Bytes,52)!=0);updating=true;canvas.Selected=i;grid.Rows.Clear();detail.Text=i==null?"OBJECT INSPECTOR\nSelect a symbol":Item.Name(i.Code)+"\nStaff "+i.Staff; if(i!=null) {int max=i.Code==16?Math.Min(11,i.Count):Math.Max(i.Count,9);for(int n=1;n<=max;n++) {int row=grid.Rows.Add(ParamName(i,n),i.P(n).ToString("G9",CultureInfo.InvariantCulture));grid.Rows[row].Tag=n;if(n<=2)grid.Rows[row].ReadOnly=true;}if(i.Code==16) {int row=grid.Rows.Add("Text · read only",i.Text);grid.Rows[row].ReadOnly=true;} }
        foreach(ListViewItem row in list.Items)if(row.Tag==i){row.Selected=true;break;}canvas.Invalidate();updating=false;UpdateClassic(); }
    void ValidateCell(object sender,DataGridViewCellValidatingEventArgs e) {if(updating||e.ColumnIndex!=1||grid.Rows[e.RowIndex].ReadOnly)return;float v;bool ok=float.TryParse(Convert.ToString(e.FormattedValue),NumberStyles.Float,CultureInfo.InvariantCulture,out v)&&!float.IsNaN(v)&&!float.IsInfinity(v)&&Math.Abs(v)<=100000;grid.Rows[e.RowIndex].ErrorText=ok?"":"Enter a finite number between -100000 and 100000.";e.Cancel=!ok;}
    void EditCell(object sender,DataGridViewCellEventArgs e) {if(updating||e.ColumnIndex!=1||canvas.Selected==null||e.RowIndex<0)return;int n=(int)grid.Rows[e.RowIndex].Tag;float v;if(!float.TryParse(Convert.ToString(grid.Rows[e.RowIndex].Cells[1].Value),NumberStyles.Float,CultureInfo.InvariantCulture,out v))return;if(canvas.Selected.P(n)==v)return;Checkpoint();canvas.Selected.Set(n,v);dirty=true;RefreshAll(canvas.Selected);}
    void Add() {if(score==null)return;Checkpoint();int[] codes={1,2,3,4,5,6,8,10,14,17,18};int c=codes[types.SelectedIndex];Item i=Item.Make(c,(float)staff.Value,50,4,0,0,1);if(c==1)i=Item.Make(c,(float)staff.Value,50,6,10,0,1);if(c==8)i=Item.Make(c,(float)staff.Value,0,0,1,200);if(c==14)i=Item.Make(c,(float)staff.Value,100,1);if(c==18)i=Item.Make(c,(float)staff.Value,15,0,4,4);if(c==4||c==5||c==6)i=Item.Make(c,(float)staff.Value,50,12,12,80,c==6?11:3);score.Items.Add(i);dirty=true;RefreshAll(i);}
    void Delete() {if(canvas.Selected==null)return;Checkpoint();score.Items.Remove(canvas.Selected);dirty=true;RefreshAll(null);}
    void Zoom(float amount) {canvas.Zoom=Math.Max(.25f,Math.Min(16,canvas.Zoom+amount));canvas.LayoutScore();UpdateClassic();}
    void Export() {using(var d=new SaveFileDialog{Filter="PNG image (*.png)|*.png",FileName="score-preview.png",InitialDirectory=NativeOutputLocation.EnsureFolder(source),RestoreDirectory=true})if(d.ShowDialog(this)==DialogResult.OK) {using(var b=new Bitmap((int)Math.Ceiling(canvas.ViewWidth),(int)Math.Ceiling(canvas.ViewHeight)))using(var g=Graphics.FromImage(b)){g.Clear(Color.White);canvas.Draw(g,false);b.Save(NativeOutputLocation.RequireWritableDestination(source,d.FileName),System.Drawing.Imaging.ImageFormat.Png);}}}
    void Reference() {if(!OpenModernHelp(ModernReferenceTopic()))ShowHelpTopic("START");}
}
static class Program {
    public static bool CheckMode;
    [STAThread] static int Main(string[] args) {
        bool openAssociatedFile=args.Length==2&&args[0]=="--open";
        CheckMode=args.Length>0&&args[0].StartsWith("--")&&!openAssociatedFile;
        bool stylesReady=false;string setupRoot=null;
        if(!CheckMode&&NativeSettings.NeedsFirstSetup()){Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);stylesReady=true;setupRoot=NativeFirstSetup.Show(!openAssociatedFile&&args.Length>0?args[0]:null);if(setupRoot==null)return 0;}
        string startupRoot=openAssociatedFile?NativeStartupFolder.Preferred:args.Length==0?NativeStartupFolder.Preferred:args[0].StartsWith("--")?null:args[0];
        if(setupRoot!=null)startupRoot=setupRoot;
        NativeScoreBehaviour.Load();NativeDrawingNames.Load();NativePageFraming.Load();NativeUiFont.Load();NativeFontInitialization.Load();IncludedPostScriptDisplay.Load();NativeLibraryLocation.Load(startupRoot);NativeScoreIdentity.Load();NativeDisplay.Initialize();NativeInterfaceText.Load();TextCachePolicy.Load();NativeMouseSelector.Load();NativeStartupSplash.LoadSettings();NativeNotationView.Load();NativeApproximation.Load();NativeBeamDisplay.Load();NativeTupletFonts.Load();GhostscriptTextRenderer.Load();NativeSlurDisplay.Load();NativeEditCompatibility.Load();NativeEndingDisplay.Load();foreach(string arg in args){if(arg=="--approximate-rendering")NativeApproximation.Set(true,false);if(arg=="--strict-rendering")NativeApproximation.Set(false,false);if(arg=="--fontinit=external")NativeFontInitialization.Set(true,false);if(arg=="--fontinit=internal")NativeFontInitialization.Set(false,false);if(arg=="--notation=vector")NativeNotationView.Set(NotationView.Vector,false);if(arg=="--notation=dat")NativeNotationView.Set(NotationView.Dat,false);if(arg=="--notation=dat-pixels")NativeNotationView.Set(NotationView.DatPixels,false);if(arg=="--notation=dos-viewport")NativeNotationView.Set(NotationView.DatViewport,false);if(arg=="--text-renderer=ghostscript")GhostscriptTextRenderer.Enabled=true;if(arg=="--text-renderer=native")GhostscriptTextRenderer.Enabled=false;if(arg.StartsWith("--ghostscript="))GhostscriptTextRenderer.Executable=arg.Substring(14);if(arg=="--drawing-names=documented")NativeDrawingNames.Set(true,false);if(arg=="--drawing-names=original")NativeDrawingNames.Set(false,false);if(arg=="--endings=toms")NativeEndingDisplay.Set(true,false);if(arg=="--endings=original")NativeEndingDisplay.Set(false,false);if(arg.StartsWith("--type1-fonts="))NativeTupletFonts.SetDirectory(arg.Substring(14),false);if(arg.StartsWith("--vga-font="))NativeInterfaceText.SetVga(arg.Substring(11),false);if(arg=="--consolas")NativeInterfaceText.SetConsolas(false);if(arg=="--edit-invalid=original")NativeEditCompatibility.Set(true,false);if(arg=="--edit-invalid=prevent")NativeEditCompatibility.Set(false,false);if(arg=="--slur-trims=reject")NativeSlurDisplay.SetReject(true,false);if(arg=="--slur-trims=original")NativeSlurDisplay.SetReject(false,false);if(arg=="--beams=printed")NativeBeamDisplay.Set(ScoreModern.RecoveredFragments.BeamAppearance.Printed,false);if(arg=="--beams=dos")NativeBeamDisplay.Set(ScoreModern.RecoveredFragments.BeamAppearance.DosScreen,false);}
        foreach(string arg in args){if(arg=="--approximate-rendering")NativeApproximation.Set(true,false);if(arg=="--strict-rendering")NativeApproximation.Set(false,false);if(arg=="--fontinit=external")NativeFontInitialization.Set(true,false);if(arg=="--fontinit=internal")NativeFontInitialization.Set(false,false);if(arg=="--ps-search=score")IncludedPostScriptDisplay.Set(false,false);if(arg=="--ps-search=score-and-mus")IncludedPostScriptDisplay.Set(true,false);}

        if(CheckMode&&args[0]!="--verify-display"){try{string dataRoot=args.Length>1?args[1]:NativeStartupFolder.Preferred;if(args[0]=="--render")dataRoot=args.Length>3?args[3]:NativeStartupFolder.Preferred;if(args[0]=="--verify")dataRoot=String.Equals(Path.GetFileName(Path.GetFullPath(args[1]).TrimEnd(Path.DirectorySeparatorChar)),"MUSDAT",StringComparison.OrdinalIgnoreCase)?Path.GetDirectoryName(Path.GetFullPath(args[1]).TrimEnd(Path.DirectorySeparatorChar)):NativeStartupFolder.Preferred;NativeScoreIdentity.Require(dataRoot,false);NativeLibraryLocation.SetProfile(dataRoot,args.Contains("--score-profile=scor4")?true:args.Contains("--score-profile=311")?false:Editor.ReadScor4InterfacePreference());}catch(Exception error){Console.Error.WriteLine(error.Message);return 1;}}
        if(args.Length>0&&args[0]=="--verify") {try {int total=0;foreach(string path in Directory.GetFiles(args[1],"*.MUS")) {var s=Score.Load(path);if(!File.ReadAllBytes(path).SequenceEqual(s.Encode()))throw new Exception("Round-trip differs: "+path);var edited=s.Clone();Item obj=edited.Items.First();float old=obj.P(3);obj.Set(3,old+1);string tmp=Path.GetTempFileName();try {File.WriteAllBytes(tmp,edited.Encode());var again=Score.Load(tmp);if(again.Items[0].P(3)!=old+1)throw new Exception("Edit mismatch");}finally{File.Delete(tmp);} Console.WriteLine(Path.GetFileName(path)+": "+s.Items.Count+" objects; exact round-trip and edit passed");total++;}var fresh=Score.New();string t=Path.GetTempFileName();try{File.WriteAllBytes(t,fresh.Encode());Score.Load(t);}finally{File.Delete(t);}Console.WriteLine("PASS: "+total+" samples; new document passed");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;} }
        if(args.Length>0&&args[0]=="--verify-library")return RegressionChecks.Library(args[1],args[2]);
        if(args.Length>0&&args[0]=="--verify-text")return RegressionChecks.Text(args[1]);
        if(args.Length>0&&args[0]=="--render") {try {var score=Score.Load(args[1]);using(var c=new Canvas()){c.Library=new DrawingLibrary(args.Length>3?args[3]:NativeStartupFolder.Preferred);c.TextLayout=new ScoreText(args.Length>3?args[3]:NativeStartupFolder.Preferred);c.Score=score;c.LayoutScore();using(var b=new Bitmap(1240,(int)c.PageHeight+60))using(var g=Graphics.FromImage(b)){g.Clear(Color.White);c.Draw(g,false);b.Save(args[2],System.Drawing.Imaging.ImageFormat.Png);}}return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;} }
        if(!stylesReady){Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);}
        if(args.Length>0&&args[0]=="--verify-slurs")return NativeSlurChecks.Verify(args[1]);
        if(args.Length>0&&args[0]=="--verify-beams")return NativeBeamChecks.Verify(args[1]);
        if(args.Length>0&&args[0]=="--verify-display")return NativeDisplayChecks.Verify(args[1]);
        if(args.Length>0&&args[0]=="--audit-render")return RenderResourceAudit.Run(args);
        if(args.Length>2&&args[0]=="--verify-input")return NativeInputChecks.Run(args);
        if(args.Length>0&&args[0]=="--export-eps")return NativeEpsExport.Run(args);
        if(args.Length>0&&args[0]=="--render-score")return NativeDisplayChecks.RenderScore(args);
        if(args.Length>0&&args[0]=="--render-ui")return ClassicChecks.Render(args);
        if(args.Length>2&&args[0]=="--verify-notation-matrix")return NotationMatrixChecks.Run(args[1],args[2]);
        if(args.Length>2&&args[0]=="--verify-filenames")return FilenameSequenceChecks.Run(args[1],args[2]);
        if(args.Length>3&&args[0]=="--verify-interface-fonts")return NativeInterfaceFontChecks.Run(args);
        if(args.Length>3&&args[0]=="--verify-zoom-framing")return NativeZoomFramingChecks.Run(args);
        if(args.Length>3&&args[0]=="--verify-integrated-ui")return NativeIntegratedUiChecks.Run(args);
        if(args.Length>3&&args[0]=="--verify-workspace-libraries")return NativeWorkspaceLibraryChecks.Run(args);
        if(args.Length>0&&args[0]=="--verify-disk-dialog-choice")return NativeDiskDialogChoiceChecks.Run(args[1]);
        if(args.Length>0&&args[0]=="--verify-print-menu-choice")return NativePrintMenuChoiceChecks.Run(args[1]);
        if(args.Length>0&&args[0]=="--profile-interactions")return NativeInteractionPerformance.Run(args);
        if(args.Length>0&&args[0]=="--verify-classic")return ClassicChecks.Verify(args[1]);
        if(args.Length>0&&args[0]=="--smoke") {try {using(var editor=new Editor(args[1],Path.Combine(args[1],"MUSDAT","PRACTICE.MUS"))){editor.Show();Application.DoEvents();editor.Close();}Console.WriteLine("PASS: Windows editor created, sample opened, layout displayed, and closed.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
        try{
            string root=startupRoot??NativeStartupFolder.Preferred;
            if((args.Length==0||openAssociatedFile)&&!NativeStartupFolder.HasLibrary(root)){
                root=NativeStartupFolder.Choose(null,root);
                if(root==null)return 0;
                NativeLibraryLocation.Load(root);
                NativeStartupFolder.Save(root);
            }
            string initial=NativeStartupFolder.InitialFile(openAssociatedFile?Path.GetFullPath(args[1]):args.Length>1?args[1]:null);
            NativeStartupSplash.ShowFor(root);var editor=new Editor(root,initial);editor.Shown+=(s,e)=>{NativeStartupSplash.CloseAfterMinimum();if(NativeStartupSplash.Status.StartsWith("Original splash unavailable:")||NativeStartupSplash.Status.Contains("Splash logo unavailable:"))editor.StartupNotice(NativeStartupSplash.Status);var issues=NativePreferences.Read(root).Issues;if(issues.Count>0)editor.StartupNotice(String.Join("\r\n",issues.ToArray()));};Application.Run(editor);return 0;}catch(Exception error){NativeStartupSplash.CloseActive();MessageBox.Show(error.Message,"SCORE startup",MessageBoxButtons.OK,MessageBoxIcon.Error);return 1;}finally{NativeStartupSplash.CloseActive();}
    }
}
}












