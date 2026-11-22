# ScoreModern.dll public member index

Generated from shared DLL file version 0.3.61.2. Additive Parts helpers are documented in Chapter 06; native and modern callers remain explicit.

Assembly: ScoreModern, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

Exported types: 340

## ScoreModern.RecoveredFragments.AutomaticPrintOffset

```text
Method Int32 FromMinimumWord(Int16)
Method Int32 SelectBottom(Int32)
```

## ScoreModern.RecoveredFragments.BeamAppearance

```text
Field ScoreModern.RecoveredFragments.BeamAppearance DosScreen
Field ScoreModern.RecoveredFragments.BeamAppearance Printed
Field Int32 value__
```

## ScoreModern.RecoveredFragments.BeamBufferEdit

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IPointBufferMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.BeamGeometry

```text
Method System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.BeamShape] Build(Single[], ScoreModern.RecoveredFragments.BeamGeometryContext, ScoreModern.RecoveredFragments.BeamAppearance)
Method System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.BeamShape] Build(Single[], ScoreModern.RecoveredFragments.BeamGeometryContext, ScoreModern.RecoveredFragments.BeamAppearance, ScoreModern.RecoveredFragments.BeamReference ByRef)
```

## ScoreModern.RecoveredFragments.BeamGeometryContext

```text
Constructor Void .ctor()
Field Single LeftVertical
Field Single Scale
Field System.Func`2[System.Single,System.Single] Vertical
```

## ScoreModern.RecoveredFragments.BeamReference

```text
Field Boolean Cue
Field Single Dx
Field Single Dy
Field Single LeftX
Field Single LeftY
Field Single StaffScale
```

## ScoreModern.RecoveredFragments.BeamShape

```text
Constructor Void .ctor()
Field Int32 Group
Field ScoreModern.RecoveredFragments.BeamVertex[] Vertices
```

## ScoreModern.RecoveredFragments.BeamStemAdjustment

```text
Method System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]], Int32, Boolean, ScoreModern.RecoveredFragments.PageCoordinates, Int32, Int32, System.Collections.Generic.ISet`1[System.Int32])
Method System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]], Int32, Boolean, ScoreModern.RecoveredFragments.PageCoordinates, Int32, Int32, System.Collections.Generic.ISet`1[System.Int32], Boolean)
Method System.Collections.Generic.List`1[System.Single[]] ApplyScor4Selected(System.Collections.Generic.IList`1[System.Single[]], Int32, Boolean, Boolean, Int32)
Method System.Collections.Generic.List`1[System.Single[]] ConfigureScor4Selected(System.Collections.Generic.IList`1[System.Single[]], Int32, Boolean)
```

## ScoreModern.RecoveredFragments.BeamVertex

```text
Constructor Void .ctor(Single, Single)
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.BiosKeyReader

```text
Method Void Execute(ScoreModern.RecoveredFragments.IBiosKeyContext, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.BufferedBeamLinePipeline

```text
Method ScoreModern.RecoveredFragments.LineDriverBodyResult Execute(ScoreModern.RecoveredFragments.IByteOrResultMemory, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
```

## ScoreModern.RecoveredFragments.BufferedCoordinateDecode

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IPointBufferMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.BufferedDrawControl

```text
Method ScoreModern.RecoveredFragments.BufferedDrawWords DecodeMarker(ScoreModern.RecoveredFragments.IBufferedDrawMemory, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.BufferedDrawWords SelectRasterPath(ScoreModern.RecoveredFragments.IBufferedDrawMemory, UInt16)
Method ScoreModern.RecoveredFragments.BufferedDrawWords UpdatePosition(ScoreModern.RecoveredFragments.IBufferedDrawMemory, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.BufferedDrawWords

```text
Field UInt16 AX
Field Boolean Branch
Field UInt16 CX
```

## ScoreModern.RecoveredFragments.BufferedPointCaller

```text
Method ScoreModern.RecoveredFragments.BufferedPointCallerResult Enter(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.BufferedPointCallerResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
Method ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteCapacityAtComparison(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityCallEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteCapacityAtComparisonThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityCallEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
Method ScoreModern.RecoveredFragments.BufferedPointCallerResult ExecuteWithCapacityEffects(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.IBufferedPointFpuEffects, ScoreModern.RecoveredFragments.ICapacityCallEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.BufferedPointCallerResult

```text
Field ScoreModern.RecoveredFragments.BufferedPointOutcome Outcome
Field UInt16 RequestedCalleeOffset
Field Boolean RetfExecuted
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.BufferedPointOutcome

```text
Field ScoreModern.RecoveredFragments.BufferedPointOutcome CapacityHandlerRequired
Field ScoreModern.RecoveredFragments.BufferedPointOutcome Completed
Field ScoreModern.RecoveredFragments.BufferedPointOutcome ExternalControlTransfer
Field ScoreModern.RecoveredFragments.BufferedPointOutcome NormalPathReady
Field Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityCallKind

```text
Field ScoreModern.RecoveredFragments.CapacityCallKind Followup
Field ScoreModern.RecoveredFragments.CapacityCallKind StringOutput
Field Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityCharacterNativeEffects

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.ICapacityInterruptEffects)
Method ScoreModern.RecoveredFragments.SignedDivide16State CallCharacter(ScoreModern.RecoveredFragments.ICapacityStringMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.CapacityCharacterOutput

```text
Method ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityStringMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityStringMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.CapacityDeviceCallees

```text
Method ScoreModern.RecoveredFragments.CapacityDeviceResult KeyStatus(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.CapacityDeviceResult KeyStatusThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.CapacityDeviceResult Poll(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.CapacityDeviceResult PollThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.CapacityDeviceOutcome

```text
Field ScoreModern.RecoveredFragments.CapacityDeviceOutcome ExternalControlTransfer
Field ScoreModern.RecoveredFragments.CapacityDeviceOutcome ReachedReturn
Field Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityDeviceResult

```text
Field ScoreModern.RecoveredFragments.CapacityDeviceOutcome Outcome
Field Boolean RetfExecuted
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.CapacityFollowup

```text
Method ScoreModern.RecoveredFragments.CapacityFollowupResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityFollowupEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
Method ScoreModern.RecoveredFragments.CapacityFollowupResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityFollowupEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
```

## ScoreModern.RecoveredFragments.CapacityFollowupCallKind

```text
Field ScoreModern.RecoveredFragments.CapacityFollowupCallKind KeyRead
Field ScoreModern.RecoveredFragments.CapacityFollowupCallKind KeyStatus
Field ScoreModern.RecoveredFragments.CapacityFollowupCallKind Poll
Field ScoreModern.RecoveredFragments.CapacityFollowupCallKind PromptOutput
Field Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityFollowupNativeEffects

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.ICapacityCharacterEffects, UInt16)
Method ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityFollowupCallKind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.CapacityFollowupOutcome

```text
Field ScoreModern.RecoveredFragments.CapacityFollowupOutcome ExternalControlTransfer
Field ScoreModern.RecoveredFragments.CapacityFollowupOutcome ReachedReturn
Field ScoreModern.RecoveredFragments.CapacityFollowupOutcome UnverifiedBranch
Field Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityFollowupResult

```text
Field ScoreModern.RecoveredFragments.CapacityFollowupOutcome Outcome
Field Boolean RetfExecuted
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.CapacityKeyReader

```text
Method ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityKeyMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityKeyMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.CapacityMessageHandler

```text
Method ScoreModern.RecoveredFragments.CapacityMessageResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityCallEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.CapacityMessageResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICapacityCallEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.CapacityMessageNativeEffects

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.ICapacityCharacterEffects, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityCallKind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.CapacityMessageNativeEffects CreateWithNativeCharacters(ScoreModern.RecoveredFragments.ICapacityInterruptEffects, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.CapacityMessageOutcome

```text
Field ScoreModern.RecoveredFragments.CapacityMessageOutcome ExternalControlTransfer
Field ScoreModern.RecoveredFragments.CapacityMessageOutcome ReachedReturn
Field Int32 value__
```

## ScoreModern.RecoveredFragments.CapacityMessageResult

```text
Field ScoreModern.RecoveredFragments.CapacityMessageOutcome Outcome
Field Boolean RetfExecuted
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.CapacityStringOutput

```text
Method ScoreModern.RecoveredFragments.CapacityDeviceResult Execute(ScoreModern.RecoveredFragments.ICapacityStringMemory, ScoreModern.RecoveredFragments.ICapacityCharacterEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
Method ScoreModern.RecoveredFragments.CapacityDeviceResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.ICapacityStringMemory, ScoreModern.RecoveredFragments.ICapacityCharacterEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
```

## ScoreModern.RecoveredFragments.CharacterAlternateOutput

```text
Method ScoreModern.RecoveredFragments.CharacterBiosWords Run(ScoreModern.RecoveredFragments.ICharacterBiosContext, ScoreModern.RecoveredFragments.CharacterBiosWords)
```

## ScoreModern.RecoveredFragments.CharacterBiosWords

```text
Field UInt16 AX
Field UInt16 BX
Field UInt16 CX
Field UInt16 DX
Field UInt16 SP
Field UInt16 SS
```

## ScoreModern.RecoveredFragments.CharacterDecisionResult

```text
Constructor Void .ctor(Byte, UInt16, Boolean)
Field Byte AL
Field Boolean Branch
Field UInt16 BX
```

## ScoreModern.RecoveredFragments.CharacterEntryWords

```text
Field UInt16 BP
Field UInt16 DI
Field UInt16 ES
Field UInt16 SP
```

## ScoreModern.RecoveredFragments.CharacterOutputDecisions

```text
Method ScoreModern.RecoveredFragments.CharacterDecisionResult ProbeAlternateGate(ScoreModern.RecoveredFragments.ICharacterDecisionMemory, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.CharacterDecisionResult ProbeBackspace(ScoreModern.RecoveredFragments.ICharacterDecisionMemory, UInt16, UInt16, UInt16, Byte, UInt16)
```

## ScoreModern.RecoveredFragments.CharacterOutputFrame

```text
Method ScoreModern.RecoveredFragments.CharacterEntryWords Enter(ScoreModern.RecoveredFragments.ICharacterFrameMemory, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.CharacterReturnWords Leave(ScoreModern.RecoveredFragments.ICharacterFrameMemory, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.CharacterOutputRoutine

```text
Method ScoreModern.RecoveredFragments.CharacterRoutineResult Run(ScoreModern.RecoveredFragments.ICharacterRoutineContext, ScoreModern.RecoveredFragments.CharacterRoutineWords)
```

## ScoreModern.RecoveredFragments.CharacterReturnWords

```text
Field UInt16 BP
Field UInt16 CS
Field UInt16 DI
Field UInt16 IP
Field UInt16 SP
```

## ScoreModern.RecoveredFragments.CharacterRoutineResult

```text
Field UInt16 CS
Field UInt16 IP
Field ScoreModern.RecoveredFragments.CharacterRoutineWords Words
```

## ScoreModern.RecoveredFragments.CharacterRoutineWords

```text
Field UInt16 AX
Field UInt16 BP
Field UInt16 BX
Field UInt16 CX
Field UInt16 DI
Field UInt16 DS
Field UInt16 DX
Field UInt16 ES
Field UInt16 SI
Field UInt16 SP
Field UInt16 SS
```

## ScoreModern.RecoveredFragments.ClippingCpu

```text
Method ScoreModern.RecoveredFragments.SignedDivide16Result Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ConsumerContextSaver

```text
Method UInt16 Execute(ScoreModern.RecoveredFragments.IConsumerContextMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.CoordinateConversion

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICoordinateFpuEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.SignedDivide16State ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.ICoordinateFpuEffects, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.CoordinateWordClamp

```text
Method UInt32 Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.DatTextLookup

```text
Method ScoreModern.RecoveredFragments.DatTextLookupWords Read(ScoreModern.RecoveredFragments.IDatTextLookupMemory, UInt16)
```

## ScoreModern.RecoveredFragments.DatTextLookupWords

```text
Field UInt16 AX
Field UInt16 BX
Field UInt16 DX
```

## ScoreModern.RecoveredFragments.DirectionalStringBlocks

```text
Method ScoreModern.RecoveredFragments.StringCopyResult Copy(ScoreModern.RecoveredFragments.IStringBlockMemory, UInt16, UInt16, UInt16, UInt16, UInt16, Boolean)
Method ScoreModern.RecoveredFragments.StringFillResult Fill(ScoreModern.RecoveredFragments.IStringBlockMemory, UInt16, UInt16, UInt16, UInt16, Boolean)
```

## ScoreModern.RecoveredFragments.EndingBracket

```text
Method ScoreModern.RecoveredFragments.EndingBracketResult Build(Single[], ScoreModern.RecoveredFragments.TupletBracketContext, Boolean, Boolean)
Method ScoreModern.RecoveredFragments.TupletBracketPoint DotPoint(Single, Single, Single, ScoreModern.RecoveredFragments.EndingBracketResult)
```

## ScoreModern.RecoveredFragments.EndingBracketResult

```text
Constructor Void .ctor()
Field Single DotX
Field Single DotY
Field Boolean HasDot
Field System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
Field System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TupletBracketPoint] Points
```

## ScoreModern.RecoveredFragments.EqualStepRaster

```text
Method ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.ExternalVgaFont

```text
Constructor Void .ctor(Byte[])
Method Byte Row(Byte, Int32)
```

## ScoreModern.RecoveredFragments.FiniteBinary32Integer64

```text
Method Boolean TryConvert(UInt32, Int64 ByRef)
```

## ScoreModern.RecoveredFragments.FormatterWordWriter

```text
Method Void Execute(ScoreModern.RecoveredFragments.IFormatterWordContext, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.HorizontalRaster

```text
Method ScoreModern.RecoveredFragments.HorizontalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.HorizontalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.HorizontalRasterWords

```text
Field UInt16 AX
Field UInt16 BX
Field UInt16 CX
Field UInt32 DefinedEFlagsMask
Field UInt16 DI
Field UInt16 DX
Field UInt32 EFlags
Field UInt16 SI
```

## ScoreModern.RecoveredFragments.IBiosKeyContext

```text
Method UInt16 ReadBiosKey()
Method Void WriteByte(UInt16, UInt16, Byte)
```

## ScoreModern.RecoveredFragments.IBufferedDrawMemory

```text
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IBufferedPointFpuEffects

```text
Method Void LoadBinary32(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IByteOrResultMemory

```text
Method Byte OrByteAndGetResult(UInt16, UInt16, Byte)
```

## ScoreModern.RecoveredFragments.ICapacityCallEffects

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityCallKind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ICapacityCharacterEffects

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State CallCharacter(ScoreModern.RecoveredFragments.ICapacityStringMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ICapacityFollowupEffects

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Call(ScoreModern.RecoveredFragments.CapacityFollowupCallKind, ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ICapacityInterruptEffects

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Interrupt(Byte, ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ICapacityKeyMemory

```text
Method Void WriteByte(UInt16, UInt16, Byte)
```

## ScoreModern.RecoveredFragments.ICapacityStringMemory

```text
Method Byte ReadByte(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ICharacterBiosContext

```text
Method Void Interrupt10(ScoreModern.RecoveredFragments.CharacterBiosWords ByRef)
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ICharacterDecisionMemory

```text
Method Byte ReadByte(UInt16, UInt16)
Method UInt16 ReadWord(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ICharacterFrameMemory

```text
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ICharacterRoutineContext

```text
Method Void Interrupt10(ScoreModern.RecoveredFragments.CharacterRoutineWords ByRef, UInt16)
Method UInt16 ReadWord(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IConsumerContextMemory

```text
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ICoordinateFpuEffects

```text
Method Void LoadControlWord(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16)
Method Void StoreControlWord(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16)
Method Void StoreInteger64AndPop(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16)
Method Void Wait()
```

## ScoreModern.RecoveredFragments.IDatTextLookupMemory

```text
Method UInt16 ReadWord(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IFormatterWordContext

```text
Method UInt16 ReadWord(UInt16)
Method Void WriteByte(UInt16, Byte)
Method Void WriteWord(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IHorizontalRasterMemory

```text
Method Void OrByte(UInt16, UInt16, Byte)
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IMode12GlyphSource

```text
Method Byte Row(Byte, Int32)
```

## ScoreModern.RecoveredFragments.IMode12Surface

```text
Method Void Scroll(Int32, Int32, Int32, Int32, Int32, Byte)
Method Void WriteGlyph(Int32, Int32, Byte, Byte)
```

## ScoreModern.RecoveredFragments.IncludedEpsTransform

```text
Constructor Void .ctor()
Field Single BoxX
Field Single BoxY
Method ScoreModern.RecoveredFragments.IncludedEpsTransform Prepare(ScoreModern.RecoveredFragments.PageItemContext, Single, Single, Single, Single, Single, Single, Single, Single, Int32, Int32)
Field Boolean Rotate
Field Single Rotation
Field Single ScaleX
Field Single ScaleY
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.IncludedPostScriptVariables

```text
Constructor Void .ctor()
Field Single BottomOffset
Field Single GlobalSize
Field Single LeftOffset
Field Single LineWidth
Field Single PointsLevel
Field Single PP3
Field Single PP4
Field Single PP5
Field Single PP6
Method ScoreModern.RecoveredFragments.IncludedPostScriptVariables Prepare(ScoreModern.RecoveredFragments.PageItemContext, Single, Single, Single, Single, Single, Int32, Int32)
Field Single StaffLevel
Field Single StaffSize
```

## ScoreModern.RecoveredFragments.InputAttachedMarks

```text
Method Single DefaultValue(Int32, Single, Single)
```

## ScoreModern.RecoveredFragments.InputAutomaticBeamMeterException

```text
Constructor Void .ctor(Int32, Int32)
```

## ScoreModern.RecoveredFragments.InputAutomaticBeams

```text
Method Single[] Durations(System.Collections.Generic.IList`1[System.Single[]])
Method Single[] Groups(System.Collections.Generic.IList`1[System.Single[]], Single[], Single, Int32)
```

## ScoreModern.RecoveredFragments.InputBeamConstructionResult

```text
Constructor Void .ctor()
Field Int32[] ForcedStems
Field System.Collections.Generic.List`1[System.Single[]] Records
```

## ScoreModern.RecoveredFragments.InputBeamFit

```text
Method ScoreModern.RecoveredFragments.InputBeamFitResult Fit(Single[], Single[], Int32, Int32, Single)
```

## ScoreModern.RecoveredFragments.InputBeamFitResult

```text
Constructor Void .ctor()
Field Int32 Direction
Field Single End
Field Single Start
Field Single UnquantizedEnd
Field Single UnquantizedStart
```

## ScoreModern.RecoveredFragments.InputBeamLevels

```text
Method System.Collections.Generic.List`1[System.Single[]] Pack(Single[], Int32[], Single[])
```

## ScoreModern.RecoveredFragments.InputBeamStemCompletion

```text
Method System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]], Int32, ScoreModern.RecoveredFragments.InputEndpointStaffContext)
```

## ScoreModern.RecoveredFragments.InputChordDots

```text
Method Void Apply(System.Collections.Generic.IList`1[System.Single[]])
Method Void ApplyRange(System.Collections.Generic.IList`1[System.Single[]], Int32, Int32)
```

## ScoreModern.RecoveredFragments.InputChordLayout

```text
Method Int32 Apply(System.Collections.Generic.IList`1[System.Single[]], Int32, Int32)
```

## ScoreModern.RecoveredFragments.InputChordPitchRecords

```text
Method ScoreModern.RecoveredFragments.InputPitchResult CompileSpacedNaturals(Single[], Single, Single, Single)
```

## ScoreModern.RecoveredFragments.InputChordRhythmRecords

```text
Method System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]], Single[], Single, Single, Single, Int32[])
Method System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]], Single[], Single, Single, Single, Int32[], Boolean, Single, System.Nullable`1[System.Single], System.Nullable`1[System.Int16], System.Nullable`1[System.Single])
```

## ScoreModern.RecoveredFragments.InputChromaticPitchCorrection

```text
Method System.Collections.Generic.Dictionary`2[System.Int32,System.Int32] Apply(System.Collections.Generic.IList`1[System.Single[]], System.Collections.Generic.IList`1[System.Single[]], Int16, Int16)
```

## ScoreModern.RecoveredFragments.InputDynamics

```text
Method Single[] Create(Int32, Single[], Single, Single)
```

## ScoreModern.RecoveredFragments.InputEditInverse

```text
Method System.String Number(Single)
Method ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]], Int32, Single, Single, Boolean)
Method ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]], Int32, Single, Single, Boolean, Boolean)
Method ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]], Int32, Single, Single, Boolean, Boolean, Boolean)
Method ScoreModern.RecoveredFragments.InputEditInverseResult Project(System.Collections.Generic.IList`1[System.Single[]], Int32, Single, Single, Boolean, Boolean, Boolean, System.Nullable`1[System.Single])
```

## ScoreModern.RecoveredFragments.InputEditInverseResult

```text
Constructor Void .ctor()
Field System.String Header
Field Int32 InitialClef
Field Single Left
Field System.String[] Lines
Field System.String Positions
Field System.Collections.Generic.List`1[System.Single[]] Prefix
Field Single Right
Field System.Collections.Generic.List`1[System.Single[]] Selected
Field Int32 Staff
```

## ScoreModern.RecoveredFragments.InputEndpointProjection

```text
Method ScoreModern.RecoveredFragments.InputEndpointResult Decode(Single, Single, Int32, System.Collections.Generic.IList`1[System.Single[]], Int32[], Int32[], Single, Single, ScoreModern.RecoveredFragments.InputEndpointStaffContext)
```

## ScoreModern.RecoveredFragments.InputEndpointRepromptException

```text
Constructor Void .ctor()
```

## ScoreModern.RecoveredFragments.InputEndpointResult

```text
Constructor Void .ctor()
Field Int16 EndFlag
Field Int16 EndItem
Field Int16 EndNote
Field Single EndPitch
Field Single EndX
Field Int16 Number
Field Int16 StartFlag
Field Int16 StartItem
Field Int16 StartNote
Field Single StartPitch
Field Single StartX
Field Int16 Stem
```

## ScoreModern.RecoveredFragments.InputEndpointStaffContext

```text
Constructor Void .ctor(Single[], Single[], Int32)
Constructor Void .ctor(Single[], Single[], Int32, Boolean)
Method Void Adjust(Single[], Single ByRef)
Method Void AdjustBeamStem(Single[], Int32, Single ByRef)
Method ScoreModern.RecoveredFragments.InputEndpointStaffContext ForStaff(Int32)
```

## ScoreModern.RecoveredFragments.InputEndpointText

```text
Method ScoreModern.RecoveredFragments.InputEndpointTokens Parse(System.String, Boolean, Int32, Int32)
Method ScoreModern.RecoveredFragments.InputEndpointTokens Parse(System.String, Boolean, Int32, Int32, ScoreModern.RecoveredFragments.InputMacroContext)
```

## ScoreModern.RecoveredFragments.InputEndpointTokens

```text
Constructor Void .ctor()
Field Boolean Automatic
Field Single GroupControl
Field Single[] Pairs
Field Int32 Skip
```

## ScoreModern.RecoveredFragments.InputListExpansion

```text
Method Void Repeat(System.Collections.Generic.List`1[System.Single], Int32, Int32, Boolean)
Method Void RepeatBeforePrevious(System.Collections.Generic.List`1[System.Single], Boolean)
```

## ScoreModern.RecoveredFragments.InputMacroContext

```text
Constructor Void .ctor()
Method Void AllowReplacement(Char)
Method Void Clear()
Method Void ClearReplacementAllowances()
Method ScoreModern.RecoveredFragments.InputMacroContext Clone()
Property System.Collections.Generic.IEnumerable`1[System.Collections.Generic.KeyValuePair`2[System.Char,System.Single[]]] Entries
Method System.Collections.Generic.IEnumerable`1[System.Collections.Generic.KeyValuePair`2[System.Char,System.Single[]]] get_Entries()
Method Int32 get_StoredDefinitionCount()
Method Void ReplaceFrom(ScoreModern.RecoveredFragments.InputMacroContext)
Property Int32 StoredDefinitionCount
```

## ScoreModern.RecoveredFragments.InputMainNoteReconstruction

```text
Method System.Collections.Generic.List`1[System.Single[]] Build(System.Collections.Generic.IList`1[System.Single[]], Int32, System.Nullable`1[System.Int16], Boolean)
```

## ScoreModern.RecoveredFragments.InputMarksCompiler

```text
Method ScoreModern.RecoveredFragments.InputMarksResult Apply(System.Collections.Generic.IList`1[System.Single[]], Int32[], Single[], Int32[], Single, Single, Single, Single, Single)
Method ScoreModern.RecoveredFragments.InputMarksResult Apply(System.Collections.Generic.IList`1[System.Single[]], Int32[], Single[], Int32[], Single, Single, Single, Single, Single, Boolean)
```

## ScoreModern.RecoveredFragments.InputMarksResult

```text
Constructor Void .ctor()
Field Int32 FirstOrnament
Field System.Collections.Generic.List`1[System.Single[]] Records
```

## ScoreModern.RecoveredFragments.InputMarkText

```text
Method ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String)
Method ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String, Boolean)
Method ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String, ScoreModern.RecoveredFragments.InputMacroContext)
Method ScoreModern.RecoveredFragments.InputMarkTokens Parse(System.String, ScoreModern.RecoveredFragments.InputMacroContext, Boolean)
```

## ScoreModern.RecoveredFragments.InputMarkTokens

```text
Constructor Void .ctor()
Field Int32[] Opposite
Field Single[] Values
```

## ScoreModern.RecoveredFragments.InputMeterDuration

```text
Method Boolean TryQuarterBeats(Single, Single, Single, Single, Single ByRef)
```

## ScoreModern.RecoveredFragments.InputMotiveReplacementException

```text
Constructor Void .ctor(Char)
Field Char Name
```

## ScoreModern.RecoveredFragments.InputPitchRecords

```text
Method ScoreModern.RecoveredFragments.InputPitchResult Compile(Single[], Single, Int32, Single, Single, Boolean)
Method ScoreModern.RecoveredFragments.InputPitchResult CompileSingleNotes(Single[], Single, Int32, Single, Single, Boolean)
```

## ScoreModern.RecoveredFragments.InputPitchResult

```text
Constructor Void .ctor()
Field Int32 Events
Field Int32 FinalClef
Field System.Collections.Generic.List`1[System.Int32] ForcedStems
Field Single Left
Field System.Collections.Generic.List`1[System.Int32] NoteIndices
Field System.Collections.Generic.List`1[System.Single[]] Records
```

## ScoreModern.RecoveredFragments.InputPitchState

```text
Constructor Void .ctor()
Field Int32 BarSpan
Field Int32 KeySignature
Field Int32 MeterBottom
Field Int32 MeterTop
Field Int32 Octave
Field Int32 Previous
Field Int32 PreviousDegree
Field Boolean Proximity
```

## ScoreModern.RecoveredFragments.InputPitchToken

```text
Method System.Nullable`1[System.Single] Parse(System.String, ScoreModern.RecoveredFragments.InputPitchState)
```

## ScoreModern.RecoveredFragments.InputProximityDelimiterException

```text
Constructor Void .ctor(Int32)
Field Int32 Caret
```

## ScoreModern.RecoveredFragments.InputRealReader

```text
Method Single Read(System.String, Int32 ByRef)
```

## ScoreModern.RecoveredFragments.InputRecordCompaction

```text
Method Byte[] Compact(Single[])
```

## ScoreModern.RecoveredFragments.InputReferenceSpacing

```text
Constructor Void .ctor(System.Collections.Generic.IList`1[System.Single[]])
Constructor Void .ctor(System.Collections.Generic.IList`1[System.Single[]], System.Action`2[System.Int32,System.Single])
Method Boolean get_HasClefAtZero()
Property Boolean HasClefAtZero
```

## ScoreModern.RecoveredFragments.InputRhythmDecoder

```text
Method ScoreModern.RecoveredFragments.InputRhythmValue Decode(Single)
```

## ScoreModern.RecoveredFragments.InputRhythmRecords

```text
Method System.Collections.Generic.List`1[System.Single[]] Apply(System.Collections.Generic.IList`1[System.Single[]], Single[], Single, Single, Single, Boolean, Single, Int32[], System.Nullable`1[System.Single], System.Nullable`1[System.Single])
```

## ScoreModern.RecoveredFragments.InputRhythmToken

```text
Method Single Parse(System.String)
Method Single Parse(System.String, Boolean)
```

## ScoreModern.RecoveredFragments.InputRhythmValue

```text
Field Single Denominator
Field Single DotControl
Field Single Duration
Field Single FlagCount
```

## ScoreModern.RecoveredFragments.InputSetupBounds

```text
Method ScoreModern.RecoveredFragments.InputSetupBoundsResult Find(System.Collections.Generic.IList`1[System.Single[]], ScoreModern.RecoveredFragments.InputSetupState)
```

## ScoreModern.RecoveredFragments.InputSetupBoundsResult

```text
Constructor Void .ctor()
Field Int32 Clef
Field System.String Key
Field Int32 Left
Field Int32 MeterBottom
Field Int32 MeterTop
Field Int32 Right
```

## ScoreModern.RecoveredFragments.InputSetupState

```text
Constructor Void .ctor()
Field System.String Key
Method Void KeyText(System.String)
Field Int32 LeftEnd
Field Int32 LeftPosition
Field Int32 MeterBottom
Field Int32 MeterTop
Method Void MoveField(Int32)
Method Boolean Numeric(Single)
Method System.String PresetName(Int32)
Field Int32 RightEnd
Field Int32 RightPosition
Field Int32 Selected
Field Single Size
Field Int32 Spacing
Method Void SpacingLetter(Char)
Field Int32 Staff
Method Single[] StaffRecord()
Field Int32 Vertical
```

## ScoreModern.RecoveredFragments.InputSlurConstruction

```text
Method System.Collections.Generic.List`1[System.Single[]] Build(System.Collections.Generic.IList`1[System.Single[]], ScoreModern.RecoveredFragments.InputEndpointResult, Single, Int32, Int16, Single, ScoreModern.RecoveredFragments.InputEndpointStaffContext)
Method Single InitialHeight(Single, Single, Single, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.InputStaccatoCentering

```text
Method Boolean Apply(Single[], Single[])
```

## ScoreModern.RecoveredFragments.InputStaffStage

```text
Field ScoreModern.RecoveredFragments.InputStaffStage Beams
Field ScoreModern.RecoveredFragments.InputStaffStage Complete
Field ScoreModern.RecoveredFragments.InputStaffStage Marks
Field ScoreModern.RecoveredFragments.InputStaffStage Pitches
Field ScoreModern.RecoveredFragments.InputStaffStage Rhythms
Field ScoreModern.RecoveredFragments.InputStaffStage Slurs
Field Int32 value__
```

## ScoreModern.RecoveredFragments.InputStaffTransaction

```text
Constructor Void .ctor(Single, Int32, Single, Single, Single, Single, Single, Single, Int16)
Constructor Void .ctor(Single, Int32, Single, Single, Single, Single, Single, Single, Int16, Boolean, Single)
Method Void Beams(ScoreModern.RecoveredFragments.InputEndpointTokens)
Method Void Beams(Single[])
Method System.Collections.Generic.List`1[System.Single[]] Commit(System.Collections.Generic.IList`1[System.Single[]])
Method Int32 get_InitialClef()
Method Single get_OriginalSelectionLeft()
Method Single get_OriginalSelectionRight()
Method Single get_OriginalStaff()
Method System.String get_ReferenceSpacingWarning()
Method Int32 get_RhythmCount()
Method ScoreModern.RecoveredFragments.InputStaffStage get_Stage()
Method System.Collections.Generic.List`1[System.Single[]] get_WorkingRecords()
Property Int32 InitialClef
Method Void Marks(Single[], Int32[])
Method Void Marks(Single[], Int32[], Boolean)
Method Single[] MouseNotePositions()
Method Void ObservePitchPreviewLoader()
Property Single OriginalSelectionLeft
Property Single OriginalSelectionRight
Property Single OriginalStaff
Method Void Pitches(Single[], Boolean)
Method Void Pitches(Single[], Boolean, Boolean)
Method Void Pitches(Single[], Boolean, Boolean, Boolean)
Method System.Collections.Generic.List`1[System.Single[]] Preview(System.Collections.Generic.IList`1[System.Single[]])
Property System.String ReferenceSpacingWarning
Method ScoreModern.RecoveredFragments.InputStaffTransaction Restart(System.Nullable`1[System.Single], System.Nullable`1[System.Single], System.Nullable`1[System.Boolean])
Method ScoreModern.RecoveredFragments.InputStaffTransaction RestartOnStaff(Int32, Int32, Single, Single)
Method ScoreModern.RecoveredFragments.InputStaffTransaction RestartOnStaffRange(Int32, Int32, Single, Single, Single, Single, System.Nullable`1[System.Boolean])
Property Int32 RhythmCount
Method Void Rhythms(Single[])
Method Void Rhythms(Single[], Boolean)
Method Void SkipRhythms()
Method Void Slurs(Single[])
Property ScoreModern.RecoveredFragments.InputStaffStage Stage
Method Void UpdateInputMeter(Int32 ByRef, Int32 ByRef)
Method Void UseReferenceSpacing(ScoreModern.RecoveredFragments.InputReferenceSpacing)
Method Void UseStaffCoordinates(ScoreModern.RecoveredFragments.InputEndpointStaffContext)
Property System.Collections.Generic.List`1[System.Single[]] WorkingRecords
```

## ScoreModern.RecoveredFragments.InputTextLists

```text
Method Single[] Pitches(System.String, ScoreModern.RecoveredFragments.InputPitchState)
Method Single[] Pitches(System.String, ScoreModern.RecoveredFragments.InputPitchState, ScoreModern.RecoveredFragments.InputMacroContext)
Method Single[] Rhythms(System.String)
Method Single[] Rhythms(System.String, ScoreModern.RecoveredFragments.InputMacroContext)
Method Single[] Rhythms(System.String, ScoreModern.RecoveredFragments.InputMacroContext, Boolean)
```

## ScoreModern.RecoveredFragments.InputTextTerminatorWrite

```text
Method UInt16 Apply(Byte[], UInt16, UInt16)
Method UInt16 Offset(UInt16, UInt16)
Field Byte Space
```

## ScoreModern.RecoveredFragments.InputTranspositionAccidentalTail

```text
Method ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult Apply(System.Collections.Generic.IList`1[System.Single[]], System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord], Int16[], System.Collections.Generic.IList`1[System.Int32], Int16, Single, Int16, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow, Single, Single)
```

## ScoreModern.RecoveredFragments.InputTranspositionAttachments

```text
Method Void Apply(System.Collections.Generic.IList`1[System.Single[]], Int16, Single, Single, Single, Boolean)
Method Boolean ShouldApply(Int16, Boolean)
```

## ScoreModern.RecoveredFragments.InputTranspositionClefs

```text
Method Void Apply(System.Collections.Generic.IList`1[System.Single[]], System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord], Int16[], Single, Int16, ScoreModern.RecoveredFragments.TranspositionClefState)
```

## ScoreModern.RecoveredFragments.InputTranspositionCollection

```text
Method System.Collections.Generic.List`1[System.Single] Boundaries(System.Collections.Generic.IList`1[System.Single[]], Int16, Single, Single)
Method ScoreModern.RecoveredFragments.TranspositionCollection FirstPass(System.Collections.Generic.IList`1[System.Single[]], Int16, Single, Single, Int16)
Method ScoreModern.RecoveredFragments.TranspositionCollection FirstPass(System.Collections.Generic.IList`1[System.Single[]], Int16, Single, Single, Int16, Int16[])
```

## ScoreModern.RecoveredFragments.InputTranspositionCollector

```text
Method ScoreModern.RecoveredFragments.TranspositionCollectorResult Build(System.Collections.Generic.IList`1[System.Single[]], Int16, Single, Single, Int16, ScoreModern.RecoveredFragments.TranspositionCollectorState)
Method ScoreModern.RecoveredFragments.TranspositionCollectorResult Build(System.Collections.Generic.IList`1[System.Single[]], Int16, Single, Single, Int16, ScoreModern.RecoveredFragments.TranspositionCollectorState, Int16[])
```

## ScoreModern.RecoveredFragments.InputTranspositionEmbeddedControl

```text
Method Void Apply(System.Collections.Generic.IList`1[System.Single[]], Int16, Int16, Boolean ByRef)
Field System.String FingeringWarning
```

## ScoreModern.RecoveredFragments.InputTranspositionInterval

```text
Method ScoreModern.RecoveredFragments.TranspositionInterval Convert(Int16)
Method Int16 Menu(Int16)
```

## ScoreModern.RecoveredFragments.InputTranspositionKey

```text
Method Void Convert(Int16, Int16, Int16 ByRef, Int16 ByRef)
Method Int16 KeyPosition(Int16)
```

## ScoreModern.RecoveredFragments.InputTranspositionKeyInsertion

```text
Method Single[] Append(System.Collections.Generic.IList`1[System.Single[]], Single, Int16, Int16)
Method Boolean ShouldInsert(Boolean, Int16, Int16)
Method Boolean ShouldPrompt(Boolean, Int16, Int16)
```

## ScoreModern.RecoveredFragments.InputTranspositionKeyRecord

```text
Method ScoreModern.RecoveredFragments.TranspositionKeyRecordResult Apply(Single[], Int16, Int16, Int16, Int16, Int16, Single)
```

## ScoreModern.RecoveredFragments.InputTranspositionMainPass

```text
Method ScoreModern.RecoveredFragments.TranspositionMainPassResult Apply(System.Collections.Generic.IList`1[System.Single[]], System.Collections.Generic.IList`1[ScoreModern.RecoveredFragments.TranspositionRecord], Int16[], System.Collections.Generic.IList`1[System.Int32], ScoreModern.RecoveredFragments.TranspositionMainState)
```

## ScoreModern.RecoveredFragments.InputTranspositionOperation

```text
Method ScoreModern.RecoveredFragments.TranspositionOperationResult Apply(System.Collections.Generic.IList`1[System.Single[]], ScoreModern.RecoveredFragments.TranspositionRequest, ScoreModern.RecoveredFragments.TranspositionWorkspace)
Method ScoreModern.RecoveredFragments.TranspositionOperationSession Begin(System.Collections.Generic.IList`1[System.Single[]], ScoreModern.RecoveredFragments.TranspositionRequest, ScoreModern.RecoveredFragments.TranspositionWorkspace)
Method Void ChangeClef(ScoreModern.RecoveredFragments.TranspositionOperationSession, Int16)
Method ScoreModern.RecoveredFragments.TranspositionOperationResult Finish(ScoreModern.RecoveredFragments.TranspositionOperationSession, Boolean)
Method ScoreModern.RecoveredFragments.TranspositionOperationResult FinishCore(ScoreModern.RecoveredFragments.TranspositionOperationSession, Boolean, Boolean)
```

## ScoreModern.RecoveredFragments.InputTranspositionOrder

```text
Method Boolean MarksModified(Int32, Boolean)
Method Int32[] OrderForMain(System.Collections.Generic.IList`1[System.Single[]], Boolean)
```

## ScoreModern.RecoveredFragments.InputTranspositionPartition

```text
Method ScoreModern.RecoveredFragments.TranspositionPartition Apply(System.Collections.Generic.IList`1[System.Single], System.Collections.Generic.IList`1[System.Single], System.Collections.Generic.IList`1[System.Int16])
```

## ScoreModern.RecoveredFragments.InputTranspositionPitch

```text
Method ScoreModern.RecoveredFragments.TranspositionPitchResult Apply(Single, Single, Int16, Int16, Int16, Int16, Int16, Int16, Int16)
```

## ScoreModern.RecoveredFragments.InputTranspositionRelationshipWindow

```text
Method Void Advance(System.Collections.Generic.IList`1[System.Single[]], System.Collections.Generic.IList`1[System.Int32], Int16 ByRef, Int16 ByRef, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow ByRef, Single, Single)
```

## ScoreModern.RecoveredFragments.InputUniformBeam

```text
Method ScoreModern.RecoveredFragments.InputBeamConstructionResult Build(System.Collections.Generic.IList`1[System.Single[]], Int32[], Int32[], Single, Single, Single, Single, Single, ScoreModern.RecoveredFragments.InputEndpointStaffContext)
```

## ScoreModern.RecoveredFragments.INumericConverterContext

```text
Method Byte ReadByte(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IParameterWordMemory

```text
Method Byte ReadByte(UInt16, UInt16)
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IPointBufferMemory

```text
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IPrefixPreparationMemory

```text
Method Byte ReadByte(UInt16, UInt16)
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteByte(UInt16, UInt16, Byte)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IScreenBiosTransferEffects

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State TransferToBios(ScoreModern.RecoveredFragments.IScreenHookMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.IScreenForwardMemory

```text
Method UInt16 ReadWord(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IScreenHookMemory

```text
Method Void WriteByte(UInt16, UInt16, Byte)
```

## ScoreModern.RecoveredFragments.ISelectorContext

```text
Method UInt16 CallFar(UInt16, UInt16, UInt16[])
Property UInt16 DataSegment
Property Boolean DirectionFlag
Method UInt16 get_DataSegment()
Method Boolean get_DirectionFlag()
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ISpanBookkeepingMemory

```text
Method Byte ReadByte(UInt16, UInt16)
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteByte(UInt16, UInt16, Byte)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IStringBlockMemory

```text
Method Byte ReadByte(UInt16, UInt16)
Method UInt16 ReadWord(UInt16, UInt16)
Method Void WriteByte(UInt16, UInt16, Byte)
Method Void WriteWord(UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.IStringOutputContext

```text
Method ScoreModern.RecoveredFragments.CharacterRoutineResult CallCharacter(ScoreModern.RecoveredFragments.CharacterRoutineWords, UInt16, UInt16)
Method Byte ReadByte(UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ItemDrawingDispatch

```text
Method ScoreModern.RecoveredFragments.ItemDrawingRoute Select(ScoreModern.RecoveredFragments.IDatTextLookupMemory, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ItemDrawingRoute

```text
Field UInt16 TableByteIndex
Field UInt16 Target
Field Boolean UsesTable
```

## ScoreModern.RecoveredFragments.JustificationAllocationResult

```text
Constructor Void .ctor()
Field Single Allocated
Field Int16 NextIndex
Field Single Remaining
```

## ScoreModern.RecoveredFragments.JustificationCoordinates

```text
Constructor Void .ctor(Single[], Int16[], Single, Single, Single)
Field Single LowerBound
Method Void Scale(Single, Single, Single)
Method Void Shift(Single, Single)
```

## ScoreModern.RecoveredFragments.JustificationGapResult

```text
Constructor Void .ctor()
Field Boolean CompressedMinimums
Field Single DeficitPerGap
Field Single MinimumTotal
Field Single NegativeSlack
Field Single PositiveSlack
Field Single Width
```

## ScoreModern.RecoveredFragments.LayoutBasicTablature

```text
Method System.Collections.Generic.List`1[System.Single[]] Build(Single[], System.Collections.Generic.IList`1[System.Single[]], Int32, Boolean, Boolean, Int16[], Int32, Boolean, System.Collections.Generic.IList`1[System.Int16], Boolean)
Method System.Collections.Generic.List`1[System.Single[]] Build(Single[], System.Collections.Generic.IList`1[System.Single[]], Int32, Boolean, Boolean, Int16[], Int32, Boolean, System.Collections.Generic.IList`1[System.Int16], Boolean, Int16[], Int32, System.Func`3[System.Single[],System.Int32,System.Single[]], System.Action`2[System.Single[],System.Int32])
```

## ScoreModern.RecoveredFragments.LayoutCoordinateAffine

```text
Method Void Apply(Single[], Int16[], Single, Single, Single, Int16)
```

## ScoreModern.RecoveredFragments.LayoutCoordinateCollection

```text
Method ScoreModern.RecoveredFragments.LayoutCoordinateCollectionResult Collect(Single[], Int32[], Int16, Int16, Single, Single, Single, Int16, Int16, Int16, Single, Single, Single, Int32, Int16)
```

## ScoreModern.RecoveredFragments.LayoutCoordinateCollectionResult

```text
Constructor Void .ctor()
Field Int16 FirstItem
Field Int16 LastItem
Field Int16[] References
```

## ScoreModern.RecoveredFragments.LayoutDecombineGroups

```text
Method Int16[] Boundaries(Int16, Single[], Int16)
```

## ScoreModern.RecoveredFragments.LayoutDisappearingStaffs

```text
Constructor Void .ctor()
Method Void Apply(Single[])
Method Boolean Hidden(Int32)
Method Int32[] HiddenStaffs()
```

## ScoreModern.RecoveredFragments.LayoutItemSpacing

```text
Constructor Void .ctor(Byte[], Single[], Int32[], Single[], Int16[], Single[], Single[], Single[])
Method Int16 Apply(Int16, Single, Single, Int16 ByRef, Single, Single, Single[], Boolean, System.Nullable`1[System.Single], Boolean)
Method Int16 Apply(Int16, Single, Single, Int16 ByRef, Single, Single, Single[], Boolean, System.Nullable`1[System.Single], Boolean, Boolean)
```

## ScoreModern.RecoveredFragments.LayoutJustificationAllocation

```text
Method ScoreModern.RecoveredFragments.JustificationAllocationResult Apply(Single[], Single[], Int16, Single, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutJustificationGaps

```text
Method ScoreModern.RecoveredFragments.JustificationGapResult Prepare(Single[], Single[], Single[], Int16, Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutJustificationMerge

```text
Method Int16 Append(Single[], Single[], Single[], Single[], Single[], Single[], Int16, Int16, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutJustificationRedistribution

```text
Method Boolean Apply(Single[], Single[], Int16, Single, Single, ScoreModern.RecoveredFragments.JustificationCoordinates)
```

## ScoreModern.RecoveredFragments.LayoutJustificationSnap

```text
Method Void Apply(Single[], Int32[], Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutJustificationSystem

```text
Method ScoreModern.RecoveredFragments.LayoutJustificationSystemResult Apply(Single[], Int32[], Int16, Int16, Single, Single, Single[], Single[], Int16, Int16, ScoreModern.RecoveredFragments.LayoutJustificationWorkspace)
```

## ScoreModern.RecoveredFragments.LayoutJustificationSystemResult

```text
Constructor Void .ctor()
Field Int16[] CoordinateReferences
Field Int16 FirstPoint
Field ScoreModern.RecoveredFragments.JustificationGapResult Gaps
Field Int16 PointCount
Field Boolean RedistributionApplied
Field Int16 SelectionCursor
```

## ScoreModern.RecoveredFragments.LayoutJustificationWorkspace

```text
Constructor Void .ctor()
Field Single[] Auxiliary
Field Single[] MergedMinimum
Field Single[] MergedPositions
Field Single[] Minimum
Field Single[] OldMinimum
Field Single[] OldPositions
Field Boolean OriginalQuarterToneWidths
Field Boolean Page4RoundScaleNumerator
Field System.Nullable`1[System.Single] PositionJoinTolerance
Field Single[] Positions
Method Void ResetForIsolatedTest()
Field Boolean Scor4NumberJustification
Field Single[] Selected
Field Int16[] SelectedReferences
Field Byte[] Spacing
Field Boolean StandaloneJust
```

## ScoreModern.RecoveredFragments.LayoutLineupBeatTotal

```text
Constructor Void .ctor()
Field Int16 Staff
Field Single Total
```

## ScoreModern.RecoveredFragments.LayoutLineupCleanup

```text
Method Void Apply(Single[], Int32[], Int16, Int16, Single[])
```

## ScoreModern.RecoveredFragments.LayoutLineupCoordinates

```text
Method Void Apply(Single[], Single[], Single[], Int16, Single, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutLineupDurationGroups

```text
Method ScoreModern.RecoveredFragments.LayoutLineupDurationGroupsResult Apply(Single[], Single[], Single[], Int16, Int16, Int16)
```

## ScoreModern.RecoveredFragments.LayoutLineupDurationGroupsResult

```text
Constructor Void .ctor()
Field Int16 CurrentIndex
Field Single CurrentSum
Field Single CurrentValue
Field Int16 OutputCount
Field Int16 PriorIndex
Field Single PriorSum
Field Single PriorValue
```

## ScoreModern.RecoveredFragments.LayoutLineupPreparation

```text
Method ScoreModern.RecoveredFragments.LayoutLineupPreparationResult Apply(Single[], Int32[], Int16, Int16, Single[], Single[], Single[], Single[], Single[], Single[])
Method ScoreModern.RecoveredFragments.LayoutLineupPreparationResult ApplyWithBeatPolicy(Single[], Int32[], Int16, Int16, Single[], Single[], Single[], Single[], Single[], Single[], Boolean, System.Action`1[System.Int32], Boolean)
```

## ScoreModern.RecoveredFragments.LayoutLineupPreparationResult

```text
Constructor Void .ctor()
Field ScoreModern.RecoveredFragments.LayoutLineupBeatTotal[] BeatTotals
Field Int16 Bottom
Field Int16 Count
Field Single Left
Field Boolean NeedsCleanup
Field Single Right
Field ScoreModern.RecoveredFragments.LayoutLineupStaffStream[] StaffStreams
Field Int16 Top
Field Boolean UnequalBeats
```

## ScoreModern.RecoveredFragments.LayoutLineupRemap

```text
Method Void Apply(Single[], Int32[], Int16, Int16, Single[], Single[], Int16, Single[], Single[], Int16[], Single[], Int16, Int32)
```

## ScoreModern.RecoveredFragments.LayoutLineupStaffStream

```text
Constructor Void .ctor()
Field Single[] Positions
Field Single[] Rhythms
Field Int16 Staff
```

## ScoreModern.RecoveredFragments.LayoutLineupStaffTables

```text
Method ScoreModern.RecoveredFragments.LayoutLineupStaffTablesResult Apply(Single[], Int16, Single[], Single[], Int16, Single[], Single[])
```

## ScoreModern.RecoveredFragments.LayoutLineupStaffTablesResult

```text
Constructor Void .ctor()
Field Int16 Count
Field Int16 FirstMerged
```

## ScoreModern.RecoveredFragments.LayoutLineupStreamUnion

```text
Method ScoreModern.RecoveredFragments.LayoutLineupStreamUnionResult Apply(Single[], Int16, Int16, Single[], Int16, Single[])
```

## ScoreModern.RecoveredFragments.LayoutLineupStreamUnionResult

```text
Constructor Void .ctor()
Field Int16 Count
Field Int16 CurrentIndex
Field Single CurrentSum
Field Single LastValue
Field Int16 PriorIndex
Field Single PriorSum
```

## ScoreModern.RecoveredFragments.LayoutLineupSystem

```text
Method ScoreModern.RecoveredFragments.LayoutLineupPreparationResult Apply(Single[], Int32[], Int16, Int16, Single[], Single, Int16, Int32, ScoreModern.RecoveredFragments.LayoutLineupWorkspace)
Method ScoreModern.RecoveredFragments.LayoutLineupPreparationResult ApplyWithBeatPolicy(Single[], Int32[], Int16, Int16, Single[], Single, Int16, Int32, ScoreModern.RecoveredFragments.LayoutLineupWorkspace, Boolean, System.Action`1[System.Int32])
```

## ScoreModern.RecoveredFragments.LayoutLineupTableCompact

```text
Method Int16 Apply(Single[], Single[], Int16, Int16, Single)
```

## ScoreModern.RecoveredFragments.LayoutLineupWorkspace

```text
Constructor Void .ctor()
Field Int32 MaximumStaff
Field Single[] Merged
Field Boolean Page4RoundEachPosition
Field Single[] Positions
Field Single[] Rhythms
Field Boolean Scor4DurationScale
Field Single[] Scratch
Field Boolean StandaloneJust
Field Single[] Subdivisions
Field Single[] Union
```

## ScoreModern.RecoveredFragments.LayoutNoteSpacingSpan

```text
Constructor Void .ctor(Byte[], Single[], Int16[], Single[], Boolean, Single)
Method Boolean Apply()
Method Void LoadRecord(Int32, Int32)
```

## ScoreModern.RecoveredFragments.LayoutParentPreparation

```text
Constructor Void .ctor(System.Collections.Generic.IList`1[System.Single[]], Int16, Int16, Single, Single, Boolean, Boolean)
Method Void AnswerDuplicate(Boolean)
Field Int16 Bottom
Field Boolean Complete
Field Int32 DuplicateIndex
Field Single Left
Field Boolean Modified
Field System.Collections.Generic.List`1[System.Int32] Origins
Field Single Right
Field System.Collections.Generic.List`1[System.Single[]] Rows
Field Int16 Top
```

## ScoreModern.RecoveredFragments.LayoutRestSpacing

```text
Method ScoreModern.RecoveredFragments.LayoutRestSpacingResult Apply(Single, Single, Single, Single, Single, Int16, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutRestSpacingResult

```text
Constructor Void .ctor()
Field Single Left
Field Single Multiplier
Field System.Nullable`1[System.Single] Parameter10Scratch
Field Single Right
```

## ScoreModern.RecoveredFragments.LayoutRhythmicCollection

```text
Method ScoreModern.RecoveredFragments.LayoutRhythmicCollectionResult Collect(Single[], Int32[], Single, Int16, Int16, Single[], Single[], Single[], System.Action`1[System.Int32], Boolean)
```

## ScoreModern.RecoveredFragments.LayoutRhythmicCollectionResult

```text
Constructor Void .ctor()
Field Int16 Count
Field Boolean InvalidatedSort
Field Int16 NextItem
Field Single Total
```

## ScoreModern.RecoveredFragments.LayoutRipple

```text
Method ScoreModern.RecoveredFragments.LayoutRippleResult Prepare(Single[], Int32[], Int16, Int16, Int16, Int16)
```

## ScoreModern.RecoveredFragments.LayoutRippleCoordinates

```text
Method Void Scale(Single[], Int16[], Single, Single)
Method Void Shift(Single[], Int16[], Single)
Method Void StaffShift(Single[], Int16[], Single)
```

## ScoreModern.RecoveredFragments.LayoutRippleResult

```text
Constructor Void .ctor()
Field Boolean Applied
Field Int16 Count
Field Int16 Destination
Field Int16 Source
Field Int32[] Starts
```

## ScoreModern.RecoveredFragments.LayoutSeparateParts

```text
Method ScoreModern.RecoveredFragments.LayoutSeparateResult Apply(System.Collections.Generic.IList`1[System.Single[]], Int16, Int16, Single, Single, Boolean)
Method Void Validate(System.Collections.Generic.IList`1[System.Single[]], Int16, Int16, Single, Single)
```

## ScoreModern.RecoveredFragments.LayoutSeparateResult

```text
Constructor Void .ctor()
Field System.Collections.Generic.List`1[System.Int16] ChangedStaffs
Field Int32 Deleted
Field System.Collections.Generic.List`1[System.Int32] Origins
Field System.Collections.Generic.List`1[System.Single[]] Rows
```

## ScoreModern.RecoveredFragments.LayoutSpacingSelection

```text
Method ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult Collect(Single[], Int32[], Int16, Int16, Int16, Single, Single, Single, Single, Single, Single, Single, Single, Int32)
Method ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult Collect(Single[], Int32[], Int16, Int16, Int16, Single, Single, Single, Single, Single, Single, Single, Single, Int32, Boolean)
Method Void Sort(Single[], Int16[], Int16)
```

## ScoreModern.RecoveredFragments.LayoutSpacingSelectionResult

```text
Constructor Void .ctor()
Field Single[] Positions
Field Int16[] RecordWords
Field Int32 StoppedItem
```

## ScoreModern.RecoveredFragments.LayoutSpacingWorkspace

```text
Method Byte[] Create()
```

## ScoreModern.RecoveredFragments.LayoutSymbolSpacing

```text
Method ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Bar(Int16, Int16, Single, Single, Single, Single, Single)
Method ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Key(Int16, Single, Single, Boolean)
Method ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult Meter(Int16, Single, Single[], Int16, Boolean)
```

## ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult

```text
Constructor Void .ctor()
Field Single Left
Field Single Minimum
Field Boolean ResetParameter10Scratch
Field Single Right
Field Single ScaleScratch
Field Single Scratch
```

## ScoreModern.RecoveredFragments.LayoutTablatureAccidentalTable

```text
Field Int32 FirstDelta
Method Int16[] Recover(UInt16)
```

## ScoreModern.RecoveredFragments.LayoutTablatureChord

```text
Method Void Apply(System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.LayoutTablatureChordNote], Int32)
```

## ScoreModern.RecoveredFragments.LayoutTablatureChordNote

```text
Constructor Void .ctor()
Field ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan Plan
Field Single[] Row
Field Int32 SortKey
```

## ScoreModern.RecoveredFragments.LayoutTablatureFrequency

```text
Method System.Collections.Generic.Dictionary`2[System.Int32,System.Int16] Apply(ScoreModern.RecoveredFragments.TranspositionCollectorResult)
```

## ScoreModern.RecoveredFragments.LayoutTablaturePitch

```text
Method ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan Prepare(Int32, Int32, Int32, Int16[])
Method ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan PrepareDrawing(Int32, Int32, Int32, Int16[], Int16)
```

## ScoreModern.RecoveredFragments.LayoutTablaturePitchPlan

```text
Constructor Void .ctor()
Field Int32 Absolute
Method Int32 SelectedString(Int32)
Field Int32 Strings
Field Int32[] Tuning
```

## ScoreModern.RecoveredFragments.LayoutTablatureRelationships

```text
Method Void Apply(System.Collections.Generic.IList`1[System.Single[]], ScoreModern.RecoveredFragments.TranspositionCollectorResult, System.Collections.Generic.Dictionary`2[System.Int32,System.Int16])
```

## ScoreModern.RecoveredFragments.LayoutTextJustification

```text
Method ScoreModern.RecoveredFragments.LayoutTextJustificationResult Apply(Single[], Int32[], Int16, Int16, Single, Single, Int16, Single, Single[], Int16, Int32, ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace)
```

## ScoreModern.RecoveredFragments.LayoutTextJustificationResult

```text
Constructor Void .ctor()
Field ScoreModern.RecoveredFragments.JustificationGapResult Gaps
Field Int16 PointCount
```

## ScoreModern.RecoveredFragments.LayoutTextJustificationWorkspace

```text
Constructor Void .ctor()
Field Single[] Minimum
Field Single[] Positions
Field Single[] Slack
```

## ScoreModern.RecoveredFragments.LayoutVerticalCollision

```text
Method Int32 Minimum(Int16[], Int16[], Int16)
```

## ScoreModern.RecoveredFragments.LayoutVerticalContour

```text
Method Void Apply(Int16[], Int16[], Int32, Int32, Boolean, Boolean, Int16[], Int16[], Int16, Int16)
```

## ScoreModern.RecoveredFragments.LayoutVerticalPlacement

```text
Method ScoreModern.RecoveredFragments.LayoutVerticalPlacementResult Apply(Int16[][], Int16[][], Boolean[], Single[], Single[], Int16, Int16, Int16, Int16, Single, Single[])
```

## ScoreModern.RecoveredFragments.LayoutVerticalPlacementResult

```text
Constructor Void .ctor()
Field Int16 Bottom
Field Single[] Offsets
Field Boolean Repeat
Field Int16 Top
```

## ScoreModern.RecoveredFragments.LineClipping

```text
Method ScoreModern.RecoveredFragments.LineClippingWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.LineClippingFlagResult ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.LineClippingDivideFault

```text
Constructor Void .ctor()
Field UInt32 EFlags
Field Boolean HasEFlags
Field ScoreModern.RecoveredFragments.LineClippingWords Words
```

## ScoreModern.RecoveredFragments.LineClippingFlagResult

```text
Field UInt32 EFlags
Field ScoreModern.RecoveredFragments.LineClippingWords Words
```

## ScoreModern.RecoveredFragments.LineClippingWords

```text
Field UInt16 AX
Field UInt16 BX
Field UInt16 CX
Field UInt16 DI
Field UInt16 DX
Field UInt16 Exit
Field UInt16 SI
```

## ScoreModern.RecoveredFragments.LineDriverBody

```text
Method ScoreModern.RecoveredFragments.LineDriverBodyResult Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteThroughReturn(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteThroughReturnWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
Method ScoreModern.RecoveredFragments.LineDriverBodyResult ExecuteWithFlags(ScoreModern.RecoveredFragments.IByteOrResultMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.LineDriverBodyResult

```text
Field UInt16 ClippingExit
Field UInt32 DefinedEFlagsMask
Field ScoreModern.RecoveredFragments.SignedDivide16Fault Fault
Field UInt16 RasterEntry
Field Boolean RetfExecuted
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.LinePreparation

```text
Method ScoreModern.RecoveredFragments.LinePreparationWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.LinePreparationWords

```text
Field UInt16 AX
Field UInt16 BX
Field UInt16 CX
Field UInt16 DI
Field UInt16 DX
Field UInt16 ES
```

## ScoreModern.RecoveredFragments.Mode12BiosTransfer

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.Mode12VideoServices, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.SignedDivide16State TransferToBios(ScoreModern.RecoveredFragments.IScreenHookMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.Mode12CellSurface

```text
Constructor Void .ctor()
Method Byte Background(Int32, Int32)
Method ScoreModern.RecoveredFragments.Mode12Glyph[] Cell(Int32, Int32)
Method Void Clear()
Method Int64 get_Revision()
Property Int64 Revision
Method Void Scroll(Int32, Int32, Int32, Int32, Int32, Byte)
Method Void WriteGlyph(Int32, Int32, Byte, Byte)
```

## ScoreModern.RecoveredFragments.Mode12Glyph

```text
Constructor Void .ctor(Byte, Byte)
Field Byte Character
Field Byte Color
```

## ScoreModern.RecoveredFragments.Mode12Plane2Surface

```text
Constructor Void .ctor(Byte[], ScoreModern.RecoveredFragments.IMode12GlyphSource)
Field Byte[] Bytes
Method Void Scroll(Int32, Int32, Int32, Int32, Int32, Byte)
Method Void WriteGlyph(Int32, Int32, Byte, Byte)
```

## ScoreModern.RecoveredFragments.Mode12VideoServices

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.IMode12Surface)
Method ScoreModern.RecoveredFragments.SignedDivide16State ExecuteService(ScoreModern.RecoveredFragments.IScreenHookMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.NumericConverter

```text
Method ScoreModern.RecoveredFragments.NumericConverterResult Execute(ScoreModern.RecoveredFragments.INumericConverterContext, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.NumericConverterResult

```text
Constructor Void .ctor(UInt16, UInt16, UInt16)
Field UInt16 AX
Field UInt16 DX
Field UInt16 Status
```

## ScoreModern.RecoveredFragments.OutputCountAdvance

```text
Method ScoreModern.RecoveredFragments.OutputCountResult Execute(ScoreModern.RecoveredFragments.ISpanBookkeepingMemory, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.OutputCountResult

```text
Constructor Void .ctor(UInt16, UInt16)
Field UInt16 AX
Field UInt16 DX
```

## ScoreModern.RecoveredFragments.PackedDrawingParts

```text
Field Int16 FinalAX
Field Int16 High
Field Int16 Low
Field Int16 Middle
```

## ScoreModern.RecoveredFragments.PackedDrawingWord

```text
Method ScoreModern.RecoveredFragments.PackedDrawingParts Decode(Int16)
```

## ScoreModern.RecoveredFragments.PageCoordinates

```text
Constructor Void .ctor()
Field Single[] Bases
Method ScoreModern.RecoveredFragments.PageItemContext Prepare(Single[], Int32, Int32)
Field Single[] Scales
Method ScoreModern.RecoveredFragments.PageStaffResult SetStaff(Single[], Boolean)
Method ScoreModern.RecoveredFragments.PageStaffResult SetStaff(Single[], Int32, Int16, Boolean)
Method Single Vertical(Int32, Single)
```

## ScoreModern.RecoveredFragments.PageItemContext

```text
Field Int16 HorizontalWord
Field Single Scale
Field Boolean ScreenHorizontalLimit
Field Single VerticalInput
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.PageStaffLine

```text
Field Single Left
Field Single Right
Field Single Y
```

## ScoreModern.RecoveredFragments.PageStaffResult

```text
Constructor Void .ctor()
Field Single Base
Field Single Bottom
Field Int32 LineCount
Field System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.PageStaffLine] Lines
Field Single Scale
Field Single Spacing
Field Int16 WidthSelector
```

## ScoreModern.RecoveredFragments.ParameterWordReader

```text
Method UInt16 Execute(ScoreModern.RecoveredFragments.IParameterWordMemory, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.PointBufferAppend

```text
Method ScoreModern.RecoveredFragments.PointBufferWords Execute(ScoreModern.RecoveredFragments.IPointBufferMemory, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.PointBufferWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IPointBufferMemory, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.PointBufferWords

```text
Field UInt16 AX
Field UInt16 BX
Field UInt32 DefinedEFlagsMask
Field UInt32 EFlags
Field UInt16 ES
Field UInt16 SI
```

## ScoreModern.RecoveredFragments.PrefixPreparationResult

```text
Constructor Void .ctor(UInt16, UInt16, UInt16, UInt16, UInt16)
Field UInt16 AX
Field UInt16 BP
Field UInt16 DI
Field UInt16 RelativeTargetSegment
Field UInt16 SI
Field UInt16 SP
Field UInt16 TargetOffset
```

## ScoreModern.RecoveredFragments.PrefixWritePreparation

```text
Method ScoreModern.RecoveredFragments.PrefixPreparationResult Prepare(ScoreModern.RecoveredFragments.IPrefixPreparationMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.PrimitiveTextGeometry

```text
Method ScoreModern.RecoveredFragments.PrimitiveTextPoint Point(Int16, ScoreModern.RecoveredFragments.PrimitiveTextScale)
Method ScoreModern.RecoveredFragments.PrimitiveTextScale Prepare(Single, Single, Single, Single, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.PrimitiveTextPoint

```text
Field Boolean CommandOnly
Field Int32 MarkerWordBack
Field Int16 Operation
Field System.Nullable`1[System.Int16] RetainedMarkerY
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.PrimitiveTextScale

```text
Field Single OriginX
Field Single OriginY
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.PrintedGlyphDefinition

```text
Method System.String Fixed3(Single)
Method ScoreModern.RecoveredFragments.PrintedGlyphVertex Vertex(Int16, Int16, Int16)
```

## ScoreModern.RecoveredFragments.PrintedGlyphVertex

```text
Field Int16 Operation
Field Int32 X
Field Int32 Y
```

## ScoreModern.RecoveredFragments.PrintedHeadRequest

```text
Field Int32 Glyph
Field System.String Scale
Field System.String Width
Field Int16 X
Field Int16 Y
```

## ScoreModern.RecoveredFragments.PrintRotation

```text
Method Single Apply(Single, Boolean, Boolean)
Method Int32 Offset(Int32, Boolean, Boolean)
```

## ScoreModern.RecoveredFragments.RasterDispatch

```text
Method ScoreModern.RecoveredFragments.RasterDispatchWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.RasterDispatchWords

```text
Field UInt16 BX
Field UInt16 CX
Field UInt16 DX
Field UInt32 EFlags
Field UInt16 Exit
```

## ScoreModern.RecoveredFragments.RasterStatusFlags

```text
Method UInt32 Add16(UInt32, UInt16, UInt16)
Method UInt32 Dec16(UInt32, UInt16)
Method UInt32 Inc16(UInt32, UInt16)
Method UInt32 Logical8(UInt32, Byte)
Method UInt32 Sub16(UInt32, UInt16, UInt16)
Method UInt32 Sub8(UInt32, Byte, Byte)
```

## ScoreModern.RecoveredFragments.RealModeIret16

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.RetainedParameterStorage

```text
Constructor Void .ctor()
Field Int32 Capacity
Method ScoreModern.RecoveredFragments.RetainedParameterStorage Clone()
Method Void CommitReplay(Byte[])
Method Void CopyBytes(Int32, Int32, Int32)
Method Void Forget()
Method Void Load(Byte[], Int32, Byte[], Byte[])
Method Boolean Matches(Byte[])
Method Void Opaque(Byte[])
Method Void Pack(Byte[])
Method Byte[] ReadKnown(Int32, Int32)
Method Void Saved(Byte[])
Method Void TextInputPadding(Int32)
Method Byte[] UndefinedWord()
Method Void WriteKnown(Int32, Byte[])
```

## ScoreModern.RecoveredFragments.ScoreVersionControls

```text
Method Single DrawingOffset(Boolean, Single)
Method Single HarpOffset(Boolean, Single)
Method Single Horizontal(Single)
Method Int16 MeterThicknessSelector(Boolean, Single)
Method Boolean MeterThicknessSupported(Boolean, Single)
Method ScoreModern.RecoveredFragments.LayoutSymbolSpacingResult NumberSpacing(Single, Single, Single)
Method Boolean SlurEndpointMode(Boolean, Single)
Method Boolean SlurEndpointOverride(Boolean, Single[])
Method Single[] SlurParameters(Boolean, Single[])
Method Int16 SlurSubtype(Boolean, Single, Single)
```

## ScoreModern.RecoveredFragments.ScorlasFilePlacement

```text
Field Int32 BottomShift
Field Int32 Cumulative
Field Int32 Delta
Field Int32 DscBottom
Field Int32 DscTop
Field Int32 NextAdvance
Field Int32 NormalizedHeight
Field Int32 OverallHeight
Field Int32 RawHeight
```

## ScoreModern.RecoveredFragments.ScorlasFilePlacementState

```text
Constructor Void .ctor()
Method ScoreModern.RecoveredFragments.ScorlasFilePlacement Add(Int16, Int16, Int32)
```

## ScoreModern.RecoveredFragments.ScorlasGuideConversion

```text
Method Int32 Convert(Single, Boolean)
```

## ScoreModern.RecoveredFragments.ScorlasStatisticsArithmetic

```text
Method Single Height(Int32, Single, Boolean)
```

## ScoreModern.RecoveredFragments.ScreenForwardResult

```text
Constructor Void .ctor(UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
Field UInt16 BX
Field UInt16 CS
Field UInt16 CX
Field UInt16 DS
Field UInt16 DX
Field UInt16 IP
Field UInt16 SP
```

## ScoreModern.RecoveredFragments.ScreenHookDecisions

```text
Method UInt16 DecrementSavedRow(ScoreModern.RecoveredFragments.IStringBlockMemory, UInt16)
Method Boolean ShouldProbeCursor(ScoreModern.RecoveredFragments.IStringBlockMemory, UInt16, UInt16)
Method Boolean ShouldScroll(ScoreModern.RecoveredFragments.IStringBlockMemory, UInt16, Byte, UInt16)
```

## ScoreModern.RecoveredFragments.ScreenHookForwarding

```text
Method ScoreModern.RecoveredFragments.ScreenForwardResult RestoreAndResolve(ScoreModern.RecoveredFragments.IScreenForwardMemory, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.ScreenHookInterrupt

```text
Method ScoreModern.RecoveredFragments.SignedDivide16State Enter(ScoreModern.RecoveredFragments.IScreenHookMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ScreenHookInterruptEffects

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.IScreenBiosTransferEffects, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.SignedDivide16State Interrupt(Byte, ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.ScreenHookOutcome

```text
Field ScoreModern.RecoveredFragments.ScreenHookOutcome ExternalControlTransfer
Field ScoreModern.RecoveredFragments.ScreenHookOutcome Forwarded
Field Int32 value__
```

## ScoreModern.RecoveredFragments.ScreenHookResult

```text
Field ScoreModern.RecoveredFragments.ScreenHookOutcome Outcome
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.ScreenHookRoutine

```text
Method ScoreModern.RecoveredFragments.ScreenHookResult Execute(ScoreModern.RecoveredFragments.IScreenHookMemory, ScoreModern.RecoveredFragments.ICapacityInterruptEffects, ScoreModern.RecoveredFragments.SignedDivide16State, UInt16)
```

## ScoreModern.RecoveredFragments.SelectorCopy

```text
Method Boolean Apply(Byte[], Int16, Boolean)
```

## ScoreModern.RecoveredFragments.SelectorRoutine

```text
Method Void Execute(ScoreModern.RecoveredFragments.ISelectorContext, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.SignedDivide16

```text
Method ScoreModern.RecoveredFragments.SignedDivide16Result Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, ScoreModern.RecoveredFragments.SignedDivide16State)
```

## ScoreModern.RecoveredFragments.SignedDivide16Fault

```text
Field ScoreModern.RecoveredFragments.SignedDivide16Fault None
Field ScoreModern.RecoveredFragments.SignedDivide16Fault QuotientOverflow
Field Int32 value__
Field ScoreModern.RecoveredFragments.SignedDivide16Fault ZeroDivisor
```

## ScoreModern.RecoveredFragments.SignedDivide16Result

```text
Field UInt32 DefinedEFlagsMask
Field ScoreModern.RecoveredFragments.SignedDivide16Fault Fault
Field ScoreModern.RecoveredFragments.SignedDivide16State State
```

## ScoreModern.RecoveredFragments.SignedDivide16State

```text
Field UInt16 AX
Field UInt16 BP
Field UInt16 BX
Field UInt16 CS
Field UInt16 CX
Field UInt16 DI
Field UInt16 DS
Field UInt16 DX
Field UInt32 EFlags
Field UInt16 ES
Field UInt16 IP
Field UInt16 SI
Field UInt16 SP
Field UInt16 SS
```

## ScoreModern.RecoveredFragments.SingleFixedDecimal

```text
Method System.String Format(Single, Int32)
```

## ScoreModern.RecoveredFragments.SlopedRaster

```text
Method ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.SlurGeometry

```text
Method ScoreModern.RecoveredFragments.SlurGeometryResult Build(Single[], ScoreModern.RecoveredFragments.SlurOriginContext, Boolean, Single, ScoreModern.RecoveredFragments.SlurVectorBuffer, Boolean)
```

## ScoreModern.RecoveredFragments.SlurGeometryResult

```text
Constructor Void .ctor()
Field ScoreModern.RecoveredFragments.SlurVectorBuffer Buffer
Field System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
Field Int32 Root
```

## ScoreModern.RecoveredFragments.SlurLineWidth

```text
Property Single DefaultUnit
Method Single Field(Int16, Int16, Single)
Method System.String Format(Single)
Method Single get_DefaultUnit()
Method Single Printed(Int16)
Method Single RawField(Int16, Int16, Single)
```

## ScoreModern.RecoveredFragments.SlurOriginContext

```text
Constructor Void .ctor()
Field Single LeftX
Field Single LeftY
Field Single RightY
Field Single Scale
Field Boolean Scor4
```

## ScoreModern.RecoveredFragments.SlurOrigins

```text
Method ScoreModern.RecoveredFragments.SlurOriginState Distance(Single[], ScoreModern.RecoveredFragments.SlurOriginContext)
Method ScoreModern.RecoveredFragments.SlurOriginState Prepare(Single[], ScoreModern.RecoveredFragments.SlurOriginContext)
```

## ScoreModern.RecoveredFragments.SlurOriginState

```text
Constructor Void .ctor()
Field Single CurveCenter
Field Single DeltaY
Field Single EffectiveStartFraction
Field Single Height
Field Single HorizontalSpan
Field Single LeftX
Field Single LeftY
Field Single Length
Field Int16 NumberWord
Field Int16 OffsetWord
Field Single OriginalLeft
Field Single OriginalRight
Field Single RightX
Field Single StartFraction
Field Single StopFraction
Field Single WorkingRight
```

## ScoreModern.RecoveredFragments.SlurPlotRequest

```text
Constructor Void .ctor(Int16, Int16, Int16)
Field Int16 Kind
Field Int16 X
Field Int16 Y
```

## ScoreModern.RecoveredFragments.SlurPostprocess

```text
Method ScoreModern.RecoveredFragments.SlurVectorBuffer From(ScoreModern.RecoveredFragments.SlurSampleResult, ScoreModern.RecoveredFragments.SlurThicknessResult, Boolean)
Method Void Pattern(ScoreModern.RecoveredFragments.SlurVectorBuffer, Single[], Single, Boolean)
Method Void Rotate(ScoreModern.RecoveredFragments.SlurVectorBuffer, ScoreModern.RecoveredFragments.SlurOriginState)
Method Void Trim(ScoreModern.RecoveredFragments.SlurVectorBuffer, Single[], Single, Single, Boolean)
```

## ScoreModern.RecoveredFragments.SlurSampleResult

```text
Constructor Void .ctor()
Field Int32 Count
Field Single Exponent
Field Single FinalIncrement
Field Single FinalX
Field Single HalfSteps
Field Single Height
Field ScoreModern.RecoveredFragments.SlurOriginState Origin
Field Single Step
Field Int16[] X
Field Int16[] Y
```

## ScoreModern.RecoveredFragments.SlurSampling

```text
Method ScoreModern.RecoveredFragments.SlurSampleResult Build(Single[], ScoreModern.RecoveredFragments.SlurOriginContext, Boolean)
Method ScoreModern.RecoveredFragments.SlurSampleResult BuildState(Single[], ScoreModern.RecoveredFragments.SlurOriginContext, ScoreModern.RecoveredFragments.SlurOriginState, Boolean, Boolean)
```

## ScoreModern.RecoveredFragments.SlurThickness

```text
Method ScoreModern.RecoveredFragments.SlurThicknessResult Build(Single[], ScoreModern.RecoveredFragments.SlurOriginContext, ScoreModern.RecoveredFragments.SlurSampleResult, Single)
Method Void Reflect(Single[], ScoreModern.RecoveredFragments.SlurSampleResult, Int16[], Boolean)
```

## ScoreModern.RecoveredFragments.SlurThicknessResult

```text
Constructor Void .ctor()
Field Single EffectiveControl
Field Int32 FinalIncrement
Field Single FinalRamp
Field Single RampStep
Field Int32 Target
Field Single Thickness
Field Int16[] X
Field Int16[] Y
```

## ScoreModern.RecoveredFragments.SlurVectorBuffer

```text
Constructor Void .ctor()
Constructor Void .ctor(UInt16, UInt16)
Field Single Angle
Field Int32 Base
Field Int32 Capacity
Method Void Copy(Int32, Int32)
Field Single Cos
Field Int32 CurveEnd
Field Int32 End
Method Int16[] ExtractX()
Method Int16[] ExtractY()
Method Void Move(Int32, Int32, Int32)
Field Int32 SampleCount
Field Single Sin
Field ScoreModern.RecoveredFragments.SlurWordSegment X
Field ScoreModern.RecoveredFragments.SlurWordSegment Y
```

## ScoreModern.RecoveredFragments.SlurVectorCommands

```text
Method System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.SlurPlotRequest] Decode(ScoreModern.RecoveredFragments.SlurVectorBuffer, Int32, Boolean, Boolean)
```

## ScoreModern.RecoveredFragments.SlurWordSegment

```text
Constructor Void .ctor(Byte[], Int32)
Method Int16 get_Item(Int32)
Method Int32 get_Length()
Property Int16 Item [Int32]
Property Int32 Length
Method Void set_Item(Int32, Int16)
```

## ScoreModern.RecoveredFragments.SpanBookkeeping

```text
Method UInt16 Execute(ScoreModern.RecoveredFragments.ISpanBookkeepingMemory, UInt16, UInt16, UInt16, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.StaffHeightLayout

```text
Method Single[] Targets(Single[], Single[], Int32, Int32[], Single, Single, Boolean, Single[])
```

## ScoreModern.RecoveredFragments.StringCopyResult

```text
Constructor Void .ctor(UInt16, UInt16)
Field UInt16 CX
Field UInt16 DI
Field UInt16 SI
```

## ScoreModern.RecoveredFragments.StringFillResult

```text
Constructor Void .ctor(UInt16)
Field UInt16 CX
Field UInt16 DI
```

## ScoreModern.RecoveredFragments.StringOutputResult

```text
Field Boolean ReachedStringReturn
Field ScoreModern.RecoveredFragments.CharacterRoutineResult Result
```

## ScoreModern.RecoveredFragments.StringOutputRoutine

```text
Method ScoreModern.RecoveredFragments.StringOutputResult Run(ScoreModern.RecoveredFragments.IStringOutputContext, ScoreModern.RecoveredFragments.CharacterRoutineWords, UInt16, UInt16)
```

## ScoreModern.RecoveredFragments.TextConverterFontState

```text
Method Void Accept(Int16)
Method ScoreModern.RecoveredFragments.TextConverterFontState Begin(Int16, Int16)
Field Boolean Forced
Method Int32 get_MetricFace()
Property Int32 MetricFace
Field Int16 Word
```

## ScoreModern.RecoveredFragments.TextMetricArithmetic

```text
Method Single Accumulate(Single, Single)
Method Single AddPosition(Int16, Single, Single, Single)
Method Single Advance(Single, ScoreModern.RecoveredFragments.TextMetricScale, Boolean)
Method Single Finish(Single, Single)
Method Single Normalize(Int16, Single)
Method ScoreModern.RecoveredFragments.TextMetricScale Prepare(Single, Single, Single, Single, Single)
Method Single StrokeHorizontal(Single, Single, Single)
```

## ScoreModern.RecoveredFragments.TextMetricScale

```text
Field Single Character
Field Single ExtraSpace
```

## ScoreModern.RecoveredFragments.TextParameterPreparation

```text
Method Single[] Adjust(Single[], Single)
Method Single Interletter(Single, Single, Single[])
Method Single OutputExtra(Single, Boolean, Single)
Method ScoreModern.RecoveredFragments.TextPreparedParameters Prepare(Single[], Single, Single, Single, Single)
```

## ScoreModern.RecoveredFragments.TextPreparedParameters

```text
Field Single GlyphHorizontal
Field Single GlyphVertical
Field Single Horizontal
Field Single Origin
Field Single RestoredHorizontal
Field Single RestoredVertical
Field Single Rotation
Field Single Spacing
Field Single Vertical
```

## ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult

```text
Constructor Void .ctor()
Field Int16 ActiveRelationships
Field Int16[] Flags
Field Int16 RelationshipIndex
Field ScoreModern.RecoveredFragments.TranspositionRelationshipWindow Window
```

## ScoreModern.RecoveredFragments.TranspositionClefState

```text
Constructor Void .ctor(Int16, Int16)
Field Int16 RetainedStaff
Field Int16 RetainedTarget
```

## ScoreModern.RecoveredFragments.TranspositionCollection

```text
Constructor Void .ctor()
Field Int16 FirstKeyState
Field System.Nullable`1[System.Single] FirstNotePosition
Field Boolean HasExtendedBeam
Field System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TranspositionRecord] Records
Field System.Collections.Generic.List`1[System.Int32] RelationshipItems
```

## ScoreModern.RecoveredFragments.TranspositionCollectorResult

```text
Constructor Void .ctor()
Field System.Collections.Generic.List`1[System.Single] CollectedBoundaries
Field Single ExpandedLeft
Field Single ExpandedRight
Field ScoreModern.RecoveredFragments.TranspositionCollection FirstPass
Field ScoreModern.RecoveredFragments.TranspositionPartition Partition
```

## ScoreModern.RecoveredFragments.TranspositionCollectorState

```text
Constructor Void .ctor(Single)
Field Single FirstNotePosition
```

## ScoreModern.RecoveredFragments.TranspositionInterval

```text
Constructor Void .ctor()
Field Int16 Diatonic
Field Int16 Key
Field Int16 SemitoneClass
Field Int16 Semitones
```

## ScoreModern.RecoveredFragments.TranspositionKeyRecordResult

```text
Constructor Void .ctor()
Field Int16 Diatonic
Field Int16 KeyOverride
Field Int16 NewKeyBase
Field Int16 OldKeyBase
Field Single[] Parameters
Field Int16 PitchBase
```

## ScoreModern.RecoveredFragments.TranspositionMainPassResult

```text
Constructor Void .ctor()
Field Single StopPosition
Field ScoreModern.RecoveredFragments.TranspositionAccidentalTailResult Tail
```

## ScoreModern.RecoveredFragments.TranspositionMainState

```text
Constructor Void .ctor()
Field Int16[] AccidentalMarkers
Field Int16 Diatonic
Field Single ErasedCode
Field ScoreModern.RecoveredFragments.TranspositionRelationshipWindow InitialWindow
Field Int16 KeyOverride
Field Int16 KindZeroGuard
Field Single RelationshipMargin
Field Single RelationshipWidth
Field Int16 Semitones
Field Single Sentinel
Field Int16 TargetKey
```

## ScoreModern.RecoveredFragments.TranspositionOperationResult

```text
Constructor Void .ctor()
Field ScoreModern.RecoveredFragments.TranspositionCollectorResult Collection
Field ScoreModern.RecoveredFragments.TranspositionMainPassResult Engine
Field Boolean InsertedKey
Field ScoreModern.RecoveredFragments.TranspositionInterval Interval
Field Boolean KeyQuestion
Field System.Collections.Generic.List`1[System.Single[]] Rows
Field Boolean SortMarksModified
Field Boolean WarningRequired
```

## ScoreModern.RecoveredFragments.TranspositionOperationSession

```text
Constructor Void .ctor()
Field ScoreModern.RecoveredFragments.TranspositionOperationResult Result
```

## ScoreModern.RecoveredFragments.TranspositionPartition

```text
Constructor Void .ctor()
Field Single[] Boundaries
Field Int16[] Kinds
```

## ScoreModern.RecoveredFragments.TranspositionPitchResult

```text
Constructor Void .ctor()
Field System.Nullable`1[System.Int16] AccidentalMarker
Field Single Pitch
Field Single Stem
```

## ScoreModern.RecoveredFragments.TranspositionRecord

```text
Constructor Void .ctor()
Field Int16 Accidental
Field Int16 Clef
Field Int32 Item
Field Int16 Kind
Field System.Nullable`1[System.Int16] Pitch
Field Single Position
```

## ScoreModern.RecoveredFragments.TranspositionRelationshipWindow

```text
Field Single EndHigh
Field Single EndLow
Field Single StartHigh
Field Single StartLow
```

## ScoreModern.RecoveredFragments.TranspositionRequest

```text
Constructor Void .ctor()
Field Boolean AddKey
Field System.Nullable`1[System.Int16] Clef
Field Int16[] ClefChanges
Field Boolean ForceKeyQuestion
Field Single Left
Field Single Right
Field Int16 Semitones
Field Single Staff
```

## ScoreModern.RecoveredFragments.TranspositionWorkspace

```text
Constructor Void .ctor(ScoreModern.RecoveredFragments.TranspositionCollectorState, ScoreModern.RecoveredFragments.TranspositionClefState, Int16[], Int16[], Int16, ScoreModern.RecoveredFragments.TranspositionRelationshipWindow, Boolean)
Field Int16[] AccidentalMarkers
Field Boolean AlreadySorted
Field ScoreModern.RecoveredFragments.TranspositionClefState Clefs
Field ScoreModern.RecoveredFragments.TranspositionCollectorState Collector
Field Int16 KindZeroGuard
Field Int16[] PitchWords
Field ScoreModern.RecoveredFragments.TranspositionRelationshipWindow Window
```

## ScoreModern.RecoveredFragments.TupletBracket

```text
Method ScoreModern.RecoveredFragments.TupletBracketResult Build(Single[], ScoreModern.RecoveredFragments.TupletBracketContext)
```

## ScoreModern.RecoveredFragments.TupletBracketContext

```text
Constructor Void .ctor()
Field Single LeftX
Field Single LeftY
Field Single RightY
Field Single Scale
```

## ScoreModern.RecoveredFragments.TupletBracketPoint

```text
Constructor Void .ctor(Int16, Single, Single)
Field Int16 Tag
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.TupletBracketResult

```text
Constructor Void .ctor()
Field System.Nullable`1[ScoreModern.RecoveredFragments.TupletPlacement] Number
Field System.Collections.Generic.List`1[ScoreModern.RecoveredFragments.TupletBracketPoint] Points
```

## ScoreModern.RecoveredFragments.TupletGeometry

```text
Method ScoreModern.RecoveredFragments.TupletPlacement Build(Int16, Int32, Boolean, Single, Single, Single, Single, Single, Single, Single)
Method Single Center(Int16, Single, Single, Single)
Method System.String NumberText(Int16)
Method Single PrintedEm(Single, Single)
```

## ScoreModern.RecoveredFragments.TupletPlacement

```text
Field Single FontScale
Field Int16 Number
Field Single X
Field Single Y
```

## ScoreModern.RecoveredFragments.VerticalRaster

```text
Method ScoreModern.RecoveredFragments.VerticalRasterWords Execute(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16)
Method ScoreModern.RecoveredFragments.VerticalRasterWords ExecuteWithFlags(ScoreModern.RecoveredFragments.IHorizontalRasterMemory, UInt16, UInt16, UInt16, UInt16, UInt16, UInt16, UInt32)
```

## ScoreModern.RecoveredFragments.VerticalRasterWords

```text
Field UInt16 AX
Field UInt16 BP
Field UInt16 BX
Field UInt16 CX
Field UInt32 DefinedEFlagsMask
Field UInt16 DI
Field UInt16 DX
Field UInt32 EFlags
Field UInt16 SI
```
