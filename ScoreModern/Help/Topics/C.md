# C commands

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

```text
"C" commands: C = group copy.
             CD = Change current disk directory.
            COM = combine alphabetically ordered files.
             CR = cresc.
            CRV = group adjustment of slur curvature.
             CS = copy complete system.
            CWn = center all whole rests on staff n.
 The group copy works exactly like the move command except that the old
 material is left intact and a new set of items is created at the position
 given in response to the the 2nd prompt. (Type "?m" for details.)
```

```text
In Edit mode only:
     C = copy and exit edit mode.  Cz = copy and move to item z.
    CC = copy and select the copy for further editing.
    CX = copy and select the next item for editing.
    CN = center on nearest note or rest  (no vert shift).
   CNs = center on nearest note or rest on staff s.
    CM = center meter or number above nearby barline. ( P4 >= 2 )
    CR = set curve of slur to what is "normal" for length.
    CV = as above, but vertical move to just above a note.
   CTx = center text on position x (if editing code 16).
  CTNs = center text on center of closest note head (on staff s).
    CD = center dashes between text syllables (edit).
```

CD \name = do a DOS Change Directory command to directory "name".

CR = cresc. This command is a shortcut for typing a code 9 item.  It should normally be followed by 2 or 3 numbers (parameters 2, 3, 4).

cr 3 45 -2  will put "cresc." at position 45 on staff 3, vertical position -2.

```text
COM = combine files.   Reads in alphabetically ordered files.  As each
      is read in, the previous data are shifted to higher staff numbers.
      (Useful to combine single line files from part extractor in PAGE, etc.)
```

CS = copies complete systems to new staff positions.  (Works same as MS.)

CW x = centers all whole rests on staff x.  If x > 32, all staves affected. (Rest's P5 must = -2 or less.)

-----The following work only while in Edit mode.------

C (copy) is the same as X but the original form of the item is left intact and the edited form is copied to the end of the item list. CC is the same as copy except that the copy is then selected for editing. CX is the same as copy except that the next item is selected for editing.

----- In Edit mode.

CN will center an item's horizontal parameters on the position of the closest note or rest.  In the case of items with more than 1 horizontal parameter (slurs, lines, beams) each parameter is automatically centered separately. CN, followed by a number (x), works as above except that only notes and rests on staff 'x' will used for centering positions.

CV centers on notes and changes the vertical position to about 2 steps beyond the note head.  (Used for centering slurs and adjusting their height.)

CR = set the curve of a slur to what is normal, considering the slur length.

CT x = center a string of text (code 16) on position 'x'.  A musical title might be centered on the page by typing: CT100  (center on position 100).

CTN = center text string on nearest note or rest.

CD = center dash(es) separating the syllables of text (lyrics) under music.

CM = center meters or numbers over bar line found nearby (within 15 steps). CM x = center a bar line on bar line found on staff x.
