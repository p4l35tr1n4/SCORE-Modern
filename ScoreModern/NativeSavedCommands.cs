using System;using System.Collections.Generic;using System.Linq;
namespace ScoreModern {public sealed partial class Editor {
 readonly string[] originalSavedCommands=new string[3];bool replayingSavedCommand;
 bool ExecuteOriginalSavedCommands(string text){
  if(replayingSavedCommand||(mode!=ScreenMode.Main&&mode!=ScreenMode.Edit))return false;string value=text.Trim();if(value.Length==0)return false;
  if(value.All(c=>c==';')){if(value.Length==4){if(mode==ScreenMode.Edit){ExitEdit(false);Message("");return true;}Message(String.Join(Environment.NewLine,Enumerable.Range(0,3).Select(i=>new string(';',i+1).PadRight(4)+(originalSavedCommands[i]??""))));return true;}if(value.Length>3)throw new NotSupportedException("Saved-command selector beyond the original three slots is prevented.");RunOriginalSavedCommand(value.Length-1);return true;}
  if(value[0]!='\''&&value[0]!=':')return false;var parsed=new string[3];int offset=0,firstSlot=-1;string firstBody=null;
  while(offset<value.Length){int count=0;while(offset<value.Length&&(value[offset]=='\''||value[offset]==':')){count++;offset++;}if(count<1||count>3)throw new NotSupportedException("Saved commands use one, two or three prefix characters.");int start=offset;while(offset<value.Length&&value[offset]!='\''&&value[offset]!=':')offset++;string body=value.Substring(start,offset-start).TrimEnd();if(body.Length==0||Char.IsWhiteSpace(body[0])||body.Length>78)throw new NotSupportedException("Saved-command body leaves the verified one-line, 78-character domain.");if(firstSlot<0){firstSlot=count-1;firstBody=body;}parsed[count-1]=body;}
  for(int i=0;i<3;i++)if(parsed[i]!=null)originalSavedCommands[i]=parsed[i];RunOriginalSavedCommandBody(firstBody);return true;
 }
 void RunOriginalSavedCommand(int slot){string body=originalSavedCommands[slot];if(String.IsNullOrEmpty(body))return;RunOriginalSavedCommandBody(body);}
 void RunOriginalSavedCommandBody(string body){replayingSavedCommand=true;try{Execute(body);}finally{replayingSavedCommand=false;}}
}}
