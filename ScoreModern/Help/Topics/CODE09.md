# Code 9 · Library symbols

## In SCORE Modern

Open **Symbol Browser…** from the context menu outside the page to look up symbol numbers by drawing bank. Right-click a symbol to copy its number or open its bank slot in DRAW.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 9 -- Symbol library -- Edited by Thomas M. Brodhead parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | library number |
| `P6` | horz. size |

| Parameter | Meaning |
| --- | --- |
| `P7` | vert. size |
| `P8` | thickness |
| `P9` | rotation |
| `P10` | laser thickness |

```text
╔══════════════════╦════════════╦═════════════════════╗
║     p11 = -1:    ║    p15     ║        p16          ║
║ BODI?.DRW glyph  ║ horizontal ║ interletter spacing ║
║ substitution for ║   offset   ║ override of p5 for  ║
║   Code9 text     ║            ║   Code9.DAT text    ║
╚══════════════════╩════════════╩═════════════════════╝
```

=============== CODE 9 creates: =============== * Symbol library items * CODE9.DAT library text items

```text
P4 = Vertical position. Note that the normal vertical position (position at
     P4 = 0) for Code9 library items varies.
```

P5 = Glyph number from symbol library; default library chart provided at the end of this help screen.

============================= P5 and the CODE9 text library =============================

P5 = 3000 to 3069 is used to access a special library of text items found in a file whose default name is CODE9.DAT. Other text-name library files may be created with other names; the exact name (including its PATH) is *ALWAYS* specified with the CODE9 entry in the PREF.SCR file.  (If no filename is specified, or if the specified filename is not found, C:\LIB\CODE9.DAT is sought for reading by the SCORE; if it cannot be accessed, no symbol will be displayed.)

The CODE9 text library precludes the use of DRW files LICCO.DRW - LICCU.DRW for use by Code9 (although their contents could in fact be invoked as Code11 items).

CODE9 text library file format: Comments may be created by preceding a line with "%", hence:

%This is a comment in the CODE9.DAT file

The entries in the file take the syntax: [p5] [text preceded by font code]

For example:

%For various Meyerbeer operas 3000 _00>>a 2 3001 _02l<<eger 3002 _00changez en _02Re

There can be no more than 70 entries (thus p5=3000-3069 is the possible span) and the total number of text characters cannot exceed 700, hence an average length of 10 characters per entry would meet the maximum allowance.

The resulting text is scaled at the equivalent size of Code16 p5=1.05 and p6=1.05.

--> To make the P6 size of the Code9 item match that of an equivalent --> Code16 item, set the Code9 P6 = (#/1.05), where # is the desired --> Code16 P6 size.  So if P6=1 is desired, the Code9 p6 value would be --> .9523809, because 1/1.05 = .9523809. --> --> But that's only half the issue: To make the P5 of the Code9 item --> match the P5 of the equivalent Code16, set *P16* of the Code9 item --> to the equivalent Code16 P5, using "1" if "0" is the desired (default) --> P5 of the Code16.  P16 hence overrides the "invisible" Code16 p5=1.05 --> setting of the Code9 item, and thus any value may be used (Code16 p5=1.23 --> would be Code9 p16=1.23).

```text
P6 = Horizontal size factor; a negative number reflects the glyph.
P7 = Vertical size factor; a negative number inverts the glyph.
(P6 size 0 = size 1.)
```

If P7 = 0, P7 defaults to the value of P6; the symbol therefore scales horizontally and vertically to the size of P6.

--> If P6= X and P7= -X, an inverted glyph is created. --> If P6= -X and P7 = X, a reflected glyph if created. --> If P6= -X and P7= -X, an inverted & reflected glyph is created.

For PAGE program part extractor: P6 =+ 100: extract in all parts =+ 200: extract in no parts =+ 300: extract in all parts, but change size to p6=1

P8 = thickness, using the form "x0y", where: x = the added vertical thickness y = the added horizontal thickness. -->  If P8 = -1, outline only prints (no filler).

P9 = rotates the symbol clockwise.

P10 = outline thickness control.  P10 adds (or subtracts) pixels to the basic line thickness established by the prompts for EPS/PS printing.

--> Negative values reduce or "thin" the outline of the glyph --> Positive values increase or "thicken" the outline of the glyph

E.g., for small staff sizes, setting p10 = -4 might be optimal for ensuring that the symbol remains crisp and distinct, rather than "puffy," which is typical at the default p10=0 setting. --> P10 correlates with P8, with P10 = 1 <--> P8 = 101, etc. --> This parameter functions identically to P10 of Code3.

P11 = -1 will substitute [crude] characters from the BODI?.DRW files for the letters in the Code9 text strings, which are normally equivalent to the following Code16 items:

```text
p5 = 45: "_028va"            p5 = 73: "_02molto"
p5 = 46: "_028ba"            p5 = 78: "_00a tempo"
p5 = 47: "_0215ma"           p5 = 79: "_02espr."
p5 = 63: "_02cresc."         p5 = 80: "_02meno"
p5 = 64: "_02dim."           p5 = 81: "_02pi>>u"
p5 = 65: "_02rit."           p5 = 85: "_02subito"
p5 = 66: "_00pizz."          p5 = 86: "_02poco"
p5 = 67: "_00arco"           p5 = 111: "_02decresc."
p5 = 68: "_02accel."         p5 = 113: "_02rall."
p5 = 72: "_02gliss."         p5 = 114: "_02mosso"
```

These Code9 text strings are sized equivalently to Code16 p5=1.05 and p6=1.05 when the Code9 p6=1 (=0).  The substitution of BODI?.DRW char- acters in place of PostScript font characters when P11 = -1 is likely a hold-over from the era when dot matrix printers were still in use.

See the discussion above on the CODE9.DAT library for a formula to over- ride the p6=1.05 size. Sadly, no conversion of p5 is possible, however.

P16 - Interletter spacing override for Code9 text items.  By default, the p5=3000+ text items have the equivalent Code16 size of p5=1.05 (for interletter spacing) and p6=1.05.  To override the interletter spacing (p5), set p16=1, which is the same as p5=0 or p5=1 in Code16. Other values for p5 in Code16 may be used in p16 here (e.g. Code16 p5=1.23 would translate to Code9 p16=1.23, etc.)

======================== P5 library number table: ========================

```text
LIBRA.DRW                LIBRB.DRW                LIBRC.DRW
0 = treble clef          10 = double sharp        20 = note tail
1 = bass clef            11 = black note          21 = wedge accent
2 = alto clef            12 = half note           22 = normal accent
3 = TAB clef             13 = whole note          23 = tenuto (dash)
4 = percuss. clef        14 = breve               24 = staccato (dot)
5 = curved brace         15 = white diamond       25 = wedge/dot
6 = flat                 16 = black    "          26 = accent/dot
7 = sharp                17 = x note              27 = tenuto/dot
8 = natural              18 = common time (C)     28 = down bow
9 = double flat          19 = alla breve (¢)      29 = up bow
```

```text
LIBRD.DRW                 LIBRE.DRW                LIBRF.DRW
30 = harmonic             40 = breve rest          50 = pppp
31 = fermata              41 = whole rest          51 = ppp
32 = arsis sign           42 = half rest           52 = pp
33 = thesis sign          43 = quarter rest        53 = p
34 = mordant              44 = eighth rest         54 = mp
35 = inverted mordant     45 = "_028va"            55 = mƒ
36 = heavy wedge          46 = "_028ba"            56 = ƒ
37 = tr                   47 = "_0215ma"           57 = ƒƒ
38 = +                    48 = pedal sign          58 = ƒƒƒ
39 = measure repeat       49 = pedal filler        59 = ƒƒƒƒ
```

```text
LIBRG.DRW                 LIBRH.DRW                LIBRI.DRW
60 = ƒp                   70 = Ped.                80 = "_02meno"
61 = sƒ                   71 = star                81 = "_02pi>>u"
62 = sƒz                  72 = "_02gliss."         82 = snap pizz.
63 = "_02cresc."          73 = "_02molto"          83 = circle
64 = "_02dim."            74 = segno               84 = old pedal sign
65 = "_02rit."            75 = turn                85 = "_02subito"
66 = "_00pizz."           76 = //                  86 = "_02poco"
67 = "_00arco"            77 = system divider      87 = guitar chord grid
68 = "_02accel."          78 = "_00a tempo"        88 = open circle
69 = arrow                79 = "_02espr."          89 = black circle
```

```text
LIBRJ.DRW                LIBRK.DRW                LIBRL.DRW
90 = circle+             100 = early mus. notes   110 = F clef
91 = ( )                 101 =   "                111 = "_02decresc."
92 = (>)                 102 =   "                112 = coda sign
93 = ornament            103 =   "                113 = "_02rall."
94 =    "                104 =   "                114 = "_02mosso"
95 = diamondX            105 =   "                115 = .//.
96 = vertical bracket    106 =   "                116 = small dbl. whl. rest
97 = (flat)              107 =   "                117 = 4-bar whole rest
98 = (#)                 108 =   "                118 = comma
99 = (natural)           109 =   "                119 = accent/tenuto ≥
```

```text
LIBRM.DRW                LIBRN.DRW                LIBRO.DRW
120 = P(edal)            130 = guitar notation    140 = (b)
121 = segno(large)       131 =    "  (also see    141 = (#)
122 = coda(small)        132 =    "   990-996)    142 = (nat.)
123 = ƒz                 133 =    "               143 = (bb)
124 = s                  134 =    "               144 = (##)
125 = r                  135 =    "               145 = 1/4 flat
126 = z                  136 =    "               146 = 3/4 flat
127 = rƒ                 137 =    "               147 = 1/4 #
128 = rƒz                138 =    "               148 = 3/4 #
129 = sƒƒz               139 =  perc. half note   149 = early music
```

```text
LIBRP.DRW                LIBRQ.DRW                LIBRR.DRW
150 = (pppp)             160 = sƒƒ                170 = 0 meter
151 = (ppp)              161 = sƒƒƒ               171 = 1/4
152 = (pp)               162 = sƒpp               172 = 1/2
153 = (p)                163 = sƒp                173 = 3/4
154 = (mp)               164 = sƒƒp               174 = (note)
155 = (mƒ)               165 = mƒpp               175 = (b-note)
156 = (ƒ)                166 = mƒp                176 = (#-note)
157 = (ƒƒ)               167 = ƒpp                177 = (nat.-note)
158 = (ƒƒƒ)              168 = ƒƒpp               178 = early music
159 = (  )               169 = ƒƒp                179 =   "     "
```

```text
LIBRS.DRW                LIBRT.DRW                LIBRU.DRW
180 = / ,                190 = early music        200 = (Ped.)
181 = (,)                191 = signum congruent.  201 = /\/
182 = 1/4 flat         * 192 =   "     "          202 = \/\
183 = 3/4 #              193 =   "     "          203 = R-
184 = 1/4 #              194 =   "     "          204 = H-
185 = 3/4 flat           195 =   "     "          205 = N-
186 = 1/4 #              196 =   "     "          206 = ¬
187 = 1/4 flat           197 =   "     "          207 = breve
188 = l.v. down (slur)   198 =   "     "          208 = (
189 = l.v. up (slur)     199 = SCORE logo         209 = guitar barre
```

```text
LIBRV.DRW                LIBRW.DRW                LIBRX.DRW
210 = n                  220 = <>                 230 = 13
211 = harp stop          221 = I                  231 = i
212 = harp stop          222 = II                 232 = ii
213 = fermata(square)    223 = III                233 = iii
214 =   "   (triangle)   224 = IV                 234 = iv
215 = ...                225 = V                  235 = v
216 = shape note         226 = VI                 236 = vi
217 =   "    "           227 = VII                237 = vii
218 =   "    "           228 = 6-4                238 = 4-3
219 =   "    "           229 = 6-5                239 = 4-2
```

```text
LIBRY.DRW                LIBRZ.DRW                LIBSA.DRW
240 = 0                  250 = b1                 260 = \6
241 = 1                  251 = b2                 261 = b7
242 = 2                  252 = b3                 262 = b
243 = 3                  253 = b4                 263 = #
244 = 4                  254 = b5                 264 = nat.
245 = 5                  255 = #1                 265 = nat.b
246 = 6                  256 = #2                 266 = nat.#
247 = 7                  257 = #3                 267 = nat.bb
248 = 8                  258 = #4                 268 = nat.##
249 = 9                  259 = #5                 269 =
```

```text
LIBSB.DRW                LIBSC.DRW
270 = circle             280 = sƒƒƒz
271 = circle filled      281 = sƒƒƒƒz
272 =   "  1/2 filled    282 = sƒzp
273 = /\/\/\             283 = sƒƒzp
274 =                    284 = sƒƒƒzp
275 =                    285 = C (large)
276 =                    286 = ¢ (large)
277 =                    287 =
278 =                    288 =
279 =                    289 =
```

```text
Percussion symbols
LIBTI.DRW                LIBTJ.DRW                LIBTK.DRW
600 = 2 hrd. sticks      610 = 2+2 hrd. sticks    620 = 2 sft. sticks
601 = 2 med.   "         611 = 2+2 med.   "       621 = 2 hrd.   "
602 = 1 small hard       612 = 2 med. sticks      622 = 2 med.   "
603 = 1 hrd. stick       613 = 2+2 sft.   "       623 = 2 heavy  "
604 = 1 med.   "         614 = 2 hrd.   "         624 = 2 sft.   "
605 = 2 sft. sticks      615 = 2 hrd.   "         625 = 2 hrd.   "
606 = 1 sft. stick       616 = 2 hrd.   "         626 = triangle
607 = triangle           617 = 2 sticks           627 = 1 mallet
608 = crash cymbals      618 = 2 light sticks     628 = mallet/stick
609 = susp. cymbal       619 = 1 stick            629 = snr. drm. stick
```

```text
LIBTL.DRW                LIBTM.DRW                LIBTN.DRW
630 = 2 med. sticks      640 = 2 sticks           650 = 1 sft. stick
631 = 1 med. stick       641 = snares on          651 = 1 hrd.   "
632 = 2 sticks           642 = snares off         652 = 1 hrd.   "
633 = 1+2 hrd. sticks    643 = 2 snares on        653 = 1 sft.   "
634 = 2 brushes          644 = 2 snares off       654 = 1 sft.   "
635 = 1 brush            645 = 2 small hrd. stk.  655 = 1 hrd.   "
636 = 3 brushes          646 = 2   "   sft.  "    656 = 1 hrd.   "
637 = antique cymb.      647 = tambourine         657 = 1 sft.   "
638 = cowbell            648 = sleighbells        658 = 1+2 sft. sticks
639 = handbell           649 = maracas            659 = 2+1 sft.   "
```

```text
LIBTO.DRW                LIBTP.DRW                LIBTQ.DRW
660 = 2+1 hrd. sticks    670 = fingernail         680 = tubular chimes
661 = fingernail         671 = 2 fingernails      681 = empty mallet box
662 = 2 hands            672 = 1 sft. chime mal.  682 = Xylo
663 = 2+1 med. sticks    673 = 2 sft. chime mals. 683 = Vibr
664 = 1+2 med.   "       674 = 1 hrd. chime mal.  684 = Mari
665 = 1 med. stick       675 = 2 hrd. chime mals. 685 = Glock
666 = 1 hrd.   "         676 = 1 yarn stick       686 = Celes
667 = 1 sft.   "         677 = claves             687 = Keyboard
668 = left hand          678 = woodblock          688 = gong
669 = right hand         679 = bass drum          689 = tam-tam
```

```text
LIBTR.DRW                LIBTT.DRW                LIBTX.DRW
690 = hi-hat cymbal      710 = SCOR4 Logo         750 = 000  Brass
691 = spiral                                      751 = 100  Fingering
692 =                                             752 = 010
693 =                                             753 = 001
694 =                                             754 = 101
695 =                                             755 = 110
696 =                                             756 = 011
697 =                                             757 = 111
698 =
699 =
```

```text
LIBUM.DRW                LIBUN.DRW               LIBUO.DRW
900 = a  18-Century      910 = l                 920 = (
901 = b  Lute Tablature  911 = m                 921 = )
902 = c                  912 = n                 922 = elision
903 = d                  913 = 4                 923 = wiggle
904 = e                  914 = 5                 924 = /
905 = f                  915 = 6                 925 = x
906 = g                  916 = /a                926 =
907 = h                  917 = //a               927 =
908 = i                  918 = ///a              928 =
909 = k                  919 = R                 929 =
```

```text
LIBUP.DRW                LIBUQ.DRW                LIBUR.DRW
930 = 1/32 rhythm        940 = /b                 950 = 3/8 meter
931 = 1/16               941 = //b                951 = 2/4
932 = 3                  942 = ///b               952 = 3/4
933 = 1/16.              943 = :║:                953 = 6/8
934 = 1/8                944 = : (5)              954 = 9/8
935 = 1/8.               945 = : (4)              955 = 6/4
936 = 1/4                946 =                    956 = common (C)
937 = 1/4.               947 =                    957 =
938 = 1/2                948 =                    958 =
939 = 1/2.               949 =                    959 =
```

```text
LIBUS.DRW                LIBUV.DRW
960 = alla breve (¢)     990 = quarter   Guitar slashes,
961 = 2                  991 = half      stems down.
962 = 3                  992 = eighth
963 = .. (?)             993 = sixteenth
964 =                    994 = dotted quarter
965 =                    995 = dotted half
966 =                    996 = dotted eighth
967 =
968 =
969 =
```

Early Music Notation LIBUW.DRW  LIBUX.DRW  LIBUY.DRW  LIBUZ.DRW 1000-1009  1010-1019  1020-1029  1030-1039

Chant Notation LIBVC.DRW  LIBVD.DRW  LIBVE.DRW 1060-1069  1070-1079  1080-1085
