using System;using System.IO;using System.Linq;using System.Text;using System.Windows.Forms;
namespace ScoreModern {
 public sealed partial class Editor {
  Item externalDrawingNameItem;Score externalDrawingLoad;string externalDrawingLoadPath;bool externalDrawingOldBlock,externalDrawingReturnDisk;Action externalDrawingRedrawContinuation;ScreenMode externalDrawingRedrawMode;
  static bool NeedsExternalDrawingReset(Item item){return item.Code==11&&item.P(13)==1||item.Code==15&&item.P(13)!=0;}
  // D114/D115: edit replies change the record; load-time replies change only
  // the drawing working copy. Never encode the latter into the MUS record.
  bool BeginLoadedDrawingNames(Score next,string path){
   if(externalDrawingNameItem!=null)throw new InvalidOperationException("Finish the drawing filename request before opening another score.");
   Item item=next.Items.FirstOrDefault(i=>NeedsExternalDrawingReset(i)&&!i.ExternalDrawingResetResolved);
   if(item==null)return false;externalDrawingLoad=next;externalDrawingLoadPath=path;BeginExternalDrawingName(item);return true;
  }
  bool BeginExternalDrawingRedraw(Action continuation){
   var reset=score.Items.Where(NeedsExternalDrawingReset).ToArray();if(reset.Length==0)return false;
   foreach(var item in reset){item.ExternalDrawingResetResolved=false;item.ExternalDrawingNameOverride=null;item.ExternalPostScriptBoxOverride=null;}
   externalDrawingRedrawContinuation=continuation;externalDrawingRedrawMode=mode;externalDrawingLoad=score;externalDrawingLoadPath=current;
   mode=ScreenMode.Main;RefreshOriginalPrintMenu();LayoutClassic();BeginExternalDrawingName(reset[0]);return true;
  }
  void BeginExternalDrawingName(Item item){
   if((item.Code!=11&&item.Code!=15)||item.Count<13)throw new NotSupportedException("Code11 filename reset requires its P13 control.");
   externalDrawingNameItem=item;externalDrawingOldBlock=canvas.BlockItemEditing;canvas.BlockItemEditing=true;
   string name=Encoding.GetEncoding(437).GetString(item.Bytes,56,Math.Max(0,Math.Min(24,item.Bytes.Length-56)));int zero=name.IndexOf('\0');if(zero>=0)name=name.Substring(0,zero);
   Message((externalDrawingLoad==null?"Old name = "+name.TrimEnd(' ')+Environment.NewLine:"")+(item.Code==15?"Enter PostScript program name or \"-\" to cancel. ->":"Enter library name or \"-\" to cancel. ->"));command.Clear();command.Focus();
  }
  bool SubmitExternalDrawingName(string text){
   if(externalDrawingNameItem==null)return false;var item=externalDrawingNameItem;
   if(scor4ExternalItemInsertion&&text=="-"){
    scor4ExternalItemInsertion=false;externalDrawingNameItem=null;canvas.BlockItemEditing=externalDrawingOldBlock;
    SetScor4ItemsBank(0);Message("");command.Clear();command.Focus();return true;
   }
   if(scor4ExternalItemInsertion&&text=="*"){
    ShowScoreDirectory(item.Code==15?"*.EPS":"*.DRW");command.Clear();command.Focus();return true;
   }
   if(item.Code==15)return SubmitOriginalPostScriptName(text);
   if(text!="-"){
    if(text.Length>24)throw new NotSupportedException("Drawing names longer than 24 bytes have not been verified against SCORE.");
    var encoding=Encoding.GetEncoding(437,EncoderFallback.ExceptionFallback,DecoderFallback.ExceptionFallback);
    char[] upper=text.ToCharArray();for(int n=0;n<upper.Length;n++)if(upper[n]>='a'&&upper[n]<='z')upper[n]=(char)(upper[n]-'a'+'A');
    string value=new String(upper);byte[] bytes=encoding.GetBytes(value.PadRight(24,' '));
    var preview=item.Clone();preview.Set(13,0);preview.ExternalDrawingNameOverride=value.PadRight(24,' ');preview.ExternalDrawingResetResolved=true;
    if(!NativeEditCompatibility.ReproduceInvalidStates){
     if(value.Length==0||value.Any(Char.IsControl))throw new NotSupportedException("Enter a drawing library name, or - to keep the current name. Strict SCORE can retain original control bytes.");
     int selector;NativeDatUserDrawings.Load(preview,canvas.Library,out selector);
    }
    if(externalDrawingLoad!=null)item.ExternalDrawingNameOverride=preview.ExternalDrawingNameOverride;
    else{if(item.Count<19)item.Set(19,0);item.Set(13,0);Array.Copy(bytes,0,item.Bytes,56,24);item.ExternalDrawingNameOverride=null;item.ExternalPostScriptBoxOverride=null;}
   }
   CompleteExternalDrawingName(item,"");return true;
  }
  void CompleteExternalDrawingName(Item item,string message){
   item.ExternalDrawingResetResolved=true;externalDrawingNameItem=null;canvas.BlockItemEditing=externalDrawingOldBlock;command.Clear();
   if(scor4ExternalItemInsertion){scor4ExternalItemInsertion=false;AppendScor4ItemForEdit(item);Message(message);return;}
   if(externalDrawingLoad!=null){
    var next=externalDrawingLoad;string path=externalDrawingLoadPath;
    if(externalDrawingRedrawContinuation!=null){
     Item pending=next.Items.FirstOrDefault(i=>NeedsExternalDrawingReset(i)&&!i.ExternalDrawingResetResolved);
     if(pending!=null){BeginExternalDrawingName(pending);return;}
     var continuation=externalDrawingRedrawContinuation;externalDrawingRedrawContinuation=null;externalDrawingLoad=null;externalDrawingLoadPath=null;mode=externalDrawingRedrawMode;Message("");continuation();
    }else{externalDrawingLoad=null;externalDrawingLoadPath=null;SetDocument(next,path);if(externalDrawingNameItem==null&&externalDrawingReturnDisk){externalDrawingReturnDisk=false;mode=ScreenMode.Disk;UpdateClassic();LayoutClassic();}}
   }else{RefreshAll(item);Message(message);command.Focus();}
  }
  bool ExternalDrawingNameKey(KeyEventArgs e){
   if(externalDrawingNameItem==null)return false;
   if(e.Alt&&e.KeyCode==Keys.D&&!e.Shift){e.SuppressKeyPress=true;MoveTextBoundary(e.Control?-1:1);return true;}
   if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;string text=command.Text;command.Clear();Safe(()=>SubmitExternalDrawingName(text));return true;}
   if(e.KeyCode==Keys.Escape){e.SuppressKeyPress=true;command.Focus();command.AppendText("\x1b");return true;}
   if(e.KeyCode>=Keys.F1&&e.KeyCode<=Keys.F12||e.Control&&(e.KeyCode==Keys.O||e.KeyCode==Keys.S||e.KeyCode==Keys.Z||e.KeyCode==Keys.Y)){e.SuppressKeyPress=true;command.Focus();return true;}
   command.Focus();return true;
  }
 }
}
