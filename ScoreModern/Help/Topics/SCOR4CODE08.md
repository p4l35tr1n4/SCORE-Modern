# Code 8 · Staves · SCOR4

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

```text
  parameter structure
╔═══════════╦════════════╦══════════════════════╦═══════════════╦══════╗
║    p1     ║     p2     ║          p3          ║      p4       ║  p5  ║
║ code num. ║ staff num. ║ left horizontal pos. ║ vertical pos. ║ size ║
╚════ 8 ════╩════════════╩══════════════════════╩═══════════════╩══════╝
```

```text
╔════════════╦══════════════╦═══════════════╦═════════════╦═══════════════════╗
║     p6     ║      p7      ║      p8       ║      p9     ║        p10        ║
║ right pos. ║  # of lines  ║ space to next ║ instr. num. ║ inches from stf.1 ║
║            ║ or invisible ║  tiled file   ║             ║                   ║
╚════════════╩══════════════╩═══════════════╩═════════════╩═══════════════════╝
```

```text
╔════════════════╦═════════════════════╗
║       p11      ║         p12         ║
║ line thickness ║ display or suppress ║
║                ║   staff p2 number   ║
╚════════════════╩═════════════════════╝
```

Code 8 creates:

- Staves

P2 = staff number.  Staves are numbered from the bottom of the page upward. Up to 32 staves may be used in 1 file.  The default staff positions, at size 1, space the staves 18 scale steps apart.

If more than 32 staves are needed, or the amount of parametric or graphical information what SCORE can accommodate in a single file, then the page will have to be "tiled" by printing from two or more individual files.  The value in p8 sets the location of the file that is printed ABOVE a given file, and the file names must DESCEND from the top down (hence for a 3-file tiled page, the top file might be named X01A.MUS, the middle file would then be named X01B.MUS, and the bottom file would be named X01C.MUS).  See the documentation on P8 below.

```text
P3 = starting horizontal location of staff
P4 = vertical displacement of staff in units correlative to the staff size
     (i.e. p4=12 is twice the vertical displacement when the staff size is
     p5=1 than when p5=.5).  P4 in inches is (p4 * ((p5 * .35) / 8)).
     P4 in millimeters is (p4 * ((p5 * 8.8888889) / 8)).
```

P5 = sets the size of the space between staff lines.  Each space is made up of 2 "scale steps."

The vertical height of a 5-line staff in inches is P5 * .35 The vertical height of a 5-line staff in millimeters is P5 * 8.8888889

The staff may be moved up or down from its "normal" position by putting a value in P4.  All P4 values are scale steps relative to the staff size expressed in P5.

It is essential that Staff 1 (i.e. p2=1) has a p4 value of 0 *ALWAYS*. There are techniques involving the creation of an invisible staff 1 if there must be a low item on the lowest visible staff that would dip below the "footer" or lowest allowable graphical item on the page; this is covered in detail in the discussion of P7 below.  Just remember to maintain p4=0 on staff 1 of each file.

When P5=0, P5 default to 1.

If you need to reproduce historical "rastral" staff sizes, use this chart:

```text
    Rastral Size    (mm)    P5              Historical Usage
```

### ====    =========

```text
    0               9.2     1.0348706       educational music
    1               7.9     0.8886389       "           "
    2               7.4     0.8323959       piano music, songs
    3               7.0     0.7874015       "            "     parts
    4               6.5     0.7311586       "            "     "
    5               6.0     0.6749156       "            "     "
    6               5.5     0.6186726       choral music, cues, ossias
    7               4.8     0.5399325       "             "     "
    8               3.7     0.4161979       full scores
```

P6 = Horizontal endpoint of staff. If P6=0, it defaults to P6=200.

```text
P7 = Number of lines in staff and origin of bottommost staff line.
     P7 may be used to create staves with other than 5 lines.  For example:
     P7=6: a six-line staff
     P7=3: a three-line staff
     P7=5: a normal, five-line staff (which is the default of p7=0).
```

In P7, the bottom line of the non-standard staff will in the same position as that of a regular staff, unless a three-digit number (XYZ) is used. In this case, the left digit (X) tells which line of a normal staff will be the new bottom line. YZ then holds the number of staff lines to appear.

*--> Thus, to create a single line percussion staff, set p7=301. This creates a single staff line (01) starting on the 3rd line (3). --> To create a 4 line staff starting on the 5th line, set p7=504, etc.*

If P7 = -1, the staff is invisible.  This can be useful in many ways. In order to keep staff 1 at p4=0 when there are extremely low items on staff 1 that would dip below a fixed "footer" (where the name of the piece or a stock number is always at e.g. p4 = -20), move all the staves up by 1 using the MS command, then create an invisible staff 1 with p7 = -1 and p4 = 0, move the "footer" to staff 1, and then move or VJ the staves so that the first visible staff (now staff 2) has its lowest item ABOVE the footer (now) on staff 1. This will allow you to maintain staff 1 at p4 = 0 on that page; the same technique can be applied to any subsequent page where item(s) would dip below the level of the "footer".

P8 = creates a non-printing "line" used by the SCORLAS program to create space (in the measurement units in use) between the bottom line of staff 1 in one file up to the bottom line of staff 1 in the next file above.  This is used when a page of music is made up of several separate "tiled" files when there is too much parametric data for the page to fit in a single file.

When INCH is the measurement unit, the value in p8 expresses inches. When METR is the measurement unit, the value in p8 expresses centimeters

*--> P8 should be set on STAFF 1 of each affected file.  Do NOT set P8 --> on other staves in the file (the other staves should have P8=0).*

If you are viewing a file with a p8 "line" and you switch between INCH and METR, the *value* of p8 stays constant, but the location of the non-printing line shifts to the location indicated by the measurement unit.  So if INCH is in force and you enter METR, the p8 line will shift *downward* (because a centimeter is less than an inch), and if METR is in force and you enter INCH, the p8 line will shift *upward* (because an inch is greater than a centimeter).

If you use the COM command to combine 2 or more tiled files into a single file, the location of the bottommost staff of each successive file will correspond to the location of the p8 "lines" of the lower files. Let's say you have 3 tiled files named X01A.MUS, X01B.MUS, and X01C.MUS. X01B.MUS has its staff 1 p8=4.25 and X01C.MUS has its staff 1 p8=5. If you then combine the three files using this command sequence in SCORE:

COM X01A 3

...then the X01B file materials will be at 5 units (inches or centimeters, depending on the units in force) above the X01C file (i.e., the bottom staffline of the X01B file will be 5 units above the bottom staffline of the X01C file), and the X01A file materials will begin 4.25 units above the bottom staff of the X01C material.

P9 = instrument identification number, used by the part extractor in PAGE.

P10 = distance, in inches, for placement of staff above normal position of staff 1.  When P10 is used, P4 is ignored.  (However, for staff 1 itself, it is best to leave both P4 and P10 at 0.)

P11 = staff line thickness.

The default thickness of staff lines will normally depend on the thickness number given when using the PRint command.  However, for P5 staff sizes less than .65, the line thickness will be decreased by 1 pixel.  However, a value in p11 will add or subtract pixels from the given PRint line thickness, regardless of what the staff size is.

The default thickness of a note's ledger lines is related to the thickness of the staff lines. (See Code 1 help.)

*--> If the staff's p11 is set to 99, then the staff lines AND the ledger lines of notes on the staff will all be the thickness given in the PRint command. However, if a note has a value in its P16, this will override the staff's p11.*

P12 - Display (0) or suppress (1) the non-printing staff number to the RIGHT of the staff. While P12 = 0, the number of the staff will appear at the far left side of the screen.  This number will NEVER appear in the printed music. If P12 is set to 1, this number will not appear on the screen.
