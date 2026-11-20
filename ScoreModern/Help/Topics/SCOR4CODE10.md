# Code 10 · Numbers and rehearsal letters · SCOR4

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

parameter structure

```text
╔═══════════╦════════════╦═════════════╦════════════╦════════╦══════╦══════╗
║    p1     ║     p2     ║     p3      ║     p4     ║   p5   ║  p6  ║  p7  ║
║ code num. ║ staff num. ║ horiz. pos. ║ vert. pos. ║ number ║ size ║ font ║
╚═══ 10 ════╩════════════╩═════════════╩════════════╩════════╩══════╩══════╝
```

```text
╔════════════╦════════════╦═══════════╦═══════════════╦═══════════════╗
║     p8     ║     p9     ║    p10    ║      p11      ║      p12      ║
║ circle,box ║ box height ║ box thick ║ horz. box sz. ║ vert. box sz. ║
╚════════════╩════════════╩═══════════╩═══════════════╩═══════════════╝
```

```text
╔═══════════╦═════════════╦════════╦═════════════╗
║    p13    ║     p14     ║   p15  ║     p16     ║
║ vert.pos. ║ space btwn. ║  horz. ║ make number ║
║  in box   ║   digits    ║ offset ║ justifiable ║
╚═══════════╩═════════════╩════════╩═════════════╝
```

CODE 10 creates:

- Numbers, used for: + measure numbers + page numbers + rehearsal numbers

P5 may be used for numbers 0 to 999. P5 = 1000: ":00" is created (use???) P5 >= 1001: Capital letters are used: --> P5 = 1001: "A" --> P5 = 1002: "B" --> P5 = 1003: "C", etc.

For PAGE program part extractor: P6 =+ 100: extract in all parts =+ 200: extract in no parts =+ 300: extract in all parts, but change size to p6=1

P7 = type font: = 0: Code16 _00 = 1: Code16 _02 = 2: Primitive (stick letters from *.DAT initialization file) = 3: Score's Bodoni Roman Numerals [crude] (from BODN0.DRW) = 4: Score's Bodoni Italic Numerals [crude] (from BODI0.DRW) = 5: Code16 _00 = 6: Score's meter number font [well-designed] (from MUSI0.DRW) = 7: Score's Bodoni Bold Numerals [crude] (from BODB0.DRW) = 8: Score's Bodoni Bold Italic Numerals [crude] (from BDIB0.DRW) = 9: Code16 _01 = 10: Code16 _01 = 11: Code16 _03 = 12: Code16 _00 = 1000 to 1089: corresponds to Code16 _00 to _89

P8 = 0: no enclosure 1, >=3, or <= -1 : circle around number 2: box around number

P9 = -1: This is a temporary value used to center a number over the closest --> Entering CM with the item selected will produce the same effect

P10 = Thickness of circle or box; use integer values

P11 = Horizontal size

P12 = Vertical size

P13 = Adjusts the vertical position of the number relative to its enclosure.

P14 = Adds space between digits

P15 = horizontal offset of number (and enclosed box/circle) from P3 position.

P16 = 1 makes justification routine sensitive to single-digit number in p5. This can be used with a P15 offset. The main purpose for this is when fingering numbers must be placed to the left side of chord notes; therefore CN-center the numbers on the chord, offset to the left with P15, and set P16 = 1 so they are accommodated during the justification routine of LJ.


**Verified implementation:** P16=1 participates in native J/Justify, JUST4 and PAGE4 spacing. P15 controls the displacement. Original tests also show this for a two-digit number; the spacing allowance is independent of P6 size.
