# Code 15 · Included PostScript

## In SCORE Modern

See the [PostScript programming supplement](11.md) for the distinction between a conventional EPS and a program that consumes SCORE parameters. Display depends on the included PostScript settings and supported rendering path.

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

Code 15 -- Included PostScript PS and EPS files -- Edited by Thomas M. Brodhead

parameter structure

| Parameter | Meaning |
| --- | --- |
| `P1` | code num. |
| `P2` | staff num. |
| `P3` | horiz. pos. |
| `P4` | vert. pos. |
| `P5` | horz. size |
| `P6` | vert. size |

```text
╔═════════════════════════════════════╦═════════════════════════╗
║          p7, p8, p9, p10            ║           p11           ║
║ bounding box numbers if file is EPS ║ = -9999 if EPS graphic, ║
║    - do not change in that case -   ║ else special PS program ║
╚═════════════════════════════════════╩═════════════════════════╝
```

```text
╔══════════╦══════════════════╦═════════════╦══════════════╗
║    p12   ║        p13       ║          p14-p19           ║
║ rotation ║ reset PostScript ║ holds PostScript file name ║
╚══════════╩══════════════════╩═════════════╩══════════════╝
```

================ CODE 15 creates: ================ * An included PS or EPS file

--> It is important to understand the distinction between PS and EPS.

--> A PS file is simply a PostScript file, which may be a complex program.

--> An EPS file is an Encapsulated PostScript file, which is intended to be a static image suitable for inclusion in another graphic.

--> Code15 allows a PS or an EPS file to be included in a SCORE file.

--> Code15 items behave differently depending on whether the file is a PS or an EPS.  Specifically, the parameters function completely differently.

--> If it is a PS file, P3-P12=0, but a series of variables are written to the resulting EPS output of SCORE (i.e. the PostScript that SCORE writes when the file is sent to a PostScript printer or written to an EPS file on disk), and some of those variables may be manipulated by P3-P12 before "printing"; this is discussed in detail below.

--> If it is an EPS file, the Code15 will have P11 set to -9999, the bounding box coordinates are recorded in P7-P10, and the item may be resized using p5 and p6.

--> Rather than simply relying on the extension (.PS or .EPS) for determining which kind of file it is, SCORE scans the file for a line starting with:

%%BoundingBox:

...and which has at least ONE number following it.  If found, then it is presumed to be an EPS file, not a PS file.

--> The parameters of Code15 are different for PS and EPS, with one exception:

*** If P13 is set to *ANY* non-zero number, a new file will be requested ***

===> IF IT IS AN EPS FILE, THE FOLLOWING PARAMETERS OBTAIN:

```text
P5     = Horizontal Size (to resize the EPS graphic manually)
P6     = Vertical Size (to resize the EPS graphic manually)
P7-P10 = Bounding box information collected from the EPS file (DO NOT ALTER)
P11    = -9999 by default to indicate a standard EPS graphic.  To make the EPS
         file function as a PostScript program in which P3-P12 serve as numeric
         controls within the program, set P11 <> -9999.  See discussion below.
P12    = Rotation in clockwise degrees
```

If P13 is set to *ANY* non-zero number, a new file name will be requested. (This is true for included PS files as well.)

P14 through P19 hold the file name, which may include a drive letter and path (so an EPS footer could be kept in a dedicated directory, etc.) --> 24 characters are possible

===> IF IT IS A PS FILE, THE FOLLOWING PARAMETERS OBTAIN:

Notice P3-P6 may be used to create a BoundingBox for your PS program manually:

```text
P3 = Horizontal Location of lower left corner of the BoundingBox
P4 = Vertical Location of lower left corner of the BoundingBox
P5 = Bounding Box Horizontal Size (= p5 * 1.1666666 * StaffSize)
P6 = BoundingBox Vertical Size, measured in P4 vertical staff steps
     (P6 = P5 if P6 = 0).
P11 <> -9999 indicates a PS program (the only restriction on this value)
P13 <> 0 prompts for a new file.
```

The values in P7-P12 are available for your own use in manipulating your PS file.  (P3-P6 may also be used, although SCORE has optimized them for the creation of the custom BoundingBox described above.)

Specifically, the values in P3-P12 are written to the PostScript output of SCORE as PostScript variables /p3, /p4, /p5, etc. through /p12.  That way, you could construct a PostScript program that uses a PostScript variable named /p7 that would receive the value of P7 in the parent Code15 item. This would in turn allow you to create dynamic PostScript programs that respond to the parametric values of your Code15 items.

There are some considerations, however. P3-P6 are treated specially, as they are assumed to be correlative to the location of your PostScript graphic and are translated into the 4000-points-per-inch scaling system of SCORE's PostScript and the results stored in separate variables.  You will therefore have access to the original P3-P6 values as found in your original Code15 item, but you will also have values that correlate to the location of the graphic within SCORE's PostScript grid system, and you may use those values for linear transformations of the graphic, etc., using the other parameter values (P7-P12).  At the same time, you are not restricted to use P3-P6 for the coordinate scaling purposes that are performed and stored in the other unique variables that are created; this is merely a courtesy by SCORE.

The 21 variables that are created when P11 <> -9999 are:

/p3 /p4 /p5 /p6 /p7 /p8 /p9 /p10 /p11 /p12 /staffsize /stafflevel /pp3 /pp4 /pp5 /pp6 /globalsize /lnwd /loff /boff /pointslevel

Definitions follow:

/p3 - /p12 = The P3-P12 values of your Code15 item (when P11 <> -9999).

/staffsize = Code8 P5 staff size of the staff the Code15 item was attached to.

/stafflevel = the location of the 0,0 of the staff the Code15 item was on, measured in 4000ppi (ppi= points-per-inch).  For staves with p4=0, as the staff's size is altered with p5, the /stafflevel moves in tandem (shifting by ((175 * c8p5) * 3) in 4000ppi). This value is *not* relative to any X,Y image offset (e.g., .5 .75) used when the EPS file is created; changing the X,Y offset does *not* affect /stafflevel

/pp3 - /pp6 require some explanation:

/p3 and /p4 are assumed to be the lower left coordinates of your PS file in Score p3 and p4 units.

/p5 and /p6 are assumed to be the respective horizontal and vertical widths of a manually-created BoundingBox.

SCORE takes those values and translates them into the 4000ppi coordinates of SCORE's PS output and stores the results in /pp3 - /pp6.

The exact formulas for /pp3 - /pp6 follow these preliminary definitions:

MagicNumber1 = 175 175 is the size in 4000ppi points of a single p4 unit on a size-1 staff

MagicNumber2 = 150 150 is the size in 4000ppi points of a single p3 unit regardless of staff size

/pp3 = /p3 * 150 /pp4 = /stafflevel + (p4 * 175 * /staffsize) /pp5 = /p5 * 175 * /staffsize /pp6 = /p6 * 175 * /staffsize

/globalsize = the print size specified with setting 2 in the print menu (1.00 = 100%, .5 = 50%)

/pointslevel = the same as /stafflevel, except measured in 72ppi. This *is* relative to X,Y image offset (e.g. .5 .75) used when the EPS is created; changing the X,Y offset does in fact change /pointslevel

/loff = the lefthand offset of the clickpoint of a staff with p2=1, p4=0, p5=1 measured in 72ppi.  It is simply the value of the /lmar variable times .018 (/lmar is calculated at 4000ppi; multiplying it by .018 recalibrates the value to 72ppi.)  This is the distance to the right of the paper's edge that the image will begin, corresponding to the X offset setting in the print menu.

/boff = the distance between the clickpoint of a staff with p2=1, p4=0, p5=1 to the absolute 0 vertical (Y) coordinate, measured in 72ppi.  It is simply the value of /bmar variable times .018 (/bmar is calculated at 4000ppi; multiplying it by .018 recalibrates the value to 72ppi.) This is the distance below the AbsoluteVerticalCoordinate that the image will begin. This corresponds to the Y offset setting in the print menu.

/lnwd = linewidth at 72ppi.  This is simply the /wdl setting, which calculated at 4000ppi, multiplied by .018, recalibrated to 72ppi.

Some notes:

Create a Code8 item with p2 = 8, p4 = 1, p5 = 1. Then create a Code12 dot with p2 = 8, p4 = 14.1428571.  The dot is at the absolute 0 vertical (Y) coordinate

Henceforth, this value will be named AbsoluteVerticalCoordinate

The 0 horizontal (X) coordinate is simply p3=0 for any staff.

The 0,0 position for a staff is three p4 units below the staff.  As staves with p4=0 are resized, their bottom staff lines remain in the same location, but the location of an item at p4=0 on that staff will change in proportion to the size of a single p4 multiplied by 3 (because the p4=0 location of an item attached to that staff will be three p4 units below the staff.)

If stafflevel variable is added to pp5 or to pp6, the result will be like that of pp4.  Thus the value in p5 and p6 corresponds to p4 units above/below the AbsoluteVerticalCoordinate
