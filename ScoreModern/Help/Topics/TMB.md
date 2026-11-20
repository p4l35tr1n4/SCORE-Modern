# External TMB utilities

Use **Preferences → TMB utilities** to choose the utilities shown in the workspace context menu. Each row takes the external Windows executable and its support folder. Leave the support folder blank when the files are beside the executable.

**Producer DAT** selects the verified SCORE identity written into the temporary MUS file. If the utility is licensed for a particular identity, select its corresponding DAT. A blank field uses the active SCORE library's verified DAT. These settings are machine-specific.

Open a score and leave item editing before running a utility. Right-click the workspace and select **ACCS**, **BEAM**, **EDITSCOR (ES)**, **LABELS**, **LJ**, **SEP**, **TRANSPOSE**, or **VJ**. Utilities with required arguments ask you for them.

| Utility | Inputs the dialog may request |
| --- | --- |
| ACCS | Kerning and configuration files when they are absent from the selected support folder |
| BEAM | Uses its staged support files |
| EDITSCOR | An EDITSCOR macro |
| LABELS | A control file and, if the support configuration omits it, the AFM CapHeight. Assign positive P9 instrument IDs to the staves before running LABELS. |
| LJ | Line up, justify, or both |
| SEP | Staff numbers and which voice to delete |
| TRANSPOSE | A control macro or a staff selector with the transposition, clef and key options |
| VJ | Report the height or set a target height using the score's units |

Core stages a temporary copy of the current score and the utility files. When the utility returns a changed, valid score, Core loads it as an unsaved edit. Use **Undo** to restore your earlier score, or save to keep the result. The utility log is available in the progress dialog and appears again for an error or unchanged result.

Enable **Keep utility output open until dismissed** in the TMB utilities tab to review the output after a successful score reload. This checkbox saves immediately. Press **Esc** or choose **Close** to dismiss the window. The **Command output** section of **Preferences → Windows → Display & text** controls its text and background colours.
