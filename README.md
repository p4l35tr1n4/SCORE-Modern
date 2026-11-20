# SCORE Modern

Windows application for legacy SCORE music engraving files. Core **0.3.67 RC2**, build **2402**; shared managed DLL **0.3.61.1**.

## Build on Windows

Use Windows PowerShell and .NET Framework 4.8:

```powershell
& .\ScoreModern\Build.ps1 -OutputDirectory (Join-Path $PWD 'build') -CoreDllPath (Join-Path $PWD 'Binaries/ScoreModern.dll')
```

Omit `-CoreDllPath` to compile the shared DLL from `ScoreReverseEngineering/NativeFragments`. The app build creates `ScoreModern.exe`, `ScoreModern.dll` and Help files. Keep them together. Supply your own compatible SCORE installation, drawing libraries, fonts and serial-bearing DAT.

`Binaries/` contains the publication executable and unchanged shared DLL. The application uses portable startup defaults, a generic legacy version label and no serial-derived denylist fingerprint. No scores, libraries, fonts, personal settings or original executables are included.

SCOR4 differences are selected in Preferences. See the [technical manual](ScoreModern/TechnicalManual/README.md), [SCOR4 chapter](ScoreModern/TechnicalManual/07-scor4.md), [DLL guide](ScoreModern/TechnicalManual/06-dll-integration.md) and [API reference](ScoreModern/TechnicalManual/DLL-API.md).
