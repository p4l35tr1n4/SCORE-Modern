# Code 13 · Invisible dummy items

## In SCORE Modern

Earlier SCORE help described this code as reserved. Tom’s topic below specifies its use as an invisible dummy item. SCORE Modern retains the record and does not paint it.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 13 -- INVISIBLE DUMMY ITEM -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |

================ CODE 13 creates: ================ * An invisible item the user may use for any purpose --> PAGE creates Code13 items as placeholders for slurs and ties that --> occur in cross-system locations during page layout and part extraction; --> these may be safely deleted after those procedures.

In LJ.EXE, if a Code13 item is attached to a justifiable item (i.e. an item of either Codes 1, 2, 3, 14, 17, or 18), and the Code13 P5=99, then the value in P7 will create a correspondiong spacing following that item (if the value in P7 is positive) or before that item (if the value in P7 is negative).

---> If p6 = 0, then the value in P7 is multiplied by the current staff size (so if the staff p5=.5 and p6=10, then the spacing created will be 5, because .5 * 10 = 5.)

---> If p6 = -1, the value in P7 is indepedent of the staff size and will remain fixed regardless of staff size changes.
