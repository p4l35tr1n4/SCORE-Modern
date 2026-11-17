# Beam input

## In SCORE Modern

During input, the score shows note numbers for the marks, beams and slurs stages. These numbers identify the items referred to by the stage syntax.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

The following keys are active in the Beams Input section of SCORE.

SCORE Command Language:

```text
AB or F10 - Auto beaming based on displayed meter.
nB;       - Beam based on group of n eighth notes.
nBn2;     - Same as above with n2 notes/rests pickup.
<numbers> - Specify beam end points by note number.
/         - Slash is used to separate pairs of numbers.
;         - Semi-colon is used to end beam input.
+         - Before note number forces beam and stems up.
-         - Before note number forces beam and stems down.
G         - Go. Skip Input for beams and slurs.
<ESC>     - Abandon Input or Correct a previous error.
xmm nn -or- mm xnn -Puts number x over beam from notes mm to nn.
```

Function Keys:

```text
F1             - Get Help.
F2..F9         - Same as entering nB command above (i.e. F2 = 2B).
```

```text
Alternate Keystrokes for windowing:   (Hold down <Alt> and press a key)
```

```text
Alt C - CLOSE. Close graphics window to minimun size (Text window max.)
Alt D - DOWN.  Move bottom of text window DOWN 1 line (make it bigger).
Alt F - FULL.  Toggle FULL screen graphics on/off.
Alt M - MENU.  Toggle function key MENU on/off.
Alt O - OPEN.  Open graphics window to maximum size. (Text window min.)
Alt U - UP.    Move bottom of text window UP 1 line (make it smaller).
```

Keystrokes for text window and line editing:

```text
Home      - Clear text window.
Lft Arrow - Move text cursor left for line editing.
Rht Arrow - Move text cursor right for line editing.
Ins       - Toggle insert mode on/off for line editing.
Del       - Delete character at current cursor location.
BackSpace - Backup one space and delete character.
Ctrl Q    - Regenerate last text line entered (must be first character).
```

```text
Mouse:   (The mouse buttons are defined (L)eft, (M)iddle, and (R)ight below)
```

Click any button on menu - Activate normal function key. Click L button on staff  - Enter left note# of beam and a space. Click R button on staff  - Enter right note# of beam and a slash. Click M button on staff  - Enter right note# of beam and a semi-colon.
