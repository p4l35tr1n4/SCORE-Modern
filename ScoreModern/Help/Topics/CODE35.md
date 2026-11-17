# Marks input

## In SCORE Modern

The score shows note numbers in this stage. Use those numbers to address the notes being marked.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

The following keys are active in the Marks Input section of SCORE.

SCORE Command Language:

(Marks attached to notes)

```text
FIF - Musica ficta, flat  (p11=1)  |  TH     - Thesis           (p11=16)
FIS - Musica ficta, sharp (p11=2)  |  MO     - Mordant          (p11=17)
FIN - Musica ficta, nat.  (p11=3)  |  IM     - Inverted mordant (p11=18)
W   - Wedge               (p11=4)  |  HW     - Heavy wedge      (p11=19)
A   - Accent              (p11=5)  |  TR     - Trill            (p11=20)
T   - Tenuto              (p11=6)  |  TRF    - Trill, flat      (p11=21)
S   - Staccato            (p11=7)  |  TRS    - Trill, sharp     (p11=22)
WS  - Wedge-staccato      (p11=8)  |  TRN    - Trill, natural   (p11=23)
AS  - Accent-staccato     (p11=9)  |  PL     - Plus             (p11=24)
TS  - Tenuto-staccato     (p11=10) |  ACT    - Accent-tenuto    (p11=25)
D   - Down bow            (p11=11) |  S2     - Two dots         (p11=26)
U   - Up bow              (p11=12) |  S3     - Three dots       (p11=27)
H   - Harmonic            (p11=13) |  S4     - Four dots        (p11=28)
FE  - Fermata             (p11=14) |  F0-999 - Numbers 0-999    (p11=30-1029)
AR  - Arsis               (p11=15) |
```

SCORE Command Language (continued):

(Other marks)

```text
PPP                             |    DI     - Dim.
PP                              |    RI     - Rit.
P                               |    PZ     - Pizzicato
MP                              |    AO     - Arco
MF                              |    AC     - Accel.
F                               |    TMT    - Tremolo beams, 32nds
FF                              |    TME    - Tremolo beams, 8ths
FFF                             |    TMS    - Tremolo beams, 16ths
FP                              |    C x y  - Hairpin crescendo x to y
SF                              |    C- x y - Hairpin decrescendo x to y
SFZ                             |    O x y  - Ottava x to y
CR  - Crescendo                 |    OB x y - Ottava bassa x to y
```

N+Code9p5 will access Code9p5; n50 will therefore access pppp (p5=50)

SCORE Command Language (continued):

```text
F1        - Get Help.
F2...F8   - Enter marks shown on menu.
<numbers> - Enter mark's horizontal position by note number.
/         - Slash is used to separate different marks.
;         - Semi-colon is used to end marks input.
+ or -    - Before mark forces mark to the opposite side of the staff.
G         - Go. Skip Input for marks, beams, and slurs.
<ESC>     - Abandon Input or Correct a previous error.
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

Mouse:

```text
Click any button on menu     - Activate normal function key.
Click Left button on staff   - Enter marks horz. position with a space.
Click Right button on staff  - Enter marks horz. position with a slash.
Click Middle button on staff - Enter marks horz. pos. with semi-colon.
```
