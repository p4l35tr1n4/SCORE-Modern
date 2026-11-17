param([string]$OutputPath)
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { throw 'The .NET Framework 4 compiler was not found.' }
$names = @('SelectorCopy','SelectorRoutine','BiosKeyReader','FormatterWordWriter','NumericConverter','ConsumerContextSaver','ParameterWordReader','DirectionalStringBlocks','SpanBookkeeping','PrefixWritePreparation','OutputCountAdvance','ScreenHookDecisions','ScreenHookForwarding','CharacterOutputDecisions','CharacterAlternateOutput','CharacterOutputFrame','CharacterOutputRoutine','StringOutputRoutine','DatTextLookup','PointBufferAppend','BufferedDrawControl','HorizontalRaster','VerticalRaster','SlopedRaster','EqualStepRaster','LinePreparation','LineClipping','SignedDivide16','ClippingCpu','RasterDispatch','LineDriverBody','RasterStatusFlags','CoordinateConversion','CoordinateWordClamp','CapacityMessageHandler','CapacityFollowup','CapacityDeviceCallees','CapacityKeyReader','CapacityStringOutput','CapacityFollowupNativeEffects','CapacityMessageNativeEffects','CapacityCharacterOutput','ScreenHookRoutine','ScreenHookInterrupt','Mode12Video','BufferedPointCaller','PackedDrawingWord','ItemDrawingDispatch','BeamBufferEdit','BufferedCoordinateDecode','BufferedBeamLinePipeline','BeamGeometry','TupletGeometry','TupletBracket','SlurOrigins','SlurSampling','SlurThickness','SlurPostprocess','SlurGeometry','SlurLineWidth','SlurVectorCommands','EndingBracket','PageCoordinates','PrintedGlyphDefinition')
$names += 'RetainedParameterStorage'
$names += 'TextMetricArithmetic'
$names += 'TextParameterPreparation'
$names += 'TextConverterFontState'
$names += 'PrimitiveTextGeometry'
$names += 'IncludedPostScriptVariables'
$names += 'SingleFixedDecimal'
$names += 'StaffHeightLayout'
$names += 'AutomaticPrintOffset'
$names += 'PrintRotation'
$names += 'IncludedEpsTransform'
$names += 'ScorlasFilePlacement'
$names += 'ScorlasGuideConversion'
$names += 'ScorlasStatisticsArithmetic'
$names += 'BeamStemAdjustment'
$names += @(Get-ChildItem -LiteralPath $PSScriptRoot -Filter 'Layout*.cs' -File | Where-Object { $_.BaseName -notlike '*Proof*' } | ForEach-Object { $_.BaseName })
$names += @(Get-ChildItem -LiteralPath $PSScriptRoot -Filter 'Input*.cs' -File | Where-Object { $_.BaseName -notlike '*Proof*' } | ForEach-Object { $_.BaseName })
$sources = @($names | ForEach-Object { Join-Path $PSScriptRoot ($_.ToString() + '.cs') })
foreach ($source in $sources) { if (-not (Test-Path -LiteralPath $source)) { throw "Missing fragment source: $source" } }
$target = if ($OutputPath) { [IO.Path]::GetFullPath($OutputPath) } else { Join-Path $PSScriptRoot 'ScoreModern.dll' }
$response = Join-Path (Split-Path -Parent $target) 'ScoreModern-fragments.rsp'
$arguments = @('/nologo', '/target:library', '/reference:System.Numerics.dll', "/out:`"$target`"")
$arguments += @($sources | ForEach-Object { '"' + $_ + '"' })
[IO.File]::WriteAllLines($response, [string[]]$arguments, [Text.Encoding]::UTF8)
try {
    & $compiler "@$response"
    if ($LASTEXITCODE -ne 0) { throw 'Fragment library compilation failed.' }
} finally {
    Remove-Item -LiteralPath $response
}
Write-Output "Built $target"








