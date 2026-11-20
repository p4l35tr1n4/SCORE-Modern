# Code 16 · Text · SCOR4

## In SCORE Modern Core

SCORE font numbers resolve through PSC metrics and the internal PostScript font name. See the [SCORE Modern guide](START.md) for font setup and duplicate versions.

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

```text
  parameter structure
╔══════════╦═══════════╦════════════╦═══════════╦════════════════════════╗
║    p1    ║    p2     ║    p3      ║     p4    ║          p5            ║
║ code num.║ staff num.║ horiz. pos.║ vert. pos.║ character space factor ║
╚═══ 16 ═══╩═══════════╩════════════╩═══════════╩════════════════════════╝
```

```text
╔════════════╦════════════╦═══════════════╦══════════╦══════════════════╗
║     p6     ║     p7     ║     p8        ║    p9    ║       p10        ║
║ horz. size ║ vert. size ║ font override ║ rotation ║ size of a space  ║
╚════════════╩════════════╩═══════════════╩══════════╩══════════════════╝
```

```text
╔══════════════════╦═════════════╦═══════════════╦════════════╗
║        p11       ║     p12     ║     p13       ║  p14-p53   ║
║ horz. displcmnt. ║ char. count ║ string length ║ (the text) ║
╚══════════════════╩═════════════╩═══════════════╩════════════╝
```

CODE 16 creates:

- Text items (both PostScript Type1 and some built-in vector fonts)

Font codes are expressed with _##, where ## references the font indicated by the FM##.PSC file in the \LIB directory (if you are using Score 3.x) or from the directory specified by the LIBRARY setting in PREF-4.SCR (if you are using Score 4.x).

This means that switching FM##.PSC files switches the fonts that are used throughout the program.  All the fonts used in other Code types are user- controllable, except for tuplet numbers in Code5 and Code6, which are hard-coded to use whichever font is specified in FM02.PSC.

The default fonts are typically "quartets" in which: A) the first is the roman form (e.g. _00 = TimesRoman) B) the second is the bold form (e.g. _01 = TimesBold) C) the third is the italic form (e.g. _02 = TimesItalic) D) the fourth is the bold italic form (e.g. _03 = TimesBoldItalic)

```text
The default font codes are:
_00-03: TimesRoman
_04-07: Helvetica
_08-11: Helvetica Narrow
_12-15: Palatino
_16-19: Avante Garde Book
_20-23: Bookman
_24-27: New Century Schoolbook
_28:    Zapf Chancery Medium
_29-32: Courier
_33:    Symbol
_34: Zapf Dingbats
_90: Score's Bodoni Roman [crude] (from BODN0 - BODN6.DRW)
_91: Score's Bodoni Bold [crude] (from BODB0 - BODB6.DRW)
_92: Score's Bodoni Italic [crude] (from BODI0 - BODI6.DRW)
_93: Score's Bodoni Bold Italic [crude] (from BDIB0 - BDIB6.DRW)
_94: Score's meter number and dynamic font (from MUSI0 - MUSI9.DRW)
     --> The numerals 0-9 are well-designed and are used for Code18 meters
     --> The lower-case letters f, m, p, s, z are well-designed and suitable
         for inclusion in text strings, but all other letters are crude
_98: Primitive (stick letters from *.DAT initialization file)
_99: PostScript commands/comments ("_99%This is a non-printing comment")
```

P5 = Interletter spacing.  This adjusts the space used by each character. For example, if P5 = .9 then each character will occupy only 90% of its ordinary space.  (The p6/p7 SIZE of the character remains the same.) The factor will hold for the entire string.

P6 = Horizontal size factor (default 0 = 1)

For PAGE program part extractor: P6 =+ 100: extract in all parts =+ 200: extract in no parts =+ 300: extract in all parts, but change size to p6=1

P7 = Vertical size factor --> If P7 is left at zero it will default to whatever value is in P6.

P8 = font override of ENTIRE text string (thus ignoring underbar code text changes within the string)

= 90: Score's Bodoni Roman [crude] (from BODN0 - BODN6.DRW) = 91: Score's Bodoni Bold [crude] (from BODB0 - BODB6.DRW) = 92: Score's Bodoni Italic [crude] (from BODI0 - BODI6.DRW) = 93: Score's Bodoni Bold Italic [crude] (from BDIB0 - BDIB6.DRW) --> Notice nomenclature: BODN? = Bodoni Normal BODB? = Bodoni Bold BODI? = Bodoni Italic BDIB? = Bodoni BoldItalic = 94: Score's meter number and dynamic font (from MUSI0 - MUSI9.DRW) --> The numerals 0-9 are well-designed and are used for Code18 meters --> The lower-case letters f, m, p, s, z are well-designed and suitable for inclusion in text strings, but all other letters are crude = 98: Primitive (stick letters from *.DAT initialization file) = 99: PostScript commands/comments ("_99%This is a non-printing comment") = 1000 to 1089 = corresponding to PostScript Type1 fonts specified in the FM*.PSC files _00 to _89.

*--> Outline (unfilled) fonts are made by adding 100 to p8. For example, to make an outline of the _04 font, set P8=104.*

*--> All of the above is used to specify fonts by using "_" followed by the --> two-digit opcode number.  So, to specify font 94 in a text string, --> precede the affected text with "_94".*

--> SCOR4 reads FM*.PSC from the library indicated in your PREF-4.SCR file, --> in contrast to SCORE 3.X, which only reads FM*.PSC from C:\LIB.

P9 = degrees of rotation for the entire string.

P10 = sets the size of the space chararacter only. --> Both P10 and P5 are factors which act relative to the P6 size factor.

P11 = Horizontal displacement of text. --> This is useful when it is desired to keep the P3 value locked on to --> a particular note position so that the relative position of the text --> will not change when notes are justified, etc.  (See CTN.)

Useful example: To keep a title centered and unaffected by LJ, first center it with CTN, then enter:

```text
    11000 3 3 -1 1100 1
```

This put p3 into p11, resets p3 to -1, and then adds 1 to p11. Now the text item is outside the range of LJ (assuming the music is located somewhere along p3=0-200) and its centering will remain fixed. This technique can be used for Code10 page numbers, for example:

```text
    15000 3 3 -1 1500 1
```

...where P15 of Code10 holds the equivalent horizontal offset to P11 in Code16.

P12 = The number of characters in the text string, including font codes. --> DO NOT ALTER THIS NUMBER; IT IS UPDATED AUTOMATICALLY BY THE PROGRAM --> WHENEVER YOU REOPEN OR REFRESH A FILE.

P13 = The p3 length of the text string, useful for centering routines in Editscor, etc. --> DO NOT ALTER THIS NUMBER; IT IS UPDATED AUTOMATICALLY BY THE PROGRAM --> WHENEVER YOU REOPEN OR REFRESH A FILE.

P14-P53 = The text, stored in 4-character "chunks"; each parameter holds 4 characters, so the maximum text length is 40 characters.

--EDITING THE TEXT--(You must be in Edit Mode to use the following commands.)

[, ] (or TX)-- The text itself may be edited by entering EDIT MODE and typing TX<enter> or just hitting the [ key.  The text will appear just above the graphics window.  At this point the left and right cursor keys (arrows) may be used to locate individual characters to changed.

```text
Characters may be inserted by first hitting the <insert> key. Hitting the
    <insert> key again causes a return to the normal (or overstrike) mode
    (i.e. it toggles).
```

The <home> key moves the cursor to the far left end of the text string.

The <end> key moves the cursor to the right end of the line.

When <enter> is hit, the new text will appear on top of the original form of the text.

When [ is hit the cursor will appear at the front of the line, just after the font number.

When ] is hit the cursor will appear at the end of the line.

The <tab> key will move the cursor to the next word to the right.

Depressing <shft>+<tab> will move the the cursor to the word to the left.

```text
Deleting characters:
<delete> will remove a single character.
<ctrl>+<end> deletes all characters from the cursor to the end of the line.
<ctrl>+<home> deletes all from the cursor back to the front of the line.
```

--RIPPLING WORDS--

RIP n -- This command will "ripple" a total of n words from one line to the next. If n is not given, only one word will be moved.  If n is negative, the words will go to the previous line (usually the line above.)  If the next item is not a Code 16 text string, RIP will create a new line below or above the one being edited.

RIPX n -- This command will ALWAYS create a new text line.  (If the text lines are not in their proper order, the OT [Order Text] command should be used. This command CANNOT be given while you are still in EDIT MODE.)

--CENTERING TEXT--

CT x -- The text may be centered on any position (x) by typing CT followed by the position number.  The last number given for this command will be reused if no new number is given.

CTN s -- Centers the text on the nearest note or rest on staff s. (P11 is altered.)

--JUSTIFYING TEXT--

J n1 n2 -- This command justifies the text between the two positions given. If no position numbers are given, the previously given numbers will again be used.  The line is justified by increasing the size of the space between each word (i.e. p10 gets changed).

JR n -- JR = Justify Right.  This sets the right end of the text string at position n.  If n is not given the previous value of n is used.

RM n1 n2 n3 -- RM = set Right Margin. With this, a total n2 lines will be adjusted so that no words extend beyond the position given as n1. The spaces between words will not be changed, but some words may be "rippled" from one line to another.

If n2 is 0, or not given, this command will effect all following text lines until no line is found which is on the same staff and has the same P3 position.

If n3 is not 0, it will set the vertical spacing of the lines.

RMJ n1 n2 n3 -- RMJ = set Right Margin and Justify the text between its current P3 position and position n1.  The other features of this command are the same as above.

PT n -- This sets the type size of the text in terms of 'n' typography points. (P7 will be set to 0, hence equal to P6.)

*IMPORTANT: If the staff size is changed later, or if the text is moved to a staff with a different size factor, the PT n command must be used again in order to reset the desired point size.*

Type PT alone to learn what the current point size is, however:

SCORE's calculation of point sizes is incorrect!!!

There are 73.734422 SCORE points per inch, and all of its "point" calculations use this non-standard value.

To get accurate, 72-points-per-inch points, set p6 using this formula:

p6 = ((DesiredPointSize / 13.885955) / StaffSize)

To determine the true 72-points-per-inch point size of a text string, provided its P6 size and staff size, use this formula:

TruePointSize = p6 * 13.885955 * StaffSize

LS n1 n2 n3 -- LS sets vertical line spacing.  In the simple form of this command, n1 sets the space between text lines in terms of vertical scale steps.  n2 is used to indicate how many lines are to be involved.  If n2 is left at 0, then all lines in the proper order, on the same staff, and in the same P3 position will be respaced.  If n3 is set to 1, then only the first two conditions above must be met.  All affected lines will be moved to the same P3 position as the original line.

LSI, LSC, LSP  These work in the same way as LS except that the "I" will set vertical spacing in inches, the "C" sets it in centimeters, and the "P" sets it in typography points. All lines altered by the use of LS will have their size factors (P6, P7) set to the values found in the original text line.

L,R,U,D -- The normal left-right-up-down edit letters may have a P added. Thus, LP 20, moves text 20 points to the left, etc.  There are 72 points in an inch.

--Tabs--

Tabs may be set within a line of text by means of the following syntax:

aaaaa ~n1~bbbb cccc ~n2~dddd eeee  ~n3~.... etc.

The portion of the text string beginning with bbbb will start at the horizontal position given as n1.  N2 sets the starting position of the next part of the string, etc.  The given tab positions are unaffected by changes to P3.  The tab positions can be changed only by means of the TX command. The tab feature may be useful when setting up various columns of text.

Type ?T to get help on the various options when doing original text input.

*--> The above commands can sometimes alter many different items at once --> It is generally a good idea to save the work (SA) before using such --> commands.*

--Inputting Lyric Text--

There are two methods: TT and TC

TT makes each syllable flush with each note (i.e. p3=note p3, p11=0).

TC centers each syllable that is NOT followed by an extender; syllables followed by an extender are made flush with the note (i.e. p3=note p3, p11=0).

To input with either form, enter the command followed by p2, p3, p4, etc.

For TT, enter:

TT p2 p3 p4 p5 etc.

For TC, enter:

TC p2 p3 p4 p5 etc.

Then type the text, separating words with forward slashes, hyphenated syllables with F7, and extenders with F6. Hit <Enter> at end of line.

Then input the note numbers lyrics should be associated with.  This can be done all in one line, like:

```text
1 3 4 5 8 10 11 12
```

If there is a one-to-one relationship between notes and lyrics starting with note 1, then simply enter:

99

If the first few skip around, type the numbers explicitly until you arrive at a point where the remaining lyrics have a one-for-one relationship, and then type 99, like this:

```text
1 3 4 7 99
```

...in this case notes 7 to the end would have lyric text items 5 and on added to them.

--Quick Text Input--

To quickly input text, enter the text followed by parameter 2 and up, as far as you wish to go parametrically: [text] p2 p3 p4 p5 etc.

So, to input "_00Symphony No. 6" on staff 5 at position p3=23.4, p4=20, p5=0, p6=101.3, enter:

"_00Symphony No. 6" 5 23.4 20 0 101.3

Additionally, to automatically CENTER the quick-input text, add 1000 to the p3 parameter.  So, to have the above text centered (i.e. shifted with p11 so that its center is at the prescribed p3 location), enter:

"_00Symphony No. 6" 5 1023.4 20 0 101.3
