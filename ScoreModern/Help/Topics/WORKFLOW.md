# SCORE Modern · Files, editing, and output

## Open and save scores

- Right-click outside the page for **Open…**, **Save**, **Save as…**, and **Print…**. Recent files are in the **Recent** submenu.
- At the Main prompt, use `G filename` to open a score and `SA filename` to save under a name. Leave Item Edit before saving. With relative paths, check the current working folder first.
- Use **F7** for the Disk menu. In **Preferences → Windows → Files & resources**, enable Windows file and folder dialogs if you prefer browsing. Strict SCORE behaviour uses the SCORE prompts.
- Use **Preferences → Windows → File associations…** to register extensions with Windows. Choose the default app for those extensions in Windows settings.

## Select, edit, and enter

Press **F3** for the Edit menu. Use `I` with an item number when you know which record to edit, or select on the score with the mouse. The parameter display shows the selected item’s values. Click a parameter to edit it in place. For a text item, use `TX` or click the text below its parameters. Press **Esc** to leave Item Edit.

**F2** opens staff input. Set up the staff and enter pitch, rhythm, marks, beams and slurs in order. During marks, beams and slurs the numbers on the preview identify the notes that each instruction addresses. See [pitch input](CODE33.md), [rhythm input](CODE34.md), [marks](CODE35.md), [beams](CODE36.md), and [slurs](CODE37.md). To enter from a MIDI controller, first select your input device in **Preferences → Windows → Input & selection**.

## Print or export

**F6** opens the configured output menu. Under **Fonts & output**, choose between the SCORE-format menu and the Windows print and export menu. In the Windows menu, **F5** exports EPS. The SCORE-format menu exposes the print settings and the **Read PRINT.SCR** and **Save PRINT.SCR** actions. See [printing and EPS](PRINT.md) for the command controls.

Print and EPS depend on the score’s resource files. If the status area reports an incomplete engraving, investigate the named item or missing resource before producing final output.
