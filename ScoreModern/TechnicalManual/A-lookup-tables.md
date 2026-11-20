# Appendix A. Lookup tables and literal data

- [Exact source array initializers](A-initializers.md). This inventory preserves original ordering, literal syntax and source locations.
- [Decoded external DAT tables](A-dat-tables.md). Raw float bits and signed word values from the local geometry resource.
- [Captured table evidence](A-captured-tables.md). Machine-readable tables preserved in Markdown code blocks.
- [Routine listings](B-routine-index.md). Review branch literals, inline switch tables and computed or retained tables at their point of use.

## J compiler workspace

These offsets initialize the recovered 64 KiB workspace. The four entries at 3882–388E are adjacent DOS descriptors, not ordinary metric values. The standalone JUST path replaces the two specific descriptor words at 3886 and 388E. A decimal interpretation is supplied only for the ordinary binary32 cells.

| Offset | Raw bits | Binary32 interpretation |
| --- | --- | --- |
| 0x383E | 0x40900000 | 4.5 |
| 0x3842 | 0x40A00000 | 5 |
| 0x3846 | 0x40354FDF | 2.8329999446868896 |
| 0x384A | 0x40000000 | 2 |
| 0x384E | 0x4019999A | 2.4000000953674316 |
| 0x3852 | 0x40066666 | 2.0999999046325684 |
| 0x3856 | 0x40800000 | 4 |
| 0x385A | 0x40200000 | 2.5 |
| 0x385E | 0x40000000 | 2 |
| 0x3862 | 0x4059999A | 3.4000000953674316 |
| 0x3866 | 0x40000000 | 2 |
| 0x386A | 0x40333333 | 2.7999999523162842 |
| 0x386E | 0x40833333 | 4.0999999046325684 |
| 0x3872 | 0x40933333 | 4.5999999046325684 |
| 0x3876 | 0x40833333 | 4.0999999046325684 |
| 0x387A | 0x40C66666 | 6.1999998092651367 |
| 0x387E | 0x40900000 | 4.5 |
| 0x3882 | 0x00002910 | DOS descriptor |
| 0x3886 | 0x00003729 | DOS descriptor |
| 0x388A | 0x002E2910 | DOS descriptor |
| 0x388E | 0x362D3729 | DOS descriptor |
| 0x7748 | 0x3A83126F | 0.0010000000474974513 |
| 0x774C | 0x3F800000 | 1 |
| 0x7750 | 0x40000000 | 2 |
| 0x7754 | 0x40A00000 | 5 |
| 0x7758 | 0x40E00000 | 7 |
| 0x775E | 0xBF800000 | -1 |
| 0x7762 | 0x3D23D70A | 0.039999999105930328 |
| 0x7766 | 0x42C80000 | 100 |
| 0x776A | 0x41200000 | 10 |
| 0x776E | 0x3FC00000 | 1.5 |
| 0x7772 | 0x3F000000 | 0.5 |
| 0x7776 | 0x3DCCCCCD | 0.10000000149011612 |
| 0x777A | 0x3F333333 | 0.69999998807907104 |
| 0x777E | 0x41A00000 | 20 |
| 0x7782 | 0x40133333 | 2.2999999523162842 |
| 0x7786 | 0x3F666666 | 0.89999997615814209 |
| 0x778A | 0x3FD9999A | 1.7000000476837158 |
| 0x778E | 0x40800000 | 4 |
| 0x7792 | 0x41400000 | 12 |
| 0x7796 | 0x3FA66666 | 1.2999999523162842 |
| 0x779A | 0x3FE66666 | 1.7999999523162842 |
| 0x779E | 0x40F00000 | 7.5 |
| 0x77A2 | 0x40D00000 | 6.5 |
| 0x77A6 | 0x40400000 | 3 |
| 0x77AA | 0x3E4CCCCD | 0.20000000298023224 |
| 0x77AE | 0x41700000 | 15 |
| 0x77B2 | 0x40200000 | 2.5 |
| 0x77B6 | 0x3CF5C28F | 0.029999999329447746 |
| 0x77BC | 0x40333333 | 2.7999999523162842 |
| 0x77C0 | 0x406CCCCD | 3.7000000476837158 |
| 0x77E0 | 0x3F800000 | 1 |
| 0x77F6 | 0xC0800000 | -4 |

JUST replacement bits are `0x00001A94` at `0x3886` and `0x4C2D1A94` at `0x388E`. See the [workspace initializer](../../ScoreReverseEngineering/NativeFragments/LayoutSpacingWorkspace.cs) and [spacing consumer](../../ScoreReverseEngineering/NativeFragments/LayoutItemSpacing.cs).

## External data that varies with the installation

| Resource | Located data | Consumer |
| --- | --- | --- |
| DAT | Geometry arrays, glyph directory, metrics and scale vectors | [DAT carrier](../NativeDatGlyphs.cs) |
| LIB drawing banks | Ten slots per directory and signed path tuples | [DRW loader](../DrawingLibrary.cs) |
| PSC and Type 1 | Font assignments, advances, right extents, and outline programs | [FONTCONV](../FontconvPscModel.cs), [text layout](../NativeTextLayout.cs) |
| CODE9.DAT | Text terms selected by the Code 9 term control | [term reader](../NativeCode9Terms.cs) |
| GUITAR.DAT | Named chord entries and fingering data | [grid reader](../NativePrintedChordGrid.cs) |


## SCOR4 number-justification constants

| Value | Binary32 bits | Role |
| --- | --- | --- |
| .2 | 0x3E4CCCCD | Fixed left allowance before P15 displacement |
| 4 | 0x40800000 | Fixed right allowance before P15 displacement |
| 150 | 0x43160000 | Raw horizontal units per SCORE unit for Code11, harp and explicit slur offsets |

The offset/scale division remains extended until the final binary32 allowance assignment. See [SCOR4 controls](07-scor4.md) and `ScoreVersionControls` in the machine/geometry source listings.
