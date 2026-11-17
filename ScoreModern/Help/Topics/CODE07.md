# Code 7 · Trills, ottava and pedal marks

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 7 -- Trills, Ottava, Pedal marks -- Edited by Thomas M. Brodhead parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horizontal pos. |
| `P4` | vertical pos. |
| `P5` | size, other |

| Parameter | Meaning |
| --- | --- |
| `P6` | right end of line, etc. |
| `P7` | symbol type |
| `P8` | various |
| `P9` | pedal or wiggle size |

```text
 ╔══════════════════╦══════════════╦═════════╦════════════╦══════════════╗
 ║        p10       ║     p11      ║   p12   ║    p13     ║     p15      ║
 ║  dash thickness  ║ wiggle vert. ║  arrow, ║  rotation  ║ horz. offset ║
 ║ harp pedal angle ║ left brack.  ║  brack. ║ (tr., 8va) ║  (tr., 8va)  ║      ║              ║
 ╚══════════════════╩══════════════╩═════════╩════════════╩══════════════╝
===============
CODE 7 creates:
===============
 * Trills
 * Ottavas
 * Graphic Harp Pedal Signs
 * Score's Unique Piano Pedal Sign
```

====== Trills ====== BASIC PARAMETRIC DISTINCTION: Trills have p7=0

```text
P5 = size
P6 = right end of wiggle (= 0 = no wiggle)
P8 = accidental above trill (1 = flat, 2 = sharp, 3 = natural)
P9 = wiggle size (default = .75)
--> Negative p9 numbers effect default size but shift the wiggle vertically
p11 = Vertical displacement of wiggle
p12 = 1 = Adds arrow to end of wiggle
p13 = rotation of wiggle
P15 = horizontal offset from given p3 position.
```

The four Code7 trills (p8=0, 1, 2, 3) can be input during the MARKS stage of INPUT with these commands, specifying start and end points:

TRW -  Wiggle trill with NO accidental above "tr" TRFW - Wiggle trill with FLAT above "tr" TRSW - Wiggle trill with SHARP above "tr" TRNW - Wiggle trill with NATURAL above "tr"

================================== Ottava, Ottava Bassa, Quindicesima ================================== BASIC PARAMETRIC DISTINCTION: P7= 8, -8, 15, 88, -88

```text
P5 = size
P6 = right end of dashed line
P7 = 8 (ottava),
   = -8 (ottava bassa)
   = 15 (quindicesima)
   = 88 makes ottava using "8" only. (No "va" part)
     --> Dashed line is at TOP, flush with top of "8"
   = -88 "     "      "     "   "
     --> Dashed line is at BOTTOM, flush with bottom of "8", not at top
P8 = dash size
P9 = size of space between dashes
p10 = thickness of dashes
P12 >= 0: bracket up at end of dashes
     < 0: bracket down at end of dashes
p13 = rotation of dashed line.
p15 = horizontal offset of symbol on the left end (e.g. the 8va symbol)
--> The left end remains stationary as P15 is adjusted.
```

During input mode, the Code7 Ottava symbols can be specified during the MARKS stage using these commands, specifying endpoints as with Code4 hairpins, etc:

O - 8va OB - 8ba

========================== Graphic harp pedal symbols ========================== BASIC PARAMETRIC DISTINCTION: P7 = 1

```text
P5 = size
P6 = 0
P7 = 1
P8 = LEFT 3 pedals, a 3-digit number: ABC
P9 = RIGHT 4 pedals, a 4-digit number: ABCD
--> for P8 & P9, each digit equals:
    1 = bottom pedal position
    2 = middle   "      "
    3 = top      "      "
    E.g., P8 = 111: all left pedals in bottom position, etc.
```

```text
P10 = *ANGLE* of harp pedal parallelograms
P11 = bracket on LEFT end
P12 = bracket on RIGHT end (inadvertently [?] extends middle line, as well)
P15 = angle of vertical line in symbol
```

===================================================== Graphic piano pedal symbols (Unique to SCORE program) ===================================================== BASIC PARAMETRIC DISTINCTION: P7 = 20

```text
P5 = a 3-digit number which shows which pedals should be
     depressed at any given time.  A one = pedal down; a zero = pedal up.
     001: right pedal down
     101: both left and right pedals down
     010: middle pedal down
P6 = end point for line to right (P6 = P3, or = 0 = no line)
P7 = 20
P8 = position of line extension to the left (only if P6 > 0)
P9 = pedal symbol size factor
P10 = Thickness of line connecting pedal symbols
P11 = + = bracket end on left side (using P8) (+100 = arrow)
P12 = + = bracket end on right (using P6) (+100 = arrow)
```

================= Piano pedal lines ================= BASIC PARAMETRIC DISTINCTION: P7 = 21, 22

```text
P6 = End point for line to right
P7 = 21: Creates "lift" that goes up and then down below baseline and
         then returns to baseline (__/\  ___)
                                       \/
     22: Creates (standard) "lift" that goes up and then directly down
         to the baseline (__/\__)
```

```text
P8 = p3 position of "lift"
P9 = size of "lift"
P11 = bracket on LEFT end (+ = up, - = down; +/-100 = arrow)
P12 = bracket on RIGHT end (+ = up, - = down; +/-100 = arrow)
```
