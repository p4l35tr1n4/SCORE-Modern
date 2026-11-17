using System;using System.IO;using System.Collections.Generic;using System.Drawing;
namespace ScoreModern {
 // Diagnostic provenance only: resource availability does not certify SCORE's
 // selection, placement, scaling, or branch semantics.
 public sealed class RenderResourceAudit:IDisposable {
  [ThreadStatic] public static RenderResourceAudit Current;
  public readonly List<string> Events=new List<string>();public int Unresolved;public bool Strict;int ordinal,code;string parameters;
  public RenderResourceAudit(bool strict){if(Current!=null)throw new InvalidOperationException("Nested resource audit");Strict=strict;Current=this;}
  public void Item(Item item){ordinal++;code=item.Code;var values=new List<string>();for(int i=1;i<=item.Count;i++)values.Add(item.P(i).ToString("R",System.Globalization.CultureInfo.InvariantCulture));parameters=String.Join(",",values.ToArray());Record("item",item.Code.ToString(),"", "selection/geometry verification separate");}
  public static void Record(string kind,string id,string resource,string state){var a=Current;if(a!=null)a.Events.Add(a.ordinal+"\t"+a.code+"\t"+kind+"\t"+Clean(id)+"\t"+Clean(resource)+"\t"+Clean(state)+"\t"+a.parameters);}
  static string Clean(string s){return (s??"").Replace("\t"," ").Replace("\r"," ").Replace("\n"," ");}
  public static bool Route(string name,Func<bool> draw){bool handled=draw();Record("render route",name,"",handled?"handled":"declined");return handled;}
  public static void Fallback(string name){Record("legacy renderer",name,"Canvas","approximate/unverified route; resource availability is not geometry verification");}
  public static bool WindowsText(string text){var a=Current;if(a==null)return false;a.Unresolved++;Record("unresolved Windows notation text",text,"","not permitted in external-resource-only comparison");if(a.Strict)throw new NotSupportedException("Resource audit rejected Windows notation text: "+text);return true;}
  public static void Unavailable(string resource,string reason){var a=Current;if(a==null)return;a.Unresolved++;Record("unavailable resource",resource,"",reason);if(a.Strict)throw new NotSupportedException(reason);}
  public static int Run(string[] args){try{if(args.Length<4)throw new ArgumentException("--audit-render SCORE_ROOT MUS_FILE OUTPUT_TSV");using(var c=new Canvas())using(var image=new Bitmap(1,1))using(var g=Graphics.FromImage(image))using(var audit=new RenderResourceAudit(false)){c.Score=Score.Load(args[2]);c.Library=new DrawingLibrary(args[1]);c.TextLayout=new ScoreText(args[1]);c.LayoutScore();foreach(string warning in c.Library.Warnings)Record("library warning","",args[1],warning);try{c.Draw(g,false);}finally{audit.Save(args[3]);}Console.WriteLine("Resource audit: "+audit.Events.Count+" events; "+audit.Unresolved+" unresolved draw requests; "+c.Library.Warnings.Count+" library warnings. Resource presence does not certify original geometry.");return audit.Unresolved==0?0:2;}}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
  public void Save(string file){File.WriteAllLines(file,new[]{"item\tcode\tkind\tid\tresource\tstate\tparameters"});File.AppendAllLines(file,Events);}
  public void Dispose(){if(Current==this)Current=null;}
 }
}

