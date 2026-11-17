# Command and item index from SCORE

## In SCORE Modern

Use the [SCORE Modern guide](START.md) for current workflows. The original command index below describes DOS SCORE. For item codes 13 and 19, see the individual topics on invisible items.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

SCORE code numbers:

```text
Code 1. = Notes.               Code 10. = Numbers, rehearsal letters.
     2. = Rests.                    11. = User-added symbol library.
     3. = Clefs.                    12. = Circles, squares
     4. = Lines, crescendo.         13. = Invisible dummy item.
     5. = Slurs, brackets.          14. = Barlines.
     6. = Beams.                    15. = Insert PostScript code.
     7. = Trills, ottava, pedals.   16. = Text.
     8. = Staff lines.              17. = Key signatures.
     9. = Music symbol library.     18. = Meter.
```

SCORE letter commands:

(key to letters below: s=staff num.(p2), h=horz. pos.(p3), v=vert. pos.(p4) )

```text
A--A = group alter.          AD s,n = adjust stems.
   AR s,h,v = arco.          AT s,h,v = a tempo.    AC s,h,v = accel.
```

```text
C--C = group copy.           CD = change disk directory.
   CS = copy system.         COM = combine files.
   CR s,h,v... = cresc.      CW s = center whole rests.
   CRV = set default slur curvatures.
```

```text
D--DP s1,s2,s3... = staves disappear/reappear.
   DPY = export display vectors for DRAW program.
   DE = group delete.                DEL = delete a disk file.
   DEC = decombine staves.           DIR = disk directory.
   DRV = set default disk drive.     DI s,h,v = dim.
```

```text
E--ED h,s,c1,c2,.... = edit by horz. pos, staff, code#s.
   EDI = edit the input mode (re-beam, etc.).
   EX(f8) = exit.                    ENH = enharmonic shift.
```

```text
F--FL = flip note stems.   FLU = up, FLD = down, FLN = normal.
   F s,h,v (also FF,FFF,FP) = quick input for "forte" etc.
```

G--G name = get a SCORE file. GM name = get more (add to data on screen).

H--H = adjust height of total image by changing space between staff lines. HS = adjust hgt. from bottom line of staff 1 to top line of top staff.

```text
I--I n = edit by item number.      IN (f2) s = input music on staff s.
```

```text
J--J = horizontal justify.           JT = justify text (as lyrics).
```

L--L = line up according to rhythm.  LJ = line up and justify.

```text
M--M = group move.    MS = move system.    MA = create macro.
   MF s,h,v = mezzo forte. (also MP)   ME s,h,v = meno.
   MCL = motive clear (clear all motive data used in INPUT mode).
   MSEE = see which motive letters are in use.
```

```text
N--N n = edit an item but with no display of parameters.
         NX = get file with next alphabetical name.
         NB = get next file backwards in alphabet.
```

```text
O--OS = reorder data by staff num. starting with lowest staff.
   OH = reorder by horizontal pos.
   OT = reorder text items only (by staff, starting with HIGHEST staff).
```

```text
P--PR(f6) = print to laser.
   PREF = examine and edit preferences
   PM = parameter list.  PMX = write parameter list to disk file.
   PI s,h,v = piu.  PZ s,h,v = pizz.
   P s,h,v (also PP,PPP) = quick input for "piano" etc.
```

```text
R--RS (name) = restart (+ get new file).
   RE = read a file
         RES = read a file, suppress all prompts
   RIP = ripple bars from one line to other.
   RNUM m,n = renumber item number m to number n.
   RA s,h,v = rall.   RI s,h,v = rit.
```

```text
S--SA name= save data to disk file.
         SM = save with same name.
         SNX = save with next alphabetical name.
   ST s,c1,c2,... = edit by staff number and code nums.
   SC s = spacing scale above staff s.
         SCX = remove spacing scale.
   SH = show true output graphics. (Follow this with Z command.)
         SHX = return to normal screen graphics. (Follow with Z command.)
   SEP = separate double parts.
   STM = shorten stems too far above/below staff.
   SP s = set staff s for spacing of next input.
   STUD = adjust spacing of groups of notes with stems up and down.
```

T--T s,h,v = text input. TT s,h,v = continue text input until line begins with "/ ". TTR s,h,v = read external file as text input. TR s = transpose staff s. TRK = same, always asks re. key signature. TYPE name = type contents of disk file to screen. TAB s, num. of strings, tunings 1, 2, 3, 4, 5, 6 = Guitar/Lute tablature.

U--UND = undelete last deleted item.

V--VJ = vertical justify. VER = see SCORE version number and information on file currently in memory.

Z--Z size, horz.%, vert.%  = zoom screen size. Recomputes all graphics. ZZ size = zoom with current item in screen center. ZL % (f9) = zoom left. ZR % (f10) =zoom right. ZU % (PgUp or up arrow) = zoom up. ZD % (PgDn or down arrow) = zoom down. (HOME = top of page, END = bottom of page) ~/` = zoom to size 1 at last level of editing or input.

EDIT mode commands:

```text
AB(f8) = abort edit.            A(f6) = align after Set command.
CN(f4) s = center.              CR = set slur default curvature.
CW = center whole rests.        CX = copy and move to next.
C(f3) n = copy, go edit item n. CC = copy and continue.
CD = center dashes(in text).    CM s = center nums., meter on barline.
CV = center slurs vertically and horizontally.
DE(f7) = delete.                P n = show original value of parameter n.
SZ = see absolute size factor.  SZ n = set absolute size.
S(f5) = set alignment position. TP n = set tab fret position to >= n.
X n (<esc>, <end>) = exit from edit mode, go edit item n.
Z,ZZ  = zoom (as above, but only before any edit changes).
UND = undelete last deleted item. (Only before any edit changes.)
```

```text
L,R,U,D n = left, right, up, down.  (n is "sticky.")
   Arrow keys are same as L,R,U,D.
   <PgUp>=U5, <PgDn>=D5, <Tab>=R5, <shft><Tab>=L5
```

```text
-, +, (f2) = change direction to next item to edit.
f9,f10 = move left or right parameters of certain items.
<alt>n = set parameter number for editing with up/down arrow keys.
```

Text editing:

```text
TX(f9,f10) = edit text string.
      <home> = move cursor to front of line, <end> = move to end.
```

CT n = center text at position n. (n is "sticky.")

J n1, n2 = justify text in space from n1 to n2. (n1 and n2 are "sticky.") JR n = right justify to position n. (n is "sticky.")

LS n = set vertical line spacing by scale steps. LSI = by inches, LSC = by centimeters, LSP = by points.

```text
PT = see current point size.      PT n = reset point size to n.
```

RIP n = ripple n words (or -n) to line below (or above). RIPX n = ripple to newly created text line.

RM n1,n2,n3 = set right margin to position n1 ("ragged right"). n2 = number of lines to justify. n3 = set new vertical line spacing. RMJ = as above, but right side justified.

For more help on the Main Menu hit <esc> and type ?30.

For more help on the Edit Mode hit <esc> and type ?31.
