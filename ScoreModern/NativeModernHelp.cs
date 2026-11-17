using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 public sealed partial class Editor {
  internal static string ModernHelpPath(string topic){
   string name=Path.GetFileNameWithoutExtension(topic);
   return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Help",name+".md");
  }

  NativeHelpWindow modernHelp;
  bool OpenModernHelp(string topic,bool explicitModern=false){
   string path=ModernHelpPath(topic);
   if(!File.Exists(path))return false;
   if(explicitModern){
    using(var dialog=new NativeHelpWindow(Path.GetDirectoryName(path)){Icon=Icon}){
     dialog.Navigate(Path.GetFileNameWithoutExtension(path));
     dialog.ShowDialog(Form.ActiveForm??this);
    }
    return true;
   }
   if(modernHelp==null||modernHelp.IsDisposed)modernHelp=new NativeHelpWindow(Path.GetDirectoryName(path)){Icon=Icon};
   modernHelp.Navigate(Path.GetFileNameWithoutExtension(path));
   if(!modernHelp.Visible)modernHelp.Show(this);
   modernHelp.Activate();
   return true;
  }

  string ModernReferenceTopic(){
   if(musicInput!=null&&musicInput.Transaction!=null){
    switch(musicInput.Transaction.Stage){
     case InputStaffStage.Pitches:return "CODE33";
     case InputStaffStage.Rhythms:return "CODE34";
     case InputStaffStage.Marks:return "CODE35";
     case InputStaffStage.Beams:return "CODE36";
     case InputStaffStage.Slurs:return "CODE37";
    }
   }
   if(mode==ScreenMode.Print)return "PRINT";
   if(mode==ScreenMode.Edit&&canvas.Selected!=null)return "CODE"+canvas.Selected.Code.ToString("00")+"";
   if(mode==ScreenMode.EditMenu)return "CODE38";
   return "START";
  }
 }

 internal sealed class NativeHelpWindow:ScoreDialog {
  sealed class Topic{
   internal string Id,Title,Text;
   public override string ToString(){return Title;}
  }
  static readonly string[] WindowsTopics={"START","SETUP","WORKFLOW","VIEW","COMPAT","RESOURCES"};
  static readonly Regex LinkPattern=new Regex(@"\[([^\]]+)\]\(([A-Z0-9]+)\.md\)");
  static readonly Regex InlinePattern=new Regex(@"(`[^`]+`|\*\*[^*]+\*\*)");
  readonly List<Topic> topics=new List<Topic>();
  readonly ListBox list=new ListBox{Dock=DockStyle.Fill,IntegralHeight=false,HorizontalScrollbar=true};
  readonly TextBox search=new TextBox{Dock=DockStyle.Fill,AccessibleName="Search help topics"};
  readonly RichTextBox article=new RichTextBox{Dock=DockStyle.Fill,ReadOnly=true,DetectUrls=false,BorderStyle=BorderStyle.None,BackColor=SystemColors.Window,ForeColor=SystemColors.WindowText};
  readonly FlowLayoutPanel related=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,WrapContents=true,MaximumSize=new Size(0,95),AutoScroll=true};
  bool updating;

  internal NativeHelpWindow(string directory):base("modern-help"){
   Text="SCORE Modern Help";StartPosition=FormStartPosition.CenterParent;
   ClientSize=new Size(1000,690);MinimumSize=new Size(600,380);KeyPreview=true;
   foreach(string path in Directory.EnumerateFiles(directory,"*.md")){
    string text=File.ReadAllText(path,Encoding.UTF8);
    string title=text.Split('\n')[0].Trim().TrimStart('#',' ');
    topics.Add(new Topic{Id=Path.GetFileNameWithoutExtension(path),Title=title,Text=text});
   }
   var layout=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=2,Padding=new Padding(10)};
   layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,285));
   layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
   var toolbar=new TableLayoutPanel{Dock=DockStyle.Top,ColumnCount=2,AutoSize=true};
   toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
   toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
   toolbar.Controls.Add(new Label{Text="Search every topic",AutoSize=true,Anchor=AnchorStyles.Left},0,0);
   toolbar.Controls.Add(search,1,0);
   layout.Controls.Add(toolbar,0,0);layout.SetColumnSpan(toolbar,2);
   layout.Controls.Add(list,0,1);
   var page=new Panel{Dock=DockStyle.Fill,Padding=new Padding(10,0,0,0)};
   page.Controls.Add(article);page.Controls.Add(related);
   layout.Controls.Add(page,1,1);Controls.Add(layout);
   search.TextChanged+=(s,e)=>LoadList(null);
   list.SelectedIndexChanged+=(s,e)=>{if(!updating&&list.SelectedItem is Topic)Render((Topic)list.SelectedItem);};
   KeyDown+=(s,e)=>{
    if(e.KeyCode==Keys.Escape){e.SuppressKeyPress=true;Close();}
    if(e.Control&&e.KeyCode==Keys.F){e.SuppressKeyPress=true;search.Focus();search.SelectAll();}
   };
   LoadList("START");
  }

  internal void Navigate(string id){
   updating=true;try{search.Clear();}finally{updating=false;}
   LoadList(id);
  }

  void LoadList(string id){
   if(updating)return;
   var words=search.Text.Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
   var matching=topics.Where(t=>words.All(w=>t.Text.IndexOf(w,StringComparison.OrdinalIgnoreCase)>=0))
    .OrderBy(t=>{int position=Array.IndexOf(WindowsTopics,t.Id);return position<0?WindowsTopics.Length:position;})
    .ThenBy(t=>t.Id,StringComparer.OrdinalIgnoreCase).ToArray();
   updating=true;list.BeginUpdate();
   try{
    list.Items.Clear();list.Items.AddRange(matching);
    if(list.Items.Count>0){
     int requested=Array.FindIndex(matching,t=>t.Id==id);
     list.SelectedIndex=Math.Max(0,requested);
    }
   }finally{list.EndUpdate();updating=false;}
   if(list.SelectedItem is Topic)Render((Topic)list.SelectedItem);
   else{article.Text="No matching topics.";ClearRelated();}
  }

  void ClearRelated(){while(related.Controls.Count>0)related.Controls[0].Dispose();}

  void Render(Topic topic){
   article.SuspendLayout();
   try{
    article.Clear();ClearRelated();
    using(var body=new Font(Font.FontFamily,Math.Max(11,Font.SizeInPoints),FontStyle.Regular))
    using(var bold=new Font(body,FontStyle.Bold))
    using(var title=new Font(body.FontFamily,body.Size+8,FontStyle.Bold))
    using(var heading=new Font(body.FontFamily,body.Size+3,FontStyle.Bold))
    using(var mono=new Font("Consolas",body.Size,FontStyle.Regular)){
     bool code=false;
     foreach(string source in topic.Text.Replace("\r","").Split('\n')){
      if(source.StartsWith("```",StringComparison.Ordinal)){code=!code;continue;}
      if(code){Append(source+"\n",mono);continue;}
      string line=LinkPattern.Replace(source,"$1");
      if(line.StartsWith("# ")){Append(line.Substring(2)+"\n",title);continue;}
      if(line.StartsWith("## ")){Append(line.Substring(3)+"\n",heading);continue;}
      if(line.StartsWith("### ")){Append(line.Substring(4)+"\n",bold);continue;}
      if(line.StartsWith("| ---"))continue;
      if(line.StartsWith("| ")){
       var cells=line.Trim('|').Split('|');
       line=String.Join("    ·    ",cells.Select(cell=>cell.Trim()));
      }
      if(line.StartsWith("> "))line=line.Substring(2);
      if(line.StartsWith("- "))line="• "+line.Substring(2);
      foreach(string part in InlinePattern.Split(line)){
       if(part.StartsWith("**")&&part.EndsWith("**")&&part.Length>4)Append(part.Substring(2,part.Length-4),bold);
       else if(part.StartsWith("`")&&part.EndsWith("`")&&part.Length>1)Append(part.Substring(1,part.Length-2),mono);
       else Append(part,body);
      }
      Append("\n",body);
     }
    }
    var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach(Match match in LinkPattern.Matches(topic.Text)){
     string target=match.Groups[2].Value;
     if(!seen.Add(target)||!topics.Exists(t=>t.Id==target))continue;
     var link=new LinkLabel{Text=match.Groups[1].Value,AutoSize=true,Margin=new Padding(4)};
     link.LinkClicked+=(s,e)=>Navigate(target);
     related.Controls.Add(link);
    }
    article.Select(0,0);article.ScrollToCaret();
   }finally{article.ResumeLayout();}
  }

  void Append(string text,Font font){article.SelectionFont=font;article.SelectedText=text;}
 }
}
