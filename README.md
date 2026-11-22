# SCORE Modern

Windows application for legacy SCORE music engraving files. Core **0.3.67 RC3**, build **2427**; shared managed DLL **0.3.61.2**.

RC3 includes compatibility, engraving, viewport, editing and part-extraction corrections, including key and meter state, rest consolidation, cues, transposition and part resources. Legacy PAGE and SCORLAS remain available. SCOR4 differences are selected in Preferences.

## Run

Extract the binary release and keep ScoreModern.exe, ScoreModern.dll and Help together. Configure your compatible SCORE installation at first startup. Supply your own drawing libraries, fonts and serial-bearing DAT; original SCORE files and personal settings are not included. Preserve your existing configuration when updating. Requires 64-bit Windows and .NET Framework 4.8.

## Build

```powershell
& .\ScoreModern\Build.ps1 -OutputDirectory (Join-Path $PWD 'build') -CoreDllPath (Join-Path $PWD 'Binaries/ScoreModern.dll')
```

Omit -CoreDllPath to compile the DLL from ScoreReverseEngineering/NativeFragments. Binaries contains the matching public executable and DLL. Publication sources use portable startup defaults and a generic legacy version label. Copyright and attribution notices are retained.

See the [technical manual](ScoreModern/TechnicalManual/README.md), [SCOR4 chapter](ScoreModern/TechnicalManual/07-scor4.md), [DLL guide](ScoreModern/TechnicalManual/06-dll-integration.md) and [API reference](ScoreModern/TechnicalManual/DLL-API.md). The binary package includes README, Help and TechnicalManual only alongside the executable and DLL.

The recovered compatibility domains and remaining limits are described in the manual; RC3 does not claim exhaustive equivalence for all original parameter combinations. The unspecified later PAGE4 4.01 transposition correction remains uncertified.
