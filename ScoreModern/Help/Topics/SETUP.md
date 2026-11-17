# SCORE Modern · Setup and portable settings

## First launch

Keep the executable, the accompanying DLL and the Help folder together. On first launch, confirm the startup folder, drawing LIB and Type 1 font locations. Browse to the original SCORE installation if the proposed folders are wrong. Choose a verified serial-bearing DAT in LIB. You can configure Type 1 fonts later, but text that needs an unavailable font cannot display fully.

The startup SCORE folder is the base for finding SCORE resources. The working directory is where relative commands such as `G MOZART` look for files. Use `CD` to change it for the current session. When opening by browsing, the app remembers the last folder used.

## Change resource locations

In **Preferences → Windows → Files & resources**, choose the startup folder and the active drawing LIB. By default a LIB override changes drawings and falls back to the installation LIB for absent drawing files. To use the selected LIB for all resources, enable the corresponding option. That can change DAT glyphs, metrics and the identity used when saving. Check the LIB and font paths in the pinned line above the score.

## Store and share settings

The Windows settings are recorded in `ScoreModern.ini` beside the executable. The **SCORE** tab contains the imported SCORE preferences. Apply stores your changes as overrides. Use **Read PREF file…** and **Save PREF file…** to exchange a SCORE preference file.

Use the portable settings export and import controls to share preferences between computers. The portable file excludes machine-specific folder locations. If startup locations are missing, the next launch asks for them.

**Strict SCORE behaviour** selects the available original operation rules while keeping Windows rendering. Your individual settings become effective again when you turn it off. See [display and compatibility](VIEW.md).
