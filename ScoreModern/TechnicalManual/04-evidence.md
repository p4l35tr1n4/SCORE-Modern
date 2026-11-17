# 04. Verification and limits

See [SCORE 4.01 evidence and limits](07-scor4.md#evidence-and-limits) for the SCOR4 profile's reference tests.

Read each result at the boundary it measured. A match at a stored coordinate word establishes that value for the tested route. A matching rendered image establishes the outcome at the tested raster configuration. Equal EPS appearance alone does not establish the same record order, packed fields, numeric literals, or storage contents.

| Evidence | Where to look | What to compare |
| --- | --- | --- |
| Executable spans, entry and exit states | [reverse-engineering decisions](../../ScoreReverseEngineering/ANALYSIS-DECISIONS.md), [coordinate recovery](../../ScoreReverseEngineering/CoordinateConverterAnalysis/README.md), [clipping faults](../../ScoreReverseEngineering/ClippingFlagsFaultAnalysis/README.md) | Original instruction span, raw inputs/outputs, flags, external DOS or BIOS effects |
| Item and layout output | [engraving root causes](../../ScoreReverseEngineering/EngravingRootCauses/README.md), [step 5 layout verification](../STEP-5-VERIFICATION.md) | Stored records, working tables and exact control route |
| DOS display | [DAT consumers](../../ScoreReverseEngineering/DatConsumerAnalysis/README.md), [DAT display modes](../../ScoreReverseEngineering/DatDisplayModes/README.md) | Raw vectors, DOS raster viewport and display state at the specified zoom |
| Glyph/text processing | [vector text integration](../../ScoreReverseEngineering/VectorTextIntegration/README.md), [FONTCONV constants](../../ScoreReverseEngineering/FontconvAnalysis/decoded-constants.json) | Bank and font version, code mapping, metrics, resulting placements and contours |
| Windows application pairing | [SCORE Modern compatibility help](../Help/Topics/COMPAT.md) | Option values and relevant differences between executable-matching and documented paths |

To reproduce an output comparison, retain the exact input bytes, selected LIB and external font versions, active PREF and PRINT settings, operation sequence, mode, and zoom or print parameters. For module-level investigation, start with the source's declared arguments. Follow the caller to determine which staff table, scratch state, and original arithmetic environment supplied them. Compare values at each indicated store or narrowing operation, including their raw bits. Preserve overlay-relative address labels as evidence labels.

Open boundaries include the unobserved WinScore large word-count variant, unverified or corrupt buffer histories, full x87 status/exception equivalence for the mathematical projections, hardware-dependent BIOS/raster side effects outside measured contexts, and executable paths or control combinations without recovery evidence. F5 playback, physical MIDI timing, and native Windows display extensions have their own scopes. The current layout and drawing modules contain guards at those boundaries. The appendices identify exact recovered literals and implementation code. Data supplied by an external font or a user drawing varies by installation and must come from that installation when reproducing its output.
