# Code 5 · Slurs, brackets and endings · SCOR4

## In SCORE Modern Core

In the supplied SCORE executable, integer `P8=10` follows the slur path. The Windows compatibility option **Endings: P8=10 follows Tom’s Help** selects the documented ending interpretation.





## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

```text
                           ╔═══════════╦═══════════╗
  parameter structure:     ║     p1    ║     p2    ║
                           ║ code num. ║ staff num.║
                           ╚═══════════╩═══════════╝
            ┌──────────────────────────────────────────────────────┐
            │                  ┌───────────────────┐               │
╔═══════════╧═════════╦════════╧══════════╦════════╧════════╦══════╧════════╗
║          p3         ║        p4         ║        p5       ║      p6       ║
║ left horizontal pos.║ left vertical pos.║ right vert. pos.║ rt. horz. pos.║
╚═════════════════════╩═══════════════════╩═════════════════╩═══════════════╝
```

```text
 ╔═══════════╦════════╦════════════╦══════════════╦══════════╦══════════════╗
 ║     p7    ║   p8   ║     p9     ║     p10      ║   p11    ║     p12      ║
 ║ curvature ║ offset ║ flattening ║ curve center ║ dash/dot ║ change direc.║
 ╚═══════════╩══(num)═╩═══(num)════╩══════════════╩══════════╩══════════════╝
```

```text
                   ╔═══════════╦════════╦════════╗
                   ║    p13    ║  p14   ║  p15   ║
                   ║ thickness ║ start  ║  stop  ║
                   ╚═══════════╩════════╩════════╝
```

```text
 ╔════════╦═════════════╦══════════════╦═══════════════════════════╗
 ║  p16   ║    p17      ║    p18       ║            p19            ║
 ║ number ║ num. lft-rt ║ num. up-down ║ fine tune slur thickening ║
 ╚════════╩═════════════╩══════════════╩═══════════════════════════╝
```

CODE 5 creates:

- Slurs & Ties

- Straight glissando lines between notes

- Tuplet brackets

- 1st and 2nd ending brackets

- Primitive Guitar Tremolo Bar Slides (see Code12 for more advanced controls)

### SLURS AND TIES

P7 = amount of curvature (in scale steps).  A positive number makes the slur curve upwards, a negative number makes the slur curve downwards (i.e. the center of the slur will be lower than the ends).

P8 = horizontal offset of slur: ---> Assume two consecutive notes, 1 and 2: =  0 or 10 = start LEFT edge of note 1 and end at LEFT edge of note 2. --> useful for downstemmed ("smiling") slur connecting --> two downstemmed beamed notes.

= -1 or 11 = start at CENTER of of note 1 and end at CENTER of note 2 --> standard slur

= -2 or 12 = slur offset into white space between notes (=TIE) --> standard tie

= -3 or 13 = same as -2, but extra space for 1st note being dotted. --> tie starting with note with augmentation dot

= -4 or 14 = start at RIGHT edge of note 1 and end at RIGHT edge of note 2. --> useful for upstemmed ("frowning") slur connecting --> two upstemmed beamed notes.

= -5 or 15 = start at CENTER of note 1 and end at RIGHT edge of note 2. --> useful for upstemmed ("frowning") sluring connecting --> a downstemmed note to the top of the *last* note of an --> upstemmed, beamed note.

= -6 or 16 = start at RIGHT edge of note 1 and end at CENTER of note 2. --> useful for upstemmed ("frowning") sluring connecting --> the TOP of the *first* note of an upstemmed, beamed note --> to a note that is either up or downstemmed.

= -7 or 17 = start at CENTER of note 1 and end at LEFT edge of note 2. --> useful for downstemmed ("smiling") sluring connecting --> a note that is *either* up or downstemmed to the LAST --> note of a downstemmed beamed group.

= -8 or 18 = start at LEFT edge of note 1 and end at CENTER of note 2. --> useful for downstemmed ("smiling") sluring connecting --> the FIRST note of a downstemmed beamed group to an --> upstemmed note.

= -9 or 19 = start at LEFT edge of note 1 and end at RIGHT edge of note 2. --> useful for CROSS-STAFF sluring between the first note of --> a downstemmed, beamed note to the last note of an upstemmed --> beamed note.

= .01 to .99 = displaces slur from "starting" location of p8 = -9/19, moving each end INWARD by the same amount.  The greater the number, the greater the inward offset.

P9 = slur flattening factor.  Longer slurs often should be flattened in the middle.  By setting P9 to a positive value, a larger portion of the slur will be flat.  Negative values will decrease the flat area.

P10 = set center of curvature.  When P10 is left at zero the default value of .5 is in effect.  If P10 is less than .5 the center of curvature shifts to the left.  Values greater than .5 shift the center to the right.  P10 also moves number in bracket.

### EDITING P10 CENTERPOINTS

To edit the P10 centerpoint, depress <Shift>+[F10] and press left or right arrow keys (<- or ->). Toggle this mode off by depressing <Shift>+[F10]

P11 >= 1: slur is made up of dashes. (P8=0) --> When P11=1, the number of dashes created is an amount that seems --> optimal by SCORE, provided the length of the slur. --> When P11>=2, the number of dashes created is EXACTLY the number indicated by P11.

P11 <= -1: slur is made up of dots. --> When P11 = -1, the number of dots created is an amount that seems --> optimal by SCORE, provided the length of the slur. --> When P11 = -2, the number of dots is always 4 --> When P11 = -3, the number of dots is always 6 --> When P11 <= -4 and an EVEN number, the number of dots created is --> EXACTLY the absolute value of the number indicated by p11; when --> P11 <= -4 and an ODD number, the number of dots created is EXACTLY --> the absolute value of the number indicated by P11 *MINUS* 1 (i.e. --> always producing an even number of dots).

P12 = half or reversed slurs. = 1: the slur shape will be as if only the left half were being drawn. = 2: only the right half shape appears. = -1: causes the slur to reverse direction at its midpoint (see P10). If P7 is positive, the 1st part of the curve will be up.  If P7 is negative, the 1st part of the curve will be down.  (The slur length must be adjusted visually when P12 = -1.)

```text
P13 = thickness control.
    =   0: normal thickness (100%)
    =  .6: 60% of normal
    = 1.3: 130% of normal, etc.
```

P14 & P15 are used to break up slurs to avoid crossing over other items. The slur first should be set up with its normal parameter values in P3, P4, P5, and P6.  Then P14 can set where the visible portion of the slur STARTS and P15 can set where the visible portion ENDS. P14 = .3 = visible part starts 30% of the way through.  P15 = .65 = visible part ends 65% of the way through, etc.  P14 and P15 can be used separately or together.

P16 = a number to be put in the middle of a broken slur.  (For triplets, etc.) If the number is NEGATIVE, it will appear (positive) just below an unbroken slur whose curve is up, or just above a slur whose curve is down.

P17 = moves the number given in P16 left or right.

P18 = moves the number given in P16 up or down. (These parameters must be used if the slur is quite steep.)

ADDITIONAL SLUR OFFSET: --> If P16 = 1, then: P17 = ABSOLUTE left offset P18 = ABSOLUTE right offset. --> If this is used, P8 will be ignored, and the other uses of --> p16, p17, and p18 are not valid.

P19 = fine tune slur thickening. P19 may control the position through the drawing of the first half of the slur where it reaches its maximum thickness. The default is P19 = 0 = .85 = 85%. This might be used (in conjunction with P13) for very short ties or nearly vertical slurs. (Use the SHOW feature while editing.)

### STRAIGHT GLISSANDO SLIDES

*--> This inherits all the properties of ties --> --> Set P7=0 --> Set P8= -2, or -3 so that the slide appears between the affected notes --> Set P9=99 --> Set P13 => 1 to thicken the line --> --> The advantage of creating a straight line this way is that it shrinks --> and expands with LJ, just as the p3-p6 endpoints of ties.*

### TRIPLET/TUPLET BRACKETS

P8 = 3: broken bracket with the number 3 in center. (P11=0)

P9 = sets the number (if number other than 3 is needed.) --> The number in P8/P9 uses the typeface indicates by FM02.PSC --> (i.e. the _02 font).  The size is equivalent to Code16 p6=.85

*--> P8 = 3 and P9 <= -2: open space in bracket (for |--- 3:5 ---| etc.), --> where the reduction in p9 corresponds to a greater gap in the bracket.*

*--> P8 = 3, P9 = -1 creates triplet bracket with LEFT bracket absent. --> Reverse P3 and P6 to make RIGHT bracket absent. --> For NO bracets, P9=0 (or number for different tuplet number) --> and P7=0.*

```text
P10 = move the number to the left or right of center.
    =  0: default (.5) = number is centered.
    = .3: number is 30% distance from left to right.
    = .8:   "    "  80%    "      "    "   "    "
```

```text
P11 = 1: continuous bracket with number inside.
    = 2:      "        "     "     "    outside.
    = 3: bracket with NO number. (P8 must = 3 for all these.)
```

```text
P17 = Horizontal shift of number
P18 = Vertical shift of number
```

### 1ST AND 2ND ENDING BRACKETS

P7 controls the length of the legs of the brackets. (Set P7=4 to make the legs exactly meet the baseline of the ending numbers, although P7=4.5 is graphically perhaps more pleasing.)

P8 = 1: 1st ending = 2: 2nd ending. --> P8 = 4-10 creates 4th through 10th endings (P8=3 creates tuplets).

If P9 is positive, then P9 sets the ending number.

```text
   --> P8 = 1 and P9 = -2: NO NUMBER and BRACKETS ON BOTH SIDES ( |-----| )
   --> P8 = 1 and P9 = -1: NO NUMBER and NO BRACKET on LEFT     (  -----| )
   --> P8 = 2 and P9 = -1: NO NUMBER and NO BRACKET on RIGHT    ( |-----  )
```

P9 = -2: no number, complete bracket

The 'CN n' command (center) will move the ends of 1st and 2nd endings to positions of nearest bar lines.  If the bar line starts from a staff below, the staff number (n) must be given.

*--> To extract 1st and 2nd endings into all parts during PAGE part extraction:*

P5 = 100: put ending in ALL parts, = 200: in NO parts (i.e. exclude from the staff where it appears)

Additional Offsets for all Code5 item types:

```text
  If P16=1, then:
     P17=absolute left offset
     P18=absolute right offset.
  In this case, P8 will be ignored - other uses of p16,17,18 are not valid.
```

### GUITAR TREMOLO BAR SLIDES

All the parameters here work just like those for slurs except that only the upper 2 vectors of a triangle will appear instead of the curve of a slur.

Set P9 = 99

```text
--> If P7 = 0, only a straight line will appear.
P10 = midpoint shift
P13 = thickness (1, 2, 3, etc.)
```

### EDITING COMMANDS FOR SLURS AND TIES

```text
On individual items:
CN = Centers the item's p3 and p6 to the closest p3 of note and/or rest
CV = Creates default curvature for slurs and ties
```

Global commands: CRV = CR all items on page in staff range specified at prompts


**Verified implementation:** P16=1 bypasses P8 and selects a slur with P17/P18 horizontal endpoint displacements in SCORE units, even when P8 otherwise selects an ending or tuplet. It does not print the label 1 in this mode. SCORE 3.11 retains its older interpretation.


**Current Core implementation:** Core supports SCOR4 P16=1 explicit endpoints. P8 is ignored and P17/P18 are horizontal SCORE-unit displacements, not number offsets.


**Verified original taper:** SCORE 3.11 and SCOR4 use the measured fixed 85% thickening ramp and ignore P19. The speculative P19 description in Tom’s reference is retained as historical documentation. Adjustable taper is an SMX extension, not original SCOR4 behavior.
