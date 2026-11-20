# SCORE Modern help

Use this guide for Windows workflows and Tom’s Help for SCORE’s command language, item codes and packed parameters.

## SCORE Modern

- [Setup and portable settings](SETUP.md) covers first launch, resource folders and sharing settings.
- [Files, editing, and output](WORKFLOW.md) covers the Main, Edit, Input, Disk and Print workflows.
- [View, navigation, and compatibility](VIEW.md) covers vector and DAT views, zoom, rulers and Strict SCORE behaviour.
- [Fonts, symbols, and layout utilities](RESOURCES.md) covers Type 1, FONTCONV, the Symbol Browser, DRAW, PAGE, SCORLAS and JUST.

## Start here

Press **Alt+P** for Preferences. Press **Alt+Shift+M** for the workspace context menu with the pointer over the page. **Alt+M** toggles the SCORE function-key rows.

Use **Preferences → Shortcuts** to assign keys to workspace context-menu items. Choose an item, press a Ctrl or Alt letter/digit combination or a function key in the capture field, and click **Assign**. SCORE and editor shortcuts are protected by default. To reuse a protected shortcut, enable **Allow shortcuts to override SCORE defaults**. Conflicting assignments are inactive in Strict SCORE behaviour.

1. On first setup, choose your startup folder, drawing LIB and, if available, your Type 1 fonts. SCORE Modern requires a verified serial-bearing DAT in LIB. The application stores local settings in `ScoreModern.ini` beside the executable.
2. Open a score with **Open…** or type `G MOZART` at the main prompt when that file is in your working folder. `CD` changes the working folder used by relative names. See [G commands](G.md) and the [Disk menu](CODE47.md).
3. Press **F3** to open the Edit menu. Choose an item with the mouse or enter `I` followed by its item number. **Esc** leaves Item Edit. See [Item Edit keys](CODE31.md) and the [Edit menu](CODE38.md).
4. Use **F6** for printing and export. The Windows export menu uses **F5** for EPS. The SCORE-format menu exposes PRINT.SCR and the original print-style parameters. See [Printing and EPS export](PRINT.md).

## Enter music, text and drawings

- **F2 music input** proceeds through [pitch](CODE33.md), [rhythm](CODE34.md), [marks](CODE35.md), [beams](CODE36.md) and [slurs](CODE37.md). Use the displayed note numbers in the last three stages. F2 MIDI entry and live rhythm input are separate from F5 playback.
- **F4 text input** and `T` create [Code 16 text](CODE16.md). To edit existing text, select it and use `TX` or click the displayed text below its parameters.
- **Symbol Browser…** on the context menu outside the page shows drawing banks with slots numbered 0–9. Right-click a slot to copy its number or edit it in DRAW. See [Code 9 library symbols](CODE09.md) and [Code 11 named drawings](CODE11.md).
- **DRAW…** opens the vector editor. **Open Bank…** browses drawings by file and preview. Turn on **Plot points** to add points. Click and drag a point to move it. **Snap to grid** follows the square grid spacing. With the canvas focused, the arrow keys move a point by one DRAW unit and Shift+arrow moves it by ten. **Ctrl+Z** undoes an edit. **Ctrl+Y** and **Ctrl+Shift+Z** redo it.
- **PAGE…**, **SCORLAS…** and **JUST…** on the same menu open the Windows versions of the layout, tiling and justification utilities.

## Fonts and display

Use **Preferences → Windows → Fonts & output** to select a Type 1 folder and review font resources. SCORE uses PSC assignments for font metrics. Keep the matching Type 1 outlines and AFM metrics together. If several fonts declare the same internal PostScript name, use the font version chooser. **FONTCONV** creates or replaces PSC metrics with the selected font assignment.

To change the notation view, open **Preferences → Windows → Display & text**. Choose vector engraving for the print-oriented display or a DAT view for SCORE’s screen glyphs. The DOS viewport option reproduces the DOS framing. The separate **Zoom framing** setting controls how zoom commands position the page. See [Z commands](Z.md).

## Find a topic

Press **F1** for help. In Item Edit it opens the item’s topic. During an F2 entry stage, it opens that stage’s keys and syntax. At the main prompt, `?` opens this guide, `?J` opens the J commands, and `?16` opens text parameters. `?PREF` and `?PRINT` open their named topics. The help window also provides a full-text search and topic navigation.

The bundled Markdown topics are available in both normal and Strict SCORE modes. You can also read them directly in the Help folder beside the executable.

## Read SCORE syntax

SCORE’s command letters depend on the current mode. For example, `CD` changes the working directory in Main mode and has a lyric-editing meaning in Item Edit. Parameter numbers such as `P3` and `P4` describe fields in an item. Some fields pack several controls into separate digits or decimal places. Read the item’s complete parameter entry before changing a packed value.

The [SCORE command and item index](HELP.md) and the detailed reference topics retain the DOS descriptions from Tom’s Help. **In SCORE Modern** notes identify Windows controls, compatibility choices and useful differences. A DOS description may cover controls beyond the Windows implementation.

See [Strict SCORE behaviour](COMPAT.md) for the effect of each compatibility setting.
