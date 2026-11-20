# Pitch input · SCOR4

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

The following keys are active in the Pitch Input section of SCORE.

SCORE Command Language:

```text
       M or F3   - Measure line.
       Mn        - Measure line up n staves.
       MD        - Double measure line.
       MR        - Repeat measure line (right).
       ML or MRL - Repeat measure line (left).
       MRD       - Double repeat measure line (both).
       MH        - Heavy double measure line.
       MS or MDS - Dashed measure line.
       MI        - Invisible measure line (dotted on screen).
       TR        - Treble clef.
       BA        - Bass clef.
       AL        - Alto clef.
       TE        - Tenor clef.
       K         - Key signature of displayed key.
       KnS       - Key signature of n sharps.
       KnF       - Key signature of n flats.
       KnSN      - Key signature of n naturals/sharp pattern.
       KnFN      - Key signature of n naturals/flat pattern.
       -TR, -BA, etc. -- invisible clef
```

SCORE Command Language (continued):

```text
       <ESC>     - Abandon Input and Exit to Input Setup Section.
       F1        - HELP section.
       T         - Time signature of displayed meter.
       n1 n2     - Time signature for meter n1/n2.
       COM       - Time signature for Common time.
       CUT       - Time signature for Cut time (alla breve).
       A to C    - Pitches A through C.
       F, S, N   - Follows pitches for Flat, Sharp, and Natural.
       FF, SS    - Follows pitches for Double Flat, Double Sharp.
       (F,(S,(N  - Parentheses around Flat, Sharp, Natural.
       0 to 8    - Follows pitches for octaves 0 through 8.
       R or F2   - Rest of any duration.
       RW        - Whole rest regardless of duration.
       RF        - Rest with fermata.
       RU        - Rest up higher on the staff.
       RD        - Rest down lower on the staff.
       RI        - Invisible rest.
       Rn        - Whole rest with number for n bars rest.
       RP        - Repeat measure symbol (./.).
       Q+        - Begin creating cue-size notes.
       Q-        - Stop cue-size notes, return to normal size.
```

SCORE Command Language (continued):

```text
       SU or F4  - Force following notes to have Stems Up.
       SD or F5  - Force following notes to have Stems Down.
       SO or F6  - Restore automatic stem direction (Stems Ordinary).
       S+,S-,S0  - Notes to staff above(+), staff below(-), home staff(0).
       P or F7     - Proximity mode on (regarding octaves).
       O or F8     - Proximity mode off (Ordinary mode on).
       YU          - Pitch Y at level up above level of previous pitch.
       YJ          - Pitch Y at level down from level of previous pitch.
       YXn         - Repeat pitch Y, etc. for n times (e.g., CS4x4).
       YaZn        - Repeat pitch Y with accidental a preserved.
       Xm n, Zm n  - Repeat previous m items a total of n times.
       /           - Slash is used to separate the above commands.
       //          - Extra slash(es) repeat previous single item.
       : or '      - Colon or apostrophe is used to separate chord pitches.
       ;           - Semi-colon is used to end pitch input.
       .           - Repeat item found before previous measure line.
       ..          - Repeat with accidental preserved.
```

```text
Alternate Keystrokes for windowing:   (Hold down <Alt> and press a key)
```

```text
       Alt C - CLOSE. Close grapics window to minimun size. (Text window max.)
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

Mouse: (The mouse buttons are defined (L)eft, (M)iddle, and (R)ight below)

```text
       Click L or R button on staff  - Enter pitch of line or space selected.
       Click R and M button on staff - Abandon Input and Exit (same as ESC).
       Click L and M button on staff -    "       "   "    "     "  "   "
       Click any button on menu bar  - Activate normal function key.
       Click L and R buttons         - Enter a colon (:) for chords.
```

MIDI Keyboard:

Play notes and chords - Enter note and chord pitches. See Manual Additions booklet.
