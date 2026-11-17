# A commands

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

"A" commands: A = group alter, AD = adjust stem lengths, AR = arco, AC = accel, AT = a tempo. In Edit Mode: A = align [after 'set'], AB = abort edit.

A -- Group Alter ---  The first prompt is: Type staff #, position 1, position 2, [code #, upper staff #]

At least 4 numbers must be typed.  If the staff number given is greater than the total possible (32), then all staves will be affected. Position 1 and 2 give the left and right horizontal limits for the group alter. The proper code number for the kind of item to be altered must be given.  If no 5th number is given items on only 1 staff will be altered. If a 5th number is typed then all the staves between that indicated by the 1st number and that by the 5th number will be altered.  If a zero is given for the 3rd number it will be taken to mean that you want the range to extend out to position 220 on the right side.

```text
2 0 59 1   prepares for alteration all notes (code 1) on
           staff 2 between position 0 and 59.
```

```text
1 0 0 3 4  prepares for alteration all clefs (code 3) on
           staves 1 through 4 between positions 0 and 220.
```

The second prompt is: Type P#, chng, P#, chng, P#, chng, ...

Up to 12 pairs of numbers may be typed.  The 1st of each pair must be a parameter number and the second is the new value of that parameter. (The same format is used for relative changes and equivalences as that used when in edit mode.)

```text
4 15  300 10  6000 3  puts 15 into parameter 4, adds 10 to
                      parameter 3 (relative change) and puts
                      into parameter 6 whatever value was found
                      in parameter 3 (equivalence).
```

These  changes then take place for all the items with the proper code number found in the range given in response to the 1st prompt.

(To back up from either of the prompts, type "b.")

AD [staff #, n] -- Adjust stem lengths of all notes to match beam's vertical position when stems (implied by notes' P5 and beams' P7) are going the same direction.

If a staff number greater than the total possible staves (i.e. >32) is typed, then note stems on all staves will be adjusted.

If a 2nd number [n] is typed (non-zero) then stems of notes and beams NEED NOT be going the same direction in order for adjustment to occur.

An "AD" should usually be done following the use of the "J" (justify) or "LI" (line up) commands.

(In some complex situations it may be best to move certain beams to other staves before doing an "AD" - then they must be moved back.)

AR = arco, AC = accel, AT = a tempo.

These commands are shortcuts for typing code 9 items.  They should normally be followed by 2 or 3 numbers (parameters 2, 3, 4).

```text
ar 3 45 14   will put "arco" at position 45 on staff 3,
              vertical position 14.
```

In Edit Mode only:

A = Align after Set -- See under "S", Set and Align.

AB = Abort edit.  Typing "AB" while in Edit Mode returns all parameters to their original values.
