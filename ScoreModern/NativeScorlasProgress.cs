using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScoreModern {
 internal sealed class NativeScorlasProgress:ScoreDialog {
  readonly Label status=new Label{AutoSize=true,Dock=DockStyle.Fill};
  readonly ProgressBar bar=new ProgressBar{Dock=DockStyle.Fill};
  internal bool Cancelled{get;private set;}
  internal NativeScorlasProgress():base("scorlas-progress"){
   Text="SCORLAS batch";Font=SystemFonts.MessageBoxFont;ClientSize=new Size(520,140);StartPosition=FormStartPosition.CenterParent;ControlBox=false;
   var panel=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),RowCount=3,ColumnCount=1};panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));panel.RowStyles.Add(new RowStyle(SizeType.Percent,100));panel.RowStyles.Add(new RowStyle(SizeType.Absolute,24));panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
   var cancel=new Button{Text="Cancel after current page",AutoSize=true,Anchor=AnchorStyles.Right};cancel.Click+=(s,e)=>{Cancelled=true;cancel.Enabled=false;status.Text="Cancelling after the current page...";};panel.Controls.Add(status,0,0);panel.Controls.Add(bar,0,1);panel.Controls.Add(cancel,0,2);Controls.Add(panel);
  }
  // Invoked only between renderer frames; never pump during EPS drawing.
  internal void Report(int completed,int total){bar.Maximum=Math.Max(1,total);bar.Value=Math.Min(completed,bar.Maximum);if(!Cancelled)status.Text=completed==total?"All pages prepared. Finalizing...":"Preparing page "+(completed+1)+" of "+total+". Cancellation takes effect between pages.";Application.DoEvents();}
 }
}
