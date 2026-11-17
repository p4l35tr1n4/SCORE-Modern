using System;using System.IO;using System.Linq;using System.Drawing;using System.Windows.Forms;
namespace ScoreModern {
 public static class NativeUiFont {
  public static string Family{get;private set;}
  static NativeUiFont(){Family="Consolas";}
  static float lineRatio;
  public static float LineRatio{get{if(lineRatio==0)using(var font=new FontFamily(Family))lineRatio=(float)font.GetLineSpacing(FontStyle.Regular)/font.GetEmHeight(FontStyle.Regular);return lineRatio;}}
  public static void Load(){Family="Consolas";try{string name=NativeSettings.Read("ui-font");if(!String.IsNullOrEmpty(name))Set(name,false);}catch(IOException){}catch(UnauthorizedAccessException){}catch(ArgumentException){}}
  internal static float WindowsTextScale {
   get {try{object raw=Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Accessibility","TextScaleFactor",100);return raw is int?Math.Max(100,Math.Min(225,(int)raw))/100f:1f;}catch(System.Security.SecurityException){return 1f;}}
  }
  internal static int RowHeight(float pixels,bool compact,bool vga){
   using(var family=new FontFamily(Family)){
    float em=family.GetEmHeight(FontStyle.Regular);
    float leading=family.GetLineSpacing(FontStyle.Regular)/em;
    float cell=(family.GetCellAscent(FontStyle.Regular)+family.GetCellDescent(FontStyle.Regular))/em;
    int glyph=(int)Math.Ceiling(pixels*(vga?leading:Math.Max(leading,cell)));
    return compact?glyph:glyph+Math.Max(2,(int)Math.Ceiling(pixels*.14f));
   }
  }
  public static Font Create(float size,GraphicsUnit unit=GraphicsUnit.Pixel){return new Font(Family,size,FontStyle.Regular,unit);}
  public static bool Matches(Font font){return font!=null&&String.Equals(font.FontFamily.Name,Family,StringComparison.OrdinalIgnoreCase);}
  public static void Set(string family,bool save){using(var candidate=new FontFamily(family)){if(!candidate.IsStyleAvailable(FontStyle.Regular))throw new ArgumentException("Choose a font with a regular face.");family=candidate.Name;}if(save)NativeSettings.WriteAllText("ui-font",family);Family=family;lineRatio=0;foreach(var editor in Application.OpenForms.OfType<Editor>().ToArray())editor.ApplyUiFont();}
  public static void Choose(Form owner){using(var dialog=new ScoreDialog("ui-font")){dialog.Text="Interface font";dialog.StartPosition=FormStartPosition.CenterParent;dialog.ClientSize=new Size(460,180);var panel=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,Padding=new Padding(12)};panel.Controls.Add(new Label{AutoSize=true,Text="Interface only. Notation keeps its SCORE Type 1 fonts.\nSize follows Windows text size. Compact text adjusts row spacing."});var names=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=420};names.Items.AddRange(FontFamily.Families.Where(f=>f.IsStyleAvailable(FontStyle.Regular)).Select(f=>f.Name).Distinct().OrderBy(n=>n).Cast<object>().ToArray());names.SelectedItem=Family;panel.Controls.Add(names);var preview=new Label{AutoSize=true,Text="SCORE  Par#  1/11  2/12  0123456789"};Font shown=null;names.SelectedIndexChanged+=(s,e)=>{if(names.SelectedItem==null)return;var next=new Font((string)names.SelectedItem,12);preview.Font=next;var old=shown;shown=next;if(old!=null)old.Dispose();};panel.Controls.Add(preview);var buttons=new FlowLayoutPanel{AutoSize=true,WrapContents=false};var apply=new Button{Text="Apply",DialogResult=DialogResult.OK,AutoSize=true};var cancel=new Button{Text="Cancel",DialogResult=DialogResult.Cancel,AutoSize=true};buttons.Controls.Add(apply);buttons.Controls.Add(cancel);panel.Controls.Add(buttons);dialog.Controls.Add(panel);dialog.AcceptButton=apply;dialog.CancelButton=cancel;try{if(dialog.ShowDialog(owner)==DialogResult.OK&&names.SelectedItem!=null){Set((string)names.SelectedItem,true);NativeInterfaceText.SetConsolas(true);}}finally{preview.Font=dialog.Font;if(shown!=null)shown.Dispose();}}}
 }
 public sealed partial class Editor {
  internal void ApplyUiFont(){var old=Font;Font=NativeUiFont.Create(12,GraphicsUnit.Point);LayoutClassic();UpdateClassic();Invalidate(true);if(old!=null)old.Dispose();}
 }
}
