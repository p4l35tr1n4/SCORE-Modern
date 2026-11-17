# SCORE Modern · Fonts, symbols, and layout utilities

## Set up Type 1 text

Under **Preferences → Windows → Fonts & output**, choose the font folder. Keep each Type 1 outline with its corresponding AFM metrics. A SCORE font number uses a PSC assignment. Inspect **Font resources** to identify the metric assignment and outline used by a score. If more than one font declares the same internal name, use the version chooser to select the intended version.

Use **FONTCONV** to create a PSC assignment from an available Type 1 font and its AFM. You can also save a named copy for reuse. The outline used to render and the AFM used for metrics should correspond to the font you selected. See [Code 16](CODE16.md) for text parameters and inline controls.

## Find or edit symbols

Open **Symbol Browser…** by right-clicking outside the page. Each row is a drawing bank. The symbol cells show the internal name and overall number for numbered banks. Right-click a symbol to copy the number or open its exact slot in DRAW.

In **DRAW**, use **Open Bank…** to browse by file and preview. Enable **Plot points** to add vectors and drag a filled point marker to move it. **Square grid** displays the grid and **Snap to grid** applies its spacing when plotting or dragging. Click the point on the canvas, then press an arrow key to move by one unit, or hold **Shift** to move by ten. **Ctrl+Z** undoes, and **Ctrl+Y** or **Ctrl+Shift+Z** redoes.

Use [Code 9](CODE09.md) for numbered library symbols and [Code 11](CODE11.md) for named external drawings.

## Assemble scores

Open **PAGE…**, **SCORLAS…**, or **JUST…** from the context menu outside the score.

- **PAGE** assembles systems or parts from an ordered file list. Drag files to reorder them and use the settings controls to reuse a job setup.
- **SCORLAS** tiles score files into pages. Order the inputs before tiling. For a regular batch, use the starting page, files per page and page or file count controls.
- **JUST** provides the integrated justification workflow. Use copies when testing a new batch of layout settings.
