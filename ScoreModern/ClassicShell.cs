using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScoreModern {
public static class NativeDisplay {
    [DllImport("user32.dll")]static extern bool SetProcessDpiAwarenessContext(IntPtr value);
    [DllImport("user32.dll")]static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")]static extern uint GetDpiForWindow(IntPtr window);
    public static void Initialize(){try{SetProcessDpiAwarenessContext(new IntPtr(-4));}catch(EntryPointNotFoundException){SetProcessDPIAware();}}
    public static float Scale(Control control){try{return GetDpiForWindow(control.Handle)/96f;}catch(EntryPointNotFoundException){return 1;}}
}
public sealed class ParameterHeader : Control {
    Item item;
    public Item Item {get{return item;} set{if(item!=value){EndInline(false);highlighted=0;}item=value;}}
    public Action<int> Edit;
    internal Func<int,string,bool> ApplyInline;
    sealed class ValueEditor:TextBox {
        protected override bool IsInputKey(Keys key){
            Keys code=key&Keys.KeyCode;
            return code==Keys.Enter||code==Keys.Escape||code==Keys.Up||code==Keys.Down||base.IsInputKey(key);
        }
    }
    internal Func<bool> CanEditInline;
    readonly TextBox valueEditor=new ValueEditor{BorderStyle=BorderStyle.FixedSingle,AutoSize=false,Visible=false};
    int parameter,highlighted;
    internal int ArrowParameter;internal bool TextNumericTail;internal Func<int,float> DisplayValue;
    internal Color SelectionColor=SystemColors.Highlight;
    internal bool EditingValue{get{return valueEditor.Visible;}}
    float CellWidth{get{return ClientSize.Width/11f;}}
    float Em{get{return Math.Max(9,Math.Min(Font.Unit==GraphicsUnit.Pixel?Font.Size:Height/(3*Math.Max(1.15f,NativeUiFont.LineRatio)),CellWidth/7));}}
    float RowPitch{get{return Height/3f;}}
    public ParameterHeader(){
        DoubleBuffered=true;
        SetStyle(ControlStyles.Selectable,true);
        Controls.Add(valueEditor);
        valueEditor.KeyDown+=(s,e)=>{
            if(e.KeyCode==Keys.Up||e.KeyCode==Keys.Down){
                e.SuppressKeyPress=true;
                MoveParameterRow(e.KeyCode==Keys.Down,true);
                return;
            }
            if(e.KeyCode!=Keys.Enter&&e.KeyCode!=Keys.Escape)return;
            e.SuppressKeyPress=true;
            EndInline(e.KeyCode==Keys.Enter);
            if(!EditingValue)Focus();
        };
        valueEditor.LostFocus+=(s,e)=>EndInline(false);
    }
    protected override bool IsInputKey(Keys key){
        Keys code=key&Keys.KeyCode;
        return highlighted!=0&&(code==Keys.Enter||code==Keys.Up||code==Keys.Down)||base.IsInputKey(key);
    }
    internal bool HandleHighlightedKey(KeyEventArgs e){
        if(!Focused||highlighted==0||Item==null||CanEditInline!=null&&!CanEditInline())return false;
        if(e.KeyCode==Keys.Up||e.KeyCode==Keys.Down)MoveParameterRow(e.KeyCode==Keys.Down,false);
        else if(e.KeyCode==Keys.Enter)OpenParameterEditor(highlighted);
        else return false;
        e.SuppressKeyPress=true;
        return true;
    }
    bool EditableParameter(int n){
        return Item!=null&&n>=1&&n<=19&&(n<=10||n<=Item.Count)&&(Item.Code!=16||n<12);
    }
    void MoveParameterRow(bool down,bool editing){
        if(down==(highlighted>10))return;
        int next=highlighted+(down?10:-10);
        if(!EditableParameter(next))return;
        if(editing){
            EndInline(true);
            if(EditingValue)return;
            OpenParameterEditor(next);
        }else{highlighted=next;Invalidate();}
    }
    void DrawValue(Graphics graphics,Font font,Brush ink,int n,float x,float y){
        string value=(DisplayValue==null?Item.P(n):DisplayValue(n)).ToString("0.###",CultureInfo.InvariantCulture);
        if(n!=ArrowParameter&&(n!=highlighted||!ContainsFocus)){NativeInterfaceText.Draw(graphics,value,font,ink,x,y);return;}
        using(var background=new SolidBrush(SelectionColor))
        using(var text=new SolidBrush(SelectionColor.GetBrightness()>.5f?Color.Black:Color.White)){
            graphics.FillRectangle(background,x,y,CellWidth,RowPitch);
            NativeInterfaceText.Draw(graphics,value,font,text,x,y);
        }
    }
    void PlaceEditor(){
        int column=parameter>10?parameter-10:parameter;
        int row=parameter>10?2:1;
        valueEditor.Bounds=new Rectangle((int)(column*CellWidth),(int)(row*RowPitch),
            Math.Max(1,(int)CellWidth-2),Math.Max(valueEditor.PreferredHeight,(int)Math.Ceiling(RowPitch)));
    }
    protected override void OnResize(EventArgs e){base.OnResize(e);if(EditingValue)PlaceEditor();}
    protected override void OnLostFocus(EventArgs e){base.OnLostFocus(e);Invalidate();}
    void EndInline(bool apply){
        if(!EditingValue)return;
        if(apply&&ApplyInline!=null&&!ApplyInline(parameter,valueEditor.Text))return;
        parameter=0;
        valueEditor.Hide();
    }
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);if(Item==null)return;float cell=CellWidth,em=Em;using(var f=NativeUiFont.Create(em)) using(var textBrush=new SolidBrush(ForeColor)){
        e.Graphics.TextRenderingHint=System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        NativeInterfaceText.Draw(e.Graphics,"Par#",f,textBrush,0,0);NativeInterfaceText.Draw(e.Graphics,(Item.Code==16?"WORD":Item.Name(Item.Code).Split(' ')[0].ToUpperInvariant()),f,textBrush,0,em*1.15f);
        for(int n=0;n<10;n++){string label=n==9?"10":(n+1)+"/"+(n+11);NativeInterfaceText.Draw(e.Graphics,label,f,textBrush,cell*(n+1),0);DrawValue(e.Graphics,f,textBrush,n+1,cell*(n+1),RowPitch);if(n<9&&(n+11<=Item.Count||n+11==ArrowParameter||Item.Code==16&&TextNumericTail)&&(Item.Code!=16||n+11<=12||TextNumericTail))DrawValue(e.Graphics,f,textBrush,n+11,cell*(n+1),2*RowPitch);}
    }}
    protected override void OnMouseDown(MouseEventArgs e){
        base.OnMouseDown(e);
        if(e.Button!=MouseButtons.Left||Item==null||ApplyInline==null||CellWidth<=0||CanEditInline!=null&&!CanEditInline())return;
        int column=(int)(e.X/CellWidth),row=(int)(e.Y/RowPitch);
        if(column<1||column>10||row<1||row>2||row==2&&column==10)return;
        int n=column+(row==2?10:0);
        if(EditableParameter(n))OpenParameterEditor(n);
    }
    void OpenParameterEditor(int n){
        EndInline(false);
        highlighted=parameter=n;
        Invalidate();
        valueEditor.Font=Font;
        valueEditor.Text=Item.P(n).ToString("G9",CultureInfo.InvariantCulture);
        PlaceEditor();
        valueEditor.Show();
        valueEditor.Focus();
        valueEditor.SelectAll();
    }
}
public sealed partial class Editor {
    enum ScreenMode{Main,EditMenu,EditGroupMenu,Edit,Disk,Input,Print}
    bool selectionPending;int lastEditedIndex=-1;
    ScreenMode mode=ScreenMode.Main;Label fileLine=new NativeTextLabel(),contextLine=new NativeTextLabel(),responseLine=new NativeTextLabel();TextBox command=new NativeCommandBox();Panel top=new Panel(),prompt=new Panel();TableLayoutPanel functionStrip=new TableLayoutPanel();ParameterHeader parameters=new ParameterHeader();
    Button[] functionKeys=new Button[10];bool fitting=true,layoutBusy;int direction=1;Item editOriginal;int editIndex=-1;bool editDirty;float lastHorizontalMove=1,lastVerticalMove=1;string initialFile;Font classicFont,classicMenuFont;
    static readonly Color ScreenColour=Color.FromArgb(176,176,176);
    void InitializeClassic(string initial){
        initialFile=initial;Text=BuildVersion.Product+" — Native Windows";ClientSize=new Size(1440,900);MinimumSize=new Size(900,600);StartPosition=FormStartPosition.CenterScreen;AutoScaleMode=AutoScaleMode.None;BackColor=ScreenColour;Font=NativeUiFont.Create(12,GraphicsUnit.Point);KeyPreview=true;
        grid.Columns.Add("p","Parameter");grid.Columns.Add("v","Value");staff.Minimum=1;staff.Maximum=32;staff.Value=1;
        top.Dock=DockStyle.Top;top.BackColor=ScreenColour;fileLine.Dock=DockStyle.Top;fileLine.AutoEllipsis=true;contextLine.Dock=DockStyle.Top;contextLine.AutoEllipsis=true;parameters.Dock=DockStyle.Top;parameters.BackColor=ScreenColour;responseLine.Dock=DockStyle.Top;responseLine.AutoEllipsis=true;
        prompt.Dock=DockStyle.Top;var arrow=new NativeTextLabel{Text=">",Dock=DockStyle.Left,Width=22};command.Dock=DockStyle.Fill;command.BorderStyle=BorderStyle.None;command.BackColor=ScreenColour;var preferencesButton=new NativeTextButton{Text="Preferences",Dock=DockStyle.Right,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,TabStop=false};preferencesButton.Click+=(s,e)=>Safe(ShowPreferences);prompt.Controls.Add(command);prompt.Controls.Add(preferencesButton);prompt.Controls.Add(arrow);
        InitializeTextWindow();
        viewport.Dock=DockStyle.Fill;viewport.AutoScroll=true;viewport.BorderStyle=BorderStyle.FixedSingle;viewport.BackColor=ScreenColour;canvas.FlatDisplay=true;canvas.ScreenPaperSize=ActivePaperSize;canvas.BackColor=ScreenColour;viewport.Controls.Add(canvas);
        functionStrip.Dock=DockStyle.Bottom;functionStrip.ColumnCount=10;functionStrip.RowCount=1;functionStrip.BackColor=ScreenColour;
        for(int n=0;n<10;n++){
            int key=n+1;
            functionStrip.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,12.5f));
            var b=new NativeTextButton{ScoreFunction=true,Dock=DockStyle.Fill,FlatStyle=FlatStyle.Flat,Margin=Padding.Empty,Padding=Padding.Empty,BackColor=ScreenColour,TabStop=false};
            b.FlatAppearance.BorderSize=0;
            b.Click+=(a,e)=>Safe(()=>Function(key));
            b.MouseUp+=(a,e)=>Safe(()=>{
                if((e.Button==MouseButtons.Right||e.Button==MouseButtons.Middle)&&MusicInputMenuButton(key,e.Button))return;
                if(e.Button==MouseButtons.Right||e.Button==MouseButtons.Middle)Function(key);
            });
            functionKeys[n]=b;
            functionStrip.Controls.Add(b,n,0);
        }
        Controls.Add(CreateRulerWorkspace());Controls.Add(top);Controls.Add(functionStrip);InitializeScor4Interface();canvas.Picked=i=>{if(i!=null)SelectByMouse(i);};canvas.KeyDown+=(a,e)=>{if(e.KeyCode==Keys.Enter&&e.Modifiers==Keys.None&&mode==ScreenMode.Edit){e.SuppressKeyPress=true;Safe(()=>Execute(""));}};canvas.MouseGesture=ScoreMouse;canvas.MouseGestureCancelled=()=>{mouseAxis=0;scor4ScreenDragStart=null;};canvas.BeforeDrag=Checkpoint;canvas.Changed=()=>{dirty=true;RefreshAll(canvas.Selected);};
        string displayedIssue="";canvas.DisplayIssue=message=>{if(letterHelpPages!=null)return;if(message.Length>0||responseLine.Text==displayedIssue)Message(message);displayedIssue=message;QueueFontVersionChoices();};
        parameters.Edit=n=>{if(letterHelpPages!=null||drawExportStage!=DrawExportStage.None||pendingParameterExport||pendingDriveChange||pendingDiskDelete!=null||pendingDiskFilename!=null||pendingLetterSave!=null||pendingLetterLoad!=null){command.Focus();return;}if(canvas.Selected==null)return;command.Text=n+" "+canvas.Selected.P(n).ToString("G9",CultureInfo.InvariantCulture);command.Focus();command.SelectAll();};
        parameters.CanEditInline=()=>externalDrawingNameItem==null&&originalTextTabContinuation==null&&originalMarginPlan==null&&letterHelpPages==null&&mode==ScreenMode.Edit&&canvas.Selected==parameters.Item&&!inlineTextActive;
        parameters.ApplyInline=(n,value)=>{
            if(letterHelpPages!=null||mode!=ScreenMode.Edit||canvas.Selected!=parameters.Item||inlineTextActive)return false;
            float number;
            if(!Single.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out number)||
               Single.IsNaN(number)||Single.IsInfinity(number)||Math.Abs(number)>100000){
                Message("Enter a finite SCORE parameter.");
                return false;
            }
            try{
                if(canvas.Selected.P(n)!=number)EditOriginalParameters(new[]{(float)n,number});
                return true;
            }catch(Exception error){
                Message(error.Message);
                return false;
            }
        };
        ((NativeCommandBox)command).ScoreLineComplete=()=>Safe(AutoCompleteMusicInputLine);
        command.KeyPress+=(a,e)=>{if(GroupFieldCharacter(e))return;if(pendingGraphicalMenu!=0&&deletingGraphicalMenu){e.Handled=true;Safe(()=>GraphicalMenuCharacter(e));return;}if(originalTextLimitGuard){e.Handled=true;return;}if(pendingLetterSave!=null){e.Handled=true;Safe(()=>SubmitLetterSave(e.KeyChar.ToString()));return;}if(pendingLetterLoad!=null){e.Handled=true;Safe(()=>SubmitLetterLoad(e.KeyChar.ToString()));return;}if(pendingDiskDelete!=null){e.Handled=true;Safe(()=>SubmitDiskDelete(e.KeyChar.ToString()));return;}if(pendingDriveChange){e.Handled=true;Safe(()=>SubmitDriveChange(e.KeyChar.ToString()));return;}if(letterHelpPages!=null){e.Handled=true;Safe(()=>HandleLetterHelpCharacter(e.KeyChar));return;}if(musicInput!=null){e.Handled=true;Safe(()=>e.Handled=MusicInputCharacter(e.KeyChar));}};
        command.KeyDown+=(a,e)=>{if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;string text=((NativeCommandBox)command).PhysicalText;RecordSharedLine(text);command.Clear();Safe(()=>Execute(text));}};
        inlineText.PreviewKeyDown+=(s,e)=>{if(e.KeyCode==Keys.Escape)e.IsInputKey=true;};
        KeyDown+=(a,e)=>{
            if(RunContextShortcut(e.KeyData)){e.SuppressKeyPress=true;return;}
            //Windows host close must precede every SCORE modal/function-key handler.
            if(e.KeyCode==Keys.F4&&e.Modifiers==Keys.Alt){e.SuppressKeyPress=true;Close();return;}
            if(OriginalInlineTextKey(e))return;
            if(PrintingKey(e.KeyData)){e.SuppressKeyPress=true;return;}
            if(FilteredEditKey(e))return;if(ExternalDrawingNameKey(e))return;if(CommandColorsKey(e))return;if(GraphicalMenuKey(e))return;if(TabCommandKey(e))return;if(PageHeightKey(e))return;if(HandleOriginalCommandFileKey(e))return;if(OriginalMarginKey(e))return;if(OriginalTextInputKey(e))return;if(HandleLetterHelpKey(e))return;if(OriginalPrintKey(e))return;
            if(pendingBeamTemporary!=null&&e.KeyCode!=Keys.ShiftKey&&e.KeyCode!=Keys.ControlKey&&e.KeyCode!=Keys.Menu){e.SuppressKeyPress=true;AcknowledgeEditPause();return;}
            if(parameters.EditingValue||setupEditField!=0)return;
            if(mode==ScreenMode.Edit&&editParameter!=0&&HandleOriginalEditMovementKey(e))return;
            if(parameters.HandleHighlightedKey(e))return;
            if(originalItemZero&&new[]{Keys.Left,Keys.Right,Keys.Up,Keys.Down}.Contains(e.KeyCode)){e.SuppressKeyPress=true;Message("Arrow keys in this compatibility state are not verified; use I and an item number to recover.");return;}
            if(pendingEditAcknowledgement&&e.KeyCode!=Keys.ShiftKey&&e.KeyCode!=Keys.ControlKey&&e.KeyCode!=Keys.Menu){e.SuppressKeyPress=true;AcknowledgeEditPause();return;}
            if(DiskFilenameKey(e)||LetterSaveKey(e)||LetterLoadKey(e)||DiskDeleteKey(e)||DriveChangeKey(e)||DrawExportKey(e)||ParameterExportKey(e)||ResourceEntryKey(e)||HandleScoreAppendKey(e)||HandleMainGroupKey(e))return;
            if(e.KeyData==(Keys.Alt|Keys.P)){e.SuppressKeyPress=true;Safe(ShowPreferences);return;}
            if(e.KeyData==(Keys.Alt|Keys.Shift|Keys.M)){e.SuppressKeyPress=true;Safe(ShowWorkspaceMenu);return;}
            if(HandleInputDisplayKey(e.KeyData)){e.SuppressKeyPress=true;return;}
            if(e.Alt&&e.KeyCode==Keys.D&&!e.Shift){e.SuppressKeyPress=true;MoveTextBoundary(e.Control?-1:1);return;}
            if(musicInput!=null){bool consumed=false;Safe(()=>consumed=HandleScor4PresetConfirmation(e)||MusicInputKey(e));if(consumed||e.SuppressKeyPress)return;}
            if(e.KeyCode==Keys.Delete&&e.Modifiers==Keys.None&&DeleteKeyMatchesF7&&mode==ScreenMode.Edit&&!originalItemZero&&pendingAppendPath==null&&pendingAppendScore==null&&groupCommand==null){e.SuppressKeyPress=true;Safe(()=>Function(7));return;}
            if(mode==ScreenMode.Edit&&!originalItemZero&&HandleOriginalEditMovementKey(e))return;
            if(e.Control&&e.Shift&&e.KeyCode==Keys.F){e.SuppressKeyPress=true;NativeInterfaceText.ChooseFont(this);return;}
            if(HandleScor4MenuKey(e))return;
            if(e.KeyCode>=Keys.F1&&e.KeyCode<=Keys.F8){e.SuppressKeyPress=true;Safe(()=>Function(e.KeyCode-Keys.F1+1));}
            else if(e.KeyCode==Keys.Escape){e.SuppressKeyPress=true;if(originalItemZero)return;if(RestoreOriginalItemPrompt())return;if(mode==ScreenMode.EditGroupMenu){LeaveOriginalGroupWindow();Message("");UpdateClassic();command.Focus();return;}if(mode==ScreenMode.Main&&!selectionPending&&editFilter==null){command.Clear();command.Focus();return;}if(mode==ScreenMode.Edit)RecordSharedLine("");ExitEdit(false);mode=ScreenMode.Main;command.Clear();UpdateClassic();command.Focus();}
            else if(e.Control&&e.KeyCode==Keys.Z){e.SuppressKeyPress=true;Undo(false);}
            else if(e.Control&&e.KeyCode==Keys.Y){e.SuppressKeyPress=true;Undo(true);}
            else if(e.Control&&e.KeyCode==Keys.O){e.SuppressKeyPress=true;Open();}
            else if(e.Control&&e.Shift&&e.KeyCode==Keys.S){e.SuppressKeyPress=true;Safe(()=>SaveCopy());}
            else if(e.Control&&e.KeyCode==Keys.S){e.SuppressKeyPress=true;Safe(()=>SaveCurrent());}
            else if(e.Control&&(e.KeyCode==Keys.Add||e.KeyCode==Keys.Oemplus)){e.SuppressKeyPress=true;fitting=false;Zoom(.25f);}
            else if(e.Control&&(e.KeyCode==Keys.Subtract||e.KeyCode==Keys.OemMinus)){e.SuppressKeyPress=true;fitting=false;Zoom(-.25f);}
            else if((e.KeyCode==Keys.Home||e.KeyCode==Keys.End)&&!command.Focused){e.SuppressKeyPress=true;fitting=true;Fit();}
            else if(mode==ScreenMode.Edit&&!command.Focused&&new[]{Keys.Left,Keys.Right,Keys.Up,Keys.Down}.Contains(e.KeyCode)){e.SuppressKeyPress=true;Safe(()=>MoveSelected(e.KeyCode==Keys.Left?-editArrowIncrement:e.KeyCode==Keys.Right?editArrowIncrement:0,e.KeyCode==Keys.Up?editArrowIncrement:e.KeyCode==Keys.Down?-editArrowIncrement:0));}
        };
        KeyPress+=(a,e)=>{if(PrintingCharacter(e))return;if(OriginalPrintCharacter(e))return;if(setupEditField==0&&!parameters.EditingValue&&!inlineTextActive&&!originalGroupValueBox.Focused&&!originalTextValueBox.Focused&&!command.Focused&&!Char.IsControl(e.KeyChar)&&(ModifierKeys&Keys.Control)==0){command.Focus();command.AppendText(e.KeyChar.ToString());e.Handled=true;}};
        RestoreWindowSettings();FormClosing+=(a,e)=>{if(!confirmedScor4Exit&&!MayDiscard())e.Cancel=true;else SaveWindowSettings();};Resize+=(a,e)=>LayoutClassic();Shown+=(a,e)=>{canvas.DisplayDpi=NativeDisplay.Scale(this);if(File.Exists(initialFile))LoadFile(initialFile);else RestartScore();LayoutClassic();command.Focus();};
        InitializeOriginalTextSetup();InitializeOriginalGroupFields();responseLine.MouseDown+=ResourceEntryClick;LayoutClassic();UpdateClassic();NativeInterfaceText.Install(this);AddLibraryMenu(ContextMenuStrip);AddStartupFolderMenu(ContextMenuStrip);AddCommandOutputPreferences(ContextMenuStrip);AddOutputFolderMenu(ContextMenuStrip);AddFallbackLibraryPathMenu(ContextMenuStrip);AddTextCacheMenu(ContextMenuStrip);NativeMouseSelector.AddMenu(ContextMenuStrip);NativeMenuMouseOptions.AddMenu(ContextMenuStrip);AddPreferenceCompatibilityMenu(ContextMenuStrip);AddBackgroundMenu(ContextMenuStrip);AddCompactTextMenu(ContextMenuStrip);AddTextAreaFontSizeMenu(ContextMenuStrip);AddInputHighlightMenu(ContextMenuStrip);AddDeleteShortcutMenu(ContextMenuStrip);NativeDrawingNames.AddMenu(ContextMenuStrip,()=>{RefreshAll(canvas.Selected);canvas.Invalidate();});NativeFontInitialization.AddMenu(ContextMenuStrip,()=>source,()=>{RefreshAll(canvas.Selected);canvas.Invalidate();});NativeStartupSplash.AddMenu(ContextMenuStrip);AddMidiMenu(ContextMenuStrip);AddPageFrameMenu(ContextMenuStrip);NativeWorkspaceGuides.AddMenu(ContextMenuStrip);NativeDiskDialogChoice.AddMenu(ContextMenuStrip);NativePrintMenuChoice.AddMenu(ContextMenuStrip);NativePrintBehaviour.AddMenu(ContextMenuStrip);NativeZoomFraming.AddMenu(ContextMenuStrip);NativeKeyboardOptions.AddMenu(ContextMenuStrip);NativeTabEnvironment.AddMenu(ContextMenuStrip);NativeEdiBehaviour.AddMenu(ContextMenuStrip);NativeWindowsColors.AddMenu(ContextMenuStrip);ContextMenuStrip.Items.Add("File associations...",null,(s,e)=>NativeFileAssociations.Show(this));NativeScoreBehaviour.AddMenu(ContextMenuStrip,()=>{RefreshAll(canvas.Selected);UpdateClassic();canvas.Invalidate();});OriginalScoreMenu.Consolidate(ContextMenuStrip);
        windowsPreferencesMenu=(PreferencesActionsMenu)ContextMenuStrip;
        Shown+=(s,e)=>{QueuePreferencesPreload();if(!Program.CheckMode)QueueFontMetricsPreload();};
        Disposed+=(s,e)=>{DisposeOverview();DisposePreparedPreferences();windowsPreferencesMenu.Dispose();};
        InstallWorkspaceMenu();
    }
    void LayoutClassic(){if(layoutBusy)return;layoutBusy=true;try{float scale=Math.Max(.65f,ClientSize.Width/1920f);float systemScale=NativeUiFont.WindowsTextScale;float fontSize=(textAreaFontPoints>0?textAreaFontPoints*96f/72f*NativeDisplay.Scale(this):Math.Max(10,24*scale))*systemScale;int row=NativeUiFont.RowHeight(fontSize,compactText,NativeInterfaceText.UseVga);Padding=new Padding((int)(18*scale),(int)(8*scale),(int)(18*scale),(int)(5*scale));
        fileLine.Height=row;contextLine.Height=row;prompt.Height=row;parameters.Height=row*3;itemTextLine.Height=row;inlineText.Height=row;itemTextLine.Visible=parameters.Visible&&parameters.Item!=null&&parameters.Item.Code==16;bool showEditHeader=mode==ScreenMode.Edit&&(!quietItemTypeout||quietHeaderWasEdit);parameters.Visible=showEditHeader;itemTextLine.Visible=!inlineTextActive&&showEditHeader&&parameters.Item!=null&&parameters.Item.Code==16;fileLine.Visible=true;
        float menuSize=Math.Max(9,23*scale)*systemScale;
        if(!NativeUiFont.Matches(classicFont)||Math.Abs(classicFont.Size-fontSize)>.01f){Font oldFont=classicFont;classicFont=NativeUiFont.Create(fontSize);parameters.Font=classicFont;contextLine.Font=classicFont;itemTextLine.Font=classicFont;inlineText.Font=classicFont;responseLine.Font=classicFont;command.Font=classicFont;foreach(Control c in prompt.Controls)if(c is Label||c is Button)c.Font=classicFont;if(oldFont!=null)oldFont.Dispose();}
        if(!NativeUiFont.Matches(classicMenuFont)||Math.Abs(classicMenuFont.Size-menuSize)>.01f){Font oldMenu=classicMenuFont;classicMenuFont=NativeUiFont.Create(menuSize);foreach(var b in functionKeys)b.Font=classicMenuFont;foreach(var b in scor4Buttons)b.Font=classicMenuFont;if(oldMenu!=null)oldMenu.Dispose();}
        int menuRowHeight=compactBottomMenu?classicMenuFont.Height+6:(int)Math.Ceiling(49*scale*systemScale);
        functionStrip.Height=menuRowHeight;scor4Strip.Height=2*menuRowHeight;
        int promptHeight=prompt.Controls.OfType<Button>().Select(b=>b.GetPreferredSize(Size.Empty).Height).DefaultIfEmpty(row).Max();prompt.Height=Math.Max(row,promptHeight);LayoutTextWindow(row);
        if(fitting&&!movingTextBoundary&&!preserveMouseView)Fit();else canvas.LayoutScore();parameters.SelectionColor=inputHighlightColor;parameters.Invalidate();
    }finally{layoutBusy=false;}}
    [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hwnd,IntPtr after,int x,int y,int cx,int cy,uint flags);
    private Size renderExtent;
    protected override void WndProc(ref System.Windows.Forms.Message m){base.WndProc(ref m);if(m.Msg==0x24&&!renderExtent.IsEmpty){System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam,8,renderExtent.Width);System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam,12,renderExtent.Height);System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam,32,renderExtent.Width);System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam,36,renderExtent.Height);}if((m.Msg==0x02e0||m.Msg==0x1a)&&canvas!=null&&IsHandleCreated){canvas.DisplayDpi=NativeDisplay.Scale(this);LayoutClassic();}}
    void Fit(){viewport.AutoScrollMinSize=Size.Empty;viewport.AutoScrollPosition=Point.Empty;canvas.HasZoomCommand=false;canvas.OriginalZoomFrame=false;canvas.CameraOffset=PointF.Empty;if(viewport.ClientSize.Width<10)return;canvas.LayoutScore();canvas.Zoom=Math.Max(.1f,Math.Min(16,Math.Min((viewport.ClientSize.Width-24)/(canvas.ViewWidth*canvas.DisplayDpi),(viewport.ClientSize.Height+canvas.CropTop-24)/(canvas.ViewHeight*canvas.DisplayDpi))));canvas.LayoutView();UpdateClassic();}
    void UpdateClassic(){UpdateGroupWindowState();ApplyPreferenceColors();if(fileLine==null)return;parameters.Item=quietItemTypeout?quietHeaderItem:canvas.Selected;itemTextLine.Text=parameters.Item!=null&&parameters.Item.Code==16?parameters.Item.Text.TrimEnd('\0',' '):"";inlineText.Visible=inlineTextActive;command.Enabled=!inlineTextActive;itemTextLine.Visible=!inlineTextActive&&parameters.Visible&&parameters.Item!=null&&parameters.Item.Code==16;parameters.ArrowParameter=mode==ScreenMode.Edit?editParameter:0;parameters.TextNumericTail=canvas.EditModified&&Object.ReferenceEquals(parameters.Item,canvas.Selected);parameters.DisplayValue=n=>parameters.Item!=null&&parameters.Item.Code==16&&n>=14?OriginalArrowValueFor(parameters.Item,n):parameters.Item.P(n);parameters.Invalidate();LayoutTextContents(false);
        if(musicInput!=null){UpdateMusicInput();return;}((NativeCommandBox)command).ScoreEditing=!originalTextSetupActive&&(mode!=ScreenMode.EditGroupMenu||groupCommand!=null);if(UpdateOriginalTextContext())return;
        ((NativeTextLabel)contextLine).FitInputRow=false;
        contextLine.Cursor=Cursors.Default;
        contextLine.Text=mode==ScreenMode.Edit&&canvas.Selected!=null?"*** Edit Item # "+(score.Items.IndexOf(canvas.Selected)+1)+" / Press <ESC> to Exit / Direction="+(direction>0?"Forward":"Backward")+" / Move="+(editParameter!=0?"Parameter "+editParameter:editEndpoint==1?"Left End":editEndpoint==2?"Right End":editEndpoint==3?"Middle":"Whole Item"):mode==ScreenMode.EditGroupMenu?"EDIT BY GROUP / ESC=Edit Menu":mode==ScreenMode.EditMenu?"EDIT  Last Item Edited="+(Math.Max(0,Math.Min(lastEditedIndex,score.Items.Count-1))+1)+"  Items="+score.Items.Count+" / ESC=Main Menu":mode==ScreenMode.Disk?OriginalDiskContext():mode==ScreenMode.Input?"Input parameters: code staff horizontal vertical ...  /  ESC=Exit":mode==ScreenMode.Print?"Print / Export \u2014 same vector drawing as the score window":"*** Measurements = "+NativeMeasurements.Label(score);
        if(quietItemTypeout&&mode==ScreenMode.Edit)contextLine.Text=quietHeaderContext;
        string[] labels=mode==ScreenMode.Edit?new[]{"HELP","DIRECTION","COPY","CENTER","SET","ALIGN","DEL","ABORT"}:mode==ScreenMode.EditGroupMenu?new[]{"MOVE","COPY","ALTER","DELETE","JUSTIFY","LINEUP","FLIP","TRANS"}:mode==ScreenMode.EditMenu?new[]{"HELP","POSITION","STAFF","ITEM","GROUP","MENU","",""}:mode==ScreenMode.Disk?new[]{"LOAD","SAVE","APPEND","READ","DIR","DRIVE","SET PATH","DELETE"}:mode==ScreenMode.Input?new[]{"HELP","STAFF","NOTE","REST","CLEF","SYMBOL","TEXT","EXIT"}:mode==ScreenMode.Print?(UsesOriginalPrintMenu?new[]{"HELP","","","","","GO PRINT","",""}:new[]{"HELP","PRINT","PNG 4K","PNG 8K","EPS","READ PRINT.SCR","SAVE PRINT.SCR","EXIT"}):new[]{"HELP","INPUT","EDIT","TEXT","PLAY","PRINT","DISK","EXIT"};
        InputMenu(labels,labels.Length);UpdateOriginalGroupContext();RefreshOriginalPrintMenu();
    }
    void Message(string text){((NativeTextLabel)responseLine).SelectedMessageRow=-1;responseLine.Text=text;}
    ScreenMode editReturnMode=ScreenMode.Main;
    void BeginEdit(Item item){if(mode!=ScreenMode.Edit)editReturnMode=mode==ScreenMode.EditMenu?ScreenMode.EditMenu:ScreenMode.Main;quietPromptCancelled=false;originalItemZero=false;canvas.SelectionContextSource=null;canvas.BlockItemEditing=false;selectionPending=false;if(editIndex!=score.Items.IndexOf(item)||editOriginal==null){editOriginal=item.Clone();ResetOriginalTextArrowState(item);editIndex=score.Items.IndexOf(item);editDirty=dirty;canvas.EditModified=false;}canvas.EditOriginal=editOriginal;mode=ScreenMode.Edit;SelectItem(item);LayoutClassic();if(!preserveMouseView)RevealSelection();NativeDosViewport.Advance(canvas);ObserveDisplaySelection(item);canvas.Focus();}
    void RevealSelection(){if(canvas.Selected==null)return;if(NativeNotationView.IsDosViewport||canvas.UsesOriginalZoomFrame){NativeDosViewport.RevealActive(canvas);canvas.LayoutScore();canvas.Invalidate();return;}PointF p=canvas.Position(canvas.Selected);p=new PointF(p.X+canvas.CameraOffset.X,p.Y+canvas.CameraOffset.Y);float x=p.X*canvas.RenderScale,y=p.Y*canvas.RenderScale-canvas.CropTop;Point scroll=viewport.AutoScrollPosition;if(x+scroll.X<20||x+scroll.X>viewport.ClientSize.Width-30||y+scroll.Y<20||y+scroll.Y>viewport.ClientSize.Height-30)viewport.AutoScrollPosition=new Point(Math.Max(0,(int)x-viewport.ClientSize.Width/2),Math.Max(0,(int)y-viewport.ClientSize.Height/2));}
    void ExitEdit(bool abort){if(originalItemZero)return;editEndpoint=0;ItemTypeout(false);editFilter=null;NativeEditPositionGuide.Set(canvas,null);selectionPending=false;if(canvas.Selected!=null&&score.Items.Contains(canvas.Selected))lastEditedIndex=score.Items.IndexOf(canvas.Selected);if(!abort&&canvas.EditModified&&canvas.Selected!=null){
      if(canvas.Selected.HasVerifiedShortDrawingName)canvas.Selected.RestoreShortDrawingPadding();
      else canvas.Selected.TrimEditedNumericParameters();
      if(scor4Interface&&lastEditedIndex>0&&lastEditedIndex<score.Items.Count)
       NativeTextTerminator.Refresh(score,score.Items[lastEditedIndex-1]);
      if(scor4Interface&&canvas.Selected.Code==8&&editOriginal!=null&&canvas.Selected.P(5)!=editOriginal.P(5))
       RefreshTextCaches();
    }ObserveDisplayExit(abort);if(abort&&editOriginal!=null&&editIndex>=0&&editIndex<score.Items.Count){Checkpoint();score.Items[editIndex]=editOriginal;dirty=editDirty;}editOriginal=null;editIndex=-1;canvas.EditOriginal=null;canvas.EditModified=false;NativeDosViewport.Remember(canvas.Selected);canvas.Selected=null;mode=editReturnMode;if(score!=null)RefreshAll(null);LayoutClassic();NativeDosViewport.Advance(canvas);}
    void AbortCurrent(){if(mode!=ScreenMode.Edit||editOriginal==null||editIndex<0||editIndex>=score.Items.Count)return;bool displayAbort=BeginTrackedDisplayAbort();Checkpoint();var restored=editOriginal.Clone();score.Items[editIndex]=restored;dirty=editDirty;canvas.EditModified=false;RefreshAll(restored);NativeDosViewport.Advance(canvas);UpdateClassic();CompleteTrackedDisplayAbort(displayAbort);}
    void MoveSelected(float x,float y){if(canvas.Selected==null)return;((NativeCommandBox)command).ErasePhysicalLine();var changed=canvas.Selected.Clone();if(editEndpoint!=0)NativeItemMotion.MoveEndpoint(changed,editEndpoint,x,y);else{NativeItemMotion.Horizontal(changed,x);NativeItemMotion.Vertical(changed,y);}if(changed.Bytes.SequenceEqual(canvas.Selected.Bytes))return;using(var display=BeginDisplayNoteParameters(null,editEndpoint==0&&TrackedRest(canvas.Selected))){Checkpoint();canvas.Selected.Bytes=changed.Bytes;dirty=true;RefreshAll(canvas.Selected);display.Complete();}}
    void Function(int key){if(tabCommandStage==TabCommandStage.None)InvalidateTabFrequencyHistory();if(externalDrawingNameItem!=null){command.Focus();return;}if(tabCommandStage!=TabCommandStage.None||pageHeightStage!=PageHeightStage.None||commandFileStage!=CommandFileStage.None||commandFileInputQuestion){command.Focus();return;}if(AcknowledgeOriginalTextTab())return;if(originalMarginPlan!=null)return;originalTextLimitGuard=false;if(OriginalTextFunction(key))return;if(letterHelpPages!=null||drawExportStage!=DrawExportStage.None||pendingParameterExport||pendingDriveChange||pendingDiskDelete!=null||pendingDiskFilename!=null||pendingLetterSave!=null||pendingLetterLoad!=null){command.Focus();return;}if(pendingAppendPath!=null||pendingAppendScore!=null||groupCommand!=null){command.Focus();return;}if(inlineTextActive){inlineText.Focus();return;}quietPromptCancelled=false;if(AcknowledgeEditPause())return;if(originalItemZero){if(key==8)return;throw new NotSupportedException("This function in the original item-0 state is not verified; use I and an item number to recover.");}
        if(mode==ScreenMode.Edit){if(key>=3&&key<=8){RecordSharedLine("");command.Clear();}switch(key){case 1:Reference();break;case 2:direction=-direction;break;case 3:command.Clear();CopyEditedItem(true);break;case 4:CenterOriginalNote("");break;case 5:ExecuteItemAlignment("S","");break;case 6:ExecuteItemAlignment("A","");break;case 7:DeleteEditedItem();break;case 8:command.Clear();AbortCurrent();break;}UpdateClassic();return;}
        if(mode==ScreenMode.EditGroupMenu){ChooseEditGroupOperation(key);UpdateClassic();if(mode==ScreenMode.EditGroupMenu&&groupCommand==null)OpenOriginalGroupValue();return;}
        if(mode==ScreenMode.EditMenu){switch(key){case 1:Reference();break;case 2:PromptOriginalFilteredEdit("ED");break;case 3:PromptOriginalFilteredEdit("ST");break;case 4:if(score.Items.Count==0){Message("There are no items to edit.");break;}selectionPending=true;Message("Enter item number (Enter uses "+(Math.Max(0,Math.Min(lastEditedIndex,score.Items.Count-1))+1)+").");command.Clear();command.Focus();break;case 5:selectionPending=false;EnterOriginalGroupWindow();UpdateClassic();OpenOriginalGroupValue();return;case 6:ToggleEditGraphicMenu(1);break;}UpdateClassic();return;}
        if(DiskFileFunction(key))return;if(mode==ScreenMode.Disk){if(key==1)Open();else if(key==2)SaveCopy();else if(key==5){using(var d=new OpenFileDialog{InitialDirectory=NativeOpenFolder.InitialDirectory(source),Filter="SCORE music|*.MUS"})if(d.ShowDialog(this)==DialogResult.OK)LoadFile(d.FileName);}else if(key==7)Message("The disk CD prompt is being verified; use Main CD with an explicit path.");else Message("This disk operation is not implemented yet.");UpdateClassic();return;}
        if(mode==ScreenMode.Print){if(!UsesOriginalPrintMenu){NativeExportPrintFunction(key);return;}if(printStage==OriginalPrintStage.None){if(key==1){ShowHelpTopic("PRINT");return;}if(key==6)RunOriginalPrint();}RefreshOriginalPrintMenu();return;}
        if(mode==ScreenMode.Input){MusicInputFunction(key,ModifierKeys);UpdateMusicInput();return;}
        switch(key){case 1:Reference();break;case 2:StartMusicInput();break;case 3:mode=ScreenMode.EditMenu;lastEditedIndex=score.Items.Count-1;if(score.Items.Count>0)NativeDosViewport.Remember(score.Items[lastEditedIndex]);selectionPending=false;Message("Select an item with the mouse, or use F4 / I followed by its item number.");canvas.Focus();break;case 4:StartOriginalTextSetup();break;case 5:Message("PLAY is not implemented yet.");break;case 6:EnterOriginalPrintMenu();break;case 7:mode=ScreenMode.Disk;lastEditedIndex=score.Items.Count-1;if(score.Items.Count>0)NativeDosViewport.Remember(score.Items[lastEditedIndex]);break;case 8:Close();break;}UpdateClassic();LayoutClassic();
    }
    void Execute(string text){TrackTabFrequencyCommand(text);if(AcknowledgeOriginalLyricEmptyWarning())return;if(SubmitOriginalGroupFlip(text))return;if(SubmitOriginalGroupField(text))return;if(SubmitFilteredEdit(text))return;if(SubmitExternalDrawingName(text))return;if(SubmitOriginalCommandColors(text))return;if(SubmitOriginalGraphicalMenu(text))return;if(SubmitOriginalTablature(text))return;if(SubmitOriginalPageHeight(text))return;if(SubmitOriginalCommandFile(text))return;if(AcknowledgeOriginalTextTab())return;if(OriginalMarginReply(text))return;if(SubmitOriginalTextSetup(text)||SubmitOriginalTextLine(text)||QuickOriginalText(text))return;if(SubmitDiskFilename(text))return;if(SubmitLetterSave(text))return;if(SubmitLetterLoad(text))return;if(SubmitDiskDelete(text))return;if(SubmitDriveChange(text))return;if(SubmitDrawExport(text))return;if(SubmitParameterExport(text))return;if(SubmitOriginalPrint(text))return;if(SubmitLetterHelp(text))return;if(SubmitResourceEntry(text))return;if(SubmitScoreAppend(text))return;if(SubmitMainGroup(text)){if(mode==ScreenMode.EditGroupMenu&&groupCommand==null){UpdateClassic();OpenOriginalGroupValue();}return;}if(musicInput!=null){SubmitMusicInput(text);return;}if(AcknowledgeEditPause())return;text=text.Trim();if(mode==ScreenMode.Main||mode==ScreenMode.Edit||mode==ScreenMode.EditMenu)text=SeparateSelectorArguments(text);if(mode==ScreenMode.Main)text=SeparateTransposeStaff(text);if(ExecuteOriginalCommandColors(text))return;if(ExecuteOriginalGraphicalMenu(text))return;if(ExecuteOriginalDisplayCommand(text))return;if(ExecuteOriginalTablature(text))return;if(ExecuteOriginalPageHeight(text))return;if(ExecuteOriginalCommandFile(text))return;if(ExecuteOriginalSavedCommands(text))return;if(ExecuteOriginalEditInput(text)||ExecuteOriginalMotives(text)||ExecuteOriginalSpacing(text)||ExecuteOriginalInputLaunch(text))return;if(OriginalQuietPromptBoundary(text))return;if(originalItemZero&&!(text.StartsWith("I ",StringComparison.OrdinalIgnoreCase)||text.StartsWith("N ",StringComparison.OrdinalIgnoreCase)||text.Equals("X",StringComparison.OrdinalIgnoreCase)))throw new NotSupportedException("This command in the original item-0 state is not verified; use I and an item number to recover.");if(mode==ScreenMode.Edit&&text.Length>1&&(text[0]=='C'||text[0]=='c'||text[0]=='X'||text[0]=='x')&&text[1]>='0'&&text[1]<='9')text=text.Substring(0,1)+" "+text.Substring(1);if(mode==ScreenMode.Edit&&ExecutePairedMovement(text))return;if(mode==ScreenMode.EditMenu&&text.Equals("GE",StringComparison.OrdinalIgnoreCase)){Function(5);return;}if(selectionPending&&text.Length==0&&score.Items.Count>0){BeginEdit(score.Items[Math.Max(0,Math.Min(lastEditedIndex,score.Items.Count-1))]);return;}if(text.Length>2&&text.StartsWith("CW",StringComparison.OrdinalIgnoreCase)&&(Char.IsDigit(text[2])||text[2]=='-'||text[2]=='+'||text[2]=='.'))text="CW "+text.Substring(2);if(text.Length==0){if(mode==ScreenMode.Edit&&canvas.Selected!=null){int n=NextEditingIndex(score.Items.IndexOf(canvas.Selected)+direction,direction);if(n>=0&&n<score.Items.Count){if(canvas.EditModified){canvas.Selected.TrimEditedNumericParameters();ObserveDisplayExit(false);}editOriginal=null;BeginEdit(score.Items[n]);}else ExitEdit(false);}return;}if(selectionPending){float candidate;if(Single.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out candidate)){int number=ReadOriginalItemNumber(text);if(number<0){Message("Enter an item number.");return;}int index=number==0?Math.Max(0,Math.Min(lastEditedIndex,score.Items.Count-1)):Math.Min(number,score.Items.Count)-1;selectionReturnItem=null;direction=1;editOriginal=null;BeginEdit(score.Items[index]);return;}}if(mode==ScreenMode.Edit)text=NormalizeCompactMovement(text);string[] fields=text.Replace(',',' ').Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);string verb=fields[0].ToUpperInvariant();string rest=text.Length>verb.Length?text.Substring(verb.Length).Trim().Trim('"'):"";
        if(DiskFileCommand(verb,rest))return;if(mode==ScreenMode.Main&&(ExecuteScor4ItemCommand(verb,rest)||ExecuteMainLetterCommand(verb,rest)))return;
        if(mode==ScreenMode.Main&&StartOriginalTextInput(verb,rest))return;
        if(mode==ScreenMode.Main&&ExecuteQuickExpression(verb,rest))return;
        if(mode==ScreenMode.Main&&ExecuteMainNotationCommand(verb,rest))return;
        if(mode==ScreenMode.Main&&scor4Interface&&verb=="ZF"){
            RequireNoLetterArguments(verb,rest);
            FitScor4Screen();
            return;
        }
        if(mode==ScreenMode.Edit&&(TryEditTypedEnharmonic(verb,rest)||ExecuteNoteAccidental(verb,rest)||ExecuteItemAlignment(verb,rest)||ExecuteEditMovement(verb,rest)))return;
        if(TryScor4Clipboard(verb,rest))return;
        if(mode==ScreenMode.Edit&&ExecuteOriginalTextFormatting(verb,rest))return;
        if(mode==ScreenMode.Edit&&(ExecuteScor4SelectedBeamCommand(verb,rest)||ExecuteScor4ChordRhythm(verb,rest)))return;
        if(verb=="TP"&&mode==ScreenMode.Edit){OriginalTabPosition(rest);return;}
        if(verb=="FLI"&&mode==ScreenMode.Edit){FlipOriginalEditedItem(rest);return;}
        if(verb=="SZ"&&mode==ScreenMode.Edit){OriginalAbsoluteSize(rest);return;}
        if(verb=="CR"&&mode==ScreenMode.Edit){SetOriginalEditCurvature(rest);return;}
        if(verb=="CV"&&mode==ScreenMode.Edit){CenterOriginalNote(rest,true);return;}
        if(verb=="CN"&&mode==ScreenMode.Edit){CenterOriginalNote(rest);return;}
        if(scor4Interface&&mode==ScreenMode.Edit&&(verb=="CNL"||verb=="CNR")){CenterScor4Slur(verb=="CNL",rest);return;}
        if(verb=="CM"&&mode==ScreenMode.Edit){CenterOriginalBarOrMeter(rest);return;}
        if(verb=="CW"){ExecuteRestCentering(fields);return;}
        if(verb=="MET"||verb.StartsWith("METR")||verb.StartsWith("INCH")){if(mode!=ScreenMode.Main)throw new NotSupportedException("Use METRIC or INCHES in Main Command Mode.");NativeMeasurements.Set(score,(short)(verb=="MET"||verb.StartsWith("METR")?1:0));RefreshAll(null);UpdateClassic();return;}if(verb=="PREF"){ShowPreferences();return;}if(verb=="AB"){AbortCurrent();return;}if(ShowOriginalParameter(verb,rest))return;if(verb=="ST"||verb=="ED"){StartOriginalFilteredEdit(verb,rest);return;}
        if(NativeNotationView.IsDosViewport&&mode==ScreenMode.Edit&&canvas.EditModified&&(verb=="Z"||verb=="ZZ"||verb=="ZL"||verb=="ZR"||verb=="ZU"||verb=="ZD")){Message("Viewport command ignored while item parameters are changed; commit or abort first.");return;}
        float zoomArgument;
        bool rebuildDisplay=verb=="Z"&&(rest.Length==0||float.TryParse(fields.ElementAtOrDefault(1),NumberStyles.Float,CultureInfo.InvariantCulture,out zoomArgument)&&zoomArgument==0);
        if(verb=="Z"||verb=="ZZ"||verb=="ZL"||verb=="ZR"||verb=="ZU"||verb=="ZD")ApplyOriginalDisplayState(rebuildDisplay,rebuildDisplay&&mode==ScreenMode.Main);
        if(verb=="Z"&&mode==ScreenMode.Main){
         if(rebuildDisplay){
          if(!NativeZoomFraming.HelpDefaults)canvas.RebuildGuideMeasurement=true;
          bool preserveDisplay=TrackedDisplayArray()!=null;
          var cache=RefreshTextCaches();
          if(preserveDisplay&&physicalDisplayReferenceRedrawPending)
           RebuildTrackedDisplay();
          else if(preserveDisplay&&cache!=null&&cache.Updated==0&&cache.TerminatorWrites!=0)
           RecordUnchangedTrackedDisplay();
         }
         RefreshAllCore(null,rebuildDisplay);
        }
        if(ExecuteCrossViewZoom(verb,rest))return;
        if(NativeNotationView.IsDosViewport&&(NativeDosViewport.CenteringCommand(canvas,verb,rest)||NativeDosViewport.Command(canvas,verb,rest))){canvas.LayoutScore();if(verb=="ZL"||verb=="ZR"||verb=="ZU"||verb=="ZD")NativeDosViewport.RevealActive(canvas);NativeDosViewport.Advance(canvas);UpdateClassic();return;}if(mode==ScreenMode.Edit&&(verb=="C"||verb=="CC"||verb=="CX")){if(verb=="C"){if(rest.Length==0)CopyEditedItem();else{int number=ReadOriginalItemNumber(rest);if(number==0)CopyEditedItem();else if(number<0){CopyEditedItem();BeginEdit(score.Items[lastEditedIndex]);SelectOriginalItem("I","-1");}else CopyEditedItem(CopyContinuation.Item,Math.Min(number,score.Items.Count+(canvas.EditModified?1:0))-1);}}else{if(rest.Length!=0)throw new NotSupportedException("Arguments after CC/CX still need original verification.");CopyEditedItem(verb=="CC"?CopyContinuation.Copy:CopyContinuation.Next);}return;}if(verb=="DEL"&&mode==ScreenMode.Edit){RequireNoLetterArguments(verb,rest);DeleteEditedItem();return;}if(verb=="DE"&&mode==ScreenMode.Edit){DeleteEditedItem();return;}if(verb=="UND"){UndeleteItem();return;}if(verb=="EX"){if(scor4Interface&&mode==ScreenMode.Main)BeginScor4Exit();else Close();return;}if(verb=="X"){if(rest.Length==0)ExitEdit(false);else{int number=ReadOriginalItemNumber(rest);if(number==0)ExitEdit(false);else if(number<0){ExitEdit(false);BeginEdit(score.Items[lastEditedIndex]);SelectOriginalItem("I","-1");}else ExitToItem(Math.Min(number,score.Items.Count)-1);}return;}if(verb=="G"){StartLetterLoad(rest);return;}if(verb=="SA"){StartLetterSave(rest);return;}if(verb=="SM"){SaveCurrent();return;}if(verb=="RS"){if(rest.Length>0)LoadFile(Resolve(rest,Directory.GetCurrentDirectory()));else RestartScore();return;}if(verb=="T"&&mode==ScreenMode.Edit&&rest.Length==0){ItemTypeout(false);LayoutClassic();UpdateClassic();return;}if(verb=="TX"||verb=="T"){EditText();return;}if(verb=="LIB"){PickSymbol();return;}if(verb=="PR"){if(mode==ScreenMode.Edit){if(NativeEditCompatibility.ReproduceInvalidStates){pendingEditAcknowledgement=true;var item=editOriginal??canvas.Selected;Message("Parameter  0 was "+ExportFixed(BitConverter.ToSingle(item.Bytes,0),4).PadLeft(11)+"  Press any key to continue.");}else Message("Leave item editing before printing.");return;}throw new NotSupportedException("PR is not available in this command context.");}if(verb=="Z"){fitting=false;float nextZoom=rest.Length==0?1:float.Parse(rest,CultureInfo.InvariantCulture);if(float.IsNaN(nextZoom)||float.IsInfinity(nextZoom)||nextZoom<.1f||nextZoom>16)throw new InvalidDataException("Zoom range is 0.1 to 16.");canvas.Zoom=nextZoom;canvas.LayoutScore();UpdateClassic();return;}if(verb=="FIT"){fitting=true;Fit();return;}if(verb=="I"||verb=="N"){SelectOriginalItem(verb,rest);return;}if(verb=="U"||verb=="D"||verb=="L"||verb=="R"){bool horizontal=verb=="L"||verb=="R";float amount=rest.Length==0?(horizontal?lastHorizontalMove:lastVerticalMove):float.Parse(rest,CultureInfo.InvariantCulture);if(Single.IsNaN(amount)||Single.IsInfinity(amount))throw new InvalidDataException("Enter a finite movement amount.");if(rest.Length>0&&canvas.Selected!=null){if(horizontal)lastHorizontalMove=amount;else lastVerticalMove=amount;}MoveSelected(verb=="L"?-amount:verb=="R"?amount:0,verb=="U"?amount:verb=="D"?-amount:0);return;}
        float[] values;try{values=fields.Select(f=>float.Parse(f,CultureInfo.InvariantCulture)).ToArray();}catch(FormatException){Message("Command not yet implemented: "+verb);return;}if(values.Any(v=>float.IsNaN(v)||float.IsInfinity(v)||Math.Abs(v)>100000))throw new InvalidDataException("Enter finite SCORE parameters.");
        if(mode==ScreenMode.Edit&&canvas.Selected!=null){if(!TryEditExplicitEnharmonic(values))EditOriginalParameters(values);}
        else {if(values.Length<2||values[0]<1||values[0]>18||values[0]==13||values[0]==15||values[0]==16||values[0]!=(int)values[0]||values[1]<1||values[1]>32||values[1]!=(int)values[1])throw new InvalidDataException("Enter a supported object code and staff 1\u201332. Use TEXT for text objects.");Checkpoint();Item i=Item.Make(values);score.Items.Add(i);dirty=true;RefreshAll(null);Message("Created "+Item.Name(i.Code)+" #"+score.Items.Count);}
    }
    bool SaveNamed(string path,bool confirm){if(!CanSaveDocument()||score==null||!grid.EndEdit())return false;if(!Path.HasExtension(path))path+=String.IsNullOrEmpty(Path.GetExtension(current))?".MUS":Path.GetExtension(current);path=NativeOutputLocation.Resolve(source,path);if(confirm&&File.Exists(path)&&!(overwriteDecision!=null?overwriteDecision(path):MessageBox.Show(this,"Write over "+path+"?","Save SCORE file",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes))return false;PrepareParameterSave();var saved=OriginalMusSave.Prepare(score,source);byte[] bytes=saved.Encode();ScoreFileStorage.Write(path,bytes);score.Trailer=saved.Trailer;RecordParameterSave(saved.Trailer);savedDocumentBytes=NativeEditCompatibility.ReproduceInvalidStates&&score.CancelledSaveRecords!=null?score.Encode():bytes;originalLayoutMarksModified=false;current=path;score.SourcePath=path;dirty=false;NativeStartupFolder.RememberFile(path);RefreshAll(canvas.Selected);return true;}
    string Resolve(string path,string directory){if(String.IsNullOrWhiteSpace(path))throw new InvalidDataException("Enter a filename.");if(!Path.HasExtension(path))path+=String.IsNullOrEmpty(Path.GetExtension(current))?".MUS":Path.GetExtension(current);return Path.IsPathRooted(path)?path:Path.Combine(directory,path);}
    void ChooseRoot(){using(var d=new FolderBrowserDialog{Description="Select the existing SCORE folder containing LIB and MUSDAT",SelectedPath=source})if(d.ShowDialog(this)==DialogResult.OK){if(!Directory.Exists(Path.Combine(d.SelectedPath,"LIB")))throw new InvalidDataException("That folder has no LIB directory.");source=d.SelectedPath;canvas.Library=new DrawingLibrary(source);canvas.TextLayout=new ScoreText(source);canvas.Invalidate();Message("Using external LIB files from "+source);}}
    void ExportResolution(int width){using(var d=new SaveFileDialog{Filter="PNG image|*.png",FileName="score-"+width+".png",InitialDirectory=NativeOutputLocation.EnsureFolder(source),RestoreDirectory=true})if(d.ShowDialog(this)==DialogResult.OK)RenderScore(width,NativeOutputLocation.RequireWritableDestination(source,d.FileName));}
    public void RenderScore(int width,string path){float scale=width/canvas.ViewWidth;using(var bitmap=new Bitmap(width,(int)Math.Ceiling(canvas.ViewHeight*scale)))using(var g=Graphics.FromImage(bitmap)){g.Clear(Color.White);g.ScaleTransform(scale,scale);canvas.Draw(g,false);bitmap.Save(path,System.Drawing.Imaging.ImageFormat.Png);}}
    public void RenderUi(int width,int height,string path){renderExtent=new Size(width,height);MaximumSize=new Size(width,height);MinimumSize=new Size(width,height);Size=new Size(width,height);SetWindowPos(Handle,IntPtr.Zero,0,0,width,height,0x16);if(Width!=width||Height!=height)throw new Exception("Windows constrained render window to "+Width+"x"+Height+" instead of "+width+"x"+height);if(score==null)LoadFile(initialFile);canvas.DisplayDpi=1;fitting=true;LayoutClassic();PerformLayout();Fit();RevealSelection();Application.DoEvents();using(var bitmap=new Bitmap(width,height)){DrawToBitmap(bitmap,new Rectangle(0,0,width,height));int dark=0;for(int y=100;y<height-50;y+=8)for(int x=10;x<width-10;x+=8)if(bitmap.GetPixel(x,y).R<80)dark++;if(dark<150)throw new Exception("UI render is empty or missing its notation.");bitmap.Save(path,System.Drawing.Imaging.ImageFormat.Png);}}
    public void RenderWordUi(int width,int height,string path){if(score==null)LoadFile(initialFile);var item=score.Items.FirstOrDefault(x=>x.Code==16&&x.Text.Contains("K.545"))??score.Items.First(x=>x.Code==16);BeginEdit(item);RenderUi(width,height,path);File.WriteAllText(path+".layout.txt","Word="+itemTextLine.Text+"\r\nVisible="+itemTextLine.Visible+" Bounds="+itemTextLine.Bounds+"\r\nBody="+textBody.Bounds+" Scroll="+textWindow.AutoScrollPosition+"\r\nParameters="+parameters.Bounds+"\r\nContext="+contextLine.Bounds);}
    public void RenderEditUi(int width,int height,string path){if(score==null)LoadFile(initialFile);Item item=score.Items.FirstOrDefault(i=>i.Code==1);if(item!=null)BeginEdit(item);RenderUi(width,height,path);}
    public void CheckClassic(){
        if(score==null)LoadFile(initialFile);byte[] initial=score.Encode();int count=score.Items.Count;
        bool closeRequested=false;var priorUnsavedDecision=unsavedDecision;var priorMode=mode;FormClosingEventHandler cancelTestClose=(sender,args)=>{closeRequested=true;args.Cancel=true;};FormClosing+=cancelTestClose;unsavedDecision=()=>DialogResult.Cancel;try{var key=new KeyEventArgs(Keys.Alt|Keys.F4);OnKeyDown(key);if(!closeRequested||!key.SuppressKeyPress||mode!=priorMode||originalTextSetupActive)throw new Exception("Alt+F4 was routed into SCORE instead of Windows close.");}finally{FormClosing-=cancelTestClose;unsavedDecision=priorUnsavedDecision;}
        if(!initial.SequenceEqual(score.Encode()))throw new Exception("Cancelled host close changed the document.");
        Function(7);if(functionKeys[0].Text!="F1-LOAD")throw new Exception("Disk F-key context differs.");mode=ScreenMode.Main;
        Execute("1 1 50 7 10 0 1");if(score.Items.Count!=count+1)throw new Exception("Numeric item input failed.");Undo(false);if(!initial.SequenceEqual(score.Encode()))throw new Exception("Input undo changed the original score.");
        Item note=score.Items.First(i=>i.Code==1);float x=note.P(3);BeginEdit(note);if(functionKeys[2].Text!="F3-COPY")throw new Exception("Edit F-key context differs.");Execute("3 "+(x+5).ToString("R",CultureInfo.InvariantCulture));if(note.P(3)!=x+5)throw new Exception("Parameter edit failed.");Function(8);if(!initial.SequenceEqual(score.Encode()))throw new Exception("Abort did not restore the original object.");
        note=score.Items.First(i=>i.Code==1);BeginEdit(note);Function(3);if(score.Items.Count!=count||canvas.Selected!=score.Items[count-1])throw new Exception("Unmodified F3 selection differs.");BeginEdit(note);Execute("3 "+(note.P(3)+1).ToString("R",CultureInfo.InvariantCulture));Function(3);if(score.Items.Count!=count+1||canvas.Selected!=score.Items[count])throw new Exception("Modified copy failed.");ExitEdit(false);Undo(false);Undo(false);if(!initial.SequenceEqual(score.Encode()))throw new Exception("Copy/edit undo changed the original score.");
        Execute("Z 4");if(NativeDosViewport.ZoomFactor!=4||NativeDosViewport.Divisor!=12||!canvas.UsesOriginalZoomFrame)throw new Exception("SCORE-derived high-resolution zoom failed.");Execute("FIT");if(canvas.Zoom<=0||canvas.Zoom>16)throw new Exception("Fit zoom failed.");
        using(var c=new Canvas()){c.Score=Score.New();Item n=Item.Make(1,1,50,7,10,0,1);c.Score.Items.Add(n);c.LayoutScore();foreach(float dpi in new[]{1f,1.5f,2f})foreach(float zoom in new[]{.5f,1f,3f,6f}){c.DisplayDpi=dpi;c.Zoom=zoom;PointF p=c.Position(n);if(c.HitTest(new PointF(p.X*c.RenderScale,p.Y*c.RenderScale))!=n)throw new Exception("DPI/zoom hit testing differs.");}}
        var empty=new DrawingLibrary(Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString()));if(empty.Symbols.Count!=0)throw new Exception("Symbols should require the external LIB folder.");
        NativeDisplayChecks.VerifyEmbeddedResources();
        CheckMainLetters();CheckMainItemOrder();CheckMainGroupEditing();NativeItemMotion.CheckEndpointMotion();CheckMainNotationEditing();CheckOriginalStemSpacing();dirty=false;Console.WriteLine("PASS: original-style mode labels, numeric input, edit/abort, copy/undo, fit/zoom, selection at 12 DPI/zoom combinations, external-only symbols, authorized title logo and program icon embedded.");
    }
}
public static class ClassicChecks {
    public static int Verify(string root){try{using(var editor=new Editor(root,Path.Combine(root,"MUSDAT","PRACTICE.MUS"))){editor.Show();Application.DoEvents();editor.CheckClassic();editor.Close();}return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
    public static int Render(string[] args){try{using(var editor=new Editor(args[1],args[2])){editor.Show();Application.DoEvents();int width=int.Parse(args[4]),height=int.Parse(args[5]);if(args.Length>6&&args[6]=="edit-text"){editor.RenderWordUi(width,height,args[3]);}else if(args.Length>6&&args[6]=="edit")editor.RenderEditUi(width,height,args[3]);else editor.RenderUi(width,height,args[3]);editor.Close();}Console.WriteLine("PASS: native classic layout rendered at "+args[4]+" \u00D7 "+args[5]);return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
}

