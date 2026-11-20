# Code 18 · Time signatures · SCOR4

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

```text
  parameter structure
╔══════════╦═══════════╦════════════╦═══════════╦═══════════════╦════════════╗
║    p1    ║     p2    ║     p3     ║    p4     ║      p5       ║     p6     ║
║ code num.║ staff num.║ horiz. pos.║ vert. pos.║ top meter num.║ bottom num.║
╚═══ 18 ═══╩═══════════╩════════════╩═══════════╩═══════════════╩════════════╝
```

```text
╔══════╦═══════════════╦════════════╦═════════════╦═════════════════════╗
║  p7  ║      p8       ║     p9     ║     p10     ║         p11         ║
║ size ║ 2nd meter top ║ 2nd bottom ║ offset of + ║ offset of 2nd meter ║
╚══════╩═══════════════╩════════════╩═════════════╩═════════════════════╝
```

```text
╔═══════════╦══════════════════╦════════════════╦══════════════╦═════════════╗
║    p12    ║       p13        ║       p14      ║      p15     ║     p16     ║
║ suppress+ ║ change type font ║ ignore justify ║ horz. offset ║ laser thick ║
╚═══════════╩══════════════════╩════════════════╩══════════════╩═════════════╝
```

CODE 18 creates:

- Meters

Meter numbers are taken from MUSI0.DRW file.

```text
P5 = numerator of meter
P6 = denominator of meter
```

*--> To get common time, alla breve, and single meters: P5 = 99 = common time (C) = 98 = alla breve  (¢) =  0 = single meter number (found in P6) centered*

```text
P7 = Size of meter
P8 = numerator of second meter
P9 = denominator of second meter
```

Therefore, to get 3/4 + 5/8:

```text
P5=3    P8=5
P6=4    P9=8
```

To get 3+4/8, let p6 = denominator and set p9 to 0:

```text
P5=3    P8=4
P6=8    P9=0
```

P9 = -1 automatically centers a meter over the nearest barline, helpful for oversized meters. (Selecting the meter and entering CM does the same.) --> This will not work with double meters.

P10 alters the position of the "+" between double meters.

P11 alters the distance between the meters.

P12 = 1 = suppress the "+" between double meters.

P13 = Font Substitution. To substitute the font in FM##.PSC, set p13=10##. For example, to get the font in FM24.PSC, set P13=1024.

P14 = 1 = suppresses recognition by LJ routine (0 = normal)

P15 = horizontal offset from given p3 position --> This p15 offset is ignored by SCORE's LJ, but recognized by LJ.EXE

P16 = outline thickness control.  P16 adds (or subtracts) pixels to the basic line thickness established by the prompts for EPS/PS printing.

*--> Negative values reduce or "thin" the outline of the glyph --> Positive values increase or "thicken" the outline of the glyph*

E.g., for small staff sizes, setting p16 = -4 might be optimal for ensuring that the meter remains crisp and distinct, rather than "puffy," which is typical at the default p16=0 setting.

*--> This parameter functions identically to P10 of Code3 and Code9.*

WARNING: a non-zero value in P16 causes SCORE's LJ routine to add a fixed, currently unquantifiable amount of space following the meter.  Therefore, if you wish to use P16 to thin meters, do it after LJ'ing in SCORE, or use Thomas Brodhead's LJ.EXE, which does not reproduce this bizarre behavior.


**Verified implementation:** P16 changes the print outline width in native vector/WYSIWYG output and EPS, including common-time symbols; it does not alter DAT contour coordinates. Negative values thin and positive values thicken. Width is restored after the meter.
