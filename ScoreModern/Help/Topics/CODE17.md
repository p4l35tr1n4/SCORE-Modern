# Code 17 · Key signatures

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 17 -- Key signatures -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | number of accidentals |

```text
╔═════════════╦══════════════╦═══════════╦════════════════╦══════════════╗
║     p6      ║      p7      ║   p8-p13  ║      p14       ║      p15     ║
║ clef number ║ space factor ║ Irregular ║ ignore justify ║ horz. offset ║
╚═════════════╩══════════════╩═══════════╩════════════════╩══════════════╝
```

================ CODE 17 creates: ================ * Key signatures

P5 = positive = sharps. = negative = flats. = + or - 100 = naturals in pattern of flats or sharps.

P6 = clef num.: 0=treb., 1=bass, 2=alto, 3=tenor. --> +100 for cue-sized key signature.

P7 = spacing factor for accidentals (default=1) --> if P7 < 1 then the accidentals will be closer together.

P8 - P13 -- Irregular Key Signatures

Each parameter can account for 1 accidental. The number given should be in the form xyz, where x = accidental number and yz = vertical position.

The accidental numbers are: 1 = flat, 2 = sharp, 3 = natural

Example: P8 = 107, P9 = 211

This puts a flat on the middle staff line (Bb) and a sharp on the top staff line (F#).

--> When p8-p13 are in use, p5 and p6 are ignored

--> Score's LJ does not account for the spacing requirements of irregular key signatures, but LJ.EXE does; however, see next entry:

--> To make Score's LJ account for the NUMBER of accidentals in your original key signature, put that number in P5.  Score will LJ the system as though the the key signature indictated in P5 is what is in force. Therefore, if your original, non-standard key signature has 4 accidentals, you could put 4, -4, 104, or -104 in P5 and SCORE's LJ would respond to it as though one of those key signatures were being used.  It will also respond to the spacing factor in P7.  This all means that you could in fact create a non-standard key signature of 2 accidentals, put 6 into P5, and SCORE's LJ would "see" a key signature of 6 sharps, and therefore create enough space to accommo- date that configuration, creating additional space following the 2 accidentals that are indicated in P8 and P9 and therefore displayed.

P14 = 1 = lineup and justify will ignore this key signature. (0 = normal)

P15 = horizontal offset from P3 position. --> (LJ will ignore item.)
