# 01. Storage, arithmetic and coordinates

See [SCORE 4.01 viewport and file behavior](07-scor4.md#files-and-viewport) for the two-row graphics frame and directory-list format.

## File and working records

An ordinary MUS file accepted by the recovered loader starts with an unsigned 16-bit little-endian count of **four-byte cells after the count itself**. Its byte length is therefore `2 + 4 × count`. Each item starts with a binary32 parameter count followed by that many four-byte cells. P1 is the code and P2 the staff. P3 normally locates the item horizontally. P4 is code-specific. There is no global parameter schema for P5 and above. Consult the [code topics](../Help/Topics/HELP.md) and the code-specific consumer before interpreting a field.

The last cell is binary32 `−9999`. The loader accepts a five- or six-cell trailer. In the six-cell form, in order, the fields are retained/undefined, raw producer ID, version, measurement, preceding-cell count, terminator. Measurement 0 selects inches and 1 selects metric. The observed SCORE 3.11 save stamps version `3.0` and preceding-cell count `5.0` and takes the producer word from the selected installation data. For a five-cell input the producer cell is inserted after the first field. Keeping a raw byte copy of the item and trailer is significant. Code 16, for example, stores its byte length in P12 and begins CP437 text at offset 56 from its record header. Text contents occupy the backing parameter cells and are not floats to be normalized. JUST 3.01's later height-batch files can wrap the complete payload in a Fortran frame. The larger WinScore word-count extension has no verified fixture here.

The normal accessor supplies zero for absent parameters. Some original routines instead read the backing record area. Those paths can see a neighbouring record or retained storage. For example, the scan copies up to 19 words into working storage and can retain the twentieth from the caller. Code 16 can force the effective copy length to 12 based on the raw P12 comparison. Code 8 writes working P12 as 1 and has a staff-1 P8 conversion branch. DPY's exact storage view and a regenerated logical item list represent different data when retained words matter.

Sources: [Item and Score](../ScoreModern.cs), [original save](../OriginalMusSave.cs), [parameter preparation](../../ScoreReverseEngineering/NativeFragments/ScanParameterPreparation.cs), [retained storage](../../ScoreReverseEngineering/NativeFragments/RetainedParameterStorage.cs), [DPY routes](../NativeRawArrayDiagnostics.cs).

## Numeric conversion and precision

Keep five operations separate when reproducing a calculation.

| Operation | Behaviour | Consequence |
| --- | --- | --- |
| Binary32 load and store | The stored value has the bit pattern of a four-byte float. Several operations can be evaluated before the next store. | Rounding a product before an addition can shift the eventual integer point. |
| Presence and equality | Some tests compare raw float bits. Other tests compare numeric values or a signed high word. | `−0.0` and `+0.0` can reach different branches. The J rest multiplier is one measured example. |
| Float-to-integer helper | Saves the x87 control word, sets rounding-control bits to truncate, stores a signed 64-bit integer and pops the stack, restores the control word, and exposes the low 32 bits in DX:AX. | Truncation acts on the value at the call, which may still be an extended intermediate. |
| Word operations | Signed division truncates toward zero and returns a signed remainder. Narrowing after integer arithmetic can wrap. | A coordinate of −51 divided by 50 yields −1, not −2. Zero divisors and out-of-range signed quotients are distinct fault branches. |
| Clamp and format | Buffer writers compare signed word pairs and sometimes compare their low part unsigned. Print formatting uses its own decimal conversion. | The drawing clamp, raw cached origin, EPS decimal output and rectangle rounding need separate reference values. |

The measured coordinate clamp projects first value to `[−32600, 32760]` and caps second value at `32750`, with no corresponding lower clamp in that block. It compares the first upper equality through the write path. Values then narrow to words. A cached printed origin can use the low signed word without that clamp. The P8 scan multiply uses two binary32 operands held through multiplication and then truncated before storing the low DWORD. Do not replace that with a multiply followed by a binary32 store. The recovered C# uses binary64 intermediates for many finite projections and explicit casts at observed stores. A match on finite outputs does not establish behavior for all extended precision or exceptional inputs.

For a signed drawing control `w`, the recovered decimal decoder uses `hi = trunc(w/10000)`, `mid = trunc((w−10000hi)/100)`, `lo = w−100trunc(w/100)`. A particular code may take the magnitude first, treat the sign as direction, or take a float remainder before converting. The decoder therefore describes that helper only.

Sources: [coordinate converter](../../ScoreReverseEngineering/NativeFragments/CoordinateConversion.cs), [P8 conversion](../../ScoreReverseEngineering/NativeFragments/ScanP8Conversion.cs), [clamp](../../ScoreReverseEngineering/NativeFragments/CoordinateWordClamp.cs), [signed division](../../ScoreReverseEngineering/NativeFragments/SignedDivide16.cs), [packed decimal word](../../ScoreReverseEngineering/NativeFragments/PackedDrawingWord.cs).

## Staff tables and geometry

The measured staff table covers slots 1–32. For staff `s`, initialize `scale[s]=25`, `baseline[s]=3150s−27150−525`. For a Code 8 staff of size `z`, with missing size defaulting to 1, set `S=f32(25z)`. One vertical SCORE step is `7S` raw units. A staff-line interval is `14S`, two steps. Staff bottom is 21 scale units above its baseline.

For a standard staff record, use `d=f32(P4 % 1000)` and `bottom=f32(d×S×7+3150s−27150)`. The recovered branch rejects `d ≥ 300`. If P10 is present, the operand instead contains a reference staff and an offset. Reference is the converted signed word divided by 100, with zero selecting staff 1. Compute `distance=f32((P10 % 100)×4000)` and, for the helper's alternate-unit mode, store `f32(distance / f32(2.54))`. Then `bottom=f32(21×scale[reference]+baseline[reference]+distance)`. Store `baseline[s]=f32(bottom−21S)`. Whether alternate units apply depends on the caller.

A negative P7 suppresses the staff lines. A positive P7 overrides the line control extracted from the thousands part of P4. A zero control defaults to five lines. If control is at least 100, its hundreds part can shift the initial drawn line by whole line intervals and the remainder is the count. Left and right are `150×P3` and `150×P6`, with a default right of 30000 raw units. Width selector P11=99 maps to 0 for staff lines. Missing thickness on a size below the stored `0.65` selects −1.

Item position uses `X=f32(P3×150)` and `Y=f32(P4_eff×S×7+baseline[s])`. The common preparation uses `P4 % 100` for Codes 1, 2, 3 and 6, preserving the packed control for its own consumer. Code-specific endpoints can call the same vertical projection with another parameter. The screen horizontal-limit flag tests the unrounded intermediate against 32700. An initialization pass loads staff geometry from Code 8 records. The actual draw dispatch updates it again at each such record. Positioning therefore depends on record order and the staff state at the time of dispatch.

### Worked coordinate

For staff 8 of size 1 with P4=0 and no P10, bottom is −1950 and baseline is −2475. A note with P3=40 and P4=7 has raw X=6000 and Y=−1250. On the standard preview profile the point is `(270, 600)`. At the default EPS scale with offsets L=2100 and B=27250, the corresponding PostScript values before output formatting are about `(145.8, 468.0)` points.

Sources: [original staff and item projection](../../ScoreReverseEngineering/NativeFragments/PageCoordinates.cs), [native document prepass and per-item state](../NativePageLayout.cs).

## Coordinate spaces and units

| Space | Projection or unit | Direction and origin |
| --- | --- | --- |
| Horizontal P3 | 150 raw units per parameter unit. 200 gives 30000 raw units. | X increases to the right. |
| Vertical staff P4 | 7 × staff scale per step. | Y increases upwards in the raw space. |
| Physical units | 4000 raw units per inch at normal size. 72 PostScript points per inch gives .018 point per raw unit. | Left and bottom offsets are print-job parameters. |
| Nominal standard Letter preview | Width `34000/30`, height `44000/30`. `x=2100/30+X/30`, `y=(44000−27250)/30−Y/30`. | Native canvas origin is top-left. View zoom, DPI, scroll and paper frame add another transform. |
| DOS raster viewport | `x=trunc(X_word/divisorX)+originX`, `y=originY−trunc(Y_word/divisorY)`, with word arithmetic in the recovered decoder. | The native 640-wide graphic uses a local Y offset of 97. The DOS clip and scroll state are separate. |
| EPS / PostScript | Default setup `.018 dup scale 2100 27250 translate`. Active jobs add size, margins and orientation. | PostScript coordinates use bottom-left orientation. A print transform can rotate or offset the drawing. |

A default staff line interval of 350 raw units is `350/4000 = .0875` inch, or 6.3 points at nominal print scale. A P4 step at default size is 175 raw units, or 3.15 printed points. DPI in Windows changes the display projection. Print resolution in the SCORE job also appears inside geometry and stroke calculations, so it can affect the emitted path. The job uses binary32 `4000/resolution + .01` for its geometry unit and divides by size for its line unit. The file's measurement field changes caller conversions. It does not turn P3 into centimetres.

Sources: [buffered DOS decode](../../ScoreReverseEngineering/NativeFragments/BufferedCoordinateDecode.cs), [viewport](../NativeDosViewport.cs), [print job](../NativePrintJob.cs), [EPS output](../NativeEpsExport.cs), [print rotation](../../ScoreReverseEngineering/NativeFragments/PrintRotation.cs).
