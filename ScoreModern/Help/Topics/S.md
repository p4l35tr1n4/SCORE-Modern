# S commands

## In SCORE Modern

Leave Item Edit before saving. In Windows prompts, check the destination folder and filename before confirming a save.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

SA name = save data in file called "name." If the file already exists you will be asked if you want to write over it. If the file name uses the current extension it need not be typed. If an extension or dot is not typed, the current extension will be used.

SM = save data with the SaMe name as last used. This is the usual, quick way to save edited files, but no overwrite warning is given.  Always be careful when saving files with "SM".

SNX = save a file with the NeXt name alphabetically from the last file saved.

SC n = put up the spacing scale above staff number n. SCX = erase the spacing scale.

SH = show certain items with their actual type fonts. After this, a Z must be typed to recompute the screen data.

SHX = turns off the display of the actual type fonts.  (This is the default situation because there are many fewer vectors to compute.) After this, a Z must be typed to recompute the screen data.

SEP = Group edit command to assist in seperating parts from multi-part staves.

SP n = set staff n to be basis for spacing future input with the IN command.

STM n = adjust length of stems which extend above or below staff n. If n is larger than 32, all staves will be considered.

Note that beams are NOT moved by this command.  Move beams to their proper positions and then use the AD command to adjust the stem lengths under beams.

STUD = adjust spacing of notes with alternating STems Up and Down. This causes the STEM position to be the basis of spacing instead of the note head position.  Use this feature with care when rhythms must line up with other parts.

ST n c1 c2 c3 = edit staff n, find up to 3 code nums. (See also the E commands.) If n > 32, then all staves are searched for code numbers. If no code numbers are given, all items on staff may be edited.

Add 1000 to 'n' to edit all staves BEGINNING with staff n.
