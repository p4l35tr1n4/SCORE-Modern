# SCOR4 interface preview

## Profiles and resource folders

Use **Preferences → Windows → SCORE profiles** to choose the interface, set each profile's LIB folder, and adjust the height of the function menu with **Compact bottom menu**.

| Setting | SCORE 3.11 | SCOR4 |
| --- | --- | --- |
| Interface | Classic F1–F8 menu | Two rows, F1–F10 and Alt+F1–F10 |
| Empty LIB field | Startup library or the mapped preference path | Inherits the classic library |
| LIB with All resources | Drawing banks, DAT glyphs, font metrics and library defaults | The same resource categories, plus the SCOR4 presets and `PREF-4.SCR` when supplied |
| Windows display and dialog choices | Shared | Shared |

The SCORE preference table shows the active profile's values and the file they come from. Each explicit LIB folder and its resource scope are stored independently.
When the startup splash is enabled, the selected profile determines which SCORE title appears alongside the Score Modern logo and build number.

## Menus, files and viewport

The two rows use F1–F10 and Alt+F1–F10. On the main row, F2 opens the File menu, F3 (`DB`) and F4 (`DX`) step through the current working directory, F5 starts music input, F6 opens printing, and F9 edits the last item. Alt+F1 fits the score in the graphics area.

In the File menu, F1 (`DIRS`) refreshes the directory and F2 (`OP`) opens the numbered file list when using SCORE file prompts. Select a file and choose **Open**, double-click it, or type its filename or number. In the list, F1 and F2 page down and up, F5 sorts by extension, and F7 reverses the order. F3 saves under the current name. F4 asks for a filename. F5 (`SNB`) saves with the previous name in the filename sequence. F10 opens the file information bank, where **Date** uses the `DATE` or `VER` report. F9 or Escape returns one bank.

Run **MkDir**, `DIRS`, or `DIRSET` to build the list for `DB` and `DX`. The list wraps at its first and last entries. Refresh it after adding or removing files. Core reads the original `#MUSDIR#.TMP` and writes that format when the filenames fit DOS 8.3. Lists containing long Windows filenames are available for the current session.

SCOR4 uses the existing Windows file and folder dialog preferences, and the existing F6 print-menu preference. `VER` uses the SCORE 4.01 format in this profile and includes the Score Modern build line.

The SCORE-format F6 menu in SCOR4 ends with **Read PRINT.SCR**, **Save PRINT.SCR**, and **F1** for Help. The SCORE 3.11 menu also has **16. DESC**. In the original SCORE 4 help, DESC is a SCORLAS4 control.

Alt+F2 opens the Screen menu. F1 resets to the normal bottom view and F2 brings the top of the score into view. F3–F6 pan, F7 zooms out and F8 zooms in. Alt+F1 enters **Drag Screen**. Hold the left button to drag and press the right button to finish. Alt+F6 runs `ZF`, which reports the page height and scales tall scores to fit. A score 4.1 inches tall or shorter keeps its current zoom and position. Alt+F9 returns to zoom 1.

With `MOUSE 0`, the middle button pans. With `MOUSE 1`, it selects an item, and all three buttons together pan. The supplied SCOR4 executable uses middle-button selection with no MOUSE directive. You can set the mode in the active SCORE preference table.

Main **Type** lists a text file. Alt+F10 opens **User**. Choose a numbered file such as `#01` to run it as a macro from the working folder. In that bank, F10 asks which file to list. The semicolon entries recall or list the three mini-macros. Use `RE` for normal command-file reading or `RES` to suppress the input-staff correction prompt.

The item edit row also offers **User**. A numbered macro can edit the selected item's parameters. In the verified forward-edit case, the macro finishes by committing the edit and advancing to the next item.

When a load would discard an unsaved edit, choose F1 to keep working or F8 to load. The dirty-score Exit prompt uses the same two keys.

## Input and editing

F5 opens SCOR4 input setup. The presets use the SCORE 4 LIB's `S*.SET` files. F10 opens the second setup bank. Ctrl+End or Alt+F8 undoes the most recent preset. All 26 preset buttons in C were checked with default geometry and with a changed starting staff, horizontal limits, vertical offset and staff size. Tested keys include Treble and Alto in F, Bass in D, Piano in G, and Piano in D with a staff size of .8.

In rhythm entry, Ctrl+F2 through Ctrl+F8 and right-clicking the corresponding duration boxes enter SCOR4 triplet rhythms. F10 opens the rhythm repeat bank. Choose a repeat multiplier to attach it to the current rhythm and return to the first bank. Notes and clefs have additional edit banks. In those banks, F9 returns to the first bank. From the first edit bank, F9 ends editing. Grey boxes identify actions unavailable in this preview. Alt+F4 closes the window from the Main bank and has a contextual action in other banks.

After choosing an ordinary duration with a function key, use Alt+F1 for **Quint** or Alt+F2 for **Triple**. For example, a quarter-note denominator of 4 becomes 5 with **Quint**, and an eighth-note denominator of 8 becomes 12 with **Triple**. The original's **Quint** leaves the displayed whole and half denominators at 1 and 2. The conversion uses the retained denominator from the most recent duration control that set it. Typing a rhythm directly leaves that denominator alone.

In item edit mode, `LL` and `RR` move horizontally by half a SCORE unit. `UU` and `DD` move vertically by half a scale step. For an attached note mark, use **Marks → LRUD**, or type `MKL`, `MKR`, `MKU`, or `MKD`.

On a note, F10 opens the second edit bank. Use **Stem−** and **Stem+** to adjust stem length by half a scale step, **CueSz** and **NrmSz** to switch size, and the notehead buttons to cycle the basic head shapes. **Staff−**, **Staff+** and **Staff0** change P12, the note's cross-staff display control.

Use **ADJ** or **ADF** to fit the selected note's stem to its beam. The crossed controls also allow an opposite stem direction. With a beam selected, the corresponding controls apply across the beam. **CHdRhy** uses the selected stemmed note's duration for the chord. Type a duration after `CH` to apply it explicitly.

Choose **Accis** on the lower note-edit row for single and double accidentals, parentheses, spacing and enharmonic spelling. **+Space** and **−Space** change P5 by .02, which moves an accidental by .2 staff steps at size 1. **NoSpc** clears the fractional spacing value. **AutoSpc** sets it to .26 for a sharp and .22 for the other standard accidentals. **NoAcc** clears the accidental and its parentheses while retaining the stored spacing.

In **Marks**, use F10 to reach fingerings, harmonic, ficta, trills and multiple dots. **LRUD** moves the mark by half a step. **FLM** changes the side of the mark. F9 returns to the mark bank you came from.

On a rest, use the lower edit row to move it, add a fermata, hide it, or adjust its horizontal offset in half steps. Choose **Rhyth** for durations and rhythmic subdivisions. In that bank, F10 brings up the numbered rest forms. These controls use the recovered rest-centering routine to position the forms between barlines.

For a library symbol (Code 9), use F10 to step through rotation, dynamics, expression terms and ornaments. F9 returns to the preceding bank. The ordinary fermata sets the horizontal size to .25. The square and pointed fermatas retain the current size.

F10 on a harp or piano-pedal item opens its size and offset controls. On a box or circle, F4 toggles fill. F10 opens offset and rotation for a box, or arc endpoints, polygon sides and oval sizing for a circle. F10 again from the circle controls opens offset and rotation.

For a meter, the second edit bank has size, numerator and denominator controls. **−Size** and **+Size** change P7 by .2, round the result to three decimal places and adjust P4 by .62 in the opposite direction. **Size 1** restores the default size and clears that vertical adjustment.

Choose **Font** in the second meter bank or in the tablature bank to select a font slot. Meter font choices use P13. Tablature font choices use P12. Each font bank's **Music** button uses the original value for its item type.

On a key signature, **CntMs** moves the key to a nearby barline and sets P15 to centre the accidentals. Both the staff size and the key's P7 spacing factor affect the offset.

For Code 11 drawings and Code 15 EPS inserts, the lower row adjusts size and rotation. F10 opens the file bank and **NewNm** asks for the replacement filename.

For a manual guitar grid, choose **Dots** or **FingNum**, select a string, then select the marker or fingering from the lower row. The original stores fret markers in P13 and fingerings in P18. The **Clear** and **P18=0** buttons clear their respective fields.

F2 reverses the edit direction. Right-click the left half of **Zm/Mv** to move to the preceding staff number, or the right half to move to the next staff number. In the slur edit bank, **MovLf** and **MovRt** select one endpoint for the arrow keys.

For a slur or tie, press F10 once for the second bank. The upper row chooses a half, changes direction, restores the normal shape, or switches to a dashed, dotted or ending form. The lower row adjusts P8 or changes the endpoint offsets by half a step. Resetting either endpoint preserves an active offset at the other end.

While editing a slur or tie, press F10 twice to reach the third bank. `CNL` or F3 centres the left end just before the first note on the staff and the right end on the nearest note. `CNR` or F5 centres the right end just before the last barline and the left end on the nearest note. Use F10 in that bank to lock the midpoint, then use the arrow keys to adjust its horizontal position and curvature. Press Escape or F10 again to clear the lock.

Alt+F9 from the Main bank opens **Items**. Choose an item to add it and enter item edit mode. Press F10 to see the second bank and F9 to go back. The Staff button places a staff at the first unused staff number. Item placement has been checked after loading a score and after numeric `Z 1` and `Z 2` commands.

The **Code11** and **Code15** entries in the second Items bank ask for a DRW library name or EPS filename before adding the item. Enter `-` to cancel. SCOR4 looks up relative EPS names from the working folder.

In the supplied SCORE 4.01 executable, bare `MET` creates a meter item. Use `METR` or `METRIC` for metric measurements, and `INCHES` to return to inches.

Choose **CntrDsh** in the Group bank or type `CD` to recalculate lyric dashes. For one staff, type `CD` followed by the staff number. The SCORE 4 preference file controls the dash length and count through `DASH` and `XDASH`.

Choose **CnWhl** in the Group bank to centre eligible whole and numbered rests between barlines across the staff bank. SCOR4 **LI** places text between its surrounding musical anchors. **LJ** applies lineup and justification, then centres eligible rests.

## Other capabilities documented for SCORE 4.01

The supplied release notes describe the following controls. They identify DOS features and parameter meanings. See individual Code topics for Core's covered cases.

| Area | SCORE 4.01 documentation |
| --- | --- |
| Lines | Code 4 `P7=-5` creates a vertical line. `P6=0` makes it participate in justification. The bracket and arrow controls are P11 and P12. |
| Slurs | Code 5 P19 changes where the first half of the slur reaches full thickness. Zero uses .85 of the half-curve. |
| Symbol library | Code 9 adds combination accidentals at P5 265–268, brass fingering diagrams at 750–757 and stem-down slash notes at 990–996. |
| Fingering numbers | Code 10 `P16=1` includes single-digit numbers in justification. P15 can offset them. |
| Meter numbers | Code 18 P13 can hold 1000 plus a font slot, for example 1004 for Helvetica. |
| Beam editing | The last beam-edit bank has an action to flip unattached inner beams. |
| Layout utilities | In SCOR4, **JUST4…** uses SCORE 4's positive-duration scaling before lineup. **PAGE…** opens the PAGE4 dialog and applies the matching lineup, justification and beam-stem calculations during part assembly. The tested single-staff parts include ordinary and stem-normalized transposition. DRAW and SCORLAS use the shared dialogs. |

The SCORE 4 distribution also includes dedicated help for the following commands and items. These entries describe the supplied help, with verification status where the behavior matters to Core.

| Topic | SCORE 4 help |
| --- | --- |
| B and K | `BAR`, `BAS`, `BEAM` and `BOX` are direct item shortcuts. `KSIG+5A 2 15` specifies five sharps in alto clef format at horizontal position 15 on staff 2. The B topic also explains the Group selection rectangle. |
| X | `X` leaves item edit. `XX` copies the edited item to the cut buffer before leaving edit. Use `PS` to paste it. Core's tested `XX` and `PS` cases match the original saved notation records. |
| Code 13 | PAGE can create invisible placeholders for cross-system slurs and ties. The topic also describes the separate `LJ.EXE` spacing convention with P5=99. |
| Code 15 | P7–P10 hold the bounding box of an EPS, P11=-9999 marks an EPS, and P14–P19 store the filename. The SCOR4 Items menu has a dedicated filename prompt. |
| Code 19 | An invisible spacing item used by `LJ.EXE`, anchored to an eligible note, rest, clef, barline, key signature or meter at the same horizontal position. The help describes creating it by transforming an existing item with EDITSCOR. |
| Q | The original help leaves this topic for user-written pages. Its fixed 25-line page format applies to the DOS help reader. |

Sources used for the profile comparison include the supplied SCORE 4.00 and 4.01 release notes, `MENU4.HLP`, `PREF-4.HLP`, and the corresponding SCORE 3.11 and Tom’s Help topics.
