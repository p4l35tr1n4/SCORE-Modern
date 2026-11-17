# Code 3 · Clefs

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 3 -- Clefs -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | clef type |
| `P6` | horz. size |

| Parameter | Meaning |
| --- | --- |
| `P7` | vertical size |
| `P10` | laser line thickness |
| `P15` | horizontal offset |

=============== CODE 3 creates: =============== * Clefs

P4 = vertical location.  For cue-sized clefs (needed for clef changes midway across a staff) add 100 to P4 (or -100 if P4 is negative.) This is equivalent to p15 = .6

```text
P5 = clef types:
     0 = treble
     1 = bass
     2 = alto
     3 = tenor
     4 = percussion
```

.8  = choral tenor (8ba = 8 below treble clef) .88 = octave up (8va = 8 above treble clef) .15 = 2 octaves up (15ma = 15 above treble clef) 1.8  = contrabass (8ba = 8 below bass clef)

.2 = 2 under treble .3 = 3 under treble .4 = 4 under treble, etc. .10-.14 = 15ba (15 below treble)

```text
TAB clefs:
44 = 4-string
55 = 5-string
66 = 6-string
```

IMPORTANT: If none of these above-listed values are used, P5 = Code9p5 (e.g. Code3 p5=31 will produce a fermata).

The normal vertical position is set with P4 = 0. For C clefs other than Alto or Tenor, simply create and move the Alto clef up or down with P4.

```text
P6 = Horizontal size factor; a negative number reflects the clef.
P7 = Vertical size factor; a negative number inverts the clef.
```

(P6 size 0 = size 1.)

If P7 = 0, P7 defaults to the value of P6; the symbol therefore scales horizontally and vertically to the size of P6.

--> If P6= X and P7= -X, an inverted clef is created. --> If P6= -X and P7 = X, a reflected clef if created. --> If P6= -X and P7= -X, an inverted & reflected clef is created.

P10 = outline thickness control.  P10 adds (or subtracts) pixels to the basic line thickness established by the prompts for EPS/PS printing.

--> Negative values reduce or "thin" the outline of the glyph --> Positive values increase or "thicken" the outline of the glyph

E.g., for small staff sizes, setting p10 = -4 might be optimal for ensuring that the clef remains crisp and distinct, rather than "puffy," which is typical at the default p10=0 setting.

--> This parameter functions identically to P10 of Code9.

P15 = horizontal offset from given p3 position --> This p15 offset is ignored by SCORE's LJ, but recognized by LJ.EXE
