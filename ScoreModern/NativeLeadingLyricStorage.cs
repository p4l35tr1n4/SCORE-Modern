using System;
namespace ScoreModern {public sealed partial class Editor {
 // Measured 3054:0371 resets the preceding lyric word index to0. For TC,
 // 046A..047E clears RN[4*index+40] before the first ?? syllable is placed.
 // Replay the byte address through a known packed RN layout, never an item number.
 void ApplyOriginalLeadingExtenderStore(OriginalTextEntry entry){
  if(!NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Starting centred lyrics with ?? can change an earlier item's parameter in SCORE. This is prevented by default; original invalid-state compatibility can reproduce established histories.");
  var active=ParameterBytes(score);const int offset=40;
  if(entry.Item.P(3)!=0||!score.RawStorageLayoutVerified||parameterStorageDirty||!parameterStorage.Matches(active)||parameterStorage.ReadKnown(offset,4)==null)throw new NotSupportedException("The original leading-extender write needs an established packed RN layout and lyric input position.");
  int start=0;Item target=null;int parameter=0;
  foreach(var item in score.Items){int end=start+item.Bytes.Length;if(offset>=start&&offset+4<=end){parameter=(offset-start)/4;if(parameter<1||parameter>item.Count)throw new NotSupportedException("The original leading-extender write reaches a record header; that invalid layout is prevented.");target=item;break;}start=end;}
  if(target==null)throw new NotSupportedException("The original leading-extender write is outside the established records.");
  if(BitConverter.ToInt32(BitConverter.GetBytes(target.P(parameter)),0)==0)return;
  using(var storage=BeginParameterMutation()){Checkpoint();target.Set(parameter,0);dirty=true;storage.Complete();score.RawStorageLayoutVerified=true;}
 }
}}
