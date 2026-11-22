# Using ScoreModern.dll from another program

This guide covers **ScoreModern.dll 0.3.61.2**, shared by current Core and SMX builds. The DLL is a Windows .NET Framework managed assembly. Its public namespace is `ScoreModern.RecoveredFragments`. Load it into your own process and call managed methods directly. There is no running SCORE Modern session to connect to, and the DLL does not provide an IPC endpoint, COM automation object, or native C API.

The DLL contains recovered calculation and state routines. The Windows executable supplies the application layer, including opening and saving a complete score, selecting external LIB and font resources, preparing a page, running commands, and drawing or exporting a score. Call the DLL for individual calculations or for a workflow whose full inputs you can supply. Use the executable's command line for complete file rendering and export.

## Set up a C# client

1. Extract `ScoreModern.dll` from the build you want to use into a folder for your project. Keep that exact binary with your application. The DLL reports assembly version `0.0.0.0`, so that metadata alone cannot identify the release or guarantee compatibility.
2. In a **Console App (.NET Framework)** project, use **Add Reference → Browse** and select the DLL. Set **Copy Local** to true so the loader can find it beside the client executable. Use a compatible .NET Framework installation on Windows. The release build was compiled with the .NET Framework v4 compiler and references `mscorlib`, `System`, `System.Core`, and `System.Numerics`.
3. Add `using ScoreModern.RecoveredFragments;` in C#. Configure application resources separately if the workflow calls for them. The examples below only need the managed DLL and the .NET Framework.

### Working example

Save this as `Example.cs` beside `ScoreModern.dll`. The first calculation prepares a staff and a note position. The second calls the J gap preparation helper directly on already selected intervals.

```csharp
using System;
using System.Globalization;
using ScoreModern.RecoveredFragments;

internal static class Example
{
    private static void Main()
    {
        var page = new PageCoordinates();

        // This helper uses a one-based parameter array. Index 0 is unused.
        var staff = new float[21];
        staff[1] = 8;       // Staff item code
        staff[2] = 8;       // Staff number
        staff[5] = 1;       // Staff size
        staff[6] = 200;     // Right edge in SCORE horizontal units
        PageStaffResult preparedStaff = page.SetStaff(staff, false);

        var note = new float[21];
        note[1] = 1;
        note[2] = 8;
        note[3] = 40;
        note[4] = 7;
        PageItemContext position = page.Prepare(note, 8, 1);

        Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
            "Staff bottom={0}, note X={1}, note Y={2}",
            preparedStaff.Bottom, position.X, position.Y));

        float[] positions = { 0f, 7f, 30f };
        float[] minima = { 10f, 10f };
        var slack = new float[2];
        JustificationGapResult gaps = LayoutJustificationGaps.Prepare(
            slack, minima, positions, 3, 0f, 30f);

        Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
            "Slack={0},{1}; positive={2}; negative magnitude={3}",
            slack[0], slack[1], gaps.PositiveSlack, gaps.NegativeSlack));
    }
}
```

To compile without an IDE, run these commands in PowerShell from the folder containing the two files. They use the same compiler family as the release build.

```powershell
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $compiler /nologo /target:exe /out:Example.exe /reference:ScoreModern.dll Example.cs
if ($LASTEXITCODE -ne 0) { throw "Example compilation failed." }
.\Example.exe
```

Expected output:

```text
Staff bottom=-1950, note X=6000, note Y=-1250
Slack=-3,13; positive=13; negative magnitude=3
```

`PageCoordinates` owns mutable staff tables. Prepare staff definitions before the items that use them. The example's coordinates are raw drawing units before preview, DOS raster, or EPS projection. A complete score needs the original traversal and per-code interpretation as well. The gap helper computes and stores slack. It can also modify the minimums if the requested width is too small. Redistribution and writing coordinates back to records are separate calls. See [coordinate spaces](01-coordinates.md) and [justification](02-spacing.md).

### Call a helper from Windows PowerShell

```powershell
Add-Type -Path (Resolve-Path .\ScoreModern.dll).Path
$parts = [ScoreModern.RecoveredFragments.PackedDrawingWord]::Decode([int16]12345)
$parts | Format-List High, Middle, Low
```

The fields are `1`, `23`, and `45`. They are decoded numeric parts. Their meaning depends on the consuming item code.

## Choose the entry point for your task

| Task | Managed entry points | What your application supplies |
| --- | --- | --- |
| Decode packed numeric controls | `PackedDrawingWord.Decode`, `FiniteBinary32Integer64.TryConvert` | Signed words or the original binary32 bits. The conversion helper's Boolean result tells you whether its finite projection applies. |
| Position a staff and item | `PageCoordinates.SetStaff`, `Prepare`, `Vertical` | Parameter arrays, staff order, and the caller's units and reference state. |
| Prepare justification gaps | `LayoutJustificationGaps.Prepare` | Positions, minimum widths, an output array, point count, and boundaries. The helper writes into the arrays you pass. |
| Run J or LI over prepared record storage | `LayoutJustificationSystem.Apply`, `LayoutLineupSystem.Apply` | A raw score word bank, address/index tables in the relevant routine's conventions, staff and preference context, and a workspace instance. Read the actual callers before constructing these arrays. |
| Generate beam geometry | `BeamGeometry.Build` | Packed Code 6 parameters and `BeamGeometryContext`, including the staff's vertical coordinate function. The result contains vertices in raw drawing units. Select printed or DOS appearance explicitly. |
| Stage staff input | `InputStaffTransaction` | Staff setup, parsed input and subsequent stages in order. The executable's interactive commands and text UI wrap this transaction. |
| Apply a transposition calculation | `InputTranspositionInterval.Convert`, `InputTranspositionPitch.Apply` | Interval, clef, keys and collected pitch context. Use the Main transposition caller for record traversal and relationships. |
| Convert text metrics | `TextMetricArithmetic.Prepare`, `Normalize`, `Advance`, `Accumulate` | Already resolved metric values, scale and spacing controls. The executable resolves actual font and PSC files. |
| Accumulate tiled page positions | `ScorlasGuideConversion.Convert`, `ScorlasFilePlacementState.Add` | Measured extents, guide and units. Retain one state object for each sequence of files. |
| Model original memory and device operations | Interfaces such as `IPointBufferMemory`, `IHorizontalRasterMemory`, `ICoordinateFpuEffects`, and `IMode12Surface` | A memory and device adapter, initial register state, required buffer sizes and any external effects named in the routine contract. |

Types and function signatures for the shipped binary are in the [assembly member index](DLL-API.md). The [technical routine index](B-routine-index.md) links to implementation listings and original recovery evidence. The [native integration listings](B-native-integration-listings.md) show how the executable supplies arguments and handles results.

## Input conventions and state ownership

- Pay attention to the indexing convention of each routine. The page helper in the example uses parameters at their one-based indices. Working array APIs elsewhere can use zero-based elements, one-based raw-word references, byte offsets or segmented addresses. Check the individual contract.
- Keep the original parameter precision. Use `float` for stored binary32 parameters and `short` for signed words where the signature calls for them. Preserve bit patterns and sentinels when comparing against an original fixture. Decimal rendering and apparent equality can hide a signed zero or a one-bit difference.
- Treat arrays, buffers and workspaces passed to a routine as caller-owned mutable data unless its source specifies otherwise. Make a copy when testing an alternative. J, for example, may compress its minimum array. Keep a staff table or tiling state for the intended document or sequence. Use independent state for independent work and avoid sharing mutable state across threads.
- Use the exact version and resource set for a reproducible result. Some helpers support only the measured parameter or buffer domain and report unsupported cases with managed exceptions. Handle an exception as a failed calculation. A lower-level operation need not roll back earlier writes.
- Native clients can place a managed wrapper or a separate application around the assembly. Calling it through `LoadLibrary` and `GetProcAddress`, C# `DllImport`, or Python `ctypes` cannot expose these managed methods as ordinary C exports.

## Render or export a complete score

For application-level tasks, run the matching executable as a subprocess. Keep `ScoreModern.dll` beside `ScoreModern.exe`. Supply an actual SCORE installation root for file commands and the resources required by the score. The working directory affects relative paths, so absolute paths are preferable in automation.

```powershell
$exe = 'C:\ScoreModern\ScoreModern.exe'

# PNG preview: MUS file, output PNG, SCORE installation root
$render = Start-Process -FilePath $exe -ArgumentList @(
    '--render', '"C:\Scores\page.mus"', '"C:\Scores\page.png"', '"C:\SCORE"'
) -WindowStyle Hidden -Wait -PassThru
if ($render.ExitCode -ne 0) { throw "Preview failed: $($render.ExitCode)" }

# EPS: SCORE installation root, MUS file, output EPS
$export = Start-Process -FilePath $exe -ArgumentList @(
    '--export-eps', '"C:\SCORE"', '"C:\Scores\page.mus"', '"C:\Scores\page.eps"'
) -WindowStyle Hidden -Wait -PassThru
if ($export.ExitCode -ne 0) { throw "EPS export failed: $($export.ExitCode)" }
```

These entry points are implemented by the executable in [its command-line dispatcher](../ScoreModern.cs) and [EPS export](../NativeEpsExport.cs). The EPS command also accepts `--print-settings=PATH` and `--type1-fonts=PATH`. For a font folder with spaces, add an argument such as `'--type1-fonts="C:\SCORE Fonts"'` after the output path in the EPS example. Use an output path separate from the input file, and inspect the exit code and output before treating a job as complete. For a diagnostic run, capture the subprocess output streams. The console examples above use placeholder paths.

## Diagnose common integration problems

| Symptom | Check |
| --- | --- |
| C# cannot resolve `ScoreModern.RecoveredFragments` | Confirm your project references the DLL from the release package. |
| Assembly load fails when the client is deployed | Copy the same DLL beside the client and check its target runtime and managed framework dependencies. |
| Expected method is missing | Compare the local binary with the pinned release and its member index. Assembly version `0.0.0.0` does not distinguish builds. |
| A geometry call throws `NotSupportedException` | Check the helper's verified domain, indexing, units, array extents and original branch assumptions. |
| The result looks scaled or flipped | Check whether the result is in parameter, raw drawing, raster, native canvas or PostScript space before applying a transform. |
| A standalone export reports a missing resource | Check the installation root, selected LIB and matching font files or external drawings for the score. |


## SCOR4 version controls

See [Chapter 07](07-scor4.md#shared-dll-scor4-engraving-and-spacing-controls-20-november-2026) for exact meanings, units and rounding. Version selection is explicit and defaults to SCORE 3.11:

```csharp
var context = new SlurOriginContext { Scor4 = true, Scale = 22.5f,
    LeftX = 4500, LeftY = -18802.5f, RightY = -18802.5f };
var workspace = new LayoutJustificationWorkspace {
    Scor4NumberJustification = true
};
float rawDrawingOffset = ScoreVersionControls.DrawingOffset(true, 2f); // 300
short meterWidthSelector = ScoreVersionControls.MeterThicknessSelector(true, -2f);
```

Existing public selector/spacing signatures remain as classic overloads. The new explicit overloads opt into SCOR4. Supply the same version to geometry, bounds and layout; do not infer it from an ordinary MUS trailer. DLL file and informational version are 0.3.61.2; assembly version remains 0.0.0.0. The EXE supplies resource resolution, print state restoration and UI. Build both applications using `-CoreDllPath` with the same canonical binary.


For command-line EPS, select the version explicitly with `--score-profile=scor4` or `--score-profile=311`. If omitted, the saved interface preference is used. For example:

```powershell
.\ScoreModernX.exe --export-eps C:\SCORE4 page.MUS page.EPS --score-profile=scor4 --notation=vector --type1-fonts=C:\SCORE4\PSFONTS
```

Core uses `ScoreModern.exe` with the same arguments. These flags select process-local rendering semantics and do not rewrite the saved profile or score.


## Parts batch: additive DLL 0.3.61.2 APIs

`InputChromaticPitchCorrection.Apply(IList<float[]> before, IList<float[]> after, short staff, short semitones)` is an explicit modern correction after native tonal processing. Array index 0 is Code/P1. It requires preserved row correspondence and standard clefs/conventional note accidentals; it changes output rows only and returns per-note diatonic corrections for attachment repair. Native original-route callers do not opt in. Explicit custom key glyphs retain their positions and spacing hint; note spelling is corrected against their sounding context.

`InputMeterDuration.TryQuarterBeats(float numerator, float denominator, float secondNumerator, float secondDenominator, out float beats)` calculates logical quarter-note duration. It supports numeric, common/cut and composite signatures. A shared second denominator is selected by zero. It returns false for nonfinite/invalid inputs and single-number graphics without a beat unit. No UI, file I/O or global preference state is involved.

The prior 1,548 public declarations remain. Four declarations are added (two types and two methods). Both fragment source trees produce the same 1,552 public declarations. Application-layer Parts state, resource transactions and SMX slur continuation remain outside the DLL.
