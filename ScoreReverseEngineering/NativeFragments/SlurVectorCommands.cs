using System;using System.Collections.Generic;
namespace ScoreModern.RecoveredFragments {
 public struct SlurPlotRequest {public short Kind,X,Y;public SlurPlotRequest(short k,short x,short y){Kind=k;X=x;Y=y;}}
 public static class SlurVectorCommands {
  // The original PostScript buffer reader treats a marker followed by a word
  // as a packet. The following X word is a coordinate even when it resembles
  // another marker. This matters at the lower coordinate clamp.
  public static List<SlurPlotRequest> Decode(SlurVectorBuffer b,int root,bool appendRestore,bool printed){
   var result=new List<SlurPlotRequest>();int originalEnd=b.End,end=originalEnd+(appendRestore?1:0);bool pending=true;
   for(int i=root;i<end;i++){
    short x=appendRestore&&i==originalEnd?unchecked((short)0x80a7):b.X[i];short kind=0;
    if(printed?x< -32600:x<= -32600){
     short command=unchecked((short)(x+0x7fbc));
     if(command==98||command==99){result.Add(new SlurPlotRequest(command,0,b.Y[i-1]));pending=true;continue;}
     if(command<90){i++;x=appendRestore&&i==originalEnd?unchecked((short)0x80a7):b.X[i];if(command>0){if(command>3)throw new NotSupportedException("Slur marker "+command);kind=command;}}
     else throw new NotSupportedException("Slur marker "+command);
    }
    if(kind==0&&pending)kind=1;result.Add(new SlurPlotRequest(kind,x,b.Y[i-1]));pending=false;
   }
   return result;
  }
 }
}
