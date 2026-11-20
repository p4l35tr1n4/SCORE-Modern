# 07. SCORE 4.01 and its DOS utilities

The supplied `SCOR4.EXE` identifies itself as SCORE 4.01. References to SCORE 4.00 documentation describe published controls. Measurements in this chapter refer to the supplied 4.01 executable unless a utility is named. The recovered SCORE 3.11 chapters supply the baseline for shared record, drawing, and arithmetic conventions. The formulas below use their notation.

## Profile and resources

SCORE Modern Core retains SCORE 3.11 as the default. The SCOR4 profile changes menu dispatch, the startup title, `VER`, and the routing for recovered version-specific operations. It has an independent LIB folder and resource scope. Using that folder for all resources enables the supplied `S*.SET` presets and `PREF-4.SCR` when present. An unset SCOR4 LIB inherits the classic library. The Windows file and folder dialog choices and the F6 print-menu preference apply to both profiles.

The original `VER` shows the 4.01 header on a blank document and on the tested older file. A file saved in SCORE 4 adds creator and date information. Core includes its own build line. The two-row original interface uses F1–F10 and Alt+F1–F10. Controls such as F9 and F10 dispatch according to the active bank. The menu data and help give the labels and documented intent. Saved records and debugger captures establish what a button did in the tested context.

## Files and viewport

`DIRS` and `DIRSET` build the persisted directory list used by `DB` and `DX`. In the clean DOS fixture the two step commands required `#MUSDIR#.TMP`, stepped in alphabetical order across valid MUS, PAG, and X files, and wrapped at the ends. Each observed entry occupied 13 bytes including padding, followed by CRLF. The visible file picker's extension sort and reverse controls did not reorder that stored step list. Core reads the original format and writes it for DOS-compatible names. A list containing long Windows filenames is held in memory for the current session.

The two-row mode-17 frame uses a binary32 vertical multiplier of 4.65 at DS:CA0E. For a positive literal zoom `z` in the recovered range, the two drawing divisors are `trunc(50 / z)`. The origin calculations use the zoom and the requested percentage positions:

| Axis | Zero percentage after default substitution | Nonzero percentage |
| --- | --- | --- |
| X | `20` | `word(trunc(f32(6.4) × 50 − z × xPercent × f32(6.4)))` |
| Y | `word(trunc(f32(4.65) × 90 − 480 × z))` | `word(trunc((yPercent × f32(4.65) − 480) × z + f32(4.65) × 45))` |

Above zoom 1, a zero percentage is first replaced by `f32(50 / (z + .75))` on X or `f32(50 / (z + 1.5))` on Y. Pan commands add a truncated signed delta using `f32(6.4)` or `f32(4.65)` per percentage point. The horizontal and vertical percentages are retained independently. On the tested multistaff score, `Z 1`, `ZU 10`, `ZD 25`, and bare `ZD` produced Y origins −61, −107, 9, and 125.

**Top** uses the highest eligible buffered Y. In the measured two-row frame, zone bounds 97 and 446 give anchor `trunc((446 − 97) / 1.5 + 97)`. The routine adds the highest buffered Y divided by 50 and truncated, then subtracts `trunc(446 / 3)`. See `25C9:0340–03B1`. `ZF` uses the height reported by `H`. It converts a stored metric height to inches with binary32 2.54, then computes `f32(f32(4.1) / inches)`. At a ratio of 1 or greater the frame stays as it was. The tall fixture gave origin (20, 195) and divisors (107, 107) in either measurement system. DOS gave negative divisors on the empty fixture. Core leaves the current frame usable and reports that there are no items to fit.

The Items insertion selector at `25C2:00B2–0138` evaluates 32 staff baselines. It keeps the first minimum of `abs(originY − trunc(baseline / divisor) − 430)`. A blank score selected staff 1 at both zoom 1 and zoom 2. Initial, literal-zoom, pan, and fitted multistaff fixtures checked the selected P2 in the subsequent MUS save.

## Input presets, rhythm, and editing

The 26 setup presets use external `Snn.SET` files. All 26 matched the DOS notation records in key C at default geometry and with staff 2, left 10, vertical offset 5, size .8, and right 150. With altered geometry Core recalculates the saved widths of preset text. In the tested non-C presets a Code 17 record is created for each affected staff. Its horizontal position is `f32(roundedSetupBound − 2.5 × setupSize)`. Its P6 uses that staff's clef code. Set21 has no clef of its own and uses the last regular preset clef retained for each staff. The original retains that value across `RS` and an intervening file load.

The ordinary and dotted function-key rhythm choices update the retained denominator at DS:A2C6. Directly typed rhythm tokens, the tested Ctrl triplet and direct 1/12 shortcuts, and double-whole preserve it. **Quint** and **Triple** use the retained word. Measured examples include quarter → 5 with Quint and eighth → 12 with Triple. Applying the conversion from 64 to 96 and then to 120 displayed `**` at 120 in the two-character field. A semicolon submits the five input stages when entered at the end of the line.

In the checked chord edit cases, `CH` copies the selected stemmed note's P7. `CH duration` uses the supplied value. A stemless selection leaves the chord unchanged. For several stems at the same staff and P3, an ordinary upward stem (P5 tens digit 1) updates notes at or above its P4, and a downward stem (digit 2) updates notes at or below. Six complete two-stem saves cover reversed pitches, unisons, and same-direction stems. The original requires an acknowledgement before resuming item-edit input.

Other measured edit stores include rounding the adjusted meter size to three decimals before storing P7 and applying the corresponding P4 adjustment, P8=1094 for the text Music font, P8=1100 plus the font slot for outlined text, and the pedal endpoint flag P16 clearing when both endpoint offsets P17 and P18 are zero. The [batch ledger](../../SCOR4-UI-BATCH.md) lists the tested record changes by menu bank.

## Selected-beam stem adjustment

The selected-item `ADJ`, `ADF`, `ADX`, and `ADFX` controls use the SCOR4 attachment interpolation at `2591:0972–0A04`. In the measured ordinary branch, beam P3 and P6 are the horizontal endpoints and P4 and P5 give their vertical positions. Add the binary32 head offset `2.833329916000366` for upward attachments and zero for downward attachments, storing each adjusted X to binary32. Use the beam direction for its endpoints and the note's direction for its attachment.

```text
span   = f32(rightX − leftX)
offset = f32(noteX − leftX)
leftH  = f32(beam.P4 − note.P4)
rightH = f32(beam.P5 − note.P4)
negate leftH and rightH for a downward note stem
P8     = f32((rightH − leftH) × (offset / span) + leftH)
```

The ordinary branch excludes the opposite stem direction. The crossed branch adds `f32((beamCount − 1) × 1.5) + 1 − 14` to an opposite stem's extension and stores again. The French branch subtracts `f32((beamCount − 1) × 1.5)` and stores again when the attachment is more than binary32 .1 from both endpoints. The single-note route updates just the selected note. Automatic packing in the tested crossed case retained a descending inner run and stored reversal code 20. The selected-beam calculation is a separate routine from the main `AD` calculation. The selected-beam implementation guards extended cases such as explicit head offsets, cue pitches, and partial descriptors.

## Spacing, justification, and part assembly

- **LI and LJ.** Main `LI` applies the SCOR4 duration scale and places intervening text using its two already mapped rhythmic or structural anchors. `LJ` performs this placement before justification and centres eligible rests afterward. Reference saves cover text shifted to six positions and a whole-rest case. Certain original text terminator writes change the next item's count low byte to `0x20`. Core protects the count by default and offers the original behavior through its invalid-state option.
- **JUST4.** At `0B82:000E`, the recovered original weights positive durations using 4.0 from DS:142E. The instruction was traced to file offset `925D`. Core reproduced all 24 weighted groups, all 25 cumulative positions, and the saved results for the tested versions and durations. The SCOR4 workspace action uses the JUST4 calculation, and the classic profile uses the JUST 3.01 calculation.
- **PAGE4.** Positive duration is multiplied by 4.0 at `1D4D:04E4`. In the tested long staff, `1D4D:0124` compared P6 against 200.0 and `1D4D:0150` stored 200.0 before collecting rhythms. The coordinate allocator stores each cumulative position to binary32 at `1E86:0373`. Downstream justification at `1C39:050B` stores the sum of the interval and its adjustment before division and compares the stored ratio. On the orchestral fixture the right endpoint after justification was 200.0000457763672. The extracted 60-record part and all 61 transposed, stem-normalized records matched. A shared bar supplied the right boundary. The tested cue .39 units before its cue position survived, while the cue .41 units before it was excluded. Those samples bracket the tested boundary.

## Printing and companion utilities

The SCOR4 F6 settings bank ends with Read PRINT.SCR, Save PRINT.SCR, and F1 Help. Its typed DESC setting did not change the saved value in the test. SCORE 4 Help assigns DESC to SCORLAS4. A 300-dpi PRINT.SCR with .60/.80 offsets and Freeze matched Core byte for byte. The tested NUL-padded Laser destination resolves to the default printer in Core.

DRAW4 produced 388 vector rows from the `SCORE` glyph. Core's ASCII vector output matched byte for byte. SCORLAS4 1.01 matched the one-tile statistics and the two-tile 600-dpi job's declared EPS box `29 33 579 784`. The measured integer ink box was also equal. The two high-resolution vertical ink edges differed by about .018 and .054 points in that comparison. The common workspace names for PAGE, DRAW and SCORLAS are retained. The TMB utilities are external programs invoked by Core through a staged score and reload workflow, described in [TMB Help](../Help/Topics/TMB.md).

## Evidence and limits

| Area | Source or evidence |
| --- | --- |
| Profile, mode dispatch and file stepping | [Scor4Interface.cs](../Scor4Interface.cs) |
| Zoom, pans and insertion | [NativeDosViewport.cs](../NativeDosViewport.cs), [Scor4ScreenDrag.cs](../Scor4ScreenDrag.cs), [Scor4ItemsMenu.cs](../Scor4ItemsMenu.cs) |
| Input and chord edits | [Scor4InputPresets.cs](../Scor4InputPresets.cs), [Scor4InputMenus.cs](../Scor4InputMenus.cs), [Scor4NoteEditMenu.cs](../Scor4NoteEditMenu.cs) |
| Beam interpolation | [BeamStemAdjustment.cs](../../ScoreReverseEngineering/NativeFragments/BeamStemAdjustment.cs) |
| Fixture details and original comparisons | [SCOR4 batch ledger](../../SCOR4-UI-BATCH.md) |

Record comparisons count notation records and use the configured handling of original count-byte writes. The six trailer words carry metadata and are outside those comparisons. The physical reference click selected item 6 at zoom 2. Right-click, middle-click, and drag comparisons used controlled guest mouse states and matching Windows routes. Physical delivery for those other gestures, exact accordion DAT pixels, untested utility modes, and guarded parameter combinations remain outside the verified results. MIDI playback was outside the SCOR4 work.


## Shared DLL SCOR4 engraving and spacing controls (20 November 2026)

Both Core and SMX use **ScoreModern.dll 0.3.61.1** for these original version-dependent semantics. The active SCORE profile selects the interpretation; loading a MUS file does not silently switch profiles. The DLL has no global profile state: callers supply the version explicitly. Five-point slurs and adjustable SMX taper remain EXE extensions.

### Named drawing displacement: Code 11 P11

Commercial SCOR4 adds P11 to the nominal horizontal position in SCORE units. The raw displacement is `f32(150 × P11)`, independent of staff scale. Apply it to the drawing origin **before** the DRW point transform and 16-bit coordinate conversion; the displaced origin is also the rotation pivot. SCORE 3.11 ignores P11. Native screen vectors, vector display, bounds measurement and EPS use the same preparation.

### Harp-pedal diagrams: Code 7, P7=1, P15

In SCORE 3.11, P15 displaces the bottom of the central vertical line while the other endpoint and the rest of the diagram remain at their original positions, producing a slant. In commercial SCOR4, `f32(150 × P15)` translates the entire diagram horizontally: both lines, end brackets and all seven pedal parallelograms. Do not apply both interpretations. P10 remains the parallelogram shear control.

### Time-signature outlines: Code 18 P16

SCOR4 uses the signed word obtained by truncating P16 as a per-meter printer line-width adjustment. With base width `b` pixels and resolution `r` dpi, the existing printer helper supplies `u = f32(4000/r + .01)` and the raw width is `f32((b + trunc(P16)) × u)`, clamped at zero when negative. Printer serialization uses the existing four-decimal SCORE width formatting. At 600 dpi and a four-pixel base, P16=-2 produces `13.3533 lw`, versus `26.7067 lw` for P16=0. Restore prior/default width after the meter. The contour coordinates do not change. The original comparison confirms the adjustment with SETSTROKE off and on, and for common-time as well as numeric meter symbols. DAT screen contours remain unchanged; WYSIWYG/vector and EPS show the print treatment. Substituted filled PostScript font faces retain their normal font rendering; width changes affect stroked numeral outlines, not font metrics.

P16 does not itself alter justification in the new rendering code. Original LJ’s separate raw-record spacing behavior is retained; Tom recommends applying thinning after native LJ or using TMB LJ.

### Explicit slur endpoints: Code 5 P16=1

In SCOR4 the mode overrides P8 completely, including a P8 that would otherwise select a tuplet or ending bracket. The item follows the slur/slide dispatch. Its left raw origin is `f32(leftOrigin + 150 × P17)` and its right raw endpoint is `f32(150 × P6 + 150 × P18)`. The displacement is in horizontal SCORE units, not staff steps. P17/P18 are not number offsets in this mode and P16 is not a label 1. P4/P5, curvature, flattening, trims and the original fixed taper retain their usual geometry roles. A zero/zero displacement still enables the mode and bypasses P8. The implementation operates on working parameters and preserves stored values during rendering.

SCORE 3.11 retains the original P8 interpretation and P16 number meaning. A file-open notice therefore describes this as a version-dependent control rather than asserting the file’s creator version. Exact original tests cover ordinary slurs and P8=1/3 bypasses; unusual reversed endpoints and invalid-buffer combinations remain subject to the existing guards and compatibility policies.

### Number justification: Code 10 P16=1

SCOR4 includes the record in the spacing selection. Let `s = f32(staffUnits × 1/25)` be the existing collector’s staff scale, and let `d=P15`. The normalized allowances are:

- `left = f32(f32(.2) − d/s)`
- `right = f32(4 + d/s)`

The division remains extended until the final binary32 assignments; do not round `d/s` to binary32 first. Existing coincidence grouping, maximum allowances, staff scaling, gap redistribution and coordinate remapping then apply. The tested routine does **not** multiply these allowances by P6 size or derive them from font outlines. Original tests with P6=1/2 and P5=3/13 confirm the same allowance; Tom’s “single-digit” description is narrower than the observed behavior. Main J, JUST4 and PAGE4 all select this branch. SCORE 3.11 selection remains unchanged.

### Integration contracts

`ScoreVersionControls` supplies the shared version controls. Set `SlurOriginContext.Scor4=true` for SCOR4 geometry. Set `LayoutJustificationWorkspace.Scor4NumberJustification=true` for SCOR4 number spacing; leave it false for SCORE 3.11. Explicit overloads of `LayoutSpacingSelection.Collect` and `LayoutItemSpacing.Apply` carry the corresponding flag. The old public signatures remain available and delegate to classic behavior, preserving binary callers. Assembly version remains `0.0.0.0`; identify the release by file/informational version and hash.

Build one canonical DLL, then pass `-CoreDllPath <path>` to each application’s Build.ps1. This ensures the two packages contain the exact same binary, including its module identity.

### Verification scope

Original-executable EPS and saved-score fixtures were collected in an isolated DOSBox-X copy; no installed score/library was modified and no physical printer job was sent. Tests compare named-drawing and harp vertices, slur outline vertices, and complete J output positions; exercise DAT/vector rendering, strict EPS, automatic bounds, Main J, JUST4 and PAGE4; and retain classic pixel and corpus regressions. See the accompanying release verification report for final counts and artifact hashes. This is verification of the documented/tested domains, not a claim of exhaustive equivalence for all parameter combinations.


### Unused Code4 auxiliary fields

Original SCOR4 EPS comparisons confirm P8 is unused for hairpins and P15 is unused for straight lines, including signed and fractional values. These retained fields must not block native rendering or automatic bounds measurement. Core/SMX preserve them in the score. This correction is included in Core 0.3.67 and SMX 0.5.81.
