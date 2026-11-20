using System;using System.Linq;using System.Collections.Generic;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {internal sealed class OriginalTextTabSpacingException:InvalidOperationException {internal OriginalTextTabSpacingException():base("*** When both JR and tabs are used, p5 must = 1.") {}}
public sealed partial class ScoreText {
 // Original323E:0B45..0CD4: JR can align the prefix before its first
 // stored tab, and Type1 ink edges come from the second external PSC
 // record. Primitive-font correction uses the external DAT metric table.
 float OriginalInkGap(NativeTextLayout layout,IEnumerable<NativeTextToken> tokens,float staffScale){
  if(!layout.MetricsSupported)throw new NotSupportedException("Text alignment controls need original metric verification.");RecordOriginalConverterPass(layout);var token=tokens.LastOrDefault(t=>!t.TabX.HasValue&&t.Code>=0&&t.Code<1000);if(token==null)throw new NotSupportedException("Trailing text control is outside the verified alignment metric domain.");
  int id=token.FontWord%100,code=token.Code;float difference;
  if(id>=90&&id<=98){int index=code+1;if(index==136||index==150)index-=56;float metric;if(index<=105)metric=BitConverter.ToSingle(textDat,2892+index*4);else{int word=index*2-217;lock(originalMetricHistoryLock){if(word<0||word+1>=originalConverterWords.Length||!originalConverterWords[word].HasValue||!originalConverterWords[word+1].HasValue)throw new NotSupportedException("Original built-in-font alignment reads unestablished adjacent converter state.");uint bits=(uint)(ushort)originalConverterWords[word].Value|((uint)(ushort)originalConverterWords[word+1].Value<<16);metric=BitConverter.ToSingle(BitConverter.GetBytes(bits),0);}}difference=(float)((double)metric*.10000000149011612);}
  else{Face face;int metricFace=token.DatMetricFace;if(metricFace<0||metricFace>=90||!faces.TryGetValue(metricFace,out face)||face.RightEdges==null)throw new NotSupportedException("Text alignment requires the selected font's external PSC advance and ink-edge records.");if(code>=162){
    //323E:0C69..0CC9 uses162 entries per face, even for embedded
    // music codes162..166 and literal800. Banks follow load order,
    // not logical font numbers; unused banks stay zero until loaded.
    // Four cold controls and the font1-before-font0 history are measured.
    if(!(code==162||code==163||code==164||code==166||code==800))throw new NotSupportedException("This trailing text control leaves the measured font-bank adjacency domain.");
    int adjacent=-1,index=code%162;lock(originalMetricHistoryLock){int slot=originalAlignmentMetricFaces.IndexOf(metricFace);if(slot<0)throw new NotSupportedException("The original metric load order is unavailable.");slot+=code/162;if(slot>=10)throw new NotSupportedException("Text alignment beyond the ten original metric banks requires adjacent-storage verification.");if(slot<originalAlignmentMetricFaces.Count)adjacent=originalAlignmentMetricFaces[slot];}
    if(adjacent<0)difference=0;else{Face next;if(!faces.TryGetValue(adjacent,out next)||next.RightEdges==null)throw new NotSupportedException("The retained adjacent metric bank is unavailable.");float advance=(float)((double)next.Widths[index]/638);difference=(float)((double)advance-(double)next.RightEdges[index]/638);}
   }else{float advance=(float)((double)face.Widths[code]/638);difference=(float)((double)advance-(double)face.RightEdges[code]/638);}}float factor=(float)((double)layout.Parameters.Horizontal*staffScale/25);return (float)((double)difference*factor*3.2832999229431152);
 }
 internal float OriginalRightEdgeGap(Item item,float staffScale){var layout=PrepareMetricLayout(item,item.Text,staffScale,0);if(layout.Tokens.Any(t=>t.TabX.HasValue))throw new NotSupportedException("This alignment route requires a separate tab-prefix calculation.");return OriginalInkGap(layout,layout.Tokens,staffScale);}
 internal float OriginalRightAlignmentWidth(Item item,float staffScale,out float gap){var layout=PrepareMetricLayout(item,item.Text,staffScale,0);int tab=layout.Tokens.FindIndex(t=>t.TabX.HasValue);if(tab<0){gap=OriginalInkGap(layout,layout.Tokens,staffScale);return item.P(13);}if(layout.Parameters.Spacing!=1)throw new OriginalTextTabSpacingException();var prefix=layout.Tokens.Take(tab).ToList();float width=0;foreach(var token in prefix)width=TextMetricArithmetic.Accumulate(width,token.Advance);gap=OriginalInkGap(layout,prefix,staffScale);return width;}
}}
