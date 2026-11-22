# How SCORE works
## Technical manual for SCORE 3.11 and the recovered SCORE 4.01 differences

This manual describes the original SCORE executable and its related DOS utilities as reconstructed in SCORE Modern. It covers storage, numeric precision, staff coordinates, layout and justification, engraving, device output, and resource files. The accompanying appendices give the literal values and implementation details needed to reproduce the recovered routines.

Chapters 01–03 and 05 use SCORE 3.11 as the baseline. Chapter 07 gives the measured changes in the supplied SCORE 4.01 executable and its companion utilities. It also identifies the routines and resource choices selected by the SCOR4 profile in SCORE Modern Core.

| Chapter | Contents |
| --- | --- |
| [01. Storage, arithmetic and coordinates](01-coordinates.md) | MUS and DRW records, packed controls, binary32, x87 conversion, staff tables, display and print units |
| [02. Spacing and justification](02-spacing.md) | F2, SP, LI/LJ/L, J, JT, VJ, staff height, ripple, page assembly and tiling |
| [03. Engraving, text and output](03-engraving.md) | Item routes, stems, beams, slurs, text metrics, DAT, vector buffers, clipping, print and EPS |
| [05. Pitch, editing, and retained state](05-pitch-and-editing.md) | TR/TRK, TAB, edit and selection math, display histories, decimal formatting |
| [06. Integrating ScoreModern.dll](06-dll-integration.md) | Managed assembly setup, tested examples, data contracts and whole-score automation |
| [07. SCORE 4.01 and its DOS utilities](07-scor4.md) | Version and resources, viewport arithmetic, input state, record editing, beam calculation, spacing, PAGE4 and utility comparisons |
| [DLL API reference](DLL-API.md) | Exported types, method signatures, properties, fields and enum values |
| [Appendix A. Lookup tables and literal data](A-lookup-tables.md) | Exact bit patterns, initializer arrays, external section offsets and captured data |
| [Appendix B. Routine and evidence index](B-routine-index.md) | Recovered modules, SCORE Modern integration and original evidence |

### How to read the formulas

- **Observed** describes an original executable, file, buffer, or output comparison. A segmented address in a source comment is relative to the named recovery context.
- **Recovered** describes a source model of an observed or decoded routine. It has the scope identified in the implementation. A numerical model can match the stored coordinates while leaving the x87 flags or exceptions outside its domain.
- **SCORE Modern** identifies a native integration choice. Its work area, native preview and resource paths belong here. Some compatibility switches select between the measured executable and published help.

The chapters describe the most useful algorithmic and numerical contracts. The routine appendix carries the complete recovered fragments for branch-specific logic and exact operation order. The corpus includes guarded cases and source comments that describe the state of an earlier research phase. Consult the actual callers and comparison reports when a source comment and a later integration disagree.

The term **raw coordinate** means a SCORE internal drawing coordinate before projection to a device. The term **parameter coordinate** refers to a value such as an item's P3 or P4. Those two axes use different scales. In formulas, `f32(x)` means store to a 32-bit float at that point, `trunc(x)` means discard the fractional part toward zero, and `word(x)` means use the low 16 bits interpreted as signed unless the routine specifies otherwise. Decimal renderings of float literals are explanatory. Hexadecimal bits in the table appendix are authoritative.

The chapter and appendix links work within the manual folder. Direct links to original source and evidence refer to the SCORE Modern workspace. The expanded routine listings are included with the manual.


The current shared-DLL SCOR4 controls, API flags, formulas and rounding details are documented in the final section of [Chapter 07](07-scor4.md). The DLL API index is regenerated for file version 0.3.61.2.

Source links target the development checkout. The routine-listing appendices reproduce the implementation for reading with a binary release package. External DAT and captured-table appendices remain unchanged; the source listings, initializer inventory and API index have been refreshed.
