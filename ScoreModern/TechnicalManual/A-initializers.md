# Exact source array initializers

This mechanical inventory includes both recovered tables and native integration arrays. A source declaration alone does not certify that its values came from DOS. The routine comments and captured evidence establish provenance. Source line numbers apply to the workspace at generation time.

## ClassicShell.cs

[Workspace source](../ClassicShell.cs)

Line 168

````csharp
new[]{(float)n,number}
````

Line 190

````csharp
new[]{Keys.Left,Keys.Right,Keys.Up,Keys.Down}
````

Line 212

````csharp
new[]{Keys.Left,Keys.Right,Keys.Up,Keys.Down}
````

Line 242

````csharp
new[]{"HELP","DIRECTION","COPY","CENTER","SET","ALIGN","DEL","ABORT"}
````

Line 242

````csharp
new[]{"MOVE","COPY","ALTER","DELETE","JUSTIFY","LINEUP","FLIP","TRANS"}
````

Line 242

````csharp
new[]{"HELP","POSITION","STAFF","ITEM","GROUP","MENU","",""}
````

Line 242

````csharp
new[]{"LOAD","SAVE","APPEND","READ","DIR","DRIVE","SET PATH","DELETE"}
````

Line 242

````csharp
new[]{"HELP","STAFF","NOTE","REST","CLEF","SYMBOL","TEXT","EXIT"}
````

Line 242

````csharp
new[]{"HELP","","","","","GO PRINT","",""}
````

Line 242

````csharp
new[]{"HELP","PRINT","PNG 4K","PNG 8K","EPS","READ PRINT.SCR","SAVE PRINT.SCR","EXIT"}
````

Line 242

````csharp
new[]{"HELP","INPUT","EDIT","TEXT","PLAY","PRINT","DISK","EXIT"}
````

Line 268

````csharp
new[]{' '}
````

Line 331

````csharp
new[]{1f,1.5f,2f}
````

Line 331

````csharp
new[]{.5f,1f,3f,6f}
````

## DrawingLibrary.cs

[Workspace source](../DrawingLibrary.cs)

Line 90

````csharp
new[]{original,dir}
````

## FontconvPscModel.cs

[Workspace source](../FontconvPscModel.cs)

Line 4

````csharp
Prefixes={"Ari","Ati","Nti","Oti","Sca","Zca","ari","ati","cop","min","nti","oti","reg","sca","tra","zca"}
````

Line 4

````csharp
Special={112,97,99,101,114,115,116,96,150,138,98,100,155,130,157,132}
````

Line 4

````csharp
Codes={95,70,79,133,134,135,136,77,39,40,73,71,38,72,37,74,1,2,3,4,5,6,7,8,9,10,67,68,139,75,140,69,141,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,146,147,148,163,76,137,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,158,159,160,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,105,106,107,163,131,119,108,109,110,111,120,123,124,122,125,163,127,117,113,163,163,128,102,163,103,104,121,118,163,163,129,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,126,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,163,142,163,161,163,163,163,163,144,145,143,162,163,163,163,163,163,149,163,163,163,152,163,163,153,154,151,156}
````

Line 27

````csharp
paths={installed,reusable}
````

Line 27

````csharp
temporary={null,null}
````

Line 27

````csharp
backup={null,null}
````

Line 27

````csharp
committed={false,false}
````

Line 27

````csharp
skip={false,false}
````

## GhostscriptTextRenderer.cs

[Workspace source](../GhostscriptTextRenderer.cs)

Line 14

````csharp
new[]{Enabled?"ghostscript":"native",Executable}
````

Line 17

````csharp
new[]{new PointF(0,0),new PointF(1,0),new PointF(0,1)}
````

Line 17

````csharp
new[]{p[1].X-p[0].X,p[1].Y-p[0].Y,p[2].X-p[0].X,p[2].Y-p[0].Y,p[0].X,p[0].Y}
````

Line 21

````csharp
new[]{new PointF(r.Left,r.Top),new PointF(r.Right,r.Top),new PointF(r.Left,r.Bottom),new PointF(r.Right,r.Bottom)}
````

## IncludedPostScriptDisplay.cs

[Workspace source](../IncludedPostScriptDisplay.cs)

Line 8

````csharp
new[]{configuredSearchBesideMusic?"score-and-mus":"score-only",configuredMapDosPaths?"mapped-dos-paths":"windows-paths"}
````

## MainDecombine.cs

[Workspace source](../MainDecombine.cs)

Line 23

````csharp
new short[]{1}
````

## MainGroupEditing.cs

[Workspace source](../MainGroupEditing.cs)

Line 19

````csharp
originalNotationRange={1,0,220,0,1}
````

Line 101

````csharp
new[]{' ',',','\t'}
````

Line 352

````csharp
new[]{new {Command="FLT",Value=11f},new {Command="SHP",Value=12f},
     new {Command="NAT",Value=13f},new {Command="NAC",Value=10f}}
````

Line 423

````csharp
new[]{Item.Make(1,5,160,7,10,0,1),Item.Make(1,4,120,9,10,0,1),
     Item.Make(2,5,50,2,0,25),Item.Make(1,4,40,7,10,0,1),
     Item.Make(1,5,40,9,10,0,1),Item.Make(2,4,40)}
````

Line 436

````csharp
new[]{2,1,2,1,2,1}
````

Line 528

````csharp
new[]{shiftedNote,cueNote,octaveNote}
````

Line 532

````csharp
new[]{shiftedNote,cueNote,octaveNote}
````

Line 576

````csharp
new[]{unitLine,unitBeam}
````

## MainItemOrder.cs

[Workspace source](../MainItemOrder.cs)

Line 30

````csharp
new[]{' ',',','\t'}
````

Line 76

````csharp
new[]{0,1,5,7,4,6,3,2}
````

Line 78

````csharp
new[]{0,5,7,3,1,4,6,2}
````

Line 80

````csharp
new[]{1,4,6,2,0,5,7,3}
````

Line 82

````csharp
new[]{1,4,2,0,5,7,6,3}
````

Line 84

````csharp
new[]{1,4,6,2,0,5,7,3}
````

Line 86

````csharp
new[]{1,4,2,0,5,7,6,3}
````

Line 90

````csharp
new[]{new[]{1,1,70},new[]{3,2,30},new[]{6,2,10},new[]{9,1,30}}
````

Line 90

````csharp
new[]{1,1,70}
````

Line 90

````csharp
new[]{3,2,30}
````

Line 90

````csharp
new[]{6,2,10}
````

Line 90

````csharp
new[]{9,1,30}
````

Line 99

````csharp
new[]{6,3,2,4,7,10,9,1,0,5,8,11}
````

## MainLayoutCommands.cs

[Workspace source](../MainLayoutCommands.cs)

Line 95

````csharp
new[]{LayoutDuplicateText()}
````

Line 104

````csharp
new[]{pref.Number("AFTC",0),pref.Number("AFTB",0),pref.Number("AFTK",0),pref.Number("AFTM",0)}
````

## MainLetterCommands.cs

[Workspace source](../MainLetterCommands.cs)

Line 127

````csharp
new[]{".",".."}
````

Line 132

````csharp
new[]{lines.ToArray()}
````

Line 148

````csharp
new[]{"\r\n","\n"}
````

Line 159

````csharp
OriginalParameterLabels={"","NOTE ","REST ","CLEF ","LINE ","SLUR ","BEAM ","TRILL","STAFF","MISC ","NUMB ","LIBRY","CIRCL","DUMMY","BARS ","POST ","WORD ","KSIG ","METER","MPUMX"}
````

Line 288

````csharp
new[]{new[]{"N4701","N4702.MUS","N4700.MUS"},new[]{"W47Z09","W47Z10.MUS","W47Z08.MUS"},new[]{"W47Z99","W48A00.MUS","W47Z98.MUS"},new[]{"W47Z00","W47Z01.MUS","W47Y99.MUS"},new[]{"J","I.MUS","I.MUS"},new[]{"T47A","T47B.MUS","T46Z.MUS"}}
````

Line 288

````csharp
new[]{"N4701","N4702.MUS","N4700.MUS"}
````

Line 288

````csharp
new[]{"W47Z09","W47Z10.MUS","W47Z08.MUS"}
````

Line 288

````csharp
new[]{"W47Z99","W48A00.MUS","W47Z98.MUS"}
````

Line 288

````csharp
new[]{"W47Z00","W47Z01.MUS","W47Y99.MUS"}
````

Line 288

````csharp
new[]{"J","I.MUS","I.MUS"}
````

Line 288

````csharp
new[]{"T47A","T47B.MUS","T46Z.MUS"}
````

## MainNotationEditing.cs

[Workspace source](../MainNotationEditing.cs)

Line 99

````csharp
new[]{1f,0f,200f,0f,staffCount}
````

Line 135

````csharp
new[]{n}
````

Line 238

````csharp
new[]{4,5,8,9,10}
````

Line 246

````csharp
new[]{4,5,8,9,10,19}
````

Line 348

````csharp
new[]{' ', ',', '\t'}
````

Line 429

````csharp
new[]{"FL","FLU","FLD","FLO"}
````

Line 441

````csharp
new[]{
      Item.Make(8,1,0,0,1,200),
      Item.Make(1,1,20,4,up?11:21,0,.5f,0,1,0,0),
      Item.Make(5,1,20,up?2:6,up?8:13,40,up?-2.5f:2.5999999046325684f,-1),
      Item.Make(1,1,40,11,down?22:12,0,.5f,0,1,0,0),
      Item.Make(5,1,40,down?13:8,down?10:4,50,down?1.6999999284744263f:-1.7999999523162842f,-1),
      Item.Make(1,1,50,7,down?20:10,0,.5f,0,1,0,0)
     }
````

Line 500

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,3f,10f,0f,0.5f,5.00066614f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,30f,5f,10f,0f,0.5f,3.66688871f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,40f,7f,10f,0f,0.5f,2.33311129f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,50f,10f,10f,0f,0.5f,-0.00066614151f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,1f,20f,8f,10f,50f,11f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,103f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,2f,30f,105f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,2f,40f,107f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,2f,50f,110f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(6f,2f,20f,8f,10f,50f,11f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,-103f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,3f,30f,-101f,10f,0f,0.5f,2f,1f,0f,0f,0f,0f,0f,0f),Item.Make(1f,3f,40f,-99f,10f,0f,0.5f,108.333115f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,3f,50f,-96f,10f,0f,0.5f,105.999336f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,3f,20f,8f,10f,50f,11f),Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,3f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,4f,30f,5f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,4f,40f,7f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,4f,50f,10f,10f,0f,0.5f,2f,0f,0f,0f,1f,0f,0f,0f),Item.Make(6f,4f,20f,8f,10f,50f,11f),Item.Make(8f,5f,0f,100f,1f,200f),Item.Make(1f,5f,20f,3f,10f,0f,0.5f,5.00066614f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,5f,30f,5f,10f,0f,0.5f,3.66688871f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,5f,40f,7f,10f,0f,0.5f,2.33311129f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,5f,50f,10f,10f,0f,0.5f,2f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(6f,5f,20f,8f,10f,50f,11f),Item.Make(8f,6f,0f,125f,1f,200f),Item.Make(1f,6f,20f,3f,10f,0f,0.5f,5.00066614f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(1f,6f,30f,5f,10f,0f,0.5f,3.66688871f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(1f,6f,40f,7f,10f,0f,0.5f,2.33311129f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(1f,6f,50f,10f,10f,0f,0.5f,2f,0f,0f,0f,0f,0f,0f,1.5f),Item.Make(6f,6f,20f,8f,10f,50f,11f)}
````

Line 505

````csharp
new[]{new[]{20f,3f},new[]{30f,5f},new[]{40f,7f},new[]{50f,10f}}
````

Line 505

````csharp
new[]{20f,3f}
````

Line 505

````csharp
new[]{30f,5f}
````

Line 505

````csharp
new[]{40f,7f}
````

Line 505

````csharp
new[]{50f,10f}
````

Line 511

````csharp
extensions={-.0003330707550048828f,1.666555643081665f,3.333444356918335f,6.000332832336426f}
````

Line 515

````csharp
new[]{new[]{.5f,.25f,.125f,.5f},new[]{.125f,.25f,.5f,.125f},new[]{.5f,.5f,.25f,.5f}}
````

Line 515

````csharp
new[]{.5f,.25f,.125f,.5f}
````

Line 515

````csharp
new[]{.125f,.25f,.5f,.125f}
````

Line 515

````csharp
new[]{.5f,.5f,.25f,.5f}
````

Line 518

````csharp
pitches={3,5,7,10}
````

Line 524

````csharp
new[]{.9996669292449951f,2.666555643081665f,4.333444595336914f,7.000332832336426f}
````

Line 529

````csharp
new[]{"FL","FLU","FLD","FLN"}
````

Line 539

````csharp
new[]{
      Item.Make(8,1,0,0,1,200),
      Item.Make(1,1,40,down?5:1,down?20:10,0,1,4,0,0,0),
      Item.Make(1,1,40,3,0,0,1,0,0,0,0),
      Item.Make(1,1,40,down?1:5,0,0,1,0,0,0,0)
     }
````

Line 561

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,5f,22.2830009f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,20f,4f,1.25f,0f,1f,0f,0f,20f,0f),Item.Make(1f,1f,20f,3f,1.773f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,4f,20f,0f,1f,3f,0f,0f,0f),Item.Make(1f,1f,40f,2f,1.28299999f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,1f,2.24000001f,0f,1f,0f,0f,20f,0f),Item.Make(1f,1f,60f,8f,20f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f,20f,0f),Item.Make(1f,1f,60f,6f,0f,0f,1f,0f,0f,0f,0f)}
````

Line 564

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,3f,11.4899998f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,20f,4f,1.25f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,20f,5f,2f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,1f,12.2399998f,0f,1f,3f,0f,0f,0f),Item.Make(1f,1f,40f,2f,1f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,40f,4f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,60f,6f,10f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,60f,8f,0f,0f,1f,0f,0f,0f,0f)}
````

Line 567

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,3f,11.4899998f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,20f,4f,1.25f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,20f,5f,2f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,40f,1f,12.2399998f,0f,1f,3f,0f,0f,0f),Item.Make(1f,1f,40f,2f,1f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,40f,4f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,60f,6f,10f,0f,1f,2f,0f,0f,0f),Item.Make(1f,1f,60f,7f,0f,0f,1f,0f,0f,10f,0f),Item.Make(1f,1f,60f,8f,0f,0f,1f,0f,0f,0f,0f)}
````

Line 613

````csharp
new[]{Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,3f,20f,0f,0.25f,-0.000333070755f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(6f,2f,20f,3f,4f,50f,22f,0f,0f,0f,0f),Item.Make(1f,2f,30f,5f,20f,0f,0.25f,1.66655564f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,40f,7f,20f,0f,0.25f,3.33344436f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,50f,10f,20f,0f,0.25f,6.00033283f,0f,0f,0f,0f,0f,0f,0.699999988f)}
````

Line 616

````csharp
new[]{Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,3f,20f,0f,0.25f,42.9996681f,0f,0f,0f,1f,0f,0f,0f),Item.Make(6f,3f,20f,3f,4f,50f,22f,0f,0f,0f,0f),Item.Make(1f,3f,30f,5f,20f,0f,0.25f,44.6665573f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,40f,7f,20f,0f,0.25f,46.3334427f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,50f,10f,20f,0f,0.25f,49.0003319f,0f,0f,0f,1f,0f,0f,0f)}
````

Line 619

````csharp
new[]{Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,3f,20f,0f,0.25f,-0.000333070755f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,4f,20f,3f,4f,50f,22f,0f,0f,0f,0f),Item.Make(1f,4f,30f,5f,20f,0f,0.25f,1.66655564f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,40f,7f,20f,0f,0.25f,3.33344436f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,50f,10f,20f,0f,0.25f,6.00033283f,0f,0f,0f,0f,0f,0f,0f)}
````

Line 639

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,1f,20f,6f,13f,40f,2.5999999f,-2f),Item.Make(1f,1f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)}
````

Line 642

````csharp
new[]{Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,2f,20f,6f,13f,40f,2.5999999f,0f),Item.Make(1f,2f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)}
````

Line 645

````csharp
new[]{Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,1f),Item.Make(1f,3f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)}
````

Line 648

````csharp
new[]{Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,4f,20f,0f,0.5f,0f,1f,0f,0f),Item.Make(5f,4f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,4f,40f,11f,20f,0f,0.5f,0f,1f,0f,0f)}
````

Line 694

````csharp
new[]{Item.Make(8f,6f,0f,125f,1f,200f),Item.Make(1f,6f,20f,3f,20f,0f,0.25f,-0.000333070755f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,6f,20f,3f,4f,50f,22f,0f,1f,0f,0f),Item.Make(1f,6f,30f,5f,20f,0f,0.25f,1.66655564f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,40f,7f,20f,0f,0.25f,3.33344436f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,50f,10f,20f,0f,0.25f,6.00033283f,0f,0f,0f,0f,0f,0f,0f)}
````

Line 714

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,10f,0f,1f,9f,0f,0f,0f),Item.Make(1f,1f,20f,8f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,20f,10f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,50f,4f,10f,0f,1f,5f,0f,0f,0f),Item.Make(1f,1f,50f,6f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,50f,9f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,80f,-2f,10f,0f,1f,18f,0f,0f,0f),Item.Make(1f,1f,80f,7f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,80f,16f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,110f,1f,10f,0f,1f,12f,0f,0f,0f),Item.Make(1f,1f,110f,7f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,110f,13f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,140f,1f,10f,0f,1f,6f,0f,0f,0f),Item.Make(1f,1f,140f,5f,0f,0f,1f,0f,0f,0f,0f),Item.Make(1f,1f,140f,7f,0f,0f,1f,0f,0f,0f,0f)}
````

Line 734

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,5f,21f,0f,0.25f,3.99950051f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,1f,1.15499997f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,1f,2f,40f,22f),Item.Make(1f,1f,40f,9f,21f,0f,0.25f,7.00049925f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.523f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,40f,4f,1f,0f,0.25f,0f,0f,20f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f)}
````

Line 737

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f)}
````

Line 740

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,3f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,10f,0f,0f)}
````

Line 743

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,5f,21f,0f,0.25f,4f,112f,0f,0f),Item.Make(1f,2f,20f,3f,1.39499998f,0f,0.25f,0f,110f,0f,0f),Item.Make(1f,2f,20f,1f,1.15499997f,0f,0.25f,0f,110f,0f,0f),Item.Make(1f,2f,40f,9f,21f,0f,0.25f,5f,112f,0f,0f),Item.Make(1f,2f,40f,5f,1.523f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,1f,0f,0.25f,0f,1110f,20f,0f)}
````

Line 746

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11.1549997f,0f,0.25f,4f,12f,0f,0f),Item.Make(1f,2f,20f,3f,1.39499998f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11.1199999f,0f,0.25f,5f,1112f,0f,0f),Item.Make(1f,2f,40f,5f,1.36000001f,0f,0.25f,0f,10f,10f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,1010f,0f,0f)}
````

Line 749

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,1f,11.1549997f,0f,0.25f,6.50074959f,0f,0f,0f),Item.Make(1f,1f,20f,3f,1.39499998f,0f,0.25f,0f,0f,0f,0f),Item.Make(1f,1f,20f,5f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(6f,1f,20f,7.5f,9f,40f,12f),Item.Make(1f,1f,40f,4f,11.1199999f,0f,0.25f,4.99925089f,0f,0f,0f),Item.Make(1f,1f,40f,5f,1.36000001f,0f,0.25f,0f,0f,10f,0f),Item.Make(1f,1f,40f,9f,1f,0f,0.25f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,20f,1f,11.1549997f,0f,0.25f,4f,12f,0f,0f),Item.Make(1f,2f,20f,3f,1.39499998f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,20f,5f,1f,0f,0.25f,0f,10f,0f,0f),Item.Make(1f,2f,40f,4f,11.1199999f,0f,0.25f,5f,1112f,0f,0f),Item.Make(1f,2f,40f,5f,1.36000001f,0f,0.25f,0f,10f,10f,0f),Item.Make(1f,2f,40f,9f,1f,0f,0.25f,0f,1010f,0f,0f)}
````

Line 760

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,10f,4f,20f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,30f,4f,20f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,50f,4f,120f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,70f,4f,120f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,90f,4f,21f,0f,2f,0f,0f,0f,1f)}
````

Line 763

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,10f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,30f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,50f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,70f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,90f,4f,11f,0f,2f,0f,0f,0f,1f)}
````

Line 766

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,10f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,30f,4f,10f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,50f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,70f,4f,110f,0f,2f,0f,0f,0f,1f),Item.Make(1f,1f,90f,4f,11f,0f,2f,0f,0f,0f,1f)}
````

Line 811

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,5f,4f,20f,0f,0.5f,0f,1f,0f,1f),Item.Make(1f,1f,10f,11f,20f,0f,0.5f,0f,1f,0f,2f),Item.Make(1f,1f,15f,4f,20f,0f,0.5f,0f,1f,0f,3f),Item.Make(1f,1f,20f,11f,20f,0f,0.5f,0f,1f,0f,4f),Item.Make(1f,1f,25f,4f,20f,0f,0.5f,0f,1f,0f,5.6500001f),Item.Make(1f,1f,30f,11f,20f,0f,0.5f,0f,1f,0f,6f),Item.Make(1f,1f,35f,4f,20f,0f,0.5f,0f,1f,0f,7f),Item.Make(1f,1f,40f,11f,20f,0f,0.5f,0f,1f,0f,8f),Item.Make(1f,1f,45f,4f,20f,0f,0.5f,0f,1f,0f,9.60000038f),Item.Make(1f,1f,50f,11f,20f,0f,0.5f,0f,1f,0f,10f),Item.Make(1f,1f,55f,4f,20f,0f,0.5f,0f,1f,0f,11f),Item.Make(1f,1f,60f,11f,20f,0f,0.5f,0f,1f,0f,12f),Item.Make(1f,1f,65f,4f,20f,0f,0.5f,0f,1f,0f,13f),Item.Make(1f,1f,70f,11f,20f,0f,0.5f,0f,1f,0f,14f),Item.Make(1f,1f,75f,4f,20f,0f,0.5f,0f,1f,0f,15f),Item.Make(1f,1f,80f,11f,20f,0f,0.5f,0f,1f,0f,16f),Item.Make(1f,1f,85f,4f,20f,0f,0.5f,0f,1f,0f,17f),Item.Make(1f,1f,90f,11f,20f,0f,0.5f,0f,1f,0f,18f),Item.Make(1f,1f,95f,4f,20f,0f,0.5f,0f,1f,0f,19.6000004f),Item.Make(1f,1f,100f,11f,20f,0f,0.5f,0f,1f,0f,20f),Item.Make(1f,1f,105f,4f,20f,0f,0.5f,0f,1f,0f,21f),Item.Make(1f,1f,110f,11f,20f,0f,0.5f,0f,1f,0f,22f),Item.Make(1f,1f,115f,4f,20f,0f,0.5f,0f,1f,0f,23f),Item.Make(1f,1f,120f,11f,20f,0f,0.5f,0f,1f,0f,24f),Item.Make(1f,1f,125f,4f,20f,0f,0.5f,0f,1f,0f,25f),Item.Make(1f,1f,130f,11f,20f,0f,0.5f,0f,1f,0f,26f),Item.Make(1f,1f,135f,4f,20f,0f,0.5f,0f,1f,0f,27f),Item.Make(1f,1f,140f,11f,20f,0f,0.5f,0f,1f,0f,28f),Item.Make(1f,1f,145f,4f,20f,0f,0.5f,0f,1f,0f,29f),Item.Make(1f,1f,150f,11f,20f,0f,0.5f,0f,1f,0f,30f),Item.Make(1f,1f,155f,4f,20f,0f,0.5f,0f,1f,0f,31f),Item.Make(1f,1f,160f,11f,20f,0f,0.5f,0f,1f,0f,32f),Item.Make(1f,1f,165f,4f,20f,0f,0.5f,0f,1f,0f,33f),Item.Make(1f,1f,170f,11f,20f,0f,0.5f,0f,1f,0f,34f),Item.Make(1f,1f,175f,4f,20f,0f,0.5f,0f,1f,0f,35f),Item.Make(1f,1f,180f,11f,20f,0f,0.5f,0f,1f,0f,36f),Item.Make(1f,1f,185f,4f,20f,0f,0.5f,0f,1f,0f,4.5999999f),Item.Make(1f,1f,190f,11f,20f,0f,0.5f,0f,1f,0f,5.05000019f),Item.Make(1f,1f,195f,4f,20f,0f,0.5f,0f,1f,0f,8.60000038f)}
````

Line 814

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,5f,4f,10f,0f,0.5f,0f,1f,0f,1f),Item.Make(1f,1f,10f,11f,10f,0f,0.5f,0f,1f,0f,2f),Item.Make(1f,1f,15f,4f,10f,0f,0.5f,0f,1f,0f,3f),Item.Make(1f,1f,20f,11f,10f,0f,0.5f,0f,1f,0f,4.69999981f),Item.Make(1f,1f,25f,4f,10f,0f,0.5f,0f,1f,0f,5.05000019f),Item.Make(1f,1f,30f,11f,10f,0f,0.5f,0f,1f,0f,6f),Item.Make(1f,1f,35f,4f,10f,0f,0.5f,0f,1f,0f,7f),Item.Make(1f,1f,40f,11f,10f,0f,0.5f,0f,1f,0f,8.69999981f),Item.Make(1f,1f,45f,4f,10f,0f,0.5f,0f,1f,0f,9f),Item.Make(1f,1f,50f,11f,10f,0f,0.5f,0f,1f,0f,10.6999998f),Item.Make(1f,1f,55f,4f,10f,0f,0.5f,0f,1f,0f,11f),Item.Make(1f,1f,60f,11f,10f,0f,0.5f,0f,1f,0f,12f),Item.Make(1f,1f,65f,4f,10f,0f,0.5f,0f,1f,0f,13f),Item.Make(1f,1f,70f,11f,10f,0f,0.5f,0f,1f,0f,14f),Item.Make(1f,1f,75f,4f,10f,0f,0.5f,0f,1f,0f,15f),Item.Make(1f,1f,80f,11f,10f,0f,0.5f,0f,1f,0f,16f),Item.Make(1f,1f,85f,4f,10f,0f,0.5f,0f,1f,0f,17f),Item.Make(1f,1f,90f,11f,10f,0f,0.5f,0f,1f,0f,18f),Item.Make(1f,1f,95f,4f,10f,0f,0.5f,0f,1f,0f,19f),Item.Make(1f,1f,100f,11f,10f,0f,0.5f,0f,1f,0f,20f),Item.Make(1f,1f,105f,4f,10f,0f,0.5f,0f,1f,0f,21f),Item.Make(1f,1f,110f,11f,10f,0f,0.5f,0f,1f,0f,22f),Item.Make(1f,1f,115f,4f,10f,0f,0.5f,0f,1f,0f,23f),Item.Make(1f,1f,120f,11f,10f,0f,0.5f,0f,1f,0f,24f),Item.Make(1f,1f,125f,4f,10f,0f,0.5f,0f,1f,0f,25f),Item.Make(1f,1f,130f,11f,10f,0f,0.5f,0f,1f,0f,26f),Item.Make(1f,1f,135f,4f,10f,0f,0.5f,0f,1f,0f,27f),Item.Make(1f,1f,140f,11f,10f,0f,0.5f,0f,1f,0f,28f),Item.Make(1f,1f,145f,4f,10f,0f,0.5f,0f,1f,0f,29f),Item.Make(1f,1f,150f,11f,10f,0f,0.5f,0f,1f,0f,30f),Item.Make(1f,1f,155f,4f,10f,0f,0.5f,0f,1f,0f,31f),Item.Make(1f,1f,160f,11f,10f,0f,0.5f,0f,1f,0f,32f),Item.Make(1f,1f,165f,4f,10f,0f,0.5f,0f,1f,0f,33f),Item.Make(1f,1f,170f,11f,10f,0f,0.5f,0f,1f,0f,34f),Item.Make(1f,1f,175f,4f,10f,0f,0.5f,0f,1f,0f,35f),Item.Make(1f,1f,180f,11f,10f,0f,0.5f,0f,1f,0f,36f),Item.Make(1f,1f,185f,4f,10f,0f,0.5f,0f,1f,0f,4f),Item.Make(1f,1f,190f,11f,10f,0f,0.5f,0f,1f,0f,5.75f),Item.Make(1f,1f,195f,4f,10f,0f,0.5f,0f,1f,0f,8f)}
````

Line 860

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,103f,20f,0f,0.25f,1.3327831f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,1f,20f,102.199997f,3.20000005f,50f,22f,0f,0f,0f,0f),Item.Make(1f,1f,30f,105f,20f,0f,0.25f,4.11092901f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,40f,107f,20f,0f,0.25f,6.88907528f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,1f,50f,110f,20f,0f,0.25f,11.3338881f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(6f,2f,20f,8f,10f,50f,12f,0f,0f,0f,0f),Item.Make(1f,2f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(1f,2f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0.699999988f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(6f,3f,20f,8f,10f,50f,12f,0f,0f,0f,0f),Item.Make(1f,3f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(1f,3f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,1f,0f,0f,0f),Item.Make(8f,4f,0f,75f,1f,200f),Item.Make(1f,4f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,4f,20f,8f,10f,50f,2f,0f,0f,0f,0f),Item.Make(1f,4f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,4f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,5f,0f,100f,1f,200f),Item.Make(1f,5f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,5f,20f,8f,10f,50f,-12f,0f,0f,0f,0f),Item.Make(1f,5f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,5f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,5f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,6f,0f,125f,1f,200f),Item.Make(1f,6f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,6f,20f,8f,10f,50f,12f,0f,1f,0f,0f),Item.Make(1f,6f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,6f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(8f,7f,0f,150f,1f,200f),Item.Make(1f,7f,20f,3f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(6f,7f,20f,8f,10f,50f,12f,0f,0f,0f,1f),Item.Make(1f,7f,30f,5f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,7f,40f,7f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f),Item.Make(1f,7f,50f,10f,10f,0f,0.25f,0f,0f,0f,0f,0f,0f,0f,0f)}
````

Line 878

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,11f,0f,0.5f,0f,1f,0f,4f),Item.Make(5f,1f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,1f,40f,11f,22f,0f,0.5f,0f,1f,0f,4.17999983f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,21f,0f,0.5f,0f,1f,0f,6f),Item.Make(5f,2f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,2f,40f,11f,12f,0f,0.5f,0f,1f,0f,6f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,21f,0f,0.5f,0f,1f,0f,19f),Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,3f,40f,11f,12f,0f,0.5f,0f,1f,0f,19f)}
````

Line 881

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,11f,0f,0.5f,0f,1f,0f,4f),Item.Make(5f,1f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,1f,40f,11f,22f,0f,0.5f,0f,1f,0f,4.17999983f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,11f,0f,0.5f,0f,1f,0f,6f),Item.Make(5f,2f,20f,13f,14.5f,40f,2.04999995f,-1f),Item.Make(1f,2f,40f,11f,22f,0f,0.5f,0f,1f,0f,6f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,21f,0f,0.5f,0f,1f,0f,19f),Item.Make(5f,3f,20f,6f,13f,40f,2.5999999f,-1f),Item.Make(1f,3f,40f,11f,12f,0f,0.5f,0f,1f,0f,19f)}
````

Line 884

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,20f,4f,11f,0f,0.5f,0f,1f,0f,4f),Item.Make(5f,1f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,1f,40f,11f,22f,0f,0.5f,0f,1f,0f,4.17999983f),Item.Make(8f,2f,0f,25f,1f,200f),Item.Make(1f,2f,20f,4f,11f,0f,0.5f,0f,1f,0f,6f),Item.Make(5f,2f,20f,13f,14.5f,40f,2.04999995f,-1f),Item.Make(1f,2f,40f,11f,22f,0f,0.5f,0f,1f,0f,6f),Item.Make(8f,3f,0f,50f,1f,200f),Item.Make(1f,3f,20f,4f,11f,0f,0.5f,0f,1f,0f,19f),Item.Make(5f,3f,20f,13f,13f,40f,1.89999998f,-1f),Item.Make(1f,3f,40f,11f,22f,0f,0.5f,0f,1f,0f,19.1800003f)}
````

Line 900

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,103f,11f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1f,0f,0.5f,0f,0f,0f,0f),Item.Make(1f,2f,30f,105f,1f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,3f,11f,0f,0.5f,2f,1f,0f,4f),Item.Make(1f,3f,30f,4f,1f,0f,0.5f,0f,0f,0f,4f),Item.Make(1f,3f,30f,5f,1f,0f,0.5f,0f,0f,0f,4f)}
````

Line 903

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,105f,21.2830009f,0f,0.5f,4f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,2f,30f,103f,1.76300001f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,3f,11f,0f,0.5f,2f,1f,0f,4f),Item.Make(1f,3f,30f,4f,1f,0f,0.5f,0f,0f,0f,4f),Item.Make(1f,3f,30f,5f,1f,0f,0.5f,0f,0f,0f,4f)}
````

Line 906

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,105f,21.2830009f,0f,0.5f,4f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,2f,30f,103f,1.76300001f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,5f,21.2830009f,0f,0.5f,2f,1f,0f,4.5f),Item.Make(1f,3f,30f,4f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,3f,30f,3f,1.76300001f,0f,0.5f,0f,0f,0f,0f)}
````

Line 909

````csharp
new[]{Item.Make(8f,1f,0f,0f,1f,200f),Item.Make(1f,1f,30f,5f,121.282997f,0f,0.5f,2f,1f,0f,0f),Item.Make(1f,1f,30f,4f,101.239998f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,1f,30f,3f,101.763f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,2f,0f,40f,1f,200f),Item.Make(1f,2f,30f,103f,11.4799995f,0f,0.5f,4f,1f,0f,0f),Item.Make(1f,2f,30f,104f,1.24000001f,0f,0.5f,0f,0f,10f,0f),Item.Make(1f,2f,30f,105f,1f,0f,0.5f,0f,0f,0f,0f),Item.Make(8f,3f,0f,80f,1f,200f),Item.Make(1f,3f,30f,5f,21.2830009f,0f,0.5f,2f,1f,0f,4.5f),Item.Make(1f,3f,30f,4f,1.24000001f,0f,0.5f,0f,0f,20f,0f),Item.Make(1f,3f,30f,3f,1.76300001f,0f,0.5f,0f,0f,0f,0f)}
````

## MainRipple.cs

[Workspace source](../MainRipple.cs)

Line 6

````csharp
prompts={"How many staves in a system? (B=backup) ->","Type staff number of bottom of source system. ->","Type staff number of bottom of destination system. ->","How many bars to move? ->"}
````

## MainSeparateParts.cs

[Workspace source](../MainSeparateParts.cs)

Line 64

````csharp
new float[]{staff,0,220,0,staff}
````

## MainStemSpacing.cs

[Workspace source](../MainStemSpacing.cs)

Line 20

````csharp
new[]{' ',',','	'}
````

Line 90

````csharp
new[]{false,true}
````

Line 99

````csharp
new[]{19.999998092651367f,38.017547607421875f,61.982444763183594f,79.99987030029297f}
````

Line 100

````csharp
new[]{20.00000762939453f,41.803672790527344f,58.19634246826172f,80.0001220703125f}
````

## MainTransposition.cs

[Workspace source](../MainTransposition.cs)

Line 21

````csharp
new[]{' ',',','\t'}
````

Line 119

````csharp
new[]{transposeStaff,-10f,200f,0f,transposeStaff}
````

## MainVerticalJustification.cs

[Workspace source](../MainVerticalJustification.cs)

Line 21

````csharp
new short[]{0,0,px,x,0}
````

Line 21

````csharp
new short[]{0,py,y,0,0}
````

## MidiInputLine.cs

[Workspace source](../MidiInputLine.cs)

Line 8

````csharp
RhythmTokens={
   "W","H","H.","Q","Q.","E","E.","S","32","S.","64","G",
   "TW","TH","TH.","TQ","TQ.","TE","TE.","TS","48","TS.","96"
  }
````

Line 13

````csharp
KeyPitchSpellings={
   new[]{"CN","DF","EFF","EF","FF","FN","GF","GN","AF","BFF","BF","CF"},
   new[]{"CN","DF","DN","EF","FF","F","GF","GN","AF","BFF","BF","CF"},
   new[]{"C","DF","DN","EF","FF","F","GF","GN","AF","AN","BF","CF"},
   new[]{"C","DF","DN","EF","EN","F","GF","G","AF","AN","BF","CF"},
   new[]{"C","DF","D","EF","EN","F","GF","G","AF","AN","BF","BN"},
   new[]{"C","DF","D","EF","EN","F","FS","G","AF","A","BF","BN"},
   new[]{"C","CS","D","EF","E","F","FS","G","AF","A","BF","BN"},
   new[]{"C","CS","D","EF","E","F","FS","G","GS","A","BF","B"},
   new[]{"C","CS","D","DS","E","FN","FS","G","GS","A","BF","B"},
   new[]{"CN","CS","D","DS","E","FN","FS","G","GS","A","AS","B"},
   new[]{"CN","CS","D","DS","E","ES","FS","GN","GS","A","AS","B"},
   new[]{"BS","CS","DN","DS","E","ES","FS","GN","GS","A","AS","B"},
   new[]{"BS","CS","DN","DS","E","ES","FS","FSS","GS","AN","AS","B"},
   new[]{"BS","CS","CSS","DS","EN","ES","FS","FSS","GS","AN","AS","B"},
   new[]{"BS","CS","CSS","DS","EN","ES","FS","FSS","GS","GSS","AS","BN"},
  }
````

Line 14

````csharp
new[]{"CN","DF","EFF","EF","FF","FN","GF","GN","AF","BFF","BF","CF"}
````

Line 15

````csharp
new[]{"CN","DF","DN","EF","FF","F","GF","GN","AF","BFF","BF","CF"}
````

Line 16

````csharp
new[]{"C","DF","DN","EF","FF","F","GF","GN","AF","AN","BF","CF"}
````

Line 17

````csharp
new[]{"C","DF","DN","EF","EN","F","GF","G","AF","AN","BF","CF"}
````

Line 18

````csharp
new[]{"C","DF","D","EF","EN","F","GF","G","AF","AN","BF","BN"}
````

Line 19

````csharp
new[]{"C","DF","D","EF","EN","F","FS","G","AF","A","BF","BN"}
````

Line 20

````csharp
new[]{"C","CS","D","EF","E","F","FS","G","AF","A","BF","BN"}
````

Line 21

````csharp
new[]{"C","CS","D","EF","E","F","FS","G","GS","A","BF","B"}
````

Line 22

````csharp
new[]{"C","CS","D","DS","E","FN","FS","G","GS","A","BF","B"}
````

Line 23

````csharp
new[]{"CN","CS","D","DS","E","FN","FS","G","GS","A","AS","B"}
````

Line 24

````csharp
new[]{"CN","CS","D","DS","E","ES","FS","GN","GS","A","AS","B"}
````

Line 25

````csharp
new[]{"BS","CS","DN","DS","E","ES","FS","GN","GS","A","AS","B"}
````

Line 26

````csharp
new[]{"BS","CS","DN","DS","E","ES","FS","FSS","GS","AN","AS","B"}
````

Line 27

````csharp
new[]{"BS","CS","CSS","DS","EN","ES","FS","FSS","GS","AN","AS","B"}
````

Line 28

````csharp
new[]{"BS","CS","CSS","DS","EN","ES","FS","FSS","GS","GSS","AS","BN"}
````

Line 34

````csharp
FlatRoute={None,Relative,None,Relative,Up,None,Relative,None,Relative,None,Relative,Up}
````

Line 35

````csharp
SharpRoute={Down,Relative,None,Relative,None,Down,Relative,None,Relative,None,Relative,None}
````

Line 36

````csharp
DoubleFlatRoute={Up,None,Up,None,None,Up,None,Up,None,Up,None,None}
````

Line 37

````csharp
DoubleSharpRoute={None,None,Down,None,Down,None,None,Down,None,Down,None,Down}
````

Line 38

````csharp
MidiKeySignatures={null,"K5F","K2S","K3F","K4S","K1F","K6S","K1S","K4F","K3S","K2F","K5S"}
````

Line 39

````csharp
MidiControlSelectors={null,"K5F","K2S","K3F","TE","AL","TE","BA","TE","TR","K2F"}
````

Line 187

````csharp
new[]{0,2,4,5,7,9,11}
````

## NativeAbsoluteSize.cs

[Workspace source](../NativeAbsoluteSize.cs)

Line 15

````csharp
new[]{17f,wanted}
````

Line 25

````csharp
new[]{(float)parameter,value,7f,height}
````

Line 26

````csharp
new[]{(float)parameter,value}
````

## NativeAppearancePreferences.cs

[Workspace source](../NativeAppearancePreferences.cs)

Line 5

````csharp
Rgb={0x000000,0x0000aa,0x00aa00,0x00aaaa,0xaa0000,0xaa00aa,0xaa5500,0xaaaaaa,0x555555,0x5555ff,0x55ff55,0x55ffff,0xff5555,0xff55ff,0xffff55,0xffffff}
````

Line 8

````csharp
new[]{' ','\t'}
````

Line 21

````csharp
new[]{new[]{Background.R/255f-r,0f,0f,0f,0f},new[]{0f,Background.G/255f-g,0f,0f,0f},new[]{0f,0f,Background.B/255f-b,0f,0f},new[]{0f,0f,0f,1f,0f},new[]{r,g,b,0f,1f}}
````

Line 21

````csharp
new[]{Background.R/255f-r,0f,0f,0f,0f}
````

Line 21

````csharp
new[]{0f,Background.G/255f-g,0f,0f,0f}
````

Line 21

````csharp
new[]{0f,0f,Background.B/255f-b,0f,0f}
````

Line 21

````csharp
new[]{0f,0f,0f,1f,0f}
````

Line 21

````csharp
new[]{r,g,b,0f,1f}
````

## NativeBeamDisplay.cs

[Workspace source](../NativeBeamDisplay.cs)

Line 42

````csharp
new[]{points[2],points[1],points[0],points[3]}
````

## NativeBeamTemporaryEdits.cs

[Workspace source](../NativeBeamTemporaryEdits.cs)

Line 15

````csharp
new[]{9f,(float)(primary*100+secondary*10+tertiary)}
````

Line 37

````csharp
new[]{7f,resultMain,10f,first,11f,left,12f,right,13f,second,14f,secondLeft,15f,secondRight}
````

Line 43

````csharp
new[]{7f,resultMain,10f,first,11f,left,12f,right,13f,second,14f,secondLeft,15f,secondRight}
````

Line 45

````csharp
new[]{7f,main,10f,descriptor,11f,-1f,12f,groups[0]==1?0:xs[one-1],13f,descriptor,14f,groups[1]==1?0:xs[one],15f,groups.Count==2?-1:xs[two-1]}
````

## NativeCode9Terms.cs

[Workspace source](../NativeCode9Terms.cs)

Line 6

````csharp
new[]{"CODE9.DAT","GUITAR.DAT"}
````

## NativeCommandColors.cs

[Workspace source](../NativeCommandColors.cs)

Line 4

````csharp
new[]{' ','\t'}
````

Line 4

````csharp
new[]{Int32.Parse(words[0]),Int32.Parse(words[1]),Int32.Parse(words[2]),words.Length>3?Int32.Parse(words[3]):9}
````

Line 6

````csharp
new[]{' ','\t'}
````

## NativeCommandDialogs.cs

[Workspace source](../NativeCommandDialogs.cs)

Line 55

````csharp
new Control[]{count,ok,cancel}
````

Line 78

````csharp
new[]{'\n'}
````

Line 83

````csharp
new[]{path}
````

## NativeCommandFiles.cs

[Workspace source](../NativeCommandFiles.cs)

Line 4

````csharp
new[]{' ','\t'}
````

## NativeContextShortcuts.cs

[Workspace source](../NativeContextShortcuts.cs)

Line 111

````csharp
new Control[]{capture,assign,clear,overrides,status}
````

## NativeDatClefs.cs

[Workspace source](../NativeDatClefs.cs)

Line 22

````csharp
shifts={.3499999940395355f,8.75f,5.599999904632568f,5.599999904632568f,5.599999904632568f,.3499999940395355f}
````

## NativeDatGlyphs.cs

[Workspace source](../NativeDatGlyphs.cs)

Line 40

````csharp
new[]{horizontal,vertical,staff,x,y}
````

Line 139

````csharp
dotOffsets={Single.NaN,19,19,19,13,15.5f,18.399999618530273f,22.299999237060547f,26.200000762939453f}
````

## NativeDatKeys.cs

[Workspace source](../NativeDatKeys.cs)

Line 10

````csharp
new[]{8,9,10,11,12,13,16}
````

Line 13

````csharp
flats={49,35,42,56}
````

Line 13

````csharp
sharps={77,63,70,35}
````

## NativeDatLines.cs

[Workspace source](../NativeDatLines.cs)

Line 9

````csharp
WaveX={B(0xC1120000u),B(0x00000000u),B(0x4184B852u),B(0x4204B958u),B(0x42471687u),B(0x4284B9DBu),B(0x42A5E873u),B(0x42C71687u),B(0x42E845A2u),B(0x431742D1u),B(0x4382A168u),B(0x431742D1u),B(0x4382A168u),B(0x431742D1u)}
````

Line 10

````csharp
WaveY={B(0x00000000u),B(0x00000000u),B(0x411AD917u),B(0x41E845A2u),B(0x421AD917u),B(0x421AD917u),B(0x41E845A2u),B(0x411AD917u),B(0x00000000u),B(0x00000000u),B(0x00000000u),B(0x42680000u),B(0x00000000u),B(0xC2680000u)}
````

Line 88

````csharp
new[]{10,11,14,16,17,18,19,20}
````

Line 110

````csharp
new[]{i.P(3),i.P(4),i.P(5),i.P(6),i.P(8),i.P(9),p.Scale}
````

## NativeDatOrnaments.cs

[Workspace source](../NativeDatOrnaments.cs)

Line 12

````csharp
new[]{10,14,16,17,18,19,20}
````

Line 22

````csharp
new[]{14,16,17,18,19,20}
````

Line 29

````csharp
new[]{14,16,17,18,19,20}
````

Line 37

````csharp
new[]{10,13,14,15,16,17,18,19,20}
````

Line 47

````csharp
new[]{6,13,14,16,17,18,19,20}
````

Line 57

````csharp
digits={(short)(int)i.P(8)/100,((short)(int)i.P(8)%100)/10,(short)(int)i.P(8)%10,(short)(int)i.P(9)/1000,((short)(int)i.P(9)%1000)/100,((short)(int)i.P(9)%100)/10,(short)(int)i.P(9)%10}
````

Line 59

````csharp
new[]{first,second,boxes}
````

## NativeDatRestAttachments.cs

[Workspace source](../NativeDatRestAttachments.cs)

Line 43

````csharp
faces={2,98,90,92,0,94,91,93,1,1,3,0}
````

## NativeDatTextControls.cs

[Workspace source](../NativeDatTextControls.cs)

Line 8

````csharp
values={137,163,80,81,82,164,141,142,143,144,145,146,147,148,149,162,150,151,152,153,154,155,156,157,158,159,160,161,166}
````

## NativeDiskDialogChoice.cs

[Workspace source](../NativeDiskDialogChoice.cs)

Line 17

````csharp
new[]{1,2,3,4,5,6,7,8}
````

## NativeDosEditCursor.cs

[Workspace source](../NativeDosEditCursor.cs)

Line 8

````csharp
Ink={0,0x10,0x38,0x7c,0xfe,0x38,0x38,0x38,0x38,0}
````

Line 9

````csharp
Mask={0xffef,0xff83,0xff01,0xff00,0xff00,0xff00,0xff01,0xff01,0xff01,0xff01}
````

## NativeDosViewport.cs

[Workspace source](../NativeDosViewport.cs)

Line 127

````csharp
new[]{' ','\t',','}
````

Line 153

````csharp
new[]{' ','\t',','}
````

Line 173

````csharp
new short[]{50,25,10,5}
````

## NativeDrawDialog.cs

[Workspace source](../NativeDrawDialog.cs)

Line 58

````csharp
new[]{"X","Y","Operation"}
````

Line 71

````csharp
new ToolStripItem[]{file,edit,construct,view}
````

Line 133

````csharp
new Control[]{BottomField("Item",itemName),plot,BottomField("Operation",operation),count,
    BottomField("Fill step (px)",fillStep),BottomField("Staff scale",gridScale,showGrid),
    BottomField("Spacing",gridSpacing,showSquareGrid),snapToGrid,showPoints,showOverlay,BottomField("Zoom",zoomControl)}
````

Line 340

````csharp
new Control[]{name,rename}
````

Line 381

````csharp
colourNames={"Background","Border","Origin axes","Staff grid","Outline","Overlay","Construction points","Fill","Square grid","Editable points","Selected point"}
````

Line 382

````csharp
defaultColours={Color.White,SystemColors.WindowFrame,Color.Gainsboro,Color.Gray,Color.FromArgb(30,32,37),Color.FromArgb(30,32,37),Color.DimGray,Color.Black,Color.FromArgb(220,230,240),Color.FromArgb(52,104,165),Color.FromArgb(210,85,35)}
````

Line 533

````csharp
new Control[]{new Label{Text="X",AutoSize=true},x,new Label{Text="Y",AutoSize=true},y,
     new Label{Text="Operation",AutoSize=true},z,relative,accept}
````

Line 578

````csharp
new Control[]{new Label{Text="X",AutoSize=true},x,new Label{Text="Y",AutoSize=true},y,apply}
````

Line 633

````csharp
new Control[]{new Label{Text="Degrees",AutoSize=true},angle,apply}
````

Line 652

````csharp
new Control[]{new Label{Text="Points per segment",AutoSize=true},divisions,fit}
````

Line 686

````csharp
new Control[]{new Label{Text="Thickness",AutoSize=true},width,
     new Label{Text="Taper fraction",AutoSize=true},taper,apply}
````

Line 700

````csharp
labels={"First degree","Last degree","First point X","First point Y","Radius X","Radius Y","Degree increment"}
````

Line 701

````csharp
defaults={0,0,0,0,50,0,3}
````

## NativeDrawDocument.cs

[Workspace source](../NativeDrawDocument.cs)

Line 61

````csharp
new[]{line.Substring(0,5),line.Substring(5,5),line.Substring(10,5)}
````

Line 114

````csharp
new[]{glyph.Points.Length}
````

## NativeDrawVectorExport.cs

[Workspace source](../NativeDrawVectorExport.cs)

Line 6

````csharp
new[]{' ','\t'}
````

## NativeEditCursorPosition.cs

[Workspace source](../NativeEditCursorPosition.cs)

Line 7

````csharp
TypeOffsets={1,-1,0,0,-1,0,0,-1,2,-1,-1,-1,-1,-1,0,-1,-1,4,3}
````

Line 8

````csharp
RestOffsets={5,5,7,8,7,4,4,2,2,1,1,1,1}
````

## NativeEditFlip.cs

[Workspace source](../NativeEditFlip.cs)

Line 7

````csharp
new[]{500f,control<20?10f:-10f}
````

Line 8

````csharp
new[]{7f,-item.P(7)}
````

Line 9

````csharp
new[]{700f,delta}
````

Line 10

````csharp
new[]{7f,-item.P(7)}
````

Line 11

````csharp
new[]{7f,-value}
````

## NativeEmbeddedTitle.cs

[Workspace source](../NativeEmbeddedTitle.cs)

Line 5

````csharp
new string[]{
"",
"",
"                                 SCORE (tm)",
"",
"                      Computer Music Typography System",
"",
"                                   from",
"                             San Andreas Press",
"                       Box 60247, Palo Alto, CA, 94306",
"",
"                             Version: 3.11     ",
"",
"                          Written by Leland Smith",
"",
"                  Programmers: Leland Smith, Perry Devine",
"                    Copyright 1977-1994 by Leland Smith",
"                Copyright 1986-1991 by Passport Designs, Inc.",
"                            All Rights Reserved.",
"",
"",
"   Program limits: 1420 items, 14200 parameters, 31000 graphics vectors.",
"",
"",
"",
"**** reading SCORE data ****",
}
````

Line 37

````csharp
new[]{
     "",
     "",
     "",
     "                                 SCORE (tm)",
     "",
     "                      Computer Music Typography System",
     "",
     "                                   from",
     "                             San Andreas Press",
     "                       Box 60247, Palo Alto, CA, 94306",
     "",
     "                               Version: 4.01",
     "",
     "                          Written by Leland Smith",
     "",
     "                    Copyright 1977-2001 by Leland Smith",
     "                            All Rights Reserved.",
     "",
     "",
     "   Program limits: 1420 items, 14200 parameters, 31000 graphics vectors.",
     "",
     "",
     "",
     "**** reading SCORE data ****",
     "",
    }
````

## NativeEpsExport.cs

[Workspace source](../NativeEpsExport.cs)

Line 63

````csharp
new float[]{3,1}
````

Line 63

````csharp
new float[]{1,1}
````

Line 63

````csharp
new float[]{3,1,1,1}
````

Line 63

````csharp
new float[]{3,1,1,1,1,1}
````

## NativeF3Menus.cs

[Workspace source](../NativeF3Menus.cs)

Line 5

````csharp
EditGroupCommands={"M","C","A","DE","J","LI","FLO","TR"}
````

## NativeFileAssociations.cs

[Workspace source](../NativeFileAssociations.cs)

Line 25

````csharp
new[]{".exe",".com",".bat",".cmd",".msi",".ps1",".vbs",".js",".lnk",".scr",".dll"}
````

Line 121

````csharp
new Control[]{apply,defaults,close}
````

## NativeFirstSetup.cs

[Workspace source](../NativeFirstSetup.cs)

Line 29

````csharp
new[]{"Startup folder","Library (LIB)","Type 1 fonts"}
````

Line 36

````csharp
new[]{intro,notice}
````

## NativeFontAllocationCache.cs

[Workspace source](../NativeFontAllocationCache.cs)

Line 11

````csharp
new[]{folder}
````

## NativeGraphicalMenus.cs

[Workspace source](../NativeGraphicalMenus.cs)

Line 4

````csharp
new[]{' ','\t'}
````

## NativeGuitarBend.cs

[Workspace source](../NativeGuitarBend.cs)

Line 32

````csharp
new[]{Point(tip,y,2),Point(end,F((double)y+half),0),Point(end,F((double)y-half),0),Point(tip,y,0)}
````

Line 72

````csharp
new[]{bend.Body,bend.Arrow}
````

## NativeHairpins.cs

[Workspace source](../NativeHairpins.cs)

Line 10

````csharp
new[]{8,11,12,19,20}
````

## NativeInformationNotice.cs

[Workspace source](../NativeInformationNotice.cs)

Line 5

````csharp
new[]{'\r','\n'}
````

Line 6

````csharp
new[]{'\r','\n'}
````

Line 6

````csharp
new[]{key}
````

## NativeInputHighlight.cs

[Workspace source](../NativeInputHighlight.cs)

Line 211

````csharp
new[]{
    "Staff # = "+inputSetup.Staff.ToString(culture),
    "Left End = "+inputSetup.LeftEnd.ToString(culture),
    "Vert. Pos = "+inputSetup.Vertical.ToString(culture),
    "Size = "+inputSetup.Size.ToString("0.00",culture),
    "Right End = "+inputSetup.RightEnd.ToString(culture)
   }
````

Line 217

````csharp
new[]{"   ","   ","   ","   "}
````

Line 218

````csharp
new[]{
    "Spacing = "+spacing,
    "Left Pos = "+inputSetup.LeftPosition.ToString(culture),
    "Right Pos = "+inputSetup.RightPosition.ToString(culture),
    "Meter = "+inputSetup.MeterTop.ToString(culture),
    inputSetup.MeterBottom.ToString(culture),
    "Key Sig. = "+inputSetup.Key
   }
````

Line 225

````csharp
new[]{"   ","   ","   ","/","   "}
````

## NativeInputMotives.cs

[Workspace source](../NativeInputMotives.cs)

Line 4

````csharp
new[]{' ','\t'}
````

Line 6

````csharp
new[]{lines.ToArray()}
````

## NativeInputShell.cs

[Workspace source](../NativeInputShell.cs)

Line 18

````csharp
setupLabels={"TREBLE","BASS","TENOR","ALTO","PIANO","VOCAL","QUART","","",""}
````

Line 19

````csharp
scor4TripletRhythms={"1.5","3","6","12","24","48","96"}
````

Line 20

````csharp
scor4DupleRhythms={"1","2","4","8","16","32","64"}
````

Line 120

````csharp
keys={"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#"}
````

Line 320

````csharp
new[]{' ','\t'}
````

Line 355

````csharp
new float[]{8,destination.Staff,destination.LeftEnd,destination.Vertical,destination.Size,destination.RightEnd}
````

Line 455

````csharp
labels={"Input= ","Pos.1, pos.2 >","Notes,etc. >","Rhythm >","Marks  >","Beams  >","Slurs >"}
````

Line 498

````csharp
pitch={"","r","m","su","sd","so","p","o"}
````

Line 498

````csharp
marks={"","p","mp","mf","f","w","ac","ri","o","ob"}
````

Line 527

````csharp
rhythm={"w","h","q","e","s","32","64"}
````

Line 633

````csharp
new[]{"N","S","F",""}
````

Line 753

````csharp
new[]{originalNotationStaff,retained.OriginalSelectionLeft,retained.OriginalSelectionRight,0,originalNotationStaff}
````

Line 755

````csharp
new[]{"HELP","INPUT","EDIT","TEXT","PLAY","PRINT","DISK","EXIT"}
````

Line 759

````csharp
new[]{"HELP","WHOLE","HALF","QUARTER","EIGHT","1/16","1/32","1/64"}
````

Line 763

````csharp
new[]{"HELP","REST","BAR","STM UP","STM DN","STM ORD","PROX ON","PROX OFF"}
````

Line 763

````csharp
new[]{"HELP","WHOLE","HALF","QUARTER","EIGHT","1/16","1/32","1/64"}
````

Line 763

````csharp
new[]{"HELP","p","mp","mf","f","WEDGE","accel","rit","8va","8vb"}
````

Line 763

````csharp
new[]{"HELP","2B","3B","4B","5B","6B","7B","8B","9B","AUTO"}
````

Line 763

````csharp
new[]{"HELP","****","****","****","****","****","****","****"}
````

Line 765

````csharp
prefixes={"Type notes,etc>","Enter "+musicInput.Transaction.RhythmCount.ToString().PadLeft(2)+" rhythms ->","Type marks   ->","Type beams   ->","Type slurs   ->"}
````

## NativeJustDialog.cs

[Workspace source](../NativeJustDialog.cs)

Line 33

````csharp
new object[]{"Line up (L)","Height (H)","Justify (J)","Line up and justify (JL)",
    "Justify and height (JH)","Line up and height (LH)","Line up, justify and height (JLH)"}
````

Line 42

````csharp
new Control[]{customLimits,new Label{Text="Left",AutoSize=true},left,new Label{Text="Right",AutoSize=true},right}
````

Line 47

````csharp
new Control[]{choose,numbered,operation,heightLabel,height,layout,close}
````

Line 83

````csharp
new Control[]{new Label{Text="Number of files",AutoSize=true},count,open}
````

Line 134

````csharp
new[]{"Saved "+written+" of "+results.Length+" files."}
````

## NativeJustLayout.cs

[Workspace source](../NativeJustLayout.cs)

Line 26

````csharp
new[]{source}
````

## NativeKeyboardOptions.cs

[Workspace source](../NativeKeyboardOptions.cs)

Line 5

````csharp
new[]{4,5,10}
````

Line 5

````csharp
new[]{5,10}
````

Line 22

````csharp
new[]{4,5,10}
````

Line 23

````csharp
new[]{10,5}
````

## NativeKeySignatures.cs

[Workspace source](../NativeKeySignatures.cs)

Line 16

````csharp
new[]{8,9,10,11,12,13,16}
````

Line 29

````csharp
flats={49,35,42,56}
````

Line 29

````csharp
sharps={77,63,70,35}
````

## NativeLetterHelp.cs

[Workspace source](../NativeLetterHelp.cs)

Line 35

````csharp
new[]{Keys.C,Keys.V,Keys.X,Keys.A}
````

## NativeLetterMusicInput.cs

[Workspace source](../NativeLetterMusicInput.cs)

Line 5

````csharp
new[]{left,vertical,size,right}
````

## NativeLibraryLocation.cs

[Workspace source](../NativeLibraryLocation.cs)

Line 54

````csharp
new[]{"\r\n","\n"}
````

## NativeLibrarySymbolBrowser.cs

[Workspace source](../NativeLibrarySymbolBrowser.cs)

Line 34

````csharp
new[]{Path.Combine(root,"LIB"),directory}
````

Line 101

````csharp
new[]{'\r','\n'}
````

## NativeLibrarySymbols.cs

[Workspace source](../NativeLibrarySymbols.cs)

Line 5

````csharp
AliasIds={66,67,78,45,46,47,63,64,65,68,72,73,79,80,81,85,86,111,113,114}
````

Line 6

````csharp
AliasText={"pizz.","arco","a tempo","8va","8ba","15ma","cresc.","dim.","rit.","accel.","gliss.","molto","espr.","meno","pi>>u","subito","poco","decresc.","rall.","mosso"}
````

Line 22

````csharp
new[]{12,14,17,18,19,20}
````

Line 118

````csharp
new[]{14*count,-14*count}
````

## NativeModernHelp.cs

[Workspace source](../NativeModernHelp.cs)

Line 58

````csharp
WindowsTopics={"START","SETUP","WORKFLOW","VIEW","COMPAT","RESOURCES"}
````

## NativeMousePosition.cs

[Workspace source](../NativeMousePosition.cs)

Line 4

````csharp
offsets={1,-1,0,0,-1,0,0,-1,2,-1,-1,-1,-1,-1,0,-1,-1,4,3}
````

Line 5

````csharp
rests={5,5,7,8,7,4,4,2,2,1,1,1,1}
````

## NativeMouseSelector.cs

[Workspace source](../NativeMouseSelector.cs)

Line 6

````csharp
offsets={1,-1,0,0,-1,0,0,-1,2,-1,-1,-1,-1,-1,0,-1,-1,4,3}
````

Line 7

````csharp
rests={5,5,7,8,7,4,4,2,2,1,1,1,1}
````

Line 59

````csharp
new ToolStripItem[]{original,high,points,unlimited}
````

## NativeNoteAttachments.cs

[Workspace source](../NativeNoteAttachments.cs)

Line 7

````csharp
Category={0,1,1,1,-1,0,0,0,-1,-1,-1,1,1,1,1,1,1,1,1,-1,1,1,1,1,1,-1}
````

## NativeNoteCentering.cs

[Workspace source](../NativeNoteCentering.cs)

Line 25

````csharp
new[]{3f,start,6f,end}
````

Line 108

````csharp
new[]{7f,curve}
````

Line 127

````csharp
new[]{3f,leftX,6f,rightX}
````

## NativeNumberDisplay.cs

[Workspace source](../NativeNumberDisplay.cs)

Line 11

````csharp
Faces={0,2,98,90,92,0,94,91,93,1,1,3,0}
````

Line 72

````csharp
new[]{new BarlinePoint(left,bottom,1),new BarlinePoint(right,bottom,0),new BarlinePoint(right,top,0),
    new BarlinePoint(left,top,0),new BarlinePoint(left,bottom,0),new BarlinePoint(right,bottom,0)}
````

## NativeOriginalGroupFields.cs

[Workspace source](../NativeOriginalGroupFields.cs)

Line 4

````csharp
originalGroupFields={1,0,200,0,0}
````

Line 8

````csharp
names={"Base Staff","Left Pos","Right Pos","Code Num","Top Staff"}
````

Line 9

````csharp
min={1,-99,-99,0,0}
````

Line 9

````csharp
max={99,216,218,18,32}
````

## NativeOriginalPrintMenu.cs

[Workspace source](../NativeOriginalPrintMenu.cs)

Line 31

````csharp
new[]{' ','\t'}
````

Line 31

````csharp
new[]{' ','\t'}
````

## NativeOriginalTextInput.cs

[Workspace source](../NativeOriginalTextInput.cs)

Line 19

````csharp
new[]{1f,0f,0f,1f,1f,0f,0f,0f,0f,0f}
````

## NativeOriginalTextSetup.cs

[Workspace source](../NativeOriginalTextSetup.cs)

Line 3

````csharp
originalTextSetup={1,5,-3,1,1,0}
````

Line 21

````csharp
fonts={0,2,1,3,4,5}
````

Line 21

````csharp
controls={"_00","_02","_01","_03","_04","??","!!"}
````

Line 22

````csharp
new[]{originalTextSetup[0],originalTextSetup[1],originalTextSetup[2],originalTextSetup[3],originalTextSetup[4]}
````

Line 23

````csharp
names={"Staff","Horz","Vert","Spacing","H.Size","V.Size"}
````

Line 23

````csharp
new[]{"TIMES","ITALIC","BOLD","BOLD ITAL","HELVT","HELVT BOLD","OTHER",""}
````

Line 23

````csharp
new[]{"TIMES","ITALIC","BOLD","BOLD ITAL","HELVT","ONE DASH","DASHES",""}
````

## NativeOverview.cs

[Workspace source](../NativeOverview.cs)

Line 8

````csharp
xx={l,l,r,r,l}
````

Line 8

````csharp
yy={t,b,b,t,t}
````

Line 12

````csharp
pageX={-1500,31500,31500,-1500,-1500}
````

Line 12

````csharp
pageY={16500,16500,-26500,-26500,16500}
````

## NativePageAutomaticLayout.cs

[Workspace source](../NativePageAutomaticLayout.cs)

Line 24

````csharp
args={0x1388,0x2000,0x2d40,0x1000,0,0x3000,0x3e8,0x2000,0x1b58,0x2000,0,0x4000,0xbb8,0x2000}
````

## NativePageAutomaticMachine.cs

[Workspace source](../NativePageAutomaticMachine.cs)

Line 16

````csharp
b={M8(seg,off),M8(seg,off+1),M8(seg,off+2),M8(seg,off+3)}
````

## NativePageBarPlan.cs

[Workspace source](../NativePageBarPlan.cs)

Line 32

````csharp
new[]{score}
````

## NativePageFileBank.cs

[Workspace source](../NativePageFileBank.cs)

Line 26

````csharp
tail={3,measurement,4,-9999}
````

## NativePageMarkedLayout.cs

[Workspace source](../NativePageMarkedLayout.cs)

Line 37

````csharp
new[]{0,2,4,6,8}
````

Line 74

````csharp
new[]{new[]{count}}
````

Line 74

````csharp
new[]{count}
````

Line 79

````csharp
new[]{file}
````

Line 79

````csharp
new[]{source}
````

Line 89

````csharp
new short[]{1,2}
````

Line 90

````csharp
new short[]{(short)(count/2),(short)((count+1)/2)}
````

Line 127

````csharp
new[]{source}
````

## NativePagePartsDialog.cs

[Workspace source](../NativePagePartsDialog.cs)

Line 23

````csharp
transposeIntervals={0,2,-2,2,7,12,-7,-12,1,-1,2,2,2,2}
````

Line 55

````csharp
new object[]{"Automatic Parts","Layout marked section","Find page turns","Old Page Data: page turns","Layout full score","Old Page Data: full score"}
````

Line 57

````csharp
new object[]{"American","European"}
````

Line 57

````csharp
new object[]{"SCORE sequence","Windows numeric sequence"}
````

Line 61

````csharp
new object[]{"Keep","Treble","Bass","Alto","Tenor"}
````

Line 77

````csharp
new Control[]{Button("Open settings...",OpenSettings),Button("Save settings...",SaveSettings)}
````

Line 81

````csharp
new Control[]{close,save,preview}
````

Line 84

````csharp
new[]{instrument,staves,size,height,firstIndent,otherIndent,firstPage}
````

Line 85

````csharp
new[]{omit,centerNumbers,restLabels}
````

Line 95

````csharp
new[]{manualLines,linesPerPage,fixedProposal}
````

Line 112

````csharp
descriptions={
    "None","B flat (Clar., Trpt., Sop. Sax)|up major 2","D (Clar., Trpt.)|down major 2",
    "Bass clef|up major 2","F (Horn, English Horn)|up perfect 5","|octave up",
    "|down perfect 5","|octave down","|up minor 2","|down minor 2","Normalize stems|up major 2",
    "Treble clef|up major 2","Alto clef|up major 2","Tenor clef|up major 2",
    "A flat (Clar. [high])|down major 6","F (Trpt. [high])|down perfect 4",
    "E (Trpt. [high])|down major 3","E flat (Clar., Trpt. [high])|down minor 3",
    "A (Clar., Trpt.)|up minor 3","G (Flute, Horn)|up perfect 4","E (Horn)|up minor 6",
    "E flat (Alto Sax, Horn)|up major 6","D (Horn)|up minor 7",
    "B flat (Tenor Sax, Bass Clar.)|up major 9","E flat (Baritone Sax)|up major 13"
   }
````

Line 161

````csharp
new Control[]{size,height,centerNumbers,firstPage}
````

Line 164

````csharp
new Control[]{instrument,firstIndent,otherIndent,startMeasure,omit,restStyle,restLabels,transposeWholeTone}
````

Line 190

````csharp
new[]{' ', ',', '\t'}
````

Line 197

````csharp
new[]{' ', ',', '\t'}
````

Line 209

````csharp
new[]{' ', ',', '\t'}
````

Line 212

````csharp
new[]{' ', ',', '\t'}
````

Line 217

````csharp
new short[]{1,2,3,4,7,8,10,11,12,13,14}
````

Line 250

````csharp
new[]{' ','\t','/'}
````

## NativePagePartsWorkflow.cs

[Workspace source](../NativePagePartsWorkflow.cs)

Line 112

````csharp
new[]{' ', '\t', '\r', '\n', ','}
````

Line 124

````csharp
new[]{' ', '\t', '\r', '\n', ','}
````

Line 138

````csharp
new[]{Path.GetFullPath(first)}
````

## NativePageRestFollowup.cs

[Workspace source](../NativePageRestFollowup.cs)

Line 9

````csharp
leftOffsets={0,1.2f,2.2f,4.4f,1,4.4f,3,0}
````

Line 10

````csharp
rightOffsets={0,0,0,0,3.6f,3.4f,4,0}
````

## NativePageScoreLayout.cs

[Workspace source](../NativePageScoreLayout.cs)

Line 71

````csharp
new[]{system}
````

## NativePageTurnPlan.cs

[Workspace source](../NativePageTurnPlan.cs)

Line 54

````csharp
new[]{score}
````

Line 56

````csharp
new[]{source}
````

Line 99

````csharp
new[]{bars}
````

## NativeParameterExport.cs

[Workspace source](../NativeParameterExport.cs)

Line 8

````csharp
places={0,1,3,2,2,2,4}
````

Line 8

````csharp
widths={0,5,8,9,9,9,11}
````

## NativePostScriptBounds.cs

[Workspace source](../NativePostScriptBounds.cs)

Line 33

````csharp
new[]{' ','\r'}
````

## NativePreferences.cs

[Workspace source](../NativePreferences.cs)

Line 16

````csharp
Names={"SCREEN","MIDI","IRQ","KEYDISK","LIBRARY","CODE9","GUITAR","REST","COLOR","BEAM","BMTH","TIES","SLUR","MIDK","METRIC","INCHES","DYNAMICS","DESC","DASH","XDASH","OUTPUT","RESOLUTION","PROCEDURES","SETSTROKE","AFTC","AFTB","AFTK","AFTM","MOUSE"}
````

Line 37

````csharp
new[]{'\r','\n'}
````

Line 52

````csharp
new[]{Program.CheckMode?besideApp:local,besideApp,Path.ChangeExtension(defaults.Path,"MSCR")}
````

Line 65

````csharp
new[]{"\r\n","\n"}
````

Line 74

````csharp
new[]{' ','\t'}
````

## NativePreferencesUi.cs

[Workspace source](../NativePreferencesUi.cs)

Line 86

````csharp
paths={NativeSettings.FilePath,NativePreferences.OriginalSourcePath(root)}
````

Line 202

````csharp
new[]{"LIBRARY","MOUSE","DASH","XDASH"}
````

Line 522

````csharp
new[]{"LIBRARY"}
````

## NativePrintCaptions.cs

[Workspace source](../NativePrintCaptions.cs)

Line 17

````csharp
dividers={211,327,401,450}
````

Line 26

````csharp
values={name,job.Project,created.HasValue?created.Value.ToString("MM-dd-yy",culture):"",
    created.HasValue?created.Value.ToString("HH:mm",culture):"",
    Number(job.Size)+" "+Number(job.Left/4000.0)+" "+Number(job.Bottom/4000.0)+"  "+job.Resolution+" "+job.LineWidth}
````

Line 51

````csharp
labels={"SCORE File:","Project:","File Date:","Time:","Print data:"}
````

Line 52

````csharp
lx={22,213,329,403,453}
````

Line 52

````csharp
vx={69,241,363,425,490}
````

## NativePrintedBarlines.cs

[Workspace source](../NativePrintedBarlines.cs)

Line 7

````csharp
HookX={60,92,122,148,169,192,162,143,117,79,0,0}
````

Line 7

````csharp
HookY={-33,-29,-18,0,24,60,28,15,5,0,0,-33}
````

## NativePrintedChordGrid.cs

[Workspace source](../NativePrintedChordGrid.cs)

Line 15

````csharp
CrossX={-55,45,55,-45,-55}
````

Line 16

````csharp
CrossY={34,135,135,34,34}
````

Line 106

````csharp
kinds={"","maj","m","?0","?9","+","dim","aug","?-","?8"}
````

## NativePrintedFilledWave.cs

[Workspace source](../NativePrintedFilledWave.cs)

Line 5

````csharp
X={13f,7f,-1f,30f,39f,46f,52f,59f,65f,74f,87f,116f,125f,127f,131f,134f,138f,143f,148f,156f,124f,117f,110f,104f,97f,90f,83f,72f,40f,32f,27f,23f,19f,17f,13f}
````

Line 6

````csharp
Y={24f,18.5f,26f,57f,64f,67f,68f,68f,67f,63f,54f,29f,22f,21f,20f,20f,21f,25f,30f,22f,-10f,-16f,-19f,-20f,-20f,-19f,-16f,-8f,21f,27f,29f,29f,28f,27f,24f}
````

Line 78

````csharp
ArrowX={148,152,154,159,201,148,268,148,201,163,158,155,148}
````

Line 79

````csharp
ArrowY={29,32,33,34,34,106,29,-47,25,25,24,22,29}
````

## NativePrintedHeads.cs

[Workspace source](../NativePrintedHeads.cs)

Line 28

````csharp
new[]{new BarlinePoint(plan.StemX,plan.StemStart,1),new BarlinePoint(plan.StemX,plan.StemEnd,0)}
````

## NativePrintedOrnaments.cs

[Workspace source](../NativePrintedOrnaments.cs)

Line 24

````csharp
new[]{10,13,14,15,16,17,18,19,20}
````

Line 33

````csharp
new[]{14,16,17,18,19,20}
````

## NativePrintedRests.cs

[Workspace source](../NativePrintedRests.cs)

Line 95

````csharp
new[]{8,11,12,14,16}
````

Line 118

````csharp
offsets={Single.NaN,19,19,19,13,15.5f,18.399999618530273f,22.299999237060547f,26.200000762939453f}
````

Line 165

````csharp
new[]{new BarlinePoint(F((double)origin-8*scale),y,1),new BarlinePoint(F((double)origin+25*scale),y,0)}
````

## NativePrintedTextControls.cs

[Workspace source](../NativePrintedTextControls.cs)

Line 6

````csharp
PrintedSpecialBytes={5,133,26,154,20,148,183,185,186,161,162,163,167,168,169,170,135,179,155,156,7,178,188,166,171,187,174,172,173,175,208,177,182,191,27,165,28,35,36,37,38,96,6,60,62,64,225,234,232,233,91,92,93,241,29,250,245,248,249,134,251,157,123,124,125,227,235}
````

## NativePrintJob.cs

[Workspace source](../NativePrintJob.cs)

Line 7

````csharp
new[]{' ','\t',','}
````

Line 15

````csharp
new[]{' ','\t'}
````

Line 25

````csharp
new[]{' ','\t',','}
````

Line 112

````csharp
new[]{' '}
````

Line 132

````csharp
new[]{d/determinant,-b/determinant,-c/determinant,a/determinant,
     (c*ty-d*tx)/determinant,(b*tx-a*ty)/determinant}
````

## NativePrintMinimum.cs

[Workspace source](../NativePrintMinimum.cs)

Line 33

````csharp
new[]{Round(left)-1,Round(bottom)-1,Round(right)+1,Round(top)+1}
````

## NativeRawPostScript.cs

[Workspace source](../NativeRawPostScript.cs)

Line 21

````csharp
new[]{'[',']','{','}','|'}
````

## NativeResourceEntry.cs

[Workspace source](../NativeResourceEntry.cs)

Line 5

````csharp
roots={10,12,20,31,30,40,42,50,61,60,71,70}
````

## NativeRestCentering.cs

[Workspace source](../NativeRestCentering.cs)

Line 47

````csharp
new float[]{1.2000000476837158f,2.200000047683716f,4.400000095367432f,1,4.400000095367432f,3}
````

Line 48

````csharp
new float[]{0,0,0,3.5999999046325684f,3.4000000953674316f,4}
````

## NativeRetainedDisplayArray.cs

[Workspace source](../NativeRetainedDisplayArray.cs)

Line 186

````csharp
new[]{1}
````

## NativeScanDynamicVectors.cs

[Workspace source](../NativeScanDynamicVectors.cs)

Line 17

````csharp
new[]{0,1,2,3,0}
````

Line 17

````csharp
new[]{2,1,0,3}
````

## NativeScanTextVectors.cs

[Workspace source](../NativeScanTextVectors.cs)

Line 43

````csharp
codes={95,96,97,98,99,100,103,109,110,111,113,114,115,129,130,131,136,143,144,151,152,153,149,132,133,134,135,145,147,138,139,140,146}
````

Line 43

````csharp
glyphs={40,10,53,23,54,24,78,76,77,10,28,35,40,58,34,65,75,21,24,48,51,54,0,1,2,3,4,5,6,7,8,9,97}
````

## NativeScoreIdentity.cs

[Workspace source](../NativeScoreIdentity.cs)

Line 21

````csharp
new[]{preferred,original}
````

Line 21

````csharp
new[]{"SCOR3_10.DAT","SCOR4.DAT","SCR4_00.DAT"}
````

## NativeScorlasQuickBatchDialog.cs

[Workspace source](../NativeScorlasQuickBatchDialog.cs)

Line 21

````csharp
new object[]{"Page number + part letter (MAT001A, MAT001B, MAT002A)","Consecutive numbers (wandh01, wandh02)","Consecutive final characters (SONAA, SONAB)"}
````

Line 23

````csharp
new object[]{"Number of pages","Number of files"}
````

Line 25

````csharp
new Control[]{cancel,use}
````

## NativeScorlasStatistics.cs

[Workspace source](../NativeScorlasStatistics.cs)

Line 18

````csharp
new[]{'\r','\n'}
````

## NativeScorlasWorkflow.cs

[Workspace source](../NativeScorlasWorkflow.cs)

Line 79

````csharp
new Control[]{pageSelector,quick,readNames,saveNames}
````

Line 80

````csharp
new Control[]{add,remove,up,down}
````

Line 82

````csharp
new object[]{"Filename (natural)","Full path (natural)","Date modified","File size","Reverse current order"}
````

Line 83

````csharp
new object[]{"Ascending","Descending"}
````

Line 89

````csharp
new Control[]{new Label{Text="Sort by",AutoSize=true,Margin=new Padding(8,7,3,3)},sortBy,sortDirection,sort}
````

Line 109

````csharp
new[]{index}
````

## NativeSettings.cs

[Workspace source](../NativeSettings.cs)

Line 20

````csharp
LegacyKeys={
   "beams","dat","display","edit-compatibility","endings","font-choices","fontinit",
   "ghostscript","input-highlight","lib-status","library","mouse","notation","open-folder",
   "output","postscript","preference-paths","slurs","splash","startup-folder","text-cache",
   "type1","window","workspace"
  }
````

## NativeStartupSplash.cs

[Workspace source](../NativeStartupSplash.cs)

Line 11

````csharp
values={result.Items,result.Parameters,result.Vectors}
````

Line 51

````csharp
new float[][]{
      new float[]{-LogoGrey,0,0,0,0},new float[]{0,-LogoGrey,0,0,0},new float[]{0,0,-LogoGrey,0,0},
      new float[]{0,0,0,1,0},new float[]{LogoGrey,LogoGrey,LogoGrey,0,1}}
````

Line 52

````csharp
new float[]{-LogoGrey,0,0,0,0}
````

Line 52

````csharp
new float[]{0,-LogoGrey,0,0,0}
````

Line 52

````csharp
new float[]{0,0,-LogoGrey,0,0}
````

Line 53

````csharp
new float[]{0,0,0,1,0}
````

Line 53

````csharp
new float[]{LogoGrey,LogoGrey,LogoGrey,0,1}
````

Line 54

````csharp
new PointF[]{new PointF(destination.Left,destination.Top),
      new PointF(destination.Right,destination.Top),new PointF(destination.Left,destination.Bottom)}
````

Line 108

````csharp
new[]{NativeInterfaceText.VgaPath,local}
````

## NativeTabEnvironment.cs

[Workspace source](../NativeTabEnvironment.cs)

Line 12

````csharp
new short[]{0,-1,1,3,-2,2,10512,0,unchecked((short)(12250+loadSegment)),74}
````

## NativeTablatureDrawing.cs

[Workspace source](../NativeTablatureDrawing.cs)

Line 17

````csharp
new short[]{(short)(int)i.P(16),(short)(int)i.P(17),(short)(int)i.P(18),i.P(19)>=100?(short)(int)i.P(18):(short)0}
````

## NativeTabPosition.cs

[Workspace source](../NativeTabPosition.cs)

Line 8

````csharp
new[]{8f,-1f}
````

Line 9

````csharp
new[]{8f,(float)requested}
````

Line 16

````csharp
new[]{8f,(float)requested}
````

Line 21

````csharp
new[]{8f,(float)chosen}
````

## NativeTextArrowParameters.cs

[Workspace source](../NativeTextArrowParameters.cs)

Line 8

````csharp
new[]{(float)parameter,value}
````

## NativeTextDashCentering.cs

[Workspace source](../NativeTextDashCentering.cs)

Line 13

````csharp
new[]{start,y,end,length,spacing}
````

Line 18

````csharp
new[]{start,y,end,0f,0f}
````

Line 30

````csharp
new[]{14f,count}
````

## NativeTextDisplay.cs

[Workspace source](../NativeTextDisplay.cs)

Line 34

````csharp
new[]{"vga",VgaPath}
````

Line 43

````csharp
new[]{new CharacterRange(start,length)}
````

Line 71

````csharp
new[]{glyph.Character}
````

## NativeTextFormattingCommands.cs

[Workspace source](../NativeTextFormattingCommands.cs)

Line 8

````csharp
new[]{"RMJ","RM","RIPX","RIP","LSI","LSC","LSP","LS"}
````

Line 18

````csharp
new[]{6f,value,7f,0f}
````

Line 24

````csharp
new[]{3f,left,10f,extra}
````

Line 26

````csharp
new[]{3f,position,11f,0f}
````

Line 26

````csharp
new[]{3f,position}
````

Line 28

````csharp
new[]{3f,origin,11f,0f}
````

Line 28

````csharp
new[]{3f,origin}
````

Line 32

````csharp
new[]{3f,nearest.P(3),11f,offset}
````

## NativeTextLayout.cs

[Workspace source](../NativeTextLayout.cs)

Line 112

````csharp
new[]{'\r','\n'}
````

## NativeTextWindow.cs

[Workspace source](../NativeTextWindow.cs)

Line 54

````csharp
new Control[]{inputStaffLine,contextLine,responseLine}
````

Line 103

````csharp
new[]{'\r','\n'}
````

Line 161

````csharp
new Control[]{inputStaffLine,contextLine,responseLine,inputCorrectionLine}
````

## NativeVectorText.cs

[Workspace source](../NativeVectorText.cs)

Line 5

````csharp
VectorFamilies={"BODN","BODB","BODI","BDIB","MUSI"}
````

## NativeWindowsColors.cs

[Workspace source](../NativeWindowsColors.cs)

Line 11

````csharp
new[]{"background","text","edit","notation","output-background","output-text"}
````

## NativeWindowSettings.cs

[Workspace source](../NativeWindowSettings.cs)

Line 9

````csharp
new[]{b.X.ToString(CultureInfo.InvariantCulture),b.Y.ToString(CultureInfo.InvariantCulture),b.Width.ToString(CultureInfo.InvariantCulture),b.Height.ToString(CultureInfo.InvariantCulture),WindowState==FormWindowState.Maximized?"maximized":"normal"}
````

## NativeWindowsPreferencesLayout.cs

[Workspace source](../NativeWindowsPreferencesLayout.cs)

Line 101

````csharp
new[]{"SCORE profiles","Files & resources","Display & text","Workspace","Input & selection","Fonts & output","SCORE compatibility"}
````

Line 143

````csharp
new Control[]{swatch,code,reset}
````

## NativeWorkspaceGuides.cs

[Workspace source](../NativeWorkspaceGuides.cs)

Line 30

````csharp
new[]{"inches","centimetres","points","SCORE units"}
````

## NoteAccidentalCommands.cs

[Workspace source](../NoteAccidentalCommands.cs)

Line 94

````csharp
new[]{(float)parameter,note.P(parameter)+offset}
````

Line 159

````csharp
new[]{3f,itemAlignmentPosition.Value}
````

Line 180

````csharp
new[]{5f,value+(accidental-(int)value%10)}
````

## OriginalEnharmonicEditing.cs

[Workspace source](../OriginalEnharmonicEditing.cs)

Line 63

````csharp
new[]{400f,direction<0?-1f:1f,500f,delta}
````

Line 90

````csharp
new[]{400f,step,500f,accidentalDelta}
````

## OriginalFilteredEditing.cs

[Workspace source](../OriginalFilteredEditing.cs)

Line 15

````csharp
new[]{' '}
````

## OriginalItemSelectors.cs

[Workspace source](../OriginalItemSelectors.cs)

Line 4

````csharp
new[]{"COL","MU","MD","SC","ED","ST","CTN","CT","CD","JR","PT","CN","I","N"}
````

## OriginalParameterEditing.cs

[Workspace source](../OriginalParameterEditing.cs)

Line 146

````csharp
new[]{3f,nearest.P(3),15f,(float)((.8f-staffSize*count*1.41f)*spacing)}
````

## OriginalPostScriptNameRequests.cs

[Workspace source](../OriginalPostScriptNameRequests.cs)

Line 33

````csharp
new[]{preview.P(7),preview.P(8),preview.P(9),preview.P(10),preview.P(11)}
````

## PostScriptProgramPreview.cs

[Workspace source](../PostScriptProgramPreview.cs)

Line 16

````csharp
new[]{new PointF(0,0),new PointF(pageWidth,0),new PointF(0,pageHeight)}
````

## PostScriptTextRuns.cs

[Workspace source](../PostScriptTextRuns.cs)

Line 23

````csharp
new[]{(float)(a/30),(float)(b/30),(float)(-c/30),(float)(-d/30),(float)tx,(float)ty}
````

## RenderResourceAudit.cs

[Workspace source](../RenderResourceAudit.cs)

Line 17

````csharp
new[]{"item\tcode\tkind\tid\tresource\tstate\tparameters"}
````

## Scor4BeamEditMenu.cs

[Workspace source](../Scor4BeamEditMenu.cs)

Line 7

````csharp
Scor4BeamEditLabels = {
            new[]{"Spl1","Spl2","Spl3","Spl4","Mini+-","P5=P4","Lft0","Rht0","<---","--->",
                "1Beam","+Beam","-Num","+Num","Lf-Nt","Lf+Nt","Rt-Nt","Rt+Nt","+-P10","+-P13"},
            new[]{"TRM1","TREM2","TREM3","TREM4","P7-","P7+","P10-","P10+","<---","--->",
                "P11-","P11+","P12-","P12+","P13-","P13+","P14-","P14+","P15-","P15+"},
            new[]{"-.1","+.1","-.02","+.02","-1","+1","-.2","+.2","<---","Flip1",
                "","","","","","","","0 Horz","0 Vert","Flip2"}
        }
````

Line 8

````csharp
new[]{"Spl1","Spl2","Spl3","Spl4","Mini+-","P5=P4","Lft0","Rht0","<---","--->",
                "1Beam","+Beam","-Num","+Num","Lf-Nt","Lf+Nt","Rt-Nt","Rt+Nt","+-P10","+-P13"}
````

Line 10

````csharp
new[]{"TRM1","TREM2","TREM3","TREM4","P7-","P7+","P10-","P10+","<---","--->",
                "P11-","P11+","P12-","P12+","P13-","P13+","P14-","P14+","P15-","P15+"}
````

Line 12

````csharp
new[]{"-.1","+.1","-.02","+.02","-1","+1","-.2","+.2","<---","Flip1",
                "","","","","","","","0 Horz","0 Vert","Flip2"}
````

Line 75

````csharp
new[]{8f, adjusted[selected][7]}
````

Line 90

````csharp
new[]{800f,index == 12 ? -1f : 1f}
````

Line 106

````csharp
steps = {-.1f,.1f,-.02f,.02f,-1f,1f,-.2f,.2f}
````

Line 125

````csharp
new[]{(float)parameter, right, parameter + 1f, left}
````

Line 129

````csharp
new[]{(float)parameter, value}
````

Line 157

````csharp
new[]{7f,-stem * 10f,10f,10f + count,
                    11f,0f,12f,0f,13f,0f,14f,0f,15f,0f,16f,0f}
````

Line 160

````csharp
new[]{7f,stem * 10f + 1f,8f,0f,9f,0f,10f,0f,
                    11f,0f,12f,0f,13f,0f,14f,0f,15f,0f,16f,0f,17f,0f,18f,0f,19f,0f}
````

## Scor4GuitarEditMenu.cs

[Workspace source](../Scor4GuitarEditMenu.cs)

Line 5

````csharp
Scor4GuitarStringPlaces = {100000,10000,1000,100,10,1}
````

Line 6

````csharp
Scor4GuitarDotsLabels = {
            "Str1","Str2","Str3","Str4","Str5","Str6","St1-6","Clear","<---","",
            "Fret 1","2","3","4","5","6","7","OpenStr","X","Blank"
        }
````

Line 10

````csharp
Scor4GuitarFingerLabels = {
            "Str1","Str2","Str3","Str4","Str5","Str6","St1-6","P18=0","<---","",
            "Fing 1","2","3","4","","","Blank","0","T",""
        }
````

Line 14

````csharp
Scor4GuitarNameLabels = {
            "-ChdNt","+ChdNt","b/#/n","-ChTyp","+ChTyp","-Intvt","+Intvt","IntAc","<---","--->",
            "Font>","-FntSiz","+FntSiz","0FntSiz","","TxLv-.5","TxLv+.5","TxLv-5","TxLv+5","TxLv0"
        }
````

Line 18

````csharp
Scor4GuitarSuffixLabels = {
            "","SusAd","Acci","-Num","+Num","P10=0","","","<---","",
            "-/Note","+/Note","/Acci","-/Num","+/Num","P11=0","","","",""
        }
````

Line 22

````csharp
Scor4GuitarSizeLabels = {
            "-Size","+Size","NrmSz","","","-Strs","+Strs","0=6St","<---","",
            "-Wdth","+Wdth","NrmWd","","","-Fret","+Fret","0=5Fr","",""
        }
````

Line 26

````csharp
Scor4GuitarBarreLabels = {
            "Lft St1","Str2","Str3","Str4","Str5","","-Fr","+Fr","<---","",
            "BarrRht","Str2","Str3","Str4","Str5","Str6","-FrLev","+FrLev","NoBarre","NoFr"
        }
````

Line 52

````csharp
new[]{5f,13f - index}
````

Line 54

````csharp
new[]{1500f,index == 17 ? -.5f : .5f}
````

Line 55

````csharp
new[]{15f,0f}
````

Line 82

````csharp
new[]{12f,next}
````

Line 93

````csharp
new[]{(float)parameter,control == 2 ? 0f : value+(control == 0 ? -.1f : .1f)}
````

Line 104

````csharp
new[]{14f,remaining+next*place}
````

Line 121

````csharp
new[]{7f,note*10f+accidental}
````

Line 127

````csharp
new[]{8f,(float)((type + (index == 3 ? 9 : 1))%10)}
````

Line 136

````csharp
new[]{16f,index == 13 ? font : font+size+(index == 11 ? -.1f : .1f)}
````

Line 139

````csharp
new[]{19f,0f}
````

Line 140

````csharp
new[]{1900f,new[]{-.5f,.5f,-5f,5f}[index-15]}
````

Line 140

````csharp
new[]{-.5f,.5f,-5f,5f}
````

Line 153

````csharp
new[]{index == 5 ? 10f : 11f,0f}
````

Line 158

````csharp
new[]{11f,value%1000+next*1000f}
````

Line 170

````csharp
new[]{(float)parameter,value+(next-digit)*place}
````

Line 186

````csharp
new[]{(float)parameter,0f}
````

Line 196

````csharp
new[]{(float)parameter,stored + (digit - previous) * place}
````

## Scor4InputMenus.cs

[Workspace source](../Scor4InputMenus.cs)

Line 14

````csharp
Scor4PitchLabels = {
            "Clef","StmUp","StmDn","StmOr","M","Rest","RsWhl","RsInv","<esc>","-->",
            "Flat","Sharp","Natural","DblFlt","DblShp",":","BackSpc","/",";","(acc)"
        }
````

Line 18

````csharp
Scor4PitchMore = {
            "","","PrxOn","PrOff","","KySig","Meter","Bars","<--","",
            "","","","","","StafUp","StafDn","StafOr","",""
        }
````

Line 22

````csharp
Scor4PitchClefs = {
            "Treb","Bass","Alto","Tenor","Perc","","","","<--","",
            "-Treb","-Bass","-Alto","-Tenor","","","","","",""
        }
````

Line 26

````csharp
Scor4PitchKeys = {
            "1 Flat","2 Flats","3 Flats","4 Flats","5 Flats","6 Flats","7 Flats","","<--","",
            "1 Sharp","2 Shrps","3 Shrps","4 Shrps","5 Shrps","6 Shrps","7 Shrps","Natural","BackSpc",""
        }
````

Line 30

````csharp
Scor4PitchMeters = {
            "3/16","2/4","3/4","4/4","5/4","6/4","C","Cut","<--","9/16",
            "6/16","2/8","3/8","4/8","5/8","6/8","7/8","8/8","9/8","12/8"
        }
````

Line 34

````csharp
Scor4PitchBars = {
            "M","MD","MH","MR","ML","MRD","MS","MI","<--","",
            "1 Staff","2 Stvs","3 Stvs","4 Stvs","5 Stvs","6 Stvs","7 Stvs","8 Stvs","9 Stvs",""
        }
````

Line 38

````csharp
Scor4PrimaryPitchTokens={"","SU/","SD/","SO/","M","R","RW","RI"}
````

Line 39

````csharp
Scor4PitchAccidentals={"F","S","N","FF","SS",":"}
````

Line 40

````csharp
Scor4PitchClefTokens={"TR","BA","AL","TE","PR"}
````

Line 41

````csharp
Scor4RhythmLabels = {
            "DblWh", "Whole", "1/2", "1/4", "1/8", "1/16", "1/32", "1/64", "<esc>", "-->",
            "Quint", "Triple", "1/2.", "1/4.", "1/8.", "Dot", "BackSpc", "/", ";", "1/12"
        }
````

Line 45

````csharp
Scor4RhythmMore = {
            "", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "<--", "Grace",
            "X9", "X10", "X12", "X16", "X20", "X24", "X28", "X32", "X35", "X40"
        }
````

Line 49

````csharp
Scor4BeamLabels = {
            "1B;","2B;","3B;","4B;","5","6","3","Go","<esc>","Above",
            "1 Pkup","2 Pkup","3 Pkup","","AutoBms","","BackSpc","/",";","Below"
        }
````

Line 53

````csharp
Scor4SlurLabels = {
            "","","","","5","6","3","Go","<esc>","Above",
            "","","","","","","BackSpc","/",";","Below"
        }
````

Line 57

````csharp
Scor4SlurEditLabels = {
            new[]{"LftHf","RhtHf","ChDir","0P12","Dash","Dots","End1","End2","<--","-->",
                "P8-.05","P8+.05","-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","P8-1","P8+1"},
            new[]{"","","CNLeft","","CNRight","","","","<--","Mid",
                "","","","","","","","","",""}
        }
````

Line 58

````csharp
new[]{"LftHf","RhtHf","ChDir","0P12","Dash","Dots","End1","End2","<--","-->",
                "P8-.05","P8+.05","-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","P8-1","P8+1"}
````

Line 60

````csharp
new[]{"","","CNLeft","","CNRight","","","","<--","Mid",
                "","","","","","","","","",""}
````

Line 66

````csharp
new[]{8f,index == 10 ? -1f : -2f}
````

Line 67

````csharp
new[]{7f,3f,8f,3f}
````

Line 71

````csharp
new[]{900f,1f}
````

Line 84

````csharp
pieces = {1,2,-1,0}
````

Line 85

````csharp
new[]{12f,pieces[index]}
````

Line 88

````csharp
new[]{8f,0f,11f,index == 4 ? 1f : -1f}
````

Line 90

````csharp
new[]{7f,5f,8f,index - 5f,11f,0f}
````

Line 95

````csharp
new[]{800f,index == 10 ? -.05f : .05f}
````

Line 102

````csharp
new[]{8f,-1f}
````

Line 103

````csharp
new[]{800f,index == 18 ? -1f : 1f}
````

Line 125

````csharp
new[]{16f,useOffsets,(float)parameter,offset}
````

Line 127

````csharp
Scor4MarksLabels = {
            new[]{"Wedge","Accnt","Tenut","Stacc","Ferma","AccSt","Harm","Go","<esc>","-->"},
            new[]{"HvyWd","ACtTn","TenSt","WdgSt","C <","C- >","DnBow","UpBow","<--","-->"},
            new[]{"ppp","pp","p","mp","mf","f","pizz","arco","<--","-->"},
            new[]{"ff","fff","fp","sf","sfz","","","","<--","-->"},
            new[]{"cresc","dim","rit","accel","8va","8ba","","","<--","-->"},
            new[]{"Fing1","Fing2","Fing3","Fing4","Fing5","FictFl","FictSh","FictNt","<--","-->"},
            new[]{"Trill","TrFlt","TrShp","TrNat","","","","","<--","-->"},
            new[]{"","","","","","","","","<--",""}
        }
````

Line 128

````csharp
new[]{"Wedge","Accnt","Tenut","Stacc","Ferma","AccSt","Harm","Go","<esc>","-->"}
````

Line 129

````csharp
new[]{"HvyWd","ACtTn","TenSt","WdgSt","C <","C- >","DnBow","UpBow","<--","-->"}
````

Line 130

````csharp
new[]{"ppp","pp","p","mp","mf","f","pizz","arco","<--","-->"}
````

Line 131

````csharp
new[]{"ff","fff","fp","sf","sfz","","","","<--","-->"}
````

Line 132

````csharp
new[]{"cresc","dim","rit","accel","8va","8ba","","","<--","-->"}
````

Line 133

````csharp
new[]{"Fing1","Fing2","Fing3","Fing4","Fing5","FictFl","FictSh","FictNt","<--","-->"}
````

Line 134

````csharp
new[]{"Trill","TrFlt","TrShp","TrNat","","","","","<--","-->"}
````

Line 135

````csharp
new[]{"","","","","","","","","<--",""}
````

Line 137

````csharp
Scor4MarksTokens = {
            new[]{"W","A","T","S","FE","AS","H"},
            new[]{"HW","ACT","TS","WS","C","C-","D","U"},
            new[]{"PPP","PP","P","MP","MF","F","PZ","AO"},
            new[]{"FF","FFF","FP","SF","SFZ"},
            new[]{"CR","DI","RI","AC","O","OB"},
            new[]{"F1","F2","F3","F4","F5","FIF","FIS","FIN"},
            new[]{"TR","TRF","TRS","TRN"},
            new string[0]
        }
````

Line 138

````csharp
new[]{"W","A","T","S","FE","AS","H"}
````

Line 139

````csharp
new[]{"HW","ACT","TS","WS","C","C-","D","U"}
````

Line 140

````csharp
new[]{"PPP","PP","P","MP","MF","F","PZ","AO"}
````

Line 141

````csharp
new[]{"FF","FFF","FP","SF","SFZ"}
````

Line 142

````csharp
new[]{"CR","DI","RI","AC","O","OB"}
````

Line 143

````csharp
new[]{"F1","F2","F3","F4","F5","FIF","FIS","FIN"}
````

Line 144

````csharp
new[]{"TR","TRF","TRS","TRN"}
````

Line 238

````csharp
new[]{"Tr~~~","TrF~~","TrS~~","TrN~~"}
````

Line 240

````csharp
new[]{"TM1(8)","TM2(16)","TM3(32)","TM4(64)"}
````

## Scor4Interface.cs

[Workspace source](../Scor4Interface.cs)

Line 33

````csharp
Scor4ConfirmationLabels = {
            "No", "", "", "", "", "", "", "Yes", "", "",
            "", "", "", "", "", "", "", "", "", ""
        }
````

Line 37

````csharp
Scor4MainLabels = {
            "Help", "File>", "<File", "File>", "Input", "Print", "Play", "Exit", "I", "--->",
            "FullPg", "Screen>", "Group>", "EDInput", "Type", "Cut", "Paste", "UNDelet", "Items>", "User>"
        }
````

Line 41

````csharp
Scor4UserLabels = {
            "#01", "#02", "#03", "#04", "#05", "#06", "#07", "#08", "<---", "Type",
            "#09", "#10", "#11", "#12", "#13", "#14", ";", ";;", ";;;", ";;;;"
        }
````

Line 45

````csharp
Scor4FileLabels = {
            "MkDir", "Open", "SavSm", "Save", "SavNb", "SavNx", "Path", "GtMor", "<---", "--->",
            "NxtBk", "GetNext", "GetPrev", "ReadTxt", "ReStart", "RSM", "Combine", "Decom", "Delete", "Macro"
        }
````

Line 49

````csharp
Scor4FileInfoLabels = {
            "PM", "PMX", "Date", "", "", "", "", "", "<---", "",
            "", "", "", "", "", "", "", "", "", ""
        }
````

Line 53

````csharp
Scor4ScreenLabels = {
            "Bottm", "Top", "MovDn", "MovUp", "<-Mov", "Mov->", "-Zoom", "+Zoom", "<---", "",
            "Drag Screen", "", "TopDn", "TopUp", "FullScr", "ZoomFit", "Show", "SHX", "Zoom 1", ""
        }
````

Line 57

````csharp
Scor4CommandTwoLabels = {
            "SC1", "SC2", "SC3", "SC4", "SC0", "SCX", "Hght", "HS", "<--", "UNDO",
            "Dir *.DRW", "", "Dir *.EPS", "", "OS", "OH", "OT", "Pref", "Del.this file", ""
        }
````

Line 61

````csharp
Scor4GroupLabels = {
            "Move", "MvSys", "Copy", "CpSys", "Alter", "DElet", "TRnsp", "TAB", "<--", "LJ",
            "FLIp", "FLUp", "FLDn", "FLNrm", "CnWhl", "ENHrm", "VertJus", "JusText", "CntrDsh", "CuRVe"
        }
````

Line 65

````csharp
Scor4SetupLabels = {
            "Treb", "Bass", "Alto", "Tenor", "Perc", "Treb8", "Piano", "Organ", "<esc>", "-->",
            "StrQt", "Voc+Pno", "SATB", "Chr+Pno", "Orch1", "Orch2", "", "Reset", "Start", ""
        }
````

Line 69

````csharp
Scor4SetupMore = {
            "Set21", "Set22", "Set23", "Set24", "Set25", "Set26", "Set27", "Set28", "<---", "",
            "Strings", "Perc", "Brass", "WoodWnd", "", "", "", "", "", ""
        }
````

Line 73

````csharp
Scor4EditCommon = {
            "Help", "<-|->", "Copy", "Flip", "Centr", "Zm/Mv", "DElet", "Abort", "EndEd", "-->"
        }
````

Line 76

````csharp
Scor4EditLower = {
            new[]{"Rhyth>", "Marks>", "Accis>", "OffLf", "OffRt", "-Offst", "+Offst", "NoOff", "Tab>", "User>"},
            new[]{"Rhyth", "RstDn", "RstUp", "RsFrm", "RsInv", "-Offst", "+Offst", "NoOfs", "", "User>"},
            new[]{"Trebl", "Bass", "Alto", "Tenor", "Perc", "Tab", "", "Mini", "", "User>"},
            new[]{"Line", "Dash", "Dot", "Wiggl", "Arpeg", "Cres<", "Decrs>", "Rotat", "Centr", "User>"},
            new[]{"Slur", "Tie", "Brk3", "Brk+", "CenVt", "Curve", "FLIp", "P5=P4", "Centr", "User>"},
            new[]{"AdjOfs>", "AdjFr", "AdjF?", "AdjSt", "Adj?", "AutoBm", "AutoBm>", "FLIp", "Centr", "User>"}
        }
````

Line 77

````csharp
new[]{"Rhyth>", "Marks>", "Accis>", "OffLf", "OffRt", "-Offst", "+Offst", "NoOff", "Tab>", "User>"}
````

Line 78

````csharp
new[]{"Rhyth", "RstDn", "RstUp", "RsFrm", "RsInv", "-Offst", "+Offst", "NoOfs", "", "User>"}
````

Line 79

````csharp
new[]{"Trebl", "Bass", "Alto", "Tenor", "Perc", "Tab", "", "Mini", "", "User>"}
````

Line 80

````csharp
new[]{"Line", "Dash", "Dot", "Wiggl", "Arpeg", "Cres<", "Decrs>", "Rotat", "Centr", "User>"}
````

Line 81

````csharp
new[]{"Slur", "Tie", "Brk3", "Brk+", "CenVt", "Curve", "FLIp", "P5=P4", "Centr", "User>"}
````

Line 82

````csharp
new[]{"AdjOfs>", "AdjFr", "AdjF?", "AdjSt", "Adj?", "AutoBm", "AutoBm>", "FLIp", "Centr", "User>"}
````

Line 84

````csharp
Scor4NoteMore = {
            "NoStm", "Stem-", "Stem+", "StNrm", "CueSz", "NrmSz", "-NtHd", "+NtHd", "<---", "ED",
            "Staff-", "Staff+", "Staff0", "ADJ", "ADJ?", "ADF", "ADF?", "", "CHdRhy", ""
        }
````

Line 88

````csharp
Scor4NoteMarks = {
            "Wedge", "Accnt", "Tenut", "Stacc", "Ferma", "Plus", "NoMrk", "LRUD", "<---", "--->",
            "DnBow", "UpBow", "HvyWd", "WdgSt", "AccSt", "TenSt", "AccTn", "MOrd", "InvMo", "FlpMk"
        }
````

Line 92

````csharp
Scor4PrimaryMarkCodes = {4, 5, 6, 7, 14, 24, 0}
````

Line 93

````csharp
Scor4SecondaryMarkCodes = {11, 12, 19, 8, 9, 10, 25, 17, 18}
````

Line 94

````csharp
Scor4FingeringMarkCodes = {31, 32, 33, 34, 35, 13}
````

Line 95

````csharp
Scor4FictaTrillDotCodes = {1, 2, 3, 20, 21, 22, 23, 26, 27, 28}
````

Line 96

````csharp
Scor4NoteMoreMarks = {
            "Fing1", "Fing2", "Fing3", "Fing4", "Fing5", "Harmo", "", "LRUD", "<---", "",
            "FicFl", "FicSh", "FicNt", "Trill", "TrFlt", "TrShp", "TrNat", "2Dots", "3Dots", "4Dots"
        }
````

Line 100

````csharp
Scor4NoteMoveMark = {
            "", "MovDn", "MovUp", "MovLf", "MovRt", "NoMov", "", "", "<---", "FLM",
            "", "", "", "", "", "", "", "", "", ""
        }
````

Line 104

````csharp
Scor4NoteAccidentals = {
            "Flat", "Sharp", "Natrl", "DblFl", "DblSh", "NoAcc", "+Space", "NoSpc", "<---", "()",
            "ENH-1", "ENH+1", "", "", "", "", "-Space", "AutoSpc", "", ""
        }
````

Line 108

````csharp
Scor4ClefMore = {
            "Trb-8", "Trb+8", "Trb15", "Bas-8", "Tab4", "Tab5", "", "", "<---", "",
            "-Size", "+Size", "0Size", "OffLf", "OffRt", "NoOFf", "", "", "", ""
        }
````

Line 166

````csharp
new[]{"SCORE 3.11", "SCOR4"}
````

Line 281

````csharp
new[]{2f, (float)staff}
````

Line 572

````csharp
new[]{11f, Scor4PrimaryMarkCodes[index], 13f, 0f, 14f, 0f}
````

Line 587

````csharp
new[]{"MKD", "MKU", "MKL", "MKR"}
````

Line 588

````csharp
new[]{13f, 0f, 14f, 0f}
````

Line 593

````csharp
new[]{"FLT", "SHP", "NAT"}
````

Line 684

````csharp
new[]{.8f, .88f, .15f, 1.8f, 44f, 55f}
````

Line 685

````csharp
new[]{5f, value}
````

Line 693

````csharp
new[]{5f, value}
````

Line 703

````csharp
new[]{5f,canvas.Selected.P(4)}
````

Line 707

````csharp
new[]{9f,-1f}
````

Line 754

````csharp
new[]{13f,canvas.Selected.P(13) == 0 ? 1f : 0f}
````

Line 758

````csharp
new[]{3f,1.5f}
````

Line 789

````csharp
new[]{1000f,index == 15 ? -.5f : .5f}
````

Line 823

````csharp
verbs = {"M", "MS", "C", "CS", "A", "DE", "TR", "TAB", null, "LJ",
                    "FLI", "FLU", "FLD", "FLN", null, "ENH", "VJ", "JT", null, "CRV"}
````

## Scor4ItemsMenu.cs

[Workspace source](../Scor4ItemsMenu.cs)

Line 7

````csharp
Scor4ItemsLabels = {
            new[]{"Note", "Rest", "Clef", "Line", "Slur", "Beam", "Bar", "Staff", "<---", "--->",
                "Code9", "Number", "Cres<", "Decr>", "KeySig", "Meter", "End1", "End2", "8va/15", "Text"},
            new[]{"Harp", "PedA", "PedB", "PedC", "Arpeg", "Box", "Circl", "Guitr", "<---", "",
                "TREM1", "TREM2", "TREM3", "TREM4", "Trill", "Accord", "Bends", "Code11", "Code15", ""}
        }
````

Line 8

````csharp
new[]{"Note", "Rest", "Clef", "Line", "Slur", "Beam", "Bar", "Staff", "<---", "--->",
                "Code9", "Number", "Cres<", "Decr>", "KeySig", "Meter", "End1", "End2", "8va/15", "Text"}
````

Line 10

````csharp
new[]{"Harp", "PedA", "PedB", "PedC", "Arpeg", "Box", "Circl", "Guitr", "<---", "",
                "TREM1", "TREM2", "TREM3", "TREM4", "Trill", "Accord", "Bends", "Code11", "Code15", ""}
````

Line 13

````csharp
Scor4BarEditLabels = {
            "-Staff", "+Staff", "1Staff", "NormBar", "DblBar", "EndBar", "RptEnd", "RptBeg", "RptBoth", "User>"
        }
````

Line 16

````csharp
Scor4MoreBarLabels = {
            new[]{"-Offs", "+Offs", "NoOff", "Invis", "HeavyRpt", "Brace", "Brakt", "SubBk", "<---", "--->",
                "TopBrk", "BotBrk", "MidBrk", "+BarSpc", "0BarSpc", "+DotSpc", "0DotSpc", "+P17", "0P17", "Dash"},
            new[]{"-LowDt", "+LowDt", "-HiDot", "+HiDot", "0Dots", "-Thick", "+Thick", "NrmThk", "<---", "",
                "-PrtBot", "+PrtBot", "0PrtBot", "-PrtTop", "+PrtTop", "0PrtTop", "+DshSiz", "0DshSiz", "+DshSpc", "0DhSp"}
        }
````

Line 17

````csharp
new[]{"-Offs", "+Offs", "NoOff", "Invis", "HeavyRpt", "Brace", "Brakt", "SubBk", "<---", "--->",
                "TopBrk", "BotBrk", "MidBrk", "+BarSpc", "0BarSpc", "+DotSpc", "0DotSpc", "+P17", "0P17", "Dash"}
````

Line 19

````csharp
new[]{"-LowDt", "+LowDt", "-HiDot", "+HiDot", "0Dots", "-Thick", "+Thick", "NrmThk", "<---", "",
                "-PrtBot", "+PrtBot", "0PrtBot", "-PrtTop", "+PrtTop", "0PrtTop", "+DshSiz", "0DshSiz", "+DshSpc", "0DhSp"}
````

Line 22

````csharp
Scor4NumberEditLabels = {
            "NoBox/C", "Box", "Circle", "Font>", "-10", "+10", "-1", "+1", "", "User>"
        }
````

Line 25

````csharp
Scor4NumberDetailsLabels = {
            "", "", "", "", "", "", "JstOn", "JsOff", "<---", "",
            "-OffSt", "+OffSt", "NoOff", "-Size", "+Size", "Size 1", "+Spread", "NoSpred", "Num.1", "Ltr A"
        }
````

Line 29

````csharp
Scor4NumberFontsLabels = {
            new[]{"TmsRm","TmsBd","TmsIt","TBdIt","Helv","HlvBd","HlvOb","HBdOb","<---","--->",
                "HelvNrw","HelvNBd","HelvNOb","HlvNBOb","PaltnRm","PaltnBd","PaltnIt","PalBdIt","",""},
            new[]{"AvGBk","AvBOb","AvDem","AvDOb","BkmLt","BkLIt","BkDem","BkDIt","<---","",
                "NwCnSRm","NwCnSBd","NwCnSIt","NwCBdIt","ZapfChn","Courier","CurBld","CurObl","CurBlOb","Music"}
        }
````

Line 30

````csharp
new[]{"TmsRm","TmsBd","TmsIt","TBdIt","Helv","HlvBd","HlvOb","HBdOb","<---","--->",
                "HelvNrw","HelvNBd","HelvNOb","HlvNBOb","PaltnRm","PaltnBd","PaltnIt","PalBdIt","",""}
````

Line 32

````csharp
new[]{"AvGBk","AvBOb","AvDem","AvDOb","BkmLt","BkLIt","BkDem","BkDIt","<---","",
                "NwCnSRm","NwCnSBd","NwCnSIt","NwCBdIt","ZapfChn","Courier","CurBld","CurObl","CurBlOb","Music"}
````

Line 36

````csharp
new[]{"OutLine","P8=0"}
````

Line 37

````csharp
Scor4StaffEditLabels = {
            "-Line", "+Line", "5-Line", "Siz-.05", "Siz+.05", "Siz=1", "BtmLin#", "", "0/200", "User>"
        }
````

Line 40

````csharp
Scor4LibrarySymbolEditLabels = {
            "-OffSt", "+OffSt", "NoOff", "-HorzSz", "+HorzSz", "P6=1", "-VertSz", "+VertSz", "P7=0", "User>"
        }
````

Line 43

````csharp
Scor4SymbolEditLabels = {
            new[]{"-Rot","+Rota","0Rota","","P5-1","P5+1","P5-10","P5+10","<---","--->",
                "","","","","","","","","",""},
            new[]{"pppp","ppp","pp","p","mp","mf","f","ff","<---","--->",
                "fff","ffff","fp","sf","sfz","sff","sffz","sfff","-P5","+P5"},
            new[]{"fz","rf","rfz","sfpp","sfp","sffp","mfpp","mfp","<---","--->",
                "fpp","ffp","sfffz","sffffz","sfzp","sffzp","sfffzp","8 arrow","cresc","decrs"},
            new[]{"pizz.","arco","dim.","accel","rit.","rall.","a tmp","espr.","<---","--->",
                "perc.","early","poco","molto","meno","piu","mosso","subto","gliss","SCORE"},
            new[]{"ped","*","p(ed)","turn","segno","frmta","frmt□","frmt^","<---","",
                "(>)","orn.1","orn.2","diamx","VrtBk","(b)","micro1","micro2","-P5","+P5"}
        }
````

Line 44

````csharp
new[]{"-Rot","+Rota","0Rota","","P5-1","P5+1","P5-10","P5+10","<---","--->",
                "","","","","","","","","",""}
````

Line 46

````csharp
new[]{"pppp","ppp","pp","p","mp","mf","f","ff","<---","--->",
                "fff","ffff","fp","sf","sfz","sff","sffz","sfff","-P5","+P5"}
````

Line 48

````csharp
new[]{"fz","rf","rfz","sfpp","sfp","sffp","mfpp","mfp","<---","--->",
                "fpp","ffp","sfffz","sffffz","sfzp","sffzp","sfffzp","8 arrow","cresc","decrs"}
````

Line 50

````csharp
new[]{"pizz.","arco","dim.","accel","rit.","rall.","a tmp","espr.","<---","--->",
                "perc.","early","poco","molto","meno","piu","mosso","subto","gliss","SCORE"}
````

Line 52

````csharp
new[]{"ped","*","p(ed)","turn","segno","frmta","frmt□","frmt^","<---","",
                "(>)","orn.1","orn.2","diamx","VrtBk","(b)","micro1","micro2","-P5","+P5"}
````

Line 55

````csharp
Scor4SymbolValues = {
            new[]{50f,51f,52f,53f,54f,55f,56f,57f,0f,0f,58f,59f,60f,61f,62f,160f,129f,161f},
            new[]{123f,127f,128f,162f,163f,164f,165f,166f,0f,0f,167f,169f,280f,281f,282f,283f,284f,69f,63f,111f},
            new[]{66f,67f,64f,68f,65f,113f,78f,79f,0f,0f,600f,1000f,86f,73f,80f,81f,114f,85f,72f,199f},
            new[]{70f,71f,120f,75f,74f,31f,213f,214f,0f,0f,92f,93f,94f,95f,96f,97f,145f,182f}
        }
````

Line 56

````csharp
new[]{50f,51f,52f,53f,54f,55f,56f,57f,0f,0f,58f,59f,60f,61f,62f,160f,129f,161f}
````

Line 57

````csharp
new[]{123f,127f,128f,162f,163f,164f,165f,166f,0f,0f,167f,169f,280f,281f,282f,283f,284f,69f,63f,111f}
````

Line 58

````csharp
new[]{66f,67f,64f,68f,65f,113f,78f,79f,0f,0f,600f,1000f,86f,73f,80f,81f,114f,85f,72f,199f}
````

Line 59

````csharp
new[]{70f,71f,120f,75f,74f,31f,213f,214f,0f,0f,92f,93f,94f,95f,96f,97f,145f,182f}
````

Line 61

````csharp
Scor4OctaveEditLabels = {
            "8va", "8", "8ba", "-88", "15ma", "15", "RtBrk", "", "Centr", "User>"
        }
````

Line 64

````csharp
Scor4OctaveOffsetLabels = {
            "-LfOff", "+LfOff", "0LfOff", "-RtOff", "+RtOff", "0RtOff", "", "", "<---", "",
            "", "", "", "", "", "", "", "", "", ""
        }
````

Line 68

````csharp
Scor4HarpEditLabels = {
            "D Ped", "C Ped", "B Ped", "E Ped", "F Ped", "G Ped", "A Ped", "", "Centr", "User>"
        }
````

Line 71

````csharp
Scor4GraphicPedalEditLabels = {
            "LfPed", "MidPed", "RtPed", "-LfLin", "+LfLin", "-RtLin", "+RtLin", "-^-Peds", "Centr", "User>"
        }
````

Line 74

````csharp
Scor4PedalLineEditLabels = {
            "-^v-", "-^-", "-Point", "+Point", "0Point", "LfBrk", "RtBrk", "GrphPed", "Centr", "User>"
        }
````

Line 77

````csharp
Scor4TrillEditLabels = {
            "-LfOff", "+LfOff", "0LfOff", "-RtOff", "+RtOff", "0RtOff", "Wgl Y/N", "Acci", "Centr", "User>"
        }
````

Line 80

````csharp
Scor4ShapeEditLabels = {
            "-Thick", "+Thick", "NoThik", "Box/Cir", "-XSize", "+XSize", "-YSize", "+YSize", "P7=0", "P7=P6"
        }
````

Line 83

````csharp
Scor4GuitarEditLabels = {
            "Dots", "ChrdNam", "Barre", "FingNum", "P5=-1", "P5=-2", "P5=-3", "-OffSet", "+OffSet", "NoOff"
        }
````

Line 86

````csharp
Scor4AccordionEditLabels = {
            "-Thick", "+Thick", "NoThk", "-Size", "+Size", "", "-OffSt", "+OffSt", "NoOff", "User>"
        }
````

Line 89

````csharp
Scor4AccordionDetailsLabels = {
            "1-Lin","2-Lin","3-Lin","","","","","","<---","",
            "TopDot","Dot2","Dot3","Dot4","","LeftDot","RghtDot","","",""
        }
````

Line 93

````csharp
Scor4BendEditLabels = {
            "-HorzSz", "+HorzSz", "P6=1", "-VertSz", "+VertSz", "P7=1", "-Flatn", "+Flatn", "P11=0", "P7=P6"
        }
````

Line 96

````csharp
Scor4ExternalDrawingEditLabels = {
            "-HorzSz", "+HorzSz", "P6=1", "-VertSz", "+VertSz", "P7=0", "-Rotate", "+Rotate", "0Rotate", "User>"
        }
````

Line 99

````csharp
Scor4ExternalPostScriptEditLabels = {
            "-HorzSz", "+HorzSz", "P5=1", "-VertSz", "+VertSz", "P6=0", "-Rotate", "+Rotate", "0Rotate", "User>"
        }
````

Line 102

````csharp
Scor4ExternalNamesLabels = {
            "", "Dir", "NewNm", "", "", "", "", "", "<---", "",
            "", "", "", "", "", "", "", "", "", ""
        }
````

Line 106

````csharp
Scor4KeyEditLabels = {
            "-OffSt", "+OffSt", "NoOff", "1 Flat", "+Flat", "1 Sharp", "+Sharp", "Natural", "0 Natrl", "User>"
        }
````

Line 109

````csharp
Scor4MeterEditLabels = {
            "-OffSt", "+OffSt", "NoOff", "2/4", "3/4", "4/4", "5/4", "Cut", "C", "User>"
        }
````

Line 112

````csharp
Scor4KeyDetailsLabels = {
            "Treb","Bass","Alto","Tenor","CueSz","-Spc","+Spc","0Spc","<---","0Acci",
            "Acci1","Acci2","Acci3","Acci4","Acci5","Acci6","AcciDn","AcciUp","JustOff","JstOn"
        }
````

Line 116

````csharp
Scor4MeterDetailsLabels = {
            "3/8","5/8","6/8","9/8","12/8","-Size","+Size","NrmSz","<---","--->",
            "1 Alone","+Num","3/16","6/16","9/16","-TopNum","+TopNum","Font>","JustOff","JstOn"
        }
````

Line 120

````csharp
Scor4DoubleMeterLabels = {
            "-TopLf","+TopLf","","0 Dbl","","-RtTop","+RtTop","","<---","",
            "BotLf 2","BotLf 4","BotLf 8","BotLf16","+on/off","RtBot 2","RtBot 4","RtBot 8","RtBot16","RtBt0"
        }
````

Line 127

````csharp
Scor4ItemDefaults = {
            new[]{1f, 0f, 100f, 1f, 10f, 0f, 1f},
            new[]{2f, 0f, 100f, 0f, 0f, 0f, 1f},
            new[]{3f, 0f, 100f},
            new[]{4f, 0f, 100f, 0f, 0f, 120f},
            new[]{5f, 0f, 100f, 14f, 14f, 120f, 3f, -1f},
            new[]{6f, 0f, 100f, 8f, 8f, 120f, 11f},
            new[]{14f, 0f, 100f, 1f}
        }
````

Line 128

````csharp
new[]{1f, 0f, 100f, 1f, 10f, 0f, 1f}
````

Line 129

````csharp
new[]{2f, 0f, 100f, 0f, 0f, 0f, 1f}
````

Line 130

````csharp
new[]{3f, 0f, 100f}
````

Line 131

````csharp
new[]{4f, 0f, 100f, 0f, 0f, 120f}
````

Line 132

````csharp
new[]{5f, 0f, 100f, 14f, 14f, 120f, 3f, -1f}
````

Line 133

````csharp
new[]{6f, 0f, 100f, 8f, 8f, 120f, 11f}
````

Line 134

````csharp
new[]{14f, 0f, 100f, 1f}
````

Line 136

````csharp
Scor4AltItemDefaults = {
            new[]{9f, 0f, 100f, -2f, 54f},
            new[]{10f, 0f, 100f, 13f, 1f},
            new[]{4f, 0f, 100f, -1f, 999f, 120f},
            new[]{4f, 0f, 100f, -1f, 999f, 120f, -1f},
            new[]{17f, 0f, 100f, 0f, 2f},
            new[]{18f, 0f, 100f, 0f, 4f, 4f},
            new[]{5f, 0f, 100f, 15f, 15f, 120f, 5f, 1f},
            new[]{5f, 0f, 100f, 15f, 15f, 120f, 5f, 2f},
            new[]{7f, 0f, 100f, 15f, 0f, 120f, 8f, 0f, 0f, 0f, 0f, -1f}
        }
````

Line 137

````csharp
new[]{9f, 0f, 100f, -2f, 54f}
````

Line 138

````csharp
new[]{10f, 0f, 100f, 13f, 1f}
````

Line 139

````csharp
new[]{4f, 0f, 100f, -1f, 999f, 120f}
````

Line 140

````csharp
new[]{4f, 0f, 100f, -1f, 999f, 120f, -1f}
````

Line 141

````csharp
new[]{17f, 0f, 100f, 0f, 2f}
````

Line 142

````csharp
new[]{18f, 0f, 100f, 0f, 4f, 4f}
````

Line 143

````csharp
new[]{5f, 0f, 100f, 15f, 15f, 120f, 5f, 1f}
````

Line 144

````csharp
new[]{5f, 0f, 100f, 15f, 15f, 120f, 5f, 2f}
````

Line 145

````csharp
new[]{7f, 0f, 100f, 15f, 0f, 120f, 8f, 0f, 0f, 0f, 0f, -1f}
````

Line 147

````csharp
Scor4SecondItemDefaults = {
            new[]{7f, 0f, 100f, -1f, 1f, 0f, 1f, 111f, 1111f},
            new[]{7f, 0f, 100f, -1f, 0f, 140f, 20f},
            new[]{7f, 0f, 100f, -1f, 0f, 140f, 21f},
            new[]{7f, 0f, 100f, -1f, 0f, 140f, 22f},
            new[]{4f, 0f, 100f, 0f, 12f, 0f, -1f},
            new[]{12f, 0f, 100f, 15f, 0f, 10f, 10f},
            new[]{12f, 0f, 100f, 15f, 1f, 10f, 10f},
            new[]{12f, 0f, 100f, 16f, -4f, 1f, 10f},
            null, null,
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 11f},
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 12f},
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 13f},
            new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 14f},
            new[]{7f, 0f, 100f, 15f, 0f, 120f},
            new[]{12f, 0f, 100f, 16f, 10f, 0f, 3f},
            new[]{12f, 0f, 100f, 16f, 2f, 2f, 2f},
            null, null, null
        }
````

Line 148

````csharp
new[]{7f, 0f, 100f, -1f, 1f, 0f, 1f, 111f, 1111f}
````

Line 149

````csharp
new[]{7f, 0f, 100f, -1f, 0f, 140f, 20f}
````

Line 150

````csharp
new[]{7f, 0f, 100f, -1f, 0f, 140f, 21f}
````

Line 151

````csharp
new[]{7f, 0f, 100f, -1f, 0f, 140f, 22f}
````

Line 152

````csharp
new[]{4f, 0f, 100f, 0f, 12f, 0f, -1f}
````

Line 153

````csharp
new[]{12f, 0f, 100f, 15f, 0f, 10f, 10f}
````

Line 154

````csharp
new[]{12f, 0f, 100f, 15f, 1f, 10f, 10f}
````

Line 155

````csharp
new[]{12f, 0f, 100f, 16f, -4f, 1f, 10f}
````

Line 157

````csharp
new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 11f}
````

Line 158

````csharp
new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 12f}
````

Line 159

````csharp
new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 13f}
````

Line 160

````csharp
new[]{6f, 0f, 100f, 5f, 0f, 0f, -10f, 0f, 0f, 14f}
````

Line 161

````csharp
new[]{7f, 0f, 100f, 15f, 0f, 120f}
````

Line 162

````csharp
new[]{12f, 0f, 100f, 16f, 10f, 0f, 3f}
````

Line 163

````csharp
new[]{12f, 0f, 100f, 16f, 2f, 2f, 2f}
````

Line 201

````csharp
new[]{5f,next}
````

Line 210

````csharp
new[]{7f,newOrigin * 100 + Math.Max(lines,1)}
````

Line 212

````csharp
new[]{3f,0f,6f,0f}
````

Line 226

````csharp
new[]{(float)rotation,0f}
````

Line 227

````csharp
new[]{rotation * 100f,control == 6 ? -5f : 5f}
````

Line 237

````csharp
new[]{(float)parameter,next}
````

Line 244

````csharp
new[]{13f,postScript ? -1f : 1f}
````

Line 254

````csharp
Scor4HarpDetailsLabels = {
            "Size 1","-Size","+Size","-Offset","+Offset","0Offset","","","<---","",
            "","","","","","","","","",""
        }
````

Line 258

````csharp
Scor4PedalDetailsLabels = {
            "-Siz","+Siz","","","LfBrk","RtBrk","","","<---","",
            "-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","-PdOff","+PdOff","0PdOff",""
        }
````

Line 265

````csharp
new[]{5f,1f}
````

Line 267

````csharp
new[]{500f,index == 1 ? -.1f : .1f}
````

Line 269

````csharp
new[]{1500f,index == 3 ? -.5f : .5f}
````

Line 270

````csharp
new[]{15f,0f}
````

Line 275

````csharp
cycle = {0,2,102,-2}
````

Line 279

````csharp
new[]{(float)parameter,cycle[(position+1)%cycle.Length]}
````

Line 285

````csharp
new[]{900f,index == 0 ? -.1f : .1f}
````

Line 291

````csharp
new[]{16f,1f,parameter*100f,operation == 0 ? -.5f : .5f}
````

Line 296

````csharp
new[]{(float)parameter,0f,16f,0f}
````

Line 297

````csharp
new[]{(float)parameter,0f}
````

Line 302

````csharp
new[]{15f,0f}
````

Line 302

````csharp
new[]{1500f,index == 16 ? -.5f : .5f}
````

Line 311

````csharp
new[]{100,10,1,1000,100,10,1}
````

Line 317

````csharp
new[]{parameter * 100f,value == 3 ? -2f * place : place}
````

Line 324

````csharp
new[]{100,10,1}
````

Line 326

````csharp
new[]{500f,digit == 0 ? (float)place : -place}
````

Line 331

````csharp
new[]{8f,position + (index == 13 ? -.5f : .5f)}
````

Line 334

````csharp
new[]{600f,index == 15 ? -.5f : .5f}
````

Line 335

````csharp
new[]{5f,0f,7f,21f,8f,0f}
````

Line 344

````csharp
new[]{7f,index == 10 ? 21f : 22f,8f,point}
````

Line 346

````csharp
new[]{800f,index == 12 ? -.5f : .5f}
````

Line 347

````csharp
new[]{8f,0f}
````

Line 349

````csharp
new[]{5f,0f,7f,20f,8f,0f,11f,0f,12f,0f}
````

Line 357

````csharp
new[]{800f,-1f}
````

Line 359

````csharp
new[]{800f,1f}
````

Line 360

````csharp
new[]{8f,0f}
````

Line 361

````csharp
new[]{5f,1f - canvas.Selected.P(5)}
````

Line 362

````csharp
new[]{7f,0f}
````

Line 363

````csharp
new[]{7f,canvas.Selected.P(6)}
````

Line 365

````csharp
new[]{parameter * 100f,index % 2 == 0 ? -.2f : .2f}
````

Line 368

````csharp
Scor4CircleDetailsLabels = {
            "Arc-A","Arc+A","Arc-B","Arc+B","NoArc","","","","<---","--->",
            "Circle","Triangl","Pentagn","Hexagon","Octagon","","VrtOval","HrzOval","","User>"
        }
````

Line 372

````csharp
Scor4ShapeOffsetLabels = {
            "-Offs","+Offs","NoOff","","-Rotat","+Rotat","NoRot","","<---","",
            "","","","","","","","","",""
        }
````

Line 377

````csharp
new[]{"","NoArrw","NormArw","-Arrow","+Arrow","NormLng","+ArwLng","-Thick","+Thick","NoThk"}
````

Line 385

````csharp
new[]{(float)parameter,angle}
````

Line 387

````csharp
new[]{10f,0f,11f,0f}
````

Line 391

````csharp
new[]{0f,120f,72f,60f,45f}
````

Line 392

````csharp
new[]{12f,rotation}
````

Line 396

````csharp
new[]{7f,1.5f*width}
````

Line 397

````csharp
new[]{7f,width,6f,1.5f*width}
````

Line 398

````csharp
new[]{6f,1.5f*height}
````

Line 406

````csharp
new[]{15f,0f}
````

Line 406

````csharp
new[]{1500f,index == 0 ? -.5f : .5f}
````

Line 408

````csharp
new[]{9f,0f}
````

Line 408

````csharp
new[]{900f,index == 4 ? -5f : 5f}
````

Line 416

````csharp
new[]{12f,-1f}
````

Line 417

````csharp
new[]{12f,0f}
````

Line 418

````csharp
new[]{1200f,-.2f}
````

Line 419

````csharp
new[]{1200f,.2f}
````

Line 420

````csharp
new[]{13f,0f}
````

Line 421

````csharp
new[]{1300f,.2f}
````

Line 423

````csharp
new[]{800f,-1f}
````

Line 425

````csharp
new[]{800f,1f}
````

Line 426

````csharp
new[]{8f,0f}
````

Line 434

````csharp
new[]{1000f,-1f}
````

Line 437

````csharp
new[]{1000f,1f}
````

Line 437

````csharp
new[]{10f,0f}
````

Line 441

````csharp
new[]{6f,size + (index == 13 ? -.2f : .2f)}
````

Line 443

````csharp
new[]{7f,0f}
````

Line 445

````csharp
new[]{15f,0f}
````

Line 445

````csharp
new[]{1500f,index == 16 ? -.5f : .5f}
````

Line 451

````csharp
new[]{7f,index+2f,8f,0f,9f,0f}
````

Line 454

````csharp
new[]{1,10,100,1000}
````

Line 458

````csharp
new[]{parameter*100f,digit == 0 ? (float)place : -place}
````

Line 469

````csharp
new[]{(float)parameter,parameter == 6 ? 0f : 1f}
````

Line 474

````csharp
new[]{(float)parameter,size + (control % 3 == 0 ? -.1f : .1f)}
````

Line 476

````csharp
new[]{1100f,index == 16 ? -5f : 5f}
````

Line 477

````csharp
new[]{11f,0f}
````

Line 478

````csharp
new[]{7f,canvas.Selected.P(6)}
````

Line 489

````csharp
new[]{12f,canvas.Selected.P(12) == -2 ? -1f : -2f}
````

Line 490

````csharp
new[]{7f,new[]{8f,88f,-8f,-88f,15f,155f}[index - 10]}
````

Line 490

````csharp
new[]{8f,88f,-8f,-88f,15f,155f}
````

Line 495

````csharp
new[]{1500f,index == 0 ? -.5f : .5f}
````

Line 496

````csharp
new[]{15f,0f}
````

Line 497

````csharp
new[]{16f,1f,1800f,index == 3 ? -.5f : .5f}
````

Line 498

````csharp
new[]{16f,0f,18f,0f}
````

Line 506

````csharp
new[]{6f,right}
````

Line 512

````csharp
new[]{8f,accidental == 3 ? 0 : accidental + 1}
````

Line 528

````csharp
new[]{7f,vertical == 0 ? -horizontal : -vertical}
````

Line 531

````csharp
new[]{1500f,index == 10 ? -.5f : .5f}
````

Line 532

````csharp
new[]{15f,0f}
````

Line 538

````csharp
new[]{(float)parameter,next}
````

Line 547

````csharp
new[]{900f,index == 0 ? -5f : 5f}
````

Line 548

````csharp
new[]{9f,0f}
````

Line 550

````csharp
new[]{500f,new[]{-1f,1f,-10f,10f}[index - 4]}
````

Line 550

````csharp
new[]{-1f,1f,-10f,10f}
````

Line 555

````csharp
new[]{500f,index == 18 ? -1f : 1f}
````

Line 558

````csharp
new[]{5f,31f,6f,.25f}
````

Line 560

````csharp
new[]{5f,symbol}
````

Line 566

````csharp
new[]{8f,new[]{0f,2f,1f}[index - 10]}
````

Line 566

````csharp
new[]{0f,2f,1f}
````

Line 569

````csharp
new[]{500f,new[]{-10f,10f,-1f,1f}[index - 14]}
````

Line 569

````csharp
new[]{-10f,10f,-1f,1f}
````

Line 574

````csharp
new[]{16f,index == 6 ? 1f : 0f}
````

Line 576

````csharp
new[]{1500f,index == 10 ? -.5f : .5f}
````

Line 577

````csharp
new[]{15f,0f}
````

Line 581

````csharp
new[]{6f,index == 15 ? 0 : size + (index == 13 ? -.1f : .1f)}
````

Line 585

````csharp
new[]{14f,(spread == 0 ? 1 : spread) + .02f}
````

Line 587

````csharp
new[]{14f,0f}
````

Line 588

````csharp
new[]{5f,index == 18 ? 1f : 1001f}
````

Line 604

````csharp
new[]{(float)parameter,meter || text ? 1094f : tablature ? 94f : 6f}
````

Line 606

````csharp
new[]{(float)parameter,0f}
````

Line 615

````csharp
new[]{8f,1100f+font}
````

Line 619

````csharp
new[]{(float)parameter,tablature ? slot : chord ? 10f*slot : 1000f + slot}
````

Line 623

````csharp
new[]{1500f,index == 10 ? -.5f : .5f}
````

Line 624

````csharp
new[]{15f,0f}
````

Line 626

````csharp
new[]{5f,index - 11f,6f,4f}
````

Line 627

````csharp
new[]{5f,index == 17 ? 98f : 99f,6f,0f}
````

Line 628

````csharp
new[]{5f,index == 13 ? -1f : 1f}
````

Line 637

````csharp
new[]{5f,next}
````

Line 642

````csharp
new[]{6f,(float)index}
````

Line 645

````csharp
new[]{6f,clef >= 100 ? clef - 100 : clef + 100}
````

Line 650

````csharp
new[]{7f,index == 7 ? 0 : spacing + (index == 5 ? -.05f : .05f)}
````

Line 653

````csharp
new[]{8f,0f,9f,0f,10f,0f,11f,0f,12f,0f,13f,0f}
````

Line 660

````csharp
new[]{(float)parameter,next}
````

Line 666

````csharp
new[]{parameter * 100f,index == 16 ? -1f : 1f}
````

Line 668

````csharp
new[]{14f,index == 18 ? 1f : 0f}
````

Line 672

````csharp
new[]{5f,new[]{3f,5f,6f,9f,12f}[index],6f,8f}
````

Line 672

````csharp
new[]{3f,5f,6f,9f,12f}
````

Line 679

````csharp
new[]{7f,next,400f,index == 5 ? .62f : -.62f}
````

Line 681

````csharp
new[]{4f,0f,7f,0f}
````

Line 684

````csharp
new[]{5f,0f,6f,1f}
````

Line 685

````csharp
new[]{600f,1f}
````

Line 686

````csharp
new[]{5f,3f * (index - 11),6f,16f}
````

Line 687

````csharp
new[]{500f,index == 15 ? -1f : 1f}
````

Line 689

````csharp
new[]{14f,index == 18 ? 1f : 0f}
````

Line 694

````csharp
new[]{500f,index == 0 ? -1f : 1f}
````

Line 695

````csharp
new[]{8f,0f,9f,0f}
````

Line 696

````csharp
new[]{800f,index == 5 ? -1f : 1f}
````

Line 698

````csharp
new[]{6f,(float)(2 << (index - 10))}
````

Line 699

````csharp
new[]{12f,canvas.Selected.P(12) == 0 ? 1f : 0f}
````

Line 700

````csharp
new[]{9f,index == 19 ? 0f : (float)(2 << (index - 15))}
````

Line 707

````csharp
new[]{7f,0f}
````

Line 712

````csharp
new[]{7f,lines == -1 ? 0 : lines + (index == 10 ? -1 : 1)}
````

Line 716

````csharp
new[]{3f,canvas.Selected.P(3) == 0 ? 200f : 0f}
````

Line 717

````csharp
new[]{400f,index == 10 ? -1f : 1f}
````

Line 718

````csharp
new[]{4f,1f}
````

Line 719

````csharp
new[]{0f,1f,2f,4f,3f,5f}
````

Line 720

````csharp
new[]{5f,style}
````

Line 732

````csharp
new[]{1500f,index == 0 ? -.5f : .5f}
````

Line 733

````csharp
new[]{15f,0f}
````

Line 734

````csharp
new[]{4f,-Math.Max(1,Math.Abs(canvas.Selected.P(4))),5f,0f}
````

Line 736

````csharp
new[]{6f,8f,9f,10f}
````

Line 737

````csharp
new[]{4f,Math.Abs(canvas.Selected.P(4)),5f,style}
````

Line 740

````csharp
new[]{5f,9f,8f,index - 9f}
````

Line 742

````csharp
new[]{1300f,.2f}
````

Line 742

````csharp
new[]{13f,0f}
````

Line 744

````csharp
new[]{1400f,.2f}
````

Line 744

````csharp
new[]{14f,0f}
````

Line 746

````csharp
new[]{1700f,.05f}
````

Line 746

````csharp
new[]{17f,0f}
````

Line 747

````csharp
new[]{5f,7f,8f,0f,9f,0f}
````

Line 755

````csharp
new[]{-200f,200f,-2f,2f}
````

Line 756

````csharp
new[]{16f,value + offset}
````

Line 758

````csharp
new[]{16f,0f}
````

Line 761

````csharp
new[]{6f,index == 7 ? 0 : Math.Max(0,value + (index == 5 ? -.2f : .2f))}
````

Line 768

````csharp
new[]{(float)parameter,action == 2 ? 0 : value + (action == 0 ? -1f : 1f)}
````

Line 774

````csharp
new[]{reset ? parameter : parameter * 100f,reset ? 0f : .2f}
````

Line 870

````csharp
new[]{7f,0f,100f,-1f,0f,0f,1f,111f,1111f}
````

Line 874

````csharp
new[]{12f,0f,100f,15f,0f,10f}
````

Line 875

````csharp
new[]{12f,0f,100f,15f,1f,10f}
````

## Scor4LineEditMenu.cs

[Workspace source](../Scor4LineEditMenu.cs)

Line 5

````csharp
Scor4LineMore = {
            "Vert","Horz","Dsh-","Dsh+","Dsh0","Spc-","Spc+","Spc0","<---","--->",
            "LfBr-","LfBr+","LfBr0","RtBr-","RtBr+","RtBr0","Lft<","Rht>","Arp>Dn","Arp>Up"
        }
````

Line 9

````csharp
Scor4LineFine = {
            "","","Thick","0Thk","Thn~~","Thn^^","Thk^^","P5=P4","<---","",
            "","","-LfOff","+LfOff","0LfOff","-RtOff","+RtOff","0RtOff","Rotate","0Rota"
        }
````

Line 16

````csharp
new[]{7f,0f}
````

Line 17

````csharp
new[]{7f,1f}
````

Line 18

````csharp
new[]{7f,-4f}
````

Line 19

````csharp
new[]{7f,-1f}
````

Line 23

````csharp
new[]{1300f,10f}
````

Line 33

````csharp
new[]{5f,top,6f,0f,7f,-1f}
````

Line 40

````csharp
new[]{5f,999f,6f,right,7f,closing ? -1f : 0f}
````

Line 45

````csharp
new[]{13f,270f}
````

Line 46

````csharp
new[]{5f,canvas.Selected.P(4),13f,0f}
````

Line 56

````csharp
new[]{11f,102f}
````

Line 57

````csharp
new[]{12f,102f}
````

Line 61

````csharp
new[]{12f,index == 18 ? -1f : 1f}
````

Line 69

````csharp
new[]{1000f,1f}
````

Line 70

````csharp
new[]{10f,0f}
````

Line 71

````csharp
new[]{5f,canvas.Selected.P(4),7f,-2f}
````

Line 72

````csharp
new[]{5f,canvas.Selected.P(4),7f,-3f}
````

Line 73

````csharp
new[]{5f,canvas.Selected.P(4),7f,-3f,8f,2f,11f,.8f}
````

Line 74

````csharp
new[]{5f,canvas.Selected.P(4)}
````

Line 78

````csharp
new[]{1300f,10f}
````

Line 79

````csharp
new[]{13f,0f}
````

Line 93

````csharp
new[]{15f,leftMode,16f,active,(float)parameter,offset}
````

Line 98

````csharp
new[]{(float)parameter,value}
````

Line 103

````csharp
new[]{(float)parameter,value}
````

## Scor4NoteEditMenu.cs

[Workspace source](../Scor4NoteEditMenu.cs)

Line 8

````csharp
Scor4NoteRhythmLabels = {
            "Whole","1/2","1/4","1/8","1/16","1/32","Dot","Grace","<---","DW",
            "1/64","Tripl","Quint","-SpDt","+SpDt","DtU/D","Sp1Nt","Dotx2","NoTail","+Tail"
        }
````

Line 12

````csharp
Scor4NoteTabLabels = {
            "-StrN","+StrN","0StrN","4Strs","5Strs","6Strs","7Strs","8Strs","<---","--->",
            "Bass","Treble","8va","15ma","Tie","X","-Capo","+Capo","Font>","User>"
        }
````

Line 16

````csharp
Scor4NoteManualTabLabels = {
            "Manual Tab","","","4Strs","5Strs","6Strs","7Strs","","<---","",
            "TP0","TP3","TP5","TP7","TP9","-StrNum","+StrNum","-FretNm","+FretNm","( )"
        }
````

Line 22

````csharp
new[]{8f,0f}
````

Line 22

````csharp
new[]{800f,index == 0 ? -1f : 1f}
````

Line 23

````csharp
new[]{6f,-index - 1f}
````

Line 30

````csharp
new[]{10,0,80,150}
````

Line 31

````csharp
new[]{6f,-(strings + modifier)}
````

Line 33

````csharp
new[]{index - 6f,-1f}
````

Line 36

````csharp
new[]{19f,Math.Max(0,capo + (index == 16 ? -1f : 1f))}
````

Line 45

````csharp
new[]{6f,-11f * (index + 1)}
````

Line 47

````csharp
positions = {"0","3","5","7","9"}
````

Line 51

````csharp
new[]{index < 17 ? 800f : 900f,index % 2 == 0 ? 1f : -1f}
````

Line 56

````csharp
new[]{12f,font >= 100 ? font - 100 : font + 100}
````

Line 66

````csharp
new[]{6f,3f,7f,8f}
````

Line 70

````csharp
new[]{9f,(int)canvas.Selected.P(9) % 1000 + 1000f}
````

Line 73

````csharp
new[]{9f,control - (int)control % 10}
````

Line 75

````csharp
new[]{900f,1f}
````

Line 88

````csharp
new[]{4f,pitch,6f,head,7f,duration,8f,extension,9f,Math.Max(0,index - 2)}
````

Line 100

````csharp
new[]{7f,(float)(duration * (dots == 1 ? 1.5 : 1.75)),9f,dots * 10f + control % 10}
````

Line 105

````csharp
new[]{9f,Math.Max((int)control,control + increment)}
````

Line 111

````csharp
new[]{900f,adjustment}
````

Line 120

````csharp
new[]{4f,pitch + (pitch < 0 ? -100 : 100),6f,0f,7f,65f,8f,101f,9f,1f}
````

Line 128

````csharp
new[]{6f,next}
````

Line 132

````csharp
new[]{15f,offset}
````

Line 138

````csharp
new[]{800f,-.5f}
````

Line 139

````csharp
new[]{800f,.5f}
````

Line 146

````csharp
new[]{12f,2f}
````

Line 147

````csharp
new[]{12f,1f}
````

Line 148

````csharp
new[]{12f,0f}
````

Line 199

````csharp
new[]{5f,value - stem * 10}
````

Line 206

````csharp
new[]{5f,packed + (accidental - (int)packed % 10)}
````

Line 215

````csharp
new[]{5f,packed - (parentheses + accidental)}
````

Line 222

````csharp
new[]{5f,packed >= 100 ? packed - 100f : packed + 100f}
````

Line 232

````csharp
new[]{5f,next >= control + 1 ? control : next}
````

Line 236

````csharp
new[]{11f,mark}
````

Line 247

````csharp
new[]{5f,control + (accidental == 2 ? .26f : .22f)}
````

Line 252

````csharp
new[]{11f,-mark}
````

Line 261

````csharp
new[]{400f,cueOffset}
````

Line 262

````csharp
new[]{400f,-cueOffset}
````

Line 272

````csharp
new[]{5f,value,8f,0f}
````

Line 282

````csharp
new[]{6f,head}
````

## Scor4RestEditMenu.cs

[Workspace source](../Scor4RestEditMenu.cs)

Line 5

````csharp
Scor4BinaryDurations = {4,2,1,.5f,.25f,.125f,.0625f}
````

Line 6

````csharp
Scor4RestRhythmLabels = {
            "Whole","1/2","1/4","1/8","1/16","1/32","Dot","RPbar","<---","--->",
            "1/64","Tripl","Quint","-SpDt","+SpDt","","","Dotx2","CueSz","NrmSz"
        }
````

Line 10

````csharp
Scor4RestNumberedLabels = {
            "WhB1","WhB2","WhB3","WhB4","WhB5","WhB6","WhB7","WhB8","<---","DW",
            "Eur1","Eur2","Eur3","Eur4","Eur5","Eur6","Eur7","Eur8","NoNum",""
        }
````

Line 18

````csharp
new[]{400f,-1f}
````

Line 19

````csharp
new[]{400f,1f}
````

Line 20

````csharp
new[]{8f,-1f}
````

Line 21

````csharp
new[]{6f,-1f}
````

Line 22

````csharp
new[]{1000f,-.5f}
````

Line 23

````csharp
new[]{1000f,.5f}
````

Line 24

````csharp
new[]{10f,0f}
````

Line 32

````csharp
new[]{5f,-4f,6f,0f,8f,0f,9f,-1f}
````

Line 44

````csharp
new[]{5f,-3f,6f,0f,7f,8f}
````

Line 46

````csharp
new[]{8f,0f}
````

Line 52

````csharp
new[]{5f,style,6f,0f,8f,(float)count,9f,-1f}
````

Line 56

````csharp
new[]{5f,index - 2f,6f,0f,7f,Scor4BinaryDurations[index],9f,0f}
````

Line 62

````csharp
new[]{7f,(float)((double)duration * (divisor - 1) / divisor)}
````

Line 72

````csharp
new[]{6f,(float)dots,7f,(float)(duration * (dots == 1 ? 1.5 : 1.75))}
````

Line 79

````csharp
new[]{6f,Math.Max(dots,prior + increment)}
````

## Scor4TextEditMenu.cs

[Workspace source](../Scor4TextEditMenu.cs)

Line 5

````csharp
Scor4TextDetailsLabels = {
            "-Spc","+Spc","NrmSp","Font>","CTN","CTx","<EdTx",">EdTx","<---","--->",
            "-ChrSp","+ChrSp","NrmChSp","-Parts","+Parts","0Parts","-Rotate","+Rotate","0Rotate","JRx"
        }
````

Line 9

````csharp
Scor4TextPointsLabels = {
            "-Pnt","Pnt10","Pnt16","Pnt24","+Pnt","","AbsSz","Abort","<---","",
            "Rip-1","Rip-2","Rip-3","Rip-4","Rip 1","Rip 2","Rip 3","Rip 4","Ripx-1","Ripx1"
        }
````

Line 17

````csharp
new[]{11f,0f}
````

Line 17

````csharp
new[]{1100f,index == 10 ? -.5f : .5f}
````

Line 25

````csharp
new[]{(float)parameter,next}
````

Line 32

````csharp
new[]{10f,0f}
````

Line 32

````csharp
new[]{1000f,index == 0 ? -.1f : .1f}
````

Line 43

````csharp
new[]{5f,1f}
````

Line 43

````csharp
new[]{500f,index == 10 ? -.01f : .01f}
````

Line 46

````csharp
new[]{6f,(float)(size+(index == 13 ? 200f : index == 14 ? 100f : 0f))}
````

Line 49

````csharp
new[]{9f,0f}
````

Line 49

````csharp
new[]{900f,index == 16 ? -5f : 5f}
````

Line 62

````csharp
new[]{6f,next}
````

Line 65

````csharp
new[]{10,16,24}
````

## ScoreModern.cs

[Workspace source](../ScoreModern.cs)

Line 39

````csharp
names={"Unknown","Note","Rest","Clef","Line / hairpin","Slur / bracket","Beam","Trill / pedal","Staff","Library symbol","Number","User symbol","Shape","Invisible item","Barline","PostScript","Text","Key signature","Time signature"}
````

Line 177

````csharp
new[]{0,8,-8,15,88,20,21,22}
````

Line 303

````csharp
a={"Stem / accidental","Notehead","Duration","Stem length","Dots / flags"}
````

Line 308

````csharp
codes={1,2,3,4,5,6,8,10,14,17,18}
````

## ScoreText.cs

[Workspace source](../ScoreText.cs)

Line 48

````csharp
accents = { "áéíóúÁÉÍÓÚ", "àèìòùÀÈÌÒÙ", "âêîôûÂÊÎÔÛ", "äëïöüÄËÏÖÜ" }
````

Line 49

````csharp
controls = { "<<", ">>", "^^", "%%" }
````

## TmbPreferences.cs

[Workspace source](../TmbPreferences.cs)

Line 83

````csharp
new[]{'\t', '\r', '\n'}
````

Line 84

````csharp
new[]{'\t', '\r', '\n'}
````

## TmbUtilities.cs

[Workspace source](../TmbUtilities.cs)

Line 16

````csharp
Names = {"ACCS", "BEAM", "ES", "LABELS", "LJ", "SEP", "TRANSPOSE", "VJ"}
````

Line 95

````csharp
new[]{"CapHeight"}
````

Line 106

````csharp
new[]{"Line up and justify", "Line up", "Justify"}
````

Line 111

````csharp
new[]{"Top", "Bottom"}
````

Line 113

````csharp
new[]{' ', '\t', ','}
````

Line 120

````csharp
new[]{"Staff number (P2)", "Staff ID (P9)", "Transpose macro"}
````

Line 127

````csharp
new[]{' ', '\t', ','}
````

Line 136

````csharp
new[]{"Set system height", "Report current system height"}
````

## Type1Outline.cs

[Workspace source](../Type1Outline.cs)

Line 51

````csharp
names={"zero","one","two","three","four","five","six","seven","eight","nine"}
````

## BeamGeometry.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BeamGeometry.cs)

Line 68

````csharp
new[]{new BeamVertex(x2,bb),new BeamVertex(x2,b),new BeamVertex(x1,a),new BeamVertex(x1,aa)}
````

## BeamStemAdjustment.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/BeamStemAdjustment.cs)

Line 62

````csharp
durations={.5f,.25f,.125f,.0625f,.03125f,.015625f}
````

## CharacterOutputRoutine.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/CharacterOutputRoutine.cs)

Line 15

````csharp
new ushort[]{0x78c,0x797,0x7a2,0x7a8,0x7b0}
````

## EqualStepRasterFlagsModel.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/EqualStepRasterFlagsModel.cs)

Line 40

````csharp
new[]{0xa77}
````

## HorizontalRasterFlagsModel.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/HorizontalRasterFlagsModel.cs)

Line 37

````csharp
left={0,1,7,20}
````

Line 37

````csharp
right={0,7,8,620}
````

Line 37

````csharp
bases={65530,0,0x4b68,34640}
````

## InputChordLayout.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputChordLayout.cs)

Line 6

````csharp
spacing={240,250,215,440,250,240,290,240,440,300,240,255,220,430,280,250,280,240,500,330,
155,250,180,380,200,240,290,230,440,250,200,250,160,420,180,0,280,160,0,0,
155,200,120,360,180,180,220,110,400,0,130,200,120,320,140,0,0,0,0,0,
120,180,120,350,160,0,200,110,0,0,0,120,0,0,0,0,0,0,0,0,
0,110,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
// Original DS39A8 table spills into DS3A70 data at distance7.
// The first pair is the stored exponent0.5849624872; preserve these
// measured adjacent words rather than rejecting the reachable lookup.
-16358,16149,10512,0,14136,14381,19761,93,10512,59,14136,5164,129,4,5,6,7,8,9,10}
````

Line 15

````csharp
below={0,1,3,3,1,1}
````

Line 15

````csharp
above={0,5,4,4,5,1}
````

## InputEditInverse.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputEditInverse.cs)

Line 18

````csharp
values={250,500,1000,2000,4000,8000,333}
````

Line 18

````csharp
tokens={"S","E","Q","H","W","D","T"}
````

Line 18

````csharp
new[]{unchecked((byte)(whole/10+48)),(byte)(whole%10+48)}
````

Line 22

````csharp
new[]{unchecked((byte)(count/10+48)),(byte)(count%10+48)}
````

Line 44

````csharp
clefs={"TR","BA","AL","TE","PR"}
````

Line 44

````csharp
offsets={0,12,6,8,0}
````

Line 45

````csharp
new[]{"","D","H","RL","RR","RD","I ","DS"}
````

Line 45

````csharp
new[]{"S0","S+","S-"}
````

Line 46

````csharp
names={"FIF","FIS","FIN","W","A","T","S","WS","AS","TS","D","U","H","FE","AR","TH","MO","IM","HW"}
````

Line 46

````csharp
new[]{"TR","TRF","TRS","TRN","PL","ACT"}
````

Line 46

````csharp
new[]{"S2","S3","S4","TR"}
````

Line 47

````csharp
new[]{"","F","S","N","FF","SS"}
````

Line 49

````csharp
new[]{"","F","S","N"}
````

Line 49

````csharp
labels={"PPP","PP","P","MP","MF","F","FF","FFF",null,"FP","SF","SFZ","CR","DI","RI","PZ","AO","AC"}
````

Line 49

````csharp
new[]{String.Join("/",pitches)+";",CompressRhythms(rhythms,reproduceInvalid)+";",String.Join("/",marks)+";",String.Join("/",beams)+";",String.Join("/",slurs)+";"}
````

## InputEndpointText.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputEndpointText.cs)

Line 31

````csharp
new[]{9999f,body.GroupControl,body.Skip}
````

## InputMainNoteReconstruction.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMainNoteReconstruction.cs)

Line 28

````csharp
new[]{encoded}
````

## InputMarksCompiler.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMarksCompiler.cs)

Line 41

````csharp
new[]{4,5,8,9,10,19}
````

## InputMarkText.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputMarkText.cs)

Line 68

````csharp
new[]{':','\''}
````

## InputReferenceSpacing.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputReferenceSpacing.cs)

Line 94

````csharp
new[]{1f,number,x,0,10,0,duration}
````

Line 95

````csharp
new[]{1f,number,x,100,10,0,65,100}
````

Line 96

````csharp
new[]{14f,number,x,1}
````

Line 97

````csharp
new[]{-duration,number,x}
````

## InputRhythmToken.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputRhythmToken.cs)

Line 15

````csharp
denominators={.0625f,16,8,4,2,1,.5f}
````

## InputSetupBounds.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputSetupBounds.cs)

Line 26

````csharp
names={"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#"}
````

## InputSetupState.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputSetupState.cs)

Line 8

````csharp
minimum={0,1,-99,-99,0,1,1,-99,1,1,1}
````

Line 9

````csharp
maximum={0,32,199,99,9,216,32,199,216,99,99}
````

Line 18

````csharp
new float[]{8,Staff,LeftEnd,Vertical,Size==1?0:Size,RightEnd==200?0:RightEnd}
````

## InputSlurConstruction.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputSlurConstruction.cs)

Line 9

````csharp
marks={4,5,6,7,8,9,10,19,30,31,32,33,34,35}
````

## InputTextLists.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTextLists.cs)

Line 106

````csharp
new[]{':','\''}
````

## InputTranspositionClefs.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionClefs.cs)

Line 10

````csharp
shifts={{0,12,6,8},{-12,0,-6,-4},{-6,6,0,2},{-8,4,-2,0}}
````

## InputTranspositionCollection.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionCollection.cs)

Line 14

````csharp
accidental={0,-1,1,3,-2,2}
````

## InputTranspositionEmbeddedControl.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionEmbeddedControl.cs)

Line 7

````csharp
pitches={70,1,3,5,6,8,10,12}
````

Line 8

````csharp
encodings={6,10,21,20,31,30,40,42,50,61,60,71,70}
````

## InputTranspositionInterval.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionInterval.cs)

Line 7

````csharp
menu={-8,-5,-4,-3,-2,2,3,5,7,8,9,10,14,21,12,-12}
````

Line 8

````csharp
keys={-6,1,-4,3,-2,5,0,-5,2,-3,4,-1,6}
````

Line 9

````csharp
degrees={0,1,1,2,2,3,3,4,5,5,6,6}
````

## InputTranspositionKeyInsertion.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionKeyInsertion.cs)

Line 24

````csharp
new float[]{17f,staff,position,0f,key,clef}
````

## InputTranspositionPitch.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputTranspositionPitch.cs)

Line 7

````csharp
clefs={0,-12,-6,-8,-7}
````

Line 8

````csharp
encodings={4,1,0,2,5}
````

Line 9

````csharp
keys={
   {-1,-1,-1,-1,-1,-1,-1},{-1,-1,-1,0,-1,-1,-1},{0,-1,-1,0,-1,-1,-1},
   {0,-1,-1,0,0,-1,-1},{0,0,-1,0,0,-1,-1},{0,0,-1,0,0,0,-1},{0,0,0,0,0,0,-1},
   {0,0,0,0,0,0,0},{0,0,0,1,0,0,0},{1,0,0,1,0,0,0},{1,0,0,1,1,0,0},
   {1,1,0,1,1,0,0},{1,1,0,1,1,1,0},{1,1,1,1,1,1,0},{1,1,1,1,1,1,1}}
````

## InputUniformBeam.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/InputUniformBeam.cs)

Line 19

````csharp
new[]{4,5,8,9,10,19}
````

## LayoutBasicTablature.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutBasicTablature.cs)

Line 20

````csharp
new[]{8f,(float)destination,staff[2],(float)((double)staff[3]/factor-strings+1),(float)((double)size*factor),staff.Length>=6?staff[5]:200f,(float)strings}
````

Line 20

````csharp
new[]{14f,(float)destination,staff[2],2f}
````

Line 22

````csharp
new[]{3f,(float)destination,(float)((double)staff[2]+2),0f,tabClef}
````

Line 35

````csharp
encodings={4,1,3,2,5}
````

Line 42

````csharp
new[]{1f,(float)destination,note[2],note[3],(float)accidental,(float)-head,note[6],frequency<=0?-1f:(float)chosen}
````

Line 58

````csharp
new[]{1f,(float)destination,note[2],0.8999999761581421f,20f,7f,-2f,-1.5f,0f,0f,0f,0f,0f,0f,0.800000011920929f,0f,-0.699999988079071f}
````

## LayoutJustificationSystem.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationSystem.cs)

Line 12

````csharp
new[]{Selected,Auxiliary,Minimum,Positions,OldMinimum,OldPositions,MergedPositions,MergedMinimum}
````

## LayoutNoteSpacingSpan.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutNoteSpacingSpan.cs)

Line 84

````csharp
new[]{0xd4d0,0xd4c4,0xd496,0xd4bc,0xd486}
````

## LayoutRestSpacing.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutRestSpacing.cs)

Line 10

````csharp
new[]{multiplier,p6,p5,p4,left,p9,p10,p15}
````

## LayoutTablatureAccidentalTable.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureAccidentalTable.cs)

Line 7

````csharp
raw={79,85,68,0,1,2,0,10,20,73,87,80,68,85,70,19787,20559,21330,22612,11610,20782,4,1,3,2,5,10512,0,12127,0,10512,70,12127,74,10512,122,12127,74,10512,176,12127,0,10512,224,12127,0,10512}
````

## LayoutTablatureFrequency.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureFrequency.cs)

Line 6

````csharp
clefs={0,-12,-6,-8,-7}
````

Line 6

````csharp
degrees={0,0,2,4,5,7,9,11}
````

Line 7

````csharp
keys={
   {-1,-1,-1,-1,-1,-1,-1},{-1,-1,-1,0,-1,-1,-1},{0,-1,-1,0,-1,-1,-1},
   {0,-1,-1,0,0,-1,-1},{0,0,-1,0,0,-1,-1},{0,0,-1,0,0,0,-1},{0,0,0,0,0,0,-1},
   {0,0,0,0,0,0,0},{0,0,0,1,0,0,0},{1,0,0,1,0,0,0},{1,0,0,1,1,0,0},
   {1,1,0,1,1,0,0},{1,1,0,1,1,1,0},{1,1,1,1,1,1,0},{1,1,1,1,1,1,1}}
````

## LayoutTablaturePitch.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/LayoutTablaturePitch.cs)

Line 17

````csharp
values={packed[pair]/100,packed[pair]%100}
````

Line 29

````csharp
clefs={0,-12,-6,-8}
````

Line 29

````csharp
accidentals={0,-1,1,0,-2,2,0,2,4,5}
````

Line 29

````csharp
degrees={2,0,2,4,5,7,9,11}
````

Line 29

````csharp
table={40,76,71,67,62,57,52,47,47,0}
````

## SelectorRoutine.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SelectorRoutine.cs)

Line 27

````csharp
new ushort[] { 0x3c7a }
````

Line 48

````csharp
new ushort[] { 0x0010, 0x3c6a, context.DataSegment }
````

Line 49

````csharp
new ushort[] { 0x3c82, returnedAx }
````

## SlopedRasterFlagsModel.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlopedRasterFlagsModel.cs)

Line 41

````csharp
new[]{0x9fd,0xa38}
````

## SlurOrigins.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurOrigins.cs)

Line 14

````csharp
left={0,0,9,27,36,18,9,18,9,0,0}
````

Line 15

````csharp
right={0,0,9,-6,-6,14,14,9,0,9,14}
````

## SlurPostprocess.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/SlurPostprocess.cs)

Line 27

````csharp
table={-1.6f,-1.6f,-.6f,.6f,1.6f,1.6f,.6f,-.6f,-1.6f,-1.6f,-.6f}
````

## TupletGeometry.cs

[Workspace source](../../ScoreReverseEngineering/NativeFragments/TupletGeometry.cs)

Line 11

````csharp
new[]{(char)(number/10+48),(char)(number%10+48)}
````

Line 12

````csharp
new[]{(char)(number/100+48),(char)((number%100)/10+48),(char)(number%10+48)}
````
