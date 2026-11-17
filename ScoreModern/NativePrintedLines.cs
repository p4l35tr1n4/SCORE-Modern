using System;using System.Collections.Generic;using System.Drawing;
namespace ScoreModern {
 public static class NativePrintedLines {
  public static bool Draw(Graphics g,Item item,NativeNotationContext page){
   if(page==null||item.Code!=4||item.P(5)>=999)return false;
   if(item.P(7)==100||item.P(7)<=-2){
    bool multiPoint=item.P(7)==100;
    float strokeWidth=NativePrintContext.DefaultLineWidth;
    var waveGeometry=item;
    if(!multiPoint){
     if(Single.IsNaN(item.P(10))||Math.Abs(item.P(10))>32767)return false;
     strokeWidth=NativePrintContext.Width(unchecked((short)((short)item.P(10)*2)));
     waveGeometry=item.Clone();
     waveGeometry.Set(10,0);
    }
    var shared=multiPoint?NativeDatLines.MultiPoint(item,page):NativeDatLines.Waves(waveGeometry,page);
    var path=shared.ConvertAll(p=>new BarlinePoint(p.X,p.Y,p.Operation));
    NativeNoteDetails.Path(g,page,path,strokeWidth,true);
    if(!multiPoint&&item.P(10)!=0)NativePrintContext.RestoreDefaultLineWidth();
    return true;
   }
   if(NativePrintedFilledWave.Draw(g,item,page))return true;bool dashed=item.P(7)>=1&&item.P(7)<=6&&item.P(7)==(int)item.P(7);
   if(!dashed&&item.P(7)!=0)return false;
   if(dashed){
    // Printer P10 selects a stroke width; DAT P10 expands copied paths.
    // Keep the source item unchanged and reuse only the measured dash endpoints.
    if(Math.Abs(item.P(10))>32767||Single.IsNaN(item.P(10)))return false;
    for(int n=11;n<=20;n++)if(n!=11&&n!=12&&n!=13&&n!=14&&item.P(n)!=0)return false;
   }else{
    if(Math.Abs(item.P(10))>32767||Single.IsNaN(item.P(10)))return false;
    for(int n=15;n<=20;n++)if(item.P(n)!=0)return false;
   }
   var points=new List<BarlinePoint>();
   var geometry=item.Clone();
   geometry.Set(10,0);
   if(!dashed)geometry.Set(8,0);
   var vectors=dashed?NativeDatLines.Dashes(geometry,page):NativeDatLines.Straight(geometry,page);
   foreach(var p in vectors)points.Add(new BarlinePoint(p.X,p.Y,p.Operation));
   float width=NativePrintContext.Width(unchecked((short)((short)item.P(10)*2)));
   RenderResourceAudit.Record(dashed?"printed dashed line":"printed straight line","default","original shared Code4 endpoints","word coordinates and printer stroke selector twice cached P10");
   NativeNoteDetails.Path(g,page,points,width,true);if(item.P(10)!=0)NativePrintContext.RestoreDefaultLineWidth();return true;
  }
 }
}


