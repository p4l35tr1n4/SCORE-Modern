using System;
namespace ScoreModern {
 internal sealed class NativePageInitialProposalResult {internal short[] Starts,Widths;}
 internal static class NativePageInitialProposal {
  static short Word(int value){return unchecked((short)value);}
  // PAGE overlay8, measured loaded at3197:000B..016A. Initial pass only;
  // the following optimisation and page-turn routines are separate stages.
  internal static NativePageInitialProposalResult Apply(short[] weights,short total,short requestedLines){
   if(weights==null||weights.Length==0||weights.Length>1000)throw new NotSupportedException("PAGE proposal has no verified bar-weight bank.");
   int lines=requestedLines>0?requestedLines:1;if(lines>=200)throw new NotSupportedException("PAGE proposal reaches its original two-hundred-line error boundary.");
   var starts=new short[lines+1];var widths=new short[lines];starts[0]=1;short remaining=total;int index=1;
   for(int line=0;line<lines;line++){
    short target=Word(remaining/(lines-line));short sum=weights[index-1];
    for(index++;index<=weights.Length;index++){
     short next=weights[index-1];short midpoint=Word(sum+next/2);if(midpoint>=target)break;sum=Word(sum+next);
    }
    widths[line]=sum;starts[line+1]=Word(index);remaining=Word(remaining-sum);
    if(index>weights.Length&&line+1<lines)throw new NotSupportedException("PAGE initial proposal reads beyond the provided weight bank.");
   }
   return new NativePageInitialProposalResult{Starts=starts,Widths=widths};
  }
 }
}
