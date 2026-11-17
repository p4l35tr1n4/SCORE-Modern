using System;using System.IO;using System.Linq;using System.Drawing;using System.Collections.Generic;using System.Windows.Forms;
namespace ScoreModern {
 internal static class NativeFirstSetup {
  internal static string[] Identities(string folder){
   if(!Directory.Exists(folder))return new string[0];
   return Directory.GetFiles(folder,"*.DAT")
    .Where(path=>{NativeScoreIdentity.Data data;return NativeScoreIdentity.TryRead(path,out data);})
    .OrderBy(path=>path,StringComparer.OrdinalIgnoreCase).ToArray();
  }
  internal static string Validate(string root,string library,string fonts,string identity,bool deferFonts){
   if(!Directory.Exists(root))return "Choose an existing startup folder.";
   if(!Directory.Exists(library)||!Directory.GetFiles(library,"*.DRW").Any())return "Choose a LIB folder containing SCORE DRW drawing banks.";
   foreach(string path in Directory.GetFiles(library,"LIB*.DRW")){try{DrawingLibrary.Load(path);}catch(Exception error){if(error is IOException||error is InvalidDataException)return "Unreadable drawing bank: "+Path.GetFileName(path);throw;}}

   if(String.IsNullOrEmpty(identity)||!Identities(library).Contains(identity,StringComparer.OrdinalIgnoreCase))return "Choose a verified serial-bearing DAT from the selected LIB. No serial values are displayed.";
   if(!deferFonts&&(!Directory.Exists(fonts)||NativeTupletFonts.Type1Files(fonts).Length==0))return "Choose a folder containing Type 1 PFB/PFA fonts, or explicitly defer font setup. No Windows font is substituted.";
   return null;
  }
  internal static Dictionary<string,string> Settings(string root,string lib,string fonts,string dat,bool defer){root=Path.GetFullPath(root);lib=Path.GetFullPath(lib);var values=new Dictionary<string,string>{{"startup-folder",root},{"first-setup","complete"},{"type1",defer?"":Path.GetFullPath(fonts)},{"dat",lib+"\n"+dat+"\n"},{NativeSettings.InstallationKey("library",root),"all\n"+lib},{NativeSettings.InstallationKey("score-preferences",root),"LIBRARY "+lib}};return values;}
  internal static string Show(string initial){Func<string> result;using(var form=Create(initial,true,out result))return form.ShowDialog()==DialogResult.OK?result():null;}
  // A plain Form deliberately reads/writes no window settings: cancelling leaves no INI.
  internal static Form Create(string initial,bool inspectOnOpen,out Func<string> resultValue){
   var form=new Form{Text="SCORE Modern first setup",StartPosition=FormStartPosition.CenterScreen,AutoScaleMode=AutoScaleMode.Dpi,AutoScaleDimensions=new SizeF(96,96),ClientSize=new Size(850,445),MinimumSize=new Size(500,300)};
   try{
    var body=new Panel{Dock=DockStyle.Fill,AutoScroll=true};form.Controls.Add(body);
    var panel=new TableLayoutPanel{Dock=DockStyle.Top,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,Padding=new Padding(16,16,16,4),ColumnCount=3,RowCount=7};panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));for(int row=0;row<7;row++)panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));body.Controls.Add(panel);
    var intro=new Label{AutoSize=true,MaximumSize=new Size(780,0),Text="First setup: resources in the current folder are shown first. Confirm these locations or browse for alternatives. Only selected folders are inspected. Settings are saved beside the EXE; SCORE resources and PREF.SCR are preserved."};panel.Controls.Add(intro,0,0);panel.SetColumnSpan(intro,3);
    var boxes=new List<TextBox>();string current=String.IsNullOrEmpty(initial)?Environment.CurrentDirectory:Path.GetFullPath(initial);
    foreach(string label in new[]{"Startup folder","Library (LIB)","Type 1 fonts"}){int row=boxes.Count+1;var box=new TextBox{Dock=DockStyle.Fill};boxes.Add(box);var browse=new Button{Text="Browse...",AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,MinimumSize=new Size(100,30),Anchor=AnchorStyles.Left|AnchorStyles.Right};panel.Controls.Add(new Label{Text=label,AutoSize=true,Anchor=AnchorStyles.Left,Margin=new Padding(3,3,12,3)},0,row);panel.Controls.Add(box,1,row);panel.Controls.Add(browse,2,row);browse.Click+=(s,e)=>{using(var dialog=new FolderBrowserDialog{Description="Choose "+label,SelectedPath=Directory.Exists(box.Text)?box.Text:current})if(dialog.ShowDialog(form)==DialogResult.OK)box.Text=dialog.SelectedPath;};}
    boxes[0].Text=current;boxes[1].Text=Path.Combine(current,"LIB");boxes[2].Text=Path.Combine(current,"PSFONTS");
    var dat=new ComboBox{Dock=DockStyle.Fill,DropDownStyle=ComboBoxStyle.DropDownList};panel.Controls.Add(new Label{Text="Serial DAT",AutoSize=true,Anchor=AnchorStyles.Left,Margin=new Padding(3,3,12,3)},0,4);panel.Controls.Add(dat,1,4);var inspect=new Button{Text="Inspect",AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,MinimumSize=new Size(100,30),Anchor=AnchorStyles.Left|AnchorStyles.Right};panel.Controls.Add(inspect,2,4);
    Action scan=()=>{dat.Items.Clear();try{foreach(string path in Identities(boxes[1].Text))dat.Items.Add(path);if(dat.Items.Count==1)dat.SelectedIndex=0;}catch(Exception error){MessageBox.Show(form,error.Message,"Unable to inspect folder");}};inspect.Click+=(s,e)=>scan();boxes[1].TextChanged+=(s,e)=>dat.Items.Clear();boxes[0].Leave+=(s,e)=>{if(boxes[0].Text!=current){current=boxes[0].Text;boxes[1].Text=Path.Combine(current,"LIB");boxes[2].Text=Path.Combine(current,"PSFONTS");scan();}};
    var defer=new CheckBox{Text="Configure Type 1 fonts later (text requiring missing fonts stays unavailable)",AutoSize=true};panel.Controls.Add(defer,0,5);panel.SetColumnSpan(defer,3);
    var notice=new Label{Text="The selected LIB supplies all resources during setup. Later LIB choices default to drawings only.",AutoSize=true,MaximumSize=new Size(780,0)};panel.Controls.Add(notice,0,6);panel.SetColumnSpan(notice,3);
    var buttons=new FlowLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,Dock=DockStyle.Bottom,FlowDirection=FlowDirection.RightToLeft,WrapContents=false,Padding=new Padding(16,8,16,16)};var cancel=new Button{Text="Cancel",AutoSize=true,MinimumSize=new Size(100,32),DialogResult=DialogResult.Cancel};var finish=new Button{Text="Use these folders",AutoSize=true,MinimumSize=new Size(155,32)};buttons.Controls.Add(cancel);buttons.Controls.Add(finish);form.Controls.Add(buttons);
    Action wrap=()=>{int width=Math.Max(1,body.ClientSize.Width-panel.Padding.Horizontal-6);foreach(var paragraph in new[]{intro,notice})if(paragraph.MaximumSize.Width!=width)paragraph.MaximumSize=new Size(width,0);};body.ClientSizeChanged+=(sender,args)=>wrap();
    form.Load+=(sender,args)=>{wrap();panel.PerformLayout();int height=panel.GetPreferredSize(new Size(body.ClientSize.Width,0)).Height+buttons.GetPreferredSize(new Size(form.ClientSize.Width,0)).Height;var work=Screen.FromControl(form).WorkingArea;form.ClientSize=new Size(form.ClientSize.Width,Math.Min(height,Math.Max(1,work.Height-(form.Height-form.ClientSize.Height))));};form.CancelButton=cancel;form.AcceptButton=finish;
    string result=null;finish.Click+=(s,e)=>{try{string error=Validate(boxes[0].Text,boxes[1].Text,boxes[2].Text,dat.SelectedItem as string,defer.Checked);if(error!=null){MessageBox.Show(form,error,"Setup needs attention");return;}NativeSettings.CompleteFirstSetup(boxes[0].Text,Settings(boxes[0].Text,boxes[1].Text,boxes[2].Text,(string)dat.SelectedItem,defer.Checked));result=Path.GetFullPath(boxes[0].Text);form.DialogResult=DialogResult.OK;}catch(Exception error){MessageBox.Show(form,error.Message,"Setup not saved");}};resultValue=()=>result;if(inspectOnOpen)scan();return form;
   }catch{form.Dispose();throw;}
  }
 }
}
