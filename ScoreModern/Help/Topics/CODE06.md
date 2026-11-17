# Code 6 · Beams

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 6 -- Beams -- Edited by Thomas M. Brodhead

```text
                         ╔═══════════╦═══════════╗
parameter structure:     ║     p1    ║     p2    ║
                         ║ code num. ║ staff num.║
                         ╚═══════════╩═══════════╝
           ┌──────────────────────────────────────────────────────┐
           │                  ┌───────────────────┐               │
```

| Parameter | Meaning |
| --- | --- |
| `P3` | left horizontal pos. |
| `P4` | left vertical pos. |
| `P5` | right vert. pos. |
| `P6` | rt. horz. pos. |

| Parameter | Meaning |
| --- | --- |
| `P7` | stem dir., num. of beams |
| `P8` | number over beams |
| `P9` | horz. offsets |

| Parameter | Meaning |
| --- | --- |
| `P10` | 2nd beam group |
| `P11` | lft.pos |
| `P12` | rt.pos |
| `P13` | 3rd beam grp. |
| `P14` | lft.pos |
| `P15` | rt.pos |

| Parameter | Meaning |
| --- | --- |
| `P16` | # horz. offset |
| `P17` | thickness |
| `P18` | size |
| `P19` | # vert. offset |

============== CODE6 creates: ============== * Beams * Tremolos

***** BEAMS *****

BASIC PARAMETRIC DISTINCTION: Beams have a non-zero value in the one's digit of P7

P4 = vertical location of LEFT end of beam. --> For matching grace (cue-sized) notes add 100 to P4 --> (or -100 if P4 is negative).  This is equivalent to p15 = .6

==========

P7 = is a two-digit number that implies stem direction and number of beams.

The ten's digit indicates the stem direction of notes attached to the beam. 1 = stems up, 2 = stems down.

The one's digit holds the number of full beams drawn between positions P3 and P6. --> P7 = 12: stems up, 2 full beams --> P7 = 23: stems down, 3 full beams

P8 = tuplet number to be centered above or below the full beams. --> The number in P8 uses the typeface indicates by FM02.PSC --> (i.e. the _02 font).  The size is equivalent to Code6 p6=.85

When EDITING ONLY: A negative value in P8 is used to split beams into smaller units. E.g., if 6 notes are found under a continuous double beam, entering '8 -3' will cause the appearance of 1 long single beam and 2 secondary beams attaching 3 notes each.  The negative number indicates the last note to be included under the 1st secondary beam.  In this case the number may have 1 or 2 digits.

If P8 has 3 or 4 negative digits, the ORIGINAL beam will have 2 partials and a NEW beam will be created made up of 1 or 2 partials.  In this case the sum of the digits MUST equal the total of the notes involved.

--> If there are 9 notes under a beam, P8 = -333 creates 3 partials, --> 1 for each 3 notes. --> If there are 10 notes under a beam, P8 = -3223 creates 4 partials, --> encompassing 3 notes, 2 notes, 2 notes, and 3 notes.

After this feature is used P8 will reset itself to 0.

P9 = Notehead-width offset of primary, secondary, and tertiary beams in cross-staff beams. (Use P13=0, P14=#, P15=# for simple offsets of non-cross-staff beams.)

P9 employs 3 digits, using 0, 1, 2, 3 only.

Here these digits will be labeled "ABC", where A is the hundred's unit, B is the ten's unit, and C is the one's unit.

Digit "A" -> P3 and P6 (left and right ends of the main beam). Digit "B" -> P11 and P12 (left and right ends of first added partial beam). Digit "C" -> P14 and P15 (left and right ends of second partial beam).

The role of these digits is as follows:

0=no offset, 1=left beam end is offset, 2=right end offset, 3=both offset

---> When editing, set P9 to -1 to create the proper values automatically.

P9 insures that the beam end points will exactly meet the note stems.

P10, P11, P12 -- P13, P14, P15

These parameters are used to create a secondary and a tertiary beam on the primary beam (expressed in p7 exlusively).

P10, P11, P12 are used to create a secondary beam P13, P14, P15 are used to create a tertiary beam

The role of the parameters in each group of 3 is exactly the same, so only P10, 11, 12 will be described. (See below re: special usage of P14 and P15 for offset notes of "simple" beams with no secondary or tertiary beams.)

P10 is a 2-digit number: * The ten's unit indicates the vertical origination of the beam. * The one's unit indicates how many beams will be drawn.

The one's unit is simple to understand, but the ten's unit is subtler:

* If the ten's unit is 0, then the secondary beam will originate AT the same vertical level as the primary beam (i.e. superimposed on that beam).

* If the ten's unit is 1, then the secondary beam will originate ONE beam level beyond the primary beam (i.e. 1.5 P4 units BELOW the primary beam if the beam is UPSTEMMED [i.e. its *P7* ten's unit is 1] or 1.5 ABOVE the primary beam if the beam is DOWNSTEMMED [i.e. its *P7* ten's unit is 2]).

* If the ten's unit is 2, then the secondary beam will originate TWO beam level beyond the primary beam (i.e. 3 P4 units BELOW the primary beam if the beam is UPSTEMMED or 3 units ABOVE the primary beam if the beam is DOWNSTEMMED).

...and so on for all ten's units to 9, where the origination point in P4 units is 1.5-times-the-ten's-units away from the primary beam.

* Negative values in the ten's unit simply move the origination location of the secondary beam BELOW the primary beam (in the case of UPSTEMMED beams) or ABOVE the primary beam (in the case of DOWNSTEMMED beams), following the same 1.5-times-the-ten's-unit principle.

--> The number of beams indicated in the one's unit will be built UP from the origination point in the case of UPSTEMMED beams, and they will be built DOWN from the origination point in the case of DOWNSTEMMED beams.

P11 = the horizontal location of the LEFT end of the secondary beam. --> If p11 = -1, then whatever value is found in p3 is used for p11.

P12 = the horizontal location of the RIGHT end of the secondary beam. --> If p12 = -1, then whatever value is found in p6 is used for p12.

If both p11=0 and p12=0, then the secondary beam "floats": each end is moved from the normal stem attachment location by 3.5 * StaffSize (i.e. the left end is displaced RIGHTWARD and the right end is displaced LEFTWARD).

If p11 = -1 and p12 = 0, then a beam "stub" 2.7 * StaffSize in p3 length is drawn from the LEFT end of the beam, with NO continuation rightward to the right end of the beam.

If p11 = 0 and p12 = -1, then a bean "stub" 2.7 * StaffSize in p3 length is drawn from the RIGHT end of the beam, with NO continuation leftward to the left end of the beam.

P13, P14, P15 work identically to P10, P11, P12 in every regard. --> Ensure that when a secondary beam is required, create it using --> p10-12, not p13-15; it is poor data form to leave p10-12 empty and --> create the secondary beam using p13-15.

Exceptional offset usage of P14, P15:

For "standard" beams with no secondary beams in p10-12 or tertiary beams in p13-15, P14 and P15 may be used to offset the LEFT and RIGHT ends of the primary beam to match p10 offsets of the associated notes.

If in a "standard" beam with no "partials" (secondary or tertiaries) the INITIAL note has p10=#, then setting p14=# will offset the beam by the same amount of the # offset.

Likewise, if in a "standard" beam the FINAL note has p10=#, then setting p15=# will offset the beam by the same amount of the # offset.

In these cases, P13 *MUST* be 0.

This offset is preferable to using the offsets offered by P9 (described above), which should typically be reserved for cross-staffed beams.

***** TREMOLOS *****

BASIC PARAMETRIC DISTINCTION: Tremolos have a zero value in the one's digit of P7, and P7 is negative (e.g. p7 = -10 or p7 = -20).

P4 = vertical location --> For matching grace (cue-sized) notes add 100 to P4 --> (or -100 if P4 is negative).  This is equivalent to p18 = .6

Stemmed tremolos on notes (without associated beams) ==================================================== P7 = -10 (stems up) or -20 (stems down).

P10 = "XY" where X = displacement, Y = number of tremolo beams.

P12 = width of the tremolo beams (when p11=0) = 0 = the default width = 4. (See p17 in "Common Controls" section for a related, useful control.)

P14 = Offsets stemmed tremolos in manner identical to Code1 P10 (in which p14=10 offsets exactly a notehead's width to the right and p14=20 offsets exactly a notehead's width to the left; other values, negative and positive, are relative displacements). This is useful for offsetting a stemmed tremolo to the location of a note offset with p10.

Parallel tremolos on beams: =========================== For each tremolo, create a full beam with p3 and p6 identical to the beam to which the tremolos must be parallel.  Set p11 = desired p3 location of each parallel tremolo (i.e. the p3 location of its associated note), and p7 and p10 as described above.

Angled tremolos on beams: ========================= Just create standard "stemmed" tremolos (with p7 and p10 as described above) with p3 = p3 of associated note locations.

p5 = 0 (always!) p6 = 0 (always!)

Floating tremolo beams between note stems: ========================================== P7 = 10 or 20. --> The beams will be suspended between positions in P3 and P6.

P10 = the number of beams. --> The ten's unit is the stem direction: 1 = upstemmed; 2 = downstemmed --> The one's unit is the number of tremolo beams.

```text
P11 = RIGHTWARD offset of floating tremolo from note on LEFT end
      of tremolo.  Default = 0 = 3.5
P12 = LEFTWARD offset of floating tremolo from note on RIGHT end
      of tremolo.  Default = 0 = -3.5
```

Tremolos within standard beams: =============================== Create a standard "simple" continuous beam with p7. Then:

If both p11=0 and p12=0, then the secondary beam "floats": each end is moved from the normal stem attachment location by 3.5 * StaffSize (i.e. the left end is displaced RIGHTWARD and the right end is displaced LEFTWARD).

***** COMMON CONTROLS FOR BOTH BEAMS AND TREMOLOS *****

P16 = percentage horizontal displacement of tuplet numbers. = 0 = default .5 (50%).

EDITING P16 TUPLET NUMBER CENTERPOINTS --> To edit the P16 centerpoint, depress <Shift>+[F10] and press left or --> right arrow keys (<- or ->).  Stop this mode by depressing <Shift>+[F9]

P17 = beam/tremolo thickness factor. (0 = default, .9 = thinner, 1.1 = thicker)

P18 = beam size factor for beams attached to notes with P15 size factor set. --> If notes have been resized with P15, use the same value of the note's --> p15 for the associated beam's P18.

P19 = shifts the tuplet number up or down.
