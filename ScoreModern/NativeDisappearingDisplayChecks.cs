using System;using System.Drawing;
namespace ScoreModern {public sealed partial class Editor {
 void CheckOriginalZooms(){
  var oldMode=NativeNotationView.Mode;short oldX=NativeDosViewport.OriginX,oldY=NativeDosViewport.OriginY,oldDivisor=NativeDosViewport.Divisor;float oldFactor=NativeDosViewport.ZoomFactor;
  try{NativeDosViewport.Reset();NativeNotationView.Set(NotationView.DatViewport,false);string[] commands={"SH","Z 1","Z","Z 1","SHX","Z","Z 1"};short[] ys={-47,-47,-71,-47,-47,-81,-47};
   for(int n=0;n<commands.Length;n++){Execute(commands[n]);if(NativeDosViewport.OriginX!=20||NativeDosViewport.OriginY!=ys[n]||NativeDosViewport.Divisor!=50)throw new Exception("Original viewport differs after "+commands[n]+": "+NativeDosViewport.OriginX+","+NativeDosViewport.OriginY+","+NativeDosViewport.Divisor);}
  }finally{NativeDosViewport.OriginX=oldX;NativeDosViewport.OriginY=oldY;NativeDosViewport.Divisor=oldDivisor;NativeDosViewport.ZoomFactor=oldFactor;NativeDosViewport.DisplayChanged();NativeNotationView.Set(oldMode,false);canvas.LayoutScore();}
 }
 void CheckDatFontPlane(string metadata){
  string[] fields=System.IO.File.ReadAllText(metadata).TrimEnd().Split('\t');if(fields.Length!=8)throw new Exception("Invalid original font-plane fixture.");var oldMode=NativeNotationView.Mode;short oldX=NativeDosViewport.OriginX,oldY=NativeDosViewport.OriginY,oldDivisor=NativeDosViewport.Divisor;int oldCrop=canvas.CropTop;
  try{NativeNotationView.Set(NotationView.DatViewport,false);canvas.CropTop=0;NativeDosViewport.OriginX=short.Parse(fields[1]);NativeDosViewport.OriginY=short.Parse(fields[2]);NativeDosViewport.Divisor=short.Parse(fields[3]);NativeDosViewport.DisplayChanged();byte[] actual=NativeDosViewport.Snapshot(canvas),expected=System.IO.File.ReadAllBytes(fields[0]);int changed=0;for(int y=int.Parse(fields[6]);y<int.Parse(fields[7]);y++)for(int x=int.Parse(fields[4]);x<int.Parse(fields[5]);x++)if(((actual[y*80+x/8]^expected[y*80+x/8])&(128>>(x%8)))!=0)changed++;if(changed!=0){System.IO.File.WriteAllBytes(metadata+".actual.bin",actual);var differences=new System.Collections.Generic.List<string>();for(int y=int.Parse(fields[6]);y<int.Parse(fields[7]);y++)for(int x=int.Parse(fields[4]);x<int.Parse(fields[5]);x++)if(((actual[y*80+x/8]^expected[y*80+x/8])&(128>>(x%8)))!=0)differences.Add(x+"\t"+y+"\t"+((expected[y*80+x/8]>>(7-x%8))&1)+"\t"+((actual[y*80+x/8]>>(7-x%8))&1));System.IO.File.WriteAllLines(metadata+".differences.tsv",differences);throw new Exception("Original display pixels differ: "+changed);}
  }finally{NativeDosViewport.OriginX=oldX;NativeDosViewport.OriginY=oldY;NativeDosViewport.Divisor=oldDivisor;canvas.CropTop=oldCrop;NativeDosViewport.DisplayChanged();NativeNotationView.Set(oldMode,false);canvas.LayoutScore();}
 }
 void CheckDisappearingDisplay(){
  if(canvas.DisappearingStaffs.Count!=32)throw new Exception("Display test requires all staffs hidden.");
  var old=NativeNotationView.Mode;
  try{foreach(NotationView view in Enum.GetValues(typeof(NotationView))){NativeNotationView.Set(view,false);canvas.LayoutScore();int width=NativeNotationView.IsDosViewport?640:(int)Math.Ceiling(canvas.ViewWidth),height=NativeNotationView.IsDosViewport?365:(int)Math.Ceiling(canvas.ViewHeight);
    using(var image=new Bitmap(width,height))using(var graphics=Graphics.FromImage(image)){graphics.Clear(Color.White);canvas.Draw(graphics,false);NativeDosOverlays.DrawVector(canvas,graphics);for(int y=0;y<height;y++)for(int x=1;x<width-1;x++){Color c=image.GetPixel(x,y);if(c.R+c.G+c.B<384)throw new Exception("Hidden notation remains in "+view+" at "+x+","+y);}}
   }
   NativeNotationView.Set(NotationView.Vector,false);canvas.LayoutScore();using(var image=new Bitmap((int)Math.Ceiling(canvas.PageWidth),(int)Math.Ceiling(canvas.PageHeight)))using(var graphics=Graphics.FromImage(image)){graphics.Clear(Color.White);NativeNotationView.Print(canvas,graphics);bool ink=false;for(int y=0;y<image.Height&&!ink;y++)for(int x=0;x<image.Width;x++){Color c=image.GetPixel(x,y);if(c.R+c.G+c.B<384){ink=true;break;}}if(!ink)throw new Exception("Display-only DP mask leaked into printing.");}
  }finally{NativeNotationView.Set(old,false);canvas.LayoutScore();}
 }
}}
