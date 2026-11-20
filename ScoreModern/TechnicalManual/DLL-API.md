# ScoreModern.dll public member index

Generated from shared DLL file version 0.3.61.1. Old spacing overloads retain SCORE 3.11 defaults; explicit SCOR4 flags opt into version-specific behavior. See Chapter 07 for semantics, units and precision.

Assembly: ScoreModern, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

Exported types: 338

## ScoreModern.RecoveredFragments.AutomaticPrintOffset

```text
static System.Int32 FromMinimumWord(System.Int16 minimumY)
static System.Int32 SelectBottom(System.Int32 preliminary)
```

## ScoreModern.RecoveredFragments.BeamAppearance

```text
static ScoreModern.RecoveredFragments.BeamAppearance DosScreen = 1
static ScoreModern.RecoveredFragments.BeamAppearance Printed = 0
System.Int32 value__
```

## ScoreModern.RecoveredFragments.BeamBufferEdit

```text
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IPointBufferMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.BeamGeometry

```text
static System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.BeamShape] Build(System.Single[] p, ScoreModern.RecoveredFragments.BeamGeometryContext c, ScoreModern.RecoveredFragments.BeamAppearance appearance)
static System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.BeamShape] Build(System.Single[] p, ScoreModern.RecoveredFragments.BeamGeometryContext c, ScoreModern.RecoveredFragments.BeamAppearance appearance, ScoreModern.RecoveredFragments.BeamReference& reference)
```

## ScoreModern.RecoveredFragments.BeamGeometryContext

```text
System.Single LeftVertical
System.Single Scale
System.Func`2[System.Single,System.Single] Vertical
BeamGeometryContext()
```

## ScoreModern.RecoveredFragments.BeamReference

```text
System.Boolean Cue
System.Single Dx
System.Single Dy
System.Single LeftX
System.Single LeftY
System.Single StaffScale
```

## ScoreModern.RecoveredFragments.BeamShape

```text
System.Int32 Group
ScoreModern.RecoveredFragments.BeamVertex[] Vertices
BeamShape()
```

## ScoreModern.RecoveredFragments.BeamStemAdjustment

```text
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Boolean force, ScoreModern.RecoveredFragments.PageCoordinates tables, System.Int32 maximumStaff, System.Int32 onlyBeam = -1, System.Collections.Generic.ISet`1[System.Int32] selectedBeams = null)
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Boolean force, ScoreModern.RecoveredFragments.PageCoordinates tables, System.Int32 maximumStaff, System.Int32 onlyBeam, System.Collections.Generic.ISet`1[System.Int32] selectedBeams, System.Boolean page4)
static System.Collections.Generic.List`1[System.Single[]] ApplyScor4Selected(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 selected, System.Boolean french, System.Boolean includeOpposite, System.Int32 onlyNote = -1)
static System.Collections.Generic.List`1[System.Single[]] ConfigureScor4Selected(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 selected, System.Boolean includeOpposite)
```

## ScoreModern.RecoveredFragments.BeamVertex

```text
System.Single X
System.Single Y
BeamVertex(System.Single x, System.Single y)
```

## ScoreModern.RecoveredFragments.BiosKeyReader

```text
static System.Void Execute(ScoreModern.RecoveredFragments.IBiosKeyContext context, System.UInt16 asciiSegment, System.UInt16 asciiOffset, System.UInt16 scanSegment, System.UInt16 scanOffset)
```

## ScoreModern.RecoveredFragments.BufferedBeamLinePipeline

```text
static ScoreModern.RecoveredFragments.LineDriverBodyResult Execute(ScoreModern.RecoveredFragments.IByteOrResultMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state, System.UInt16 driverSegment)
```

## ScoreModern.RecoveredFragments.BufferedCoordinateDecode

```text
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IPointBufferMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.BufferedDrawControl

```text
static ScoreModern.RecoveredFragments.BufferedDrawWords DecodeMarker(ScoreModern.RecoveredFragments.IBufferedDrawMemory m, System.UInt16 ds, System.UInt16 ax)
static ScoreModern.RecoveredFragments.BufferedDrawWords SelectRasterPath(ScoreModern.RecoveredFragments.IBufferedDrawMemory m, System.UInt16 ds)
static ScoreModern.RecoveredFragments.BufferedDrawWords UpdatePosition(ScoreModern.RecoveredFragments.IBufferedDrawMemory m, System.UInt16 ds, System.UInt16 ax)
```

## ScoreModern.RecoveredFragments.BufferedDrawWords

```text
System.UInt16 AX
System.Boolean Branch
System.UInt16 CX
```

## ScoreModern.RecoveredFragments.BufferedPointCaller

```text
static ScoreModern.RecoveredFragments.BufferedPointCallerResult Enter(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 converterSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteCapacityAtComparison(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteCapacityAtComparisonThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 converterSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteWithCapacityEffects(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects fpu, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 converterSegment, System.UInt16 stringSegment, System.UInt16 followupSegment)
```

## ScoreModern.RecoveredFragments.BufferedPointCallerResult

```text
ScoreModern.RecoveredFragments.BufferedPointOutcome Outcome
System.UInt16 RequestedCalleeOffset
System.Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.BufferedPointOutcome

```text
static ScoreModern.RecoveredFragments.BufferedPointOutcome CapacityHandlerRequired = 1
static ScoreModern.RecoveredFragments.BufferedPointOutcome Completed = 0
static ScoreModern.RecoveredFragments.BufferedPointOutcome ExternalControlTransfer = 3
static ScoreModern.RecoveredFragments.BufferedPointOutcome NormalPathReady = 2
System.Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityCallKind

```text
static ScoreModern.RecoveredFragments.CapacityCallKind Followup = 1
static ScoreModern.RecoveredFragments.CapacityCallKind StringOutput = 0
System.Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityCharacterNativeEffects

```text
CapacityCharacterNativeEffects(ScoreModern.RecoveredFragments.ICapacityInterruptEffects effects)
ScoreModern.RecoveredFragments.SignedDivide16State CallCharacter(ScoreModern.RecoveredFragments.ICapacityStringMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.CapacityCharacterOutput

```text
static ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.CapacityDeviceCallees

```text
static ScoreModern.RecoveredFragments.CapacityDeviceResult KeyStatus(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult KeyStatusThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult Poll(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult PollThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.CapacityDeviceOutcome

```text
static ScoreModern.RecoveredFragments.CapacityDeviceOutcome ExternalControlTransfer = 1
static ScoreModern.RecoveredFragments.CapacityDeviceOutcome ReachedReturn = 0
System.Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityDeviceResult

```text
ScoreModern.RecoveredFragments.CapacityDeviceOutcome Outcome
System.Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.CapacityFollowup

```text
static ScoreModern.RecoveredFragments.CapacityFollowupResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityFollowupEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 calleeSegment)
static ScoreModern.RecoveredFragments.CapacityFollowupResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityFollowupEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 calleeSegment)
```

## ScoreModern.RecoveredFragments.CapacityFollowupCallKind

```text
static ScoreModern.RecoveredFragments.CapacityFollowupCallKind KeyRead = 2
static ScoreModern.RecoveredFragments.CapacityFollowupCallKind KeyStatus = 0
static ScoreModern.RecoveredFragments.CapacityFollowupCallKind Poll = 3
static ScoreModern.RecoveredFragments.CapacityFollowupCallKind PromptOutput = 1
System.Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityFollowupNativeEffects

```text
CapacityFollowupNativeEffects(ScoreModern.RecoveredFragments.ICapacityInterruptEffects interruptEffects, ScoreModern.RecoveredFragments.ICapacityCharacterEffects characterEffects, System.UInt16 characterTargetSegment)
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityFollowupCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.CapacityFollowupOutcome

```text
static ScoreModern.RecoveredFragments.CapacityFollowupOutcome ExternalControlTransfer = 1
static ScoreModern.RecoveredFragments.CapacityFollowupOutcome ReachedReturn = 0
static ScoreModern.RecoveredFragments.CapacityFollowupOutcome UnverifiedBranch = 2
System.Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityFollowupResult

```text
ScoreModern.RecoveredFragments.CapacityFollowupOutcome Outcome
System.Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.CapacityKeyReader

```text
static ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityKeyMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityKeyMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.CapacityMessageHandler

```text
static ScoreModern.RecoveredFragments.CapacityMessageResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
static ScoreModern.RecoveredFragments.CapacityMessageResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
```

## ScoreModern.RecoveredFragments.CapacityMessageNativeEffects

```text
CapacityMessageNativeEffects(ScoreModern.RecoveredFragments.ICapacityInterruptEffects interruptEffects, ScoreModern.RecoveredFragments.ICapacityCharacterEffects characterEffects, System.UInt16 sharedCalleeSegment, System.UInt16 characterTargetSegment)
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
static ScoreModern.RecoveredFragments.CapacityMessageNativeEffects CreateWithNativeCharacters(ScoreModern.RecoveredFragments.ICapacityInterruptEffects interruptEffects, System.UInt16 sharedCalleeSegment, System.UInt16 characterTargetSegment)
```

## ScoreModern.RecoveredFragments.CapacityMessageOutcome

```text
static ScoreModern.RecoveredFragments.CapacityMessageOutcome ExternalControlTransfer = 1
static ScoreModern.RecoveredFragments.CapacityMessageOutcome ReachedReturn = 0
System.Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityMessageResult

```text
ScoreModern.RecoveredFragments.CapacityMessageOutcome Outcome
System.Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.CapacityStringOutput

```text
static ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityCharacterEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 characterSegment)
static ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityCharacterEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 characterSegment)
```

## ScoreModern.RecoveredFragments.CharacterAlternateOutput

```text
static ScoreModern.RecoveredFragments.CharacterBiosWords Run(ScoreModern.RecoveredFragments.ICharacterBiosContext context, ScoreModern.RecoveredFragments.CharacterBiosWords w)
```

## ScoreModern.RecoveredFragments.CharacterBiosWords

```text
System.UInt16 AX
System.UInt16 BX
System.UInt16 CX
System.UInt16 DX
System.UInt16 SP
System.UInt16 SS
```

## ScoreModern.RecoveredFragments.CharacterDecisionResult

```text
System.Byte AL
System.Boolean Branch
System.UInt16 BX
CharacterDecisionResult(System.Byte al, System.UInt16 bx, System.Boolean branch)
```

## ScoreModern.RecoveredFragments.CharacterEntryWords

```text
System.UInt16 BP
System.UInt16 DI
System.UInt16 ES
System.UInt16 SP
```

## ScoreModern.RecoveredFragments.CharacterOutputDecisions

```text
static ScoreModern.RecoveredFragments.CharacterDecisionResult ProbeAlternateGate(ScoreModern.RecoveredFragments.ICharacterDecisionMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 di, System.UInt16 bx)
static ScoreModern.RecoveredFragments.CharacterDecisionResult ProbeBackspace(ScoreModern.RecoveredFragments.ICharacterDecisionMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 di, System.Byte dl, System.UInt16 bx)
```

## ScoreModern.RecoveredFragments.CharacterOutputFrame

```text
static ScoreModern.RecoveredFragments.CharacterEntryWords Enter(ScoreModern.RecoveredFragments.ICharacterFrameMemory m, System.UInt16 ss, System.UInt16 sp, System.UInt16 bp, System.UInt16 di)
static ScoreModern.RecoveredFragments.CharacterReturnWords Leave(ScoreModern.RecoveredFragments.ICharacterFrameMemory m, System.UInt16 ss, System.UInt16 sp)
```

## ScoreModern.RecoveredFragments.CharacterOutputRoutine

```text
static ScoreModern.RecoveredFragments.CharacterRoutineResult Run(ScoreModern.RecoveredFragments.ICharacterRoutineContext m, ScoreModern.RecoveredFragments.CharacterRoutineWords w)
```

## ScoreModern.RecoveredFragments.CharacterReturnWords

```text
System.UInt16 BP
System.UInt16 CS
System.UInt16 DI
System.UInt16 IP
System.UInt16 SP
```

## ScoreModern.RecoveredFragments.CharacterRoutineResult

```text
System.UInt16 CS
System.UInt16 IP
ScoreModern.RecoveredFragments.CharacterRoutineWords Words
```

## ScoreModern.RecoveredFragments.CharacterRoutineWords

```text
System.UInt16 AX
System.UInt16 BP
System.UInt16 BX
System.UInt16 CX
System.UInt16 DI
System.UInt16 DS
System.UInt16 DX
System.UInt16 ES
System.UInt16 SI
System.UInt16 SP
System.UInt16 SS
```

## ScoreModern.RecoveredFragments.ClippingCpu

```text
static ScoreModern.RecoveredFragments.SignedDivide16Result Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.ConsumerContextSaver

```text
static System.UInt16 Execute(ScoreModern.RecoveredFragments.IConsumerContextMemory m, System.UInt16 stackSegment, System.UInt16 entrySp, System.UInt16 entryBp, System.UInt16 entrySi, System.UInt16 entryDi, System.UInt16 entryDs)
```

## ScoreModern.RecoveredFragments.CoordinateConversion

```text
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICoordinateFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.SignedDivide16State ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICoordinateFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.CoordinateWordClamp

```text
static System.UInt32 Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt32 flags)
```

## ScoreModern.RecoveredFragments.DatTextLookup

```text
static ScoreModern.RecoveredFragments.DatTextLookupWords Read(ScoreModern.RecoveredFragments.IDatTextLookupMemory m, System.UInt16 ds)
```

## ScoreModern.RecoveredFragments.DatTextLookupWords

```text
System.UInt16 AX
System.UInt16 BX
System.UInt16 DX
```

## ScoreModern.RecoveredFragments.DirectionalStringBlocks

```text
static ScoreModern.RecoveredFragments.StringCopyResult Copy(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds, System.UInt16 si, System.UInt16 es, System.UInt16 di, System.UInt16 count, System.Boolean directionFlag)
static ScoreModern.RecoveredFragments.StringFillResult Fill(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 es, System.UInt16 di, System.UInt16 ax, System.UInt16 count, System.Boolean directionFlag)
```

## ScoreModern.RecoveredFragments.EndingBracket

```text
static ScoreModern.RecoveredFragments.EndingBracketResult Build(System.Single[] p, ScoreModern.RecoveredFragments.TupletBracketContext c, System.Boolean printed, System.Boolean tomsTenth)
static ScoreModern.RecoveredFragments.TupletBracketPoint DotPoint(System.Single x, System.Single y, System.Single scale, ScoreModern.RecoveredFragments.EndingBracketResult r)
```

## ScoreModern.RecoveredFragments.EndingBracketResult

```text
System.Single DotX
System.Single DotY
System.Boolean HasDot
System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TupletBracketPoint] Points
EndingBracketResult()
```

## ScoreModern.RecoveredFragments.EqualStepRaster

```text
static ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx, System.UInt16 si)
static ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx, System.UInt16 si, System.UInt32 eflags)
```

## ScoreModern.RecoveredFragments.ExternalVgaFont

```text
ExternalVgaFont(System.Byte[] data)
System.Byte Row(System.Byte character, System.Int32 line)
```

## ScoreModern.RecoveredFragments.FiniteBinary32Integer64

```text
static System.Boolean TryConvert(System.UInt32 bits, System.Int64& value)
```

## ScoreModern.RecoveredFragments.FormatterWordWriter

```text
static System.Void Execute(ScoreModern.RecoveredFragments.IFormatterWordContext context, System.UInt16 low, System.UInt16 high)
```

## ScoreModern.RecoveredFragments.HorizontalRaster

```text
static ScoreModern.RecoveredFragments.HorizontalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx)
static ScoreModern.RecoveredFragments.HorizontalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx, System.UInt32 eflags)
```

## ScoreModern.RecoveredFragments.HorizontalRasterWords

```text
System.UInt16 AX
System.UInt16 BX
System.UInt16 CX
System.UInt32 DefinedEFlagsMask
System.UInt16 DI
System.UInt16 DX
System.UInt32 EFlags
System.UInt16 SI
```

## ScoreModern.RecoveredFragments.IBiosKeyContext

```text
System.UInt16 ReadBiosKey()
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
```

## ScoreModern.RecoveredFragments.IBufferedDrawMemory

```text
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IBufferedPointFpuEffects

```text
System.Void LoadBinary32(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.IByteOrResultMemory

```text
System.Byte OrByteAndGetResult(System.UInt16 segment, System.UInt16 offset, System.Byte mask)
```

## ScoreModern.RecoveredFragments.ICapacityCallEffects

```text
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.ICapacityCharacterEffects

```text
ScoreModern.RecoveredFragments.SignedDivide16State CallCharacter(ScoreModern.RecoveredFragments.ICapacityStringMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.ICapacityFollowupEffects

```text
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityFollowupCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.ICapacityInterruptEffects

```text
ScoreModern.RecoveredFragments.SignedDivide16State Interrupt(System.Byte number, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.ICapacityKeyMemory

```text
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
```

## ScoreModern.RecoveredFragments.ICapacityStringMemory

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.ICharacterBiosContext

```text
System.Void Interrupt10(ScoreModern.RecoveredFragments.CharacterBiosWords& words)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.ICharacterDecisionMemory

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.ICharacterFrameMemory

```text
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.ICharacterRoutineContext

```text
System.Void Interrupt10(ScoreModern.RecoveredFragments.CharacterRoutineWords& words, System.UInt16 returnOffset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.IConsumerContextMemory

```text
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.ICoordinateFpuEffects

```text
System.Void LoadControlWord(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
System.Void StoreControlWord(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
System.Void StoreInteger64AndPop(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
System.Void Wait()
```

## ScoreModern.RecoveredFragments.IDatTextLookupMemory

```text
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.IFormatterWordContext

```text
System.UInt16 ReadWord(System.UInt16 offset)
System.Void WriteByte(System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IHorizontalRasterMemory

```text
System.Void OrByte(System.UInt16 segment, System.UInt16 offset, System.Byte mask)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IMode12GlyphSource

```text
System.Byte Row(System.Byte character, System.Int32 line)
```

## ScoreModern.RecoveredFragments.IMode12Surface

```text
System.Void Scroll(System.Int32 top, System.Int32 left, System.Int32 bottom, System.Int32 right, System.Int32 lines, System.Byte background)
System.Void WriteGlyph(System.Int32 row, System.Int32 column, System.Byte character, System.Byte color)
```

## ScoreModern.RecoveredFragments.IncludedEpsTransform

```text
System.Single BoxX
System.Single BoxY
System.Boolean Rotate
System.Single Rotation
System.Single ScaleX
System.Single ScaleY
System.Single X
System.Single Y
IncludedEpsTransform()
static ScoreModern.RecoveredFragments.IncludedEpsTransform Prepare(ScoreModern.RecoveredFragments.PageItemContext page, System.Single staffBase, System.Single vertical, System.Single horizontalSize, System.Single verticalSize, System.Single boxLeft, System.Single boxBottom, System.Single rotation, System.Single printSize, System.Int32 leftOffset, System.Int32 bottomOffset)
```

## ScoreModern.RecoveredFragments.IncludedPostScriptVariables

```text
System.Single BottomOffset
System.Single GlobalSize
System.Single LeftOffset
System.Single LineWidth
System.Single PointsLevel
System.Single PP3
System.Single PP4
System.Single PP5
System.Single PP6
System.Single StaffLevel
System.Single StaffSize
IncludedPostScriptVariables()
static ScoreModern.RecoveredFragments.IncludedPostScriptVariables Prepare(ScoreModern.RecoveredFragments.PageItemContext item, System.Single staffLevel, System.Single p5, System.Single p6, System.Single printSize, System.Single lineWidth, System.Int32 leftOffset, System.Int32 bottomOffset)
```

## ScoreModern.RecoveredFragments.InputAttachedMarks

```text
static System.Single DefaultValue(System.Int32 mark, System.Single pitch, System.Single stem)
```

## ScoreModern.RecoveredFragments.InputAutomaticBeamMeterException

```text
InputAutomaticBeamMeterException(System.Int32 top, System.Int32 bottom)
```

## ScoreModern.RecoveredFragments.InputAutomaticBeams

```text
static System.Single[] Durations(System.Collections.Generic.IList`1[System.Single[]] rows)
static System.Single[] Groups(System.Collections.Generic.IList`1[System.Single[]] rows, System.Single[] durations, System.Single groupControl, System.Int32 skip)
```

## ScoreModern.RecoveredFragments.InputBeamConstructionResult

```text
System.Int32[] ForcedStems
System.Collections.Generic.List`1[System.Single[]] Records
InputBeamConstructionResult()
```

## ScoreModern.RecoveredFragments.InputBeamFit

```text
static ScoreModern.RecoveredFragments.InputBeamFitResult Fit(System.Single[] pitch, System.Single[] x, System.Int32 direction, System.Int32 flags, System.Single slopeLimit)
```

## ScoreModern.RecoveredFragments.InputBeamFitResult

```text
System.Int32 Direction
System.Single End
System.Single Start
System.Single UnquantizedEnd
System.Single UnquantizedStart
InputBeamFitResult()
```

## ScoreModern.RecoveredFragments.InputBeamLevels

```text
static System.Collections.Generic.List`1[System.Single[]] Pack(System.Single[] prototype, System.Int32[] original, System.Single[] x)
```

## ScoreModern.RecoveredFragments.InputBeamStemCompletion

```text
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] items, System.Int32 firstItem, ScoreModern.RecoveredFragments.InputEndpointStaffContext context = null)
```

## ScoreModern.RecoveredFragments.InputChordDots

```text
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows)
static System.Void ApplyRange(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int32 first, System.Int32 countLimit)
```

## ScoreModern.RecoveredFragments.InputChordLayout

```text
static System.Int32 Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int32 start, System.Int32 end)
```

## ScoreModern.RecoveredFragments.InputChordPitchRecords

```text
static ScoreModern.RecoveredFragments.InputPitchResult CompileSpacedNaturals(System.Single[] source, System.Single staff, System.Single left, System.Single right)
```

## ScoreModern.RecoveredFragments.InputChordRhythmRecords

```text
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Single[] encoded, System.Single left, System.Single right, System.Single scale, System.Int32[] stems)
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Single[] encoded, System.Single left, System.Single right, System.Single scale, System.Int32[] stems, System.Boolean proportional, System.Single exponent, System.Nullable`1[System.Single] referenceStart = null, System.Nullable`1[System.Int16] retainedGraceWord = null, System.Nullable`1[System.Single] rememberedLeft = null)
```

## ScoreModern.RecoveredFragments.InputDynamics

```text
static System.Single[] Create(System.Int32 symbol, System.Single[] note, System.Single staff, System.Single preference)
```

## ScoreModern.RecoveredFragments.InputEditInverse

```text
static System.String Number(System.Single n)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted = False)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted, System.Boolean documentedRepeatedMarks)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted, System.Boolean documentedRepeatedMarks, System.Boolean reproduceInvalid)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted, System.Boolean documentedRepeatedMarks, System.Boolean reproduceInvalid, System.Nullable`1[System.Single] verifiedTrailingWord)
```

## ScoreModern.RecoveredFragments.InputEditInverseResult

```text
System.String Header
System.Int32 InitialClef
System.Single Left
System.String[] Lines
System.String Positions
System.Collections.Generic.List`1[System.Single[]] Prefix
System.Single Right
System.Collections.Generic.List`1[System.Single[]] Selected
System.Int32 Staff
InputEditInverseResult()
```

## ScoreModern.RecoveredFragments.InputEndpointProjection

```text
static ScoreModern.RecoveredFragments.InputEndpointResult Decode(System.Single first, System.Single last, System.Int32 stage, System.Collections.Generic.IList`1[System.Single[]] records, System.Int32[] noteToItem, System.Int32[] stems, System.Single beginning, System.Single end, ScoreModern.RecoveredFragments.InputEndpointStaffContext context = null)
```

## ScoreModern.RecoveredFragments.InputEndpointRepromptException

```text
InputEndpointRepromptException()
```

## ScoreModern.RecoveredFragments.InputEndpointResult

```text
System.Int16 EndFlag
System.Int16 EndItem
System.Int16 EndNote
System.Single EndPitch
System.Single EndX
System.Int16 Number
System.Int16 StartFlag
System.Int16 StartItem
System.Int16 StartNote
System.Single StartPitch
System.Single StartX
System.Int16 Stem
InputEndpointResult()
```

## ScoreModern.RecoveredFragments.InputEndpointStaffContext

```text
InputEndpointStaffContext(System.Single[] bases, System.Single[] scales, System.Int32 staff)
InputEndpointStaffContext(System.Single[] bases, System.Single[] scales, System.Int32 staff, System.Boolean scor4)
System.Void Adjust(System.Single[] row, System.Single& pitch)
System.Void AdjustBeamStem(System.Single[] row, System.Int32 direction, System.Single& extension)
ScoreModern.RecoveredFragments.InputEndpointStaffContext ForStaff(System.Int32 value)
```

## ScoreModern.RecoveredFragments.InputEndpointText

```text
static ScoreModern.RecoveredFragments.InputEndpointTokens Parse(System.String text, System.Boolean beams, System.Int32 meterTop, System.Int32 meterBottom)
static ScoreModern.RecoveredFragments.InputEndpointTokens Parse(System.String text, System.Boolean beams, System.Int32 meterTop, System.Int32 meterBottom, ScoreModern.RecoveredFragments.InputMacroContext context)
```

## ScoreModern.RecoveredFragments.InputEndpointTokens

```text
System.Boolean Automatic
System.Single GroupControl
System.Single[] Pairs
System.Int32 Skip
InputEndpointTokens()
```

## ScoreModern.RecoveredFragments.InputListExpansion

```text
static System.Void Repeat(System.Collections.Generic.List`1[System.Single] tokens, System.Int32 entries, System.Int32 total, System.Boolean keepAccidentals)
static System.Void RepeatBeforePrevious(System.Collections.Generic.List`1[System.Single] tokens, System.Boolean keepAccidentals)
```

## ScoreModern.RecoveredFragments.InputMacroContext

```text
System.Collections.Generic.IEnumerable`1[System.Collections.Generic.KeyValuePair`2[System.Char,System.Single[]]] Entries { get; }
System.Int32 StoredDefinitionCount { get; set; }
InputMacroContext()
System.Void AllowReplacement(System.Char name)
System.Void Clear()
System.Void ClearReplacementAllowances()
ScoreModern.RecoveredFragments.InputMacroContext Clone()
System.Void ReplaceFrom(ScoreModern.RecoveredFragments.InputMacroContext source)
```

## ScoreModern.RecoveredFragments.InputMainNoteReconstruction

```text
static System.Collections.Generic.List`1[System.Single[]] Build(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 stem, System.Nullable`1[System.Int16] retainedGraceWord = null, System.Boolean originalInvalidStates = False)
```

## ScoreModern.RecoveredFragments.InputMarksCompiler

```text
static ScoreModern.RecoveredFragments.InputMarksResult Apply(System.Collections.Generic.IList`1[System.Single[]] original, System.Int32[] map, System.Single[] tokens, System.Int32[] opposite, System.Single staff, System.Single beginning, System.Single end, System.Single followingStaffScale, System.Single dynamics)
static ScoreModern.RecoveredFragments.InputMarksResult Apply(System.Collections.Generic.IList`1[System.Single[]] original, System.Int32[] map, System.Single[] tokens, System.Int32[] opposite, System.Single staff, System.Single beginning, System.Single end, System.Single followingStaffScale, System.Single dynamics, System.Boolean reproduceRetainedDescriptor)
```

## ScoreModern.RecoveredFragments.InputMarksResult

```text
System.Int32 FirstOrnament
System.Collections.Generic.List`1[System.Single[]] Records
InputMarksResult()
```

## ScoreModern.RecoveredFragments.InputMarkText

```text
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context)
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context, System.Boolean rejectMultiDigitF)
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text)
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text, System.Boolean rejectMultiDigitF)
```

## ScoreModern.RecoveredFragments.InputMarkTokens

```text
System.Int32[] Opposite
System.Single[] Values
InputMarkTokens()
```

## ScoreModern.RecoveredFragments.InputMotiveReplacementException

```text
System.Char Name
InputMotiveReplacementException(System.Char name)
```

## ScoreModern.RecoveredFragments.InputPitchRecords

```text
static ScoreModern.RecoveredFragments.InputPitchResult Compile(System.Single[] tokens, System.Single staff, System.Int32 clef, System.Single left, System.Single right, System.Boolean insetLeft)
static ScoreModern.RecoveredFragments.InputPitchResult CompileSingleNotes(System.Single[] tokens, System.Single staff, System.Int32 clef, System.Single left, System.Single right, System.Boolean insetLeft)
```

## ScoreModern.RecoveredFragments.InputPitchResult

```text
System.Int32 Events
System.Int32 FinalClef
System.Collections.Generic.List`1[System.Int32] ForcedStems
System.Single Left
System.Collections.Generic.List`1[System.Int32] NoteIndices
System.Collections.Generic.List`1[System.Single[]] Records
InputPitchResult()
```

## ScoreModern.RecoveredFragments.InputPitchState

```text
System.Int32 BarSpan
System.Int32 KeySignature
System.Int32 MeterBottom
System.Int32 MeterTop
System.Int32 Octave
System.Int32 Previous
System.Int32 PreviousDegree
System.Boolean Proximity
InputPitchState()
```

## ScoreModern.RecoveredFragments.InputPitchToken

```text
static System.Nullable`1[System.Single] Parse(System.String text, ScoreModern.RecoveredFragments.InputPitchState state)
```

## ScoreModern.RecoveredFragments.InputProximityDelimiterException

```text
System.Int32 Caret
InputProximityDelimiterException(System.Int32 caret)
```

## ScoreModern.RecoveredFragments.InputRealReader

```text
static System.Single Read(System.String text, System.Int32& at)
```

## ScoreModern.RecoveredFragments.InputRecordCompaction

```text
static System.Byte[] Compact(System.Single[] parameters)
```

## ScoreModern.RecoveredFragments.InputReferenceSpacing

```text
System.Boolean HasClefAtZero { get; }
InputReferenceSpacing(System.Collections.Generic.IList`1[System.Single[]] staff)
InputReferenceSpacing(System.Collections.Generic.IList`1[System.Single[]] staff, System.Action`2[System.Int32,System.Single] normalizedPosition)
```

## ScoreModern.RecoveredFragments.InputRhythmDecoder

```text
static ScoreModern.RecoveredFragments.InputRhythmValue Decode(System.Single encoded)
```

## ScoreModern.RecoveredFragments.InputRhythmRecords

```text
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Single[] encoded, System.Single left, System.Single right, System.Single scale, System.Boolean proportional, System.Single exponent, System.Int32[] forcedStems, System.Nullable`1[System.Single] referenceStart = null, System.Nullable`1[System.Single] rememberedLeft = null)
```

## ScoreModern.RecoveredFragments.InputRhythmToken

```text
static System.Single Parse(System.String text)
static System.Single Parse(System.String text, System.Boolean originalMantissaWrap)
```

## ScoreModern.RecoveredFragments.InputRhythmValue

```text
System.Single Denominator
System.Single DotControl
System.Single Duration
System.Single FlagCount
```

## ScoreModern.RecoveredFragments.InputSetupBounds

```text
static ScoreModern.RecoveredFragments.InputSetupBoundsResult Find(System.Collections.Generic.IList`1[System.Single[]] records, ScoreModern.RecoveredFragments.InputSetupState state)
```

## ScoreModern.RecoveredFragments.InputSetupBoundsResult

```text
System.Int32 Clef
System.String Key
System.Int32 Left
System.Int32 MeterBottom
System.Int32 MeterTop
System.Int32 Right
InputSetupBoundsResult()
```

## ScoreModern.RecoveredFragments.InputSetupState

```text
System.String Key
System.Int32 LeftEnd
System.Int32 LeftPosition
System.Int32 MeterBottom
System.Int32 MeterTop
System.Int32 RightEnd
System.Int32 RightPosition
System.Int32 Selected
System.Single Size
System.Int32 Spacing
System.Int32 Staff
System.Int32 Vertical
InputSetupState()
System.Void KeyText(System.String key)
System.Void MoveField(System.Int32 delta)
System.Boolean Numeric(System.Single value)
static System.String PresetName(System.Int32 scan)
System.Void SpacingLetter(System.Char letter)
System.Single[] StaffRecord()
```

## ScoreModern.RecoveredFragments.InputSlurConstruction

```text
static System.Collections.Generic.List`1[System.Single[]] Build(System.Collections.Generic.IList`1[System.Single[]] original, ScoreModern.RecoveredFragments.InputEndpointResult e, System.Single staff, System.Int32 boundary, System.Int16 tiesMode, System.Single end, ScoreModern.RecoveredFragments.InputEndpointStaffContext context = null)
static System.Single InitialHeight(System.Single left, System.Single right, System.Single y1, System.Single y2, System.Single offset, System.Single height)
```

## ScoreModern.RecoveredFragments.InputStaccatoCentering

```text
static System.Boolean Apply(System.Single[] p, System.Single[] staffScales)
```

## ScoreModern.RecoveredFragments.InputStaffStage

```text
static ScoreModern.RecoveredFragments.InputStaffStage Beams = 3
static ScoreModern.RecoveredFragments.InputStaffStage Complete = 5
static ScoreModern.RecoveredFragments.InputStaffStage Marks = 2
static ScoreModern.RecoveredFragments.InputStaffStage Pitches = 0
static ScoreModern.RecoveredFragments.InputStaffStage Rhythms = 1
static ScoreModern.RecoveredFragments.InputStaffStage Slurs = 4
System.Int32 value__
```

## ScoreModern.RecoveredFragments.InputStaffTransaction

```text
System.Int32 InitialClef { get; }
System.Single OriginalSelectionLeft { get; }
System.Single OriginalSelectionRight { get; }
System.Single OriginalStaff { get; }
System.String ReferenceSpacingWarning { get; set; }
System.Int32 RhythmCount { get; }
ScoreModern.RecoveredFragments.InputStaffStage Stage { get; set; }
System.Collections.Generic.List`1[System.Single[]] WorkingRecords { get; }
InputStaffTransaction(System.Single staff, System.Int32 clef, System.Single beginning, System.Single end, System.Single scale, System.Single followingScale, System.Single beam, System.Single dynamics, System.Int16 ties)
InputStaffTransaction(System.Single staff, System.Int32 clef, System.Single beginning, System.Single end, System.Single scale, System.Single followingScale, System.Single beam, System.Single dynamics, System.Int16 ties, System.Boolean proportional, System.Single exponent)
System.Void Beams(System.Single[] endpointPairs)
System.Void Beams(ScoreModern.RecoveredFragments.InputEndpointTokens tokens)
System.Collections.Generic.List`1[System.Single[]] Commit(System.Collections.Generic.IList`1[System.Single[]] prefix)
System.Void Marks(System.Single[] tokens, System.Int32[] opposite)
System.Void Marks(System.Single[] tokens, System.Int32[] opposite, System.Boolean reproduceRetainedDescriptor)
System.Single[] MouseNotePositions()
System.Void ObservePitchPreviewLoader()
System.Void Pitches(System.Single[] tokens, System.Boolean insetLeft)
System.Void Pitches(System.Single[] tokens, System.Boolean insetLeft, System.Boolean allowLargeInput)
System.Void Pitches(System.Single[] tokens, System.Boolean insetLeft, System.Boolean allowLargeInput, System.Boolean useReferenceClef)
System.Collections.Generic.List`1[System.Single[]] Preview(System.Collections.Generic.IList`1[System.Single[]] prefix)
ScoreModern.RecoveredFragments.InputStaffTransaction Restart(System.Nullable`1[System.Single] correctedBeginning = null, System.Nullable`1[System.Single] correctedEnd = null, System.Nullable`1[System.Boolean] correctedProportional = null)
ScoreModern.RecoveredFragments.InputStaffTransaction RestartOnStaff(System.Int32 number, System.Int32 initialClef, System.Single factor, System.Single nextFactor)
ScoreModern.RecoveredFragments.InputStaffTransaction RestartOnStaffRange(System.Int32 number, System.Int32 initialClef, System.Single factor, System.Single nextFactor, System.Single correctedBeginning, System.Single correctedEnd, System.Nullable`1[System.Boolean] correctedProportional = null)
System.Void Rhythms(System.Single[] tokens)
System.Void Rhythms(System.Single[] tokens, System.Boolean reproduceNonfiniteChordDuration)
System.Void SkipRhythms()
System.Void Slurs(System.Single[] endpointPairs)
System.Void UpdateInputMeter(System.Int32& top, System.Int32& bottom)
System.Void UseReferenceSpacing(ScoreModern.RecoveredFragments.InputReferenceSpacing reference)
System.Void UseStaffCoordinates(ScoreModern.RecoveredFragments.InputEndpointStaffContext context)
```

## ScoreModern.RecoveredFragments.InputTextLists

```text
static System.Single[] Pitches(System.String text, ScoreModern.RecoveredFragments.InputPitchState state)
static System.Single[] Pitches(System.String text, ScoreModern.RecoveredFragments.InputPitchState state, ScoreModern.RecoveredFragments.InputMacroContext context)
static System.Single[] Rhythms(System.String text)
static System.Single[] Rhythms(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context)
static System.Single[] Rhythms(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context, System.Boolean originalMantissaWrap)
```

## ScoreModern.RecoveredFragments.InputTextTerminatorWrite

```text
static System.Byte Space = 32
static System.UInt16 Apply(System.Byte[] segment, System.UInt16 destination, System.UInt16 length)
static System.UInt16 Offset(System.UInt16 destination, System.UInt16 length)
```

## ScoreModern.RecoveredFragments.InputTranspositionAccidentalTail

```text
static ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord] entries, System.Int16[] pitches, System.Collections.Generic.IList`1[System.Int32] relationships, System.Int16 activeCount, System.Single stopPosition, System.Int16 kindZeroGuard, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow initialWindow, System.Single margin, System.Single width)
```

## ScoreModern.RecoveredFragments.InputTranspositionAttachments

```text
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single left, System.Single right, System.Single diatonic, System.Boolean enabled)
static System.Boolean ShouldApply(System.Int16 relationshipCount, System.Boolean extendedBeam)
```

## ScoreModern.RecoveredFragments.InputTranspositionClefs

```text
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord] entries, System.Int16[] pitches, System.Single staff, System.Int16 target, ScoreModern.RecoveredFragments.TranspositionClefState state)
```

## ScoreModern.RecoveredFragments.InputTranspositionCollection

```text
static System.Collections.Generic.List`1[System.Single] Boundaries(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single firstNotePosition, System.Single expandedRight)
static ScoreModern.RecoveredFragments.TranspositionCollection FirstPass(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single expandedLeft, System.Single expandedRight, System.Int16 mode)
static ScoreModern.RecoveredFragments.TranspositionCollection FirstPass(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single expandedLeft, System.Single expandedRight, System.Int16 mode, System.Int16[] recoveredAccidentalTable)
```

## ScoreModern.RecoveredFragments.InputTranspositionCollector

```text
static ScoreModern.RecoveredFragments.TranspositionCollectorResult Build(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single left, System.Single right, System.Int16 mode, ScoreModern.RecoveredFragments.TranspositionCollectorState state)
static ScoreModern.RecoveredFragments.TranspositionCollectorResult Build(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single left, System.Single right, System.Int16 mode, ScoreModern.RecoveredFragments.TranspositionCollectorState state, System.Int16[] recoveredAccidentalTable)
```

## ScoreModern.RecoveredFragments.InputTranspositionEmbeddedControl

```text
static System.String FingeringWarning = ******** Fingerings for guitar chords will NOT transpose when P5 = -1 or -2.
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Int16 semitoneClass, System.Boolean& warningRequired)
```

## ScoreModern.RecoveredFragments.InputTranspositionInterval

```text
static ScoreModern.RecoveredFragments.TranspositionInterval Convert(System.Int16 semitones)
static System.Int16 Menu(System.Int16 choice)
```

## ScoreModern.RecoveredFragments.InputTranspositionKey

```text
static System.Void Convert(System.Int16 semitones, System.Int16 oldKey, System.Int16& diatonic, System.Int16& newKey)
static System.Int16 KeyPosition(System.Int16 key)
```

## ScoreModern.RecoveredFragments.InputTranspositionKeyInsertion

```text
static System.Single[] Append(System.Collections.Generic.IList`1[System.Single[]] rows, System.Single staff, System.Int16 key, System.Int16 clef)
static System.Boolean ShouldInsert(System.Boolean answerYes, System.Int16 firstKeyState, System.Int16 semitoneClass)
static System.Boolean ShouldPrompt(System.Boolean forceQuestion, System.Int16 firstKeyState, System.Int16 semitoneClass)
```

## ScoreModern.RecoveredFragments.InputTranspositionKeyRecord

```text
static ScoreModern.RecoveredFragments.TranspositionKeyRecordResult Apply(System.Single[] parameters, System.Int16 collectedKey, System.Int16 semitones, System.Int16 previousOverride, System.Int16 clef, System.Int16 collectorIndex, System.Single erasedCode)
```

## ScoreModern.RecoveredFragments.InputTranspositionMainPass

```text
static ScoreModern.RecoveredFragments.TranspositionMainPassResult Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord] entries, System.Int16[] pitches, System.Collections.Generic.IList`1[System.Int32] relationships, ScoreModern.RecoveredFragments.TranspositionMainState state)
```

## ScoreModern.RecoveredFragments.InputTranspositionOperation

```text
static ScoreModern.RecoveredFragments.TranspositionOperationResult Apply(System.Collections.Generic.IList`1[System.Single[]] source, ScoreModern.RecoveredFragments.TranspositionRequest request, ScoreModern.RecoveredFragments.TranspositionWorkspace workspace)
static ScoreModern.RecoveredFragments.TranspositionOperationSession Begin(System.Collections.Generic.IList`1[System.Single[]] source, ScoreModern.RecoveredFragments.TranspositionRequest request, ScoreModern.RecoveredFragments.TranspositionWorkspace workspace)
static System.Void ChangeClef(ScoreModern.RecoveredFragments.TranspositionOperationSession session, System.Int16 choice)
static ScoreModern.RecoveredFragments.TranspositionOperationResult Finish(ScoreModern.RecoveredFragments.TranspositionOperationSession session, System.Boolean addKey)
static ScoreModern.RecoveredFragments.TranspositionOperationResult FinishCore(ScoreModern.RecoveredFragments.TranspositionOperationSession session, System.Boolean addKey, System.Boolean mainAttachments)
```

## ScoreModern.RecoveredFragments.InputTranspositionOrder

```text
static System.Boolean MarksModified(System.Int32 recordCount, System.Boolean alreadySorted)
static System.Int32[] OrderForMain(System.Collections.Generic.IList`1[System.Single[]] rows, System.Boolean alreadySorted)
```

## ScoreModern.RecoveredFragments.InputTranspositionPartition

```text
static ScoreModern.RecoveredFragments.TranspositionPartition Apply(System.Collections.Generic.IList`1[System.Single] bars, System.Collections.Generic.IList`1[System.Single] positions, System.Collections.Generic.IList`1[System.Int16] kinds)
```

## ScoreModern.RecoveredFragments.InputTranspositionPitch

```text
static ScoreModern.RecoveredFragments.TranspositionPitchResult Apply(System.Single originalPitch, System.Single originalStem, System.Int16 collectedPitch, System.Int16 accidental, System.Int16 clef, System.Int16 oldKey, System.Int16 newKey, System.Int16 diatonic, System.Int16 keyOverride)
```

## ScoreModern.RecoveredFragments.InputTranspositionRelationshipWindow

```text
static System.Void Advance(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[System.Int32] itemNumbers, System.Int16& activeCount, System.Int16& index, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow& window, System.Single margin, System.Single width)
```

## ScoreModern.RecoveredFragments.InputUniformBeam

```text
static ScoreModern.RecoveredFragments.InputBeamConstructionResult Build(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32[] map, System.Int32[] stems, System.Single first, System.Single last, System.Single begin, System.Single end, System.Single slopeLimit, ScoreModern.RecoveredFragments.InputEndpointStaffContext context = null)
```

## ScoreModern.RecoveredFragments.INumericConverterContext

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IParameterWordMemory

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IPointBufferMemory

```text
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IPrefixPreparationMemory

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IScreenBiosTransferEffects

```text
ScoreModern.RecoveredFragments.SignedDivide16State TransferToBios(ScoreModern.RecoveredFragments.IScreenHookMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.IScreenForwardMemory

```text
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.IScreenHookMemory

```text
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
```

## ScoreModern.RecoveredFragments.ISelectorContext

```text
System.UInt16 DataSegment { get; }
System.Boolean DirectionFlag { get; }
System.UInt16 CallFar(System.UInt16 relativeSegment, System.UInt16 offset, System.UInt16[] stackWords)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.ISpanBookkeepingMemory

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IStringBlockMemory

```text
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ScoreModern.RecoveredFragments.IStringOutputContext

```text
ScoreModern.RecoveredFragments.CharacterRoutineResult CallCharacter(ScoreModern.RecoveredFragments.CharacterRoutineWords words, System.UInt16 targetSegment, System.UInt16 targetOffset)
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
```

## ScoreModern.RecoveredFragments.ItemDrawingDispatch

```text
static ScoreModern.RecoveredFragments.ItemDrawingRoute Select(ScoreModern.RecoveredFragments.IDatTextLookupMemory memory, System.UInt16 ds, System.UInt16 cs)
```

## ScoreModern.RecoveredFragments.ItemDrawingRoute

```text
System.UInt16 TableByteIndex
System.UInt16 Target
System.Boolean UsesTable
```

## ScoreModern.RecoveredFragments.JustificationAllocationResult

```text
System.Single Allocated
System.Int16 NextIndex
System.Single Remaining
JustificationAllocationResult()
```

## ScoreModern.RecoveredFragments.JustificationCoordinates

```text
System.Single LowerBound
JustificationCoordinates(System.Single[] words, System.Int16[] references, System.Single lowerBound, System.Single ratioThreshold, System.Single ratioReplacement)
System.Void Scale(System.Single left, System.Single right, System.Single delta)
System.Void Shift(System.Single threshold, System.Single delta)
```

## ScoreModern.RecoveredFragments.JustificationGapResult

```text
System.Boolean CompressedMinimums
System.Single DeficitPerGap
System.Single MinimumTotal
System.Single NegativeSlack
System.Single PositiveSlack
System.Single Width
JustificationGapResult()
```

## ScoreModern.RecoveredFragments.LayoutBasicTablature

```text
static System.Collections.Generic.List`1[System.Single[]] Build(System.Single[] staff, System.Collections.Generic.IList`1[System.Single[]] notes, System.Int32 destination, System.Boolean clef, System.Boolean rhythmLine, System.Int16[] packedTuning, System.Int32 strings, System.Boolean bass, System.Collections.Generic.IList`1[System.Int16] frequencies, System.Boolean hasCollection)
static System.Collections.Generic.List`1[System.Single[]] Build(System.Single[] staff, System.Collections.Generic.IList`1[System.Single[]] notes, System.Int32 destination, System.Boolean clef, System.Boolean rhythmLine, System.Int16[] packedTuning, System.Int32 strings, System.Boolean bass, System.Collections.Generic.IList`1[System.Int16] frequencies, System.Boolean hasCollection, System.Int16[] recoveredAccidentals, System.Int32 firstDelta, System.Func`3[System.Single[],System.Int32,System.Single[]] retainedCopy = null, System.Action`2[System.Single[],System.Int32] writeRecord = null)
```

## ScoreModern.RecoveredFragments.LayoutCoordinateAffine

```text
static System.Void Apply(System.Single[] words, System.Int16[] references, System.Single first, System.Single second, System.Single third, System.Int16 mode)
```

## ScoreModern.RecoveredFragments.LayoutCoordinateCollection

```text
static ScoreModern.RecoveredFragments.LayoutCoordinateCollectionResult Collect(System.Single[] words, System.Int32[] itemStarts, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Single tolerance, System.Int16 first, System.Int16 last, System.Int16 staffTableThreshold, System.Single codeFilter, System.Single emptyStaffControl, System.Single compoundBarControl, System.Int32 capacity, System.Int16 mode = 0)
```

## ScoreModern.RecoveredFragments.LayoutCoordinateCollectionResult

```text
System.Int16 FirstItem
System.Int16 LastItem
System.Int16[] References
LayoutCoordinateCollectionResult()
```

## ScoreModern.RecoveredFragments.LayoutDecombineGroups

```text
static System.Int16[] Boundaries(System.Int16 maximum, System.Single[] sizes, System.Int16 lastSize)
```

## ScoreModern.RecoveredFragments.LayoutDisappearingStaffs

```text
LayoutDisappearingStaffs()
System.Void Apply(System.Single[] values)
System.Boolean Hidden(System.Int32 staff)
System.Int32[] HiddenStaffs()
```

## ScoreModern.RecoveredFragments.LayoutItemSpacing

```text
LayoutItemSpacing(System.Byte[] workspace, System.Single[] scoreWords, System.Int32[] itemStarts, System.Single[] selectedPositions, System.Int16[] selectedReferences, System.Single[] auxiliary, System.Single[] minimumGaps, System.Single[] outputPositions)
System.Int16 Apply(System.Int16 staff, System.Single staffScale, System.Single firstStaff, System.Int16& cursor, System.Single left, System.Single right, System.Single[] afterSymbol, System.Boolean originalQuarterToneWidths = True, System.Nullable`1[System.Single] positionJoinTolerance = null, System.Boolean standaloneJust = False)
System.Int16 Apply(System.Int16 staff, System.Single staffScale, System.Single firstStaff, System.Int16& cursor, System.Single left, System.Single right, System.Single[] afterSymbol, System.Boolean originalQuarterToneWidths, System.Nullable`1[System.Single] positionJoinTolerance, System.Boolean standaloneJust, System.Boolean scor4Numbers)
```

## ScoreModern.RecoveredFragments.LayoutJustificationAllocation

```text
static ScoreModern.RecoveredFragments.JustificationAllocationResult Apply(System.Single[] positions, System.Single[] allocations, System.Int16 index, System.Single target, System.Single limit, System.Single remaining, System.Single minimum)
```

## ScoreModern.RecoveredFragments.LayoutJustificationGaps

```text
static ScoreModern.RecoveredFragments.JustificationGapResult Prepare(System.Single[] slack, System.Single[] minimum, System.Single[] positions, System.Int16 pointCount, System.Single left, System.Single right)
```

## ScoreModern.RecoveredFragments.LayoutJustificationMerge

```text
static System.Int16 Append(System.Single[] oldPositions, System.Single[] oldMinimum, System.Single[] newPositions, System.Single[] newMinimum, System.Single[] mergedPositions, System.Single[] mergedMinimum, System.Int16 oldCount, System.Int16 newCount, System.Single right, System.Single rightThreshold, System.Single epsilon)
```

## ScoreModern.RecoveredFragments.LayoutJustificationRedistribution

```text
static System.Boolean Apply(System.Single[] slack, System.Single[] positions, System.Int16 pointCount, System.Single positiveSlack, System.Single negativeSlack, ScoreModern.RecoveredFragments.JustificationCoordinates coordinates)
```

## ScoreModern.RecoveredFragments.LayoutJustificationSnap

```text
static System.Void Apply(System.Single[] words, System.Int32[] itemStarts, System.Single tolerance, System.Single rightEdge)
```

## ScoreModern.RecoveredFragments.LayoutJustificationSystem

```text
static ScoreModern.RecoveredFragments.LayoutJustificationSystemResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Single[] scales, System.Single[] afterSymbol, System.Int16 staffTableThreshold, System.Int16 coordinateCapacity, ScoreModern.RecoveredFragments.LayoutJustificationWorkspace work)
```

## ScoreModern.RecoveredFragments.LayoutJustificationSystemResult

```text
System.Int16[] CoordinateReferences
System.Int16 FirstPoint
ScoreModern.RecoveredFragments.JustificationGapResult Gaps
System.Int16 PointCount
System.Boolean RedistributionApplied
System.Int16 SelectionCursor
LayoutJustificationSystemResult()
```

## ScoreModern.RecoveredFragments.LayoutJustificationWorkspace

```text
System.Single[] Auxiliary
System.Single[] MergedMinimum
System.Single[] MergedPositions
System.Single[] Minimum
System.Single[] OldMinimum
System.Single[] OldPositions
System.Boolean OriginalQuarterToneWidths
System.Boolean Page4RoundScaleNumerator
System.Nullable`1[System.Single] PositionJoinTolerance
System.Single[] Positions
System.Boolean Scor4NumberJustification
System.Single[] Selected
System.Int16[] SelectedReferences
System.Byte[] Spacing
System.Boolean StandaloneJust
LayoutJustificationWorkspace()
System.Void ResetForIsolatedTest()
```

## ScoreModern.RecoveredFragments.LayoutLineupBeatTotal

```text
System.Int16 Staff
System.Single Total
LayoutLineupBeatTotal()
```

## ScoreModern.RecoveredFragments.LayoutLineupCleanup

```text
static System.Void Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] staffUnits)
```

## ScoreModern.RecoveredFragments.LayoutLineupCoordinates

```text
static System.Void Apply(System.Single[] merged, System.Single[] positions, System.Single[] weights, System.Int16 count, System.Single left, System.Single right, System.Single staffUnit, System.Single exponent)
```

## ScoreModern.RecoveredFragments.LayoutLineupDurationGroups

```text
static ScoreModern.RecoveredFragments.LayoutLineupDurationGroupsResult Apply(System.Single[] current, System.Single[] prior, System.Single[] output, System.Int16 firstCurrent, System.Int16 priorCount, System.Int16 initialOutputCount)
```

## ScoreModern.RecoveredFragments.LayoutLineupDurationGroupsResult

```text
System.Int16 CurrentIndex
System.Single CurrentSum
System.Single CurrentValue
System.Int16 OutputCount
System.Int16 PriorIndex
System.Single PriorSum
System.Single PriorValue
LayoutLineupDurationGroupsResult()
```

## ScoreModern.RecoveredFragments.LayoutLineupPreparation

```text
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] rhythms, System.Single[] positions, System.Single[] merged, System.Single[] scratch, System.Single[] union, System.Single[] subdivisions)
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult ApplyWithBeatPolicy(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] rhythms, System.Single[] positions, System.Single[] merged, System.Single[] scratch, System.Single[] union, System.Single[] subdivisions, System.Boolean returnUnequalBeats, System.Action`1[System.Int32] acknowledgeNegativePosition = null, System.Boolean standaloneJust = False)
```

## ScoreModern.RecoveredFragments.LayoutLineupPreparationResult

```text
ScoreModern.RecoveredFragments.LayoutLineupBeatTotal[] BeatTotals
System.Int16 Bottom
System.Int16 Count
System.Single Left
System.Boolean NeedsCleanup
System.Single Right
ScoreModern.RecoveredFragments.LayoutLineupStaffStream[] StaffStreams
System.Int16 Top
System.Boolean UnequalBeats
LayoutLineupPreparationResult()
```

## ScoreModern.RecoveredFragments.LayoutLineupRemap

```text
static System.Void Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] merged, System.Single[] newPositions, System.Int16 count, System.Single[] savedRhythms, System.Single[] savedPositions, System.Int16[] staffStarts, System.Single[] staffUnits, System.Int16 staffThreshold, System.Int32 capacity)
```

## ScoreModern.RecoveredFragments.LayoutLineupStaffStream

```text
System.Single[] Positions
System.Single[] Rhythms
System.Int16 Staff
LayoutLineupStaffStream()
```

## ScoreModern.RecoveredFragments.LayoutLineupStaffTables

```text
static ScoreModern.RecoveredFragments.LayoutLineupStaffTablesResult Apply(System.Single[] merged, System.Int16 mergedCount, System.Single[] storedRhythms, System.Single[] storedPositions, System.Int16 firstStored, System.Single[] outputRhythms, System.Single[] outputPositions)
```

## ScoreModern.RecoveredFragments.LayoutLineupStaffTablesResult

```text
System.Int16 Count
System.Int16 FirstMerged
LayoutLineupStaffTablesResult()
```

## ScoreModern.RecoveredFragments.LayoutLineupStreamUnion

```text
static ScoreModern.RecoveredFragments.LayoutLineupStreamUnionResult Apply(System.Single[] current, System.Int16 firstCurrent, System.Int16 currentCount, System.Single[] prior, System.Int16 priorCount, System.Single[] output)
```

## ScoreModern.RecoveredFragments.LayoutLineupStreamUnionResult

```text
System.Int16 Count
System.Int16 CurrentIndex
System.Single CurrentSum
System.Single LastValue
System.Int16 PriorIndex
System.Single PriorSum
LayoutLineupStreamUnionResult()
```

## ScoreModern.RecoveredFragments.LayoutLineupSystem

```text
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] staffUnits, System.Single exponent, System.Int16 staffThreshold, System.Int32 capacity, ScoreModern.RecoveredFragments.LayoutLineupWorkspace work)
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult ApplyWithBeatPolicy(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] staffUnits, System.Single exponent, System.Int16 staffThreshold, System.Int32 capacity, ScoreModern.RecoveredFragments.LayoutLineupWorkspace work, System.Boolean returnUnequalBeats, System.Action`1[System.Int32] acknowledgeNegativePosition = null)
```

## ScoreModern.RecoveredFragments.LayoutLineupTableCompact

```text
static System.Int16 Apply(System.Single[] source, System.Single[] destination, System.Int16 first, System.Int16 last, System.Single retainedTag)
```

## ScoreModern.RecoveredFragments.LayoutLineupWorkspace

```text
System.Int32 MaximumStaff
System.Single[] Merged
System.Boolean Page4RoundEachPosition
System.Single[] Positions
System.Single[] Rhythms
System.Boolean Scor4DurationScale
System.Single[] Scratch
System.Boolean StandaloneJust
System.Single[] Subdivisions
System.Single[] Union
LayoutLineupWorkspace()
```

## ScoreModern.RecoveredFragments.LayoutNoteSpacingSpan

```text
LayoutNoteSpacingSpan(System.Byte[] workspace, System.Single[] scoreWords, System.Int16[] recordReferences, System.Single[] selectedPositions, System.Boolean originalQuarterToneWidths, System.Single followingBarScale)
System.Boolean Apply()
System.Void LoadRecord(System.Int32 wordIndex, System.Int32 countAddress)
```

## ScoreModern.RecoveredFragments.LayoutParentPreparation

```text
System.Int16 Bottom
System.Boolean Complete
System.Int32 DuplicateIndex
System.Single Left
System.Boolean Modified
System.Collections.Generic.List`1[System.Int32] Origins
System.Single Right
System.Collections.Generic.List`1[System.Single[]] Rows
System.Int16 Top
LayoutParentPreparation(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Boolean alreadySorted, System.Boolean lineupMode)
System.Void AnswerDuplicate(System.Boolean delete)
```

## ScoreModern.RecoveredFragments.LayoutRestSpacing

```text
static ScoreModern.RecoveredFragments.LayoutRestSpacingResult Apply(System.Single multiplier, System.Single p6, System.Single p5, System.Single p4, System.Single left, System.Int16 parameterCount, System.Single p9, System.Single p10, System.Single p15)
```

## ScoreModern.RecoveredFragments.LayoutRestSpacingResult

```text
System.Single Left
System.Single Multiplier
System.Nullable`1[System.Single] Parameter10Scratch
System.Single Right
LayoutRestSpacingResult()
```

## ScoreModern.RecoveredFragments.LayoutRhythmicCollection

```text
static ScoreModern.RecoveredFragments.LayoutRhythmicCollectionResult Collect(System.Single[] words, System.Int32[] starts, System.Single staff, System.Int16 inputMode, System.Int16 cursor, System.Single[] rhythms, System.Single[] positions, System.Single[] scratch, System.Action`1[System.Int32] acknowledgeNegativePosition = null, System.Boolean standaloneJust = False)
```

## ScoreModern.RecoveredFragments.LayoutRhythmicCollectionResult

```text
System.Int16 Count
System.Boolean InvalidatedSort
System.Int16 NextItem
System.Single Total
LayoutRhythmicCollectionResult()
```

## ScoreModern.RecoveredFragments.LayoutRipple

```text
static ScoreModern.RecoveredFragments.LayoutRippleResult Prepare(System.Single[] words, System.Int32[] starts, System.Int16 count, System.Int16 source, System.Int16 destination, System.Int16 bars)
```

## ScoreModern.RecoveredFragments.LayoutRippleCoordinates

```text
static System.Void Scale(System.Single[] words, System.Int16[] references, System.Single origin, System.Single factor)
static System.Void Shift(System.Single[] words, System.Int16[] references, System.Single delta)
static System.Void StaffShift(System.Single[] words, System.Int16[] references, System.Single delta)
```

## ScoreModern.RecoveredFragments.LayoutRippleResult

```text
System.Boolean Applied
System.Int16 Count
System.Int16 Destination
System.Int16 Source
System.Int32[] Starts
LayoutRippleResult()
```

## ScoreModern.RecoveredFragments.LayoutSeparateParts

```text
static ScoreModern.RecoveredFragments.LayoutSeparateResult Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Boolean deleteBottom)
static System.Void Validate(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right)
```

## ScoreModern.RecoveredFragments.LayoutSeparateResult

```text
System.Collections.Generic.List`1[System.Int16] ChangedStaffs
System.Int32 Deleted
System.Collections.Generic.List`1[System.Int32] Origins
System.Collections.Generic.List`1[System.Single[]] Rows
LayoutSeparateResult()
```

## ScoreModern.RecoveredFragments.LayoutSpacingSelection

```text
static ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult Collect(System.Single[] words, System.Int32[] starts, System.Int16 staff, System.Int16 firstStaff, System.Int16 cursor, System.Single left, System.Single right, System.Single tolerance, System.Single one, System.Single clefLimit, System.Single spanningLimit, System.Single barLimit, System.Single drawingLimit, System.Int32 capacity)
static ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult Collect(System.Single[] words, System.Int32[] starts, System.Int16 staff, System.Int16 firstStaff, System.Int16 cursor, System.Single left, System.Single right, System.Single tolerance, System.Single one, System.Single clefLimit, System.Single spanningLimit, System.Single barLimit, System.Single drawingLimit, System.Int32 capacity, System.Boolean scor4Numbers)
static System.Void Sort(System.Single[] positions, System.Int16[] records, System.Int16 count)
```

## ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult

```text
System.Single[] Positions
System.Int16[] RecordWords
System.Int32 StoppedItem
LayoutSpacingSelectionResult()
```

## ScoreModern.RecoveredFragments.LayoutSpacingWorkspace

```text
static System.Byte[] Create()
```

## ScoreModern.RecoveredFragments.LayoutSymbolSpacing

```text
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Bar(System.Int16 previous, System.Int16 count, System.Single p5, System.Single p7, System.Single effectiveP8, System.Single p13, System.Single p14)
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Key(System.Int16 previous, System.Single p7, System.Single p5, System.Boolean standaloneJust = False)
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Meter(System.Int16 previous, System.Single minimum, System.Single[] parameters, System.Int16 count, System.Boolean standaloneJust = False)
```

## ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult

```text
System.Single Left
System.Single Minimum
System.Boolean ResetParameter10Scratch
System.Single Right
System.Single ScaleScratch
System.Single Scratch
LayoutSymbolSpacingResult()
```

## ScoreModern.RecoveredFragments.LayoutTablatureAccidentalTable

```text
static System.Int32 FirstDelta = -23
static System.Int16[] Recover(System.UInt16 loadSegment)
```

## ScoreModern.RecoveredFragments.LayoutTablatureChord

```text
static System.Void Apply(System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.LayoutTablatureChordNote] notes, System.Int32 staffStrings)
```

## ScoreModern.RecoveredFragments.LayoutTablatureChordNote

```text
ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan Plan
System.Single[] Row
System.Int32 SortKey
LayoutTablatureChordNote()
```

## ScoreModern.RecoveredFragments.LayoutTablatureFrequency

```text
static System.Collections.Generic.Dictionary`2[System.Int32,System.Int16] Apply(ScoreModern.RecoveredFragments.TranspositionCollectorResult collection)
```

## ScoreModern.RecoveredFragments.LayoutTablaturePitch

```text
static ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan Prepare(System.Int32 head, System.Int32 pitch, System.Int32 accidental, System.Int16[] packed)
static ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan PrepareDrawing(System.Int32 head, System.Int32 pitch, System.Int32 accidental, System.Int16[] packed, System.Int16 capo)
```

## ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan

```text
System.Int32 Absolute
System.Int32 Strings
System.Int32[] Tuning
LayoutTablaturePitchPlan()
System.Int32 SelectedString(System.Int32 requested)
```

## ScoreModern.RecoveredFragments.LayoutTablatureRelationships

```text
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, ScoreModern.RecoveredFragments.TranspositionCollectorResult collected, System.Collections.Generic.Dictionary`2[System.Int32,System.Int16] frequencies)
```

## ScoreModern.RecoveredFragments.LayoutTextJustification

```text
static ScoreModern.RecoveredFragments.LayoutTextJustificationResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Int16 textStaff, System.Single level, System.Single[] staffUnits, System.Int16 staffThreshold, System.Int32 capacity, ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace work)
```

## ScoreModern.RecoveredFragments.LayoutTextJustificationResult

```text
ScoreModern.RecoveredFragments.JustificationGapResult Gaps
System.Int16 PointCount
LayoutTextJustificationResult()
```

## ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace

```text
System.Single[] Minimum
System.Single[] Positions
System.Single[] Slack
LayoutTextJustificationWorkspace()
```

## ScoreModern.RecoveredFragments.LayoutVerticalCollision

```text
static System.Int32 Minimum(System.Int16[] upper, System.Int16[] lower, System.Int16 radius)
```

## ScoreModern.RecoveredFragments.LayoutVerticalContour

```text
static System.Void Apply(System.Int16[] x, System.Int16[] y, System.Int32 first, System.Int32 next, System.Boolean skipGuide, System.Boolean initialize, System.Int16[] minimum, System.Int16[] maximum, System.Int16 binWidth, System.Int16 bias)
```

## ScoreModern.RecoveredFragments.LayoutVerticalPlacement

```text
static ScoreModern.RecoveredFragments.LayoutVerticalPlacementResult Apply(System.Int16[][] minimum, System.Int16[][] maximum, System.Boolean[] occupied, System.Single[] scales, System.Single[] bases, System.Int16 bottom, System.Int16 top, System.Int16 verticalSteps, System.Int16 radius, System.Single staffGap, System.Single[] retainedOffsets)
```

## ScoreModern.RecoveredFragments.LayoutVerticalPlacementResult

```text
System.Int16 Bottom
System.Single[] Offsets
System.Boolean Repeat
System.Int16 Top
LayoutVerticalPlacementResult()
```

## ScoreModern.RecoveredFragments.LineClipping

```text
static ScoreModern.RecoveredFragments.LineClippingWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt16 si, System.UInt16 di)
static ScoreModern.RecoveredFragments.LineClippingFlagResult ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt16 si, System.UInt16 di, System.UInt32 eflags)
```

## ScoreModern.RecoveredFragments.LineClippingDivideFault

```text
System.UInt32 EFlags
System.Boolean HasEFlags
ScoreModern.RecoveredFragments.LineClippingWords Words
LineClippingDivideFault()
```

## ScoreModern.RecoveredFragments.LineClippingFlagResult

```text
System.UInt32 EFlags
ScoreModern.RecoveredFragments.LineClippingWords Words
```

## ScoreModern.RecoveredFragments.LineClippingWords

```text
System.UInt16 AX
System.UInt16 BX
System.UInt16 CX
System.UInt16 DI
System.UInt16 DX
System.UInt16 Exit
System.UInt16 SI
```

## ScoreModern.RecoveredFragments.LineDriverBody

```text
static ScoreModern.RecoveredFragments.LineDriverBodyResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteThroughReturnWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.LineDriverBodyResult

```text
System.UInt16 ClippingExit
System.UInt32 DefinedEFlagsMask
ScoreModern.RecoveredFragments.SignedDivide16Fault Fault
System.UInt16 RasterEntry
System.Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.LinePreparation

```text
static ScoreModern.RecoveredFragments.LinePreparationWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 ax)
```

## ScoreModern.RecoveredFragments.LinePreparationWords

```text
System.UInt16 AX
System.UInt16 BX
System.UInt16 CX
System.UInt16 DI
System.UInt16 DX
System.UInt16 ES
```

## ScoreModern.RecoveredFragments.Mode12BiosTransfer

```text
Mode12BiosTransfer(ScoreModern.RecoveredFragments.Mode12VideoServices services, System.UInt16 measuredCallbackSegment, System.UInt16 measuredIretOffset)
ScoreModern.RecoveredFragments.SignedDivide16State TransferToBios(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.Mode12CellSurface

```text
System.Int64 Revision { get; set; }
Mode12CellSurface()
System.Byte Background(System.Int32 row, System.Int32 column)
ScoreModern.RecoveredFragments.Mode12Glyph[] Cell(System.Int32 row, System.Int32 column)
System.Void Clear()
System.Void Scroll(System.Int32 top, System.Int32 left, System.Int32 bottom, System.Int32 right, System.Int32 lines, System.Byte background)
System.Void WriteGlyph(System.Int32 row, System.Int32 column, System.Byte character, System.Byte color)
```

## ScoreModern.RecoveredFragments.Mode12Glyph

```text
System.Byte Character
System.Byte Color
Mode12Glyph(System.Byte c, System.Byte a)
```

## ScoreModern.RecoveredFragments.Mode12Plane2Surface

```text
System.Byte[] Bytes
Mode12Plane2Surface(System.Byte[] initial, ScoreModern.RecoveredFragments.IMode12GlyphSource glyphs)
System.Void Scroll(System.Int32 top, System.Int32 left, System.Int32 bottom, System.Int32 right, System.Int32 lines, System.Byte background)
System.Void WriteGlyph(System.Int32 row, System.Int32 column, System.Byte character, System.Byte color)
```

## ScoreModern.RecoveredFragments.Mode12VideoServices

```text
Mode12VideoServices(ScoreModern.RecoveredFragments.IMode12Surface output)
ScoreModern.RecoveredFragments.SignedDivide16State ExecuteService(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.NumericConverter

```text
static ScoreModern.RecoveredFragments.NumericConverterResult Execute(ScoreModern.RecoveredFragments.INumericConverterContext m, System.UInt16 statusSegment, System.UInt16 statusOffset, System.UInt16 inputSegment, System.UInt16 inputOffset)
```

## ScoreModern.RecoveredFragments.NumericConverterResult

```text
System.UInt16 AX
System.UInt16 DX
System.UInt16 Status
NumericConverterResult(System.UInt16 ax, System.UInt16 dx, System.UInt16 status)
```

## ScoreModern.RecoveredFragments.OutputCountAdvance

```text
static ScoreModern.RecoveredFragments.OutputCountResult Execute(ScoreModern.RecoveredFragments.ISpanBookkeepingMemory m, System.UInt16 ss, System.UInt16 countOffset, System.UInt16 ds, System.UInt16 descriptorSi)
```

## ScoreModern.RecoveredFragments.OutputCountResult

```text
System.UInt16 AX
System.UInt16 DX
OutputCountResult(System.UInt16 ax, System.UInt16 dx)
```

## ScoreModern.RecoveredFragments.PackedDrawingParts

```text
System.Int16 FinalAX
System.Int16 High
System.Int16 Low
System.Int16 Middle
```

## ScoreModern.RecoveredFragments.PackedDrawingWord

```text
static ScoreModern.RecoveredFragments.PackedDrawingParts Decode(System.Int16 word)
```

## ScoreModern.RecoveredFragments.PageCoordinates

```text
System.Single[] Bases
System.Single[] Scales
PageCoordinates()
ScoreModern.RecoveredFragments.PageItemContext Prepare(System.Single[] p, System.Int32 staff, System.Int32 code)
ScoreModern.RecoveredFragments.PageStaffResult SetStaff(System.Single[] p, System.Boolean alternateUnits)
ScoreModern.RecoveredFragments.PageStaffResult SetStaff(System.Single[] p, System.Int32 staff, System.Int16 referenceWord, System.Boolean alternateUnits)
System.Single Vertical(System.Int32 staff, System.Single value)
```

## ScoreModern.RecoveredFragments.PageItemContext

```text
System.Int16 HorizontalWord
System.Single Scale
System.Boolean ScreenHorizontalLimit
System.Single VerticalInput
System.Single X
System.Single Y
```

## ScoreModern.RecoveredFragments.PageStaffLine

```text
System.Single Left
System.Single Right
System.Single Y
```

## ScoreModern.RecoveredFragments.PageStaffResult

```text
System.Single Base
System.Single Bottom
System.Int32 LineCount
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.PageStaffLine] Lines
System.Single Scale
System.Single Spacing
System.Int16 WidthSelector
PageStaffResult()
```

## ScoreModern.RecoveredFragments.ParameterWordReader

```text
static System.UInt16 Execute(ScoreModern.RecoveredFragments.IParameterWordMemory m, System.UInt16 cursorSegment, System.UInt16 cursorOffset, System.UInt16 destinationSegment, System.UInt16 destinationOffset)
```

## ScoreModern.RecoveredFragments.PointBufferAppend

```text
static ScoreModern.RecoveredFragments.PointBufferWords Execute(ScoreModern.RecoveredFragments.IPointBufferMemory m, System.UInt16 ds, System.UInt16 ss, System.UInt16 bp)
static ScoreModern.RecoveredFragments.PointBufferWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IPointBufferMemory m, System.UInt16 ds, System.UInt16 ss, System.UInt16 bp, System.UInt32 flags)
```

## ScoreModern.RecoveredFragments.PointBufferWords

```text
System.UInt16 AX
System.UInt16 BX
System.UInt32 DefinedEFlagsMask
System.UInt32 EFlags
System.UInt16 ES
System.UInt16 SI
```

## ScoreModern.RecoveredFragments.PrefixPreparationResult

```text
System.UInt16 AX
System.UInt16 BP
System.UInt16 DI
System.UInt16 RelativeTargetSegment
System.UInt16 SI
System.UInt16 SP
System.UInt16 TargetOffset
PrefixPreparationResult(System.UInt16 ax, System.UInt16 bp, System.UInt16 sp, System.UInt16 si, System.UInt16 di)
```

## ScoreModern.RecoveredFragments.PrefixWritePreparation

```text
static ScoreModern.RecoveredFragments.PrefixPreparationResult Prepare(ScoreModern.RecoveredFragments.IPrefixPreparationMemory m, System.UInt16 ss, System.UInt16 ds, System.UInt16 cs, System.UInt16 sp, System.UInt16 bp, System.UInt16 si, System.UInt16 di)
```

## ScoreModern.RecoveredFragments.PrimitiveTextGeometry

```text
static ScoreModern.RecoveredFragments.PrimitiveTextPoint Point(System.Int16 packed, ScoreModern.RecoveredFragments.PrimitiveTextScale scale)
static ScoreModern.RecoveredFragments.PrimitiveTextScale Prepare(System.Single horizontal, System.Single vertical, System.Single staff, System.Single x, System.Single y, System.Single left, System.Single baseline)
```

## ScoreModern.RecoveredFragments.PrimitiveTextPoint

```text
System.Boolean CommandOnly
System.Int32 MarkerWordBack
System.Int16 Operation
System.Nullable`1[System.Int16] RetainedMarkerY
System.Single X
System.Single Y
```

## ScoreModern.RecoveredFragments.PrimitiveTextScale

```text
System.Single OriginX
System.Single OriginY
System.Single X
System.Single Y
```

## ScoreModern.RecoveredFragments.PrintedGlyphDefinition

```text
static System.String Fixed3(System.Single value)
static ScoreModern.RecoveredFragments.PrintedGlyphVertex Vertex(System.Int16 x, System.Int16 y, System.Int16 operation)
```

## ScoreModern.RecoveredFragments.PrintedGlyphVertex

```text
System.Int16 Operation
System.Int32 X
System.Int32 Y
```

## ScoreModern.RecoveredFragments.PrintedHeadRequest

```text
System.Int32 Glyph
System.String Scale
System.String Width
System.Int16 X
System.Int16 Y
```

## ScoreModern.RecoveredFragments.PrintRotation

```text
static System.Single Apply(System.Single size, System.Boolean before, System.Boolean after)
static System.Int32 Offset(System.Int32 candidate, System.Boolean before, System.Boolean after)
```

## ScoreModern.RecoveredFragments.RasterDispatch

```text
static ScoreModern.RecoveredFragments.RasterDispatchWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt32 flags)
```

## ScoreModern.RecoveredFragments.RasterDispatchWords

```text
System.UInt16 BX
System.UInt16 CX
System.UInt16 DX
System.UInt32 EFlags
System.UInt16 Exit
```

## ScoreModern.RecoveredFragments.RasterStatusFlags

```text
static System.UInt32 Add16(System.UInt32 flags, System.UInt16 a, System.UInt16 b)
static System.UInt32 Dec16(System.UInt32 flags, System.UInt16 a)
static System.UInt32 Inc16(System.UInt32 flags, System.UInt16 a)
static System.UInt32 Logical8(System.UInt32 flags, System.Byte result)
static System.UInt32 Sub16(System.UInt32 flags, System.UInt16 a, System.UInt16 b)
static System.UInt32 Sub8(System.UInt32 flags, System.Byte a, System.Byte b)
```

## ScoreModern.RecoveredFragments.RealModeIret16

```text
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.RetainedParameterStorage

```text
static System.Int32 Capacity = 56800
RetainedParameterStorage()
ScoreModern.RecoveredFragments.RetainedParameterStorage Clone()
System.Void CommitReplay(System.Byte[] active)
System.Void CopyBytes(System.Int32 source, System.Int32 destination, System.Int32 count)
System.Void Forget()
System.Void Load(System.Byte[] payload, System.Int32 trailerBytes, System.Byte[] producer, System.Byte[] active)
System.Boolean Matches(System.Byte[] active)
System.Void Opaque(System.Byte[] active)
System.Void Pack(System.Byte[] active)
System.Byte[] ReadKnown(System.Int32 offset, System.Int32 count)
System.Void Saved(System.Byte[] trailer)
System.Void TextInputPadding(System.Int32 offset)
System.Byte[] UndefinedWord()
System.Void WriteKnown(System.Int32 offset, System.Byte[] bytes)
```

## ScoreModern.RecoveredFragments.ScoreVersionControls

```text
static System.Single DrawingOffset(System.Boolean scor4, System.Single value)
static System.Single HarpOffset(System.Boolean scor4, System.Single value)
static System.Single Horizontal(System.Single value)
static System.Int16 MeterThicknessSelector(System.Boolean scor4, System.Single value)
static System.Boolean MeterThicknessSupported(System.Boolean scor4, System.Single value)
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult NumberSpacing(System.Single size, System.Single offset, System.Single staffScale)
static System.Boolean SlurEndpointMode(System.Boolean scor4, System.Single p16)
static System.Boolean SlurEndpointOverride(System.Boolean scor4, System.Single[] p)
static System.Single[] SlurParameters(System.Boolean scor4, System.Single[] parameters)
static System.Int16 SlurSubtype(System.Boolean scor4, System.Single p8, System.Single p16)
```

## ScoreModern.RecoveredFragments.ScorlasFilePlacement

```text
System.Int32 BottomShift
System.Int32 Cumulative
System.Int32 Delta
System.Int32 DscBottom
System.Int32 DscTop
System.Int32 NextAdvance
System.Int32 NormalizedHeight
System.Int32 OverallHeight
System.Int32 RawHeight
```

## ScoreModern.RecoveredFragments.ScorlasFilePlacementState

```text
ScorlasFilePlacementState()
ScoreModern.RecoveredFragments.ScorlasFilePlacement Add(System.Int16 minimum, System.Int16 maximum, System.Int32 guide)
```

## ScoreModern.RecoveredFragments.ScorlasGuideConversion

```text
static System.Int32 Convert(System.Single value, System.Boolean metric)
```

## ScoreModern.RecoveredFragments.ScorlasStatisticsArithmetic

```text
static System.Single Height(System.Int32 cumulativeHeight, System.Single printSize, System.Boolean metric)
```

## ScoreModern.RecoveredFragments.ScreenForwardResult

```text
System.UInt16 BX
System.UInt16 CS
System.UInt16 CX
System.UInt16 DS
System.UInt16 DX
System.UInt16 IP
System.UInt16 SP
ScreenForwardResult(System.UInt16 dx, System.UInt16 cx, System.UInt16 bx, System.UInt16 ds, System.UInt16 sp, System.UInt16 ip, System.UInt16 cs)
```

## ScoreModern.RecoveredFragments.ScreenHookDecisions

```text
static System.UInt16 DecrementSavedRow(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds)
static System.Boolean ShouldProbeCursor(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds, System.UInt16 ax)
static System.Boolean ShouldScroll(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds, System.Byte al, System.UInt16 dx)
```

## ScoreModern.RecoveredFragments.ScreenHookForwarding

```text
static ScoreModern.RecoveredFragments.ScreenForwardResult RestoreAndResolve(ScoreModern.RecoveredFragments.IScreenForwardMemory m, System.UInt16 ss, System.UInt16 sp, System.UInt16 codeSegment)
```

## ScoreModern.RecoveredFragments.ScreenHookInterrupt

```text
static ScoreModern.RecoveredFragments.SignedDivide16State Enter(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.ScreenHookInterruptEffects

```text
ScreenHookInterruptEffects(ScoreModern.RecoveredFragments.IScreenBiosTransferEffects biosEffects, ScoreModern.RecoveredFragments.ICapacityInterruptEffects otherInterruptEffects, System.UInt16 hookedSegment, System.UInt16 hookedOffset, System.UInt16 relocatedDataSegment)
ScoreModern.RecoveredFragments.SignedDivide16State Interrupt(System.Byte number, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScoreModern.RecoveredFragments.ScreenHookOutcome

```text
static ScoreModern.RecoveredFragments.ScreenHookOutcome ExternalControlTransfer = 1
static ScoreModern.RecoveredFragments.ScreenHookOutcome Forwarded = 0
System.Int32 value__
```

## ScoreModern.RecoveredFragments.ScreenHookResult

```text
ScoreModern.RecoveredFragments.ScreenHookOutcome Outcome
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.ScreenHookRoutine

```text
static ScoreModern.RecoveredFragments.ScreenHookResult Execute(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 dataSegment)
```

## ScoreModern.RecoveredFragments.SelectorCopy

```text
static System.Boolean Apply(System.Byte[] dataSegment, System.Int16 state, System.Boolean directionFlag)
```

## ScoreModern.RecoveredFragments.SelectorRoutine

```text
static System.Void Execute(ScoreModern.RecoveredFragments.ISelectorContext context, System.UInt16 parameterSegment, System.UInt16 parameterOffset)
```

## ScoreModern.RecoveredFragments.SignedDivide16

```text
static ScoreModern.RecoveredFragments.SignedDivide16Result Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScoreModern.RecoveredFragments.SignedDivide16Fault

```text
static ScoreModern.RecoveredFragments.SignedDivide16Fault None = 0
static ScoreModern.RecoveredFragments.SignedDivide16Fault QuotientOverflow = 2
System.Int32 value__
static ScoreModern.RecoveredFragments.SignedDivide16Fault ZeroDivisor = 1
```

## ScoreModern.RecoveredFragments.SignedDivide16Result

```text
System.UInt32 DefinedEFlagsMask
ScoreModern.RecoveredFragments.SignedDivide16Fault Fault
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.SignedDivide16State

```text
System.UInt16 AX
System.UInt16 BP
System.UInt16 BX
System.UInt16 CS
System.UInt16 CX
System.UInt16 DI
System.UInt16 DS
System.UInt16 DX
System.UInt32 EFlags
System.UInt16 ES
System.UInt16 IP
System.UInt16 SI
System.UInt16 SP
System.UInt16 SS
```

## ScoreModern.RecoveredFragments.SingleFixedDecimal

```text
static System.String Format(System.Single value, System.Int32 decimalPlaces)
```

## ScoreModern.RecoveredFragments.SlopedRaster

```text
static ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx)
static ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt32 eflags)
```

## ScoreModern.RecoveredFragments.SlurGeometry

```text
static ScoreModern.RecoveredFragments.SlurGeometryResult Build(System.Single[] parameters, ScoreModern.RecoveredFragments.SlurOriginContext context, System.Boolean printed, System.Single configuredControl, ScoreModern.RecoveredFragments.SlurVectorBuffer caller, System.Boolean rejectInvalidTrims)
```

## ScoreModern.RecoveredFragments.SlurGeometryResult

```text
ScoreModern.RecoveredFragments.SlurVectorBuffer Buffer
System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
System.Int32 Root
SlurGeometryResult()
```

## ScoreModern.RecoveredFragments.SlurLineWidth

```text
System.Single DefaultUnit { get; }
static System.Single Field(System.Int16 selector, System.Int16 basis, System.Single unit)
static System.String Format(System.Single value)
static System.Single Printed(System.Int16 selector)
static System.Single RawField(System.Int16 selector, System.Int16 basis, System.Single unit)
```

## ScoreModern.RecoveredFragments.SlurOriginContext

```text
System.Single LeftX
System.Single LeftY
System.Single RightY
System.Single Scale
System.Boolean Scor4
SlurOriginContext()
```

## ScoreModern.RecoveredFragments.SlurOrigins

```text
static ScoreModern.RecoveredFragments.SlurOriginState Distance(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext c)
static ScoreModern.RecoveredFragments.SlurOriginState Prepare(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext c)
```

## ScoreModern.RecoveredFragments.SlurOriginState

```text
System.Single CurveCenter
System.Single DeltaY
System.Single EffectiveStartFraction
System.Single Height
System.Single HorizontalSpan
System.Single LeftX
System.Single LeftY
System.Single Length
System.Int16 NumberWord
System.Int16 OffsetWord
System.Single OriginalLeft
System.Single OriginalRight
System.Single RightX
System.Single StartFraction
System.Single StopFraction
System.Single WorkingRight
SlurOriginState()
```

## ScoreModern.RecoveredFragments.SlurPlotRequest

```text
System.Int16 Kind
System.Int16 X
System.Int16 Y
SlurPlotRequest(System.Int16 k, System.Int16 x, System.Int16 y)
```

## ScoreModern.RecoveredFragments.SlurPostprocess

```text
static ScoreModern.RecoveredFragments.SlurVectorBuffer From(ScoreModern.RecoveredFragments.SlurSampleResult sample, ScoreModern.RecoveredFragments.SlurThicknessResult thickness, System.Boolean printed)
static System.Void Pattern(ScoreModern.RecoveredFragments.SlurVectorBuffer b, System.Single[] p, System.Single scale, System.Boolean printed)
static System.Void Rotate(ScoreModern.RecoveredFragments.SlurVectorBuffer b, ScoreModern.RecoveredFragments.SlurOriginState o)
static System.Void Trim(ScoreModern.RecoveredFragments.SlurVectorBuffer b, System.Single[] p, System.Single start, System.Single stop, System.Boolean printed)
```

## ScoreModern.RecoveredFragments.SlurSampleResult

```text
System.Int32 Count
System.Single Exponent
System.Single FinalIncrement
System.Single FinalX
System.Single HalfSteps
System.Single Height
ScoreModern.RecoveredFragments.SlurOriginState Origin
System.Single Step
System.Int16[] X
System.Int16[] Y
SlurSampleResult()
```

## ScoreModern.RecoveredFragments.SlurSampling

```text
static ScoreModern.RecoveredFragments.SlurSampleResult Build(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext context, System.Boolean printed)
static ScoreModern.RecoveredFragments.SlurSampleResult BuildState(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext context, ScoreModern.RecoveredFragments.SlurOriginState o, System.Boolean printed, System.Boolean trimmed)
```

## ScoreModern.RecoveredFragments.SlurThickness

```text
static ScoreModern.RecoveredFragments.SlurThicknessResult Build(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext context, ScoreModern.RecoveredFragments.SlurSampleResult sample, System.Single configuredControl)
static System.Void Reflect(System.Single[] p, ScoreModern.RecoveredFragments.SlurSampleResult sample, System.Int16[] y, System.Boolean printed)
```

## ScoreModern.RecoveredFragments.SlurThicknessResult

```text
System.Single EffectiveControl
System.Int32 FinalIncrement
System.Single FinalRamp
System.Single RampStep
System.Int32 Target
System.Single Thickness
System.Int16[] X
System.Int16[] Y
SlurThicknessResult()
```

## ScoreModern.RecoveredFragments.SlurVectorBuffer

```text
System.Single Angle
System.Int32 Base
System.Int32 Capacity
System.Single Cos
System.Int32 CurveEnd
System.Int32 End
System.Int32 SampleCount
System.Single Sin
ScoreModern.RecoveredFragments.SlurWordSegment X
ScoreModern.RecoveredFragments.SlurWordSegment Y
SlurVectorBuffer()
SlurVectorBuffer(System.UInt16 xSegment, System.UInt16 ySegment)
System.Void Copy(System.Int32 destination, System.Int32 source)
System.Int16[] ExtractX()
System.Int16[] ExtractY()
System.Void Move(System.Int32 destination, System.Int32 source, System.Int32 count)
```

## ScoreModern.RecoveredFragments.SlurVectorCommands

```text
static System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.SlurPlotRequest] Decode(ScoreModern.RecoveredFragments.SlurVectorBuffer b, System.Int32 root, System.Boolean appendRestore, System.Boolean printed)
```

## ScoreModern.RecoveredFragments.SlurWordSegment

```text
System.Int16 Item { get; set; }
System.Int32 Length { get; }
SlurWordSegment(System.Byte[] bytes, System.Int32 address)
```

## ScoreModern.RecoveredFragments.SpanBookkeeping

```text
static System.UInt16 Execute(ScoreModern.RecoveredFragments.ISpanBookkeepingMemory m, System.UInt16 ss, System.UInt16 sourceArgumentOffset, System.UInt16 ds, System.UInt16 remainingSi, System.UInt16 transferredDi)
```

## ScoreModern.RecoveredFragments.StaffHeightLayout

```text
static System.Single[] Targets(System.Single[] positions, System.Single[] sizes, System.Int32 maximum, System.Int32[] groups, System.Single oldHeight, System.Single newHeight, System.Boolean metric, System.Single[] physicalPositions)
```

## ScoreModern.RecoveredFragments.StringCopyResult

```text
System.UInt16 CX
System.UInt16 DI
System.UInt16 SI
StringCopyResult(System.UInt16 si, System.UInt16 di)
```

## ScoreModern.RecoveredFragments.StringFillResult

```text
System.UInt16 CX
System.UInt16 DI
StringFillResult(System.UInt16 di)
```

## ScoreModern.RecoveredFragments.StringOutputResult

```text
System.Boolean ReachedStringReturn
ScoreModern.RecoveredFragments.CharacterRoutineResult Result
```

## ScoreModern.RecoveredFragments.StringOutputRoutine

```text
static ScoreModern.RecoveredFragments.StringOutputResult Run(ScoreModern.RecoveredFragments.IStringOutputContext m, ScoreModern.RecoveredFragments.CharacterRoutineWords w, System.UInt16 currentSegment, System.UInt16 characterSegment)
```

## ScoreModern.RecoveredFragments.TextConverterFontState

```text
System.Boolean Forced
System.Int16 Word
System.Int32 MetricFace { get; }
System.Void Accept(System.Int16 token)
static ScoreModern.RecoveredFragments.TextConverterFontState Begin(System.Int16 overrideWord, System.Int16 previousWord)
```

## ScoreModern.RecoveredFragments.TextMetricArithmetic

```text
static System.Single Accumulate(System.Single total, System.Single advance)
static System.Single AddPosition(System.Int16 index, System.Single x, System.Single advance, System.Single units)
static System.Single Advance(System.Single metric, ScoreModern.RecoveredFragments.TextMetricScale scale, System.Boolean space)
static System.Single Finish(System.Single total, System.Single multiplier)
static System.Single Normalize(System.Int16 width, System.Single divisor)
static ScoreModern.RecoveredFragments.TextMetricScale Prepare(System.Single size, System.Single staff, System.Single divisor, System.Single factor, System.Single extraSpace)
static System.Single StrokeHorizontal(System.Single metric, System.Single horizontal, System.Single primitiveMetric)
```

## ScoreModern.RecoveredFragments.TextMetricScale

```text
System.Single Character
System.Single ExtraSpace
```

## ScoreModern.RecoveredFragments.TextParameterPreparation

```text
static System.Single[] Adjust(System.Single[] advances, System.Single delta)
static System.Single Interletter(System.Single total, System.Single factor, System.Single[] advances)
static System.Single OutputExtra(System.Single value, System.Boolean printed, System.Single units)
static ScoreModern.RecoveredFragments.TextPreparedParameters Prepare(System.Single[] p, System.Single x, System.Single modulus, System.Single glyphScale, System.Single units)
```

## ScoreModern.RecoveredFragments.TextPreparedParameters

```text
System.Single GlyphHorizontal
System.Single GlyphVertical
System.Single Horizontal
System.Single Origin
System.Single RestoredHorizontal
System.Single RestoredVertical
System.Single Rotation
System.Single Spacing
System.Single Vertical
```

## ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult

```text
System.Int16 ActiveRelationships
System.Int16[] Flags
System.Int16 RelationshipIndex
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow Window
TranspositionAccidentalTailResult()
```

## ScoreModern.RecoveredFragments.TranspositionClefState

```text
System.Int16 RetainedStaff
System.Int16 RetainedTarget
TranspositionClefState(System.Int16 staff, System.Int16 target)
```

## ScoreModern.RecoveredFragments.TranspositionCollection

```text
System.Int16 FirstKeyState
System.Nullable`1[System.Single] FirstNotePosition
System.Boolean HasExtendedBeam
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TranspositionRecord] Records
System.Collections.Generic.List`1[System.Int32] RelationshipItems
TranspositionCollection()
```

## ScoreModern.RecoveredFragments.TranspositionCollectorResult

```text
System.Collections.Generic.List`1[System.Single] CollectedBoundaries
System.Single ExpandedLeft
System.Single ExpandedRight
ScoreModern.RecoveredFragments.TranspositionCollection FirstPass
ScoreModern.RecoveredFragments.TranspositionPartition Partition
TranspositionCollectorResult()
```

## ScoreModern.RecoveredFragments.TranspositionCollectorState

```text
System.Single FirstNotePosition
TranspositionCollectorState(System.Single firstNotePosition)
```

## ScoreModern.RecoveredFragments.TranspositionInterval

```text
System.Int16 Diatonic
System.Int16 Key
System.Int16 SemitoneClass
System.Int16 Semitones
TranspositionInterval()
```

## ScoreModern.RecoveredFragments.TranspositionKeyRecordResult

```text
System.Int16 Diatonic
System.Int16 KeyOverride
System.Int16 NewKeyBase
System.Int16 OldKeyBase
System.Single[] Parameters
System.Int16 PitchBase
TranspositionKeyRecordResult()
```

## ScoreModern.RecoveredFragments.TranspositionMainPassResult

```text
System.Single StopPosition
ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult Tail
TranspositionMainPassResult()
```

## ScoreModern.RecoveredFragments.TranspositionMainState

```text
System.Int16[] AccidentalMarkers
System.Int16 Diatonic
System.Single ErasedCode
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow InitialWindow
System.Int16 KeyOverride
System.Int16 KindZeroGuard
System.Single RelationshipMargin
System.Single RelationshipWidth
System.Int16 Semitones
System.Single Sentinel
System.Int16 TargetKey
TranspositionMainState()
```

## ScoreModern.RecoveredFragments.TranspositionOperationResult

```text
ScoreModern.RecoveredFragments.TranspositionCollectorResult Collection
ScoreModern.RecoveredFragments.TranspositionMainPassResult Engine
System.Boolean InsertedKey
ScoreModern.RecoveredFragments.TranspositionInterval Interval
System.Boolean KeyQuestion
System.Collections.Generic.List`1[System.Single[]] Rows
System.Boolean SortMarksModified
System.Boolean WarningRequired
TranspositionOperationResult()
```

## ScoreModern.RecoveredFragments.TranspositionOperationSession

```text
ScoreModern.RecoveredFragments.TranspositionOperationResult Result
TranspositionOperationSession()
```

## ScoreModern.RecoveredFragments.TranspositionPartition

```text
System.Single[] Boundaries
System.Int16[] Kinds
TranspositionPartition()
```

## ScoreModern.RecoveredFragments.TranspositionPitchResult

```text
System.Nullable`1[System.Int16] AccidentalMarker
System.Single Pitch
System.Single Stem
TranspositionPitchResult()
```

## ScoreModern.RecoveredFragments.TranspositionRecord

```text
System.Int16 Accidental
System.Int16 Clef
System.Int32 Item
System.Int16 Kind
System.Nullable`1[System.Int16] Pitch
System.Single Position
TranspositionRecord()
```

## ScoreModern.RecoveredFragments.TranspositionRelationshipWindow

```text
System.Single EndHigh
System.Single EndLow
System.Single StartHigh
System.Single StartLow
```

## ScoreModern.RecoveredFragments.TranspositionRequest

```text
System.Boolean AddKey
System.Nullable`1[System.Int16] Clef
System.Int16[] ClefChanges
System.Boolean ForceKeyQuestion
System.Single Left
System.Single Right
System.Int16 Semitones
System.Single Staff
TranspositionRequest()
```

## ScoreModern.RecoveredFragments.TranspositionWorkspace

```text
System.Int16[] AccidentalMarkers
System.Boolean AlreadySorted
ScoreModern.RecoveredFragments.TranspositionClefState Clefs
ScoreModern.RecoveredFragments.TranspositionCollectorState Collector
System.Int16 KindZeroGuard
System.Int16[] PitchWords
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow Window
TranspositionWorkspace(ScoreModern.RecoveredFragments.TranspositionCollectorState collector, ScoreModern.RecoveredFragments.TranspositionClefState clefs, System.Int16[] pitchWords, System.Int16[] markers, System.Int16 guard, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow window, System.Boolean sorted)
```

## ScoreModern.RecoveredFragments.TupletBracket

```text
static ScoreModern.RecoveredFragments.TupletBracketResult Build(System.Single[] p, ScoreModern.RecoveredFragments.TupletBracketContext c)
```

## ScoreModern.RecoveredFragments.TupletBracketContext

```text
System.Single LeftX
System.Single LeftY
System.Single RightY
System.Single Scale
TupletBracketContext()
```

## ScoreModern.RecoveredFragments.TupletBracketPoint

```text
System.Int16 Tag
System.Single X
System.Single Y
TupletBracketPoint(System.Int16 tag, System.Single x, System.Single y)
```

## ScoreModern.RecoveredFragments.TupletBracketResult

```text
System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TupletBracketPoint] Points
TupletBracketResult()
```

## ScoreModern.RecoveredFragments.TupletGeometry

```text
static ScoreModern.RecoveredFragments.TupletPlacement Build(System.Int16 number, System.Int32 direction, System.Boolean cue, System.Single percent, System.Single verticalShift, System.Single scale, System.Single leftX, System.Single leftY, System.Single dx, System.Single dy)
static System.Single Center(System.Int16 number, System.Single scale, System.Single along, System.Single left)
static System.String NumberText(System.Int16 number)
static System.Single PrintedEm(System.Single fontScale, System.Single staffScale)
```

## ScoreModern.RecoveredFragments.TupletPlacement

```text
System.Single FontScale
System.Int16 Number
System.Single X
System.Single Y
```

## ScoreModern.RecoveredFragments.VerticalRaster

```text
static ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx)
static ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt32 eflags)
```

## ScoreModern.RecoveredFragments.VerticalRasterWords

```text
System.UInt16 AX
System.UInt16 BP
System.UInt16 BX
System.UInt16 CX
System.UInt32 DefinedEFlagsMask
System.UInt16 DI
System.UInt16 DX
System.UInt32 EFlags
System.UInt16 SI
```

