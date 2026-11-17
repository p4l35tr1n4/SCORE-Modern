using System;using System.Globalization;using System.Linq;using System.Text.RegularExpressions;
namespace ScoreModern { public sealed partial class Editor {
 static string NormalizeCompactMovement(string text){return Regex.Replace(text,@"^([LRUD])([+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?)$","$1 $2",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);}
 bool ExecutePairedMovement(string text){
  const string number=@"[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?";
  var match=Regex.Match(text,@"^([LRUD]) *("+number+@")[ ,]+([LRUD]) *("+number+@")$",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
  if(!match.Success)return false;if(canvas.Selected==null)return true;
  var changed=canvas.Selected.Clone();float horizontal=lastHorizontalMove,vertical=lastVerticalMove;
  for(int n=0;n<2;n++){
   char direction=Char.ToUpperInvariant(match.Groups[n*2+1].Value[0]);float distance=Single.Parse(match.Groups[n*2+2].Value,CultureInfo.InvariantCulture);
   if(Single.IsNaN(distance)||Single.IsInfinity(distance))throw new InvalidOperationException("Enter a finite movement amount.");
   bool x=direction=='L'||direction=='R';char first=Char.ToUpperInvariant(match.Groups[1].Value[0]);if(n==1&&x==(first=='L'||first=='R'))break;if(x)horizontal=distance;else vertical=distance;
   float dx=x?(direction=='L'?-distance:distance):0,dy=x?0:(direction=='D'?-distance:distance);
   if(editEndpoint!=0)NativeItemMotion.MoveEndpoint(changed,editEndpoint,dx,dy);else{NativeItemMotion.Horizontal(changed,dx);NativeItemMotion.Vertical(changed,dy);}
  }
  if(!changed.Bytes.SequenceEqual(canvas.Selected.Bytes)){Checkpoint();canvas.Selected.Bytes=changed.Bytes;dirty=true;RefreshAll(canvas.Selected);}
  lastHorizontalMove=horizontal;lastVerticalMove=vertical;return true;
 }
}}
