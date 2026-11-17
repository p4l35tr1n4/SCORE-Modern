# Code 18 · Time signatures

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 18 -- Meter (Time Signature) -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | top meter num. |
| `P6` | bottom num. |

| Parameter | Meaning |
| --- | --- |
| `P7` | size |
| `P8` | 2nd meter top |
| `P9` | 2nd bottom |
| `P10` | offset of + |
| `P11` | offset of 2nd meter |

| Parameter | Meaning |
| --- | --- |
| `P12` | suppress+ |
| `P13` | change type font |
| `P14` | ignore justify |
| `P15` | horz. offset |

================ CODE 18 creates: ================ * Meters

Meter numbers are taken from MUSI0.DRW file.

```text
P5 = numerator of meter
P6 = denominator of meter
```

--> To get common time, alla breve, and single meters: P5 = 99 = common time (C) = 98 = alla breve  (¢) =  0 = single meter number (found in P6) centered

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

P13 = PostScript Font Substitution. To substitute the font in FM##.PSC, set p13=10##.  For example, to get the font in FM24.PSC, set P13=1024.

P14 = 1 = suppresses recognition by LJ routine (0 = normal)

P15 = horizontal offset from P3 position.  (LJ will ignore item.)
