# D commands

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

```text
"D" commands: DE =group delete           DEL <name> =Delete a file from disk
              DP =make staves disappear  DPY =export
              DRV =Default drive letter  DEC =DECombine
              DI = dim.
```

In edit mode only:  DE = delete the item Dn = move the item down 'n' scale steps

DE, the group delete, works like the move command except that only the 1st staff- position-code number prompt is given.

"Type staff #, left position, right position, [code #, upper staff #]"

Up to 5 numbers may be typed.  The 1st number should be that of the lowest staff involved in the delete - unless the command is to involve all staves at once, in which case type any number greater than the highest possible staff number (32 in SCORE version 3). The 2nd and 3rd numbers set the limits of the material to be deleted. If the the 3rd number is zero then the default right hand limit will be position 220.  If the the 4th number is not zero then only items with that code number will be deleted. If a 5th number appears it will be the number of the uppermost staff to be involved in the deletions.

Example -  prompt response: 2 45 137 0 5

This deletes all material on staves 2 - 5 found between positions 45 and 137.

***** You will always be given the chance backup from the DE command.

Example -  prompt response: 2 45 137 2 5

This deletes all rests (code 2) on staves 2 - 5 found between positions 45 and 137.

Example -  prompt response: 3

This deletes everything on staff 3.

***** Special feature: If you type '3 -99', the -99 causes EVERYTHING to be deleted EXCEPT material on staff 3.  Use this feature with extreme care!

DRV = The current drive letter is displayed and a new drive letter is requested.

DI = dim. This command is a shortcut for typing a code 9 item.  It should normally be followed by 2 or 3 numbers (parameters 2, 3, 4).

```text
di 3 45 -2   will put "dim." at position 45 on staff 3,
             vertical position -2.
```

DEC = DeCombine. Saves each staff of the current file to a seperate file.

**** The following work only while in edit mode. ****

DE deletes the item currently active in edit mode.  If any changes were made to the item just before typing DE, the DE command will be ignored. After a deletetion, the next item in the list will come into edit mode.

Dn moves the item down 'n' steps.  The value of 'n' is "sticky", i.e. typing the D again, with no number, will move the item down again.  The U command (up) works the same way.  The same stored value of 'n' is used by both D and U.

DP = make selected staves disappear. Use this to avoid exceeding the graphics vector limit. (c.32000 in "big" version)

DP can be followed by up to 32 numbers.  Each number will represent the staff that will disappear or re-appear when the Z command is given.

The numbers following DP will operate on the flip-flop principle.  That is, if DP 2 <enter> is typed, and then Z<enter>, all items attached to staff 2 will disappear from the display.  (The items will still remain in memory however.)  If DP 2 <enter> followed by Z <enter> is typed again, the items will reappear.

DP 99 <enter> Z <enter>  makes data on all staves reappear. DP -1 <enter> Z <enter>  makes data on all staves disappear. DP alone displays current DP status.

To operate on a limited group of staves at once, type: DP X 0 Y Here, X is the lowest staff involved and Y is the highest. DP 3 0 7 will make staves 3 through 7 disappear or reappear.

DPY = Export items on the DisPlaY to a vector list file that can be imported into DRAW. Also...  (DPY -1 displays the vector list on the screen.) (DPY 1 causes the parameter array to be displayed on the screen.)
