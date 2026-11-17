using System;
using System.IO;
using System.Linq;
using System.Drawing;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeBeamChecks {
  public static int Verify(string samples){try{
   int files=0,roundTrips=0,beams=0,shapes=0,tremolos=0,zeroTypes=0;var unsupported=new System.Collections.Generic.List<string>();
   foreach(string path in Directory.GetFiles(samples,"*.mus",SearchOption.AllDirectories)){
    byte[] original=File.ReadAllBytes(path);try{var score=Score.Load(path);if(!original.SequenceEqual(score.Encode()))throw new Exception("MUS round-trip mismatch: "+path);roundTrips++;}catch(InvalidDataException e){unsupported.Add(path+": "+e.Message);}files++;var beamItems=new System.Collections.Generic.List<Item>();int offset=2;while(original.Length-offset>24){int count=(int)Math.Round(BitConverter.ToSingle(original,offset));if(count<1||offset+(count+1)*4>original.Length-20)throw new Exception("Invalid framing: "+path);byte[] record=new byte[(count+1)*4];Array.Copy(original,offset,record,0,record.Length);if(BitConverter.ToSingle(record,4)==6)beamItems.Add(new Item(record));offset+=record.Length;}
    foreach(var item in beamItems){
     bool tremolo=item.P(7)<0||(int)item.P(7)%10==0;if(item.P(7)==0){zeroTypes++;continue;}if(tremolo)tremolos++;
     float[] p=new float[21];for(int n=1;n<=20;n++)p[n]=item.P(n);
     var context=new BeamGeometryContext{Scale=25,LeftVertical=p[4]*175,Vertical=v=>v*175};
     foreach(var mode in new[]{BeamAppearance.Printed,BeamAppearance.DosScreen}){
      var geometry=BeamGeometry.Build(p,context,mode);if(geometry.Count==0&&!tremolo)throw new Exception("Missing beam geometry: "+path);
      if(geometry.Any(s=>s.Vertices.Length!=4||s.Vertices.Any(v=>float.IsNaN(v.X)||float.IsInfinity(v.X)||float.IsNaN(v.Y)||float.IsInfinity(v.Y))))throw new Exception("Invalid beam geometry: "+path);
      if(mode==BeamAppearance.Printed)shapes+=geometry.Count;
     }if(!tremolo)beams++;
    }
   }
   int[] dark=new int[2];Item beam=Item.Make(6,1,5,0,0,15,12);
   for(int n=0;n<2;n++){NativeBeamDisplay.Set(n==0?BeamAppearance.Printed:BeamAppearance.DosScreen,false);using(var bitmap=new Bitmap(1200,1000))using(var g=Graphics.FromImage(bitmap)){g.Clear(Color.White);g.ScaleTransform(4,4);if(!NativeBeamDisplay.Draw(g,beam,5,180))throw new Exception("Native renderer rejected beam");for(int y=0;y<1000;y++)for(int x=0;x<1200;x++)if(bitmap.GetPixel(x,y).R<100)dark[n]++;}}
   if(dark[0]<=dark[1]||dark[1]==0)throw new Exception("Printed/DOS modes did not produce distinct expected thicknesses");
   NativeBeamDisplay.Set(BeamAppearance.Printed,false);
   NativeDisplayChecks.VerifyEmbeddedResources();
   Console.WriteLine("PASS: "+files+" framed sample files; "+roundTrips+" exact app MUS round-trips; "+beams+" ordinary beams plus "+tremolos+" tremolo records / "+shapes+" polygons accepted in both display modes; "+zeroTypes+" zero-P7 records excluded; distinct native render modes; the app embeds the authorized title logo and program icon. Corpus acceptance is not an original-output equivalence proof.");foreach(string message in unsupported)Console.WriteLine("UNSUPPORTED BY EXISTING APP LOADER: "+message);return 0;
  }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 }
}
