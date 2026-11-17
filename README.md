# SCORE Modern

SCORE Modern is a Windows application for working with legacy SCORE music engraving files. This repository contains the Core 0.3.60 RC2 source snapshot, based on build 2111.

## Build on Windows

Use Windows PowerShell and the .NET Framework 4.8 compiler installed with Windows. From the repository root, run:

```powershell
$output = Join-Path (Get-Location) 'build'
& .\ScoreModern\Build.ps1 -OutputDirectory $output -NativeBuild 2111
```

The build creates `ScoreModern.exe`, `ScoreModern.dll`, and a `Help` directory. Keep them together. First setup asks for the location of an existing SCORE installation and its resources. SCORE executables, scores, libraries, fonts, and local preferences are external.

`ScoreModern/` contains the Windows application. `ScoreReverseEngineering/NativeFragments/` contains the recovered routines built into the managed DLL. The bundled help topics are under [ScoreModern/Help/Topics](ScoreModern/Help/Topics).

For integration with other software, see the [DLL integration guide](ScoreModern/TechnicalManual/06-dll-integration.md) and [managed API reference](ScoreModern/TechnicalManual/DLL-API.md).

The publication build uses portable startup defaults and a generic legacy version label. Startup still requires a compatible serial-bearing DAT supplied by the user.

The [SCOR4 technical chapter](ScoreModern/TechnicalManual/07-scor4.md) covers the recovered SCORE 4.01 differences.
