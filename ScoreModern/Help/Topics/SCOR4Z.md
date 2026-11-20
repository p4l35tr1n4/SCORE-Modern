# Z commands · SCOR4

## In SCORE Modern Core

The executable defaults retain zoom for bare `Z` and use 2 for bare `ZZ`. Preferences also offers Tom’s defaults of 1 and 3 and lets you choose the framing mode.

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

"Z" = zoom.  For changing the size or position of the image in the window.

To change the size of the image...

Z n x y = zoom to n times the normal size and move percentage positions x and y to center of screen.  The default screen display is Z 1 50 50.  This means zoom to size 1 and place the horizontal point which is 50% of the way across the screen to the center point, etc. (which, of course, is redundant.)

Z 2 20 10 zooms to double size and takes the point 20% of the distance from the left to right and 10% of the distance from bottom to top and places it at the center of the screen.

Z alone is the same as Z 1.

ZF = zoom to fit screen size. The zoom factor will be set automatically so that the vertical size of the image will approximately fill the window.

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

### The left single quote (`) does the same as ZZ 1 -- no <enter> needed.

When NOT in edit mode, clicking with the middle mouse button at the screen edges will move the screen image in the appropriate direction. Also the four arrow keys and PageUp/PageDown keys will move the image. Clicking on the SCREEN menu box will give other options.

In EDIT MODE...

ZX (or ZOFF), ZON: AutoZoom -- If the image has been zoomed to a large size and an item is selected for editing, the screen will automatically shift so the source point of the item is visible. ZX (or ZOFF) will turn off this feature. ZON will turn it back on.

While in edit mode, the whole image may be shifted up and down with <ctrl>U and <ctrl>D, and left and right with <ctrl>L and <ctrl>R.

~
