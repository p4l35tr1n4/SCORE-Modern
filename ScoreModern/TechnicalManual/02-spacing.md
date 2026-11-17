# 02. Spacing and justification

See [SCORE 4.01 spacing and part assembly](07-scor4.md#spacing-justification-and-part-assembly) for the version-specific duration weighting and stores.

## Different commands solve different problems

| Command | Input model | Result |
| --- | --- | --- |
| F2 | Parsed pitch, rhythm, structural tokens and staff setup | Creates records and computes the first set of X positions. |
| SP | Timed anchors and durations on a reference staff | Maps a query position onto the reference spacing. |
| L / LI / LJ | Multiple streams of timed records and structural symbols | Aligns corresponding events and allocates horizontal space. |
| J | Existing X coordinates and original minimum widths | Redistributes intervals while preserving selected relations. |
| JT | Existing text level, stored width and text offset | Uses the J gap engine with text-specific minimums. |
| VJ | Signed vector contours and vertical constraints | Computes vertical offsets between occupied staves. |
| H / HS | Staff groups, physical height and staff sizes | Changes staff positions or reference heights. |
| RIP | A range and delta, plus record relationships | Moves coordinates belonging to the affected range or span. |
| PAGE / COM / DEC / SEP / SCORLAS | Systems, files, staves, page markers or extrema | Assembles, partitions, transposes, places, or tiles existing material. |

## F2 and reference spacing

The F2 transaction runs pitches, rhythms, marks, beams, slurs and completion in that order. Keep the map from input note number to item index. The working arrays can contain clefs, keys, meters, rests and bars alongside sounding notes. Chord records and structural markers affect the map. Beam and slur endpoints refer to the working note numbers. Spacing computed in the rhythm phase is separate from stem changes, attachment centering and the final independent body records.

In the normal rhythm allocator, decode the original token into duration and shape. For a shape index `k` and `d` dots the ordinary shape check compares the duration with `2^(−k) × (2−2^(−d))`. The allocator starts with duration as a weight. With the proportional branch enabled it raises the weight to the supplied exponent and stores the result to binary32. The recovered cue/grace branch using the special denominator `.0625` substitutes `.1` or zero with a reference start. Zero duration can carry the x87 indefinite spacing value. It is handled as an exceptional branch and should not be treated as a zero-width ordinary event. Chord input uses its own allocation and duration helpers.

For each weighted event, retain `total=f32(total+weight)`. With the allocator's usable width `A` and current position `x`, store the event at `x`, then update `x=f32(x + weight/total × A)`. Opening symbols and structural markers have separate placement rules. The exact allocations for chords, grace groups, meter prefixes, barlines and retained left spacings are in the routine appendix.

SP builds two arrays. The data anchors are P3 positions of the timed events plus an end position. The end is the closing bar P3 when available, and otherwise the literal 200. A separate query grid starts at the first timed anchor, ends at the same end, and advances by `duration × f32(span/total_duration)` with a store at each step. The query lookup uses the original `.001`-shortened upper boundaries. It interpolates between source anchors, with a structural or grace marker able to supply a nearer drawing anchor without consuming beats. An absent opening clef contributes a zero default.

Sources: [staff transaction](../../ScoreReverseEngineering/NativeFragments/InputStaffTransaction.cs), [rhythm records](../../ScoreReverseEngineering/NativeFragments/InputRhythmRecords.cs), [chord rhythm](../../ScoreReverseEngineering/NativeFragments/InputChordRhythmRecords.cs), [reference spacing](../../ScoreReverseEngineering/NativeFragments/InputReferenceSpacing.cs), [automatic beams](../../ScoreReverseEngineering/NativeFragments/InputAutomaticBeams.cs), [slurs from input](../../ScoreReverseEngineering/NativeFragments/InputSlurConstruction.cs).

## LI, L and LJ horizontal alignment

LI gathers staff streams, prepares durations and encoded structural entries, merges the streams in rhythmic order, compacts the result, allocates coordinates, and remaps the original record fields. Relational endpoints and marks can be collected and remapped separately. This is a rhythmic alignment operation. The output is not a simple horizontal scaling of every P3.

In the recovered allocator, nonnegative kinds contribute `pow(duration, exponent)` to the rhythmic total. Negative kinds contribute fixed structural widths. The final structural entry receives zero width.

| Structural condition | Fixed width |
| --- | ---: |
| Ordinary structural slot | 3.5 |
| −14 or −140 followed by −15 or −17 | 1.5 |
| −14 or −140 followed by −18 | 1 |
| −15 followed by a kind at least −16 | 12 |
| −15 followed by a kind below −16, except −18 | 8 |
| −15 followed by −18 | 6 |
| −18 followed by −14 | 4.6 |

Calculate `symbolScale=f32(staffUnit/25)`, `noteScale=f32((right−left−symbolTotal×symbolScale)/positiveTotal)`. Weight stores are binary32. The total of powers adds the higher precision power to the previous stored total before the next store. Each scaled weight is stored to binary32. The running coordinate is retained across the output loop and each emitted position is rounded to binary32. The final scale branch uses the sign and raw bits of the merged value. A zero rhythmic total reaches a guarded divide case.

Sources: [lineup system](../../ScoreReverseEngineering/NativeFragments/LayoutLineupSystem.cs), [rhythmic collection](../../ScoreReverseEngineering/NativeFragments/LayoutRhythmicCollection.cs), [duration groups](../../ScoreReverseEngineering/NativeFragments/LayoutLineupDurationGroups.cs), [stream union](../../ScoreReverseEngineering/NativeFragments/LayoutLineupStreamUnion.cs), [allocation](../../ScoreReverseEngineering/NativeFragments/LayoutLineupCoordinates.cs), [remap](../../ScoreReverseEngineering/NativeFragments/LayoutLineupRemap.cs).

## J minimum widths and redistribution

**1. Collect.** J obtains position references from the raw score storage. A signed entry denotes a one-based word address by magnitude. It does not identify an item by ordinal. The collector and per-staff selection use explicit range and staff arguments. The ordinary wrapper uses a range tolerance of `.001`, tables with 200 cells and a separate coordinate capacity.

**2. Compute per-staff minima.** Sort qualifying note, rest and structural positions in the recovered order. Spacing is computed by branches over parameters, not by drawn bounding boxes. Within one position, take the appropriate maxima of left and right contributions, with a separate meter override. The ordinary point-join tolerance comes from stored `.03`. Let `s=f32(staffScale×f32(.04))`. Final gap minima are `f32((right[i]+left[i+1])×s)`. The clef path can also rewrite P3 when the original space conditions are met.

### Examples of width rules

- Rest right width begins at **4.5**. A negative P5 uses 6, or 9.5 for P5=−4. For the negative branch, a nonzero raw P9 may select 1. P5 above 1 adds `.5(P5−1)`. P6 adds `1.75×P6`. If |P4|≥100, the multiplier becomes the stored .6. When available, nonzero P10 sets left to −P10 and adds P10 to right. P15 scales the multiplier when its raw bits are nonzero.
- Bar width begins at left 1, right 2.5. Let `u` be the caller's effective P8. A preceding clef or key subtracts 1 from left. Nonzero P7 subtracts from left and adds to right. The Main J styles are below. The standalone JUST executable has a different literal bar width branch.
- Key width starts left 2 if preceded by a note, else 0. For `n=abs(P5 % 100)` and `sKey=P7` or 1 for the zero-bit default, right is `base+(n−1)×sKey×2.8`. Base is 5.5 after a clef in Main J, otherwise 4.5.
- Notes select width contributions based on head, accidental, dots, stem and voice context, and adjacent bars or grace records. The scratch state and branch order are preserved in the [full note spacing routine](../../ScoreReverseEngineering/NativeFragments/LayoutNoteSpacingSpan.cs). Accidentals 106–109 can address the adjacent descriptors outside the ordinary accidental width entries. Strict uses recovered executable behavior. The documented alternative uses the corresponding 6–9 widths.
- AFTC, AFTB, AFTK and AFTM are absolute SCORE minimums after clef, bar, key and meter on the applicable path. The recovered helper divides by staff scaling before adjusting a normalized width. Its subtractions from the corresponding defaults are 1.8, 2.5, 2.8 and the stored 3.7. The compact clef branch bypasses the AFTC correction. LI has its own symbol weights.

| Bar style | Left before neighbour and P7 adjustments | Right |
| --- | --- | --- |
| 1 | `2.2u + P13` | 2.5 |
| 2 | `3.3u + P13` | 2.5 |
| 3 | `5.4u + P14 + P13` | 2.5 |
| 4 | `2u` | `6u + P13 + P14` |
| 5 | `5.4u + P13 + P14` | `6u + P13 + P14` |
| 6 | `4.3u + P14` | `7u + P13 + P14` |

**3. Merge staff tables.** Add a closing boundary when the gap to it exceeds `.1`. For two breakpoints within `.1`, choose the earlier coordinate and the larger minimum. When an intermediate point splits the other staff's remaining interval, allocate `f32((target−anchor)×remaining/(limit−anchor))`, bounded below by the new minimum. Subtract it from the remainder with a separate store. The right cutoff is `f32(right−.2)`. The first interval can be suppressed when the first two positions differ by at most `.1`.

**4. Prepare slack.** Sum minimums by repeated binary32 addition. If the higher-precision requested width is smaller than that sum, use `deficit=f32((sum−storedWidth)/gapCount)` and subtract it from every minimum, with individual stores. For interval `i`, compute `slack[i]=f32(x[i+1]−x[i]−minimum[i])`. Sum positive slack and the magnitude of negative slack separately, with a store at every addition. The original uses sign bits for this classification.

**5. Redistribute and remap.** When there is negative slack and some positive slack, compute `ratio=f32(negative/positive)`. Traverse intervals from the left. Set `delta=−slack[i]` and multiply negative deltas by the ratio, storing the result. A shrinking interval scales its internal references before shifting the references at or after the right boundary. An expanding interval shifts first and scales second. Each consumed reference is cleared. The affine ratio is `(span+delta)/span`, compared with the stored threshold .2 before using the rounded result. At or below the threshold the recovered caller substitutes 1. A running lower bound prevents certain backwards coordinate moves. All source references are applied in list order. The ordinary Main path then snaps P3 values within the strict tolerance of .01 from 0 or 200. These two comparisons use the original P3 and the pass can affect indexed items outside the selected system. Standalone JUST skips that Main snap.

### Worked redistribution

Take the gap core with positions 0, 7, 30 and minimum widths 10, 10. The requested width is 30 and the sum of the minimums is 20, so no minimum compression occurs. Slack is −3 and +13. The ratio is stored as `f32(3/13)`. Expanding the first gap by 3 moves the middle position to 10. The second gap gives up approximately 3, restoring the right extent to approximately 30 at the stored precision. This example starts after staff-table merging. An actual J command also collects and remaps the item and span references.

Sources: [system orchestration](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationSystem.cs), [spacing tables](../../ScoreReverseEngineering/NativeFragments/LayoutItemSpacing.cs), [symbol widths](../../ScoreReverseEngineering/NativeFragments/LayoutSymbolSpacing.cs), [rest widths](../../ScoreReverseEngineering/NativeFragments/LayoutRestSpacing.cs), [merge](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationMerge.cs), [slack](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationGaps.cs), [remap](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationCoordinates.cs), [snap](../../ScoreReverseEngineering/NativeFragments/LayoutJustificationSnap.cs).

## JT and VJ

JT selects Code 16 records on the requested staff and horizontal range. Level 99 permits level selection without the ordinary `1.9` proximity filter. Use the first baseline as a reference. Other accepted text positions must be within 1 of the baseline before they are aligned. Compute `gap=f32((staffUnit/25)×1.5)`. The text's stored P13 width and P11 offset contribute `f32(P13+gap+P11)`, and P11 also subtracts from the preceding minimum. Add the closing position when needed and run the common slack and coordinate remap. The JT caller uses .2 for its left and right boundary decisions. P13 comes from the text metrics path. Measuring a system font would change the inputs.

VJ uses a contour of the actual recovered vector stream on a 101-cell horizontal grid. Initialize minimums to +32760 and maximums to −32760. Reject marker-like runs and runs wholly outside the relevant horizontal span as in the contour routine. Convert raw X into bins using signed division of `word(X+bias)` by the bin width. Vertical segments contribute both extrema to one bin. Horizontal runs contribute across bins. Sloping runs compute a stored binary32 slope, then truncate each reconstructed Y at the corresponding bin step.

For upper contours against lower contours, scan bin pairs within the chosen radius and minimize `upperMinimum−lowerMaximum`. Skip empty sentinels. For adjacent occupied staves let `s=7×min(scale1,scale2)` at its store, compute `required=trunc(verticalSteps×s)`, and compute the minimum floor from `staffGap×s−(baseDifference−8s)` narrowed to a word. Apply the larger of `required−distance` and floor to the cumulative shift. Normalize the cumulative value by the target staff's own `7×scale`. An absolute result below the stored .2 becomes zero. The recovered path requests a repeat when a baseline reaches 32100, resetting that cumulative calculation to `32000−base`. The wrapper determines how each offset is written back to the relevant staff controls.

Sources: [JT](../../ScoreReverseEngineering/NativeFragments/LayoutTextJustification.cs), [VJ contours](../../ScoreReverseEngineering/NativeFragments/LayoutVerticalContour.cs), [distance](../../ScoreReverseEngineering/NativeFragments/LayoutVerticalCollision.cs), [vertical placement](../../ScoreReverseEngineering/NativeFragments/LayoutVerticalPlacement.cs), [VJ integration](../MainVerticalJustification.cs).

## Height, movement, assembly and tiling

H distributes the requested physical height difference over the number of adjustable group gaps. Its ordinary path divides that physical increment by the stored unit for inches or centimetres, approximately .04375 or .1111252, respectively. It advances a cumulative step for each group and divides it by individual staff size before adding it to staff P4. P10-based placement takes its separate physical increment and, on the recovered branch, stores the intermediate with 10000 added before subtracting that marker. The extra store can alter the result at a boundary.

RIP, group movement and affine layouts collect explicit coordinate references. A multi-endpoint item may contribute more than P3. Staff numbers, protected ranges, and shared markers affect collection and write order. COM/DEC/SEP and PAGE change record groups and mappings through separate source routines. PAGE's assembly includes distinct collection, system selection, line layout, measure numbering, note spacing, curve stitching and part output paths. Its supported layout branches are linked individually in the routine index. They do not share one universal spacing formula.

SCORLAS uses scan extrema and a retained guide. With signed minimum `m`, maximum `M`, set `h=M−m`, `b=word(m+24000)`, `n=h+b`. The first file records first-bottom shift `b0=b` and starts cumulative position at zero. For following files, `delta=previousAdvance−(previousGuide!=0 ? 0 : b)`, add delta to the cumulative position, and compute total `cumulative+n−b0`. Each file sets its next advance to its nonzero guide or `n+1000`. The final DSC bounds in the recovered branch use the current minimum and that minimum plus the total. Ink extents are a separate measurement. Empty-file sentinels can produce reversed bounds in original mode.

Sources: [height](../../ScoreReverseEngineering/NativeFragments/StaffHeightLayout.cs), [ripple](../../ScoreReverseEngineering/NativeFragments/LayoutRipple.cs), [affine coordinates](../../ScoreReverseEngineering/NativeFragments/LayoutCoordinateAffine.cs), [PAGE layout](../NativePageScoreLayout.cs), [PAGE note spacing](../NativePageNoteSpacing.cs), [SCORLAS file placement](../../ScoreReverseEngineering/NativeFragments/ScorlasFilePlacement.cs).
