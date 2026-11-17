// Verified title rows extracted from the supplied SCORE 3.11 executable. No EXE/font data is bundled.
using System.IO;
namespace ScoreModern {
 internal static class NativeEmbeddedTitle {
  internal static OriginalTitle Read(string root){return new OriginalTitle{Source=Path.Combine(root,"SCORE.EXE"),Version="3.11",Items=1420,Parameters=14200,Vectors=31000,Rows=new string[]{
"",
"",
"                                 SCORE (tm)",
"",
"                      Computer Music Typography System",
"",
"                                   from",
"                             San Andreas Press",
"                       Box 60247, Palo Alto, CA, 94306",
"",
"                             Version: 3.11     ",
"",
"                          Written by Leland Smith",
"",
"                  Programmers: Leland Smith, Perry Devine",
"                    Copyright 1977-1994 by Leland Smith",
"                Copyright 1986-1991 by Passport Designs, Inc.",
"                            All Rights Reserved.",
"",
"",
"   Program limits: 1420 items, 14200 parameters, 31000 graphics vectors.",
"",
"",
"",
"**** reading SCORE data ****",
}};}

  // Startup text and columns captured from the supplied SCOR4 4.01 executable.
  internal static OriginalTitle ReadScor4(){
   return new OriginalTitle {
    Source="SCOR4 4.01",Version="4.01",Items=1420,Parameters=14200,Vectors=31000,
    Rows=new[]{
     "",
     "",
     "",
     "                                 SCORE (tm)",
     "",
     "                      Computer Music Typography System",
     "",
     "                                   from",
     "                             San Andreas Press",
     "                       Box 60247, Palo Alto, CA, 94306",
     "",
     "                               Version: 4.01",
     "",
     "                          Written by Leland Smith",
     "",
     "                    Copyright 1977-2001 by Leland Smith",
     "                            All Rights Reserved.",
     "",
     "",
     "   Program limits: 1420 items, 14200 parameters, 31000 graphics vectors.",
     "",
     "",
     "",
     "**** reading SCORE data ****",
     "",
    }
   };
  }
 }
}
