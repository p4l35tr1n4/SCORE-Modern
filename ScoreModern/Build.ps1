param([string]$OutputDirectory, [ValidatePattern("^[a-z]?$")][string]$VersionSuffix = "", [ValidatePattern("^\d*$")][string]$NativeBuild = "", [string]$CoreDllPath = "")
$ErrorActionPreference = 'Stop'
$buildOutput = if ($OutputDirectory) { [IO.Path]::GetFullPath($OutputDirectory) } else { $PSScriptRoot }
if ($VersionSuffix -and $buildOutput.TrimEnd([IO.Path]::DirectorySeparatorChar) -eq ([IO.Path]::GetFullPath($PSScriptRoot)).TrimEnd([IO.Path]::DirectorySeparatorChar)) { throw "Temporary versions require a separate output directory." }
New-Item -ItemType Directory -Path $buildOutput -Force | Out-Null
$versionSource = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'BuildVersion.cs') -Raw
if ($versionSource -notmatch 'const string Version="(\d+\.\d+\.\d+)"') { throw 'Missing three-part build version.' }
$releaseVersion = $Matches[1]
$manifestVersion = $releaseVersion + ".0"
$compileVersionPath = $null
if ($VersionSuffix -or $NativeBuild) {
    $temporaryVersion = $versionSource.Replace('NativeBuild=""', ('NativeBuild="' + $NativeBuild + '"'))
    if ($VersionSuffix) {
        $revision = [int][char]$VersionSuffix - [int][char]"a" + 1
        $manifestVersion = $releaseVersion + "." + $revision
        $temporaryVersion = $temporaryVersion.Replace(('Version="' + $releaseVersion + '"'), ('Version="' + $releaseVersion + $VersionSuffix + '"'))
        $temporaryVersion = $temporaryVersion.Replace('FileVersion=Version+".0"', ('FileVersion="' + $manifestVersion + '"'))
    }
    $compileVersionPath = Join-Path $buildOutput "BuildVersion.Temporary.cs"
    [IO.File]::WriteAllText($compileVersionPath, $temporaryVersion, (New-Object Text.UTF8Encoding($false)))
}
$manifestPath = Join-Path $PSScriptRoot 'ScoreModern.manifest'
$manifestText = Get-Content -LiteralPath $manifestPath -Raw
$manifestText = $manifestText -replace '(assemblyIdentity version=")[^"]+', ('${1}' + $manifestVersion)
[xml]$validatedManifest = $manifestText
if ($buildOutput -ne $PSScriptRoot) { $manifestPath = Join-Path $buildOutput "ScoreModern.manifest" }
[IO.File]::WriteAllText($manifestPath, $manifestText)

$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if ($CoreDllPath) {
    $sharedDll = [IO.Path]::GetFullPath($CoreDllPath)
    if (-not (Test-Path -LiteralPath $sharedDll -PathType Leaf)) { throw 'Shared core DLL not found.' }
    if ($sharedDll -ne [IO.Path]::GetFullPath((Join-Path $buildOutput 'ScoreModern.dll'))) { Copy-Item -LiteralPath $sharedDll -Destination (Join-Path $buildOutput 'ScoreModern.dll') -Force }
} else {
    & (Join-Path $PSScriptRoot '..\ScoreReverseEngineering\NativeFragments\Build-Fragments.ps1') -OutputPath (Join-Path $buildOutput 'ScoreModern.dll')
}
$sourceFiles = Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.cs' -File
$strictUtf8 = New-Object System.Text.UTF8Encoding($false, $true)
foreach ($sourceFile in $sourceFiles) { [void]$strictUtf8.GetString([IO.File]::ReadAllBytes($sourceFile.FullName)) }
$compileSources = @($sourceFiles.FullName)
if ($compileVersionPath) { $compileSources = @($sourceFiles | Where-Object Name -ne "BuildVersion.cs" | ForEach-Object FullName) + $compileVersionPath }
$programIcon = Join-Path $PSScriptRoot 'Assets\SM-icon.ico'
$fileIcon = Join-Path $PSScriptRoot 'Assets\SM-file-icon.ico'
$titleLogo = Join-Path $PSScriptRoot 'Assets\TitleLogo.png'
$response = Join-Path $buildOutput 'ScoreModern-app.rsp'
$arguments = @(
    '/nologo', '/codepage:65001', '/target:winexe', '/optimize+'
    "/out:`"$buildOutput\ScoreModern.App.exe`""
    "/win32manifest:`"$manifestPath`""
    "/win32icon:`"$programIcon`""
    "/resource:`"$programIcon`",ScoreModern.Program.ico"
    "/resource:`"$fileIcon`",ScoreModern.Document.ico"
    "/resource:`"$titleLogo`",ScoreModern.TitleLogo.png"
    '/reference:System.Windows.Forms.dll', '/reference:System.Drawing.dll'
    "/reference:`"$buildOutput\ScoreModern.dll`""
)
$arguments += @($compileSources | ForEach-Object { '"' + $_ + '"' })
[IO.File]::WriteAllLines($response, [string[]]$arguments, [Text.Encoding]::UTF8)
try {
    & $compiler "@$response"
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
} finally {
    Remove-Item -LiteralPath $response
}
# Keep the managed entry assembly identity distinct from ScoreModern.dll.
Move-Item -LiteralPath (Join-Path $buildOutput 'ScoreModern.App.exe') -Destination (Join-Path $buildOutput 'ScoreModern.exe') -Force
$helpOutput = Join-Path $buildOutput 'Help'
New-Item -ItemType Directory -Path $helpOutput -Force | Out-Null
Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'Help/Topics') -Filter '*.md' -File |
    Copy-Item -Destination $helpOutput -Force
Write-Host "Built $buildOutput\ScoreModern.exe"
