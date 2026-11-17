using System;using System.IO;using System.Windows.Forms;
namespace ScoreModern {
 public static class TextCachePolicy {
  static bool configuredMarkModified;public static bool MarkModified{get{return !NativeScoreBehaviour.Enabled&&configuredMarkModified;}set{configuredMarkModified=value;}}
  static string Settings{get{return "text-cache";}}
  public static void Load(){MarkModified=false;try{MarkModified=NativeSettings.Exists(Settings)&&NativeSettings.ReadAllText(Settings).Trim()=="modified";}catch(IOException){}catch(UnauthorizedAccessException){}}
  public static void Set(bool value,bool save){if(save)NativeSettings.WriteAllText(Settings,value?"modified":"score");MarkModified=value;}
  public static bool SameDocument(byte[] a,byte[] b){if(a==null||b==null||a.Length!=b.Length)return false;if(MarkModified)return Equal(a,b);return Equal(WithoutWidths(a),WithoutWidths(b));}
  static bool Equal(byte[] a,byte[] b){for(int n=0;n<a.Length;n++)if(a[n]!=b[n])return false;return true;}
  static byte[] WithoutWidths(byte[] data){var copy=(byte[])data.Clone();int p=2;while(copy.Length-p>24){float f=BitConverter.ToSingle(copy,p);if(Single.IsNaN(f)||Single.IsInfinity(f))return data;int count=(int)Math.Round(f);long end=(long)p+(count+1L)*4;if(count<1||count>4096||end>copy.Length-20)return data;if(count>=13&&BitConverter.ToSingle(copy,p+4)==16){Array.Clear(copy,p+52,4);float length=BitConverter.ToSingle(copy,p+48);if(!Single.IsNaN(length)&&!Single.IsInfinity(length)&&length>=0&&length<=(count+1)*4-56){int terminator=p+56+(int)Math.Truncate(length);if(terminator<end)copy[terminator]=0;else if(terminator==end&&terminator<copy.Length-24&&(copy[terminator]==0||copy[terminator]==32))copy[terminator]=0;}}p=(int)end;}return copy;}
 }
 public sealed partial class Editor {
  string textCacheIssues="";
  TextCacheRefreshResult RefreshTextCaches(Item only=null){if(score==null||canvas.TextLayout==null)return null;var result=NativeTextCache.Refresh(score,canvas.TextLayout,only);textCacheIssues=String.Join(Environment.NewLine,result.Unresolved.ToArray());return result;}
  void AddTextCacheMenu(ContextMenuStrip menu){var item=new ToolStripMenuItem("Cache-only text refresh marks document modified");item.ToolTipText="Off: SCORE-style default. Refreshed widths are saved when you save, without causing an unsaved-changes prompt.";item.Click+=(s,e)=>Safe(()=>{TextCachePolicy.Set(!TextCachePolicy.MarkModified,true);RefreshAll(canvas.Selected);UpdateClassic();});menu.Items.Add(item);menu.Opening+=(s,e)=>item.Checked=TextCachePolicy.MarkModified;}
 }
}
