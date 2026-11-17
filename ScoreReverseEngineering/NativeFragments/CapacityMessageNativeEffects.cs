using System;
namespace ScoreModern.RecoveredFragments {
 // Connects the enclosing message handler to verified native instruction bodies.
 public sealed class CapacityMessageNativeEffects:ICapacityCallEffects {
  readonly CapacityFollowupNativeEffects followup;readonly ICapacityCharacterEffects characters;readonly ushort calleeSegment,characterSegment;
  public CapacityMessageNativeEffects(ICapacityInterruptEffects interruptEffects,ICapacityCharacterEffects characterEffects,ushort sharedCalleeSegment,ushort characterTargetSegment){if(interruptEffects==null||characterEffects==null)throw new ArgumentNullException();characters=characterEffects;calleeSegment=sharedCalleeSegment;characterSegment=characterTargetSegment;followup=new CapacityFollowupNativeEffects(interruptEffects,characterEffects,characterTargetSegment);}
  public static CapacityMessageNativeEffects CreateWithNativeCharacters(ICapacityInterruptEffects interruptEffects,ushort sharedCalleeSegment,ushort characterTargetSegment){return new CapacityMessageNativeEffects(interruptEffects,new CapacityCharacterNativeEffects(interruptEffects),sharedCalleeSegment,characterTargetSegment);}
  public SignedDivide16State Call(CapacityCallKind kind,IHorizontalRasterMemory memory,SignedDivide16State state){
   switch(kind){
    case CapacityCallKind.StringOutput:var text=memory as ICapacityStringMemory;if(text==null)throw new ArgumentException("String byte reads required.");return CapacityStringOutput.ExecuteThroughReturn(text,characters,state,characterSegment).State;
    case CapacityCallKind.Followup:return CapacityFollowup.ExecuteThroughReturn(memory,followup,state,calleeSegment).State;
    default:throw new ArgumentOutOfRangeException("kind");
   }
  }
 }
}
