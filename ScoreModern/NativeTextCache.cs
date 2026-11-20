using System;using System.IO;using System.Collections.Generic;using System.Linq;
namespace ScoreModern {
 public sealed class TextCacheRefreshResult {public int Updated,TerminatorWrites;public readonly List<string> Unresolved=new List<string>();}
 public static class NativeTextCache {
  public static TextCacheRefreshResult Refresh(Score score,ScoreText text,Item only=null){var result=new TextCacheRefreshResult();var page=new NativePageDocument(score);foreach(var item in score.Items){if(item.Code!=16||(only!=null&&!Object.ReferenceEquals(item,only)))continue;try{if(item.Count<13||item.P(12)<0||item.P(12)>item.Bytes.Length-56)throw new NotSupportedException("Short/special or inconsistent stored text length: cached width retained.");NativeNotationContext context;if(!page.Contexts.TryGetValue(item,out context))throw new NotSupportedException("No verified staff context for text cache refresh.");var layout=text.PrepareMetricLayout(item,item.P(12)==0?" ":item.Text,context.Scale,0);if(!layout.MetricsSupported||layout.Tokens.Any(t=>t.Code<0))throw new NotSupportedException("Unverified text controls: cached width retained.");text.RecordOriginalGlyphPass(layout);if(NativeTextTerminator.Refresh(score,item))result.TerminatorWrites++;float value=layout.Width;if(Single.IsNaN(value)||Single.IsInfinity(value))throw new NotSupportedException("Non-finite text width: cached value retained.");if(BitConverter.ToInt32(BitConverter.GetBytes(item.P(13)),0)!=BitConverter.ToInt32(BitConverter.GetBytes(value),0)){item.Set(13,value);result.Updated++;}}
   catch(Exception e){if(!(e is IOException)&&!(e is InvalidDataException)&&!(e is NotSupportedException))throw;result.Unresolved.Add("Item "+(score.Items.IndexOf(item)+1)+": "+e.Message);}}
   return result;
  }
 }
}
