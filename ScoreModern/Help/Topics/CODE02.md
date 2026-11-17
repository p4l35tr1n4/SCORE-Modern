# Code 2 · Rests

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 2 -- Rests -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horizontal pos. |
| `P4` | vertical pos. |
| `P5` | rest type |

| Parameter | Meaning |
| --- | --- |
| `P6` | dots |
| `P7` | rhythmic time value |
| `P8` | number over |
| `P9` | centering |
| `P10` | horz. offset |

| Parameter | Meaning |
| --- | --- |
| `P11` | num hgt. |
| `P12` | num size |
| `P13` | cues |
| `P14` | num font |
| `P15` | rest sz factor |
| `P16` | num horz offset |

```text
╔════════════════════╗
║        p17         ║
║ quarter-rest count ║
║ of multi-bar rests ║
║  (PAGE-generated)  ║
╚════════════════════╝
```

=============== CODE 2 creates: =============== * Rests (including repeat bars and multi-bar rests)

P4 = vertical location.  For grace (cue-sized) rests add 100 to P4 (or -100 if P4 is negative.)  This is equivalent to p15 = .6

```text
P5 rest types: 0 = quarter       -1 = half
               1 = eighth        -2 = whole
               2 = sixteenth     -3 = breve (double whole)
               3 = 32nd          -4 = repeat bar sign
               4 = 64th          -5 = long, multiple bar rest  (Must have
                    etc.               number in P8, and must use P9 to
                                       be centered.)
                                 -6 = narrow double whole
                                 -7 = 4-bar rest
```

```text
European-style multi-bar rests. (The appropriate number should be put in P8.)
  P5 = -11 = 1 bar rest     P5 = -14 = 4 bar rest     P5 = -17 = 7 bar rest
  P5 = -12 = 2 bar rest     P5 = -15 = 5 bar rest     P5 = -18 = 8 bar rest
  P5 = -13 = 3 bar rest     P5 = -16 = 6 bar rest     P5 = -19 = 9 bar rest
```

P6 holds the number of dots needed when 1 <= p6 and p6 <= 4.

If P6 <= -1 the rest becomes invisible.  Invisible rests are sometimes needed so the automatic alignment system can work properly.  The main input routine (IN) will often require invisible rests when there must be more than 1 musical voice on only a partial section of a single staff.

If P6 is >= 5 the dots disappear and a horizontal line will appear above the rest. This line may be used for visual centering of rests.  (The line will NOT appear in hard copy output.)  The length of this line equals the value of P6.

P7 holds rhythmic time values (always needed for automatic spacing systems). For a normal quarter rest P7=1; for an eighth, P7=.5; for a half, P7=2; for a whole rest, P7=4 (i.e. 4 times as long as a quarter rest). A triplet eighth = .3333, or 1/3 of the time of a quarter.

P8 holds the number to be put over a rest indicating a certain number of bars rest to be counted. = -1 puts a fermata over a rest. = -2 puts a fermata under a rest. (The height of the fermata in each case can be adjusted with p11.)

P9 = Centering position of whole rests and p3 location of end of multi-bar rests.

When P9 is set to -1 the rest will automatically be centered between the position given in P3 and the position of the next bar line found. The program then inserts the actual position value into P9.  If the music is shifted due to justifying or re-aligning you must enter edit mode and reset P9 to -1 to re-center the rest. (Or use the CW command.)

When multibar rests are centered with p9 = -1, the resulting value in p9 will equal the horizontal location of the right end of the multibar rests (in standard p3 units).

--> Editing shortcut: CW.  Typing CW (center whole rest) has the same --> effect as setting P9 to -1.  CW and P9 work only when P5 is < -1.

P10 = horizontal displacement of rest from normal position as set by P3.

P11 = height of number or height of fermata over rest (may be raised or lowered).

P12 = size of number over rest.  Default size = 1. --> A negative number creates an inverted mirror image of the number.

P13 = Instrument ID of received cued material during part extraction in PAGE. When a part has a whole measure rest, P13 can hold the number of the instrument where cue notes are to be found.  (The instrument numbers must be given in P9 of each staff.)

P14 = Font override for numbers over rests; the default (0=6) is from Score's meter number font (from MUSI0.DRW), which is set by the REST setting setting in PREF.SCR.  The possible values are identical to the P7 font codes in Code10:

= 0:  Score's Meter Number Font [well-designed] (from MUSI0.DRW) = 1:  Code16 _02 = 2:  Primitive (stick letters from *.DAT initialization file) = 3:  Score's Bodoni Roman Numerals [crude] (from BODN0.DRW) = 4:  Score's Bodoni Italic Numerals [crude] (from BODI0.DRW) = 5:  Code16 _00 = 6:  Score's Meter Number Font [well-designed] (from MUSI0.DRW) = 7:  Score's Bodoni Bold Numerals [crude] (from BODB0.DRW) = 8:  Score's Bodoni Bold Italic Numerals [crude] (from BDIB0.DRW) = 9:  Code16 _01 = 10: Code16 _01 = 11: Code16 _03 = 12: Code16 _00 = 1000 to 1089: corresponds to Code16 _00 to _89

P15 = General rest size factor.  (But remember that the various automatic spacing and justifying features of Score's LJ will NOT take into account rest size changes made with P15.) --> To match cue size, set p15 = .6 --> A negative number creates an inverted mirror image of the rest at (increasing) sizes.

P16 = Horizontal offset of rest number.

P17 = The number of quarter rests comprising a multi-bar rest (P5=5, P5<= -11) This value is added by PAGE when extracting a part and creating multi- bar rests.  For example, if the meter is 3/4 and there is a multi-bar rest spanning 2 measures, then P17 will equal 6, because 3x2=6. Notice that the value in P7 of multi-bar rests is 2 by default, which creates identical widths for all of them following LJ, regardless of the number of measures they span. Therefore, you might benefit your parts by resetting P7 of multi-bar rests to some fractional value of P17 (not the entire amount, as that would be excessive in most cases), perhaps a quarter of the stated value, but never allowing the result to be less than 2. This will create multi-bar rests of lengths relatively proportionate to the number of beats that they span.
