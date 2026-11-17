# P commands

## In SCORE Modern

See [Printing and EPS export](PRINT.md) and [SCORE preferences](PREF.md).

## SCORE reference

> Source: Tom’s Help, edited by Thomas M. Brodhead.

PR (f6) = brings up the printing menu.  A PRINT.SCR file may be created. See Manual Additions booklet for details.

PREF = see and edit current preference setup. The PREF.SCR file should normally reside in the \LIB directory. See Manual Additions booklet for details.

```text
PM  = prints on the screen the parameter list for all items.
PMX = creates a file containing all parameter data.  If you hit only
        <enter> when prompted for a file name, it will write a file with
        the name "PARAM."
```

P, PP, PPP These commands are shortcuts for typing a code 9 items.  They should normally be followed by 2 or 3 numbers (parameters 2, 3, 4).

```text
pp 3 45 -2   will put "pp" at position 45 on staff 3,
              vertical position -2.
```

(See Code 9 (?9) for details.)

```text
PI as above, but displays "piu."     PZ as above, but displays "pizz."
```
