# Code 19 · Invisible MPUMX spacing items

## In SCORE Modern

SCORE Modern retains these spacing records without painting them.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 19 -- MPUMX -- Undocumented, Invisible Spacing Item

Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |

| Parameter | Meaning |
| --- | --- |
| `P10` | spacing factor |
| `P11` | spacing factor. |

================ CODE 19 creates: ================ * MPUMX, an otherwise undocumented, invisible spacing item (the meaning of whose name is uncertain; Math Processor Unit M-Export?) for creating additional space during LJ. (LJ.EXE supports these.)

MPUMXs *ONLY* work when attached to (i.e. share p3 with): + Code1 items (notes) + Code2 items (rests) + Code3 items (clefs) + Code14 items (barlines) + Code17 items (key signatures) + Code18 items (meters)

--> These items CANNOT BE CREATED DIRECTLY IN SCORE --> You may use EDITSCOR to *TRANSFORM* an item into a Code19 item and --> then manipulate its controls. --> The Code13, p5=99, p6/p7 spacing item of LJ.EXE offers MUCH greater --> control than this item does. --> Scor4 will claim the item is not yet in service when selected, but --> it will in fact work as described here.

SPACING VALUE:

The space created by a MPUMX = (15.5 + p10 + p11) * StaffSize

For example, if p10=3 and p11=7 and the staff size = .85, then if you have an MPUMX attached to a note, the amount of space that will be created after it following LJ will be:

= (15.5 + 3 + 7) * .85 = (25.5) * .85 = 5.95

Thus 5.95 p3 units of space will be cleared following the attached Code1 item after LJ.
