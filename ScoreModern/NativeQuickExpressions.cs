using System;using System.Collections.Generic;using System.IO;
namespace ScoreModern {public sealed partial class Editor {
 // Ordinary original command captures: batch-4/expressions-original.
 // Keep this Main dispatch separate from CR curvature and F2 mark aliases.
 static readonly Dictionary<string,int> measuredExpressions=new Dictionary<string,int>{{"AC",68},{"AR",67},{"AO",67},{"AT",78},{"CR",63},{"DI",64},{"F",56},{"FF",57},{"FFF",58},{"FP",60},{"MF",55},{"MP",54},{"ME",80},{"P",53},{"PP",52},{"PPP",51},{"PI",81},{"PZ",66},{"RIT",65},{"RA",113},{"SF",61},{"SFZ",62}};
 bool ExecuteQuickExpression(string verb,string arguments){if(verb=="RI")return true;int symbol;if(!measuredExpressions.TryGetValue(verb,out symbol))return false;
  var values=LayoutNumbers(arguments);if(values.Length>19)throw new NotSupportedException("This expression argument form still requires original verification; at most nineteen numeric controls are currently verified.");
  foreach(float value in values)if(Single.IsNaN(value)||Single.IsInfinity(value))throw new InvalidDataException("Expression parameters must be finite.");
  var parameters=new float[Math.Max(5,values.Length+1)];parameters[0]=9;parameters[1]=1;Array.Copy(values,0,parameters,1,values.Length);parameters[4]=symbol;var item=Item.Make(parameters);item.TrimEditedNumericParameters();Checkpoint();score.Items.Add(item);dirty=true;RefreshAll(null);return true;
 }
}}
