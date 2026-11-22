using System.Reflection;
[assembly: AssemblyTitle("SCORE Modern")]
[assembly: AssemblyProduct("SCORE Modern")]
[assembly: AssemblyVersion(ScoreModern.BuildVersion.FileVersion)]
[assembly: AssemblyFileVersion(ScoreModern.BuildVersion.FileVersion)]
[assembly: AssemblyInformationalVersion(ScoreModern.BuildVersion.Version)]
namespace ScoreModern {
 public static class BuildVersion {
  public const string Version="0.3.67";
  public const string FileVersion=Version+".0";
  public const string CumulativeBuild="2427";
  public const string NativeBuild="";
  public static string BuildNumber{get{return NativeBuild.Length==0?CumulativeBuild:NativeBuild;}}
  public static string DisplayVersion{get{return Product+"  |  Build #"+BuildNumber;}}
  public const string Product="SCORE Modern "+Version+" RC3";
 }
}


