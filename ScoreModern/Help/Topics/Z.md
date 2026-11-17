# Z commands

## In SCORE Modern

The executable defaults retain zoom for bare `Z` and use 2 for bare `ZZ`. Preferences also offers Tom’s defaults of 1 and 3 and lets you choose the framing mode.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

"Z" = zoom.  For changing the size or position of the image in the window.

To change the size of the image...

Z n x y = zoom to n times the normal size and move percentage positions x and y to center of screen.  The default screen display is Z 1 50 50.  This means zoom to size 1 and place the horizontal point which is 50% of the way across the screen to the center point, etc. (which, of course, is redundant.)

Z 2 20 10 zooms to double size and takes the point 20% of the distance from the left to right and 10% of the distance from bottom to top and places it at the center of the screen.

```text
╔════════════════════════════════════════════════════════════════════════════╗
║ Z alone is the same as Z 1, but all vectors are recomputed.  The Z command ║
║  MUST always be used if the staff lines(code 8) themselves are edited, etc.║
╚════════════════════════════════════════════════════════════════════════════╝
```

To change the position of the image...

```text
ZL n = move the "window" n% of the screen width to the left.
ZR n =  "    "      "    "   "  "     "     "    "  "  right.
ZU n =  "    "      "    "   "  "     "   height upward
ZD n =  "    "      "    "   "  "     "   height downward
```

The number n is "sticky" with the left-right, up-down commands.

Z 1 = bring image back to normal size - staff 1 at bottom of screen.

ZZ n = zoom to size n with center of screen located at last item entered or edited.  If n is not given the default zoom size is 3.

**** The left single quote (`) does the same as ZZ 1 -- no <enter> needed.

```text
╔════════════════════════════════════════════════════════════════════════════╗
║   All Z commands will work when in edit mode ---                           ║
║     BUT only BEFORE any of an item's parameters have been changed.         ║
╚════════════════════════════════════════════════════════════════════════════╝
```
