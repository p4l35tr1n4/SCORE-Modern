# K commands · SCOR4

## SCOR4 reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

*Original fixed-width diagrams and examples retain their source spacing.*

KSIG = key signature shortcut command. See also help for Code 17.

KSIG alone will produce a 1 sharp key signature, in treble clef format, on staff 1 at position 100 (center of screen).

KSIG may be followed by any of the proper Code 17 parameters. For example: KSIG 3 12 places the key signature on staff 3 at position 12.

```text
Extended commands: KSIGvnw -- v should be + or - for sharps or flats.
                              n is the number of sharps or flats.
                              w indicates the clef format:
                                   blank = treble clef
                                   B     = bass clef
                                   A     = alto clef
                                   T     = tenor clef
```

Example: KSIG+5A 2 15 will put a 5 sharp key signature in alto clef format on staff 2 at position 15. (Be sure there are no spaces in the first seven characters.)

~
