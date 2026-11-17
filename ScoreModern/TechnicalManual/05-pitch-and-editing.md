# 05. Pitch, editing, and retained state

See [SCORE 4.01 input and editing](07-scor4.md#input-presets-rhythm-and-editing) for retained denominator and clef state, chord durations, and packed menu controls.

## Transposition and pitch reconstruction

TR and TRK select and transform records using pitch context. The opening pass collects clef, key, accidentals and relevant relationships. Preserve the order of that collection. The native dispatch uses the original interval, key and pitch helpers as separate stages. P4 is a diatonic staff position with packed controls, so adding a semitone count directly to P4 does not transpose the sounding pitch correctly.

The original interval menu is one-based. Its semitone order is `−8, −5, −4, −3, −2, 2, 3, 5, 7, 8, 9, 10, 14, 21, 12, −12`. For interval `n`, the key table index starts at `word(n+7)`. Subtract 12 up to twice while it is above 13, then add 12 if it is nonpositive. This is the original bounded operation rather than a general modulo. For `r=abs(n)%12`, the diatonic magnitude uses the indexed degree table `[0,1,1,2,2,3,3,4,5,5,6,6]` and adds a single 7 for magnitudes of at least 12. Apply the interval sign to the remainder and diatonic movement. The source guards inputs outside its verified table.

For the ordinary Main pitch path, compute the collected pitch plus its clef offset as a signed word. Reduce to a degree with `word((value+6)%7+1)` and add 7 if nonpositive. Use the destination degree in the 15-by-7 key table, indexed by key −7 through 7. If an explicit accidental exists, add its difference from the old key's accidental at the old degree. Explicit natural is a separate control. Values beyond double-flat/double-sharp adjust the diatonic pitch by a step and saturate the accidental. Encode the result in the accidental digit of the stem control. The cue marker is restored according to the resulting pitch sign after applying the diatonic offset.

Relationship and key insertion paths can move related staves or adjust associated records. Do not apply the pitch helper in isolation to each row and expect the same output as the command.

Sources: [interval tables](../../ScoreReverseEngineering/NativeFragments/InputTranspositionInterval.cs), [pitch table and accidental encoding](../../ScoreReverseEngineering/NativeFragments/InputTranspositionPitch.cs), [key pass](../../ScoreReverseEngineering/NativeFragments/InputTranspositionKey.cs), [main pass](../../ScoreReverseEngineering/NativeFragments/InputTranspositionMainPass.cs), [TR/TRK integration](../MainTransposition.cs).

## Tablature

Decode the head control into variant and number of strings. Octave variants 8, 15 and 9 add 7, 14 and −7 diatonic steps respectively before using the basic variant. Use clef, degree and accidental tables to compute the pitch class and octave. In the common recovered path, the absolute pitch is `trunc((diatonic+34)/7)×12+degreeOffset+accidentalOffset`. Select the first preferred string whose tuning does not exceed the pitch and respect the requested-string constraint. Fret position comes from the difference from the selected tuning on the applicable path.

The drawing and typed TAB routes have distinct rules for packed tuning pairs, replacement values, capo and bass displacement. The typed route's ordinary packed pair uses integer hundreds and remainder. The drawing route can replace absolute tuning values above 32. The DOS quarter-tone path has a context-dependent accidental table model and an optional rejection policy. Do not reuse one table for every variant.

Sources: [pitch and tuning tables](../../ScoreReverseEngineering/NativeFragments/LayoutTablaturePitch.cs), [accidental-table projection](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureAccidentalTable.cs), [relationships](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureRelationships.cs), [chords](../../ScoreReverseEngineering/NativeFragments/LayoutTablatureChord.cs).

## Editing coordinates, order and history

An affine RIP update computes `offset=f32(origin−origin×factor)` once, then stores each `f32(x×factor+offset)`. An ordinary relative horizontal shift stores `f32(x+delta)`. The staff shift applies only to positive coordinate references and takes the neighbouring staff field at its original offset. A range can include the second endpoint of a spanning record. Coordinate references are applied in their recovered traversal order.

The ordinary spacing sort exchanges values and their corresponding record references together. Strictly less X causes a swap, so equal positions retain reference order. Collection uses signed high words and unsigned low words of stored values for its horizontal range tests, which should not be generalized to floating-point sorting of negative or signed-zero inputs. Item ordering commands and helpers apply their own secondary keys.

The original mouse hit calculation can use signed 16-bit differences and Manhattan distance against the stored clickpoints. The strict projection uses the DOS coordinate grid and passes the active record index to the selector. The native bounded and high-resolution choices have different predicates. Drawn hover bounds and the original selection anchor are separate calculations.

The DOS viewport retains the current display raster, the draw state for an edited record, and cursor save/restore pixels. Its incremental operations can erase or replay parts of the vector bank. A full redraw reconstructs cache state and the first glyph of a given size can seed the offset table for subsequent glyph draws. Preserve the order of open, zoom, edit, preview, commit or abort when comparing DOS output. A score's serialized item list alone does not encode the whole display history.

Sources: [RIP updates](../../ScoreReverseEngineering/NativeFragments/LayoutRippleCoordinates.cs), [stable spacing sort](../../ScoreReverseEngineering/NativeFragments/LayoutSpacingSelection.cs), [item order](../MainItemOrder.cs), [mouse selection](../NativeMouseSelector.cs), [DOS raster cache](../NativeDosFrameCache.cs).

## Additional measured precision points

SCORLAS's P8 guide conversion multiplies the stored guide by 4000 for inches or binary32 1574.8031005859375 for metric before the truncating conversion. Do not substitute a rounded decimal 1575 or store the product as binary32 before converting. The tiling placement then works with the converted signed guide.

The original fixed single-real formatter reconstructs the stored binary32 value, limits the field to seven significant digits and rounds a discarded decimal digit of 5 or greater with carry. The formatter preserves sign from the stored bits, including negative zero. Field padding is a separate operation. This is a different boundary from the ties-to-even integer rounding used in the DSC rectangle and from the truncation used to write point coordinates.

Sources: [guide converter](../../ScoreReverseEngineering/NativeFragments/ScorlasGuideConversion.cs), [decimal formatter](../../ScoreReverseEngineering/NativeFragments/SingleFixedDecimal.cs), [formatter word writer](../../ScoreReverseEngineering/NativeFragments/FormatterWordWriter.cs).
