# Code 10 · Numbers and rehearsal letters

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 10 -- Numbers -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | number |
| `P6` | size |
| `P7` | font |

| Parameter | Meaning |
| --- | --- |
| `P8` | circle,box |
| `P9` | box height |
| `P10` | box thick |
| `P11` | horz. box sz. |
| `P12` | vert. box sz. |

```text
╔═══════════╦═════════════╦════════╗
║    p13    ║     p14     ║   p15  ║
║ vert.pos. ║ space btwn. ║  horz. ║
║  in box   ║   digits    ║ offset ║
╚═══════════╩═════════════╩════════╝
```

================ CODE 10 creates: ================ * Numbers, used for: + measure numbers + page numbers + rehearsal numbers

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
