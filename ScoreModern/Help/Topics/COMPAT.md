# SCORE Modern · Strict SCORE behaviour

Open **Preferences → Windows → SCORE compatibility**. The **Strict SCORE behaviour** checkbox applies the available original operation rules as a group. Your individual preferences are stored while it is on and become effective again when it is off. High-resolution Windows notation and the Type 1 rendering options remain available.

## Keyboard and mouse

### Strict SCORE behaviour

Applies the original settings described here. F1 and question-mark help use the bundled Help folder. Open **Explain compatibility settings** to return to this page from Preferences.

### Item PgUp / PgDn step

Moves the selected item vertically in Item Edit. Strict uses four staff scale steps. The alternatives are five or ten.

### Item Tab step

Moves through horizontal positions in Item Edit. Strict uses ten horizontal SCORE units. The alternative uses five.

### F3 prompt Escape cancels instead of recalling the last item

Cancels the item-selection prompt when enabled. Strict retains the original recall of the last item.

### Allow Alt+S overview during item editing

Makes the Windows overview shortcut available in Item Edit. Strict bypasses it there.

### Allow Alt+F full graphics in text setup

Gives the score more room during text setup. Strict leaves the graphics bounds as they were.

### Allow Alt+F full graphics in Group menu

Allows full graphics from the Group menu. Strict retains the original Group header and ignores Alt+F in that menu.

### Home clears text instead of showing notation top

Clears the command transcript while keeping the pinned file line. Strict uses Home to show the highest buffered notation point.

### Main Up/Down uses documented 5% instead of original 10%

Sets the Main-mode vertical pan increment. Strict uses 10%.

### Ctrl+R also recalls the last line

Adds Ctrl+R as a command recall key. Strict retains Ctrl+Q as the original recall route.

### Three buttons exit Edit Menu as documented

Enables Tom’s documented three-button exit. In the supplied original executable, three buttons map to F8 and do nothing there. Left+middle or right+middle maps to Escape.

### Selection coordinates

Chooses the original quantized DOS coordinates or higher-resolution distances. Strict uses the DOS grid.

### Original unrestricted clicks

Makes selection consider the nearest eligible item even when a click is outside the normal target area. Strict enables unrestricted selection.

### Larger item numbers

Enlarges the number shown by a hover marker. Strict uses the standard size.

## Notation and editing

### Beam and slur geometry

Chooses printed/EPS geometry or DOS-screen geometry for the screen appearance. The setting is separate from the original-operation preset and from the choice of vector or DAT display.

### P8=10 follows Tom’s Help

For the integer Code 5 control P8=10, selects the ending interpretation. Strict uses the measured slur route. See [slurs and endings](CODE05.md).

### Reject invalid trim ranges

Rejects reversed or out-of-range slur and hairpin trims. Strict uses the recovered original buffer effects for the measured cases.

### Measured DOS quarter-tone spacing

Uses the original measured width-table reads for J with accidental controls 106–109. These values can depend on the DOS load address. The alternative uses the documented widths for controls 6–9. Strict selects the measured reads.

### Reproduce original invalid states

Permits measured legacy edge cases such as entering item 0, specific writes to adjacent text records, and known retained parameter effects. Use I with a valid item number to leave the measured item-0 state. Strict enables the measured cases.

### Grace-chord flips: original input-stage behavior

Uses the original retained F2 stage state in the grace-chord stem calculation. The alternative uses the deterministic calculation. Strict uses the original state.

### Allow more than 150 F2 note positions

Enables larger Windows input buffers for F2. Strict retains the original limit rather than extending the input buffer.

### Reject multi-digit F marks

Requires a single mark digit followed by separated targets. Strict parses packed legacy input. For example, F20 10 means F2 applied to targets 0 and 10.

### EDI: retain documented multiple-dot/trill marks

Keeps documented EDI tokens S2, S3, S4 and TR for marks 26–29. Strict uses the measured executable’s conversion.

### Allow approximate engraving

Allows SCORE Modern approximations where the recovered renderer cannot draw an item. Strict disables approximation.

## Commands and external resources

### DPY: allow labelled native logical arrays

Allows reconstructed arrays for current items when the original DOS storage history cannot be reproduced. Strict requires a verified original storage path.

### Z / ZZ defaults

Chooses the executable defaults, with bare Z retaining zoom and bare ZZ selecting 2, or Tom’s defaults, with Z selecting 1 and ZZ selecting 3. Strict follows the executable.

### Zoom framing

Chooses original DOS framing or a full scrollable page with SCORE zoom and pan amounts. Strict uses original framing. The notation view remains a separate choice.

### TAB quarter-tone conversion

Choose the recovered DOS accidental-table interpretation or reject quarter-tone conversion. Strict uses the recovered table.

### TAB: reject adjacent accidental-table reads

Rejects conversion when it would inspect an adjacent part of the legacy accidental table. Strict uses the recovered DOS table on verified paths.

### TAB: DOS load segment

Supplies a hexadecimal segment for address-dependent TAB results. 0824 matches the reference DOSBox-X environment. This setting models the DOS calculation using the supplied segment.

### Drawing filenames

The original mode applies SCORE 3.11’s short filename interpretation for Code 11. The documented mode accepts full .DRW names. Strict uses the original interpretation.

### PostScript files: also search beside MUS

Extends lookup of included PS/EPS to the open score’s folder. Strict uses the SCORE resource lookup.

### Map DOS PostScript paths

Allows DOS paths to resolve against the selected SCORE installation. Strict enables mapping.

### Cache-only text refresh marks document modified

Controls the unsaved-changes flag when only cached text widths are refreshed. Strict leaves the flag clear for a cache-only refresh. New widths can still be included on the next save.

### NX / NB filename progression

Standard mode uses Tom’s stem convention for alphabetic names and the recovered numeric suffix rules. Strict uses the recovered executable progression for alphabetic names as well.

## Printing, files, and other Windows options

### Retain incomplete EPS when exited

Keeps the unfinished stream when a print job is exited with X. Strict retains the original side effect. Turning this off protects an existing EPS from an incomplete replacement.

### Reset Go suppression after this job

Treats Go suppression as applying to one job. Strict keeps DESC=1 after G, as measured in the executable.

### Reproduce SCORLAS empty-file bounds

Uses the verified empty-file sentinels in tiling, which can produce a reversed EPS bounding box. Strict enables those verified cases.

### F6 menu

Chooses the SCORE-format menu or the Windows PNG/EPS function buttons. Strict opens the SCORE-format menu.

### File and folder dialogs

Windows dialogs can handle load/save and F7 folder choices. Strict uses the SCORE prompts.

### Show command lists in copyable dialogs

Opens supported listings in a copyable Windows window. Strict uses the original command output route.

### Delete key acts as F7 Delete

Lets Delete trigger deletion where F7 is labelled DEL. Strict uses F7.

### Group held MIDI pitches into chords

Can combine held controller pitches during F2 input. Strict follows the available SCORE input behaviour.

### Windows screen colours

Individual RGB overrides can be used in the normal Windows configuration. Strict takes the SCORE palette from COLO.

The original rules are documented in [Tom’s command and item index](HELP.md). See [view and navigation](VIEW.md) for options that affect the Windows display.
