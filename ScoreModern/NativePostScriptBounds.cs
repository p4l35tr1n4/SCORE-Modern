using System;using System.Collections.Generic;using System.Drawing;using System.Globalization;
namespace ScoreModern {
 internal static class NativePostScriptBounds {
  internal const string Program=@"
/SMboundnum {40 string cvs print ( ) print} bind def
/SMboundbox {
 20 dict begin count /depth exch def
 {pathbbox /t exch def /r exch def /b exch def /l exch def
 /p [l b transform l t transform r b transform r t transform] def
 /xl p 0 get def /xr xl def /yb p 1 get def /yt yb def
 0 2 6 {/i exch def /v p i get def v xl lt {/xl v def} if v xr gt {/xr v def} if
 /v p i 1 add get def v yb lt {/yb v def} if v yt gt {/yt v def} if} for
 (SMBOUND ) print /SMbounditem where {pop SMbounditem} {0} ifelse SMboundnum
 xl SMboundnum yb SMboundnum xr SMboundnum yt SMboundnum (\n) print
 } stopped {count depth sub {pop} repeat} if end
} bind def
/SMboundstroke /stroke load def
/SMboundfill /fill load def
/SMboundeofill /eofill load def
/SMboundshow /show load def
/SMboundawidthshow /awidthshow load def
/stroke {gsave strokepath SMboundbox grestore SMboundstroke} bind def
/fill {gsave SMboundbox grestore SMboundfill} bind def
/eofill {gsave SMboundbox grestore SMboundeofill} bind def
/SMboundstring {gsave false charpath SMboundbox grestore} bind def
/show {dup SMboundstring SMboundshow} bind def
/SMboundawidth {
 12 dict begin /str exch def /ay exch def /ax exch def /ch exch def /cy exch def /cx exch def
 gsave str {/c exch def /s 1 string def s 0 c put s SMboundstring s stringwidth rmoveto ax ay rmoveto c ch eq {cx cy rmoveto} if} forall grestore end
} bind def
/awidthshow {6 copy SMboundawidth SMboundawidthshow} bind def
";
  internal static Dictionary<int,Rectangle> Read(string log,int height){var result=new Dictionary<int,Rectangle>();foreach(string line in log.Split('\n')){if(!line.StartsWith("SMBOUND ",StringComparison.Ordinal))continue;var p=line.Split(new[]{' ','\r'},StringSplitOptions.RemoveEmptyEntries);int item;if(p.Length!=6||!Int32.TryParse(p[1],out item)||item<=0)continue;var n=new double[4];bool valid=true;for(int k=0;k<4;k++){double value;if(!Double.TryParse(p[k+2],NumberStyles.Float,CultureInfo.InvariantCulture,out value)||Double.IsNaN(value)||Double.IsInfinity(value)||Math.Abs(value)>100000000){valid=false;break;}n[k]=value;}if(!valid)continue;var box=Rectangle.FromLTRB((int)Math.Floor(n[0]),(int)Math.Floor(height-n[3]),(int)Math.Ceiling(n[2]),(int)Math.Ceiling(height-n[1]));if(box.Width<=0||box.Height<=0)continue;Rectangle old;result[item]=result.TryGetValue(item,out old)?Rectangle.Union(old,box):box;}return result;}
 }
}
