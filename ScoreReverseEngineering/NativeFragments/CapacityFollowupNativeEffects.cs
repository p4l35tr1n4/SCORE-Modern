using System;
namespace ScoreModern.RecoveredFragments {
 // Connects the verified instruction bodies. BIOS/device services and character
 // rendering remain injected dependencies; no Windows mapping is supplied.
 public sealed class CapacityFollowupNativeEffects:ICapacityFollowupEffects {
  readonly ICapacityInterruptEffects interrupts;readonly ICapacityCharacterEffects characters;readonly ushort characterSegment;
  public CapacityFollowupNativeEffects(ICapacityInterruptEffects interruptEffects,ICapacityCharacterEffects characterEffects,ushort characterTargetSegment){if(interruptEffects==null||characterEffects==null)throw new ArgumentNullException();interrupts=interruptEffects;characters=characterEffects;characterSegment=characterTargetSegment;}
  public SignedDivide16State Call(CapacityFollowupCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state){
   switch(kind){
    case CapacityFollowupCallKind.KeyStatus:return CapacityDeviceCallees.KeyStatusThroughReturn(memory,interrupts,state).State;
    case CapacityFollowupCallKind.Poll:return CapacityDeviceCallees.PollThroughReturn(memory,interrupts,state).State;
    case CapacityFollowupCallKind.PromptOutput:
     var text=memory as ICapacityStringMemory;if(text==null)throw new ArgumentException("String byte reads required.");return CapacityStringOutput.ExecuteThroughReturn(text,characters,state,characterSegment).State;
    case CapacityFollowupCallKind.KeyRead:
     var key=memory as ICapacityKeyMemory;if(key==null)throw new ArgumentException("Key output byte writes required.");return CapacityKeyReader.ExecuteThroughReturn(key,interrupts,state).State;
    default:throw new ArgumentOutOfRangeException("kind");
   }
  }
 }
}
