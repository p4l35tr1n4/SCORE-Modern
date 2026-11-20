# PLAY · SCOR4

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

MIDI Playback Sound Setup Commands.

```text
The top of the screen has four lines of numbers in 21 columns as follows:
┌─────┬───────┬─────┬─────────┬─────────────╥─────────────────────────────┐
│Track│Channel│Patch│Amplitude│Transposition║-- 16 possible staff numbers.│
└─────┴───────┴─────┴─────────┴─────────────╨─────────────────────────────┘
```

The tracks are numbered 1 through 4. No more than 4 staves of music may be played at once.

Each track may be played on a separate Channel - or all on the same channel. The available channel numbers will depend on the MIDI system in use.

The Patch is the particular instrument tone color number. The tone numbers will depend on the MIDI system in use.

The Amplitude numbers are sometimes also known as key velocity values.

F9 will zero all parameters in first five columns. F10 will zero all staff numbers (last 16 columns). Hitting F10 again will bring back default staff numbers.

Transposition is indicated in chromatic 1/2 steps, using + or - numbers.

Some conventional transpositions--

```text
  Piccolo ....................= +12   Trumpet in D ..................= +2
  Clarinet, Trumpet in Bb ....= -2    Clarinet in A .................= -3
  Alto Flute in G ............= -5    English Horn, French Horn in F = -7
  Alto Sax, French Horn in Eb = -9    Tenor Sax, Bass Clarinet ......= -14
  Baritone Sax ...............= -21   Clarinet in Eb ................= +3
  etc.
```

Entering and changing playback parameters:

```text
       Left / Right Arrows - Select parameter for alteration.
       Up / Down Arrows    - Select track for parameter alteration.
       <numbers>           - Enter a new value for the selected parameter.
                            (Also see below re. mouse usage.)
```

Bottom Screen Menu:

```text
 ───────┬───────────┬──────────┬──────────┬───────┬───────┬──────────┬────────
 F1-HELP│F2-TMPO=120│F3-MIDI=on│F4-SPKR=on│F5-READ│F6-SAVE│SPACE=Play│F8=Retrn
 ───────┴───────────┴──────────┴──────────┴───────┴───────┴──────────┴────────
 F1 -HELP      - Get this help file.
 F2 -TMPO      - Set Tempo value. Tempo may also be changed with
                      the + and - keys on the keyboard far right.
 F3 -MIDI      - Toggle MIDI Sound Playback on/off.
 F4 -SPKR*     - Toggle PC Speaker Playback on/off.
 F5 -READ      - Read play setup file.
 F6 -SAVE      - Save current setup parameters.
 <SPACE>       - Begin Playback.
 F8 -Retrn     - Return to Main Menu.  ( <ESC> will also return. )
```

Any of these options may be activated by hitting the indicated function key or by pointing and clicking with the left mouse button.

- Change to the PC speaker should be made before playing begins, otherwise some notes may be omitted. For best results with the MIDI playback, leave the PC speaker off at all times.

The READ/SAVE features (F5/F6) allow you to save and read back all the information in the top parameter area.  After hitting F6 (SAVE) you are prompted for a file name. If no name is given (you hit <enter> only), the name of the current SCORE file will be used, but without any extension. The same holds true when you hit F5 to read in the parameters. When the PLAY section of SCORE is initiated, this saved file will be read in automatically if a file with the appropriate name is present on the disk. To clear these default parameters, use the F9 and F10 keys (see above).

Playback is halted by hitting <ESC> or the <SPACE> key or by a mouse click.

TEMPO may be changed at any time by hitting F2 and typing the desired value. Changes may also be made by holding down the + or - keys at the far right of a standard computer keyboard. These keys change the tempo in increments that conform to the conventional metronome values

```text
Keystrokes for graphics window:   (Zoom, Pan, Page up/down, etc.)
```

```text
     End       - Zoom to size 1 and display the bottom of the page. (Default)
     PgUp      - Move 50% up the page.
     PgDn      - Move 50% down the page.
     Ctrl PgUp - Zoom in. Make graphics bigger. See less of the page.
     Ctrl PgDn - Zoom out. Make graphics smaller. See more of the page.
     Ctrl Left - Pan the page 10% left.
     Ctrl Rght - Pan the page 10% right.
```

Mouse: Click any button on parameter - Select setup parameter for alteration, then type number. Click any button on menu bar  - Activate indicated function key.

Mouse while pointing in graphics window: Click left button on staff to add to play list - will put staff number at postion of cursor in top window. Click middle button to put zero in play list. Click right button to backspace cursor in top window. Click left and middle button together to move cursor to line below. Click right and middle button together to move cursor to line above.
