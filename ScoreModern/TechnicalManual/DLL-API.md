# ScoreModern.dll public member index

Generated from SCORE Modern 0.3.60 RC1 build 1950. Reflection displays CLR type names. Use the routine sources for argument semantics and limits. Inherited members and compiler-generated accessors are omitted. Enum names and values are shown explicitly.

Assembly: ScoreModern, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

Exported types: 337

## AutomaticPrintOffset

```text
ScoreModern.RecoveredFragments.AutomaticPrintOffset
static System.Int32 FromMinimumWord(System.Int16 minimumY)
static System.Int32 SelectBottom(System.Int32 preliminary)
```

## BeamAppearance

```text
ScoreModern.RecoveredFragments.BeamAppearance
Printed = 0
DosScreen = 1
```

## BeamBufferEdit

```text
ScoreModern.RecoveredFragments.BeamBufferEdit
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IPointBufferMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## BeamGeometry

```text
ScoreModern.RecoveredFragments.BeamGeometry
static System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.BeamShape] Build(System.Single[] p, ScoreModern.RecoveredFragments.BeamGeometryContext c, ScoreModern.RecoveredFragments.BeamAppearance appearance)
static System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.BeamShape] Build(System.Single[] p, ScoreModern.RecoveredFragments.BeamGeometryContext c, ScoreModern.RecoveredFragments.BeamAppearance appearance, ScoreModern.RecoveredFragments.BeamReference& reference)
```

## BeamGeometryContext

```text
ScoreModern.RecoveredFragments.BeamGeometryContext
.ctor()
Single LeftVertical
Single Scale
System.Func`2[System.Single,System.Single] Vertical
```

## BeamReference

```text
ScoreModern.RecoveredFragments.BeamReference
Boolean Cue
Single Dx
Single Dy
Single LeftX
Single LeftY
Single StaffScale
```

## BeamShape

```text
ScoreModern.RecoveredFragments.BeamShape
.ctor()
Int32 Group
ScoreModern.RecoveredFragments.BeamVertex[] Vertices
```

## BeamStemAdjustment

```text
ScoreModern.RecoveredFragments.BeamStemAdjustment
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Boolean force, ScoreModern.RecoveredFragments.PageCoordinates tables, System.Int32 maximumStaff, System.Int32 onlyBeam, System.Collections.Generic.ISet`1[System.Int32] selectedBeams)
```

## BeamVertex

```text
ScoreModern.RecoveredFragments.BeamVertex
.ctor(System.Single x, System.Single y)
Single X
Single Y
```

## BiosKeyReader

```text
ScoreModern.RecoveredFragments.BiosKeyReader
static System.Void Execute(ScoreModern.RecoveredFragments.IBiosKeyContext context, System.UInt16 asciiSegment, System.UInt16 asciiOffset, System.UInt16 scanSegment, System.UInt16 scanOffset)
```

## BufferedBeamLinePipeline

```text
ScoreModern.RecoveredFragments.BufferedBeamLinePipeline
static ScoreModern.RecoveredFragments.LineDriverBodyResult Execute(ScoreModern.RecoveredFragments.IByteOrResultMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state, System.UInt16 driverSegment)
```

## BufferedCoordinateDecode

```text
ScoreModern.RecoveredFragments.BufferedCoordinateDecode
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IPointBufferMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## BufferedDrawControl

```text
ScoreModern.RecoveredFragments.BufferedDrawControl
static ScoreModern.RecoveredFragments.BufferedDrawWords DecodeMarker(ScoreModern.RecoveredFragments.IBufferedDrawMemory m, System.UInt16 ds, System.UInt16 ax)
static ScoreModern.RecoveredFragments.BufferedDrawWords SelectRasterPath(ScoreModern.RecoveredFragments.IBufferedDrawMemory m, System.UInt16 ds)
static ScoreModern.RecoveredFragments.BufferedDrawWords UpdatePosition(ScoreModern.RecoveredFragments.IBufferedDrawMemory m, System.UInt16 ds, System.UInt16 ax)
```

## BufferedDrawWords

```text
ScoreModern.RecoveredFragments.BufferedDrawWords
UInt16 AX
Boolean Branch
UInt16 CX
```

## BufferedPointCaller

```text
ScoreModern.RecoveredFragments.BufferedPointCaller
static ScoreModern.RecoveredFragments.BufferedPointCallerResult Enter(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 converterSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteCapacityAtComparison(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteCapacityAtComparisonThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 converterSegment)
static ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteWithCapacityEffects(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects fpu, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 converterSegment, System.UInt16 stringSegment, System.UInt16 followupSegment)
```

## BufferedPointCallerResult

```text
ScoreModern.RecoveredFragments.BufferedPointCallerResult
ScoreModern.RecoveredFragments.BufferedPointOutcome Outcome
UInt16 RequestedCalleeOffset
Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## BufferedPointOutcome

```text
ScoreModern.RecoveredFragments.BufferedPointOutcome
Completed = 0
CapacityHandlerRequired = 1
NormalPathReady = 2
ExternalControlTransfer = 3
```

## CapacityCallKind

```text
ScoreModern.RecoveredFragments.CapacityCallKind
StringOutput = 0
Followup = 1
```

## CapacityCharacterNativeEffects

```text
ScoreModern.RecoveredFragments.CapacityCharacterNativeEffects
.ctor(ScoreModern.RecoveredFragments.ICapacityInterruptEffects effects)
ScoreModern.RecoveredFragments.SignedDivide16State CallCharacter(ScoreModern.RecoveredFragments.ICapacityStringMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## CapacityCharacterOutput

```text
ScoreModern.RecoveredFragments.CapacityCharacterOutput
static ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## CapacityDeviceCallees

```text
ScoreModern.RecoveredFragments.CapacityDeviceCallees
static ScoreModern.RecoveredFragments.CapacityDeviceResult KeyStatus(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult KeyStatusThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult Poll(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult PollThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## CapacityDeviceOutcome

```text
ScoreModern.RecoveredFragments.CapacityDeviceOutcome
ReachedReturn = 0
ExternalControlTransfer = 1
```

## CapacityDeviceResult

```text
ScoreModern.RecoveredFragments.CapacityDeviceResult
ScoreModern.RecoveredFragments.CapacityDeviceOutcome Outcome
Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## CapacityFollowup

```text
ScoreModern.RecoveredFragments.CapacityFollowup
static ScoreModern.RecoveredFragments.CapacityFollowupResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityFollowupEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 calleeSegment)
static ScoreModern.RecoveredFragments.CapacityFollowupResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityFollowupEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 calleeSegment)
```

## CapacityFollowupCallKind

```text
ScoreModern.RecoveredFragments.CapacityFollowupCallKind
KeyStatus = 0
PromptOutput = 1
KeyRead = 2
Poll = 3
```

## CapacityFollowupNativeEffects

```text
ScoreModern.RecoveredFragments.CapacityFollowupNativeEffects
.ctor(ScoreModern.RecoveredFragments.ICapacityInterruptEffects interruptEffects, ScoreModern.RecoveredFragments.ICapacityCharacterEffects characterEffects, System.UInt16 characterTargetSegment)
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityFollowupCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## CapacityFollowupOutcome

```text
ScoreModern.RecoveredFragments.CapacityFollowupOutcome
ReachedReturn = 0
ExternalControlTransfer = 1
UnverifiedBranch = 2
```

## CapacityFollowupResult

```text
ScoreModern.RecoveredFragments.CapacityFollowupResult
ScoreModern.RecoveredFragments.CapacityFollowupOutcome Outcome
Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## CapacityKeyReader

```text
ScoreModern.RecoveredFragments.CapacityKeyReader
static ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityKeyMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityKeyMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## CapacityMessageHandler

```text
ScoreModern.RecoveredFragments.CapacityMessageHandler
static ScoreModern.RecoveredFragments.CapacityMessageResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
static ScoreModern.RecoveredFragments.CapacityMessageResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICapacityCallEffects effects, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 stringSegment, System.UInt16 followupSegment)
```

## CapacityMessageNativeEffects

```text
ScoreModern.RecoveredFragments.CapacityMessageNativeEffects
.ctor(ScoreModern.RecoveredFragments.ICapacityInterruptEffects interruptEffects, ScoreModern.RecoveredFragments.ICapacityCharacterEffects characterEffects, System.UInt16 sharedCalleeSegment, System.UInt16 characterTargetSegment)
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
static ScoreModern.RecoveredFragments.CapacityMessageNativeEffects CreateWithNativeCharacters(ScoreModern.RecoveredFragments.ICapacityInterruptEffects interruptEffects, System.UInt16 sharedCalleeSegment, System.UInt16 characterTargetSegment)
```

## CapacityMessageOutcome

```text
ScoreModern.RecoveredFragments.CapacityMessageOutcome
ReachedReturn = 0
ExternalControlTransfer = 1
```

## CapacityMessageResult

```text
ScoreModern.RecoveredFragments.CapacityMessageResult
ScoreModern.RecoveredFragments.CapacityMessageOutcome Outcome
Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## CapacityStringOutput

```text
ScoreModern.RecoveredFragments.CapacityStringOutput
static ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityCharacterEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 characterSegment)
static ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityStringMemory m, ScoreModern.RecoveredFragments.ICapacityCharacterEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 characterSegment)
```

## CharacterAlternateOutput

```text
ScoreModern.RecoveredFragments.CharacterAlternateOutput
static ScoreModern.RecoveredFragments.CharacterBiosWords Run(ScoreModern.RecoveredFragments.ICharacterBiosContext context, ScoreModern.RecoveredFragments.CharacterBiosWords w)
```

## CharacterBiosWords

```text
ScoreModern.RecoveredFragments.CharacterBiosWords
UInt16 AX
UInt16 BX
UInt16 CX
UInt16 DX
UInt16 SP
UInt16 SS
```

## CharacterDecisionResult

```text
ScoreModern.RecoveredFragments.CharacterDecisionResult
.ctor(System.Byte al, System.UInt16 bx, System.Boolean branch)
Byte AL
Boolean Branch
UInt16 BX
```

## CharacterEntryWords

```text
ScoreModern.RecoveredFragments.CharacterEntryWords
UInt16 BP
UInt16 DI
UInt16 ES
UInt16 SP
```

## CharacterOutputDecisions

```text
ScoreModern.RecoveredFragments.CharacterOutputDecisions
static ScoreModern.RecoveredFragments.CharacterDecisionResult ProbeAlternateGate(ScoreModern.RecoveredFragments.ICharacterDecisionMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 di, System.UInt16 bx)
static ScoreModern.RecoveredFragments.CharacterDecisionResult ProbeBackspace(ScoreModern.RecoveredFragments.ICharacterDecisionMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 di, System.Byte dl, System.UInt16 bx)
```

## CharacterOutputFrame

```text
ScoreModern.RecoveredFragments.CharacterOutputFrame
static ScoreModern.RecoveredFragments.CharacterEntryWords Enter(ScoreModern.RecoveredFragments.ICharacterFrameMemory m, System.UInt16 ss, System.UInt16 sp, System.UInt16 bp, System.UInt16 di)
static ScoreModern.RecoveredFragments.CharacterReturnWords Leave(ScoreModern.RecoveredFragments.ICharacterFrameMemory m, System.UInt16 ss, System.UInt16 sp)
```

## CharacterOutputRoutine

```text
ScoreModern.RecoveredFragments.CharacterOutputRoutine
static ScoreModern.RecoveredFragments.CharacterRoutineResult Run(ScoreModern.RecoveredFragments.ICharacterRoutineContext m, ScoreModern.RecoveredFragments.CharacterRoutineWords w)
```

## CharacterReturnWords

```text
ScoreModern.RecoveredFragments.CharacterReturnWords
UInt16 BP
UInt16 CS
UInt16 DI
UInt16 IP
UInt16 SP
```

## CharacterRoutineResult

```text
ScoreModern.RecoveredFragments.CharacterRoutineResult
UInt16 CS
UInt16 IP
ScoreModern.RecoveredFragments.CharacterRoutineWords Words
```

## CharacterRoutineWords

```text
ScoreModern.RecoveredFragments.CharacterRoutineWords
UInt16 AX
UInt16 BP
UInt16 BX
UInt16 CX
UInt16 DI
UInt16 DS
UInt16 DX
UInt16 ES
UInt16 SI
UInt16 SP
UInt16 SS
```

## ClippingCpu

```text
ScoreModern.RecoveredFragments.ClippingCpu
static ScoreModern.RecoveredFragments.SignedDivide16Result Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ConsumerContextSaver

```text
ScoreModern.RecoveredFragments.ConsumerContextSaver
static System.UInt16 Execute(ScoreModern.RecoveredFragments.IConsumerContextMemory m, System.UInt16 stackSegment, System.UInt16 entrySp, System.UInt16 entryBp, System.UInt16 entrySi, System.UInt16 entryDi, System.UInt16 entryDs)
```

## CoordinateConversion

```text
ScoreModern.RecoveredFragments.CoordinateConversion
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICoordinateFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.SignedDivide16State ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.ICoordinateFpuEffects fpu, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## CoordinateWordClamp

```text
ScoreModern.RecoveredFragments.CoordinateWordClamp
static System.UInt32 Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt32 flags)
```

## DatTextLookup

```text
ScoreModern.RecoveredFragments.DatTextLookup
static ScoreModern.RecoveredFragments.DatTextLookupWords Read(ScoreModern.RecoveredFragments.IDatTextLookupMemory m, System.UInt16 ds)
```

## DatTextLookupWords

```text
ScoreModern.RecoveredFragments.DatTextLookupWords
UInt16 AX
UInt16 BX
UInt16 DX
```

## DirectionalStringBlocks

```text
ScoreModern.RecoveredFragments.DirectionalStringBlocks
static ScoreModern.RecoveredFragments.StringCopyResult Copy(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds, System.UInt16 si, System.UInt16 es, System.UInt16 di, System.UInt16 count, System.Boolean directionFlag)
static ScoreModern.RecoveredFragments.StringFillResult Fill(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 es, System.UInt16 di, System.UInt16 ax, System.UInt16 count, System.Boolean directionFlag)
```

## EndingBracket

```text
ScoreModern.RecoveredFragments.EndingBracket
static ScoreModern.RecoveredFragments.EndingBracketResult Build(System.Single[] p, ScoreModern.RecoveredFragments.TupletBracketContext c, System.Boolean printed, System.Boolean tomsTenth)
static ScoreModern.RecoveredFragments.TupletBracketPoint DotPoint(System.Single x, System.Single y, System.Single scale, ScoreModern.RecoveredFragments.EndingBracketResult r)
```

## EndingBracketResult

```text
ScoreModern.RecoveredFragments.EndingBracketResult
.ctor()
Single DotX
Single DotY
Boolean HasDot
System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TupletBracketPoint] Points
```

## EqualStepRaster

```text
ScoreModern.RecoveredFragments.EqualStepRaster
static ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx, System.UInt16 si)
static ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx, System.UInt16 si, System.UInt32 eflags)
```

## ExternalVgaFont

```text
ScoreModern.RecoveredFragments.ExternalVgaFont
.ctor(System.Byte[] data)
System.Byte Row(System.Byte character, System.Int32 line)
```

## FiniteBinary32Integer64

```text
ScoreModern.RecoveredFragments.FiniteBinary32Integer64
static System.Boolean TryConvert(System.UInt32 bits, System.Int64& value)
```

## FormatterWordWriter

```text
ScoreModern.RecoveredFragments.FormatterWordWriter
static System.Void Execute(ScoreModern.RecoveredFragments.IFormatterWordContext context, System.UInt16 low, System.UInt16 high)
```

## HorizontalRaster

```text
ScoreModern.RecoveredFragments.HorizontalRaster
static ScoreModern.RecoveredFragments.HorizontalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx)
static ScoreModern.RecoveredFragments.HorizontalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 cx, System.UInt16 dx, System.UInt32 eflags)
```

## HorizontalRasterWords

```text
ScoreModern.RecoveredFragments.HorizontalRasterWords
UInt16 AX
UInt16 BX
UInt16 CX
UInt32 DefinedEFlagsMask
UInt16 DI
UInt16 DX
UInt32 EFlags
UInt16 SI
```

## IBiosKeyContext

```text
ScoreModern.RecoveredFragments.IBiosKeyContext
System.UInt16 ReadBiosKey()
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
```

## IBufferedDrawMemory

```text
ScoreModern.RecoveredFragments.IBufferedDrawMemory
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IBufferedPointFpuEffects

```text
ScoreModern.RecoveredFragments.IBufferedPointFpuEffects
System.Void LoadBinary32(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
```

## IByteOrResultMemory

```text
ScoreModern.RecoveredFragments.IByteOrResultMemory
System.Byte OrByteAndGetResult(System.UInt16 segment, System.UInt16 offset, System.Byte mask)
```

## ICapacityCallEffects

```text
ScoreModern.RecoveredFragments.ICapacityCallEffects
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ICapacityCharacterEffects

```text
ScoreModern.RecoveredFragments.ICapacityCharacterEffects
ScoreModern.RecoveredFragments.SignedDivide16State CallCharacter(ScoreModern.RecoveredFragments.ICapacityStringMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ICapacityFollowupEffects

```text
ScoreModern.RecoveredFragments.ICapacityFollowupEffects
ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityFollowupCallKind kind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ICapacityInterruptEffects

```text
ScoreModern.RecoveredFragments.ICapacityInterruptEffects
ScoreModern.RecoveredFragments.SignedDivide16State Interrupt(System.Byte number, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ICapacityKeyMemory

```text
ScoreModern.RecoveredFragments.ICapacityKeyMemory
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
```

## ICapacityStringMemory

```text
ScoreModern.RecoveredFragments.ICapacityStringMemory
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
```

## ICharacterBiosContext

```text
ScoreModern.RecoveredFragments.ICharacterBiosContext
System.Void Interrupt10(ScoreModern.RecoveredFragments.CharacterBiosWords& words)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ICharacterDecisionMemory

```text
ScoreModern.RecoveredFragments.ICharacterDecisionMemory
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## ICharacterFrameMemory

```text
ScoreModern.RecoveredFragments.ICharacterFrameMemory
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ICharacterRoutineContext

```text
ScoreModern.RecoveredFragments.ICharacterRoutineContext
System.Void Interrupt10(ScoreModern.RecoveredFragments.CharacterRoutineWords& words, System.UInt16 returnOffset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## IConsumerContextMemory

```text
ScoreModern.RecoveredFragments.IConsumerContextMemory
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## ICoordinateFpuEffects

```text
ScoreModern.RecoveredFragments.ICoordinateFpuEffects
System.Void LoadControlWord(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
System.Void StoreControlWord(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
System.Void StoreInteger64AndPop(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, System.UInt16 segment, System.UInt16 offset)
System.Void Wait()
```

## IDatTextLookupMemory

```text
ScoreModern.RecoveredFragments.IDatTextLookupMemory
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## IFormatterWordContext

```text
ScoreModern.RecoveredFragments.IFormatterWordContext
System.UInt16 ReadWord(System.UInt16 offset)
System.Void WriteByte(System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 offset, System.UInt16 value)
```

## IHorizontalRasterMemory

```text
ScoreModern.RecoveredFragments.IHorizontalRasterMemory
System.Void OrByte(System.UInt16 segment, System.UInt16 offset, System.Byte mask)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IMode12GlyphSource

```text
ScoreModern.RecoveredFragments.IMode12GlyphSource
System.Byte Row(System.Byte character, System.Int32 line)
```

## IMode12Surface

```text
ScoreModern.RecoveredFragments.IMode12Surface
System.Void Scroll(System.Int32 top, System.Int32 left, System.Int32 bottom, System.Int32 right, System.Int32 lines, System.Byte background)
System.Void WriteGlyph(System.Int32 row, System.Int32 column, System.Byte character, System.Byte color)
```

## IncludedEpsTransform

```text
ScoreModern.RecoveredFragments.IncludedEpsTransform
.ctor()
Single BoxX
Single BoxY
Boolean Rotate
Single Rotation
Single ScaleX
Single ScaleY
Single X
Single Y
static ScoreModern.RecoveredFragments.IncludedEpsTransform Prepare(ScoreModern.RecoveredFragments.PageItemContext page, System.Single staffBase, System.Single vertical, System.Single horizontalSize, System.Single verticalSize, System.Single boxLeft, System.Single boxBottom, System.Single rotation, System.Single printSize, System.Int32 leftOffset, System.Int32 bottomOffset)
```

## IncludedPostScriptVariables

```text
ScoreModern.RecoveredFragments.IncludedPostScriptVariables
.ctor()
Single BottomOffset
Single GlobalSize
Single LeftOffset
Single LineWidth
Single PointsLevel
Single PP3
Single PP4
Single PP5
Single PP6
Single StaffLevel
Single StaffSize
static ScoreModern.RecoveredFragments.IncludedPostScriptVariables Prepare(ScoreModern.RecoveredFragments.PageItemContext item, System.Single staffLevel, System.Single p5, System.Single p6, System.Single printSize, System.Single lineWidth, System.Int32 leftOffset, System.Int32 bottomOffset)
```

## InputAttachedMarks

```text
ScoreModern.RecoveredFragments.InputAttachedMarks
static System.Single DefaultValue(System.Int32 mark, System.Single pitch, System.Single stem)
```

## InputAutomaticBeamMeterException

```text
ScoreModern.RecoveredFragments.InputAutomaticBeamMeterException
.ctor(System.Int32 top, System.Int32 bottom)
```

## InputAutomaticBeams

```text
ScoreModern.RecoveredFragments.InputAutomaticBeams
static System.Single[] Durations(System.Collections.Generic.IList`1[System.Single[]] rows)
static System.Single[] Groups(System.Collections.Generic.IList`1[System.Single[]] rows, System.Single[] durations, System.Single groupControl, System.Int32 skip)
```

## InputBeamConstructionResult

```text
ScoreModern.RecoveredFragments.InputBeamConstructionResult
.ctor()
Int32[] ForcedStems
System.Collections.Generic.List`1[System.Single[]] Records
```

## InputBeamFit

```text
ScoreModern.RecoveredFragments.InputBeamFit
static ScoreModern.RecoveredFragments.InputBeamFitResult Fit(System.Single[] pitch, System.Single[] x, System.Int32 direction, System.Int32 flags, System.Single slopeLimit)
```

## InputBeamFitResult

```text
ScoreModern.RecoveredFragments.InputBeamFitResult
.ctor()
Int32 Direction
Single End
Single Start
Single UnquantizedEnd
Single UnquantizedStart
```

## InputBeamLevels

```text
ScoreModern.RecoveredFragments.InputBeamLevels
static System.Collections.Generic.List`1[System.Single[]] Pack(System.Single[] prototype, System.Int32[] original, System.Single[] x)
```

## InputBeamStemCompletion

```text
ScoreModern.RecoveredFragments.InputBeamStemCompletion
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] items, System.Int32 firstItem, ScoreModern.RecoveredFragments.InputEndpointStaffContext context)
```

## InputChordDots

```text
ScoreModern.RecoveredFragments.InputChordDots
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows)
static System.Void ApplyRange(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int32 first, System.Int32 countLimit)
```

## InputChordLayout

```text
ScoreModern.RecoveredFragments.InputChordLayout
static System.Int32 Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int32 start, System.Int32 end)
```

## InputChordPitchRecords

```text
ScoreModern.RecoveredFragments.InputChordPitchRecords
static ScoreModern.RecoveredFragments.InputPitchResult CompileSpacedNaturals(System.Single[] source, System.Single staff, System.Single left, System.Single right)
```

## InputChordRhythmRecords

```text
ScoreModern.RecoveredFragments.InputChordRhythmRecords
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Single[] encoded, System.Single left, System.Single right, System.Single scale, System.Int32[] stems)
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Single[] encoded, System.Single left, System.Single right, System.Single scale, System.Int32[] stems, System.Boolean proportional, System.Single exponent, System.Nullable`1[System.Single] referenceStart, System.Nullable`1[System.Int16] retainedGraceWord, System.Nullable`1[System.Single] rememberedLeft)
```

## InputDynamics

```text
ScoreModern.RecoveredFragments.InputDynamics
static System.Single[] Create(System.Int32 symbol, System.Single[] note, System.Single staff, System.Single preference)
```

## InputEditInverse

```text
ScoreModern.RecoveredFragments.InputEditInverse
static System.String Number(System.Single n)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted, System.Boolean documentedRepeatedMarks)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted, System.Boolean documentedRepeatedMarks, System.Boolean reproduceInvalid)
static ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 staff, System.Single left, System.Single right, System.Boolean sorted, System.Boolean documentedRepeatedMarks, System.Boolean reproduceInvalid, System.Nullable`1[System.Single] verifiedTrailingWord)
```

## InputEditInverseResult

```text
ScoreModern.RecoveredFragments.InputEditInverseResult
.ctor()
System.String Header
Int32 InitialClef
Single Left
System.String[] Lines
System.String Positions
System.Collections.Generic.List`1[System.Single[]] Prefix
Single Right
System.Collections.Generic.List`1[System.Single[]] Selected
Int32 Staff
```

## InputEndpointProjection

```text
ScoreModern.RecoveredFragments.InputEndpointProjection
static ScoreModern.RecoveredFragments.InputEndpointResult Decode(System.Single first, System.Single last, System.Int32 stage, System.Collections.Generic.IList`1[System.Single[]] records, System.Int32[] noteToItem, System.Int32[] stems, System.Single beginning, System.Single end, ScoreModern.RecoveredFragments.InputEndpointStaffContext context)
```

## InputEndpointRepromptException

```text
ScoreModern.RecoveredFragments.InputEndpointRepromptException
.ctor()
```

## InputEndpointResult

```text
ScoreModern.RecoveredFragments.InputEndpointResult
.ctor()
Int16 EndFlag
Int16 EndItem
Int16 EndNote
Single EndPitch
Single EndX
Int16 Number
Int16 StartFlag
Int16 StartItem
Int16 StartNote
Single StartPitch
Single StartX
Int16 Stem
```

## InputEndpointStaffContext

```text
ScoreModern.RecoveredFragments.InputEndpointStaffContext
.ctor(System.Single[] bases, System.Single[] scales, System.Int32 staff)
System.Void Adjust(System.Single[] row, System.Single& pitch)
System.Void AdjustBeamStem(System.Single[] row, System.Int32 direction, System.Single& extension)
ScoreModern.RecoveredFragments.InputEndpointStaffContext ForStaff(System.Int32 value)
```

## InputEndpointText

```text
ScoreModern.RecoveredFragments.InputEndpointText
static ScoreModern.RecoveredFragments.InputEndpointTokens Parse(System.String text, System.Boolean beams, System.Int32 meterTop, System.Int32 meterBottom)
static ScoreModern.RecoveredFragments.InputEndpointTokens Parse(System.String text, System.Boolean beams, System.Int32 meterTop, System.Int32 meterBottom, ScoreModern.RecoveredFragments.InputMacroContext context)
```

## InputEndpointTokens

```text
ScoreModern.RecoveredFragments.InputEndpointTokens
.ctor()
Boolean Automatic
Single GroupControl
Single[] Pairs
Int32 Skip
```

## InputListExpansion

```text
ScoreModern.RecoveredFragments.InputListExpansion
static System.Void Repeat(System.Collections.Generic.List`1[System.Single] tokens, System.Int32 entries, System.Int32 total, System.Boolean keepAccidentals)
static System.Void RepeatBeforePrevious(System.Collections.Generic.List`1[System.Single] tokens, System.Boolean keepAccidentals)
```

## InputMacroContext

```text
ScoreModern.RecoveredFragments.InputMacroContext
.ctor()
System.Void AllowReplacement(System.Char name)
System.Void Clear()
System.Void ClearReplacementAllowances()
ScoreModern.RecoveredFragments.InputMacroContext Clone()
System.Void ReplaceFrom(ScoreModern.RecoveredFragments.InputMacroContext source)
System.Collections.Generic.IEnumerable`1[System.Collections.Generic.KeyValuePair`2[System.Char,System.Single[]]] Entries
Int32 StoredDefinitionCount
```

## InputMainNoteReconstruction

```text
ScoreModern.RecoveredFragments.InputMainNoteReconstruction
static System.Collections.Generic.List`1[System.Single[]] Build(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32 stem, System.Nullable`1[System.Int16] retainedGraceWord, System.Boolean originalInvalidStates)
```

## InputMarksCompiler

```text
ScoreModern.RecoveredFragments.InputMarksCompiler
static ScoreModern.RecoveredFragments.InputMarksResult Apply(System.Collections.Generic.IList`1[System.Single[]] original, System.Int32[] map, System.Single[] tokens, System.Int32[] opposite, System.Single staff, System.Single beginning, System.Single end, System.Single followingStaffScale, System.Single dynamics)
static ScoreModern.RecoveredFragments.InputMarksResult Apply(System.Collections.Generic.IList`1[System.Single[]] original, System.Int32[] map, System.Single[] tokens, System.Int32[] opposite, System.Single staff, System.Single beginning, System.Single end, System.Single followingStaffScale, System.Single dynamics, System.Boolean reproduceRetainedDescriptor)
```

## InputMarksResult

```text
ScoreModern.RecoveredFragments.InputMarksResult
.ctor()
Int32 FirstOrnament
System.Collections.Generic.List`1[System.Single[]] Records
```

## InputMarkText

```text
ScoreModern.RecoveredFragments.InputMarkText
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context)
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context, System.Boolean rejectMultiDigitF)
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text)
static ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String text, System.Boolean rejectMultiDigitF)
```

## InputMarkTokens

```text
ScoreModern.RecoveredFragments.InputMarkTokens
.ctor()
Int32[] Opposite
Single[] Values
```

## InputMotiveReplacementException

```text
ScoreModern.RecoveredFragments.InputMotiveReplacementException
.ctor(System.Char name)
Char Name
```

## InputPitchRecords

```text
ScoreModern.RecoveredFragments.InputPitchRecords
static ScoreModern.RecoveredFragments.InputPitchResult Compile(System.Single[] tokens, System.Single staff, System.Int32 clef, System.Single left, System.Single right, System.Boolean insetLeft)
static ScoreModern.RecoveredFragments.InputPitchResult CompileSingleNotes(System.Single[] tokens, System.Single staff, System.Int32 clef, System.Single left, System.Single right, System.Boolean insetLeft)
```

## InputPitchResult

```text
ScoreModern.RecoveredFragments.InputPitchResult
.ctor()
Int32 Events
Int32 FinalClef
System.Collections.Generic.List`1[System.Int32] ForcedStems
Single Left
System.Collections.Generic.List`1[System.Int32] NoteIndices
System.Collections.Generic.List`1[System.Single[]] Records
```

## InputPitchState

```text
ScoreModern.RecoveredFragments.InputPitchState
.ctor()
Int32 BarSpan
Int32 KeySignature
Int32 MeterBottom
Int32 MeterTop
Int32 Octave
Int32 Previous
Int32 PreviousDegree
Boolean Proximity
```

## InputPitchToken

```text
ScoreModern.RecoveredFragments.InputPitchToken
static System.Nullable`1[System.Single] Parse(System.String text, ScoreModern.RecoveredFragments.InputPitchState state)
```

## InputProximityDelimiterException

```text
ScoreModern.RecoveredFragments.InputProximityDelimiterException
.ctor(System.Int32 caret)
Int32 Caret
```

## InputRealReader

```text
ScoreModern.RecoveredFragments.InputRealReader
static System.Single Read(System.String text, System.Int32& at)
```

## InputRecordCompaction

```text
ScoreModern.RecoveredFragments.InputRecordCompaction
static System.Byte[] Compact(System.Single[] parameters)
```

## InputReferenceSpacing

```text
ScoreModern.RecoveredFragments.InputReferenceSpacing
.ctor(System.Collections.Generic.IList`1[System.Single[]] staff)
.ctor(System.Collections.Generic.IList`1[System.Single[]] staff, System.Action`2[System.Int32,System.Single] normalizedPosition)
Boolean HasClefAtZero
```

## InputRhythmDecoder

```text
ScoreModern.RecoveredFragments.InputRhythmDecoder
static ScoreModern.RecoveredFragments.InputRhythmValue Decode(System.Single encoded)
```

## InputRhythmRecords

```text
ScoreModern.RecoveredFragments.InputRhythmRecords
static System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Single[] encoded, System.Single left, System.Single right, System.Single scale, System.Boolean proportional, System.Single exponent, System.Int32[] forcedStems, System.Nullable`1[System.Single] referenceStart, System.Nullable`1[System.Single] rememberedLeft)
```

## InputRhythmToken

```text
ScoreModern.RecoveredFragments.InputRhythmToken
static System.Single Parse(System.String text)
static System.Single Parse(System.String text, System.Boolean originalMantissaWrap)
```

## InputRhythmValue

```text
ScoreModern.RecoveredFragments.InputRhythmValue
Single Denominator
Single DotControl
Single Duration
Single FlagCount
```

## InputSetupBounds

```text
ScoreModern.RecoveredFragments.InputSetupBounds
static ScoreModern.RecoveredFragments.InputSetupBoundsResult Find(System.Collections.Generic.IList`1[System.Single[]] records, ScoreModern.RecoveredFragments.InputSetupState state)
```

## InputSetupBoundsResult

```text
ScoreModern.RecoveredFragments.InputSetupBoundsResult
.ctor()
Int32 Clef
System.String Key
Int32 Left
Int32 MeterBottom
Int32 MeterTop
Int32 Right
```

## InputSetupState

```text
ScoreModern.RecoveredFragments.InputSetupState
.ctor()
System.String Key
Int32 LeftEnd
Int32 LeftPosition
Int32 MeterBottom
Int32 MeterTop
Int32 RightEnd
Int32 RightPosition
Int32 Selected
Single Size
Int32 Spacing
Int32 Staff
Int32 Vertical
System.Void KeyText(System.String key)
System.Void MoveField(System.Int32 delta)
System.Boolean Numeric(System.Single value)
static System.String PresetName(System.Int32 scan)
System.Void SpacingLetter(System.Char letter)
System.Single[] StaffRecord()
```

## InputSlurConstruction

```text
ScoreModern.RecoveredFragments.InputSlurConstruction
static System.Collections.Generic.List`1[System.Single[]] Build(System.Collections.Generic.IList`1[System.Single[]] original, ScoreModern.RecoveredFragments.InputEndpointResult e, System.Single staff, System.Int32 boundary, System.Int16 tiesMode, System.Single end, ScoreModern.RecoveredFragments.InputEndpointStaffContext context)
static System.Single InitialHeight(System.Single left, System.Single right, System.Single y1, System.Single y2, System.Single offset, System.Single height)
```

## InputStaccatoCentering

```text
ScoreModern.RecoveredFragments.InputStaccatoCentering
static System.Boolean Apply(System.Single[] p, System.Single[] staffScales)
```

## InputStaffStage

```text
ScoreModern.RecoveredFragments.InputStaffStage
Pitches = 0
Rhythms = 1
Marks = 2
Beams = 3
Slurs = 4
Complete = 5
```

## InputStaffTransaction

```text
ScoreModern.RecoveredFragments.InputStaffTransaction
.ctor(System.Single staff, System.Int32 clef, System.Single beginning, System.Single end, System.Single scale, System.Single followingScale, System.Single beam, System.Single dynamics, System.Int16 ties, System.Boolean proportional, System.Single exponent)
.ctor(System.Single staff, System.Int32 clef, System.Single beginning, System.Single end, System.Single scale, System.Single followingScale, System.Single beam, System.Single dynamics, System.Int16 ties)
System.Void Beams(ScoreModern.RecoveredFragments.InputEndpointTokens tokens)
System.Void Beams(System.Single[] endpointPairs)
System.Collections.Generic.List`1[System.Single[]] Commit(System.Collections.Generic.IList`1[System.Single[]] prefix)
System.Void Marks(System.Single[] tokens, System.Int32[] opposite, System.Boolean reproduceRetainedDescriptor)
System.Void Marks(System.Single[] tokens, System.Int32[] opposite)
System.Single[] MouseNotePositions()
System.Void ObservePitchPreviewLoader()
System.Void Pitches(System.Single[] tokens, System.Boolean insetLeft, System.Boolean allowLargeInput, System.Boolean useReferenceClef)
System.Void Pitches(System.Single[] tokens, System.Boolean insetLeft)
System.Void Pitches(System.Single[] tokens, System.Boolean insetLeft, System.Boolean allowLargeInput)
System.Collections.Generic.List`1[System.Single[]] Preview(System.Collections.Generic.IList`1[System.Single[]] prefix)
ScoreModern.RecoveredFragments.InputStaffTransaction Restart(System.Nullable`1[System.Single] correctedBeginning, System.Nullable`1[System.Single] correctedEnd, System.Nullable`1[System.Boolean] correctedProportional)
ScoreModern.RecoveredFragments.InputStaffTransaction RestartOnStaff(System.Int32 number, System.Int32 initialClef, System.Single factor, System.Single nextFactor)
ScoreModern.RecoveredFragments.InputStaffTransaction RestartOnStaffRange(System.Int32 number, System.Int32 initialClef, System.Single factor, System.Single nextFactor, System.Single correctedBeginning, System.Single correctedEnd, System.Nullable`1[System.Boolean] correctedProportional)
System.Void Rhythms(System.Single[] tokens)
System.Void Rhythms(System.Single[] tokens, System.Boolean reproduceNonfiniteChordDuration)
System.Void SkipRhythms()
System.Void Slurs(System.Single[] endpointPairs)
System.Void UpdateInputMeter(System.Int32& top, System.Int32& bottom)
System.Void UseReferenceSpacing(ScoreModern.RecoveredFragments.InputReferenceSpacing reference)
System.Void UseStaffCoordinates(ScoreModern.RecoveredFragments.InputEndpointStaffContext context)
Int32 InitialClef
Single OriginalSelectionLeft
Single OriginalSelectionRight
Single OriginalStaff
System.String ReferenceSpacingWarning
Int32 RhythmCount
ScoreModern.RecoveredFragments.InputStaffStage Stage
System.Collections.Generic.List`1[System.Single[]] WorkingRecords
```

## InputTextLists

```text
ScoreModern.RecoveredFragments.InputTextLists
static System.Single[] Pitches(System.String text, ScoreModern.RecoveredFragments.InputPitchState state)
static System.Single[] Pitches(System.String text, ScoreModern.RecoveredFragments.InputPitchState state, ScoreModern.RecoveredFragments.InputMacroContext context)
static System.Single[] Rhythms(System.String text)
static System.Single[] Rhythms(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context)
static System.Single[] Rhythms(System.String text, ScoreModern.RecoveredFragments.InputMacroContext context, System.Boolean originalMantissaWrap)
```

## InputTextTerminatorWrite

```text
ScoreModern.RecoveredFragments.InputTextTerminatorWrite
static Byte Space
static System.UInt16 Apply(System.Byte[] segment, System.UInt16 destination, System.UInt16 length)
static System.UInt16 Offset(System.UInt16 destination, System.UInt16 length)
```

## InputTranspositionAccidentalTail

```text
ScoreModern.RecoveredFragments.InputTranspositionAccidentalTail
static ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord] entries, System.Int16[] pitches, System.Collections.Generic.IList`1[System.Int32] relationships, System.Int16 activeCount, System.Single stopPosition, System.Int16 kindZeroGuard, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow initialWindow, System.Single margin, System.Single width)
```

## InputTranspositionAttachments

```text
ScoreModern.RecoveredFragments.InputTranspositionAttachments
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single left, System.Single right, System.Single diatonic, System.Boolean enabled)
static System.Boolean ShouldApply(System.Int16 relationshipCount, System.Boolean extendedBeam)
```

## InputTranspositionClefs

```text
ScoreModern.RecoveredFragments.InputTranspositionClefs
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord] entries, System.Int16[] pitches, System.Single staff, System.Int16 target, ScoreModern.RecoveredFragments.TranspositionClefState state)
```

## InputTranspositionCollection

```text
ScoreModern.RecoveredFragments.InputTranspositionCollection
static System.Collections.Generic.List`1[System.Single] Boundaries(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single firstNotePosition, System.Single expandedRight)
static ScoreModern.RecoveredFragments.TranspositionCollection FirstPass(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single expandedLeft, System.Single expandedRight, System.Int16 mode, System.Int16[] recoveredAccidentalTable)
static ScoreModern.RecoveredFragments.TranspositionCollection FirstPass(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single expandedLeft, System.Single expandedRight, System.Int16 mode)
```

## InputTranspositionCollector

```text
ScoreModern.RecoveredFragments.InputTranspositionCollector
static ScoreModern.RecoveredFragments.TranspositionCollectorResult Build(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single left, System.Single right, System.Int16 mode, ScoreModern.RecoveredFragments.TranspositionCollectorState state)
static ScoreModern.RecoveredFragments.TranspositionCollectorResult Build(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Single left, System.Single right, System.Int16 mode, ScoreModern.RecoveredFragments.TranspositionCollectorState state, System.Int16[] recoveredAccidentalTable)
```

## InputTranspositionEmbeddedControl

```text
ScoreModern.RecoveredFragments.InputTranspositionEmbeddedControl
static System.String FingeringWarning
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 staff, System.Int16 semitoneClass, System.Boolean& warningRequired)
```

## InputTranspositionInterval

```text
ScoreModern.RecoveredFragments.InputTranspositionInterval
static ScoreModern.RecoveredFragments.TranspositionInterval Convert(System.Int16 semitones)
static System.Int16 Menu(System.Int16 choice)
```

## InputTranspositionKey

```text
ScoreModern.RecoveredFragments.InputTranspositionKey
static System.Void Convert(System.Int16 semitones, System.Int16 oldKey, System.Int16& diatonic, System.Int16& newKey)
static System.Int16 KeyPosition(System.Int16 key)
```

## InputTranspositionKeyInsertion

```text
ScoreModern.RecoveredFragments.InputTranspositionKeyInsertion
static System.Single[] Append(System.Collections.Generic.IList`1[System.Single[]] rows, System.Single staff, System.Int16 key, System.Int16 clef)
static System.Boolean ShouldInsert(System.Boolean answerYes, System.Int16 firstKeyState, System.Int16 semitoneClass)
static System.Boolean ShouldPrompt(System.Boolean forceQuestion, System.Int16 firstKeyState, System.Int16 semitoneClass)
```

## InputTranspositionKeyRecord

```text
ScoreModern.RecoveredFragments.InputTranspositionKeyRecord
static ScoreModern.RecoveredFragments.TranspositionKeyRecordResult Apply(System.Single[] parameters, System.Int16 collectedKey, System.Int16 semitones, System.Int16 previousOverride, System.Int16 clef, System.Int16 collectorIndex, System.Single erasedCode)
```

## InputTranspositionMainPass

```text
ScoreModern.RecoveredFragments.InputTranspositionMainPass
static ScoreModern.RecoveredFragments.TranspositionMainPassResult Apply(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord] entries, System.Int16[] pitches, System.Collections.Generic.IList`1[System.Int32] relationships, ScoreModern.RecoveredFragments.TranspositionMainState state)
```

## InputTranspositionOperation

```text
ScoreModern.RecoveredFragments.InputTranspositionOperation
static ScoreModern.RecoveredFragments.TranspositionOperationResult Apply(System.Collections.Generic.IList`1[System.Single[]] source, ScoreModern.RecoveredFragments.TranspositionRequest request, ScoreModern.RecoveredFragments.TranspositionWorkspace workspace)
static ScoreModern.RecoveredFragments.TranspositionOperationSession Begin(System.Collections.Generic.IList`1[System.Single[]] source, ScoreModern.RecoveredFragments.TranspositionRequest request, ScoreModern.RecoveredFragments.TranspositionWorkspace workspace)
static System.Void ChangeClef(ScoreModern.RecoveredFragments.TranspositionOperationSession session, System.Int16 choice)
static ScoreModern.RecoveredFragments.TranspositionOperationResult Finish(ScoreModern.RecoveredFragments.TranspositionOperationSession session, System.Boolean addKey)
static ScoreModern.RecoveredFragments.TranspositionOperationResult FinishCore(ScoreModern.RecoveredFragments.TranspositionOperationSession session, System.Boolean addKey, System.Boolean mainAttachments)
```

## InputTranspositionOrder

```text
ScoreModern.RecoveredFragments.InputTranspositionOrder
static System.Boolean MarksModified(System.Int32 recordCount, System.Boolean alreadySorted)
static System.Int32[] OrderForMain(System.Collections.Generic.IList`1[System.Single[]] rows, System.Boolean alreadySorted)
```

## InputTranspositionPartition

```text
ScoreModern.RecoveredFragments.InputTranspositionPartition
static ScoreModern.RecoveredFragments.TranspositionPartition Apply(System.Collections.Generic.IList`1[System.Single] bars, System.Collections.Generic.IList`1[System.Single] positions, System.Collections.Generic.IList`1[System.Int16] kinds)
```

## InputTranspositionPitch

```text
ScoreModern.RecoveredFragments.InputTranspositionPitch
static ScoreModern.RecoveredFragments.TranspositionPitchResult Apply(System.Single originalPitch, System.Single originalStem, System.Int16 collectedPitch, System.Int16 accidental, System.Int16 clef, System.Int16 oldKey, System.Int16 newKey, System.Int16 diatonic, System.Int16 keyOverride)
```

## InputTranspositionRelationshipWindow

```text
ScoreModern.RecoveredFragments.InputTranspositionRelationshipWindow
static System.Void Advance(System.Collections.Generic.IList`1[System.Single[]] rows, System.Collections.Generic.IList`1[System.Int32] itemNumbers, System.Int16& activeCount, System.Int16& index, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow& window, System.Single margin, System.Single width)
```

## InputUniformBeam

```text
ScoreModern.RecoveredFragments.InputUniformBeam
static ScoreModern.RecoveredFragments.InputBeamConstructionResult Build(System.Collections.Generic.IList`1[System.Single[]] source, System.Int32[] map, System.Int32[] stems, System.Single first, System.Single last, System.Single begin, System.Single end, System.Single slopeLimit, ScoreModern.RecoveredFragments.InputEndpointStaffContext context)
```

## INumericConverterContext

```text
ScoreModern.RecoveredFragments.INumericConverterContext
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IParameterWordMemory

```text
ScoreModern.RecoveredFragments.IParameterWordMemory
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IPointBufferMemory

```text
ScoreModern.RecoveredFragments.IPointBufferMemory
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IPrefixPreparationMemory

```text
ScoreModern.RecoveredFragments.IPrefixPreparationMemory
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IScreenBiosTransferEffects

```text
ScoreModern.RecoveredFragments.IScreenBiosTransferEffects
ScoreModern.RecoveredFragments.SignedDivide16State TransferToBios(ScoreModern.RecoveredFragments.IScreenHookMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## IScreenForwardMemory

```text
ScoreModern.RecoveredFragments.IScreenForwardMemory
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
```

## IScreenHookMemory

```text
ScoreModern.RecoveredFragments.IScreenHookMemory
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
```

## ISelectorContext

```text
ScoreModern.RecoveredFragments.ISelectorContext
System.UInt16 CallFar(System.UInt16 relativeSegment, System.UInt16 offset, System.UInt16[] stackWords)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
UInt16 DataSegment
Boolean DirectionFlag
```

## ISpanBookkeepingMemory

```text
ScoreModern.RecoveredFragments.ISpanBookkeepingMemory
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IStringBlockMemory

```text
ScoreModern.RecoveredFragments.IStringBlockMemory
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
System.UInt16 ReadWord(System.UInt16 segment, System.UInt16 offset)
System.Void WriteByte(System.UInt16 segment, System.UInt16 offset, System.Byte value)
System.Void WriteWord(System.UInt16 segment, System.UInt16 offset, System.UInt16 value)
```

## IStringOutputContext

```text
ScoreModern.RecoveredFragments.IStringOutputContext
ScoreModern.RecoveredFragments.CharacterRoutineResult CallCharacter(ScoreModern.RecoveredFragments.CharacterRoutineWords words, System.UInt16 targetSegment, System.UInt16 targetOffset)
System.Byte ReadByte(System.UInt16 segment, System.UInt16 offset)
```

## ItemDrawingDispatch

```text
ScoreModern.RecoveredFragments.ItemDrawingDispatch
static ScoreModern.RecoveredFragments.ItemDrawingRoute Select(ScoreModern.RecoveredFragments.IDatTextLookupMemory memory, System.UInt16 ds, System.UInt16 cs)
```

## ItemDrawingRoute

```text
ScoreModern.RecoveredFragments.ItemDrawingRoute
UInt16 TableByteIndex
UInt16 Target
Boolean UsesTable
```

## JustificationAllocationResult

```text
ScoreModern.RecoveredFragments.JustificationAllocationResult
.ctor()
Single Allocated
Int16 NextIndex
Single Remaining
```

## JustificationCoordinates

```text
ScoreModern.RecoveredFragments.JustificationCoordinates
.ctor(System.Single[] words, System.Int16[] references, System.Single lowerBound, System.Single ratioThreshold, System.Single ratioReplacement)
Single LowerBound
System.Void Scale(System.Single left, System.Single right, System.Single delta)
System.Void Shift(System.Single threshold, System.Single delta)
```

## JustificationGapResult

```text
ScoreModern.RecoveredFragments.JustificationGapResult
.ctor()
Boolean CompressedMinimums
Single DeficitPerGap
Single MinimumTotal
Single NegativeSlack
Single PositiveSlack
Single Width
```

## LayoutBasicTablature

```text
ScoreModern.RecoveredFragments.LayoutBasicTablature
static System.Collections.Generic.List`1[System.Single[]] Build(System.Single[] staff, System.Collections.Generic.IList`1[System.Single[]] notes, System.Int32 destination, System.Boolean clef, System.Boolean rhythmLine, System.Int16[] packedTuning, System.Int32 strings, System.Boolean bass, System.Collections.Generic.IList`1[System.Int16] frequencies, System.Boolean hasCollection)
static System.Collections.Generic.List`1[System.Single[]] Build(System.Single[] staff, System.Collections.Generic.IList`1[System.Single[]] notes, System.Int32 destination, System.Boolean clef, System.Boolean rhythmLine, System.Int16[] packedTuning, System.Int32 strings, System.Boolean bass, System.Collections.Generic.IList`1[System.Int16] frequencies, System.Boolean hasCollection, System.Int16[] recoveredAccidentals, System.Int32 firstDelta, System.Func`3[System.Single[],System.Int32,System.Single[]] retainedCopy, System.Action`2[System.Single[],System.Int32] writeRecord)
```

## LayoutCoordinateAffine

```text
ScoreModern.RecoveredFragments.LayoutCoordinateAffine
static System.Void Apply(System.Single[] words, System.Int16[] references, System.Single first, System.Single second, System.Single third, System.Int16 mode)
```

## LayoutCoordinateCollection

```text
ScoreModern.RecoveredFragments.LayoutCoordinateCollection
static ScoreModern.RecoveredFragments.LayoutCoordinateCollectionResult Collect(System.Single[] words, System.Int32[] itemStarts, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Single tolerance, System.Int16 first, System.Int16 last, System.Int16 staffTableThreshold, System.Single codeFilter, System.Single emptyStaffControl, System.Single compoundBarControl, System.Int32 capacity, System.Int16 mode)
```

## LayoutCoordinateCollectionResult

```text
ScoreModern.RecoveredFragments.LayoutCoordinateCollectionResult
.ctor()
Int16 FirstItem
Int16 LastItem
Int16[] References
```

## LayoutDecombineGroups

```text
ScoreModern.RecoveredFragments.LayoutDecombineGroups
static System.Int16[] Boundaries(System.Int16 maximum, System.Single[] sizes, System.Int16 lastSize)
```

## LayoutDisappearingStaffs

```text
ScoreModern.RecoveredFragments.LayoutDisappearingStaffs
.ctor()
System.Void Apply(System.Single[] values)
System.Boolean Hidden(System.Int32 staff)
System.Int32[] HiddenStaffs()
```

## LayoutItemSpacing

```text
ScoreModern.RecoveredFragments.LayoutItemSpacing
.ctor(System.Byte[] workspace, System.Single[] scoreWords, System.Int32[] itemStarts, System.Single[] selectedPositions, System.Int16[] selectedReferences, System.Single[] auxiliary, System.Single[] minimumGaps, System.Single[] outputPositions)
System.Int16 Apply(System.Int16 staff, System.Single staffScale, System.Single firstStaff, System.Int16& cursor, System.Single left, System.Single right, System.Single[] afterSymbol, System.Boolean originalQuarterToneWidths, System.Nullable`1[System.Single] positionJoinTolerance, System.Boolean standaloneJust)
```

## LayoutJustificationAllocation

```text
ScoreModern.RecoveredFragments.LayoutJustificationAllocation
static ScoreModern.RecoveredFragments.JustificationAllocationResult Apply(System.Single[] positions, System.Single[] allocations, System.Int16 index, System.Single target, System.Single limit, System.Single remaining, System.Single minimum)
```

## LayoutJustificationGaps

```text
ScoreModern.RecoveredFragments.LayoutJustificationGaps
static ScoreModern.RecoveredFragments.JustificationGapResult Prepare(System.Single[] slack, System.Single[] minimum, System.Single[] positions, System.Int16 pointCount, System.Single left, System.Single right)
```

## LayoutJustificationMerge

```text
ScoreModern.RecoveredFragments.LayoutJustificationMerge
static System.Int16 Append(System.Single[] oldPositions, System.Single[] oldMinimum, System.Single[] newPositions, System.Single[] newMinimum, System.Single[] mergedPositions, System.Single[] mergedMinimum, System.Int16 oldCount, System.Int16 newCount, System.Single right, System.Single rightThreshold, System.Single epsilon)
```

## LayoutJustificationRedistribution

```text
ScoreModern.RecoveredFragments.LayoutJustificationRedistribution
static System.Boolean Apply(System.Single[] slack, System.Single[] positions, System.Int16 pointCount, System.Single positiveSlack, System.Single negativeSlack, ScoreModern.RecoveredFragments.JustificationCoordinates coordinates)
```

## LayoutJustificationSnap

```text
ScoreModern.RecoveredFragments.LayoutJustificationSnap
static System.Void Apply(System.Single[] words, System.Int32[] itemStarts, System.Single tolerance, System.Single rightEdge)
```

## LayoutJustificationSystem

```text
ScoreModern.RecoveredFragments.LayoutJustificationSystem
static ScoreModern.RecoveredFragments.LayoutJustificationSystemResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Single[] scales, System.Single[] afterSymbol, System.Int16 staffTableThreshold, System.Int16 coordinateCapacity, ScoreModern.RecoveredFragments.LayoutJustificationWorkspace work)
```

## LayoutJustificationSystemResult

```text
ScoreModern.RecoveredFragments.LayoutJustificationSystemResult
.ctor()
Int16[] CoordinateReferences
Int16 FirstPoint
ScoreModern.RecoveredFragments.JustificationGapResult Gaps
Int16 PointCount
Boolean RedistributionApplied
Int16 SelectionCursor
```

## LayoutJustificationWorkspace

```text
ScoreModern.RecoveredFragments.LayoutJustificationWorkspace
.ctor()
Single[] Auxiliary
Single[] MergedMinimum
Single[] MergedPositions
Single[] Minimum
Single[] OldMinimum
Single[] OldPositions
Boolean OriginalQuarterToneWidths
System.Nullable`1[System.Single] PositionJoinTolerance
Single[] Positions
Single[] Selected
Int16[] SelectedReferences
Byte[] Spacing
Boolean StandaloneJust
System.Void ResetForIsolatedTest()
```

## LayoutLineupBeatTotal

```text
ScoreModern.RecoveredFragments.LayoutLineupBeatTotal
.ctor()
Int16 Staff
Single Total
```

## LayoutLineupCleanup

```text
ScoreModern.RecoveredFragments.LayoutLineupCleanup
static System.Void Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] staffUnits)
```

## LayoutLineupCoordinates

```text
ScoreModern.RecoveredFragments.LayoutLineupCoordinates
static System.Void Apply(System.Single[] merged, System.Single[] positions, System.Single[] weights, System.Int16 count, System.Single left, System.Single right, System.Single staffUnit, System.Single exponent)
```

## LayoutLineupDurationGroups

```text
ScoreModern.RecoveredFragments.LayoutLineupDurationGroups
static ScoreModern.RecoveredFragments.LayoutLineupDurationGroupsResult Apply(System.Single[] current, System.Single[] prior, System.Single[] output, System.Int16 firstCurrent, System.Int16 priorCount, System.Int16 initialOutputCount)
```

## LayoutLineupDurationGroupsResult

```text
ScoreModern.RecoveredFragments.LayoutLineupDurationGroupsResult
.ctor()
Int16 CurrentIndex
Single CurrentSum
Single CurrentValue
Int16 OutputCount
Int16 PriorIndex
Single PriorSum
Single PriorValue
```

## LayoutLineupPreparation

```text
ScoreModern.RecoveredFragments.LayoutLineupPreparation
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] rhythms, System.Single[] positions, System.Single[] merged, System.Single[] scratch, System.Single[] union, System.Single[] subdivisions)
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult ApplyWithBeatPolicy(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] rhythms, System.Single[] positions, System.Single[] merged, System.Single[] scratch, System.Single[] union, System.Single[] subdivisions, System.Boolean returnUnequalBeats, System.Action`1[System.Int32] acknowledgeNegativePosition, System.Boolean standaloneJust)
```

## LayoutLineupPreparationResult

```text
ScoreModern.RecoveredFragments.LayoutLineupPreparationResult
.ctor()
ScoreModern.RecoveredFragments.LayoutLineupBeatTotal[] BeatTotals
Int16 Bottom
Int16 Count
Single Left
Boolean NeedsCleanup
Single Right
ScoreModern.RecoveredFragments.LayoutLineupStaffStream[] StaffStreams
Int16 Top
Boolean UnequalBeats
```

## LayoutLineupRemap

```text
ScoreModern.RecoveredFragments.LayoutLineupRemap
static System.Void Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] merged, System.Single[] newPositions, System.Int16 count, System.Single[] savedRhythms, System.Single[] savedPositions, System.Int16[] staffStarts, System.Single[] staffUnits, System.Int16 staffThreshold, System.Int32 capacity)
```

## LayoutLineupStaffStream

```text
ScoreModern.RecoveredFragments.LayoutLineupStaffStream
.ctor()
Single[] Positions
Single[] Rhythms
Int16 Staff
```

## LayoutLineupStaffTables

```text
ScoreModern.RecoveredFragments.LayoutLineupStaffTables
static ScoreModern.RecoveredFragments.LayoutLineupStaffTablesResult Apply(System.Single[] merged, System.Int16 mergedCount, System.Single[] storedRhythms, System.Single[] storedPositions, System.Int16 firstStored, System.Single[] outputRhythms, System.Single[] outputPositions)
```

## LayoutLineupStaffTablesResult

```text
ScoreModern.RecoveredFragments.LayoutLineupStaffTablesResult
.ctor()
Int16 Count
Int16 FirstMerged
```

## LayoutLineupStreamUnion

```text
ScoreModern.RecoveredFragments.LayoutLineupStreamUnion
static ScoreModern.RecoveredFragments.LayoutLineupStreamUnionResult Apply(System.Single[] current, System.Int16 firstCurrent, System.Int16 currentCount, System.Single[] prior, System.Int16 priorCount, System.Single[] output)
```

## LayoutLineupStreamUnionResult

```text
ScoreModern.RecoveredFragments.LayoutLineupStreamUnionResult
.ctor()
Int16 Count
Int16 CurrentIndex
Single CurrentSum
Single LastValue
Int16 PriorIndex
Single PriorSum
```

## LayoutLineupSystem

```text
ScoreModern.RecoveredFragments.LayoutLineupSystem
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] staffUnits, System.Single exponent, System.Int16 staffThreshold, System.Int32 capacity, ScoreModern.RecoveredFragments.LayoutLineupWorkspace work)
static ScoreModern.RecoveredFragments.LayoutLineupPreparationResult ApplyWithBeatPolicy(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single[] staffUnits, System.Single exponent, System.Int16 staffThreshold, System.Int32 capacity, ScoreModern.RecoveredFragments.LayoutLineupWorkspace work, System.Boolean returnUnequalBeats, System.Action`1[System.Int32] acknowledgeNegativePosition)
```

## LayoutLineupTableCompact

```text
ScoreModern.RecoveredFragments.LayoutLineupTableCompact
static System.Int16 Apply(System.Single[] source, System.Single[] destination, System.Int16 first, System.Int16 last, System.Single retainedTag)
```

## LayoutLineupWorkspace

```text
ScoreModern.RecoveredFragments.LayoutLineupWorkspace
.ctor()
Int32 MaximumStaff
Single[] Merged
Single[] Positions
Single[] Rhythms
Single[] Scratch
Boolean StandaloneJust
Single[] Subdivisions
Single[] Union
```

## LayoutNoteSpacingSpan

```text
ScoreModern.RecoveredFragments.LayoutNoteSpacingSpan
.ctor(System.Byte[] workspace, System.Single[] scoreWords, System.Int16[] recordReferences, System.Single[] selectedPositions, System.Boolean originalQuarterToneWidths, System.Single followingBarScale)
System.Boolean Apply()
System.Void LoadRecord(System.Int32 wordIndex, System.Int32 countAddress)
```

## LayoutParentPreparation

```text
ScoreModern.RecoveredFragments.LayoutParentPreparation
.ctor(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Boolean alreadySorted, System.Boolean lineupMode)
Int16 Bottom
Boolean Complete
Int32 DuplicateIndex
Single Left
Boolean Modified
System.Collections.Generic.List`1[System.Int32] Origins
Single Right
System.Collections.Generic.List`1[System.Single[]] Rows
Int16 Top
System.Void AnswerDuplicate(System.Boolean delete)
```

## LayoutRestSpacing

```text
ScoreModern.RecoveredFragments.LayoutRestSpacing
static ScoreModern.RecoveredFragments.LayoutRestSpacingResult Apply(System.Single multiplier, System.Single p6, System.Single p5, System.Single p4, System.Single left, System.Int16 parameterCount, System.Single p9, System.Single p10, System.Single p15)
```

## LayoutRestSpacingResult

```text
ScoreModern.RecoveredFragments.LayoutRestSpacingResult
.ctor()
Single Left
Single Multiplier
System.Nullable`1[System.Single] Parameter10Scratch
Single Right
```

## LayoutRhythmicCollection

```text
ScoreModern.RecoveredFragments.LayoutRhythmicCollection
static ScoreModern.RecoveredFragments.LayoutRhythmicCollectionResult Collect(System.Single[] words, System.Int32[] starts, System.Single staff, System.Int16 inputMode, System.Int16 cursor, System.Single[] rhythms, System.Single[] positions, System.Single[] scratch, System.Action`1[System.Int32] acknowledgeNegativePosition, System.Boolean standaloneJust)
```

## LayoutRhythmicCollectionResult

```text
ScoreModern.RecoveredFragments.LayoutRhythmicCollectionResult
.ctor()
Int16 Count
Boolean InvalidatedSort
Int16 NextItem
Single Total
```

## LayoutRipple

```text
ScoreModern.RecoveredFragments.LayoutRipple
static ScoreModern.RecoveredFragments.LayoutRippleResult Prepare(System.Single[] words, System.Int32[] starts, System.Int16 count, System.Int16 source, System.Int16 destination, System.Int16 bars)
```

## LayoutRippleCoordinates

```text
ScoreModern.RecoveredFragments.LayoutRippleCoordinates
static System.Void Scale(System.Single[] words, System.Int16[] references, System.Single origin, System.Single factor)
static System.Void Shift(System.Single[] words, System.Int16[] references, System.Single delta)
static System.Void StaffShift(System.Single[] words, System.Int16[] references, System.Single delta)
```

## LayoutRippleResult

```text
ScoreModern.RecoveredFragments.LayoutRippleResult
.ctor()
Boolean Applied
Int16 Count
Int16 Destination
Int16 Source
Int32[] Starts
```

## LayoutSeparateParts

```text
ScoreModern.RecoveredFragments.LayoutSeparateParts
static ScoreModern.RecoveredFragments.LayoutSeparateResult Apply(System.Collections.Generic.IList`1[System.Single[]] source, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Boolean deleteBottom)
static System.Void Validate(System.Collections.Generic.IList`1[System.Single[]] rows, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right)
```

## LayoutSeparateResult

```text
ScoreModern.RecoveredFragments.LayoutSeparateResult
.ctor()
System.Collections.Generic.List`1[System.Int16] ChangedStaffs
Int32 Deleted
System.Collections.Generic.List`1[System.Int32] Origins
System.Collections.Generic.List`1[System.Single[]] Rows
```

## LayoutSpacingSelection

```text
ScoreModern.RecoveredFragments.LayoutSpacingSelection
static ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult Collect(System.Single[] words, System.Int32[] starts, System.Int16 staff, System.Int16 firstStaff, System.Int16 cursor, System.Single left, System.Single right, System.Single tolerance, System.Single one, System.Single clefLimit, System.Single spanningLimit, System.Single barLimit, System.Single drawingLimit, System.Int32 capacity)
static System.Void Sort(System.Single[] positions, System.Int16[] records, System.Int16 count)
```

## LayoutSpacingSelectionResult

```text
ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult
.ctor()
Single[] Positions
Int16[] RecordWords
Int32 StoppedItem
```

## LayoutSpacingWorkspace

```text
ScoreModern.RecoveredFragments.LayoutSpacingWorkspace
static System.Byte[] Create()
```

## LayoutSymbolSpacing

```text
ScoreModern.RecoveredFragments.LayoutSymbolSpacing
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Bar(System.Int16 previous, System.Int16 count, System.Single p5, System.Single p7, System.Single effectiveP8, System.Single p13, System.Single p14)
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Key(System.Int16 previous, System.Single p7, System.Single p5, System.Boolean standaloneJust)
static ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Meter(System.Int16 previous, System.Single minimum, System.Single[] parameters, System.Int16 count, System.Boolean standaloneJust)
```

## LayoutSymbolSpacingResult

```text
ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult
.ctor()
Single Left
Single Minimum
Boolean ResetParameter10Scratch
Single Right
Single ScaleScratch
Single Scratch
```

## LayoutTablatureAccidentalTable

```text
ScoreModern.RecoveredFragments.LayoutTablatureAccidentalTable
static Int32 FirstDelta
static System.Int16[] Recover(System.UInt16 loadSegment)
```

## LayoutTablatureChord

```text
ScoreModern.RecoveredFragments.LayoutTablatureChord
static System.Void Apply(System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.LayoutTablatureChordNote] notes, System.Int32 staffStrings)
```

## LayoutTablatureChordNote

```text
ScoreModern.RecoveredFragments.LayoutTablatureChordNote
.ctor()
ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan Plan
Single[] Row
Int32 SortKey
```

## LayoutTablatureFrequency

```text
ScoreModern.RecoveredFragments.LayoutTablatureFrequency
static System.Collections.Generic.Dictionary`2[System.Int32,System.Int16] Apply(ScoreModern.RecoveredFragments.TranspositionCollectorResult collection)
```

## LayoutTablaturePitch

```text
ScoreModern.RecoveredFragments.LayoutTablaturePitch
static ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan Prepare(System.Int32 head, System.Int32 pitch, System.Int32 accidental, System.Int16[] packed)
static ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan PrepareDrawing(System.Int32 head, System.Int32 pitch, System.Int32 accidental, System.Int16[] packed, System.Int16 capo)
```

## LayoutTablaturePitchPlan

```text
ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan
.ctor()
Int32 Absolute
Int32 Strings
Int32[] Tuning
System.Int32 SelectedString(System.Int32 requested)
```

## LayoutTablatureRelationships

```text
ScoreModern.RecoveredFragments.LayoutTablatureRelationships
static System.Void Apply(System.Collections.Generic.IList`1[System.Single[]] rows, ScoreModern.RecoveredFragments.TranspositionCollectorResult collected, System.Collections.Generic.Dictionary`2[System.Int32,System.Int16] frequencies)
```

## LayoutTextJustification

```text
ScoreModern.RecoveredFragments.LayoutTextJustification
static ScoreModern.RecoveredFragments.LayoutTextJustificationResult Apply(System.Single[] words, System.Int32[] starts, System.Int16 bottom, System.Int16 top, System.Single left, System.Single right, System.Int16 textStaff, System.Single level, System.Single[] staffUnits, System.Int16 staffThreshold, System.Int32 capacity, ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace work)
```

## LayoutTextJustificationResult

```text
ScoreModern.RecoveredFragments.LayoutTextJustificationResult
.ctor()
ScoreModern.RecoveredFragments.JustificationGapResult Gaps
Int16 PointCount
```

## LayoutTextJustificationWorkspace

```text
ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace
.ctor()
Single[] Minimum
Single[] Positions
Single[] Slack
```

## LayoutVerticalCollision

```text
ScoreModern.RecoveredFragments.LayoutVerticalCollision
static System.Int32 Minimum(System.Int16[] upper, System.Int16[] lower, System.Int16 radius)
```

## LayoutVerticalContour

```text
ScoreModern.RecoveredFragments.LayoutVerticalContour
static System.Void Apply(System.Int16[] x, System.Int16[] y, System.Int32 first, System.Int32 next, System.Boolean skipGuide, System.Boolean initialize, System.Int16[] minimum, System.Int16[] maximum, System.Int16 binWidth, System.Int16 bias)
```

## LayoutVerticalPlacement

```text
ScoreModern.RecoveredFragments.LayoutVerticalPlacement
static ScoreModern.RecoveredFragments.LayoutVerticalPlacementResult Apply(System.Int16[][] minimum, System.Int16[][] maximum, System.Boolean[] occupied, System.Single[] scales, System.Single[] bases, System.Int16 bottom, System.Int16 top, System.Int16 verticalSteps, System.Int16 radius, System.Single staffGap, System.Single[] retainedOffsets)
```

## LayoutVerticalPlacementResult

```text
ScoreModern.RecoveredFragments.LayoutVerticalPlacementResult
.ctor()
Int16 Bottom
Single[] Offsets
Boolean Repeat
Int16 Top
```

## LineClipping

```text
ScoreModern.RecoveredFragments.LineClipping
static ScoreModern.RecoveredFragments.LineClippingWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt16 si, System.UInt16 di)
static ScoreModern.RecoveredFragments.LineClippingFlagResult ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt16 si, System.UInt16 di, System.UInt32 eflags)
```

## LineClippingDivideFault

```text
ScoreModern.RecoveredFragments.LineClippingDivideFault
.ctor()
UInt32 EFlags
Boolean HasEFlags
ScoreModern.RecoveredFragments.LineClippingWords Words
```

## LineClippingFlagResult

```text
ScoreModern.RecoveredFragments.LineClippingFlagResult
UInt32 EFlags
ScoreModern.RecoveredFragments.LineClippingWords Words
```

## LineClippingWords

```text
ScoreModern.RecoveredFragments.LineClippingWords
UInt16 AX
UInt16 BX
UInt16 CX
UInt16 DI
UInt16 DX
UInt16 Exit
UInt16 SI
```

## LineDriverBody

```text
ScoreModern.RecoveredFragments.LineDriverBody
static ScoreModern.RecoveredFragments.LineDriverBodyResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteThroughReturnWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
static ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## LineDriverBodyResult

```text
ScoreModern.RecoveredFragments.LineDriverBodyResult
UInt16 ClippingExit
UInt32 DefinedEFlagsMask
ScoreModern.RecoveredFragments.SignedDivide16Fault Fault
UInt16 RasterEntry
Boolean RetfExecuted
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## LinePreparation

```text
ScoreModern.RecoveredFragments.LinePreparation
static ScoreModern.RecoveredFragments.LinePreparationWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 ax)
```

## LinePreparationWords

```text
ScoreModern.RecoveredFragments.LinePreparationWords
UInt16 AX
UInt16 BX
UInt16 CX
UInt16 DI
UInt16 DX
UInt16 ES
```

## Mode12BiosTransfer

```text
ScoreModern.RecoveredFragments.Mode12BiosTransfer
.ctor(ScoreModern.RecoveredFragments.Mode12VideoServices services, System.UInt16 measuredCallbackSegment, System.UInt16 measuredIretOffset)
ScoreModern.RecoveredFragments.SignedDivide16State TransferToBios(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## Mode12CellSurface

```text
ScoreModern.RecoveredFragments.Mode12CellSurface
.ctor()
System.Byte Background(System.Int32 row, System.Int32 column)
ScoreModern.RecoveredFragments.Mode12Glyph[] Cell(System.Int32 row, System.Int32 column)
System.Void Clear()
System.Void Scroll(System.Int32 top, System.Int32 left, System.Int32 bottom, System.Int32 right, System.Int32 lines, System.Byte background)
System.Void WriteGlyph(System.Int32 row, System.Int32 column, System.Byte character, System.Byte color)
Int64 Revision
```

## Mode12Glyph

```text
ScoreModern.RecoveredFragments.Mode12Glyph
.ctor(System.Byte c, System.Byte a)
Byte Character
Byte Color
```

## Mode12Plane2Surface

```text
ScoreModern.RecoveredFragments.Mode12Plane2Surface
.ctor(System.Byte[] initial, ScoreModern.RecoveredFragments.IMode12GlyphSource glyphs)
Byte[] Bytes
System.Void Scroll(System.Int32 top, System.Int32 left, System.Int32 bottom, System.Int32 right, System.Int32 lines, System.Byte background)
System.Void WriteGlyph(System.Int32 row, System.Int32 column, System.Byte character, System.Byte color)
```

## Mode12VideoServices

```text
ScoreModern.RecoveredFragments.Mode12VideoServices
.ctor(ScoreModern.RecoveredFragments.IMode12Surface output)
ScoreModern.RecoveredFragments.SignedDivide16State ExecuteService(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## NumericConverter

```text
ScoreModern.RecoveredFragments.NumericConverter
static ScoreModern.RecoveredFragments.NumericConverterResult Execute(ScoreModern.RecoveredFragments.INumericConverterContext m, System.UInt16 statusSegment, System.UInt16 statusOffset, System.UInt16 inputSegment, System.UInt16 inputOffset)
```

## NumericConverterResult

```text
ScoreModern.RecoveredFragments.NumericConverterResult
.ctor(System.UInt16 ax, System.UInt16 dx, System.UInt16 status)
UInt16 AX
UInt16 DX
UInt16 Status
```

## OutputCountAdvance

```text
ScoreModern.RecoveredFragments.OutputCountAdvance
static ScoreModern.RecoveredFragments.OutputCountResult Execute(ScoreModern.RecoveredFragments.ISpanBookkeepingMemory m, System.UInt16 ss, System.UInt16 countOffset, System.UInt16 ds, System.UInt16 descriptorSi)
```

## OutputCountResult

```text
ScoreModern.RecoveredFragments.OutputCountResult
.ctor(System.UInt16 ax, System.UInt16 dx)
UInt16 AX
UInt16 DX
```

## PackedDrawingParts

```text
ScoreModern.RecoveredFragments.PackedDrawingParts
Int16 FinalAX
Int16 High
Int16 Low
Int16 Middle
```

## PackedDrawingWord

```text
ScoreModern.RecoveredFragments.PackedDrawingWord
static ScoreModern.RecoveredFragments.PackedDrawingParts Decode(System.Int16 word)
```

## PageCoordinates

```text
ScoreModern.RecoveredFragments.PageCoordinates
.ctor()
Single[] Bases
Single[] Scales
ScoreModern.RecoveredFragments.PageItemContext Prepare(System.Single[] p, System.Int32 staff, System.Int32 code)
ScoreModern.RecoveredFragments.PageStaffResult SetStaff(System.Single[] p, System.Boolean alternateUnits)
ScoreModern.RecoveredFragments.PageStaffResult SetStaff(System.Single[] p, System.Int32 staff, System.Int16 referenceWord, System.Boolean alternateUnits)
System.Single Vertical(System.Int32 staff, System.Single value)
```

## PageItemContext

```text
ScoreModern.RecoveredFragments.PageItemContext
Int16 HorizontalWord
Single Scale
Boolean ScreenHorizontalLimit
Single VerticalInput
Single X
Single Y
```

## PageStaffLine

```text
ScoreModern.RecoveredFragments.PageStaffLine
Single Left
Single Right
Single Y
```

## PageStaffResult

```text
ScoreModern.RecoveredFragments.PageStaffResult
.ctor()
Single Base
Single Bottom
Int32 LineCount
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.PageStaffLine] Lines
Single Scale
Single Spacing
Int16 WidthSelector
```

## ParameterWordReader

```text
ScoreModern.RecoveredFragments.ParameterWordReader
static System.UInt16 Execute(ScoreModern.RecoveredFragments.IParameterWordMemory m, System.UInt16 cursorSegment, System.UInt16 cursorOffset, System.UInt16 destinationSegment, System.UInt16 destinationOffset)
```

## PointBufferAppend

```text
ScoreModern.RecoveredFragments.PointBufferAppend
static ScoreModern.RecoveredFragments.PointBufferWords Execute(ScoreModern.RecoveredFragments.IPointBufferMemory m, System.UInt16 ds, System.UInt16 ss, System.UInt16 bp)
static ScoreModern.RecoveredFragments.PointBufferWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IPointBufferMemory m, System.UInt16 ds, System.UInt16 ss, System.UInt16 bp, System.UInt32 flags)
```

## PointBufferWords

```text
ScoreModern.RecoveredFragments.PointBufferWords
UInt16 AX
UInt16 BX
UInt32 DefinedEFlagsMask
UInt32 EFlags
UInt16 ES
UInt16 SI
```

## PrefixPreparationResult

```text
ScoreModern.RecoveredFragments.PrefixPreparationResult
.ctor(System.UInt16 ax, System.UInt16 bp, System.UInt16 sp, System.UInt16 si, System.UInt16 di)
UInt16 AX
UInt16 BP
UInt16 DI
UInt16 RelativeTargetSegment
UInt16 SI
UInt16 SP
UInt16 TargetOffset
```

## PrefixWritePreparation

```text
ScoreModern.RecoveredFragments.PrefixWritePreparation
static ScoreModern.RecoveredFragments.PrefixPreparationResult Prepare(ScoreModern.RecoveredFragments.IPrefixPreparationMemory m, System.UInt16 ss, System.UInt16 ds, System.UInt16 cs, System.UInt16 sp, System.UInt16 bp, System.UInt16 si, System.UInt16 di)
```

## PrimitiveTextGeometry

```text
ScoreModern.RecoveredFragments.PrimitiveTextGeometry
static ScoreModern.RecoveredFragments.PrimitiveTextPoint Point(System.Int16 packed, ScoreModern.RecoveredFragments.PrimitiveTextScale scale)
static ScoreModern.RecoveredFragments.PrimitiveTextScale Prepare(System.Single horizontal, System.Single vertical, System.Single staff, System.Single x, System.Single y, System.Single left, System.Single baseline)
```

## PrimitiveTextPoint

```text
ScoreModern.RecoveredFragments.PrimitiveTextPoint
Boolean CommandOnly
Int32 MarkerWordBack
Int16 Operation
System.Nullable`1[System.Int16] RetainedMarkerY
Single X
Single Y
```

## PrimitiveTextScale

```text
ScoreModern.RecoveredFragments.PrimitiveTextScale
Single OriginX
Single OriginY
Single X
Single Y
```

## PrintedGlyphDefinition

```text
ScoreModern.RecoveredFragments.PrintedGlyphDefinition
static System.String Fixed3(System.Single value)
static ScoreModern.RecoveredFragments.PrintedGlyphVertex Vertex(System.Int16 x, System.Int16 y, System.Int16 operation)
```

## PrintedGlyphVertex

```text
ScoreModern.RecoveredFragments.PrintedGlyphVertex
Int16 Operation
Int32 X
Int32 Y
```

## PrintedHeadRequest

```text
ScoreModern.RecoveredFragments.PrintedHeadRequest
Int32 Glyph
System.String Scale
System.String Width
Int16 X
Int16 Y
```

## PrintRotation

```text
ScoreModern.RecoveredFragments.PrintRotation
static System.Single Apply(System.Single size, System.Boolean before, System.Boolean after)
static System.Int32 Offset(System.Int32 candidate, System.Boolean before, System.Boolean after)
```

## RasterDispatch

```text
ScoreModern.RecoveredFragments.RasterDispatch
static ScoreModern.RecoveredFragments.RasterDispatchWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt32 flags)
```

## RasterDispatchWords

```text
ScoreModern.RecoveredFragments.RasterDispatchWords
UInt16 BX
UInt16 CX
UInt16 DX
UInt32 EFlags
UInt16 Exit
```

## RasterStatusFlags

```text
ScoreModern.RecoveredFragments.RasterStatusFlags
static System.UInt32 Add16(System.UInt32 flags, System.UInt16 a, System.UInt16 b)
static System.UInt32 Dec16(System.UInt32 flags, System.UInt16 a)
static System.UInt32 Inc16(System.UInt32 flags, System.UInt16 a)
static System.UInt32 Logical8(System.UInt32 flags, System.Byte result)
static System.UInt32 Sub16(System.UInt32 flags, System.UInt16 a, System.UInt16 b)
static System.UInt32 Sub8(System.UInt32 flags, System.Byte a, System.Byte b)
```

## RealModeIret16

```text
ScoreModern.RecoveredFragments.RealModeIret16
static ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## RetainedParameterStorage

```text
ScoreModern.RecoveredFragments.RetainedParameterStorage
.ctor()
static Int32 Capacity
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

## ScorlasFilePlacement

```text
ScoreModern.RecoveredFragments.ScorlasFilePlacement
Int32 BottomShift
Int32 Cumulative
Int32 Delta
Int32 DscBottom
Int32 DscTop
Int32 NextAdvance
Int32 NormalizedHeight
Int32 OverallHeight
Int32 RawHeight
```

## ScorlasFilePlacementState

```text
ScoreModern.RecoveredFragments.ScorlasFilePlacementState
.ctor()
ScoreModern.RecoveredFragments.ScorlasFilePlacement Add(System.Int16 minimum, System.Int16 maximum, System.Int32 guide)
```

## ScorlasGuideConversion

```text
ScoreModern.RecoveredFragments.ScorlasGuideConversion
static System.Int32 Convert(System.Single value, System.Boolean metric)
```

## ScorlasStatisticsArithmetic

```text
ScoreModern.RecoveredFragments.ScorlasStatisticsArithmetic
static System.Single Height(System.Int32 cumulativeHeight, System.Single printSize, System.Boolean metric)
```

## ScreenForwardResult

```text
ScoreModern.RecoveredFragments.ScreenForwardResult
.ctor(System.UInt16 dx, System.UInt16 cx, System.UInt16 bx, System.UInt16 ds, System.UInt16 sp, System.UInt16 ip, System.UInt16 cs)
UInt16 BX
UInt16 CS
UInt16 CX
UInt16 DS
UInt16 DX
UInt16 IP
UInt16 SP
```

## ScreenHookDecisions

```text
ScoreModern.RecoveredFragments.ScreenHookDecisions
static System.UInt16 DecrementSavedRow(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds)
static System.Boolean ShouldProbeCursor(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds, System.UInt16 ax)
static System.Boolean ShouldScroll(ScoreModern.RecoveredFragments.IStringBlockMemory m, System.UInt16 ds, System.Byte al, System.UInt16 dx)
```

## ScreenHookForwarding

```text
ScoreModern.RecoveredFragments.ScreenHookForwarding
static ScoreModern.RecoveredFragments.ScreenForwardResult RestoreAndResolve(ScoreModern.RecoveredFragments.IScreenForwardMemory m, System.UInt16 ss, System.UInt16 sp, System.UInt16 codeSegment)
```

## ScreenHookInterrupt

```text
ScoreModern.RecoveredFragments.ScreenHookInterrupt
static ScoreModern.RecoveredFragments.SignedDivide16State Enter(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## ScreenHookInterruptEffects

```text
ScoreModern.RecoveredFragments.ScreenHookInterruptEffects
.ctor(ScoreModern.RecoveredFragments.IScreenBiosTransferEffects biosEffects, ScoreModern.RecoveredFragments.ICapacityInterruptEffects otherInterruptEffects, System.UInt16 hookedSegment, System.UInt16 hookedOffset, System.UInt16 relocatedDataSegment)
ScoreModern.RecoveredFragments.SignedDivide16State Interrupt(System.Byte number, ScoreModern.RecoveredFragments.IHorizontalRasterMemory memory, ScoreModern.RecoveredFragments.SignedDivide16State state)
```

## ScreenHookOutcome

```text
ScoreModern.RecoveredFragments.ScreenHookOutcome
Forwarded = 0
ExternalControlTransfer = 1
```

## ScreenHookResult

```text
ScoreModern.RecoveredFragments.ScreenHookResult
ScoreModern.RecoveredFragments.ScreenHookOutcome Outcome
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScreenHookRoutine

```text
ScoreModern.RecoveredFragments.ScreenHookRoutine
static ScoreModern.RecoveredFragments.ScreenHookResult Execute(ScoreModern.RecoveredFragments.IScreenHookMemory m, ScoreModern.RecoveredFragments.ICapacityInterruptEffects fx, ScoreModern.RecoveredFragments.SignedDivide16State s, System.UInt16 dataSegment)
```

## SelectorCopy

```text
ScoreModern.RecoveredFragments.SelectorCopy
static System.Boolean Apply(System.Byte[] dataSegment, System.Int16 state, System.Boolean directionFlag)
```

## SelectorRoutine

```text
ScoreModern.RecoveredFragments.SelectorRoutine
static System.Void Execute(ScoreModern.RecoveredFragments.ISelectorContext context, System.UInt16 parameterSegment, System.UInt16 parameterOffset)
```

## SignedDivide16

```text
ScoreModern.RecoveredFragments.SignedDivide16
static ScoreModern.RecoveredFragments.SignedDivide16Result Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, ScoreModern.RecoveredFragments.SignedDivide16State s)
```

## SignedDivide16Fault

```text
ScoreModern.RecoveredFragments.SignedDivide16Fault
None = 0
ZeroDivisor = 1
QuotientOverflow = 2
```

## SignedDivide16Result

```text
ScoreModern.RecoveredFragments.SignedDivide16Result
UInt32 DefinedEFlagsMask
ScoreModern.RecoveredFragments.SignedDivide16Fault Fault
ScoreModern.RecoveredFragments.SignedDivide16State State
```

## SignedDivide16State

```text
ScoreModern.RecoveredFragments.SignedDivide16State
UInt16 AX
UInt16 BP
UInt16 BX
UInt16 CS
UInt16 CX
UInt16 DI
UInt16 DS
UInt16 DX
UInt32 EFlags
UInt16 ES
UInt16 IP
UInt16 SI
UInt16 SP
UInt16 SS
```

## SingleFixedDecimal

```text
ScoreModern.RecoveredFragments.SingleFixedDecimal
static System.String Format(System.Single value, System.Int32 decimalPlaces)
```

## SlopedRaster

```text
ScoreModern.RecoveredFragments.SlopedRaster
static ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx)
static ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt32 eflags)
```

## SlurGeometry

```text
ScoreModern.RecoveredFragments.SlurGeometry
static ScoreModern.RecoveredFragments.SlurGeometryResult Build(System.Single[] parameters, ScoreModern.RecoveredFragments.SlurOriginContext context, System.Boolean printed, System.Single configuredControl, ScoreModern.RecoveredFragments.SlurVectorBuffer caller, System.Boolean rejectInvalidTrims)
```

## SlurGeometryResult

```text
ScoreModern.RecoveredFragments.SlurGeometryResult
.ctor()
ScoreModern.RecoveredFragments.SlurVectorBuffer Buffer
System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
Int32 Root
```

## SlurLineWidth

```text
ScoreModern.RecoveredFragments.SlurLineWidth
static System.Single Field(System.Int16 selector, System.Int16 basis, System.Single unit)
static System.String Format(System.Single value)
static System.Single Printed(System.Int16 selector)
static System.Single RawField(System.Int16 selector, System.Int16 basis, System.Single unit)
Single DefaultUnit
```

## SlurOriginContext

```text
ScoreModern.RecoveredFragments.SlurOriginContext
.ctor()
Single LeftX
Single LeftY
Single RightY
Single Scale
```

## SlurOrigins

```text
ScoreModern.RecoveredFragments.SlurOrigins
static ScoreModern.RecoveredFragments.SlurOriginState Distance(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext c)
static ScoreModern.RecoveredFragments.SlurOriginState Prepare(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext c)
```

## SlurOriginState

```text
ScoreModern.RecoveredFragments.SlurOriginState
.ctor()
Single CurveCenter
Single DeltaY
Single EffectiveStartFraction
Single Height
Single HorizontalSpan
Single LeftX
Single LeftY
Single Length
Int16 NumberWord
Int16 OffsetWord
Single OriginalLeft
Single OriginalRight
Single RightX
Single StartFraction
Single StopFraction
Single WorkingRight
```

## SlurPlotRequest

```text
ScoreModern.RecoveredFragments.SlurPlotRequest
.ctor(System.Int16 k, System.Int16 x, System.Int16 y)
Int16 Kind
Int16 X
Int16 Y
```

## SlurPostprocess

```text
ScoreModern.RecoveredFragments.SlurPostprocess
static ScoreModern.RecoveredFragments.SlurVectorBuffer From(ScoreModern.RecoveredFragments.SlurSampleResult sample, ScoreModern.RecoveredFragments.SlurThicknessResult thickness, System.Boolean printed)
static System.Void Pattern(ScoreModern.RecoveredFragments.SlurVectorBuffer b, System.Single[] p, System.Single scale, System.Boolean printed)
static System.Void Rotate(ScoreModern.RecoveredFragments.SlurVectorBuffer b, ScoreModern.RecoveredFragments.SlurOriginState o)
static System.Void Trim(ScoreModern.RecoveredFragments.SlurVectorBuffer b, System.Single[] p, System.Single start, System.Single stop, System.Boolean printed)
```

## SlurSampleResult

```text
ScoreModern.RecoveredFragments.SlurSampleResult
.ctor()
Int32 Count
Single Exponent
Single FinalIncrement
Single FinalX
Single HalfSteps
Single Height
ScoreModern.RecoveredFragments.SlurOriginState Origin
Single Step
Int16[] X
Int16[] Y
```

## SlurSampling

```text
ScoreModern.RecoveredFragments.SlurSampling
static ScoreModern.RecoveredFragments.SlurSampleResult Build(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext context, System.Boolean printed)
static ScoreModern.RecoveredFragments.SlurSampleResult BuildState(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext context, ScoreModern.RecoveredFragments.SlurOriginState o, System.Boolean printed, System.Boolean trimmed)
```

## SlurThickness

```text
ScoreModern.RecoveredFragments.SlurThickness
static ScoreModern.RecoveredFragments.SlurThicknessResult Build(System.Single[] p, ScoreModern.RecoveredFragments.SlurOriginContext context, ScoreModern.RecoveredFragments.SlurSampleResult sample, System.Single configuredControl)
static System.Void Reflect(System.Single[] p, ScoreModern.RecoveredFragments.SlurSampleResult sample, System.Int16[] y, System.Boolean printed)
```

## SlurThicknessResult

```text
ScoreModern.RecoveredFragments.SlurThicknessResult
.ctor()
Single EffectiveControl
Int32 FinalIncrement
Single FinalRamp
Single RampStep
Int32 Target
Single Thickness
Int16[] X
Int16[] Y
```

## SlurVectorBuffer

```text
ScoreModern.RecoveredFragments.SlurVectorBuffer
.ctor()
.ctor(System.UInt16 xSegment, System.UInt16 ySegment)
Single Angle
Int32 Base
Int32 Capacity
Single Cos
Int32 CurveEnd
Int32 End
Int32 SampleCount
Single Sin
ScoreModern.RecoveredFragments.SlurWordSegment X
ScoreModern.RecoveredFragments.SlurWordSegment Y
System.Void Copy(System.Int32 destination, System.Int32 source)
System.Int16[] ExtractX()
System.Int16[] ExtractY()
System.Void Move(System.Int32 destination, System.Int32 source, System.Int32 count)
```

## SlurVectorCommands

```text
ScoreModern.RecoveredFragments.SlurVectorCommands
static System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.SlurPlotRequest] Decode(ScoreModern.RecoveredFragments.SlurVectorBuffer b, System.Int32 root, System.Boolean appendRestore, System.Boolean printed)
```

## SlurWordSegment

```text
ScoreModern.RecoveredFragments.SlurWordSegment
.ctor(System.Byte[] bytes, System.Int32 address)
Int16 Item [Int32]
Int32 Length
```

## SpanBookkeeping

```text
ScoreModern.RecoveredFragments.SpanBookkeeping
static System.UInt16 Execute(ScoreModern.RecoveredFragments.ISpanBookkeepingMemory m, System.UInt16 ss, System.UInt16 sourceArgumentOffset, System.UInt16 ds, System.UInt16 remainingSi, System.UInt16 transferredDi)
```

## StaffHeightLayout

```text
ScoreModern.RecoveredFragments.StaffHeightLayout
static System.Single[] Targets(System.Single[] positions, System.Single[] sizes, System.Int32 maximum, System.Int32[] groups, System.Single oldHeight, System.Single newHeight, System.Boolean metric, System.Single[] physicalPositions)
```

## StringCopyResult

```text
ScoreModern.RecoveredFragments.StringCopyResult
.ctor(System.UInt16 si, System.UInt16 di)
UInt16 CX
UInt16 DI
UInt16 SI
```

## StringFillResult

```text
ScoreModern.RecoveredFragments.StringFillResult
.ctor(System.UInt16 di)
UInt16 CX
UInt16 DI
```

## StringOutputResult

```text
ScoreModern.RecoveredFragments.StringOutputResult
Boolean ReachedStringReturn
ScoreModern.RecoveredFragments.CharacterRoutineResult Result
```

## StringOutputRoutine

```text
ScoreModern.RecoveredFragments.StringOutputRoutine
static ScoreModern.RecoveredFragments.StringOutputResult Run(ScoreModern.RecoveredFragments.IStringOutputContext m, ScoreModern.RecoveredFragments.CharacterRoutineWords w, System.UInt16 currentSegment, System.UInt16 characterSegment)
```

## TextConverterFontState

```text
ScoreModern.RecoveredFragments.TextConverterFontState
Boolean Forced
Int16 Word
System.Void Accept(System.Int16 token)
static ScoreModern.RecoveredFragments.TextConverterFontState Begin(System.Int16 overrideWord, System.Int16 previousWord)
Int32 MetricFace
```

## TextMetricArithmetic

```text
ScoreModern.RecoveredFragments.TextMetricArithmetic
static System.Single Accumulate(System.Single total, System.Single advance)
static System.Single AddPosition(System.Int16 index, System.Single x, System.Single advance, System.Single units)
static System.Single Advance(System.Single metric, ScoreModern.RecoveredFragments.TextMetricScale scale, System.Boolean space)
static System.Single Finish(System.Single total, System.Single multiplier)
static System.Single Normalize(System.Int16 width, System.Single divisor)
static ScoreModern.RecoveredFragments.TextMetricScale Prepare(System.Single size, System.Single staff, System.Single divisor, System.Single factor, System.Single extraSpace)
static System.Single StrokeHorizontal(System.Single metric, System.Single horizontal, System.Single primitiveMetric)
```

## TextMetricScale

```text
ScoreModern.RecoveredFragments.TextMetricScale
Single Character
Single ExtraSpace
```

## TextParameterPreparation

```text
ScoreModern.RecoveredFragments.TextParameterPreparation
static System.Single[] Adjust(System.Single[] advances, System.Single delta)
static System.Single Interletter(System.Single total, System.Single factor, System.Single[] advances)
static System.Single OutputExtra(System.Single value, System.Boolean printed, System.Single units)
static ScoreModern.RecoveredFragments.TextPreparedParameters Prepare(System.Single[] p, System.Single x, System.Single modulus, System.Single glyphScale, System.Single units)
```

## TextPreparedParameters

```text
ScoreModern.RecoveredFragments.TextPreparedParameters
Single GlyphHorizontal
Single GlyphVertical
Single Horizontal
Single Origin
Single RestoredHorizontal
Single RestoredVertical
Single Rotation
Single Spacing
Single Vertical
```

## TranspositionAccidentalTailResult

```text
ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult
.ctor()
Int16 ActiveRelationships
Int16[] Flags
Int16 RelationshipIndex
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow Window
```

## TranspositionClefState

```text
ScoreModern.RecoveredFragments.TranspositionClefState
.ctor(System.Int16 staff, System.Int16 target)
Int16 RetainedStaff
Int16 RetainedTarget
```

## TranspositionCollection

```text
ScoreModern.RecoveredFragments.TranspositionCollection
.ctor()
Int16 FirstKeyState
System.Nullable`1[System.Single] FirstNotePosition
Boolean HasExtendedBeam
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TranspositionRecord] Records
System.Collections.Generic.List`1[System.Int32] RelationshipItems
```

## TranspositionCollectorResult

```text
ScoreModern.RecoveredFragments.TranspositionCollectorResult
.ctor()
System.Collections.Generic.List`1[System.Single] CollectedBoundaries
Single ExpandedLeft
Single ExpandedRight
ScoreModern.RecoveredFragments.TranspositionCollection FirstPass
ScoreModern.RecoveredFragments.TranspositionPartition Partition
```

## TranspositionCollectorState

```text
ScoreModern.RecoveredFragments.TranspositionCollectorState
.ctor(System.Single firstNotePosition)
Single FirstNotePosition
```

## TranspositionInterval

```text
ScoreModern.RecoveredFragments.TranspositionInterval
.ctor()
Int16 Diatonic
Int16 Key
Int16 SemitoneClass
Int16 Semitones
```

## TranspositionKeyRecordResult

```text
ScoreModern.RecoveredFragments.TranspositionKeyRecordResult
.ctor()
Int16 Diatonic
Int16 KeyOverride
Int16 NewKeyBase
Int16 OldKeyBase
Single[] Parameters
Int16 PitchBase
```

## TranspositionMainPassResult

```text
ScoreModern.RecoveredFragments.TranspositionMainPassResult
.ctor()
Single StopPosition
ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult Tail
```

## TranspositionMainState

```text
ScoreModern.RecoveredFragments.TranspositionMainState
.ctor()
Int16[] AccidentalMarkers
Int16 Diatonic
Single ErasedCode
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow InitialWindow
Int16 KeyOverride
Int16 KindZeroGuard
Single RelationshipMargin
Single RelationshipWidth
Int16 Semitones
Single Sentinel
Int16 TargetKey
```

## TranspositionOperationResult

```text
ScoreModern.RecoveredFragments.TranspositionOperationResult
.ctor()
ScoreModern.RecoveredFragments.TranspositionCollectorResult Collection
ScoreModern.RecoveredFragments.TranspositionMainPassResult Engine
Boolean InsertedKey
ScoreModern.RecoveredFragments.TranspositionInterval Interval
Boolean KeyQuestion
System.Collections.Generic.List`1[System.Single[]] Rows
Boolean SortMarksModified
Boolean WarningRequired
```

## TranspositionOperationSession

```text
ScoreModern.RecoveredFragments.TranspositionOperationSession
.ctor()
ScoreModern.RecoveredFragments.TranspositionOperationResult Result
```

## TranspositionPartition

```text
ScoreModern.RecoveredFragments.TranspositionPartition
.ctor()
Single[] Boundaries
Int16[] Kinds
```

## TranspositionPitchResult

```text
ScoreModern.RecoveredFragments.TranspositionPitchResult
.ctor()
System.Nullable`1[System.Int16] AccidentalMarker
Single Pitch
Single Stem
```

## TranspositionRecord

```text
ScoreModern.RecoveredFragments.TranspositionRecord
.ctor()
Int16 Accidental
Int16 Clef
Int32 Item
Int16 Kind
System.Nullable`1[System.Int16] Pitch
Single Position
```

## TranspositionRelationshipWindow

```text
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow
Single EndHigh
Single EndLow
Single StartHigh
Single StartLow
```

## TranspositionRequest

```text
ScoreModern.RecoveredFragments.TranspositionRequest
.ctor()
Boolean AddKey
System.Nullable`1[System.Int16] Clef
Int16[] ClefChanges
Boolean ForceKeyQuestion
Single Left
Single Right
Int16 Semitones
Single Staff
```

## TranspositionWorkspace

```text
ScoreModern.RecoveredFragments.TranspositionWorkspace
.ctor(ScoreModern.RecoveredFragments.TranspositionCollectorState collector, ScoreModern.RecoveredFragments.TranspositionClefState clefs, System.Int16[] pitchWords, System.Int16[] markers, System.Int16 guard, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow window, System.Boolean sorted)
Int16[] AccidentalMarkers
Boolean AlreadySorted
ScoreModern.RecoveredFragments.TranspositionClefState Clefs
ScoreModern.RecoveredFragments.TranspositionCollectorState Collector
Int16 KindZeroGuard
Int16[] PitchWords
ScoreModern.RecoveredFragments.TranspositionRelationshipWindow Window
```

## TupletBracket

```text
ScoreModern.RecoveredFragments.TupletBracket
static ScoreModern.RecoveredFragments.TupletBracketResult Build(System.Single[] p, ScoreModern.RecoveredFragments.TupletBracketContext c)
```

## TupletBracketContext

```text
ScoreModern.RecoveredFragments.TupletBracketContext
.ctor()
Single LeftX
Single LeftY
Single RightY
Single Scale
```

## TupletBracketPoint

```text
ScoreModern.RecoveredFragments.TupletBracketPoint
.ctor(System.Int16 tag, System.Single x, System.Single y)
Int16 Tag
Single X
Single Y
```

## TupletBracketResult

```text
ScoreModern.RecoveredFragments.TupletBracketResult
.ctor()
System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TupletBracketPoint] Points
```

## TupletGeometry

```text
ScoreModern.RecoveredFragments.TupletGeometry
static ScoreModern.RecoveredFragments.TupletPlacement Build(System.Int16 number, System.Int32 direction, System.Boolean cue, System.Single percent, System.Single verticalShift, System.Single scale, System.Single leftX, System.Single leftY, System.Single dx, System.Single dy)
static System.Single Center(System.Int16 number, System.Single scale, System.Single along, System.Single left)
static System.String NumberText(System.Int16 number)
static System.Single PrintedEm(System.Single fontScale, System.Single staffScale)
```

## TupletPlacement

```text
ScoreModern.RecoveredFragments.TupletPlacement
Single FontScale
Int16 Number
Single X
Single Y
```

## VerticalRaster

```text
ScoreModern.RecoveredFragments.VerticalRaster
static ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx)
static ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory m, System.UInt16 ds, System.UInt16 es, System.UInt16 ax, System.UInt16 bx, System.UInt16 cx, System.UInt16 dx, System.UInt32 eflags)
```

## VerticalRasterWords

```text
ScoreModern.RecoveredFragments.VerticalRasterWords
UInt16 AX
UInt16 BP
UInt16 BX
UInt16 CX
UInt32 DefinedEFlagsMask
UInt16 DI
UInt16 DX
UInt32 EFlags
UInt16 SI
```

