using System;using System.Collections.Generic;using System.Linq;
namespace ScoreModern {public sealed partial class Editor {
 // The ordinary T/TT/TTR scanner separates on literal slash too. Unlike
 // note-positioned lyrics, !! and ?? remain text in this path. Backslash
 // is SCORE's printable slash escape and is not a separator.
 List<Item> PrepareOriginalFixedTextLine(OriginalTextEntry entry,string text,int font,out int finalFont){
  var result=new List<Item>();finalFont=font;string clean=text.TrimEnd('/');if(clean.Length==0)return result;
  var partEntry=new OriginalTextEntry{Item=entry.Item.Clone(),RawX=entry.RawX,ReadFile=entry.ReadFile};
  foreach(string part in clean.Split('/')){int next;var item=PrepareOriginalText(partEntry,part,finalFont,out next);result.Add(item);finalFont=next;partEntry.Item=partEntry.Item.Clone();partEntry.Item.Set(3,(float)((double)item.P(3)+item.P(13)+2));}
  // The next physical input line writes a new count header after this pass.
  // Apply the converter space only within the current line's allocated units.
  var lineScore=new Score{Items=result};foreach(var item in result)NativeTextTerminator.Refresh(lineScore,item);
  return result;
 }
 void RequireOriginalTextCapacity(IList<Item> added){if(score.Items.Count+added.Count>1420||score.Items.Sum(i=>(long)i.Bytes.Length)+added.Sum(i=>(long)i.Bytes.Length)>56800)throw new NotSupportedException("Text entry exceeds SCORE's item or parameter capacity.");}
}}
