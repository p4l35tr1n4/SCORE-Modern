# Code 4 · Lines, arrows and hairpins · SCOR4

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
 ╔══════════╧═════════╦════════╧══════════╦════════╧════════╦══════╧════════╗
 ║         p3         ║        p4         ║        p5       ║      p6       ║
 ║left horizontal pos.║ left vertical pos.║ right vert. pos.║ rt. horz. pos.║
 ╚════════════════════╩═══════════════════╩═════════════════╩═══════════════╝
```

```text
          ╔═════════════╦═══════════╦═══════════════════╦═══════════╗
          ║     p7      ║     p8    ║        p9         ║    p10    ║
          ║ dash/wiggle ║ dash size ║ rotate/dash space ║ thickness ║
          ╚═════════════╩═══════════╩═══════════════════╩═══════════╝
```

```text
  ╔══════════════════╦═════════════════╦══════════╦═════════════╦═══════════╗
  ║       p11        ║       p12       ║   p13    ║     p14     ║    p15    ║    p18       |
  ║left vert. bracket║ right vert. brk.║ rotation ║ start cresc.║stop cresc.║
  ╚══════════════════╩═════════════════╩══════════╩═════════════╩═══════════╝
```

```text
  ╔═══════╦═════╦══════╗
  ║ Additional Offsets ║
  ║  p16  ║ p17 ║ p18  ║
  ╚═══════╩═════╩══════╝
  ╔═══════╦═════════╦═════════╦═════════╦═════════╗
  ║       ║      Multi-point lines      ║         ║
  ║ p8-p9 ║ p10-p11 ║ p12-p13 ║ p14-p15 ║ p16-p17 ║
  ║  x-y  ║   x-y   ║   x-y   ║   x-y   ║   x-y   ║
  ╚═══════╩═════════╩═════════╩═════════╩═════════╝
```

CODE 4 creates:

- Hairpins

- Straight lines

- Dashed lines

- Wavy lines (e.g. arpeggios)

- Multi-point lines (for multi-point curve fitting in DRAW)

- Lyric hyphens ...with lots of sub-controls (for the creation of brackets, arrows, etc.)

P5 and P7 determine what type of line is created:

If p5 >= 999, a hairpin is created (this trumps whatever might be set in p7). If p5 < 999, then the value of p7 determines the type of line created:

- p7 = 0 --> Normal line, bracket

- p7 = 1 --> Dashed line

- p7 = -1 or <= -3 --> Wavy "trill" line

- p7 = -2 --> Unfilled wavy line (straight line segments: /\/\/\/\ )

- p7 = 100 --> Multi-point line.

### To make a dotted line, use Code5, setting p11 less than 0 and p7=0.

### HAIRPINS (i.e. crescendo/decrescendo lines): P5 >= 999

```text
p5 >= 999
P7 =  0 = crescendo
P7 = -1 = decrescendo
```

Default spread of the sign is 2.3 scale steps.  A number other than 0 or -1 in P7 changes the spread.  (Use negative numbers for decrescendo.)

```text
P9 = rotation (in clockwise degrees)
P10 = line thickness (+ or -)
---> Thickness does NOT work when p7 = -1, but it will work for other
---> negative values (as well as all positive values). If you need a p7 = -1
---> hairpin to be thickened, change it to the equivalent spread of -2.3
p13 = rotation (in clockwise degrees)
```

*---> To erase the beginning or ending of a hairpin, thus creating ---> a partial hairpin, use p14 and p15:*

```text
P14 = where partial begins -- a percentage of the total p3-p6 length.
P15 = where partial ends -- a percentage of the total p3-p6 length.
```

(Code5 slurs employ the same parameters with the same results.)

P14 can be used to create the continuation of a crescendo hairpin from a previous line that needs to be open at the system beginning. The p3 of the hairpin must be extended to the left of the beginning of the system, and P14 must be set to "erase" the section of the hairpin leading up to the desired graphical beginning point.

P15 can be used to create decrescendo hairpin that extends beyond the end of a system and needs to be open at the system end. The p6 of the hairpin must terminate beyond the end of the system, and P15 must be set to "erase" the section of the hairpin starting at the end of the system.

To break a hairpin, where a "window" must be created to make room for another item, create 2 identical hairpins with different P14 and P15 values:

If the item is at position 100, and a hairpin runs from 80 to 120, you could create two superimposed hairpins with these values:

```text
Hairpin 1: P3=80, P6=120, P14=0,   P15=.45
Hairpin 2: P3=80, P6=120, P14=.55, P15=0
```

P14 and P15 would be adjusted for the size of the item. The P3 and P6 of the two hairpins must be identical.

```text
Additional Offsets:
If P16 = 1, then:
   P17 = absolute left offset
   P18 = absolute right offset.
Other uses of p16, p17, p18 are then not valid when p16=1.
```




### ALL OTHER LINES <

### STRAIGHT LINES: P7=0

```text
  P7 = 0
  P9 = rotation (in clockwise degrees)
```

### DASHED LINES: P7=1

```text
   P7 = 1
   P8 = dash length
   P9 = space between dashes
   P13 = rotation (in clockwise degrees)
```

*---> P9 defaults to P8 when P9 = 0*

### COMMON CONTROLS FOR STRAIGHT LINES & DASHES

```text
  P10 = added line thickness
  P11 = left vertical line
  P12 = right vertical line
```

*---> ARROWS : Add 100 to P11 or P12 for arrow heads. ---> UP/DOWN: Add 200 to P11 or P12 for lines extending both up and down.*

The height of the vertical line or spread of arrow head is the value given (+ or -) in P11 or P12.

*---> VERTICAL LINES WITHOUT BRACKETS OR ARROWS:*

- Set P6 equal to P3

- P4 = bottom

- P5 = top.

- P11, P12 = 0 always.

*---> VERTICAL LINES WITH BRACKETS OR ARROWS*

- Use P9 or P13 (as indicated for line type) to rotate item to 90 degrees

### LYRIC HYPHENS

When a Code4 dash is created as a lyric hypen, P14 indicates the number of hyphens in the dash.  This can be edited by resetting P14 to the desired number of hyphens or entering CD# where # is the desired number of hyphens.

### HORIZONTAL WIGGLES - for trills, gliss., etc.

```text
    P4 = vertical position
    P5 = 0
    P7 = -1 for normal trills and arpeggios
       = -2 for thin-line wiggles
       = -3 for triangular wiggles
    P8 = width of wiggle unit (default 1)
    P9 = height of wiggle (default 1)
    P10 = add thickness
    P12 => 100 adds arrow to right (rotate 180 degrees for arrow to left)
    P13 = rotation
    ---> P9 defaults to P8 when P9 = 0
    ---> The size of the arrow indicated with p12 is not altered with the
         value of p12; any value 100+ creates the same size arrow (small).
    ---> When P7 = -3, P11 is used to thicken ONLY the downward slope of
         the triangular pattern. (Values from .3 to .9 are good here.)
```

### VERTICAL WIGGLES - for arpeggios:

```text
    P4 = bottom
    P5 = top
    P6 = 0 (NOTICE!)
    P7 = -1
    P8 = vert. size of wiggle unit (default 1)
    P9 = horz. size (default 1)
    P12 = 1 adds arrow at top, = -1 adds arrow at bottom
    P15 = horizontal offset from given P3 position
```

*---> P9 defaults to P8 when P9 = 0 ---> To center on nearest chord, select and enter CN ---> These arpeggio items cannot be rotated!*

### Multi-point lines: P7 = 100

```text
 P8  = added horizontal point 1,  P9  = vertical point 1.
 P10 = added horizontal point 2,  P11 = vertical point 2.
 P12 = added horizontal point 3,  P13 = vertical point 3.
 P14 = added horizontal point 4,  P15 = vertical point 4.
 P16 = added horizontal point 5,  P17 = vertical point 5.
 ---> The various MOVE commands will NOT alter
 ---> these parameters automatically.
```

This feature is designed primarily to facilitate the production of unusual slur shapes, such as cross-staff slurs (e.g. Ravel's "Le Tombeau de Couperin," Minuet movement). The result is an original Code11 item.

- First draw a basic line with the overall desired slope using p3-p6, p4-p5.

- Then add internal points using the p8/p9, p10/p11, etc., pairs. --> Make the internal point wrap in the general direction of the changing --> slope of the desired slur.

- Make a note of item number of the multi-point line

- Enter DPY

- Specify the item number of the multi-point line, and save it to a text file with an easily-remembered name (like LINE.TXT).

- Start DRAW

- Read in the DPY vector file using the RE command (e.g. "RE LINE.TXT")

- Enter E to get into Edit mode

- Enter CRJ to make a curve through the points

- At the "How many points per segment?" prompt, you may have to experiment; try the suggested 8-12 points and see the result; repeat this CRJ step if necessary if the result isn't acceptable.

- Enter CPD to erase the original multi-point line

(Continued on next screen...)

- Enter THK to thicken the slur.  Here you may be able to calculate a value that will work well.  The P6 size of the resulting Code11 item that will make the symbol the right size is (.4 / StaffSize).  Provided that, if THK=2.5/P6, then the thickness of the resulting slur will match a default slur on the target staff where the slur's p13=0.

- .3 is a decent value for thinning the endpoints of the slur

- Save the resulting graphic as a Code11 item (i.e. a DRW file whose name does *NOT* begin with LIB).

- Create the Code11 item on the target staff, using P6=(.4 /StaffSize)

### COMMON CONTROLS FOR ALL CODE4 ITEMS

To move the ends of a Code4 item, press [F9] to activate the left end, or press [F10] to activate the right end.  Then make adjustments with the left and right arrow keys [<-] [->]






**Current Core implementation:** Core supports verified straight-line and hairpin P16=1 endpoint offsets in P17/P18, including native page-bounds scans and EPS.


**Unused stored fields:** Original SCOR4 tests confirm that P8 on hairpins and P15 on straight lines do not change their paths. Core/SMX preserve these stored values and ignore them during rendering and bounds measurement. They must not prevent a tiled page from opening.
