# Code 12 · Shapes and guitar grids

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 12 -- rectangles, circles, guitar chord grid --

Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | shape code |

```text
╔════════════╦════════════╦═════════╦══════════════╗
║     p6     ║     p7     ║  p8-p14 ║     p15      ║
║ horz. size ║ vert. size ║ various ║ horz. offset ║
╚════════════╩════════════╩═════════╩══════════════╝
```

```text
╔════════════╦═════════════╦══════════════════╦════════════╗
║        FOR GUITAR CHORD GRIDS OF BOTH CATEGORIES         ║
║     p16    ║     p17     ║       p18        ║     p19    ║
║ chord name ║ grid aspect ║ bottom fingering ║ chord name ║
║ type size  ║    ratio    ║      number      ║    vert.   ║
╚════════════╩═════════════╩══════════════════╩════════════╝
```

================ Code 12 creates: ================ * Rectangles * Circles * Guitar Grids * Guitar Bends * Accordion Button Symbols

========== Rectangles ==========

BASIC PARAMETRIC DISTINCTION: Rectangles have p5=0

```text
P5 = 0 (for rectangle)
P6 = horizontal size (in scale steps)
P7 = vertical size.
--> If P7 = 0 then P7 = P6.
--> Negative values in P6 and P7 have no effect; treated as positive values
P8 = Outline thickness
```

```text
P9 = clockwise rotation in degrees.
P10 = apparently a shear transformation; positive value pulls shape to right,
    negative value pulls shape to left (thus creating a parallelogram).
P13 = 1 --> Fill shape in black
```

======= Circles ======= BASIC PARAMETRIC DISTINCTION: Circles have p5=1

```text
p5 = 1 (for circle)
p6 = horizontal size in scale steps.
P7 = vertical size.
--> If P7 = 0 then P7 = P6.
P8 = Outline thickness
P9 = clockwise rotation in degrees.
P10 = beginning point in degrees for start of arc
P11 = ending point for arc
P12 = sets the point increment.  Normally there are 360 points.
      When P12 = some number which divides evenly in 360 an equal-sided
      polygon is created
```

--> P12 = 72: forms a pentagon, because 360/72 = 5 --> With circles of small radius some time is saved if P12 = 2 or 3, thereby producing many fewer points.

P13 = 1 --> Fill shape in black

================== Guitar Chord Grids ================== BASIC PARAMETRIC DISTINCTION: Guitar Chord Grids have -1 <= p5 and p5 <= -6

There two subcategories: 1) Grids where -1 <= p5 and p5 <= -3 2) Grids where -4 <= p5 and p5 <= -6

***** GUITAR CHORD GRID SUBCATEGORY 1 *****

```text
P5 = -1 = grid with nut (heavy bar) on top
     -2 =  "   without nut
     -3 = no grid - prints chord name only
P6 = Size
```

```text
P7 = 2-digit number XY specifying chord name above the grid.
     X sets the chord root letter:
        1=C,  2=D,  3=E,  4=F,  5=G,  6=A,  7=B.
     Y sets the accidental:
        0=none,  1=flat,  2=sharp.
```

```text
P8 = sets the chord type:
     0=none (=major)
     1=major
     2=minor
     3=circle(=diminished)
     4=slashed circle (=1/2 diminished)
     5=+ (=augmented)
     6=diminished
     7=augmented
```

```text
P9 = 3 digit number XYZ setting an added interval number
     with a preceding accidental:
     X is the accidental:
        0=none
        1=flat
        2=#
        3=natural
```

YZ = is the 2-digit interval. --> For example, to get #6, P8=206, etc.

```text
P10 = 4 digit number ABCD specifying other items
      A = 1 inserts the letters "sus"
        = 2 makes "add"
        = 3 makes "(add . .)"
      B = 1 inserts a flat
        = 2 inserts a sharp
        = 3 inserts a natural
      CD = adds an interval number. (Use leading zero if less than 10.)
```

```text
P11 = 4 digit number WXYZ for "/" followed by a note letter and accidental,
      or accidental and number.
      W = note name. (1=C, 2=D, ... 7=B)
      X = the accidental:
        = 0:none
        = 1:flat
        = 2:sharp
        = 3:natural.
      YZ = a number.
      ---> For example, P11 = 4200: "/F#"
                        P11 = 213: "/#13"
```

```text
P12 = 4-digit number WXYZ which set horizontal "barre"
      and special fret number.
      WX = range of strings under the barre.
      YZ = fret number placed to right of grid.
```

--> The strings are numbered from left (1) to right (6) --> The "fr" indication may be displaced to other fret levels --> by adding a number after the decimal point.

P13 = six-digit number ABCDEF specifying dots on the guitar chord grid where (P5=-1 or -2). Each number refers to a string. A refers to the lowest string F refers to the highest string The numbers used in each of these positions will refer to a fret (up to the 7th fret) to be fingered. The number 8 will put an open circle above the grid. The number 9 will put an X above the grid. --> P13 = 932818: --> String 1 = X --> String 2 = dot on fret 3 --> String 3 = dot on fret 2 --> String 4 = open circle above

--> String 5 = dot on fret 1 --> String 6 = open circle above

```text
P14 = Two-digit number XY for special grid formations.
      X = number of strings (vertical lines).
      Y = number of frets (horizontal lines).
```

P15 = Horizontal offset from given P3 position (for the entire object).

P16 = changes the type size of the top chord name only, using the form WXY.ZZ, where WX can be any valid font number and Y.ZZ is the text size factor (where 0=1).  So 210.8 = font 21, size .8

```text
P17 = grid's aspect ratio.
    = 0 or 1: square,
    > 1: extend the width.
```

P18 = Up to 6-digit number specifying fingering numbers at bottom of grid. ALL 6 DIGITS MUST BE ACCOUNTED FOR Possible numbers: 0,1,2,3,4 and 7,8,9 --> 7 = open space --> 8 or 0 = 0 --> 9 = "T"(thumb)

P19 = Vertical shift of the position of the top chord name.

***** GUITAR CHORD GRID SUBCATEGORY 2 *****

```text
p5 = -4 = chord name and fingering from GUITAR.DAT file.
     -5 =   "    "   only --------   "    "         "
     -6 = grid and fingering only    "    "         "
```

```text
P7 = 2-digit number AB
     A = letter name: 1=C, 2=D,... 7=B.
     B = accidental: 1=flat, 2=sharp
```

```text
P8 = the chord type number, 1-20.
    1 = major
    2 = minor
    3 = circle(dim7)
    4 = slashed circle(1/2 dim7)
    5 = + (aug)
    6 = 6
    7 = 7
    8 = maj7
    9 = 9
```

```text
10 = m6
11 = m7
12 = sus4 (11th)
13 = 13
14 = 7 b5
15 = min,Maj7
16 = 7 +5
17 = min9
18 = 7 b9
19 = 7 #9
20 = 5
(User may create different names.)
```

P9 = 0-4 indicating fingering alternative. --> Example: To get the chord grid for C#m7, 1st alternative fingering: --> P7=12, P8=11, P9=1 --> [here 1=C, 2=Sharp]  [11=m7 chord]

(P10 - P13 not used when P5=-4, -5, -6.)

```text
P14 = Two-digit number XY for special grid formations.
      X = number of strings (vertical lines).
      Y = number of frets (horizontal lines).
```

P15 = Horizontal offset from given P3 position (for the entire object.)

P16 = changes the type size of the top chord name only, using the form WXY.ZZ, where WX can be any valid font number and Y.ZZ is the text size factor (where 0=1).  So 210.8 = font 21, size .8

```text
P17 = grid's aspect ratio.
    = 0 or 1: square,
    > 1: extend the width.
```

P18 = Up to 6-digit number specifying fingering numbers at bottom of grid. --> 9 = "T"(thumb) --> 0 = leaves an open space

--> To suppress any fingerings that may have been set in GUITAR.DAT, --> set p18=666666

P19 = Vertical shift of the position of the top chord name.

============ Guitar Bends ============

BASIC PARAMETRIC DISTINCTION: Guitar Bends have p5=2

```text
P5 = 2 (curved lines with arrow head)
P6 = horizontal size factors of arc.
P7 = vertical size factors of arc.
P8 = line thickness.
P9 = rotation.
P10 = 0 = curve; = 1 = straight line.
P11 = curve factor. 0 = most curvature; 90 = least.
P12 = horizontal size of arrow head.
    --> -1 = no arrow head.
P13 = vertical size factors of arrow head.
```

================= According Buttons ================= BASIC PARAMETRIC DISTINCTION: Accordion Buttons have p5=10

```text
P5 = 10 (to make accordion button)
P6 = size of circle (0 = default size 1).
```

```text
P7 = Circle division number
     --> 0: (default) divides the circle in 3 parts
     --> 2: divides circle in 2 parts
     --> 3: divides circle in 3 parts
     --> 4: divides circle in 4 parts
P8 = vertical dot pattern, depending on P7 value.
     --> The number of circle divisions determines the number of digits
     -->
     --> The LEFTMOST digit is the LOWEST dot
     -->
     --> If p7 = 2, then p8 is a two-digit number 00, 01, 10, or 11:
     --> 00: No dots
     --> 10: Dot on bottom
     --> 01: Dot on top
     --> 11: Dot on bottom and top
     -->
     --> If p7 = 3, then p8 is a three-digit number 000, 001, 010, 011
     --> 100, 101, 110, 111
     -->
     --> 100: Dot on bottom
     --> 001: Dot on top, etc.
     -->
     --> Likewise for 4 digits
```

```text
P9 = horizontal dots (center row only)
     001 = dot to right
     100 = dot to left.
     101 = dots on left and right
     --> NOTE: 0 is *ALWAYS* used for the middle digit, as the "central"
     --> dot would only be set by p8
```

P10 = sets thickness of outer circle.

============ TAB NOTATION ============ To convert music to TAB notation, Enter TAB plus up to 8 numbers: 1: Staff of music to be converted to TAB notation 2: Number of desired strings (0 defaults to 6; 4 assumes bass clef notation) 3-8: Special tunings for strings 1-6

For example: To convert staff 5 to 6-string tablature, with string 3 tuned UP 2 half steps and string 6 tuned down 2 half steps, enter:

TAB 5 0 0 0 2 0 0 -2

To automatically convert notes to TAB numbers, use the group alter command to set P6 of the notes on the staff to the number of strings in the TAB staff * -1.  So if there are 7 strings:

```text
a
99 0 0 1
6 -7
```

The possible values for P6 here are therefore -4, -5, -6, -7, -8, corresponding to 4, 5, 6, 7, and 8 line staves.

This procedures does not read key signatures, so accidentals MUST be present on each of the notes on the staff.

P8 indicates the string number of the fingering, with 0 being the default. Alternate string positions may be indicated by editing P8 appropriately (so that if the number is to appear on the second string down, then p8=2).

For Bass Guitar, however, set p14 = -1X, where X = the number of strings. (The first digit, 1, indicates bass clef; the second number indicates the number of strings, so for 5 strings set p14 = -15, etc.)

P16-P19 = indicate special tunings of the strings in the form ABCD. In P16, AB = tuning of string 1; CD = tuning of string 2. In P17, AB = tuning of string 3; CD = tuning of string 4. In P18, AB = tuning of string 5; CD = tuning of string 6. In P19, AB = tuning of string 7; CD = tuning of string 8.

```text
Tuning indicates the number of half steps up or down from the standard tuning:
    01 = 1 half step up, 02 = 2 half steps up
    21 = 1 half step down, 22 = 2 half steps down, etc.
```

Example: Guitar tuned for A Major. The 2nd, 3rd, and 4th striong are each tuned 2 half steps up. Setting P16=0002 and P17=0202 will produce the right fret numbers

Bass Guitar tuned for flat keys: P16=0101 and P17=2121 are needed for this tuning.

**** OCTAVE HIGHER, TWO OCTAVES HIGHER ****

If the original notes are under 8va or 15ma signs, then precede the P6 number by 8 or 15 respectively, e.g. p6 = -86, p6 = -156 (normal would be p6 = -6).

**** TABLATURE TYPE FONT ****

The default font is Helvetica-Bold.  To specify a different font, set P12 = the FM##.PSC number, so to specify the _25 font, set P12=25.

To enclose the numbers in parentheses, add 100 to p12.  P12=100 adds parentheses around the default Helvetica-Bold font; P12=125 would add parentheses around the _25 font.

**** X Notes, Tied Notes ****

When P9 = -1, an "x" appears as the tablature number. If P8 = -1, the text "(TIE)" will appear in place of the tablature number ON THE SCREEN ONLY.  This will NOT appear when printed.  This is useful to account for tied notes in the original input.

**** The CAPO Bar ****

P19 cab set the fret position of the capo bar.  In the automatic mode (p6 = -6, etc.), the string level and fingering number will shift according to the CAPO position. For example, if P19=3, the capo bar is set at 3rd fret.

--> SCORE knows that P19 is not involved in a special tuning when the given --> value is less than 100.

**** TP (Tab fingering Position) ****

While editing TAB notes, typing TP n will cause the fret number to be no lower than n.  Thus, if a passage were to be played in 5th position, editing each note with TP 5 would cause the fret numbers to be 5 or higher.

Use the group alter command "A" to effect an entire line, specifying TP instead of a parameter change, like this:

```text
a
1 0 0 8 5
TP
```

**** TAB and LJ ****

If you need to LJ a TABbed line, remove the TAB clef FIRST, then LJ, then restore the TAB clef.  SCORE's LJ will typically create too much space for the TAB clef, and therefore it will require removal before LJ and restoration afterwards.

**** Non-automatic Tablature ****

If P6 is set to -66 (or -44, etc.), all information about string number and fret number must be specifically entered in P8 and P9.  In this case, P4, P5, P16-P19 will all be ignored.

**** Tablature Letters ****

If P6 is set to -66 (or -44, etc.) and P8 is greater than 0, P9 may be used to print the letters of the alphabet, using the same system for P5 of Code10 when lettered rehearsal numbers are specified (P9=1001->A ; P9=1002->B; etc.)

To get lowercase letters, use 2000s instead of 1000s (P9=2001->a ; P9=2002->b)

The font may be changed with P12, as described above.

P13 = rotation of the letter or number.

P14 = Vertical location.

P15 = Size control.

GUITAR.DAT FORMAT: Data for fingerings of up to 5 different positions of 20 different chord types for each chromatic scale note.  File allows 20 lines of numbers for each chromatic note (20x12=240 lines). Each line = chord name + semicolon, a chord ID, up to 5 sets of 3 numbers indicating 5 possible fingerings, + final ;. Best to maintain the identity numbers' order given in the file. E.g.:

```text
C;  1    x32o1o 0 732717    x35553 3500 713331    133211 1608 134211  ...;
Cm; 2    x3554x 0 713427    133111 1608 134111    x31o1x 0    741727  ...;
```

The chord name can be any text string but is LIMITED TO 20 CHARACTERS, including font indications.  For example:

_05B?1?07\E?1; = Bb°7/Eb

```text
Notice these font codes for special characters:
?0 = °   ?9 = φ (superscript circle w/slash)   ?8 =  (unfilled triangle)
```

First number after chord name and semicolon is the chord ID. The numbers are arbitrary and could be used for any chord name. Chord fingerings = exactly 6 characters.  Fret numbers 1-6 are used in addition to x (=don't play) and o (=open string). Number 0 leaves that string with no mark. Note that this fret ID system is similar to P13, except here use "x" and "o" in place of 9 and 8.
