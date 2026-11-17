# Code 1 · Notes

## In SCORE Modern

P5 packs stem direction, accidental, parentheses and horizontal displacement into one value. For example, `13` gives an up-stem with a natural and `123` gives a down-stem with a parenthesized natural. Use the parameter table to identify a field, then read its detailed controls before editing.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 1 -- Notes -- Edited by Thomas M. Brodhead parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horizontal pos. |
| `P4` | vertical pos. |
| `P5` | stem-accidental |

| Parameter | Meaning |
| --- | --- |
| `P6` | note head type |
| `P7` | rhythmic time value |
| `P8` | stem length |
| `P9` | dots-tails |

| Parameter | Meaning |
| --- | --- |
| `P10` | horz. offset |
| `P11` | marks |
| `P12` | staff above-below |
| `P13` | mrk, horz. |
| `P14` | mrk, vert. |

| Parameter | Meaning |
| --- | --- |
| `P15` | note size factor |
| `P16` | ledg.ln.thick |
| `P17` | stem thick./origin |
| `P18` | mark size |

```text
╔════════════════╦═══════╦═══════╦═══════╦═══════╗
║       p19      ║  p20  ║  p21  ║  p22  ║  p23  ║
║ aug. dot vert. ║  temp. value for enh. change  ║
╚════════════════╩═══════╩═══════╩═══════╩═══════╝
```

=============== CODE 1 creates: =============== * Notes

P4 = vertical location.  For grace (cue-sized) notes add 100 to P4 (or -100 if P4 is negative.)  This is equivalent to p15 = .6

P5 = 3-digit number integer + decimal XYZ.# which does quadruple duty.

The one's digit (digit Z) indicates the note's accidental.

```text
0 = none
1 = flat           (=LIBRA symbol 7)   ...i.e., to modify these symbols,
2 = sharp          (=LIBRA symbol 8)   locate them in the named libraries
3 = natural        (=LIBRA symbol 9)   in the indicated 1-10 positions.
4 = double flat    (=LIBRA symbol 10)  E.g., the quarter-tone accidentals
5 = double sharp   (=LIBRB symbol 1)   could be replaced with normal acci-
6 = 1/4 tone flat  (=LIBRO symbol 6)   dentals in thin editorial brackets,
7 = 3/4 tone flat  (=LIBRO symbol 7)   etc. The names of the symbols in
8 = 1/4 tone sharp (=LIBRO symbol 8)   DRW files are arbitrary ("noir")
9 = 3/4 tone sharp (=LIBRO symbol 9)   and could be changed to anything.
```

```text
The hundred's digit (digit X) indicates a parenthetical accidental:
100 = no accidental
101 = parenthetical flat         (=LIBRO symbol 1)
102 = parenthetical sharp        (=LIBRO symbol 2)
103 = parenthetical natural      (=LIBRO symbol 3)
104 = parenthetical double flat  (=LIBRO symbol 4)
105 = parenthetical double sharp (=LIBRO symbol 5)
```

106-109 function the same as 6-9, using the same symbols from LIBRO.

The ten's digit (digit Y) indicates which direction the stem will go.

```text
1 = stem up
2 = stem down
0 = no stem
```

```text
For example:
P5 = 13: stem up, natural sign on note
P5 = 21: stem down, flat on note
P5 = 102: stemless note with a parenthetical sharp
P5 = 123: downstemmed note with parenthetical natural
```

Accidentals are displaced to the LEFT using the decimal values of P5. The decimal value is multiplied by 10 to get the actual displacement, relative to the note (i.e. the spacing is NOT absolute and therefore scales with changes in staff size).

If the 1st digit after the decimal point is 9, the remaining digits X 10 gives the displacement to the RIGHT.

For example:

```text
P5 = 12.25 = stem up, sharp displaced 2.5 steps to LEFT of normal.
P5 = 12.935 = stem up, sharp displaced .35 steps to RIGHT of normal.
```

--> Editing shortcut: To change P5 accidental, select note and enter:

```text
FLT = flat
SHP = sharp
NAT = natural
NAC = no accidental
```

--> Editing shortcut: With note selected, enter FLI to flip the --> note's stem to the opposite direction.

--> ENHarmonic changes: Type ENH n, where n = 1 or -1. If n = 1, the note moves UP 1 step and a sharp is changed to a flat. If n = -1, the note moves DOWN 1 step and a flat is changed to a sharp. If NO number is given, the proper change is assumed. ENH does NOT see key signatures or previous accidentals. See p20-23 below.

```text
P6 = note head types.
   = -1: invisible/headless
   =  0: black                           = 130-138: Parallelogram
   =  1: half                              Renaissance noteheads
   =  2: whole
   =  3: breve (dbl. whole)              = 139: open x note
   =  4: white diamond
   =  5: black diamond                   = 190-197: Diamond-shaped
   =  6: X note                            Renaissance noteheads
   =  7: invisible/headless
   =  8: diamond/x
   =  9: oval/x
```

--> Add 10 to P6=0-9 to put those note heads in parentheses. --> Any DRW symbol in LIBRC.DRW or a higher library (i.e. Code9 p5=20+) may be used as a note head; set Code1 P6 = Code9 P5=20+.

P7 holds rhythmic time value (ALWAYS needed for automatic spacing systems).

For a normal quarter note P7=1; for an eighth, P7=.5; for a half, P7=2; for a whole note, P7=4 (i.e. 4 times as long as a quarter note). An eighth note triplet = .3333, or 1/3 of the time of a quarter.

Negative values in P7 are used for special situations.

```text
P7 = -1 = ignore note when justifying.
     -2 = suppress printing of ledger lines no matter what vert. pos.
     -3 = do both of the above.
```

P7 = 64, 65 = grace note

P8 holds stem length information.  P8 = 0 = normal stem (1 octave up or down). Positive values in P8 extend the stem.  P8 = 1.5 = stem is 1.5 scale steps longer than normal.  Negative values shorten the stem.

To add a slash through the tail of a grace note (where p4 <= -100 or 100 <= p4), add 100 to p8 (when p8 is positive) or subtract 100 to p8 (when p8 is negative).

P9 can be a 2-digit number, with the left digit indicating the number of DOTS which are to follow the note, and the right digit indicating the number of TAILS (flags) to be added to the stem.  Numbers to the right of P9's decimal point will cause the dot(s) to be displaced to the right.  The program multiplies this part of P9 by 10 to get the displacement value. --> If the 1st digit after the decimal point is 9, the remaining digits X 10 gives the displacement to the LEFT.

```text
P9 = 12 = note with 1 dot and 2 tails.
P9 = 20.31 = note with 2 dots displaced to right by 3.1 units, no tails.
P9 = 10.94 = note with 1 dot displaced to LEFT by .4, no tails.
```

When 100 is added to P9 the dot following a note which appears ON a staff line will appear below that line instead of in the normal position above the line.  Adding 1000 to P9 displaces dots one note width to the right.

P10 is used to displace the note head from its normal position (given in P3). This is usually used for chord structures which include the close interval of the second.  If P10 = exactly 10, the note is displaced 1 note width to the right.  If P10 = exactly 20, the displacement is 1 note width to the left.  Any other numbers will displace the note in terms of horizontal space units.

P11 holds information about the marks which are often attached to note heads. [See below re. mark displacement (P13, P14) and mark size (P18).]

```text
 1 = musica ficta flat     13 = harmonic (o)         25 = accent-tenuto
 2 =   "      "   sharp    14 = fermata              26 = double stac. ..
 3 =   "      "   natural  15 = arsis sign           27 = triple  "    ...
 4 = wedge                 16 = thesis sign          28 = quad.   "    ....
 5 = accent                17 = mordant              29 = trill
 6 = tenuto (dash)         18 = inverted mordant     30 = fingering 0
 7 = staccato              19 = heavy wedge          31 =     "     1
 8 = wedge-staccato        20 = trill                32 =     "     2
 9 = accent-staccato       21 =   "   with flat      33 =     "     3
10 = tenuto-staccato       22 =   "     "  sharp     34 =     "     4
11 = down bow              23 =   "     "  natural   35 =     "     5
12 = up bow                24 = plus (+)             36 =     "     6
```

Other marks may be positioned near notes using Code 9 or Code 11.

--> Numbers created by p30 and up default to the font specified by _00 and have a p6 size = .7 --> Any number greater than 30 in p11 creates the digit (p11 - 30) (e.g. p11=55 creates the number 25, because 55 - 30 = 25).

P11 = 99 makes heavy line extending to right of note head. P13 sets how many steps to start of heavy line. (P13 default = 2.3) P14 sets total distance from front edge of note to end of heavy line. (P14 default = 10.)

During input mode, the p11 marks can be specified using the following letter commands:

```text
FIF     - musica ficta, flat    (p11=1)
FIS     - musica ficta, sharp   (p11=2)
FIN     - musica ficta, natural (p11=3)
W       - wedge                 (p11=4)
A       - accent                (p11=5)
T       - tenuto                (p11=6)
S       - staccato              (p11=7)
WS      - wedge staccato        (p11=8)
AS      - accent staccato       (p11=9)
TS      - tenuto staccato       (p11=10)
D       - downbow               (p11=11)
U       - upbow                 (p11=12)
H       - harmonic              (p11=13)
FE      - fermata               (p11=14)
```

```text
AR      - arsis                 (p11=15)
TH      - thesis                (p11=16)
MO      - mordant               (p11=17)
IM      - inverted mordant      (p11=18)
HW      - heavy wedge           (p11=19)
TR      - trill                 (p11=20)
TRF     - trill with flat       (p11=21)
TRS     - trill with sharp      (p11=22)
TRN     - trill with natural    (p11=23)
PL      - plus                  (p11=24)
ACT     - accent tenuto         (p11=25)
S2      - 2 dots                (p11=26)
S3      - 3 dots                (p11=27)
S4      - 4 dots                (p11=28)
F#      - fingering             (p11=30 + #)
```

Normally, most marks appear on the note-head side of the note-stem combination.  For these marks the use of a negative value in P11 will force the mark to appear on the stem side, regardless of the stem direction as set in P5.

```text
P5 = 10, P11 = 32 = stem up, fingering number 2 below note head.
P5 = 10, P11 = -32 = stem up, number 2 above note's stem.
```

Numbers to the right of P11's decimal point will cause the mark to be displaced vertically.  The program multiplies this part of P11 by 10 to get the displacement value in scale steps. (P14 may also be used for this.)

```text
P11 = 5.13 = accent over note, displaced by 1.3 scale steps.
P5 = 20, P11 = -34.2 = stem down, fingering number 4 pushed 2 steps
                        below end of note stem.
```

P12 = Staff displacement, where P12=1 displaces the note to the staff ABOVE and P12=2 displaces the note to the staff BELOW. This is essential for cross-staff beamed notes in keyboard music.

--> P4= 4, P12= 1 = note appears in bottom space of staff above.

P13 = horizontal mark displacement (+/-) in note-relative units. If P11 = 7 and P13 = 99, the mark will be centered on the note stem. (In this case 99 is a temporary value that is a signal to SCORE to center the staccato; it is immediately replaced with the true, needed value.)

--> If you need to center another mark (e.g. P11=5), temporarily change P11 to 7 or -7, set P13 = 99 to get the correct centering value, and then change P11 back to the desired mark.

P14 = vertical mark displacement in scale steps (+/-).

--> When fingerings are specified in p11 (i.e. P11=30+), p14 will separate multiple digits in numbers 10 and greater. A negative number in p14 in this case will displace the number down and to the left, but it inverts the number order (e.g. 98 becomes 89) as well as separating the digits.

P15 = note size factor (e.g. P15=.75 creates a note that is 75% of the normal size, relative to the associated staff size.)

--> A negative number creates an inverted mirror image of the note at the size specified (i.e. the absolute value of p15).

P16 = Ledger line thickness (on print only; not reproduced on screen). The default situation is that ledger lines will be 2 pixels thicker than staff lines.  (Staff line thickness depends on the basic line thickness given when using the PRint command.  However the thickness of staff lines can also be influenced by a staff's P11.  See Code 8 help.) Mini-notes (P4>100) will normally have only one pixel added to ledger line thickness. The value in P16 will add or subtract from the line thickness given with the PRint command, WITHOUT REGARD to staff line thickness. If P16 = 99 then the note's ledger lines will be exactly the same thickness as the staff lines.  (This is for very small staff sizes.) If P16 is not zero, all defaults are overridden.

P17 = extension of SOURCE of note stem - in + or - scale steps from P4 pos. Add 100s for thicker stems -- 100=add 1 pixel, 200=add 2, etc.

P18 = change size of marks (P11) from default. --> A negative number creates an inverted mirror image at various (increasing) sizes. (Multiple digits specified with p11=40+ are not separated, as will happen with negative values in p14.)

p19 = vertical shift of augmentation dot in p4 units.

p20-p23 = temporary value that produces enharmonic respelling of note. Use p20 for music in treble clef. Use p21 for music in bass clef. Use p22 for music in alto clef. Use p23 for music in tenor clef. --> Set p2# = 1 to move the note to a higher p4 position with an accidental that indicates a pitch equivalent to the original pitch. --> Set p2# = -1 to move the note to a lower p4 position with an accidental that indicates a pitch equivalent to the original pitch.
