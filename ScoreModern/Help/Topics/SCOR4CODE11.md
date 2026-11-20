# Code 11 · User drawings · SCOR4

## In SCORE Modern Core

Use **DRAW…** from the context menu outside the page to edit drawing banks. The active drawing LIB takes priority over the installation’s LIB for matching filenames. Named drawings and numbered library symbols have separate item codes.

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

parameter structure

```text
╔══════════╦═══════════╦════════════╦═══════════╦════════════════╗
║    p1    ║     p2    ║     p3     ║     p4    ║       p5       ║
║ code num.║ staff num.║ horiz. pos.║ vert. pos.║ library number ║
╚═══ 11 ═══╩═══════════╩════════════╩═══════════╩════════════════╝
```

```text
╔════════════╦════════════╦═══════════╦══════════╦═════════════════╗
║     p6     ║     p7     ║    p8     ║    p9    ║       p10       ║
║ horz. size ║ vert. size ║ thickness ║ rotation ║ laser thickness ║
╚════════════╩════════════╩═══════════╩══════════╩═════════════════╝
```

```text
╔══════════════╦════════════════════╦════════════╦════════════╗
║     p11      ║        p13         ║         p14-p19         ║
║ horz. offset ║ reset library name ║ holds library file name ║
╚══════════════╩════════════════════╩════════════╩════════════╝
```

CODE 11 creates:

- Glyphs from original DRW files accessed outside the LIB*.DRW sequence

Each library file may hold 10 drawings, numbered 0 to 9.  P5 is used to access particular drawings.  If a series of library files are given alphabetically ordered names, only the first name of the series need be used. Thus with the series, PERCA, PERCB, and PERCC, only PERCA need be given. P5 values from 0 to 29 may be used to access the symbols from all 3 files.

```text
P5 = 0-9, corresponding to the 10 glyph positions in the DRW file.
P6 = Horizontal size factor; a negative number reflects the glyph.
P7 = Vertical size factor; a negative number inverts the glyph.
(P6 size 0 = size 1.)
```

If P7 = 0, P7 defaults to the value of P6; the symbol therefore scales horizontal and vertically to the size of P6.

*--> If P6= X and P7= -X, an inverted glyph is created. --> If P6= -X and P7 = X, a reflected glyph if created. --> If P6= -X and P7= -X, an inverted & reflected glyph is created.*

NOTICE: Unlike Code9, Code11 items cannot be made to be extractible by adding 100 to p6, nor non-extractible by adding 200 to p6, nor extractible but resized to p6=1 by adding 300 to p6.

P8 = thickness, using the form "x0y", where: x = the added vertical thickness y = the added horizontal thickness. -->  If P8 = -1, outline only prints (no filler).

P9 = rotates the symbol clockwise.

P10 = laser line thickness control.  P10 adds (or subtracts) pixels to the basic line thickness established by the prompts for laser output copy.  In general, P10=-1 will give lighter copy, P10=1, or more, gives darker copy.  Positive values of P10 will produce heavier copy on dot matrix printers also.  (P10 = 1 is the same as P8 = 101 in this case.)

P11 = Horizontal offset from p3 position.  Notice that this is not available in Score 3.X nor in Scor4 Beta, so it is not backwards-compatible with those versions of Score.

If P13 is set to 1 a new library file name will be requested.

P14-P19 hold the file name The prefix of the file name may *NEVER* have more than 6 letters, e.g. HELLO1.DRW is valid, but GOODBYE.DRW is not. There are some peculiarities:

1) When prompted for the library name, you may enter just the prefix (HELLO) or the full name (HELLO.DRW), and either will be read correctly. (Whatever you specified--suffix or no suffix--is stored in the SCORE file, and SCORE will add the suffix .DRW dynamically when calling the file if none is specified.)

2) You should *NOT* specify the PATH location, however (unlike Code15 items, which DO accept drive letter and path specification).  Code11 items are assumed either to be in the LOCAL WORKING DIRECTORY or the LIBRARY DIRECTORY specified in the PREF file (PREF.SCR in Score 3.11/Scor4 beta, or PREF-4.SCR in commercial Scor4.)

3) The displayed name when the item is selected will only be the first 5 letters, so if you entered HELLO1.DRW or HELLO1, then only "HELLO" will be displayed as the name of the file, even though HELLO1.DRW is being read. To avoid this confusion, it is perhaps best to limit Code11 prefixes to 5 letters, even though 6 letter prefixes are allowed and stored in the files.

4) NUL characters are used internally to "pad" the file name to the end of P19.


**Verified implementation:** Commercial SCOR4 P11 is supported for native display, bounds and EPS. Its horizontal offset is applied before rotation, in SCORE units. SCORE 3.11 ignores it.
