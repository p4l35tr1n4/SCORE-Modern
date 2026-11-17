# Code 14 · Barlines

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 14 -- bar lines -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | number of staves up |
| `P5` | bar type |

```text
╔═══════════╦═══════════╦═════════╦═══════════════╦════════════════╗
║     p6    ║    p7     ║    p8   ║       9       ║    p10, p11    ║
║ thickness ║ hrz.dsplc.║ dash sz.║ dash space sz.║ bar extensions ║
╚═══════════╩═══════════╩═════════╩═══════════════╩════════════════╝
```

| Parameter | Meaning |
| --- | --- |
| `P12` | mark for PAGE |
| `P13` | dbl.space |
| `P14` | dot space |
| `P15` | horz. offset |

| Parameter | Meaning |
| --- | --- |
| `P16` | vert. dot pos. |
| `P17` | normal space |

================ CODE 14 creates: ================ * Barlines (including system barlines, piano braces, etc.)

NB: All bar lines originate at the bottom line of the staff indicated by P2.

P4 = number of staves extending upward.

If P4 = 3 and P2 = 2, the bar line will extend from the bottom line of staff 2 up to the top line of staff 4.

If P4 is negative (-1, -2, etc.), barline will be invisible when printed.

To alter the vertical origination and termination of the barline, use p10 and p11 respectively (see documentation below).

```text
P5 =  -1: thick barline, centered at p3 and thickened to the LEFT
   =   0: ordinary bar
   =   1: double bar (two thin bars)
   =   2: ending double bar (1 thin, 1 thick)
   =   3: repeat double bar, pointing left (thin, heavy - dots to left)
   =   4:    "     "     "      "     right (heavy, thin)
   =   5:    "     "     "      "     both ways (thin, heavy, thin)
   =   6:    "     "     "      "      "    "   (heavy, heavy)
   =   7: dashed bar. (P8=size of dash, P9=size of space, default 2,1)
   =   8: curved brace
   =   9: heavy bracket (see P8 below for top and bottom hook suppression)
   =  10: thin bracket to left
   >= 11: p5=7 dashed bar, top & bottom extensions equivalent to p10=1, p11=13
```

P6 = added thickness on P5=0, P5=8, and P5=11 barlines ONLY.

On P5=0 and P5=8 barlines, the P6 value may be either positive or negative --> If P6 is positive, the barline is thickened (stretched) to the LEFT --> If P6 is negative, the barline is thickened (stretched) to the RIGHT

This produces good effects for P5=0 barlines, with p6=4 making barlines more visible for players.  On the other hand, this produces bizarre and typically unusable effects for P5=8 barlines, EXCEPT if you need a mirror image of one, in which case P6= -1 will do the trick.

On P5=11 barlines, only positive values have an effect, and the dashes are stretched both left and right equally.

--> When printed, SCORE automatically doubles the thickness of all thin --> and dashed barlines.

P7 = This may be used for the horizontal displacement of a bar. Its usual use is to position a brace (curved) to the left of a bracket in an orchestral score.  When P7 is used, the various justifying and spacing routines will not change the brace's relative position. A negative value displaces to the LEFT, and a positive value displaces to the RIGHT.

```text
P8 = Partial heavy brackets (P5 = 9) may be made by using P8.
   = 0 = end units of bracket appear at both top and bottom.
   = 1 = end unit appears only at top.
   = 2 =  "   "      "     "   "  bottom.
   = 3 = vertical part of bracket only. (No top or bottom units.)
```

Use these (in conjunction with P10, P11) when preparing large orchestra scores, where several separate files may be needed for a single complete system of staves.

P9 = size of the space in dashed barlines (i.e. when P5=7)

P10 = sets origin of barline (bottom). = 0 = default = 3, bottom line

P11 = sets destination of barline (top). = 0 = default = 11, top line

The values are in scale steps.  If P10 is negative, the bar will begin below the staff.  If P10 = 11, it will begin at the top line of the originating staff.  P11 = 7 will cause the bar to end at middle line of destination staff, etc.

--> To make a barline in keyboard music that travels through the --> "white space" between staves, set p10=11 and p11=3

--> To make p5=10 thin brackets travel from the bottom staffline --> of the bottom staff to the top staffline of the top staff, --> set p10=3 and p11=11

P12 = 1 marks the bar as an extraction point for laying out music with the PAGE program using the "P" and subsequent "M" and "N" options. Contrary to the written documentation (which is WRONG), this feature works as follows:

If music with barlines marked with P12=1 is fed into PAGE for page layout, then the "P" option may then be used to "use Part of input data." There are two subsequent options, M or N.

Option M allows you to pull out material between successive p12=1 marked bars.  So, if you have 5 sections in a piece, you'd have 4 p12=1 bars, which we'll call ABCDE here.

```text
A = beginning of piece to marked bar 1
B = marked bar 1 to marked bar 2
C = marked bar 2 to marked bar 3
D = marked bar 3 to marked bar 4
E = marked bar 4 to marked bar 5
```

To pull out section "A", you'd respond "0 1" to the M option, because you're starting at the beginning of the piece (0) and running to the first marked bar.

To pull out section "B", you'd respond "1 2" to the M option, because you're starting at the FIRST marked bar and ending at the SECOND marked bar.

To pull out section "C", you'd respond "2 3" to the M option, because you're starting at the SECOND marked bar and ending at the THIRD marked bar.

etc.

In general: Whichever section of the piece is being pulled out is the SECOND of the two numbers (so, to pull out the 13th section, you'd respond "12 13" to the M prompt, where the first number is 1 less than the section number and the second number is the section number).

Notice that you're putting 1 into p12, *NOT* the section numbers.

The "N" option works like the "M" option, except that instead of specifying "0 1", "1 2", "2 3", etc., you specify the exact measure numbers where the barlines are located.  This is problematic in that PAGE may miscount the measure numbers when it reads in the MUS data, making debugging the procedure extremely difficult when the extracted material is not what you expected to receive.  It is therefore much safer to use the "M" option.

The P->M option in PAGE is useful for extracting parts.  First, extract a part by the normal method in PAGE.  Then, examine the results and determine where you want page breaks to occur.  Mark those barlines with p12=1, and then feed the extracted part back into PAGE for page layout using the P->M option, pulling out the part section-by-section. In this way you can obtain page turns precisely where you want them, and with different numbers of staves per page (which the U option in PAGE, which attempts to find page turns for you, does not allow).

It is the utility to the part extraction process that the P->M option in PAGE allows.

========== Double bar spacing ==========

```text
P13 = alters the space between the lines of a double bar or repeat bar.
P14 = alters the space between the dots and the lines of repeat bars.
      (i.e. pushing them farther away from the repeat bars when P14 is
      increased)
P15 = Horizontal displacement of barline...identical to P7, except
      ignored by SCORE's LJ routine.
```

P16 = the vertical positions of the repeat dots.

= 0 (normal) puts dots in the second and third spaces of the staff. These correspond to note levels 6 and 8.

```text
= 3-digit number XYZ:
  X  = vertical position of lower dot
  YZ = position of upper dot
```

For example:

P16 = 410 puts dots at level 4 and 10 (the bottom and top spaces). = 408 puts dots at level 4 and 8.

P17 = staff size at which to duplicate the double bar spacing, overriding the current staff size.  This is useful for scores with staves of different sizes, e.g. a Violin Sonata score, where the Piano is at normal size (say .7) and the Violin part above is smaller (say .5)

For example:

P17 = .5 = space the double bar as if it were attached to a C8P5=.5 staff.
