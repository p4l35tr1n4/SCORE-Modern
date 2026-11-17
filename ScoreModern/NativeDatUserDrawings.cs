using System;using System.IO;using System.Text;using System.Collections.Generic;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeDatUserDrawings {
  static float F(double x){return (float)x;}
  public static Glyph Load(Item i,DrawingLibrary library,out int selector){
   if(i.Bytes.Length<80&&i.ExternalDrawingNameOverride==null||i.P(13)!=0&&!i.ExternalDrawingResetResolved)throw new NotSupportedException("User drawing requires a stored filename without reset.");
   selector=(short)(int)i.P(5);if(selector<0)throw new NotSupportedException("User drawing bank selector must be nonnegative.");int bank=selector/10;selector%=10;string name=NativeDrawingNames.Name(i);if(name.Length==0)throw new NotSupportedException("User drawing filename is empty.");if(name.IndexOf(':')>=0||Path.IsPathRooted(name)||name.IndexOf('/')>=0||name.IndexOf('\\')>=0)throw new NotSupportedException("User drawing filename outside the measured local/library route.");
   if(bank!=0){string stem=Path.GetFileNameWithoutExtension(name),extension=Path.GetExtension(name);if(stem.Length==0||!((stem[stem.Length-1]>='A'&&stem[stem.Length-1]<='Z'&&stem[stem.Length-1]+bank<='Z')||(stem[stem.Length-1]>='0'&&stem[stem.Length-1]<='9'&&stem[stem.Length-1]+bank<='9')))throw new NotSupportedException("Named DRW bank exceeds its verified alphabetic/numeric series.");name=stem.Substring(0,stem.Length-1)+(char)(stem[stem.Length-1]+bank)+extension;}
   var selected=NativeLibraryLocation.Get(library.Root);string local=Path.Combine(Directory.GetCurrentDirectory(),name),chosen=selected==null?null:Path.Combine(selected.Folder,name);string path=chosen!=null&&File.Exists(chosen)?chosen:local;if(!File.Exists(path))path=Path.Combine(library.Root,name);if(!File.Exists(path))path=NativeLibraryLocation.FileFor(library.Root,name);return DrawingLibrary.Load(path)[selector];
  }
  public static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext p,DrawingLibrary library){
   return Prepare(i,p,library,true);
  }
  internal static List<PrimitiveTextPoint> Prepare(Item i,NativeNotationContext p,DrawingLibrary library,bool applyThickness){
   for(int n=3;n<=13;n++)if(Single.IsNaN(i.P(n))||Single.IsInfinity(i.P(n)))throw new NotSupportedException("DAT user drawing nonfinite controls need fault verification.");
   if(i.Bytes.Length<80&&i.ExternalDrawingNameOverride==null||i.P(13)!=0&&!i.ExternalDrawingResetResolved)throw new NotSupportedException("DAT user drawing requires a stored filename without reset.");
   int selector;var glyph=Load(i,library,out selector);float h=i.P(6)==0?1:i.P(6),v=i.P(7)==0?h:i.P(7);h=F((double)h*p.Scale);v=F((double)v*p.Scale);var points=new List<PrimitiveTextPoint>();
   for(int n=0;n<glyph.Points.Length;n++)points.Add(new PrimitiveTextPoint{X=NativeNotationContext.Word(F((double)glyph.Points[n].X*h+p.Prepared.X),true),Y=NativeNotationContext.Word(F((double)glyph.Points[n].Y*v+p.Prepared.Y),false),Operation=glyph.Operations[n]});
   if((short)(int)i.P(9)!=0)NativeDatLines.Rotate(points,i.P(9),p.Prepared.X,p.Prepared.Y);short width=(short)(int)i.P(8);if(width==0&&(short)(int)i.P(10)>0)width=unchecked((short)((short)(int)i.P(10)*101));if(applyThickness)NativeDatLines.Thickness(points,width);RenderResourceAudit.Record("DAT user drawing",selector+":"+glyph.Name,glyph.Source,"external DRW; word-buffer projection");return points;
  }
 }
}
