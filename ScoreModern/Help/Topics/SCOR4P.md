# P commands · SCOR4

## In SCORE Modern Core

See [Printing and EPS export](PRINT.md) and [SCORE preferences](PREF.md).

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

PA  = PATH. Resets the current disk drive and directory. First types the current path, then prompts as follows.

Press new drive letter or <Enter> -> (If you hit <Enter> the drive is not changed.)

Type new directory name or <Enter> -> (If you hit <Enter> the directory is not changed.)

PS  = PASTE. Paste in the contents of the !!CUT.TMP file that was created with the CUT (CT) command.  See CT help. (?C)

PR (f6) = brings up the printing menu. A print preference file, PRINT.SCR, file may be created. See Manual Additions booklet, pg. 8.01, for details.

PREF = see and edit current preference setup. The PREF-4.SCR file should normally reside in the \SCOR4\LIB directory. See Manual Additions booklet, pg. 7.01, for details.

PM  = prints on the screen the parameter list for all items.

```text
PMX = creates a file containing all parameter data.  If you hit only
        <enter> when prompted for a file name, it will write a file with
        the name "PARAM."
```

P, PP, PPP These commands are shortcuts for typing a code 9 items.  They should normally be followed by 2 or 3 numbers (parameters 2, 3, 4).

```text
         pp 3 45 -2   will put "pp" at position 45 on staff 3,
                       vertical position -2.
```

(See Code 9 (?9) for details.)

```text
PIU as above, but displays "piu."     PIZ = "pizz."   POC = "poco"
PED, PEDA, PEDB, PEDC = various pedal signs
PER = percussion clef (Code 3)
POS = read in a PostScript .EPS file (Code 15)
```

~
