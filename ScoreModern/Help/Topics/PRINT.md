# Printing and EPS export

## In SCORE Modern

F6 opens the configured print menu. In the Windows export menu, F5 exports EPS. The SCORE-format menu reads and saves PRINT.SCR and uses its configured output settings.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

PostScript printing from SCORE

Items requiring a Yes/No response can be given Y or N, or the numbers 1 or 0. The default values appear as in the menu, however items 6 and 11 may be set in your preference file, PREF.SCR. Item 4, the print position offsets, are set automatically by SCORE unless item 5 (Freeze offsets) is set to Yes. Any settings given will be maintained from page to page until changed.

1.  The default OUTPUT for printing is direct to the PostScript (laser) printer. (Use the SCORLAS program for batch printing and printing large composite pages. Use SCORDOT for printing on dot matrix printers.) If you have not designated the output channel in your PREF.SCR file, you will be asked to indicate whether you are using a Serial or Parallel connection to your printer.  If you are unsure about your printer connection hit the <esc> key so the printing process will not be started.  (It is very important that you give the proper response here, as an error could require that the computer be restarted.)

```text
<esc> -- If you are printing a page with very many items, the printing
         process may be halted at any time by hitting the <esc> key.
         (See regarding DESC - disable <esc> - below.)
```

The printing output may be directed to a disk file by typing a name after the number 1. The program will automatically add the extension .EPS (for encap- sulated PostScript) to the given file name.  EPS files may be transported to other computers for printing or they may be processed by a variety of desk top publishing systems or they may be converted to formats needed for all- digital FAX transmission. If you wish the EPS file to have the same name as your current SCORE file (i.e. the name appearing in the upper left corner of your screen), you need only type 1 followed by a space and a dot. You will be warned if there is the possibility of writing over an existing file. Type 1 alone to return to direct laser printing. (When saving your regular SCORE files it is a good idea to avoid using extensions such as EPS, TMP, EXE, COM, BAT, or any other three-letter combinations that have come to have special traditional meanings in computer data organization.)

2. SIZE -- Printing at size 1 will normally produce music with a staff width of 7.5". If wider paper is used the printing size should be scaled upward proportionately. Then, if a standard staff size is to be maintained, a smaller size factor must be given for P5 of each staff. Thus if you are printing to 11" wide paper, a printing size of 1.333 must be given to utilize the full width of the paper. In this case, if the printed staff size is to remain at the basic size 1, P5 of each staff should be set to .75 (.75 x 1.333 = 1).

3. PAPER size -- The three basic American paper sizes may be set with the following abbreviations: LTR = Letter (8.5 x 11), LEG = Legal (8.5 x 14), LDG = Ledger (11 x 17) For any other sizes, type in the appropriate numbers in inches.

4. OFFSETS -- The image offsets are figured from the bottom left hand corner of the paper to the bottom left corner of staff 1 (P3 = 0, P4 = 0). These offsets refer to this position even if staff 1's P3 and or P4 have been altered. Before a page is printed SCORE scans the data to see if some point may fall out of bounds. If the image needs more than .75" below staff 1, the Y (vertical) offset is automatically adjusted.

To manually change the offsets, type two numbers (X, Y) after typing the 4.

5. FREEZE offsets -- The Y offset (vertical) is usually set automatically. If there are items found below staff 1 (copyright notices, etc.) Y is adjusted to ensure that all printing will appear above the paper's required 1/2" margin. Special offsets are set with the number 4 (see above). Setting FREEZE to Yes will hold these offsets for all printing until you change the offsets or exit SCORE.

6. RESOLUTION of Laser Printer -- Normally you will set the laser printer resolution in your PREF.SCR file. However, you may want to make a temporary change if you are preparing .EPS files for printing away from your own computer system.

7. LINE width -- This sets the basic width of lines drawn by SCORE. The default setting creates lines with a thickness of  2/300". This will usually control the thickness of the staff lines, note stems, etc.  Using a lower number here will produce generally lighter copy while higher numbers produce darker copy. The line width control may be useful when printing large orchestral scores which require a very small staff size. Decreasing the line width will usually make small accidentals, etc. appear clearer. Many SCORE items have a parameter which can add to or subtract from the thickness of lines. (PostScript type fonts are in no way affected by the line width setting.)

8. HEADER/FOOTER -- A header or footer may be added to the printed page by typing H or F after the number 8. Both header and footer will appear if you type B. The header/footer has the following format.

File name. / Project name. / File creation date-time. / Print data.

You will be prompted for a project name and the vertical offset of the header box.  The project name can include up to 18 characters. The default offset from the top of a page is -.18". From the bottom of the page it is +.18". The boxes may be located anywhere else by changing these numbers. The Print Data includes the Size Factor, the X/Y offsets, the printer Resolution, and the basic Line Width.

9. ROTATE --When ROTATE is set to Yes the printed image is rotated 90 degrees, to "landscape" format. The size factor is automatically increased by 1/3. Thus, size 1 printing will change to size 1.333. If you wish to maintain equivalent staff sizes when rotating copy (e.g. for organ music), P5 in each staff item should be scaled appropriately. For example, if you wish a rotated staff to print at the equivalent of size .9, you must set the staff's P5 to .675 (.9/1.3333 = .675).

10. MIRROR -- When MIRROR is set to Yes the PostScript printing produces a mirror image of the complete page of music. This is useful when producing transparancies for direct exposure to lithographic plates.

11. SETSTROKE -- Because of internal round-off errors, PostScript does not always produce thin lines of consistent thickness. This problem is especially noticeable with some staff lines drawn on 300 dpi printers. If SETSTROKE is set to Yes, a special PostScript program is sent to the printer which corrects this problem. This added program can increase the printing time by up to 40% so this feature should be used only when it is felt to be necessary. (SETSTROKE may also be implemented in your PREF.SCR file.)

12. MANUAL -- When MANUAL is set to Yes the printer will wait for paper to be fed to the manual input path. This command is useful with printers that have no external commands for manually feeding paper.

13. SETPAPERTRAY -- For most printers this should be left at 0. If your printer has multiple trays, set this as needed.

14. READ print parameters. If all these commands have been set up in a normal disk file, the READ command may be used to activate them. You should type the name of the file to be read in immediately after the word READ.

15. SAVE the current print parameters. The current state of all the print commands may be saved at any time. In this way you can easily recall a set of commands to maintain a consistent format when printing many pages of a large project. You should type the file name to be used immediately after the word SAVE.

16. DESC -- enable/disable <escape> key (0 or 1). While the <esc> key is active you will not be able to type ahead any commands until the printing process is completed.  Disabling the key allows the typing ahead of commands while a large page of music is being transferred to the printer.

The same result may be accomplished while the printing is taking place by hitting the "G" key (= go). This will disable <esc> through to the end of that particular run, thus allowing you to type ahead.
